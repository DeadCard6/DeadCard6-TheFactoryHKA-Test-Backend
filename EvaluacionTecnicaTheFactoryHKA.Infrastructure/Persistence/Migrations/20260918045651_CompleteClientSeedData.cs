using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvaluacionTecnicaTheFactoryHKA.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompleteClientSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "Phone", "RegistrationDate" },
                values: new object[] { "Calle 50 #25-30, Bogotá", "+57 300 123 4567", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "Phone", "RegistrationDate" },
                values: new object[] { null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }
    }
}
