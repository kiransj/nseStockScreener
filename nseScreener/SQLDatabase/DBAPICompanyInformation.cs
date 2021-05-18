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
        private async Task UpdateCompanyInformation(NseParsedData nseData)
        {
            var industryMapping = new Dictionary<string, string>();
            var equityMapping = new Dictionary<string, EquityInformationTable>();

            // Build a dictionary of IsinNumber to Industry.
            // The same company might exist in multiple indexes. Hence do a tryAdd to avoid exception.
            nseData.industryMappings.ForEach(x => {industryMapping.TryAdd(x.IsinNumber, x.Industry);});

            // Convert nseData equity list to EquityInformation Table. 
            // If the industry mapping exists update it.
            nseData.equityList.ForEach(x => {                
                string industry = industryMapping.ContainsKey(x.IsinNumber) ? industryMapping[x.IsinNumber] : null;                
                equityMapping.Add(x.IsinNumber, CompanyInformationToTable(x, industry));
            });

            // Convert nseData ETF list to EquityInformation Table. 
            // If the industry mapping exists update it.
            nseData.etfList.ForEach(x => equityMapping.Add(x.IsinNumber, ETFInformationToTable(x)));

            // For each row in companyinformation table compute the difference and udpate the DB with only the difference
            foreach(var item in stockDatabase.CompanyInformation)
            {
                if(equityMapping.TryGetValue(item.ISINNumber, out var x))
                {                    
                    if(item.Symbol != x.Symbol) {
                        //Update the symbol name
                        log.Warn($"updating symbol from {item.Symbol} -> {x.Symbol}");
                        item.Symbol = x.Symbol;
                    }

                    if(item.Series != x.Series) {
                        //Update the symbol name
                        log.Warn($"updating Series ({item.Symbol}) from {item.Series} -> {x.Series}");
                        item.Series = x.Series;
                    }

                    if(item.FaceValue != x.FaceValue) {
                        //Update the symbol name
                        log.Warn($"updating FaceValue ({item.Symbol}) from {item.FaceValue} -> {x.FaceValue}");
                        item.FaceValue = x.FaceValue;
                    }

                    if(item.CompanyName != x.CompanyName) {
                        //Update the symbol name
                        log.Warn($"updating CompanyName from {item.CompanyName} -> {x.CompanyName}");
                        item.CompanyName = x.CompanyName;
                    }

                    // If the company already exists then remove it from mapping.
                    equityMapping.Remove(item.ISINNumber);
                }
            }

            // The companies which remain on mapping until now means they are new companies and needs to be added to DB.
            var companyList = equityMapping.Select(x => x.Value).ToList();
            if(companyList.Count > 0 ) {
                companyList.ForEach(x => { log.Info($"Adding new company {x.Symbol} to DB");});
                await stockDatabase.CompanyInformation.AddRangeAsync(companyList);
            }
        }

        private async Task UpdatePledgedSharesInformation(NseParsedData nseData)
        {
            var pledgedSummaryMapping = new Dictionary<string, PledgedSummaryTable>();
            nseData.pledgedSummary.ForEach(x => pledgedSummaryMapping.TryAdd(x.CompanyName, PledgedSummaryToTable(x)));

            // Pledged data is saved as a new row into DB for existing company if there is any change in the data
            // This is will help in plot the change in pledged data
            var dbPledgedMapping = new Dictionary<string, PledgedSummaryTable>();
            await stockDatabase.PledgedSummary.OrderByDescending(x => x.Id).ForEachAsync(x => dbPledgedMapping.TryAdd(x.CompanyName, x));
            foreach(var item in dbPledgedMapping.Values)
            {
                if(pledgedSummaryMapping.TryGetValue(item.CompanyName, out var x))
                {
                    // Check if there is a change pledged shares summary
                    if(!DiffPledgedSharesCompare(item, x)) 
                    {
                        pledgedSummaryMapping.Remove(item.CompanyName);
                    }
                    else
                    {
                        log.Info($"Pledged Shares for company '{item.CompanyName}' has changed");
                    }                 
                }
            }

            // Add the companies for which pledged data has chaned to DB
            var pledgedSummaryList = pledgedSummaryMapping.Select(x => x.Value).ToList();
            if(pledgedSummaryList.Count > 0 ) {
                pledgedSummaryList.ForEach(x => { log.Info($"Adding pledged summary for '{x.CompanyName}' to DB");});
                await stockDatabase.PledgedSummary.AddRangeAsync(pledgedSummaryList);
            }
        }

        private EquityInformationTable CompanyInformationToTable(EquityInformation e, string industry = null)
        {
            var et = new EquityInformationTable();
            et.Symbol = e.Symbol;
            et.CompanyName = e.CompanyName;
            et.ISINNumber = e.IsinNumber;
            et.MarketLot = e.MarketLot;
            et.PaidUpValue = e.PaidUpValue;
            et.Series = e.Series;
            et.DateOfListing = e.DateOfListing;
            et.FaceValue = e.FaceValue;
            et.IsETF = false;
            et.Underlying = "";
            et.Industry = industry ?? "UNKNOWN";
            return et;
        }

        private EquityInformationTable ETFInformationToTable(ETFInformation e)
        {
            var et = new EquityInformationTable();
            et.Symbol = e.Symbol;
            et.CompanyName = e.ETFName;
            et.ISINNumber = e.IsinNumber;
            et.MarketLot = e.MarketLot;
            et.DateOfListing = e.DateOfListing;
            et.FaceValue = e.FaceValue;
            et.IsETF = true;
            et.Underlying = e.Underlying;
            et.PaidUpValue = 0;
            et.Series = "ETF";
            et.Industry = "ETF";
            return et;
        }

        private PledgedSummaryTable PledgedSummaryToTable(PledgedSummary x)
        {
            var item = new PledgedSummaryTable();
            item.CompanyName = x.CompanyName;
            item.DisclosureDate = x.DisclosureDate.Length < 3 ? DateTime.Parse("01-01-2000") : DateTime.Parse(x.DisclosureDate);
            item.PromotersShares = ConversionHelper.convertToLong(x.PromotersShares);
            item.PromotersSharesPledged = ConversionHelper.convertToLong(x.PromotersSharesPledged);
            item.TotalNumberOfShares = ConversionHelper.convertToLong(x.TotalNumberOfShares);
            item.totalPledgedShares = ConversionHelper.convertToLong(x.totalPledgedShares);            
            return item;
        }

        private bool DiffPledgedSharesCompare(PledgedSummaryTable x1, PledgedSummaryTable x2) 
        {
            if(x1.DisclosureDate.CompareTo(x2.DisclosureDate) != 0) return true;
            if(x1.PromotersShares != x2.PromotersShares) return true;
            if(x1.PromotersSharesPledged != x2.PromotersSharesPledged) return true;
            if(x1.TotalNumberOfShares != x2.TotalNumberOfShares) return true;
            if(x1.totalPledgedShares != x2.totalPledgedShares) return true;
            return false;
        }
     }
}