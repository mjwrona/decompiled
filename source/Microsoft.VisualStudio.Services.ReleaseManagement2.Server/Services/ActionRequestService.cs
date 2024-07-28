// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ActionRequestService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ActionRequestService : ReleaseManagement2ServiceBase
  {
    private static readonly Guid RetainBuildActionRequestsProcessorJobId = new Guid("CCE846E5-50EC-4EED-B47A-413C69D3FDEA");
    private static readonly Guid StartReleaseEnvironmentActionRequestJobId = new Guid("B240D773-C168-4A03-8914-A63BCB9982EB");
    private static readonly Guid ManageBuildRetentionLeasesActionRequestsProcessorJobId = new Guid("83376822-C4F9-4F26-9A3E-C6DA5F9CE9DE");

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public ActionRequest AddActionRequest(
      IVssRequestContext context,
      Guid projectId,
      ActionRequest actionRequest)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (actionRequest == null)
        throw new ArgumentNullException(nameof (actionRequest));
      ActionRequestService.InitializeActionRequest(actionRequest);
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", nameof (AddActionRequest), 1971082))
      {
        Func<ActionRequestSqlComponent, ActionRequest> action = (Func<ActionRequestSqlComponent, ActionRequest>) (component => component.AddActionRequest(projectId, actionRequest));
        return context.ExecuteWithinUsingWithComponent<ActionRequestSqlComponent, ActionRequest>(action);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public IEnumerable<ActionRequest> GetActionRequests(
      IVssRequestContext context,
      ActionRequestType actionRequestType,
      int top,
      int maxRetries,
      int continuationToken)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", nameof (GetActionRequests), 1971079))
      {
        Func<ActionRequestSqlComponent, IEnumerable<ActionRequest>> action = (Func<ActionRequestSqlComponent, IEnumerable<ActionRequest>>) (component => component.GetActionRequests((int) actionRequestType, top, maxRetries, continuationToken));
        return context.ExecuteWithinUsingWithComponent<ActionRequestSqlComponent, IEnumerable<ActionRequest>>(action);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public void DeleteActionRequest(IVssRequestContext context, int actionRequestId)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", nameof (DeleteActionRequest), 1971081))
      {
        Action<ActionRequestSqlComponent> action = (Action<ActionRequestSqlComponent>) (component => component.DeleteActionRequest(actionRequestId));
        context.ExecuteWithinUsingWithComponent<ActionRequestSqlComponent>(action);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public ActionRequest UpdateActionRequest(
      IVssRequestContext context,
      ActionRequest actionRequest)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (actionRequest == null)
        throw new ArgumentNullException(nameof (actionRequest));
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", nameof (UpdateActionRequest), 1971080))
      {
        Func<ActionRequestSqlComponent, ActionRequest> action = (Func<ActionRequestSqlComponent, ActionRequest>) (component => component.UpdateActionRequest(actionRequest));
        return context.ExecuteWithinUsingWithComponent<ActionRequestSqlComponent, ActionRequest>(action);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This is logged for diagnosability")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "We require default value to be false.")]
    public void QueueActionRequestsProcessorJob(
      IVssRequestContext requestContext,
      ActionRequestType requestType,
      bool delayJob,
      bool highPriority = false)
    {
      try
      {
        ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
        switch (requestType)
        {
          case ActionRequestType.RetainBuild:
            if (delayJob)
            {
              service.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
              {
                ActionRequestService.RetainBuildActionRequestsProcessorJobId
              }, 300);
              break;
            }
            service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
            {
              ActionRequestService.RetainBuildActionRequestsProcessorJobId
            }, false);
            break;
          case ActionRequestType.StartReleaseEnvironment:
            service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
            {
              ActionRequestService.StartReleaseEnvironmentActionRequestJobId
            }, (highPriority ? 1 : 0) != 0);
            break;
          case ActionRequestType.ManageBuildRetentionLease:
            if (delayJob)
            {
              service.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
              {
                ActionRequestService.ManageBuildRetentionLeasesActionRequestsProcessorJobId
              }, 300);
              break;
            }
            service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
            {
              ActionRequestService.ManageBuildRetentionLeasesActionRequestsProcessorJobId
            }, false);
            break;
          default:
            throw new NotImplementedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cannot find ActionRequestType: {0}", (object) requestType));
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1971083, "ReleaseManagementService", "JobLayer", ex);
      }
    }

    private static void InitializeActionRequest(ActionRequest actionRequest)
    {
      actionRequest.NumberOfAttempts = 0;
      actionRequest.Id = 0;
    }
  }
}
