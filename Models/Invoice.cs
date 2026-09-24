namespace Legacy.Models
{
    public class Invoice
    {
        public string No { get; set; }
        public string Cust { get; set; }
        public decimal Amt { get; set; }
        public decimal Tax { get; set; }
        public decimal Tot { get; set; }
        public decimal Bal { get; set; }
        public DateTime Dt { get; set; }
        public DateTime Due { get; set; }
        public string St { get; set; }
        public int Cls { get; set; }
        public List<string> Ords { get; set; }
        public string Ym { get; set; }
        public string Memo { get; set; }
    }
}
