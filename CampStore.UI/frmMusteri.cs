using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// frmMusteri.cs — Müşteri Yönetimi Formu
// Müşteri ekleme, güncelleme, silme ve listeleme işlemleri burada yapılır.
// Şehir seçilince ilçe combobox'ı otomatik filtrelenir.
using CampStore.BusinessLayer;
using CampStore.Entities;

namespace CampStore.UI
{
    public partial class frmMusteri : Form
    {
        private MusteriBL musteriBL = new MusteriBL();
        private int seciliMusteriID = 0;

        // ── KONTROLLER ────────────────────────────────────────────────────
        private Panel pnlBaslik, pnlForm, pnlGrid;
        private Label lblBaslik, lblAd, lblSoyad, lblEmail;
        private Label lblTelefon, lblSifre, lblAdres, lblSehir, lblIlce, lblDurum;
        private TextBox txtAd, txtSoyad, txtEmail, txtTelefon, txtSifre, txtAdres;
        private ComboBox cmbSehir, cmbIlce;
        private CheckBox chkDurum;
        private Button btnEkle, btnGuncelle, btnSil, btnTemizle;
        private DataGridView dgvMusteri;

        public frmMusteri()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            this.Text = "Müşteri Yönetimi";
            this.Size = new Size(1050, 680);
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
                Text = "👤  Müşteri Yönetimi",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };
            pnlBaslik.Controls.Add(lblBaslik);

            // ── FORM PANELİ ───────────────────────────────────────────────
            pnlForm = new Panel
            {
                Size = new Size(1050, 210),
                Location = new Point(0, 60),
                BackColor = Color.FromArgb(31, 52, 72)
            };
            this.Controls.Add(pnlForm);

            // Yardımcı metot: label oluştur
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

            // Yardımcı metot: textbox oluştur
            TextBox Txt(int x, int y, int w = 170)
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

            // Yardımcı metot: combobox oluştur
            ComboBox Cmb(int x, int y, int w = 170)
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
                pnlForm.Controls.Add(c);
                return c;
            }

            // Satır 1
            lblAd = Lbl("Ad", 20, 15); txtAd = Txt(20, 38);
            lblSoyad = Lbl("Soyad", 210, 15); txtSoyad = Txt(210, 38);
            lblEmail = Lbl("Email", 400, 15); txtEmail = Txt(400, 38, 220);
            lblTelefon = Lbl("Telefon", 640, 15); txtTelefon = Txt(640, 38, 150);
            lblSifre = Lbl("Şifre", 810, 15); txtSifre = Txt(810, 38, 150);

            // Satır 2
            lblAdres = Lbl("Adres", 20, 85); txtAdres = Txt(20, 108, 360);
            lblSehir = Lbl("Şehir", 400, 85); cmbSehir = Cmb(400, 108, 180);
            lblIlce = Lbl("İlçe", 600, 85); cmbIlce = Cmb(600, 108, 180);

            lblDurum = Lbl("Durum", 810, 85);
            chkDurum = new CheckBox
            {
                Text = "Aktif",
                Location = new Point(810, 110),
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.White,
                Checked = true
            };
            pnlForm.Controls.Add(chkDurum);

            // Şifre maskeli olsun
            txtSifre.PasswordChar = '●';

            // Butonlar — satır 3
            btnEkle = new Button
            {
                Text = "➕ Ekle",
                Size = new Size(110, 38),
                Location = new Point(20, 158),
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
                Location = new Point(140, 158),
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
                Location = new Point(260, 158),
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
                Location = new Point(360, 158),
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
                Size = new Size(1050, 400),
                Location = new Point(0, 270),
                BackColor = Color.FromArgb(27, 42, 59)
            };
            this.Controls.Add(pnlGrid);

            dgvMusteri = new DataGridView
            {
                Size = new Size(1010, 370),
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
            dgvMusteri.CellClick += dgvMusteri_CellClick;
            pnlGrid.Controls.Add(dgvMusteri);

            this.Load += frmMusteri_Load;
        }

        // ── FORM YÜKLENINCE ───────────────────────────────────────────────
        private void frmMusteri_Load(object sender, EventArgs e)
        {
            DgvAyarla();
            SehirComboYukle();
            MusterileriYukle();

            // Şehir değişince ilçeleri filtrele
            cmbSehir.SelectedIndexChanged += (s, ev) =>
            {
                if (cmbSehir.SelectedItem is ComboItem item && item.ID > 0)
                    IlceComboYukle(item.ID);
                else
                    cmbIlce.Items.Clear();
            };
        }

        // ── MÜŞTERİLERİ YÜKLE ────────────────────────────────────────────
        private void MusterileriYukle()
        {
            try
            {
                dgvMusteri.DataSource = musteriBL.MusteriListele();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteriler yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── ŞEHİR COMBOBOX ───────────────────────────────────────────────
        private void SehirComboYukle()
        {
            try
            {
                DataTable dt = musteriBL.SehirListele();
                cmbSehir.Items.Clear();
                cmbSehir.Items.Add(new ComboItem { ID = 0, Ad = "— Şehir Seçin —" });

                foreach (DataRow row in dt.Rows)
                    cmbSehir.Items.Add(new ComboItem
                    {
                        ID = Convert.ToInt32(row["SehirID"]),
                        Ad = row["SehirAdi"].ToString()
                    });

                cmbSehir.DisplayMember = "Ad";
                cmbSehir.ValueMember = "ID";
                cmbSehir.SelectedIndex = 0;
            }
            catch { }
        }

        // ── İLÇE COMBOBOX ────────────────────────────────────────────────
        private void IlceComboYukle(int sehirID)
        {
            try
            {
                DataTable dt = musteriBL.IlceListeleBySehir(sehirID);
                cmbIlce.Items.Clear();
                cmbIlce.Items.Add(new ComboItem { ID = 0, Ad = "— İlçe Seçin —" });

                foreach (DataRow row in dt.Rows)
                    cmbIlce.Items.Add(new ComboItem
                    {
                        ID = Convert.ToInt32(row["IlceID"]),
                        Ad = row["IlceAdi"].ToString()
                    });

                cmbIlce.DisplayMember = "Ad";
                cmbIlce.ValueMember = "ID";
                cmbIlce.SelectedIndex = 0;
            }
            catch { }
        }
        private void dgvMusteri_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow satir = dgvMusteri.Rows[e.RowIndex];

                seciliMusteriID = Convert.ToInt32(satir.Cells["MusteriID"].Value);
                txtAd.Text = satir.Cells["Ad"].Value?.ToString() ?? "";
                txtSoyad.Text = satir.Cells["Soyad"].Value?.ToString() ?? "";
                txtEmail.Text = satir.Cells["Email"].Value?.ToString() ?? "";
                txtTelefon.Text = satir.Cells["Telefon"].Value?.ToString() ?? "";
                txtAdres.Text = satir.Cells["Adres"].Value?.ToString() ?? "";
                txtSifre.Text = satir.Cells["Sifre"].Value?.ToString() ?? "";
                chkDurum.Checked = Convert.ToBoolean(satir.Cells["Durum"].Value);

                // Şehir seç
                int sehirID = Convert.ToInt32(satir.Cells["SehirID"].Value);
                for (int i = 0; i < cmbSehir.Items.Count; i++)
                {
                    if (((ComboItem)cmbSehir.Items[i]).ID == sehirID)
                    {
                        cmbSehir.SelectedIndex = i;
                        break;
                    }
                }

                // İlçe yükle ve seç
                IlceComboYukle(sehirID);
                int ilceID = Convert.ToInt32(satir.Cells["IlceID"].Value);
                for (int i = 0; i < cmbIlce.Items.Count; i++)
                {
                    if (((ComboItem)cmbIlce.Items[i]).ID == ilceID)
                    {
                        cmbIlce.SelectedIndex = i;
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
        //private void dgvMusteri_CellClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex < 0) return;

        //    DataGridViewRow satir = dgvMusteri.Rows[e.RowIndex];

        //    seciliMusteriID = Convert.ToInt32(satir.Cells["MusteriID"].Value);
        //    txtAd.Text = satir.Cells["Ad"].Value.ToString();
        //    txtSoyad.Text = satir.Cells["Soyad"].Value.ToString();
        //    txtEmail.Text = satir.Cells["Email"].Value.ToString();
        //    txtTelefon.Text = satir.Cells["Telefon"].Value.ToString();
        //    txtAdres.Text = satir.Cells["Adres"].Value.ToString();
        //    txtSifre.Text = satir.Cells["Sifre"].Value.ToString();
        //    chkDurum.Checked = Convert.ToBoolean(satir.Cells["Durum"].Value);

        //    // Şehir seç
        //    int sehirID = Convert.ToInt32(satir.Cells["SehirID"].Value);
        //    for (int i = 0; i < cmbSehir.Items.Count; i++)
        //    {
        //        if (((ComboItem)cmbSehir.Items[i]).ID == sehirID)
        //        {
        //            cmbSehir.SelectedIndex = i;
        //            break;
        //        }
        //    }

        //    // İlçe yükle ve seç
        //    IlceComboYukle(sehirID);
        //    int ilceID = Convert.ToInt32(satir.Cells["IlceID"].Value);
        //    for (int i = 0; i < cmbIlce.Items.Count; i++)
        //    {
        //        if (((ComboItem)cmbIlce.Items[i]).ID == ilceID)
        //        {
        //            cmbIlce.SelectedIndex = i;
        //            break;
        //        }
        //    }

        //    btnGuncelle.Enabled = true;
        //    btnSil.Enabled = true;
        //}

        // ── EKLE ─────────────────────────────────────────────────────────
        private void btnEkle_Click(object sender, EventArgs e)
        {
            Musteri m = new Musteri
            {
                Ad = txtAd.Text.Trim(),
                Soyad = txtSoyad.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Sifre = txtSifre.Text,
                Telefon = txtTelefon.Text.Trim(),
                Adres = txtAdres.Text.Trim(),
                SehirID = cmbSehir.SelectedItem is ComboItem cs ? cs.ID : 0,
                IlceID = cmbIlce.SelectedItem is ComboItem ci ? ci.ID : 0,
                Durum = chkDurum.Checked
            };

            string sonuc = musteriBL.MusteriEkle(m);

            if (sonuc == "OK")
            {
                MessageBox.Show("Müşteri başarıyla eklendi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Temizle();
                MusterileriYukle();
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
            if (seciliMusteriID == 0) return;

            Musteri m = new Musteri
            {
                MusteriID = seciliMusteriID,
                Ad = txtAd.Text.Trim(),
                Soyad = txtSoyad.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Sifre = txtSifre.Text,
                Telefon = txtTelefon.Text.Trim(),
                Adres = txtAdres.Text.Trim(),
                SehirID = cmbSehir.SelectedItem is ComboItem cs ? cs.ID : 0,
                IlceID = cmbIlce.SelectedItem is ComboItem ci ? ci.ID : 0,
                Durum = chkDurum.Checked
            };

            string sonuc = musteriBL.MusteriGuncelle(m);

            if (sonuc == "OK")
            {
                MessageBox.Show("Müşteri başarıyla güncellendi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Temizle();
                MusterileriYukle();
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
            if (seciliMusteriID == 0) return;

            if (MessageBox.Show("Bu müşteriyi silmek istediğinize emin misiniz?",
                "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                string sonuc = musteriBL.MusteriSil(seciliMusteriID);

                if (sonuc == "OK")
                {
                    MessageBox.Show("Müşteri silindi!",
                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Temizle();
                    MusterileriYukle();
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
            txtAd.Clear(); txtSoyad.Clear(); txtEmail.Clear();
            txtTelefon.Clear(); txtSifre.Clear(); txtAdres.Clear();
            chkDurum.Checked = true;
            cmbSehir.SelectedIndex = 0;
            cmbIlce.Items.Clear();
            seciliMusteriID = 0;
            btnGuncelle.Enabled = false;
            btnSil.Enabled = false;
            txtAd.Focus();
        }

        // ── DGV AYARLA ───────────────────────────────────────────────────
        private void DgvAyarla()
        {
            dgvMusteri.EnableHeadersVisualStyles = false;
            dgvMusteri.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 52, 72);
            dgvMusteri.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMusteri.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvMusteri.DefaultCellStyle.BackColor = Color.FromArgb(27, 42, 59);
            dgvMusteri.DefaultCellStyle.ForeColor = Color.White;
            dgvMusteri.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 204, 113);
            dgvMusteri.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 34, 51);
            dgvMusteri.RowTemplate.Height = 30;
        }
    }
}
