// file:	dao\objectdao.cs
// summary:	Implements the objectdao class

using SAPbobsCOM;
using System;
using System.Reflection;
using Core.SDK.DI.Exceptions;
using Core.SDK.DI.Models;
using Core.Utility;
using System.Linq;

namespace Core.SDK.DI.DAO
{
    /// <summary> An object dao. </summary>
    /// <remarks> Ranaya, 26/05/2017. </remarks>
    /// <typeparam name="T"> Generic type parameter. </typeparam>

    public class ObjectDAO<T> : IObjectDAO<T> where T : IObject
    {
        #region Public

        /// <summary> Initializes this object. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>

        public void Initialize()
        {
            //Get instance of current object
            T lObjInstance = GetInstance();

            //Create tables and fields if not existsm
            InitializeTable(lObjInstance);
            InitializeFields(lObjInstance);

            //Register as object
            InitializeObject(lObjInstance);
        }

        /// <summary> Adds pObjRecord. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <exception cref="TableException"> Thrown when a Table error condition occurs. </exception>
        /// <param name="pObjRecord"> The Object record to add. </param>

        public void Add(T pObjRecord)
        {
            SAPbobsCOM.GeneralService lObjGeneralService = null;
            SAPbobsCOM.GeneralData lObjGeneralData = null;
            SAPbobsCOM.GeneralDataParams lObjGeneralDataParams = null;
            SAPbobsCOM.CompanyService lObjCompanyService = null;

            try
            {
                //Initialize variables
                lObjCompanyService = DIApplication.Company.GetCompanyService();
                lObjGeneralService = lObjCompanyService.GetGeneralService(GetObjectCode());
                lObjGeneralData = ((SAPbobsCOM.GeneralData)(lObjGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData)));

                //Set data
                lObjGeneralData = PopulateGeneralData(lObjGeneralData, pObjRecord);

                //Add data
                lObjGeneralDataParams = lObjGeneralService.Add(lObjGeneralData);
            }
            catch (Exception e)
            {
                throw new TableException(e.Message, e);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjGeneralService, lObjGeneralData, lObjGeneralDataParams, lObjCompanyService);
            }
        }

        /// <summary> Updates the given pObjRecord. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <exception cref="TableException"> Thrown when a Table error condition occurs. </exception>
        /// <param name="pObjRecord"> The object record. </param>

        public void Update(T pObjRecord)
        {
            SAPbobsCOM.GeneralService lObjGeneralService = null;
            SAPbobsCOM.GeneralData lObjGeneralData = null;
            SAPbobsCOM.GeneralDataParams lObjGeneralDataParams = null;
            SAPbobsCOM.CompanyService lObjCompanyService = null;

            try
            {
                //Initialize variables
                lObjCompanyService = DIApplication.Company.GetCompanyService();
                lObjGeneralService = lObjCompanyService.GetGeneralService(GetObjectCode());
                lObjGeneralDataParams = ((SAPbobsCOM.GeneralDataParams)(lObjGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)));

                //Get current data by params
                lObjGeneralDataParams.SetProperty("DocEntry", pObjRecord.GetKey());
                lObjGeneralData = lObjGeneralService.GetByParams(lObjGeneralDataParams);

                //Set new data
                lObjGeneralData = PopulateGeneralData(lObjGeneralData, pObjRecord);

                //Update data
                lObjGeneralService.Update(lObjGeneralData);
            }
            catch (Exception e)
            {
                throw new TableException(e.Message, e);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjGeneralService, lObjGeneralData, lObjGeneralDataParams, lObjCompanyService);
            }
        }

        /// <summary> Removes the given pStrDocEntry. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <exception cref="TableException"> Thrown when a Table error condition occurs. </exception>
        /// <param name="pStrDocEntry"> The String Document entry to remove. </param>

        public void Remove(string pStrDocEntry)
        {
            SAPbobsCOM.GeneralService lObjGeneralService = null;
            SAPbobsCOM.GeneralData lObjGeneralData = null;
            SAPbobsCOM.GeneralDataParams lObjGeneralDataParams = null;
            SAPbobsCOM.CompanyService lObjCompanyService = null;

            try
            {
                //Initialize variables
                lObjCompanyService = DIApplication.Company.GetCompanyService();
                lObjGeneralService = lObjCompanyService.GetGeneralService(GetObjectCode());
                lObjGeneralDataParams = ((SAPbobsCOM.GeneralDataParams)(lObjGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)));

                //Set docentry
                lObjGeneralDataParams.SetProperty("DocEntry", pStrDocEntry);

                //Delete data
                lObjGeneralService.Delete(lObjGeneralDataParams);
            }
            catch (Exception e)
            {
                throw new TableException(e.Message, e);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjGeneralService, lObjGeneralData, lObjGeneralDataParams, lObjCompanyService);
            }
        }

        /// <summary> Populate user table. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <param name="pObjGeneralData"> The object user table. </param>
        /// <param name="pObjRecord">      The object record. </param>
        /// <returns> A SAPbobsCOM.UserTable. </returns>

        private SAPbobsCOM.GeneralData PopulateGeneralData(SAPbobsCOM.GeneralData pObjGeneralData, T pObjRecord)
        {
            foreach (PropertyInfo lObjProperty in pObjRecord.GetType().GetProperties().Where(x => x.GetMethod.IsPublic && !x.GetMethod.IsVirtual))
            {
                string lStrFieldName = string.Format("U_{0}", lObjProperty.Name);
                object lUnkFieldValue = lObjProperty.GetType() == typeof(bool) ? ((bool)lObjProperty.GetValue(pObjRecord, null) ? "Y" : "N") : (lObjProperty.GetValue(pObjRecord, null));

                pObjGeneralData.SetProperty(lStrFieldName, lUnkFieldValue);
            }
            return pObjGeneralData;
        }

        #endregion

        #region Protected

        /// <summary> Gets user table. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <returns> The user table. </returns>

        protected SAPbobsCOM.UserTable GetUserTable()
        {
            return DIApplication.Company.UserTables.Item(GetUserTableName());
        }

        /// <summary> Gets object code. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <returns> The object code. </returns>

        protected string GetObjectCode()
        {
            return GetInstance().GetAttributes().ObjectCode;
        }

        /// <summary> Gets user table name. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <returns> The user table name. </returns>

        protected string GetUserTableName()
        {
            return GetInstance().GetAttributes().Name;
        }

        /// <summary> Gets the instance. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <returns> The instance. </returns>

        protected T GetInstance()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        #endregion

        #region Private

        /// <summary> Gets field identifier. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <exception cref="FieldException"> Thrown when a Field error condition occurs. </exception>
        /// <param name="pStrTableName"> Name of the string table. </param>
        /// <param name="pStrFieldName"> Name of the string field. </param>
        /// <returns> The field identifier. </returns>

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

        /// <summary> Exists table. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <exception cref="TableException"> Thrown when a Table error condition occurs. </exception>
        /// <param name="pStrTableName"> Name of the string table. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>

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

        /// <summary> Exists field. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <exception cref="TableException"> Thrown when a Table error condition occurs. </exception>
        /// <param name="pStrTableName">  Name of the string table. </param>
        /// <param name="pIntFieldIndex"> Zero-based index of the int field. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>

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

        /// <summary> Exists field. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <exception cref="FieldException"> Thrown when a Field error condition occurs. </exception>
        /// <param name="pStrTableName"> Name of the string table. </param>
        /// <param name="pStrFieldName"> Name of the string field. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>

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

        /// <summary> Exists object. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <exception cref="TableException"> Thrown when a Table error condition occurs. </exception>
        /// <param name="pStrObjectCode"> The string object code. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>

        private bool ExistsObject(string pStrObjectCode)
        {
            UserTablesMD lObjUserTable = null;

            try
            {
                lObjUserTable = (UserTablesMD)DIApplication.Company.GetBusinessObject(BoObjectTypes.oUserObjectsMD);
                return lObjUserTable.GetByKey(pStrObjectCode);
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

        /// <summary> Initializes the table. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <param name="pObjObject"> The object object. </param>

        private void InitializeTable(T pObjObject)
        {
            UserTablesMD lObjUserTable = pObjObject.GetUserTable();

            try
            {
                if (!ExistsTable(lObjUserTable.TableName))
                {
                    HandleException.Table(lObjUserTable.Add());
                }
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjUserTable);
            }
        }

        /// <summary> Initializes the fields. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <param name="pObjObject"> The object object. </param>

        private void InitializeFields(T pObjObject)
        {
            foreach (Models.Field lObjField in pObjObject.GetFields())
            {
                InitializeField(lObjField);
            }
        }

        /// <summary> Initializes the field. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <param name="pObjField"> The object field. </param>

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

        /// <summary> Initializes the object. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <param name="pObjObject"> The object object. </param>

        private void InitializeObject(IObject pObjObject)
        {
            UserObjectsMD lObjUserObject = pObjObject.GetUserObject();

            try
            {
                if(!ExistsObject(lObjUserObject.Code))
                {
                    HandleException.Object(lObjUserObject.Add());
                }
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjUserObject);
            }
        }

        /// <summary> Gets table query. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <param name="pStrTableName"> Name of the string table. </param>
        /// <returns> The table query. </returns>

        private string GetTableQuery(string pStrTableName)
        {
            return string.Format("SELECT * FROM CUFD WHERE TableName = '{0}'", pStrTableName);
        }

        /// <summary> Gets field query. </summary>
        /// <remarks> Ranaya, 26/05/2017. </remarks>
        /// <param name="pStrTableName"> Name of the string table. </param>
        /// <param name="pStrFieldName"> Name of the string field. </param>
        /// <returns> The field query. </returns>

        private string GetFieldQuery(string pStrTableName, string pStrFieldName)
        {
            return string.Format("SELECT * FROM CUFD WHERE TableID = '{0}' AND AliasID = '{1}'", pStrTableName, pStrFieldName);
        }

        #endregion
    }
}
