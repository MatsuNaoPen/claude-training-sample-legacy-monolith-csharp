using Legacy.Models;
using Legacy.Common;
using Legacy.Services;

namespace Legacy.Data
{
    public class Tbl<T>
    {
        public List<T> Rows = new List<T>();
        public string Nm;
        public int Ver;

        public Tbl(string nm)
        {
            Nm = nm;
        }

        public void Add(T r)
        {
            Rows.Add(r);
            Ver++;
        }

        public T Find(Func<T, bool> f)
        {
            foreach (var r in Rows) if (f(r)) return r;
            return default(T);
        }

        public int Cnt
        {
            get { return Rows.Count; }
        }

        public bool Del(T r)
        {
            bool ok = Rows.Remove(r);
            if (ok) Ver++;
            return ok;
        }
    }

    public static class Repo
    {
        public static Tbl<Order> Ord2 = new Tbl<Order>("ORD");
        public static Tbl<Customer> Cust2 = new Tbl<Customer>("CUST");
        public static Tbl<Payment> Pay2 = new Tbl<Payment>("PAY");
        public static int Saved = 0;
        public static int Loaded = 0;

        public static Rslt Save(string tbl, Dictionary<string, object> d)
        {
            var r = new Rslt();
            object o = Cnv.FromDic(tbl, d);
            if (o == null)
            {
                r.Rc = "NG";
                r.Msg = "cnv:" + tbl;
                return r;
            }
            switch (tbl)
            {
                case "ORD":
                    InMem.Add("ORD", o);
                    Ord2.Add((Order)o);
                    break;
                case "CUST":
                    InMem.Add("CUST", o);
                    Cust2.Add((Customer)o);
                    break;
                case "PAY":
                    InMem.Add("PAY", o);
                    Pay2.Add((Payment)o);
                    break;
                default:
                    InMem.Add(tbl, o);
                    break;
            }
            Saved++;
            r.Rc = "OK";
            r.Ok = true;
            r.Val = o;
            return r;
        }

        public static object Load(string tbl, string key)
        {
            Loaded++;
            switch (tbl)
            {
                case "ORD": return InMem.FindOrd(key);
                case "CUST": return InMem.FindCust(key);
                case "ITM": return InMem.FindItm(key);
                case "WH": return InMem.FindWh(key);
                case "SHP": return InMem.FindShp(key);
                case "INV": return InMem.FindInv(key);
                case "PAY": return InMem.FindPay(key);
                default: return null;
            }
        }

        public static Rslt Upd(string tbl, string key, Dictionary<string, object> d)
        {
            var r = new Rslt();
            var o = Load(tbl, key);
            if (o == null)
            {
                r.Rc = "NF";
                r.Msg = tbl + ":" + key;
                return r;
            }
            Cnv.Apply(o, d);
            InMem.Ver++;
            r.Rc = "OK";
            r.Ok = true;
            r.Val = o;
            return r;
        }

        public static bool Del(string tbl, string key)
        {
            var o = Load(tbl, key);
            if (o == null) return false;
            switch (tbl)
            {
                case "ORD": return InMem.Ord.Remove((Order)o);
                case "CUST": return InMem.Cust.Remove((Customer)o);
                case "PAY": return InMem.Pay.Remove((Payment)o);
                default: return false;
            }
        }
    }
}
