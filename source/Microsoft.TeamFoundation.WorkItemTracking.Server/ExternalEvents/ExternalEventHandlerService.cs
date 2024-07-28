// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalEvents.ExternalEventHandlerService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalEvents
{
  public class ExternalEventHandlerService : IExternalEventHandlerService, IVssFrameworkService
  {
    private IEnumerable<IExternalEventHandler> externalEventHandlers;

    public void ServiceStart(IVssRequestContext requestContext) => this.externalEventHandlers = (IEnumerable<IExternalEventHandler>) requestContext.GetExtensions<IExternalEventHandler>(ExtensionLifetime.Service);

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public void HandleEvent(
      IVssRequestContext requestContext,
      IDictionary<string, string> headers,
      string json,
      EventSource eventSource)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(headers, nameof (headers));
      ArgumentUtility.CheckForNull<string>(json, nameof (json));
      IExternalEventHandler eventHandler = this.GetEventHandler(requestContext, headers, json);
      if (!eventHandler.ValidateEvent(requestContext, headers, json, eventSource))
        throw new InvalidPayloadException();
      eventHandler.HandleEvent(requestContext, headers, json);
    }

    private IExternalEventHandler GetEventHandler(
      IVssRequestContext requestContext,
      IDictionary<string, string> headers,
      string json)
    {
      foreach (IExternalEventHandler externalEventHandler in this.externalEventHandlers)
      {
        (bool SupportedEvent, bool SupportedHostType) = externalEventHandler.SupportsEvent(requestContext, headers, json);
        if (SupportedEvent & SupportedHostType)
          return externalEventHandler;
        if (SupportedEvent)
          throw new UnsupportedExternalEventHostType();
      }
      throw new UnsupportedExternalEventSourceException();
    }
  }
}
