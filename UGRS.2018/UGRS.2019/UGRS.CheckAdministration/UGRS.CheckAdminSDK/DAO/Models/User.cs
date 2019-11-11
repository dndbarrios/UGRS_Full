using System.Threading.Tasks;
using UGRS.CheckAdminSDK.DAO;
using UGRS.CheckAdminSDK.SDK.DI;
using UGRS.Core.Extensions;

namespace UGRS.CheckAdminSDK.Models {
    public class User {

        public string Code { get; set; }
        public string Area { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBank { get; set; }
        public bool IsArchive { get; set; }
        public string Phase { get; set; }
        public string DeliverTo { get; set; }
        public string BankUserCode { get; set; }
        public string ArchiveUserCode { get; set; }

        public User() {

            var checkAdminDAO = new CheckAdminDAO();
            Code = DIApplication.Company.UserName.ToUpper();

            Parallel.Invoke(
                () => Area = checkAdminDAO.GetUserArea(Code),
                () => BankUserCode = checkAdminDAO.GetBankUserCode().ToUpper(),
                () => ArchiveUserCode = checkAdminDAO.GetArchiveUserCode().ToUpper()
            );

            SetRoles();
            SetPhases();
        }

        private void SetRoles() {
            IsAdmin = Code.ToUpper().IsOneOf(BankUserCode, ArchiveUserCode);
            IsBank = Code.ToUpper().Contains(BankUserCode);
            IsArchive = Code.ToUpper().IsOneOf(ArchiveUserCode);
        }

        private void SetPhases() {
            if (IsBank) {
                Phase = "BANCO";
                DeliverTo = "AREA";
            }
            else if (IsArchive) {
                Phase = "ARCHIVO";
            }
            else if (!IsBank && !IsArchive) {
                Phase = "AREA";
                DeliverTo = "BANCO";
            }
        }
    }
}
