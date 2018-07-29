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

            List<Line> allLines = new List<Line>();
            foreach (string file in args)
            {
                string[] rawInput = ReadAllLines(file);
                Line[] lns = new Line[rawInput.Length - 1];
                for (int i = 0; i < rawInput.Length - 1; i++)
                    lns[i] = new Line(rawInput[i]);
                allLines.AddRange(lns);
                WriteLine("Added " + file);
            }

            Line[] lines = allLines.ToArray();
            List<byte> b = new List<byte>();

            foreach (Line l in lines)
                b.AddRange(l.enc());

            DeflateStream ds = new DeflateStream(Open(db, FileMode.Create, FileAccess.Write), CompressionLevel.Optimal, false);

            ds.Write(b.ToArray(), 0, b.Count);
        }
    }
}
