using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IsEmriTakip.API.Migrations
{
    /// <inheritdoc />
    public partial class AnaTablolarVeIliskilerEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    KullaniciID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SifreHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RolID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.KullaniciID);
                    table.ForeignKey(
                        name: "FK_Kullanicilar_Roller_RolID",
                        column: x => x.RolID,
                        principalTable: "Roller",
                        principalColumn: "RolID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IsEmirleri",
                columns: table => new
                {
                    IsEmriID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KategoriID = table.Column<int>(type: "int", nullable: false),
                    OncelikID = table.Column<int>(type: "int", nullable: false),
                    DurumID = table.Column<int>(type: "int", nullable: false),
                    OlusturanYoneticiID = table.Column<int>(type: "int", nullable: false),
                    AtananTeknisyenID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IsEmirleri", x => x.IsEmriID);
                    table.ForeignKey(
                        name: "FK_IsEmirleri_Durumlar_DurumID",
                        column: x => x.DurumID,
                        principalTable: "Durumlar",
                        principalColumn: "DurumID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IsEmirleri_Kategoriler_KategoriID",
                        column: x => x.KategoriID,
                        principalTable: "Kategoriler",
                        principalColumn: "KategoriID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IsEmirleri_Kullanicilar_AtananTeknisyenID",
                        column: x => x.AtananTeknisyenID,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_IsEmirleri_Kullanicilar_OlusturanYoneticiID",
                        column: x => x.OlusturanYoneticiID,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IsEmirleri_Oncelikler_OncelikID",
                        column: x => x.OncelikID,
                        principalTable: "Oncelikler",
                        principalColumn: "OncelikID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Durumlar",
                columns: new[] { "DurumID", "DurumAdi" },
                values: new object[,]
                {
                    { 1, "Atandı" },
                    { 2, "Devam Ediyor" },
                    { 3, "Tamamlandı" },
                    { 4, "İptal Edildi" }
                });

            migrationBuilder.InsertData(
                table: "Kategoriler",
                columns: new[] { "KategoriID", "KategoriAdi" },
                values: new object[,]
                {
                    { 1, "Bakım" },
                    { 2, "Arıza" },
                    { 3, "Montaj" }
                });

            migrationBuilder.InsertData(
                table: "Oncelikler",
                columns: new[] { "OncelikID", "OncelikAdi" },
                values: new object[,]
                {
                    { 1, "Düşük" },
                    { 2, "Orta" },
                    { 3, "Yüksek" }
                });

            migrationBuilder.InsertData(
                table: "Roller",
                columns: new[] { "RolID", "RolAdi" },
                values: new object[,]
                {
                    { 1, "Yonetici" },
                    { 2, "Teknisyen" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_AtananTeknisyenID",
                table: "IsEmirleri",
                column: "AtananTeknisyenID");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_DurumID",
                table: "IsEmirleri",
                column: "DurumID");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_KategoriID",
                table: "IsEmirleri",
                column: "KategoriID");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_OlusturanYoneticiID",
                table: "IsEmirleri",
                column: "OlusturanYoneticiID");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_OncelikID",
                table: "IsEmirleri",
                column: "OncelikID");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_Email",
                table: "Kullanicilar",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_RolID",
                table: "Kullanicilar",
                column: "RolID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IsEmirleri");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DeleteData(
                table: "Durumlar",
                keyColumn: "DurumID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Durumlar",
                keyColumn: "DurumID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Durumlar",
                keyColumn: "DurumID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Durumlar",
                keyColumn: "DurumID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "KategoriID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "KategoriID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "KategoriID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Oncelikler",
                keyColumn: "OncelikID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Oncelikler",
                keyColumn: "OncelikID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Oncelikler",
                keyColumn: "OncelikID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roller",
                keyColumn: "RolID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roller",
                keyColumn: "RolID",
                keyValue: 2);
        }
    }
}
