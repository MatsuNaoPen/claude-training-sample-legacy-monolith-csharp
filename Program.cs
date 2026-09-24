using Legacy.Models;
using Legacy.Services;
using Legacy.Common;
using Legacy.Data;
using Legacy.Batch;
using Legacy.Report;

var p = new OrderProc();

var o1 = new Order
{
    No = "DUMMY-0001",
    Cust = new Customer { Cd = "C-100", Nm = "SAMPLE A", Rk = "A", Flg = false, Cls = 20 },
    Dt = new DateTime(2026, 1, 10, 10, 0, 0),
    Urg = false,
    Lines = new List<OrderLine>
    {
        new OrderLine { Itm = "P-01", Qty = 10, Prc = 1200, Ret = false, Typ = "N" },
        new OrderLine { Itm = "P-02", Qty = 0, Prc = 500, Ret = false, Typ = "N" },
        new OrderLine { Itm = "P-03", Qty = 2, Prc = 300, Ret = true, Typ = "N" },
    }
};

var o2 = new Order
{
    No = "DUMMY-0002",
    Cust = new Customer { Cd = "X-200", Nm = "SAMPLE B", Rk = "B", Flg = true, Cls = 25 },
    Dt = new DateTime(2026, 1, 26, 9, 0, 0),
    Urg = true,
    Lines = new List<OrderLine>
    {
        new OrderLine { Itm = "P-10", Qty = 3, Prc = 1500, Ret = false, Typ = "S" },
        new OrderLine { Itm = "P-11", Qty = 1, Prc = 2000, Ret = false, Typ = "N" },
    }
};

var o3 = new Order
{
    No = "DUMMY-0003",
    Cust = new Customer { Cd = "C-300", Nm = "SAMPLE C", Rk = "C", Flg = false, Cls = 0 },
    Dt = new DateTime(2026, 1, 20, 16, 30, 0),
    Urg = false,
    Lines = new List<OrderLine>
    {
        new OrderLine { Itm = "P-20", Qty = 5, Prc = 400, Ret = true, Typ = "N" },
        new OrderLine { Itm = "P-21", Qty = 8, Prc = 900, Ret = false, Typ = "N" },
    }
};

string mode = args.Length > 0 ? args[0].ToLowerInvariant() : "";
string flg = args.Length > 1 ? args[1] : "";
G.Mode = mode == "" ? "ALL" : mode;
bool ordDone = false;

if (mode == "" || mode == "ord")
{
    foreach (var o in new[] { o1, o2, o3 })
    {
        var r = p.Proc(o);
        Console.WriteLine($"{o.No}\t{r.rc}\t{r.amt}");
    }
}

Seed.Run();
if (flg == "D") G.Flg = "D";

void DoOrd()
{
    if (ordDone) return;
    ordDone = true;
    G.Mode = "O";
    Log.Blank();
    Log.H("ORDER");
    OrdSvc.RegAll(new List<Order> { o1, o2, o3 });
    var wk = new List<Order>();
    foreach (var o in InMem.Ord)
    {
        if (InMem.Rc(o.No) == "") wk.Add(o);
    }
    OrdSvc.RegAll(wk);
    OrdSvc.Cancel("DUMMY-0006");
    G.Put("ord:n", OrdSvc.N);
}

void DoStk()
{
    DoOrd();
    G.Mode = "I";
    Log.H("STOCK");
    RepRun.Run("stk");
}

void DoShip()
{
    DoOrd();
    G.Mode = "S";
    Log.H("SHIP");
    var r = ShipProc.Run();
    Log.W("shipped=" + r.Cnt);
}

void DoBill()
{
    DoOrd();
    G.Mode = "L";
    Log.H("BILL");
    var ivs = Bill.MkAll();
    int sent = Bill.SendAll();
    Log.W("inv=" + ivs.Count + " sent=" + sent);
    Log.H("PAY");
    var pr = Pay.MatchAll();
    Log.W("matched=" + pr.Cnt + " ng=" + Chk.Join(pr.Errs));
}

void DoBatch()
{
    DoOrd();
    NightBatch.Run(flg);
}

void DoRep()
{
    DoOrd();
    RepRun.All();
}

void DoMst()
{
    Mst.Dump();
    var errs = Mst.Vfy();
    foreach (var e in errs)
    {
        Log.Wn(e);
    }
    Log.W("mst err=" + errs.Count);
}

switch (mode)
{
    case "":
        DoOrd();
        DoShip();
        DoBill();
        DoBatch();
        DoRep();
        break;
    case "ord":
        DoOrd();
        break;
    case "inv":
    case "stk":
        DoStk();
        break;
    case "ship":
        DoShip();
        break;
    case "bill":
    case "pay":
        DoBill();
        break;
    case "batch":
        DoBatch();
        break;
    case "rep":
        DoRep();
        break;
    case "mst":
        DoMst();
        break;
    default:
        Log.E("mode? " + mode);
        Log.W("usage: ord | inv | ship | bill | batch [S|X|F|C] | rep | mst");
        break;
}

Ntf.Flush();
if (mode == "" || flg == "D")
{
    Log.H("HIST " + InMem.Tx.Count);
    Hist.Dump(15);
}
Log.W("END mode=" + G.Mode + " last=" + G.LastRc + " err=" + G.Err + " tx=" + InMem.Tx.Count + " log=" + Log.N);
