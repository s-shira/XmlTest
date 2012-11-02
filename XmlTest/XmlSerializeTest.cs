using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace XmlTest
{
    /// <summary>
    /// XmlSerializerによる xml読み込みテスト用クラス
    /// </summary>
    class XmlSerializeTest
    {
        #region メンバ
        /// <summary>
        /// 読み込んだXMLファイルへのパス [-] (-)
        /// </summary>
        private string _xmlPath;

        /// <summary>
        /// XMLファイルから読み込んだデータ内容 [-] (-)
        /// </summary>
        private GUIFormat _data;
        #endregion

        /// <summary>
        /// テストの初期化：xmlPathファイルからXMLSerializerを使用してデータ読み込み
        /// </summary>
        /// <param name="xmlPath">xmlファイルへのパス [-] (-)</param>
        public void init(string xmlPath)
        {
            using (FileStream reader = new FileStream(xmlPath, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GUIFormat));
                _data = (GUIFormat)serializer.Deserialize(reader);
                _xmlPath = xmlPath;
            }
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

#if false
            // 自分でリストを回して検索する方法
            if (null != _data)
            {
                foreach (var tab in _data.Tabs)
                {
                    if (tabName == tab.contents)
                    {
                        foreach (var param in tab.Parameters)
                        {
                            if (paramName == param.key)
                            {
                                value = param.value;
                                hitFlag = true;

                                // 値の書き換え
                                param.value = -value;
                                break;
                            }
                        }
                    }

                    if (true == hitFlag)
                    {
                        break;
                    }
                }
            }
#endif

#if false
            // List.Findメソッドを利用して検索する方法
            Tab tab = null;
            Parameter param = null;
            if (null != _data)
            {
                tab = _data.Tabs.Find(tabNode => tabNode.contents == tabName);
            }
            if (null != tab)
            {
                param = tab.Parameters.Find(paramNode => paramNode.key == paramName);
            }

            if (null != param)
            {
                value = param.value;
                hitFlag = true;

                // 値の書き換え
                param.value = -value;
            }
#endif

#if true
            // Linqによって検索する方法
            var query = from tab in _data.Tabs
                        where tab.contents == tabName
                        from param in tab.Parameters
                        where param.key == paramName
                        select param;
            foreach (var elem in query)
            {
                value = elem.value;
                hitFlag = true;

                // 値の書き換え
                elem.value = -value;
            }
#endif
            if (false == hitFlag) throw new Exception("Invalid tab/param name");
            return value;
        }

        /// <summary>
        /// データをxmlファイルへ出力
        /// </summary>
        /// <param name="xmlPath">出力先のxmlファイルパス [-] (-)</param>
        public void OutputToXmlFile(string xmlPath)
        {
            using (TextWriter writer = new StreamWriter(xmlPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GUIFormat));
                serializer.Serialize(writer, _data);
            }
        }
    }
}
