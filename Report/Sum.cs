using Legacy.Models;
using Legacy.Common;
using Legacy.Data;
using Legacy.Services;

namespace Legacy.Report
{
    public class Sum : RepBase
    {
        public Sum()
        {
            Nm = "SUM";
            Ttl = "サマリ";
        }

        protected override void Build()
        {
            var byRk = new Dictionary<string, Dictionary<string, object>>();
            var rks = new List<string> { "A", "B", "C", "?" };
            foreach (var rk in rks) byRk[rk] = Mk();
            foreach (var o in InMem.Ord)
            {
                string rc = InMem.Rc(o.No);
                if (!K.Ok(rc)) continue;
                string rk = o.Cust == null ? "?" : o.Cust.Rk;
                if (!byRk.ContainsKey(rk)) rk = "?";
                var d = byRk[rk];
                d["cnt"] = (int)d["cnt"] + 1;
                d["amt"] = (decimal)d["amt"] + InMem.Amt(o.No);
            }
            foreach (var iv in InMem.Inv)
            {
                if (iv.St == "X") continue;
                string rk = Mst.Rk(iv.Cust);
                if (!byRk.ContainsKey(rk)) rk = "?";
                var d = byRk[rk];
                d["tax"] = (decimal)d["tax"] + iv.Tax;
                d["bal"] = (decimal)d["bal"] + iv.Bal;
                d["inv"] = (int)d["inv"] + 1;
            }
            Ln(Col("RK", 3) + Col("NAME", 6) + Util.PadL("ORD", 5) + Util.PadL("AMT", 12) + Util.PadL("INV", 5) + Util.PadL("TAX", 10) + Util.PadL("BAL", 12));
            decimal ta = 0;
            decimal tb = 0;
            foreach (var rk in rks)
            {
                var d = byRk[rk];
                Row(Col(rk, 3) + Col(Util.RkNm(rk), 6) + ColR((int)d["cnt"], 5) + ColR((decimal)d["amt"], 12) + ColR((int)d["inv"], 5) + ColR((decimal)d["tax"], 10) + ColR((decimal)d["bal"], 12));
                ta += (decimal)d["amt"];
                tb += (decimal)d["bal"];
            }
            Ln(Fmt.Line());
            Ln(Util.PadR("TOTAL", 14) + ColR(ta, 12) + Util.PadL("", 15) + ColR(tb, 12));
            Ln("cnt: ord=" + InMem.Ord.Count + " shp=" + InMem.Shp.Count + " inv=" + InMem.Inv.Count + " pay=" + InMem.Pay.Count + " alc=" + InMem.Alc.Count + " tx=" + InMem.Tx.Count + " kv=" + InMem.Kv.Count);
            Ln("bill=" + Amt(Bill.Tot()) + " unmatched=" + Amt(Pay.Unmatched()) + " fee=" + Amt(ShipProc.FeeTot()) + " ntf=" + Ntf.Sent + "/" + Ntf.Drop);
            Ln("G: mode=" + G.Mode + " usr=" + G.Usr + " last=" + G.LastRc + "/" + Amt(G.LastAmt) + " cnt=" + G.Cnt + " err=" + G.Err + " seq=" + G.Seq);
            Ln("bag=" + Util.DumpDic(G.Bag));
            if (G.Msgs.Count > 0)
            {
                Ln("msgs:");
                foreach (var m in G.Msgs) Ln("  " + m);
            }
        }

        Dictionary<string, object> Mk()
        {
            var d = new Dictionary<string, object>();
            d["cnt"] = 0;
            d["inv"] = 0;
            d["amt"] = 0m;
            d["tax"] = 0m;
            d["bal"] = 0m;
            return d;
        }
    }
}
