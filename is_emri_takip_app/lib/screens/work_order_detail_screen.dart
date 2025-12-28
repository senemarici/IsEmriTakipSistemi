import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:is_emri_takip_app/services/api_services.dart'; 
class WorkOrderDetailScreen extends StatefulWidget {
  // Dışarıdan veri gelmesi için bu değişkeni ekledik
  final Map<String, dynamic> task;

  const WorkOrderDetailScreen({super.key, required this.task});

  @override
  State<WorkOrderDetailScreen> createState() => _WorkOrderDetailScreenState();
}

class _WorkOrderDetailScreenState extends State<WorkOrderDetailScreen> {
  final ApiService _apiService = ApiService();
  bool _isLoading = false; // Yükleniyor animasyonu için

  // --- DURUM GÜNCELLEME FONKSİYONU ---
  Future<void> _durumGuncelle(int yeniDurumId, String yeniDurumAdi) async {
    setState(() {
      _isLoading = true;
    });

    // API'ye güncelleme isteği atıyoruz
    // NOT: Backend'deki ID ismi 'isEmriID' ise burayı değiştirme. 'id' ise düzelt.
    bool basarili = await _apiService.updateTaskStatus(widget.task['isEmriID'], yeniDurumId);

    setState(() {
      _isLoading = false;
    });

    if (basarili) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Durum '$yeniDurumAdi' yapıldı!"), backgroundColor: Colors.green),
      );
      // Geri dönerken 'true' gönderiyoruz ki Ana Sayfa listeyi yenilesin
      Navigator.pop(context, true); 
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Hata oluştu!"), backgroundColor: Colors.red),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    // Verileri değişkenlere alalım (Boş gelirse varsayılan metin yazsın)
    final baslik = widget.task['baslik'] ?? "Başlık Yok";
    final aciklama = widget.task['aciklama'] ?? "Açıklama girilmemiş.";
    final oncelik = widget.task['oncelikAdi'] ?? "-";
    final kategori = widget.task['kategoriAdi'] ?? "-";

    return Scaffold(
      backgroundColor: const Color(0xffF4F5F7),
      appBar: AppBar(
        backgroundColor: Colors.white,
        elevation: 0.5,
        centerTitle: true,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back, color: Colors.black),
          // 1. DÜZELTME: GERİ BUTONUNU AKTİF ETTİK
          onPressed: () {
             Navigator.pop(context); 
          },
        ),
        title: Text(
          'Görev Detayı',
          style: GoogleFonts.inter(
            fontWeight: FontWeight.bold,
            color: Colors.black,
          ),
        ),
      ),

      // Yükleniyorsa dönen daire, değilse sayfa içeriği
      body: _isLoading 
        ? const Center(child: CircularProgressIndicator()) 
        : Padding(
        padding: const EdgeInsets.all(20.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            /// Üst Bilgi Kartı
            Container(
              padding: const EdgeInsets.all(20),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(14),
                boxShadow: const [
                  BoxShadow(
                    color: Colors.black12,
                    blurRadius: 6,
                    offset: Offset(0, 3),
                  )
                ],
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // 2. DÜZELTME: BAŞLIK VERİTABANINDAN GELİYOR
                  Text(
                    baslik,
                    style: GoogleFonts.inter(
                      fontSize: 18,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                  const SizedBox(height: 16),

                  Row(
                    children: [
                      const Icon(Icons.flag, size: 20, color: Colors.redAccent),
                      const SizedBox(width: 8),
                      // DÜZELTME: Flexible ekledik, yazı uzunsa alt satıra geçmez, sığışır.
                      Flexible(
                        child: Text(
                          'Öncelik: $oncelik', 
                          style: GoogleFonts.inter(),
                          overflow: TextOverflow.ellipsis, // Sığmazsa ... koyar
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 8),

                  Row(
                    children: [
                      const Icon(Icons.category, size: 20, color: Colors.blueGrey),
                      const SizedBox(width: 8),
                      // DÜZELTME: Kategori isminde de aynı koruma
                      Flexible(
                        child: Text(
                          'Kategori: $kategori', 
                          style: GoogleFonts.inter(),
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),

            const SizedBox(height: 24),

            Text('Açıklama',
                style: GoogleFonts.inter(
                    fontSize: 15, fontWeight: FontWeight.w600)),
            const SizedBox(height: 8),

            // 5. DÜZELTME: AÇIKLAMA VERİSİ (Kaydırılabilir yaptık)
            Expanded(
              child: SingleChildScrollView(
                child: Text(
                  aciklama,
                  style: GoogleFonts.inter(fontSize: 14, color: Colors.grey[700], height: 1.5),
                ),
              ),
            ),

            const SizedBox(height: 20),

            /// BUTONLAR
            Row(
              children: [
                Expanded(
                  child: InkWell( 
                    // DÜZELTME: ID'yi 2 yaptık (Atandı/Devam Ediyor Modu)
                    onTap: () => _durumGuncelle(2, "Devam Ediyor"), 
                    child: Container(
                      height: 48,
                      decoration: BoxDecoration(
                        color: Colors.grey.shade300,
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: Center(
                        child: Text(
                          'Devam Ediyor',
                          style: GoogleFonts.inter(
                              fontWeight: FontWeight.w600, fontSize: 15),
                        ),
                      ),
                    ),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: InkWell( 
                    // DÜZELTME: ID'yi 3 yaptık (Çünkü sende 3 Tamamlandı demekmiş)
                    onTap: () => _durumGuncelle(3, "Tamamlandı"), 
                    child: Container(
                      height: 48,
                      decoration: BoxDecoration(
                        color: const Color(0xFF4CAF50), 
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: Center(
                        child: Text(
                          'Tamamla',
                          style: GoogleFonts.inter(
                              color: Colors.white,
                              fontWeight: FontWeight.w600,
                              fontSize: 15),
                        ),
                      ),
                    ),
                  ),
                ),
              ],
            )
          ],
        ),
      ),
    );
  }
}