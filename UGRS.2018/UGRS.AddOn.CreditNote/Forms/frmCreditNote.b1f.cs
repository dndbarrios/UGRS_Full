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
using UGRS.Core.Utility;
using System.IO;
using UGRS.Core.SDK.DI.CreditNote.DOC;
using UGRS.Core.SDK.DI.CreditNote.Enum;
using UGRS.Core.Extension.Enum;

namespace UGRS.AddOn.CreditNote.Forms
{
    [FormAttribute("UGRS.AddOn.CreditNote.Forms.frmCreditNote", "Forms/frmCreditNote.b1f")]
    class frmCreditNote : UserFormBase
    {
        #region Properties
        CreditNoteT mObjCreditNoteT = new CreditNoteT();
        CreditNoteFactory mObjCreditNoteFactory = new CreditNoteFactory();
        StatusEnum mEnumStatus;
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
            this.btnAttach.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btnAttach_ClickAfter);
            this.txtAttach = ((SAPbouiCOM.EditText)(this.GetItem("txtAttach").Specific));
            this.lblInfo = ((SAPbouiCOM.StaticText)(this.GetItem("lblInfo").Specific));
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
            mEnumStatus = StatusEnum.PendingReport;
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

                string lStrNcId = txtFolio.Value;
                switch (mEnumStatus)
                {  
                    case StatusEnum.Canceled:
                        UIApplication.ShowMessageBox("Reporte cancelado");
                        break;

                    case StatusEnum.PendingReport:
                        SaveReport(lObjSaveNC, lObjGetList);
                        break;

                    case StatusEnum.PendingAutorized:
                        UIApplication.ShowMessageBox("Falta autorizacion");
                        break;

                    case StatusEnum.Authorized:
                        SaveDraft(lObjSaveNC, lObjGetList, lObjCreditNoteTSaved, lStrNcId);
                        break;

                    case StatusEnum.PendingDraft:
                        SaveDraft(lObjSaveNC, lObjGetList, lObjCreditNoteTSaved, lStrNcId);
                        break;

                    case StatusEnum.DraftOk:
                        UpdateDocRel(lObjSaveNC, lStrNcId);
                        break;

                    case StatusEnum.PendignDocRel:
                        UpdateDocRel(lObjSaveNC, lStrNcId);
                        break;

                    case StatusEnum.DocRelOk:
                        SaveDraftToDocument(lObjSaveNC, lStrNcId);
                        break;

                    case StatusEnum.PendignNC:
                        SaveDraftToDocument(lObjSaveNC, lStrNcId);
                        break;
                        
                    case StatusEnum.NcOk:
                        DeleteDraft();
                        break;

                    case StatusEnum.PendingDelDraft:
                        DeleteDraft();
                        break;

                    case StatusEnum.DelDraftOK:
                        VerifyStatus(lStrNcId);
                        break;

                    case StatusEnum.Processed:
                         UIApplication.ShowMessageBox("Notas de crédito ya procesadas");
                        break;
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


        private void btnAttach_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
               
                SelectFileDialogUtility lObjDialog = new SelectFileDialogUtility(Core.Utility.DialogType.OPEN, "", "");
                lObjDialog.Open();
                if (!string.IsNullOrEmpty(lObjDialog.SelectedFile))
                {
                    string lStrAttach = AttatchFile(lObjDialog.SelectedFile);
                    if (!string.IsNullOrEmpty(lStrAttach))
                    {
                        mObjCreditNoteT.Attach = lStrAttach;
                        if (mObjCreditNoteFactory.GetCreditNoteTService().Update(mObjCreditNoteT) == 0)
                        {
                            txtAttach.Value = lStrAttach;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("btnAttach_ClickAfter: {0}", ex.Message));
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format("btnAttach_ClickAfter: {0}", ex.Message));
            }
        }
        #endregion

        #region Methods

        private void SaveReport(SaveNC_UDT pObjSaveNC, GetCN_List pObjGetList)
        {
            if (btnNC.Item.Enabled && SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("¿Desea guardar el reporte de Notas de crédito?", 2, "Si", "No", "") == 1)
            {
                //Obtener datos para guardar reporte
                string pStrId = pObjGetList.GetId();
                List<CreditNoteDet> lLstCreditNoteDet = pObjGetList.GetMatrixData(pStrId);
                List<CreditNoteDoc> lLstCreditNoteDoc = pObjGetList.GetNC_Doc(pStrId, lLstCreditNoteDet);
                CreditNoteT lObjCreditNoteT = pObjGetList.GetNC_Header(pStrId, lLstCreditNoteDoc);

                //Guardado de reporte
                int lIntResult = pObjSaveNC.SaveInUDT(lObjCreditNoteT);
                if (lIntResult == 0)
                {
                    UIApplication.ShowSuccess("Reporte guardado correctamente");
                    LoadReport(pStrId);
                }
                else
                {
                    UIApplication.ShowError("No fue posible guardar el reporte favor de revisar el log");
                }
            }
        }

        //Guarda preliminares
        private void SaveDraft(SaveNC_UDT pObjSaveNC, GetCN_List pObjGetList, CreditNoteT pObjCreditNoteTSaved, string pStrNcId)
        {
            //Guardado de borrador
            pObjCreditNoteTSaved = VerifyStatus(pStrNcId);
            int lIntResult = pObjSaveNC.SaveCreditNoteDoc(pObjCreditNoteTSaved.LstCreditNoteDoc);

            if (lIntResult == 0)
            {
                //llamado Actualiza documentos preliminares
                pObjCreditNoteTSaved = VerifyStatus(pStrNcId);
                UpdateDocRel(pObjSaveNC, pStrNcId);
            }
        }

        //Actualiza documentos preliminares
        private List<string> UpdateDocRel(SaveNC_UDT pObjSaveNC,   string pStrNcId)
        {
            CreditNoteT lObjCreditNoteTSaved = new CreditNoteT();
            GetCN_List lObjGetList = new GetCN_List(DtMatrix, txtDateTo.Value);

            lObjCreditNoteTSaved = VerifyStatus(pStrNcId);
            pObjSaveNC.UpdateDocRel(lObjCreditNoteTSaved.LstCreditNoteDoc);
           
            //Verifica DocRel
            lObjCreditNoteTSaved = VerifyStatus(pStrNcId);
            //lObjCreditNoteTSaved = lObjGetList.GetCreditNoteTSaved(pStrId);
            List<string> lLstError = pObjSaveNC.ValidateDraftRelation(lObjGetList.GetDraftReference(pStrNcId), lObjCreditNoteTSaved);
            if (lLstError.Count() > 0)
            {
                ShowMessageboxList("Algunos facturas no fueron relacionadas correctamente:", lLstError);
            }
            else
            {
                //llama el guardado de preliminares a nc
                SaveDraftToDocument(pObjSaveNC, pStrNcId);
            }
            return lLstError;
        }

        //Guarda preliminares a nota de credito
        private List<string> SaveDraftToDocument(SaveNC_UDT pObjSaveNC, string pStrNcId)
        {
            CreditNoteT lObjCreditNoteTSaved = new CreditNoteT();
            lObjCreditNoteTSaved = VerifyStatus(pStrNcId);
            //Guarda Nota de credito desde borrador
            List<string> lLstErrorDoc = pObjSaveNC.SaveDraftToDocument(lObjCreditNoteTSaved);
            if (lLstErrorDoc.Count() > 0)
            {
                ShowMessageboxList("No fue posible generar algunos documentos", lLstErrorDoc);
            }
            else
            {
                //llama al borrado de preliminares
                DeleteDraft();
            }
            return lLstErrorDoc;
        }

        //Borrado de preliminares
        private void DeleteDraft()
        {

        }

        private CreditNoteT VerifyStatus(string pStrNcId)
        {
            GetCN_List lObjNC_List = new GetCN_List(DtMatrix, txtDateFrom.Value);
            return lObjNC_List.GetCreditNoteTSaved(pStrNcId);
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
            txtFolio.Value = pStrNcId;
            txtAttach.Value = mObjCreditNoteT.Attach;
            txtDateFrom.Value = mObjCreditNoteT.Date.ToString("yyyyMMdd");
            txtDateTo.Value = mObjCreditNoteT.Date.ToString("yyyyMMdd");
            txtFolio.Item.Click();
            txtDateTo.Item.Enabled = false;
            txtDateFrom.Item.Enabled = false;
            btnSearch.Item.Enabled = false;
            btnAttach.Item.Enabled = true;
           
            mEnumStatus = SetStatus(mObjCreditNoteT);
            string pStrStatus = mEnumStatus.GetDescription();
             lblStatus.Caption = string.Format("Estatus: {0}",  pStrStatus);
        }

        private string AttatchFile(string pStrFile)
        {
            int lIntAttachement = 0;
            string lStrAttach = string.Empty;
            string lStrAttachPath = mObjCreditNoteFactory.GetCreditNoteService().GetAttachPath();
            if (!string.IsNullOrEmpty(pStrFile))
            {

                if (!Directory.Exists(lStrAttachPath))
                {
                    if (SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(string.Format("Carpeta {0} \n no accesible es posible que no pueda adjuntar el xml ¿Desea continuar?", lStrAttachPath), 2, "Si", "No", "") == 2)
                    {
                        return "";
                    }
                }
            }
            CN_AttachDI lObjAttachmentDI = new CN_AttachDI();
            lIntAttachement = lObjAttachmentDI.AttachFile(pStrFile);
            if (lIntAttachement > 0)
            {
                lStrAttach = lStrAttachPath + System.IO.Path.GetFileName(pStrFile);
            }
            else
            {
                LogService.WriteError("InvoiceDI (AttachDocument) " + DIApplication.Company.GetLastErrorDescription());
                UIApplication.ShowError(string.Format("InvoiceDI (AttachDocument) : {0}", DIApplication.Company.GetLastErrorDescription()));
            }
            return lStrAttach;
        }

        private StatusEnum SetStatus(CreditNoteT pObjCreditNoteT)
        {
            //Sin guardar
            StatusEnum lEnum = StatusEnum.PendingReport;

            if (pObjCreditNoteT != null)
            {

                //Cancelado
                if (pObjCreditNoteT.IsCanceled == "Y")
                {
                    lEnum = StatusEnum.Canceled;
                }

                //Autorizado
                if (pObjCreditNoteT.IsAutorized == "Y")
                {
                    lEnum = StatusEnum.Authorized;
                }

                //Borrador
                if (pObjCreditNoteT.LstCreditNoteDoc.Where(x => x.IsDraft == "Y").Count() > 0 && pObjCreditNoteT.LstCreditNoteDoc.Where(x => x.IsDraft == "N").Count() > 0)
                {
                    lEnum = StatusEnum.PendingDraft;
                }
                //Borrador ok
                if (pObjCreditNoteT.LstCreditNoteDoc.Where(X => X.IsDraft == "Y").Count() == pObjCreditNoteT.LstCreditNoteDoc.Count())
                {
                    lEnum = StatusEnum.DraftOk;
                }

                //Documentos relacionados
                if (pObjCreditNoteT.LstCreditNoteDoc.Where(x => x.IsDocRel == "Y").Count() > 0 && pObjCreditNoteT.LstCreditNoteDoc.Where(x => x.IsDocRel == "N").Count() > 0)
                {
                    lEnum = StatusEnum.PendignDocRel;
                }
                //Documentos relacionados ok
                if (pObjCreditNoteT.LstCreditNoteDoc.Where(X => X.IsDocRel == "Y").Count() == pObjCreditNoteT.LstCreditNoteDoc.Count())
                {
                    lEnum = StatusEnum.DocRelOk;
                }

                //Nota de credito
                if (pObjCreditNoteT.LstCreditNoteDoc.Where(x => x.IsDocument == "Y").Count() > 0 && pObjCreditNoteT.LstCreditNoteDoc.Where(x => x.IsDocument == "N").Count() > 0)
                {
                    lEnum = StatusEnum.PendignNC;
                }
                //Nota de credito ok
                if (pObjCreditNoteT.LstCreditNoteDoc.Where(X => X.IsDocument == "Y").Count() == pObjCreditNoteT.LstCreditNoteDoc.Count())
                {
                    lEnum = StatusEnum.NcOk;
                }

                //Preliminar Borrado
                if (pObjCreditNoteT.LstCreditNoteDoc.Where(x => x.IsDelDraft == "Y").Count() > 0 && pObjCreditNoteT.LstCreditNoteDoc.Where(x => x.IsDelDraft == "N").Count() > 0)
                {
                    lEnum = StatusEnum.PendingDelDraft;
                }
                //Preliminar Borrado ok
                if (pObjCreditNoteT.LstCreditNoteDoc.Where(X => X.IsDelDraft == "Y").Count() == pObjCreditNoteT.LstCreditNoteDoc.Count())
                {
                    lEnum = StatusEnum.DelDraftOK;
                }

                if (pObjCreditNoteT.IsProcessed == "Y")
                {
                    lEnum = StatusEnum.Processed;
                }
            }
            return lEnum;
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
        private SAPbouiCOM.StaticText lblInfo;
        #endregion




    }
}
  
