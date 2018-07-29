using System.Collections.Generic;
using System.Linq;
using static System.IO.File;
using static System.Console;
using System.Text;
using lib73;

namespace _73XmlTimeSort
{
    class Program
    {
        static void Main(string[] args)
        {
            Write("Output file: ");
            string output_file = ReadLine();

            Line[] lines = Line.from_xml(args[0]);

            lines = s(lines);

            StringBuilder sb = new StringBuilder("<sad>");
            foreach (Line l in lines)
                sb.Append("\n    " + l.ToString());
            sb.Append("\n</sad>");

            WriteAllText(output_file, sb.ToString());
        }

        static Line[] s(Line[] u)
        {
            if (u.Length < 2)
                return u;

            List<Line> l = new List<Line>();
            List<Line> r = new List<Line>();
            int m = u.Length / 2;

            for (int i = 0; i < m; i++)
                l.Add(u[i]);

            for (int i = m; i < u.Length; i++)
                r.Add(u[i]);

            return s(s(l.ToArray()).ToList(), s(r.ToArray()).ToList()).ToArray();
        }

        static List<Line> s(List<Line> l, List<Line> r)
        {
            List<Line> res = new List<Line>();

            while (l.Count > 0 || r.Count > 0)
            {
                if (l.Count > 0 && r.Count > 0)
                    if (l.First().time <= r.First().time)
                    {
                        res.Add(l.First());
                        l.Remove(l.First());
                    }
                    else
                    {
                        res.Add(r.First());
                        r.Remove(r.First());
                    }
                else if (l.Count > 0)
                {
                    res.Add(l.First());
                    l.Remove(l.First());
                }
                else if (r.Count > 0)
                {
                    res.Add(r.First());
                    r.Remove(r.First());
                }
            }
            return res;
        }
    }
}
