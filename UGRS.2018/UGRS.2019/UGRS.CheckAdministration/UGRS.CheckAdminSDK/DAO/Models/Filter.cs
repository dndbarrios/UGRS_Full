
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Extensions;
using UGRS.Core.Utility;

namespace UGRS.CheckAdminSDK.Models {
    public class Filter {
        public string From { get; set; }
        public string To { get; set; }
        public string Number { get; set; }
        public string CheckAc { get; set; }
        public string Status { get; set; }

        public List<string> BuildQueryParameters(User user) {

            var filters = new List<string>();

            var table = "OCHO";
            var udt = "CHECKADMIN";
            string join1 = " INNER JOIN OUSR T2 ON OCHO.UserSign = T2.USERID";
            string join2 = " INNER JOIN \"@UG_GLO_COSTCENT\" T3 ON T3.\"Code\" = T2.U_GLO_CostCenter";
            string notCanceled = $"{table}.CANCELED = 'N'";
            string wereAnd = user.IsAdmin ? $" WHERE {notCanceled} AND" : $" {join1} {join2} WHERE {notCanceled} AND CHECKADMIN.U_Area = '{user.Area}' AND";

            try {

                var dateField = $"{table}.CheckDate";
                if (!String.IsNullOrEmpty(this.From) && !String.IsNullOrEmpty(this.To)) {
                    filters.Add($"{wereAnd} {dateField} BETWEEN '{this.From}' AND '{this.To}'");
                    wereAnd = "AND";
                }
                else if (!String.IsNullOrEmpty(this.From) && String.IsNullOrEmpty(this.To)) {
                    filters.Add($"{wereAnd} {dateField} >= '{this.From}'");
                    wereAnd = "AND";
                }
                else if (String.IsNullOrEmpty(this.From) && !String.IsNullOrEmpty(this.To)) {
                    filters.Add($"{wereAnd} {dateField} <= '{this.To}'");
                    wereAnd = "AND";
                }

                if (!String.IsNullOrEmpty(this.Number)) {
                    filters.Add($"{wereAnd} {table}.CheckNum = '{this.Number}'");
                    wereAnd = "AND";
                }

                if (!String.IsNullOrEmpty(this.CheckAc)) {
                    filters.Add($"{wereAnd} {table}.CheckAcct LIKE '%{this.CheckAc}%'");
                    wereAnd = "AND";
                }

                if (!String.IsNullOrEmpty(this.Status)) {
                    filters.Add($"{wereAnd} {udt}.U_Status LIKE '%{this.Status}%'");
                    wereAnd = "AND";
                }

                if (user.IsArchive) {
                    filters.Add($"{wereAnd} {udt}.U_Status LIKE '%ARCHIVO'");
                }


                if (filters.Count <= 0) {
                    wereAnd = wereAnd.Remove(wereAnd.Length - 3);
                    filters.Add(wereAnd);
                }
            }
            catch (Exception ex) {
                LogEntry.WriteException(ex);
            }


            return filters;
        }


        public static Filter BuildFilter(EditText[] editTexts, ComboBox cbxStatus) {
            var filter = new Filter();
            editTexts.AsParallel().ForAll(text => {
                SetFilterValue(filter, text.Value, text.Item.UniqueID.Substring(3, text.Item.UniqueID.Length - 3));
            });

            filter.Status = cbxStatus.Value;
            return filter;
        }

        private static void SetFilterValue(Filter filter, string value, string property) {
            if (!String.IsNullOrEmpty(value)) {
                filter.SetPropertyValue(property, value.ToUpper());
            }
        }
    }
}
