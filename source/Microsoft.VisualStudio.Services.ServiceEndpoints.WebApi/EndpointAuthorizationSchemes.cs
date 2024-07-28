// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointAuthorizationSchemes
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [GenerateAllConstants(null)]
  public sealed class EndpointAuthorizationSchemes
  {
    public const string AzureStorage = "AzureStorage";
    public const string ApplicationOAuth = "ApplicationOAuthToken";
    public const string OAuth = "OAuth";
    public const string OAuthWrap = "OAuthWrap";
    public const string OAuth2 = "OAuth2";
    public const string Certificate = "Certificate";
    public const string UsernamePassword = "UsernamePassword";
    public const string Token = "Token";
    public const string PersonalAccessToken = "PersonalAccessToken";
    public const string ServicePrincipal = "ServicePrincipal";
    public const string WorkloadIdentityFederation = "WorkloadIdentityFederation";
    public const string None = "None";
    public const string Jwt = "JWT";
    public const string Kubernetes = "Kubernetes";
    public const string ManagedServiceIdentity = "ManagedServiceIdentity";
    public const string InstallationToken = "InstallationToken";
    public const string JiraConnectApp = "JiraConnectApp";
    public const string PublishProfile = "PublishProfile";
  }
}
