namespace _73XmlTimeSort
{
    class Line
    {
        public long time;
        public string tokens;
        public string caller;
        public string name;

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
