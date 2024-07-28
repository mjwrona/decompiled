// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Internal.HttpHeaders
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class HttpHeaders
  {
    public const string ActivityId = "ActivityId";
    public const string ETag = "ETag";
    public const string Referrer = "Referer";
    public const string TfsVersion = "X-TFS-Version";
    public const string TfsRedirect = "X-TFS-Redirect";
    public const string TfsException = "X-TFS-Exception";
    public const string TfsServiceError = "X-TFS-ServiceError";
    public const string TfsSessionHeader = "X-TFS-Session";
    public const string TfsSoapException = "X-TFS-SoapException";
    public const string TfsFedAuthRealm = "X-TFS-FedAuthRealm";
    public const string TfsFedAuthIssuer = "X-TFS-FedAuthIssuer";
    public const string TfsFedAuthRedirect = "X-TFS-FedAuthRedirect";
    public const string VssAuthorizationEndpoint = "X-VSS-AuthorizationEndpoint";
    public const string VssPageHandlers = "X-VSS-PageHandlers";
    public const string VssE2EID = "X-VSS-E2EID";
    public const string VssOrchestrationId = "X-VSS-OrchestrationId";
    public const string AuditCorrelationId = "X-VSS-Audit-CorrelationId";
    public const string VssOriginUserAgent = "X-VSS-OriginUserAgent";
    public const string VssSenderDeploymentId = "X-VSS-SenderDeploymentId";
    public const string TfsInstanceHeader = "X-TFS-Instance";
    public const string TfsVersionOneHeader = "X-VersionControl-Instance";
    public const string TfsImpersonate = "X-TFS-Impersonate";
    public const string TfsSubjectDescriptorImpersonate = "X-TFS-SubjectDescriptorImpersonate";
    public const string MsContinuationToken = "X-MS-ContinuationToken";
    public const string VssUserData = "X-VSS-UserData";
    public const string VssAgentHeader = "X-VSS-Agent";
    public const string VssAuthenticateError = "X-VSS-AuthenticateError";
    public const string VssReauthenticationAction = "X-VSS-ReauthenticationAction";
    public const string RequestedWith = "X-Requested-With";
    public const string VssRateLimitResource = "X-RateLimit-Resource";
    public const string VssRateLimitDelay = "X-RateLimit-Delay";
    public const string VssRateLimitLimit = "X-RateLimit-Limit";
    public const string VssRateLimitRemaining = "X-RateLimit-Remaining";
    public const string VssRateLimitReset = "X-RateLimit-Reset";
    public const string RetryAfter = "Retry-After";
    public const string VssGlobalMessage = "X-VSS-GlobalMessage";
    public const string VssRequestRouted = "X-VSS-RequestRouted";
    public const string VssUseRequestRouting = "X-VSS-UseRequestRouting";
    public const string VssResourceTenant = "X-VSS-ResourceTenant";
    public const string VssOverridePrompt = "X-VSS-OverridePrompt";
    public const string VssOAuthS2STargetService = "X-VSS-S2STargetService";
    public const string VssHostOfflineError = "X-VSS-HostOfflineError";
    public const string VssForceMsaPassThrough = "X-VSS-ForceMsaPassThrough";
    public const string VssRequestPriority = "X-VSS-RequestPriority";
    public const string VssClientAccessMapping = "X-VSS-ClientAccessMapping";
    public const string VssReadConsistencyLevel = "X-VSS-ReadConsistencyLevel";
    public const string VssDownloadTicket = "X-VSS-DownloadTicket";
    public const string IfModifiedSince = "If-Modified-Since";
    public const string Authorization = "Authorization";
    public const string Location = "Location";
    public const string ProxyAuthenticate = "Proxy-Authenticate";
    public const string WwwAuthenticate = "WWW-Authenticate";
    public const string AfdIncomingRouteKey = "X-FD-RouteKey";
    public const string AfdOutgoingRouteKey = "X-AS-RouteKey";
    public const string AfdIncomingEndpointList = "X-FD-RouteKeyApplicationEndpointList";
    public const string AfdOutgoingEndpointList = "X-AS-RouteKeyApplicationEndpointList";
    public const string AfdResponseRef = "X-MSEdge-Ref";
    public const string AfdIncomingClientIp = "X-FD-ClientIP";
    public const string AfdIncomingSocketIp = "X-FD-SocketIP";
    public const string ArrForwardedFor = "X-Arr-Forwarded-For";
    public const string ArrAuthorization = "X-Arr-Authorization";
    public const string AfdIncomingRef = "X-FD-Ref";
    public const string AfdIncomingEventId = "X-FD-EventId";
    public const string AfdIncomingEdgeEnvironment = "X-FD-EdgeEnvironment";
    public const string AfdOutgoingQualityOfResponse = "X-AS-QualityOfResponse";
    public const string AfdOutgoingClientIp = "X-MSEdge-ClientIP";
    public const string SmartRouterForwardedFor = "X-SmartRouter-Forwarded-For";
  }
}
