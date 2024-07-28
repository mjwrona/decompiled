// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.OAuthConfigurationSecurityProvider
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public class OAuthConfigurationSecurityProvider : LibrarySecurityProvider
  {
    private static readonly string OAuthConfiguration = nameof (OAuthConfiguration);

    public static string GetToken(string configId) => configId == "0" ? OAuthConfigurationSecurityProvider.OAuthConfiguration + (object) LibrarySecurityProvider.NamespaceSeparator : OAuthConfigurationSecurityProvider.OAuthConfiguration + (object) LibrarySecurityProvider.NamespaceSeparator + configId;

    protected override string GetTokenSuffix(string configId) => OAuthConfigurationSecurityProvider.GetToken(configId);
  }
}
