using Legacy.Models;
using Legacy.Common;
using Legacy.Data;

namespace Legacy.Services
{
    public static class Tax
    {
        public static decimal Rate(string cat)
        {
            if (cat == "F") return Cfg.GetD("TAX2");
            if (cat == "X") return 0;
            return Cfg.GetD("TAX");
        }

        public static decimal Calc(decimal amt, string cat)
        {
            return Rnd(amt * Rate(cat) / 100);
        }

        public static decimal Rnd(decimal v)
        {
            return Math.Round(v, 0);
        }

        public static decimal Total(Order o)
        {
            decimal t = 0;
            if (o == null || o.Lines == null) return 0;
            foreach (var l in o.Lines)
            {
                if (l.Qty <= 0) continue;
                decimal a = l.Qty * l.Prc;
                if (l.Ret) a = -a;
                if (l.Typ == "S" && !l.Ret) a = a * 0.9m;
                string cat = "N";
                var it = InMem.FindItm(l.Itm);
                if (it != null) cat = it.Cat;
                t += Calc(a, cat);
            }
            return t;
        }

        public static decimal Incl(decimal amt, string cat)
        {
            return amt + Calc(amt, cat);
        }

        public static List<decimal> Split(decimal tax, int n)
        {
            return Util.Div2(tax, n);
        }
    }
}
