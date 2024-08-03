using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Names2CSV
{
    internal class Program
    {
        static string datFilename = "Names.dat";
        static string csvFilename = "Names.CSV";
        static void Main(string[] args)
        {
            Console.WriteLine("Names2CSV for 1998 Gangsters: Organized Crime");
            Console.WriteLine("This tool will convert Names.dat file to/from CSV.");
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine();

            var dir = AppDomain.CurrentDomain.BaseDirectory;
            if (!File.Exists(dir + datFilename))
            {
                Console.WriteLine(datFilename + " file not found!!! Check if the folder containing the Names2CSV tool is the right one.");
                PressAnyKeyToExit();
                return;
            }

            Console.WriteLine("Press 1 to convert Names.dat to Names.CSV [DAT -> CSV]");
            Console.WriteLine("Press 9 to convert Names.CSV to Names.dat [CSV -> DAT]");
            Console.WriteLine();

            while (true)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        Console.WriteLine("You pressed 1. Conversion started...");

                        var data = File.ReadAllBytes(dir + datFilename);
                        var uncompressedRLE = new MemoryStream();
                        HuffmanTree.Decode(new MemoryStream(data), uncompressedRLE);
                        var uncompressed = PackBytes.Decode(uncompressedRLE.ToArray());
                        var names = new Names(new MemoryStream(uncompressed));
                        File.WriteAllText(dir + csvFilename, names.ToCsv());

                        Console.WriteLine("Done!");
                        PressAnyKeyToExit();

                        return;
                    case ConsoleKey.D9:
                        Console.WriteLine("You pressed 9. Conversion started...");

                        if (!File.Exists(dir + csvFilename))
                        {
                            Console.WriteLine(datFilename + " file not found!!! Check if the folder containing the Names2CSV tool is the right one.");
                            PressAnyKeyToExit();
                            return;
                        }
                        
                        var csv = File.ReadAllText(dir + csvFilename);
                        var namesFromCSV = new Names(csv);
                        var bin = new MemoryStream();
                        namesFromCSV.SerializeToBinary(bin);
                        var bin2 = PackBytes.Encode(bin.ToArray());
                        var output = new MemoryStream();
                        HuffmanTree.Encode(new MemoryStream(bin2), output);
                        File.WriteAllBytes(dir + datFilename, output.ToArray());

                        Console.WriteLine("Done!");
                        PressAnyKeyToExit();

                        return;
                }
            }
        }

        static void PressAnyKeyToExit()
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
