using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XmlTest
{
    /// <summary>
    /// GUIパラメータクラス
    /// </summary>
    public class Parameter
    {
        [XmlAttribute]
        public string key;

        [XmlAttribute]
        public decimal value;

        [XmlAttribute]
        public string unit;
    }

    /// <summary>
    /// Tab設定クラス
    /// </summary>
    public class Tab
    {
        [XmlAttribute]
        public string contents;

        [XmlElement("Parameter")]
        public List<Parameter> Parameters;

        public Tab()
        {
            Parameters = new List<Parameter>();
        }
    }

    /// <summary>
    /// GUIFormatクラス
    /// </summary>
    [XmlRoot("GUIFormat")]
    public class GUIFormat
    {
        public string ADvProjPath;
        public int MonInterval;

        [XmlElement("Tab")]
        public List<Tab> Tabs;

        public GUIFormat()
        {
            Tabs = new List<Tab>();
        }
    }
}
