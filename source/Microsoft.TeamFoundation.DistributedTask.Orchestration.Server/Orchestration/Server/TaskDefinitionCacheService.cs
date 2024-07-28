// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskDefinitionCacheService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal sealed class TaskDefinitionCacheService : 
    ITaskDefinitionCacheService,
    IVssFrameworkService
  {
    private const string c_layer = "TaskDefinitionCacheService";
    private TaskDefinitionRefreshCache m_cache;
    private ConcurrentDictionary<Guid, TaskDefinitionRefreshCache> m_caches;

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void Invalidate(IVssRequestContext requestContext)
    {
      requestContext.TraceInfo(10015144, nameof (TaskDefinitionCacheService), "Invalidating task definition cache");
      this.m_cache = (TaskDefinitionRefreshCache) null;
      this.m_caches = (ConcurrentDictionary<Guid, TaskDefinitionRefreshCache>) null;
    }

    public TaskDefinitionResult GetTasks(IVssRequestContext requestContext, Guid? targetInstanceId)
    {
      TaskDefinitionRefreshCache definitionRefreshCache1;
      if (!targetInstanceId.HasValue)
      {
        definitionRefreshCache1 = this.m_cache;
        if (definitionRefreshCache1 == null)
        {
          TaskDefinitionRefreshCache definitionRefreshCache2 = new TaskDefinitionRefreshCache(requestContext, new Guid?());
          definitionRefreshCache1 = Interlocked.CompareExchange<TaskDefinitionRefreshCache>(ref this.m_cache, definitionRefreshCache2, (TaskDefinitionRefreshCache) null) ?? definitionRefreshCache2;
        }
      }
      else
      {
        ConcurrentDictionary<Guid, TaskDefinitionRefreshCache> concurrentDictionary1 = this.m_caches;
        if (concurrentDictionary1 == null)
        {
          ConcurrentDictionary<Guid, TaskDefinitionRefreshCache> concurrentDictionary2 = new ConcurrentDictionary<Guid, TaskDefinitionRefreshCache>();
          concurrentDictionary1 = Interlocked.CompareExchange<ConcurrentDictionary<Guid, TaskDefinitionRefreshCache>>(ref this.m_caches, concurrentDictionary2, (ConcurrentDictionary<Guid, TaskDefinitionRefreshCache>) null) ?? concurrentDictionary2;
        }
        definitionRefreshCache1 = concurrentDictionary1.GetOrAdd(targetInstanceId.Value, (Func<Guid, TaskDefinitionRefreshCache>) (x => new TaskDefinitionRefreshCache(requestContext, targetInstanceId)));
      }
      return definitionRefreshCache1.Get(requestContext);
    }
  }
}
