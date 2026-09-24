using Legacy.Models;
using Legacy.Common;
using Legacy.Data;
using Legacy.Services;

namespace Legacy.Report
{
    public class ShipRep : RepBase
    {
        public ShipRep()
        {
            Nm = "SHP";
            Ttl = "出荷一覧";
        }

        protected override void Build()
        {
            Ln(Col("NO", 8) + Col("ORD", 11) + Col("CUST", 6) + Col("WH", 3) + Col("ST", 3) + Col("DT", 11) + Util.PadL("FEE", 7) + " U " + Col("TRK", 18) + "LINES");
            decimal tf = 0;
            foreach (var s in InMem.Shp)
            {
                var sb = new System.Text.StringBuilder();
                if (s.Lines != null)
                {
                    foreach (var l in s.Lines) sb.Append(l.Itm).Append("x").Append(l.Qty).Append("@").Append(l.Wh).Append(" ");
                }
                Row(Col(s.No, 8) + Col(s.OrdNo, 11) + Col(s.Cust, 6) + Col(s.Wh, 3) + Col(s.St, 3) + Col(Util.Ymd(s.Dt), 11) + ColR(s.Fee, 7) + " " + (s.Urg ? "*" : " ") + " " + Col(Util.Nz(s.Trk), 18) + sb.ToString().Trim() + (s.Memo.IsNul() ? "" : " (" + s.Memo + ")"));
                if (s.St == "S") tf += s.Fee;
            }
            Ln(Fmt.Line());
            Ln(Util.PadR("FEE(S)", 42) + ColR(tf, 7));
            int n = 0;
            int p = 0;
            int x = 0;
            foreach (var s in InMem.Shp)
            {
                if (s.St == "N") n++;
                if (s.St == "P") p++;
                if (s.St == "X") x++;
            }
            Ln("st: N=" + n + " P=" + p + " S=" + (InMem.Shp.Count - n - p - x) + " X=" + x + "  " + K.StNm("N") + "/" + K.StNm("P") + "/" + K.StNm("S") + "/" + K.StNm("X"));
        }
    }
}
