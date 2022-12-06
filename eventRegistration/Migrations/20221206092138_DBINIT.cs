using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eventRegistration.Migrations
{
    /// <inheritdoc />
    public partial class DBINIT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "varchar(254)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
<<<<<<< HEAD:eventRegistration/Migrations/20221206092138_DBINIT.cs
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
=======
                    PhoneNumber = table.Column<int>(type: "varchar(50)", nullable: false),
>>>>>>> 1847d57b422d5fa48e84f80a4434368f14924562:eventRegistration/Migrations/20221205123538_DBINIT.cs
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAttended = table.Column<bool>(type: "bit", nullable: false),
                    Okay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Table = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guest", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guest_Email",
                table: "Guest",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guest");
        }
    }
}
