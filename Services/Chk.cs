using Legacy.Models;
using Legacy.Common;
using Legacy.Data;

namespace Legacy.Services
{
    public static class Chk
    {
        public static int N = 0;

        public static List<string> Ord(Order o)
        {
            N++;
            var errs = new List<string>();
            if (o == null) { errs.Add("NULL"); return errs; }
            if (o.No.IsNul()) errs.Add("NO:REQ");
            else if (!Util.ChkCd(o.No, Cfg.Pfx)) errs.Add("NO:FMT " + o.No);
            if (o.Cust == null) errs.Add("CUST:NULL");
            else
            {
                if (InMem.Init && InMem.FindCust(o.Cust.Cd) == null) errs.Add("CUST:NF " + o.Cust.Cd);
                if (!K.IsRk(o.Cust.Rk)) G.Msg("RK? " + o.Cust.Cd + " " + o.Cust.Rk);
            }
            if (o.Lines == null || o.Lines.Count == 0) { errs.Add("LINES:0"); return errs; }
            int i = 0;
            foreach (var l in o.Lines)
            {
                i++;
                Line(l, i, errs);
            }
            if (o.Dt > G.Dt.AddDays(30)) errs.Add("DT:FUTURE");
            if (o.Dt.Year < 2000) errs.Add("DT:OLD");
            return errs;
        }

        static void Line(OrderLine l, int i, List<string> errs)
        {
            string nm = "L" + i;
            if (l == null) { errs.Add(nm + ":NULL"); return; }
            if (l.Itm.IsNul()) { errs.Add(nm + ":ITM"); return; }
            if (InMem.Init)
            {
                var it = InMem.FindItm(l.Itm);
                if (it == null) errs.Add(nm + ":ITM NF " + l.Itm);
                else if (it.Dis) errs.Add(nm + ":ITM DIS " + l.Itm);
            }
            if (l.Qty < 0) errs.Add(nm + ":QTY " + l.Qty);
            if (l.Qty > K.MAXQ) errs.Add(nm + ":QTY>MAX");
            if (l.Prc < 0) errs.Add(nm + ":PRC");
            if (!l.Typ.In("N", "S", "F", "", null)) errs.Add(nm + ":TYP " + l.Typ);
        }

        public static List<string> Cust(Customer c)
        {
            N++;
            var errs = Util.ChkCust(c);
            if (c == null) return errs;
            if (c.Cd.IsX() && c.Rk == "C") errs.Add("X-CUST RK C");
            return errs;
        }

        public static List<string> Itm(Item it)
        {
            N++;
            var errs = Util.ChkItm(it);
            if (it == null) return errs;
            if (!it.Typ.In("N", "S", "F")) errs.Add("TYP:" + it.Typ);
            if (it.Typ == "S" && it.Cat == "F") errs.Add("SET+FOOD");
            if (InMem.Init && InMem.FindWh(it.Wh) == null) errs.Add("WH:NF " + it.Wh);
            return errs;
        }

        public static List<string> Pay(Payment p)
        {
            N++;
            var errs = new List<string>();
            if (p == null) { errs.Add("NULL"); return errs; }
            Util.ChkReq(p.No, "NO", errs);
            Util.ChkReq(p.Cust, "CUST", errs);
            if (p.Amt <= 0) errs.Add("AMT<=0");
            if (!p.Typ.In("S", "C", "N")) errs.Add("TYP:" + p.Typ);
            if (p.Dt > G.Dt) errs.Add("DT:FUTURE");
            return errs;
        }

        public static List<string> Inv(Invoice iv)
        {
            N++;
            var errs = new List<string>();
            if (iv == null) { errs.Add("NULL"); return errs; }
            if (iv.Tot != iv.Amt + iv.Tax) errs.Add("TOT!=AMT+TAX");
            if (iv.Bal > iv.Tot) errs.Add("BAL>TOT");
            if (iv.Bal < 0) errs.Add("BAL<0");
            if (iv.Due < iv.Dt) errs.Add("DUE<DT");
            if (iv.Ords == null || iv.Ords.Count == 0) errs.Add("ORDS:0");
            return errs;
        }

        public static bool Ok(List<string> e)
        {
            return e == null || e.Count == 0;
        }

        public static string Join(List<string> e)
        {
            return Util.Join(e, ";");
        }
    }
}
