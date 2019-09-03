using SAPbobsCOM;
using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using UGRS.AddOn.FoodProduction.Forms;
using UGRS.AddOn.FoodProduction.Services;
using UGRS.AddOn.FoodProduction.UI;
using UGRS.AddOn.FoodProduction.UI.ChooseFromlist;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.FoodProduction;
using UGRS.Core.SDK.DI.FoodProduction.DAO;
using UGRS.Core.SDK.DI.FoodProduction.DTO;
using UGRS.Core.SDK.DI.FoodProduction.Services;
using UGRS.Core.SDK.DI.FoodProduction.Tables;
using UGRS.Core.Services;
using UGRS.Core.Utility;

namespace UGRS.AddOn.FoodProduction
{
    [FormAttribute("UGRS.PlantaAlimentos.Forms.TicketsListForm", "Forms/TicketsListForm.b1f")]
    class TicketsListForm : UserFormBase
    {
        #region Properties
        QueryManager mObjQueryManager = new QueryManager();
        TicketDAO mObjTicketDAO = new TicketDAO();
        TicketServices mObjTicketServices = new TicketServices();
        FoodProductionSeviceFactory mObjFoodProdutionServiceFactory = new FoodProductionSeviceFactory();
        SAPbouiCOM.ChooseFromListCreationParams mObjCFLCreationParams = null;
        SAPbouiCOM.ChooseFromList mObjCFLSocio = null;
        string mStrODraftDocument = string.Empty;

        List<Ticket> lLstTickets = null;
        List<string> lStrLstFolios = new List<string>();
        List<string> lStrLstInvalidFolios = new List<string>();
        private string mStrTipoDoc;
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.mtxTicketsList = ((SAPbouiCOM.Matrix)(this.GetItem("mtxTksLst").Specific));
            this.txtBP = ((SAPbouiCOM.EditText)(this.GetItem("txtBP").Specific));
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_2").Specific));
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_3").Specific));
            this.cboTicketStatus = ((SAPbouiCOM.ComboBox)(this.GetItem("cboTicSta").Specific));
            this.txtDate1 = ((SAPbouiCOM.EditText)(this.GetItem("txtDate1").Specific));
            this.txtDate2 = ((SAPbouiCOM.EditText)(this.GetItem("txtDate2").Specific));
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_6").Specific));
            this.StaticText3 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_7").Specific));
            this.cboTypeDate = ((SAPbouiCOM.ComboBox)(this.GetItem("cboTypDat").Specific));
            this.cboTicketType = ((SAPbouiCOM.ComboBox)(this.GetItem("cboTicTyp").Specific));
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("Item_9").Specific));
            this.Button1 = ((SAPbouiCOM.Button)(this.GetItem("Item_10").Specific));
            this.btnSearch = ((SAPbouiCOM.Button)(this.GetItem("btnSearch").Specific));
            this.btnEdit = ((SAPbouiCOM.Button)(this.GetItem("btnEdit").Specific));
            //this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("Item_13").Specific));
            this.StaticText4 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_14").Specific));
            this.btnAction = ((SAPbouiCOM.Button)(this.GetItem("btnAct").Specific));
            this.chkSelect = ((SAPbouiCOM.CheckBox)(this.GetItem("chkSelect").Specific));
            this.cboDocType = ((SAPbouiCOM.ComboBox)(this.GetItem("cboDocType").Specific));
            this.lblType = ((SAPbouiCOM.StaticText)(this.GetItem("lblType").Specific));

            this.OnCustomInitialize();

            cboTypeDate.ValidValues.Add("Seleccione", "Seleccione");
            cboTypeDate.ValidValues.Add("Hoy", "Hoy");
            cboTypeDate.ValidValues.Add("Ultima semana", "Ultima semana");
            cboTypeDate.ValidValues.Add("Ultimo mes", "Ultimo mes");
            cboTypeDate.ValidValues.Add("Fechas seleccionadas", "Fechas seleccionadas");
            cboTypeDate.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
            cboTypeDate.Select(0, BoSearchKey.psk_Index);

            cboTicketStatus.ValidValues.Add("Seleccione", "Seleccione");
            cboTicketStatus.ValidValues.Add("Cerrado", "Cerrado");
            cboTicketStatus.ValidValues.Add("Abierto", "Abierto");
            cboTicketStatus.ValidValues.Add("Pendiente", "Pendiente");
            cboTicketStatus.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
            cboTicketStatus.Select(3, BoSearchKey.psk_Index);

            cboTicketType.ValidValues.Add("Seleccione", "Seleccione");
            cboTicketType.ValidValues.Add("Venta", "Venta");
            cboTicketType.ValidValues.Add("Compra", "Compra");
            cboTicketType.ValidValues.Add("Traslado - Entrada", "Traslado - Entrada");
            cboTicketType.ValidValues.Add("Traslado - Salida", "Traslado - Salida");
            cboTicketType.ValidValues.Add("Venta de pesaje", "Venta de pesaje");
            cboTicketType.ValidValues.Add("Pesaje", "Pesaje");
            cboTicketType.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;


            cboDocType.ValidValues.Add("Seleccione", "Seleccione");
            cboDocType.ValidValues.Add("Pedido", "Pedido");
            cboDocType.ValidValues.Add("Factura de reserva", "Factura de reserva");
            cboDocType.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
            cboDocType.Select(0, BoSearchKey.psk_Index);


            //Application
            UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(TicketsListFrom_ApplicationItemEvent);
            UIApplication.GetApplication().MenuEvent += new _IApplicationEvents_MenuEventEventHandler(TicketsListFrom_ApplicationMenuEvent);

            cboTicketType.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cboTicketType_ComboSelectAfter);
            cboDocType.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cboDocType_ComboSelectAfter);
            btnAction.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnAction_ClickBefore);
            btnSearch.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSearch_ClickBefore);
            btnEdit.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnEdit_ClickBefore);
            // chkSelect.ClickBefore += new SAPbouiCOM._ICheckBoxEvents_ClickBeforeEventHandler(this.chkSelect_ClickBefore);
            //mtxTicketsList chkSelect.DoubleClickBefore += new SAPbouiCOM._ICheckBoxEvents_DoubleClickBeforeEventHandler(this.chkSelect_DoubleClickBefore);
            chkSelect.ClickAfter += new SAPbouiCOM._ICheckBoxEvents_ClickAfterEventHandler(this.chkSelect_ClickAfter);
            txtBP.Item.Enabled = false;
            InitDataSources();
            AddChooseFromList();

            UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);

            txtDate1.DataBind.SetBound(true, "", "UDDate");
            txtDate2.DataBind.SetBound(true, "", "UDDate2");

            chkSelect.ValOn = "Y";
            chkSelect.ValOff = "N";
            chkSelect.DataBind.SetBound(true, "", "CheckboxDS");

            // this sets the checkbox to checked
            //oForm.DataSources.UserDataSources.Item("CheckboxDS").Value = "Y"; 
            mtxTicketsList.AutoResizeColumns();
        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }

        private void InitDataSources()
        {
            try
            {
                this.UIAPIRawForm.DataSources.DataTables.Add("RESULT");
                this.UIAPIRawForm.DataSources.UserDataSources.Add("UDSocio", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
                mObjCFLSocio = initChooseFromLists(false, "2", "CFL_Socio");
                this.UIAPIRawForm.DataSources.UserDataSources.Add("UDDate", SAPbouiCOM.BoDataType.dt_DATE, 254);
                this.UIAPIRawForm.DataSources.UserDataSources.Add("UDDate2", SAPbouiCOM.BoDataType.dt_DATE, 254);
                this.UIAPIRawForm.DataSources.UserDataSources.Add("CheckboxDS", BoDataType.dt_SHORT_TEXT, 1);

                // mObjCFLSocio = mObjTicketServices.initChooseFromLists(false, "2", "CFL_Socio", this.UIAPIRawForm.ChooseFromLists);

                //Parametros de choose para list de Bussines Partner
                mObjCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)UIApplication.GetApplication().CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);
                mObjCFLCreationParams.MultiSelection = false;
                mObjCFLCreationParams.ObjectType = "2";
                mObjCFLCreationParams.UniqueID = "CFL_Params";


                SAPbouiCOM.ChooseFromListCollection lObjCFLs = null;
                lObjCFLs = this.UIAPIRawForm.ChooseFromLists;
                mObjCFLSocio = lObjCFLs.Add(mObjCFLCreationParams);

            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
            }
        }

        private void OnCustomInitialize()
        {
            loadMenu();

        }

        #endregion
        
        #region Events
        private void chkSelect_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                SelectAll();
            }
            catch (Exception ex)
            {
                LogService.WriteError("[chkSelect_ClickAfter]: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void chkSelect_DoubleClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                SelectAll();
            }
            catch (Exception ex)
            {
                LogService.WriteError("[chkSelect_DoubleClickBefore]: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void chkSelect_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {


                SelectAll();
            }

            catch (Exception ex)
            {
                LogService.WriteError("[chkSelect_ClickBefore]: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void btnEdit_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {


                List<string> lstrSelectedRows = getSelectedRows();
                Ticket lObjTicket = new Ticket(); ;
                IList<TicketDetail> lObjTicketDetail = null;
                if (lstrSelectedRows.Count > 0)
                {
                    string lStrcode = mObjQueryManager.GetValue("Code", "U_Folio", lstrSelectedRows[0], "[@UG_PL_TCKT]");
                    lObjTicket = mObjTicketDAO.GetTicket(lStrcode);
                    lObjTicketDetail = mObjTicketDAO.GetListTicketDetail(lstrSelectedRows[0]);
                }

                if (lObjTicketDetail != null)
                {
                    bool lBolIsOpen = false;
                    SAPbouiCOM.Forms lObjForms = UIApplication.GetApplication().Forms;

                    foreach (Form lObjForm in lObjForms)
                    {
                        if (lObjForm.TypeEx == "UGRS.PlantaAlimentos.Forms.TicketForm" && lObjForm.Visible)
                        {
                            lBolIsOpen = true;
                        }
                    }
                    if (!lBolIsOpen)
                    {
                        TicketForm lObjTicketForm = new TicketForm(lObjTicket, lObjTicketDetail);
                        lObjTicketForm.UIAPIRawForm.Left = GetLeftMargin(lObjTicketForm.UIAPIRawForm);
                        lObjTicketForm.UIAPIRawForm.Top = GetTopMargin(lObjTicketForm.UIAPIRawForm);
                        lObjTicketForm.Show();
                    }
                    else
                    {
                        UIApplication.ShowMessageBox("Ya se tiene abierta una ventana de tickets");
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[btnEdit_ClickBefore]: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void TicketsListFrom_ApplicationMenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                if (!pVal.BeforeAction)
                {
                    switch (pVal.MenuUID)
                    {
                        case "1281": // Search Record
                            break;

                        case "1282": // Add New Record
                            break;

                        case "1288": // Next Record
                            break;

                        case "1289": // Pevious Record
                            break;

                        case "1290": // First Record
                            break;

                        case "1291": // Last record 
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("MenuEventException: {0}", ex.Message));
                LogService.WriteError("[TicketsListFrom_ApplicationMenuEvent]: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void TicketsListFrom_ApplicationItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
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
                            //case BoEventTypes.et_COMBO_SELECT:
                            //    cboTicketType_ComboSelectAfter();
                            //    break;

                            case BoEventTypes.et_FORM_CLOSE:
                                UIApplication.GetApplication().ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(TicketsListFrom_ApplicationItemEvent);
                                UIApplication.GetApplication().MenuEvent -= new _IApplicationEvents_MenuEventEventHandler(TicketsListFrom_ApplicationMenuEvent);
                                break;

                            case BoEventTypes.et_COMBO_SELECT:
                                if (pVal.ItemUID == "cboTicTyp")
                                {
                                    ChangeCaptureMode(pVal);
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[TicketsListFrom_ApplicationItemEvent]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format("ItemEventException: {0}", ex.Message));
            }
        }

        private void btnSearch_ClickBefore(object pObjSBOObject, SBOItemEventArg pObjEventArg, out bool pObjBubbleEvent)
        {
            pObjBubbleEvent = true;
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                LogService.WriteError("[btnSearch_ClickBefore]: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        ///<summary>    Choose from list after event. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        ///<param name="pValEvent"> The value event. </param>
        private void chooseFromListAfterEvent(SAPbouiCOM.ItemEvent pValEvent)
        {
            try
            {


                if (pValEvent.Action_Success)
                {
                    SAPbouiCOM.IChooseFromListEvent oCFLEvento = null;
                    oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pValEvent;

                    string sCFL_ID = null;
                    sCFL_ID = oCFLEvento.ChooseFromListUID;

                    SAPbouiCOM.ChooseFromList oCFL = null;
                    oCFL = this.UIAPIRawForm.ChooseFromLists.Item(sCFL_ID);

                    SAPbouiCOM.DataTable oDataTable = null;
                    oDataTable = oCFLEvento.SelectedObjects;
                    try
                    {
                        if (oDataTable != null && pValEvent.ItemUID == txtBP.Item.UniqueID)
                        {
                            this.UIAPIRawForm.DataSources.UserDataSources.Item("UDSocio").ValueEx = System.Convert.ToString(oDataTable.GetValue(0, 0));
                            // txtNomSoc.Value = System.Convert.ToString(oDataTable.GetValue(1, 0));
                            // txtClSoc.Value = System.Convert.ToString(oDataTable.GetValue(0, 0));
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.WriteError("[chooseFromListAfterEvent]: " + ex.Message);
                        LogService.WriteError(ex);
                        UIApplication.ShowError(string.Format("ChooseFromListException: {0}", ex.Message));
                    }
                }
            }
             
            catch (Exception ex)
            {
                LogService.WriteError("[chooseFromListAfterEvent]: " + ex.Message);
                LogService.WriteError(ex);
            }
            //mtxArtLst.SetCellFocus(0, 0);
        }


        ///<summary>    Sbo application item event. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        ///<param name="FormUID">       The form UID. </param>
        ///<param name="pVal">          [in,out] The value. </param>
        ///<param name="BubbleEvent">   [out] True to bubble event. </param>

        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
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
                                chooseFromListAfterEvent(pVal);
                                break;

                            case BoEventTypes.et_COMBO_SELECT:
                                ChangeCombobox(pVal);
                                break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[chooseFromListAfterEvent]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format("ItemEventException: {0}", ex.Message));
            }
        }


        private void btnAction_ClickBefore(object pObjSBOObject, SAPbouiCOM.SBOItemEventArg pObjEventArg, out bool pObjBubbleEvent)
        {
            mStrODraftDocument = string.Empty;
            pObjBubbleEvent = true;
            if (DIApplication.Company.InTransaction)
            {
                DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
            }
            DIApplication.Company.StartTransaction();
            bool lBolSuccess = false;
            try
            {
                //Form lObjForm = null;
                string lStrValue = cboTicketType.Value;
                btnAction.Item.Enabled = true;

                List<string> lstrSelectedRows = getSelectedRows();
                List<Ticket> lLstTickets = new List<Ticket>();
                lStrLstFolios = new List<string>();
                lStrLstInvalidFolios = new List<string>();
                Ticket lObjTicket = new Ticket();
                IList<TicketDetail> lObjTicketDetail = new List<TicketDetail>();

               

                if (lstrSelectedRows.Count > 0)
                {
                    bool lBolAmount = true;
                 
                    int i = 0;
                    foreach (string lStrTicketCode in lstrSelectedRows)
                    {
                        foreach (TicketDetail lObjTicketD in mObjTicketDAO.GetListTicketDetail(lStrTicketCode))
                        {
                            if (lObjTicketD.Amount == 0 && lObjTicketD.netWeight == 0)
                            {
                                lBolAmount = false;
                                break;
                            }
                            lObjTicketDetail.Add(lObjTicketD);
                        }
                        if (lBolAmount)
                        {
                            string lStrcode = mObjQueryManager.GetValue("Code", "U_Folio", lstrSelectedRows[i], "[@UG_PL_TCKT]");
                            lObjTicket = mObjTicketDAO.GetTicket(lStrcode);
                            lLstTickets.Add(lObjTicket);
                        }
                        i++;
                    }
                    if (lObjTicket.CapType == 4)
                    {
                        lBolAmount = true;
                    }
                    if (lBolAmount && (lObjTicket.Status == 1 || lObjTicket.Status == 2))
                    {
                        lBolSuccess = false;
                        switch (lStrValue)
                        {
                            case "Venta":
                                if (lObjTicket.DocType == 0)
                                {
                                     lBolSuccess = CrearDocumento(lLstTickets, BoObjectTypes.oDrafts, "ORDR", 17, "RDR1");
                                }
                                else
                                {
                                    //Regla de denocio no realizar el ajuste
                                    //List<TicketDetail> lLstTicketOut = AdjustmentTicket(lLstTickets, true, true); 
                                    //if (lLstTicketOut.Count > 0)
                                    //{
                                    //    CreateInventoryDocument(BoObjectTypes.oInventoryGenExit, lLstTicketOut);
                                    //}
                                    lBolSuccess = CrearDocumento(lLstTickets, BoObjectTypes.oDrafts, "OINV", 13, "INV1");// CrearEntrega(lObjTicket, lObjTicketDetail); //CrearEntrega(lObjTicket, lObjTicketDetail); //
                                }
                                break;
                            case "Compra":

                                lLstTickets = CheckAmounts(lLstTickets);

                                if (lLstTickets.Count > 0)
                                {
                                    if (lObjTicket.DocType == 0)
                                    {
                                        lBolSuccess = CrearDocumento(lLstTickets, BoObjectTypes.oDrafts, "OPOR", 22, "POR1"); // CrearFacturaCompra(lObjTicket, lObjTicketDetail); 
                                    }
                                    else
                                    {
                                        List<TicketDetail> lLstTicketEntry = AdjustmentTicket(lLstTickets, false, true);
                                        if (lLstTicketEntry.Count > 0)
                                        {
                                          lBolSuccess =  CreateInventoryDocument(BoObjectTypes.oInventoryGenEntry, lLstTicketEntry);
                                        }
                                      
                                            lBolSuccess = CrearDocumento(lLstTickets, BoObjectTypes.oDrafts, "OPCH", 18, "PCH1"); // CrearFacturaCompra(lObjTicket, lObjTicketDetail); 
                                        

                                    }
                                }
                                PrintWarnings();

                                break;
                            case "Traslado - Entrada":
                                lBolSuccess = Creartransferencia(lLstTickets);
                                break;
                            case "Traslado - Salida":
                                lBolSuccess = Creartransferencia(lLstTickets);
                                break;

                            case "Venta de pesaje":
                                //lBolCorrect = CrearDocumento(lLstTickets, BoObjectTypes.oDrafts, "ORDR", 17, "RDR1");
                                lBolSuccess = CrearFacturaVenta(lLstTickets);
                                break;
                        }
                        if (lBolSuccess)
                        {
                            //if (!cboTicketType.Value.Equals("Venta de pesaje") || (cboTicketType.Value.Equals("Venta de pesaje") && cboTicketStatus.Value.Equals("Pendiente")))
                            //{
                                lBolSuccess = CloseTicket(lLstTickets);
                           // }
                        }
                    }
                    else
                    {
                        if (lObjTicket.Status == 0)
                        {
                            UIApplication.ShowMessageBox(string.Format("No es posible generar docuemento de un ticket ya cerrado"));
                        }
                        else
                        {
                            UIApplication.ShowMessageBox(string.Format("La cantidad de un articulo no debe de estar en 0"));
                        }
                    }

                    //if (lObjForm != null)
                    //{
                    //    lObjForm.Select();
                    //}
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[btnAction_ClickBefore]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessage(ex.Message);
                lBolSuccess = false;
            }
            finally
            {
                try
                {
                    if (lBolSuccess)
                    {
                        DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        UIApplication.ShowMessageBox(string.Format("Proceso realizado correctamente"));
                        if (!string.IsNullOrEmpty(mStrODraftDocument))
                        {
                            SAPbouiCOM.Form lObjFormDraft = SAPbouiCOM.Framework.Application.SBO_Application.OpenForm((SAPbouiCOM.BoFormObjectEnum)112, "", mStrODraftDocument);
                        }
                        Search();
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
                    LogService.WriteError("(btnSave_ClickBefore): " + ex.Message);
                    LogService.WriteError(ex);
                }
            }
        }


        #endregion

        #region Methods
        public TicketsListForm()
        {
        }

        private void SelectAll()
        {
            this.UIAPIRawForm.Freeze(true);
            for (int i = 1; i <= mtxTicketsList.RowCount; i++)
            {
                if (chkSelect.Checked)
                {
                    ((SAPbouiCOM.CheckBox)mtxTicketsList.Columns.Item("chkSelect").Cells.Item(i).Specific).Checked = false;
                    // (CheckBox)mtxTicketsList.Columns.Item(0).Cells.Item(i).Specific).Checked)
                }
                else
                {
                    ((SAPbouiCOM.CheckBox)mtxTicketsList.Columns.Item("chkSelect").Cells.Item(i).Specific).Checked = true;
                }
            }

            mtxTicketsList.Item.Refresh();
            this.UIAPIRawForm.Freeze(false);
        }

        private int GetLeftMargin(SAPbouiCOM.IForm pObjForm)
        {
            double lDblWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            return (int)((lDblWidth / 2) - (pObjForm.Width / 2) + 100);
        }

        private int GetTopMargin(SAPbouiCOM.IForm pObjForm)
        {
            double lDblHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            return (int)((lDblHeight / 2) - (pObjForm.Height / 2) - 100);
        }
        /// <summary>
        /// Obtiene el tipo de captura
        /// </summary>
        private int GetCapType(string pStrType)
        {
            int lIntType = 0;
            switch (pStrType)
            {
                case "Venta":
                    lIntType = 0;
                    break;

                case "Compra":
                    lIntType = 1;
                    break;

                case "Traslado - Entrada":
                    lIntType = 2;
                    break;

                case "Traslado - Salida":
                    lIntType = 3;
                    break;

                case "Venta de pesaje":
                    lIntType = 4;
                    break;
                case "Pesaje":
                    lIntType = 5;
                    break;
            }
            return lIntType;
        }

        private string GetDocType(string pStrType)
        {
            string lStrType = "";
            switch (pStrType)
            {
                case "Pedido":
                    lStrType = "0";
                    break;

                case "Factura de reserva":
                    lStrType = "1";
                    break;
            }
            return lStrType;
        }

        ///<summary>    Adds choose from list. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        private void AddChooseFromList()
        {
            try
            {
                txtBP.DataBind.SetBound(true, "", "UDSocio");
                txtBP.ChooseFromListUID = "CFL_Socio";
                //txtNomSoc.ChooseFromListUID = "CFL_SocioNombre";
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("CustomerChooseListException: {0}", ex.Message));
            }
        }

        private void ChangeCaptureMode(SAPbouiCOM.ItemEvent pValEvent)
        {

            switch (pValEvent.PopUpIndicator)
            {
                case 0:
                    txtBP.Item.Enabled = true;
                    mStrTipoDoc = "CFL_Venta";
                    lblType.Item.Visible = true;
                   
                    cboDocType.Item.Visible = true;
                    cboDocType.Select(1, BoSearchKey.psk_Index);
                    break;

                case 1:
                    txtBP.Item.Enabled = true;
                    mStrTipoDoc = "CFL_Venta";
                    lblType.Item.Visible = true;
                    btnSearch.Item.Visible = true;
                    cboDocType.Item.Visible = true;
                    cboDocType.Select(1, BoSearchKey.psk_Index);
                    break;

                case 2:
                    txtBP.Item.Enabled = true;
                    mStrTipoDoc = "CFL_Compra";
                    lblType.Item.Visible = true;
                    btnSearch.Item.Visible = true;
                    cboDocType.Item.Visible = true;
                    cboDocType.Select(1, BoSearchKey.psk_Index);
                    break;

                case 3:
                    txtBP.Item.Enabled = false;
                    lblType.Item.Visible = false;
                    btnSearch.Item.Visible = true;
                    cboDocType.Item.Visible = false;
                    cboDocType.Select(0, BoSearchKey.psk_Index);
                    break;

                case 4:
                    txtBP.Item.Enabled = false;
                    lblType.Item.Visible = false;
                    btnSearch.Item.Visible = true;
                    cboDocType.Item.Visible = false;
                    cboDocType.Select(0, BoSearchKey.psk_Index);
                    break;

                case 5:
                    txtBP.Item.Enabled = true;
                    btnSearch.Item.Visible = true;
                    lblType.Item.Visible = true;
                    cboDocType.Item.Visible = true;
                    cboDocType.Select(1, BoSearchKey.psk_Index);

                    mStrTipoDoc = "CFL_Venta";
                    break;

                case 6:
                    txtBP.Item.Enabled = false;
                    btnSearch.Item.Visible = true;
                    lblType.Item.Visible = false;
                     cboDocType.Item.Visible = false;
                    cboDocType.Select(0, BoSearchKey.psk_Index);
                    break;

            }
            ConditionsUI lObjCondition = new ConditionsUI();
            lObjCondition.initChooseFromListBussinesPartner(mStrTipoDoc, mObjCFLSocio);
            txtBP.ChooseFromListUID = "CFL_Params";
        }


        DateTime mDate1;
        DateTime mDate2;
        private void ChangeCombobox(ItemEvent pValEvent)
        {
            if (pValEvent.ItemUID == "cboTypDat")
            {
                switch (pValEvent.PopUpIndicator)
                {
                    case 0:
                        //UPDATE Rcordova 27/10/2017
                        txtDate1.Value = "";
                        txtDate2.Value = "";
                        //UPDATE Rcordova 27/10/2017
                        break;
                    case 1: //HOY
                        mDate1 = DateTime.Now;
                        mDate2 = DateTime.Now;
                        //UPDATE Rcordova 27/10/2017
                        txtDate1.Value = "";
                        txtDate2.Value = "";
                        //UPDATE Rcordova 27/10/2017
                        break;

                    case 2: //Semana
                        mDate1 = DateTime.Now.AddDays(-7);
                        mDate2 = DateTime.Now;
                        //UPDATE Rcordova 27/10/2017
                        txtDate1.Value = "";
                        txtDate2.Value = "";
                        //UPDATE Rcordova 27/10/2017
                        break;

                    case 3: //ultimo mes
                        mDate1 = DateTime.Now.AddDays(-30);
                        mDate2 = DateTime.Now;
                        //UPDATE Rcordova 27/10/2017
                        txtDate1.Value = "";
                        txtDate2.Value = "";
                        //UPDATE Rcordova 27/10/2017
                        break;
                    case 4: //Seleccionar
                        txtDate1.Item.Enabled = true;
                        txtDate2.Item.Enabled = true;
                        break;

                    // mtxArtLst.Columns.
                }

            }
        }

        //private SAPbouiCOM.Button Button0;


        ///<summary>    Initializes the choose from lists. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        ///<param name="pbol">      True to pbol. </param>
        ///<param name="pStrType">  Type of the string. </param>
        ///<param name="pStrID">    Identifier for the string. </param>
        ///<returns>    A ChooseFromList. </returns>

        private SAPbouiCOM.ChooseFromList initChooseFromLists(bool pbol, string pStrType, string pStrID)
        {
            SAPbouiCOM.ChooseFromList oCFL = null;
            try
            {
                SAPbouiCOM.ChooseFromListCollection oCFLs = null;
                SAPbouiCOM.Conditions oCons = null;
                SAPbouiCOM.Condition oCon = null;

                oCFLs = this.UIAPIRawForm.ChooseFromLists;


                SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
                oCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)UIApplication.GetApplication().CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);

                //  Adding 2 CFL, one for the button and one for the edit text.
                oCFLCreationParams.MultiSelection = pbol;
                oCFLCreationParams.ObjectType = pStrType;
                oCFLCreationParams.UniqueID = pStrID;

                oCFL = oCFLs.Add(oCFLCreationParams);

                //  Adding Conditions to CFL1
                oCons = oCFL.GetConditions();

                oCon = oCons.Add();
                oCon.Alias = "DocStatus";
                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oCon.CondVal = "O";
                //oCFL.SetConditions(oCons);//condiciones

            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("InitCustomerChooseFromListException: {0}", ex.Message));
            }
            return oCFL;
        }

        private void Search()
        {
            if (cboTicketStatus.Value == "Abierto" && cboDocType.Value != "Pedido")
            {
                btnAction.Item.Enabled = false;
            }
            else
            {
                btnAction.Item.Enabled = true;
            }

            if (cboTicketType.Value != "Seleccione")
            {
                Recordset lObjRecordSet = null;
                string lStrDate1 = "";
                string lStrDate2 = "";
                if (cboTypeDate.Value == "Fechas seleccionadas" && !string.IsNullOrEmpty(txtDate1.Value) && !string.IsNullOrEmpty(txtDate2.Value))
                {

                    lStrDate1 = txtDate1.Value;
                    lStrDate2 = txtDate2.Value;
                }
                else
                {
                    lStrDate1 = mDate1.ToString("yyyyMMdd");
                    lStrDate2 = mDate2.ToString("yyyyMMdd");
                }

                if (string.IsNullOrEmpty(lStrDate1) || string.IsNullOrEmpty(lStrDate2) || cboTypeDate.Value == "Seleccione")
                {
                    lStrDate1 = DateTime.Now.ToString("yyyyMMdd");
                    lStrDate2 = DateTime.Now.ToString("yyyyMMdd");
                }
                this.UIAPIRawForm.Freeze(true);
                try
                {
                    //lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                    int lIntStatus = 0;
                    if (cboTicketStatus.Value == "Cerrada")
                    {
                        lIntStatus = 2;
                    }
                    switch (cboTicketStatus.Value)
                    {
                        case "Cerrado":
                            lIntStatus = 0;
                            break;
                        case "Abierto":
                            lIntStatus = 1;
                            break;
                        case "Pendiente":
                            lIntStatus = 2;
                            break;
                    }


                    string lStrQuery = mObjTicketDAO.GetFilteredTickets(txtBP.Value, lStrDate1, lStrDate2, GetCapType(cboTicketType.Value), lIntStatus.ToString(), GetDocType(cboDocType.Value));
                    //ItemDAO mObjITemDAO = new ItemDAO();
                    //var ss = mObjITemDAO.GetItemByCode("c");
                    this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                    //("select * from [@UG_PL_TCKT]");

                    // ExecuteQuery(mObjRejectedDAO.GetRejectedQuery(lStrWhsCode));
                    mtxTicketsList.Columns.Item("chkSelect").DataBind.Bind("RESULT", "CardType");
                    mtxTicketsList.Columns.Item("ColTicket").DataBind.Bind("RESULT", "U_Folio");
                    // mtxTicketsList.Columns.Item("ColTypeT").DataBind.Bind("RESULT", "U_Folio");
                    mtxTicketsList.Columns.Item("ColDateE").DataBind.Bind("RESULT", "U_EntryDate");
                    mtxTicketsList.Columns.Item("ColDateO").DataBind.Bind("RESULT", "U_OutputDate");
                    mtxTicketsList.Columns.Item("ColDoc").DataBind.Bind("RESULT", "U_Number");
                    mtxTicketsList.Columns.Item("ColBP").DataBind.Bind("RESULT", "U_BPCode");
                    mtxTicketsList.Columns.Item("ColBPName").DataBind.Bind("RESULT", "CardName");
                    mtxTicketsList.Columns.Item("ColDriver").DataBind.Bind("RESULT", "U_Driver");
                    mtxTicketsList.Columns.Item("ColCardTag").DataBind.Bind("RESULT", "U_CarTag");
                    //mtxTicketsList.Columns.Item("ColItemC").DataBind.Bind("RESULT", "U_Folio");
                    //mtxTicketsList.Columns.Item("ColItem").DataBind.Bind("RESULT", "U_Item");
                    mtxTicketsList.Columns.Item("ColInputWT").DataBind.Bind("RESULT", "U_InputWT");
                    mtxTicketsList.Columns.Item("ColOutWT").DataBind.Bind("RESULT", "U_OutputWT");
                    mtxTicketsList.Columns.Item("ColNetWT").DataBind.Bind("RESULT", "U_netWeight");
                    mtxTicketsList.Columns.Item("ColAmount").DataBind.Bind("RESULT", "U_Amount");
                    mtxTicketsList.LoadFromDataSource();
                    mtxTicketsList.AutoResizeColumns();
                }
                catch (Exception ex)
                {
                    UIApplication.ShowError(string.Format("ItemEventException: {0}", ex.Message));
                }
                finally
                {
                    MemoryUtility.ReleaseComObject(lObjRecordSet);
                    this.UIAPIRawForm.Freeze(false);
                }
            }
        }


        private void PrintWarnings()
        {
            string lStrWarning = string.Empty;
            if (lStrLstInvalidFolios.Count > 0)
            {
                foreach (var item in lStrLstInvalidFolios)
                {
                    lStrWarning += item + " ";
                }

                SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText("La cantidad no debe ser 0 en: " + lStrWarning
              , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

            }

        }

        private List<Ticket> CheckAmounts(List<Ticket> lLstTickets)
        {
            for (int i = 0; i < lLstTickets.Count; i++)
            {
                if (lLstTickets[i].Amount > 0)
                {
                    lStrLstFolios.Add(lLstTickets[i].Folio);
                }
                else
                {
                    lStrLstInvalidFolios.Add(lLstTickets[i].Folio);
                }
            }

            return CheckList(lLstTickets);
        }

        private List<Ticket> CheckList(List<Ticket> lLstTickets)
        {
            List<Ticket> TempList = new List<Ticket>();
            try
            {
                foreach (var item in lStrLstFolios)
                {
                    TempList.AddRange(lLstTickets.FindAll(x => x.Folio == item));
                }

                lLstTickets = TempList;

            }
            catch (Exception ex)
            {
                string error = ex.ToString();
            }

            return lLstTickets;
        }

        //private void CheckList(List<Ticket> lLstTickets)
        //{
        //    List<Ticket> TempList = new List<Ticket>();
        //    try
        //    {
        //        foreach (var item in lStrLstFolios)
        //        {
        //            TempList.AddRange(lLstTickets.FindAll(x=>x.Folio == item));
        //        }

        //        lLstTickets = TempList;

        //    }
        //    catch (Exception ex)
        //    {
        //        string error = ex.ToString();
        //    }
        //}


        private bool CloseTicket(List<Ticket> pLstTicket)
        {
            bool lBolResult = false;
            try
            {
                foreach (Ticket lObjTicket in pLstTicket)
                {
                    FoodProductionSeviceFactory lObjFoodProductionFactory = new FoodProductionSeviceFactory();
                    //Ticket lObjTicket = gett
                    lObjTicket.RowCode = lObjFoodProductionFactory.GetTicketService().GetTicketCode("", lObjTicket.Folio);
                    lObjTicket.Status = 0;
                    lObjTicket.OutputDate = DateTime.Now;
                    TicketService lObjTicketService = new TicketService();
                    if (lObjTicketService.Update(lObjTicket) == 0)
                    {
                       
                        lBolResult = true;
                    }
                    else
                    {
                        lBolResult = false;
                    }
                }
            }
            catch (Exception lObjException)
            {
                lBolResult = false;
                UIApplication.ShowError(string.Format("ItemEventException: {0}", lObjException.Message));
            }
            return lBolResult;
        }


        private List<TicketDetail> AdjustmentTicket(List<Ticket> pLstTicket, bool pBolIsVenta, bool pBolIsInventory)
        {
            List<TicketDetail> lLstTicketDetail = new List<TicketDetail>();
            List<TicketDetail> lLstTicketNewLines = new List<TicketDetail>();
            foreach (Ticket lObjTicket in pLstTicket)
            {
                lLstTicketDetail = mObjTicketDAO.GetListTicketDetail(lObjTicket.Folio) as List<TicketDetail>;

                foreach (TicketDetail lObjTicketDetail in lLstTicketDetail)
                {
                    string lStrOpenQty = "";
                    double lFloNetWeight = 0;
                    if (pBolIsVenta)
                    {
                        lStrOpenQty = mObjTicketServices.GetOpenLine("OINV", "INV1", "OpenCreQty", lObjTicket.Number.ToString(), lObjTicketDetail.BaseLine.ToString());
                    }
                    else
                    {
                        lStrOpenQty = mObjTicketServices.GetOpenLine("OPCH", "PCH1", "OpenCreQty", lObjTicket.Number.ToString(), lObjTicketDetail.BaseLine.ToString());
                    }

                    if (pBolIsInventory && lStrOpenQty != "" && lObjTicketDetail.netWeight > Convert.ToDouble(lStrOpenQty, CultureInfo.InvariantCulture))
                    {
                        lFloNetWeight = lObjTicketDetail.netWeight - Convert.ToDouble(lStrOpenQty, CultureInfo.InvariantCulture);
                    }

                    if (pBolIsInventory)
                    {
                        lObjTicketDetail.netWeight = lFloNetWeight;
                    }
                    else
                    {
                        if (lObjTicketDetail.netWeight > Convert.ToDouble(lStrOpenQty, CultureInfo.InvariantCulture))
                        {
                            lObjTicketDetail.netWeight = Convert.ToDouble(lStrOpenQty, CultureInfo.InvariantCulture);
                        }
                    }
                    if (lObjTicketDetail.netWeight > 0)
                    {
                        lLstTicketNewLines.Add(lObjTicketDetail);
                    }
                }
            }
            return lLstTicketNewLines;
        }

        private string GetCostCenter()
        {
            string lStrCostCenter = "";
            try
            {
                lStrCostCenter = mObjQueryManager.GetValue("U_GLO_CostCenter", "UserID", DIApplication.Company.UserSignature.ToString(), "OUSR");
            }
            catch (Exception lObjException)
            {
                UIApplication.ShowError(string.Format("CostCenter: {0}", lObjException.Message));
            }
            return lStrCostCenter;
        }
        //U_GLO_CostCenter


        #region Documents
        private bool CrearDocumento(List<Ticket> pLstTicket, SAPbobsCOM.BoObjectTypes pObjType, string pStrTableBase, int pIntBaseType, string pStrTableDetail)
        {
            bool lBolIsSuccess = true;
            try
            {
                string lStrDocEntry = string.Empty;
                string lStrCostCenter = GetCostCenter();
                List<TicketDetail> lLstTicketDetail = new List<TicketDetail>();
                SAPbobsCOM.Documents lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(pObjType); //SAPbobsCOM.BoObjectTypes.oInvoices);
                foreach (Ticket lObjTicket in pLstTicket)
                {
                    if (pStrTableBase == "ORDR")
                    {
                        lObjDocument.DocObjectCodeEx = "13";
                    }

                    // lStrDocEntry = mObjQueryManager.GetValue("DocEntry", "DocNum", lObjTicket.Number.ToString(), pStrTableBase); Cambio DocNumPorDocEntry
                    lStrDocEntry = lObjTicket.Number.ToString();
                    lObjDocument.CardCode = lObjTicket.BPCode;

                    if (lObjTicket.CapType == 0)
                    {
                        lObjDocument.DocObjectCode = BoObjectTypes.oInvoices;
                    }
                    lLstTicketDetail = mObjTicketDAO.GetListTicketDetail(lObjTicket.Folio) as List<TicketDetail>;
                    if (pStrTableBase == "OINV")
                    {
                        lLstTicketDetail = AdjustmentTicket(pLstTicket, true, false);
                        lObjDocument.DocObjectCodeEx = "15";// pIntBaseType.ToString();
                    }
                    if (pStrTableBase == "OPCH")
                    {
                        lLstTicketDetail = AdjustmentTicket(pLstTicket, false, false);
                        lObjDocument.DocObjectCodeEx = "20";
                    }
                    if (pStrTableBase == "OPOR")
                    {
                        lObjDocument.DocObjectCodeEx = "20";
                    }

                    for (int i = 0; i < lLstTicketDetail.Count; i++)
                    {
                        if (lObjTicket.Number != 0 && VerifyDocItem(lStrDocEntry, lLstTicketDetail[i].Item, pStrTableDetail))
                        {
                            lObjDocument.Lines.BaseEntry = int.Parse(lStrDocEntry);
                            lObjDocument.Lines.BaseLine = lLstTicketDetail[i].BaseLine;
                            lObjDocument.Lines.BaseType = pIntBaseType;
                           
                        }
                        // lObjDocument.Lines.AccountCode = "2180010000000";
                        if (lLstTicketDetail[i].netWeight < 0)
                        {
                            lLstTicketDetail[i].netWeight *= -1;
                        }

                        lObjDocument.Lines.ItemCode = lLstTicketDetail[i].Item;

                        //lObjDocument.Lines.UnitsOfMeasurment = 0;
                        lObjDocument.Lines.UnitPrice = lLstTicketDetail[i].Price;
                        lObjDocument.Lines.COGSCostingCode = lStrCostCenter;

                        if (lObjTicket.CapType == 4)
                        {
                            lObjDocument.Lines.Quantity = 1;
                        }
                        else
                        {
                            lObjDocument.Lines.Quantity = lLstTicketDetail[i].netWeight;
                        }
                        lObjDocument.Lines.WarehouseCode = lLstTicketDetail[i].WhsCode;
                        // lObjDocument.Lines.ProjectCode = lObjTicket.Project;

                        lObjDocument.Lines.UserFields.Fields.Item("U_PL_Ticket").Value = lLstTicketDetail[i].Folio;
                        lObjDocument.Lines.UserFields.Fields.Item("U_GLO_BagsBales").Value = lLstTicketDetail[i].BagsBales;
                        lObjDocument.Lines.Add();
                    }
                    if (lObjDocument.Add() != 0)
                    {
                        lBolIsSuccess = false;
                        UIApplication.ShowMessageBox(string.Format("Exception: {0}", DIApplication.Company.GetLastErrorDescription()));
                        LogService.WriteError("[ERROR]" + DIApplication.Company.GetLastErrorDescription());

                    }

                }
                if (lBolIsSuccess)
                {
                    LogService.WriteSuccess("[CrearDocumento] DocEntry" + DIApplication.Company.GetNewObjectKey());// lObjDocument.DocEntry);
                    MemoryUtility.ReleaseComObject(lObjDocument);

                    if (pObjType == BoObjectTypes.oDrafts)
                    {
                        mStrODraftDocument = DIApplication.Company.GetNewObjectKey();
                        //SAPbouiCOM.Form lObjFormDraft = SAPbouiCOM.Framework.Application.SBO_Application.OpenForm((SAPbouiCOM.BoFormObjectEnum)112, "", lStrNewDraft);
                    }
                }
            }
            catch (Exception ex)
            {
                lBolIsSuccess = false;
                LogService.WriteError("[CrearDocumento]" + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(string.Format("Exception: {0}", ex.Message));
            }
            return lBolIsSuccess;
        }

        private bool CreateInventoryDocument(SAPbobsCOM.BoObjectTypes pObjType, List<TicketDetail> pLstTicketDetail)
        {
            bool lBolIsSuccess = false;
            try
            {
                string lStrInvTypeMov = mObjTicketDAO.GetInvMovType();
                string lStrDocEntry = string.Empty;
                string lStrCostCenter = GetCostCenter();
                SAPbobsCOM.Documents lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(pObjType); //SAPbobsCOM.BoObjectTypes.oInvoices);
                for (int i = 0; i < pLstTicketDetail.Count; i++)
                {
                    lObjDocument.UserFields.Fields.Item("U_GLO_InMo").Value = lStrInvTypeMov;
                    lObjDocument.Lines.AccountCode = mObjQueryManager.GetValue("U_Value", "Name", "PL_ACC_ENTRY_ADJUSTMENT", "[@UG_CONFIG]");
                    if (pLstTicketDetail[i].netWeight < 0)
                    {

                        pLstTicketDetail[i].netWeight *= -1;
                    }
                    lObjDocument.Lines.Quantity = pLstTicketDetail[i].netWeight;
                    lObjDocument.Lines.ItemCode = pLstTicketDetail[i].Item;
                    lObjDocument.Lines.UnitPrice = pLstTicketDetail[i].Price;
                    lObjDocument.Lines.COGSCostingCode = lStrCostCenter;
                    lObjDocument.Lines.WarehouseCode = pLstTicketDetail[i].WhsCode;

                    lObjDocument.Lines.UserFields.Fields.Item("U_GLO_BagsBales").Value = pLstTicketDetail[i].BagsBales;
                    lObjDocument.Lines.UserFields.Fields.Item("U_PL_Ticket").Value = pLstTicketDetail[i].Folio;
                    lObjDocument.Lines.Add();
                }
                if (lObjDocument.Add() != 0)
                {
                    UIApplication.ShowMessageBox(string.Format("Exception: {0}", DIApplication.Company.GetLastErrorDescription()));
                    LogService.WriteError("[CreateInventoryDocument]" + DIApplication.Company.GetLastErrorDescription());
                }
                else
                {
                    lBolIsSuccess = true;
                    LogService.WriteSuccess("[CreateInventoryDocument] DocEntry:" + lObjDocument.DocEntry);
                    MemoryUtility.ReleaseComObject(lObjDocument);

                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("Exception: {0}", ex.Message));
                LogService.WriteError("[CreateInventoryDocument]" + ex.Message);
            }
            return lBolIsSuccess;

        }

        #region M
        private bool CrearFacturaCompra(Ticket pObjTicket, IList<TicketDetail> pLstTicketDetail)
        {
            bool lBolIsSuccess = false;
            string lStrCostCenter = GetCostCenter();
            try
            {

                for (int i = 0; i < pLstTicketDetail.Count; i++)
                {
                    //get numero de ticket
                    QueryManager lObjQueryManager = new QueryManager();
                    string p = lObjQueryManager.GetValue("U_Number", "U_Folio", pLstTicketDetail[0].Folio, "[@UG_PL_TCKT]");
                    pObjTicket.Number = int.Parse(p);
                    string lStrDocEntry = pObjTicket.Number.ToString(); //Cambio de DocEntry por DocNum

                    /*//Obtener numero de documento
                    SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                    string lStrDocEntry = string.Empty;
                    lObjRecordSet.DoQuery("select DocEntry from OPOR where DocNum = " + pObjTicket.Number);
                    if (lObjRecordSet.RecordCount == 1)
                    {
                        lStrDocEntry = lObjRecordSet.Fields.Item(0).Value.ToString();
                        pObjTicket.Number = int.Parse(lStrDocEntry);
                    }
                    MemoryUtility.ReleaseComObject(lObjRecordSet);*/

                    SAPbobsCOM.Documents lObjGoodReceipt = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes);
                    lObjGoodReceipt.CardCode = pObjTicket.BPCode;

                    if (pObjTicket.Number != 0 && VerifyDocItem(lStrDocEntry, pLstTicketDetail[i].Item, "RDR1"))
                    {
                        lObjGoodReceipt.Lines.BaseEntry = pObjTicket.Number;
                        lObjGoodReceipt.Lines.BaseLine = i;
                        lObjGoodReceipt.Lines.BaseType = 22;
                    }
                    if (pLstTicketDetail[i].netWeight < 0)
                    {
                        pLstTicketDetail[i].netWeight *= -1;
                    }
                    lObjGoodReceipt.Lines.ItemCode = pLstTicketDetail[i].Item;
                    lObjGoodReceipt.Lines.UnitPrice = pLstTicketDetail[i].Price;
                    lObjGoodReceipt.Lines.Quantity = pLstTicketDetail[i].netWeight;
                    lObjGoodReceipt.Lines.WarehouseCode = "PLHE";
                    lObjGoodReceipt.Lines.UserFields.Fields.Item("U_GLO_BagsBales").Value = pLstTicketDetail[i].BagsBales;
                    lObjGoodReceipt.Lines.UserFields.Fields.Item("U_PL_Ticket").Value = pLstTicketDetail[i].Folio;
                    lObjGoodReceipt.Lines.COGSCostingCode = lStrCostCenter;
                    // lObjGoodReceipt.Lines.ProjectCode = lObjTicket.Project;
                    lObjGoodReceipt.Lines.Add();

                    if (lObjGoodReceipt.Add() != 0)
                    {
                        UIApplication.ShowMessageBox(string.Format("Exception: {0}", DIApplication.Company.GetLastErrorDescription()));
                        LogService.WriteError(DIApplication.Company.GetLastErrorDescription());

                    }
                    else
                    {
                        lBolIsSuccess = true;
                        MemoryUtility.ReleaseComObject(lObjGoodReceipt);
                        UIApplication.ShowMessageBox(string.Format("Entrada de mercancía realizada correctamente"));
                    }
                }

            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("Exception: {0}", ex.Message));
                LogService.WriteError(DIApplication.Company.GetLastErrorDescription());
            }
            return lBolIsSuccess;
        }

        private bool CrearFacturaVenta(List<Ticket> pLstTicket)
        {
            bool lBolIsSuccess = false;
            bool lBoolHasDraft = false;

            string lStrDraftFolio = string.Empty;

            string lStrCostCenter = GetCostCenter();
            try
            {

                SAPbobsCOM.Documents lObjInvoice = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                lObjInvoice.DocObjectCode = BoObjectTypes.oInvoices;

                foreach (Ticket lObjTicket in pLstTicket)
                {
                    lBoolHasDraft = false;
                    List<TicketDetail> pLstTicketDetail = mObjTicketDAO.GetListTicketDetail(lObjTicket.Folio) as List<TicketDetail>;
                    for (int i = 0; i < pLstTicketDetail.Count; i++)
                    {
                        lBoolHasDraft = HasDraft(pLstTicketDetail[i].Folio);
                        if (!lBoolHasDraft)
                        {
                            //get numero de ticket
                            QueryManager lObjQueryManager = new QueryManager();
                            string p = lObjQueryManager.GetValue("U_Number", "U_Folio", pLstTicketDetail[i].Folio, "[@UG_PL_TCKT]");
                            lObjTicket.Number = int.Parse(p);

                            lObjInvoice.CardCode = lObjTicket.BPCode;
                            lObjInvoice.Lines.ItemCode = pLstTicketDetail[i].Item;
                            lObjInvoice.Lines.UnitPrice = pLstTicketDetail[i].Price;
                            lObjInvoice.Lines.COGSCostingCode = lStrCostCenter;
                            lObjInvoice.Lines.TaxCode = "V16";
                            lObjInvoice.Lines.Quantity = 1;
                            lObjInvoice.Lines.WarehouseCode = "PLHE"; //pLstTicketDetail[i].WhsCode;
                            lObjInvoice.Lines.UserFields.Fields.Item("U_GLO_BagsBales").Value = pLstTicketDetail[i].BagsBales;
                            lObjInvoice.Lines.UserFields.Fields.Item("U_PL_Ticket").Value = pLstTicketDetail[i].Folio;
                            lObjInvoice.Lines.Add();
                        }
                        else
                        {
                            lStrDraftFolio += pLstTicketDetail[i].Folio + " ";
                        }

                    }
                }
                if (!lBoolHasDraft && lObjInvoice.Add() != 0)
                {
                    UIApplication.ShowMessageBox(string.Format("Exception: {0}", DIApplication.Company.GetLastErrorDescription()));
                }
                else
                {
                    if (lStrDraftFolio == string.Empty)
                    {
                        lBolIsSuccess = true;
                        MemoryUtility.ReleaseComObject(lObjInvoice);
                        UIApplication.ShowMessageBox(string.Format("Factura realizada correctamente"));
                    }
                    else
                    {
                        SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText("Ya existen Preliminares en folio(s): " + lStrDraftFolio
      , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                    }

                    mStrODraftDocument = DIApplication.Company.GetNewObjectKey();
                }

            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("Exception: {0}", ex.Message));
                LogService.WriteError(DIApplication.Company.GetLastErrorDescription());
            }
            return lBolIsSuccess;
        }

        private bool HasDraft(string pStrFolio)
        {
            return mObjTicketDAO.SearchDrafts(pStrFolio);
        }


        //private bool CrearEntrega(Ticket pObjTicket, IList<TicketDetail> pLstTicketDetail)
        //{
        //    bool lBolIsSuccess = false;
        //    string lStrCostCenter = GetCostCenter();
        //    try
        //    {
        //        SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
        //        string lStrDocEntry = string.Empty;
        //        lObjRecordSet.DoQuery("select DocEntry from OINV where DocNum = " + pObjTicket.Number);
        //        if (lObjRecordSet.RecordCount == 1)
        //        {
        //            lStrDocEntry = lObjRecordSet.Fields.Item(0).Value.ToString();
        //            pObjTicket.Number = int.Parse(lStrDocEntry);
        //        }
        //        MemoryUtility.ReleaseComObject(lObjRecordSet);

        //        SAPbobsCOM.Documents lObjEntrega = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDeliveryNotes);
        //        lObjEntrega.CardCode = pObjTicket.BPCode;

        //        for (int i = 0; i < pLstTicketDetail.Count; i++)
        //        {
        //            if (pObjTicket.Number != 0)
        //            {
        //                lObjEntrega.Lines.BaseEntry = pObjTicket.Number;
        //                lObjEntrega.Lines.BaseLine = i;
        //                lObjEntrega.Lines.BaseType = 13;
        //            }
        //            if (pLstTicketDetail[i].netWeight < 0)
        //                pLstTicketDetail[i].netWeight *= -1;


        //            lObjEntrega.Lines.ItemCode = pLstTicketDetail[i].Item;
        //            lObjEntrega.Lines.UnitPrice = pLstTicketDetail[i].Price;


        //            lObjEntrega.Lines.Quantity = pLstTicketDetail[i].netWeight;
        //            lObjEntrega.Lines.WarehouseCode = "PLHE";
        //            lObjEntrega.Lines.UserFields.Fields.Item("U_GLO_BagsBales").Value = pLstTicketDetail[i].BagsBales;
        //            lObjEntrega.Lines.UserFields.Fields.Item("U_PL_Ticket").Value = pLstTicketDetail[i].Folio;
        //            lObjEntrega.Lines.COGSCostingCode = lStrCostCenter; 
        //            lObjEntrega.Lines.Add();
        //        }

        //        if (lObjEntrega.Add() != 0)
        //        {
        //            UIApplication.ShowMessageBox(string.Format("Exception: {0}", DIApplication.Company.GetLastErrorDescription()));

        //        }
        //        else
        //        {
        //            lBolIsSuccess = true;
        //            MemoryUtility.ReleaseComObject(lObjEntrega);
        //            UIApplication.ShowMessageBox(string.Format("Entrega realizada correctamente"));
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        UIApplication.ShowMessageBox(string.Format("Exception: {0}", ex.Message));
        //    }

        //    return lBolIsSuccess;
        //}
        #endregion
        private bool Creartransferencia(List<Ticket> pLstTicket)
        {
            bool lBolIsSuccess = false;
            string lStrCostCenter = GetCostCenter();
            try
            {
                


                string lStrDocEntry = string.Empty;
                string lStrToWhsCode = string.Empty;
                string lStrFromWhsCode = string.Empty;

                

                foreach (Ticket lObjTicket in pLstTicket)
                {
                    SAPbobsCOM.StockTransfer lObjStockTrasnfer = (SAPbobsCOM.StockTransfer)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer);
                    SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                    lObjRecordSet.DoQuery("select DocEntry, Filler, ToWhsCode from OWTQ where DocEntry = " + lObjTicket.Number);
                    if (lObjRecordSet.RecordCount == 1)
                    {
                        lStrDocEntry = lObjRecordSet.Fields.Item(0).Value.ToString();
                        lStrFromWhsCode = lObjRecordSet.Fields.Item(1).Value.ToString();
                        lStrToWhsCode = lObjRecordSet.Fields.Item(2).Value.ToString();

                        lObjTicket.Number = int.Parse(lStrDocEntry);
                    }
                    MemoryUtility.ReleaseComObject(lObjRecordSet);
                    IList<TicketDetail> lLstTicketDetail = new List<TicketDetail>();
                    lLstTicketDetail = mObjTicketDAO.GetListTicketDetail(lObjTicket.Folio);
                    for (int i = 0; i < lLstTicketDetail.Count; i++)
                    {
                        SAPbobsCOM.Recordset lObjRecordSetDetail = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                        string lStrFromWhs = string.Empty;
                        string lStrWhsCode = string.Empty;
                        lObjRecordSetDetail.DoQuery("select FromWhsCod, WhsCode from WTQ1 where DocEntry = '" + lObjTicket.Number + "' and LineNum = '" + i + "'");
                        if (lObjRecordSetDetail.RecordCount == 1)
                        {
                            lObjStockTrasnfer.FromWarehouse = lObjRecordSetDetail.Fields.Item(0).Value.ToString();
                            lObjStockTrasnfer.ToWarehouse = lObjRecordSetDetail.Fields.Item(1).Value.ToString();
                        }
                        MemoryUtility.ReleaseComObject(lObjRecordSetDetail);


                        if (lObjTicket.Number != 0)
                        {
                            lObjStockTrasnfer.Lines.BaseEntry = int.Parse(lStrDocEntry); // pObjTicket.Number;
                            lObjStockTrasnfer.Lines.BaseLine = lLstTicketDetail[i].BaseLine;
                            lObjStockTrasnfer.Lines.BaseType = SAPbobsCOM.InvBaseDocTypeEnum.InventoryTransferRequest;
                        }
                        if (lLstTicketDetail[i].netWeight < 0)
                        {
                            lLstTicketDetail[i].netWeight *= -1;
                        }
                        lObjStockTrasnfer.Lines.ItemCode = lLstTicketDetail[i].Item;
                        lObjStockTrasnfer.Lines.UnitPrice = lLstTicketDetail[i].Price;
                        lObjStockTrasnfer.Lines.Quantity = lLstTicketDetail[i].netWeight;


                        if (lStrToWhsCode == "PLHE")
                        {
                            lObjStockTrasnfer.Lines.WarehouseCode = lStrToWhsCode;
                            lObjStockTrasnfer.Lines.FromWarehouseCode = lObjStockTrasnfer.FromWarehouse;
                        }
                        else
                        {
                            lObjStockTrasnfer.Lines.WarehouseCode = mObjTicketDAO.GetWhsTransfer(lLstTicketDetail[i].Item);
                            if (string.IsNullOrEmpty(lObjStockTrasnfer.Lines.WarehouseCode))
                            {
                                LogService.WriteInfo("No se encontraron almacenes de transferencia disponibles");
                                UIApplication.ShowMessageBox("No se encontraron almacenes de transferencia disponibles");
                            }
                            lObjStockTrasnfer.Lines.FromWarehouseCode = lLstTicketDetail[i].WhsCode;
                        }
                      
                        //lObjStockTrasnfer.Lines.WarehouseCode = "PLHE";
                        //lObjStockTrasnfer.Lines.FromWarehouseCode 
                        lObjStockTrasnfer.Lines.UserFields.Fields.Item("U_GLO_BagsBales").Value = lLstTicketDetail[i].BagsBales;
                        lObjStockTrasnfer.Lines.UserFields.Fields.Item("U_PL_Ticket").Value = lLstTicketDetail[i].Folio;
                        //lObjStockTrasnfer.Lines.cost = lStrCostCenter; 
                        lObjStockTrasnfer.Lines.Add();
                    }
                    //lObjStockTrasnfer.FromWarehouse = lStrFromWhsCode;
                    //lObjStockTrasnfer.ToWarehouse = lStrToWhsCode;

                    //lObjStockTrasnfer.FromWarehouse = ((SAPbouiCOM.EditText)Matrix1.Columns.Item(3).Cells.Item(i).Specific).Value;
                    //lObjStockTrasnfer.ToWarehouse = lObjInventory.ToWarehouse;
                    lObjStockTrasnfer.DocDate = DateTime.Now;

                    
                    

                    if (lObjStockTrasnfer.Add() != 0)
                    {
                        UIApplication.ShowMessageBox(string.Format("Exception: {0}", DIApplication.Company.GetLastErrorDescription()));
                        LogService.WriteError("[CrearTransferencia]" + DIApplication.Company.GetLastErrorDescription());

                    }
                    else
                    {
                        string lStrNewDocEntry = DIApplication.Company.GetNewObjectKey().ToString();
                        MessageDTO lObjMessageDTO = new MessageDTO();
                        lObjMessageDTO.UserCode = mObjFoodProdutionServiceFactory.GetTicketService().GetUserTransfer(lObjTicket.Number.ToString());
                        string lStrNewDocNum = mObjQueryManager.GetValue("DocNum", "DocEntry", lStrNewDocEntry, "OWTR");
                        lObjMessageDTO.Message = "Se ha realizado la transferencia de mercancia solicitada en el documento #" + lStrNewDocNum;
                        mObjFoodProdutionServiceFactory.GetCreateAlertService().SaveAlert(lObjMessageDTO);
                        lBolIsSuccess = true;
                        LogService.WriteSuccess("[CrearTransferencia] DocNum:" + lStrNewDocNum);
                        MemoryUtility.ReleaseComObject(lObjStockTrasnfer);
                        //UIApplication.ShowMessageBox(string.Format("Transferencia realizada correctamente"));

                    }

                }
              
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("Exception: {0}", ex.Message));
                LogService.WriteError("[CrearTransferencia]" + ex.Message);
            }
            return lBolIsSuccess;
        }

        private void CargarVenta(Ticket pObjTicket, IList<TicketDetail> pLstTicketDetail)
        {
            //  A/R &Invoice	2053f
            //  A/P &Invoice	2308
            UIApplication.GetApplication().ActivateMenuItem("2053"); // Abre ventana de Factura deudores
            //Get Invoice form
            SAPbouiCOM.Form lObjForm = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(133, -1);
            SAPbouiCOM.EditText txtCardCode = (SAPbouiCOM.EditText)lObjForm.Items.Item("4").Specific;
            SAPbouiCOM.EditText txtCardName = (SAPbouiCOM.EditText)lObjForm.Items.Item("54").Specific;
            SAPbouiCOM.EditText txtAmount = (SAPbouiCOM.EditText)lObjForm.Items.Item("22").Specific;
            txtCardCode.Value = pObjTicket.BPCode;
            txtCardName.Active = true;

            //Thread.Sleep(2000);
            SAPbouiCOM.Matrix mtxItems = (SAPbouiCOM.Matrix)lObjForm.Items.Item("38").Specific;
            var a = ((SAPbouiCOM.EditText)mtxItems.Columns.Item(3).Cells.Item(1).Specific);

            //mObjMatrix.LoadFromDataSource();


            lObjForm.Freeze(true);
            try
            {
                SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                string lStrDocEntry = pObjTicket.Number.ToString();
               /* lObjRecordSet.DoQuery("select DocEntry from ORDR where DocNum = " + pObjTicket.Number);
                if (lObjRecordSet.RecordCount == 1)
                {
                    lStrDocEntry = lObjRecordSet.Fields.Item(0).Value.ToString();

                }
                MemoryUtility.ReleaseComObject(lObjRecordSet);*/


                int i = 1;
                foreach (TicketDetail lObjTicketDetail in pLstTicketDetail)
                {

                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(3).Cells.Item(i).Specific).Value = lObjTicketDetail.Item;
                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(20).Cells.Item(i).Specific).Value = lObjTicketDetail.Price.ToString(CultureInfo.InvariantCulture);
                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(13).Cells.Item(i).Specific).Value = lObjTicketDetail.netWeight.ToString(CultureInfo.InvariantCulture);
                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(404).Cells.Item(i).Specific).Value = lObjTicketDetail.Folio.ToString();

                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(54).Cells.Item(i).Specific).Active = true;
                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(54).Cells.Item(i).Specific).Value = pObjTicket.Number.ToString();


                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(55).Cells.Item(i).Specific).Active = true;
                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(55).Cells.Item(i).Specific).Value = lStrDocEntry;


                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(56).Cells.Item(i).Specific).Active = true;
                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(56).Cells.Item(i).Specific).Value = (i - 1).ToString();

                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(53).Cells.Item(i).Specific).Active = true;
                    //   int x = (int)SAPbobsCOM.p.InventoryTransferRequest;
                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(53).Cells.Item(i).Specific).Value = "17";
                    //var x = ((SAPbouiCOM.EditText)mtxItems.Columns.Item(53).Cells.Item(i).Specific).Value;
                    i++;
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));

            }
            finally
            {
                lObjForm.Freeze(false);
            }

            //for (int i = 1; i <= mObjMatrix.Columns.Item(i).Cells.Count; i++)
            //{
            //    if (!string.IsNullOrEmpty(((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("ItemCode").Cells.Item(i).Specific).Value))
            //    {
            //        string b = ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("ItemCode").Cells.Item(i).Specific).Value;
            //        ((SAPbouiCOM.EditText)mtxItems.Columns.Item(3).Cells.Item(i).Specific).Value = ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("ItemCode").Cells.Item(i).Specific).Value;
            //        ((SAPbouiCOM.EditText)mtxItems.Columns.Item(20).Cells.Item(i).Specific).Value = ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Price").Cells.Item(i).Specific).Value;
            //        ((SAPbouiCOM.EditText)mtxItems.Columns.Item(13).Cells.Item(i).Specific).Value = ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("PesoN").Cells.Item(i).Specific).Value;
            //    }
            //}
        }

        private void CargarRecepcion(Ticket pObjTicket, IList<TicketDetail> pLstTicketDetail)
        {
            UIApplication.GetApplication().ActivateMenuItem("2306");
            SAPbouiCOM.Form lObjForm = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(143, -1);
            try
            {
                lObjForm.Freeze(true);
                //  A/R &Invoice	2053
                //  A/P &Invoice	2308
                // Get Invoice form
                SAPbouiCOM.EditText txtCardCode = (SAPbouiCOM.EditText)lObjForm.Items.Item("4").Specific;
                SAPbouiCOM.EditText txtCardName = (SAPbouiCOM.EditText)lObjForm.Items.Item("54").Specific;
                SAPbouiCOM.EditText txtAmount = (SAPbouiCOM.EditText)lObjForm.Items.Item("22").Specific;
                txtCardCode.Value = pObjTicket.BPCode;
                txtCardName.Active = true;

                //Thread.Sleep(2000);
                SAPbouiCOM.Matrix mtxItems = (SAPbouiCOM.Matrix)lObjForm.Items.Item("38").Specific;
                var a = ((SAPbouiCOM.EditText)mtxItems.Columns.Item(3).Cells.Item(1).Specific);
                // mtxItems.
                //mObjMatrix.LoadFromDataSource();


                int i = 1;

                foreach (TicketDetail lObjTicketDetail in pLstTicketDetail)
                {

                    mtxItems.AddRow(1, i);
                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(3).Cells.Item(i).Specific).Active = true;
                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(3).Cells.Item(i).Specific).Value = lObjTicketDetail.Item;
                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(20).Cells.Item(i).Specific).Active = true;
                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(20).Cells.Item(i).Specific).Value = lObjTicketDetail.Price.ToString();
                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(13).Cells.Item(i).Specific).Active = true;

                    if (lObjTicketDetail.netWeight < 0)
                    {
                        lObjTicketDetail.netWeight = lObjTicketDetail.netWeight * -1;
                    }
                    ((SAPbouiCOM.EditText)mtxItems.Columns.Item(13).Cells.Item(i).Specific).Value = lObjTicketDetail.netWeight.ToString();
                    i++;
                }


            }
            catch (Exception)
            {

                //  throw;
            }
            finally
            {
                lObjForm.Freeze(false);
            }
        }

        private void CargarTransferencia(Ticket pObjTicket, IList<TicketDetail> pLstTicketDetail)
        {
            //  A/R &Invoice	2053
            //  A/P &Invoice	2308

            //Open Stock Transfer form
            UIApplication.GetApplication().ActivateMenuItem("3080");

            //Get Invoice form
            SAPbouiCOM.Form lObjForm = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(940, -1);
            SAPbouiCOM.EditText txtCardCode = (SAPbouiCOM.EditText)lObjForm.Items.Item("3").Specific;
            SAPbouiCOM.EditText txtCardName = (SAPbouiCOM.EditText)lObjForm.Items.Item("7").Specific;

            // txtCardCode.Value = pObjTicket.BPCode;
            txtCardName.Active = true;

            //Thread.Sleep(2000);
            SAPbouiCOM.Matrix mtxItems = (SAPbouiCOM.Matrix)lObjForm.Items.Item("23").Specific;

            //mObjMatrix.LoadFromDataSource();

            //for (int i = 1; i <= mObjMatrix.Columns.Item(i).Cells.Count; i++)
            //{
            //    if (!string.IsNullOrEmpty(((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("ItemCode").Cells.Item(i).Specific).Value))
            //    {
            //        string b = ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("ItemCode").Cells.Item(i).Specific).Value;
            //        ((SAPbouiCOM.EditText)mtxItems.Columns.Item(3).Cells.Item(i).Specific).Value = ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("ItemCode").Cells.Item(i).Specific).Value;
            //        ((SAPbouiCOM.EditText)mtxItems.Columns.Item(20).Cells.Item(i).Specific).Value = ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Price").Cells.Item(i).Specific).Value;
            //        ((SAPbouiCOM.EditText)mtxItems.Columns.Item(13).Cells.Item(i).Specific).Value = ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("PesoN").Cells.Item(i).Specific).Value;
            //    }
            //}
        }

        #endregion

        private bool VerifyDocItem(string pStrCode, string pStrItem, string pStrTable)
        {
            bool lBolVerify = false;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "select DocEntry from {Table} with (nolock) where ItemCode = '{ItemCode}' and DocEntry = '{DocEntry}'";

            SAPbobsCOM.Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                List<string> lStrResult = new List<string>();
                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("ItemCode", pStrItem);
                lLstStrParameters.Add("DocEntry", pStrCode);
                lLstStrParameters.Add("Table", pStrTable);
                lStrQuery = lStrQuery.Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);
                if (lObjRecordSet.RecordCount > 0)
                {
                    lBolVerify = true;
                    //lStrResult.Add(lObjRecordSet.Fields.Item(0).Value.ToString());
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("Error de consulta", ex.Message));

            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            return lBolVerify;
            // return lStrResult;
        }

        #region ComboBox

        private void cboTicketType_ComboSelectAfter(object pObjSBOObject, SAPbouiCOM.SBOItemEventArg pObjEventArg)
        {
            string lStrValue = cboTicketType.Value;
            btnAction.Item.Enabled = true;

            switch (lStrValue)
            {
                case "Venta":
                    btnAction.Caption = "Factura";
                    break;

                case "Compra":
                    btnAction.Caption = "Recibir";
                    break;

                case "Traslado - Entrada":
                    btnAction.Caption = "Transferir";
                    break;

                case "Traslado - Salida":
                    btnAction.Caption = "Transferir";
                    break;

                case "Venta de pesaje":
                    btnAction.Caption = "Factura";
                    break;


                default:
                    btnAction.Caption = "Acción";
                    btnAction.Item.Visible = false;
                    break;
            }
        }

        private void cboDocType_ComboSelectAfter(object pObjSBOObject, SAPbouiCOM.SBOItemEventArg pObjEventArg)
        {
            string lStrValue = cboDocType.Value;
            btnAction.Item.Enabled = true;
            if (cboTicketType.Value == "Venta")
            {
                switch (lStrValue)
                {
                    case "Pedido":
                        btnAction.Caption = "Facturar";
                        break;

                    case "Factura de reserva":
                         btnAction.Caption = "Entregar";

                        break;

                    default:
                        //btnAction.Caption = "Acción";
                       // btnAction.Item.Enabled = false;
                        break;
                }
            }
        }

        #endregion

        private SAPbouiCOM.Matrix mtxTicketsList;



        private SAPbobsCOM.StockTransfer GetWhsTransf(SAPbobsCOM.StockTransfer pObjDocument, Ticket pObjTicket, List<TicketDetail> pLstTicketDetail)
        {
            try
            {
                SAPbobsCOM.Recordset lObjRecordSetDetail = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                string lStrFromWhs = string.Empty;
                string lStrWhsCode = string.Empty;
                foreach (TicketDetail lObjTicket in pLstTicketDetail)
                {
                    lObjRecordSetDetail.DoQuery("select FromWhsCod, WhsCode from WTQ1 where DocEntry = '" + pObjTicket.Number + "' and LineNum = '" + lObjTicket.Line + "'");
                    if (lObjRecordSetDetail.RecordCount == 1)
                    {
                        pObjDocument.Lines.WarehouseCode = lObjRecordSetDetail.Fields.Item(0).Value.ToString();
                        pObjDocument.Lines.FromWarehouseCode = lObjRecordSetDetail.Fields.Item(1).Value.ToString();
                    }
                    MemoryUtility.ReleaseComObject(lObjRecordSetDetail);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("GetWhsTransf " + ex.Message);
                LogService.WriteError(ex);
            }
            return pObjDocument;
        }

        private List<string> getSelectedRows()
        {
            string lStrBP = string.Empty;

            List<string> listLabels = new List<string>();

            for (int i = 1; i < mtxTicketsList.RowCount + 1; i++)
            {
                if (((dynamic)mtxTicketsList.Columns.Item(0).Cells.Item(i).Specific).Checked)
                {
                    //Get box label UID
                    string lStrBussines = ((dynamic)mtxTicketsList.Columns.Item(5).Cells.Item(i).Specific).value;
                    if (lStrBP == lStrBussines || lStrBP == "")
                    {
                        lStrBP = lStrBussines;
                    }
                    else
                    {
                        UIApplication.ShowMessageBox(string.Format("No es posible generar documento con diferentes clientes"));
                        return new List<string>();
                    }
                    listLabels.Add(((dynamic)mtxTicketsList.Columns.Item(1).Cells.Item(i).Specific).Value);
                }
            }
            return listLabels;
        }

        private void loadMenu()
        {
            this.UIAPIRawForm.EnableMenu("520", true); // Print
            this.UIAPIRawForm.EnableMenu("6659", false);  // Fax
            this.UIAPIRawForm.EnableMenu("1281", false); // Search Record
            this.UIAPIRawForm.EnableMenu("1282", false); // Add New Record
            this.UIAPIRawForm.EnableMenu("1288", false);  // Next Record
            this.UIAPIRawForm.EnableMenu("1289", false);  // Pevious Record
            this.UIAPIRawForm.EnableMenu("1290", false);  // First Record
            this.UIAPIRawForm.EnableMenu("1291", false);  // Last record 
        }

        #endregion


        #region Controls
        private SAPbouiCOM.EditText txtBP;
        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.StaticText StaticText1;
        private SAPbouiCOM.ComboBox cboTicketStatus;
        private SAPbouiCOM.EditText txtDate1;
        private SAPbouiCOM.EditText txtDate2;
        private SAPbouiCOM.StaticText StaticText2;
        private SAPbouiCOM.StaticText StaticText3;
        private SAPbouiCOM.ComboBox cboTypeDate;
        private SAPbouiCOM.ComboBox cboTicketType;
        private SAPbouiCOM.Button Button0;
        private SAPbouiCOM.Button Button1;
        private SAPbouiCOM.Button btnSearch;
        private SAPbouiCOM.Button btnEdit;
        private SAPbouiCOM.EditText EditText2;
        private SAPbouiCOM.StaticText StaticText4;
        private SAPbouiCOM.Button btnAction;
        private SAPbouiCOM.CheckBox chkSelect;
        private SAPbouiCOM.ComboBox cboDocType;
        private SAPbouiCOM.StaticText lblType;
        #endregion
    }
}
