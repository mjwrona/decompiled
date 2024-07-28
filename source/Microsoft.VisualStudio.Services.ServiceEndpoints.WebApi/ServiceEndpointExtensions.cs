// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  public static class ServiceEndpointExtensions
  {
    public static bool IsBasic(this EndpointAuthorization authorization) => authorization != null && string.Equals(authorization.Scheme, "UsernamePassword", StringComparison.OrdinalIgnoreCase);

    public static bool HasAuthorizationScheme(this ServiceEndpoint endpoint, string scheme) => string.Equals(scheme, endpoint.Authorization?.Scheme, StringComparison.InvariantCultureIgnoreCase);

    public static bool IsToken(this EndpointAuthorization authorization)
    {
      if (authorization == null)
        return false;
      return string.Equals(authorization.Scheme, "Token", StringComparison.OrdinalIgnoreCase) || string.Equals(authorization.Scheme, "PersonalAccessToken", StringComparison.OrdinalIgnoreCase) || string.Equals(authorization.Scheme, "OAuth", StringComparison.OrdinalIgnoreCase) || string.Equals(authorization.Scheme, "OAuthWrap", StringComparison.OrdinalIgnoreCase) || string.Equals(authorization.Scheme, "OAuth2", StringComparison.OrdinalIgnoreCase) || string.Equals(authorization.Scheme, "ApplicationOAuthToken", StringComparison.OrdinalIgnoreCase) || string.Equals(authorization.Scheme, "InstallationToken", StringComparison.OrdinalIgnoreCase);
    }

    public static bool ShouldAcceptUntrustedCertificates(this ServiceEndpoint serviceEndpoint)
    {
      if (serviceEndpoint == null)
        throw new ArgumentNullException(nameof (serviceEndpoint));
      string str;
      bool result = ((!serviceEndpoint.Data.TryGetValue("acceptUntrustedCerts", out str) ? 0 : (bool.TryParse(str, out result) ? 1 : 0)) & (result ? 1 : 0)) != 0;
      return result;
    }
  }
}
