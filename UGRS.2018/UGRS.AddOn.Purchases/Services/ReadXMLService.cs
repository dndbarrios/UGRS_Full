using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using UGRS.Core.SDK.DI.Purchases;
using UGRS.Core.SDK.DI.Purchases.DTO;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;
using UGRS.Core.SDK.DI.Purchases.Services;
using UGRS.Core.SDK.DI.Purchases.Services.ServicesDAO;
using servTim = UGRS.AddOn.Purchases.TimbradoSoap33Prodigia;

namespace UGRS.AddOn.Purchases.Services {
    public class ReadXMLService {
        PurchasesServiceFactory mObjPurchasesServiceFactory = new PurchasesServiceFactory();
        /// <summary>
        /// Lee los campos del xml
        /// <summary>
        public PurchaseXMLDTO ReadXML(string pStrFileName) {

            XmlTextReader reader = new XmlTextReader(pStrFileName);
            XDocument lObjDoc = XDocument.Load(pStrFileName);
            XNamespace cfdiNamespace = lObjDoc.Root.Name.Namespace;
            XElement lElementAdenda = lObjDoc.Root.Element(cfdiNamespace + "Addenda");
            if(lElementAdenda != null) {
                lObjDoc.Root.Element(cfdiNamespace + "Addenda").Remove();
            }

            List<string> lLstStrSchemas = GetXmlSchemas(lObjDoc);
            LogService.WriteInfo("Obtencion de esquemas correctamente");


            //DownloadSchema(lLstStrSchemas);


            List<string> lStrLstXML = new List<string>();//lStrValidateSchema(lObjDoc, lLstStrSchemas); //
            PurchaseXMLDTO lObjXML = new PurchaseXMLDTO();
            string lStrField = string.Empty;


            if(lStrLstXML.Count == 0) {

                try {
                    XNamespace cfdi;
                    cfdi = lObjDoc.Root.Name.Namespace;
                    XNamespace tfd = @"http://www.sat.gob.mx/TimbreFiscalDigital";
                    lStrField = "Complemento " + " " + "TimbreFiscalDigital" + " UUID ";
                    lObjXML.FolioFiscal = lObjDoc.Root.Element(cfdi + "Complemento").Element(tfd + "TimbreFiscalDigital").Attribute("UUID").Value;

                    lStrField = "Receptor " + " " + "Rfc";
                    lObjXML.RFCReceptor = lObjDoc.Root.Element(cfdi + "Receptor").Attribute("Rfc").Value;

                    string lStrRFC = mObjPurchasesServiceFactory.GetPurchaseXmlService().GetRFC();
                    if(lStrRFC != lObjXML.RFCReceptor) {
                        SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("RFC de receptor incorrecto");
                        lObjXML = null;
                    }
                    else {
                        if(string.IsNullOrEmpty(lObjXML.FolioFiscal)) {
                            lStrLstXML.Add("El folio fiscal no existe en el xml");
                        }
                        else {
                            if(ValidateUUID(lObjXML.FolioFiscal)) {
                                lStrLstXML.Add(" El folio fiscal ya existe en la base de datos");
                            }
                            else {
                                UIApplication.ShowSuccess(string.Format("XML válido"));

                                lStrField = "Comprobante" + " " + "Folio";
                                XAttribute lObjAttribute = lObjDoc.Element(cfdi + "Comprobante").Attribute("Folio");
                                if(lObjAttribute != null) {

                                    lObjXML.ReferenceFolio = lObjDoc.Element(cfdi + "Comprobante").Attribute("Folio").Value;
                                }

                                lStrField = "Emisor " + " " + "Rfc";
                                lObjXML.RFCProvider = lObjDoc.Root.Element(cfdi + "Emisor").Attribute("Rfc").Value;

                                lStrField = "Emisor" + " " + "Nombre";
                                lObjXML.BPName = lObjDoc.Root.Element(cfdi + "Emisor").Attribute("Nombre").Value;

                                lStrField = "Comprobante" + " " + "Fecha";
                                lObjXML.Date = lObjDoc.Element(cfdi + "Comprobante").Attribute("Fecha").Value;

                                lStrField = "Comprobante" + " " + "SubTotal";
                                lObjXML.SubTotal = lObjDoc.Element(cfdi + "Comprobante").Attribute("SubTotal").Value;


                                lStrField = "Comprobante" + " " + "Total";
                                lObjXML.Total = lObjDoc.Element(cfdi + "Comprobante").Attribute("Total").Value;

                                lStrField = "Conceptos";
                                IEnumerable<XElement> lLstConcepts = lObjDoc.Root.Element(cfdi + "Conceptos").Elements();
                                List<ConceptsXMLDTO> lLstConceptsXMLDTO = new List<ConceptsXMLDTO>();
                                foreach(XElement lObjElements in lLstConcepts) {
                                    ConceptsXMLDTO lObjConcepts = new ConceptsXMLDTO();

                                    lStrField = "Conceptos" + "//" + "ClaveProdServ";
                                    lObjConcepts.ClassificationCode = lObjElements.Attribute("ClaveProdServ").Value;

                                    lStrField = "Conceptos" + "//" + "Cantidad";
                                    lObjConcepts.Quantity = lObjElements.Attribute("Cantidad").Value;

                                    // lStrField = "Conceptos" + "//" + "ClaveProdServ";
                                    //lObjConcepts.CodeItmProd = lObjElements.Attribute("ClaveProdServ").Value;

                                    lStrField = "Conceptos" + "//" + "ClaveUnidad";
                                    lObjConcepts.UnitType = lObjElements.Attribute("ClaveUnidad").Value;

                                    lStrField = "Conceptos" + "//" + "Descripcion";
                                    lObjConcepts.Description = lObjElements.Attribute("Descripcion").Value;

                                    lStrField = "Conceptos" + "//" + "Importe";
                                    lObjConcepts.Amount = lObjElements.Attribute("Importe").Value;
                                    //lObjConcepts.NoIdentification = lObjElements.Attribute("NoIdentificacion").Value != null ? lObjElements.Attribute("NoIdentificacion").Value : "";

                                    lStrField = "Conceptos" + "//" + "Unidad";
                                    lObjConcepts.Unit = (string)lObjElements.Attribute("Unidad");

                                    lStrField = "Conceptos" + "//" + "ValorUnitario";
                                    lObjConcepts.UnitPrice = lObjElements.Attribute("ValorUnitario").Value;


                                    XAttribute lObjDiscount = lObjElements.Attribute("Descuento");
                                    if(lObjDiscount != null) {
                                        lStrField = "Conceptos" + "//" + "Descuento";
                                        lObjConcepts.Discount = lObjDiscount.Value;
                                        //lObjConcepts.Amount = (Convert.ToDecimal(lObjConcepts.Amount) - Convert.ToDecimal(lObjConcepts.Discount)).ToString();
                                    }



                                    if(lObjElements.HasElements) {
                                        XElement lObjHasTax = lObjElements.Element(cfdi + "Impuestos");
                                        if(lObjHasTax != null) {
                                            XElement lObjHasTraslate = lObjElements.Element(cfdi + "Impuestos").Element(cfdi + "Traslados");
                                            if(lObjHasTraslate != null) {
                                                lStrField = "Conceptos//Impuestos" + "//" + "Traslados";
                                                IEnumerable<XElement> lLstTaxesXml = lObjElements.Element(cfdi + "Impuestos").Elements(cfdi + "Traslados").Elements();

                                                List<TaxesXMLDTO> lLstTaxes = new List<TaxesXMLDTO>();
                                                foreach(XElement lObjTax in lLstTaxesXml) {
                                                    TaxesXMLDTO lObjTaxes = new TaxesXMLDTO();

                                                    lStrField = "Conceptos//Impuestos//Traslados" + "//" + "Importe";
                                                    lObjTaxes.Amount = (string)lObjTax.Attribute("Importe");
                                                    if(string.IsNullOrEmpty(lObjTaxes.Amount)) lObjTaxes.Amount = "0";

                                                    if(Convert.ToDouble(lObjTaxes.Amount) > 0) {
                                                        lStrField = "Conceptos//Impuestos//Traslados" + "//" + "TasaOCuota";
                                                        lObjTaxes.Rate = (string)lObjTax.Attribute("TasaOCuota");
                                                        if(string.IsNullOrEmpty(lObjTaxes.Rate)) lObjTaxes.Rate = "";

                                                        //lObjTaxes.Rate = lObjTax.Attribute("TasaOCuota").Value;

                                                        lStrField = "Conceptos//Impuestos//Traslados" + "//" + "Importe";
                                                        lObjTaxes.Amount = (string)lObjTax.Attribute("Importe");
                                                        if(string.IsNullOrEmpty(lObjTaxes.Amount)) lObjTaxes.Amount = "";

                                                        lStrField = "Conceptos//Impuestos//Traslados" + "//" + "Base";
                                                        lObjTaxes.Base = lObjTax.Attribute("Base").Value;

                                                        lStrField = "Conceptos//Impuestos//Traslados" + "//" + "Impuesto";
                                                        lObjTaxes.Tax = lObjTax.Attribute("Impuesto").Value;

                                                        lStrField = "Conceptos//Impuestos//Traslados" + "//" + "TipoFactor";
                                                        lObjTaxes.TypeFactor = lObjTax.Attribute("TipoFactor").Value;
                                                        lLstTaxes.Add(lObjTaxes);

                                                        decimal lDecAmount = Convert.ToDecimal(lObjConcepts.Amount) - Convert.ToDecimal(lObjConcepts.Discount);
                                                        decimal lDecAmount1 = lDecAmount + Convert.ToDecimal(0.01);
                                                        decimal lDecAmount01 = lDecAmount - Convert.ToDecimal(0.01);
                                                        decimal lDecBase = Convert.ToDecimal(lObjTaxes.Base);

                                                        //if(lObjTaxes.Tax == "002" && (lDecAmount != lDecBase && lDecAmount1 != lDecBase && lDecAmount01 != lDecBase)) {

                                                        if (lObjTaxes.Tax == "002" && !((lDecAmount1 >= lDecBase && lDecAmount <= lDecBase) || (lDecAmount01 <= lDecBase && lDecAmount01 >= lDecBase)))
                                                        {
                                                            if (Convert.ToDouble(lObjTaxes.Amount) > 0)
                                                            {
                                                                lObjConcepts.Amount = (Convert.ToDecimal(lObjTaxes.Base) + Convert.ToDecimal(lObjConcepts.Discount)).ToString();
                                                                //lObjConcepts.Amount = lObjTaxes.Base;
                                                                lObjConcepts.UnitPrice = Convert.ToDecimal((((Convert.ToDouble(lObjTaxes.Base) + Convert.ToDouble(lObjConcepts.Discount)) / Convert.ToDouble(lObjConcepts.Quantity)))).ToString();
                                                                lLstTaxes.Add(AddIeps(lDecAmount, Convert.ToDouble(lObjTaxes.Base)));
                                                            }
                                                        }
                                                    }
                                                }
                                                lObjConcepts.LstTaxes = lLstTaxes;
                                            }


                                            XElement lObjHasWht = lObjElements.Element(cfdi + "Impuestos").Element(cfdi + "Retenciones");
                                            if(lObjHasWht != null) {
                                                //Retenciones
                                                lStrField = "Conceptos//Impuestos//Retenciones";
                                                IEnumerable<XElement> lXmlWithholdingTax = lObjElements.Element(cfdi + "Impuestos").Elements(cfdi + "Retenciones").Elements();
                                                List<TaxesXMLDTO> lLstWithholdingTax = new List<TaxesXMLDTO>();
                                                foreach(XElement lObjTax in lXmlWithholdingTax) {
                                                    TaxesXMLDTO lObjTaxes = new TaxesXMLDTO();
                                                    lStrField = "Conceptos//Impuestos//Retenciones" + "//" + "TasaOCuota";
                                                    lObjTaxes.Rate = lObjTax.Attribute("TasaOCuota").Value;

                                                    lStrField = "Conceptos//Impuestos//Retenciones" + "//" + "Importe";
                                                    lObjTaxes.Amount = lObjTax.Attribute("Importe").Value;

                                                    lStrField = "Conceptos//Impuestos//Retenciones" + "//" + "Impuesto";
                                                    lObjTaxes.Tax = lObjTax.Attribute("Impuesto").Value;

                                                    lStrField = "Conceptos//Impuestos//Retenciones" + "//" + "TipoFactor";
                                                    lObjTaxes.TypeFactor = lObjTax.Attribute("TipoFactor").Value;
                                                    // lObjTaxes.Base = lObjTax.Attribute("Base").Value != null ? lObjTax.Attribute("Base").Value : "";
                                                    lLstWithholdingTax.Add(lObjTaxes);
                                                }
                                                lObjConcepts.LstWithholdingTax = lLstWithholdingTax;
                                            }
                                        }
                                    }
                                    lLstConceptsXMLDTO.Add(lObjConcepts);
                                }
                                lObjXML.ConceptLines = lLstConceptsXMLDTO;

                                List<SchemaDTO> lLstSchemaName = GetSchemasName(lObjDoc);

                                if(lLstSchemaName.Where(x => x.Key == "implocal").Count() > 0) {
                                    XNamespace XNSimplocal = lLstSchemaName.Where(x => x.Key == "implocal").FirstOrDefault().Value;
                                    lStrField = "Conceptos//ImpuestosLocales//TotaldeTraslados";
                                    XElement lXElement = lObjDoc.Root.Element(cfdi + "Complemento");

                                    IEnumerable<XElement> XImpLoc = lXElement.Descendants();//.Element(XNSimplocal + "ImpuestosLocales");
                                    IEnumerable<XAttribute> xLstAtr = XImpLoc.Attributes();
                                    XAttribute xAtr = xLstAtr.Where(x => x.Name == "TotaldeTraslados").FirstOrDefault();
                                    if(xAtr != null) {
                                        TaxesXMLDTO lObjTaxesXMLDTO = AddLocalTax(Convert.ToDecimal(xAtr.Value), 0.02);
                                        lObjXML.LstLocalTax = new List<TaxesXMLDTO>();
                                        lObjXML.LstLocalTax.Add(lObjTaxesXMLDTO);
                                    }


                                }
                            }


                            XElement lObjImpuestos = lObjDoc.Root.Element(cfdi + "Impuestos");
                            if(lObjImpuestos != null) {
                                lStrField = "Impuestos" + " " + "TotalImpuestosTrasladados";
                                XAttribute lObjTotalTraslados = lObjImpuestos.Attribute("TotalImpuestosTrasladados");
                                if(lObjTotalTraslados != null) {
                                    lObjXML.TaxesTransfers = lObjDoc.Root.Element(cfdi + "Impuestos").Attribute("TotalImpuestosTrasladados").Value;
                                }



                                lStrField = "Impuestos//Retenciones";
                                IEnumerable<XElement> lLstXMLWithholdingTaxDoc = lObjDoc.Root.Element(cfdi + "Impuestos").Elements(cfdi + "Retenciones").Elements();
                                List<TaxesXMLDTO> lLstWithholdingTaxDoc = new List<TaxesXMLDTO>();


                                foreach(XElement lObjTax in lLstXMLWithholdingTaxDoc) {
                                    TaxesXMLDTO lObjTaxes = new TaxesXMLDTO();
                                    lStrField = "Impuestos//Retenciones" + "//" + "Importe";
                                    lObjTaxes.Amount = lObjTax.Attribute("Importe").Value;

                                    lStrField = "Impuestos//Retenciones" + "//" + "Importe";
                                    lObjTaxes.Tax = lObjTax.Attribute("Impuesto").Value;
                                    // lObjTaxes.Base = lObjTax.Attribute("Base").Value != null ? lObjTax.Attribute("Base").Value : "";
                                    lLstWithholdingTaxDoc.Add(lObjTaxes);

                                    lStrField = string.Empty;
                                }

                                lObjXML.WithholdingTax = lLstWithholdingTaxDoc;
                            }
                        }
                        //}
                    }
                }
                catch(Exception ex) {
                    lObjXML = null;
                    lStrLstXML.Add(lStrField);
                    LogService.WriteError("ReadXMLService (ReadXML) " + ex.Message + ": " + lStrField);
                    LogService.WriteError(ex);
                }

            }
            if(lStrLstXML.Count > 0) {
                lObjXML = null;
                string lStrMessage = string.Format("Error en  {0}:\n{1}",
                    (lStrLstXML.Count == 1 ? "el siguiente campo" : "los siguientes campos " + lStrField),
                    string.Join("\n", lStrLstXML.Select(x => string.Format("-{0}", x)).ToArray()));

                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(lStrMessage);
                LogService.WriteError("ReadXMLService (ReadXML) " + lStrMessage);
            }
            return lObjXML;
        }


        private TaxesXMLDTO AddIeps(decimal lDblAmount, double lDblBase) {
            TaxesXMLDTO lObjTaxIeps = new TaxesXMLDTO();
            lObjTaxIeps.Tax = "003";
            lObjTaxIeps.Rate = "1";
            lObjTaxIeps.Amount = (Convert.ToDecimal(lDblAmount) - Convert.ToDecimal(lDblBase)).ToString();
            lObjTaxIeps.Base = "0.1"; //??
            lObjTaxIeps.TypeFactor = "Cuota"; // ??

            return lObjTaxIeps;
        }

        private TaxesXMLDTO AddLocalTax(decimal lDblAmount, double lDblBase) {
            TaxesXMLDTO lObjLocalTax = new TaxesXMLDTO();
            lObjLocalTax.Tax = "ISH";
            lObjLocalTax.Rate = "0.02";
            lObjLocalTax.Amount = lDblAmount.ToString();
            lObjLocalTax.TypeFactor = string.Empty;

            return lObjLocalTax;
        }


        /// <summary>
        /// Validacion de esquema comparandolo con el archivo de cfdv33
        /// <summary>
        private List<string> lStrValidateSchema(XDocument pObjDoc, List<string> pLstStringSchema) {


            List<string> lStrLstXML = new List<string>();
            try {
                // ReadXMLService lObjReadXML = new ReadXMLService();

                XmlSchemaSet lObjSchemas = new XmlSchemaSet();
                //lObjNamespace.ToString()
                //UIApplication.ShowWarning(string.Format("Validando XML"));
                //SAPbouiCOM.Framework.Application.SBO_Application.StatusBar = statu

                foreach(string lStrSchema in pLstStringSchema) {
                    LogService.WriteInfo("Esquema: " + lStrSchema);
                    string lStrFilename = lStrSchema.Substring(lStrSchema.LastIndexOf("/") + 1);
                    string lStrPath = Path.Combine(Environment.CurrentDirectory, @"Services\", lStrFilename);

                    string lStrNameSpace = lStrSchema.Substring(0, lStrSchema.LastIndexOf(" "));

                    XmlSchema lObjSchema = GetXmlSchema(lStrFilename);
                    LogService.WriteInfo("Carga " + lStrFilename);
                    LogService.WriteInfo("NameSpace" + lStrNameSpace);
                    LogService.WriteInfo("lStrPath" + lStrPath);
                    lObjSchemas.Add(null, lStrPath);
                    //lObjSchemas.Add(lObjSchema);


                    LogService.WriteInfo("Carga realizada correctamente" + lStrFilename);
                }

                //lObjSchemas.Add(@"http://www.sat.gob.mx/sitio_internet/cfd/catalogos", "C:\\Users\\amartinez\\Desktop\\XML\\catCFDI.xsd");
                //lObjSchemas.Add(@"http://www.sat.gob.mx/sitio_internet/cfd/tipoDatos/tdCFDI", "C:\\Users\\amartinez\\Desktop\\XML\\tdCFDI.xsd");

                //lObjSchemas.Add("http://www.sat.gob.mx/cfd/3", "C:\\Users\\amartinez\\Desktop\\XML\\cfdv33.xsd");
                // lObjSchemas.Add("http://www.sat.gob.mx/sitio_internet/cfd/tipoDatos/tdCFDI", "C:\\Users\\amartinez\\Desktop\\XML\\tdCFDI.xsd");


                pObjDoc.Validate(lObjSchemas, (o, e) => {
                    lStrLstXML.Add(e.Message);
                });
            }
            catch(Exception ex) {
                lStrLstXML.Add(ex.Message);
                LogService.WriteError("ReadXMLService (lStrValidateSchema) " + ex.Message);
                LogService.WriteError(ex);
            }
            return lStrLstXML;
        }

        static void ValidationCallback(object sender, ValidationEventArgs args) {
            if(args.Severity == XmlSeverityType.Warning)
                LogService.WriteError("WARNING: ");
            else if(args.Severity == XmlSeverityType.Error)
                LogService.WriteError("ERROR: ");
            LogService.WriteError("ReadXMLService (ValidationCallback) " + args.Message);
        }

        private bool ValidateUUID(string UUID) {
            return mObjPurchasesServiceFactory.GetPurchaseXmlService().ValidateUUID(UUID);
        }

        private XmlSchema GetXmlSchema(string pStrFileName) {

            XmlSchema lObjXmlSchema = new XmlSchema();
            try {
                LogService.WriteInfo("Carga de esquema:" + pStrFileName);
                string lStrFileName = pStrFileName;
                string lStrPath = Path.Combine(Environment.CurrentDirectory, @"Services\", lStrFileName);
                XmlTextReader lObjReader = new XmlTextReader(lStrPath); //"C:\\Users\\amartinez\\Desktop\\XML\\cfdv33.xsd");



            }
            catch(Exception ex) {
                LogService.WriteError("ReadXMLService (GetXmlSchema) " + ex.Message);
            }
            LogService.WriteInfo("Realizado correctamente:" + pStrFileName);
            return lObjXmlSchema;
        }

        private void DownloadSchema(List<string> lLstStrSchemas) {

            string lStrFilenameCatch = string.Empty;

            try {
                foreach(string lStrSchema in lLstStrSchemas) {

                    string lStrFilename = lStrSchema.Substring(lStrSchema.LastIndexOf("/") + 1);
                    lStrFilenameCatch = lStrFilename;
                    if(!ExistFile(lStrFilename)) {
                        using(var client = new WebClient()) {
                            LogService.WriteInfo("Descargando esquema: " + lStrSchema);
                            client.DownloadFile(lStrSchema.Substring(lStrSchema.LastIndexOf(" ") + 1), Path.Combine(Environment.CurrentDirectory, @"Services\", lStrFilename));
                            LogService.WriteInfo("Descarga correcta:" + lStrSchema);
                        }
                    }
                }
            }
            catch(Exception ex) {
                UIApplication.ShowMessageBox("Error al descargar el esquema " + lStrFilenameCatch + " favor de revisar el log o permisos de guardado " + ex.Message);
                LogService.WriteError("ReadXMLService (DownloadSchema) " + ex.Message);
                LogService.WriteError(ex);
            }
        }


        private bool ExistFile(string pStrFile) {
            try {
                pStrFile = Path.Combine(Environment.CurrentDirectory, @"Services\", pStrFile);
                if(System.IO.File.Exists(pStrFile)) {
                    return true;
                }
                else {
                    LogService.WriteInfo("Esquema no encontrado: " + pStrFile);
                    return false;
                }
            }
            catch(Exception ex) {

                UIApplication.ShowError("Error al obtener los esquemas favor de revisar el log " + ex.Message);
                LogService.WriteError("ReadXMLService (ExistFile) " + ex.Message);
                LogService.WriteError(ex);
                return false;
            }
        }


        private List<SchemaDTO> GetSchemasName(XDocument pObjxDoc) {
            XmlDocument lXmlDoc = new XmlDocument();
            List<SchemaDTO> lStrName = new List<SchemaDTO>();
            using(var xmlReader = pObjxDoc.CreateReader()) {
                lXmlDoc.Load(xmlReader);
            }

            IDictionary<string, string> IDicLocalNamespaces = null;
            XPathNavigator lObjxNav = lXmlDoc.CreateNavigator();
            while(lObjxNav.MoveToFollowing(XPathNodeType.Element)) {
                IDicLocalNamespaces = lObjxNav.GetNamespacesInScope(XmlNamespaceScope.Local);
                foreach(var localNamespace in IDicLocalNamespaces) {
                    lStrName.Add(new SchemaDTO { Key = localNamespace.Key, Value = localNamespace.Value });
                }
            }

            return lStrName;
        }


        private List<string> GetXmlSchemas(XDocument pObjxDoc) {
            List<string> lLstStrLocations = new List<string>();
            try {
                XmlDocument lXmlDoc = new XmlDocument();

                using(var xmlReader = pObjxDoc.CreateReader()) {
                    lXmlDoc.Load(xmlReader);
                }

                IDictionary<string, string> IDicLocalNamespaces = null;
                XPathNavigator lObjxNav = lXmlDoc.CreateNavigator();
                while(lObjxNav.MoveToFollowing(XPathNodeType.Element)) {
                    IDicLocalNamespaces = lObjxNav.GetNamespacesInScope(XmlNamespaceScope.Local);
                    foreach(var localNamespace in IDicLocalNamespaces) {
                        XmlDocument lObjDoc = new XmlDocument();
                        lObjDoc.LoadXml(lObjxNav.OuterXml);
                        XmlNode lObjNode = lObjDoc.DocumentElement;
                        XmlAttributeCollection lObjAttributeCollection = lObjNode.Attributes;
                        for(int i = 0; i < lObjAttributeCollection.Count; i++) {
                            if(lObjAttributeCollection[i].Name.Contains("schemaLocation")) {
                                if(!lLstStrLocations.Any(x => x == lObjAttributeCollection[i].Value)) {
                                    string lStrchema = lObjAttributeCollection[i].Value.Substring(lObjAttributeCollection[i].Value.LastIndexOf(" ") + 1);
                                    lLstStrLocations.Add(lObjAttributeCollection[i].Value);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch(Exception ex) {
                UIApplication.ShowError("Error al obtener los esquemas favor de revisar el log " + ex.Message);
                LogService.WriteError("ReadXMLService (GetXmlSchemas) " + ex.Message);
                LogService.WriteError(ex);

            }
            return lLstStrLocations;
        }

        public static bool PingHost(string pStrAddress) {
            bool lBolpingable = false;
            Ping pinger = null;
            try {
                pinger = new Ping();
                PingReply reply = pinger.Send(pStrAddress);
                lBolpingable = reply.Status == IPStatus.Success;
            }
            catch(PingException ex) {
                LogService.WriteError("No fue posible realizar el ping" + pStrAddress);
                LogService.WriteError(ex);
                // Discard PingExceptions and return false;
            }
            finally {
                if(pinger != null) {
                    pinger.Dispose();
                }
            }

            return lBolpingable;
        }

        public bool CheckVoucherStatus(PurchaseXMLDTO lObjPurchaseXML) {

            try {
                var pacConfDTO = new PurchaseXmlService().GetConfigurationPac();
                var mObjTimbradorp = new servTim.PadeTimbradoServiceClient();
                var resultXML = mObjTimbradorp.consultarEstatusComprobante(pacConfDTO.Contract, pacConfDTO.User, pacConfDTO.Pass, lObjPurchaseXML.FolioFiscal, lObjPurchaseXML.RFCProvider, lObjPurchaseXML.RFCReceptor, lObjPurchaseXML.Total, null);
                //var resultXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><servicioConsultaComprobante><consultaOk>true</consultaOk><codigo>91</codigo><codigoEstatus>S - Comprobante obtenido satisfactoriamente.</codigoEstatus><esCancelable>Cancelable sin aceptación</esCancelable><estado>Vigente</estado><estatusCfdi>4</estatusCfdi></servicioConsultaComprobante>";
                var statusXML = XDocument.Parse(resultXML)
                                         .Document.Descendants("servicioConsultaComprobante")
                                         .Select(p => new VoucherStatusDTO {
                                             QueryOK = p.Element("consultaOk").Value ?? "",
                                             Code = p.Element("codigo").Value ?? "",
                                             StatusCode = p.Element("codigoEstatus").Value ?? "",
                                             IsCancelable = p.Element("esCancelable").Value ?? "",
                                             Status = p.Element("estado").Value ?? "",
                                         }).FirstOrDefault();

                if(statusXML.QueryOK.Equals("true") && statusXML.Status.Equals("Vigente")) {
                    UIApplication.ShowMessageBox(String.Format("El estado del comprobante se consulto satisfactoriamente: \nFolio Fiscal: {0} \nCodigoEstatus: {1} \nEstado: {2} \nCancelable: {3}\n", lObjPurchaseXML.FolioFiscal, statusXML.StatusCode, statusXML.Status, statusXML.IsCancelable));
                    return true;
                }
                else {
                    UIApplication.ShowMessageBox(String.Format("Estado del comprobante consultado sin éxito: \nFolio Fiscal: {0} \nCodigoEstatus: {1} \nEstado: {2}\n", lObjPurchaseXML.FolioFiscal, statusXML.StatusCode, statusXML.Status));
                }

            }
            catch(Exception ex) {
                LogService.WriteError("(CheckVoucherStatus): " + ex.Message + "," + ex.StackTrace);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox("Problemas al consultar el estado del comprobante: " + ex.Message);
            }
            return false;
        }

    }
}
