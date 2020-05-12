using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using Core.SDK.DI.Facturacion.DTO;
using Core.SDK.DI.Facturacion.DAO;
using System.IO;
using Core.Services;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data;

namespace Core.SDK.DI.Facturacion
{
    public class TimbradoCFDi
    {
        private FacturaCFDi factura;
        private XmlDocument newXmlDoc;
        private ConfigSetupDTO mObjConfig = null;
        OperationsSAP lObjOperacionSAP = null;
        private TimbradoFactory mObjTimbradoFactory = null;

        private string newArchivoXml = "";
        private string newArchivoPdf = "";
        private string newXmlPath = "";
        private string archivoXml = "";
        private string nomReport = "";

        private string mStrConection = string.Empty;

        public TimbradoCFDi()
        {
            mObjTimbradoFactory = new TimbradoFactory();

            if (mObjConfig == null)
            {
                mObjConfig = mObjTimbradoFactory.GetSetupConfigService().GetTimbradoConfigs();
                mStrConection = mObjConfig.ConexionString;
            }
        }

        private void ConsultarXmlPath()
        {
            newXmlPath = mObjConfig.newXmlPath;
            newXmlPath = newXmlPath.EndsWith("\\") ? newXmlPath : newXmlPath + "\\";
        }

        private void CargarXMLOriginal()
        {
            try
            {
                if (factura.Timbrado < 1)
                {
                    var xmlstr = factura.Select();
                    newXmlDoc = new XmlDocument();
                    newXmlDoc.LoadXml(xmlstr);
                    factura.XMLModificado = 1;
                    
                    lObjOperacionSAP = new OperationsSAP(newXmlDoc, factura);
                    factura.Status = 1;
                    lObjOperacionSAP.UpdateSAP("1");
                }
                else
                {
                    newXmlDoc = new XmlDocument();
                    newXmlDoc.LoadXml(factura.XmlActualizado);
                }

            }
            catch (Exception ex)
            {
                //_log.Debug(ex.Message + " - Factura DocEntry: " + factura.DocEntry + "\n");
                Core.Utility.LogUtility.Write(String.Format("{0} - Factura DocEntry: {1}{2}", ex.Message, factura.DocEntry, "\n"));
                throw ex;
            }
        }

        private void CargarObjFactura()
        {
            // Inserta la factura en un control interno para evitar duplicar timbrados
            var tfdNsM = new XmlNamespaceManager(newXmlDoc.NameTable);
            tfdNsM.AddNamespace("tfd", @"http://www.sat.gob.mx/TimbreFiscalDigital");
            var comNsM = new XmlNamespaceManager(newXmlDoc.NameTable);
            comNsM.AddNamespace("cfdi", @"http://www.sat.gob.mx/cfd/3");
            if (newXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["FormaPago"] != null)
                factura.MetodoPago = newXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["FormaPago"].Value;
            factura.NumeroDocumento = int.Parse(newXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["Folio"].Value);
            factura.FolioFiscal = newXmlDoc.SelectSingleNode("//tfd:TimbreFiscalDigital", tfdNsM).Attributes["UUID"].Value;
            factura.CertEmisor = newXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["NoCertificado"].Value;
            factura.CertSAT = newXmlDoc.SelectSingleNode("//tfd:TimbreFiscalDigital", tfdNsM).Attributes["NoCertificadoSAT"].Value;
            factura.SelloSAT = newXmlDoc.SelectSingleNode("//tfd:TimbreFiscalDigital", tfdNsM).Attributes["SelloSAT"].Value;
            factura.SelloCFD = newXmlDoc.SelectSingleNode("//tfd:TimbreFiscalDigital", tfdNsM).Attributes["SelloCFD"].Value;
            factura.FechaTimbrado = newXmlDoc.SelectSingleNode("//tfd:TimbreFiscalDigital", tfdNsM).Attributes["FechaTimbrado"].Value;
            factura.moneda = newXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["Moneda"].Value;
            factura.TimbreFiscalDigital = newXmlDoc.SelectSingleNode("//tfd:TimbreFiscalDigital", tfdNsM).OuterXml.ToString();
            //factura.CadenaOriginal = CFDIV33.CadenaOriginalImpresa(factura.TimbreFiscalDigital).Replace("\n", "");
            factura.CadenaOriginal = CFDIV33.CadenaOriginalImpresa(factura.TimbreFiscalDigital, mObjConfig.XsltCadenaOriginalImpresa).Replace("\n", "");

            factura.ImporteFactura = float.Parse(newXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["Total"].Value);
            factura.Status = 3;

            lObjOperacionSAP = new OperationsSAP(factura);
            lObjOperacionSAP.UpdateSAP("3");
            
        }

        private void GenerarXMLTimbrado()
        {
            if (factura.XMLGenerado < 1)
            {

                var xml = "";
                if (factura.XmlActualizado != newXmlDoc.InnerXml)
                {
                    xml = factura.XmlActualizado;
                }
                else
                    xml = newXmlDoc.InnerXml;

                File.AppendAllText(newArchivoXml, xml);
                factura.XMLGenerado = 1;
                factura.XmlActualizado = xml;
                factura.ArchivoXML = newArchivoXml;
                
                lObjOperacionSAP = new OperationsSAP(newXmlDoc, factura);
                lObjOperacionSAP.UpdateSAP("3");
                factura.Status = 3;
                
                //_log.Debug("XMl generado - " + factura.DocEntry);
                LogService.WriteInfo(String.Format("XMl generado - {0}", factura.DocEntry.ToString()));
            }
        }

        private void TerminarProceso()
        {
            try
            {
                if (factura.XMLOriginalBorrado < 1)
                {
                    factura.XMLOriginalBorrado = 1;

                    
                    lObjOperacionSAP = new OperationsSAP(newXmlDoc, factura);
                    lObjOperacionSAP.UpdateSAP("7");
                    factura.Status = 7;
                    //_log.Debug("Proceso terminado - " + factura.DocEntry);
                    LogService.WriteInfo(String.Format("Proceso terminado - {0}", factura.DocEntry.ToString()));
                }
            }
            catch (Exception ex)
            {
                //_log.Error( ex.Message+" - " + factura.DocEntry);
                throw ex;
            }
        }

        private void AdjuntarArchivos()
        {
            try
            {

                if (factura.ArchivosAdjuntados < 1)
                {
                    // Adjuntar archivos a Factura en SAP
                    factura.ArchivoXML = newArchivoXml;
                    factura.ArchivoPDF = newArchivoPdf;
                    factura.ArchivosAdjuntados = 1;
                    
                    lObjOperacionSAP = new OperationsSAP(newXmlDoc, factura);
                    lObjOperacionSAP.UpdateSAP("6");
                    
                    factura.Status = 6;
                    //_log.Debug("Archivos Adjuntos - " + factura.DocEntry);
                    LogService.WriteInfo(String.Format("Archivos Adjuntos - {0}", factura.DocEntry.ToString()));
                }

            }
            catch (Exception ex)
            {
                factura.Error = "Error al adjuntar los documentos al registro - " + factura.DocEntry.ToString();
                throw ex;
            }

        }

        private void GenerarPDF()
        {
            if (factura.PDFGenerado < 1)
            {
                string folderServicio = mObjConfig.RutaRepCrystal;
                folderServicio = folderServicio.EndsWith("\\") ? folderServicio : folderServicio + "\\";

                string catalogo = string.Empty;
                string usuarioHANA = string.Empty;
                string passHANA = string.Empty;
                string lStrConexion = string.Empty;
                if (DIApplication.Company.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    OdbcConnectionStringBuilder builder = new OdbcConnectionStringBuilder(factura.mObjConfig.ConexionString);
                    catalogo = factura.mObjConfig.CompanyName;
                    usuarioHANA = builder["uid"].ToString();
                    passHANA = builder["pwd"].ToString();
                    lStrConexion = factura.mObjConfig.Server;
                }
                else
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(mStrConection);
                    catalogo = builder.InitialCatalog;
                    usuarioHANA = builder.UserID;
                    passHANA = builder.Password;
                    lStrConexion = builder.ConnectionString;
                }

                string lStrNombreReporte = string.Empty;


                switch (factura.CodigoObjeto)
                {
                    case "13":
                        nomReport = "facturacliente.rpt";
                        break;
                    case "14":
                        nomReport = "notacredito.rpt";
                        break;
                    case "203":
                        nomReport = "anticipos.rpt";
                        break;
                }


                string argumentos =
                    "\"" + newArchivoPdf + "\" " +
                    factura.mObjConfig.Server + " " +
                    catalogo + " " +
                    usuarioHANA + " " +
                    passHANA + " " +
                    factura.DocEntry.ToString() + " " +
                    "N" + " " +
                    "\"" + folderServicio + nomReport + "\" " +
                    "\"" + factura.mObjConfig.ConexionString + "\"";

                try
                {
                    Process prc = new Process();
                    //ProcessStartInfo psi = new ProcessStartInfo(ConfigurationManager.AppSettings["RutaGeneradorPDF"]);
                    ProcessStartInfo psi = new ProcessStartInfo(mObjConfig.RutaGeneradorPDF);
                    psi.UseShellExecute = false;
                    psi.Arguments = argumentos;
                    prc.StartInfo = psi;
                    prc.Start();
                    prc.WaitForExit();


                    if (File.Exists(newArchivoPdf))
                    {
                        factura.PDFGenerado = 1;
                        factura.ArchivoPDF = newArchivoPdf;
                        
                        lObjOperacionSAP = new OperationsSAP(newXmlDoc, factura);
                        lObjOperacionSAP.UpdateSAP("4");
                        
                        factura.Status = 4;
                        //_log.Debug("PDFgenerado - " + factura.DocEntry);
                        LogService.WriteInfo(String.Format("PDFgenerado - {0}", factura.DocEntry.ToString()));
                    }
                    else
                    {
                        throw new Exception("No se pudo generar el PDF contacte Error en el archivo .rpt");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public string TimbrarXMLs(int pIntDocEntry)
        {
            string respuesta = "";

            try
            {
                mObjConfig = mObjTimbradoFactory.GetSetupConfigService().GetTimbradoConfigs();
                if (mObjConfig == null)
                {
                    throw new Exception("No se encontraron valores de Parametrizaciones");
                }

                mStrConection = mObjConfig.ConexionString;
                ConsultarXmlPath();
                try
                {
                    factura = new FacturaCFDi();
                    factura = GetFacturaPorTimbrar(pIntDocEntry);
                    
                    if (factura == null)
                        throw new Exception(String.Format("No se encontró la factura {0} al buscar los datos generales para timbrar.", pIntDocEntry.ToString()));

                    factura.mObjConfig = mObjConfig;
                    archivoXml = factura.RfcEmisor + "-" + factura.SerieDocumento + "-" + factura.NumeroDocumento;
                    newArchivoXml = newXmlPath + archivoXml + ".xml";
                    newArchivoPdf = newXmlPath + archivoXml + ".pdf";
                    if (File.Exists(newArchivoXml))
                    {
                        newXmlDoc = new XmlDocument();
                        newXmlDoc.Load(newArchivoXml);
                        factura.XMLGenerado = 1;
                        CargarObjFactura();
                    }
                    else
                    {
                        CargarXMLOriginal();
                        TimbrarFactura();
                        GenerarXMLTimbrado();
                    }

                    //GenerarPDF();
                    //string lStrPruebaCorreo = ConfigurationManager.AppSettings["PruebaCorreo"].ToString();
                    
                    //if (lStrPruebaCorreo == "0")
                    //    EnviarEmail();
                    AdjuntarArchivos();
                    TerminarProceso();
                    respuesta = "OK";

                }
                catch (Exception ex)
                {
                    respuesta += ex.Message + ". ";

                    if (factura != null)
                    {
                        if (factura.Error == "")
                            factura.Error = ex.Message;

                        lObjOperacionSAP = new OperationsSAP(newXmlDoc, factura);
                        lObjOperacionSAP.UpdateSAP(factura.Status.ToString());

                        //_log.Error(ex.Message + " - " + factura.DocEntry);
                        LogService.WriteError(ex.Message + " - " + factura.DocEntry);

                        factura.Error = "";
                    }
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("SAPbobsCOM.Documents"))
                {
                    throw ex;
                }
                else
                    return ex.Message + " " + ex.StackTrace;
            }
        }

        private void TimbrarFactura()
        {
            try
            {
                if (factura.Timbrado < 1)
                {
                    var xml = newXmlDoc.InnerXml;
                    var Contract = mObjConfig.Contract;
                    var User = mObjConfig.User;
                    var Passw = mObjConfig.Passw;
                    string soap = "";
                    string soapxml = "";
                    XmlDocument xmlDocRes = new XmlDocument();
                    XmlDocument xmlDocRes2 = new XmlDocument();
                    if (factura.Timbrado == 1)
                    {
                        soap = factura.TimbreFiscalDigital;
                        xmlDocRes2.LoadXml(soap);
                    }
                    else
                    {
                        //_log.Debug(string.Format("XML para la Factura con DocNum {0} y DocEntry {1}: {2}", factura.NumeroDocumento, factura.DocEntry, xml));
                        LogService.WriteInfo(string.Format("XML para la Factura con DocNum {0} y DocEntry {1}: {2}", factura.NumeroDocumento, factura.DocEntry, xml));

                        xmlDocRes = new XmlDocument();
                        TimbradoDAO timbrador = new TimbradoDAO();
                        soap = timbrador.Timbrar(xml, mObjConfig);

                        xmlDocRes.LoadXml(soap);
                        if (xmlDocRes.SelectSingleNode("//timbradoOk").InnerXml.ToString() != "true")
                        {

                            throw new Exception(xmlDocRes.SelectSingleNode("//codigo").InnerXml.ToString() + "-" + xmlDocRes.SelectSingleNode("//mensaje").InnerXml.ToString());
                        }


                        soapxml = new System.Text.ASCIIEncoding().GetString(Convert.FromBase64String(xmlDocRes.SelectSingleNode("//xmlBase64").InnerText.ToString()));
                        xmlDocRes2.LoadXml(soapxml);
                    }
                    newXmlDoc.LoadXml(xmlDocRes2.InnerXml);
                    if (factura.Timbrado == 0)
                    {
                        CargarObjFactura();
                        factura.Timbrado = 1;
                        factura.XmlActualizado = newXmlDoc.InnerXml;
                        
                        lObjOperacionSAP = new OperationsSAP(newXmlDoc, factura);
                        factura.Status = 2;
                        lObjOperacionSAP.UpdateSAP("2");
                        
                        //_log.Debug("Timbrado - " + factura.DocEntry + "\n");
                        LogService.WriteInfo(String.Format("Timbrado - {0}{1}", factura.DocEntry, "\n"));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FacturaCFDi GetFacturaPorTimbrar(int pIntDocEntry)
        {
            FacturaCFDi lObjFacturaCFDi = null;
            try
            {
                if (DIApplication.Company.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    lObjFacturaCFDi = GetFacturaHana(pIntDocEntry);
                }
                else
                {
                    lObjFacturaCFDi = GetFacturaSql(pIntDocEntry);
                }

                //lLstFacturas = lLstFacturas.OrderByDescending(x => x.DocEntry).ToList();
                if (lObjFacturaCFDi == null)
                {
                    LogService.WriteError(string.Format("No se encontró la información para la Factura con DocEntry {0}", pIntDocEntry));
                }
                else
                {
                    LogService.WriteError(string.Format("Información encontrada para la Factura con DocEntry {0} y DocNum {1}", pIntDocEntry, lObjFacturaCFDi.NumeroDocumento));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lObjFacturaCFDi;
        }

        private FacturaCFDi GetFacturaSql(int pIntDocEntry)
        {
            FacturaCFDi lObjFactura = null;
            using (SqlConnection conexion = new SqlConnection(mStrConection))
            {
                conexion.Open();
                SqlCommand comandoSql = new SqlCommand();
                SqlDataReader lector = null;
                comandoSql.Connection = conexion;
                comandoSql.CommandType = CommandType.StoredProcedure;
                comandoSql.CommandText = "SP_GetFacturaTimbrar_ByID";
                comandoSql.Parameters.Add("@IdSuc", SqlDbType.Int).Value = mObjConfig.IDSucursal;
                comandoSql.Parameters.Add("@DocEntry", SqlDbType.Int).Value = pIntDocEntry;
                lector = comandoSql.ExecuteReader();

                while (lector.Read())
                {
                    lObjFactura = new FacturaCFDi();
                    lObjFactura.SerieDocumento = int.Parse(lector["Series"].ToString());
                    lObjFactura.NumeroDocumento = int.Parse(lector["DocNum"].ToString());
                    lObjFactura.DocEntry = int.Parse(lector["DocEntry"].ToString());
                    lObjFactura.DocType = lector["DocType"].ToString();
                    lObjFactura.Serie = int.Parse(lector["Series"].ToString());
                    lObjFactura.CodigoObjeto = lector["ObjType"].ToString();
                    lObjFactura.tipoDocumento = int.Parse(lector["ObjType"].ToString());
                    lObjFactura.NombreSerie = lector["SeriesName"].ToString();
                    lObjFactura.MetodoPago = lector["MetodoPago"].ToString();
                    lObjFactura.NombreMetodoPago = lector["NombreMetodoPago"].ToString();
                    lObjFactura.CuentaCliente = lector["CuentaCliente"].ToString();
                    lObjFactura.RFC = lector["RFC"].ToString();
                    lObjFactura.FolioFiscal = lector["FolioFiscal"].ToString();
                    lObjFactura.CertEmisor = lector["CertEmisor"].ToString();
                    lObjFactura.CertSAT = lector["CertSAT"].ToString();
                    lObjFactura.SelloSAT = lector["SelloSAT"].ToString();
                    lObjFactura.SelloCFD = lector["SelloCFD"].ToString();
                    lObjFactura.FechaTimbrado = lector["FechaTimbrado"].ToString();
                    lObjFactura.TimbreFiscalDigital = lector["TimbreFiscalDigital"].ToString();
                    lObjFactura.CadenaOriginal = lector["CadenaOriginal"].ToString();
                    lObjFactura.CadenaOriginal = lector["CadenaOriginal"].ToString();
                    lObjFactura.ArchivoXML = lector["ArchivoXML"].ToString();
                    lObjFactura.ArchivoPDF = lector["ArchivoPDF"].ToString();
                    lObjFactura.ImporteFactura = float.Parse(lector["ImporteFactura"].ToString());
                    lObjFactura.XMLModificado = GetIntByBool(lector["XMlModificado"].ToString());
                    lObjFactura.Timbrado = GetIntByBool(lector["Timbrado"].ToString());
                    lObjFactura.XMLGenerado = GetIntByBool(lector["XMLGenerado"].ToString());
                    lObjFactura.PDFGenerado = GetIntByBool(lector["PDFGenerado"].ToString());
                    lObjFactura.EmailEnviado = GetIntByBool(lector["EmailEnviado"].ToString());
                    lObjFactura.ArchivosAdjuntados = GetIntByBool(lector["ArchivosAdjuntados"].ToString());
                    lObjFactura.XMLOriginalBorrado = GetIntByBool(lector["XMLOriginalBorrado"].ToString());
                    lObjFactura.XmlActualizado = lector["XmlActualizado"].ToString();
                    lObjFactura.RfcEmisor = lector["RFCEmisor"].ToString();
                    lObjFactura.NombreSN = lector["NombreSN"].ToString();
                }
                comandoSql.Dispose();
                conexion.Close();
            }

            return lObjFactura;
        }

        private FacturaCFDi GetFacturaHana(int pIntDocEntry)
        {
            FacturaCFDi lObjFactura = null;

            OdbcCommand comandoHana;

            using (OdbcConnection conexion = new OdbcConnection(mObjConfig.ConexionString))
            {
                conexion.Open();
                comandoHana = new OdbcCommand();
                comandoHana.Connection = conexion;
                comandoHana.CommandType = CommandType.Text;
                comandoHana.CommandText = "call \"" + mObjConfig.CompanyName + "\".SP_GetFacturaTimbrar_ByID(" + mObjConfig.IDSucursal + ", " + pIntDocEntry + ")";
                OdbcDataReader lector = comandoHana.ExecuteReader(CommandBehavior.Default);

                while (lector.Read())
                {
                    lObjFactura = new FacturaCFDi();
                    lObjFactura.SerieDocumento = int.Parse(lector["Series"].ToString());
                    lObjFactura.NumeroDocumento = int.Parse(lector["DocNum"].ToString());
                    lObjFactura.DocEntry = int.Parse(lector["DocEntry"].ToString());
                    lObjFactura.DocType = lector["DocType"].ToString();
                    lObjFactura.Serie = int.Parse(lector["Series"].ToString());
                    lObjFactura.CodigoObjeto = lector["ObjType"].ToString();
                    lObjFactura.tipoDocumento = int.Parse(lector["ObjType"].ToString());
                    lObjFactura.NombreSerie = lector["SeriesName"].ToString();
                    lObjFactura.MetodoPago = lector["MetodoPago"].ToString();
                    lObjFactura.NombreMetodoPago = lector["NombreMetodoPago"].ToString();
                    lObjFactura.CuentaCliente = lector["CuentaCliente"].ToString();
                    lObjFactura.RFC = lector["RFC"].ToString();
                    lObjFactura.FolioFiscal = lector["FolioFiscal"].ToString();
                    lObjFactura.CertEmisor = lector["CertEmisor"].ToString();
                    lObjFactura.CertSAT = lector["CertSAT"].ToString();
                    lObjFactura.SelloSAT = lector["SelloSAT"].ToString();
                    lObjFactura.SelloCFD = lector["SelloCFD"].ToString();
                    lObjFactura.FechaTimbrado = lector["FechaTimbrado"].ToString();
                    lObjFactura.TimbreFiscalDigital = lector["TimbreFiscalDigital"].ToString();
                    lObjFactura.CadenaOriginal = lector["CadenaOriginal"].ToString();
                    lObjFactura.CadenaOriginal = lector["CadenaOriginal"].ToString();
                    lObjFactura.ArchivoXML = lector["ArchivoXML"].ToString();
                    lObjFactura.ArchivoPDF = lector["ArchivoPDF"].ToString();
                    lObjFactura.ImporteFactura = float.Parse(lector["ImporteFactura"].ToString());
                    lObjFactura.XMLModificado = GetIntByBool(lector["XMlModificado"].ToString());
                    lObjFactura.Timbrado = GetIntByBool(lector["Timbrado"].ToString());
                    lObjFactura.XMLGenerado = GetIntByBool(lector["XMLGenerado"].ToString());
                    lObjFactura.PDFGenerado = GetIntByBool(lector["PDFGenerado"].ToString());
                    lObjFactura.EmailEnviado = GetIntByBool(lector["EmailEnviado"].ToString());
                    lObjFactura.ArchivosAdjuntados = GetIntByBool(lector["ArchivosAdjuntados"].ToString());
                    lObjFactura.XMLOriginalBorrado = GetIntByBool(lector["XMLOriginalBorrado"].ToString());
                    lObjFactura.XmlActualizado = lector["XmlActualizado"].ToString();
                    lObjFactura.RfcEmisor = lector["RFCEmisor"].ToString();
                    lObjFactura.NombreSN = lector["NombreSN"].ToString();
                }
                conexion.Dispose();
                conexion.Close();
            }

            return lObjFactura;
        }

        private int GetIntByBool(string pStrBol)
        {
            if (pStrBol == "False" || pStrBol == "0")
                return 0;
            else return 1;
        }
    }
}
