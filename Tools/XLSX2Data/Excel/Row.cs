using System.Xml.Serialization;

namespace Excel
{
    /// <summary>
    /// (c) 2014 Vienna, Dietmar Schoder
    /// 
    /// Code Project Open License (CPOL) 1.02
    /// 
    /// Deals with an Excel row
    /// </summary>
    public class Row
    {
        [XmlElement("c")]
        public Cell[] FilledCells;
        [XmlIgnore]
        public Cell[] Cells;

        public void ExpandCells(int NumberOfColumns)
        {
            Cells = new Cell[NumberOfColumns];
            foreach (var cell in FilledCells)
                Cells[cell.ColumnIndex] = cell;
            FilledCells = null;
        }
    }
}
