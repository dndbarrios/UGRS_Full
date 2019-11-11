/*
 * Author: LCC Abraham Saúl Sandoval Meneses
 * Decription: UGRS Inventory Revaluation AddOn
 * Date: 03/09/2019
 */

using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Utility;
using UGRS.InvRevaluationSDK.Models;
using UGRS.InvRevaluationSDK.SDK.DI.Services;
using UGRS.InvRevaluationSDK.SDK.UI;


namespace UGRS.InvRevaluationAddOn {
    [FormAttribute("UGRS.InventoryRevaluation.Form1", "RevaluationForm.b1f")]
    class RevaluationForm : UserFormBase {

        Dictionary<string, BoFieldsType> gridColumns;
        string[] gridHeaders;
        private Button btnSearch;
        private Button btnAcept;
        private Button btnCancel;
        private Button btnAcept2;
        private StaticText lblExit;
        private EditText txtExit;
        private Grid grid0;
        SapB1 sapB1;
        RevaluationItem[] items;

        public RevaluationForm() {
            sapB1 = new SapB1();

            InitializeGrid("grid0");
            btnAcept.Item.Enabled = false;
            btnAcept2.Item.Enabled = false;
        }

        private void InitializeGrid(string id) {
            gridColumns = GetGridColumns();
            gridHeaders = GetGridHeaders();
            this.UIAPIRawForm.DataSources.DataTables.Add(id);
            grid0.DataTable = UIGrid.CreateDataTable(this, id, gridColumns);
            UIGrid.SetHeaders(grid0, gridHeaders);
            grid0.AutoResizeColumns();
        }

        public override void OnInitializeComponent() {
            this.lblExit = ((SAPbouiCOM.StaticText)(this.GetItem("lblExit").Specific));
            this.txtExit = ((SAPbouiCOM.EditText)(this.GetItem("txtExit").Specific));
            this.grid0 = ((SAPbouiCOM.Grid)(this.GetItem("grid0").Specific));
            this.btnSearch = ((SAPbouiCOM.Button)(this.GetItem("btnSearch").Specific));
            this.btnSearch.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSearch_ClickBefore);
            this.btnAcept = ((SAPbouiCOM.Button)(this.GetItem("btnAcept1").Specific));
            this.btnAcept.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnAcept_ClickBefore);
            this.btnCancel = ((SAPbouiCOM.Button)(this.GetItem("btnCancel").Specific));
            this.btnCancel.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCancel_ClickBefore);
            this.btnAcept2 = ((SAPbouiCOM.Button)(this.GetItem("btnAcept2").Specific));
            this.btnAcept2.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnAcept2_ClickBefore);
            this.OnCustomInitialize();

        }

        public override void OnInitializeFormEvents() { }
        private void OnCustomInitialize() { }
        private Dictionary<string, BoFieldsType> GetGridColumns() => new Dictionary<string, BoFieldsType>() {
                                                                       { "Row", BoFieldsType.ft_ShortNumber },
                                                                       { "ItemCode", BoFieldsType.ft_AlphaNumeric },
                                                                       { "Description", BoFieldsType.ft_AlphaNumeric},
                                                                       { "Whs", BoFieldsType.ft_AlphaNumeric },
                                                                       { "DocDate", BoFieldsType.ft_Date},
                                                                       { "Rev1", BoFieldsType.ft_AlphaNumeric},
                                                                     };
        private string[] GetGridHeaders() => new string[] { "#", "Código Artículo", "Descripcíon", "Almacen", "Fecha Documento", "Revalorización" };

        private void btnCancel_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            this.UIAPIRawForm.Close();
        }

        private void btnSearch_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;

            if (!String.IsNullOrEmpty(txtExit.Value))
                Search(txtExit.Value, "1");
        }

        private void Search(string docNum, string type) {
            try {

                var count = sapB1.DAO.RevaluationsCount(docNum);
                if (count.Equals(1))
                    type = "2";

                items = sapB1.DAO.GetInvRevaluationItems(docNum, type);
                this.UIAPIRawForm.Freeze(true);
                UIGrid.Fill(grid0, items, gridColumns.Keys.ToArray());

                if (grid0.Rows.Count > 0) {
                    if (type.Equals("1") || count.Equals(1)) {
                        if (count.Equals(1)) {
                            btnAcept.Item.Enabled = false;
                            btnAcept2.Item.Enabled = true;
                        }
                        else if (count.Equals(0)){
                            btnAcept.Item.Enabled = true;
                        }

                    }
                }

            }
            catch (Exception ex) {
                LogEntry.WriteException(ex);
            }
            finally {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        private void btnAcept_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            if (btnAcept.Item.Enabled) {

                if (sapB1.DAO.ExistRevaluation(txtExit.Value)) {
                    UIApplication.ShowMessageBox($"Ya existe una Revaluación de Inventario para la Salida: {txtExit.Value}");
                    btnAcept.Item.Enabled = false;
                }
                else {
                    var result = false;
                    foreach (var item in items) {
                        if (item.Rev1.Equals(0)) {
                            UIApplication.ShowMessageBox($"No se puede crear la revalorización con valor 0");
                        }
                        else {
                            result = sapB1.InventoryRevaluation.Insert(item.ItemCode, item.Whs, item.Rev1, item.DocDate, txtExit.Value);
                        }
                    }

                    if (result) {
                        UIApplication.ShowMessageBox($"Se ha creado la primera revalorización de inventario para la salida {txtExit.Value}");
                        btnAcept.Item.Enabled = false;
                        btnAcept2.Item.Enabled = true;
                        Search(txtExit.Value, "2");
                    }
                    else
                        UIApplication.ShowMessageBox($"Fallo en insertarse la revalorización para la salida {txtExit.Value}");
                }
            }
        }

        private void btnAcept2_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            if (btnAcept2.Item.Enabled) {
                var result2 = false;

                foreach (var item2 in items) {
                    if (item2.Rev1.Equals(0)) {
                        UIApplication.ShowMessageBox($"No se puede crear la revalorización con valor 0");
                    }
                    else {
                        result2 = sapB1.InventoryRevaluation.Insert(item2.ItemCode, item2.Whs, item2.Rev1, DateTime.Now, txtExit.Value);
                    }
                }

                if (result2) {
                    UIApplication.ShowMessageBox($"Se ha creado con exito la segunda revalorización de inventario para la salida {txtExit.Value}");
                    btnAcept2.Item.Enabled = false;
                }
            }

        }
    }
}