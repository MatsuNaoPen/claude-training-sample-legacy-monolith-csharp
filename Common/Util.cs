using System.Text;
using Legacy.Models;

namespace Legacy.Common
{
    public static class Util
    {
        public static string[] HOL = { "01/01", "01/02", "01/03", "05/03", "05/04", "05/05", "08/13", "08/14", "08/15", "12/29", "12/30", "12/31" };
        public static string[] RK = { "A", "B", "C" };
        public static string[] TYP = { "N", "S", "F" };
        public static string[] FMT = { "yyyy/MM/dd", "yyyy/MM/dd HH:mm", "yyyy/MM/dd HH:mm:ss", "yyyyMMdd", "yyyy-MM-dd", "yyyy-MM-dd HH:mm" };
        public static int MAXLEN = 40;
        public static int W1 = 12;
        public static int W2 = 16;
        static int _seq = 0;

        public static string Ymd(DateTime d) { return d.ToString("yyyy/MM/dd"); }
        public static string Ymd2(DateTime d) { return d.ToString("yyyyMMdd"); }
        public static string Ymdhm(DateTime d) { return d.ToString("yyyy/MM/dd HH:mm"); }
        public static string Ym(DateTime d) { return d.ToString("yyyyMM"); }

        public static DateTime ParseDt(string s)
        {
            if (s == null || s.Trim() == "") return G.Dt;
            s = s.Trim();
            DateTime d;
            foreach (var f in FMT)
            {
                if (DateTime.TryParseExact(s, f, null, System.Globalization.DateTimeStyles.None, out d)) return d;
            }
            if (DateTime.TryParse(s, out d)) return d;
            G.Err++;
            return G.Dt;
        }

        public static DateTime ParseDt2(string s)
        {
            if (s == null) return DateTime.MinValue;
            s = s.Trim().Replace(".", "/").Replace("-", "/");
            DateTime d;
            if (DateTime.TryParseExact(s, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out d)) return d;
            if (DateTime.TryParseExact(s, "yyyy/M/d", null, System.Globalization.DateTimeStyles.None, out d)) return d;
            if (s.Length == 8 && IsNum(s))
            {
                try
                {
                    return new DateTime(int.Parse(s.Substring(0, 4)), int.Parse(s.Substring(4, 2)), int.Parse(s.Substring(6, 2)));
                }
                catch
                {
                }
            }
            return DateTime.MinValue;
        }

        public static bool IsDt(string s) { return ParseDt2(s) != DateTime.MinValue; }
        public static DateTime EomDt(DateTime d) { return new DateTime(d.Year, d.Month, DateTime.DaysInMonth(d.Year, d.Month)); }
        public static int DiffD(DateTime a, DateTime b) { return (int)(b.Date - a.Date).TotalDays; }

        public static bool IsHol(DateTime d)
        {
            if (d.DayOfWeek == DayOfWeek.Saturday) return true;
            if (d.DayOfWeek == DayOfWeek.Sunday) return true;
            var md = d.ToString("MM/dd");
            foreach (var h in HOL) if (h == md) return true;
            return false;
        }

        public static DateTime NextBiz(DateTime d)
        {
            var w = d;
            int n = 0;
            while (IsHol(w))
            {
                w = w.AddDays(1);
                n++;
                if (n > 10) break;
            }
            return w;
        }

        public static DateTime PrevBiz(DateTime d)
        {
            var w = d;
            int n = 0;
            while (IsHol(w))
            {
                w = w.AddDays(-1);
                n++;
                if (n > 10) break;
            }
            return w;
        }

        public static string Wareki(DateTime d)
        {
            int y = d.Year;
            if (y >= 2019) return "R" + (y - 2018).ToString("00") + d.ToString(".MM.dd");
            if (y >= 1989) return "H" + (y - 1988).ToString("00") + d.ToString(".MM.dd");
            return "S" + (y - 1925).ToString("00") + d.ToString(".MM.dd");
        }

        public static bool CutOff(DateTime d, int cls)
        {
            if (cls <= 0) cls = 20;
            if (d.Day > cls) return true;
            if (d.Day == cls && d.Hour >= 15) return true;
            return false;
        }

        public static bool CutOff2(DateTime d, int cls)
        {
            if (cls <= 0) cls = Cfg.GetI("CLS");
            int dim = DateTime.DaysInMonth(d.Year, d.Month);
            if (cls > dim) cls = dim;
            return d.Day >= cls;
        }

        public static DateTime ClsDt(DateTime d, int cls)
        {
            if (cls <= 0) cls = 20;
            int dim = DateTime.DaysInMonth(d.Year, d.Month);
            if (cls > dim) cls = dim;
            var c = new DateTime(d.Year, d.Month, cls);
            if (d.Day > cls) c = c.AddMonths(1);
            return c;
        }

        public static DateTime DueDt(DateTime d, int days)
        {
            var w = EomDt(d).AddDays(days);
            return NextBiz(w);
        }

        public static DateTime DueDt2(DateTime d, string rk)
        {
            int days = 30;
            if (rk == "A") days = 60;
            if (rk == "B") days = 45;
            if (rk == "C") days = 30;
            var w = EomDt(d).AddDays(days);
            if (IsHol(w)) w = PrevBiz(w);
            return w;
        }

        public static string Age(DateTime from, DateTime to)
        {
            int d = DiffD(from, to);
            if (d < 0) return "-";
            if (d <= 30) return "0-30";
            if (d <= 60) return "31-60";
            if (d <= 90) return "61-90";
            return "90+";
        }

        public static string Nz(string s) { return s == null ? "" : s; }

        public static string Nz2(string s, string d)
        {
            if (s == null) return d;
            if (s.Trim() == "") return d;
            return s;
        }

        public static string Trim2(string s)
        {
            if (s == null) return "";
            return s.Trim().Replace("　", " ").Trim();
        }

        public static string PadL(string s, int n)
        {
            s = Nz(s);
            if (s.Length >= n) return s;
            return new string(' ', n - s.Length) + s;
        }

        public static string PadR(string s, int n)
        {
            s = Nz(s);
            if (s.Length >= n) return s;
            return s + new string(' ', n - s.Length);
        }

        public static int Wd(string s)
        {
            if (s == null) return 0;
            int w = 0;
            foreach (var c in s)
            {
                if (c > 255) w += 2;
                else w += 1;
            }
            return w;
        }

        public static string CutW(string s, int n)
        {
            s = Nz(s);
            var sb = new StringBuilder();
            int w = 0;
            foreach (var c in s)
            {
                int cw = c > 255 ? 2 : 1;
                if (w + cw > n) break;
                sb.Append(c);
                w += cw;
            }
            if (w < n) sb.Append(new string(' ', n - w));
            return sb.ToString();
        }

        public static string PadJ(string s, int n)
        {
            s = Nz(s);
            int w = Wd(s);
            if (w >= n) return CutW(s, n);
            return s + new string(' ', n - w);
        }

        public static string PadJL(string s, int n)
        {
            s = Nz(s);
            int w = Wd(s);
            if (w >= n) return CutW(s, n);
            return new string(' ', n - w) + s;
        }

        public static string Rep(string s, int n)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < n; i++) sb.Append(s);
            return sb.ToString();
        }

        public static string Cut(string s, int n)
        {
            s = Nz(s);
            if (s.Length <= n) return s;
            return s.Substring(0, n);
        }

        public static string Join(List<string> a, string sep)
        {
            var sb = new StringBuilder();
            int i = 0;
            foreach (var s in a)
            {
                if (i > 0) sb.Append(sep);
                sb.Append(s);
                i++;
            }
            return sb.ToString();
        }

        public static string Quote(string s) { return "\"" + Nz(s).Replace("\"", "\"\"") + "\""; }

        public static string Unq(string s)
        {
            s = Nz(s).Trim();
            if (s.Length >= 2 && s.StartsWith("\"") && s.EndsWith("\"")) s = s.Substring(1, s.Length - 2).Replace("\"\"", "\"");
            return s;
        }

        public static string Rev(string s)
        {
            var a = Nz(s).ToCharArray();
            Array.Reverse(a);
            return new string(a);
        }

        public static bool IsNum(string s)
        {
            if (s == null || s == "") return false;
            foreach (var c in s) if (c < '0' || c > '9') return false;
            return true;
        }

        public static bool IsNum2(string s)
        {
            if (s == null || s == "") return false;
            s = s.Replace(",", "").Replace("-", "");
            foreach (var c in s) if (c < '0' || c > '9') return false;
            return true;
        }

        public static bool IsAlpha(string s)
        {
            if (s == null || s == "") return false;
            foreach (var c in s)
            {
                bool ok = (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
                if (!ok) return false;
            }
            return true;
        }

        public static bool IsCd(string s)
        {
            s = Nz(s);
            if (s.Length < 3) return false;
            if (s[1] != '-') return false;
            if (!IsAlpha(s.Substring(0, 1))) return false;
            return IsNum(s.Substring(2));
        }

        public static bool ChkCd(string s, string pfx)
        {
            s = Nz(s);
            if (!s.StartsWith(pfx)) return false;
            if (s.Length != pfx.Length + 5) return false;
            return IsNum(s.Substring(pfx.Length + 1));
        }

        public static string HanNum(string s)
        {
            var sb = new StringBuilder();
            foreach (var c in Nz(s))
            {
                if (c >= 0xFF10 && c <= 0xFF19) sb.Append((char)(c - 0xFF10 + '0'));
                else if (c == 0xFF0D) sb.Append('-');
                else sb.Append(c);
            }
            return sb.ToString();
        }

        public static string Fix(string s)
        {
            s = HanNum(Trim2(s));
            s = s.Replace("－", "-").Replace("_", "-");
            return s.ToUpperInvariant();
        }

        public static int Hash(string s)
        {
            int h = 7;
            foreach (var c in Nz(s))
            {
                h = h * 31 + c;
                h = h % 1000003;
            }
            return Math.Abs(h);
        }

        public static bool Eq(string a, string b) { return Nz(a).Trim().ToUpperInvariant() == Nz(b).Trim().ToUpperInvariant(); }

        public static decimal Rnd(decimal v) { return Math.Floor(v); }
        public static decimal Rnd2(decimal v) { return Math.Round(v, 0, MidpointRounding.AwayFromZero); }
        public static decimal Rnd3(decimal v) { return Math.Ceiling(v); }

        public static decimal RndBy(decimal v, string m)
        {
            if (m == "F") return Math.Floor(v);
            else if (m == "C") return Math.Ceiling(v);
            else if (m == "R") return Math.Round(v, 0, MidpointRounding.AwayFromZero);
            else if (m == "F") return Math.Floor(v);
            else if (m == "E") return Math.Round(v, 0);
            return Math.Floor(v);
        }

        public static decimal RndRk(decimal v, string rk)
        {
            if (rk == "A") return Math.Floor(v);
            if (rk == "B") return Math.Round(v, 0);
            return Math.Ceiling(v);
        }

        public static decimal Tax(decimal v) { return Math.Floor(v * 10 / 100); }
        public static decimal Tax8(decimal v) { return Math.Floor(v * 8 / 100); }

        public static decimal TaxOf(decimal v, string cat)
        {
            if (cat == "F") return Tax8(v);
            if (cat == "X") return 0;
            return Tax(v);
        }

        public static decimal Incl(decimal v) { return Math.Floor(v * 1.1m); }
        public static string Comma(decimal v) { return v.ToString("#,##0"); }

        public static string Comma2(decimal v)
        {
            bool neg = v < 0;
            long n = (long)Math.Abs(Math.Truncate(v));
            string s = n.ToString();
            var sb = new StringBuilder();
            int c = 0;
            for (int i = s.Length - 1; i >= 0; i--)
            {
                sb.Insert(0, s[i]);
                c++;
                if (c % 3 == 0 && i > 0) sb.Insert(0, ',');
            }
            string r = sb.ToString();
            if (neg) r = "-" + r;
            return r;
        }

        public static string Money(decimal v)
        {
            if (v < 0) return "-\\" + Comma(Math.Abs(v));
            return "\\" + Comma(v);
        }

        public static string Yen(decimal v) { return Comma2(v) + "円"; }

        public static decimal Sum(List<decimal> a)
        {
            decimal s = 0;
            foreach (var v in a) s += v;
            return s;
        }

        public static List<decimal> Div(decimal tot, int n)
        {
            var r = new List<decimal>();
            if (n <= 0) return r;
            decimal each = Math.Floor(tot / n);
            decimal rem = tot - each * n;
            for (int i = 0; i < n; i++)
            {
                if (i == 0) r.Add(each + rem);
                else r.Add(each);
            }
            return r;
        }

        public static List<decimal> Div2(decimal tot, int n)
        {
            var r = new List<decimal>();
            if (n <= 0) return r;
            decimal each = Math.Floor(tot / n);
            decimal rem = tot - each * n;
            for (int i = 0; i < n; i++)
            {
                if (i == n - 1) r.Add(each + rem);
                else r.Add(each);
            }
            return r;
        }

        public static string Neg(decimal v)
        {
            if (v < 0) return "▲" + Comma(Math.Abs(v));
            return Comma(v);
        }

        public static bool InLim(decimal v) { return v <= Cfg.GetD("LIM"); }

        public static string[] SplitCsv(string line)
        {
            var r = new List<string>();
            if (line == null) return r.ToArray();
            var sb = new StringBuilder();
            bool q = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    if (q && i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; }
                    else q = !q;
                }
                else if (c == ',' && !q)
                {
                    r.Add(sb.ToString());
                    sb.Clear();
                }
                else sb.Append(c);
            }
            r.Add(sb.ToString());
            return r.ToArray();
        }

        public static string[] SplitCsv2(string line)
        {
            if (line == null) return new string[0];
            var a = line.Split(',');
            for (int i = 0; i < a.Length; i++) a[i] = a[i].Trim();
            return a;
        }

        public static string[] Csv2(string line, string sep)
        {
            if (line == null) return new string[0];
            var a = line.Split(new string[] { sep }, StringSplitOptions.None);
            for (int i = 0; i < a.Length; i++) a[i] = Unq(a[i]);
            return a;
        }

        public static string ToCsv(params object[] a)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < a.Length; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append(Esc(ToS(a[i])));
            }
            return sb.ToString();
        }

        public static string Esc(string s)
        {
            s = Nz(s);
            if (s.Contains(",") || s.Contains("\"") || s.Contains("\n")) return Quote(s);
            return s;
        }

        public static string[] Hdr(string txt)
        {
            if (txt == null) return new string[0];
            var lines = txt.Replace("\r\n", "\n").Split('\n');
            if (lines.Length == 0) return new string[0];
            return SplitCsv2(lines[0]);
        }

        public static List<string[]> Rows(string txt)
        {
            var r = new List<string[]>();
            if (txt == null) return r;
            var lines = txt.Replace("\r\n", "\n").Split('\n');
            int n = 0;
            foreach (var l in lines)
            {
                n++;
                if (n == 1) continue;
                if (l.Trim() == "") continue;
                if (l.StartsWith("#")) continue;
                r.Add(SplitCsv(l));
            }
            return r;
        }

        public static string Col(string[] r, int i)
        {
            if (r == null) return "";
            if (i < 0 || i >= r.Length) return "";
            return Nz(r[i]).Trim();
        }

        public static string ColS(string[] r, int i) { return Unq(Col(r, i)); }
        public static int ColI(string[] r, int i) { return ToInt(Col(r, i)); }
        public static decimal ColD(string[] r, int i) { return ToDec(Col(r, i)); }
        public static DateTime ColDt(string[] r, int i) { return ParseDt(Col(r, i)); }

        public static void ChkReq(string s, string nm, List<string> errs)
        {
            if (s == null || s.Trim() == "") errs.Add(nm + ":REQ");
        }

        public static void ChkLen(string s, int max, string nm, List<string> errs)
        {
            if (s != null && s.Length > max) errs.Add(nm + ":LEN>" + max);
        }

        public static void ChkNum(string s, string nm, List<string> errs)
        {
            if (!IsNum2(s)) errs.Add(nm + ":NUM");
        }

        public static void ChkRk(string rk, List<string> errs)
        {
            bool ok = false;
            foreach (var r in RK) if (r == rk) ok = true;
            if (!ok) errs.Add("RK:" + Nz(rk));
        }

        public static void ChkQty(int q, string nm, List<string> errs)
        {
            if (q < 0) errs.Add(nm + ":QTY<0");
            if (q > 9999) errs.Add(nm + ":QTY>9999");
        }

        public static void ChkPrc(decimal p, string nm, List<string> errs)
        {
            if (p < 0) errs.Add(nm + ":PRC<0");
            if (p > 9999999) errs.Add(nm + ":PRC>LIM");
        }

        public static void ChkAmt(decimal a, string nm, List<string> errs)
        {
            if (a < 0) errs.Add(nm + ":AMT<0");
            if (!InLim(a)) errs.Add(nm + ":AMT>LIM");
        }

        public static void ChkTyp(string t, string nm, List<string> errs)
        {
            bool ok = false;
            foreach (var x in TYP) if (x == t) ok = true;
            if (!ok) errs.Add(nm + ":TYP=" + Nz(t));
        }

        public static List<string> ChkOrd(Order o)
        {
            var errs = new List<string>();
            if (o == null) { errs.Add("ORD:NULL"); return errs; }
            ChkReq(o.No, "NO", errs);
            if (o.Cust == null) errs.Add("CUST:NULL");
            else
            {
                ChkReq(o.Cust.Cd, "CUST.CD", errs);
                ChkRk(o.Cust.Rk, errs);
            }
            if (o.Lines == null || o.Lines.Count == 0) { errs.Add("LINES:EMPTY"); return errs; }
            int i = 0;
            foreach (var l in o.Lines)
            {
                i++;
                ChkLine(l, i, errs);
            }
            return errs;
        }

        public static void ChkLine(OrderLine l, int idx, List<string> errs)
        {
            string nm = "L" + idx;
            if (l == null) { errs.Add(nm + ":NULL"); return; }
            ChkReq(l.Itm, nm + ".ITM", errs);
            ChkQty(l.Qty, nm, errs);
            ChkPrc(l.Prc, nm, errs);
            ChkTyp(l.Typ, nm, errs);
        }

        public static List<string> ChkCust(Customer c)
        {
            var errs = new List<string>();
            if (c == null) { errs.Add("CUST:NULL"); return errs; }
            ChkReq(c.Cd, "CD", errs);
            ChkReq(c.Nm, "NM", errs);
            ChkLen(c.Nm, MAXLEN, "NM", errs);
            ChkRk(c.Rk, errs);
            if (c.Cls < 0 || c.Cls > 31) errs.Add("CLS:" + c.Cls);
            return errs;
        }

        public static List<string> ChkItm(Item it)
        {
            var errs = new List<string>();
            if (it == null) { errs.Add("ITM:NULL"); return errs; }
            ChkReq(it.Cd, "CD", errs);
            ChkReq(it.Nm, "NM", errs);
            ChkPrc(it.Prc, "PRC", errs);
            if (it.Cst > it.Prc && it.Prc > 0) errs.Add("CST>PRC");
            return errs;
        }

        public static bool Ok(List<string> errs) { return errs == null || errs.Count == 0; }

        public static decimal ToDec(string s)
        {
            decimal v;
            if (s == null) return 0;
            s = s.Trim().Replace(",", "").Replace("\\", "").Replace("円", "");
            if (decimal.TryParse(s, out v)) return v;
            return 0;
        }

        public static decimal ToDec2(object o)
        {
            if (o == null) return 0;
            if (o is decimal) return (decimal)o;
            if (o is int) return (int)o;
            if (o is long) return (long)o;
            if (o is double) return (decimal)(double)o;
            return ToDec(o.ToString());
        }

        public static int ToInt(string s)
        {
            int v;
            if (s == null) return 0;
            s = s.Trim().Replace(",", "");
            if (int.TryParse(s, out v)) return v;
            decimal d;
            if (decimal.TryParse(s, out d)) return (int)d;
            return 0;
        }

        public static int ToInt2(object o)
        {
            if (o == null) return 0;
            if (o is int) return (int)o;
            if (o is decimal) return (int)(decimal)o;
            if (o is bool) return (bool)o ? 1 : 0;
            return ToInt(o.ToString());
        }

        public static bool ToBool(string s)
        {
            s = Nz(s).Trim().ToUpperInvariant();
            if (s == "1") return true;
            if (s == "Y") return true;
            if (s == "T") return true;
            if (s == "TRUE") return true;
            if (s == "ON") return true;
            return false;
        }

        public static bool ToBool2(object o)
        {
            if (o == null) return false;
            if (o is bool) return (bool)o;
            if (o is int) return (int)o != 0;
            return ToBool(o.ToString());
        }

        public static DateTime ToDt(object o)
        {
            if (o == null) return G.Dt;
            if (o is DateTime) return (DateTime)o;
            return ParseDt(o.ToString());
        }

        public static string ToS(object o)
        {
            if (o == null) return "";
            if (o is DateTime) return Ymd((DateTime)o);
            if (o is decimal) return ((decimal)o).ToString("0.##");
            if (o is bool) return (bool)o ? "1" : "0";
            return o.ToString();
        }

        public static object Obj(string typ, string val)
        {
            if (typ == "I") return ToInt(val);
            if (typ == "D") return ToDec(val);
            if (typ == "DT") return ParseDt(val);
            if (typ == "B") return ToBool(val);
            return Nz(val);
        }

        public static string TypOf(object o)
        {
            if (o == null) return "";
            if (o is string) return "S";
            if (o is int) return "I";
            if (o is decimal) return "D";
            if (o is DateTime) return "DT";
            if (o is bool) return "B";
            return "O";
        }

        public static Order Cp(Order o)
        {
            if (o == null) return null;
            var r = new Order();
            r.No = o.No;
            r.Cust = Cp(o.Cust);
            r.Dt = o.Dt;
            r.Urg = o.Urg;
            r.Lines = new List<OrderLine>();
            if (o.Lines != null)
            {
                foreach (var l in o.Lines) r.Lines.Add(Cp(l));
            }
            return r;
        }

        public static Customer Cp(Customer c)
        {
            if (c == null) return null;
            var r = new Customer();
            r.Cd = c.Cd;
            r.Nm = c.Nm;
            r.Rk = c.Rk;
            r.Flg = c.Flg;
            r.Cls = c.Cls;
            return r;
        }

        public static OrderLine Cp(OrderLine l)
        {
            if (l == null) return null;
            var r = new OrderLine();
            r.Itm = l.Itm;
            r.Qty = l.Qty;
            r.Prc = l.Prc;
            r.Ret = l.Ret;
            r.Typ = l.Typ;
            return r;
        }

        public static Dictionary<string, object> Merge(Dictionary<string, object> a, Dictionary<string, object> b)
        {
            var r = new Dictionary<string, object>();
            if (a != null) foreach (var k in a.Keys) r[k] = a[k];
            if (b != null) foreach (var k in b.Keys) r[k] = b[k];
            return r;
        }

        public static List<string> Diff(Dictionary<string, object> a, Dictionary<string, object> b)
        {
            var r = new List<string>();
            if (a == null || b == null) return r;
            foreach (var k in a.Keys)
            {
                if (!b.ContainsKey(k)) { r.Add(k + ":-"); continue; }
                if (ToS(a[k]) != ToS(b[k])) r.Add(k + ":" + ToS(a[k]) + "->" + ToS(b[k]));
            }
            foreach (var k in b.Keys) if (!a.ContainsKey(k)) r.Add(k + ":+");
            return r;
        }

        public static int Seq()
        {
            _seq++;
            return _seq;
        }

        public static string No(string pfx) { return pfx + "-" + Seq().ToString("0000"); }
        public static string Id() { return Ymd2(G.Dt) + "-" + Seq().ToString("000000"); }

        public static string Dump(object o)
        {
            if (o == null) return "(null)";
            if (o is Order) return DumpOrd((Order)o);
            if (o is Dictionary<string, object>) return DumpDic((Dictionary<string, object>)o);
            if (o is Customer) { var c = (Customer)o; return c.Cd + "/" + c.Nm + "/" + c.Rk + "/" + (c.Flg ? "1" : "0") + "/" + c.Cls; }
            return o.ToString();
        }

        public static string DumpDic(Dictionary<string, object> d)
        {
            var sb = new StringBuilder();
            sb.Append("{");
            int i = 0;
            foreach (var k in d.Keys)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(k).Append("=").Append(ToS(d[k]));
                i++;
            }
            sb.Append("}");
            return sb.ToString();
        }

        public static string DumpOrd(Order o)
        {
            if (o == null) return "(null)";
            var sb = new StringBuilder();
            sb.Append(o.No).Append(" ");
            sb.Append(o.Cust == null ? "-" : o.Cust.Cd).Append(" ");
            sb.Append(Ymdhm(o.Dt)).Append(" ");
            sb.Append(o.Urg ? "U" : "-").Append(" [");
            if (o.Lines != null)
            {
                int i = 0;
                foreach (var l in o.Lines)
                {
                    if (i > 0) sb.Append("; ");
                    sb.Append(l.Itm).Append("x").Append(l.Qty).Append("@").Append(l.Prc);
                    if (l.Ret) sb.Append("R");
                    if (l.Typ == "S") sb.Append("S");
                    i++;
                }
            }
            sb.Append("]");
            return sb.ToString();
        }

        public static decimal OldCalc(Order o)
        {
            decimal sub = 0;
            foreach (var l in o.Lines)
            {
                if (l.Qty <= 0) continue;
                decimal t = l.Qty * l.Prc;
                if (l.Ret) t = -t;
                if (l.Typ == "S") t = t * 0.95m;
                sub += t;
            }
            if (o.Cust.Rk == "A") sub = sub * 0.97m;
            if (o.Cust.Rk == "B" && sub > 50000) sub = sub - 1000;
            if (o.Urg) sub += 1000;
            return OldRnd(sub);
        }

        public static decimal OldRnd(decimal v) { return Math.Round(v, 0, MidpointRounding.ToEven); }
        public static decimal OldTax(decimal v) { return Math.Floor(v * 0.08m); }

        public static bool Sw(string k)
        {
            if (Cfg.Mode2 == "X") return true;
            if (G.Bag.ContainsKey("sw:" + k)) return ToBool2(G.Bag["sw:" + k]);
            return false;
        }

        public static string St(string s)
        {
            if (s == "N") return "新規";
            if (s == "P") return "処理中";
            if (s == "S") return "完了";
            if (s == "X") return "取消";
            if (s == "M") return "消込済";
            if (s == "R") return "引当";
            if (s == "B") return "欠品";
            return s;
        }

        public static string RkNm(string rk)
        {
            if (rk == "A") return "特約";
            if (rk == "B") return "一般";
            if (rk == "C") return "新規";
            return "他";
        }

        public static string Ln() { return Rep("-", 60); }
        public static string Ln2() { return Rep("=", 60); }
        public static bool IsX(string s) { return s != null && s.StartsWith("X"); }
        public static bool IsDummy(string s) { return Nz(s).StartsWith(Cfg.Pfx); }

        public static string Ver() { return "LEGACY " + Cfg.Ver + " " + Cfg.Mode; }
    }
}
