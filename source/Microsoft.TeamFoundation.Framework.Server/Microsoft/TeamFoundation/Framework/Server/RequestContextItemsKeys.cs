// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RequestContextItemsKeys
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class RequestContextItemsKeys
  {
    internal static readonly string SecurityEvaluatorKeyBase = "$SecurityEvaluator:";
    internal static readonly string SecurityEvaluationLogKey = "$SecurityEvaluationLogKey";
    public static readonly string ServicingOperationClass = "$ServicingOperationClass";
    internal static readonly string CurrentServicingOperation = "$CurrentServicingOperation";
    internal static readonly string LogItems = "$LogItems";
    internal static readonly string TfsImpersonate = "$TfsImpersonate";
    internal static readonly string Scope = "$Scp";
    internal static readonly string ClientId = "$ClientId";
    internal static readonly string HostAuthorizationId = "$HostAuthorizationId";
    internal static readonly string Realm = "$Realm";
    internal static readonly string ClientAccessMappingMonikers = "$ClientAccessMappingMonikers";
    internal static readonly string ClientAccessMapping = "$ClientAccessMapping";
    internal static readonly string ServiceHostRouteContext = "$ServiceHostRouteContext";
    internal static readonly string IgnoreOAuthEnabledChecks = "$IgnoreOAuthEnabledChecks";
    internal static readonly string AlternateAuthCredentialsContextKey = "$AlternateCredentials";
    public const string ExemptFromAuthenticationTypeCheckInDelegatedAuthorization = "ExemptFromAuthenticationTypeCheckInDelegatedAuthorization";
    internal static readonly string AadAccessTokenUsedAsAlternateAuthCredentialsContextKey = "$AadAccessTokenAsAlternateAuthCredentials";
    internal static readonly string AlternateAuthCredentialsIdentityCreatorContextKey = "$AlternateCredentialsIdentityCreator";
    internal static readonly string AlternateAuthCredentialsIdentityUpdaterContextKey = "$AlternateCredentialsIdentityUpdater";
    internal static readonly string AadAuthorizationCode = "$AadAuthorizationCode";
    internal static readonly string AadAuthorizationToken = "$AadAuthorizationToken";
    internal static readonly string ExtensionCache = "$Extensions";
    internal static readonly string AccessIntentCache = "$AccessIntent";
    internal static readonly string FeatureAvailabilityCache = "$FeatureAvailability";
    internal static readonly string CustomerStage = "$CustomerStage";
    internal static readonly string HttpRetryInfo = "$HttpRetryInfo";
    internal static readonly string ComponentInitInfo = "$ComponentInitInfo";
    internal static readonly string SkipHostLastUserAccessUpdate = "$SkipHostLastUserAccessUpdate";
    public static readonly string ForceHostLastUserAccessUpdate = "$ForceHostLastUserAccessUpdate";
    internal static readonly string BypassLoopbackHandler = "$BypassLoopbackHandler";
    public static readonly string AnonymousIdentifier = "$AnonymousIdentifier";
    public static readonly string HostProxyData = "$HostProxyData";
    internal static readonly string IncludePerformanceTimingsInResponse = "$IncludePerformanceTimingsInResponse";
    internal static readonly string IncludePerformanceTimingsInServerTimingHeader = "$IncludePerformanceTimingsInServerTimingHeader";
    internal static readonly string BypassTarpitting = "$BypassTarpitting";
    internal static readonly string ResourceUtilizationCounters = "$ResourceUtilizationCounters";
    internal static readonly string ResourceUtilizationEvents = "$ResourceUtilizationEvents";
    internal static readonly string ThrottleInfo = "$ThrottleInfo";
    internal static readonly string ThrottleInfo2 = "$ThrottleInfo2";
    internal static readonly string ThrottleReason = "$ThrottleReason";
    internal static readonly string RUCompatibleLicense = "$RUCompatibleLicense";
    internal static readonly string SpsDefinition = "$SpsDefinition";
    internal static readonly string TrackDatabaseStatistics = "$TrackDatabaseStatistics";
    internal static readonly string IsProcessingAuthenticationModules = "$IsProcessingAuthenticationModules";
    internal static readonly string IdentityTracingItems = "$IdentityTracingItems";
    internal static readonly string AuthenticationMechanism = "$AuthenticationMechanism";
    internal static readonly string CanIssueUserAuthenticationToken = "$CanIssueUserAuthenticationToken";
    internal static readonly string CanIssueFedAuthToken = "$CanIssueFedAuthToken";
    internal static readonly string RedirectWritesOnReadOnlyDatabase = "$RedirectWritesOnReadOnlyDatabase";
    internal static readonly string CancellationReason = "$CancellationReason";
    internal static readonly string IsActivity = "$IsActivity";
    internal static readonly string TaskPoolName = "$TaskPoolName";
    public static readonly string GovernXEvents = "$GovernXEvents";
    internal static readonly string SecurityTracking = "$SecurityTracking";
    public static readonly string RequestCacheable = "$RequestCacheable";
    internal static readonly string IsPublicUser = "$IsPublicUser";
    internal static readonly string IsPublicResourceLicense = "$IsPublicResourceLicense";
    internal static readonly string IsOrgUser = "$IsOrgUser";
    public static readonly string UseDelegatedS2STokens = "$UseDelegatedS2STokens";
    public static readonly string AuthorizedClaimsIdentity = "$AuthorizedClaimsIdentity";
    public static readonly string IsPipelineIdentity = "$IsPipelineIdentity";
    public static readonly string IsProxyIdentity = "$IsProxyIdentity";
    internal static readonly string AuthorizationId = "$AuthorizationId";
    internal static readonly string OAuthAppId = "$OAuthAppId";
    internal static readonly string ValidatorIdentityError = "$ValidatorIdentityError";
    internal static readonly string EncodedIpAddress = "$EncodedIpAddress";
    internal static readonly string AllowAnonymousWrites = "$AllowAnonymousWrites";
    internal static readonly string AllowPublicUserWrites = "$AllowPublicUserWrites";
    internal static readonly string SupportsPublicAccess = "$SupportsPublicAccess";
    public static readonly string IsFrameworkIdentityReadPermissionCheckComplete = "$IsFrameworkIdentityReadPermissionCheckComplete";
    public static readonly string BypassDataspaceRestrictionForMembers = "$BypassDataspaceRestrictionForMembers";
    internal static readonly string AllowCrossDataspaceAccess = nameof (AllowCrossDataspaceAccess);
    internal static readonly string MinSequenceContext = "$MinSequenceContext";
    public static readonly string AuditLogCorrelationId = "$AuditLogCorrelationId";
    internal static readonly string IgnoreMembershipCache = "$IgnoreMembershipCache";
    public static readonly string BypassIdentityCacheWhenReadingByVsid = "$BypassIdentityCacheWhenReadingByVsid";
    public const string BypassIdentityCacheWhenReadingByDescriptor = "$BypassIdentityCacheWhenReadingByDescriptor";
    internal static readonly string RoutesMatchedExceptVersion = "$RoutesMatchedExceptVersion";
    internal static readonly string AzureSubscriptionId = "$AzureSubscriptionId";
    internal static readonly string ResourceManagerAadTenantId = "$ResourceManagerAadTenantId";
    internal static readonly string ResourceManagementUrl = "$ResourceManagementUrl";
    internal static readonly string ResourceGroupName = "$ResourceGroupName";
    internal static readonly string RedisException = "$RedisException";
    public static readonly string RequestUriSignatureValidated = "$RequestUriSignatureValidated";
    internal static readonly string ReadOnlyReplicaReadEnabled = nameof (ReadOnlyReplicaReadEnabled);
    public const string ReadConsistencyLevel = "ReadConsistencyLevel";
    public const string SmartRouterContext = "SmartRouterContext";
  }
}
