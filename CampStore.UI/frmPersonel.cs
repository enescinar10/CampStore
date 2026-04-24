using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// frmPersonel.cs — Personel Yönetimi Formu
// Personel ekleme, güncelleme, silme ve listeleme işlemleri burada yapılır.
// Rol combobox'ı RollerDAL üzerinden doldurulur.
using CampStore.BusinessLayer;
using CampStore.Entities;

namespace CampStore.UI
{
    public partial class frmPersonel : Form
    {
        private PersonelBL personelBL = new PersonelBL();
        private int seciliPerID = 0;

        // ── KONTROLLER ────────────────────────────────────────────────────
        private Panel pnlBaslik, pnlForm, pnlGrid;
        private Label lblBaslik, lblAd, lblSoyad, lblTC;
        private Label lblTelefon, lblAdres, lblDogumTarihi;
        private Label lblIseGiris, lblIstenCikis, lblRol, lblSifre;
        private TextBox txtAd, txtSoyad, txtTC, txtTelefon;
        private TextBox txtAdres, txtSifre;
        private DateTimePicker dtpDogumTarihi, dtpIseGiris, dtpIstenCikis;
        private CheckBox chkIstenCikis;
        private ComboBox cmbRol;
        private Button btnEkle, btnGuncelle, btnSil, btnTemizle;
        private DataGridView dgvPersonel;

        public frmPersonel()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            this.Text = "Personel Yönetimi";
            this.Size = new Size(1050, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(27, 42, 59);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ── BAŞLIK ────────────────────────────────────────────────────
            pnlBaslik = new Panel
            {
                Size = new Size(1050, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(22, 34, 51)
            };
            this.Controls.Add(pnlBaslik);

            lblBaslik = new Label
            {
                Text = "👨‍💼  Personel Yönetimi",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };
            pnlBaslik.Controls.Add(lblBaslik);

            // ── FORM PANELİ ───────────────────────────────────────────────
            pnlForm = new Panel
            {
                Size = new Size(1050, 240),
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

            TextBox Txt(int x, int y, int w = 160)
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

            DateTimePicker Dtp(int x, int y, int w = 160)
            {
                var d = new DateTimePicker
                {
                    Size = new Size(w, 30),
                    Location = new Point(x, y),
                    Font = new Font("Segoe UI", 10f),
                    Format = DateTimePickerFormat.Short,
                    CalendarForeColor = Color.White,
                    CalendarMonthBackground = Color.FromArgb(44, 62, 80)
                };
                pnlForm.Controls.Add(d);
                return d;
            }

            // Satır 1
            lblAd = Lbl("Ad", 20, 15); txtAd = Txt(20, 38);
            lblSoyad = Lbl("Soyad", 200, 15); txtSoyad = Txt(200, 38);
            lblTC = Lbl("TC Kimlik No", 380, 15);
            txtTC = Txt(380, 38, 150);
            txtTC.MaxLength = 11;
            txtTC.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) && ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };

            lblTelefon = Lbl("Telefon", 550, 15); txtTelefon = Txt(550, 38, 140);
            lblSifre = Lbl("Şifre", 710, 15);
            txtSifre = Txt(710, 38, 130);
            txtSifre.PasswordChar = '●';

            // Rol combobox
            lblRol = Lbl("Rol", 860, 15);
            cmbRol = new ComboBox
            {
                Size = new Size(160, 30),
                Location = new Point(860, 38),
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            pnlForm.Controls.Add(cmbRol);

            // Satır 2
            lblAdres = Lbl("Adres", 20, 85); txtAdres = Txt(20, 108, 340);

            lblDogumTarihi = Lbl("Doğum Tarihi", 380, 85);
            dtpDogumTarihi = Dtp(380, 108);

            lblIseGiris = Lbl("İşe Giriş Tarihi", 560, 85);
            dtpIseGiris = Dtp(560, 108);

            // İşten çıkış — opsiyonel
            lblIstenCikis = Lbl("İşten Çıkış", 740, 85);
            chkIstenCikis = new CheckBox
            {
                Text = "Aktif Çalışıyor",
                Location = new Point(740, 108),
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.White,
                Checked = true,
                AutoSize = true
            };
            pnlForm.Controls.Add(chkIstenCikis);

            dtpIstenCikis = Dtp(740, 132);
            dtpIstenCikis.Enabled = false; // Başlangıçta kapalı

            // Aktif çalışıyor checkbox'ı değişince
            chkIstenCikis.CheckedChanged += (s, ev) =>
            {
                dtpIstenCikis.Enabled = !chkIstenCikis.Checked;
            };

            // Butonlar — satır 3
            btnEkle = new Button
            {
                Text = "➕ Ekle",
                Size = new Size(110, 38),
                Location = new Point(20, 185),
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
                Location = new Point(140, 185),
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
                Location = new Point(260, 185),
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
                Size = new Size(110, 38),
                Location = new Point(360, 185),
                BackColor = Color.FromArgb(127, 140, 141),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnTemizle.FlatAppearance.BorderSize = 0;
            btnTemizle.Click += btnTemizle_Click;
            pnlForm.Controls.Add(btnTemizle);

            // ── GRID PANELİ ───────────────────────────────────────────────
            pnlGrid = new Panel
            {
                Size = new Size(1050, 390),
                Location = new Point(0, 300),
                BackColor = Color.FromArgb(27, 42, 59)
            };
            this.Controls.Add(pnlGrid);

            dgvPersonel = new DataGridView
            {
                Size = new Size(1010, 360),
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
            dgvPersonel.CellClick += dgvPersonel_CellClick;
            pnlGrid.Controls.Add(dgvPersonel);

            this.Load += frmPersonel_Load;
        }

        // ── FORM YÜKLENINCE ───────────────────────────────────────────────
        private void frmPersonel_Load(object sender, EventArgs e)
        {
            DgvAyarla();
            RolComboYukle();
            PersonelleriYukle();
        }

        // ── ROL COMBOBOX ──────────────────────────────────────────────────
        private void RolComboYukle()
        {
            try
            {
                DataTable dt = personelBL.RolListele();
                cmbRol.Items.Clear();
                cmbRol.Items.Add(new ComboItem { ID = 0, Ad = "— Rol Seçin —" });

                foreach (DataRow row in dt.Rows)
                    cmbRol.Items.Add(new ComboItem
                    {
                        ID = Convert.ToInt32(row["RolID"]),
                        Ad = row["RolAdi"].ToString()
                    });

                cmbRol.DisplayMember = "Ad";
                cmbRol.ValueMember = "ID";
                cmbRol.SelectedIndex = 0;
            }
            catch { }
        }

        // ── PERSONELLERİ YÜKLE ────────────────────────────────────────────
        private void PersonelleriYukle()
        {
            try
            {
                dgvPersonel.DataSource = personelBL.PersonelListele();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Personeller yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── GRID SATIRINA TIKLANINCA ──────────────────────────────────────
        private void dgvPersonel_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow satir = dgvPersonel.Rows[e.RowIndex];

            // Kolon adı yerine index kullan — kolon adı uyuşmazlığını önler
            try
            {
                seciliPerID = Convert.ToInt32(satir.Cells["PerID"].Value);
                txtAd.Text = satir.Cells["PerAd"].Value?.ToString() ?? "";
                txtSoyad.Text = satir.Cells["PerSoyad"].Value?.ToString() ?? "";
                txtTC.Text = satir.Cells["TC"].Value?.ToString() ?? "";
                txtTelefon.Text = satir.Cells["Telefon"].Value?.ToString() ?? "";
                txtAdres.Text = satir.Cells["Adres"].Value?.ToString() ?? "";
                txtSifre.Text = satir.Cells["Sifre"].Value?.ToString() ?? "";

                // Null kontrolüyle DateTime parse et
                if (satir.Cells["DogumTarihi"].Value != null &&
                    satir.Cells["DogumTarihi"].Value != DBNull.Value)
                    dtpDogumTarihi.Value = Convert.ToDateTime(satir.Cells["DogumTarihi"].Value);

                if (satir.Cells["IseGirisTarihi"].Value != null &&
                    satir.Cells["IseGirisTarihi"].Value != DBNull.Value)
                    dtpIseGiris.Value = Convert.ToDateTime(satir.Cells["IseGirisTarihi"].Value);

                if (satir.Cells["IstenCikisTarihi"].Value == DBNull.Value ||
                    satir.Cells["IstenCikisTarihi"].Value == null)
                {
                    chkIstenCikis.Checked = true;
                    dtpIstenCikis.Enabled = false;
                }
                else
                {
                    chkIstenCikis.Checked = false;
                    dtpIstenCikis.Enabled = true;
                    dtpIstenCikis.Value = Convert.ToDateTime(satir.Cells["IstenCikisTarihi"].Value);
                }

                // Rol seç
                int rolID = Convert.ToInt32(satir.Cells["RolID"].Value);
                for (int i = 0; i < cmbRol.Items.Count; i++)
                {
                    if (((ComboItem)cmbRol.Items[i]).ID == rolID)
                    {
                        cmbRol.SelectedIndex = i;
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

        // ── EKLE ─────────────────────────────────────────────────────────
        private void btnEkle_Click(object sender, EventArgs e)
        {
            Personel p = new Personel
            {
                PerAd = txtAd.Text.Trim(),
                PerSoyad = txtSoyad.Text.Trim(),
                TC = txtTC.Text.Trim(),
                Telefon = txtTelefon.Text.Trim(),
                Adres = txtAdres.Text.Trim(),
                Sifre = txtSifre.Text,
                RolID = cmbRol.SelectedItem is ComboItem ci ? ci.ID : 0,
                DogumTarihi = dtpDogumTarihi.Value,
                IseGirisTarihi = dtpIseGiris.Value,
                IstenCikisTarihi = chkIstenCikis.Checked
                                   ? (DateTime?)null
                                   : dtpIstenCikis.Value
            };

            string sonuc = personelBL.PersonelEkle(p);

            if (sonuc == "OK")
            {
                MessageBox.Show("Personel başarıyla eklendi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Temizle();
                PersonelleriYukle();
            }
            else
            {
                MessageBox.Show(sonuc, "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ── GÜNCELLE ─────────────────────────────────────────────────────
        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliPerID == 0) return;

            Personel p = new Personel
            {
                PerID = seciliPerID,
                PerAd = txtAd.Text.Trim(),
                PerSoyad = txtSoyad.Text.Trim(),
                TC = txtTC.Text.Trim(),
                Telefon = txtTelefon.Text.Trim(),
                Adres = txtAdres.Text.Trim(),
                Sifre = txtSifre.Text,
                RolID = cmbRol.SelectedItem is ComboItem ci ? ci.ID : 0,
                DogumTarihi = dtpDogumTarihi.Value,
                IseGirisTarihi = dtpIseGiris.Value,
                IstenCikisTarihi = chkIstenCikis.Checked
                                   ? (DateTime?)null
                                   : dtpIstenCikis.Value
            };

            string sonuc = personelBL.PersonelGuncelle(p);

            if (sonuc == "OK")
            {
                MessageBox.Show("Personel başarıyla güncellendi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Temizle();
                PersonelleriYukle();
            }
            else
            {
                MessageBox.Show(sonuc, "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ── SİL ──────────────────────────────────────────────────────────
        private void btnSil_Click(object sender, EventArgs e)
        {
            if (seciliPerID == 0) return;

            if (MessageBox.Show("Bu personeli silmek istediğinize emin misiniz?",
                "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                string sonuc = personelBL.PersonelSil(seciliPerID);

                if (sonuc == "OK")
                {
                    MessageBox.Show("Personel silindi!",
                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Temizle();
                    PersonelleriYukle();
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
            txtAd.Clear(); txtSoyad.Clear(); txtTC.Clear();
            txtTelefon.Clear(); txtAdres.Clear(); txtSifre.Clear();
            dtpDogumTarihi.Value = DateTime.Now;
            dtpIseGiris.Value = DateTime.Now;
            dtpIstenCikis.Value = DateTime.Now;
            dtpIstenCikis.Enabled = false;
            chkIstenCikis.Checked = true;
            cmbRol.SelectedIndex = 0;
            seciliPerID = 0;
            btnGuncelle.Enabled = false;
            btnSil.Enabled = false;
            txtAd.Focus();
        }

        // ── DGV AYARLA ───────────────────────────────────────────────────
        private void DgvAyarla()
        {
            dgvPersonel.EnableHeadersVisualStyles = false;
            dgvPersonel.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 52, 72);
            dgvPersonel.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPersonel.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvPersonel.DefaultCellStyle.BackColor = Color.FromArgb(27, 42, 59);
            dgvPersonel.DefaultCellStyle.ForeColor = Color.White;
            dgvPersonel.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 204, 113);
            dgvPersonel.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 34, 51);
            dgvPersonel.RowTemplate.Height = 30;
        }
    }
}
