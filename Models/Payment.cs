namespace Legacy.Models
{
    public class Payment
    {
        public string No { get; set; }
        public string Cust { get; set; }
        public decimal Amt { get; set; }
        public decimal Fee { get; set; }
        public DateTime Dt { get; set; }
        public string Typ { get; set; }
        public string St { get; set; }
        public string InvNo { get; set; }
        public string Memo { get; set; }
        public decimal Rem { get; set; }
    }
}
