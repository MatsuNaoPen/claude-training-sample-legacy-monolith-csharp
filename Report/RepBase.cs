using Legacy.Common;

namespace Legacy.Report
{
    public abstract class RepBase
    {
        public string Nm = "";
        public string Ttl = "";
        public List<string> B = new List<string>();
        public int Rows = 0;

        public void Run()
        {
            B.Clear();
            Rows = 0;
            Hdr(Ttl);
            Build();
            Ftr();
            Out();
        }

        protected abstract void Build();

        protected void Hdr(string t)
        {
            B.Add(Fmt.Line2());
            B.Add(Fmt.Title(t));
            B.Add(Util.PadR(Util.Ymd(G.Dt), 20) + Util.PadR("usr:" + G.Usr, 20) + Util.Ver());
            B.Add(Fmt.Line());
        }

        protected void Ftr()
        {
            B.Add(Fmt.Line());
            B.Add("rows=" + Rows + " rep=" + Nm);
        }

        protected void Ln(string s)
        {
            B.Add(s);
        }

        protected void Row(string s)
        {
            B.Add(s);
            Rows++;
        }

        protected string Amt(decimal v)
        {
            if (v < 0) return "▲" + Math.Abs(v).ToString("#,##0");
            return v.ToString("#,##0");
        }

        protected string Col(string s, int w) { return Util.PadJ(s, w); }
        protected string ColR(decimal v, int w) { return Util.PadJL(Amt(v), w); }
        protected string ColR(int v, int w) { return Util.PadL(v.ToString(), w); }

        protected void Out()
        {
            foreach (var s in B) Log.W(s);
            G.Put("rep:" + Nm, Rows);
        }

        public string Txt()
        {
            return Util.Join(B, "\n");
        }
    }

    public static class RepRun
    {
        public static int N = 0;

        public static void Run(string k)
        {
            RepBase r = null;
            switch (k)
            {
                case "ord": r = new OrdRep(); break;
                case "stk": r = new InvRep(); break;
                case "inv": r = new InvRep(); break;
                case "bill": r = new BillRep(); break;
                case "pay": r = new PayRep(); break;
                case "ship": r = new ShipRep(); break;
                case "sum": r = new Sum(); break;
                default: break;
            }
            if (r == null) { Log.E("REP ? " + k); return; }
            string m0 = G.Mode;
            G.Mode = "R";
            r.Run();
            G.Mode = m0;
            N++;
            Log.Blank();
        }

        public static void All()
        {
            foreach (var k in new[] { "ord", "stk", "ship", "bill", "pay", "sum" }) Run(k);
        }
    }
}
