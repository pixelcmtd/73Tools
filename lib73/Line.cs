﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace lib73
{
    public class Line
    {
        public DateTime time;
        public string[] tokens;
        public string caller;
        public string name;

        public Line(string l)
        {
            time = new DateTime(int.Parse(l.Substring(6, 4)), int.Parse(l.Substring(3, 2)), int.Parse(l.Substring(0, 2)), int.Parse(l.Substring(11, 2)), int.Parse(l.Substring(14, 2)), int.Parse(l.Substring(17, 2)));
            string[] s = l.Substring(20).Split(' ', '	');
            name = s[0];
            caller = s[2];
            tokens = new string[s.Length - 3];
            Array.Copy(s, 3, tokens, 0, tokens.Length);
        }

        public Line(DateTime time, string tokens, string caller, string name)
        {
            this.time = time;
            this.tokens = tokens.Split(' ');
            this.caller = caller;
            this.name = name;
        }

        public Line(DateTime time, string[] tokens, string caller, string name)
        {
            this.time = time;
            this.tokens = tokens;
            this.caller = caller;
            this.name = name;
        }

        public Line(string time, string tokens, string caller, string name)
        {
            this.time = DateTime.FromBinary(long.Parse(time));
            this.tokens = tokens.Split(' ');
            this.caller = caller;
            this.name = name;
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
            b.AddRange(BitConverter.GetBytes(time.ToBinary()));
            string t = combine_tokens();
            b.AddRange(BitConverter.GetBytes((ushort)t.Length));
            b.AddRange(Encoding.UTF8.GetBytes(t));
            return b.ToArray();
        }

        public static Line dec(byte[] enc)
        {
            Stream s = new MemoryStream(enc, false);
            byte[] buffer = new byte[s.ReadByte()];
            s.Read(buffer, 0, buffer.Length);
            string name = Encoding.UTF8.GetString(buffer);
            buffer = new byte[s.ReadByte()];
            s.Read(buffer, 0, buffer.Length);
            string caller = Encoding.UTF8.GetString(buffer);
            buffer = new byte[8];
            s.Read(buffer, 0, 8);
            DateTime time = DateTime.FromBinary(BitConverter.ToInt64(buffer, 0));
            buffer = new byte[2];
            s.Read(buffer, 0, 2);
            buffer = new byte[BitConverter.ToUInt16(buffer, 0)];
            s.Read(buffer, 0, buffer.Length);
            string tokens = Encoding.UTF8.GetString(buffer);
            return new Line(time, tokens, caller, name);
        }

        public string to_xml()
        {
            return $"<line name=\"{xml_esc(name)}\" caller=\"{xml_esc(caller)}\" tokens=\"{xml_esc(combine_tokens())}\" time=\"{time.ToBinary()}\" />";
        }

        public static Line from_xml(XmlReader xml)
        {
            return new Line(xml_reverse_esc(xml.GetAttribute("time")), xml_reverse_esc(xml.GetAttribute("tokens")), xml_reverse_esc(xml.GetAttribute("caller")), xml_reverse_esc(xml.GetAttribute("name")));
        }

        public static Line[] from_xml(string file)
        {
            List<Line> lines = new List<Line>();
            XmlReader xml = XmlReader.Create(file);
            while (xml.Read())
                if (xml.Name == "line")
                    lines.Add(from_xml(xml));
            xml.Close();
            return lines.ToArray();
        }

        static string xml_reverse_esc(string s)
        {
            return s.Replace("[SOH]", "").Replace("[STX]", "").Replace("[ETX]", "");
        }

        static string xml_esc(string s)
        {
            return s.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("'", "&apos;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("", "[SOH]").Replace("", "[STX]").Replace("", "[ETX]");
        }
    }
}
