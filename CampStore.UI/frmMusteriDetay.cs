using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// frmMusteriDetay.cs — Müşteri Ekle/Güncelle Popup Formu

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CampStore.BusinessLayer;
using CampStore.Entities;

namespace CampStore.UI
{
    public partial class frmMusteriDetay : Form
    {
        private MusteriBL musteriBL = new MusteriBL();

        public int MusteriID { get; set; } = 0;
        public bool Kaydedildi { get; private set; } = false;

        private TextBox txtAd, txtSoyad, txtEmail;
        private TextBox txtTelefon, txtSifre, txtAdres;
        private ComboBox cmbSehir, cmbIlce;
        private CheckBox chkDurum;
        private Button btnKaydet, btnIptal;

        public frmMusteriDetay()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            // SİHİRLİ DOKUNUŞ: Formun DIŞINI değil, İÇ (Kullanım) alanını 500x505 yaptık.
            // Başlık(65) + Form(380) + Buton(60) = Tam 505 piksel yapar. Sıfır kayma!
            this.ClientSize = new Size(500, 505);
            this.Text = "Müşteri";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // 1️⃣ BAŞLIK PANELİ (Y=0'dan başlar)
            Panel pnlBaslik = UIHelper.BaslikPaneliOlustur("👤  Müşteri Bilgileri", this.ClientSize.Width);
            pnlBaslik.Location = new Point(0, 0);
            this.Controls.Add(pnlBaslik);

            // 2️⃣ FORM PANELİ (Y=65'ten başlar, tam başlığın bittiği yer)
            Panel pnlForm = new Panel
            {
                Size = new Size(500, 380),
                Location = new Point(0, 65),
                BackColor = UIHelper.RenkPanel
            };
            this.Controls.Add(pnlForm);

            // 3️⃣ BUTON PANELİ (Y=445'ten başlar, 65+380 = 445)
            Panel pnlButon = new Panel
            {
                Size = new Size(500, 60),
                Location = new Point(0, 445),
                BackColor = UIHelper.RenkMenuKoyu
            };
            this.Controls.Add(pnlButon);

            // --- FORM İÇİ ELEMANLAR (Hiçbirine dokunmadım, aynı koordinatların) ---

            // Satır 1
            pnlForm.Controls.Add(UIHelper.LblOlustur("Ad", 20, 20));
            txtAd = UIHelper.TxtOlustur(20, 43, 215);
            pnlForm.Controls.Add(txtAd);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Soyad", 255, 20));
            txtSoyad = UIHelper.TxtOlustur(255, 43, 215);
            pnlForm.Controls.Add(txtSoyad);

            // Satır 2
            pnlForm.Controls.Add(UIHelper.LblOlustur("Email", 20, 88));
            txtEmail = UIHelper.TxtOlustur(20, 111, 215);
            pnlForm.Controls.Add(txtEmail);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Telefon", 255, 88));
            txtTelefon = UIHelper.TxtOlustur(255, 111, 215);
            pnlForm.Controls.Add(txtTelefon);

            // Satır 3
            pnlForm.Controls.Add(UIHelper.LblOlustur("Şifre", 20, 156));
            txtSifre = UIHelper.TxtOlustur(20, 179, 215);
            txtSifre.PasswordChar = '●';
            pnlForm.Controls.Add(txtSifre);

            chkDurum = new CheckBox
            {
                Text = "Müşteri Aktif",
                Location = new Point(255, 182),
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.White,
                Checked = true,
                AutoSize = true
            };
            pnlForm.Controls.Add(chkDurum);

            // Satır 4
            pnlForm.Controls.Add(UIHelper.LblOlustur("Adres", 20, 224));
            txtAdres = UIHelper.TxtOlustur(20, 247, 450);
            pnlForm.Controls.Add(txtAdres);

            // Satır 5
            pnlForm.Controls.Add(UIHelper.LblOlustur("Şehir", 20, 292));
            cmbSehir = UIHelper.CmbOlustur(20, 315, 215);
            cmbSehir.SelectedIndexChanged += (s, ev) =>
            {
                if (cmbSehir.SelectedItem is ComboItem item && item.ID > 0)
                    IlceYukle(item.ID);
                else
                    cmbIlce.Items.Clear();
            };
            pnlForm.Controls.Add(cmbSehir);

            pnlForm.Controls.Add(UIHelper.LblOlustur("İlçe", 255, 292));
            cmbIlce = UIHelper.CmbOlustur(255, 315, 215);
            pnlForm.Controls.Add(cmbIlce);

            // --- BUTONLAR ---
            btnKaydet = UIHelper.BtnOlustur("💾 Kaydet", UIHelper.RenkYesil, 270, 11, 110, 38);
            btnKaydet.Click += btnKaydet_Click;
            pnlButon.Controls.Add(btnKaydet);

            btnIptal = UIHelper.BtnOlustur("✖ İptal", UIHelper.RenkGri, 390, 11, 90, 38);
            btnIptal.Click += (s, ev) => this.Close();
            pnlButon.Controls.Add(btnIptal);

            this.AcceptButton = btnKaydet;
            this.Load += frmMusteriDetay_Load;
        }

        private void frmMusteriDetay_Load(object sender, EventArgs e)
        {
            SehirYukle();

            // Şehir değişince ilçe yükle
            cmbSehir.SelectedIndexChanged += (s, ev) =>
            {
                if (cmbSehir.SelectedItem is ComboItem item && item.ID > 0)
                    IlceYukle(item.ID);
                else
                    cmbIlce.Items.Clear();
            };

            if (MusteriID > 0)
            {
                try
                {
                    dynamic sonuc = ApiServis.Get<dynamic>("musteri");
                    DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                        Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                    DataRow[] rows = dt.Select($"MusteriID = {MusteriID}");
                    if (rows.Length == 0) return;

                    DataRow row = rows[0];
                    txtAd.Text = row["Ad"].ToString();
                    txtSoyad.Text = row["Soyad"].ToString();
                    txtEmail.Text = row["Email"].ToString();
                    txtTelefon.Text = row["Telefon"].ToString();
                    txtAdres.Text = row["Adres"].ToString();
                    chkDurum.Checked = Convert.ToBoolean(row["Durum"]);

                    // Şehir seç
                    if (dt.Columns.Contains("SehirID"))
                    {
                        int sehirID = Convert.ToInt32(row["SehirID"]);
                        for (int i = 0; i < cmbSehir.Items.Count; i++)
                        {
                            if (((ComboItem)cmbSehir.Items[i]).ID == sehirID)
                            {
                                cmbSehir.SelectedIndex = i;
                                break;
                            }
                        }

                        // İlçe yükle ve seç
                        if (dt.Columns.Contains("IlceID"))
                        {
                            int ilceID = Convert.ToInt32(row["IlceID"]);
                            IlceYukle(sehirID);

                            for (int i = 0; i < cmbIlce.Items.Count; i++)
                            {
                                if (((ComboItem)cmbIlce.Items[i]).ID == ilceID)
                                {
                                    cmbIlce.SelectedIndex = i;
                                    break;
                                }
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

        private void SehirYukle()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("musteri/sehir");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                cmbSehir.Items.Clear();
                cmbSehir.Items.Add(new ComboItem { ID = 0, Ad = "— Şehir Seçin —" });
                foreach (DataRow row in dt.Rows)
                    cmbSehir.Items.Add(new ComboItem
                    {
                        ID = Convert.ToInt32(row["SehirID"]),
                        Ad = row["SehirAdi"].ToString()
                    });

                cmbSehir.DisplayMember = "Ad";
                cmbSehir.ValueMember = "ID";
                cmbSehir.SelectedIndex = 0;
            }
            catch { }
        }

        private void IlceYukle(int sehirID)
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>($"musteri/ilce/{sehirID}");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                cmbIlce.Items.Clear();
                cmbIlce.Items.Add(new ComboItem { ID = 0, Ad = "— İlçe Seçin —" });
                foreach (DataRow row in dt.Rows)
                    cmbIlce.Items.Add(new ComboItem
                    {
                        ID = Convert.ToInt32(row["IlceID"]),
                        Ad = row["IlceAdi"].ToString()
                    });

                cmbIlce.DisplayMember = "Ad";
                cmbIlce.ValueMember = "ID";
                cmbIlce.SelectedIndex = 0;
            }
            catch { }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                var data = new
                {
                    ad = txtAd.Text.Trim(),
                    soyad = txtSoyad.Text.Trim(),
                    email = txtEmail.Text.Trim(),
                    sifre = txtSifre.Text,
                    telefon = txtTelefon.Text.Trim(),
                    adres = txtAdres.Text.Trim(),
                    sehirID = cmbSehir.SelectedItem is ComboItem cs ? cs.ID : 0,
                    ilceID = cmbIlce.SelectedItem is ComboItem ci ? ci.ID : 0,
                    durum = chkDurum.Checked
                };

                if (MusteriID > 0)
                    ApiServis.Put<dynamic>($"musteri/{MusteriID}", data);
                else
                    ApiServis.Post<dynamic>("musteri", data);

                MessageBox.Show(MusteriID > 0 ? "Müşteri güncellendi!" : "Müşteri eklendi!",
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