using System.Text;
using Legacy.Common;

namespace Legacy.Services
{
    public static class Ntf
    {
        public static List<string> Q = new List<string>();
        public static int Sent = 0;
        public static int Drop = 0;

        public static void Send(string typ, string to, string msg)
        {
            if (Cfg.Ntf == "X") { Drop++; return; }
            if (G.Dry) { Drop++; return; }
            string pfx = "[NOTE]";
            if (typ == "M") pfx = "[MAIL]";
            if (typ == "S") pfx = "[SLACK]";
            if (typ == "N") pfx = "[NOTE]";
            string s = pfx + " " + Util.Ymdhm(G.Dt) + " to=" + Util.Nz2(to, Cfg.To) + " " + msg;
            try
            {
                if (typ == "M" && to.IsNul()) throw new Exception("no to");
            }
            catch
            {
            }
            Q.Add(s);
            Sent++;
            if (Cfg.Ntf == "N") return;
            Log.W(s);
        }

        public static void Flush()
        {
            if (Q.Count == 0) return;
            Log.H("NOTIFY " + Q.Count);
            foreach (var s in Q) Log.W(s);
            Q.Clear();
        }

        public static string Mail(string to, string sub, string body)
        {
            var sb = new StringBuilder();
            sb.Append("From: ").Append(Cfg.To).Append("\n");
            sb.Append("To: ").Append(to).Append("\n");
            sb.Append("Subject: ").Append(sub).Append("\n");
            sb.Append("\n");
            sb.Append(body).Append("\n");
            sb.Append("--\n").Append(Util.Ver()).Append("\n");
            return sb.ToString();
        }

        public static int Cnt()
        {
            return Q.Count;
        }

        public static void Alert(string msg)
        {
            Send("S", "#dummy-alert", msg);
        }
    }
}
