
using UGRS.CheckAdminSDK.SDK.DI.DAO;
using UGRS.CheckAdminSDK.SDK.Tables;

namespace UGRS.CheckAdminSDK.Setups {
    public class CheckAdminSetup {

        private readonly TableDAO<CheckAdmin> checkAdmin;

        public CheckAdminSetup() {
            checkAdmin = new TableDAO<CheckAdmin>();
        }

        public void InitializeTables() {
            checkAdmin.Initialize();
        }
    }
}
