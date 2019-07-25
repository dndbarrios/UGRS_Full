using SAPbobsCOM;
using System;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.CreditNote.Tables
{
    [Table(Name = "UG_PE_NC", Description = "Nota de credito Permisos", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class CreditNoteT : Table
    {
        [Field(Description = "NcId", Type = BoFieldTypes.db_Alpha, Size = 64)]
        public string NcId { get; set; }

        [Field(Description = "Date", Type = BoFieldTypes.db_Date)]
        public DateTime Date { get; set; }

        [Field(Description = "Estatus", Type = BoFieldTypes.db_Alpha, Size = 2)]
        public string Status { get; set; }

        [Field(Description = "Total", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Price)]
        public float Total { get; set; }

        [Field(Description = "User", Size = 64)]
        public string User { get; set; }

        [Field(Description = "Cancelado", Size = 1)]
        public string Cancleado { get; set; }

        [Field(Description = "Date", Type = BoFieldTypes.db_Date)]
        public DateTime DateSaved { get; set; }

        [Field(Description = "Time", Size = 5)]
        public string Time { get; set; }
    }
}



