using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Core.Utility
{
    public static class XmlUtility
    {
        public static XmlDocument GetDocument(string pStrXmlPath)
        {
            XmlDocument lObjResult = new XmlDocument();
            string lStrXml = string.Empty;

            if (File.Exists(pStrXmlPath))
            {
                StreamReader lObjReader = new StreamReader
                (
                    new FileStream(
                        pStrXmlPath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read)
                );

                lStrXml = lObjReader.ReadToEnd();
                lObjReader.Close();
                lObjResult.LoadXml(lStrXml);
            }
            else
            {
                throw new Exception("No se encontró el documento XML.");
            }

            return lObjResult;
        }

        public static XDocument GetXDocument(string pStrXmlPath)
        {
            XDocument lObjResult = null;

            if (File.Exists(pStrXmlPath))
            {
                lObjResult = XDocument.Load(pStrXmlPath);
            }
            else
            {
                throw new Exception("No se encontró el documento XML.");
            }

            return lObjResult;
        }
    }
}
