namespace _73XmlTimeSort
{
    class Line
    {
        public long time { get; private set; }
        public string tokens { get; private set; }
        public string caller { get; private set; }
        public string name { get; private set; }

        public Line(string name, string caller, string tokens, string time)
        {
            this.time = long.Parse(time);
            this.name = name;
            this.caller = caller;
            this.tokens = tokens;
        }

        public override string ToString() => $"<line name=\"{xml_esc(name)}\" caller=\"{xml_esc(caller)}\" tokens=\"{xml_esc(tokens)}\" time=\"{time}\" />";

        string xml_esc(string s) => s.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("'", "&apos;").Replace("<", "&lt;").Replace(">", "&gt;");
    }
}
