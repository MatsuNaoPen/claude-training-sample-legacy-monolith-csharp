using Legacy.Models;
using Legacy.Common;
using Legacy.Data;
using Legacy.Services;

namespace Legacy.Report
{
    public class InvRep : RepBase
    {
        public InvRep()
        {
            Nm = "STK";
            Ttl = "在庫一覧";
        }

        protected override void Build()
        {
            foreach (var w in InMem.Wh)
            {
                Ln("[" + w.Cd + "] " + w.Nm + " " + K.WhTyp(w.Typ) + " pri=" + w.Pri);
                Ln("  " + Col("ITM", 6) + Col("NAME", 14) + Util.PadL("QTY", 6) + Util.PadL("RSV", 6) + Util.PadL("AVL", 6) + " F " + Col("LOT", 8) + "UPD");
                int tq = 0;
                int tr = 0;
                foreach (var s in Inv.ByWh(w.Cd))
                {
                    var it = InMem.FindItm(s.Itm);
                    string nm = it == null ? "?" : it.Nm;
                    int avl = s.Qty - s.Rsv;
                    string mk = " ";
                    if (s.Flg == "X") mk = "X";
                    else if (it != null && avl < it.Min) mk = "!";
                    Row("  " + Col(s.Itm, 6) + Col(nm, 14) + ColR(s.Qty, 6) + ColR(s.Rsv, 6) + ColR(avl, 6) + " " + mk + " " + Col(s.Lot, 8) + Util.Ymd(s.Upd));
                    tq += s.Qty;
                    tr += s.Rsv;
                }
                Ln("  " + Util.PadR("sub", 20) + ColR(tq, 6) + ColR(tr, 6) + ColR(tq - tr, 6));
            }
            Ln(Fmt.Line());
            int bo = 0;
            foreach (var a in InMem.Alc)
            {
                if (a.St != "B") continue;
                Ln("BO " + a.OrdNo + " " + a.Itm + " " + a.Qty);
                bo++;
            }
            Ln("bo=" + bo + " alloc=" + InMem.Alc.Count + " rsvd=" + Inv.Rsvd + " short=" + Inv.Short);
            foreach (var l in Inv.Low()) Ln("LOW " + l);
        }
    }
}
