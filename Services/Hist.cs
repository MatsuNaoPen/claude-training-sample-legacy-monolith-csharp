using Legacy.Common;
using Legacy.Data;

namespace Legacy.Models
{
    public class Tx
    {
        public int Seq { get; set; }
        public string Kind { get; set; }
        public string Key { get; set; }
        public string Val { get; set; }
        public DateTime Dt { get; set; }
        public string Usr { get; set; }
        public string Memo { get; set; }
    }
}

namespace Legacy.Services
{
    public static class Hist
    {
        static int _n = 0;

        public static void Add(string kind, string key, string val)
        {
            Add(kind, key, val, "");
        }

        public static void Add(string kind, string key, string val, string memo)
        {
            _n++;
            var t = new Models.Tx();
            t.Seq = _n;
            t.Kind = kind;
            t.Key = key;
            t.Val = val;
            t.Dt = G.Dt;
            t.Usr = G.Usr;
            t.Memo = memo;
            InMem.Tx.Add(t);
            Log.D("TX " + kind + " " + key + " " + val);
        }

        public static int Cnt(string kind)
        {
            int n = 0;
            foreach (var t in InMem.Tx) if (kind == "" || t.Kind == kind) n++;
            return n;
        }

        public static void Dump(int n)
        {
            int st = InMem.Tx.Count - n;
            if (st < 0) st = 0;
            for (int i = st; i < InMem.Tx.Count; i++)
            {
                var t = InMem.Tx[i];
                Log.W("  " + t.Seq.ToString("0000") + " " + Util.PadR(t.Kind, 4) + " " + Util.PadR(t.Key, 12) + " " + t.Val + (t.Memo == "" ? "" : " ; " + t.Memo));
            }
        }
    }
}
