using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// frmAnaSayfa.cs — Ana Sayfa Code-Behind
// Sol menüden diğer formlara geçiş yapılır.
// Dashboard kartlarında özet sayılar gösterilir.
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CampStore.BusinessLayer;

namespace CampStore.UI
{
    public partial class frmAnaSayfa : Form
    {
        // BL nesneleri — dashboard sayılarını çekmek için
        private UrunBL urunBL = new UrunBL();
        private MusteriBL musteriBL = new MusteriBL();
        private SatisBL satisBL = new SatisBL();
        private PersonelBL personelBL = new PersonelBL();

        // Saat güncellemek için Timer
        private Timer saatTimer = new Timer();

        public frmAnaSayfa()
        {
            InitializeComponent();
            // Kontroller oluştuktan hemen sonra dashboard'ı yükle
            this.Shown += frmAnaSayfa_Shown;
             //Maximize ekle
            //this.WindowState = FormWindowState.Maximized;
            //this.MaximizeBox = false;
            //this.FormBorderStyle = FormBorderStyle.Sizable;
            //KontrolleriOlustur();
        }


        // ── FORM YÜKLENINCE ───────────────────────────────────────────────
        private void frmAnaSayfa_Load(object sender, EventArgs e)
        {
            // Hoş geldin mesajına giriş yapan personelin adını yaz
            lblHosgeldin.Text = $"Hoş geldiniz, " +
                $"{OturumBilgisi.AktifPersonel.PerAd} " +
                $"{OturumBilgisi.AktifPersonel.PerSoyad}";

            // Saati güncelle
            lblTarihSaat.Text = DateTime.Now.ToString("dd.MM.yyyy — HH:mm");

            // Timer: her saniye saati güncelle
            saatTimer.Interval = 1000;
            saatTimer.Tick += (s, ev) =>
            {
                lblTarihSaat.Text = DateTime.Now.ToString("dd.MM.yyyy — HH:mm:ss");
            };
            saatTimer.Start();
            dgvSonSatislar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;



        }

        // ── DASHBOARD SAYILARINI YÜKLE ────────────────────────────────────
        private void DashboardYukle()
        {
            try
            {
                dynamic urunler = ApiServis.Get<dynamic>("urun");
                dynamic musteriler = ApiServis.Get<dynamic>("musteri");
                dynamic satislar = ApiServis.Get<dynamic>("satis");
                dynamic personeller = ApiServis.Get<dynamic>("personel");

                // Satır sayısını hesapla
                lblUrunSayisi.Text = ((Newtonsoft.Json.Linq.JArray)urunler).Count.ToString();
                lblMusteriSayisi.Text = ((Newtonsoft.Json.Linq.JArray)musteriler).Count.ToString();
                lblSatisSayisi.Text = ((Newtonsoft.Json.Linq.JArray)satislar).Count.ToString();
                lblPersonelSayisi.Text = ((Newtonsoft.Json.Linq.JArray)personeller).Count.ToString();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Dashboard yüklenirken hata: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void DashboardYukle()
        //{
        //    try
        //    {
        //        DataTable dtUrun = urunBL.UrunListele();
        //        DataTable dtMusteri = musteriBL.MusteriListele();
        //        DataTable dtSatis = satisBL.SatisListele();
        //        DataTable dtPersonel = personelBL.PersonelListele();

        //        lblUrunSayisi.Text = dtUrun.Rows.Count.ToString();
        //        lblMusteriSayisi.Text = dtMusteri.Rows.Count.ToString();
        //        lblSatisSayisi.Text = dtSatis.Rows.Count.ToString();
        //        lblPersonelSayisi.Text = dtPersonel.Rows.Count.ToString();

        //        // Kritik stok kontrolü
        //        int kritikStok = 0;
        //        foreach (DataRow row in dtUrun.Rows)
        //        {
        //            if (Convert.ToInt32(row["Stok"]) <= 5)
        //                kritikStok++;
        //        }

        //        // Kritik stok varsa uyarı göster
        //        if (kritikStok > 0)
        //        {
        //            MessageBox.Show(
        //                $"⚠️ {kritikStok} ürünün stoğu kritik seviyede (5 ve altı)!\n" +
        //                "Lütfen stok takibi sayfasını kontrol edin.",
        //                "Stok Uyarısı",
        //                MessageBoxButtons.OK,
        //                MessageBoxIcon.Warning);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Dashboard yüklenirken hata: " + ex.Message,
        //            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        // ── SON SATIŞLARI YÜKLE ───────────────────────────────────────────
        private void SonSatislarYukle()
        {
            try
            {
                DataTable dt = satisBL.SatisListele();
                dgvSonSatislar.DataSource = dt;
            }
            catch { }
        }

        // ── DATAGRIDVIEW GÖRÜNÜM AYARI ────────────────────────────────────
        private void DgvAyarla()
        {
            dgvSonSatislar.EnableHeadersVisualStyles = false;
            dgvSonSatislar.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 52, 72);
            dgvSonSatislar.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSonSatislar.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvSonSatislar.DefaultCellStyle.BackColor = Color.FromArgb(27, 42, 59);
            dgvSonSatislar.DefaultCellStyle.ForeColor = Color.White;
            dgvSonSatislar.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 204, 113);
            dgvSonSatislar.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 34, 51);
            dgvSonSatislar.RowTemplate.Height = 30;
        }

        // ── MENÜ BUTON HOVER EFEKTLERİ ───────────────────────────────────
        // Tüm menü butonları için ortak hover metodu
        private void MenuBtn_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.BackColor != Color.FromArgb(46, 204, 113)) // aktif değilse
                btn.BackColor = Color.FromArgb(31, 52, 72);
        }

        private void MenuBtn_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.BackColor != Color.FromArgb(46, 204, 113)) // aktif değilse
                btn.BackColor = Color.FromArgb(22, 34, 51);
        }

        // ── AKTİF MENÜ BUTONU AYARLA ─────────────────────────────────────
        // Tıklanan butonu yeşil yapar, diğerlerini normale döndürür
        private void AktifMenuAyarla(Button aktifBtn)
        {
            Button[] menuButonlari = {
                btnAnaSayfa, btnUrun, btnKategori, btnMusteri,
                btnPersonel, btnSatis, btnFatura, btnStok, btnLog
            };

            foreach (Button btn in menuButonlari)
                btn.BackColor = Color.FromArgb(22, 34, 51);

            aktifBtn.BackColor = Color.FromArgb(46, 204, 113);
        }

        // ═══════════════════════════════════════════════════════════════
        // MENÜ BUTON TIKLAMA OLAYLARI
        // ═══════════════════════════════════════════════════════════════

        private void btnAnaSayfa_Click(object sender, EventArgs e)
        {
            AktifMenuAyarla(btnAnaSayfa);
            DashboardYukle();
            SonSatislarYukle();
        }

        private void btnUrun_Click(object sender, EventArgs e)
        {
            AktifMenuAyarla(btnUrun);
            frmUrun frm = new frmUrun();
            frm.ShowDialog(); // Modal olarak aç
        }

        private void btnKategori_Click(object sender, EventArgs e)
        {
            AktifMenuAyarla(btnKategori);
            frmKategori frm = new frmKategori();
            frm.ShowDialog();
        }

        private void btnMusteri_Click(object sender, EventArgs e)
        {
            AktifMenuAyarla(btnMusteri);
            frmMusteri frm = new frmMusteri();
            frm.ShowDialog();
        }

        private void btnPersonel_Click(object sender, EventArgs e)
        {
            AktifMenuAyarla(btnPersonel);
            frmPersonel frm = new frmPersonel();
            frm.ShowDialog();
        }

        private void btnSatis_Click(object sender, EventArgs e)
        {
            AktifMenuAyarla(btnSatis);
            frmSatis frm = new frmSatis();
            frm.ShowDialog();
        }

        private void btnFatura_Click(object sender, EventArgs e)
        {
            AktifMenuAyarla(btnFatura);
            frmFatura frm = new frmFatura();
            frm.ShowDialog();
        }

        private void btnStok_Click(object sender, EventArgs e)
        {
            AktifMenuAyarla(btnStok);
            frmStok frm = new frmStok();
            frm.ShowDialog();
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            AktifMenuAyarla(btnLog);
            frmLog frm = new frmLog();
            frm.ShowDialog();
        }

        // ── ÇIKIŞ YAP ────────────────────────────────────────────────────
        private void btnCikis_Click(object sender, EventArgs e)
        {
            DialogResult sonuc = MessageBox.Show(
                "Çıkış yapmak istediğinize emin misiniz?",
                "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (sonuc == DialogResult.Yes)
            {
                saatTimer.Stop();
                OturumBilgisi.Temizle(); // Oturum bilgisini temizle
                Application.Restart();  // Uygulamayı yeniden başlat (login'e dön)
            }
        }

        // ── FORM KAPANINCA ────────────────────────────────────────────────
        private void frmAnaSayfa_FormClosing(object sender, FormClosingEventArgs e)
        {
            saatTimer.Stop();
            Application.Exit(); // Ana sayfa kapanınca uygulama tamamen kapansın
        }
        // Shown eventi — form ekranda göründükten SONRA çalışır
        // Load'dan farkı: tüm kontroller kesinlikle hazır olur
        private void frmAnaSayfa_Shown(object sender, EventArgs e)
        {
            DashboardYukle();
            SonSatislarYukle();
            DgvAyarla();
            RoleGoreMenuAyarla(); // ← Ekle
        }
        /*private void KontrolleriOlustur()
        {
            // ... pnlMenu, pnlUstBar, pnlIcerik ve renkli kartlarını oluşturduğun kodlar ...

            // 1. Kartlarını bir diziye koy (kendi panel isimlerini yaz)
            Control[] dashboardKartlari = { pnlKartUrun, pnlKartMusteri, pnlKartSatis, pnlKartPersonel};

            // 2. Yeniden boyutlandırma işlemini bir Action olarak tanımla
            Action yenidenHizala = () =>
            {
                // pnlIcerik genişliğine göre kartları 25px aralıkla diz
                UIHelper.DashboardKartlariHizala(pnlIcerik, dashboardKartlari, 25);
            };

            // 3. İlk açılışta kartları yerleştir
            yenidenHizala();

            // 4. Form maximize olduğunda veya mouse ile kenarlarından çekildiğinde tekrar tetikle
            pnlIcerik.Resize += (s, e) => yenidenHizala();
        }*/

        // ── ROLE GÖRE MENÜ AYARLA ─────────────────────────────────────────
        private void RoleGoreMenuAyarla()
        {
            string rol = OturumBilgisi.AktifPersonel.RolAdi;

            switch (rol)
            {
                case "Yönetici":
                    // Tüm menüler açık — hiçbir şey gizlenmiyor
                    break;

                case "Kasiyer":
                    // Sadece Satış, Fatura, Müşteri görebilir
                    btnUrun.Visible = false;
                    btnKategori.Visible = false;
                    btnPersonel.Visible = false;
                    btnStok.Visible = false;
                    btnLog.Visible = false;
                    break;

                case "Depo":
                    // Sadece Ürün, Stok görebilir
                    btnMusteri.Visible = false;
                    btnPersonel.Visible = false;
                    btnSatis.Visible = false;
                    btnFatura.Visible = false;
                    btnLog.Visible = false;
                    break;

                default:
                    // Bilinmeyen rol — sadece ana sayfa
                    btnUrun.Visible = false;
                    btnKategori.Visible = false;
                    btnMusteri.Visible = false;
                    btnPersonel.Visible = false;
                    btnSatis.Visible = false;
                    btnFatura.Visible = false;
                    btnStok.Visible = false;
                    btnLog.Visible = false;
                    break;
            }

            // Üst barda rol bilgisini göster
            lblHosgeldin.Text = $"Hoş geldiniz, " +
                $"{OturumBilgisi.AktifPersonel.PerAd} " +
                $"{OturumBilgisi.AktifPersonel.PerSoyad} " +
                $"— {OturumBilgisi.AktifPersonel.RolAdi}";
        }
        // ── KRİTİK STOK UYARI TABLOSU ─────────────────────────────────────
        private void KritikStoklariYukle()
        {
            try
            {
                // vw_StokDurumu view'ından kritik ve düşük stokları getir
                DataTable dt = urunBL.KritikStokListele();

                if (dt.Rows.Count > 0)
                {
                    // Ana sayfada kritik stok uyarısı göster
                    string uyari = "⚠️ Kritik Stok Uyarısı:\n\n";
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["StokDurumu"].ToString() == "Kritik" ||
                            row["StokDurumu"].ToString() == "Tükendi")
                        {
                            uyari += $"• {row["UrunAdi"]} — " +
                                     $"Stok: {row["Stok"]} " +
                                     $"({row["StokDurumu"]})\n";
                        }
                    }
                }
            }
            catch { }
        }
    }
}