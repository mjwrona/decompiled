// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssHttpMessageHandlerService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;
using System.Net.Http;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VssHttpMessageHandlerService : IVssHttpMessageHandlerService, IVssFrameworkService
  {
    private bool m_ownsProvider;
    private VssHttpMessageHandlerService.VssHttpErrorEventListener m_eventListener;
    private IVssHttpMessageHandlerProvider m_provider;
    private IDisposableReadOnlyList<IVssHttpMessageHandlerProvider> m_handlers;

    public virtual HttpMessageHandler GetHandler(
      IVssRequestContext requestContext,
      Uri baseUri,
      Guid targetServicePrincipal = default (Guid))
    {
      return this.m_provider.GetHandler(requestContext, baseUri, targetServicePrincipal);
    }

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (!this.m_ownsProvider)
        return;
      this.m_provider?.Dispose();
      this.m_handlers?.Dispose();
      this.m_eventListener?.Dispose();
    }

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        this.m_eventListener = new VssHttpMessageHandlerService.VssHttpErrorEventListener();
        this.m_handlers = systemRequestContext.GetExtensions<IVssHttpMessageHandlerProvider>();
        this.m_provider = this.CreateProvider(systemRequestContext, (IReadOnlyList<IVssHttpMessageHandlerProvider>) this.m_handlers);
        this.m_ownsProvider = true;
        this.m_provider.Initialize(systemRequestContext);
      }
      else
        this.m_provider = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<VssHttpMessageHandlerService>().m_provider;
    }

    protected virtual IVssHttpMessageHandlerProvider CreateProvider(
      IVssRequestContext requestContext,
      IReadOnlyList<IVssHttpMessageHandlerProvider> handlers)
    {
      if (handlers.Count > 1)
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Found more than one implementation of IVssHttpMessageHandlerProvider: {0}", (object) string.Join(", ", handlers.Select<IVssHttpMessageHandlerProvider, string>((Func<IVssHttpMessageHandlerProvider, string>) (x => x.GetType().FullName)))));
      if (handlers.Count == 1)
        return handlers.Single<IVssHttpMessageHandlerProvider>();
      return requestContext.ExecutionEnvironment.IsHostedDeployment ? (IVssHttpMessageHandlerProvider) new CloudHttpMessageHandlerProvider() : (IVssHttpMessageHandlerProvider) new VssHttpMessageHandlerProvider();
    }

    private class VssHttpErrorEventListener : EventListener
    {
      protected override void OnEventSourceCreated(EventSource eventSource)
      {
        base.OnEventSourceCreated(eventSource);
        if (!eventSource.Name.Equals("Microsoft-VSS-Http", StringComparison.Ordinal))
          return;
        this.EnableEvents(eventSource, EventLevel.Critical, (EventKeywords) 2);
      }

      protected override void OnEventWritten(EventWrittenEventArgs eventData)
      {
        if (eventData.EventId != 30)
          return;
        TeamFoundationTracingService.TraceRawAlwaysOn(1633921763, TraceLevel.Error, nameof (VssHttpErrorEventListener), "HttpOperation", eventData.Message);
      }
    }
  }
}
