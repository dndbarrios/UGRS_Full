using SAPbobsCOM;
using System;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;


namespace UGRS.Core.SDK.DI.CreditNote.DOC
{
    public class CreditNoteDOC
    {
        [Table(Name = "NC_Header", Description = "Dispersion Encabezado", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
        public class DispersionT : Table
        {
            [Field(Description = "DispersionId", Type = BoFieldTypes.db_Alpha, Size = 64)]
            public string DispId { get; set; }

            [Field(Description = "Date", Type = BoFieldTypes.db_Date)]
            public DateTime Date { get; set; }

            [Field(Description = "Nombre del archivo", Type = BoFieldTypes.db_Alpha, Size = 100)]
            public string FileName { get; set; }

            [Field(Description = "Directorio de guardado", Type = BoFieldTypes.db_Memo, SubType = BoFldSubTypes.st_Link)]
            public string Path { get; set; }

            [Field(Description = "Estatus", Type = BoFieldTypes.db_Alpha, Size = 2)]
            public string Status { get; set; }

            [Field(Description = "Total", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Price)]
            public float Total { get; set; }

            [Field(Description = "Tipo", Type = BoFieldTypes.db_Alpha, Size = 20)]
            public string Type { get; set; }

            [Field(Description = "Account", Type = BoFieldTypes.db_Alpha, Size = 30)]
            public string Account { get; set; }

        }

    }
}
