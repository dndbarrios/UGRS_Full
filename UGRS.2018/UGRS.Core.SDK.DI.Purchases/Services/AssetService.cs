using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI.Purchases.DAO;

namespace UGRS.Core.SDK.DI.Purchases.Services
{
    public class AssetService
    {
        private AssetDAO mObjRouteListDAO;

        public AssetService()
        {
            mObjRouteListDAO = new AssetDAO();
        }

        public string GetAssetCFLQuery(string pStrArea, string pStrAFCode)
        {
            string lStrQuery = mObjRouteListDAO.GetAssetCFLQuery();

            if (!string.IsNullOrEmpty(pStrArea))
            {
                lStrQuery += string.Format(" WHERE T2.OcrCode = '{0}'", pStrArea);
            }

            if (!string.IsNullOrEmpty(pStrAFCode))
            {
                lStrQuery += string.Format(" AND T0.OcrCode LIKE '%{0}%'", pStrAFCode);
            }

            return lStrQuery;
        }
    }
}
