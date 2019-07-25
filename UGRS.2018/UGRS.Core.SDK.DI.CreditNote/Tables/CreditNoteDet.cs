using SAPbobsCOM;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.CreditNote.Tables
{
    [Table(Name = "UG_PE_NCDET", Description = "Nota de credito Permisos", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class CreditNoteDet : Table
    {
        [Field(Description = "NcId", Type = BoFieldTypes.db_Alpha, Size = 16)]
        public string NcId { get; set; }

        [Field(Description = "Line", Type = BoFieldTypes.db_Numeric)]
        public int Line { get; set; }

        [Field(Description = "CardCode", Type = BoFieldTypes.db_Alpha, Size = 16)]
        public string CardCode { get; set; }

        [Field(Description = "CardName", Type = BoFieldTypes.db_Alpha, Size = 64)]
        public string CardName { get; set; }

        [Field(Description = "Cert", Type = BoFieldTypes.db_Alpha, Size = 16)]
        public string Cert { get; set; }

        [Field(Description = "DocEntryINV", Type = BoFieldTypes.db_Numeric)]
        public int DocEntryINV { get; set; }

        [Field(Description = "DocNumINV", Type = BoFieldTypes.db_Alpha, Size = 16)]
        public string DocNumINV { get; set; }

        [Field(Description = "QtyInv", Type = BoFieldTypes.db_Numeric)]
        public int QtyInv { get; set; }

        [Field(Description = "QtyExp", Type = BoFieldTypes.db_Numeric)]
        public int QtyExp { get; set; }

        [Field(Description = "QtyNoCruz", Type = BoFieldTypes.db_Numeric)]
        public int QtyNoCruz { get; set; }

        [Field(Description = "Amount", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Price)]
        public float Amount { get; set; }
    }
}
