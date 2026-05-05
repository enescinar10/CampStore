using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// frmUrunDetay.cs — Ürün Ekle/Güncelle Popup Formu
// frmUrun'dan çağrılır, modal olarak açılır.
// Kaydedilince frmUrun grid'i yeniler.
using CampStore.BusinessLayer;
using CampStore.Entities;

namespace CampStore.UI
{
    public partial class frmUrunDetay : Form
    {
        private UrunBL urunBL = new UrunBL();
        private KategoriBL kategoriBL = new KategoriBL();

        // Dışarıdan set edilir — 0 ise ekle, >0 ise güncelle
        public int UrunID { get; set; } = 0;

        // Form kapanınca dışarıya "kaydedildi mi" bilgisi verir
        public bool Kaydedildi { get; private set; } = false;

        // ── KONTROLLER ────────────────────────────────────────────────────
        private Label lblBaslik;
        private Label lblUrunAdi, lblKategori, lblFiyat, lblStok;
        private Label lblDurum, lblAciklama;
        private TextBox txtUrunAdi, txtFiyat, txtStok, txtAciklama;
        private ComboBox cmbKategori;
        private CheckBox chkDurum;
        private Button btnKaydet, btnIptal;

        public frmUrunDetay()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            UIHelper.FormAyarla(this, "Ürün", 480, 480);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // ── BAŞLIK ────────────────────────────────────────────────────
            Panel pnlBaslik = UIHelper.BaslikPaneliOlustur("📦  Ürün Bilgileri", 480);
            this.Controls.Add(pnlBaslik);

            // ── FORM ALANLARI ─────────────────────────────────────────────
            Panel pnlForm = new Panel
            {
                Size = new Size(480, 340),
                Location = new Point(0, 65),
                BackColor = UIHelper.RenkPanel
            };
            this.Controls.Add(pnlForm);

            // Ürün Adı
            pnlForm.Controls.Add(UIHelper.LblOlustur("Ürün Adı", 20, 20));
            txtUrunAdi = UIHelper.TxtOlustur(20, 43, 420);
            pnlForm.Controls.Add(txtUrunAdi);

            // Kategori
            pnlForm.Controls.Add(UIHelper.LblOlustur("Kategori", 20, 88));
            cmbKategori = UIHelper.CmbOlustur(20, 111, 420);
            pnlForm.Controls.Add(cmbKategori);

            // Fiyat + Stok yan yana
            pnlForm.Controls.Add(UIHelper.LblOlustur("Fiyat (₺)", 20, 156));
            txtFiyat = UIHelper.TxtOlustur(20, 179, 195);
            txtFiyat.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) &&
                    ev.KeyChar != ',' &&
                    ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };
            pnlForm.Controls.Add(txtFiyat);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Stok", 245, 156));
            txtStok = UIHelper.TxtOlustur(245, 179, 195);
            txtStok.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) && ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };
            pnlForm.Controls.Add(txtStok);

            // Açıklama
            pnlForm.Controls.Add(UIHelper.LblOlustur("Açıklama", 20, 224));
            txtAciklama = UIHelper.TxtOlustur(20, 247, 420);
            pnlForm.Controls.Add(txtAciklama);

            // Durum
            chkDurum = new CheckBox
            {
                Text = "Ürün Aktif",
                Location = new Point(20, 292),
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.White,
                Checked = true,
                AutoSize = true
            };
            pnlForm.Controls.Add(chkDurum);

            // ── BUTONLAR ──────────────────────────────────────────────────
            Panel pnlButon = new Panel
            {
                Size = new Size(480, 60),
                Location = new Point(0, 405),
                BackColor = UIHelper.RenkMenuKoyu
            };
            this.Controls.Add(pnlButon);

            btnKaydet = UIHelper.BtnOlustur("💾 Kaydet",
                UIHelper.RenkYesil, 240, 11, 110, 38);
            btnKaydet.Click += btnKaydet_Click;
            pnlButon.Controls.Add(btnKaydet);

            btnIptal = UIHelper.BtnOlustur("✖ İptal",
                UIHelper.RenkGri, 360, 11, 90, 38);
            btnIptal.Click += (s, ev) => this.Close();
            pnlButon.Controls.Add(btnIptal);

            this.AcceptButton = btnKaydet;
            this.Load += frmUrunDetay_Load;
        }

        // ── FORM YÜKLENINCE ───────────────────────────────────────────────
        private void frmUrunDetay_Load(object sender, EventArgs e)
        {
            KategoriComboYukle();

            if (UrunID > 0)
            {
                // Güncelleme modu — mevcut veriyi doldur
                this.Text = "Ürün Güncelle";
                ((Label)((Panel)this.Controls[0]).Controls[1]).Text = "📦  Ürün Güncelle";
                MevcutVeriDoldur();
            }
            else
            {
                // Ekleme modu
                this.Text = "Yeni Ürün Ekle";
            }
        }

        // ── KATEGORİ COMBO ────────────────────────────────────────────────
        private void KategoriComboYukle()
        {
            try
            {
                DataTable dt = kategoriBL.KategoriListele();
                cmbKategori.Items.Clear();
                cmbKategori.Items.Add(new ComboItem { ID = 0, Ad = "— Kategori Seçin —" });

                foreach (DataRow row in dt.Rows)
                    cmbKategori.Items.Add(new ComboItem
                    {
                        ID = Convert.ToInt32(row["KatID"]),
                        Ad = row["KatAdi"].ToString()
                    });

                cmbKategori.DisplayMember = "Ad";
                cmbKategori.ValueMember = "ID";
                cmbKategori.SelectedIndex = 0;
            }
            catch { }
        }

        // ── MEVCUT VERİYİ DOLDUR (güncelleme modu) ───────────────────────
        private void MevcutVeriDoldur()
        {
            try
            {
                Urun u = urunBL.UrunGetirById(UrunID);
                if (u == null) return;

                txtUrunAdi.Text = u.UrunAdi;
                txtFiyat.Text = u.Fiyat.ToString("F2");
                txtStok.Text = u.Stok.ToString();
                txtAciklama.Text = u.Aciklama;
                chkDurum.Checked = u.Durum;

                // Kategoriyi seç
                for (int i = 0; i < cmbKategori.Items.Count; i++)
                {
                    if (((ComboItem)cmbKategori.Items[i]).ID == u.KatID)
                    {
                        cmbKategori.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri yüklenemedi: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── KAYDET ───────────────────────────────────────────────────────
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtFiyat.Text, out decimal fiyat))
            {
                MessageBox.Show("Geçerli bir fiyat giriniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtStok.Text, out int stok))
            {
                MessageBox.Show("Geçerli bir stok giriniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var urunData = new
                {
                    katID = ((ComboItem)cmbKategori.SelectedItem).ID,
                    urunAdi = txtUrunAdi.Text.Trim(),
                    fiyat = fiyat,
                    stok = stok,
                    durum = chkDurum.Checked,
                    aciklama = txtAciklama.Text.Trim()
                };

                dynamic sonuc;

                if (UrunID > 0)
                    // Güncelle
                    sonuc = ApiServis.Put<dynamic>($"urun/{UrunID}", urunData);
                else
                    // Ekle
                    sonuc = ApiServis.Post<dynamic>("urun", urunData);

                MessageBox.Show(
                    UrunID > 0 ? "Ürün güncellendi!" : "Ürün eklendi!",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Kaydedildi = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
