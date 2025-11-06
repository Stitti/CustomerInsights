namespace CustomerInsights.DocumentService.Models
{
    public sealed class UploadFile : IDisposable
    {
        public required Stream Content { get; init; }
        public required string FileName { get; init; }
        public required string ContentType { get; init; }
        public required long Length { get; init; }
        public required string Extension { get; init; }

        public bool IsTempFile { get; init; }
        public string? TempPath { get; init; }

        public void Dispose()
        {
            Content.Dispose();
            if (IsTempFile && string.IsNullOrEmpty(TempPath) == false && File.Exists(TempPath))
            {
                try
                {
                    File.Delete(TempPath);
                }
                catch
                {
                /* ignore */
                }
            }
        }
    }
}