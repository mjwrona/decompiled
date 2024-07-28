// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.NuGetRealNonForwardingUpstreamHttpClientFacade
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server
{
  public class NuGetRealNonForwardingUpstreamHttpClientFacade : 
    INonForwardingPublicUpstreamHttpClient,
    IRequestContextAwareHttpClient,
    IVssFrameworkService
  {
    private readonly IRequestContextAwareHttpClient implementation = UpstreamHttpClient.ForProtocolNoRedirects((IProtocol) Protocol.NuGet);

    public Task<HttpResponseMessage> SendAsync(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      HttpCompletionOption completionOption)
    {
      return this.implementation.SendAsync(requestContext, request, completionOption);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
