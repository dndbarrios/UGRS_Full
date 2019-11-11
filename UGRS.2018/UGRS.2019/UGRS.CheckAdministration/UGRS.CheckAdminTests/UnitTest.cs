using Microsoft.VisualStudio.TestTools.UnitTesting;
using UGRS.CheckAdminSDK.DAO;
using UGRS.CheckAdminSDK.SDK.DI;
using UGRS.CheckAdminSDK.Services;


namespace UGRS.CheckAdminTests {
    [TestClass]
    public class CheckAdministrationTests {

        CheckAdminDAO checkAdmonDAO = new CheckAdminDAO();

        public CheckAdministrationTests() {
            DIApplication.DIConnect();
        }

        [TestMethod]
        public void ConnectionTest() {
            Assert.IsTrue(DIApplication.Connected);
        }

        [TestMethod]
        public void GetAreasTest() {
            var areas = checkAdmonDAO.GetAreas();
            Assert.IsTrue(areas.Length > 0);
        }

        [TestMethod]
        public void ExistCheckTest() {
            var existCheck = checkAdmonDAO.ExistCheck("123");
            Assert.IsFalse(existCheck);
        }

        [TestMethod]
        public void GetTableKeyTest() {

            var key = checkAdmonDAO.GetTableKey("64001");
            Assert.IsTrue(key > 0);
        }

        [TestMethod]
        public void SendAlertTest() {

            var alertMessage = new AlertMessage();
            var result = alertMessage.Insert("MAQUINARIA", "Test");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetUserCostCenter() {

            var costCenter = checkAdmonDAO.GetUserArea("MAQUINARIA");
            Assert.AreEqual(costCenter, "MAQUINARIA");
        }

        [TestMethod]
        public void GetUserCodesTest() {

            var bankUserCode = checkAdmonDAO.GetBankUserCode();
            var archiveUserCode = checkAdmonDAO.GetArchiveUserCode();

            Assert.IsNotNull(bankUserCode);
            Assert.IsNotNull(archiveUserCode);
        }

        [TestMethod]
        public void GetUserToNotifyTest() {

            var user = checkAdmonDAO.GetUserToNotify("TRANSPORTE");
        }
    }
}
