using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helper;
using Microsoft.EntityFrameworkCore;
using NseData;

namespace StockDatabase
{
    public partial class StockDBApi
    {
        public async Task<DateTime> GetLastUpdateDate()
        {
            var day = await stockDatabase.EquityDailyTable.MaxAsync(x => x.day);            
            return DateHelper.DayToDate(day);
        }

        public async Task<List<EquityInformationTable>> GetListOfCompanies()
        {
            var listOfCompanies = await stockDatabase.CompanyInformation.ToListAsync();
            return listOfCompanies;
        }

        public async Task<List<IndexInformationTable>> GetListOfIndex()
        {
            var listOfCompanies = await stockDatabase.IndexInformation.ToListAsync();
            return listOfCompanies;
        }

        public async Task<List<PledgedSummaryTable>> GetLatestPledgedData()
        {
            var data = await stockDatabase.PledgedSummary.OrderByDescending(x => x.Id).ToListAsync();
            var mapping = data.GroupBy(x => x.CompanyName).Select(x => x.OrderByDescending(x => x.Id).First()).ToList();
            return mapping;
        }

        public async Task<List<EquityDailyTable>> GetBhavForDate(DateTime date)
        {
            int day = DateHelper.DateToDay(date);
            var data = await stockDatabase.EquityDailyTable.Where(x => x.day == day).ToListAsync();
            return data;
        }

        public async Task<List<EquityDailyTable>> GetBhavForCompany(int CompanyId)
        {            
            var data = await stockDatabase.EquityDailyTable.Where(x => x.CompanyId == CompanyId).OrderBy(x => x.day).ToListAsync();
            return data;
        }
    }
}