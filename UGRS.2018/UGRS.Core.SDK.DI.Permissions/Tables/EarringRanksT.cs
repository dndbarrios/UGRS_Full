﻿using SAPbobsCOM;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.AddOn.Permissions.Tables
{
     [Table(Name = "UG_PE_ERNK", Description = "PE Rangos de Aretes", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class EarringRanksT : Table
    {
         public virtual int Row { get; set; }

         [Field(Description = "Documento", Size = 50)]
         public string BaseEntry { get; set; }

         [Field(Description = "Desde", Size = 50)]
         public string EarringFrom { get; set; }

         [Field(Description = "Hasta", Size = 50)]
         public string EarringTo { get; set; }

         [Field(Description = "Prefijo", Size = 50)]
         public string Prefix { get; set; }

         [Field(Description = "Cancelado", Size = 10)]
         public string Cancelled { get; set; }
    }
}
