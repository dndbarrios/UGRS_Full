/*
Autor: LCC Abraham Saúl Sandoval Meneses
Description: Food Delivery Form
Date: 16/08/2018
Company: Qualisys
*/

using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI.Corrals.DAO;
using UGRS.Core.SDK.DI.Corrals.DTO;
using UGRS.Core.SDK.DI.Corrals.Services;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;


namespace UGRS.AddOn.Corrals.Forms {
    [FormAttribute("UGRS.AddOn.Corrals.Forms.frmDelivery", "Forms/frmDelivery.b1f")]
    class frmDelivery : UserFormBase {

        #region Properties
        bool requireBagsField = false;
        int changedRow = 0;
        int mIntRowId = 0;
        string defaultUserWhs = String.Empty;
        string defaultUserArea = String.Empty;
        int series = 0;
        Dictionary<string, BoFieldsType> matrixColumns;
        DistributionDAO lObjDistributionDAO = new DistributionDAO();
        Dictionary<string, double> lDicNonLockedItems = null;
        #endregion

        #region Constructor
        public frmDelivery() {

            Parallel.Invoke(PrepareMatrix,
                            LoadChoosesFromList,
                            LoadEvents,
                            SetDefaulValues
                            );
        }

        public void PrepareMatrix() {

            matrixColumns = new Dictionary<string, BoFieldsType>() { { "#", BoFieldsType.ft_AlphaNumeric }, { "Code", BoFieldsType.ft_AlphaNumeric }, { "Name", BoFieldsType.ft_AlphaNumeric }, { "Whs", BoFieldsType.ft_AlphaNumeric }, { "Type", BoFieldsType.ft_AlphaNumeric }, { "Exist", BoFieldsType.ft_Float }, { "Food", BoFieldsType.ft_Float }, { "Bags", BoFieldsType.ft_Float }, { "Deliv", BoFieldsType.ft_AlphaNumeric } };
            dt0 = CreateDataTable("DataTable", matrixColumns);
            mtx0.Columns.Item("C_Result").Visible = false;
            mtx0.AutoResizeColumns();
        }

        public void SetDefaulValues() {

            defaultUserWhs = lObjDistributionDAO.GetUserDefaultWarehouse();
            defaultUserArea = lObjDistributionDAO.GetUserCostCenter();
            series = lObjDistributionDAO.GetSeries(defaultUserWhs, "15"); //ODLN - Delivery - 15
            txtDate.Value = DateTime.Now.ToString("yyyyMMdd");
        }
        #endregion

        #region Load & Unload Events
        private void LoadEvents() {
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent += new _IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
            SAPbouiCOM.Framework.Application.SBO_Application.MenuEvent += new _IApplicationEvents_MenuEventEventHandler(SBO_Application_MenuEvent);
        }

        private void UnLoadEvents() {

            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent -= new _IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
            SAPbouiCOM.Framework.Application.SBO_Application.MenuEvent -= new _IApplicationEvents_MenuEventEventHandler(SBO_Application_MenuEvent);
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent() {
            this.btnAdd = ((SAPbouiCOM.Button)(this.GetItem("btnAdd").Specific));
            this.btnAdd.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnAdd_ClickBefore);
            this.btnCancel = ((SAPbouiCOM.Button)(this.GetItem("btnCancel").Specific));
            this.btnCancel.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCancel_ClickBefore);
            this.lblItem = ((SAPbouiCOM.StaticText)(this.GetItem("lblItem").Specific));
            this.txtCode = ((SAPbouiCOM.EditText)(this.GetItem("txtCode").Specific));
            this.lblCorral = ((SAPbouiCOM.StaticText)(this.GetItem("lblCorral").Specific));
            this.btnSearch = ((SAPbouiCOM.Button)(this.GetItem("btnSearch").Specific));
            this.btnSearch.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSearch_ClickBefore);
            this.txtCorral = ((SAPbouiCOM.EditText)(this.GetItem("txtCorral").Specific));
            this.lblTotal = ((SAPbouiCOM.StaticText)(this.GetItem("lblTotal").Specific));
            this.txtKilos = ((SAPbouiCOM.EditText)(this.GetItem("txtKilos").Specific));
            this.txtBags = ((SAPbouiCOM.EditText)(this.GetItem("txtBags").Specific));
            this.lblDate = ((SAPbouiCOM.StaticText)(this.GetItem("lblDate").Specific));
            this.txtDate = ((SAPbouiCOM.EditText)(this.GetItem("txtDate").Specific));
            this.mtx0 = ((SAPbouiCOM.Matrix)(this.GetItem("mtx0").Specific));
            this.mtx0.ValidateAfter += new SAPbouiCOM._IMatrixEvents_ValidateAfterEventHandler(this.mtx0_ValidateAfter);
            this.txtItem = ((SAPbouiCOM.EditText)(this.GetItem("txtItem").Specific));
            this.btnNew = ((SAPbouiCOM.Button)(this.GetItem("btnNew").Specific));
            this.btnNew.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnNew_ClickBefore);
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents() { }
        private void OnCustomInitialize() { }
        #endregion

        #region Matrix
        private SAPbouiCOM.DataTable CreateDataTable(string tableID, Dictionary<string, BoFieldsType> columns) {

            DataTable dataTable;
            try {

                this.UIAPIRawForm.DataSources.DataTables.Add(tableID);
                dataTable = this.UIAPIRawForm.DataSources.DataTables.Item(tableID);

                Parallel.ForEach(columns, column => {
                    dataTable.Columns.Add("C_" + column.Key, column.Value);
                });

                dataTable.Columns.Add("C_Result", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);

                return dataTable;
            }
            catch(Exception ex) {
                HandleException(ex, "(Create DataTable)");
                return null;
            }
        }

        private void FillMatrix() {

            txtKilos.Value = "0";
            txtBags.Value = "0";

            try {

                dt0.Rows.Clear();
                var lLstDistribution = lObjDistributionDAO.GetDistributionFoodInCorrals(txtDate.Value, txtCorral.Value).ToList();
                var columns = new List<string>(matrixColumns.Keys);

                foreach (var column in columns)
                {
                    mtx0.Columns.Item(string.Format("C_{0}", column)).TitleObject.Sortable = true;
                    mtx0.Columns.Item(string.Format("C_{0}", column)).TitleObject.Sort(SAPbouiCOM.BoGridSortType.gst_Ascending);
                }

                if(lLstDistribution.Count > 0) {
                    Parallel.For(0, lLstDistribution.Count, row => {
                        dt0.Rows.Add();
                    });

                    Task.Factory.StartNew(() => {
                        Parallel.For(0, lLstDistribution.Count, row => {
                            dt0.SetValue("C_#", row, row + 1);
                        });
                    });

                    Parallel.ForEach(Partitioner.Create(0, lLstDistribution.Count), (range, state) => {
                        for(int i = range.Item1; i < range.Item2; i++) {
                            Parallel.ForEach(columns.Skip(1), column => {
                                dt0.SetValue("C_" + column, i, lLstDistribution[i].GetType().GetProperty(column).GetValue(lLstDistribution[i], null));
                            });
                        }
                    });

                    BindDataMatrix(mtx0, "DataTable", columns);
                    LogService.WriteInfo("(frmDelivery) Matriz Cargada correctamente");
                }
                else {
                    this.UIAPIRawForm.Freeze(true);
                    mtx0.Clear();
                    mtx0.AutoResizeColumns();
                    this.UIAPIRawForm.Freeze(false);
                }
            }
            catch(AggregateException ae) {
                ae.Handle(e => {
                    HandleException(e, "(AE)");
                    return true;
                });
            }
            catch(Exception ex) {
                HandleException(ex, "(FillMatrix)");
            }
            finally {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        public void BindDataMatrix(Matrix mtx, string tableID, List<string> columns) {

            this.UIAPIRawForm.Freeze(true);
            columns.Add("Result");
            Parallel.ForEach(columns, column => {
                mtx.Columns.Item("C_" + column).DataBind.Bind(tableID, "C_" + column);
            });

            mtx.LoadFromDataSource();
            mtx.AutoResizeColumns();
            this.UIAPIRawForm.Freeze(false);
        }

        public void updateDataTable(string fieldName) {

            if(changedRow > 0) {
                string column = (fieldName == "C_Food") ? "C_Food" : "C_Bags";
                oEdit = (SAPbouiCOM.EditText)mtx0.Columns.Item(fieldName).Cells.Item(changedRow).Specific;
                dt0.SetValue(column, mIntRowId - 1, oEdit.Value);
            }
        }

        #endregion

        #region ChooseFromList
        /// <summary>
        /// Fill choose from list.
        /// </summary>
        private void LoadChoosesFromList() {

            SAPbouiCOM.ChooseFromList lObjCFLItem = InitChooseFromLists(false, "4", "CFL_Item", this.UIAPIRawForm.ChooseFromLists);
            AddConditionCFL(lObjCFLItem, "CFL_Item");
            SAPbouiCOM.ChooseFromList lObjCFLWhs = InitChooseFromLists(false, "64", "CFL_Whs", this.UIAPIRawForm.ChooseFromLists);
            AddConditionCFL(lObjCFLWhs, "CFL_Whs");
            SetChooseFromListToTxt();
        }

        public SAPbouiCOM.ChooseFromList InitChooseFromLists(bool pbol, string pStrType, string pStrID, SAPbouiCOM.ChooseFromListCollection pObjCFLs) //
        {
            SAPbouiCOM.ChooseFromList lObjoCFL = null;
            try {
                SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
                oCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)UIApplication.GetApplication().CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);

                oCFLCreationParams.MultiSelection = pbol;
                oCFLCreationParams.ObjectType = pStrType;
                oCFLCreationParams.UniqueID = pStrID;

                lObjoCFL = pObjCFLs.Add(oCFLCreationParams);

                this.UIAPIRawForm.DataSources.UserDataSources.Add(pStrID, SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
            }
            catch(Exception ex) {
                HandleException(ex, "InitCustomerChooseFromListException");

            }
            return lObjoCFL;
        }

        private void SetChooseFromListToTxt() {
            txtItem.DataBind.SetBound(true, "", "CFL_Item");
            txtItem.ChooseFromListUID = "CFL_Item";

            txtCorral.DataBind.SetBound(true, "", "CFL_Whs");
            txtCorral.ChooseFromListUID = "CFL_Whs";
        }
        #endregion

        #region ChooseFromListAfterEvent
        private void ChooseFromListAfterEvent(SAPbouiCOM.ItemEvent pObjValEvent) {

            if(pObjValEvent.Action_Success) {
                SAPbouiCOM.IChooseFromListEvent lObjCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pObjValEvent;
                SAPbouiCOM.DataTable lObjDataTable = lObjCFLEvento.SelectedObjects;
                if(lObjCFLEvento.SelectedObjects == null)
                    return;

                if(lObjDataTable.UniqueID == "CFL_Item") {

                    txtCode.Value = Convert.ToString(lObjDataTable.GetValue(0, 0));
                    this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Item").ValueEx = Convert.ToString(lObjDataTable.GetValue(1, 0));
                    requireBagsField = lObjDistributionDAO.GetIsBagRequired(txtCode.Value);
                }
                else if(lObjDataTable.UniqueID == "CFL_Whs") {

                    this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_Whs").ValueEx = Convert.ToString(lObjDataTable.GetValue(0, 0));
                }

            }
        }
        #endregion

        #region ChooseFromListCondition

        private void AddConditionCFL(SAPbouiCOM.ChooseFromList pCFL, string pCFLID) {

            SAPbouiCOM.Condition lObjCon = null;
            SAPbouiCOM.Conditions lObjCons = new SAPbouiCOM.Conditions();

            if(pCFLID == "CFL_Whs") {
                lObjCon = lObjCons.Add();
                lObjCon.Alias = "location";
                lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                lObjCon.CondVal = "1";

            }
            else if(pCFLID == "CFL_Item") {

                lDicNonLockedItems = lObjDistributionDAO.GetNonLockeditems();

                try {
                    int i = 1;
                    if(lDicNonLockedItems.Count() > 0) {
                        foreach(string itemCode in lDicNonLockedItems.Keys) {

                            lObjCon = lObjCons.Add();
                            lObjCon.Alias = "ItemCode";
                            lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                            lObjCon.CondVal = itemCode;

                            if(lDicNonLockedItems.Count() > i) {
                                lObjCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_OR;
                            }
                            i++;
                        }
                    }

                    pCFL.SetConditions(lObjCons);
                }
                catch(Exception ex) {
                    HandleException(ex, "AddConditionChoseFromList");
                }
            }

            pCFL.SetConditions(lObjCons);
        }


        #endregion

        #region EventsHandle
        public void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent) {
            BubbleEvent = true;

            try {
                if(pVal.BeforeAction) {
                    switch(pVal.MenuUID) {
                        case "1282":
                        btnAdd.Item.Enabled = true;
                        txtCode.Value = String.Empty;
                        txtItem.Value = String.Empty;

                        break;
                    }
                }
            }
            catch(Exception ex) {
                HandleException(ex, "SBO_Application_MenuEvent");
            }
        }

        /// <summary>
        /// SBO_Application_ItemEvent
        /// Metodo para controlar los eventos de la pantalla.
        /// @Author FranciscoFimbres
        /// </summary>
        /// <param name="FormUID"></param>
        /// <param name="pVal"></param>
        /// <param name="BubbleEvent"></param>
        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent) {
            BubbleEvent = true;

            try {

                if(FormUID.Equals(this.UIAPIRawForm.UniqueID)) {

                    if(!pVal.BeforeAction) {
                        switch(pVal.EventType) {
                            case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST:
                            ChooseFromListAfterEvent(pVal);
                            break;
                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                            UnLoadEvents();
                            break;
                            case SAPbouiCOM.BoEventTypes.et_LOST_FOCUS:
                            if(pVal.ColUID == "C_Food") {

                                updateDataTable("C_Food");
                                txtKilos.Value = CalculateTotals("C_Food");

                            }
                            else if(pVal.ColUID == "C_Bags") {

                                updateDataTable("C_Bags");
                                txtBags.Value = CalculateTotals("C_Bags");
                            }
                            break;
                        }
                    }
                }
                else if(String.IsNullOrEmpty(pVal.ItemUID) && pVal.FormType == 10000075) {


                    if(this.UIAPIRawForm.Selected) {

                        switch(pVal.EventType) {

                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:

                            if(pVal.ActionSuccess) {
                                ValidateDate();
                            }
                            break;
                        }
                    }
                }
            }
            catch(Exception ex) {
                if(!ex.Message.Contains("Form - Invalid Form"))
                    SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(ex.Message);
            }
        }
        #endregion

        #region Calculate Totals
        /// <summary>
        /// Calculates in Parallel the total amount for a specific column.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string CalculateTotals(string fieldName) {

            double total = 0.0;
            object monitor = new object();

            try {
                Parallel.ForEach(Partitioner.Create(1, mtx0.RowCount + 1), () => 0.0, (range, state, local) => {
                    for(int i = range.Item1; i < range.Item2; i++) {

                        local += Convert.ToDouble(((SAPbouiCOM.EditText)mtx0.Columns.Item(fieldName).Cells.Item(i).Specific).Value);
                    }
                    return local;
                }, local => { lock(monitor) total += local; });
            }
            catch(AggregateException ae) {
                ae.Handle(e => {
                    HandleException(e, "CalculateTotals");
                    return true;
                });
            }

            return total.ToString();
        }
        #endregion

        #region Validates
        /// <summary>
        /// Validate the selectes Date by accepting only 3 days before range
        /// </summary>
        public void ValidateDate() {

            DateTime selectedDate = DateTime.ParseExact(txtDate.Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            DateTime compareDate = DateTime.Now.AddDays(-3);
            double diff = Math.Ceiling((selectedDate - compareDate).TotalDays);

            if(!(diff <= 3 && diff >= 0)) {
                txtDate.Value = DateTime.Now.ToString("yyyyMMdd"); ;
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Solo se aceptan 3 días hacia atras como máximo a partir de la fecha actual");
                return;
            }
            else {
                FillMatrix();
                btnAdd.Item.Enabled = true;
            }
        }

        private void mtx0_ValidateAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal) {
            changedRow = pVal.Row;
            SAPbouiCOM.EditText lObjTxt = (SAPbouiCOM.EditText)mtx0.Columns.Item("C_#").Cells.Item(pVal.Row).Specific;
            mIntRowId = Convert.ToInt32(dt0.GetValue("C_#", Convert.ToInt32(lObjTxt.Value) - 1).ToString());
        }


        /// <summary>
        /// Validate Field Bags if the Item require it by OITM.QryGroup29 == 'Y'
        /// </summary>
        private bool ValidateBagsFields() {


            var validatedRows = new List<int>();
            if(requireBagsField) {
                for(int i = 0; i < dt0.Rows.Count; i++) {

                    if(String.IsNullOrEmpty(dt0.GetValue("C_Food", i).ToString()) || dt0.GetValue("C_Food", i).ToString().Equals("0")) {
                        continue;
                    }
                    else if(dt0.GetValue("C_Bags", i).ToString().Equals("0")) {
                        validatedRows.Add(i + 1);
                    }
                }

                if(validatedRows.Count > 0) {
                    SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(String.Format("Se require el campo Sacos-Pacas en lineas: {0}", String.Join(",", validatedRows)));
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Click Events
        private void btnCancel_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            this.UIAPIRawForm.Close();

        }

        private void btnNew_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            Clear();
        }

        private void btnSearch_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent) {

            BubbleEvent = false;
            FillMatrix();
            btnAdd.Item.Enabled = true;
        }

        /// <summary>
        ///  Button Add to store Delivery Records on SAP B1 by selected Item and Matrix Rows
        /// </summary>
        /// <param name="sboObject"></param>
        /// <param name="pVal"></param>
        /// <param name="BubbleEvent"></param>
        private void btnAdd_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent) {

            BubbleEvent = true;


            txtCode.Active = true;

            if(btnAdd.Item.Enabled == false) {
                return;
            }

            if(dt0.Rows.Count <= 0) {
                return;
            }

            if(String.IsNullOrEmpty(txtCode.Value)) {
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("No se ha seleccionado Artículo");
                return;
            }

            if(!ValidateBagsFields()) {
                return;
            }

            LogService.WriteInfo("Begin Creating Deliveries");

            var currentClient = dt0.GetValue(1, 0).ToString();
            var deliveryDTO = new DeliveryDTO();
            DeliveryLines deliveryLine = null;
            var results = new List<Task>();
            var lastClient = String.Empty;
            var nextClient = String.Empty;
            var nextClientValue = String.Empty;
            var lastClientValue = String.Empty;
            var processed = false;

            try {
                      
                for(int i = 0; i < dt0.Rows.Count; i++) {
                    int lIntRowId = Convert.ToInt32(dt0.GetValue("C_#", i).ToString());
                    lIntRowId--;
                    var ss = dt0.GetValue("C_Food", lIntRowId).ToString();
                    if (String.IsNullOrEmpty(dt0.GetValue("C_Food", lIntRowId).ToString()) || dt0.GetValue("C_Food", lIntRowId).ToString().Equals("0"))
                    {
                        continue;
                    }

                    currentClient = dt0.GetValue("C_Code", lIntRowId).ToString();
                    lastClient = (lIntRowId == 0) ? String.Empty : dt0.GetValue("C_Code", lIntRowId - 1).ToString();
                    lastClientValue = (lIntRowId == 0) ? String.Empty : dt0.GetValue("C_Food", lIntRowId - 1).ToString();
                    nextClient = (lIntRowId < dt0.Rows.Count - 1) ? dt0.GetValue("C_Code", lIntRowId + 1).ToString() : String.Empty;
                    nextClientValue = (lIntRowId < dt0.Rows.Count - 1) ? dt0.GetValue("C_Food", lIntRowId + 1).ToString() : String.Empty;

                    if(currentClient != lastClient || (currentClient == lastClient && lastClientValue == "0")) {
                        deliveryDTO = new DeliveryDTO();
                        deliveryDTO.CardCode = dt0.GetValue("C_Code", lIntRowId).ToString();
                        deliveryDTO.CardName = dt0.GetValue("C_Name", lIntRowId).ToString();
                        deliveryDTO.DocDate = DateTime.ParseExact(txtDate.Value, "yyyyMMdd", CultureInfo.InvariantCulture);
                        deliveryDTO.Series = series;
                    }

                    deliveryLine = new DeliveryLines();
                    deliveryLine.ItemCode = txtCode.Value;
                    deliveryLine.Dscription = txtItem.Value;
                    deliveryLine.WhsCode = defaultUserWhs;
                    deliveryLine.Corral = dt0.GetValue("C_Whs", lIntRowId).ToString();
                    deliveryLine.Quantity = (double)dt0.GetValue("C_Food", lIntRowId);
                    deliveryLine.BagsBales = (double)dt0.GetValue("C_Bags", lIntRowId);
                    deliveryLine.Area = defaultUserArea;
                    deliveryLine.Price = lDicNonLockedItems[txtCode.Value];
                    deliveryDTO.DocLines.Add(deliveryLine);

                    if(currentClient != lastClient || currentClient == lastClient) {
                        if(nextClient == currentClient && nextClientValue != "0") {
                            continue;
                        }
                        else {
                            processed = true;
                            var result = DeliveryDI.CreateDelivery(deliveryDTO);

                            for(int j = 0; j < dt0.Rows.Count; j++) {
                                int lIntRowIdJ = Convert.ToInt32(dt0.GetValue("C_#", j).ToString());
                                if (dt0.GetValue("C_Code", lIntRowIdJ - 1).ToString() == currentClient && !dt0.GetValue("C_Food", lIntRowIdJ - 1).ToString().Equals("0"))
                                {
                                    dt0.SetValue("C_Result", lIntRowIdJ - 1, result.Message);
                                    dt0.SetValue("C_Deliv", lIntRowIdJ - 1, (result.Success ? "SI" : "NO"));
                                }
                            }
                        }
                    }
                }
            }
            catch(AggregateException ae) {
                ae.Handle(e => {
                    HandleException(e, "Concurrent Adding Delivery");
                    return true;
                });

                return;
            }
            catch(Exception e) {
                 HandleException(e, "Adding Delivery");
                return;
            }

            if(processed) {
                this.UIAPIRawForm.Freeze(true);
                mtx0.Columns.Item("C_Result").Visible = true;
                mtx0.LoadFromDataSource();
                mtx0.AutoResizeColumns();
                this.UIAPIRawForm.Freeze(false);

                UIApplication.ShowMessageBox("Revisar la Columna de Resultados");
                btnAdd.Item.Enabled = false;
                LogService.WriteInfo("Done Creating Deliveries");
            }
        }

        #endregion

        #region Clear Form
        public void Clear() {

            txtItem.Value = "";
            txtCode.Value = "";
            txtCorral.Value = "";
            txtKilos.Value = "";
            txtBags.Value = "";
            txtDate.Value = DateTime.Now.ToString("yyyyMMdd");
            mtx0.Columns.Item("C_Result").Visible = false;
            this.UIAPIRawForm.Width = 760;

            dt0.Rows.Clear();

            this.UIAPIRawForm.Freeze(true);
            mtx0.LoadFromDataSource();
            this.UIAPIRawForm.Freeze(false);
        }
        #endregion

        #region Handle Exception

        public void HandleException(Exception ex, string section) {

            UIApplication.ShowMessageBox(ex.Message);
            LogService.WriteError(String.Format("{0}: {1}, {2}", section, ex.Message, ex.StackTrace));
        }
        #endregion

        #region Controls
        private SAPbouiCOM.Button btnAdd;
        private SAPbouiCOM.Button btnCancel;
        private SAPbouiCOM.StaticText lblItem;
        private SAPbouiCOM.EditText txtCode;
        private SAPbouiCOM.StaticText lblCorral;
        private SAPbouiCOM.Button btnSearch;
        private SAPbouiCOM.EditText txtCorral;
        private SAPbouiCOM.StaticText lblTotal;
        private SAPbouiCOM.EditText txtKilos;
        private SAPbouiCOM.EditText txtBags;
        private SAPbouiCOM.StaticText lblDate;
        private SAPbouiCOM.EditText txtDate;
        private SAPbouiCOM.Matrix mtx0;
        private SAPbouiCOM.EditText txtItem;
        private SAPbouiCOM.DataTable dt0;
        private SAPbouiCOM.EditText oEdit;
        private SAPbouiCOM.Button btnNew;
        #endregion


    }
}