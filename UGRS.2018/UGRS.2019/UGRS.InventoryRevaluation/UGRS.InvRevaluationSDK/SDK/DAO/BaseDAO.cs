using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Extensions;
using UGRS.Core.Utility;
using UGRS.InvRevaluationSDK.SDK.DI;


namespace UGRS.InvRevaluationSDK.SDK.DAO {

    public class BaseDAO {

        #region GetArray
        protected T[] GetArray<T>(string query) {

            var oRecordset = DIApplication.GetRecordset();
            T[] array = null;

            try {

                oRecordset.DoQuery(query);
                if (oRecordset.RecordCount > 0) {

                    array = new T[oRecordset.RecordCount];
                    for (int i = 0; i < oRecordset.RecordCount; i++) {
                        var element = (T)Activator.CreateInstance(typeof(T));
                        foreach (var field in oRecordset.Fields.OfType<Field>()) {
                            element.SetPropertyValue(field.Name, field.Value);
                        }
                        array[i] = element;
                        oRecordset.MoveNext();
                    }
                }
            }
            catch (Exception ex) {
                LogEntry.WriteException(ex);
            }
            finally {
                MemoryUtility.ReleaseComObject(oRecordset);
            }

            return array;
        }
        #endregion

        #region GetValue
        private T GetValue<T>(string query) {

            var oRecordset = DIApplication.GetRecordset();

            try {
                oRecordset.DoQuery(query);
                if (oRecordset.RecordCount > 0) {
                    return (T)oRecordset.Fields.Item(0).Value;
                }
            }
            catch (Exception ex) {
                LogEntry.WriteException(ex);
            }
            finally {
                MemoryUtility.ReleaseComObject(oRecordset);
            }

            return default(T);
        }
        #endregion

        public int GetCount(string table, string condition, string value) => GetValue<dynamic>(this.GetSQL("GetCount").Inject(new Dictionary<string, string>() { { "Table", table }, { "Condition", condition }, { "{Value}", value } }));
    }
}
