using System.Collections.Generic;
using System.Xml;
using static System.Console;
using static System.IO.File;

namespace _73XmlToNames
{
    class Program
    {
        static void Main(string[] args)
        {
            Write("Output file: ");
            string name_file = ReadLine();
            XmlReader xml = XmlReader.Create(args[0]);
            List<string> names = new List<string>();
            while (xml.Read())
                if (xml.Name == "line" && !names.Contains(xml.GetAttribute("name")))
                    names.Add(xml.GetAttribute("name"));
            WriteAllLines(name_file, names);
        }
    }
}
