using System;
using System.Collections.Generic;

namespace UGRS.Core.SDK.DI.Purchases.DTO
{
    public class PurchaseXMLDTO
    {
        public int DocEntry { get; set; }
        public string Folio { get; set; }
        public string CodeVoucher { get; set; }
        public string FolioFiscal { get; set; }
        public string RFCProvider { get; set; }
        public string BPName { get; set; }
        public string Name { get; set; }
        public string RFCReceptor { get; set; }
        public List<ConceptsXMLDTO> ConceptLines { get; set; }
        public string SubTotal { get; set; }
        public string Total { get; set; }
        public string CurrencyDocument { get; set; }
        public string CardCode { get; set; }
        public string Withholdings { get; set; }
        public string Version { get; set; }
        public string Date { get; set; }
        public string Area { get; set; }
        public string Employee { get; set; }
        public string XMLFile { get; set; }
        public string PDFFile { get; set; }
        public string Account { get; set; }
        public string TaxesTransfers { get; set; }
        public double Ieps { get; set; }
        public double Iva { get; set; }
        public double RetISR { get; set; }
        public double RetIva4 { get; set; }
        public double RetIva { get; set; }
        public string Obs { get; set; }
        public string MQRise { get; set; }
        public string CodeMov { get; set; }
        public string XMLTotal { get; set; }
        public List<TaxesXMLDTO> WithholdingTax { get; set; }
        public DateTime TaxDate { get; set; }
        public DateTime DocDate { get; set; }
        public List<TaxesXMLDTO> LstLocalTax { get; set; }
        public string Type { get; set; }
        public string RowLine { get; set; }
        public string ReferenceFolio { get; set; }
        public bool IsDraft { get; set; }
       
    }
}
