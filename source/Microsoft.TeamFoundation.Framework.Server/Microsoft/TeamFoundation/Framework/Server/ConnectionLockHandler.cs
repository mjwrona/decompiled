// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConnectionLockHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ConnectionLockHandler : DelegatingHandler
  {
    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      HttpResponseMessage httpResponseMessage;
      using (this.GetConnectionLock(request))
      {
        bool continueOnCapturedContext = false;
        request.Properties.TryGetValue<bool>(FrameworkServerConstants.HandlerConfigureAwaitFeatureFlag, out continueOnCapturedContext);
        httpResponseMessage = await base.SendAsync(request, cancellationToken).ConfigureAwait(continueOnCapturedContext);
      }
      return httpResponseMessage;
    }

    private IDisposable GetConnectionLock(HttpRequestMessage request)
    {
      object obj;
      if (request.Properties.TryGetValue(TfsApiPropertyKeys.TfsRequestContextClient, out obj))
      {
        IVssRequestContext context = obj as IVssRequestContext;
        if (context.ServiceHost != null && !context.ServiceHost.IsProduction)
          return context.AcquireConnectionLock(ConnectionLockNameType.REST);
      }
      return (IDisposable) null;
    }
  }
}
