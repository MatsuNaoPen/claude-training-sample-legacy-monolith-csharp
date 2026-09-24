namespace Legacy.Models
{
    public class Shipment
    {
        public string No { get; set; }
        public string OrdNo { get; set; }
        public string Cust { get; set; }
        public string Wh { get; set; }
        public string St { get; set; }
        public DateTime Dt { get; set; }
        public decimal Fee { get; set; }
        public bool Urg { get; set; }
        public string Trk { get; set; }
        public List<ShipLine> Lines { get; set; }
        public string Memo { get; set; }
    }

    public class ShipLine
    {
        public string Itm { get; set; }
        public string Wh { get; set; }
        public int Qty { get; set; }
        public string Lot { get; set; }
        public string St { get; set; }
    }
}
