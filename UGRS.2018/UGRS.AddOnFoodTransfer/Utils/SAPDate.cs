/*
 * Autor: LCC Abraham SaÚL Sandoval Meneses
 * Description: SAP B1 Matrix
 * Date: 28/09/2018
 */


using SAPbouiCOM;
using System;
using System.Globalization;


namespace UGRS.AddOnFoodTransfer.Utils {

    public class SAPDate {

        public static DateTime ParseDate(string date) {
            string[] dateFormat = { "MM-dd-yyyy", "dd-MM-yyyy", "MM/dd/yyyy", "dd/MM/yyyy", "MMddyy", "ddMMyy", "Mddyy", "yyyyMMdd" };
            DateTime dateTime = DateTime.Now;
            if (DateTime.TryParseExact(date, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime)) {
                return dateTime;
            }
            else {
                return DateTime.Now;
            }
        }
        public static bool ValidateDate(EditText txtDate, int diffDays) {

            DateTime selectedDate = ParseDate(txtDate.Value);
            double diff = Math.Ceiling((selectedDate - DateTime.Now.AddDays(-diffDays)).TotalDays);

            if (!(diff <= 3 && diff >= 0)) {
                txtDate.Value = DateTime.Now.ToString("yyyyMMdd");
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Solo se aceptan 3 días hacia atras como máximo a partir de la fecha actual");
                return false;
            }
            else {
                return true;
            }
        }
    }
}
