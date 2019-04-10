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
            WriteAllText(output_file, Line.to_xml(
                                          s(new List<Line>(
                                              Line.from_xml(args[0])))));
        }

        static List<Line> s(List<Line> u)
        {
            if (u.Count < 2) return u;
            List<Line> l = new List<Line>();
            List<Line> r = new List<Line>();
            int m = u.Count / 2;
            for (int i = 0; i < m; i++) l.Add(u[i]);
            for (int i = m; i < u.Count; i++) r.Add(u[i]);
            return s(s(l), s(r));
        }

        static List<Line> s(List<Line> l, List<Line> r)
        {
            List<Line> res = new List<Line>();

            while (l.Count > 0 || r.Count > 0)
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
            return res;
        }
    }
}
