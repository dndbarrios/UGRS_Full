﻿using System.Collections.Generic;
using UGRS.Core.SDK.DI.Transports.DAO;
using UGRS.Core.SDK.DI.Transports.DTO;

namespace UGRS.Core.SDK.DI.Transports.Services
{
    public class CommissionDriverService
    {
        private CommissionDriverDAO mObjCommissionDriver;

        public CommissionDriverService()
        {
            mObjCommissionDriver = new CommissionDriverDAO();
        }

        #region Commission Driver
        public IList<CommissionDriverDetailsDTO> GetCommissionDriversDetails(string pStrFolio, string pStrAccount)
        {
            return mObjCommissionDriver.GetCommissionDriversDetails(pStrFolio, pStrAccount);
        }

        public string GetCmsFoliosQuery()
        {
            return mObjCommissionDriver.GetCmsFoliosQuery();
        }
        #endregion
    }
}
