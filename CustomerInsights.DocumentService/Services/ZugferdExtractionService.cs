using System.Text;
using PdfLexer;

namespace CustomerInsights.DocumentService.Services
{
    public sealed class ZugferdExtractionService
    {
        public Task<string> ExtractInvoiceXmlAsync(Stream pdfStream, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                using PdfDocument doc = PdfDocument.Open(pdfStream);
                PdfDictionary cat = doc.Catalog;

                // Suche in AF (Associated Files)
                if (cat.TryGetValue("AF", out var afObj))
                {
                    var afArray = afObj as PdfArray;
                    if (afArray != null)
                    {
                        foreach (var item in afArray)
                        {
                            var resolved = item.Resolve();
                            var filespec = resolved as PdfDictionary;
                            if (TryGetEmbeddedXmlFromFileSpec(doc, filespec, out var xml))
                                return xml;
                        }
                    }
                }

                // Suche in Names/EmbeddedFiles
                if (cat.TryGetValue("Names", out var namesObj))
                {
                    var namesDict = namesObj.Resolve() as PdfDictionary;
                    if (namesDict != null && namesDict.TryGetValue("EmbeddedFiles", out var efTreeObj))
                    {
                        var treeDict = efTreeObj.Resolve() as PdfDictionary;
                        if (treeDict != null)
                        {
                            if (TrySearchNameTreeForXml(doc, treeDict, out var xml))
                                return xml;
                        }
                    }
                }

                throw new InvalidOperationException("Keine ZUGFeRD/Factur-X XML im PDF gefunden.");
            }, cancellationToken);
        }

        private static bool TrySearchNameTreeForXml(PdfDocument doc, PdfDictionary node, out string xml)
        {
            xml = string.Empty;

            if (node.TryGetValue("Names", out var namesObj))
            {
                if (namesObj is PdfArray namesArr)
                {
                    for (int i = 0; i + 1 < namesArr.Count; i += 2)
                    {
                        PdfDictionary? value = namesArr[i + 1].Resolve() as PdfDictionary;
                        if (TryGetEmbeddedXmlFromFileSpec(doc, value, out xml))
                            return true;
                    }
                }
            }

            if (node.TryGetValue("Kids", out var kidsObj) && kidsObj is PdfArray kids)
            {
                foreach (var k in kids)
                {
                    PdfDictionary? child = k.Resolve() as PdfDictionary;
                    if (child != null && TrySearchNameTreeForXml(doc, child, out xml))
                        return true;
                }
            }

            return false;
        }

        private static bool TryGetEmbeddedXmlFromFileSpec(PdfDocument doc, PdfDictionary? fileSpec, out string xml)
        {
            xml = string.Empty;
            if (fileSpec == null)
                return false;

            if (fileSpec.TryGetValue("Type", out var typeObj) && typeObj is PdfName typeName)
            {
                if (typeName.Value.Equals("Filespec", StringComparison.OrdinalIgnoreCase) == false)
                {
                    // Trotzdem weitermachen, könnte trotzdem gültig sein
                }
            }

            string? name = null;
            if (fileSpec.TryGetValue("UF", out var ufObj))
            {
                if (ufObj is PdfString ufStr)
                    name = ufStr.Value;
            }
            if (name == null && fileSpec.TryGetValue("F", out var fObj))
            {
                if (fObj is PdfString fStr)
                    name = fStr.Value;
            }

            if (fileSpec.TryGetValue("EF", out var efObj) == false)
                return false;

            PdfDictionary? efDict = efObj.Resolve() as PdfDictionary;
            if (efDict == null)
                return false;

            // Stream holen (F bevorzugt, dann UF)
            PdfStream? efStream = null;
            if (efDict.TryGetValue("F", out var efF))
                efStream = efF.Resolve() as PdfStream;

            if (efStream == null && efDict.TryGetValue("UF", out var efUF))
                efStream = efUF.Resolve() as PdfStream;

            if (efStream == null)
                return false;

            bool looksXml = false;
            if (efStream.Dictionary.TryGetValue("Subtype", out var subtypeObj) && subtypeObj is PdfName subtypeName)
            {
                string subtypeValue = subtypeName.Value;
                looksXml = subtypeValue.Equals("XML", StringComparison.OrdinalIgnoreCase) ||
                           subtypeValue.Contains("xml", StringComparison.OrdinalIgnoreCase);
            }

            byte[] bytes = efStream.Contents.GetDecodedData();
            string text = DetectAndDecodeText(bytes);

            // Zusätzliche Prüfung anhand Name und Inhalt
            if (looksXml == false)
            {
                looksXml = IsLikelyZugferdXmlName(name) || IsLikelyZugferdXmlContent(text);
                if (looksXml == false)
                    return false;
            }

            xml = text;
            return true;
        }

        private static bool IsLikelyZugferdXmlName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            string n = name.ToLowerInvariant();
            return n.EndsWith(".xml") &&
                   (n.Contains("zugferd") || n.Contains("factur") ||
                    n.Contains("en16931") || n.Contains("cii") ||
                    n.Contains("invoice"));
        }

        private static bool IsLikelyZugferdXmlContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            return content.Contains("CrossIndustryInvoice", StringComparison.OrdinalIgnoreCase) ||
                   content.Contains("rsm:", StringComparison.Ordinal) ||
                   content.Contains("urn:cen.eu:en16931", StringComparison.OrdinalIgnoreCase) ||
                   content.Contains("factur-x", StringComparison.OrdinalIgnoreCase);
        }

        private static string DetectAndDecodeText(byte[] bytes)
        {
            if (bytes.Length == 0)
                return string.Empty;

            // UTF-8 BOM
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);

            // UTF-16 LE BOM
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
                return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);

            // UTF-16 BE BOM
            if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
                return Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2);

            // Standard: UTF-8 ohne BOM
            return Encoding.UTF8.GetString(bytes);
        }
    }
}