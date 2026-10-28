using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Docnet.Core;
using Docnet.Core.Models;
using Tesseract;
using VisitorManagment.Core.DTOs;

namespace VisitorManagment.Web.Pages.Admin.Users
{
    /// <summary>جدول خط‌کشی‌شده فارسی را از تصویر واقعی PDF، مستقل از نگاشت خراب فونت، می‌خواند.</summary>
    internal static class PdfUserTableReader
    {
        public static List<BulkUserImportRowViewModel> Read(Stream input)
        {
            using var memory = new MemoryStream();
            input.CopyTo(memory);
            using var document = DocLib.Instance.GetDocReader(memory.ToArray(), new PageDimensions(2400, 3200));
            if (document.GetPageCount() > 30)
                throw new InvalidDataException("حداکثر ۳۰ صفحه PDF در هر بار قابل پردازش است.");
            using var engine = new TesseractEngine(Path.Combine(AppContext.BaseDirectory, "tessdata"), "fas+eng", EngineMode.LstmOnly);
            var result = new List<BulkUserImportRowViewModel>();
            for (var pageIndex = 0; pageIndex < document.GetPageCount(); pageIndex++)
            {
                using var page = document.GetPageReader(pageIndex);
                var width = page.GetPageWidth();
                var height = page.GetPageHeight();
                var pixels = page.GetImage();
                // PDFium زمینه شفاف می‌دهد؛ پیش از تشخیص خطوط و OCR روی سفید ترکیب می‌شود.
                for (var offset = 0; offset < pixels.Length; offset += 4)
                {
                    var alpha = pixels[offset + 3];
                    for (var channel = 0; channel < 3; channel++)
                        pixels[offset + channel] = (byte)((pixels[offset + channel] * alpha + 255 * (255 - alpha)) / 255);
                    pixels[offset + 3] = 255;
                }
                var horizontal = FindLines(height, y => Enumerable.Range(0, width).Count(x => IsDark(pixels, width, x, y)) > width * .55);
                var vertical = FindLines(width, x => Enumerable.Range(0, height).Count(y => IsDark(pixels, width, x, y)) > height * .35);
                if (horizontal.Count < 2 || vertical.Count != 8)
                    throw new InvalidDataException("ساختار هفت‌ستونه جدول در صفحه " + (pageIndex + 1) + " تشخیص داده نشد؛ هیچ ردیفی از این فایل ثبت نشده است.");
                using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                var bits = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                try { Marshal.Copy(pixels, 0, bits.Scan0, pixels.Length); }
                finally { bitmap.UnlockBits(bits); }
                for (var rowIndex = 0; rowIndex < horizontal.Count - 1; rowIndex++)
                {
                    var cells = new List<string>();
                    for (var column = 0; column < 7; column++)
                    {
                        var bounds = new Rectangle(vertical[column] + 5, horizontal[rowIndex] + 5,
                            vertical[column + 1] - vertical[column] - 10, horizontal[rowIndex + 1] - horizontal[rowIndex] - 10);
                        if (bounds.Width < 1 || bounds.Height < 1) { cells.Add(""); continue; }
                        using var crop = bitmap.Clone(bounds, bitmap.PixelFormat);
                        using var png = new MemoryStream();
                        crop.Save(png, ImageFormat.Png);
                        using var pix = Pix.LoadFromMemory(png.ToArray());
                        using var recognized = engine.Process(pix, PageSegMode.SingleBlock);
                        cells.Add(BulkUserImportParser.NormalizeText(recognized.GetText()));
                    }
                    if (cells[2].Contains("کارگزینی") || cells[3].Contains("نشان")) continue;
                    var fullName = cells[3].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var code = Regex.Replace(BulkUserImportParser.NormalizeDigits(cells[2]), "[^0-9]", "");
                    result.Add(new BulkUserImportRowViewModel
                    {
                        PersonalCode = code,
                        FirstName = fullName.FirstOrDefault() ?? "",
                        LastName = string.Join(" ", fullName.Skip(1)),
                        RankTitle = cells[4], UnitTitle = cells[5],
                        Mobile = Regex.Replace(BulkUserImportParser.NormalizeDigits(cells[1]), "[^0-9+]", ""),
                        Selected = false, Status = "نیازمند بازبینی OCR",
                        ErrorMessage = "صفحه " + (pageIndex + 1) + "، ردیف جدول " + (rowIndex + 1) + ": کد پرسنلی و مشخصات را کنترل و سپس ردیف را انتخاب کنید."
                    });
                }
            }
            return result;
        }

        private static bool IsDark(byte[] data, int width, int x, int y) =>
            data[(y * width + x) * 4] < 140 && data[(y * width + x) * 4 + 1] < 140;

        /// <summary>ضخامت چندپیکسلی هر خط به یک مرز تبدیل می‌شود.</summary>
        private static List<int> FindLines(int length, Func<int, bool> isLine)
        {
            var lines = new List<int>();
            for (var index = 0; index < length; index++)
            {
                if (!isLine(index)) continue;
                var start = index;
                while (index + 1 < length && isLine(index + 1)) index++;
                lines.Add((start + index) / 2);
            }
            return lines;
        }
    }
}
