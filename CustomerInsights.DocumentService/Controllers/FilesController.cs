using CustomerInsights.DocumentService.Models;
using CustomerInsights.DocumentService.Services;
using CustomerInsights.DocumentService.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Xml;

namespace CustomerInsights.DocumentService.Controllers
{
    [ApiController]
    [Route("api/files")]
    public sealed class FilesController : ControllerBase
    {
        private const int PDF_SIZE = 104857600;
        private const int EML_SIZE = 52428800;
        private const int MEMORY_TRESHOLD = 8388608;
        private readonly PdfTextExtractionService _pdfExtractor;
        private readonly ZugferdExtractionService _zugferdExtractor;
        private readonly EmlReaderService _emlReader;

        public FilesController(PdfTextExtractionService pdfExtractor, ZugferdExtractionService zugferdExtractor, EmlReaderService emlReader)
        {
            _pdfExtractor = pdfExtractor;
            _zugferdExtractor = zugferdExtractor;
            _emlReader = emlReader;
        }

        [HttpPost("pdf/text")]
        [Produces("text/plain")]
        public async Task<IActionResult> ExtractPdfText(CancellationToken ct)
        {
            try
            {
                using UploadFile upload = await UploadStreamHelper.GetSingleFileAsync(Request, PDF_SIZE, new[] { ".pdf" }, MEMORY_TRESHOLD, ct);
                string text = await _pdfExtractor.ExtractTextAsync(upload.Content, ct);
                return Content(text, "text/plain; charset=utf-8");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("pdf/zugferd")]
        [Produces("application/json")]
        public async Task<IActionResult> ExtractZugferdXml(CancellationToken ct)
        {
            try
            {
                using UploadFile upload = await UploadStreamHelper.GetSingleFileAsync(Request, PDF_SIZE, new[] { ".pdf" }, MEMORY_TRESHOLD, ct);
                string xml = await _zugferdExtractor.ExtractInvoiceXmlAsync(upload.Content, ct);
                if (string.IsNullOrWhiteSpace(xml))
                    return BadRequest();

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                string json = JsonConvert.SerializeXmlNode(xmlDocument);
                return Content(json);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("eml")]
        [Produces("application/json")]
        public async Task<IActionResult> ReadEml(CancellationToken ct)
        {
            try
            {
                using UploadFile upload = await UploadStreamHelper.GetSingleFileAsync(
                    Request,
                    sizeLimitBytes: EML_SIZE,
                    permittedExtensions: new[] { ".eml" },
                    ct: ct);

                EmlMessage message = await _emlReader.ReadAsync(upload.Content, ct);
                return Ok(message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}