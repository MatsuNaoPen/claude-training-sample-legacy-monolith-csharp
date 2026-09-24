using Legacy.Models;
using Legacy.Common;
using Legacy.Data;

namespace Legacy.Services
{
    public static class Mst
    {
        public static int Upd = 0;

        public static Rslt AddCust(Customer c)
        {
            var r = new Rslt("OK");
            var errs = Chk.Cust(c);
            if (errs.Count > 0)
            {
                r.Rc = "NG";
                r.Errs = errs;
                r.Msg = Chk.Join(errs);
                return r;
            }
            if (InMem.FindCust(c.Cd) != null) { r.Rc = "DUP"; return r; }
            InMem.Cust.Add(c);
            Repo.Cust2.Add(c);
            Upd++;
            Hist.Add("CUST", c.Cd, "ADD");
            return r;
        }

        public static Rslt UpdCust(string cd, Dictionary<string, object> d)
        {
            var r = Repo.Upd("CUST", cd, d);
            if (r.Rc == "OK") { Upd++; Hist.Add("CUST", cd, "UPD " + Util.DumpDic(d)); }
            return r;
        }

        public static string Rk(string cd)
        {
            var c = InMem.FindCust(cd);
            if (c == null) return "";
            return c.Rk;
        }

        public static Rslt AddItm(Item it)
        {
            var r = new Rslt("OK");
            var errs = Chk.Itm(it);
            if (errs.Count > 0)
            {
                r.Rc = "NG";
                r.Errs = errs;
                r.Msg = Chk.Join(errs);
                return r;
            }
            if (InMem.FindItm(it.Cd) != null) { r.Rc = "DUP"; return r; }
            it.Upd = Util.Ymd(G.Dt);
            InMem.Itm.Add(it);
            Upd++;
            Hist.Add("ITM", it.Cd, "ADD");
            return r;
        }

        public static decimal Prc(string cd)
        {
            var it = InMem.FindItm(cd);
            if (it == null) return 0;
            return it.Prc;
        }

        public static Rslt AddWh(Warehouse w)
        {
            var r = new Rslt("OK");
            if (w == null || w.Cd.IsNul()) { r.Rc = "NG"; return r; }
            if (InMem.FindWh(w.Cd) != null) { r.Rc = "DUP"; return r; }
            InMem.Wh.Add(w);
            Upd++;
            Hist.Add("WH", w.Cd, "ADD");
            return r;
        }

        public static object Find(string tbl, string key)
        {
            return Repo.Load(tbl, key);
        }

        public static void Dump()
        {
            Log.H("MASTER");
            Log.W("CUST " + InMem.Cust.Count);
            foreach (var c in InMem.Cust) Log.W("  " + Util.PadJ(c.Cd, 6) + " " + Util.PadJ(c.Nm, 12) + " " + c.Rk + " " + Util.PadJ(Util.RkNm(c.Rk), 4) + " cls=" + Util.PadL(c.Cls.ToString(), 2) + (c.Flg ? " *" : ""));
            Log.W("ITM " + InMem.Itm.Count);
            foreach (var it in InMem.Itm) Log.W("  " + Util.PadJ(it.Cd, 6) + " " + Util.PadJ(it.Nm, 14) + " " + Util.PadL(Util.Comma(it.Prc), 8) + " " + it.Typ + " " + it.Cat + " " + it.Wh + " min=" + it.Min + (it.Dis ? " DIS" : ""));
            Log.W("WH " + InMem.Wh.Count);
            foreach (var w in InMem.Wh) Log.W("  " + Util.PadJ(w.Cd, 6) + " " + Util.PadJ(w.Nm, 12) + " " + K.WhTyp(w.Typ) + " pri=" + w.Pri + " cap=" + w.Cap);
            Log.W("USR " + InMem.Usr.Count);
            foreach (var u in InMem.Usr) Log.W("  " + Util.PadJ(u.Id, 6) + " " + Util.PadJ(u.Nm, 12) + " " + u.Rl + " " + u.Dep + (u.Flg ? "" : " (off)"));
        }

        public static List<string> Vfy()
        {
            var errs = new List<string>();
            foreach (var c in InMem.Cust)
            {
                if (!K.IsRk(c.Rk)) errs.Add("CUST " + c.Cd + " rk=" + c.Rk);
                if (c.Cls > 28) errs.Add("CUST " + c.Cd + " cls=" + c.Cls);
                int d = 0;
                foreach (var c2 in InMem.Cust) if (c2.Cd == c.Cd) d++;
                if (d > 1) errs.Add("CUST DUP " + c.Cd);
            }
            foreach (var it in InMem.Itm)
            {
                if (it.Prc <= 0 && !it.Dis) errs.Add("ITM " + it.Cd + " prc=0");
                var w = InMem.FindWh(it.Wh);
                if (w == null) errs.Add("ITM " + it.Cd + " wh nf " + it.Wh);
                else if (w.Typ == "X" && !it.Dis) errs.Add("ITM " + it.Cd + " ext wh");
            }
            foreach (var s in InMem.Stk)
            {
                if (InMem.FindItm(s.Itm) == null) errs.Add("STK orphan " + s.Itm);
                if (s.Rsv > s.Qty) errs.Add("STK rsv>qty " + s.Itm + "/" + s.Wh + " " + s.Rsv + ">" + s.Qty);
                if (s.Qty < 0) errs.Add("STK neg " + s.Itm + "/" + s.Wh);
            }
            G.Put("mstErr", errs.Count);
            return errs;
        }

        public static int Purge()
        {
            int n = 0;
            var keep = new List<Item>();
            foreach (var it in InMem.Itm)
            {
                if (it.Dis && Inv.Tot(it.Cd) == 0) { n++; continue; }
                keep.Add(it);
            }
            InMem.Itm.Clear();
            foreach (var it in keep) InMem.Itm.Add(it);
            Hist.Add("ITM", "*", "PURGE " + n);
            return n;
        }

        public static string Exp()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("cd,nm,rk,flg,cls\n");
            foreach (var c in InMem.Cust) sb.Append(Util.ToCsv(c.Cd, c.Nm, c.Rk, c.Flg, c.Cls)).Append("\n");
            return sb.ToString();
        }

        public static int Imp(string tbl, string csv)
        {
            int n = 0;
            var hdr = Util.Hdr(csv);
            foreach (var row in Util.Rows(csv))
            {
                var d = new Dictionary<string, object>();
                for (int i = 0; i < hdr.Length; i++) d[hdr[i]] = Util.Col(row, i);
                var r = Repo.Save(tbl, d);
                if (r.Rc == "OK") n++;
            }
            Hist.Add(tbl, "*", "IMP " + n);
            return n;
        }
    }
}
