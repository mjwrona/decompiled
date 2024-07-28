// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationTaskService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class TeamFoundationTaskService : IVssFrameworkService, ITeamFoundationTaskService
  {
    internal const string c_notificationPool = "NotificationPool";
    internal const string c_taskPool = "TaskPool";
    private TaskManager<Guid> m_tasks;
    private TaskManager<int> m_notificationTasks;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_tasks = new TaskManager<Guid>(systemRequestContext, "TaskPool", FrameworkServerConstants.MaxTaskRunners, systemRequestContext.ServiceHost.InstanceId);
      this.m_notificationTasks = new TaskManager<int>(systemRequestContext, "NotificationPool", FrameworkServerConstants.MaxHighPriorityTaskRunners, systemRequestContext.ServiceHost.ServiceHostInternal().DatabaseId);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => this.Shutdown();

    internal void Shutdown()
    {
      if (this.m_notificationTasks != null)
      {
        this.m_notificationTasks.Dispose();
        this.m_notificationTasks = (TaskManager<int>) null;
      }
      if (this.m_tasks == null)
        return;
      this.m_tasks.Dispose();
      this.m_tasks = (TaskManager<Guid>) null;
    }

    public void AddTask(IVssRequestContext requestContext, TeamFoundationTaskCallback callback) => this.AddTask(requestContext.ServiceHost.InstanceId, new TeamFoundationTask(callback));

    public void AddTask(IVssRequestContext requestContext, TeamFoundationTask task) => this.AddTask(requestContext.ServiceHost.InstanceId, task);

    public void AddTask(Guid instanceId, TeamFoundationTask task)
    {
      ArgumentUtility.CheckForEmptyGuid(instanceId, nameof (instanceId));
      if (this.m_tasks == null)
        return;
      this.m_tasks.AddTask(instanceId, (TeamFoundationTask<Guid>) task);
    }

    public void RemoveTask(IVssRequestContext requestContext, TeamFoundationTaskCallback callback) => this.RemoveTask(requestContext.ServiceHost.InstanceId, new TeamFoundationTask(callback));

    public void RemoveTask(IVssRequestContext requestContext, TeamFoundationTask task) => this.RemoveTask(requestContext.ServiceHost.InstanceId, task);

    public void RemoveTask(Guid instanceId, TeamFoundationTask task)
    {
      if (this.m_tasks == null)
        return;
      this.m_tasks.RemoveTask(instanceId, (TeamFoundationTask<Guid>) task);
    }

    public void RemoveAllTasks(Guid instanceId)
    {
      if (this.m_tasks == null)
        return;
      this.m_tasks.RemoveAllTasksForHost(instanceId);
    }

    public void AddTask(int databaseId, TeamFoundationTask<int> task)
    {
      if (this.m_notificationTasks == null)
        return;
      this.m_notificationTasks.AddTask(databaseId, task);
    }

    public void RemoveTask(int databaseId, TeamFoundationTask<int> task)
    {
      if (this.m_notificationTasks == null)
        return;
      this.m_notificationTasks.RemoveTask(databaseId, task);
    }
  }
}
