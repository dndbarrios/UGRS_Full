using SAPbobsCOM;
using System;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.CreditNote.Tables
{
    [Table(Name = "UG_PE_NCDET", Description = "Nota de credito Detalle", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class CreditNoteDet : Table
    {


        [Field(Description = "NcId", Type = BoFieldTypes.db_Alpha, Size = 16)]
        public string NcId { get; set; }

        [Field(Description = "Line", Type = BoFieldTypes.db_Numeric)]
        public int Line { get; set; }

        [Field(Description = "FolioDoc", Type = BoFieldTypes.db_Alpha, Size = 16)]
        public string FolioDoc { get; set; }

        [Field(Description = "User", Size = 32)]
        public string User { get; set; }

        [Field(Description = "CardCode", Type = BoFieldTypes.db_Alpha, Size = 16)]
        public string CardCode { get; set; }

        [Field(Description = "CardName", Type = BoFieldTypes.db_Alpha, Size = 64)]
        public string CardName { get; set; }

        [Field(Description = "Cert", Type = BoFieldTypes.db_Alpha, Size = 16)]
        public string Cert { get; set; }

        [Field(Description = "SerieINV", Type = BoFieldTypes.db_Alpha, Size = 16)]
        public string SerieINV { get; set; }

        [Field(Description = "DateInv", Type = BoFieldTypes.db_Date)]
        public DateTime DateINV { get; set; }

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

        [Field(Description = "Processed", Type = BoFieldTypes.db_Alpha, Size = 1)]
        public string IsProcessed { get; set; }

        [Field(Description = "Cancelado", Type = BoFieldTypes.db_Alpha, Size = 1)]
        public string IsCanceled { get; set; }

        [Field(Description = "Creation date", Type = BoFieldTypes.db_Date)]
        public DateTime CreationDate { get; set; }

        [Field(Description = "Creation time", Type = BoFieldTypes.db_Alpha, Size = 4)]
        public string CreationTime { get; set; }

        [Field(Description = "Modification date", Type = BoFieldTypes.db_Date)]
        public DateTime ModificationDate { get; set; }

        [Field(Description = "Modification time", Type = BoFieldTypes.db_Alpha, Size = 4)]
        public string ModificationTime { get; set; }

        [Field(Description = "UserMod", Size = 32)]
        public string UserMod { get; set; }

        [Field(Description = "FolioFiscal", Size = 64)]
        public string FolioFiscal { get; set; }


        [Field(Description = "Area", Size = 32)]
        public string Area { get; set; }



    }
}
