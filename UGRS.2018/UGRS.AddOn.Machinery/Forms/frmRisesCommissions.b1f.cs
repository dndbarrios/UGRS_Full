using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using UGRS.Core.SDK.UI;
using UGRS.Core.SDK.DI.Machinery;
using UGRS.Core.SDK.DI.Machinery.Enums;
using UGRS.Core.SDK.DI.Machinery.DTO;
using UGRS.Core.SDK.DI;
using UGRS.AddOn.Machinery.Enums;
using UGRS.Core.SDK.DI.Machinery.Tables;
using UGRS.Core.Utility;

namespace UGRS.AddOn.Machinery.Forms
{
    [FormAttribute("UGRS.AddOn.Machinery.Forms.frmRisesCommissions", "Forms/frmRisesCommissions.b1f")]
    class frmRisesCommissions : UserFormBase
    {
        #region Properties
        private MachinerySeviceFactory mObjMachineryServiceFactory = null;
        private frmCFLFolios mObjFrmFolios = null;
        private string mStrSupervisorId = string.Empty;
        #endregion

        #region Constructor
        public frmRisesCommissions()
        {
            mObjMachineryServiceFactory = new MachinerySeviceFactory();

            LoadEvents();
            LoadInitialsControls();
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.lblRise = ((SAPbouiCOM.StaticText)(this.GetItem("lblRise").Specific));
            this.lblDate = ((SAPbouiCOM.StaticText)(this.GetItem("lblDate").Specific));
            this.txtFolioRise = ((SAPbouiCOM.EditText)(this.GetItem("txtRise").Specific));
            this.txtDate = ((SAPbouiCOM.EditText)(this.GetItem("txtDate").Specific));
            this.lblAccount = ((SAPbouiCOM.StaticText)(this.GetItem("lblAcct").Specific));
            this.txtAccount = ((SAPbouiCOM.EditText)(this.GetItem("txtAcct").Specific));
            this.btnSearch = ((SAPbouiCOM.Button)(this.GetItem("btnSearch").Specific));
            this.btnSearch.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btnSearch_ClickAfter);
            this.mtxCommissions = ((SAPbouiCOM.Matrix)(this.GetItem("mtxCom").Specific));
            this.mtxCommissions.ValidateBefore += new SAPbouiCOM._IMatrixEvents_ValidateBeforeEventHandler(this.mtxCommissions_ValidateBefore);
            this.lblTotal = ((SAPbouiCOM.StaticText)(this.GetItem("lblTotal").Specific));
            this.txtTotal = ((SAPbouiCOM.EditText)(this.GetItem("txtTotal").Specific));
            this.btnCreate = ((SAPbouiCOM.Button)(this.GetItem("btnCreate").Specific));
            this.btnCreate.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btnCreate_ClickAfter);
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.CloseBefore += new SAPbouiCOM.Framework.FormBase.CloseBeforeHandler(this.Form_CloseBefore);
            this.ResizeAfter += new SAPbouiCOM.Framework.FormBase.ResizeAfterHandler(this.Form_ResizeAfter);

        }

        private void OnCustomInitialize()
        {

        }
        #endregion

        #region Load & Unload Events
        private void LoadEvents()
        {
            UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }

        private void UnLoadEvents()
        {
            UIApplication.GetApplication().ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }
        #endregion

        #region EventsHandle
        /// <summary>
        /// SBO_Application_ItemEvent
        /// Metodo para controlar los eventos de la pantalla.
        /// @Author FranciscoFimbres
        /// </summary>
        /// <param name="FormUID"></param>
        /// <param name="pVal"></param>
        /// <param name="BubbleEvent"></param>
        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            //string y = pVal.CharPressed.ToString();
            try
            {
                if (pVal.FormTypeEx.Equals("UGRS.AddOn.Machinery.Forms.frmCFLFolios"))
                {
                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                if (mObjFrmFolios != null)
                                {
                                    if (string.IsNullOrEmpty(mObjFrmFolios.mStrFolio))
                                        return;

                                    // Validar si la comprobación esta autorizada
                                    // Se elimina validación a petición de UGRS
                                    // Se agrega de nuevo validación a petición de UGRS
                                    int lIntStatus = mObjMachineryServiceFactory.GetRiseService().GetVoucherStatus(mObjFrmFolios.mStrFolio);
                                    if (lIntStatus != 4) //Autorizado
                                    {
                                        UIApplication.ShowError(string.Format("El folio de comprobación de gastos de la subida {0} no está autirzado, favor de revisar", mObjFrmFolios.mStrFolio));
                                        return;
                                    }

                                    txtFolioRise.Value = mObjFrmFolios.mStrFolio;

                                    GetRiseDetails(txtFolioRise.Value.Trim());
                                }
                                break;
                        }
                    }
                }

                if (FormUID.Equals(this.UIAPIRawForm.UniqueID))
                {
                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_CLICK:
                                if (pVal.ItemUID.Equals("btnSearch"))
                                {
                                    //InitSearch();
                                }
                                break;
                            case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST:

                                break;
                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                UnLoadEvents();
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.WriteError(String.Format("[frmRisesCommissions - SBO_Application_ItemEvent] Error: {0}", ex.Message));

                if (!ex.Message.Contains("Form - Invalid Form"))
                    SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(ex.Message);
            }
        }
        #endregion

        #region Events
        private void Form_CloseBefore(SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            UnLoadEvents();
        }

        private void btnSearch_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                mObjFrmFolios = new frmCFLFolios(FoliosFormModeEnum.Commission);
                mObjFrmFolios.Show();
            }
            catch (Exception lObjException)
            {
                if (lObjException.Message.Contains("Failed to create form. Please check the form attributes"))
                {
                    if (Application.SBO_Application.MessageBox("La pantalla de folios ya se encuentra abierta, ¿desea cerrar la actual?", 1, "Aceptar", "Cancelar", "") == 1)
                    {
                        UIApplication.GetApplication().Forms.Item("frmFolios").Close();

                        mObjFrmFolios = new frmCFLFolios(FoliosFormModeEnum.Commission);
                        mObjFrmFolios.Show();
                    }
                }
                else
                {
                    UIApplication.ShowMessageBox(string.Format("Error al abrir la pantalla de folios: {0}", lObjException.Message));
                }
            }
        }

        private void btnCreate_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                if (Application.SBO_Application.MessageBox("¿Desea crear la comisión?", 1, "Aceptar", "Cancelar", "") != 1)
                {
                    return;
                }

                this.UIAPIRawForm.Freeze(true);

                CreateJournalEntry();
            }
            catch (Exception lObjException)
            {
                UIApplication.ShowMessageBox(lObjException.Message);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        private void mtxCommissions_ValidateBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                this.UIAPIRawForm.Freeze(true);

                string lStrDTColumn = string.Empty;

                if (pVal.ColUID == "ColImpFS")
                {
                    string lStrImportFS = (mtxCommissions.Columns.Item("ColImpFS").Cells.Item(pVal.Row).Specific as SAPbouiCOM.EditText).Value.Trim();
                    string lStrCommission = (mtxCommissions.Columns.Item("ColCom").Cells.Item(pVal.Row).Specific as SAPbouiCOM.EditText).Value.Trim();
                    string lStrAdeudo = (mtxCommissions.Columns.Item("ColAdeudo").Cells.Item(pVal.Row).Specific as SAPbouiCOM.EditText).Value.Trim();
                    string lStrOriginalImportFS = dtCommissions.GetValue("OrigImpFS", pVal.Row - 1).ToString();
                    string lStrOldImportFS = dtCommissions.GetValue("ImpFS", pVal.Row - 1).ToString();

                    double lDblImportFS = string.IsNullOrEmpty(lStrImportFS) ? 0 : double.Parse(lStrImportFS);
                    double lDblCommission = string.IsNullOrEmpty(lStrCommission) ? 0 : double.Parse(lStrCommission);
                    double lDblAdeudo = string.IsNullOrEmpty(lStrAdeudo) ? 0 : double.Parse(lStrAdeudo);
                    double lDblTotal = lDblCommission - lDblImportFS - lDblAdeudo;
                    double lDblPendiente = ((lDblImportFS + lDblAdeudo) - lDblCommission);
                    double lDblOriginalImportFS = string.IsNullOrEmpty(lStrOriginalImportFS) ? 0 : double.Parse(lStrOriginalImportFS);
                    double lDblOriginalTotal = lDblImportFS < 0 ? (lDblCommission - lDblImportFS) : lDblCommission;

                    bool lBolOrigImportFSIsNegative = lDblOriginalImportFS < 0;
                    bool lBolImportFSIsNegative = lDblImportFS < 0;

                    if (lBolOrigImportFSIsNegative)
                    {
                        if (lDblOriginalImportFS != lDblImportFS)
                        {
                            UIApplication.ShowError("Si el importe faltante/sobrante es negativo este no se puede modificar");
                            (mtxCommissions.Columns.Item("ColImpFS").Cells.Item(pVal.Row).Specific as SAPbouiCOM.EditText).Value = lStrOldImportFS;
                            return;
                        }
                    }
                    else
                    {

                        if (lDblImportFS > lDblOriginalImportFS && !lBolImportFSIsNegative)
                        {
                            if (lDblImportFS == 0)
                            {

                            }
                            else
                            {
                                UIApplication.ShowError(string.Format("El valor máximo para el importe faltante/sobrante es de {0}", lDblOriginalImportFS));
                                (mtxCommissions.Columns.Item("ColImpFS").Cells.Item(pVal.Row).Specific as SAPbouiCOM.EditText).Value = lStrOldImportFS;
                                return;
                            }
                        }

                        if (lBolOrigImportFSIsNegative != lBolImportFSIsNegative)
                        {
                            if (lDblImportFS == 0)
                            {

                            }
                            else
                            {
                                UIApplication.ShowError(string.Format("El valor ingresado debe ser del mismo signo que su valor original {0}", lDblOriginalImportFS));
                                (mtxCommissions.Columns.Item("ColImpFS").Cells.Item(pVal.Row).Specific as SAPbouiCOM.EditText).Value = lStrOldImportFS;
                                return;
                            }
                        }

                        if (lBolImportFSIsNegative)
                        {
                            if (lDblImportFS < lDblOriginalImportFS)
                            {
                                if (lDblImportFS == 0)
                                {

                                }
                                else
                                {
                                    UIApplication.ShowError(string.Format("El valor máximo para el importe faltante/sobrante es de {0}", lDblOriginalImportFS));
                                    (mtxCommissions.Columns.Item("ColImpFS").Cells.Item(pVal.Row).Specific as SAPbouiCOM.EditText).Value = lStrOldImportFS;
                                    return;
                                }
                            }
                        }
                    }

                    dtCommissions.SetValue("ImpFS", pVal.Row - 1, lDblImportFS);
                    dtCommissions.SetValue("Total", pVal.Row - 1, lDblTotal);
                    dtCommissions.SetValue("Pend", pVal.Row - 1, lDblPendiente > 0 ? lDblPendiente : lDblAdeudo);
                    dtCommissions.SetValue("OrigTotal", pVal.Row - 1, lDblOriginalTotal);
                }
                else if (pVal.ColUID == "ColAdeudo")
                {
                    string lStrAdeudo = (mtxCommissions.Columns.Item("ColAdeudo").Cells.Item(pVal.Row).Specific as SAPbouiCOM.EditText).Value.Trim();
                    string lStrImportFS = (mtxCommissions.Columns.Item("ColImpFS").Cells.Item(pVal.Row).Specific as SAPbouiCOM.EditText).Value.Trim();
                    string lStrCommission = (mtxCommissions.Columns.Item("ColCom").Cells.Item(pVal.Row).Specific as SAPbouiCOM.EditText).Value.Trim();
                    string lStrOriginalAdeudo = dtCommissions.GetValue("OrigAdudo", pVal.Row - 1).ToString();
                    string lStrOldAdeudo = dtCommissions.GetValue("Adeudo", pVal.Row - 1).ToString();

                    double lDblAdeudo = string.IsNullOrEmpty(lStrAdeudo) ? 0 : double.Parse(lStrAdeudo);
                    double lDblImportFS = string.IsNullOrEmpty(lStrImportFS) ? 0 : double.Parse(lStrImportFS);
                    double lDblCommission = string.IsNullOrEmpty(lStrCommission) ? 0 : double.Parse(lStrCommission);
                    double lDblTotal = lDblCommission - lDblImportFS - lDblAdeudo;
                    double lDblPendiente = ((lDblImportFS + lDblAdeudo) - lDblCommission);
                    double lDblOriginalAdeudo = string.IsNullOrEmpty(lStrOriginalAdeudo) ? 0 : double.Parse(lStrOriginalAdeudo);
                    double lDblOriginalTotal = lDblImportFS < 0 ? (lDblCommission - lDblImportFS) : lDblCommission;

                    if (lDblAdeudo > lDblOriginalAdeudo)
                    {
                        UIApplication.ShowError(string.Format("El valor máximo para el adeudo es de {0}", lDblOriginalAdeudo));
                        (mtxCommissions.Columns.Item("ColAdeudo").Cells.Item(pVal.Row).Specific as SAPbouiCOM.EditText).Value = lStrOldAdeudo;
                        return;
                    }

                    if (lDblAdeudo < 0)
                    {
                        UIApplication.ShowError("No se permiten valores negativos para el campo adeudo");
                        (mtxCommissions.Columns.Item("ColAdeudo").Cells.Item(pVal.Row).Specific as SAPbouiCOM.EditText).Value = lStrOldAdeudo;
                        return;
                    }

                    dtCommissions.SetValue("Adeudo", pVal.Row - 1, lDblAdeudo);
                    dtCommissions.SetValue("Total", pVal.Row - 1, lDblTotal);
                    dtCommissions.SetValue("Pend", pVal.Row - 1, lDblPendiente > 0 ? lDblPendiente : lDblAdeudo);
                    dtCommissions.SetValue("OrigTotal", pVal.Row - 1, lDblOriginalTotal);
                }

                mtxCommissions.LoadFromDataSource();
                mtxCommissions.AutoResizeColumns();

                CalculateTotal();
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                UIApplication.ShowError(ex.Message);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        private void Form_ResizeAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            mtxCommissions.AutoResizeColumns();
        }
        #endregion

        #region Functions
        private void LoadInitialsControls()
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);

                CreateCommissionsDatatable();

                txtDate.Value = DateTime.Now.ToString("dd/MM/yyy");

                txtAccount.Value = mObjMachineryServiceFactory.GetConfigurationsService().GetConfigurationByName(ConfigurationsEnum.CashRegisterAccount).Value; //CommissionAccount
                string lStrSupTarifa = mObjMachineryServiceFactory.GetConfigurationsService().GetConfigurationByName(ConfigurationsEnum.SupervisorCommisionRate).Value;
                string lStrOprTarifa = mObjMachineryServiceFactory.GetConfigurationsService().GetConfigurationByName(ConfigurationsEnum.OperatorCommisionRate).Value;
            }
            catch (Exception lObjException)
            {
                UIApplication.GetApplication().SetStatusBarMessage(lObjException.Message, SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        private void CreateJournalEntry()
        {
            try
            {
                string lStrCommissionAccount = mObjMachineryServiceFactory.GetConfigurationsService().GetAccountCode(ConfigurationsEnum.CommissionAccount);
                string lStrFuncEmpAcct = mObjMachineryServiceFactory.GetConfigurationsService().GetAccountCode(ConfigurationsEnum.ViaticExpenses);
                string lStrCashRegisterAccount = mObjMachineryServiceFactory.GetConfigurationsService().GetAccountCode(ConfigurationsEnum.CashRegisterAccount);
                string lStrUserId = mObjMachineryServiceFactory.GetUsersService().GetUserId(Application.SBO_Application.Company.UserName).ToString();
                double lDblTotalTot = double.Parse(txtTotal.Value);
                //double lDblTotalCommissions = CalculateCommissionTotal();
                //double lDblDifference = CalculateDifference(lDblTotalCommissions, lDblTotalTot);
                string lStrTravelExpFolio = mObjMachineryServiceFactory.GetTravelExpensesService().GetCurrentFolio(txtFolioRise.Value);
                string lStrSupId = string.Empty;

                if (string.IsNullOrEmpty(lStrFuncEmpAcct))
                {
                    UIApplication.ShowMessageBox("La cuenta de funcionarios y empleados no está asignada en la tabla de parametrizaciones");
                    return;
                }

                if (string.IsNullOrEmpty(lStrCommissionAccount))
                {
                    UIApplication.ShowMessageBox("La cuenta de comisiones no está asignada en la tabla de parametrizaciones");
                    return;
                }

                if (string.IsNullOrEmpty(lStrCashRegisterAccount))
                {
                    UIApplication.ShowMessageBox("La cuenta de caja no está asignada en la tabla de parametrizaciones");
                    return;
                }

                if (dtCommissions.Rows.Count == 0)
                {
                    UIApplication.ShowMessageBox("No puede crear la comisión sin líneas");
                    return;
                }

                SAPbobsCOM.JournalEntries lObjJournalEntry = (SAPbobsCOM.JournalEntries)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);
                lObjJournalEntry.TransactionCode = "MQCM";
                lObjJournalEntry.Reference = txtFolioRise.Value;
                lObjJournalEntry.ReferenceDate = DateTime.Now;
                lObjJournalEntry.TaxDate = DateTime.Now;
                lObjJournalEntry.DueDate = DateTime.Now;

                for (int i = 0; i < dtCommissions.Rows.Count; i++)
                {
                    double lDblImportFSU = double.Parse(dtCommissions.GetValue("ImpFS", i).ToString());
                    double lDblDeudaU = double.Parse(dtCommissions.GetValue("Adeudo", i).ToString());
                    double lDblTotalCommission = double.Parse(dtCommissions.GetValue("Cmson", i).ToString());
                    double lDblTotalLine = double.Parse(dtCommissions.GetValue("Total", i).ToString());
                    string lStrEmpId = dtCommissions.GetValue("EmpId", i).ToString();
                    string lStrIsSup = dtCommissions.GetValue("IsSup", i).ToString();
                    string lStrPosition = dtCommissions.GetValue("PstoId", i).ToString();
                    string lStrHours = dtCommissions.GetValue("Hrs", i).ToString();
                    double lDblDeudaR = double.Parse(dtCommissions.GetValue("OrigAdudo", i).ToString());
                    double lDblImportFSR = double.Parse(dtCommissions.GetValue("OrigImpFS", i).ToString());
                    string lStrLastCodeMov = dtCommissions.GetValue("CodeMov", i).ToString();
                    //"OrigImpFS", dtCommissions.Rows.Count - 1, pObjCommissionDetail.ImportFS);

                    lObjJournalEntry.Lines.SetCurrentLine(lObjJournalEntry.Lines.Count - 1);
                    lObjJournalEntry.Lines.TaxDate = DateTime.Now;
                    lObjJournalEntry.Lines.AccountCode = lStrCommissionAccount;
                    lObjJournalEntry.Lines.Debit = lDblTotalCommission;
                    lObjJournalEntry.Lines.Credit = 0;
                    lObjJournalEntry.Lines.CostingCode = "MQ_MAQUI";
                    lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_TypeAux").Value = "2";
                    lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliar").Value = lStrEmpId; //code empleado
                    lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = txtFolioRise.Value;
                    if (lStrPosition == "S")
                    {
                        lObjJournalEntry.Lines.Reference2 = lStrHours;
                    }
                    else
                    {
                        lObjJournalEntry.Lines.AdditionalReference = lStrHours;
                    }
                    lObjJournalEntry.Lines.Add();

                    if (lStrIsSup == "Y")
                    {
                        if (lDblTotalLine > 0)
                        {
                            double lDblDeudaF = lDblDeudaR - lDblDeudaU;
                            double lDblImpFSF = lDblImportFSR - lDblImportFSU;

                            if (lDblDeudaR > 0)
                            {
                                lObjJournalEntry.Lines.SetCurrentLine(lObjJournalEntry.Lines.Count - 1);
                                lObjJournalEntry.Lines.TaxDate = DateTime.Now;
                                lObjJournalEntry.Lines.AccountCode = lStrFuncEmpAcct;
                                lObjJournalEntry.Lines.Debit = 0;
                                lObjJournalEntry.Lines.Credit = lDblDeudaR;
                                lObjJournalEntry.Lines.CostingCode = "MQ_MAQUI";
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_TypeAux").Value = "2";
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliar").Value = lStrEmpId; //code empleado
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = lStrLastCodeMov;
                                lObjJournalEntry.Lines.Add();
                            }

                            if (lDblDeudaF > 0)
                            {
                                lObjJournalEntry.Lines.SetCurrentLine(lObjJournalEntry.Lines.Count - 1);
                                lObjJournalEntry.Lines.TaxDate = DateTime.Now;
                                lObjJournalEntry.Lines.AccountCode = lStrFuncEmpAcct;
                                lObjJournalEntry.Lines.Debit = lDblDeudaF;
                                lObjJournalEntry.Lines.Credit = 0;
                                lObjJournalEntry.Lines.CostingCode = "MQ_MAQUI";
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_TypeAux").Value = "2";
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliar").Value = lStrEmpId; //code empleado
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = lStrTravelExpFolio;
                                lObjJournalEntry.Lines.Add();
                            }

                            if (lDblImportFSR > 0 && lDblImportFSU > 0)
                            {
                                lObjJournalEntry.Lines.SetCurrentLine(lObjJournalEntry.Lines.Count - 1);
                                lObjJournalEntry.Lines.TaxDate = DateTime.Now;
                                lObjJournalEntry.Lines.AccountCode = lStrFuncEmpAcct;
                                lObjJournalEntry.Lines.Debit = 0;
                                lObjJournalEntry.Lines.Credit = lDblImportFSU;
                                lObjJournalEntry.Lines.CostingCode = "MQ_MAQUI";
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_TypeAux").Value = "2";
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliar").Value = lStrEmpId; //code empleado
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = lStrTravelExpFolio;
                                lObjJournalEntry.Lines.Add();
                            }
                            else if (lDblImportFSR < 0)
                            {
                                lObjJournalEntry.Lines.SetCurrentLine(lObjJournalEntry.Lines.Count - 1);
                                lObjJournalEntry.Lines.TaxDate = DateTime.Now;
                                lObjJournalEntry.Lines.AccountCode = lStrFuncEmpAcct;
                                lObjJournalEntry.Lines.Debit = lDblImportFSU * -1;
                                lObjJournalEntry.Lines.Credit = 0;
                                lObjJournalEntry.Lines.CostingCode = "MQ_MAQUI";
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_TypeAux").Value = "2";
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliar").Value = lStrEmpId; //code empleado
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = lStrTravelExpFolio;
                                lObjJournalEntry.Lines.Add();
                            }
                        }
                        else
                        {
                            double lDblSaldo = lDblDeudaR - lDblTotalCommission;

                            if (lDblDeudaR > 0)
                            {
                                lObjJournalEntry.Lines.SetCurrentLine(lObjJournalEntry.Lines.Count - 1);
                                lObjJournalEntry.Lines.TaxDate = DateTime.Now;
                                lObjJournalEntry.Lines.AccountCode = lStrFuncEmpAcct;
                                lObjJournalEntry.Lines.Debit = 0;
                                lObjJournalEntry.Lines.Credit = lDblDeudaR;
                                lObjJournalEntry.Lines.CostingCode = "MQ_MAQUI";
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_TypeAux").Value = "2";
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliar").Value = lStrEmpId; //code empleado
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = lStrLastCodeMov;
                                lObjJournalEntry.Lines.Add();
                            }

                            if (lDblDeudaR > 0 && lDblSaldo > 0)
                            {
                                lObjJournalEntry.Lines.SetCurrentLine(lObjJournalEntry.Lines.Count - 1);
                                lObjJournalEntry.Lines.TaxDate = DateTime.Now;
                                lObjJournalEntry.Lines.AccountCode = lStrFuncEmpAcct;
                                lObjJournalEntry.Lines.Debit = lDblSaldo;
                                lObjJournalEntry.Lines.Credit = 0;
                                lObjJournalEntry.Lines.CostingCode = "MQ_MAQUI";
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_TypeAux").Value = "2";
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliar").Value = lStrEmpId; //code empleado
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = lStrTravelExpFolio;
                                lObjJournalEntry.Lines.Add();
                            }

                            if (lDblImportFSR > 0 && lDblSaldo < 0)
                            {
                                lObjJournalEntry.Lines.SetCurrentLine(lObjJournalEntry.Lines.Count - 1);
                                lObjJournalEntry.Lines.TaxDate = DateTime.Now;
                                lObjJournalEntry.Lines.AccountCode = lStrFuncEmpAcct;
                                lObjJournalEntry.Lines.Debit = 0;
                                lObjJournalEntry.Lines.Credit = lDblSaldo * -1;
                                lObjJournalEntry.Lines.CostingCode = "MQ_MAQUI";
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_TypeAux").Value = "2";
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliar").Value = lStrEmpId; //code empleado
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = lStrTravelExpFolio;
                                lObjJournalEntry.Lines.Add();
                            }

                            if (lDblImportFSR < 0)
                            {
                                lObjJournalEntry.Lines.SetCurrentLine(lObjJournalEntry.Lines.Count - 1);
                                lObjJournalEntry.Lines.TaxDate = DateTime.Now;
                                lObjJournalEntry.Lines.AccountCode = lStrFuncEmpAcct;
                                lObjJournalEntry.Lines.Debit = 0;
                                lObjJournalEntry.Lines.Credit = lDblImportFSR * -1;
                                lObjJournalEntry.Lines.CostingCode = "MQ_MAQUI";
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_TypeAux").Value = "2";
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliar").Value = lStrEmpId; //code empleado
                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = lStrTravelExpFolio;
                                lObjJournalEntry.Lines.Add();
                            }
                        }
                    }
                }

                //Total de comisiones
                if (lDblTotalTot > 0)
                {
                    lObjJournalEntry.Lines.SetCurrentLine(lObjJournalEntry.Lines.Count - 1);
                    lObjJournalEntry.Lines.TaxDate = DateTime.Now;
                    lObjJournalEntry.Lines.AccountCode = lStrCashRegisterAccount; //cuenta maquinaria m.c.
                    lObjJournalEntry.Lines.Debit = 0;
                    lObjJournalEntry.Lines.Credit = lDblTotalTot;
                    lObjJournalEntry.Lines.CostingCode = "MQ_MAQUI";
                    lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_TypeAux").Value = "2";
                    lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliar").Value = lStrUserId; //code empleado
                    lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = txtFolioRise.Value; //folio de la solicitud de viáticos de la subida
                    lObjJournalEntry.Lines.Add();
                }

                if (lObjJournalEntry.Add() != 0)
                {
                    string lStrLastError = DIApplication.Company.GetLastErrorDescription();
                    UIApplication.ShowMessageBox(string.Format("Error al generar el asiento de la comisión: {0}", DIApplication.Company.GetLastErrorDescription()));
                }
                else
                {
                    int lIntDocEntry = int.Parse(DIApplication.Company.GetNewObjectKey());

                    LogUtility.WriteSuccess(String.Format("[frmRisesCommissions - CreateJournalEntry] Comisión creada correctamente con el DocEntry {0} para la Subida: {1}", lIntDocEntry, txtFolioRise.Value));

                    SaveCommissionsRecords(lIntDocEntry);

                    //this.UIAPIRawForm.Close();
                    mObjMachineryServiceFactory.GetRiseService().MarkRiseAsCommissioned(int.Parse(txtFolioRise.Value));
                    ClearControls();

                    UIApplication.ShowSuccess(string.Format("Comisión creada correctamente con el número de documento: {0}", lIntDocEntry));
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.WriteError(String.Format("[frmRisesCommissions - CreateJournalEntry] Error al crear el asiento contable: {0}", lObjException.Message));
                throw new Exception(string.Format("Error al crear el asiento contable: {0}", lObjException.Message));
            }
            finally
            {

            }
        }

        private double CalculateDifference(double lDblTotalCommissions, double lDblTotalTot)
        {
            return Math.Abs(lDblTotalCommissions - lDblTotalTot);
        }

        private void SaveCommissionsRecords(int pIntJournalEntry)
        {
            try
            {
                List<Commissions> lLstCommissions = mObjMachineryServiceFactory.GetCommissionsService()
                                                        .ConvetToEntity(mObjMachineryServiceFactory
                                                        .GetCommissionsService().DataTableToDTO(dtCommissions))
                                                        .Select(c => { c.RiseId = int.Parse(txtFolioRise.Value); c.JournalEntryId = pIntJournalEntry; return c; }).ToList();

                for (int i = 0; i < lLstCommissions.Count; i++)
                {
                    string lStrCode = dtCommissions.GetValue("Code", i).ToString();

                    if (string.IsNullOrEmpty(lStrCode))
                    {
                        mObjMachineryServiceFactory.GetCommissionsService().Add(lLstCommissions[i]);
                        dtCommissions.SetValue("Code", i, mObjMachineryServiceFactory.GetCommissionsService().GetLastCode());
                    }
                    else
                    {
                        mObjMachineryServiceFactory.GetCommissionsService().Update(lLstCommissions[i]);
                    }
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.WriteError(String.Format("[frmRisesCommissions - CreateJournalEntry] Error al guardar los registros de comisiones: {0}", lObjException.Message));
                throw new Exception(string.Format("Error al guardar los registros de comisiones: {0}", lObjException.Message));
            }
        }

        #region OldVersion
        //private void CreateJournalEntry()
        //{
        //    try
        //    {
        //        string lStrCommissionAccount = mObjMachineryServiceFactory.GetConfigurationsService().GetAccountCode(ConfigurationsEnum.CommissionAccount);
        //        string lStrFuncEmpAcct = mObjMachineryServiceFactory.GetConfigurationsService().GetAccountCode(ConfigurationsEnum.ViaticExpenses);
        //        string lStrCashRegisterAccount = mObjMachineryServiceFactory.GetConfigurationsService().GetAccountCode(ConfigurationsEnum.CashRegisterAccount);
        //        string lStrUserId = mObjMachineryServiceFactory.GetUsersService().GetUserId(Application.SBO_Application.Company.UserName).ToString();
        //        double lDblTotalTot = double.Parse(txtTotal.Value);
        //        double lDblTotalCommissions = CalculateCommissionTotal();
        //        string lStrTravelExpFolio = mObjMachineryServiceFactory.GetTravelExpensesService().GetCurrentFolio(txtFolioRise.Value);

        //        if (string.IsNullOrEmpty(lStrFuncEmpAcct))
        //        {
        //            UIApplication.ShowMessageBox("La cuenta de funcionarios y empleados no está asignada en la tabla de parametrizaciones");
        //            return;
        //        }

        //        if (string.IsNullOrEmpty(lStrCommissionAccount))
        //        {
        //            UIApplication.ShowMessageBox("La cuenta de comisiones no está asignada en la tabla de parametrizaciones");
        //            return;
        //        }

        //        if (string.IsNullOrEmpty(lStrCashRegisterAccount))
        //        {
        //            UIApplication.ShowMessageBox("La cuenta de caja no está asignada en la tabla de parametrizaciones");
        //            return;
        //        }

        //        SAPbobsCOM.JournalEntries lObjJournalEntry = (SAPbobsCOM.JournalEntries)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);
        //        lObjJournalEntry.TransactionCode = "MQCM";
        //        lObjJournalEntry.Reference = txtFolioRise.Value;
        //        lObjJournalEntry.ReferenceDate = DateTime.Now;
        //        lObjJournalEntry.TaxDate = DateTime.Now;
        //        lObjJournalEntry.DueDate = DateTime.Now;

        //        for (int i = 0; i < dtCommissions.Rows.Count; i++)
        //        {
        //            double lDblImportFS = double.Parse(dtCommissions.GetValue("ImpFS", i).ToString());
        //            double lDblAdeudo = double.Parse(dtCommissions.GetValue("Adeudo", i).ToString());
        //            double lDblTotalCommission = double.Parse(dtCommissions.GetValue("Cmson", i).ToString());
        //            double lDblTotalLine = double.Parse(dtCommissions.GetValue("Total", i).ToString());
        //            string lStrEmpId = dtCommissions.GetValue("EmpId", i).ToString();
        //            string lStrIsSup = dtCommissions.GetValue("IsSup", i).ToString();
        //            string lStrPosition = dtCommissions.GetValue("PstoId", i).ToString();
        //            string lStrHours = dtCommissions.GetValue("Hrs", i).ToString();

        //            if (lDblTotalLine < 0)
        //            {
        //                UIApplication.ShowError("No se permiten valores negativos para la columna total");
        //                return;
        //            }

        //            lObjJournalEntry.Lines.SetCurrentLine(lObjJournalEntry.Lines.Count - 1);
        //            lObjJournalEntry.Lines.TaxDate = DateTime.Now;
        //            lObjJournalEntry.Lines.AccountCode = lStrCommissionAccount;
        //            lObjJournalEntry.Lines.Debit = lDblTotalCommission;
        //            lObjJournalEntry.Lines.Credit = 0;
        //            lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_TypeAux").Value = "2";
        //            lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliar").Value = lStrEmpId; //code empleado
        //            lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = txtFolioRise.Value;
        //            if (lStrPosition == "S")
        //            {
        //                lObjJournalEntry.Lines.Reference2 = lStrHours;
        //            }
        //            else
        //            {
        //                lObjJournalEntry.Lines.AdditionalReference = lStrHours;
        //            }
        //            lObjJournalEntry.Lines.Add();

        //            if (lStrIsSup == "Y")
        //            {

        //                if (lDblImportFS != 0)
        //                {
        //                    lObjJournalEntry.Lines.SetCurrentLine(lObjJournalEntry.Lines.Count - 1);
        //                    lObjJournalEntry.Lines.TaxDate = DateTime.Now;
        //                    lObjJournalEntry.Lines.AccountCode = lStrFuncEmpAcct;
        //                    lObjJournalEntry.Lines.Debit = 0;
        //                    lObjJournalEntry.Lines.Credit = lDblImportFS;
        //                    lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_TypeAux").Value = "2";
        //                    lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliar").Value = lStrEmpId; //code empleado
        //                    lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = lStrTravelExpFolio; //folio de la solicitud de viáticos de la subida
        //                    lObjJournalEntry.Lines.Add();
        //                }

        //                if (lDblAdeudo != 0)
        //                {
        //                    List<SupervisorDebitDTO> lLstSupervisorDebit = mObjMachineryServiceFactory.GetCommissionsService()
        //                                                                   .GetSupervisorDebit(txtFolioRise.Value, lStrFuncEmpAcct, dtCommissions.GetValue("EmpId", i).ToString());

        //                    foreach (var lObjSupDebit in lLstSupervisorDebit)
        //                    {
        //                        if (lDblAdeudo != 0)
        //                        {
        //                            lObjJournalEntry.Lines.SetCurrentLine(lObjJournalEntry.Lines.Count - 1);
        //                            lObjJournalEntry.Lines.TaxDate = DateTime.Now;
        //                            lObjJournalEntry.Lines.AccountCode = lStrFuncEmpAcct;
        //                            lObjJournalEntry.Lines.Debit = 0;
        //                            lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_TypeAux").Value = "2";
        //                            lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliar").Value = lStrEmpId; //code empleado

        //                            if (lDblAdeudo > lObjSupDebit.Debit)
        //                            {
        //                                lObjJournalEntry.Lines.Credit = lObjSupDebit.Debit;
        //                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = lObjSupDebit.MovementCode;
        //                                lObjJournalEntry.Lines.Add();

        //                                lObjSupDebit.Debit -= lObjSupDebit.Debit;
        //                                lDblAdeudo -= lObjSupDebit.Debit;
        //                            }
        //                            else
        //                            {
        //                                lObjJournalEntry.Lines.Credit = lDblAdeudo;
        //                                lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = lObjSupDebit.MovementCode;
        //                                lObjJournalEntry.Lines.Add();

        //                                lObjSupDebit.Debit -= lDblAdeudo;
        //                                lDblAdeudo -= lDblAdeudo;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        //Diferencia total de comisiones y columna total
        //        lObjJournalEntry.Lines.SetCurrentLine(lObjJournalEntry.Lines.Count - 1);
        //        lObjJournalEntry.Lines.TaxDate = DateTime.Now;
        //        lObjJournalEntry.Lines.AccountCode = lStrCashRegisterAccount; //cuenta maquinaria m.c.
        //        lObjJournalEntry.Lines.Debit = lDblTotalCommissions - lDblTotalTot;
        //        lObjJournalEntry.Lines.Credit = 0;
        //        lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_TypeAux").Value = "2";
        //        lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliar").Value = lStrUserId; //code empleado
        //        lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = txtFolioRise.Value; //folio de la solicitud de viáticos de la subida
        //        lObjJournalEntry.Lines.Add();

        //        //Total de comisiones
        //        lObjJournalEntry.Lines.SetCurrentLine(lObjJournalEntry.Lines.Count - 1);
        //        lObjJournalEntry.Lines.TaxDate = DateTime.Now;
        //        lObjJournalEntry.Lines.AccountCode = lStrCashRegisterAccount; //cuenta maquinaria m.c.
        //        lObjJournalEntry.Lines.Debit = 0;
        //        lObjJournalEntry.Lines.Credit = lDblTotalCommissions;
        //        lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_TypeAux").Value = "2";
        //        lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliar").Value = lStrUserId; //code empleado
        //        lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = txtFolioRise.Value; //folio de la solicitud de viáticos de la subida
        //        lObjJournalEntry.Lines.Add();

        //        if (lObjJournalEntry.Add() != 0)
        //        {
        //            string lStrLastError = DIApplication.Company.GetLastErrorDescription();
        //            UIApplication.ShowMessageBox(string.Format("Error al generar el asiento de la comisión: {0}", DIApplication.Company.GetLastErrorDescription()));
        //        }
        //        else
        //        {
        //            int lIntDocEntry = int.Parse(DIApplication.Company.GetNewObjectKey());

        //            //this.UIAPIRawForm.Close();
        //            mObjMachineryServiceFactory.GetRiseService().MarkRiseAsCommissioned(int.Parse(txtFolioRise.Value));
        //            ClearControls();

        //            UIApplication.ShowSuccess("Comisión creada correctamente");
        //        }
        //    }
        //    catch (Exception lObjException)
        //    {
        //        throw new Exception(string.Format("Error al crear el asiento contable: {0}", lObjException.Message));
        //    }
        //    finally
        //    {

        //    }
        //}
        #endregion

        private void GetRiseDetails(string pStrRiseId)
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);

                string lStrFuncEmpAcct = mObjMachineryServiceFactory.GetConfigurationsService().GetAccountCode(ConfigurationsEnum.ViaticExpenses);
                mStrSupervisorId = mObjMachineryServiceFactory.GetRiseService().GetByRiseId(int.Parse(pStrRiseId)).SupervisorId.ToString();

                if (string.IsNullOrEmpty(lStrFuncEmpAcct))
                {
                    UIApplication.ShowMessageBox("La cuenta de funcionarios y empleados no está asignada en la tabla de parametrizaciones");
                    return;
                }

                ClearMatrix(dtCommissions.UniqueID, mtxCommissions);

                List<CommissionDetailsDTO> lLstCommissionsDetails = mObjMachineryServiceFactory.GetCommissionsService().GetRisesDetailsForCommissions(pStrRiseId, lStrFuncEmpAcct);

                foreach (var lObjCommissionDetail in lLstCommissionsDetails)
                {
                    AddRiseCommissionDetail(lObjCommissionDetail, lLstCommissionsDetails);
                }

                CalculateTotal();
            }
            catch (Exception lObjException)
            {
                UIApplication.ShowMessageBox(lObjException.Message);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        private void AddRiseCommissionDetail(CommissionDetailsDTO pObjCommissionDetail, List<CommissionDetailsDTO> pLstCommissionsDetails)
        {
            try
            {
                if (pObjCommissionDetail == null)
                    return;

                this.UIAPIRawForm.Freeze(true);

                string lStrIsSupervisor = pLstCommissionsDetails.Where(x => x.IsSupervisor == "S").Count() == 1 && pObjCommissionDetail.IsSupervisor == "S" ? "Y" :
                                          (pObjCommissionDetail.Position == "S" && mStrSupervisorId == pObjCommissionDetail.EmployeeId.ToString()) ? "Y" : "N";
                double lDblImportFS = pLstCommissionsDetails.Where(x => x.IsSupervisor == "S").Count() == 1 && lStrIsSupervisor == "Y" ? pObjCommissionDetail.ImportFS :
                                          (pObjCommissionDetail.Position == "S") ? pObjCommissionDetail.ImportFS : 0;
                double lDblAdeudo = pLstCommissionsDetails.Where(x => x.IsSupervisor == "S").Count() == 1 && lStrIsSupervisor == "Y" ? pObjCommissionDetail.Adeudo :
                                          (pObjCommissionDetail.Position == "S") ? pObjCommissionDetail.Adeudo : 0;

                double lDblPendiente = ((lDblImportFS + lDblAdeudo) - pObjCommissionDetail.Commission);
                double lDblTotal = pObjCommissionDetail.Commission - lDblImportFS - lDblAdeudo;
                double lDblOriginalTotal = lDblImportFS < 0 ? (pObjCommissionDetail.Commission - lDblImportFS /*pObjCommissionDetail.ImportFS*/) : pObjCommissionDetail.Commission;

                dtCommissions.Rows.Add();
                dtCommissions.SetValue("#", dtCommissions.Rows.Count - 1, dtCommissions.Rows.Count);
                dtCommissions.SetValue("PstoId", dtCommissions.Rows.Count - 1, pObjCommissionDetail.Position);
                dtCommissions.SetValue("PstoNm", dtCommissions.Rows.Count - 1, pObjCommissionDetail.PositionDesc);
                dtCommissions.SetValue("EmpId", dtCommissions.Rows.Count - 1, pObjCommissionDetail.EmployeeId);
                dtCommissions.SetValue("EmpNm", dtCommissions.Rows.Count - 1, pObjCommissionDetail.Employee);
                dtCommissions.SetValue("Hrs", dtCommissions.Rows.Count - 1, pObjCommissionDetail.Hours);
                dtCommissions.SetValue("Tarifa", dtCommissions.Rows.Count - 1, pObjCommissionDetail.Rate);
                dtCommissions.SetValue("Cmson", dtCommissions.Rows.Count - 1, pObjCommissionDetail.Commission);
                dtCommissions.SetValue("ImpFS", dtCommissions.Rows.Count - 1, lDblImportFS /*pObjCommissionDetail.ImportFS*/);
                dtCommissions.SetValue("Adeudo", dtCommissions.Rows.Count - 1, lDblAdeudo /*pObjCommissionDetail.Adeudo*/);
                dtCommissions.SetValue("Total", dtCommissions.Rows.Count - 1, lDblTotal);
                dtCommissions.SetValue("Pend", dtCommissions.Rows.Count - 1, lDblPendiente > 0 ? lDblPendiente : lDblAdeudo /*pObjCommissionDetail.Adeudo*/);
                dtCommissions.SetValue("IsSup", dtCommissions.Rows.Count - 1, lStrIsSupervisor);
                dtCommissions.SetValue("OrigTotal", dtCommissions.Rows.Count - 1, lDblOriginalTotal);
                dtCommissions.SetValue("OrigImpFS", dtCommissions.Rows.Count - 1, lDblImportFS /*pObjCommissionDetail.ImportFS*/);
                dtCommissions.SetValue("OrigAdudo", dtCommissions.Rows.Count - 1, lDblAdeudo /*pObjCommissionDetail.Adeudo*/);
                dtCommissions.SetValue("CodeMov", dtCommissions.Rows.Count - 1, pObjCommissionDetail.CodeMov);

                mtxCommissions.LoadFromDataSource();
                mtxCommissions.AutoResizeColumns();

                SAPbouiCOM.CommonSetting lObjCmmnSetting = mtxCommissions.CommonSetting;
                if (lStrIsSupervisor == "N") //Operador
                {
                    /*if (pLstCommissionsDetails.Where(x => x.IsSupervisor == "S").Count() == 1 && pObjCommissionDetail.IsSupervisor == "S")
                    {
                        lObjCmmnSetting.SetCellEditable(dtCommissions.Rows.Count, 6, true);
                        lObjCmmnSetting.SetCellEditable(dtCommissions.Rows.Count, 7, true);
                    }
                    else
                    {*/
                    lObjCmmnSetting.SetCellEditable(dtCommissions.Rows.Count, 6, false);
                    lObjCmmnSetting.SetCellEditable(dtCommissions.Rows.Count, 7, false);
                    //}
                }
                else
                {
                    lObjCmmnSetting.SetCellEditable(dtCommissions.Rows.Count, 6, true);
                    lObjCmmnSetting.SetCellEditable(dtCommissions.Rows.Count, 7, true);
                }
            }
            catch (Exception lObjException)
            {
                throw new Exception(string.Format("Error al agregar el detalle de la comisión: {0}", lObjException.Message));
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        private void CreateCommissionsDatatable()
        {
            this.UIAPIRawForm.DataSources.DataTables.Add("DTCom");
            dtCommissions = this.UIAPIRawForm.DataSources.DataTables.Item("DTCom");
            dtCommissions.Columns.Add("#", SAPbouiCOM.BoFieldsType.ft_Integer);
            dtCommissions.Columns.Add("PstoId", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            dtCommissions.Columns.Add("PstoNm", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            dtCommissions.Columns.Add("EmpId", SAPbouiCOM.BoFieldsType.ft_Integer);
            dtCommissions.Columns.Add("EmpNm", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            dtCommissions.Columns.Add("Hrs", SAPbouiCOM.BoFieldsType.ft_Quantity);
            dtCommissions.Columns.Add("Tarifa", SAPbouiCOM.BoFieldsType.ft_Price);
            dtCommissions.Columns.Add("Cmson", SAPbouiCOM.BoFieldsType.ft_Price);
            dtCommissions.Columns.Add("ImpFS", SAPbouiCOM.BoFieldsType.ft_Price);
            dtCommissions.Columns.Add("Adeudo", SAPbouiCOM.BoFieldsType.ft_Price);
            dtCommissions.Columns.Add("Total", SAPbouiCOM.BoFieldsType.ft_Price);
            dtCommissions.Columns.Add("Pend", SAPbouiCOM.BoFieldsType.ft_Price);
            dtCommissions.Columns.Add("IsSup", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            dtCommissions.Columns.Add("OrigTotal", SAPbouiCOM.BoFieldsType.ft_Price);
            dtCommissions.Columns.Add("OrigImpFS", SAPbouiCOM.BoFieldsType.ft_Price);
            dtCommissions.Columns.Add("OrigAdudo", SAPbouiCOM.BoFieldsType.ft_Price);
            dtCommissions.Columns.Add("CodeMov", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            dtCommissions.Columns.Add("Code", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);

            FillContractsMatrix();
        }

        private void CalculateTotal()
        {
            try
            {
                double lDblTotal = 0;
                for (int i = 0; i < mtxCommissions.RowCount; i++)
                {
                    double lDblImport = double.Parse(dtCommissions.GetValue("Total", i).ToString());

                    if (lDblImport > 0)
                    {
                        lDblTotal += lDblImport;
                    }
                }

                txtTotal.Value = (lDblTotal).ToString();
            }
            catch (Exception lObjException)
            {
                throw new Exception(string.Format("Error al calcular el total: {0}", lObjException.Message));
            }
        }

        private double CalculateCommissionTotal()
        {
            try
            {
                double lDblTotal = 0;
                for (int i = 0; i < mtxCommissions.RowCount; i++)
                {
                    double lDblImport = double.Parse(dtCommissions.GetValue("Cmson", i).ToString()); //Cmson
                    string lStrIsSup = dtCommissions.GetValue("IsSup", i).ToString();

                    //if (lStrIsSup == "Y" && lDblImport < 0)
                    //{
                    //    continue;
                    //}

                    lDblTotal += lDblImport;
                }

                return lDblTotal;
            }
            catch (Exception lObjException)
            {
                throw new Exception(string.Format("Error al calcular el total de comisiones: {0}", lObjException.Message));
            }
        }

        private void ClearControls()
        {
            ClearMatrix(dtCommissions.UniqueID, mtxCommissions);
            mStrSupervisorId = string.Empty;
            txtFolioRise.Value = string.Empty;
            txtAccount.Value = mObjMachineryServiceFactory.GetConfigurationsService().GetConfigurationByName(ConfigurationsEnum.CommissionAccount).Value;
            txtDate.Value = DateTime.Now.ToString("dd/MM/yyy");
        }

        private void FillContractsMatrix()
        {
            mtxCommissions.Columns.Item("#").DataBind.Bind("DTCom", "#");
            mtxCommissions.Columns.Item("ColPuesto").DataBind.Bind("DTCom", "PstoNm");
            mtxCommissions.Columns.Item("ColEmp").DataBind.Bind("DTCom", "EmpNm");
            mtxCommissions.Columns.Item("ColHrs").DataBind.Bind("DTCom", "Hrs");
            mtxCommissions.Columns.Item("ColTarifa").DataBind.Bind("DTCom", "Tarifa");
            mtxCommissions.Columns.Item("ColCom").DataBind.Bind("DTCom", "Cmson");
            mtxCommissions.Columns.Item("ColImpFS").DataBind.Bind("DTCom", "ImpFS");
            mtxCommissions.Columns.Item("ColAdeudo").DataBind.Bind("DTCom", "Adeudo");
            mtxCommissions.Columns.Item("ColTotal").DataBind.Bind("DTCom", "Total");
            mtxCommissions.Columns.Item("ColPend").DataBind.Bind("DTCom", "Pend");

            mtxCommissions.AutoResizeColumns();
        }

        private void ClearMatrix(string pStrDTName, SAPbouiCOM.Matrix pObjMatrix)
        {
            if (!this.UIAPIRawForm.DataSources.DataTables.Item(pStrDTName).IsEmpty)
            {
                this.UIAPIRawForm.DataSources.DataTables.Item(pStrDTName).Rows.Clear();
                pObjMatrix.Clear();
            }
        }
        #endregion

        #region Controls
        private SAPbouiCOM.StaticText lblRise;
        private SAPbouiCOM.StaticText lblDate;
        private SAPbouiCOM.EditText txtFolioRise;
        private SAPbouiCOM.EditText txtDate;
        private SAPbouiCOM.StaticText lblAccount;
        private SAPbouiCOM.EditText txtAccount;
        private SAPbouiCOM.Button btnSearch;
        private SAPbouiCOM.Matrix mtxCommissions;
        private SAPbouiCOM.StaticText lblTotal;
        private SAPbouiCOM.EditText txtTotal;
        private SAPbouiCOM.Button btnCreate;
        private SAPbouiCOM.DataTable dtCommissions;
        #endregion

    }
}
