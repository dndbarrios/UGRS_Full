using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using UGRS.Core.Services;
using UGRS.Core.SDK.UI;
using UGRS.Core.SDK.DI.CreditNote;
using UGRS.Core.SDK.DI.CreditNote.DTO;

namespace UGRS.AddOn.CreditNote.Forms
{
    [FormAttribute("UGRS.AddOn.CreditNote.Forms.frmCreditNote", "Forms/frmCreditNote.b1f")]
    class frmCreditNote : UserFormBase
    {
        CreditNoteFactory mObjCreditNoteFactory = new CreditNoteFactory();
        public frmCreditNote()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.txtDate = ((SAPbouiCOM.EditText)(this.GetItem("txtDate").Specific));
            this.btnNC = ((SAPbouiCOM.Button)(this.GetItem("btnNC").Specific));
            this.btnNC.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btnNC_ClickAfter);
            this.btnReport = ((SAPbouiCOM.Button)(this.GetItem("btnReport").Specific));
            this.mtxInv = ((SAPbouiCOM.Matrix)(this.GetItem("mtxInv").Specific));
            this.lblDate = ((SAPbouiCOM.StaticText)(this.GetItem("lblDate").Specific));
            this.btnSearch = ((SAPbouiCOM.Button)(this.GetItem("btnSearch").Specific));
            this.btnSearch.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSearch_ClickBefore);
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }

        private void OnCustomInitialize()
        {
          
        }

        private void SearchValue()
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);
                SetDataTableValues();
                BindMatrix();
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
                //mObjProgressBar.Dispose();
            }
        }

        /// <summary>
        /// Carga los pagos desde una consulta al datatable
        /// </summary>
        private void SetDataTableValues()
        {
            DtMatrix = this.UIAPIRawForm.DataSources.DataTables.Item("Dt_INV");
            DateTime lDtmDate = DateTime.Now;// Convert.ToDateTime(txtDate.Value);
            DtMatrix.ExecuteQuery(mObjCreditNoteFactory.GetCreditNoteService().GetInvoiceQuery(lDtmDate));
        }

        /// <summary>
        /// Asigna las columnas del datatable a la matriz
        /// </summary>
        private void BindMatrix()
        {
            foreach (SAPbouiCOM.Column item in mtxInv.Columns)
            {
                mtxInv.Columns.Item(item.UniqueID).DataBind.Bind("Dt_INV", item.UniqueID);
            }
            mtxInv.LoadFromDataSource();
            mtxInv.AutoResizeColumns();
        }


        private SAPbouiCOM.DataTable DtMatrix;
        private SAPbouiCOM.EditText txtDate;
        private SAPbouiCOM.Button btnNC;
        private SAPbouiCOM.Button btnReport;
        private SAPbouiCOM.Matrix mtxInv;
        private SAPbouiCOM.StaticText lblDate;
        private SAPbouiCOM.Button btnSearch;

        private void btnSearch_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            SearchValue();
        }

        private void btnNC_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            CreditNoteDTO lLstCreditNote = new CreditNoteDTO();

            mObjCreditNoteFactory.GetCreditNoteDocService().CreateCreditNote(lLstCreditNote);

        }
    }
}
