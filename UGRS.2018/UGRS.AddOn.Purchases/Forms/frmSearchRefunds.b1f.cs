using System;
using System.Collections.Generic;
using System.Linq;
using SAPbouiCOM.Framework;
using SAPbouiCOM;
using UGRS.Core.SDK.UI;
using UGRS.Core.SDK.DI.Purchases.Tables;
using UGRS.Core.SDK.DI.Purchases.DTO;
using UGRS.Core.SDK.DI.Purchases.Enums;
using UGRS.Core.Extension.Enum;
using UGRS.Core.SDK.DI.Purchases;
using UGRS.Core.Services;
using UGRS.Core.SDK.UI.ProgressBar;

       

namespace UGRS.AddOn.Purchases.Forms
{
    [FormAttribute("UGRS.AddOn.Purchases.Forms.frmSearchRefunds", "Forms/frmSearchRefunds.b1f")]
    class frmSearchRefunds : UserFormBase
    {
        #region Properties
        ChooseFromList mObjCFLEmployee;
        string mStrEmployeId;
        int mIntRowSelected;
        PurchasesServiceFactory mObjPurchasesServiceFactory = new PurchasesServiceFactory();
        #endregion
        
        #region Contruct
        public frmSearchRefunds()
        {
        }
        #endregion
        
        #region initialize
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.lblArea = ((SAPbouiCOM.StaticText)(this.GetItem("lblArea").Specific));
            this.lblEmploye = ((SAPbouiCOM.StaticText)(this.GetItem("lblEmploye").Specific));
            this.lblStatus = ((SAPbouiCOM.StaticText)(this.GetItem("lblStatus").Specific));
            this.txtArea = ((SAPbouiCOM.EditText)(this.GetItem("txtArea").Specific));
            this.txtEmploye = ((SAPbouiCOM.EditText)(this.GetItem("txtEmploye").Specific));
            this.lblStartD = ((SAPbouiCOM.StaticText)(this.GetItem("lblStartD").Specific));
            this.lblEndDate = ((SAPbouiCOM.StaticText)(this.GetItem("lblEndDate").Specific));
            this.cboStatus = ((SAPbouiCOM.ComboBox)(this.GetItem("cboStatus").Specific));
            this.cboStatus.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cboStatus_ComboSelectAfter);
            this.txtStartD = ((SAPbouiCOM.EditText)(this.GetItem("txtStartD").Specific));
            this.txtEndDate = ((SAPbouiCOM.EditText)(this.GetItem("txtEndDate").Specific));
            this.btnSearch = ((SAPbouiCOM.Button)(this.GetItem("btnSearch").Specific));
            this.btnSearch.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSearch_ClickBefore);
            this.mtxRefound = ((SAPbouiCOM.Matrix)(this.GetItem("mtxRefound").Specific));
            this.mtxRefound.DoubleClickAfter += new SAPbouiCOM._IMatrixEvents_DoubleClickAfterEventHandler(this.mtxRefound_DoubleClickAfter);
            this.mtxRefound.ClickBefore += new SAPbouiCOM._IMatrixEvents_ClickBeforeEventHandler(this.mtxRefound_ClickBefore);
            this.btnReview = ((SAPbouiCOM.Button)(this.GetItem("btnReview").Specific));
            this.btnReview.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnReview_ClickBefore);
            //UGRS.Core.SDK.UI.UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(this.SBO_Application_ItemEvent);
            UGRS.Core.SDK.UI.UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(this.SBO_Application_ItemEvent);
            this.btnPayments = ((SAPbouiCOM.Button)(this.GetItem("btnPaym").Specific));
            this.btnPayments.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnPayments_ClickBefore);
            this.lblBank = ((SAPbouiCOM.StaticText)(this.GetItem("lblBank").Specific));
            this.cbBank = ((SAPbouiCOM.ComboBox)(this.GetItem("cbBank").Specific));
            this.cbBank.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cbBank_ComboSelectAfter);
            this.lblAccount = ((SAPbouiCOM.StaticText)(this.GetItem("lblAccount").Specific));
            this.cbAccount = ((SAPbouiCOM.ComboBox)(this.GetItem("cbAccount").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.ResizeAfter += new SAPbouiCOM.Framework.FormBase.ResizeAfterHandler(this.Form_ResizeAfter);
            this.CloseAfter += new CloseAfterHandler(this.Form_CloseAfter);

        }

        private void OnCustomInitialize()
        {
            try
            {
                 string lStrCostCenter = mObjPurchasesServiceFactory.GetPurchaseInvoiceService().GetCostCenter();
                //PermissionsEnum.Permission lObjPermissionEnum = mObjPurchasesServiceFactory.GetPurchasePermissionsService().GetPermissionType(lStrCostCenter, "U_GLO_ExpeCheck");
                string lStrCCTypeCode = mObjPurchasesServiceFactory.GetPurchaseInvoiceService().CCTypeCode(lStrCostCenter);
                HiddecontrolsPayments();
                if (lStrCCTypeCode == "O")
                {
                    txtArea.Value = lStrCostCenter;
                    txtEmploye.Item.Click();
                    txtArea.Item.Enabled = false;
                }
                else
                {
                    txtArea.Item.Enabled = true;
                    LoadChooseFromList();
                    SetChooseToTxt();
                }

                LoadCombobox();
                cbAccount.ValidValues.Add("", "");
                mtxRefound.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmSearchRefunds (OnCustomInitialize) " + ex.Message);
                LogService.WriteError(ex);
            }
        }
        #endregion

        #region Events
        private void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (FormUID.Equals(this.UIAPIRawForm.UniqueID))
                {
                    if (pVal.EventType == BoEventTypes.et_FORM_CLOSE)
                    {
                        UIApplication.GetApplication().ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(this.SBO_Application_ItemEvent);
                    }

                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case BoEventTypes.et_CHOOSE_FROM_LIST:
                                ChooseFromListAfterEvent(pVal);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("ItemEventException: {0}", ex.Message));
                LogService.WriteError("frmSearchRefunds (SBO_Application_ItemEvent) " + ex.Message);
                LogService.WriteError(ex);
            }
        }
       
        private void ChooseFromListAfterEvent(ItemEvent pObjValEvent)
        {
            try
            {
                if (pObjValEvent.Action_Success)
                {
                    SAPbouiCOM.IChooseFromListEvent lObjCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pObjValEvent;
                    if (lObjCFLEvento.SelectedObjects != null)
                    {
                        SAPbouiCOM.DataTable lObjDataTable = lObjCFLEvento.SelectedObjects;

                        if (lObjDataTable != null)
                        {
                            this.UIAPIRawForm.DataSources.UserDataSources.Item(lObjDataTable.UniqueID).ValueEx = System.Convert.ToString(lObjDataTable.GetValue(0, 0));

                            switch (lObjDataTable.UniqueID)
                            {
                                case "CFL_Area":
                                    string lStrArea = lObjDataTable.GetValue(0, 0).ToString();
                                    this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Area").ValueEx = lStrArea;
                                    this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Employ").ValueEx = "";
                                    AddConditionChoseFromListEmployee(mObjCFLEmployee, mObjPurchasesServiceFactory.GetPurchaseService().GetDepartment(lStrArea));
                                    break;

                                case "CFL_Employ":
                                    this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Employ").ValueEx = lObjDataTable.GetValue(2, 0).ToString() + " " + lObjDataTable.GetValue(3, 0).ToString() + lObjDataTable.GetValue(1, 0).ToString();
                                     mStrEmployeId = lObjDataTable.GetValue(0, 0).ToString();
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmSearchRefunds (ChooseFromListAfterEvent) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void btnSearch_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            Search();
        }

        private void mtxRefound_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (pVal.Row > 0)
                {
                    mtxRefound.SelectRow(pVal.Row, true, false);
                    mIntRowSelected = pVal.Row;

                    int lIntRowStatus = Convert.ToInt32(DtMatrix.GetValue("C_Status", mIntRowSelected - 1).ToString());
                    if (lIntRowStatus == 4)
                    {
                        ShowControlsPayment();
                    }
                    else
                    {
                        HiddecontrolsPayments();
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmSearchRefunds (mtxRefound_ClickBefore) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void btnReview_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            LoadFormReceipts();
        }

        private void LoadFormReceipts()
        {
            try
            {
                if (mIntRowSelected > 0)
                {
                    string pStrRowCode = DtMatrix.GetValue("C_RowCode", mIntRowSelected - 1).ToString();
                    string lStrArea = DtMatrix.GetValue("C_Area", mIntRowSelected -1).ToString();
                    frmReceipts lObjfrmReceipts = new frmReceipts(lStrArea, pStrRowCode, TypeEnum.Type.Refund);
                    lObjfrmReceipts.UIAPIRawForm.Left = 500;
                    lObjfrmReceipts.Show();
                    
                    
                }
                else
                {
                    UIApplication.ShowMessageBox("Favor de seleccionar un registro");
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmSearchRefunds (btnReview_ClickBefore) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void btnPayments_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            Form lObjPaymentForm = null;
            Form lObjPaymentFormUDF = null;
            Form lObjAccountForm = null;
            try
            {
                if (mIntRowSelected > 0)
                {
                   
                    StatusEnum lEnmStatus = (StatusEnum)int.Parse(DtMatrix.GetValue("C_Status", mIntRowSelected - 1).ToString());
                    string lStrFolio = DtMatrix.GetValue("C_Folio", mIntRowSelected - 1).ToString();
                    string lStrEmpCode = DtMatrix.GetValue("C_Employe", mIntRowSelected - 1).ToString();
                    string lStrArea = DtMatrix.GetValue("C_Area", mIntRowSelected - 1).ToString();
                    double lDblImport = double.Parse(DtMatrix.GetValue("C_Amount", mIntRowSelected - 1).ToString());
                    string lStrEmpName = mObjPurchasesServiceFactory.GetPurchaseService().GetEmpName(lStrEmpCode);
                    string lStrLineAccount = mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetAccountRefund(lStrArea);
                    //string lStrChkAcct = mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetAccountInConfig("GLO_CTAREEMBCHEQ");
                    string lStrChkAcct = cbAccount.Value;
                   // Dictionary<string, string> lStrBankInfo = mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetBankInfo(lStrChkAcct);
                    Dictionary<string, string> lStrBankInfo = new Dictionary<string, string>();
                    lStrBankInfo.Add(cbAccount.Value, cbBank.Selected.Description);

                    if (lEnmStatus != StatusEnum.Authorized_Ope_Admon)
                    {
                        UIApplication.ShowError("El reembolso seleccionado no está autorizado");
                        return;
                    }

                    string lStrValidation = ValidatePaymentsFields(lStrFolio, lStrEmpCode, lStrArea, lDblImport, lStrChkAcct, lStrBankInfo, lStrLineAccount);
                    if (!string.IsNullOrEmpty(lStrValidation))
                    {
                        UIApplication.ShowMessageBox(lStrValidation);
                        return;
                    }
                    if (mObjPurchasesServiceFactory.GetPurchaseReceiptsService().ExistsPayment(lStrEmpCode, lStrFolio, lStrArea))
                    /*string lStrDocEntry = DtMatrix.GetValue("C_DocEntry", mIntRowSelected - 1).ToString();
                   if (mObjPurchasesServiceFactory.GetPurchaseReceiptsService().ExistsPayment(lStrDocEntry))*/
                    {
                        UIApplication.ShowError("Ya existe un pago para el reembolso seleccionado");
                        return;
                    }
                    this.UIAPIRawForm.Freeze(true);
                    UIApplication.GetApplication().ActivateMenuItem("2818");
                    if (!UIApplication.GetApplication().Menus.Item("6913").Checked)
                    {
                        UIApplication.GetApplication().ActivateMenuItem("6913");//2050
                    }
                    
                    lObjPaymentForm = UIApplication.GetApplication().Forms.GetForm("426", -1);
                    lObjPaymentFormUDF = UIApplication.GetApplication().Forms.GetForm("-426", -1);

                    lObjPaymentForm.Freeze(true);
                    lObjPaymentFormUDF.Freeze(true);
                    SAPbouiCOM.EditText txtDocDate = ((SAPbouiCOM.EditText)lObjPaymentForm.Items.Item("10").Specific);
                    txtDocDate.Value = DateTime.Now.ToString("yyyyMMdd");

                    SAPbouiCOM.OptionBtn optionBtnAccount = ((SAPbouiCOM.OptionBtn)lObjPaymentForm.Items.Item("58").Specific);
                    optionBtnAccount.Selected = true;

                    SAPbouiCOM.ComboBox cboPymtType = ((SAPbouiCOM.ComboBox)lObjPaymentFormUDF.Items.Item("U_GLO_PaymentType").Specific);
                    cboPymtType.Select("GLREM", SAPbouiCOM.BoSearchKey.psk_ByValue);

                    SAPbouiCOM.EditText txtCodeMov = ((SAPbouiCOM.EditText)lObjPaymentFormUDF.Items.Item("U_GLO_CodeMov").Specific);
                    txtCodeMov.Value = lStrFolio;

                    SAPbouiCOM.ComboBox txtCostCenter = ((SAPbouiCOM.ComboBox)lObjPaymentFormUDF.Items.Item("U_GLO_CostCenter").Specific);
                    txtCostCenter.Select(lStrArea, SAPbouiCOM.BoSearchKey.psk_ByValue);

                    SAPbouiCOM.ComboBox cboAuxiliarType = ((SAPbouiCOM.ComboBox)lObjPaymentFormUDF.Items.Item("U_FZ_AuxiliarType").Specific);
                    cboAuxiliarType.Select("2", SAPbouiCOM.BoSearchKey.psk_ByValue);

                    SAPbouiCOM.EditText txtName = ((SAPbouiCOM.EditText)lObjPaymentForm.Items.Item("10000166").Specific);
                    txtName.Value = lStrEmpName;

                    SAPbouiCOM.EditText txtAuxiliar = ((SAPbouiCOM.EditText)lObjPaymentFormUDF.Items.Item("U_FZ_Auxiliar").Specific);
                    txtAuxiliar.Value = lStrEmpCode;


                    SAPbouiCOM.Button btnBank = ((SAPbouiCOM.Button)lObjPaymentForm.Items.Item("234000001").Specific);
                    btnBank.Item.Click();

                    lObjAccountForm = UIApplication.GetApplication().Forms.GetForm("196", -1);
                    lObjAccountForm.Freeze(true);
                    SAPbouiCOM.Folder folderCheck = ((SAPbouiCOM.Folder)lObjAccountForm.Items.Item("3").Specific);
                    folderCheck.Item.Click();

                    SAPbouiCOM.Matrix mtxCheck = ((SAPbouiCOM.Matrix)lObjAccountForm.Items.Item("28").Specific);
                    ((SAPbouiCOM.EditText)mtxCheck.Columns.Item("7").Cells.Item(1).Specific).Value = lDblImport.ToString();

                    ComboBox cb = ((SAPbouiCOM.ComboBox)mtxCheck.Columns.Item("2").Cells.Item(1).Specific);

                    string ss = lStrBankInfo[lStrBankInfo.Keys.ElementAt(0)]; //lStrBankInfo.ElementAt(0);


                    ((SAPbouiCOM.ComboBox)mtxCheck.Columns.Item("2").Cells.Item(1).Specific).Select(lStrBankInfo[lStrBankInfo.Keys.ElementAt(0)], SAPbouiCOM.BoSearchKey.psk_ByValue);//Banco
                    ((SAPbouiCOM.ComboBox)mtxCheck.Columns.Item("4").Cells.Item(1).Specific).Select(lStrBankInfo.Keys.ElementAt(0), SAPbouiCOM.BoSearchKey.psk_ByValue);//Cuenta?

                    SAPbouiCOM.Button btnOk = ((SAPbouiCOM.Button)lObjAccountForm.Items.Item("1").Specific);
                    btnOk.Item.Click();

                    SAPbouiCOM.Matrix mtxPayment = ((SAPbouiCOM.Matrix)lObjPaymentForm.Items.Item("71").Specific);
                    mtxPayment.AddRow();
                    ((SAPbouiCOM.EditText)mtxPayment.Columns.Item("5").Cells.Item(1).Specific).Value = lDblImport.ToString();
                    ((SAPbouiCOM.EditText)mtxPayment.Columns.Item("8").Cells.Item(1).Specific).Value = lStrLineAccount; //cuenta de mayor
                }
                else
                {
                    UIApplication.ShowMessageBox("Favor de seleccionar un registro");
                }

               
                    
                    
                    lObjPaymentFormUDF.Freeze(false);
                    this.UIAPIRawForm.Freeze(false);
                   
                
               
                if (lObjPaymentForm != null) lObjPaymentForm.Freeze(false);
                if (lObjAccountForm != null) lObjPaymentFormUDF.Freeze(false);
                if (this.UIAPIRawForm != null) this.UIAPIRawForm.Freeze(false);
               // if (lObjAccountForm. ==) lObjAccountForm.Freeze(false);
                UIApplication.ShowSuccess("Carga de pago terminada");
            }
            catch (Exception ex)
            {
               
                LogService.WriteError("frmSearchRefunds (btnPayments_ClickBefore) " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(string.Format("Error al abrir pantalla de pagos: {0}", ex.Message));
            }
            finally
            {
                if (lObjPaymentFormUDF != null)
                {
                    lObjPaymentFormUDF.Freeze(false);
                }
                this.UIAPIRawForm.Freeze(false);
               
            }
        }

        private string ValidatePaymentsFields(string pStrFolio, string pStrEmpCode, string pStrArea, double pDblImport, string pStrChkAcct, Dictionary<string, string> lStrBankInfo, string pStrLineAccount)
        {
            string lStrResult = string.Empty;

            if (string.IsNullOrEmpty(pStrFolio))
            {
                lStrResult = string.Join("\n", "- El folio del reembolso no tiene valor");
            }

            if (string.IsNullOrEmpty(pStrEmpCode))
            {
                lStrResult = string.Join("\n", "- El código del empleado no tiene valor", lStrResult);
            }

            if (string.IsNullOrEmpty(pStrArea))
            {
                lStrResult = string.Join("\n", "- El área del reembolso no tiene valor", lStrResult);
            }

            if (pDblImport <= 0)
            {
                lStrResult = string.Join("\n", "- El importe debe ser mayor a 0", lStrResult);
            }

            if (string.IsNullOrEmpty(pStrChkAcct))
            {
                lStrResult = string.Join("\n", "- La cuenta de cheque no tiene valor", lStrResult);
            }

            if (lStrBankInfo.Count == 0)
            {
                lStrResult = string.Join("\n", "- La información del banco para la cuenta cheque", lStrResult);
            }

            if (string.IsNullOrEmpty(pStrLineAccount))
            {
                lStrResult = string.Join("\n", "- La cuenta del reembolso no tiene valor", lStrResult);
            }

            if (!string.IsNullOrEmpty(lStrResult))
            {
                lStrResult = string.Format("Verificar los campos:\n {0}", lStrResult);
            }

            return lStrResult;
        }

        private void cboStatus_ComboSelectAfter(object sboObject, SBOItemEventArg pVal)
        {
            Search();
        }

        private void cbBank_ComboSelectAfter(object sboObject, SBOItemEventArg pVal)
        {
            LoadAccounts();
        }

        #endregion

        #region Methods

        #region ChooseFromList
        private void SetChooseToTxt()
        {
            try
            {
                txtArea.DataBind.SetBound(true, "", "CFL_Area");
                txtArea.ChooseFromListUID = "CFL_Area";
                txtArea.ChooseFromListAlias = "PrcCode";

                txtEmploye.DataBind.SetBound(true, "", "CFL_Employ");
                txtEmploye.ChooseFromListUID = "CFL_Employ";
                txtEmploye.ChooseFromListAlias = "lastName";
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmSearchRefunds (SetChooseToTxt) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void LoadChooseFromList()
        {
            try
            {
                ChooseFromList lObjCFLProjectArea = InitChooseFromLists(false, "61", "CFL_Area", this.UIAPIRawForm.ChooseFromLists);
                AddConditionChoseFromListArea(lObjCFLProjectArea);

                mObjCFLEmployee = InitChooseFromLists(false, "171", "CFL_Employ", this.UIAPIRawForm.ChooseFromLists);
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmSearchRefunds (LoadChooseFromList) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        #region ConditionsChose
        private void AddConditionChoseFromListArea(ChooseFromList pCFL)
        {
            try
            {


                SAPbouiCOM.Condition lObjCon = null;
                SAPbouiCOM.Conditions lObjCons = null;
                lObjCons = pCFL.GetConditions();

                //DimCode
                lObjCon = lObjCons.Add();
                lObjCon.Alias = "DimCode";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = "1";

                lObjCon.Relationship = BoConditionRelationship.cr_AND;

                lObjCon = lObjCons.Add();
                lObjCon.Alias = "Active";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = "Y";



                //lObjCon.Relationship = BoConditionRelationship.cr_AND;

                //lObjCon = lObjCons.Add();
                //lObjCon.Alias = "GrpCode";
                //lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_NULL;


                pCFL.SetConditions(lObjCons);
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmSearchRefunds (AddConditionChoseFromListArea) " + ex.Message);
                LogService.WriteError(ex);
            }

        }

        private void AddConditionChoseFromListEmployee(ChooseFromList pCFL, string pStrDept)
        {
            try
            {
                SAPbouiCOM.Condition lObjCon = null;
                SAPbouiCOM.Conditions lObjCons = new Conditions();
                pCFL.SetConditions(lObjCons);

                //DimCode
                lObjCon = lObjCons.Add();
                lObjCon.Alias = "dept";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = pStrDept;

                pCFL.SetConditions(lObjCons);
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmSearchRefunds (AddConditionChoseFromListEmployee) " + ex.Message);
                LogService.WriteError(ex);
            }

        }
        #endregion
        public ChooseFromList InitChooseFromLists(bool pbolMultiselecction, string pStrType, string pStrID, SAPbouiCOM.ChooseFromListCollection pObjCFLs) //
        {
            SAPbouiCOM.ChooseFromList lObjoCFL = null;
            try
            {
                SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
                oCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)UIApplication.GetApplication().CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);

                oCFLCreationParams.MultiSelection = pbolMultiselecction;
                oCFLCreationParams.ObjectType = pStrType;
                oCFLCreationParams.UniqueID = pStrID;

                lObjoCFL = pObjCFLs.Add(oCFLCreationParams);

                this.UIAPIRawForm.DataSources.UserDataSources.Add(pStrID, SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("InitCustomerChooseFromListException: {0}", ex.Message));
                LogService.WriteError("frmSearchRefunds (InitChooseFromLists) " + ex.Message);
                LogService.WriteError(ex);

            }
            return lObjoCFL;
        }
        #endregion

        private void Search()
        {
            try
            {

                SearchVouchersDTO lObjSearchVouchers = new SearchVouchersDTO();
                lObjSearchVouchers.Area = txtArea.Value;
                if (string.IsNullOrEmpty(txtEmploye.Value))
                {
                    mStrEmployeId = "";
                }
                if (string.IsNullOrEmpty(cboStatus.Value))
                {
                    lObjSearchVouchers.Status = 0;
                }
                else
                {
                    lObjSearchVouchers.Status = Convert.ToInt32(cboStatus.Value);
                }

                lObjSearchVouchers.Employee = mStrEmployeId;
                lObjSearchVouchers.StartDate = txtStartD.Value;
                lObjSearchVouchers.EndDate = txtEndDate.Value;

                List<Vouchers> lLstVouchers = new List<Vouchers>();
                lLstVouchers = mObjPurchasesServiceFactory.GetPurchaseVouchersService().GetVouchersList(lObjSearchVouchers).ToList();
                if (DtMatrix == null)
                {
                    CreateDataTableMatrix();
                }
                else
                {
                    DtMatrix.Rows.Clear();
                }
                FillMatrix(lLstVouchers);
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("frmSearchRefunds (btnSearch_ClickBefore) " + ex.Message);
                LogService.WriteError(ex);
            }
        }
        private void LoadCombobox()
        {
            try
            {
                List<StatusEnum> lLstStatusEnum = Enum.GetValues(typeof(StatusEnum)).Cast<StatusEnum>().ToList();
                cboStatus.ValidValues.Add("", "");
                foreach (StatusEnum lObjStatusEnum in lLstStatusEnum)
                {
                    cboStatus.ValidValues.Add(((int)lObjStatusEnum).ToString(), lObjStatusEnum.GetDescription());
                }
                cboStatus.Item.DisplayDesc = true;
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmSearchRefunds (LoadCombobox) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void LoadCombobxBank()
        {
           

            IList<BankDTO> lLstObjBanks = mObjPurchasesServiceFactory.GetPurchaseVouchersService().GetBankList();
            cbBank.ValidValues.Add("", "");
            foreach (BankDTO lObjBank in lLstObjBanks)
            {
                cbBank.ValidValues.Add(lObjBank.BankCode, lObjBank.BankName);
            }
        }



        private void CreateDataTableMatrix()
        {
            try
            {
                this.UIAPIRawForm.DataSources.DataTables.Add("DsVouchers");
                DtMatrix = this.UIAPIRawForm.DataSources.DataTables.Item("DsVouchers");
                DtMatrix.Columns.Add("#", SAPbouiCOM.BoFieldsType.ft_ShortNumber);
                DtMatrix.Columns.Add("C_RowCode", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Folio", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Status", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_StatusDescription", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Area", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Employe", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Date", SAPbouiCOM.BoFieldsType.ft_Date);
                DtMatrix.Columns.Add("C_Amount", SAPbouiCOM.BoFieldsType.ft_Price);
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmSearchRefunds (CreateDataTableMatrix) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void FillMatrix(List<Vouchers> pLstVouchers)
        {
            try
            {
                int i = 0;
                this.UIAPIRawForm.Freeze(true);
                foreach (Vouchers lObjVoucher in pLstVouchers)
                {
                    DtMatrix.Rows.Add();
                    DtMatrix.SetValue("#", i, i + 1);
                    DtMatrix.SetValue("C_RowCode", i, lObjVoucher.RowCode);
                    DtMatrix.SetValue("C_Folio", i, lObjVoucher.Folio);
                    DtMatrix.SetValue("C_Status", i, lObjVoucher.Status);
                    DtMatrix.SetValue("C_StatusDescription", i, ((StatusEnum)lObjVoucher.Status).GetDescription());
                    DtMatrix.SetValue("C_Area", i, lObjVoucher.Area);
                    DtMatrix.SetValue("C_Employe", i, lObjVoucher.Employee);
                    DtMatrix.SetValue("C_Date", i, lObjVoucher.Date);
                    DtMatrix.SetValue("C_Amount", i, lObjVoucher.Total);
                    i++;
                }

                mtxRefound.Columns.Item("#").DataBind.Bind("DsVouchers", "#");
                mtxRefound.Columns.Item("C_Folio").DataBind.Bind("DsVouchers", "C_Folio");
                mtxRefound.Columns.Item("C_Status").DataBind.Bind("DsVouchers", "C_StatusDescription");
                mtxRefound.Columns.Item("C_Area").DataBind.Bind("DsVouchers", "C_Area");
                mtxRefound.Columns.Item("C_Employe").DataBind.Bind("DsVouchers", "C_Employe");
                mtxRefound.Columns.Item("C_Date").DataBind.Bind("DsVouchers", "C_Date");
                mtxRefound.Columns.Item("C_Amount").DataBind.Bind("DsVouchers", "C_Amount");
                mtxRefound.LoadFromDataSource();
                mtxRefound.AutoResizeColumns();
                this.UIAPIRawForm.Freeze(false);
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("frmSearchRefunds (frmPurchaseNotes) " + ex.Message);
                LogService.WriteError(ex);

            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        private void ShowControlsPayment()
        {
            btnPayments.Item.Visible = true;
            lblBank.Item.Visible = true;
            lblAccount.Item.Visible = true;
            cbBank.Item.Visible = true;
            cbAccount.Item.Visible = true;
            LoadCombobxBank();
        }

        private void HiddecontrolsPayments()
        {
            btnPayments.Item.Visible = false;
            lblBank.Item.Visible = false;
            lblAccount.Item.Visible = false;
            cbBank.Item.Visible = false;
            cbAccount.Item.Visible = false;
        }

 /// <summary>
        /// Loads the available bank accounts in the comoboxes.
        /// </summary>
        public void LoadAccounts()
        {
           
            cbAccount.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
            //Remove old valid values
            int lIntValuesLength = cbAccount.ValidValues.Count - 1;
            for (int i = 0; i < lIntValuesLength; i++)
            {
                cbAccount.ValidValues.Remove(1, SAPbouiCOM.BoSearchKey.psk_Index);
            }

            cbAccount.Item.Enabled = true;

            List<AccountDTO> lLstAccounts = new List<AccountDTO>();
            lLstAccounts =  mObjPurchasesServiceFactory.GetPurchaseVouchersService().GetBankAccountList(cbBank.Value.ToString()).ToList();
            foreach (AccountDTO lObjAccount in lLstAccounts)
            {
                cbAccount.ValidValues.Add(lObjAccount.Account, lObjAccount.Account);
            }
        }
        #endregion

        #region controls
        private SAPbouiCOM.StaticText lblArea;
        private SAPbouiCOM.StaticText lblEmploye;
        private SAPbouiCOM.StaticText lblStatus;
        private SAPbouiCOM.EditText txtArea;
        private SAPbouiCOM.EditText txtEmploye;
        private SAPbouiCOM.StaticText lblStartD;
        private SAPbouiCOM.StaticText lblEndDate;
        private SAPbouiCOM.ComboBox cboStatus;
        private SAPbouiCOM.EditText txtStartD;
        private SAPbouiCOM.EditText txtEndDate;
        private SAPbouiCOM.Button btnSearch;
        private SAPbouiCOM.Matrix mtxRefound;
        private SAPbouiCOM.Button btnReview;
        private SAPbouiCOM.DataTable DtMatrix;
        private SAPbouiCOM.Button btnPayments;
        private StaticText lblBank;
        private ComboBox cbBank;
        private StaticText lblAccount;
        private ComboBox cbAccount;
        #endregion

        private void Form_ResizeAfter(SBOItemEventArg pVal)
        {
            mtxRefound.AutoResizeColumns();

        }

        private void Form_CloseAfter(SBOItemEventArg pVal)
        {
            UnLoadEvents();

        }

        #region Load & Unload Events
        private void LoadEvents()
        {
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }
        private void UnLoadEvents()
        {
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }
        #endregion

        private void mtxRefound_DoubleClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            if (mIntRowSelected > 0)
            {
                LoadFormReceipts();
            }

        }
       

    }
}
