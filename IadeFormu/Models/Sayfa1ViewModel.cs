using System.ComponentModel.DataAnnotations;

namespace IadeFormu.Models
{
    public class Sayfa1ViewModel
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin.")]
        public string GonderilecekEposta { get; set; }

        [Required(ErrorMessage = "Mağaza adı zorunludur.")]
        public string MagazaAdi { get; set; }
    }
}
