// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.PackagingHttpClient
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class PackagingHttpClient
  {
    public static IHttpClient ForProtocol(IVssRequestContext requestContext, IProtocol protocol) => (IHttpClient) new HttpClientFacade(requestContext, PackagingHttpClient.ForProtocol(protocol));

    public static IRequestContextAwareHttpClient ForProtocol(IProtocol protocol) => (IRequestContextAwareHttpClient) new HttpClientWrapper(CoreHttpClients.NormalHttpClient, "PackagingHttpClient." + protocol.CorrectlyCasedName);

    public static IHttpClient ForProtocolNoRedirects(
      IVssRequestContext requestContext,
      IProtocol protocol)
    {
      return (IHttpClient) new HttpClientFacade(requestContext, PackagingHttpClient.ForProtocolNoRedirects(protocol));
    }

    public static IRequestContextAwareHttpClient ForProtocolNoRedirects(IProtocol protocol) => (IRequestContextAwareHttpClient) new HttpClientWrapper(CoreHttpClients.NoRedirectsHttpClient, "PackagingHttpClient." + protocol.CorrectlyCasedName + ".NoRedirects");
  }
}
