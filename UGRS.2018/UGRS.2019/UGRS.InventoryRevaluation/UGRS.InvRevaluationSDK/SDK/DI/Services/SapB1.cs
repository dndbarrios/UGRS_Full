using UGRS.InvRevaluationSDK.SDK.DAO;

namespace UGRS.InvRevaluationSDK.SDK.DI.Services {
    public class SapB1 {

        public InvRevaluationDAO DAO;
        public InventoryRevaluation InventoryRevaluation;

        public SapB1() {
            DAO = new InvRevaluationDAO();
            InventoryRevaluation = new InventoryRevaluation();
        }
    }
}
