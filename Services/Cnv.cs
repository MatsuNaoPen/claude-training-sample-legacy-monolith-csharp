using Legacy.Models;
using Legacy.Common;
using Legacy.Data;

namespace Legacy.Services
{
    public static class Cnv
    {
        public static int Err = 0;

        public static Dictionary<string, object> ToDic(object o)
        {
            var d = new Dictionary<string, object>();
            if (o == null) return d;
            if (o is Order)
            {
                var x = (Order)o;
                d["typ"] = "ORD";
                d["no"] = x.No;
                d["cust"] = x.Cust == null ? "" : x.Cust.Cd;
                d["dt"] = x.Dt;
                d["urg"] = x.Urg;
                d["lines"] = x.Lines == null ? 0 : x.Lines.Count;
                d["rc"] = InMem.Rc(x.No);
                d["amt"] = InMem.Amt(x.No);
                return d;
            }
            if (o is Customer)
            {
                var x = (Customer)o;
                d["typ"] = "CUST";
                d["cd"] = x.Cd;
                d["nm"] = x.Nm;
                d["rk"] = x.Rk;
                d["flg"] = x.Flg;
                d["cls"] = x.Cls;
                return d;
            }
            if (o is Item)
            {
                var x = (Item)o;
                d["typ"] = "ITM";
                d["cd"] = x.Cd;
                d["nm"] = x.Nm;
                d["prc"] = x.Prc;
                d["cst"] = x.Cst;
                d["cat"] = x.Cat;
                d["wh"] = x.Wh;
                d["dis"] = x.Dis;
                return d;
            }
            if (o is Invoice)
            {
                var x = (Invoice)o;
                d["typ"] = "INV";
                d["no"] = x.No;
                d["cust"] = x.Cust;
                d["amt"] = x.Amt;
                d["tax"] = x.Tax;
                d["tot"] = x.Tot;
                d["bal"] = x.Bal;
                d["st"] = x.St;
                d["due"] = x.Due;
                return d;
            }
            if (o is Payment)
            {
                var x = (Payment)o;
                d["typ"] = "PAY";
                d["no"] = x.No;
                d["cust"] = x.Cust;
                d["amt"] = x.Amt;
                d["dt"] = x.Dt;
                d["ptyp"] = x.Typ;
                d["st"] = x.St;
                d["inv"] = x.InvNo;
                return d;
            }
            d["typ"] = Util.TypOf(o);
            d["val"] = o;
            return d;
        }

        public static object FromDic(string typ, Dictionary<string, object> d)
        {
            if (d == null) return null;
            try
            {
                switch (typ)
                {
                    case "ORD": return MkOrd(d);
                    case "CUST": return MkCust(d);
                    case "ITM": return MkItm(d);
                    case "STK": return MkStk(d);
                    case "PAY": return MkPay(d);
                    default: break;
                }
            }
            catch
            {
                Err++;
            }
            return null;
        }

        static Order MkOrd(Dictionary<string, object> d)
        {
            var o = new Order();
            o.No = d.S("no");
            o.Cust = InMem.FindCust(d.S("cust"));
            if (o.Cust == null)
            {
                o.Cust = new Customer { Cd = d.S("cust"), Nm = "?", Rk = "C", Flg = false, Cls = 0 };
            }
            o.Dt = d.Dt("dt");
            o.Urg = d.B("urg");
            o.Lines = new List<OrderLine>();
            if (d.Has("itm")) o.Lines.Add(MkLine(d));
            return o;
        }

        static OrderLine MkLine(Dictionary<string, object> d)
        {
            var l = new OrderLine();
            l.Itm = d.S("itm");
            l.Qty = d.I("qty");
            l.Prc = d.D("prc");
            if (l.Prc == 0) l.Prc = Mst.Prc(l.Itm);
            l.Ret = d.B("ret");
            l.Typ = d.S("typ").Or("N");
            return l;
        }

        static Customer MkCust(Dictionary<string, object> d)
        {
            var c = new Customer();
            c.Cd = Util.Fix(d.S("cd"));
            c.Nm = d.S("nm");
            c.Rk = d.S("rk").Or("C");
            c.Flg = d.B("flg");
            c.Cls = d.I("cls");
            return c;
        }

        static Item MkItm(Dictionary<string, object> d)
        {
            var it = new Item();
            it.Cd = Util.Fix(d.S("cd"));
            it.Nm = d.S("nm");
            it.Prc = d.D("prc");
            it.Cst = d.D("cst");
            it.Typ = d.S("ityp").Or("N");
            it.Unit = d.S("unit").Or("PC");
            it.Cat = d.S("cat").Or("N");
            it.Wh = d.S("wh").Or(Cfg.Wh1);
            it.Min = d.I("min");
            it.Dis = d.B("dis");
            return it;
        }

        static Stock MkStk(Dictionary<string, object> d)
        {
            var s = new Stock();
            s.Itm = d.S("itm");
            s.Wh = d.S("wh").Or(Cfg.Wh1);
            s.Qty = d.I("qty");
            s.Rsv = d.I("rsv");
            s.Flg = d.S("flg");
            s.Lot = d.S("lot");
            s.Upd = G.Dt;
            s.Usr = G.Usr;
            return s;
        }

        static Payment MkPay(Dictionary<string, object> d)
        {
            var p = new Payment();
            p.No = d.S("no");
            p.Cust = Util.Fix(d.S("cust"));
            p.Amt = d.D("amt");
            p.Dt = d.Dt("dt");
            p.Typ = d.S("ptyp").Or(d.S("typ")).Or("S");
            p.St = "N";
            p.Memo = d.S("memo");
            return p;
        }

        public static void Apply(object o, Dictionary<string, object> d)
        {
            if (o == null || d == null) return;
            if (o is Customer)
            {
                var c = (Customer)o;
                if (d.Has("nm")) c.Nm = d.S("nm");
                if (d.Has("rk")) c.Rk = d.S("rk");
                if (d.Has("flg")) c.Flg = d.B("flg");
                if (d.Has("cls")) c.Cls = d.I("cls");
                return;
            }
            if (o is Stock)
            {
                var s = (Stock)o;
                if (d.Has("qty")) s.Qty = d.I("qty");
                if (d.Has("flg")) s.Flg = d.S("flg");
                s.Upd = G.Dt;
                return;
            }
            Err++;
        }

        public static Order Csv2Ord(CsvRow r, string[] hdr)
        {
            var d = Csv.ToDic(r, hdr);
            var o = (Order)FromDic("ORD", d);
            return o;
        }

        public static OrderLine Csv2Line(CsvRow r, string[] hdr)
        {
            var d = Csv.ToDic(r, hdr);
            return MkLine(d);
        }

        public static Stock Csv2Stk(string[] cols)
        {
            var s = new Stock();
            try
            {
                s.Itm = cols[0].Trim();
                s.Wh = cols[1].Trim();
                s.Qty = int.Parse(cols[2].Trim());
                s.Flg = "";
                s.Upd = G.Dt;
                s.Usr = G.Usr;
            }
            catch
            {
            }
            return s;
        }

        public static Payment Csv2Pay(CsvRow r, string[] hdr)
        {
            var d = Csv.ToDic(r, hdr);
            return (Payment)FromDic("PAY", d);
        }

        public static Invoice Ord2Inv(Order o, decimal amt)
        {
            var iv = new Invoice();
            iv.No = Util.No("IV");
            iv.Cust = o.Cust == null ? "" : o.Cust.Cd;
            iv.Amt = amt;
            iv.Tax = Util.Tax(amt);
            iv.Tot = iv.Amt + iv.Tax;
            iv.Bal = iv.Tot;
            iv.Dt = G.Dt;
            iv.Due = Util.DueDt(G.Dt, 30);
            iv.St = "N";
            iv.Ords = new List<string> { o.No };
            return iv;
        }

        public static decimal ToD(object o)
        {
            try
            {
                if (o is decimal) return (decimal)o;
                if (o is int) return (int)o;
                return decimal.Parse(o.ToString().Replace(",", ""));
            }
            catch
            {
            }
            return 0;
        }

        public static int ToI(object o)
        {
            try
            {
                if (o is int) return (int)o;
                return int.Parse(o.ToString().Trim());
            }
            catch
            {
            }
            return 0;
        }

        public static DateTime ToDt(object o)
        {
            try
            {
                if (o is DateTime) return (DateTime)o;
                return DateTime.Parse(o.ToString());
            }
            catch
            {
            }
            return G.Dt;
        }

        public static bool ToB(object o)
        {
            return Util.ToBool2(o);
        }
    }
}
