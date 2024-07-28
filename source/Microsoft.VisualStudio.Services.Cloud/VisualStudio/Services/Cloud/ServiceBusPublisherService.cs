// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusPublisherService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal sealed class ServiceBusPublisherService : 
    IMessageBusPublisherService,
    IVssFrameworkService
  {
    private const string s_Area = "ServiceBusPublisherService";
    private const string s_Layer = "Service";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void Publish(
      IVssRequestContext requestContext,
      string messageBusName,
      object[] serializableObjects,
      bool throwOnMissingPublisher = true,
      bool allowLoopback = true,
      bool includeAssignedHosts = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(messageBusName, nameof (messageBusName));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) serializableObjects, nameof (serializableObjects));
      requestContext.TraceEnter(1005700, nameof (ServiceBusPublisherService), "Service", "ServiceBusPublisherService.Publish");
      try
      {
        bool flag;
        if (((requestContext.ServiceHost.IsProduction ? 0 : (requestContext.RootContext.TryGetItem<bool>("IsAsyncNotification", out flag) ? 1 : 0)) & (flag ? 1 : 0)) != 0)
          requestContext.TraceAlways(543788189, TraceLevel.Info, nameof (ServiceBusPublisherService), "Service", string.Format("Synchronous publishing message on {0} during notification. host {1} stack trace {2}", (object) messageBusName, (object) requestContext.ServiceHost.InstanceId, (object) EnvironmentWrapper.ToReadableStackTrace()));
        MessageBusMessage[] messages = this.BuildMessages(requestContext, messageBusName, serializableObjects, allowLoopback, includeAssignedHosts);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<ServiceBusManagementService>().Publish(vssRequestContext, messageBusName, messages, throwOnMissingPublisher);
      }
      finally
      {
        requestContext.TraceLeave(1005710, nameof (ServiceBusPublisherService), "Service", "ServiceBusPublisherService.Publish");
      }
    }

    public async Task PublishAsync(
      IVssRequestContext requestContext,
      string messageBusName,
      object[] serializableObjects,
      bool throwOnMissingPublisher = true,
      bool allowLoopback = true,
      bool includeAssignedHosts = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(messageBusName, nameof (messageBusName));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) serializableObjects, nameof (serializableObjects));
      requestContext.AssertAsyncExecutionEnabled();
      requestContext.TraceEnter(1005700, nameof (ServiceBusPublisherService), "Service", "ServiceBusPublisherService.Publish");
      try
      {
        MessageBusMessage[] source = this.BuildMessages(requestContext, messageBusName, serializableObjects, allowLoopback, includeAssignedHosts);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        await vssRequestContext.GetService<ServiceBusManagementService>().PublishAsync(vssRequestContext, messageBusName, ((IEnumerable<MessageBusMessage>) source).ToArray<MessageBusMessage>(), throwOnMissingPublisher);
      }
      finally
      {
        requestContext.TraceLeave(1005710, nameof (ServiceBusPublisherService), "Service", "ServiceBusPublisherService.Publish");
      }
    }

    public void TryPublish(
      IVssRequestContext requestContext,
      string messageBusName,
      object[] serializableObjects,
      bool throwOnMissingPublisher = true,
      bool allowLoopback = true,
      bool includeAssignedHosts = false)
    {
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ServiceBusManagementService>().PublishDispatcher.Run(requestContext.Elevate(), nameof (TryPublish), (Func<IVssRequestContext, Task>) (context => this.PublishAsync(context, messageBusName, serializableObjects, throwOnMissingPublisher, allowLoopback, includeAssignedHosts)));
    }

    private MessageBusMessage[] BuildMessages(
      IVssRequestContext requestContext,
      string messageBusName,
      object[] serializableObjects,
      bool allowLoopback,
      bool includeAssignedHosts)
    {
      List<KeyValuePair<string, object>> keyValuePairList = new List<KeyValuePair<string, object>>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Guid guid1 = requestContext.ServiceInstanceId();
      Guid guid2 = requestContext.ServiceInstanceType();
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        requestContext.Trace(1005702, TraceLevel.Info, nameof (ServiceBusPublisherService), "Service", "Not deployment host, adding location service properties");
        IInstanceManagementService service = vssRequestContext.GetService<IInstanceManagementService>();
        Dictionary<Guid, HostInstanceMapping> dictionary = service.GetHostInstanceMappings(vssRequestContext, requestContext.ServiceHost.InstanceId).ToDictionary<HostInstanceMapping, Guid>((Func<HostInstanceMapping, Guid>) (s => s.ServiceInstance.InstanceType));
        Dictionary<Guid, List<VirtualHostInstanceMapping>> instanceMappings = requestContext.GetService<IVirtualHostInstanceMappingRetrievalService>().GetVirtualHostInstanceMappings(requestContext);
        foreach (ServiceInstanceType serviceInstanceType in (IEnumerable<ServiceInstanceType>) service.GetServiceInstanceTypes(vssRequestContext))
        {
          requestContext.Trace(1005704, TraceLevel.Info, nameof (ServiceBusPublisherService), "Service", "VS Service : {0}", (object) serviceInstanceType.InstanceType);
          List<Guid> instanceIds = new List<Guid>();
          HostInstanceMapping hostInstanceMapping1;
          if (dictionary.TryGetValue(serviceInstanceType.InstanceType, out hostInstanceMapping1) && (hostInstanceMapping1.Status == ServiceStatus.Active || includeAssignedHosts && hostInstanceMapping1.Status == ServiceStatus.Assigned) && (allowLoopback || serviceInstanceType.InstanceType != guid2))
          {
            requestContext.Trace(1005706, TraceLevel.Info, nameof (ServiceBusPublisherService), "Service", "Instance Id : {0}", (object) hostInstanceMapping1.ServiceInstance.InstanceId);
            instanceIds.Add(hostInstanceMapping1.ServiceInstance.InstanceId);
          }
          List<VirtualHostInstanceMapping> hostInstanceMappingList;
          if (instanceMappings.TryGetValue(serviceInstanceType.InstanceType, out hostInstanceMappingList))
          {
            foreach (VirtualHostInstanceMapping hostInstanceMapping2 in hostInstanceMappingList)
            {
              if (allowLoopback || hostInstanceMapping2.ServiceInstanceId != guid1)
                instanceIds.Add(hostInstanceMapping2.ServiceInstanceId);
            }
          }
          if (instanceIds.Count == 0)
          {
            requestContext.Trace(1005708, TraceLevel.Info, nameof (ServiceBusPublisherService), "Service", "No active instance for VS Service : {0}", (object) serviceInstanceType.InstanceType);
            instanceIds.Add(Guid.Empty);
          }
          keyValuePairList.Add(new KeyValuePair<string, object>(ServiceBusPropertyHelper.GetServiceInstanceProperty(serviceInstanceType.InstanceType), (object) ServiceBusPropertyHelper.GetServiceInstancesValue(instanceIds)));
        }
        keyValuePairList.Add(new KeyValuePair<string, object>("ParentSourceHostId", (object) requestContext.To(TeamFoundationHostType.Parent).ServiceHost.InstanceId));
        if (!requestContext.ServiceHost.IsProduction && requestContext.ServiceHost.IsCreating())
          requestContext.TraceAlways(1792874482, TraceLevel.Info, nameof (ServiceBusPublisherService), "Service", "Publishing message on {0} during host creation of host {1}, type {2}, stack trace {3}", (object) messageBusName, (object) requestContext.ServiceHost.InstanceId, (object) requestContext.ServiceHost.HostType, (object) EnvironmentWrapper.ToReadableStackTrace());
      }
      keyValuePairList.Add(new KeyValuePair<string, object>("SourceHostId", (object) requestContext.ServiceHost.InstanceId));
      keyValuePairList.Add(new KeyValuePair<string, object>("SourceHostType", (object) (int) ServiceBusPropertyHelper.GetNormalizedHostType(requestContext.ServiceHost.HostType)));
      keyValuePairList.Add(new KeyValuePair<string, object>("SourceServiceInstanceId", (object) guid1));
      keyValuePairList.Add(new KeyValuePair<string, object>("SourceServiceInstanceType", (object) guid2));
      keyValuePairList.Add(new KeyValuePair<string, object>("TopicName", (object) messageBusName));
      List<MessageBusMessage> messageBusMessageList = new List<MessageBusMessage>(serializableObjects.Length);
      foreach (object serializableObject in serializableObjects)
      {
        if (!(serializableObject is MessageBusMessage messageBusMessage))
          messageBusMessage = new MessageBusMessage(serializableObject);
        foreach (KeyValuePair<string, object> keyValuePair in keyValuePairList)
          messageBusMessage.Properties[keyValuePair.Key] = keyValuePair.Value;
        messageBusMessageList.Add(messageBusMessage);
      }
      return messageBusMessageList.ToArray();
    }
  }
}
