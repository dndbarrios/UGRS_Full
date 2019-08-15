using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UGRS.Core.Extension.Enum;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.Purchases;
using UGRS.Core.SDK.DI.Purchases.DTO;
using UGRS.Core.SDK.DI.Purchases.Enums;
using UGRS.Core.SDK.DI.Purchases.Services;
using UGRS.Core.SDK.DI.Purchases.Tables;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;
using UGRS.Core.SDK.DI.Purchases.DAO;
using UGRS.Core.SDK.UI.ProgressBar;


namespace UGRS.AddOn.Purchases.Forms
{
    [FormAttribute("UGRS.AddOn.Purchases.Forms.frmReceipts", "Forms/frmReceipts.b1f")]
    class frmReceipts : UserFormBase
    {
        #region Properties
        ChooseFromList mObjCFLEmployee;
        PurchasesServiceFactory mObjPurchasesServiceFactory = new PurchasesServiceFactory();
        string mStrEmployeId;
        int mIntRowSelected = 0;
        string mStrRowCode = string.Empty;
        string mStrUserCode = string.Empty;
        TypeEnum.Type mEnumType;
        string mStrType;
        string mStrTypePrefix;
        private UGRS.Core.SDK.UI.ProgressBar.ProgressBarManager mObjProgressBar = null;

        // int mIntQtyRefresh = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// inicia la ventana
        /// <summary>
        public frmReceipts(TypeEnum.Type pEnumType)
        {
            try
            {


                mStrUserCode = UIApplication.GetCompany().UserName;
                LogService.WriteInfo("Pantalla de comprobantes cargada correctamente");
                mEnumType = pEnumType;// TypeEnum.Type.Refund;

                btnCFDI.Item.Visible = true;
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmReceipts: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// inicia la ventana seleccionando un codigo
        /// <summary>
        public frmReceipts(string pStrRowCode, TypeEnum.Type pEnumType)
        {
            // OnCustomInitialize();
            mEnumType = pEnumType;// TypeEnum.Type.Refund;
            if (mEnumType == TypeEnum.Type.Refund)
            {
                mStrType = "U_GLO_Refund";
            }
            else
            {
                mStrType = "U_GLO_ExpeCheck";
            }
            //mStrType = "U_GLO_Refund";
            LoadVoucher(pStrRowCode);
            LogService.WriteInfo("Pantalla de comprobantes con un folio cargada correctamente");
        }

        /// <summary>
        /// inicia la ventana seleccionando una solicitud de viaticos
        /// <summary>
        public frmReceipts(string pStrArea, string pStrEmployee, string pStrEmployeeId, string pStrCodeMov, TypeEnum.Type pEnumType)
        {
            try
            {
                mEnumType = pEnumType;// TypeEnum.Type.Voucher;
                if (mEnumType == TypeEnum.Type.Refund)
                {
                    mStrType = "U_GLO_Refund";
                }
                else
                {
                    mStrType = "U_GLO_ExpeCheck";
                }

                txtComents.Item.Click();
                DisableControls();
                txtAreaF.Value = pStrArea;
                txtEmpF.Value = pStrEmployee;
                this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Area").ValueEx = pStrArea;
                this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Employ").ValueEx = pStrEmployeeId;
                lblCodeMov.Item.Visible = true;
                txtCodeMov.Item.Visible = true;

                txtCodeMov.Value = pStrCodeMov;
                mStrEmployeId = pStrEmployeeId;

                int lIntFolio = (Convert.ToInt32(mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetVoucherFolio(txtArea.Value, ((int)mEnumType).ToString())));
                txtFolio.Value = (lIntFolio + 1).ToString();

                LogService.WriteInfo("Pantalla de comprobantes desde una solicitud de viaticos cargada correctamente");

            }
            catch (Exception ex)
            {

                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("frmReceipts: " + ex.Message);
                LogService.WriteError(ex);
            }




        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            //          try
            //          {
            this.lblFolio = ((SAPbouiCOM.StaticText)(this.GetItem("lblFolio").Specific));
            this.lblArea = ((SAPbouiCOM.StaticText)(this.GetItem("lblArea").Specific));
            this.lblEmploye = ((SAPbouiCOM.StaticText)(this.GetItem("lblEmploye").Specific));
            this.lblComents = ((SAPbouiCOM.StaticText)(this.GetItem("lblComents").Specific));
            this.txtFolio = ((SAPbouiCOM.EditText)(this.GetItem("txtFolio").Specific));
            this.txtArea = ((SAPbouiCOM.EditText)(this.GetItem("txtArea").Specific));
            this.txtEmploye = ((SAPbouiCOM.EditText)(this.GetItem("txtEmploye").Specific));
            this.txtComents = ((SAPbouiCOM.EditText)(this.GetItem("txtComents").Specific));
            this.lblFecha = ((SAPbouiCOM.StaticText)(this.GetItem("lblFecha").Specific));
            this.lblTotal = ((SAPbouiCOM.StaticText)(this.GetItem("lblTotal").Specific));
            this.lblStatus = ((SAPbouiCOM.StaticText)(this.GetItem("lblStatus").Specific));
            this.txtDate = ((SAPbouiCOM.EditText)(this.GetItem("txtDate").Specific));
            this.txtTotal = ((SAPbouiCOM.EditText)(this.GetItem("txtTotal").Specific));
            this.btnCFDI = ((SAPbouiCOM.Button)(this.GetItem("btnCFDI").Specific));
            this.btnCFDI.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btnCFDI_ClickAfter);
            this.btnNotes = ((SAPbouiCOM.Button)(this.GetItem("btnNotes").Specific));
            this.btnNotes.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btnNotes_ClickAfter);
            this.btnCancel = ((SAPbouiCOM.Button)(this.GetItem("btnCancel").Specific));
            this.btnCancel.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCancel_ClickBefore);
            this.mtxReceipt = ((SAPbouiCOM.Matrix)(this.GetItem("mtxReceipt").Specific));
            this.mtxReceipt.ValidateAfter += new SAPbouiCOM._IMatrixEvents_ValidateAfterEventHandler(this.mtxReceipt_ValidateAfter);
            this.mtxReceipt.LinkPressedBefore += new SAPbouiCOM._IMatrixEvents_LinkPressedBeforeEventHandler(this.mtxReceipt_LinkPressedBefore);
            this.mtxReceipt.ClickBefore += new SAPbouiCOM._IMatrixEvents_ClickBeforeEventHandler(this.mtxReceipt_ClickBefore);
            this.btnSave = ((SAPbouiCOM.Button)(this.GetItem("btnSave").Specific));
            this.btnSave.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSave_ClickBefore);
            this.btnNotify = ((SAPbouiCOM.Button)(this.GetItem("btnNotify").Specific));
            this.btnNotify.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnNotify_ClickBefore);
            this.btnReject = ((SAPbouiCOM.Button)(this.GetItem("btnReject").Specific));
            this.btnReject.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnReject_ClickBefore);
            this.btnAuthor = ((SAPbouiCOM.Button)(this.GetItem("btnAuthor").Specific));
            this.btnAuthor.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnAuthor_ClickBefore);
            UGRS.Core.SDK.UI.UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(this.SBO_Application_ItemEvent);
            UGRS.Core.SDK.UI.UIApplication.GetApplication().MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(this.SBO_Application_MenuEvent);
            //      UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(this.SBO_Application_ItemEvent);
            this.cboStatus = ((SAPbouiCOM.ComboBox)(this.GetItem("cboStatus").Specific));
            this.lblCodeMov = ((SAPbouiCOM.StaticText)(this.GetItem("lblCodeMov").Specific));
            this.txtCodeMov = ((SAPbouiCOM.EditText)(this.GetItem("txtCodeMov").Specific));
            this.txtAreaF = ((SAPbouiCOM.EditText)(this.GetItem("txtAreaF").Specific));
            this.txtEmpF = ((SAPbouiCOM.EditText)(this.GetItem("txtEmpF").Specific));
            //          }
            //          catch (Exception ex)
            //          {
            //              LogService.WriteError("frmReceipts: " + ex.Message);
            //              LogService.WriteError(ex);
            //          }
            this.chkCopy = ((SAPbouiCOM.CheckBox)(this.GetItem("chk_Copy").Specific));
            this.btnInv = ((SAPbouiCOM.Button)(this.GetItem("btnInv").Specific));
            this.btnInv.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnInv_ClickBefore);
            this.btnPay = ((SAPbouiCOM.Button)(this.GetItem("btnPay").Specific));
            this.btnPay.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnPay_ClickBefore);
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

        /// <summary>
        /// Carga los controles
        /// <summary>
        private void OnCustomInitialize()
        {
            try
            {

                loadMenu();
                if (mEnumType == TypeEnum.Type.Refund)
                {
                    mStrType = "U_GLO_Refund";
                }
                else
                {
                    mStrType = "U_GLO_ExpeCheck";
                }
                LoadChoseFromList();
                SetChooseToTxt();
                CreateDataTableMatrix();
                LoadCombobox();

                chkCopy.ValOff = "N";
                chkCopy.ValOn = "Y";
                //CostCenterType();
                //loadMenu();
                lblCodeMov.Item.Visible = false;
                txtCodeMov.Item.Visible = false;

                txtEmploye.Item.Enabled = true;
                string lStrCostCenter = mObjPurchasesServiceFactory.GetPurchaseInvoiceService().GetCostCenter();

                SetPermission(mObjPurchasesServiceFactory.GetPurchasePermissionsService().GetPermissionType(lStrCostCenter, mStrType));
                EnableControls();

                if (string.IsNullOrEmpty(txtDate.Value))
                {
                    this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_Date").ValueEx = Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd");
                }

            }
            catch (Exception ex)
            {
                LogService.WriteError("OnCustomInitialize: " + ex.Message);
                LogService.WriteError(ex);

            }
        }

        private void CostCenterType()
        {
            try
            {
                string lStrCostCenter = mObjPurchasesServiceFactory.GetPurchaseInvoiceService().GetCostCenter();
                //PermissionsEnum.Permission lObjPermissionEnum = mObjPurchasesServiceFactory.GetPurchasePermissionsService().GetPermissionType(lStrCostCenter, "U_GLO_ExpeCheck");
                string lStrCCTypeCode = mObjPurchasesServiceFactory.GetPurchaseInvoiceService().CCTypeCode(lStrCostCenter);

                if (lStrCCTypeCode == "O")
                {
                    txtArea.Value = lStrCostCenter;
                    txtEmploye.Item.Click();
                    txtArea.Item.Enabled = false;
                }
                else
                {
                    txtArea.Item.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmSearchRefunds (OnCustomInitialize) " + ex.Message);
                LogService.WriteError(ex);
            }
        }
        #endregion

        #region ChooseFromLists

        /// <summary>
        /// Carga los ChooseFromList de Empleado y area
        /// <summary>
        private void LoadChoseFromList()
        {
            try
            {
                ChooseFromList lObjCFLArea = InitChooseFromLists(false, "61", "CFL_Area", this.UIAPIRawForm.ChooseFromLists);
                AddConditionChoseFromListArea(lObjCFLArea);

                mObjCFLEmployee = InitChooseFromLists(false, "171", "CFL_Employ", this.UIAPIRawForm.ChooseFromLists);
            }
            catch (Exception ex)
            {
                LogService.WriteError("LoadChoseFromList: " + ex.Message);
                LogService.WriteError(ex);
            }

        }

        /// <summary>
        /// Establece los txt a los ChooseFromList
        /// <summary>
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
                LogService.WriteError("LoadChoseFromList: " + ex.Message);
                LogService.WriteError(ex);
            }

        }

        /// <summary>
        /// inicia los datos a un ChooseFromList
        /// <summary>
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
                UIApplication.ShowError(string.Format("InitCustomerChooseFromListException: {0}", ex.Message));
                LogService.WriteError("InitChooseFromLists: " + ex.Message);
                LogService.WriteError(ex);

            }
            return lObjoCFL;
        }

        #region ConditionsChooseFromList
        /// <summary>
        /// Condiciones de chooseFromList Area
        /// <summary>
        private void AddConditionChoseFromListArea(ChooseFromList pCFL)
        {
            try
            {
                SAPbouiCOM.Condition lObjCon = null;
                SAPbouiCOM.Conditions lObjCons = new Conditions();
                pCFL.SetConditions(lObjCons);

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

                pCFL.SetConditions(lObjCons);
            }
            catch (Exception ex)
            {
                LogService.WriteError("AddConditionChoseFromListArea: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Condiciones de chooseFromList Empleado cargando una lista
        /// <summary>
        private void AddConditionChoseFromListEmployee(ChooseFromList pCFL, List<string> pLstEmployeeId)
        {
            SAPbouiCOM.Condition lObjCon = null;
            SAPbouiCOM.Conditions lObjCons = new Conditions();
            try
            {
                int i = 1;
                if (pLstEmployeeId.Count() > 0)
                {
                    foreach (string pStrEmployeeId in pLstEmployeeId)
                    {
                        lObjCon = lObjCons.Add();
                        lObjCon.Alias = "empID";
                        lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        lObjCon.CondVal = pStrEmployeeId;

                        if (pLstEmployeeId.Count() > i)
                        {
                            lObjCon.Relationship = BoConditionRelationship.cr_OR;
                        }
                        i++;
                    }
                }
                else
                {
                    lObjCon = lObjCons.Add();
                    lObjCon.Alias = "empID";
                    lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                    lObjCon.CondVal = "";
                }
                pCFL.SetConditions(lObjCons);
            }
            catch (Exception ex)
            {
                LogService.WriteError("AddConditionChoseFromListEmployee: " + ex.Message);
                LogService.WriteError(ex);
            }
        }


        #endregion

        #endregion

        #region Methods

        private void loadMenu()
        {
            this.UIAPIRawForm.EnableMenu("519", true);//Preview
            this.UIAPIRawForm.EnableMenu("520", false); // Print
            //this.UIAPIRawForm.EnableMenu("6659", false);  // Fax
            //this.UIAPIRawForm.EnableMenu("1281", true); // Search Record
            //this.UIAPIRawForm.EnableMenu("1282", true); // Add New Record
            this.UIAPIRawForm.EnableMenu("1288", true);  // Next Record
            this.UIAPIRawForm.EnableMenu("1289", true);  // Pevious Record
            this.UIAPIRawForm.EnableMenu("1290", true);  // First Record
            this.UIAPIRawForm.EnableMenu("1291", true);  // Last record
        }

        /// <summary>
        /// Carga el enum de estatus como combobox
        /// <summary>
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
                cboStatus.Select("1", BoSearchKey.psk_ByValue);
            }
            catch (Exception ex)
            {
                LogService.WriteError("LoadCombobox: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Obtiene el comprobante del formulario
        /// <summary>
        private Vouchers GetVoucherForm()
        {

            Vouchers lObjVoucher = new Vouchers();
            try
            {
                string lStrArea = txtArea.Value == "" ? txtAreaF.Value : txtArea.Value;

                mStrTypePrefix = String.IsNullOrEmpty(txtCodeMov.Value) ? "CG_" : "";



                // Si es un comprobante nuevo o uno ya registrado
                if (string.IsNullOrEmpty(mStrRowCode))
                {
                    int lIntFolio = (Convert.ToInt32(mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetVoucherFolio(lStrArea, ((int)mEnumType).ToString())));
                    string lStrFolio = (String.IsNullOrEmpty(txtCodeMov.Value)) ?
                    String.Format("{0}{1}_{2}", mStrTypePrefix, txtArea.Value, lIntFolio + 1) :
                    String.Format(mStrTypePrefix + txtCodeMov.Value);
                    lObjVoucher.Folio = lStrFolio;
                    lObjVoucher.UserCode = UIApplication.GetCompany().UserName;
                }
                else
                {
                    lObjVoucher.RowCode = mStrRowCode;
                    lObjVoucher.Folio = txtFolio.Value;
                }
                lObjVoucher.TypeVoucher = (int)mEnumType;
                lObjVoucher.Area = lStrArea;
                lObjVoucher.Coments = txtComents.Value;
                lObjVoucher.Date = Convert.ToDateTime(this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_Date").Value);// DateTime.ParseExact(txtDate.Value, "yyyyMMdd", CultureInfo.InvariantCulture);
                lObjVoucher.Employee = mStrEmployeId;
                lObjVoucher.UserCode = mStrUserCode;
                lObjVoucher.Total = string.IsNullOrEmpty(txtTotal.Value) ? 0 : Convert.ToDouble(txtTotal.Value);
                lObjVoucher.Status = Convert.ToInt16(cboStatus.Value);
                lObjVoucher.LstVouchersDetail = GetVouchersDetail();

                // Si es de tipo viaticos
                if (!string.IsNullOrEmpty(txtCodeMov.Value))
                {
                    lObjVoucher.CodeMov = txtCodeMov.Value;
                    lObjVoucher.Area = txtAreaF.Value;
                    //EnableControls();
                }
                else
                {
                    lObjVoucher.CodeMov = string.Empty;
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("LoadCombobox: " + ex.Message);
                LogService.WriteError(ex);
            }
            return lObjVoucher;
        }

        /// <summary>
        /// Obtiene los datos del datatable
        /// <summary>
        private List<VouchersDetail> GetVouchersDetail()
        {
            List<VouchersDetail> lLstVoucherDetail = new List<VouchersDetail>();
            try
            {
                for (int i = 0; i < DtMatrix.Rows.Count; i++)
                {
                    VouchersDetail lObjVoucherDetail = new VouchersDetail()
                    {
                        NA = DtMatrix.GetValue("C_NA", i).ToString(),
                        RowCode = DtMatrix.GetValue("C_RowCode", i).ToString(),
                        Date = Convert.ToDateTime(DtMatrix.GetValue("C_Date", i).ToString()),
                        Type = DtMatrix.GetValue("C_Type", i).ToString(),
                        DocNum = DtMatrix.GetValue("C_DocNum", i).ToString(),
                        DocEntry = DtMatrix.GetValue("C_DocEntrF", i).ToString() == "" ? DtMatrix.GetValue("C_DocEntrJ", i).ToString()
                                                            : DtMatrix.GetValue("C_DocEntrF", i).ToString(),
                        Provider = DtMatrix.GetValue("C_Provider", i).ToString(),
                        Status = DtMatrix.GetValue("C_Status", i).ToString(),
                        Coments = DtMatrix.GetValue("C_Coments", i).ToString(),
                        Subtotal = Convert.ToDouble(DtMatrix.GetValue("C_Subtotal", i).ToString()),
                        IVA = Convert.ToDouble(DtMatrix.GetValue("C_Iva", i).ToString()),
                        ISR = Convert.ToDouble(DtMatrix.GetValue("C_ISR", i).ToString()),
                        IEPS = Convert.ToDouble(DtMatrix.GetValue("C_IEPS", i)),
                        Total = Convert.ToDouble(DtMatrix.GetValue("C_Total", i)),
                        Coment = DtMatrix.GetValue("C_Coment", i).ToString(),
                        UserCode = DtMatrix.GetValue("C_User", i).ToString()
                    };
                    lLstVoucherDetail.Add(lObjVoucherDetail);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("GetVouchersDetail: " + ex.Message);
                LogService.WriteError(ex);
            }
            return lLstVoucherDetail;
        }

        /// <summary>
        /// Habilita los controles de Area y empleado
        /// Cambia los estatus de visibilidad
        /// <summary>
        private void EnableControls()
        {
            try
            {

                txtAreaF.Item.Visible = false;
                txtEmpF.Item.Visible = false;

                txtArea.Item.Visible = true;
                txtEmploye.Item.Visible = true;
                mtxReceipt.Columns.Item("C_Coment").Editable = false;
            }
            catch (Exception ex)
            {
                LogService.WriteError("GetVouchersDetail: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Inhabilita los controles de Area y empleado
        /// Cambia los estatus de visibilidad
        /// <summary>
        private void DisableControls()
        {
            try
            {


                txtAreaF.Item.Visible = true;
                txtEmpF.Item.Visible = true;

                txtArea.Item.Visible = false;
                txtEmploye.Item.Visible = false;
                mtxReceipt.Columns.Item("C_Coment").Editable = true;
            }
            catch (Exception ex)
            {
                LogService.WriteError("GetVouchersDetail: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Crea las columnas que tendra el DataTable
        /// <summary>
        private void CreateDataTableMatrix()
        {
            try
            {
                this.UIAPIRawForm.DataSources.DataTables.Add("VoucherDataTable");
                DtMatrix = this.UIAPIRawForm.DataSources.DataTables.Item("VoucherDataTable");
                DtMatrix.Columns.Add("#", SAPbouiCOM.BoFieldsType.ft_ShortNumber);
                DtMatrix.Columns.Add("C_NA", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_RowCode", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Date", SAPbouiCOM.BoFieldsType.ft_Date);
                DtMatrix.Columns.Add("C_Type", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_DocEntrF", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_DocEntrJ", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_DocNum", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_DocNumF", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Provider", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Status", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Coments", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Subtotal", SAPbouiCOM.BoFieldsType.ft_Price);
                DtMatrix.Columns.Add("C_Iva", SAPbouiCOM.BoFieldsType.ft_Price);
                DtMatrix.Columns.Add("C_ISR", SAPbouiCOM.BoFieldsType.ft_Price);
                DtMatrix.Columns.Add("C_IEPS", SAPbouiCOM.BoFieldsType.ft_Price);
                DtMatrix.Columns.Add("C_Total", SAPbouiCOM.BoFieldsType.ft_Price);
                DtMatrix.Columns.Add("C_Coment", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_User", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_VoucherCode", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_DocFolio", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            }
            catch (Exception ex)
            {
                LogService.WriteError("CreateDataTableMatrix: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Llena el Datatable y llena la matrix con los datos
        /// <summary>
        private void FillMatrixQuitar()
        {
            try
            {

                DtMatrix.Rows.Clear();


                bool lBolUpdateStatus = false;
                List<VouchersDetail> lLstVouchersDetail = mObjPurchasesServiceFactory.GetPurchaseVouchersService().GetVouchesDetail(mStrRowCode);
                int i = 0;
                foreach (VouchersDetail lObjVoucherDetail in lLstVouchersDetail)
                {

                    lBolUpdateStatus = mObjPurchasesServiceFactory.GetPurchaseInvoiceService().UpdateStatus(lObjVoucherDetail);
                    DtMatrix.Rows.Add();
                    DtMatrix.SetValue("#", i, i + 1);
                    if (string.IsNullOrEmpty(lObjVoucherDetail.NA) || lObjVoucherDetail.NA == "N")
                    {
                        DtMatrix.SetValue("C_NA", i, "N");
                    }
                    else
                    {
                        DtMatrix.SetValue("C_NA", i, "Y");
                    }
                    DtMatrix.SetValue("C_RowCode", i, lObjVoucherDetail.RowCode);
                    DtMatrix.SetValue("C_Date", i, lObjVoucherDetail.Date);
                    DtMatrix.SetValue("C_Type", i, lObjVoucherDetail.Type);
                    DtMatrix.SetValue("C_DocNum", i, lObjVoucherDetail.DocNum);

                    if (lObjVoucherDetail.Type == "Nota")
                    {
                        DtMatrix.SetValue("C_DocEntrJ", i, lObjVoucherDetail.DocEntry);
                    }
                    else
                    {
                        DtMatrix.SetValue("C_DocEntrF", i, lObjVoucherDetail.DocEntry);
                        DtMatrix.SetValue("C_DocNumF", i, lObjVoucherDetail.DocNum);
                    }

                    DtMatrix.SetValue("C_Provider", i, lObjVoucherDetail.Provider);
                    DtMatrix.SetValue("C_Status", i, lObjVoucherDetail.Status);
                    DtMatrix.SetValue("C_Coments", i, lObjVoucherDetail.Coments);
                    DtMatrix.SetValue("C_Subtotal", i, lObjVoucherDetail.Subtotal);
                    DtMatrix.SetValue("C_Iva", i, lObjVoucherDetail.IVA);
                    DtMatrix.SetValue("C_ISR", i, lObjVoucherDetail.ISR);
                    DtMatrix.SetValue("C_IEPS", i, lObjVoucherDetail.IEPS);
                    DtMatrix.SetValue("C_Total", i, lObjVoucherDetail.Total);
                    DtMatrix.SetValue("C_Coment", i, lObjVoucherDetail.Coment);
                    DtMatrix.SetValue("C_User", i, lObjVoucherDetail.UserCode);
                    DtMatrix.SetValue("C_VoucherCode", i, lObjVoucherDetail.CodeVoucher);
                    i++;
                }

                if (lBolUpdateStatus)
                {
                    FillMatrixInvoice();
                }

                mtxReceipt.Columns.Item("#").DataBind.Bind("VoucherDataTable", "#");
                mtxReceipt.Columns.Item("C_NA").DataBind.Bind("VoucherDataTable", "C_NA");
                mtxReceipt.Columns.Item("C_Date").DataBind.Bind("VoucherDataTable", "C_Date");
                mtxReceipt.Columns.Item("C_Type").DataBind.Bind("VoucherDataTable", "C_Type");
                mtxReceipt.Columns.Item("C_DocEntrF").DataBind.Bind("VoucherDataTable", "C_DocNumF");//C_DocEntrF
                mtxReceipt.Columns.Item("C_DocEntrJ").DataBind.Bind("VoucherDataTable", "C_DocEntrJ");
                mtxReceipt.Columns.Item("C_Provider").DataBind.Bind("VoucherDataTable", "C_Provider");
                mtxReceipt.Columns.Item("C_Status").DataBind.Bind("VoucherDataTable", "C_Status");
                mtxReceipt.Columns.Item("C_Coments").DataBind.Bind("VoucherDataTable", "C_Coments");
                mtxReceipt.Columns.Item("C_Subtotal").DataBind.Bind("VoucherDataTable", "C_Subtotal");
                mtxReceipt.Columns.Item("C_Iva").DataBind.Bind("VoucherDataTable", "C_Iva");
                mtxReceipt.Columns.Item("C_ISR").DataBind.Bind("VoucherDataTable", "C_ISR");
                mtxReceipt.Columns.Item("C_IEPS").DataBind.Bind("VoucherDataTable", "C_IEPS");
                mtxReceipt.Columns.Item("C_Total").DataBind.Bind("VoucherDataTable", "C_Total");
                mtxReceipt.Columns.Item("C_Coment").DataBind.Bind("VoucherDataTable", "C_Coment");


                //LinkedButton
                SAPbouiCOM.LinkedButton lObjLinkInvoice = (SAPbouiCOM.LinkedButton)mtxReceipt.Columns.Item("C_DocEntrF").ExtendedObject;
                //lObjLinkInvoice.LinkedObject = SAPbouiCOM.BoLinkedObject.lf_PurchaseInvoice;

                SAPbouiCOM.LinkedButton lObjLinkJournal = (SAPbouiCOM.LinkedButton)mtxReceipt.Columns.Item("C_DocEntrJ").ExtendedObject;
                //lObjLinkJournal.LinkedObject = SAPbouiCOM.BoLinkedObject.lf_JournalPosting;

                //oLink.LinkedObject = SAPbouiCOM.BoLinkedObject.lf_JournalPosting;

                this.UIAPIRawForm.Freeze(true);
                mIntRowSelected = 0;
                mtxReceipt.LoadFromDataSource();
                mtxReceipt.AutoResizeColumns();
                this.UIAPIRawForm.Freeze(false);
                LogService.WriteSuccess("FillMatrix carga correcta de datos Code:" + mStrRowCode + ", Cantidad: " + lLstVouchersDetail.Count);

            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("FillMatrix: " + ex.Message);
                LogService.WriteError(ex);
                //throw;
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }




        /// <summary>
        /// Carga los datos en el formulario desde un RowCode
        /// <summary>
        private void LoadVoucher(string pStrRowCode)
        {
            try
            {

                List<Vouchers> lObjVouchers = mObjPurchasesServiceFactory.GetPurchaseVouchersService().GetVouches(pStrRowCode);
                if (lObjVouchers.Count() > 0)
                {
                    txtFolio.Value = lObjVouchers[0].Folio;

                    txtComents.Value = lObjVouchers[0].Coments;
                    txtTotal.Value = lObjVouchers[0].Total.ToString(); // lObjVouchers[0].Total.ToString();
                    cboStatus.Select(lObjVouchers[0].Status.ToString(), BoSearchKey.psk_ByValue);
                    this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_Date").ValueEx = Convert.ToDateTime(lObjVouchers[0].Date).ToString("yyyyMMdd");
                    this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Area").ValueEx = lObjVouchers[0].Area;
                    this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Employ").ValueEx = lObjVouchers[0].Employee;
                    mStrEmployeId = lObjVouchers[0].Employee;
                    mStrUserCode = lObjVouchers[0].UserCode;
                    mStrRowCode = pStrRowCode;
                    txtAreaF.Value = lObjVouchers[0].Area;
                    if (txtEmploye != null)
                    {
                        txtEmpF.Value = mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetEmployeName(txtEmploye.Value); //Error
                    }

                    if (!string.IsNullOrEmpty(lObjVouchers[0].CodeMov))
                    {
                        lblCodeMov.Item.Visible = true;
                        txtCodeMov.Item.Visible = true;
                        txtCodeMov.Value = lObjVouchers[0].CodeMov;

                        txtComents.Item.Click();
                        DisableControls();
                    }
                    else if (!string.IsNullOrEmpty(mStrRowCode))
                    {
                        DisableControls();
                    }
                    else
                    {
                        EnableControls();
                    }
                    string lStrCostCenter = mObjPurchasesServiceFactory.GetPurchaseInvoiceService().GetCostCenter();
                    SetPermission(mObjPurchasesServiceFactory.GetPurchasePermissionsService().GetPermissionType(lStrCostCenter, mStrType));
                    FillMatrixInvoice();
                    LogService.WriteSuccess("Carga correcta de comprobante Code:" + pStrRowCode);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("LoadVoucher: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Actualiza el estatus del comprobante
        /// <summary>
        private void UpdateStatus(StatusEnum pObjStatus)
        {
            try
            {
                Vouchers lObjVoucher = new Vouchers();
                lObjVoucher = GetVoucherForm();

                lObjVoucher.Status = (int)pObjStatus;
                if (mObjPurchasesServiceFactory.GetVouchersService().Update(lObjVoucher) != 0)
                {
                    string lStrerror = DIApplication.Company.GetLastErrorDescription();
                    UIApplication.ShowMessageBox(lStrerror);
                }
                else
                {
                    cboStatus.Select(((int)pObjStatus).ToString(), BoSearchKey.psk_ByValue);
                    UIApplication.ShowMessageBox("Cambio de estatus a: \"" + pObjStatus.GetDescription() + "\" realizado correctamente");
                    LogService.WriteSuccess("UpdateStatus: RowCode: " + lObjVoucher.RowCode + " Cambio de estatus a: \"" + pObjStatus.GetDescription() + "\" realizado correctamente");
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("UpdateStatus: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Visibilidad de los botones dependiendo de los permisos
        /// <summary>
        private void SetPermission(PermissionsEnum.Permission pObjPermission)
        {
            try
            {
                //Amartnez
                // pObjPermission = PermissionsEnum.Permission.Purchase;
                switch (pObjPermission)
                {
                    //Si no cuenta con permisos se coloca por defecto false/true
                    case PermissionsEnum.Permission.None:
                        bool lBolVisible = false;
                        btnAuthor.Item.Visible = lBolVisible;
                        btnNotify.Item.Visible = lBolVisible;
                        btnReject.Item.Visible = lBolVisible;
                        btnSave.Item.Visible = lBolVisible;
                        btnCFDI.Item.Visible = lBolVisible;
                        btnNotes.Item.Visible = lBolVisible;
                        btnCancel.Item.Visible = lBolVisible;
                        break;

                    case PermissionsEnum.Permission.Purchase:
                        btnAuthor.Item.Visible = false;
                        btnNotify.Item.Visible = true;
                        btnReject.Item.Visible = false;
                        btnSave.Item.Visible = true;
                        StatusEnum lObjStatusEnum = (StatusEnum)Convert.ToInt16(cboStatus.Value);
                        if (lObjStatusEnum == StatusEnum.Pending)
                        {
                            btnCFDI.Item.Visible = true;
                            btnNotes.Item.Visible = true;
                            btnCancel.Item.Visible = true;
                        }
                        else
                        {
                            btnCFDI.Item.Visible = false;
                            btnNotes.Item.Visible = false;
                            btnCancel.Item.Visible = false;
                        }
                        break;

                    case PermissionsEnum.Permission.AuthorizePurchase:
                        btnAuthor.Item.Visible = true;
                        btnNotify.Item.Visible = false;
                        btnReject.Item.Visible = true;
                        btnSave.Item.Visible = true;
                        btnCFDI.Item.Visible = false;
                        btnNotes.Item.Visible = false;
                        btnCancel.Item.Visible = false;
                        break;

                    case PermissionsEnum.Permission.AuthorizeOperations:
                        btnAuthor.Item.Visible = true;
                        btnNotify.Item.Visible = false;
                        btnReject.Item.Visible = true;
                        btnSave.Item.Visible = true;
                        btnCFDI.Item.Visible = false;
                        btnNotes.Item.Visible = false;
                        btnCancel.Item.Visible = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("SetPermission: " + ex.Message);
                LogService.WriteError(ex);
            }
        }


        /// <summary>
        /// Guarda el comprobante
        /// <summary>
        private bool SaveVoucher()
        {
            string lStrFolio;
            string lStrArea;
            bool lBolResult = false;
            Vouchers lObjVouchers = GetVoucherForm();

            if (mObjPurchasesServiceFactory.GetVouchersService().Add(lObjVouchers) != 0)
            {
                string lStrerror = DIApplication.Company.GetLastErrorDescription();
                UIApplication.ShowMessageBox(lStrerror);
                LogService.WriteError("SaveVoucher: " + lStrerror);
            }
            else
            {
                lStrFolio = lObjVouchers.Folio;
                lStrArea = lObjVouchers.Area;
                mStrRowCode = mObjPurchasesServiceFactory.GetPurchasePaymentService().GetVoucherCode(lStrFolio, lStrArea, (int)mEnumType);//ojo
                txtFolio.Value = lStrFolio;
                lBolResult = true;
                LogService.WriteSuccess("SaveVoucher: Guardado correcto RowCode:" + mStrRowCode);
            }
            return lBolResult;
        }

        /// <summary>
        /// Limpia los controles de la pantalla
        /// <summary>
        private void ClearControls()
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);
                this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Employ").ValueEx = string.Empty;
                this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Area").ValueEx = string.Empty;
                this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_Date").ValueEx = Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd");
                txtFolio.Value = string.Empty;
                txtArea.Value = string.Empty;
                txtEmploye.Value = string.Empty;
                txtComents.Value = string.Empty;
                txtTotal.Value = string.Empty;
                txtCodeMov.Value = string.Empty;
                lblCodeMov.Item.Visible = false;
                txtCodeMov.Item.Visible = false;
                mStrEmployeId = string.Empty;
                mStrRowCode = string.Empty;
                cboStatus.Select(1, BoSearchKey.psk_Index);
                //mStrUserCode = string.Empty;

                EnableControls();
                DtMatrix.Rows.Clear();
                mIntRowSelected = 0;
                mtxReceipt.LoadFromDataSource();
                this.UIAPIRawForm.Freeze(false);
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                LogService.WriteError("SetPermission: " + ex.Message);
                LogService.WriteError(ex);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }

        }

        private string GetAccountRefound(string pStrArea)
        {
            // Si es de tipo viaticos
            string lStrAccountRefound;
            if (txtCodeMov != null && string.IsNullOrEmpty(txtCodeMov.Value))//Error
            {
                lStrAccountRefound = mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetAccountRefund(pStrArea);
            }
            else
            {
                lStrAccountRefound = mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetAccountInConfig("MQ_DEUDORESVIAT");
            }
            return lStrAccountRefound;
        }

        private string GetAccountDU()
        {
            // Si es de tipo viaticos
            string lStrAcountDU = string.Empty;
            try
            {

                if (string.IsNullOrEmpty(txtCodeMov.Value))
                {
                    lStrAcountDU = mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetAccountInConfig("GLO_DEU_REEM");
                }
                else
                {
                    lStrAcountDU = "";
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("SetPermission: " + ex.Message);
                LogService.WriteError(ex);
            }
            return lStrAcountDU;
        }

        #endregion

        #region Events

        private void SBO_Application_MenuEvent(ref MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            //int lIntRowCode;
            try
            {
                string ss = pVal.MenuUID;
                /*  if(!pVal.BeforeAction && UIApplication.GetApplication().Forms.ActiveForm.UniqueID == this.UIAPIRawForm.UniqueID) {
                      switch(pVal.MenuUID) {
                          case "1288": // Next Record.
                          if(string.IsNullOrEmpty(mStrRowCode)) {
                              lIntRowCode = mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetLastReceipt();
                          }
                          else {
                              lIntRowCode = Convert.ToInt32(mStrRowCode) + 1;
                          }


                          if(lIntRowCode > (mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetLastReceipt())) {
                              UIApplication.ShowWarning(string.Format("Primer registro de datos"));
                              LoadVoucher("1");
                          }
                          else {
                              LoadVoucher(lIntRowCode.ToString());
                          }
                          break;

                          case "1289": // Pevious Record
                          lIntRowCode = Convert.ToInt32(mStrRowCode) - 1;
                          if(lIntRowCode < 1) {
                              LoadVoucher((mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetLastReceipt()).ToString());
                              UIApplication.ShowWarning(string.Format("Ultimo registro de datos"));
                          }
                          else {
                              LoadVoucher(lIntRowCode.ToString());
                          }
                          break;

                          case "1290": // First Record
                          LoadVoucher("1");

                          //SearchTicketEnter(lIntFolio.ToString());
                          break;

                          case "1291": // Last record 
                          LoadVoucher((mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetLastReceipt()).ToString());
                          break;
                      }
                  }*/

                switch (pVal.MenuUID)
                {
                    case "519":
                        if (!pVal.BeforeAction && UIApplication.GetApplication().Forms.ActiveForm.UniqueID == this.UIAPIRawForm.UniqueID)
                        {
                            printUDO(txtArea.Value, txtFolio.Value);
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                LogService.WriteError("SetPermission: " + ex.Message);
                LogService.WriteError(ex);
            }

        }

        /// <summary>
        /// Eventos de chooseFromList de Empleado y Area
        /// <summary>
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
                        this.UIAPIRawForm.DataSources.UserDataSources.Item(lObjDataTable.UniqueID).ValueEx = System.Convert.ToString(lObjDataTable.GetValue(0, 0));

                        switch (lObjDataTable.UniqueID)
                        {
                            case "CFL_Area":
                                string lStrArea = lObjDataTable.GetValue(0, 0).ToString();
                                this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Area").ValueEx = lStrArea;
                                this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Employ").ValueEx = "";
                                string lStrDepartment = mObjPurchasesServiceFactory.GetPurchaseService().GetDepartment(lStrArea);
                                AddConditionChoseFromListEmployee(mObjCFLEmployee, mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetEmployeList(lStrDepartment).ToList());
                                int lIntFolio = (Convert.ToInt32(mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetVoucherFolio(txtArea.Value, ((int)mEnumType).ToString())));
                                txtFolio.Value = (lIntFolio + 1).ToString();
                                FillMatrixInvoice();
                                break;

                            case "CFL_Employ":
                                this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Employ").ValueEx = lObjDataTable.GetValue(2, 0).ToString() + " " + lObjDataTable.GetValue(3, 0).ToString() + lObjDataTable.GetValue(1, 0).ToString();
                                mStrEmployeId = lObjDataTable.GetValue(0, 0).ToString();
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("ChooseFromListAfterEvent: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        ///Eventos de aplicación
        /// <summary>
        private void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (pVal.EventType != BoEventTypes.et_FORM_ACTIVATE && pVal.EventType != BoEventTypes.et_LOST_FOCUS && pVal.EventType != BoEventTypes.et_GOT_FOCUS)
                {
                    if (FormUID.Equals(this.UIAPIRawForm.UniqueID))
                    {


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
                //// Si se cierra la ventana de xml o notas
                //// if (FormUID.Equals("frmXML") || FormUID.Equals("frmNotes")) ((SAPbouiCOM.ItemEventClass)(pVal)).FormTypeEx "UGRS.AddOn.Purchases.Forms.frmPurchaseXML"
                //if (pVal.FormTypeEx.Equals("UGRS.AddOn.Purchases.Forms.frmPurchaseXML") || pVal.FormTypeEx.Equals("UGRS.AddOn.Purchases.Forms.frmPurchaseNotes"))
                //{
                //    if (!pVal.BeforeAction)
                //    {
                //        switch (pVal.EventType)
                //        {
                //            case BoEventTypes.et_FORM_CLOSE:
                //                mBolFlagClose = true;
                //                break;
                //        }
                //    }
                //}
                //if (mBolFlagClose && pVal.FormTypeEx.Equals("UGRS.AddOn.Purchases.Forms.frmReceipts") && pVal.BeforeAction && pVal.EventType == BoEventTypes.et_FORM_ACTIVATE )
                //{
                //    
                //    FillMatrix();
                //    mBolFlagClose = false;
                //}



            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("ItemEventException: {0}", ex.Message));
                LogService.WriteError("SBO_Application_ItemEvent: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Evento de guardar un comprobante
        /// <summary>
        private void btnSave_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                Vouchers lObjVouchers = GetVoucherForm();
                if (lObjVouchers.LstVouchersDetail.Count > 0)
                {
                    if (string.IsNullOrEmpty(mStrRowCode))
                    {
                        mStrUserCode = UIApplication.GetCompany().UserName;
                        lObjVouchers.UserCode = mStrUserCode;
                        if (mObjPurchasesServiceFactory.GetVouchersService().Add(lObjVouchers) != 0)
                        {
                            string lStrerror = DIApplication.Company.GetLastErrorDescription();
                            UIApplication.ShowMessageBox(lStrerror);
                            LogService.WriteError("btnSave_ClickBefore: " + lStrerror);
                        }
                        else
                        {
                            ClearControls();
                            UIApplication.ShowMessageBox("Guardado correcto, Folio:" + lObjVouchers.Folio);
                            LogService.WriteSuccess("Guardado correcto RowCode:" + lObjVouchers.RowCode);
                        }
                    }
                    else
                    {
                        if (mObjPurchasesServiceFactory.GetVouchersService().Update(lObjVouchers) != 0)
                        {
                            string lStrerror = DIApplication.Company.GetLastErrorDescription();
                            UIApplication.ShowMessageBox(lStrerror);
                            LogService.WriteError("btnSave_ClickBefore: " + lStrerror + " RowCode:" + lObjVouchers.RowCode);
                        }
                        else
                        {
                            ClearControls();
                            UIApplication.ShowMessageBox("Actualizacion del folio: " + lObjVouchers.Folio + " guardado correctamente");
                            LogService.WriteSuccess("Actualizacion del RowCode: " + lObjVouchers.RowCode + " guardado correctamente");
                        }
                    }
                }
                else
                {
                    UIApplication.ShowMessageBox("El registro no cuenta con lineas");
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("btnSave_ClickBefore: " + ex.Message);
                LogService.WriteError(ex);
            }

        }

        private void btnCFDI_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                string lStrArea = !string.IsNullOrEmpty(txtArea.Value) ? txtArea.Value : txtAreaF.Value;
                Vouchers lObjVouchers = GetVoucherForm();

                if (ValidateSaveVoucher(lStrArea, lObjVouchers))
                {
                    DateTime lDtmDate = Convert.ToDateTime(this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_Date").Value);
                    frmPurchaseXML lObjfrmPurchaseXML = new frmPurchaseXML(mStrRowCode, lStrArea, mStrEmployeId, txtFolio.Value, GetAccountRefound(lStrArea),
                                                                        lObjVouchers, chkCopy.Checked, lDtmDate);
                    lObjfrmPurchaseXML.UIAPIRawForm.Left = 500;
                    lObjfrmPurchaseXML.UIAPIRawForm.Top = 10;
                    lObjfrmPurchaseXML.Show();
                    lObjfrmPurchaseXML.CloseAfter += CloseAfterEvent;
                }

            }
            catch (Exception ex)
            {
                LogService.WriteError("btnCFDI_ClickBefore: " + ex.Message);
                LogService.WriteError(ex);
            }

        }

        private bool ValidateSaveVoucher(string pStrArea, Vouchers pObjVouchers)
        {
            bool lBolResult = false;
            if (!string.IsNullOrEmpty(pStrArea))
            {
                if (!string.IsNullOrEmpty(mStrEmployeId))
                {

                    if (string.IsNullOrEmpty(mStrRowCode))
                    {
                        mStrUserCode = UIApplication.GetCompany().UserName;
                        if (mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetVoucherEmp(pObjVouchers))
                        {
                            lBolResult = SaveVoucher();
                        }
                        else
                        {
                            UIApplication.ShowMessageBox("Existen comprobantes pendientes para esta area y empleado");
                        }
                    }
                    else
                    {
                        if (mObjPurchasesServiceFactory.GetVouchersService().Update(pObjVouchers) != 0)
                        {
                            string lStrerror = DIApplication.Company.GetLastErrorDescription();
                            UIApplication.ShowMessageBox(lStrerror);
                            LogService.WriteError("btnSave_ClickBefore: " + lStrerror + " RowCode:" + pObjVouchers.RowCode);
                            lBolResult = false;
                        }
                        else
                        {
                            lBolResult = true;
                        }
                    }
                }
                else
                {
                    UIApplication.ShowMessageBox("Favor de seleccionar el empleado");
                }
            }
            else
            {
                UIApplication.ShowMessageBox("Favor de seleccionar el área");
            }
            return lBolResult;
        }

        /// <summary>
        /// Evento que se se ejecuta cuando se cierra una ventana de notas o CFDI
        /// <summary>
        private void CloseAfterEvent(SBOItemEventArg pVal)
        {
            try
            {
                if (string.IsNullOrEmpty(txtFolio.Value))
                {
                    int lIntFolio = (Convert.ToInt32(mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetVoucherFolio(txtArea.Value, ((int)mEnumType).ToString())));
                    txtFolio.Value = lIntFolio.ToString();
                }

                mStrRowCode = mObjPurchasesServiceFactory.GetPurchasePaymentService().GetVoucherCode(txtFolio.Value, txtArea.Value, (int)mEnumType);
                //LoadVoucher(mStrRowCode);


            }
            catch (Exception ex)
            {
                LogService.WriteError("CloseAfterEvent: " + ex.Message);
                LogService.WriteError(ex);
            }

        }

        /// <summary>
        /// Evento de abrir la ventana de notas
        /// <summary>
        private void btnNotes_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                string lStrArea = !string.IsNullOrEmpty(txtArea.Value) ? txtArea.Value : txtAreaF.Value;
                Vouchers lObjVouchers = GetVoucherForm();
                if (ValidateSaveVoucher(lStrArea, lObjVouchers))
                {
                    bool lBolIsCheeckingCost;
                    TypeEnum.Type lNoteType;
                    // Si es de tipo viaticos
                    if (!string.IsNullOrEmpty(txtCodeMov.Value))
                    {
                        lBolIsCheeckingCost = true;
                        lNoteType = TypeEnum.Type.Voucher;
                        //EnableControls();
                    }
                    else
                    {
                        lBolIsCheeckingCost = false;
                        lNoteType = TypeEnum.Type.Refund;
                    }
                    lObjVouchers.RowCode = mStrRowCode;
                    frmPurchaseNotes lObjfrmPurchaseNotes = new frmPurchaseNotes(lObjVouchers, GetAccountRefound(lStrArea), lNoteType, GetAccountDU(), lBolIsCheeckingCost, txtEmpF.Value, chkCopy.Checked);
                    lObjfrmPurchaseNotes.UIAPIRawForm.Left = 500;
                    lObjfrmPurchaseNotes.UIAPIRawForm.Top = 10;
                    lObjfrmPurchaseNotes.Show();
                    lObjfrmPurchaseNotes.CloseAfter += CloseAfterEvent;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("btnNotes_ClickBefore: " + ex.Message);
                LogService.WriteError(ex);
            }
        }


        /// <summary>
        /// Cancelar un documento seleccionado
        /// <summary>
        private void btnCancel_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (mIntRowSelected > 0)
                {
                    if (SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("¿Desea cancelar el item seleccionado?", 2, "Si", "No", "") == 1)
                    {
                        string lStrType = DtMatrix.GetValue("C_Type", mIntRowSelected - 1).ToString();

                        string lStrVoucherCode = DtMatrix.GetValue("C_VoucherCode", mIntRowSelected - 1).ToString();
                        if (lStrType == "XML")
                        {
                            string lStrDocEntry = DtMatrix.GetValue("C_DocEntrF", mIntRowSelected - 1).ToString();
                            PaymentDI lObjPaymentDI = new PaymentDI();
                            if (lObjPaymentDI.CancelPayment(lStrDocEntry))
                            {
                                InvoiceDI lObjInvoiceDI = new InvoiceDI();
                                if (lObjInvoiceDI.CancelDocument(lStrDocEntry))
                                {
                                    lObjInvoiceDI.UpdateStatus(mStrRowCode, lStrDocEntry, lStrVoucherCode);
                                    UIApplication.ShowMessageBox("Documento " + lStrDocEntry + " cancelado correctamente");
                                    LogService.WriteSuccess("Documento " + lStrDocEntry + " cancelado correctamente");
                                    //FillMatrixInvoice();
                                    LoadVoucher(mStrRowCode);

                                }
                                // UIApplication.ShowMessageBox("Pago " + lStrDocEntry + " cancelado correctamente");
                                LogService.WriteSuccess("Pago " + lStrDocEntry + " cancelado correctamente");
                            }
                        }
                        if (lStrType == "Nota")
                        {
                            string lStrDocEntry = DtMatrix.GetValue("C_DocEntrJ", mIntRowSelected - 1).ToString();
                            PolicyDI lObjPolicytDI = new PolicyDI();

                            if (lObjPolicytDI.CancelJournalEntry(lStrDocEntry, lStrVoucherCode))
                            {
                                LoadVoucher(mStrRowCode);
                                UIApplication.ShowMessageBox("Asiento " + lStrDocEntry + " cancelado correctamente");
                                LogService.WriteSuccess("Asiento " + lStrDocEntry + " cancelado correctamente");
                            }
                        }
                    }
                }
                else
                {
                    UIApplication.ShowMessageBox("Favor de seleccionar un documento a cancelar");
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox("Error al cancelar el documento ");
                LogService.WriteError("btnCancel_ClickBefore: " + ex.Message);
                LogService.WriteError(ex);
            }
        }


        /// <summary>
        /// Funcionalidad de autorizar un documento
        /// <summary>
        private void btnAuthor_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                CreateAlert lObjCreateAlert = new CreateAlert();
                StatusEnum lObjStatusEnum = (StatusEnum)Convert.ToInt16(cboStatus.Value);
                Vouchers lObjVoucher = new Vouchers();
                lObjVoucher = GetVoucherForm();

                string lStrMessageType = "";

                MessageDTO lObjMessageDTO = new MessageDTO();
                lObjMessageDTO.UserCode = lObjVoucher.UserCode;
                //GetUserPermission
                PermissionsEnum.Permission lObjPermissionEnum = mObjPurchasesServiceFactory.GetPurchasePermissionsService().GetPermissionType(lObjVoucher.Area, mStrType);

                ////Verifiacion si el permiso y el estatus es correcto para autorizar
                //if (lObjStatusEnum == StatusEnum.PendingArea && lObjPermissionEnum == PermissionsEnum.Permission.AuthorizePurchase)
                //{
                //    UpdateStatus(StatusEnum.AuthorizedArea);
                //    lObjMessageDTO.Message = "Estatus del comprobante " + txtFolio.Value + " ha cambiado a: \"" + StatusEnum.AuthorizedArea.GetDescription() + "\"";
                //    LogService.WriteSuccess("btnAuthor_ClickBefore: Estatus del comprobante " + txtFolio.Value + " ha cambiado a: \"" + StatusEnum.AuthorizedArea.GetDescription() + "\"");
                //    lObjMessageDTO.UserId = mObjPurchasesDAO.GetUserId(lObjMessageDTO.UserCode);
                //    if (lObjCreateAlert.SaveAlert(lObjMessageDTO))
                //    {
                //        LogService.WriteSuccess("btnAuthor_ClickBefore: Alerta enviada correctamente usuario: " + lObjVoucher.UserCode + ", Folio:" + lObjVoucher.RowCode);
                //    }
                //    lStrMessageType = "CO_COMP_3";
                //}
                //else 
                if (lObjStatusEnum == StatusEnum.PendingArea && lObjPermissionEnum == PermissionsEnum.Permission.AuthorizeOperations)
                {
                    UpdateStatus(StatusEnum.Authorized_Ope_Admon);
                    lObjMessageDTO.Message = "Estatus del comprobante " + txtFolio.Value + " Area:" + lObjVoucher.Area + " ha cambiado a: \"" + StatusEnum.Authorized_Ope_Admon.GetDescription() + "\"";
                    LogService.WriteSuccess("btnAuthor_ClickBefore: Estatus del comprobante " + txtFolio.Value + " ha cambiado a: \"" + StatusEnum.Authorized_Ope_Admon.GetDescription() + "\"");
                    lObjMessageDTO.UserId = mObjPurchasesServiceFactory.GetPurchaseMessageService().GetUserId(lObjMessageDTO.UserCode);
                    if (lObjCreateAlert.SaveAlert(lObjMessageDTO))
                    {
                        LogService.WriteSuccess(" btnAuthor_ClickBefore: Alerta enviada correctamente usuario: " + lObjVoucher.UserCode + ", Folio:" + lObjVoucher.RowCode + " Area: " + lObjVoucher.Area);
                    }

                    lStrMessageType = "CO_COMP_1";
                }
                else
                {
                    UIApplication.ShowMessageBox("Estatus incorrecto para autorizar: Estatus del documento:" + lObjStatusEnum.GetDescription() + " Permiso: " + lObjPermissionEnum.GetDescription());
                    LogService.WriteWarning("btnAuthor_ClickBefore: Estatus incorrecto para autorizar: Estatus del documento:" + lObjStatusEnum.GetDescription() + " Permiso: " + lObjPermissionEnum.GetDescription());
                }

                if (lStrMessageType != "")
                {
                    List<MessageDTO> lLstMessageDTO = mObjPurchasesServiceFactory.GetPurchaseMessageService().GetMessage(lStrMessageType).ToList();
                    foreach (MessageDTO lObjMessage in lLstMessageDTO)
                    {
                        lObjMessage.Message += " Folio:" + lObjVoucher.Folio + " Area:" + lObjVoucher.Area;
                        if (lObjCreateAlert.SaveAlert(lObjMessage))
                        {
                            LogService.WriteSuccess("btnAuthor_ClickBefore: Alerta enviada correctamente usuario: " + lObjVoucher.UserCode + ", Folio:" + lObjVoucher.RowCode + " Area: " + lObjVoucher.Area);
                        }
                    }
                    ClearControls();
                    LogService.WriteSuccess("btnAuthor_ClickBefore: Autorizacion de documento: " + lObjVoucher.RowCode + " Realizado correctamente, Status: " + ((StatusEnum)lObjVoucher.Status).GetDescription());
                }


            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("btnAuthor_ClickBefore: " + ex.Message);
                LogService.WriteError(ex);
            }

        }
        /// <summary>
        /// Funcionalidad de Rechazar un documento
        /// <summary>
        private void btnReject_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;


            Vouchers lObjVoucher = new Vouchers();
            lObjVoucher = GetVoucherForm();
            try
            {
                txtComents.Item.Click();
                StatusEnum lObjStatusEnum = (StatusEnum)Convert.ToInt16(cboStatus.Value);
                if (lObjStatusEnum != StatusEnum.Pending && lObjStatusEnum != StatusEnum.Authorized_Ope_Admon)
                {
                    if (lObjVoucher.LstVouchersDetail.Where(x => x.NA == "Y").Count() > 0)
                    {
                        if (mObjPurchasesServiceFactory.GetVouchersService().Update(lObjVoucher) != 0)
                        {
                            string lStrerror = DIApplication.Company.GetLastErrorDescription();

                        }
                        else
                        {
                            foreach (VouchersDetail lObjVouchersDetail in lObjVoucher.LstVouchersDetail)
                            {
                                if (mObjPurchasesServiceFactory.GetVouchersDetailService().Update(lObjVouchersDetail) != 0)
                                {
                                    string lStrerror = DIApplication.Company.GetLastErrorDescription();
                                }
                            }

                            CreateAlert lObjCreateAlert = new CreateAlert();
                            MessageDTO lObjMessageDTO = new MessageDTO();
                            lObjMessageDTO.UserCode = lObjVoucher.UserCode;
                            lObjMessageDTO.Message = "El comprobante con el folio: " + txtFolio.Value + " Area:" + lObjVoucher.Area + " ha sido rechazado por el usuario: " + UIApplication.GetCompany().UserName;
                            LogService.WriteSuccess("btnAuthor_ClickBefore: Estatus del comprobante " + txtFolio.Value + " ha cambiado a: \"" + StatusEnum.Pending.GetDescription() + "\"");
                            lObjMessageDTO.UserId = mObjPurchasesServiceFactory.GetPurchaseMessageService().GetUserId(lObjMessageDTO.UserCode);
                            if (lObjCreateAlert.SaveAlert(lObjMessageDTO))
                            {
                                LogService.WriteSuccess(" btnAuthor_ClickBefore: Alerta enviada correctamente usuario: " + lObjVoucher.UserCode + ", Folio:" + lObjVoucher.RowCode + " Area: " + lObjVoucher.Area);
                            }


                            UpdateStatus(StatusEnum.Pending);
                            ClearControls();
                            LogService.WriteSuccess("Rechazo de documento: " + lObjVoucher.RowCode + " Realizado correctamente, Status: " + ((StatusEnum)lObjVoucher.Status).GetDescription());
                        }
                    }
                    else
                    {
                        UIApplication.ShowMessageBox("Favor de seleccionar documentos a rechazar");
                        LogService.WriteWarning("btnReject_ClickBefore:  Favor de seleccionar documentos a rechazar");
                    }
                }
                else
                {
                    UIApplication.ShowMessageBox("No es posible rechazar un documento con estatus pendiente o autorizado");
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("btnReject_ClickBefore: " + ex.Message);
                LogService.WriteError(ex);
            }

            // mObjPurchasesServiceFactory.GetVouchersDetailService().Update()
        }

        /// <summary>
        /// Funcionalidad de Rechazar un documento
        /// <summary>
        private void btnNotify_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                Vouchers lObjVoucher = new Vouchers();
                lObjVoucher = GetVoucherForm();
                if (lObjVoucher.LstVouchersDetail.Where(x => x.Type == "XML" && x.Status == "Abierto").Count() == 0)
                {
                    if (lObjVoucher.LstVouchersDetail.Count() > 0)
                    {
                        if (lObjVoucher.Status == (int)StatusEnum.Pending)
                        {
                            //string lStrMessageDTO = mObjPurchasesDAO.GetMessage("CO_COMP_1").ToList()[0].Message; //CO_COMP_1 Operaciones //CO_COMP_2 
                            List<MessageDTO> lLstMessageDTO = new List<MessageDTO>();
                            if (lObjVoucher.Area == "MQ_MAQUI")
                            {
                                lLstMessageDTO = mObjPurchasesServiceFactory.GetPurchaseMessageService().GetMessage("CO_MAQUI_1").ToList();
                            }
                            else
                            {
                                lLstMessageDTO = mObjPurchasesServiceFactory.GetPurchaseMessageService().GetMessage("CO_COMP_3").ToList();// mObjPurchasesDAO.GetUsersMessage(lStrMessageDTO, lObjVoucher.Area).ToList();
                            }

                            foreach (MessageDTO lObjMessage in lLstMessageDTO)
                            {
                                CreateAlert lObjAlert = new CreateAlert();
                                lObjMessage.Message += " Folio:" + lObjVoucher.Folio + " Area: " + lObjVoucher.Area;
                                lObjAlert.SaveAlert(lObjMessage);
                            }

                            UpdateStatus(StatusEnum.PendingArea);
                            ClearControls();
                            LogService.WriteSuccess("Notificacion: " + lObjVoucher.RowCode + " Realizado correctamente, Status: " + ((StatusEnum)lObjVoucher.Status).GetDescription());
                        }
                        else
                        {
                            UIApplication.ShowMessageBox("Estatus incorrecto para notificar, Estatus del documento:" + ((StatusEnum)lObjVoucher.Status).GetDescription());
                            LogService.WriteWarning("Estatus incorrecto para notificar, Estatus del documento:" + ((StatusEnum)lObjVoucher.Status).GetDescription());
                        }
                    }
                    else
                    {
                        UIApplication.ShowMessageBox("No existen registros a notificar");
                        LogService.WriteWarning("No existen registros a notificar");
                    }
                }
                else
                {
                    UIApplication.ShowMessageBox("Existen registros con estatus abierto");
                    LogService.WriteWarning("Existen registros con estatus abierto");
                }

            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("btnNotify_ClickBefore: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Seleccionar un renglon de la matriz
        /// <summary>
        private void mtxReceipt_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (pVal.Row > 0)
                {
                    mtxReceipt.SelectRow(pVal.Row, true, false);
                    mIntRowSelected = pVal.Row;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("mtxReceipt_ClickBefore: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Seleccion del linkbutton
        /// <summary>
        private void mtxReceipt_LinkPressedBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                //ColUID = "C_DocEntrJ"

                if (pVal.ColUID == "C_DocEntrF")
                {
                    string lStrDocEntry = DtMatrix.GetValue("C_DocEntrF", pVal.Row - 1).ToString();
                    SAPbouiCOM.Framework.Application.SBO_Application.OpenForm(BoFormObjectEnum.fo_PurchaseInvoice, "", lStrDocEntry);
                }
                if (pVal.ColUID == "C_DocEntrJ")
                {
                    string lStrDocEntry = DtMatrix.GetValue("C_DocEntrJ", pVal.Row - 1).ToString();
                    SAPbouiCOM.Framework.Application.SBO_Application.OpenForm(BoFormObjectEnum.fo_JournalPosting, "", lStrDocEntry);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("mtxReceipt_LinkPressedBefore: " + ex.Message);
                LogService.WriteError(ex);
            }
        }


        /// <summary>
        /// Actualizacion de el datatable al modificar el contenido de la matriz
        /// <summary>
        private void mtxReceipt_ValidateAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                if (pVal.ColUID == "C_Coment")
                {
                    string lStrComent = (mtxReceipt.Columns.Item("C_Coment").Cells.Item(pVal.Row).Specific as EditText).Value.Trim();
                    DtMatrix.SetValue("C_Coment", pVal.Row - 1, lStrComent);
                }
                if (pVal.ColUID == "C_NA")
                {
                    bool lBolChecked = (mtxReceipt.Columns.Item("C_NA").Cells.Item(pVal.Row).Specific as CheckBox).Checked;
                    if (lBolChecked)
                    {
                        DtMatrix.SetValue("C_NA", pVal.Row - 1, "Y");
                    }
                    else
                    {
                        DtMatrix.SetValue("C_NA", pVal.Row - 1, "N");
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("mtxReceipt_ClickBefore: " + ex.Message);
                LogService.WriteError(ex);
            }
        }
        #endregion

        #region Controls
        private SAPbouiCOM.StaticText lblFolio;
        private SAPbouiCOM.StaticText lblArea;
        private SAPbouiCOM.StaticText lblEmploye;
        private SAPbouiCOM.StaticText lblComents;
        private SAPbouiCOM.EditText txtFolio;
        private SAPbouiCOM.EditText txtArea;
        private SAPbouiCOM.EditText txtEmploye;
        private SAPbouiCOM.EditText txtComents;
        private SAPbouiCOM.StaticText lblFecha;
        private SAPbouiCOM.StaticText lblTotal;
        private SAPbouiCOM.StaticText lblStatus;
        private SAPbouiCOM.EditText txtDate;
        private SAPbouiCOM.EditText txtTotal;
        private SAPbouiCOM.Button btnCFDI;
        private SAPbouiCOM.Button btnNotes;
        private SAPbouiCOM.Button btnCancel;
        private SAPbouiCOM.Matrix mtxReceipt;
        private SAPbouiCOM.Button btnSave;
        private SAPbouiCOM.Button btnNotify;
        private SAPbouiCOM.Button btnReject;
        private SAPbouiCOM.Button btnAuthor;
        private StaticText lblCodeMov;
        private EditText txtCodeMov;
        private ComboBox cboStatus;
        private DataTable DtMatrix;
        private EditText txtAreaF;
        private EditText txtEmpF;
        private CheckBox chkCopy;
        private Button btnInv;
        private Button btnPay;
        #endregion


        private void LinkedButton0_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {


        }

        private void LinkedButton1_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {


        }

        private void btnInv_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {

            BubbleEvent = true;
            FillMatrixInvoice();

        }

        private void btnPay_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            DoPayments();


        }

        private void FillMatrixInvoice()
        {
            try
            {
                DtMatrix.Rows.Clear();
                if (!string.IsNullOrEmpty(txtArea.Value))
                {
                    txtComents.Item.Click();


                    bool lBolUpdateStatus = false;
                    List<VouchersDetailDTO> lLstVouchersDetail = mObjPurchasesServiceFactory.GetPurchaseVouchersService().GetInvoiceVouchesDetail(txtArea.Value, txtFolio.Value);


                    mObjProgressBar = new ProgressBarManager(UIApplication.GetApplication(), "Cargando facturas", lLstVouchersDetail.Count + 2);
                    if (lLstVouchersDetail.Count() > 0)
                    {

                        List<Vouchers> lObjVouchers = mObjPurchasesServiceFactory.GetPurchaseVouchersService().GetVouches(mStrRowCode);
                        if (lObjVouchers.Count > 0 & lObjVouchers[0].Total != lLstVouchersDetail.Where(y => y.Status != "Cancelado").Sum(x => x.Total))
                        {
                            UpdateTotal(lLstVouchersDetail[0].CodeVoucher);
                        }
                        txtTotal.Value = (lLstVouchersDetail.Where(y => y.Status != "Cancelado").Sum(x => x.Total)).ToString(); // lObjVouchers[0].Total.ToString();
                        mObjProgressBar.NextPosition();
                    }
                    int i = 0;

                    if (lLstVouchersDetail.Where(y => y.Status != "Cancelado").ToList().Count > 0)
                    {
                        txtDate.Item.Enabled = false;
                    }
                    else
                    {
                        txtDate.Item.Enabled = true;
                    }

                    foreach (VouchersDetailDTO lObjVoucherDetailDTO in lLstVouchersDetail)
                    {
                        if (!string.IsNullOrEmpty(lObjVoucherDetailDTO.Line))
                        {
                            lBolUpdateStatus = mObjPurchasesServiceFactory.GetPurchaseInvoiceService().UpdateStatus(lObjVoucherDetailDTO);
                        }
                        else
                        {
                            // lBolUpdateStatus = AddVoucherDetail(lObjVoucherDetail);
                        }
                        DtMatrix.Rows.Add();
                        DtMatrix.SetValue("#", i, i + 1);
                        if (string.IsNullOrEmpty(lObjVoucherDetailDTO.NA) || lObjVoucherDetailDTO.NA == "N")
                        {
                            DtMatrix.SetValue("C_NA", i, "N");
                        }
                        else
                        {
                            DtMatrix.SetValue("C_NA", i, "Y");
                        }
                        DtMatrix.SetValue("C_RowCode", i, lObjVoucherDetailDTO.RowCode);
                        DtMatrix.SetValue("C_Date", i, lObjVoucherDetailDTO.Date);
                        DtMatrix.SetValue("C_Type", i, lObjVoucherDetailDTO.Type);
                        DtMatrix.SetValue("C_DocNum", i, lObjVoucherDetailDTO.DocNum);

                        if (lObjVoucherDetailDTO.Type == "Nota")
                        {
                            DtMatrix.SetValue("C_DocEntrJ", i, lObjVoucherDetailDTO.DocEntry);
                        }
                        else
                        {
                            DtMatrix.SetValue("C_DocEntrF", i, lObjVoucherDetailDTO.DocEntry);
                            DtMatrix.SetValue("C_DocNumF", i, lObjVoucherDetailDTO.DocNum);
                        }

                        DtMatrix.SetValue("C_Provider", i, lObjVoucherDetailDTO.Provider);
                        DtMatrix.SetValue("C_Status", i, lObjVoucherDetailDTO.Status);
                        DtMatrix.SetValue("C_Coments", i, lObjVoucherDetailDTO.Coments);
                        DtMatrix.SetValue("C_Subtotal", i, lObjVoucherDetailDTO.Subtotal);
                        DtMatrix.SetValue("C_Iva", i, lObjVoucherDetailDTO.IVA);
                        DtMatrix.SetValue("C_ISR", i, lObjVoucherDetailDTO.ISR);
                        DtMatrix.SetValue("C_IEPS", i, lObjVoucherDetailDTO.IEPS);
                        DtMatrix.SetValue("C_Total", i, lObjVoucherDetailDTO.Total);
                        DtMatrix.SetValue("C_Coment", i, lObjVoucherDetailDTO.Coment);
                        DtMatrix.SetValue("C_User", i, lObjVoucherDetailDTO.UserCode);
                        DtMatrix.SetValue("C_VoucherCode", i, lObjVoucherDetailDTO.CodeVoucher);
                        DtMatrix.SetValue("C_DocFolio", i, lObjVoucherDetailDTO.DocFolio);
                        i++;
                        if (lBolUpdateStatus)
                        {
                            // FillMatrixInvoice();
                        }
                        mObjProgressBar.NextPosition();
                    }



                    mtxReceipt.Columns.Item("#").DataBind.Bind("VoucherDataTable", "#");
                    mtxReceipt.Columns.Item("C_NA").DataBind.Bind("VoucherDataTable", "C_NA");
                    mtxReceipt.Columns.Item("C_Date").DataBind.Bind("VoucherDataTable", "C_Date");
                    mtxReceipt.Columns.Item("C_Type").DataBind.Bind("VoucherDataTable", "C_Type");
                    mtxReceipt.Columns.Item("C_DocEntrF").DataBind.Bind("VoucherDataTable", "C_DocNumF");//C_DocEntrF
                    mtxReceipt.Columns.Item("C_DocEntrJ").DataBind.Bind("VoucherDataTable", "C_DocEntrJ");
                    mtxReceipt.Columns.Item("C_Provider").DataBind.Bind("VoucherDataTable", "C_Provider");
                    mtxReceipt.Columns.Item("C_Status").DataBind.Bind("VoucherDataTable", "C_Status");
                    mtxReceipt.Columns.Item("C_Coments").DataBind.Bind("VoucherDataTable", "C_Coments");
                    mtxReceipt.Columns.Item("C_Subtotal").DataBind.Bind("VoucherDataTable", "C_Subtotal");
                    mtxReceipt.Columns.Item("C_Iva").DataBind.Bind("VoucherDataTable", "C_Iva");
                    mtxReceipt.Columns.Item("C_ISR").DataBind.Bind("VoucherDataTable", "C_ISR");
                    mtxReceipt.Columns.Item("C_IEPS").DataBind.Bind("VoucherDataTable", "C_IEPS");
                    mtxReceipt.Columns.Item("C_Total").DataBind.Bind("VoucherDataTable", "C_Total");
                    mtxReceipt.Columns.Item("C_Coment").DataBind.Bind("VoucherDataTable", "C_Coment");
                    mtxReceipt.Columns.Item("C_DocFolio").DataBind.Bind("VoucherDataTable", "C_DocFolio");
                    this.UIAPIRawForm.Freeze(true);
                    mIntRowSelected = 0;
                    mtxReceipt.LoadFromDataSource();
                    mtxReceipt.AutoResizeColumns();
                    this.UIAPIRawForm.Freeze(false);
                    LogService.WriteSuccess("FillMatrixInvoice carga correcta de datos Code:" + mStrRowCode + ", Cantidad: " + lLstVouchersDetail.Count);
                    mObjProgressBar.NextPosition();

                }
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("FillMatrixInvoice: " + ex.Message);
                LogService.WriteError(ex);
                //throw;
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
                mObjProgressBar.Dispose();
            }
        }

        /// <summary>
        /// Realizar los pagos
        /// </summary>
        private void DoPayments()
        {

            var lLstVoucherDetail = new List<PurchaseXMLDTO>();
            var lBolIsSuccess = false;

            PurchasePaymentDAO purchasePaymentDAO = new PurchasePaymentDAO();

            try
            {
                List<VouchersDetailDTO> lLstVouchersDetail = mObjPurchasesServiceFactory.GetPurchaseVouchersService().GetInvoiceVouchesDetail(txtArea.Value, txtFolio.Value);

                string lStrArea = !string.IsNullOrEmpty(txtArea.Value) ? txtArea.Value : txtAreaF.Value;
                var account = GetAccountRefound(lStrArea);

                DIApplication.Company.StartTransaction();

                for (int i = 0; i < DtMatrix.Rows.Count; i++)
                {
                    int lIntPago = i + 1;
                    UIApplication.ShowSuccess("Generando pago " + lIntPago + " de " + DtMatrix.Rows.Count);
                    var lObjPayment = new PurchaseXMLDTO();
                    var lStrtype = DtMatrix.GetValue("C_Type", i).ToString();

                    if (lStrtype == "XML")
                    {
                        lObjPayment.CardCode = DtMatrix.GetValue("C_Provider", i).ToString();
                        lObjPayment.Total = DtMatrix.GetValue("C_Total", i).ToString();
                        lObjPayment.DocEntry = DtMatrix.GetValue("C_DocEntrF", i).ToString() == "" ?
                            int.Parse(DtMatrix.GetValue("C_DocEntrJ", i).ToString())
                          : int.Parse(DtMatrix.GetValue("C_DocEntrF", i).ToString());
                        lObjPayment.DocDate =  Convert.ToDateTime(this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_Date").Value);
                        lObjPayment.TaxDate = Convert.ToDateTime(this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_Date").Value);
                        lObjPayment.Account = account;
                        lObjPayment.Employee = mStrEmployeId;
                        lObjPayment.Area = lStrArea;
                        lObjPayment.CodeMov = purchasePaymentDAO.GetCodeMov(lObjPayment.DocEntry, lStrtype);

                        var existPayment = mObjPurchasesServiceFactory.GetPurchaseReceiptsService().ExistsPayment(lObjPayment.DocEntry.ToString());
                        if (!existPayment)
                        {
                            PaymentDI lObjPaymentDI = new PaymentDI();
                            lBolIsSuccess = lObjPaymentDI.CreatePayment(lObjPayment);
                        }
                        else
                        {
                            continue;
                        }

                        if (!lBolIsSuccess)
                        {
                            if (DIApplication.Company.InTransaction)
                            {
                                DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                                LogService.WriteError("DoPayments: Fallo en Crearse el pago de la factura: " + lObjPayment.DocEntry);
                                UIApplication.ShowMessageBox("Fallo en Crearse el pago con total: " + lObjPayment.Total + " los pagos anteriores fueron revertidos");
                                return;
                            }
                        }

                    }
                }
                if (lLstVoucherDetail.Count > 0)
                {
                    lBolIsSuccess = UpdateTotal(lLstVouchersDetail[0].CodeVoucher);

                }

                foreach (VouchersDetail lObjVoucherDetail in lLstVouchersDetail)
                {
                    if (string.IsNullOrEmpty(lObjVoucherDetail.Line))
                    {
                        lBolIsSuccess = AddVoucherDetail(lObjVoucherDetail);
                        if (!lBolIsSuccess)
                        {
                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogService.WriteError("DoPayments: " + ex.Message);
                LogService.WriteError(ex);
                return;
            }
            finally
            {
                CommitTransaction(lBolIsSuccess);
                FillMatrixInvoice();
            }


        }


        private void CommitTransaction(bool pBolSuccess)
        {
            try
            {
                if (pBolSuccess)
                {
                    DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    UIApplication.ShowMessageBox(string.Format("Proceso realizado correctamente"));

                }
                else
                {
                    //mStrCodeVoucher = string.Empty;
                    if (DIApplication.Company.InTransaction)
                    {
                        DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                    }
                }

            }
            catch (Exception ex)
            {
                mObjProgressBar.Dispose();
                this.UIAPIRawForm.Freeze(false);
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("(btnSave_ClickBefore): " + ex.Message);
                LogService.WriteError(ex);
            }
        }


        /// <summary>
        /// Agregar detalles al comprobante
        /// </summary>
        private bool AddVoucherDetail(VouchersDetail pObjVouchersDetail)
        {
            try
            {
                //Cambio factura preliminar
                /* string lStrDocNum = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetDocNum(pObjDocument.DocEntry.ToString());

                 string lStrDocStatus = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetDocCanceled(pObjDocument.DocEntry.ToString(), "XML");
                 if(!lStrDocStatus.Equals("Cancelado")) {
                     lStrDocStatus = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetDocStatus(pObjDocument.DocEntry.ToString());
                 }*/



                // descomentar
                /* pObjVouchersDetail.IEPS = pObjDocument.Ieps;
                 //lObjVouchersDetail.IEPS = pObjDocument.ConceptLines.SelectMany(b => b.LstTaxes.Where(x => x.Tax == "003")).Sum(x => Convert.ToDouble(x.Amount));//.Sum(x => Convert.ToDouble(x.Amount));
                 pObjVouchersDetail.ISR = pObjDocument.RetISR;
                 pObjVouchersDetail.IVA = pObjDocument.Iva;
                 pObjVouchersDetail.RetIVA = pObjDocument.RetIva + pObjDocument.RetIva4;*/


                Vouchers lObjVouchers = GetVoucherForm();

                pObjVouchersDetail.Type = "XML";
                // mStrRowCode = mObjPurchasesServiceFactory.GetPurchasePaymentService().GetVoucherCode(lObjVouchers.Folio., lObjVouchers.Area, (int)mEnumType);
                pObjVouchersDetail.Line = (mObjPurchasesServiceFactory.GetPurchaseVouchersService().GetVouchesDetail(mStrRowCode).Count() + 1).ToString();



                pObjVouchersDetail.CodeVoucher = mStrRowCode;

                if (mObjPurchasesServiceFactory.GetVouchersDetailService().Add(pObjVouchersDetail) == 0)
                {
                    UpdateInvoiceLine(Convert.ToInt32(pObjVouchersDetail.DocEntry), pObjVouchersDetail.Line);

                    LogService.WriteSuccess("InvoiceDI AddDetail" + pObjVouchersDetail.DocNum);


                    if (UpdateTotal(pObjVouchersDetail.CodeVoucher))
                    {
                        return true;
                    }
                }
                else
                {
                    UIApplication.ShowMessageBox(string.Format("Exception: {0}", DIApplication.Company.GetLastErrorDescription()));
                    LogService.WriteError("InvoiceDI (AddVoucherDetail) " + DIApplication.Company.GetLastErrorDescription());
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("InvoiceDI (CreateDocument) " + ex.Message);
                LogService.WriteError(ex);
            }
            return false;
        }

        private void UpdateInvoiceLine(int pIntDocEnrtry, string pStrLine)
        {
            SAPbobsCOM.Documents lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
            lObjDocument.GetByKey(pIntDocEnrtry);
            lObjDocument.UserFields.Fields.Item("U_MQ_OrigenFol_Det").Value = pStrLine;
            int i = lObjDocument.Update();
        }


        private bool UpdateTotal(string pStrCodeVoucher)
        {
            if (mObjPurchasesServiceFactory.GetVouchersService().UpdateTotal(pStrCodeVoucher) != 0)
            {
                string lstr = DIApplication.Company.GetLastErrorDescription();
                LogService.WriteError("InvoiceDI (UpdateTotal) " + DIApplication.Company.GetLastErrorDescription());
                return false;
            }
            else
            {
                LogService.WriteSuccess("InvoiceDI UpdateTotal" + pStrCodeVoucher);
                return true;
            }
        }

        private void Form_ResizeAfter(SBOItemEventArg pVal)
        {
            mtxReceipt.AutoResizeColumns();

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




        public void printUDO(string pStrArea, string pStrFolio)
        {
            try
            {
                // execute menu and enter document number
                string lStrMenu = mObjPurchasesServiceFactory.GetPurchaseReceiptsService().GetMenuId();
                UIApplication.GetApplication().ActivateMenuItem(lStrMenu);
                Form lObjForm = UIApplication.GetApplication().Forms.ActiveForm;
                ((EditText)lObjForm.Items.Item("1000003").Specific).String = pStrArea;
                ((EditText)lObjForm.Items.Item("1000009").Specific).String = pStrFolio;
                lObjForm.Items.Item("1").Click(BoCellClickType.ct_Regular); // abrir reporte
            }
            catch (Exception ex)
            {
                LogService.WriteError("printUDO " + ex.Message);
                LogService.WriteError(ex);
            }
        }

       
        
    }
}
