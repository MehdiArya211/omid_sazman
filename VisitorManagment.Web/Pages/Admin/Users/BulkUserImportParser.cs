using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Tesseract;
using VisitorManagment.Core.DTOs;
using UglyToad.PdfPig;

namespace VisitorManagment.Web.Pages.Admin.Users
{
    /// <summary>
    /// جدول کاربران را از فایل‌های Excel، Word و PDF متنی یا اسکن‌شده استخراج می‌کند.
    /// خروجی این کلاس همیشه پیش‌نمایش است و تا تأیید مدیر در بانک ثبت نمی‌شود.
    /// </summary>
    internal static class BulkUserImportParser
    {
        private const int MaximumRows = 1000;

        public static List<BulkUserImportRowViewModel> Parse(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new InvalidDataException("فایل انتخاب‌شده خالی است.");

            if (file.Length > 10 * 1024 * 1024)
                throw new InvalidDataException("حجم فایل نمی‌تواند بیشتر از ۱۰ مگابایت باشد.");

            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            using var stream = file.OpenReadStream();
            var rows = extension switch
            {
                ".xlsx" => ParseExcel(stream),
                ".docx" => ParseWord(stream),
                ".pdf" => ParsePdf(stream),
                _ => throw new InvalidDataException("فقط فایل‌های xlsx، docx و pdf قابل قبول هستند.")
            };

            if (!rows.Any())
                throw new InvalidDataException(extension == ".pdf"
                    ? "OCR فایل انجام شد، اما هیچ کد پرسنلی قابل اطمینانی در PDF پیدا نشد. کیفیت اسکن و خوانایی ستون شماره کارگزینی را بررسی کنید."
                    : "هیچ ردیف قابل تشخیصی در فایل پیدا نشد.");

            return rows.Take(MaximumRows).Select((row, index) =>
            {
                row.RowNumber = index + 1;
                return row;
            }).ToList();
        }

        private static List<BulkUserImportRowViewModel> ParseExcel(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            var sheet = workbook.Worksheets.FirstOrDefault();
            if (sheet == null) return new List<BulkUserImportRowViewModel>();

            var used = sheet.RangeUsed();
            if (used == null) return new List<BulkUserImportRowViewModel>();

            var headerRow = used.Rows().Take(10).FirstOrDefault(row =>
                row.Cells().Any(cell => IsPersonnelHeader(cell.GetString())));
            if (headerRow == null)
                throw new InvalidDataException("ستون «شماره کارگزینی/کد پرسنلی» در فایل Excel پیدا نشد.");

            var map = BuildHeaderMap(headerRow.Cells().Select(cell => cell.GetString()).ToList());
            return used.Rows()
                .Where(row => row.RowNumber() > headerRow.RowNumber())
                .Select(row => CreateRow(map, column => row.Cell(column + 1).GetFormattedString()))
                .Where(HasAnyValue)
                .ToList();
        }

        private static List<BulkUserImportRowViewModel> ParseWord(Stream stream)
        {
            using var document = WordprocessingDocument.Open(stream, false);
            var tables = document.MainDocumentPart?.Document?.Body?.Descendants<Table>() ?? Enumerable.Empty<Table>();
            var result = new List<BulkUserImportRowViewModel>();

            foreach (var table in tables)
            {
                var rows = table.Elements<TableRow>().ToList();
                var headerIndex = rows.FindIndex(row => row.Elements<TableCell>().Any(cell => IsPersonnelHeader(cell.InnerText)));
                if (headerIndex < 0) continue;

                var map = BuildHeaderMap(rows[headerIndex].Elements<TableCell>().Select(cell => cell.InnerText).ToList());
                result.AddRange(rows.Skip(headerIndex + 1)
                    .Select(row =>
                    {
                        var cells = row.Elements<TableCell>().Select(cell => cell.InnerText).ToList();
                        return CreateRow(map, column => column < cells.Count ? cells[column] : string.Empty);
                    })
                    .Where(HasAnyValue));
            }

            return result;
        }

        private static List<BulkUserImportRowViewModel> ParsePdf(Stream stream)
        {
            var result = new List<BulkUserImportRowViewModel>();
            using var document = PdfDocument.Open(stream);
            var pages = document.GetPages().ToList();

            foreach (var page in pages)
            {
                AddRowsFromRecognizedText(result, page.Text,
                    "اطلاعات این ردیف از متن PDF استخراج شده و باید پیش از ثبت کنترل شود.");
            }

            // PDF اسکن‌شده لایه متنی ندارد؛ تصویر اصلی هر صفحه با OCR فارسی/انگلیسی خوانده می‌شود.
            if (!result.Any())
            {
                var recognizedText = ReadScannedPdfWithOcr(pages);
                AddRowsFromRecognizedText(result, recognizedText,
                    "این ردیف با OCR فارسی از PDF اسکن‌شده خوانده شده است؛ کد پرسنلی را پیش از ثبت کنترل کنید.");
            }

            return result.GroupBy(row => row.PersonalCode).Select(group => group.First()).ToList();
        }

        /// <summary>
        /// تصویرهای بزرگ هر صفحه را OCR می‌کند. تصویرهای کوچک مانند مهر و لوگو کنار گذاشته می‌شوند.
        /// </summary>
        private static string ReadScannedPdfWithOcr(IEnumerable<UglyToad.PdfPig.Content.Page> pages)
        {
            var tessdataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");
            if (!File.Exists(Path.Combine(tessdataPath, "fas.traineddata")))
                throw new InvalidDataException("فایل زبان فارسی OCR در خروجی برنامه وجود ندارد. پروژه را دوباره Build/Publish کنید.");

            try
            {
                using var engine = new TesseractEngine(tessdataPath, "fas+eng", EngineMode.LstmOnly);
                var recognizedPages = new List<string>();

                foreach (var page in pages)
                {
                    var images = page.GetImages()
                        .Where(image => image.WidthInSamples >= 300 && image.HeightInSamples >= 300)
                        .OrderByDescending(image => (long)image.WidthInSamples * image.HeightInSamples)
                        .Take(3)
                        .ToList();

                    foreach (var image in images)
                    {
                        try
                        {
                            var bytes = image.TryGetPng(out var pngBytes)
                                ? pngBytes
                                : image.RawBytes.ToArray();
                            if (bytes == null || bytes.Length == 0) continue;

                            using var pix = Pix.LoadFromMemory(bytes);
                            using var ocrPage = engine.Process(pix, PageSegMode.Auto);
                            var text = ocrPage.GetText();
                            if (!string.IsNullOrWhiteSpace(text)) recognizedPages.Add(text);
                        }
                        catch
                        {
                            // ممکن است یک تصویر فرعی PDF فرمت نامتعارف داشته باشد؛ سایر تصاویر ادامه پیدا می‌کنند.
                        }
                    }
                }

                return string.Join(Environment.NewLine, recognizedPages);
            }
            catch (InvalidDataException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new InvalidDataException("موتور OCR روی سرور اجرا نشد. Visual C++ 2019 Runtime را نصب و دوباره تلاش کنید.", exception);
            }
        }

        private static void AddRowsFromRecognizedText(ICollection<BulkUserImportRowViewModel> rows,
            string text, string reviewMessage)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            var lines = Regex.Split(text, "[\\r\\n]+").Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
            foreach (var line in lines)
            {
                var candidates = ExtractNumericCandidates(line);
                var personalCode = candidates.FirstOrDefault(IsPersonnelCode);
                if (string.IsNullOrEmpty(personalCode)) continue;

                rows.Add(new BulkUserImportRowViewModel
                {
                    PersonalCode = personalCode,
                    Mobile = candidates.FirstOrDefault(IsMobile),
                    Status = "نیازمند استعلام و بازبینی",
                    ErrorMessage = reviewMessage
                });
            }

            // بعضی موتورهای PDF یا OCR شکست سطر را حفظ نمی‌کنند؛ کدها از کل متن نیز بازیابی می‌شوند.
            if (!rows.Any())
            {
                foreach (var code in ExtractNumericCandidates(text).Where(IsPersonnelCode).Distinct())
                {
                    rows.Add(new BulkUserImportRowViewModel
                    {
                        PersonalCode = code,
                        Status = "نیازمند استعلام و بازبینی",
                        ErrorMessage = reviewMessage
                    });
                }
            }
        }

        private static List<string> ExtractNumericCandidates(string value) =>
            Regex.Matches(NormalizeNumerals(value), @"(?<!\d)(?:\+?\d[\s\-]*){6,13}(?!\d)")
                .Cast<Match>()
                .Select(match => NormalizeDigits(match.Value))
                .Where(candidate => !string.IsNullOrWhiteSpace(candidate))
                .ToList();

        private static bool IsPersonnelCode(string value) =>
            !IsMobile(value) && Regex.IsMatch(value ?? string.Empty, @"^\d{6,10}$");

        private static Dictionary<string, int> BuildHeaderMap(IReadOnlyList<string> headers)
        {
            var map = new Dictionary<string, int>();
            for (var index = 0; index < headers.Count; index++)
            {
                var header = NormalizeText(headers[index]);
                if (IsPersonnelHeader(header)) map["personal"] = index;
                else if (header.Contains("نام و نشان") || header == "نام" || header.Contains("نام خانوادگی")) map["name"] = index;
                else if (header.Contains("درجه")) map["rank"] = index;
                else if (header.Contains("یگان")) map["unit"] = index;
                else if (header.Contains("موبایل") || header.Contains("همراه")) map["mobile"] = index;
            }
            return map;
        }

        private static BulkUserImportRowViewModel CreateRow(Dictionary<string, int> map, Func<int, string> valueAt)
        {
            var fullName = GetValue(map, "name", valueAt);
            SplitName(fullName, out var firstName, out var lastName);
            return new BulkUserImportRowViewModel
            {
                PersonalCode = NormalizeDigits(GetValue(map, "personal", valueAt)),
                FirstName = firstName,
                LastName = lastName,
                RankTitle = GetValue(map, "rank", valueAt),
                UnitTitle = GetValue(map, "unit", valueAt),
                Mobile = NormalizeDigits(GetValue(map, "mobile", valueAt)),
                Status = "در انتظار بررسی"
            };
        }

        private static string GetValue(Dictionary<string, int> map, string key, Func<int, string> valueAt) =>
            map.TryGetValue(key, out var index) ? NormalizeText(valueAt(index)) : string.Empty;

        private static bool HasAnyValue(BulkUserImportRowViewModel row) =>
            !string.IsNullOrWhiteSpace(row.PersonalCode) || !string.IsNullOrWhiteSpace(row.FirstName) ||
            !string.IsNullOrWhiteSpace(row.LastName) || !string.IsNullOrWhiteSpace(row.UnitTitle);

        private static bool IsPersonnelHeader(string value)
        {
            var header = NormalizeText(value);
            return header.Contains("کارگزینی") || header.Contains("پرسنلی");
        }

        private static bool IsMobile(string value) => Regex.IsMatch(value ?? string.Empty, @"^(?:\+?98|0)?9\d{9}$");

        private static void SplitName(string fullName, out string firstName, out string lastName)
        {
            var parts = NormalizeText(fullName).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            firstName = parts.FirstOrDefault() ?? string.Empty;
            lastName = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;
        }

        internal static string NormalizeText(string value) => Regex.Replace((value ?? string.Empty)
            .Replace('ي', 'ی').Replace('ك', 'ک').Replace('\u200c', ' ').Trim(), @"\s+", " ");

        internal static string NormalizeDigits(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            return NormalizeNumerals(value).Replace(" ", string.Empty).Replace("-", string.Empty);
        }

        private static string NormalizeNumerals(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            var result = value.Trim();
            const string persian = "۰۱۲۳۴۵۶۷۸۹";
            const string arabic = "٠١٢٣٤٥٦٧٨٩";
            for (var index = 0; index < 10; index++)
                result = result.Replace(persian[index], (char)('0' + index)).Replace(arabic[index], (char)('0' + index));
            return result;
        }
    }
}
