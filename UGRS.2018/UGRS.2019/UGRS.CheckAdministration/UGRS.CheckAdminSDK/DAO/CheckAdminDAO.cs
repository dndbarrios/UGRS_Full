using UGRS.CheckAdminSDK.Models;
using UGRS.Core.Extensions;
using System.Text;
using System.Collections.Generic;
using System;

namespace UGRS.CheckAdminSDK.DAO {
    public class CheckAdminDAO : BaseDAO {

        public Check[] GetChecks(Filter filter, User user) {

            var queryBuilder = new StringBuilder().Append(this.GetSQL("GetChecks"));
            filter.BuildQueryParameters(user).ForEach(q => queryBuilder.Append(" " + q));
            var query = queryBuilder.ToString();

            return GetArray<Check>(query);
        }

        public string[] GetAreas() => GetFieldValues<string>("PrcName", "OPRC", "DimCode", "1");
        public bool ExistCheck(string checkNum) => Exist("@UG_CHECKADMIN", "U_CheckNum", checkNum);
        public string GetBankUserCode() => GetFieldValue("U_Value", "@UG_CONFIG", "Name", "FZ_CE_AUT2");
        public string GetArchiveUserCode() => GetFieldValue("U_Value", "@UG_CONFIG", "Name", "FZ_CE_AUT3");
        public int GetTableKey(string num) => GetFieldValue("Code", "@UG_CHECKADMIN", "U_CheckNum", num);
        private bool Exist(string table, string field, string value) => (GetCount(table, field, value) > 0 ? true : false);
        public string GetUserToNotify(string area) => GetValue<string>(this.GetSQL("GetUserToAlert").Replace("{Area}", area));
        public string GetUserArea(string userCode) => GetValue<string>(this.GetSQL("GetUserArea").Replace("{UserCode}", userCode));

    }
}
