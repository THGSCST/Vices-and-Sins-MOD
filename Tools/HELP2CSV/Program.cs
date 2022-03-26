using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HELP2TXT
{
    class Program
    {
        static string outputFilename = "HelpTooltips.CSV";
        static string[] inputFilenames = new string[] { "help.dat", "help.idx", "tooltips.dat", "tooltips.idx" };
        static void Main(string[] args)
        {
            Console.WriteLine("HELP2CSV for 1998 Gangsters: Organized Crime");
            Console.WriteLine("This tool will convert Help and Tooltips DAT/IDX files to CSV.");
            Console.WriteLine("----------------------------------------------------------------------");

            var dir = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine("Target dir: " + dir);

            var data = new List<BinaryReader>(4);
            var swriter = new StringWriter();

            foreach (var file in inputFilenames)
                if (!File.Exists(dir + file))
                {
                    Console.WriteLine(file + " file not found!!! Check if the folder containing the HELP2CSV tool is the right one.");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey(true);
                    return;
                }
                else
                    data.Add(new BinaryReader(File.OpenRead(dir + file), System.Text.Encoding.GetEncoding(1252)));

            if (data[1].ReadUInt32() != 3452764707 || data[3].ReadUInt32() != 3452764707 || data[1].ReadUInt16() != data[3].ReadUInt16())
                throw new InvalidDataException("Header of index files are wrong");

            if (data[1].BaseStream.Length != data[3].BaseStream.Length)
                throw new InvalidDataException("Indexes dont have same size");

            int indexSize = (int)((data[1].BaseStream.Length - data[1].BaseStream.Position) / 4);
            int charsToRead;

            swriter.WriteLine("Id;Tooltip Text;Help Window Contents");

            for (int i = 0; i < indexSize; i++)
            {
                swriter.Write(i);
                swriter.Write(";"); //Tab
                data[2].BaseStream.Position = data[3].ReadUInt16();
                charsToRead = data[3].ReadUInt16();
                swriter.Write(data[2].ReadChars(charsToRead));
                swriter.Write(";"); //Tab
                data[0].BaseStream.Position = data[1].ReadUInt16();
                charsToRead = data[1].ReadUInt16();
                swriter.Write(data[0].ReadChars(charsToRead));
                swriter.WriteLine();
            }
            System.IO.File.WriteAllText(outputFilename, swriter.ToString());
            Console.WriteLine("All done! Generated output: " + outputFilename);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
