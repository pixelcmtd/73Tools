using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace _73LogToXml
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Xml file: ");
            string xml = Console.ReadLine();

            List<Line> allLines = new List<Line>();
            foreach(string file in args)
            {
                string[] rawInput = File.ReadAllLines(file);
                Line[] lns = new Line[rawInput.Length - 1];
                for (uint i = 0; i < rawInput.Length - 1; i++)
                    lns[i] = new Line(rawInput[i]);
                allLines.AddRange(lns);
                Console.WriteLine("Added "+file);
            }

            Line[] lines = allLines.ToArray();

            StringBuilder sb = new StringBuilder("<sad>");
            foreach (Line l in lines)
                sb.Append("\r\n    "+l.ToString());
            sb.Append("\r\n</sad>");

            File.WriteAllText(xml, sb.ToString());
        }
    }
}
