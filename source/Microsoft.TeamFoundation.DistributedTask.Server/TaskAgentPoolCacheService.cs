// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskAgentPoolCacheService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class TaskAgentPoolCacheService : VssMemoryCacheService<int, TaskAgentPoolData>
  {
    private const string s_area = "TaskAgentPoolCacheService";

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "DistributedTask", SqlNotificationEventIds.PoolChanged, new SqlNotificationCallback(this.PoolChangedCallback), true);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      base.ServiceEnd(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "DistributedTask", SqlNotificationEventIds.PoolChanged, new SqlNotificationCallback(this.PoolChangedCallback), false);
    }

    public override bool TryGetValue(
      IVssRequestContext requestContext,
      int key,
      out TaskAgentPoolData value)
    {
      TaskAgentPoolData taskAgentPoolData;
      int num = base.TryGetValue(requestContext, key, out taskAgentPoolData) ? 1 : 0;
      value = taskAgentPoolData?.Clone();
      return num != 0;
    }

    private void PoolChangedCallback(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceVerbose(nameof (TaskAgentPoolCacheService), "Received pool changed event with data: {0}", (object) eventData);
      int result;
      if (!int.TryParse(eventData, out result))
        requestContext.TraceError(nameof (TaskAgentPoolCacheService), "Cannot parse poolId from data: {0}", (object) eventData);
      else
        this.Remove(requestContext, result);
    }
  }
}
