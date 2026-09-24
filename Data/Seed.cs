using Legacy.Models;
using Legacy.Common;

namespace Legacy.Data
{
    public static class Seed
    {
        public static bool Done = false;

        public static string CsvOrd =
            "no,cust,dt,urg,itm,qty,prc,ret,typ\n" +
            "DUMMY-0101,C-100,2026/01/28,0,P-01,4,1200,0,N\n" +
            "DUMMY-0101,C-100,2026/01/28,0,P-02,2,500,0,N\n" +
            "DUMMY-0102,X-200,2026/01/29 16:00,1,\"P-10\",2,1500,0,S\n" +
            "# DUMMY-0102 2nd line held\n" +
            "DUMMY-0103,C-999,2026/01/30,0,P-20,1,400,0,N\n" +
            "DUMMY-0104,C-300,bad-date,0,P-21,1,900,0,N\n" +
            "DUMMY-0105,C-700,2026/01/30,0,P-30,-1,250,0,N\n";

        public static string CsvStk =
            "itm,wh,qty\n" +
            "P-01,W1,50\n" +
            "P-30,W2,100\n" +
            "P-99,W1,5\n" +
            "P-40,W2,2\n" +
            "BAD LINE\n" +
            "P-11,W1,10\n";

        public static string CsvPay =
            "no,cust,amt,dt,typ,memo\n" +
            "PY-0004,C-300,7040,2026/01/30,S,IV-1004\n" +
            "PY-0005,C-400,\"1,234\",2026/01/30,C,\n" +
            "PY-0006,X-500,17250,2026/01/31,S,\n";

        public static void Run()
        {
            if (Done) return;
            InMem.Clear();
            Cust();
            Itm();
            Wh();
            Stk();
            Usr();
            Ord();
            Pay();
            G.Dt = new DateTime(2026, 1, 31, 9, 0, 0);
            G.Usr = "U02";
            InMem.Init = true;
            Done = true;
        }

        static void Cust()
        {
            InMem.Cust.Add(new Customer { Cd = "C-100", Nm = "SAMPLE A", Rk = "A", Flg = false, Cls = 20 });
            InMem.Cust.Add(new Customer { Cd = "X-200", Nm = "SAMPLE B", Rk = "B", Flg = true, Cls = 25 });
            InMem.Cust.Add(new Customer { Cd = "C-300", Nm = "SAMPLE C", Rk = "C", Flg = false, Cls = 0 });
            InMem.Cust.Add(new Customer { Cd = "C-400", Nm = "SAMPLE D", Rk = "B", Flg = false, Cls = 15 });
            InMem.Cust.Add(new Customer { Cd = "X-500", Nm = "SAMPLE E", Rk = "A", Flg = true, Cls = 31 });
            InMem.Cust.Add(new Customer { Cd = "C-600", Nm = "SAMPLE F", Rk = "D", Flg = false, Cls = 20 });
            InMem.Cust.Add(new Customer { Cd = "C-700", Nm = "SAMPLE G", Rk = "C", Flg = true, Cls = 10 });
        }

        static void Itm()
        {
            InMem.Itm.Add(new Item { Cd = "P-01", Nm = "DUMMY BOLT M6", Prc = 1200, Cst = 700, Typ = "N", Unit = "BOX", Cat = "N", Wh = "W1", Min = 10 });
            InMem.Itm.Add(new Item { Cd = "P-02", Nm = "DUMMY NUT M6", Prc = 500, Cst = 200, Typ = "N", Unit = "BOX", Cat = "N", Wh = "W1", Min = 10 });
            InMem.Itm.Add(new Item { Cd = "P-03", Nm = "DUMMY WASHER", Prc = 300, Cst = 100, Typ = "N", Unit = "BOX", Cat = "N", Wh = "W1", Min = 20 });
            InMem.Itm.Add(new Item { Cd = "P-10", Nm = "DUMMY KIT S", Prc = 1500, Cst = 900, Typ = "S", Unit = "SET", Cat = "N", Wh = "W1", Min = 5 });
            InMem.Itm.Add(new Item { Cd = "P-11", Nm = "DUMMY KIT L", Prc = 2000, Cst = 1300, Typ = "S", Unit = "SET", Cat = "N", Wh = "W1", Min = 5 });
            InMem.Itm.Add(new Item { Cd = "P-20", Nm = "DUMMY TAPE", Prc = 400, Cst = 150, Typ = "N", Unit = "PC", Cat = "N", Wh = "W2", Min = 30 });
            InMem.Itm.Add(new Item { Cd = "P-21", Nm = "DUMMY GLUE", Prc = 900, Cst = 400, Typ = "N", Unit = "PC", Cat = "N", Wh = "W2", Min = 10 });
            InMem.Itm.Add(new Item { Cd = "P-30", Nm = "DUMMY SNACK", Prc = 250, Cst = 120, Typ = "N", Unit = "PC", Cat = "F", Wh = "W2", Min = 50 });
            InMem.Itm.Add(new Item { Cd = "P-31", Nm = "DUMMY WATER", Prc = 80, Cst = 30, Typ = "N", Unit = "PC", Cat = "F", Wh = "W2", Min = 100 });
            InMem.Itm.Add(new Item { Cd = "P-40", Nm = "DUMMY MOTOR", Prc = 15000, Cst = 9000, Typ = "N", Unit = "PC", Cat = "N", Wh = "W9", Min = 1 });
            InMem.Itm.Add(new Item { Cd = "P-41", Nm = "DUMMY PUMP", Prc = 32000, Cst = 21000, Typ = "N", Unit = "PC", Cat = "N", Wh = "W1", Min = 1 });
            InMem.Itm.Add(new Item { Cd = "P-99", Nm = "DUMMY OLD", Prc = 0, Cst = 0, Typ = "N", Unit = "PC", Cat = "X", Wh = "W1", Min = 0, Dis = true });
        }

        static void Wh()
        {
            InMem.Wh.Add(new Warehouse { Cd = "W1", Nm = "MAIN-DUMMY", Typ = "N", Flg = true, Pri = 1, Cap = 10000, Area = "E" });
            InMem.Wh.Add(new Warehouse { Cd = "W2", Nm = "SUB-DUMMY", Typ = "N", Flg = true, Pri = 2, Cap = 5000, Area = "W" });
            InMem.Wh.Add(new Warehouse { Cd = "W9", Nm = "EXT-DUMMY", Typ = "X", Flg = false, Pri = 9, Cap = 0, Area = "-" });
        }

        static void Stk()
        {
            Add("P-01", "W1", 30);
            Add("P-01", "W2", 20);
            Add("P-02", "W1", 40);
            Add("P-03", "W1", 100);
            Add("P-10", "W1", 8);
            Add("P-10", "W2", 4);
            Add("P-11", "W1", 3, "X");
            Add("P-11", "W2", 5);
            Add("P-20", "W2", 60);
            Add("P-21", "W1", 6);
            Add("P-21", "W2", 10);
            Add("P-30", "W2", 200);
            Add("P-31", "W2", 500);
            Add("P-40", "W9", 3);
            Add("P-41", "W1", 50);
            Add("P-99", "W1", 0);
        }

        static void Add(string itm, string wh, int qty)
        {
            Add(itm, wh, qty, "");
        }

        static void Add(string itm, string wh, int qty, string flg)
        {
            var s = new Stock();
            s.Itm = itm;
            s.Wh = wh;
            s.Qty = qty;
            s.Rsv = 0;
            s.Flg = flg;
            s.Lot = "L" + wh + itm.Replace("P-", "");
            s.Upd = new DateTime(2026, 1, 1);
            s.Usr = "SEED";
            InMem.Stk.Add(s);
        }

        static void Usr()
        {
            InMem.Usr.Add(new User { Id = "U01", Nm = "ADMIN-DUMMY", Rl = "A", Dep = "SYS", Flg = true, Last = new DateTime(2026, 1, 30) });
            InMem.Usr.Add(new User { Id = "U02", Nm = "OPE-DUMMY", Rl = "O", Dep = "SALES", Flg = true, Last = new DateTime(2026, 1, 31) });
            InMem.Usr.Add(new User { Id = "U03", Nm = "READ-DUMMY", Rl = "R", Dep = "ACC", Flg = false, Last = new DateTime(2025, 12, 1) });
        }

        static void Ord()
        {
            var o4 = new Order
            {
                No = "DUMMY-0004",
                Cust = InMem.FindCust("C-400"),
                Dt = new DateTime(2026, 1, 12, 11, 0, 0),
                Urg = false,
                Lines = new List<OrderLine>
                {
                    new OrderLine { Itm = "P-30", Qty = 12, Prc = 250, Ret = false, Typ = "N" },
                    new OrderLine { Itm = "P-31", Qty = 40, Prc = 80, Ret = false, Typ = "N" },
                }
            };
            InMem.Ord.Add(o4);

            var o5 = new Order
            {
                No = "DUMMY-0005",
                Cust = InMem.FindCust("X-500"),
                Dt = new DateTime(2026, 1, 31, 10, 0, 0),
                Urg = true,
                Lines = new List<OrderLine>
                {
                    new OrderLine { Itm = "P-40", Qty = 1, Prc = 15000, Ret = false, Typ = "N" },
                }
            };
            InMem.Ord.Add(o5);

            var o6 = new Order
            {
                No = "DUMMY-0006",
                Cust = InMem.FindCust("C-600"),
                Dt = new DateTime(2026, 1, 15, 14, 0, 0),
                Urg = false,
                Lines = new List<OrderLine>
                {
                    new OrderLine { Itm = "P-41", Qty = 40, Prc = 32000, Ret = false, Typ = "N" },
                }
            };
            InMem.Ord.Add(o6);

            var o7 = new Order
            {
                No = "DUMMY-0007",
                Cust = InMem.FindCust("C-700"),
                Dt = new DateTime(2026, 1, 5, 9, 30, 0),
                Urg = false,
                Lines = new List<OrderLine>
                {
                    new OrderLine { Itm = "P-20", Qty = 3, Prc = 400, Ret = true, Typ = "N" },
                    new OrderLine { Itm = "P-21", Qty = 2, Prc = 900, Ret = false, Typ = "N" },
                }
            };
            InMem.Ord.Add(o7);
        }

        static void Pay()
        {
            InMem.Pay.Add(new Payment { No = "PY-0001", Cust = "C-100", Amt = 11470, Dt = new DateTime(2026, 1, 30), Typ = "S", St = "N", Memo = "" });
            InMem.Pay.Add(new Payment { No = "PY-0002", Cust = "X-200", Amt = 11000, Dt = new DateTime(2026, 1, 30), Typ = "C", St = "N", Memo = "" });
            InMem.Pay.Add(new Payment { No = "PY-0003", Cust = "C-999", Amt = 5000, Dt = new DateTime(2026, 1, 31), Typ = "S", St = "N", Memo = "unknown" });
        }
    }
}
