import React, { useState, useEffect } from "react";
import axios from "axios";
import {
  Shield,
  LayoutDashboard,
  ClipboardList,
  Users,
  Settings,
  LogOut,
  Plus,
  Search,
  Eye,
  EyeOff,
  X,
  Edit,
  Trash2,
  User
} from "lucide-react";

// --- AYARLAR ---
// Backend portunun 7079 olduğundan eminsen burayı değiştirme.
const API_BASE_URL = "http://localhost:5266/api";

export default function App() {
  // --- STATE (DURUM) YÖNETİMİ ---
  const [loggedIn, setLoggedIn] = useState(false);
  const [token, setToken] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  
  // Giriş Formu
  const [email, setEmail] = useState("yonetici@proje.com");
  const [password, setPassword] = useState("");
  const [errorMsg, setErrorMsg] = useState("");

  // Veriler
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(false);

  // Modal (Popup)
  const [showModal, setShowModal] = useState(false);
  const [newTask, setNewTask] = useState({
    baslik: "",
    aciklama: "",
    kategoriID: 1,
    oncelikID: 2
  });

  // --- FONKSİYONLAR ---

  // 1. Giriş Yap
  const handleLogin = async () => {
    setLoading(true);
    setErrorMsg("");
    try {
      const response = await axios.post(`${API_BASE_URL}/auth/login`, {
        email: email,
        sifre: password
      });
      
      // Token'ı kaydet ve içeri al
      setToken(response.data.token);
      setLoggedIn(true);
      fetchTasks(response.data.token); // Hemen listeyi çek

    } catch (error) {
      console.error(error);
      setErrorMsg("Giriş başarısız. Lütfen bilgilerinizi kontrol edin.");
    } finally {
      setLoading(false);
    }
  };

  // 2. Listeyi Çek
  const fetchTasks = async (authToken) => {
    try {
      const response = await axios.get(`${API_BASE_URL}/isemirleri`, {
        headers: { Authorization: `Bearer ${authToken}` }
      });
      setTasks(response.data);
    } catch (error) {
      console.error("Veri çekilemedi", error);
    }
  };

  // 3. Yeni İş Emri Ekle
  const handleCreate = async () => {
    if(!newTask.baslik || !newTask.aciklama) {
      alert("Lütfen başlık ve açıklama girin.");
      return;
    }

    try {
      await axios.post(`${API_BASE_URL}/isemirleri`, newTask, {
        headers: { Authorization: `Bearer ${token}` }
      });
      
      // Temizlik ve Yenileme
      setShowModal(false);
      setNewTask({ baslik: "", aciklama: "", kategoriID: 1, oncelikID: 2 });
      fetchTasks(token); // Listeyi güncelle
      alert("İş emri başarıyla oluşturuldu.");

    } catch (error) {
      alert("Bir hata oluştu.");
    }
  };

  // Yardımcı: Tarih Formatla
  const formatDate = (dateStr) => {
    const date = new Date(dateStr);
    return date.toLocaleDateString("tr-TR");
  };

  // --- ARAYÜZ (RENDER) ---

  // EKRAN 1: GİRİŞ EKRANI
  if (!loggedIn) {
    return (
      <div className="min-h-screen bg-gray-100 flex items-center justify-center p-4">
        <div className="bg-white p-8 rounded-2xl shadow-xl w-full max-w-md text-center">
          <div className="bg-gray-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4">
            <Shield className="w-8 h-8 text-slate-700" />
          </div>
          <h1 className="text-xl font-bold text-slate-800">Yönetici Paneline Hoş Geldiniz</h1>
          <p className="text-sm text-gray-500 mb-8 mt-2">Lütfen hesabınızla giriş yapın</p>

          <div className="text-left space-y-4">
            <div>
              <label className="text-xs font-bold text-gray-500 uppercase ml-1">E-posta</label>
              <input 
                type="email" 
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="w-full mt-1 p-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-600 outline-none bg-gray-50"
                placeholder="E-posta adresinizi girin"
              />
            </div>
            <div>
              <label className="text-xs font-bold text-gray-500 uppercase ml-1">Şifre</label>
              <div className="relative mt-1">
                <input 
                  type={showPassword ? "text" : "password"} 
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  className="w-full p-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-600 outline-none bg-gray-50"
                  placeholder="Şifrenizi girin"
                />
                <button 
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-3.5 text-gray-400 hover:text-gray-600"
                >
                  {showPassword ? <EyeOff size={20}/> : <Eye size={20}/>}
                </button>
              </div>
            </div>
          </div>

          {errorMsg && <p className="text-red-500 text-sm mt-4">{errorMsg}</p>}

          <button 
            onClick={handleLogin}
            disabled={loading}
            className="w-full bg-blue-700 hover:bg-blue-800 text-white py-3.5 rounded-xl mt-8 font-semibold transition shadow-lg shadow-blue-700/20"
          >
            {loading ? "Giriş Yapılıyor..." : "Yönetici Girişi"}
          </button>
          
          <button className="mt-4 text-sm text-blue-600 font-medium hover:underline">
            Şifremi Unuttum?
          </button>
        </div>
      </div>
    );
  }

  // EKRAN 2: YÖNETİM PANELİ
  return (
    <div className="min-h-screen flex bg-gray-50 font-sans text-slate-800">
      
      {/* SIDEBAR */}
      <aside className="w-64 bg-[#0F172A] text-white flex flex-col fixed h-full z-10">
        <div className="p-6 flex items-center gap-3 border-b border-slate-800">
          <div className="bg-blue-600 p-2 rounded-lg">
            <LayoutDashboard size={20} className="text-white" />
          </div>
          <div>
            <h2 className="font-bold text-sm tracking-wide">YÖNETİCİ PANELİ</h2>
            <p className="text-xs text-slate-400">admin@sirket.com</p>
          </div>
        </div>

        <div className="p-4">
          <div className="flex items-center gap-3 p-3 mb-6 bg-slate-800/50 rounded-xl border border-slate-700">
            <div className="w-10 h-10 rounded-full bg-slate-600 flex items-center justify-center">
              <User size={20} />
            </div>
            <div>
              <p className="text-sm font-medium">Yönetici Adı</p>
              <p className="text-xs text-slate-400">Sistem Yöneticisi</p>
            </div>
          </div>

          <nav className="space-y-1">
            <SidebarItem icon={<LayoutDashboard size={18}/>} text="Dashboard" />
            <SidebarItem icon={<ClipboardList size={18}/>} text="İş Emirleri" active />
            <SidebarItem icon={<Users size={18}/>} text="Personeller" />
            <SidebarItem icon={<Settings size={18}/>} text="Ayarlar" />
          </nav>
        </div>

        <div className="mt-auto p-4 border-t border-slate-800">
          <button onClick={() => setLoggedIn(false)} className="flex items-center gap-3 text-slate-400 hover:text-white transition w-full p-2 rounded-lg hover:bg-slate-800">
            <LogOut size={18} /> Çıkış Yap
          </button>
        </div>
      </aside>

      {/* ANA İÇERİK */}
      <main className="flex-1 ml-64 p-8">
        <div className="flex justify-between items-end mb-8">
          <div>
            <h1 className="text-3xl font-bold text-slate-900">İş Emirleri</h1>
            <p className="text-slate-500 mt-1">Sistemdeki tüm aktif görevleri buradan yönetebilirsiniz.</p>
          </div>
          <button 
            onClick={() => setShowModal(true)}
            className="bg-[#0F172A] hover:bg-slate-800 text-white px-5 py-3 rounded-xl font-medium flex items-center gap-2 shadow-lg transition"
          >
            <Plus size={20} /> Yeni İş Emri Oluştur
          </button>
        </div>

        {/* FİLTRELER */}
        <div className="bg-white p-4 rounded-xl shadow-sm border border-gray-100 mb-6 flex gap-4">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-3 text-gray-400" size={20} />
            <input type="text" placeholder="İş emri kodu veya başlık ara..." className="w-full pl-10 p-2.5 border border-gray-200 rounded-lg outline-none focus:border-blue-500" />
          </div>
          <select className="border border-gray-200 rounded-lg px-4 py-2 bg-white text-sm text-slate-600 outline-none"><option>Kategori</option></select>
          <select className="border border-gray-200 rounded-lg px-4 py-2 bg-white text-sm text-slate-600 outline-none"><option>Öncelik</option></select>
          <select className="border border-gray-200 rounded-lg px-4 py-2 bg-white text-sm text-slate-600 outline-none"><option>Durum</option></select>
        </div>

        {/* TABLO */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-gray-50 text-xs font-bold text-gray-500 uppercase tracking-wider border-b border-gray-100">
                <th className="p-4">ID</th>
                <th className="p-4">BAŞLIK</th>
                <th className="p-4">KATEGORİ</th>
                <th className="p-4">ÖNCELİK</th>
                <th className="p-4">DURUM</th>
                <th className="p-4">TARİH</th>
                <th className="p-4 text-right">EYLEMLER</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-50">
              {tasks.map((task) => (
                <tr key={task.isEmriID} className="hover:bg-blue-50/30 transition">
                  <td className="p-4 text-gray-500 font-mono text-sm">#{task.isEmriID}</td>
                  <td className="p-4 font-semibold text-slate-700">{task.baslik}</td>
                  <td className="p-4 text-sm text-slate-600">{task.kategoriAdi}</td>
                  <td className="p-4">
                    <span className={`px-2 py-1 rounded text-xs font-bold ${
                      task.oncelikAdi === 'Yüksek' ? 'bg-red-50 text-red-600' : 
                      task.oncelikAdi === 'Orta' ? 'bg-yellow-50 text-yellow-600' : 'bg-green-50 text-green-600'
                    }`}>
                      {task.oncelikAdi}
                    </span>
                  </td>
                  <td className="p-4">
                    <Badge status={task.durumAdi} />
                  </td>
                  <td className="p-4 text-sm text-gray-500">{formatDate(task.olusturmaTarihi)}</td>
                  <td className="p-4 text-right flex justify-end gap-2">
                    <button className="p-1.5 hover:bg-gray-100 rounded text-gray-400 hover:text-blue-600"><Edit size={16} /></button>
                    <button className="p-1.5 hover:bg-gray-100 rounded text-gray-400 hover:text-red-600"><Trash2 size={16} /></button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {tasks.length === 0 && (
            <div className="p-12 text-center text-gray-400 text-sm">Kayıt bulunamadı.</div>
          )}
        </div>
        
        <div className="flex justify-between items-center mt-4 text-xs text-gray-500">
          <span>Sayfa 1 / 10</span>
          <div className="flex gap-2">
            <button className="px-3 py-1 border rounded hover:bg-gray-50">Önceki</button>
            <button className="px-3 py-1 border rounded hover:bg-gray-50">Sonraki</button>
          </div>
        </div>
      </main>

      {/* MODAL (POPUP) */}
      {showModal && (
        <div className="fixed inset-0 bg-slate-900/40 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-2xl shadow-2xl w-full max-w-2xl overflow-hidden animate-in fade-in zoom-in duration-200">
            <div className="p-6 border-b border-gray-100 flex justify-between items-center">
              <div>
                <h2 className="text-lg font-bold text-slate-800">Yeni İş Emri Oluştur</h2>
                <p className="text-xs text-gray-500">Lütfen iş emri detaylarını girin.</p>
              </div>
              <button onClick={() => setShowModal(false)} className="text-gray-400 hover:text-gray-600 p-1 hover:bg-gray-100 rounded-full transition">
                <X size={20} />
              </button>
            </div>
            
            <div className="p-6 space-y-5">
              <div>
                <label className="block text-sm font-semibold text-slate-700 mb-1.5">Başlık</label>
                <input 
                  type="text" 
                  className="w-full p-3 border border-gray-200 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none text-sm"
                  placeholder="İş emri başlığını girin"
                  onChange={(e) => setNewTask({...newTask, baslik: e.target.value})}
                />
              </div>

              <div>
                <label className="block text-sm font-semibold text-slate-700 mb-1.5">Açıklama</label>
                <textarea 
                  className="w-full p-3 border border-gray-200 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none text-sm h-24 resize-none"
                  placeholder="Detaylı bir açıklama yazın..."
                  onChange={(e) => setNewTask({...newTask, aciklama: e.target.value})}
                ></textarea>
              </div>

              <div className="grid grid-cols-2 gap-6">
                <div>
                  <label className="block text-sm font-semibold text-slate-700 mb-1.5">Kategori</label>
                  <select 
                    className="w-full p-3 border border-gray-200 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none text-sm bg-white"
                    onChange={(e) => setNewTask({...newTask, kategoriID: parseInt(e.target.value)})}
                  >
                    <option value="1">Bakım</option>
                    <option value="2">Arıza</option>
                    <option value="3">Montaj</option>
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-semibold text-slate-700 mb-1.5">Öncelik</label>
                  <select 
                    className="w-full p-3 border border-gray-200 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none text-sm bg-white"
                    onChange={(e) => setNewTask({...newTask, oncelikID: parseInt(e.target.value)})}
                  >
                    <option value="1">Düşük</option>
                    <option value="2">Orta</option>
                    <option value="3">Yüksek</option>
                  </select>
                </div>
              </div>
            </div>

            <div className="p-4 bg-gray-50 flex justify-end gap-3 border-t border-gray-100">
              <button onClick={() => setShowModal(false)} className="px-5 py-2.5 text-slate-600 font-medium hover:bg-gray-200 rounded-lg transition text-sm">İptal</button>
              <button onClick={handleCreate} className="px-5 py-2.5 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700 shadow-md transition text-sm">Oluştur</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

// --- YARDIMCI BİLEŞENLER ---

function SidebarItem({ icon, text, active }) {
  return (
    <div className={`flex items-center gap-3 px-3 py-2.5 rounded-lg cursor-pointer transition ${active ? 'bg-blue-600 text-white shadow-md' : 'text-slate-400 hover:bg-slate-800 hover:text-white'}`}>
      {icon}
      <span className="text-sm font-medium">{text}</span>
    </div>
  );
}

function Badge({ status }) {
  const styles = {
    'Tamamlandı': 'bg-emerald-100 text-emerald-700 border-emerald-200',
    'Devam Ediyor': 'bg-amber-100 text-amber-700 border-amber-200',
    'Atandı': 'bg-blue-100 text-blue-700 border-blue-200',
    'İptal Edildi': 'bg-red-100 text-red-700 border-red-200',
  };
  
  const defaultStyle = 'bg-gray-100 text-gray-700 border-gray-200';

  return (
    <span className={`px-2.5 py-1 rounded-full text-xs font-bold border ${styles[status] || defaultStyle}`}>
      {status}
    </span>
  );
}