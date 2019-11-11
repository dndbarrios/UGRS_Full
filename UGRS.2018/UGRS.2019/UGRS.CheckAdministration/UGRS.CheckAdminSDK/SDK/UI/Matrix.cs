/*
 * Autor: LCC Abraham SaÚL Sandoval Meneses
 * Description: SAP B1 SDK UI Matrix
 * Date: 04/09/2018
 */

using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UGRS.Core.Extensions;
using UGRS.Core.Utility;



namespace UGRS.CheckAdminSDK.SDK.UI {
    public class UIMatrix {

        static Object padLock = new Object();
        public static void Fill<T>(Matrix mtx, DataTable dataTable, List<string> columns, T[] data) {

            try {
                if (!Object.ReferenceEquals(data, null) && data.Length > 0) {

                    AddEmptyRowsToDataTable(data.Length, dataTable);
                    FillRowNumberColumn(dataTable);
                    Parallel.ForEach(GetPartitioner(0, data.Length), (range, state) => {
                        for (int i = range.Item1; i < range.Item2; i++) {
                            foreach (var column in columns.Skip(1).Take(columns.Count - 2))
                                dataTable.SetValue("C_" + column, i, data[i].GetPropertyValue(column));
                        }
                    });

                    Bind(mtx, dataTable.UniqueID, columns);
                }
                else {
                    ClearMtx(mtx);
                }
            }
            catch (AggregateException ae) {
                ae.Handle(e => {
                    LogEntry.WriteException(e);
                    return true;
                });
            }
            catch (Exception ex) {
                LogEntry.WriteException(ex);
            }
        }

        public static OrderablePartitioner<Tuple<int, int>> GetPartitioner(int fromInclusive, int toExclusive) {
            var chunk = (int)Math.Round((double)(toExclusive / Environment.ProcessorCount), MidpointRounding.AwayFromZero);

            if (chunk.Equals(0)) {
                if (toExclusive % 2 == 0)
                    chunk = toExclusive / 2;
            }

            var partitioner = Partitioner.Create(fromInclusive, toExclusive, chunk > 0 ? chunk : 1);
            return partitioner;
        }

        private static void FillRowNumberColumn(DataTable dt) {
            Task.Factory.StartNew(() => {
                Parallel.For(0, dt.Rows.Count, row => {
                    dt.SetValue("C_#", row, row + 1);
                });
            });
        }

        public static DataTable CreateDataTable(string tableID, Dictionary<string, BoFieldsType> columns, UserFormBase frm) {
            DataTable dataTable = null;
            try {

                frm.UIAPIRawForm.DataSources.DataTables.Add(tableID);
                dataTable = frm.UIAPIRawForm.DataSources.DataTables.Item(tableID);

                foreach (KeyValuePair<string, BoFieldsType> column in columns) {
                    dataTable.Columns.Add("C_" + column.Key, column.Value);
                }

            }
            catch (Exception ex) {
                LogEntry.WriteException(ex);
            }
            return dataTable;
        }

        private static void Bind(Matrix mtx, string tableID, List<string> columns) {

            columns.ForEach(column => {
                mtx.Columns.Item("C_" + column).DataBind.Bind(tableID, "C_" + column);
            });

            mtx.LoadFromDataSource();
            mtx.AutoResizeColumns();
        }

        public static void ClearMtx(Matrix mtx) {
            mtx.Clear();
            mtx.AutoResizeColumns();
        }

        private static void AddEmptyRowsToDataTable(int length, DataTable dataTable) {
            dataTable.Rows.Clear();
            Parallel.For(0, length, row => {
                dataTable.Rows.Add();
            });
        }

        public static void AddRowData<T>(Matrix mtx, DataTable dt, List<string> columns, T rowData) {
            dt.Rows.Add();
            FillRowNumberColumn(dt);

            columns.Skip(1).ToList().ForEach(column => {
                dt.SetValue("C_" + column, dt.Rows.Count - 1, rowData.GetType().GetProperty(column).GetValue(rowData, null));
            });

            UIMatrix.Bind(mtx, dt.UniqueID, columns);
        }

        public static void RemoveRowData(Matrix mtx, DataTable dt, int row) {
            if (row > 0) {
                dt.Rows.Remove(row - 1);
                mtx.DeleteRow(row);
            }
        }

        public static double SumColumn(Matrix mtx, string column) {
            double total = 0.0;
            object padLock = new object();

            Parallel.ForEach(GetPartitioner(1, mtx.RowCount + 1), () => 0.0, (range, state, local) => {
                for (int i = range.Item1; i < range.Item2; i++) {
                    local += Convert.ToDouble(((EditText)mtx.Columns.Item(column).Cells.Item(i).Specific).Value);
                }
                return local;
            }, local => { lock (padLock) total += local; });

            return total;
        }

        public static bool SearchColumnValue(Matrix mtx, string column, string value) {
            if (mtx.RowCount > 0) {
                return (!Task.Factory.StartNew(() => {
                    return Parallel.ForEach(GetPartitioner(1, mtx.RowCount + 1), (range, state) => {
                        for (int i = range.Item1; i < range.Item2; i++)
                            if (((EditText)mtx.Columns.Item(column).Cells.Item(i).Specific).Value == value)
                                state.Stop();
                    }).IsCompleted;
                }).Result);
            }

            return false;
        }

        public static LinkedButton CreateLinkedButton(Matrix mtx, string column, BoLinkedObject linkedObject) {
            LinkedButton oLink = (LinkedButton)mtx.Columns.Item(column).ExtendedObject;
            oLink.LinkedObject = linkedObject;

            return oLink;
        }

        public static void AddColumnComboBoxValues(Matrix mtx, string column, Dictionary<string, string> validValues) {
            Column oColumn = mtx.Columns.Item(column);

            foreach (var value in validValues) {
                oColumn.ValidValues.Add(value.Value, value.Key);
            }
        }

        public static int[] GetSelectedRowsCondition(Matrix mtx, string column, string condition) {
            var bag = new ConcurrentBag<int>();
            if (!String.IsNullOrEmpty(condition)) {
                if (mtx.RowCount > 0) {
                    Parallel.ForEach(GetPartitioner(1, mtx.RowCount + 1), (range, state) => {
                        for (int i = range.Item1; i < range.Item2; i++) {
                            if (mtx.IsRowSelected(i) && GetComboBoxValue(mtx, column, i).Contains(condition))
                                bag.Add(i);
                        }
                    });
                }
            }
            return bag.AsParallel().ToArray();
        }

        public static int[] GetSelectedRows(Matrix mtx) {
            var bag = new ConcurrentBag<int>();
            if (mtx.RowCount > 0) {
                Parallel.ForEach(GetPartitioner(1, mtx.RowCount + 1), (range, state) => {
                    for (int i = range.Item1; i < range.Item2; i++) {
                        if (mtx.IsRowSelected(i))
                            bag.Add(i);
                    }
                });
            }
            return bag.AsParallel().ToArray();
        }

        public static void RemoveRowSelections(Matrix mtx, int[] selectedRows) {
            Parallel.ForEach(GetPartitioner(1, selectedRows.Length + 1), (range, state) => {
                for (int i = range.Item1; i < range.Item2; i++) {
                    mtx.SelectRow(selectedRows[i - 1], false, true);
                }
            });
        }

        public static void EnableColumnCells(Matrix mtx, int column, bool isAdmin, bool isArchive, int[] rows) {
            if (rows != null && rows.Length > 0) {
                Parallel.ForEach(GetPartitioner(0, rows.Count()), (range, state) => {
                    for (int i = range.Item1; i < range.Item2; i++) {
                        var status = GetComboBoxValue(mtx, "C_Status", rows[i]);
                        var area = GetComboBoxValue(mtx, "C_Area", rows[i]);

                        if (area.Equals("") || status.Equals(""))
                            continue;

                        if (isArchive) {
                            EnableCell(mtx, rows[i], column, false);
                            EnableCell(mtx, rows[i], column - 1, false);
                        }
                        else {
                            if (status.Contains("Pendiente") || status.Equals("Entregado a ARCHIVO")) {
                                EnableCell(mtx, rows[i], column, false);
                                EnableCell(mtx, rows[i], column - 1, false);
                            }
                            else {
                                EnableCell(mtx, rows[i], column, true);
                                if (isAdmin) {
                                    EnableCell(mtx, rows[i], column - 1, true);
                                }
                            }
                        }
                    }
                });
            }
        }

        public static void SelectAllRows(Matrix mtx) {
            Parallel.ForEach(GetPartitioner(1, mtx.RowCount + 1), (range, state) => {
                for (int i = range.Item1; i < range.Item2; i++) {
                    mtx.SelectRow(i, true, true);
                }
            });
        }

        public static void FillComboBoxColumn(Matrix mtx, DataTable dt, string column, string value) {
            Parallel.ForEach(GetPartitioner(1, mtx.RowCount + 1), (range, state) => {
                for (int i = range.Item1; i < range.Item2; i++) {
                    lock (padLock) {
                        ((SAPbouiCOM.ComboBox)mtx.Columns.Item(column).Cells.Item(i).Specific).Select(value, BoSearchKey.psk_ByValue);
                    }
                }
            });
        }

        private static void EnableCell(Matrix mtx, int row, int column, bool enable) => mtx.CommonSetting.SetCellEditable(row, column, enable);
        public static string GetCellValue(Matrix mtx, string column, int row) => ((EditText)mtx.Columns.Item(column).Cells.Item(row).Specific).Value;
        public static string GetComboBoxValue(Matrix mtx, string column, int row) => (string)(((ComboBox)mtx.Columns.Item(column).Cells.Item(row).Specific).Value);

    }
}
