namespace Legacy.Common
{
    public static class Ext
    {
        public static bool IsNul(this string s) { return s == null || s.Trim() == ""; }
        public static string Or(this string s, string d) { return s.IsNul() ? d : s; }

        public static decimal ToDec(this string s)
        {
            decimal v;
            if (decimal.TryParse((s ?? "").Replace(",", ""), out v)) return v;
            return 0;
        }

        public static int ToInt(this string s)
        {
            int v;
            if (int.TryParse((s ?? "").Trim(), out v)) return v;
            return 0;
        }

        public static string ToMoney(this decimal v)
        {
            if (v < 0) return "-" + Math.Abs(v).ToString("#,##0");
            return v.ToString("#,##0");
        }

        public static string Yen(this decimal v) { return "\\" + v.ToString("#,##0"); }

        public static bool Eq(this string a, string b)
        {
            return string.Equals(a ?? "", b ?? "", StringComparison.OrdinalIgnoreCase);
        }

        public static bool In(this string s, params string[] vals)
        {
            foreach (var v in vals) if (s == v) return true;
            return false;
        }

        public static bool IsX(this string s) { return s != null && s.StartsWith("X"); }

        public static bool Has(this Dictionary<string, object> d, string k)
        {
            return d != null && d.ContainsKey(k) && d[k] != null;
        }

        public static string S(this Dictionary<string, object> d, string k)
        {
            if (!d.Has(k)) return "";
            return Util.ToS(d[k]);
        }

        public static decimal D(this Dictionary<string, object> d, string k)
        {
            if (!d.Has(k)) return 0;
            return Util.ToDec2(d[k]);
        }

        public static int I(this Dictionary<string, object> d, string k)
        {
            if (!d.Has(k)) return 0;
            return Util.ToInt2(d[k]);
        }

        public static DateTime Dt(this Dictionary<string, object> d, string k)
        {
            if (!d.Has(k)) return G.Dt;
            return Util.ToDt(d[k]);
        }

        public static bool B(this Dictionary<string, object> d, string k)
        {
            if (!d.Has(k)) return false;
            return Util.ToBool2(d[k]);
        }

        public static string Ymd(this DateTime d) { return d.ToString("yyyy/MM/dd"); }
    }
}
