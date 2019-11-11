// file:	DAO\TableDAO.cs
// summary:	Implements the table dao class


using UGRS.Core.Utility;
using UGRS.CheckAdminSDK.SDK.DI.DAO.Interfaces;
using SAPbobsCOM;
using System;
using System.Linq;
using System.Reflection;

namespace UGRS.CheckAdminSDK.SDK.DI.DAO {

    public class TableDAO<T> : ITableDAO<T> where T : ITable {
        #region Public

        public void Initialize() {
            //Get instance of current object
            T instance = GetInstance();

            //Create tables and fields if not exists
            InitializeTable(instance);
            InitializeFields(instance);
        }

        public int Add(T pObjRecord) {
            UserTable userTable = GetUserTable();

            try {
                userTable = PopulateUserTable(userTable, pObjRecord);
                return userTable.Add();
            }
            catch (Exception e) {
                LogEntry.WriteException(e);
                return -1;
            }
            finally {
                MemoryUtility.ReleaseComObject(userTable);
            }
        }

        public int Update(T pObjRecord, int key = 0) {
            UserTable userTable = GetUserTable();

            try {
                if (userTable.GetByKey(key.ToString())) {
                    userTable = PopulateUserTable(userTable, pObjRecord);
                    return userTable.Update();
                }
                else {
                    LogEntry.WriteError($"No existe el registro '{pObjRecord.GetKey()}'.");
                    return -1;
                }
            }
            catch (Exception e) {
                LogEntry.WriteException(e);
                return -1;
            }
            finally {
                MemoryUtility.ReleaseComObject(userTable);
            }
        }

        public int Remove(string pStrCode) {
            UserTable lObjUserTable = GetUserTable();

            try {
                if (lObjUserTable.GetByKey(pStrCode)) {
                    return lObjUserTable.Remove();
                }
                else {
                    LogEntry.WriteError($"No existe el registro '{pStrCode}'.");
                    return -1;
                }
            }
            catch (Exception e) {
                LogEntry.WriteException(e);
                return -1;
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjUserTable);
            }
        }
        #endregion

        #region Protecte
        protected UserTable GetUserTable() => DIApplication.Company.UserTables.Item(GetUserTableName());

        protected string GetUserTableName() => GetInstance().GetAttributes().Name;

        protected T GetInstance() => (T)Activator.CreateInstance(typeof(T));


        #endregion

        #region Private


        private UserTable PopulateUserTable(UserTable pObjUserTable, T pObjRecord) {
            if (!string.IsNullOrEmpty(pObjRecord.GetName())) {
                pObjUserTable.Name = pObjRecord.GetName();
            }

            foreach (PropertyInfo lObjProperty in pObjRecord.GetType().GetProperties().Where(x => x.GetMethod.IsPublic && !x.GetMethod.IsVirtual)) {
                try {
                    string lStrFieldName = string.Format("U_{0}", lObjProperty.Name);
                    Type lObjFieldType = lObjProperty.PropertyType;
                    object lUnkFieldValue = lObjFieldType == typeof(bool) ? ((bool)lObjProperty.GetValue(pObjRecord, null) ? "Y" : "N") : (lObjProperty.GetValue(pObjRecord, null));

                    pObjUserTable.UserFields.Fields.Item(lStrFieldName).Value = (lObjFieldType != typeof(DateTime) ? lUnkFieldValue.ToString() : lUnkFieldValue);
                }
                catch {

                }
            }

            return pObjUserTable;
        }

        private bool ExistsTable(string pStrTableName) {
            UserTablesMD lObjUserTable = null;

            try {
                lObjUserTable = (UserTablesMD)DIApplication.Company.GetBusinessObject(BoObjectTypes.oUserTables);
                return lObjUserTable.GetByKey(pStrTableName);
            }
            catch (Exception e) {
                LogEntry.WriteException(e);
                return false;
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjUserTable);
            }
        }

        private bool ExistsField(string pStrTableName, string pStrFieldName) {
            //Get recordset
            Recordset lObjRecordSet = DIApplication.GetRecordset();

            try {
                //Get field query and execute
                lObjRecordSet.DoQuery(GetFieldQuery(pStrTableName, pStrFieldName));

                //Return result
                return lObjRecordSet.RecordCount > 0 ? true : false;
            }
            catch (Exception e) {
                LogEntry.WriteException(e);
                return false;
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }


        private void InitializeTable(T pObjTable) {

            UserTablesMD lObjUserTable = pObjTable.GetUserTable();

            try {
                if (!ExistsTable(lObjUserTable.TableName)) {

                    if (lObjUserTable.Add().Equals(0)) {
                        LogEntry.WriteSuccess($"User defined table added: {lObjUserTable.TableName}");
                    }
                    else {
                        LogEntry.WriteError($"Failed to add User Defined Table: {lObjUserTable.TableName}, {DIApplication.Company.GetLastErrorDescription()}");
                    }

                }
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjUserTable);
            }
        }


        private void InitializeFields(T pObjTable) => pObjTable.GetFields().ForEach(field => InitializeField(field));

        private void InitializeField(UGRS.CheckAdminSDK.SDK.DI.DAO.Models.Field pObjField) {
            UserFieldsMD lObjUserField = null;

            try {
                if (!ExistsField(pObjField.TableName.ToUpper(), pObjField.GetAttributes().Name)) {
                    lObjUserField = pObjField.GetUserField();

                    if (lObjUserField.Add().Equals(0)) {
                        LogEntry.WriteInfo($"New field Added: {pObjField.TableName}, {pObjField.GetAttributes().Name}");
                    }
                    else {
                        LogEntry.WriteInfo($"Failed to add field: {pObjField.TableName}, {pObjField.GetAttributes().Name}: {DIApplication.Company.GetLastErrorDescription()}");
                    }
                }
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjUserField);
            }
        }

        private string GetFieldQuery(string pStrTableName, string pStrFieldName) => $"SELECT * FROM \"CUFD\" WHERE \"TableID\" = '{pStrTableName}' AND \"AliasID\" = '{pStrFieldName}'";
        #endregion
    }
}
