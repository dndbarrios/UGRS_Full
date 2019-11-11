namespace UGRS.InvRevaluationSDK.Models {
    public class RevaluationItem {
        public int Row { get; set; }
        public string Description { get; set; }
        public string ItemCode { get; set; }
        public string Whs { get; set; }
        public double Rev1 { get; set; }
        public double Qty { get; set; }
        public double QtyAdj { get; set; }
        public System.DateTime DocDate { get; set; }
    }
}
