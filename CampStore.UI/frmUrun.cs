using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// frmUrun.cs — Ürün Yönetimi Formu
// Ürün ekleme, güncelleme, silme ve listeleme işlemleri burada yapılır.
// Kategori combobox'ı KategoriBL üzerinden doldurulur.
using CampStore.BusinessLayer;
using CampStore.Entities;

namespace CampStore.UI
{
    public partial class frmUrun : Form
    {
        private UrunBL urunBL = new UrunBL();
        private KategoriBL kategoriBL = new KategoriBL();

        // Seçili ürünün ID'sini tutar — güncelleme için gerekli
        private int seciliUrunID = 0;

        // ── KONTROLLER ────────────────────────────────────────────────────
        private Panel pnlBaslik, pnlForm, pnlGrid;
        private Label lblBaslik, lblUrunAdi, lblKategori;
        private Label lblFiyat, lblStok, lblDurum, lblAciklama;
        private TextBox txtUrunAdi, txtFiyat, txtStok, txtAciklama;
        private ComboBox cmbKategori;
        private CheckBox chkDurum;
        private Button btnEkle, btnGuncelle, btnSil, btnTemizle;
        private DataGridView dgvUrun;

        public frmUrun()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        // ── KONTROLLERİ OLUŞTUR ───────────────────────────────────────────
        private void KontrolleriOlustur()
        {
            this.Text = "Ürün Yönetimi";
            this.Size = new Size(1000, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(27, 42, 59);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ── BAŞLIK PANELİ ─────────────────────────────────────────────
            pnlBaslik = new Panel
            {
                Size = new Size(1000, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(22, 34, 51)
            };
            this.Controls.Add(pnlBaslik);

            lblBaslik = new Label
            {
                Text = "📦  Ürün Yönetimi",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };
            pnlBaslik.Controls.Add(lblBaslik);

            // ── FORM PANELİ ───────────────────────────────────────────────
            pnlForm = new Panel
            {
                Size = new Size(1000, 160),
                Location = new Point(0, 60),
                BackColor = Color.FromArgb(31, 52, 72)
            };
            this.Controls.Add(pnlForm);

            // Ürün Adı
            lblUrunAdi = new Label
            {
                Text = "Ürün Adı",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };
            pnlForm.Controls.Add(lblUrunAdi);

            txtUrunAdi = new TextBox
            {
                Size = new Size(200, 30),
                Location = new Point(20, 38),
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlForm.Controls.Add(txtUrunAdi);

            // Kategori
            lblKategori = new Label
            {
                Text = "Kategori",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(240, 15),
                AutoSize = true
            };
            pnlForm.Controls.Add(lblKategori);

            cmbKategori = new ComboBox
            {
                Size = new Size(180, 30),
                Location = new Point(240, 38),
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            pnlForm.Controls.Add(cmbKategori);

            // Fiyat
            lblFiyat = new Label
            {
                Text = "Fiyat (₺)",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(440, 15),
                AutoSize = true
            };
            pnlForm.Controls.Add(lblFiyat);

            txtFiyat = new TextBox
            {
                Size = new Size(120, 30),
                Location = new Point(440, 38),
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            // Sadece rakam ve nokta girilsin
            txtFiyat.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) &&
                    ev.KeyChar != ',' &&
                    ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };
            pnlForm.Controls.Add(txtFiyat);

            // Stok
            lblStok = new Label
            {
                Text = "Stok",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(580, 15),
                AutoSize = true
            };
            pnlForm.Controls.Add(lblStok);

            txtStok = new TextBox
            {
                Size = new Size(100, 30),
                Location = new Point(580, 38),
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            // Sadece rakam girilsin
            txtStok.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) && ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };
            pnlForm.Controls.Add(txtStok);

            // Durum
            lblDurum = new Label
            {
                Text = "Durum",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(700, 15),
                AutoSize = true
            };
            pnlForm.Controls.Add(lblDurum);

            chkDurum = new CheckBox
            {
                Text = "Aktif",
                Location = new Point(700, 40),
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.White,
                Checked = true // Varsayılan aktif
            };
            pnlForm.Controls.Add(chkDurum);

            // Açıklama
            lblAciklama = new Label
            {
                Text = "Açıklama",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 82),
                AutoSize = true
            };
            pnlForm.Controls.Add(lblAciklama);

            txtAciklama = new TextBox
            {
                Size = new Size(500, 30),
                Location = new Point(20, 105),
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlForm.Controls.Add(txtAciklama);

            // Butonlar
            btnEkle = new Button
            {
                Text = "➕ Ekle",
                Size = new Size(110, 38),
                Location = new Point(580, 105),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEkle.FlatAppearance.BorderSize = 0;
            btnEkle.Click += btnEkle_Click;
            pnlForm.Controls.Add(btnEkle);

            btnGuncelle = new Button
            {
                Text = "✏️ Güncelle",
                Size = new Size(110, 38),
                Location = new Point(700, 105),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnGuncelle.FlatAppearance.BorderSize = 0;
            btnGuncelle.Click += btnGuncelle_Click;
            pnlForm.Controls.Add(btnGuncelle);

            btnSil = new Button
            {
                Text = "🗑️ Sil",
                Size = new Size(90, 38),
                Location = new Point(820, 105),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnSil.FlatAppearance.BorderSize = 0;
            btnSil.Click += btnSil_Click;
            pnlForm.Controls.Add(btnSil);

            btnTemizle = new Button
            {
                Text = "🔄 Temizle",
                Size = new Size(110, 28),
                Location = new Point(580, 68),
                BackColor = Color.FromArgb(127, 140, 141),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnTemizle.FlatAppearance.BorderSize = 0;
            btnTemizle.Click += btnTemizle_Click;
            pnlForm.Controls.Add(btnTemizle);

            // ── GRID PANELİ ───────────────────────────────────────────────
            pnlGrid = new Panel
            {
                Size = new Size(1000, 400),
                Location = new Point(0, 220),
                BackColor = Color.FromArgb(27, 42, 59)
            };
            this.Controls.Add(pnlGrid);

            dgvUrun = new DataGridView
            {
                Size = new Size(960, 370),
                Location = new Point(20, 15),
                BackgroundColor = Color.FromArgb(27, 42, 59),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dgvUrun.CellClick += dgvUrun_CellClick;
            pnlGrid.Controls.Add(dgvUrun);

            this.Load += frmUrun_Load;

            // ── STOK ALARM AÇIKLAMASI ─────────────────────────────────────────
            Panel pnlAlarmAciklama = new Panel
            {
                Size = new Size(960, 30),
                Location = new Point(20, 388),
                BackColor = Color.FromArgb(22, 34, 51)
            };
            pnlGrid.Controls.Add(pnlAlarmAciklama);

            // Kırmızı gösterge
            Panel pnlKirmizi = new Panel
            {
                Size = new Size(14, 14),
                Location = new Point(10, 8),
                BackColor = Color.FromArgb(231, 76, 60)
            };
            pnlAlarmAciklama.Controls.Add(pnlKirmizi);

            new Label
            {
                Text = "Kritik Stok (≤5)",
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.White,
                Location = new Point(30, 7),
                AutoSize = true,
                Parent = pnlAlarmAciklama
            };

            // Sarı gösterge
            Panel pnlSari = new Panel
            {
                Size = new Size(14, 14),
                Location = new Point(160, 8),
                BackColor = Color.FromArgb(241, 196, 15)
            };
            pnlAlarmAciklama.Controls.Add(pnlSari);

            new Label
            {
                Text = "Düşük Stok (≤10)",
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.White,
                Location = new Point(180, 7),
                AutoSize = true,
                Parent = pnlAlarmAciklama
            };
        }


        // ── FORM YÜKLENINCE ───────────────────────────────────────────────
        private void frmUrun_Load(object sender, EventArgs e)
        {
            DgvAyarla();
            KategoriComboYukle();
            UrunleriYukle();
        }

        // ── ÜRÜNLERİ GRID'E YÜKLE ────────────────────────────────────────
        private void UrunleriYukle()
        {
            try
            {
                dgvUrun.DataSource = urunBL.UrunListele();
                StokAlarmKontrol(); // Yükledikten sonra alarmı kontrol et
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürünler yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── KATEGORİ COMBOBOX'INI YÜKLE ──────────────────────────────────
        private void KategoriComboYukle()
        {
            try
            {
                DataTable dt = kategoriBL.KategoriListele();
                cmbKategori.Items.Clear();
                cmbKategori.Items.Add(new ComboItem { ID = 0, Ad = "— Kategori Seçin —" });

                foreach (DataRow row in dt.Rows)
                {
                    cmbKategori.Items.Add(new ComboItem
                    {
                        ID = Convert.ToInt32(row["KatID"]),
                        Ad = row["KatAdi"].ToString()
                    });
                }

                cmbKategori.DisplayMember = "Ad";
                cmbKategori.ValueMember = "ID";
                cmbKategori.SelectedIndex = 0;
            }
            catch { }
        }
        private void dgvUrun_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow satir = dgvUrun.Rows[e.RowIndex];

                seciliUrunID = Convert.ToInt32(satir.Cells["UrunID"].Value);
                txtUrunAdi.Text = satir.Cells["UrunAdi"].Value?.ToString() ?? "";
                txtFiyat.Text = satir.Cells["Fiyat"].Value?.ToString() ?? "";
                txtStok.Text = satir.Cells["Stok"].Value?.ToString() ?? "";
                txtAciklama.Text = satir.Cells["Aciklama"].Value?.ToString() ?? "";
                chkDurum.Checked = Convert.ToBoolean(satir.Cells["Durum"].Value);

                int katID = Convert.ToInt32(satir.Cells["KatID"].Value);
                for (int i = 0; i < cmbKategori.Items.Count; i++)
                {
                    if (((ComboItem)cmbKategori.Items[i]).ID == katID)
                    {
                        cmbKategori.SelectedIndex = i;
                        break;
                    }
                }

                btnGuncelle.Enabled = true;
                btnSil.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Satır okunurken hata: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── GRID SATIRINA TIKLANINCA ──────────────────────────────────────
        //private void dgvUrun_CellClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex < 0) return;

        //    DataGridViewRow satir = dgvUrun.Rows[e.RowIndex];

        //    seciliUrunID = Convert.ToInt32(satir.Cells["UrunID"].Value);
        //    txtUrunAdi.Text = satir.Cells["UrunAdi"].Value.ToString();
        //    txtFiyat.Text = satir.Cells["Fiyat"].Value.ToString();
        //    txtStok.Text = satir.Cells["Stok"].Value.ToString();
        //    txtAciklama.Text = satir.Cells["Aciklama"].Value.ToString();
        //    chkDurum.Checked = Convert.ToBoolean(satir.Cells["Durum"].Value);

        //    // Kategori combobox'ını seçili ürünün kategorisine ayarla
        //    int katID = Convert.ToInt32(satir.Cells["KatID"].Value);
        //    for (int i = 0; i < cmbKategori.Items.Count; i++)
        //    {
        //        if (((ComboItem)cmbKategori.Items[i]).ID == katID)
        //        {
        //            cmbKategori.SelectedIndex = i;
        //            break;
        //        }
        //    }

        //    btnGuncelle.Enabled = true;
        //    btnSil.Enabled = true;
        //}

        // ── EKLE BUTONU ───────────────────────────────────────────────────
        private void btnEkle_Click(object sender, EventArgs e)
        {
            // Fiyat ve stok parse kontrolü
            if (!decimal.TryParse(txtFiyat.Text, out decimal fiyat))
            {
                MessageBox.Show("Geçerli bir fiyat giriniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtStok.Text, out int stok))
            {
                MessageBox.Show("Geçerli bir stok miktarı giriniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Urun u = new Urun
            {
                UrunAdi = txtUrunAdi.Text.Trim(),
                KatID = ((ComboItem)cmbKategori.SelectedItem).ID,
                Fiyat = fiyat,
                Stok = stok,
                Durum = chkDurum.Checked,
                Aciklama = txtAciklama.Text.Trim()
            };

            string sonuc = urunBL.UrunEkle(u);

            if (sonuc == "OK")
            {
                MessageBox.Show("Ürün başarıyla eklendi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Temizle();
                UrunleriYukle();
            }
            else
            {
                MessageBox.Show(sonuc, "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ── GÜNCELLE BUTONU ───────────────────────────────────────────────
        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliUrunID == 0) return;

            if (!decimal.TryParse(txtFiyat.Text, out decimal fiyat))
            {
                MessageBox.Show("Geçerli bir fiyat giriniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtStok.Text, out int stok))
            {
                MessageBox.Show("Geçerli bir stok miktarı giriniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Urun u = new Urun
            {
                UrunID = seciliUrunID,
                UrunAdi = txtUrunAdi.Text.Trim(),
                KatID = ((ComboItem)cmbKategori.SelectedItem).ID,
                Fiyat = fiyat,
                Stok = stok,
                Durum = chkDurum.Checked,
                Aciklama = txtAciklama.Text.Trim()
            };

            string sonuc = urunBL.UrunGuncelle(u);

            if (sonuc == "OK")
            {
                MessageBox.Show("Ürün başarıyla güncellendi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Temizle();
                UrunleriYukle();
            }
            else
            {
                MessageBox.Show(sonuc, "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ── SİL BUTONU ────────────────────────────────────────────────────
        private void btnSil_Click(object sender, EventArgs e)
        {
            if (seciliUrunID == 0) return;

            DialogResult onay = MessageBox.Show(
                "Bu ürünü silmek istediğinize emin misiniz?",
                "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (onay == DialogResult.Yes)
            {
                string sonuc = urunBL.UrunSil(seciliUrunID);

                if (sonuc == "OK")
                {
                    MessageBox.Show("Ürün silindi!",
                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Temizle();
                    UrunleriYukle();
                }
                else
                {
                    MessageBox.Show(sonuc, "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // ── TEMİZLE BUTONU ────────────────────────────────────────────────
        private void btnTemizle_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        // ── FORM ALANLARINI TEMİZLE ───────────────────────────────────────
        private void Temizle()
        {
            txtUrunAdi.Clear();
            txtFiyat.Clear();
            txtStok.Clear();
            txtAciklama.Clear();
            chkDurum.Checked = true;
            cmbKategori.SelectedIndex = 0;
            seciliUrunID = 0;
            btnGuncelle.Enabled = false;
            btnSil.Enabled = false;
            txtUrunAdi.Focus();
        }

        // ── DATAGRIDVIEW GÖRÜNÜM AYARI ────────────────────────────────────
        private void DgvAyarla()
        {
            dgvUrun.EnableHeadersVisualStyles = false;
            dgvUrun.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 52, 72);
            dgvUrun.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUrun.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvUrun.DefaultCellStyle.BackColor = Color.FromArgb(27, 42, 59);
            dgvUrun.DefaultCellStyle.ForeColor = Color.White;
            dgvUrun.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 204, 113);
            dgvUrun.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 34, 51);
            dgvUrun.RowTemplate.Height = 30;
        }
        // ── STOK ALARM SİSTEMİ ────────────────────────────────────────────
        // Stok 10'un altındaysa sarı, 5'in altındaysa kırmızı gösterir
        private void StokAlarmKontrol()
        {
            foreach (DataGridViewRow satir in dgvUrun.Rows)
            {
                if (satir.Cells["Stok"].Value == null) continue;

                int stok = Convert.ToInt32(satir.Cells["Stok"].Value);

                if (stok <= 5)
                {
                    // Kritik — kırmızı
                    satir.DefaultCellStyle.BackColor = Color.FromArgb(231, 76, 60);
                    satir.DefaultCellStyle.ForeColor = Color.White;
                }
                else if (stok <= 10)
                {
                    // Uyarı — sarı
                    satir.DefaultCellStyle.BackColor = Color.FromArgb(241, 196, 15);
                    satir.DefaultCellStyle.ForeColor = Color.FromArgb(27, 42, 59);
                }
                else
                {
                    // Normal — varsayılan
                    satir.DefaultCellStyle.BackColor = Color.FromArgb(27, 42, 59);
                    satir.DefaultCellStyle.ForeColor = Color.White;
                }
            }
        }
    }
}
