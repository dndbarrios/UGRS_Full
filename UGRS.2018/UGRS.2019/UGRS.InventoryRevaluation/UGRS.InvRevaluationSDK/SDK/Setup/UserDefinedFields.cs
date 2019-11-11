using System;
using System.Collections.Generic;
using UGRS.InvRevaluationSDK.SDK.DI;
using UGRS.InvRevaluationSDK.SDK.UI;
using UGRS.Core.Utility;

namespace UGRS.InvRevaluationSDK.SDK.Setup {
    public class UserDefinedFields {

        public void CreateFields(string pStrTable, string pStrNameField, string pStrDescription, SAPbobsCOM.BoFieldTypes pFtpType, int pIntSize = 0) {

            if (!DIApplication.Connected)
                DIApplication.DIConnect((SAPbobsCOM.Company)UIApplication.GetDICompany());

            SAPbobsCOM.UserFieldsMD userField = (SAPbobsCOM.UserFieldsMD)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
            try {
                if (!ExistsUFD(pStrTable, pStrNameField)) {
                    userField.TableName = pStrTable;
                    userField.Name = pStrNameField;
                    userField.Description = pStrDescription;
                    userField.Type = pFtpType;
                    userField.SubType = SAPbobsCOM.BoFldSubTypes.st_None;

                    if (pFtpType != SAPbobsCOM.BoFieldTypes.db_Memo) {
                        userField.Size = pIntSize;
                        userField.EditSize = pIntSize;
                    }

                    if (userField.Add() != 0) {
                        int lIntError = 0;
                        string lStrError = string.Empty;
                        DIApplication.Company.GetLastError(out lIntError, out lStrError);
                        throw new Exception(lStrError);
                    }
                }
            }
            catch (Exception ex) {
                throw new Exception("Error al generar un campo definido por usuario de la tabla: " + pStrTable + ", Error: " + ex.Message);
            }
            finally {
                MemoryUtility.ReleaseComObject(userField);
            }
        }

        private bool ExistsUFD(string tableName, string ufdName) {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            try {
                lObjRecordSet = DIApplication.GetRecordset();
                lObjRecordSet.DoQuery(string.Format("SELECT \"AliasID\" FROM CUFD WHERE \"TableID\" = '{0}' AND \"AliasID\" = '{1}'", tableName, ufdName));
                if (lObjRecordSet.RecordCount > 0) {
                    return true;
                }
            }
            catch (Exception ex) {
                throw ex;
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            return false;
        }
    }
}
