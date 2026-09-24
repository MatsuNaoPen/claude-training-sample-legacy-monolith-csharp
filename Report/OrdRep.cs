using Legacy.Models;
using Legacy.Common;
using Legacy.Data;
using Legacy.Services;

namespace Legacy.Report
{
    public class OrdRep : RepBase
    {
        public OrdRep()
        {
            Nm = "ORD";
            Ttl = "受注一覧";
        }

        protected override void Build()
        {
            Ln(Col("NO", 11) + Col("CUST", 6) + Col("DT", 11) + "U " + Col("RC", 3) + Util.PadL("AMT", 10) + " " + Util.PadL("SUB", 10) + " " + Col("SHP", 8) + Col("INV", 8) + Col("CLS", 7) + "NAME");
            decimal tot = 0;
            decimal hold = 0;
            foreach (var o in InMem.Ord)
            {
                string rc = InMem.Rc(o.No);
                decimal amt = InMem.Amt(o.No);
                string cust = o.Cust == null ? "-" : o.Cust.Cd;
                Row(Col(o.No, 11) + Col(cust, 6) + Col(Util.Ymd(o.Dt), 11) + (o.Urg ? "* " : "  ") + Col(rc, 3) + ColR(amt, 10) + " " + ColR(OrdSvc.Tot(o), 10) + " " + Col(InMem.GetS("shp:" + o.No), 8) + Col(InMem.GetS("inv:" + o.No), 8) + Col(InMem.GetS("cls:" + o.No), 7) + K.Nm(rc));
                if (K.Ok(rc)) tot += amt;
                if (rc == "HL" || rc == "RV") hold += amt;
            }
            Ln(Fmt.Line());
            Ln(Util.PadR("TOTAL(OK)", 33) + ColR(tot, 10));
            Ln(Util.PadR("HOLD", 33) + ColR(hold, 10));
            Ln("rc: OK=" + OrdSvc.CntRc("OK") + " NX=" + OrdSvc.CntRc("NX") + " NU=" + OrdSvc.CntRc("NU") + " BO=" + OrdSvc.CntRc("BO") + " HL=" + OrdSvc.CntRc("HL") + " RV=" + OrdSvc.CntRc("RV") + " NG=" + OrdSvc.CntRc("NG") + " CX=" + OrdSvc.CntRc("CX"));
        }
    }
}
