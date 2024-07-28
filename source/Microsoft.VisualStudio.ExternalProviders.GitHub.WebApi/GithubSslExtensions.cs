// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GithubSslExtensions
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  public static class GithubSslExtensions
  {
    public static GitHubAuthentication ParseSslContext(
      this GitHubAuthentication authentication,
      IDictionary<string, string> data)
    {
      string str;
      bool result;
      if (data != null && data.TryGetValue("acceptUntrustedCerts", out str) && bool.TryParse(str, out result))
        authentication.AcceptUntrustedCertificates = result;
      return authentication;
    }
  }
}
