import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:is_emri_takip_app/screens/work_order_detail_screen.dart';
import 'package:is_emri_takip_app/services/api_services.dart';

class WorkOrderListScreen extends StatefulWidget {
  const WorkOrderListScreen({super.key});

  @override
  State<WorkOrderListScreen> createState() => _WorkOrderListScreenState();
}

class _WorkOrderListScreenState extends State<WorkOrderListScreen> {
  final ApiService _apiService = ApiService();

  // Sayfayı aşağı çekince yenilemek için
  Future<void> _refreshList() async {
    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF5F5F5),
      appBar: AppBar(
        backgroundColor: const Color(0xFF0F172A),
        title: Text('İş Emirleri', style: GoogleFonts.inter(fontWeight: FontWeight.bold, color: Colors.white)),
        centerTitle: true,
        automaticallyImplyLeading: false,
        actions: [
          // Çıkış Butonu
          IconButton(
            icon: const Icon(Icons.logout, color: Colors.white),
            onPressed: () => Navigator.pop(context), // Girişe atar
          ),
        ],
      ),
      // FUTURE BUILDER: API'den veriyi bekleyen yapı
      body: FutureBuilder<List<dynamic>>(
        future: _apiService.getMyTasks(), // API'ye git sor
        builder: (context, snapshot) {
          // 1. DURUM: Veri Bekleniyor (Yükleniyor)
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }

          // 2. DURUM: Hata Çıktı
          if (snapshot.hasError) {
            return Center(child: Text('Bir hata oluştu: ${snapshot.error}'));
          }

          // 3. DURUM: Veri Geldi ama Liste Boş
          final tasks = snapshot.data ?? [];
          if (tasks.isEmpty) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Icon(Icons.task_alt, size: 64, color: Colors.grey),
                  const SizedBox(height: 16),
                  Text("Size atanmış aktif görev yok.", style: GoogleFonts.inter(color: Colors.grey)),
                ],
              ),
            );
          }

          // 4. DURUM: Veri Geldi ve Dolu -> Listeyi Çiz
          return RefreshIndicator(
            onRefresh: _refreshList,
            child: ListView.separated(
              padding: const EdgeInsets.all(16),
              itemCount: tasks.length,
              separatorBuilder: (ctx, index) => const SizedBox(height: 12),
              itemBuilder: (context, index) {
                final task = tasks[index]; // Sıradaki görev verisi (JSON)
                
                return _buildTaskCard(
                  context,
                  title: task['baslik'] ?? 'Başlıksız',
                  description: task['aciklama'] ?? '',
                  date: _formatDate(task['olusturmaTarihi']),
                  status: task['durumAdi'] ?? 'Bilinmiyor',
                  categoryName: task['kategoriAdi'] ?? 'Genel',
                  taskId: task['isEmriID'],
                );
              },
            ),
          );
        },
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: _refreshList, // Basınca listeyi yenile
        backgroundColor: const Color(0xFF0F172A),
        child: const Icon(Icons.refresh, color: Colors.white),
      ),
    );
  }

  // Tarihi güzelleştiren basit fonksiyon (2025-10-30 -> 30.10.2025 gibi)
  String _formatDate(String? dateStr) {
    if (dateStr == null) return "";
    try {
      DateTime dt = DateTime.parse(dateStr);
      return "${dt.day}.${dt.month}.${dt.year}";
    } catch (e) {
      return dateStr;
    }
  }

  Widget _buildTaskCard(BuildContext context, {
    required String title,
    required String description,
    required String date,
    required String status,
    required String categoryName,
    required int taskId,
  }) {
    
    // İkon Seçici Mantığı (İsme göre)
    IconData getIcon() {
      // Backend'den gelen Kategori Adına göre ikon seçiyoruz
      if (categoryName.contains("Bakım")) return Icons.build_circle_outlined;
      if (categoryName.contains("Arıza")) return Icons.error_outline;
      if (categoryName.contains("Montaj")) return Icons.cable;
      return Icons.work_outline;
    }

    // Duruma göre renk seçici
    Color getStatusColor() {
      if (status == "Tamamlandı") return Colors.green.shade100;
      if (status == "İptal Edildi") return Colors.red.shade100;
      return Colors.orange.shade100; // Atandı, Devam Ediyor
    }
    
    Color getStatusTextColor() {
      if (status == "Tamamlandı") return Colors.green.shade800;
      if (status == "İptal Edildi") return Colors.red.shade800;
      return Colors.orange.shade800;
    }

    return GestureDetector(
      onTap: () {
        // Detay sayfasına git (İleride buraya ID göndereceğiz)
        Navigator.push(context, MaterialPageRoute(builder: (context) => const WorkOrderDetailScreen()));
      },
      child: Container(
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          boxShadow: [
            BoxShadow(color: Colors.black.withOpacity(0.05), blurRadius: 10, offset: const Offset(0, 4)),
          ],
        ),
        padding: const EdgeInsets.all(16),
        child: Row(
          children: [
            // SOL İKON
            Container(
              height: 50,
              width: 50,
              decoration: BoxDecoration(
                color: const Color.fromARGB(255, 255, 5, 5),
                shape: BoxShape.circle,
              ),
              child: Icon(getIcon(), color: const Color.fromARGB(255, 255, 255, 255), size: 28),
            ),
            const SizedBox(width: 16),
            
            // ORTA BİLGİ
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(title, style: GoogleFonts.inter(fontWeight: FontWeight.bold, fontSize: 16)),
                  const SizedBox(height: 4),
                  Text(
                    description, 
                    style: GoogleFonts.inter(color: Colors.grey, fontSize: 12),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis, // Uzun yazı taşmasın
                  ),
                  const SizedBox(height: 4),
                  Row(
                    children: [
                      const Icon(Icons.calendar_today, size: 12, color: Colors.grey),
                      const SizedBox(width: 4),
                      Text(date, style: GoogleFonts.inter(color: Colors.grey, fontSize: 12)),
                    ],
                  )
                ],
              ),
            ),

            // SAĞ DURUM
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
              decoration: BoxDecoration(
                color: getStatusColor(),
                borderRadius: BorderRadius.circular(20),
              ),
              child: Text(
                status,
                style: GoogleFonts.inter(color: getStatusTextColor(), fontSize: 12, fontWeight: FontWeight.bold),
              ),
            ),
          ],
        ),
      ),
    );
  }
}