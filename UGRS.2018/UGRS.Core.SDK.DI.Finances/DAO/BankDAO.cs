﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.Utility;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.Finances.DTO;

namespace UGRS.Core.SDK.DI.Finances.DAO
{
    public class BankDAO
    {
        public IList<BankDTO> GetBanks()
        {
            SAPbobsCOM.Recordset lObjResults = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            IList<BankDTO> lLstObjBanks = new List<BankDTO>();
            try
            {
                string lStrQuery = this.GetSQL("GetBanks");
                lObjResults.DoQuery(lStrQuery);
                if (lObjResults.RecordCount > 0)
                {
                    for (int i = 0; i < lObjResults.RecordCount; i++)
                    {
                        BankDTO lObjBank = new BankDTO();
                        lObjBank.BankCode = lObjResults.GetColumnValue<string>("BankCode");
                        lObjBank.BankName = lObjResults.GetColumnValue<string>("BankName");
                        lLstObjBanks.Add(lObjBank);
                        lObjResults.MoveNext();
                    }
                }
                return lLstObjBanks;
            }
            catch (Exception e)
            {
                LogUtility.WriteError(string.Format("[BankDAO - GetBanks] Error al obtener los bancos: {0}", e.Message));
                throw new Exception(string.Format("Error al obtener los bancos: {0}", e.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjResults);
            }
        }

        public IList<AccountDTO> GetBankAccounts(string pBankCode)
        {
            SAPbobsCOM.Recordset lObjResults = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            IList<AccountDTO> lLstObjBanks = new List<AccountDTO>();
            try
            {
                string lStrQuery = this.GetSQL("GetBankAccounts").InjectSingleValue("BankCode", pBankCode);
                lObjResults.DoQuery(lStrQuery);
                if (lObjResults.RecordCount > 0)
                {
                    for (int i = 0; i < lObjResults.RecordCount; i++)
                    {
                        AccountDTO lObjAccount = new AccountDTO();
                        lObjAccount.BankCode = lObjResults.GetColumnValue<string>("BankCode");
                        lObjAccount.Account = lObjResults.GetColumnValue<string>("Account");
                        lObjAccount.Branch = lObjResults.GetColumnValue<string>("Branch");
                        lObjAccount.GLAccount = lObjResults.GetColumnValue<string>("GLAccount");
                        lLstObjBanks.Add(lObjAccount);
                        lObjResults.MoveNext();
                    }
                }
                return lLstObjBanks;
            }
            catch (Exception e)
            {
                LogUtility.WriteError(string.Format("[BankDAO - GetBankAccounts] Error al obtener las cuentas para el banco {0}: {1}", pBankCode, e.Message));
                throw new Exception(string.Format("Error al obtener las cuentas para el banco {0}: {1}", pBankCode, e.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjResults);
            }
        }

        public IList<BankDTO> GetClientBanks(string pCardCode)
        {
            SAPbobsCOM.Recordset lObjResults = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            IList<BankDTO> lLstObjBanks = new List<BankDTO>();
            try
            {
                string lStrQuery = this.GetSQL("GetClientBanks").InjectSingleValue("CardCode", pCardCode);
                lObjResults.DoQuery(lStrQuery);
                if (lObjResults.RecordCount > 0)
                {
                    for (int i = 0; i < lObjResults.RecordCount; i++)
                    {
                        BankDTO lObjBank = new BankDTO();
                        lObjBank.BankCode = lObjResults.GetColumnValue<string>("BankCode");
                        lObjBank.BankName = lObjResults.GetColumnValue<string>("BankName");
                        lLstObjBanks.Add(lObjBank);
                        lObjResults.MoveNext();
                    }
                }
                return lLstObjBanks;
            }
            catch (Exception e)
            {
                LogUtility.WriteError(string.Format("[BankDAO - GetClientBanks] Error al obtener las cuentas para el cliente {0}: {1}", pCardCode, e.Message));
                throw new Exception(string.Format("Error al obtener las cuentas para el cliente {0}: {1}", pCardCode, e.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjResults);
            }
        }

        public IList<AccountDTO> GetClientBankAccounts(string pCardCode, string pBankCode)
        {
            SAPbobsCOM.Recordset lObjResults = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            IList<AccountDTO> lLstObjBanks = new List<AccountDTO>();
            try
            {
                string lStrQuery = this.GetSQL("GetClientBankAccounts");
                Dictionary<string, string> lObjParameters = new Dictionary<string, string>();
                lObjParameters.Add("CardCode", pCardCode);
                lObjParameters.Add("BankCode", pBankCode);
                lObjResults.DoQuery(lStrQuery.Inject(lObjParameters));
                if (lObjResults.RecordCount > 0)
                {
                    for (int i = 0; i < lObjResults.RecordCount; i++)
                    {
                        AccountDTO lObjAccount = new AccountDTO();
                        lObjAccount.BankCode = lObjResults.GetColumnValue<string>("BankCode");
                        lObjAccount.Account = lObjResults.GetColumnValue<string>("Account");
                        lObjAccount.Branch = lObjResults.GetColumnValue<string>("Branch");
                        lLstObjBanks.Add(lObjAccount);
                        lObjResults.MoveNext();
                    }
                }
                return lLstObjBanks;
            }
            catch (Exception e)
            {
                LogUtility.WriteError(string.Format("[BankDAO - GetClientBankAccounts] Error al obtener las cuentas para el cliente {0} con el banco {1}: {2}", pCardCode, pBankCode, e.Message));
                throw new Exception(string.Format("Error al obtener las cuentas para el cliente {0} con el banco {1}: {2}", pCardCode, pBankCode, e.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjResults);
            }
        }
    }
}
