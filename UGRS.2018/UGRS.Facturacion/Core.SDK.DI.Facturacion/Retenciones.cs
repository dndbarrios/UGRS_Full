using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SDK.DI.Facturacion
{
    public class Retenciones
    {
        public int LineNum { get; set; }
        public float ImporteBase { get; set; }
        public float TaxRate { get; set; }
        public float ImporteRetencion { get; set; }
        public string impuestoCod { get; set; }
        public string TipoFactor { get; set; }
    }
}
