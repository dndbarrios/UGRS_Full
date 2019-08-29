using SAPbobsCOM;
using System;
using System.Collections.Generic;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.CreditNote.Tables
{
    [Table(Name = "UG_PE_NCDOC", Description = "Nota de credito documento", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
   public  class CreditNoteDoc : Table
    {
        [Field(Description = "NcId", Type = BoFieldTypes.db_Alpha, Size = 16)]
        public string NcId { get; set; }

        [Field(Description = "Line", Type = BoFieldTypes.db_Numeric)]
        public int Line { get; set; }

        [Field(Description = "FolioDoc", Type = BoFieldTypes.db_Alpha, Size = 16)]
        public string FolioDoc { get; set; }

        [Field(Description = "User", Size = 32)]
        public string User { get; set; }

        [Field(Description = "DocEntry", Type = BoFieldTypes.db_Alpha)]
        public string DocEntry { get; set; }

        [Field(Description = "DocEntryDraft", Type = BoFieldTypes.db_Alpha)]
        public string DocEntryDraft { get; set; }

        [Field(Description = "CardCode", Type = BoFieldTypes.db_Alpha, Size = 16)]
        public string CardCode { get; set; }

        [Field(Description = "CardName", Type = BoFieldTypes.db_Alpha, Size = 64)]
        public string CardName { get; set; }

        [Field(Description = "Amount", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Price)]
        public float Amount { get; set; }

        [Field(Description = "TaxCode", Type = BoFieldTypes.db_Alpha, Size = 3)]
        public string TaxCode { get; set; }

        [Field(Description = "IVA", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Price)]
        public float IVA { get; set; }

        [Field(Description = "Processed", Type = BoFieldTypes.db_Alpha, Size = 1)]
        public string IsProcessed { get; set; }

        [Field(Description = "Document", Type = BoFieldTypes.db_Alpha, Size = 1)]
        public string IsDocument { get; set; }

        [Field(Description = "DocRel", Type = BoFieldTypes.db_Alpha, Size = 1)]
        public string IsDocRel { get; set; }
       
        [Field(Description = "QtyInv", Type = BoFieldTypes.db_Numeric)]
        public int QtyInv { get; set; }

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


       

        public List<CreditNoteDet> LstCreditNoteDet;


    }
}
