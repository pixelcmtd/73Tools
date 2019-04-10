using lib73;
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
            Write("XML file: ");
            string xml = ReadLine();
            List<Line> lines = new List<Line>();
            foreach(string file in args)
            {
                string[] rawInput = ReadAllLines(file);
                for (int i = 0; i < rawInput.Length; i++)
                    if (rawInput[i] != "") lines.Add(new Line(rawInput[i]));
                WriteLine("Added " + file);
            }
            WriteAllText(xml, Line.to_xml(lines));
        }
    }
}
