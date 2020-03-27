using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SDK.DI.Facturacion
{
    public class Traslados
    {
        public int LineNum { get; set; }
        public decimal ImporteBase { get; set; }
        public decimal TaxRate { get; set; }
        public decimal ImporteTrasaldo { get; set; }
        public string impuestoCod { get; set; }
        public string TipoFactor { get; set; }
        public string Descripcion { get; set; }
        public string TaxCode { get; set; }
    }
}
