using System.ComponentModel.DataAnnotations;

namespace IadeFormu.Models
{
    public class IadeFormuViewModel
    {
        // Adım 1 verileri
        public string? GonderilecekEposta { get; set; }
        public string? MagazaAdi { get; set; }

        // Adım 2 verileri
        public string? MusteriAdiSoyadi { get; set; }
        public string? MusteriTcKimlikNo { get; set; }

        // Adım 3 verileri
        public string? SatisYapanPersonel { get; set; }
        public string? IadeAlanPersonel { get; set; }
        public DateTime SatisTarihi { get; set; }
        public DateTime IadeTarihi { get; set; }
        public decimal SatisTutari { get; set; }
        public decimal IadeTutari { get; set; }
        public List<string>? IadeEdilenUrunGrubu { get; set; }

        public string? IadeEdilenUrunModeli { get; set; }

        // Adım 4 verileri (ödeme ve banka bilgileri)
        public List<string>? OdemeIadeSecenegi { get; set; }
        public List<string>? BankaSecenegi { get; set; }
        public string? IbanNumarasi { get; set; }

        // Adım 5 verileri (iade detayları ve belgeler)
        public string? IadeSebebiAciklama { get; set; }
        public bool UrunDepodanCiktiMi { get; set; }
        public bool UrunDepoyaIadeOlduMu { get; set; }
        public bool ServisIadeDurumu { get; set; }
        public List<IFormFile> Ekler { get; set; } = new List<IFormFile>();
    }
}
