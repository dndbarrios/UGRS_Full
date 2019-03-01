﻿using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Transports.Tables;

namespace UGRS.Core.SDK.DI.Transports.Services
{
   public class SetupService
    {
       private TableDAO<InternalFreight> mObjInternalFreightDAO;
       private TableDAO<Routes> mObjRoutesDAO;
       private TableDAO<Commissions> mObjCommissionsDAO;
       private TableDAO<CommissionLine> mObjCommissionLineDAO;
       private TableDAO<StartDay> mObjStartDay;

       public SetupService()
       {
           mObjInternalFreightDAO = new TableDAO<InternalFreight>();
           mObjRoutesDAO = new TableDAO<Routes>();
           mObjCommissionsDAO = new TableDAO<Commissions>();
           mObjCommissionLineDAO = new TableDAO<CommissionLine>();
           mObjStartDay = new TableDAO<StartDay>();
           
       }

       public void InitializeTables()
       {
           mObjInternalFreightDAO.Initialize();
           mObjRoutesDAO.Initialize();
           mObjCommissionsDAO.Initialize();
           mObjCommissionLineDAO.Initialize();
           mObjStartDay.Initialize();
       }
    }
}