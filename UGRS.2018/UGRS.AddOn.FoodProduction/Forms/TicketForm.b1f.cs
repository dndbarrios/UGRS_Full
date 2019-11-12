using Microsoft.Reporting.WinForms;
using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using UGRS.AddOn.FoodProduction.DTO;
using UGRS.AddOn.FoodProduction.Enums;
using UGRS.AddOn.FoodProduction.Services;
using UGRS.AddOn.FoodProduction.UI;
using UGRS.AddOn.FoodProduction.UI.Matriz;
using UGRS.AddOn.FoodProduction.Utilities;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Exceptions;
using UGRS.Core.SDK.DI.FoodProduction.DAO;
using UGRS.Core.SDK.DI.FoodProduction.Tables;
using UGRS.Core.Services;
using UGRS.Core.Utility;
using UGRS.Object.WeighingMachine;
//using UGRS.Object.WritePort;



namespace UGRS.AddOn.FoodProduction.Forms
{
    [FormAttribute("UGRS.PlantaAlimentos.Forms.TicketForm", "Forms/TicketForm.b1f")]
    class TicketForm : UserFormBase
    {
        #region Properties
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        /// 
        public SAPbouiCOM.DBDataSource mDBDataSourceD;
        public SAPbouiCOM.DBDataSource mDBDataSourceVenta;
        public SAPbouiCOM.DBDataSource mDBDAtaSourceFac;
        public SAPbouiCOM.ChooseFromList mObjCFLSocio = null;
        public SAPbouiCOM.ChooseFromList mObjCFLVenta = null;
        public SAPbouiCOM.IItem mObjItem;
        public SAPbouiCOM.Matrix mObjMatrix;
        public SAPbouiCOM.IColumn mObjColumn;
        public SAPbouiCOM.ItemEvent mObjItemEvent;
        public string mStrColumn;
        private SAPbouiCOM.UserDataSource oUserDataSourcePesoN;
        private SAPbouiCOM.CommonSetting mObjRowCtrl;
        private string mStrTipoDoc;
        private string mStrSource;
        private string mStrWeight;//Cambiar por double
        private string mStrSimWeight;
        private string mStrLastPeso;
        private string mStrFolio = string.Empty;
        public int mIntRow;
        private int mIntRowModify;
        private string mStrDocEntry;
        private bool mBolConnected;
        private bool mBolPriceModify = false;
        private bool mBolIsUpdate = false;
        private WeighingMachineServerObject mObjWeighingMachine;
        //private WritePortServerObject mObjWritePort;
        private WrapperObject mObjWrapperObject;
        private Guid mObjConnection;
        private Guid mObjConnectionPrinter;
        private Ticket mObjTicket;
        QueryManager mObjQueryManager = new QueryManager();
        TicketServices mObjTicketServices = new TicketServices();
        Calculations mObjCalculation = new Calculations();
        Validations mObjValidations = new Validations();
        TicketDAO mObjTicketDAO = new TicketDAO();
        MatrixUI mObjMatrixUI = new MatrixUI();
        TicketDocumentCreation mObjTicketDocCreation = new TicketDocumentCreation();
        Thread mObjInternalWorker;
        string mStrDataReceived;
        DateTime mDblTime;
        private bool mBolStarted = false;
        int mIntLastPrintLines = 0;

        bool mBolLoadingT = false;///////

        #endregion

        #region Construct
        public TicketForm()
        {
            mBolLoadingT = false;
        }
        /// <summary>
        /// Inicializa el ticket ya guardado.
        public TicketForm(Ticket pObjTicket, IList<TicketDetail> pLstTicketDetail)
        {
            mBolLoadingT = true;
            LoadTicket(pObjTicket, pLstTicketDetail);
            mBolLoadingT = false;
        }
        #endregion

        #region Events

        #region Initialize
        public override void OnInitializeComponent()
        {
            try
            {
                UIApplication.GetApplication().ItemEvent += new _IApplicationEvents_ItemEventEventHandler(TicketFrom_ApplicationItemEvent);
                UIApplication.GetApplication().MenuEvent += new _IApplicationEvents_MenuEventEventHandler(TicketFrom_ApplicationMenuEvent);

                this.cboTypTic = ((SAPbouiCOM.ComboBox)(this.GetItem("cboTypTic").Specific));
                this.txtDoc = ((SAPbouiCOM.EditText)(this.GetItem("txtDoc").Specific));
                this.txtSim = ((SAPbouiCOM.EditText)(this.GetItem("txtSim").Specific));
                this.txtClSoc = ((SAPbouiCOM.EditText)(this.GetItem("txtClSoc").Specific));
                this.txtNomSoc = ((SAPbouiCOM.EditText)(this.GetItem("txtNomSoc").Specific));
                this.lblWeight = ((SAPbouiCOM.StaticText)(this.GetItem("lblWeight").Specific));
                this.btnCopyTo = ((SAPbouiCOM.Button)(this.GetItem("btnCopyTo").Specific));

                this.btnSave = ((SAPbouiCOM.Button)(this.GetItem("btnSave").Specific));
                this.btnSave.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSave_ClickBefore);
                this.cbWTType = ((SAPbouiCOM.ComboBox)(this.GetItem("cbWTType").Specific));
                this.cbCapType = ((SAPbouiCOM.ComboBox)(this.GetItem("cbCapType").Specific));
                this.txtDriver = ((SAPbouiCOM.EditText)(this.GetItem("txtDriver").Specific));
                this.txtCarTag = ((SAPbouiCOM.EditText)(this.GetItem("txtCarTag").Specific));
                this.txtComents = ((SAPbouiCOM.EditText)(this.GetItem("txtComents").Specific));
                this.txtOutputWT = ((SAPbouiCOM.EditText)(this.GetItem("tOutputWT").Specific));
                this.txtInWT = ((SAPbouiCOM.EditText)(this.GetItem("tInputWT").Specific));

                this.txtPesoB = ((SAPbouiCOM.EditText)(this.GetItem("txtPesoB").Specific));
                this.txtPesoN = ((SAPbouiCOM.EditText)(this.GetItem("txtPesoNet").Specific));
                this.txtTara = ((SAPbouiCOM.EditText)(this.GetItem("txtTara").Specific));
                this.txtAmount = ((SAPbouiCOM.EditText)(this.GetItem("txtAmount").Specific));
                this.btnWeight = ((SAPbouiCOM.Button)(this.GetItem("btnWeight").Specific));

                this.txtDate = ((SAPbouiCOM.EditText)(this.GetItem("txtDate").Specific));
                this.txtDateOut = ((SAPbouiCOM.EditText)(this.GetItem("txtDateOut").Specific));
                //this.mtxArtLst = ((SAPbouiCOM.Matrix)(this.GetItem("txtDateOut").Specific));

                this.txtFolio = ((SAPbouiCOM.EditText)(this.GetItem("txtFolio").Specific));
                this.txtStatus = ((SAPbouiCOM.EditText)(this.GetItem("txtStatus").Specific));
                this.btnLastWT = ((SAPbouiCOM.Button)(this.GetItem("btnLastWT").Specific));
                this.txtVari = ((SAPbouiCOM.EditText)(this.GetItem("txtVari").Specific));

                this.btnWT1 = ((SAPbouiCOM.Button)(this.GetItem("btnWT1").Specific));
                this.btnWT2 = ((SAPbouiCOM.Button)(this.GetItem("btnWT2").Specific));
                this.cbDocType = ((SAPbouiCOM.ComboBox)(this.GetItem("cbDocType").Specific));
                this.lblTypeD = (SAPbouiCOM.StaticText)(this.GetItem("lblTypeD").Specific);

                this.btnCancel = (SAPbouiCOM.Button)(this.GetItem("btnCancel").Specific);
                this.btnClose = (SAPbouiCOM.Button)(this.GetItem("btnClose").Specific);
                this.btnAddItem = (SAPbouiCOM.Button)(this.GetItem("btnAdd").Specific);
                this.btnDelete = (SAPbouiCOM.Button)(this.GetItem("btnDelete").Specific);
                this.txtPro = ((SAPbouiCOM.EditText)(this.GetItem("txtPro").Specific));
                this.lblProj = (SAPbouiCOM.StaticText)(this.GetItem("Item_4").Specific);
                this.btnPrint = (SAPbouiCOM.Button)(this.GetItem("btnPrint").Specific);

                cboTypTic.ValidValues.Add("Seleccione", "Seleccione");
                cboTypTic.ValidValues.Add("Venta", "Venta");
                cboTypTic.ValidValues.Add("Compra", "Compra");
                cboTypTic.ValidValues.Add("Traslado - Entrada", "Traslado - Entrada");
                cboTypTic.ValidValues.Add("Traslado - Salida", "Traslado - Salida");
                cboTypTic.ValidValues.Add("Venta de pesaje", "Venta de pesaje");
                cboTypTic.ValidValues.Add("Pesaje", "Pesaje");
                cboTypTic.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;

                ///Item invisible temporalmente
                txtPro.Item.Visible = false;
                lblProj.Item.Visible = false;

                cbCapType.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
                cbDocType.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
                cboTypTic.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
                cbWTType.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;

                btnWT1.Item.Visible = false;
                btnWT2.Item.Visible = false;
                btnClose.Item.Visible = false;
                btnSave.Item.Visible = true;
                lblTypeD.Item.Visible = false;
                cbDocType.Item.Visible = false;
                btnPrint.Item.Visible = false;
                cbDocType.Select(0, BoSearchKey.psk_Index);

                txtDoc.Item.Enabled = false;

                mStrFolio = GetLastTicket().ToString();
                txtFolio.Value = mStrFolio;
                txtStatus.Value = "Abierto";

                //this.txtTara.LostFocusAfter += new SAPbouiCOM._IEditTextEvents_LostFocusAfterEventHandler(this.LostFocusTara);
                this.btnWeight.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnWeight_ClickBefore);
                this.btnWT1.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnWT1_ClickBefore);
                this.btnWT2.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnWT2_ClickBefore);
                this.btnCancel.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCancel_ClickBefore);
                this.btnAddItem.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnAddItem_ClicBefore);
                this.btnDelete.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnDelete_ClicDelete);
                this.btnClose.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnClose_ClickBefore);
                this.btnPrint.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnPrint_ClickBefore);
                this.txtSim.ValidateAfter += new SAPbouiCOM._IEditTextEvents_ValidateAfterEventHandler(this.txtSimValidateAfter);




                this.LoadApplicationEvents();
                this.OnCustomInitialize();

                btnDelete.Item.Visible = false;
                btnAddItem.Item.Visible = false;

                AddChooseFromList();
                CreateMatrix();
                txtOutputWT.Item.Enabled = false;
                txtInWT.Item.Enabled = false;
                // HttpChannel lObjChannel = new HttpChannel();
                cboTypTic.Select(0, BoSearchKey.psk_Index);
                txtClSoc.Item.Enabled = false;
                //ChannelServices.RegisterChannel(lObjChannel, false);
                // InitializeRemoteServer();


                cbDocType.Select(0, BoSearchKey.psk_Index);
                //mStrWeight = mObjTicketServices.RandomNumber(1, 100).ToString();
                lblWeight.Caption = mStrWeight;


                //createfactura();


            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("Iniciar el formulario: {0}", ex.Message));
                LogService.WriteError(string.Format("Iniciar el formulario: {0}", ex.Message));
                LogService.WriteError(ex);
                // throw;
            }

            //print();
        }

        private void txtSimValidateAfter(object sboObject, SBOItemEventArg pVal)
        {
            string lStrSim = string.Empty;
            Form lObjForm = UIApplication.GetApplication().Forms.ActiveForm;
            this.txtSim = ((SAPbouiCOM.EditText)(lObjForm.Items.Item("txtSim").Specific));
            if (!string.IsNullOrEmpty(txtSim.Value))
            {
                lStrSim = txtSim.Value;
                if (lStrSim.Length < 9 && Convert.ToDecimal(lStrSim) > 0)
                {
                    mStrWeight = lStrSim;
                    mStrSimWeight = lStrSim;
                }
            }

        }




        private void btnDelete_ClicDelete(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                string lStrLineNum = mDBDataSourceD.GetValue("LineNum", mIntRow - 1);
                string lStrPeso = mDBDataSourceD.GetValue("LineTotal", mIntRow - 1);
                string lStrBaseLine = mDBDataSourceD.GetValue("LineNum", mIntRow - 1);
                string lStrStatus = txtStatus.Value;
                /*(lStrBaseLine == "" || cboTypTic.Value == "Traslado - Entrada" || cboTypTic.Value == "Traslado - Salida") &&*/
                if (lStrStatus == "Abierto" && Convert.ToDouble(lStrPeso, CultureInfo.GetCultureInfo("en-US")) == 0)
                {
                    if (SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("¿Desea elimiar el item seleccionado?", 2, "Si", "No", "") == 1)
                    {
                        this.UIAPIRawForm.Freeze(true);
                        mObjMatrix.DeleteRow(mIntRow);
                        mDBDataSourceD.RemoveRecord(mIntRow - 1);
                        mObjMatrix.LoadFromDataSource();
                        mObjMatrix.Item.Refresh();
                        this.UIAPIRawForm.Freeze(false);
                        btnDelete.Item.Visible = false;
                        SearchLastWeight(); /////LE
                        CheckCurrentRow();////LE
                        mIntRow = 0;
                        if (mObjMatrix.RowCount > 0)
                        {
                            CalcImport(cboTypTic.Value);
                        }
                    }
                }
                else
                {
                    SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("No es posible eliminar la linea \n Verifique que el documento no este cerrado o no tenga registrado un peso ");
                }
                //if (string.IsNullOrEmpty(lStrLineNum))
                //{
                //    UIApplication.ShowMessage("No se posible eliminar un item de ");

                CalcTotals();
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("[btnDelete_ClicDelete]: {0}", ex.Message));
                LogService.WriteError(ex);
            }
        }



        #region reacomodar luis??
        /// <summary>
        /// Metodo CheckcurrentRow, ver cual es la linea correspondiente al ultimo peso
        /// </summary>
        private void CheckCurrentRow()
        {
            double lDbSecondW = 0;
            for (int i = 1; i < mObjMatrix.RowCount; i++)
            {
                lDbSecondW = Convert.ToDouble((mObjMatrix.Columns.Item("Peso2").Cells.Item(i).Specific as EditText).Value, CultureInfo.GetCultureInfo("en-US"));
                if (lDbSecondW.Equals(Convert.ToDouble(txtOutputWT.Value, CultureInfo.GetCultureInfo("en-US"))))
                {
                    mIntRow = i;
                }
            }

        }
        /// <summary>
        /// Metodo LastWeight
        /// </summary>
        private void SearchLastWeight()
        {
            double lFlMaxOrMinValue = MaxAndMinValues();

            txtOutputWT.Value = lFlMaxOrMinValue.ToString(CultureInfo.GetCultureInfo("en-US"));

        }
        /// <summary>
        /// buscar el ultimo peso maximo o minimo
        /// </summary>
        /// <returns></returns>
        private double MaxAndMinValues()
        {
            double lFlMaxOrMin = 0;
            if (mStrSource.Equals("RDR1"))
            {
                lFlMaxOrMin = mObjCalculation.getLargeNumber(mObjMatrix);
            }
            else
            {
                if (mStrSource.Equals("POR1"))
                {
                    lFlMaxOrMin = mObjCalculation.getSmallerNumber(mObjMatrix);
                }
            }
            return lFlMaxOrMin;
        }
        #endregion

        private void btnAddItem_ClicBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                this.UIAPIRawForm.Freeze(true);

                if (cbDocType.Value != "Factura de reserva")
                {
                    if (mObjMatrix.RowCount > 0)
                    {
                        string lStrLastItem = (mObjMatrix.Columns.Item("ItemCode").Cells.Item(mObjMatrix.RowCount).Specific as EditText).Value;

                        if (mObjMatrix.RowCount > 0 && !string.IsNullOrEmpty(lStrLastItem))
                        {
                            Additem();
                        }
                    }
                    else
                    {
                        Additem();
                    }

                    if (mObjMatrix.RowCount > 0)
                    {
                        ((SAPbouiCOM.CheckBox)mObjMatrix.Columns.Item("Check").Cells.Item(mObjMatrix.RowCount).Specific).Checked = false;
                    }
                    CalcTotals();
                }
                else
                {
                    SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText("No se pueden agregar líneas cuando el documento es Factura de reserva"
                    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }
                mObjMatrix.LoadFromDataSource();

            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("btnAddItem_ClicBefore: {0}", ex.Message));
                LogService.WriteError(ex);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }

        }
        private void AddCheckboxData()
        {
            for (int i = 0; i < mDBDataSourceD.Size; i++)
            {
                string s = mDBDataSourceD.GetValue("TreeType", i);
                if (mDBDataSourceD.GetValue("TreeType", i) == "Y")
                {
                    ((SAPbouiCOM.CheckBox)mObjMatrix.Columns.Item("Check").Cells.Item(i + 1).Specific).Checked = true;
                    //mDBDataSourceD.SetValue("TreeType", i - 1, "Y");

                }
            }
        }

        private void Additem()
        {
            mDBDataSourceD.InsertRecord(mObjMatrix.RowCount);
            mDBDataSourceD.SetValue("ItemCode", mObjMatrix.RowCount, null);
            mObjMatrix.AddRow();
            (mObjMatrix.Columns.Item("ItemCode").Cells.Item(mObjMatrix.RowCount).Specific as EditText).Value = "";
            (mObjMatrix.Columns.Item("Peso1").Cells.Item(mObjMatrix.RowCount).Specific as EditText).Value = "0.000";
            (mObjMatrix.Columns.Item("Peso2").Cells.Item(mObjMatrix.RowCount).Specific as EditText).Value = "0.000";
            //(mObjMatrix.Columns.Item("PesoN").Cells.Item(pObjVal.Row).Specific as EditText).Value = "0.000";
            (mObjMatrix.Columns.Item("Importe").Cells.Item(mObjMatrix.RowCount).Specific as EditText).Value = "0.000";

            mDBDataSourceD.SetValue("Quantity", mObjMatrix.RowCount, "0.0");
            //mDBDataSourceD.SetValue("LineTotal", mIntRow - 1, "0.0");
            mDBDataSourceD.SetValue("Weight1", mObjMatrix.RowCount, "0.0");
            mDBDataSourceD.SetValue("Weight2", mObjMatrix.RowCount, "0.0");
            mDBDataSourceD.SetValue("TreeType", mObjMatrix.RowCount, "N");
            mBolPriceModify = false;
            AddCheckboxData();
            mObjMatrix.LoadFromDataSource();

            //VerificarCheck();
            //mDBDataSourceD.InsertRecord(0);
            // mDBDataSourceD.InsertRecord(mObjMatrix.RowCount-1);
            //mObjMatrix.AddRow();
            //mObjMatrix.Item.Refresh();
        }


        /// <summary>
        /// Boton cancelar
        /// </summary>
        private void btnCancel_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            UIAPIRawForm.Close();
        }

        public override void OnInitializeFormEvents()
        {

        }
        private void InitializeRemoteServer()
        {
            //RemotingConfiguration.RegisterWellKnownClientType(typeof(BasculeObject), "http://localhost:12345/BasculeSever"); //Error 

        }
        private void OnCustomInitialize()
        {
            loadMenu();
            InitDataSources();
            //StartService();
            //GetRemoteObject();
            //GetRemoteObjectPrint();
            mBolStarted = true;
        }



        #endregion

        #region Application
        ///<summary>    Sbo application item event. </summary>

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
                                if (pVal.Action_Success)
                                {
                                    ChooseFromListAfterEvent(pVal);
                                }
                                break;

                            case BoEventTypes.et_COMBO_SELECT:
                                if (pVal.FormMode > 1)
                                {
                                    ChangeCaptureMode(pVal);
                                }
                                break;

                            case BoEventTypes.et_FORM_CLOSE:
                                UnloadApplicationEvents();
                                break;
                        }
                    }
                    else
                    {
                        switch (pVal.EventType)
                        {
                            case BoEventTypes.et_CHOOSE_FROM_LIST:
                                if (pVal.ItemUID == "txtDoc")
                                {
                                    SetParametersToChoose();
                                }
                                break;
                        }
                    }


                    if (mBolStarted && (string.IsNullOrEmpty(lblWeight.Caption.Trim()) || string.IsNullOrEmpty(mStrWeight)))
                    {
                       // LogService.WriteError("No fue posible leer datos de la bascula");
                       // ServiceController mObjServiceController = new ServiceController();
                        //mObjServiceController.ServiceName = "WeighingMachineService";

                        //mObjServiceController.Close();
                        //mObjServiceController.Stop();
                        //  mObjServiceController.WaitForStatus(ServiceControllerStatus.Running);
                        //LogService.WriteInfo("Deteniendo servicio de impresion");
                        //UIApplication.ShowWarning("Deteniendo servicio de impresion");
                        //mObjServiceController.WaitForStatus(ServiceControllerStatus.Stopped);

                       // StartService();
                        //GetRemoteObject();//Servicio de bascula
                    }
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("ItemEventException: {0}", ex.Message));
                LogService.WriteError(string.Format("SBO_Application_ItemEvent: {0}", ex.Message));
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Botones superiores
        /// </summary>
        private void TicketFrom_ApplicationMenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (!pVal.BeforeAction && UIApplication.GetApplication().Forms.ActiveForm.UniqueID == this.UIAPIRawForm.UniqueID)
                {
                    int lIntFolio;
                    switch (pVal.MenuUID)
                    {
                        case "1281": // Search Record
                            txtFolio.Item.Enabled = true;
                            txtFolio.Item.Click();
                            break;

                        case "1282": // Add New Record
                            Ticket lObjTicket = new Ticket();
                            IList<TicketDetail> lLstTicket = new List<TicketDetail>();
                            lObjTicket.Status = 1;
                            LoadTicket(lObjTicket, lLstTicket);
                            btnPrint.Item.Visible = false;

                            cboTypTic.Select(0, BoSearchKey.psk_Index);
                            cboTypTic.Item.Enabled = true;
                            mBolIsUpdate = false;
                            mStrSource = "RDR1";
                            break;

                        case "1288": // Next Record
                            lIntFolio = Convert.ToInt32(txtFolio.Value) + 1;
                            if (lIntFolio > (GetLastTicket() - 1))
                            {
                                UIApplication.ShowWarning(string.Format("Primer registro de datos"));
                                SearchTicketEnter("1");
                            }
                            else
                            {
                                SearchTicketEnter(lIntFolio.ToString());
                            }
                            break;

                        case "1289": // Pevious Record
                            lIntFolio = Convert.ToInt32(txtFolio.Value) - 1;
                            if (lIntFolio < 1)
                            {
                                SearchTicketEnter((GetLastTicket() - 1).ToString());
                                UIApplication.ShowWarning(string.Format("Ultimo registro de datos"));
                            }
                            else
                            {
                                SearchTicketEnter(lIntFolio.ToString());
                            }
                            break;

                        case "1290": // First Record
                            SearchTicketEnter("1");

                            //SearchTicketEnter(lIntFolio.ToString());
                            break;

                        case "1291": // Last record 
                            SearchTicketEnter((GetLastTicket() - 1).ToString());
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("MenuEventException: {0}", ex.Message));
                LogService.WriteError(string.Format("TicketFrom_ApplicationMenuEvent: {0}", ex.Message));
                LogService.WriteError(ex);
            }
        }

        ///<summary>    Ticket from application item event. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        ///<param name="FormUID">       The form UID. </param>
        ///<param name="pObjVal">          [in,out] The value. </param>btn
        ///<param name="BubbleEvent">   [out] True to bubble event. </param>
        private void TicketFrom_ApplicationItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pObjVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (this.UIAPIRawForm != null && FormUID.Equals(this.UIAPIRawForm.UniqueID) && !pObjVal.BeforeAction)
                {
                    switch (pObjVal.EventType)
                    {
                        case BoEventTypes.et_FORM_CLOSE:
                            UIApplication.GetApplication().ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(TicketFrom_ApplicationItemEvent);
                            UIApplication.GetApplication().MenuEvent -= new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(TicketFrom_ApplicationMenuEvent);
                            break;
                        case BoEventTypes.et_CLICK:
                            if (mBolPriceModify)
                            {

                                ClicPesoN(pObjVal);
                            }


                            if (pObjVal.ItemUID == "mtxArtLst2" && mObjMatrix.RowCount > 0 && pObjVal.Row > 0 && !mBolLoadingT)
                            {
                                if (!mObjMatrix.IsRowSelected(pObjVal.Row))
                                {
                                    SelectRow(pObjVal.Row, pObjVal.ColUID);
                                }
                                else
                                {
                                    mIntRow = pObjVal.Row;
                                    mStrColumn = pObjVal.ColUID;
                                }

                                //Realiza calculos si la celda esta habilitada



                                if (mIntRow > 0 && (mObjMatrix.Columns.Item(pObjVal.ColUID).Editable == true))//.Cells.Item(pObjVal.Row).Specific as EditText).Item.Enabled)
                                {
                                    SelectedRowSettings();

                                    if (pObjVal.ColUID.Equals("Check"))
                                    {
                                        ClicCheckbox(pObjVal);
                                    }
                                    else if (pObjVal.ColUID.Equals("PesoN"))
                                    {
                                        ClicPesoN(pObjVal);
                                    }
                                    else if (pObjVal.ColUID.Equals("Sacos"))
                                    {
                                        ClicPesoN(pObjVal);
                                    }
                                    if (pObjVal.ColUID.Equals("Price"))
                                    {
                                        ClicPesoN(pObjVal);
                                    }
                                    if (MakeCalculation(pObjVal.Row, pObjVal.ColUID))
                                    {
                                        ReloadDatasource(pObjVal.Row, pObjVal.ColUID);
                                    }
                                }




                                
                            }


                            //if (pObjVal.ItemUID == "mtxArtLst2" && mObjMatrix.RowCount > 0 && pObjVal.Row > 0 && IsDisableRow(pObjVal.Row)) //CLICK EVENT
                            //{
                            //    if (!mObjMatrix.IsRowSelected(pObjVal.Row))
                            //    {
                            //        SelectRow(pObjVal.Row, pObjVal.ColUID);
                            //    }

                            //}

                            //if (pObjVal.ItemUID == "mtxArtLst2" && pObjVal.ColUID == "Check")
                            //{
                            //    ClicCheckbox(pObjVal);
                            //    CalcImport(cboTypTic.Value);
                            //    CalcTotals();
                            //}

                            break;
                        case BoEventTypes.et_KEY_DOWN:
                            if (pObjVal.CharPressed == 9)
                            {
                                ClicTabPesoN();
                            }
                            if (pObjVal.CharPressed == 13 && (pObjVal.ItemUID == "txtFolio"))
                            {
                                SearchTicketEnter(txtFolio.Value);
                            }
                            if (pObjVal.CharPressed == 13 && (pObjVal.ItemUID == "txtTara"))
                            {
                                // LostFocusTara();
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                //UIApplication.ShowError(string.Format("Eventos clic: {0} {1}", ex.Message, pObjVal.ColUID));
                LogService.WriteError(string.Format("TicketFrom_ApplicationItemEvent: {0} {1}", ex.Message, pObjVal.ColUID));
                LogService.WriteError(ex);
            }
        }



        private bool MakeCalculation(int pIntRow, string pStrColumn)
        {
            LogService.WriteInfo("MakeCalculation");
            double lDblPesoNeto = Convert.ToDouble((mObjMatrix.Columns.Item("PesoN").Cells.Item(pIntRow).Specific as EditText).Value.Trim(), CultureInfo.GetCultureInfo("en-US"));
            if (lDblPesoNeto > 0 && !pStrColumn.Equals("Price") && !pStrColumn.Equals("PesoN") && !pStrColumn.Equals("Sacos") && !pStrColumn.Equals("ItemCode") && !pStrColumn.Equals("WhsCode"))
            {
                this.UIAPIRawForm.Freeze(true);
                CalcImport(cboTypTic.Value);
                //CalcTotals();
                this.UIAPIRawForm.Freeze(false);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ReloadDatasource(int pIntRow, string pStrColumn)
        {
            //if (mObjValidations.VerificarSegundoPeso(mObjMatrix) && !string.IsNullOrEmpty(mStrLastPeso))
            //{
            this.UIAPIRawForm.Freeze(true);
            mObjMatrix.LoadFromDataSource();
            SelectRow(pIntRow, pStrColumn);
            this.UIAPIRawForm.Freeze(false);
            //}
        }

        private void SelectedRowSettings()
        {
            LogService.WriteInfo("SelectedRowSettings");
            bool lBolCheck = false;
            if (cboTypTic.Value != "Venta de pesaje")
            {
                lBolCheck = (mObjMatrix.Columns.Item("Check").Cells.Item(mIntRow).Specific as CheckBox).Checked;
            }
            string lStrPeso1 = ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Peso1").Cells.Item(mIntRow).Specific).Value;
            mStrLastPeso = "";
            if (mStrSource == "RDR1" || cboTypTic.Value == "Traslado - Salida")
            {
                if (mObjCalculation.getLargeNumber(mObjMatrix) > 0)
                {
                    mStrLastPeso = mObjCalculation.getLargeNumber(mObjMatrix).ToString(CultureInfo.GetCultureInfo("en-US")); 
                }
            }
            if (mStrSource == "POR1" || cboTypTic.Value == "Traslado - Entrada")
            {
                if (mObjCalculation.getSmallerNumber(mObjMatrix) > 0)
                {
                    mStrLastPeso = mObjCalculation.getSmallerNumber(mObjMatrix).ToString(CultureInfo.GetCultureInfo("en-US"));
                }
            }

            var sss = (((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("ItemCode").Cells.Item(mIntRow).Specific).Value);

            if (mObjValidations.VerificarSegundoPeso(mObjMatrix) && !string.IsNullOrEmpty(mStrLastPeso) && lStrPeso1 == "0.0" && !lBolCheck
                && !string.IsNullOrEmpty(((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("ItemCode").Cells.Item(mIntRow).Specific).Value))
            {
                if (mStrColumn != "Check")
                {
                    if (SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Desea establecer el peso guardado como primer peso", 2, "Si", "No", "") == 1)
                    {
                        this.UIAPIRawForm.Freeze(true);
                        ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Peso1").Cells.Item(mIntRow).Specific).Value = mStrLastPeso;
                        mDBDataSourceD.SetValue("Weight1", mIntRow - 1, mStrLastPeso);
                        mObjMatrix.LoadFromDataSource();
                        //mStrLastPeso = string.Empty;
                    }
                }
                this.UIAPIRawForm.Freeze(false);
            }
            if (cboTypTic.Value != "Venta de pesaje" && cboTypTic.Value != "Pesaje")
            {
                btnDelete.Item.Visible = true;
            }
        }


        /// <summary>
        /// Coloca una bandera si se le da clic a la celda de peso neto (lostfocus)
        /// </summary>
        private void ClicPesoN(SAPbouiCOM.ItemEvent pObjVal)
        {
           
            LogService.WriteInfo("ClicPeso");
            if (pObjVal.ItemUID == "mtxArtLst2" && (pObjVal.ColUID == "PesoN" || pObjVal.ColUID == "Sacos" || pObjVal.ColUID == "Price"))
            {
                if (!mBolPriceModify)
                {
                    mBolPriceModify = true;

                    mIntRowModify = pObjVal.Row;
                    double ldblPesoNet = Convert.ToDouble((mObjMatrix.Columns.Item("PesoN").Cells.Item(pObjVal.Row).Specific as EditText).Value.Trim(), CultureInfo.GetCultureInfo("en-US"));
                    double ldblSacos = Convert.ToDouble((mObjMatrix.Columns.Item("Sacos").Cells.Item(pObjVal.Row).Specific as EditText).Value.Trim(), CultureInfo.GetCultureInfo("en-US"));
                    double ldblPrice = Convert.ToDouble((mObjMatrix.Columns.Item("Price").Cells.Item(pObjVal.Row).Specific as EditText).Value.Trim(), CultureInfo.GetCultureInfo("en-US"));
                    if (ldblPesoNet == 0)
                    {
                        mDBDataSourceD.SetValue("Quantity", pObjVal.Row - 1, ldblPesoNet.ToString(CultureInfo.GetCultureInfo("en-US")));
                        CalcImport(cboTypTic.Value);
                    }
                    mDBDataSourceD.SetValue("U_GLO_BagsBales", pObjVal.Row - 1, ldblSacos.ToString(CultureInfo.GetCultureInfo("en-US")));
                    mDBDataSourceD.SetValue("Price", pObjVal.Row - 1, ldblPrice.ToString(CultureInfo.GetCultureInfo("en-US")));
                }
                else
                {
                    if (mIntRowModify != pObjVal.Row)
                    {

                        double ldblPesoNet = Convert.ToDouble((mObjMatrix.Columns.Item("PesoN").Cells.Item(mIntRowModify).Specific as EditText).Value.Trim(), CultureInfo.GetCultureInfo("en-US"));
                        double ldblSacos = Convert.ToDouble((mObjMatrix.Columns.Item("Sacos").Cells.Item(mIntRowModify).Specific as EditText).Value.Trim(), CultureInfo.GetCultureInfo("en-US"));
                        double ldblPrice = Convert.ToDouble((mObjMatrix.Columns.Item("Price").Cells.Item(pObjVal.Row).Specific as EditText).Value.Trim(), CultureInfo.GetCultureInfo("en-US"));

                        mDBDataSourceD.SetValue("Quantity", mIntRowModify - 1, ldblPesoNet.ToString(CultureInfo.GetCultureInfo("en-US")));
                        CalcImport(cboTypTic.Value);

                        mDBDataSourceD.SetValue("U_GLO_BagsBales", mIntRowModify - 1, ldblSacos.ToString(CultureInfo.GetCultureInfo("en-US")));
                        mDBDataSourceD.SetValue("Price", pObjVal.Row - 1, ldblPrice.ToString(CultureInfo.GetCultureInfo("en-US")));
                        mIntRowModify = pObjVal.Row;
                    }
                }
            }
            else
            {
                if (mBolPriceModify && mObjMatrix.RowCount > 0)
                {
                    CalcImport(cboTypTic.Value);
                    mBolPriceModify = false;
                }
            }
        }



        /// <summary>
        /// Calcula el importe si presiona la tecla tab (lostFocus)
        /// </summary>
        private void ClicTabPesoN()
        {
            if (mBolPriceModify)
            {
                CalcImport(cboTypTic.Value);
            }
            mBolPriceModify = false;
        }

        /// <summary>
        /// Realiza la busqueda si se presiona la tecla enter
        /// </summary>
        private void SearchTicketEnter(string pStrFolio)
        {
            //// string pStrCode = mObjQueryManager.GetValue("Code", "U_Folio", pStrFolio, "[@UG_PL_TCKT]");
            string pStrCode = mObjQueryManager.GetValue("Code", "U_Folio", pStrFolio, "[@UG_PL_TCKT]");

            Ticket lObjTicket = new Ticket(); ;
            IList<TicketDetail> lObjTicketDetail = null;
            //TicketDAO mObjTicketDAO = new TicketDAO();
            lObjTicket = mObjTicketDAO.GetTicket(pStrCode);
            lObjTicketDetail = mObjTicketDAO.GetListTicketDetail(pStrFolio);
            if (lObjTicket != null)
            {
                LoadTicket(lObjTicket, lObjTicketDetail);

                txtComents.Item.Click();
                txtFolio.Item.Enabled = false;
            }
            else
            {
                UIApplication.ShowMessageBox(string.Format("Ticket no encontrado"));
            }
            this.UIAPIRawForm.Mode = BoFormMode.fm_OK_MODE;
        }

        /// <summary>
        /// Inicializa los pesos al seleccionar el checkbox del renglon
        /// </summary>
        private void ClicCheckbox(SAPbouiCOM.ItemEvent pObjVal)
        {
            //if ((mObjMatrix.Columns.Item("OpenCreQty").Cells.Item(pObjVal.Row).Specific as EditText).Value != "0.0") //verifica que la cantidad pendiente
            //{
            mIntRow = pObjVal.Row;
            SAPbouiCOM.CommonSetting lObjRowCtrl;
            lObjRowCtrl = mObjMatrix.CommonSetting;
            mStrLastPeso = string.Empty;

            bool lBoolCheck = true;
            //if ((mObjMatrix.Columns.Item("PesoN").Cells.Item(pObjVal.Row).Specific as EditText).Value == "0.0")
            //{
            if (mIntRow > 0)
            {
                if (!(mObjMatrix.Columns.Item("Check").Cells.Item(pObjVal.Row).Specific as CheckBox).Checked)
                {
                    lObjRowCtrl.SetCellEditable(pObjVal.Row, 5, false);
                    mDBDataSourceD.SetValue("Quantity", pObjVal.Row, "0.000");
                    mDBDataSourceD.SetValue("LineTotal", pObjVal.Row, "0.000");
                    mDBDataSourceD.SetValue("TreeType", pObjVal.Row - 1, "N");
                }
                else
                {
                    this.UIAPIRawForm.Freeze(true);
                    mStrLastPeso = "";
                    lObjRowCtrl.SetCellEditable(pObjVal.Row, 5, true);
                    string sfes = (mObjMatrix.Columns.Item("Peso1").Cells.Item(pObjVal.Row).Specific as EditText).Value;
                    if (((mObjMatrix.Columns.Item("Peso1").Cells.Item(pObjVal.Row).Specific as EditText).Value != "0.0" || (mObjMatrix.Columns.Item("Peso2").Cells.Item(pObjVal.Row).Specific as EditText).Value != "0.0"))
                    {
                        this.UIAPIRawForm.Freeze(false);
                        if (SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Desea capturar el peso manual se borraran los registros anteriores", 2, "Si", "No", "") == 1)
                        {
                            this.UIAPIRawForm.Freeze(true);
                            (mObjMatrix.Columns.Item("Peso1").Cells.Item(pObjVal.Row).Specific as EditText).Value = "0.000";
                            (mObjMatrix.Columns.Item("Peso2").Cells.Item(pObjVal.Row).Specific as EditText).Value = "0.000";
                            //(mObjMatrix.Columns.Item("PesoN").Cells.Item(pObjVal.Row).Specific as EditText).Value = "0.000";
                            (mObjMatrix.Columns.Item("Importe").Cells.Item(pObjVal.Row).Specific as EditText).Value = "0.000";
                            mDBDataSourceD.SetValue("Quantity", mIntRow - 1, "0.0");
                            //mDBDataSourceD.SetValue("LineTotal", mIntRow - 1, "0.0");
                            mDBDataSourceD.SetValue("Weight1", mIntRow - 1, "0.0");
                            mDBDataSourceD.SetValue("Weight2", mIntRow - 1, "0.0");

                            lBoolCheck = true;

                            //mObjMatrix.LoadFromDataSource();
                        }
                        else
                        {
                            lBoolCheck = false;
                        }
                    }

                    if (lBoolCheck)
                    {
                        mBolPriceModify = true;
                        mIntRowModify = pObjVal.Row;
                        mDBDataSourceD.SetValue("TreeType", mIntRow - 1, "Y");
                    }
                    else
                    {
                        mBolPriceModify = false;
                        mDBDataSourceD.SetValue("TreeType", mIntRow - 1, "N");
                    }

                    ReloadDatasource(mIntRow, "0");

                    this.UIAPIRawForm.Freeze(false);
                }
            }
            //CalcImport(cboTypTic.Value);
            VerificarCheck();
            //}
            //else
            //{
            //    ((SAPbouiCOM.CheckBox)mObjMatrix.Columns.Item("Check").Cells.Item(pObjVal.Row).Specific).Checked = false;
            //}
        }
        #endregion

        #region ChooseFromListEvent
        ///<summary>    Choose from list after event. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        ///<param name="pObjValEvent"> The value event. </param>
        private void ChooseFromListAfterEvent(SAPbouiCOM.ItemEvent pObjValEvent)//Choosefromlist
        {

            SAPbouiCOM.IChooseFromListEvent lObjCFLEvento = null;
            lObjCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pObjValEvent;

            string lStrCFL_ID = null;
            lStrCFL_ID = lObjCFLEvento.ChooseFromListUID;

            SAPbouiCOM.DataTable lObjDataTable = null;
            lObjDataTable = lObjCFLEvento.SelectedObjects;

            try
            {
                if (lObjDataTable != null)
                {
                    string lStrvalChoose = System.Convert.ToString(lObjDataTable.GetValue(0, 0));
                    if (pObjValEvent.ItemUID == txtDoc.Item.UniqueID) //Busqueda de documento
                    {
                        ChooseDocument(pObjValEvent, lObjDataTable, lStrvalChoose);
                    }

                    //if (pObjValEvent.ItemUID == txtPro.Item.UniqueID)-------------------------project
                    //{
                    //    this.UIAPIRawForm.DataSources.UserDataSources.Item("UDS_Pro").ValueEx
                    //        = System.Convert.ToString(lObjoDataTable.GetValue(0, 0)); ;
                    //}

                    if (pObjValEvent.ItemUID == txtClSoc.Item.UniqueID)//Busqueda de socio de negocio
                    {
                        ChooseBussinesPartner(pObjValEvent, lObjDataTable, lStrvalChoose);
                    }
                    if (pObjValEvent.ItemUID == "mtxArtLst2") //Busqueda de articulo / almacen en matriz
                    {
                        ChooseSelectItem(pObjValEvent, lObjDataTable, lStrvalChoose);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("chooseFromListAfterEvent: {0}", ex.Message));
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format(" ChooseFromListException: {0}", ex.Message));
            }


            //mtxArtLst.SetCellFocus(0, 0);
        }

        private void ChooseDocument(ItemEvent pObjValEvent, SAPbouiCOM.DataTable pObjDataTable, string pStrValChoose)
        {
            this.UIAPIRawForm.DataSources.UserDataSources.Item("UDS").ValueEx =
                               System.Convert.ToString(pObjDataTable.GetValue(1, 0)); ;
            // LoadMatrixColumns(mStrSource);
            LoadMatrixData("DocEntry", pStrValChoose);
            mStrDocEntry = pStrValChoose;
            VerificarCheck();
        }

        private void ChooseBussinesPartner(ItemEvent pObjValEvent, SAPbouiCOM.DataTable pObjDataTable, string pStrValChoose)
        {
            txtDoc.Value = string.Empty;
            this.UIAPIRawForm.DataSources.UserDataSources.Item("UDSocio").ValueEx = pStrValChoose;
            txtNomSoc.Value = System.Convert.ToString(pObjDataTable.GetValue(1, 0));
            txtDoc.Item.Enabled = true;
            //txtDate.DataBind.UnBind();
            txtDoc.DataBind.SetBound(true, "", "UDS");
            InitChooseFromListBussinesPartner();
            SetParametersToChoose();
            //Rcordova UPDATE 
            //mDBDataSourceD.Clear();
            txtDoc.ChooseFromListUID = mStrTipoDoc;
            if (mObjMatrix.Columns.Count < 1)
            {
                LoadMatrixColumns(mStrSource);
            }
            if (cboTypTic.Value != "Venta de pesaje" && cboTypTic.Value != "Compra")
            {
                mDBDataSourceD.Clear();
                mDBDataSourceD = this.UIAPIRawForm.DataSources.DBDataSources.Item(mStrSource);
                mDBDataSourceD.InsertRecord(0);
                mDBDataSourceD.SetValue("ItemCode", 0, "");
                mObjMatrix.Columns.Item("ItemCode").Editable = true;
                mDBDataSourceD.SetValue("Quantity", pObjValEvent.Row - 1, "1");
                btnAddItem.Item.Visible = true;
            }
            if (cboTypTic.Value == "Venta de pesaje")
            {

                cbDocType.Select(1, BoSearchKey.psk_Index);
                mDBDataSourceD = this.UIAPIRawForm.DataSources.DBDataSources.Item(mStrSource);
                mDBDataSourceD = mObjMatrixUI.AddItemService(mDBDataSourceD);

                //txtDoc.Item.Enabled = false;
                mObjMatrix.LoadFromDataSource();
                (mObjMatrix.Columns.Item("PesoN").Cells.Item(1).Specific as EditText).Value = "0.000";


            }
            else
            {
                VerificarCheck();
                mObjMatrix.LoadFromDataSource();
            }
        }

        private void ChooseSelectItem(ItemEvent pObjValEvent, SAPbouiCOM.DataTable pObjDataTable, string pStrValChoose)
        {
            if (pObjValEvent.ColUID == "ItemCode")
            {
                SelectedItem(pStrValChoose, pObjValEvent, pObjDataTable.GetValue(1, 0).ToString());
            }
            if (pObjValEvent.ColUID == "WhsCode")
            {
                SelectedWareHouse(pStrValChoose, pObjValEvent);
            }
        }

        /// <summary>
        /// GetWhsCode
        /// Get whsCode by UserSignature
        /// <remarks>Rcordova 03-11-2017</remarks>
        /// </summary>
        /// <returns></returns>
        private string GetWhsCode()
        {
            string lStrUserSignature = DIApplication.Company.UserSignature.ToString();
            string lStrResult = mObjTicketDAO.GetWareHouse(lStrUserSignature);

            return lStrResult;
        }

        /// <summary>
        /// IsSetWhsCode
        /// Check ItemCode have attribute 31 (WhsCode)
        /// </summary>
        /// <param name="pStrValChoose"></param>
        /// <returns></returns>
        private bool IsSetWhsCode(string pStrValChoose)
        {
            bool lBolResul = true;
            string lStrResult = "N";

            lStrResult = mObjQueryManager.GetValue("QryGroup31", "ItemCode", pStrValChoose, "OITM");
            if (lStrResult == "Y")
            {
                return false;
            }
            return lBolResul;
        }

        private void SelectedWareHouse(string pStrValChoose, ItemEvent pObjValEvent)
        {
            TicketDAO lObjTicketDAO = new TicketDAO();
            string lStrSacos = ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Sacos").Cells.Item(mIntRow).Specific).Value;
            mDBDataSourceD.SetValue("U_GLO_BagsBales", pObjValEvent.Row - 1, lStrSacos);
            mDBDataSourceD.SetValue("WhsCode", pObjValEvent.Row - 1, pStrValChoose);
            mDBDataSourceD.SetValue("FromWhsCod", pObjValEvent.Row - 1, pStrValChoose);
            mObjMatrix.LoadFromDataSource();
        }

        private void SelectedItem(string pStrValChoose, ItemEvent pObjValEvent, string pStrDescription)
        {
            TicketDAO lObjTicketDAO = new TicketDAO();
            string lStrPrice = lObjTicketDAO.GetPrice(pStrValChoose);
            mDBDataSourceD = this.UIAPIRawForm.DataSources.DBDataSources.Item(mStrSource);
            mDBDataSourceD.SetValue("ItemCode", pObjValEvent.Row - 1, pStrValChoose);
            mDBDataSourceD.SetValue("Dscription", pObjValEvent.Row - 1, pStrDescription);// lObjoDataTable.GetValue(1, 0).ToString());
            mDBDataSourceD.SetValue("Price", pObjValEvent.Row - 1, Convert.ToDouble(lStrPrice, CultureInfo.GetCultureInfo("en-US")).ToString(CultureInfo.GetCultureInfo("en-US")));
            mDBDataSourceD.SetValue("Weight1", pObjValEvent.Row - 1, "0");
            mDBDataSourceD.SetValue("Weight2", pObjValEvent.Row - 1, "0");
            if (IsSetWhsCode(pStrValChoose))
            {
                mDBDataSourceD.SetValue("WhsCode", pObjValEvent.Row - 1, GetWhsCode());
            }
            else
            {
                mDBDataSourceD.SetValue("WhsCode", pObjValEvent.Row - 1, "");
            }
            mDBDataSourceD.SetValue("Quantity", pObjValEvent.Row - 1, "0");
            mDBDataSourceD.SetValue("LineTotal", pObjValEvent.Row - 1, "0");

            if (mObjMatrix.RowCount == pObjValEvent.Row)
            {
                mObjMatrix.AddRow(pObjValEvent.Row + 1);
                //mObjMatrix.Item.Refresh();
                mDBDataSourceD.InsertRecord(pObjValEvent.Row);
            }
            mDBDataSourceD.SetValue("ItemCode", pObjValEvent.Row, "");
            mObjMatrix.LoadFromDataSource();
        }

        #endregion

        #region ButtonEvents


        /// <summary>
        /// Coloca el peso obtenido 
        /// </summary>
        private void btnWT2_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (mBolIsUpdate)
                {
                    txtOutputWT.Value = mStrWeight;
                    //mStrWeight = mObjTicketServices.RandomNumber(1, 100).ToString();
                    CalcTotals();
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("[btnWT2_ClickBefore]: {0}", ex.Message));
                LogService.WriteError(ex);
            }


            // SBO_Application.SendKeys("")

        }

        /// <summary>
        /// Coloca el peso obtenido 
        /// </summary>
        private void btnWT1_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (string.IsNullOrEmpty(txtInWT.Value) || Convert.ToDouble(txtInWT.Value, CultureInfo.GetCultureInfo("en-US")) == 0)
                {
                    txtInWT.Value = mStrWeight;
                    //mStrWeight = mObjTicketServices.RandomNumber(1, 100).ToString();
                    CalcTotals();
                }
                else
                {
                    UIApplication.ShowMessageBox("Peso inicial ya registrado");
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("[btnWT1_ClickBefore]: {0}", ex.Message));
                LogService.WriteError(ex);
            }
        }


        private void btnClose_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.UIAPIRawForm.Freeze(true);
            try
            {
                mObjTicket = new Ticket();
                mObjTicket = GetTicketForm(txtFolio.Value);
                mObjTicket.Status = 0;
                if (mObjTicketServices.SaveTicket(mObjTicket, true))
                {
                    mBolIsUpdate = false;
                    ClearControls();
                    UIApplication.ShowMessageBox("Ticket cerrado correctamente!");
                }

            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("[btnClose_ClickBefore]: {0}", ex.Message));
                LogService.WriteError(ex);

            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }

        }
        private void btnPrint_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                //StartService();
                //GetRemoteObjectPrint();
                //// string pStrCode = mObjQueryManager.GetValue("Code", "U_Folio", pStrFolio, "[@UG_PL_TCKT]");
                string pStrCode = mObjQueryManager.GetValue("Code", "U_Folio", txtFolio.Value, "[@UG_PL_TCKT]");

                Ticket lObjTicket = new Ticket(); ;
                List<TicketDetail> lLstTicketDetail = null;
                //TicketDAO mObjTicketDAO = new TicketDAO();
                lObjTicket = mObjTicketDAO.GetTicket(pStrCode);
                lLstTicketDetail = mObjTicketDAO.GetListTicketDetail(txtFolio.Value).ToList();
                if (lObjTicket != null)
                {
                    List<string> lLstLines = GetListToPrint(lObjTicket, lLstTicketDetail);

                    bool lBolUpdate = false;
                    int lIntPrintedLine = 0;

                    mIntLastPrintLines = lObjTicket.PrintLine;

                    //if (lLstLines.Count != lObjTicket.PrintLine)
                    //{
                    //    lBolUpdate = true;
                    //    if (lLstLines.Count - lObjTicket.PrintLine > 0)
                    //    {
                    //        lIntPrintedLine = lObjTicket.PrintLine;
                    //    }
                    //}
                    //else
                    //{
                    //    if (SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("¿Desea imprimir el ticket completo o la impresion anterior?", 1, "Anterior", "Completo", "") == 1)
                    //    {
                    //        LogService.WriteInfo("Impresión anterior");
                    //        lBolUpdate = true;
                    //        lIntPrintedLine = Convert.ToInt16(lObjTicket.RowName);
                    //    }
                    //    else
                    //    {
                    //        LogService.WriteInfo("Imprimiendo TODO el ticket");
                    //    }
                    //}


                    PrintTicketReport(lLstLines, 1);

                    if (lLstLines.Count != lObjTicket.PrintLine)
                    {
                        lObjTicket.RowName = lObjTicket.PrintLine.ToString();
                    }
                    lObjTicket.PrintLine = lLstLines.Count();

                    //mObjTicketServices.SaveTicket(lObjTicket, true);

                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("[btnClose_ClickBefore]: {0}", ex.Message));
                LogService.WriteError(ex);
            }
        }


        /// <summary>
        /// Guarda el ticket
        /// </summary>
        private void btnSave_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            CultureInfo ci = new CultureInfo("es-MX");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;


            this.UIAPIRawForm.Freeze(true);
            for (int i = 1; i <= mObjMatrix.RowCount; i++)
            {
                MakeCalculation(i, "");
            }

            try
            {
                //var x = mObjQueryManager.Max();
                mObjTicket = new Ticket();

                if (mObjMatrix.RowCount > 0)
                {
                    CalcImport(cboTypTic.Value);
                    CalcTotals();
                }

                if (ValidateControls())
                {
                    string lStrFolio = txtFolio.Value;
                    if (!mBolIsUpdate)
                    {
                        lStrFolio = GetLastTicket().ToString();
                    }

                    //Obtiene el objeto ticket del formulario
                    mObjTicket = GetTicketForm(lStrFolio);
                    bool lBolPesada = false;

                    if (cboTypTic.Value == "Venta de pesaje" || cboTypTic.Value == "Pesaje")
                    {
                        lBolPesada = true;
                    }


                    //Obtiene las lineas de almatriz
                    List<TicketDetail> lLstTicketDetail = mObjCalculation.GetTicketDetailMatrix(lStrFolio, mObjMatrix, mBolIsUpdate, lBolPesada, mDBDataSourceD, mObjTicket.WTType == 1? true : false);

                    //Verifica si el item requiere numero de pacas
                    bool lBolVerify = true;
                    bool lBolWhCode = true;
                    string lStrItemBAG = "";
                    foreach (TicketDetail lObjTicketDet in lLstTicketDetail)
                    {
                        // si la linea no tiene algun peso (peso inicial o peso neto). La ignora.
                        if (lObjTicketDet.netWeight > 0)
                        {
                            lBolVerify = mObjValidations.VerifyBagBales(Convert.ToInt32(lObjTicketDet.BagsBales), lObjTicketDet.Item);
                        }
                        if (!lBolVerify && (lObjTicketDet.netWeight > 0 || lObjTicketDet.FirstWT > 0))
                        {

                            UIApplication.ShowMessageBox("Falta el registro de pacas-sacos en el artículo " + lStrItemBAG);
                            break;
                        }

                        if (string.IsNullOrEmpty(lObjTicketDet.WhsCode) && cboTypTic.Value != "Venta de pesaje" && cboTypTic.Value != "Pesaje")
                        {
                            lBolWhCode = false;
                            lStrItemBAG = lObjTicketDet.Item;
                            UIApplication.ShowMessageBox("Falta registrar el almacén en el artículo: " + lStrItemBAG);
                            break;
                        }

                    }
                    if (lLstTicketDetail.Count > 0)
                    {
                        bool lBoolNoDelivery;
                        int lIntPrintedLines = 0;
                        if (lBolVerify && lBolWhCode)
                        {
                            //Verifica si hay entregas hasta el momento(para no volver a generarla en pesajes dobles)
                            lBoolNoDelivery = WithoutDelivery();


                            lIntPrintedLines = mObjTicket.PrintLine;

                            if (mObjTicketServices.SaveTicket(mObjTicket, mBolIsUpdate)
                                && mObjTicketServices.SaveTicketDetail(lLstTicketDetail, mBolIsUpdate)
                                && mObjTicketServices.RemoveTicketDetail(lLstTicketDetail))
                            {
                                //Validacion para generar entregas en caso de ser venta de pesaje por medio de facturas de reserva
                                if (WithoutInvoice() != string.Empty && !lBoolNoDelivery)
                                {
                                    List<Ticket> lLstTickets = new List<Ticket>();

                                    string lStrcode = mObjQueryManager.GetValue("Code", "U_Folio", mObjTicket.Folio, "[@UG_PL_TCKT]");
                                    mObjTicket = mObjTicketDAO.GetTicket(lStrcode);
                                    lLstTickets.Add(mObjTicket);

                                    mObjTicketDocCreation.CrearDocumento(lLstTickets, SAPbobsCOM.BoObjectTypes.oDeliveryNotes, "OINV", 13, "INV1");
                                }
                              
                                if (Print(mObjTicket, lLstTicketDetail))
                                {
                                    //mObjTicket.RowName = mObjTicket.PrintLine.ToString();
                                    //mObjTicket.PrintLine = lLstLines.Count();

                                    //mObjTicketServices.SaveTicket(mObjTicket, true);
                                    mBolIsUpdate = true;
                                    //ClearControls();
                                    this.UIAPIRawForm.Freeze(false);
                                    UIApplication.ShowMessageBox("Ticket guardado correctamente!");
                                }

                                btnPrint.Item.Visible = true;
                            }
                        }

                        mBolLoadingT = false;
                    }
                    else
                    {
                        this.UIAPIRawForm.Freeze(false);
                        UIApplication.ShowMessageBox("No es posible guardar el ticket sin ningun articulo ");
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("Error al verificar los datos: {0}", ex.Message));
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(string.Format("Error al verificar los datos: {0}", ex.Message));
                this.UIAPIRawForm.Freeze(false);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }
        #region Print reacomodar
        //private bool Print(List<string> pLstLines, int pIntPrintedLines, bool pBolIsUpdate, int pIntStatus)
        private bool Print(Ticket pObjTicket, List<TicketDetail> pLstTicketDetail)
        {
            try
            {
                string lStrCopies = mObjQueryManager.GetValue("U_Value", "Name", "PL_PRINT_COPIES", "[@UG_Config]");
                int lIntCopies = string.IsNullOrEmpty(lStrCopies) ? 1 : int.Parse(lStrCopies);
                List<string> lLstLines = GetListToPrint(pObjTicket, pLstTicketDetail);
                //        {
                if (mBolIsUpdate)
                {
                    if ((pObjTicket.Status == (int)TicketEnum.TicketStatus.Pending || pObjTicket.Status == (int)TicketEnum.TicketStatus.Close))
                    {

                        PrintTicketReport(lLstLines, lIntCopies);

                    }
                }
                else
                {
                    //Impresion por tarimera
                    if (pLstTicketDetail.Where(x => x.WeighingM == 1).Count() == 0)
                    {
                        if (pObjTicket.PrintLine == 0)
                        {
                            lIntCopies = 1;
                        }
                        PrintTicketReport(lLstLines, lIntCopies);
                    }

                    //PrintTicketPort()
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowError("Imprimir: " + ex.Message);
                LogService.WriteError("Imprimir: " + ex.Message);
                LogService.WriteError(ex);
                return false;
            }
            return true;
        }


        private List<string> GetListToPrint(Ticket pObjTicket, List<TicketDetail> pLstTicketDetail)
        {
            List<string> lLstLine = new List<string>();
            lLstLine.Add("Folio: " + pObjTicket.Folio);
            //lLstLine.Add("UNION GANADERA REGIONAL DE SONORA");
            //TicketEntrada/Salida
            string lStrEntradaSalida = "";
            if((pObjTicket.Status == (int)TicketEnum.TicketStatus.Pending || pObjTicket.Status == (int)TicketEnum.TicketStatus.Close))
            {
                lStrEntradaSalida = "Salida";
            }
            else
            {
                lStrEntradaSalida = "Entrada";
            }
            lLstLine.Add("Ticket " + lStrEntradaSalida);
            if (pObjTicket.CapType == 2 || pObjTicket.CapType == 3)
            {
                lLstLine.Add("Destino: "+ GetWhs(pObjTicket));
            }
            else
            {
                lLstLine.Add("Código: " + pObjTicket.BPCode);
                string lStrCliProv = string.Empty;
                if (pObjTicket.CapType == 1) // 1 compra
                {
                    lStrCliProv = "Proveedor: ";
                }
                else
                {
                    lStrCliProv = "Cliente: ";
                }
                lLstLine.Add(lStrCliProv + mObjTicketServices.SearchBPName(pObjTicket.BPCode));
            }
            lLstLine.Add("Chofer: " + pObjTicket.Driver);
            lLstLine.Add("Placas: " + pObjTicket.CarTag);
            DateTime lDtmDateEntry = mObjTicketServices.GetDateTime(pObjTicket.EntryDate, pLstTicketDetail[0].EntryTime.ToString());
            lLstLine.Add("Fecha: " + lDtmDateEntry.ToString("dd/MM/yyyy HH:mm"));
            // lLstLine.Add("Peso Inicial: " + pObjTicket.InputWT);

            int i = 0;
            foreach (TicketDetail lObjTicketDetail in pLstTicketDetail.OrderBy(x => x.Line))
            {
                
                i++;
                //if ((!string.IsNullOrEmpty(lObjTicketDetail.FirstWT.ToString()) && lObjTicketDetail.FirstWT > 0) || 
                //    (!string.IsNullOrEmpty(lObjTicketDetail.netWeight.ToString()) && lObjTicketDetail.netWeight > 0) || lObjTicketDetail.WeighingM == 1)
                //{
                //  //lLstLine.Add("Cod. Prod: " + lObjTicketDetail.Item);
                lLstLine.Add(string.Format("{0} {1} {2}", i.ToString("00"), "Prod:", mObjTicketServices.SearchItemName(lObjTicketDetail.Item)));
                //}
             
            }
          

            //if (!string.IsNullOrEmpty(pObjTicket.OutputWT.ToString()) && pObjTicket.OutputWT > 0)
            //{
            //    lLstLine.Add("Peso salida: " + pObjTicket.OutputWT.ToString());
            //}
            int j = 0;
            foreach (TicketDetail lObjTicketDetail in pLstTicketDetail)
            {
                //lLstLine.Add("");
                j++;
                if (!string.IsNullOrEmpty(lObjTicketDetail.FirstWT.ToString(CultureInfo.GetCultureInfo("en-US"))) && lObjTicketDetail.FirstWT > 0)
                {
                    lLstLine.Add(j.ToString("00") + " Peso Ent: " + lObjTicketDetail.FirstWT);
                }
                if (!string.IsNullOrEmpty(lObjTicketDetail.SecondWT.ToString(CultureInfo.GetCultureInfo("en-US"))) && lObjTicketDetail.SecondWT > 0)
                {
                    lLstLine.Add(j.ToString("00") + " Peso Sal: " + lObjTicketDetail.SecondWT);
                }
                if (!string.IsNullOrEmpty(lObjTicketDetail.netWeight.ToString(CultureInfo.GetCultureInfo("en-US"))) && lObjTicketDetail.netWeight > 0)
                {
                    lLstLine.Add(j.ToString("00") + " Peso Neto: " + lObjTicketDetail.netWeight);
                }
            }

            if ((pObjTicket.Status == (int)TicketEnum.TicketStatus.Pending || pObjTicket.Status == (int)TicketEnum.TicketStatus.Close))
            {
                DateTime lDtmDateOutput = mObjTicketServices.GetDateTime(pLstTicketDetail[pLstTicketDetail.Count - 1].OutputDate, pLstTicketDetail[pLstTicketDetail.Count - 1].OutputTime.ToString());
                lLstLine.Add("Fecha cierre: " + lDtmDateOutput.ToString("dd/MM/yyyy HH:mm"));
                if (!string.IsNullOrEmpty(pObjTicket.Coments))
                {
                    lLstLine.Add("Comentario: \n" + pObjTicket.Coments);
                }
            }

            if ((pObjTicket.Status == (int)TicketEnum.TicketStatus.Pending || pObjTicket.Status == (int)TicketEnum.TicketStatus.Close) || pObjTicket.CapType == 4 || pObjTicket.CapType == 5)
            {
                // ;)//Salida
                lLstLine.Add("");
                lLstLine.Add("");
                lLstLine.Add("____________________________________");
                lLstLine.Add("    ENTREGO                 ENTREGO ");
                lLstLine.Add("    CLIENTE                   PORTERO ");
                lLstLine.Add("");

            }
            else
            {
                lLstLine.Add("");
                lLstLine.Add("      Firma                             Sacos/Pacas");

                int lIntI = 0;
                foreach (TicketDetail lObjTicketDetail in pLstTicketDetail.OrderBy(x => x.Line))
                {
                    lIntI++;
                   
                    lLstLine.Add("");
                    lLstLine.Add(string.Format("{0} {1}  {2}", lIntI.ToString("00"), "____________________", "________"));
                    lLstLine.Add("");
                   
                }
            }
           
            //if (!string.IsNullOrEmpty(lObjTicket.OutputWT.ToString()) && lObjTicket.OutputWT > 0)
            //{
            //  //  lLstLine.Add("Peso Final: " + lObjTicket.OutputWT);
            //}
            //mObjTicketServices.print(pObjTicket, pLstTicketDetail);
            return lLstLine;
        }

        private void PrintTicketLog(List<string> pLstLine)
        {
            LogService.WriteSuccess("********* Impresion de ticket **********");
            foreach (string lStrLine in pLstLine)
            {
                string lStrValue = Truncate(lStrLine, 35);
                //mObjWritePort.WriteSerialPort(lStrValue);
                LogService.WriteInfo(lStrValue);
            }
        }

        private void PrintTicketReport(List<string> pLstLine, int pIntCopies)
        {
            PrintTicketLog(pLstLine);

            //int lIntCopies = string.IsNullOrEmpty(lStrCopies) ? 1 : int.Parse(lStrCopies);



            //for (int i = 0; i < lIntCopies; i++)
            //{
            //string lStrCopies = mObjQueryManager.GetValue("U_Value", "Name", "PL_PRINT_COPIES", "[@UG_Config]");


            string lStrPrinterName = mObjQueryManager.GetValue("U_Value", "Name", "PL_PRINTER_NAME", "[@UG_Config]");
            if (string.IsNullOrEmpty(lStrPrinterName))
            {
                throw new Exception("No se encontró valor para la cofiguración PL_PRINTER_NAME en la tabla de configuraciones, agreguela para continuar");
            }

            LocalReport lObjLocalReport = new LocalReport();
            lObjLocalReport.ReportPath = @"Reports/Tickets/rptTicket.rdlc";

            lObjLocalReport.SetParameters(new ReportParameter("Value", string.Join("|", pLstLine)));
            lObjLocalReport.Refresh();

            LogService.WriteSuccess("Reporte generado");

            CultureInfo culture = CultureInfo.CurrentCulture;
            LogService.WriteInfo(string.Format("The current culture before print is {0} [{1}]",
                        culture.NativeName, culture.Name));

            
            PinterUtility.PrintReport(lObjLocalReport, lStrPrinterName);

             culture = CultureInfo.CurrentCulture;
            LogService.WriteInfo(string.Format("The current culture after print is {0} [{1}]",
                        culture.NativeName, culture.Name));


            CultureInfo ci = new CultureInfo("es-MX");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            culture = CultureInfo.CurrentCulture;
            LogService.WriteInfo(string.Format("Change th current culture to {0} [{1}]",
                 culture.NativeName, culture.Name));

            //using (var doc = new ReportPrintDocument(lObjLocalReport))
            //{

            //    doc.PrinterSettings.PrinterName = lStrPrinterName;
            //    for (int i = 0; i < pIntCopies; i++)
            //    {
            //        doc.Print();
            //    }
            //}
        }

        private string Truncate(string pStrValue, int pIntMaxLength)
        {
            if (string.IsNullOrEmpty(pStrValue)) return pStrValue;
            return pStrValue.Length <= pIntMaxLength ? pStrValue : pStrValue.Substring(0, pIntMaxLength);
        }


        private void PrintUpdate(List<string> pLstLine, int pIntPrintedLine)
        {

            LogService.WriteSuccess("********* Impresion de actualizacion **********");
            List<string> lLstLineToPrint = new List<string>();
            for (int i = 1; i <= pLstLine.Count; i++)
            {
                if (i > pIntPrintedLine)
                {
                    //mObjWritePort.WriteSerialPort(pLstLine[i - 1]);
                    LogService.WriteInfo(pLstLine[i - 1]);
                }
                else
                {
                    //mObjWritePort.WriteSerialPort("");
                    LogService.WriteInfo("");
                }
            }
        }

        #endregion


        #region Luis Validaciones Venta pesaje (reacomodar) (Aun no lo reacomoda :|)
        /// <summary>
        ///Validar si hay entregas (venta de pesaje)
        /// </summary>
        /// <returns></returns>
        private bool WithoutDelivery()
        {
            if (cboTypTic.Value.Equals("Venta de pesaje") && cbDocType.Value.Equals("Factura de reserva"))
            {
                return CheckDelivery();
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// Validar si hay entregas (venta de pesaje)
        /// </summary>
        /// <returns></returns>
        private bool CheckDelivery()
        {
            int lIntFolio = Convert.ToInt32(txtFolio.Value);
            if (!mBolIsUpdate)
            {
                lIntFolio = GetLastTicket();
            }

            return mObjTicketServices.GetLastDelivery(lIntFolio);//ticket activo
        }

        /// <summary>
        /// Validar si tiene alguna factura (venta de pesaje con factura de reserva)
        /// </summary>
        /// <returns></returns>
        private string WithoutInvoice()
        {
            if (cboTypTic.Value.Equals("Venta de pesaje") && cbDocType.Value.Equals("Factura de reserva"))
            {
                return mObjTicket.Number.ToString();// CheckInvoice();

            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// Validar si tiene alguna factura (venta de pesaje con factura de reserva)
        /// </summary>
        /// <returns></returns>
        private string CheckInvoice()
        {
            return mObjQueryManager.GetValue("DocEntry", "DocEntry", mObjTicket.Number.ToString(), "OINV"); //Cambio de DocNum por DocEntry 17/01/19
            //return mObjQueryManager.GetValue("DocEntry", "DocNum", lObjTicket.Number.ToString(), "OINV"); Cambio de DocNum por DocEntry 17/01/19
        }

        #endregion


        /// <summary>
        /// Boton get bascula
        /// </summary>
        private void btnWeight_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            string lStrWeight = "0";
            try
            {
                Form lObjForm = UIApplication.GetApplication().Forms.ActiveForm;
                //Simulacion de bascula eliminar
                string lStrSim = string.Empty;
                this.txtSim = ((SAPbouiCOM.EditText)(lObjForm.Items.Item("txtSim").Specific));
                if (!string.IsNullOrEmpty(txtSim.Value))
                {
                    lStrSim = txtSim.Value;
                    LogService.WriteInfo(string.Format("Peso asignado por el usuario:{0}", lStrSim));
                    if (lStrSim.Length < 9 && Convert.ToDecimal(lStrSim) > 0)
                    {
                        lStrWeight = lStrSim;
                        mStrSimWeight = lStrSim;
                    }
                }
                else
                {
                    lStrWeight = lblWeight.Caption;
                    mStrSimWeight = string.Empty;

                }

                LogService.WriteInfo("Dato de bascula " + lStrWeight);
                if (!string.IsNullOrEmpty(lStrWeight) && lStrWeight.Length < 9)
                {
                    // RCordova UPDATE 31-10-2017 Ticket #71 
                    if (mIntRow == 0)
                    {
                        UIApplication.ShowMessageBox("No se puede asignar peso si no se tiene seleccionado un articulo");
                    }
                    else if (!btnWeight.Item.Enabled && ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("ItemCode").Cells.Item(mIntRow).Specific).Value == ""
                            && (((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("PesoN").Cells.Item(mIntRow).Specific).Value == "0.0"))
                    {
                        UIApplication.ShowMessageBox("No se puede asignar peso si no se tiene seleccionado un articulo");
                    }
                    else
                    {

                        bool lBolCheck = false;


                        if (mObjMatrix.RowCount > 0 && mStrColumn != null && cboTypTic.Value != "Venta de pesaje")
                        {
                            lBolCheck = (mObjMatrix.Columns.Item("Check").Cells.Item(mIntRow).Specific as CheckBox).Checked;
                        }
                        if (mObjMatrix.RowCount > 0 && mStrColumn != null && mObjMatrix.IsRowSelected(mIntRow) && !lBolCheck
                            && mObjCalculation.VerifyWeightSecuence(lStrWeight, mStrSource, mObjMatrix, cboTypTic.Value))
                        {
                            if (((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Peso1").Cells.Item(mIntRow).Specific).Value == "0.0")
                            {
                                if (mObjValidations.VerificarSegundoPeso(mObjMatrix))
                                {
                                    ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Peso1").Cells.Item(mIntRow).Specific).Value = lStrWeight;
                                    mStrLastPeso = lStrWeight;//string.Empty;
                                }
                            }
                            else
                            {
                                //if ((Convert.ToDouble(((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Price").Cells.Item(mIntRow).Specific).Value, CultureInfo.GetCultureInfo("en-US")) != 0)
                                //    || (mStrTipoDoc == "CFL_Traslado"))
                                //{
                                if (cbWTType.Value == "Doble")//((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Peso2").Cells.Item(mIntRow).Specific).Value == "0.0")
                                {
                                    //string lStrBaseDoc = txtDoc.Value;
                                    //if (WeightInvoice(lStrBaseDoc)) /// Valida si hay factura para concluir el proceso del pesaje
                                    //{
                                    ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Peso2").Cells.Item(mIntRow).Specific).Value = lStrWeight;
                                    mStrLastPeso = lStrWeight;
                                    //}
                                    //else
                                    //{
                                    //    SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText("Favor de facturar ara continuar"
                                    //    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    //}


                                }
                                //}
                                //else
                                //{
                                //    lBoolPrice = false;
                                //}
                            }
                            // mObjMatrix.Columns.Item("Price").Editable = true;

                            //if (lBoolPrice)
                            //{
                            if (string.IsNullOrEmpty(txtInWT.Value) || Convert.ToDouble(txtInWT.Value, CultureInfo.GetCultureInfo("en-US")) == 0)
                            {
                                txtInWT.Value = lStrWeight;
                            }

                            int lIntSelectedRow = mObjMatrix.GetNextSelectedRow(0, SAPbouiCOM.BoOrderType.ot_RowOrder);
                            if (lIntSelectedRow == mObjMatrix.RowCount)
                            {

                            }
                            CalcImport(cboTypTic.Value);
                            if (mObjValidations.IsLastWeight(mObjMatrix))
                            {
                                txtOutputWT.Value = lStrWeight;
                            }
                            mObjMatrix.Item.Refresh();
                            //}
                            //else
                            //{
                            //    SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText("Agregar el precio del artículo para continuar"
                            //  , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            //}

                        }

                        //mStrWeight = mObjTicketServices.RandomNumber(1, 100).ToString();
                        //lblWeight.Caption = mStrWeight;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[btnWeight_ClickBefore]: " + ex.Message);
                LogService.WriteError(ex);
            }



        }

        /// <summary>
        /// Buscar factura dependiendo del ticket
        /// </summary>
        /// <param name="pStrBaseDoc"></param>
        /// <returns></returns>
        private bool WeightInvoice(string pStrBaseDoc)
        {
            int lIntFolio = Convert.ToInt32(txtFolio.Value);
            if (!mBolIsUpdate)
            {
                lIntFolio = GetLastTicket();
            }

            if (cboTypTic.Value.Equals("Venta de pesaje") && !SearchInvoiceDoc(pStrBaseDoc))
            {
                return mObjTicketServices.GetLastInvoice(lIntFolio);
            }
            else
            {
                return true;
            }
        }

        private bool SearchInvoiceDoc(string pStrDocEntry)
        {
            return mObjTicketServices.GetInvDocument((pStrDocEntry));
        }
        #endregion

        #endregion

        #region Methods

        #region Form

        ///<summary>    load application events. </summary>
        ///<remarks>  </remarks>
        public void LoadApplicationEvents()
        {
            //Bascula
            try
            {

                StartService();
                GetRemoteObject();//Servicio de bascula
                //GetRemoteObjectPrint();
                //mObjInternalWorker = new Thread(GetRemoteObject);
                //mObjInternalWorker.Start();
                // mBolConnected = GetRemoteObject(); //Servicio de bascula
                UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
            }
            catch (Exception ex)
            {
                LogService.WriteError("[LoadApplicationEvents]: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        ///<summary>    Unload application events. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        public void UnloadApplicationEvents()
        {
            //Application

            try
            {
                /*mObjWrapperObject.WrapperDataReceived -= new WeighingMachineEventHandler(OnDataReceived);
                LogService.WriteInfo("WeighingMachineEventHandler(OnDataReceived);");*/

                //Bascula
                /*HttpChannel lObjChannel = new HttpChannel();
                ChannelServices.UnregisterChannel(lObjChannel);*/
                mObjWrapperObject.WrapperDataReceived -= new WeighingMachineEventHandler(OnDataReceived);
                mObjWeighingMachine.DataReceived -= new WeighingMachineEventHandler(mObjWrapperObject.WrapperOnDataReceived);


                mObjWeighingMachine.Disconnect(mObjConnection);
                LogService.WriteInfo("Disconnect(mObjConnection)");
                //mObjWritePort.Disconnect(mObjConnectionPrinter);
                LogService.WriteInfo("Disconnect(mObjConnectionPrinter)");

                UIApplication.GetApplication().ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);

                /* mObjWritePort.DisconnectAll();
                 mObjWeighingMachine.DisconnectAll();
			  
                 LogService.WriteInfo("DisconnectAll()");*/



            }
            catch (Exception lObjException)
            {
                LogService.WriteError("[UnloadApplicationEvents]: " + lObjException.Message);
                LogService.WriteError(lObjException);
                UIApplication.ShowMessage(string.Format("Servicio de bascula: {0}", lObjException.Message));
                // MessageBox.Show(ex.Message);
            }
        }

        private void GetTime(string lStrmesage)
        {
            TimeSpan lTmsTime = DateTime.Now - mDblTime;
            LogService.WriteInfo(lTmsTime.Seconds + "." + lTmsTime.Milliseconds + " Segundos " + lStrmesage);
        }

        ///<summary>    Cargar el formulario con los ticket. </summary>
        ///<remarks>    </remarks>
        private void LoadTicket(Ticket pObjTicket, IList<TicketDetail> pLstTicketDetail)
        {
            mDblTime = DateTime.Now;
            //OnInitializeComponent();
            try
            {
                this.UIAPIRawForm.Freeze(true);
                mBolLoadingT = true;

                cboTypTic.Item.Enabled = true;
                cboTypTic.Select(pObjTicket.CapType + 1, BoSearchKey.psk_Index);


                mObjMatrix.Item.Enabled = true;
                btnWeight.Item.Enabled = true;
                txtClSoc.Item.Enabled = false;

                btnPrint.Item.Visible = true;
                btnSave.Caption = "Guardar";
                cbDocType.Select(pObjTicket.DocType, BoSearchKey.psk_Index);
                txtDriver.Value = pObjTicket.Driver;
                txtCarTag.Value = pObjTicket.CarTag;
                txtComents.Value = pObjTicket.Coments;
                DateTime dt = DateTime.ParseExact(pObjTicket.EntryDate.ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.GetCultureInfo("en-US"));
                txtDate.Value = dt.ToString("dd/MM/yyyy");
                cboTypTic.Item.Enabled = false;
                //cbWTType.Value == "Doble";

                // txtPro.Value = pObjTicket.Project;
                if (pObjTicket.EntryDate > pObjTicket.OutputDate)
                {
                    txtDateOut.Value = "";
                }
                else
                {
                    txtDateOut.Value = pObjTicket.OutputDate.ToString("yyyyMMdd");
                }
                if (pObjTicket.Folio == null)
                {
                    //txtDate.Value = DateTime.Now.ToString("yyyyMMdd");
                    txtDate.Value = DateTime.Now.ToString("dd/MM/yyyy");
                    txtDateOut.Value = "";
                    pObjTicket.Folio = GetLastTicket().ToString();
                    pObjTicket.Status = 1;
                }

                txtPesoN.Value = (pObjTicket.InputWT - pObjTicket.OutputWT).ToString(CultureInfo.GetCultureInfo("en-US"));
                txtAmount.Value = pObjTicket.Amount.ToString(CultureInfo.GetCultureInfo("en-US"));
                this.UIAPIRawForm.DataSources.UserDataSources.Item("UDS_Pro").ValueEx = pObjTicket.Project;
                txtTara.Value = string.Empty;
                txtInWT.Value = pObjTicket.InputWT.ToString(CultureInfo.GetCultureInfo("en-US"));
                txtOutputWT.Value = pObjTicket.OutputWT.ToString(CultureInfo.GetCultureInfo("en-US"));
                txtFolio.Value = pObjTicket.Folio;
                mStrFolio = pObjTicket.Folio;

                switch (pObjTicket.Status)
                {
                    case 0:
                        txtStatus.Value = "Cerrado";
                        break;
                    case 1:
                        txtStatus.Value = "Abierto";
                        break;
                    case 2:
                        txtStatus.Value = "Pendiente";
                        break;
                }

                //  lObjParnerMap.Type = (MappingTypeEnum)Enum.Parse(typeof(MappingTypeEnum), "EXISTING");

                ChangeTypeTicket(pObjTicket.CapType);
                SetParametersToChoose();
                mStrLastPeso = "0";
                LoadMatrixColumns(mStrSource);
                this.UIAPIRawForm.DataSources.UserDataSources.Item("UDSocio").ValueEx = pObjTicket.BPCode;
                //txtDoc.Item.Enabled = true;f
                //txtDoc.Value = pObjTicket.Number.ToString();
                this.UIAPIRawForm.DataSources.UserDataSources.Item("UDS").ValueEx = GetDocNumByDocEntry(pObjTicket.Number.ToString()); // pObjTicket.Number.ToString(); 
                mStrDocEntry = pObjTicket.Number.ToString();
                txtNomSoc.Value = mObjTicketServices.SearchBPName(pObjTicket.BPCode);
                mDBDataSourceD = this.UIAPIRawForm.DataSources.DBDataSources.Item(mStrSource);
                mDBDataSourceD.Clear();

                if (pLstTicketDetail != null)
                {
                    mDBDataSourceD = mObjMatrixUI.LoadMatrixData(pObjTicket, mDBDataSourceD, pLstTicketDetail, mStrSource);//  LoadMatrixData(pObjTicket, pLstTicketDetail);
                }
                mBolIsUpdate = true;



                ///Validacion para calcular la linea al editar
                if (mObjMatrix.RowCount > 0)
                {

                    for (int i = 1; i <= mObjMatrix.RowCount; i++)
                    {
                        mObjMatrix.SelectRow(i, true, false);
                        int lIntSelectedRow = mObjMatrix.GetNextSelectedRow(0, SAPbouiCOM.BoOrderType.ot_RowOrder);
                        mIntRow = lIntSelectedRow;

                        CalcImport(cboTypTic.Value);
                    }
                    CalcTotals();
                }
                mObjMatrix.LoadFromDataSource();
                mDBDataSourceD = mObjMatrixUI.SetCheckbox(mObjMatrix, pLstTicketDetail, mDBDataSourceD);
                mObjMatrix.AutoResizeColumns();
                mObjMatrix.LoadFromDataSource();

                if (cboTypTic.Value != "Venta de pesaje")
                {
                    btnAddItem.Item.Visible = true;
                    // txtDoc.Item.Enabled = true;
                }
                else
                {
                    DateTime lDtmDateEntry = mObjTicketServices.GetDateTime(pLstTicketDetail[0].EntryDate, pLstTicketDetail[0].EntryTime.ToString());
                    if (pObjTicket.WTType == 0 && pObjTicket.Status == 1)
                    {
                        VerifyToleranceDay(lDtmDateEntry);
                    }
                    txtDate.Value = lDtmDateEntry.ToString("dd/MM/yyyy HH:mm");
                    btnAddItem.Item.Visible = false;
                    cbWTType.Item.Enabled = true;

                    // txtDoc.Item.Enabled = false;
                }

                if (pObjTicket.WTType == 0)
                {
                    cbWTType.Item.Enabled = false;
                    cbWTType.Select(0, BoSearchKey.psk_Index);
                    //cbWTType.Item.Enabled = true;
                }
                else
                {
                    cbWTType.Select(1, BoSearchKey.psk_Index);
                }
                this.UIAPIRawForm.Freeze(false);
                GetTime("");
                CalcTotals();
            }
            catch (Exception ex)
            {
                LogService.WriteError("[LoadTicket]: " + ex.Message);
                LogService.WriteError(ex);
                this.UIAPIRawForm.Freeze(false);

                UIApplication.ShowMessageBox("No es posible cargar un ticket " + ex.Message);
            }
            finally
            {
                mBolLoadingT = false;
                this.UIAPIRawForm.Freeze(false);
            }
        }

        //Verifica la tolerancia
        private void VerifyToleranceDay(DateTime pDtmDateEntry)
        {
            DateTime lDtmToleranceDay = mObjTicketServices.GetToleranceDay(pDtmDateEntry);
            List<string> lLstDateTime = mObjTicketServices.GetServerDatetime();
            DateTime lDtmNow = Convert.ToDateTime(lLstDateTime[0]);
            lDtmNow = mObjTicketServices.GetDateTime(lDtmNow, lLstDateTime[1].Replace(":", ""));

            if (lDtmNow > lDtmToleranceDay)
            {
                UIApplication.ShowMessageBox("El período de tolerancia a expirado");
                mObjMatrix.Item.Enabled = false;
                btnWeight.Item.Enabled = false;
                btnSave.Item.Visible = false;
                btnClose.Item.Visible = true;
            }

        }

        /// <summary>
        /// limpia los controles del formulario
        /// </summary>
        private void ClearControls()
        {
            try
            {


                txtClSoc.Value = string.Empty;
                txtNomSoc.Value = string.Empty;
                txtDoc.Value = string.Empty;

                //txtPro.Value = string.Empty;

                txtDriver.Value = string.Empty;
                txtCarTag.Value = string.Empty;

                txtPesoN.Value = string.Empty;
                txtPesoB.Value = string.Empty;
                txtTara.Value = string.Empty;
                txtAmount.Value = "0";
                txtInWT.Value = string.Empty;
                txtOutputWT.Value = string.Empty;
                txtComents.Value = string.Empty;
                mObjMatrix.Clear();
                mDBDataSourceD.Clear();
                txtStatus.Value = "Abierto";
                txtFolio.Value = GetLastTicket().ToString();
                mObjMatrix.LoadFromDataSource();

                txtFolio.Value = GetLastTicket().ToString();
                mStrLastPeso = "";
                txtVari.Value = "";
                btnDelete.Item.Visible = false;
                btnAddItem.Item.Visible = false;
                btnPrint.Item.Visible = false;
            }
            catch (Exception ex)
            {
                LogService.WriteError("[ClearControls]: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        #region Gets

        /// <summary>
        /// Obtiene los datos de ticket del formuario
        /// </summary>
        private Ticket GetTicketForm(string pStrFolio)
        {
            Ticket lObjTicket = new Ticket();
            try
            {
                LogService.WriteInfo("Guardando un Ticket " + pStrFolio);
                LogService.WriteInfo("txtOutput " + txtOutputWT.Value);
                if (mObjValidations.IsLastWeight(mObjMatrix) && txtOutputWT.Value != "" && Convert.ToDouble(txtOutputWT.Value, CultureInfo.GetCultureInfo("en-US")) != 0)
                {
                    //string lStrBaseDoc = txtDoc.Value;//Cambio DocEntry por DocNum 17/01/19
                    string lStrBaseDoc = mStrDocEntry;
                    if (cboTypTic.Value.Equals("Venta de pesaje") && WeightInvoice(lStrBaseDoc))// validacion para cerrar el ticket al hacer la segunda pesada (Venta de pesaje)
                    {
                        lObjTicket.Status = 0;
                    }
                    else
                    {
                        lObjTicket.Status = 2;
                    }
                }
                else
                {
                    string lStrBaseLine = mDBDataSourceD.GetValue("LineNum", mIntRow - 1);
                    if (cboTypTic.Value.Equals("Venta de pesaje") && lStrBaseLine != "" && cbWTType.Value.Equals("Simple")) //Valida si la venta de pesaje simple viene de un documento 
                    {
                        lObjTicket.Status = 0;
                    }
                    else
                    {                        // UPDATE RCordova -20/10/2017-
                        // Planta de Alimentos - Error pesaje simple y doble ( Ticket 124-125 )
                        if (cbWTType.Value.Equals("Simple"))
                        {
                            lObjTicket.Status = 2;
                        }
                        else
                        {
                            lObjTicket.Status = 1;
                        }
                    }
                }




                LogService.WriteInfo("txtDoc " + txtDoc.Value);
                LogService.WriteInfo("mStrDocEntry" + mStrDocEntry);
                if (!string.IsNullOrEmpty(txtDoc.Value))
                {
                    lObjTicket.Number = Convert.ToInt32(mStrDocEntry); ;
                }

                lObjTicket.BPCode = txtClSoc.Value;
                lObjTicket.Folio = pStrFolio;

                lObjTicket.CapType = GetCapType(cboTypTic.Value);
                lObjTicket.DocType = GetDocType(cbDocType.Value);

                lObjTicket.CarTag = txtCarTag.Value;
                lObjTicket.Driver = txtDriver.Value;

                //lObjTicket.Project = txtPro.Value;-------------------------------------------project

                List<string> lLstDateTime = mObjTicketServices.GetServerDatetime();
                LogService.WriteInfo("lLstDateTime " + lLstDateTime[0].ToString());
                lObjTicket.EntryDate = Convert.ToDateTime(lLstDateTime[0]);
                lObjTicket.WTType = GetDocWT(cbWTType.Value);
                LogService.WriteInfo("EntryDate " + lObjTicket.EntryDate.ToString());

                lObjTicket.InputWT = Convert.ToDouble(txtInWT.Value, CultureInfo.GetCultureInfo("en-US"));
                lObjTicket.OutputWT = Convert.ToDouble(txtOutputWT.Value, CultureInfo.GetCultureInfo("en-US"));
                string lStrAmount = txtAmount.Value;
                LogService.WriteInfo("Amount " + lStrAmount);
                //lStrAmount = lStrAmount.Substring(1, lStrAmount.Length - 1);
                lObjTicket.Amount = Convert.ToDouble(lStrAmount, CultureInfo.GetCultureInfo("en-US"));
                //lObjTicket.Amount = Convert.ToDouble(txtAmount.Value, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"));
                LogService.WriteInfo("Amount " + lObjTicket.Amount);
                lObjTicket.Coments = txtComents.Value;

                try
                {
                    if (mBolIsUpdate)
                    {
                        lObjTicket.PrintLine = Convert.ToInt32(mObjQueryManager.GetValue("U_PrintLine", "U_Folio", pStrFolio, "[@UG_PL_TCKT]"));
                    }
                }
                catch (Exception)
                {

                    lObjTicket.PrintLine = 0;
                }
               

            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                UIApplication.ShowMessageBox(string.Format("Error al verificar los datos: {0} ", ex.Message + " " + DIApplication.Company.GetLastErrorDescription()));
                LogService.WriteInfo("Amount " + lObjTicket.Amount);
                LogService.WriteError("[GetTicketForm]: " + ex.Message);
                LogService.WriteError(ex);
            }

            return lObjTicket;
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

        /// <summary>
        /// Obtiene el tipo de documento
        /// </summary>
        private int GetDocType(string pStrDoc)
        {
            int lIntType = 0;
            switch (pStrDoc)
            {
                case "Pedidos":
                    lIntType = 0;
                    break;

                case "Factura de reserva":
                    lIntType = 1;
                    break;
            }
            if (txtDoc.Value == "" || txtDoc.Value == "0")
            {
                lIntType = 0;
            }
            return lIntType;
        }

        /// <summary>
        /// Obtiene el tipo de pesada
        /// </summary>
        private int GetDocWT(string pStrDoc)
        {
            int lIntType = 0;
            switch (pStrDoc)
            {
                case "Doble":
                    lIntType = 0;
                    break;

                case "Simple":
                    lIntType = 1;
                    break;
            }
            return lIntType;
        }
        #endregion

        #endregion

        #region Menu
        ///<summary>    </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        private void loadMenu()
        { 
            this.UIAPIRawForm.EnableMenu("520", true); // Print
            this.UIAPIRawForm.EnableMenu("6659", false);  // Fax
            this.UIAPIRawForm.EnableMenu("1281", true); // Search Record
            this.UIAPIRawForm.EnableMenu("1282", true); // Add New Record
            this.UIAPIRawForm.EnableMenu("1288", true);  // Next Record
            this.UIAPIRawForm.EnableMenu("1289", true);  // Pevious Record
            this.UIAPIRawForm.EnableMenu("1290", true);  // First Record
            this.UIAPIRawForm.EnableMenu("1291", true);  // Last record
        }
        #endregion

        #region Data
        ///<summary>    Initializes the data sources. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        private void InitDataSources()
        {
            try
            {
                this.UIAPIRawForm.DataSources.DataTables.Add("RESULT");
                this.UIAPIRawForm.DataSources.UserDataSources.Add("UDS", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
                this.UIAPIRawForm.DataSources.UserDataSources.Add("UDSocio", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
                this.UIAPIRawForm.DataSources.UserDataSources.Add("UDDate", SAPbouiCOM.BoDataType.dt_DATE, 254);
                this.UIAPIRawForm.DataSources.UserDataSources.Add("UDDate2", SAPbouiCOM.BoDataType.dt_DATE, 254);
                this.UIAPIRawForm.DataSources.UserDataSources.Add("UDS_Pro", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
                //mStrTipoDoc = "CFL_Compra";

                mObjCFLVenta = mObjTicketServices.initChooseFromLists(false, "17", "CFL_Venta", this.UIAPIRawForm.ChooseFromLists);
                mObjTicketServices.initChooseFromLists(false, "1250000001", "CFL_Traslado", this.UIAPIRawForm.ChooseFromLists);
                mObjTicketServices.initChooseFromLists(false, "22", "CFL_Compra", this.UIAPIRawForm.ChooseFromLists);
                mObjTicketServices.initChooseFromLists(false, "64", "CFL_Almacen", this.UIAPIRawForm.ChooseFromLists);
                mObjTicketServices.initChooseFromLists(false, "13", "CFL_Factura", this.UIAPIRawForm.ChooseFromLists);
                mObjTicketServices.initChooseFromLists(false, "18", "CFL_FacturaProveedor", this.UIAPIRawForm.ChooseFromLists);
                mObjTicketServices.initChooseFromLists(false, "63", "CFL_Project", this.UIAPIRawForm.ChooseFromLists);

                this.UIAPIRawForm.DataSources.UserDataSources.Add("Peso1", SAPbouiCOM.BoDataType.dt_MEASURE, 20);
                this.UIAPIRawForm.DataSources.UserDataSources.Add("Peso2", SAPbouiCOM.BoDataType.dt_MEASURE, 20);
                oUserDataSourcePesoN = this.UIAPIRawForm.DataSources.UserDataSources.Add("PesoN", SAPbouiCOM.BoDataType.dt_MEASURE, 20);

                mDBDataSourceD = this.UIAPIRawForm.DataSources.DBDataSources.Add("RDR1");
                mDBDAtaSourceFac = this.UIAPIRawForm.DataSources.DBDataSources.Add("INV1");
                mDBDataSourceVenta = this.UIAPIRawForm.DataSources.DBDataSources.Add("RDR1");
                this.UIAPIRawForm.DataSources.DBDataSources.Add("POR1");
                this.UIAPIRawForm.DataSources.DBDataSources.Add("WTQ1");
                this.UIAPIRawForm.DataSources.DBDataSources.Add("PCH1");
                //oUserDataSource = this.UIAPIRawForm.DataSources.UserDataSources.Add("Importe", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 20); 
                //  Adding 2 CFL, one for the button and one for the edit text.
                mObjCFLSocio = mObjTicketServices.initChooseFromLists(false, "2", "CFL_Socio", this.UIAPIRawForm.ChooseFromLists);
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
                LogService.WriteError("[InitDataSources]: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        private void SetCHFLToTextBox(string lStrSource)
        {
            try
            {
                SAPbouiCOM.Conditions lObjCons = null;

                switch (lStrSource)
                {
                    case "CFL_Socio":
                        txtClSoc.DataBind.SetBound(true, "", "UDSocio");
                        txtClSoc.ChooseFromListUID = "CFL_Socio";
                        txtClSoc.ChooseFromListAlias = "CardCode";
                        break;

                    case "CFL_Compra":

                        lObjCons = mObjCFLSocio.GetConditions();
                        txtDoc.DataBind.SetBound(true, "", "UDS");
                        txtDoc.ChooseFromListUID = "CFL_Compra";
                        break;

                    case "CFL_Venta":
                        txtDoc.DataBind.SetBound(true, "", "UDS");
                        txtDoc.ChooseFromListUID = "CFL_Venta";
                        break;

                    case "CFL_Traslado":
                        txtDoc.DataBind.SetBound(true, "", "UDS");
                        txtDoc.ChooseFromListUID = "CFL_Traslado";
                        break;

                    case "CFL_Factura":
                        txtDoc.DataBind.SetBound(true, "", "UDS");
                        txtDoc.ChooseFromListUID = "CFL_Factura";
                        break;

                    case "CFL_FacturaProveedor":
                        txtDoc.DataBind.SetBound(true, "", "UDS");
                        txtDoc.ChooseFromListUID = "CFL_FacturaProveedor";
                        break;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[SetCHFLToTextBox]: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        ///<summary>    Initializes the chooseFromlist. 
        /// </summary>
        private void InitChooseFromListBussinesPartner()
        {
            string lStrType = string.Empty;
            if (mStrTipoDoc == "CFL_Venta")
            {
                lStrType = "C";
            }

            if (mStrTipoDoc == "CFL_Compra")
            {
                lStrType = "S";
            }


            //Condicion para recargar los socios de negocio cuando el tipo de movimiento es factura de reserva LE
            if (mStrTipoDoc.Equals("CFL_Factura"))
            {
                if (cboTypTic.Value.Contains("Venta"))
                {
                    lStrType = "C";
                }
                else if (cboTypTic.Value.Equals("Compra"))
                {
                    lStrType = "S";
                }
            }



            try
            {
                this.UIAPIRawForm.Freeze(true);

                SAPbouiCOM.Conditions lObjCons = null;
                SAPbouiCOM.Condition lObjCon = null;
                //  Adding Conditions to CFLPO
                lObjCons = mObjCFLSocio.GetConditions();

                bool lBolNewCond = true;
                foreach (SAPbouiCOM.Condition lObjCond in lObjCons)
                {
                    if (lObjCond.Alias == "CardType")
                    {
                        lObjCond.CondVal = lStrType;
                        lBolNewCond = false;
                        break;
                    }
                }

                if (lBolNewCond)
                {
                    lObjCon = lObjCons.Add();
                    lObjCon.Alias = "CardType";
                    lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                    lObjCon.CondVal = lStrType;
                }

                mObjCFLSocio.SetConditions(lObjCons);


                SetCHFLToTextBox("CFL_Socio");

                //txtClSoc.DataBind.SetBound(true, "", "UDSocio");
                //txtClSoc.ChooseFromListUID = "CFL_Params";
                //txtClSoc.ChooseFromListAlias = "CardCode";

            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                UIApplication.ShowMessageBox(string.Format("InitChooseFromListException: {0}", ex.Message));
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }
        #endregion

        #region Matrix
        ///<summary>    Creates the matrix. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        private void CreateMatrix()
        {
            //txtDate.DataBind.SetBound(true, "", "UDDate");
            //txtDate.Value = DateTime.Now.ToString("yyyyMMdd"); // Format(Date.Today, "yyyyMMdd")
            DateTime dt = DateTime.ParseExact(DateTime.Now.ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.GetCultureInfo("en-US"));
            txtDate.Value = dt.ToString("dd/MM/yyyy");
            txtDateOut.DataBind.SetBound(true, "", "UDDate2");
            mObjItem = this.UIAPIRawForm.Items.Add("mtxArtLst2", SAPbouiCOM.BoFormItemTypes.it_MATRIX);
            mObjItem.Left = 10;
            mObjItem.Top = 160;
            mObjItem.Height = 140;
            mObjItem.Width = this.UIAPIRawForm.ClientWidth - 10;
            SAPbouiCOM.ChooseFromListCollection lObjCFLs = this.UIAPIRawForm.ChooseFromLists;

            mObjMatrix = mObjMatrixUI.CreateMatrix(mObjItem, lObjCFLs);

            mObjMatrix.ValidateAfter += lObjMatrix_ValidateAfter;
            mObjMatrix.ValidateBefore += lObjMatrix_ValidateBefore;
        }

        private void lObjMatrix_ValidateBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            // throw new NotImplementedException();
        }

        private void lObjMatrix_ValidateAfter(object sboObject, SBOItemEventArg pVal)
        {

            // ClicPesoN(pVal);
        }



        ///<summary>    Loads matrix columns. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        ///<param name="pStrDataSource">    The pstr datasource. </param>
        private void LoadMatrixColumns(string pStrDataSource)
        {
            this.UIAPIRawForm.Freeze(true);
            SetConditionCFLWareHouse();
            try
            {
                mObjMatrix = mObjMatrixUI.LoadMatrixColumns(mObjMatrix, pStrDataSource, IsPurshase(), cboTypTic.Value);
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                UIApplication.ShowMessageBox(string.Format("InitCustomerChooseFromListException: {0}", ex.Message));
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        private bool IsPurshase()
        {
            if (mStrSource == "POR1")
            {
                return true;
            }
            return false;
        }

        ///<summary>    Loads matrix data from document. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        ///<param name="pstrAlias"> The pstr alias. </param>
        ///<param name="pstrCond">  The pstr condition. </param>
        private void LoadMatrixData(string pstrAlias, string pstrCond)
        {
            mObjMatrix.Clear();
            mDBDataSourceD.Clear();
            mDBDataSourceD = this.UIAPIRawForm.DataSources.DBDataSources.Item(mStrSource);
            mDBDataSourceD = mObjMatrixUI.LoadMatrixConditions(pstrAlias, pstrCond, mDBDataSourceD);
            int i = 0;
            for (i = 0; i <= mDBDataSourceD.Size - 1; i++)
            {
                mDBDataSourceD.SetValue("Quantity", i, "0");
                mDBDataSourceD.SetValue("LineTotal", i, "0");
                //string lStrLine = mObjMatrixUI.GetTableLine(pObjTicket, mStrSource);
                //string lStrTable = lStrLine.Remove(lStrLine.Length - 1);
                //mDBDataSourceD.SetValue("OpenCreQty", i, mObjTicketServices.GetDeliveryLine("O" + lStrTable, lStrLine, "OpenCreQty", pObjTicket.Number.ToString(), lObjTicketDetail.BaseLine.ToString()));
                //mDBDataSourceD.SetValue("OpenRtnQty", i, mObjTicketServices.GetDeliveryLine("O" + lStrTable, lStrLine, "OpenRtnQty", pObjTicket.Number.ToString(), lObjTicketDetail.BaseLine.ToString()));


                //mDBDataSourceD.SetValue("OpenCreQty", i, mObjTicketServices.GetDeliveryLine("OINV", "INV1", "OpenCreQty", pObjTicket.Number.ToString(), lObjTicketDetail.BaseLine.ToString()));
                //mDBDataSourceD.SetValue("DelivrdQty", i, mObjTicketServices.GetDeliveryLine("OINV", "INV1", "DelivrdQty", pObjTicket.Number.ToString(), lObjTicketDetail.BaseLine.ToString()));
                if (IsPurshase())
                {
                    mDBDataSourceD.SetValue("Price", i, "0");
                }
                // Load data back to 0 
            }
            if (mDBDataSourceD.Size > 0)
            {
                mObjMatrix.LoadFromDataSource();
                mObjMatrix.AutoResizeColumns();
            }

            if (cboTypTic.Value != "Venta de pesaje")
            {
                btnAddItem.Item.Visible = true;
            }

        }

        private bool IsDisableRow(int lIntRow)
        {
            string lStrDocEntry = mDBDataSourceD.GetValue("DocEntry", lIntRow - 1);
            string lStrLineStatus = mObjTicketServices.GetLineStatus(mStrSource, lStrDocEntry, (lIntRow - 1).ToString());


            if (lStrLineStatus == "C")
            {
                //    ((SAPbouiCOM.CheckBox)mObjMatrix.Columns.Item("Check").Cells.Item(mIntRow+1).Specific).Item.Enabled = false;
                //((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Sacos").Cells.Item(mIntRow+1).Specific).Item.Enabled = false;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Selecciona el renglon
        /// </summary>
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
                mObjRowCtrl = mObjMatrix.CommonSetting; // Guarda la fila seleccionada

               // mObjMatrix.LoadFromDataSourceEx();
                mObjMatrix.SelectRow(pIntRow, true, false);


                mStrColumn = pStrColumnID;
                mIntRow = pIntRow;

                //mObjMatrix.SetCellFocus(mIntRow, pStrColumnID);

                //bool lBolCheck = false;
                //if (cboTypTic.Value != "Venta de pesaje")
                //{
                //    lBolCheck = (mObjMatrix.Columns.Item("Check").Cells.Item(pIntRow).Specific as CheckBox).Checked;
                //}
                //string lStrPeso1 = ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Peso1").Cells.Item(mIntRow).Specific).Value;
                //mStrLastPeso = "";
                //if (mStrSource == "RDR1" || cboTypTic.Value == "Traslado - Salida")
                //{
                //    if (mObjCalculation.getLargeNumber(mObjMatrix) > 0)
                //    {
                //        mStrLastPeso = mObjCalculation.getLargeNumber(mObjMatrix).ToString();
                //    }
                //}
                //if (mStrSource == "POR1" || cboTypTic.Value == "Traslado - Entrada")
                //{
                //    if (mObjCalculation.getSmallerNumber(mObjMatrix) > 0)
                //    {
                //        mStrLastPeso = mObjCalculation.getSmallerNumber(mObjMatrix).ToString();
                //    }
                //}
                //if (mObjValidations.VerificarSegundoPeso(mObjMatrix) && !string.IsNullOrEmpty(mStrLastPeso) && lStrPeso1 == "0.0" && !lBolCheck
                //    && !string.IsNullOrEmpty(((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("ItemCode").Cells.Item(mIntRow).Specific).Value))
                //{
                //    if (pStrColumnID != "Check")
                //    {
                //        this.UIAPIRawForm.Freeze(false);
                //        if (SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Desea establecer el peso guardado como primer peso", 2, "Si", "No", "") == 1)
                //        {
                //            this.UIAPIRawForm.Freeze(true);
                //            ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Peso1").Cells.Item(mIntRow).Specific).Value = mStrLastPeso;
                //            mDBDataSourceD.SetValue("Weight1", mIntRow - 1, mStrLastPeso);
                //            mObjMatrix.LoadFromDataSource();
                //            //mStrLastPeso = string.Empty;
                //        }
                //    }
                //    if (cboTypTic.Value != "Venta de pesaje" && cboTypTic.Value != "Pesaje")
                //    {
                //        btnDelete.Item.Visible = true;
                //    }
                //}
            }
            catch (Exception lObjException)
            {
                UIApplication.ShowError(string.Format("Matriz: {0}", lObjException.Message));
                // UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        //private bool RowChecked(int pIntRow)
        //{
        //    if ((mObjMatrix.Columns.Item("Check").Cells.Item(pIntRow).Specific as CheckBox).Checked)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        #endregion

        #region Calc
        ///<summary>    Calculates the import. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        ///<param name="pVal">  The value. </param>
        /// 
        private void CalcImport(string pStrTypeTicket)
        {
            LogService.WriteInfo("CalcImport");
            this.UIAPIRawForm.Freeze(true);
            try
            {
                if (mIntRow > 0)
                {
                    mDBDataSourceD = mObjCalculation.CalcImport(pStrTypeTicket, mObjMatrix, mIntRow, mDBDataSourceD);
                    //mObjMatrix.LoadFromDataSource();
                    //SelectRow(mIntRow, "0");
                    if (mObjMatrix.IsRowSelected(mIntRow))
                    {
                        SelectRow(mIntRow, "0");
                    }
                    mObjMatrix.Item.Refresh();
                }

            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                UIApplication.ShowMessageBox("Error al calcular Importe " + ex.Message);
            }
            finally
            {
                CalcTotals();
                VerificarCheck();
                this.UIAPIRawForm.Freeze(false);
            }
        }

        /// <summary>
        /// Calcula los totales en la parte inferior
        /// </summary>
        private void CalcTotals()
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);
                double lDblInput = 0;
                double lDblOutput = 0;
                if (btnWT1.Item.Visible == true)
                {
                    if (!string.IsNullOrEmpty(txtInWT.Value))
                    {
                        lDblInput = Convert.ToDouble(txtInWT.Value, CultureInfo.GetCultureInfo("en-US"));
                    }
                    if (!string.IsNullOrEmpty(txtOutputWT.Value))
                    {
                        lDblOutput = Convert.ToDouble(txtOutputWT.Value, CultureInfo.GetCultureInfo("en-US"));
                    }
                }
                else
                {
                    lDblInput = mObjCalculation.getSmallerNumber(mObjMatrix);
                    lDblOutput = mObjCalculation.getLargeNumber(mObjMatrix);
                }
                TotalsDTO lObjTotals = mObjCalculation.CalcTotals(mObjMatrix, lDblInput, lDblOutput);
                txtTara.Value = lObjTotals.Tara.ToString(CultureInfo.GetCultureInfo("en-US"));// lDblTara.ToString();
                txtPesoB.Value = lObjTotals.WeightB.ToString(CultureInfo.GetCultureInfo("en-US"));//  lDblPesoBruto.ToString();
                //LE 62 
                txtVari.Value = (lObjTotals.Variation).ToString(CultureInfo.GetCultureInfo("en-US")); //(lDblPeso - lDblVariacion).ToString();
                txtPesoN.Value = lObjTotals.WeightNet.ToString(CultureInfo.GetCultureInfo("en-US"));// (lDblPesoBruto + lDblTara).ToString();
                txtAmount.Value = lObjTotals.Amount.ToString(CultureInfo.GetCultureInfo("en-US"));// Convert.ToDouble(lObjTotals.Amount, CultureInfo.GetCultureInfo("en-US")).ToString(CultureInfo.GetCultureInfo("en-US"));
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                UIApplication.ShowMessageBox("Error al calcular totales" + ex.Message);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }
        #endregion

        #region Choosefromlist
        ///<summary>    Adds choose from list. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        private void AddChooseFromList()
        {
            try
            {
                SetCHFLToTextBox("CFL_Compra");

                //txtDoc.DataBind.SetBound(true, "", "UDS");
                //txtDoc.ChooseFromListUID = "CFL_Compra";

                //SetConditionCFLProject();--------------------------------------------project
                //txtPro.DataBind.SetBound(true, "", "UDS_Pro");
                //txtPro.ChooseFromListUID = "CFL_Project";

            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("CustomerChooseListException: {0}", ex.Message));
            }
        }
        private void SetConditionCFLWareHouse()
        {
            TicketDAO lObjTicketDAO = new TicketDAO();
            string lStrWareHouse = lObjTicketDAO.GetWareHouse(DIApplication.Company.UserSignature.ToString());
            List<string> lLstWareHouse = lObjTicketDAO.GetWareHousePather(lStrWareHouse).ToList();

            SAPbouiCOM.ChooseFromListCollection lObjCFLs = null;
            ChooseFromList lObjCFL = null;
            lObjCFLs = this.UIAPIRawForm.ChooseFromLists;
            SAPbouiCOM.Conditions lObjCons = new Conditions();
            SAPbouiCOM.Condition lObjCon = null;

            lObjCFL = lObjCFLs.Item("CFL_Ware");

            int i = 1;
            foreach (string lStrWareHousePather in lLstWareHouse)
            {
                lObjCon = lObjCons.Add();
                lObjCon.Alias = "WhsCode";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = lStrWareHousePather;

                if (lLstWareHouse.Count() > i)
                {
                    lObjCon.Relationship = BoConditionRelationship.cr_OR;
                }
                i++;

            }

            lObjCFL.SetConditions(lObjCons);

        }
        private void SetConditionCFLProject()
        {
            TicketDAO lObjTicketDAO = new TicketDAO();
            List<string> lLstWareHouse = lObjTicketDAO.GetProjects().ToList();

            SAPbouiCOM.ChooseFromListCollection lObjCFLs = null;
            ChooseFromList lObjCFL = null;
            lObjCFLs = this.UIAPIRawForm.ChooseFromLists;
            SAPbouiCOM.Conditions lObjCons = new Conditions();
            SAPbouiCOM.Condition lObjCon = null;

            lObjCFL = lObjCFLs.Item("CFL_Project");

            int i = 1;
            foreach (string lStrWareHousePather in lLstWareHouse)
            {
                lObjCon = lObjCons.Add();
                lObjCon.Alias = "PrjCode";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = lStrWareHousePather;

                if (lLstWareHouse.Count() > i)
                {
                    lObjCon.Relationship = BoConditionRelationship.cr_OR;
                }
                i++;

            }

            lObjCFL.SetConditions(lObjCons);

        }


        /// <summary>
        /// Establece los parametros del choosefromlist
        /// </summary>
        private void SetParametersToChoose()
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);

                SAPbouiCOM.ChooseFromListCollection lObjCFLs = null;
                ChooseFromList lObjCFL = null;
                lObjCFLs = this.UIAPIRawForm.ChooseFromLists;
                SAPbouiCOM.Conditions lObjCons = null;
                SAPbouiCOM.Condition lObjCon = null;


                lObjCFL = lObjCFLs.Item(mStrTipoDoc);
                lObjCons = lObjCFL.GetConditions();

                if (lObjCons.Count > 0) //si ya contiene parametros se cargan nuevos
                {
                    lObjCons = new SAPbouiCOM.Conditions();
                    lObjCFL.SetConditions(lObjCons);
                }

                List<string> lLstDocuments = new List<string>();


                if (cbDocType.Value == "Factura de reserva")
                {
                    SetInvoice_Settings();
                }
                else
                {
                    SetDocument_Settings();
                }


                if (cboTypTic.Value.Equals("Venta"))
                {
                    if (cbDocType.Value == "Factura de reserva")
                    {
                        lLstDocuments = GetDocEntryByInvoice(txtClSoc.Value, "OINV", "INV1", "C", "!=");
                    }
                    else
                    {
                        lLstDocuments = GetDocEntryByBP("ORDR", "RDR1", txtClSoc.Value);///LE
                    }
                }
                else if (cboTypTic.Value.Equals("Compra"))
                {
                    if (cbDocType.Value == "Factura de reserva")
                    {
                        lLstDocuments = GetDocEntryByInvoice(txtClSoc.Value, "OPCH", "PCH1", "O", "!=");
                    }
                    else
                    {
                        lLstDocuments = GetDocEntryByBP("OPOR", "POR1", txtClSoc.Value);
                    }

                }
                else if (cboTypTic.Value.Equals("Traslado - Entrada"))
                {
                    lLstDocuments = GetDocEntryBytoWhs("PLHE");
                }
                else if (cboTypTic.Value.Equals("Traslado - Salida"))
                {
                    lLstDocuments = GetDocEntryByFiller("PLHE");
                }
                else if (cboTypTic.Value.Equals("Venta de pesaje"))
                {
                    if (cbDocType.Value == "Factura de reserva")
                    {
                        lLstDocuments = GetDocEntryByInvoice(txtClSoc.Value, "OINV", "INV1", "C", "=");
                    }
                }

                //}


                int i = 1;
                //lObjCon.Relationship = BoConditionRelationship.cr_AND;
                if (lLstDocuments != null)
                {
                    if (lLstDocuments.Count > 0)
                    {

                        var lvargroup = from p in lLstDocuments group p by p into gruped select gruped;

                        foreach (var item in lvargroup)
                        {
                            lObjCon = lObjCons.Add();
                            lObjCon.Alias = "DocEntry";
                            lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                            lObjCon.CondVal = item.Key;

                            if (lvargroup.Count() > i)
                            {
                                lObjCon.Relationship = BoConditionRelationship.cr_OR;
                            }
                            i++;
                        }
                    }
                    else
                    {
                        lObjCon = lObjCons.Add();
                        lObjCon.Alias = "DocEntry";
                        lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        lObjCon.CondVal = null;
                    }
                }
                //lObjCFL.SetConditions(lObjCons);
                lObjCFL.SetConditions(lObjCons);

                //}
            }
            catch (Exception ex)
            {
                LogService.WriteError("SetParametersToChoose " + ex);
                LogService.WriteError(ex);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        #region Reorganizar Luis
        /// <summary>
        /// Metodo para obtener los documentos para compra o venta
        /// </summary>
        /// <param name="pStrTableO"></param>
        /// <param name="pStrBP"></param>
        /// <returns></returns>
        private List<string> GetDocEntryByBP(string pStrTableO, string pStrTableL, string pStrBP)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            List<string> lLstDoc = new List<string>();

            try
            {
                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string lStrQuery = string.Format(@"
			select distinct T0.DocEntry from {0} T0
			inner join {1} T1 on T1.DocEntry=T0.DocEntry
			where CardCode ='{2}' and DocStatus !='C' and t1.WhsCode ='PLHE' -- and T0.DocEntry not in(select A0.U_Number from [@UG_PL_TCKT] A0 where A0.U_Status != '0' and U_BPCode = '{2}'  and A0.U_Number != null)
			 ", pStrTableO, pStrTableL, pStrBP);

                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstDoc.Add(lObjRecordset.Fields.Item(0).Value.ToString());
                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return lLstDoc;

        }

        /// <summary>
        /// Metodo para obtener las solicitudes de traslado (entrada/salida)
        /// </summary>
        /// <param name="pStrFiller"></param>
        /// <returns></returns>
        private List<string> GetDocEntryByFiller(string pStrFiller)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            List<string> lLstDoc = new List<string>();
            try
            {
                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string lStrQuery = string.Format(@"
			select DocEntry from OWTQ where DocStatus !='C' and Filler = '{0}' and CardCode is null --and DocEntry not in(select U_Number from [@UG_PL_TCKT] where U_Status != '0' and  U_BPCode = '')
		   ", pStrFiller);
                //                string lStrQuery = string.Format(@"
                //                                            select DocEntry, Filler FROM OWTQ T0
                //                                 Inner join OWHS T1 on T0.ToWhsCode = T1.WhsCode
                //                                 where  U_GLO_WhsTransit = 'S' AND DocStatus !='C'  and CardCode is null and DocNum not in(select U_Number from [@UG_PL_TCKT] where U_Status != '0' and  U_BPCode = '' and Filler = '{0}')
                //                                            ", pStrFiller);

                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstDoc.Add(lObjRecordset.Fields.Item(0).Value.ToString());
                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return lLstDoc;
        }



        /// <summary>
        /// Metodo para obtener las solicitudes de traslado (entrada/salida)
        /// </summary>
        /// <param name="pStrtoWhs"></param>
        /// <returns></returns>
        private List<string> GetDocEntryBytoWhs(string pStrtoWhs)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            List<string> lLstDoc = new List<string>();
            try
            {
                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string lStrQuery = string.Format(@"
			select DocEntry from OWTQ where DocStatus !='C' and ToWhsCode = '{0}' and CardCode is null --and DocEntry not in(select U_Number from [@UG_PL_TCKT] where U_Status != '0' and  U_BPCode = '')
		   ", pStrtoWhs);

                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstDoc.Add(lObjRecordset.Fields.Item(0).Value.ToString());
                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return lLstDoc;
        }

        /// <summary>
        /// Metodo para obtener las facturas de reserva(venta/compra)
        /// </summary>
        /// <param name="pStrCardCode"></param>
        /// <param name="pStrTableD"></param>
        /// <param name="pStrUpdivnt"></param>
        /// <returns></returns>
        private List<string> GetDocEntryByInvoice(string pStrCardCode, string pStrTableD, string pStrTableL, string pStrUpdivnt, string pStrExtraCondition)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            List<string> lLstDoc = new List<string>();
            try
            {
                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string lStrQuery = string.Format(@"
					select distinct T0.DocEntry from {0} T0
					inner join {1} T1 on T1.DocEntry = T0.DocEntry
					where T0.UPdinvnt = '{2}' and T0.CardCode = '{3}' and T0.CANCELED = 'N' and T1.WhsCode = 'PLHE' and T0.InvntSttus != 'C' and T1.ItemCode {4}'{5}' --AND T0.DocEntry not in (select U_Number from [@UG_PL_TCKT] where U_Status != '0' and U_BPCode = '{3}' and U_Number != null)
					", pStrTableD, pStrTableL, pStrUpdivnt, pStrCardCode, pStrExtraCondition, GetArticle());

                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstDoc.Add(lObjRecordset.Fields.Item(0).Value.ToString());
                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return lLstDoc;
        }

        private string GetDocNumByDocEntry(string mStrDocEntry)
        {
            return mObjQueryManager.GetValue("DocNum", "DocEntry", mStrDocEntry, "O" + mStrSource.Substring(0, 3));
        }

        private string GetArticle()
        {
            SAPbobsCOM.Recordset lObjRecordset = null;

            try
            {
                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string lStrQuery = "SELECT U_Value FROM [@UG_CONFIG] WHERE Name = 'PL_WEIGHING_SALE'";

                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    string x = (string)lObjRecordset.Fields.Item(0).Value;
                    return (string)lObjRecordset.Fields.Item(0).Value;
                }

            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return "";
        }


        //Todos los metodos excluyen los documentos que ya tienen un ticket abierto o pendiente (con documento abierto) o los cerrados

        #endregion


        #endregion

        #region CaptureMode

        /// <summary>1
        /// Cambia las variables de tipo de documento
        /// </summary>
        private void ChangeTypeTicket(int pIntType)
        {
            switch (pIntType)
            {
                case 0:
                    mStrTipoDoc = "CFL_Venta";
                    mStrSource = "RDR1";
                    break;

                case 1:
                    mStrTipoDoc = "CFL_Compra";
                    mStrSource = "POR1";
                    break;

                case 2:
                    mStrTipoDoc = "CFL_Traslado";
                    mStrSource = "WTQ1";
                    break;

                case 3:
                    mStrTipoDoc = "CFL_Traslado";
                    mStrSource = "WTQ1";
                    break;

                case 4:
                    mStrTipoDoc = "CFL_Venta";
                    mStrSource = "RDR1";
                    break;

                case 5:
                    mStrTipoDoc = "CFL_Venta";
                    mStrSource = "RDR1";
                    break;

            }
            InitChooseFromListBussinesPartner();
        }

        /// <summary>
        /// Cambia el tipo de ticket 
        /// </summary>
        private void ChangeCaptureMode(SAPbouiCOM.ItemEvent pValEvent)
        {
            this.UIAPIRawForm.Freeze(true);
            if (pValEvent.ItemUID == "cboTypTic")// && mBolIsUpdate == false)
            {
                ClearControls();
                mDBDataSourceD.Clear();
                mStrSource = "RDR1";
                cbWTType.Item.Enabled = false;
                cbCapType.Select(1, BoSearchKey.psk_Index);
                cbWTType.Select(0, BoSearchKey.psk_Index);

                cbDocType.Item.Visible = true;
                mStrTipoDoc = "CFL_Venta";
                txtClSoc.Item.Enabled = false;
                txtDoc.Item.Enabled = false;
                cbDocType.Item.Enabled = true;  /////
                switch (pValEvent.PopUpIndicator)
                {
                    case 0:
                        break;
                    case 1:
                        SetFiltersVenta();
                        break;

                    case 2:
                        SetFiltersCompra();
                        break;

                    case 3:
                        SetFIltersTraslados();
                        break;

                    case 4:
                        SetFIltersTraslados();
                        break;

                    case 5:
                        SetFiltersVentaPesaje();
                        break;
                    case 6:
                        SetWeighing();
                        break;
                }

                SetCHFLToTextBox(mStrTipoDoc);

                if (cbDocType.Value == "Factura de reserva" && !cboTypTic.Value.Equals("Pesaje"))////
                {
                    SetInvoice_Settings();
                }
                else
                {
                    SetDocument_Settings();
                }

                txtDoc.Value = null;
                mObjMatrix.Clear();
                LoadMatrixColumns(mStrSource);
                mObjMatrix.LoadFromDataSource();
                VerificarCheck();
            }
            if (pValEvent.ItemUID == "cbDocType" && mBolIsUpdate == false && cboTypTic.Value != "Seleccione")
            {
                ChangeToInvoice(pValEvent);
            }
            this.UIAPIRawForm.Freeze(false);
        }

        private void SetFiltersVenta()
        {
            mStrTipoDoc = "CFL_Venta";
            mStrSource = "RDR1";
            InitChooseFromListBussinesPartner();

            cbDocType.Item.Visible = true;
            txtClSoc.Item.Enabled = true;
            txtDoc.Item.Enabled = false;
            lblTypeD.Item.Visible = true;
            ReloadMatrix();
        }

        private void SetFiltersCompra()
        {
            mStrTipoDoc = "CFL_Compra";
            mStrSource = "POR1";
            InitChooseFromListBussinesPartner();
            txtClSoc.Item.Enabled = true;
            txtDoc.Item.Enabled = false;
            cbDocType.Item.Visible = true;
            lblTypeD.Item.Visible = true;
            ReloadMatrix();
        }

        private void SetFIltersTraslados()
        {
            mStrTipoDoc = "CFL_Traslado";
            mStrSource = "WTQ1";
            SetParametersToChoose();
            txtClSoc.Item.Enabled = false;
            txtDoc.Item.Enabled = true;
            cbDocType.Item.Visible = false;
            lblTypeD.Item.Visible = false;
            ReloadMatrix();
        }

        private void SetFiltersVentaPesaje()
        {
            mStrTipoDoc = "CFL_Venta";
            mStrSource = "INV1";
            cbDocType.Item.Visible = true;
            cbDocType.Item.Enabled = false;
            cbDocType.Select(1, BoSearchKey.psk_Index);
            cbWTType.Select(0, BoSearchKey.psk_Index);
            lblTypeD.Item.Visible = true;
            InitChooseFromListBussinesPartner();

            txtClSoc.Item.Enabled = true;
            cbWTType.Item.Enabled = true;
            txtDoc.Item.Enabled = false;
            //mDBDataSourceD.Clear();

        }


        private void SetWeighing()
        {
            mStrTipoDoc = "CFL_Venta";
            mStrSource = "INV1";
            txtDoc.Item.Enabled = false;
            cbDocType.Item.Enabled = false;
            cbWTType.Item.Enabled = true;
            cbDocType.Item.Visible = false;
            lblTypeD.Item.Visible = false;
            mDBDataSourceD = this.UIAPIRawForm.DataSources.DBDataSources.Item(mStrSource);
            mDBDataSourceD = mObjMatrixUI.AddItemWeigin(mDBDataSourceD);
            mObjMatrix.LoadFromDataSource();
        }

        private void ReloadMatrix()
        {
            mDBDataSourceD = this.UIAPIRawForm.DataSources.DBDataSources.Item(mStrSource);
        }

        /// <summary>
        /// Cambia el tipo de documento a factura
        /// </summary>
        private void ChangeToInvoice(SAPbouiCOM.ItemEvent pValEvent)
        {
            switch (pValEvent.PopUpIndicator)
            {
                case 1:
                    //Solo se crearon metodos individuales para poder ser reutilizados fuera del evento
                    SetInvoice_Settings();
                    SetParametersToChoose();
                    btnAddItem.Item.Visible = false;
                    btnDelete.Item.Visible = false;

                    break;

                case 0:
                    //Solo se crearon metodos individuales para poder ser reutilizados fuera del evento
                    SetDocument_Settings();
                    SetParametersToChoose();
                    SetFirstLine();
                    btnAddItem.Item.Visible = true;
                    break;
            }
            mStrLastPeso = "";
            txtInWT.Value = "";
            txtOutputWT.Value = "";
            txtDoc.Value = "";
            CalcTotals();
        }

        private void SetFirstLine()
        {
            if (cboTypTic.Value != "Venta de pesaje" && cboTypTic.Value != "Compra" && cboTypTic.Selected.Value != "Seleccione" && mObjMatrix.RowCount == 0)
            {
                mDBDataSourceD.Clear();
                mDBDataSourceD = this.UIAPIRawForm.DataSources.DBDataSources.Item(mStrSource);
                mDBDataSourceD.InsertRecord(0);
                mDBDataSourceD.SetValue("ItemCode", 0, "");
                mObjMatrix.Columns.Item("ItemCode").Editable = true;
                mDBDataSourceD.SetValue("Quantity", 0, "0");
                btnAddItem.Item.Visible = true;

                mObjMatrix.LoadFromDataSource();
            }
        }


        #region Reorganizar Luis
        /// <summary>
        /// Metodo para poner los defaults de documentos
        /// </summary>
        private void SetDocument_Settings()
        {
            if (cboTypTic.Value == "Venta")
            {
                mStrTipoDoc = "CFL_Venta";
                mStrSource = "RDR1";
                txtDoc.ChooseFromListUID = "CFL_Venta";
                mDBDataSourceD.Clear();
                mObjMatrix.Clear();
                LoadMatrixColumns("RDR1");
            }
            else if (cboTypTic.Value == "Compra")
            {
                mStrTipoDoc = "CFL_Compra";
                mStrSource = "POR1";
                txtDoc.ChooseFromListUID = "CFL_Compra";
                mDBDataSourceD.Clear();
                mObjMatrix.Clear();
                LoadMatrixColumns("POR1");
            }
        }

        /// <summary>
        /// Metodo para Poner los defaults en factura de reserva
        /// </summary>
        private void SetInvoice_Settings()
        {

            if (cboTypTic.Value == "Venta")
            {
                mStrTipoDoc = "CFL_Factura";
                mStrSource = "INV1";
                txtDoc.ChooseFromListUID = "CFL_Factura";
                SetCHFLToTextBox("CFL_Factura");
                LoadMatrixColumns("INV1");
                mDBDataSourceD.Clear();
                mObjMatrix.Clear();
            }
            else if (cboTypTic.Value == "Compra")
            {
                mStrTipoDoc = "CFL_FacturaProveedor";
                mStrSource = "PCH1";
                SetCHFLToTextBox("CFL_FacturaProveedor");
                txtDoc.ChooseFromListUID = "CFL_FacturaProveedor";
                LoadMatrixColumns("PCH1");
                mDBDataSourceD.Clear();
                mObjMatrix.Clear();
            }
            else if (cboTypTic.Value == "Venta de pesaje")
            {
                mStrTipoDoc = "CFL_Factura";
                mStrSource = "INV1";
                txtDoc.ChooseFromListUID = "CFL_Factura";
                SetCHFLToTextBox("CFL_Factura");
            }
        }
        #endregion

        #endregion

        #region Validation

        /// <summary>
        /// Validaciones de campos no nulos
        /// </summary>
        private bool ValidateControls()
        {
            IList<string> lLstmissingFields = new List<string>();
            if (string.IsNullOrEmpty(txtInWT.Value) || Convert.ToDouble(txtInWT.Value, CultureInfo.GetCultureInfo("en-US")) == 0)
            {
                lLstmissingFields.Add("Peso inicial");
            }
            if (txtStatus.Value == "Cerrado")
            {
                lLstmissingFields.Add("Ticket en estatus cerrado");
                this.UIAPIRawForm.Freeze(false);
                UIApplication.ShowMessageBox("Ticket en estatus cerrado");
                return false;
            }
            if (txtStatus.Value == "Pendiente")
            {
                if (!cboTypTic.Value.Equals("Compra"))
                {
                    lLstmissingFields.Add("Ticket en estatus pendiente");
                    this.UIAPIRawForm.Freeze(false);
                    UIApplication.ShowMessageBox("Ticket en estatus pendiente");
                    return false;
                    //this.UIAPIRawForm.Freeze(true);
                }
            }


            if (string.IsNullOrEmpty(txtClSoc.Value.Trim()) && cboTypTic.Value != "Traslado - Entrada" && cboTypTic.Value != "Traslado - Salida" && cboTypTic.Value != "Pesaje")
            {
                lLstmissingFields.Add("Socio de negocio");
            }

            if (string.IsNullOrEmpty(cboTypTic.Value.Trim()))
            {
                lLstmissingFields.Add("Tipo de movimiento");
            }
            //if (string.IsNullOrEmpty(txtClSoc.Value.Trim()))
            //{
            //    lLstmissingFields.Add("Socio de negocio");
            //}

            if (string.IsNullOrEmpty(cbWTType.Value.Trim()))
            {
                lLstmissingFields.Add("Tipo de pesada");
            }
            if (string.IsNullOrEmpty(txtDriver.Value.Trim()))
            {
                lLstmissingFields.Add("Chofer");
            }
            if (string.IsNullOrEmpty(txtCarTag.Value.Trim()))
            {
                lLstmissingFields.Add("Placas");
            }
            if (string.IsNullOrEmpty(txtDate.Value.Trim()))
            {
                lLstmissingFields.Add("Fecha de entrada");
            }
            if (string.IsNullOrEmpty(txtInWT.Value.Trim()))
            {
                txtInWT.Value = "0";
            }
            if (string.IsNullOrEmpty(txtOutputWT.Value.Trim()))
            {
                txtOutputWT.Value = "0";
            }
            if (string.IsNullOrEmpty(txtAmount.Value.Trim()))
            {
                txtAmount.Value = "0";
            }

            //if (cboTypTic.Value == "Venta")
            //{
            //    double ldubAmount = Convert.ToDouble(txtAmount.Value, CultureInfo.GetCultureInfo("en-US"));
            //    if (ldubAmount < 0)
            //    {
            //        lLstmissingFields.Add("Verificar datos de báscula");
            //    }
            //}

            //if (cboTypTic.Value == "Compra")
            //{
            //    double ldubAmount = Convert.ToDouble(txtAmount.Value, CultureInfo.GetCultureInfo("en-US"));
            //    if (ldubAmount > 0)
            //    {
            //        lLstmissingFields.Add("Verificar datos de báscula");
            //    }
            //}
            if (lLstmissingFields.Count > 0)
            {
                string message = string.Format("Favor de completar {0}:\n{1}",
                    (lLstmissingFields.Count == 1 ? "el siguiente campo" : "los siguientes campos"),
                    string.Join("\n", lLstmissingFields.Select(x => string.Format("-{0}", x)).ToArray()));
                this.UIAPIRawForm.Freeze(false);
                UIApplication.ShowMessageBox(message);
                this.UIAPIRawForm.Freeze(true);
            }

            return lLstmissingFields.Count == 0 ? true : false;
        }

        private void VerificarCheck()
        {
            try
            {


                SAPbouiCOM.CommonSetting lObjRowCtrl;
                lObjRowCtrl = mObjMatrix.CommonSetting;
                bool lBolActivateButtons = false;
                for (int i = 1; i <= mObjMatrix.RowCount; i++)
                {
                    if ((mObjMatrix.Columns.Item("Check").Cells.Item(i).Specific as CheckBox).Checked)
                    {

                        lBolActivateButtons = true;
                    }
                }
                if (lBolActivateButtons)
                {
                    btnWT1.Item.Visible = true;
                    btnWT2.Item.Visible = true;
                }
                else
                {
                    btnWT1.Item.Visible = false;
                    btnWT2.Item.Visible = false;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[VerificarCheck]: " + ex.Message);
                LogService.WriteError(ex);
            }
        }

        #endregion

        #region Searches
        private List<string> GetDocumentByWhs(string pStrWhsCode, string pStrCardCode)
        {
            List<string> lLstDocuments = mObjTicketServices.GetDocumentByWhs(pStrWhsCode, mStrSource, pStrCardCode);
            //QueryManager mObjQueryManager = new QueryManager();
            //mObjQueryManager.GetObjectsList<DocumentDTO>("WhsCode", lStrWhsCode, mStrSource);
            return lLstDocuments;
        }
        private List<string> GetDocumentsByInvoice(string pStrCardCode)
        {
            List<string> lLstDocuments = mObjTicketServices.GetDocumentsByInvoice(pStrCardCode);
            return lLstDocuments;
        }
        private List<string> GetDocumentsByAPInvoice(string pStrCardCode)
        {
            List<string> lLstDocuments = mObjTicketServices.GetDocumentsByAPInvoice(pStrCardCode);
            return lLstDocuments;
        }

        private int GetLastTicket()
        {
            int lIntFolio = 0;
            try
            {
                QueryManager mObjQueryManager = new QueryManager();
                string lStrCode = mObjQueryManager.Max<string>("Code", "[@UG_PL_TCKT]");
                string lStrFolio = mObjQueryManager.GetValue("U_Folio", "Code", lStrCode, "[@UG_PL_TCKT]");
                if (string.IsNullOrEmpty(lStrCode))
                {
                    lStrCode = "0";
                }


                lIntFolio = (Convert.ToInt32(lStrCode) + 1);
            }
            catch (Exception)
            {

                throw;
            }
            return lIntFolio;
        }
        #endregion

        #region Services
        #region WeiginMachine
        /// <summary>
        /// Conexion con el servicio de bascula.
        /// </summary>
        /// 

        private void GetRemoteObject()
        {
            ServiceController mObjServiceController = new ServiceController();
            mObjServiceController.ServiceName = "WeighingMachineService";


            try
            {
                if (mObjServiceController.Status == ServiceControllerStatus.Running)
                {

                    //Objects
                    mObjWeighingMachine = (WeighingMachineServerObject)Activator.GetObject(typeof(WeighingMachineServerObject), "http://localhost:8810/WeighingMachine");
                    mObjWrapperObject = new WrapperObject();

                    //Events

                    mObjWeighingMachine.DataReceived += new WeighingMachineEventHandler(mObjWrapperObject.WrapperOnDataReceived);
                    mObjWrapperObject.WrapperDataReceived += new WeighingMachineEventHandler(OnDataReceived);


                    //Connection
                    mObjConnection = mObjWeighingMachine.Connect();

                }

            }
            catch (Exception lObjException)
            {
                LogService.WriteError("[GetRemoteObject]: " + lObjException.Message);
                LogService.WriteError(lObjException);
                UIApplication.ShowMessageBox(string.Format("No fue posible conetarse con el servicio de bascula o impresora : {0}", lObjException.Message));
                // return false;
            }

        }


        private void GetRemoteObjectPrint()
        {
            try
            {
                ServiceController mObjServiceControllerPrint = new ServiceController();
                //mObjWritePort = (WritePortServerObject)Activator.GetObject(typeof(WritePortServerObject), "http://localhost:8830/WritePort");
                mObjServiceControllerPrint.ServiceName = "WritePortService";

                if (mObjServiceControllerPrint.Status == ServiceControllerStatus.Running)
                {
                    try
                    {

                        //mObjConnectionPrinter = mObjWritePort.Connect();

                    }
                    catch (Exception ex)
                    {
                        LogService.WriteError("[GetRemoteObjectPrint]: " + ex.Message);
                        //mObjWritePort.DisconnectAll();
                        if (mObjServiceControllerPrint.CanStop)
                        {
                            if (!IsAdministrator())
                            {
                                UIApplication.ShowMessageBox("Favor de iniciar la aplicación como Administrador.");
                            }
                            else
                            {
                                mObjServiceControllerPrint.Close();
                                mObjServiceControllerPrint.Stop();
                                LogService.WriteInfo("Deteniendo servicio de impresion");
                                UIApplication.ShowWarning("Deteniendo servicio de impresion");
                                mObjServiceControllerPrint.WaitForStatus(ServiceControllerStatus.Stopped);

                                //StartService();
                            }
                        }
                        else
                        {
                            LogService.WriteInfo("El servicio de impresion no puede ser detenido");
                            UIApplication.ShowWarning("El servicio de impresion no puede ser detenido");
                        }

                    }
                }

            }
            catch (Exception lObjException)
            {
                LogService.WriteError("[GetRemoteObject]: " + lObjException.Message);
                LogService.WriteError(lObjException);
                UIApplication.ShowMessageBox(string.Format("No fue posible conetarse con el servicio de bascula o impresora : {0}", lObjException.Message));
            }
        }

        private void StartService()
        {
            LogService.WriteInfo("Iniciando servicio");
            ServiceController mObjServiceController = new ServiceController();
            //ServiceController mObjServiceControllerPrint = new ServiceController();
            mObjServiceController.ServiceName = "WeighingMachineService";
            //mObjServiceControllerPrint.ServiceName = "WritePortService";
            //.MachineName = Environment.MachineName;

            try
            {
                if (mObjServiceController.Status == ServiceControllerStatus.Stopped)
                {
                    LogService.WriteInfo("Iniciando servicio de bascula");
                    StartService(mObjServiceController);

                }

                //if (mObjServiceControllerPrint.Status == ServiceControllerStatus.Stopped)
                //{
                //    LogService.WriteInfo("Iniciando servicio de Impresora");
                //    StartService(mObjServiceControllerPrint);

                //}
            }
            catch (Exception lObjException)
            {
                LogService.WriteError("Error" + lObjException.Message);
            }
            finally
            {

            }

        }

        private void StartService(ServiceController pObjServiceController)
        {

            UIApplication.ShowWarning("Inciando servicio de bascula favor espere");
            LogService.WriteInfo("Iniciando servicio...");

            try
            {
                pObjServiceController.Start();
                UIApplication.ShowWarning("iniciando servicio");
                pObjServiceController.WaitForStatus(ServiceControllerStatus.Running);

                UIApplication.ShowWarning("Inciando servicio de bascula iniciado");
                LogService.WriteInfo("Listo.");
            }
            catch (InvalidOperationException ex)
            {
                if (!IsAdministrator())
                {
                    UIApplication.ShowMessageBox("Favor de iniciar la aplicación como Administrador.");
                }
                else
                {
                    UIApplication.ShowMessageBox("No se pudo iniciar el servicio. " + ex.Message);
                    LogService.WriteError("No se pudo iniciar el servicio.");
                    LogService.WriteError(ex);
                }


            }
        }

        private bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
        }

        //<summary>
        //Cada vez que se obtenga un dato se actualiza el textblock.
        //</summary>
        private void OnDataReceived(string pStrValue)
        {
            try
            {
                if (UIApplication.GetApplication().Forms.Count > 0)
                {
                    Form lObjForm = UIApplication.GetApplication().Forms.ActiveForm;

                    //if (!string.IsNullOrEmpty(mStrSimWeight))
                    //{
                    //    //this.txtSim = ((SAPbouiCOM.EditText)(lObjForm.Items.Item("txtSim").Specific));
                    //    //if (!string.IsNullOrEmpty(txtSim.Value))
                    //    //{
                    //    //    lStrSim = txtSim.Value;
                    //    if (mStrSimWeight.Length < 9 && Convert.ToDecimal(mStrSimWeight) > 0)
                    //    {
                    //        mStrWeight = mStrSimWeight;
                    //    }
                    //}

                    //else
                    //{
                    if (pStrValue.Length < 9 && Convert.ToDecimal(pStrValue) > 0 && lObjForm != null && lObjForm.TypeEx == "UGRS.PlantaAlimentos.Forms.TicketForm")
                    {
                        this.lblWeight = ((SAPbouiCOM.StaticText)(lObjForm.Items.Item("lblWeight").Specific));
                        mStrWeight = pStrValue;
                        lblWeight.Caption = pStrValue;

                        //mStrWeight = pStrValue;
                        //lblWeight.Caption = pStrValue;

                    }
                }
                //lblWeight.Caption = GetDataReceived(pStrValue);
            }
            catch (Exception lObjException)
            {

                LogService.WriteError("[OnDataReceived]: " + lObjException.Message);
                LogService.WriteError(lObjException);
                UIApplication.ShowMessage(string.Format("Recibir datos de báscula: {0}", lObjException.Message));
            }

            mBolStarted = true;
            // txtDoc.Value = pStrValue;

            //lblWeight.Dispatcher.Invoke((Action)delegate
            //{
            //    tblWeight.Text = pStrValue;
            //});
        }

        private string GetDataReceived(string pStrValue)
        {
            string lStrData = "";
            mStrDataReceived = "0";

            try
            {
                if (!string.IsNullOrEmpty(pStrValue.Trim()) && Convert.ToDouble(pStrValue, CultureInfo.GetCultureInfo("en-US")) >= 0)
                {
                    lStrData += pStrValue;
                }
                else
                {
                    mStrDataReceived = lStrData;
                }
            }
            catch (Exception lObjException)
            {
                LogService.WriteError("[GetDataReceived]: " + lObjException.Message);
                LogService.WriteError(lObjException);
                UIApplication.ShowError(string.Format("Recibir datos de báscula: {0}", lObjException.Message));
            }

            mStrWeight = mStrDataReceived;
            return mStrDataReceived;
        }

        #endregion
        #endregion

        #endregion

        #region Controls

        #region EditText
        private SAPbouiCOM.EditText txtDoc;
        private SAPbouiCOM.EditText txtClSoc;
        private SAPbouiCOM.EditText txtNomSoc;
        private SAPbouiCOM.EditText txtDriver;
        private SAPbouiCOM.EditText txtCarTag;
        private SAPbouiCOM.EditText txtComents;
        private SAPbouiCOM.EditText txtOutputWT;
        private SAPbouiCOM.EditText txtInWT;
        private SAPbouiCOM.EditText txtPesoB;
        private SAPbouiCOM.EditText txtPesoN;
        private SAPbouiCOM.EditText txtTara;
        private SAPbouiCOM.EditText txtAmount;
        private SAPbouiCOM.EditText txtDate;
        private SAPbouiCOM.EditText txtDateOut;
        private SAPbouiCOM.EditText txtFolio;
        private SAPbouiCOM.EditText txtStatus;
        private SAPbouiCOM.EditText txtVari;
        private SAPbouiCOM.EditText txtPro;
        private SAPbouiCOM.EditText txtSim;
        #endregion

        #region StaticText
        private SAPbouiCOM.StaticText lblWeight;
        private SAPbouiCOM.StaticText lblProj;
        private SAPbouiCOM.StaticText lblTypeD;
        #endregion

        #region Button
        private SAPbouiCOM.Button btnCopyTo;
        private SAPbouiCOM.Button btnSave;
        private SAPbouiCOM.Button btnWeight;
        private SAPbouiCOM.Button btnLastWT;
        private SAPbouiCOM.Button btnCancel;
        private SAPbouiCOM.Button btnWT1;
        private SAPbouiCOM.Button btnWT2;
        private SAPbouiCOM.Button btnAddItem;
        private SAPbouiCOM.Button btnDelete;
        private SAPbouiCOM.Button btnClose;
        private SAPbouiCOM.Button btnPrint;
        #endregion

        #region Combobox
        private SAPbouiCOM.ComboBox cbWTType;
        private SAPbouiCOM.ComboBox cbCapType;
        private SAPbouiCOM.ComboBox cboTypTic;
        private SAPbouiCOM.ComboBox cbDocType;
        #endregion

        #region Matrix
        private SAPbouiCOM.Matrix mtxArtLst;
        #endregion
        #endregion


        private string GetWhs(Ticket pObjTicket)
        {
            string lStrDocEntry = string.Empty;
            string lStrToWhsCode = string.Empty;
            string lStrFromWhsCode = string.Empty;
            try
            {
                SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordSet.DoQuery("select DocEntry, Filler, ToWhsCode from OWTQ where DocEntry = " + pObjTicket.Number);
                if (lObjRecordSet.RecordCount == 1)
                {
                    lStrDocEntry = lObjRecordSet.Fields.Item(0).Value.ToString();
                    lStrFromWhsCode = lObjRecordSet.Fields.Item(1).Value.ToString();
                    lStrToWhsCode = lObjRecordSet.Fields.Item(2).Value.ToString();
                }
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
            }
            return lStrToWhsCode;
        }
    }
}
