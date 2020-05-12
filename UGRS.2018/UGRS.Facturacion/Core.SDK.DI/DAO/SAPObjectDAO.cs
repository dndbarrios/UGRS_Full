using SAPbobsCOM;
using System;
using Core.SDK.DI.Exceptions;
using Core.SDK.DI.Models;
using Core.Utility;

namespace Core.SDK.DI.DAO
{
    public class SAPObjectDAO<T> where T : ISAPObject
    {
        private QueryManager mObjQueryManager;

        public SAPObjectDAO()
        {
            mObjQueryManager = new QueryManager();
        }

        public void InitializeUserFields()
        {
            //Get instance of current object
            T lObjInstance = GetInstance();

            //Create user fields if not exists
            InitializeFields(lObjInstance);
        }

        protected T GetInstance()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        private void InitializeFields(T pObjObject)
        {
            foreach (Models.Field lObjField in pObjObject.GetFields())
            {
                InitializeField(lObjField);
            }
        }

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

        private bool ExistsField(string pStrTableName, string pStrFieldName)
        {
            return mObjQueryManager.ExistsUserField(pStrTableName, pStrFieldName);
        }
    }
}
