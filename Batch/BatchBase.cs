using Legacy.Models;
using Legacy.Common;
using Legacy.Services;

namespace Legacy.Batch
{
    public abstract class BatchBase
    {
        public string Nm = "";
        public int Cnt = 0;
        public int Err = 0;
        public string Rc = "";
        public Dictionary<string, object> Prm = new Dictionary<string, object>();

        public Rslt Run()
        {
            var r = new Rslt();
            G.Mode = "B";
            G.Put("batch", Nm);
            Log.H("BATCH " + Nm + (G.Dry ? " (DRY)" : ""));
            try
            {
                Pre();
                Exec();
                Post();
                Rc = Err > 0 ? "W" : "OK";
            }
            catch (Exception ex)
            {
                Rc = "NG";
                Log.E(Nm + " " + ex.Message);
            }
            r.Rc = Rc;
            r.Cnt = Cnt;
            r.Ok = Rc != "NG";
            r.Msg = Nm;
            r.Set("err", Err);
            G.LastRc = Rc;
            G.Put("batch:" + Nm, Rc);
            Log.W(Nm + " end rc=" + Rc + " cnt=" + Cnt + " err=" + Err);
            return r;
        }

        protected virtual void Pre()
        {
        }

        protected abstract void Exec();

        protected virtual void Post()
        {
        }

        protected string P(string k)
        {
            if (Prm.ContainsKey(k)) return Util.ToS(Prm[k]);
            return G.GetS(k);
        }

        protected bool Dry()
        {
            return G.Dry || P("dry") == "1";
        }
    }

    public static class NightBatch
    {
        public static List<Rslt> Log2 = new List<Rslt>();

        public static Rslt Run(string flg)
        {
            var r = new Rslt("OK");
            string usr0 = G.Usr;
            G.Mode = "B";
            G.Usr = "BATCH";
            G.Dry = flg == "X";
            if (flg == "F") G.Flg = "X";
            var steps = new List<BatchBase>();
            if (flg != "S") steps.Add(new ImpBatch());
            steps.Add(new RecalcBatch());
            steps.Add(new CloseBatch());
            steps.Add(new ChkBatch());
            foreach (var b in steps)
            {
                var x = b.Run();
                Log2.Add(x);
                if (x.Rc == "NG")
                {
                    r.Rc = "NG";
                    r.Msg = b.Nm;
                    if (flg != "C") break;
                }
                if (x.Rc == "W" && r.Rc == "OK") r.Rc = "W";
                r.Cnt += x.Cnt;
            }
            G.Dry = false;
            G.Flg = "";
            G.Usr = usr0;
            Hist.Add("BAT", "NIGHT", r.Rc + " " + flg);
            Log.W("NIGHT rc=" + r.Rc + " steps=" + Log2.Count + " cnt=" + r.Cnt + " flg=" + Util.Nz2(flg, "-"));
            return r;
        }
    }
}
