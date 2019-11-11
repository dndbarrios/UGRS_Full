using SAPbobsCOM;
using UGRS.CheckAdminSDK.SDK.DI.Attributes;
using UGRS.CheckAdminSDK.SDK.DI.DAO.Models;
using UGRS.CheckAdminSDK.SDK.DI.DAO.Interfaces;
using UGRS.CheckAdminSDK.Models;

namespace UGRS.CheckAdminSDK.SDK.Tables {
    [Table(Name = "UG_CheckAdmin", Description = "Administración de Cheques", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class CheckAdmin : Table {

        [Key]
        [Field(Description = "Número de Cheque", Size = 20, Type = BoFieldTypes.db_Numeric)]
        public string CheckNum { get; set; }

        [Field(Description = "Area", Size = 100, Type = BoFieldTypes.db_Alpha)]
        public string Area { get; set; }

        [Field(Description = "Estatus", Size = 250, Type = BoFieldTypes.db_Alpha)]
        public string Status { get; set; }

        [Field(Description = "Fase de Autorización", Size = 50, Type = BoFieldTypes.db_Alpha)]
        public string Phase { get; set; }

        public CheckAdmin() {}

        public CheckAdmin(string checkNum, string area, string status) {
            this.CheckNum = checkNum;
            this.Area = area;
            this.Status = status;
        }
    }

    public static class CheckAdminExtension {
        public static void Fill(this CheckAdmin checkAdmin, string checkNum, string area, string status) {
            checkAdmin.CheckNum = checkNum;
            checkAdmin.Area = area;
            checkAdmin.Status = status;
        }
    }
}








