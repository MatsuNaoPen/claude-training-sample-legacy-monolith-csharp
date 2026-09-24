namespace Legacy.Common
{
    public static class K
    {
        public static string[] RK = { "A", "B", "C" };
        public static string[] ST = { "N", "P", "S", "X" };
        public static string[] RCOK = { "OK", "NX", "NU", "BO" };
        public static string[] RCNG = { "E0", "E1", "E2", "E3", "R1", "Z0", "HL", "NG" };
        public static int CLS = 20;
        public static int CLSH = 15;
        public static decimal FREE = 10000;
        public static decimal LIM = 1000000;
        public static int MAXQ = 9999;
        public static string ORD = "ORD";
        public static string CUST = "CUST";
        public static string ITM = "ITM";
        public static string STK = "STK";
        public static string SHP = "SHP";
        public static string INV = "INV";
        public static string PAY = "PAY";
        public static string WH = "WH";

        public static string Nm(string rc)
        {
            switch (rc)
            {
                case "OK": return "受注OK";
                case "NX": return "翌月扱い";
                case "NU": return "翌月至急";
                case "RV": return "要確認(返品)";
                case "HL": return "保留(高額)";
                case "Z0": return "金額ゼロ";
                case "R1": return "返品超過";
                case "E0": return "注文なし";
                case "E1": return "顧客なし";
                case "E2": return "明細なし";
                case "E3": return "数量不正";
                case "BO": return "欠品";
                case "NG": return "検証NG";
                case "CX": return "取消";
                default: return rc;
            }
        }

        public static string StNm(string st)
        {
            switch (st)
            {
                case "N": return "未処理";
                case "P": return "ピック済";
                case "S": return "出荷済";
                case "X": return "取消";
                case "M": return "消込済";
                default: return st;
            }
        }

        public static string InvSt(string st)
        {
            switch (st)
            {
                case "N": return "未送付";
                case "S": return "送付済";
                case "P": return "入金済";
                case "X": return "無効";
                default: return st;
            }
        }

        public static string PaySt(string st)
        {
            switch (st)
            {
                case "N": return "未消込";
                case "M": return "消込済";
                case "P": return "一部消込";
                case "X": return "不明入金";
                default: return st;
            }
        }

        public static string PayTyp(string t)
        {
            if (t == "S") return "振込";
            if (t == "C") return "現金";
            if (t == "N") return "手形";
            return t;
        }

        public static string WhTyp(string t)
        {
            if (t == "X") return "外部";
            if (t == "N") return "自社";
            return t;
        }

        public static bool Ok(string rc)
        {
            foreach (var r in RCOK) if (r == rc) return true;
            return false;
        }

        public static bool IsRk(string rk)
        {
            foreach (var r in RK) if (r == rk) return true;
            return false;
        }
    }
}
