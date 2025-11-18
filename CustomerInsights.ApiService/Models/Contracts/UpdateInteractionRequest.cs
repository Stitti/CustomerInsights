using CustomerInsights.ApiService.Patching;

namespace CustomerInsights.ApiService.Models.Contracts
{
    public class UpdateInteractionRequest
    {
        public string? Subject { get; set; }
        public PatchField<Guid?> ContactId { get; set; }
        public PatchField<Guid?> AccountId { get; set; }

        public TextInferencePatch? TextInference { get; set; }
    }

    public class TextInferencePatch
    {
        public EmotionPatch? Emotions { get; set; }
        public AspectPatch? Aspects { get; set; }
    }

    public class EmotionPatch
    {
        public string[] Add { get; set; } = Array.Empty<string>();
        public string[] Remove { get; set; } = Array.Empty<string>();
    }

    public class AspectPatch
    {
        public string[] Add { get; set; } = Array.Empty<string>();
        public string[] Remove { get; set; } = Array.Empty<string>();
    }

}
