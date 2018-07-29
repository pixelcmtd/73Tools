using System.Collections.Generic;
using System.Text;
using static System.Console;
using static System.IO.File;

namespace _73LogToXml
{
    static class Program
    {
        static void Main(string[] args)
        {
            Write("Xml file: ");
            string xml = ReadLine();

            List<Line> allLines = new List<Line>();
            foreach(string file in args)
            {
                string[] rawInput = ReadAllLines(file);
                Line[] lns = new Line[rawInput.Length - 1];
                for (int i = 0; i < rawInput.Length - 1; i++)
                    lns[i] = new Line(rawInput[i]);
                allLines.AddRange(lns);
                WriteLine("Added " + file);
            }

            Line[] lines = allLines.ToArray();

            StringBuilder sb = new StringBuilder("<sad>");
            foreach (Line l in lines)
                sb.Append("\n    "+l.ToString());
            sb.Append("\n</sad>");

            WriteAllText(xml, sb.ToString());
        }
    }
}
