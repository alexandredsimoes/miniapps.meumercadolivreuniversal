using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyML.UWP.Migrations
{
    public partial class MyFirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    AnswerId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    date_created = table.Column<string>(nullable: true),
                    status = table.Column<string>(nullable: true),
                    text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.AnswerId);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsFavorite = table.Column<bool>(nullable: false),
                    accepts_mercadopago = table.Column<bool>(nullable: true),
                    available_quantity = table.Column<int>(nullable: true),
                    buying_mode = table.Column<string>(nullable: true),
                    category_id = table.Column<string>(nullable: true),
                    condition = table.Column<string>(nullable: true),
                    currency_id = table.Column<string>(nullable: true),
                    id = table.Column<string>(nullable: true),
                    listing_type_id = table.Column<string>(nullable: true),
                    original_price = table.Column<double>(nullable: true),
                    permalink = table.Column<string>(nullable: true),
                    price = table.Column<double>(nullable: true),
                    seller_id = table.Column<double>(nullable: true),
                    site_id = table.Column<string>(nullable: true),
                    sold_quantity = table.Column<int>(nullable: true),
                    status = table.Column<string>(nullable: true),
                    stop_time = table.Column<string>(nullable: true),
                    subtitle = table.Column<string>(nullable: true),
                    thumbnail = table.Column<string>(nullable: true),
                    title = table.Column<string>(nullable: true),
                    variation_id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                });

            migrationBuilder.CreateTable(
                name: "ProductQuestions",
                columns: table => new
                {
                    ProductQuestionContentId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnswerId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    buyer_experience = table.Column<string>(nullable: true),
                    buyer_id = table.Column<long>(nullable: true),
                    date_created = table.Column<DateTime>(nullable: true),
                    id = table.Column<long>(nullable: true),
                    item_id = table.Column<string>(nullable: true),
                    nickname = table.Column<string>(nullable: true),
                    registration_date = table.Column<DateTimeOffset>(nullable: true),
                    seller_id = table.Column<long>(nullable: true),
                    status = table.Column<string>(nullable: true),
                    text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductQuestions", x => x.ProductQuestionContentId);
                    table.ForeignKey(
                        name: "FK_ProductQuestions_Answers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answers",
                        principalColumn: "AnswerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductQuestions_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductQuestions_AnswerId",
                table: "ProductQuestions",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductQuestions_ItemId",
                table: "ProductQuestions",
                column: "ItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductQuestions");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
