using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

namespace XmlTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string xmlPath = @"test_data.xml";

            // テストデータの生成
            generateTestXml(xmlPath);

            // テスト
            TestMain(xmlPath);
        }

        /// <summary>
        /// xmlSerializer を使用してテストデータ(xmlファイル)を作成
        /// </summary>
        /// <param name="xmlPath">出力先xmlファイル</param>
        static void generateTestXml(string xmlPath)
        {
            // テストデータの作成
            GUIFormat dataRoot = new GUIFormat();

            dataRoot.ADvProjPath = "project path";
            dataRoot.MonInterval = 1500;

            const int tabCount = 10;
            const int paramCntPerTab = 10;

            for (int i = 0; i < tabCount; ++i)
            {
                Tab tab = new Tab();
                tab.contents = "tab" + (i + 1).ToString();

                for (int j = 0; j < paramCntPerTab; ++j)
                {
                    tab.Parameters.Add(
                        new Parameter()
                        {
                            key = ("param" + ((i + 1) * 10 + (j + 1)).ToString()),
                            value = (i + 1) * 10M + (j + 1) * 0.1M,
                            unit = "deg"
                        }
                    );
                }

                dataRoot.Tabs.Add(tab);
            }

            // xmlファイルへ出力
            XmlSerializer serializer = new XmlSerializer(typeof(GUIFormat));
            try
            {
                using (TextWriter writer = new StreamWriter(xmlPath))
                {
                    serializer.Serialize(writer, dataRoot);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Main: Error: xmlファイルへの出力中にエラーが発生しました。");
                Console.WriteLine("     " + exp.Message);
            }
        }

        /// <summary>
        /// テストの実行関数
        /// </summary>
        /// <param name="xmlPath"></param>
        static void TestMain(string xmlPath)
        {
            List<KeyValuePair<string, string>> testCaseList = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>( "tab1", "param11" ),
                new KeyValuePair<string, string>( "tab2", "param21" ),
                new KeyValuePair<string, string>( "tab1", "param13" ),
                new KeyValuePair<string, string>( "tab2", "param23" )
            };

            long calcTime;
            List<long> calcTimeList = new List<long>();
            List<List<decimal>> valueList = new List<List<decimal>>();

            //--------------------------------------------------------------//
            // Linq による読み込みテスト
            //--------------------------------------------------------------//
            List<decimal> linqValueList = new List<decimal>();
            calcTime = LinqTestMain(xmlPath, testCaseList, ref linqValueList);

            valueList.Add(linqValueList);
            calcTimeList.Add(calcTime);

            //--------------------------------------------------------------//
            // XMLserializer による読み込みテスト
            //--------------------------------------------------------------//
            List<decimal> serializeValueList = new List<decimal>();
            calcTime = SerializeTestMain(xmlPath, testCaseList, ref serializeValueList);

            valueList.Add(serializeValueList);
            calcTimeList.Add(calcTime);


            //--------------------------------------------------------------//
            // 結果の表示
            //--------------------------------------------------------------//
            for(int i = 0; i < testCaseList.Count; ++i )
            {
                Console.WriteLine("----------");
                Console.WriteLine("TestCase: tab={0}, param={1}", testCaseList[i].Key, testCaseList[i].Value );
                Console.WriteLine("  SerializeTest: " + valueList[1][i].ToString());
                Console.WriteLine("  LingTest:      " + valueList[0][i].ToString());
                Console.WriteLine("");
            }

            Console.WriteLine("----------");
            Console.WriteLine("Time [msec]: ");
            Console.WriteLine("  SerializeTest: " + calcTimeList[1].ToString());
            Console.WriteLine("  LinqTest:      " + calcTimeList[0].ToString());
        }

        /// <summary>
        /// XMLserializerによるXML読み込みテストのmain関数
        /// </summary>
        /// <param name="xmlPath">xmlファイルへのパス [-] (-)</param>
        /// <param name="testCaseList">テストケースのリスト [-] (-)</param>
        /// <param name="valueList">xmlから読み込んだvalueの格納先 [-] (-)</param>
        /// <returns>xml読み込み/データ取得にかかった時間 [msec] (-)</returns>
        static long SerializeTestMain( string xmlPath, List<KeyValuePair<string, string>> testCaseList, ref List<decimal> valueList)
        {
            Stopwatch stopWatch = new Stopwatch();
            XmlSerializeTest serializeTest = new XmlSerializeTest();

            try
            {
                stopWatch.Start();
                serializeTest.init(xmlPath);
                stopWatch.Stop();

                foreach (var testCase in testCaseList)
                {
                    stopWatch.Start();
                    decimal value = serializeTest.getValue(testCase.Key, testCase.Value);
                    stopWatch.Stop();

                    valueList.Add(value);
                }

                serializeTest.OutputToXmlFile("serializeTest_output.xml");
            }
            catch (Exception exp)
            {
                Console.WriteLine("SerializeTest: Error: " + exp.Message);
            }

            return stopWatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// LinqによるXML読み込みテストのmain関数
        /// </summary>
        /// <param name="xmlPath">xmlファイルへのパス [-] (-)</param>
        /// <param name="testCaseList">テストケースのリスト [-] (-)</param>
        /// <param name="valueList">xmlから読み込んだvalueの格納先 [-] (-)</param>
        /// <returns>xml読み込み/データ取得にかかった時間 [msec] (-)</returns>
        static long LinqTestMain(string xmlPath, List<KeyValuePair<string, string>> testCaseList, ref List<decimal> valueList)
        {
            Stopwatch stopWatch = new Stopwatch();
            XmlLinqTest linqTest = new XmlLinqTest();

            try
            {
                stopWatch.Start();
                linqTest.init(xmlPath);
                stopWatch.Stop();

                foreach (var testCase in testCaseList)
                {
                    stopWatch.Start();
                    decimal value = linqTest.getValue(testCase.Key, testCase.Value);
                    stopWatch.Stop();

                    valueList.Add(value);
                }

                linqTest.OutputToXmlFile("linqTest_output.xml");
            }
            catch (Exception exp)
            {
                Console.WriteLine("LinqTest: Error: " + exp.Message);
            }

            return stopWatch.ElapsedMilliseconds;
        }
    }
}
