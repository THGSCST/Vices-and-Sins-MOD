using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Excel
{
    /// <summary>
    /// (c) 2014 Vienna, Dietmar Schoder
    /// 
    /// Code Project Open License (CPOL) 1.02
    /// 
    /// Deals with an Excel workbook in an xlsx-file and provides all worksheets in it
    /// </summary>
    public class Workbook
    {
        public static sst SharedStrings;

        /// <summary>
        /// All worksheets in the Excel workbook deserialized
        /// </summary>
        /// <param name="ExcelFileName">Full path and filename of the Excel xlsx-file</param>
        /// <returns></returns>
        public static IEnumerable<worksheet> Worksheets(string ExcelFileName)
        {
            FileStream fs = new FileStream(ExcelFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Worksheets(fs);
        }

        /// <summary>
        /// All worksheets in the Excel workbook deserialized
        /// </summary>
        /// <param name="ExcelFileStream">Stream of the Excel xlsx-file</param>
        /// <returns></returns>
        public static IEnumerable<worksheet> Worksheets(Stream ExcelFileStream)
        {
            worksheet ws;

            using (ZipArchive zipArchive = new ZipArchive(ExcelFileStream, ZipArchiveMode.Read))
            {
                SharedStrings = DeserializedZipEntry<sst>(GetZipArchiveEntry(zipArchive, @"xl/sharedStrings.xml"));
                var sheetNames = GetExcelSheetNames(zipArchive).GetEnumerator();
                var worksheets = GetWorkSheetFileNames(zipArchive);
                foreach (var worksheetEntry in worksheets)
                {
                    ws = DeserializedZipEntry<worksheet>(worksheetEntry);
                    ws.NumberOfColumns = worksheet.MaxColumnIndex + 1;
                    ws.ExpandRows();
                    if (sheetNames.MoveNext())
                        ws.Name = sheetNames.Current;
                    yield return ws;
                }
            }
        }

        /// <summary>
        /// Method converting an Excel cell value to a date
        /// </summary>
        /// <param name="ExcelCellValue"></param>
        /// <returns></returns>
        public static DateTime DateFromExcelFormat(string ExcelCellValue)
        {
            return DateTime.FromOADate(Convert.ToDouble(ExcelCellValue));
        }

        private static ZipArchiveEntry GetZipArchiveEntry(ZipArchive ZipArchive, string ZipEntryName)
        {
            return ZipArchive.Entries.First<ZipArchiveEntry>(n => n.FullName.Equals(ZipEntryName));
        }
        private static List<ZipArchiveEntry> GetWorkSheetFileNames(ZipArchive zipArchive)
        {
            return zipArchive.Entries
                             .Where(entry => entry.FullName.StartsWith("xl/worksheets/sheet"))
                             .OrderBy(entry =>
                             {
                                 // Attempt to parse the integer part of the filename.
                                 string fileName = entry.FullName;
                                 int startIndex = "xl/worksheets/sheet".Length;
                                 int endIndex = fileName.IndexOf(".xml", startIndex);
                                 string numberPart = fileName.Substring(startIndex, endIndex - startIndex);

                                 // Convert it to an integer.
                                 if (int.TryParse(numberPart, out int sheetNumber))
                                 {
                                     return sheetNumber;
                                 }
                                 return int.MaxValue; // In case of any parsing failure, place this item at the end.
                             })
                             .ToList();
        }
        private static IEnumerable<string> GetExcelSheetNames(ZipArchive zipArchive)
        {
            var workbookEntry = zipArchive.Entries.FirstOrDefault(entry => entry.FullName.Equals("xl/workbook.xml"));

            if (workbookEntry != null)
            {
                using (Stream stream = workbookEntry.Open())
                {
                    XDocument doc = XDocument.Load(stream);
                    var sheetNames = doc.Descendants()
                                        .Where(e => e.Name.LocalName == "sheet")
                                        .Select(sheet => (string)sheet.Attribute("name"))
                                        .ToList();

                    return sheetNames;
                }
            }

            // Return an empty enumerable if the workbook.xml file is not found
            return Enumerable.Empty<string>();
        }

        private static T DeserializedZipEntry<T>(ZipArchiveEntry ZipArchiveEntry)
        {
            using (Stream stream = ZipArchiveEntry.Open())
                return (T)new XmlSerializer(typeof(T)).Deserialize(XmlReader.Create(stream));
        }
    }
}
