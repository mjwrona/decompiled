// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssPriorityHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class VssPriorityHandler : DelegatingHandler
  {
    public VssPriorityHandler()
    {
    }

    public VssPriorityHandler(HttpMessageHandler innerHandler)
      : base(innerHandler)
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage requestMessage,
      CancellationToken cancellationToken)
    {
      IVssRequestContext requestContext = this.GetRequestContext(requestMessage);
      bool continueOnCapturedContext = false;
      requestMessage.Properties.TryGetValue<bool>(FrameworkServerConstants.HandlerConfigureAwaitFeatureFlag, out continueOnCapturedContext);
      if (requestContext != null)
      {
        this.AddPriorityHeader(requestContext, requestMessage);
        IVssHttpRetryInfo vssHttpRetryInfo;
        if (requestContext.RootContext.TryGetItem<IVssHttpRetryInfo>(RequestContextItemsKeys.HttpRetryInfo, out vssHttpRetryInfo))
          requestMessage.Properties["HttpRetryInfo"] = (object) vssHttpRetryInfo;
      }
      return await base.SendAsync(requestMessage, cancellationToken).ConfigureAwait(continueOnCapturedContext);
    }

    internal IVssRequestContext GetRequestContext(HttpRequestMessage requestMessage)
    {
      IVssRequestContext requestContext = (IVssRequestContext) null;
      object obj = (object) null;
      if (requestMessage.Properties.TryGetValue(TfsApiPropertyKeys.TfsRequestContextClient, out obj))
        requestContext = obj as IVssRequestContext;
      else if (HttpContext.Current != null)
        requestContext = (IVssRequestContext) HttpContext.Current.Items[(object) HttpContextConstants.IVssRequestContext];
      return requestContext;
    }

    internal void AddPriorityHeader(
      IVssRequestContext requestContext,
      HttpRequestMessage requestMessage)
    {
      requestMessage.Headers.Add("X-VSS-RequestPriority", requestContext.RequestPriority.ToString());
    }
  }
}
