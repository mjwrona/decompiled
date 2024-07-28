// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LongRunningTaskService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LongRunningTaskService : IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private HashSet<Guid> m_runningTasks = new HashSet<Guid>();
    private const string s_area = "LongRunningTaskService";
    private const string s_layer = "LongRunningTaskService";

    public void ServiceStart(IVssRequestContext context) => this.m_serviceHostId = context.ServiceHost.InstanceId;

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    internal void ScheduleLongRunningTask(
      IVssRequestContext context,
      Guid taskId,
      TeamFoundationTaskCallback callback,
      object taskArgs,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      this.ValidateRequestContext(context);
      if (this.m_runningTasks.Contains(taskId))
        throw new TaskScheduleExistsException(string.Format("Task {0} has already been scheduled on host {1}.", (object) taskId, (object) context.ServiceHost.InstanceId));
      this.m_runningTasks.Add(taskId);
      IVssDeploymentServiceHost deploymentHost = context.ServiceHost.DeploymentServiceHost;
      Guid hostId = context.ServiceHost.InstanceId;
      new Task((Action) (() =>
      {
        try
        {
          using (IVssRequestContext systemContext = deploymentHost.CreateSystemContext(false))
          {
            try
            {
              using (IVssRequestContext requestContext = systemContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(systemContext, hostId, RequestContextType.SystemContext, false))
                callback(requestContext, taskArgs);
            }
            catch (HostDoesNotExistException ex)
            {
              systemContext.TraceException(3241645, TraceLevel.Info, nameof (LongRunningTaskService), nameof (LongRunningTaskService), (Exception) ex);
            }
            catch (Exception ex)
            {
              systemContext.TraceException(3241646, nameof (LongRunningTaskService), nameof (LongRunningTaskService), ex);
            }
          }
        }
        finally
        {
          this.m_runningTasks.Remove(taskId);
        }
      }), cancellationToken, TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning).Start();
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!this.m_serviceHostId.Equals(requestContext.ServiceHost.InstanceId))
        throw new InvalidRequestContextHostException(FrameworkResources.ServiceRequestContextHostMessage((object) this.GetType().Name, (object) this.m_serviceHostId, (object) requestContext.ServiceHost.InstanceId));
    }
  }
}
