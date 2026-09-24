using Legacy.Models;
using Legacy.Common;
using Legacy.Data;

namespace Legacy.Services
{
    public static class Pay
    {
        public static int Matched = 0;

        public static decimal Tol()
        {
            if (G.Has("tol")) return G.GetD("tol");
            return Cfg.GetD("TOL");
        }

        public static Rslt Match(Payment p)
        {
            var r = new Rslt("OK");
            if (p == null) { r.Rc = "E"; return r; }
            if (p.St == "M") { r.Rc = "DUP"; return r; }
            if (p.Amt < 0 && p.Amt > 0) { r.Rc = "??"; return r; }
            var c = InMem.FindCust(p.Cust);
            if (c == null)
            {
                p.St = "X";
                r.Rc = "X";
                r.Msg = "cust nf";
                Log.Wn("PAY " + p.No + " cust nf " + p.Cust + " " + Util.Comma(p.Amt));
                return r;
            }
            Invoice hit = null;
            try
            {
                if (!p.Memo.IsNul() && p.Memo.StartsWith("IV-")) { hit = InMem.FindInv(p.Memo.Trim()); if (hit.Cust != p.Cust) hit = null; }
            }
            catch
            {
            }
            var open = Bill.Open(p.Cust);
            if (hit == null)
            {
                foreach (var iv in open)
                {
                    if (iv.Bal == p.Amt) { hit = iv; break; }
                }
            }
            if (hit == null)
            {
                foreach (var iv in open)
                {
                    decimal diff = iv.Bal - p.Amt;
                    if (diff < 0) diff = -diff;
                    if (diff <= Tol()) { hit = iv; break; }
                }
            }
            if (hit == null && p.Typ == "S")
            {
                foreach (var iv in open)
                {
                    if (iv.Bal - p.Amt == Cfg.GetD("BANKFEE"))
                    {
                        hit = iv;
                        p.Fee = Cfg.GetD("BANKFEE");
                        break;
                    }
                }
            }
            if (hit == null)
            {
                if (open.Count == 0)
                {
                    p.St = "X";
                    r.Rc = "X";
                    r.Msg = "no open inv";
                    Log.Wn("PAY " + p.No + " no open inv " + p.Cust + " " + Util.Comma(p.Amt));
                    return r;
                }
                hit = open[0];
                r.Msg = "oldest";
            }
            decimal app = p.Amt + p.Fee;
            if (app > hit.Bal) app = hit.Bal;
            hit.Bal = Rnd(hit.Bal - app);
            p.Rem = Rnd(p.Amt + p.Fee - app);
            p.InvNo = hit.No;
            if (hit.Bal <= 0) { hit.St = "P"; hit.Bal = 0; }
            if (p.Rem > 0) { p.St = "P"; r.Rc = "P"; }
            else
            {
                p.St = "M";
                r.Rc = "OK";
            }
            Matched++;
            r.Amt = app;
            Hist.Add("PAY", p.No, hit.No + "/" + app);
            string tail = "";
            if (p.Rem > 0) tail += " rem=" + Util.Comma(p.Rem);
            if (r.Msg != "") tail += " (" + r.Msg + ")";
            Log.W("PAY " + p.No + " " + Util.PadJ(p.Cust, 6) + " " + Util.PadL(Util.Comma(p.Amt), 10) + " " + K.PayTyp(p.Typ) + " -> " + hit.No + " " + K.PaySt(p.St) + tail);
            return r;
        }

        public static decimal Rnd(decimal v)
        {
            return Math.Round(v, 0);
        }

        public static Rslt MatchAll()
        {
            var r = new Rslt("OK");
            foreach (var p in InMem.Pay)
            {
                if (p.St != "N") continue;
                var x = Match(p);
                if (x.Rc == "OK" || x.Rc == "P") r.Cnt++;
                else r.Errs.Add(p.No + ":" + x.Rc);
            }
            G.Put("matched", r.Cnt);
            G.LastRc = r.Errs.Count > 0 ? "W" : "OK";
            return r;
        }

        public static void Undo(Payment p)
        {
            if (p == null || p.InvNo.IsNul()) return;
            var iv = InMem.FindInv(p.InvNo);
            if (iv != null) { iv.Bal += p.Amt + p.Fee - p.Rem; if (iv.St == "P") iv.St = "S"; }
            p.St = "N";
            p.InvNo = "";
            p.Rem = 0;
            Hist.Add("PAY", p.No, "UNDO");
        }

        public static decimal Unmatched()
        {
            decimal t = 0;
            foreach (var p in InMem.Pay)
            {
                if (p.St == "X") t += p.Amt;
                if (p.St == "P") t += p.Rem;
            }
            return t;
        }

    }
}
