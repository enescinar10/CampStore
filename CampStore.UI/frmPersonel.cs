using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CampStore.BusinessLayer;

namespace CampStore.UI
{
    public partial class frmPersonel : Form
    {
        private PersonelBL personelBL = new PersonelBL();

        private Panel pnlBaslik, pnlAraBar, pnlGrid;
        private TextBox txtAra;
        private Button btnYeniEkle, btnYenile;
        private DataGridView dgvPersonel;
        private Label lblToplamPersonel;

        public frmPersonel()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            UIHelper.FormAyarla(this, "Personel Yönetimi", 1050, 620);
            this.MaximizeBox = true;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // ── BAŞLIK ────────────────────────────────────────────────────  
            pnlBaslik = UIHelper.BaslikPaneliOlustur("👨‍💼  Personel Yönetimi", 1050);
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
            UIHelper.PlaceholderEkle(txtAra, "Personel adı ara...");
            txtAra.TextChanged += (s, ev) => PersonelleriFiltrele();
            pnlAraBar.Controls.Add(txtAra);

            lblToplamPersonel = UIHelper.LblOlustur("Toplam: 0 personel", 340, 18, false, 9);
            lblToplamPersonel.ForeColor = UIHelper.RenkGri;
            pnlAraBar.Controls.Add(lblToplamPersonel);

            // Butonların X (Left) lokasyonları 1050 genişliğine göre hesaplandı
            btnYeniEkle = UIHelper.BtnOlustur("➕ Yeni Personel",
                UIHelper.RenkYesil, 847, 9, 145, 38);
            btnYeniEkle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnYeniEkle.Click += btnYeniEkle_Click;
            pnlAraBar.Controls.Add(btnYeniEkle);

            btnYenile = UIHelper.BtnOlustur("🔄",
                UIHelper.RenkGri, 1002, 9, 38, 38);
            btnYenile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnYenile.Click += (s, ev) => PersonelleriYukle();
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

            dgvPersonel = new DataGridView
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
            dgvPersonel.CellClick += dgvPersonel_CellClick;
            pnlGrid.Controls.Add(dgvPersonel);

            // ── RESIZE AYARI — form büyüyünce her şey büyüsün ────────────  
            UIHelper.FormResizeAyarla(this, pnlBaslik, pnlAraBar, pnlGrid, dgvPersonel);

            this.Load += frmPersonel_Load;
        }

        private void frmPersonel_Load(object sender, EventArgs e) => PersonelleriYukle();

        private void PersonelleriYukle()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("personel");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                dgvPersonel.DataSource = dt;
                UIHelper.DgvAyarla(dgvPersonel);

                if (!dgvPersonel.Columns.Contains("colGuncelle"))
                    UIHelper.DgvIconButonEkle(dgvPersonel);

                lblToplamPersonel.Text = $"Toplam: {dt.Rows.Count} personel";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Personeller yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PersonelleriFiltrele()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("personel");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                DataView view = dt.DefaultView;
                if (!string.IsNullOrWhiteSpace(txtAra.Text) &&
                    txtAra.Text != "Personel adı ara...")
                    view.RowFilter = $"PerAd LIKE '%{txtAra.Text.Trim()}%' " +
                                     $"OR PerSoyad LIKE '%{txtAra.Text.Trim()}%'";

                dgvPersonel.DataSource = view.ToTable();
                UIHelper.DgvAyarla(dgvPersonel);

                if (!dgvPersonel.Columns.Contains("colGuncelle"))
                    UIHelper.DgvIconButonEkle(dgvPersonel);

                lblToplamPersonel.Text = $"Toplam: {view.Count} personel";
            }
            catch { }
        }

        private void dgvPersonel_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int perID = Convert.ToInt32(
                dgvPersonel.Rows[e.RowIndex].Cells["PerID"].Value);

            if (e.ColumnIndex == dgvPersonel.Columns["colGuncelle"].Index)
            {
                frmPersonelDetay frm = new frmPersonelDetay { PerID = perID };
                frm.ShowDialog();
                if (frm.Kaydedildi) PersonelleriYukle();
            }
            else if (e.ColumnIndex == dgvPersonel.Columns["colSil"].Index)
            {
                if (MessageBox.Show("Bu personeli silmek istediğinize emin misiniz?",
                    "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    try
                    {
                        ApiServis.Delete<dynamic>($"personel/{perID}");
                        MessageBox.Show("Personel silindi!",
                            "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        PersonelleriYukle();
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
            frmPersonelDetay frm = new frmPersonelDetay { PerID = 0 };
            frm.ShowDialog();
            if (frm.Kaydedildi) PersonelleriYukle();
        }
    }
}