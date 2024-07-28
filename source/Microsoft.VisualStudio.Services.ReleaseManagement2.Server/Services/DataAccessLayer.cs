// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.DataAccessLayer
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is the interface between business logic and data tier")]
  public class DataAccessLayer : IDataAccessLayer
  {
    private readonly IVssRequestContext requestContext;
    private readonly Guid projectId;

    public DataAccessLayer(IVssRequestContext requestContext, Guid projectId)
    {
      this.requestContext = requestContext;
      this.projectId = projectId;
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release AddRelease(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      using (ReleaseManagementTimer.Create(this.requestContext, nameof (DataAccessLayer), "DataAccessLayer.AddRelease", 1971015))
        return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>((Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) (component => component.AddRelease(this.projectId, release, comment)));
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release UpdateDraftRelease(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      string comment)
    {
      using (ReleaseManagementTimer.Create(this.requestContext, nameof (DataAccessLayer), "DataAccessLayer.UpdateDraftRelease", 1971034))
        return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>((Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) (component => component.UpdateDraftRelease(this.projectId, release, comment)));
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release GetRelease(int id)
    {
      using (ReleaseManagementTimer.Create(this.requestContext, nameof (DataAccessLayer), "DataAccessLayer.GetRelease", 1971005))
        return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>((Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) (component => component.GetRelease(this.projectId, id)));
    }

    public ReleaseEnvironmentStep GetReleaseStep(int id) => this.requestContext.ExecuteWithinUsingWithComponent<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>((Func<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>) (component => component.GetReleaseEnvironmentStep(this.projectId, id, false))).SingleOrDefault<ReleaseEnvironmentStep>();

    public IEnumerable<ReleaseEnvironmentStep> AddReleaseSteps(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps,
      bool handleParallelApprovals)
    {
      if (releaseEnvironmentSteps == null)
        throw new ArgumentNullException(nameof (releaseEnvironmentSteps));
      Guid requestedBy = this.requestContext.GetUserId(true);
      IEnumerable<ReleaseEnvironmentStep> source = this.requestContext.ExecuteWithinUsingWithComponent<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>((Func<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>) (component => component.AddReleaseEnvironmentSteps(this.projectId, releaseEnvironmentSteps, handleParallelApprovals, requestedBy)));
      if (source.Count<ReleaseEnvironmentStep>() > 0)
      {
        ReleaseEnvironmentStep firstStep = source.First<ReleaseEnvironmentStep>();
        if (release != null)
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = release.Environments.Single<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (e => e.Id == firstStep.ReleaseEnvironmentId));
          foreach (ReleaseEnvironmentStep step in source)
            releaseEnvironment.AddReleaseEnvironmentStep(step);
          if (firstStep.StepType != EnvironmentStepType.Deploy && !firstStep.IsAutomated)
            this.SendReleaseEnvironmentUpdatedEvent(release.ReleaseDefinitionId, release.Id, releaseEnvironment.Id);
        }
      }
      return source;
    }

    public ReleaseEnvironmentStep UpdateReleaseStep(ReleaseEnvironmentStep releaseEnvironmentStep)
    {
      if (releaseEnvironmentStep == null)
        throw new ArgumentNullException(nameof (releaseEnvironmentStep));
      return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseEnvironmentStepSqlComponent, ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStepSqlComponent, ReleaseEnvironmentStep>) (component => component.UpdateReleaseEnvironmentStep(this.projectId, releaseEnvironmentStep)));
    }

    public IEnumerable<ReleaseEnvironmentStep> UpdateReleaseEnvironmentSteps(
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps)
    {
      if (releaseEnvironmentSteps == null)
        throw new ArgumentNullException(nameof (releaseEnvironmentSteps));
      if (!releaseEnvironmentSteps.Any<ReleaseEnvironmentStep>() || releaseEnvironmentSteps.Any<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s == null)))
        throw new InvalidDataException();
      IEnumerable<ReleaseEnvironmentStep> distinctSteps = releaseEnvironmentSteps.GroupBy<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (s => s.Id)).Select<IGrouping<int, ReleaseEnvironmentStep>, ReleaseEnvironmentStep>((Func<IGrouping<int, ReleaseEnvironmentStep>, ReleaseEnvironmentStep>) (g => g.First<ReleaseEnvironmentStep>()));
      IEnumerable<ReleaseEnvironmentStep> list = (IEnumerable<ReleaseEnvironmentStep>) this.requestContext.ExecuteWithinUsingWithComponent<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>((Func<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>) (component => component.UpdateReleaseEnvironmentSteps(this.projectId, distinctSteps))).ToList<ReleaseEnvironmentStep>();
      if (list.Any<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.Status == ReleaseEnvironmentStepStatus.Reassigned)))
      {
        ReleaseEnvironmentStep releaseEnvironmentStep = list.First<ReleaseEnvironmentStep>();
        this.SendReleaseEnvironmentUpdatedEvent(releaseEnvironmentStep.ReleaseDefinitionId, releaseEnvironmentStep.ReleaseId, releaseEnvironmentStep.ReleaseEnvironmentId);
      }
      return list;
    }

    public ReleaseEnvironmentStep HandleEnvironmentDeployJobStarted(
      int releaseId,
      int releaseStepId,
      int releaseDeployPhaseId)
    {
      ReleaseEnvironmentStep releaseEnvironmentStep = this.requestContext.ExecuteWithinUsingWithComponent<ReleaseEnvironmentStepSqlComponent, ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStepSqlComponent, ReleaseEnvironmentStep>) (component => component.HandleEnvironmentDeployJobStarted(this.projectId, releaseId, releaseStepId, releaseDeployPhaseId)));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = this.GetRelease(releaseId);
      ReleaseEnvironmentStep step = release.GetStep(releaseStepId);
      int releaseEnvironmentId = step.ReleaseEnvironmentId;
      this.SendDeployJobStartedEvent(release.ReleaseDefinitionId, releaseId, step.DefinitionEnvironmentId, releaseEnvironmentId);
      this.PublishCommitStatusForRelease(release, releaseEnvironmentId);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment = release.GetEnvironment(releaseEnvironmentId);
      this.SendEnvironmentStatusUpdatedEvent(release.Id, release.ReleaseDefinitionId, environment);
      return releaseEnvironmentStep;
    }

    public void PublishCommitStatusForRelease(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release, int releaseEnvironmentId) => PublishCommitStatusHelper.PublishEnvironmentStatus(this.requestContext, this.projectId, release, releaseEnvironmentId);

    public void HandlePipelineAssigned(int releaseId, int releaseStepId)
    {
      Action<ReleaseEnvironmentStepSqlComponent> action = (Action<ReleaseEnvironmentStepSqlComponent>) (component => component.HandleEnvironmentPipelineStatusUpdate(this.projectId, releaseId, releaseStepId, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.QueuedForAgent));
      if (this.requestContext != null)
        this.requestContext.ExecuteWithinUsingWithComponent<ReleaseEnvironmentStepSqlComponent>(action);
      ReleaseEnvironmentStep releaseStep = this.GetReleaseStep(releaseStepId);
      if (releaseStep == null)
        throw new InvalidDataException();
      ReleaseEnvironmentData releaseEnvironmentData = this.GetReleaseEnvironmentData(releaseStep.ReleaseId, releaseStep.ReleaseEnvironmentId, false, false, false);
      this.SendEnvironmentStatusUpdatedEvent(releaseStep.ReleaseId, releaseStep.ReleaseDefinitionId, releaseEnvironmentData.Environment);
    }

    public void HandleQueuedForPipeline(int releaseId, int releaseStepId)
    {
      this.requestContext.ExecuteWithinUsingWithComponent<ReleaseEnvironmentStepSqlComponent>((Action<ReleaseEnvironmentStepSqlComponent>) (component => component.HandleEnvironmentPipelineStatusUpdate(this.projectId, releaseId, releaseStepId, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.QueuedForPipeline)));
      ReleaseEnvironmentStep releaseStep = this.GetReleaseStep(releaseStepId);
      ReleaseEnvironmentData releaseEnvironmentData = this.GetReleaseEnvironmentData(releaseStep.ReleaseId, releaseStep.ReleaseEnvironmentId, false, false, false);
      this.SendEnvironmentStatusUpdatedEvent(releaseStep.ReleaseId, releaseStep.ReleaseDefinitionId, releaseEnvironmentData.Environment);
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release UpdateReleaseStatus(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus status,
      string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      Guid requestedBy = this.requestContext.GetUserId(true);
      return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>((Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) (component => component.PatchRelease(this.projectId, release.Id, requestedBy, (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>(), comment, new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus?(status))));
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment QueueReleaseOnEnvironment(
      int releaseDefinitionId,
      int releaseId,
      int definitionEnvironmentId,
      int releaseEnvironmentId,
      Guid requestedBy,
      Guid requestedFor,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentReason reason,
      string comment)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment = this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>((Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>) (component => component.CreateAndQueueDeployment(this.projectId, releaseDefinitionId, releaseId, definitionEnvironmentId, releaseEnvironmentId, requestedBy, requestedFor, reason, comment)));
      if (deployment == null)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.CouldNotCreateDeploymentAsOneAlreadyExists, (object) releaseEnvironmentId));
      this.requestContext.SendReleaseEnvironmentStatusUpdatedNotification(this.projectId, releaseDefinitionId, releaseId, releaseEnvironmentId, EnvironmentStatus.Queued, deployment.Status.ToWebApi(), deployment.OperationStatus.ToWebApi());
      this.SendReleaseEnvironmentUpdatedEvent(deployment.ReleaseDefinitionId, deployment.ReleaseId, releaseEnvironmentId);
      return deployment;
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release UpdateReleaseEnvironmentConditions(
      int releaseId,
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment> releaseEnvironments)
    {
      return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>((Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) (component => component.UpdateReleaseEnvironmentConditions(this.projectId, releaseId, releaseEnvironments)));
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release UpdateReleaseEnvironmentStatus(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int releaseEnvironmentId,
      ReleaseEnvironmentStatus statusFrom,
      ReleaseEnvironmentStatus statusTo,
      ReleaseEnvironmentStatusChangeDetails changeDetails,
      string comment,
      int attempt)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1 = this.UpdateEnvironmentStatus(release.Id, releaseEnvironmentId, statusFrom, statusTo, changeDetails, comment, attempt);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment1 = release1.Environments.Where<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (environment => environment.Id == releaseEnvironmentId)).FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>();
      if (statusTo != ReleaseEnvironmentStatus.Succeeded && statusTo != ReleaseEnvironmentStatus.PartiallySucceeded)
      {
        this.SendEnvironmentStatusUpdatedEvent(release1.Id, release1.ReleaseDefinitionId, environment1);
        this.SendReleaseEnvironmentUpdatedEvent(release1.ReleaseDefinitionId, release1.Id, releaseEnvironmentId);
      }
      return release1;
    }

    private Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release UpdateEnvironmentStatus(
      int releaseId,
      int releaseEnvironmentId,
      ReleaseEnvironmentStatus statusFrom,
      ReleaseEnvironmentStatus statusTo,
      ReleaseEnvironmentStatusChangeDetails changeDetails,
      string comment,
      int attempt)
    {
      Guid requestedBy = this.requestContext.GetUserId(true);
      return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>((Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) (component => component.UpdateReleaseEnvironmentStatus(this.projectId, releaseId, releaseEnvironmentId, statusFrom, statusTo, requestedBy, changeDetails, comment, attempt)));
    }

    public IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> RejectReleaseEnvironments(
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps)
    {
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> releases = this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>>((Func<ReleaseSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>>) (component => component.RejectMultipleReleaseEnvironments(this.projectId, releaseEnvironmentSteps)));
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1 in releases)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = release1;
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>) release.Environments)
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = environment;
          if (releaseEnvironmentSteps.ToList<ReleaseEnvironmentStep>().Any<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (x => x.ReleaseEnvironmentId == releaseEnvironment.Id && x.ReleaseDefinitionId == release.ReleaseDefinitionId && x.ReleaseId == release.Id)))
          {
            this.SendEnvironmentStatusUpdatedEvent(release.Id, release.ReleaseDefinitionId, releaseEnvironment);
            this.SendReleaseEnvironmentUpdatedEvent(release.ReleaseDefinitionId, release.Id, releaseEnvironment.Id);
          }
        }
        this.SendReleaseUpdatedNotification(release.ReleaseDefinitionId, release.Id);
      }
      return releases;
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release CancelDeploymentOnEnvironment(
      int releaseId,
      int releaseEnvironmentId,
      string comment,
      bool addCommentAsDeploymentIssue,
      bool evaluateForCanceling)
    {
      Guid requestedBy = this.requestContext.GetUserId(true);
      return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>((Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) (component => component.CancelDeploymentOnEnvironment(this.projectId, releaseId, releaseEnvironmentId, requestedBy, comment, addCommentAsDeploymentIssue, evaluateForCanceling)));
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release UpdateReleaseEnvironment(
      int releaseId,
      int releaseEnvironmentId,
      DateTime? scheduledDateTime,
      Guid? stageSchedulingJobId = null)
    {
      Guid requestedBy = this.requestContext.GetUserId(true);
      return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>((Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) (component => component.UpdateReleaseEnvironment(this.projectId, releaseId, releaseEnvironmentId, scheduledDateTime, stageSchedulingJobId, requestedBy)));
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release StartDraftRelease(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      Guid requestedBy = this.requestContext.GetUserId(true);
      release.Status = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Active;
      using (ReleaseManagementTimer.Create(this.requestContext, nameof (DataAccessLayer), "DataAccessLayer.StartDraftRelease", 1971033))
        return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>((Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) (component => component.StartDraftRelease(this.projectId, release, requestedBy, comment)));
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition GetReleaseDefinition(
      int id)
    {
      bool isDefaultToLatestArtifactVersionEnabled = this.requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.DefaultToLatestArtifactVersion");
      using (ReleaseManagementTimer.Create(this.requestContext, nameof (DataAccessLayer), "DataAccessLayer.GetReleaseDefinition", 1961203))
        return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition>((Func<ReleaseDefinitionSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition>) (component => component.GetReleaseDefinition(this.projectId, id, isDefaultToLatestArtifactVersionEnabled: isDefaultToLatestArtifactVersionEnabled)));
    }

    public IEnumerable<ReleaseEnvironmentStep> GetReleaseSteps(
      int releaseId,
      int releaseEnvironmentId,
      int stepRank,
      int trialNumber)
    {
      return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>((Func<ReleaseEnvironmentStepSqlComponent, IEnumerable<ReleaseEnvironmentStep>>) (component => component.GetReleaseEnvironmentApprovalSteps(this.projectId, releaseId, releaseEnvironmentId, stepRank, trialNumber)));
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release UpdateApprovalStepsStatus(
      int releaseId,
      int releaseEnvironmentId,
      EnvironmentStepType stepType,
      ReleaseEnvironmentStepStatus statusFrom,
      ReleaseEnvironmentStepStatus statusTo,
      string comment)
    {
      return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>((Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) (component => component.UpdateApprovalStepsStatus(this.projectId, releaseId, releaseEnvironmentId, stepType, statusFrom, statusTo, comment)));
    }

    public void SetReleaseScheduledPromotion(
      int releaseId,
      int definitionEnvironmentId,
      int releaseEnvironmentStepId,
      DateTime? deferredDateTime)
    {
      throw new NotImplementedException();
    }

    public void DeleteReleaseScheduledPromotion(int releaseId) => throw new NotImplementedException();

    public DefinitionEnvironment GetDefinitionEnvironment(
      int releaseDefinitionId,
      int definitionEnvironmentId)
    {
      return this.requestContext.ExecuteWithinUsingWithComponent<DefinitionEnvironmentSqlComponent, DefinitionEnvironment>((Func<DefinitionEnvironmentSqlComponent, DefinitionEnvironment>) (component => component.GetDefinitionEnvironment(this.projectId, releaseDefinitionId, definitionEnvironmentId)));
    }

    public IEnumerable<ReleaseEnvironmentQueueData> GetUnhealthyReleaseEnvironments(
      int releaseDefinitionId,
      int definitionEnvironmentId,
      int daysToCheck)
    {
      return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseEnvironmentQueueSqlComponent, IEnumerable<ReleaseEnvironmentQueueData>>((Func<ReleaseEnvironmentQueueSqlComponent, IEnumerable<ReleaseEnvironmentQueueData>>) (component => component.GetUnhealthyReleaseEnvironments(this.projectId, releaseDefinitionId, definitionEnvironmentId, daysToCheck)));
    }

    public QueuingPolicyResult GetReleaseEnvironmentsTorunAfterEnforcingQueuingPolicy(
      int releaseDefinitionId,
      int definitionEnvironmentId,
      int releaseId,
      int releaseEnvironmentId,
      int maxConcurrent,
      int maxQueueDepth)
    {
      Guid requestedBy = this.requestContext.GetUserId(true);
      return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseEnvironmentQueueSqlComponent, QueuingPolicyResult>((Func<ReleaseEnvironmentQueueSqlComponent, QueuingPolicyResult>) (component => component.GetReleaseEnvironmentsTorunAfterEnforcingQueuingPolicy(this.projectId, releaseDefinitionId, definitionEnvironmentId, releaseId, releaseEnvironmentId, maxConcurrent, maxQueueDepth, requestedBy)));
    }

    public void RemoveEnvironmentFromQueue(int releaseId, int releaseEnvironmentId) => this.requestContext.ExecuteWithinUsingWithComponent<ReleaseEnvironmentQueueSqlComponent>((Action<ReleaseEnvironmentQueueSqlComponent>) (component => component.RemoveEnvironmentFromQueue(this.projectId, releaseId, releaseEnvironmentId)));

    public ReleaseDeployPhase AddDeployPhase(
      int releaseId,
      int releaseEnvironmentId,
      int attempt,
      int rank,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes phaseType)
    {
      Guid requestedBy = this.requestContext.GetUserId(true);
      return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, ReleaseDeployPhase>((Func<ReleaseSqlComponent, ReleaseDeployPhase>) (component => component.AddReleaseDeployPhase(this.projectId, releaseId, releaseEnvironmentId, rank, attempt, phaseType, requestedBy)));
    }

    public ReleaseDeployPhase UpdateDeployPhase(
      ReleaseDeployPhase releaseDeployPhase,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus operationStatus,
      int definitionId)
    {
      Guid requestedBy = this.requestContext.GetUserId(true);
      ReleaseDeployPhase releaseDeployPhase1 = this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, ReleaseDeployPhase>((Func<ReleaseSqlComponent, ReleaseDeployPhase>) (component => component.UpdateReleaseDeployPhase(this.projectId, releaseDeployPhase, operationStatus, requestedBy)));
      if (releaseDeployPhase1.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseStatus.NotStarted)
        this.SendReleaseEnvironmentUpdatedEvent(definitionId, releaseDeployPhase.ReleaseId, releaseDeployPhase1.ReleaseEnvironmentId);
      ReleaseEnvironmentData releaseEnvironmentData = this.GetReleaseEnvironmentData(releaseDeployPhase.ReleaseId, releaseDeployPhase.ReleaseEnvironmentId, false, false, false);
      this.SendEnvironmentStatusUpdatedEvent(releaseDeployPhase.ReleaseId, definitionId, releaseEnvironmentData.Environment);
      return releaseDeployPhase1;
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release UpdateEnvironmentAndDeploymentStatus(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int releaseEnvironmentId,
      int attempt,
      ReleaseEnvironmentStatus completionStatus,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus deploymentStatus,
      ReleaseEnvironmentStatusChangeDetails changeDetails,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus operationStatus = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Undefined)
    {
      Guid changedBy = this.requestContext.GetUserId(true);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1 = this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>((Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) (component => component.UpdateEnvironmentAndDeploymentStatus(this.projectId, release.Id, releaseEnvironmentId, attempt, changedBy, completionStatus, deploymentStatus, changeDetails, operationStatus)));
      this.requestContext.SendReleaseEnvironmentStatusUpdatedNotification(this.projectId, release.ReleaseDefinitionId, release.Id, releaseEnvironmentId, completionStatus.ToWebApi(), deploymentStatus.ToWebApi(), Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Undefined);
      this.SendReleaseEnvironmentUpdatedEvent(release1.ReleaseDefinitionId, release1.Id, releaseEnvironmentId);
      return release1;
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment UpdateDeploymentOperationStatus(
      int releaseId,
      int releaseEnvironmentId,
      int attempt,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus operationStatus)
    {
      Guid changedBy = this.requestContext.GetUserId(true);
      return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>((Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>) (component => component.UpdateDeploymentOperationStatus(this.projectId, releaseId, releaseEnvironmentId, attempt, operationStatus, changedBy)));
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention AddManualIntervention(
      int releaseId,
      int releaseEnvironmentId,
      int releaseDeployPhaseId,
      TaskActivityData taskActivityData,
      string instructions)
    {
      Guid changedBy = this.requestContext.GetUserId(true);
      return this.requestContext.ExecuteWithinUsingWithComponent<ManualInterventionSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention>((Func<ManualInterventionSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention>) (component => component.CreateManualIntervention(this.projectId, releaseId, releaseEnvironmentId, releaseDeployPhaseId, taskActivityData, instructions, changedBy)));
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention GetManualIntervention(
      int releaseId,
      int manualInterventionId)
    {
      return this.requestContext.ExecuteWithinUsingWithComponent<ManualInterventionSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention>((Func<ManualInterventionSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention>) (component => component.GetManualIntervention(this.projectId, releaseId, manualInterventionId)));
    }

    public IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention> GetManualInterventionsForRelease(
      int releaseId)
    {
      return this.requestContext.ExecuteWithinUsingWithComponent<ManualInterventionSqlComponent, IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention>>((Func<ManualInterventionSqlComponent, IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention>>) (component => component.GetManualInterventionsForRelease(this.projectId, releaseId)));
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention UpdateManualIntervention(
      int releaseId,
      int manualInterventionId,
      Guid approvedBy,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ManualInterventionStatus status,
      string comment)
    {
      return this.requestContext.ExecuteWithinUsingWithComponent<ManualInterventionSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention>((Func<ManualInterventionSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention>) (component => component.UpdateManualIntervention(this.projectId, releaseId, manualInterventionId, approvedBy, status, comment)));
    }

    public IEnumerable<PipelineArtifactSource> GetReleaseArtifactSources(
      IEnumerable<int> releases,
      string artifactTypeId)
    {
      return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, IEnumerable<PipelineArtifactSource>>((Func<ReleaseSqlComponent, IEnumerable<PipelineArtifactSource>>) (component => component.GetReleaseArtifactSources(this.projectId, releases, artifactTypeId)));
    }

    public ReleaseEnvironmentData GetReleaseEnvironmentData(
      int releaseId,
      int releaseEnvironmentId,
      bool includeDeployments,
      bool includeApprovals,
      bool includeArtifacts)
    {
      return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, ReleaseEnvironmentData>((Func<ReleaseSqlComponent, ReleaseEnvironmentData>) (component => component.GetReleaseEnvironmentData(this.projectId, releaseId, releaseEnvironmentId, includeDeployments, includeApprovals, includeArtifacts)));
    }

    public void SendReleaseUpdatedNotification(int definitionId, int releaseId) => this.requestContext.SendReleaseUpdatedEvent(this.projectId, definitionId, releaseId);

    public void SendReleaseEnvironmentUpdatedEvent(
      int definitionId,
      int releaseId,
      int releaseEnvironmentId)
    {
      this.requestContext.SendReleaseEnvironmentUpdatedEvent(this.projectId, definitionId, releaseId, releaseEnvironmentId);
    }

    public void SendDeployJobStartedEvent(
      int definitionId,
      int releaseId,
      int definitionEnvironmentId,
      int releaseEnvironmentId)
    {
      this.requestContext.SendDeployJobStartedEvent(this.projectId, definitionId, releaseId, definitionEnvironmentId, releaseEnvironmentId);
    }

    public void SendEnvironmentStatusUpdatedEvent(
      int releaseId,
      int releaseDefinitionId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment)
    {
      if (environment == null)
        return;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment latestDeployment = environment.GetLatestDeployment();
      if (latestDeployment == null)
        return;
      this.requestContext.SendReleaseEnvironmentStatusUpdatedNotification(this.projectId, releaseDefinitionId, releaseId, environment.Id, environment.Status.ToWebApi(), latestDeployment.Status.ToWebApi(), latestDeployment.OperationStatus.ToWebApi());
    }

    public void SendReleaseApprovalPendingEvent(
      int definitionId,
      int releaseId,
      int releaseEnvironmentId,
      int approvalId,
      Guid approverId)
    {
      this.requestContext.SendReleaseApprovalPendingEvent(this.projectId, definitionId, releaseId, releaseEnvironmentId, approvalId, approverId);
    }

    public void SaveProperties(ArtifactSpec artifactSpec, IEnumerable<PropertyValue> propertyValue)
    {
      if (artifactSpec == null || propertyValue == null || !propertyValue.Any<PropertyValue>())
        return;
      this.requestContext.GetService<TeamFoundationPropertyService>().SetProperties(this.requestContext, artifactSpec, propertyValue);
    }

    public void SaveProperties(
      IEnumerable<ArtifactPropertyValue> artifactPropertyValue)
    {
      if (artifactPropertyValue == null || !artifactPropertyValue.Any<ArtifactPropertyValue>())
        return;
      this.requestContext.GetService<TeamFoundationPropertyService>().SetProperties(this.requestContext, artifactPropertyValue);
    }

    public DeploymentGate AddDeploymentGate(
      int releaseId,
      int releaseEnvironmentId,
      int deploymentId,
      int stepId,
      EnvironmentStepType stepType)
    {
      return this.requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, DeploymentGate>((Func<DeploymentSqlComponent, DeploymentGate>) (component => component.AddDeploymentGate(this.projectId, releaseId, releaseEnvironmentId, deploymentId, stepId, stepType)));
    }

    public DeploymentGate GetDeploymentGate(int releaseId, int releaseEnvironmentId, int stepId) => this.requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, DeploymentGate>((Func<DeploymentSqlComponent, DeploymentGate>) (component => component.GetDeploymentGate(this.projectId, releaseId, releaseEnvironmentId, stepId)));

    public DeploymentGate UpdateDeploymentGate(
      int releaseId,
      int releaseEnvironmentId,
      int stepId,
      GateStatus status,
      Guid? runPlanId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus operationStatus,
      Guid changedBy)
    {
      return this.requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, DeploymentGate>((Func<DeploymentSqlComponent, DeploymentGate>) (component => component.UpdateDeploymentGate(this.projectId, releaseId, releaseEnvironmentId, stepId, status, runPlanId, operationStatus, changedBy)));
    }

    public ReleaseEnvironmentSnapshotDelta GetDeploymentSnapshotDelta(
      int releaseId,
      int deploymentId)
    {
      return this.requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, ReleaseEnvironmentSnapshotDelta>((Func<DeploymentSqlComponent, ReleaseEnvironmentSnapshotDelta>) (component => component.GetReleaseEnvironmentSnapshotDelta(this.projectId, releaseId, deploymentId)));
    }

    public ReleaseEnvironmentSnapshotDelta AddDeploymentSnapshotDelta(
      int releaseId,
      int releaseEnvironmentId,
      int deploymentId,
      IEnumerable<DeploymentGroupPhaseDelta> deploymentGroupPhaseDelta,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables)
    {
      return this.requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, ReleaseEnvironmentSnapshotDelta>((Func<DeploymentSqlComponent, ReleaseEnvironmentSnapshotDelta>) (component => component.AddReleaseEnvironmentSnapshotDelta(this.projectId, releaseId, releaseEnvironmentId, deploymentId, deploymentGroupPhaseDelta, variables)));
    }

    public IEnumerable<string> GetBuildsRetainedByReleases(IEnumerable<int> releaseIds) => this.requestContext.ExecuteWithinUsingWithComponent<RetentionSqlComponent, IEnumerable<string>>((Func<RetentionSqlComponent, IEnumerable<string>>) (component => component.GetBuildsRetainedByReleases(this.projectId, releaseIds)));

    public DeploymentGate IgnoreGates(
      int releaseId,
      int stepId,
      IEnumerable<string> gatesToIgnore,
      string beforeGatesIgnored,
      string afterGatesIgnored,
      string comment,
      bool markProcessed)
    {
      if (gatesToIgnore == null)
        throw new ArgumentNullException(nameof (gatesToIgnore));
      if (!gatesToIgnore.Any<string>())
        throw new InvalidDataException();
      return this.requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, DeploymentGate>((Func<DeploymentSqlComponent, DeploymentGate>) (component => component.UpdateIgnoredGates(this.projectId, releaseId, stepId, gatesToIgnore, beforeGatesIgnored, afterGatesIgnored, comment, this.requestContext.GetUserId(true), markProcessed)));
    }

    public ReleaseManagementJobInfo GetReleaseManagementJobInfo(
      string jobName,
      bool createIfNotExists)
    {
      return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseManagementJobInfoSqlComponent, ReleaseManagementJobInfo>((Func<ReleaseManagementJobInfoSqlComponent, ReleaseManagementJobInfo>) (component => component.GetReleaseManagementJobInfo(this.projectId, jobName, createIfNotExists)));
    }

    public QueuingPolicyResult GetReleaseEnvironmentsTorunAfterCancelingScheduledReleases(
      int releaseDefinitionId,
      int definitionEnvironmentId,
      int releaseId,
      int releaseEnvironmentId,
      int maxConcurrent,
      int maxQueueDepth)
    {
      Guid requestedBy = this.requestContext.GetUserId(true);
      return this.requestContext.ExecuteWithinUsingWithComponent<ReleaseEnvironmentQueueSqlComponent, QueuingPolicyResult>((Func<ReleaseEnvironmentQueueSqlComponent, QueuingPolicyResult>) (component => component.GetReleaseEnvironmentsTorunAfterCancelingScheduledReleases(this.projectId, releaseDefinitionId, definitionEnvironmentId, releaseId, releaseEnvironmentId, maxConcurrent, maxQueueDepth, requestedBy)));
    }
  }
}
