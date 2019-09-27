using System;
using System.Collections.Generic;
using System.Linq;
using SAPbouiCOM.Framework;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;
using SAPbouiCOM;
using UGRS.Core.SDK.DI.Transports;
using UGRS.Core.SDK.DI.Transports.DTO;
using UGRS.Core.SDK.DI.Transports.Tables;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.UI.ProgressBar;
using UGRS.Core.SDK.DI.Tramsport.DTO;
using UGRS.Core.SDK.DI.Transports.Enums;
using UGRS.Core.Extension.Enum;
using System.Collections;

namespace UGRS.AddOn.Transports.Forms
{
    [FormAttribute("UGRS.AddOn.Transports.Forms.frmCommissions", "Forms/frmCommissions.b1f")]
    class frmCommissions : UserFormBase
    {
        #region Properties
        int mIntSelectedRow;
        int mIntSelectedComissionRow;
        TransportServiceFactory mObjTransportsFactory = new TransportServiceFactory();
        //int mIntFirstDay;
        DateTime mDtmFirstDay;
        DateTime mDtmLastDay;
        string mStrFolio;
        const int mIntColorGreen = 39219;
        const int mIntColorWhite = 16777215;
        const int mIntColorRed = 225;
        const int mIntColorGray = 14869218;
        StatusEnum mEnumStatus;
        AuthorizerEnum mAuthorizerEnum;
        List<CommissionDriverDTO> mLstCmsnDriverDTO = new List<CommissionDriverDTO>();
        #endregion
      
        #region Constructor
        public frmCommissions()
        {
            LoadEvents();
            
        }

        #endregion

        #region InitializeComponents
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.mObjlblFolio = ((SAPbouiCOM.StaticText)(this.GetItem("lblFolio").Specific));
            this.mObjTxtFolio = ((SAPbouiCOM.EditText)(this.GetItem("txtFolio").Specific));
            this.mObjTxtFolio.ValidateAfter += new SAPbouiCOM._IEditTextEvents_ValidateAfterEventHandler(this.mObjTxtFolio_ValidateAfter);
            this.mObjBtnSearch = ((SAPbouiCOM.Button)(this.GetItem("btnSearch").Specific));
            this.mObjBtnSearch.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.mObjBtnSearch_ClickAfter);
            this.mObjTxtStatus = ((SAPbouiCOM.EditText)(this.GetItem("txtStatus").Specific));
            this.mObjlblStat = ((SAPbouiCOM.StaticText)(this.GetItem("lblStat").Specific));
            this.mObjTxtArea = ((SAPbouiCOM.EditText)(this.GetItem("txtAuArea").Specific));
            this.mObjBtnAuth = ((SAPbouiCOM.Button)(this.GetItem("btnAuth").Specific));
            this.mObjBtnAuth.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.mObjBtnAuth_ClickAfter);
            this.mObjlblArea = ((SAPbouiCOM.StaticText)(this.GetItem("lblAuArea").Specific));
            this.mObjMtxDrv = ((SAPbouiCOM.Matrix)(this.GetItem("mtxDrv").Specific));
            this.mObjMtxDrv.ClickAfter += new SAPbouiCOM._IMatrixEvents_ClickAfterEventHandler(this.mObjMtxDrv_ClickAfter);
            this.mObjMtxDrv.ClickBefore += new SAPbouiCOM._IMatrixEvents_ClickBeforeEventHandler(this.mObjMtxDrv_ClickBefore);
            this.mObjMtxCommission = ((SAPbouiCOM.Matrix)(this.GetItem("mtxCom").Specific));
            this.mObjMtxCommission.LinkPressedAfter += new SAPbouiCOM._IMatrixEvents_LinkPressedAfterEventHandler(this.mObjMtxCommission_LinkPressedAfter);
            this.mObjMtxCommission.ClickBefore += new SAPbouiCOM._IMatrixEvents_ClickBeforeEventHandler(this.mObjMtxCommission_ClickBefore);
            this.mObjBtnOk = ((SAPbouiCOM.Button)(this.GetItem("btnOk").Specific));
            this.mObjBtnOk.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.mObjBtnOk_ClickBefore);
            this.mObjBtnOk.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.mObjBtnOk_ClickAfter);
            this.mObjlblCmnt = ((SAPbouiCOM.StaticText)(this.GetItem("lblCmnt").Specific));
            this.mObjTxtCmnt = ((SAPbouiCOM.EditText)(this.GetItem("txtCmnt").Specific));
            this.mObjBtnCancel = ((SAPbouiCOM.Button)(this.GetItem("btnCancel").Specific));
            this.mObjBtnCancel.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.mObjBtnCancel_ClickAfter);
            this.lblDates = ((SAPbouiCOM.StaticText)(this.GetItem("lblDates").Specific));
            this.txtAuOp = ((SAPbouiCOM.EditText)(this.GetItem("txtAuOp").Specific));
            this.lblAuOp = ((SAPbouiCOM.StaticText)(this.GetItem("lblAuOp").Specific));
            this.lblAuFz = ((SAPbouiCOM.StaticText)(this.GetItem("lblAuFz").Specific));
            this.txtAuFz = ((SAPbouiCOM.EditText)(this.GetItem("txtAuFz").Specific));
            //         this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("Item_4").Specific));
            this.mObjBtnReject = ((SAPbouiCOM.Button)(this.GetItem("BtnRej").Specific));
            this.mObjBtnReject.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.mObjBtnReject_ClickAfter);
            this.cboWeek = ((SAPbouiCOM.ComboBox)(this.GetItem("cboWeek").Specific));
            this.lblWeek = ((SAPbouiCOM.StaticText)(this.GetItem("lblWeek").Specific));
            this.lblYear = ((SAPbouiCOM.StaticText)(this.GetItem("lblYear").Specific));
            this.cboYear = ((SAPbouiCOM.ComboBox)(this.GetItem("cboYear").Specific));
            this.chkAdd = ((SAPbouiCOM.CheckBox)(this.GetItem("chkAdd").Specific));
            this.txtDate = ((SAPbouiCOM.EditText)(this.GetItem("txtDate").Specific));
            this.lblDate = ((SAPbouiCOM.StaticText)(this.GetItem("Item_1").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.ResizeAfter += new ResizeAfterHandler(this.Form_ResizeAfter);

        }


        private void OnCustomInitialize()
        {
            try
            {
                //LoadChooseFromList();
               // SetChooseToTxt();
                
                mStrFolio = GetFolioWeek();
                mObjTxtFolio.Value = mStrFolio;
                LoadFirstDay(DateTime.Now.Year);
                VisibleAuthControls(false);
                VisibleStatusControls(false);
                
                //Obtiene el tipo permiso
                GetUserPermissionAuth();
              
                this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_Date").Value = DateTime.Now.ToString("yyyyMMdd");
               
            }
            catch (Exception ex)
            {
                LogService.WriteError("(frmCommissions): " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);

            }
        }
        #endregion

        #region Events
        private void Form_ResizeAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                UIAPIRawForm.Freeze(true);

                //Width
                mObjMtxDrv.Item.Width = UIAPIRawForm.Width - 50;
                mObjMtxCommission.Item.Width = UIAPIRawForm.Width - 50;

                //Height
                mObjMtxDrv.Item.Height = UIAPIRawForm.Height / 3;
                mObjMtxCommission.Item.Height = (int)(UIAPIRawForm.Height /3.5);

                ////Left
                //mOBjMtxDrv.Item.Left = mOBjMtxDrv.Item.Width + 5;
                //mObjMtxCommission.Item.Left = mObjMtxCommission.Item.Width + ;5w
                
                //Top
                mObjMtxCommission.Item.Top = mObjMtxDrv.Item.Top + mObjMtxDrv.Item.Height + 20;
              

                mObjMtxDrv.AutoResizeColumns();
                mObjMtxCommission.AutoResizeColumns();

                
            }
            catch (Exception lObjException)
            {
                UIApplication.ShowError("frmCommissions (Form_ResizeAfter)" + lObjException.Message);
                LogService.WriteError("frmCommissions (Form_ResizeAfter)" + lObjException.Message);
                LogService.WriteError(lObjException);
            }
            finally
            {
                UIAPIRawForm.Freeze(false);
            }
        }

        private void mObjMtxDrv_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            { 
                this.UIAPIRawForm.Freeze(true);
                
                if (pVal.Row > 0)
                {
                    mIntSelectedRow = pVal.Row;
                    mObjMtxDrv.SelectRow(pVal.Row, true, false);
                    BindMatrixInv();
                    LoadDtCommissionInfo();
                }
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                LogService.WriteError("(frmCommissions): " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
            finally
            {
                mObjProgressBar.Dispose();
                this.UIAPIRawForm.Freeze(false);
            }
        }

        private void mObjMtxCommission_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                this.UIAPIRawForm.Freeze(true);

                if (pVal.Row > 0)
                {
                    this.UIAPIRawForm.Freeze(false);
                    mIntSelectedComissionRow = pVal.Row;
                    mObjMtxCommission.SelectRow(pVal.Row, true, false);

                }
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                LogService.WriteError("(frmCommissions): " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }

        }

        private void mObjMtxCommission_LinkPressedAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                mDtMatrixCmsn = this.UIAPIRawForm.DataSources.DataTables.Item("Dt_Cmsn");
                string lStrDocEntry = mDtMatrixCmsn.GetValue("cDocEntry", pVal.Row - 1).ToString();
                string lStrType = mDtMatrixCmsn.GetValue("cOpType", pVal.Row - 1).ToString();
                if (lStrType == "FI")
                {
                    SAPbouiCOM.Framework.Application.SBO_Application.OpenForm(BoFormObjectEnum.fo_JournalPosting, "", lStrDocEntry);
                }
                else
                {
                    SAPbouiCOM.Framework.Application.SBO_Application.OpenForm(BoFormObjectEnum.fo_Invoice, "Inv", lStrDocEntry);
                }
                //
            }
            catch (Exception ex)
            {
                LogService.WriteError("(mObjMtxCommission_LinkPressedAfter): " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }

        }

        private void mObjBtnOk_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            string lStrDocEntry = String.Empty;
            bool lBolSuccess = false;
            if (mEnumStatus == StatusEnum.OPEN || mEnumStatus == StatusEnum.CANCELED)
            {
                if (SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("¿Desea realizar la comision a choferes?", 2, "Si", "No", "") == 1)
                {
                    try
                    {
                        string lStrLastFolio = GetLastFolio(mObjTxtFolio.Value, 1);
                        Commissions lLstComm = GetCommisionHeader(lStrLastFolio);
                        bool lBolOk = true;
                        if (lLstComm.Status != (int)StatusEnum.CLOSED)
                        {
                            lBolOk = 
                                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(
                                "La comisión anterior no esta cerrada y no tomara encuenta el adeudo anterior ¿Desea continuar?", 2, "Si", "No", "") == 1 ? true :false;
                           
                        }
                        if (lBolOk)
                        {
                            DIApplication.Company.StartTransaction();
                            this.UIAPIRawForm.Freeze(true);
                            lBolSuccess = SaveCommission();
                        }
                    }
                    catch (Exception ex)
                    {
                        this.UIAPIRawForm.Freeze(false);
                        lBolSuccess = false;
                        LogService.WriteError("SaveCommission " + ex.Message);
                        LogService.WriteError(ex);
                        UIApplication.ShowMessageBox(ex.Message);
                    }
                    finally
                    {
                        mObjProgressBar.Dispose();
                        CommitTransaction(lBolSuccess);
                        this.UIAPIRawForm.Freeze(false);
                        if (lBolSuccess)
                        {
                            mObjBtnSearch.Item.Click();
                        }
                    }
                }
            }
            else
            {
                UIApplication.ShowMessageBox("El folio ya se encuentra cerrado");
            }

        }
 
        private void mObjBtnOk_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

        }

        private void mObjBtnSearch_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                string lStrFolio = mObjTxtFolio.Value;
                int lIntWeek = Convert.ToInt32(lStrFolio.Substring(0, 2));
                int lIntYear = Convert.ToInt32(lStrFolio.Substring(3, 2));
                List<CommissionLine> lLstCmsnLn = new List<CommissionLine>();
                this.UIAPIRawForm.Freeze(true);
                BindMatrixDriv();
                
                Commissions lObjCommissionHeader =  GetCommisionHeader(mObjTxtFolio.Value);

               
                if (lObjCommissionHeader == null || lObjCommissionHeader.Status == (int)StatusEnum.CANCELED)
                {
                    SetControlsNew();
                    if (lObjCommissionHeader != null)
                    {
                        SetTxtStatus((StatusEnum)lObjCommissionHeader.Status);
                    }
                }
                else
                {
                    lLstCmsnLn = mObjTransportsFactory.GetCommissionService().GetCommissionLine(lObjCommissionHeader.RowCode).ToList();
                    SetControlsLoad(lObjCommissionHeader);
                }
                SetMatrixLoad(lLstCmsnLn, lObjCommissionHeader);
               
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                LogService.WriteError("mObjBtnSearch_ClickBefore " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
            finally
            {
                if (mObjProgressBar != null)
                {
                    mObjProgressBar.Dispose();
                }
                this.UIAPIRawForm.Freeze(false);
            }

        }      

        private void mObjTxtFolio_ValidateAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                
                string lStrFolio =  mObjTxtFolio.Value;
                int lIntWeek = Convert.ToInt32(lStrFolio.Substring(0, 2));
                int lIntYear = Convert.ToInt32(lStrFolio.Substring(3, 2));
                LoadFirstDay(lIntYear + 2000);
               // mDtmFirstDay = new DateTime(lIntYear+2000, 1, mIntFirstDay);
                mDtmFirstDay = mDtmFirstDay.AddDays(((lIntWeek - 1) * 7));
                mDtmLastDay = mDtmFirstDay.AddDays(6);

                mStrFolio = lIntWeek.ToString("00") + "-" + lIntYear.ToString("00");
                lblDates.Caption = " Fechas: Del " + mDtmFirstDay.ToString("dd/MM/yyyy") + " Al " + mDtmLastDay.ToString("dd/MM/yyyy");

            }
            catch (Exception ex )
            {
                LogService.WriteError("mObjTxtFolio_ValidateAfter " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }

        }

        private void mObjBtnAuth_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                Commissions lObjCommissionHeader = GetCommisionHeader(mObjTxtFolio.Value);
                if (UpdateCommissionAuth(lObjCommissionHeader))
                {
                    VisibleAuthControls(false);
                    SendAlertByAuthorizer(lObjCommissionHeader);
                    UIApplication.ShowMessageBox(string.Format("Folio: {0} fue autorizado correctamente", lObjCommissionHeader.Folio));
                    SetStatusControls(GetCommisionHeader(mObjTxtFolio.Value));
                   

                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("mObjBtnAuth_ClickAfter " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }



        }
 
        private void mObjBtnReject_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                if (mEnumStatus != StatusEnum.CLOSED)
                {
                    Commissions lObjCommissionHeader = GetCommisionHeader(mObjTxtFolio.Value);
                    lObjCommissionHeader.Status = (int)StatusEnum.REJECT;
                    //lObjCommissionHeader.AutBanks = false;
                    //lObjCommissionHeader.AutOperations = false;
                    //lObjCommissionHeader.AutTrans = false;
                    UpdateCommission(lObjCommissionHeader);
                    CreateAlertReject(lObjCommissionHeader);
                    SetStatusControls(GetCommisionHeader(mObjTxtFolio.Value));
                }
                else
                {
                    UIApplication.ShowMessageBox("El folio ya se encuentra cerrado");

                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("(frmCommissions): " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
        }

        private void mObjBtnCancel_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            bool lBolSuccess = false;
            try
            {
                if (SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("¿Desea canclear la comision a choferes?", 2, "Si", "No", "") == 1)
                {
                    DIApplication.Company.StartTransaction();
                    this.UIAPIRawForm.Freeze(true);
                    lBolSuccess = CancelMovement();
                }

            }
            catch (Exception ex)
            {
                LogService.WriteError("(frmCommissions): " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
            finally
            {
                mObjProgressBar.Dispose();
                CommitTransaction(lBolSuccess);
                this.UIAPIRawForm.Freeze(false);
            }
            if (lBolSuccess)
            {
                mObjBtnSearch.Item.Click();
            }
            else
            {
                UIApplication.ShowMessageBox("No fue posible realizar la cancelación");
            }
        }

        private void mObjMtxDrv_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);
                if (pVal.ColUID == "cGenerate")
                {
                    bool lBolGenerate = ((SAPbouiCOM.CheckBox)mObjMtxDrv.Columns.Item("cGenerate").Cells.Item((pVal.Row - 1) + 1).Specific).Checked;
                    string lStrGenerate = lBolGenerate ? "Y" : "N";
                    mDtMatrixDrv.SetValue("cGenerate", pVal.Row - 1, lStrGenerate);
                    List<CommissionDriverDTO> lLstCmsDTO = GetCommissionDriverMatrix();
                    //mDtMatrixDrv.Rows.Add();
                    AddTotalLine(mDtMatrixDrv.Rows.Count - 1, lLstCmsDTO);
                    mObjMtxDrv.LoadFromDataSourceEx(false);
                    //mObjMtxDrv.LoadFromDataSource();
                }
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                LogService.WriteError("(frmCommissions): " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
            finally
            {
                mObjProgressBar.Dispose();
                this.UIAPIRawForm.Freeze(false);
            }

        }
        #endregion

        #region Methods
        #region Load & Unload Events
        public void LoadEvents()
        {
            this.ResizeAfter += new SAPbouiCOM.Framework.FormBase.ResizeAfterHandler(this.Form_ResizeAfter);
        }

        public void UnLoadEvents()
        {
            this.ResizeAfter -= new SAPbouiCOM.Framework.FormBase.ResizeAfterHandler(this.Form_ResizeAfter);
        }
        #endregion

         /* #region ChooseFromList
        private void SetChooseToTxt()
        {
            mObjTxtArea.DataBind.SetBound(true, "", "CFL_Area");
            mObjTxtArea.ChooseFromListUID = "CFL_Area";
            mObjTxtArea.ChooseFromListAlias = "PrcCode";
        }

        private void LoadChooseFromList()
        {
            ChooseFromList lObjCFLProjectArea = InitChooseFromLists(false, "61", "CFL_Area", this.UIAPIRawForm.ChooseFromLists);
            AddConditionChoseFromListArea(lObjCFLProjectArea);
        }

        /// <summary>
        /// Condiciones de chooseFromList
        /// <summary>
        private void AddConditionChoseFromListArea(ChooseFromList pCFL)
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

        /// <summary>
        /// inicia los datos a un ChooseFromList
        /// <summary>
        public ChooseFromList InitChooseFromLists(bool pbolMultiselecction, string pStrType, string pStrID, SAPbouiCOM.ChooseFromListCollection pObjCFLs) //
        {
            SAPbouiCOM.ChooseFromList lObjoCFL = null;

            SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
            oCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)UIApplication.GetApplication().CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);

            oCFLCreationParams.MultiSelection = pbolMultiselecction;
            oCFLCreationParams.ObjectType = pStrType;
            oCFLCreationParams.UniqueID = pStrID;

            lObjoCFL = pObjCFLs.Add(oCFLCreationParams);

            this.UIAPIRawForm.DataSources.UserDataSources.Add(pStrID, SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);

            return lObjoCFL;
        }

        #endregion
        */

        #region Loads
        private void SetMatrixLoad(List<CommissionLine> pLstCmsnLn, Commissions pObjCmsnHeader)
        {
            List<CommissionDriverDTO> lLstCmsnDriverDTO = new List<CommissionDriverDTO>();
           
            if (pObjCmsnHeader == null || pObjCmsnHeader.Status == (int) StatusEnum.CANCELED)
            {
                lLstCmsnDriverDTO = mObjTransportsFactory.GetCommissionService().GetCommissionDriver(mDtmFirstDay.ToString("yyyyMMdd"), mDtmLastDay.ToString("yyyyMMdd"));
                if (lLstCmsnDriverDTO != null)
               {
                   lLstCmsnDriverDTO = GetComisssionDriverList(lLstCmsnDriverDTO);
                   double lDblAdd = 0;
                    if (chkAdd.Checked)
                   {
                       lDblAdd = 1500;
                   }
                   LoadDtDivInfo(AddCommissionDebt(lLstCmsnDriverDTO, lDblAdd));
               }
            }
            else
            {
                lLstCmsnDriverDTO = mObjTransportsFactory.GetCommissionService().GetCommissionDriverSaved(pObjCmsnHeader.Folio);
                lLstCmsnDriverDTO = GetComisssionDriverList(lLstCmsnDriverDTO);
                LoadDtDivInfo(lLstCmsnDriverDTO);
            }
          
        }

        private void SetControlsNew()
        {
            mEnumStatus = StatusEnum.OPEN;
            VisibleAuthControls(false);
            VisibleStatusControls(false);
            SetTxtStatus(mEnumStatus);
            mObjBtnOk.Item.Enabled = true;
            mObjBtnOk.Caption = "Guardar";
            mObjMtxDrv.Columns.Item("cGenerate").Editable = true;
        }

        private void SetControlsLoad(Commissions pObjCommissionHeader)
        {
            mObjBtnOk.Item.Enabled = false;
            SetStatusControls(pObjCommissionHeader);
            if (pObjCommissionHeader.AutBanks && pObjCommissionHeader.AutOperations && pObjCommissionHeader.AutTrans)
            {
                mObjBtnOk.Item.Enabled = true;
                mObjBtnOk.Caption = "Generar";
            }
            SetTxtStatus((StatusEnum)pObjCommissionHeader.Status);
            VisibleStatusControls(true);

            //Coloca los controles dependiendo del permiso
            SetControlsByPermissionUser(mAuthorizerEnum, pObjCommissionHeader);
            mObjTxtFolio.Item.Click();
            mObjMtxDrv.Columns.Item("cGenerate").Editable = false;
        }
        #endregion

        #region Matrix

        private List<CommissionDriverDTO> AddCommissionDebt(List<CommissionDriverDTO> pLstCommissionDriver, double pDblPayAdd)
        {
            mObjProgressBar = new ProgressBarManager(UIApplication.GetApplication(), "Consultando deudas anteriores", pLstCommissionDriver.Count + 2);
            foreach (CommissionDriverDTO lObjDriverDTO in pLstCommissionDriver)
            {
                List<CommissionDebtDTO> lLstDebt = new List<CommissionDebtDTO>();
                lLstDebt = GetCommissionDebt(mObjTxtFolio.Value, lObjDriverDTO.DriverId);
                lObjDriverDTO.ListDebt = lLstDebt;
                lObjDriverDTO.LstDisc = lLstDebt.Sum(x => x.Debit) - lLstDebt.Sum(x => x.Credit);
                lObjDriverDTO.LstDisc = lObjDriverDTO.LstDisc < 0 ? 0 : lObjDriverDTO.LstDisc;
                mObjProgressBar.NextPosition();
            }

        //Agrupar y realizar los calculos
            pLstCommissionDriver = pLstCommissionDriver.GroupBy(x => x.DriverId).Select(y => new CommissionDriverDTO
            {
                DriverId = y.First().DriverId,
                Driver = y.First().Driver,
                FrgAm = y.Sum(s => s.FrgAm),
                InsAm = y.Sum(s => s.InsAm),
                LstDisc = y.First().LstDisc, //Pendiente
                WkDisc = y.First().WkDisc,
                TotDisc = y.First().LstDisc + y.First().WkDisc,
                Comm = y.Sum(s => s.Comm) + pDblPayAdd,
                NoGenerate = y.First().NoGenerate,
                TotComm = y.Sum(s => s.Comm) - (y.First().LstDisc + y.First().WkDisc) + pDblPayAdd > 0 ? y.Sum(s => s.Comm) - y.First().LstDisc - y.First().WkDisc + pDblPayAdd : 0,
                Doubt = y.Sum(s => s.Comm) - (y.First().LstDisc + y.First().WkDisc) + pDblPayAdd < 0 ? (y.First().LstDisc + y.First().WkDisc) - y.Sum(s => s.Comm) - pDblPayAdd : 0,
            }).ToList();
            mObjProgressBar.NextPosition();
            mObjProgressBar.Dispose();

            return pLstCommissionDriver;
        }

        private string GetLastFolio(string pStrCurrentFolio, int lIntWeeksBef)
        {
           
            //Format WW-YY
            string lStrLastFolio = string.Empty;
            int lIntWeek = Convert.ToInt16(pStrCurrentFolio.Substring(0, 2));
            int lIntYear = Convert.ToInt16(pStrCurrentFolio.Substring(3, 2));

            lIntWeek = lIntWeek - lIntWeeksBef;
            if (lIntWeek  <= 0)
            {
                var lLstCmsn = mObjTransportsFactory.GetCommissionService().GetCmsnByYear(lIntYear - 1).Where(y => y.Status != 3).ToList();
                if (lLstCmsn != null && lLstCmsn.Count > 0)
                {
                    lStrLastFolio = lLstCmsn.OrderByDescending(x => x.Week).FirstOrDefault().Folio;
                }
                //lIntYear = lIntYear - 1;
                //lIntWeek
            }
            else
            {
                lStrLastFolio = lIntWeek.ToString("00") + "-" + lIntYear.ToString("00"); ;
            }
                return lStrLastFolio;


            //List<Commissions> lLstCmsn= mObjTransportsFactory.GetCommissionService().GetCommissionByFolio(pStrCurrentFolio).ToList();
            //List<Commissions> lLstCmsnLast = new List<Commissions>();
            //if (lLstCmsn.Count > 0)
            //{
            //    int lIntRowCode = Convert.ToInt16(lLstCmsn[0].RowCode) - lIntWeeksBef;
            //    lLstCmsnLast = mObjTransportsFactory.GetCommissionService().GetCommissionByRowCode(lIntRowCode.ToString()).ToList();
            //}
            //else
            //{
            //    int lIntRowcode = Convert.ToInt32(mObjTransportsFactory.GetCommissionService().GetLastCommissionId()) - lIntWeeksBef;
            //    lLstCmsnLast = mObjTransportsFactory.GetCommissionService().GetCommissionByRowCode(lIntRowcode.ToString()).ToList();
            //}

            //if (lLstCmsnLast.Count > 0)
            //{
            //    lStrFolio = lLstCmsnLast.First().Folio;
            //}

            ////Liquidacion cancelada
            //if (lLstCmsnLast.First().Status == 3)
            //{
            //    lStrFolio = "";
            //}

            //return lStrFolio;

          
        }

        private List<CommissionDriverDTO> GetListDrivers()
        {
            return mObjTransportsFactory.GetCommissionService().GetListDrivers();
        }

        private List<CommissionDriverDTO> GetComisssionDriverList(List<CommissionDriverDTO> pLstCmsnDrv)
        {
            mDtMatrixDrv = this.UIAPIRawForm.DataSources.DataTables.Item("Dt_Driv");
            mDtMatrixDrv.Rows.Clear();
            mLstCmsnDriverDTO = pLstCmsnDrv;// mObjTransportsFactory.GetCommissionService().GetCommissionDriver(mDtmFirstDay.ToString("yyyyMMdd"), mDtmLastDay.ToString("yyyyMMdd"));
            mLstCmsnDriverDTO.AddRange(GetListDrivers().Where(d => !mLstCmsnDriverDTO.Any(x => x.DriverId == d.DriverId)));
            
            List<CommissionDriverDTO> lLstCommisssionDriverDTO = new List<CommissionDriverDTO>();
            lLstCommisssionDriverDTO = (mLstCmsnDriverDTO as IEnumerable<CommissionDriverDTO>).ToList();

            var ss = mEnumStatus;

            lLstCommisssionDriverDTO = lLstCommisssionDriverDTO.GroupBy(x => x.DriverId).Select(y => new CommissionDriverDTO
            {
                DriverId = y.First().DriverId,
                Driver = y.First().Driver,
                FrgAm = y.Sum(s => s.FrgAm),
                InsAm = y.Sum(s => s.InsAm),
                LstDisc = y.First().LstDisc, //Pendiente
                WkDisc = y.First().WkDisc,
                TotDisc = y.First().LstDisc + y.First().WkDisc,
              //
                Comm = y.Sum(s => s.Comm),
                TotComm = y.Sum(s => s.Comm) - (y.First().LstDisc + y.First().WkDisc)  > 0 ? y.Sum(s => s.Comm) - (y.First().LstDisc + y.First().WkDisc)  : 0,
                Doubt = y.Sum(s => s.Comm) - (y.First().LstDisc + y.First().WkDisc)  < 0 ? (y.First().LstDisc + y.First().WkDisc) - y.Sum(s => s.Comm)  : 0,
                NoGenerate = y.First().NoGenerate,
            }).OrderBy(z => Convert.ToInt32(z.DriverId)).ToList();


            //foreach (var item in lLstCommisssionDriverDTO)
            //{
               

            //}


            return lLstCommisssionDriverDTO;
        }

        private void AddTotalLine(int i, List<CommissionDriverDTO> pLstCommisssionDriverDTO)
        {
           
            mDtMatrixDrv.SetValue("#", i, i + 1);
            mDtMatrixDrv.SetValue("cDrvr", i, "TOTAL");
            mDtMatrixDrv.SetValue("cDrvrId", i, "");
            mDtMatrixDrv.SetValue("cFrgAm", i, pLstCommisssionDriverDTO.Where(y => y.NoGenerate == false).Sum(x => x.FrgAm));
            mDtMatrixDrv.SetValue("cInsAm", i, pLstCommisssionDriverDTO.Where(y => y.NoGenerate == false).Sum(x => x.InsAm));
            mDtMatrixDrv.SetValue("cLstDisc", i, pLstCommisssionDriverDTO.Where(y => y.NoGenerate == false).Sum(x => x.LstDisc));
            mDtMatrixDrv.SetValue("cWkDisc", i, pLstCommisssionDriverDTO.Where(y => y.NoGenerate == false).Sum(x => x.WkDisc));
            mDtMatrixDrv.SetValue("cTotDisc", i, pLstCommisssionDriverDTO.Where(y => y.NoGenerate == false).Sum(x => x.TotDisc));
            mDtMatrixDrv.SetValue("cComm", i, pLstCommisssionDriverDTO.Where(y => y.NoGenerate == false).Sum(x => x.Comm));
            mDtMatrixDrv.SetValue("cTotComm", i, pLstCommisssionDriverDTO.Where(y => y.NoGenerate == false).Sum(x => x.TotComm));
            mDtMatrixDrv.SetValue("cDoubt", i, pLstCommisssionDriverDTO.Where(y => y.NoGenerate == false).Sum(x => x.Doubt));
           
        }

        private void BindMatrixDriv()
        {
            mObjMtxDrv.Columns.Item("#").DataBind.Bind("Dt_Driv", "#");
            mObjMtxDrv.Columns.Item("cDrvId").DataBind.Bind("Dt_Driv", "cDrvrId");
            mObjMtxDrv.Columns.Item("cDrvr").DataBind.Bind("Dt_Driv", "cDrvr");
            mObjMtxDrv.Columns.Item("cFrgAm").DataBind.Bind("Dt_Driv", "cFrgAm");
            mObjMtxDrv.Columns.Item("cInsAm").DataBind.Bind("Dt_Driv", "cInsAm");
            mObjMtxDrv.Columns.Item("cLstDisc").DataBind.Bind("Dt_Driv", "cLstDisc");
            mObjMtxDrv.Columns.Item("cWkDisc").DataBind.Bind("Dt_Driv", "cWkDisc");
            mObjMtxDrv.Columns.Item("cTotDisc").DataBind.Bind("Dt_Driv", "cTotDisc");
            mObjMtxDrv.Columns.Item("cComm").DataBind.Bind("Dt_Driv", "cComm");
            mObjMtxDrv.Columns.Item("cTotComm").DataBind.Bind("Dt_Driv", "cTotComm");
            mObjMtxDrv.Columns.Item("cDoubt").DataBind.Bind("Dt_Driv", "cDoubt");
            mObjMtxDrv.Columns.Item("cGenerate").DataBind.Bind("Dt_Driv", "cGenerate");
        }

        private void BindMatrixInv()
        {
            mObjMtxCommission.Columns.Item("#").DataBind.Bind("Dt_Cmsn", "#");
            mObjMtxCommission.Columns.Item("cDate").DataBind.Bind("Dt_Cmsn", "cDate");
            mObjMtxCommission.Columns.Item("cInvFol").DataBind.Bind("Dt_Cmsn", "cInvFol");
            mObjMtxCommission.Columns.Item("cOpType").DataBind.Bind("Dt_Cmsn", "cOpType");
            mObjMtxCommission.Columns.Item("cRoute").DataBind.Bind("Dt_Cmsn", "cRoute");
            mObjMtxCommission.Columns.Item("cVcle").DataBind.Bind("Dt_Cmsn", "cVcle");
            mObjMtxCommission.Columns.Item("cPyld").DataBind.Bind("Dt_Cmsn", "cPyld");
            mObjMtxCommission.Columns.Item("cAmnt").DataBind.Bind("Dt_Cmsn", "cAmnt");
            mObjMtxCommission.Columns.Item("cIns").DataBind.Bind("Dt_Cmsn", "cIns");
            mObjMtxCommission.Columns.Item("cCmsn").DataBind.Bind("Dt_Cmsn", "cCmsn");
        }

        private void LoadDtDivInfo(List<CommissionDriverDTO> lLstCommisssionDriverDTO)
        {
            int i = 0;
            mObjProgressBar = new ProgressBarManager(UIApplication.GetApplication(), "Consultando comisiones", lLstCommisssionDriverDTO.Count + 2);
            foreach (CommissionDriverDTO lObjCommissionDrvierDTO in lLstCommisssionDriverDTO)
            {
                mDtMatrixDrv.Rows.Add();
                mDtMatrixDrv.SetValue("#", i, i + 1);
                mDtMatrixDrv.SetValue("cDrvr", i, lObjCommissionDrvierDTO.Driver);
                mDtMatrixDrv.SetValue("cDrvrId", i, lObjCommissionDrvierDTO.DriverId);
                mDtMatrixDrv.SetValue("cFrgAm", i, lObjCommissionDrvierDTO.FrgAm);
                mDtMatrixDrv.SetValue("cInsAm", i, lObjCommissionDrvierDTO.InsAm);
                mDtMatrixDrv.SetValue("cLstDisc", i, lObjCommissionDrvierDTO.LstDisc);
                mDtMatrixDrv.SetValue("cWkDisc", i, lObjCommissionDrvierDTO.WkDisc);
                mDtMatrixDrv.SetValue("cTotDisc", i, lObjCommissionDrvierDTO.TotDisc);
                mDtMatrixDrv.SetValue("cComm", i, lObjCommissionDrvierDTO.Comm);
                mDtMatrixDrv.SetValue("cTotComm", i, lObjCommissionDrvierDTO.TotComm);
                mDtMatrixDrv.SetValue("cDoubt", i, lObjCommissionDrvierDTO.Doubt);
                mDtMatrixDrv.SetValue("cGenerate", i, lObjCommissionDrvierDTO.NoGenerate == false ? "N": "Y"); //
                i++;
                mObjProgressBar.NextPosition();
            }
            mDtMatrixDrv.Rows.Add();
            AddTotalLine(i, lLstCommisssionDriverDTO);
            mObjProgressBar.NextPosition();
            mObjMtxDrv.LoadFromDataSource();
            mObjMtxDrv.AutoResizeColumns();
            mObjProgressBar.Dispose();
        }

        private void LoadDtCommissionInfo()
        {
            mDtMatrixCmsn = this.UIAPIRawForm.DataSources.DataTables.Item("Dt_Cmsn");
            string lStrDriverId = mDtMatrixDrv.GetValue("cDrvrId", mIntSelectedRow - 1).ToString();
            List<CommissionDriverDTO> lLstCommisssionDTO = mLstCmsnDriverDTO.Where(x => x.DriverId == lStrDriverId).ToList(); //mObjTransportsFactory.GetCommissionService().GetCommission(lStrDriverId, mDtmFirstDay.ToString("yyyyMMdd"), mDtmLastDay.ToString("yyyyMMdd"));
            int i = 0;
            mObjProgressBar = new ProgressBarManager(UIApplication.GetApplication(), "Consultando comisiones", lLstCommisssionDTO.Count + 1);
            mDtMatrixCmsn.Rows.Clear();
            if (lLstCommisssionDTO.Count > 0 && string.IsNullOrEmpty(lLstCommisssionDTO[0].DocNum))
            {
                lLstCommisssionDTO = mObjTransportsFactory.GetCommissionService().GetCommissionDriverLine(mObjTxtFolio.Value).Where(x => x.DriverId == lStrDriverId).ToList();
            }
            foreach (CommissionDriverDTO lObjCmsnDTO in lLstCommisssionDTO)
            {
                if (!string.IsNullOrEmpty(lObjCmsnDTO.DocNum))
                {
                    mDtMatrixCmsn.Rows.Add();
                    mDtMatrixCmsn.SetValue("#", i, i + 1);
                    mDtMatrixCmsn.SetValue("cDate", i, lObjCmsnDTO.DocDate);
                    mDtMatrixCmsn.SetValue("cInvFol", i, lObjCmsnDTO.DocNum);
                    mDtMatrixCmsn.SetValue("cOpType", i, lObjCmsnDTO.Type == "INV" ? "FE" : "FI");
                    mDtMatrixCmsn.SetValue("cRoute", i, lObjCmsnDTO.Route);
                    mDtMatrixCmsn.SetValue("cVcle", i, lObjCmsnDTO.AF);
                    mDtMatrixCmsn.SetValue("cPyld", i, lObjCmsnDTO.TypLoad);
                    mDtMatrixCmsn.SetValue("cAmnt", i, lObjCmsnDTO.FrgAm);
                    mDtMatrixCmsn.SetValue("cIns", i, lObjCmsnDTO.InsAm);
                    mDtMatrixCmsn.SetValue("cCmsn", i, lObjCmsnDTO.Comm);
                    mDtMatrixCmsn.SetValue("cDocEntry", i, lObjCmsnDTO.Id);
                }

                i++;
                mObjProgressBar.NextPosition();
            }
            mObjMtxCommission.LoadFromDataSource();
            mObjProgressBar.NextPosition();
        }
       

        #endregion

        #region Save


        private bool SaveCommission()
        {
            bool lBolSuccess = false;
             Commissions lObjCommissionHeader =  GetCommisionHeader(mObjTxtFolio.Value);
            

             if (lObjCommissionHeader == null || lObjCommissionHeader.Status == (int) StatusEnum.CANCELED)
             {
                
                 Commissions lObjCommission = GetCommissionData();
                 lObjCommission.Status = (int)StatusEnum.OPEN;
                 lObjCommission.HasDriverCms = "N";
                
                      //Guardado de encabezado
                 if (mObjTransportsFactory.GetCommissionService().AddCommission(lObjCommission) == 0)
                 {
                     List<CommissionsRows> lObjCommisssionRows = GetcommissionRows(GetCommissionDriverMatrix());
                     string lStrCommissionId = mObjTransportsFactory.GetCommissionService().GetLastCommissionId();
                     //Guardado de rows
                     if (SaveCmsnRow(lObjCommisssionRows, lStrCommissionId))
                     {
                         //Guardado de lineas
                         mObjProgressBar = new ProgressBarManager(UIApplication.GetApplication(), "Guardando comisiones", lObjCommission.LstCommissionLine.Count);
                        
                         bool lBolsuccessLine = true;
                         foreach (CommissionLine lObjCommisionLine in lObjCommission.LstCommissionLine.Where(x => x.NoGenerate == false && x.Amount > 0))
                         {
                             lObjCommisionLine.CommisionId = lStrCommissionId;
                             if (mObjTransportsFactory.GetCommissionLineService().AddCommissionLine(lObjCommisionLine) != 0)
                             {
                                 lBolsuccessLine = false;
                             }
                             mObjProgressBar.NextPosition();
                         }
                         mObjProgressBar.NextPosition();

                         lBolSuccess = lBolsuccessLine;
                     }
                     else
                     {
                         lBolSuccess = false;
                     }
                 }
                 else
                 {
                     lBolSuccess = false;
                 }

                 mObjProgressBar.Dispose();


                 if (lBolSuccess)
                 {
                     //Creacion de alerta
                     CreateAlertArea(lObjCommission);
                 }
             }
             if (lObjCommissionHeader != null)
             {
                 if (lObjCommissionHeader.Status == (int)StatusEnum.OPEN && lObjCommissionHeader.AutBanks && lObjCommissionHeader.AutOperations && lObjCommissionHeader.AutTrans)
                 {
                     //Guardado de asiento
                     lBolSuccess = GenerateJournalEntry();
                     if (lBolSuccess)
                     {
                         lObjCommissionHeader.Status = (int)StatusEnum.CLOSED;
                         lBolSuccess = UpdateCommission(lObjCommissionHeader);
                     }
                 }
                 else
                 {
                     if (lObjCommissionHeader.Status == (int)StatusEnum.OPEN)
                     {
                         mObjProgressBar.Dispose();
                         UIApplication.ShowMessageBox("Falta autorizaciones");
                         LogService.WriteError("Falta autorizaciones");
                     }
                 }
             }
            return lBolSuccess;
        }

        private bool SaveCmsnRow(List<CommissionsRows> pLstCmsnRow, string pStrCmsnId)
        {
            bool lBolSuccessRow = true;
            foreach (CommissionsRows lObjCmsnRow in pLstCmsnRow)
            {
                lObjCmsnRow.Id = pStrCmsnId;
                if (mObjTransportsFactory.GetCmsnRowService().AddCmsnRow(lObjCmsnRow) != 0)
                {
                    lBolSuccessRow = false;
                }
               
            }
            return lBolSuccessRow;
        }

        private bool GenerateJournalEntry()
        {
            bool lBolSuccess = false;
            //Guardando asientos
            AccountsJournalEntryDTO lObjAccount = GetAcountDTO();
            List<CommissionDriverDTO> lLstCmsDriverDTO = GetCommissionDriverMatrix().Where(x => x.NoGenerate == false).ToList();
            List<JournalLineDTO> lLstJournalLine = new List<JournalLineDTO>();
            mObjProgressBar = new ProgressBarManager(UIApplication.GetApplication(), "Guardando asientos", lLstCmsDriverDTO.Count + 1);
            foreach (CommissionDriverDTO lObjCommisionLine in lLstCmsDriverDTO)
            {
                lObjCommisionLine.ListDebt = GetCommissionDebt(lObjCommisionLine.Folio, lObjCommisionLine.DriverId);
                JournalEntryLine lObjJournalEntryLine = new JournalEntryLine();
               // lObjCommisionLine.AF = 
                lLstJournalLine.AddRange(lObjJournalEntryLine.CreateJournalEntryLine(lObjCommisionLine, lObjAccount));

                //List<JournalLineDTO> lLstJournalLine2 = CreateJournalEntryLine(lObjCommisionLine, lObjAccount);
                //lBolSuccess = mObjTransportsFactory.GetJournalService().CreateNewJournal(lLstJournalLine2, mObjTxtFolio.Value, "TRCM", "");

                mObjProgressBar.NextPosition();
            }

            string lStrDate = this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_Date").Value.ToString();
            DateTime DtmDate = DateTime.Now;
            if (!string.IsNullOrEmpty(lStrDate))
            {
                DtmDate = Convert.ToDateTime(lStrDate);
            }

            lBolSuccess = mObjTransportsFactory.GetJournalService().CreateNewJournal(lLstJournalLine, mObjTxtFolio.Value, "TRCM", mObjTxtCmnt.Value, DtmDate);
            mObjProgressBar.NextPosition();
            mObjProgressBar.Dispose();
            return lBolSuccess;
        }

        private AccountsJournalEntryDTO GetAcountDTO()
        {
            AccountsJournalEntryDTO lObjAcc = new AccountsJournalEntryDTO();
            lObjAcc.Tope = Convert.ToDecimal(mObjTransportsFactory.GetCommissionService().GetAccountConfig("TR_LIQ_TOPE"));// 1169;
            lObjAcc.AccountViat = mObjTransportsFactory.GetCommissionService().GetAccountConfig("TR_ACC_VIAT");// "1010010000000"; //NO SE ENCONTRO
            lObjAcc.AccountRepMen = mObjTransportsFactory.GetCommissionService().GetAccountConfig("TR_ACC_REPMEN");// "5010340000000";
            lObjAcc.AccountLiquid = mObjTransportsFactory.GetCommissionService().GetAccountConfig("TR_ACC_LIQCHOF");// "2110020000000";
            lObjAcc.AccountFuncEmpl = mObjTransportsFactory.GetCommissionService().GetAccountConfig("TR_ACC_FUNCEMPL");// "1070010000000";
            return lObjAcc;
        }

        //Obtener datos de la liquidacion de la forma
        private Commissions GetCommissionData()
        {
            List<CommissionLine> lLstComissionList = GetCommissionsLineData();
            Commissions lObjCommission = new Commissions();
            lObjCommission.Amount = Convert.ToDouble(lLstComissionList.Sum(x => Convert.ToDecimal(x.Amount)));
            lObjCommission.Folio = lLstComissionList.First().Folio;
            lObjCommission.Status = (int)StatusEnum.OPEN;
            lObjCommission.User = DIApplication.Company.UserName;
            lObjCommission.LstCommissionLine = lLstComissionList;
            lObjCommission.Coments = mObjTxtCmnt.Value;

            int lIntWeek = Convert.ToInt16(lObjCommission.Folio.Substring(0, 2));
            int lIntYear = Convert.ToInt16(lObjCommission.Folio.Substring(3, 2));
            lObjCommission.Year = lIntYear;
            lObjCommission.Week = lIntWeek;
            lObjCommission.PaymClean = chkAdd.Checked ? true : false;
            lObjCommission.StartDate = mDtmFirstDay;
            lObjCommission.EndDate = mDtmLastDay;
            return lObjCommission;
        }

        //Obtener datos por linea
        private List<CommissionLine> GetCommissionsLineData()
        {
            List<CommissionLine> lLstCommissionsLine = new List<CommissionLine>();
            ///variable global
            /// comparacion de la matriz guardada y la seleccion de checkbox
            List<CommissionDriverDTO> lLstCommisssionDriverDTO = (mLstCmsnDriverDTO as IEnumerable<CommissionDriverDTO>).ToList();
            List<CommissionDriverDTO> lLstMatrix = GetCommissionDriverMatrix();


            foreach (var item in lLstCommisssionDriverDTO)
            {
                int i = lLstMatrix.Where(x => x.DriverId == item.DriverId).ToList().Count();
                
                item.NoGenerate = lLstMatrix.Where(x => x.DriverId == item.DriverId).First().NoGenerate;
            }
           

            //mObjTransportsFactory.GetCommissionService().GetCommissionDriver(mDtmFirstDay.ToString("yyyyMMdd"), mDtmLastDay.ToString("yyyyMMdd"));

            foreach (CommissionDriverDTO lObjLine in lLstCommisssionDriverDTO)
            {
                CommissionLine lObjCommisionLine = new CommissionLine();
                lObjCommisionLine.DriverId = lObjLine.DriverId;
                lObjCommisionLine.Folio = mStrFolio;
                lObjCommisionLine.DocEntry = lObjLine.Id;
                lObjCommisionLine.Amount = lObjLine.FrgAm;
                lObjCommisionLine.NoGenerate = lObjLine.NoGenerate;
                lObjCommisionLine.Type = lObjLine.Type;
                lObjCommisionLine.CmsnAmount = lObjLine.Comm;
                lLstCommissionsLine.Add(lObjCommisionLine);
            }
            return lLstCommissionsLine;
        }

        //Obtener los datos de la matriz
        private List<CommissionDriverDTO> GetCommissionDriverMatrix()
        {
            List<CommissionDriverDTO> lLstCommissionDriver = new List<CommissionDriverDTO>();
            mDtMatrixDrv = this.UIAPIRawForm.DataSources.DataTables.Item("Dt_Driv");

            for (int i = 0; i < mDtMatrixDrv.Rows.Count; i++)
            {
                string ss = mDtMatrixDrv.GetValue("cGenerate", i).ToString();
                bool ssef = ((SAPbouiCOM.CheckBox)mObjMtxDrv.Columns.Item("cGenerate").Cells.Item(i + 1).Specific).Checked;
                //if (mDtMatrixDrv.GetValue("cGenerate", i).ToString() == "N")

                CommissionDriverDTO lObjCommissionDriver = new CommissionDriverDTO();
                lObjCommissionDriver.Folio = mObjTxtFolio.Value;
                lObjCommissionDriver.Driver = mDtMatrixDrv.GetValue("cDrvr", i).ToString(); //
                lObjCommissionDriver.DriverId = mDtMatrixDrv.GetValue("cDrvrId", i).ToString();
                lObjCommissionDriver.FrgAm = Convert.ToDouble(mDtMatrixDrv.GetValue("cFrgAm", i).ToString());
                lObjCommissionDriver.InsAm = Convert.ToDouble(mDtMatrixDrv.GetValue("cInsAm", i).ToString());
                lObjCommissionDriver.LstDisc = Convert.ToDouble(mDtMatrixDrv.GetValue("cLstDisc", i).ToString());
                lObjCommissionDriver.WkDisc = Convert.ToDouble(mDtMatrixDrv.GetValue("cWkDisc", i).ToString());
                lObjCommissionDriver.TotDisc = Convert.ToDouble(mDtMatrixDrv.GetValue("cTotDisc", i).ToString());
                lObjCommissionDriver.Comm = Convert.ToDouble(mDtMatrixDrv.GetValue("cComm", i).ToString());
                lObjCommissionDriver.TotComm = Convert.ToDouble(mDtMatrixDrv.GetValue("cTotComm", i).ToString());
                lObjCommissionDriver.Doubt = Convert.ToDouble(mDtMatrixDrv.GetValue("cDoubt", i).ToString());
                lObjCommissionDriver.NoGenerate = ((SAPbouiCOM.CheckBox)mObjMtxDrv.Columns.Item("cGenerate").Cells.Item(i + 1).Specific).Checked;

                if (lObjCommissionDriver.Driver != "TOTAL")
                {
                    lLstCommissionDriver.Add(lObjCommissionDriver);
                }

            }
            return lLstCommissionDriver;
        }

        //Consulta la deuda de la semana anterior
        private List<CommissionDebtDTO> GetCommissionDebt(string pStrFolio, string pStrAux)
        {
            List<CommissionDebtDTO> lLstDebt = new List<CommissionDebtDTO>();
            int i = 0;
            List<CommissionDebtDTO> lLstCommissionDebt;
            do
            {
                i++;
                lLstCommissionDebt = new List<CommissionDebtDTO>();
                string lStrLastFolio = GetLastFolio(pStrFolio, i);
                if (!string.IsNullOrEmpty(lStrLastFolio))
                {
                    string lStrAccountFunEmp = mObjTransportsFactory.GetCommissionService().GetAccountConfig("TR_ACC_FUNCEMPL");
                   
                    lLstCommissionDebt = mObjTransportsFactory.GetCommissionService().GetCommissionDebtDTO(lStrLastFolio, lStrAccountFunEmp, "2", pStrAux);
                    if (lLstCommissionDebt.Sum(x => x.Debit) > lLstCommissionDebt.Sum(y => y.Credit))
                    {
                        CommissionDebtDTO lObjCmsDebt = new CommissionDebtDTO();
                        lObjCmsDebt.Id = i;
                        lObjCmsDebt.Auxiliar = pStrAux;
                        lObjCmsDebt.Credit = 0;
                        lObjCmsDebt.Debit = Convert.ToDouble(Convert.ToDecimal(lLstCommissionDebt.Sum(x => x.Debit)) - Convert.ToDecimal(lLstCommissionDebt.Sum(y => y.Credit)));
                        lObjCmsDebt.Folio = lStrLastFolio;
                        lObjCmsDebt.Importe = Convert.ToDecimal(lLstCommissionDebt.Sum(x => x.Debit)) - Convert.ToDecimal(lLstCommissionDebt.Sum(y => y.Credit));
                        lLstDebt.Add(lObjCmsDebt);
                    }
                }
               
            } while (lLstCommissionDebt.Sum(x => x.Debit) > lLstCommissionDebt.Sum(y => y.Credit));
            return lLstDebt;
        }

        //Creacion de lineas de asiento por algoritmo
        //private List<JournalLineDTO> CreateJournalEntryLine(CommissionDriverDTO pObjCmsnDriverDTO, AccountsJournalEntryDTO pObjAccounts)
        //{
        //    pObjCmsnDriverDTO.ListDebt = GetCommissionDebt(pObjCmsnDriverDTO.Folio, pObjCmsnDriverDTO.DriverId);
        //    JournalEntryLine lObjJournalEntryLine = new JournalEntryLine();
        //    lObjJournalEntryLine.CreateJournalEntryLine(pObjCmsnDriverDTO, pObjAccounts);

        //    List<JournalLineDTO> lLstJounralLineDTO = new List<JournalLineDTO>();

        //    decimal lDecTotal = Convert.ToDecimal(pObjCmsnDriverDTO.TotComm) - Convert.ToDecimal(pObjCmsnDriverDTO.Doubt);
        //    decimal lDecDeuda = Convert.ToDecimal(pObjCmsnDriverDTO.LstDisc);
        //    decimal lDecDeuSem = Convert.ToDecimal(pObjCmsnDriverDTO.WkDisc);
        //    decimal lDecComision = Convert.ToDecimal(pObjCmsnDriverDTO.Comm);
        //    decimal lDecRemanente = 0;
        //    decimal lDecSemD = 0;

        //    if (lDecDeuda > 0 && lDecComision > 0)
        //    {
        //        lDecRemanente = lDecComision;
                
        //        foreach (CommissionDebtDTO DebtDTO in pObjCmsnDriverDTO.ListDebt)
        //        {
        //            if (lDecRemanente <= 0)
        //            {
        //                break;
        //            }
        //            else
        //            {
        //                //if (DebtDTO.Importe <= lDecRemanente)
        //                if (DebtDTO.Importe > lDecRemanente)
        //                {
        //                    lLstJounralLineDTO.Add(new JournalLineDTO
        //                    {
        //                        AccountCode = pObjAccounts.AccountFuncEmpl,
        //                        Debit = 0,
        //                        Credit = Convert.ToDouble(DebtDTO.Importe - lDecRemanente),
        //                        TypeAux = "2",
        //                        Auxiliar = pObjCmsnDriverDTO.DriverId,
        //                        Ref1 = DebtDTO.Folio
        //                    });
        //                    lDecRemanente -= DebtDTO.Importe;
        //                }
        //                else
        //                {
        //                    lLstJounralLineDTO.Add(new JournalLineDTO
        //                    {
        //                        AccountCode = pObjAccounts.AccountFuncEmpl,
        //                        Debit = 0,
        //                        Credit = Convert.ToDouble(DebtDTO.Importe),//Convert.ToDouble(lDecRemanente),
        //                        Ref1 = DebtDTO.Folio,
        //                        TypeAux = "2",
        //                        Auxiliar = pObjCmsnDriverDTO.DriverId
        //                    });
        //                    lDecRemanente = 0;
        //                }

        //            }
        //        }
        //    }

        //    if (lDecTotal > 0)
        //    {
        //        lLstJounralLineDTO.Add(new JournalLineDTO
        //        {
        //            AccountCode = pObjAccounts.AccountLiquid,
        //            Debit = 0,
        //            Credit = Convert.ToDouble(lDecTotal),
        //            Ref1 = pObjCmsnDriverDTO.Folio,
        //            TypeAux = "2",
        //            Auxiliar = pObjCmsnDriverDTO.DriverId
        //        });
        //    }
        //    else if (lDecDeuda == 0)
        //    {
        //        lDecSemD = lDecDeuSem - lDecRemanente;

        //        lLstJounralLineDTO.Add(new JournalLineDTO
        //        {
        //            AccountCode = pObjAccounts.AccountFuncEmpl,
        //            Debit = Convert.ToDouble(lDecSemD),
        //            Credit = 0,
        //            Ref1 = pObjCmsnDriverDTO.Folio,
        //            TypeAux = "2",
        //            Auxiliar = pObjCmsnDriverDTO.DriverId
        //        });
        //        // lDecTotal = lDecTotal - 
        //    }
        //    else
        //    {
        //        decimal lDec = lDecTotal < 0 ? -1 : 1;
        //        lLstJounralLineDTO.Add(new JournalLineDTO
        //        {
        //            AccountCode = pObjAccounts.AccountFuncEmpl,
        //            Debit = Convert.ToDouble(lDecTotal * lDec + lDecRemanente),
        //            Credit = 0,
        //            Ref1 = pObjCmsnDriverDTO.Folio,
        //            TypeAux = "2",
        //            Auxiliar = pObjCmsnDriverDTO.DriverId
        //        });
        //    }

        //    lDecTotal = Convert.ToDecimal(lLstJounralLineDTO.Sum(x => x.Credit)) - Convert.ToDecimal(lLstJounralLineDTO.Sum(x => x.Debit));

        //    if (lDecTotal >= pObjAccounts.Tope || lDecTotal <= pObjAccounts.Tope * -1)
        //    {
        //        decimal lDec = lDecTotal < 0 ? -1 : 1;

        //        lLstJounralLineDTO.Add(new JournalLineDTO
        //        {
        //            AccountCode = pObjAccounts.AccountViat,
        //            Debit = Convert.ToDouble(pObjAccounts.Tope * lDec),
        //            Credit = 0,
        //            Ref1 = pObjCmsnDriverDTO.Folio,
        //            TypeAux = "2",
        //            Auxiliar = pObjCmsnDriverDTO.DriverId
        //        });

        //        lLstJounralLineDTO.Add(new JournalLineDTO
        //        {
        //            AccountCode = pObjAccounts.AccountRepMen,
        //            Debit = Convert.ToDouble((lDecTotal) - (pObjAccounts.Tope * lDec)),
        //            Credit = 0,
        //            Ref1 = pObjCmsnDriverDTO.Folio,
        //            TypeAux = "2",
        //            Auxiliar = pObjCmsnDriverDTO.DriverId
        //        });
        //    }
        //    else
        //    {
        //        decimal lDec = lDecTotal < 0 ? -1 : 1;
        //        lLstJounralLineDTO.Add(new JournalLineDTO
        //        {
        //            AccountCode = pObjAccounts.AccountViat,
        //            Debit = Convert.ToDouble(lDecTotal),
        //            Credit = 0,
        //            Ref1 = pObjCmsnDriverDTO.Folio,
        //            TypeAux = "2",
        //            Auxiliar = pObjCmsnDriverDTO.DriverId
        //        });
        //    }
        //    return lLstJounralLineDTO;
        //}
           
        
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
        
        //Cancelacion del asiento
        private bool CancelMovement()
        {
            bool lBolSuccess = false;
            Commissions lObjCommissionHeader = GetCommisionHeader(mObjTxtFolio.Value);

            int lIntStatus = lObjCommissionHeader.Status;
            lObjCommissionHeader.Status = (int)StatusEnum.CANCELED;
            lBolSuccess = UpdateCommission(lObjCommissionHeader);
            if (lBolSuccess && lIntStatus == (int)StatusEnum.CLOSED)
            {
                lBolSuccess = mObjTransportsFactory.GetJournalService().ReverseJournal(mObjTxtFolio.Value, "TRCM");
            }

            lObjCommissionHeader.Status = (int)StatusEnum.CANCELED;
            List<CommissionLine> lLstCmnsLine = mObjTransportsFactory.GetCommissionService().GetCommissionLine(lObjCommissionHeader.RowCode).ToList();
            if (lBolSuccess)
            {
                foreach (var CmsnLine in lLstCmnsLine)
                {
                    CmsnLine.Status = (int)StatusEnum.CANCELED;
                    if (!UpdateCommissionLine(CmsnLine))
                    {
                        lBolSuccess = false;
                        break;
                    }
                }
            }
            return lBolSuccess;
        }
        #endregion
       
        #region Alerts
        private void CreateAlertArea(Commissions pObjCommission)
        {
            MessageDTO lObjMsgDTO = new MessageDTO();
            if (!pObjCommission.AutTrans)
            {
                List<string> lLstUsers = GetUsers("TR_AU_Area");
                if (lLstUsers.Count > 0)
                {
                    lObjMsgDTO.Message = string.Format("Transportes: solicitud de aprobación de comisiones de choferes con folio: {0}", pObjCommission.Folio); //Get Message
                    lObjMsgDTO.UserCode = lLstUsers;
                    mObjTransportsFactory.GetAlertService().SaveAlert(lObjMsgDTO);
                }
            }
        }

        private void CreateAlertOperaciones(Commissions pObjCommission)
        {
            MessageDTO lObjMsgDTO = new MessageDTO();

            List<string> lLstUsers = GetUsers("TR_AU_OPE");
            if (lLstUsers.Count > 0)
            {
                lObjMsgDTO.Message = string.Format("Transportes: solicitud de aprobación de comisiones de choferes con folio: {0}", pObjCommission.Folio); //Get Message
                lObjMsgDTO.UserCode = lLstUsers;
                mObjTransportsFactory.GetAlertService().SaveAlert(lObjMsgDTO);
            }
        }

        private void CreateAlertFinanzas(Commissions pObjCommission)
        {
            MessageDTO lObjMsgDTO = new MessageDTO();

            List<string> lLstUsers = GetUsers("TR_AU_FZ");
            if (lLstUsers.Count > 0)
            {
                lObjMsgDTO.Message = string.Format("Transportes: solicitud de aprobación de comisiones de choferes con folio: {0}", pObjCommission.Folio); //Get Message
                lObjMsgDTO.UserCode = lLstUsers;
                mObjTransportsFactory.GetAlertService().SaveAlert(lObjMsgDTO);
            }
            if (pObjCommission != null && pObjCommission.Status == (int)StatusEnum.OPEN && pObjCommission.AutBanks && pObjCommission.AutOperations && pObjCommission.AutTrans)
            {
                CreateAlertAcept(pObjCommission);
            }
        }

        private void CreateAlertReject(Commissions pObjCommission)
        {
            MessageDTO lObjMsgDTO = new MessageDTO();


            List<string> lLstUsers = new List<string>();

            lLstUsers.Add(pObjCommission.User);
            if (lLstUsers.Count > 0)
            {
                lObjMsgDTO.Message = string.Format("Transportes: solicitud de comisiones de choferes rechazada: {0}", pObjCommission.Folio); //Get Message
                lObjMsgDTO.UserCode = lLstUsers;
                mObjTransportsFactory.GetAlertService().SaveAlert(lObjMsgDTO);
            }
        }

        private void CreateAlertAcept(Commissions pObjCommission)
        {
            MessageDTO lObjMsgDTO = new MessageDTO();
            List<string> lLstUsers = new List<string>();
            lLstUsers.Add(pObjCommission.User);
            if (lLstUsers.Count > 0)
            {
                lObjMsgDTO.Message = string.Format("Transportes: solicitud de comisiones de aprobada ya puede generar el folio: {0}", pObjCommission.Folio); //Get Message
                lObjMsgDTO.UserCode = lLstUsers;
                mObjTransportsFactory.GetAlertService().SaveAlert(lObjMsgDTO);
            }
        }

        private List<string> GetUsers(string pStrConfigName)
        {
            List<string> lLstUsers = new List<string>();
            lLstUsers = mObjTransportsFactory.GetCommissionService().GetAuthorizers(pStrConfigName);
            if (lLstUsers.Count > 0)
            {
                return lLstUsers;
            }
            else
            {
                LogService.WriteError("No se encontro la configuración " + pStrConfigName);
                UIApplication.ShowMessageBox("No se encontro la configuración " + pStrConfigName);
            }

            return lLstUsers;
        }

        private void SendAlertByAuthorizer(Commissions pObjCommissionHeader)
        {
            switch (mAuthorizerEnum)
            {
                case AuthorizerEnum.AutTrans:
                    CreateAlertOperaciones(pObjCommissionHeader);
                    break;
                case AuthorizerEnum.AutOperations:
                    CreateAlertFinanzas(pObjCommissionHeader);
                    break;

                case AuthorizerEnum.AutBanks:
                    break;
            }
        }

        #endregion

        #region Permmission

        /// <summary>
        /// Coloca visible o no visible los contorles de status.
        /// </summary>
        private void VisibleStatusControls(bool pBoolVisible)
        {
            lblAuFz.Item.Visible = pBoolVisible;
            lblAuOp.Item.Visible = pBoolVisible;
            mObjlblArea.Item.Visible = pBoolVisible;
            mObjTxtArea.Item.Visible = pBoolVisible;
            txtAuFz.Item.Visible = pBoolVisible;
            txtAuOp.Item.Visible = pBoolVisible;
        }


        /// <summary>
        /// Coloca visible o no visible los contorles de autorizacion.
        /// </summary>
        private void VisibleAuthControls(bool pBoolVisible)
        {
            mObjBtnAuth.Item.Visible = pBoolVisible;
            mObjBtnReject.Item.Visible = pBoolVisible;
        }


        /// <summary>
        /// Coloca el color del edittect.
        /// </summary>
        private void ColorAuth(EditText pTxtEditText )
        {
            pTxtEditText.BackColor = mIntColorGreen;
            pTxtEditText.ForeColor = mIntColorWhite;
            pTxtEditText.Value = "Autorizado";
        }

        private void ColorDefault(EditText pTxtEditText)
        {
            pTxtEditText.BackColor = mIntColorGray;
            pTxtEditText.ForeColor = 0;
            pTxtEditText.Value = "Pendiente";
        }

        /// <summary>
        /// Coloca el color del edittect.
        /// </summary>
        private void ColorReject(EditText pTxtEditText)
        {
            pTxtEditText.BackColor = mIntColorRed;
            pTxtEditText.ForeColor = mIntColorWhite;
            pTxtEditText.Value = "Rechazado";
        }

        /// <summary>
        /// </summary>
        private void SetStatusControls(Commissions pObjCommissions)
        {
          
            
            if (pObjCommissions.AutTrans)
            {
                ColorAuth(mObjTxtArea);
            }
            else
            {
                ColorDefault(mObjTxtArea);
            }

            if (pObjCommissions.AutOperations)
            {
               
                ColorAuth(txtAuOp);
            }
            else
            {
                ColorDefault(txtAuOp);
            }

            if (pObjCommissions.AutBanks)
            {
              
                ColorAuth(txtAuFz);
            }
            else
            {
                ColorDefault(txtAuFz);
            }

            if (pObjCommissions.Status == (int)StatusEnum.REJECT)
            {
                SetRejectStatus(pObjCommissions);
            }

        }

        private void SetRejectStatus(Commissions pObjCommissions)
        {
            if (!pObjCommissions.AutTrans)
            {
                ColorReject(mObjTxtArea);
            }
            else if (!pObjCommissions.AutOperations)
            {
                ColorReject(txtAuOp);
            }
            else if (!pObjCommissions.AutBanks)
            {
                ColorReject(txtAuFz);
            }
        }



        /// <summary>
        /// Establece el estatus de la comision
        /// </summary>
        private void SetTxtStatus(StatusEnum pObjStatus)
        {
            mEnumStatus = pObjStatus;
            mObjTxtStatus.Value = mEnumStatus.GetDescription();
        }

        /// <summary>
        /// Obtiene el permisos de autorizacion del usuario
        /// </summary>
        private void GetUserPermissionAuth()
        {
            string lStrUserAuth = mObjTransportsFactory.GetCommissionService().GetUSerAuthorization(DIApplication.Company.UserName);

            switch (lStrUserAuth)
            {
                case "TR_AU_AREA":
                    mAuthorizerEnum = AuthorizerEnum.AutTrans;
                    break;

                case "TR_AU_OPE":
                    mAuthorizerEnum = AuthorizerEnum.AutOperations;
                    break;

                case "TR_AU_FZ":
                    mAuthorizerEnum = AuthorizerEnum.AutBanks;
                    break;

                default:
                    mAuthorizerEnum = AuthorizerEnum.NoAut;
                    break;
            }
        }

        /// <summary>
        /// Establece el permiso de autorizacion del usuario
        /// </summary>
        private void SetControlsByPermissionUser(AuthorizerEnum pObjAuthEnum, Commissions pObjCommissions)
        {
            if (mAuthorizerEnum == AuthorizerEnum.NoAut)
            {
                VisibleAuthControls(false);
            }
            else
            {
                switch (pObjAuthEnum)
                {
                    case AuthorizerEnum.AutTrans:
                        if (!pObjCommissions.AutTrans && !pObjCommissions.AutOperations && !pObjCommissions.AutBanks)
                        {
                            VisibleAuthControls(true);
                        }
                        else
                        {
                            VisibleAuthControls(false);
                        }
                        break;

                    case AuthorizerEnum.AutOperations:
                        if (pObjCommissions.AutTrans && !pObjCommissions.AutOperations && !pObjCommissions.AutBanks)
                        {
                            VisibleAuthControls(true);
                        }
                        else
                        {
                            VisibleAuthControls(false);
                        }
                        break;

                    case AuthorizerEnum.AutBanks:
                        if (pObjCommissions.AutTrans && pObjCommissions.AutOperations && !pObjCommissions.AutBanks)
                        {
                            VisibleAuthControls(true);
                        }
                        else
                        {
                            VisibleAuthControls(false);
                        }
                        break;
                    default:
                        VisibleAuthControls(false);
                        break;
                }
            }
        }

        private bool UpdateCommission(Commissions pObjCommissionHeader)
        {
            bool lBolSuccess = false;
            if (mObjTransportsFactory.GetCommissionService().UpdateCommission(pObjCommissionHeader) == 0)
            {
                LogService.WriteSuccess(string.Format("UpdateCommission ok Folio: {0}, Estatus: {1}", pObjCommissionHeader.Folio, mAuthorizerEnum.GetDescription()));
                lBolSuccess = true;

            }
            else
            {
                string lStrError = DIApplication.Company.GetLastErrorDescription();
                LogService.WriteError(string.Format("UpdateCommission: {0}", lStrError));
                UIApplication.ShowMessageBox(lStrError);
                lBolSuccess = false;
            }

            return lBolSuccess;
        }

        private bool UpdateCommissionLine(CommissionLine pObjCommissionLine)
        {
            bool lBolSuccess = false;
            if (mObjTransportsFactory.GetCommissionLineService().UpdateCommissionLine(pObjCommissionLine) == 0)
            {
                LogService.WriteSuccess(string.Format("UpdateCommission ok Folio: {0}, Estatus: {1}", pObjCommissionLine.Folio, mAuthorizerEnum.GetDescription()));
                lBolSuccess = true;

            }
            else
            {
                string lStrError = DIApplication.Company.GetLastErrorDescription();
                LogService.WriteError(string.Format("UpdateCommission: {0}", lStrError));
                UIApplication.ShowMessageBox(lStrError);
                lBolSuccess = false;
            }

            return lBolSuccess;
        }


        private bool UpdateCommissionAuth(Commissions pObjCommissionHeader)
        {
            bool lBolIsUpdate = true;

            switch (mAuthorizerEnum)
            {
                case AuthorizerEnum.AutTrans:
                    if (!pObjCommissionHeader.AutTrans && !pObjCommissionHeader.AutOperations && !pObjCommissionHeader.AutBanks)
                    {
                        pObjCommissionHeader.AutTrans = true;
                    }
                    else
                    {
                        lBolIsUpdate = false;
                    }
                    break;
                case AuthorizerEnum.AutOperations:

                    if (pObjCommissionHeader.AutTrans && !pObjCommissionHeader.AutOperations && !pObjCommissionHeader.AutBanks)
                    {
                        pObjCommissionHeader.AutOperations = true;
                    }
                    else
                    {
                        lBolIsUpdate = false;
                    }
                    break;

                case AuthorizerEnum.AutBanks:

                    if (pObjCommissionHeader.AutTrans && pObjCommissionHeader.AutOperations && !pObjCommissionHeader.AutBanks)
                    {
                        pObjCommissionHeader.AutBanks = true;
                    }
                    else
                    {
                        lBolIsUpdate = false;
                    }
                    break;
            }
            if (lBolIsUpdate)
            {
                return UpdateCommission(pObjCommissionHeader);
            }

            return false;
        }

        #endregion

        private Commissions GetCommisionHeader(string pStrFolio)
        {

            IList<Commissions> lObjCommissions = mObjTransportsFactory.GetCommissionService().GetCommissionByFolio(pStrFolio);
            if (lObjCommissions.Count > 0)
            {
                return lObjCommissions.Last();
            }

            return null;
        }

        private List<CommissionsRows> GetcommissionRows(List<CommissionDriverDTO> pLstCommissionDriver)
        {
            List<CommissionsRows> lLstCmsRow = new List<CommissionsRows>();
            string lStrCommissionId = mObjTransportsFactory.GetCommissionService().GetLastCommissionId();
            foreach (var lObjCms in pLstCommissionDriver)
            {
                CommissionsRows lObjCmsRow = new CommissionsRows();
                lObjCmsRow.Id = lStrCommissionId;
                lObjCmsRow.Folio = lObjCms.Folio;
                lObjCmsRow.Driver = lObjCms.Driver;
                lObjCmsRow.DriverId = lObjCms.DriverId;
                lObjCmsRow.FrgAm = lObjCms.FrgAm;
                lObjCmsRow.InsAm = lObjCms.InsAm;
                lObjCmsRow.LstDisc = lObjCms.LstDisc;
                lObjCmsRow.WkDisc = lObjCms.WkDisc;
                lObjCmsRow.TotDisc = lObjCms.TotDisc;
                lObjCmsRow.Doubt = lObjCms.Doubt;
                lObjCmsRow.NoGenerate = lObjCms.NoGenerate;
                lObjCmsRow.DocDate = lObjCms.DocDate;
                lObjCmsRow.Comm = lObjCms.Comm;
                lObjCmsRow.TotComm = lObjCms.TotComm;
                lObjCmsRow.Doubt = lObjCms.Doubt;
                lLstCmsRow.Add(lObjCmsRow);
            }


            return lLstCmsRow;
        }

        private void LoadFirstDay(int pIntYear)
        {
            StartDay lObjStartDay = mObjTransportsFactory.GetCommissionService().GetFirstDay(pIntYear);
            if (lObjStartDay == null || string.IsNullOrEmpty(lObjStartDay.FirstDay.ToString()))
            {
                UIApplication.ShowMessageBox("Favor de agregar el primer día del año '"+pIntYear+"' a la tabla TR_Day");
                LogService.WriteError("Favor de agregar el primer día del año '" + pIntYear + "' a la tabla TR_Day");
            }
            else
            {
                mDtmFirstDay = new DateTime(lObjStartDay.Year, Convert.ToInt32(lObjStartDay.RowName) != lObjStartDay.Year ? 12 : 1, lObjStartDay.FirstDay);
                lblDates.Caption = " Fechas: Del " + mDtmFirstDay.ToString("dd/MM/yyyy") + " Al " + mDtmLastDay.ToString("dd/MM/yyyy");
            }
        }

       private string GetFolioWeek()
        {
            LoadFirstDay(DateTime.Now.Year);
            DateTime lDtmNow = DateTime.Now;
            int lIntQtyDays = (lDtmNow - mDtmFirstDay).Days;
            int lIntWeek = ((int)(lIntQtyDays) / 7) + 1;
            mDtmFirstDay = mDtmFirstDay.AddDays(((lIntWeek - 1) * 7));
            mDtmLastDay = mDtmFirstDay.AddDays(6);
         

            return lIntWeek.ToString("00") + "-" + DateTime.Now.ToString("yy");
        }

        #endregion
      
        #region Controls
        private SAPbouiCOM.StaticText mObjlblFolio;
        private SAPbouiCOM.EditText mObjTxtFolio;
        private SAPbouiCOM.Button mObjBtnSearch;
        private SAPbouiCOM.EditText mObjTxtStatus;
        private SAPbouiCOM.StaticText mObjlblStat;
        private SAPbouiCOM.EditText mObjTxtArea;
        private SAPbouiCOM.Button mObjBtnAuth;
        private SAPbouiCOM.Button mObjBtnReject;
        private SAPbouiCOM.StaticText mObjlblArea;
        private SAPbouiCOM.Matrix mObjMtxDrv;
        private SAPbouiCOM.Matrix mObjMtxCommission;
        private SAPbouiCOM.Button mObjBtnOk;
        private SAPbouiCOM.StaticText mObjlblCmnt;
        private SAPbouiCOM.EditText mObjTxtCmnt;
        private SAPbouiCOM.Button mObjBtnCancel;
        private SAPbouiCOM.DataTable mDtMatrixDrv = null;
        private SAPbouiCOM.DataTable mDtMatrixCmsn = null;
        private SAPbouiCOM.StaticText lblDates;
        private EditText txtAuOp;
        private StaticText lblAuOp;
        private StaticText lblAuFz;
        private EditText txtAuFz;
        //private EditText EditText2;
        //private Button Button0;
        private ComboBox cboWeek;
        private StaticText lblWeek;
        private StaticText lblYear;
        private ComboBox cboYear;
        private CheckBox chkAdd;
        private EditText txtDate;
        private StaticText lblDate;
        private UGRS.Core.SDK.UI.ProgressBar.ProgressBarManager mObjProgressBar = null;
        #endregion
      
       

      

     

      

       

      
      

     
       
       
    }
}
