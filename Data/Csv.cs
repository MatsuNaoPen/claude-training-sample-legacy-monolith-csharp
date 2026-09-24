using System.Text;
using Legacy.Common;

namespace Legacy.Models
{
    public class CsvRow
    {
        public int Ln { get; set; }
        public string Src { get; set; }
        public string[] Cols { get; set; }
        public string Flg { get; set; }
        public string Err { get; set; }
        public Dictionary<string, object> Dic { get; set; }
    }
}

namespace Legacy.Data
{
    using Legacy.Models;

    public static class Csv
    {
        public static char SEP = ',';
        public static int Skipped = 0;
        public static int Parsed = 0;

        public static List<CsvRow> Parse(string txt, string src)
        {
            var rows = new List<CsvRow>();
            if (txt == null) return rows;
            var lines = txt.Replace("\r\n", "\n").Split('\n');
            int ln = 0;
            foreach (var line in lines)
            {
                ln++;
                var r = new CsvRow();
                r.Ln = ln;
                r.Src = src;
                r.Flg = "";
                if (line.Trim() == "")
                {
                    r.Flg = "X";
                    Skipped++;
                    rows.Add(r);
                    continue;
                }
                if (line.StartsWith("#"))
                {
                    r.Flg = "X";
                    Skipped++;
                    rows.Add(r);
                    continue;
                }
                r.Cols = Split(line);
                if (ln == 1) r.Flg = "H";
                Parsed++;
                rows.Add(r);
            }
            return rows;
        }

        public static string[] Split(string line)
        {
            var r = new List<string>();
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
                else if (c == SEP && !q)
                {
                    r.Add(sb.ToString().Trim());
                    sb.Clear();
                }
                else sb.Append(c);
            }
            r.Add(sb.ToString().Trim());
            return r.ToArray();
        }

        public static string[] Split2(string line)
        {
            var a = line.Split(SEP);
            for (int i = 0; i < a.Length; i++) a[i] = a[i].Trim().Trim('"');
            return a;
        }

        public static string Get(CsvRow r, int i)
        {
            if (r == null || r.Cols == null) return "";
            if (i < 0 || i >= r.Cols.Length) return "";
            return r.Cols[i];
        }

        public static string Get(CsvRow r, string[] hdr, string nm)
        {
            int i = Idx(hdr, nm);
            if (i < 0) return "";
            return Get(r, i);
        }

        public static string[] Hdr(List<CsvRow> rows)
        {
            foreach (var r in rows) if (r.Flg == "H") return r.Cols;
            return new string[0];
        }

        public static int Idx(string[] hdr, string nm)
        {
            if (hdr == null) return -1;
            for (int i = 0; i < hdr.Length; i++) if (hdr[i].ToLowerInvariant() == nm.ToLowerInvariant()) return i;
            return -1;
        }

        public static List<CsvRow> Body(List<CsvRow> rows)
        {
            var r = new List<CsvRow>();
            foreach (var x in rows)
            {
                if (x.Flg == "H") continue;
                if (x.Flg == "X") continue;
                r.Add(x);
            }
            return r;
        }

        public static Dictionary<string, object> ToDic(CsvRow r, string[] hdr)
        {
            var d = new Dictionary<string, object>();
            if (r == null || r.Cols == null) return d;
            for (int i = 0; i < hdr.Length; i++) d[hdr[i]] = Get(r, i);
            r.Dic = d;
            return d;
        }
    }
}
