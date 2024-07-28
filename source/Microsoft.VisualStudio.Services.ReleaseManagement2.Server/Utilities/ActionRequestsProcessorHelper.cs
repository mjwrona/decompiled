// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ActionRequestsProcessorHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class ActionRequestsProcessorHelper
  {
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required to catch all exception")]
    public static void AddRetainBuildActionRequest(
      IVssRequestContext context,
      Guid projectId,
      ActionRequestService actionRequestService,
      int buildId,
      bool retainBuildState,
      string errorMessage)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (actionRequestService == null)
        throw new ArgumentNullException(nameof (actionRequestService));
      RetainBuildActionType actionType = retainBuildState ? RetainBuildActionType.RetainBuild : RetainBuildActionType.StopRetainBuild;
      RetainBuildActionRequestData actionRequestData = new RetainBuildActionRequestData(projectId, buildId, actionType);
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) buildId);
      ActionRequest actionRequest = new ActionRequest()
      {
        LastException = errorMessage,
        ActionRequestType = ActionRequestType.RetainBuild,
        ActionRequestData = (ActionRequestData) actionRequestData,
        ResourceId = str
      };
      try
      {
        actionRequestService.AddActionRequest(context, projectId, actionRequest);
      }
      catch (Exception ex)
      {
        context.TraceCatch(1971048, TraceLevel.Error, "ReleaseManagementService", "Service", ex);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required to catch all exception")]
    public static void AddStartReleaseEnvironmentActionRequest(
      IVssRequestContext context,
      ActionRequestService actionRequestService,
      StartEnvironmentData startData)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (actionRequestService == null)
        throw new ArgumentNullException(nameof (actionRequestService));
      StartReleaseEnvironmentActionRequestData actionRequestData = startData != null ? new StartReleaseEnvironmentActionRequestData(startData) : throw new ArgumentNullException(nameof (startData));
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) startData.ProjectId, (object) startData.ReleaseId);
      ActionRequest actionRequest = new ActionRequest()
      {
        LastException = string.Empty,
        ActionRequestType = ActionRequestType.StartReleaseEnvironment,
        ActionRequestData = (ActionRequestData) actionRequestData,
        ResourceId = str
      };
      try
      {
        actionRequestService.AddActionRequest(context, startData.ProjectId, actionRequest);
      }
      catch (Exception ex)
      {
        context.TraceCatch(1960087, TraceLevel.Error, "ReleaseManagementService", "Pipeline", ex);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required to catch all exception")]
    [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "ArgumentUtility does this.")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters are simpler than overloaded method")]
    public static void AddManageBuildRetentionLeaseActionRequest(
      IVssRequestContext context,
      Guid projectId,
      ActionRequestService actionRequestService,
      ManageBuildRetentionLeaseActionType actionType,
      string errorMessage,
      int buildId,
      Guid releaseProjectId,
      int buildDefinitionId,
      string leaseOwnerId,
      int releaseId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<ActionRequestService>(actionRequestService, nameof (actionRequestService));
      ManageBuildRetentionLeaseActionRequestData actionRequestData = new ManageBuildRetentionLeaseActionRequestData(projectId, buildId, actionType, releaseProjectId, buildDefinitionId, leaseOwnerId, releaseId);
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) buildId);
      ActionRequest actionRequest = new ActionRequest()
      {
        LastException = errorMessage,
        ActionRequestType = ActionRequestType.ManageBuildRetentionLease,
        ActionRequestData = (ActionRequestData) actionRequestData,
        ResourceId = str
      };
      try
      {
        actionRequestService.AddActionRequest(context, projectId, actionRequest);
      }
      catch (Exception ex)
      {
        context.TraceCatch(1971048, TraceLevel.Error, "ReleaseManagementService", "Service", ex);
      }
    }
  }
}
