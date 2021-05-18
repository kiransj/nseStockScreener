using System.Threading;
using nseScreener;

namespace nseScreenerUI.Data
{
    public partial class NseStockDataBaseService
    {
        private static NseStockService nseService = null;
        private static readonly object padlock = new object();

        public void Update()
        {                    
            new Thread(() => 
            {
                Thread.CurrentThread.IsBackground = true; 
                /* run your code here */ 
                nseService.UpdateDataToToday().Wait();
            }).Start();
            return;
        }

        public NseStockDataBaseService() 
        {
            if(nseService == null)
            {
                lock(padlock)
                {
                    if(nseService == null)
                    {
                        //nseService = new NseStockService("/mnt/c/source/nseScreener/options.json");
                        nseService = new NseStockService(Program.configFile);
                        nseService.Initialize();
                    }
                }
            }
        }
    }
}