using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.CheckAdminSDK.Models {
    public class Result {
        public bool Success { get; set; } = false;
        public string Msg { get; set; }
        public int Row { get; set; }
    }
}
