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
  User,
  Save,
  Filter,
  Mail,
  Briefcase
} from "lucide-react";

// --- AYARLAR ---
// Not: Port numarasının senin backend portunla (örn: 5266 veya 7079) aynı olduğundan emin ol.
const API_BASE_URL = "https://localhost:7079/api"; 

export default function App() {
  
  // --- STATE (DURUM) YÖNETİMİ ---
  const [loggedIn, setLoggedIn] = useState(false);
  const [token, setToken] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  
  // Tab Yönetimi
  const [activeTab, setActiveTab] = useState("tasks"); // 'tasks' veya 'personnel'

  // Giriş Formu
  const [email, setEmail] = useState("yonetici@proje.com");
  const [password, setPassword] = useState("");
  const [errorMsg, setErrorMsg] = useState("");

  // İş Emri Verileri
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(false);

  // Personel Verileri
  const [technicians, setTechnicians] = useState([]);

  // --- FİLTRELEME STATE'LERİ ---
  const [searchTerm, setSearchTerm] = useState("");
  const [filterKategori, setFilterKategori] = useState("");
  const [filterOncelik, setFilterOncelik] = useState("");
  const [filterDurum, setFilterDurum] = useState("");

  // Modal
  const [showModal, setShowModal] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [editingId, setEditingId] = useState(null);

  // Yeni/Düzenlenen Görev State'i
  const [newTask, setNewTask] = useState({
    baslik: "",
    aciklama: "",
    kategoriID: 1,
    oncelikID: 2,
    atananTeknisyenID: "" // Teknisyen seçimi için alan eklendi
  });

  // --- SAYFA YÜKLENİNCE ---
  useEffect(() => {
    const savedToken = localStorage.getItem("token");
    if (savedToken) {
      setToken(savedToken);
      setLoggedIn(true);
      fetchTasks(savedToken);
      fetchTechnicians(savedToken); // <-- ÖNEMLİ: Sayfa açılınca teknisyenleri de çekiyoruz ki dropdown dolsun.
    }
  }, []);

  // --- İSTATİSTİKLER ---
  const toplamIs = tasks.length;
  const tamamlananIs = tasks.filter(t => t.durumAdi === 'Tamamlandı').length;
  const devamEdenIs = tasks.filter(t => t.durumAdi === 'Devam Ediyor' || t.durumAdi === 'Atandı').length;
  const acilIs = tasks.filter(t => t.oncelikAdi === 'Yüksek' && t.durumAdi !== 'Tamamlandı').length;

  // --- FİLTRELEME MANTIĞI ---
  const filteredTasks = tasks.filter((task) => {
    const matchesSearch = 
      task.baslik.toLowerCase().includes(searchTerm.toLowerCase()) || 
      task.isEmriID.toString().includes(searchTerm);

    const matchesKategori = filterKategori === "" || task.kategoriAdi === filterKategori;
    const matchesOncelik = filterOncelik === "" || task.oncelikAdi === filterOncelik;
    const matchesDurum = filterDurum === "" || task.durumAdi === filterDurum;

    return matchesSearch && matchesKategori && matchesOncelik && matchesDurum;
  });

  // --- FONKSİYONLAR ---

  const handleLogin = async () => {
    setLoading(true);
    setErrorMsg("");
    try {
      const response = await axios.post(`${API_BASE_URL}/auth/login`, {
        email: email,
        sifre: password
      });
      const gelenToken = response.data.token;
      localStorage.setItem("token", gelenToken);
      setToken(gelenToken);
      setLoggedIn(true);
      fetchTasks(gelenToken); 
      fetchTechnicians(gelenToken); // Giriş yapınca da teknisyenleri çek
    } catch (error) {
      console.error(error);
      setErrorMsg("Giriş başarısız. Bilgileri kontrol edin.");
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem("token");
    setLoggedIn(false);
    setToken("");
    setTasks([]);
    setTechnicians([]);
    setActiveTab("tasks");
  };

  // İş Emirlerini Çek
  const fetchTasks = async (authToken) => {
    try {
      const response = await axios.get(`${API_BASE_URL}/isemirleri`, {
        headers: { Authorization: `Bearer ${authToken}` }
      });
      setTasks(response.data);
    } catch (error) {
      if(error.response && error.response.status === 401){
        handleLogout();
      }
    }
  };

  // Personelleri Çek
  const fetchTechnicians = async (authToken) => {
    try {
      const tokenToUse = authToken || token;
      const response = await axios.get(`${API_BASE_URL}/kullanicilar/teknisyenler`, {
         headers: { Authorization: `Bearer ${tokenToUse}` } 
      });
      setTechnicians(response.data);
    } catch (error) {
      console.error("Personel çekilemedi:", error);
    }
  };

  // Sidebar Tıklama Yönetimi
  const handleTabChange = (tabName) => {
    setActiveTab(tabName);
    if (tabName === "personnel") {
        fetchTechnicians(token);
    } else {
        fetchTasks(token);
    }
  };

  const openNewModal = () => {
    // Yeni görevde teknisyen boş gelsin
    setNewTask({ baslik: "", aciklama: "", kategoriID: 1, oncelikID: 2, atananTeknisyenID: "" });
    setIsEditing(false);
    setEditingId(null);
    setShowModal(true);
  };

  const openEditModal = (task) => {
    setNewTask({
        isEmriID: task.isEmriID, 
        baslik: task.baslik,
        aciklama: task.aciklama,
        kategoriID: task.kategoriID || 1, 
        oncelikID: task.oncelikID || 2,
        durumID: task.durumID || 1,
        olusturanYoneticiID: task.olusturanYoneticiID,
        atananTeknisyenID: task.atananTeknisyenID, // Mevcut teknisyeni seçili getir
        olusturmaTarihi: task.olusturmaTarihi
    });
    setIsEditing(true); 
    setEditingId(task.isEmriID);
    setShowModal(true);
  };

  const handleSave = async () => {
    if(!newTask.baslik || !newTask.aciklama) {
      alert("Lütfen başlık ve açıklama girin.");
      return;
    }

    const payload = {
      isEmriID: isEditing ? parseInt(editingId) : 0, 
      baslik: newTask.baslik,
      aciklama: newTask.aciklama,
      kategoriID: parseInt(newTask.kategoriID),
      oncelikID: parseInt(newTask.oncelikID),
      durumID: isEditing ? parseInt(newTask.durumID) : 1,
      olusturanYoneticiID: newTask.olusturanYoneticiID ? parseInt(newTask.olusturanYoneticiID) : 1, 
      
      // Teknisyen Seçimi Kontrolü:
      // Eğer kullanıcı seçim yaptıysa o ID'yi gönder, yapmadıysa listedeki ilk kişiyi veya varsayılan 2'yi gönder.
      atananTeknisyenID: newTask.atananTeknisyenID ? parseInt(newTask.atananTeknisyenID) : (technicians.length > 0 ? technicians[0].kullaniciID : 2),
      
      olusturmaTarihi: newTask.olusturmaTarihi || new Date().toISOString()
    };

    try {
      if (isEditing) {
        await axios.put(`${API_BASE_URL}/isemirleri/${editingId}`, payload, {
          headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' }
        });
        alert("Güncellendi!");
      } else {
        await axios.post(`${API_BASE_URL}/isemirleri`, payload, {
          headers: { Authorization: `Bearer ${token}` }
        });
        alert("Oluşturuldu!");
      }
      setShowModal(false);
      fetchTasks(token); 
    } catch (error) {
      alert(`Hata: ${JSON.stringify(error.response?.data?.errors || "Bilinmeyen hata")}`);
    }
  };

  const isEmriSil = async (id) => {
    if (window.confirm("Silmek istediğinize emin misiniz?")) {
      try {
        const currentToken = localStorage.getItem("token");
        await axios.delete(`${API_BASE_URL}/IsEmirleri/${id}`, {
          headers: { Authorization: `Bearer ${currentToken}` }
        });
        alert("Silindi!");
        fetchTasks(currentToken);
      } catch (error) {
        alert("Silinemedi.");
      }
    }
  };

  const formatDate = (dateStr) => {
    if(!dateStr) return "-";
    const date = new Date(dateStr);
    return date.toLocaleDateString("tr-TR");
  };

  // --- ARAYÜZ ---

  if (!loggedIn) {
    return (
      <div className="min-h-screen bg-gray-100 flex items-center justify-center p-4">
        <div className="bg-white p-8 rounded-2xl shadow-xl w-full max-w-md text-center">
          <div className="bg-gray-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4">
            <Shield className="w-8 h-8 text-slate-700" />
          </div>
          <h1 className="text-xl font-bold text-slate-800">Yönetici Girişi</h1>
          <div className="mt-6 space-y-4 text-left">
            <div>
              <label className="text-xs font-bold text-gray-500 uppercase ml-1">E-posta</label>
              <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} className="w-full mt-1 p-3 border border-gray-200 rounded-xl bg-gray-50" placeholder="admin@sirket.com" />
            </div>
            <div>
              <label className="text-xs font-bold text-gray-500 uppercase ml-1">Şifre</label>
              <div className="relative mt-1">
                <input type={showPassword ? "text" : "password"} value={password} onChange={(e) => setPassword(e.target.value)} className="w-full p-3 border border-gray-200 rounded-xl bg-gray-50" placeholder="******" />
                <button onClick={() => setShowPassword(!showPassword)} className="absolute right-3 top-3.5 text-gray-400">{showPassword ? <EyeOff size={20}/> : <Eye size={20}/>}</button>
              </div>
            </div>
          </div>
          <button onClick={handleLogin} disabled={loading} className="w-full bg-blue-700 hover:bg-blue-800 text-white py-3.5 rounded-xl mt-8 font-semibold transition shadow-lg shadow-blue-700/20">
            {loading ? "Giriş Yapılıyor..." : "Giriş Yap"}
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex bg-gray-50 font-sans text-slate-800">
      
      {/* SIDEBAR */}
      <aside className="w-64 bg-[#0F172A] text-white flex flex-col fixed h-full z-10">
        <div className="p-6 flex items-center gap-3 border-b border-slate-800">
          <div className="bg-blue-600 p-2 rounded-lg"><LayoutDashboard size={20} className="text-white" /></div>
          <div><h2 className="font-bold text-sm tracking-wide">YÖNETİCİ PANELİ</h2></div>
        </div>
        <div className="p-4">
            <nav className="space-y-1">
            <SidebarItem 
                icon={<ClipboardList size={18}/>} 
                text="İş Emirleri" 
                active={activeTab === 'tasks'} 
                onClick={() => handleTabChange('tasks')}
            />
            <SidebarItem 
                icon={<Users size={18}/>} 
                text="Personeller" 
                active={activeTab === 'personnel'} 
                onClick={() => handleTabChange('personnel')}
            />
            </nav>
        </div>
        <div className="mt-auto p-4 border-t border-slate-800">
          <button onClick={handleLogout} className="flex items-center gap-3 text-slate-400 hover:text-white transition w-full p-2 rounded-lg hover:bg-slate-800"><LogOut size={18} /> Çıkış Yap</button>
        </div>
      </aside>

      {/* ANA İÇERİK */}
      <main className="flex-1 ml-64 p-8">
        
        {/* --- DURUMA GÖRE İÇERİK DEĞİŞİMİ --- */}
        
        {activeTab === 'tasks' ? (
          <>
            {/* İŞ EMİRLERİ SAYFASI */}
            
            {/* DASHBOARD KARTLARI */}
            <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
              <StatCard title="Toplam İş" value={toplamIs} color="blue" icon={<ClipboardList size={24} />} />
              <StatCard title="Devam Eden" value={devamEdenIs} color="amber" icon={<LayoutDashboard size={24} />} />
              <StatCard title="Tamamlanan" value={tamamlananIs} color="emerald" icon={<Shield size={24} />} />
              <StatCard title="Acil Bekleyen" value={acilIs} color="red" icon={<Settings size={24} />} />
            </div>

            <div className="flex justify-between items-end mb-6">
              <div>
                <h1 className="text-3xl font-bold text-slate-900">İş Emirleri</h1>
                <p className="text-slate-500 mt-1">Tüm işlerinizi buradan yönetin ve filtreleyin.</p>
              </div>
              <button onClick={openNewModal} className="bg-[#0F172A] hover:bg-slate-800 text-white px-5 py-3 rounded-xl font-medium flex items-center gap-2 shadow-lg transition">
                <Plus size={20} /> Yeni İş Emri
              </button>
            </div>

            {/* FİLTRELEME ALANI */}
            <div className="bg-white p-4 rounded-xl shadow-sm border border-gray-100 mb-6 flex flex-wrap gap-4 items-center">
              <div className="relative flex-1 min-w-[200px]">
                <Search className="absolute left-3 top-3 text-gray-400" size={20} />
                <input 
                  type="text" 
                  placeholder="ID veya Başlık Ara..." 
                  className="w-full pl-10 p-2.5 border border-gray-200 rounded-lg outline-none focus:border-blue-500 transition" 
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
              </div>
              
              <div className="flex items-center gap-2">
                <Filter size={20} className="text-slate-400" />
                <select className="border border-gray-200 rounded-lg px-4 py-2.5 bg-white text-sm text-slate-600 outline-none focus:border-blue-500 cursor-pointer" value={filterKategori} onChange={(e) => setFilterKategori(e.target.value)}>
                  <option value="">Tüm Kategoriler</option><option value="Bakım">Bakım</option><option value="Arıza">Arıza</option><option value="Montaj">Montaj</option>
                </select>

                <select className="border border-gray-200 rounded-lg px-4 py-2.5 bg-white text-sm text-slate-600 outline-none focus:border-blue-500 cursor-pointer" value={filterOncelik} onChange={(e) => setFilterOncelik(e.target.value)}>
                  <option value="">Tüm Öncelikler</option><option value="Düşük">Düşük</option><option value="Orta">Orta</option><option value="Yüksek">Yüksek</option>
                </select>

                <select className="border border-gray-200 rounded-lg px-4 py-2.5 bg-white text-sm text-slate-600 outline-none focus:border-blue-500 cursor-pointer" value={filterDurum} onChange={(e) => setFilterDurum(e.target.value)}>
                  <option value="">Tüm Durumlar</option><option value="Bekliyor">Bekliyor</option><option value="Atandı">Atandı</option><option value="Devam Ediyor">Devam Ediyor</option><option value="Tamamlandı">Tamamlandı</option>
                </select>
                
                {(filterKategori || filterOncelik || filterDurum || searchTerm) && (
                   <button onClick={() => {setFilterKategori(""); setFilterOncelik(""); setFilterDurum(""); setSearchTerm("");}} className="text-red-500 text-sm font-medium hover:underline px-2">Temizle</button>
                )}
              </div>
            </div>

            {/* İŞ TABLOSU */}
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
                    <th className="p-4 text-center">EYLEMLER</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-50">
                  {filteredTasks.map((task) => (
                    <tr key={task.isEmriID} className="hover:bg-blue-50/30 transition">
                      <td className="p-4 text-gray-500 font-mono text-sm">#{task.isEmriID}</td>
                      <td className="p-4 font-semibold text-slate-700">{task.baslik}</td>
                      <td className="p-4 text-sm text-slate-600">{task.kategoriAdi}</td>
                      <td className="p-4">
                        <span className={`px-2 py-1 rounded text-xs font-bold ${
                          task.oncelikAdi === 'Yüksek' ? 'bg-red-50 text-red-600' : 
                          task.oncelikAdi === 'Orta' ? 'bg-yellow-50 text-yellow-600' : 'bg-green-50 text-green-600'
                        }`}>{task.oncelikAdi}</span>
                      </td>
                      <td className="p-4"><Badge status={task.durumAdi} /></td>
                      <td className="p-4 text-sm text-gray-500">{formatDate(task.olusturmaTarihi)}</td>
                      <td className="p-4 text-center flex justify-center gap-2">
                        <button onClick={() => openEditModal(task)} className="p-2 hover:bg-blue-50 rounded text-blue-400 hover:text-blue-600 transition" title="Düzenle"><Edit size={18} /></button>
                        <button onClick={() => isEmriSil(task.isEmriID)} className="p-2 hover:bg-red-50 rounded text-red-400 hover:text-red-600 transition" title="Sil"><Trash2 size={18} /></button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
              {filteredTasks.length === 0 && (<div className="p-12 text-center text-gray-400 text-sm">Kayıt bulunamadı.</div>)}
            </div>
            <div className="mt-4 text-xs text-gray-500 text-right">Toplam {filteredTasks.length} sonuç listelendi.</div>
          </>

        ) : (
          <>
            {/* PERSONELLER SAYFASI */}
            <div className="flex justify-between items-end mb-6 animate-in fade-in duration-300">
              <div>
                <h1 className="text-3xl font-bold text-slate-900">Personel Yönetimi</h1>
                <p className="text-slate-500 mt-1">Sistemde kayıtlı teknisyenleri görüntüleyin.</p>
              </div>
              <button className="bg-white border border-gray-200 text-slate-600 px-5 py-3 rounded-xl font-medium flex items-center gap-2 shadow-sm transition cursor-default">
                 <Users size={20} className="text-blue-600"/> Toplam: {technicians.length}
              </button>
            </div>

            <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden animate-in fade-in duration-500">
              <table className="w-full text-left border-collapse">
                <thead>
                  <tr className="bg-gray-50 text-xs font-bold text-gray-500 uppercase tracking-wider border-b border-gray-100">
                    <th className="p-4 w-24">ID</th> 
                    <th className="p-4">AD SOYAD</th>
                    <th className="p-4">E-POSTA</th>
                    <th className="p-4">ÜNVAN</th>
                    <th className="p-4 text-center">DURUM</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-50">
                  {technicians.length > 0 ? (
                    technicians.map((person) => (
                      <tr key={person.kullaniciID} className="hover:bg-blue-50/30 transition align-middle">
                        
                        <td className="p-4 text-gray-500 font-mono text-sm">
                          #{person.kullaniciID}
                        </td>

                        <td className="p-4 font-semibold text-slate-700">
                          <div className="flex items-center gap-3">
                              <div className="bg-blue-100 p-2 rounded-full text-blue-600">
                                <User size={16}/>
                              </div>
                              {person.adSoyad}
                          </div>
                        </td>

                        <td className="p-4 text-sm text-slate-600">
                          <div className="flex items-center gap-2">
                            <Mail size={14} className="text-gray-400"/> 
                            {person.email}
                          </div>
                        </td>

                        <td className="p-4">
                          <div className="flex items-center gap-2 text-sm text-slate-700">
                            <Briefcase size={14} className="text-gray-400"/>
                            <span className="bg-gray-100 text-gray-600 px-2 py-1 rounded text-xs font-bold border border-gray-200">
                              {person.unvan || "Teknisyen"}
                            </span>
                          </div>
                        </td>

                        <td className="p-4 text-center">
                          <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium bg-emerald-100 text-emerald-800 border border-emerald-200">
                            <span className="w-1.5 h-1.5 rounded-full bg-emerald-600"></span>
                            Aktif
                          </span>
                        </td>

                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td colSpan="5" className="p-12 text-center text-gray-400 text-sm">
                        Henüz teknisyen bulunamadı veya liste yükleniyor...
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          </>
        )}

      </main>

      {/* MODAL (POPUP) */}
      {showModal && (
        <div className="fixed inset-0 bg-slate-900/40 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-2xl shadow-2xl w-full max-w-2xl overflow-hidden animate-in fade-in zoom-in duration-200">
            <div className="p-6 border-b border-gray-100 flex justify-between items-center">
              <div>
                <h2 className="text-lg font-bold text-slate-800">{isEditing ? "İş Emrini Düzenle" : "Yeni İş Emri Oluştur"}</h2>
                <p className="text-xs text-gray-500">{isEditing ? `#${editingId} düzenleniyor.` : "Detayları girin."}</p>
              </div>
              <button onClick={() => setShowModal(false)} className="text-gray-400 hover:text-gray-600 p-1 hover:bg-gray-100 rounded-full transition"><X size={20} /></button>
            </div>
            
            <div className="p-6 space-y-5">
              <div>
                <label className="block text-sm font-semibold text-slate-700 mb-1.5">Başlık</label>
                <input type="text" className="w-full p-3 border border-gray-200 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none text-sm" value={newTask.baslik} onChange={(e) => setNewTask({...newTask, baslik: e.target.value})} />
              </div>
              <div>
                <label className="block text-sm font-semibold text-slate-700 mb-1.5">Açıklama</label>
                <textarea className="w-full p-3 border border-gray-200 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none text-sm h-24 resize-none" value={newTask.aciklama} onChange={(e) => setNewTask({...newTask, aciklama: e.target.value})}></textarea>
              </div>
              <div className="grid grid-cols-2 gap-6">
                <div>
                  <label className="block text-sm font-semibold text-slate-700 mb-1.5">Kategori</label>
                  <select className="w-full p-3 border border-gray-200 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none text-sm bg-white" value={newTask.kategoriID} onChange={(e) => setNewTask({...newTask, kategoriID: parseInt(e.target.value)})}>
                    <option value="1">Bakım</option><option value="2">Arıza</option><option value="3">Montaj</option>
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-semibold text-slate-700 mb-1.5">Öncelik</label>
                  <select className="w-full p-3 border border-gray-200 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none text-sm bg-white" value={newTask.oncelikID} onChange={(e) => setNewTask({...newTask, oncelikID: parseInt(e.target.value)})}>
                    <option value="1">Düşük</option><option value="2">Orta</option><option value="3">Yüksek</option>
                  </select>
                </div>
              </div>

              {/* --- YENİ EKLENEN KISIM: TEKNİSYEN SEÇİMİ --- */}
              <div>
                 <label className="block text-sm font-semibold text-slate-700 mb-1.5">Teknisyen Ata</label>
                 <select 
                    className="w-full p-3 border border-gray-200 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none text-sm bg-white"
                    value={newTask.atananTeknisyenID}
                    onChange={(e) => setNewTask({...newTask, atananTeknisyenID: parseInt(e.target.value)})}
                 >
                    <option value="">Otomatik / İlk Uygun</option>
                    {technicians.map((tech) => (
                        <option key={tech.kullaniciID} value={tech.kullaniciID}>
                            {tech.adSoyad} ({tech.unvan || "Teknisyen"})
                        </option>
                    ))}
                 </select>
              </div>

            </div>
            <div className="p-4 bg-gray-50 flex justify-end gap-3 border-t border-gray-100">
              <button onClick={() => setShowModal(false)} className="px-5 py-2.5 text-slate-600 font-medium hover:bg-gray-200 rounded-lg transition text-sm">İptal</button>
              <button onClick={handleSave} className="px-5 py-2.5 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700 shadow-md transition text-sm flex items-center gap-2"><Save size={18} /> {isEditing ? "Güncelle" : "Oluştur"}</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

// --- YARDIMCI BİLEŞENLER ---
function StatCard({ title, value, color, icon }) {
  const colors = {
    blue: "bg-blue-50 text-blue-600",
    amber: "bg-amber-50 text-amber-600",
    emerald: "bg-emerald-50 text-emerald-600",
    red: "bg-red-50 text-red-600"
  };
  return (
    <div className="bg-white p-5 rounded-xl shadow-sm border border-gray-100 flex items-center gap-4">
      <div className={`p-3 rounded-lg ${colors[color]}`}>{icon}</div>
      <div>
        <p className="text-sm text-gray-500 font-medium">{title}</p>
        <h3 className="text-2xl font-bold text-slate-800">{value}</h3>
      </div>
    </div>
  );
}

// SidebarItem
function SidebarItem({ icon, text, active, onClick }) {
  return (
    <div 
        onClick={onClick}
        className={`flex items-center gap-3 px-3 py-2.5 rounded-lg cursor-pointer transition ${active ? 'bg-blue-600 text-white shadow-md' : 'text-slate-400 hover:bg-slate-800 hover:text-white'}`}
    >
        {icon}
        <span className="text-sm font-medium">{text}</span>
    </div>
  );
}

function Badge({ status }) {
  const styles = { 'Tamamlandı': 'bg-emerald-100 text-emerald-700 border-emerald-200', 'Devam Ediyor': 'bg-amber-100 text-amber-700 border-amber-200', 'Atandı': 'bg-blue-100 text-blue-700 border-blue-200', 'İptal Edildi': 'bg-red-100 text-red-700 border-red-200' };
  return (<span className={`px-2.5 py-1 rounded-full text-xs font-bold border ${styles[status] || 'bg-gray-100 text-gray-700 border-gray-200'}`}>{status}</span>);
}