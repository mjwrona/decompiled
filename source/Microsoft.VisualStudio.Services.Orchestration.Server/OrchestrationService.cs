// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationService
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Orchestration.History;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  public class OrchestrationService : IOrchestrationService, IVssFrameworkService
  {
    private string m_sqlNotificationDataspaceCategory;
    private bool m_hubCacheFresh;
    private readonly ConcurrentDictionary<string, OrchestrationHubDescription> m_hubCache = new ConcurrentDictionary<string, OrchestrationHubDescription>();
    private readonly ConcurrentDictionary<string, OrchestrationSerializer> m_serializerCache = new ConcurrentDictionary<string, OrchestrationSerializer>();
    private ILockName m_cacheLock;
    private static readonly Guid s_refreshCacheNotificationId = new Guid("{EF2E585E-1484-4E01-B312-B9ABADE30C4D}");

    public virtual OrchestrationHubDescription CreateHub(
      IVssRequestContext requestContext,
      OrchestrationHubDescription description)
    {
      using (OrchestrationComponent component = requestContext.CreateComponent<OrchestrationComponent>())
        description = component.CreateHub(description);
      this.CreateOrchestrationHubJobs(requestContext, description);
      this.m_hubCache[description.HubName] = description;
      return description;
    }

    public virtual OrchestrationInstance CreateOrchestrationInstance(
      IVssRequestContext requestContext,
      string hubName,
      string name,
      string version,
      string instanceId,
      object input)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(hubName, nameof (hubName));
      ArgumentUtility.CheckStringForNullOrEmpty(nameof (name), name);
      ArgumentUtility.CheckStringForNullOrEmpty(nameof (instanceId), instanceId);
      ArgumentUtility.CheckForNull<object>(input, nameof (input));
      OrchestrationHubDescription orchestrationHubDescription = this.EnsureHubDescription(requestContext, hubName);
      string str = Guid.NewGuid().ToString("N");
      OrchestrationInstance orchestrationInstance = new OrchestrationInstance()
      {
        InstanceId = instanceId,
        ExecutionId = str
      };
      OrchestrationSerializer serializer = this.GetSerializer(requestContext, orchestrationHubDescription);
      string input1 = serializer.Serialize(input);
      TaskMessage messageObject = new TaskMessage()
      {
        OrchestrationInstance = orchestrationInstance,
        Event = (HistoryEvent) new ExecutionStartedEvent(-1, DateTime.UtcNow, input1)
        {
          Name = name,
          Version = version,
          OrchestrationInstance = orchestrationInstance
        }
      };
      OrchestrationMessage messageFromObject = Utilities.GetMessageFromObject(orchestrationHubDescription, serializer, instanceId, messageObject);
      using (OrchestrationComponent component = requestContext.CreateComponent<OrchestrationComponent>())
        component.CreateOrchestrationSession(orchestrationHubDescription, instanceId, (IEnumerable<OrchestrationMessage>) new OrchestrationMessage[1]
        {
          messageFromObject
        });
      orchestrationHubDescription.OrchestrationDispatcher.Counters.OrchestrationSessionsQueuedPerSec.Increment();
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      try
      {
        service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          orchestrationHubDescription.OrchestrationDispatcher.JobId
        }, false);
      }
      catch (JobDefinitionNotFoundException ex)
      {
        this.CreateOrchestrationHubJobs(requestContext, orchestrationHubDescription);
        service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          orchestrationHubDescription.OrchestrationDispatcher.JobId
        }, false);
      }
      return new OrchestrationInstance()
      {
        InstanceId = instanceId,
        ExecutionId = str
      };
    }

    public virtual OrchestrationHubDescription GetHubDescription(
      IVssRequestContext requestContext,
      string hubName)
    {
      return this.GetHubDescription(requestContext, hubName, true);
    }

    public OrchestrationSession GetOrchestrationSession(
      IVssRequestContext requestContext,
      string hubName,
      string instanceId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(hubName, nameof (hubName));
      ArgumentUtility.CheckStringForNullOrEmpty(instanceId, nameof (instanceId));
      OrchestrationHubDescription orchestrationHubDescription = this.EnsureHubDescription(requestContext, hubName);
      using (OrchestrationComponent component = requestContext.CreateComponent<OrchestrationComponent>())
        return component.GetOrchestrationSession(orchestrationHubDescription.HubName, instanceId);
    }

    public IList<OrchestrationState> GetOrchestrationState(
      IVssRequestContext requestContext,
      string hubName,
      string instanceId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(hubName, nameof (hubName));
      ArgumentUtility.CheckStringForNullOrEmpty(instanceId, nameof (instanceId));
      OrchestrationHubDescription orchestrationHubDescription = this.EnsureHubDescription(requestContext, hubName);
      using (OrchestrationComponent component = requestContext.CreateComponent<OrchestrationComponent>())
        return component.GetOrchestrationState(orchestrationHubDescription.HubName, instanceId, (string) null);
    }

    public OrchestrationState GetOrchestrationState(
      IVssRequestContext requestContext,
      string hubName,
      string instanceId,
      string executionId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(hubName, nameof (hubName));
      ArgumentUtility.CheckStringForNullOrEmpty(instanceId, nameof (instanceId));
      ArgumentUtility.CheckStringForNullOrEmpty(executionId, nameof (executionId));
      OrchestrationHubDescription orchestrationHubDescription = this.EnsureHubDescription(requestContext, hubName);
      using (OrchestrationComponent component = requestContext.CreateComponent<OrchestrationComponent>())
        return component.GetOrchestrationState(orchestrationHubDescription.HubName, instanceId, executionId).FirstOrDefault<OrchestrationState>();
    }

    public IList<OrchestrationState> GetRunningOrchestrationState(
      IVssRequestContext requestContext,
      string hubName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(hubName, nameof (hubName));
      OrchestrationHubDescription orchestrationHubDescription = this.EnsureHubDescription(requestContext, hubName);
      using (OrchestrationComponent component = requestContext.CreateComponent<OrchestrationComponent>())
        return component.GetRunningOrchestrationState(orchestrationHubDescription.HubName, (string) null);
    }

    public OrchestrationState GetRunningOrchestrationState(
      IVssRequestContext requestContext,
      string hubName,
      string instanceId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(hubName, nameof (hubName));
      ArgumentUtility.CheckStringForNullOrEmpty(instanceId, nameof (instanceId));
      OrchestrationHubDescription orchestrationHubDescription = this.EnsureHubDescription(requestContext, hubName);
      using (OrchestrationComponent component = requestContext.CreateComponent<OrchestrationComponent>())
        return component.Version < 10 ? component.GetOrchestrationState(orchestrationHubDescription.HubName, instanceId, (string) null).Where<OrchestrationState>((Func<OrchestrationState, bool>) (x => x.OrchestrationStatus == OrchestrationStatus.Running)).FirstOrDefault<OrchestrationState>() : component.GetRunningOrchestrationState(orchestrationHubDescription.HubName, instanceId).FirstOrDefault<OrchestrationState>();
    }

    public virtual Task RaiseEventAsync(
      IVssRequestContext requestContext,
      string hubName,
      OrchestrationInstance instance,
      string eventName,
      object eventData,
      bool ensureOrchestrationExists = false,
      DateTime? fireAt = null)
    {
      ArgumentUtility.CheckForNull<OrchestrationInstance>(instance, nameof (instance));
      ArgumentUtility.CheckStringForNullOrEmpty(instance.InstanceId, "instance.InstanceId");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(eventName, nameof (eventName));
      OrchestrationHubDescription hubDescription = this.EnsureHubDescription(requestContext, hubName);
      string input = this.GetSerializer(requestContext, hubDescription).Serialize(eventData);
      TaskMessage taskMessage = new TaskMessage()
      {
        OrchestrationInstance = instance,
        Event = (HistoryEvent) new EventRaisedEvent(-1, DateTime.UtcNow, input)
        {
          Name = eventName
        },
        FireAt = fireAt
      };
      return this.SendMessageToSessionAsync(requestContext, hubDescription, instance.InstanceId, taskMessage, ensureOrchestrationExists);
    }

    public OrchestrationHubDescription RenameHub(
      IVssRequestContext requestContext,
      string hubName,
      string newHubName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(hubName, nameof (hubName));
      ArgumentUtility.CheckStringForNullOrEmpty(newHubName, nameof (newHubName));
      OrchestrationHubDescription orchestrationHubDescription1 = this.EnsureHubDescription(requestContext, hubName);
      using (OrchestrationComponent component = requestContext.CreateComponent<OrchestrationComponent>())
        orchestrationHubDescription1 = component.UpdateHub(hubName, newHubName);
      List<TeamFoundationJobDefinition> jobUpdates = new List<TeamFoundationJobDefinition>();
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition1 = service.QueryJobDefinition(requestContext, orchestrationHubDescription1.OrchestrationDispatcher.JobId);
      if (foundationJobDefinition1 != null)
      {
        foundationJobDefinition1.Data = Utilities.GetJobData(newHubName);
        if (foundationJobDefinition1.PriorityClass < JobPriorityClass.AboveNormal)
          foundationJobDefinition1.PriorityClass = JobPriorityClass.AboveNormal;
        jobUpdates.Add(foundationJobDefinition1);
      }
      List<ActivityDispatcherDescriptor> source = new List<ActivityDispatcherDescriptor>();
      source.AddRange((IEnumerable<ActivityDispatcherDescriptor>) orchestrationHubDescription1.ActivityDispatchers.Values);
      Dictionary<Guid, TeamFoundationJobDefinition> dictionary = service.QueryJobDefinitions(requestContext, source.Select<ActivityDispatcherDescriptor, Guid>((Func<ActivityDispatcherDescriptor, Guid>) (x => x.JobId))).ToDictionary<TeamFoundationJobDefinition, Guid>((Func<TeamFoundationJobDefinition, Guid>) (x => x.JobId));
      foreach (ActivityDispatcherDescriptor dispatcherDescriptor in source)
      {
        TeamFoundationJobDefinition foundationJobDefinition2;
        if (dictionary.TryGetValue(dispatcherDescriptor.JobId, out foundationJobDefinition2))
        {
          foundationJobDefinition2.Data = Utilities.GetJobData(newHubName, dispatcherDescriptor.Type);
          if (foundationJobDefinition2.PriorityClass < JobPriorityClass.AboveNormal)
            foundationJobDefinition2.PriorityClass = JobPriorityClass.AboveNormal;
          jobUpdates.Add(foundationJobDefinition2);
        }
      }
      service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
      using (requestContext.AcquireWriterLock(this.m_cacheLock))
      {
        OrchestrationHubDescription orchestrationHubDescription2;
        if (this.m_hubCache.TryRemove(hubName, out orchestrationHubDescription2))
          requestContext.Trace(0, TraceLevel.Verbose, "Orchestration", "Service", "Removed hub {0} from the cache", (object) orchestrationHubDescription2.HubName);
        this.m_hubCache[orchestrationHubDescription1.HubName] = orchestrationHubDescription1;
      }
      return orchestrationHubDescription1;
    }

    public Task TerminateOrchestrationInstanceAsync(
      IVssRequestContext requestContext,
      string hubName,
      string instanceId)
    {
      return this.TerminateOrchestrationInstanceAsync(requestContext, hubName, instanceId, string.Empty);
    }

    public Task TerminateOrchestrationInstanceAsync(
      IVssRequestContext requestContext,
      string hubName,
      string instanceId,
      string reason)
    {
      IVssRequestContext requestContext1 = requestContext;
      string hubName1 = hubName;
      OrchestrationInstance instance = new OrchestrationInstance();
      instance.InstanceId = instanceId;
      string reason1 = reason;
      return this.TerminateOrchestrationInstanceAsync(requestContext1, hubName1, instance, reason1);
    }

    public Task TerminateOrchestrationInstanceAsync(
      IVssRequestContext requestContext,
      string hubName,
      OrchestrationInstance instance,
      string reason)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(hubName, nameof (hubName));
      ArgumentUtility.CheckForNull<OrchestrationInstance>(instance, nameof (instance));
      ArgumentUtility.CheckStringForNullOrEmpty(instance.InstanceId, "instance.InstanceId");
      OrchestrationHubDescription hubDescription = this.EnsureHubDescription(requestContext, hubName);
      TaskMessage terminateMessage = Utilities.GetForcedTerminateMessage(instance.InstanceId, reason, DateTime.UtcNow);
      return this.SendMessageToSessionAsync(requestContext, hubDescription, instance.InstanceId, terminateMessage);
    }

    public void ScheduleDipatcherJobForDeliverableMessages(
      IVssRequestContext requestContext,
      IEnumerable<string> hubNames)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) hubNames, nameof (hubNames));
      IList<OrchestrationHubMessageCount> hubMessageCounts;
      using (OrchestrationComponent component = requestContext.CreateComponent<OrchestrationComponent>())
        hubMessageCounts = component.GetOrchestrationHubMessageCounts(hubNames);
      List<Guid> jobIds = new List<Guid>();
      jobIds.AddRange(hubMessageCounts.Where<OrchestrationHubMessageCount>((Func<OrchestrationHubMessageCount, bool>) (x => x.ActivityMessagesCount > 0)).Select<OrchestrationHubMessageCount, Guid>((Func<OrchestrationHubMessageCount, Guid>) (x => x.ActivityDispatcherId)));
      jobIds.AddRange(hubMessageCounts.Where<OrchestrationHubMessageCount>((Func<OrchestrationHubMessageCount, bool>) (x => x.OrchestrationMessagesCount > 0)).Select<OrchestrationHubMessageCount, Guid>((Func<OrchestrationHubMessageCount, Guid>) (x => x.OrchestrationDispatcherId)));
      if (jobIds.Count <= 0)
        return;
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      List<Guid> list = service.QueryJobDefinitions(requestContext, (IEnumerable<Guid>) jobIds).Where<TeamFoundationJobDefinition>((Func<TeamFoundationJobDefinition, bool>) (x => x != null)).Select<TeamFoundationJobDefinition, Guid>((Func<TeamFoundationJobDefinition, Guid>) (x => x.JobId)).ToList<Guid>();
      if (list.Count <= 0)
        return;
      service.QueueJobsNow(requestContext, (IEnumerable<Guid>) list);
      requestContext.Trace(15010010, TraceLevel.Verbose, "Orchestration", "Service", "Queued {0} stuck orchestration/activity dispatcher jobs.", (object) list.Count);
    }

    internal async Task<IEnumerable<ActivityDispatcherDescriptor>> CreateActivityDispatchersAsync(
      IVssRequestContext requestContext,
      OrchestrationHubDescription hub,
      IEnumerable<ActivityDispatcherDescriptor> dispatchers)
    {
      List<ActivityDispatcherDescriptor> newDispatchers = new List<ActivityDispatcherDescriptor>();
      using (OrchestrationComponent component = requestContext.CreateComponent<OrchestrationComponent>())
      {
        IEnumerable<ActivityDispatcherDescriptor> dispatcherDescriptors = await component.AddActivityDispatchersAsync(hub.HubName, dispatchers);
        if (dispatcherDescriptors == null)
          return (IEnumerable<ActivityDispatcherDescriptor>) newDispatchers;
        foreach (ActivityDispatcherDescriptor dispatcherDescriptor in dispatcherDescriptors)
        {
          newDispatchers.Add(dispatcherDescriptor);
          hub.ActivityDispatchers.TryAdd(dispatcherDescriptor.Type, dispatcherDescriptor);
        }
      }
      if (newDispatchers.Count > 0)
        requestContext.GetService<ITeamFoundationJobService>().UpdateJobDefinitions(requestContext, this.GetActivityDispatcherJobs(hub, (IEnumerable<ActivityDispatcherDescriptor>) newDispatchers));
      return (IEnumerable<ActivityDispatcherDescriptor>) newDispatchers;
    }

    internal OrchestrationHubDescription GetHubDescription(
      IVssRequestContext requestContext,
      string hubName,
      bool useCache)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(hubName, nameof (hubName));
      OrchestrationHubDescription hubDescription;
      if (useCache && this.m_hubCache.TryGetValue(hubName, out hubDescription))
        return hubDescription;
      this.EnsureHubDescriptions(requestContext, true);
      return this.m_hubCache.TryGetValue(hubName, out hubDescription) ? hubDescription : (OrchestrationHubDescription) null;
    }

    private void CreateOrchestrationHubJobs(
      IVssRequestContext requestContext,
      OrchestrationHubDescription description)
    {
      List<TeamFoundationJobDefinition> jobUpdates = new List<TeamFoundationJobDefinition>();
      jobUpdates.Add(new TeamFoundationJobDefinition()
      {
        JobId = description.OrchestrationDispatcher.JobId,
        Name = OrchestrationResources.OrchestrationDispatcherName(),
        Data = Utilities.GetJobData(description.HubName),
        PriorityClass = JobPriorityClass.High,
        ExtensionName = "Microsoft.VisualStudio.Services.Orchestration.Extensions.OrchestrationDispatcherJob",
        Schedule = {
          new TeamFoundationJobSchedule()
          {
            Interval = (int) TimeSpan.FromDays(1.0).TotalSeconds,
            ScheduledTime = new DateTime(2011, 7, 3, 1, 0, 0, DateTimeKind.Utc)
          }
        }
      });
      jobUpdates.AddRange(this.GetActivityDispatcherJobs(description, (IEnumerable<ActivityDispatcherDescriptor>) description.ActivityDispatchers.Values));
      requestContext.GetService<ITeamFoundationJobService>().UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
    }

    private IEnumerable<TeamFoundationJobDefinition> GetActivityDispatcherJobs(
      OrchestrationHubDescription hub,
      IEnumerable<ActivityDispatcherDescriptor> dispatchers)
    {
      List<TeamFoundationJobDefinition> activityDispatcherJobs = new List<TeamFoundationJobDefinition>();
      foreach (ActivityDispatcherDescriptor dispatcher in dispatchers)
      {
        string str = OrchestrationResources.ActivityDispatcherName();
        if (!string.IsNullOrEmpty(dispatcher.Type))
          str = OrchestrationResources.CustomActivityDispatcherName((object) dispatcher.Type);
        activityDispatcherJobs.Add(new TeamFoundationJobDefinition()
        {
          JobId = dispatcher.JobId,
          Name = str,
          Data = Utilities.GetJobData(hub.HubName, dispatcher.Type),
          PriorityClass = JobPriorityClass.High,
          ExtensionName = "Microsoft.VisualStudio.Services.Orchestration.Extensions.ActivityDispatcherJob",
          Schedule = {
            new TeamFoundationJobSchedule()
            {
              Interval = (int) TimeSpan.FromDays(1.0).TotalSeconds,
              ScheduledTime = new DateTime(2011, 7, 3, 1, 0, 0, DateTimeKind.Utc)
            }
          }
        });
      }
      return (IEnumerable<TeamFoundationJobDefinition>) activityDispatcherJobs;
    }

    private OrchestrationHubDescription EnsureHubDescription(
      IVssRequestContext requestContext,
      string hubName)
    {
      return this.GetHubDescription(requestContext, hubName) ?? throw new OrchestrationHubNotFoundException(OrchestrationResources.HubNotFound((object) hubName));
    }

    private void EnsureHubDescriptions(IVssRequestContext requestContext, bool force = false)
    {
      if (this.m_hubCacheFresh && !force)
        return;
      List<OrchestrationHubDescription> orchestrationHubDescriptionList = new List<OrchestrationHubDescription>();
      using (OrchestrationComponent component = requestContext.CreateComponent<OrchestrationComponent>())
        orchestrationHubDescriptionList.AddRange(component.GetHubs((string) null));
      using (requestContext.AcquireWriterLock(this.m_cacheLock))
      {
        this.m_hubCache.Clear();
        foreach (OrchestrationHubDescription orchestrationHubDescription in orchestrationHubDescriptionList)
          this.m_hubCache[orchestrationHubDescription.HubName] = orchestrationHubDescription;
        this.m_hubCacheFresh = true;
      }
    }

    private OrchestrationSerializer GetSerializer(
      IVssRequestContext requestContext,
      OrchestrationHubDescription hubDescription)
    {
      return this.m_serializerCache.GetOrAdd(hubDescription.HubType, (Func<string, OrchestrationSerializer>) (n => new OrchestrationSerializer()));
    }

    private void InvalidateCache(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      using (requestContext.AcquireWriterLock(this.m_cacheLock))
      {
        this.m_hubCache.Clear();
        this.m_hubCacheFresh = false;
      }
    }

    private async Task SendMessageToSessionAsync(
      IVssRequestContext requestContext,
      OrchestrationHubDescription hubDescription,
      string sessionId,
      TaskMessage taskMessage,
      bool ensureSessionExists = false)
    {
      OrchestrationMessage messageFromObject = Utilities.GetMessageFromObject(hubDescription, this.GetSerializer(requestContext, hubDescription), sessionId, taskMessage);
      using (OrchestrationComponent hubComponent = requestContext.CreateComponent<OrchestrationComponent>())
        await hubComponent.UpdateOrchestrationSessionAsync(hubDescription.HubName, OrchestrationHubDispatcherType.Orchestration, orchestrationMessages: (IEnumerable<OrchestrationMessage>) new OrchestrationMessage[1]
        {
          messageFromObject
        }, ensureSessionExists: (ensureSessionExists ? 1 : 0) != 0);
      hubDescription.OrchestrationDispatcher.Counters.OrchestrationSessionsQueuedPerSec.Increment();
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      try
      {
        service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          hubDescription.OrchestrationDispatcher.JobId
        }, false);
      }
      catch (JobDefinitionNotFoundException ex)
      {
        this.CreateOrchestrationHubJobs(requestContext, hubDescription);
        service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          hubDescription.OrchestrationDispatcher.JobId
        }, false);
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, this.m_sqlNotificationDataspaceCategory, OrchestrationService.s_refreshCacheNotificationId, new SqlNotificationHandler(this.InvalidateCache), false);

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      this.m_cacheLock = requestContext.ServiceHost.CreateUniqueLockName("OrchestrationService.HubCache");
      Dataspace dataspace = requestContext.GetService<IDataspaceService>().QueryDataspace(requestContext, "Orchestration", Guid.Empty, false);
      this.m_sqlNotificationDataspaceCategory = dataspace != null ? dataspace.DataspaceCategory : "Default";
      requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, this.m_sqlNotificationDataspaceCategory, OrchestrationService.s_refreshCacheNotificationId, new SqlNotificationHandler(this.InvalidateCache), true);
      using (IDisposableReadOnlyList<IOrchestrationHubExtension> extensions = requestContext.GetExtensions<IOrchestrationHubExtension>())
      {
        foreach (IOrchestrationHubExtension orchestrationHubExtension in (IEnumerable<IOrchestrationHubExtension>) extensions)
        {
          if (this.m_serializerCache.ContainsKey(orchestrationHubExtension.HubType))
          {
            requestContext.Trace(0, TraceLevel.Error, "Orchestration", "Service", "Duplicate hub extensions for hub type '{0}'", (object) orchestrationHubExtension.HubType);
          }
          else
          {
            OrchestrationSerializer orchestrationSerializer = new OrchestrationSerializer(orchestrationHubExtension.GetSerializerSettings(requestContext));
            this.m_serializerCache[orchestrationHubExtension.HubType] = orchestrationSerializer;
          }
        }
      }
    }

    public void RemovePoisonedOrchestrations(
      IVssRequestContext requestContext,
      string hubName,
      IList<string> orchestrationIds,
      TimeSpan? timeout = null)
    {
      using (OrchestrationComponent component = requestContext.CreateComponent<OrchestrationComponent>())
      {
        if (string.IsNullOrEmpty(hubName))
          component.RemovePoisonedOrchestrations(orchestrationIds, timeout);
        else
          component.RemovePoisonedOrchestrations(hubName, orchestrationIds, timeout);
      }
    }
  }
}
