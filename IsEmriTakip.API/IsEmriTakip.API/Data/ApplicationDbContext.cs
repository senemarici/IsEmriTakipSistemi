using Microsoft.EntityFrameworkCore;
using IsEmriTakip.API.Models; // Models klasörümüzü ekliyoruz


namespace IsEmriTakip.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor (Yapıcı Metot)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Modellerimizi DbSet olarak ekliyoruz (adım 1'de oluşturulan her plan {rol,kullanıcı gibi}DbSet olarak bildirilerek gerçek tabloları inşa edilir.
        // Bu işlemler yapılmasaydı planlar tanımlanmış olsa bile EF Core görmezden gelir ve tablolar oluşturulmazdı.
        public DbSet<Rol> Roller { get; set; }
        public DbSet<Durum> Durumlar { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Oncelik> Oncelikler { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<IsEmri> IsEmirleri { get; set; }

        //Planlarda belirtilmeyen ekstra talimatlar
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Kullanici.Email alanını UNIQUE yap.
            modelBuilder.Entity<Kullanici>()
                .HasIndex(k => k.Email)
                .IsUnique();

            // IsEmri -> Kullanici (Yönetici) ilişkisi
            modelBuilder.Entity<IsEmri>()
                .HasOne(ie => ie.OlusturanYonetici)
                .WithMany() // Bir yönetici birden fazla iş emri oluşturabilir
                .HasForeignKey(ie => ie.OlusturanYoneticiID)
                .OnDelete(DeleteBehavior.Restrict); // Yönetici silinirse iş emirleri silinmesin

            // IsEmri -> Kullanici (Teknisyen) ilişkisi
            modelBuilder.Entity<IsEmri>()
                .HasOne(ie => ie.AtananTeknisyen)
                .WithMany() // Bir teknisyen birden fazla iş emrine atanabilir
                .HasForeignKey(ie => ie.AtananTeknisyenID)
                .OnDelete(DeleteBehavior.SetNull); // Teknisyen silinirse görev "null" a düşsün

            // --- BAŞLANGIÇ VERİSİ (SEED DATA) ---
            // Uygulama ilk kurulduğunda bu veriler tablolara eklenecek

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

