// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskAgentSessionCacheService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class TaskAgentSessionCacheService : 
    VssMemoryCacheService<Guid, TaskAgentSessionData>
  {
    private static readonly MemoryCacheConfiguration<Guid, TaskAgentSessionData> s_defaultConfiguration = new MemoryCacheConfiguration<Guid, TaskAgentSessionData>().WithMaxElements(10000).WithCleanupInterval(TimeSpan.FromMinutes(5.0)).WithInactivityInterval(TimeSpan.FromMinutes(10.0));

    public TaskAgentSessionCacheService()
      : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, TaskAgentSessionCacheService.s_defaultConfiguration)
    {
    }

    public bool IsRevoked(IVssRequestContext requestContext, Guid sessionId) => requestContext.GetService<DeletedAgentSessionCacheService>().TryPeekValue(requestContext, sessionId, out TaskAgentSessionData _);

    public void Remove(
      IVssRequestContext requestContext,
      IEnumerable<TaskAgentSessionData> sessions)
    {
      foreach (TaskAgentSessionData session in sessions)
        this.RemoveSession(requestContext, session.SessionId);
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "DistributedTask", SqlNotificationEventIds.SessionDeleted, new SqlNotificationCallback(this.SessionDeletedCallback), true);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "DistributedTask", SqlNotificationEventIds.SessionDeleted, new SqlNotificationCallback(this.SessionDeletedCallback), false);
      base.ServiceEnd(systemRequestContext);
    }

    private void RemoveSession(IVssRequestContext requestContext, Guid sessionId)
    {
      requestContext.GetService<DeletedAgentSessionCacheService>().TryAdd(requestContext, sessionId, new TaskAgentSessionData()
      {
        SessionId = sessionId
      });
      if (!this.Remove(requestContext, sessionId))
        return;
      requestContext.TraceVerbose("ResourceService", "Removed session {0} from cache", (object) sessionId);
    }

    private void SessionDeletedCallback(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      Guid result;
      if (!Guid.TryParse(eventData, out result))
        return;
      this.RemoveSession(requestContext, result);
    }
  }
}
