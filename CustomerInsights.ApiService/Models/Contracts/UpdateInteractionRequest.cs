using CustomerInsights.ApiService.Patching;

namespace CustomerInsights.ApiService.Models.Contracts
{
    public sealed class UpdateInteractionRequest
    {
        public string? Subject { get; set; }
        public PatchField<Guid?> ContactId { get; set; }
        public PatchField<Guid?> AccountId { get; set; }

        public TextInferencePatch? TextInference { get; set; }
    }

    public sealed class TextInferencePatch
    {
        public EmotionPatch? Emotions { get; set; }
        public AspectPatch? Aspects { get; set; }
    }

    public sealed class EmotionPatch
    {
        public string[] Add { get; set; } = Array.Empty<string>();
        public string[] Remove { get; set; } = Array.Empty<string>();
    }

    public sealed class AspectPatch
    {
        public string[] Add { get; set; } = Array.Empty<string>();
        public string[] Remove { get; set; } = Array.Empty<string>();
    }

}
