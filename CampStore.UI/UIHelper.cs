using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// UIHelper.cs — Tüm formlarda kullanılacak ortak UI metodları
// Buton oluşturma, DataGridView ayarlama, renk sabitleri burada
using System.Drawing;
using System.Windows.Forms;

namespace CampStore.UI
{
    // UIHelper.cs'in en altına — namespace içinde ama UIHelper sınıfının dışına ekle

    // ── COMBOBOX YARDIMCI SINIFI ──────────────────────────────────────────
    // Tüm formlarda ComboBox'a ID+Ad çifti eklemek için kullanılır
    public class ComboItem
    {
        public int ID { get; set; }
        public string Ad { get; set; }
        public override string ToString() => Ad;
    }
    public static class UIHelper
    {
        // ── RENK SABİTLERİ ────────────────────────────────────────────────
        public static Color RenkArkaplan = Color.FromArgb(27, 42, 59);
        public static Color RenkPanel = Color.FromArgb(31, 52, 72);
        public static Color RenkMenuKoyu = Color.FromArgb(22, 34, 51);
        public static Color RenkYesil = Color.FromArgb(46, 204, 113);
        public static Color RenkMavi = Color.FromArgb(52, 152, 219);
        public static Color RenkKirmizi = Color.FromArgb(231, 76, 60);
        public static Color RenkTuruncu = Color.FromArgb(230, 126, 34);
        public static Color RenkGri = Color.FromArgb(127, 140, 141);
        public static Color RenkInput = Color.FromArgb(44, 62, 80);
        public static Color RenkSatirAlt = Color.FromArgb(22, 34, 51);

        // ── BUTON OLUŞTUR ─────────────────────────────────────────────────
        public static Button BtnOlustur(string text, Color renk,
            int x, int y, int w = 110, int h = 38)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(w, h),
                Location = new Point(x, y),
                BackColor = renk,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;

            // Hover efekti
            Color hoverRenk = ControlPaint.Dark(renk, 0.1f);
            btn.MouseEnter += (s, e) => btn.BackColor = hoverRenk;
            btn.MouseLeave += (s, e) => btn.BackColor = renk;

            return btn;
        }

        // ── TEXTBOX OLUŞTUR ───────────────────────────────────────────────
        public static TextBox TxtOlustur(int x, int y, int w = 200, int h = 32)
        {
            return new TextBox
            {
                Size = new Size(w, h),
                Location = new Point(x, y),
                Font = new Font("Segoe UI", 10f),
                BackColor = RenkInput,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        // ── LABEL OLUŞTUR ─────────────────────────────────────────────────
        public static Label LblOlustur(string text, int x, int y,
            bool kalin = true, int fontSize = 9)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", fontSize,
                            kalin ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = Color.White,
                Location = new Point(x, y),
                AutoSize = true
            };
        }

        // ── COMBOBOX OLUŞTUR ──────────────────────────────────────────────
        public static ComboBox CmbOlustur(int x, int y, int w = 200)
        {
            return new ComboBox
            {
                Size = new Size(w, 32),
                Location = new Point(x, y),
                Font = new Font("Segoe UI", 10f),
                BackColor = RenkInput,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
        }

        // ── DGV AYARLA ───────────────────────────────────────────────────
        public static void DgvAyarla(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 52, 72);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgv.ColumnHeadersHeight = 38;
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(27, 42, 59);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 204, 113);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.Padding = new Padding(3);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 34, 51);
            dgv.RowTemplate.Height = 36;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(44, 62, 80);
            // Mevcut ayarlar...
            dgv.EnableHeadersVisualStyles = false;
            // ... diğer ayarlar ...

            // Anchor ekle — form genişleyince grid de genişlesin
            dgv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                         AnchorStyles.Left | AnchorStyles.Right;
        }

        // ── DGV'YE İKON BUTON KOLONU EKLE ────────────────────────────────
        // Güncelle ve Sil için icon buton kolonları ekler
        public static void DgvIconButonEkle(DataGridView dgv)
        {
            // Güncelle kolonu
            DataGridViewButtonColumn btnGuncelle = new DataGridViewButtonColumn
            {
                Name = "colGuncelle",
                HeaderText = "",
                Text = "✏️",
                UseColumnTextForButtonValue = true,
                Width = 40,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle =
                {
                    BackColor   = Color.FromArgb(52, 152, 219),
                    ForeColor   = Color.White,
                    Font        = new Font("Segoe UI", 11f),
                    Padding     = new Padding(2)
                }
            };
            dgv.Columns.Add(btnGuncelle);

            // Sil kolonu
            DataGridViewButtonColumn btnSil = new DataGridViewButtonColumn
            {
                Name = "colSil",
                HeaderText = "",
                Text = "🗑️",
                UseColumnTextForButtonValue = true,
                Width = 40,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle =
                {
                    BackColor   = Color.FromArgb(231, 76, 60),
                    ForeColor   = Color.White,
                    Font        = new Font("Segoe UI", 11f),
                    Padding     = new Padding(2)
                }
            };
            dgv.Columns.Add(btnSil);
        }

        // ── BAŞLIK PANELİ OLUŞTUR ────────────────────────────────────────
        public static Panel BaslikPaneliOlustur(string baslik, int genislik)
        {
            Panel pnl = new Panel
            {
                Size = new Size(genislik, 65),
                Location = new Point(0, 0),
                BackColor = RenkMenuKoyu
            };

            // Sol çizgi aksanı
            Panel aksanCizgi = new Panel
            {
                Size = new Size(4, 65),
                Location = new Point(0, 0),
                BackColor = RenkYesil
            };
            pnl.Controls.Add(aksanCizgi);

            Label lbl = new Label
            {
                Text = baslik,
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 18),
                AutoSize = true
            };
            pnl.Controls.Add(lbl);

            return pnl;
        }

        // ── FORM AYARLA ───────────────────────────────────────────────────
        public static void FormAyarla(Form form, string baslik,
            int genislik, int yukseklik)
        {
            form.Text = baslik;
            form.Size = new Size(genislik, yukseklik);
            form.StartPosition = FormStartPosition.CenterScreen;
            form.BackColor = RenkArkaplan;
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.MaximizeBox = false;
            form.Font = new Font("Segoe UI", 9f);
        }
        // UIHelper.cs'e ekle — tüm formlar bu metodu çağırır
        public static void FormResizeAyarla(Form form, Panel pnlBaslik,
            Panel pnlAraBar, Panel pnlGrid, DataGridView dgv)
        {
            Action yeniden = () =>
            {
                int baslikY = 0;
                int baslikH = pnlBaslik.Height;
                int araBarH = pnlAraBar != null ? pnlAraBar.Height : 0;
                int icerikY = baslikH + araBarH;
                int icerikH = form.ClientSize.Height - icerikY;
                int genislik = form.ClientSize.Width;

                pnlBaslik.SetBounds(0, baslikY, genislik, baslikH);

                if (pnlAraBar != null)
                    pnlAraBar.SetBounds(0, baslikH, genislik, araBarH);

                pnlGrid.SetBounds(0, icerikY, genislik, icerikH);
                dgv.SetBounds(0, 0, pnlGrid.Width, pnlGrid.Height);
            };

            yeniden();
            form.Resize += (s, e) => yeniden();
        }
        // ── DASHBOARD KARTLARINI RESPONSIVE (DİNAMİK) YAPMA ───────────────────
        // Form büyüdüğünde kartların genişliğini ve aralıklarını otomatik hesaplar
        public static void DashboardKartlariHizala(Panel pnlIcerik, Control[] kartlar, int margin = 20)
        {
            if (kartlar == null || kartlar.Length == 0) return;

            int kartSayisi = kartlar.Length;

            // Toplam boşluk (En sol, kartların araları ve en sağdaki boşluklar)
            int toplamBosluk = margin * (kartSayisi + 1);

            // Her bir karta düşen net genişlik
            int kartGenisligi = (pnlIcerik.ClientSize.Width - toplamBosluk) / kartSayisi;

            int mevcutX = margin;
            int mevcutY = margin; // Yukarıdan bırakılacak boşluk

            foreach (var kart in kartlar)
            {
                // Kartın yeni genişliğini ve konumunu ayarla
                kart.Width = kartGenisligi;
                kart.Location = new Point(mevcutX, mevcutY);

                // Bir sonraki kartın başlayacağı X koordinatını hesapla
                mevcutX += kartGenisligi + margin;
            }
        }
        // UIHelper.cs'e ekle
        public static void PlaceholderEkle(TextBox txt, string placeholderMetin)
        {
            txt.Text = placeholderMetin;
            txt.ForeColor = Color.FromArgb(127, 140, 141);

            txt.Enter += (s, ev) =>
            {
                if (txt.Text == placeholderMetin)
                {
                    txt.Text = "";
                    txt.ForeColor = Color.White;
                }
            };

            txt.Leave += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = placeholderMetin;
                    txt.ForeColor = Color.FromArgb(127, 140, 141);
                }
            };
        }
    }
}
