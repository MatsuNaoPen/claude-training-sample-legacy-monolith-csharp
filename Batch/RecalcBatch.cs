using Legacy.Models;
using Legacy.Common;
using Legacy.Data;
using Legacy.Services;

namespace Legacy.Batch
{
    public class RecalcBatch : BatchBase
    {
        public int Diff = 0;
        public int InvDiff = 0;

        public RecalcBatch()
        {
            Nm = "RECALC";
        }

        protected override void Exec()
        {
            var p = new OrderProc();
            foreach (var o in InMem.Ord)
            {
                string rc0 = InMem.Rc(o.No);
                if (rc0 == "NG" || rc0 == "CX") continue;
                decimal a0 = InMem.Amt(o.No);
                var r = p.Proc(o);
                decimal a1 = r.amt;
                if (o.Cust != null && o.Cust.Rk == "B") a1 = Math.Floor(a1);
                string rc1 = r.rc;
                if (rc0 == "BO" && K.Ok(rc1)) rc1 = "BO";
                if (a0 != a1 || rc0 != rc1)
                {
                    Diff++;
                    Log.W("DIFF " + o.No + " " + rc0 + "/" + Util.Comma(a0) + " -> " + rc1 + "/" + Util.Comma(a1));
                    if (!Dry()) { InMem.Put("rc:" + o.No, rc1); InMem.Put("amt:" + o.No, a1); }
                    Hist.Add("RECALC", o.No, rc0 + ">" + rc1);
                }
                Cnt++;
            }
            if (!Dry()) Inv.ReRsv();
            ReBill();
        }

        void ReBill()
        {
            foreach (var iv in InMem.Inv)
            {
                if (iv.St == "X" || iv.St == "P") continue;
                if (iv.Ords == null) continue;
                decimal amt = 0;
                foreach (var no in iv.Ords) amt += InMem.Amt(no);
                if (amt == iv.Amt) continue;
                InvDiff++;
                decimal paid = iv.Tot - iv.Bal;
                Log.W("DIFF " + iv.No + " amt " + Util.Comma(iv.Amt) + " -> " + Util.Comma(amt));
                if (Dry()) continue;
                iv.Amt = amt;
                iv.Tot = Math.Floor(iv.Amt + iv.Tax);
                iv.Bal = iv.Tot - paid;
                if (iv.Bal < 0) iv.Bal = 0;
                Hist.Add("RECALC", iv.No, Util.Comma(amt));
            }
        }

        protected override void Post()
        {
            G.Put("recalc:diff", Diff);
            G.Put("recalc:inv", InvDiff);
            if (Diff > 0) Ntf.Alert("RECALC diff=" + Diff);
            Hist.Add("BAT", Nm, Cnt + "/" + Diff);
        }
    }
}
