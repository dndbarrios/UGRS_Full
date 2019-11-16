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
                                                                       { "ItemCode", BoFieldsType.ft_AlphaNumeric },
                                                                       { "whCode", BoFieldsType.ft_AlphaNumeric },
                                                                       { "DocDateRev1", BoFieldsType.ft_Date},
                                                                       { "Rev1", BoFieldsType.ft_AlphaNumeric},
                                                                       { "CostoRev1", BoFieldsType.ft_AlphaNumeric},
                                                                       { "DocDateRev2", BoFieldsType.ft_Date},
                                                                       { "Rev2", BoFieldsType.ft_AlphaNumeric},
                                                                       { "CostoRev2", BoFieldsType.ft_AlphaNumeric},
                                                                     };
        private string[] GetGridHeaders() => new string[] { "Código Artículo", "Código Almacen", "Fecha Reval1", "Importe Reval1", "Costo Rev1", "Fecha Reval2", "Importe Reval2", "Costo Rev2" };

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
                items = sapB1.DAO.GetInvRevaluationItems(docNum);
                grid0.DataTable.Rows.Clear();

                if (items.Length > 0) {
                    this.UIAPIRawForm.Freeze(true);
                    UIGrid.Fill(grid0, items, gridColumns.Keys.ToArray());
                    grid0.AutoResizeColumns();
                    btnAcept.Item.Enabled = true;
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
                InsertRevaluation(1);
            }
        }

        private void btnAcept2_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            if (btnAcept2.Item.Enabled) {
                InsertRevaluation(2);
            }
        }

        private void InsertRevaluation(int type) {

            var revValue = type.Equals(1) ? items.FirstOrDefault().Rev1.Equals(0) : items.FirstOrDefault().Rev2.Equals(0);
            if (revValue) {
                UIApplication.ShowMessageBox($"No se puede crear revalorización con valor 0");
                if (type.Equals(1))
                    btnAcept2.Item.Enabled = true;
            }
            else {

                var revItems = items.Select(item => {

                    var revVal = type.Equals(1) ? item.Rev1 : item.Rev2;

                    if (revVal > 0 && revVal < 0.1) {
                        UIApplication.ShowMessageBox($"El el valor para el artículo {item.ItemCode} no se concidera para revalorizacion por ser menor a 0.1");
                        return null;
                    }
                    else {
                        return item;
                    }
                }).Where(r => r != null).ToArray();

                if (revItems != null && revItems.Length > 0) {

                    var result = sapB1.InventoryRevaluation.Insert(txtExit.Value, items, type);
                    if (result) {
                        UIApplication.ShowMessageBox($"Se ha creado la revalorización {type} para la salida {txtExit.Value}");
                        if (type.Equals(1)) {
                            btnAcept.Item.Enabled = false;
                            btnAcept2.Item.Enabled = true;
                        }
                        else {
                            btnAcept.Item.Enabled = false;
                            btnAcept2.Item.Enabled = false;
                        }
                    }
                    else
                        UIApplication.ShowMessageBox($"Fallo en insertarse la revalorización para la salida {txtExit.Value}");
                }
                else {
                    UIApplication.ShowMessageBox($"No hay artículos para revalorizar");
                }
            }

        }
    }
}