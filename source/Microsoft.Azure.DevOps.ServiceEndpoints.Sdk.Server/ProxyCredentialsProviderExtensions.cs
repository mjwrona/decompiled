// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ProxyCredentialsProviderExtensions
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System;
using System.Net;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public static class ProxyCredentialsProviderExtensions
  {
    public static ICredentials Get(this IProxyCredentialsProvider provider, string proxy) => Uri.IsWellFormedUriString(proxy, UriKind.Absolute) ? provider.Get(new Uri(proxy)) : throw new ArgumentException("Passed proxy url " + proxy + " is not well formated absolute proxy.", nameof (proxy));
  }
}
