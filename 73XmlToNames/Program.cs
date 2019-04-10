using System.Collections.Generic;
using static System.Console;
using static System.IO.File;
using lib73;

namespace _73XmlToNames
{
    class Program
    {
        static void Main(string[] args)
        {
            Write("Input file: ");
            string in_file = ReadLine();
            Write("Output file: ");
            string name_file = ReadLine();
            List<string> names = new List<string>();
            foreach (Line l in Line.from_xml(in_file))
                if (!names.Contains(l.name))
                    names.Add(l.name);
            WriteAllLines(name_file, names);
        }
    }
}
