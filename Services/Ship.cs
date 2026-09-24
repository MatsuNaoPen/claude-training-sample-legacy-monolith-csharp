using Legacy.Models;

namespace Legacy.Services
{
    public static class Ship
    {
        public static decimal Calc(Order o, decimal sub, int cnt)
        {
            decimal s = 0;
            if (sub >= 10000)
            {
                s = 0;
            }
            else
            {
                if (cnt <= 3) s = 800;
                else s = 1200;
            }
            if (o.Urg)
            {
                s += 1500;
            }
            var c = o.Cust;
            if (c.Cd != null && c.Cd.StartsWith("X"))
            {
                s = s * 2;
            }
            if (c.Rk == "A" && !o.Urg)
            {
                s = 0;
            }
            return s;
        }
    }
}
