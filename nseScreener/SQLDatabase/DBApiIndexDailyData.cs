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
        private IndexDailyDataTable IndexInformationToTable(IndexDailyData data, int indexId)
        {
            var et = new IndexDailyDataTable();
            
            et.CloseValue = ConversionHelper.convertToDouble(data.CloseValue);
            et.OpenValue = ConversionHelper.convertToDouble(data.OpenValue);
            et.HighValue = ConversionHelper.convertToDouble(data.HighValue);
            et.LowValue = ConversionHelper.convertToDouble(data.LowValue);
            et.PointsChange = ConversionHelper.convertToDouble(data.PointsChange);
            et.PointsChangePct = ConversionHelper.convertToDouble(data.PointsChangePct);
            et.Volume = ConversionHelper.convertToDouble(data.Volume);
            et.TurnOverinCr = ConversionHelper.convertToDouble(data.TurnOverinCr);
            et.PE = ConversionHelper.convertToDouble(data.PE);
            et.PB = ConversionHelper.convertToDouble(data.PB);
            et.DivYield = ConversionHelper.convertToDouble(data.DivYield);
            et.date = DateTime.ParseExact(data.TradeDate, "dd-MM-yyyy", null);
            et.IndexId = indexId;
            return et;
        }

        private async Task UpdateIndexData(NseParsedData data)
        {
            var list = data.indexDailyDatas.ToDictionary(x => x.IndexName, x => true);
            await stockDatabase.IndexInformation.ForEachAsync(x => {
                if(list.ContainsKey(x.IndexName))
                    list.Remove(x.IndexName);
            });
            
            if(list.Count > 0) {
                foreach(var item in list) {log.Info($"Adding index {item.Key} to database");}
                await stockDatabase.IndexInformation.AddRangeAsync(list.Select(x => new IndexInformationTable() { IndexName = x.Key }));
                await SaveDataBaseChanges();
            }

            var mapping = await stockDatabase.IndexInformation.ToDictionaryAsync(x => x.IndexName, x => x.IndexId);
            var indexDataList = data.indexDailyDatas.Select(x => IndexInformationToTable(x, mapping[x.IndexName]));

            await stockDatabase.IndexDailyDataTable.AddRangeAsync(indexDataList);
            log.Info($"Added {indexDataList.Count()} rows to index Daily data table");
        }
     }
}