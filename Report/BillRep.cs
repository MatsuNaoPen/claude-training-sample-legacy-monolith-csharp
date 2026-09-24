using Legacy.Models;
using Legacy.Common;
using Legacy.Data;
using Legacy.Services;

namespace Legacy.Report
{
    public class BillRep : RepBase
    {
        public BillRep()
        {
            Nm = "INV";
            Ttl = "請求一覧";
        }

        protected override void Build()
        {
            Ln(Col("NO", 8) + Col("CUST", 6) + Col("NAME", 10) + Util.PadL("AMT", 10) + Util.PadL("TAX", 8) + Util.PadL("TOT", 10) + Util.PadL("BAL", 10) + " " + Col("DUE", 11) + Col("ST", 3) + "ORDS");
            decimal ta = 0;
            decimal tt = 0;
            decimal tb = 0;
            foreach (var iv in InMem.Inv)
            {
                var c = InMem.FindCust(iv.Cust);
                string nm = c == null ? "?" : c.Nm;
                string ords = iv.Ords == null ? "" : Util.Join(iv.Ords, ",");
                string ov = "";
                if (iv.St != "P" && iv.St != "X" && iv.Due < G.Dt) ov = " OVER";
                Row(Col(iv.No, 8) + Col(iv.Cust, 6) + Col(nm, 10) + ColR(iv.Amt, 10) + ColR(iv.Tax, 8) + ColR(iv.Tot, 10) + ColR(iv.Bal, 10) + " " + Col(Util.Ymd(iv.Due), 11) + Col(iv.St, 3) + ords + ov);
                if (iv.St == "X") continue;
                ta += iv.Amt;
                tt += iv.Tot;
                tb += iv.Bal;
            }
            Ln(Fmt.Line());
            Ln(Util.PadR("TOTAL", 24) + ColR(ta, 10) + Util.PadL("", 8) + ColR(tt, 10) + ColR(tb, 10));
            Ln("cust bal:");
            foreach (var c in InMem.Cust)
            {
                decimal b = Bill.Bal(c.Cd);
                if (b == 0) continue;
                Ln("  " + Col(c.Cd, 6) + Col(c.Nm, 10) + ColR(b, 10) + " " + Util.RkNm(c.Rk));
            }
            Ln("st: " + K.InvSt("N") + "/" + K.InvSt("S") + "/" + K.InvSt("P") + "/" + K.InvSt("X"));
        }
    }
}
