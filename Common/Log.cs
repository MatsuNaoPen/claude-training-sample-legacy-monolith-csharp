namespace Legacy.Common
{
    public static class Log
    {
        public static List<string> Buf = new List<string>();
        public static int Lvl = 1;
        public static bool Quiet = false;
        public static int N = 0;

        public static void W(string s)
        {
            Buf.Add(s);
            N++;
            if (!Quiet) Console.WriteLine(s);
        }

        public static void I(string s)
        {
            if (Lvl > 1) return;
            W("[I] " + s);
        }

        public static void E(string s)
        {
            G.Err++;
            G.Msg(s);
            W("[E] " + s);
        }

        public static void Wn(string s) { W("[W] " + s); }

        public static void D(string s)
        {
            if (G.Flg == "D" || Cfg.Dbg == "1") W("[D] " + s);
        }

        public static void Sep() { W(new string('-', 60)); }
        public static void Sep2() { W(new string('=', 60)); }

        public static void H(string t)
        {
            Sep2();
            W("## " + t + "  (" + Util.Ymd(G.Dt) + " " + G.Usr + ")");
            Sep2();
        }

        public static void Blank() { W(""); }

        public static void Dump()
        {
            foreach (var s in Buf) Console.WriteLine(s);
        }

        public static void Clr()
        {
            Buf.Clear();
            N = 0;
        }
    }
}
