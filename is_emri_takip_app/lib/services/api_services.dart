import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';

class ApiService {
  
  final String baseUrl = "http://localhost:5266/api"; 

  // 1. GİRİŞ YAP (Login)
  Future<bool> login(String email, String password) async {
    final url = Uri.parse('$baseUrl/auth/login');
    
    try {
      final response = await http.post(
        url,
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'email': email,
          'sifre': password,
        }),
      );

      if (response.statusCode == 200) {
        // Giriş Başarılı! Token'ı telefona kaydet.
        final data = jsonDecode(response.body);
        String token = data['token'];
        
        final prefs = await SharedPreferences.getInstance();
        await prefs.setString('jwt_token', token);
        await prefs.setString('user_role', data['rol']); // Rolü de kaydet (lazım olabilir)
        
        return true;
      } else {
        // Hatalı şifre vs.
        return false;
      }
    } catch (e) {
      print("Hata oluştu: $e");
      return false;
    }
  }

  // 2. GÖREVLERİMİ GETİR (Get Tasks)
  Future<List<dynamic>> getMyTasks() async {
    final url = Uri.parse('$baseUrl/gorevlerim');
    final token = await _getToken();

    if (token == null) return [];

    try {
      final response = await http.get(
        url,
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token', // Kartımızı gösteriyoruz
        },
      );

      if (response.statusCode == 200) {
        return jsonDecode(response.body);
      } else {
        return [];
      }
    } catch (e) {
      print("Görevler çekilemedi: $e");
      return [];
    }
  }

  // 3. GÖREV DURUMU GÜNCELLE (Update Task)
  Future<bool> updateTaskStatus(int taskId, int newStatusId) async {
    final url = Uri.parse('$baseUrl/gorevlerim/$taskId/durum');
    final token = await _getToken();

    if (token == null) return false;

    try {
      final response = await http.put(
        url,
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
        body: jsonEncode({
          'yeniDurumID': newStatusId,
        }),
      );

      return response.statusCode == 200;
    } catch (e) {
      return false;
    }
  }

  // Yardımcı: Kayıtlı Token'ı getir
  Future<String?> _getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('jwt_token');
  }
}