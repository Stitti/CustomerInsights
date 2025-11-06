using CustomerInsights.DocumentService.Models;
using DotNext;
using DotNext.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace CustomerInsights.DocumentService.Utils
{
    public static class UploadStreamHelper
    {
        public static async Task<UploadFile> GetSingleFileAsync(HttpRequest request, long sizeLimitBytes, string[] permittedExtensions, long inMemoryThresholdBytes = 8 * 1024 * 1024, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            if (request.HasFormContentType)
            {
                var form = await request.ReadFormAsync(ct).ConfigureAwait(false);
                var file = form.Files.FirstOrDefault();
                if (file == null)
                    throw new InvalidOperationException("Keine Datei im Multipart-Request gefunden.");

                if (file.Length <= 0)
                    throw new InvalidOperationException("Leere Datei hochgeladen.");

                if (file.Length > sizeLimitBytes)
                    throw new InvalidOperationException($"Datei zu groß. Limit: {sizeLimitBytes} Bytes.");

                string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                ValidateExtension(ext, permittedExtensions);

                if (file.Length <= inMemoryThresholdBytes)
                {
                    MemoryStream ms = new MemoryStream((int)file.Length);
                    await file.CopyToAsync(ms, ct).ConfigureAwait(false);
                    ms.Position = 0;
                    return new UploadFile
                    {
                        Content = ms,
                        FileName = file.FileName,
                        ContentType = file.ContentType ?? "application/octet-stream",
                        Length = file.Length,
                        Extension = ext,
                        IsTempFile = false
                    };
                }
                else
                {
                    var temp = Path.GetTempFileName();
                    await using (var fs = File.Create(temp))
                        await file.CopyToAsync(fs, ct).ConfigureAwait(false);

                    FileStream read = new FileStream(temp, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20, FileOptions.DeleteOnClose | FileOptions.SequentialScan);
                    return new UploadFile
                    {
                        Content = read,
                        FileName = file.FileName,
                        ContentType = file.ContentType ?? "application/octet-stream",
                        Length = file.Length,
                        Extension = ext,
                        IsTempFile = true,
                        TempPath = temp
                    };
                }
            }
            else
            {
                // Raw body upload (e.g., application/pdf, message/rfc822)
                string contentType = request.ContentType ?? "application/octet-stream";
                var fileName = GetFileNameFromHeader(request) ?? InferFileNameFromContentType(contentType);
                string ext = Path.GetExtension(fileName).ToLowerInvariant();
                ValidateExtension(ext, permittedExtensions);

                long? length = request.ContentLength;
                if (length.HasValue && length.Value > sizeLimitBytes)
                    throw new InvalidOperationException($"Body zu groß. Limit: {sizeLimitBytes} Bytes.");

                if (length.HasValue && length.Value <= inMemoryThresholdBytes)
                {
                    MemoryStream ms = new MemoryStream(length.Value > 0 ? (int)length.Value : 0);
                    await request.Body.CopyToAsync(ms, ct).ConfigureAwait(false);
                    ms.Position = 0;
                    return new UploadFile
                    {
                        Content = ms,
                        FileName = fileName,
                        ContentType = contentType,
                        Length = ms.Length,
                        Extension = ext,
                        IsTempFile = false
                    };
                }
                else
                {
                    string temp = Path.GetTempFileName();
                    await using (var fs = File.Create(temp))
                        await request.Body.CopyToAsync(fs, ct).ConfigureAwait(false);

                    FileStream read = new FileStream(temp, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20, FileOptions.DeleteOnClose | FileOptions.SequentialScan);
                    return new UploadFile
                    {
                        Content = read,
                        FileName = fileName,
                        ContentType = contentType,
                        Length = read.Length,
                        Extension = ext,
                        IsTempFile = true,
                        TempPath = temp
                    };
                }
            }
        }

        private static void ValidateExtension(string ext, string[] permitted)
        {
            if (permitted.Length == 0)
                return;

            if (permitted.Contains(ext, StringComparer.OrdinalIgnoreCase) == false)
                throw new InvalidOperationException($"Dateiendung nicht erlaubt: {ext}");
        }

        private static string? GetFileNameFromHeader(HttpRequest request)
        {
            if(request.Headers.TryGetValue("Content-Disposition", out StringValues values))
    {
                throw new Exception("Content-Disposition header not found");
            }

            if (ContentDispositionHeaderValue.TryParse(values.First(), out ContentDispositionHeaderValue? cd) && cd != null)
            {
                return cd.FileNameStar.HasValue ? cd.FileNameStar.Value : cd.FileName.HasValue ? cd.FileName.Value : null;
            }

            return null;
        }

        private static string InferFileNameFromContentType(string contentType)
        {
            return contentType.ToLowerInvariant() switch
            {
                "application/pdf" => "upload.pdf",
                "message/rfc822" => "upload.eml",
                _ => "upload.bin"
            };
        }
    }
}