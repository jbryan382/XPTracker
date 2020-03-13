using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace XPTracker.Migrations
{
    public partial class DropedAndRefreshedDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LevelTracker",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Level = table.Column<int>(nullable: false),
                    MinXP = table.Column<int>(nullable: false),
                    MaxXP = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelTracker", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "XPTracker",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TotalXP = table.Column<int>(nullable: false),
                    TotalLevel = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XPTracker", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionTracker",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SessionNumber = table.Column<int>(nullable: false),
                    SessionDate = table.Column<DateTime>(nullable: false),
                    SessionDescription = table.Column<string>(nullable: true),
                    SocialInteractionXP = table.Column<int>(nullable: false),
                    ExplorationXP = table.Column<int>(nullable: false),
                    CombatXP = table.Column<int>(nullable: false),
                    XPId = table.Column<int>(nullable: false),
                    XPTrackerModelId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionTracker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionTracker_XPTracker_XPTrackerModelId",
                        column: x => x.XPTrackerModelId,
                        principalTable: "XPTracker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionTracker_XPTrackerModelId",
                table: "SessionTracker",
                column: "XPTrackerModelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LevelTracker");

            migrationBuilder.DropTable(
                name: "SessionTracker");

            migrationBuilder.DropTable(
                name: "XPTracker");
        }
    }
}
