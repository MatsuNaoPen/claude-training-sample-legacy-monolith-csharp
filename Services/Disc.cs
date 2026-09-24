using Legacy.Models;

namespace Legacy.Services
{
    public static class Disc
    {
        public static decimal Calc(Customer c, decimal sub, int cnt)
        {
            decimal r = 0;
            if (c.Rk == "A")
            {
                if (sub >= 10000) r = sub * 0.05m;
                else r = sub * 0.02m;
                if (cnt >= 10) r += 500;
            }
            else if (c.Rk == "B")
            {
                if (sub >= 30000) r = sub * 0.03m;
                if (c.Flg && cnt >= 5) r += 300;
            }
            else if (c.Rk == "C")
            {
                if (sub >= 100000) r = sub * 0.01m;
            }
            else
            {
                r = 0;
            }
            if (r > sub * 0.1m) r = sub * 0.1m;
            return r;
        }

        public static decimal Rnd(decimal v, Customer c)
        {
            if (c.Rk == "A") return Math.Floor(v);
            if (c.Rk == "B") return Math.Round(v, 0, MidpointRounding.AwayFromZero);
            return Math.Ceiling(v);
        }
    }
}
