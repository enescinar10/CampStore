using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// FaturaBL.cs — İş Katmanı
// Fatura işlemleri için iş kuralları burada uygulanır.
using System;
using System.Data;
using CampStore.DataAccessLayer;
using CampStore.Entities;

namespace CampStore.BusinessLayer
{
    public class FaturaBL
    {
        private FaturaDAL faturaDAL = new FaturaDAL();

        // ── EKLE ─────────────────────────────────────────────────────────
        public string FaturaEkle(Fatura f)
        {
            // İş kuralı 1: Satış seçilmeli
            if (f.SatisID <= 0)
                return "Geçerli bir satış seçiniz!";

            // İş kuralı 2: Fatura no boş olamaz
            if (string.IsNullOrWhiteSpace(f.FaturaNo))
                return "Fatura numarası boş bırakılamaz!";

            // İş kuralı 3: Tutar sıfırdan büyük olmalı
            if (f.Tutar <= 0)
                return "Fatura tutarı sıfırdan büyük olmalıdır!";

            // İş kuralı 4: Tarih bugünden ileri olamaz
            if (f.Tarih > DateTime.Now)
                return "Fatura tarihi bugünden ileri bir tarih olamaz!";

            try
            {
                faturaDAL.FaturaEkle(f);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        // ── GÜNCELLE ─────────────────────────────────────────────────────
        public string FaturaGuncelle(Fatura f)
        {
            if (string.IsNullOrWhiteSpace(f.FaturaNo))
                return "Fatura numarası boş bırakılamaz!";

            if (f.Tutar <= 0)
                return "Fatura tutarı sıfırdan büyük olmalıdır!";

            if (f.Tarih > DateTime.Now)
                return "Fatura tarihi bugünden ileri bir tarih olamaz!";

            try
            {
                faturaDAL.FaturaGuncelle(f);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        // ── SİL ──────────────────────────────────────────────────────────
        public string FaturaSil(int faturaID)
        {
            if (faturaID <= 0)
                return "Geçersiz fatura!";

            try
            {
                faturaDAL.FaturaSil(faturaID);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        // ── LİSTELE ──────────────────────────────────────────────────────
        public DataTable FaturaListele()
        {
            return faturaDAL.FaturaListele();
        }

        // ── ID'YE GÖRE GETİR ─────────────────────────────────────────────
        public Fatura FaturaGetirById(int faturaID)
        {
            return faturaDAL.FaturaGetirById(faturaID);
        }

        // ── SATIŞA GÖRE GETİR ────────────────────────────────────────────
        public Fatura FaturaGetirBySatis(int satisID)
        {
            return faturaDAL.FaturaGetirBySatis(satisID);
        }

        // ── OTOMATIK FATURA NO ÜRET ───────────────────────────────────────
        // FAT-20260422-001 formatında benzersiz fatura numarası üretir.
        public string FaturaNoUret()
        {
            return "FAT-" + DateTime.Now.ToString("yyyyMMdd") + "-" +
                   DateTime.Now.ToString("HHmmss");
        }
    }
}