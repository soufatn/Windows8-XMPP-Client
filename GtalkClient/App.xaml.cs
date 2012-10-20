using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace GtalkClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static App Instance;
        public static String Directory ="../";
        private String _DefaultStyle = "MyStyle.xaml";

        public App()
        {
            Instance = this;
            Directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string stringsFile = Path.Combine(Directory, "Styles", _DefaultStyle);
            LoadStyleDictionaryFromFile(stringsFile);
        }

        /// <summary>
        /// This funtion loads a ResourceDictionary from a file at runtime
        /// </summary>
        public void LoadStyleDictionaryFromFile(string inFileName)
        {
            if (File.Exists(inFileName))
            {
                try
                {
                    using (var fs = new FileStream(inFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        // Read in ResourceDictionary File
                        var dic = (ResourceDictionary)XamlReader.Load(fs);
                        // Clear any previous dictionaries loaded
                        Resources.MergedDictionaries.Clear();
                        // Add in newly loaded Resource Dictionary
                        Resources.MergedDictionaries.Add(dic);
                    }
                }
                catch
                {
                }
            }
            else {
                Console.WriteLine("Bad File." + inFileName);
            }
        }
    }
}
