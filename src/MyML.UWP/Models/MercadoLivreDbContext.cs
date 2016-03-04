using Microsoft.Data.Entity;
using MyML.UWP.Models.Mercadolivre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.Models
{
    public class MercadoLivreDbContext : DbContext
    {
        public DbSet<MLAutorizationInfo> Autorizations { get; set; }
        public DbSet<ProductQuestionContent> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {           
            optionsBuilder.UseSqlite($"Filename=MeuMercadoLivreUniversal.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore(typeof(Item));

            /*
            modelBuilder.Entity<Item>().Ignore(c => c.seller);
            modelBuilder.Entity<Item>().Ignore(c => c.seller_address);
            modelBuilder.Entity<Item>().Ignore(c => c.shipping);
            modelBuilder.Entity<Item>().Ignore(c => c.installments);
            modelBuilder.Entity<Item>().Ignore(c => c.pictures);
            modelBuilder.Entity<Item>().Ignore(c => c.attributes);
            modelBuilder.Entity<Item>().Ignore(c => c.variation_attributes);
            modelBuilder.Entity<Item>().Ignore(c => c.differential_pricing);
            modelBuilder.Entity<Item>().Ignore(c => c.ListType);
            */
        }
    }
}
