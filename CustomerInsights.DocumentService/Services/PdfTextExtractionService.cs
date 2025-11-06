using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using PdfiumViewer;
using Tesseract;
using System.Drawing.Imaging;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
// using UglyToad.PdfPig; // entfernt

namespace CustomerInsights.DocumentService.Services
{
    public sealed class PdfTextExtractionService
    {
        private readonly string _tessdataPath;
        private readonly string _language;
        private readonly int _dpi;
        private readonly int _ocrTriggerMinChars;

        public PdfTextExtractionService(string tessdataPath, string language = "deu+eng", int dpi = 300, int ocrTriggerMinChars = 64)
        {
            _tessdataPath = tessdataPath ?? throw new ArgumentNullException(nameof(tessdataPath));
            _language = string.IsNullOrWhiteSpace(language) ? "deu+eng" : language;
            _dpi = dpi > 0 ? dpi : 300;
            _ocrTriggerMinChars = Math.Max(0, ocrTriggerMinChars);
        }

        // ehemals ExtractXmlAsync
        public async Task<string> ExtractTextAsync(Stream pdfStream, CancellationToken cancellationToken = default)
        {
            if (pdfStream is null) throw new ArgumentNullException(nameof(pdfStream));

            // 1) In MemoryStream kopieren, damit wir den Stream gefahrlos mehrfach verwenden können
            MemoryStream working = await CopyToMemoryAsync(pdfStream, cancellationToken).ConfigureAwait(false);
            working.Position = 0;

            // 2) Erstes: nativer Text-Extract (schnell)
            string extracted = await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var pdf = PdfDocument.Load(working); // verwendet die Memory-Kopie
                var sb = new StringBuilder();

                for (int i = 0; i < pdf.PageCount; i++)
                {
                    if ((i & 0x07) == 0) cancellationToken.ThrowIfCancellationRequested();
                    string pageText = pdf.GetPdfText(i); // PdfiumViewer-Extraktion
                    if (!string.IsNullOrEmpty(pageText))
                        sb.AppendLine(pageText);
                }

                return sb.ToString();
            }, cancellationToken).ConfigureAwait(false);

            // 3) Falls wenig/kein Text → OCR
            if (!NeedsOcr(extracted))
                return extracted;

            // OCR mit derselben Memory-Kopie
            working.Position = 0;
            return await OcrPdfAsync(working, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<MemoryStream> CopyToMemoryAsync(Stream source, CancellationToken ct)
        {
            var ms = new MemoryStream(source.CanSeek ? (int)Math.Min(source.Length, int.MaxValue) : 0);
            await source.CopyToAsync(ms, 81920, ct).ConfigureAwait(false);
            ms.Position = 0;
            return ms;
        }

        private bool NeedsOcr(string text)
        {
            return string.IsNullOrWhiteSpace(text) || text.Trim().Length < _ocrTriggerMinChars;
        }

        private Task<string> OcrPdfAsync(Stream pdfStream, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var doc = PdfDocument.Load(pdfStream);
                using var engine = new TesseractEngine(_tessdataPath, _language, EngineMode.Default)
                {
                    DefaultPageSegMode = PageSegMode.Auto
                };

                var sb = new StringBuilder(1024 * 64);

                for (int i = 0; i < doc.PageCount; i++)
                {
                    if ((i & 0x03) == 0) cancellationToken.ThrowIfCancellationRequested();

                    using var img = RenderPage(doc, i, _dpi);
                    // Direkter Weg: Bitmap → Pix (spart PNG-Umweg)
                    using var ms = new MemoryStream();
                    img.Save(ms, ImageFormat.Png);
                    ms.Position = 0;
                    using var pix = Pix.LoadFromMemory(ms.ToArray());
                    using var page = engine.Process(pix);
                    string pageText = page.GetText();
                    if (!string.IsNullOrEmpty(pageText))
                        sb.AppendLine(pageText);
                }

                return sb.ToString();
            }, cancellationToken);
        }

        private static Bitmap RenderPage(PdfDocument doc, int pageIndex, int dpi)
        {
            var size = doc.PageSizes[pageIndex];

            int width = (int)Math.Ceiling(size.Width / 72.0 * dpi);
            int height = (int)Math.Ceiling(size.Height / 72.0 * dpi);

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);

                Rectangle bounds = new Rectangle(0, 0, width, height);
                doc.Render(pageIndex, g, dpi, dpi, bounds, false);
            }

            return bmp;
        }

    }
}