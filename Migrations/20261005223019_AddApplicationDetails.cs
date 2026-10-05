using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "FollowUpDate",
                table: "JobApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "JobApplications",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowUpDate",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "JobApplications");
        }
    }
}
