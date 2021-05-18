using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helper;
using Microsoft.EntityFrameworkCore;
using NseData;


/*
    dotnet ef migrations add StockDatabaseCreate
    dotnet ef database update
*/

namespace StockDatabase
{
    public partial class StockDBApi 
    {
        private static Logger log = Logger.GetLoggerInstance();
        private NseStockDBContext stockDatabase;

        public StockDBApi(string dbPath = null)
        {
            stockDatabase = new NseStockDBContext(true, dbPath);
        }

        private async Task<int> SaveDataBaseChanges() 
        {
            log.Info("Saving changes to Database");
            try 
            {
                int numberOfRowsUpdated = await stockDatabase.SaveChangesAsync();
                log.Database($"Updated {numberOfRowsUpdated} rows in database");
                return numberOfRowsUpdated;
            }
            catch(DbUpdateException ex)
            {
                log.Database($"failed to update database due to error : {ex.InnerException.Message}");
                log.Error($"{ex.InnerException.Message}");
                throw new Exception("failed to update DB");                
            }
            catch(Exception ex) 
            {
                log.Error($"{ex.Message}");
                throw new Exception("failed to update DB");                
            }
        }
        
        public async Task<int> updateDatabase(NseParsedData nseData)
        {
            await UpdatePledgedSharesInformation(nseData);
            await UpdateCompanyInformation(nseData);

            // Commit the change related to updating Company information
            // before update trade related details
            await SaveDataBaseChanges();
            
            await UpdateEquityDailyData(nseData);
            await UpdateIndexData(nseData);
            return await SaveDataBaseChanges();
        }
    }
}