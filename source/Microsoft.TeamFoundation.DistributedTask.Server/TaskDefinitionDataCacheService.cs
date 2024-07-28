// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskDefinitionDataCacheService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class TaskDefinitionDataCacheService : 
    ITaskDefinitionDataCacheService,
    IVssFrameworkService
  {
    private const string c_layer = "TaskDefinitionDataCacheService";
    private TaskDefinitionDataRefreshCache m_cache;

    public void ServiceStart(IVssRequestContext requestContext) => this.m_cache = new TaskDefinitionDataRefreshCache(requestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void Invalidate(IVssRequestContext requestContext)
    {
      requestContext.TraceInfo(10015144, nameof (TaskDefinitionDataCacheService), "Invalidating task definition cache");
      this.m_cache = new TaskDefinitionDataRefreshCache(requestContext);
    }

    public TaskDefinitionDataResult GetTasks(IVssRequestContext requestContext) => this.m_cache.Get(requestContext);
  }
}
