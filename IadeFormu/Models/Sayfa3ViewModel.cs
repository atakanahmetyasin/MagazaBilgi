using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IadeFormu.Models
{
    public class Sayfa3ViewModel
    {
        [Required(ErrorMessage = "Satış yapan personelin adı zorunludur.")]
        public string SatisYapanPersonel { get; set; }

        [Required(ErrorMessage = "İade alan personelin adı zorunludur.")]
        public string IadeAlanPersonel { get; set; }

        [Required(ErrorMessage = "Satış tarihi zorunludur.")]
        public DateTime SatisTarihi { get; set; }

        [Required(ErrorMessage = "İade tarihi zorunludur.")]
        public DateTime IadeTarihi { get; set; }

        [Required(ErrorMessage = "Satış tutarı girilmelidir.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Satış tutarı sıfırdan büyük olmalıdır.")]
        public decimal SatisTutari { get; set; }

        [Required(ErrorMessage = "İade tutarı girilmelidir.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "İade tutarı sıfırdan büyük olmalıdır.")]
        public decimal IadeTutari { get; set; }

        [Required(ErrorMessage = "İade edilen ürün grubu seçilmelidir.")]
        public List<string> IadeEdilenUrunGrubu { get; set; }

        [Required(ErrorMessage = "İade edilen ürün modeli zorunludur.")]
        public string IadeEdilenUrunModeli { get; set; }
    }
}
