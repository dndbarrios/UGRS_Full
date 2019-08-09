using SAPbouiCOM;
using System;
using System.Text.RegularExpressions;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.Transports;
using UGRS.Core.SDK.DI.Transports.DTO;
using UGRS.Core.SDK.DI.Transports.Utility;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;

namespace UGRS.AddOn.Transports
{
    public class Freights
    {
        SAPbouiCOM.Form mObjSOForm;
        SAPbouiCOM.Form mObjUFForm;
        SAPbouiCOM.Form mObjRetentions;
       // SAPbouiCOM.Item mObjItm;
        //SAPbouiCOM.Button mObjBtnFreight;
        SAPbouiCOM.EditText mObjTxtCardCode;
        SAPbouiCOM.ComboBox mObjCboDoc;

        SAPbouiCOM.Matrix mObjMtxSO;
        SAPbouiCOM.EditText mObjTxtItem;

        Utils mObjUtility = new Utils();
        TransportServiceFactory mObjTransportService = new TransportServiceFactory();
        Forms.frmFreights mFrmFreights = null;

        private SalesOrderLinesDTO mObjSalesOrderLines = null;
        private FreightsParamsDTO mObjFreightsParams = null;
        private int mIntUserSignature = DIApplication.Company.UserSignature;
        private int mIntFormType = 0;
        private string mStrCardCode = string.Empty;

        bool mBoolInsurance = false;
        bool mBoolFreightsModal = false;
        bool mBoolChooseFromList = false;

        #region Constructor
        public Freights()
        {
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }

        private void SetFormParameters(int pIntFormType, bool pBoolInsurance, bool pBoolInternal, bool pBoolLoaded)
        {
            mObjFreightsParams = new FreightsParamsDTO();

            mObjFreightsParams.UserSign = mIntUserSignature;
            mObjFreightsParams.FormType = pIntFormType;
            mObjFreightsParams.Insurance = pBoolInsurance;
            mObjFreightsParams.Internal = pBoolInternal;
            mObjFreightsParams.Loaded = pBoolLoaded;
            mObjFreightsParams.SalesOrderLines = mObjSalesOrderLines;
            mObjFreightsParams.CardCode = mStrCardCode;
        }
        #endregion

        #region Methods
        private void UnloadEvents()
        {
            try
            {

                //SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
               // mObjBtnFreight.ClickBefore -= new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.lObjBtnFreight_ClickBefore);
                //this.mObjTxtCardCode.LostFocusAfter -= lObjTxtCardCode_LostFocusAfter;
            }
            catch (Exception)
            {
                //Ignore

            }
        }

        private void SetFormSettings(Form pFrmActive)
        {
            try
            {
                mObjSOForm = pFrmActive;//SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetFormByTypeAndCount(pFrmActive.Type, pFrmActive.TypeCount);
                mIntFormType = pFrmActive.Type;
                mObjTxtCardCode = (SAPbouiCOM.EditText)mObjSOForm.Items.Item("4").Specific;
                
               
                mObjMtxSO = (SAPbouiCOM.Matrix)mObjSOForm.Items.Item("38").Specific;
                mObjCboDoc = (SAPbouiCOM.ComboBox)mObjSOForm.Items.Item("81").Specific;
            }
            catch (Exception)
            {
                //ignore

            }
        }

        private void AddButton()
        {
           
            if (!mObjUtility.ItemExist("btnFrgh", mObjSOForm))
            {
                SAPbouiCOM.Button lObjBtnFreight;
                SAPbouiCOM.Item lObjItm;
                SAPbouiCOM.Item txtUUID = mObjSOForm.Items.Item("70");
                lObjItm = mObjSOForm.Items.Add("btnFrgh", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                lObjItm.FromPane = 1;
                lObjItm.ToPane = 1;
                lObjItm.Top = txtUUID.Top + 25;
                lObjItm.Left = txtUUID.Left;

               
                lObjBtnFreight = ((SAPbouiCOM.Button)mObjSOForm.Items.Item("btnFrgh").Specific);
                lObjBtnFreight.Caption = "Fletes";
                lObjBtnFreight.Item.Enabled = false;
                lObjBtnFreight.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.lObjBtnFreight_ClickBefore);

            }
        }

        private bool EmptyOrder()
        {
            if (mObjMtxSO.RowCount >= 1)
            {
                if (CheckMtxItems() || !string.IsNullOrEmpty(mObjTxtItem.Value))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckMtxItems()
        {
            mBoolInsurance = false;
            mObjSalesOrderLines = null;
            for (int i = 1; i <= mObjMtxSO.RowCount; i++)
            {
                mObjTxtItem = (SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("1").Cells.Item(i).Specific;
                if (!string.IsNullOrEmpty(mObjTxtItem.Value) && TransportsItem(mObjTxtItem.Value))
                {
                    SetSalesOrderLine(i);
                }
                else if (Insuranceline(mObjTxtItem.Value))
                {
                    mBoolInsurance = true;
                }
            }
            return mObjSalesOrderLines != null || mBoolInsurance ? true : false;
        }

        private bool Insuranceline(string pStrItemCode)
        {
            return mObjTransportService.GetRouteService().CheckInsuranceLine(pStrItemCode);
        }

        private bool TransportsItem(string pStrItemCode)
        {
            return mObjTransportService.GetRouteService().CheckTransportsItem(pStrItemCode, mIntUserSignature);
        }

        private void SetSalesOrderLine(int pIntRow)
        {
          
            mObjSalesOrderLines = new SalesOrderLinesDTO();

            mObjSalesOrderLines.ItemCode = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("1").Cells.Item(pIntRow).Specific).Value;
            mObjSalesOrderLines.Description = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("3").Cells.Item(pIntRow).Specific).Value;
            mObjSalesOrderLines.Shared = ((SAPbouiCOM.EditText)mObjUFForm.Items.Item("U_TR_Shared").Specific).Value == "N" ? false : true;
            mObjSalesOrderLines.Folio = ((SAPbouiCOM.EditText)mObjUFForm.Items.Item("U_GLO_Ticket").Specific).Value;
            //mObjSalesOrderLines.Shared = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("U_TR_Compartido").Cells.Item(pIntRow).Specific).Value == "N" ? false : true; //mObjTransportService.GetRouteService().CheckIfShared(mObjSalesOrderLines.Folio) ? true : false;
            mObjSalesOrderLines.PayloadType = ((SAPbouiCOM.ComboBox)mObjMtxSO.Columns.Item("U_TR_LoadType").Cells.Item(pIntRow).Specific).Value;
            mObjSalesOrderLines.VehicleType = ((SAPbouiCOM.ComboBox)mObjMtxSO.Columns.Item("U_TR_VehicleType").Cells.Item(pIntRow).Specific).Value;
            string lStrRoute = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("U_TR_Paths").Cells.Item(pIntRow).Specific).Value;
            lStrRoute = string.IsNullOrEmpty(lStrRoute) ? "0" : lStrRoute;
            mObjSalesOrderLines.Route = Convert.ToInt32(lStrRoute);

            string lStr = ((SAPbouiCOM.ComboBox)mObjMtxSO.Columns.Item("27").Cells.Item(pIntRow).Specific).Selected.Description;
            mObjSalesOrderLines.Employee = ((SAPbouiCOM.ComboBox)mObjMtxSO.Columns.Item("27").Cells.Item(pIntRow).Specific).Selected.Description;
            mObjSalesOrderLines.Asset = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("2003").Cells.Item(pIntRow).Specific).Value;
            mObjSalesOrderLines.TotKm = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("U_TR_TotKm").Cells.Item(pIntRow).Specific).Value;
            mObjSalesOrderLines.Extra = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("U_TR_AdditionalExpen").Cells.Item(pIntRow).Specific).Value;
            mObjSalesOrderLines.KmA = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("U_TR_TypeA").Cells.Item(pIntRow).Specific).Value;
            mObjSalesOrderLines.KmB = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("U_TR_TypeB").Cells.Item(pIntRow).Specific).Value;
            mObjSalesOrderLines.KmC = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("U_TR_TypeC").Cells.Item(pIntRow).Specific).Value;
            mObjSalesOrderLines.KmD = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("U_TR_TypeD").Cells.Item(pIntRow).Specific).Value;
            mObjSalesOrderLines.KmE = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("U_TR_TypeE").Cells.Item(pIntRow).Specific).Value;
            mObjSalesOrderLines.KmF = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("U_TR_TypeF").Cells.Item(pIntRow).Specific).Value;
            mObjSalesOrderLines.TotKg = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("U_TR_TotKilos").Cells.Item(pIntRow).Specific).Value;
            mObjSalesOrderLines.Heads = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("U_TR_Heads").Cells.Item(pIntRow).Specific).Value;
            mObjSalesOrderLines.Bags = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("U_GLO_BagsBales").Cells.Item(pIntRow).Specific).Value;
            mObjSalesOrderLines.Varios = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("U_TR_OtherLoad").Cells.Item(pIntRow).Specific).Value;
            
            string lStrUnitPrice = ((SAPbouiCOM.EditText)mObjMtxSO.Columns.Item("14").Cells.Item(pIntRow).Specific).Value;
            lStrUnitPrice = Regex.Replace(lStrUnitPrice, "[^0-9.]", "");
            mObjSalesOrderLines.UnitPrice = float.Parse(lStrUnitPrice);

        }

        private void OpenFreightsForm()
        {
            if (mObjUtility.ItemExist("btnFrgh", mObjSOForm))
            {
                SAPbouiCOM.Item lObjItm;
                lObjItm = mObjSOForm.Items.Item("btnFrgh");
                if (!string.IsNullOrEmpty(mObjTxtCardCode.Value) && (lObjItm.Specific as SAPbouiCOM.Button).Item.Enabled)
                {
                    mStrCardCode = mObjTxtCardCode.Value;
                    SetFormParameters(mIntFormType, false, false, false);
                    if (!mObjUtility.FormExists("freights"))
                    {
                        mFrmFreights = new Forms.frmFreights(mObjFreightsParams);
                        mFrmFreights.UIAPIRawForm.Top = mObjSOForm.Top + mObjSOForm.Height /2 - mFrmFreights.UIAPIRawForm.Height / 2;
                        mFrmFreights.UIAPIRawForm.Left = mObjSOForm.Left + mObjSOForm.Width / 2 - mFrmFreights.UIAPIRawForm.Width / 2;
                        mFrmFreights.Show();
                        mBoolFreightsModal = mFrmFreights.pBoolFreightsModal;
                    }
                }
            }
        }

        private void LoadFreightsForm()
        {
            if (mObjUtility.ItemExist("btnFrgh", mObjSOForm))
            {
                SAPbouiCOM.Item lObjItm;
                lObjItm = mObjSOForm.Items.Item("btnFrgh");
                if (!string.IsNullOrEmpty(mObjTxtCardCode.Value) && (lObjItm.Specific as SAPbouiCOM.Button).Item.Enabled)
                {
                    mStrCardCode = mObjTxtCardCode.Value;
                    SetFormParameters(mIntFormType, mBoolInsurance, false, true);
                    if (!mObjUtility.FormExists("freights"))
                    {
                        mFrmFreights = new Forms.frmFreights(mObjFreightsParams);
                        mFrmFreights.UIAPIRawForm.Top = mObjSOForm.Top + mObjSOForm.Height / 2 - mFrmFreights.UIAPIRawForm.Height / 2;
                        mFrmFreights.UIAPIRawForm.Left = mObjSOForm.Left + mObjSOForm.Width / 2 - mFrmFreights.UIAPIRawForm.Width / 2;
                        mFrmFreights.Show();
                        mBoolFreightsModal = mFrmFreights.pBoolFreightsModal;
                    }
                }
            }
        }

        #endregion

        #region Events
        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            #region AddButton In Sales Order
            try
            {
                if (pVal.FormType == 139 || pVal.FormType == 133 || pVal.FormType == 149)
                {
                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                                mObjSOForm = SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);
                                SetFormSettings(mObjSOForm);
                                this.mObjTxtCardCode.LostFocusAfter += lObjTxtCardCode_LostFocusAfter;
                                AddButton();
                                break;

                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                UnloadEvents();
                                mObjUFForm = null;
                                mObjRetentions = null;
                                break;

                            case SAPbouiCOM.BoEventTypes.et_CLICK:
                                if (pVal.ItemUID == "btnFrgh")
                                {
                                    mObjUFForm = SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType * -1, pVal.FormTypeCount);
                                    LoadFreight();
                                }
                                break;
                            case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST:
                                if (pVal.ItemUID == "4")
                                {
                                    mBoolChooseFromList = true;
                                }

                                break;

                            case SAPbouiCOM.BoEventTypes.et_FORM_ACTIVATE:
                                if (mBoolChooseFromList)
                                {
                                    mObjSOForm = SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);
                                    EnableButton();
                                    mBoolChooseFromList = false;
                                }
                                break;
                        }
                    }
                }
                else if (pVal.FormType == -139 || pVal.FormType == -133 || pVal.FormType == -149)
                {
                    if (mObjUFForm == null)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                                mObjUFForm = SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);
                                break;
                        }
                    }
                }
                else if (pVal.FormType == 60504)
                {
                    if (mObjRetentions == null)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                                mObjRetentions = SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);
                                break;
                        }
                    }
                }
            }
             
            catch (Exception ex)
            {
                LogService.WriteError("(SBO_Application_ItemEvent): " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
            #endregion
        }

        void lObjTxtCardCode_LostFocusAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {

            EnableButton();
        }

        private void EnableButton()
        {
            if (mObjUtility.ItemExist("btnFrgh", mObjSOForm))
            {
                SAPbouiCOM.Item lObjItm;
                lObjItm = mObjSOForm.Items.Item("btnFrgh");
                Form lObjSOForm = UIApplication.GetApplication().Forms.ActiveForm;
                SetFormSettings(lObjSOForm);
                if (!string.IsNullOrEmpty(mObjTxtCardCode.Value) && !mObjCboDoc.Item.Description.Equals("Cerrado"))
                {
                    (lObjItm.Specific as SAPbouiCOM.Button).Item.Enabled = true;
                }
                else
                {
                    (lObjItm.Specific as SAPbouiCOM.Button).Item.Enabled = false;
                }
            }
        }

        private void lObjBtnFreight_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            LoadFreight();
            
        }

        private void LoadFreight()
        {
            try
            {
                Form lObjSOForm = UIApplication.GetApplication().Forms.ActiveForm;//SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetFormByTypeAndCount(mIntFormType = pFrmActive.Type;, pVal.FormTypeCount);
                SetFormSettings(lObjSOForm);
                //lObjMtxSO = UIApplication.GetApplication().Forms.ActiveForm;

                //5897
                if (EmptyOrder())
                {
                    OpenFreightsForm();
                }
                else
                {
                    LoadFreightsForm();
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("(LoadCombobox): " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
        }

        #endregion
    }
}
