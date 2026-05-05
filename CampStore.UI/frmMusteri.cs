using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CampStore.BusinessLayer;

namespace CampStore.UI
{
    public partial class frmMusteri : Form
    {
        private MusteriBL musteriBL = new MusteriBL();

        private Panel pnlBaslik, pnlAraBar, pnlGrid;
        private TextBox txtAra;
        private Button btnYeniEkle, btnYenile;
        private DataGridView dgvMusteri;
        private Label lblToplamMusteri;

        public frmMusteri()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            UIHelper.FormAyarla(this, "Müşteri Yönetimi", 1050, 620);
            this.MaximizeBox = true;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // ── BAŞLIK ────────────────────────────────────────────────────  
            pnlBaslik = UIHelper.BaslikPaneliOlustur("👤  Müşteri Yönetimi", 1050);
            this.Controls.Add(pnlBaslik);

            // ── ARA BAR ───────────────────────────────────────────────────  
            pnlAraBar = new Panel
            {
                Size = new Size(1050, 55),
                Location = new Point(0, 65),
                BackColor = UIHelper.RenkPanel
            };
            this.Controls.Add(pnlAraBar);

            pnlAraBar.Controls.Add(UIHelper.LblOlustur("🔍", 15, 17, false, 12));

            txtAra = UIHelper.TxtOlustur(45, 13, 280, 30);
            UIHelper.PlaceholderEkle(txtAra, "Müşteri adı ara...");
            txtAra.TextChanged += (s, ev) => MusterileriFiltrele();
            pnlAraBar.Controls.Add(txtAra);

            lblToplamMusteri = UIHelper.LblOlustur("Toplam: 0 müşteri", 340, 18, false, 9);
            lblToplamMusteri.ForeColor = UIHelper.RenkGri;
            pnlAraBar.Controls.Add(lblToplamMusteri);

            // Butonların X (Left) lokasyonları 1050 genişliğine göre hesaplandı
            btnYeniEkle = UIHelper.BtnOlustur("➕ Yeni Müşteri",
                UIHelper.RenkYesil, 847, 9, 145, 38);
            btnYeniEkle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnYeniEkle.Click += btnYeniEkle_Click;
            pnlAraBar.Controls.Add(btnYeniEkle);

            btnYenile = UIHelper.BtnOlustur("🔄",
                UIHelper.RenkGri, 1002, 9, 38, 38);
            btnYenile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnYenile.Click += (s, ev) => MusterileriYukle();
            pnlAraBar.Controls.Add(btnYenile);

            // Ara bar içi butonları sağa hizala  
            pnlAraBar.Resize += (s, ev) =>
            {
                btnYenile.Left = pnlAraBar.Width - 48;
                btnYeniEkle.Left = pnlAraBar.Width - 203;
            };

            // ── GRID ──────────────────────────────────────────────────────  
            pnlGrid = new Panel
            {
                Size = new Size(1050, 500), // Form yüksekliği 620 - 120 (üst boşluk) = 500
                Location = new Point(0, 120),
                BackColor = UIHelper.RenkArkaplan
            };
            this.Controls.Add(pnlGrid);

            dgvMusteri = new DataGridView
            {
                Size = new Size(1050, 500),
                Location = new Point(0, 0),
                BackgroundColor = UIHelper.RenkArkaplan,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dgvMusteri.CellClick += dgvMusteri_CellClick;
            pnlGrid.Controls.Add(dgvMusteri);

            // ── RESIZE AYARI — form büyüyünce her şey büyüsün ────────────  
            UIHelper.FormResizeAyarla(this, pnlBaslik, pnlAraBar, pnlGrid, dgvMusteri);

            this.Load += frmMusteri_Load;
        }

        private void frmMusteri_Load(object sender, EventArgs e) => MusterileriYukle();

        private void MusterileriYukle()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("musteri");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                dgvMusteri.DataSource = dt;
                UIHelper.DgvAyarla(dgvMusteri);

                if (!dgvMusteri.Columns.Contains("colGuncelle"))
                    UIHelper.DgvIconButonEkle(dgvMusteri);

                lblToplamMusteri.Text = $"Toplam: {dt.Rows.Count} müşteri";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteriler yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MusterileriFiltrele()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("musteri");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                DataView view = dt.DefaultView;
                if (!string.IsNullOrWhiteSpace(txtAra.Text) &&
                    txtAra.Text != "Müşteri adı ara...")
                    view.RowFilter = $"Ad LIKE '%{txtAra.Text.Trim()}%' " +
                                     $"OR Soyad LIKE '%{txtAra.Text.Trim()}%'";

                dgvMusteri.DataSource = view.ToTable();
                UIHelper.DgvAyarla(dgvMusteri);

                if (!dgvMusteri.Columns.Contains("colGuncelle"))
                    UIHelper.DgvIconButonEkle(dgvMusteri);

                lblToplamMusteri.Text = $"Toplam: {view.Count} müşteri";
            }
            catch { }
        }

        private void dgvMusteri_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int musteriID = Convert.ToInt32(
                dgvMusteri.Rows[e.RowIndex].Cells["MusteriID"].Value);

            if (e.ColumnIndex == dgvMusteri.Columns["colGuncelle"].Index)
            {
                frmMusteriDetay frm = new frmMusteriDetay { MusteriID = musteriID };
                frm.ShowDialog();
                if (frm.Kaydedildi) MusterileriYukle();
            }
            else if (e.ColumnIndex == dgvMusteri.Columns["colSil"].Index)
            {
                if (MessageBox.Show("Bu müşteriyi silmek istediğinize emin misiniz?",
                    "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    try
                    {
                        ApiServis.Delete<dynamic>($"musteri/{musteriID}");
                        MessageBox.Show("Müşteri silindi!",
                            "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MusterileriYukle();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata: " + ex.Message,
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnYeniEkle_Click(object sender, EventArgs e)
        {
            frmMusteriDetay frm = new frmMusteriDetay { MusteriID = 0 };
            frm.ShowDialog();
            if (frm.Kaydedildi) MusterileriYukle();
        }
    }
}