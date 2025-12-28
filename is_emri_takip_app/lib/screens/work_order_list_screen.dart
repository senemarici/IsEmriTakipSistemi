import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:is_emri_takip_app/screens/work_order_detail_screen.dart';
import 'package:is_emri_takip_app/services/api_services.dart';
import 'package:is_emri_takip_app/screens/login_screen.dart'; // <-- GİRİŞ EKRANI IMPORTU (Dosya adın farklıysa düzelt)

class WorkOrderListScreen extends StatefulWidget {
  const WorkOrderListScreen({super.key});

  @override
  State<WorkOrderListScreen> createState() => _WorkOrderListScreenState();
}

class _WorkOrderListScreenState extends State<WorkOrderListScreen> {
  final ApiService _apiService = ApiService();

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
          // --- DÜZELTİLEN ÇIKIŞ BUTONU ---
          IconButton(
            icon: const Icon(Icons.logout, color: Colors.white),
            onPressed: () async {
              // 1. Token'ı hafızadan sil
              final prefs = await SharedPreferences.getInstance();
              await prefs.remove('token');

              // 2. Güvenli yönlendirme
              if (context.mounted) {
                // Tüm geçmişi sil ve Login ekranını aç (Geri tuşuna basınca dönemezsin)
                Navigator.pushAndRemoveUntil(
                  context,
                  MaterialPageRoute(builder: (context) => const LoginScreen()), 
                  (route) => false, // Geçmişteki tüm sayfaları kapat
                );
              }
            },
          ),
        ],
      ),
      body: FutureBuilder<List<dynamic>>(
        future: _apiService.getMyTasks(),
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }

          if (snapshot.hasError) {
            return Center(child: Text('Bir hata oluştu: ${snapshot.error}'));
          }

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

          return RefreshIndicator(
            onRefresh: _refreshList,
            child: ListView.separated(
              padding: const EdgeInsets.all(16),
              itemCount: tasks.length,
              separatorBuilder: (ctx, index) => const SizedBox(height: 12),
              itemBuilder: (context, index) {
                final task = tasks[index];
                return _buildTaskCard(context, task);
              },
            ),
          );
        },
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: _refreshList,
        backgroundColor: const Color(0xFF0F172A),
        child: const Icon(Icons.refresh, color: Colors.white),
      ),
    );
  }

  String _formatDate(String? dateStr) {
    if (dateStr == null) return "";
    try {
      DateTime dt = DateTime.parse(dateStr);
      return "${dt.day}.${dt.month}.${dt.year}";
    } catch (e) {
      return dateStr;
    }
  }

  Widget _buildTaskCard(BuildContext context, Map<String, dynamic> task) {
    final title = task['baslik'] ?? 'Başlıksız';
    final description = task['aciklama'] ?? '';
    final date = _formatDate(task['olusturmaTarihi']);
    final status = task['durumAdi'] ?? 'Bilinmiyor';
    final categoryName = task['kategoriAdi'] ?? 'Genel';

    IconData getIcon() {
      if (categoryName.contains("Bakım")) return Icons.build_circle_outlined;
      if (categoryName.contains("Arıza")) return Icons.error_outline;
      if (categoryName.contains("Montaj")) return Icons.cable;
      return Icons.work_outline;
    }

    Color getStatusColor() {
      if (status == "Tamamlandı") return Colors.green.shade100;
      if (status == "İptal Edildi") return Colors.red.shade100;
      return Colors.orange.shade100;
    }

    Color getStatusTextColor() {
      if (status == "Tamamlandı") return Colors.green.shade800;
      if (status == "İptal Edildi") return Colors.red.shade800;
      return Colors.orange.shade800;
    }

    return GestureDetector(
      onTap: () async {
        final result = await Navigator.push(
          context,
          MaterialPageRoute(
            builder: (context) => WorkOrderDetailScreen(task: task),
          ),
        );

        if (result == true) {
          _refreshList();
        }
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
            Container(
              height: 50,
              width: 50,
              decoration: const BoxDecoration(
                color: Color.fromARGB(255, 255, 5, 5),
                shape: BoxShape.circle,
              ),
              child: Icon(getIcon(), color: Colors.white, size: 28),
            ),
            const SizedBox(width: 16),
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
                    overflow: TextOverflow.ellipsis,
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