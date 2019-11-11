using UGRS.CheckAdminSDK.DAO;

namespace UGRS.CheckAdminSDK.Services {
    public class SapB1 {
        readonly public CheckAdministration CheckAdministration;
        readonly public AlertMessage AlertMessage;
        readonly public CheckAdminDAO DAO = new CheckAdminDAO();

        public SapB1() {
            CheckAdministration = new CheckAdministration();
            AlertMessage = new AlertMessage();
            DAO = new CheckAdminDAO();
        }
    }
}
