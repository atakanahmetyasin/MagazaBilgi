using System.ComponentModel.DataAnnotations;

namespace IadeFormu.Models
{
    public class Sayfa2ViewModel
    {
        [Required(ErrorMessage = "Müşteri adı ve soyadı zorunludur.")]
        public string MusteriAdiSoyadi { get; set; }

        [Required(ErrorMessage = "T.C. kimlik numarası zorunludur.")]
        [StringLength(
            11,
            MinimumLength = 11,
            ErrorMessage = "T.C. kimlik numarası 11 haneli olmalıdır."
        )]
        [RegularExpression(
            "^[0-9]*$",
            ErrorMessage = "T.C. kimlik numarası sadece rakamlardan oluşmalıdır."
        )]
        public string MusteriTcKimlikNo { get; set; }
    }
}
