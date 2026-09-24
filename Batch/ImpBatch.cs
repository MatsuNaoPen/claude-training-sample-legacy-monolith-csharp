using Legacy.Models;
using Legacy.Common;
using Legacy.Data;
using Legacy.Services;

namespace Legacy.Batch
{
    public class ImpBatch : BatchBase
    {
        public int Ords = 0;
        public int Stks = 0;
        public int Pays = 0;

        public ImpBatch()
        {
            Nm = "IMP";
        }

        protected override void Pre()
        {
            Seed.Run();
            G.Put("imp:src", "SEED");
        }

        protected override void Exec()
        {
            ImpOrd(Seed.CsvOrd);
            ImpStk(Seed.CsvStk);
            ImpPay(Seed.CsvPay);
        }

        void ImpOrd(string txt)
        {
            var rows = Csv.Parse(txt, "ORD");
            var hdr = Csv.Hdr(rows);
            var map = new Dictionary<string, Order>();
            var keys = new List<string>();
            foreach (var r in Csv.Body(rows))
            {
                try
                {
                    string no = Csv.Get(r, hdr, "no");
                    if (no == "") { Err++; continue; }
                    if (!map.ContainsKey(no)) { map[no] = Cnv.Csv2Ord(r, hdr); keys.Add(no); }
                    else map[no].Lines.Add(Cnv.Csv2Line(r, hdr));
                }
                catch
                {
                }
            }
            foreach (var k in keys)
            {
                if (InMem.FindOrd(k) != null)
                {
                    Log.Wn("IMP dup " + k);
                    Err++;
                    continue;
                }
                var res = OrdSvc.Reg(map[k]);
                if (res.Rc == "NG") Err++;
                else Ords++;
                Cnt++;
            }
            Log.W("IMP ORD rows=" + rows.Count + " ord=" + keys.Count + " ok=" + Ords + " skip=" + Csv.Skipped);
        }

        void ImpStk(string txt)
        {
            var lines = txt.Split('\n');
            int n = 0;
            foreach (var line in lines)
            {
                n++;
                if (n == 1) continue;
                if (line.Trim() == "") continue;
                var a = line.Split(',');
                if (a.Length < 3)
                {
                    Log.Wn("IMP STK bad line " + n + ": " + line);
                    Err++;
                    continue;
                }
                var s = Cnv.Csv2Stk(a);
                if (InMem.FindItm(s.Itm) == null)
                {
                    Log.Wn("IMP STK itm nf " + s.Itm);
                    Err++;
                    continue;
                }
                if (InMem.FindWh(s.Wh) == null)
                {
                    Log.Wn("IMP STK wh nf " + s.Wh);
                    Err++;
                    continue;
                }
                Inv.Adj(s.Itm, s.Wh, s.Qty, "I");
                Stks++;
                Cnt++;
            }
            Log.W("IMP STK lines=" + (n - 1) + " ok=" + Stks);
        }

        void ImpPay(string txt)
        {
            var rows = Csv.Parse(txt, "PAY");
            var hdr = Csv.Hdr(rows);
            foreach (var r in Csv.Body(rows))
            {
                var p = Cnv.Csv2Pay(r, hdr);
                if (p == null) { Err++; continue; }
                var errs = Chk.Pay(p);
                if (errs.Count > 0)
                {
                    Log.Wn("IMP PAY " + p.No + " " + Chk.Join(errs));
                    Err++;
                    continue;
                }
                if (InMem.FindPay(p.No) != null)
                {
                    Log.Wn("IMP PAY dup " + p.No);
                    Err++;
                    continue;
                }
                InMem.Pay.Add(p);
                Repo.Pay2.Add(p);
                Pays++;
                Cnt++;
            }
            Log.W("IMP PAY rows=" + rows.Count + " ok=" + Pays);
        }

        protected override void Post()
        {
            G.Put("imp:cnt", Cnt);
            G.Put("imp:err", Err);
            G.Put("imp:ord", Ords);
            G.Put("imp:stk", Stks);
            G.Put("imp:pay", Pays);
            Hist.Add("BAT", Nm, Cnt + "/" + Err);
        }
    }
}
