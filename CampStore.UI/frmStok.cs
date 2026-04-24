using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// frmStok.cs — Stok Takip Formu
// Stok girişi, çıkışı ve hareket geçmişi burada yönetilir.
// Ürün seçilince o ürüne ait hareketler filtrelenir.
using CampStore.BusinessLayer;
using CampStore.Entities;

namespace CampStore.UI
{
    public partial class frmStok : Form
    {
        private StokBL stokBL = new StokBL();
        private UrunBL urunBL = new UrunBL();

        // ── KONTROLLER ────────────────────────────────────────────────────
        private Panel pnlBaslik, pnlForm, pnlGrid;
        private Label lblBaslik, lblUrun, lblGiris, lblCikis, lblTarih;
        private Label lblMevcutStok;
        private ComboBox cmbUrun;
        private TextBox txtGiris, txtCikis, txtMevcutStok;
        private DateTimePicker dtpTarih;
        private Button btnEkle, btnTemizle, btnTumunuGoster;
        private DataGridView dgvStok;

        public frmStok()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            this.Text = "Stok Takibi";
            this.Size = new Size(1000, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(27, 42, 59);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ── BAŞLIK ────────────────────────────────────────────────────
            pnlBaslik = new Panel
            {
                Size = new Size(1000, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(22, 34, 51)
            };
            this.Controls.Add(pnlBaslik);

            new Label
            {
                Text = "📊  Stok Takibi",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true,
                Parent = pnlBaslik
            };

            // ── FORM PANELİ ───────────────────────────────────────────────
            pnlForm = new Panel
            {
                Size = new Size(1000, 150),
                Location = new Point(0, 60),
                BackColor = Color.FromArgb(31, 52, 72)
            };
            this.Controls.Add(pnlForm);

            // Yardımcı metotlar
            Label Lbl(string text, int x, int y)
            {
                var l = new Label
                {
                    Text = text,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    ForeColor = Color.White,
                    Location = new Point(x, y),
                    AutoSize = true
                };
                pnlForm.Controls.Add(l);
                return l;
            }

            TextBox Txt(int x, int y, int w = 130)
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
                pnlForm.Controls.Add(t);
                return t;
            }

            // Ürün seçimi
            lblUrun = Lbl("Ürün", 20, 15);
            cmbUrun = new ComboBox
            {
                Size = new Size(260, 30),
                Location = new Point(20, 38),
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            // Ürün seçilince mevcut stoku göster ve hareketleri filtrele
            cmbUrun.SelectedIndexChanged += (s, ev) =>
            {
                if (cmbUrun.SelectedItem is ComboItem item && item.ID > 0)
                {
                    // Ürünün mevcut stok miktarını getir
                    Urun u = urunBL.UrunGetirById(item.ID);
                    if (u != null)
                        txtMevcutStok.Text = u.Stok.ToString();

                    // O ürüne ait stok hareketlerini filtrele
                    StokHareketleriYukle(item.ID);
                }
                else
                {
                    txtMevcutStok.Clear();
                    StokHareketleriYukle(0); // Tümünü göster
                }
            };
            pnlForm.Controls.Add(cmbUrun);

            // Mevcut stok — sadece okunabilir
            lblMevcutStok = Lbl("Mevcut Stok", 300, 15);
            txtMevcutStok = Txt(300, 38, 100);
            txtMevcutStok.ReadOnly = true;
            txtMevcutStok.BackColor = Color.FromArgb(30, 50, 70);
            txtMevcutStok.ForeColor = Color.FromArgb(46, 204, 113);

            // Giriş miktarı
            lblGiris = Lbl("Giriş Miktarı", 420, 15);
            txtGiris = Txt(420, 38);
            txtGiris.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) && ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };
            // Giriş girilince çıkışı sıfırla
            txtGiris.TextChanged += (s, ev) =>
            {
                if (!string.IsNullOrEmpty(txtGiris.Text))
                    txtCikis.Clear();
            };

            // Çıkış miktarı
            lblCikis = Lbl("Çıkış Miktarı", 570, 15);
            txtCikis = Txt(570, 38);
            txtCikis.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) && ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };
            // Çıkış girilince girişi sıfırla
            txtCikis.TextChanged += (s, ev) =>
            {
                if (!string.IsNullOrEmpty(txtCikis.Text))
                    txtGiris.Clear();
            };

            // Tarih
            lblTarih = Lbl("Tarih", 720, 15);
            dtpTarih = new DateTimePicker
            {
                Size = new Size(160, 30),
                Location = new Point(720, 38),
                Font = new Font("Segoe UI", 10f),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };
            pnlForm.Controls.Add(dtpTarih);

            // Butonlar — satır 2
            btnEkle = new Button
            {
                Text = "➕ Hareket Ekle",
                Size = new Size(140, 38),
                Location = new Point(20, 98),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEkle.FlatAppearance.BorderSize = 0;
            btnEkle.Click += btnEkle_Click;
            pnlForm.Controls.Add(btnEkle);

            btnTemizle = new Button
            {
                Text = "🔄 Temizle",
                Size = new Size(110, 38),
                Location = new Point(170, 98),
                BackColor = Color.FromArgb(127, 140, 141),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnTemizle.FlatAppearance.BorderSize = 0;
            btnTemizle.Click += btnTemizle_Click;
            pnlForm.Controls.Add(btnTemizle);

            btnTumunuGoster = new Button
            {
                Text = "📋 Tüm Hareketler",
                Size = new Size(150, 38),
                Location = new Point(290, 98),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnTumunuGoster.FlatAppearance.BorderSize = 0;
            btnTumunuGoster.Click += (s, ev) =>
            {
                cmbUrun.SelectedIndex = 0;
                StokHareketleriYukle(0);
            };
            pnlForm.Controls.Add(btnTumunuGoster);

            // ── GRID PANELİ ───────────────────────────────────────────────
            pnlGrid = new Panel
            {
                Size = new Size(1000, 400),
                Location = new Point(0, 210),
                BackColor = Color.FromArgb(27, 42, 59)
            };
            this.Controls.Add(pnlGrid);

            // Özet bilgi paneli — giriş/çıkış toplamları
            Panel pnlOzet = new Panel
            {
                Size = new Size(960, 40),
                Location = new Point(20, 5),
                BackColor = Color.FromArgb(22, 34, 51)
            };
            pnlGrid.Controls.Add(pnlOzet);

            new Label
            {
                Name = "lblToplamGiris",
                Text = "Toplam Giriş: 0",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Location = new Point(20, 12),
                AutoSize = true,
                Parent = pnlOzet
            };

            new Label
            {
                Name = "lblToplamCikis",
                Text = "Toplam Çıkış: 0",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(200, 12),
                AutoSize = true,
                Parent = pnlOzet
            };

            dgvStok = new DataGridView
            {
                Size = new Size(960, 340),
                Location = new Point(20, 50),
                BackgroundColor = Color.FromArgb(27, 42, 59),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            pnlGrid.Controls.Add(dgvStok);

            this.Load += frmStok_Load;
        }

        // ── FORM YÜKLENINCE ───────────────────────────────────────────────
        private void frmStok_Load(object sender, EventArgs e)
        {
            DgvAyarla();
            UrunComboYukle();
            StokHareketleriYukle(0); // Başlangıçta tümünü göster
        }

        // ── ÜRÜN COMBO ───────────────────────────────────────────────────
        private void UrunComboYukle()
        {
            try
            {
                DataTable dt = urunBL.UrunListele();
                cmbUrun.Items.Clear();
                cmbUrun.Items.Add(new ComboItem { ID = 0, Ad = "— Tüm Ürünler —" });

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

        // ── STOK HAREKETLERİNİ YÜKLE ─────────────────────────────────────
        private void StokHareketleriYukle(int urunID)
        {
            try
            {
                DataTable dt;

                // urunID 0 ise tümünü getir, değilse filtrele
                dt = urunID == 0
                     ? stokBL.StokHareketListele()
                     : stokBL.StokHareketListeleByUrun(urunID);

                dgvStok.DataSource = dt;

                // Toplam giriş ve çıkış hesapla
                int toplamGiris = 0;
                int toplamCikis = 0;

                foreach (DataRow row in dt.Rows)
                {
                    toplamGiris += Convert.ToInt32(row["GirisMiktar"]);
                    toplamCikis += Convert.ToInt32(row["CikisMiktar"]);
                }

                // Özet labellarını güncelle
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Panel pnlGrid)
                    {
                        foreach (Control inner in pnlGrid.Controls)
                        {
                            if (inner is Panel pnlOzet)
                            {
                                foreach (Control lbl in pnlOzet.Controls)
                                {
                                    if (lbl.Name == "lblToplamGiris")
                                        lbl.Text = $"Toplam Giriş: {toplamGiris}";
                                    if (lbl.Name == "lblToplamCikis")
                                        lbl.Text = $"Toplam Çıkış: {toplamCikis}";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Stok hareketleri yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── HAREKET EKLE ─────────────────────────────────────────────────
        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (!(cmbUrun.SelectedItem is ComboItem item) || item.ID == 0)
            {
                MessageBox.Show("Lütfen bir ürün seçiniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int giris = 0, cikis = 0;

            // Giriş veya çıkıştan biri dolu olmalı
            if (!string.IsNullOrEmpty(txtGiris.Text))
            {
                if (!int.TryParse(txtGiris.Text, out giris) || giris <= 0)
                {
                    MessageBox.Show("Geçerli bir giriş miktarı giriniz!",
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else if (!string.IsNullOrEmpty(txtCikis.Text))
            {
                if (!int.TryParse(txtCikis.Text, out cikis) || cikis <= 0)
                {
                    MessageBox.Show("Geçerli bir çıkış miktarı giriniz!",
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Çıkış mevcut stoktan fazla olamaz
                if (int.TryParse(txtMevcutStok.Text, out int mevcutStok) &&
                    cikis > mevcutStok)
                {
                    MessageBox.Show($"Çıkış miktarı mevcut stoktan ({mevcutStok}) fazla olamaz!",
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Lütfen giriş veya çıkış miktarı giriniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StokHareket sh = new StokHareket
            {
                UrunID = item.ID,
                GirisMiktar = giris,
                CikisMiktar = cikis,
                Tarih = dtpTarih.Value
            };

            string sonuc = stokBL.StokHareketEkle(sh);

            if (sonuc == "OK")
            {
                MessageBox.Show("Stok hareketi kaydedildi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Temizle();
                StokHareketleriYukle(0);

                // Ürün stok miktarını yenile
                Urun u = urunBL.UrunGetirById(item.ID);
                if (u != null)
                    txtMevcutStok.Text = u.Stok.ToString();
            }
            else
            {
                MessageBox.Show(sonuc, "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ── TEMİZLE ──────────────────────────────────────────────────────
        private void btnTemizle_Click(object sender, EventArgs e) => Temizle();

        private void Temizle()
        {
            cmbUrun.SelectedIndex = 0;
            txtGiris.Clear();
            txtCikis.Clear();
            txtMevcutStok.Clear();
            dtpTarih.Value = DateTime.Now;
        }

        // ── DGV AYARLA ───────────────────────────────────────────────────
        private void DgvAyarla()
        {
            dgvStok.EnableHeadersVisualStyles = false;
            dgvStok.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 52, 72);
            dgvStok.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvStok.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvStok.DefaultCellStyle.BackColor = Color.FromArgb(27, 42, 59);
            dgvStok.DefaultCellStyle.ForeColor = Color.White;
            dgvStok.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 204, 113);
            dgvStok.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 34, 51);
            dgvStok.RowTemplate.Height = 30;
        }
    }
}
