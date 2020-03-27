// file:	Database\QueryManager.cs
// summary:	Implements the query manager class

using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Enums.Query;
using Core.SDK.DI.Exceptions;
using Core.Utility;
using Core.SDK.DI.Extension;
using Core.Extension;
using Core.SDK.DI.Models;

namespace Core.SDK.DI.DAO
{
    /// <summary> Manager for queries. </summary>
    /// <remarks> Ranaya, 09/05/2017. </remarks>

    public class QueryManager
    {
        public bool ExistsUserField(string pStrTableName, string pStrFieldName)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("TableName", pStrTableName);
                lLstStrParameters.Add("FieldName", pStrFieldName);

                lStrQuery = this.GetSQL("ExistsUserField").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                return lObjRecordSet.RecordCount > 0 ? true : false;
            }
            catch (Exception e)
            {
                throw new QueryException(e.Message, e);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public bool Exists(string pStrWhereFieldName, string pStrWhereFieldValue, string pStrTableName)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("WhereFieldName", pStrWhereFieldName);
                lLstStrParameters.Add("WhereFieldValue", pStrWhereFieldValue);
                lLstStrParameters.Add("TableName", pStrTableName);

                lStrQuery = this.GetSQL("ExistsTemplate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                return lObjRecordSet.RecordCount > 0 ? true : false;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public string GetValue(string pStrSelectFieldName, string pStrWhereFieldName, string pStrWhereFieldValue, string pStrTableName)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("SelectFieldName", pStrSelectFieldName);
                lLstStrParameters.Add("WhereFieldName", pStrWhereFieldName);
                lLstStrParameters.Add("WhereFieldValue", pStrWhereFieldValue);
                lLstStrParameters.Add("TableName", pStrTableName);

                lStrQuery = this.GetSQL("GetValueTemplate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return lObjRecordSet.Fields.Item(pStrSelectFieldName).Value.ToString();
                }

                return string.Empty;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public T GetTableObject<T>(string pStrWhereFieldName, string pStrWhereFieldValue, string pStrTableName) where T : Table
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("WhereFieldName", pStrWhereFieldName);
                lLstStrParameters.Add("WhereFieldValue", pStrWhereFieldValue);
                lLstStrParameters.Add("TableName", pStrTableName);

                lStrQuery = this.GetSQL("GetObjectTemplate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return lObjRecordSet.GetTableObject<T>();
                }
                return null;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public T GetTableObject<T>(string pStrTableName) where T : Table
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("TableName", pStrTableName);

                lStrQuery = this.GetSQL("GetObject").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return lObjRecordSet.GetTableObject<T>();
                }
                return null;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public IList<T> GetObjectsList<T>(string pStrWhereFieldName, string pStrWhereFieldValue, string pStrTableName) where T : Table
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            IList<T> lLstResult = new List<T>();
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("WhereFieldName", pStrWhereFieldName);
                lLstStrParameters.Add("WhereFieldValue", pStrWhereFieldValue);
                lLstStrParameters.Add("TableName", pStrTableName);

                lStrQuery = this.GetSQL("GetObjectTemplate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {
                        lLstResult.Add(lObjRecordSet.GetTableObject<T>());
                        lObjRecordSet.MoveNext();
                    }

                   
                }

                return lLstResult;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public int Count(string pStrWhereFieldName, string pStrWhereFieldValue, string pStrTableName)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("WhereFieldName", pStrWhereFieldName);
                lLstStrParameters.Add("WhereFieldValue", pStrWhereFieldValue);
                lLstStrParameters.Add("TableName", pStrTableName);

                lStrQuery = this.GetSQL("CountTemplate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                return (int)lObjRecordSet.Fields.Item("Count").Value;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public T Max<T>(string pStrFieldName, string pStrTableName) where T : IConvertible
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("FieldName", pStrFieldName);
                lLstStrParameters.Add("TableName", pStrTableName);

                lStrQuery = this.GetSQL("MaxTemplate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                return (T)Convert.ChangeType(lObjRecordSet.Fields.Item("Max").Value.ToString(), typeof(T));
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public T Max<T>(string pStrFieldName, string pStrWhereFieldName, string pStrWhereFieldValue, string pStrTableName) where T : IConvertible
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("FieldName", pStrFieldName);
                lLstStrParameters.Add("TableName", pStrTableName);
                lLstStrParameters.Add("WhereFieldName", pStrWhereFieldName);
                lLstStrParameters.Add("WhereFieldValue", pStrWhereFieldValue);

                lStrQuery = this.GetSQL("ConditionalMaxTemplate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                return (T)Convert.ChangeType(lObjRecordSet.Fields.Item("Max").Value.ToString(), typeof(T));
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public int GetSeriesByName(string pStrObjectCode, string pStrSeriesName)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("ObjectCode", pStrObjectCode);
                lLstStrParameters.Add("SeriesName", pStrSeriesName);

                lStrQuery = this.GetSQL("GetSeriesByNameTemplate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                return (int)lObjRecordSet.Fields.Item("Series").Value;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }
    }
}
