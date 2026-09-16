using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class blogtableupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDate",
                table: "Blogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReadingTime",
                table: "Blogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Blogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "PublishDate",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "ReadingTime",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Blogs");
        }
    }
}
