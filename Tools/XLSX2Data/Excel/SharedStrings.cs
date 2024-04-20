using System;
using System.Xml;
using System.Xml.Serialization;

namespace Excel
{
    /// <summary>
    /// (c) 2014 Vienna, Dietmar Schoder
    /// 
    /// Code Project Open License (CPOL) 1.02
    /// 
    /// Handles a "shared strings XML-file" in an Excel xlsx-file
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
    [XmlRoot("sst", Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
    public class sst
    {
        [XmlAttribute]
        public string uniqueCount;
        [XmlAttribute]
        public string count;
        [XmlElement("si")]
        public SharedString[] si;

        public sst()
        {
        }
    }
    public class SharedString
    {
        public string t;
    }
}
