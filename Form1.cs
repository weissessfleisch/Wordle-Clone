using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wordle
{
    public partial class Form1 : Form
    {
        /*  PROJE TASARIMI VE HAYAT KALİTESİ NOTLARI:
        
          - txtTahmin kutusunun MaxLength özelliği Properties panelinden 5 olarak ayarlanmıştır. (Geçersiz uzunlukta girişleri arayüzde engellemek için)
          - txtTahmin kontrolünün KeyPress eventi yakalanarak, klavyeden girilen karakterlerin anlık olarak Türk alfabesine uygun (ı -> I, i -> İ) büyütülmesi sağlanmıştır. (Karakter uyuşmazlığını önlemek için)
          - Formun AcceptButton özelliği Properties panelinden btnTahmin olarak ayarlanmıştır. (Kullanıcının her seferinde butona tıklamak yerine ENTER tuşu ile hızlı tahmin yapabilmesi için)
          - Arayüzdeki kutuların (Label) hizalamaları MiddleCenter yapılarak fontların kutuyu ortalaması sağlanmıştır.
         
        */

        //  Değişkenleri ve kelime havuzunu global olarak tanımlıyoruz
        string[] kelimeHavuzu = {
            "KALEM", "KABAK", "BEBEK", "KİLİT", "KİTAP", "MASAL", "KANAT", "DENİZ", "BULUT", "ÇİÇEK",
            "KÖPEK", "KUMRU", "KAŞIK", "ÇATAL", "TABAK", "FIRIN", "DOLAP", "MASKE", "PERDE", "YATAK",
            "SAHİL", "ORMAN", "BAHÇE", "MEYVE", "SEBZE", "ŞEKER", "TUZLU", "TATLI", "EKMEK", "SİMİT",
            "BÖREK", "KİRAZ", "ARMUT", "İNCİR", "LİMON", "KAVUN", "ÇİLEK", "GÜNEŞ", "DÜNYA", "KÖPRÜ",
            "KAĞIT", "KUTUP", "KURAL", "SABUN", "HAVLU", "KİRLİ", "TEMİZ", "ÇEKİÇ", "TORBA", "ŞAPKA",
            "BİLET", "RADYO", "VİDEO", "KANAL", "EKRAN", "KABLO", "TAVUK", "HOROZ", "BALIK", "KARGA",
            "ASLAN", "YILAN", "AKREP", "MARTI", "TİLKİ", "ÇORAP", "KAZAK", "CEKET", "YÜZÜK", "DUVAR",
            "TAVAN", "ZEMİN", "MOTOR", "ŞOFÖR", "YOLCU", "SOKAK", "CADDE", "ŞEHİR", "KASAP", "MANAV",
            "PAZAR", "ÇANTA", "SİLGİ", "NOKTA", "ATLAS", "SATIR", "SÜTUN", "TABLO", "FİZİK", "TARİH",
            "SINAV", "SINIF", "DALGA", "YAMAÇ", "NEHİR", "IRMAK", "DAMLA", "ÇAMUR", "BUHAR", "DUMAN"
        };

        string gizliKelime = ""; // Seçilen kelimeyi burada tutacağız
        int aktifSatir = 0;      // Kaçıncı tahminde olduğumuzu takip etmek için (0'dan 6'ya kadar)
        Random rastgele = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        // Oyun Açıldığında Çalışacak Kodlar
        private void Form1_Load(object sender, EventArgs e)
        {
            OyunuBaslat();
            lblTahmin.Text = "Tahmin: 0 / 6";
        }

        // Rastgele Kelime Seçme Metodu
        // Ayrı bir metot olarak tanımlandı çünkü "Yeni Oyun" butonuna basıldığında da aynı işlemin çağrılması gerekiyor.
        private void OyunuBaslat()
        {
            // Havuzdan 0 ile 99 arasında rastgele bir indeks seç ve gizli kelimeye ata
            int rastgeleIndeks = rastgele.Next(0, kelimeHavuzu.Length);
            gizliKelime = kelimeHavuzu[rastgeleIndeks];

            // Aktif satırı (tahmin hakkını) sıfırla
            aktifSatir = 0; // tahminHakki yerine aktifSatir ismini kullandım çünkü kutu bulma formülünde (satır*5) + i + 1 matematiğini kullanıyoruz tahminSayisi kullansaydık mantıksız olurdu.
        }

        private void btnTahmin_Click(object sender, EventArgs e)
        {
            // Kullanıcının metnini al
            string tahmin = txtTahmin.Text;

            // HATA KONTROLÜ 1: 5 Harf Kuralı
            if (tahmin.Length != 5)
            {
                MessageBox.Show("Lütfen 5 harfli bir kelime giriniz!", "Geçersiz Uzunluk", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Metodu burada sonlandırarak alt satırlara inmesini (tahmin hakkı gitmez) engelle
            }

            // HATA KONTROLÜ 2: Kelime Havuzu Kuralı
            // Contains metodu kullanılarak girilen kelimenin dizide olup olmadığı kontrol edilir
            if (!kelimeHavuzu.Contains(tahmin))
            {
                MessageBox.Show("Girdiğiniz kelime sözlükte bulunamadı!", "Geçersiz Kelime", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- EĞER KOD BURAYA KADAR GELDİYSE TAHMİN GEÇERLİDİR ---

            // Aktif satırdaki 5 Label'ı bularak harfleri içlerine atama işlemi
            Label[] satirKutulari = new Label[5];
            for (int i = 0; i < 5; i++)
            {
                // Kutu isimleri label1, label2 ... label30 şeklinde adlandırıldığı için dinamik olarak aranır
                int labelNo = (aktifSatir * 5) + i + 1;
                satirKutulari[i] = (Label)this.Controls.Find("label" + labelNo, true)[0];

                satirKutulari[i].Text = tahmin[i].ToString(); // Harfi kutunun Text özelliğine ata
            }

            // Renk Değerlendirme Algoritması İçin Hazırlık
            char[] gizliHarfler = gizliKelime.ToCharArray();
            char[] tahminHarfler = tahmin.ToCharArray();
            Color[] kutuRenkleri = new Color[5];
            bool[] gizliKullanildi = new bool[5]; // Aynı harfi iki kere sarı yapmamak için takip dizisi

            // Kutuların BackColor özelliği başlangıçta GRİ olarak ayarlanır
            for (int i = 0; i < 5; i++) kutuRenkleri[i] = Color.Gray;

            // Sadece YEŞİLLERİ (Doğru Konum) Bulma İşlemi
            for (int i = 0; i < 5; i++)
            {
                if (tahminHarfler[i] == gizliHarfler[i])
                {
                    kutuRenkleri[i] = Color.Green;
                    gizliKullanildi[i] = true; // Bu gizli harf eşleştiği için kilitlendi
                    tahminHarfler[i] = '*';    // 2. turda sarı algoritmasına girmemesi için işaretlendi
                }
            }

            // SARILARI (Yanlış Konum) Bulma İşlemi
            for (int i = 0; i < 5; i++)
            {
                if (tahminHarfler[i] == '*') continue; // Zaten yeşil olduysa bu harfi pas geç

                for (int j = 0; j < 5; j++)
                {
                    // Gizli kelimedeki harf daha önce eşleşmediyse ve harfler aynıysa
                    if (!gizliKullanildi[j] && tahminHarfler[i] == gizliHarfler[j])
                    {
                        kutuRenkleri[i] = Color.Goldenrod; // Sarı renk ataması
                        gizliKullanildi[j] = true;
                        break; // Aynı tahmin harfi için birden fazla sarı yakalamamak için iç döngüden çık
                    }
                }
            }

            // Renkleri kutuların özelliklerine uygula
            for (int i = 0; i < 5; i++)
            {
                satirKutulari[i].BackColor = kutuRenkleri[i];
                satirKutulari[i].ForeColor = Color.White;
            }

            // Kazanma ve Kaybetme Kontrolü
            if (tahmin == gizliKelime)
            {
                // Oyun bitmeden önce Text özelliği üzerinden sayacı güncelle
                lblTahmin.Text = "Tahmin: " + (aktifSatir + 1) + " / 6"; //  aktifSatir'ı 0'dan başlattığımız için aktifSatir+1 kullandık
                lblMesaj.Text = "Tebrikler! Oyunu Kazandınız!";
                lblMesaj.ForeColor = Color.Green;

                // Enabled özellikleri false yapılarak oyuna müdahale edilmesi engellenir
                txtTahmin.Enabled = false;
                btnTahmin.Enabled = false;
            }
            else
            {
                aktifSatir++; // Sonraki satıra geçmek için değişkeni bir artır (Aynı zamanda tahmin hakkı da artmış olur)

                if (aktifSatir == 6)
                {
                    lblTahmin.Text = "Tahmin: " + aktifSatir + " / 6"; // aktifSatir+1 kullanmadık çünkü zaten kaybetme koşulu aktifSatir == 6 olması
                    lblMesaj.Text = "Oyunu Kaybettiniz! Kelime: " + gizliKelime;
                    lblMesaj.ForeColor = Color.Red;
                    txtTahmin.Enabled = false;
                    btnTahmin.Enabled = false;

                    // Oyun kaybedildiğinde form üzerindeki tüm kontroller taranır
                    foreach (Control kontrol in this.Controls)
                    {
                        // Sadece Label tipinde olan ve Name özelliği StartsWith metoduna "label" parametresi gönderilerek filtrelenen oyun kutuları kırmızıya boyanır. Bu sayede oyunun "label" parametresiyle başlamayan genel (Wordle) ve alt başlığı (Kelime Tahmin Oyunu) renk değişikliğinden etkilenmezler.
                        if (kontrol is Label lbl && lbl.Name.StartsWith("label"))
                        {
                            lbl.BackColor = Color.Red;
                            lbl.ForeColor = Color.White;
                        }
                    }
                }
                else
                {
                    // Tahmin yanlışsa ve hak bitmediyse sayacı güncelle
                    lblTahmin.Text = "Tahmin: " + aktifSatir + " / 6";
                }
            }

            // Yeni tahmin için TextBox'ın içi boşaltılır ve "Focus" metodu çağrılarak imleç otomatik olarak kutuya odaklanır
            txtTahmin.Text = "";
            txtTahmin.Focus();
        }

        private void btnYeniOyun_Click(object sender, EventArgs e)
        {
            foreach (Control kontrol in this.Controls)
            {
                // StartsWith metodu ile filtrelenen oyun kutularının renkleri ve metinleri varsayılan değerlere döndürülür.
                // Ana başlıkların Name özelliği "label" ile başlamadığı için bu döngüden etkilenmezler.
                if (kontrol is Label lbl && lbl.Name.StartsWith("label"))
                {
                    lbl.Text = "";
                    lbl.BackColor = Color.White;
                    lbl.ForeColor = Color.Black;
                }
            }

            // Sayaçlar ve mesajlar sıfırlanır
            lblTahmin.Text = "Tahmin: 0 / 6";
            lblMesaj.Text = "";

            // Enabled özellikleri true yapılarak butonlar ve metin kutusu tekrar kullanıma açılır
            txtTahmin.Enabled = true;
            btnTahmin.Enabled = true;

            // Text temizlenip "Focus" metodu ile imleç konumlandırılır
            txtTahmin.Text = "";
            txtTahmin.Focus();

            // Yeni kelime seçilmesi için OyunuBaslat metodu çağrılır
            OyunuBaslat();
        }

        private void txtTahmin_KeyPress(object sender, KeyPressEventArgs e)
        {
            // En üstte hayat kalitesi kısmında bahsettiğim girilen harflerin otomatik olarak Türkçe'ye uygun şekilde büyütülmesini sağlayan event
            e.KeyChar = char.ToUpper(e.KeyChar, new System.Globalization.CultureInfo("tr-TR"));
        }
    }
}
