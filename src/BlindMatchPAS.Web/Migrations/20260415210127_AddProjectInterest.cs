using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlindMatchPAS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectInterest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectProposals_AspNetUsers_StudentUserId",
                table: "ProjectProposals");

            migrationBuilder.AddColumn<DateTime>(
                name: "IdentityRevealedAtUtc",
                table: "ProjectProposals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MatchedAtUtc",
                table: "ProjectProposals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatchedSupervisorId",
                table: "ProjectProposals",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectInterests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectProposalId = table.Column<int>(type: "int", nullable: false),
                    SupervisorUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectInterests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectInterests_AspNetUsers_SupervisorUserId",
                        column: x => x.SupervisorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectInterests_ProjectProposals_ProjectProposalId",
                        column: x => x.ProjectProposalId,
                        principalTable: "ProjectProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProposals_MatchedSupervisorId",
                table: "ProjectProposals",
                column: "MatchedSupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInterests_ProjectProposalId",
                table: "ProjectInterests",
                column: "ProjectProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInterests_SupervisorUserId",
                table: "ProjectInterests",
                column: "SupervisorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectProposals_AspNetUsers_MatchedSupervisorId",
                table: "ProjectProposals",
                column: "MatchedSupervisorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectProposals_AspNetUsers_StudentUserId",
                table: "ProjectProposals",
                column: "StudentUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectProposals_AspNetUsers_MatchedSupervisorId",
                table: "ProjectProposals");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectProposals_AspNetUsers_StudentUserId",
                table: "ProjectProposals");

            migrationBuilder.DropTable(
                name: "ProjectInterests");

            migrationBuilder.DropIndex(
                name: "IX_ProjectProposals_MatchedSupervisorId",
                table: "ProjectProposals");

            migrationBuilder.DropColumn(
                name: "IdentityRevealedAtUtc",
                table: "ProjectProposals");

            migrationBuilder.DropColumn(
                name: "MatchedAtUtc",
                table: "ProjectProposals");

            migrationBuilder.DropColumn(
                name: "MatchedSupervisorId",
                table: "ProjectProposals");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectProposals_AspNetUsers_StudentUserId",
                table: "ProjectProposals",
                column: "StudentUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
