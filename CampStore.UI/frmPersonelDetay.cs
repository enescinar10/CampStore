using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// frmPersonelDetay.cs — Personel Ekle/Güncelle Popup Formu
using CampStore.BusinessLayer;
using CampStore.Entities;

namespace CampStore.UI
{
    public partial class frmPersonelDetay : Form
    {
        private PersonelBL personelBL = new PersonelBL();

        public int PerID { get; set; } = 0;
        public bool Kaydedildi { get; private set; } = false;

        private TextBox txtAd, txtSoyad, txtTC, txtTelefon, txtAdres, txtSifre;
        private ComboBox cmbRol;
        private DateTimePicker dtpDogum, dtpIseGiris, dtpIstenCikis;
        private CheckBox chkAktif;
        private Button btnKaydet, btnIptal;

        public frmPersonelDetay()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            // SİHİRLİ DOKUNUŞ: Formun İÇ(tuval) boyutunu netleştiriyoruz!
            // Başlık(65) + Form(440) + Buton(60) = Toplam 565 piksel
            this.ClientSize = new Size(520, 565);
            this.Text = "Personel";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = UIHelper.RenkArkaplan;

            // 1️⃣ BAŞLIK PANELİ (Y=0'dan başlar)
            Panel pnlBaslik = UIHelper.BaslikPaneliOlustur("👨‍💼  Personel Bilgileri", this.ClientSize.Width);
            pnlBaslik.Location = new Point(0, 0);
            this.Controls.Add(pnlBaslik);

            // 2️⃣ FORM PANELİ (Y=65'ten başlar)
            Panel pnlForm = new Panel
            {
                Size = new Size(520, 440),
                Location = new Point(0, 65),
                BackColor = UIHelper.RenkPanel
            };
            this.Controls.Add(pnlForm);

            // 3️⃣ BUTON PANELİ (Y=505'ten başlar, 65+440=505)
            Panel pnlButon = new Panel
            {
                Size = new Size(520, 60),
                Location = new Point(0, 505),
                BackColor = UIHelper.RenkMenuKoyu
            };
            this.Controls.Add(pnlButon);

            // --- Satır 1 — Ad, Soyad ---
            pnlForm.Controls.Add(UIHelper.LblOlustur("Ad", 20, 20));
            txtAd = UIHelper.TxtOlustur(20, 43, 225);
            pnlForm.Controls.Add(txtAd);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Soyad", 265, 20));
            txtSoyad = UIHelper.TxtOlustur(265, 43, 225);
            pnlForm.Controls.Add(txtSoyad);

            // --- Satır 2 — TC, Telefon ---
            pnlForm.Controls.Add(UIHelper.LblOlustur("TC Kimlik No", 20, 88));
            txtTC = UIHelper.TxtOlustur(20, 111, 225);
            txtTC.MaxLength = 11;
            txtTC.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) && ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };
            pnlForm.Controls.Add(txtTC);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Telefon", 265, 88));
            txtTelefon = UIHelper.TxtOlustur(265, 111, 225);
            pnlForm.Controls.Add(txtTelefon);

            // --- Satır 3 — Şifre, Rol ---
            pnlForm.Controls.Add(UIHelper.LblOlustur("Şifre", 20, 156));
            txtSifre = UIHelper.TxtOlustur(20, 179, 225);
            txtSifre.PasswordChar = '●';
            pnlForm.Controls.Add(txtSifre);

            pnlForm.Controls.Add(UIHelper.LblOlustur("Rol", 265, 156));
            cmbRol = UIHelper.CmbOlustur(265, 179, 225);
            pnlForm.Controls.Add(cmbRol);

            // --- Satır 4 — Adres ---
            pnlForm.Controls.Add(UIHelper.LblOlustur("Adres", 20, 224));
            txtAdres = UIHelper.TxtOlustur(20, 247, 470);
            pnlForm.Controls.Add(txtAdres);

            // --- Satır 5 — Doğum Tarihi, İşe Giriş ---
            pnlForm.Controls.Add(UIHelper.LblOlustur("Doğum Tarihi", 20, 292));
            dtpDogum = new DateTimePicker
            {
                Size = new Size(225, 30),
                Location = new Point(20, 315),
                Font = new Font("Segoe UI", 10f),
                Format = DateTimePickerFormat.Short
            };
            pnlForm.Controls.Add(dtpDogum);

            pnlForm.Controls.Add(UIHelper.LblOlustur("İşe Giriş Tarihi", 265, 292));
            dtpIseGiris = new DateTimePicker
            {
                Size = new Size(225, 30),
                Location = new Point(265, 315),
                Font = new Font("Segoe UI", 10f),
                Format = DateTimePickerFormat.Short
            };
            pnlForm.Controls.Add(dtpIseGiris);

            // --- Satır 6 — Aktif çalışıyor, İşten çıkış ---
            chkAktif = new CheckBox
            {
                Text = "Aktif Çalışıyor",
                Location = new Point(20, 365),
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.White,
                Checked = true,
                AutoSize = true
            };
            chkAktif.CheckedChanged += (s, ev) =>
                dtpIstenCikis.Enabled = !chkAktif.Checked;
            pnlForm.Controls.Add(chkAktif);

            pnlForm.Controls.Add(UIHelper.LblOlustur("İşten Çıkış Tarihi", 265, 348));
            dtpIstenCikis = new DateTimePicker
            {
                Size = new Size(225, 30),
                Location = new Point(265, 371),
                Font = new Font("Segoe UI", 10f),
                Format = DateTimePickerFormat.Short,
                Enabled = false
            };
            pnlForm.Controls.Add(dtpIstenCikis);

            // --- Butonları Ekle ---
            btnKaydet = UIHelper.BtnOlustur("💾 Kaydet", UIHelper.RenkYesil, 290, 11, 110, 38);
            btnKaydet.Click += btnKaydet_Click;
            pnlButon.Controls.Add(btnKaydet);

            btnIptal = UIHelper.BtnOlustur("✖ İptal", UIHelper.RenkGri, 410, 11, 90, 38);
            btnIptal.Click += (s, ev) => this.Close();
            pnlButon.Controls.Add(btnIptal);

            this.AcceptButton = btnKaydet;
            this.Load += frmPersonelDetay_Load;
        }

        private void frmPersonelDetay_Load(object sender, EventArgs e)
        {
            RolYukle();

            if (PerID > 0)
            {
                try
                {
                    dynamic sonuc = ApiServis.Get<dynamic>("personel");
                    DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                        Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                    DataRow[] rows = dt.Select($"PerID = {PerID}");
                    if (rows.Length == 0) return;

                    DataRow row = rows[0];
                    txtAd.Text = row["PerAd"].ToString();
                    txtSoyad.Text = row["PerSoyad"].ToString();
                    txtTC.Text = row["TC"].ToString();
                    txtTelefon.Text = row["Telefon"].ToString();
                    txtSifre.Text = row["Sifre"].ToString();

                    // Adres kolonunu kontrol et
                    if (dt.Columns.Contains("Adres"))
                        txtAdres.Text = row["Adres"].ToString();

                    // Tarih kolonlarını kontrol ederek yükle
                    if (dt.Columns.Contains("DogumTarihi") &&
                        row["DogumTarihi"] != DBNull.Value)
                        dtpDogum.Value = Convert.ToDateTime(row["DogumTarihi"]);

                    if (dt.Columns.Contains("IseGirisTarihi") &&
                        row["IseGirisTarihi"] != DBNull.Value)
                        dtpIseGiris.Value = Convert.ToDateTime(row["IseGirisTarihi"]);
                    else if (dt.Columns.Contains("IşeGirişTarihi") &&
                             row["IşeGirişTarihi"] != DBNull.Value)
                        dtpIseGiris.Value = Convert.ToDateTime(row["IşeGirişTarihi"]);

                    // İşten çıkış
                    string istenCikisKolon = dt.Columns.Contains("IstenCikisTarihi")
                        ? "IstenCikisTarihi" : null;

                    if (istenCikisKolon != null &&
                        row[istenCikisKolon] != DBNull.Value)
                    {
                        chkAktif.Checked = false;
                        dtpIstenCikis.Enabled = true;
                        dtpIstenCikis.Value = Convert.ToDateTime(row[istenCikisKolon]);
                    }
                    else
                    {
                        chkAktif.Checked = true;
                        dtpIstenCikis.Enabled = false;
                    }

                    // Rol seç
                    int rolID = Convert.ToInt32(row["RolID"]);
                    for (int i = 0; i < cmbRol.Items.Count; i++)
                    {
                        if (((ComboItem)cmbRol.Items[i]).ID == rolID)
                        {
                            cmbRol.SelectedIndex = i;
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
        }

        private void RolYukle()
        {
            try
            {
                dynamic sonuc = ApiServis.Get<dynamic>("personel/roller");
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(sonuc));

                cmbRol.Items.Clear();
                cmbRol.Items.Add(new ComboItem { ID = 0, Ad = "— Rol Seçin —" });
                foreach (DataRow row in dt.Rows)
                    cmbRol.Items.Add(new ComboItem
                    {
                        ID = Convert.ToInt32(row["RolID"]),
                        Ad = row["RolAdi"].ToString()
                    });

                cmbRol.DisplayMember = "Ad";
                cmbRol.ValueMember = "ID";
                cmbRol.SelectedIndex = 0;
            }
            catch { }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                var data = new
                {
                    perAd = txtAd.Text.Trim(),
                    perSoyad = txtSoyad.Text.Trim(),
                    tC = txtTC.Text.Trim(),
                    telefon = txtTelefon.Text.Trim(),
                    adres = txtAdres.Text.Trim(),
                    sifre = txtSifre.Text,
                    rolID = cmbRol.SelectedItem is ComboItem ci ? ci.ID : 0,
                    dogumTarihi = dtpDogum.Value,
                    iseGirisTarihi = dtpIseGiris.Value,
                    istenCikisTarihi = chkAktif.Checked ? (DateTime?)null : dtpIstenCikis.Value
                };

                if (PerID > 0)
                    ApiServis.Put<dynamic>($"personel/{PerID}", data);
                else
                    ApiServis.Post<dynamic>("personel", data);

                MessageBox.Show(PerID > 0 ? "Personel güncellendi!" : "Personel eklendi!",
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
