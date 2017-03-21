using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;


namespace JsonAnalyzer
{

    class ScriptLibraryContext
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public List<string> Scripts { get; set; }
    }

    class ScriptContext
    {
        public string Category { get; set; }

        public string ResultType { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }


        public bool CarveDeleted { get; set; }

        public List<string> Packages { get; set; }

        public List<string> LogicalPaths { get; set; }

        public string Priority { get; set; }

        public bool StopOnFail { get; set; }

        public enum ParallelType
        {
            False,
            True,
            Primary
        }

        public ParallelType Parallel { get; set; }
    }

    class ScriptDescription
    {
        public List<ScriptLibraryContext> ScriptsLibrary { get; set; }

        public List<ScriptContext> Scripts { get; set; }

        public ScriptDescription() { Scripts = new List<ScriptContext>(); ScriptsLibrary = new List<ScriptLibraryContext>(); }
    }

    class JsonDictionary
    {
        private ScriptDescription m_iOS;
        private ScriptDescription m_android;

        public void ExtractAppData(string sAndroidFilePath, string siOsFilePath)
        {
            var androidStr = File.ReadAllText(sAndroidFilePath);
            var iOsStr = File.ReadAllText(siOsFilePath);

            m_android = JsonConvert.DeserializeObject<ScriptDescription>(androidStr);
            m_iOS = JsonConvert.DeserializeObject<ScriptDescription>(iOsStr);
        }



        public void ShowSummary()
        {
           
            int overlapCount = 0;
            foreach (var androidApp in m_android.Scripts)
            {
                foreach (var iosApp in m_iOS.Scripts)
                {
                    if (androidApp.Name.Substring(0, androidApp.Name.IndexOf('.') + 1).Equals(iosApp.Name.Substring(0, iosApp.Name.IndexOf('.') + 1)))
                    {
                        overlapCount++;
                    }
      
                }
            }
            Console.WriteLine("iOS : {0} / Android : {1} / Overlap : {2}\n", m_iOS.Scripts.Count(), m_android.Scripts.Count(), overlapCount);
        }

        public void ShowListByResultType(string sInputResultType, string sOsType)
        {
            int count = 0;

            if (sOsType.Equals("iOS"))
            {
                foreach (var app in m_iOS.Scripts)
                {
                    if (app.ResultType.Contains(sInputResultType))
                    {
                        Console.WriteLine("{0}\n", app.Name);

                        count++;
                    }
                }
            }
            else
            {
                foreach (var app in m_android.Scripts)
                {
                    if (app.ResultType.Contains(sInputResultType))
                    {
                        Console.WriteLine("{0}\n", app.Name);
                        
                        count++;
                    }
                }
            }
            
            Console.WriteLine("\n\n================================\n");
            Console.WriteLine("Found APP number : {0}\n", count);
        }
        

        public static void Main(string[] args)
        {
            var jsonInstance = new JsonDictionary();


            jsonInstance.ExtractAppData("/Android_Apps.json", "/iOS.json");

            int cmd ;

            do
            {
                Console.WriteLine("========================================\n");
                Console.WriteLine("Select Option.\n");
                Console.WriteLine("1. Show summary\n");
                Console.WriteLine("2. Show List of Specific Result Type \n");
                Console.WriteLine("3. End program \n");
                Console.WriteLine("========================================\n");

                cmd = int.Parse(Console.ReadLine());

                switch (cmd)
                {
                    case 1:
                        jsonInstance.ShowSummary();
                        break;
                    case 2:
                        Console.WriteLine("========================================\n");
                        Console.WriteLine("\nSelect OS.\n");
                        Console.WriteLine("1. iOS\n");
                        Console.WriteLine("2. Andriod\n");
                        

                        int os = int.Parse(Console.ReadLine());
                        Console.WriteLine("========================================\n");
                        string strOs;
                        if (os == 1)
                        {
                            strOs = "iOS";
                        }
                        else if (os == 2)
                        {
                            strOs = "android";
                        }
                        else
                        {
                            Console.WriteLine("\nInvalid input.\n");
                            break;
                        }
                        Console.WriteLine("========================================\n");
                        Console.WriteLine("\n\nInput Result Type : ");
                        string strResultType = Console.ReadLine();
                        Console.WriteLine("========================================\n");
                        jsonInstance.ShowListByResultType(strResultType, strOs);
                        break;
                    default:
                        break;
                }
            } while (cmd != 3);
          
        }
    }
}
