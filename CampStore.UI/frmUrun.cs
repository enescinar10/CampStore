using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CampStore.BusinessLayer;

namespace CampStore.UI
{
    public partial class frmUrun : Form
    {
        private UrunBL urunBL = new UrunBL();

        private Panel pnlBaslik, pnlAraBar, pnlGrid;
        private TextBox txtAra;
        private Button btnYeniEkle, btnYenile;
        private DataGridView dgvUrun;
        private Label lblToplamUrun;

        public frmUrun()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
{
    UIHelper.FormAyarla(this, "Ürün Yönetimi", 1000, 620);
    this.MaximizeBox     = true;
    this.FormBorderStyle = FormBorderStyle.Sizable;

    // ── BAŞLIK ────────────────────────────────────────────────────
    pnlBaslik = UIHelper.BaslikPaneliOlustur("📦  Ürün Yönetimi", 1000);
    this.Controls.Add(pnlBaslik);

    // ── ARA BAR ───────────────────────────────────────────────────
    pnlAraBar = new Panel
    {
        Size      = new Size(1000, 55),
        Location  = new Point(0, 65),
        BackColor = UIHelper.RenkPanel
    };
    this.Controls.Add(pnlAraBar);

    pnlAraBar.Controls.Add(UIHelper.LblOlustur("🔍", 15, 17, false, 12));

    txtAra = UIHelper.TxtOlustur(45, 13, 300, 30);
    UIHelper.PlaceholderEkle(txtAra, "Ürün adı ara...");
    txtAra.TextChanged += (s, ev) => UrunleriFiltrele();
    pnlAraBar.Controls.Add(txtAra);

    lblToplamUrun = UIHelper.LblOlustur("Toplam: 0 ürün", 370, 18, false, 9);
    lblToplamUrun.ForeColor = UIHelper.RenkGri;
    pnlAraBar.Controls.Add(lblToplamUrun);

    btnYeniEkle = UIHelper.BtnOlustur("➕ Yeni Ürün",
        UIHelper.RenkYesil, 820, 9, 130, 38);
    btnYeniEkle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
    btnYeniEkle.Click += btnYeniEkle_Click;
    pnlAraBar.Controls.Add(btnYeniEkle);

    btnYenile = UIHelper.BtnOlustur("🔄",
        UIHelper.RenkGri, 960, 9, 38, 38);
    btnYenile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
    btnYenile.Click += (s, ev) => UrunleriYukle();
    pnlAraBar.Controls.Add(btnYenile);

    // Ara bar içi butonları sağa hizala
    pnlAraBar.Resize += (s, ev) =>
    {
        btnYenile.Left   = pnlAraBar.Width - 48;
        btnYeniEkle.Left = pnlAraBar.Width - 188;
    };

    // ── GRID ──────────────────────────────────────────────────────
    pnlGrid = new Panel
    {
        Size      = new Size(1000, 490),
        Location  = new Point(0, 120),
        BackColor = UIHelper.RenkArkaplan
    };
    this.Controls.Add(pnlGrid);

    dgvUrun = new DataGridView
    {
        Size                = new Size(1000, 490),
        Location            = new Point(0, 0),
        BackgroundColor     = UIHelper.RenkArkaplan,
        BorderStyle         = BorderStyle.None,
        RowHeadersVisible   = false,
        AllowUserToAddRows  = false,
        ReadOnly            = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        SelectionMode       = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect         = false
    };
    dgvUrun.CellClick += dgvUrun_CellClick;
    pnlGrid.Controls.Add(dgvUrun);

    // ── RESIZE AYARI — form büyüyünce her şey büyüsün ────────────
    UIHelper.FormResizeAyarla(this, pnlBaslik, pnlAraBar, pnlGrid, dgvUrun);

    this.Load += frmUrun_Load;
}

        private void frmUrun_Load(object sender, EventArgs e) => UrunleriYukle();

        // frmUrun.cs — API ile çalışacak şekilde güncelle
        // BL çağrıları yerine ApiServis çağrıları kullanılır

        private void UrunleriYukle()
        {
            try
            {
                // API'den ürünleri getir
                dynamic sonuc = ApiServis.Get<dynamic>("urun");

                // DataTable'a dönüştür
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                dgvUrun.DataSource = dt;
                UIHelper.DgvAyarla(dgvUrun);

                if (!dgvUrun.Columns.Contains("colGuncelle"))
                    UIHelper.DgvIconButonEkle(dgvUrun);

                StokAlarmKontrol();
                lblToplamUrun.Text = $"Toplam: {dt.Rows.Count} ürün";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürünler yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UrunleriFiltrele()
        {
            try
            {
                DataTable dt = urunBL.UrunListele();
                DataView view = dt.DefaultView;

                if (!string.IsNullOrWhiteSpace(txtAra.Text) &&
                    txtAra.Text != "Ürün adı ara...")
                    view.RowFilter = $"UrunAdi LIKE '%{txtAra.Text.Trim()}%'";

                dgvUrun.DataSource = view.ToTable();
                UIHelper.DgvAyarla(dgvUrun);

                if (!dgvUrun.Columns.Contains("colGuncelle"))
                    UIHelper.DgvIconButonEkle(dgvUrun);

                StokAlarmKontrol();
                lblToplamUrun.Text = $"Toplam: {view.Count} ürün";
            }
            catch { }
        }

        private void dgvUrun_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int urunID = Convert.ToInt32(dgvUrun.Rows[e.RowIndex].Cells["UrunID"].Value);

            if (e.ColumnIndex == dgvUrun.Columns["colGuncelle"].Index)
            {
                frmUrunDetay frm = new frmUrunDetay { UrunID = urunID };
                frm.ShowDialog();
                if (frm.Kaydedildi) UrunleriYukle();
            }
            else if (e.ColumnIndex == dgvUrun.Columns["colSil"].Index)
            {
                if (MessageBox.Show("Bu ürünü silmek istediğinize emin misiniz?",
                    "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    try
                    {
                        ApiServis.Delete<dynamic>($"urun/{urunID}");
                        MessageBox.Show("Ürün silindi!",
                            "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UrunleriYukle();
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
            frmUrunDetay frm = new frmUrunDetay { UrunID = 0 };
            frm.ShowDialog();
            if (frm.Kaydedildi) UrunleriYukle();
        }

        private void StokAlarmKontrol()
        {
            foreach (DataGridViewRow satir in dgvUrun.Rows)
            {
                if (satir.Cells["Stok"].Value == null) continue;
                int stok = Convert.ToInt32(satir.Cells["Stok"].Value);

                if (stok <= 5)
                {
                    satir.DefaultCellStyle.BackColor = Color.FromArgb(231, 76, 60);
                    satir.DefaultCellStyle.ForeColor = Color.White;
                }
                else if (stok <= 10)
                {
                    satir.DefaultCellStyle.BackColor = Color.FromArgb(241, 196, 15);
                    satir.DefaultCellStyle.ForeColor = Color.FromArgb(27, 42, 59);
                }
            }
        }
    }
}