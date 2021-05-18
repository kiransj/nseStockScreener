using Microsoft.EntityFrameworkCore.Migrations;

namespace nseScreener.Migrations
{
    public partial class UpdateIndexInCompanyInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyInformation_Symbol_Series",
                table: "CompanyInformation");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyInformation_Industry",
                table: "CompanyInformation",
                column: "Industry");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyInformation_Symbol",
                table: "CompanyInformation",
                column: "Symbol");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyInformation_Industry",
                table: "CompanyInformation");

            migrationBuilder.DropIndex(
                name: "IX_CompanyInformation_Symbol",
                table: "CompanyInformation");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyInformation_Symbol_Series",
                table: "CompanyInformation",
                columns: new[] { "Symbol", "Series" },
                unique: true);
        }
    }
}
