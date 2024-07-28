// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.HooksPublisherPublisherEventTypesController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  [VersionedApiControllerCustomName(Area = "hooks", ResourceName = "EventTypes")]
  [ClientGroupByResource("publishers")]
  public class HooksPublisherPublisherEventTypesController : ServiceHooksPublisherControllerBase
  {
    [HttpGet]
    [ClientExample("GET__hooks_publishers__publisherId__eventTypes.json", null, null, null)]
    public IEnumerable<EventTypeDescriptor> ListEventTypes(string publisherId)
    {
      List<EventTypeDescriptor> supportedEvents = this.TfsRequestContext.GetService<ServiceHooksPublisherService>().GetPublisher(this.TfsRequestContext, publisherId).ToPublisherModel(this.TfsRequestContext).SupportedEvents;
      supportedEvents.SetEventTypeUrl(this.Url, this.TfsRequestContext);
      return (IEnumerable<EventTypeDescriptor>) supportedEvents;
    }

    [ClientExample("GET__hooks_publishers__publisherId__eventTypes__eventTypeId_.json", null, null, null)]
    public EventTypeDescriptor GetEventType(string publisherId, string eventTypeId)
    {
      EventTypeDescriptor eventType = this.TfsRequestContext.GetService<ServiceHooksPublisherService>().GetPublisher(this.TfsRequestContext, publisherId).ToPublisherModel(this.TfsRequestContext).SupportedEvents.FirstOrDefault<EventTypeDescriptor>((Func<EventTypeDescriptor, bool>) (e => string.Equals(e.Id, eventTypeId)));
      if (eventType == null)
        throw new PublisherEventTypeNotFoundException(publisherId, eventTypeId);
      eventType.SetEventTypeUrl(this.Url, this.TfsRequestContext);
      return eventType;
    }
  }
}
