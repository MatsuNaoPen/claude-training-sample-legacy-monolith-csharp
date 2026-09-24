using Legacy.Models;
using Legacy.Common;
using Legacy.Data;

namespace Legacy.Services
{
    public static class ShipProc
    {
        public static int Made = 0;

        public static Shipment Mk(Order o)
        {
            if (o == null) return null;
            if (InMem.GetS("shp:" + o.No) != "") return InMem.ShpOfOrd(o.No);
            var s = new Shipment();
            s.No = G.NextNo("SH");
            s.OrdNo = o.No;
            s.Cust = o.Cust == null ? "" : o.Cust.Cd;
            s.St = "N";
            s.Dt = G.Dt;
            s.Urg = o.Urg;
            s.Lines = new List<ShipLine>();
            s.Wh = "";
            s.Memo = "";
            foreach (var a in InMem.AlcOf(o.No))
            {
                if (a.St != "R") continue;
                var l = new ShipLine { Itm = a.Itm, Wh = a.Wh, Qty = a.Qty, Lot = Inv.Lot(a.Itm, a.Wh), St = "N" };
                s.Lines.Add(l);
                a.ShpNo = s.No;
                if (s.Wh == "") s.Wh = a.Wh;
                else if (s.Wh != a.Wh) s.Wh = "*";
            }
            s.Fee = Fee(o);
            if (s.Lines.Count == 0) { s.St = "X"; s.Memo = "no alloc"; }
            InMem.Shp.Add(s);
            InMem.Put("shp:" + o.No, s.No);
            Made++;
            Hist.Add("SHP", s.No, "MK " + o.No);
            return s;
        }

        public static Rslt Pick(Shipment s)
        {
            var r = new Rslt("OK");
            if (s == null) { r.Rc = "E"; return r; }
            if (s.St != "N")
            {
                r.Rc = "ST";
                r.Msg = s.St;
                return r;
            }
            foreach (var l in s.Lines)
            {
                var st = InMem.FindStk(l.Itm, l.Wh);
                if (st == null)
                {
                    Log.E("PICK NF " + l.Itm + " " + l.Wh);
                    r.Rc = "NG";
                    continue;
                }
                st.Qty -= l.Qty;
                st.Rsv -= l.Qty;
                st.Upd = G.Dt;
                l.St = "P";
                r.Cnt += l.Qty;
            }
            foreach (var a in InMem.AlcOf(s.OrdNo)) if (a.St == "R") a.St = "P";
            s.St = "P";
            Hist.Add("SHP", s.No, "PICK " + r.Cnt);
            return r;
        }

        public static Rslt Out(Shipment s)
        {
            var r = new Rslt("OK");
            if (s == null) { r.Rc = "E"; return r; }
            if (s.St == "S") { r.Rc = "DUP"; return r; }
            if (s.St != "P")
            {
                r.Rc = "ST";
                r.Msg = s.St;
                return r;
            }
            s.St = "S";
            s.Dt = G.Dt;
            s.Trk = "TRK" + Util.Ymd2(G.Dt) + s.No.Substring(3);
            foreach (var a in InMem.AlcOf(s.OrdNo)) if (a.St == "P") a.St = "S";
            var w = InMem.FindWh(s.Wh);
            if (w != null && w.Typ == "X") Ntf.Send("M", Cfg.To, "EXT SHIP " + s.No);
            else Ntf.Send("N", "", "SHIP " + s.No + " " + s.OrdNo + " " + s.Trk);
            if (s.Urg) Ntf.Send("S", "#dummy-ship", "URG OUT " + s.No);
            InMem.Put("shpdt:" + s.OrdNo, s.Dt);
            Hist.Add("SHP", s.No, "OUT " + s.Trk);
            Log.W("SHP " + s.No + " " + s.OrdNo + " " + Util.PadJ(s.Wh, 2) + " " + K.StNm(s.St) + " fee=" + Util.Comma(s.Fee) + " lines=" + s.Lines.Count);
            return r;
        }

        public static Rslt Cancel(Shipment s)
        {
            var r = new Rslt("OK");
            if (s == null) { r.Rc = "E"; return r; }
            if (s.St == "S")
            {
                r.Rc = "NG";
                r.Msg = "shipped";
                return r;
            }
            if (s.St == "Z")
            {
                r.Rc = "NG";
                r.Msg = "locked";
                return r;
            }
            if (s.St == "P")
            {
                foreach (var l in s.Lines)
                {
                    var st = InMem.FindStk(l.Itm, l.Wh);
                    if (st != null) st.Qty += l.Qty;
                }
            }
            s.St = "X";
            InMem.Put("shp:" + s.OrdNo, "");
            Hist.Add("SHP", s.No, "CX");
            return r;
        }

        public static decimal Fee(Order o)
        {
            decimal sub = OrdSvc.Tot(o);
            int cnt = OrdSvc.Cnt(o);
            decimal s = 0;
            if (sub > Cfg.GetD("FREE")) s = 0;
            else
            {
                if (cnt <= 3) s = Cfg.GetD("FEE");
                else s = Cfg.GetD("FEE2");
            }
            if (o.Urg) s += Cfg.GetD("URG");
            if (o.Cust != null && o.Cust.Cd != null && o.Cust.Cd.StartsWith("X")) s = s * 2;
            if (o.Cust != null && o.Cust.Rk == "A" && !o.Urg) s = 0;
            return s;
        }

        public static Rslt Run()
        {
            var r = new Rslt("OK");
            foreach (var o in OrdSvc.Open())
            {
                var s = Mk(o);
                if (s == null || s.St == "X") { Log.Wn("SHP skip " + o.No + " " + (s == null ? "-" : s.Memo)); continue; }
                var p = Pick(s);
                if (p.Rc != "OK") { Log.Wn("PICK " + s.No + " " + p.Rc); continue; }
                var x = Out(s);
                if (x.Rc == "OK") r.Cnt++;
            }
            G.LastRc = r.Rc;
            G.Put("shipped", r.Cnt);
            return r;
        }

        public static decimal FeeTot()
        {
            decimal t = 0;
            foreach (var s in InMem.Shp) if (s.St == "S") t += s.Fee;
            return t;
        }
    }
}
