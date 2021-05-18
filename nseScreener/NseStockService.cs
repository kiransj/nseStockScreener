using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Helper;
using NseData;
using StockDatabase;

namespace nseScreener
{
    public class NseStockService
    {
        private static Logger log = Logger.GetLoggerInstance();
        private string configFileName;
        private StockDBApi stockDatabase = null;

        public NseStockService(string configFileName)
        {
            this.configFileName = configFileName;
            stockDatabase = null;
        }

        public void Initialize()
        {
            if (!File.Exists(this.configFileName))
            {
                log.Fatal($"Config file {this.configFileName} does not exist");
            }
            try
            {
                Options.ReadOptionsFromFile(this.configFileName);
                stockDatabase = new StockDBApi(Options.app.dbPath);
            }
            catch (Exception ex)
            {
                log.printException(ex);
                Environment.Exit(2);
            }
        }

        private async Task<DateTime> GetLastUpdatedDateFromDB()
        {
            try
            {
                var date = await stockDatabase.GetLastUpdateDate();
                log.Info($"DB udpated until {date.ToShortDateString()}");
                return date;
            }
            catch(Exception)
            {
                log.Error("Unable to get last Updated Date. Might be Database is empty");
                return DateTime.ParseExact("01-04-2021", "dd-MM-yyyy", null);
            }
        }

        public async Task UpdateDataToToday()
        {
            var date = await GetLastUpdatedDateFromDB();
            for(var d = date.AddDays(1); d <= DateTime.Now; d = d.AddDays(1))
            {
                if(d.DayOfWeek == DayOfWeek.Sunday || d.DayOfWeek == DayOfWeek.Saturday)
                {
                    /*Markets are closed on weekends*/
                    continue;
                }
                NseParsedData parsedData;
                try
                {
                    var nseData = new NseDailyData(d, Options.app.nseUrls);
                    await nseData.DownloadData(Options.app.tmpFolderPath, false);
                    parsedData = nseData.parseData(d);
                }
                catch(Exception)
                {
                    //log.printException(ex);
                    log.Error($"Fetching data for {d.ToLongDateString()} failed. Could be it was a holiday. Continuing");
                    continue;
                }

                try
                {
                    await stockDatabase.updateDatabase(parsedData);
                }
                catch(Exception ex)
                {
                    log.Fatal($"Error updating DB. {ex.Message}");
                }
            }

            log.Info($"Updated data upto '{DateTime.Now.ToLongDateString()}'");
        }


        public async Task<List<EquityInformationTable>> GetListOfCompanies(string series = null)
        {
            var listOfCompanies = await stockDatabase.GetListOfCompanies();
            if(!string.IsNullOrEmpty(series)) {
                listOfCompanies = listOfCompanies.Where(x => x.Series == series || x.IsETF).ToList();
            }
            return listOfCompanies;
        }

        public async Task<List<IndexInformationTable>> GetListOfIndex()
        {
            var listOfCompanies = await stockDatabase.GetListOfIndex();
            return listOfCompanies;
        }

        public async Task<List<PledgedSummaryTable>> GetLatestPledgedData()
        {
            return await stockDatabase.GetLatestPledgedData();
        }

        public async Task<DateTime> GetLastUpdatedDate()
        {            
            var date = await stockDatabase.GetLastUpdateDate();            
            return date;
        }

        public async Task<List<EquityDailyTable>> GetBhavForDate(DateTime date)
        {
            return await stockDatabase.GetBhavForDate(date);
        }

        public async Task<List<EquityDailyTable>> GetBhavForCompany(int CompanyId)
        {            
            var data = await stockDatabase.GetBhavForCompany(CompanyId);            
            return data;
        }
    }
}