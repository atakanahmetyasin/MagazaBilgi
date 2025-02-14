using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using IadeFormu.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace IadeFormu.Controllers
{
    public class DemirbasController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ThankYou()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(DemirbasFormuViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Dosyalar", "Lütfen en az bir dosya ekleyin.");
                Console.WriteLine("Hata2");

                return View(model);
            }

            if (model.Dosyalar == null || model.Dosyalar.Count == 0)
            {
                ModelState.AddModelError("Dosyalar", "En az bir dosya yüklemeniz gerekmektedir.");
                Console.WriteLine("Hata");
                return View(model);
            }

            byte[] excelDosyasi = ExcelDosyasiOlustur(model);
            await MailGonder(model, excelDosyasi);

            return RedirectToAction("ThankYou");
        }

        private byte[] ExcelDosyasiOlustur(DemirbasFormuViewModel model)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("DemirbasKaydi");

            // Başlıkları ekle
            var headers = new[]
            {
                "Mağaza",
                "Tarih",
                "Sözleşme No",
                "Stok Kod",
                "Ürün Adı",
                "Adet",
                "Açıklama",
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }

            int row = 2;
            foreach (var urun in model.Urunler)
            {
                worksheet.Cells[row, 1].Value = model.Magaza;
                worksheet.Cells[row, 2].Value = model.Tarih.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 3].Value = model.SozlesmeNo;
                worksheet.Cells[row, 4].Value = urun.StokKod;
                worksheet.Cells[row, 5].Value = urun.UrunAdi;
                worksheet.Cells[row, 6].Value = urun.Adet;
                worksheet.Cells[row, 7].Value = urun.Aciklama;
                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return package.GetAsByteArray();
        }

        private async Task MailGonder(DemirbasFormuViewModel model, byte[] excelDosyasi)
        {
            var message = new MimeMessage();
            message.From.Add(
                new MailboxAddress(
                    $"Demirbaş Ürün Kaydı - {model.Magaza}",
                    "magazabilgi@caglarteknoloji.com.tr"
                )
            );
            message.To.Add(new MailboxAddress("Alıcı", "mustafa.eren@caglarteknoloji.com.tr"));
            message.Cc.Add(new MailboxAddress("CC", model.DolduranKisiEposta));
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

            message.Subject = $"Demirbaş Kaydı - {model.SozlesmeNo}";

            // Mail içeriği için ürün tablosu oluştur
            string urunTablosu = "";
            foreach (var urun in model.Urunler)
            {
                urunTablosu +=
                    $@"
            <tr>
             <td style='border: 1px solid #ddd; padding: 8px;'>{model.Magaza}</td>
              <td style='border: 1px solid #ddd; padding: 8px;'>{model.Tarih}</td>
               <td style='border: 1px solid #ddd; padding: 8px;'>{model.SozlesmeNo}</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>{urun.StokKod}</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>{urun.UrunAdi}</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>{urun.Adet}</td>
                 <td style='border: 1px solid #ddd; padding: 8px;'>{urun.Aciklama}</td>
            </tr>";
            }

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody =
                    $@"

        <h4>Ürün Bilgileri</h4>
        <table style='border-collapse: collapse; width: 100%; font-family: Arial, sans-serif;'>
            <tr style='background-color: #f2f2f2;'>
            <th style='border: 1px solid #ddd; padding: 8px;'>Mağaza</th>
            <th style='border: 1px solid #ddd; padding: 8px;'>Tarih</th>
            <th style='border: 1px solid #ddd; padding: 8px;'>Sözleşme No</th>
                <th style='border: 1px solid #ddd; padding: 8px;'>Stok Kod</th>
                <th style='border: 1px solid #ddd; padding: 8px;'>Ürün Adı</th>
                <th style='border: 1px solid #ddd; padding: 8px;'>Adet</th>
                <th style='border: 1px solid #ddd; padding: 8px;'>Aciklama</th>
            </tr>
            {urunTablosu}
        </table>",
            };

            // Excel dosyasını ekle
            bodyBuilder.Attachments.Add(
                "DemirbasKaydi.xlsx",
                excelDosyasi,
                new ContentType(
                    "application",
                    "vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                )
            );

            // Ek dosyaları ekle
            foreach (var dosya in model.Dosyalar)
            {
                if (dosya.Length > 0)
                {
                    using var stream = new MemoryStream();
                    await dosya.CopyToAsync(stream);
                    bodyBuilder.Attachments.Add(dosya.FileName, stream.ToArray());
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
            await client.SendAsync(message);
            client.Disconnect(true);
        }
    }
}
