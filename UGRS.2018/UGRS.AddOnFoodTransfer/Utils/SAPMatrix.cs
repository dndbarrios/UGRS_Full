/*
 * Autor: LCC Abraham SaÚL Sandoval Meneses
 * Description: SAP B1 Matrix
 * Date: 04/09/2018
 */


using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI.FoodTransfer.DTO;


namespace UGRS.AddOnFoodTransfer.Utils {

    public class SAPMatrix {
        public static void Fill<T>(string tableID, DataTable dataTable, Matrix mtx, List<string> columns, T[] data) {

            try {

                if(!Object.ReferenceEquals(data, null) && data.Length > 0) {

                    dataTable.Rows.Clear();

                    Parallel.For(0, data.Length, row => {
                        dataTable.Rows.Add();
                    });

                    Task.Factory.StartNew(() => {
                        Parallel.For(0, data.Length, row => {
                            dataTable.SetValue("C_#", row, row + 1);
                        });
                    });

                    Parallel.ForEach(Partitioner.Create(0, data.Length), (range, state) => {
                        for(int i = range.Item1; i < range.Item2; i++) {

                            Parallel.ForEach(columns.Skip(1), column => {
                                dataTable.SetValue("C_" + column, i, data[i].GetType().GetProperty(column).GetValue(data[i], null));
                            });
                        }
                    });
                    Bind(mtx, tableID, columns);
                }
                else {
                    ClearMtx(mtx);
                }
            }
            catch(AggregateException ae) {
                ae.Handle(e => {
                    SAPException.Handle(e, "(AE)");
                    return true;
                });
            }
            catch(Exception ex) {
                SAPException.Handle(ex, "(FillMatrix0)");
            }
        }

        public static void Bind(Matrix mtx, string tableID, List<string> columns) {

            columns.AsParallel().ForAll(column => {
                mtx.Columns.Item("C_" + column).DataBind.Bind(tableID, "C_" + column);
            });
            mtx.LoadFromDataSource();
            mtx.AutoResizeColumns();
        }

        public static void ClearMtx(Matrix mtx) {
            mtx.Clear();
            mtx.AutoResizeColumns();
        }

        public static DataTable CreateDataTable(string tableID, Dictionary<string, BoFieldsType> columns, UserFormBase frm) {
            DataTable dataTable = null;
            try {
                frm.UIAPIRawForm.DataSources.DataTables.Add(tableID);
                dataTable = frm.UIAPIRawForm.DataSources.DataTables.Item(tableID);

                columns.AsParallel().ForAll(column => {
                    dataTable.Columns.Add("C_" + column.Key, column.Value);
                });
            }
            catch(Exception ex) {
                SAPException.Handle(ex, "(Create DataTable)");
            }
            return dataTable;
        }

        public static void SetColumnQuantities<T>(Matrix mtx, string column, T[] data, string property) {

            Parallel.ForEach(Partitioner.Create(1, mtx.RowCount + 1), (range, state) => {
                for(int j = range.Item1; j < range.Item2; j++) {
                    var value = ((EditText)mtx.Columns.Item("C_" + column).Cells.Item(j).Specific).Value;
                    data[j - 1].GetType().GetProperty(property).SetValue(data[j - 1], Convert.ToDouble(value == "" ? "0" : value));
                }
            });
        }

        public static double SumColumnQuantities(Matrix mtx, string column, Component[] data) {

            double total = 0.0;
            object padLock = new object();

            Parallel.ForEach(Partitioner.Create(1, mtx.RowCount + 1), () => 0.0, (range, state, local) => {

                for(int i = range.Item1; i < range.Item2; i++) {
                    var item = ((SAPbouiCOM.EditText)mtx.Columns.Item("C_Item").Cells.Item(i).Specific).Value;
                    if(data.Where(c => c.Item == item).FirstOrDefault().Prod == 0) {
                        local += Convert.ToDouble(((SAPbouiCOM.EditText)mtx.Columns.Item(column).Cells.Item(i).Specific).Value);
                    }
                }
                return local;
            }, local => { lock(padLock) total += local; });

            return total;
        }
    }
}
