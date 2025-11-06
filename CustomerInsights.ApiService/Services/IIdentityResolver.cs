namespace CustomerInsights.ApiService.Services
{
    public interface IIdentityResolver
    {
        Guid? ResolveAccountIdFromEmail(string? email);
        Guid? ResolveContactIdFromEmail(string? email);
        Guid? ResolveThreadId(string source, string externalThreadId);
    }
}