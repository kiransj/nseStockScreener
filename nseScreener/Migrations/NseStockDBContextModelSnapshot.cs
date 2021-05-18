﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StockDatabase;

namespace nseScreener.Migrations
{
    [DbContext(typeof(NseStockDBContext))]
    partial class NseStockDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.5");

            modelBuilder.Entity("StockDatabase.EquityDailyTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("Close")
                        .HasColumnType("REAL");

                    b.Property<int>("CompanyId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("DeliverableQuantity")
                        .HasColumnType("INTEGER");

                    b.Property<double>("High")
                        .HasColumnType("REAL");

                    b.Property<double>("Last")
                        .HasColumnType("REAL");

                    b.Property<double>("Low")
                        .HasColumnType("REAL");

                    b.Property<double>("Open")
                        .HasColumnType("REAL");

                    b.Property<double>("PrevClose")
                        .HasColumnType("REAL");

                    b.Property<long>("TotTradedQty")
                        .HasColumnType("INTEGER");

                    b.Property<double>("TotTradedValue")
                        .HasColumnType("REAL");

                    b.Property<long>("TotalTrades")
                        .HasColumnType("INTEGER");

                    b.Property<int>("day")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("day");

                    b.HasIndex("CompanyId", "day")
                        .IsUnique();

                    b.ToTable("EquityDailyTable");
                });

            modelBuilder.Entity("StockDatabase.EquityInformationTable", b =>
                {
                    b.Property<int>("CompanyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateOfListing")
                        .HasColumnType("TEXT");

                    b.Property<double>("FaceValue")
                        .HasColumnType("REAL");

                    b.Property<string>("ISINNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Industry")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsETF")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MarketLot")
                        .HasColumnType("INTEGER");

                    b.Property<double>("PaidUpValue")
                        .HasColumnType("REAL");

                    b.Property<string>("Series")
                        .HasColumnType("TEXT");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Underlying")
                        .HasColumnType("TEXT");

                    b.HasKey("CompanyId");

                    b.HasIndex("ISINNumber")
                        .IsUnique();

                    b.HasIndex("Industry");

                    b.HasIndex("Symbol");

                    b.ToTable("CompanyInformation");
                });

            modelBuilder.Entity("StockDatabase.IndexDailyDataTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("CloseValue")
                        .HasColumnType("REAL");

                    b.Property<double>("DivYield")
                        .HasColumnType("REAL");

                    b.Property<double>("HighValue")
                        .HasColumnType("REAL");

                    b.Property<int>("IndexId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("LowValue")
                        .HasColumnType("REAL");

                    b.Property<double>("OpenValue")
                        .HasColumnType("REAL");

                    b.Property<double>("PB")
                        .HasColumnType("REAL");

                    b.Property<double>("PE")
                        .HasColumnType("REAL");

                    b.Property<double>("PointsChange")
                        .HasColumnType("REAL");

                    b.Property<double>("PointsChangePct")
                        .HasColumnType("REAL");

                    b.Property<double>("TurnOverinCr")
                        .HasColumnType("REAL");

                    b.Property<double>("Volume")
                        .HasColumnType("REAL");

                    b.Property<int>("day")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("IndexId");

                    b.HasIndex("day");

                    b.HasIndex("IndexId", "day")
                        .IsUnique();

                    b.ToTable("IndexDailyDataTable");
                });

            modelBuilder.Entity("StockDatabase.IndexInformationTable", b =>
                {
                    b.Property<int>("IndexId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("IndexName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("IndexId");

                    b.HasIndex("IndexName")
                        .IsUnique();

                    b.ToTable("IndexInformation");
                });

            modelBuilder.Entity("StockDatabase.PledgedSummaryTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DisclosureDate")
                        .HasColumnType("TEXT");

                    b.Property<long>("PromotersShares")
                        .HasColumnType("INTEGER");

                    b.Property<long>("PromotersSharesPledged")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TotalNumberOfShares")
                        .HasColumnType("INTEGER");

                    b.Property<long>("totalPledgedShares")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CompanyName");

                    b.ToTable("PledgedSummary");
                });
#pragma warning restore 612, 618
        }
    }
}
