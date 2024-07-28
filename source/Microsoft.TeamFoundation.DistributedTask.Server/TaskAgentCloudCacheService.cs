// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskAgentCloudCacheService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class TaskAgentCloudCacheService : VssMemoryCacheService<int, TaskAgentCloud>
  {
    private const string s_area = "TaskAgentCloudCacheService";

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      bool flag = systemRequestContext.IsFeatureEnabled("DistributedTask.AgentCloudCacheInvalidation");
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "DistributedTask", SqlNotificationEventIds.AgentCloudChanged, new SqlNotificationCallback(this.CloudChangedCallback), !flag);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      base.ServiceEnd(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "DistributedTask", SqlNotificationEventIds.AgentCloudChanged, new SqlNotificationCallback(this.CloudChangedCallback), false);
    }

    private void CloudChangedCallback(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceVerbose(nameof (TaskAgentCloudCacheService), "Received cloud changed event with data: {0}", (object) eventData);
      int result;
      if (!int.TryParse(eventData, out result))
        requestContext.TraceError(nameof (TaskAgentCloudCacheService), "Cannot parse poolId from data: {0}", (object) eventData);
      else
        this.Remove(requestContext, result);
    }
  }
}
