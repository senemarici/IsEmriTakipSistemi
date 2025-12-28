import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';

class ApiService {
  // WEB (CHROME) İÇİN AYAR:
  // Tarayıcıda çalıştığın için doğrudan localhost kullanabilirsin.
  // DİKKAT: Backend portunun (burada 5266) doğru olduğundan emin ol. 
  // Backend çalışırken tarayıcıda açılan adres neyse (örn: https://localhost:7079) onu yaz.
  final String baseUrl = "http://localhost:5266/api"; 

  // --- 1. GİRİŞ YAP (LOGIN) ---
  Future<bool> login(String email, String password) async {
    final url = Uri.parse('$baseUrl/auth/login');
    
    try {
      final response = await http.post(
        url,
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json', // Web için eklemek iyidir
        },
        body: jsonEncode({
          'email': email,
          'sifre': password,
        }),
      );

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        String token = data['token'];
        String? rol = data['rol']; // Backend'den rol dönüyorsa al
        
        final prefs = await SharedPreferences.getInstance();
        await prefs.setString('jwt_token', token);
        if (rol != null) await prefs.setString('user_role', rol);
        
        return true;
      } else {
        print("Giriş Başarısız: ${response.body}");
        return false;
      }
    } catch (e) {
      print("Bağlantı Hatası (Login): $e");
      return false;
    }
  }

  // --- 2. GÖREVLERİMİ GETİR (GET) ---
  Future<List<dynamic>> getMyTasks() async {
    final url = Uri.parse('$baseUrl/gorevlerim'); // Endpoint ismini kontrol et
    final token = await _getToken();

    if (token == null) return [];

    try {
      final response = await http.get(
        url,
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        return jsonDecode(response.body);
      } else {
        print("Veri Hatası: ${response.statusCode}");
        return [];
      }
    } catch (e) {
      print("Bağlantı Hatası (GetTasks): $e");
      return [];
    }
  }

  // Görev Durumunu Güncelle
  Future<bool> updateTaskStatus(int isEmriID, int yeniDurumId) async {
    try {
      // DİKKAT: Backend'deki [FromQuery] yapısına uygun URL:
      // Örn: .../api/IsEmirleri/7/durum?yeniDurumId=3
      final response = await http.put(
        Uri.parse('$baseUrl/IsEmirleri/$isEmriID/durum?yeniDurumId=$yeniDurumId'),
        headers: await _getHeaders(),
      );

      if (response.statusCode == 200) {
        return true;
      } else {
        print("Güncelleme Hatası: ${response.body}");
        return false;
      }
    } catch (e) {
      print("Bağlantı Hatası (Update): $e");
      return false;
    }
  }

  // --- 4. ÇIKIŞ YAP ---
  Future<void> logout() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.clear();
  }

  // --- YARDIMCI ---
  Future<String?> _getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('jwt_token');
  }
  
  // --- YARDIMCI: Header Oluşturucu (Token'ı otomatik ekler) ---
  Future<Map<String, String>> _getHeaders() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('token') ?? ""; // Token yoksa boş string al
    
    return {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer $token', // Backend'deki [Authorize] kilidini açan anahtar
    };
  }
}