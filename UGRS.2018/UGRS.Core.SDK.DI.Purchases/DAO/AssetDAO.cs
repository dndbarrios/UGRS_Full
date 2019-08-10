using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Services;

namespace UGRS.Core.SDK.DI.Purchases.DAO
{
    public class AssetDAO
    {
        public string GetAssetCFLQuery()
        {
            try
            {
                string lStrQuery = this.GetSQL("GetAssetsCFL");

                return lStrQuery;
            }
            catch (Exception lObjException)
            {
                LogService.WriteError("AssetDAO [GetAssetCFLQuery]: " + lObjException.Message);
                LogService.WriteError(lObjException);

                throw new Exception(string.Format("Error al obtener el query de activos fijos: {0}", lObjException.Message));
            }
        }
    }
}
