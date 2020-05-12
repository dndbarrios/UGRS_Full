using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServTim = Core.SDK.DI.Facturacion.ServicioTimbrado33Prodigia;
using Core.SDK.DI.Facturacion.DTO;
using System.IO;
using System.Net;

namespace Core.SDK.DI.Facturacion.DAO
{
    public class TimbradoDAO
    {
        public TimbradoDAO() { }

        public string Timbrar(string pStrXML, ConfigSetupDTO pSetupConfig)
        {
            try
            {
                //Core.SDK.DI.Facturacion.Extension.Memory.ReleaseCOMObject(this.oClientTimbrado);
                ServTim.PadeTimbradoServiceClient lClientTimbrado = new ServTim.PadeTimbradoServiceClient();

                string lStrResult = string.Empty;

                //AmbientePruebaTimbrado = true (prueba)
                //AmbientePruebaTimbrado = false (prod)
                if (pSetupConfig.AmbientePruebaTimbrado)
                {
                    lStrResult = lClientTimbrado.timbradoPrueba(pSetupConfig.Contract, pSetupConfig.User, pSetupConfig.Passw, pStrXML, null);
                }
                else
                {
                    lStrResult = lClientTimbrado.timbrado(pSetupConfig.Contract, pSetupConfig.User, pSetupConfig.Passw, pStrXML, null);
                }

                return lStrResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
