namespace Legacy.Common
{
    public static class Cfg
    {
        public static string Tax = "10";
        public static string Tax2 = "8";
        public static string Due = "30";
        public static string Cls = "20";
        public static string ClsH = "15";
        public static string Mode = "P";
        public static string Mode2 = "";
        public static string Wh1 = "W1";
        public static string Wh2 = "W2";
        public static string Wh9 = "W9";
        public static string Fee = "800";
        public static string Fee2 = "1200";
        public static string Urg = "1500";
        public static string Free = "10000";
        public static string Lim = "1000000";
        public static string Sep = ",";
        public static string Ntf = "N";
        public static string To = "dummy@example.invalid";
        public static string Pfx = "DUMMY";
        public static string Tol = "100";
        public static string Ver = "2.7.14";
        public static string Enc = "SJIS";
        public static string Dbg = "0";
        public static string BankFee = "440";
        public static string RepW = "72";

        public static string Get(string k)
        {
            switch (k)
            {
                case "TAX": return Tax;
                case "TAX2": return Tax2;
                case "DUE": return Due;
                case "CLS": return Cls;
                case "CLSH": return ClsH;
                case "MODE": return Mode;
                case "MODE2": return Mode2;
                case "WH1": return Wh1;
                case "WH2": return Wh2;
                case "WH9": return Wh9;
                case "FEE": return Fee;
                case "FEE2": return Fee2;
                case "URG": return Urg;
                case "FREE": return Free;
                case "LIM": return Lim;
                case "SEP": return Sep;
                case "NTF": return Ntf;
                case "TO": return To;
                case "PFX": return Pfx;
                case "TOL": return Tol;
                case "VER": return Ver;
                case "ENC": return Enc;
                case "DBG": return Dbg;
                case "BANKFEE": return BankFee;
                case "REPW": return RepW;
                default: return "";
            }
        }

        public static int GetI(string k)
        {
            int v;
            if (int.TryParse(Get(k), out v)) return v;
            return 0;
        }

        public static decimal GetD(string k)
        {
            decimal v;
            if (decimal.TryParse(Get(k), out v)) return v;
            return 0;
        }

        public static bool Is(string k, string v) { return Get(k) == v; }
    }
}
