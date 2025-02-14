using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IadeFormu.Models
{
    public class Sayfa4ViewModel
    {
        [IbanGerekli(
            "OdemeIadeSecenegi",
            ErrorMessage = "Havale/EFT seçildiğinde IBAN zorunludur."
        )]
        public string? IbanNumarasi { get; set; }

        [Required(ErrorMessage = "En az bir iade seçeneği seçmelisiniz.")]
        public  List<string> OdemeIadeSecenegi { get; set; }

        [Required(ErrorMessage = "Banka seçimi zorunludur.")]
        public List<string> BankaSecenegi { get; set; }
    }
}

public class IbanGerekliAttribute : ValidationAttribute
{
    private readonly string _odemeSecenegiProperty;

    public IbanGerekliAttribute(string odemeSecenegiProperty)
    {
        _odemeSecenegiProperty = odemeSecenegiProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var odemeSecenegi =
            validationContext
                .ObjectType.GetProperty(_odemeSecenegiProperty)
                ?.GetValue(validationContext.ObjectInstance) as List<string>;

        if (odemeSecenegi != null && odemeSecenegi.Contains("Havale / EFT"))
        {
            if (string.IsNullOrEmpty(value as string))
            {
                return new ValidationResult(ErrorMessage);
            }
        }
        return ValidationResult.Success;
    }
}
