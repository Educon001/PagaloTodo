namespace UCABPagaloTodoMS.Authorization;

public static class AuthorizationPolicies
{
    public const string AdminPolicy = "AdminPolicy";
    public const string ConsumerPolicy = "ConsumerPolicy";
    public const string ProviderPolicy = "ProviderPolicy";
    public const string AdminOrConsumerPolicy = "AdminOrConsumerPolicy";
    public const string AdminOrProviderPolicy = "AdminOrProviderPolicy";
    public const string ConsumerOrProviderPolicy = "ConsumerOrProviderPolicy";
    public const string AllPolicies = "AllPolicies";
}