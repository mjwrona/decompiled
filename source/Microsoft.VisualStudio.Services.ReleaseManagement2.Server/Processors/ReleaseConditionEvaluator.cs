// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.ReleaseConditionEvaluator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.ServerEvents;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  internal class ReleaseConditionEvaluator
  {
    private readonly IVssRequestContext context;
    private readonly Guid projectId;
    private readonly IDataAccessLayer dataAccessLayer;

    public ReleaseConditionEvaluator(IVssRequestContext context, Guid projectId)
    {
      this.context = context;
      this.projectId = projectId;
      this.dataAccessLayer = (IDataAccessLayer) new DataAccessLayer(context, projectId);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public void StartRelease(Release release)
    {
      this.context.Trace(1980000, TraceLevel.Verbose, "ReleaseManagementService", "Pipeline", "ReleaseConditionEvaluator: start release {0}, evaluating conditions.", (object) release.Id);
      this.QueueReleaseOnValidEnvironment(release, true);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public void EvaluateConditionsOnEnvironmentCompletion(int releaseId, int completedEnvironmentId)
    {
      this.context.Trace(1980001, TraceLevel.Verbose, "ReleaseManagementService", "Pipeline", "ReleaseConditionEvaluator: environment completed. releaseId : {0}, environmentId {1}, evaluating conditions", (object) releaseId, (object) completedEnvironmentId);
      this.QueueReleaseOnValidEnvironment(this.dataAccessLayer.GetRelease(releaseId), false, completedEnvironmentId);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public void EvaluateConditionsOnEnvironmentCompletion(
      Release release,
      int completedEnvironmentId)
    {
      this.context.Trace(1980001, TraceLevel.Verbose, "ReleaseManagementService", "Pipeline", "ReleaseConditionEvaluator: environment completed. releaseId : {0}, environmentId {1}, evaluating conditions", (object) release.Id, (object) completedEnvironmentId);
      this.QueueReleaseOnValidEnvironment(release, false, completedEnvironmentId);
    }

    private static bool? EvaluatePullRequestSettings(
      Release release,
      ReleaseEnvironment environment)
    {
      if (release.Reason != ReleaseReason.PullRequest)
        return new bool?();
      return environment.EnvironmentOptions == null || !environment.EnvironmentOptions.PullRequestDeploymentEnabled ? new bool?(false) : new bool?(true);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private void QueueReleaseOnValidEnvironment(
      Release release,
      bool isReleaseStart,
      int completedEnvironmentId = 0)
    {
      if (release.Environments.All<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (env => env.Conditions == null)))
      {
        this.context.Trace(1980002, TraceLevel.Info, "ReleaseManagementService", "Pipeline", "ReleaseConditionEvaluator: no conditions available for release {0}. Using ranks to find next environment.", (object) release.Id);
        this.QueueReleaseOnNextEnvironmentBasedOnRank(release, isReleaseStart, completedEnvironmentId);
      }
      else
      {
        IEnumerable<ReleaseEnvironment> releaseEnvironments = release.Environments.Where<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (env => env.Status == ReleaseEnvironmentStatus.NotStarted));
        bool flag = false;
        foreach (ReleaseEnvironment releaseEnvironment in releaseEnvironments)
        {
          bool? pullRequestSettings = ReleaseConditionEvaluator.EvaluatePullRequestSettings(release, releaseEnvironment);
          if (pullRequestSettings.HasValue && !pullRequestSettings.Value)
          {
            this.context.Trace(1980006, TraceLevel.Verbose, "ReleaseManagementService", "Pipeline", "ReleaseConditionEvaluator: Pull request deployment is not enabled for environment {0}", (object) releaseEnvironment.Id);
          }
          else
          {
            Guid deploymentRequestedFor;
            if (releaseEnvironment.EvaluateConditions(this.context, release, isReleaseStart, completedEnvironmentId, out deploymentRequestedFor))
            {
              this.context.Trace(1980003, TraceLevel.Verbose, "ReleaseManagementService", "Pipeline", "ReleaseConditionEvaluator: Conditions satisfied for environment {0}, release {1}", (object) releaseEnvironment.Id, (object) release.Id);
              this.QueueStartEnvironmentJob(release, releaseEnvironment, this.context.GetUserId(true), deploymentRequestedFor, isReleaseStart);
              flag = true;
            }
          }
        }
        if (releaseEnvironments.Any<ReleaseEnvironment>())
          this.dataAccessLayer.UpdateReleaseEnvironmentConditions(release.Id, releaseEnvironments);
        if (flag)
          return;
        this.SendReleaseHaltedNotification(release);
      }
    }

    private void QueueReleaseOnNextEnvironmentBasedOnRank(
      Release release,
      bool isReleaseStart,
      int completedEnvironmentId)
    {
      if (isReleaseStart)
      {
        ReleaseEnvironment releaseEnvironment = release.Environments.Single<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (env => env.Rank == 1));
        this.QueueStartEnvironmentJob(release, releaseEnvironment, this.context.GetUserId(true), release.CreatedBy, isReleaseStart);
      }
      else
      {
        ReleaseEnvironment lastEnvironment = completedEnvironmentId != 0 ? release.Environments.Single<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (env => env.Id == completedEnvironmentId)) : release.GetLastModifiedEnvironment();
        ReleaseEnvironment releaseEnvironment1 = release.Environments.SingleOrDefault<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (env => env.DefinitionEnvironmentId == release.TargetEnvironmentId));
        ReleaseEnvironment releaseEnvironment2 = release.Environments.SingleOrDefault<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (env => env.Rank == lastEnvironment.Rank + 1));
        if (releaseEnvironment2 != null && releaseEnvironment1 != null && releaseEnvironment2.Rank <= releaseEnvironment1.Rank)
        {
          Guid requestedFor = lastEnvironment.GetLatestDeploymentRequestedFor();
          if (requestedFor == Guid.Empty)
            requestedFor = release.CreatedBy;
          this.QueueStartEnvironmentJob(release, releaseEnvironment2, this.context.GetUserId(true), requestedFor, isReleaseStart);
        }
        else
          this.SendReleaseHaltedNotification(release);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private void QueueStartEnvironmentJob(
      Release release,
      ReleaseEnvironment releaseEnvironment,
      Guid requestedBy,
      Guid requestedFor,
      bool isReleaseStart)
    {
      this.context.Trace(1980004, TraceLevel.Verbose, "ReleaseManagementService", "Pipeline", "ReleaseConditionEvaluator: Queueing start environment job for environment {0}, release {1}", (object) releaseEnvironment.Id, (object) release.Id);
      DefinitionEnvironment definitionEnvironment = this.dataAccessLayer.GetDefinitionEnvironment(release.ReleaseDefinitionId, releaseEnvironment.DefinitionEnvironmentId);
      if (definitionEnvironment != null && definitionEnvironment.Schedules.Any<ReleaseSchedule>())
      {
        int deploymentAttemptId = releaseEnvironment.GetLastDeploymentAttemptId();
        this.dataAccessLayer.UpdateReleaseEnvironmentStatus(release, releaseEnvironment.Id, ReleaseEnvironmentStatus.Undefined, ReleaseEnvironmentStatus.Scheduled, (ReleaseEnvironmentStatusChangeDetails) null, (string) null, deploymentAttemptId);
      }
      else
      {
        bool flag = isReleaseStart && release.Reason == ReleaseReason.Schedule;
        new OrchestratorServiceProcessorV2(this.context, this.projectId).QueueOnStartEnvironmentJob(release.Id, release.ReleaseDefinitionId, releaseEnvironment.DefinitionEnvironmentId, releaseEnvironment.Id, requestedBy, requestedFor, DeploymentReason.Automated, !flag);
      }
    }

    private void SendReleaseHaltedNotification(Release release)
    {
      ReleaseUpdatedServerEvent updatedServerEvent = new ReleaseUpdatedServerEvent()
      {
        ProjectId = this.projectId,
        Release = release
      };
      this.context.GetService<ITeamFoundationSqlNotificationService>().SendNotification(this.context, ReleaseUpdatedServerEvent.EventClass, JsonConvert.SerializeObject((object) updatedServerEvent));
    }
  }
}
