using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Linq;

namespace XmlTest
{
    /// <summary>
    /// XmlLinqによるXML読み込みテスト
    /// </summary>
    class XmlLinqTest
    {
        #region メンバ変数
        /// <summary>
        /// 読み込んだXMLファイルへのパス [-] (-)
        /// </summary>
        private string _xmlPath;

        /// <summary>
        /// XMLルートノード(xelement) [-] (-)
        /// </summary>
        private XElement _dataRoot;
        #endregion

        /// <summary>
        /// テストの初期化：xmlPathファイルからデータ読み込み
        /// </summary>
        /// <param name="xmlPath">xmlファイルへのパス [-] (-)</param>
        public void init(string xmlPath)
        {
            _dataRoot = XElement.Load(xmlPath);
        }

        /// <summary>
        /// tabName, paramNameで特定される値(value)を取得する。
        /// パラメータが特定されない場合は例外(Exception:Message = "Invalid tab/param name")をthrowする。
        /// </summary>
        /// <param name="tabName">タブ名 [-] (-)</param>
        /// <param name="paramName">パラメータ名 [-] (-)</param>
        /// <returns>値 [-] (-)</returns>
        public decimal getValue(string tabName, string paramName)
        {
            bool hitFlag = false;
            decimal value = 0.0M;

            if ( null != _dataRoot )
            {
                var query = from tab in _dataRoot.Elements("Tab")
                            where tab.Attribute("contents").Value == tabName
                            from param in tab.Elements("Parameter")
                            where param.Attribute("key").Value == paramName
                            //select param;
                            select param.Attribute("value");

                foreach (var elem in query)
                {
                    if (true == decimal.TryParse(elem.Value, out value))
                    {
                        hitFlag = true;

                        // 値の書き換え
                        elem.Value = (-value).ToString();
                    }
                    break;

                }
            }

            if (false == hitFlag) throw new Exception("Invalid tab/param name");
            return value;
        }

        /// <summary>
        /// データをxmlファイルへ出力
        /// </summary>
        /// <param name="xmlPath">出力先のxmlファイルパス [-] (-)</param>
        public void OutputToXmlFile(string xmlPath)
        {
            _dataRoot.Save(xmlPath);
        }
    }
}
