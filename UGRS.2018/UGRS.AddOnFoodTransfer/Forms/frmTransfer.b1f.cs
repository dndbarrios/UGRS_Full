/*
 * Autor: LCC Abraham SaÚL Sandoval Meneses
 * Description: Transfer of Transit Inventory to Area Form
 * Date: 31/08/2018
 */


using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UGRS.AddOnFoodTransfer.Utils;
using UGRS.Core.SDK.DI.FoodTransfer.DAO;
using UGRS.Core.SDK.DI.FoodTransfer.DTO;
using UGRS.Core.SDK.DI.FoodTransfer.Services;
using UGRS.Core.SDK.UI;

namespace UGRS.AddOnFoodTransfer.Forms {
    [FormAttribute("UGRS.AddOnFoodTransfer.Forms.frmTransfer", "Forms/frmTransfer.b1f")]
    class frmTransfer : UserFormBase {

        #region Properties
        int selectedRow = 0;
        Dictionary<string, BoFieldsType> columns0;
        Dictionary<string, BoFieldsType> columns1;
        FoodTransferDAO foodTransferDAO = new FoodTransferDAO();
        PendingTransfer[] pendingTransfers = null;
        TransferItem[] transferItems = null;
        SeriesNumber[] seriesNumbers = null;
        bool resize = false;
        #endregion

        #region Constructor
        public frmTransfer() {

            try {
                Task.Factory.StartNew(FillMatrix0);
                Task.Run(() => PrepareMatrix1());
            }
            catch(AggregateException ae) {
                ae.Handle(e => {
                    SAPException.Handle(e, "Constructor");
                    return true;
                });
            }
        }
        #endregion

        #region Matrix
        public void FillMatrix0() {
            columns0 = new Dictionary<string, BoFieldsType>() { { "#", BoFieldsType.ft_ShortNumber }, { "Folio", BoFieldsType.ft_AlphaNumeric }, { "DocDate", BoFieldsType.ft_AlphaNumeric }, { "Comments", BoFieldsType.ft_AlphaNumeric } };
            dt0 = SAPMatrix.CreateDataTable("DT0", columns0, this);
            pendingTransfers = foodTransferDAO.GetPendingTransfers();
            this.UIAPIRawForm.Freeze(true);
            SAPMatrix.Fill("DT0", dt0, mtx0, columns0.Keys.ToList(), pendingTransfers);
            this.UIAPIRawForm.Freeze(false);
        }

        public void PrepareMatrix1() {
            columns1 = new Dictionary<string, BoFieldsType>() { { "#", BoFieldsType.ft_ShortNumber }, { "Item", BoFieldsType.ft_AlphaNumeric }, { "Desc", BoFieldsType.ft_AlphaNumeric }, { "Quantity", BoFieldsType.ft_AlphaNumeric }, { "Bags", BoFieldsType.ft_Float } };
            dt1 = SAPMatrix.CreateDataTable("DT1", columns1, this);
            mtx1.AutoResizeColumns();
        }
        #endregion

        #region Events
        public override void OnInitializeComponent() {
            this.mtx0 = ((Matrix)(this.GetItem("mtx0").Specific));
            this.mtx0.ClickAfter += new SAPbouiCOM._IMatrixEvents_ClickAfterEventHandler(this.mtx0_ClickAfter);
            this.mtx1 = ((Matrix)(this.GetItem("mtx1").Specific));
            this.btnAccept = ((Button)(this.GetItem("btnAccept").Specific));
            this.btnReturn = ((Button)(this.GetItem("btnReturn").Specific));
            this.lblComment = ((StaticText)(this.GetItem("lblComment").Specific));
            this.txtComment = ((EditText)(this.GetItem("txtComment").Specific));
            this.btnAccept.ClickBefore += new _IButtonEvents_ClickBeforeEventHandler(this.btnAccept_ClickBefore);
            this.btnReturn.ClickBefore += new _IButtonEvents_ClickBeforeEventHandler(this.btnReturn_ClickBefore);
            this.OnCustomInitialize();
        }

        public override void OnInitializeFormEvents() {
            this.ResizeAfter += new ResizeAfterHandler(this.Form_ResizeAfter);
        }

        private void OnCustomInitialize() { }

        private void btnAccept_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            if(!Object.ReferenceEquals(transferItems, null)) {
                CreateTransfer(false);
            }
        }

        private void btnReturn_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            if(!Object.ReferenceEquals(transferItems, null)) {
                CreateTransfer(true);
            }
        }

        private void Form_ResizeAfter(SBOItemEventArg pVal) {

            try {
                if(resize) {
                    UIAPIRawForm.Freeze(true);
                    mtx0.Item.Height = UIAPIRawForm.Height / 2 - 100;
                    mtx1.Item.Top = UIAPIRawForm.Height / 2 - 50;
                    mtx1.Item.Height = mtx0.Item.Height;
                    UIAPIRawForm.Freeze(false);
                }
                else {
                    resize = true;
                }
            }
            catch(Exception ex) {
                SAPException.Handle(ex, "Form_ResizeAfter");
            }

        }

        private void mtx0_ClickAfter(object sboObject, SBOItemEventArg pVal) {
            try {
                if(!SelectRow(mtx0, pVal.Row))
                    return;

                string docNum = dt0.GetValue("C_Folio", selectedRow - 1).ToString();
                string docEntry = pendingTransfers.Where(t => t.Folio == docNum).FirstOrDefault().DocEntry.ToString();
                transferItems = foodTransferDAO.GetTransferItems(docEntry);

                Task.Factory.StartNew(() => {
                    this.UIAPIRawForm.Freeze(true);
                    SAPMatrix.Fill("DT1", dt1, mtx1, columns1.Keys.ToList(), transferItems);
                    seriesNumbers = foodTransferDAO.GetSeriesNumbers(docEntry);
                    this.UIAPIRawForm.Freeze(false);
                });
            }
            catch(AggregateException ae) {
                ae.Handle(e => {
                    SAPException.Handle(e, "mtx0_ClickAfter(AggregateException)");
                    return true;
                });
            }
        }
        #endregion

        #region CreateTransfer
        private void CreateTransfer(bool isCancellation) {

            var transferDocument = new DocumentTransfer();

            try {
                transferDocument.Document = pendingTransfers[selectedRow - 1];
                transferDocument.Lines = transferItems;
                transferDocument.Series = seriesNumbers;

                if(!String.IsNullOrEmpty(txtComment.Value)) {
                    transferDocument.Document.Comments = txtComment.Value;
                }

                Task.Factory.StartNew(() => {
                    return StockTransferDI.CreateTransfer(transferDocument, isCancellation);
                }).ContinueWith(t => {
                    if(t.Result.Success) {

                        pendingTransfers = foodTransferDAO.GetPendingTransfers();
                        this.UIAPIRawForm.Freeze(true);
                        SAPMatrix.Fill("DT0", dt0, mtx0, columns0.Keys.ToList(), pendingTransfers);
                        this.UIAPIRawForm.Freeze(false);

                        if(isCancellation) {
                            AlertMessageDI.Create(new MessageDTO() {
                                UserCode = foodTransferDAO.GetUserCode(transferDocument.Document.UserID.ToString()),
                                Message = String.Format("Se ha rechazado la transferencia #{0} y se ha solicitado la trasnferencia de devolución #{1}", transferDocument.Document.Folio, t.Result.DocEntry)
                            });
                        }

                        SAPMatrix.ClearMtx(mtx1);
                        transferItems = null;
                        seriesNumbers = null;
                        transferDocument = null;
                    }

                    UIApplication.ShowMessageBox(t.Result.Message);
                });
            }
            catch(AggregateException ae) {
                ae.Handle(e => {
                    SAPException.Handle(e, "CreateTransfer");
                    return true;
                });
            }
        }
        #endregion

        #region Controls
        private Matrix mtx0;
        private Matrix mtx1;
        private Button btnAccept;
        private Button btnReturn;
        private StaticText lblComment;
        private EditText txtComment;
        private DataTable dt0;
        private DataTable dt1;
        #endregion

        #region SelectRow
        private bool SelectRow(Matrix mtx, int row) {
            if(row != 0) {
                mtx.SelectRow(row, true, false);
                selectedRow = row;
                return true;
            }
            else {
                mtx.SelectRow(row, false, false);
                return false;
            }
        }
        #endregion

    }
}

