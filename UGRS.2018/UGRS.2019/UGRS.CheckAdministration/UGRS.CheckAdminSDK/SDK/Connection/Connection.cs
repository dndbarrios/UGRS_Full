using SAPbobsCOM;

namespace UGRS.CheckAdminSDK.SDK.Connection {

    public interface IConnection {
        string GetConnectionString();
    }

    public interface IDIConnection : IConnection {

        string Token {
            get;
        }

        Company GetCompany();

        void ReconnectDI();

        void SetCompany(Company company);
    }

    public class Connection : IConnection {
        protected string ConnectionString;
        public string GetConnectionString() {
            return this.ConnectionString;
        }
    }
}
