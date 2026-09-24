namespace Legacy.Models
{
    public class Stock
    {
        public string Itm { get; set; }
        public string Wh { get; set; }
        public int Qty { get; set; }
        public int Rsv { get; set; }
        public string Flg { get; set; }
        public string Lot { get; set; }
        public DateTime Upd { get; set; }
        public string Usr { get; set; }
    }

    public class Alloc
    {
        public string OrdNo { get; set; }
        public string Itm { get; set; }
        public string Wh { get; set; }
        public int Qty { get; set; }
        public string St { get; set; }
        public DateTime Dt { get; set; }
        public string ShpNo { get; set; }
    }
}
