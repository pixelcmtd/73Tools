using System;

namespace _73LogToXml
{
    class Line
    {
        public DateTime time;
        public string[] tokens;
        public string caller;
        public string name;

        public Line(string l)
        {
            time = parse_time(l);
            string[] s = l.Substring(20).Split(' ', '	');
            name = s[0];
            caller = s[2];
            tokens = new string[s.Length - 3];
            Array.Copy(s, 3, tokens, 0, tokens.Length);
        }

        string combine_tokens()
        {
            string s = tokens[0];
            for (int i = 1; i < tokens.Length; i++)
                s += " " + tokens[i];
            return s;
        }

        public override string ToString() => $"<line name=\"{xml_esc(name)}\" caller=\"{xml_esc(caller)}\" tokens=\"{xml_esc(combine_tokens())}\" time=\"{time.ToBinary()}\" />";

        DateTime parse_time(string s) => new DateTime(int.Parse(s.Substring(6, 4)), int.Parse(s.Substring(3, 2)), int.Parse(s.Substring(0, 2)), int.Parse(s.Substring(11, 2)), int.Parse(s.Substring(14, 2)), int.Parse(s.Substring(17, 2)));

        string xml_esc(string s) => s.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("'", "&apos;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("", "[SOH]").Replace("", "[STX]").Replace("", "[ETX]");
    }
}
