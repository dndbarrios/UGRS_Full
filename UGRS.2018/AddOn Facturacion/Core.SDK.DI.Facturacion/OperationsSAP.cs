using Core.SDK.DI.Facturacion.Extension;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.SDK.DI.Facturacion.DTO;
using Core.Services;

namespace Core.SDK.DI.Facturacion
{
    class OperationsSAP
    {
        #region Attributes
        SAPbobsCOM.Documents oInvoice;
        SAPbobsCOM.Payments mObjPagoDoc;
        //private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        string currentCompany;
        private System.Xml.XmlDocument DocXml;
        private FacturaCFDi factura;
        //string mStrDabeDatos = ConfigurationManager.AppSettings["SAPCompanyName"].ToString();
        private System.Xml.XmlDocument doc;
        //private BO.PagosCFDi mObjPago;
        private List<ConfigSetupDTO> mLstConfig;

        /*private string StringConnection
        {
            get
            {
                if (IntPtr.Size == 8)
                    return ConfigurationManager.ConnectionStrings["SAPHANADB"].ConnectionString;
                else
                    return ConfigurationManager.ConnectionStrings["SAPHANADB32"].ConnectionString;
            }
        }*/

        #endregion

        #region Contructor

        public OperationsSAP(System.Xml.XmlDocument pXmlNewXmlDoc, FacturaCFDi pObjfactura)
        {
            this.DocXml = pXmlNewXmlDoc;
            this.factura = pObjfactura;
        }

        public OperationsSAP(FacturaCFDi factura)
        {

            this.factura = factura;
        }

        //public OperationsSAP(PagosCFDi pago)
        //{

        //    this.mObjPago = pago;
        //}

        public OperationsSAP()
        {

        }

        //public OperationsSAP(System.Xml.XmlDocument doc, BO.PagosCFDi mObjPago)
        //{
        //    // TODO: Complete member initialization
        //    this.doc = doc;
        //    this.mObjPago = mObjPago;
        //}

        #endregion

        #region Methods

        #region Facturas, Facturas de anticipo, Notas de credito
        public void UpdateSAP(string pStrStatus)
        {
            try
            {
                //string lStrDbServer = ConfigurationManager.AppSettings["SAPDBServer"].ToString();
                string lStrQueryUpdate = string.Empty;

                if (DIApplication.Company.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    SaveOrUpdateHana(pStrStatus);
                }
                else
                {
                    //if (ConfigurationManager.AppSettings["UpdatesDirectos"].ToString() == "1")
                    if(factura.mObjConfig.UpdatesDirectos)
                        SaveOrUpdateSQL(pStrStatus);
                    else
                        SaveOrUpdateDI(pStrStatus);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message + " - DocEntry: " + factura.DocEntry);
                throw ex;
            }
        }

        public void SaveOrUpdateHana(string pStrStatus)
        {
            string lStrQueryUpdate = string.Empty;
            OdbcCommand comandoHana;
            if (factura.Error != "")
            {
                switch (factura.CodigoObjeto)
                {
                    case "13":
                        lStrQueryUpdate = "update \"" + factura.mObjConfig.CompanyName + "\".OINV " +
                        "set " +
                        "\"U_ErrorCFDi\" = '" + factura.Error.Replace("'", "") + "' where \"DocEntry\" = " + factura.DocEntry;
                        break;
                    case "14":
                        lStrQueryUpdate = "update \"" + factura.mObjConfig.CompanyName + "\".ORIN " +
                        "set " +
                        "\"U_ErrorCFDi\" = '" + factura.Error.Replace("'", "") + "' where \"DocEntry\" = " + factura.DocEntry;
                        break;
                    case "203":
                        lStrQueryUpdate = "update \"" + factura.mObjConfig.CompanyName + "\".ODPI " +
                        "set " +
                        "\"U_ErrorCFDi\" = '" + factura.Error.Replace("'", "") + "' where \"DocEntry\" = " + factura.DocEntry;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (factura.CodigoObjeto)
                {
                    case "13":
                        lStrQueryUpdate = "update \"" + factura.mObjConfig.CompanyName + "\".OINV " +
                        "set " +
                        "\"U_ImporteFactura\" = " + factura.ImporteFactura + "," +
                        "\"U_SelloCFD\" = '" + factura.SelloCFD + "'," +
                        "\"U_SelloSAT\" = '" + factura.SelloSAT + "'," +
                        "\"U_TimbradoFiscalDigital\" = '" + factura.TimbreFiscalDigital + "'," +
                        "\"U_CertSAT\" = '" + factura.CertSAT + "'," +
                        "\"U_FolioFiscal\" = '" + factura.FolioFiscal + "'," +
                        "\"U_FechaTimbrado\" = '" + factura.FechaTimbrado + "'," +
                        "\"U_CertEmisor\" = '" + factura.CertEmisor + "'," +
                        "\"U_NombreSerie\" = '" + factura.NombreSerie + "'," +
                        "\"U_CuentaCliente\" = '" + factura.CuentaCliente + "'," +
                        "\"U_NombreMetodoPago\" = '" + factura.NombreMetodoPago + "'," +
                        "\"U_CadenaOriginal\" = '" + factura.CadenaOriginal + "'," +
                        "\"U_ErrorCFDi\" = '" + factura.Error + "', " +
                        "\"U_ArchivoXML\" = '" + factura.ArchivoXML + "', " +
                        "\"U_ArchivoPDF\" = '" + factura.ArchivoPDF + "', " +
                        "\"U_CveMetodoPago\" = '" + factura.FormaPago + "', " +
                        "\"U_CantidadLetra\" = '" + factura.cantidadletra + "' where \"DocEntry\" = " + factura.DocEntry;
                        break;
                    case "14":
                        lStrQueryUpdate = "update \"" + factura.mObjConfig.CompanyName + "\".ORIN " +
                        "set " +
                        "\"U_ImporteFactura\" = " + factura.ImporteFactura + "," +
                        "\"U_SelloCFD\" = '" + factura.SelloCFD + "'," +
                        "\"U_SelloSAT\" = '" + factura.SelloSAT + "'," +
                        "\"U_TimbradoFiscalDigital\" = '" + factura.TimbreFiscalDigital + "'," +
                        "\"U_CertSAT\" = '" + factura.CertSAT + "'," +
                        "\"U_FolioFiscal\" = '" + factura.FolioFiscal + "'," +
                        "\"U_FechaTimbrado\" = '" + factura.FechaTimbrado + "'," +
                        "\"U_CertEmisor\" = '" + factura.CertEmisor + "'," +
                        "\"U_NombreSerie\" = '" + factura.NombreSerie + "'," +
                        "\"U_CuentaCliente\" = '" + factura.CuentaCliente + "'," +
                        "\"U_NombreMetodoPago\" = '" + factura.NombreMetodoPago + "'," +
                        "\"U_CadenaOriginal\" = '" + factura.CadenaOriginal + "'," +
                        "\"U_ErrorCFDi\" = '" + factura.Error + "', " +
                        "\"U_ArchivoXML\" = '" + factura.ArchivoXML + "', " +
                        "\"U_ArchivoPDF\" = '" + factura.ArchivoPDF + "', " +
                        "\"U_CveMetodoPago\" = '" + factura.FormaPago + "', " +
                        "\"U_CantidadLetra\" = '" + factura.cantidadletra + "' where \"DocEntry\" = " + factura.DocEntry;
                        break;
                    case "203":
                        lStrQueryUpdate = "update \"" + factura.mObjConfig.CompanyName + "\".ODPI " +
                        "set " +
                        "\"U_ImporteFactura\" = " + factura.ImporteFactura + "," +
                        "\"U_SelloCFD\" = '" + factura.SelloCFD + "'," +
                        "\"U_SelloSAT\" = '" + factura.SelloSAT + "'," +
                        "\"U_TimbradoFiscalDigital\" = '" + factura.TimbreFiscalDigital + "'," +
                        "\"U_CertSAT\" = '" + factura.CertSAT + "'," +
                        "\"U_FolioFiscal\" = '" + factura.FolioFiscal + "'," +
                        "\"U_FechaTimbrado\" = '" + factura.FechaTimbrado + "'," +
                        "\"U_CertEmisor\" = '" + factura.CertEmisor + "'," +
                        "\"U_NombreSerie\" = '" + factura.NombreSerie + "'," +
                        "\"U_CuentaCliente\" = '" + factura.CuentaCliente + "'," +
                        "\"U_NombreMetodoPago\" = '" + factura.NombreMetodoPago + "'," +
                        "\"U_CadenaOriginal\" = '" + factura.CadenaOriginal + "'," +
                        "\"U_ErrorCFDi\" = '" + factura.Error + "', " +
                        "\"U_ArchivoXML\" = '" + factura.ArchivoXML + "', " +
                        "\"U_ArchivoPDF\" = '" + factura.ArchivoPDF + "', " +
                        "\"U_CveMetodoPago\" = '" + factura.FormaPago + "', " +
                        "\"U_CantidadLetra\" = '" + factura.cantidadletra + "' where \"DocEntry\" = " + factura.DocEntry;
                        break;
                    default:
                        break;
                }
            }

            try
            {
                using (OdbcConnection conexion = new OdbcConnection(factura.mObjConfig.ConexionString))
                {
                    try
                    {
                        conexion.Open();
                        comandoHana = new OdbcCommand();
                        comandoHana.Connection = conexion;
                        comandoHana.CommandType = CommandType.Text;
                        comandoHana.CommandText = lStrQueryUpdate;
                        int lInt = comandoHana.ExecuteNonQuery();
                        conexion.Dispose();
                        conexion.Close();
                        LogService.WriteInfo("Operacion realizada correctamente status: " + pStrStatus + "DocEntry : " + factura.DocEntry);
                    }
                    catch (Exception ex)
                    {
                        LogService.WriteError(ex.Message + "Status : " + pStrStatus + " DocEntry: " + factura.DocEntry + "Query" + lStrQueryUpdate);
                    }
                }

                SaveOrUpdateBitacoraHana();
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message + "Status : " + pStrStatus + " DocEntry: " + factura.DocEntry);
            }
        }

        public void SaveOrUpdateBitacoraHana()
        {
            using (OdbcConnection ConectionBitacora = new OdbcConnection(factura.mObjConfig.ConexionString))
            {
                string lStrQuery = string.Format("call \"SBOCFDI\".FacturasCFDiUpdate ({0}, {1}, {2}, '{3}', {4}, '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', {21}, {22}, {23}, {24}, {25}, {26}, {27}, {28} ,'{29}', '{30}', {31}, '{32}')",
                    factura.SerieDocumento,
                    factura.NumeroDocumento,
                    factura.DocEntry,
                    factura.DocType,
                    factura.Serie,
                    factura.NombreSerie,
                    factura.CodigoObjeto,
                factura.MetodoPago,
                factura.NombreMetodoPago,
                factura.CuentaCliente,
                factura.RFC,
                factura.FolioFiscal,
                factura.CertEmisor,
                factura.CertSAT,
                factura.SelloSAT,
                factura.SelloCFD,
                factura.FechaTimbrado,
                factura.TimbreFiscalDigital,
                factura.CadenaOriginal,
                factura.ArchivoXML,
                factura.ArchivoPDF,
                factura.ImporteFactura,
                factura.XMLModificado,
                factura.Timbrado,
                factura.XMLGenerado,
                factura.PDFGenerado,
                factura.EmailEnviado,
                factura.ArchivosAdjuntados,
                factura.XMLOriginalBorrado,
                factura.Error,
                "",
                factura.mObjConfig.IDSucursal,
                factura.XmlActualizado);

                ConectionBitacora.Open();
                OdbcCommand comandoSql = new OdbcCommand();
                comandoSql.Connection = ConectionBitacora;
                comandoSql.CommandType = CommandType.Text;
                comandoSql.CommandText = lStrQuery;
                comandoSql.ExecuteNonQuery();
            }
        }

        public void SaveOrUpdateDI(string pStrStatus)
        {
            switch (factura.CodigoObjeto)
            {
                case "13":
                    oInvoice = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                    break;
                case "14":
                    oInvoice = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes);
                    break;
                case "203":
                    oInvoice = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDownPayments);
                    break;
                default:
                    break;
            }

            bool lBolIsSuccess = false;
            if (oInvoice.GetByKey(int.Parse(factura.DocEntry.ToString())))
            {
                //falta agarrar el campo definido por usuario y agregar el status a la factura y descomentar la linea de abajo
                CargarObjetoFActuraSAP(pStrStatus);
                if (oInvoice.Update() != 0)
                {
                    string lStrError = string.Empty;
                    int lIntError = 0;
                    DIApplication.Company.GetLastError(out lIntError, out lStrError);
                    LogService.WriteError(lStrError + " - DocEntry: " + factura.DocEntry);
                    throw new Exception(lStrError);

                }
                else
                    lBolIsSuccess = true;
            }
            Memory.ReleaseComObject(oInvoice);
            factura.Status = int.Parse(pStrStatus);
            if (lBolIsSuccess)
                SaveOrUpdateBitacora();
        }

        public void SaveOrUpdateBitacora()
        {
            SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            lObjRecordSet.DoQuery("select Code from [@TIMBRADOSCFDI] WHERE U_IdDocumento ='" + factura.DocEntry.ToString() + "' and U_TipoDoc =" + factura.CodigoObjeto + "");
            SAPbobsCOM.UserTable lObjtable = DIApplication.Company.UserTables.Item("TIMBRADOSCFDI");
            if (lObjRecordSet.RecordCount > 0)
            {
                string lStrParametro = lObjRecordSet.Fields.Item("Code").Value.ToString();
                if (lObjtable.GetByKey(lStrParametro))
                {
                    lObjtable = CargarObjetoTabla(lObjtable);
                    if (lObjtable.Update() != 0)
                    {
                        string lStrError = string.Empty;
                        int lIntError = 0;
                        DIApplication.Company.GetLastError(out lIntError, out lStrError);
                        Memory.ReleaseComObject(lObjRecordSet);
                        LogService.WriteError(lStrError + " - DocEntry: " + factura.DocEntry);
                        throw new Exception(lStrError);
                    }
                }
            }
            else
            {
                lObjtable = CargarObjetoTabla(lObjtable);
                if (lObjtable.Add() != 0)
                {
                    string lStrError = string.Empty;
                    int lIntError = 0;
                    DIApplication.Company.GetLastError(out lIntError, out lStrError);
                    Memory.ReleaseComObject(lObjRecordSet);
                    LogService.WriteError(lStrError + " - DocEntry: " + factura.DocEntry);
                    throw new Exception(lStrError);
                }
            }
            Memory.ReleaseComObject(lObjRecordSet);
            Memory.ReleaseComObject(lObjtable);
        }

        public void SaveOrUpdateSQL(string pStrStatus)
        {
            string lStrTabla = string.Empty;
            string lStrQuery = string.Empty;
            SqlCommand comandoSql = null;
            //switch (factura.CodigoObjeto)
            //{
            //    case "13":
            //        lStrTabla = "OINV";
            //        break;
            //    case "14":
            //        lStrTabla = "ORIN";
            //        break;
            //    case "203":
            //        lStrTabla = "ODPI";
            //        break;
            //    default:
            //        break;
            //}
            //if (factura.mObjConfig.NewDB == "1")
            //{
            //    lStrQuery = "update " + lStrTabla + " " +
            //                    "set " +
            //                    "U_Status = '" + pStrStatus + "'," +
            //                    "U_SO1_02FOLIOOPER = '" + factura.FolioFiscal + "'," +
            //                    "U_UDF_UUID = '" + factura.FolioFiscal + "'," +
            //                    "U_ImporteFactura = " + factura.ImporteFactura + "," +
            //                    "U_SelloCFD = '" + factura.SelloCFD + "'," +
            //                    "U_SelloSAT = '" + factura.SelloSAT + "'," +
            //                    "U_TimbradoFiscalDigital = '" + factura.TimbreFiscalDigital + "'," +
            //                    "U_CertSAT = '" + factura.CertSAT + "'," +
            //                    "U_FolioFiscal = '" + factura.FolioFiscal + "'," +
            //                    "U_FechaTimbrado = '" + factura.FechaTimbrado + "'," +
            //                    "U_CertEmisor = '" + factura.CertEmisor + "'," +
            //                    "U_CadenaOriginal = '" + factura.CadenaOriginal + "', " +
            //                    "U_Error = '" + factura.Error.Replace("'","") + "', " +
            //                    "U_CFDI_RelUUID = '" + factura.DocRelacionados.Replace("'", "") + "', " +
            //                    "U_CFDI_TipoRel = '" + factura.TipoRelacion.Replace("'", "") + "', " +
            //                    "U_ArchivoXML = '" + factura.ArchivoXML + "', " +
            //                    "U_ArchivoPDF = '" + factura.ArchivoPDF + "' " +
            //                    " where DocEntry = " + factura.DocEntry;
            //}
            //else
            //{
            //    lStrQuery = "update " + lStrTabla + " " +
            //    "set " +
            //    "U_SO1_02FOLIOOPER = '" + factura.FolioFiscal + "'," +
            //    "U_SO1_02NUMCERT = '" + factura.CertEmisor + "'," +
            //    "U_SO1_02CADENAORIG = '" + factura.CadenaOriginal + "'," +
            //    "U_SO1_02SELLODIGITAL = '" + factura.SelloCFD + "'," +
            //    "U_SO1_02SELLOSAT = '" + factura.SelloSAT + "'," +
            //    "U_SO1_02FECHATIMB = '" + factura.FechaTimbrado + "'," +
            //    "U_SO1_02NUMCERTSAT = '" + factura.CertSAT + "'," +
            //    "U_SO1_02METODOPAGO = '" + factura.MetodoPago + "'," +
            //    "U_SO1_02SERIEFISCAL = '" + factura.NombreSerie + "'," +
            //    "U_SO1_02RUTACFD = '" + factura.ArchivoXML + "', " +
            //    "U_SO1_02RUTAPDF = '" + factura.ArchivoPDF + "', " +
            //    "U_ImporteFactura = '" + factura.ImporteFactura + "', " +
            //    "U_CFDI_RelUUID = '" + factura.DocRelacionados.Replace("'", "") + "', " +
            //    "U_CFDI_TipoRel = '" + factura.TipoRelacion.Replace("'", "") + "', " +
            //    "U_ErrorCFDi = '" + factura.Error + "' " +
            //    " where DocEntry = " + factura.DocEntry;
            //}


            using (SqlConnection conexion = new SqlConnection(factura.mObjConfig.ConexionString))
            {
                conexion.Open();
                comandoSql = new SqlCommand();
                try
                {
                    comandoSql.Connection = conexion;
                    comandoSql.CommandType = CommandType.StoredProcedure;
                    comandoSql.CommandText = "SP_SaveInvoiceByType";
                    comandoSql.Parameters.Add("@Cadena", SqlDbType.VarChar).Value = factura.CadenaOriginal;
                    comandoSql.Parameters.Add("@SelloCFD", SqlDbType.VarChar).Value = factura.SelloCFD;
                    comandoSql.Parameters.Add("@SelloSAT", SqlDbType.VarChar).Value = factura.SelloSAT;
                    comandoSql.Parameters.Add("@CertSAT", SqlDbType.VarChar).Value = factura.CertSAT;
                    comandoSql.Parameters.Add("@FolioFiscal", SqlDbType.VarChar).Value = factura.FolioFiscal;
                    comandoSql.Parameters.Add("@FechaTimbrado", SqlDbType.VarChar).Value = factura.FechaTimbrado;
                    comandoSql.Parameters.Add("@CertEmisor", SqlDbType.VarChar).Value = factura.CertEmisor;
                    comandoSql.Parameters.Add("@Error", SqlDbType.VarChar).Value = factura.Error;
                    comandoSql.Parameters.Add("@NombreMetodoPago", SqlDbType.VarChar).Value = factura.NombreMetodoPago;
                    comandoSql.Parameters.Add("@ArchivoXML", SqlDbType.VarChar).Value = factura.ArchivoXML;
                    comandoSql.Parameters.Add("@ArchivoPDF", SqlDbType.VarChar).Value = factura.ArchivoPDF;
                    comandoSql.Parameters.Add("@DocEntry", SqlDbType.Int).Value = factura.DocEntry;
                    comandoSql.Parameters.Add("@DocType", SqlDbType.VarChar).Value = factura.CodigoObjeto;
                    comandoSql.Parameters.Add("@ImporteFactura", SqlDbType.Decimal).Value = factura.ImporteFactura;
                    comandoSql.Parameters.Add("@TipoRel", SqlDbType.VarChar).Value = factura.TipoRelacion;
                    comandoSql.Parameters.Add("@DocRel", SqlDbType.VarChar).Value = factura.DocRelacionados;


                    comandoSql.ExecuteNonQuery();

                    using (SqlConnection ConectionBitacora = new SqlConnection(ConfigurationManager.ConnectionStrings["CFDiDB"].ToString()))
                    {
                        ConectionBitacora.Open();
                        comandoSql = new SqlCommand();
                        comandoSql.Connection = ConectionBitacora;
                        comandoSql.CommandType = CommandType.StoredProcedure;
                        comandoSql.CommandText = "FacturasCFDiUpdate";
                        comandoSql.Parameters.Add(new SqlParameter("@SerieDocumento", SqlDbType.SmallInt)).Value = factura.SerieDocumento;
                        comandoSql.Parameters.Add(new SqlParameter("@NumeroDocumento", SqlDbType.BigInt)).Value = factura.NumeroDocumento;
                        comandoSql.Parameters.Add(new SqlParameter("@DocEntry", SqlDbType.BigInt)).Value = factura.DocEntry;
                        comandoSql.Parameters.Add(new SqlParameter("@DocTipo", SqlDbType.VarChar)).Value = factura.DocType;
                        comandoSql.Parameters.Add(new SqlParameter("@Serie", SqlDbType.SmallInt)).Value = factura.Serie;
                        comandoSql.Parameters.Add(new SqlParameter("@NombreSerie", SqlDbType.VarChar)).Value = factura.NombreSerie;
                        comandoSql.Parameters.Add(new SqlParameter("@CodigoObjeto", SqlDbType.VarChar)).Value = factura.CodigoObjeto;
                        comandoSql.Parameters.Add(new SqlParameter("@MetodoPago", SqlDbType.VarChar)).Value = factura.MetodoPago;
                        comandoSql.Parameters.Add(new SqlParameter("@NombreMetodoPago", SqlDbType.VarChar)).Value = factura.NombreMetodoPago;
                        comandoSql.Parameters.Add(new SqlParameter("@CuentaCliente", SqlDbType.VarChar)).Value = factura.CuentaCliente;
                        comandoSql.Parameters.Add(new SqlParameter("@RFC", SqlDbType.VarChar)).Value = factura.RFC;
                        comandoSql.Parameters.Add(new SqlParameter("@FolioFiscal", SqlDbType.VarChar)).Value = factura.FolioFiscal;
                        comandoSql.Parameters.Add(new SqlParameter("@CertEmisor", SqlDbType.VarChar)).Value = factura.CertEmisor;
                        comandoSql.Parameters.Add(new SqlParameter("@CertSAT", SqlDbType.VarChar)).Value = factura.CertSAT;
                        comandoSql.Parameters.Add(new SqlParameter("@SelloSAT", SqlDbType.VarChar)).Value = factura.SelloSAT;
                        comandoSql.Parameters.Add(new SqlParameter("@SelloCFD", SqlDbType.VarChar)).Value = factura.SelloCFD;
                        comandoSql.Parameters.Add(new SqlParameter("@FechaTimbrado", SqlDbType.VarChar)).Value = factura.FechaTimbrado;
                        comandoSql.Parameters.Add(new SqlParameter("@TimbreFiscalDigital", SqlDbType.Text)).Value = factura.TimbreFiscalDigital;
                        comandoSql.Parameters.Add(new SqlParameter("@CadenaOriginal", SqlDbType.Text)).Value = factura.CadenaOriginal;
                        comandoSql.Parameters.Add(new SqlParameter("@ArchivoXML", SqlDbType.VarChar)).Value = factura.ArchivoXML;
                        comandoSql.Parameters.Add(new SqlParameter("@ArchivoPDF", SqlDbType.VarChar)).Value = factura.ArchivoPDF;
                        comandoSql.Parameters.Add(new SqlParameter("@ImporteFactura", SqlDbType.Decimal)).Value = factura.ImporteFactura;
                        comandoSql.Parameters.Add(new SqlParameter("@XMLModificado", SqlDbType.Bit)).Value = factura.XMLModificado;
                        comandoSql.Parameters.Add(new SqlParameter("@Timbrado", SqlDbType.Bit)).Value = factura.Timbrado;
                        comandoSql.Parameters.Add(new SqlParameter("@XMLGenerado", SqlDbType.Bit)).Value = factura.XMLGenerado;
                        comandoSql.Parameters.Add(new SqlParameter("@PDFGenerado", SqlDbType.Bit)).Value = factura.PDFGenerado;
                        comandoSql.Parameters.Add(new SqlParameter("@EmailEnviado", SqlDbType.Bit)).Value = factura.EmailEnviado;
                        comandoSql.Parameters.Add(new SqlParameter("@ArchivosAdjuntados", SqlDbType.Bit)).Value = factura.ArchivosAdjuntados;
                        comandoSql.Parameters.Add(new SqlParameter("@XMLOriginalBorrado", SqlDbType.Bit)).Value = factura.XMLOriginalBorrado;
                        comandoSql.Parameters.Add(new SqlParameter("@Error", SqlDbType.VarChar)).Value = factura.Error;
                        //comandoSql.Parameters.Add(new SqlParameter("@NombreSN", SqlDbType.VarChar)).Value = factura.NombreSN;
                        comandoSql.Parameters.Add(new SqlParameter("@XmlActualizado", SqlDbType.VarChar)).Value = factura.XmlActualizado;
                        comandoSql.Parameters.Add(new SqlParameter("@IdSucursal", SqlDbType.BigInt)).Value = factura.mObjConfig.IDSucursal;
                        comandoSql.ExecuteNonQuery();
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conexion.Dispose();
                    conexion.Close();
                }
            }
        }

        #endregion

        #region Complemento de Pagos
        /*
        public void SaveOrUpdatePagoHana(string pStrStatus)
        {
            OdbcCommand comandoHana;
            string lStrQueryUpdate = string.Empty;
            if (mObjPago.Error != "")
            {
                lStrQueryUpdate = "update \"" + mObjPago.mObjConfig.CompanyName + "\".ORCT " +
                        "set " +
                        "\"U_Status\" = '" + pStrStatus + "', " +
                        "\"U_Error\" = '" + mObjPago.Error.Replace("'", "") + "' where \"DocEntry\" = " + mObjPago.DocEntry;
            }
            else
            {
                lStrQueryUpdate = "update \"" + mObjPago.mObjConfig.CompanyName + "\".ORCT " +
                        "set " +
                        "\"U_Status\" = '" + pStrStatus + "'," +
                        "\"U_SelloCFD\" = '" + mObjPago.SelloCFD + "'," +
                        "\"U_SelloSAT\" = '" + mObjPago.SelloSAT + "'," +
                        "\"U_TimbradoFiscalDigital\" = '" + mObjPago.TimbreFiscalDigital + "'," +
                        "\"U_CertSAT\" = '" + mObjPago.CertSAT + "'," +
                        "\"U_FolioFiscal\" = '" + mObjPago.FolioFiscal + "'," +
                        "\"U_FechaTimbrado\" = '" + mObjPago.FechaTimbrado + "'," +
                        "\"U_CertEmisor\" = '" + mObjPago.CertEmisor + "'," +
                        "\"U_CadenaOriginal\" = '" + mObjPago.CadenaOriginal + "'," +
                        "\"U_Error\" = '" + mObjPago.Error + "', " +
                        "\"U_ArchivoXML\" = '" + mObjPago.ArchivoXML + "', " +
                        "\"U_ArchivoPDF\" = '" + mObjPago.ArchivoPDF + "', " + "' where \"DocEntry\" = " + mObjPago.DocEntry;
            }

            try
            {
                using (OdbcConnection conexion = new OdbcConnection(StringConnection))
                {
                    try
                    {
                        conexion.Open();
                        comandoHana = new OdbcCommand();
                        comandoHana.Connection = conexion;
                        comandoHana.CommandType = CommandType.Text;
                        comandoHana.CommandText = lStrQueryUpdate;
                        OdbcDataAdapter reader = new OdbcDataAdapter(comandoHana);
                        DataTable tabla = new DataTable();
                        reader.Fill(tabla);
                        conexion.Dispose();
                        conexion.Close();
                        LogService.WriteInfo("Operacion realizada correctamente status: " + pStrStatus + "DocEntry : " + mObjPago.DocEntry);
                    }
                    catch (Exception ex)
                    {
                        LogService.WriteError(ex.Message + "Status : " + pStrStatus + " DocEntry: " + mObjPago.DocEntry + "Query" + lStrQueryUpdate);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message + "Status : " + pStrStatus + " DocEntry: " + mObjPago.DocEntry);
            }
        }
        
        public void SaveOrUpdatePagoDI(string pStrStatus)
        {
            mObjPagoDoc = (SAPbobsCOM.Payments)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments);
            if (mObjPagoDoc.GetByKey(mObjPago.DocEntry))
            {
                CargarObjetoPagoSAP(pStrStatus);
                if (mObjPagoDoc.Update() != 0)
                {
                    string lStrError = string.Empty;
                    int lIntError = 0;
                    DIApplication.Company.GetLastError(out lIntError, out lStrError);
                    LogService.WriteError(lStrError + " - DocEntry: " + mObjPago.DocEntry);
                    throw new Exception(lStrError);
                }
            }

            Memory.ReleaseComObject(mObjPagoDoc);
            mObjPago.Status = int.Parse(pStrStatus);
            SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            lObjRecordSet.DoQuery("select Code from [@TIMBRADOSCFDI] WHERE U_IdDocumento ='" + mObjPago.DocEntry.ToString() + "' and U_TipoDoc =" + mObjPago.CodigoObjeto + "");
            SAPbobsCOM.UserTable lObjtable = DIApplication.Company.UserTables.Item("TIMBRADOSCFDI");
            if (lObjRecordSet.RecordCount > 0)
            {
                string lStrParametro = lObjRecordSet.Fields.Item("Code").Value.ToString();
                if (lObjtable.GetByKey(lStrParametro))
                {
                    lObjtable = CargarObjetoTablaPago(lObjtable);
                    if (lObjtable.Update() != 0)
                    {
                        string lStrError = string.Empty;
                        int lIntError = 0;
                        DIApplication.Company.GetLastError(out lIntError, out lStrError);
                        Memory.ReleaseComObject(lObjRecordSet);
                        LogService.WriteError(lStrError + " - DocEntry: " + mObjPago.DocEntry);
                        throw new Exception(lStrError);

                    }
                }
            }
            else
            {
                lObjtable = CargarObjetoTablaPago(lObjtable);
                if (lObjtable.Add() != 0)
                {
                    string lStrError = string.Empty;
                    int lIntError = 0;
                    DIApplication.Company.GetLastError(out lIntError, out lStrError);
                    Memory.ReleaseComObject(lObjRecordSet);
                    LogService.WriteError(lStrError + " - DocEntry: " + mObjPago.DocEntry);
                    throw new Exception(lStrError);
                }
            }
            Memory.ReleaseComObject(lObjRecordSet);
            Memory.ReleaseComObject(lObjtable);
        }

        public void SaveOrUpdatePagoSQL(string pStrStatus)
        {
            string lStrTabla = string.Empty;
            string lStrQuery = string.Empty;
            SqlCommand comandoSql = null;


            using (SqlConnection conexion = new SqlConnection(mObjPago.mObjConfig.ConexionString))
            {
                conexion.Open();
                comandoSql = new SqlCommand();
                comandoSql.Connection = conexion;
                comandoSql.CommandType = CommandType.StoredProcedure;
                comandoSql.CommandText = "SP_SavePaymentCfdi";
                comandoSql.Parameters.Add(new SqlParameter("@DocEntry", SqlDbType.Int)).Value = mObjPago.DocEntry;
                comandoSql.Parameters.Add(new SqlParameter("@SelloCFD", SqlDbType.VarChar)).Value = mObjPago.SelloCFD;
                comandoSql.Parameters.Add(new SqlParameter("@SelloSAT", SqlDbType.VarChar)).Value = mObjPago.SelloSAT;
                comandoSql.Parameters.Add(new SqlParameter("@TimbreFiscalDigital", SqlDbType.VarChar)).Value = mObjPago.TimbreFiscalDigital;
                comandoSql.Parameters.Add(new SqlParameter("@CertSAT", SqlDbType.VarChar)).Value = mObjPago.CertSAT;
                comandoSql.Parameters.Add(new SqlParameter("@FolioFiscal", SqlDbType.VarChar)).Value = mObjPago.FolioFiscal;
                comandoSql.Parameters.Add(new SqlParameter("@FechaTimbrado", SqlDbType.VarChar)).Value = mObjPago.FechaTimbrado;
                comandoSql.Parameters.Add(new SqlParameter("@CertEmisor", SqlDbType.VarChar)).Value = mObjPago.CertEmisor;
                comandoSql.Parameters.Add(new SqlParameter("@CadenaOriginal", SqlDbType.VarChar)).Value = mObjPago.CadenaOriginal;
                comandoSql.Parameters.Add(new SqlParameter("@Error", SqlDbType.VarChar)).Value = mObjPago.Error;
                comandoSql.Parameters.Add(new SqlParameter("@ArchivoXML", SqlDbType.VarChar)).Value = mObjPago.ArchivoXML;
                comandoSql.Parameters.Add(new SqlParameter("@ArchivoPDF", SqlDbType.VarChar)).Value = mObjPago.ArchivoPDF;
                comandoSql.ExecuteNonQuery();

            }

            using (SqlConnection ConectionBitacora = new SqlConnection(ConfigurationManager.ConnectionStrings["CFDiDB"].ToString()))
            {
                ConectionBitacora.Open();
                comandoSql = new SqlCommand();
                comandoSql.Connection = ConectionBitacora;
                comandoSql.CommandType = CommandType.StoredProcedure;
                comandoSql.CommandText = "FacturasCFDiUpdate";
                comandoSql.Parameters.Add(new SqlParameter("@SerieDocumento", SqlDbType.SmallInt)).Value = mObjPago.SerieDocumento;
                comandoSql.Parameters.Add(new SqlParameter("@NumeroDocumento", SqlDbType.BigInt)).Value = mObjPago.NumeroDocumento;
                comandoSql.Parameters.Add(new SqlParameter("@DocEntry", SqlDbType.BigInt)).Value = mObjPago.DocEntry;
                comandoSql.Parameters.Add(new SqlParameter("@DocTipo", SqlDbType.VarChar)).Value = mObjPago.CodigoObjeto;
                comandoSql.Parameters.Add(new SqlParameter("@Serie", SqlDbType.SmallInt)).Value = mObjPago.SerieDocumento;
                comandoSql.Parameters.Add(new SqlParameter("@NombreSerie", SqlDbType.VarChar)).Value = mObjPago.SerieName;
                comandoSql.Parameters.Add(new SqlParameter("@CodigoObjeto", SqlDbType.VarChar)).Value = mObjPago.CodigoObjeto;
                comandoSql.Parameters.Add(new SqlParameter("@MetodoPago", SqlDbType.VarChar)).Value = "";
                comandoSql.Parameters.Add(new SqlParameter("@NombreMetodoPago", SqlDbType.VarChar)).Value = "";
                comandoSql.Parameters.Add(new SqlParameter("@CuentaCliente", SqlDbType.VarChar)).Value = "";
                comandoSql.Parameters.Add(new SqlParameter("@RFC", SqlDbType.VarChar)).Value = mObjPago.Rfc;
                comandoSql.Parameters.Add(new SqlParameter("@FolioFiscal", SqlDbType.VarChar)).Value = mObjPago.FolioFiscal;
                comandoSql.Parameters.Add(new SqlParameter("@CertEmisor", SqlDbType.VarChar)).Value = mObjPago.CertEmisor;
                comandoSql.Parameters.Add(new SqlParameter("@CertSAT", SqlDbType.VarChar)).Value = mObjPago.CertSAT;
                comandoSql.Parameters.Add(new SqlParameter("@SelloSAT", SqlDbType.VarChar)).Value = mObjPago.SelloSAT;
                comandoSql.Parameters.Add(new SqlParameter("@SelloCFD", SqlDbType.VarChar)).Value = mObjPago.SelloCFD;
                comandoSql.Parameters.Add(new SqlParameter("@FechaTimbrado", SqlDbType.VarChar)).Value = mObjPago.FechaTimbrado;
                comandoSql.Parameters.Add(new SqlParameter("@TimbreFiscalDigital", SqlDbType.Text)).Value = mObjPago.TimbreFiscalDigital;
                comandoSql.Parameters.Add(new SqlParameter("@CadenaOriginal", SqlDbType.Text)).Value = mObjPago.CadenaOriginal;
                comandoSql.Parameters.Add(new SqlParameter("@ArchivoXML", SqlDbType.VarChar)).Value = mObjPago.ArchivoXML;
                comandoSql.Parameters.Add(new SqlParameter("@ArchivoPDF", SqlDbType.VarChar)).Value = mObjPago.ArchivoPDF;
                comandoSql.Parameters.Add(new SqlParameter("@ImporteFactura", SqlDbType.Decimal)).Value = 0;
                comandoSql.Parameters.Add(new SqlParameter("@XMLModificado", SqlDbType.Bit)).Value = mObjPago.XMLModificado;
                comandoSql.Parameters.Add(new SqlParameter("@Timbrado", SqlDbType.Bit)).Value = mObjPago.Timbrado;
                comandoSql.Parameters.Add(new SqlParameter("@XMLGenerado", SqlDbType.Bit)).Value = mObjPago.XMLGenerado;
                comandoSql.Parameters.Add(new SqlParameter("@PDFGenerado", SqlDbType.Bit)).Value = mObjPago.PDFGenerado;
                comandoSql.Parameters.Add(new SqlParameter("@EmailEnviado", SqlDbType.Bit)).Value = mObjPago.EmailEnviado;
                comandoSql.Parameters.Add(new SqlParameter("@ArchivosAdjuntados", SqlDbType.Bit)).Value = mObjPago.ArchivosAdjuntados;
                comandoSql.Parameters.Add(new SqlParameter("@XMLOriginalBorrado", SqlDbType.Bit)).Value = mObjPago.XMLOriginalBorrado;
                comandoSql.Parameters.Add(new SqlParameter("@Error", SqlDbType.VarChar)).Value = mObjPago.Error;
                //comandoSql.Parameters.Add(new SqlParameter("@NombreSN", SqlDbType.VarChar)).Value = factura.NombreSN;
                comandoSql.Parameters.Add(new SqlParameter("@XmlActualizado", SqlDbType.VarChar)).Value = mObjPago.XmlActualizado;
                comandoSql.Parameters.Add(new SqlParameter("@IdSucursal", SqlDbType.BigInt)).Value = mObjPago.mObjConfig.IDSucursal;
                comandoSql.ExecuteNonQuery();
            }
        }

        public void UpdateSAPPago(string pStrStatus)
        {
            try
            {
                string lStrDbServer = ConfigurationManager.AppSettings["SAPDBServer"].ToString();
                string lStrQueryUpdate = string.Empty;

                if (lStrDbServer == "HANA")
                {
                    SaveOrUpdatePagoHana(pStrStatus);
                }
                else
                {
                    if (ConfigurationManager.AppSettings["UpdatesDirectos"].ToString() == "1")
                        SaveOrUpdatePagoSQL(pStrStatus);
                    else
                        SaveOrUpdatePagoDI(pStrStatus);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message + " - DocEntry: " + mObjPago.DocEntry);
                throw ex;
            }
        }
        */
        #endregion

        private SAPbobsCOM.UserTable CargarObjetoTabla(SAPbobsCOM.UserTable pObjTable)
        {
            pObjTable.UserFields.Fields.Item("U_IdDocumento").Value = factura.DocEntry.ToString();
            pObjTable.UserFields.Fields.Item("U_FechaTimbrado").Value = factura.FechaTimbrado;
            pObjTable.UserFields.Fields.Item("U_Error").Value = factura.Error;
            pObjTable.UserFields.Fields.Item("U_TipoDoc").Value = factura.CodigoObjeto;
            pObjTable.UserFields.Fields.Item("U_Status").Value = factura.Status.ToString();
            pObjTable.UserFields.Fields.Item("U_FolioFiscal").Value = factura.FolioFiscal;
            return pObjTable;
        }

        //private SAPbobsCOM.UserTable CargarObjetoTablaPago(SAPbobsCOM.UserTable pObjTable)
        //{
        //    pObjTable.UserFields.Fields.Item("U_IdDocumento").Value = mObjPago.DocEntry.ToString();
        //    pObjTable.UserFields.Fields.Item("U_FechaTimbrado").Value = mObjPago.FechaTimbrado;
        //    pObjTable.UserFields.Fields.Item("U_Error").Value = mObjPago.Error;
        //    pObjTable.UserFields.Fields.Item("U_TipoDoc").Value = mObjPago.CodigoObjeto;
        //    pObjTable.UserFields.Fields.Item("U_Status").Value = mObjPago.Status.ToString();
        //    pObjTable.UserFields.Fields.Item("U_FolioFiscal").Value = mObjPago.FolioFiscal;
        //    return pObjTable;
        //}

        private void CargarObjetoFActuraSAP(string pStrStatus)
        {
            oInvoice.UserFields.Fields.Item("U_Status").Value = pStrStatus;
            oInvoice.UserFields.Fields.Item("U_SO1_02FOLIOOPER").Value = factura.FolioFiscal;
            oInvoice.UserFields.Fields.Item("U_UDF_UUID").Value = factura.FolioFiscal;
            oInvoice.UserFields.Fields.Item("U_ImporteFactura").Value = factura.ImporteFactura;
            oInvoice.UserFields.Fields.Item("U_SelloCFD").Value = factura.SelloCFD;
            oInvoice.UserFields.Fields.Item("U_SelloSAT").Value = factura.SelloSAT;
            oInvoice.UserFields.Fields.Item("U_TimbradoFiscalDigital").Value = factura.TimbreFiscalDigital;
            oInvoice.UserFields.Fields.Item("U_CertSAT").Value = factura.CertSAT;
            oInvoice.UserFields.Fields.Item("U_FolioFiscal").Value = factura.FolioFiscal;
            oInvoice.UserFields.Fields.Item("U_FechaTimbrado").Value = factura.FechaTimbrado;
            oInvoice.UserFields.Fields.Item("U_CertEmisor").Value = factura.CertEmisor;
            oInvoice.UserFields.Fields.Item("U_ArchivoXML").Value = factura.ArchivoXML;
            oInvoice.UserFields.Fields.Item("U_ArchivoPDF").Value = factura.ArchivoPDF;
            //oInvoice.UserFields.Fields.Item("U_NombreMetodoPago").Value = factura.NombreMetodoPago;
            oInvoice.UserFields.Fields.Item("U_CadenaOriginal").Value = factura.CadenaOriginal;
            //oInvoice.UserFields.Fields.Item("U_CantidadLetra").Value = factura.cantidadletra;
        }
        /*
        private void CargarObjetoPagoSAP(string pStrStatus)
        {
            mObjPagoDoc.UserFields.Fields.Item("U_Status").Value = pStrStatus;
            mObjPagoDoc.UserFields.Fields.Item("U_SelloCFD").Value = mObjPago.SelloCFD;
            mObjPagoDoc.UserFields.Fields.Item("U_SelloSAT").Value = mObjPago.SelloSAT;
            mObjPagoDoc.UserFields.Fields.Item("U_TimbreFiscalDigital").Value = mObjPago.TimbreFiscalDigital;
            mObjPagoDoc.UserFields.Fields.Item("U_CertSAT").Value = mObjPago.CertSAT;
            mObjPagoDoc.UserFields.Fields.Item("U_FolioFiscal").Value = mObjPago.FolioFiscal;
            mObjPagoDoc.UserFields.Fields.Item("U_FechaTimbrado").Value = mObjPago.FechaTimbrado;
            mObjPagoDoc.UserFields.Fields.Item("U_CertEmisor").Value = mObjPago.CertEmisor;
            mObjPagoDoc.UserFields.Fields.Item("U_CadenaOriginal").Value = mObjPago.CadenaOriginal;
        }
        */
        public void CrearFacturasPruebas()
        {
            SAPbobsCOM.Documents oInvoice = null;
            try
            {
                for (int i = 0; i < 150; i++)
                {
                    oInvoice = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                    oInvoice.DocDate = DateTime.Now;
                    oInvoice.DocObjectCode = SAPbobsCOM.BoObjectTypes.oInvoices;
                    oInvoice.CardCode = "CL00000001";
                    oInvoice.Series = 407;
                    oInvoice.UserFields.Fields.Item("U_B1SYS_MainUsage").Value = "G01";
                    oInvoice.Lines.ItemCode = "A00000001";
                    oInvoice.Lines.Price = 300.00;
                    oInvoice.Lines.TaxCode = "V0";
                    oInvoice.Lines.Add();
                    if (oInvoice.Add() != 0)
                    {
                        string lStrError = string.Empty;
                        int lIntError = 0;
                        DIApplication.Company.GetLastError(out lIntError, out lStrError);

                        LogService.WriteError(lStrError + " - DocEntry: " + factura.DocEntry);
                        throw new Exception(lStrError);
                    }
                    Memory.ReleaseComObject(oInvoice);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                Memory.ReleaseComObject(oInvoice);
            }

        }
        /*
        private void GetCompanies()
        {
            using (SqlConnection lObjConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["CFDiDB"].ToString()))
            {
                lObjConnection.Open();
                SqlCommand lObjCommand = new SqlCommand();
                lObjCommand.Connection = lObjConnection;
                lObjCommand.CommandType = CommandType.Text;
                lObjCommand.CommandText = "SELECT IDSuc, " +
                            "max(case when parametro ='ContratoProd' then valor else '' end) ContratoProd, " +
                            "max(case when parametro ='UsuarioProd' then valor else '' end) UsuarioProd, " +
                            "max(case when parametro ='PasswProd' then valor else '' end) PasswProd, " +
                            "max(case when parametro ='XmlTimbradoPath' then valor else '' end) XmlTimbradoPath, " +
                            "max(case when parametro ='ArchivoCertificado' then valor else '' end) ArchivoCertificado, " +
                            "max(case when parametro ='CertificadoSAT' then valor else '' end) CertificadoSAT, " +
                            "max(case when parametro ='KeySAT' then valor else '' end) KeySAT, " +
                            "max(case when parametro ='PasswCertificado' then valor else '' end) PasswCertificado, " +
                            "max(case when parametro ='SMTPServer' then valor else '' end) SMTPServer, " +
                            "max(case when parametro ='SMTPPort' then valor else '' end) SMTPPort, " +
                            "max(case when parametro ='SMTPSSL' then valor else '' end) SMTPSSL, " +
                            "max(case when parametro ='SMTPUser' then valor else '' end) SMTPUser, " +
                            "max(case when parametro ='SMTPPassword' then valor else '' end) SMTPPassword, " +
                            "max(case when parametro ='SenderEmail' then valor else '' end) SenderEmail, " +
                            "max(case when parametro ='CCEmails' then valor else '' end) CCEmails, " +
                            "max(case when parametro ='Body' then valor else '' end) Body, " +
                            "max(case when parametro ='SAPB1DB' then valor else '' end) SAPB1DB, " +
                            "max(case when parametro ='Mail' then valor else '' end) Mail, " +
                            "max(case when parametro ='RutaRepCrystal' then valor else '' end) RutaRepCrystal, " +
                            "max(case when parametro ='EnviaCorreoCan' then valor else '' end) EnviaCorreoCan, " +
                            "max(case when parametro ='EnviaCorreo' then valor else '' end) EnviaCorreo, " +
                            "max(case when parametro ='XmlCanceladoPath' then valor else '' end) XmlCanceladoPath, " +
                            "max(case when parametro ='Subject' then valor else '' end) [Subject], " +
                            "max(case when parametro ='Server' then valor else '' end) Server, " +
                            "max(case when parametro ='LicenseServer' then valor else '' end) LicenseServer, " +
                            "max(case when parametro ='UsernameSAP' then valor else '' end) UsernameSAP, " +
                            "max(case when parametro ='PassSAP' then valor else '' end) PassSAP, " +
                            "max(case when parametro ='DBServerType' then valor else '' end) DBServerType, " +
                            "max(case when parametro ='ServerSQL' then valor else '' end) ServerSQL, " +
                            "max(case when parametro ='UserSQL' then valor else '' end) UserSQL, " +
                            "max(case when parametro ='PassSQL' then valor else '' end) PassSQL, " +
                            "max(case when parametro ='DBName' then valor else '' end) DBName, " +
                            "max(case when parametro ='Language' then valor else '' end) Language " +
                            "FROM   Parametrizaciones " +
                            "group by IDSuc ";
                SqlDataReader lObjReader = lObjCommand.ExecuteReader();
                TimbradoSetupDTO lObjConfigCFDi;
                mLstConfig = new List<TimbradoSetupDTO>();

                while (lObjReader.Read())
                {
                    lObjConfigCFDi = new TimbradoSetupDTO();
                    lObjConfigCFDi.IDSucursal = lObjReader["IDSuc"].ToString();
                    lObjConfigCFDi.Contract = lObjReader["ContratoProd"].ToString();
                    lObjConfigCFDi.WSUrl = lObjReader["ContratoProd"].ToString();
                    lObjConfigCFDi.User = lObjReader["UsuarioProd"].ToString();
                    lObjConfigCFDi.Passw = lObjReader["PasswProd"].ToString();
                    lObjConfigCFDi.CertificadoSAT = lObjReader["CertificadoSAT"].ToString();
                    lObjConfigCFDi.newXmlPath = lObjReader["XmlTimbradoPath"].ToString();
                    lObjConfigCFDi.ArchivoCertificado = lObjReader["ArchivoCertificado"].ToString();
                    lObjConfigCFDi.KeySAT = lObjReader["KeySAT"].ToString();
                    lObjConfigCFDi.PasswCertificado = lObjReader["PasswCertificado"].ToString();
                    lObjConfigCFDi.SMTPServer = lObjReader["SMTPServer"].ToString();
                    lObjConfigCFDi.SMTPPort = lObjReader["SMTPPort"].ToString();
                    lObjConfigCFDi.SMTPSSL = lObjReader["SMTPSSL"].ToString();
                    lObjConfigCFDi.SMTPUser = lObjReader["SMTPUser"].ToString();
                    lObjConfigCFDi.SMTPPassword = lObjReader["SMTPPassword"].ToString();
                    lObjConfigCFDi.SenderEmail = lObjReader["SenderEmail"].ToString();
                    lObjConfigCFDi.CCEmails = lObjReader["CCEmails"].ToString();
                    lObjConfigCFDi.Body = lObjReader["Body"].ToString();
                    lObjConfigCFDi.Subject = lObjReader["Subject"].ToString();
                    lObjConfigCFDi.ConexionString = lObjReader["SAPB1DB"].ToString();
                    lObjConfigCFDi.EnviaCorreoCan = lObjReader["EnviaCorreoCan"].ToString();
                    lObjConfigCFDi.EnviaCorreo = lObjReader["EnviaCorreo"].ToString();
                    lObjConfigCFDi.XmlPathCan = lObjReader["XmlCanceladoPath"].ToString();
                    lObjConfigCFDi.Server = lObjReader["Server"].ToString();
                    lObjConfigCFDi.LicenseServer = lObjReader["LicenseServer"].ToString();
                    lObjConfigCFDi.UserNameSAP = lObjReader["UserNameSAP"].ToString();
                    lObjConfigCFDi.PassSAP = lObjReader["PassSAP"].ToString();
                    lObjConfigCFDi.DBServerType = (SAPbobsCOM.BoDataServerTypes)Enum.Parse(typeof(SAPbobsCOM.BoDataServerTypes), lObjReader["DBServerType"].ToString(), true);
                    lObjConfigCFDi.ServerSQL = lObjReader["ServerSQL"].ToString();
                    lObjConfigCFDi.UserSQL = lObjReader["UserSQL"].ToString();
                    lObjConfigCFDi.PassSQL = lObjReader["PassSQL"].ToString();
                    lObjConfigCFDi.DBName = lObjReader["DBName"].ToString();
                    lObjConfigCFDi.Language = (SAPbobsCOM.BoSuppLangs)Enum.Parse(typeof(SAPbobsCOM.BoSuppLangs), lObjReader["Language"].ToString(), true);
                    mLstConfig.Add(lObjConfigCFDi);
                }
            }
        }*/

        /*public void CreateTablesAndFields()
        {
            GetCompanies();

            foreach (TimbradoSetupDTO lObjConfig in mLstConfig)
            {
                DIQueryManager lObj = new DIQueryManager(lObjConfig);
                lObj.DdPrepare();
            }
        }*/

        #endregion
    }
}
