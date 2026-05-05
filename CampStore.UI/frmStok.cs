using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CampStore.BusinessLayer;
using CampStore.Entities;

namespace CampStore.UI
{
    public partial class frmStok : Form
    {
        private StokBL stokBL = new StokBL();
        private UrunBL urunBL = new UrunBL();

        private Panel pnlBaslik, pnlForm, pnlGrid;
        private ComboBox cmbUrun;
        private TextBox txtGiris, txtCikis, txtMevcutStok;
        private DateTimePicker dtpTarih;
        private Button btnEkle, btnTemizle, btnTumu;
        private DataGridView dgvStok;
        private Label lblToplamGiris, lblToplamCikis;

        public frmStok()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            UIHelper.FormAyarla(this, "Stok Takibi", 1000, 640);
            this.MaximizeBox = true;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // ── BAŞLIK ────────────────────────────────────────────────────
            pnlBaslik = UIHelper.BaslikPaneliOlustur("📊  Stok Takibi", 1000);
            this.Controls.Add(pnlBaslik);

            // ── FORM PANELİ ───────────────────────────────────────────────
            pnlForm = new Panel
            {
                Size = new Size(1000, 130),
                Location = new Point(0, 65),
                BackColor = UIHelper.RenkPanel
            };
            this.Controls.Add(pnlForm);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Ürün", 20, 12));
            cmbUrun = UIHelper.CmbOlustur(20, 35, 260);
            cmbUrun.SelectedIndexChanged += (s, ev) =>
            {
                if (cmbUrun.SelectedItem is ComboItem item && item.ID > 0)
                {
                    Urun u = urunBL.UrunGetirById(item.ID);
                    if (u != null)
                        txtMevcutStok.Text = u.Stok.ToString();
                    StokHareketleriYukle(item.ID);
                }
                else
                {
                    txtMevcutStok.Clear();
                    StokHareketleriYukle(0);
                }
            };
            pnlForm.Controls.Add(cmbUrun);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Mevcut Stok", 300, 12));
            txtMevcutStok = UIHelper.TxtOlustur(300, 35, 100);
            txtMevcutStok.ReadOnly = true;
            txtMevcutStok.BackColor = UIHelper.RenkMenuKoyu;
            txtMevcutStok.ForeColor = UIHelper.RenkYesil;
            pnlForm.Controls.Add(txtMevcutStok);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Giriş Miktarı", 420, 12));
            txtGiris = UIHelper.TxtOlustur(420, 35, 120);
            txtGiris.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) && ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };
            txtGiris.TextChanged += (s, ev) =>
            {
                if (!string.IsNullOrEmpty(txtGiris.Text))
                    txtCikis.Clear();
            };
            pnlForm.Controls.Add(txtGiris);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Çıkış Miktarı", 560, 12));
            txtCikis = UIHelper.TxtOlustur(560, 35, 120);
            txtCikis.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) && ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };
            txtCikis.TextChanged += (s, ev) =>
            {
                if (!string.IsNullOrEmpty(txtCikis.Text))
                    txtGiris.Clear();
            };
            pnlForm.Controls.Add(txtCikis);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Tarih", 700, 12));
            dtpTarih = new DateTimePicker
            {
                Size = new Size(160, 32),
                Location = new Point(700, 35),
                Font = new Font("Segoe UI", 10f),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };
            pnlForm.Controls.Add(dtpTarih);

            // Butonlar
            btnEkle = UIHelper.BtnOlustur("➕ Hareket Ekle",
                UIHelper.RenkYesil, 20, 85, 140, 35);
            btnEkle.Click += btnEkle_Click;
            pnlForm.Controls.Add(btnEkle);

            btnTemizle = UIHelper.BtnOlustur("🔄 Temizle",
                UIHelper.RenkGri, 170, 85, 110, 35);
            btnTemizle.Click += (s, ev) => Temizle();
            pnlForm.Controls.Add(btnTemizle);

            btnTumu = UIHelper.BtnOlustur("📋 Tüm Hareketler",
                UIHelper.RenkMenuKoyu, 290, 85, 150, 35);
            btnTumu.Click += (s, ev) =>
            {
                cmbUrun.SelectedIndex = 0;
                StokHareketleriYukle(0);
            };
            pnlForm.Controls.Add(btnTumu);

            // Özet
            lblToplamGiris = UIHelper.LblOlustur("Toplam Giriş: 0", 500, 92, false, 9);
            lblToplamGiris.ForeColor = UIHelper.RenkYesil;
            pnlForm.Controls.Add(lblToplamGiris);

            lblToplamCikis = UIHelper.LblOlustur("Toplam Çıkış: 0", 680, 92, false, 9);
            lblToplamCikis.ForeColor = UIHelper.RenkKirmizi;
            pnlForm.Controls.Add(lblToplamCikis);

            // ── GRID ──────────────────────────────────────────────────────
            pnlGrid = new Panel
            {
                Size = new Size(1000, 430),
                Location = new Point(0, 195),
                BackColor = UIHelper.RenkArkaplan
            };
            this.Controls.Add(pnlGrid);

            dgvStok = new DataGridView
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
            pnlGrid.Controls.Add(dgvStok);

            UIHelper.FormResizeAyarla(this, pnlBaslik, pnlForm, pnlGrid, dgvStok);
            this.Load += frmStok_Load;
        }

        private void frmStok_Load(object sender, EventArgs e)
        {
            UIHelper.DgvAyarla(dgvStok);
            UrunComboYukle();
            StokHareketleriYukle(0);
        }

        private void UrunComboYukle()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("urun");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                cmbUrun.Items.Clear();
                cmbUrun.Items.Add(new ComboItem { ID = 0, Ad = "— Tüm Ürünler —" });
                foreach (DataRow row in dt.Rows)
                    cmbUrun.Items.Add(new ComboItem
                    {
                        ID = Convert.ToInt32(row["UrunID"]),
                        Ad = row["UrunAdi"].ToString()
                    });

                cmbUrun.DisplayMember = "Ad";
                cmbUrun.ValueMember = "ID";
                cmbUrun.SelectedIndex = 0;
            }
            catch { }
        }

        private void StokHareketleriYukle(int urunID)
        {
            try
            {
                string endpoint = urunID == 0 ? "stok" : $"stok/urun/{urunID}";
                dynamic sonuc = ApiServis.Get<dynamic>(endpoint);
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                dgvStok.DataSource = dt;
                UIHelper.DgvAyarla(dgvStok);

                int toplamGiris = 0, toplamCikis = 0;
                foreach (DataRow row in dt.Rows)
                {
                    toplamGiris += Convert.ToInt32(row["GirisMiktar"]);
                    toplamCikis += Convert.ToInt32(row["CikisMiktar"]);
                }

                lblToplamGiris.Text = $"Toplam Giriş: {toplamGiris}";
                lblToplamCikis.Text = $"Toplam Çıkış: {toplamCikis}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Stok hareketleri yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (!(cmbUrun.SelectedItem is ComboItem item) || item.ID == 0)
            {
                MessageBox.Show("Lütfen bir ürün seçiniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int giris = 0, cikis = 0;

            if (!string.IsNullOrEmpty(txtGiris.Text))
            {
                if (!int.TryParse(txtGiris.Text, out giris) || giris <= 0)
                {
                    MessageBox.Show("Geçerli bir giriş miktarı giriniz!",
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else if (!string.IsNullOrEmpty(txtCikis.Text))
            {
                if (!int.TryParse(txtCikis.Text, out cikis) || cikis <= 0)
                {
                    MessageBox.Show("Geçerli bir çıkış miktarı giriniz!",
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Lütfen giriş veya çıkış miktarı giriniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var data = new
                {
                    urunID = item.ID,
                    girisMiktar = giris,
                    cikisMiktar = cikis,
                    tarih = dtpTarih.Value
                };

                ApiServis.Post<dynamic>("stok", data);
                MessageBox.Show("Stok hareketi kaydedildi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Temizle();
                StokHareketleriYukle(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Temizle()
        {
            cmbUrun.SelectedIndex = 0;
            txtGiris.Clear();
            txtCikis.Clear();
            txtMevcutStok.Clear();
            dtpTarih.Value = DateTime.Now;
        }
    }
}