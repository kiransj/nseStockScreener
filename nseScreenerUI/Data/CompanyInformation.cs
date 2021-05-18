
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nseScreenerUI.Data
{
    public class CompanyInformation
    {
        public int companyId { get; set; }
        public string symbol { get; set; }
        public string companyName { get; set; }
        public string series { get; set; }
        public bool isETF { get; set; }
        public string isinNumber { get; set; }
        public string underlying { get; set; }
        public string industry { get; set; }
        public long TotalNumberOfShares { get; set; }
        public long PromotersShares { get; set; }
        public long PromotersSharesPledged { get; set; }
        public long totalPledgedShares { get; set; }
        public long promotorOwnerShipPct { 
            get {
                return TotalNumberOfShares == 0 ? 0 : ((PromotersShares*100)/TotalNumberOfShares);
            }
        }

        public long promotorPledgedPct { 
            get {
                return PromotersShares == 0 ? 0 : ((PromotersSharesPledged*100)/PromotersShares);
            }
        }

        public long totalPledgedPct { 
            get {
                return TotalNumberOfShares == 0 ? 0 : ((totalPledgedShares*100)/TotalNumberOfShares);
            }
        }

        public CompanyInformation() 
        {
            TotalNumberOfShares = 0;
            PromotersShares = 0;
            PromotersSharesPledged = 0;
            totalPledgedShares = 0;
        }
    }

    public partial class NseStockDataBaseService
    {
        private Dictionary<int, CompanyInformation> companyIdToCompanyMapping = null;
        private async Task<Dictionary<int, CompanyInformation>> GetCompanyIdToCompanyMapping()
        {
            var listOfCompanies = await GetListOfCompanies();
            companyIdToCompanyMapping = listOfCompanies.ToDictionary(x => x.companyId, x => x);

            return companyIdToCompanyMapping;
        }

        public async Task<CompanyInformation> GetCompanyDetails(int companyId)
        {

            if(companyIdToCompanyMapping != null && companyIdToCompanyMapping.ContainsKey(companyId)) 
            {
                return companyIdToCompanyMapping[companyId];
            }
            await GetCompanyIdToCompanyMapping();
            
            if(companyIdToCompanyMapping.ContainsKey(companyId)) 
            {
                return companyIdToCompanyMapping[companyId];
            }
            return null;
        }
        public async Task<IEnumerable<CompanyInformation>> GetListOfCompanies()
        {
            var listOfCompanies = await nseService.GetListOfCompanies();
            var pledgedData = await nseService.GetLatestPledgedData();
            var mapping = pledgedData.ToDictionary(x => x.CompanyName, x => x);

            var list = new List<CompanyInformation>();
            listOfCompanies.ForEach(x => {
                var c = new CompanyInformation() {
                    companyId = x.CompanyId,
                    companyName = x.CompanyName,
                    industry = x.Industry,
                    isETF = x.IsETF,
                    underlying = x.Underlying,
                    isinNumber = x.ISINNumber,
                    symbol = x.Symbol,
                    series = x.Series
                };
                if(mapping.ContainsKey(c.companyName))
                {
                    var pd = mapping[c.companyName];
                    c.PromotersShares = pd.PromotersShares;
                    c.PromotersSharesPledged = pd.PromotersSharesPledged;
                    c.TotalNumberOfShares = pd.TotalNumberOfShares;
                    c.totalPledgedShares = pd.totalPledgedShares;
                }
                list.Add(c);
            });
            return list;
        }

        public string convertValuetoString(double value)
        {
            double oneLakh = 100000.0;
            double res = value/oneLakh;
            if(res < 100.0) 
            {
                return string.Format($"{res.ToString("#.00")}L");
            }
            else 
            {
                res = res/100.0;
                return string.Format($"{res.ToString("#.00")}C");
            }        
        }
    }
}