/*
Autor: LCC Abraham Saúl Sandoval Meneses
Description: Stock Exit And Mass Invoicen Form
Date: 16/08/2018
Company: Qualisys
*/

using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI.Corrals.DAO;
using UGRS.Core.SDK.DI.Corrals.DTO;
using UGRS.Core.SDK.DI.Corrals.Services;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;

namespace UGRS.AddOn.Corrals.Forms {
    [FormAttribute("UGRS.AddOn.Corrals.Forms.frmMassBilling", "Forms/frmMassBilling.b1f")]

    public class frmMassBilling : UserFormBase {

        #region Properties
        Dictionary<string, BoFieldsType> columnsClient;
        Dictionary<string, BoFieldsType> columnsExit;
        MassInvoicingDAO massInvoicingDAO = new MassInvoicingDAO();
        DistributionDAO distributionDAO = new DistributionDAO();
        List<LivestockDTO> livestock = new List<LivestockDTO>();
        List<BatchDTO> batches = new List<BatchDTO>();
        List<FloorServiceLineDTO> floorServiceLines = new List<FloorServiceLineDTO>();
        List<PendingInvoiceDTO> pendingInvoices = new List<PendingInvoiceDTO>();
        UserValues user = new UserValues();
        FloorService floorServiceItem = new FloorService();
        bool invoicedFlag = false;
        bool mBoolAuthP = false;
        int selectedRow = 0;
        #endregion

        #region Constructor
        public frmMassBilling() {
            Parallel.Invoke(PrepareMatrix1, PrepareMatrix2, setDefaultValues, LoadEvents);
        }
        #endregion

        #region Load & Unload Events
        private void LoadEvents() {
            SAPbouiCOM.Framework.Application.SBO_Application.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(SBO_Application_FormDataEvent);
        }

        private void UnLoadEvents() {

            SAPbouiCOM.Framework.Application.SBO_Application.FormDataEvent -= new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(SBO_Application_FormDataEvent);
        }

        private void SBO_Application_FormDataEvent(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent) {
            BubbleEvent = true;

            try {
                if(BusinessObjectInfo.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD) {
                    invoicedFlag = true;
                    FillMatrix0("ClientDataTable", dt0, "N");
                    ClearMtx(mtx1);
                    invoicedFlag = false;
                }
            }
            catch { }
        }
        #endregion

        #region Prepare Matrix
        public void PrepareMatrix1() {

            columnsClient = new Dictionary<string, BoFieldsType>() { { "C_#", BoFieldsType.ft_AlphaNumeric }, { "C_Code", BoFieldsType.ft_AlphaNumeric }, { "C_Name", BoFieldsType.ft_AlphaNumeric }, { "C_Debt", BoFieldsType.ft_Float }, { "C_Invoiced", BoFieldsType.ft_AlphaNumeric } };
            dt0 = CreateDataTable("ClientDataTable", columnsClient);
            mtx0.Columns.Item("C_Result").Visible = false;
            mtx0.AutoResizeColumns();
        }

        public void PrepareMatrix2() {

            columnsExit = new Dictionary<string, BoFieldsType>() { { "C_#", BoFieldsType.ft_ShortNumber }, { "C_ItemCode", BoFieldsType.ft_AlphaNumeric }, { "C_ItemName", BoFieldsType.ft_AlphaNumeric }, { "C_Corral", BoFieldsType.ft_AlphaNumeric }, { "C_AuctDate", BoFieldsType.ft_AlphaNumeric }, { "C_Exist", BoFieldsType.ft_Float }, { "C_Quantity", BoFieldsType.ft_AlphaNumeric } };
            dt1 = CreateDataTable("LivestockDataTable", columnsExit);
            mtx1.AutoResizeColumns();
        }
        #endregion

        #region Default Values
        public void setDefaultValues() {

            user.WhsCode = distributionDAO.GetUserDefaultWarehouse();
            user.Area = distributionDAO.GetUserCostCenter();
            floorServiceItem = massInvoicingDAO.GetFloorServiceItem(user.WhsCode);
        }
        #endregion

        #region Create DataTable Matrix
        private SAPbouiCOM.DataTable CreateDataTable(string tableID, Dictionary<string, BoFieldsType> columns) {

            DataTable dataTable;
            try {

                this.UIAPIRawForm.DataSources.DataTables.Add(tableID);
                dataTable = this.UIAPIRawForm.DataSources.DataTables.Item(tableID);

                Parallel.ForEach(columns, column => {
                    dataTable.Columns.Add(column.Key, column.Value);
                });

                if(tableID.Equals("ClientDataTable"))
                    dataTable.Columns.Add("C_Result", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);

                return dataTable;
            }
            catch(Exception ex) {
                HandleException(ex, "(Create DataTable)");
                return null;
            }
        }
        #endregion

        #region Fill Matrix
        /// <summary>
        /// Fill Client Matrix Data By Parallel Loopps
        /// </summary>
        /// <param name="tableID"></param>
        /// <param name="dataTable"></param>
        private void FillMatrix0(string tableID, SAPbouiCOM.DataTable dataTable, string type) {

            try {

                dataTable.Rows.Clear();

                var columns = new List<string>(columnsClient.Keys);
                pendingInvoices = massInvoicingDAO.GetInvoicesPending(type);

                if(pendingInvoices.Count > 0) {

                    //add empty rows to datatable
                    Parallel.For(0, pendingInvoices.Count, row => {
                        dataTable.Rows.Add();
                    });

                    //add the index column in a separate task
                    Task.Factory.StartNew(() => {
                        Parallel.For(0, pendingInvoices.Count, row => {
                            dataTable.SetValue("C_#", row, row + 1);
                        });
                    });

                    //popolate the empty rows of the matrix 
                    Parallel.ForEach(Partitioner.Create(0, pendingInvoices.Count), (range, state) => {
                        for(int i = range.Item1; i < range.Item2; i++) {
                            Parallel.ForEach(columns.Skip(1), column => {
                                dataTable.SetValue(column, i, pendingInvoices[i].GetType().GetProperty(column.Replace("C_", String.Empty)).GetValue(pendingInvoices[i], null));
                            });
                        }
                    });

                    //bind datatable with matrix 
                    BindDataMatrix(mtx0, tableID, columns);
                    LogService.WriteInfo("(frmDelivery) Matriz Cargada correctamente");
                }
                else {
                    ClearMtx(mtx0);
                }
            }
            catch(AggregateException ae) {
                ae.Handle(e => {
                    HandleException(e, "(AE)");
                    return true;

                });
            }
            catch(Exception ex) {
                if(!invoicedFlag) {
                    HandleException(ex, "(FillMatrix0)");
                }
            }
        }

        /// <summary>
        /// Fill Livestock Matrix Data By Parallel Loopps
        /// </summary>
        /// <param name="tableID"></param>
        /// <param name="dataTable"></param>
        /// <param name="client"></param>
        /// <param name="type"></param>
        private void FillMatrix1(string tableID, SAPbouiCOM.DataTable dataTable, string client, string type) {

            try {
                dataTable.Rows.Clear();
                livestock = massInvoicingDAO.GetDistributedLiveStock(client, type);

                var columns = new List<string>(columnsExit.Keys);

                if(livestock.Count > 0) {
                    Parallel.For(0, livestock.Count, row => {
                        dataTable.Rows.Add();
                    });

                    Task.Factory.StartNew(() => {
                        Parallel.For(0, livestock.Count, row => {
                            dataTable.SetValue("C_#", row, row + 1);
                        });
                    });

                    Parallel.ForEach(Partitioner.Create(0, livestock.Count), (range, state) => {
                        for(int i = range.Item1; i < range.Item2; i++) {
                            Parallel.ForEach(columns.Skip(1), column => {
                                dataTable.SetValue(column, i, livestock[i].GetType().GetProperty(column.Replace("C_", String.Empty)).GetValue(livestock[i], null));
                            });
                        }
                    });

                    BindDataMatrix(mtx1, tableID, columns);

                    if(type.Equals("N")) {
                        Task.Factory.StartNew(() => {
                            batches = massInvoicingDAO.GetBatches(client, user.WhsCode, type);
                            floorServiceLines = massInvoicingDAO.GetFloorServiceLines(client, user.WhsCode, type);
                        });
                    }
                }
                else {
                    ClearMtx(mtx1);
                }


            }
            catch(AggregateException ae) {
                ae.Handle(e => {
                    HandleException(e, "(AE)");
                    return true;

                });
            }
            catch(Exception ex) {
                HandleException(ex, "(FillMatrix2)");
            }
        }
        #endregion

        #region BindData Matrix
        /// <summary>
        /// Bind the datable columns values to matrix
        /// </summary>
        /// <param name="mtx"></param>
        /// <param name="tableID"></param>
        /// <param name="columns"></param>
        public void BindDataMatrix(Matrix mtx, string tableID, List<string> columns) {

            this.UIAPIRawForm.Freeze(true);
            Parallel.ForEach(columns, column => {
                mtx.Columns.Item(column).DataBind.Bind(tableID, column);
            });

            if(tableID.Equals("ClientDataTable"))
                mtx.Columns.Item("C_Result").DataBind.Bind(tableID, "C_Result");

            mtx.LoadFromDataSource();
            mtx.AutoResizeColumns();
            this.UIAPIRawForm.Freeze(false);
        }

        /// <summary>
        /// Clear all columns and rows from matrix  
        /// </summary>
        /// <param name="mtx"></param>
        public void ClearMtx(Matrix mtx) {
            this.UIAPIRawForm.Freeze(true);
            mtx.Clear();
            this.UIAPIRawForm.Freeze(false);

        }

        /// <summary>
        ///Bind the result column values to matrix
        /// </summary>
        /// <param name="results"></param>
        /// <param name="type"></param>
        private void BindResultColumn(List<string> results, string type) {

            try {

                FillMatrix0("ClientDataTable", dt0, type);
                //Parallel.For(0, results.Count, row => {
                //    dt0.SetValue("C_Result", row, (!results[row].Equals("Error:") ? results[row] : "Error: No Se Pudo Crear La Factura"));
                //});

                for(int row = 0; row < dt0.Rows.Count; row++) {
                    dt0.SetValue("C_Result", row, (!results[row].Equals("Error:") ? results[row] : "Error: No Se Pudo Crear La Factura"));
                }

                this.UIAPIRawForm.Freeze(true);
                mtx0.Columns.Item("C_Result").Visible = true;
                mtx0.LoadFromDataSource();
                mtx0.AutoResizeColumns();
                mtx1.Clear();
            }
            catch(Exception ex) {
                HandleException(ex, "BindResultColumn");
            }
            finally {
                this.UIAPIRawForm.Freeze(false);
            }
        }
        #endregion

        #region Click Events
        private void mtx0_ClickAfter(object sboObject, SBOItemEventArg pVal) {

            if(!cbxType.Value.Equals("Facturación Subasta")) {
                if(pVal.Row != 0) {

                    selectedRow = pVal.Row;
                    mtx0.SelectRow(selectedRow, true, false);
                    FillMatrix1("LivestockDataTable", dt1, dt0.GetValue("C_Code", pVal.Row - 1).ToString(), "N");
                }
                else {
                    mtx0.SelectRow(pVal.Row, false, false);
                    selectedRow = 0;
                }
            }
        }


        private void btnCancel_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            UnLoadEvents();
            this.UIAPIRawForm.Close();
        }

        private void btnInvoice_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;

            if(!Validate()) {
                return;
            }

            switch(cbxType.Value) {

                case "Cobro Normal":
                Normal();
                break;

                case "Cobro Cierre":
                CreateInvoice("N");
                break;

                case "Facturación Subasta":
                CreateInvoice("S");

                break;
            }
        }

        #endregion

        #region MassInvoicing
        /// <summary>
        /// Normal Charge: Open the Invoice Form with the preliminary 
        /// </summary>
        private void Normal() {

            try {
                var results = new List<string>();
                var result = new ResultDTO();
                var client = dt0.GetValue("C_Code", selectedRow - 1).ToString();
                var invoice = new DocumentDTO();

                invoice.Document = pendingInvoices[selectedRow - 1];
                //invoice.FloorServiceLines = floorServiceLines;

                floorServiceLines = massInvoicingDAO.GetFloorServiceLines(client, user.WhsCode, "N");
                invoice.FloorServiceLines = floorServiceLines;
                invoice.DeliveryLines = massInvoicingDAO.GetDeliveryLines(client, user.WhsCode);

                if(invoice.FloorServiceLines.Count == 0 && invoice.DeliveryLines.Count == 0) {
                    results.Add("Sin servicio de piso ni entregas de alimento");
                }
                else {
                    result = InvoiceDI.CreateDraft(invoice, user, floorServiceItem, "N");
                }

                if(result.Success) {
                    selectedRow = 0;
                    invoicedFlag = true;
                    SAPbouiCOM.Framework.Application.SBO_Application.OpenForm((BoFormObjectEnum)112, "", result.Message);
                }
                else {
                    UIApplication.ShowMessageBox(String.Format("Error: {0}", result.Message));

                }
            }
            catch(Exception ex) {
                HandleException(ex, "(Normal)");
            }
        }


        /// <summary>
        /// Create Invoice For Closure Charge and Auction Invoicing
        /// </summary>
        private void CreateInvoice(string type) {

            var tasks = new List<Task<ResultDTO>>();
            var results = new List<string>();

            //try {
            LogService.WriteInfo("Begin Mass Billing " + type);
            DraftService lObjDraftService = new DraftService();
            lObjDraftService.DeleteDrafts(type);

            foreach(var doc in pendingInvoices) {

                //  tasks.Add(Task.Run(() => {

                try {
                    LogService.WriteInfo("Begin Creating Invoice for Cient: " + doc.Code);
                    var invoice = new DocumentDTO();
                    invoice.Document = doc;
                    invoice.FloorServiceLines = massInvoicingDAO.GetFloorServiceLines(doc.Code, user.WhsCode, type);
                    invoice.DeliveryLines = massInvoicingDAO.GetDeliveryLines(doc.Code, user.WhsCode);
                    //invoice.FloorServiceLines = massInvoicingDAO.GetFloorServiceLines(doc.Code, "CRHE", type);
                    //invoice.DeliveryLines = massInvoicingDAO.GetDeliveryLines(doc.Code, "CRHE");

                    if(invoice.FloorServiceLines.Count == 0 && invoice.DeliveryLines.Count == 0) {
                        results.Add("Sin servicio de piso ni entregas de alimento");
                    }
                    else {
                        results.Add(InvoiceDI.CreateInvoice(invoice, user, floorServiceItem, type).Message);
                    }
                    LogService.WriteInfo("Successfully Creating Invoice for Cient: " + doc.Code);
                    //  }));
                    //Thread.Sleep(130);
                }
                catch(Exception ex) {
                    HandleException(ex, "[Exception] Invoice for Client " + doc.Code);
                }
                //Task.WaitAll(tasks.ToArray());
            }

            //catch(AggregateException ae) {
            //    ae.Handle(e => {
            //        HandleException(e, "(Closure)");
            //        return true;
            //    });
            //}
            //catch(Exception ex) {
            //    HandleException(ex, "(Closure)");
            //}

            Task.Factory.StartNew(() => {
                // BindResultColumn(tasks.Select(t => t.Result.Message).AsParallel().AsOrdered().ToList(), type);
                BindResultColumn(results, type);
                LogService.WriteInfo("Done Mass Billing " + type);
            });

            UIApplication.ShowMessageBox("Revisar la Columna de Resultados");
        }

        #endregion

        #region LiveStock Exit
        /// <summary>
        /// Exit Livestock Event
        /// </summary>
        /// <param name="sboObject"></param>
        /// <param name="pVal"></param>
        /// <param name="BubbleEvent"></param>
        private void btnOut_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;

            if(!ValidateLivestockExit(true)) {
                return;
            }

            try {

                SetLivestockExitQuantities(cbOutAll.Checked ? "C_Exist" : "C_Quantity");
                if(!ValidateLivestockExit(false)) {
                    return;
                }

                LogService.WriteInfo("Begin LiveStock Exit for Client: " + dt0.GetValue("C_Code", selectedRow - 1).ToString());
                var iExit = new DocumentDTO();
                iExit.Document = pendingInvoices[selectedRow - 1];
                iExit.Lines = livestock.Where(l => l.Quantity != 0).AsParallel().ToList();
                iExit.Batches = batches;
                iExit.AuthProcess = mBoolAuthP;
                var result = IExitDI.CreateInventoryExit(iExit, "N", user);

                UIApplication.ShowMessageBox(result.Message);

                if(result.Success) {
                    Task.Factory.StartNew(() => { FillMatrix1("LivestockDataTable", dt1, dt0.GetValue("C_Code", selectedRow - 1).ToString(), "N"); });
                    Task.Factory.StartNew(() => {
                        FillMatrix0("ClientDataTable", dt0, "N");

                        if(mtx0.RowCount > 0) {
                            mtx0.SelectRow(selectedRow, true, false);
                        }
                    });
                    LogService.WriteInfo("Done LiveStock Exit for Client: " + dt0.GetValue("C_Code", selectedRow - 1).ToString());
                }
            }
            catch(AggregateException ae) {
                ae.Handle(e => {
                    HandleException(e, "(LivestockExit)");
                    return true;
                });
            }
            catch(Exception e) {
                HandleException(e, "(LivestockExit)");
            }
            finally {
                user.AppraisalValidation = false;
            }

        }

        private void Button0_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;

            try {
                if(UIApplication.GetApplication().MessageBox("¿Desea procesar los preliminares y convertirlos a facturas?", 2, "Si", "No", "") != 1) {
                    return;
                }
                string lStrInvoiceProcessorPath;
                if (Environment.Is64BitOperatingSystem)
                {
                    lStrInvoiceProcessorPath = new QueryManager().GetValue("U_VALUE", "Name", "GLO_InvoiceProcessorAppPath", "[@UG_Config]");
                }
                else
                {
                    lStrInvoiceProcessorPath = new QueryManager().GetValue("U_VALUE", "Name", "GLO_InvoiceProcessorAppPathx86", "[@UG_Config]");
                }
                if(string.IsNullOrEmpty(lStrInvoiceProcessorPath)) {
                    UIApplication.ShowError("Agregue un valor en la configuración para el campo GLO_InvoiceProcessorAppPath");
                    return;
                }

                if(!System.IO.File.Exists(lStrInvoiceProcessorPath)) {
                    UIApplication.ShowError("No existe la ruta de la aplicación asignada en la configuración en el campo GLO_InvoiceProcessorAppPath");
                    return;
                }

                UIApplication.GetApplication().Forms.ActiveForm.Freeze(true);

                using(var lObjProcess = new Process()) {
                    lObjProcess.StartInfo.FileName = lStrInvoiceProcessorPath;
                    lObjProcess.StartInfo.Arguments = "-v -s -a";
                    lObjProcess.Start();
                    lObjProcess.WaitForExit();
                    //proc.Close();
                }

                UIApplication.ShowSuccess("La aplicación se estará ejecutando en paralelo, quede atento a la consola o al log");
            }
            catch(Exception ex) {
                UIApplication.ShowMessageBox(string.Format("Error al procesar los preliminares: {0}", ex.Message));
            }
            finally {
                UIApplication.GetApplication().Forms.ActiveForm.Freeze(false);
            }
        }
        #endregion

        #region Auxiliary Methods
        /// <summary>
        /// Calculate Appraisal Import in Parallel
        /// </summary>
        /// <param name="livestock"></param>
        /// <returns></returns>
        public double CalculateAppraisalImport(List<LivestockDTO> livestock) {

            double import = 0.0;
            object monitor = new object();

            var task = Task.Factory.StartNew(() => {
                Parallel.ForEach(Partitioner.Create(0, livestock.Count), () => 0.0, (range, state, local) => {
                    for(int i = range.Item1; i < range.Item2; i++) {
                        local += ((livestock[i].Exist - livestock[i].Quantity) / 3) * livestock[i].Import;
                    }
                    return local;
                }, local => { lock(monitor) import += local; });
            });

            task.Wait();
            return import;
        }

        /// <summary>
        /// Set Livestock Exit Quantities Parallel
        /// </summary>
        /// <param name="column"></param>
        public void SetLivestockExitQuantities(string column) {
            var task = Task.Factory.StartNew(() => {
                Parallel.ForEach(Partitioner.Create(1, mtx1.RowCount + 1), (range, state) => {
                    for(int j = range.Item1; j < range.Item2; j++) {
                        livestock[j - 1].Quantity = Convert.ToDouble(((SAPbouiCOM.EditText)mtx1.Columns.Item(column).Cells.Item(j).Specific).Value == "" ? "0" : ((SAPbouiCOM.EditText)mtx1.Columns.Item(column).Cells.Item(j).Specific).Value);
                    }
                });
            });
            task.Wait();
        }

        /// <summary>
        /// ComboBox Event
        /// </summary>
        /// <param name="sboObject"></param>
        /// <param name="pVal"></param>
        private void cbxType_ComboSelectAfter(object sboObject, SBOItemEventArg pVal) {

            selectedRow = 0;
            var type = (cbxType.Value.Contains("Subasta")) ? "S" : "N";
            FillMatrix0("ClientDataTable", dt0, type);
            ClearMtx(mtx1);
        }
        #endregion

        #region Validations
        /// <summary>
        /// Validate before 
        /// </summary>
        /// <returns></returns>
        private bool Validate() {

            if(String.IsNullOrEmpty(cbxType.Value)) {
                UIApplication.ShowMessageBox("Seleccionar Tipo de Cobro");
                return false;
            }

            if(Object.ReferenceEquals(pendingInvoices, null) || pendingInvoices.Count <= 0)
                return false;

            switch(cbxType.Value) {

                case "Cobro Normal":

                if(selectedRow.Equals(0)) {
                    UIApplication.ShowMessageBox("Seleccionar Cliente");
                    return false;
                }

                if(dt0.GetValue("C_Debt", selectedRow - 1).ToString().Equals("0")) {
                    UIApplication.ShowMessageBox("Ya se ha facturado la deuda del cliente: " + dt0.GetValue("C_Name", selectedRow - 1).ToString());
                    return false;
                }
                break;

                case "Cobro Cierre":
                case "Facturación Subasta":
                if(UIApplication.ShowOptionBox("Se realizará la factura de tipo " + cbxType.Value) == 2) {
                    return false;
                }
                break;
            }
            return true;
        }

        /// <summary>
        /// Validate After and Before fill the quantities for Stock Exit in Livestock Matrix
        /// </summary>
        /// <param name="beforeQuantities"></param>
        /// <returns></returns>
        public bool ValidateLivestockExit(bool beforeQuantities) {

            if(beforeQuantities) {

                if(selectedRow.Equals(0)) {
                    UIApplication.ShowMessageBox("Selecionar Cliente");
                    return false;
                }

                if(String.IsNullOrEmpty(cbxType.Value))
                    return false;

                if(cbxType.Value.Equals("Facturación Subasta"))
                    return false;

                if(cbOutAll.Checked) {
                    if(UIApplication.ShowOptionBox("Estas seguro se sar salida a todo el ganado") == 2)
                        return false;
                }

                if(!pendingInvoices[selectedRow - 1].Debt.Equals(0)) {
                    UIApplication.ShowMessageBox("No se puede dar salida al ganado sin haber facturado la deuda");
                    return false;
                }
            }
            else { //Validate the Quantities

                var appraisalImport = 0.0;
                var debtImport = 0.0;

                if(livestock.Where(l => l.Quantity == 0).AsParallel().Count() == livestock.Count) {
                    UIApplication.ShowMessageBox("No se ha ingresado cantidad para dar salida al ganado");
                    return false;
                }

                //This task validate that the Quantity have to be less or equal than Existence 
                var task = Task.Factory.StartNew(() => {

                    var cts = new CancellationTokenSource();
                    var po = new ParallelOptions();
                    po.CancellationToken = cts.Token;

                    ParallelLoopResult result = Parallel.ForEach(Partitioner.Create(0, livestock.Count), (range, state) => {
                        for(int i = range.Item1; i < range.Item2; i++) {
                            if(livestock[i].Quantity > livestock[i].Exist) {
                                cts.Cancel(); //cancel the loop 
                            }
                        }
                    });
                    if(cts.IsCancellationRequested) {
                        return false;
                    }
                    return true;
                });

                task.Wait();

                if(!task.Result) {
                    UIApplication.ShowMessageBox("La cantidad no debe ser mayor que la existencia");
                    return false;
                }

                //Validate Appraisal Import and Debt Import
                var task2 = Task.Factory.StartNew(() => { appraisalImport = CalculateAppraisalImport(livestock); });
                var task3 = Task.Factory.StartNew(() => { debtImport = massInvoicingDAO.GetDebtImport(pendingInvoices[selectedRow - 1].Code, user.WhsCode); });
                Task.WaitAll(task2, task3);

                if(appraisalImport < debtImport) {
                    if(UIApplication.ShowOptionBox("El importe avaluo es menor que el importe deuda, estas seguro de dar salida al ganado") == 2) {
                        mBoolAuthP = false;
                        return false;
                    }
                    else {
                        mBoolAuthP = true;
                        user.AppraisalValidation = true;
                    }
                }
            }

            return true;
        }
        #endregion

        #region Initialize Methods
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent() {
            this.lblPayType = ((SAPbouiCOM.StaticText)(this.GetItem("lblPayType").Specific));
            this.cbxType = ((SAPbouiCOM.ComboBox)(this.GetItem("cbxType").Specific));
            this.cbxType.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cbxType_ComboSelectAfter);
            this.mtx0 = ((SAPbouiCOM.Matrix)(this.GetItem("mtx0").Specific));
            this.mtx0.ClickAfter += new SAPbouiCOM._IMatrixEvents_ClickAfterEventHandler(this.mtx0_ClickAfter);
            this.mtx1 = ((SAPbouiCOM.Matrix)(this.GetItem("mtx1").Specific));
            this.cbOutAll = ((SAPbouiCOM.CheckBox)(this.GetItem("cbOutAll").Specific));
            this.btnCancel = ((SAPbouiCOM.Button)(this.GetItem("btnCancel").Specific));
            this.btnCancel.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCancel_ClickBefore);
            this.btnOut = ((SAPbouiCOM.Button)(this.GetItem("btnOut").Specific));
            this.btnOut.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnOut_ClickBefore);
            this.btnInvoice = ((SAPbouiCOM.Button)(this.GetItem("btnInvoice").Specific));
            this.btnInvoice.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnInvoice_ClickBefore);
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("btnPInv").Specific));
            this.Button0.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button0_ClickBefore);
            this.OnCustomInitialize();

        }

        private void Form_UnloadAfter(SBOItemEventArg pVal) {
            UnLoadEvents();
        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents() {
            this.UnloadAfter += new UnloadAfterHandler(this.Form_UnloadAfter);

        }
        private void OnCustomInitialize() { }
        #endregion

        #region Controls
        private StaticText lblPayType;
        private ComboBox cbxType;
        private Matrix mtx0;
        private Matrix mtx1;
        private CheckBox cbOutAll;
        private Button btnCancel;
        private Button btnOut;
        private DataTable dt0;
        private DataTable dt1;
        private Button btnInvoice;
        #endregion

        #region Handle Exception

        public void HandleException(Exception ex, string section) {

            UIApplication.ShowMessageBox(ex.Message);
            LogService.WriteError(String.Format("{0}: {1}, {2}", section, ex.Message, ex.StackTrace));
        }
        #endregion
        private Button Button0;

    }
}

