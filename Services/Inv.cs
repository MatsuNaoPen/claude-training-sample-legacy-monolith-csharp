using Legacy.Models;
using Legacy.Common;
using Legacy.Data;

namespace Legacy.Services
{
    public static class Inv
    {
        public static int Short = 0;
        public static int Rsvd = 0;

        public static Rslt Rsv(Order o)
        {
            var r = new Rslt("OK");
            int sh = 0;
            if (o == null || o.Lines == null) { r.Rc = "E"; return r; }
            foreach (var l in o.Lines)
            {
                if (l.Qty <= 0) continue;
                if (l.Ret) { Adj(l.Itm, G.Wh, l.Qty, "R"); continue; }
                int rem = RsvLine(o.No, l.Itm, l.Qty);
                if (rem > 0)
                {
                    sh += rem;
                    var a = new Alloc { OrdNo = o.No, Itm = l.Itm, Wh = "", Qty = rem, St = "B", Dt = G.Dt };
                    InMem.Alc.Add(a);
                    Log.Wn("SHORT " + o.No + " " + l.Itm + " " + rem);
                }
            }
            Short += sh;
            r.Set("short", sh);
            r.Cnt = sh;
            if (sh > 0) { r.Rc = "BO"; r.Ok = false; }
            Hist.Add("RSV", o.No, r.Rc + "/" + sh);
            return r;
        }

        static int RsvLine(string no, string itm, int qty)
        {
            int rem = qty;
            foreach (var w in SortWh())
            {
                if (rem <= 0) break;
                if (w.Typ == "X" && G.Flg != "X") continue;
                var s = InMem.FindStk(itm, w.Cd);
                if (s == null) continue;
                if (s.Flg == "X") continue;
                int avl = s.Qty - s.Rsv;
                if (avl <= 0) continue;
                int take = avl < rem ? avl : rem;
                s.Rsv += take;
                s.Upd = G.Dt;
                s.Usr = G.Usr;
                InMem.Alc.Add(new Alloc { OrdNo = no, Itm = itm, Wh = w.Cd, Qty = take, St = "R", Dt = G.Dt });
                Rsvd += take;
                rem -= take;
            }
            return rem;
        }

        static List<Warehouse> SortWh()
        {
            var r = new List<Warehouse>(InMem.Wh);
            for (int i = 0; i < r.Count; i++)
            {
                for (int j = i + 1; j < r.Count; j++)
                {
                    if (r[j].Pri < r[i].Pri)
                    {
                        var t = r[i];
                        r[i] = r[j];
                        r[j] = t;
                    }
                }
            }
            return r;
        }

        public static void Rel(Order o)
        {
            if (o == null) return;
            foreach (var a in InMem.AlcOf(o.No))
            {
                if (a.St != "R") continue;
                var s = InMem.FindStk(a.Itm, a.Wh);
                if (s != null) s.Rsv -= a.Qty;
                a.St = "X";
            }
            Hist.Add("REL", o.No, "");
        }

        public static int Avl(string itm)
        {
            int n = 0;
            foreach (var s in InMem.StkOf(itm))
            {
                if (s.Flg == "X") continue;
                var w = InMem.FindWh(s.Wh);
                if (w != null && w.Typ == "X") continue;
                n += s.Qty - s.Rsv;
            }
            return n;
        }

        public static int Tot(string itm)
        {
            int n = 0;
            foreach (var s in InMem.StkOf(itm)) n += s.Qty;
            return n;
        }

        public static void Adj(string itm, string wh, int d, string flg)
        {
            var s = InMem.FindStk(itm, wh);
            if (s == null)
            {
                s = new Stock { Itm = itm, Wh = wh, Qty = 0, Rsv = 0, Flg = "", Lot = "L" + wh + "NEW", Upd = G.Dt, Usr = G.Usr };
                InMem.Stk.Add(s);
            }
            if (flg == "X") s.Flg = "X";
            else if (flg == "X")
            {
                s.Flg = "";
            }
            else s.Qty += d;
            s.Upd = G.Dt;
            s.Usr = G.Usr;
            if (s.Qty < 0) Log.Wn("NEG " + itm + " " + wh + " " + s.Qty);
            Hist.Add("STK", itm + "/" + wh, flg + d);
        }

        public static bool Chk(string itm, int qty)
        {
            return Avl(itm) >= qty;
        }

        public static void Move(string itm, string from, string to, int qty)
        {
            var a = InMem.FindStk(itm, from);
            if (a == null) return;
            if (a.Qty - a.Rsv < qty) return;
            a.Qty -= qty;
            Adj(itm, to, qty, "M");
            Hist.Add("MOV", itm, from + ">" + to + " " + qty);
        }

        public static void ReRsv()
        {
            foreach (var s in InMem.Stk) s.Rsv = 0;
            var keep = new List<Alloc>();
            foreach (var a in InMem.Alc) if (a.St == "P" || a.St == "S") keep.Add(a);
            InMem.Alc.Clear();
            foreach (var a in keep) InMem.Alc.Add(a);
            int n = 0;
            foreach (var o in InMem.Ord)
            {
                if (!K.Ok(InMem.Rc(o.No))) continue;
                if (InMem.GetS("shp:" + o.No) != "") continue;
                n += ReRsvOne(o);
            }
            G.Put("rersv", n);
            Log.W("RERSV ord=" + n + " alc=" + InMem.Alc.Count);
        }

        static int ReRsvOne(Order o)
        {
            int sh = 0;
            foreach (var l in o.Lines)
            {
                if (l.Qty <= 0 || l.Ret) continue;
                int rem = RsvLine(o.No, l.Itm, l.Qty);
                if (rem > 0)
                {
                    sh += rem;
                    InMem.Alc.Add(new Alloc { OrdNo = o.No, Itm = l.Itm, Wh = "", Qty = rem, St = "B", Dt = G.Dt });
                }
            }
            string rc = InMem.Rc(o.No);
            if (sh > 0 && rc != "BO") InMem.Put("rc:" + o.No, "BO");
            if (sh == 0 && rc == "BO") InMem.Put("rc:" + o.No, "OK");
            return 1;
        }

        public static List<Stock> ByWh(string wh)
        {
            var r = new List<Stock>();
            foreach (var s in InMem.Stk) if (s.Wh == wh) r.Add(s);
            return r;
        }

        public static string Lot(string itm, string wh)
        {
            var s = InMem.FindStk(itm, wh);
            if (s == null) return "";
            return Util.Nz(s.Lot);
        }

        public static List<string> Low()
        {
            var r = new List<string>();
            foreach (var it in InMem.Itm)
            {
                if (it.Dis) continue;
                int a = Avl(it.Cd);
                if (a < it.Min) r.Add(it.Cd + " avl=" + a + " min=" + it.Min);
            }
            return r;
        }
    }
}
