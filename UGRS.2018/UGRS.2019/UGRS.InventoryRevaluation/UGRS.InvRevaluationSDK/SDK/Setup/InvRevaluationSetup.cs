using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.InvRevaluationSDK.SDK.Setup {
    public class InvRevaluationSetup {
        public static void InitializeFields() {
            UserDefinedFields userDefinedField = new UserDefinedFields();
            userDefinedField.CreateFields("OMRV", "DocNumSalida", "Referencia Salida de Mercancia", SAPbobsCOM.BoFieldTypes.db_Alpha, 15);
        }
    }
}
