using Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace XLSX2Data
{
    internal class Program
    {
        static string dir = AppDomain.CurrentDomain.BaseDirectory;
        static string xlsxFilename = "Data.xlsx";

        static void Main(string[] args)
        {
            Console.WriteLine("XLSX2Data - VSModTools for 1998 Gangsters: Organized Crime");
            Console.WriteLine("This tool extracts data from an XLSX spreadsheet and converts it into TXT files for use in the Data folder.");
            Console.WriteLine("----------------------------------------------------------------------");

            if (!File.Exists(dir + xlsxFilename))
            {
                Console.WriteLine(xlsxFilename + " file not found!!! Check if the folder containing the XLSX2Data has the XLSX spreadsheet file.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("Monitoring for file changes in the following location:");
            Console.WriteLine($"{dir}{xlsxFilename}");
            Console.WriteLine();
            Console.WriteLine("Edit the spreadsheet anytime. Saved changes will update all TXT files automatically.");
            Console.WriteLine("For manual updates, press 'M'.");
            Console.WriteLine("To stop monitoring, close this console window.");

            AwaitingMessage();

            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = dir;
                watcher.Filter = xlsxFilename;
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size;
                watcher.Error += OnError;
                watcher.Changed += UpdateAllFiles;
                watcher.Created += UpdateAllFiles;
                watcher.EnableRaisingEvents = true;

                while (true)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if (char.ToUpperInvariant(keyInfo.KeyChar) == 'M')
                    {
                        Console.WriteLine("Manual update requested!");
                        UpdateAllFiles(null, null);
                    }

                    Thread.Sleep(500);
                }
            }

            void UpdateAllFiles(object source, FileSystemEventArgs e)
            {
                Console.WriteLine("----------------------------------------------------------------------");
                foreach (var worksheet in Workbook.Worksheets(dir + xlsxFilename))
                {
                    if (worksheet.Name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var row in worksheet.Rows)
                        {
                            foreach (var cell in row.Cells)
                                if (cell != null)
                                {
                                    sb.Append(cell.Text);
                                    if (cell.Text.StartsWith("### End of File ###"))
                                        goto End;
                                    sb.Append("\t");
                                }
                            sb.Length--; //To remove last tab
                            sb.Append("\r\n");
                        }

                    End:
                        File.WriteAllText(dir + worksheet.Name, sb.ToString());
                        Console.WriteLine(DateTime.Now.ToString() + " File: " + worksheet.Name + " has been updated");
                    }
                }
                AwaitingMessage();
            }

            void OnError(object sender, ErrorEventArgs e)
            {
                Console.WriteLine($"Error: {e.GetException().Message}");
            }

            void AwaitingMessage()
            {
                Console.WriteLine("----------------------------------------------------------------------");
                Console.WriteLine($"Waiting for updates to {xlsxFilename} or 'M' key press...");
            }

        }

    }
}
