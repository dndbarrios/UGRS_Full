﻿using System;
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
using SAPbouiCOM;

namespace UGRS.AddOn.CreditNote.Forms
{
    [FormAttribute("UGRS.AddOn.CreditNote.Forms.frmCreditNote", "Forms/frmCreditNote.b1f")]
    class frmCreditNote : UserFormBase
    {
        #region Properties
        CreditNoteT mObjCreditNoteT = new CreditNoteT();
        CreditNoteFactory mObjCreditNoteFactory = new CreditNoteFactory();
        StatusEnum mEnumStatus;
        int mIntNcCode = 0;
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
            this.btnReport.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btnReport_ClickAfter);
            this.mtxInv = ((SAPbouiCOM.Matrix)(this.GetItem("mtxInv").Specific));
            this.mtxInv.LinkPressedAfter += new SAPbouiCOM._IMatrixEvents_LinkPressedAfterEventHandler(this.mtxInv_LinkPressedAfter);
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
            UGRS.Core.SDK.UI.UIApplication.GetApplication().MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(this.TicketFrom_ApplicationMenuEvent);
            this.lblProgress = ((SAPbouiCOM.StaticText)(this.GetItem("lblProgres").Specific));
            this.btnCancel = ((SAPbouiCOM.Button)(this.GetItem("btnCancel").Specific));
            this.btnCancel.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btnCancel_ClickAfter);
            this.OnCustomInitialize();

        }

        private void TicketFrom_ApplicationMenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (!pVal.BeforeAction && UIApplication.GetApplication().Forms.ActiveForm.UniqueID == this.UIAPIRawForm.UniqueID)
                {
                    Menu(pVal.MenuUID);
                }
            }
            catch (Exception ex)
            {

                LogService.WriteError(string.Format("TicketFrom_ApplicationMenuEvent: {0}", ex.Message));
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format("MenuEventException: {0}", ex.Message));
            }

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
            btnReport.Item.Enabled = false;
            btnCancel.Item.Enabled = false;
            btnAttach.Item.Enabled = false;
            txtFolio.Item.Enabled = false;
            loadMenu();
        }

        #endregion

        #region Events

        private void btnSearch_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (btnSearch.Item.Enabled)
            {
                SearchValue();
            }
        }

        private void btnNC_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (btnNC.Item.Enabled)
            {
                try
                {
                    SaveNC_UDT lObjSaveNC = new SaveNC_UDT();
                    GetCN_List lObjGetList = new GetCN_List(DtMatrix, txtDateFrom.Value, txtDateTo.Value);
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
                            Autorize(lObjSaveNC, lObjCreditNoteTSaved, lStrNcId);
                            break;

                       
                        case StatusEnum.Authorized:
                            SaveCreditNote(lObjSaveNC, lObjGetList, lObjCreditNoteTSaved, lStrNcId);
                            break;

                        case StatusEnum.PendignNC:
                            SaveCreditNote(lObjSaveNC, lObjGetList, lObjCreditNoteTSaved, lStrNcId);
                            break;

                        case StatusEnum.NcOk:
                            UpdateDocRel(lObjSaveNC, lStrNcId);
                            break;

                        case StatusEnum.PendignDocRel:
                            UpdateDocRel(lObjSaveNC, lStrNcId);
                            break;

                        case StatusEnum.DocRelOk:
                            UpdateDocument(lObjSaveNC, lStrNcId);
                            break;

                        case StatusEnum.Processed:
                            UIApplication.ShowMessageBox("Notas de crédito ya procesadas");
                            break;
                    }
                    LoadReport(lStrNcId);
                }
                catch (Exception ex)
                {
                    LogService.WriteError(ex);
                    UIApplication.ShowMessageBox(ex.Message);
                }
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
            if (btnAttach.Item.Enabled)
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
                                LoadReport(mObjCreditNoteT.NcId);
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
        }


        private void btnCancel_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (btnCancel.Item.Enabled)
            {
                try
                {
                    CancelReport();
                }
                catch (Exception ex)
                {
                    LogService.WriteError(ex);
                }
            }
        }

         private void btnReport_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            printUDO(txtFolio.Value);

        }

         private void mtxInv_LinkPressedAfter(object sboObject, SBOItemEventArg pVal)
         {
             try
             {
                 //ColUID = "C_DocEntrJ"

                 if (pVal.ColUID == "C_CardCode")
                 {
                     string lStrCardCode = DtMatrix.GetValue("C_CardCode", pVal.Row - 1).ToString();
                     SAPbouiCOM.Framework.Application.SBO_Application.OpenForm(BoFormObjectEnum.fo_BusinessPartner, "", lStrCardCode);
                 }
                 if (pVal.ColUID == "C_DocNum")
                 {
                     string lStrDocEntry = DtMatrix.GetValue("C_DocEntry", pVal.Row - 1).ToString();
                     SAPbouiCOM.Framework.Application.SBO_Application.OpenForm(BoFormObjectEnum.fo_Invoice, "", lStrDocEntry);
                 }
             }
             catch (Exception ex)
             {
                 LogService.WriteError("mtxReceipt_LinkPressedBefore: " + ex.Message);
                 LogService.WriteError(ex);
             }
         }
        #endregion

        #region Methods

        /// <summary>
        /// Establece el menu
        /// </summary>
        private void loadMenu()
        {
            this.UIAPIRawForm.EnableMenu("520", true); // Print
            this.UIAPIRawForm.EnableMenu("6659", false);  // Fax
            this.UIAPIRawForm.EnableMenu("1281", true); // Search Record
            this.UIAPIRawForm.EnableMenu("1282", true); // Add New Record
            this.UIAPIRawForm.EnableMenu("1288", true);  // Next Record
            this.UIAPIRawForm.EnableMenu("1289", true);  // Pevious Record
            this.UIAPIRawForm.EnableMenu("1290", true);  // First Record
            this.UIAPIRawForm.EnableMenu("1291", true);  // Last record
        }


        #region Menu
        private void Menu(string pStrMenuUID)
        {
            txtDateFrom.Item.Click();
            switch (pStrMenuUID)
            {
                case "1281": // Search Record
                    MenuSearchRecord();
                    break;

                case "1282": // Add New Record
                    MenuNewReport();
                    break;

                case "1288": // Next Record
                    MenuNextRecord();
                    break;

                case "1289": // Preview Record
                    MenuPreviewRecord();
                    break;

                case "1290": // First Record
                    MenuFirstRecord();
                    break;

                case "1291": // Last record 
                    MenuLastRecord();
                    break;
            }
        }
       
        private void MenuNewReport()
        {
            mIntNcCode = 0;

          
            mObjCreditNoteT = null;
            DtMatrix = this.UIAPIRawForm.DataSources.DataTables.Item("Dt_INV");
            DtMatrix.Rows.Clear();
            BindMatrix();
            txtFolio.Value = string.Empty;
            txtAttach.Value = string.Empty;
            txtDateFrom.Value = string.Empty;
            txtDateTo.Value = string.Empty;
            txtDateTo.Item.Enabled = true;
            txtDateFrom.Item.Enabled = true;
            txtDateFrom.Item.Click();
            txtFolio.Item.Enabled = false;
            btnSearch.Item.Enabled = true;
            btnAttach.Item.Enabled = false;
            btnNC.Item.Enabled = true;
            btnNC.Caption = "Guardar reporte";
            btnReport.Item.Enabled = false;
            btnCancel.Item.Enabled = false;

            mEnumStatus = SetStatus(mObjCreditNoteT);
            string pStrStatus = mEnumStatus.GetDescription();
            lblStatus.Caption = string.Format("Estatus: {0}", pStrStatus);
        }

        private void MenuSearchRecord()
        {
            this.UIAPIRawForm.Mode = SAPbouiCOM.BoFormMode.fm_VIEW_MODE;
            txtFolio.Item.Enabled = true;
            txtFolio.Value = "";
            txtFolio.Item.Click();
        }

        private void MenuNextRecord()
        {
            txtFolio.Item.Enabled = false;
            this.UIAPIRawForm.Mode = SAPbouiCOM.BoFormMode.fm_VIEW_MODE;

            if ((mIntNcCode + 1) > mObjCreditNoteFactory.GetCreditNoteService().GetLastCode())
            {
                UIApplication.ShowWarning(string.Format("Primer registro de datos"));
                LoadReport("NC_" + mObjCreditNoteFactory.GetCreditNoteService().GetFirstCode());
            }
            else
            {
                LoadReport("NC_" + (mIntNcCode + 1));
            }
        }

        private void MenuPreviewRecord()
        {
           
            txtFolio.Item.Enabled = false;
            this.UIAPIRawForm.Mode = SAPbouiCOM.BoFormMode.fm_VIEW_MODE;
            if ((mIntNcCode - 1 < mObjCreditNoteFactory.GetCreditNoteService().GetFirstCode()))
            {
                UIApplication.ShowWarning(string.Format("Ultimo registro de datos"));
                LoadReport("NC_" + mObjCreditNoteFactory.GetCreditNoteService().GetLastCode());
            }
            else
            {
                LoadReport("NC_" + (mIntNcCode - 1));
            }
        }

        private void MenuFirstRecord()
        {
            txtFolio.Item.Enabled = false;
            this.UIAPIRawForm.Mode = SAPbouiCOM.BoFormMode.fm_VIEW_MODE;
            LoadReport("NC_" + mObjCreditNoteFactory.GetCreditNoteService().GetFirstCode());
        }

        private void MenuLastRecord()
        {
            txtFolio.Item.Enabled = false;
            this.UIAPIRawForm.Mode = SAPbouiCOM.BoFormMode.fm_VIEW_MODE;
            LoadReport("NC_" + mObjCreditNoteFactory.GetCreditNoteService().GetLastCode());
        }
        #endregion Menu

        #region Save
        private void SaveReport(SaveNC_UDT pObjSaveNC, GetCN_List pObjGetList)
        {
            if (btnNC.Item.Enabled && SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("¿Desea guardar el reporte de Notas de crédito?", 2, "Si", "No", "") == 1)
            {

                //Obtener datos para guardar reporte
                string pStrId = pObjGetList.GetId();

                UIApplication.ShowWarning("Guardando reporte" + pStrId);
                LogService.WriteInfo("Guardando reporte" + pStrId);
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

        private void Autorize(SaveNC_UDT pObjSaveNC, CreditNoteT pObjCreditNoteTSaved, string pStrNcId)
        {
            //Guardado de Notas de credito
            pObjCreditNoteTSaved = VerifyStatus(pStrNcId);
            if (!string.IsNullOrEmpty(pObjCreditNoteTSaved.Attach))
            {
                if (SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("¿Desea autorizar el reporte?", 2, "Si", "No", "") == 1)
                {
                    pObjCreditNoteTSaved.IsAutorized = "Y";
                    pObjCreditNoteTSaved.UserMod = DIApplication.Company.UserName;
                    pObjCreditNoteTSaved.ModificationDate = DateTime.Now;
                    pObjCreditNoteTSaved.ModificationTime = DateTime.Now.ToString("hhmm");

                    if (pObjSaveNC.UpdateTinUDT(pObjCreditNoteTSaved) == 0)
                    {
                        UIApplication.ShowSuccess("Autorizado correctamente");
                    }
                    else
                    {
                        string lStrError = string.Format("Fallo al autorizar {0}", DIApplication.Company.GetLastErrorDescription());
                        UIApplication.ShowMessageBox(lStrError);
                        LogService.WriteError(lStrError);
                    }
                }
            }
            else
            {
                UIApplication.ShowMessageBox("Favor de adjuntar el reporte");
            }
        }

        //Guarda preliminares
        private void SaveCreditNote(SaveNC_UDT pObjSaveNC, GetCN_List pObjGetList, CreditNoteT pObjCreditNoteTSaved, string pStrNcId)
        {
            if (SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("¿Desea iniciar la generacion de Notas de crédito?", 2, "Si", "No", "") == 1)
            {
                pObjCreditNoteTSaved = VerifyStatus(pStrNcId);
                UIApplication.ShowWarning("Guardando Notas de credito " + pStrNcId);
                LogService.WriteInfo("Guardando Nota de credito " + pStrNcId);
                List<string> LstResult = pObjSaveNC.SaveCreditNoteDoc(pObjCreditNoteTSaved.LstCreditNoteDoc);

                if (LstResult.Count == 0)
                {
                    //llamado Actualiza documentos preliminares
                    pObjCreditNoteTSaved = VerifyStatus(pStrNcId);
                    UIApplication.ShowSuccess("Guardado Notas de credito realizado correctamente " + pStrNcId);
                    LogService.WriteInfo("Guardado Notas de credito realizado correctamente " + pStrNcId);
                    UpdateDocRel(pObjSaveNC, pStrNcId);
                }
                else
                {
                    ShowMessageboxList("Algunas notas no fueron generadas correctamente: ", LstResult);
                }
            }

        }

        //Actualiza documentos relacionados
        private List<string> UpdateDocRel(SaveNC_UDT pObjSaveNC, string pStrNcId)
        {
            CreditNoteT lObjCreditNoteTSaved = new CreditNoteT();
            GetCN_List lObjGetList = new GetCN_List(DtMatrix, txtDateTo.Value, txtDateTo.Value);
            lObjCreditNoteTSaved = VerifyStatus(pStrNcId);

            UIApplication.ShowWarning("Actualizando documentos relacionados " + pStrNcId);
            
            LogService.WriteInfo("Actualizando documentos relacionados " + pStrNcId);
            pObjSaveNC.UpdateDocRel(lObjCreditNoteTSaved.LstCreditNoteDoc);
           
            //Verifica DocRel
            lObjCreditNoteTSaved = VerifyStatus(pStrNcId);

            UIApplication.ShowWarning("Verificando documentos relacionados " + pStrNcId);
            LogService.WriteInfo("Verificando documentos relacionados " + pStrNcId);
            //lObjCreditNoteTSaved = lObjGetList.GetCreditNoteTSaved(pStrId);
            List<string> lLstError = pObjSaveNC.ValidateCreditNoteRelation(lObjGetList.GetDraftReference(pStrNcId), lObjCreditNoteTSaved);
            if (lLstError.Count() > 0)
            {
                ShowMessageboxList("Algunos facturas no fueron relacionadas correctamente: ", lLstError);
            }
            else
            {
                //llama el guardado de preliminares a nc
                UpdateDocument(pObjSaveNC, pStrNcId);
            }
            return lLstError;
        }

        //Actualiza Notas de credito a relevante
        private List<string> UpdateDocument(SaveNC_UDT pObjSaveNC, string pStrNcId)
        {
            CreditNoteT lObjCreditNoteTSaved = new CreditNoteT();
            lObjCreditNoteTSaved = VerifyStatus(pStrNcId);
            //Guarda Nota de credito desde borrador

            UIApplication.ShowWarning("Actualizando notas de credito a relevante " + pStrNcId);
            LogService.WriteInfo("Actualizando notas de credito a relevante " + pStrNcId);
            List<string> lLstErrorDoc = pObjSaveNC.UpdateDocument(lObjCreditNoteTSaved);
            if (lLstErrorDoc.Count() > 0)
            { 
                ShowMessageboxList("No fue posible generar algunos documentos ", lLstErrorDoc);
            }
            else
            {
                UIApplication.ShowMessageBox("Proceso generado correctamente");
                //llama al borrado de preliminares
               // DeleteDraft(pObjSaveNC, pStrNcId);
            }
            return lLstErrorDoc;
        }

        //Borrado de preliminares
        //private void DeleteDraft(SaveNC_UDT pObjSaveNC, string pStrNcId)
        //{
        //    CreditNoteT lObjCreditNoteTSaved = new CreditNoteT();
        //    lObjCreditNoteTSaved = VerifyStatus(pStrNcId);
        //    List<string> lLstErrorDoc = pObjSaveNC.RemoveDraft(lObjCreditNoteTSaved);
        //    if (lLstErrorDoc.Count() > 0)
        //    {
        //        ShowMessageboxList("No fue posible borrar los documentos preliminar", lLstErrorDoc);
        //    }
        //    else
        //    {
        //        //llama al borrado de preliminares

        //    }

        //}

        private CreditNoteT VerifyStatus(string pStrNcId)
        {
            UIApplication.ShowMessage("Consultando reporte " + pStrNcId);
            GetCN_List lObjNC_List = new GetCN_List(DtMatrix, txtDateFrom.Value, txtDateTo.Value);
            return lObjNC_List.GetCreditNoteTSaved(pStrNcId);
        }

        private void ShowMessageboxList(string pStrMessage, List<string> pLstError)
        {
            string lStrMessageError = string.Format(pStrMessage + "\n{0}",
                       string.Join("\n", pLstError.Select(x => string.Format("{0}", x)).ToArray()));
            LogService.WriteError(lStrMessageError);
            SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(lStrMessageError);
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


        private void CancelReport()
        {
            if (SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("¿Desea cancelar el reporte de Notas de crédito?", 2, "Si", "No", "") == 1)
            {
                CreditNoteT lObjCreditNoteT = VerifyStatus(txtFolio.Value);

                if (lObjCreditNoteT.LstCreditNoteDoc.Where(x => x.IsDocument == "Y").Count() > 0)
                {
                    UIApplication.ShowMessageBox("Existen notas de credito ya generadas no es posible cancelar ");
                }
                else
                {
                    SaveNC_UDT lObjSaveNC = new SaveNC_UDT();
                    lObjSaveNC.CancelReport(lObjCreditNoteT);
                    LoadReport(txtFolio.Value);
                }
            }
        }
        #endregion

        #region Search
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
            string lStrDateFrom = txtDateFrom.Value;
            string lStrDateTo = txtDateTo.Value;
            DtMatrix.ExecuteQuery(mObjCreditNoteFactory.GetCreditNoteService().GetInvoiceQuery(lStrDateFrom, lStrDateTo));
        }

        /// <summary>
        /// Asigna las columnas del datatable a la matriz
        /// </summary>
        private void BindMatrix()
        {
            DtMatrix = this.UIAPIRawForm.DataSources.DataTables.Item("Dt_INV");
            if (DtMatrix.Rows.Count > 0)
            {
                foreach (SAPbouiCOM.Column item in mtxInv.Columns)
                {
                    mtxInv.Columns.Item(item.UniqueID).DataBind.Bind("Dt_INV", item.UniqueID);
                }
                mtxInv.LoadFromDataSource();
                mtxInv.AutoResizeColumns();
            }
            else
            {
                UIApplication.ShowError("No se econtraron registros");
            }

        }


        private void LoadReport(string pStrNcId)
        {
            try
            {
                if (!string.IsNullOrEmpty(pStrNcId))
                {
                    this.UIAPIRawForm.Freeze(true);

                    UIApplication.ShowWarning("Cargando reporte " + pStrNcId);
                    LogService.WriteInfo("Cargando reporte " + pStrNcId);

                    mIntNcCode = string.IsNullOrEmpty(pStrNcId) ? 0 : Convert.ToInt32(pStrNcId.Substring(3));
                    GetCN_List lObjNC_List = new GetCN_List(DtMatrix, txtDateFrom.Value, txtDateTo.Value);
                    mObjCreditNoteT = lObjNC_List.GetCreditNoteTSaved(pStrNcId);
                    if (mObjCreditNoteT != null)
                    {
                        DtMatrix = this.UIAPIRawForm.DataSources.DataTables.Item("Dt_INV");
                        DtMatrix.Rows.Clear();
                        DtMatrix.ExecuteQuery(mObjCreditNoteFactory.GetCreditNoteService().GetReportSavedQuery(pStrNcId));
                        BindMatrix();
                        txtFolio.Value = pStrNcId;
                        txtAttach.Value = mObjCreditNoteT.Attach;
                        txtDateFrom.Value = mObjCreditNoteT.DateFrom.ToString("yyyyMMdd");
                        txtDateTo.Value = mObjCreditNoteT.DateTo.ToString("yyyyMMdd");
                        // txtFolio.Item.Click();
                        txtDateTo.Item.Enabled = true;
                        txtDateFrom.Item.Enabled = true;
                        btnSearch.Item.Enabled = false;
                        btnAttach.Item.Enabled = true;
                        btnNC.Item.Enabled = true;
                        btnReport.Item.Enabled = true;
                        btnCancel.Item.Enabled = true;

                        mEnumStatus = SetStatus(mObjCreditNoteT);

                        switch (mEnumStatus)
                        {
                            case StatusEnum.PendingAutorized:
                                btnNC.Caption = "Autorizar";
                                break;
                            case StatusEnum.PendingReport:
                                btnNC.Caption = "Guardar reporte";
                                break;
                            default:
                                btnNC.Caption = "Generar NC";
                                break;
                        }
                        string pStrStatus = mEnumStatus.GetDescription();
                        lblStatus.Caption = string.Format("Estatus: {0}", pStrStatus);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex);
                this.UIAPIRawForm.Freeze(false);
                UIApplication.ShowError(ex.Message);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        private StatusEnum SetStatus(CreditNoteT pObjCreditNoteT)
        {
            //Sin guardar
            StatusEnum lEnum = StatusEnum.PendingReport;

            if (pObjCreditNoteT != null)
            {

                lEnum = StatusEnum.PendignNC;
                //Cancelado
                if (pObjCreditNoteT.IsCanceled == "Y")
                {
                    lEnum = StatusEnum.Canceled;
                    btnNC.Item.Enabled = false;
                    btnReport.Item.Enabled = false;
                    btnCancel.Item.Enabled = false;
                    btnAttach.Item.Enabled = false;
                    btnSearch.Item.Enabled = false;
                }

                //Autorizado
                if (pObjCreditNoteT.IsAutorized == "N")
                {
                    lEnum = StatusEnum.PendingAutorized;
                }

                //Autorizado
                if (pObjCreditNoteT.IsAutorized == "Y")
                {
                    lEnum = StatusEnum.Authorized;
                }

                ////Borrador
                //if (pObjCreditNoteT.LstCreditNoteDoc.Where(x => x.IsDraft == "Y").Count() > 0 && pObjCreditNoteT.LstCreditNoteDoc.Where(x => x.IsDraft == "N").Count() > 0)
                //{
                //    lEnum = StatusEnum.PendingDraft;
                //}
                ////Borrador ok
                //if (pObjCreditNoteT.LstCreditNoteDoc.Where(X => X.IsDraft == "Y").Count() == pObjCreditNoteT.LstCreditNoteDoc.Count())
                //{
                //    lEnum = StatusEnum.DraftOk;
                //}

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


                //Procesado
                if (pObjCreditNoteT.LstCreditNoteDoc.Where(x => x.IsProcessed == "Y").Count() > 0 && pObjCreditNoteT.LstCreditNoteDoc.Where(x => x.IsProcessed == "N").Count() > 0)
                {
                    lEnum = StatusEnum.PendingUpdateNcGenerate;
                }

                //Procesado
                if (pObjCreditNoteT.LstCreditNoteDoc.Where(X => X.IsProcessed == "Y").Count() == pObjCreditNoteT.LstCreditNoteDoc.Count())
                {
                    lEnum = StatusEnum.Processed;
                }

            }
            return lEnum;
        }
        #endregion


        public void printUDO(string pStrNcId)
        {
            try
            {
                // execute menu and enter document number
                string lStrMenu = mObjCreditNoteFactory.GetCreditNoteService().GetMenuId();
                UIApplication.GetApplication().ActivateMenuItem(lStrMenu);
                Form lObjForm = UIApplication.GetApplication().Forms.ActiveForm;
                ((EditText)lObjForm.Items.Item("1000003").Specific).String = pStrNcId;
                lObjForm.Items.Item("1").Click(BoCellClickType.ct_Regular); // abrir reporte
            }
            catch (Exception ex)
            {
                LogService.WriteError("printUDO " + ex.Message);
                LogService.WriteError(ex);
            }
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
        private SAPbouiCOM.StaticText lblProgress;
        private SAPbouiCOM.Button btnCancel;
        #endregion





    }
}
  
