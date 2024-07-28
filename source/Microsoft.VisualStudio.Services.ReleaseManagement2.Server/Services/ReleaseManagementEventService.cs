// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseManagementEventService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  internal class ReleaseManagementEventService : IReleaseManagementEventService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void PublishDecisionPoint(IVssRequestContext requestContext, object notificationEvent) => requestContext.GetService<ITeamFoundationEventService>().PublishDecisionPoint(requestContext, notificationEvent);

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safe handle and trace exception.")]
    public void PublishNotification(IVssRequestContext requestContext, object notificationEvent)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.UseTaskDispatcherForEvents"))
        requestContext.Fork<ReleaseManagementEventDispatcher>((Func<IVssRequestContext, Task>) (forkedContext =>
        {
          using (ReleaseManagementTimer.Create(forkedContext, "ReleaseEventDispatcher", "SyncPublishNotification", 1976450))
          {
            try
            {
              forkedContext.GetService<ITeamFoundationEventService>().SyncPublishNotification(forkedContext, notificationEvent);
            }
            catch (Exception ex)
            {
              forkedContext.TraceException(1976451, "ReleaseManagementService", "ReleaseEventDispatcher", ex);
            }
            return Task.CompletedTask;
          }
        }), nameof (PublishNotification));
      else
        requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, notificationEvent);
    }
  }
}
