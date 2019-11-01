using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace ApexVoxApi.Infrastructure.Data.Migrations
{
    public partial class AddRLSRelatedProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var scriptsDirectory = "Infrastructure/Data/Scripts";
            var scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, scriptsDirectory, "RLS_CREATE_SecurityPolicy.sql");
            migrationBuilder.Sql(File.ReadAllText(scriptPath));

            //Add table to rls policy procedure
            var procedurePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, scriptsDirectory, "RLS_CREATE_SP_AddTableToPolicy.sql");
            migrationBuilder.Sql(File.ReadAllText(procedurePath));

            //Add trigger on create table for adding it to rls procedure
            var trigerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, scriptsDirectory, "RLS_CREATE_Trigger_ON_CreateTable_For_SecurityPolicy.sql");
            migrationBuilder.Sql(File.ReadAllText(trigerPath));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
