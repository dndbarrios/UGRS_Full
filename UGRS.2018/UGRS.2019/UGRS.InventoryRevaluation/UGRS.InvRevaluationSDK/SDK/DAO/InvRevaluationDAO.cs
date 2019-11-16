using UGRS.Core.Extensions;
using UGRS.InvRevaluationSDK.Models;

namespace UGRS.InvRevaluationSDK.SDK.DAO {
    public class InvRevaluationDAO : BaseDAO {


        public bool ExistRevaluation(string docNum) => Exist("OMRV", "U_DocNumSalida", docNum);
        public int RevaluationsCount(string docNum) => (GetCount("OMRV", "U_DocNumSalida", docNum));
        private bool Exist(string table, string field, string value) => (GetCount(table, field, value) > 0 ? true : false);
        public RevaluationItem[] GetInvRevaluationItems(string docNum) => GetArray<RevaluationItem>(this.GetSQL($"GetRevaluation").Replace("{DocNum}", docNum));
    }
}
