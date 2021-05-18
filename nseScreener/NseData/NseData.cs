using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helper;

namespace NseData
{
    public class NseParsedData 
    {
        readonly public DateTime dateTime;
        readonly public List<EquityInformation> equityList;
        readonly public List<ETFInformation> etfList;
        readonly public List<CompanyToIndustryMapping> industryMappings;
        readonly public List<PledgedSummary> pledgedSummary;
        readonly public List<IndexDailyData> indexDailyDatas;
        readonly public List<EquityDailyData> equityDailyData;
        public NseParsedData(DateTime date,
                             List<EquityInformation> equityList, 
                             List<ETFInformation> etfList,
                             List<CompanyToIndustryMapping> industryMappings,
                             List<PledgedSummary> pledgedSummary,
                             List<IndexDailyData> indexData,
                             List<EquityDailyData> equityDailyData)
        {
            this.dateTime = date;
            this.equityList = equityList;
            this.etfList = etfList;
            this.industryMappings = industryMappings;
            this.pledgedSummary = pledgedSummary;
            this.indexDailyDatas = indexData;
            this.equityDailyData = equityDailyData;
        }

        public NseParsedData() {

        }
    }
    public class NseDailyData
    {
        private static Logger log = Logger.GetLoggerInstance();
        private NseURLs urls;
        private DateTime date;

        private Dictionary<string, string> urlToFileMapping;
        private Dictionary<string, string> indexUrlToFileMapping;

        private NseURLs normalizeNseURLs(DateTime date, NseURLs nseUrls) 
        {
            var formater = new URLFormater(date);
            var result = new NseURLs();
            
            result.EquityListUrl = formater.normalizeUrl(nseUrls.EquityListUrl);
            result.ETFListUrl = formater.normalizeUrl(nseUrls.ETFListUrl);
            result.BhavUrl = formater.normalizeUrl(nseUrls.BhavUrl);
            result.IndexBhavUrl = formater.normalizeUrl(nseUrls.IndexBhavUrl);
            result.DeliveryPositionUrl = formater.normalizeUrl(nseUrls.DeliveryPositionUrl);
            result.PledgedSummaryUrl = formater.normalizeUrl(nseUrls.PledgedSummaryUrl);
            nseUrls.Indexes.ForEach(x => { result.Indexes.Add(formater.normalizeUrl(x)); });
            return result;
        }

        public NseDailyData(DateTime date, NseURLs nseUrls)
        {
            this.date = date;
            urls = normalizeNseURLs(date, nseUrls);
            urlToFileMapping = new Dictionary<string, string>();
            indexUrlToFileMapping = new Dictionary<string, string>();
        }

        public async Task DownloadData(string tmpFolderPath, bool stubDownloads = false)
        {
            var fd = new FileDownloader();
            var listOfUrls = urls.listOfUrls();
            var listOfIndexUrls = urls.listOfIndexUrls();
            var tasks = new List<Task>();

            if(!stubDownloads) {
                FileSystemHelpers.DeleteAllFilesInFolder(tmpFolderPath);
            }

            log.Info($"Downloading NSE data for '{date.ToString("dd-MMM-yyyy")}' to folder '{tmpFolderPath}'");
            foreach(var url in listOfUrls) 
            {
                var filename = $"{tmpFolderPath}/{URLFormater.fileNameInURL(url)}";
                if(!stubDownloads) {await fd.DownloadAsFileAsync(url, filename, Options.app.httpHeaders);}                
                if(string.Compare(URLFormater.getFileExtension(filename), ".zip", true) == 0) {
                    log.Debug($"Extracting zip file {filename} to folder {tmpFolderPath}");
                    FileSystemHelpers.ExtractZipFile(filename, tmpFolderPath);
                    filename = filename.Replace(".zip", string.Empty);                    
                }
                urlToFileMapping.Add(url, filename);
            }

            log.Debug($"Download the list of indexes and associated companies");
            foreach(var url in listOfIndexUrls) 
            {
                var filename = $"{tmpFolderPath}/{URLFormater.fileNameInURL(url)}";                
                if(!stubDownloads) {await fd.DownloadAsFileAsync(url, filename, Options.app.httpHeaders);}
                indexUrlToFileMapping.Add(url, filename);
            }
            
            log.Info($"All Data downloaded for {date.ToLongDateString()}");
        }

        public NseParsedData parseData(DateTime date) 
        {
            var result = string.Empty;
            var parser = new NseDataParser();

            var listOfCompanies = parser.ParseEquityInformationFile(urlToFileMapping[urls.EquityListUrl]);
            var listOfETFs = parser.ParseETFInformationFile(urlToFileMapping[urls.ETFListUrl]);
            var listOfPledgedSummary = parser.ParsePledgedSummaryFile(urlToFileMapping[urls.PledgedSummaryUrl]);
            var indexDailyData = parser.ParseIndexDailyDataFile(urlToFileMapping[urls.IndexBhavUrl]);
            var equityDailyData = parser.ParseEquityDailyData(urlToFileMapping[urls.BhavUrl]);
            var mtoDailyData = parser.ParseMTODailyData(urlToFileMapping[urls.DeliveryPositionUrl]);

            // Update the delivery QTY in equityDaily data. The delivery QTY is available in the MTO file
            var mtoMapping = mtoDailyData.ToDictionary(x => new {x.Symbol, x.Series}, x => x.DeliverableQuantity);
            equityDailyData.ForEach(x => {
                if(mtoMapping.TryGetValue(new {x.Symbol, x.Series}, out var dQty)) {
                    x.DeliverableQuantity = dQty;
                }
            });

            // Parse all the index mapping files and create 
            // unique list of company to industry mapping
            var companyToIndustryMapping = new List<CompanyToIndustryMapping>();
            foreach(var item in indexUrlToFileMapping.Values)  {
                companyToIndustryMapping.AddRange(parser.ParseCompanyToIndustryMappingFile(item));
            }
            companyToIndustryMapping = companyToIndustryMapping.OrderBy(x => x.IsinNumber).Distinct().ToList();

            log.Info($"{date.ToShortDateString()},companies {listOfCompanies.Count()}, etf's {listOfETFs.Count()},"+
                      $"eq {equityDailyData.Count()}, index {indexDailyData.Count()}");

            return new NseParsedData(date, listOfCompanies, listOfETFs, companyToIndustryMapping, 
                                    listOfPledgedSummary, 
                                    indexDailyData, equityDailyData);
        }
    }
}