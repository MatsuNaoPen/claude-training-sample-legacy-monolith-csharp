using Legacy.Models;
using Legacy.Common;
using Legacy.Data;

namespace Legacy.Services
{
    public static class Bill
    {
        public static int Made = 0;

        public static Invoice Mk(Customer c, List<Order> ords)
        {
            if (c == null || ords == null || ords.Count == 0) return null;
            var iv = new Invoice();
            iv.No = G.NextNo("IV");
            iv.Cust = c.Cd;
            iv.Dt = G.Dt;
            iv.Ym = Util.Ym(G.Dt);
            iv.St = "N";
            iv.Cls = c.Cls;
            iv.Ords = new List<string>();
            iv.Memo = "";
            decimal amt = 0;
            decimal tax = 0;
            foreach (var o in ords)
            {
                amt += InMem.Amt(o.No);
                tax += Tax.Total(o);
                iv.Ords.Add(o.No);
                InMem.Put("inv:" + o.No, iv.No);
            }
            iv.Amt = amt;
            iv.Tax = Rnd0(tax);
            iv.Tot = iv.Amt + iv.Tax;
            iv.Bal = iv.Tot;
            iv.Due = DueDt(c);
            InMem.Inv.Add(iv);
            Made++;
            Hist.Add("INV", iv.No, c.Cd + "/" + iv.Tot);
            Log.W("INV " + iv.No + " " + Util.PadJ(c.Cd, 6) + " " + Util.PadL(Util.Comma(iv.Amt), 10) + " " + Util.PadL(Util.Comma(iv.Tax), 8) + " " + Util.PadL(Util.Comma(iv.Tot), 10) + " due=" + Util.Ymd(iv.Due) + " ords=" + iv.Ords.Count);
            return iv;
        }

        public static bool IsCls(Order o)
        {
            int cls = o.Cust == null ? 0 : o.Cust.Cls;
            if (cls <= 0) cls = Cfg.GetI("CLS");
            if (o.Dt.Day > cls) return true;
            if (o.Dt.Day == cls && o.Dt.Hour >= 17) return true;
            return false;
        }

        public static DateTime DueDt(Customer c)
        {
            int days = Cfg.GetI("DUE");
            if (c.Rk == "A") days += 15;
            if (c.Flg) days += 5;
            var d = Util.EomDt(G.Dt).AddDays(days);
            return Util.NextBiz(d);
        }

        public static decimal Rnd0(decimal v)
        {
            return Math.Round(v, 0, MidpointRounding.AwayFromZero);
        }

        public static List<Invoice> MkAll()
        {
            var r = new List<Invoice>();
            var grp = new Dictionary<string, List<Order>>();
            var keys = new List<string>();
            foreach (var o in InMem.Ord)
            {
                if (o.Cust == null) continue;
                string rc = InMem.Rc(o.No);
                if (!K.Ok(rc)) continue;
                if (InMem.Billed(o.No)) continue;
                if (IsCls(o) && G.Mode != "B" && Util.Sw("cls")) continue;
                if (!grp.ContainsKey(o.Cust.Cd)) { grp[o.Cust.Cd] = new List<Order>(); keys.Add(o.Cust.Cd); }
                grp[o.Cust.Cd].Add(o);
            }
            foreach (var k in keys)
            {
                var c = InMem.FindCust(k);
                if (c == null) c = grp[k][0].Cust;
                var iv = Mk(c, grp[k]);
                if (iv != null) r.Add(iv);
            }
            G.Put("billed", r.Count);
            G.LastRc = r.Count > 0 ? "OK" : "NONE";
            return r;
        }

        public static void Send(Invoice iv)
        {
            if (iv == null) return;
            if (iv.St != "N") return;
            iv.St = "S";
            Ntf.Send("M", Cfg.To, "INV " + iv.No + " " + iv.Cust + " " + Util.Comma(iv.Tot) + " due " + Util.Ymd(iv.Due));
            Hist.Add("INV", iv.No, "SEND");
        }

        public static int SendAll()
        {
            int n = 0;
            foreach (var iv in InMem.Inv)
            {
                if (iv.St != "N") continue;
                Send(iv);
                n++;
            }
            return n;
        }

        public static decimal Bal(string cust)
        {
            decimal t = 0;
            foreach (var iv in InMem.InvOf(cust))
            {
                if (iv.St == "X") continue;
                t += iv.Bal;
            }
            return t;
        }

        public static List<Invoice> Open(string cust)
        {
            var r = new List<Invoice>();
            foreach (var iv in InMem.InvOf(cust))
            {
                if (iv.St == "X") continue;
                if (iv.St == "P") continue;
                if (iv.Bal <= 0) continue;
                r.Add(iv);
            }
            return r;
        }

        public static decimal Tot()
        {
            decimal t = 0;
            foreach (var iv in InMem.Inv)
            {
                if (iv.St == "X") continue;
                t += iv.Tot;
            }
            return t;
        }

        public static List<Invoice> Over(DateTime d)
        {
            var r = new List<Invoice>();
            foreach (var iv in InMem.Inv)
            {
                if (iv.St == "X" || iv.St == "P") continue;
                if (iv.Due < d) r.Add(iv);
            }
            return r;
        }
    }
}
