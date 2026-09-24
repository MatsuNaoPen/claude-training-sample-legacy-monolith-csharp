namespace Legacy.Common
{
    public static class Fmt
    {
        public static int W = 72;

        public static string Num(decimal v)
        {
            if (v < 0) return "(" + Math.Abs(v).ToString("#,##0") + ")";
            return v.ToString("#,##0");
        }

        public static string Dt(DateTime d) { return d.ToString("yyyy/MM/dd"); }
        public static string Col(string s, int w) { return Util.PadJ(s, w); }
        public static string ColR(decimal v, int w) { return Util.PadJL(Num(v), w); }
        public static string Line() { return new string('-', W); }
        public static string Line2() { return new string('=', W); }

        public static string Title(string t)
        {
            int n = (W - Util.Wd(t)) / 2;
            if (n < 0) n = 0;
            return new string(' ', n) + t;
        }

    }
}
