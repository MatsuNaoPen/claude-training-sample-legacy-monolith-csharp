using Legacy.Models;
using Legacy.Common;

namespace Legacy.Data
{
    public static class InMem
    {
        public static List<Customer> Cust = new List<Customer>();
        public static List<Item> Itm = new List<Item>();
        public static List<Warehouse> Wh = new List<Warehouse>();
        public static List<Stock> Stk = new List<Stock>();
        public static List<Order> Ord = new List<Order>();
        public static List<Shipment> Shp = new List<Shipment>();
        public static List<Invoice> Inv = new List<Invoice>();
        public static List<Payment> Pay = new List<Payment>();
        public static List<User> Usr = new List<User>();
        public static List<Tx> Tx = new List<Tx>();
        public static List<Alloc> Alc = new List<Alloc>();
        public static Dictionary<string, object> Kv = new Dictionary<string, object>();
        public static bool Init = false;
        public static int Ver = 0;

        public static void Clear()
        {
            Cust.Clear();
            Itm.Clear();
            Wh.Clear();
            Stk.Clear();
            Ord.Clear();
            Shp.Clear();
            Inv.Clear();
            Pay.Clear();
            Usr.Clear();
            Tx.Clear();
            Alc.Clear();
            Kv.Clear();
            Init = false;
            Ver++;
        }

        public static Customer FindCust(string cd)
        {
            if (cd == null) return null;
            foreach (var c in Cust) if (c.Cd == cd) return c;
            return null;
        }

        public static Item FindItm(string cd)
        {
            if (cd == null) return null;
            foreach (var i in Itm) if (i.Cd == cd) return i;
            return null;
        }

        public static Warehouse FindWh(string cd)
        {
            if (cd == null) return null;
            foreach (var w in Wh) if (w.Cd == cd) return w;
            return null;
        }

        public static Stock FindStk(string itm, string wh)
        {
            foreach (var s in Stk) if (s.Itm == itm && s.Wh == wh) return s;
            return null;
        }

        public static List<Stock> StkOf(string itm)
        {
            var r = new List<Stock>();
            foreach (var s in Stk) if (s.Itm == itm) r.Add(s);
            return r;
        }

        public static Order FindOrd(string no)
        {
            if (no == null) return null;
            foreach (var o in Ord) if (o.No == no) return o;
            return null;
        }

        public static Shipment FindShp(string no)
        {
            foreach (var s in Shp) if (s.No == no) return s;
            return null;
        }

        public static Shipment ShpOfOrd(string ordNo)
        {
            foreach (var s in Shp) if (s.OrdNo == ordNo && s.St != "X") return s;
            return null;
        }

        public static Invoice FindInv(string no)
        {
            foreach (var i in Inv) if (i.No == no) return i;
            return null;
        }

        public static Payment FindPay(string no)
        {
            foreach (var p in Pay) if (p.No == no) return p;
            return null;
        }

        public static List<Order> OrdOf(string cust)
        {
            var r = new List<Order>();
            foreach (var o in Ord) if (o.Cust != null && o.Cust.Cd == cust) r.Add(o);
            return r;
        }

        public static List<Invoice> InvOf(string cust)
        {
            var r = new List<Invoice>();
            foreach (var i in Inv) if (i.Cust == cust) r.Add(i);
            return r;
        }

        public static List<Alloc> AlcOf(string ordNo)
        {
            var r = new List<Alloc>();
            foreach (var a in Alc) if (a.OrdNo == ordNo) r.Add(a);
            return r;
        }

        public static void Add(string tbl, object o)
        {
            if (o == null) return;
            switch (tbl)
            {
                case "CUST": Cust.Add((Customer)o); break;
                case "ITM": Itm.Add((Item)o); break;
                case "WH": Wh.Add((Warehouse)o); break;
                case "STK": Stk.Add((Stock)o); break;
                case "ORD": Ord.Add((Order)o); break;
                case "SHP": Shp.Add((Shipment)o); break;
                case "INV": Inv.Add((Invoice)o); break;
                case "PAY": Pay.Add((Payment)o); break;
                case "USR": Usr.Add((User)o); break;
                case "TX": Tx.Add((Tx)o); break;
                case "ALC": Alc.Add((Alloc)o); break;
                default: G.Err++; break;
            }
            Ver++;
        }

        public static void Put(string k, object v)
        {
            Kv[k] = v;
        }

        public static object Get(string k)
        {
            if (Kv.ContainsKey(k)) return Kv[k];
            return null;
        }

        public static string GetS(string k)
        {
            var v = Get(k);
            if (v == null) return "";
            return v.ToString();
        }

        public static decimal GetD(string k)
        {
            var v = Get(k);
            if (v == null) return 0;
            if (v is decimal) return (decimal)v;
            decimal d;
            if (decimal.TryParse(v.ToString(), out d)) return d;
            return 0;
        }

        public static string Rc(string no)
        {
            return GetS("rc:" + no);
        }

        public static decimal Amt(string no)
        {
            return GetD("amt:" + no);
        }

        public static bool Billed(string no)
        {
            return GetS("inv:" + no) != "";
        }
    }
}
