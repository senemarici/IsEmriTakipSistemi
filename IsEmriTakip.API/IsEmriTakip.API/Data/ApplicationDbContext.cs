using Microsoft.EntityFrameworkCore;
using IsEmriTakip.API.Models; 

namespace IsEmriTakip.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Rol> Roller { get; set; }
        public DbSet<Durum> Durumlar { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Oncelik> Oncelikler { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<IsEmri> IsEmirleri { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Kullanici>()
                .HasIndex(k => k.Email)
                .IsUnique();

            modelBuilder.Entity<IsEmri>()
                .HasOne(ie => ie.OlusturanYonetici)
                .WithMany() 
                .HasForeignKey(ie => ie.OlusturanYoneticiID)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<IsEmri>()
                .HasOne(ie => ie.AtananTeknisyen)
                .WithMany() 
                .HasForeignKey(ie => ie.AtananTeknisyenID)
                .OnDelete(DeleteBehavior.SetNull); 


            modelBuilder.Entity<Rol>().HasData(
                new Rol { RolID = 1, RolAdi = "Yonetici" },
                new Rol { RolID = 2, RolAdi = "Teknisyen" }
            );

            modelBuilder.Entity<Durum>().HasData(
                new Durum { DurumID = 1, DurumAdi = "Atandı" },
                new Durum { DurumID = 2, DurumAdi = "Devam Ediyor" },
                new Durum { DurumID = 3, DurumAdi = "Tamamlandı" },
                new Durum { DurumID = 4, DurumAdi = "İptal Edildi" }
            );

            modelBuilder.Entity<Oncelik>().HasData(
                new Oncelik { OncelikID = 1, OncelikAdi = "Düşük" },
                new Oncelik { OncelikID = 2, OncelikAdi = "Orta" },
                new Oncelik { OncelikID = 3, OncelikAdi = "Yüksek" }
            );

            modelBuilder.Entity<Kategori>().HasData(
                new Kategori { KategoriID = 1, KategoriAdi = "Bakım" },
                new Kategori { KategoriID = 2, KategoriAdi = "Arıza" },
                new Kategori { KategoriID = 3, KategoriAdi = "Montaj" }
            );
        }
    }
}

