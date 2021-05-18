using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockDatabase;
using System;
namespace nseScreenerUI.Data
{
    public class ValueHistory
    {
        public DateTime dateTime {get; set; }
        public double value { get; set; }
    }
    public class BhavInformation
    {
        public string industry { get; set; }
        public string symbol { get; set;}
        public string series { get; set;}
        public int companyId {get; set; }
        public double Open { get; set; }        
        public double High { get; set; }        
        public double Low { get; set; }        
        public double Close { get; set; }        
        public double Last { get; set; }        
        public double PrevClose { get; set; }        
        public long TotTradedQty { get; set; }        
        public double TotTradedValue { get; set; }
        public long TotalTrades { get; set; }        
        public long deliverableQuantity { get; set; }
        public long deliverableQuantityPct {  
            get {
                return (deliverableQuantity*100)/TotTradedQty;
            }
        }

        public double priceChangePct {
            get {
                return ((Close - PrevClose)*100.0)/PrevClose;
            }
        }
        BhavInformation() 
        {
            this.series = "Not Known";
            this.symbol = "Not Known";
        }
        public BhavInformation(EquityDailyTable e, ref Dictionary<int, CompanyInformation> mapping)
        {            
            companyId = e.CompanyId;
            Open = e.Open;
            High = e.High;
            Low = e.Low;
            Close = e.Close;
            Last = e.Last;
            PrevClose = e.PrevClose;
            TotTradedQty = e.TotTradedQty;
            TotTradedValue = e.TotTradedValue;
            TotalTrades = e.TotalTrades;
            deliverableQuantity = e.DeliverableQuantity;
            if(mapping.TryGetValue(e.CompanyId, out CompanyInformation x))
            {
                this.symbol =  x.symbol;
                this.series =  x.series;
                this.industry = x.industry;
            }
        }
    }
    public partial class NseStockDataBaseService
    {
        public async Task<string> GetLastUpdatedDate()
        {
            try 
            {
                var d = await nseService.GetLastUpdatedDate();
                return d.ToLongDateString();
            }
            catch(Exception) 
            {
                return "Database is empty";
            }
        }

        public async Task<IEnumerable<BhavInformation>> GetBhavForDate(DateTime date)
        {                        
            var mapping = await GetCompanyIdToCompanyMapping();
            var d = await nseService.GetBhavForDate(date);


            return d.Select(x => new BhavInformation(x, ref mapping));
        }

        public async Task<IEnumerable<ValueHistory>> GetPriceGraphForCompany(int CompanyId)
        {                        
            var d = await nseService.GetBhavForCompany(CompanyId);

            return d.Select(x => new ValueHistory() { dateTime = x.date, value = x.Close });
        }

        public async Task<IEnumerable<ValueHistory>> GetDeliveryQtyForCompany(int CompanyId)
        {                        
            var d = await nseService.GetBhavForCompany(CompanyId);            

            return d.Select(x => new ValueHistory() { dateTime = x.date, value = x.DeliverableQuantity });
        }

        public async Task<IEnumerable<ValueHistory>> GetTotalTradedQtyForCompany(int CompanyId)
        {                        
            var d = await nseService.GetBhavForCompany(CompanyId);            

            return d.Select(x => new ValueHistory() { dateTime = x.date, value = x.TotTradedQty });
        }
    }

}