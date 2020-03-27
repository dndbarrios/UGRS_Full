using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.SDK.DI;
using Core.SDK.DI.Facturacion.DTO;
using System.Configuration;
using System.Data;

namespace Core.SDK.DI.Facturacion.DAO
{
    public class ConfigSetupDAO
    {
        //private QueryManager QryMngr;
        private string mStrCnn;

        public ConfigSetupDAO()
        {
            //this.QryMngr = new QueryManager();
            this.mStrCnn = ConfigurationManager.ConnectionStrings["CFDiDB"].ConnectionString;
        }

        public ConfigSetupDTO GetTimbradoConfigs()
        {
            ConfigSetupDTO lLstResult = null;

            //this.mStrDatabaseType = (DIApplication.Company.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB) ? "HANA" : "SQL";

            if (DIApplication.Company.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
            {
                //Falta implementación para HANA
            }
            else
            {
                using (System.Data.SqlClient.SqlConnection Cnn = new System.Data.SqlClient.SqlConnection(this.mStrCnn))
                {
                    if (Cnn.State == System.Data.ConnectionState.Closed)
                        Cnn.Open();
                    using (System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand())
                    {
                        Cmd.Connection = Cnn;
                        Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        Cmd.Parameters.Add("@DBName", SqlDbType.VarChar).Value = DIApplication.Company.CompanyDB;
                        Cmd.CommandText = "SP_Timbrado_Get_Configs";

                        using (System.Data.SqlClient.SqlDataReader lector = Cmd.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                lLstResult = GetObjFromReader(lector);
                            }
                        }
                    }
                }
            }

            return lLstResult;
        }

        private ConfigSetupDTO GetObjFromReader(dynamic lector)
        {
            if (DIApplication.Company.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                lector = (System.Data.Odbc.OdbcDataReader)lector;
            else
                lector = (System.Data.SqlClient.SqlDataReader)lector;

            ConfigSetupDTO lObjConfig = new ConfigSetupDTO();

            lObjConfig.IDSucursal = lector["IDSuc"].ToString();
            lObjConfig.Contract = lector["ContratoProd"].ToString();
            lObjConfig.WSUrl = lector["ContratoProd"].ToString();
            lObjConfig.User = lector["UsuarioProd"].ToString();
            lObjConfig.Passw = lector["PasswProd"].ToString();
            lObjConfig.CertificadoSAT = lector["CertificadoSAT"].ToString();
            lObjConfig.newXmlPath = lector["XmlTimbradoPath"].ToString();
            lObjConfig.ArchivoCertificado = lector["ArchivoCertificado"].ToString();
            lObjConfig.KeySAT = lector["KeySAT"].ToString();
            lObjConfig.PasswCertificado = lector["PasswCertificado"].ToString();
            lObjConfig.SMTPServer = lector["SMTPServer"].ToString();
            lObjConfig.SMTPPort = lector["SMTPPort"].ToString();
            lObjConfig.SMTPSSL = lector["SMTPSSL"].ToString();
            lObjConfig.SMTPUser = lector["SMTPUser"].ToString();
            lObjConfig.SMTPPassword = lector["SMTPPassword"].ToString();
            lObjConfig.SenderEmail = lector["SenderEmail"].ToString();
            lObjConfig.CCEmails = lector["CCEmails"].ToString();
            lObjConfig.Body = lector["Body"].ToString();
            lObjConfig.Subject = lector["Subject"].ToString();
            lObjConfig.ConexionString = lector["SAPB1DB"].ToString();
            lObjConfig.EnviaCorreoCan = lector["EnviaCorreoCan"].ToString();
            lObjConfig.EnviaCorreo = lector["EnviaCorreo"].ToString();
            lObjConfig.XmlPathCan = lector["XmlCanceladoPath"].ToString();
            lObjConfig.Server = lector["Server"].ToString();
            lObjConfig.RutaRepCrystal = lector["RutaRepCrystal"].ToString();
            lObjConfig.NewDB = lector["NewDB"].ToString();
            lObjConfig.CompanyName = lector["CompanyName"].ToString();
            lObjConfig.RutaGeneradorPDF = lector["RutaGeneradorPDF"].ToString();

            lObjConfig.AmbientePruebaTimbrado = GetBoolFromInt(lector["AmbientePruebaTimbrado"].ToString());
            lObjConfig.PruebaCorreo = GetBoolFromInt(lector["PruebaCorreo"].ToString());
            lObjConfig.XsltCadenaOriginal = lector["XsltCadenaOriginal"].ToString();
            lObjConfig.XsltCadenaOriginalImpresa = lector["XsltCadenaOriginalImpresa"].ToString();
            lObjConfig.XsltCadenaOriginalPagos = lector["XsltCadenaOriginalPagos"].ToString();
            lObjConfig.VersionCfdi = lector["VersionCfdi"].ToString();

            //lObjConfig.TaxCodeRetenciones = !String.IsNullOrEmpty(lector["TaxCodeRetenciones"].ToString()) ?
            //    lector["TaxCodeRetenciones"].ToString().Split(';').ToList<string>() : new List<string>();
            lObjConfig.TaxCodeRetenciones = !String.IsNullOrEmpty(lector["TaxCodeRetenciones"].ToString()) ?
                lector["TaxCodeRetenciones"].ToString() : string.Empty;

            lObjConfig.UpdatesDirectos = GetBoolFromInt(lector["UpdatesDirectos"].ToString());

            return lObjConfig;
        }

        public bool GetBoolFromInt(string pStrValue)
        {
            int pBool = int.Parse(pStrValue);
            if (pBool > 0)
                return true;
            else
                return false;
        }
    }
}
