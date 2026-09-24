namespace Legacy.Models
{
    public class OrderLine
    {
        public string Itm { get; set; }
        public int Qty { get; set; }
        public decimal Prc { get; set; }
        public bool Ret { get; set; }
        public string Typ { get; set; }
    }
}
