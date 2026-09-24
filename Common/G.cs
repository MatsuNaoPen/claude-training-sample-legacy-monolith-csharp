namespace Legacy.Common
{
    public static class G
    {
        public static string Usr = "SYS";
        public static DateTime Dt = new DateTime(2026, 1, 31, 9, 0, 0);
        public static string Mode = "";
        public static object Last;
        public static string LastRc = "";
        public static decimal LastAmt;
        public static string Flg = "";
        public static int Cnt;
        public static int Err;
        public static int Seq = 1000;
        public static bool Dry;
        public static string Wh = "W1";
        public static Dictionary<string, object> Bag = new Dictionary<string, object>();
        public static List<string> Msgs = new List<string>();

        public static string NextNo(string pfx)
        {
            Seq++;
            return pfx + "-" + Seq.ToString("0000");
        }

        public static void Put(string k, object v)
        {
            Bag[k] = v;
        }

        public static object Get(string k)
        {
            if (Bag.ContainsKey(k)) return Bag[k];
            return null;
        }

        public static bool Has(string k)
        {
            return Bag.ContainsKey(k);
        }

        public static string GetS(string k)
        {
            var v = Get(k);
            return v == null ? "" : v.ToString();
        }

        public static int GetI(string k)
        {
            var v = Get(k);
            if (v == null) return 0;
            if (v is int) return (int)v;
            int r;
            if (int.TryParse(v.ToString(), out r)) return r;
            return 0;
        }

        public static decimal GetD(string k)
        {
            var v = Get(k);
            if (v == null) return 0;
            if (v is decimal) return (decimal)v;
            if (v is int) return (int)v;
            decimal r;
            if (decimal.TryParse(v.ToString(), out r)) return r;
            return 0;
        }

        public static void Msg(string s)
        {
            Msgs.Add("[" + Mode + "] " + s);
        }

    }
}
