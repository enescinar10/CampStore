using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// frmFatura.cs — Fatura Yönetimi Formu
// Fatura ekleme, güncelleme, silme ve listeleme işlemleri burada yapılır.
// Satış seçilince fatura bilgileri otomatik dolar.
using CampStore.BusinessLayer;
using CampStore.Entities;

namespace CampStore.UI
{
    public partial class frmFatura : Form
    {
        // Diğer buton değişkenlerinin yanına ekle
        private Button btnYazdir;
        private FaturaBL faturaBL = new FaturaBL();
        private SatisBL satisBL = new SatisBL();

        private int seciliFaturaID = 0;

        // ── KONTROLLER ────────────────────────────────────────────────────
        private Panel pnlBaslik, pnlForm, pnlGrid;
        private Label lblBaslik, lblSatis, lblFaturaNo, lblTarih, lblTutar;
        private ComboBox cmbSatis;
        private TextBox txtFaturaNo, txtTutar;
        private DateTimePicker dtpTarih;
        private Button btnEkle, btnGuncelle, btnSil, btnTemizle, btnOtomatikNo;
        private DataGridView dgvFatura;

        public frmFatura()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            this.Text = "Fatura Yönetimi";
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
                Text = "🧾  Fatura Yönetimi",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true,
                Parent = pnlBaslik
            };

            // ── FORM PANELİ ───────────────────────────────────────────────
            pnlForm = new Panel
            {
                Size = new Size(1000, 140),
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

            TextBox Txt(int x, int y, int w = 180)
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

            // Satış seçimi
            lblSatis = Lbl("Satış", 20, 15);
            cmbSatis = new ComboBox
            {
                Size = new Size(250, 30),
                Location = new Point(20, 38),
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            // Satış seçilince tutarı otomatik getir
            cmbSatis.SelectedIndexChanged += (s, ev) =>
            {
                if (cmbSatis.SelectedItem is ComboItem item && item.ID > 0)
                {
                    Satis satis = satisBL.SatisGetirById(item.ID);
                    if (satis != null)
                        txtTutar.Text = satis.ToplamTutar.ToString("F2");
                }
            };
            pnlForm.Controls.Add(cmbSatis);

            // Fatura No
            lblFaturaNo = Lbl("Fatura No", 290, 15);
            txtFaturaNo = Txt(290, 38, 200);

            // Otomatik No butonu
            btnOtomatikNo = new Button
            {
                Text = "🔁 Otomatik",
                Size = new Size(100, 30),
                Location = new Point(500, 38),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnOtomatikNo.FlatAppearance.BorderSize = 0;
            btnOtomatikNo.Click += (s, ev) =>
            {
                // FaturaBL'den otomatik numara üret
                txtFaturaNo.Text = faturaBL.FaturaNoUret();
            };
            pnlForm.Controls.Add(btnOtomatikNo);

            // Tarih
            lblTarih = Lbl("Tarih", 620, 15);
            dtpTarih = new DateTimePicker
            {
                Size = new Size(160, 30),
                Location = new Point(620, 38),
                Font = new Font("Segoe UI", 10f),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };
            pnlForm.Controls.Add(dtpTarih);

            // Tutar
            lblTutar = Lbl("Tutar (₺)", 800, 15);
            txtTutar = Txt(800, 38, 140);
            //txtTutar.ReadOnly = true; // Satıştan otomatik gelir
            //txtTutar.BackColor = Color.FromArgb(30, 50, 70);
            // Sadece rakam ve virgül girilsin
            txtTutar.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) &&
                    ev.KeyChar != ',' &&
                    ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };

            // Butonlar — satır 2
            btnEkle = new Button
            {
                Text = "➕ Ekle",
                Size = new Size(110, 38),
                Location = new Point(20, 88),
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
                Location = new Point(140, 88),
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
                Location = new Point(260, 88),
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
                Location = new Point(360, 88),
                BackColor = Color.FromArgb(127, 140, 141),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnTemizle.FlatAppearance.BorderSize = 0;
            btnTemizle.Click += btnTemizle_Click;
            pnlForm.Controls.Add(btnTemizle);

            btnYazdir = new Button
            {
                Text = "🖨️ Yazdır",
                Size = new Size(110, 38),
                Location = new Point(480, 88),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnYazdir.FlatAppearance.BorderSize = 0;
            btnYazdir.Click += btnYazdir_Click;
            pnlForm.Controls.Add(btnYazdir);


            // ── GRID PANELİ ───────────────────────────────────────────────
            pnlGrid = new Panel
            {
                Size = new Size(1000, 400),
                Location = new Point(0, 200),
                BackColor = Color.FromArgb(27, 42, 59)
            };
            this.Controls.Add(pnlGrid);

            dgvFatura = new DataGridView
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
            dgvFatura.CellClick += dgvFatura_CellClick;
            pnlGrid.Controls.Add(dgvFatura);

            this.Load += frmFatura_Load;
        }

        // ── FORM YÜKLENINCE ───────────────────────────────────────────────
        private void frmFatura_Load(object sender, EventArgs e)
        {
            DgvAyarla();
            SatisComboYukle();
            FaturalariYukle();
        }

        // ── SATIŞ COMBO ───────────────────────────────────────────────────
        private void SatisComboYukle()
        {
            try
            {
                DataTable dt = satisBL.SatisListele();
                cmbSatis.Items.Clear();
                cmbSatis.Items.Add(new ComboItem { ID = 0, Ad = "— Satış Seçin —" });

                foreach (DataRow row in dt.Rows)
                    cmbSatis.Items.Add(new ComboItem
                    {
                        ID = Convert.ToInt32(row["SatisID"]),
                        Ad = $"Satış #{row["SatisID"]} — " +
                             $"{row["MusteriID"]} — " +
                             $"{Convert.ToDecimal(row["ToplamTutar"]):F2} ₺"
                    });

                cmbSatis.DisplayMember = "Ad";
                cmbSatis.ValueMember = "ID";
                cmbSatis.SelectedIndex = 0;
            }
            catch { }
        }

        // ── FATURALARI YÜKLE ──────────────────────────────────────────────
        private void FaturalariYukle()
        {
            try
            {
                dgvFatura.DataSource = faturaBL.FaturaListele();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Faturalar yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── GRID SATIRINA TIKLANINCA ──────────────────────────────────────
        private void dgvFatura_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow satir = dgvFatura.Rows[e.RowIndex];

            seciliFaturaID = Convert.ToInt32(satir.Cells["FaturaID"].Value);
            txtFaturaNo.Text = satir.Cells["FaturaNo"].Value.ToString();
            dtpTarih.Value = Convert.ToDateTime(satir.Cells["Tarih"].Value);
            txtTutar.Text = satir.Cells["Tutar"].Value.ToString();

            // Satış seç
            int satisID = Convert.ToInt32(satir.Cells["SatisID"].Value);
            for (int i = 0; i < cmbSatis.Items.Count; i++)
            {
                if (((ComboItem)cmbSatis.Items[i]).ID == satisID)
                {
                    cmbSatis.SelectedIndex = i;
                    break;
                }
            }

            btnGuncelle.Enabled = true;
            btnSil.Enabled = true;
            btnYazdir.Enabled = true;
        }

        // ── EKLE ─────────────────────────────────────────────────────────
        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtTutar.Text, out decimal tutar))
            {
                MessageBox.Show("Geçerli bir tutar giriniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Fatura f = new Fatura
            {
                SatisID = cmbSatis.SelectedItem is ComboItem cs ? cs.ID : 0,
                FaturaNo = txtFaturaNo.Text.Trim(),
                Tarih = dtpTarih.Value,
                Tutar = tutar
            };

            string sonuc = faturaBL.FaturaEkle(f);

            if (sonuc == "OK")
            {
                MessageBox.Show("Fatura başarıyla eklendi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Temizle();
                FaturalariYukle();
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
            if (seciliFaturaID == 0) return;

            if (!decimal.TryParse(txtTutar.Text, out decimal tutar))
            {
                MessageBox.Show("Geçerli bir tutar giriniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Fatura f = new Fatura
            {
                FaturaID = seciliFaturaID,
                SatisID = cmbSatis.SelectedItem is ComboItem cs ? cs.ID : 0,
                FaturaNo = txtFaturaNo.Text.Trim(),
                Tarih = dtpTarih.Value,
                Tutar = tutar
            };

            string sonuc = faturaBL.FaturaGuncelle(f);

            if (sonuc == "OK")
            {
                MessageBox.Show("Fatura güncellendi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Temizle();
                FaturalariYukle();
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
            if (seciliFaturaID == 0) return;

            if (MessageBox.Show("Bu faturayı silmek istediğinize emin misiniz?",
                "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                string sonuc = faturaBL.FaturaSil(seciliFaturaID);

                if (sonuc == "OK")
                {
                    MessageBox.Show("Fatura silindi!",
                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Temizle();
                    FaturalariYukle();
                }
                else
                {
                    MessageBox.Show(sonuc, "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        // ── FATURA YAZDIR ─────────────────────────────────────────────────
        private void btnYazdir_Click(object sender, EventArgs e)
        {
            if (seciliFaturaID == 0)
            {
                MessageBox.Show("Lütfen yazdırılacak faturayı seçin!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Fatura fatura = faturaBL.FaturaGetirById(seciliFaturaID);
            if (fatura == null) return;

            // PrintDocument oluştur
            System.Drawing.Printing.PrintDocument pd =
                new System.Drawing.Printing.PrintDocument();

            pd.PrintPage += (s, ev) =>
            {
                Graphics g = ev.Graphics;
                Font fontBaslik = new Font("Segoe UI", 18f, FontStyle.Bold);
                Font fontNormal = new Font("Segoe UI", 10f);
                Font fontKalin = new Font("Segoe UI", 10f, FontStyle.Bold);
                Font fontKucuk = new Font("Segoe UI", 8f);
                Brush siyah = Brushes.Black;
                Brush gri = new SolidBrush(Color.FromArgb(100, 100, 100));
                int y = 40;
                int solKenar = 60;

                // ── ŞİRKET BAŞLIĞI ────────────────────────────────────────
                g.DrawString("🏕️ CampStore", fontBaslik, siyah, solKenar, y);
                y += 35;
                g.DrawString("Kamp Malzemeleri Yönetim Sistemi",
                    fontNormal, gri, solKenar, y);
                y += 20;
                g.DrawString("Tel: 0232 123 45 67 | info@campstore.com",
                    fontKucuk, gri, solKenar, y);
                y += 30;

                // Çizgi
                g.DrawLine(Pens.Black, solKenar, y, 540, y);
                y += 15;

                // ── FATURA BAŞLIĞI ─────────────────────────────────────────
                g.DrawString("FATURA", new Font("Segoe UI", 14f, FontStyle.Bold),
                    siyah, solKenar, y);
                y += 30;

                // Fatura bilgileri
                g.DrawString("Fatura No  :", fontKalin, siyah, solKenar, y);
                g.DrawString(fatura.FaturaNo, fontNormal, siyah, solKenar + 120, y);
                y += 22;

                g.DrawString("Tarih        :", fontKalin, siyah, solKenar, y);
                g.DrawString(fatura.Tarih.ToString("dd.MM.yyyy"), fontNormal, siyah, solKenar + 120, y);
                y += 22;

                g.DrawString("Satış ID    :", fontKalin, siyah, solKenar, y);
                g.DrawString(fatura.SatisID.ToString(), fontNormal, siyah, solKenar + 120, y);
                y += 35;

                // Çizgi
                g.DrawLine(Pens.Black, solKenar, y, 540, y);
                y += 15;

                // ── TUTAR ALANI ────────────────────────────────────────────
                g.DrawString("Toplam Tutar :", fontKalin, siyah, solKenar, y);
                g.DrawString($"{fatura.Tutar:F2} ₺",
                    new Font("Segoe UI", 12f, FontStyle.Bold),
                    siyah, solKenar + 140, y);
                y += 40;

                // Alt çizgi
                g.DrawLine(Pens.Black, solKenar, y, 540, y);
                y += 15;

                // ── TEŞEKKÜR MESAJI ────────────────────────────────────────
                g.DrawString("Bizi tercih ettiğiniz için teşekkür ederiz!",
                    fontKucuk, gri, solKenar, y);
                y += 15;
                g.DrawString($"Yazdırma Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}",
                    fontKucuk, gri, solKenar, y);
            };

            // Print Preview göster
            System.Windows.Forms.PrintPreviewDialog preview =
                new System.Windows.Forms.PrintPreviewDialog();
            preview.Document = pd;
            preview.Width = 800;
            preview.Height = 600;
            preview.ShowDialog();
        }

        // ── TEMİZLE ──────────────────────────────────────────────────────
        private void btnTemizle_Click(object sender, EventArgs e) => Temizle();

        private void Temizle()
        {
            cmbSatis.SelectedIndex = 0;
            txtFaturaNo.Clear();
            dtpTarih.Value = DateTime.Now;
            txtTutar.Clear();
            seciliFaturaID = 0;
            btnGuncelle.Enabled = false;
            btnSil.Enabled = false;
            btnYazdir.Enabled = false; 
        }

        // ── DGV AYARLA ───────────────────────────────────────────────────
        private void DgvAyarla()
        {
            dgvFatura.EnableHeadersVisualStyles = false;
            dgvFatura.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 52, 72);
            dgvFatura.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvFatura.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvFatura.DefaultCellStyle.BackColor = Color.FromArgb(27, 42, 59);
            dgvFatura.DefaultCellStyle.ForeColor = Color.White;
            dgvFatura.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 204, 113);
            dgvFatura.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 34, 51);
            dgvFatura.RowTemplate.Height = 30;
        }
    }
}