using Core.SDK.DI.Facturacion.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SDK.DI.Facturacion
{
    public class TimbradoFactory
    {
        public SetupConfigService GetSetupConfigService()
        {
            return new SetupConfigService();
        }

        public TimbradoCFDi GetTimbradoCFDi()
        {
            return new TimbradoCFDi();
        }
    }
}
