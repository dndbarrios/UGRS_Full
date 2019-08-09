using System;
using System.Collections.Generic;
using System.Linq;
using SAPbouiCOM.Framework;
using UGRS.Core.Extension.Enum;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;
using SAPbouiCOM;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.Transports.Enums;
using UGRS.Core.SDK.DI.Transports.DTO;
using UGRS.Core.SDK.DI.Transports;
using UGRS.AddOn.Transports.ModalForms;
using UGRS.Core.SDK.DI.Transports.Tables;
using UGRS.Core.Utility;

namespace UGRS.AddOn.Transports.Forms
{
    [FormAttribute("UGRS.AddOn.Transports.Forms.frmFreights", "Forms/frmFreights.b1f")]
    class frmFreights : UserFormBase
    {
        #region Properties
        private mfrmRouteFinder mModalFrmRouteFinder = null;
        private mfrmCFL mModalFrmCFL = null;
        TransportServiceFactory mObjTransportsFactory = new TransportServiceFactory();
        List<VehiclesDTO> mLstVehicles;

        private bool mBoolRoadLoaded = false;
        private bool mBoolFormLoaded = false;
        private int mIntUserSignature = 0;
        private int mIntActualRoute = 0;
        private float mFlKm = 0;
        private bool mBoolInsurance = false;
        private bool mBoolInternal;
        private bool mBoolSearchMode;
        private int mIntFormType;
        private string mStrFolio = string.Empty;
        private string mStrKey = string.Empty;
        private string mStrCardCode = string.Empty;
        private float mFlRetention = 0;
        private float mFlTax = 0;
        private bool mBolWT = false;
        private string mStrJounralEntryId = string.Empty;
        private string mStrDriverId = string.Empty;
        private InsuranceDTO mObjInsurance;
        private string mStrType = string.Empty;
        private string mStrTxtCFL = string.Empty;
            //private List<SalesOrderLinesDTO> mLstSalesOrderLines = null;

        private CFLParamsDTO mObjCFLParameters = null;
        private SalesOrderLinesDTO mObjSalesOrderLines = null;
        private SalesOrderLinesDTO mObjRoutes = null;
        #endregion

        #region Controls
        //Sales Order Form
        SAPbouiCOM.Form SO_FrmSalesOrder;
        SAPbouiCOM.Form SO_FrmFields;
        SAPbouiCOM.EditText SO_TxtFolio;
        SAPbouiCOM.EditText SO_TxtShared;

        SAPbouiCOM.Matrix SO_MtxSO;
        SAPbouiCOM.ComboBox SO_CboPyloadType;
        SAPbouiCOM.ComboBox SO_CboVehicleType;
        SAPbouiCOM.EditText SO_TxtRoute;
        //SAPbouiCOM.EditText SO_TxtTotKm;
        SAPbouiCOM.EditText SO_TxtKmA;
        SAPbouiCOM.EditText SO_TxtKmB;
        SAPbouiCOM.EditText SO_TxtKmC;
        SAPbouiCOM.EditText SO_TxtKmD;
        SAPbouiCOM.EditText SO_TxtKmE;
        SAPbouiCOM.EditText SO_TxtKmF;
        SAPbouiCOM.EditText SO_TxtTotKG;
        SAPbouiCOM.EditText SO_TxtExtra;
        //SAPbouiCOM.EditText SO_TxtAnthPyld;
        SAPbouiCOM.EditText SO_TxtDestination;
        SAPbouiCOM.EditText SO_TxtItem;
        SAPbouiCOM.EditText SO_TxtQtty;
        SAPbouiCOM.EditText SO_TxtPrice;
        SAPbouiCOM.ComboBox SO_CboEmployee;
        SAPbouiCOM.EditText SO_TxtoAF;
        SAPbouiCOM.EditText SO_TxtHeads;
        SAPbouiCOM.EditText SO_TxtSacos;
        SAPbouiCOM.EditText SO_TxtVarios;
        SAPbouiCOM.EditText SO_TxtAFDesc;
        SAPbouiCOM.EditText SO_TxtKilometers;

        SAPbobsCOM.UserTable mObjInternalFreight;

        public SAPbouiCOM.Button CFL_Btn;
        public SAPbouiCOM.Matrix CFL_Mtx;
        public bool pBoolFreightsModal;
        #endregion

        #region Constructor
        //public frmFreights(int pIntFormType, int pIntUserSign = 0, bool pBoolInternal = false)
        //{
        //    mIntUserSignature = pIntUserSign != 0 ? pIntUserSign : DIApplication.Company.UserSignature;
        //    mBoolInternal = pBoolInternal;
        //    mIntFormType = pIntFormType;

        //    if (mBoolInternal)
        //    {
        //        SetInternalConfig();
        //    }
        //    else
        //    {
        //        pBoolFreightsModal = true;
        //    }
        //    InitializeForm();
        //}

        //public frmFreights(int pIntFormType, int pIntUserSignature, SalesOrderLinesDTO pObjSalesOrderLines, bool pBoolInsurance)
        //{
        //    mIntFormType = pIntFormType;
        //    mIntUserSignature = pIntUserSignature;
        //    mObjSalesOrderLines = pObjSalesOrderLines;
        //    mBoolInsurance = pBoolInsurance;
        //    LoadForm();
        //}

        public frmFreights(FreightsParamsDTO pObjParameters)
        {
            mStrCardCode = pObjParameters.CardCode;
            if (pObjParameters.Loaded)
            {
                mIntFormType = pObjParameters.FormType;
                mIntUserSignature = pObjParameters.UserSign;
                mObjSalesOrderLines = pObjParameters.SalesOrderLines;
                mBoolInsurance = pObjParameters.Insurance;
                mBoolFormLoaded = pObjParameters.Loaded;
                LoadForm();
            }
            else
            {
                mIntUserSignature = pObjParameters.UserSign != 0 ? pObjParameters.UserSign : DIApplication.Company.UserSignature;
                mBoolInternal = pObjParameters.Internal;
                mIntFormType = pObjParameters.FormType;

                if (mBoolInternal)
                {
                    SetInternalConfig();
                }
                else
                {
                    pBoolFreightsModal = true;
                }
                InitializeForm();
            }
        }


        #endregion

        #region Methods
        #region Load & Unload Events
        private void LoadEvents()
        {
            
            UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
            UIApplication.GetApplication().MenuEvent += frmFreights_MenuEvent;
            this.cboShared.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.lObjCboShared_ComboSelectAfter);
            this.cboVehicleType.ComboSelectAfter += new _IComboBoxEvents_ComboSelectAfterEventHandler(this.lObjCboVehicleType_ComboSelectAfter);
            this.btnUpdate.ClickBefore += new _IButtonEvents_ClickBeforeEventHandler(this.lObjBtnUpdate_ClickBefore);
            this.btnRoute.ClickBefore += new _IButtonEvents_ClickBeforeEventHandler(this.lObjBtnRoute_ClickBefore);
            this.txtKmA.ValidateBefore += new _IEditTextEvents_ValidateBeforeEventHandler(this.lObjTxt_Validate);
            this.txtKmB.ValidateBefore += this.lObjTxt_Validate;
            this.txtKmC.ValidateBefore += this.lObjTxt_Validate;
            this.txtKmD.ValidateBefore += this.lObjTxt_Validate;
            this.txtKmE.ValidateBefore += this.lObjTxt_Validate;
            this.txtKmF.ValidateBefore += this.lObjTxt_Validate;
            this.txtOrigin.ValidateBefore += this.lObjTxt_Validate;
            this.txtDestination.ValidateBefore += this.lObjTxt_Validate;
            this.txtMorign.ValidateBefore += this.lObjTxt_Validate;
            this.txtMDest.ValidateBefore += this.lObjTxt_Validate;
            this.txtKmA.LostFocusAfter += new _IEditTextEvents_LostFocusAfterEventHandler(this.lObjTxt_LostFocusAfter);
            this.txtKmB.LostFocusAfter += this.lObjTxt_LostFocusAfter;
            this.txtKmC.LostFocusAfter += this.lObjTxt_LostFocusAfter;
            this.txtKmD.LostFocusAfter += this.lObjTxt_LostFocusAfter;
            this.txtKmE.LostFocusAfter += this.lObjTxt_LostFocusAfter;
            this.txtKmF.LostFocusAfter += this.lObjTxt_LostFocusAfter;
            this.txtExtra.LostFocusAfter += this.lObjTxt_LostFocusAfter;
            this.txtAmountEns.LostFocusAfter += this.lObjTxt_LostFocusAfter;
            this.txtAmountKm.LostFocusAfter += lObjTxtTotKM_LostAfter;
            this.chkEnsm.PressedBefore += chkEnsm_PressedBefore;
            this.btnAccept.ClickBefore += new _IButtonEvents_ClickBeforeEventHandler(this.lObjBtnAccept_ClickBefore);
            this.btnAccept.ClickAfter += new _IButtonEvents_ClickAfterEventHandler(this.lObjBtnAccept_ClickAfter);
            this.btnExit.ClickAfter += lObjBtnExit_ClickAfter;
            this.btnCancel.ClickAfter += lObjBtnCancel_ClickAfter;


            if (!mBoolInternal)
            {
                this.txtArticle.LostFocusAfter += lObjTxtArticle_LostFocusAfter;
                this.txtArticle.KeyDownAfter += lObjTxt_KeyDownAfter;
            }
            else
            {
                this.txtInternal.KeyDownAfter += lObjTxt_KeyDownAfter;
            }
            this.txtFolio.KeyDownAfter += lObjTxt_KeyDownAfter;
            this.txtEcNum.KeyDownAfter += lObjTxt_KeyDownAfter;
            this.txtMorign.KeyDownAfter += lObjTxt_KeyDownAfter;
            this.txtMDest.KeyDownAfter += lObjTxt_KeyDownAfter;
            this.txtDriver.KeyDownAfter += lObjTxt_KeyDownAfter;
            //}

        }

      
     

        private void txtTotalFreight_ValidateBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
        }

     

        public void UnloadEvents()
        {
            //UIApplication.GetApplication().ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
            UIApplication.GetApplication().MenuEvent -= frmFreights_MenuEvent;
            this.cboShared.ComboSelectAfter -= new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.lObjCboShared_ComboSelectAfter);
            this.cboVehicleType.ComboSelectAfter -= new _IComboBoxEvents_ComboSelectAfterEventHandler(this.lObjCboVehicleType_ComboSelectAfter);
            this.btnUpdate.ClickBefore -= new _IButtonEvents_ClickBeforeEventHandler(this.lObjBtnUpdate_ClickBefore);
            this.btnRoute.ClickBefore -= new _IButtonEvents_ClickBeforeEventHandler(this.lObjBtnRoute_ClickBefore);
            this.txtKmA.ValidateBefore -= new _IEditTextEvents_ValidateBeforeEventHandler(this.lObjTxt_Validate);
            this.txtKmB.ValidateBefore -= this.lObjTxt_Validate;
            this.txtKmC.ValidateBefore -= this.lObjTxt_Validate;
            this.txtKmD.ValidateBefore -= this.lObjTxt_Validate;
            this.txtKmE.ValidateBefore -= this.lObjTxt_Validate;
            this.txtKmF.ValidateBefore -= this.lObjTxt_Validate;
            this.txtOrigin.ValidateBefore -= this.lObjTxt_Validate;
            this.txtDestination.ValidateBefore -= this.lObjTxt_Validate;
            this.txtMorign.ValidateBefore -= this.lObjTxt_Validate;
            this.txtMDest.ValidateBefore -= this.lObjTxt_Validate;
            this.txtKmA.LostFocusAfter -= new _IEditTextEvents_LostFocusAfterEventHandler(this.lObjTxt_LostFocusAfter);
            this.txtKmB.LostFocusAfter -= this.lObjTxt_LostFocusAfter;
            this.txtKmC.LostFocusAfter -= this.lObjTxt_LostFocusAfter;
            this.txtKmD.LostFocusAfter -= this.lObjTxt_LostFocusAfter;
            this.txtKmE.LostFocusAfter -= this.lObjTxt_LostFocusAfter;
            this.txtKmF.LostFocusAfter -= this.lObjTxt_LostFocusAfter;
            this.txtExtra.LostFocusAfter -= this.lObjTxt_LostFocusAfter;
            this.txtAmountEns.LostFocusAfter -= this.lObjTxt_LostFocusAfter;
            this.chkEnsm.ClickBefore -= chkEnsm_ClickBefore;
            this.chkEnsm.PressedBefore -= chkEnsm_PressedBefore;
            //this.lObjBtnAccept.ClickAfter -= new _IButtonEvents_ClickAfterEventHandler(this.lObjBtnAccept_ClickAfter);
            this.btnCancel.ClickAfter -= lObjBtnCancel_ClickAfter;
            this.btnExit.ClickAfter -= lObjBtnExit_ClickAfter;




            if (!mBoolInternal)
            {
                this.txtArticle.LostFocusAfter -= lObjTxtArticle_LostFocusAfter;
                this.txtArticle.KeyDownAfter -= lObjTxt_KeyDownAfter;
            }
            else
            {
                this.txtInternal.KeyDownAfter -= lObjTxt_KeyDownAfter;
            }

            this.txtFolio.KeyDownAfter -= lObjTxt_KeyDownAfter;
            this.txtEcNum.KeyDownAfter -= lObjTxt_KeyDownAfter;
            this.txtMorign.KeyDownAfter -= lObjTxt_KeyDownAfter;
            this.txtMDest.KeyDownAfter -= lObjTxt_KeyDownAfter;
            this.txtDriver.KeyDownAfter -= lObjTxt_KeyDownAfter;
            //}
        }

        #endregion
        private void LoadForm()
        {
            InitializeForm();
            KmEnableDisable(true);
            LoadLine(false);
        }

        private void LoadLine(bool pBolLoadShared)
        {
            this.UIAPIRawForm.Freeze(true);
            //textboxes
            if (mObjSalesOrderLines != null)
            {
                int lIntShrd = (int)(mObjSalesOrderLines.Shared ? SharedEnum.Yes : SharedEnum.NO);
                if (!pBolLoadShared)
                {
                    txtArticle.Value = mObjSalesOrderLines.ItemCode;
                    txtAmountKm.Item.Enabled = false;
                }
                else
                {
                    txtAmountKm.Item.Enabled = true;
                }
                txtDesc.Value = mObjSalesOrderLines.Description;
                cboPayload.Select(mObjSalesOrderLines.PayloadType, BoSearchKey.psk_ByValue);
                cboVehicleType.Select(mObjSalesOrderLines.VehicleType, BoSearchKey.psk_ByValue);
                cboShared.Select(lIntShrd.ToString(), BoSearchKey.psk_ByValue);
                txtDriver.Value = mObjSalesOrderLines.Employee;
                txtEcNum.Value = mObjSalesOrderLines.Asset;
                lblAFDesc.Caption = mObjSalesOrderLines.AssetDesc;
                txtExtra.Value = mObjSalesOrderLines.Extra;
                txtFolio.Value = mObjSalesOrderLines.Folio;
                txtTax.Value = mObjSalesOrderLines.Tax.ToString();
                txtRetentions.Value = mObjSalesOrderLines.TaxWT.ToString();
                mObjRoutes = RoutesToDTO(mObjTransportsFactory.GetRouteService().GetRoute((long)mObjSalesOrderLines.Route));
                txtKg.Value = mObjSalesOrderLines.TotKg;
                SetRoutesTextsBoxes();
                txtAmountKm.Value = mObjSalesOrderLines.UnitPrice.ToString();

                LoadVarious();
                string lStrValidate = mObjTransportsFactory.GetRouteService().ValidateSinKM(txtArticle.Value);

                if (lStrValidate == "Y" || pBolLoadShared)
                {
                    txtAmountKm.Item.Enabled = true;
                }
                else
                {
                    txtAmountKm.Item.Enabled = false;
                }

              
                lnkJournal.Item.Visible = false;
                lnkCancel.Item.Visible = false;
            }
            KmEnableDisable(true);
            chkEnsm.Checked = mBoolInsurance ? true : false;
            this.UIAPIRawForm.Freeze(false);
        }

        private void LoadVarious()
        {
            if (!string.IsNullOrEmpty(mObjSalesOrderLines.PayloadType))
            {
                txtVarios.Item.Visible = true;
                lblVarios.Item.Visible = true;

                mStrType = mObjTransportsFactory.GetRouteService().GetTypeTRTY(mObjSalesOrderLines.PayloadType);
                switch (mStrType)
                {
                    case "G":
                        lblVarios.Caption = "Cabezas";
                        txtVarios.Value = mObjSalesOrderLines.Heads;
                        break;
                    case "A":
                        lblVarios.Caption = "Sacos";
                        txtVarios.Value = mObjSalesOrderLines.Bags;
                        break;
                    case "O":
                        lblVarios.Caption = "Varios";
                        txtVarios.Value = mObjSalesOrderLines.Varios;
                        break;
                }
            }
            else
            {
                txtVarios.Item.Visible = false;
                lblVarios.Item.Visible = false;
            }
        }

        private void LoadInternalFreight(string pStrFolio, bool pBolShared)
        {
            this.UIAPIRawForm.Freeze(true);
            try
            {
                mObjSalesOrderLines = new SalesOrderLinesDTO();
                if (!string.IsNullOrEmpty(pStrFolio))
                {
                    //mObjInternalFreight = mObjTransportsFactory.GetRouteService().GetFreight();

                    mObjInternalFreight = (SAPbobsCOM.UserTable)DIApplication.Company.UserTables.Item("UG_TR_INTLFRGHT");
                    if (mObjInternalFreight != null)
                    {
                        mObjInternalFreight.GetByKey(GetKeyByFolio(pStrFolio));
                    }
                }
                txtFolio.Value = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_Ticket").Value.ToString() : string.Empty;
                txtEcNum.Value = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_Asset").Value.ToString() : string.Empty;
                //mObjRoutes = mObjInternalFreight != null ? RoutesToDTO(mObjTransportsFactory.GetRouteService().GetRoute(Convert.ToInt64(mObjInternalFreight.UserFields.Fields.Item("U_Route").Value.ToString()))) : null;

                mObjRoutes = new SalesOrderLinesDTO();
                mObjRoutes.Origin = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_Origin").Value.ToString() : string.Empty;
                mObjRoutes.MOrigin = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_MOrigin").Value.ToString() : string.Empty;
                mObjRoutes.Destination = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_Destination").Value.ToString() : string.Empty;
                mObjRoutes.MDestination = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_MDestination").Value.ToString() : string.Empty;
                mObjRoutes.KmA = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_KmA").Value.ToString() : string.Empty;
                mObjRoutes.KmB = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_KmB").Value.ToString() : string.Empty;
                mObjRoutes.KmC = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_KmC").Value.ToString() : string.Empty;
                mObjRoutes.KmD = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_KmD").Value.ToString() : string.Empty;
                mObjRoutes.KmE = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_KmE").Value.ToString() : string.Empty;
                mObjRoutes.KmF = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_KmF").Value.ToString() : string.Empty;
                mIntActualRoute = mObjInternalFreight != null ? Convert.ToInt32(mObjInternalFreight.UserFields.Fields.Item("U_Route").Value.ToString()) : 0;

                SetRoutesTextsBoxes();
                mStrDriverId = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_Driver").Value.ToString() : string.Empty;
                txtDriver.Value = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_DriverName").Value.ToString() : string.Empty;
                txtExtra.Value = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_Extra").Value.ToString() : string.Empty;
                txtComment.Value = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_Comments").Value.ToString() : string.Empty;

                string lStrShared = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_Shared").Value.ToString() : "";
                string lStrPayload = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_PayloadType").Value.ToString() : "";
                string lStrCostingCode = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_Area").Value.ToString() : "";
                string lStrVehicleType = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_VehicleType").Value.ToString() : "";
                string lStrInsurance = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_Insurance").Value.ToString() : "N";

                txtKg.Value = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_TotKG").Value.ToString() : string.Empty;
                
                switch ( mObjTransportsFactory.GetRouteService().GetTypeTRTY(lStrPayload))
                {
                    case "G":
                        txtVarios.Value = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_Heads").Value.ToString() : string.Empty;
                        break;
                    case "A":
                        txtVarios.Value = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_Bags").Value.ToString() : string.Empty;
                        break;
                    case "O":
                        txtVarios.Value = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_Varios").Value.ToString() : string.Empty;
                        break;
                }

                cboShared.Select(lStrShared, SAPbouiCOM.BoSearchKey.psk_ByValue);
                cboVehicleType.Select(lStrVehicleType, SAPbouiCOM.BoSearchKey.psk_ByValue);
                cboPayload.Select(lStrPayload, SAPbouiCOM.BoSearchKey.psk_ByValue);
                cboArea.Select(lStrCostingCode, SAPbouiCOM.BoSearchKey.psk_ByValue);

                chkEnsm.Item.Enabled = true;
                chkEnsm.Checked = lStrInsurance.Equals("Y") ? true : false;
                txtEcNum.Value = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_Asset").Value.ToString() : string.Empty;
                
                if (!pBolShared)
                {
                    lnkJournal.Item.Visible = true;
                    lnkCancel.Item.Visible = true;
                    mStrJounralEntryId = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_JournalEntryId").Value.ToString() : string.Empty;
                    txtInternal.Value = mObjInternalFreight != null ? mObjInternalFreight.UserFields.Fields.Item("U_InternalFolio").Value.ToString() : mObjTransportsFactory.GetRouteService().GetFolio(pStrFolio);
                   SetInitialInternalConfig(txtInternal.Value); 
                    if (lStrShared == "2")
                    {
                         txtAmountKm.Value = mObjInternalFreight.UserFields.Fields.Item("U_AmountKM").Value.ToString();
                         SetKilometers(); 
                    }
                    else
                    {
                        SetKilometers(); 
                        SetKilometerAmount();
                    }
                }
               
              
                SetTotalAmount();
                LoadInsuranceAmount(lStrInsurance.Equals("Y") ? true : false);
                SetCashAmounts();
                this.UIAPIRawForm.Freeze(false);
            }
            catch (Exception lObjException)
            {
                this.UIAPIRawForm.Freeze(false);
                SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText(lObjException.Message
                                    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
                MemoryUtility.ReleaseComObject(mObjInternalFreight);
                mObjInternalFreight = null;
            }
        }

        private string GetKeyByFolio(string pStrFolio)
        {
            mStrKey = mObjTransportsFactory.GetRouteService().GetKeyByFolio(pStrFolio);

            return !string.IsNullOrEmpty(mStrKey) ? mStrKey : "";
        }

        private SalesOrderLinesDTO RoutesToDTO(Routes pObjRoutes)
        {
            if (mObjSalesOrderLines == null)
            {
                mObjSalesOrderLines = new SalesOrderLinesDTO();
            } 
           if (pObjRoutes == null) return null;
            mIntActualRoute = Convert.ToInt32(pObjRoutes.RowCode);
            
           

            return new SalesOrderLinesDTO()
            {
                RouteName = pObjRoutes.RowName.ToString(),
                KmA = string.IsNullOrEmpty(mObjSalesOrderLines.KmA) ? pObjRoutes.TypeA.ToString() : mObjSalesOrderLines.KmA,
                KmB = string.IsNullOrEmpty(mObjSalesOrderLines.KmB) ? pObjRoutes.TypeB.ToString() : mObjSalesOrderLines.KmB,
                KmC = string.IsNullOrEmpty(mObjSalesOrderLines.KmC) ? pObjRoutes.TypeC.ToString() : mObjSalesOrderLines.KmC,
                KmD = string.IsNullOrEmpty(mObjSalesOrderLines.KmD) ? pObjRoutes.TypeD.ToString() : mObjSalesOrderLines.KmD,
                KmE = string.IsNullOrEmpty(mObjSalesOrderLines.KmE) ? pObjRoutes.TypeE.ToString() : mObjSalesOrderLines.KmE,
                KmF = string.IsNullOrEmpty(mObjSalesOrderLines.KmF) ? pObjRoutes.TypeF.ToString() : mObjSalesOrderLines.KmF,
                Origin = pObjRoutes.Origen,
                MOrigin = pObjRoutes.TR_TOWNORIG,
                Destination = pObjRoutes.Destino,
                MDestination = pObjRoutes.TR_TOWNDES
            };
        }

        private void SetInternalConfig()
        {
            lblArticle.Caption = "Folio interno";
            txtInternal.Item.Visible = true;
            cboShared.Item.Click();
            txtArticle.Item.Visible = false;


            lblDescription.Caption = "Estatus";
            txtDesc.Item.Visible = false;
            txtStatus.Item.Visible = true;

            cboArea.Item.Visible = true;
            lblArea.Item.Visible = true;
            txtComment.Item.Visible = true;
            lblComment.Item.Visible = true;

            btnCancel.Item.Visible = true;

            this.UIAPIRawForm.EnableMenu("520", true); // Print
            this.UIAPIRawForm.EnableMenu("6659", false);  // Fax
            this.UIAPIRawForm.EnableMenu("1281", true); // Search Record
            this.UIAPIRawForm.EnableMenu("1282", true); // Add New Record
            this.UIAPIRawForm.EnableMenu("1288", true);  // Next Record
            this.UIAPIRawForm.EnableMenu("1289", true);  // Pevious Record
            this.UIAPIRawForm.EnableMenu("1290", true);  // First Record
            this.UIAPIRawForm.EnableMenu("1291", true);  // Last record

            SetInitialInternalConfig();
        }

        private void SetInitialInternalConfig(string pStrFolio = "")
        {
            if (string.IsNullOrEmpty(pStrFolio))
            {
                txtInternal.Value = mObjTransportsFactory.GetRouteService().GetFolio(pStrFolio);
            }

            txtStatus.Value = mObjTransportsFactory.GetRouteService().GetStatus(txtInternal.Value);

            if (txtStatus.Value.Equals(StatusEnum.CLOSED.GetDescription()) || txtStatus.Value.Equals(StatusEnum.CANCELED.GetDescription()))
            {
                this.UIAPIRawForm.Mode = BoFormMode.fm_VIEW_MODE;
            }
            else
            {
                CreationMode();
            }

            btnCancel.Item.Enabled = txtStatus.Value.Equals("Cerrado") ? true : false;
            lnkJournal.Item.Visible = txtStatus.Value.Equals("Abierto") ? false : true;
            lnkCancel.Item.Visible = txtStatus.Value.Equals("Cancelado") ? true : false;
            btnExit.Item.Enabled = true;
        }

        private void CreationMode()
        {
            txtKmA.Value = string.Empty;
            txtPriceA.Value = string.Empty;
            txtPriceB.Value = string.Empty;
            txtPriceC.Value = string.Empty;
            txtPriceD.Value = string.Empty;
            txtPriceE.Value = string.Empty;
            txtPriceF.Value = string.Empty;
            txtDriver.Value = string.Empty;
            txtTotKm.Value = string.Empty;
            txtAmountKm.Value = string.Empty;
            txtExtra.Value = string.Empty;
            txtAmountEns.Value = string.Empty;
            txtRetentions.Value = string.Empty;
            txtTax.Value = string.Empty;
            txtTotalFreight.Value = string.Empty;
            txtVarios.Value = string.Empty;
            txtKg.Value = string.Empty;
            txtFolio.Value = string.Empty;
            txtKg.Value = string.Empty;
            txtVarios.Value = string.Empty;

            cboShared.Item.Enabled = true;
            txtFolio.Item.Enabled = true;
            txtFolio.Item.Click();
            cboPayload.Item.Enabled = true;
            cboVehicleType.Item.Enabled = true;
            txtEcNum.Item.Enabled = true;
            txtOrigin.Item.Enabled = true;
            txtDestination.Item.Enabled = true;
            txtMorign.Item.Enabled = true;
            txtMDest.Item.Enabled = true;
            txtKmA.Item.Enabled = true;
            txtKmB.Item.Enabled = true;
            txtKmC.Item.Enabled = true;
            txtKmD.Item.Enabled = true;
            txtKmE.Item.Enabled = true;
            txtKmF.Item.Enabled = true;
            txtDriver.Item.Enabled = true;
            txtExtra.Item.Enabled = true;
            chkEnsm.Item.Enabled = true;
            btnRoute.Item.Enabled = true;
            btnAccept.Item.Enabled = true;
            txtKg.Item.Enabled = true;
            txtVarios.Item.Enabled = true;

            if (mBoolInternal)
            {
                txtInternal.Item.Enabled = false;
                cboArea.Item.Enabled = true;
                txtComment.Item.Enabled = true;
            }

        }

        private void InitializeForm()
        {
            this.UIAPIRawForm.Freeze(true);
            txtVarios.Item.Visible = false;
            lblVarios.Item.Visible = false;
            LoadEvents();
            LoadCombos();
            mObjInsurance = mObjTransportsFactory.GetRouteService().GetInsuranceLine(mIntUserSignature);
            this.UIAPIRawForm.Freeze(false);
        }

        private void LoadCombos()
        {
            try
            {
                ///Combo Compartido
                List<SharedEnum> lLstSharedEnum = Enum.GetValues(typeof(SharedEnum)).Cast<SharedEnum>().ToList();
                cboShared.ValidValues.Add("", "");
                foreach (SharedEnum lObjSharedEnum in lLstSharedEnum)
                {
                    cboShared.ValidValues.Add(((int)lObjSharedEnum).ToString(), lObjSharedEnum.GetDescription());
                }
                //Combo Vehiculos
                mLstVehicles = mObjTransportsFactory.GetVehiclesService().GetVehiclesTypeList();
                cboVehicleType.ValidValues.Add("", "");
                foreach (var item in mLstVehicles)
                {
                    cboVehicleType.ValidValues.Add(item.EquipType, item.Name);
                }
                //Combo Tipo de carga
                List<PayLoadTypeDTO> lLstPayloadTypes = mObjTransportsFactory.GetVehiclesService().GetPayloadTypeList();
                cboPayload.ValidValues.Add("", "");
                foreach (var item in lLstPayloadTypes)
                {
                    cboPayload.ValidValues.Add(item.Code, item.Name);
                }

                if (mBoolInternal)
                {
                    List<CostingCodesDTO> lLstCostingCodes = mObjTransportsFactory.GetRouteService().GetCostingCodes();

                    cboArea.ValidValues.Add("", "");
                    foreach (var item in lLstCostingCodes)
                    {
                        cboArea.ValidValues.Add(item.Code, item.Name);
                    }
                }

                cboVehicleType.Item.DisplayDesc = true;
                cboShared.Item.DisplayDesc = true;
                cboPayload.Item.DisplayDesc = true;
            }
            catch (Exception ex)
            {
                LogService.WriteError("(LoadCombobox): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void SetRoutesTextsBoxes()
        {
            this.UIAPIRawForm.Freeze(true);
            if (mObjRoutes != null)
            {
                txtOrigin.Value = mObjRoutes.Origin;

                txtMorign.Value = mObjRoutes.MOrigin;

                txtMDest.Value = mObjRoutes.MDestination;

                txtDestination.Value = mObjRoutes.Destination;

                txtKmA.Value = mObjRoutes.KmA;
                txtKmB.Value = mObjRoutes.KmB;
                txtKmC.Value = mObjRoutes.KmC;
                txtKmD.Value = mObjRoutes.KmD;
                txtKmE.Value = mObjRoutes.KmE;
                txtKmF.Value = mObjRoutes.KmF;
                LoadConfig();
            }
            else
            {
                txtOrigin.Value = string.Empty;

                txtMorign.Value = string.Empty;

                txtMDest.Value = string.Empty;

                txtDestination.Value = string.Empty;

                txtKmA.Value = string.Empty;
                txtKmB.Value = string.Empty;
                txtKmC.Value = string.Empty;
                txtKmD.Value = string.Empty;
                txtKmE.Value = string.Empty;
                txtKmF.Value = string.Empty;
            }
            this.UIAPIRawForm.Freeze(false);
        }

        private void LoadConfig()
        {
            btnUpdate.Caption = "Actualizar";
            btnUpdate.Item.Enabled = false;
        }

        private void SetTextBox()
        {
           
            string ss = mStrTxtCFL;
            switch (mStrTxtCFL)
            {
                case "txtMOrig":
                    txtMorign.Value = ((SAPbouiCOM.EditText)mModalFrmCFL.pObjMtxCFL.Columns.Item("cName")
                        .Cells.Item(mModalFrmCFL.mIntRow).Specific).Value.ToString();
                    break;
                case "txtMDest":
                    txtMDest.Value = ((SAPbouiCOM.EditText)mModalFrmCFL.pObjMtxCFL.Columns.Item("cName")
                        .Cells.Item(mModalFrmCFL.mIntRow).Specific).Value.ToString();
                    break;
                case "txtArt":
                    txtArticle.Value = ((SAPbouiCOM.EditText)mModalFrmCFL.pObjMtxCFL.Columns.Item("cName")
                        .Cells.Item(mModalFrmCFL.mIntRow).Specific).Value.ToString();
                    txtDesc.Value = ((SAPbouiCOM.EditText)mModalFrmCFL.pObjMtxCFL.Columns.Item("cDesc")
                        .Cells.Item(mModalFrmCFL.mIntRow).Specific).Value.ToString();

                    string lStrValidate = mObjTransportsFactory.GetRouteService().ValidateSinKM(txtArticle.Value);

                    if (lStrValidate == "Y")
                    {
                        txtAmountKm.Item.Enabled = true;
                    }
                    else
                    {
                        txtAmountKm.Item.Enabled = false;
                    }

                    break;
                case "txtEcNum":
                    txtEcNum.Value = ((SAPbouiCOM.EditText)mModalFrmCFL.pObjMtxCFL.Columns.Item("cItem")
                        .Cells.Item(mModalFrmCFL.mIntRow).Specific).Value.ToString();
                    lblAFDesc.Caption = ((SAPbouiCOM.EditText)mModalFrmCFL.pObjMtxCFL.Columns.Item("cFrgN")
                        .Cells.Item(mModalFrmCFL.mIntRow).Specific).Value.ToString();

                    break;
                case "txtDriv":
                    mStrDriverId = ((SAPbouiCOM.EditText)mModalFrmCFL.pObjMtxCFL.Columns.Item("cType").Cells.Item(mModalFrmCFL.mIntRow).Specific).Value.ToString();
                    txtDriver.Value = ((SAPbouiCOM.EditText)mModalFrmCFL.pObjMtxCFL.Columns.Item("cName")
                        .Cells.Item(mModalFrmCFL.mIntRow).Specific).Value.ToString();
                    break;
                case "txtFol":
                    txtFolio.Value = ((SAPbouiCOM.EditText)mModalFrmCFL.pObjMtxCFL.Columns.Item("cFolio")
                        .Cells.Item(mModalFrmCFL.mIntRow).Specific).Value.ToString();

                     string lStrName = ((SAPbouiCOM.EditText)mModalFrmCFL.pObjMtxCFL.Columns.Item("cDocNum")
                        .Cells.Item(mModalFrmCFL.mIntRow).Specific).Value.ToString();

                     string lStrStatus = ((SAPbouiCOM.EditText)mModalFrmCFL.pObjMtxCFL.Columns.Item("cStat")
                         .Cells.Item(mModalFrmCFL.mIntRow).Specific).Value.ToString();


                     if (!string.IsNullOrEmpty(lStrStatus))
                     {
                         SalesOrderLinesDTO lObjSOLinesDTO = mObjTransportsFactory.GetCommissionService().SalesOrdeLines(lStrName, txtFolio.Value);
                         mObjSalesOrderLines = lObjSOLinesDTO;
                         LoadLine(true);
                     }
                     else
                     {
                         LoadInternalFreight(lStrName, true);
                     }
                     KmSetZero();
                     txtEcNum.Item.Click();
                     KmEnableDisable(false);
                    break;
            }
        }

        private void KmSetZero()
        {
            txtKmA.Value = "0";
            txtKmB.Value = "0";
            txtKmC.Value = "0";
            txtKmD.Value = "0";
            txtKmE.Value = "0";
            txtKmF.Value = "0";
            txtTotKm.Value = "0";
        }

        private void KmEnableDisable(bool pBolEnable)
        {
            txtKmA.Item.Enabled = pBolEnable;
            txtKmB.Item.Enabled = pBolEnable;
            txtKmC.Item.Enabled = pBolEnable;
            txtKmD.Item.Enabled = pBolEnable;
            txtKmE.Item.Enabled = pBolEnable;
            txtKmF.Item.Enabled = pBolEnable;
        }


        private void OpenCFLModal(string pStrModal, string pStrCFL, string pStrEquip)
        {
            SetModalParameters(pStrModal, pStrCFL, pStrEquip);
            //mModalFrmCFL = new ModalForms.mfrmCFL(pStrModal, pStrCFL, mIntUserSignature, pStrEquip);
            mModalFrmCFL = new ModalForms.mfrmCFL(mObjCFLParameters);
            CFL_Btn = mModalFrmCFL.pObjBtnSelect;
            CFL_Mtx = mModalFrmCFL.pObjMtxCFL;
        }

        private void SetModalParameters(string pStrModal, string pStrCFL, string pStrEquip)
        {
            mObjCFLParameters = new CFLParamsDTO();

            mObjCFLParameters.FormName = pStrModal;
            mObjCFLParameters.CFLType = pStrCFL;
            mObjCFLParameters.UserSign = mIntUserSignature;
            mObjCFLParameters.Equip = pStrEquip;
            mObjCFLParameters.CardCode = mStrCardCode;

        }

        private void ShowMfRouteFinder()
        {
            mModalFrmRouteFinder = new ModalForms.mfrmRouteFinder("frmMRouteFinder");
            CFL_Btn = mModalFrmRouteFinder.pObjBtnSelect;
            CFL_Mtx = mModalFrmRouteFinder.pObjMtxRoutes;
        }

        private void LoadVehiclePrices(bool pBoolValid)
        {
            VehiclesDTO lObjVehicle = mLstVehicles.Select(x => x).Where(x => x.EquipType == cboVehicleType.Value).FirstOrDefault();

            txtPriceA.Value = pBoolValid ? lObjVehicle.PathA : string.Empty;
            txtPriceB.Value = pBoolValid ? lObjVehicle.PathB : string.Empty;
            txtPriceC.Value = pBoolValid ? lObjVehicle.PathC : string.Empty;
            txtPriceD.Value = pBoolValid ? lObjVehicle.PathD : string.Empty;
            txtPriceE.Value = pBoolValid ? lObjVehicle.PathE : string.Empty;
            txtPriceF.Value = pBoolValid ? lObjVehicle.PathF : string.Empty;
        }

        private void SetKmAmounts()
        {
            this.UIAPIRawForm.Freeze(true);
            SetKilometers();
           
                SetKilometerAmount();
            
            this.UIAPIRawForm.Freeze(false);
        }

        private void SetCashAmounts()
        {
            this.UIAPIRawForm.Freeze(true);
            if (txtTotalFreight.Item.Enabled)
            {
                if (float.Parse(txtAmountEns.Value) != 0)
                {
                    LoadInsuranceAmount(true);
                }
               
              
            } 
            SetRetentions();
            SetTotalAmount();
            this.UIAPIRawForm.Freeze(false);
        }

        private void SetKilometers()
        {
            mFlKm = float.Parse(txtKmA.Value) + float.Parse(txtKmB.Value) + float.Parse(txtKmC.Value) + float.Parse(txtKmD.Value) + float.Parse(txtKmE.Value) + float.Parse(txtKmF.Value);
            txtTotKm.Value = mFlKm.ToString();
        }

        private void SetKilometerAmount()
        {
            float lFlA = float.Parse(txtKmA.Value) * float.Parse(txtPriceA.Value);
            float lFlB = float.Parse(txtKmB.Value) * float.Parse(txtPriceB.Value);
            float lFlC = float.Parse(txtKmC.Value) * float.Parse(txtPriceC.Value);
            float lFlD = float.Parse(txtKmD.Value) * float.Parse(txtPriceD.Value);
            float lFlE = float.Parse(txtKmE.Value) * float.Parse(txtPriceE.Value);
            float lFlF = float.Parse(txtKmF.Value) * float.Parse(txtPriceF.Value);
            if ((lFlA + lFlB + lFlC + lFlD + lFlE + lFlF) > 0)
            {
                txtAmountKm.Value = (lFlA + lFlB + lFlC + lFlD + lFlE + lFlF).ToString();
            }
        }

        
        private void SetRetentions()
        {
           
            txtRetentions.Value = Math.Round(((float.Parse(txtAmountKm.Value) + float.Parse(txtExtra.Value)) * (mFlRetention != 0 ? mFlRetention : 0)),2).ToString();
            txtTax.Value = Math.Round(((float.Parse(txtAmountKm.Value) + float.Parse(txtExtra.Value)) * (mFlTax != 0 ? mFlTax/100 : 0)),2).ToString();
        }

        private void SetTotalAmount()
        {
           
            txtTotalFreight.Value = (float.Parse(txtAmountKm.Value) + float.Parse(txtExtra.Value)
                 + float.Parse(txtAmountEns.Value) + float.Parse(txtTax.Value) - float.Parse(txtRetentions.Value)).ToString();
        }

        private void SharedConfig(bool pBoolShared)
        {
            txtAmountKm.Item.Enabled = pBoolShared;
            txtAmountEns.Item.Enabled = pBoolShared;
            KmEnableDisable(true);
        }

        private void LoadInsuranceAmount(bool pBoolCheck)
        {
            this.UIAPIRawForm.Freeze(true);
            txtAmountEns.Value = pBoolCheck ? (mObjInsurance.Price * mFlKm).ToString() : "0";
            //SetCashAmounts();
            this.UIAPIRawForm.Freeze(false);
        }

        private void NewRoute()
        {
            if (ValidateRouteFields())
            {
                mIntActualRoute = 0;
                Routes lObjRoute = GetRouteObject();
                int lIntResult = mObjTransportsFactory.GetRouteService().AddRoute(lObjRoute);
                if (lIntResult == 0)
                {
                    mIntActualRoute = mObjTransportsFactory.GetRouteService().GetNextRouteId() - 1;
                    lObjRoute.RowCode = mIntActualRoute.ToString();
                    mObjRoutes = RoutesToDTO(lObjRoute); 
                    SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText("Ruta añadida correctamente"
                    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    LoadConfig();
                }
                else
                {
                    SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText("Error: " + DIApplication.Company.GetLastErrorDescription()
                    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    LogService.WriteError(DIApplication.Company.GetLastErrorDescription());
                }
            }
        }

        private void UpdateRoute()
        {
            if (ValidateRouteFields())
            {
                int lIntResult = mObjTransportsFactory.GetRouteService().UpdateRoute(GetRouteObject());
                if (lIntResult == 0)
                {

                    SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText("Ruta actualizada correctamente"
                    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    LoadConfig();
                }
                else
                {
                    SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText("Error: " + DIApplication.Company.GetLastErrorDescription()
                    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    LogService.WriteError(DIApplication.Company.GetLastErrorDescription());
                }
            }
        }

        private List<SalesOrderLinesDTO> GetSalesOrderLines()
        {
            List<SalesOrderLinesDTO> lLstSalesOrderLines = new List<SalesOrderLinesDTO>();
            lLstSalesOrderLines.Add(new SalesOrderLinesDTO()
            {
                ItemCode = txtArticle.Value,
                Quantity = 1,
                UnitPrice = (float.Parse(txtAmountKm.Value)) + (float.Parse(txtExtra.Value)), //  (float.Parse(lObjTxtTotalFreight.Value)-float.Parse(lObjTxtAmountEns.Value)),
                PayloadType = cboPayload.Value,
                VehicleType = cboVehicleType.Value,
                Route = mIntActualRoute,
                TotKm = txtTotKm.Value,
                Folio = txtFolio.Value,
                KmA = txtKmA.Value,
                KmB = txtKmB.Value,
                KmC = txtKmC.Value,
                KmD = txtKmD.Value,
                KmE = txtKmE.Value,
                KmF = txtKmF.Value,
                Employee = txtDriver.Value,
                Asset = txtEcNum.Value,
                Extra = txtExtra.Value,
                Origin = txtOrigin.Value,
                Destination = txtDestination.Value,
                AnotherPyld = mStrType == "O" ? txtVarios.Value : "",
                Bags = mStrType == "A" ? txtVarios.Value : "",
                Heads = mStrType == "G" ? txtVarios.Value : "",
                TotKg = txtKg.Value,
                TaxWT = float.Parse(txtRetentions.Value),
                Shared = cboShared.Value == "2" ? true : false,
                AssetDesc = lblAFDesc.Caption,

            });

            if (chkEnsm.Checked)
            {
                lLstSalesOrderLines.Add(new SalesOrderLinesDTO()
                {
                    ItemCode = mObjInsurance.ItemCode,
                    Quantity = 1,
                    UnitPrice = float.Parse(txtAmountEns.Value),
                    Asset = txtEcNum.Value,
                    AnotherPyld = mStrType == "O" ? txtVarios.Value : "",
                    Bags = mStrType == "A" ? txtVarios.Value : "",
                    Heads = mStrType == "G" ? txtVarios.Value : "",
                    TotKg = txtKg.Value,
                    Route = mIntActualRoute,
                    TotKm = txtTotKm.Value,
                });
            }
            return lLstSalesOrderLines;
        }

        private bool SetSalesOrder()
        {
            try
            {

                if (SetSalesOrderControls())
                {
                    List<SalesOrderLinesDTO> lLstSalesOrderLines = GetSalesOrderLines();
                    FillSalesOrder(lLstSalesOrderLines);
                }
                return true;
            }
            catch (Exception ex)
            {
               
                LogService.WriteError("(OpenFormBP): " + ex.Message);
                LogService.WriteError(ex);
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(ex.Message);
                return false;
            }
            finally
            {
                if (SO_FrmSalesOrder != null)
                {
                    SO_FrmSalesOrder.Freeze(false);
                }
            }
        }

        private bool SetSalesOrderControls()
        {
            try
            {
                SO_FrmSalesOrder = SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetFormByTypeAndCount(mIntFormType, -1);
                SO_FrmFields = SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetFormByTypeAndCount(-mIntFormType, -1);

                SO_TxtFolio = (SAPbouiCOM.EditText)SO_FrmFields.Items.Item("U_GLO_Ticket").Specific;
                SO_TxtShared = (SAPbouiCOM.EditText)SO_FrmFields.Items.Item("U_TR_Shared").Specific;

                SO_MtxSO = (SAPbouiCOM.Matrix)SO_FrmSalesOrder.Items.Item("38").Specific;
                return true;
            }
            catch (Exception lObjException)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Favor de abrir campos definidos de usuario");
                LogService.WriteError("(OpenFormBP): " + lObjException.Message);
                LogService.WriteError(lObjException.Message + "-" + lObjException.InnerException.Message);
                return false;
            }
        }

        private void SetTableIR()
        {
            try
            {
                Form lFrmInv = SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetFormByTypeAndCount(mIntFormType, -1);
                var ss = lFrmInv.GetAsXML();
                lFrmInv.Menu.Item("5897").Activate();
                Form lFrmIR = UIApplication.GetApplication().Forms.ActiveForm;

                lFrmIR.Visible = false;
                lFrmIR.Freeze(true);
                //lFrmIR.Freeze(true);
                var lFrmIsR = UIApplication.GetApplication().Forms.ActiveForm.GetAsXML();
                Matrix lMtxTable = (Matrix)lFrmIR.Items.Item("6").Specific;
                Column lCol = (Column)lMtxTable.Columns.Item(3);
                //string lCol = lCol
                EditText lTxtTax = (EditText)lMtxTable.Columns.Item(3).Cells.Item(1).Specific;

                lTxtTax.Value = "FV";
                Button lBtnOk = (Button)lFrmIR.Items.Item(0).Specific;
                lFrmIR.Freeze(false);
                lBtnOk.Item.Click();
                lBtnOk.Item.Click();
            }
            catch (Exception ex)
            {
                //Ignore :)
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void FillSalesOrder(List<SalesOrderLinesDTO> pLstSalesOrderLines)
        {
            SO_FrmSalesOrder.Freeze(true);


            for (int i = 1; i <= pLstSalesOrderLines.Count; i++)
            {
                SO_TxtItem = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("1").Cells.Item(i).Specific;
                SO_TxtItem.Value = pLstSalesOrderLines[i - 1].ItemCode;

                SO_CboPyloadType = (SAPbouiCOM.ComboBox)SO_MtxSO.Columns.Item("U_TR_LoadType").Cells.Item(i).Specific;
                SO_CboPyloadType.Select(pLstSalesOrderLines[i - 1].PayloadType, SAPbouiCOM.BoSearchKey.psk_ByValue);

                SO_CboVehicleType = (SAPbouiCOM.ComboBox)SO_MtxSO.Columns.Item("U_TR_VehicleType").Cells.Item(i).Specific;
                SO_CboVehicleType.Select(pLstSalesOrderLines[i - 1].VehicleType, SAPbouiCOM.BoSearchKey.psk_ByValue);

                SO_TxtRoute = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("U_TR_Paths").Cells.Item(i).Specific;
                SO_TxtRoute.Value = pLstSalesOrderLines[i - 1].Route.ToString();

                txtTotKm = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("U_TR_TotKm").Cells.Item(i).Specific;
                txtTotKm.Value = pLstSalesOrderLines[i - 1].TotKm;

                SO_TxtKmA = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("U_TR_TypeA").Cells.Item(i).Specific;
                SO_TxtKmA.Value = pLstSalesOrderLines[i - 1].KmA;

                SO_TxtKmB = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("U_TR_TypeB").Cells.Item(i).Specific;
                SO_TxtKmB.Value = pLstSalesOrderLines[i - 1].KmB;

                SO_TxtKmC = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("U_TR_TypeC").Cells.Item(i).Specific;
                SO_TxtKmC.Value = pLstSalesOrderLines[i - 1].KmC;

                SO_TxtKmD = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("U_TR_TypeD").Cells.Item(i).Specific;
                SO_TxtKmD.Value = pLstSalesOrderLines[i - 1].KmD;

                SO_TxtKmE = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("U_TR_TypeE").Cells.Item(i).Specific;
                SO_TxtKmE.Value = pLstSalesOrderLines[i - 1].KmE;

                SO_TxtKmF = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("U_TR_TypeF").Cells.Item(i).Specific;
                SO_TxtKmF.Value = pLstSalesOrderLines[i - 1].KmF;

               
                SO_TxtExtra = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("U_TR_AdditionalExpen").Cells.Item(i).Specific;
                SO_TxtExtra.Value = pLstSalesOrderLines[i - 1].Extra;

                SO_TxtDestination = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("U_TR_Destination").Cells.Item(i).Specific;
                SO_TxtDestination.Value = pLstSalesOrderLines[i - 1].Origin + " - "+ pLstSalesOrderLines[i - 1].Destination;

               

                if (!string.IsNullOrEmpty(pLstSalesOrderLines[i - 1].Employee))
                {
                    SO_CboEmployee = (SAPbouiCOM.ComboBox)SO_MtxSO.Columns.Item("27").Cells.Item(i).Specific;
                    SO_CboEmployee.Select(pLstSalesOrderLines[i - 1].Employee, SAPbouiCOM.BoSearchKey.psk_ByDescription);
                }

                SO_TxtoAF = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("2003").Cells.Item(i).Specific;
                SO_TxtoAF.Value = pLstSalesOrderLines[i - 1].Asset;

                SO_TxtQtty = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("11").Cells.Item(i).Specific;
                SO_TxtQtty.Value = pLstSalesOrderLines[i - 1].Quantity.ToString();

                SO_TxtPrice = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("14").Cells.Item(i).Specific;
                SO_TxtPrice.Value = pLstSalesOrderLines[i - 1].UnitPrice.ToString();

                SO_TxtTotKG = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("U_TR_TotKilos").Cells.Item(i).Specific;
                SO_TxtTotKG.Value = pLstSalesOrderLines[i - 1].TotKg;


                SO_TxtHeads = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("U_TR_Heads").Cells.Item(i).Specific;
                SO_TxtHeads.Value = pLstSalesOrderLines[i - 1].Heads;

                SO_TxtSacos = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("U_GLO_BagsBales").Cells.Item(i).Specific;
                SO_TxtSacos.Value = pLstSalesOrderLines[i - 1].Bags;

                SO_TxtVarios = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("U_TR_OtherLoad").Cells.Item(i).Specific;
                SO_TxtVarios.Value = pLstSalesOrderLines[i - 1].AnotherPyld;

                SO_TxtAFDesc = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("U_MQ_AFDesc").Cells.Item(i).Specific;
                SO_TxtAFDesc.Value = pLstSalesOrderLines[i - 1].AssetDesc;

                SO_TxtKilometers = (SAPbouiCOM.EditText)SO_MtxSO.Columns.Item("U_MQ_Kilometers").Cells.Item(i).Specific;
                SO_TxtKilometers.Value = pLstSalesOrderLines[i - 1].TotKm;

                if (pLstSalesOrderLines[i - 1].TaxWT > 0)
                {
                    mBolWT = true;
                }

                SO_TxtShared.Value = pLstSalesOrderLines[i - 1].Shared == true ? "Y" : "N";
            }

 
            if (SO_TxtFolio != null && SO_TxtFolio.Item.Enabled)
            {
               
                SO_TxtFolio.Value = pLstSalesOrderLines.Select(x => x.Folio).FirstOrDefault();
            }
            else
            {
                UIApplication.ShowMessageBox("el campo U_GLO_Ticket no esta activo");
            }
            if (mBoolFormLoaded && SO_MtxSO.RowCount > 2)
            {
                if (!chkEnsm.Checked)
                {
                    SO_MtxSO.DeleteRow(2);
                }
            }



          
            SO_FrmSalesOrder.Freeze(false);
        }

        private Routes GetRouteObject()
        {
            Routes lObjRoute = new Routes();

            lObjRoute.RowCode = mIntActualRoute.ToString();
            lObjRoute.Activo = "S";
            lObjRoute.TypeA = float.Parse(txtKmA.Value);
            lObjRoute.TypeB = float.Parse(txtKmB.Value);
            lObjRoute.TypeC = float.Parse(txtKmC.Value);
            lObjRoute.TypeD = float.Parse(txtKmD.Value);
            lObjRoute.TypeE = float.Parse(txtKmE.Value);
            lObjRoute.TypeF = float.Parse(txtKmF.Value);
            lObjRoute.Origen = txtOrigin.Value;
            lObjRoute.Destino = txtDestination.Value;
            lObjRoute.TR_TOWNORIG = txtMorign.Value.ToUpper();
            lObjRoute.TR_TOWNDES = txtMDest.Value.ToUpper();
            lObjRoute.CasetaTC = 0;
            lObjRoute.CasetaRB = 0;
            lObjRoute.RowName = GetRouteName();

            return lObjRoute;

        }

        private string GetRouteName()
        {
            string lStrOrgn = txtOrigin.Value.Length >= 4 ? txtOrigin.Value.Substring(0, 4) : txtOrigin.Value.Substring(0, txtOrigin.Value.Length);
            string lStrDest = txtDestination.Value.Length >= 4 ? txtDestination.Value.Substring(0, 4) : txtDestination.Value.Substring(0, txtDestination.Value.Length);
            int lIntNextId = mObjTransportsFactory.GetRouteService().GetNextRouteId();

            return string.Format("{0}-{1}{2}", lStrOrgn, lStrDest, lIntNextId);
        }

        public void CloseForm()
        {
            UnloadEvents();
            this.UIAPIRawForm.Close();
            if (mBolWT)
            {
                SetTableIR();
            }
        }

        public void SelectModal(string pStrItemUID)
        {
            mStrTxtCFL = pStrItemUID;
            switch (pStrItemUID)
            {
                case "txtMOrig":
                    OpenCFLModal("frmMCFL", "CFL_TownsA", "");
                    break;
                case "txtMDest":
                    OpenCFLModal("frmMCFL", "CFL_TownsB", "");
                    break;
                case "txtArt":
                    OpenCFLModal("frmMCFL", "CFL_Items", "");
                    break;
                case "txtEcNum":
                    if (!string.IsNullOrEmpty(cboVehicleType.Value))
                    {
                        OpenCFLModal("frmMCFL", "CFL_AF", cboVehicleType.Value);
                    }
                    break;
                case "txtDriv":
                    OpenCFLModal("frmMCFL", "CFL_DR", "");
                    break;
                case "txtFol":
                    if (!string.IsNullOrEmpty(cboShared.Value) && (SharedEnum)Convert.ToInt32(cboShared.Value) == SharedEnum.Yes)
                    {
                        OpenCFLModal("frmMCFL", "CFL_Folios", "");
                    }
                    break;
            }
        }

        public void SetRoutes()
        {

            if (mObjRoutes != null || mModalFrmRouteFinder.pRoutes != null)
            {
                mObjRoutes = mModalFrmRouteFinder.pRoutes;
                SetRoutesTextsBoxes();
                mModalFrmRouteFinder.CloseForm();
                mModalFrmRouteFinder = null;
                mBoolRoadLoaded = true;
                mIntActualRoute = mObjRoutes != null ? mObjRoutes.Route : 0;
                txtDriver.Item.Click();
            }
            else
            {
                UIApplication.ShowMessageBox("Favor de seleccionar una ruta");
            }
        }

        private bool SaveDetails()
        {
            try
            {
                SAPbobsCOM.UserTable lObjInternalFreight = (SAPbobsCOM.UserTable)DIApplication.Company.UserTables.Item("UG_TR_INTLFRGHT");

                lObjInternalFreight.UserFields.Fields.Item("U_InternalFolio").Value = txtInternal.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_JournalEntryId").Value = DIApplication.Company.GetNewObjectKey().ToString();
                lObjInternalFreight.UserFields.Fields.Item("U_Shared").Value = cboShared.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_Insurance").Value = chkEnsm.Checked ? "Y" : "N";
                lObjInternalFreight.UserFields.Fields.Item("U_Status").Value = StatusEnum.CLOSED.GetDescription();
                lObjInternalFreight.UserFields.Fields.Item("U_Route").Value = mIntActualRoute.ToString();
                lObjInternalFreight.UserFields.Fields.Item("U_VehicleType").Value = cboVehicleType.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_Ticket").Value = txtFolio.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_Comments").Value = string.Format("FI {0}: {1} ", mObjRoutes.RouteName, txtComment.Value);
                lObjInternalFreight.UserFields.Fields.Item("U_Area").Value = cboArea.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_Extra").Value = txtExtra.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_Driver").Value = mStrDriverId;
                lObjInternalFreight.UserFields.Fields.Item("U_DriverName").Value = txtDriver.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_PayloadType").Value = cboPayload.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_Asset").Value = txtEcNum.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_Origin").Value = txtOrigin.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_MOrigin").Value = txtMorign.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_Destination").Value = txtDestination.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_MDestination").Value = txtMDest.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_KmA").Value = txtKmA.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_KmB").Value = txtKmB.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_KmC").Value = txtKmC.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_KmD").Value = txtKmD.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_KmE").Value = txtKmE.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_KmF").Value = txtKmF.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_TotKG").Value = txtKg.Value;
                lObjInternalFreight.UserFields.Fields.Item("U_Heads").Value = mStrType == "G" ? txtVarios.Value: string.Empty;
                lObjInternalFreight.UserFields.Fields.Item("U_Bags").Value = mStrType == "A" ? txtVarios.Value : string.Empty;
                lObjInternalFreight.UserFields.Fields.Item("U_Varios").Value = mStrType == "O" ? txtVarios.Value : string.Empty;
                lObjInternalFreight.UserFields.Fields.Item("U_AmountKM").Value = txtAmountKm.Value;
                

                if (lObjInternalFreight.Add() == 0)
                {
                    return true;
                }
                else
                {
                    string lStrMessage = DIApplication.Company.GetLastErrorDescription();
                    LogService.WriteError("SaveDetails: " + lStrMessage);
                    UIApplication.ShowMessageBox("SaveDetails" + lStrMessage);
                    return false;
                }

            }
            catch (Exception lObjException)
            {    
                LogService.WriteError(lObjException.Message);
                 LogService.WriteError(lObjException);
                SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText("SaveDetails: " + lObjException.Message
                    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
       
            }
                return false;
            


        }

        private bool CreateJournal()
        { string lStrDebitAc = string.Empty;
            List<JournalLineDTO> lLstJurnalEntryLine = new List<JournalLineDTO>();
            List<CostingCodesDTO> lLstCostingCodes = mObjTransportsFactory.GetRouteService().GetCostingCodes().Where(x => x.Code == cboArea.Value).ToList();
            if (lLstCostingCodes.Count > 0)
            {
                lStrDebitAc = lLstCostingCodes.First().Account;
            }

            if (!string.IsNullOrEmpty(lStrDebitAc))
            {
                lLstJurnalEntryLine = GetJournalLines(lStrDebitAc);
            }
            else
            {
                lLstJurnalEntryLine = GetJournalLines();
            }

            return mObjTransportsFactory.GetJournalService().CreateNewJournal(lLstJurnalEntryLine, txtInternal.Value, "TR/F", "Flete interno" + DateTime.Now.ToShortDateString(), DateTime.Now);
        }


        private bool ReverseJournal()
        {
            return mObjTransportsFactory.GetJournalService().ReverseJournal(txtInternal.Value, "TR/F");
        }

        private List<JournalLineDTO> GetJournalLines()
        {
            List<JournalLineDTO> lLstJournalLines = new List<JournalLineDTO>();

            string lStrCreditAccount = mObjTransportsFactory.GetJournalService().GetCreditAccount();
            string lStrDebitAccount = mObjTransportsFactory.GetJournalService().GetDebitAccount();
               
            lLstJournalLines.Add(new JournalLineDTO()
            {
                AccountCode = lStrCreditAccount,
                ContraAccount = lStrDebitAccount,
                CostingCode = mObjTransportsFactory.GetRouteService().GetCostingCode(mIntUserSignature),
                Credit = double.Parse(txtTotalFreight.Value),
                CostingCode2 = txtEcNum.Value,
                Debit = 0,
                Paths = mIntActualRoute.ToString(),
                Auxiliar = mStrDriverId,
                Comment = txtComment.Value,

            });

            lLstJournalLines.Add(new JournalLineDTO()
            {
                AccountCode = lStrDebitAccount,
                ContraAccount = lStrCreditAccount,
                CostingCode = cboArea.Value,
                CostingCode2 = txtEcNum.Value,
                Credit = 0,
                Debit = double.Parse(txtTotalFreight.Value),
                Paths = mIntActualRoute.ToString(),
                Auxiliar = mStrDriverId,
                Comment = txtComment.Value,
            });

            return lLstJournalLines;
        }


        private List<JournalLineDTO> GetJournalLines(string pStrDebitAccount)
        {
            List<JournalLineDTO> lLstJournalLines = new List<JournalLineDTO>();

            string lStrCreditAccount = mObjTransportsFactory.GetJournalService().GetCreditAccount();
           

            lLstJournalLines.Add(new JournalLineDTO()
            {
                AccountCode = lStrCreditAccount,
                ContraAccount = pStrDebitAccount,
                CostingCode = mObjTransportsFactory.GetRouteService().GetCostingCode(mIntUserSignature), 
                CostingCode2 = txtEcNum.Value,
                Debit = 0,
                 Credit = double.Parse(txtTotalFreight.Value),
                Paths = mIntActualRoute.ToString(),
                Auxiliar = mStrDriverId,
                Comment = txtComment.Value,
            });

            lLstJournalLines.Add(new JournalLineDTO()
            {
                AccountCode = pStrDebitAccount,
                ContraAccount = lStrCreditAccount,
                CostingCode = cboArea.Value,
                CostingCode2 = txtEcNum.Value,
                Debit = double.Parse(txtTotalFreight.Value),
                Credit = 0,
                Paths = mIntActualRoute.ToString(),
                Auxiliar = mStrDriverId,
                Comment = txtComment.Value,
            });

            return lLstJournalLines;
        }

        public void SetCFLTextBoxes()
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);
                SetTextBox();
                mModalFrmCFL.CloseForm();
                mModalFrmCFL = null;
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                if (mModalFrmCFL != null)
                {
                    mModalFrmCFL.UnloadEvents();
                    mModalFrmCFL.CloseForm();
                    mModalFrmCFL = null;
                }
                
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
         
        }

        private int GetFirstFolio()
        {
            return mObjTransportsFactory.GetRouteService().GetFirstFolio();
        }

        private int GetLastFolio()
        {
            return mObjTransportsFactory.GetRouteService().GetLastFolio();
        }

        private void SearchMode()
        {
            this.UIAPIRawForm.Mode = BoFormMode.fm_VIEW_MODE;
            txtInternal.Item.Enabled = true;
            txtInternal.Value = "";
            mBoolSearchMode = true;
        }

        private void SearchFolio()
        {
            if (ValidateFolio())
            {
                LoadInternalFreight(txtInternal.Value, false);
                mBoolSearchMode = false;
            }
        }

        private bool ValidateFolio()
        {
            IList<string> lLstmssingFields = new List<string>();

            if (string.IsNullOrEmpty(txtInternal.Value))
            {
                lLstmssingFields.Add("Folio interno");
            }

            if (!mObjTransportsFactory.GetRouteService().FolioExists(txtInternal.Value))
            {
                lLstmssingFields.Add("El Folio ingresado no existe");
            }

            if (lLstmssingFields.Count > 0)
            {
                string message = string.Format("Favor de revisar {0}:\n{1}",
                    (lLstmssingFields.Count == 1 ? "el siguiente campo" : "los siguientes campos"),
                    string.Join("\n", lLstmssingFields.Select(x => string.Format("-{0}", x)).ToArray()));

                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(message, 1, "Ok", "Cancelar", "");

            }
            return lLstmssingFields.Count == 0 ? true : false;

        }

        private bool ValidateRouteFields()
        {
            IList<string> lLstmssingFields = new List<string>();

            if (string.IsNullOrEmpty(txtOrigin.Value))
            {
                lLstmssingFields.Add("Agregue origen");
            }

            if (string.IsNullOrEmpty(txtDestination.Value))
            {
                lLstmssingFields.Add("Agregue destino");
            }

            if (!ValidateTown(txtMorign.Value))
            {
                lLstmssingFields.Add("El municipio origen no existe");
            }

            if (!ValidateTown(txtMDest.Value))
            {
                lLstmssingFields.Add("El municipio destino no existe");
            }

            if (string.IsNullOrEmpty(txtMorign.Value))
            {
                lLstmssingFields.Add("Agregue municipio de origen");
            }

            if (string.IsNullOrEmpty(txtMDest.Value))
            {
                lLstmssingFields.Add("Agregue municipio destino");
            }

            if (lLstmssingFields.Count > 0)
            {
                string message = string.Format("Favor de completar {0}:\n{1}",
                    (lLstmssingFields.Count == 1 ? "el siguiente campo" : "los siguientes campos"),
                    string.Join("\n", lLstmssingFields.Select(x => string.Format("-{0}", x)).ToArray()));

                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(message, 1, "Ok", "Cancelar", "");

            }
            return lLstmssingFields.Count == 0 ? true : false;

        }

        private bool ValidateTown(string pStrTown)
        {
            return mObjTransportsFactory.GetRouteService().CheckTown(pStrTown);
        }

        private bool ValidateLine()
        {
            IList<string> lLstmssingFields = new List<string>();

            if (!mBoolInternal && string.IsNullOrEmpty(txtArticle.Value))
            {
                lLstmssingFields.Add("Agregue un articulo");
            }

            if (mBoolInternal && string.IsNullOrEmpty(txtInternal.Value))
            {
                lLstmssingFields.Add("Folio Interno");
            }

            if (mBoolInternal && string.IsNullOrEmpty(cboArea.Value))
            {
                lLstmssingFields.Add("Área");
            }

            if (string.IsNullOrEmpty(txtFolio.Value) && mIntFormType != 139)
            {
                lLstmssingFields.Add("Ingrese un folio de papeleta");
            }

            if (string.IsNullOrEmpty(cboShared.Value))
            {
                lLstmssingFields.Add("Seleccione si es compartido");
            }

            if (string.IsNullOrEmpty(cboPayload.Value))
            {
                lLstmssingFields.Add("Seleccione el tipo de carga");
            }

            if (string.IsNullOrEmpty(cboVehicleType.Value))
            {
                lLstmssingFields.Add("Seleccione el tipo de vehiculo");
            }

            if (string.IsNullOrEmpty(txtEcNum.Value) && mIntFormType != 139)
            {
                lLstmssingFields.Add("Agregue el numero economico");
            }

            if (string.IsNullOrEmpty(txtDriver.Value) && mIntFormType != 139)
            {
                lLstmssingFields.Add("Ingrese a un chofer");
            }

            if (string.IsNullOrEmpty(txtOrigin.Value))
            {
                lLstmssingFields.Add("Seleccione o agregue una ruta");
            }

            if (string.IsNullOrEmpty(txtMorign.Value))
            {
                lLstmssingFields.Add("Seleccione un municipio origen");
            }
             if (string.IsNullOrEmpty(txtMDest.Value))
            {
                lLstmssingFields.Add("Seleccione un municipio destino");
            }

            //if (string.IsNullOrEmpty(mStrDriverId))
            //{
            //    lLstmssingFields.Add("Seleccione un chofer");
            //}

            if (lLstmssingFields.Count > 0)
            {
                string message = string.Format("Favor de completar {0}:\n{1}",
                    (lLstmssingFields.Count == 1 ? "el siguiente campo" : "los siguientes campos"),
                    string.Join("\n", lLstmssingFields.Select(x => string.Format("-{0}", x)).ToArray()));

                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(message, 1, "Ok", "Cancelar", "");

            }
            return lLstmssingFields.Count == 0 ? true : false;
        }

        private void ChangeFreightStatus()
        {
            try
            {
                SAPbobsCOM.UserTable lObjInternalFreight = (SAPbobsCOM.UserTable)DIApplication.Company.UserTables.Item("UG_TR_INTLFRGHT");

                lObjInternalFreight.UserFields.Fields.Item("U_Status").Value = StatusEnum.CANCELED.GetDescription();
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
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
                    if (pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                pBoolFreightsModal = false;
                                UnloadEvents();
                                break;
                        }
                    }
                }

                #region CFL
                else if (mModalFrmCFL != null && (FormUID.Equals(mModalFrmCFL.mStrFrmName) && mModalFrmCFL.mIntRow > 0))
                {
                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_CLICK:
                                if (pVal.ItemUID.Equals(mModalFrmCFL.pObjBtnSelect.Item.UniqueID))
                                {
                                    SetCFLTextBoxes();
                                }
                                break;

                            case SAPbouiCOM.BoEventTypes.et_DOUBLE_CLICK:
                                if (pVal.ItemUID.Equals(CFL_Mtx.Item.UniqueID.ToString()))
                                {
                                    SetCFLTextBoxes();
                                }
                                break;
                            case BoEventTypes.et_FORM_CLOSE:
                                mModalFrmCFL.UnloadEvents();
                                break;
                        }
                    }
                }

                #endregion
                #region Routes
                else if (mModalFrmRouteFinder != null
                    && (FormUID.Equals(mModalFrmRouteFinder.mStrFrmName)))
                {
                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_CLICK:

                                if (mModalFrmRouteFinder != null)
                                {
                                    CFL_Btn = mModalFrmRouteFinder.pObjBtnSelect;
                                    if (pVal.ItemUID.Equals(CFL_Btn.Item.UniqueID.ToString()))
                                    {
                                        SetRoutes();
                                    }
                                }
                                break;
                            case BoEventTypes.et_FORM_CLOSE:
                                mModalFrmRouteFinder.UnloadEvents();
                                break;
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                //UIApplication.ShowMessageBox(ex.Message);
            }
         

        }

        private void lObjTxt_Validate(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if ((mBoolRoadLoaded && mObjRoutes != null) && (mIntActualRoute == mObjRoutes.Route))
                {
                    if (pVal.ItemChanged)
                    {
                        btnUpdate.Caption = "Actualizar";
                        btnUpdate.Item.Enabled = true;
                    }
                }
                else
                {
                    if (pVal.ItemChanged)
                    {
                        btnUpdate.Caption = "Nuevo";
                        btnUpdate.Item.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }


        }

        private void lObjTxt_LostFocusAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);
                SAPbouiCOM.EditText lObjTxt = sboObject as SAPbouiCOM.EditText;
                if (string.IsNullOrEmpty(lObjTxt.Value))
                {
                    lObjTxt.Value = "0";
                }
                if (!txtAmountKm.Item.Enabled || txtAmountKm.Value == "0.0")
                {
                    SetKmAmounts();
                }
                SetCashAmounts();
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
            }
        }

        private void lObjTxtTotKM_LostAfter(object sboObject, SBOItemEventArg pVal)
        {

            try
            {
                SetCashAmounts();
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
            }
        }

        private void lObjBtnCancel_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                if (btnCancel.Item.Enabled)
                {
                    if (SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("¿Desea cancelar el movimiento?", 2, "Si", "No", "") == 1)
                    {
                        if (ReverseJournal())
                        {
                            ChangeFreightStatus();
                            LoadInternalFreight(txtInternal.Value, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
        }

        private void lObjCboShared_ComboSelectAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                if (!string.IsNullOrEmpty(cboShared.Value) && (SharedEnum)Convert.ToInt32(cboShared.Value) == SharedEnum.Yes)
                {
                    SharedConfig(true);
                }
                else
                {
                    SharedConfig(false);
                    string lStrValidate = mObjTransportsFactory.GetRouteService().ValidateSinKM(txtArticle.Value);

                    if (lStrValidate == "Y")
                    {
                        txtAmountKm.Item.Enabled = true;
                    }
                    else
                    {
                        txtAmountKm.Item.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
        }
        
        private void lObjTxt_KeyDownAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                if (pVal.CharPressed == 13)
                {
                    if (!mBoolSearchMode)
                    {
                        SelectModal(pVal.ItemUID);
                    }
                    else
                    {
                        SearchFolio();
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
        }

     

        private void lObjCboVehicleType_ComboSelectAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                if (!string.IsNullOrEmpty(cboVehicleType.Value))
                {
                    txtEcNum.Value = string.Empty;
                    LoadVehiclePrices(true);
                    SetKmAmounts();
                    SetCashAmounts();
                }
                else
                {
                    LoadVehiclePrices(false);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
        }

        private void lObjBtnRoute_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (btnRoute.Item.Enabled)
                {
                    ShowMfRouteFinder();
                }
            }
            catch (Exception ex)
            {
                 
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText(ex.Message
                                      , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
        }

        private void lObjBtnUpdate_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (btnUpdate.Item.Enabled)
                {
                    SetKmAmounts();
                    SetCashAmounts();

                    switch (btnUpdate.Caption)
                    {
                        case "Nuevo":
                            NewRoute();
                            break;
                        case "Actualizar":
                            UpdateRoute();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
        }

        private void chkEnsm_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
         
            
        }

        private void chkEnsm_PressedBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (chkEnsm.Item.Enabled)
                {
                    if (chkEnsm.Checked)
                    {
                        LoadInsuranceAmount(true);
                        SetCashAmounts();
                    }
                    else
                    {
                        LoadInsuranceAmount(false);
                        SetCashAmounts();
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
        }


        private void lObjBtnAccept_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
         
        }

        private void SaveJournalEntry()
        {
            bool lBolSuccess = false;
            try
            {
                SetKmAmounts();
                SetCashAmounts();

                DIApplication.Company.StartTransaction();
                if (CreateJournal())
                {
                    lBolSuccess = SaveDetails();
                }
            }
            catch (Exception ex)
            {
                lBolSuccess = false;
                LogService.WriteError("(LoadCombobox): " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
            finally
            {
                try
                {
                    if (lBolSuccess)
                    {
                        DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        UIApplication.ShowMessageBox(string.Format("Proceso realizado correctamente"));
                        LoadInternalFreight(string.Empty, false);
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
                    UIApplication.ShowMessageBox(ex.Message);
                    LogService.WriteError("(btnSave_ClickBefore): " + ex.Message);
                    LogService.WriteError(ex);
                }
            }
        }

        private void lObjBtnAccept_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                if (btnAccept.Item.Enabled && ValidateLine())
                {
                    if (!mBoolInternal)
                    {
                        if (txtTotalFreight.Item.Enabled)
                        {
                            SetKmAmounts();
                            SetCashAmounts();
                        }
                        if (SetSalesOrder())
                        {
                            CloseForm();
                        }
                    }
                    else
                    {
                        SaveJournalEntry();

                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("(btnSave_ClickBefore): " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
            
        }

        private void lObjBtnExit_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                if (btnExit.Item.Enabled)
                {
                    CloseForm();
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
        }

        private void frmFreights_MenuEvent(ref MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (mBoolInternal)
                {
                    int lIntFolio;
                    if (!pVal.BeforeAction && UIApplication.GetApplication().Forms.ActiveForm.UniqueID == this.UIAPIRawForm.UniqueID)
                    {
                        switch (pVal.MenuUID)
                        {
                            case "1281": //SearchRecord
                                SearchMode();
                                break;

                            case "1282": //New Record
                                LoadInternalFreight(string.Empty, false);
                                break;

                            case "1288": //Next Record
                                lIntFolio = Convert.ToInt32(txtInternal.Value) + 1;
                                if (lIntFolio <= GetLastFolio())
                                {
                                    LoadInternalFreight(lIntFolio.ToString(), false);
                                }
                                else
                                {
                                    LoadInternalFreight(GetFirstFolio().ToString(), false);
                                }
                                break;

                            case "1289"://Previous Record
                                lIntFolio = Convert.ToInt32(txtInternal.Value) - 1;
                                if (lIntFolio >= GetFirstFolio())
                                {
                                    LoadInternalFreight(lIntFolio.ToString(), false);
                                }
                                else
                                {
                                    LoadInternalFreight(GetLastFolio().ToString(), false);
                                }
                                break;

                            case "1290"://First Record
                                LoadInternalFreight(GetFirstFolio().ToString(), false);
                                break;

                            case "1291": //Last Record
                                LoadInternalFreight(GetLastFolio().ToString(), false);
                                break;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText(ex.Message
                                                    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                this.UIAPIRawForm.Close();
            }
        }

        private void lObjTxtArticle_LostFocusAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtArticle.Value))
                {
                    mFlRetention = mObjTransportsFactory.GetRouteService().GetTaxWT(txtArticle.Value);
                    mFlTax = mObjTransportsFactory.GetRouteService().GetTax(txtArticle.Value);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
        }


        //public void Accept()
        //{
        //    GetSalesOrderLines();
        //    OpenSalesOrder();
        //}
        #endregion

        #region InitializeComponents
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.txtArticle = ((SAPbouiCOM.EditText)(this.GetItem("txtArt").Specific));
            this.txtDesc = ((SAPbouiCOM.EditText)(this.GetItem("txtDesc").Specific));
            this.cboShared = ((SAPbouiCOM.ComboBox)(this.GetItem("cboShrd").Specific));
            this.lblArticle = ((SAPbouiCOM.StaticText)(this.GetItem("lblArt").Specific));
            this.lblShared = ((SAPbouiCOM.StaticText)(this.GetItem("lblShrd").Specific));
            this.cboVehicleType = ((SAPbouiCOM.ComboBox)(this.GetItem("txtVehT").Specific));
            this.lblVehicleType = ((SAPbouiCOM.StaticText)(this.GetItem("lblVehT").Specific));
            this.txtFolio = ((SAPbouiCOM.EditText)(this.GetItem("txtFol").Specific));
            this.lblFolio = ((SAPbouiCOM.StaticText)(this.GetItem("lblFol").Specific));
            this.lblEcNum = ((SAPbouiCOM.StaticText)(this.GetItem("lblEcNum").Specific));
            this.txtEcNum = ((SAPbouiCOM.EditText)(this.GetItem("txtEcNum").Specific));
            this.cboPayload = ((SAPbouiCOM.ComboBox)(this.GetItem("cboPyld").Specific));
            this.cboPayload.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cboPayload_ComboSelectAfter);
            this.lblPayload = ((SAPbouiCOM.StaticText)(this.GetItem("lblPyld").Specific));
            this.lblOrigin = ((SAPbouiCOM.StaticText)(this.GetItem("lblOrgn").Specific));
            this.lblDestination = ((SAPbouiCOM.StaticText)(this.GetItem("lblDest").Specific));
            this.lblKmA = ((SAPbouiCOM.StaticText)(this.GetItem("lblKmA").Specific));
            this.txtOrigin = ((SAPbouiCOM.EditText)(this.GetItem("txtOrgn").Specific));
            this.txtDestination = ((SAPbouiCOM.EditText)(this.GetItem("txtDest").Specific));
            this.txtKmA = ((SAPbouiCOM.EditText)(this.GetItem("txtKmA").Specific));
            this.lblKmC = ((SAPbouiCOM.StaticText)(this.GetItem("lblKmC").Specific));
            this.lblKmE = ((SAPbouiCOM.StaticText)(this.GetItem("lblKmE").Specific));
            this.lblPriceA = ((SAPbouiCOM.StaticText)(this.GetItem("lblPrcA").Specific));
            this.lblPriceC = ((SAPbouiCOM.StaticText)(this.GetItem("lblPrcC").Specific));
            this.lblPriceE = ((SAPbouiCOM.StaticText)(this.GetItem("lblPrcE").Specific));
            this.txtKmC = ((SAPbouiCOM.EditText)(this.GetItem("txtKmC").Specific));
            this.txtKmE = ((SAPbouiCOM.EditText)(this.GetItem("txtKmE").Specific));
            this.txtPriceA = ((SAPbouiCOM.EditText)(this.GetItem("txtPrcA").Specific));
            this.txtPriceC = ((SAPbouiCOM.EditText)(this.GetItem("txtPrcC").Specific));
            this.txtPriceE = ((SAPbouiCOM.EditText)(this.GetItem("txtPrcE").Specific));
            this.txtKmB = ((SAPbouiCOM.EditText)(this.GetItem("txtKmB").Specific));
            this.txtKmD = ((SAPbouiCOM.EditText)(this.GetItem("txtKmD").Specific));
            this.txtKmF = ((SAPbouiCOM.EditText)(this.GetItem("txtKmF").Specific));
            this.txtPriceB = ((SAPbouiCOM.EditText)(this.GetItem("txtPrcB").Specific));
            this.txtPriceD = ((SAPbouiCOM.EditText)(this.GetItem("txtPrcD").Specific));
            this.txtPriceF = ((SAPbouiCOM.EditText)(this.GetItem("txtPrcF").Specific));
            this.lblKmB = ((SAPbouiCOM.StaticText)(this.GetItem("lblKmB").Specific));
            this.lblKmD = ((SAPbouiCOM.StaticText)(this.GetItem("lblKmD").Specific));
            this.lblKmF = ((SAPbouiCOM.StaticText)(this.GetItem("lblKmF").Specific));
            this.lblPriceF = ((SAPbouiCOM.StaticText)(this.GetItem("lblPrcF").Specific));
            this.lblPriceD = ((SAPbouiCOM.StaticText)(this.GetItem("lblPrcD").Specific));
            this.lblPriceB = ((SAPbouiCOM.StaticText)(this.GetItem("lblPrcB").Specific));
            this.btnUpdate = ((SAPbouiCOM.Button)(this.GetItem("btnUpd").Specific));
            this.lblDriver = ((SAPbouiCOM.StaticText)(this.GetItem("lblDriv").Specific));
            this.txtDriver = ((SAPbouiCOM.EditText)(this.GetItem("txtDriv").Specific));
            this.lblTotkm = ((SAPbouiCOM.StaticText)(this.GetItem("lblTotKm").Specific));
            this.lblAmountKm = ((SAPbouiCOM.StaticText)(this.GetItem("lblAmtKm").Specific));
            this.lblExtra = ((SAPbouiCOM.StaticText)(this.GetItem("lblExtra").Specific));
            this.lblAmountEns = ((SAPbouiCOM.StaticText)(this.GetItem("lblAmtEns").Specific));
            this.lblTotalFreight = ((SAPbouiCOM.StaticText)(this.GetItem("lblTFrgh").Specific));
            this.chkEnsm = ((SAPbouiCOM.CheckBox)(this.GetItem("chkEnsm").Specific));
            this.lblFrom = ((SAPbouiCOM.StaticText)(this.GetItem("lblFrom").Specific));
            this.lblTo = ((SAPbouiCOM.StaticText)(this.GetItem("lblTo").Specific));
            this.btnAccept = ((SAPbouiCOM.Button)(this.GetItem("btnAcpt").Specific));
            this.txtTotKm = ((SAPbouiCOM.EditText)(this.GetItem("txtTotKm").Specific));
            this.txtAmountKm = ((SAPbouiCOM.EditText)(this.GetItem("txtAmtKm").Specific));
            this.txtExtra = ((SAPbouiCOM.EditText)(this.GetItem("txtExtra").Specific));
            this.txtAmountEns = ((SAPbouiCOM.EditText)(this.GetItem("txtAmtEns").Specific));
            this.txtTotalFreight = ((SAPbouiCOM.EditText)(this.GetItem("txtTFrgh").Specific));
            this.btnRoute = ((SAPbouiCOM.Button)(this.GetItem("btnRoute").Specific));
            this.lblDescription = ((SAPbouiCOM.StaticText)(this.GetItem("lblDesc").Specific));
            this.txtMorign = ((SAPbouiCOM.EditText)(this.GetItem("txtMOrig").Specific));
            this.txtMDest = ((SAPbouiCOM.EditText)(this.GetItem("txtMDest").Specific));
            this.lblArea = ((SAPbouiCOM.StaticText)(this.GetItem("lblArea").Specific));
            this.lblComment = ((SAPbouiCOM.StaticText)(this.GetItem("lblCmnt").Specific));
            this.txtComment = ((SAPbouiCOM.EditText)(this.GetItem("txtCmnt").Specific));
            this.btnExit = ((SAPbouiCOM.Button)(this.GetItem("bntExt").Specific));
            this.txtInternal = ((SAPbouiCOM.EditText)(this.GetItem("txtItrnl").Specific));
            this.txtStatus = ((SAPbouiCOM.EditText)(this.GetItem("txtStat").Specific));
            this.btnCancel = ((SAPbouiCOM.Button)(this.GetItem("btnCan").Specific));
            this.cboArea = ((SAPbouiCOM.ComboBox)(this.GetItem("cboArea").Specific));
            this.lblRetention = ((SAPbouiCOM.StaticText)(this.GetItem("lblRet").Specific));
            this.txtRetentions = ((SAPbouiCOM.EditText)(this.GetItem("txtRet").Specific));
            this.lblTax = ((SAPbouiCOM.StaticText)(this.GetItem("lblTax").Specific));
            this.txtTax = ((SAPbouiCOM.EditText)(this.GetItem("txtTax").Specific));
            this.lnkJournal = ((SAPbouiCOM.LinkedButton)(this.GetItem("lnkJournal").Specific));
            this.lnkJournal.ClickAfter += new SAPbouiCOM._ILinkedButtonEvents_ClickAfterEventHandler(this.LinkedButton0_ClickAfter);
            this.lnkCancel = ((SAPbouiCOM.LinkedButton)(this.GetItem("lnkCancel").Specific));
            this.lnkCancel.ClickAfter += new SAPbouiCOM._ILinkedButtonEvents_ClickAfterEventHandler(this.LinkedButton1_ClickAfter);
            this.lblKg = ((SAPbouiCOM.StaticText)(this.GetItem("lblKg").Specific));
            this.txtKg = ((SAPbouiCOM.EditText)(this.GetItem("txtKg").Specific));
            this.txtVarios = ((SAPbouiCOM.EditText)(this.GetItem("txtVarios").Specific));
            this.lblVarios = ((SAPbouiCOM.StaticText)(this.GetItem("lblVarios").Specific));
            this.lblAFDesc = ((SAPbouiCOM.StaticText)(this.GetItem("lblAFDesc").Specific));
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
            //LinkedButton0.LinkedObject = BoLinkedObject.lf_JournalPosting;
            //LinkedButton0.LinkedObjectType = "30";
                
        }
        #endregion

        #region SAP objects
        private SAPbouiCOM.EditText txtArticle;
        private SAPbouiCOM.EditText txtDesc;
        private SAPbouiCOM.ComboBox cboShared;
        private SAPbouiCOM.StaticText lblArticle;
        private SAPbouiCOM.StaticText lblShared;
        private SAPbouiCOM.ComboBox cboVehicleType;
        private SAPbouiCOM.StaticText lblVehicleType;
        private SAPbouiCOM.EditText txtFolio;
        private SAPbouiCOM.StaticText lblFolio;
        private SAPbouiCOM.StaticText lblEcNum;
        private SAPbouiCOM.EditText txtEcNum;
        private SAPbouiCOM.ComboBox cboPayload;
        private SAPbouiCOM.StaticText lblPayload;
        private SAPbouiCOM.StaticText lblOrigin;
        private SAPbouiCOM.StaticText lblDestination;
        private SAPbouiCOM.StaticText lblKmA;
        private SAPbouiCOM.EditText txtOrigin;
        private SAPbouiCOM.EditText txtDestination;
        private SAPbouiCOM.EditText txtKmA;
        private SAPbouiCOM.EditText txtMorign;
        private SAPbouiCOM.EditText txtMDest;
        private SAPbouiCOM.StaticText lblKmC;
        private SAPbouiCOM.StaticText lblKmE;
        private SAPbouiCOM.StaticText lblPriceA;
        private SAPbouiCOM.StaticText lblPriceC;
        private SAPbouiCOM.StaticText lblPriceE;
        private SAPbouiCOM.EditText txtKmC;
        private SAPbouiCOM.EditText txtKmE;
        private SAPbouiCOM.EditText txtPriceA;
        private SAPbouiCOM.EditText txtPriceC;
        private SAPbouiCOM.EditText txtPriceE;
        private SAPbouiCOM.EditText txtKmB;
        private SAPbouiCOM.EditText txtKmD;
        private SAPbouiCOM.EditText txtKmF;
        private SAPbouiCOM.EditText txtPriceB;
        private SAPbouiCOM.EditText txtPriceD;
        private SAPbouiCOM.EditText txtPriceF;
        private SAPbouiCOM.StaticText lblKmB;
        private SAPbouiCOM.StaticText lblKmD;
        private SAPbouiCOM.StaticText lblKmF;
        private SAPbouiCOM.StaticText lblPriceF;
        private SAPbouiCOM.StaticText lblPriceD;
        private SAPbouiCOM.StaticText lblPriceB;
        private SAPbouiCOM.Button btnUpdate;
        private SAPbouiCOM.StaticText lblDriver;
        private SAPbouiCOM.EditText txtDriver;
        private SAPbouiCOM.StaticText lblTotkm;
        private SAPbouiCOM.StaticText lblAmountKm;
        private SAPbouiCOM.StaticText lblExtra;
        private SAPbouiCOM.StaticText lblAmountEns;
        private SAPbouiCOM.StaticText lblTotalFreight;
        private SAPbouiCOM.CheckBox chkEnsm;
        private SAPbouiCOM.StaticText lblFrom;
        private SAPbouiCOM.StaticText lblTo;
        private SAPbouiCOM.Button btnAccept;
        private SAPbouiCOM.EditText txtTotKm;
        private SAPbouiCOM.EditText txtAmountKm;
        private SAPbouiCOM.EditText txtExtra;
        private SAPbouiCOM.EditText txtAmountEns;
        private SAPbouiCOM.EditText txtTotalFreight;
        private SAPbouiCOM.Button btnRoute;
        private SAPbouiCOM.StaticText lblDescription;
        private SAPbouiCOM.StaticText lblArea;
        private SAPbouiCOM.StaticText lblComment;
        private SAPbouiCOM.EditText txtComment;
        private SAPbouiCOM.Button btnExit;
        private SAPbouiCOM.EditText txtInternal;
        private SAPbouiCOM.EditText txtStatus;
        private SAPbouiCOM.Button btnCancel;
        private SAPbouiCOM.ComboBox cboArea;
        private SAPbouiCOM.StaticText lblRetention;
        private SAPbouiCOM.EditText txtRetentions;
        private SAPbouiCOM.StaticText lblTax;
        private SAPbouiCOM.EditText txtTax;
        private SAPbouiCOM.LinkedButton lnkJournal;
        private SAPbouiCOM.LinkedButton lnkCancel;
        private SAPbouiCOM.StaticText lblKg;
        private SAPbouiCOM.EditText txtKg;
        private SAPbouiCOM.EditText txtVarios;
        private SAPbouiCOM.StaticText lblVarios;

        #endregion
  

        private void LinkedButton0_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                if (txtInternal.Item.Visible && !string.IsNullOrEmpty(txtInternal.Value))
                {
                   // int lIntJounralEntryID = mObjTransportsFactory.GetJournalService().GetTransId(lObjTxtInternal.Value, "TR/F");
                    SAPbouiCOM.Framework.Application.SBO_Application.OpenForm(BoFormObjectEnum.fo_JournalPosting, "", mStrJounralEntryId);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
        }

      

        private void LinkedButton1_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                if (txtInternal.Item.Visible && !string.IsNullOrEmpty(txtInternal.Value))
                {
                    string lStrTransId = mObjTransportsFactory.GetJournalService().GetCancelJournal(mStrJounralEntryId);
                    SAPbouiCOM.Framework.Application.SBO_Application.OpenForm(BoFormObjectEnum.fo_JournalPosting, "", lStrTransId);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
        }

        private void cboPayload_ComboSelectAfter(object sboObject, SBOItemEventArg pVal)
        {
            if (!string.IsNullOrEmpty(cboPayload.Value))
            {
                txtVarios.Item.Visible = true;
                lblVarios.Item.Visible = true;

                mStrType = mObjTransportsFactory.GetRouteService().GetTypeTRTY(cboPayload.Value);
                switch (mStrType)
                {
                    case "G":
                        lblVarios.Caption = "Cabezas";
                        break;
                    case "A":
                        lblVarios.Caption = "Sacos";
                        break;
                    case "O":
                        lblVarios.Caption = "Varios";
                        break;
                }
            }
            else
            {
                txtVarios.Item.Visible = false;
                lblVarios.Item.Visible = false;
            }
           
        }

        private StaticText lblAFDesc;

       

    }
}
