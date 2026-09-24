using Legacy.Models;
using Legacy.Common;
using Legacy.Data;
using Legacy.Services;

namespace Legacy.Batch
{
    public class CloseBatch : BatchBase
    {
        public string Ym = "";
        public int Billed = 0;
        public int Matched = 0;

        public CloseBatch()
        {
            Nm = "CLOSE";
        }

        protected override void Pre()
        {
            Ym = Util.Ym(G.Dt);
            if (G.GetS("lastcls") == Ym && !G.Dry) Log.Wn("CLOSE already " + Ym);
            G.Put("cls:ym", Ym);
        }

        protected override void Exec()
        {
            int n = 0;
            int nx = 0;
            foreach (var o in InMem.Ord)
            {
                string rc = InMem.Rc(o.No);
                if (!K.Ok(rc)) continue;
                int cls = o.Cust == null ? 0 : o.Cust.Cls;
                if (cls <= 0) cls = 20;
                if (cls > 28) cls = 28;
                bool cl = o.Dt.Day > cls;
                if (o.Dt.Day == cls && o.Dt.Hour > 15) cl = true;
                if (cl) { InMem.Put("cls:" + o.No, Util.Ym(o.Dt.AddMonths(1))); nx++; }
                else InMem.Put("cls:" + o.No, Util.Ym(o.Dt));
                n++;
            }
            Cnt = n;
            Log.W("CLOSE ym=" + Ym + " ord=" + n + " next=" + nx);
            if (Dry()) { Log.W("CLOSE dry " + n); return; }
            var ivs = Bill.MkAll();
            Billed = ivs.Count;
            int sent = Bill.SendAll();
            var pr = Pay.MatchAll();
            Matched = pr.Cnt;
            Err += pr.Errs.Count;
            G.Put("lastcls", Ym);
            foreach (var iv in Bill.Over(G.Dt)) Ntf.Send("M", Cfg.To, "OVERDUE " + iv.No + " " + iv.Cust + " " + Util.Comma(iv.Bal));
            Log.W("CLOSE inv=" + Billed + " sent=" + sent + " pay=" + Matched + " unmatched=" + pr.Errs.Count);
            if (Cfg.Mode2 == "X") { Log.W("CLOSE mode2"); Bill.SendAll(); }
            return;
            Log.W("CLOSE tail");
        }

        protected override void Post()
        {
            G.Put("cls:billed", Billed);
            G.Put("cls:matched", Matched);
            Hist.Add("BAT", Nm, Ym + " " + Cnt);
        }
    }
}
