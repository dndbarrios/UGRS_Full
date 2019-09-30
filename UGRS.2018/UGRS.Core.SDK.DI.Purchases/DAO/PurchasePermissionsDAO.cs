using UGRS.Core.Services;
using System;
using System.Collections.Generic;
using UGRS.Core.SDK.DI.Purchases.Enums;
using UGRS.Core.SDK.UI;
using UGRS.Core.Utility;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.Extension;

namespace UGRS.Core.SDK.DI.Purchases.DAO
{
    public class PurchasePermissionsDAO
    {
        #region Permissions

        public PermissionsEnum.Permission GetPermissionType(string pStrCostCenter, string pStrType, string pStrArea)
        {
            PermissionsEnum.Permission lObjPermissionEnum = new PermissionsEnum.Permission();
            lObjPermissionEnum = PermissionsEnum.Permission.None;
            string lStrUserCode = UIApplication.GetCompany().UserName;

            bool lBolAutoOP = false;
            if ((pStrType == "U_GLO_ExpeCheck" && lStrUserCode == "MAQUINARIA" && pStrCostCenter == "MQ_MAQUI" && pStrArea == "MQ_MAQUI"))
            {
                lBolAutoOP = true;
            }
            else
            {
                if (Permission_Purchases(lStrUserCode, "Permission_Authorizes_Operations", "", pStrType))
                {
                    if (pStrType == "U_GLO_ExpeCheck")
                    {
                        if (pStrArea != "MQ_MAQUI")
                        {
                            lBolAutoOP = true;
                        }
                    }
                    else if (pStrType == "U_GLO_Refund")
                    {
                        if (lStrUserCode != "MAQUINARIA")
                        {
                            lBolAutoOP = true;
                        }
                    }
                }
            }

            //if (Permission_Purchases(lStrUserCode, "Permission_Authorizes_Operations", "", pStrType) && (pStrType == "U_GLO_ExpeCheck" && pStrArea != "MQ_MAQUI") && (pStrType == "U_GLO_Refund" && lStrUserCode != "MAQUINARIA")
            //    || (pStrType == "U_GLO_ExpeCheck" && lStrUserCode == "MAQUINARIA" && pStrCostCenter == "MQ_MAQUI" && pStrArea == "MQ_MAQUI"))
           if(lBolAutoOP)
            {

                lObjPermissionEnum = PermissionsEnum.Permission.AuthorizeOperations;
            }
            else if (Permission_Purchases(lStrUserCode, "Permission_Purchases", "", pStrType))
            {
                lObjPermissionEnum = PermissionsEnum.Permission.Purchase;
            }
            else if (Permission_Purchases(lStrUserCode, "Permission_Authorize_Purchases", pStrCostCenter, pStrType))
            {
                //lObjPermissionEnum = PermissionsEnum.Permission.AuthorizePurchase;
            }
            return lObjPermissionEnum;
        }

        private bool Permission_Purchases(string pStrUserCode, string pStrSqlQuery, string pStrCostCenter, string pStrType)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            try
            {
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("UserCode", pStrUserCode);
                if (!string.IsNullOrEmpty(pStrCostCenter))
                {
                    lLstStrParameters.Add("CostCenter", pStrCostCenter);
                }
                string lStrQuery = this.GetSQL(pStrSqlQuery).Inject(lLstStrParameters);
                lStrQuery = lStrQuery + " and " + pStrType + "= 'Y' ";
                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);
                if (lObjRecordset.RecordCount > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
                LogService.WriteError("PurchasesDAO (Permission_Purchases): " + ex.Message);
                LogService.WriteError(ex);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return false;
        }

        #endregion
    }
}
