using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Helper
{
    public class NseURLs
    {
        public string EquityListUrl { get; set; }
        public string ETFListUrl { get; set; }
        public List<string> Indexes {get; set; }
        public string BhavUrl { get; set; }
        public string IndexBhavUrl { get; set; }
        public string DeliveryPositionUrl { get; set; }
        public string PledgedSummaryUrl {get; set;}

        // For any new public item added to this class
        // Update function NseDailyData::normalizeNseURLs to add the new item to normalize URL list
        // Update function NseURLs::listOfUrls to add the new item to list


        public NseURLs() 
        {
            Indexes = new List<string>();
        }

        public override string ToString()
        {
            string result = string.Empty;

            result += $"NseURLS\n";
            result += $"\tEquityListURL = {EquityListUrl}\n";
            result += $"\tETFListUrl = {ETFListUrl}\n";
            result += $"\tIndexBhavUrl = {IndexBhavUrl}\n";
            result += $"\tBhavUrl = {BhavUrl}\n";
            result += $"\tDeliveryPositionUrl = {DeliveryPositionUrl}\n";
            result += $"\tPledgedSummary = {PledgedSummaryUrl}\n";
            result += $"\tNumber ofIndexes = {Indexes.Count}\n";
            return result;
        }

        public List<string> listOfUrls() 
        {
            var urls = new List<string>();
            urls.Add(EquityListUrl);
            urls.Add(ETFListUrl);
            urls.Add(BhavUrl);
            urls.Add(IndexBhavUrl);
            urls.Add(DeliveryPositionUrl);
            urls.Add(PledgedSummaryUrl);

            return urls;
        }

        public List<string> listOfIndexUrls() 
        {
            return Indexes;
        }
    }

    public class AppOptions
    {        
        public string tmpFolderPath { get; set; } //Folder to unzip files
        public string logFileName { get; set; }        
        public LogLevel logLevel { get; set; }        
        public string dbPath { get; set; }
        public List<KeyValuePair<string, string>> httpHeaders;
        public NseURLs nseUrls = new NseURLs();

        public AppOptions()
        {            
            tmpFolderPath = null;
            httpHeaders = new List<KeyValuePair<string, string>>();
        }

        public override string ToString()
        {
            string result = string.Empty;
            string output = string.Empty;
            foreach(var item in httpHeaders) {
                output += $"{item.Key, 12} => {item.Value}\n";
            }
            result += "\n";
            result += $"LogLevel: {logLevel}\n";
            result += $"http Options:\n{output}";            
            result += $"{nseUrls.ToString()}";
            return result;
        }
    }

    public class Options
    {
        private static readonly object padlock = new object();
        private static AppOptions options = null;
        private static string fileName = "";
        private static Logger log = Logger.GetLoggerInstance();
        static public AppOptions app { get { return options; } }
        Options()
        {

        }

        static public void ReadOptionsFromFile(string filename)
        {
            log.Info($"Reading Options from file '{filename}'");
            if (options == null)
            {
                var text = File.ReadAllText(filename);
                lock (padlock)
                {
                    if (options == null)
                    {
                        try 
                        {
                            // Convert JsonFile to AppOptions Object
                            options = JsonConvert.DeserializeObject<AppOptions>(text);

                            //Sanitize the input configuration
                            if(!Directory.Exists(options.tmpFolderPath)) 
                            {
                                log.Error($"Tmp folder '{options.tmpFolderPath}' does not exists!");
                                Environment.Exit(1);
                            }
                            
                            // If logFileName is not empty 
                            if(!string.IsNullOrEmpty(options.logFileName))
                            {
                                log.LogToFile(options.logFileName);
                            }
                        }
                        catch(Exception ex) 
                        {
                            log.Fatal($"Error while parsing '{filename}'. {ex.Message}");
                        }
                        Options.fileName = filename;
                        log.SetLogLevel(options.logLevel);
                        log.Debug(options.ToString());
                    }
                }
            }
            else
            {
                log.Error($"Trying to Initialize options twice. Options already initialized with contents of file {fileName}");
                throw new Exception($"Trying to Initialize options twice. Options initialized with contents of fiel {fileName}");
            }
        }
    }
}