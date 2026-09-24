namespace Legacy.Models
{
    public class Rslt
    {
        public string Rc { get; set; }
        public string Msg { get; set; }
        public bool Ok { get; set; }
        public int Cnt { get; set; }
        public decimal Amt { get; set; }
        public object Val { get; set; }
        public Dictionary<string, object> Bag { get; set; }
        public List<string> Errs { get; set; }

        public Rslt()
        {
            Rc = "";
            Msg = "";
            Bag = new Dictionary<string, object>();
            Errs = new List<string>();
        }

        public Rslt(string rc)
        {
            Rc = rc;
            Msg = "";
            Ok = rc == "OK";
            Bag = new Dictionary<string, object>();
            Errs = new List<string>();
        }

        public void Set(string k, object v)
        {
            Bag[k] = v;
        }

        public object Get(string k)
        {
            if (Bag.ContainsKey(k)) return Bag[k];
            return null;
        }
    }
}
