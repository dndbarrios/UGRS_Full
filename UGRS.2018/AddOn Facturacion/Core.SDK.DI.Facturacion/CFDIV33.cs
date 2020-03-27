using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Text;
using System.Configuration;
using System.Xml.XPath;

namespace Core.SDK.DI.Facturacion
{
    public class CFDIV33
    {
        public static string CadenaOriginal(string pStrXml, string pStrPathXsltCadenaOriginal)
        {
            string lStrCadena = string.Empty;
            byte[] byteArray = Encoding.UTF8.GetBytes(pStrXml);
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    XPathDocument myXPathDoc = new XPathDocument(reader);
                    XslCompiledTransform myXslTrans = new XslCompiledTransform();
                    //myXslTrans.Load(ConfigurationManager.AppSettings["XsltCadenaOriginal"].ToString());
                    myXslTrans.Load(pStrPathXsltCadenaOriginal);
                    using (StringWriter str = new StringWriter())
                    {
                        XmlTextWriter myWriter = new XmlTextWriter(str);
                        myXslTrans.Transform(myXPathDoc, null, myWriter);
                        lStrCadena = str.ToString();
                    }
                }
            }
            return lStrCadena;
        }


        public static string generadorCadena33(string strXml, string pStrPathXsltCadenaOriginal)
        {
            try
            {
                var xslt_cadenaoriginal_3_3 = new XslCompiledTransform();
                //xslt_cadenaoriginal_3_3.Load(ConfigurationManager.AppSettings["XsltCadenaOriginal"].ToString());
                xslt_cadenaoriginal_3_3.Load(pStrPathXsltCadenaOriginal);
                string resultado = null;
                StringWriter writer = new StringWriter();
                XmlReader xml = XmlReader.Create(new StringReader(strXml));
                xslt_cadenaoriginal_3_3.Transform(xml, null, writer);
                resultado = writer.ToString().Trim();
                writer.Close();

                return resultado;
            }
            catch (System.Exception ex)
            {

                throw new System.Exception("El XML proporcionado no es válido. Error real: " + ex.Message);
            }
        }

        public static string CadenaOriginalImpresa(string pStrXml, string pStrPathXsltCadenaOriginalImpresa)
        {
            string lStrCadena = string.Empty;
            byte[] byteArray = Encoding.UTF8.GetBytes(pStrXml);
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    XPathDocument myXPathDoc = new XPathDocument(reader);
                    XslCompiledTransform myXslTrans = new XslCompiledTransform();
                    //myXslTrans.Load(ConfigurationManager.AppSettings["XsltCadenaOriginalImpresa"].ToString());
                    myXslTrans.Load(pStrPathXsltCadenaOriginalImpresa);
                    using (StringWriter str = new StringWriter())
                    {
                        XmlTextWriter myWriter = new XmlTextWriter(str);
                        myXslTrans.Transform(myXPathDoc, null, myWriter);
                        lStrCadena = str.ToString();
                    }
                }
            }
            return lStrCadena;
        }


        public static string CadenaOriginalPagos(string pStrXml, string pStrPathXsltCadenaOriginalPagos)
        {
            string lStrCadena = string.Empty;
            byte[] byteArray = Encoding.UTF8.GetBytes(pStrXml);
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    XPathDocument myXPathDoc = new XPathDocument(reader);
                    XslCompiledTransform myXslTrans = new XslCompiledTransform();
                    //myXslTrans.Load(ConfigurationManager.AppSettings["XsltCadenaOriginalPagos"].ToString());
                    myXslTrans.Load(pStrPathXsltCadenaOriginalPagos);
                    using (StringWriter str = new StringWriter())
                    {
                        XmlTextWriter myWriter = new XmlTextWriter(str);
                        myXslTrans.Transform(myXPathDoc, null, myWriter);
                        lStrCadena = str.ToString();
                    }
                }
            }
            return lStrCadena;
        }
    }
}
