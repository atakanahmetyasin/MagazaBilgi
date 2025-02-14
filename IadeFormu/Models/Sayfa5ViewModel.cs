using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace IadeFormu.Models
{
    public class Sayfa5ViewModel
    {
        [Required(ErrorMessage = "İade sebebi açıklaması zorunludur.")]
        public string IadeSebebiAciklama { get; set; }

        [Required(ErrorMessage = "Ürünün depodan çıkış durumu belirtilmelidir.")]
        public bool UrunDepodanCiktiMi { get; set; }

        [Required(ErrorMessage = "Ürünün depoya iade durumu belirtilmelidir.")]
        public bool UrunDepoyaIadeOlduMu { get; set; }

        [Required(ErrorMessage = "Servis iade durumu belirtilmelidir.")]
        public bool ServisIadeDurumu { get; set; }

        [Required(ErrorMessage = "En az 3 dosya eklenmelidir.")]
        [MinEklerSayisi(
            3,
            ErrorMessage = "En az 3 dosya yüklenmelidir: Satış Formu, İade Formu ve Kredi Kartı Slip Görüntüsü."
        )]
        public List<IFormFile> Ekler { get; set; } = new List<IFormFile>();
    }

    public class MinEklerSayisiAttribute : ValidationAttribute
    {
        private readonly int _minCount;

        public MinEklerSayisiAttribute(int minCount)
        {
            _minCount = minCount;
        }

        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext
        )
        {
            var files = value as List<IFormFile>;
            if (files == null || files.Count < _minCount)
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
