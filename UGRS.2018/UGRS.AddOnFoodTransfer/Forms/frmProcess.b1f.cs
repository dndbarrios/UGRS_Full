/*
Autor: LCC Abraham Saúl Sandoval Meneses
Description: Production Process Form
Date: 10/09/2018
Company: Qualisys
*/

using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.AddOnFoodTransfer.Utils;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.FoodTransfer.DAO;
using UGRS.Core.SDK.DI.FoodTransfer.DTO;
using UGRS.Core.SDK.DI.FoodTransfer.Services;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;

namespace UGRS.AddOnFoodTransfer.Forms {
    [FormAttribute("UGRS.AddOnFoodTransfer.Forms.frmProcess", "Forms/frmProcess.b1f")]
    class frmProcess : UserFormBase {

        #region Properties
        Dictionary<string, BoFieldsType> columns = null;
        FoodTransferDAO foodTransferDAO = new FoodTransferDAO();
        Component[] components = null;
        User user = null;
        Component productionLine = new Component();
        int docEntry = 0;
        double plannedQty = 0;
        #endregion

        #region Constructor
        public frmProcess() {

            try {
                user = new User("");
                user.FormID = this.UIAPIRawForm.UniqueID;
                EnableButtons(false);
                Parallel.Invoke(PrepareMatrix, LoadEvents, user.IsFoodPlant == true ? new Action(InitFoodPlant) : new Action(InitQuarantine));
                LogService.WriteInfo("AddOn User: " + user.Name);

            }
            catch (AggregateException ae) {
                ae.Handle(e => {
                    SAPException.Handle(e, "frmProcess");
                    return true;
                });
            }
        }
        #endregion

        #region Matrix
        private void PrepareMatrix() {
            columns = new Dictionary<string, BoFieldsType>() { { "#", BoFieldsType.ft_ShortNumber }, { "Item", BoFieldsType.ft_AlphaNumeric }, { "Desc", BoFieldsType.ft_AlphaNumeric }, { "Whs", BoFieldsType.ft_AlphaNumeric }, { "Exist", BoFieldsType.ft_Quantity }, { "Plan", BoFieldsType.ft_Quantity }, { "Qty", BoFieldsType.ft_Quantity }, { "Consumed", BoFieldsType.ft_Quantity }, { "Bags", BoFieldsType.ft_Float } };
            dt0 = SAPMatrix.CreateDataTable("dt0", columns, this);
            mtx0.Columns.Item("C_Consumed").Visible = false;
            mtx0.AutoResizeColumns();
        }

        private void FillMatrix() {
            this.UIAPIRawForm.Freeze(true);
            SAPMatrix.Fill("dt0", dt0, mtx0, columns.Keys.ToList(), components);
            this.UIAPIRawForm.Freeze(false);
        }

        public void ClearMatrix() {

            txtBags.Value = String.Empty;
            txtItem.Value = String.Empty;
            txtDate.Value = String.Empty;
            txtQDif.Value = String.Empty;
            txtQPlan.Value = String.Empty;
            txtQReal.Value = String.Empty;
            txtFolio.Value = String.Empty;

            this.UIAPIRawForm.Freeze(true);
            SAPMatrix.ClearMtx(mtx0);
            this.UIAPIRawForm.Freeze(false);

            components = null;
            productionLine = new Component();
            docEntry = 0;
            plannedQty = 0;

            EnableButtons(false);
        }
        #endregion

        #region ChooseFromList
        private void LoadChooseFromList(string id, string code, Dictionary<string, string> conditions, EditText txt) {

            ChooseFromList oCFLFolio = SAPChooseFromList.Init(false, code, id, this);
            if (!Object.ReferenceEquals(conditions, null) && conditions.Count > 0) {
                SAPChooseFromList.AddConditions(oCFLFolio, conditions);
            }
            //bind choose from list to editText
            SAPChooseFromList.Bind(id, txt);
        }

        private void LoadChooseFromList2(string id, string code, Dictionary<string, string> conditions, EditText txt) {

            ChooseFromList oCFLFolio = SAPChooseFromList.Init(false, code, id, this);
            if (!Object.ReferenceEquals(conditions, null) && conditions.Count > 0) {
                SAPChooseFromList.AddConditions2(oCFLFolio, conditions);
            }
            //bind choose from list to editText
            SAPChooseFromList.Bind(id, txt);
        }
        #endregion

        #region Events
        public void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent) {
            BubbleEvent = true;

            try {
                if (pVal.BeforeAction) {
                    switch (pVal.MenuUID) {
                        case "1282":    //System SAP B1 Create Button Menu
                            ClearMatrix();
                            break;
                    }
                }
            }
            catch (Exception ex) {

                UIApplication.ShowMessageBox(ex.Message);
            }
        }

        private void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            try {
                if (FormUID.Equals(this.UIAPIRawForm.UniqueID)) {
                    if (!pVal.BeforeAction) {
                        switch (pVal.EventType) {
                            case BoEventTypes.et_CHOOSE_FROM_LIST:
                                try {
                                    if (String.IsNullOrEmpty(SAPChooseFromList.GetValue(pVal, 0)))
                                        return;

                                    txtBags.Item.Enabled = chkStatus.Checked ? false : true;
                                    if (user.IsFoodPlant) {
                                        //FOOD PLANT----------------------------
                                        btnCreate.Item.Enabled = !chkStatus.Checked ? true : false;
                                        btnCancel.Item.Enabled = chkStatus.Checked ? true : false;

                                        docEntry = int.Parse(SAPChooseFromList.GetValue(pVal, 0));
                                        txtItem.Value = SAPChooseFromList.GetValue(pVal, 48);
                                        txtDate.Value = Convert.ToDateTime(SAPChooseFromList.GetValue(pVal, 9).Substring(0, 10)).ToString("yyyyMMdd");

                                        this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_PO").ValueEx = SAPChooseFromList.GetValue(pVal, 1);
                                        SetProductionItem(SAPChooseFromList.GetValue(pVal, 3), txtItem.Value, Double.Parse(SAPChooseFromList.GetValue(pVal, 7)), !chkStatus.Checked ? 0 : foodTransferDAO.GetProdItemBags(docEntry));
                                        components = foodTransferDAO.GetComponents(SAPChooseFromList.GetValue(pVal, 0), user.WhsCode, false);
                                        LogService.WriteInfo("ProductionOrder - DocEntry: " + docEntry.ToString() + " - " + "ComponentsCount: " + components.Length.ToString());

                                        plannedQty = GetPlannedQty(docEntry);
                                        txtQPlan.Value = plannedQty.ToString();

                                        if (chkStatus.Checked && productionLine.Bags > 0) {
                                            txtBags.Value = productionLine.Bags.ToString("#.####");
                                        }
                                        else if (chkStatus.Checked && productionLine.Bags <= 0) {
                                            txtBags.Value = "0.0000";
                                        }

                                        Task.Run(() => {
                                            FillMatrix();
                                            CalculateQuantities(components, plannedQty);
                                        });

                                    }
                                    else {//QUARANTINE--------------------------
                                          //Material List
                                        if (pVal.ItemUID == "txtItem") {

                                            btnCreate.Item.Enabled = true;
                                            this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_ML").ValueEx = SAPChooseFromList.GetValue(pVal, 24);
                                            SetProductionItem(SAPChooseFromList.GetValue(pVal, 0), txtItem.Value, Double.Parse(SAPChooseFromList.GetValue(pVal, 3)), 0);
                                            components = foodTransferDAO.GetComponents(SAPChooseFromList.GetValue(pVal, 0), user.WhsCode, chkStatus.Checked);
                                            LogService.WriteInfo("MaterialList - DocEntry: " + docEntry.ToString() + " - " + "ComponentsCount: " + components.Length.ToString());
                                        }
                                        //Stock Entry
                                        else if (pVal.ItemUID == "txtFolio") {

                                            btnCancel.Item.Enabled = true;
                                            docEntry = int.Parse(SAPChooseFromList.GetValue(pVal, 0));
                                            txtDate.Value = Convert.ToDateTime(SAPChooseFromList.GetValue(pVal, 10).Substring(0, 10)).ToString("yyyyMMdd");
                                            this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_SE").ValueEx = SAPChooseFromList.GetValue(pVal, 1);
                                            productionLine = foodTransferDAO.GetProductionItem(docEntry, user.WhsCode);
                                            txtItem.Value = productionLine.Desc;

                                            components = foodTransferDAO.GetComponents(SAPChooseFromList.GetValue(pVal, "U_MQ_OrigenFol"), user.WhsCode, chkStatus.Checked);
                                            LogService.WriteInfo("StockEntry - DocEntry: " + docEntry.ToString() + " - " + "ComponentsCount: " + components.Length.ToString());
                                        }

                                        Task.Run(() => {
                                            FillMatrix();
                                            txtQReal.Value = !chkStatus.Checked ? SAPMatrix.SumColumnQuantities(mtx0, "C_Qty", components).ToString() : productionLine.Qty.ToString();
                                        });
                                    }
                                }
                                catch (Exception ex) {
                                    SAPException.Handle(ex, "SBO_Application_ItemEvent");
                                }
                                break;

                            case SAPbouiCOM.BoEventTypes.et_LOST_FOCUS:
                                if (pVal.ColUID == "C_Qty") {
                                    if (user.IsFoodPlant) {
                                        CalculateQuantities(components, plannedQty);
                                    }
                                    else {
                                        txtQReal.Value = SAPMatrix.SumColumnQuantities(mtx0, pVal.ColUID, components).ToString();
                                    }
                                }
                                break;
                        }
                    }
                }//Event for date picker
                else if (String.IsNullOrEmpty(pVal.ItemUID) && pVal.FormType == 10000075 && !user.IsFoodPlant) {
                    if (this.UIAPIRawForm.Selected) {
                        switch (pVal.EventType) {
                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                if (pVal.ActionSuccess) {
                                    SAPDate.ValidateDate(txtDate, 3);
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex) {
                SAPException.Handle(ex, "SBO_Application_ItemEvent");
            }
        }

        private void btnCancel_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            if (btnCancel.Item.Enabled) {


                SAPMatrix.SetColumnQuantities(mtx0, "Consumed", components, "Qty");
                CreateDocuments(true);
            }
        }

        private void btnCreate_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;

            if (btnCreate.Item.Enabled) {

                SAPMatrix.SetColumnQuantities(mtx0, "Qty", components, "Qty");
                if (components.Where(l => l.Qty == 0).Count() == components.Length)
                    return;

                CreateDocuments(false);
            }
        }

        private void btnExit_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            this.UIAPIRawForm.Close();
        }

        public override void OnInitializeFormEvents() {
            this.UnloadAfter += new UnloadAfterHandler(this.Form_UnloadAfter);
        }

        private void OnCustomInitialize() { }

        private void LoadEvents() {
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent += new _IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
            SAPbouiCOM.Framework.Application.SBO_Application.MenuEvent += new _IApplicationEvents_MenuEventEventHandler(SBO_Application_MenuEvent);
        }

        private void UnLoadEvents() {
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent -= new _IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
            SAPbouiCOM.Framework.Application.SBO_Application.MenuEvent -= new _IApplicationEvents_MenuEventEventHandler(SBO_Application_MenuEvent);
        }

        private void Form_UnloadAfter(SBOItemEventArg pVal) {
            UnLoadEvents();
        }

        private void chkStatus_ClickAfter(object sboObject, SBOItemEventArg pVal) {

            try {
                var checkBox = (CheckBox)sboObject;

                //FOOD PLANT ----------------------------------------------------------------------------------------
                if (user.IsFoodPlant) {
                    if (!checkBox.Checked) {
                        SAPChooseFromList.AddConditionValues(this.UIAPIRawForm.ChooseFromLists.Item("CFL_PO"), "DocNum", foodTransferDAO.GetCancelledOrders(user.WhsCode));
                    }
                    else {
                        SAPChooseFromList.AddConditions(this.UIAPIRawForm.ChooseFromLists.Item("CFL_PO"), new Dictionary<string, string>() { { "Type", "S" }, { "Status", "R" }, { "Warehouse", "PLHE" } });
                    }
                }
                //QUARANTINE ----------------------------------------------------------------------------------------
                else {
                    txtBags.Active = true;
                    //Stock Entry's
                    if (!checkBox.Checked) {
                        txtFolio.Item.Enabled = true;
                        txtItem.Item.Enabled = false;

                        if (this.UIAPIRawForm.ChooseFromLists.Count < 3) {
                            LoadChooseFromList2("CFL_SE", "59", new Dictionary<string, string>() { { "Series", foodTransferDAO.GetSeries(user.WhsCode, "59", "Series").ToString() }, { "U_GLO_InMo", "E-PROD" }, { "U_GLO_Status", "O" } }, txtFolio);
                        }
                        else {
                            SAPChooseFromList.Bind("CFL_SE", txtFolio);
                        }
                        txtFolio.Active = true;
                        btnCreate.Item.Enabled = false;
                    }
                    //Material List's
                    else {
                        txtFolio.Item.Enabled = false;
                        txtItem.Item.Enabled = true;
                        SAPChooseFromList.Bind("CFL_ML", txtItem);
                        txtItem.Active = true;
                    }
                }
                //--------------------------------------------------------------------------------------------------
                txtFolio.Active = true;
                mtx0.Columns.Item("C_Consumed").Visible = !checkBox.Checked ? true : false;
                mtx0.Columns.Item("C_Qty").Visible = !checkBox.Checked ? false : true;
                mtx0.Columns.Item("C_Bags").Editable = !checkBox.Checked ? false : true;
                ClearMatrix();

                if (!user.IsFoodPlant && checkBox.Checked) {
                    txtFolio.Value = foodTransferDAO.GetSeries(user.WhsCode, "59", "NextNumber").ToString();
                }
            }
            catch (Exception ex) {
                SAPException.Handle(ex, "chkStatus_ClickAfter");
            }
        }
        #endregion

        #region Create Documents
        private void CreateDocuments(bool cancellation) {
            try {

                //Validations
                if (!ValidateColumn("Existence") || !ValidateColumn("Emision"))
                    return;

                if (!cancellation && !ValidateColumn("BagBales"))

                    return;

                var prodDate = !String.IsNullOrEmpty(txtDate.Value) ? SAPDate.ParseDate(txtDate.Value) : DateTime.Now;

                //Setting the Bags/Bales values and Real Quantity 
                SetBagsBalesAndRealQuantity();

                //Transaction
                BeginTransaction();

                //Invetory Exit
                var task = Task.Factory.StartNew(() => {
                    var exit = new DocumentProduction();
                    exit.Lines = !cancellation ? components : new Component[1] { productionLine };
                    exit.DocEntry = docEntry;
                    exit.DocNum = txtFolio.Value;
                    return StockExitDI.CreateDocument(exit, user, cancellation, prodDate);
                });
                task.Wait();

                if (!user.IsFoodPlant && !cancellation) {
                    productionLine.LineTotal = task.Result.DocTotal;
                }

                //Inventory Entry 
                var task2 = Task.Factory.StartNew(() => {
                    if (!cancellation) {
                        productionLine.Qty = components.Where(c => c.Prod.Equals(0)).Sum(s => s.Qty);
                    }
                    var entry = new DocumentProduction();
                    entry.Lines = !cancellation ? new Component[1] { productionLine } : components;
                    entry.DocEntry = docEntry;
                    entry.DocNum = txtFolio.Value;
                    return StockEntryDI.CreateDocument(entry, user, cancellation, prodDate);
                });

                var task3 = Task.Factory.ContinueWhenAll(new[] { task, task2 }, _ => {

                    var resultMessage = (!cancellation) ? new StringBuilder()
                                                         .Append("Entrada de Producto Terminado: ").AppendLine(task2.Result.Message)
                                                         .Append("Salida de Componentes: ").AppendLine(task.Result.Message)
                                                         : new StringBuilder()
                                                         .Append("Salida de Producto Terminado: ").AppendLine(task.Result.Message)
                                                         .Append("Entrada de Componentes: ").AppendLine(task2.Result.Message);

                    if (task.Result.Success && task2.Result.Success) {

                        if (user.IsFoodPlant && !cancellation) {
                            //Update Status to Closed for Production Order Document
                            var result = ProductionOrderDI.CancelDocument(docEntry, task2.Result.DocEntry, task.Result.DocEntry);
                            resultMessage.Append("Orden de Fabricación: ").AppendLine(result.Message);
                        }

                        if (cancellation) {
                            var result = InventoryRevaluationDI.CreateDocument(productionLine, user, task.Result.DocEntry, docEntry);
                            if (result.Success) {
                                resultMessage.Append("Revalorización de Inventario: ").AppendLine(result.Message);
                            }
                        }
                        if (StockEntryDI.UpdateDocument(task2.Result.DocEntry, task.Result.DocEntry, cancellation)) {
                            Commit();
                        }
                        else {
                            RollBack(resultMessage.ToString());
                        }
                    }
                    else {
                        RollBack(resultMessage.ToString());
                    }

                    UIApplication.ShowMessageBox(resultMessage.ToString());
                    ClearMatrix();

                    if (user.IsFoodPlant && cancellation) {
                        SAPChooseFromList.AddConditionValues(this.UIAPIRawForm.ChooseFromLists.Item("CFL_PO"), "DocNum", foodTransferDAO.GetCancelledOrders(user.WhsCode));
                    }
                    else if (!user.IsFoodPlant && !cancellation) {
                        txtFolio.Value = foodTransferDAO.GetSeries(user.WhsCode, "59", "NextNumber").ToString(); //OIGN
                    }
                });
            }
            catch (Exception ex) {
                SAPException.Handle(ex, "CreateDocuments");
            }
        }


        public void BeginTransaction() {
            DIApplication.Company.StartTransaction();
        }

        public void RollBack(string message) {
            UIApplication.ShowMessageBox(message);
            UIApplication.ShowMessageBox("Se produjo un error. Todos los cambios anteriores fueron revertidos");
            try {
                DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
            }
            catch (Exception ex) {
                UIApplication.ShowMessageBox(ex.Message);
            }
        }

        public void Commit() {
            try {
                DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
            }
            catch (Exception ex) {
                RollBack(ex.Message);
            }
        }
        #endregion

        #region Other Methods
        private void SetBagsBalesAndRealQuantity() {

            SAPMatrix.SetColumnQuantities(mtx0, "Bags", components, "Bags");

            if (!chkStatus.Checked && !String.IsNullOrEmpty(txtBags.Value)) {
                productionLine.Bags = Double.Parse(txtBags.Value);
            }

            productionLine.Qty = Double.Parse(txtQReal.Value);
        }

        private double CalculateConsumedQty(Component[] components) {
            return components.Where(c => c.Prod.Equals(0)).Select(c => c.Consumed).Aggregate(0.0, (total, subtotal) => total += subtotal, i => i);
        }

        private double GetPlannedQty(int docEntry) {
            return foodTransferDAO.GetPlannedQty(docEntry.ToString());
        }

        private double CalculateDifferenceQty(double real, double planned) {
            return real - planned;
        }

        private void CalculateQuantities(Component[] components, double plannedQty) {

            if (!Object.ReferenceEquals(components, null) && components.Length > 0) {
                var realQty = !chkStatus.Checked ? SAPMatrix.SumColumnQuantities(mtx0, "C_Qty", components) : CalculateConsumedQty(components);
                txtQReal.Value = !chkStatus.Checked ? realQty.ToString() : productionLine.Qty.ToString();
                txtQDif.Value = CalculateDifferenceQty(realQty, plannedQty).ToString("#.####");
            }
        }

        private void SetProductionItem(string item, string desctiption, double qty, double bags) {
            productionLine.Item = item;
            productionLine.Desc = desctiption;
            productionLine.Qty = qty;
            productionLine.Whs = user.WhsCode;
            productionLine.AccCode = foodTransferDAO.GetAccountCode(item, user.WhsCode);
            productionLine.BagsRequired = foodTransferDAO.GetIsBagRequired(item);
            productionLine.Exist = foodTransferDAO.GetItemStock(item, user.WhsCode);
            productionLine.Bags = bags;
        }

        public void EnableButtons(bool enabled) {
            btnCancel.Item.Enabled = enabled;
            btnCreate.Item.Enabled = enabled;
        }

        private void InitFoodPlant() {
            LoadChooseFromList("CFL_PO", "202", new Dictionary<string, string>() { { "Type", "S" }, { "Status", "R" }, { "Warehouse", "PLHE" } }, txtFolio);
        }

        private void InitQuarantine() {
            lblFolio.Caption = "Folio Entradas";
            txtItem.Item.Enabled = true;
            txtDate.Item.Enabled = true;
            txtQPlan.Item.Visible = false;
            txtQDif.Item.Visible = false;
            lblQDif.Item.Visible = false;
            lblQPlan.Item.Visible = false;
            txtDate.Value = DateTime.Now.ToString("yyyyMMdd");
            LoadChooseFromList("CFL_ML", "66", new Dictionary<string, string>() { { "U_CU_Check", "Y" } }, txtItem);
            txtFolio.Item.Enabled = false;
            txtFolio.Value = foodTransferDAO.GetSeries(user.WhsCode, "59", "NextNumber").ToString(); //OIGN
            chkStatus.Caption = "Cancelar Entradas";
        }
        #endregion

        #region Validations

        private bool ValidateColumn(string column) {

            if (column.Equals("BagBales") && productionLine.BagsRequired.Equals(1) && (txtBags.Value == "" || txtBags.Value == "0.0")) {
                UIApplication.ShowMessageBox(String.Format("Se require el campo Sacos-Pacas en el artículo de producción: {0}", txtItem.Value));
                return false;
            }
            else if (column.Equals("Existence") && chkStatus.Checked && !ValidateItemStock(productionLine)) {
                UIApplication.ShowMessageBox(String.Format("El artículo de Producción {0} no tiene stock suficiente en el almacen {1}", productionLine.Desc, user.WhsCode));
                return false;
            }

            if (!Task.Factory.StartNew(() => {
                return Parallel.ForEach(Partitioner.Create(1, components.Length + 1), (range, state) => {
                    for (int i = range.Item1; i < range.Item2; i++) {
                        if (column.Equals("Existence") && !chkStatus.Checked) {
                            if (components[i - 1].Exist < components[i - 1].Qty && components[i - 1].Resource == 0 && components[i - 1].Inventorial.Equals(0)) {
                                state.Stop();
                                UIApplication.ShowMessageBox((String.Format("Se require la cantidad {0} sea menor que la exitencia {1} para el artículo {2} en la linea {3}", components[i - 1].Qty, components[i - 1].Exist, components[i - 1].Desc, i)));
                            }

                            if (components[i - 1].Qty <= 0 && components[i - 1].Inventorial.Equals(0)) {
                                state.Stop();
                                UIApplication.ShowMessageBox((String.Format("La cantidad tiene que ser mayor que 0 para el artículo {0} en la linea: {1}", components[i - 1].Item, i)));
                            }
                        }
                        else if (column.Equals("Emision")) {
                            if (components[i - 1].Method.Equals("B") && components[i - 1].Inventorial.Equals(0)) {
                                state.Stop();
                                UIApplication.ShowMessageBox((String.Format("El método de emisión del artículo {0} es Notificación, se require que se cambié a Manual en la linea: {1}", components[i - 1].Item, i)));
                            }
                        }
                        else {
                            var value = ((EditText)mtx0.Columns.Item("C_Bags").Cells.Item(i).Specific).Value;
                            if (components[i - 1].BagsRequired.Equals(1) && (value == "" || value == "0.0") && !chkStatus.Checked) {
                                state.Stop();
                                UIApplication.ShowMessageBox((String.Format("Se require el campo Sacos-Pacas para el artículo {0} en la linea: {1}", components[i - 1].Item, i)));
                            }
                        }
                    }
                }).IsCompleted;
            }).Result) {
                return false;
            }

            return true;
        }
        public bool ValidateItemStock(Component productionLine) {
            return foodTransferDAO.GetItemStock(productionLine.Item, productionLine.Whs) < productionLine.Qty ? false : true;
        }
        #endregion

        #region OnInitializeComponent
        public override void OnInitializeComponent() {
            this.lblFolio = ((SAPbouiCOM.StaticText)(this.GetItem("lblFolio").Specific));
            this.lblItem = ((SAPbouiCOM.StaticText)(this.GetItem("lblItem").Specific));
            this.lblDate = ((SAPbouiCOM.StaticText)(this.GetItem("lblDate").Specific));
            this.lblQReal = ((SAPbouiCOM.StaticText)(this.GetItem("lblQReal").Specific));
            this.lblQPlan = ((SAPbouiCOM.StaticText)(this.GetItem("lblQPlan").Specific));
            this.lblQDif = ((SAPbouiCOM.StaticText)(this.GetItem("lblQDif").Specific));
            this.txtFolio = ((SAPbouiCOM.EditText)(this.GetItem("txtFolio").Specific));
            this.txtItem = ((SAPbouiCOM.EditText)(this.GetItem("txtItem").Specific));
            this.txtDate = ((SAPbouiCOM.EditText)(this.GetItem("txtDate").Specific));
            this.txtQReal = ((SAPbouiCOM.EditText)(this.GetItem("txtQReal").Specific));
            this.txtQPlan = ((SAPbouiCOM.EditText)(this.GetItem("txtQPlan").Specific));
            this.txtQDif = ((SAPbouiCOM.EditText)(this.GetItem("txtQDif").Specific));
            this.btnCancel = ((SAPbouiCOM.Button)(this.GetItem("btnCancel").Specific));
            this.btnCancel.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCancel_ClickBefore);
            this.btnCreate = ((SAPbouiCOM.Button)(this.GetItem("btnCreate").Specific));
            this.btnCreate.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCreate_ClickBefore);
            this.mtx0 = ((SAPbouiCOM.Matrix)(this.GetItem("mtx0").Specific));
            this.btnExit = ((SAPbouiCOM.Button)(this.GetItem("btnExit").Specific));
            this.btnExit.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnExit_ClickBefore);
            this.chkStatus = ((SAPbouiCOM.CheckBox)(this.GetItem("chkStatus").Specific));
            this.chkStatus.ClickAfter += new SAPbouiCOM._ICheckBoxEvents_ClickAfterEventHandler(this.chkStatus_ClickAfter);
            this.lblBags = ((SAPbouiCOM.StaticText)(this.GetItem("lblBags").Specific));
            this.txtBags = ((SAPbouiCOM.EditText)(this.GetItem("txtBags").Specific));
            this.OnCustomInitialize();

        }
        #endregion

        #region Controls
        private StaticText lblFolio;
        private StaticText lblItem;
        private StaticText lblDate;
        private StaticText lblQReal;
        private StaticText lblQPlan;
        private StaticText lblQDif;
        private EditText txtFolio;
        private EditText txtItem;
        private EditText txtDate;
        private EditText txtQReal;
        private EditText txtQPlan;
        private EditText txtQDif;
        private Button btnCancel;
        private Button btnCreate;
        private Button btnExit;
        private Matrix mtx0;
        private DataTable dt0;
        private CheckBox chkStatus;
        private StaticText lblBags;
        private EditText txtBags;
        #endregion



    }
}
