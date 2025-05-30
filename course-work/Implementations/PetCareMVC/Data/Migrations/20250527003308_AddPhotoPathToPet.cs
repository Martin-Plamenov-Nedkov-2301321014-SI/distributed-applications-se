﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCareMVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoPathToPet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Pets",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Pets");
        }
    }
}
