using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using static lib73.clib;

namespace lib73
{
	/// <summary>
    /// Represents a line in the log of Schuladmin.
    /// </summary>
    public class Line
    {
		/// <summary>
        /// The timestamp of the line.
        /// </summary>
        public DateTime time;

        /// <summary>
        /// The tokens that are given after the caller and name.
        /// </summary>
        public string tokens;

        /// <summary>
        /// The caller that did cause this event.
        /// </summary>
        public string caller;

        /// <summary>
        /// The name of the event.
        /// </summary>
        public string name;

        /// <summary>
        /// The return value of the call.
        /// </summary>
        public int error;

        /// <summary>
		/// Parses a line of Schuladmin log into a new <see cref="Line"/> object.
        /// </summary>
        /// <param name="l">The Schuladmin line</param>
        public Line(string l)
        {
            string[] s = l.Split('	');
            if (s.Length != 5) throw new Exception("Invalid 73LogLine: " + l);
            time = DateTime.Parse($"{s[0]} {s[1]}");
            string[] s2 = s[2].Split(' ');
            name = s2[0];
            if (s2[1] != "in") throw new Exception("s2[1] should be in, but it is " + s2[1]);
            caller = s2[2];
            for (int i = 3; i < s2.Length; i++)
                caller += " " + s2[i];
            tokens = s[3];
            error = int.Parse(s[4]);
        }

        /// <summary>
        /// Constructs a new <see cref="Line"/> object from the given variables.
        /// </summary>
        public Line(DateTime time, string tokens, string caller, string name, int error)
        {
            this.time = time;
            this.tokens = tokens;
            this.caller = caller;
            this.name = name;
            this.error = error;
        }

        public Line(string time, string tokens, string caller, string name, int error)
        : this(DateTime.Parse(time), tokens, caller, name, error) { }

        public byte[] enc()
        {
            List<byte> b = new List<byte>();
            b.Add((byte)utf8c(name));
            b.AddRange(utf8(name));
            b.Add((byte)utf8c(caller));
            b.AddRange(utf8(caller));
            long l = time.ToBinary();
            b.Add((byte)(l >> 56));
            b.Add((byte)(l >> 48));
            b.Add((byte)(l >> 40));
            b.Add((byte)(l >> 32));
            b.Add((byte)(l >> 24));
            b.Add((byte)(l >> 16));
            b.Add((byte)(l >> 8));
            b.Add((byte)l);
            int i = utf8c(tokens);
            b.Add((byte)(i >> 8));
            b.Add((byte)i);
            b.AddRange(utf8(tokens));
            b.Add((byte)(error >> 24));
            b.Add((byte)(error >> 16));
            b.Add((byte)(error >> 8));
            b.Add((byte)error);
            return b.ToArray();
        }

        /// <summary>
        /// Encodes the lines to a 73DB.
        /// </summary>
        /// <returns>The encoded 73db.</returns>
        /// <param name="lines">The lines.</param>
        public static byte[] enc_73db(IEnumerable<Line> lines)
        {
            MemoryStream ms = new MemoryStream();
            DeflateStream ds = new DeflateStream(ms, CompressionLevel.Optimal, true);
            foreach (Line l in lines)
			{
				byte[] b = l.enc();
				ds.Write(b, 0, b.Length);
			}
            ds.Close();
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
            string name = utf8(buffer);
            buffer = new byte[s.ReadByte()];
            s.Read(buffer, 0, buffer.Length);
            string caller = utf8(buffer);
            DateTime time = DateTime.FromBinary((s.ReadByte() << 56) |
                (s.ReadByte() << 48) | (s.ReadByte() << 40) |
                (s.ReadByte() << 32) | (s.ReadByte() << 24) |
                (s.ReadByte() << 16) | (s.ReadByte() << 8)  | s.ReadByte());
            buffer = new byte[(s.ReadByte() << 8) | s.ReadByte()];
            s.Read(buffer, 0, buffer.Length);
            string tokens = utf8(buffer);
            int error = (s.ReadByte() << 24) | (s.ReadByte() << 16) |
                        (s.ReadByte() << 8)  |  s.ReadByte();
            return new Line(time, tokens, caller, name, error);
        }

        public static Line[] dec_73db(byte[] bytes)
        {
            DeflateStream ds = new DeflateStream(new MemoryStream(bytes, false), CompressionMode.Decompress);
            MemoryStream ms = new MemoryStream();
            ds.CopyTo(ms);
            List<Line> l = new List<Line>();
            while(true)
                try { l.Add(dec(ms)); }
                catch { break; }
            return l.ToArray();
        }

        public string to_xml()
        {
            return $"<line name=\"{xml_esc(name)}\" caller=\"{xml_esc(caller)}\" " +
                   $"tokens=\"{xml_esc(tokens)}\" time=\"{time.ToBinary()}\" " +
                   $"error=\"{error}\" />";
        }

        public static string to_xml(IEnumerable<Line> lines)
        {
            StringBuilder sb = new StringBuilder("<sad>");
            foreach (Line l in lines)
            {
                sb.Append("\n    ");
                sb.Append(l.to_xml());
            }
            sb.Append("\n</sad>\n");
            return sb.ToString();
        }

        public static Line from_xml(XmlReader xml)
        {
            return new Line(xml.GetAttribute("time"),
                xml_reverse_esc(xml.GetAttribute("tokens")),
                xml_reverse_esc(xml.GetAttribute("caller")),
                xml_reverse_esc(xml.GetAttribute("name")),
                int.Parse(xml.GetAttribute("error")));
        }

        /// <summary>
        /// Parses a 73XML to a line array.
        /// </summary>
        /// <returns>The parsed lines.</returns>
        /// <param name="file">The file of the 73XML.</param>
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
            return s
                .Replace("[SOH]", "\u0001")
                .Replace("[STX]", "\u0002")
                .Replace("[ETX]", "\u0003");
        }

        /// <summary>
        /// Escapes a few chars for XML.
        /// </summary>
        /// <returns>The escaped string.</returns>
        /// <param name="s">The raw string.</param>
        static string xml_esc(string s)
        {
            return s
                .Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\u0001", "[SOH]")
                .Replace("\u0002", "[STX]")
                .Replace("\u0003", "[ETX]");
        }
    }
}
