using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NseData;

namespace StockDatabase
{
     public partial class StockDBApi 
     {
        private EquityDailyTable EquityDataToTable(EquityDailyData data, DateTime date, int companyId)
        {
            var et = new EquityDailyTable();

            et.CompanyId = companyId;            
            et.date = date;
            et.Open = ConversionHelper.convertToDouble(data.Open);
            et.High = ConversionHelper.convertToDouble(data.High);
            et.Low = ConversionHelper.convertToDouble(data.Low);
            et.Close = ConversionHelper.convertToDouble(data.Close);
            et.Last = ConversionHelper.convertToDouble(data.Last);
            et.PrevClose = ConversionHelper.convertToDouble(data.PrevClose);
            et.TotTradedQty = ConversionHelper.convertToLong(data.TotTradedQty);
            et.TotTradedValue = ConversionHelper.convertToDouble(data.TotTradedValue);            
            et.TotalTrades = ConversionHelper.convertToLong(data.TotalTrades);
            et.DeliverableQuantity = ConversionHelper.convertToLong(data.DeliverableQuantity);
            
            return et;
        }

        private async Task UpdateEquityDailyData(NseParsedData data)
        {
            var list = data.equityDailyData.Where(x => x.Series != "BL").ToDictionary(x => x.ISINNumber, x => x);
            await stockDatabase.CompanyInformation.ForEachAsync(x => {                
                if(list.ContainsKey(x.ISINNumber))
                    list.Remove(x.ISINNumber);
            });

            int count = 1;
            var companyList = list.Values.Select(x => new EquityInformationTable() {
                Symbol = x.Symbol,
                Series = $"{x.Series}_CUSTOM_{count++}",
                CompanyName = x.Symbol,
                ISINNumber = x.ISINNumber,
                DateOfListing = data.dateTime
            });

            log.Info($"{companyList.Count()} new companies found in equity data will be added to DB");

            await stockDatabase.CompanyInformation.AddRangeAsync(companyList);
            await SaveDataBaseChanges();
            
            /*foreach(var item in companyList) 
            {
                log.Info($"{item.CompanyName} {item.Series}");
                await stockDatabase.CompanyInformation.AddAsync(item);
                await SaveDataBaseChanges();
            }*/
            
            
            var companyIDMapping = await stockDatabase.CompanyInformation.ToDictionaryAsync(x => x.ISINNumber, x => x.CompanyId);
            var equityTablelist = data.equityDailyData.Where(x => x.Series != "BL").Select(x => EquityDataToTable(x, data.dateTime, companyIDMapping[x.ISINNumber]));

            log.Info($"Adding equity data for date {data.dateTime.ToShortDateString()}. Number of rows {equityTablelist.Count()}");
            await stockDatabase.EquityDailyTable.AddRangeAsync(equityTablelist);
            await SaveDataBaseChanges();
        }
     }
}