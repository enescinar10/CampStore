using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// frmSatis.cs — Satış Yönetimi Formu
// Satış oluşturma, güncelleme, silme ve listeleme işlemleri burada yapılır.
// Satış detayları alt grid'de gösterilir.
using CampStore.BusinessLayer;
using CampStore.Entities;

namespace CampStore.UI
{
    public partial class frmSatis : Form
    {
        private SatisBL satisBL = new SatisBL();
        private MusteriBL musteriBL = new MusteriBL();
        private UrunBL urunBL = new UrunBL();

        private int seciliSatisID = 0;
        private int seciliDetayID = 0;

        // ── KONTROLLER ────────────────────────────────────────────────────
        private Panel pnlBaslik, pnlForm, pnlDetay, pnlGrid;
        private Label lblBaslik;
        private Label lblMusteri, lblDurum, lblToplamTutar;
        private Label lblDetayBaslik, lblUrun, lblMiktar, lblBirimFiyat;
        private ComboBox cmbMusteri, cmbDurum, cmbUrun;
        private TextBox txtToplamTutar, txtMiktar, txtBirimFiyat;
        private Button btnSatisEkle, btnSatisGuncelle, btnSatisSil, btnTemizle;
        private Button btnDetayEkle, btnDetaySil;
        private DataGridView dgvSatis, dgvDetay;

        public frmSatis()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            this.Text = "Satış Yönetimi";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(27, 42, 59);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ── BAŞLIK ────────────────────────────────────────────────────
            pnlBaslik = new Panel
            {
                Size = new Size(1100, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(22, 34, 51)
            };
            this.Controls.Add(pnlBaslik);

            new Label
            {
                Text = "🛒  Satış Yönetimi",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true,
                Parent = pnlBaslik
            };

            // ── SATIŞ FORM PANELİ ─────────────────────────────────────────
            pnlForm = new Panel
            {
                Size = new Size(1100, 110),
                Location = new Point(0, 60),
                BackColor = Color.FromArgb(31, 52, 72)
            };
            this.Controls.Add(pnlForm);
            // ── TARİH FİLTRE SATIRI ───────────────────────────────────────────
            Panel pnlFiltre = new Panel
            {
                Size = new Size(1100, 45),
                Location = new Point(0, 115),
                BackColor = Color.FromArgb(22, 34, 51)
            };
            this.Controls.Add(pnlFiltre);

            // pnlGrid'in konumunu aşağı kaydır
            // pnlGrid Location'ını 170'den 215'e güncelle
            // (KontrolleriOlustur'da pnlGrid oluşturulurken Location'ı 215 yap)

            new Label
            {
                Text = "Başlangıç:",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 13),
                AutoSize = true,
                Parent = pnlFiltre
            };

            DateTimePicker dtpBaslangic = new DateTimePicker
            {
                Size = new Size(140, 28),
                Location = new Point(100, 10),
                Font = new Font("Segoe UI", 9f),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddMonths(-1),
                Name = "dtpBaslangic"
            };
            pnlFiltre.Controls.Add(dtpBaslangic);

            new Label
            {
                Text = "Bitiş:",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(255, 13),
                AutoSize = true,
                Parent = pnlFiltre
            };

            DateTimePicker dtpBitis = new DateTimePicker
            {
                Size = new Size(140, 28),
                Location = new Point(300, 10),
                Font = new Font("Segoe UI", 9f),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now,
                Name = "dtpBitis"
            };
            pnlFiltre.Controls.Add(dtpBitis);

            Button btnRaporFiltrele = new Button
            {
                Text = "🔍 Filtrele",
                Size = new Size(100, 28),
                Location = new Point(455, 9),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRaporFiltrele.FlatAppearance.BorderSize = 0;
            btnRaporFiltrele.Click += (s, ev) =>
            {
                try
                {
                    // Tüm satışları getir, tarih aralığına göre filtrele
                    DataTable dt = satisBL.SatisListele();
                    DataView view = dt.DefaultView;

                    view.RowFilter = $"SatisTarihi >= #{dtpBaslangic.Value:MM/dd/yyyy}# " +
                                     $"AND SatisTarihi <= #{dtpBitis.Value:MM/dd/yyyy}#";

                    dgvSatis.DataSource = view.ToTable();

                    // Toplam tutarı hesapla ve göster
                    decimal toplam = 0;
                    foreach (DataRow row in view.ToTable().Rows)
                        toplam += Convert.ToDecimal(row["ToplamTutar"]);

                    MessageBox.Show(
                        $"Filtrelenen Satış Sayısı: {view.Count}\n" +
                        $"Toplam Ciro: {toplam:F2} ₺",
                        "Rapor Özeti",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Filtreleme hatası: " + ex.Message,
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            pnlFiltre.Controls.Add(btnRaporFiltrele);

            Button btnRaporTemizle = new Button
            {
                Text = "🔄 Tümü",
                Size = new Size(90, 28),
                Location = new Point(565, 9),
                BackColor = Color.FromArgb(127, 140, 141),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRaporTemizle.FlatAppearance.BorderSize = 0;
            btnRaporTemizle.Click += (s, ev) => SatisleriYukle();
            pnlFiltre.Controls.Add(btnRaporTemizle);

            // Toplam ciro göstergesi
            new Label
            {
                Text = "Toplam Ciro:",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(680, 13),
                AutoSize = true,
                Parent = pnlFiltre
            };

            Label lblToplamCiro = new Label
            {
                Text = "0,00 ₺",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Location = new Point(775, 13),
                AutoSize = true,
                Name = "lblToplamCiro"
            };
            pnlFiltre.Controls.Add(lblToplamCiro);

            // Yardımcı metot
            Label Lbl(string text, int x, int y, Panel parent = null)
            {
                var l = new Label
                {
                    Text = text,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    ForeColor = Color.White,
                    Location = new Point(x, y),
                    AutoSize = true
                };
                (parent ?? pnlForm).Controls.Add(l);
                return l;
            }

            ComboBox Cmb(int x, int y, int w = 200, Panel parent = null)
            {
                var c = new ComboBox
                {
                    Size = new Size(w, 30),
                    Location = new Point(x, y),
                    Font = new Font("Segoe UI", 10f),
                    BackColor = Color.FromArgb(44, 62, 80),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                (parent ?? pnlForm).Controls.Add(c);
                return c;
            }

            TextBox Txt(int x, int y, int w = 150, Panel parent = null)
            {
                var t = new TextBox
                {
                    Size = new Size(w, 30),
                    Location = new Point(x, y),
                    Font = new Font("Segoe UI", 10f),
                    BackColor = Color.FromArgb(44, 62, 80),
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                (parent ?? pnlForm).Controls.Add(t);
                return t;
            }

            // Satış form alanları
            lblMusteri = Lbl("Müşteri", 20, 15);
            cmbMusteri = Cmb(20, 38, 250);

            lblDurum = Lbl("Durum", 290, 15);
            cmbDurum = Cmb(290, 38, 160);
            cmbDurum.Items.AddRange(new object[]
            {
                "Hazırlanıyor", "Kargoda", "Teslim Edildi", "İptal"
            });
            cmbDurum.SelectedIndex = 0;

            lblToplamTutar = Lbl("Toplam Tutar (₺)", 470, 15);
            txtToplamTutar = Txt(470, 38, 150);
            //txtToplamTutar.ReadOnly = true; // Detaylardan otomatik hesaplanır
            //txtToplamTutar.BackColor = Color.FromArgb(30, 50, 70);
            txtToplamTutar.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) &&
                    ev.KeyChar != ',' &&
                    ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };

            // Satış butonları
            btnSatisEkle = new Button
            {
                Text = "➕ Yeni Satış",
                Size = new Size(120, 38),
                Location = new Point(640, 38),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSatisEkle.FlatAppearance.BorderSize = 0;
            btnSatisEkle.Click += btnSatisEkle_Click;
            pnlForm.Controls.Add(btnSatisEkle);

            btnSatisGuncelle = new Button
            {
                Text = "✏️ Güncelle",
                Size = new Size(110, 38),
                Location = new Point(770, 38),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnSatisGuncelle.FlatAppearance.BorderSize = 0;
            btnSatisGuncelle.Click += btnSatisGuncelle_Click;
            pnlForm.Controls.Add(btnSatisGuncelle);

            btnSatisSil = new Button
            {
                Text = "🗑️ Sil",
                Size = new Size(90, 38),
                Location = new Point(890, 38),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnSatisSil.FlatAppearance.BorderSize = 0;
            btnSatisSil.Click += btnSatisSil_Click;
            pnlForm.Controls.Add(btnSatisSil);

            btnTemizle = new Button
            {
                Text = "🔄 Temizle",
                Size = new Size(90, 38),
                Location = new Point(990, 38),
                BackColor = Color.FromArgb(127, 140, 141),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnTemizle.FlatAppearance.BorderSize = 0;
            btnTemizle.Click += btnTemizle_Click;
            pnlForm.Controls.Add(btnTemizle);

            // ── SATIŞ GRID ────────────────────────────────────────────────
            pnlGrid = new Panel
            {
                Size = new Size(1100, 250),
                Location = new Point(0, 215),
                BackColor = Color.FromArgb(27, 42, 59)
            };
            this.Controls.Add(pnlGrid);


            new Label
            {
                Text = "Satış Listesi",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 5),
                AutoSize = true,
                Parent = pnlGrid
            };

            dgvSatis = new DataGridView
            {
                Size = new Size(1060, 210),
                Location = new Point(20, 28),
                BackgroundColor = Color.FromArgb(27, 42, 59),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dgvSatis.CellClick += dgvSatis_CellClick;
            pnlGrid.Controls.Add(dgvSatis);

            // ── DETAY PANELİ ──────────────────────────────────────────────
            pnlDetay = new Panel
            {
                Size = new Size(1100, 290),
                Location = new Point(0, 465),
                BackColor = Color.FromArgb(31, 52, 72)
            };
            this.Controls.Add(pnlDetay);

            lblDetayBaslik = Lbl("📋  Satış Detayları", 20, 10, pnlDetay);
            lblDetayBaslik.Font = new Font("Segoe UI", 10f, FontStyle.Bold);

            // Detay form alanları
            lblUrun = Lbl("Ürün", 20, 40, pnlDetay);
            cmbUrun = Cmb(20, 63, 280, pnlDetay);

            lblMiktar = Lbl("Miktar", 320, 40, pnlDetay);
            txtMiktar = Txt(320, 63, 100, pnlDetay);
            txtMiktar.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) && ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };

            lblBirimFiyat = Lbl("Birim Fiyat (₺)", 440, 40, pnlDetay);
            txtBirimFiyat = Txt(440, 63, 130, pnlDetay);
            txtBirimFiyat.ReadOnly = true; // Ürün seçilince otomatik dolar
            txtBirimFiyat.BackColor = Color.FromArgb(30, 50, 70);

            // Ürün seçilince birim fiyat otomatik gelsin
            cmbUrun.SelectedIndexChanged += (s, ev) =>
            {
                if (cmbUrun.SelectedItem is ComboItem item && item.ID > 0)
                {
                    Urun u = urunBL.UrunGetirById(item.ID);
                    if (u != null)
                        txtBirimFiyat.Text = u.Fiyat.ToString("F2");
                }
                else
                {
                    txtBirimFiyat.Clear();
                }
            };

            btnDetayEkle = new Button
            {
                Text = "➕ Detay Ekle",
                Size = new Size(120, 35),
                Location = new Point(590, 63),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false // Satış seçilmeden aktif olmaz
            };
            btnDetayEkle.FlatAppearance.BorderSize = 0;
            btnDetayEkle.Click += btnDetayEkle_Click;
            pnlDetay.Controls.Add(btnDetayEkle);

            btnDetaySil = new Button
            {
                Text = "🗑️ Detay Sil",
                Size = new Size(110, 35),
                Location = new Point(720, 63),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnDetaySil.FlatAppearance.BorderSize = 0;
            btnDetaySil.Click += btnDetaySil_Click;
            pnlDetay.Controls.Add(btnDetaySil);

            // Detay grid
            dgvDetay = new DataGridView
            {
                Size = new Size(1060, 170),
                Location = new Point(20, 108),
                BackgroundColor = Color.FromArgb(31, 52, 72),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dgvDetay.CellClick += dgvDetay_CellClick;
            pnlDetay.Controls.Add(dgvDetay);

            this.Load += frmSatis_Load;
        }

        // ── FORM YÜKLENINCE ───────────────────────────────────────────────
        private void frmSatis_Load(object sender, EventArgs e)
        {
            DgvAyarla();
            MusteriComboYukle();
            UrunComboYukle();
            SatisleriYukle();
        }

        // ── MÜŞTERİ COMBO ────────────────────────────────────────────────
        private void MusteriComboYukle()
        {
            try
            {
                DataTable dt = musteriBL.MusteriListele();
                cmbMusteri.Items.Clear();
                cmbMusteri.Items.Add(new ComboItem { ID = 0, Ad = "— Müşteri Seçin —" });

                foreach (DataRow row in dt.Rows)
                    cmbMusteri.Items.Add(new ComboItem
                    {
                        ID = Convert.ToInt32(row["MusteriID"]),
                        Ad = row["Ad"].ToString() + " " + row["Soyad"].ToString()
                    });

                cmbMusteri.DisplayMember = "Ad";
                cmbMusteri.ValueMember = "ID";
                cmbMusteri.SelectedIndex = 0;
            }
            catch { }
        }

        // ── ÜRÜN COMBO ───────────────────────────────────────────────────
        private void UrunComboYukle()
        {
            try
            {
                DataTable dt = urunBL.UrunListele();
                cmbUrun.Items.Clear();
                cmbUrun.Items.Add(new ComboItem { ID = 0, Ad = "— Ürün Seçin —" });

                foreach (DataRow row in dt.Rows)
                    cmbUrun.Items.Add(new ComboItem
                    {
                        ID = Convert.ToInt32(row["UrunID"]),
                        Ad = row["UrunAdi"].ToString()
                    });

                cmbUrun.DisplayMember = "Ad";
                cmbUrun.ValueMember = "ID";
                cmbUrun.SelectedIndex = 0;
            }
            catch { }
        }

        // ── SATIŞLARI YÜKLE ───────────────────────────────────────────────
        private void SatisleriYukle()
        {
            try
            {
                DataTable dt = satisBL.SatisListele();
                dgvSatis.DataSource = dt;

                // Toplam ciroyu hesapla
                decimal toplam = 0;
                foreach (DataRow row in dt.Rows)
                    toplam += Convert.ToDecimal(row["ToplamTutar"]);

                // Toplam ciro labelını güncelle
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Panel p)
                    {
                        foreach (Control inner in p.Controls)
                        {
                            if (inner.Name == "lblToplamCiro")
                                inner.Text = $"{toplam:F2} ₺";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Satışlar yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── DETAYLARI YÜKLE ───────────────────────────────────────────────
        private void DetaylariYukle(int satisID)
        {
            try
            {
                dgvDetay.DataSource = satisBL.SatisDetayListeleBySatis(satisID);
                ToplamHesapla();
            }
            catch { }
        }

        // ── TOPLAM TUTARI HESAPLA ─────────────────────────────────────────
        private void ToplamHesapla()
        {
            decimal toplam = 0;
            if (dgvDetay.DataSource is DataTable dt)
            {
                foreach (DataRow row in dt.Rows)
                {
                    decimal birimFiyat = Convert.ToDecimal(row["BirimFiyat"]);
                    int miktar = Convert.ToInt32(row["Miktar"]);
                    toplam += birimFiyat * miktar;
                }
            }
            txtToplamTutar.Text = toplam.ToString("F2");
        }
        private void dgvSatis_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow satir = dgvSatis.Rows[e.RowIndex];
                seciliSatisID = Convert.ToInt32(satir.Cells["SatisID"].Value);

                // MusteriID ile combobox'ı seç
                int musteriID = Convert.ToInt32(satir.Cells["MusteriID"].Value);
                for (int i = 0; i < cmbMusteri.Items.Count; i++)
                {
                    if (((ComboItem)cmbMusteri.Items[i]).ID == musteriID)
                    {
                        cmbMusteri.SelectedIndex = i;
                        break;
                    }
                }

                // Durum seç
                string durum = satir.Cells["Durum"].Value?.ToString() ?? "Hazırlanıyor";
                for (int i = 0; i < cmbDurum.Items.Count; i++)
                {
                    if (cmbDurum.Items[i].ToString() == durum)
                    {
                        cmbDurum.SelectedIndex = i;
                        break;
                    }
                }

                // Toplam tutar
                txtToplamTutar.Text = satir.Cells["ToplamTutar"].Value?.ToString() ?? "0";

                // Detayları yükle
                DetaylariYukle(seciliSatisID);

                btnSatisGuncelle.Enabled = true;
                btnSatisSil.Enabled = true;
                btnDetayEkle.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Satır okunurken hata: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── SATIŞ GRID TIKLAMA ────────────────────────────────────────────
        //private void dgvSatis_CellClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex < 0) return;

        //    DataGridViewRow satir = dgvSatis.Rows[e.RowIndex];
        //    seciliSatisID = Convert.ToInt32(satir.Cells["SatisID"].Value);

        //    // Müşteri seç
        //    int musteriID = Convert.ToInt32(satir.Cells["MusteriID"].Value);
        //    for (int i = 0; i < cmbMusteri.Items.Count; i++)
        //    {
        //        if (((ComboItem)cmbMusteri.Items[i]).ID == musteriID)
        //        {
        //            cmbMusteri.SelectedIndex = i;
        //            break;
        //        }
        //    }

        //    // Durum seç
        //    string durum = satir.Cells["Durum"].Value.ToString();
        //    cmbDurum.SelectedItem = durum;

        //    // Detayları yükle
        //    DetaylariYukle(seciliSatisID);

        //    btnSatisGuncelle.Enabled = true;
        //    btnSatisSil.Enabled = true;
        //    btnDetayEkle.Enabled = true;
        //}

        // ── DETAY GRID TIKLAMA ────────────────────────────────────────────
        private void dgvDetay_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow satir = dgvDetay.Rows[e.RowIndex];
            seciliDetayID = Convert.ToInt32(satir.Cells["DetayID"].Value);

            // Ürün seç
            int urunID = Convert.ToInt32(satir.Cells["UrunID"].Value);
            for (int i = 0; i < cmbUrun.Items.Count; i++)
            {
                if (((ComboItem)cmbUrun.Items[i]).ID == urunID)
                {
                    cmbUrun.SelectedIndex = i;
                    break;
                }
            }

            txtMiktar.Text = satir.Cells["Miktar"].Value.ToString();
            txtBirimFiyat.Text = satir.Cells["BirimFiyat"].Value.ToString();
            btnDetaySil.Enabled = true;
        }

        // ── YENİ SATIŞ EKLE ──────────────────────────────────────────────
        private void btnSatisEkle_Click(object sender, EventArgs e)
        {
            Satis s = new Satis
            {
                MusteriID = cmbMusteri.SelectedItem is ComboItem cm ? cm.ID : 0,
                PerID = OturumBilgisi.AktifPersonel.PerID, // Oturumdaki personel
                ToplamTutar = 0, // Detay eklenince güncellenecek
                Durum = cmbDurum.SelectedItem?.ToString() ?? "Hazırlanıyor"
            };

            string sonuc = satisBL.SatisEkle(s, out int yeniID);

            if (sonuc == "OK")
            {
                seciliSatisID = yeniID;
                MessageBox.Show($"Satış oluşturuldu! ID: {yeniID}\nŞimdi ürün ekleyebilirsiniz.",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SatisleriYukle();
                btnDetayEkle.Enabled = true;
                btnSatisGuncelle.Enabled = true;
                btnSatisSil.Enabled = true;
            }
            else
            {
                MessageBox.Show(sonuc, "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ── SATIŞ GÜNCELLE ───────────────────────────────────────────────
        private void btnSatisGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliSatisID == 0) return;

            Satis s = new Satis
            {
                SatisID = seciliSatisID,
                MusteriID = cmbMusteri.SelectedItem is ComboItem cm ? cm.ID : 0,
                PerID = OturumBilgisi.AktifPersonel.PerID,
                ToplamTutar = decimal.TryParse(txtToplamTutar.Text, out decimal t) ? t : 0,
                Durum = cmbDurum.SelectedItem?.ToString() ?? "Hazırlanıyor"
            };

            string sonuc = satisBL.SatisGuncelle(s);

            if (sonuc == "OK")
            {
                MessageBox.Show("Satış güncellendi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SatisleriYukle();
            }
            else
            {
                MessageBox.Show(sonuc, "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ── SATIŞ SİL ────────────────────────────────────────────────────
        private void btnSatisSil_Click(object sender, EventArgs e)
        {
            if (seciliSatisID == 0) return;

            if (MessageBox.Show("Bu satışı ve tüm detaylarını silmek istiyor musunuz?",
                "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                string sonuc = satisBL.SatisSil(seciliSatisID);

                if (sonuc == "OK")
                {
                    MessageBox.Show("Satış silindi!",
                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Temizle();
                    SatisleriYukle();
                }
                else
                {
                    MessageBox.Show(sonuc, "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // ── DETAY EKLE ───────────────────────────────────────────────────
        private void btnDetayEkle_Click(object sender, EventArgs e)
        {
            if (seciliSatisID == 0)
            {
                MessageBox.Show("Lütfen önce bir satış seçin veya oluşturun!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtMiktar.Text, out int miktar) || miktar <= 0)
            {
                MessageBox.Show("Geçerli bir miktar giriniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtBirimFiyat.Text, out decimal birimFiyat))
            {
                MessageBox.Show("Geçerli bir birim fiyat giriniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SatisDetay sd = new SatisDetay
            {
                SatisID = seciliSatisID,
                UrunID = cmbUrun.SelectedItem is ComboItem cu ? cu.ID : 0,
                Miktar = miktar,
                BirimFiyat = birimFiyat
            };

            string sonuc = satisBL.SatisDetayEkle(sd);

            if (sonuc == "OK")
            {
                // Toplam tutarı güncelle
                decimal yeniToplam = (decimal.TryParse(txtToplamTutar.Text,
                    out decimal mevcutToplam) ? mevcutToplam : 0)
                    + (miktar * birimFiyat);

                // Satışın toplam tutarını da güncelle
                Satis s = new Satis
                {
                    SatisID = seciliSatisID,
                    MusteriID = cmbMusteri.SelectedItem is ComboItem cm ? cm.ID : 0,
                    PerID = OturumBilgisi.AktifPersonel.PerID,
                    ToplamTutar = yeniToplam,
                    Durum = cmbDurum.SelectedItem?.ToString() ?? "Hazırlanıyor"
                };
                satisBL.SatisGuncelle(s);

                DetaylariYukle(seciliSatisID);
                SatisleriYukle();

                // Detay alanlarını temizle
                cmbUrun.SelectedIndex = 0;
                txtMiktar.Clear();
                txtBirimFiyat.Clear();
            }
            else
            {
                MessageBox.Show(sonuc, "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ── DETAY SİL ────────────────────────────────────────────────────
        private void btnDetaySil_Click(object sender, EventArgs e)
        {
            if (seciliDetayID == 0) return;

            if (MessageBox.Show("Bu detay satırını silmek istiyor musunuz?",
                "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                string sonuc = satisBL.SatisDetaySil(seciliDetayID);

                if (sonuc == "OK")
                {
                    seciliDetayID = 0;
                    DetaylariYukle(seciliSatisID);
                    SatisleriYukle();
                    btnDetaySil.Enabled = false;
                }
                else
                {
                    MessageBox.Show(sonuc, "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // ── TEMİZLE ──────────────────────────────────────────────────────
        private void btnTemizle_Click(object sender, EventArgs e) => Temizle();

        private void Temizle()
        {
            cmbMusteri.SelectedIndex = 0;
            cmbDurum.SelectedIndex = 0;
            txtToplamTutar.Clear();
            cmbUrun.SelectedIndex = 0;
            txtMiktar.Clear();
            txtBirimFiyat.Clear();
            dgvDetay.DataSource = null;
            seciliSatisID = 0;
            seciliDetayID = 0;
            btnSatisGuncelle.Enabled = false;
            btnSatisSil.Enabled = false;
            btnDetayEkle.Enabled = false;
            btnDetaySil.Enabled = false;
        }

        // ── DGV AYARLA ───────────────────────────────────────────────────
        private void DgvAyarla()
        {
            foreach (DataGridView dgv in new[] { dgvSatis, dgvDetay })
            {
                dgv.EnableHeadersVisualStyles = false;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 52, 72);
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                dgv.DefaultCellStyle.BackColor = Color.FromArgb(27, 42, 59);
                dgv.DefaultCellStyle.ForeColor = Color.White;
                dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 204, 113);
                dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 34, 51);
                dgv.RowTemplate.Height = 30;
            }
        }
    }
}
