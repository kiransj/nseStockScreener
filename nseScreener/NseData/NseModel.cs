using System;

namespace NseData
{
    //https://www1.nseindia.com/content/equities/EQUITY_L.csv
    public class EquityInformation
    {
        public string Symbol { get; set; }
        public string CompanyName { get; set; }
        public string Series { get; set; }
        public DateTime DateOfListing { get; set; }
        public double PaidUpValue { get; set; }
        public int MarketLot { get; set; }
        public string IsinNumber { get; set; }
        public double FaceValue { get; set; }

        public override string ToString()
        {
            var result = string.Empty;
            result = $"{Symbol},{CompanyName},{Series},{IsinNumber}";
            return result;
        }
    }

    //https://www.nseindia.com/content/equities/eq_etfseclist.csv
    public class ETFInformation
    {
        public string Symbol { get; set; }
        public string Underlying { get; set; }
        public string ETFName { get; set; }
        public DateTime DateOfListing { get; set; }
        public int MarketLot { get; set; }
        public string IsinNumber { get; set; }
        public double FaceValue { get; set; }

        public override string ToString()
        {
            var result = string.Empty;
            result = $"{Symbol},{ETFName},{Underlying},{IsinNumber}";
            return result;
        }
    }

    //https://www.nseindia.com/content/indices/ind_nifty500list.csv
    public class CompanyToIndustryMapping
    {
        public string CompanyName {get; set; }
        public string Industry {get; set; }
        public string Symbol {get; set; }
        public string Series {get; set; }
        public string IsinNumber {get; set; }

        public override string ToString()
        {
            var result = string.Empty;
            result = $"{Symbol},{Industry},{IsinNumber}";
            return result;
        }
    }

    public class PledgedSummary
    {
        public string CompanyName { get; set; }
        public string TotalNumberOfShares { get; set; }
        public string PromotersShares { get; set; }     
        public string PromotersSharesPledged { get; set; }
        public string DisclosureDate { get; set; }
        public string totalPledgedShares { get; set; }

        public PledgedSummary() 
        {
            TotalNumberOfShares = string.Empty;
            PromotersShares = string.Empty;
            PromotersSharesPledged = string.Empty;
            DisclosureDate = string.Empty;
            totalPledgedShares = string.Empty;
        }

        public override string ToString()
        {
            var result = string.Empty;
            result = $"{CompanyName},{TotalNumberOfShares},{totalPledgedShares},{DisclosureDate}";
            return result;
        }
    }


    public class IndexDailyData
    {
        public string IndexName {  get; set; }
        public string TradeDate { get; set; }
        public string OpenValue { get; set; }
        public string HighValue { get; set; }
        public string LowValue { get; set; }
        public string CloseValue { get; set; }
        public string PointsChange { get; set; }
        public string PointsChangePct { get; set; }
        public string Volume { get; set; }
        public string TurnOverinCr { get; set; }
        public string PE { get; set; }
        public string PB { get; set; }
        public string DivYield { get; set; }

        public override string ToString()
        {
            var result = string.Empty;
            result = $"{IndexName},{CloseValue},{TurnOverinCr},{Volume},{DivYield}";
            return result;
        }
    }

    public class EquityDailyData
    {
        public string Symbol { get; set; }
        public string Series { get; set; }
        public string Open { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
        public string Close { get; set; }
        public string Last { get; set; }
        public string PrevClose { get; set; }
        public string TotTradedQty { get; set; }
        public string TotTradedValue { get; set; }
        public string TimeStamp { get; set; }
        public string TotalTrades { get; set; }
        public string ISINNumber { get; set; }
        public string DeliverableQuantity { get; set; }

        public override string ToString()
        {
            var result = string.Empty;
            result = $"{Symbol},{Close},{PrevClose},{TotTradedQty},{TotalTrades},D{DeliverableQuantity}";
            return result;
        }
    }

    public class MTODailyData
    {
        public string Symbol { get; set; }
        public string Series { get; set; }
        public string DeliverableQuantity { get; set; }

        public override string ToString()
        {
            var result = string.Empty;
            result = $"{Symbol},{Series},{DeliverableQuantity}";
            return result;
        }
    }
}