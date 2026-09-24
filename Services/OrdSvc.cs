using Legacy.Models;
using Legacy.Common;
using Legacy.Data;

namespace Legacy.Services
{
    public static class OrdSvc
    {
        public static OrderProc P = new OrderProc();
        public static int N = 0;

        public static Rslt Reg(Order o)
        {
            var res = new Rslt();
            var errs = Chk.Ord(o);
            if (errs.Count > 0)
            {
                res.Rc = "NG";
                res.Errs = errs;
                res.Msg = Chk.Join(errs);
                G.LastRc = "NG";
                G.Last = o;
                Log.E("ORD NG " + (o == null ? "-" : o.No) + " " + res.Msg);
                if (o != null && !o.No.IsNul()) InMem.Put("rc:" + o.No, "NG");
                return res;
            }
            var r = P.Proc(o);
            G.Last = o;
            G.LastRc = r.rc;
            G.LastAmt = r.amt;
            G.Cnt++;
            N++;
            if (r.rc == "E0" && o != null) Log.E("E0 with order");
            res.Rc = r.rc;
            res.Amt = r.amt;
            res.Ok = K.Ok(r.rc);
            if (InMem.FindOrd(o.No) == null) InMem.Ord.Add(o);
            InMem.Put("rc:" + o.No, r.rc);
            InMem.Put("amt:" + o.No, r.amt);
            InMem.Put("usr:" + o.No, G.Usr);
            Hist.Add("ORD", o.No, r.rc + "/" + r.amt);
            if (K.Ok(r.rc))
            {
                var iv = Inv.Rsv(o);
                res.Set("short", iv.Get("short"));
                if (iv.Rc == "BO")
                {
                    res.Rc = "BO";
                    InMem.Put("rc:" + o.No, "BO");
                    Ntf.Send("S", "#dummy-wh", "BO " + o.No + " short=" + iv.Cnt);
                }
            }
            if (r.rc == "HL" || r.rc == "RV") Ntf.Send("M", Cfg.To, r.rc + " " + o.No + " " + o.Cust.Cd + " " + r.amt);
            if (o.Urg && K.Ok(res.Rc)) Ntf.Send("S", "#dummy-ship", "URG " + o.No);
            Log.W("ORD " + o.No + " " + Util.PadR(res.Rc, 3) + " " + Util.PadL(Util.Comma(r.amt), 10) + "  " + K.Nm(res.Rc));
            return res;
        }

        public static List<Rslt> RegAll(List<Order> lst)
        {
            var r = new List<Rslt>();
            foreach (var o in lst) r.Add(Reg(o));
            return r;
        }

        public static Rslt Cancel(string no)
        {
            var res = new Rslt();
            var o = InMem.FindOrd(no);
            if (o == null) { res.Rc = "NF"; return res; }
            string rc = InMem.Rc(no);
            if (rc == "CX") { res.Rc = "DUP"; return res; }
            if (InMem.GetS("shp:" + no) != "")
            {
                res.Rc = "SHP";
                res.Msg = "already shipped";
                return res;
            }
            Inv.Rel(o);
            InMem.Put("rc:" + no, "CX");
            InMem.Put("amt:" + no, 0m);
            Hist.Add("ORD", no, "CX", "was " + rc);
            G.LastRc = "CX";
            res.Rc = "OK";
            res.Ok = true;
            Log.W("ORD " + no + " CX  (was " + rc + ")");
            return res;
        }

        public static decimal Tot(Order o)
        {
            decimal sub = 0;
            if (o == null || o.Lines == null) return 0;
            foreach (var l in o.Lines)
            {
                if (l.Qty <= 0) continue;
                decimal t = l.Qty * l.Prc;
                if (l.Ret) t = -t;
                sub += t;
            }
            return sub;
        }

        public static int Cnt(Order o)
        {
            int n = 0;
            if (o == null || o.Lines == null) return 0;
            foreach (var l in o.Lines)
            {
                if (l.Qty <= 0) continue;
                if (l.Ret) continue;
                n += l.Qty;
            }
            return n;
        }

        public static string Rc(string no)
        {
            return InMem.Rc(no);
        }

        public static Order Cp(Order o)
        {
            return Util.Cp(o);
        }

        public static List<Order> Open()
        {
            var r = new List<Order>();
            foreach (var o in InMem.Ord)
            {
                if (!K.Ok(InMem.Rc(o.No))) continue;
                if (InMem.GetS("shp:" + o.No) != "") continue;
                r.Add(o);
            }
            return r;
        }

        public static int CntRc(string rc)
        {
            int n = 0;
            foreach (var o in InMem.Ord) if (InMem.Rc(o.No) == rc) n++;
            return n;
        }
    }
}
