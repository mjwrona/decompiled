// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointAuthorizationSchemes
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public sealed class EndpointAuthorizationSchemes
  {
    public const string AzureStorage = "AzureStorage";
    public const string OAuth = "OAuth";
    public const string OAuth2 = "OAuth2";
    public const string OAuthWrap = "OAuthWrap";
    public const string Certificate = "Certificate";
    public const string UsernamePassword = "UsernamePassword";
    public const string Token = "Token";
    public const string PersonalAccessToken = "PersonalAccessToken";
    public const string ServicePrincipal = "ServicePrincipal";
    public const string WorkloadIdentityFederation = "WorkloadIdentityFederation";
    public const string None = "None";
    public const string Jwt = "JWT";
    public const string InstallationToken = "InstallationToken";
  }
}
