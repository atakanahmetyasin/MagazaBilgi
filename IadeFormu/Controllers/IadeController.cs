using System.Drawing;
using System.IO;
using IadeFormu.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace IadeFormu.Controllers
{
    public class IadeFormuController : Controller
    {
        private static IadeFormuViewModel FormData = new IadeFormuViewModel();

        public IActionResult Sayfa1()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Sayfa1(Sayfa1ViewModel model)
        {
            if (ModelState.IsValid)
            {
                FormData.GonderilecekEposta = model.GonderilecekEposta;
                FormData.MagazaAdi = model.MagazaAdi;
                return RedirectToAction("Sayfa2");
            }
            return View(model);
        }

        public IActionResult Sayfa2()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Sayfa2(Sayfa2ViewModel model)
        {
            if (ModelState.IsValid)
            {
                FormData.MusteriAdiSoyadi = model.MusteriAdiSoyadi;
                FormData.MusteriTcKimlikNo = model.MusteriTcKimlikNo;
                return RedirectToAction("Sayfa3");
            }
            return View(model);
        }

        public IActionResult Sayfa3()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Sayfa3(Sayfa3ViewModel model)
        {
            if (ModelState.IsValid)
            {
                FormData.SatisYapanPersonel = model.SatisYapanPersonel;
                FormData.IadeAlanPersonel = model.IadeAlanPersonel;
                FormData.SatisTarihi = model.SatisTarihi;
                FormData.IadeTarihi = model.IadeTarihi;
                FormData.SatisTutari = model.SatisTutari;
                FormData.IadeTutari = model.IadeTutari;
                FormData.IadeEdilenUrunGrubu = model.IadeEdilenUrunGrubu;
                FormData.IadeEdilenUrunModeli = model.IadeEdilenUrunModeli;

                return RedirectToAction("Sayfa4");
            }
            return View(model);
        }

        public IActionResult Sayfa4()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Sayfa4(Sayfa4ViewModel model)
        {
            if (ModelState.IsValid)
            {
                FormData.OdemeIadeSecenegi = model.OdemeIadeSecenegi;
                FormData.BankaSecenegi = model.BankaSecenegi;
                FormData.IbanNumarasi = model.IbanNumarasi;
                return RedirectToAction("Sayfa5");
            }
            return View(model);
        }

        public IActionResult Sayfa5()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Sayfa5(Sayfa5ViewModel model)
        {
            if (ModelState.IsValid)
            {
                FormData.IadeSebebiAciklama = model.IadeSebebiAciklama;
                FormData.UrunDepodanCiktiMi = model.UrunDepodanCiktiMi;
                FormData.UrunDepoyaIadeOlduMu = model.UrunDepoyaIadeOlduMu;
                FormData.ServisIadeDurumu = model.ServisIadeDurumu;
                FormData.Ekler = model.Ekler;

                // Mail gönder ve kullanıcıyı teşekkür sayfasına yönlendir
                MailGonder();
                return RedirectToAction("Tesekkurler");
            }
            return View(model);
        }

        public IActionResult Tesekkurler()
        {
            return View();
        }

        private byte[] ExcelDosyasiOlustur()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("IadeFormu");

            // Başlık ve veri çiftleri (Gönderim tarihi ilk sırada olacak şekilde düzenledik)
            var data = new List<(string Başlık, string Değer)>
            {
                ("Gönderim Tarihi ve Saati", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")),
                ("Mağaza Adı", FormData.MagazaAdi),
                ("Müşteri Adı Soyadı", FormData.MusteriAdiSoyadi),
                ("T.C. Kimlik No", FormData.MusteriTcKimlikNo),
                ("Satış Yapan Personel", FormData.SatisYapanPersonel),
                ("İade Alan Personel", FormData.IadeAlanPersonel),
                ("Satış Tarihi", FormData.SatisTarihi.ToString("dd/MM/yyyy")),
                ("İade Tarihi", FormData.IadeTarihi.ToString("dd/MM/yyyy")),
                ("Satış Tutarı", FormData.SatisTutari.ToString("C")),
                ("İade Tutarı", FormData.IadeTutari.ToString("C")),
                (
                    "İade Edilen Ürün Grubu",
                    string.Join(", ", FormData.IadeEdilenUrunGrubu ?? new List<string>())
                ),
                ("İade Edilen Ürün Modeli", FormData.IadeEdilenUrunModeli),
                (
                    "Ödeme İade Seçeneği",
                    string.Join(", ", FormData.OdemeIadeSecenegi ?? new List<string>())
                ),
                (
                    "Banka Seçenekleri",
                    string.Join(", ", FormData.BankaSecenegi ?? new List<string>())
                ),
                (
                    "IBAN Numarası",
                    string.IsNullOrWhiteSpace(FormData.IbanNumarasi)
                        ? "Belirtilmedi"
                        : FormData.IbanNumarasi
                ),
                ("İade Sebebi Açıklama", FormData.IadeSebebiAciklama),
                ("Ürünün Depodan Çıkışı Oldu mu?", FormData.UrunDepodanCiktiMi ? "Evet" : "Hayır"),
                ("Ürün Depoya İade Oldu mu?", FormData.UrunDepoyaIadeOlduMu ? "Evet" : "Hayır"),
                ("Servis İade Durumu", FormData.ServisIadeDurumu ? "Evet" : "Hayır"),
            };

            // Başlıkları ilk satıra yaz
            for (int i = 0; i < data.Count; i++)
            {
                worksheet.Cells[1, i + 1].Value = data[i].Başlık;
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }

            // Değerleri ikinci satıra yaz
            for (int i = 0; i < data.Count; i++)
            {
                worksheet.Cells[2, i + 1].Value = data[i].Değer;
            }

            // Otomatik sütun genişliği
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Excel dosyasını byte dizisi olarak döndür
            return package.GetAsByteArray();
        }

        private void MailGonder()
        {
            var message = new MimeMessage();
            message.From.Add(
                new MailboxAddress(
                    $"{FormData.MagazaAdi} - {FormData.IadeAlanPersonel}",
                    "magazabilgi@caglarteknoloji.com.tr"
                )
            );
            message.To.Add(
                new MailboxAddress("Alıcı Adı", "mustafa.eren@caglarteknoloji.com.tr")
            );

            message.Cc.Add(new MailboxAddress("Bilgi", FormData.GonderilecekEposta));

            // Sabit CC e-posta adreslerini ekleme
            var ccEmails = new List<string>
            {
                "deniz.erguc@caglarteknoloji.com.tr",
                "mustafa.sagdemir@caglarteknoloji.com.tr",
                "oykum.uysal@caglarteknoloji.com.tr",
                "tugce.akdeniz@caglarteknoloji.com.tr",
                "cisem.gulcan@caglarteknoloji.com.tr",
            };

            foreach (var email in ccEmails)
            {
                message.Cc.Add(new MailboxAddress("CC", email));
            }

            message.Subject =
                $"İade Formu Bildirimi - {FormData.MagazaAdi} - {FormData.IadeAlanPersonel}";

            // Excel dosyasını oluştur
            byte[] excelFile = ExcelDosyasiOlustur();

            // E-posta içeriğini tablo formatında oluşturuyoruz
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody =
                    $@"
        <h3 style='color: #2c3e50;'>İade Formu Detayları</h3>
        <table border='1' cellpadding='10' cellspacing='0' style='border-collapse: collapse; width: 100%; font-family: Arial, sans-serif;'>
            <tr style='background-color: #f2f2f2;'>
                <th style='text-align: left;'>Başlık</th>
                <th style='text-align: left;'>Değer</th>
            </tr>
            <tr>
                <td>Mağaza Adı</td>
                <td>{FormData.MagazaAdi}</td>
            </tr>
            <tr>
                <td>Müşteri Adı Soyadı</td>
                <td>{FormData.MusteriAdiSoyadi}</td>
            </tr>
            <tr>
                <td>Müşteri T.C. Kimlik No</td>
                <td>{FormData.MusteriTcKimlikNo}</td>
            </tr>
            <tr>
                <td>Satış Yapan Personel</td>
                <td>{FormData.SatisYapanPersonel}</td>
            </tr>
            <tr>
                <td>İade Alan Personel</td>
                <td>{FormData.IadeAlanPersonel}</td>
            </tr>
            <tr>
                <td>Satış Tarihi</td>
                <td>{FormData.SatisTarihi:dd/MM/yyyy}</td>
            </tr>
            <tr>
                <td>İade Tarihi</td>
                <td>{FormData.IadeTarihi:dd/MM/yyyy}</td>
            </tr>
            <tr>
                <td>Satış Tutarı (TL)</td>
                <td>{FormData.SatisTutari:C}</td>
            </tr>
            <tr>
                <td>İade Tutarı (TL)</td>
                <td>{FormData.IadeTutari:C}</td>
            </tr>
            <tr>
                <td>İade Edilen Ürün Grubu</td>
                <td>{string.Join(", ", FormData.IadeEdilenUrunGrubu)}</td>
            </tr>
            <tr>
                <td>İade Edilen Ürün Modeli</td>
                <td>{FormData.IadeEdilenUrunModeli}</td>
            </tr>
            <tr>
                <td>Ödeme İade Seçeneği</td>
                <td>{string.Join(", ", FormData.OdemeIadeSecenegi)}</td>
            </tr>
            <tr>
                <td>Banka Seçenekleri</td>
                <td>{string.Join(", ", FormData.BankaSecenegi)}</td>
            </tr>
            <tr>
                <td>IBAN Numarası</td>
                <td>{FormData.IbanNumarasi}</td>
            </tr>
            <tr>
                <td>İade Sebebi</td>
                <td>{FormData.IadeSebebiAciklama}</td>
            </tr>
            <tr>
                <td>Ürünün Depodan Çıkışı Oldu mu?</td>
                <td>{(FormData.UrunDepodanCiktiMi ? "Evet" : "Hayır")}</td>
            </tr>
            <tr>
                <td>Ürün Depoya İade Oldu mu?</td>
                <td>{(FormData.UrunDepoyaIadeOlduMu ? "Evet" : "Hayır")}</td>
            </tr>
            <tr>
                <td>Servis İade Durumu</td>
                <td>{(FormData.ServisIadeDurumu ? "Evet" : "Hayır")}</td>
            </tr>
        </table>",
            };

            // Excel dosyasını maile ekle
            bodyBuilder.Attachments.Add(
                "IadeFormu.xlsx",
                excelFile,
                new ContentType(
                    "application",
                    "vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                )
            );

            // Dosyaları bellekten okuyarak ekle
            foreach (var file in FormData.Ekler)
            {
                if (file.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    file.CopyTo(memoryStream);
                    memoryStream.Position = 0; // Stream'in başına dön

                    bodyBuilder.Attachments.Add(file.FileName, memoryStream.ToArray());
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            client.Connect(
                "smtp.yandex.com",
                465,
                MailKit.Security.SecureSocketOptions.SslOnConnect
            );
            client.Authenticate("magazabilgi@caglarteknoloji.com.tr", "pveyyltpqhoomvpt");
            client.Send(message);
            client.Disconnect(true);
        }
    }
}
