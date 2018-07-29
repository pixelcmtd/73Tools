using System;
using System.Collections.Generic;
using System.Text;

namespace _73LogToDb
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

        public byte[] enc()
        {
            List<byte> b = new List<byte>();
            b.Add((byte)name.Length);
            b.AddRange(Encoding.UTF8.GetBytes(name));
            b.Add((byte)caller.Length);
            b.AddRange(Encoding.UTF8.GetBytes(caller));
            b.AddRange(BitConverter.GetBytes(time.Ticks));
            string t = combine_tokens();
            b.AddRange(BitConverter.GetBytes((ushort)t.Length));
            b.AddRange(Encoding.UTF8.GetBytes(t));
            return b.ToArray();
        }

        DateTime parse_time(string s)
        {
            return new DateTime(int.Parse(s.Substring(6, 4)), int.Parse(s.Substring(3, 2)), int.Parse(s.Substring(0, 2)), int.Parse(s.Substring(11, 2)), int.Parse(s.Substring(14, 2)), int.Parse(s.Substring(17, 2)));
        }
    }
}
