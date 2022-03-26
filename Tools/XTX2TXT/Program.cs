using System;
using System.IO;
using System.Linq;

namespace XTX2TXT
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("XTX2TXT - VSModTools for 1998 Gangsters: Organized Crime");
            Console.WriteLine("This tool will convert all XTX to TXT files inside containing folder.");
            Console.WriteLine("----------------------------------------------------------------------");

            var dir = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine("Target dir: " + dir);

            var fileList = Directory.EnumerateFiles(dir, "*.xtx");
            if (fileList.Count() == 0)
                Console.WriteLine("No XTX files found! Check if the folder containing the XTX2TXT tool is the right one.");
            else
            {
                Console.WriteLine("XTX files found in target directory: " + fileList.Count().ToString());
                Console.WriteLine("Press any key to start the conversion...");
                Console.ReadKey(true);

                foreach (var file in fileList)
                {
                    var data = File.ReadAllBytes(file);
                    Decode(data);
                    string newFileName = file.Replace(".xtx", ".txt");
                    Console.WriteLine(newFileName);
                    File.WriteAllBytes(newFileName, data);
                }
                Console.WriteLine("All done! With TXT files the game will no longer load XTX files.");
                Console.WriteLine("Do you want to delete all XTX files? Press '9' to delete or any other key to exit.");
                if(Console.ReadKey(true).KeyChar == '9')
                    foreach (var file in fileList)
                    {
                        File.Delete(file);
                        Console.WriteLine("Deleted: " + file);
                    }

            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }

        static void Decode(byte[] data)
        {
             byte[] key = new byte[] { 175, 222, 222, 250 }; //0xFADEDEAF

            int roundedLenght = data.Length & 0xFFFFFFC;

            for (int i = 0; i < roundedLenght; i++)
                data[i] = (byte)(data[i] ^ key[i % 4]);

            if (data.Length % 2 == 1)
                data[roundedLenght] = (byte)(data[roundedLenght] ^ 0xFF);
        }
    }
}
