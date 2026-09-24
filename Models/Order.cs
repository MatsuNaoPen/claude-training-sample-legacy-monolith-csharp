namespace Legacy.Models
{
    public class Order
    {
        public string No { get; set; }
        public Customer Cust { get; set; }
        public List<OrderLine> Lines { get; set; }
        public DateTime Dt { get; set; }
        public bool Urg { get; set; }
    }
}
