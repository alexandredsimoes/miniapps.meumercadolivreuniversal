using Microsoft.EntityFrameworkCore;
using MyML.UWP.Models.Mercadolivre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.Models
{
    public class MercadoLivreContext: DbContext
    {
        public DbSet<ProductQuestionContent> ProductQuestions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=MiniApps.MyML.UWP.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .Ignore(c => c.differential_pricing)
                .Ignore(c => c.seller)
                .Ignore(c => c.installments)
                .Ignore(c => c.address)
                .Ignore(c => c.seller_address)
                .Ignore(c => c.attributes)
                .Ignore(c => c.pictures)
                .Ignore(c => c.variation_attributes)
                .Ignore(c => c.ListType)
                .Ignore(c => c.shipping);
        }
    }
}
