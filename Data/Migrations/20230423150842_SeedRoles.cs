using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace UserManagementWithIdentity.Data.Migrations
{
    public partial class SeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //adding custom first Role (صلاحية )
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] {"Id","Name","NormalizedName","ConcurrencyStamp" } ,
                values:  new object[] { Guid.NewGuid().ToString(),"Admin","Admin".ToUpper(), Guid.NewGuid().ToString() } , 
                schema: "security" 
                ) ;
            //adding second Role
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { Guid.NewGuid().ToString(), "User", "User".ToUpper(), Guid.NewGuid().ToString() },
                schema: "security"
                );
            //adding third Role
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { Guid.NewGuid().ToString(), "HR", "HR".ToUpper(), Guid.NewGuid().ToString() },
                schema: "security"
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [security].[Roles]");
        }
    }
}
