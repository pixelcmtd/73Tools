using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace _73XmlToNames
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Output file: ");
            string name_file = Console.ReadLine();
            XmlReader xml = XmlReader.Create(args[0]);
            List<string> names = new List<string>();
            while (xml.Read())
                if (xml.Name == "line" && !names.Contains(xml.GetAttribute("name")))
                    names.Add(xml.GetAttribute("name"));
            File.WriteAllLines(name_file, names);
        }
    }
}
