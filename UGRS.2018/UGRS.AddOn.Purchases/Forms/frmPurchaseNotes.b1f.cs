using System;
using System.Collections.Generic;
using System.Linq;
using SAPbouiCOM.Framework;
using SAPbouiCOM;
using UGRS.Core.SDK.UI;
using UGRS.Core.SDK.DI.Purchases.DTO;
using UGRS.Core.SDK.DI.Purchases.Services;
using System.Threading;
using UGRS.Core.Utility;
using UGRS.Core.SDK.DI.Purchases.Enums;
using UGRS.Core.SDK.DI.Purchases;
using UGRS.Core.SDK.DI.Purchases.Tables;
using UGRS.Core.Services;
using UGRS.Core.SDK.DI;
using UGRS.AddOn.Purchases.Utilities;

namespace UGRS.AddOn.Purchases.Forms
{
    [FormAttribute("UGRS.AddOn.Purchases.Forms.frmPurchaseNotes", "Forms/frmPurchaseNotes.b1f")]
    class frmPurchaseNotes : UserFormBase
    {
        #region Properties
        PurchasesServiceFactory mObjPurchaseServiceFactory = new PurchasesServiceFactory();

        int mIntRowSelected;
        //string mStrFolio;
        //string mStrArea;
        string mStrAffectable;
        string mStrAccountDEU;
        //string mStrAuxCode;
        //string mStrAuxCodeAfectable;
        //string mStrCodeVoucher;
        //string mStrCodeMov;
        string mStrEmployeName;
        Vouchers mObjVouchers;
        bool mBolIsCheeckingCost;
        ChooseFromList mObjCFLAsset;
        ChooseFromList mObjCFLProject;
        ChooseFromList mObjCFLAux;
        TypeEnum.Type mNoteType;
        string mStrLine;
        private frmModalAF mObjModalAF = null;
        frmReceipts mObjBaseForm;
        #endregion

        #region Constructor
        public frmPurchaseNotes(frmReceipts pObjForm, Vouchers pObjVouchers, string pStrAffectable, TypeEnum.Type pNoteType, string pStrAccountDEU, bool pBolIsCheeckingCost, string pStrEmployeName, bool pBolCopyComents)
        {
            try
            {
                mObjBaseForm = pObjForm;
                CreateDatatableMatrix();
                //mStrArea = pObjVouchers.Area;
                //mStrFolio = pObjVouchers.Folio; //Folio de comprobante
                mStrAffectable = pStrAffectable;
                mStrAccountDEU = pStrAccountDEU;
                //mStrCodeVoucher = pObjVouchers.RowCode; //Codigointerno
                mBolIsCheeckingCost = pBolIsCheeckingCost;
                //mStrCodeMov = pObjVouchers.CodeMov; // MQ_Folio
                // mStrAuxCode = pObjVouchers.Employee;
                //mStrAuxCodeAfectable = pObjVouchers.Employee;
                lblAux.Item.Visible = false;
                txtAux.Item.Visible = false;
                mNoteType = pNoteType;


                cboType.Select("Gastos/Costos");
                cboMovement.Item.Visible = false;
                lblCodeMov.Item.Visible = false;
                mObjVouchers = pObjVouchers;
                mStrEmployeName = pStrEmployeName;


                SetChooseToTxt();
                LoadCboTypeAccount(pNoteType);

                AddComboboxMQ();

                if (pBolCopyComents)
                {
                    txtObs.Value = pObjVouchers.Coments;
                }
                txtFolio.Item.Click();
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (frmPurchaseNotes) " + ex.Message);
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
            this.lblFolio = ((SAPbouiCOM.StaticText)(this.GetItem("lblFolio").Specific));
            this.lblCredi = ((SAPbouiCOM.StaticText)(this.GetItem("lblCredi").Specific));
            this.lblProv = ((SAPbouiCOM.StaticText)(this.GetItem("lblProv").Specific));
            this.lblAccount = ((SAPbouiCOM.StaticText)(this.GetItem("lblAccount").Specific));
            this.lblArea = ((SAPbouiCOM.StaticText)(this.GetItem("lblArea").Specific));
            this.lblAF = ((SAPbouiCOM.StaticText)(this.GetItem("lblAF").Specific));
            this.lblProject = ((SAPbouiCOM.StaticText)(this.GetItem("lblProject").Specific));
            this.lblAmount = ((SAPbouiCOM.StaticText)(this.GetItem("lblAmount").Specific));
            this.lblAttach = ((SAPbouiCOM.StaticText)(this.GetItem("lblAttach").Specific));
            this.lblObs = ((SAPbouiCOM.StaticText)(this.GetItem("lblObs").Specific));
            this.txtFolio = ((SAPbouiCOM.EditText)(this.GetItem("txtFolio").Specific));
            this.txtCredit = ((SAPbouiCOM.EditText)(this.GetItem("txtCredit").Specific));
            this.txtProv = ((SAPbouiCOM.EditText)(this.GetItem("txtProv").Specific));
            this.txtAccount = ((SAPbouiCOM.EditText)(this.GetItem("txtAccount").Specific));
            this.txtArea = ((SAPbouiCOM.EditText)(this.GetItem("txtArea").Specific));
            this.txtAF = ((SAPbouiCOM.EditText)(this.GetItem("txtAF").Specific));
            this.txtAF.KeyDownAfter += new SAPbouiCOM._IEditTextEvents_KeyDownAfterEventHandler(this.txtAF_KeyDownAfter);
            this.txtProject = ((SAPbouiCOM.EditText)(this.GetItem("txtProject").Specific));
            this.txtAmount = ((SAPbouiCOM.EditText)(this.GetItem("txtAmount").Specific));
            this.txtAttach = ((SAPbouiCOM.EditText)(this.GetItem("txtAttach").Specific));
            this.txtObs = ((SAPbouiCOM.EditText)(this.GetItem("txtObs").Specific));
            this.btnRemove = ((SAPbouiCOM.Button)(this.GetItem("btnRemove").Specific));
            this.btnRemove.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnRemove_ClickBefore);
            this.btnAdd = ((SAPbouiCOM.Button)(this.GetItem("btnAdd").Specific));
            this.btnAdd.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnAdd_ClickBefore);
            this.btnAttach = ((SAPbouiCOM.Button)(this.GetItem("btnAttach").Specific));
            this.btnAttach.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnAttach_ClickBefore);
            this.mtxNotes = ((SAPbouiCOM.Matrix)(this.GetItem("mtxNotes").Specific));
            this.mtxNotes.ClickBefore += new SAPbouiCOM._IMatrixEvents_ClickBeforeEventHandler(this.mtxNotes_ClickBefore);
            this.btnCreate = ((SAPbouiCOM.Button)(this.GetItem("btnCreate").Specific));
            this.btnCreate.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCreate_ClickBefore);
            this.txtTotal = ((SAPbouiCOM.EditText)(this.GetItem("Item_1").Specific));
            this.lblTotal = ((SAPbouiCOM.StaticText)(this.GetItem("lblTotal").Specific));
            UGRS.Core.SDK.UI.UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(this.SBO_Application_ItemEvent);
            //UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
            this.lblType = ((SAPbouiCOM.StaticText)(this.GetItem("lblType").Specific));
            this.cboType = ((SAPbouiCOM.ComboBox)(this.GetItem("cboType").Specific));
            this.cboType.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cboType_ComboSelectAfter);
            this.txtAux = ((SAPbouiCOM.EditText)(this.GetItem("txtAux").Specific));
            this.lblAux = ((SAPbouiCOM.StaticText)(this.GetItem("lblAux").Specific));
            this.lblCodeMov = ((SAPbouiCOM.StaticText)(this.GetItem("lblCodeMov").Specific));
            this.cboMovement = ((SAPbouiCOM.ComboBox)(this.GetItem("cboMov").Specific));
            this.lblSubidaMq = ((SAPbouiCOM.StaticText)(this.GetItem("lblSubMq").Specific));
            this.cboSubidaMq = ((SAPbouiCOM.ComboBox)(this.GetItem("cboSubMq").Specific));
            this.lblAGL = ((SAPbouiCOM.StaticText)(this.GetItem("lblAGL").Specific));
            this.txtAGL = ((SAPbouiCOM.EditText)(this.GetItem("txtAGL").Specific));
            this.lblLine = ((SAPbouiCOM.StaticText)(this.GetItem("lblLine").Specific));
            this.txtLinea = ((SAPbouiCOM.EditText)(this.GetItem("txtLinea").Specific));
            this.OnCustomInitialize();

        }
        public override void OnInitializeFormEvents()
        {
            this.ResizeAfter += new SAPbouiCOM.Framework.FormBase.ResizeAfterHandler(this.Form_ResizeAfter);
            this.CloseAfter += new CloseAfterHandler(this.Form_CloseAfter);

        }
        private void OnCustomInitialize()
        {
            try
            {
                btnCreate.Item.Visible = true;
                LoadChoseFromList();

                if (mBolIsCheeckingCost)
                {
                    cboType.Item.Enabled = false;
                }
                else
                {
                    cboType.Item.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (OnCustomInitialize) " + ex.Message);
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
                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case BoEventTypes.et_CHOOSE_FROM_LIST:
                                ChooseFromListAfterEvent(pVal);
                                break;
                            case BoEventTypes.et_COMBO_SELECT:
                                AddConditionAssets(mObjCFLAsset);
                                break;
                        }
                    }
                }

                if (pVal.FormTypeEx.Equals("UGRS.AddOn.Purchases.Forms.frmModalAF"))
                {
                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                if (mObjModalAF != null)
                                {
                                    if (string.IsNullOrEmpty(mObjModalAF.mStrAFCode))
                                        return;

                                    txtAF.Value = mObjModalAF.mStrAFCode;
                                    mObjModalAF = null;
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("ItemEventException: {0}", ex.Message));
                LogService.WriteError("frmPurchaseNote (SBO_Application_ItemEvent) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void btnCreate_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            bool lBolSuccess = false;
            try
            {
                PolicyDI lObjPolicyDI = new PolicyDI();

                List<PurchaseNote> lLstPurchaseNote = ReadDatatable();

                DIApplication.Company.StartTransaction();

                //if (string.IsNullOrEmpty(lLstPurchaseNote[0].CodeVoucher) || Convert.ToInt32(lLstPurchaseNote[0].CodeVoucher) < 0)
                //{
                //    mStrCodeVoucher = SaveVoucher(mObjVouchers);
                //    lLstPurchaseNote[0].CodeVoucher = mStrCodeVoucher;

                //}

                if (!string.IsNullOrEmpty(lLstPurchaseNote[0].CodeVoucher) || Convert.ToInt32(lLstPurchaseNote[0].CodeVoucher) > 0)
                {
                    var lLstResult = lObjPolicyDI.CreateDocument(lLstPurchaseNote, mObjVouchers, mNoteType);
                    if (lLstResult.Count > 0)
                    {
                        string lStrMessage = string.Format("Error al crear asiento {0}: ",
                            string.Join("\n", lLstResult.Select(x => string.Format("-{0}", x)).ToArray()));

                        UIApplication.ShowMessageBox(lStrMessage);
                        LogService.WriteError(lStrMessage);
                        lBolSuccess = false;
                    }
                    else
                    {
                        string lStrDocEntry = DIApplication.Company.GetNewObjectKey().ToString();

                        lBolSuccess = AddVoucherDetail(lLstPurchaseNote, lStrDocEntry);
                        if (lBolSuccess)
                        {
                            try
                            {
                                this.UIAPIRawForm.Freeze(true);
                                ClearControls();
                                DtMatrix.Rows.Clear();
                                mtxNotes.LoadFromDataSource();
                                txtTotal.Value = "";
                            }
                            catch (Exception ex)
                            {
                                UIAPIRawForm.Freeze(false);
                                UIApplication.ShowError(ex.Message);
                                LogService.WriteError("frmPurchaseNote (btnCreate_ClickBefore) " + ex.Message);
                                LogService.WriteError(ex);
                            }
                            finally
                            {
                                UIAPIRawForm.Freeze(false);
                            }


                        }
                        else
                        {
                            UIApplication.ShowError("(btnSave_ClickBefore): " + "No fue posible guardar el detalle del comprobante");
                            LogService.WriteError("(btnSave_ClickBefore): " + "No fue posible guardar el detalle del comprobante");
                        }


                    }
                }
                else
                {
                    UIApplication.ShowError("(btnSave_ClickBefore): " + " No fue posible crear el comprobante");
                    LogService.WriteError("(btnSave_ClickBefore): " + " No fue posible crear el comprobante");
                    lBolSuccess = false;
                }

            }
            catch (Exception ex)
            {
                lBolSuccess = false;
                LogService.WriteError("frmPurchaseNote (SBO_Application_ItemEvent) " + ex.Message);
                LogService.WriteError(ex);
            }
            finally
            {
                try
                {
                    if (lBolSuccess)
                    {
                        mObjBaseForm.UpdateMatriz();
                        DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);

                        UIApplication.ShowMessageBox("Asiento guardado correctamente");
                        LogService.WriteSuccess("Asiento guardado correctamente Folio: " + mObjVouchers.RowCode);
                        this.UIAPIRawForm.Close();
                    }
                    else
                    {
                        // mStrCodeVoucher = string.Empty;
                        if (DIApplication.Company.InTransaction)
                        {
                            DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string urss = DIApplication.Company.GetLastErrorDescription();
                    LogService.WriteError("(btnSave_ClickBefore): " + ex.Message);
                    LogService.WriteError(ex);
                }

            }
        }

        private void mtxNotes_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (pVal.Row > 0)
                {
                    mtxNotes.SelectRow(pVal.Row, true, false);
                    mIntRowSelected = pVal.Row;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (mtxNotes_ClickBefore) " + ex.Message);
                LogService.WriteError(ex);

            }
        }

        private void btnRemove_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {

                if (mIntRowSelected > 0 && mtxNotes.IsRowSelected(mIntRowSelected)
                    && (SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("¿Desea elimiar el item seleccionado?", 2, "Si", "No", "") == 1))
                {

                    DtMatrix.Rows.Remove(mIntRowSelected - 1);
                    this.UIAPIRawForm.Freeze(true);
                    mtxNotes.LoadFromDataSource();
                    this.UIAPIRawForm.Freeze(false);
                    mIntRowSelected = 0;
                    SetTotal();
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (btnRemove_ClickBefore) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void txtAF_KeyDownAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                if (pVal.CharPressed == 13)
                {
                    if (UIUtility.IsFormOpen("UGRS.AddOn.Purchases.Forms.frmModalAF"))
                    {
                        UIApplication.GetApplication().Forms.Item("frmMdlAF").Close();
                    }

                    mObjModalAF = new frmModalAF(txtArea.Value, txtAF.Value, cboSubidaMq.Value);
                    mObjModalAF.Show();
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
        }

        private void btnAttach_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            CreateFolderBroserThread();

        }

        private void btnAdd_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                this.UIAPIRawForm.Freeze(true);
                if (ValidateControls())
                {
                    if (cboMovement.Item.Visible && string.IsNullOrEmpty(cboMovement.Value))
                    {
                        UIApplication.ShowError("Seleccione un código de movimiento");
                        return;
                    }

                    PurchaseNote lObjPurchaseNote = new PurchaseNote();
                    lObjPurchaseNote.Folio = txtFolio.Value;
                    lObjPurchaseNote.Provider = txtProv.Value;
                    lObjPurchaseNote.Account = txtAccount.Value;
                    lObjPurchaseNote.Area = txtArea.Value;
                    lObjPurchaseNote.CostingCode = mObjVouchers.Area;
                    lObjPurchaseNote.AF = txtAF.Value;
                    lObjPurchaseNote.Amount = Convert.ToDouble(txtAmount.Value);
                    lObjPurchaseNote.Affectable = txtCredit.Value;
                    lObjPurchaseNote.Project = txtProject.Value;
                    lObjPurchaseNote.Coments = txtObs.Value;
                    lObjPurchaseNote.File = txtAttach.Value;
                    lObjPurchaseNote.Aux = string.IsNullOrEmpty(mObjVouchers.Employee) ? "" : mObjVouchers.Employee;
                    lObjPurchaseNote.AuxName = string.IsNullOrEmpty(txtAux.Value) ? mStrEmployeName : txtAux.Value;
                    lObjPurchaseNote.CodeVoucher = string.IsNullOrEmpty(mObjVouchers.RowCode) ? "" : mObjVouchers.RowCode;
                    lObjPurchaseNote.CodeMov = cboMovement.Item.Visible ? cboMovement.Value : mObjVouchers.CodeMov;
                    lObjPurchaseNote.AuxAfectable = mObjVouchers.Employee;
                    lObjPurchaseNote.AGL = txtAGL.Value;
                    lObjPurchaseNote.Line = txtLinea.Value;
                    AddRow(lObjPurchaseNote);
                    SetTotal();
                    ClearControls();
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("frmPurchaseNote (frmPurchaseNotes) " + ex.Message);
                LogService.WriteError(ex);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }

        }

        private void cboType_ComboSelectAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {

                if (cboType.Value == "Deudora")
                {
                    lblAux.Item.Visible = true;
                    txtAux.Item.Visible = true;
                    txtAccount.Item.Enabled = false;
                    txtAccount.Value = mStrAccountDEU;
                }
                else
                {
                    lblAux.Item.Visible = false;
                    txtAux.Item.Visible = false;
                    txtAux.Value = "";
                    txtAccount.Value = "";
                    txtAccount.Item.Enabled = true;
                    cboMovement.Item.Visible = false;
                    lblCodeMov.Item.Visible = false;
                }

            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (cboType_ComboSelectAfter) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void Form_ResizeAfter(SBOItemEventArg pVal)
        {
            mtxNotes.AutoResizeColumns();
        }

        private void Form_CloseAfter(SBOItemEventArg pVal)
        {
            UnLoadEvents();
        }
        #endregion

        #region ChooseFromLists
        private void LoadChoseFromList()
        {
            try
            {
                mObjCFLProject = InitChooseFromLists(false, "63", "CFL_Pro", this.UIAPIRawForm.ChooseFromLists);


                ChooseFromList lObjCFLProjectArea = InitChooseFromLists(false, "61", "CFL_Area", this.UIAPIRawForm.ChooseFromLists);
                AddConditionChoseFromListCostCenter(lObjCFLProjectArea, "1");

                //mObjCFLAsset = InitChooseFromLists(false, "61", "CFL_Asset", this.UIAPIRawForm.ChooseFromLists);


                ChooseFromList lOBjCFLAccount = InitChooseFromLists(false, "1", "CFL_Acc", this.UIAPIRawForm.ChooseFromLists);
                AddContitionsAccount(lOBjCFLAccount);

                ChooseFromList lObjCFLAffectable = InitChooseFromLists(false, "1", "CFL_Aff", this.UIAPIRawForm.ChooseFromLists);
                AddConditionAffectable(lObjCFLAffectable);

                mObjCFLAux = InitChooseFromLists(false, "171", "CFL_Aux", this.UIAPIRawForm.ChooseFromLists);

                ChooseFromList lObjCFLAGL = InitChooseFromLists(false, "61", "CFL_AGL", this.UIAPIRawForm.ChooseFromLists);
                AddConditionChoseFromListCostCenter(lObjCFLAGL, "3");

                ChooseFromList lObjCFLLines = InitChooseFromLists(false, "61", "CFL_Line", this.UIAPIRawForm.ChooseFromLists);
                AddConditionChoseFromListCostCenter(lObjCFLLines, "4");


            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (LoadChoseFromList) " + ex.Message);
                LogService.WriteError(ex);
            }

        }

        private void SetChooseToTxt()
        {
            try
            {
                txtProject.DataBind.SetBound(true, "", "CFL_Pro");
                txtProject.ChooseFromListUID = "CFL_Pro";
                txtProject.ChooseFromListAlias = "PrjCode";

                txtArea.DataBind.SetBound(true, "", "CFL_Area");
                txtArea.ChooseFromListUID = "CFL_Area";
                txtArea.ChooseFromListAlias = "PrcCode";
                this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Area").ValueEx = mObjVouchers.Area;
                AddConditionAssets(mObjCFLAsset);
                AddConditionChoseFromListProject(mObjCFLProject);
                AddConditionAuxiliar(mObjCFLAux, mObjPurchaseServiceFactory.GetPurchaseService().GetDepartment(txtArea.Value));

                //txtAF.DataBind.SetBound(true, "", "CFL_Asset");
                //txtAF.ChooseFromListUID = "CFL_Asset";
                //txtAF.ChooseFromListAlias = "PrcCode";

                txtCredit.Value = mStrAffectable;

                //txtCredit.DataBind.SetBound(true, "", "CFL_Aff");
                //txtCredit.ChooseFromListUID = "CFL_Aff";

                txtAccount.DataBind.SetBound(true, "", "CFL_Acc");
                txtAccount.ChooseFromListUID = "CFL_Acc";
                txtAccount.ChooseFromListAlias = "AcctCode";

                txtAux.DataBind.SetBound(true, "", "CFL_Aux");
                txtAux.ChooseFromListUID = "CFL_Aux";
                txtAux.ChooseFromListAlias = "lastName";

                txtAGL.DataBind.SetBound(true, "", "CFL_AGL");
                txtAGL.ChooseFromListUID = "CFL_AGL";
                txtAGL.ChooseFromListAlias = "PrcCode";


                txtLinea.DataBind.SetBound(true, "", "CFL_Line");
                txtLinea.ChooseFromListUID = "CFL_Line";
                txtLinea.ChooseFromListAlias = "PrcCode";
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (SetChooseToTxt) " + ex.Message);
                LogService.WriteError(ex);
            }

        }

        #region ConditionsChoosefromlist
        private void AddConditionChoseFromListProject(ChooseFromList pCFL)
        {
            try
            {
                SAPbouiCOM.Condition lObjCon = null;
                SAPbouiCOM.Conditions lObjCons = new Conditions();

                lObjCon = lObjCons.Add();
                lObjCon.Alias = "Active";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = "Y";

                /*lObjCon.Relationship = BoConditionRelationship.cr_AND;

                lObjCon = lObjCons.Add();
                lObjCon.Alias = "U_GLO_PrcCode";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = txtArea.Value;*/


                pCFL.SetConditions(lObjCons);
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (AddConditionChoseFromListProject) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void AddConditionChoseFromListCostCenter(ChooseFromList pCFL, string pStrDimension)
        {
            try
            {
                SAPbouiCOM.Condition lObjCon = null;
                SAPbouiCOM.Conditions lObjCons = new Conditions();

                //DimCode
                lObjCon = lObjCons.Add();
                lObjCon.Alias = "DimCode";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = pStrDimension;

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
                LogService.WriteError("frmPurchaseNote (AddConditionChoseFromListArea) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void AddConditionAssets(ChooseFromList pCFL)
        {
            try
            {
                //SAPbouiCOM.Condition lObjCon = null;
                //SAPbouiCOM.Conditions lObjCons = new Conditions();

                ////List<AssetsDTO> lLstAssetsDTO = new List<AssetsDTO>();
                ////if (string.IsNullOrEmpty(cboSubidaMq.Value))
                ////{
                ////    lLstAssetsDTO = mObjPurchaseServiceFactory.GetPurchaseService().GetAssets(txtArea.Value).ToList();
                ////}
                ////else
                ////{
                ////    lLstAssetsDTO = mObjPurchaseServiceFactory.GetPurchaseXmlService().GetRiseAF(cboSubidaMq.Value).ToList();
                ////}

                //List<AssetsDTO> lLstAssetsDTO = mObjPurchaseServiceFactory.GetPurchaseService().GetAssets(txtArea.Value).ToList();
                //int i = 1;
                //if (lLstAssetsDTO.Count > 0)
                //{
                //    foreach (AssetsDTO lObjAssetDTO in lLstAssetsDTO)
                //    {
                //        lObjCon = lObjCons.Add();
                //        lObjCon.Alias = "PrcCode";
                //        lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                //        lObjCon.CondVal = lObjAssetDTO.PrcCode;

                //        if (lLstAssetsDTO.Count() > i)
                //        {
                //            lObjCon.Relationship = BoConditionRelationship.cr_OR;
                //        }
                //        i++;
                //    }
                //    pCFL.SetConditions(lObjCons);
                //}
                //else
                //{
                //    lObjCon = lObjCons.Add();
                //    lObjCon.Alias = "PrcCode";
                //    lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                //    lObjCon.CondVal = "";
                //    pCFL.SetConditions(lObjCons);
                //}
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (AddConditionAssets) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void AddContitionsAccount(ChooseFromList pCFL)
        {
            try
            {
                SAPbouiCOM.Condition lObjCon = null;
                SAPbouiCOM.Conditions lObjCons = null;
                lObjCons = pCFL.GetConditions();

                lObjCon = lObjCons.Add();
                lObjCon.Alias = "Postable";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = "Y";

                /*
                lObjCon = lObjCons.Add();
                lObjCon.Alias = "GroupMask";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = "5";
                lObjCon.Relationship = BoConditionRelationship.cr_AND;
                lObjCon = lObjCons.Add();
                lObjCon.Alias = "Postable";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = "Y";

                lObjCon.Relationship = BoConditionRelationship.cr_OR;

                lObjCon = lObjCons.Add();
                lObjCon.Alias = "GroupMask";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = "6";
                lObjCon.Relationship = BoConditionRelationship.cr_AND;
                lObjCon = lObjCons.Add();
                lObjCon.Alias = "Postable";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = "Y";*/

                pCFL.SetConditions(lObjCons);
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (AddContitionsAccount) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void AddConditionAffectable(ChooseFromList pCFL)
        {
            try
            {
                SAPbouiCOM.Condition lObjCon = null;
                SAPbouiCOM.Conditions lObjCons = new Conditions();

                List<string> lLstAffectableDTO = mObjPurchaseServiceFactory.GetPurchaseNoteService().GetAffectable().ToList();
                if (lLstAffectableDTO.Count > 0)
                {
                    int i = 1;
                    foreach (string lObjAssetDTO in lLstAffectableDTO)
                    {
                        lObjCon = lObjCons.Add();
                        lObjCon.Alias = "AcctCode";
                        lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        lObjCon.CondVal = lObjAssetDTO;

                        if (lLstAffectableDTO.Count() > i)
                        {
                            lObjCon.Relationship = BoConditionRelationship.cr_OR;
                        }
                        i++;
                    }
                    pCFL.SetConditions(lObjCons);
                }
                else
                {
                    lObjCon = lObjCons.Add();
                    lObjCon.Alias = "AcctCode";
                    lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                    lObjCon.CondVal = "";
                    pCFL.SetConditions(lObjCons);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (AddConditionAffectable) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void AddConditionAuxiliar(ChooseFromList pCFL, string pStrDept)
        {
            try
            {
                SAPbouiCOM.Condition lObjCon = null;
                SAPbouiCOM.Conditions lObjCons = new Conditions();


                //DimCode
                lObjCon = lObjCons.Add();
                lObjCon.Alias = "dept";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = pStrDept;

                pCFL.SetConditions(lObjCons);
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (AddConditionAuxiliar) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        #endregion

        public ChooseFromList InitChooseFromLists(bool pbolMultiselection, string pStrObjectType, string pStrID, SAPbouiCOM.ChooseFromListCollection pObjCFLs) //
        {
            SAPbouiCOM.ChooseFromList lObjoCFL = null;
            try
            {
                SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
                oCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)UIApplication.GetApplication().CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);

                oCFLCreationParams.MultiSelection = pbolMultiselection;
                oCFLCreationParams.ObjectType = pStrObjectType;
                oCFLCreationParams.UniqueID = pStrID;

                lObjoCFL = pObjCFLs.Add(oCFLCreationParams);

                this.UIAPIRawForm.DataSources.UserDataSources.Add(pStrID, SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("InitCustomerChooseFromListException: {0}", ex.Message));
                LogService.WriteError("frmPurchaseNote (InitChooseFromLists) " + ex.Message);
                LogService.WriteError(ex);

            }
            return lObjoCFL;
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
                        this.UIAPIRawForm.DataSources.UserDataSources.Item(lObjDataTable.UniqueID).ValueEx = System.Convert.ToString(lObjDataTable.GetValue(0, 0));

                        if (lObjDataTable.UniqueID == "CFL_Aux")
                        {
                            mObjVouchers.Employee = System.Convert.ToString(lObjDataTable.GetValue(0, 0));
                            this.UIAPIRawForm.DataSources.UserDataSources.Item(lObjDataTable.UniqueID).ValueEx = System.Convert.ToString(lObjDataTable.GetValue(1, 0) + System.Convert.ToString(lObjDataTable.GetValue(2, 0)));
                            LoadCboMovementPayment(mObjVouchers.Employee);
                            cboMovement.Item.Visible = true;
                            lblCodeMov.Item.Visible = true;

                        }
                        if (lObjDataTable.UniqueID == "CFL_Area")
                        {
                            AddConditionAssets(mObjCFLAsset);
                            AddConditionChoseFromListProject(mObjCFLProject);
                            AddConditionAuxiliar(mObjCFLAux, mObjPurchaseServiceFactory.GetPurchaseService().GetDepartment(txtArea.Value));
                        }

                        if (lObjDataTable.UniqueID == "CFL_Line")
                        {
                            this.UIAPIRawForm.DataSources.UserDataSources.Item(lObjDataTable.UniqueID).ValueEx = System.Convert.ToString(lObjDataTable.GetValue("PrcName", 0));
                            mStrLine = lObjDataTable.GetValue("PrcCode", 0).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (ChooseFromListAfterEvent) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        #endregion

        #region Matrix
        private void CreateDatatableMatrix()
        {
            try
            {

                this.UIAPIRawForm.DataSources.DataTables.Add("TransactionsResult");
                DtMatrix = this.UIAPIRawForm.DataSources.DataTables.Item("TransactionsResult");
                DtMatrix.Columns.Add("C_CodeVoucher", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Folio", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Provider", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Account", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Area", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_CostingCode", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_AF", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Amount", SAPbouiCOM.BoFieldsType.ft_Price);
                DtMatrix.Columns.Add("C_Project", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Affect", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Coments", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_File", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Aux", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_AuxName", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_CodeMov", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_AuxAf", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_AGL", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Line", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_LineCode", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);

                FillMatrix();
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (CreateDatatableMatrix) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void FillMatrix()
        {
            try
            {
                mtxNotes.Columns.Item("C_Folio").DataBind.Bind("TransactionsResult", "C_Folio");
                mtxNotes.Columns.Item("C_Provider").DataBind.Bind("TransactionsResult", "C_Provider");
                mtxNotes.Columns.Item("C_Account").DataBind.Bind("TransactionsResult", "C_Account");
                mtxNotes.Columns.Item("C_Area").DataBind.Bind("TransactionsResult", "C_Area");
                mtxNotes.Columns.Item("C_AF").DataBind.Bind("TransactionsResult", "C_AF");
                mtxNotes.Columns.Item("C_Amount").DataBind.Bind("TransactionsResult", "C_Amount");
                mtxNotes.Columns.Item("C_Project").DataBind.Bind("TransactionsResult", "C_Project");
                mtxNotes.Columns.Item("C_Affect").DataBind.Bind("TransactionsResult", "C_Affect");
                mtxNotes.Columns.Item("C_Coments").DataBind.Bind("TransactionsResult", "C_Coments");
                mtxNotes.Columns.Item("C_File").DataBind.Bind("TransactionsResult", "C_File");
                mtxNotes.Columns.Item("C_Aux").DataBind.Bind("TransactionsResult", "C_Aux");
                mtxNotes.Columns.Item("C_AuxName").DataBind.Bind("TransactionsResult", "C_AuxName");
                mtxNotes.Columns.Item("C_AGL").DataBind.Bind("TransactionsResult", "C_AGL");
                mtxNotes.Columns.Item("C_Line").DataBind.Bind("TransactionsResult", "C_Line");


                mtxNotes.LoadFromDataSource();
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (FillMatrix) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void AddRow(PurchaseNote pObjPurchadeNote)
        {
            try
            {
                DtMatrix.Rows.Add();
                int i = DtMatrix.Rows.Count - 1;
                DtMatrix.SetValue("C_Folio", i, pObjPurchadeNote.Folio);
                DtMatrix.SetValue("C_Provider", i, pObjPurchadeNote.Provider);
                DtMatrix.SetValue("C_Account", i, pObjPurchadeNote.Account);
                DtMatrix.SetValue("C_Area", i, pObjPurchadeNote.Area);
                DtMatrix.SetValue("C_AF", i, pObjPurchadeNote.AF);
                DtMatrix.SetValue("C_Amount", i, pObjPurchadeNote.Amount);
                DtMatrix.SetValue("C_Project", i, pObjPurchadeNote.Project);
                DtMatrix.SetValue("C_Affect", i, pObjPurchadeNote.Affectable);
                DtMatrix.SetValue("C_Coments", i, pObjPurchadeNote.Coments);
                DtMatrix.SetValue("C_File", i, pObjPurchadeNote.File);
                DtMatrix.SetValue("C_Aux", i, pObjPurchadeNote.Aux);
                DtMatrix.SetValue("C_AuxName", i, pObjPurchadeNote.AuxName);
                DtMatrix.SetValue("C_CodeVoucher", i, pObjPurchadeNote.CodeVoucher);
                DtMatrix.SetValue("C_CostingCode", i, string.IsNullOrEmpty(pObjPurchadeNote.CostingCode) ? "" : pObjPurchadeNote.CostingCode);
                DtMatrix.SetValue("C_CodeMov", i, pObjPurchadeNote.CodeMov);
                DtMatrix.SetValue("C_AuxAf", i, pObjPurchadeNote.AuxAfectable);
                DtMatrix.SetValue("C_AGL", i, pObjPurchadeNote.AGL);
                DtMatrix.SetValue("C_Line", i, pObjPurchadeNote.Line);
                DtMatrix.SetValue("C_LineCode", i, string.IsNullOrEmpty(txtLinea.Value) ? "" : mStrLine);
                mtxNotes.LoadFromDataSource();
                mtxNotes.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                UIApplication.ShowError(string.Format("ItemEventException: {0}", ex.Message));
                LogService.WriteError("frmPurchaseNote (AddRow) " + ex.Message);
                LogService.WriteError(ex);
                this.UIAPIRawForm.Freeze(true);
            }

        }

        #endregion

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

        #region Methods
        private bool ValidateControls()
        {
            IList<string> lLstmissingFields = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(txtAccount.Value))
                {
                    lLstmissingFields.Add("Cuenta");
                }
                if (string.IsNullOrEmpty(txtAmount.Value))
                {
                    lLstmissingFields.Add("Importe");
                }
                double lDblNum;
                if (!double.TryParse(txtAmount.Value, out lDblNum))
                {
                    if (lDblNum < 0)
                    {
                        lLstmissingFields.Add("Contenido en campo importe");
                    }
                }
                if (string.IsNullOrEmpty(txtArea.Value))
                {
                    lLstmissingFields.Add("Area");
                }
                if (string.IsNullOrEmpty(txtCredit.Value))
                {
                    lLstmissingFields.Add("Acredora");
                }
                if (string.IsNullOrEmpty(txtFolio.Value))
                {
                    lLstmissingFields.Add("Folio");
                }
                if (string.IsNullOrEmpty(txtProv.Value))
                {
                    lLstmissingFields.Add("Proveedor");
                }

                if (string.IsNullOrEmpty(cboType.Value))
                {
                    lLstmissingFields.Add("Tipo de cuenta");
                }
                if (cboType.Value != "Gastos/Costos")
                {
                    if (string.IsNullOrEmpty(txtAux.Value))
                    {
                        lLstmissingFields.Add("Auxiliar");
                    }
                }

                if (lLstmissingFields.Count > 0)
                {
                    string message = string.Format("Favor de completar {0}:\n{1}",
                        (lLstmissingFields.Count == 1 ? "el siguiente campo" : "los siguientes campos"),
                        string.Join("\n", lLstmissingFields.Select(x => string.Format("-{0}", x)).ToArray()));
                    this.UIAPIRawForm.Freeze(false);
                    UIApplication.ShowMessageBox(message);
                    this.UIAPIRawForm.Freeze(true);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (ValidateControls) " + ex.Message);
                LogService.WriteError(ex);
            }
            return lLstmissingFields.Count == 0 ? true : false;
        }

        private void ClearControls()
        {
            try
            {
                txtAccount.Value = string.Empty;
                txtAF.Value = string.Empty;
                txtAmount.Value = string.Empty;
                //txtArea.Value = string.Empty;
                txtAttach.Value = string.Empty;
                //txtCredit.Value = string.Empty;
                txtFolio.Value = string.Empty;
                txtObs.Value = string.Empty;
                txtProject.Value = string.Empty;
                txtProv.Value = string.Empty;
                cboType.Select("Gastos/Costos");
                txtAux.Value = string.Empty;
                lblCodeMov.Item.Visible = false;
                txtAGL.Value = string.Empty;
                txtLinea.Value = string.Empty;

                int lIntcboCount = cboMovement.ValidValues.Count;
                for (int i = 0; i < lIntcboCount; i++)
                {
                    cboMovement.ValidValues.Remove(0, BoSearchKey.psk_Index);
                }
                cboMovement.ValidValues.Add("", "");
                cboMovement.Select(0, BoSearchKey.psk_Index);
                txtFolio.Item.Click();
                cboMovement.Item.Visible = false;
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (ClearControls) " + ex.Message);
                LogService.WriteError(ex);
            }

        }

        private void SetTotal()
        {
            try
            {
                double lDblTotal = 0;
                for (int i = 0; i < DtMatrix.Rows.Count; i++)
                {

                    lDblTotal += Convert.ToDouble(DtMatrix.GetValue("C_Amount", i));
                }
                txtTotal.Value = lDblTotal.ToString("C");
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (SetTotal) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void LoadCboTypeAccount(TypeEnum.Type pNoteType)
        {
            try
            {
                if (pNoteType == TypeEnum.Type.Refund)
                {
                    cboType.ValidValues.Add("Deudora", "");
                    cboType.ValidValues.Add("Gastos/Costos", "");
                    cboType.Select("Gastos/Costos");
                }
                if (pNoteType == TypeEnum.Type.Voucher)
                {
                    cboType.ValidValues.Add("Gastos/Costos", "");

                    cboType.Select("Gastos/Costos");
                    cboType.Item.Enabled = false;

                }
                cboType.Item.DisplayDesc = false;
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (LoadCboTypeAccount) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void LoadCboMovementPayment(string pStrEmpId)
        {
            try
            {

                int lIntcboCount = cboMovement.ValidValues.Count;
                for (int i = 0; i < lIntcboCount; i++)
                {
                    cboMovement.ValidValues.Remove(0, BoSearchKey.psk_Index);
                }


                List<PaymentDTO> lLstPayment = mObjPurchaseServiceFactory.GetPurchaseCheeckingCostService().GetPayment(txtArea.Value, "", !txtArea.Item.Enabled).Where(x => x.EmpId == pStrEmpId).ToList();
                cboMovement.ValidValues.Add("", "");
                foreach (PaymentDTO lObjPayment in lLstPayment)
                {
                    cboMovement.ValidValues.Add(lObjPayment.Folio, lObjPayment.Folio);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (LoadCboTypeAccount) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void AddComboboxMQ()
        {
            try
            {
                List<string> lLstStringRises = mObjPurchaseServiceFactory.GetPurchaseXmlService().GetMqSubidas().Distinct().ToList();
                cboSubidaMq.ValidValues.Add("", "");
                foreach (string lStrIdRise in lLstStringRises)
                {
                    cboSubidaMq.ValidValues.Add(lStrIdRise, "");
                }
                cboSubidaMq.ExpandType = BoExpandType.et_ValueOnly;

            }
            catch (Exception ex)
            {
                LogService.WriteError("(AddComboboxMQ): " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format("Error al obtener las subidas de maquinaria: {0}", ex.Message));
            }
        }

        private List<PurchaseNote> ReadDatatable()
        {
            List<PurchaseNote> lLstPurchaseNote = new List<PurchaseNote>();
            for (int i = 0; i < DtMatrix.Rows.Count; i++)
            {
                PurchaseNote lObjPurchaseNote = new PurchaseNote();
                lObjPurchaseNote.Folio = DtMatrix.GetValue("C_Folio", i).ToString();
                lObjPurchaseNote.Provider = DtMatrix.GetValue("C_Provider", i).ToString();
                lObjPurchaseNote.Account = DtMatrix.GetValue("C_Account", i).ToString();
                lObjPurchaseNote.Area = DtMatrix.GetValue("C_Area", i).ToString();
                lObjPurchaseNote.CostingCode = DtMatrix.GetValue("C_CostingCode", i).ToString();
                lObjPurchaseNote.AF = DtMatrix.GetValue("C_AF", i).ToString();
                lObjPurchaseNote.Amount = Convert.ToDouble(DtMatrix.GetValue("C_Amount", i).ToString());
                lObjPurchaseNote.Project = DtMatrix.GetValue("C_Project", i).ToString();
                lObjPurchaseNote.Affectable = DtMatrix.GetValue("C_Affect", i).ToString();
                lObjPurchaseNote.Coments = DtMatrix.GetValue("C_Coments", i).ToString();
                lObjPurchaseNote.File = DtMatrix.GetValue("C_File", i).ToString();
                lObjPurchaseNote.Aux = DtMatrix.GetValue("C_Aux", i).ToString();
                lObjPurchaseNote.CodeVoucher = DtMatrix.GetValue("C_CodeVoucher", i).ToString();
                lObjPurchaseNote.CodeMov = DtMatrix.GetValue("C_CodeMov", i).ToString();
                lObjPurchaseNote.AuxAfectable = DtMatrix.GetValue("C_AuxAf", i).ToString();
                lObjPurchaseNote.AGL = DtMatrix.GetValue("C_AGL", i).ToString();
                lObjPurchaseNote.Line = DtMatrix.GetValue("C_LineCode", i).ToString();
                lLstPurchaseNote.Add(lObjPurchaseNote);
            }
            return lLstPurchaseNote;
        }

        //Agrega las lineas a la tabla de comprobantes detalle
        private bool AddVoucherDetail(List<PurchaseNote> pLstObjDocument, string pStrDocentry)
        {
            bool lbolResult = false;
            try
            {
                //string lStrDocEntry = DIApplication.Company.GetNewObjectKey().ToString();
                //string lStrDocNum = mObjPurchasesDAO.GetDocNum(lStrDocEntry);
                double lDblTotal = pLstObjDocument.Sum(x => x.Amount);
                string lStrCodeVoucher = pLstObjDocument[0].CodeVoucher;
                VouchersDetail lObjVouchersDetail = new VouchersDetail();
                lObjVouchersDetail.NA = "N";
                lObjVouchersDetail.Coments = ""; //Puede haber varios comentarios 
                lObjVouchersDetail.Coment = "";
                lObjVouchersDetail.Date = mObjVouchers.Date;
                lObjVouchersDetail.DocNum = pStrDocentry;
                lObjVouchersDetail.DocEntry = pStrDocentry;
                lObjVouchersDetail.CodeVoucher = pLstObjDocument[0].CodeVoucher;
                lObjVouchersDetail.IEPS = 0;
                lObjVouchersDetail.ISR = 0;
                lObjVouchersDetail.IVA = 0;
                //lObjVouchersDetail.Provider = pObjDocument.Provider;
                lObjVouchersDetail.RetIVA = 0;
                lObjVouchersDetail.Status = "Cerrado";
                lObjVouchersDetail.Subtotal = lDblTotal;
                lObjVouchersDetail.Type = "Nota";
                lObjVouchersDetail.Total = lDblTotal;
                lObjVouchersDetail.Line = (mObjPurchaseServiceFactory.GetPurchaseVouchersService().GetVouchesDetail(lStrCodeVoucher).Count() + 1).ToString();
                if (mObjPurchaseServiceFactory.GetVouchersDetailService().Add(lObjVouchersDetail) == 0)
                {
                    LogService.WriteSuccess("PolicyDI (AddVoucherDetail) Detalle de comprobante agregado correctamente, Codigo de comprobante:" + pLstObjDocument[0].CodeVoucher);
                    if (mObjPurchaseServiceFactory.GetVouchersService().UpdateTotal(lObjVouchersDetail.CodeVoucher) == 0)
                    {
                        lbolResult = true;
                    }
                }
                else
                {
                    LogService.WriteError("PolicyDI (AddVoucherDetail)  Código de comprobante:" + pLstObjDocument[0].CodeVoucher + " Mensaje:" + DIApplication.Company.GetLastErrorDescription());
                    lbolResult = false;
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("PolicyDI (AddVoucherDetail) Código de comprobante:" + pLstObjDocument[0].CodeVoucher + " Mensaje:" + ex.Message);
                LogService.WriteError(ex);
                lbolResult = false;
            }
            return lbolResult;
        }

        /// <summary>
        /// Guarda el comprobante
        /// <summary>
        //private string SaveVoucher(Vouchers pObjVouchers)
        //{
        //    string lStrRowCode = string.Empty;
        //    try
        //    {
        //        string lStrFolio;
        //        string lStrArea;
        //        Vouchers lObjVouchers = pObjVouchers;

        //        lStrFolio = mObjPurchaseServiceFactory.GetPurchaseReceiptsService().GetVoucherFolio(txtArea.Value, (pObjVouchers.TypeVoucher.ToString()));
        //        int lIntFolio = Convert.ToInt32(lStrFolio);
        //        lStrFolio = (lIntFolio + 1).ToString();
        //        lObjVouchers.Folio = lStrFolio;
        //        if (mObjPurchaseServiceFactory.GetVouchersService().Add(lObjVouchers) != 0)
        //        {
        //            string lStrerror = DIApplication.Company.GetLastErrorDescription();
        //            UIApplication.ShowMessageBox(lStrerror);
        //            LogService.WriteError("SaveVoucher: " + lStrerror);
        //        }
        //        else
        //        {
        //            lStrFolio = lObjVouchers.Folio;
        //            lStrArea = lObjVouchers.Area;
        //            lStrRowCode = mObjPurchaseServiceFactory.GetPurchasePaymentService().GetVoucherCode(lStrFolio, lStrArea, lObjVouchers.TypeVoucher);
        //            txtFolio.Value = lStrFolio;
        //            LogService.WriteSuccess("SaveVoucher: Guardado correcto RowCode:" + lStrRowCode);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        UIApplication.ShowError(ex.Message);
        //        LogService.WriteError("SaveVoucher Folio #" + pObjVouchers.Folio + " Mensaje:" + ex.Message);
        //        LogService.WriteError(ex);
        //    }
        //    return lStrRowCode;
        //}
        #endregion

        #region Controls
        private SAPbouiCOM.StaticText lblFolio;
        private SAPbouiCOM.StaticText lblCredi;
        private SAPbouiCOM.StaticText lblProv;
        private SAPbouiCOM.StaticText lblAccount;
        private SAPbouiCOM.StaticText lblArea;
        private SAPbouiCOM.StaticText lblAF;
        private SAPbouiCOM.StaticText lblProject;
        private SAPbouiCOM.StaticText lblAmount;
        private SAPbouiCOM.StaticText lblAttach;
        private SAPbouiCOM.StaticText lblObs;
        private SAPbouiCOM.StaticText lblTotal;
        private SAPbouiCOM.EditText txtFolio;
        private SAPbouiCOM.EditText txtCredit;
        private SAPbouiCOM.EditText txtProv;
        private SAPbouiCOM.EditText txtAccount;
        private SAPbouiCOM.EditText txtArea;
        private SAPbouiCOM.EditText txtAF;
        private SAPbouiCOM.EditText txtProject;
        private SAPbouiCOM.EditText txtAmount;
        private SAPbouiCOM.EditText txtObs;
        private SAPbouiCOM.EditText txtAttach;
        private SAPbouiCOM.EditText txtTotal;
        private SAPbouiCOM.Button btnRemove;
        private SAPbouiCOM.Button btnAdd;
        private SAPbouiCOM.Button btnAttach;
        private SAPbouiCOM.Button btnCreate;
        private SAPbouiCOM.Matrix mtxNotes;
        private SAPbouiCOM.DataTable DtMatrix = null;
        private StaticText lblType;
        private ComboBox cboType;
        private EditText txtAux;
        private StaticText lblAux;
        private StaticText lblCodeMov;
        private ComboBox cboMovement;
        private StaticText lblSubidaMq;
        private ComboBox cboSubidaMq;
        private StaticText lblAGL;
        private EditText txtAGL;
        private StaticText lblLine;
        private EditText txtLinea;

        #endregion

        #region OpenFile
        /// <summary>
        /// Crea hilo para abrir carpeta 
        /// </summary>
        private void CreateFolderBroserThread()
        {
            try
            {
                Thread ShowFolderBroserThread = new Thread(OpenFileBrowser);
                if (ShowFolderBroserThread.ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    ShowFolderBroserThread.SetApartmentState(System.Threading.ApartmentState.STA);
                    ShowFolderBroserThread.Start();
                }
                else
                {
                    ShowFolderBroserThread.Start();
                    ShowFolderBroserThread.Join();

                }
                while (ShowFolderBroserThread.ThreadState == System.Threading.ThreadState.Running)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("frmPurchaseNote (CreateFolderBroserThread) " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Abre dialogo de seleccionar archivo
        /// </summary>
        private void OpenFileBrowser()
        {
            try
            {
                string lStrFileName = ShowFolderBrowser();
                txtAttach.Value = lStrFileName;
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseNote (OpenFileBrowser) " + ex.Message);
                LogService.WriteError(ex);
            }
        }


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        public string ShowFolderBrowser()
        {
            string lStrfileName = "";
            using (System.Windows.Forms.OpenFileDialog lObjFile = new System.Windows.Forms.OpenFileDialog())
            {
                try
                {
                    IntPtr sapProc = GetForegroundWindow();
                    WindowWrapper MyWindow = null;

                    MyWindow = new WindowWrapper(sapProc);

                    lObjFile.Multiselect = false;

                    //GetFileDialogFilter(pStrBank, lObjFile);
                    //oFile.Filter = "Archivos Excel(*.xls)|*.xls|Archivos TXT(*.txt)|*.txt|Archivos CSV(*.csv)|*.csv";
                    lObjFile.FilterIndex = 0;
                    lObjFile.RestoreDirectory = true;
                    var dialogResult = lObjFile.ShowDialog(MyWindow);

                    if (dialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        lStrfileName = lObjFile.FileName;
                    }
                }
                catch (Exception ex)
                {
                    LogService.WriteError("frmPurchaseNote (ShowFolderBrowser) " + ex.Message);
                    LogService.WriteError(ex);
                }
            }
            return lStrfileName;
        }
        #endregion














    }
}
