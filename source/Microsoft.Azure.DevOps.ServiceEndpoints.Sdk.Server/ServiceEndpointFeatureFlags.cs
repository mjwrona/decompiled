// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointFeatureFlags
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public static class ServiceEndpointFeatureFlags
  {
    public const string ShareServiceEndpoints = "WebAccess.ServiceEndpoints.ShareAcrossProjects";
    public const string AllowDisablingServiceEndpoints = "ServiceEndpoints.AllowDisablingServiceEndpoints";
    public const string EnableAzureBoardsOAuthAppId = "ServiceEndpoints.EnableAzureBoardsOAuthAppId";
    public const string EnableAzurePipelinesMarketplaceAppId = "ServiceEndpoints.EnableAzurePipelinesMarketplaceAppId";
    public const string EnableAzureBoardsMarketplaceAppId = "ServiceEndpoints.EnableAzureBoardsMarketplaceAppId";
    public const string EnableCustomHeadersInExtensionDataSource = "ServiceEndpoints.EnableCustomHeadersInExtensionDataSource";
    public const string HonorIsReadyFlagForEndpoints = "ServiceEndpoints.HonorIsReadyFlagForEndpoints";
    public const string VerifyEndpointIpAddresses = "ServiceEndpoints.VerifyEndpointIpAddresses";
    public const string EnableNewServiceEndpointAPIs = "ServiceEndpoints.NewServiceEndpointAPIs";
    public const string ShareAcrossProjectsNewUx = "ServiceEndpoints.ShareAcrossProjectsNewUx";
    public const string GenerateInstallationAccessToken = "ServiceEndpoints.GenerateInstallationAccessToken";
    public const string AllowCreatingEndpointsInDraftState = "ServiceEndpoints.AllowCreatingEndpointsInDraftState";
    public const string UseHostIdInIdTokenIssuer = "ServiceEndpoints.UseHostIdInIdTokenIssuer";
    public const string EnableServiceEndpointsCheckRoleAlreadyExists = "ServiceEndpoints.EnableServiceEndpointsCheckRoleAlreadyExists";
    public const string ValidateRelativeUrls = "ServiceEndpoints.EndpointProxyValidateRelativeUrls";
    public const string ValidateRelativeUrlsDisableErrorLogging = "ServiceEndpoints.EndpointProxyValidateRelativeUrlsErrorLogging.Disabled";
    public const string EndpointProxyDNSPinning = "ServiceEndpoints.EndpointProxy.DNSPinning.Enabled";
    public const string EndpointProxyPreferIPv4 = "ServiceEndpoints.EndpointProxy.PreferIPv4.Enabled";
    public const string EnableARMSecretExpirationProvider = "ServiceEndpoints.EnableARMSecretExpirationProvider";
    public const string DisableJavaScriptSerializerForJwt = "ServiceEndpoints.DisableJavaScriptSerializerForJwt";
    public const string EnableSerializerTransitionMode = "ServiceEndpoints.EnableSerializerTransitionMode";
  }
}
