// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssRequestContextCaptureHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Threading;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VssRequestContextCaptureHandler : DelegatingHandler
  {
    private IVssRequestContext m_requestContext;
    private bool m_isConfigureAwaitFeatureEnabled;
    private bool m_useDelegatedS2STokens;
    private bool m_disableDelegatedS2STokens;
    private bool m_isClientRateLimiterFeatureEnabled;

    public VssRequestContextCaptureHandler(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_isConfigureAwaitFeatureEnabled = this.m_requestContext.IsFeatureEnabled(FrameworkServerConstants.HandlerConfigureAwaitFeatureFlag);
      this.m_useDelegatedS2STokens = this.m_requestContext.IsFeatureEnabled(FrameworkServerConstants.UseDelegatedS2STokens);
      this.m_disableDelegatedS2STokens = this.m_requestContext.IsFeatureEnabled(FrameworkServerConstants.DisableDelegatedS2STokens);
      this.m_isClientRateLimiterFeatureEnabled = this.m_requestContext.IsFeatureEnabled(FrameworkServerConstants.HandlerHttpClientRateLimiterFeatureFlag);
    }

    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      request.Properties[TfsApiPropertyKeys.TfsRequestContextClient] = (object) this.m_requestContext;
      request.Properties[FrameworkServerConstants.HandlerConfigureAwaitFeatureFlag] = (object) this.m_isConfigureAwaitFeatureEnabled;
      request.Properties[FrameworkServerConstants.UseDelegatedS2STokens] = (object) this.m_useDelegatedS2STokens;
      request.Properties[FrameworkServerConstants.DisableDelegatedS2STokens] = (object) this.m_disableDelegatedS2STokens;
      request.Properties[FrameworkServerConstants.HandlerHttpClientRateLimiterFeatureFlag] = (object) this.m_isClientRateLimiterFeatureEnabled;
      if (cancellationToken != this.m_requestContext.CancellationToken)
        cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this.m_requestContext.CancellationToken).Token;
      Task<HttpResponseMessage> task;
      if (this.m_isConfigureAwaitFeatureEnabled)
      {
        using (new VssRequestSynchronizationContext(this.m_requestContext is VssRequestContext requestContext ? requestContext.RequestId : 0L, this.m_requestContext.ActivityId).ActivateOnCurrentThread())
          task = base.SendAsync(request, cancellationToken);
      }
      else
        task = base.SendAsync(request, cancellationToken);
      return task;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.m_requestContext = (IVssRequestContext) null;
      base.Dispose(disposing);
    }
  }
}
