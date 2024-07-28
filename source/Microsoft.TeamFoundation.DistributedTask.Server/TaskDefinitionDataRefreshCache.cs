// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskDefinitionDataRefreshCache
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class TaskDefinitionDataRefreshCache
  {
    private const string c_layer = "TaskDefinitionDataRefreshCache";
    private static readonly TimeSpan c_collectionRefreshInterval = TimeSpan.FromMinutes(60.0);
    private static readonly TimeSpan c_deploymentRefreshInterval = TimeSpan.FromMinutes(60.0);
    private static readonly TimeSpan c_slowThreshold = TimeSpan.FromSeconds(10.0);
    private const string c_refreshIntervalRegistryPath = "/Service/DistributedTask/TaskCacheRefreshInterval";
    private VssRefreshCache<TaskDefinitionDataResult> m_cache;
    private TaskDefinitionDataRefreshCache.TaskCacheStatistics m_statistics;

    public TaskDefinitionDataRefreshCache(IVssRequestContext requestContext)
    {
      int num = new Random(Guid.NewGuid().GetHashCode()).Next(0, 120);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      TimeSpan timeSpan = requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? TaskDefinitionDataRefreshCache.c_deploymentRefreshInterval : TaskDefinitionDataRefreshCache.c_collectionRefreshInterval;
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) "/Service/DistributedTask/TaskCacheRefreshInterval";
      TimeSpan defaultValue = timeSpan;
      this.m_cache = new VssRefreshCache<TaskDefinitionDataResult>(service.GetValue<TimeSpan>(requestContext1, in local, false, defaultValue).Add(TimeSpan.FromSeconds((double) num)), new Func<IVssRequestContext, TaskDefinitionDataResult>(this.Refresh), true);
    }

    public TaskDefinitionDataResult Get(IVssRequestContext requestContext)
    {
      this.m_statistics?.TraceEnteringGet(requestContext);
      return this.m_cache.Get(requestContext);
    }

    private TaskDefinitionDataResult Refresh(IVssRequestContext requestContext)
    {
      using (new MethodScope(requestContext, nameof (TaskDefinitionDataRefreshCache), nameof (Refresh)))
      {
        requestContext.TraceInfo(10015145, nameof (TaskDefinitionDataRefreshCache), "Populating task definition cache");
        TaskDefinitionDataResult definitions = new TaskDefinitionDataResult();
        int num1 = 0;
        if (!requestContext.ServiceHost.HostType.Equals((object) TeamFoundationHostType.Application))
        {
          using (TaskDefinitionComponent component = requestContext.CreateComponent<TaskDefinitionComponent>())
          {
            using (requestContext.TraceSlowCall(TaskDefinitionDataRefreshCache.c_slowThreshold, nameof (TaskDefinitionDataRefreshCache), "GetTaskDefinitions took more than: {0}", (object) TaskDefinitionDataRefreshCache.c_slowThreshold))
            {
              foreach (TaskDefinitionData taskDefinition in component.GetTaskDefinitions(new Guid?(), (TaskVersion) null, false))
              {
                taskDefinition.SetHostType(requestContext);
                try
                {
                  int num2 = num1;
                  byte[] metadataDocument = taskDefinition.MetadataDocument;
                  int length = metadataDocument != null ? metadataDocument.Length : 0;
                  num1 = checked (num2 + length);
                }
                catch (OverflowException ex)
                {
                  num1 = int.MaxValue;
                }
                definitions.Add(taskDefinition);
              }
            }
          }
        }
        requestContext.TraceInfo(10015147, nameof (TaskDefinitionDataRefreshCache), "Retrieved task definition count: {0}; Size in kb: {1}", (object) definitions.Count, (object) (num1 / 1024));
        this.m_statistics = new TaskDefinitionDataRefreshCache.TaskCacheStatistics(definitions, this.m_statistics);
        return definitions;
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
        TaskDefinitionDataResult definitions,
        TaskDefinitionDataRefreshCache.TaskCacheStatistics previous)
      {
        this.m_refreshTime = DateTime.Now;
        this.m_currentHash = this.CreateHashCode(definitions);
        this.m_previousHash = previous != null ? previous.m_currentHash : 0;
      }

      public void TraceEnteringGet(IVssRequestContext requestContext)
      {
        if (this.m_previousHash == 0 || this.m_currentHash == this.m_previousHash || !(DateTime.Now - this.m_refreshTime > TaskDefinitionDataRefreshCache.TaskCacheStatistics.c_fixDetectionThreshold) || Interlocked.Exchange(ref this.m_fixDetected, 1) != 0)
          return;
        requestContext.TraceAlways(10015141, TraceLevel.Info, "DistributedTask", nameof (TaskDefinitionDataRefreshCache), "Tasks changed");
      }

      private int CreateHashCode(TaskDefinitionDataResult definitions)
      {
        int hashCode = 0;
        foreach (TaskDefinitionData definition in (IEnumerable<TaskDefinitionData>) definitions)
          hashCode ^= string.Format("{0}{1}", (object) definition.Id, (object) definition.Version).GetHashCode();
        return hashCode;
      }
    }
  }
}
