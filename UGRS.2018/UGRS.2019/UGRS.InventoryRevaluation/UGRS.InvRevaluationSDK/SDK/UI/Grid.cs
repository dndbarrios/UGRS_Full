
/*
 * Autor: LCC Abraham SaÚL Sandoval Meneses
 * Description: Grid Methods
 * Date: 10/06/2019
 */

using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UGRS.InvRevaluationSDK.SDK.UI {
    public class UIGrid {
        #region SetHeaders
        public static void SetHeaders(Grid grid, string[] headers) {
            for (int i = 0; i < headers.Length; i++) {
                grid.Columns.Item(i).TitleObject.Caption = headers[i];
            }
        }
        #endregion

        #region CreateDataTable
        public static DataTable CreateDataTable(UserFormBase form, string tableID, Dictionary<string, SAPbouiCOM.BoFieldsType> columns) {
            DataTable dataTable = form.UIAPIRawForm.DataSources.DataTables.Item(tableID);
            foreach (var column in columns) {
                dataTable.Columns.Add(column.Key, column.Value);
            }
            return form.UIAPIRawForm.DataSources.DataTables.Item(tableID);
        }
        #endregion

        #region Fill
        public static void Fill<T>(Grid grid, T[] data, string[] columns) {
            if (data != null && data.Length > 0) {
                AddEmptyRows(grid, data.Length);
                Parallel.Invoke(
                    () => SetColumnValues(grid, data, columns, 0, 5),
                    () => SetColumnValues(grid, data, columns, 5, 8)
                 );
            }
            else {
                grid.DataTable.Rows.Clear();   
            }
        }
        #endregion

        #region AddEmptyRows
        private static void AddEmptyRows(Grid grid, int length) {
            Parallel.For(0, length, row => {
                grid.DataTable.Rows.Add();
            });
        }
        #endregion

        #region SetColumnValues
        private static void SetColumnValues<T>(Grid grid, T[] data, string[] columns, int skip, int take) {
            Parallel.ForEach(GetPartitioner(0, data.Length), (range, state) => {
                for (int i = range.Item1; i < range.Item2; i++) {
                    foreach (var column in columns.Skip(skip).Take(take)) {
                        var value = data[i].GetType().GetProperty(column).GetValue(data[i], null) ?? 0;
                        grid.DataTable.SetValue(column, i, value);
                    }
                }
            });
        }
        #endregion

        private static OrderablePartitioner<Tuple<int, int>> GetPartitioner(int fromInclusive, int toExclusive) {
            var chunk = (int)Math.Round((double)(toExclusive / Environment.ProcessorCount), MidpointRounding.AwayFromZero);

            if (chunk.Equals(0)) {
                if (toExclusive % 2 == 0)
                    chunk = toExclusive / 2;
            }

            var partitioner = Partitioner.Create(fromInclusive, toExclusive, chunk > 0 ? chunk : 1);
            return partitioner;
        }
    }
}

