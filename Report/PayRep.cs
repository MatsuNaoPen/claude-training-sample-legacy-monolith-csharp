using Legacy.Models;
using Legacy.Common;
using Legacy.Data;
using Legacy.Services;

namespace Legacy.Report
{
    public class PayRep : RepBase
    {
        public PayRep()
        {
            Nm = "PAY";
            Ttl = "入金一覧";
        }

        protected override void Build()
        {
            Ln(Col("NO", 8) + Col("CUST", 6) + Col("DT", 11) + Util.PadL("AMT", 10) + Util.PadL("FEE", 6) + Util.PadL("REM", 8) + " " + Col("TYP", 5) + Col("ST", 3) + Col("INV", 8) + "MEMO");
            decimal ta = 0;
            decimal tu = 0;
            foreach (var p in InMem.Pay)
            {
                Row(Col(p.No, 8) + Col(p.Cust, 6) + Col(Util.Ymd(p.Dt), 11) + ColR(p.Amt, 10) + ColR(p.Fee, 6) + ColR(p.Rem, 8) + " " + Col(K.PayTyp(p.Typ), 5) + Col(p.St, 3) + Col(Util.Nz(p.InvNo), 8) + Util.Nz(p.Memo));
                ta += p.Amt;
                if (p.St == "X") tu += p.Amt;
                if (p.St == "P") tu += p.Rem;
            }
            Ln(Fmt.Line());
            Ln(Util.PadR("TOTAL", 25) + ColR(ta, 10));
            Ln(Util.PadR("UNMATCHED", 25) + ColR(tu, 10) + (tu != Pay.Unmatched() ? " ?" : ""));
            Ln("st: " + K.PaySt("N") + "/" + K.PaySt("M") + "/" + K.PaySt("P") + "/" + K.PaySt("X") + "  matched=" + Pay.Matched);
        }
    }
}
