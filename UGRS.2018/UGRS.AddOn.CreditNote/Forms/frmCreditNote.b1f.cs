using System;
using System.Collections.Generic;
using SAPbouiCOM.Framework;
using UGRS.Core.Services;
using UGRS.Core.SDK.UI;
using UGRS.Core.SDK.DI.CreditNote;
using UGRS.Core.SDK.DI.CreditNote.Tables;
using System.Linq;
using UGRS.Core.SDK.DI;
using UGRS.AddOn.CreditNote.Services;

namespace UGRS.AddOn.CreditNote.Forms
{
    [FormAttribute("UGRS.AddOn.CreditNote.Forms.frmCreditNote", "Forms/frmCreditNote.b1f")]
    class frmCreditNote : UserFormBase
    {
        #region Properties
        CreditNoteT mObjCreditNoteT = new CreditNoteT();
        CreditNoteFactory mObjCreditNoteFactory = new CreditNoteFactory();
        #endregion

        #region Constructor
        public frmCreditNote()
        {

        }
        #endregion

        #region Initialize


        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.txtDateTo = ((SAPbouiCOM.EditText)(this.GetItem("txtDate").Specific));
            this.btnNC = ((SAPbouiCOM.Button)(this.GetItem("btnNC").Specific));
            this.btnNC.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btnNC_ClickAfter);
            this.btnReport = ((SAPbouiCOM.Button)(this.GetItem("btnReport").Specific));
            this.mtxInv = ((SAPbouiCOM.Matrix)(this.GetItem("mtxInv").Specific));
            this.lblDateTo = ((SAPbouiCOM.StaticText)(this.GetItem("lblDate").Specific));
            this.btnSearch = ((SAPbouiCOM.Button)(this.GetItem("btnSearch").Specific));
            this.btnSearch.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSearch_ClickBefore);
            this.UD_DateTo = this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_DateTo");
            this.UD_DateFrom = this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_DateFrm");
            this.txtDateFrom = ((SAPbouiCOM.EditText)(this.GetItem("txtDateFrm").Specific));
            this.lblDateFrom = ((SAPbouiCOM.StaticText)(this.GetItem("lblDateFrm").Specific));
            this.lblFolio = ((SAPbouiCOM.StaticText)(this.GetItem("lblFolio").Specific));
            this.txtFolio = ((SAPbouiCOM.EditText)(this.GetItem("txtFolio").Specific));
            this.txtFolio.KeyDownAfter += new SAPbouiCOM._IEditTextEvents_KeyDownAfterEventHandler(this.txtFolio_KeyDownAfter);
            this.lblStatus = ((SAPbouiCOM.StaticText)(this.GetItem("lblStatus").Specific));
            this.btnAttach = ((SAPbouiCOM.Button)(this.GetItem("btnAttach").Specific));
            this.txtAttach = ((SAPbouiCOM.EditText)(this.GetItem("txtAttach").Specific));
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

        #endregion

        #region Events

        private void btnSearch_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            SearchValue();
        }

        private void btnNC_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {

                SaveNC_UDT lObjSaveNC = new SaveNC_UDT();
                GetCN_List lObjGetList = new GetCN_List(DtMatrix, txtDateTo.Value);
                CreditNoteT lObjCreditNoteTSaved = new CreditNoteT();

                if (btnNC.Item.Enabled && SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("¿Desea guardar el reporte de Notas de crédito?", 2, "Si", "No", "") == 1)
                {
                    if (SaveReport(lObjSaveNC, lObjGetList) == 0)
                    {
                        UIApplication.ShowSuccess("Reporte guardado correctamente");
                    }
                    else
                    {
                        UIApplication.ShowError("No fue posible guardar el reporte favor de revisar el log");
                    }
                }

                
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
        }

        private void txtFolio_KeyDownAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);
                if (pVal.CharPressed == (char)System.Windows.Forms.Keys.Enter)
                {
                    LoadReport(txtFolio.Value);
                }
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                LogService.WriteError(string.Format("txtFolio_KeyDownAfter: {0}", ex.Message));
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format("txtFolio_KeyDownAfter: {0}", ex.Message));

            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }

        }
        #endregion

        #region Methods

        private int SaveReport(SaveNC_UDT pObjSaveNC, GetCN_List pObjGetList)
        {
            //Obtener datos para guardar reporte
            string pStrId = pObjGetList.GetId();
            List<CreditNoteDet> lLstCreditNoteDet = pObjGetList.GetMatrixData(pStrId);
            List<CreditNoteDoc> lLstCreditNoteDoc = pObjGetList.GetNC_Doc(pStrId, lLstCreditNoteDet);
            CreditNoteT lObjCreditNoteT = pObjGetList.GetNC_Header(pStrId, lLstCreditNoteDoc);

            //Guardado de reporte
           return pObjSaveNC.SaveInUDT(lObjCreditNoteT);
        }

        private void SaveDraft(SaveNC_UDT pObjSaveNC, GetCN_List pObjGetList, CreditNoteT pObjCreditNoteTSaved, string pStrNcId)
        {
            //Guardado de borrador
            pObjCreditNoteTSaved = pObjGetList.GetCreditNoteTSaved(pStrNcId);
            pObjSaveNC.SaveCreditNoteDoc(pObjCreditNoteTSaved.LstCreditNoteDoc);
        }

        private List<string> UpdateDocRel(SaveNC_UDT pObjSaveNC, GetCN_List pObjGetList, CreditNoteT pObjCreditNoteTSaved, string pStrNcId)
        {
            //Actualizacion de borrador
            //lObjCreditNoteTSaved = lObjGetList.GetCreditNoteTSaved(pStrId);
            //lObjSaveNC.UpdateDocRel(lObjCreditNoteTSaved.LstCreditNoteDoc);
            //Test
            pObjCreditNoteTSaved = pObjGetList.GetCreditNoteTSaved(pStrNcId);
            pObjSaveNC.UpdateDocRel(pObjCreditNoteTSaved.LstCreditNoteDoc);
            //lObjSaveNC.UpdateDocRel(lObjCreditNoteTSaved.LstCreditNoteDoc.Where(x => x.FolioDoc == "NC_4_23").ToList());

            //Actualiza UDT status 
            //Test
            pObjCreditNoteTSaved = pObjGetList.GetCreditNoteTSaved(pStrNcId);
            //lObjCreditNoteTSaved = lObjGetList.GetCreditNoteTSaved(pStrId);
            List<string> lLstError = pObjSaveNC.ValidateDraftRelation(pObjGetList.GetDraftReference(pStrNcId), pObjCreditNoteTSaved);
            if (lLstError.Count() > 0)
            {
                ShowMessageboxList("Algunos facturas no fueron relacionadas correctamente:", lLstError);
            }
            return lLstError;
        }

        private List<string> SaveDraftToDocument(SaveNC_UDT pObjSaveNC, CreditNoteT pObjCreditNoteTSaved)
        {
            //Guarda Nota de credito desde borrador
            List<string> lLstErrorDoc = pObjSaveNC.SaveDraftToDocument(pObjCreditNoteTSaved);
            if (lLstErrorDoc.Count() > 0)
            {
                ShowMessageboxList("No fue posible generar algunos documentos", lLstErrorDoc);
            }
            return lLstErrorDoc;
        }

        private void ShowMessageboxList(string pStrMessage, List<string> pLstError)
        {
            string lStrMessageError = string.Format(pStrMessage + "\n{0}",
                       string.Join("\n", pLstError.Select(x => string.Format("{0}", x)).ToArray()));
            LogService.WriteError(lStrMessageError);
            Application.SBO_Application.MessageBox(lStrMessageError);
        }


        /// <summary>
        /// Realiza la busqueda
        /// </summary>
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
        /// Carga los datos desde una consulta al datatable
        /// </summary>
        private void SetDataTableValues()
        {
            DtMatrix = this.UIAPIRawForm.DataSources.DataTables.Item("Dt_INV");
            DtMatrix.Rows.Clear();
            string lStrDateFrom = txtDateTo.Value;
            string lStrDateTo = txtDateTo.Value;
            DtMatrix.ExecuteQuery(mObjCreditNoteFactory.GetCreditNoteService().GetInvoiceQuery(lStrDateFrom, lStrDateTo));
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


        private void LoadReport(string pStrNcId)
        {
            GetCN_List lObjNC_List = new GetCN_List(DtMatrix, txtDateFrom.Value);
            mObjCreditNoteT = lObjNC_List.GetCreditNoteTSaved(pStrNcId);
            DtMatrix = this.UIAPIRawForm.DataSources.DataTables.Item("Dt_INV");
            DtMatrix.Rows.Clear();
            DtMatrix.ExecuteQuery(mObjCreditNoteFactory.GetCreditNoteService().GetReportSavedQuery(pStrNcId));
            BindMatrix();

            txtDateFrom.Value = mObjCreditNoteT.Date.ToString("yyyyMMdd");
            txtDateTo.Value = mObjCreditNoteT.Date.ToString("yyyyMMdd");
            txtDateTo.Item.Enabled = false;
            txtDateFrom.Item.Enabled = false;
            btnSearch.Item.Enabled = false;

        }

        #endregion

        #region Controls
        private SAPbouiCOM.DataTable DtMatrix;
        private SAPbouiCOM.EditText txtDateTo;
        private SAPbouiCOM.Button btnNC;
        private SAPbouiCOM.Button btnReport;
        private SAPbouiCOM.Matrix mtxInv;
        private SAPbouiCOM.StaticText lblDateTo;
        private SAPbouiCOM.Button btnSearch;
        private SAPbouiCOM.UserDataSource UD_DateTo;
        private SAPbouiCOM.UserDataSource UD_DateFrom;
        private SAPbouiCOM.EditText txtDateFrom;
        private SAPbouiCOM.StaticText lblDateFrom;
        private SAPbouiCOM.StaticText lblFolio;
        private SAPbouiCOM.EditText txtFolio;
        private SAPbouiCOM.StaticText lblStatus;
        private SAPbouiCOM.Button btnAttach;
        private SAPbouiCOM.EditText txtAttach;
        #endregion
       
    }
}
