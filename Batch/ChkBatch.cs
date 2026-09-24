using Legacy.Models;
using Legacy.Common;
using Legacy.Data;
using Legacy.Services;

namespace Legacy.Batch
{
    public class ChkBatch : BatchBase
    {
        public List<string> Errs = new List<string>();

        public ChkBatch()
        {
            Nm = "CHK";
        }

        protected override void Exec()
        {
            foreach (var s in InMem.Stk)
            {
                if (s.Qty < 0) Errs.Add("STK<0 " + s.Itm + "/" + s.Wh + " " + s.Qty);
                if (s.Rsv > s.Qty) Errs.Add("RSV>QTY " + s.Itm + "/" + s.Wh + " " + s.Rsv + ">" + s.Qty);
                if (s.Rsv < 0) Errs.Add("RSV<0 " + s.Itm + "/" + s.Wh + " " + s.Rsv);
            }
            foreach (var iv in InMem.Inv)
            {
                var e = Chk.Inv(iv);
                foreach (var x in e) Errs.Add("INV " + iv.No + " " + x);
            }
            foreach (var a in InMem.Alc)
            {
                if (InMem.FindOrd(a.OrdNo) == null) Errs.Add("ALC orphan " + a.OrdNo);
                if (a.St == "B") Errs.Add("ALC short " + a.OrdNo + " " + a.Itm + " " + a.Qty);
            }
            foreach (var p in InMem.Pay)
            {
                if (p.St == "X") Errs.Add("PAY unmatched " + p.No + " " + p.Cust + " " + Util.Comma(p.Amt));
                if (p.St == "P") Errs.Add("PAY partial " + p.No + " rem=" + Util.Comma(p.Rem));
            }
            foreach (var o in InMem.Ord)
            {
                string rc = InMem.Rc(o.No);
                if (rc == "") Errs.Add("ORD no rc " + o.No);
                if (K.Ok(rc) && InMem.GetS("shp:" + o.No) == "" && rc != "BO") Errs.Add("ORD not shipped " + o.No);
                if (rc == "HL" || rc == "RV") Errs.Add("ORD hold " + o.No + " " + rc);
            }
            foreach (var m in Mst.Vfy()) Errs.Add("MST " + m);
            foreach (var l in Inv.Low()) Errs.Add("LOW " + l);
            foreach (var e in Errs) Log.W("  " + e);
            Cnt = Errs.Count;
            Err = 0;
            foreach (var e in Errs) if (e.StartsWith("STK") || e.StartsWith("INV") || e.StartsWith("RSV")) Err++;
        }

        protected override void Post()
        {
            G.Put("chk:cnt", Cnt);
            G.Put("chk:err", Err);
            if (Err > 0) Ntf.Alert("CHK err=" + Err);
        }
    }
}
