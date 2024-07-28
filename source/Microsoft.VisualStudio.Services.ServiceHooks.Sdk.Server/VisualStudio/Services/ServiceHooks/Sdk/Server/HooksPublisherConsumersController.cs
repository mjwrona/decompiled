// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.HooksPublisherConsumersController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  [VersionedApiControllerCustomName(Area = "hooks", ResourceName = "Consumers")]
  public class HooksPublisherConsumersController : ServiceHooksPublisherControllerBase
  {
    [HttpGet]
    [ClientExample("GET__hooks_consumers.json", null, null, null)]
    public IEnumerable<Consumer> ListConsumers(string publisherId = null)
    {
      IEnumerable<ServiceHooksPublisher> publishers = this.FindPublishers(publisherId);
      if (!publishers.Any<ServiceHooksPublisher>())
        return (IEnumerable<Consumer>) Array.Empty<Consumer>();
      IEnumerable<string> eventTypes = publishers.SelectMany<ServiceHooksPublisher, EventTypeDescriptor>((Func<ServiceHooksPublisher, IEnumerable<EventTypeDescriptor>>) (p => this.GetSupportedEvents(this.TfsRequestContext, p))).Select<EventTypeDescriptor, string>((Func<EventTypeDescriptor, string>) (e => e.Id));
      List<Consumer> consumers = publishers.First<ServiceHooksPublisher>().GetHooksService(this.TfsRequestContext).GetConsumers(this.TfsRequestContext).FilterByEventTypes(eventTypes);
      consumers.SetConsumerUrl(this.Url, this.TfsRequestContext);
      return (IEnumerable<Consumer>) consumers;
    }

    [ClientExample("GET__hooks_consumers__consumerId_.json", null, null, null)]
    public Consumer GetConsumer(string consumerId, string publisherId = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(consumerId, nameof (consumerId));
      IEnumerable<ServiceHooksPublisher> publishers = this.FindPublishers(publisherId);
      if (!publishers.Any<ServiceHooksPublisher>())
        throw new NoPublishersDefinedException();
      IEnumerable<string> eventTypes = publishers.SelectMany<ServiceHooksPublisher, EventTypeDescriptor>((Func<ServiceHooksPublisher, IEnumerable<EventTypeDescriptor>>) (p => this.GetSupportedEvents(this.TfsRequestContext, p))).Select<EventTypeDescriptor, string>((Func<EventTypeDescriptor, string>) (e => e.Id));
      Consumer consumer = publishers.First<ServiceHooksPublisher>().GetHooksService(this.TfsRequestContext).GetConsumer(this.TfsRequestContext, consumerId);
      consumer.Actions = (IList<ConsumerAction>) consumer.Actions.FilterByEventTypes(eventTypes);
      consumer.SetConsumerUrl(this.Url, this.TfsRequestContext);
      return consumer;
    }
  }
}
