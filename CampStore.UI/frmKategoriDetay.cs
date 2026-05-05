using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// frmKategoriDetay.cs — Kategori Ekle/Güncelle Popup Formu

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CampStore.BusinessLayer;
using CampStore.Entities;

namespace CampStore.UI
{
    public partial class frmKategoriDetay : Form
    {
        private KategoriBL kategoriBL = new KategoriBL();

        public int KatID { get; set; } = 0;
        public bool Kaydedildi { get; private set; } = false;

        private TextBox txtKatAdi;
        private ComboBox cmbUstKategori;
        private Button btnKaydet, btnIptal;

        public frmKategoriDetay()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            UIHelper.FormAyarla(this, "Kategori", 420, 320);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Başlık
            Panel pnlBaslik = UIHelper.BaslikPaneliOlustur("🗂️  Kategori Bilgileri", 420);
            this.Controls.Add(pnlBaslik);

            // Form alanları
            Panel pnlForm = new Panel
            {
                Size = new Size(420, 170),
                Location = new Point(0, 65),
                BackColor = UIHelper.RenkPanel
            };
            this.Controls.Add(pnlForm);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Kategori Adı", 20, 20));
            txtKatAdi = UIHelper.TxtOlustur(20, 43, 380);
            pnlForm.Controls.Add(txtKatAdi);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Üst Kategori (Opsiyonel)", 20, 88));
            cmbUstKategori = UIHelper.CmbOlustur(20, 111, 380);
            pnlForm.Controls.Add(cmbUstKategori);

            // Butonlar
            Panel pnlButon = new Panel
            {
                Size = new Size(420, 60),
                Location = new Point(0, 235),
                BackColor = UIHelper.RenkMenuKoyu
            };
            this.Controls.Add(pnlButon);

            btnKaydet = UIHelper.BtnOlustur("💾 Kaydet", UIHelper.RenkYesil, 190, 11, 110, 38);
            btnKaydet.Click += btnKaydet_Click;
            pnlButon.Controls.Add(btnKaydet);

            btnIptal = UIHelper.BtnOlustur("✖ İptal", UIHelper.RenkGri, 310, 11, 90, 38);
            btnIptal.Click += (s, ev) => this.Close();
            pnlButon.Controls.Add(btnIptal);

            this.AcceptButton = btnKaydet;
            this.Load += frmKategoriDetay_Load;
        }

        private void frmKategoriDetay_Load(object sender, EventArgs e)
        {
            UstKategoriYukle();

            if (KatID > 0)
            {
                try
                {
                    // Tüm kategori listesinden filtrele
                    dynamic sonuc = ApiServis.Get<dynamic>("kategori");
                    DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                        Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                    DataRow[] rows = dt.Select($"KatID = {KatID}");
                    if (rows.Length == 0) return;

                    DataRow row = rows[0];
                    txtKatAdi.Text = row["KatAdi"].ToString();

                    // UstKategoriID kolonunu kontrol et
                    if (dt.Columns.Contains("UstKategoriID") &&
                        row["UstKategoriID"] != DBNull.Value)
                    {
                        int ustID = Convert.ToInt32(row["UstKategoriID"]);
                        for (int i = 0; i < cmbUstKategori.Items.Count; i++)
                        {
                            if (((ComboItem)cmbUstKategori.Items[i]).ID == ustID)
                            {
                                cmbUstKategori.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veri yüklenemedi: " + ex.Message,
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UstKategoriYukle()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("kategori");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                cmbUstKategori.Items.Clear();
                cmbUstKategori.Items.Add(new ComboItem { ID = 0, Ad = "— Üst Kategori Yok —" });

                foreach (DataRow row in dt.Rows)
                    cmbUstKategori.Items.Add(new ComboItem
                    {
                        ID = Convert.ToInt32(row["KatID"]),
                        Ad = row["KatAdi"].ToString()
                    });

                cmbUstKategori.DisplayMember = "Ad";
                cmbUstKategori.ValueMember = "ID";
                cmbUstKategori.SelectedIndex = 0;
            }
            catch { }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtKatAdi.Text))
            {
                MessageBox.Show("Kategori adı boş olamaz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var data = new
                {
                    katAdi = txtKatAdi.Text.Trim(),
                    ustKategoriID = cmbUstKategori.SelectedItem is ComboItem ci && ci.ID > 0
                                   ? (int?)ci.ID : null
                };

                if (KatID > 0)
                    ApiServis.Put<dynamic>($"kategori/{KatID}", data);
                else
                    ApiServis.Post<dynamic>("kategori", data);

                MessageBox.Show(KatID > 0 ? "Kategori güncellendi!" : "Kategori eklendi!",
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