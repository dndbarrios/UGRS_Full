using SAPbobsCOM;
using System.Collections.Specialized;
using System.Configuration;
using UGRS.Core.Extensions;
using UGRS.Core.Utility;

namespace UGRS.CheckAdminSDK.SDK.DI {

    public class DIApplication {

        private static bool connected;
        private static Company company;

        public static Company Company {
            get { return company; }
            private set { company = value; }
        }

        public static bool Connected {
            get { return connected; }
            private set { connected = value; }
        }

        public static void DIConnect(Company company) {
            Company = company;
            Connected = true;
        }
        public static void DIConnect() {

            if (!DIApplication.Connected) {

                string erroMsg = string.Empty;
                int errorCode = 0;

                Company = BuildCompany();

                if (Company.Connect() != 0) {
                    Company.GetLastError(out errorCode, out erroMsg);
                    LogEntry.WriteError($"Code: {errorCode}, Description: {erroMsg}");
                }

                if (Company != null && Company.Connected) {
                    DIApplication.DIConnect(Company);
                }
            }
        }

        private static Company BuildCompany() {
            var company = new Company();
            company.DbServerType = GetSAPDbServerType("dst_MSSQL2012");

            foreach (var key in (ConfigurationManager.GetSection("SAP/Credentials") as NameValueCollection).AllKeys) {
                company.SetPropertyValue(key, (ConfigurationManager.GetSection("SAP/Credentials") as NameValueCollection)[key]);
            }
            return company;
        }

        public static Recordset GetRecordset() {
            return (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
        }

        static BoDataServerTypes GetSAPDbServerType(string serverType) {

            switch (serverType) {
                case "dst_MSSQL":
                    return BoDataServerTypes.dst_MSSQL;
                case "dst_DB_2":
                    return BoDataServerTypes.dst_DB_2;
                case "dst_SYBASE":
                    return BoDataServerTypes.dst_SYBASE;
                case "dst_MSSQL2005":
                    return BoDataServerTypes.dst_MSSQL2005;
                case "dst_MSSQL2008":
                    return BoDataServerTypes.dst_MSSQL2008;
                case "dst_MAXDB":
                    return BoDataServerTypes.dst_MAXDB;
                case "dst_MSSQL2012":
                    return BoDataServerTypes.dst_MSSQL2012;
                case "dst_MSSQL2014":
                    return BoDataServerTypes.dst_MSSQL2014;
                case "dst_HANADB":
                    return BoDataServerTypes.dst_HANADB;
                case "dst_MSSQL2016":
                    return BoDataServerTypes.dst_MSSQL2016;
                default:
                    return BoDataServerTypes.dst_MSSQL2016;

            }
        }
    }
}
