using System;
using SAPbobsCOM;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.FoodProduction.Tables
{
    [Table(Name = "UG_PL_TCKT", Description = "PL Ticket", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class Ticket : Table
    {
        [Field(Description = "Folio", Size = 64)]
        public string Folio { get; set; }

        [Field(Description = "Socio de negocio", Size = 64)]
        public string BPCode { get; set; }

        [Field(Description = "No. Documento", Type = BoFieldTypes.db_Numeric)] //DocEntry
        public int Number { get; set; }


        [Field(Description = "Tipo de Documento", Type = BoFieldTypes.db_Numeric)]
        public int DocType { get; set; }

        [Field(Description = "Tipo de pesada", Type = BoFieldTypes.db_Numeric)] //1 simple 0 double
        public int WTType { get; set; }

        [Field(Description = "Tipo de captura", Type = BoFieldTypes.db_Numeric)]
        public int CapType { get; set; }

        [Field(Description = "Placas", Size = 64)]
        public string CarTag { get; set; }

        [Field(Description = "Chofer", Size = 64)]
        public string Driver { get; set; }

        [Field(Description = "Fecha entrada", Type = BoFieldTypes.db_Date)]
        public DateTime EntryDate { get; set; }

        [Field(Description = "Fecha salida", Type = BoFieldTypes.db_Date)]
        public DateTime OutputDate { get; set; }

        [Field(Description = "Peso entrada", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity)]
        public double InputWT { get; set; }

        [Field(Description = "Peso salida", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity)]
        public double OutputWT { get; set; }

        [Field(Description = "Importe", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Price)]
        public double Amount { get; set; }

        [Field(Description = "Proyecto", Size = 64)]
        public string Project { get; set; }

        [Field(Description = "Comentarios", Size = 64)]
        public string Coments { get; set; }

        [Field(Description = "Estado", Type = BoFieldTypes.db_Numeric)]
        public int Status { get; set; }

        /*[Description("Cerrado")]
            Close = 0,
            [Description("Abierto")]
            Open = 1,
            [Description("Pendiente de facturar")]
            Pending= 2,*/

        [Field(Description = "Lineas impresas", Type = BoFieldTypes.db_Numeric)]
        public int PrintLine { get; set; }
    }
}
