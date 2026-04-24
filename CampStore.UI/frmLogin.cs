using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// frmLogin.cs — Giriş Ekranı
// Tüm kontroller kod ile oluşturulur, Designer kullanılmaz.

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

        // ── KONTROLLER ────────────────────────────────────────────────────
        private Panel pnlArkaplan, pnlKart;
        private Label lblLogo, lblBaslik, lblAltBaslik;
        private Label lblTC, lblSifre;
        private TextBox txtTC, txtSifre;
        private Button btnGiris;
        private Label lblHata;

        public frmLogin()
        {
            InitializeComponent();
            KontrolleriOlustur();
        }

        private void KontrolleriOlustur()
        {
            this.Text = "CampStore — Giriş";
            this.Size = new Size(480, 540);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(27, 42, 59);

            // ── ARKA PLAN PANELİ ──────────────────────────────────────────
            pnlArkaplan = new Panel
            {
                Size = new Size(480, 540),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(27, 42, 59)
            };
            this.Controls.Add(pnlArkaplan);

            // ── KART PANELİ ───────────────────────────────────────────────
            pnlKart = new Panel
            {
                Size = new Size(360, 390),
                Location = new Point(60, 70),
                BackColor = Color.White
            };
            pnlArkaplan.Controls.Add(pnlKart);

            // Köşeleri yuvarla
            pnlKart.Paint += (s, ev) =>
            {
                GraphicsPath yol = new GraphicsPath();
                int r = 20;
                Rectangle alan = pnlKart.ClientRectangle;
                yol.AddArc(alan.X, alan.Y, r, r, 180, 90);
                yol.AddArc(alan.Right - r, alan.Y, r, r, 270, 90);
                yol.AddArc(alan.Right - r, alan.Bottom - r, r, r, 0, 90);
                yol.AddArc(alan.X, alan.Bottom - r, r, r, 90, 90);
                yol.CloseAllFigures();
                pnlKart.Region = new Region(yol);
            };

            // ── LOGO ──────────────────────────────────────────────────────
            lblLogo = new Label
            {
                Text = "🏕️",
                Font = new Font("Segoe UI", 32f),
                ForeColor = Color.FromArgb(27, 42, 59),
                Location = new Point(145, 25),
                AutoSize = true
            };
            pnlKart.Controls.Add(lblLogo);

            // ── BAŞLIK ────────────────────────────────────────────────────
            lblBaslik = new Label
            {
                Text = "CampStore",
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 42, 59),
                Location = new Point(95, 85),
                AutoSize = true
            };
            pnlKart.Controls.Add(lblBaslik);

            lblAltBaslik = new Label
            {
                Text = "Yönetim Paneline Giriş",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.Gray,
                Location = new Point(100, 120),
                AutoSize = true
            };
            pnlKart.Controls.Add(lblAltBaslik);

            // ── TC KİMLİK ─────────────────────────────────────────────────
            lblTC = new Label
            {
                Text = "TC Kimlik No",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 42, 59),
                Location = new Point(30, 158),
                AutoSize = true
            };
            pnlKart.Controls.Add(lblTC);

            txtTC = new TextBox
            {
                Size = new Size(300, 35),
                Location = new Point(30, 178),
                Font = new Font("Segoe UI", 11f),
                BorderStyle = BorderStyle.FixedSingle,
                MaxLength = 11,
                BackColor = Color.FromArgb(245, 245, 245)
            };
            // Sadece rakam girilsin
            txtTC.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) && ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };
            pnlKart.Controls.Add(txtTC);

            // ── ŞİFRE ─────────────────────────────────────────────────────
            lblSifre = new Label
            {
                Text = "Şifre",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 42, 59),
                Location = new Point(30, 225),
                AutoSize = true
            };
            pnlKart.Controls.Add(lblSifre);

            txtSifre = new TextBox
            {
                Size = new Size(300, 35),
                Location = new Point(30, 245),
                Font = new Font("Segoe UI", 11f),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '●',
                BackColor = Color.FromArgb(245, 245, 245)
            };
            pnlKart.Controls.Add(txtSifre);

            // ── HATA MESAJI ───────────────────────────────────────────────
            lblHata = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(30, 288),
                Size = new Size(300, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlKart.Controls.Add(lblHata);

            // ── GİRİŞ BUTONU ──────────────────────────────────────────────
            btnGiris = new Button
            {
                Text = "Giriş Yap",
                Size = new Size(300, 45),
                Location = new Point(30, 315),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnGiris.FlatAppearance.BorderSize = 0;
            btnGiris.Click += btnGiris_Click;
            btnGiris.MouseEnter += (s, ev) =>
                btnGiris.BackColor = Color.FromArgb(39, 174, 96);
            btnGiris.MouseLeave += (s, ev) =>
                btnGiris.BackColor = Color.FromArgb(46, 204, 113);
            pnlKart.Controls.Add(btnGiris);

            // ── ALT BİLGİ ─────────────────────────────────────────────────
            new Label
            {
                Text = "© 2026 CampStore — Kamp Malzemeleri",
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(100, 120, 140),
                Location = new Point(90, 480),
                AutoSize = true,
                Parent = pnlArkaplan
            };

            // Enter tuşuyla giriş yapılsın
            this.AcceptButton = btnGiris;

            this.Load += frmLogin_Load;
        }

        // ── FORM YÜKLENINCE ───────────────────────────────────────────────
        private void frmLogin_Load(object sender, EventArgs e)
        {
            txtTC.Focus();
        }

        // ── GİRİŞ YAP BUTONU ─────────────────────────────────────────────
        private void btnGiris_Click(object sender, EventArgs e)
        {
            // Hata mesajını temizle
            lblHata.Text = "";
            btnGiris.Enabled = false;
            btnGiris.Text = "Kontrol ediliyor...";

            string mesaj;
            Personel personel = personelBL.LoginKontrol(
                txtTC.Text.Trim(),
                txtSifre.Text,
                out mesaj
            );

            if (mesaj == "OK" && personel != null)
            {
                // Oturumu kaydet
                OturumBilgisi.AktifPersonel = personel;

                // Ana sayfayı aç
                frmAnaSayfa anaForm = new frmAnaSayfa();
                anaForm.Show();

                // Login formunu gizle
                this.Hide();
            }
            else
            {
                // Hata mesajını göster
                lblHata.Text = mesaj;
                txtSifre.Clear();
                txtSifre.Focus();

                btnGiris.Enabled = true;
                btnGiris.Text = "Giriş Yap";
            }
        }
    }
}
