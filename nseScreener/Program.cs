using System;
using System.Threading.Tasks;
using Helper;
using NseData;
using StockDatabase;
using System.Threading;

namespace nseScreener
{
    class Program
    {
        private static Logger log = Logger.GetLoggerInstance();
        static void Main(string[] args)
        {
            NseStockService nse  = new NseStockService("options.json");
            nse.Initialize();
            nse.UpdateDataToToday().Wait();
        }
    }
}
