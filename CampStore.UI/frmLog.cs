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

        private Panel pnlBaslik, pnlForm, pnlGrid;
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
            UIHelper.FormAyarla(this, "Log Kayıtları", 1000, 640);
            this.MaximizeBox = true;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // ── BAŞLIK ────────────────────────────────────────────────────
            pnlBaslik = UIHelper.BaslikPaneliOlustur("📋  Log Kayıtları", 1000);
            this.Controls.Add(pnlBaslik);

            // ── FİLTRE PANELİ ─────────────────────────────────────────────
            pnlForm = new Panel
            {
                Size = new Size(1000, 75),
                Location = new Point(0, 65),
                BackColor = UIHelper.RenkPanel
            };
            this.Controls.Add(pnlForm);

            pnlForm.Controls.Add(UIHelper.LblOlustur("İşlem Türü", 20, 12));
            cmbIslemTuru = UIHelper.CmbOlustur(20, 35, 180);
            cmbIslemTuru.Items.AddRange(new object[]
            {
                "Tümü", "GİRİŞ", "EKLE", "GÜNCELLE",
                "SİL", "SATIŞ", "STOK", "HATA"
            });
            cmbIslemTuru.SelectedIndex = 0;
            pnlForm.Controls.Add(cmbIslemTuru);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Açıklamada Ara", 220, 12));
            txtAra = UIHelper.TxtOlustur(220, 35, 280);
            UIHelper.PlaceholderEkle(txtAra, "Anahtar kelime...");
            txtAra.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter) Filtrele();
            };
            pnlForm.Controls.Add(txtAra);

            btnFiltrele = UIHelper.BtnOlustur("🔍 Filtrele",
                UIHelper.RenkMavi, 520, 30, 110, 35);
            btnFiltrele.Click += (s, ev) => Filtrele();
            pnlForm.Controls.Add(btnFiltrele);

            btnTemizle = UIHelper.BtnOlustur("🔄 Temizle",
                UIHelper.RenkGri, 640, 30, 110, 35);
            btnTemizle.Click += (s, ev) =>
            {
                cmbIslemTuru.SelectedIndex = 0;
                txtAra.Text = "Anahtar kelime...";
                txtAra.ForeColor = UIHelper.RenkGri;
                LoglariYukle();
            };
            pnlForm.Controls.Add(btnTemizle);

            btnYenile = UIHelper.BtnOlustur("🔁 Yenile",
                UIHelper.RenkYesil, 760, 30, 100, 35);
            btnYenile.Click += (s, ev) => LoglariYukle();
            pnlForm.Controls.Add(btnYenile);

            lblToplamKayit = UIHelper.LblOlustur("Toplam: 0 kayıt", 880, 38, false, 9);
            lblToplamKayit.ForeColor = UIHelper.RenkGri;
            pnlForm.Controls.Add(lblToplamKayit);

            // ── GRID ──────────────────────────────────────────────────────
            pnlGrid = new Panel
            {
                Size = new Size(1000, 490),
                Location = new Point(0, 140),
                BackColor = UIHelper.RenkArkaplan
            };
            this.Controls.Add(pnlGrid);

            dgvLog = new DataGridView
            {
                Size = new Size(1000, 490),
                Location = new Point(0, 0),
                BackgroundColor = UIHelper.RenkArkaplan,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            pnlGrid.Controls.Add(dgvLog);

            UIHelper.FormResizeAyarla(this, pnlBaslik, pnlForm, pnlGrid, dgvLog);
            this.Load += frmLog_Load;
        }

        private void frmLog_Load(object sender, EventArgs e)
        {
            UIHelper.DgvAyarla(dgvLog);
            LoglariYukle();
        }

        private void LoglariYukle()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("log");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                dgvLog.DataSource = dt;
                UIHelper.DgvAyarla(dgvLog);
                lblToplamKayit.Text = $"Toplam: {dt.Rows.Count} kayıt";
                RenkleriAyarla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Log kayıtları yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Filtrele()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("log");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                DataView view = dt.DefaultView;
                string filtre = "";

                if (cmbIslemTuru.SelectedIndex > 0)
                    filtre += $"IslemTuru = '{cmbIslemTuru.SelectedItem}'";

                if (!string.IsNullOrWhiteSpace(txtAra.Text) &&
                    txtAra.Text != "Anahtar kelime...")
                {
                    if (!string.IsNullOrEmpty(filtre)) filtre += " AND ";
                    filtre += $"Aciklama LIKE '%{txtAra.Text.Trim()}%'";
                }

                view.RowFilter = filtre;
                dgvLog.DataSource = view.ToTable();
                UIHelper.DgvAyarla(dgvLog);
                lblToplamKayit.Text = $"Toplam: {view.Count} kayıt";
                RenkleriAyarla();
            }
            catch { }
        }

        private void RenkleriAyarla()
        {
            foreach (DataGridViewRow satir in dgvLog.Rows)
            {
                if (satir.Cells["IslemTuru"].Value == null) continue;
                switch (satir.Cells["IslemTuru"].Value.ToString())
                {
                    case "GİRİŞ":
                        satir.DefaultCellStyle.ForeColor = Color.FromArgb(46, 204, 113);
                        break;
                    case "EKLE":
                        satir.DefaultCellStyle.ForeColor = Color.FromArgb(52, 152, 219);
                        break;
                    case "GÜNCELLE":
                        satir.DefaultCellStyle.ForeColor = Color.FromArgb(230, 126, 34);
                        break;
                    case "SİL":
                        satir.DefaultCellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                        break;
                    case "SATIŞ":
                        satir.DefaultCellStyle.ForeColor = Color.FromArgb(155, 89, 182);
                        break;
                    case "STOK":
                        satir.DefaultCellStyle.ForeColor = Color.FromArgb(26, 188, 156);
                        break;
                    case "HATA":
                        satir.DefaultCellStyle.ForeColor = Color.FromArgb(192, 57, 43);
                        satir.DefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                        break;
                    default:
                        satir.DefaultCellStyle.ForeColor = Color.White;
                        break;
                }
            }
        }
    }
}