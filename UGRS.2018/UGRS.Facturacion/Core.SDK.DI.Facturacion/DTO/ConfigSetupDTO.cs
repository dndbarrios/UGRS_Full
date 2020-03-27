using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SDK.DI.Facturacion.DTO
{
    public class ConfigSetupDTO
    {
        public string IDSucursal { get; set; }
        public string ConexionString { get; set; }
        public string newXmlPath { get; set; }
        public string XmlPathCan { get; set; }
        public string Contract { get; set; }
        public string User { get; set; }
        public string Passw { get; set; }
        public string WSUrl { get; set; }
        public string RutaGeneradorPDF { get; set; }
        public string ArchivoCertificado { get; set; }
        public string CertificadoSAT { get; set; }
        public string PasswCertificado { get; set; }
        public string KeySAT { get; set; }
        public string SenderEmail { get; set; }
        public string CCEmails { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SMTPServer { get; set; }
        public string SMTPPort { get; set; }
        public string SMTPSSL { get; set; }
        public string SMTPUser { get; set; }
        public string SMTPPassword { get; set; }
        public string RutaRepCrystal { get; set; }
        public string EnviaCorreoCan { get; set; }
        public string EnviaCorreo { get; set; }
        public string LicenseServer { get; set; }
        public string UserNameSAP { get; set; }
        public string PassSAP { get; set; }
        public SAPbobsCOM.BoDataServerTypes DBServerType { get; set; }
        public string Server { get; set; }
        public string ServerSQL { get; set; }
        public string UserSQL { get; set; }
        public string PassSQL { get; set; }
        public string DBName { get; set; }
        public string CompanyName { get; set; }
        public string NewDB { get; set; }
        public SAPbobsCOM.BoSuppLangs Language { get; set; }

        #region Propiedades removidas del archivo config y agregadas a la tabla de parametrizaciones
        /// <summary>
        /// 1 = Prueba, 0 = Prod.
        /// </summary>
        public bool AmbientePruebaTimbrado { get; set; }
        /// <summary>
        /// 1 = no envia correo, 0 = si envía correo.
        /// </summary>
        public bool PruebaCorreo { get; set; }

        /// <summary>
        /// Indica la ruta al archivo XSLT de CadenaOriginal.
        /// </summary>
        public string XsltCadenaOriginal { get; set; }

        public string XsltCadenaOriginalImpresa { get; set; }

        /// <summary>
        /// Indica la ruta al archivo XSLT de la CadenaOriginal para Pagos.
        /// </summary>
        public string XsltCadenaOriginalPagos { get; set; }

        /// <summary>
        /// Version del CFDi
        /// </summary>
        public string VersionCfdi { get; set; }

        /// <summary>
        /// Lista de TaxCodes que NO se agregarán a lista obtenida del SP: SP_GetTraladosByDocEntry.
        /// </summary>
        //public List<string> TaxCodeRetenciones { get; set; }
        public string TaxCodeRetenciones { get; set; }

        public bool UpdatesDirectos { get; set; }
        #endregion

    }
}
