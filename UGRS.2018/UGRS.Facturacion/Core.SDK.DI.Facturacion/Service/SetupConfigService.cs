using Core.SDK.DI.Facturacion.DAO;
using Core.SDK.DI.Facturacion.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SDK.DI.Facturacion.Service
{
    public class SetupConfigService
    {
        private ConfigSetupDAO mObjConfigSetupDAO = null;

        public SetupConfigService()
        {
            mObjConfigSetupDAO = new ConfigSetupDAO();
        }

        public ConfigSetupDTO GetTimbradoConfigs()
        {
            return mObjConfigSetupDAO.GetTimbradoConfigs();
        }
    }
}
