/*
 * Author: LCC Abraham Saúl Sandoval Meneses 
 * Date: 13/07/2019
 * Description: Base Data Access Object
 */

using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UGRS.CheckAdminSDK.SDK.DI;
using UGRS.Core.Utility;
using UGRS.Core.Extensions;


namespace UGRS.CheckAdminSDK.DAO {

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

        #region GetObject
        protected T GetObject<T>(string query) {

            var oRecordset = DIApplication.GetRecordset();
            T obj = (T)Activator.CreateInstance(typeof(T));

            try {

                oRecordset.DoQuery(query);
                if (oRecordset.RecordCount > 0) {
                    foreach (var field in oRecordset.Fields.OfType<Field>())
                        obj.GetType().GetProperty(field.Name).SetValue(obj, field.Value);
                }
            }
            catch (Exception ex) {
                LogEntry.WriteException(ex);
            }
            finally {
                MemoryUtility.ReleaseComObject(oRecordset);
            }

            return obj;
        }
        #endregion

        protected T[] GetValues<T>(string query) {

            T[] values = null;
            var oRecordset = DIApplication.GetRecordset();

            try {
                oRecordset.DoQuery(query);
                if (oRecordset.RecordCount > 0) {
                    values = new T[oRecordset.RecordCount];
                    for (int i = 0; i < oRecordset.RecordCount; i++) {
                        values[i] = (T)oRecordset.Fields.Item(0).Value;
                        oRecordset.MoveNext();
                    }
                    return values;
                }
            }
            catch (Exception ex) {
                LogEntry.WriteException(ex);
            }
            finally {
                MemoryUtility.ReleaseComObject(oRecordset);
            }
            return null;
        }
        protected T GetValue<T>(string query) {

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

        public int GetCount(string table, string condition, string value) => GetValue<int>(this.GetSQL("GetFieldValues").Inject(new Dictionary<string, string>() { { "Field", "COUNT(*)" }, { "Table", table }, { "Condition", condition }, { "{Value}", value } }));
        protected T[] GetFieldValues<T>(string field, string table, string condition, string value) => GetValues<T>(this.GetSQL("GetFieldValues").Inject(new Dictionary<string, string>() { { "Field", field }, { "Table", table }, { "Condition", condition }, { "{Value}", value } }));
        protected dynamic GetFieldValue(string field, string table, string condition, string value) => GetValue<dynamic>(this.GetSQL("GetFieldValues").Inject(new Dictionary<string, string>() { { "Field", field }, { "Table", table }, { "Condition", condition }, { "{Value}", value } }));
    }
}
