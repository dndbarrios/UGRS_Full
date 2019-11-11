using SAPbobsCOM;
using System.Collections.Generic;
using UGRS.CheckAdminSDK.SDK.DI.Attributes;


namespace UGRS.CheckAdminSDK.SDK.DI.DAO.Interfaces {
    public interface ITable {
        TableAttribute GetAttributes();
        string GetKey();
        string GetName();
        UserTablesMD GetUserTable();
        List<UGRS.CheckAdminSDK.SDK.DI.DAO.Models.Field> GetFields();
    }
}
