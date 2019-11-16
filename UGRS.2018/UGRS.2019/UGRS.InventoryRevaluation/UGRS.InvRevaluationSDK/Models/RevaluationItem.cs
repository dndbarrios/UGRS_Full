namespace UGRS.InvRevaluationSDK.Models {
    public class RevaluationItem {
        public int Row { get; set; } 
        public string Description { get; set; }
        public string ItemCode { get; set; }
        public string whCode { get; set; }
        public double Rev1 { get; set; }
        public double Rev2 { get; set; }
        public double Qty { get; set; }
        public double QtyAdj { get; set; }
        public string CostoReal { get; set; }
        public System.DateTime DocDateRev1 { get; set; }
        public System.DateTime DocDateRev2 { get; set; }
        public double CostoRev1 { get; set; }
        public double CostoRev2 { get; set; }

    }
}
