using System;
using System.Collections.Generic;
using System.Linq;
using SAPbouiCOM.Framework;
using SAPbouiCOM;
using UGRS.Core.SDK.UI;
using UGRS.Core.SDK.DI.Purchases.DTO;
using UGRS.Core.SDK.DI.Purchases.Enums;
using UGRS.Core.Extension.Enum;
using UGRS.Core.SDK.DI.Purchases;
using UGRS.Core.Services;
using UGRS.Core.SDK.DI.Purchases.Tables;


namespace UGRS.AddOn.Purchases.Forms
{
    [FormAttribute("UGRS.AddOn.Purchases.Forms.frmCheeckingCosts", "Forms/frmCheeckingCosts.b1f")]
    class frmCheeckingCosts : UserFormBase
    {
        int mIntRowSelected;
        PurchasesServiceFactory mObjPurchasesServiceFactory = new PurchasesServiceFactory();
        public frmCheeckingCosts()
        {
        }

        #region Initialize
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.lblArea = ((SAPbouiCOM.StaticText)(this.GetItem("lblArea").Specific));
            this.lblStatus = ((SAPbouiCOM.StaticText)(this.GetItem("lblStatus").Specific));
            this.txtArea = ((SAPbouiCOM.EditText)(this.GetItem("txtArea").Specific));
            this.btnSearch = ((SAPbouiCOM.Button)(this.GetItem("btnSearch").Specific));
            this.btnSearch.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSearch_ClickBefore);
            this.mtxCosts = ((SAPbouiCOM.Matrix)(this.GetItem("mtxCosts").Specific));
            this.mtxCosts.DoubleClickAfter += new SAPbouiCOM._IMatrixEvents_DoubleClickAfterEventHandler(this.mtxCosts_DoubleClickAfter);
            this.mtxCosts.ClickBefore += new SAPbouiCOM._IMatrixEvents_ClickBeforeEventHandler(this.mtxCosts_ClickBefore);
            this.cboStatus = ((SAPbouiCOM.ComboBox)(this.GetItem("cboStatus").Specific));
            UGRS.Core.SDK.UI.UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(this.SBO_Application_ItemEvent);
            this.btnEdit = ((SAPbouiCOM.Button)(this.GetItem("btnEdit").Specific));
            this.btnEdit.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnEdit_ClickBefore);
            this.btnPaym = ((SAPbouiCOM.Button)(this.GetItem("btnPaym").Specific));
            this.btnPaym.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnPaym_ClickBefore);
            this.lblBank = ((SAPbouiCOM.StaticText)(this.GetItem("lblBank").Specific));
            this.cbAccount = ((SAPbouiCOM.ComboBox)(this.GetItem("cbAccount").Specific));
            this.lblAccount = ((SAPbouiCOM.StaticText)(this.GetItem("lblAccount").Specific));
            this.cbBank = ((SAPbouiCOM.ComboBox)(this.GetItem("cbBank").Specific));
            this.cbBank.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cbBank_ComboSelectAfter);
            this.OnCustomInitialize();

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

                if (lStrCCTypeCode == "O")
                {
                    txtArea.Value = lStrCostCenter;
                    cboStatus.Item.Click();
                    txtArea.Item.Enabled = false;
                }
                else
                {
                    txtArea.Item.Enabled = true; 
                    LoadChooseFromList();
                    SetChooseToTxt();
                }

                LoadCombobox();
                LoadCombobxBank();
                CreateDataTableMatrix();
                mtxCosts.AutoResizeColumns();
                HiddecontrolsPayments();
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
                LogService.WriteError("(SBO_Application_ItemEvent): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void ChooseFromListAfterEvent(ItemEvent pObjValEvent)
        {
            try
            {
                if (pObjValEvent.Action_Success)
                {
                    SAPbouiCOM.IChooseFromListEvent lObjCFLEvent = (SAPbouiCOM.IChooseFromListEvent)pObjValEvent;
                    if (lObjCFLEvent.SelectedObjects != null)
                    {
                        SAPbouiCOM.DataTable lObjDataTable = lObjCFLEvent.SelectedObjects;

                        if (lObjDataTable != null)
                        {
                            this.UIAPIRawForm.DataSources.UserDataSources.Item(lObjDataTable.UniqueID).ValueEx = System.Convert.ToString(lObjDataTable.GetValue(0, 0));

                            switch (lObjDataTable.UniqueID)
                            {
                                case "CFL_Area":
                                    //txtArea.Value = lObjDataTable.GetValue(0, 0).ToString();
                                    this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Area").ValueEx = lObjDataTable.GetValue(0, 0).ToString();
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogService.WriteError("(ChooseFromListAfterEvent): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void btnSearch_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            FillMatrix();

        }

        private void mtxCosts_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (pVal.Row > 0)
                {
                    mtxCosts.SelectRow(pVal.Row, true, false);
                    mIntRowSelected = pVal.Row;

                    decimal lIntRowPend = Convert.ToDecimal(DtMatrix.GetValue("C_SaldoPen", mIntRowSelected - 1).ToString());

                    int lIntRowStatus = Convert.ToInt32(DtMatrix.GetValue("C_Status", mIntRowSelected - 1).ToString());
                    if (lIntRowStatus == 4 && lIntRowPend < 0)
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
                UIApplication.ShowError(ex.Message);
                LogService.WriteError("(mtxCosts_ClickBefore): " + ex.Message);
                LogService.WriteError(ex);
            }

        }

        private void LoadCombobxBank()
        {
            int lIntValuesLength = cbBank.ValidValues.Count - 1;
            for (int i = 0; i < lIntValuesLength + 1; i++)
            {
                cbBank.ValidValues.Remove(0, SAPbouiCOM.BoSearchKey.psk_Index);
            }

            IList<BankDTO> lLstObjBanks = mObjPurchasesServiceFactory.GetPurchaseVouchersService().GetBankList();
            cbBank.ValidValues.Add("", "");
            foreach (BankDTO lObjBank in lLstObjBanks)
            {
                cbBank.ValidValues.Add(lObjBank.BankCode, lObjBank.BankName);
            }
        }

        public void LoadAccounts()
        {
            try
            {
               
                //Remove old valid values
                int lIntValuesLength = cbAccount.ValidValues.Count;
                for (int i = 0; i < lIntValuesLength; i++)
                {
                    cbAccount.ValidValues.Remove(0, SAPbouiCOM.BoSearchKey.psk_Index);
                }

                cbAccount.Item.Enabled = true;

                List<AccountDTO> lLstAccounts = new List<AccountDTO>();
                lLstAccounts = mObjPurchasesServiceFactory.GetPurchaseVouchersService().GetBankAccountList(cbBank.Value.ToString()).ToList();
                cbAccount.ValidValues.Add("", "");
                foreach (AccountDTO lObjAccount in lLstAccounts)
                {
                    cbAccount.ValidValues.Add(lObjAccount.Account, lObjAccount.Account);
                }
                cbAccount.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmSearchRefunds (LoadAccounts) " + ex.Message);
                LogService.WriteError(ex);

            }
        }

        private void btnEdit_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            LoadFormReceipt();
        }
        #endregion

        #region Methods
        
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
                LogService.WriteError("(LoadCombobox): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        #region Matrix
        private void CreateDataTableMatrix()
        {
            try
            {
                this.UIAPIRawForm.DataSources.DataTables.Add("PaymentsDataTable");
                DtMatrix = this.UIAPIRawForm.DataSources.DataTables.Item("PaymentsDataTable");
                DtMatrix.Columns.Add("#", SAPbouiCOM.BoFieldsType.ft_ShortNumber);
                /*DtMatrix.Columns.Add("C_DocEntry", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_DocNum", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);*/
                DtMatrix.Columns.Add("C_Folio", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Status", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_StatusDescription", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric); 
                DtMatrix.Columns.Add("C_EmpId", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Employe", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Area", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
               // DtMatrix.Columns.Add("C_Date", SAPbouiCOM.BoFieldsType.ft_Date); Cambio 
                DtMatrix.Columns.Add("C_ImpSol", SAPbouiCOM.BoFieldsType.ft_Price);
                DtMatrix.Columns.Add("C_ImpComp", SAPbouiCOM.BoFieldsType.ft_Price);
                DtMatrix.Columns.Add("C_ImpSob", SAPbouiCOM.BoFieldsType.ft_Price);
                DtMatrix.Columns.Add("C_ImpFalt", SAPbouiCOM.BoFieldsType.ft_Price);
                DtMatrix.Columns.Add("C_SaldoPen", SAPbouiCOM.BoFieldsType.ft_Price);
            }
            catch (Exception ex)
            {
                LogService.WriteError("(CreateDataTableMatrix): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void FillMatrix()
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);
                if (DtMatrix != null)
                {
                    DtMatrix.Rows.Clear();
                }
                List<PaymentDTO> lLstPaymentDTO = mObjPurchasesServiceFactory.GetPurchaseCheeckingCostService().GetPayment(txtArea.Value, cboStatus.Value, !txtArea.Item.Enabled).ToList();

                int i = 0;

                foreach (PaymentDTO lObjPaymentDTO in lLstPaymentDTO)
                {
                    DtMatrix.Rows.Add();
                    DtMatrix.SetValue("#", i, i + 1);
                    /*DtMatrix.SetValue("C_DocEntry", i, lObjPaymentDTO.DocEntry);
                    DtMatrix.SetValue("C_DocNum", i, lObjPaymentDTO.DocNum);*/
                    DtMatrix.SetValue("C_Folio", i, lObjPaymentDTO.Folio);
                    string lStrStatus = "";
                    if (lObjPaymentDTO.Status == "0" || lObjPaymentDTO.Status == "")
                    {
                        lStrStatus = "";
                    }
                    else
                    {
                        StatusEnum lObjStatusEnum = (StatusEnum)Convert.ToInt16(lObjPaymentDTO.Status);
                        lStrStatus = lObjStatusEnum.GetDescription();
                    }

                    DtMatrix.SetValue("C_Status", i, lObjPaymentDTO.Status);
                  
                    int lIntStatus = 0;
                    if (int.TryParse(lObjPaymentDTO.Status, out lIntStatus))
                    {
                        lIntStatus = Convert.ToInt32(lObjPaymentDTO.Status);
                    }

                    DtMatrix.SetValue("C_Status", i, lIntStatus.ToString());
                    DtMatrix.SetValue("C_StatusDescription", i, lStrStatus);
                    DtMatrix.SetValue("C_EmpId", i, lObjPaymentDTO.EmpId);
                    DtMatrix.SetValue("C_Employe", i, lObjPaymentDTO.Employee);
                    DtMatrix.SetValue("C_Area", i, lObjPaymentDTO.Area);
                   // DtMatrix.SetValue("C_Date", i, Convert.ToDateTime(lObjPaymentDTO.Date));
                    DtMatrix.SetValue("C_ImpSol", i, lObjPaymentDTO.ImpSol);
                    DtMatrix.SetValue("C_ImpComp", i, lObjPaymentDTO.ImpComp);
                    DtMatrix.SetValue("C_ImpFalt", i, lObjPaymentDTO.ImpFalt);
                    DtMatrix.SetValue("C_ImpSob", i, lObjPaymentDTO.ImpSob);
                    DtMatrix.SetValue("C_SaldoPen", i, lObjPaymentDTO.SaldoPen);
                    i++;
                }

                mtxCosts.Columns.Item("#").DataBind.Bind("PaymentsDataTable", "#");
                mtxCosts.Columns.Item("C_Folio").DataBind.Bind("PaymentsDataTable", "C_Folio");
                //mtxCosts.Columns.Item("C_DocEntry").DataBind.Bind("PaymentsDataTable", "C_DocNum");
                mtxCosts.Columns.Item("C_Status").DataBind.Bind("PaymentsDataTable", "C_StatusDescription");
                mtxCosts.Columns.Item("C_Employe").DataBind.Bind("PaymentsDataTable", "C_Employe");
                mtxCosts.Columns.Item("C_Area").DataBind.Bind("PaymentsDataTable", "C_Area");
                //mtxCosts.Columns.Item("C_Date").DataBind.Bind("PaymentsDataTable", "C_Date");
                mtxCosts.Columns.Item("C_ImpSol").DataBind.Bind("PaymentsDataTable", "C_ImpSol");
                mtxCosts.Columns.Item("C_ImpComp").DataBind.Bind("PaymentsDataTable", "C_ImpComp");
                mtxCosts.Columns.Item("C_ImpFalt").DataBind.Bind("PaymentsDataTable", "C_ImpFalt");
                mtxCosts.Columns.Item("C_ImpSob").DataBind.Bind("PaymentsDataTable", "C_ImpSob");
                mtxCosts.Columns.Item("C_SaldoPen").DataBind.Bind("PaymentsDataTable", "C_SaldoPen");

              
                mtxCosts.LoadFromDataSource();
                mtxCosts.AutoResizeColumns();
                this.UIAPIRawForm.Freeze(false);
                LogService.WriteInfo("(frmCheckingCost) Matriz Cargada correctamente: Area " + txtArea.Value + ", Estatus " + cboStatus.Value);
               
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("(FillMatrix): " + ex.Message);
                LogService.WriteError(ex);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        ////Tarda bastate tiempo :(
        //private bool UpdateStatus(List<PaymentDTO> pLstPaymentDTO)
        //{
        //    bool lBolUpdateStatus = false;
        //    foreach (PaymentDTO lObjPaymentDTO in pLstPaymentDTO)
        //    {
        //        string lStrCode = mObjPurchasesServiceFactory.GetPurchaseCheeckingCostService().CheckingCost(lObjPaymentDTO.Folio);
        //        List<VouchersDetail> lLstVouchersDetails = mObjPurchasesServiceFactory.GetPurchaseVouchersService().GetVouchesDetail(lStrCode);

        //        foreach (VouchersDetail lObjVoucherDetail in lLstVouchersDetails)
        //        {
        //           lBolUpdateStatus = mObjPurchasesServiceFactory.GetPurchaseInvoiceService().UpdateStatus(lObjVoucherDetail);
        //        }
        //    }
        //    return lBolUpdateStatus;
        //}



        #endregion
        private void ShowControlsPayment()
        {
            btnPaym.Item.Visible = true;
            lblBank.Item.Visible = true;
            lblAccount.Item.Visible = true;
            cbBank.Item.Visible = true;
            cbAccount.Item.Visible = true;
            LoadCombobxBank();
        }

        private void HiddecontrolsPayments()
        {
            btnPaym.Item.Visible = false;
            lblBank.Item.Visible = false;
            lblAccount.Item.Visible = false;
            cbBank.Item.Visible = false;
            cbAccount.Item.Visible = false;
        }


        #region ChooseFromList
        private void SetChooseToTxt()
        {
            try
            {
                txtArea.DataBind.SetBound(true, "", "CFL_Area");
                txtArea.ChooseFromListUID = "CFL_Area";
                txtArea.ChooseFromListAlias = "PrcCode";
            }
            catch (Exception ex)
            {
                LogService.WriteError("(SetChooseToTxt): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void LoadChooseFromList()
        {
            try
            {
                ChooseFromList lObjCFLProjectArea = InitChooseFromLists(false, "61", "CFL_Area", this.UIAPIRawForm.ChooseFromLists);
                AddConditionChoseFromListArea(lObjCFLProjectArea);
            }
            catch (Exception ex)
            {
                LogService.WriteError("(LoadChooseFromList): " + ex.Message);
                LogService.WriteError(ex);
            }
         
        }

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
                LogService.WriteError("(AddConditionChoseFromListArea): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

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
                LogService.WriteError("(InitChooseFromLists): " + ex.Message);
                LogService.WriteError(ex);
            }
            return lObjoCFL;
        }

        #endregion
        #endregion

        #region Controls
        private SAPbouiCOM.StaticText lblArea;
        private SAPbouiCOM.StaticText lblStatus;
        private SAPbouiCOM.EditText txtArea;
        private SAPbouiCOM.Button btnSearch;
        private SAPbouiCOM.Matrix mtxCosts;
        private SAPbouiCOM.ComboBox cboStatus;
        private SAPbouiCOM.DataTable DtMatrix; 
        private Button btnEdit;
        private Button btnPaym;
        private StaticText lblBank;
        private ComboBox cbAccount;
        private StaticText lblAccount;
        private ComboBox cbBank;
        #endregion

        private void Form_ResizeAfter(SBOItemEventArg pVal)
        {
            mtxCosts.AutoResizeColumns();

        }

        private void cbBank_ComboSelectAfter(object sboObject, SBOItemEventArg pVal)
        {
            LoadAccounts();

        }


        private void LoadFormReceipt()
        {
            try
            {
                if (mIntRowSelected > 0 && mtxCosts.IsRowSelected(mIntRowSelected))
                {
                    string pStrRowCode = DtMatrix.GetValue("C_Folio", mIntRowSelected - 1).ToString();

                    //List<Vouchers> lObjVouchers = mObjPurchasesDAO.GetVouches(pStrRowCode);
                    //if (lObjVouchers.Count() > 0)
                    //{
                    //    frmReceipts lObjfrmReceipts = new frmReceipts(pStrRowCode);
                    //    lObjfrmReceipts.Show();
                    //}
                    //else
                    //{
                    string lStrArea = DtMatrix.GetValue("C_Area", mIntRowSelected - 1).ToString();
                    string lStrEmployee = DtMatrix.GetValue("C_Employe", mIntRowSelected - 1).ToString();
                    string lStrFolio = DtMatrix.GetValue("C_Folio", mIntRowSelected - 1).ToString();
                    string lStrEmpId = DtMatrix.GetValue("C_EmpId", mIntRowSelected - 1).ToString();
                    string lStrStatus = DtMatrix.GetValue("C_Status", mIntRowSelected - 1).ToString();

                    string lStrCode = mObjPurchasesServiceFactory.GetPurchaseCheeckingCostService().CheckingCost(lStrFolio);

                    if (string.IsNullOrEmpty(lStrStatus) && !string.IsNullOrEmpty(lStrCode))
                    {
                        UIApplication.ShowMessageBox("Ya existe un comprobante registrado");
                    }
                    else if (!string.IsNullOrEmpty(lStrCode))
                    {
                        frmReceipts lObjfrmReceipts = new frmReceipts(lStrCode, TypeEnum.Type.Voucher);
                        lObjfrmReceipts.UIAPIRawForm.Left = 500;
                        lObjfrmReceipts.UIAPIRawForm.Top = 10;
                        lObjfrmReceipts.Show();
                    }
                    else
                    {
                        frmReceipts lObjfrmReceipts = new frmReceipts(lStrArea, lStrEmployee, lStrEmpId, lStrFolio, TypeEnum.Type.Voucher);
                        lObjfrmReceipts.UIAPIRawForm.Left = 500;
                        lObjfrmReceipts.UIAPIRawForm.Top = 10;
                        lObjfrmReceipts.Show();

                    }
                    //}
                }
                else
                {
                    UIApplication.ShowMessageBox("Favor de seleccionar una linea");
                }
            }
            catch (Exception ex)
            {

                LogService.WriteError("(btnEdit_ClickBefore): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        public void LoadPayment()
        {
            
            Form lObjPaymentForm = null;
            Form lObjPaymentFormUDF = null;
            Form lObjAccountForm = null;
            try
            {
                if (mIntRowSelected > 0)
                {
                  
                    StatusEnum lEnmStatus = (StatusEnum)int.Parse(DtMatrix.GetValue("C_Status", mIntRowSelected - 1).ToString());
                    string lStrFolio = DtMatrix.GetValue("C_Folio", mIntRowSelected - 1).ToString();
                    string lStrEmpCode = DtMatrix.GetValue("C_EmpId", mIntRowSelected - 1).ToString();
                    string lStrArea = DtMatrix.GetValue("C_Area", mIntRowSelected - 1).ToString();
                    double lDblImport = double.Parse(DtMatrix.GetValue("C_SaldoPen", mIntRowSelected - 1).ToString());
                    lDblImport = lDblImport * -1;
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
                    cboPymtType.Select("GLREG", SAPbouiCOM.BoSearchKey.psk_ByValue);

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

        private void btnPaym_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            LoadPayment();

        }

        private void Form_CloseAfter(SBOItemEventArg pVal)
        {
            UnLoadEvents();

        }

        private void mtxCosts_DoubleClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            LoadFormReceipt();

        }
    }
}
