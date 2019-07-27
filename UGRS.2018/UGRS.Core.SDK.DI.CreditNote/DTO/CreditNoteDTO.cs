using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.Core.SDK.DI.CreditNote.DTO
{
    public class CreditNoteDTO
    {
        public string C_CardCode { get; set; }
        public string C_CardName { get; set; }
        public string C_Cert { get; set; }
        public int C_DocEntry { get; set; }
        public string C_DocNum { get; set; }
        public int C_InvHead { get; set; }
        public int C_HeadExp { get; set; }
        public int C_HeadNoC  { get; set; }
        public float C_Amount { get; set; }
        public List<string> LstDocEntry { get; set; }
    }
}
