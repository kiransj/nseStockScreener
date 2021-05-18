using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace nseScreener.Migrations
{
    public partial class StockDatabaseCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyInformation",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    CompanyName = table.Column<string>(type: "TEXT", nullable: false),
                    ISINNumber = table.Column<string>(type: "TEXT", nullable: false),
                    FaceValue = table.Column<double>(type: "REAL", nullable: false),
                    MarketLot = table.Column<int>(type: "INTEGER", nullable: false),
                    DateOfListing = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaidUpValue = table.Column<double>(type: "REAL", nullable: false),
                    IsETF = table.Column<bool>(type: "INTEGER", nullable: false),
                    Industry = table.Column<string>(type: "TEXT", nullable: false),
                    Underlying = table.Column<string>(type: "TEXT", nullable: true),
                    Series = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyInformation", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "EquityDailyTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompanyId = table.Column<int>(type: "INTEGER", nullable: false),
                    Open = table.Column<double>(type: "REAL", nullable: false),
                    High = table.Column<double>(type: "REAL", nullable: false),
                    Low = table.Column<double>(type: "REAL", nullable: false),
                    Close = table.Column<double>(type: "REAL", nullable: false),
                    Last = table.Column<double>(type: "REAL", nullable: false),
                    PrevClose = table.Column<double>(type: "REAL", nullable: false),
                    TotTradedQty = table.Column<long>(type: "INTEGER", nullable: false),
                    TotTradedValue = table.Column<double>(type: "REAL", nullable: false),
                    day = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalTrades = table.Column<long>(type: "INTEGER", nullable: false),
                    DeliverableQuantity = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquityDailyTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndexDailyDataTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IndexId = table.Column<int>(type: "INTEGER", nullable: false),
                    day = table.Column<int>(type: "INTEGER", nullable: false),
                    CloseValue = table.Column<double>(type: "REAL", nullable: false),
                    OpenValue = table.Column<double>(type: "REAL", nullable: false),
                    HighValue = table.Column<double>(type: "REAL", nullable: false),
                    LowValue = table.Column<double>(type: "REAL", nullable: false),
                    PointsChange = table.Column<double>(type: "REAL", nullable: false),
                    PointsChangePct = table.Column<double>(type: "REAL", nullable: false),
                    Volume = table.Column<double>(type: "REAL", nullable: false),
                    TurnOverinCr = table.Column<double>(type: "REAL", nullable: false),
                    PE = table.Column<double>(type: "REAL", nullable: false),
                    PB = table.Column<double>(type: "REAL", nullable: false),
                    DivYield = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexDailyDataTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndexInformation",
                columns: table => new
                {
                    IndexId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IndexName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexInformation", x => x.IndexId);
                });

            migrationBuilder.CreateTable(
                name: "PledgedSummary",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompanyName = table.Column<string>(type: "TEXT", nullable: false),
                    TotalNumberOfShares = table.Column<long>(type: "INTEGER", nullable: false),
                    PromotersSharesPledged = table.Column<long>(type: "INTEGER", nullable: false),
                    PromotersShares = table.Column<long>(type: "INTEGER", nullable: false),
                    DisclosureDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    totalPledgedShares = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PledgedSummary", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyInformation_ISINNumber",
                table: "CompanyInformation",
                column: "ISINNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyInformation_Symbol_Series",
                table: "CompanyInformation",
                columns: new[] { "Symbol", "Series" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquityDailyTable_CompanyId",
                table: "EquityDailyTable",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_EquityDailyTable_CompanyId_day",
                table: "EquityDailyTable",
                columns: new[] { "CompanyId", "day" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquityDailyTable_day",
                table: "EquityDailyTable",
                column: "day");

            migrationBuilder.CreateIndex(
                name: "IX_IndexDailyDataTable_day",
                table: "IndexDailyDataTable",
                column: "day");

            migrationBuilder.CreateIndex(
                name: "IX_IndexDailyDataTable_IndexId",
                table: "IndexDailyDataTable",
                column: "IndexId");

            migrationBuilder.CreateIndex(
                name: "IX_IndexDailyDataTable_IndexId_day",
                table: "IndexDailyDataTable",
                columns: new[] { "IndexId", "day" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndexInformation_IndexName",
                table: "IndexInformation",
                column: "IndexName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PledgedSummary_CompanyName",
                table: "PledgedSummary",
                column: "CompanyName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyInformation");

            migrationBuilder.DropTable(
                name: "EquityDailyTable");

            migrationBuilder.DropTable(
                name: "IndexDailyDataTable");

            migrationBuilder.DropTable(
                name: "IndexInformation");

            migrationBuilder.DropTable(
                name: "PledgedSummary");
        }
    }
}
