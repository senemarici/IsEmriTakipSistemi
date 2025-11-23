import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:is_emri_takip_app/screens/work_order_list_screen.dart';
import 'package:is_emri_takip_app/services/api_services.dart';


class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  // --- DEĞİŞKENLERİMİZ ---
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final ApiService _apiService = ApiService();
  bool _isLoading = false; // Buton dönsün mü dönmesin mi?

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF5F5F5),
      body: Center(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(24.0),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              // 1. LOGO ALANI
              Container(
                height: 120,
                width: 120,
                decoration: BoxDecoration(
                  color: Colors.white,
                  shape: BoxShape.circle,
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withOpacity(0.1),
                      blurRadius: 10,
                      offset: const Offset(0, 5),
                    ),
                  ],
                ),
                padding: const EdgeInsets.all(20),
                
                child: Image.asset('assets/logo.png'), 
              ),
              const SizedBox(height: 30),
              
              // BAŞLIK
              Text(
                'Hoşgeldiniz',
                style: GoogleFonts.inter(
                  fontSize: 28,
                  fontWeight: FontWeight.bold,
                  color: const Color(0xFF0F172A),
                ),
              ),
              const SizedBox(height: 40),

              // 2. E-POSTA KUTUSU
              Align(
                alignment: Alignment.centerLeft,
                child: Text('E-posta Adresi', style: GoogleFonts.inter(fontWeight: FontWeight.w600)),
              ),
              const SizedBox(height: 8),
              TextField(
                controller: _emailController, // Değişkeni bağladık
                decoration: InputDecoration(
                  hintText: 'ornek@sirket.com',
                  filled: true,
                  fillColor: Colors.white,
                  suffixIcon: const Icon(Icons.mail_outline),
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: BorderSide.none,
                  ),
                ),
              ),
              const SizedBox(height: 20),

              // 3. ŞİFRE KUTUSU
              Align(
                alignment: Alignment.centerLeft,
                child: Text('Şifre', style: GoogleFonts.inter(fontWeight: FontWeight.w600)),
              ),
              const SizedBox(height: 8),
              TextField(
                controller: _passwordController, // Değişkeni bağladık
                obscureText: true, // Şifreyi gizle
                decoration: InputDecoration(
                  hintText: '••••••••',
                  filled: true,
                  fillColor: Colors.white,
                  suffixIcon: const Icon(Icons.lock_outline),
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: BorderSide.none,
                  ),
                ),
              ),
              const SizedBox(height: 30),

              // 4. GİRİŞ BUTONU (MANTIK BURADA)
              SizedBox(
                width: double.infinity,
                height: 56,
                child: ElevatedButton(
                  onPressed: _isLoading ? null : () async {
                    // Butona basılınca çalışacak kod:
                    
                    // a) Yükleniyor başlat
                    setState(() {
                      _isLoading = true;
                    });

                    // b) API'ye git sor
                    bool success = await _apiService.login(
                      _emailController.text.trim(),
                      _passwordController.text.trim(),
                    );

                    // c) Yükleniyor durdur
                    setState(() {
                      _isLoading = false;
                    });

                    // d) Sonuca bak
                    if (success) {
                      // Başarılı -> Listeye git
                      if (mounted) {
                        Navigator.pushReplacement(
                          context,
                          MaterialPageRoute(builder: (context) => const WorkOrderListScreen()),
                        );
                      }
                    } else {
                      // Hatalı -> Uyarı ver
                      if (mounted) {
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(
                            content: Text('Giriş başarısız. E-posta veya şifre hatalı.'),
                            backgroundColor: Colors.red,
                          ),
                        );
                      }
                    }
                  },
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color(0xFF0F172A),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                  // Yükleniyorsa dönen halka, değilse "Giriş Yap" yazısı göster
                  child: _isLoading
                      ? const CircularProgressIndicator(color: Colors.white)
                      : Text(
                          'Giriş Yap',
                          style: GoogleFonts.inter(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                            color: Colors.white,
                          ),
                        ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}