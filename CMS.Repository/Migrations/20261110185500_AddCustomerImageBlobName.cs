using CMS.Repository.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS.Repository.Migrations;

[DbContext(typeof(CMSDBContext))]
[Migration("20261110185500_AddCustomerImageBlobName")]
public partial class AddCustomerImageBlobName : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ImageBlobName",
            table: "CRM_Customers",
            type: "nvarchar(512)",
            maxLength: 512,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ImageBlobName",
            table: "CRM_Customers");
    }
}
