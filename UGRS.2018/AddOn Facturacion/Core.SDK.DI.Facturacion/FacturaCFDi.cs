using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data;
using Core.Services;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace Core.SDK.DI.Facturacion
{
    public class FacturaCFDi
    {
        #region Atributos
        public int SerieDocumento { get; set; }
        public int NumeroDocumento { get; set; }
        public int DocEntry { get; set; }
        public int Serie { get; set; }
        public string NombreSerie { get; set; }
        public string CodigoObjeto { get; set; }
        public string MetodoPago { get; set; }
        public string NombreMetodoPago { get; set; }
        public string CuentaCliente { get; set; }
        public string RFC { get; set; }
        public string FolioFiscal { get; set; }
        public string CertEmisor { get; set; }
        public string CertSAT { get; set; }
        public string SelloSAT { get; set; }
        public string SelloCFD { get; set; }
        public string FechaTimbrado { get; set; }
        public string TimbreFiscalDigital { get; set; }
        public string CadenaOriginal { get; set; }
        public string ArchivoXML { get; set; }
        public string ArchivoPDF { get; set; }
        public float ImporteFactura { get; set; }
        public int XMLModificado { get; set; }
        public int Timbrado { get; set; }
        public int XMLGenerado { get; set; }
        public int PDFGenerado { get; set; }
        public int EmailEnviado { get; set; }
        public int ArchivosAdjuntados { get; set; }
        public int XMLOriginalBorrado { get; set; }
        public string Error { get; set; }
        public int tipoDocumento { get; set; }
        public string DocType { get; set; }
        public int Status { get; set; }
        public string Connection { get; set; }
        public string XmlActualizado { get; set; }
        public string FormaPago { get; set; }
        public string DocRelacionados { get; set; }
        public string TipoRelacion { get; set; }
        public float RetencionTotal { get; set; }
        public string RfcEmisor { get; set; }
        public string NombreSN { get; set; }
        int lIntRetenciones = 0;
        public string moneda { get; set; }
        public string cantidadletra { get; set; }
        public Core.SDK.DI.Facturacion.DTO.ConfigSetupDTO mObjConfig { get; set; }
        #endregion

        #region XMLElemet
        XmlDocument gObjXmlDoc;
        XmlDocument xmlDoc;
        XmlDeclaration xmldecl;
        XmlElement newRoot;
        XmlAttribute attribute;
        XmlElement emisor;
        XmlElement receptor;
        XmlElement domicilioFiscal;
        XmlElement domicilio;
        XmlElement conceptos;
        XmlElement concepto;
        XmlElement impuestos;
        XmlElement retenciones;
        XmlElement retencion;
        XmlElement traslados;
        XmlElement traslado;
        XmlElement regimen;
        XmlElement ImpuestosConcepto;
        XmlElement trasladosConcepto;
        XmlElement trasladoConcepto;
        XmlElement RetencionesConcepto;
        XmlElement RetencionConcepto;
        XmlElement Relacionados;
        XmlElement Relacion;
        XmlElement Complemento;
        XmlElement ComercioExterior;
        XmlElement EmisorComplemento;
        XmlElement ReceptorComplemento;
        XmlElement Mercancias;
        XmlElement Mercancia;
        XmlElement DomicilioEmisor;
        XmlElement DomicilioReceptor;
        #endregion

        public FacturaCFDi()
        {
            SerieDocumento = 0;
            NumeroDocumento = 0;
            DocEntry = 0;
            DocType = "";
            Serie = 0;
            NombreSerie = "";
            CodigoObjeto = "";
            MetodoPago = "";
            NombreMetodoPago = "";
            CuentaCliente = "";
            RFC = "";
            FolioFiscal = "";
            CertEmisor = "";
            CertSAT = "";
            SelloSAT = "";
            SelloCFD = "";
            FechaTimbrado = "";
            TimbreFiscalDigital = "";
            CadenaOriginal = "";
            ArchivoXML = "";
            ArchivoPDF = "";
            ImporteFactura = 0;
            XMLModificado = 0;
            Timbrado = 0;
            XMLGenerado = 0;
            PDFGenerado = 0;
            EmailEnviado = 0;
            ArchivosAdjuntados = 0;
            XMLOriginalBorrado = 0;
            Error = "";
            moneda = "MXP";
            cantidadletra = "";
            tipoDocumento = 0;
            XmlActualizado = "";
            DocRelacionados = "";
            TipoRelacion = "";

        }

        private void IniciarXMLElements()
        {
            gObjXmlDoc = new XmlDocument();
            xmldecl = gObjXmlDoc.CreateXmlDeclaration("1.0", null, null);
            xmldecl.Encoding = "UTF-8";
            xmldecl.Standalone = "yes";
            newRoot = gObjXmlDoc.CreateElement("cfdi", "Comprobante", "http://www.sat.gob.mx/cfd/3");
            attribute = gObjXmlDoc.CreateAttribute("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
            newRoot.Attributes.Append(attribute);
            newRoot.SetAttribute("xsi:schemaLocation", "http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd");
            emisor = gObjXmlDoc.CreateElement("cfdi", "Emisor", "http://www.sat.gob.mx/cfd/3");
            receptor = gObjXmlDoc.CreateElement("cfdi", "Receptor", "http://www.sat.gob.mx/cfd/3");
            domicilioFiscal = gObjXmlDoc.CreateElement("cfdi", "DomicilioFiscal", "http://www.sat.gob.mx/cfd/3");
            domicilio = gObjXmlDoc.CreateElement("cfdi", "Domicilio", "http://www.sat.gob.mx/cfd/3");
            conceptos = gObjXmlDoc.CreateElement("cfdi", "Conceptos", "http://www.sat.gob.mx/cfd/3");
            concepto = gObjXmlDoc.CreateElement("cfdi", "Concepto", "http://www.sat.gob.mx/cfd/3");
            impuestos = gObjXmlDoc.CreateElement("cfdi", "Impuestos", "http://www.sat.gob.mx/cfd/3");
            retenciones = gObjXmlDoc.CreateElement("cfdi", "Retenciones", "http://www.sat.gob.mx/cfd/3");
            retencion = gObjXmlDoc.CreateElement("cfdi", "Retencion", "http://www.sat.gob.mx/cfd/3");
            traslados = gObjXmlDoc.CreateElement("cfdi", "Traslados", "http://www.sat.gob.mx/cfd/3");
            traslado = gObjXmlDoc.CreateElement("cfdi", "Traslado", "http://www.sat.gob.mx/cfd/3");
            regimen = gObjXmlDoc.CreateElement("cfdi", "RegimenFiscal", "http://www.sat.gob.mx/cfd/3");
            ImpuestosConcepto = gObjXmlDoc.CreateElement("cfdi", "Impuestos", "http://www.sat.gob.mx/cfd/3");
            trasladosConcepto = gObjXmlDoc.CreateElement("cfdi", "Traslados", "http://www.sat.gob.mx/cfd/3");
            trasladoConcepto = gObjXmlDoc.CreateElement("cfdi", "Traslado", "http://www.sat.gob.mx/cfd/3");
            RetencionesConcepto = gObjXmlDoc.CreateElement("cfdi", "Retenciones", "http://www.sat.gob.mx/cfd/3");
            RetencionConcepto = gObjXmlDoc.CreateElement("cfdi", "Retencion", "http://www.sat.gob.mx/cfd/3");
            Relacionados = gObjXmlDoc.CreateElement("cfdi", "CfdiRelacionados", "http://www.sat.gob.mx/cfd/3");
            Relacion = gObjXmlDoc.CreateElement("cfdi", "CfdiRelacionado", "http://www.sat.gob.mx/cfd/3");
        }

        private List<Traslados> GetTraslados()
        {
            List<Traslados> lLstTraslados = null;
            //string lStrTaxCodeToIgnore = ConfigurationManager.AppSettings["TaxCodeRetenciones"];
            //List<string> lStrTaxCodeToIgnore = mObjConfig.TaxCodeRetenciones;

            if (DIApplication.Company.DbServerType != SAPbobsCOM.BoDataServerTypes.dst_HANADB)
            {
                SqlCommand comandoSql = null;
                SqlDataReader lector = null;

                using (SqlConnection conexion = new SqlConnection(mObjConfig.ConexionString))
                {
                    conexion.Open();
                    comandoSql = new SqlCommand();
                    comandoSql.Connection = conexion;
                    comandoSql.CommandType = CommandType.StoredProcedure;
                    comandoSql.CommandText = "SP_GetTraladosByDocEntry";
                    comandoSql.Parameters.Add(new SqlParameter("@DocEntry", SqlDbType.BigInt)).Value = DocEntry;
                    comandoSql.Parameters.Add(new SqlParameter("@DocTipo", SqlDbType.BigInt)).Value = CodigoObjeto;

                    lector = comandoSql.ExecuteReader();
                    if (lector.HasRows)
                    {
                        lLstTraslados = new List<Traslados>();
                        while (lector.Read())
                        {
                            Traslados lObjTralados = new Traslados();
                            lObjTralados.LineNum = int.Parse(lector["LineNum"].ToString());
                            lObjTralados.ImporteBase = decimal.Parse(lector["BaseSum"].ToString());
                            lObjTralados.TaxRate = decimal.Parse(lector["TaxRate"].ToString());
                            lObjTralados.ImporteTrasaldo = decimal.Parse(lector["montoiva"].ToString());
                            lObjTralados.impuestoCod = lector["ImpuestoTrasladoCod"].ToString();
                            lObjTralados.TipoFactor = lector["TipoFactor"].ToString();
                            lObjTralados.TaxCode = lector["TaxCode"].ToString();

                            if (lObjTralados.TaxCode != mObjConfig.TaxCodeRetenciones)
                            //if(!mObjConfig.TaxCodeRetenciones.Any(x => x == lObjTralados.TaxCode))
                            {
                                lLstTraslados.Add(lObjTralados);
                            }
                            else
                            {
                                LogService.WriteInfo(string.Format("El Traslado para la Linea con LineNum {0} de la Factura con DocEntry {1} sera ignorado debido a su TaxCode {2}"
                                                        , lObjTralados.LineNum, DocEntry, lObjTralados.TaxCode));
                                /*_log.Debug(string.Format("El Traslado para la Linea con LineNum {0} de la Factura con DocEntry {1} sera ignorado debido a su TaxCode {2}"
                                                        , lObjTralados.LineNum, DocEntry, lObjTralados.TaxCode));*/
                            }

                        }
                        return lLstTraslados;
                    }
                    else
                        return null;
                }
            }
            else
            {
                OdbcCommand comandoHana;
                using (OdbcConnection conexion = new OdbcConnection(mObjConfig.ConexionString))
                {
                    comandoHana = new OdbcCommand();
                    comandoHana.Connection = conexion;
                    comandoHana.CommandType = CommandType.Text;
                    comandoHana.CommandText = string.Format("call \"{0}\".SP_GetTraladosByDocEntry( {1}, {2})", mObjConfig.CompanyName, DocEntry, CodigoObjeto);
                    OdbcDataAdapter reader = new OdbcDataAdapter(comandoHana);
                    OdbcDataReader lector = comandoHana.ExecuteReader(CommandBehavior.Default);
                    if (lector.HasRows)
                    {
                        lLstTraslados = new List<Traslados>();
                        while (lector.Read())
                        {
                            Traslados lObjTralados = new Traslados();
                            lObjTralados.LineNum = int.Parse(lector["LineNum"].ToString());
                            lObjTralados.ImporteBase = decimal.Parse(lector["BaseSum"].ToString());
                            lObjTralados.TaxRate = decimal.Parse(lector["BaseSum"].ToString());
                            lObjTralados.ImporteTrasaldo = decimal.Parse(lector["montoiva"].ToString());
                            lObjTralados.impuestoCod = lector["ImpuestoTrasladoCod"].ToString();
                            lObjTralados.TipoFactor = lector["TipoFactor"].ToString();
                            lObjTralados.TaxCode = lector["TaxCode"].ToString();
                            lLstTraslados.Add(lObjTralados);

                            //TODO: Agregar validación TaxCodeRetenciones (a ignorar)
                        }
                        return lLstTraslados;
                    }
                    else
                        return null;
                }
            }

        }

        private List<Retenciones> GetRetenciones()
        {
            List<Retenciones> lLstRetencion = null;
            if (DIApplication.Company.DbServerType != SAPbobsCOM.BoDataServerTypes.dst_HANADB)
            {
                SqlCommand comandoSql = null;
                SqlDataReader lector = null;

                using (SqlConnection conexion = new SqlConnection(mObjConfig.ConexionString))
                {
                    conexion.Open();
                    comandoSql = new SqlCommand();
                    comandoSql.Connection = conexion;
                    comandoSql.CommandType = CommandType.StoredProcedure;
                    comandoSql.CommandText = "SP_GetRetencionesByDocEntry";
                    comandoSql.Parameters.Add(new SqlParameter("@DocEntry", SqlDbType.BigInt)).Value = DocEntry;
                    comandoSql.Parameters.Add(new SqlParameter("@DocTipo", SqlDbType.BigInt)).Value = CodigoObjeto;

                    lector = comandoSql.ExecuteReader();
                    if (lector.HasRows)
                    {
                        lLstRetencion = new List<Retenciones>();
                        while (lector.Read())
                        {
                            Retenciones lObjRetenciones = new Retenciones();
                            lObjRetenciones.LineNum = int.Parse(lector["LineNum"].ToString());
                            lObjRetenciones.ImporteBase = float.Parse(lector["ImporteBase"].ToString());
                            lObjRetenciones.TaxRate = float.Parse(lector["Impuesto"].ToString());
                            lObjRetenciones.ImporteRetencion = float.Parse(lector["ImporteReten"].ToString());
                            lObjRetenciones.impuestoCod = lector["Tipo"].ToString();
                            lObjRetenciones.TipoFactor = lector["Factor"].ToString();
                            lLstRetencion.Add(lObjRetenciones);
                        }
                        return lLstRetencion;
                    }
                    else
                        return null;
                }
            }
            else
            {
                OdbcCommand comandoHana;
                using (OdbcConnection conexion = new OdbcConnection(mObjConfig.ConexionString))
                {
                    comandoHana = new OdbcCommand();
                    comandoHana.Connection = conexion;
                    comandoHana.CommandType = CommandType.Text;
                    comandoHana.CommandText = string.Format("call \"{0}\".SP_GetRetencionesByDocEntry( {1}, {2})", mObjConfig.CompanyName, DocEntry, CodigoObjeto);
                    OdbcDataAdapter reader = new OdbcDataAdapter(comandoHana);
                    OdbcDataReader lector = comandoHana.ExecuteReader(CommandBehavior.Default);
                    if (lector.HasRows)
                    {
                        lLstRetencion = new List<Retenciones>();
                        while (lector.Read())
                        {
                            Retenciones lObjRetenciones = new Retenciones();
                            lObjRetenciones.LineNum = int.Parse(lector["LineNum"].ToString());
                            lObjRetenciones.ImporteBase = float.Parse(lector["ImporteBase"].ToString());
                            lObjRetenciones.TaxRate = float.Parse(lector["Impuesto"].ToString());
                            lObjRetenciones.ImporteRetencion = float.Parse(lector["ImporteReten"].ToString());
                            lObjRetenciones.impuestoCod = lector["Tipo"].ToString();
                            lObjRetenciones.TipoFactor = lector["Factor"].ToString();
                            lLstRetencion.Add(lObjRetenciones);
                        }
                        return lLstRetencion;
                    }
                    else
                        return null;
                }
            }
        }

        public string Select()
        {
            try
            {
                //string lStrSAPServer = ConfigurationManager.AppSettings["SAPDBServer"].ToString();
                //string lStrQuery = string.Empty;
                if (DIApplication.Company.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    //CreateXmlHana();
                }
                else
                {
                    CreateXMLSQL();
                }

                //_log.Debug("Informacion obtenida en xml");
                LogService.WriteInfo("Informacion obtenida en xml");
                gObjXmlDoc = AgregarDatosFaltantesXML(gObjXmlDoc);
                XmlElement root = gObjXmlDoc.DocumentElement;
                gObjXmlDoc.InsertBefore(xmldecl, root);
                xmlDoc = gObjXmlDoc;
                //_log.Debug("Informacion fiscal se adjuntó al xml");
                LogService.WriteInfo("Informacion fiscal se adjuntó al xml");
                return xmlDoc.InnerXml;
            }
            catch (Exception ex)
            {
                //_log.Error("Error: " + gObjXmlDoc.InnerXml);
                LogService.WriteError(String.Format("Error: {0}", gObjXmlDoc.InnerXml));
                throw ex;
            }
        }

        private decimal TruncateFunction(decimal number)
        {
            decimal stepper = (decimal)(Math.Pow(10.0, 2));
            Int64 temp = (Int64)(stepper * number);
            return (decimal)temp / stepper;
        }

        private void GetRootXML(dynamic pObjDataReader)
        {
            if (DIApplication.Company.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                pObjDataReader = (OdbcDataReader)pObjDataReader;
            else
                pObjDataReader = (SqlDataReader)pObjDataReader;

            newRoot.SetAttribute("Folio", pObjDataReader["folio"].ToString());
            DateTime lDtFecha;
            DateTime.TryParse(pObjDataReader["Fecha"].ToString(), out lDtFecha);

            if (DateTime.Now.Date.ToString("yyyy-MM-dd") == lDtFecha.ToString("yyyy-MM-dd"))
            {
                newRoot.SetAttribute("Fecha", lDtFecha.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("hh:mm:ss"));
            }
            else
            {
                newRoot.SetAttribute("Fecha", lDtFecha.ToString("yyyy-MM-dd") + "T" + DateTime.Now.AddMinutes(5.0).ToString("hh:mm:ss"));
            }

            //newRoot.SetAttribute("Fecha", lDtFecha.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("HH:mm:ss"));
            newRoot.SetAttribute("Moneda", pObjDataReader["Moneda"].ToString());
            newRoot.SetAttribute("TipoCambio", pObjDataReader["TipoCambio"].ToString());
            newRoot.SetAttribute("Total", TruncateFunction(decimal.Parse(pObjDataReader["Total"].ToString())).ToString());
            newRoot.SetAttribute("SubTotal", pObjDataReader["SubTotal"].ToString());
            double lFltTotal = double.Parse(pObjDataReader["total"].ToString());
            if (lFltTotal > 99999999)
            {
                newRoot.SetAttribute("Confirmacion", pObjDataReader["Confirm"].ToString());
            }
            newRoot.SetAttribute("Serie", pObjDataReader["Serie"].ToString());
            newRoot.SetAttribute("MetodoPago", pObjDataReader["MetodoPago"].ToString());
            FormaPago = pObjDataReader["MetodoPago"].ToString();

            if (decimal.Parse(pObjDataReader["Descuento"].ToString()) != 0)
                newRoot.SetAttribute("Descuento", pObjDataReader["Descuento"].ToString());


            newRoot.SetAttribute("FormaPago", pObjDataReader["FormaDePago"].ToString());
            newRoot.SetAttribute("TipoDeComprobante", pObjDataReader["TipoDeComprobante"].ToString());
            newRoot.SetAttribute("LugarExpedicion", pObjDataReader["LugarExpedicion"].ToString());
        }

        private void GetEmisor(dynamic pObjDataReader)
        {
            //emisor
            emisor.SetAttribute("Nombre", pObjDataReader["NombreEmisor"].ToString());
            emisor.SetAttribute("Rfc", pObjDataReader["RfcEmisor"].ToString());
            //Regimen
            string lStrRegimenFiscal = pObjDataReader["RegimenFiscal"].ToString();
            emisor.SetAttribute("RegimenFiscal", lStrRegimenFiscal);
        }

        private void GetReceptor(dynamic pObjDataReader)
        {
            receptor.SetAttribute("Rfc", pObjDataReader["RfcReceptor"].ToString());
            if (pObjDataReader["NombreReceptor"].ToString() != "")
                receptor.SetAttribute("Nombre", pObjDataReader["NombreReceptor"].ToString());
            receptor.SetAttribute("UsoCFDI", pObjDataReader["UsoCFDi"].ToString());
        }

        private XmlElement GetConcepto(dynamic pObjDataReader, List<Traslados> pLstTraslados, List<Retenciones> pLstRetenciones)
        {
            XmlElement concepto = gObjXmlDoc.CreateElement("cfdi", "Concepto", "http://www.sat.gob.mx/cfd/3");
            concepto.SetAttribute("ClaveProdServ", pObjDataReader["ClaveProdServ"].ToString());
            concepto.SetAttribute("Cantidad", pObjDataReader["Cantidad"].ToString());
            concepto.SetAttribute("ClaveUnidad", pObjDataReader["ClaveUnidad"].ToString());
            string lStrDescript = pObjDataReader["Descripcion"].ToString();
            if (lStrDescript.Length > 1000)
                concepto.SetAttribute("Descripcion", lStrDescript.Substring(0, 1000));
            else
                concepto.SetAttribute("Descripcion", lStrDescript);
            concepto.SetAttribute("ValorUnitario", pObjDataReader["ValorUnitario"].ToString());
            string ss = pObjDataReader["DescuentoItem"].ToString();
            if (float.Parse(pObjDataReader["DescuentoItem"].ToString()) != 0)
                concepto.SetAttribute("Descuento", (pObjDataReader["DescuentoItem"].ToString()));// TruncateFunction(decimal.Parse(pObjDataReader["DescuentoItem"].ToString())).ToString());
            concepto.SetAttribute("Importe", pObjDataReader["Importe"].ToString());

            foreach (var item in pLstTraslados)
            {
                if (int.Parse(pObjDataReader["Linea"].ToString()) == item.LineNum && item.ImporteBase > 0)
                {

                    XmlElement trasladoConcepto = gObjXmlDoc.CreateElement("cfdi", "Traslado", "http://www.sat.gob.mx/cfd/3");
                    trasladoConcepto.SetAttribute("Base", item.ImporteBase.ToString());
                    trasladoConcepto.SetAttribute("Impuesto", item.impuestoCod.ToString());
                    trasladoConcepto.SetAttribute("TipoFactor", item.TipoFactor.ToString());
                    if (item.TipoFactor != "Exento")
                    {
                        Decimal tasa = Decimal.Parse(item.TaxRate.ToString());
                        string lStrTasaOCuota = (tasa * decimal.Parse("0.01")).ToString("0.000000");
                        trasladoConcepto.SetAttribute("TasaOCuota", lStrTasaOCuota);
                        trasladoConcepto.SetAttribute("Importe", item.ImporteTrasaldo.ToString());
                    }

                    trasladosConcepto.AppendChild(trasladoConcepto);

                }
            }
            if (trasladosConcepto.ChildNodes.Count > 0)
                ImpuestosConcepto.AppendChild(trasladosConcepto);

            if (pLstRetenciones != null && pLstRetenciones.Count > 0)
            {
                if (pLstRetenciones.Any(x => x.LineNum == int.Parse(pObjDataReader["Linea"].ToString())))
                {
                    foreach (var item in pLstRetenciones)
                    {
                        if (int.Parse(pObjDataReader["Linea"].ToString()) == item.LineNum)
                        {
                            XmlElement RetencionConcepto = gObjXmlDoc.CreateElement("cfdi", "Retencion", "http://www.sat.gob.mx/cfd/3");
                            RetencionConcepto.SetAttribute("Base", item.ImporteBase.ToString());
                            RetencionConcepto.SetAttribute("Impuesto", item.impuestoCod);
                            RetencionConcepto.SetAttribute("TipoFactor", item.TipoFactor);
                            Decimal tasa = Decimal.Parse(item.TaxRate.ToString());
                            string lStrTasaOCuota = (tasa * decimal.Parse("0.01")).ToString("0.000000");
                            RetencionConcepto.SetAttribute("TasaOCuota", lStrTasaOCuota);
                            RetencionConcepto.SetAttribute("Importe", item.ImporteRetencion.ToString());
                            RetencionesConcepto.AppendChild(RetencionConcepto);
                        }
                    }
                    ImpuestosConcepto.AppendChild(RetencionesConcepto);
                }
            }
            if (ImpuestosConcepto.ChildNodes.Count > 0)
                concepto.AppendChild(ImpuestosConcepto);

            return concepto;
        }

        private bool GetCFDIRelecionados()
        {
            bool lBolResult = false;
            if (CodigoObjeto == "13" || CodigoObjeto == "14")
            {
                if (DIApplication.Company.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    OdbcCommand comandoHana;
                    using (OdbcConnection lObjConexion = new OdbcConnection(mObjConfig.ConexionString))
                    {
                        lObjConexion.Open();
                        comandoHana = new OdbcCommand();
                        comandoHana.Connection = lObjConexion;
                        comandoHana.CommandType = CommandType.Text;
                        comandoHana.CommandText = string.Format("call \"{0}\".SP_GetDocRelacionados({1}, {2})", mObjConfig.CompanyName, DocEntry, CodigoObjeto);
                        OdbcDataAdapter reader = new OdbcDataAdapter(comandoHana);
                        OdbcDataReader lector = comandoHana.ExecuteReader(CommandBehavior.Default);

                        while (lector.Read())
                        {
                            lBolResult = true;
                            GetCfdiRelacionados(lector);
                        }
                    }
                }
                else
                {
                    SqlCommand comando;
                    using (SqlConnection lObjConexion = new SqlConnection(mObjConfig.ConexionString))
                    {
                        lObjConexion.Open();
                        comando = new SqlCommand();
                        comando.Connection = lObjConexion;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandText = "SP_GetDocRelacionados";
                        comando.Parameters.Add("@DocEntry", SqlDbType.Int).Value = DocEntry;
                        comando.Parameters.Add("@DocTipo", SqlDbType.Int).Value = CodigoObjeto;
                        SqlDataReader lector = comando.ExecuteReader();
                        while (lector.Read())
                        {
                            lBolResult = true;
                            GetCfdiRelacionados(lector);
                        }
                    }
                }
            }

            return lBolResult;
        }

        private bool GetCfdiRelacionados(dynamic pObjDataReader)
        {

            Relacionados.SetAttribute("TipoRelacion", pObjDataReader["ObjType"].ToString());
            TipoRelacion = pObjDataReader["ObjType"].ToString();

            Relacion = GetCfdiRelacionado(pObjDataReader);
            Relacionados.AppendChild(Relacion);

            return true;
        }

        private XmlElement GetCfdiRelacionado(dynamic pObjDataReader)
        {
            XmlElement Rel = gObjXmlDoc.CreateElement("cfdi", "CfdiRelacionado", "http://www.sat.gob.mx/cfd/3");
            Rel.SetAttribute("UUID", pObjDataReader["FolioFiscal"].ToString());
            if (DocRelacionados == string.Empty)
                DocRelacionados = pObjDataReader["FolioFiscal"].ToString();
            else
                DocRelacionados += ", " + pObjDataReader["FolioFiscal"].ToString();

            return Rel;
        }

        private void GetImpuestosCalculados(string pStrDataReader, decimal pDmlRetenciones)
        {
            decimal lDmlImpuestosTrasladados = TruncateFunction(decimal.Parse(pStrDataReader));
            impuestos.SetAttribute("TotalImpuestosTrasladados", lDmlImpuestosTrasladados.ToString());
            if (pDmlRetenciones > 0)
                impuestos.SetAttribute("TotalImpuestosRetenidos", TruncateFunction(pDmlRetenciones).ToString());
        }

        private XmlDocument AgregarDatosFaltantesXML(XmlDocument pXmlDoc)
        {
            XmlAttribute attVersion = pXmlDoc.CreateAttribute("Version");
            attVersion.Value = mObjConfig.VersionCfdi; //ConfigurationManager.AppSettings.Get("VersionCfdi");
            pXmlDoc.DocumentElement.Attributes.Append(attVersion);
            var noCertificado = "";

            X509Certificate2 certX509 = new X509Certificate2(mObjConfig.ArchivoCertificado, mObjConfig.PasswCertificado, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
            string certificado = Convert.ToBase64String(certX509.GetRawCertData());
            noCertificado = ObtenerNumeroCertificado(certX509);
            XmlAttribute attSello = pXmlDoc.CreateAttribute("Sello");
            XmlAttribute attCertificado = pXmlDoc.CreateAttribute("Certificado");
            XmlAttribute attNoCertificado = pXmlDoc.CreateAttribute("NoCertificado");
            attCertificado.Value = certificado;
            attNoCertificado.Value = noCertificado;
            pXmlDoc.DocumentElement.Attributes.Append(attCertificado);
            pXmlDoc.DocumentElement.Attributes.Append(attNoCertificado);

            string lStrCadena = CFDIV33.generadorCadena33(pXmlDoc.InnerXml, mObjConfig.XsltCadenaOriginal);
            byte[] data = Encoding.UTF8.GetBytes(lStrCadena);
            CadenaOriginal = lStrCadena;
            var certificatePfx = File.ReadAllBytes(mObjConfig.ArchivoCertificado);


            RSACryptoServiceProvider rsa = default(RSACryptoServiceProvider);
            byte[] signatureBytes = default(byte[]);

            rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(certX509.PrivateKey.ToXmlString(true));

            signatureBytes = rsa.SignData(data, CryptoConfig.MapNameToOID("SHA256"));
            var sello = Convert.ToBase64String(signatureBytes);

            attSello.Value = sello;
            pXmlDoc.DocumentElement.Attributes.Append(attSello);

            return pXmlDoc;
        }

        public string ObtenerNumeroCertificado(X509Certificate2 cert)
        {
            string hexadecimalString = cert.SerialNumber;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= hexadecimalString.Length - 2; i += 2)
            {
                sb.Append(Convert.ToString(Convert.ToChar(Int32.Parse(hexadecimalString.Substring(i, 2), System.Globalization.NumberStyles.HexNumber))));
            }
            return sb.ToString();
        }

        private void CreateXMLSQL()
        {
            try
            {
                SqlCommand comandoSql = null;
                SqlDataReader lector = null;
                IniciarXMLElements();

                List<Traslados> lLstTraslados = GetTraslados();
                List<Retenciones> lLstRetenciones = GetRetenciones();


                using (SqlConnection conexion = new SqlConnection(mObjConfig.ConexionString))
                {

                    conexion.Open();
                    comandoSql = new SqlCommand();
                    comandoSql.Connection = conexion;
                    comandoSql.CommandType = CommandType.StoredProcedure;
                    comandoSql.CommandText = "SP_GenerarXmlParaTimbrar";
                    comandoSql.Parameters.Add(new SqlParameter("@DocEntry", SqlDbType.BigInt)).Value = DocEntry;
                    comandoSql.Parameters.Add(new SqlParameter("@DocTipo", SqlDbType.BigInt)).Value = CodigoObjeto;


                    lector = comandoSql.ExecuteReader();
                    SqlDataReader lObjLector = lector;


                    string xmlData = string.Empty;
                    bool lBolPrimeraVuelta = true;
                    string lStrRegimen = string.Empty;
                    string lStrImpuesto = string.Empty;
                    string lStrImpuestoRe = string.Empty;
                    bool lBolSuccess = false;
                    string lStrLinea = string.Empty;
                    string lStrUuid = string.Empty;
                    
                    string mStrTotalTrasladados = string.Empty;

                    while (lector.Read())
                    {
                        lBolSuccess = true;
                        if (lBolPrimeraVuelta)
                        {
                            //raiz
                            GetRootXML(lector);
                            GetEmisor(lector);
                            GetReceptor(lector);

                            concepto = GetConcepto(lector, lLstTraslados, lLstRetenciones);
                            mStrTotalTrasladados = lector["totalImpuestosTrasladados"].ToString();
                            conceptos.AppendChild(concepto);
                            lStrLinea = lector["Linea"].ToString();
                            lBolPrimeraVuelta = false;
                        }
                        else
                        {
                            concepto = gObjXmlDoc.CreateElement("cfdi", "Concepto", "http://www.sat.gob.mx/cfd/3");
                            ImpuestosConcepto = gObjXmlDoc.CreateElement("cfdi", "Impuestos", "http://www.sat.gob.mx/cfd/3");
                            trasladosConcepto = gObjXmlDoc.CreateElement("cfdi", "Traslados", "http://www.sat.gob.mx/cfd/3");
                            trasladoConcepto = gObjXmlDoc.CreateElement("cfdi", "Traslado", "http://www.sat.gob.mx/cfd/3");
                            RetencionesConcepto = gObjXmlDoc.CreateElement("cfdi", "Retenciones", "http://www.sat.gob.mx/cfd/3");
                            RetencionConcepto = gObjXmlDoc.CreateElement("cfdi", "Retencion", "http://www.sat.gob.mx/cfd/3");

                            concepto = concepto = GetConcepto(lector, lLstTraslados, lLstRetenciones);
                            conceptos.AppendChild(concepto);
                        }
                    }
                    if (lBolSuccess)
                    {

                        if (lLstRetenciones != null)
                        {
                            foreach (var retencionesLinea in lLstRetenciones)
                            {
                                XmlElement retencion = gObjXmlDoc.CreateElement("cfdi", "Retencion", "http://www.sat.gob.mx/cfd/3");
                                retencion.SetAttribute("Impuesto", retencionesLinea.impuestoCod.ToString());
                                retencion.SetAttribute("Importe", retencionesLinea.ImporteRetencion.ToString());

                                if (!retenciones.ChildNodes.Cast<XmlElement>().ToList().Exists(x => x.Attributes[0].Value == retencion.Attributes[0].Value))
                                    retenciones.AppendChild(retencion);
                                else
                                {
                                    XmlElement retencionNew = retenciones.ChildNodes.Cast<XmlElement>().ToList().Find(x => x.Attributes[0].Value == retencion.Attributes[0].Value);
                                    retencionNew.Attributes[1].Value = (decimal.Parse(retencionNew.Attributes[1].Value) + decimal.Parse(retencion.Attributes[1].Value)).ToString();
                                }

                            }

                            impuestos.AppendChild(retenciones);

                        }

                        foreach (var trasladoLinea in lLstTraslados)
                        {
                            XmlElement traslado = gObjXmlDoc.CreateElement("cfdi", "Traslado", "http://www.sat.gob.mx/cfd/3");
                            traslado.SetAttribute("Impuesto", trasladoLinea.impuestoCod.ToString());
                            traslado.SetAttribute("TipoFactor", trasladoLinea.TipoFactor);
                            Decimal tasa = Decimal.Parse(trasladoLinea.TaxRate.ToString());
                            string lStrTasaOCuota = (tasa * decimal.Parse("0.01")).ToString("0.000000");
                            traslado.SetAttribute("TasaOCuota", lStrTasaOCuota);
                            traslado.SetAttribute("Importe", trasladoLinea.ImporteTrasaldo.ToString());

                            if (!traslados.ChildNodes.Cast<XmlElement>().ToList().Exists(x => x.Attributes[0].Value == traslado.Attributes[0].Value && x.Attributes[1].Value == traslado.Attributes[1].Value && x.Attributes[2].Value == traslado.Attributes[2].Value))
                                traslados.AppendChild(traslado);
                            else
                            {
                                XmlElement trasladoNew = traslados.ChildNodes.Cast<XmlElement>().ToList().Find(x => x.Attributes[0].Value == traslado.Attributes[0].Value && x.Attributes[1].Value == traslado.Attributes[1].Value && x.Attributes[2].Value == traslado.Attributes[2].Value);
                                trasladoNew.Attributes[3].Value = (decimal.Parse(trasladoNew.Attributes[3].Value) + decimal.Parse(traslado.Attributes[3].Value)).ToString();
                            }
                        }

                        decimal lDmlTotalTraslados = 0;
                        decimal lDmlTotalRetenciones = 0;
                        foreach (XmlElement traslado in traslados)
                        {
                            traslado.Attributes[3].Value = traslado.Attributes[3].Value.ToString();
                            lDmlTotalTraslados += decimal.Parse(traslado.Attributes[3].Value.ToString());
                        }
                        foreach (XmlElement retencion in retenciones)
                        {
                            retencion.Attributes[1].Value = retencion.Attributes[1].Value.ToString();
                            lDmlTotalRetenciones += decimal.Parse(retencion.Attributes[1].Value.ToString());
                        }

                        if (traslados.HasChildNodes)
                            impuestos.AppendChild(traslados);

                        if (GetCFDIRelecionados())
                            newRoot.AppendChild(Relacionados);

                        GetImpuestosCalculados(mStrTotalTrasladados, lDmlTotalRetenciones);

                        newRoot.AppendChild(emisor);
                        newRoot.AppendChild(receptor);
                        newRoot.AppendChild(conceptos);
                        if (impuestos.HasChildNodes)
                            newRoot.AppendChild(impuestos);
                        gObjXmlDoc.AppendChild(newRoot);

                        lector.Close();
                        comandoSql.Dispose();
                        conexion.Close();
                    }
                    else
                    {
                        lector.Close();
                        comandoSql.Dispose();
                        conexion.Close();
                        throw new Exception("No es posible cargar los datos de la factura favor de comunicarse con el consultor de SAP.");
                    }
                }
            }
            catch (Exception ex)
            {
                //_log.Debug("CreateXMLSQL " + ex.Message);
                LogService.WriteError(String.Format("CreateXMLSQL|{0}", ex.Message));
            }
        }

    }
}
