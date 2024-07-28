// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.RedeployTriggerDeploymentGroupEventService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
  public class RedeployTriggerDeploymentGroupEventService : ReleaseManagement2ServiceBase
  {
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public IList<RedeployTriggerDeploymentGroupEvent> AddRedeployTriggerDeploymentGroupEvents(
      IVssRequestContext context,
      Guid projectId,
      string eventType,
      int deploymentGroupId,
      IList<RedeployTriggerDeploymentGroupEvent> events)
    {
      using (ReleaseManagementTimer.Create(context, "Service", "RedeployTriggerDeploymentGroupEventService.AddRedeployTriggerDeploymentGroupEvents", 1976427))
      {
        Func<RedeployTriggerDeploymentGroupEventSqlComponent, IList<RedeployTriggerDeploymentGroupEvent>> action = (Func<RedeployTriggerDeploymentGroupEventSqlComponent, IList<RedeployTriggerDeploymentGroupEvent>>) (component => component.AddRedeployTriggerDeploymentGroupEvents(projectId, eventType, deploymentGroupId, events));
        return context.ExecuteWithinUsingWithComponent<RedeployTriggerDeploymentGroupEventSqlComponent, IList<RedeployTriggerDeploymentGroupEvent>>(action);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "top is optional and defaulted")]
    public virtual IList<RedeployTriggerDeploymentGroupEvent> GetRedeployTriggerDeploymentGroupEvents(
      IVssRequestContext context,
      Guid projectId,
      int top = 50)
    {
      using (ReleaseManagementTimer.Create(context, "Service", "RedeployTriggerDeploymentGroupEventService.GetRedeployTriggerDeploymentGroupEvent", 1976427))
      {
        Func<RedeployTriggerDeploymentGroupEventSqlComponent, IList<RedeployTriggerDeploymentGroupEvent>> action = (Func<RedeployTriggerDeploymentGroupEventSqlComponent, IList<RedeployTriggerDeploymentGroupEvent>>) (component => component.GetRedeployTriggerDeploymentGroupEvents(projectId, top));
        return context.ExecuteWithinUsingWithComponent<RedeployTriggerDeploymentGroupEventSqlComponent, IList<RedeployTriggerDeploymentGroupEvent>>(action);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual IList<RedeployTriggerDeploymentGroupEvent> UpdateRedeployTriggerDeploymentGroupEventReadyForDeploymentState(
      IVssRequestContext context,
      Guid projectId,
      int environmentId,
      bool readyForDeployment,
      DateTime lastModifiedOn)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (ReleaseManagementTimer.Create(context, "Service", "RedeployTriggerDeploymentGroupEventService.UpdateRedeployTriggerDeploymentGroupEventReadyForDeploymentState", 1976427))
      {
        Func<RedeployTriggerDeploymentGroupEventSqlComponent, IList<RedeployTriggerDeploymentGroupEvent>> action = (Func<RedeployTriggerDeploymentGroupEventSqlComponent, IList<RedeployTriggerDeploymentGroupEvent>>) (component => component.UpdateRedeployTriggerDeploymentGroupEventReadyForProcessing(projectId, environmentId, readyForDeployment, lastModifiedOn));
        return context.ExecuteWithinUsingWithComponent<RedeployTriggerDeploymentGroupEventSqlComponent, IList<RedeployTriggerDeploymentGroupEvent>>(action);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual void DeleteRedeployTriggerDeploymentGroupEvents(
      IVssRequestContext context,
      Guid projectId,
      IList<long> eventIds)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) eventIds, nameof (eventIds));
      using (ReleaseManagementTimer.Create(context, "Service", "RedeployTriggerDeploymentGroupEventService.DeleteRedeployTriggerDeploymentGroupEvent", 1976427))
      {
        Action<RedeployTriggerDeploymentGroupEventSqlComponent> action = (Action<RedeployTriggerDeploymentGroupEventSqlComponent>) (component => component.DeleteRedeployTriggerDeploymentGroupEvents(projectId, eventIds));
        context.ExecuteWithinUsingWithComponent<RedeployTriggerDeploymentGroupEventSqlComponent>(action);
      }
    }
  }
}
