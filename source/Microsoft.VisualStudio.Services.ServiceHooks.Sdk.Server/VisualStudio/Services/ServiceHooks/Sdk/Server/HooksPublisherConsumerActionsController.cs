// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.HooksPublisherConsumerActionsController
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
  [VersionedApiControllerCustomName(Area = "hooks", ResourceName = "Actions")]
  [ClientGroupByResource("consumers")]
  public class HooksPublisherConsumerActionsController : ServiceHooksPublisherControllerBase
  {
    [HttpGet]
    [ClientExample("GET__hooks_consumers__consumerId__actions.json", null, null, null)]
    public IEnumerable<ConsumerAction> ListConsumerActions(string consumerId, string publisherId = null)
    {
      IEnumerable<ServiceHooksPublisher> publishers = this.FindPublishers(publisherId);
      if (!publishers.Any<ServiceHooksPublisher>())
        return (IEnumerable<ConsumerAction>) Array.Empty<ConsumerAction>();
      IEnumerable<string> eventTypes = publishers.SelectMany<ServiceHooksPublisher, EventTypeDescriptor>((Func<ServiceHooksPublisher, IEnumerable<EventTypeDescriptor>>) (p => this.GetSupportedEvents(this.TfsRequestContext, p))).Select<EventTypeDescriptor, string>((Func<EventTypeDescriptor, string>) (e => e.Id));
      List<ConsumerAction> consumerActions = publishers.First<ServiceHooksPublisher>().GetHooksService(this.TfsRequestContext).GetConsumer(this.TfsRequestContext, consumerId).Actions.FilterByEventTypes(eventTypes);
      consumerActions.SetConsumerActionUrl(this.Url, this.TfsRequestContext);
      return (IEnumerable<ConsumerAction>) consumerActions;
    }

    [ClientExample("GET__hooks_consumers__consumerId__actions__consumerActionId_.json", null, null, null)]
    public ConsumerAction GetConsumerAction(
      string consumerId,
      string consumerActionId,
      string publisherId = null)
    {
      IEnumerable<ServiceHooksPublisher> publishers = this.FindPublishers(publisherId);
      if (!publishers.Any<ServiceHooksPublisher>())
        throw new NoPublishersDefinedException();
      IEnumerable<string> eventTypes = publishers.SelectMany<ServiceHooksPublisher, EventTypeDescriptor>((Func<ServiceHooksPublisher, IEnumerable<EventTypeDescriptor>>) (p => this.GetSupportedEvents(this.TfsRequestContext, p))).Select<EventTypeDescriptor, string>((Func<EventTypeDescriptor, string>) (e => e.Id));
      ConsumerAction consumerAction = publishers.First<ServiceHooksPublisher>().GetHooksService(this.TfsRequestContext).GetConsumer(this.TfsRequestContext, consumerId).Actions.FilterByEventTypes(eventTypes).FirstOrDefault<ConsumerAction>((Func<ConsumerAction, bool>) (a => string.Equals(a.Id, consumerActionId)));
      if (consumerAction == null)
        throw new ConsumerActionNotFoundException(consumerId, consumerActionId);
      consumerAction.SetConsumerActionUrl(this.Url, this.TfsRequestContext);
      return consumerAction;
    }
  }
}
