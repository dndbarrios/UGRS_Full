// file:	DAO\TableDAO.cs
// summary:	Implements the table dao class

using SAPbobsCOM;
using System;
using System.Linq;
using Core.SDK.DI.Exceptions;
using Core.SDK.DI.Models;
using Core.Utility;
using Core.Extension;
using System.Reflection;

namespace Core.SDK.DI.DAO
{
    /// <summary>
    /// A table dao.
    /// </summary>
    /// <remarks>
    /// Ranaya, 26/05/2017.
    /// </remarks>
    /// <typeparam name="T">
    /// Generic type parameter.
    /// </typeparam>

    public class TableDAO<T> : ITableDAO<T> where T : ITable
    {
        #region Public

        /// <summary>
        /// Initializes this object.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>

        public void Initialize()
        {
            //Get instance of current object
            T lObjInstance = GetInstance();

            //Create tables and fields if not exists
            InitializeTable(lObjInstance);
            InitializeFields(lObjInstance);
        }

        /// <summary>
        /// Adds pObjRecord.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <exception cref="TableException">
        /// Thrown when a Table error condition occurs.
        /// </exception>
        /// <param name="pObjRecord">
        /// The Object record to add.
        /// </param>
        /// <returns>
        /// An int.
        /// </returns>

        public int Add(T pObjRecord)
        {
            SAPbobsCOM.UserTable lObjUserTable = GetUserTable();

            try
            {
                lObjUserTable = PopulateUserTable(lObjUserTable, pObjRecord);
                return lObjUserTable.Add();
            }
            catch (Exception e)
            {
                throw new TableException(e.Message, e);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjUserTable);
            }
        }

        //public int Add(T pObjRecord, out string pStrCode)
        //{
        //    int lIntResult = -1;
        //    SAPbobsCOM.UserTable lObjUserTable = GetUserTable();
        //    pStrCode = "";

        //    try
        //    {
        //        lObjUserTable = PopulateUserTable(lObjUserTable, pObjRecord);
        //        lIntResult = lObjUserTable.Add();
        //        pStrCode = lObjUserTable.Code;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new TableException(e.Message, e);
        //    }
        //    finally
        //    {
        //        MemoryUtility.ReleaseComObject(lObjUserTable);
        //    }

        //    return lIntResult;
        //}

        /// <summary>
        /// Updates the given pObjRecord.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <exception cref="TableException">
        /// Thrown when a Table error condition occurs.
        /// </exception>
        /// <param name="pObjRecord">
        /// The object record.
        /// </param>
        /// <returns>
        /// An int.
        /// </returns>

        public int Update(T pObjRecord)
        {
            SAPbobsCOM.UserTable lObjUserTable = GetUserTable();

            try
            {
                if (lObjUserTable.GetByKey(pObjRecord.GetKey()))
                {
                    lObjUserTable = PopulateUserTable(lObjUserTable, pObjRecord);
                    return lObjUserTable.Update();
                }
                else
                {
                    throw new TableException(string.Format("No existe el registro '{0}'.", pObjRecord.GetKey()));
                }
            }
            catch (Exception e)
            {
                throw new TableException(e.Message, e);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjUserTable);
            }
        }

        /// <summary>
        /// Removes the given pStrDocEntry.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <exception cref="TableException">
        /// Thrown when a Table error condition occurs.
        /// </exception>
        /// <param name="pStrDocEntry">
        /// The String Document entry to remove.
        /// </param>
        /// <returns>
        /// An int.
        /// </returns>

        public int Remove(string pStrCode)
        {
            SAPbobsCOM.UserTable lObjUserTable = GetUserTable();

            try
            {
                if (lObjUserTable.GetByKey(pStrCode))
                {
                    return lObjUserTable.Remove();
                }
                else
                {
                    throw new TableException(string.Format("No existe el registro '{0}'.", pStrCode));
                }
            }
            catch (Exception e)
            {
                throw new TableException(e.Message, e);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjUserTable);
            }
        }

        #endregion

        #region Protected

        /// <summary>
        /// Gets user table.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <returns>
        /// The user table.
        /// </returns>

        public SAPbobsCOM.UserTable GetUserTable()
        {
            return DIApplication.Company.UserTables.Item(GetUserTableName());
        }

        /// <summary>
        /// Gets user table name.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <returns>
        /// The user table name.
        /// </returns>

        public string GetUserTableName()
        {
            return GetInstance().GetAttributes().Name;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <returns>
        /// The instance.
        /// </returns>

        protected T GetInstance()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        #endregion

        #region Private

        /// <summary>
        /// Populate user table.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <param name="pObjUserTable">
        /// The object user table.
        /// </param>
        /// <param name="pObjRecord">
        /// The object record.
        /// </param>
        /// <returns>
        /// A SAPbobsCOM.UserTable.
        /// </returns>

        private SAPbobsCOM.UserTable PopulateUserTable(SAPbobsCOM.UserTable pObjUserTable, T pObjRecord)
        {
            if (!string.IsNullOrEmpty(pObjRecord.GetName()))
            {
                pObjUserTable.Name = pObjRecord.GetName();
            }
                
            foreach (PropertyInfo lObjProperty in pObjRecord.GetType().GetProperties().Where(x => x.GetMethod.IsPublic && !x.GetMethod.IsVirtual))
            {
                try
                {
                    string lStrFieldName = string.Format("U_{0}", lObjProperty.Name);
                    Type lObjFieldType = lObjProperty.PropertyType;
                    object lUnkFieldValue = lObjFieldType == typeof(bool) ? ((bool)lObjProperty.GetValue(pObjRecord, null) ? "Y" : "N") : (lObjProperty.GetValue(pObjRecord, null));

                    pObjUserTable.UserFields.Fields.Item(lStrFieldName).Value = (lObjFieldType != typeof(DateTime) ? lUnkFieldValue.ToString() : lUnkFieldValue);
                }
                catch (Exception lObjException)
                {
                    //Ignore ;)
                    //LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
                }
            }

            return pObjUserTable;
        }

        /// <summary>
        /// Gets field identifier.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <exception cref="FieldException">
        /// Thrown when a Field error condition occurs.
        /// </exception>
        /// <param name="pStrTableName">
        /// Name of the string table.
        /// </param>
        /// <param name="pStrFieldName">
        /// Name of the string field.
        /// </param>
        /// <returns>
        /// The field identifier.
        /// </returns>

        private int GetFieldID(string pStrTableName, string pStrFieldName)
        {
            //Get recordset
            Recordset lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

            try
            {
                //Get field query and execute
                lObjRecordSet.DoQuery(GetFieldQuery(pStrTableName, pStrFieldName));

                //If field exists
                if (lObjRecordSet.RecordCount > 0)
                {
                    //Return field id
                    return Convert.ToInt32(lObjRecordSet.Fields.Item("FieldID").Value.ToString());
                }
                //If field not exists
                return -1;
            }
            catch (Exception e)
            {
                throw new FieldException(e.Message, e);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        /// <summary>
        /// Exists table.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <exception cref="TableException">
        /// Thrown when a Table error condition occurs.
        /// </exception>
        /// <param name="pStrTableName">
        /// Name of the string table.
        /// </param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>

        private bool ExistsTable(string pStrTableName)
        {
            UserTablesMD lObjUserTable = null;

            try
            {
                lObjUserTable = (UserTablesMD)DIApplication.Company.GetBusinessObject(BoObjectTypes.oUserTables);
                return lObjUserTable.GetByKey(pStrTableName);
            }
            catch (Exception e)
            {
                throw new TableException(e.Message, e);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjUserTable);
            }
        }

        /// <summary>
        /// Exists field.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <exception cref="TableException">
        /// Thrown when a Table error condition occurs.
        /// </exception>
        /// <param name="pStrTableName">
        /// Name of the string table.
        /// </param>
        /// <param name="pIntFieldIndex">
        /// Zero-based index of the int field.
        /// </param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>

        private bool ExistsField(string pStrTableName, int pIntFieldIndex)
        {
            UserFieldsMD lObjUserField = null;

            try
            {
                lObjUserField = (UserFieldsMD)DIApplication.Company.GetBusinessObject(BoObjectTypes.oUserFields);
                return lObjUserField.GetByKey(string.Format("@{0}", pStrTableName), pIntFieldIndex);
            }
            catch (Exception e)
            {
                throw new TableException(e.Message, e);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjUserField);
            }
        }

        /// <summary>
        /// Exists field.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <exception cref="FieldException">
        /// Thrown when a Field error condition occurs.
        /// </exception>
        /// <param name="pStrTableName">
        /// Name of the string table.
        /// </param>
        /// <param name="pStrFieldName">
        /// Name of the string field.
        /// </param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>

        private bool ExistsField(string pStrTableName, string pStrFieldName)
        {
            //Get recordset
            Recordset lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

            try
            {
                //Get field query and execute
                lObjRecordSet.DoQuery(GetFieldQuery(pStrTableName, pStrFieldName));

                //Return result
                return lObjRecordSet.RecordCount > 0 ? true : false;
            }
            catch (Exception e)
            {
                throw new FieldException(e.Message, e);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        /// <summary>
        /// Initializes the table.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <param name="pObjTable">
        /// The object table.
        /// </param>

        private void InitializeTable(T pObjTable)
        {
            UserTablesMD lObjUserTable = pObjTable.GetUserTable();

            try
            {
                if(!ExistsTable(lObjUserTable.TableName))
                {
                    HandleException.Table(lObjUserTable.Add());
                }
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjUserTable);
            }
        }

        /// <summary>
        /// Initializes the fields.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <param name="pObjTable">
        /// The object table.
        /// </param>

        private void InitializeFields(T pObjTable)
        {
            foreach (Models.Field lObjField in pObjTable.GetFields())
            {
                InitializeField(lObjField);
            }
        }

        /// <summary>
        /// Initializes the field.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <param name="pObjField">
        /// The object field.
        /// </param>

        private void InitializeField(Models.Field pObjField)
        {
            UserFieldsMD lObjUserField = null;

            try
            {
                if (!ExistsField(pObjField.TableName, pObjField.GetAttributes().Name))
                {
                    lObjUserField = pObjField.GetUserField();
                    HandleException.Field(lObjUserField.Add());
                }
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjUserField);
            }
        }

        /// <summary>
        /// Gets table query.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <param name="pStrTableName">
        /// Name of the string table.
        /// </param>
        /// <returns>
        /// The table query.
        /// </returns>

        private string GetTableQuery(string pStrTableName)
        {
            return string.Format("SELECT * FROM CUFD WHERE TableName = '{0}'", pStrTableName);
        }

        /// <summary>
        /// Gets field query.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <param name="pStrTableName">
        /// Name of the string table.
        /// </param>
        /// <param name="pStrFieldName">
        /// Name of the string field.
        /// </param>
        /// <returns>
        /// The field query.
        /// </returns>

        private string GetFieldQuery(string pStrTableName, string pStrFieldName)
        {
            return string.Format("SELECT * FROM CUFD WHERE TableID = '{0}' AND AliasID = '{1}'", pStrTableName, pStrFieldName);
        }

        #endregion
    }
}
