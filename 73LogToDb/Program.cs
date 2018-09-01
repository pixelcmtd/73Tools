using lib73;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using static System.Console;
using static System.IO.File;

namespace _73LogToDb
{
    class Program
    {
        static void Main(string[] args)
        {
            Write("73db file: ");
            string db = ReadLine();
            List<Line> lines = new List<Line>();
            foreach (string file in args)
            {
                string[] rawInput = ReadAllLines(file);
                long i = rawInput.LongLength - 1;
                for (int j = 0; j < i; j++)
                    lines.Add(new Line(rawInput[j]));
                WriteLine("Added " + file);
            }
            WriteAllBytes(db, Line.enc_73db(lines));
        }
    }
}
