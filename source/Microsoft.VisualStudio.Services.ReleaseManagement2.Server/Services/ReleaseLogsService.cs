// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseLogsService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ReleaseLogsService : ReleaseManagement2ServiceBase
  {
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public void DownloadLog(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      int releaseDeployPhaseId,
      int logId,
      int attemptId,
      long? startLine,
      long? endLine,
      StreamWriter streamWriter)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      requestContext.TraceEnter(1900046, "ReleaseManagementService", "Service", "ReleaseLogsService::DownloadLog");
      IList<ReleaseDeployPhase> releaseEnvironment = requestContext.GetService<ReleasesService>().GetDeployPhasesForReleaseEnvironment(requestContext, projectId, releaseId, environmentId, releaseDeployPhaseId, attemptId);
      Guid? nullable = releaseEnvironment != null && releaseEnvironment.Any<ReleaseDeployPhase>() ? releaseEnvironment.First<ReleaseDeployPhase>().RunPlanId : throw new ReleaseManagementObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.NoRunPlanExists, (object) releaseId, (object) environmentId, (object) attemptId, (object) releaseDeployPhaseId));
      if (nullable.HasValue)
      {
        Guid planId;
        DeployPhaseTypes phaseType;
        if (releaseDeployPhaseId == 0)
        {
          nullable = releaseEnvironment.First<ReleaseDeployPhase>().RunPlanId;
          planId = nullable.Value;
          phaseType = releaseEnvironment.First<ReleaseDeployPhase>().PhaseType;
        }
        else
        {
          int count = releaseEnvironment.Count;
          nullable = releaseEnvironment.First<ReleaseDeployPhase>().RunPlanId;
          planId = nullable.Value;
          phaseType = releaseEnvironment.First<ReleaseDeployPhase>().PhaseType;
        }
        requestContext.TraceLeave(1900046, "ReleaseManagementService", "Service", "ReleaseLogsService::DownloadLog");
        new ReleaseLogsProcessor().DownloadLog(requestContext, projectId, planId, phaseType, logId, startLine, endLine, streamWriter);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public void DownloadGreenlightingLog(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      int gateId,
      int logId,
      StreamWriter streamWriter)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      requestContext.TraceEnter(1900046, "ReleaseManagementService", "Service", "ReleaseLogsService::DownloadGreenlightingLog");
      Func<DeploymentSqlComponent, DeploymentGate> action = (Func<DeploymentSqlComponent, DeploymentGate>) (component => component.GetDeploymentGate(projectId, releaseId, environmentId, gateId));
      DeploymentGate deploymentGate = requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, DeploymentGate>(action);
      if ((deploymentGate != null ? (!deploymentGate.RunPlanId.HasValue ? 1 : 0) : 1) != 0)
        throw new ReleaseManagementObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.NoGreenlightingPlan, (object) releaseId, (object) environmentId, (object) gateId));
      requestContext.TraceLeave(1900046, "ReleaseManagementService", "Service", "ReleaseLogsService::DownloadGreenlightingLog");
      ReleaseLogsProcessor.DownloadLog(requestContext, deploymentGate.RunPlanId.Value, logId, streamWriter, (IDistributedTaskOrchestrator) new GreenlightingOrchestrator(requestContext, projectId));
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public void DownloadLogs(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      ZipArchive zipArchive)
    {
      requestContext.TraceEnter(1900046, "ReleaseManagementService", "Service", "ReleaseLogsService::DownloadLogs");
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      Func<ReleaseSqlComponent, ReleaseLogContainers> action = (Func<ReleaseSqlComponent, ReleaseLogContainers>) (component => component.GetReleaseLogContainers(projectId, releaseId, false));
      ReleaseLogContainers logContainers = requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, ReleaseLogContainers>(action);
      requestContext.TraceLeave(1900046, "ReleaseManagementService", "Service", "ReleaseLogsService::DownloadLogs");
      new ReleaseLogsProcessor().DownloadLogs(requestContext, projectId, logContainers, zipArchive);
    }

    public void DownloadLog(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      int attemptId,
      Guid timelineId,
      int logId,
      long? startLine,
      long? endLine,
      StreamWriter streamWriter)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      requestContext.TraceEnter(1900046, "ReleaseManagementService", "Service", "ReleaseLogsService::DownloadLog");
      ReleaseLogContainers containersForRelease = this.GetLogContainersForRelease(requestContext, projectId, releaseId, environmentId);
      requestContext.TraceLeave(1900046, "ReleaseManagementService", "Service", "ReleaseLogsService::DownloadLog::GetLogContainersForReleaseFinish");
      if (containersForRelease.DeployPhases.Any<ReleaseDeployPhaseRef>())
      {
        ReleaseDeployPhaseRef releaseDeployPhaseRef = containersForRelease.DeployPhases.FirstOrDefault<ReleaseDeployPhaseRef>((Func<ReleaseDeployPhaseRef, bool>) (phase => phase.PlanId == timelineId));
        int num;
        if (releaseDeployPhaseRef == null)
        {
          num = 1;
        }
        else
        {
          Guid planId = releaseDeployPhaseRef.PlanId;
          num = 0;
        }
        if (num != 0)
          throw new ReleaseManagementObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.NoTimeLineExists, (object) releaseId, (object) environmentId, (object) attemptId, (object) timelineId));
        new ReleaseLogsProcessor().DownloadLog(requestContext, projectId, releaseDeployPhaseRef.PlanId, releaseDeployPhaseRef.PhaseType, logId, startLine, endLine, streamWriter);
      }
      else if (containersForRelease.DeploySteps.Any<ReleaseEnvironmentStep>())
      {
        ReleaseEnvironmentStep releaseEnvironmentStep = containersForRelease.DeploySteps.FirstOrDefault<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.ReleaseEnvironmentId == environmentId && step.TrialNumber == attemptId));
        if ((releaseEnvironmentStep != null ? (!releaseEnvironmentStep.RunPlanId.HasValue ? 1 : 0) : 1) != 0)
          throw new ReleaseManagementObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.NoTimeLineExists, (object) releaseId, (object) environmentId, (object) attemptId, (object) timelineId));
        Guid valueOrDefault = releaseEnvironmentStep.RunPlanId.GetValueOrDefault();
        ReleaseLogsProcessor.DownloadLog(requestContext, valueOrDefault, logId, startLine, endLine, streamWriter, (IDistributedTaskOrchestrator) new PipelineOrchestrator(requestContext, projectId));
      }
      else
        throw new ReleaseManagementObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.NoTimeLineExists, (object) releaseId, (object) environmentId, (object) attemptId, (object) timelineId));
      requestContext.TraceLeave(1900046, "ReleaseManagementService", "Service", "ReleaseLogsService::DownloadLog");
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public ReleaseLogContainers GetLogContainersForRelease(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId)
    {
      Func<ReleaseSqlComponent, ReleaseLogContainers> action = (Func<ReleaseSqlComponent, ReleaseLogContainers>) (component => component.GetReleaseLogContainers(projectId, releaseId, releaseEnvironmentId, false));
      return requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, ReleaseLogContainers>(action);
    }
  }
}
