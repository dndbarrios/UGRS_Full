using SAPbobsCOM;
using System;
using System.Collections.Generic;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.CreditNote.Tables
{
    [Table(Name = "UG_PE_NC", Description = "Nota de credito Encabezado", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class CreditNoteT : Table
    {
        [Field(Description = "NcId", Type = BoFieldTypes.db_Alpha, Size = 64)]
        public string NcId { get; set; }

        [Field(Description = "Date", Type = BoFieldTypes.db_Date)]
        public DateTime DateFrom { get; set; }

        [Field(Description = "Date", Type = BoFieldTypes.db_Date)]
        public DateTime DateTo { get; set; }

        [Field(Description = "Total", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Price)]
        public float Total { get; set; }

        [Field(Description = "User", Size = 32)]
        public string User { get; set; }

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

        [Field(Description = "Autorizado", Type = BoFieldTypes.db_Alpha, Size = 1)]
        public string IsAutorized { get; set; }

        [Field(Description = "Procesado", Type = BoFieldTypes.db_Alpha, Size = 1)]
        public string IsProcessed { get; set; } 
        
        [Field(Description = "Cancelado", Type = BoFieldTypes.db_Alpha, Size = 1)]
        public string IsCanceled { get; set; }

        [Field(Description = "Adjunto", Type = BoFieldTypes.db_Memo, SubType = BoFldSubTypes.st_Link)]
        public string Attach  { get; set; }

        [Field(Description = "Qty Documents", Type = BoFieldTypes.db_Numeric)]
        public int QtyDoc { get; set; }

        [Field(Description = "Qty Invoice", Type = BoFieldTypes.db_Numeric)]
        public int QtyInv { get; set; }

     


        public List<CreditNoteDoc> LstCreditNoteDoc;


    }
}



