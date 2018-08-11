using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
            byte[] c = BitConverter.GetBytes(time.ToBinary());
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(c);
            b.AddRange(c);
            string t = combine_tokens();
            c = BitConverter.GetBytes((ushort)t.Length);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(c);
            b.AddRange(c);
            b.AddRange(Encoding.UTF8.GetBytes(t));
            return b.ToArray();
        }

        public static byte[] enc_and_comp(IEnumerable<Line> lines)
        {
            List<byte> bytes = new List<byte>();
            foreach (Line l in lines)
                bytes.AddRange(l.enc());
            MemoryStream ms = new MemoryStream();
            DeflateStream ds = new DeflateStream(ms, CompressionLevel.Optimal, true);
            ds.Write(bytes.ToArray(), 0, bytes.Count);
            return ms.ToArray();
        }

        public static Line dec(byte[] enc)
        {
            return dec(new MemoryStream(enc, false));
        }

        public static Line dec(Stream s)
        {
            byte[] buffer = new byte[s.ReadByte()];
            s.Read(buffer, 0, buffer.Length);
            string name = Encoding.UTF8.GetString(buffer);
            buffer = new byte[s.ReadByte()];
            s.Read(buffer, 0, buffer.Length);
            string caller = Encoding.UTF8.GetString(buffer);
            buffer = new byte[8];
            s.Read(buffer, 0, 8);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            DateTime time = DateTime.FromBinary(BitConverter.ToInt64(buffer, 0));
            buffer = new byte[2];
            s.Read(buffer, 0, 2);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            buffer = new byte[BitConverter.ToUInt16(buffer, 0)];
            s.Read(buffer, 0, buffer.Length);
            string tokens = Encoding.UTF8.GetString(buffer);
            return new Line(time, tokens, caller, name);
        }

        public static Line[] decomp_and_dec(byte[] bytes)
        {
            DeflateStream ds = new DeflateStream(new MemoryStream(bytes, false), CompressionMode.Decompress);
            MemoryStream ms = new MemoryStream();
            ds.CopyTo(ms);
            List<Line> l = new List<Line>();
            while(true)
                try
                {
                    l.Add(dec(ms));
                }
                catch
                {
                    break;
                }
            return l.ToArray();
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
