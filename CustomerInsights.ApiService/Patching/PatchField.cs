namespace CustomerInsights.ApiService.Patching
{
    public readonly struct PatchField<T>
    {
        public bool IsSpecified { get; }
        public T? Value { get; }

        private PatchField(bool isSpecified, T? value)
        {
            IsSpecified = isSpecified;
            Value = value;
        }

        public static PatchField<T> From(T? value)
        {
            return new PatchField<T>(true, value);
        }

        public static implicit operator PatchField<T>(T? value)
        {
            return From(value);
        }

        public override string ToString()
        {
            return IsSpecified ? (Value is null ? "(specified null)" : $"(specified {Value})") : "(unspecified)";
        }
    }
}
