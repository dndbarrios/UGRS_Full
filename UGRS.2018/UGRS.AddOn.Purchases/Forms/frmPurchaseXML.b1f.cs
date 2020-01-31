using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using UGRS.AddOn.Purchases.Services;
using UGRS.AddOn.Purchases.Utilities;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.Purchases;
using UGRS.Core.SDK.DI.Purchases.DTO;
using UGRS.Core.SDK.DI.Purchases.Services;
using UGRS.Core.SDK.DI.Purchases.Tables;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;
using UGRS.Core.Utility;

namespace UGRS.AddOn.Purchases.Forms
{
    [FormAttribute("UGRS.AddOn.Purchases.Forms.frmPurchaseXML", "Forms/frmPurchaseXML.b1f")]
    class frmPurchaseXML : UserFormBase
    {
        #region Propieties
        PurchasesServiceFactory mObjPurchaseServiceFactory = new PurchasesServiceFactory();
        string mStrFileName = "";
        ChooseFromList mObjCFLAsset;
        ChooseFromList mObjCFLProject;
        string mStrArea;
        string mStrFolio;
        string mStrCodeVoucher;
        string mStrCodeLineVoucher;
        string mStrEmployee;
        string mStrUUID;
        int mIntRowSelected = 0;
        double mDblTotal = 0;
        string mStrAccount;
        bool mFlagSaveItem = false;
        bool mFlagSaveInv= false;
        bool mFlagPurshaseAddon = false;
        string mStrType = String.Empty;
        DateTime mDtmDate = DateTime.Now;
        Vouchers mObjVoucher = new Vouchers();
        private frmModalAF mObjModalAF = null;
        frmReceipts mObjBaseForm;


        PurchaseXMLDTO mObjPurchaseXMLDTO = new PurchaseXMLDTO();
        List<TaxesXMLDTO> mLstWithholdingTax = new List<TaxesXMLDTO>();
        #endregion

        #region Constructor
        /// <summary>
        /// Inicia la ventana
        /// </summary>
        public frmPurchaseXML(frmReceipts pObjForm, string pStrCodeVoucher, string pStrArea, string pStrEmployee, string pStrFolio, string pStrAccount, Vouchers pObjVoucher, bool pBolCopyComent, DateTime pDtmDate)
        {
            try
            {
                mObjBaseForm = pObjForm;
                mFlagPurshaseAddon = true;
                mtxXML.AutoResizeColumns();
                mStrArea = pStrArea;
                mStrEmployee = pStrEmployee;
                SetChooseToTxt();
                mStrCodeVoucher = pStrCodeVoucher;
                txtFolio.Value = pStrFolio;
                mStrFolio = pStrFolio;
                mStrAccount = pStrAccount;
                mObjVoucher = pObjVoucher;
                mDtmDate = pDtmDate;
                txtDate.Value = mDtmDate.ToString("yyyyMMdd");
                mStrType = String.IsNullOrEmpty(pObjVoucher.CodeMov) ? "CG" : "CV";

                if (pBolCopyComent)
                {
                    txtObs.Value = pObjVoucher.Coments;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("(frmPurchaseXML): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        public frmPurchaseXML()
        {
            mFlagPurshaseAddon = false;
            mStrArea = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetCostCenter();
            SetChooseToTxt();

        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.RightClickAfter += new SAPbouiCOM.Framework.FormBase.RightClickAfterHandler(this.Form_RightClickAfter);
            this.CloseAfter += new CloseAfterHandler(this.Form_CloseAfter);

        }

        private void OnCustomInitialize()
        {
            try
            {
                LoadChoseFromList();

                UIAPIRawForm.EnableMenu("1293", true);//Borrar
                UIAPIRawForm.EnableMenu("1294", true);//Duplicar

                txtCantD.Value = "1";
                AddComboboxMQ();

            }
            catch (Exception ex)
            {
                LogService.WriteError("(OnCustomInitialize): " + ex.Message);
                LogService.WriteError(ex);
            }
        }
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.btnBP = ((SAPbouiCOM.Button)(this.GetItem("btnBP").Specific));
            this.btnCancel = ((SAPbouiCOM.Button)(this.GetItem("btnCancel").Specific));
            this.btnItem = ((SAPbouiCOM.Button)(this.GetItem("btnItem").Specific));
            this.btnPDF = ((SAPbouiCOM.Button)(this.GetItem("btnPDF").Specific));
            this.btnSave = ((SAPbouiCOM.Button)(this.GetItem("btnSave").Specific));
            this.btnXML = ((SAPbouiCOM.Button)(this.GetItem("btnXML").Specific));
            this.txtAF = ((SAPbouiCOM.EditText)(this.GetItem("txtAF").Specific));
            this.txtAF.KeyDownAfter += new SAPbouiCOM._IEditTextEvents_KeyDownAfterEventHandler(this.txtAF_KeyDownAfter);
            this.txtArea = ((SAPbouiCOM.EditText)(this.GetItem("txtArea").Specific));
            this.txtBP = ((SAPbouiCOM.EditText)(this.GetItem("txtBP").Specific));
            this.txtCurren = ((SAPbouiCOM.EditText)(this.GetItem("txtCurren").Specific));
            this.txtFolio = ((SAPbouiCOM.EditText)(this.GetItem("txtFolio").Specific));
            this.txtIEPS = ((SAPbouiCOM.EditText)(this.GetItem("txtIEPS").Specific));
            this.txtName = ((SAPbouiCOM.EditText)(this.GetItem("txtName").Specific));
            this.txtObs = ((SAPbouiCOM.EditText)(this.GetItem("txtObs").Specific));
            this.txtPDF = ((SAPbouiCOM.EditText)(this.GetItem("txtPDF").Specific));
            this.txtProject = ((SAPbouiCOM.EditText)(this.GetItem("txtProject").Specific));
            this.txtRet4 = ((SAPbouiCOM.EditText)(this.GetItem("txtRet4").Specific));
            this.txtRetISR = ((SAPbouiCOM.EditText)(this.GetItem("txtRetISR").Specific));
            this.txtRetIva = ((SAPbouiCOM.EditText)(this.GetItem("txtRetIva").Specific));
            this.txtRFC = ((SAPbouiCOM.EditText)(this.GetItem("txtRFC").Specific));
            this.txtSubT = ((SAPbouiCOM.EditText)(this.GetItem("txtSubT").Specific));
            this.txtTax = ((SAPbouiCOM.EditText)(this.GetItem("txtTax").Specific));
            this.txtTotal = ((SAPbouiCOM.EditText)(this.GetItem("txtTotal").Specific));
            this.txtXML = ((SAPbouiCOM.EditText)(this.GetItem("txtXML").Specific));
            this.mtxXML = ((SAPbouiCOM.Matrix)(this.GetItem("mtxXML").Specific));
            this.mtxXML.KeyDownAfter += new SAPbouiCOM._IMatrixEvents_KeyDownAfterEventHandler(this.mtxXML_KeyDownAfter);
            this.lblAF = ((SAPbouiCOM.StaticText)(this.GetItem("lblAF").Specific));
            this.lblArea = ((SAPbouiCOM.StaticText)(this.GetItem("lblArea").Specific));
            this.lblBP = ((SAPbouiCOM.StaticText)(this.GetItem("lblBP").Specific));
            this.lblCurre = ((SAPbouiCOM.StaticText)(this.GetItem("lblCurre").Specific));
            this.lblFolio = ((SAPbouiCOM.StaticText)(this.GetItem("lblFolio").Specific));
            this.lblIEPS = ((SAPbouiCOM.StaticText)(this.GetItem("lblIEPS").Specific));
            this.lblName = ((SAPbouiCOM.StaticText)(this.GetItem("lblName").Specific));
            this.lblObs = ((SAPbouiCOM.StaticText)(this.GetItem("lblObs").Specific));
            this.lblPDF = ((SAPbouiCOM.StaticText)(this.GetItem("lblPDF").Specific));
            this.lblProyect = ((SAPbouiCOM.StaticText)(this.GetItem("lblProyect").Specific));
            this.lblRet4 = ((SAPbouiCOM.StaticText)(this.GetItem("lblRet4").Specific));
            this.lblRetISR = ((SAPbouiCOM.StaticText)(this.GetItem("lblRetISR").Specific));
            this.lblRetIva = ((SAPbouiCOM.StaticText)(this.GetItem("lblRetIva").Specific));
            this.lblRFC = ((SAPbouiCOM.StaticText)(this.GetItem("lblRFC").Specific));
            this.lblSubT = ((SAPbouiCOM.StaticText)(this.GetItem("lblSubT").Specific));
            this.lblTax = ((SAPbouiCOM.StaticText)(this.GetItem("lblTax").Specific));
            this.lblTotal = ((SAPbouiCOM.StaticText)(this.GetItem("lblTotal").Specific));
            this.lblXML = ((SAPbouiCOM.StaticText)(this.GetItem("lblXML").Specific));
            this.txtCantD = ((SAPbouiCOM.EditText)(this.GetItem("txtCantD").Specific));
            this.lblCantD = ((SAPbouiCOM.StaticText)(this.GetItem("lblCantD").Specific));
            this.mtxXML.ValidateAfter += new SAPbouiCOM._IMatrixEvents_ValidateAfterEventHandler(this.mtxXML_ValidateAfter);
            this.mtxXML.ClickBefore += new SAPbouiCOM._IMatrixEvents_ClickBeforeEventHandler(this.mtxXML_ClickBefore);
            this.btnBP.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnBP_ClickBefore);
            this.btnCancel.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCancel_ClickBefore);
            this.btnItem.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnItem_ClickBefore);
            this.btnPDF.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnPDF_ClickBefore);
            this.btnSave.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSave_ClickBefore);
            this.btnXML.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnXML_ClickBefore);
            UGRS.Core.SDK.UI.UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(this.SBO_Application_ItemEvent);
            // SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
            UGRS.Core.SDK.UI.UIApplication.GetApplication().MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(this.SBO_Application_MenuEvent);
            // SAPbouiCOM.Framework.Application.SBO_Application.MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(SBO_Application_MenuEvent);
            this.btnItem.Item.Visible = true;
            this.btnBP.Item.Visible = false;
            this.lblSub = ((SAPbouiCOM.StaticText)(this.GetItem("lblSub").Specific));
            this.cboSubida = ((SAPbouiCOM.ComboBox)(this.GetItem("cboSubida").Specific));
            this.lblDesc = ((SAPbouiCOM.StaticText)(this.GetItem("lblDesc").Specific));
            this.txtDesc = ((SAPbouiCOM.EditText)(this.GetItem("txtDesc").Specific));
            //             try
            //             {
            //         }
            //         catch (Exception ex)
            //         {
            //             LogService.WriteError("(OnInitializeComponent): " + ex.Message);
            //             LogService.WriteError(ex);
            //         }
            this.lblDate = ((SAPbouiCOM.StaticText)(this.GetItem("lblDate").Specific));
            this.txtDate = ((SAPbouiCOM.EditText)(this.GetItem("txtDate").Specific));
            this.lblItem = ((SAPbouiCOM.StaticText)(this.GetItem("lblItem").Specific));
            this.txtItem = ((SAPbouiCOM.EditText)(this.GetItem("txtItem").Specific));
            this.txtItem.ValidateAfter += new SAPbouiCOM._IEditTextEvents_ValidateAfterEventHandler(this.txtItem_ValidateAfter);
            this.lblItemName = ((SAPbouiCOM.StaticText)(this.GetItem("lblItemN").Specific));
            this.lblAdminOper = ((SAPbouiCOM.StaticText)(this.GetItem("lblAdmOper").Specific));
            this.cboAdminOper = ((SAPbouiCOM.ComboBox)(this.GetItem("cboAdmOper").Specific));
            this.OnCustomInitialize();

        }
        #endregion

        #region Events

        /// <summary>
        /// Eventos de la aplicacion
        /// </summary>
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
                        UIApplication.GetApplication().MenuEvent -= new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(this.SBO_Application_MenuEvent);

                    }

                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case BoEventTypes.et_CHOOSE_FROM_LIST:
                                ChooseFromListAfterEvent(pVal);
                                break;
                            case BoEventTypes.et_COMBO_SELECT:
                                if (pVal.ItemUID == "cboSubida")
                                {
                                    //this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Asset").ValueEx = "";
                                    //List<AssetsDTO> lLstAssetsDTO = new List<AssetsDTO>();
                                    //if (string.IsNullOrEmpty(cboSubida.Value))
                                    //{
                                    //    lLstAssetsDTO = mObjPurchaseServiceFactory.GetPurchaseService().GetAssets(txtArea.Value).ToList();
                                    //}
                                    //else
                                    //{
                                    //    lLstAssetsDTO = mObjPurchaseServiceFactory.GetPurchaseXmlService().GetRiseAF(cboSubida.Value).ToList();
                                    //}
                                    //AddConditionAssets(mObjCFLAsset, lLstAssetsDTO);
                                }
                                else
                                {
                                    SelectCombobox(pVal);
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (pVal.EventType)
                        {
                            case BoEventTypes.et_CHOOSE_FROM_LIST:
                                ChooseFromListBeforeEvent(pVal);
                                break;
                            case BoEventTypes.et_RIGHT_CLICK:
                                mtxXML.SelectRow(pVal.Row, true, false);
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

                                    if (!mObjModalAF.mBolForMatrix)
                                    {
                                        txtAF.Value = mObjModalAF.mStrAFCode;
                                    }
                                    else
                                    {
                                        this.UIAPIRawForm.Freeze(true);
                                        //string lStrAF = (mtxXML.Columns.Item("C_AF").Cells.Item(mIntRowSelected).Specific as EditText).Value;
                                        DtMatrix.SetValue("C_AF", mIntRowSelected - 1, mObjModalAF.mStrAFCode);
                                        mtxXML.LoadFromDataSource();
                                        this.UIAPIRawForm.Freeze(false);
                                    }
                                    mObjModalAF = null;
                                }
                                break;
                        }
                    }
                }



                if (mFlagSaveItem && FormUID.Equals(mFormItem.UniqueID) && pVal.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED)
                {
                    mFormItem.Close();
                    mFlagSaveItem = false;
                }


             

            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("ItemEventException: {0}", ex.Message));
                LogService.WriteError("(SBO_Application_ItemEvent): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void SBO_Application_FormDataEvent(ref BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (BusinessObjectInfo.EventType != BoEventTypes.et_FORM_ACTIVATE)
                {
                    if (mFormItem.UniqueID == BusinessObjectInfo.FormUID && !BusinessObjectInfo.BeforeAction &&
                        (BusinessObjectInfo.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD || BusinessObjectInfo.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE))
                    {
                        string lStrItemCode = ((SAPbouiCOM.EditText)(mFormItem.Items.Item("5").Specific)).Value;
                        string lStrItemName = ((SAPbouiCOM.EditText)(mFormItem.Items.Item("7").Specific)).Value;

                        DtMatrix.SetValue("C_Item", mIntRowSelected - 1, lStrItemCode);
                        DtMatrix.SetValue("C_ItemSAP", mIntRowSelected - 1, lStrItemName);
                        VerifyInvntItems();
                        mtxXML.LoadFromDataSource();
                        mFlagSaveItem = true;
                       
                    }
                }

               
            }
            catch (Exception ex)
            {
                LogService.WriteError("(SBO_Application_FormDataEvent): " + ex.Message);
                LogService.WriteError(ex);
            }

        }


        private void SBO_Application_FormDataEventDraft(ref BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (mFormDraftInv.UniqueID == BusinessObjectInfo.FormUID && !BusinessObjectInfo.BeforeAction &&
                     (BusinessObjectInfo.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD || BusinessObjectInfo.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE))
                {
                    string pStrDocEntryInv = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetCreatedInvoice(mStrFolio, mStrCodeLineVoucher);
                    mFlagSaveInv = true;
                    if (!string.IsNullOrEmpty(pStrDocEntryInv))
                    {
                       // DeleteDraft(lIntDraftEntry);
                        mObjBaseForm.UpdateMatriz();

                    }
                }
                if (mFlagSaveInv && mFormDraftInv.UniqueID == BusinessObjectInfo.FormUID && BusinessObjectInfo.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED)
                {
                    mFormDraftInv.Close();
                    mFlagSaveInv = false;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex);
            }
          
        }

        /// <summary>
        ///Evento de matriz para ver si se habilita el boton de item
        /// </summary>
        private void mtxXML_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (pVal.Row > 0)
                {
                    mtxXML.SelectRow(pVal.Row, true, false);
                    mIntRowSelected = pVal.Row;

                    //if (DtMatrix.GetValue("C_Item", pVal.Row - 1).ToString() == "")
                    //{
                    //    List<ItemDTO> lLstAssetsDTO = mObjPurchaseServiceFactory.GetPurchaseXmlService().GetItems(DtMatrix.GetValue("C_CodCla", pVal.Row - 1).ToString()).ToList();
                    //    if (lLstAssetsDTO.Count == 0)
                    //    {
                    //        btnItem.Item.Visible = true;
                    //    }
                    //    else
                    //    {
                    //        btnItem.Item.Visible = true; //false
                    //    }
                    //}
                    //else
                    //{
                    //    btnItem.Item.Visible = true; //false
                    //}
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(ex.Message);
                LogService.WriteError("(SBO_Application_ItemEvent): " + ex.Message);
                LogService.WriteError(ex);
            }


        }

        /// <summary>
        /// Boton PDF
        /// </summary>
        private void btnPDF_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                mStrFileName = "PDF";
                CreateFolderBroserThread();
                if (mStrFileName != "")
                {
                    txtPDF.Value = mStrFileName;
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(ex.Message);
                LogService.WriteError("(btnPDF_ClickBefore): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Boton xml
        /// </summary>
        private void btnXML_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            mStrFileName = "XML";
            CreateFolderBroserThread();

            if (mStrFileName != "")
            {
                try
                {
                    this.UIAPIRawForm.Freeze(true);
                    txtXML.Value = mStrFileName;
                    ReadXMLService lObjReadXMLService = new ReadXMLService();
                    PurchaseXMLDTO lObjPurchaseXML = lObjReadXMLService.ReadXML(mStrFileName);
                    mObjPurchaseXMLDTO = lObjPurchaseXML;

                    if (lObjPurchaseXML != null)
                    {
                        if (lObjPurchaseXML.ConceptLines != null && lObjPurchaseXML.ConceptLines.Count > 0 && lObjReadXMLService.CheckVoucherStatus(lObjPurchaseXML)) //timbre
                        {

                            if (DtMatrix == null)
                            {
                                CreateDatatableMatrix();
                            }
                            else
                            {
                                DtMatrix.Rows.Clear();
                            }
                            FillMatrix(lObjPurchaseXML);
                            mtxXML.LoadFromDataSource();

                            VerifyInvntItems();
                            LoadXMLData(lObjPurchaseXML);
                        }
                    }
                    else
                    {
                        ClearControls();
                    }
                }

                catch (Exception ex)
                {

                    this.UIAPIRawForm.Freeze(false);
                    UIApplication.ShowMessageBox(ex.Message);
                    LogService.WriteError("(btnXML_ClickBefore): " + ex.Message);
                    LogService.WriteError(ex);
                }
                finally
                {
                    this.UIAPIRawForm.Freeze(false);
                }
            }

        }

        /// <summary>
        /// Abrir Ventana de Socios
        /// </summary>
        private void btnBP_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            OpenFormBP();
        }

        /// <summary>
        /// Eventos del menu de duplicar y eliminar registros de la matriz
        /// </summary>
        private void SBO_Application_MenuEvent(ref MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {

                if (pVal.MenuUID == "1294" && pVal.BeforeAction == true) //Duplicar
                {
                    if (mtxXML.IsRowSelected(mIntRowSelected))
                    {
                        int lIntQtyD = Convert.ToInt32(txtCantD.Value);
                        for (int i = 1; i <= lIntQtyD; i++)
                        {
                            DuplicateLine(pVal);
                        }

                        UIAPIRawForm.EnableMenu("8801", false);// Eliminar
                        SetTotal();
                    }
                }
                if (pVal.MenuUID == "1293" && pVal.BeforeAction == true)//Borrar
                {
                    if (SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("¿Desea elimiar el item seleccionado?", 2, "Si", "No", "") == 1)
                    {
                        try
                        {
                            this.UIAPIRawForm.Freeze(true);
                            DtMatrix.Rows.Remove(mIntRowSelected - 1);
                            SetTotal();
                            UIAPIRawForm.EnableMenu("8801", false);
                        }
                        catch (Exception ex)
                        {
                            this.UIAPIRawForm.Freeze(false);
                            LogService.WriteError("(SBO_Application_MenuEvent 1293 true): " + ex.Message);
                            LogService.WriteError(ex);
                        }
                        finally
                        {
                            this.UIAPIRawForm.Freeze(false);
                        }
                    }
                    else
                    {
                        BubbleEvent = false;
                    }
                }
                if (pVal.MenuUID == "1293" && pVal.BeforeAction == false) //Borrar
                {
                    try
                    {
                        this.UIAPIRawForm.Freeze(true);
                        mtxXML.LoadFromDataSource();
                    }
                    catch (Exception ex)
                    {
                        this.UIAPIRawForm.Freeze(false);
                        LogService.WriteError("(SBO_Application_MenuEvent 1293 false): " + ex.Message);
                        LogService.WriteError(ex);
                    }
                    finally
                    {
                        this.UIAPIRawForm.Freeze(false);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("(SBO_Application_MenuEvent): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Abrir Ventana de Articulos
        /// </summary>
        private void btnItem_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (mIntRowSelected > 0)
            {
                OpenFormItem();
            }
            else
            {
                UIApplication.ShowMessageBox("Favor de seleccionar un item");
            }
        }


        /// <summary>
        /// Guardar documento y generar la factura y el pago
        /// </summary>
        private void btnSave_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            bool lBolSuccess = false;
            string lStrDocEntry = String.Empty;
            PurchaseXMLDTO lObjPurchaseXmlDTO = null;
            try
            {
                InvoiceDI lObjInvioceDI = new InvoiceDI();

                //if(ValidateTotals()){ Cambio por factura preliminar
                if (true)
                {
                    lObjPurchaseXmlDTO = GetInvoiceInfo();
                    lObjPurchaseXmlDTO.Folio = txtFolio.Value;

                    DIApplication.Company.StartTransaction();


                    //if (string.IsNullOrEmpty(lObjPurchaseXmlDTO.CodeVoucher) || Convert.ToInt32(lObjPurchaseXmlDTO.CodeVoucher) < 0)
                    //{
                    //     mStrCodeVoucher= SaveVoucher(mObjVoucher);
                    //     lObjPurchaseXmlDTO.CodeVoucher = mStrCodeVoucher;

                    //}

                    if (!string.IsNullOrEmpty(lObjPurchaseXmlDTO.CodeVoucher) || Convert.ToInt32(lObjPurchaseXmlDTO.CodeVoucher) > 0 || !mFlagPurshaseAddon)
                    {


                        lObjPurchaseXmlDTO.Type = mStrType;
                        lBolSuccess = lObjInvioceDI.CreateDocument(lObjPurchaseXmlDTO, mFlagPurshaseAddon);


                        if (lBolSuccess)
                        {
                            LogService.WriteSuccess("Factura generada correctamente");
                            lStrDocEntry = lObjPurchaseXmlDTO.DocEntry.ToString();
                            if (mFlagPurshaseAddon)
                            {

                                //SAPbobsCOM.Documents lObjDocInvoice = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
                                //lObjDocInvoice.GetByKey(Convert.ToInt32(lObjPurchaseXmlDTO.DocEntry));
                                //lObjPurchaseXmlDTO.Total = lObjDocInvoice.DocTotal.ToString();
                                lBolSuccess = AddVoucherDetail(lObjPurchaseXmlDTO);

                            }

                            if (!lBolSuccess)
                            {
                                UIApplication.ShowWarning("No fue posible guardar el detalle del comprobante debido a un error interno favor de intentarlo de nuevo o revisar el log");
                                LogService.WriteError("(btnSave_ClickBefore): " + "No fue posible guardar el detalle del comprobante: " + lObjPurchaseXmlDTO.CodeVoucher);
                            }
                        }
                        else
                        {
                            UIApplication.ShowWarning("No fue posible crear la factura debido a un error interno favor de intentarlo de nuevo o revisar el log");
                            LogService.WriteError("(btnSave_ClickBefore): " + " No fue posible crear la factura: " + lObjPurchaseXmlDTO.CodeVoucher);
                            lBolSuccess = false;
                        }
                    }
                    else
                    {
                        UIApplication.ShowWarning(" No fue posible crear el comprobante debido a un error interno favor de intentarlo de nuevo o revisar el log");
                        LogService.WriteError("(btnSave_ClickBefore): " + " No fue posible crear el comprobante: " + lObjPurchaseXmlDTO.CodeVoucher);
                        lBolSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("(btnSave_ClickBefore): " + ex.Message);
                LogService.WriteError(ex);
                lBolSuccess = false;

            }
            finally
            {
                try
                {
                    if (lBolSuccess)
                    {
                        DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        UIApplication.ShowMessageBox(string.Format("Documento preliminar realizado correctamente, Generando factura..."));

                        DraftToDocument(lObjPurchaseXmlDTO, true);
                        if (mObjBaseForm != null && lObjPurchaseXmlDTO != null && !lObjPurchaseXmlDTO.IsDraft )
                        {
                            this.UIAPIRawForm.Close();
                            mObjBaseForm.UpdateMatriz();
                        }
                        //open invoice draft form
                        else
                        {
                            SAPbouiCOM.Form lObjFormDraft = SAPbouiCOM.Framework.Application.SBO_Application.OpenForm((SAPbouiCOM.BoFormObjectEnum)112, "", lStrDocEntry);
                            this.UIAPIRawForm.Close();
                            mFormDraftInv = lObjFormDraft;
                            SAPbouiCOM.Framework.Application.SBO_Application.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(SBO_Application_FormDataEventDraft);
                        }

                        
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

        private void DraftToDocument(PurchaseXMLDTO pObjPurchase, bool pBolCreatePayment)
        {
            //string lStrDocEntry = DIApplication.Company.GetNewObjectKey().ToString();
            //pObjPurchase.DocEntry = Convert.ToInt32(lStrDocEntry);
            //SAPbobsCOM.Documents lObjDocInvoice = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
            SAPbobsCOM.Documents lObjDocInvoice = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
            lObjDocInvoice.GetByKey(Convert.ToInt32(pObjPurchase.DocEntry));

            decimal lFlDif = decimal.Parse(pObjPurchase.XMLTotal) - decimal.Parse(lObjDocInvoice.DocTotal.ToString());

            decimal lFlDifConfg = GetDifFact();
            if (lFlDif < lFlDifConfg && lFlDif > lFlDifConfg * (-1))
            {
                SAPbobsCOM.Documents lObjDocDraft = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                if (lFlDif != 0)
                {
                  
                    /*lObjDocInvoice.Rounding = BoYesNoEnum.tYES;
                    lObjDocInvoice.RoundingDiffAmount = Convert.ToDouble(lFlDif);
                   */
                    SAPbouiCOM.Form lObjFormDraft = AddRoundByUI(pObjPurchase.DocEntry, lFlDif);

                    //SaveAsDraft
                    UIApplication.GetApplication().Menus.Item("5907").Activate();

                    lObjFormDraft.Close();
                    //lObjDocDraft.GetByKey(Convert.ToInt32(lStrDocEntry));
                    //lObjDocDraft.Rounding = BoYesNoEnum.tYES;
                    //lObjDocDraft.RoundingDiffAmount = Convert.ToDouble(lFlDif);
                    //lObjDocDraft.Add();
                    //string lStrDraftEntry = DIApplication.Company.GetNewObjectKey().ToString();
                    //lObjDocInvoice.GetByKey(Convert.ToInt32(lStrDraftEntry));
                    //pObjPurchase.DocEntry = Convert.ToInt32( lStrDraftEntry);
                }

                lObjDocDraft.GetByKey(Convert.ToInt32(pObjPurchase.DocEntry));
                if (lObjDocDraft.SaveDraftToDocument() == 0)
                {
                    pObjPurchase.IsDraft = false;
                    int lIntDraftEntry = pObjPurchase.DocEntry;

                    pObjPurchase.DocEntry = Convert.ToInt32(DIApplication.Company.GetNewObjectKey());

                    if (pBolCreatePayment)
                    {
                        PaymentDI lObjPaymentDI = new PaymentDI();
                        pObjPurchase.Total = lObjDocDraft.DocTotal.ToString();// lObjDocInvoice.DocTotal.ToString();
                        lObjPaymentDI.CreatePayment(pObjPurchase);
                    }
                    DeleteDraft(lIntDraftEntry); 
                }
                else
                {
                    this.UIAPIRawForm.Close();
                    pObjPurchase.IsDraft = true;
                    UIApplication.ShowMessageBox(string.Format("No fue posible generar la factura a partir de la factura preliminar"+
                                                    "\n Se abrirá la pantalla de borrador "+
                                                    "\n Error: {0}", DIApplication.Company.GetLastErrorDescription()));
                    SAPbouiCOM.Framework.Application.SBO_Application.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(SBO_Application_FormDataEventDraft);
                    SAPbouiCOM.Form lObjFormDraft = AddRoundByUI(pObjPurchase.DocEntry, lFlDif);
                    mFormDraftInv = lObjFormDraft;
                }
            }
            else
            {
                this.UIAPIRawForm.Close();
                pObjPurchase.IsDraft = true;
                UIApplication.ShowMessageBox(string.Format("El redondeo {0} es mayor al indicado en configuración \n Se abrirá la pantalla de borrador", lFlDif));
                 SAPbouiCOM.Framework.Application.SBO_Application.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(SBO_Application_FormDataEventDraft);
                SAPbouiCOM.Form lObjFormDraft = AddRoundByUI(pObjPurchase.DocEntry, lFlDif);
                mFormDraftInv = lObjFormDraft;
              
            }
        }


        private SAPbouiCOM.Form AddRoundByUI(int pIntDraftEntry, decimal pDecDif)
        {
            SAPbouiCOM.Form lObjFormDraft = SAPbouiCOM.Framework.Application.SBO_Application.OpenForm((SAPbouiCOM.BoFormObjectEnum)112, "", pIntDraftEntry.ToString());
            CheckBox lObjChk = (CheckBox)lObjFormDraft.Items.Item("105").Specific;
            lObjChk.Checked = true;
            lObjChk.Checked = false;
            lObjChk.Checked = true;
            EditText lObjTxtRound = (EditText)lObjFormDraft.Items.Item("103").Specific;
            lObjTxtRound.Value = pDecDif.ToString();

            return lObjFormDraft;
        }

        private decimal GetDifFact()
        {
            string lStrDifFact = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetDifFact();
            decimal lFltDifFact = 0;
            if (!string.IsNullOrEmpty(lStrDifFact))
            {
                lFltDifFact = decimal.Parse(lStrDifFact);
            }
            return lFltDifFact;
        }

        public int DeleteDraft(int pIntDocEntryDraft)
        {
            SAPbobsCOM.Documents lObjDraft = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
            lObjDraft.GetByKey(pIntDocEntryDraft);
            return lObjDraft.Remove();
        }

        /// <summary>
        /// Guarda en Datatable los datos modificados en la matriz
        /// </summary>
        private void mtxXML_ValidateAfter(object sboObject, SBOItemEventArg pVal)//, out bool BubbleEvent)
        {
            // BubbleEvent = true;
            try
            {


                if (pVal.ColUID == "C_Cant" || pVal.ColUID == "C_Price")
                {
                    try
                    {
                        this.UIAPIRawForm.Freeze(true);
                        double lDblQuantity = Convert.ToDouble((mtxXML.Columns.Item("C_Cant").Cells.Item(pVal.Row).Specific as EditText).Value.Trim());
                        double lDblPrice = Convert.ToDouble((mtxXML.Columns.Item("C_Price").Cells.Item(pVal.Row).Specific as EditText).Value.Trim());
                        string lStrTasa = DtMatrix.GetValue("C_Tasa", pVal.Row - 1).ToString();
                        double lDblTasa = string.IsNullOrEmpty(lStrTasa) ? 0 : Convert.ToDouble(lStrTasa);
                        double lDblDesc = Convert.ToDouble((mtxXML.Columns.Item("C_Desc").Cells.Item(pVal.Row).Specific as EditText).Value.Trim());

                        double lDblSubtotal = lDblQuantity * lDblPrice;
                        double lDblTax = (lDblSubtotal - lDblDesc) * lDblTasa;
                        DtMatrix.SetValue("C_Cant", pVal.Row - 1, lDblQuantity);
                        DtMatrix.SetValue("C_Price", pVal.Row - 1, lDblPrice);
                        DtMatrix.SetValue("C_Total", pVal.Row - 1, lDblSubtotal);
                        DtMatrix.SetValue("C_SubTotal", pVal.Row - 1, lDblSubtotal);
                        DtMatrix.SetValue("C_Tax", pVal.Row - 1, lDblTax);

                        mtxXML.LoadFromDataSource();
                        SetTotal();
                    }
                    catch (Exception ex)
                    {
                        this.UIAPIRawForm.Freeze(false);
                        UIApplication.ShowError(ex.Message);
                        LogService.WriteError("(mtxXML_ValidateBefore C_Cant): " + ex.Message);
                        LogService.WriteError(ex);
                    }
                    finally
                    {
                        this.UIAPIRawForm.Freeze(false);
                    }

                }
                if (pVal.ColUID == "C_Total")
                {
                    try
                    {
                        this.UIAPIRawForm.Freeze(true);
                        double lDblTotal = Convert.ToDouble((mtxXML.Columns.Item("C_Total").Cells.Item(pVal.Row).Specific as EditText).Value.Trim());
                        DtMatrix.SetValue("C_Total", pVal.Row - 1, lDblTotal);
                        txtTotal.Value = lDblTotal.ToString();
                        mtxXML.LoadFromDataSource();
                        //SetTotal();
                    }
                    catch (Exception ex)
                    {
                        this.UIAPIRawForm.Freeze(false);
                        UIApplication.ShowError(ex.Message);
                        LogService.WriteError("(mtxXML_ValidateBefore C_Total): " + ex.Message);
                        LogService.WriteError(ex);
                    }
                    finally
                    {
                        this.UIAPIRawForm.Freeze(false);
                    }

                }
                if (pVal.ColUID == "C_AF")
                {
                    try
                    {
                        this.UIAPIRawForm.Freeze(true);
                        string lStrAF = (mtxXML.Columns.Item("C_AF").Cells.Item(pVal.Row).Specific as EditText).Value;
                        DtMatrix.SetValue("C_AF", pVal.Row - 1, lStrAF);

                        mtxXML.LoadFromDataSource();

                        //SetTotal();
                    }
                    catch (Exception ex)
                    {
                        this.UIAPIRawForm.Freeze(false);
                        UIApplication.ShowError(ex.Message);
                        LogService.WriteError("(mtxXML_ValidateBefore C_AF): " + ex.Message);
                        LogService.WriteError(ex);
                    }
                    finally
                    {
                        this.UIAPIRawForm.Freeze(false);
                    }

                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("(mtxXML_ValidateBefore): " + ex.Message);
                LogService.WriteError(ex);
            }

        }

        /// <summary>
        /// Evento de choosefromlist
        /// </summary>
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

                                case "CFL_BP":
                                    txtName.Value = System.Convert.ToString(lObjDataTable.GetValue(1, 0));
                                    txtRFC.Value = System.Convert.ToString(lObjDataTable.GetValue(23, 0));
                                    break;

                                case "CFL_Area":
                                    ////txtArea.Value = lObjDataTable.GetValue(0, 0).ToString();
                                    //this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Asset").ValueEx = "";
                                    //List<AssetsDTO> lLstAssetsDTO = new List<AssetsDTO>();
                                    //if (string.IsNullOrEmpty(cboSubida.Value))
                                    //{
                                    //    lLstAssetsDTO = mObjPurchaseServiceFactory.GetPurchaseService().GetAssets(txtArea.Value).ToList();
                                    //}
                                    //else
                                    //{
                                    //    lLstAssetsDTO = mObjPurchaseServiceFactory.GetPurchaseXmlService().GetRiseAF(cboSubida.Value).ToList();
                                    //}
                                    //AddConditionAssets(mObjCFLAsset, lLstAssetsDTO);
                                    //AddConditionChoseFromListProject(mObjCFLProject);
                                    break;

                                case "CFL_AreaMx":
                                    string lStrCostCenter = lObjDataTable.GetValue(0, 0).ToString();
                                    DtMatrix.SetValue("C_Area", pObjValEvent.Row - 1, lObjDataTable.GetValue(0, 0));
                                    DtMatrix.SetValue("C_AF", pObjValEvent.Row - 1, "");
                                    DtMatrix.SetValue("C_Whs", pObjValEvent.Row - 1, mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetWhouse(lObjDataTable.GetValue(0, 0).ToString()));
                                    mtxXML.LoadFromDataSource();
                                    break;

                                case "CFL_AssetM":
                                    DtMatrix.SetValue("C_AF", pObjValEvent.Row - 1, lObjDataTable.GetValue(0, 0));
                                    mtxXML.LoadFromDataSource();
                                    break;

                                case "CFL_C_Item":
                                    DtMatrix.SetValue("C_Item", pObjValEvent.Row - 1, lObjDataTable.GetValue(0, 0));
                                    DtMatrix.SetValue("C_ItemSAP", pObjValEvent.Row - 1, lObjDataTable.GetValue(1, 0));
                                    VerifyInvntItems();
                                    mtxXML.LoadFromDataSource();
                                    break;

                                case "CFL_Acc":
                                    DtMatrix.SetValue("C_Account", pObjValEvent.Row - 1, lObjDataTable.GetValue(0, 0));
                                    mtxXML.LoadFromDataSource();
                                    break;

                                case "CFL_C_Pro":
                                    //string ss = lObjDataTable.GetValue(0, 0).ToString();
                                    DtMatrix.SetValue("C_Project", pObjValEvent.Row - 1, lObjDataTable.GetValue(0, 0));
                                    mtxXML.LoadFromDataSource();
                                    break;

                                case "CFL_C_AGL":
                                    string sss = lObjDataTable.GetValue(0, 0).ToString();
                                    DtMatrix.SetValue("C_AGL", pObjValEvent.Row - 1, lObjDataTable.GetValue(0, 0));
                                    mtxXML.LoadFromDataSource();
                                    break;

                                case "CFL_Item":
                                    // txtItem.Value = System.Convert.ToString(lObjDataTable.GetValue(0, 0));
                                    lblItemName.Caption = System.Convert.ToString(lObjDataTable.GetValue(1, 0));
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

        /// <summary>
        /// Carga de choosefromlist antes de abrir
        /// </summary>
        private void ChooseFromListBeforeEvent(ItemEvent pObjValEvent)
        {
            try
            {
                SAPbouiCOM.IChooseFromListEvent lObjCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pObjValEvent;
                string lStrCflUID = lObjCFLEvento.ChooseFromListUID;
                ChooseFromList lObjChooseFromList = UIAPIRawForm.ChooseFromLists.Item(lStrCflUID);
                //AddConditionItems(lObjChooseFromList);
                if (lStrCflUID == "CFL_AssetM")
                {
                    //List<AssetsDTO> lLstAssetsDTO = new List<AssetsDTO>();
                    //if (string.IsNullOrEmpty(cboSubida.Value))
                    //{
                    //    lLstAssetsDTO = mObjPurchaseServiceFactory.GetPurchaseService().GetAssets(DtMatrix.GetValue("C_Area", pObjValEvent.Row - 1).ToString()).ToList();
                    //}
                    //else
                    //{
                    //    lLstAssetsDTO = mObjPurchaseServiceFactory.GetPurchaseXmlService().GetRiseAF(cboSubida.Value).ToList();
                    //}
                    //AddConditionAssets(lObjChooseFromList, lLstAssetsDTO);
                }

                if (lStrCflUID == "CFL_C_Item")
                {

                    /*List<ItemDTO> lLstItemDTO = mObjPurchaseServiceFactory.GetPurchaseXmlService().GetItems().ToList();

                    AddConditionItems(lObjChooseFromList, lLstItemDTO);*/
                }

                if (lStrCflUID == "CFL_C_Pro")
                {
                    AddConditionChoseFromListProject(lObjChooseFromList);
                }
                if (lStrCflUID == "CFL_Item")
                {
                    List<ItemDTO> lLstItemDTO = mObjPurchaseServiceFactory.GetPurchaseXmlService().GetItems().ToList();
                    AddConditionItems(lObjChooseFromList, lLstItemDTO);
                }


            }
            catch (Exception ex)
            {
                LogService.WriteError("(ChooseFromListBeforeEvent): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        ///Cerrar ventana
        /// </summary>
        private void btnCancel_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.UIAPIRawForm.Close();
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

        #region ChooseFromLists

        /// <summary>
        /// Carga los ChooseFromList
        /// <summary>
        private void LoadChoseFromList()
        {
            try
            {
                mObjCFLProject = InitChooseFromLists(false, "63", "CFL_Pro", this.UIAPIRawForm.ChooseFromLists);

                ChooseFromList lObjCFLProjectArea = InitChooseFromLists(false, "61", "CFL_Area", this.UIAPIRawForm.ChooseFromLists);
                AddConditionChoseFromListArea(lObjCFLProjectArea);

                //mObjCFLAsset = InitChooseFromLists(false, "61", "CFL_Asset", this.UIAPIRawForm.ChooseFromLists);


                ChooseFromList lObjCFLBP = InitChooseFromLists(false, "2", "CFL_BP", this.UIAPIRawForm.ChooseFromLists);
                AddConditionBP(lObjCFLBP);


                ChooseFromList lObjCFLProjectAreaMtx = InitChooseFromLists(false, "61", "CFL_AreaMx", this.UIAPIRawForm.ChooseFromLists);
                AddConditionChoseFromListArea(lObjCFLProjectAreaMtx);

                ChooseFromList lObjCFLAssetMtx = InitChooseFromLists(false, "61", "CFL_AssetM", this.UIAPIRawForm.ChooseFromLists);



                ChooseFromList lOBjCFLAccount = InitChooseFromLists(false, "1", "CFL_Acc", this.UIAPIRawForm.ChooseFromLists);
                AddContitionsAccount(lOBjCFLAccount);

                ChooseFromList mObjCFL_C_Proyect = InitChooseFromLists(false, "63", "CFL_C_Pro", this.UIAPIRawForm.ChooseFromLists);
                InitChooseFromLists(false, "62", "CFL_C_AGL", this.UIAPIRawForm.ChooseFromLists);

                ChooseFromList lObjCFLProjectItems = InitChooseFromLists(false, "4", "CFL_C_Item", this.UIAPIRawForm.ChooseFromLists);
                List<ItemDTO> lLstItemDTO = mObjPurchaseServiceFactory.GetPurchaseXmlService().GetItems().ToList();
                //mObjPurchaseServiceFactory.GetPurchaseXmlService().GetItems(DtMatrix.GetValue("C_CodCla", pObjValEvent.Row - 1).ToString()).ToList();
                AddConditionItems(lObjCFLProjectItems, lLstItemDTO);

                ChooseFromList lObjCFLItems = InitChooseFromLists(false, "4", "CFL_Item", this.UIAPIRawForm.ChooseFromLists);
                //List<ItemDTO> lLstItemDTO = mObjPurchaseServiceFactory.GetPurchaseXmlService().GetItems().ToList();
                //mObjPurchaseServiceFactory.GetPurchaseXmlService().GetItems(DtMatrix.GetValue("C_CodCla", pObjValEvent.Row - 1).ToString()).ToList();
                AddConditionItems(lObjCFLItems, lLstItemDTO);




            }
            catch (Exception ex)
            {
                LogService.WriteError("(LoadChoseFromList): " + ex.Message);
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
                txtProject.DataBind.SetBound(true, "", "CFL_Pro");
                txtProject.ChooseFromListUID = "CFL_Pro";
                txtProject.ChooseFromListAlias = "PrjCode";

                txtArea.DataBind.SetBound(true, "", "CFL_Area");
                txtArea.ChooseFromListUID = "CFL_Area";
                txtArea.ChooseFromListAlias = "PrcCode";

                this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Area").ValueEx = mStrArea;
                //List<AssetsDTO> lLstAssetsDTO = mObjPurchaseServiceFactory.GetPurchaseService().GetAssets(txtArea.Value).ToList();
                //AddConditionAssets(mObjCFLAsset, lLstAssetsDTO);
                AddConditionChoseFromListProject(mObjCFLProject);

                //txtAF.DataBind.SetBound(true, "", "CFL_Asset");
                //txtAF.ChooseFromListUID = "CFL_Asset";
                //txtAF.ChooseFromListAlias = "PrcCode";

                txtBP.DataBind.SetBound(true, "", "CFL_BP");
                txtBP.ChooseFromListUID = "CFL_BP";
                txtBP.ChooseFromListAlias = "CardCode";

                txtItem.DataBind.SetBound(true, "", "CFL_Item");
                txtItem.ChooseFromListUID = "CFL_Item";
                txtItem.ChooseFromListAlias = "ItemCode";


            }
            catch (Exception ex)
            {
                LogService.WriteError("(SetChooseToTxt): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        #region ConditionsChoodefromlist
        /// <summary>
        /// Condiciones de chooseFromList
        /// <summary>
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
                LogService.WriteError("(AddConditionChoseFromListProject): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Condiciones de chooseFromList
        /// <summary>
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

        /// <summary>
        /// Condiciones de chooseFromList
        /// <summary>
        private void AddConditionAssets(ChooseFromList pCFL, List<AssetsDTO> pLstAssetsDTO)
        {
            try
            {
                SAPbouiCOM.Condition lObjCon = null;
                SAPbouiCOM.Conditions lObjCons = new Conditions();
                pCFL.SetConditions(lObjCons);
                int i = 1;
                if (pLstAssetsDTO.Count() > 0)
                {
                    foreach (AssetsDTO lObjAssetDTO in pLstAssetsDTO)
                    {
                        lObjCon = lObjCons.Add();
                        lObjCon.Alias = "PrcCode";
                        lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        lObjCon.CondVal = lObjAssetDTO.PrcCode;

                        if (pLstAssetsDTO.Count() > i)
                        {
                            lObjCon.Relationship = BoConditionRelationship.cr_OR;
                        }
                        i++;
                    }
                }
                else
                {
                    lObjCon = lObjCons.Add();
                    lObjCon.Alias = "PrcCode";
                    lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                    lObjCon.CondVal = "";
                }
                pCFL.SetConditions(lObjCons);
            }
            catch (Exception ex)
            {
                LogService.WriteError("(AddConditionAssets): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Condiciones de chooseFromList
        /// <summary>
        private void AddConditionItems(ChooseFromList pCFL, List<ItemDTO> pLstItemDTO)
        {
            try
            {


                SAPbouiCOM.Condition lObjCon = null;
                SAPbouiCOM.Conditions lObjCons = new Conditions();
                pCFL.SetConditions(lObjCons);
                int i = 1;


                if (pLstItemDTO.Count() > 0)
                {
                    foreach (ItemDTO lObjItemDTO in pLstItemDTO)
                    {
                        lObjCon = lObjCons.Add();
                        lObjCon.Alias = "ItemCode";
                        lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        lObjCon.CondVal = lObjItemDTO.ItemCode;

                        if (pLstItemDTO.Count() > i)
                        {
                            lObjCon.Relationship = BoConditionRelationship.cr_OR;
                        }
                        i++;
                    }
                }
                else
                {
                    lObjCon = lObjCons.Add();
                    lObjCon.Alias = "ItemCode";
                    lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                    lObjCon.CondVal = "";
                }
                pCFL.SetConditions(lObjCons);

                //SAPbouiCOM.Column lObjColumnItem = mtxXML.Columns.Item("C_Item");
                //string ss = lObjColumnItem.ChooseFromListAlias;
                //lObjColumnItem.ChooseFromListAlias = "ItemCode";
            }
            catch (Exception ex)
            {
                LogService.WriteError("(AddConditionItems): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Condiciones de chooseFromList
        /// <summary>
        private void AddConditionBP(ChooseFromList pCFL)
        {
            try
            {
                SAPbouiCOM.Condition lObjCon = null;
                SAPbouiCOM.Conditions lObjCons = null;
                lObjCons = pCFL.GetConditions();

                //DimCode
                lObjCon = lObjCons.Add();
                lObjCon.Alias = "CardType";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = "S";

                pCFL.SetConditions(lObjCons);
            }
            catch (Exception ex)
            {
                LogService.WriteError("(AddConditionBP): " + ex.Message);
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

                /*lObjCon = lObjCons.Add();
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

        #endregion

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
                UIApplication.ShowMessageBox(string.Format("InitCustomerChooseFromListException: {0}", ex.Message));
                LogService.WriteError("(InitChooseFromLists): " + ex.Message);
                LogService.WriteError(ex);
            }
            return lObjoCFL;
        }



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
                LogService.WriteError("(CreateFolderBroserThread): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Abre dialogo de seleccionar archivo
        /// </summary>
        private void OpenFileBrowser()
        {
            mStrFileName = ShowFolderBrowser();

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
                    if (mStrFileName == "PDF")
                    {
                        lObjFile.Filter = "Archivos PDF|*.PDF";
                        lObjFile.Title = "Selecciona el archivo PDF";
                    }
                    if (mStrFileName == "XML")
                    {
                        lObjFile.Filter = "Archivos XML|*.XML";
                        lObjFile.Title = "Selecciona el archivo XML";
                    }
                    lObjFile.RestoreDirectory = true;
                    var dialogResult = lObjFile.ShowDialog(MyWindow);

                    if (dialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        lStrfileName = lObjFile.FileName;
                        LogService.WriteSuccess("(ShowFolderBrowser): Archivo " + lStrfileName + " Cargado correctamente");
                    }
                }
                catch (Exception ex)
                {
                    UIApplication.ShowMessageBox(ex.Message);
                    LogService.WriteError("(ShowFolderBrowser): " + ex.Message);
                    LogService.WriteError(ex);
                }
            }
            return lStrfileName;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Duplica una linea en la Matriz
        /// <summary>
        private void DuplicateLine(MenuEvent pVal)
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);
                mtxXML.LoadFromDataSource();

                DtMatrix.Rows.Add();
                int i = 0;
                foreach (var item in DtMatrix.Columns)
                {

                    DtMatrix.SetValue(i, DtMatrix.Rows.Count - 1, DtMatrix.GetValue(i, mIntRowSelected - 1));
                    i++;
                }
                DtMatrix.SetValue(i, DtMatrix.Rows.Count - 1, DtMatrix.GetValue(i, mIntRowSelected - 1));


                for (int j = 0; j < DtMatrix.Rows.Count; j++)
                {
                    DtMatrix.SetValue("#", j, j + 1);
                }

                mtxXML.LoadFromDataSource();
                SetTotal();
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                LogService.WriteError("(DuplicateLine): " + ex.Message);
                LogService.WriteError(ex);

            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        /// <summary>
        /// Carga los datos del XML en el formulario
        /// <summary>
        private void LoadXMLData(PurchaseXMLDTO pObjPurchaseXML)
        {
            try
            {
                txtCurren.Value = pObjPurchaseXML.CurrencyDocument;
                txtName.Value = pObjPurchaseXML.BPName;
                txtRFC.Value = pObjPurchaseXML.RFCProvider;
                txtSubT.Value = Convert.ToDouble(pObjPurchaseXML.SubTotal).ToString();
                txtTax.Value = Convert.ToDouble(pObjPurchaseXML.TaxesTransfers).ToString();
                mDblTotal = Convert.ToDouble(pObjPurchaseXML.Total);


                txtTotal.Value = mDblTotal.ToString();
                mStrUUID = pObjPurchaseXML.FolioFiscal;

                SetTotal();

                string lStrBP = mObjPurchaseServiceFactory.GetPurchaseXmlService().GetBussinesPartner(pObjPurchaseXML.RFCProvider);
                if (lStrBP == "")
                {
                    btnBP.Item.Visible = true;
                }
                else
                {
                    btnBP.Item.Visible = false;
                }
                this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_BP").ValueEx = lStrBP;
                mObjPurchaseXMLDTO.CardCode = lStrBP;
                mObjPurchaseXMLDTO.CodeVoucher = mStrCodeVoucher;
            }
            catch (Exception ex)
            {
                LogService.WriteError("(LoadXMLData): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Abre ventana de socios de negocio y se envian los datos
        /// <summary>
        private void OpenFormBP()
        {
            try
            {
                SAPbouiCOM.Framework.Application.SBO_Application.ActivateMenuItem("2561");
                mFormBP = SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetFormByTypeAndCount(134, -1);
                SAPbouiCOM.Framework.Application.SBO_Application.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(SBO_Application_FormDataEventBP);

                mFormBP.Mode = BoFormMode.fm_ADD_MODE;
                txtProviderRFC = ((SAPbouiCOM.EditText)(mFormBP.Items.Item("41").Specific));
                txtProviderName = ((SAPbouiCOM.EditText)(mFormBP.Items.Item("7").Specific));
                //var ss = mFormBP.Items.Item("1320002080");
                cboSeries = ((SAPbouiCOM.ComboBox)(mFormBP.Items.Item("1320002080").Specific));
                cboCardType = ((SAPbouiCOM.ComboBox)(mFormBP.Items.Item("40").Specific));


                // cboSeries.Select("Manual");
                cboCardType.Select("Vendor");
                txtProviderRFC.Value = txtRFC.Value;
                txtProviderName.Value = txtName.Value;
            }
            catch (Exception ex)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(ex.Message);
                LogService.WriteError("(OpenFormBP): " + ex.Message);
                LogService.WriteError(ex);
                //PayrollInvoiceLogService.WriteError("Abrir ventana de factura: " + ex.Message);
            }
        }

        private void SBO_Application_FormDataEventBP(ref BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (BusinessObjectInfo.EventType != BoEventTypes.et_FORM_ACTIVATE)
                {
                    if (mFormBP.UniqueID == BusinessObjectInfo.FormUID && !BusinessObjectInfo.BeforeAction &&
                        (BusinessObjectInfo.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD || BusinessObjectInfo.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE))
                    {
                        string lStrBPCode = ((SAPbouiCOM.EditText)(mFormBP.Items.Item("5").Specific)).Value;
                        string lStrBPName = ((SAPbouiCOM.EditText)(mFormBP.Items.Item("7").Specific)).Value;
                        string lStrBPRFC = ((SAPbouiCOM.EditText)(mFormBP.Items.Item("41").Specific)).Value;

                        txtBP.Value = lStrBPCode;
                        txtName.Value = lStrBPName;
                        txtRFC.Value = lStrBPRFC;




                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("(SBO_Application_FormDataEvent): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Abre ventana de  Articulos y se envian los datos
        /// <summary>
        private void OpenFormItem()
        {
            try
            {
                SAPbouiCOM.Framework.Application.SBO_Application.ActivateMenuItem("3073");
                mFormItem = SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetFormByTypeAndCount(150, -1);
                //mFormItem.
                SAPbouiCOM.Framework.Application.SBO_Application.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(SBO_Application_FormDataEvent);
                mFormItem.Mode = BoFormMode.fm_ADD_MODE;
                txtItemDescription = ((SAPbouiCOM.EditText)(mFormItem.Items.Item("7").Specific));
                txtItemClassificationCode = ((SAPbouiCOM.EditText)(mFormItem.Items.Item("170002038").Specific));


                txtItemDescription.Value = DtMatrix.GetValue("C_DescFact", mIntRowSelected - 1).ToString();
                txtItemClassificationCode.Value = DtMatrix.GetValue("C_CodCla", mIntRowSelected - 1).ToString();

            }
            catch (Exception ex)
            {
                LogService.WriteError("(OpenFormItem): " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Crea las columnas que tendra el DataTable
        /// <summary>
        private void CreateDatatableMatrix()
        {
            try
            {
                this.UIAPIRawForm.DataSources.DataTables.Add("XmlConcepts");
                DtMatrix = this.UIAPIRawForm.DataSources.DataTables.Item("XmlConcepts");
                DtMatrix.Columns.Add("#", SAPbouiCOM.BoFieldsType.ft_ShortNumber);
                DtMatrix.Columns.Add("C_Item", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_ItemSAP", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_DescFact", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_CodCla", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Admin", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Area", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_AF", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Cant", SAPbouiCOM.BoFieldsType.ft_Quantity);
                DtMatrix.Columns.Add("C_Price", SAPbouiCOM.BoFieldsType.ft_Price);
                DtMatrix.Columns.Add("C_Tasa", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Tax", SAPbouiCOM.BoFieldsType.ft_Price);
                DtMatrix.Columns.Add("C_Total", SAPbouiCOM.BoFieldsType.ft_Price);
                DtMatrix.Columns.Add("C_TaxCode", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_TaxRate", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_IEPS", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Whs", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Account", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_HasTax", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_HasWth", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_Desc", SAPbouiCOM.BoFieldsType.ft_Price);
                DtMatrix.Columns.Add("C_SubTotal", SAPbouiCOM.BoFieldsType.ft_Price);
                DtMatrix.Columns.Add("C_Project", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
                DtMatrix.Columns.Add("C_AGL", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);




                SAPbouiCOM.Column lObjColumnItem = mtxXML.Columns.Item("C_Item");
                lObjColumnItem.DataBind.SetBound(true, "", "CFL_C_Item");
                lObjColumnItem.ChooseFromListUID = "CFL_C_Item";
                lObjColumnItem.ChooseFromListAlias = "ItemCode";

                //SAPbouiCOM.Column lObjColumnAsset = mtxXML.Columns.Item("C_AF");
                //lObjColumnAsset.DataBind.SetBound(true, "", "CFL_AssetM");
                //lObjColumnAsset.ChooseFromListUID = "CFL_AssetM";
                //lObjColumnAsset.ChooseFromListAlias = "PrcCode";

                SAPbouiCOM.Column lObjColumnAccount = mtxXML.Columns.Item("C_Account");
                lObjColumnAccount.DataBind.SetBound(true, "", "CFL_Acc");
                lObjColumnAccount.ChooseFromListUID = "CFL_Acc";
                lObjColumnAccount.ChooseFromListAlias = "AcctCode";

                SAPbouiCOM.Column lObjColumn = mtxXML.Columns.Item("C_Area");
                lObjColumn.DataBind.SetBound(true, "", "CFL_AreaMx");
                lObjColumn.ChooseFromListUID = "CFL_AreaMx";
                lObjColumn.ChooseFromListAlias = "PrcCode";

                SAPbouiCOM.Column lObjColumnProject = mtxXML.Columns.Item("C_Project");
                lObjColumnProject.DataBind.SetBound(true, "", "CFL_C_Pro");
                lObjColumnProject.ChooseFromListUID = "CFL_C_Pro";
                lObjColumnProject.ChooseFromListAlias = "PrjCode";

                SAPbouiCOM.Column lObjColumnAGL = mtxXML.Columns.Item("C_AGL");
                lObjColumnAGL.DataBind.SetBound(true, "", "CFL_C_AGL");
                lObjColumnAGL.ChooseFromListUID = "CFL_C_AGL";
                lObjColumnAGL.ChooseFromListAlias = "OcrCode";


            }
            catch (Exception ex)
            {
                LogService.WriteError("(CreateDatatableMatrix): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Llena el Datatable y llena la matrix con los datos
        /// <summary>
        private void FillMatrix(PurchaseXMLDTO pObjXML)
        {
            try
            {
                int i = 0;
                string lStrTaxCode;
                mLstWithholdingTax = new List<TaxesXMLDTO>();
                foreach (ConceptsXMLDTO lObjConcepts in pObjXML.ConceptLines)
                {

                    i = DtMatrix.Rows.Count;

                    double lDblRate = 0;
                    if (lObjConcepts.LstTaxes != null && lObjConcepts.LstTaxes.Count > 0)
                    {
                        if (lObjConcepts.LstTaxes.Where(x => x.Tax == "002").Count() > 0)
                        {
                            string lStrRate = lObjConcepts.LstTaxes.Where(x => x.Tax == "002").FirstOrDefault().Rate;
                            lDblRate = string.IsNullOrEmpty(lStrRate) ? 0 : Convert.ToDouble(lStrRate);
                        }
                    }

                    lStrTaxCode = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetTaxCode((lDblRate * 100).ToString()); //
                    DtMatrix.Rows.Add();
                    DtMatrix.SetValue("#", i, i + 1);
                    DtMatrix.SetValue("C_CodCla", i, lObjConcepts.ClassificationCode);
                    int ss = lObjConcepts.Description.Count();
                    DtMatrix.SetValue("C_DescFact", i, lObjConcepts.Description.Count() > 253 ? lObjConcepts.Description.Substring(0, 250) + "..." : lObjConcepts.Description);
                    DtMatrix.SetValue("C_Cant", i, lObjConcepts.Quantity);
                    DtMatrix.SetValue("C_Price", i, lObjConcepts.UnitPrice);
                    string lStrTax = string.Empty;


                    if (lObjConcepts.LstTaxes != null && lObjConcepts.LstTaxes.Count > 0)
                    {

                        DtMatrix.SetValue("C_Tasa", i, lObjConcepts.LstTaxes.Select(x => x.Rate).First().ToString());
                        lStrTax = lObjConcepts.LstTaxes.Where(y => y.Tax != "003").Where(y => y.Amount != "").Sum(x => Convert.ToDecimal(x.Amount)).ToString();
                        DtMatrix.SetValue("C_Tax", i, string.IsNullOrEmpty(lStrTax) ? "0" : lStrTax);
                        List<TaxesXMLDTO> lLstTaxesIEPS = new List<TaxesXMLDTO>();
                        lLstTaxesIEPS = lObjConcepts.LstTaxes.Where(x => x.Tax == "003").ToList();
                        if (lLstTaxesIEPS.Count() > 0)
                        {
                            AddIepsToDT(lLstTaxesIEPS);
                        }
                    }

                    Decimal lDecTax = string.IsNullOrEmpty(lStrTax) ? 0 : Convert.ToDecimal(lStrTax);
                    Decimal lDecDiscount = string.IsNullOrEmpty(lObjConcepts.Discount) ? 0 : Convert.ToDecimal(lObjConcepts.Discount);

                    //decimal lDecSubtotal = Convert.ToDecimal(lObjConcepts.Amount);
                    decimal lDecSubtotal = Convert.ToDecimal(lObjConcepts.UnitPrice) * Convert.ToDecimal(lObjConcepts.Quantity);
                    DtMatrix.SetValue("C_SubTotal", i, lDecSubtotal.ToString());
                    DtMatrix.SetValue("C_TaxCode", i, lStrTaxCode);
                    DtMatrix.SetValue("C_TaxRate", i, lDblRate);
                    DtMatrix.SetValue("C_Desc", i, lObjConcepts.Discount == null ? "0" : lObjConcepts.Discount);
                    DtMatrix.SetValue("C_Total", i, (Convert.ToDecimal(lObjConcepts.Amount) + lDecTax - lDecDiscount).ToString());
                    DtMatrix.SetValue("C_Area", i, txtArea.Value);
                    DtMatrix.SetValue("C_AF", i, txtAF.Value);
                    DtMatrix.SetValue("C_Project", i, txtProject.Value);
                    DtMatrix.SetValue("C_Admin", i, string.IsNullOrEmpty(cboAdminOper.Value) ? "A" : cboAdminOper.Value);
                    DtMatrix.SetValue("C_HasTax", i, lObjConcepts.LstTaxes != null && lObjConcepts.LstTaxes.Count > 0 ? "Y" : "N");
                    DtMatrix.SetValue("C_HasWth", i, lObjConcepts.LstWithholdingTax != null && lObjConcepts.LstWithholdingTax.Count > 0 ? "Y" : "N");

                    DtMatrix.SetValue("C_Whs", i, GetWhs(lObjConcepts.CodeItmProd, txtArea.Value));
                    if (lObjConcepts.AdmOper == "O")
                    {

                        DtMatrix.SetValue("C_Account", i, mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetCostAccount(lObjConcepts.CodeItmProd));
                    }
                    //List<ItemDTO> lLstItemDTO = mObjPurchaseServiceFactory.GetPurchaseXmlService().GetItems().ToList();
                    List<ItemDTO> lLstItemDTO = mObjPurchaseServiceFactory.GetPurchaseXmlService().GetItems(DtMatrix.GetValue("C_CodCla", i).ToString()).ToList();
                    if (lLstItemDTO.Count == 1)
                    {
                        DtMatrix.SetValue("C_Item", i, lLstItemDTO[0].ItemCode);
                        DtMatrix.SetValue("C_ItemSAP", i, lLstItemDTO[0].ItemName);
                    }
                    else
                    {
                        DtMatrix.SetValue("C_Item", i, txtItem.Value);
                        DtMatrix.SetValue("C_ItemSAP", i, lblItemName.Caption);
                    }
                    //(mtxXML.Columns.Item("C_Area").Cells.Item(0).Specific as EditText).Value = "OG_AUINT";

                    i++;
                    if (lObjConcepts.LstWithholdingTax != null && lObjConcepts.LstWithholdingTax.Count() > 0)
                    {
                        mLstWithholdingTax.AddRange(lObjConcepts.LstWithholdingTax);
                    }

                }

                if (pObjXML.LstLocalTax != null && pObjXML.LstLocalTax.Count() > 0)
                {
                    List<TaxesXMLDTO> lLstTaxesLocal = new List<TaxesXMLDTO>();
                    lLstTaxesLocal = pObjXML.LstLocalTax.Where(x => x.Tax == "ISH").ToList();
                    if (lLstTaxesLocal.Count() > 0)
                    {
                        AddLocalTax(lLstTaxesLocal);
                    }
                }

                mtxXML.Columns.Item("#").DataBind.Bind("XmlConcepts", "#");
                mtxXML.Columns.Item("C_CodCla").DataBind.Bind("XmlConcepts", "C_CodCla");
                mtxXML.Columns.Item("C_DescFact").DataBind.Bind("XmlConcepts", "C_DescFact");
                mtxXML.Columns.Item("C_Cant").DataBind.Bind("XmlConcepts", "C_Cant");
                mtxXML.Columns.Item("C_Price").DataBind.Bind("XmlConcepts", "C_Price");
                mtxXML.Columns.Item("C_Tasa").DataBind.Bind("XmlConcepts", "C_Tasa");
                mtxXML.Columns.Item("C_Tax").DataBind.Bind("XmlConcepts", "C_Tax");
                mtxXML.Columns.Item("C_Total").DataBind.Bind("XmlConcepts", "C_Total");
                mtxXML.Columns.Item("C_Area").DataBind.Bind("XmlConcepts", "C_Area");
                mtxXML.Columns.Item("C_Admin").DataBind.Bind("XmlConcepts", "C_Admin");
                mtxXML.Columns.Item("C_AF").DataBind.Bind("XmlConcepts", "C_AF");
                mtxXML.Columns.Item("C_Item").DataBind.Bind("XmlConcepts", "C_Item");
                mtxXML.Columns.Item("C_ItemSAP").DataBind.Bind("XmlConcepts", "C_ItemSAP");
                mtxXML.Columns.Item("C_Whs").DataBind.Bind("XmlConcepts", "C_Whs");
                mtxXML.Columns.Item("C_Account").DataBind.Bind("XmlConcepts", "C_Account");
                mtxXML.Columns.Item("C_Desc").DataBind.Bind("XmlConcepts", "C_Desc");
                mtxXML.Columns.Item("C_SubT").DataBind.Bind("XmlConcepts", "C_SubTotal");
                mtxXML.Columns.Item("C_Project").DataBind.Bind("XmlConcepts", "C_Project");
                mtxXML.Columns.Item("C_AGL").DataBind.Bind("XmlConcepts", "C_AGL");



                AddComboboxAdmin();
                //DtMatrix.Columns.Item("CredTotal").Cells.Item(i).Value = lObjTran.CreditTotal.ToString("C");
                LogService.WriteSuccess("(FillMatrix): Carga de datos de manera correcta");

            }
            catch (Exception ex)
            {

                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("(FillMatrix): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void AddIepsToDT(List<TaxesXMLDTO> lLstTaxesIEPS)
        {
            try
            {
                string lStrItemCode = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetItemIEPS();


                if (!string.IsNullOrEmpty(lStrItemCode))
                {
                    int i = DtMatrix.Rows.Count;

                    foreach (TaxesXMLDTO lObjTaxes in lLstTaxesIEPS)
                    {
                        string lStrTaxCode = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetTaxCode((Convert.ToDouble(lObjTaxes.Rate) * 100).ToString()); //
                        //if (!string.IsNullOrEmpty(lStrTaxCode))
                        //{
                        DtMatrix.Rows.Add();

                        DtMatrix.SetValue("#", i, i + 1);

                        DtMatrix.SetValue("C_Item", i, lStrItemCode);
                        DtMatrix.SetValue("C_ItemSAP", i, "IEPS");
                        DtMatrix.SetValue("C_Cant", i, 1);
                        DtMatrix.SetValue("C_Price", i, lObjTaxes.Amount);
                        DtMatrix.SetValue("C_TaxCode", i, lStrTaxCode);
                        DtMatrix.SetValue("C_Subtotal", i, lObjTaxes.Amount);
                        DtMatrix.SetValue("C_Total", i, lObjTaxes.Amount);
                        DtMatrix.SetValue("C_Project", i, txtProject.Value);
                        DtMatrix.SetValue("C_Area", i, txtArea.Value);
                        DtMatrix.SetValue("C_AF", i, txtAF.Value);
                        DtMatrix.SetValue("C_Admin", i, string.IsNullOrEmpty(cboAdminOper.Value) ? "A" : cboAdminOper.Value);
                        DtMatrix.SetValue("C_Whs", i, mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetWhouse(txtArea.Value));
                        //}
                        i++;

                    }
                }
                else
                {
                    UIApplication.ShowMessageBox("No se encontro el articulo IEPS");
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("(AddIepsToDT): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void AddLocalTax(List<TaxesXMLDTO> pLstTaxesLocal)
        {
            try
            {
                string lStrItemCode = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetLocalTaxt();
                if (!string.IsNullOrEmpty(lStrItemCode))
                {
                    int i = DtMatrix.Rows.Count;
                    foreach (TaxesXMLDTO lObjTaxes in pLstTaxesLocal)
                    {
                        string lStrTaxCode = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetTaxCode((Convert.ToDouble(lObjTaxes.Rate) * 100).ToString()); //
                        DtMatrix.Rows.Add();
                        DtMatrix.SetValue("#", i, i + 1);
                        DtMatrix.SetValue("C_Item", i, lStrItemCode);
                        DtMatrix.SetValue("C_ItemSAP", i, "ISH");
                        DtMatrix.SetValue("C_Cant", i, 1);
                        DtMatrix.SetValue("C_Price", i, lObjTaxes.Amount);
                        DtMatrix.SetValue("C_TaxCode", i, lStrTaxCode);
                        DtMatrix.SetValue("C_Subtotal", i, lObjTaxes.Amount);
                        DtMatrix.SetValue("C_Total", i, lObjTaxes.Amount);
                        DtMatrix.SetValue("C_Project", i, txtProject.Value);
                        DtMatrix.SetValue("C_Area", i, txtArea.Value);
                        DtMatrix.SetValue("C_AF", i, txtAF.Value);
                        DtMatrix.SetValue("C_Admin", i, string.IsNullOrEmpty(cboAdminOper.Value) ? "A" : cboAdminOper.Value);
                        DtMatrix.SetValue("C_Whs", i, mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetWhouse(txtArea.Value));
                        i++;
                    }
                }
                else
                {
                    UIApplication.ShowMessageBox("No se encontro el articulo IEPS");
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("(AddIepsToDT): " + ex.Message);
                LogService.WriteError(ex);
            }
        }


        private string GetWhs(string pStrItemCode, string pStrArea)
        {
            string lStrWhsMQ = "";
            if (!string.IsNullOrEmpty(cboSubida.Value) && !string.IsNullOrEmpty(pStrItemCode))
            {
                lStrWhsMQ = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetMQWhs(pStrItemCode);
            }

            if (string.IsNullOrEmpty(lStrWhsMQ))
            {
                return mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetWhouse(pStrArea);
            }
            else
            {
                return lStrWhsMQ;
            }

        }

        private void VerifyInvntItems()
        {
            if (mFlagPurshaseAddon)
            {
                try
                {
                    this.UIAPIRawForm.Freeze(true);
                    string lStrCodeItmProd;
                    string lStrWhsCodeDropShip;
                    for (int i = 0; i < DtMatrix.Rows.Count; i++)
                    {
                        lStrCodeItmProd = DtMatrix.GetValue("C_Item", i).ToString();
                        if (mObjPurchaseServiceFactory.GetPurchaseXmlService().IsInvntItem(lStrCodeItmProd))
                        {
                            SAPbouiCOM.CommonSetting lObjRowCtrl;
                            lObjRowCtrl = mtxXML.CommonSetting;
                            lObjRowCtrl.SetCellEditable(i + 1, 7, false);
                            lObjRowCtrl.SetCellBackColor(i + 1, 7, 14737632); //16777215 FFFFFF
                            lObjRowCtrl.SetCellEditable(i + 1, 5, false);

                            lStrWhsCodeDropShip = mObjPurchaseServiceFactory.GetPurchaseXmlService().GetWhsDropShip();
                            //DtMatrix.SetValue("C_Account", i, "5010020003000");// modificar esto
                            string lStrWhsMQ = "";
                            if (!string.IsNullOrEmpty(cboSubida.Value))
                            {
                                lStrWhsMQ = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetMQWhs(lStrCodeItmProd);
                            }

                            if (!string.IsNullOrEmpty(lStrWhsMQ))
                            {
                                DtMatrix.SetValue("C_Whs", i, lStrWhsMQ);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(lStrWhsCodeDropShip))
                                {
                                    DtMatrix.SetValue("C_Whs", i, lStrWhsCodeDropShip);
                                }
                                else
                                {
                                    UIApplication.ShowMessageBox("No se encotro el almacen de entrega directa");
                                    LogService.WriteError("No se encontro el almacen de entrega directa");
                                }
                            }
                        }
                        else
                        {

                            SAPbouiCOM.CommonSetting lObjRowCtrl;
                            lObjRowCtrl = mtxXML.CommonSetting;
                            lObjRowCtrl.SetCellEditable(i + 1, 7, false);
                            lObjRowCtrl.SetCellBackColor(i + 1, 7, 14737632); //E0E0E0
                            lObjRowCtrl.SetCellEditable(i + 1, 5, true);
                            DtMatrix.SetValue("C_Account", i, "");
                            DtMatrix.SetValue("C_Whs", i, GetWhs(lStrCodeItmProd, txtArea.Value));

                        }
                    }
                    mtxXML.LoadFromDataSource();

                }
                catch (Exception ex)
                {
                    LogService.WriteError("frmPurchaseXML (VerifyInvntItems)" + ex.Message);
                }
                finally
                {
                    this.UIAPIRawForm.Freeze(false);
                }
            }


        }

        /// <summary>
        /// Valores validos del combobox 
        /// <summary>
        private void AddComboboxAdmin()
        {
            try
            {
                SAPbouiCOM.Column lObjColumn = (SAPbouiCOM.Column)mtxXML.Columns.Item("C_Admin");
                SAPbouiCOM.Cells lObjCells = lObjColumn.Cells;

                if (lObjColumn.ValidValues.Count == 0)
                {
                    lObjColumn.ValidValues.Add("A", "Administración");
                    lObjColumn.ValidValues.Add("O", "Operativo");
                    lObjColumn.DisplayDesc = true;
                }
                lObjColumn.ComboSelectAfter += lObjColumn_ComboSelectAfter;
            }
            catch (Exception ex)
            {
                LogService.WriteError("(AddComboboxAdmin): " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void AddComboboxMQ()
        {
            try
            {
                List<string> lLstStringRises = mObjPurchaseServiceFactory.GetPurchaseXmlService().GetMqSubidas();
                cboSubida.ValidValues.Add("", "");
                foreach (string lStrIdRise in lLstStringRises)
                {
                    cboSubida.ValidValues.Add(lStrIdRise, "");
                }
                cboSubida.ExpandType = BoExpandType.et_ValueOnly;

            }
            catch (Exception ex)
            {

                LogService.WriteError("(AddComboboxMQ): " + ex.Message);
                LogService.WriteError(ex);
            }
        }


        private void lObjColumn_ComboSelectAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);

                string lStrAdminOper = DtMatrix.GetValue("C_Admin", pVal.Row - 1).ToString();
                if (lStrAdminOper == "O")
                {
                    string lStrItemCode = DtMatrix.GetValue("C_Item", pVal.Row - 1).ToString();
                    DtMatrix.SetValue("C_Account", pVal.Row - 1, mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetCostAccount(lStrItemCode));
                }
                else
                {
                    DtMatrix.SetValue("C_Account", pVal.Row - 1, "");
                }
                mtxXML.LoadFromDataSource();
            }
            catch (Exception ex)
            {
                LogService.WriteError("frmPurchaseXML (lObjColumn_ComboSelectAfter): " + ex.Message);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        /// <summary>
        /// Guarda el valor seleccionado  del combobox en DataTable 
        /// <summary>
        private void SelectCombobox(ItemEvent pObjValEvent)
        {

            try
            {
                if (pObjValEvent.ItemUID == "mtxXML")
                {
                    SAPbouiCOM.Cell lObjCell = mtxXML.Columns.Item("C_Admin").Cells.Item(pObjValEvent.Row);
                    SAPbouiCOM.ComboBox lObjCombobox = ((SAPbouiCOM.ComboBox)(lObjCell.Specific));

                    DtMatrix.SetValue("C_Admin", pObjValEvent.Row - 1, lObjCombobox.Value);
                    this.UIAPIRawForm.Freeze(true);
                    mtxXML.LoadFromDataSource();
                }



            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("(SelectCombobox): " + ex.Message);
                LogService.WriteError(ex);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }

        }

        /// <summary>
        /// Seleccionar un renglon de la matriz
        /// <summary>
        private void SelectRow(int pIntRow, string pStrColumnID)
        {
            //string lStrLineStatus = mObjQueryManager.GetValue("LineStatus", "DocEntry", mDBDataSourceD.GetValue("DocEntry", i), "RDR1");
            //if (lStrLineStatus == "C")
            //{
            //    DisableRow(i);
            //}

            try
            {
                this.UIAPIRawForm.Freeze(true);

                mtxXML.SelectRow(pIntRow, true, false);

            }
            catch (Exception lObjException)
            {
                UIApplication.ShowError(string.Format("Matriz: {0}", lObjException.Message));
                LogService.WriteError("(SelectRow): " + lObjException.Message);
                LogService.WriteError(lObjException);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }


        /// <summary>
        /// Valida los totales de la matriz
        /// <summary>
        private bool ValidateTotals()
        {
            try
            {
                double lDblTotal = Double.Parse(txtTotal.Value, System.Globalization.NumberStyles.Currency);
                if (mDblTotal != lDblTotal && mDblTotal != lDblTotal + 0.01 && mDblTotal != lDblTotal - 0.01)
                {
                    UIApplication.ShowMessageBox("El total no es igual al del xml cargado: " + mDblTotal.ToString("C") + "\n Diferencia: " + (Convert.ToDecimal(mDblTotal) - Convert.ToDecimal(lDblTotal)).ToString());
                    return false;
                }
                else
                {
                    txtTotal.Value = mDblTotal.ToString();
                }

                if (string.IsNullOrEmpty(txtDate.Value))
                {
                    UIApplication.ShowMessageBox("Favor de capturar la fecha de vencimiento");

                    return false;
                }


                for (int i = 0; i < DtMatrix.Rows.Count; i++)
                {
                    string lStrCodeItmProd = DtMatrix.GetValue("C_Item", i).ToString();
                    //if (mObjPurchaseServiceFactory.GetPurchaseXmlService().IsInvntItem(lStrCodeItmProd) && string.IsNullOrEmpty(DtMatrix.GetValue("C_Account", i).ToString()))
                    //{
                    //    UIApplication.ShowMessageBox("Favor de capturar la cuenta en linea "+ i);
                    //    return false;
                    //}
                }

            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox("Ocurrio un error al momento de validar los totales");
                LogService.WriteError("(ValidateTotals): " + ex.Message);
                LogService.WriteError(ex);
                return false;
            }


            return true;

        }

        /// <summary>
        /// Obtiene los datos de la matriz
        /// <summary>
        private PurchaseXMLDTO GetInvoiceInfo()
        {
            try
            {

                mObjPurchaseXMLDTO.Area = txtArea.Value;
                mObjPurchaseXMLDTO.Employee = mStrEmployee;
                mObjPurchaseXMLDTO.XMLFile = txtXML.Value;
                mObjPurchaseXMLDTO.PDFFile = txtPDF.Value;
                //mObjPurchaseXMLDTO.Folio =
                mObjPurchaseXMLDTO.CodeVoucher = mStrCodeVoucher;
                mObjPurchaseXMLDTO.FolioFiscal = mStrUUID == null ? "" : mStrUUID;
                mObjPurchaseXMLDTO.BPName = txtBP.Value;
                mObjPurchaseXMLDTO.CardCode = this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_BP").ValueEx;
                mObjPurchaseXMLDTO.WithholdingTax = mLstWithholdingTax;
                mObjPurchaseXMLDTO.Ieps = Double.Parse(txtIEPS.Value, System.Globalization.NumberStyles.Currency);
                mObjPurchaseXMLDTO.Iva = Double.Parse(txtTax.Value, System.Globalization.NumberStyles.Currency);
                mObjPurchaseXMLDTO.RetISR = Double.Parse(txtRetISR.Value, System.Globalization.NumberStyles.Currency);
                mObjPurchaseXMLDTO.RetIva = Double.Parse(txtRetIva.Value, System.Globalization.NumberStyles.Currency);
                mObjPurchaseXMLDTO.RetIva4 = Double.Parse(txtRet4.Value, System.Globalization.NumberStyles.Currency);
                mObjPurchaseXMLDTO.Obs = txtObs.Value;
                mObjPurchaseXMLDTO.Account = mStrAccount;
                mObjPurchaseXMLDTO.MQRise = cboSubida.Value;
                mObjPurchaseXMLDTO.CodeMov = mObjVoucher.CodeMov;
                mObjPurchaseXMLDTO.Total = txtTotal.Value;
                mObjPurchaseXMLDTO.Type = mStrType;
                mObjPurchaseXMLDTO.RowLine = (mObjPurchaseServiceFactory.GetPurchaseVouchersService().GetVouchesDetail(mStrCodeVoucher).Count() + 1).ToString();
                mObjPurchaseXMLDTO.TaxDate = string.IsNullOrEmpty(txtDate.Value) ? DateTime.Now :
                                DateTime.ParseExact(txtDate.Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                mObjPurchaseXMLDTO.DocDate = mDtmDate;
                mStrCodeLineVoucher = mObjPurchaseXMLDTO.RowLine;
               // mObjPurchaseXMLDTO.XMLTotal = mObjPurchaseXMLDTO.XMLTotal;

                List<ConceptsXMLDTO> lLstObjConceptsXML = new List<ConceptsXMLDTO>();
                for (int i = 0; i < DtMatrix.Rows.Count; i++)
                {
                    ConceptsXMLDTO lObjConceptsDTO = new ConceptsXMLDTO()
                    {
                        TaxCode = DtMatrix.GetValue("C_TaxCode", i).ToString(),
                        TaxRate = DtMatrix.GetValue("C_TaxRate", i).ToString(),
                        CodeItmProd = DtMatrix.GetValue("C_Item", i).ToString(),
                        Description = DtMatrix.GetValue("C_DescFact", i).ToString(),
                        Discount = DtMatrix.GetValue("C_Desc", i).ToString(),
                        ClassificationCode = DtMatrix.GetValue("C_CodCla", i).ToString(),
                        Quantity = DtMatrix.GetValue("C_Cant", i).ToString(),
                        UnitPrice = DtMatrix.GetValue("C_Price", i).ToString(),
                        CostingCode = DtMatrix.GetValue("C_Area", i).ToString(),
                        Subtotal = DtMatrix.GetValue("C_SubTotal", i).ToString(),
                        Amount = DtMatrix.GetValue("C_Total", i).ToString(),
                        AdmOper = DtMatrix.GetValue("C_Admin", i).ToString(),
                        WareHouse = DtMatrix.GetValue("C_Whs", i).ToString(),
                        Account = DtMatrix.GetValue("C_Account", i).ToString(),
                        HasTax = DtMatrix.GetValue("C_HasTax", i).ToString() == "Y" ? true : false,
                        HasWht = DtMatrix.GetValue("C_HasWth", i).ToString() == "Y" ? true : false,
                        AF = DtMatrix.GetValue("C_AF", i).ToString(),
                        Project = DtMatrix.GetValue("C_Project", i).ToString(),
                        AGL = DtMatrix.GetValue("C_AGL", i).ToString(),

                    };
                    lLstObjConceptsXML.Add(lObjConceptsDTO);
                }
                mObjPurchaseXMLDTO.ConceptLines = lLstObjConceptsXML;
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("(ReadDataTable): " + ex.Message);
                LogService.WriteError(ex);
            }
            return mObjPurchaseXMLDTO;
        }

        /// <summary>
        /// Carga el total en los EditText
        /// <summary>
        private void SetTotal()
        {
            try
            {
                double lDblRetIva = 0;
                double lDblRet4 = 0;
                double lDblRetISR = 0;
                double lDblTotal = 0;
                double lDblTax = 0;
                double lDblIeps = 0;
                double lDblDesc = 0;
                double lDblSubTotal = 0;
                for (int i = 0; i < DtMatrix.Rows.Count; i++)
                {
                    string lStrItemSAP = DtMatrix.GetValue("C_ItemSAP", i).ToString();
                    if (lStrItemSAP.Equals("IEPS"))
                    {
                        lDblIeps += Convert.ToDouble(DtMatrix.GetValue("C_Total", i));
                    }
                    else
                    {
                        lDblTotal += Convert.ToDouble(DtMatrix.GetValue("C_Total", i));
                        lDblSubTotal += Convert.ToDouble(DtMatrix.GetValue("C_SubTotal", i));
                    }

                    lDblRetIva = mLstWithholdingTax.Where(x => x.Tax == "002" && Convert.ToDouble(x.Rate) != 0.04).Sum(y => Convert.ToDouble(y.Amount));


                    lDblRet4 = mLstWithholdingTax.Where(x => x.Tax == "002" && Convert.ToDouble(x.Rate) == 0.04).Sum(y => Convert.ToDouble(y.Amount));


                    lDblRetISR = mLstWithholdingTax.Where(x => x.Tax == "001").Sum(y => Convert.ToDouble(y.Amount));

                    txtRetIva.Value = lDblRetIva.ToString();
                    txtRet4.Value = lDblRet4.ToString();
                    txtRetISR.Value = lDblRetISR.ToString();
                    lDblDesc += Convert.ToDouble(DtMatrix.GetValue("C_Desc", i));
                    lDblRet4 = mObjPurchaseXMLDTO.RetIva4 = Double.Parse(txtRet4.Value, System.Globalization.NumberStyles.Currency);
                    lDblTax += Convert.ToDouble(DtMatrix.GetValue("C_Tax", i));
                }

                //lDblTax = Math.Round(100 * lDblTax) / 100;
                //lDblSubTotal = Math.Round(100 * lDblSubTotal) / 100;
                //lDblTotal = Math.Round(100 * lDblTotal) / 100;
                //lDblIeps = Math.Round(100 * lDblIeps) / 100;
                //lDblRetIva = Math.Round(100 * lDblRetIva) / 100;
                //lDblRet4 = Math.Round(100 * lDblRet4) / 100;
                //lDblRetISR = Math.Round(100 * lDblRetISR) / 100;
                //lDblDesc = Math.Round(100 * lDblDesc) / 100;

                txtIEPS.Value = lDblIeps.ToString();
                txtSubT.Value = lDblSubTotal.ToString();
                txtDesc.Value = lDblDesc.ToString();
                txtTax.Value = lDblTax.ToString();
                double ss = lDblTax + lDblTotal + lDblIeps - lDblRetIva - lDblRet4 - lDblRetISR;// -lDblDesc;
                txtTotal.Value = (lDblTax + lDblSubTotal + lDblIeps - lDblRetIva - lDblRet4 - lDblRetISR - lDblDesc).ToString();
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError("(SetTotal): " + ex.Message);
                LogService.WriteError(ex);
            }

        }

        /// <summary>
        /// Limpia los controles del formulario
        /// <summary>
        private void ClearControls()
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);
                if (txtBP != null)
                {
                    txtBP.Value = string.Empty;
                    txtName.Value = string.Empty;
                    txtRFC.Value = string.Empty;
                    txtObs.Value = string.Empty;
                    txtCurren.Value = string.Empty;
                    txtAF.Value = string.Empty;
                    txtProject.Value = string.Empty;
                    txtXML.Value = string.Empty;
                    txtPDF.Value = string.Empty;

                    if (DtMatrix != null)
                    {
                        DtMatrix.Rows.Clear();
                        mtxXML.LoadFromDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                LogService.WriteError("(ClearControls): " + ex.Message);
                LogService.WriteError(ex);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);

            }


            //txtdate
        }


        /// <summary>
        /// Agregar detalles al comprobante
        /// </summary>
        private bool AddVoucherDetail(PurchaseXMLDTO pObjDocument)
        {
            try
            {
                //Cambio factura preliminar
                /* string lStrDocNum = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetDocNum(pObjDocument.DocEntry.ToString());

                 string lStrDocStatus = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetDocCanceled(pObjDocument.DocEntry.ToString(), "XML");
                 if(!lStrDocStatus.Equals("Cancelado")) {
                     lStrDocStatus = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetDocStatus(pObjDocument.DocEntry.ToString());
                 }*/



                VouchersDetail lObjVouchersDetail = new VouchersDetail();
                lObjVouchersDetail.NA = "N";
                lObjVouchersDetail.Coments = pObjDocument.Obs;//pObjDocument.
                lObjVouchersDetail.Coment = "";
                lObjVouchersDetail.Date = Convert.ToDateTime(pObjDocument.Date);
                lObjVouchersDetail.DocEntry = pObjDocument.DocEntry.ToString();
                //lObjVouchersDetail.DocNum = lStrDocNum; //Comentado por fatura preliminar
                lObjVouchersDetail.CodeVoucher = pObjDocument.CodeVoucher;

                lObjVouchersDetail.IEPS = pObjDocument.Ieps;
                //lObjVouchersDetail.IEPS = pObjDocument.ConceptLines.SelectMany(b => b.LstTaxes.Where(x => x.Tax == "003")).Sum(x => Convert.ToDouble(x.Amount));//.Sum(x => Convert.ToDouble(x.Amount));
                lObjVouchersDetail.ISR = pObjDocument.RetISR;
                lObjVouchersDetail.IVA = pObjDocument.Iva;
                lObjVouchersDetail.RetIVA = pObjDocument.RetIva + pObjDocument.RetIva4;


                lObjVouchersDetail.Provider = pObjDocument.CardCode;

                //lObjVouchersDetail.Status = lStrDocStatus; //Comentado por fatura preliminar
                lObjVouchersDetail.Subtotal = Convert.ToDouble(pObjDocument.SubTotal);
                lObjVouchersDetail.Type = "XML";
                lObjVouchersDetail.Total = Convert.ToDouble(pObjDocument.Total);
                lObjVouchersDetail.UserCode = UIApplication.GetCompany().UserName;
                lObjVouchersDetail.Line = pObjDocument.RowLine;

                if (mObjPurchaseServiceFactory.GetVouchersDetailService().Add(lObjVouchersDetail) == 0)
                {
                    LogService.WriteSuccess("InvoiceDI AddDetail" + lObjVouchersDetail.DocNum);


                    if (mObjPurchaseServiceFactory.GetVouchersService().UpdateTotal(pObjDocument.CodeVoucher) != 0)
                    {
                        string lstr = DIApplication.Company.GetLastErrorDescription();
                        LogService.WriteError("InvoiceDI (UpdateTotal) " + DIApplication.Company.GetLastErrorDescription());
                    }
                    else
                    {
                        LogService.WriteSuccess("InvoiceDI UpdateTotal" + lObjVouchersDetail.DocNum);
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
        private SAPbouiCOM.Button btnBP;
        private SAPbouiCOM.Button btnCancel;
        private SAPbouiCOM.Button btnItem;
        private SAPbouiCOM.Button btnPDF;
        private SAPbouiCOM.Button btnSave;
        private SAPbouiCOM.Button btnXML;
        private SAPbouiCOM.EditText txtAF;
        private SAPbouiCOM.EditText txtArea;
        private SAPbouiCOM.EditText txtBP;
        private SAPbouiCOM.EditText txtCurren;
        private SAPbouiCOM.EditText txtFolio;
        private SAPbouiCOM.EditText txtIEPS;
        private SAPbouiCOM.EditText txtName;
        private SAPbouiCOM.EditText txtObs;
        private SAPbouiCOM.EditText txtPDF;
        private SAPbouiCOM.EditText txtProject;
        private SAPbouiCOM.EditText txtRet4;
        private SAPbouiCOM.EditText txtRetISR;
        private SAPbouiCOM.EditText txtRetIva;
        private SAPbouiCOM.EditText txtRFC;
        private SAPbouiCOM.EditText txtSubT;
        private SAPbouiCOM.EditText txtTax;
        private SAPbouiCOM.EditText txtTotal;
        private SAPbouiCOM.EditText txtXML;
        private SAPbouiCOM.EditText txtCantD;
        private SAPbouiCOM.Matrix mtxXML;
        private SAPbouiCOM.StaticText lblAF;
        private SAPbouiCOM.StaticText lblArea;
        private SAPbouiCOM.StaticText lblBP;
        private SAPbouiCOM.StaticText lblCurre;
        private SAPbouiCOM.StaticText lblIEPS;
        private SAPbouiCOM.StaticText lblName;
        private SAPbouiCOM.StaticText lblObs;
        private SAPbouiCOM.StaticText lblPDF;
        private SAPbouiCOM.StaticText lblProyect;
        private SAPbouiCOM.StaticText lblRet4;
        private SAPbouiCOM.StaticText lblRetISR;
        private SAPbouiCOM.StaticText lblRetIva;
        private SAPbouiCOM.StaticText lblRFC;
        private SAPbouiCOM.StaticText lblSubT;
        private SAPbouiCOM.StaticText lblTax;
        private SAPbouiCOM.StaticText lblTotal;
        private SAPbouiCOM.StaticText lblXML;
        private SAPbouiCOM.StaticText lblFolio;
        private SAPbouiCOM.StaticText lblCantD;

        private StaticText lblItem;
        private EditText txtItem;
        private StaticText lblDesc;
        private EditText txtDesc;
        private StaticText lblDate;
        private EditText txtDate;
        private StaticText lblItemName;


        private SAPbouiCOM.Form mFormBP;
        private SAPbouiCOM.EditText txtProviderRFC;
        private SAPbouiCOM.EditText txtProviderName;
        private SAPbouiCOM.ComboBox cboCardType;
        private SAPbouiCOM.ComboBox cboSeries;
        private SAPbouiCOM.DataTable DtMatrix = null;

        private SAPbouiCOM.Form mFormItem;
        private SAPbouiCOM.EditText txtItemDescription;
        private SAPbouiCOM.EditText txtItemClassificationCode;

        private SAPbouiCOM.Form mFormDraftInv;

        private StaticText lblSub;
        private ComboBox cboSubida;

        private StaticText lblAdminOper;
        private ComboBox cboAdminOper;

        #endregion

        private void txtItem_ValidateAfter(object sboObject, SBOItemEventArg pVal)
        {
            if (string.IsNullOrEmpty(txtItem.Value))
            {
                lblItemName.Caption = string.Empty;
            }
        }

        private void Form_RightClickAfter(ref ContextMenuInfo eventInfo)
        {
            mtxXML.AutoResizeColumns();

        }

        private void Form_CloseAfter(SBOItemEventArg pVal)
        {
            UnLoadEvents();

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

                    mObjModalAF = new frmModalAF(txtArea.Value, txtAF.Value, cboSubida.Value);
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

        private void mtxXML_KeyDownAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                if (pVal.ColUID == "C_AF")
                {
                    if (pVal.CharPressed == 13)
                    {
                        if (UIUtility.IsFormOpen("UGRS.AddOn.Purchases.Forms.frmModalAF"))
                        {
                            UIApplication.GetApplication().Forms.Item("frmMdlAF").Close();
                        }

                        string lStrArea = (mtxXML.Columns.Item("C_Area").Cells.Item(pVal.Row).Specific as EditText).Value;
                        string lStrAF = (mtxXML.Columns.Item("C_AF").Cells.Item(pVal.Row).Specific as EditText).Value;
                        mObjModalAF = new frmModalAF(lStrArea, lStrAF, string.Empty, true);
                        mObjModalAF.Show();

                    }

                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[mtxXML_KeyDownAfter]: " + ex.Message);
                LogService.WriteError(ex);
            }

        }


    }
}
