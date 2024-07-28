// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Events.ServiceBusPublisher
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Events
{
  internal static class ServiceBusPublisher
  {
    private const string ResourceApiVersion = "1.0-preview.1";

    public static void PublishToServiceBus(
      IVssRequestContext requestContext,
      string messageBusName,
      string eventType,
      object payload)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (string.IsNullOrWhiteSpace(messageBusName))
        throw new ArgumentException("Message bus name missing", nameof (messageBusName));
      if (string.IsNullOrWhiteSpace(eventType))
        throw new ArgumentException("Event Type missing", nameof (eventType));
      if (payload == null)
        throw new ArgumentNullException(nameof (payload));
      ServiceEvent baseServiceEvent = ServiceBusPublisher.GetBaseServiceEvent(requestContext, eventType);
      baseServiceEvent.Resource = payload;
      ServiceBusPublisher.PublishToServiceBus(requestContext, messageBusName, new ServiceEvent[1]
      {
        baseServiceEvent
      });
    }

    private static ServiceEvent GetBaseServiceEvent(
      IVssRequestContext requestContext,
      string eventType)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Guid instanceType = vssRequestContext.ServiceInstanceType();
      ServiceInstanceType serviceInstanceType = vssRequestContext.GetService<IInstanceManagementService>().GetServiceInstanceType(vssRequestContext, instanceType);
      Publisher publisher = new Publisher()
      {
        Name = ServiceBusPublisher.ToPascalCase(serviceInstanceType.Name),
        ServiceOwnerId = instanceType
      };
      return new ServiceEvent()
      {
        Publisher = publisher,
        EventType = eventType,
        ResourceVersion = "1.0-preview.1",
        ResourceContainers = ServiceBusPublisher.GetResourceContainers(requestContext)
      };
    }

    private static string ToPascalCase(string input) => char.ToUpperInvariant(input[0]).ToString() + input.ToLowerInvariant().Substring(1);

    private static Dictionary<string, object> GetResourceContainers(
      IVssRequestContext requestContext)
    {
      Dictionary<string, object> resourceContainers = new Dictionary<string, object>();
      IVssServiceHost collectionServiceHost = requestContext.ServiceHost.CollectionServiceHost;
      Guid guid = collectionServiceHost != null ? collectionServiceHost.InstanceId : Guid.Empty;
      IVssServiceHost organizationServiceHost = requestContext.ServiceHost.OrganizationServiceHost;
      resourceContainers.Add("Account", (object) (organizationServiceHost == null || !organizationServiceHost.IsOnly(TeamFoundationHostType.Application) ? Guid.Empty : organizationServiceHost.InstanceId));
      resourceContainers.Add("Collection", (object) guid);
      return resourceContainers;
    }

    private static void PublishToServiceBus(
      IVssRequestContext requestContext,
      string messageBusName,
      ServiceEvent[] payloadToPublish)
    {
      requestContext.GetService<IMessageBusPublisherService>().Publish(requestContext.Elevate(), messageBusName, (object[]) payloadToPublish, false);
    }
  }
}
