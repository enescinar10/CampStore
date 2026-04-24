using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// frmKategori.cs — Kategori Yönetimi Formu
// Kategori ekleme, güncelleme, silme ve listeleme işlemleri burada yapılır.
// Tüm kontroller kod ile oluşturulur, Designer kullanılmaz.
using CampStore.BusinessLayer;
using CampStore.Entities;

namespace CampStore.UI
{
    public partial class frmKategori : Form
    {
        private KategoriBL kategoriBL = new KategoriBL();

        // Seçili kategorinin ID'sini tutar — güncelleme için gerekli
        private int seciliKatID = 0;

        // ── KONTROLLER ────────────────────────────────────────────────────
        private Panel pnlBaslik, pnlForm, pnlGrid;
        private Label lblBaslik, lblKatAdi, lblUstKategori;
        private TextBox txtKatAdi;
        private ComboBox cmbUstKategori;
        private Button btnEkle, btnGuncelle, btnSil, btnTemizle;
        private DataGridView dgvKategori;

        public frmKategori()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        // ── KONTROLLERİ OLUŞTUR ───────────────────────────────────────────
        private void KontrolleriOlustur()
        {
            this.Text = "Kategori Yönetimi";
            this.Size = new Size(860, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(27, 42, 59);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ── BAŞLIK PANELİ ─────────────────────────────────────────────
            pnlBaslik = new Panel
            {
                Size = new Size(860, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(22, 34, 51)
            };
            this.Controls.Add(pnlBaslik);

            lblBaslik = new Label
            {
                Text = "🗂️  Kategori Yönetimi",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };
            pnlBaslik.Controls.Add(lblBaslik);

            // ── FORM PANELİ (giriş alanları) ──────────────────────────────
            pnlForm = new Panel
            {
                Size = new Size(860, 120),
                Location = new Point(0, 60),
                BackColor = Color.FromArgb(31, 52, 72)
            };
            this.Controls.Add(pnlForm);

            // Kategori Adı
            lblKatAdi = new Label
            {
                Text = "Kategori Adı",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };
            pnlForm.Controls.Add(lblKatAdi);

            txtKatAdi = new TextBox
            {
                Size = new Size(220, 30),
                Location = new Point(20, 38),
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlForm.Controls.Add(txtKatAdi);

            // Üst Kategori
            lblUstKategori = new Label
            {
                Text = "Üst Kategori (Opsiyonel)",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(270, 15),
                AutoSize = true
            };
            pnlForm.Controls.Add(lblUstKategori);

            cmbUstKategori = new ComboBox
            {
                Size = new Size(220, 30),
                Location = new Point(270, 38),
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            pnlForm.Controls.Add(cmbUstKategori);

            // Butonlar
            btnEkle = new Button
            {
                Text = "➕ Ekle",
                Size = new Size(110, 38),
                Location = new Point(520, 38),
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
                Location = new Point(640, 38),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false // Kayıt seçilmeden aktif olmaz
            };
            btnGuncelle.FlatAppearance.BorderSize = 0;
            btnGuncelle.Click += btnGuncelle_Click;
            pnlForm.Controls.Add(btnGuncelle);

            btnSil = new Button
            {
                Text = "🗑️ Sil",
                Size = new Size(90, 38),
                Location = new Point(760, 38),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false // Kayıt seçilmeden aktif olmaz
            };
            btnSil.FlatAppearance.BorderSize = 0;
            btnSil.Click += btnSil_Click;
            pnlForm.Controls.Add(btnSil);

            btnTemizle = new Button
            {
                Text = "🔄 Temizle",
                Size = new Size(110, 28),
                Location = new Point(520, 82),
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
                Size = new Size(860, 370),
                Location = new Point(0, 180),
                BackColor = Color.FromArgb(27, 42, 59)
            };
            this.Controls.Add(pnlGrid);

            dgvKategori = new DataGridView
            {
                Size = new Size(820, 340),
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
            dgvKategori.CellClick += dgvKategori_CellClick;
            pnlGrid.Controls.Add(dgvKategori);

            // Form load olayı
            this.Load += frmKategori_Load;
        }

        // ── FORM YÜKLENINCE ───────────────────────────────────────────────
        private void frmKategori_Load(object sender, EventArgs e)
        {
            DgvAyarla();
            KategorileriYukle();
            UstKategoriComboYukle();
        }

        // ── KATEGORİLERİ GRID'E YÜKLE ────────────────────────────────────
        private void KategorileriYukle()
        {
            try
            {
                dgvKategori.DataSource = kategoriBL.KategoriListele();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kategoriler yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── ÜST KATEGORİ COMBOBOX'INI YÜKLE ─────────────────────────────
        private void UstKategoriComboYukle()
        {
            try
            {
                DataTable dt = kategoriBL.KategoriListele();

                // Combobox'a "Yok" seçeneği ekle (ana kategori için)
                cmbUstKategori.Items.Clear();
                cmbUstKategori.Items.Add(new ComboItem { ID = 0, Ad = "— Üst Kategori Yok —" });

                foreach (DataRow row in dt.Rows)
                {
                    cmbUstKategori.Items.Add(new ComboItem
                    {
                        ID = Convert.ToInt32(row["KatID"]),
                        Ad = row["KatAdi"].ToString()
                    });
                }

                // DisplayMember ile gösterilecek alan
                cmbUstKategori.DisplayMember = "Ad";
                cmbUstKategori.ValueMember = "ID";
                cmbUstKategori.SelectedIndex = 0; // Varsayılan: Üst Kategori Yok
            }
            catch { }
        }

        // ── GRID SATIRINA TIKLANINCA ──────────────────────────────────────
        // Seçili satırın bilgilerini form alanlarına doldurur
        private void dgvKategori_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow satir = dgvKategori.Rows[e.RowIndex];

            // Seçili ID'yi sakla
            seciliKatID = Convert.ToInt32(satir.Cells["KatID"].Value);
            txtKatAdi.Text = satir.Cells["KatAdi"].Value.ToString();

            // Üst kategori combobox'ını ayarla
            if (satir.Cells["UstKategoriID"].Value == DBNull.Value ||
                satir.Cells["UstKategoriID"].Value == null)
            {
                cmbUstKategori.SelectedIndex = 0;
            }
            else
            {
                int ustID = Convert.ToInt32(satir.Cells["UstKategoriID"].Value);
                for (int i = 0; i < cmbUstKategori.Items.Count; i++)
                {
                    if (((ComboItem)cmbUstKategori.Items[i]).ID == ustID)
                    {
                        cmbUstKategori.SelectedIndex = i;
                        break;
                    }
                }
            }

            // Güncelle ve Sil butonlarını aktif et
            btnGuncelle.Enabled = true;
            btnSil.Enabled = true;
        }

        // ── EKLE BUTONU ───────────────────────────────────────────────────
        private void btnEkle_Click(object sender, EventArgs e)
        {
            Kategori k = new Kategori
            {
                KatAdi = txtKatAdi.Text.Trim(),
                UstKategoriID = ((ComboItem)cmbUstKategori.SelectedItem).ID == 0
                                ? (int?)null
                                : ((ComboItem)cmbUstKategori.SelectedItem).ID
            };

            string sonuc = kategoriBL.KategoriEkle(k);

            if (sonuc == "OK")
            {
                MessageBox.Show("Kategori başarıyla eklendi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Temizle();
                KategorileriYukle();
                UstKategoriComboYukle();
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
            if (seciliKatID == 0)
            {
                MessageBox.Show("Lütfen güncellenecek kategoriyi seçin!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Kategori k = new Kategori
            {
                KatID = seciliKatID,
                KatAdi = txtKatAdi.Text.Trim(),
                UstKategoriID = ((ComboItem)cmbUstKategori.SelectedItem).ID == 0
                                ? (int?)null
                                : ((ComboItem)cmbUstKategori.SelectedItem).ID
            };

            string sonuc = kategoriBL.KategoriGuncelle(k);

            if (sonuc == "OK")
            {
                MessageBox.Show("Kategori başarıyla güncellendi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Temizle();
                KategorileriYukle();
                UstKategoriComboYukle();
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
            if (seciliKatID == 0) return;

            DialogResult onay = MessageBox.Show(
                "Bu kategoriyi silmek istediğinize emin misiniz?",
                "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (onay == DialogResult.Yes)
            {
                string sonuc = kategoriBL.KategoriSil(seciliKatID);

                if (sonuc == "OK")
                {
                    MessageBox.Show("Kategori silindi!",
                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Temizle();
                    KategorileriYukle();
                    UstKategoriComboYukle();
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
            txtKatAdi.Clear();
            cmbUstKategori.SelectedIndex = 0;
            seciliKatID = 0;
            btnGuncelle.Enabled = false;
            btnSil.Enabled = false;
            txtKatAdi.Focus();
        }

        // ── DATAGRIDVIEW GÖRÜNÜM AYARI ────────────────────────────────────
        private void DgvAyarla()
        {
            dgvKategori.EnableHeadersVisualStyles = false;
            dgvKategori.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 52, 72);
            dgvKategori.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvKategori.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvKategori.DefaultCellStyle.BackColor = Color.FromArgb(27, 42, 59);
            dgvKategori.DefaultCellStyle.ForeColor = Color.White;
            dgvKategori.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 204, 113);
            dgvKategori.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 34, 51);
            dgvKategori.RowTemplate.Height = 30;
        }
    }

    // ── COMBOBOX İÇİN YARDIMCI SINIF ─────────────────────────────────────
    // ComboBox'a ID + Ad çifti eklemek için kullanılır
    public class ComboItem
    {
        public int ID { get; set; }
        public string Ad { get; set; }
        public override string ToString() => Ad;
    }
}
