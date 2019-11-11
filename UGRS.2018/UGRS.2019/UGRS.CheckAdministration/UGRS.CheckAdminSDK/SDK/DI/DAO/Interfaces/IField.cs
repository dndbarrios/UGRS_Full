using UGRS.CheckAdminSDK.SDK.DI.Attributes;
using SAPbobsCOM;

namespace UGRS.CheckAdminSDK.SDK.DI.DAO.Interfaces {
    public interface IField {
        FieldAttribute GetAttributes();
        UserFieldsMD GetUserField();
    }
}
