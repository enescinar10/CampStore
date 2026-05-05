using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CampStore.BusinessLayer;
using CampStore.Entities;

namespace CampStore.UI
{
    public partial class frmSatis : Form
    {
        private SatisBL satisBL = new SatisBL();
        private MusteriBL musteriBL = new MusteriBL();
        private UrunBL urunBL = new UrunBL();

        private int seciliSatisID = 0;
        private int seciliDetayID = 0;

        private Panel pnlBaslik, pnlForm, pnlSatisGrid, pnlDetay;
        private ComboBox cmbMusteri, cmbDurum, cmbUrun;
        private TextBox txtToplamTutar, txtMiktar, txtBirimFiyat;
        private Button btnSatisEkle, btnSatisGuncelle, btnSatisSil;
        private Button btnDetayEkle, btnDetaySil, btnTemizle;
        private DataGridView dgvSatis, dgvDetay;
        private Label lblToplamCiro;

        public frmSatis()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            UIHelper.FormAyarla(this, "Satış Yönetimi", 1100, 780);
            this.MaximizeBox = true;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // ── BAŞLIK ────────────────────────────────────────────────────
            pnlBaslik = UIHelper.BaslikPaneliOlustur("🛒  Satış Yönetimi", 1100);
            this.Controls.Add(pnlBaslik);

            // ── SATIŞ FORM PANELİ ─────────────────────────────────────────
            pnlForm = new Panel
            {
                Size = new Size(1100, 110),
                Location = new Point(0, 65),
                BackColor = UIHelper.RenkPanel
            };
            this.Controls.Add(pnlForm);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Müşteri", 20, 12));
            cmbMusteri = UIHelper.CmbOlustur(20, 35, 250);
            pnlForm.Controls.Add(cmbMusteri);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Durum", 290, 12));
            cmbDurum = UIHelper.CmbOlustur(290, 35, 160);
            cmbDurum.Items.AddRange(new object[]
            { "Hazırlanıyor", "Kargoda", "Teslim Edildi", "İptal" });
            cmbDurum.SelectedIndex = 0;
            pnlForm.Controls.Add(cmbDurum);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Toplam Tutar (₺)", 470, 12));
            txtToplamTutar = UIHelper.TxtOlustur(470, 35, 140);
            txtToplamTutar.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) &&
                    ev.KeyChar != ',' &&
                    ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };
            pnlForm.Controls.Add(txtToplamTutar);

            btnSatisEkle = UIHelper.BtnOlustur("➕ Yeni Satış",
                UIHelper.RenkYesil, 630, 35, 120, 38);
            btnSatisEkle.Click += btnSatisEkle_Click;
            pnlForm.Controls.Add(btnSatisEkle);

            btnSatisGuncelle = UIHelper.BtnOlustur("✏️ Güncelle",
                UIHelper.RenkMavi, 760, 35, 110, 38);
            btnSatisGuncelle.Enabled = false;
            btnSatisGuncelle.Click += btnSatisGuncelle_Click;
            pnlForm.Controls.Add(btnSatisGuncelle);

            btnSatisSil = UIHelper.BtnOlustur("🗑️ Sil",
                UIHelper.RenkKirmizi, 880, 35, 90, 38);
            btnSatisSil.Enabled = false;
            btnSatisSil.Click += btnSatisSil_Click;
            pnlForm.Controls.Add(btnSatisSil);

            btnTemizle = UIHelper.BtnOlustur("🔄",
                UIHelper.RenkGri, 980, 35, 80, 38);
            btnTemizle.Click += (s, ev) => Temizle();
            pnlForm.Controls.Add(btnTemizle);

            lblToplamCiro = UIHelper.LblOlustur("Toplam Ciro: 0,00 ₺", 20, 78, false, 9);
            lblToplamCiro.ForeColor = UIHelper.RenkYesil;
            pnlForm.Controls.Add(lblToplamCiro);

            // ── SATIŞ GRİD ────────────────────────────────────────────────
            pnlSatisGrid = new Panel
            {
                Size = new Size(1100, 230),
                Location = new Point(0, 175),
                BackColor = UIHelper.RenkArkaplan
            };
            this.Controls.Add(pnlSatisGrid);

            new Label
            {
                Text = "Satış Listesi",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 8),
                AutoSize = true,
                Parent = pnlSatisGrid
            };

            dgvSatis = new DataGridView
            {
                Size = new Size(1060, 195),
                Location = new Point(20, 30),
                BackgroundColor = UIHelper.RenkArkaplan,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dgvSatis.CellClick += dgvSatis_CellClick;
            pnlSatisGrid.Controls.Add(dgvSatis);

            // ── DETAY PANELİ ──────────────────────────────────────────────
            pnlDetay = new Panel
            {
                Size = new Size(1100, 360),
                Location = new Point(0, 405),
                BackColor = UIHelper.RenkPanel
            };
            this.Controls.Add(pnlDetay);

            new Label
            {
                Text = "📋  Satış Detayları",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 10),
                AutoSize = true,
                Parent = pnlDetay
            };

            pnlDetay.Controls.Add(UIHelper.LblOlustur("Ürün", 20, 38));
            cmbUrun = UIHelper.CmbOlustur(20, 61, 300);
            cmbUrun.SelectedIndexChanged += (s, ev) =>
            {
                if (cmbUrun.SelectedItem is ComboItem item && item.ID > 0)
                {
                    Urun u = urunBL.UrunGetirById(item.ID);
                    if (u != null)
                        txtBirimFiyat.Text = u.Fiyat.ToString("F2");
                }
                else txtBirimFiyat.Clear();
            };
            pnlDetay.Controls.Add(cmbUrun);

            pnlDetay.Controls.Add(UIHelper.LblOlustur("Miktar", 340, 38));
            txtMiktar = UIHelper.TxtOlustur(340, 61, 100);
            txtMiktar.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) && ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };
            pnlDetay.Controls.Add(txtMiktar);

            pnlDetay.Controls.Add(UIHelper.LblOlustur("Birim Fiyat (₺)", 460, 38));
            txtBirimFiyat = UIHelper.TxtOlustur(460, 61, 130);
            pnlDetay.Controls.Add(txtBirimFiyat);

            btnDetayEkle = UIHelper.BtnOlustur("➕ Detay Ekle",
                UIHelper.RenkYesil, 610, 61, 120, 35);
            btnDetayEkle.Enabled = false;
            btnDetayEkle.Click += btnDetayEkle_Click;
            pnlDetay.Controls.Add(btnDetayEkle);

            btnDetaySil = UIHelper.BtnOlustur("🗑️ Detay Sil",
                UIHelper.RenkKirmizi, 740, 61, 110, 35);
            btnDetaySil.Enabled = false;
            btnDetaySil.Click += btnDetaySil_Click;
            pnlDetay.Controls.Add(btnDetaySil);

            dgvDetay = new DataGridView
            {
                Size = new Size(1060, 245),
                Location = new Point(20, 108),
                BackgroundColor = UIHelper.RenkPanel,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dgvDetay.CellClick += dgvDetay_CellClick;
            pnlDetay.Controls.Add(dgvDetay);

            // ── RESIZE ────────────────────────────────────────────────────
            Action yeniden = () =>
            {
                int w = this.ClientSize.Width;
                int h = this.ClientSize.Height;

                pnlBaslik.SetBounds(0, 0, w, 65);
                pnlForm.SetBounds(0, 65, w, 110);
                pnlSatisGrid.SetBounds(0, 175, w, (h - 175) / 2);
                pnlDetay.SetBounds(0, 175 + pnlSatisGrid.Height, w,
                    h - 175 - pnlSatisGrid.Height);

                dgvSatis.SetBounds(20, 30, pnlSatisGrid.Width - 40,
                    pnlSatisGrid.Height - 35);
                dgvDetay.SetBounds(20, 108, pnlDetay.Width - 40,
                    pnlDetay.Height - 115);
            };

            yeniden();
            this.Resize += (s, ev) => yeniden();
            this.Load += frmSatis_Load;
        }

        private void frmSatis_Load(object sender, EventArgs e)
        {
            UIHelper.DgvAyarla(dgvSatis);
            UIHelper.DgvAyarla(dgvDetay);
            MusteriComboYukle();
            UrunComboYukle();
            SatisleriYukle();
        }

        private void MusteriComboYukle()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("musteri");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                cmbMusteri.Items.Clear();
                cmbMusteri.Items.Add(new ComboItem { ID = 0, Ad = "— Müşteri Seçin —" });
                foreach (DataRow row in dt.Rows)
                    cmbMusteri.Items.Add(new ComboItem
                    {
                        ID = Convert.ToInt32(row["MusteriID"]),
                        Ad = row["Ad"] + " " + row["Soyad"]
                    });

                cmbMusteri.DisplayMember = "Ad";
                cmbMusteri.ValueMember = "ID";
                cmbMusteri.SelectedIndex = 0;
            }
            catch { }
        }

        private void UrunComboYukle()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("urun");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                cmbUrun.Items.Clear();
                cmbUrun.Items.Add(new ComboItem { ID = 0, Ad = "— Ürün Seçin —" });
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

        private void SatisleriYukle()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("satis");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                dgvSatis.DataSource = dt;
                UIHelper.DgvAyarla(dgvSatis);

                decimal toplam = 0;
                foreach (DataRow row in dt.Rows)
                    toplam += Convert.ToDecimal(row["ToplamTutar"]);
                lblToplamCiro.Text = $"Toplam Ciro: {toplam:F2} ₺";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Satışlar yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DetaylariYukle(int satisID)
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>($"satis/{satisID}/detay");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                dgvDetay.DataSource = dt;
                UIHelper.DgvAyarla(dgvDetay);
                ToplamHesapla();
            }
            catch { }
        }


        private void ToplamHesapla()
        {
            decimal toplam = 0;
            if (dgvDetay.DataSource is DataTable dt)
                foreach (DataRow row in dt.Rows)
                    toplam += Convert.ToDecimal(row["BirimFiyat"]) *
                              Convert.ToInt32(row["Miktar"]);
            txtToplamTutar.Text = toplam.ToString("F2");
        }

        private void dgvSatis_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            try
            {
                DataGridViewRow satir = dgvSatis.Rows[e.RowIndex];
                seciliSatisID = Convert.ToInt32(satir.Cells["SatisID"].Value);

                int musteriID = Convert.ToInt32(satir.Cells["MusteriID"].Value);
                for (int i = 0; i < cmbMusteri.Items.Count; i++)
                    if (((ComboItem)cmbMusteri.Items[i]).ID == musteriID)
                    { cmbMusteri.SelectedIndex = i; break; }

                string durum = satir.Cells["Durum"].Value?.ToString() ?? "Hazırlanıyor";
                for (int i = 0; i < cmbDurum.Items.Count; i++)
                    if (cmbDurum.Items[i].ToString() == durum)
                    { cmbDurum.SelectedIndex = i; break; }

                txtToplamTutar.Text = satir.Cells["ToplamTutar"].Value?.ToString() ?? "0";
                btnSatisGuncelle.Enabled = true;
                btnSatisSil.Enabled = true;
                btnDetayEkle.Enabled = true;
                DetaylariYukle(seciliSatisID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDetay_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow satir = dgvDetay.Rows[e.RowIndex];
            seciliDetayID = Convert.ToInt32(satir.Cells["DetayID"].Value);

            int urunID = Convert.ToInt32(satir.Cells["UrunID"].Value);
            for (int i = 0; i < cmbUrun.Items.Count; i++)
                if (((ComboItem)cmbUrun.Items[i]).ID == urunID)
                { cmbUrun.SelectedIndex = i; break; }

            txtMiktar.Text = satir.Cells["Miktar"].Value?.ToString() ?? "";
            txtBirimFiyat.Text = satir.Cells["BirimFiyat"].Value?.ToString() ?? "";
            btnDetaySil.Enabled = true;
        }

        private void btnSatisEkle_Click(object sender, EventArgs e)
        {
            try
            {
                var data = new
                {
                    musteriID = cmbMusteri.SelectedItem is ComboItem cm ? cm.ID : 0,
                    perID = OturumBilgisi.AktifPersonel.PerID,
                    toplamTutar = 0,
                    durum = cmbDurum.SelectedItem?.ToString() ?? "Hazırlanıyor"
                };

                dynamic sonuc = ApiServis.Post<dynamic>("satis", data);
                seciliSatisID = (int)sonuc.satisID;

                MessageBox.Show($"Satış oluşturuldu! ID: {seciliSatisID}\nŞimdi ürün ekleyebilirsiniz.",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                SatisleriYukle();
                btnDetayEkle.Enabled = true;
                btnSatisGuncelle.Enabled = true;
                btnSatisSil.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSatisGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliSatisID == 0) return;
            try
            {
                var data = new
                {
                    musteriID = cmbMusteri.SelectedItem is ComboItem cm ? cm.ID : 0,
                    perID = OturumBilgisi.AktifPersonel.PerID,
                    toplamTutar = decimal.TryParse(txtToplamTutar.Text, out decimal t) ? t : 0,
                    durum = cmbDurum.SelectedItem?.ToString() ?? "Hazırlanıyor"
                };

                ApiServis.Put<dynamic>($"satis/{seciliSatisID}", data);
                MessageBox.Show("Satış güncellendi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SatisleriYukle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSatisSil_Click(object sender, EventArgs e)
        {
            if (seciliSatisID == 0) return;
            if (MessageBox.Show("Bu satışı silmek istediğinize emin misiniz?",
                "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                try
                {
                    ApiServis.Delete<dynamic>($"satis/{seciliSatisID}");
                    MessageBox.Show("Satış silindi!",
                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Temizle();
                    SatisleriYukle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message,
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnDetayEkle_Click(object sender, EventArgs e)
        {
            if (seciliSatisID == 0) return;

            if (!int.TryParse(txtMiktar.Text, out int miktar) || miktar <= 0)
            {
                MessageBox.Show("Geçerli bir miktar giriniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtBirimFiyat.Text, out decimal birimFiyat))
            {
                MessageBox.Show("Geçerli bir birim fiyat giriniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var detayData = new
                {
                    satisID = seciliSatisID,
                    urunID = cmbUrun.SelectedItem is ComboItem cu ? cu.ID : 0,
                    miktar = miktar,
                    birimFiyat = birimFiyat
                };

                ApiServis.Post<dynamic>("satis/detay", detayData);

                // Toplam tutarı güncelle
                decimal yeniToplam = (decimal.TryParse(txtToplamTutar.Text,
                    out decimal mevcut) ? mevcut : 0) + (miktar * birimFiyat);

                ApiServis.Put<dynamic>($"satis/{seciliSatisID}", new
                {
                    musteriID = cmbMusteri.SelectedItem is ComboItem cm ? cm.ID : 0,
                    perID = OturumBilgisi.AktifPersonel.PerID,
                    toplamTutar = yeniToplam,
                    durum = cmbDurum.SelectedItem?.ToString() ?? "Hazırlanıyor"
                });

                DetaylariYukle(seciliSatisID);
                SatisleriYukle();
                cmbUrun.SelectedIndex = 0;
                txtMiktar.Clear();
                txtBirimFiyat.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDetaySil_Click(object sender, EventArgs e)
        {
            if (seciliDetayID == 0) return;
            if (MessageBox.Show("Bu detay satırını silmek istiyor musunuz?",
                "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                try
                {
                    ApiServis.Delete<dynamic>($"satis/detay/{seciliDetayID}");
                    seciliDetayID = 0;
                    btnDetaySil.Enabled = false;
                    DetaylariYukle(seciliSatisID);
                    SatisleriYukle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message,
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Temizle()
        {
            cmbMusteri.SelectedIndex = 0;
            cmbDurum.SelectedIndex = 0;
            txtToplamTutar.Clear();
            cmbUrun.SelectedIndex = 0;
            txtMiktar.Clear();
            txtBirimFiyat.Clear();
            dgvDetay.DataSource = null;
            seciliSatisID = 0;
            seciliDetayID = 0;
            btnSatisGuncelle.Enabled = false;
            btnSatisSil.Enabled = false;
            btnDetayEkle.Enabled = false;
            btnDetaySil.Enabled = false;
        }
    }
}