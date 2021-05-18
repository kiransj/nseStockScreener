using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helper;
using TinyCsvParser;
using TinyCsvParser.Mapping;

namespace NseData 
{
    internal class CSVEquityInformation: CsvMapping<EquityInformation>
    {
        public CSVEquityInformation() : base()
        {
            MapProperty(0, x => x.Symbol);
            MapProperty(1, x => x.CompanyName);
            MapProperty(2, x => x.Series);
            MapProperty(3, x => x.DateOfListing);
            MapProperty(4, x => x.PaidUpValue);
            MapProperty(5, x => x.MarketLot);
            MapProperty(6, x => x.IsinNumber);
            MapProperty(7, x => x.FaceValue);
        }
    }

    internal class CSVCompanyToIndustryMapping: CsvMapping<CompanyToIndustryMapping>
    {
        public CSVCompanyToIndustryMapping() : base()
        {
            MapProperty(0, x => x.CompanyName);
            MapProperty(1, x => x.Industry);
            MapProperty(2, x => x.Symbol);
            MapProperty(3, x => x.Series);
            MapProperty(4, x => x.IsinNumber);
        }
    }

    internal class CSVETFInformation: CsvMapping<ETFInformation>
    {
        public CSVETFInformation() : base()
        {
            MapProperty(0, x => x.Symbol);
            MapProperty(1, x => x.Underlying);
            MapProperty(2, x => x.ETFName);
            MapProperty(3, x => x.DateOfListing);
            MapProperty(4, x => x.MarketLot);
            MapProperty(5, x => x.IsinNumber);
            MapProperty(6, x => x.FaceValue);
        }
    }

    internal class CSVPledgedSummary: CsvMapping<PledgedSummary>
    {
        public CSVPledgedSummary() : base()
        {
            MapProperty(0, x => x.CompanyName);
            MapProperty(1, x => x.TotalNumberOfShares);
            MapProperty(2, x => x.PromotersShares);
            MapProperty(5, x => x.PromotersSharesPledged);
            MapProperty(8, x => x.DisclosureDate);
            MapProperty(9, x => x.totalPledgedShares);
        }
    }

    internal class CSVIndexDailyData : CsvMapping<IndexDailyData>
    {
        public CSVIndexDailyData(): base()
        {
            MapProperty(0, x => x.IndexName);
            MapProperty(1, x => x.TradeDate);
            MapProperty(2, x => x.OpenValue);
            MapProperty(3, x => x.HighValue);
            MapProperty(4, x => x.LowValue);
            MapProperty(5, x => x.CloseValue);
            MapProperty(6, x => x.PointsChange);
            MapProperty(7, x => x.PointsChangePct);
            MapProperty(8, x => x.Volume);
            MapProperty(9, x => x.TurnOverinCr);
            MapProperty(10, x => x.PE);
            MapProperty(11, x => x.PB);
            MapProperty(12, x => x.DivYield);
        }
    }

    internal class CSVEquityDailyData : CsvMapping<EquityDailyData>
    {
        public CSVEquityDailyData(): base() 
        {
            MapProperty(0, x => x.Symbol);
            MapProperty(1, x => x.Series);
            MapProperty(2, x => x.Open);
            MapProperty(3, x => x.High);
            MapProperty(4, x => x.Low);
            MapProperty(5, x => x.Close);
            MapProperty(6, x => x.Last);
            MapProperty(7, x => x.PrevClose);
            MapProperty(8, x => x.TotTradedQty);
            MapProperty(9, x => x.TotTradedValue);
            MapProperty(10, x => x.TimeStamp);
            MapProperty(11, x => x.TotalTrades);
            MapProperty(12, x => x.ISINNumber);
        }
    }


    internal class CSVMTODailyData : CsvMapping<MTODailyData>
    {
        public CSVMTODailyData(): base() 
        {
            MapProperty(2, x => x.Symbol);
            MapProperty(3, x => x.Series);
            MapProperty(5, x => x.DeliverableQuantity);
        }
    }

    internal class NseDataParser
    {
        private static Logger log = Logger.GetLoggerInstance();

        public List<IndexDailyData> ParseIndexDailyDataFile(string filename)
        {
            CsvParser<IndexDailyData> csvParser = new CsvParser<IndexDailyData>(new CsvParserOptions(true, ','), new CSVIndexDailyData());
            log.Debug($"Parsing Index daily data for file {filename}");            
            var result = csvParser.ReadFromFile(filename, Encoding.ASCII)
                            .Select(x => x.Result)
                            .Where(x => x != null)
                            .ToList();
            log.Debug($"Number of indexes {result.Count()}");

            // Print the information
            var output = string.Empty;
            result.ForEach(x => output += (x.ToString() + "\n"));
            log.Debug("indexes daily data\n" + output);

            return result;
        }
        
        public List<EquityInformation> ParseEquityInformationFile(string filename)
        {
            CsvParser<EquityInformation> csvParser = new CsvParser<EquityInformation>(new CsvParserOptions(true, ','), new CSVEquityInformation());
            log.Debug($"Parsing NSE Company Information CSV file {filename}");            
            var result = csvParser.ReadFromFile(filename, Encoding.ASCII)
                            .Select(x => x.Result)
                            .Where(x => x != null)
                            .ToList();
            log.Debug($"Number of companies {result.Count()}");

            // Print the information
            var output = string.Empty;
            result.ForEach(x => output += (x.ToString() + "\n"));
            log.Debug2("list of companies\n" + output);

            return result;
        }

        public List<ETFInformation> ParseETFInformationFile(string filename)
        {
            CsvParser<ETFInformation> csvParser = new CsvParser<ETFInformation>(new CsvParserOptions(true, ','), new CSVETFInformation());
            log.Debug($"Parsing NSE ETF Information CSV file {filename}");

            var result = csvParser.ReadFromFile(filename, Encoding.ASCII)
                            .Select(x => x.Result)
                            .Where(x => x != null)
                            .ToList();

            log.Debug($"Number of ETF's {result.Count()}");

            // Print the information
            var output = string.Empty;
            result.ForEach(x => output += (x.ToString() + "\n"));
            log.Debug2("list of ETF's\n" + output);

            return result;
        }

        public List<CompanyToIndustryMapping> ParseCompanyToIndustryMappingFile(string filename)
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CSVCompanyToIndustryMapping csvMapper = new CSVCompanyToIndustryMapping();
            CsvParser<CompanyToIndustryMapping> csvParser = new CsvParser<CompanyToIndustryMapping>(csvParserOptions, csvMapper);

            log.Debug($"Parsing industry Mapping CSV file {filename}");
            var result = csvParser.ReadFromFile(filename, Encoding.ASCII)
                            .Select(x => x.Result)
                            .Where(x => x != null)
                            .ToList();
            log.Debug($"Industry Mapping CSV file {filename} has {result.Count()} enteries");

            // Print the information
            var output = string.Empty;
            result.ForEach(x => output += (x.ToString() + "\n"));
            log.Debug2("Industry Mapping \n" + output);
            return result;
        }

        public List<PledgedSummary> ParsePledgedSummaryFile(string fileName)
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CSVPledgedSummary csvMapper = new CSVPledgedSummary();
            CsvParser<PledgedSummary> csvParser = new CsvParser<PledgedSummary>(csvParserOptions, csvMapper);

            log.Debug($"Parsing NSE Pledged Summary CSV file {fileName}");
            var result = csvParser.ReadFromFile(fileName, Encoding.ASCII)
                            .Select(x => x.Result)
                            .Where(x => x != null)
                            .ToList();
            log.Debug($"Pledged Summary CSV file has {result.Count()} enteries");

            // Print the information
            var output = string.Empty;
            result.ForEach(x => output += (x.ToString() + "\n"));
            log.Debug2("Pledged Summaries\n" + output);
            return result;
        }

        public List<EquityDailyData> ParseEquityDailyData(string fileName)
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CSVEquityDailyData csvMapper = new CSVEquityDailyData();
            CsvParser<EquityDailyData> csvParser = new CsvParser<EquityDailyData>(csvParserOptions, csvMapper);

            log.Debug($"Parsing Equity Daily data CSV file {fileName}");
            var result = csvParser.ReadFromFile(fileName, Encoding.ASCII)
                            .Select(x => x.Result)
                            .Where(x => x != null)
                            .ToList();
            log.Debug($"Equity Daily data file has {result.Count()} enteries");

            // Print the information
            var output = string.Empty;
            result.ForEach(x => output += (x.ToString() + "\n"));
            log.Debug2("Equity Daily data \n" + output);
            return result;
        }


        public List<MTODailyData> ParseMTODailyData(string fileName)
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CSVMTODailyData csvMapper = new CSVMTODailyData();
            CsvParser<MTODailyData> csvParser = new CsvParser<MTODailyData>(csvParserOptions, csvMapper);

            log.Debug($"Parsing MTO data CSV file {fileName}");
            var result = csvParser.ReadFromFile(fileName, Encoding.ASCII)
                            .Select(x => x.Result)
                            .Where(x => x != null)
                            .ToList();
            log.Debug($"MTODailyData file has {result.Count()} enteries");

            // Print the information
            var output = string.Empty;
            result.ForEach(x => output += (x.ToString() + "\n"));
            log.Debug2("MTODailyData data \n" + output);
            return result;
        }
    }
}