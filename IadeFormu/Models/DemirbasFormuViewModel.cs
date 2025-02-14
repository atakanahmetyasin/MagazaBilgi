using System.ComponentModel.DataAnnotations;

namespace IadeFormu.Models
{
    public class DemirbasFormuViewModel
    {
        [Required(ErrorMessage = "Dolduran kişi e-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string DolduranKisiEposta { get; set; }

        [Required(ErrorMessage = "Mağaza adı zorunludur.")]
        public string Magaza { get; set; }

        [Required(ErrorMessage = "Tarih zorunludur.")]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "Sözleşme numarası zorunludur.")]
        public string SozlesmeNo { get; set; }

        [Required(ErrorMessage = "En az bir ürün girilmelidir.")]
        public List<UrunBilgisi> Urunler { get; set; } = new List<UrunBilgisi>();

        public List<IFormFile> Dosyalar { get; set; } = new List<IFormFile>();
    }

    public class UrunBilgisi
    {
        [Required(ErrorMessage = "Stok kodu zorunludur.")]
        public string StokKod { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        public string UrunAdi { get; set; }

        [Required(ErrorMessage = "Adet girilmelidir.")]
        [Range(1, int.MaxValue, ErrorMessage = "Adet en az 1 olmalıdır.")]
        public int Adet { get; set; }

        [Required(ErrorMessage = "Açıklama girilmelidir.")]
        public string Aciklama { get; set; }
    }
}
