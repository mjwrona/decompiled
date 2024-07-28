// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.ConsumerExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
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
      Guid consumersLocationId = ServiceHooksApiConstants.ConsumersLocationId;
      consumer.Url = urlHelper.RestLink(requestContext, consumersLocationId, (object) new
      {
        consumerId = consumer.Id
      });
      if (consumer.Actions == null)
        return;
      consumer.Actions.SetConsumerActionUrl(urlHelper, requestContext);
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
      Guid actionsLocationId = ServiceHooksApiConstants.ConsumerActionsLocationId;
      consumerAction.Url = urlHelper.RestLink(requestContext, actionsLocationId, (object) new
      {
        consumerId = consumerAction.ConsumerId,
        consumerActionId = consumerAction.Id
      });
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
  }
}
