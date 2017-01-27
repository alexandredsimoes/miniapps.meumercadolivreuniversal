using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MyML.UWP.Models;

namespace MyML.UWP.Migrations
{
    [DbContext(typeof(MercadoLivreContext))]
    partial class MercadoLivreContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("MyML.UWP.Models.Mercadolivre.Answer", b =>
                {
                    b.Property<int>("AnswerId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("date_created");

                    b.Property<string>("status");

                    b.Property<string>("text");

                    b.HasKey("AnswerId");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("MyML.UWP.Models.Mercadolivre.Item", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsFavorite");

                    b.Property<bool?>("accepts_mercadopago");

                    b.Property<int?>("available_quantity");

                    b.Property<string>("buying_mode");

                    b.Property<string>("category_id");

                    b.Property<string>("condition");

                    b.Property<string>("currency_id");

                    b.Property<string>("id");

                    b.Property<string>("listing_type_id");

                    b.Property<double?>("original_price");

                    b.Property<string>("permalink");

                    b.Property<double?>("price");

                    b.Property<double?>("seller_id");

                    b.Property<string>("site_id");

                    b.Property<int?>("sold_quantity");

                    b.Property<string>("status");

                    b.Property<string>("stop_time");

                    b.Property<string>("subtitle");

                    b.Property<string>("thumbnail");

                    b.Property<string>("title");

                    b.Property<string>("variation_id");

                    b.HasKey("ItemId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("MyML.UWP.Models.Mercadolivre.ProductQuestionContent", b =>
                {
                    b.Property<int>("ProductQuestionContentId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AnswerId");

                    b.Property<int>("ItemId");

                    b.Property<string>("buyer_experience");

                    b.Property<long?>("buyer_id");

                    b.Property<DateTime?>("date_created");

                    b.Property<long?>("id");

                    b.Property<string>("item_id");

                    b.Property<string>("nickname");

                    b.Property<DateTimeOffset?>("registration_date");

                    b.Property<long?>("seller_id");

                    b.Property<string>("status");

                    b.Property<string>("text");

                    b.HasKey("ProductQuestionContentId");

                    b.HasIndex("AnswerId");

                    b.HasIndex("ItemId");

                    b.ToTable("ProductQuestions");
                });

            modelBuilder.Entity("MyML.UWP.Models.Mercadolivre.ProductQuestionContent", b =>
                {
                    b.HasOne("MyML.UWP.Models.Mercadolivre.Answer", "answer")
                        .WithMany()
                        .HasForeignKey("AnswerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyML.UWP.Models.Mercadolivre.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
