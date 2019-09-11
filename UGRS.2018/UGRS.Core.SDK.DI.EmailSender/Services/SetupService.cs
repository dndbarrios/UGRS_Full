using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI.Exceptions;
using UGRS.Core.Services;
using UGRS.Core.Utility;

namespace UGRS.Core.SDK.DI.EmailSender.Services
{
    public class SetupService
    {
        public void Initialize()
        {
            InitializeUserFields();
        }

        private void InitializeUserFields()
        {
            InitializeEmailField();
        }
        
        private void InitializeEmailField()
        {
            SAPbobsCOM.UserFieldsMD lObjUserField = (SAPbobsCOM.UserFieldsMD)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
            try
            {
                LogService.WriteInfo("[InitializeUserFields] OUSR - Email");
                if (!ExistsUFD("OUSR", "Email"))
                {
                    lObjUserField.TableName = "OUSR";
                    lObjUserField.Name = "Email";
                    lObjUserField.Description = "Correo";
                    lObjUserField.Type = SAPbobsCOM.BoFieldTypes.db_Alpha;
                    lObjUserField.SubType = SAPbobsCOM.BoFldSubTypes.st_None;
                    lObjUserField.Size = 30;
                    lObjUserField.EditSize = 30;
                    //lObjUserField.LinkedTable = "ORST";
                    HandleException.Field(lObjUserField.Add());

                    LogService.WriteInfo("[InitializeUserFields] OUSR - Email: Campo creado");
                }
            }
            catch (Exception pObjException)
            {
                LogService.WriteError(string.Format("[InitializeUserFields] Error al crear el campo Email: {0}", pObjException.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjUserField);
            }
        }

        public bool ExistsUFD(string tableName, string ufdName)
        {
            SAPbobsCOM.Recordset rs = DIApplication.GetRecordset();
            try
            {
                rs.DoQuery(string.Format("SELECT \"AliasID\" FROM \"CUFD\" WHERE \"TableID\" = '{0}' AND \"AliasID\" = '{1}'", tableName, ufdName));
                if (rs.RecordCount > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                //UIApplication.ShowError(string.Format("LabelServiceException: {0}", ex.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(rs);
            }
            return false;
        }
    }
}
