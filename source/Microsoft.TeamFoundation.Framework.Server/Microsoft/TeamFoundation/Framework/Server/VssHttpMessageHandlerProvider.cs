// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssHttpMessageHandlerProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net.Http;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class VssHttpMessageHandlerProvider : IVssHttpMessageHandlerProvider, IDisposable
  {
    public void Dispose()
    {
    }

    public void Initialize(IVssRequestContext requestContext)
    {
    }

    public HttpMessageHandler GetHandler(
      IVssRequestContext requestContext,
      Uri baseUri,
      Guid targetServicePrincipal = default (Guid))
    {
      return !requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.BypassLoopbackHandler) ? (HttpMessageHandler) WebApiConfiguration.GetHttpServer(requestContext) : (HttpMessageHandler) new VssHttpMessageHandler((VssCredentials) new WindowsCredential(), new VssHttpRequestSettings());
    }
  }
}
