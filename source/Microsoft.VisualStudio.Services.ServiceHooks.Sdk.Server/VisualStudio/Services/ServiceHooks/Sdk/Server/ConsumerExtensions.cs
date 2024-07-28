// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.ConsumerExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public static class ConsumerExtensions
  {
    public static void SetConsumerUrl(
      this IEnumerable<Consumer> consumers,
      UrlHelper urlHelper,
      IVssRequestContext requestContext)
    {
      foreach (Consumer consumer in consumers)
        consumer.SetConsumerUrl(urlHelper, requestContext);
    }

    public static void SetConsumerUrl(
      this Consumer consumer,
      UrlHelper urlHelper,
      IVssRequestContext requestContext)
    {
      Guid consumersLocationId = ServiceHooksPublisherApiConstants.ConsumersLocationId;
      if (urlHelper == null)
        return;
      consumer.Url = urlHelper.RestLink(requestContext, consumersLocationId, (object) new
      {
        consumerId = consumer.Id
      });
      if (consumer.Actions != null)
        consumer.Actions.SetConsumerActionUrl(urlHelper, requestContext);
      consumer.Links = ServiceHooksLinksUtility.GetConsumerReferenceLinks(requestContext, consumer, urlHelper);
    }

    public static void SetConsumerActionUrl(
      this IEnumerable<ConsumerAction> consumerActions,
      UrlHelper urlHelper,
      IVssRequestContext requestContext)
    {
      foreach (ConsumerAction consumerAction in consumerActions)
        consumerAction.SetConsumerActionUrl(urlHelper, requestContext);
    }

    public static void SetConsumerActionUrl(
      this ConsumerAction consumerAction,
      UrlHelper urlHelper,
      IVssRequestContext requestContext)
    {
      Guid actionsLocationId = ServiceHooksPublisherApiConstants.ConsumerActionsLocationId;
      if (urlHelper == null)
        return;
      consumerAction.Url = urlHelper.RestLink(requestContext, actionsLocationId, (object) new
      {
        consumerId = consumerAction.ConsumerId,
        consumerActionId = consumerAction.Id
      });
      consumerAction.Links = ServiceHooksLinksUtility.GetConsumerActionReferenceLinks(requestContext, consumerAction, urlHelper);
    }

    public static InputDescriptor GetInputDescriptor(
      this Consumer consumer,
      string actionId,
      string inputId)
    {
      IEnumerable<InputDescriptor> inputDescriptors = (IEnumerable<InputDescriptor>) consumer.InputDescriptors;
      if (inputDescriptors != null)
      {
        InputDescriptor inputDescriptor = inputDescriptors.FirstOrDefault<InputDescriptor>((Func<InputDescriptor, bool>) (i => string.Equals(i.Id, inputId)));
        if (inputDescriptor != null)
          return inputDescriptor;
      }
      IEnumerable<ConsumerAction> actions = (IEnumerable<ConsumerAction>) consumer.Actions;
      if (actions != null)
      {
        ConsumerAction consumerAction = actions.FirstOrDefault<ConsumerAction>((Func<ConsumerAction, bool>) (a => string.Equals(actionId, a.Id)));
        if (consumerAction != null && consumerAction.InputDescriptors != null)
        {
          InputDescriptor inputDescriptor = consumerAction.InputDescriptors.FirstOrDefault<InputDescriptor>((Func<InputDescriptor, bool>) (i => string.Equals(i.Id, inputId)));
          if (inputDescriptor != null)
            return inputDescriptor;
        }
      }
      return (InputDescriptor) null;
    }

    public static List<Consumer> FilterByEventTypes(
      this IEnumerable<Consumer> allConsumers,
      IEnumerable<string> eventTypes)
    {
      List<Consumer> consumerList = new List<Consumer>();
      HashSet<string> eventTypes1 = new HashSet<string>(eventTypes);
      foreach (Consumer allConsumer in allConsumers)
      {
        List<ConsumerAction> consumerActionList = ConsumerExtensions.FilterByEventTypes(allConsumer.Actions, eventTypes1);
        if (consumerActionList.Count > 0)
        {
          allConsumer.Actions = (IList<ConsumerAction>) consumerActionList;
          consumerList.Add(allConsumer);
        }
      }
      return consumerList;
    }

    public static List<ConsumerAction> FilterByEventTypes(
      this IEnumerable<ConsumerAction> consumerActions,
      IEnumerable<string> eventTypes)
    {
      return ConsumerExtensions.FilterByEventTypes(consumerActions, new HashSet<string>(eventTypes));
    }

    public static List<ConsumerAction> FilterByEventTypes(
      this IEnumerable<ConsumerAction> consumerActions,
      HashSet<string> eventTypes)
    {
      List<ConsumerAction> consumerActionList = new List<ConsumerAction>();
      foreach (ConsumerAction consumerAction in consumerActions)
      {
        consumerAction.SupportedEventTypes = ((IEnumerable<string>) consumerAction.SupportedEventTypes).Where<string>((Func<string, bool>) (e => e == "*" || eventTypes.Contains(e))).ToArray<string>();
        if (consumerAction.SupportedEventTypes.Length != 0)
          consumerActionList.Add(consumerAction);
      }
      return consumerActionList;
    }
  }
}
