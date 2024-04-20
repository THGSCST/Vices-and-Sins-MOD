using System;
using System.Xml.Serialization;

namespace Excel
{
    /// <summary>
    /// (c) 2014 Vienna, Dietmar Schoder
    /// 
    /// Code Project Open License (CPOL) 1.02
    /// 
    /// Deals with an Excel worksheet in an xlsx-file
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
    [XmlRoot("worksheet", Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
    public class worksheet
    {
        [XmlArray("sheetData")]
        [XmlArrayItem("row")]
        public Row[] Rows;

        public string Name;
        [XmlIgnore]
        public int NumberOfColumns; // Total number of columns in this worksheet

        public static int MaxColumnIndex = 0; // Temporary variable for import

        public worksheet()
        {
        }

        public void ExpandRows()
        {
            foreach (var row in Rows)
                row.ExpandCells(NumberOfColumns);
        }
    }
}
