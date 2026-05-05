using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CampStore.BusinessLayer;

namespace CampStore.UI
{
    public partial class frmKategori : Form
    {
        private KategoriBL kategoriBL = new KategoriBL();

        private Panel pnlBaslik, pnlAraBar, pnlGrid;
        private TextBox txtAra;
        private Button btnYeniEkle, btnYenile;
        private DataGridView dgvKategori;
        private Label lblToplamKategori;

        public frmKategori()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            UIHelper.FormAyarla(this, "Kategori Yönetimi", 860, 580);
            this.MaximizeBox = true;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // ── BAŞLIK ────────────────────────────────────────────────────  
            pnlBaslik = UIHelper.BaslikPaneliOlustur("🗂️  Kategori Yönetimi", 860);
            this.Controls.Add(pnlBaslik);

            // ── ARA BAR ───────────────────────────────────────────────────  
            pnlAraBar = new Panel
            {
                Size = new Size(860, 55),
                Location = new Point(0, 65),
                BackColor = UIHelper.RenkPanel
            };
            this.Controls.Add(pnlAraBar);

            pnlAraBar.Controls.Add(UIHelper.LblOlustur("🔍", 15, 17, false, 12));

            txtAra = UIHelper.TxtOlustur(45, 13, 280, 30);
            UIHelper.PlaceholderEkle(txtAra, "Kategori adı ara...");
            txtAra.TextChanged += (s, ev) => KategorileriFiltrele();
            pnlAraBar.Controls.Add(txtAra);

            lblToplamKategori = UIHelper.LblOlustur("Toplam: 0 kategori", 340, 18, false, 9);
            lblToplamKategori.ForeColor = UIHelper.RenkGri;
            pnlAraBar.Controls.Add(lblToplamKategori);

            // Butonların X (Left) lokasyonları 860 genişliğine göre hesaplandı
            btnYeniEkle = UIHelper.BtnOlustur("➕ Yeni Kategori",
                UIHelper.RenkYesil, 657, 9, 145, 38);
            btnYeniEkle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnYeniEkle.Click += btnYeniEkle_Click;
            pnlAraBar.Controls.Add(btnYeniEkle);

            btnYenile = UIHelper.BtnOlustur("🔄",
                UIHelper.RenkGri, 812, 9, 38, 38);
            btnYenile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnYenile.Click += (s, ev) => KategorileriYukle();
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
                Size = new Size(860, 460), // Form yüksekliği 580 - 120 (üst boşluk) = 460
                Location = new Point(0, 120),
                BackColor = UIHelper.RenkArkaplan
            };
            this.Controls.Add(pnlGrid);

            dgvKategori = new DataGridView
            {
                Size = new Size(860, 460),
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
            dgvKategori.CellClick += dgvKategori_CellClick;
            pnlGrid.Controls.Add(dgvKategori);

            // ── RESIZE AYARI — form büyüyünce her şey büyüsün ────────────  
            UIHelper.FormResizeAyarla(this, pnlBaslik, pnlAraBar, pnlGrid, dgvKategori);

            this.Load += frmKategori_Load;
        }

        private void frmKategori_Load(object sender, EventArgs e) => KategorileriYukle();

        private void KategorileriYukle()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("kategori");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                dgvKategori.DataSource = dt;
                UIHelper.DgvAyarla(dgvKategori);

                if (!dgvKategori.Columns.Contains("colGuncelle"))
                    UIHelper.DgvIconButonEkle(dgvKategori);

                lblToplamKategori.Text = $"Toplam: {dt.Rows.Count} kategori";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kategoriler yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void KategorileriFiltrele()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("kategori");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                DataView view = dt.DefaultView;
                if (!string.IsNullOrWhiteSpace(txtAra.Text) &&
                    txtAra.Text != "Kategori adı ara...")
                    view.RowFilter = $"KatAdi LIKE '%{txtAra.Text.Trim()}%'";

                dgvKategori.DataSource = view.ToTable();
                UIHelper.DgvAyarla(dgvKategori);

                if (!dgvKategori.Columns.Contains("colGuncelle"))
                    UIHelper.DgvIconButonEkle(dgvKategori);

                lblToplamKategori.Text = $"Toplam: {view.Count} kategori";
            }
            catch { }
        }

        private void dgvKategori_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int katID = Convert.ToInt32(
                dgvKategori.Rows[e.RowIndex].Cells["KatID"].Value);

            if (e.ColumnIndex == dgvKategori.Columns["colGuncelle"].Index)
            {
                frmKategoriDetay frm = new frmKategoriDetay { KatID = katID };
                frm.ShowDialog();
                if (frm.Kaydedildi) KategorileriYukle();
            }
            else if (e.ColumnIndex == dgvKategori.Columns["colSil"].Index)
            {
                if (MessageBox.Show("Bu kategoriyi silmek istediğinize emin misiniz?",
                    "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    try
                    {
                        ApiServis.Delete<dynamic>($"kategori/{katID}");
                        MessageBox.Show("Kategori silindi!",
                            "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        KategorileriYukle();
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
            frmKategoriDetay frm = new frmKategoriDetay { KatID = 0 };
            frm.ShowDialog();
            if (frm.Kaydedildi) KategorileriYukle();
        }
    }
}