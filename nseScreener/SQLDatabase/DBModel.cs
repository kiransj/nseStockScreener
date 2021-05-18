using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

/*
    dotnet tool update --global dotnet-ef
    dotnet ef migrations add StockDatabaseCreate
    dotnet ef database update
*/
namespace StockDatabase
{

    public class EquityInformationTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompanyId { get; set; }
        [Required]
        public string Symbol {get; set;}
        [Required]
        public string CompanyName {get; set;}
        [Required]
        public string ISINNumber {get; set;}
        [Required]
        public double FaceValue {get; set;}
        [Required]
        public int MarketLot {get; set;}
        [Required]
        public DateTime DateOfListing {get; set;}
        [Required]
        public double PaidUpValue {get; set;}
        [Required]
        public bool IsETF { get; set; }
        [Required]
        public string Industry {get; set; }

        // Not mandatory Items
        public string Underlying { get; set; }
        public string Series {get; set;}

        public EquityInformationTable() {
            IsETF = false;
            Industry = "UNKNOWN";
            DateOfListing = DateTime.ParseExact("01-01-2000", "dd-MM-yyyy", null);
        }
    }

    public class IndexInformationTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IndexId { get; set; }
        [Required]
        public string IndexName { get; set;}
    }

    public class IndexDailyDataTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int IndexId { get; set; }
        [Required]
        public int day { get; set; }
        [Required]
        public double CloseValue { get; set; }
        public double OpenValue { get; set; }
        public double  HighValue { get; set; }
        public double LowValue { get; set; }
        public double PointsChange { get; set; }
        public double PointsChangePct { get; set; }
        public double Volume { get; set; }
        public double TurnOverinCr { get; set; }
        public double PE { get; set; }
        public double PB { get; set; }
        public double DivYield { get; set; }

        [NotMapped]
        public DateTime date {
            get {
                return DateHelper.DayToDate(day);
            }
            set {
                day = DateHelper.DateToDay(value);
            }
        }
    }

    public class PledgedSummaryTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public long TotalNumberOfShares { get; set; }
        public long PromotersSharesPledged { get; set; }
        public long PromotersShares { get; set; }
        public DateTime DisclosureDate { get; set; }
        public long totalPledgedShares { get; set; }
    }

    public class EquityDailyTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        public double Open { get; set; }
        [Required]
        public double High { get; set; }
        [Required]
        public double Low { get; set; }
        [Required]
        public double Close { get; set; }
        [Required]
        public double Last { get; set; }
        [Required]
        public double PrevClose { get; set; }
        [Required]
        public long TotTradedQty { get; set; }
        [Required]
        public double TotTradedValue { get; set; }
        [Required]
        public int day { get; set; }
        [Required]
        public long TotalTrades { get; set; }
        [Required]
        public long DeliverableQuantity { get; set; }

        [NotMapped]
        public DateTime date {
            get 
            {
                return DateHelper.DayToDate(day);
            }
            set 
            {
                day = DateHelper.DateToDay(value);
            }
        }

        [NotMapped]
        public long deliverableQuantityPct {  
            get {
                return (DeliverableQuantity*100)/TotTradedQty;
            }
        }
    }

    public class NseStockDBContext : DbContext
    {
        private static Logger log = Logger.GetLoggerInstance();
        public DbSet<EquityInformationTable> CompanyInformation { get; set; }
        public DbSet<IndexInformationTable>  IndexInformation { get; set; }
        public DbSet<PledgedSummaryTable>  PledgedSummary { get; set; }
        public DbSet<IndexDailyDataTable>  IndexDailyDataTable { get; set; }
        public DbSet<EquityDailyTable>  EquityDailyTable { get; set; }

        private const string dbFilename = "nseStockdb.db";
        private string dbFilePath = dbFilename;

        public NseStockDBContext(bool strictCheck = false, string dbPath = null)
        {
            dbFilePath = dbFilename;
            
            if(!string.IsNullOrEmpty(dbPath)) 
            {
                dbFilePath = $"{dbPath}/{dbFilename}";
            }            
            if(File.Exists(dbFilePath))
            {
                FileInfo s = new FileInfo(dbFilePath);
                log.Info($"DbFile '{dbFilePath}', Size: {s.Length/1024}KB, Modified: {s.LastWriteTime}");                                    
            }
            else if(strictCheck == true)
            {
                log.Error($"DBFile {dbFilePath} not found");
                throw new Exception($"DBFile {dbFilePath} not found");
            }            

        }

        public NseStockDBContext(DbContextOptions<NseStockDBContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = string.Format($"Data Source={dbFilePath}");
            log.Info($"connectionString = {connectionString}");
            optionsBuilder.UseSqlite(connectionString);
            optionsBuilder.EnableSensitiveDataLogging(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<EquityInformationTable>().HasIndex(x => new {x.Symbol, x.Series}).IsUnique();
            modelBuilder.Entity<EquityInformationTable>().HasIndex(x => x.Symbol);
            modelBuilder.Entity<EquityInformationTable>().HasIndex(x => x.Industry);
            modelBuilder.Entity<EquityInformationTable>().HasIndex(x => x.ISINNumber).IsUnique();
            modelBuilder.Entity<IndexInformationTable>().HasIndex(x => x.IndexName).IsUnique();
            modelBuilder.Entity<PledgedSummaryTable>().HasIndex(x => x.CompanyName);

            modelBuilder.Entity<IndexDailyDataTable>().HasIndex(x => x.IndexId);
            modelBuilder.Entity<IndexDailyDataTable>().HasIndex(x => x.day);
            modelBuilder.Entity<IndexDailyDataTable>().HasIndex(x => new{x.IndexId, x.day}).IsUnique();
            
            modelBuilder.Entity<EquityDailyTable>().HasIndex(x => x.CompanyId);
            modelBuilder.Entity<EquityDailyTable>().HasIndex(x => x.day);
            modelBuilder.Entity<EquityDailyTable>().HasIndex(x => new{x.CompanyId, x.day}).IsUnique();
        }
    }
}