// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DistributedTaskHubService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public class DistributedTaskHubService : IDistributedTaskHubService, IVssFrameworkService
  {
    private bool m_cacheFresh;
    private int m_pipelineVersion;
    private ILockName m_hubCacheLock;
    private ITaskHubResolver m_defaultHubResolver;
    private IDisposableReadOnlyList<ITaskHubResolver> m_allHubResolvers;
    private IDisposableReadOnlyList<TaskHubExtension> m_allHubExtensions;
    private IDictionary<string, TaskHubExtension> m_allHubExtensionsByType;
    private IDictionary<string, int> m_allHubVersionsByName;
    private ConcurrentDictionary<string, TaskHub> m_hubCache = new ConcurrentDictionary<string, TaskHub>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private const string c_hubVersion = "HubVersion";
    private const string c_pipelineVersion = "Pipelines/Version";
    private const string s_hubSettingsRootPath = "/Service/DistributedTask/";
    internal static readonly RegistryQuery s_hubVersionKeyPath = (RegistryQuery) "/Service/DistributedTask/HubVersion";
    private static readonly RegistryQuery s_hubPipelineVersionKeyPath = (RegistryQuery) "/Service/DistributedTask/Pipelines/Version";
    private static readonly RegistryQuery s_hubSettingsFilter = (RegistryQuery) "/Service/DistributedTask/...";

    public TaskHub CreateTaskHub(
      IVssRequestContext requestContext,
      string name,
      string dataspaceCategory)
    {
      TaskHubExtension hubExtension;
      if (!this.m_allHubExtensionsByType.TryGetValue(name, out hubExtension))
        throw new InvalidOperationException(TaskResources.HubExtensionNotFound((object) name));
      TaskHub hub;
      using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>("DistributedTaskOrchestration"))
        hub = component.CreateHub(name, dataspaceCategory);
      hub.PipelineVersion = this.m_pipelineVersion;
      hub.Version = this.GetTaskHubVersion(hubExtension);
      hub.Extension = hubExtension;
      this.m_hubCache[hub.Name] = hub;
      return hub;
    }

    public TaskHub GetTaskHub(IVssRequestContext requestContext, string name, bool includeDefault)
    {
      this.EnsureHubsLoaded(requestContext);
      TaskHub taskHub;
      if (this.m_hubCache.TryGetValue(name, out taskHub))
        return taskHub;
      this.EnsureHubsLoaded(requestContext, true);
      return this.m_hubCache.TryGetValue(name, out taskHub) || !includeDefault ? taskHub : this.GetDefaultTaskHub(requestContext);
    }

    internal TaskHub GetDefaultTaskHub(IVssRequestContext requestContext)
    {
      TaskHub defaultTaskHub = (TaskHub) null;
      if (this.m_defaultHubResolver != null)
      {
        defaultTaskHub = this.m_defaultHubResolver.GetDefaultTaskHub(requestContext);
        if (defaultTaskHub != null)
        {
          TaskHubExtension hubExtension;
          if (!this.m_allHubExtensionsByType.TryGetValue(defaultTaskHub.Name, out hubExtension))
          {
            requestContext.TraceError("OrchestrationService", "TaskHub {0} does not have a valid extension and will not be loaded", (object) defaultTaskHub.Name);
            return (TaskHub) null;
          }
          defaultTaskHub.PipelineVersion = this.m_pipelineVersion;
          defaultTaskHub.Version = this.GetTaskHubVersion(hubExtension);
          defaultTaskHub.Extension = hubExtension;
        }
      }
      return defaultTaskHub;
    }

    private TaskHub EnsureHub(IVssRequestContext requestContext, string hubName)
    {
      TaskHub taskHub;
      if (this.m_hubCache.TryGetValue(hubName, out taskHub))
        return taskHub;
      this.EnsureHubsLoaded(requestContext, true);
      if (this.m_hubCache.TryGetValue(hubName, out taskHub))
        return taskHub;
      throw new TaskHubNotFoundException(TaskResources.HubNotFound((object) hubName));
    }

    private void EnsureHubsLoaded(IVssRequestContext requestContext, bool force = false)
    {
      if (this.m_cacheFresh && !force)
        return;
      List<TaskHub> taskHubList = new List<TaskHub>();
      using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>("DistributedTaskOrchestration"))
        taskHubList.AddRange((IEnumerable<TaskHub>) component.GetHubs());
      using (requestContext.Lock(this.m_hubCacheLock))
      {
        this.m_hubCache.Clear();
        foreach (TaskHub taskHub in taskHubList)
        {
          TaskHubExtension hubExtension;
          if (!this.m_allHubExtensionsByType.TryGetValue(taskHub.Name, out hubExtension))
          {
            requestContext.TraceError("OrchestrationService", "TaskHub {0} does not have a valid extension and will not be loaded", (object) taskHub.Name);
          }
          else
          {
            taskHub.PipelineVersion = this.m_pipelineVersion;
            taskHub.Version = this.GetTaskHubVersion(hubExtension);
            taskHub.Extension = hubExtension;
            this.m_hubCache[taskHub.Name] = taskHub;
          }
        }
        this.m_cacheFresh = true;
      }
    }

    private void SettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      bool flag = false;
      if (this.m_allHubExtensions != null)
      {
        foreach (TaskHubExtension allHubExtension in (IEnumerable<TaskHubExtension>) this.m_allHubExtensions)
        {
          string query = "/Service/DistributedTask/" + allHubExtension.OrchestrationVersionKey;
          int num = service.GetValue<int>(requestContext, (RegistryQuery) query, true, allHubExtension.DefaultOrchestrationVersion);
          if (this.m_allHubVersionsByName[allHubExtension.HubName] != num)
          {
            this.m_allHubVersionsByName[allHubExtension.HubName] = num;
            flag = true;
          }
        }
      }
      int num1 = this.ModifyPipelineVersionBasedOnFeatureFlags(requestContext, service.GetValue<int>(requestContext, in DistributedTaskHubService.s_hubPipelineVersionKeyPath, true));
      if (num1 != this.m_pipelineVersion)
      {
        flag = true;
        this.m_pipelineVersion = num1;
      }
      if (!flag)
        return;
      using (requestContext.Lock(this.m_hubCacheLock))
      {
        foreach (TaskHub taskHub in (IEnumerable<TaskHub>) this.m_hubCache.Values)
        {
          taskHub.PipelineVersion = this.m_pipelineVersion;
          taskHub.Version = this.GetTaskHubVersion(taskHub.Extension);
        }
      }
    }

    private int GetTaskHubVersion(TaskHubExtension hubExtension) => this.m_allHubVersionsByName[hubExtension.HubName];

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      this.m_hubCacheLock = requestContext.ServiceHost.CreateUniqueLockName("DistributedTask.HubCache");
      this.m_allHubExtensions = requestContext.GetExtensions<TaskHubExtension>();
      this.m_allHubExtensionsByType = (IDictionary<string, TaskHubExtension>) this.m_allHubExtensions.ToDictionary<TaskHubExtension, string>((Func<TaskHubExtension, string>) (x => x.HubName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_allHubResolvers = requestContext.GetExtensions<ITaskHubResolver>();
      if (this.m_allHubResolvers.Count > 0)
      {
        if (this.m_allHubResolvers.Count > 1)
          requestContext.TraceWarning("OrchestrationService", TaskResources.MultipleHubResolversNotSupported((object) string.Join(", ", this.m_allHubResolvers.Select<ITaskHubResolver, string>((Func<ITaskHubResolver, string>) (x => x.GetType().FullName)))));
        this.m_defaultHubResolver = this.m_allHubResolvers.OrderBy<ITaskHubResolver, string>((Func<ITaskHubResolver, string>) (x => x.HubName)).First<ITaskHubResolver>();
      }
      IVssRegistryService registryService = requestContext.GetService<IVssRegistryService>();
      this.m_allHubVersionsByName = (IDictionary<string, int>) this.m_allHubExtensions.ToDictionary<TaskHubExtension, string, int>((Func<TaskHubExtension, string>) (x => x.HubName), (Func<TaskHubExtension, int>) (x => registryService.GetValue<int>(requestContext, (RegistryQuery) ("/Service/DistributedTask/" + x.OrchestrationVersionKey), true, x.DefaultOrchestrationVersion)), (IEqualityComparer<string>) StringComparer.Ordinal);
      RegistryQuery registryQuery1 = new RegistryQuery("/FeatureAvailability/Entries/DistributedTask.SetStageInProgressOnOrchestrationStart/AvailabilityState");
      RegistryQuery registryQuery2 = new RegistryQuery("/FeatureAvailability/Entries/DistributedTask.ReplaceQueuedGraphNodes/AvailabilityState");
      RegistryQuery registryQuery3 = new RegistryQuery("/FeatureAvailability/Entries/DistributedTask.CancelCheckpointsOnRetry/AvailabilityState");
      registryService.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.SettingsChanged), true, (IEnumerable<RegistryQuery>) new RegistryQuery[4]
      {
        DistributedTaskHubService.s_hubSettingsFilter,
        registryQuery1,
        registryQuery2,
        registryQuery3
      });
      this.m_pipelineVersion = this.ModifyPipelineVersionBasedOnFeatureFlags(requestContext, registryService.GetValue<int>(requestContext, in DistributedTaskHubService.s_hubPipelineVersionKeyPath, true));
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.SettingsChanged));
      this.m_hubCache.Clear();
      if (this.m_allHubExtensions != null)
      {
        foreach (TaskHubExtension allHubExtension in (IEnumerable<TaskHubExtension>) this.m_allHubExtensions)
          allHubExtension.Dispose();
        this.m_allHubExtensions.Dispose();
        this.m_allHubExtensions = (IDisposableReadOnlyList<TaskHubExtension>) null;
      }
      if (this.m_allHubResolvers == null)
        return;
      this.m_allHubResolvers.Dispose();
      this.m_allHubResolvers = (IDisposableReadOnlyList<ITaskHubResolver>) null;
    }

    private int ModifyPipelineVersionBasedOnFeatureFlags(
      IVssRequestContext requestContext,
      int version)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.SetStageInProgressOnOrchestrationStart"))
        version = Math.Min(14, version);
      if (!requestContext.IsFeatureEnabled("DistributedTask.ReplaceQueuedGraphNodes"))
        version = Math.Min(15, version);
      if (!requestContext.IsFeatureEnabled("DistributedTask.CancelCheckpointsOnRetry"))
        version = Math.Min(16, version);
      if (!requestContext.IsFeatureEnabled("DistributedTask.UseNewestNodesForGraphRetry"))
        version = Math.Min(17, version);
      return version;
    }
  }
}
