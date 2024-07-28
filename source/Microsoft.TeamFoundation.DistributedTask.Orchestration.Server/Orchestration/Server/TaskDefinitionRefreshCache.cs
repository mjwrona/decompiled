// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskDefinitionRefreshCache
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class TaskDefinitionRefreshCache
  {
    private const string c_layer = "TaskDefinitionRefreshCache";
    private static readonly TimeSpan c_collectionRefreshInterval = TimeSpan.FromMinutes(60.0);
    private static readonly TimeSpan c_deploymentRefreshInterval = TimeSpan.FromMinutes(60.0);
    private const string c_refreshIntervalRegistryPath = "/Service/DistributedTask/TaskCacheRefreshInterval";
    private readonly Guid? m_targetInstanceId;
    private VssRefreshCache<TaskDefinitionResult> m_cache;
    private TaskDefinitionRefreshCache.TaskCacheStatistics m_statistics;

    public TaskDefinitionRefreshCache(IVssRequestContext requestContext, Guid? targetInstanceId)
    {
      this.m_targetInstanceId = targetInstanceId;
      int num = new Random(Guid.NewGuid().GetHashCode()).Next(0, 120);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      TimeSpan timeSpan = requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? TaskDefinitionRefreshCache.c_deploymentRefreshInterval : TaskDefinitionRefreshCache.c_collectionRefreshInterval;
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) "/Service/DistributedTask/TaskCacheRefreshInterval";
      TimeSpan defaultValue = timeSpan;
      this.m_cache = new VssRefreshCache<TaskDefinitionResult>(service.GetValue<TimeSpan>(requestContext1, in local, false, defaultValue).Add(TimeSpan.FromSeconds((double) num)), new Func<IVssRequestContext, TaskDefinitionResult>(this.Refresh), true);
    }

    public TaskDefinitionResult Get(IVssRequestContext requestContext)
    {
      this.m_statistics?.TraceEnteringGet(requestContext);
      return this.m_cache.Get(requestContext);
    }

    private TaskDefinitionResult Refresh(IVssRequestContext requestContext)
    {
      using (new MethodScope(requestContext, nameof (TaskDefinitionRefreshCache), nameof (Refresh)))
      {
        requestContext.TraceInfo(10015145, nameof (TaskDefinitionRefreshCache), "Populating task definition cache");
        TaskDefinitionResult definitions1 = new TaskDefinitionResult();
        if (!requestContext.ServiceHost.HostType.Equals((object) TeamFoundationHostType.Application))
        {
          TaskAgentHttpClient taskAgentHttpClient = this.m_targetInstanceId.HasValue ? requestContext.GetClient<TaskAgentHttpClient>(this.m_targetInstanceId.Value) : requestContext.GetClient<TaskAgentHttpClient>();
          bool? nullable = new bool?(true);
          Guid? taskId = new Guid?();
          bool? scopeLocal = nullable;
          bool? allVersions = new bool?();
          CancellationToken cancellationToken = new CancellationToken();
          List<TaskDefinition> definitions2 = taskAgentHttpClient.GetTaskDefinitionsAsync(taskId, scopeLocal: scopeLocal, allVersions: allVersions, cancellationToken: cancellationToken).SyncResult<List<TaskDefinition>>();
          definitions1.Add((IEnumerable<TaskDefinition>) definitions2);
        }
        this.m_statistics = new TaskDefinitionRefreshCache.TaskCacheStatistics(definitions1, this.m_statistics);
        return definitions1;
      }
    }

    private sealed class TaskCacheStatistics
    {
      private static readonly TimeSpan c_fixDetectionThreshold = TimeSpan.FromMinutes(1.0);
      private DateTime m_refreshTime;
      private int m_currentHash;
      private int m_previousHash;
      private int m_fixDetected;

      public TaskCacheStatistics(
        TaskDefinitionResult definitions,
        TaskDefinitionRefreshCache.TaskCacheStatistics previous)
      {
        this.m_refreshTime = DateTime.Now;
        this.m_currentHash = this.CreateHashCode(definitions);
        this.m_previousHash = previous != null ? previous.m_currentHash : 0;
      }

      public void TraceEnteringGet(IVssRequestContext requestContext)
      {
        if (this.m_previousHash == 0 || this.m_currentHash == this.m_previousHash || !(DateTime.Now - this.m_refreshTime > TaskDefinitionRefreshCache.TaskCacheStatistics.c_fixDetectionThreshold) || Interlocked.Exchange(ref this.m_fixDetected, 1) != 0)
          return;
        requestContext.TraceAlways(10015141, TraceLevel.Info, "DistributedTask", nameof (TaskDefinitionRefreshCache), "Tasks changed");
      }

      private int CreateHashCode(TaskDefinitionResult definitions)
      {
        int hashCode = 0;
        foreach (TaskDefinition definition in (IEnumerable<TaskDefinition>) definitions)
          hashCode ^= string.Format("{0}{1}", (object) definition.Id, (object) definition.Version).GetHashCode();
        return hashCode;
      }
    }
  }
}
