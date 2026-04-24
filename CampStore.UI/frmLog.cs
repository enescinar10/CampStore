using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// frmLog.cs — Log Kayıtları Formu
// Sistemdeki tüm işlem kayıtları burada listelenir.
// Sadece görüntüleme amaçlıdır, ekleme/silme yapılmaz.

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CampStore.BusinessLayer;

namespace CampStore.UI
{
    public partial class frmLog : Form
    {
        private LogBL logBL = new LogBL();

        // ── KONTROLLER ────────────────────────────────────────────────────
        private Panel pnlBaslik, pnlForm, pnlGrid;
        private Label lblBaslik, lblIslemTuru, lblAra;
        private ComboBox cmbIslemTuru;
        private TextBox txtAra;
        private Button btnFiltrele, btnTemizle, btnYenile;
        private DataGridView dgvLog;
        private Label lblToplamKayit;

        public frmLog()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            this.Text = "Log Kayıtları";
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

            lblBaslik = new Label
            {
                Text = "📋  Log Kayıtları",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };
            pnlBaslik.Controls.Add(lblBaslik);

            // ── FİLTRE PANELİ ─────────────────────────────────────────────
            pnlForm = new Panel
            {
                Size = new Size(1000, 80),
                Location = new Point(0, 60),
                BackColor = Color.FromArgb(31, 52, 72)
            };
            this.Controls.Add(pnlForm);

            // İşlem türü filtresi
            lblIslemTuru = new Label
            {
                Text = "İşlem Türü",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };
            pnlForm.Controls.Add(lblIslemTuru);

            cmbIslemTuru = new ComboBox
            {
                Size = new Size(180, 30),
                Location = new Point(20, 38),
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            // Log türleri — sistemde kullanılan işlem türleri
            cmbIslemTuru.Items.AddRange(new object[]
            {
                "Tümü",
                "GİRİŞ",
                "EKLE",
                "GÜNCELLE",
                "SİL",
                "SATIŞ",
                "STOK",
                "HATA"
            });
            cmbIslemTuru.SelectedIndex = 0;
            pnlForm.Controls.Add(cmbIslemTuru);

            // Arama kutusu
            lblAra = new Label
            {
                Text = "Açıklamada Ara",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(220, 15),
                AutoSize = true
            };
            pnlForm.Controls.Add(lblAra);

            txtAra = new TextBox
            {
                Size = new Size(280, 30),
                Location = new Point(220, 38),
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            // Enter tuşuyla filtrele
            txtAra.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter)
                    Filtrele();
            };
            pnlForm.Controls.Add(txtAra);

            // Filtrele butonu
            btnFiltrele = new Button
            {
                Text = "🔍 Filtrele",
                Size = new Size(110, 38),
                Location = new Point(520, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnFiltrele.FlatAppearance.BorderSize = 0;
            btnFiltrele.Click += (s, ev) => Filtrele();
            pnlForm.Controls.Add(btnFiltrele);

            // Temizle butonu
            btnTemizle = new Button
            {
                Text = "🔄 Temizle",
                Size = new Size(110, 38),
                Location = new Point(640, 30),
                BackColor = Color.FromArgb(127, 140, 141),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnTemizle.FlatAppearance.BorderSize = 0;
            btnTemizle.Click += (s, ev) =>
            {
                cmbIslemTuru.SelectedIndex = 0;
                txtAra.Clear();
                LoglariYukle();
            };
            pnlForm.Controls.Add(btnTemizle);

            // Yenile butonu
            btnYenile = new Button
            {
                Text = "🔁 Yenile",
                Size = new Size(110, 38),
                Location = new Point(760, 30),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnYenile.FlatAppearance.BorderSize = 0;
            btnYenile.Click += (s, ev) => LoglariYukle();
            pnlForm.Controls.Add(btnYenile);

            // ── GRID PANELİ ───────────────────────────────────────────────
            pnlGrid = new Panel
            {
                Size = new Size(1000, 460),
                Location = new Point(0, 140),
                BackColor = Color.FromArgb(27, 42, 59)
            };
            this.Controls.Add(pnlGrid);

            // Toplam kayıt sayısı
            lblToplamKayit = new Label
            {
                Text = "Toplam Kayıt: 0",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(149, 165, 166),
                Location = new Point(20, 8),
                AutoSize = true
            };
            pnlGrid.Controls.Add(lblToplamKayit);

            dgvLog = new DataGridView
            {
                Size = new Size(960, 415),
                Location = new Point(20, 30),
                BackgroundColor = Color.FromArgb(27, 42, 59),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            pnlGrid.Controls.Add(dgvLog);

            this.Load += frmLog_Load;
        }

        // ── FORM YÜKLENINCE ───────────────────────────────────────────────
        private void frmLog_Load(object sender, EventArgs e)
        {
            DgvAyarla();
            LoglariYukle();
        }

        // ── LOGLARI YÜKLE ─────────────────────────────────────────────────
        private void LoglariYukle()
        {
            try
            {
                DataTable dt = logBL.LogListele();
                dgvLog.DataSource = dt;
                lblToplamKayit.Text = $"Toplam Kayıt: {dt.Rows.Count}";

                // Satır renklerini işlem türüne göre ayarla
                RenkleriAyarla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Log kayıtları yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── FİLTRELE ─────────────────────────────────────────────────────
        private void Filtrele()
        {
            try
            {
                DataTable dt = logBL.LogListele();
                DataView view = dt.DefaultView;

                // Filtre koşullarını birleştir
                string filtre = "";

                // İşlem türü filtresi
                if (cmbIslemTuru.SelectedIndex > 0)
                {
                    string tur = cmbIslemTuru.SelectedItem.ToString();
                    filtre += $"IslemTuru = '{tur}'";
                }

                // Arama kelimesi filtresi
                if (!string.IsNullOrWhiteSpace(txtAra.Text))
                {
                    if (!string.IsNullOrEmpty(filtre))
                        filtre += " AND ";
                    filtre += $"Aciklama LIKE '%{txtAra.Text.Trim()}%'";
                }

                view.RowFilter = filtre;
                dgvLog.DataSource = view.ToTable();
                lblToplamKayit.Text = $"Toplam Kayıt: {view.Count}";

                RenkleriAyarla();
            }
            catch { }
        }

        // ── SATIR RENKLERİNİ İŞLEM TÜRÜNE GÖRE AYARLA ────────────────────
        // Her işlem türü farklı renkte gösterilir — okunabilirliği artırır
        private void RenkleriAyarla()
        {
            foreach (DataGridViewRow satir in dgvLog.Rows)
            {
                if (satir.Cells["IslemTuru"].Value == null) continue;

                string tur = satir.Cells["IslemTuru"].Value.ToString();

                switch (tur)
                {
                    case "GİRİŞ":
                        satir.DefaultCellStyle.ForeColor = Color.FromArgb(46, 204, 113);  // Yeşil
                        break;
                    case "EKLE":
                        satir.DefaultCellStyle.ForeColor = Color.FromArgb(52, 152, 219);  // Mavi
                        break;
                    case "GÜNCELLE":
                        satir.DefaultCellStyle.ForeColor = Color.FromArgb(230, 126, 34);  // Turuncu
                        break;
                    case "SİL":
                        satir.DefaultCellStyle.ForeColor = Color.FromArgb(231, 76, 60);   // Kırmızı
                        break;
                    case "SATIŞ":
                        satir.DefaultCellStyle.ForeColor = Color.FromArgb(155, 89, 182);  // Mor
                        break;
                    case "STOK":
                        satir.DefaultCellStyle.ForeColor = Color.FromArgb(26, 188, 156);  // Turkuaz
                        break;
                    case "HATA":
                        satir.DefaultCellStyle.ForeColor = Color.FromArgb(192, 57, 43);   // Koyu kırmızı
                        satir.DefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                        break;
                    default:
                        satir.DefaultCellStyle.ForeColor = Color.White;
                        break;
                }
            }
        }

        // ── DGV AYARLA ───────────────────────────────────────────────────
        private void DgvAyarla()
        {
            dgvLog.EnableHeadersVisualStyles = false;
            dgvLog.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 52, 72);
            dgvLog.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLog.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvLog.DefaultCellStyle.BackColor = Color.FromArgb(27, 42, 59);
            dgvLog.DefaultCellStyle.ForeColor = Color.White;
            dgvLog.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 204, 113);
            dgvLog.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 34, 51);
            dgvLog.RowTemplate.Height = 30;
        }
    }
}
