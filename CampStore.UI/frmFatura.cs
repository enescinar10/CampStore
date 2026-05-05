using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using CampStore.BusinessLayer;
using CampStore.Entities;

namespace CampStore.UI
{
    public partial class frmFatura : Form
    {
        private FaturaBL faturaBL = new FaturaBL();
        private SatisBL satisBL = new SatisBL();

        private int seciliFaturaID = 0;

        private Panel pnlBaslik, pnlForm, pnlGrid;
        private ComboBox cmbSatis;
        private TextBox txtFaturaNo, txtTutar;
        private DateTimePicker dtpTarih;
        private Button btnEkle, btnGuncelle, btnSil, btnTemizle, btnYazdir;
        private DataGridView dgvFatura;
        private Label lblToplamFatura;

        public frmFatura()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            UIHelper.FormAyarla(this, "Fatura Yönetimi", 1000, 640);
            this.MaximizeBox = true;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // ── BAŞLIK ────────────────────────────────────────────────────
            pnlBaslik = UIHelper.BaslikPaneliOlustur("🧾  Fatura Yönetimi", 1000);
            this.Controls.Add(pnlBaslik);

            // ── FORM PANELİ ───────────────────────────────────────────────
            pnlForm = new Panel
            {
                Size = new Size(1000, 130),
                Location = new Point(0, 65),
                BackColor = UIHelper.RenkPanel
            };
            this.Controls.Add(pnlForm);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Satış", 20, 12));
            cmbSatis = UIHelper.CmbOlustur(20, 35, 260);
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

            pnlForm.Controls.Add(UIHelper.LblOlustur("Fatura No", 300, 12));
            txtFaturaNo = UIHelper.TxtOlustur(300, 35, 200);
            pnlForm.Controls.Add(txtFaturaNo);

            Button btnOtomatik = UIHelper.BtnOlustur("🔁 Otomatik",
                UIHelper.RenkMenuKoyu, 510, 35, 110, 32);
            btnOtomatik.Click += (s, ev) =>
                txtFaturaNo.Text = faturaBL.FaturaNoUret();
            pnlForm.Controls.Add(btnOtomatik);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Tarih", 640, 12));
            dtpTarih = new DateTimePicker
            {
                Size = new Size(160, 32),
                Location = new Point(640, 35),
                Font = new Font("Segoe UI", 10f),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };
            pnlForm.Controls.Add(dtpTarih);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Tutar (₺)", 820, 12));
            txtTutar = UIHelper.TxtOlustur(820, 35, 150);
            txtTutar.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) &&
                    ev.KeyChar != ',' &&
                    ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };
            pnlForm.Controls.Add(txtTutar);

            // Butonlar
            btnEkle = UIHelper.BtnOlustur("➕ Ekle",
                UIHelper.RenkYesil, 20, 82, 100, 35);
            btnEkle.Click += btnEkle_Click;
            pnlForm.Controls.Add(btnEkle);

            btnGuncelle = UIHelper.BtnOlustur("✏️ Güncelle",
                UIHelper.RenkMavi, 130, 82, 110, 35);
            btnGuncelle.Enabled = false;
            btnGuncelle.Click += btnGuncelle_Click;
            pnlForm.Controls.Add(btnGuncelle);

            btnSil = UIHelper.BtnOlustur("🗑️ Sil",
                UIHelper.RenkKirmizi, 250, 82, 90, 35);
            btnSil.Enabled = false;
            btnSil.Click += btnSil_Click;
            pnlForm.Controls.Add(btnSil);

            btnYazdir = UIHelper.BtnOlustur("🖨️ Yazdır",
                UIHelper.RenkMenuKoyu, 350, 82, 110, 35);
            btnYazdir.Enabled = false;
            btnYazdir.Click += btnYazdir_Click;
            pnlForm.Controls.Add(btnYazdir);

            btnTemizle = UIHelper.BtnOlustur("🔄 Temizle",
                UIHelper.RenkGri, 470, 82, 110, 35);
            btnTemizle.Click += (s, ev) => Temizle();
            pnlForm.Controls.Add(btnTemizle);

            lblToplamFatura = UIHelper.LblOlustur("Toplam: 0 fatura", 700, 90, false, 9);
            lblToplamFatura.ForeColor = UIHelper.RenkGri;
            pnlForm.Controls.Add(lblToplamFatura);

            // ── GRID ──────────────────────────────────────────────────────
            pnlGrid = new Panel
            {
                Size = new Size(1000, 430),
                Location = new Point(0, 195),
                BackColor = UIHelper.RenkArkaplan
            };
            this.Controls.Add(pnlGrid);

            dgvFatura = new DataGridView
            {
                Size = new Size(1000, 430),
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
            dgvFatura.CellClick += dgvFatura_CellClick;
            pnlGrid.Controls.Add(dgvFatura);

            // ── RESIZE ────────────────────────────────────────────────────
            UIHelper.FormResizeAyarla(this, pnlBaslik, pnlForm, pnlGrid, dgvFatura);

            this.Load += frmFatura_Load;
        }

        private void frmFatura_Load(object sender, EventArgs e)
        {
            UIHelper.DgvAyarla(dgvFatura);
            SatisComboYukle();
            FaturalariYukle();
        }

        private void SatisComboYukle()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("satis");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                cmbSatis.Items.Clear();
                cmbSatis.Items.Add(new ComboItem { ID = 0, Ad = "— Satış Seçin —" });
                foreach (DataRow row in dt.Rows)
                    cmbSatis.Items.Add(new ComboItem
                    {
                        ID = Convert.ToInt32(row["SatisID"]),
                        Ad = $"Satış #{row["SatisID"]} — " +
                             $"{row["MusteriAdSoyad"]} — " +
                             $"{Convert.ToDecimal(row["ToplamTutar"]):F2} ₺"
                    });

                cmbSatis.DisplayMember = "Ad";
                cmbSatis.ValueMember = "ID";
                cmbSatis.SelectedIndex = 0;
            }
            catch { }
        }

        private void FaturalariYukle()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("fatura");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                dgvFatura.DataSource = dt;
                UIHelper.DgvAyarla(dgvFatura);
                lblToplamFatura.Text = $"Toplam: {dt.Rows.Count} fatura";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Faturalar yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvFatura_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            try
            {
                DataGridViewRow satir = dgvFatura.Rows[e.RowIndex];
                seciliFaturaID = Convert.ToInt32(satir.Cells["FaturaID"].Value);
                txtFaturaNo.Text = satir.Cells["FaturaNo"].Value?.ToString() ?? "";
                dtpTarih.Value = Convert.ToDateTime(satir.Cells["Tarih"].Value);
                txtTutar.Text = satir.Cells["Tutar"].Value?.ToString() ?? "";

                int satisID = Convert.ToInt32(satir.Cells["SatisID"].Value);
                for (int i = 0; i < cmbSatis.Items.Count; i++)
                    if (((ComboItem)cmbSatis.Items[i]).ID == satisID)
                    { cmbSatis.SelectedIndex = i; break; }

                btnGuncelle.Enabled = true;
                btnSil.Enabled = true;
                btnYazdir.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtTutar.Text, out decimal tutar))
            {
                MessageBox.Show("Geçerli bir tutar giriniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var data = new
                {
                    satisID = cmbSatis.SelectedItem is ComboItem cs ? cs.ID : 0,
                    faturaNo = txtFaturaNo.Text.Trim(),
                    tarih = dtpTarih.Value,
                    tutar = tutar
                };

                ApiServis.Post<dynamic>("fatura", data);
                MessageBox.Show("Fatura eklendi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Temizle();
                FaturalariYukle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliFaturaID == 0) return;
            if (!decimal.TryParse(txtTutar.Text, out decimal tutar))
            {
                MessageBox.Show("Geçerli bir tutar giriniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var data = new
                {
                    satisID = cmbSatis.SelectedItem is ComboItem cs ? cs.ID : 0,
                    faturaNo = txtFaturaNo.Text.Trim(),
                    tarih = dtpTarih.Value,
                    tutar = tutar
                };

                ApiServis.Put<dynamic>($"fatura/{seciliFaturaID}", data);
                MessageBox.Show("Fatura güncellendi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Temizle();
                FaturalariYukle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (seciliFaturaID == 0) return;
            if (MessageBox.Show("Bu faturayı silmek istediğinize emin misiniz?",
                "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                try
                {
                    ApiServis.Delete<dynamic>($"fatura/{seciliFaturaID}");
                    MessageBox.Show("Fatura silindi!",
                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Temizle();
                    FaturalariYukle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message,
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnYazdir_Click(object sender, EventArgs e)
        {
            if (seciliFaturaID == 0) return;
            Fatura fatura = faturaBL.FaturaGetirById(seciliFaturaID);
            if (fatura == null) return;

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, ev) =>
            {
                Graphics g = ev.Graphics;
                Font fBold = new Font("Segoe UI", 14f, FontStyle.Bold);
                Font fNormal = new Font("Segoe UI", 10f);
                Font fKalin = new Font("Segoe UI", 10f, FontStyle.Bold);
                Font fKucuk = new Font("Segoe UI", 8f);
                Brush siyah = Brushes.Black;
                Brush gri = new SolidBrush(Color.Gray);
                int y = 40, sol = 60;

                g.DrawString("🏕️ CampStore", fBold, siyah, sol, y); y += 30;
                g.DrawString("Kamp Malzemeleri Yönetim Sistemi", fNormal, gri, sol, y); y += 20;
                g.DrawString("Tel: 0232 123 45 67 | info@campstore.com", fKucuk, gri, sol, y); y += 25;
                g.DrawLine(Pens.Black, sol, y, 540, y); y += 15;

                g.DrawString("FATURA", new Font("Segoe UI", 13f, FontStyle.Bold), siyah, sol, y); y += 28;
                g.DrawString("Fatura No :", fKalin, siyah, sol, y);
                g.DrawString(fatura.FaturaNo, fNormal, siyah, sol + 110, y); y += 22;
                g.DrawString("Tarih      :", fKalin, siyah, sol, y);
                g.DrawString(fatura.Tarih.ToString("dd.MM.yyyy"), fNormal, siyah, sol + 110, y); y += 22;
                g.DrawString("Satış ID :", fKalin, siyah, sol, y);
                g.DrawString(fatura.SatisID.ToString(), fNormal, siyah, sol + 110, y); y += 30;

                g.DrawLine(Pens.Black, sol, y, 540, y); y += 15;
                g.DrawString("Toplam Tutar :", fKalin, siyah, sol, y);
                g.DrawString($"{fatura.Tutar:F2} ₺",
                    new Font("Segoe UI", 12f, FontStyle.Bold), siyah, sol + 130, y); y += 35;
                g.DrawLine(Pens.Black, sol, y, 540, y); y += 15;
                g.DrawString("Bizi tercih ettiğiniz için teşekkür ederiz!", fKucuk, gri, sol, y); y += 15;
                g.DrawString($"Yazdırma: {DateTime.Now:dd.MM.yyyy HH:mm}", fKucuk, gri, sol, y);
            };

            PrintPreviewDialog preview = new PrintPreviewDialog
            {
                Document = pd,
                Width = 800,
                Height = 600
            };
            preview.ShowDialog();
        }

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
    }
}