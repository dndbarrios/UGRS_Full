using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using UGRS.Core.SDK.UI;
using UGRS.Core.Utility;
using UGRS.Core.SDK.DI.Purchases;
using UGRS.Core.SDK.DI;

namespace UGRS.AddOn.Purchases.Forms
{
    [FormAttribute("UGRS.AddOn.Purchases.Forms.frmModalAF", "Forms/frmModalAF.b1f")]
    class frmModalAF : UserFormBase
    {
        #region Properties
        PurchasesServiceFactory mObjPurchaseServiceFactory = new PurchasesServiceFactory();
        public string mStrAFCode = string.Empty;
        public string mStrAreaParam = string.Empty;
        public string mStrAFParam = string.Empty;
        public bool mBolForMatrix = false;
        #endregion

        #region Constructor
        public frmModalAF(string pStrArea = null, string pStrAFCode = null, bool pBolForMatrix = false)
        {
            mBolForMatrix = pBolForMatrix;
            mStrAreaParam = pStrArea;
            mStrAFParam = pStrAFCode;

            CreateDatatableAF();
            LoadAF(pStrArea, pStrAFCode);
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.btnSelect = ((SAPbouiCOM.Button)(this.GetItem("btnSelect").Specific));
            this.btnSelect.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSelect_ClickBefore);
            this.btnCancel = ((SAPbouiCOM.Button)(this.GetItem("btnCancel").Specific));
            this.btnCancel.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCancel_ClickBefore);
            this.mtxAF = ((SAPbouiCOM.Matrix)(this.GetItem("mtxAF").Specific));
            this.mtxAF.DoubleClickBefore += new SAPbouiCOM._IMatrixEvents_DoubleClickBeforeEventHandler(this.mtxAF_DoubleClickBefore);
            this.mtxAF.ClickBefore += new SAPbouiCOM._IMatrixEvents_ClickBeforeEventHandler(this.mtxAF_ClickBefore);
            this.lblSearch = ((SAPbouiCOM.StaticText)(this.GetItem("lblSearch").Specific));
            this.txtSearch = ((SAPbouiCOM.EditText)(this.GetItem("txtSearch").Specific));
            this.txtSearch.KeyDownAfter += new SAPbouiCOM._IEditTextEvents_KeyDownAfterEventHandler(this.txtSearch_KeyDownAfter);
            this.btnSearch = ((SAPbouiCOM.Button)(this.GetItem("btnSearch").Specific));
            this.btnSearch.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSearch_ClickBefore);
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.ResizeAfter += new ResizeAfterHandler(this.Form_ResizeAfter);

        }

        private void OnCustomInitialize()
        {

        }
        #endregion

        #region Events
        private void btnSearch_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            LoadAF(mStrAreaParam, txtSearch.Value);
        }

        private void btnSelect_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            SelectItem();
        }

        private void btnCancel_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.UIAPIRawForm.Close();
        }

        private void mtxAF_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            if(pVal.Row > 0)
                mtxAF.SelectRow(pVal.Row, true, false);
        }

        private void mtxAF_DoubleClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            SelectItem();
        }

        private void Form_ResizeAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            mtxAF.AutoResizeColumns();
        }

        private void txtSearch_KeyDownAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (pVal.CharPressed == 13)
            {
                LoadAF(mStrAreaParam, txtSearch.Value);
            }
        }
        #endregion

        #region Functions
        private void LoadAF(string pStrArea, string pStrAFCode)
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);
                UIApplication.ShowMessage("Buscando folios...");

                ClearMatrix();

                this.UIAPIRawForm.DataSources.DataTables.Item("DTAF").ExecuteQuery(mObjPurchaseServiceFactory.GetAssetService().GetAssetCFLQuery(pStrArea, pStrAFCode));

                mtxAF.AutoResizeColumns();
                mtxAF.LoadFromDataSource();
            }
            catch (Exception lObjException)
            {
                LogUtility.WriteError(String.Format("[frmModalAF - LoadAF] Error: {0}", lObjException.Message));
                UIApplication.GetApplication().MessageBox(string.Format("Error al obtener los activos fijos: {0}", lObjException.Message));
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        private void ClearMatrix()
        {
            if (!this.UIAPIRawForm.DataSources.DataTables.Item("DTAF").IsEmpty)
            {
                this.UIAPIRawForm.DataSources.DataTables.Item("DTAF").Rows.Clear();
                mtxAF.Clear();
            }
        }

        private void SelectItem()
        {
            int lIntRow = mtxAF.GetNextSelectedRow(0, SAPbouiCOM.BoOrderType.ot_SelectionOrder);
            if (lIntRow >= 0)
            {
                mStrAFCode = (mtxAF.Columns.Item(1).Cells.Item(lIntRow).Specific as SAPbouiCOM.EditText).Value.Trim();
                this.UIAPIRawForm.Close();
            }
        }

        private void CreateDatatableAF()
        {
            this.UIAPIRawForm.DataSources.DataTables.Add("DTAF");
            dtAF = this.UIAPIRawForm.DataSources.DataTables.Item("DTAF");
            dtAF.Columns.Add("#", SAPbouiCOM.BoFieldsType.ft_Integer);
            dtAF.Columns.Add("AFCode", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            dtAF.Columns.Add("AFDesc", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            dtAF.Columns.Add("DescItem", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            dtAF.Columns.Add("AreaCode", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            //dtAF.Columns.Add("AreaName", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);

            FillMatrixEmployees();
        }

        private void FillMatrixEmployees()
        {
            mtxAF.Columns.Item("#").DataBind.Bind("DTAF", "#");
            mtxAF.Columns.Item("ColCode").DataBind.Bind("DTAF", "AFCode");
            mtxAF.Columns.Item("ColDesc").DataBind.Bind("DTAF", "AFDesc");
            mtxAF.Columns.Item("ColDscItm").DataBind.Bind("DTAF", "DescItem");
            mtxAF.Columns.Item("ColArea").DataBind.Bind("DTAF", "AreaCode");
        }
        #endregion

        #region Controls
        private SAPbouiCOM.Button btnSelect;
        private SAPbouiCOM.Button btnCancel;
        private SAPbouiCOM.Matrix mtxAF;
        private SAPbouiCOM.StaticText lblSearch;
        private SAPbouiCOM.EditText txtSearch;
        private SAPbouiCOM.Button btnSearch;
        private SAPbouiCOM.DataTable dtAF;
        #endregion

    }
}
