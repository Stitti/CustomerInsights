using System.Text.Json;

namespace CustomerInsights.ApiService.Utilities
{
    public static class JsonPick
    {
        // unterstützt einfache Pfade wie "$.id", "$.a.b", "$.items[0].id"
        public static JsonElement? TryGet(JsonElement root, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return root;

            if (path.StartsWith("$.") == false)
                throw new ArgumentException("Pfad muss mit $. beginnen");

            JsonElement cur = root;
            string[] segments = path[2..].Split('.');
            foreach (var raw in segments)
            {
                string value = raw;
                int? idx = null;
                int bi = raw.IndexOf('[');
                if (bi >= 0 && raw.EndsWith("]"))
                {
                    value = raw[..bi];
                    idx = int.Parse(raw[(bi + 1)..^1]);
                }

                if (value.Length > 0)
                {
                    if (cur.TryGetProperty(value, out var next) == false)
                        return null;

                    cur = next;
                }
                if (idx != null)
                {
                    if (cur.ValueKind != JsonValueKind.Array) return null;
                    if (cur.GetArrayLength() <= idx) return null;
                    cur = cur[idx.Value];
                }
            }
            return cur;
        }

        public static string? TryGetString(JsonElement root, string path)
        {
            return TryGet(root, path) is { } e
                ? e.ValueKind switch {
                JsonValueKind.String => e.GetString(),
                JsonValueKind.Number => e.GetRawText(),
                JsonValueKind.True or JsonValueKind.False => e.GetRawText(),
                _ => e.GetRawText()
            }
            : null;
        }

        public static DateTime? TryGetDate(JsonElement root, string path)
        {
            return DateTime.TryParse(TryGetString(root, path), out var dt) ? dt : null;
        }
    }
}