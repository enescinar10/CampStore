// frmLogin.cs — Yenilenmiş versiyon
// TC, Kullanıcı Adı ile giriş desteği
// Giriş tipini RadioButton ile seçer

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CampStore.BusinessLayer;
using CampStore.Entities;

namespace CampStore.UI
{
    public partial class frmLogin : Form
    {
        private PersonelBL personelBL = new PersonelBL();

        private Panel pnlArkaplan, pnlKart;
        private Label lblLogo, lblBaslik, lblAltBaslik;
        private Label lblGirisTipi, lblGirisBilgisi, lblSifre;
        private RadioButton rdoTC, rdoKullanici;
        private TextBox txtGirisBilgisi, txtSifre;
        private Button btnGiris;
        private Label lblHata;
        private CheckBox chkSifreGoster;

        public frmLogin()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            // KontrolleriOlustur'da
            UIHelper.FormAyarla(this, "CampStore — Giriş", 500, 600);
            this.MaximizeBox = false;
            this.Resize += (s, ev) => frmLogin_Load(s, ev); // Resize'da da ortala
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Arka plan
            pnlArkaplan = new Panel
            {
                Size = new Size(480, 580),
                Location = new Point(0, 0),
                BackColor = UIHelper.RenkArkaplan
            };
            this.Controls.Add(pnlArkaplan);

            // Üst dekoratif şerit
            Panel pnlSerit = new Panel
            {
                Size = new Size(480, 6),
                Location = new Point(0, 0),
                BackColor = UIHelper.RenkYesil
            };
            pnlArkaplan.Controls.Add(pnlSerit);

            // Kart paneli
            pnlKart = new Panel
            {
                Size = new Size(380, 460),
                Location = new Point(50, 55),
                BackColor = Color.White
            };
            pnlKart.Paint += (s, ev) =>
            {
                GraphicsPath yol = new GraphicsPath();
                int r = 16;
                Rectangle alan = pnlKart.ClientRectangle;
                yol.AddArc(alan.X, alan.Y, r, r, 180, 90);
                yol.AddArc(alan.Right - r, alan.Y, r, r, 270, 90);
                yol.AddArc(alan.Right - r, alan.Bottom - r, r, r, 0, 90);
                yol.AddArc(alan.X, alan.Bottom - r, r, r, 90, 90);
                yol.CloseAllFigures();
                pnlKart.Region = new Region(yol);
            };
            pnlArkaplan.Controls.Add(pnlKart);

            // Sol yeşil aksanı
            Panel pnlAksan = new Panel
            {
                Size = new Size(4, 460),
                Location = new Point(50, 55),
                BackColor = UIHelper.RenkYesil
            };
            pnlArkaplan.Controls.Add(pnlAksan);

            // Logo
            lblLogo = new Label
            {
                Text = "🏕️",
                Font = new Font("Segoe UI", 30f),
                ForeColor = UIHelper.RenkArkaplan,
                Location = new Point(155, 20),
                AutoSize = true
            };
            pnlKart.Controls.Add(lblLogo);

            // Başlık
            lblBaslik = new Label
            {
                Text = "CampStore",
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = UIHelper.RenkArkaplan,
                Location = new Point(100, 75),
                AutoSize = true
            };
            pnlKart.Controls.Add(lblBaslik);

            lblAltBaslik = new Label
            {
                Text = "Yönetim Paneline Giriş",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.Gray,
                Location = new Point(108, 110),
                AutoSize = true
            };
            pnlKart.Controls.Add(lblAltBaslik);

            // Ayraç çizgisi
            Panel pnlCizgi = new Panel
            {
                Size = new Size(320, 1),
                Location = new Point(30, 138),
                BackColor = Color.FromArgb(220, 220, 220)
            };
            pnlKart.Controls.Add(pnlCizgi);

            // ── GİRİŞ TİPİ SEÇİMİ ────────────────────────────────────────
            lblGirisTipi = new Label
            {
                Text = "Giriş Tipi",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = UIHelper.RenkArkaplan,
                Location = new Point(30, 155),
                AutoSize = true
            };
            pnlKart.Controls.Add(lblGirisTipi);

            // RadioButton container
            Panel pnlRadio = new Panel
            {
                Size = new Size(320, 35),
                Location = new Point(30, 175),
                BackColor = Color.FromArgb(245, 247, 250)
            };
            pnlKart.Controls.Add(pnlRadio);

            rdoTC = new RadioButton
            {
                Text = "🪪 TC Kimlik",
                Font = new Font("Segoe UI", 9f),
                ForeColor = UIHelper.RenkArkaplan,
                Location = new Point(10, 8),
                Checked = true,
                AutoSize = true
            };
            rdoTC.CheckedChanged += GirisTipiDegisti;
            pnlRadio.Controls.Add(rdoTC);

            rdoKullanici = new RadioButton
            {
                Text = "👤 Kullanıcı Adı",
                Font = new Font("Segoe UI", 9f),
                ForeColor = UIHelper.RenkArkaplan,
                Location = new Point(140, 8),
                AutoSize = true
            };
            rdoKullanici.CheckedChanged += GirisTipiDegisti;
            pnlRadio.Controls.Add(rdoKullanici);

            // ── GİRİŞ BİLGİSİ ────────────────────────────────────────────
            lblGirisBilgisi = new Label
            {
                Text = "TC Kimlik No",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = UIHelper.RenkArkaplan,
                Location = new Point(30, 223),
                AutoSize = true
            };
            pnlKart.Controls.Add(lblGirisBilgisi);

            txtGirisBilgisi = new TextBox
            {
                Size = new Size(320, 35),
                Location = new Point(30, 243),
                Font = new Font("Segoe UI", 11f),
                BorderStyle = BorderStyle.FixedSingle,
                MaxLength = 11,
                BackColor = Color.FromArgb(245, 247, 250)
            };
            txtGirisBilgisi.KeyPress += GirisBilgisiKeyPress;
            pnlKart.Controls.Add(txtGirisBilgisi);

            // ── ŞİFRE ─────────────────────────────────────────────────────
            lblSifre = new Label
            {
                Text = "Şifre",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = UIHelper.RenkArkaplan,
                Location = new Point(30, 292),
                AutoSize = true
            };
            pnlKart.Controls.Add(lblSifre);

            txtSifre = new TextBox
            {
                Size = new Size(320, 35),
                Location = new Point(30, 312),
                Font = new Font("Segoe UI", 11f),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '●',
                BackColor = Color.FromArgb(245, 247, 250)
            };
            pnlKart.Controls.Add(txtSifre);

            // Şifreyi göster checkbox
            chkSifreGoster = new CheckBox
            {
                Text = "Şifreyi Göster",
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.Gray,
                Location = new Point(30, 352),
                AutoSize = true
            };
            chkSifreGoster.CheckedChanged += (s, ev) =>
                txtSifre.PasswordChar = chkSifreGoster.Checked ? '\0' : '●';
            pnlKart.Controls.Add(chkSifreGoster);

            // ── HATA MESAJI ───────────────────────────────────────────────
            lblHata = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 8f),
                ForeColor = UIHelper.RenkKirmizi,
                Location = new Point(30, 375),
                Size = new Size(320, 18),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlKart.Controls.Add(lblHata);

            // ── GİRİŞ BUTONU ──────────────────────────────────────────────
            btnGiris = UIHelper.BtnOlustur("Giriş Yap",
                UIHelper.RenkYesil, 30, 398, 320, 45);
            btnGiris.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            btnGiris.Click += btnGiris_Click;
            pnlKart.Controls.Add(btnGiris);

            // Alt bilgi
            new Label
            {
                Text = "© 2026 CampStore — Kamp Malzemeleri",
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(100, 120, 140),
                Location = new Point(95, 530),
                AutoSize = true,
                Parent = pnlArkaplan
            };

            this.AcceptButton = btnGiris;
            this.Load += frmLogin_Load;
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            // Kart her zaman ortada olsun
            pnlKart.Location = new Point(
                (this.ClientSize.Width - pnlKart.Width) / 2,
                (this.ClientSize.Height - pnlKart.Height) / 2 - 20
            );

            // Sol aksan çizgisini da güncelle
            foreach (Control ctrl in pnlArkaplan.Controls)
            {
                if (ctrl is Panel p && p.Width == 4)
                {
                    p.Location = new Point(pnlKart.Location.X, pnlKart.Location.Y);
                    p.Height = pnlKart.Height;
                }
            }
            txtGirisBilgisi.Focus();
        }

        // ── GİRİŞ TİPİ DEĞİŞTİ ───────────────────────────────────────────
        private void GirisTipiDegisti(object sender, EventArgs e)
        {
            if (rdoTC.Checked)
            {
                lblGirisBilgisi.Text = "TC Kimlik No";
                txtGirisBilgisi.MaxLength = 11;
            }
            else
            {
                lblGirisBilgisi.Text = "Kullanıcı Adı";
                txtGirisBilgisi.MaxLength = 50;
            }

            txtGirisBilgisi.Clear();
            lblHata.Text = "";
            txtGirisBilgisi.Focus();
        }

        // ── GİRİŞ BİLGİSİ KEY PRESS ──────────────────────────────────────
        private void GirisBilgisiKeyPress(object sender, KeyPressEventArgs e)
        {
            // TC modunda sadece rakam
            if (rdoTC.Checked)
            {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                    e.Handled = true;
            }
        }

        // ── GİRİŞ YAP ─────────────────────────────────────────────────────
        private void btnGiris_Click(object sender, EventArgs e)
        {
            lblHata.Text = "";
            btnGiris.Enabled = false;
            btnGiris.Text = "Kontrol ediliyor...";

            try
            {
                // API'ye login isteği gönder
                var loginData = new
                {
                    girisBilgisi = txtGirisBilgisi.Text.Trim(),
                    sifre = txtSifre.Text
                };

                dynamic sonuc = ApiServis.Post<dynamic>("auth/login", loginData);

                if (sonuc != null && (bool)sonuc.basarili)
                {
                    // Personel bilgisini OturumBilgisi'ne kaydet
                    OturumBilgisi.AktifPersonel = new Entities.Personel
                    {
                        PerID = (int)sonuc.personel.perID,
                        PerAd = (string)sonuc.personel.perAd,
                        PerSoyad = (string)sonuc.personel.perSoyad,
                        RolID = (int)sonuc.personel.rolID,
                        RolAdi = (string)sonuc.personel.rolAdi,
                        TC = (string)sonuc.personel.tc
                    };

                    // Log kaydı API'ye gönder
                    try
                    {
                        ApiServis.Post<dynamic>("log", new
                        {
                            islemTuru = "GİRİŞ",
                            aciklama = $"{OturumBilgisi.AktifPersonel.PerAd} " +
                                        $"{OturumBilgisi.AktifPersonel.PerSoyad} giriş yaptı."
                        });
                    }
                    catch { }

                    frmAnaSayfa anaForm = new frmAnaSayfa();
                    anaForm.Show();
                    this.Hide();
                }
                else
                {
                    lblHata.Text = sonuc?.mesaj ?? "Giriş başarısız!";
                    txtSifre.Clear();
                    txtSifre.Focus();
                    btnGiris.Enabled = true;
                    btnGiris.Text = "Giriş Yap";
                }
            }
            catch (Exception ex)
            {
                lblHata.Text = "API bağlantı hatası: " + ex.Message;
                btnGiris.Enabled = true;
                btnGiris.Text = "Giriş Yap";
            }
        }
    }
}