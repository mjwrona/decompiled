// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DistributedTaskEventPublisherService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class DistributedTaskEventPublisherService : 
    IDistributedTaskEventPublisherService,
    IVssFrameworkService
  {
    private const string c_layer = "EventPublisher";

    public void NotifyAgentChangeEvent(
      IVssRequestContext requestContext,
      string eventType,
      TaskAgentPool pool,
      TaskAgent agent)
    {
      if (requestContext.IsFeatureEnabled("DistributedTask.IgnoreAgentChangeEvents"))
        return;
      using (new MethodScope(requestContext, "EventPublisher", nameof (NotifyAgentChangeEvent)))
        this.PublishEvent(requestContext, eventType, (object) new AgentChangeEvent(eventType, agent, pool.AsReference()));
    }

    public void NotifyAgentPoolEvent(
      IVssRequestContext requestContext,
      string eventType,
      TaskAgentPool pool)
    {
      using (new MethodScope(requestContext, "EventPublisher", nameof (NotifyAgentPoolEvent)))
        this.PublishEvent(requestContext, eventType, (object) new AgentPoolEvent(eventType, pool));
    }

    public void NotifyAgentQueueEvent(
      IVssRequestContext requestContext,
      string eventType,
      TaskAgentQueue queue)
    {
      using (new MethodScope(requestContext, "EventPublisher", nameof (NotifyAgentQueueEvent)))
        this.PublishEvent(requestContext, eventType, (object) new AgentQueueEvent(eventType, queue));
    }

    public void NotifyAgentQueuesEvent(
      IVssRequestContext requestContext,
      string eventType,
      IEnumerable<TaskAgentQueue> queues)
    {
      using (new MethodScope(requestContext, "EventPublisher", nameof (NotifyAgentQueuesEvent)))
      {
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          return;
        this.PublishEvent(requestContext, eventType, (object) new AgentQueuesEvent(eventType, queues));
      }
    }

    public void NotifyElasticPoolResizedEvent(
      IVssRequestContext requestContext,
      TaskAgentPool pool,
      ElasticPool elasticPool,
      int previousSize,
      int newSize)
    {
      using (new MethodScope(requestContext, "EventPublisher", nameof (NotifyElasticPoolResizedEvent)))
      {
        if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
          return;
        ElasticAgentPoolResizedEvent data = new ElasticAgentPoolResizedEvent(pool, elasticPool, previousSize, newSize);
        this.PublishEvent(requestContext, "MS.TF.DistributedTask.ElasticAgentPoolResized", (object) data);
        VssNotificationEvent theEvent = new VssNotificationEvent((object) data)
        {
          EventType = "ms.vss-distributed-task.elastic-pool-resized-event"
        };
        theEvent.AddScope(VssNotificationEvent.ScopeNames.Project, elasticPool.ServiceEndpointScope);
        requestContext.GetService<INotificationEventService>().PublishSystemEvent(requestContext, theEvent);
      }
    }

    public void NotifyMachinesChangedEvent(
      IVssRequestContext requestContext,
      DeploymentGroup machineGroup,
      IList<DeploymentMachineChangedData> deploymentMachines)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (NotifyMachinesChangedEvent)))
      {
        List<DeploymentMachinesChangeEvent> machinesChangeEventList = new List<DeploymentMachinesChangeEvent>();
        for (int count = 0; count < deploymentMachines.Count; count += 10)
        {
          IEnumerable<DeploymentMachineChangedData> source = deploymentMachines.Skip<DeploymentMachineChangedData>(count).Take<DeploymentMachineChangedData>(10);
          DeploymentMachinesChangeEvent machinesChangeEvent = new DeploymentMachinesChangeEvent(machineGroup.AsReference(), (IList<DeploymentMachineChangedData>) source.ToList<DeploymentMachineChangedData>());
          machinesChangeEventList.Add(machinesChangeEvent);
        }
        this.PublishEvent(requestContext, "MS.TF.DistributedTask.DeploymentMachinesChanged", (object[]) machinesChangeEventList.ToArray());
      }
    }

    public void NotifySecureFilesEvent(
      IVssRequestContext requestContext,
      string eventType,
      IEnumerable<SecureFile> secureFiles,
      Guid projectId)
    {
      using (new MethodScope(requestContext, "EventPublisher", nameof (NotifySecureFilesEvent)))
        this.PublishEvent(requestContext, eventType, (object) new SecureFileEvent(eventType, secureFiles, projectId));
    }

    private Dictionary<string, object> GetResourceContainers(IVssRequestContext requestContext)
    {
      Dictionary<string, object> resourceContainers = new Dictionary<string, object>();
      IVssServiceHost organizationServiceHost = requestContext.ServiceHost.OrganizationServiceHost;
      if (organizationServiceHost != null)
        resourceContainers["Account"] = (object) organizationServiceHost.InstanceId;
      IVssServiceHost collectionServiceHost = requestContext.ServiceHost.CollectionServiceHost;
      if (collectionServiceHost != null)
        resourceContainers["Collection"] = (object) collectionServiceHost.InstanceId;
      return resourceContainers;
    }

    private void PublishEvent(
      IVssRequestContext requestContext,
      string eventType,
      params object[] eventsData)
    {
      if (eventsData == null || eventsData.Length == 0)
        return;
      ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
      foreach (object notificationEvent in eventsData)
        service.PublishNotification(requestContext, notificationEvent);
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      Dictionary<string, object> resourceContainers = this.GetResourceContainers(requestContext);
      Publisher publisher = new Publisher()
      {
        Name = "Tfs",
        ServiceOwnerId = ServiceInstanceTypes.TFS
      };
      ServiceEvent[] serializableObjects = Array.ConvertAll<object, ServiceEvent>(eventsData, (Converter<object, ServiceEvent>) (eventData => DistributedTaskEventPublisherService.ToServiceEvent(eventType, eventData, publisher, resourceContainers)));
      requestContext.GetService<IMessageBusPublisherService>().TryPublish(requestContext, "Microsoft.TeamFoundation.DistributedTask.Server", (object[]) serializableObjects, allowLoopback: false);
      requestContext.TraceInfo(10015119, "DistributedTask", "Published distributed task event of type {0}", (object) eventType);
    }

    private static ServiceEvent ToServiceEvent(
      string eventType,
      object eventData,
      Publisher publisher,
      Dictionary<string, object> resourceContainers)
    {
      return new ServiceEvent()
      {
        EventType = eventType,
        Publisher = publisher,
        Resource = eventData,
        ResourceVersion = "2.0",
        ResourceContainers = resourceContainers
      };
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
