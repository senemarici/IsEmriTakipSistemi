import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';

class ApiService {
  
  final String baseUrl = "http://localhost:5266/api"; 

  // --- 1. GİRİŞ YAP (LOGIN) ---
  Future<bool> login(String email, String password) async {
    final url = Uri.parse('$baseUrl/auth/login');
    
    try {
      final response = await http.post(
        url,
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json', 
        },
        body: jsonEncode({
          'email': email,
          'sifre': password,
        }),
      );

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        String token = data['token'];
        String? rol = data['rol']; 

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
    final url = Uri.parse('$baseUrl/gorevlerim'); 
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

  // --- 3. GÖREV DURUMUNU GÜNCELLE (UPDATE) ---
  Future<bool> updateTaskStatus(int isEmriID, int yeniDurumId) async {
    try {
      
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

  
  Future<String?> _getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('jwt_token');
  }
  
  
  Future<Map<String, String>> _getHeaders() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('token') ?? ""; 
    
    return {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer $token', 
    };
  }
}