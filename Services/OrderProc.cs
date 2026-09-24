using Legacy.Models;

namespace Legacy.Services
{
    public class OrderProc
    {
        public (decimal amt, string rc) Proc(Order o)
        {
            if (o == null) return (0, "E0");
            if (o.Cust == null) return (0, "E1");
            if (o.Lines == null || o.Lines.Count == 0) return (0, "E2");

            decimal sub = 0;
            int cnt = 0;
            bool r1 = false;
            bool r2 = false;
            foreach (var l in o.Lines)
            {
                if (l.Qty == 0)
                {
                    continue;
                }
                if (l.Qty < 0)
                {
                    return (0, "E3");
                }
                decimal tmp = l.Qty * l.Prc;
                if (l.Ret)
                {
                    tmp = -tmp;
                    r1 = true;
                }
                else
                {
                    cnt += l.Qty;
                }
                if (l.Typ == "S" && !l.Ret)
                {
                    tmp = tmp * 0.9m;
                }
                sub += tmp;
            }

            if (sub < 0)
            {
                return (0, "R1");
            }
            if (r1 && o.Cust.Rk == "C")
            {
                r2 = true;
            }

            decimal d = Disc.Calc(o.Cust, sub, cnt);
            decimal s = Ship.Calc(o, sub, cnt);
            decimal amt = sub - d + s;

            string rc = "OK";
            int cls = o.Cust.Cls;
            if (cls <= 0) cls = 20;
            if (o.Dt.Day > cls)
            {
                rc = "NX";
                if (o.Urg) rc = "NU";
            }
            else
            {
                if (o.Dt.Day == cls && o.Dt.Hour >= 15) rc = "NX";
            }
            if (r2) rc = "RV";

            amt = Disc.Rnd(amt, o.Cust);
            if (amt < 0)
            {
                amt = 0;
                rc = "Z0";
            }
            if (amt > 1000000 && o.Cust.Rk != "A")
            {
                rc = "HL";
            }
            return (amt, rc);
        }
    }
}
