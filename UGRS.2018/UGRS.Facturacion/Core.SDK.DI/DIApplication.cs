using SAPbobsCOM;
using System;
using Core.SDK.Connection;
using Core.SDK.Models;
using Core.Utility;

namespace Core.SDK.DI
{
    public class DIApplication
    {
        private static bool mBolConnected;

        private static SAPbobsCOM.Company mObjCompany;

        public static SAPbobsCOM.Company Company
        {
            get { return mObjCompany; }
            private set { mObjCompany = value; }
        }

        public static bool Connected
        {
            get { return mBolConnected; }
            private set { mBolConnected = value; }
        }

        public static void DIConnect(SAPbobsCOM.Company pObjCompany)
        {
            Company = pObjCompany;
            Connected = true;
        }

        public static void DIConnect()
        {
            if (!DIApplication.Connected)
            {
                DIConnection lObjDIConnection = new DIConnection();
                lObjDIConnection.ConnectToDI(GetCredentials());

                if (lObjDIConnection.Company != null && lObjDIConnection.Company.Connected)
                {
                    DIApplication.DIConnect(lObjDIConnection.Company);
                }
            }
        }

        public static void DIReconnect()
        {
            try
            {
                if (Company != null)
                {
                    Company.Disconnect();
                    MemoryUtility.ReleaseComObject(Company);
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.WriteException(lObjException);
            }

            DIConnection lObjDIConnection = new DIConnection();
            lObjDIConnection.ConnectToDI(GetCredentials());

            if (lObjDIConnection.Company != null && lObjDIConnection.Company.Connected)
            {
                DIApplication.DIConnect(lObjDIConnection.Company);
            }
        }

        public static Recordset GetRecordset()
        {
            return (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
        }

        public static Credentials GetCredentials()
        {
            return new Credentials()
            {
                LicenseServer = ConfigurationUtility.GetValue<string>("LicenseServer"),
                UserName = ConfigurationUtility.GetValue<string>("UserName"),
                Password = ConfigurationUtility.GetValue<string>("Password"),
                DbServerType = ConfigurationUtility.GetValue<SAPbobsCOM.BoDataServerTypes>("DbServerType"),
                SQLServer = ConfigurationUtility.GetValue<string>("SQLServer"),
                SQLUserName = ConfigurationUtility.GetValue<string>("SQLUserName"),
                SQLPassword = ConfigurationUtility.GetValue<string>("SQLPassword"),
                DataBaseName = ConfigurationUtility.GetValue<string>("DataBaseName"),
                Language = ConfigurationUtility.GetValue<SAPbobsCOM.BoSuppLangs>("Language"),
            };
        }
    }
}
