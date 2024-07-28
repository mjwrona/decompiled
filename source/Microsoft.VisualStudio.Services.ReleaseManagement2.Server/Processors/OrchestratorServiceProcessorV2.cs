// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.OrchestratorServiceProcessorV2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.SecretAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.Processor.Builders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  public class OrchestratorServiceProcessorV2 : IEnvironmentOrchestrator
  {
    private readonly IDataAccessLayer dataAccessLayer;
    private readonly IVssRequestContext requestContext;
    private readonly Guid projectId;
    private readonly SecretsHelper secretsHelper;
    private Guid serviceAccountId = Guid.Empty;

    public OrchestratorServiceProcessorV2(IVssRequestContext requestContext, Guid projectId)
      : this(requestContext, projectId, (IDataAccessLayer) new DataAccessLayer(requestContext, projectId), new SecretsHelper())
    {
    }

    protected OrchestratorServiceProcessorV2(
      IVssRequestContext requestContext,
      Guid projectId,
      IDataAccessLayer dataAccessLayer,
      SecretsHelper secretsHelper)
    {
      this.requestContext = requestContext;
      this.projectId = projectId;
      this.dataAccessLayer = dataAccessLayer;
      this.secretsHelper = secretsHelper;
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Used for testing")]
    public static void HandleApprovalPolicyAndRetryReleaseImplementation(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      DefinitionEnvironmentData definitionEnvironmentData,
      int nextTrialNumber,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, DefinitionEnvironmentData, IEnumerable<DefinitionEnvironmentStepData>, int, int> createNextReleaseStep,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, DefinitionEnvironmentData, int, int> moveToNextReleaseStep)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (releaseEnvironment == null)
        throw new ArgumentNullException(nameof (releaseEnvironment));
      if (definitionEnvironmentData == null)
        throw new ArgumentNullException(nameof (definitionEnvironmentData));
      if (createNextReleaseStep == null)
        throw new ArgumentNullException(nameof (createNextReleaseStep));
      if (moveToNextReleaseStep == null)
        throw new ArgumentNullException(nameof (moveToNextReleaseStep));
      ReleaseEnvironmentStep lastRunReleaseStep = releaseEnvironment.GetLastRunReleaseStep();
      if (lastRunReleaseStep != null && lastRunReleaseStep.StepType != EnvironmentStepType.PreDeploy)
      {
        List<DefinitionEnvironmentStepData> list = definitionEnvironmentData.Steps.Where<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (step => step.StepType == EnvironmentStepType.PreDeploy)).ToList<DefinitionEnvironmentStepData>();
        int num1 = list.Max<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, int>) (step => step.Rank));
        if (!list.First<DefinitionEnvironmentStepData>().IsAutomated)
        {
          ReleaseEnvironmentStep latestApprovalStep = releaseEnvironment.GetLatestApprovalStep(EnvironmentStepType.PreDeploy);
          DefinitionEnvironmentStepData environmentStepData = new DefinitionEnvironmentStepData()
          {
            Rank = num1,
            IsAutomated = true,
            IsNotificationOn = false,
            ApproverId = latestApprovalStep != null ? latestApprovalStep.ApproverId : Guid.Empty,
            StepType = EnvironmentStepType.PreDeploy,
            DefinitionEnvironmentId = definitionEnvironmentData.Id
          };
          Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, DefinitionEnvironmentData, IEnumerable<DefinitionEnvironmentStepData>, int, int> action = createNextReleaseStep;
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1 = release;
          DefinitionEnvironmentData definitionEnvironmentData1 = definitionEnvironmentData;
          List<DefinitionEnvironmentStepData> environmentStepDataList = new List<DefinitionEnvironmentStepData>();
          environmentStepDataList.Add(environmentStepData);
          int definitionEnvironmentRank = lastRunReleaseStep.DefinitionEnvironmentRank;
          int num2 = nextTrialNumber;
          action(release1, definitionEnvironmentData1, (IEnumerable<DefinitionEnvironmentStepData>) environmentStepDataList, definitionEnvironmentRank, num2);
          return;
        }
      }
      moveToNextReleaseStep(release, definitionEnvironmentData, 0, nextTrialNumber);
    }

    public void QueueOnStartEnvironmentJob(
      int releaseId,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      int releaseEnvironmentId,
      Guid requestedBy,
      Guid requestedFor,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentReason deploymentReason,
      bool isDefaultJobPriorityHigh)
    {
      this.QueueOnStartEnvironmentJob(releaseId, releaseDefinitionId, definitionEnvironmentId, releaseEnvironmentId, requestedBy, requestedFor, deploymentReason, (string) null, isDefaultJobPriorityHigh, (ReleaseEnvironmentSnapshotDelta) null);
    }

    public void QueueOnStartEnvironmentJob(
      int releaseId,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      int releaseEnvironmentId,
      Guid requestedBy,
      Guid requestedFor,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentReason deploymentReason,
      string comment,
      bool isDefaultJobPriorityHigh)
    {
      this.QueueOnStartEnvironmentJob(releaseId, releaseDefinitionId, definitionEnvironmentId, releaseEnvironmentId, requestedBy, requestedFor, deploymentReason, comment, isDefaultJobPriorityHigh, (ReleaseEnvironmentSnapshotDelta) null);
    }

    public void QueueOnStartEnvironmentJob(
      int releaseId,
      int releaseDefinitionId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      Guid requestedBy,
      Guid requestedFor,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentReason deploymentReason,
      string comment,
      bool isDefaultJobPriorityHigh,
      ReleaseEnvironmentSnapshotDelta deploymentDelta)
    {
      this.QueueOnStartEnvironmentJob(releaseId, releaseDefinitionId, releaseEnvironment.DefinitionEnvironmentId, releaseEnvironment.Id, requestedBy, requestedFor, deploymentReason, comment, isDefaultJobPriorityHigh, deploymentDelta);
      if (!this.requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.UseDirectReleaseStageScheduling") || !(releaseEnvironment.ScheduledOperationId != Guid.Empty) || releaseEnvironment.Status != ReleaseEnvironmentStatus.Scheduled)
        return;
      this.DeleteStageSchedulingJobs((IEnumerable<Guid>) new List<Guid>(1)
      {
        releaseEnvironment.ScheduledOperationId
      });
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public void QueueOnStartEnvironmentJob(
      int releaseId,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      int releaseEnvironmentId,
      Guid requestedBy,
      Guid requestedFor,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentReason deploymentReason,
      string comment,
      bool isDefaultJobPriorityHigh,
      ReleaseEnvironmentSnapshotDelta deploymentDelta)
    {
      if (requestedBy.Equals(Guid.Empty))
        requestedBy = this.requestContext.GetUserId(true);
      if (requestedFor.Equals(Guid.Empty))
        requestedFor = requestedBy;
      StartEnvironmentData startEnvironmentData = StartEnvironmentData.GetStartEnvironmentData(this.projectId, releaseId, releaseDefinitionId, definitionEnvironmentId, releaseEnvironmentId);
      ArgumentUtility.CheckForNull<StartEnvironmentData>(startEnvironmentData, "releaseEnvironmentData");
      if (releaseEnvironmentId > 0)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment = this.dataAccessLayer.QueueReleaseOnEnvironment(releaseDefinitionId, releaseId, definitionEnvironmentId, releaseEnvironmentId, requestedBy, requestedFor, deploymentReason, comment);
        if (deploymentDelta != null)
        {
          deploymentDelta.DeploymentId = deployment.Id;
          this.dataAccessLayer.AddDeploymentSnapshotDelta(deploymentDelta.ReleaseId, deploymentDelta.ReleaseEnvironmentId, deploymentDelta.DeploymentId, (IEnumerable<DeploymentGroupPhaseDelta>) null, deploymentDelta.Variables);
          this.secretsHelper.StoreSecrets(this.requestContext, this.projectId, deploymentDelta);
        }
      }
      bool registryKeyValue = this.requestContext.GetRegistryKeyValue<bool>("/Service/ReleaseManagement/Settings/JobPriorityAboveNormal/StartEnvironmentJob", isDefaultJobPriorityHigh);
      if (this.requestContext.IsFeatureEnabled("AzureDevops.ReleaseManagement.StartReleaseEnvironmentActionRequestProcessorJob"))
      {
        ActionRequestService service = this.requestContext.GetService<ActionRequestService>();
        ActionRequestsProcessorHelper.AddStartReleaseEnvironmentActionRequest(this.requestContext, service, startEnvironmentData);
        service.QueueActionRequestsProcessorJob(this.requestContext, ActionRequestType.StartReleaseEnvironment, false, registryKeyValue);
        this.requestContext.Trace(1960087, TraceLevel.Info, "ReleaseManagementService", "Pipeline", "OSPV2: QueueOnStartEnvironmentJob: jobData: {0}", (object) startEnvironmentData);
      }
      else
      {
        XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) startEnvironmentData);
        ArgumentUtility.CheckForNull<XmlNode>(xml, "jobData");
        this.requestContext.Trace(1960087, TraceLevel.Info, "ReleaseManagementService", "Pipeline", "OSPV2: QueueOnStartEnvironmentJob: jobId: {0}, jobData: {1}", (object) this.requestContext.GetService<TeamFoundationJobService>().QueueOneTimeJob(this.requestContext, "OnStartEnvironment", "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.StartEnvironmentJob", xml, registryKeyValue), (object) xml.OuterXml);
      }
    }

    private static bool HasMatchingSteps(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int definitionEnvironmentId,
      EnvironmentStepType stepType,
      int rank)
    {
      return release.DefinitionSnapshot.Environments.FirstOrDefault<DefinitionEnvironmentData>((Func<DefinitionEnvironmentData, bool>) (s => s.Id == definitionEnvironmentId)).Steps.Count<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (s => s.Rank == rank && s.StepType == stepType)) > 0;
    }

    private static bool IsApprovalPolicyFeatureRequirementMet(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int releaseEnvironmentId,
      int trialNumber)
    {
      if ((trialNumber <= 1 ? 0 : (requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.ApprovalPolicies") ? 1 : 0)) == 0)
        return false;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = (release != null ? release.Environments.SingleOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (env => env.Id == releaseEnvironmentId)) : (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment) null) ?? (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment) null;
      ReleaseEnvironmentStep releaseEnvironmentStep = releaseEnvironment?.GetLatestApprovalStep(EnvironmentStepType.PreDeploy) ?? (ReleaseEnvironmentStep) null;
      if (!((releaseEnvironmentStep != null ? releaseEnvironmentStep.ModifiedOn : DateTime.UtcNow) > (release != null ? release.ModifiedOn : DateTime.UtcNow)))
        return false;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment = releaseEnvironment?.GetLastDeploymentAttempt() ?? (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment) null;
      return (deployment == null || deployment.OperationStatus != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Canceled ? 0 : (deployment != null ? (deployment.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.NotDeployed ? 1 : 0) : 0)) == 0;
    }

    private static string JoinStepIds(IEnumerable<ReleaseEnvironmentStep> steps) => string.Join<int>(",", steps.Select<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (s => s.Id)));

    private static string JoinReleaseIds(IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> releases) => string.Join<int>(",", releases.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int>) (s => s.Id)));

    private static string JoinStepReleaseIds(IEnumerable<ReleaseEnvironmentStep> steps) => string.Join<int>(",", steps.Select<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (s => s.ReleaseId)));

    private static ApprovalOptions GetApprovalOptionsFromReleaseEnvironment(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int releaseEnvironmentId,
      EnvironmentStepType stepType)
    {
      return release.GetEnvironment(releaseEnvironmentId)?.GetApprovalOptions(stepType);
    }

    private static Dictionary<int, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> GetStepReleaseMap(
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> releases,
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps)
    {
      Dictionary<int, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> stepReleaseMap = new Dictionary<int, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>();
      foreach (ReleaseEnvironmentStep releaseEnvironmentStep in releaseEnvironmentSteps)
      {
        ReleaseEnvironmentStep step = releaseEnvironmentStep;
        stepReleaseMap[step.Id] = releases.First<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, bool>) (r => r.Id == step.ReleaseId));
      }
      return stepReleaseMap;
    }

    private static bool CanMoveWorkflowForward(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      ReleaseEnvironmentStep currentReleaseStep,
      IEnumerable<ReleaseEnvironmentStep> releaseSteps)
    {
      List<ReleaseEnvironmentStep> list = releaseSteps.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.Status != ReleaseEnvironmentStepStatus.Reassigned && step.Status != ReleaseEnvironmentStepStatus.Pending && step.Status != ReleaseEnvironmentStepStatus.Skipped)).ToList<ReleaseEnvironmentStep>();
      if (!list.All<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.Status == ReleaseEnvironmentStepStatus.Done)))
        return false;
      ApprovalOptions releaseEnvironment = OrchestratorServiceProcessorV2.GetApprovalOptionsFromReleaseEnvironment(release, currentReleaseStep.ReleaseEnvironmentId, currentReleaseStep.StepType);
      int totalNumberOfApprovers = release.NumberOfApprovalsWithSameRank(currentReleaseStep.DefinitionEnvironmentId, currentReleaseStep.Rank);
      return releaseEnvironment != null ? releaseEnvironment.HasMetRequiredNumberOfApproverCriteria(list.Count<ReleaseEnvironmentStep>(), totalNumberOfApprovers) : list.Count == totalNumberOfApprovers;
    }

    private static void UpdateDefinitionEnvironmentData(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release oldRelease, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release newRelease)
    {
      if (oldRelease == null || newRelease == null)
        return;
      newRelease.DefinitionSnapshot = oldRelease.DefinitionSnapshot.DeepClone();
      newRelease.Environments.ForEach<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>) (x => x.DeploymentSnapshot = oldRelease.Environments.Single<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (y => x.Id == y.Id)).DeploymentSnapshot.DeepClone()));
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release StartRelease(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      StartReleaseValidator.ValidateRelease(release);
      return this.StartReleaseImplementation(release, new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>(this.EvaluateConditionsAndStartRelease), comment);
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release StartReleaseImplementation(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> evaluateConditionsAndStartRelease,
      string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (evaluateConditionsAndStartRelease == null)
        throw new ArgumentNullException(nameof (evaluateConditionsAndStartRelease));
      this.TraceInformationMessage(1960021, "OSPV2: StartReleaseImplementation: Enter. ReleaseId: {0}, ReleaseName: {1}, ReleaseDefinitionName: {2}", (object) release.Id, (object) release.Name, (object) release.ReleaseDefinitionName);
      if (release.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Draft)
        release = this.dataAccessLayer.StartDraftRelease(release, comment);
      evaluateConditionsAndStartRelease(release);
      return release;
    }

    private void UpdateVariables(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release oldRelease, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release newRelease)
    {
      if (oldRelease == null || newRelease == null)
        return;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment1 in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>) newRelease.Environments)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment env = environment1;
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = oldRelease.Environments.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (environment => environment.Id == env.Id));
        if (env.Variables.Count > 0)
          env.Variables.ForEach<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>((Action<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>) (variable => this.TraceInformationMessage(1960071, "OSPV2: UpdateVariables: Variable with key {0} already present with value {1}", (object) variable.Key, (object) variable.Value)));
        if (releaseEnvironment != null && releaseEnvironment.Variables != null)
        {
          foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>) releaseEnvironment.Variables)
            env.Variables[variable.Key] = variable.Value;
        }
      }
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release StartReleaseOnEnvironment(
      int releaseId,
      int releaseEnvironmentId)
    {
      this.TraceInformationMessage(1960084, "OSPV2: StartReleaseOnEnvironment: Enter. ReleaseId: {0}, ReleaseEnvironmentId: {1}", (object) releaseId, (object) releaseEnvironmentId);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = this.dataAccessLayer.GetRelease(releaseId);
      return release.Status != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Abandoned ? this.StartReleaseOnEnvironmentImplementation(release, releaseEnvironmentId, new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, DefinitionEnvironmentData, int, int>(this.MoveToNextReleaseStep), new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, DefinitionEnvironmentData, int>(this.HandleApprovalPolicyAndRetryRelease)) : throw new InvalidReleaseStatusException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.CannotStartDeploymentAsReleaseIsAbandoned, (object) release.Id, (object) releaseEnvironmentId));
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release StartReleaseOnEnvironmentImplementation(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int releaseEnvironmentId,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, DefinitionEnvironmentData, int, int> moveToNextReleaseStep,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, DefinitionEnvironmentData, int> handleApprovalPolicyAndRetryRelease)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (moveToNextReleaseStep == null)
        throw new ArgumentNullException(nameof (moveToNextReleaseStep));
      if (handleApprovalPolicyAndRetryRelease == null)
        throw new ArgumentNullException(nameof (handleApprovalPolicyAndRetryRelease));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = release.Environments.SingleOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (e => e.Id == releaseEnvironmentId));
      if (releaseEnvironment == null)
      {
        this.TraceInformationMessage(1971036, "OSPV2: StartReleaseOnEnvironment: Provided invalid environment Id {0}, ReleaseId : {1}", (object) releaseEnvironmentId, (object) release.Id);
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.NotValidReleaseEnvironmentId, (object) release.Name, (object) release.Id, (object) releaseEnvironmentId));
      }
      int deploymentAttemptId = releaseEnvironment.GetLastDeploymentAttemptId();
      this.ProcessReleaseEnvironmentChecks(release, releaseEnvironment, deploymentAttemptId);
      release = this.dataAccessLayer.UpdateReleaseEnvironmentStatus(release, releaseEnvironment.Id, ReleaseEnvironmentStatus.Queued, ReleaseEnvironmentStatus.InProgress, (ReleaseEnvironmentStatusChangeDetails) null, (string) null, deploymentAttemptId);
      this.FireEnvironmentExecutionStartEvent(release, releaseEnvironment);
      this.dataAccessLayer.PublishCommitStatusForRelease(release, releaseEnvironment.Id);
      DefinitionEnvironmentData definitionEnvironmentData = release.DefinitionSnapshot.Environments.Single<DefinitionEnvironmentData>((Func<DefinitionEnvironmentData, bool>) (env => env.Id == releaseEnvironment.DefinitionEnvironmentId));
      int trialNumberForSteps = releaseEnvironment.GetNextTrialNumberForSteps();
      this.TraceInformationMessage(1960081, "OSPV2: StartReleaseOnEnvironment: Starting release (ReleaseId: {0}, Name: {1}) on Environment (Id: {2}, Name: {3}, trialNumber: {4}))", (object) release.Id, (object) release.Name, (object) releaseEnvironmentId, (object) definitionEnvironmentData.Name, (object) trialNumberForSteps);
      if (OrchestratorServiceProcessorV2.IsApprovalPolicyFeatureRequirementMet(this.requestContext, release, releaseEnvironmentId, trialNumberForSteps))
        handleApprovalPolicyAndRetryRelease(release, releaseEnvironment, definitionEnvironmentData, trialNumberForSteps);
      else
        moveToNextReleaseStep(release, definitionEnvironmentData, 0, trialNumberForSteps);
      return release;
    }

    private void RetainBuildIfRequired(int releaseId, int releaseStepId)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = this.dataAccessLayer.GetRelease(releaseId);
      if (release == null)
        return;
      IEnumerable<string> retainedByReleases = this.dataAccessLayer.GetBuildsRetainedByReleases((IEnumerable<int>) new int[1]
      {
        release.Id
      });
      if (retainedByReleases != null && retainedByReleases.Count<string>() != 0)
        return;
      int releaseEnvironmentId = release.GetStep(releaseStepId).ReleaseEnvironmentId;
      int definitionEnvironmentId = release.GetDefinitionEnvironmentId(releaseEnvironmentId);
      DefinitionEnvironment definitionEnvironment = this.dataAccessLayer.GetDefinitionEnvironment(release.ReleaseDefinitionId, definitionEnvironmentId);
      if (definitionEnvironment == null || definitionEnvironment.RetentionPolicy == null || !definitionEnvironment.RetentionPolicy.RetainBuild)
        return;
      QueueJobUtility.QueueUpdateRetainBuildJob(this.requestContext, UpdateRetainBuildData.GetUpdateRetainBuildData(UpdateRetainBuildReason.JobStarted, this.projectId, release.ReleaseDefinitionId, releaseIds: new Collection<int>()
      {
        release.Id
      }));
    }

    private Guid ServiceAccountId
    {
      get
      {
        if (this.serviceAccountId.Equals(Guid.Empty))
        {
          Microsoft.VisualStudio.Services.Identity.Identity serviceUsersGroup = this.requestContext.GetService<IdentityService>().GetServiceUsersGroup(this.requestContext);
          if (serviceUsersGroup != null)
            this.serviceAccountId = serviceUsersGroup.Id;
        }
        return this.serviceAccountId;
      }
    }

    private bool CheckApproverAllowed(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      ReleaseEnvironmentStep releaseEnvironmentStep)
    {
      if (releaseEnvironmentStep.IsAutomated || !releaseEnvironmentStep.IsApprovalStep())
        return true;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment = release.GetEnvironment(releaseEnvironmentStep.ReleaseEnvironmentId);
      if (environment == null)
        return true;
      ApprovalOptions releaseEnvironment = OrchestratorServiceProcessorV2.GetApprovalOptionsFromReleaseEnvironment(release, releaseEnvironmentStep.ReleaseEnvironmentId, releaseEnvironmentStep.StepType);
      if (releaseEnvironment == null)
        return true;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deploymentByAttempt = environment.GetDeploymentByAttempt(releaseEnvironmentStep.TrialNumber);
      Guid guid1 = deploymentByAttempt == null ? Guid.Empty : deploymentByAttempt.RequestedFor;
      Guid guid2 = release.CreatedBy;
      if (guid1.Equals(this.ServiceAccountId))
        guid1 = Guid.Empty;
      if (guid2.Equals(this.ServiceAccountId))
        guid2 = Guid.Empty;
      Guid empty = Guid.Empty;
      if (empty.Equals(guid1))
      {
        empty = Guid.Empty;
        if (empty.Equals(guid2))
          return true;
      }
      return releaseEnvironment.IsApproverAllowed(releaseEnvironmentStep.ActualApproverId, guid2, guid1);
    }

    private void HandleApprovalPolicyAndRetryRelease(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      DefinitionEnvironmentData definitionEnvironmentData,
      int nextTrialNumber)
    {
      OrchestratorServiceProcessorV2.HandleApprovalPolicyAndRetryReleaseImplementation(release, releaseEnvironment, definitionEnvironmentData, nextTrialNumber, new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, DefinitionEnvironmentData, IEnumerable<DefinitionEnvironmentStepData>, int, int>(this.CreateNextReleaseStep), new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, DefinitionEnvironmentData, int, int>(this.MoveToNextReleaseStep));
    }

    public void MoveToNextReleaseStep(int releaseId, int completedStepId)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = this.dataAccessLayer.GetRelease(releaseId);
      ReleaseEnvironmentStep completedStep = release.GetStep(completedStepId);
      DefinitionEnvironmentData currentEnvironment = release.DefinitionSnapshot.Environments.Single<DefinitionEnvironmentData>((Func<DefinitionEnvironmentData, bool>) (env => env.Id == completedStep.DefinitionEnvironmentId));
      this.MoveToNextReleaseStep(release, currentEnvironment, completedStep.Rank, completedStep.TrialNumber);
    }

    private void MoveToNextReleaseStep(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      DefinitionEnvironmentData currentEnvironment,
      int currentStepRank,
      int trialNumber)
    {
      this.MoveToNextReleaseStepImplementation(release, currentEnvironment, new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, DefinitionEnvironmentData, IEnumerable<DefinitionEnvironmentStepData>, int, int>(this.CreateNextReleaseStep), currentStepRank, trialNumber);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "In case of parallel approvals it is required")]
    public void MoveToNextReleaseStepImplementation(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      DefinitionEnvironmentData currentEnvironment,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, DefinitionEnvironmentData, IEnumerable<DefinitionEnvironmentStepData>, int, int> createNextReleaseStep,
      int currentStepRank,
      int trialNumber)
    {
      this.TraceInformationMessage(1960009, "OSPV2: MoveToNextReleaseStepImplementation: Enter. ReleaseId: {0}, CurrentEnvironmentId: {1}, CurrentStepRank: {2}", (object) release.Id, (object) currentEnvironment.Id, (object) currentStepRank);
      IEnumerable<DefinitionEnvironmentStepData> environmentStepDatas = currentEnvironment.Steps.Where<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (s => s.Rank == currentStepRank + 1));
      createNextReleaseStep(release, currentEnvironment, environmentStepDatas, currentEnvironment.Rank, trialNumber);
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release CancelDeploymentOnEnvironment(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int releaseEnvironmentId,
      string comment,
      bool addCommentAsDeploymentIssue,
      bool forceUpdate)
    {
      return this.CancelDeploymentOnEnvironmentImplementation(release, releaseEnvironmentId, new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int, EnvironmentStatus, string>(this.FireEnvironmentCompletionEvent), comment, addCommentAsDeploymentIssue, new Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, ReleaseEnvironmentStep, bool>(new DeployPhaseOrchestrator(this.requestContext, this.projectId).CancelDeployPhase), forceUpdate);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Reviewed")]
    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release CancelDeploymentOnEnvironmentImplementation(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int releaseEnvironmentId,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int, EnvironmentStatus, string> fireEnvironmentCompletionEvent,
      string comment,
      bool addCommentAsDeploymentIssue,
      Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, ReleaseEnvironmentStep, bool> cancelDeployPhase,
      bool forceUpdate = false)
    {
      this.TraceEnterInformationMessage(1960088, nameof (CancelDeploymentOnEnvironmentImplementation));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = release != null ? release.GetEnvironment(releaseEnvironmentId) : throw new ArgumentNullException(nameof (release));
      ReleaseEnvironmentStep lastRunReleaseStep = releaseEnvironment.GetLastRunReleaseStep();
      int latestTrialNumber = releaseEnvironment.GetLatestTrialNumber();
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deploymentByAttempt = releaseEnvironment.GetDeploymentByAttempt(latestTrialNumber);
      if (lastRunReleaseStep != null && lastRunReleaseStep.TrialNumber == latestTrialNumber && deploymentByAttempt != null && deploymentByAttempt.OperationStatus == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Cancelling && !forceUpdate)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.DeploymentAlreadyInCancelingState, (object) releaseEnvironment.Status, (object) ReleaseEnvironmentStatus.Canceled));
      bool isPotentialStateTransition = false;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1 = this.CancelDeployment(release, deploymentByAttempt, releaseEnvironmentId, comment, addCommentAsDeploymentIssue, cancelDeployPhase, forceUpdate, out isPotentialStateTransition);
      if (isPotentialStateTransition)
        this.SendCancelUpdateNotification(release1, releaseEnvironmentId, comment, 1960089, fireEnvironmentCompletionEvent);
      return release1;
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release RetryDeploymentOnEnvironment(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int releaseEnvironmentId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentUpdateMetadata environmentUpdateData)
    {
      return this.RetryDeploymentOnEnvironmentImplementation(release, releaseEnvironmentId, new Action<int, int, int, int, Guid, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentReason, string, bool, ReleaseEnvironmentSnapshotDelta>(this.QueueOnStartEnvironmentJob), environmentUpdateData);
    }

    public void SendCancelUpdateNotification(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int releaseEnvironmentId,
      string comment,
      int tracePoint)
    {
      this.SendCancelUpdateNotification(release, releaseEnvironmentId, comment, tracePoint, new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int, EnvironmentStatus, string>(this.FireEnvironmentCompletionEvent));
    }

    private Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release CancelDeployment(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment currentDeployment,
      int releaseEnvironmentId,
      string comment,
      bool addCommentAsDeploymentIssue,
      Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, ReleaseEnvironmentStep, bool> cancelDeployPhase,
      bool forceUpdate,
      out bool isPotentialStateTransition)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1 = this.dataAccessLayer.CancelDeploymentOnEnvironment(release.Id, releaseEnvironmentId, comment, addCommentAsDeploymentIssue, true);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment = release1.GetEnvironment(releaseEnvironmentId);
      if (environment == null)
      {
        isPotentialStateTransition = true;
        return release1;
      }
      if (this.requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.UseDirectReleaseStageScheduling") && environment.ScheduledOperationId != Guid.Empty && environment.Status == ReleaseEnvironmentStatus.Canceled)
        this.DeleteStageSchedulingJobs((IEnumerable<Guid>) new List<Guid>(1)
        {
          environment.ScheduledOperationId
        });
      ReleaseEnvironmentStep lastRunReleaseStep = environment.GetLastRunReleaseStep();
      if (lastRunReleaseStep == null)
      {
        isPotentialStateTransition = true;
        return release1;
      }
      if (lastRunReleaseStep.IsApprovalStep())
        ReleaseOperationHelper.DeleteJobDefinition(this.requestContext, lastRunReleaseStep.ApprovalTimeoutJobId, release.Id);
      if (currentDeployment.OperationStatus == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.EvaluatingGates && lastRunReleaseStep.IsGateStep())
      {
        DeploymentGate deploymentGate = this.dataAccessLayer.GetDeploymentGate(release.Id, releaseEnvironmentId, lastRunReleaseStep.Id);
        GreenlightingRunner greenlightingRunner1 = new GreenlightingRunner(this.requestContext, this.projectId);
        if (deploymentGate != null)
        {
          Guid? runPlanId = deploymentGate.RunPlanId;
          if (runPlanId.HasValue)
          {
            GreenlightingRunner greenlightingRunner2 = greenlightingRunner1;
            int releaseId = deploymentGate.ReleaseId;
            int environmentStepId = deploymentGate.ReleaseEnvironmentStepId;
            runPlanId = deploymentGate.RunPlanId;
            Guid planId = runPlanId.Value;
            greenlightingRunner2.Cancel(releaseId, environmentStepId, planId);
          }
        }
      }
      if (environment.Status == ReleaseEnvironmentStatus.Canceled)
      {
        isPotentialStateTransition = true;
        return release1;
      }
      try
      {
        int num = cancelDeployPhase(release1, lastRunReleaseStep) ? 1 : 0;
      }
      catch (Exception ex) when (ex is TaskOrchestrationPlanTerminatedException || ex is TaskOrchestrationPlanNotFoundException)
      {
        return this.MoveDeploymentToCancelledState(release.Id, releaseEnvironmentId, comment, addCommentAsDeploymentIssue, out isPotentialStateTransition);
      }
      catch (Exception ex) when (forceUpdate)
      {
        this.TraceInformationMessage(1976390, "OSPV2: CancelDeployment with force: Enter. ReleaseId: {0}, ReleaseEnvironmentId: {1}, Exception: {2}", (object) release.Id, (object) releaseEnvironmentId, (object) ex);
      }
      if (forceUpdate)
        return this.MoveDeploymentToCancelledState(release.Id, releaseEnvironmentId, comment, addCommentAsDeploymentIssue, out isPotentialStateTransition);
      isPotentialStateTransition = false;
      this.SendReleaseEnvironmentUpdatedEvent(release1.ReleaseDefinitionId, release1.Id, releaseEnvironmentId);
      return release1;
    }

    private Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release MoveDeploymentToCancelledState(
      int releaseId,
      int releaseEnvironmentId,
      string comment,
      bool addCommentAsDeploymentIssue,
      out bool isPotentialStateTransition)
    {
      try
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release cancelledState = this.dataAccessLayer.CancelDeploymentOnEnvironment(releaseId, releaseEnvironmentId, comment, addCommentAsDeploymentIssue, false);
        isPotentialStateTransition = true;
        return cancelledState;
      }
      catch (InvalidReleaseEnvironmentStatusUpdateException ex)
      {
        isPotentialStateTransition = false;
        return this.dataAccessLayer.GetRelease(releaseId);
      }
    }

    private void SendCancelUpdateNotification(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int releaseEnvironmentId,
      string comment,
      int tracePoint,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int, EnvironmentStatus, string> fireEnvironmentCompletionEvent)
    {
      if (release == null)
        return;
      EnvironmentStatus releaseEnvironmentStatus = EnvironmentStatus.Canceled;
      this.requestContext.SendReleaseEnvironmentStatusUpdatedNotification(this.projectId, release.ReleaseDefinitionId, release.Id, releaseEnvironmentId, releaseEnvironmentStatus, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus.Failed, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Canceled);
      this.SendReleaseEnvironmentUpdatedEvent(release.ReleaseDefinitionId, release.Id, releaseEnvironmentId);
      fireEnvironmentCompletionEvent(release, releaseEnvironmentId, releaseEnvironmentStatus, comment);
      this.TraceLeaveInformationMessage(tracePoint, nameof (SendCancelUpdateNotification));
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Used for testing")]
    private Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release RetryDeploymentOnEnvironmentImplementation(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int releaseEnvironmentId,
      Action<int, int, int, int, Guid, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentReason, string, bool, ReleaseEnvironmentSnapshotDelta> queueStartEnvironmentJob,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentUpdateMetadata environmentUpdateData)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (environmentUpdateData == null)
        throw new ArgumentNullException(nameof (environmentUpdateData));
      if (queueStartEnvironmentJob == null)
        throw new ArgumentNullException(nameof (queueStartEnvironmentJob));
      this.TraceInformationMessage(1960030, "OSPV2: RetryDeploymentOnEnvironmentImplementation: Enter. ReleaseId: {0}, ReleaseName: {1}, ReleaseDefinitionName: {2}, IsDeploymentTimeVariableOverride: {3}", (object) release.Id, (object) release.Name, (object) release.ReleaseDefinitionName, (object) !environmentUpdateData.Variables.IsNullOrEmpty<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>());
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = release.Environments.Single<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (environment => environment.Id == releaseEnvironmentId));
      DefinitionEnvironmentData definitionEnvironmentData = release.DefinitionSnapshot.Environments.First<DefinitionEnvironmentData>((Func<DefinitionEnvironmentData, bool>) (env => env.Id == releaseEnvironment.DefinitionEnvironmentId));
      release.Environments.Single<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (env => env.Id == releaseEnvironmentId)).Status = ReleaseEnvironmentStatus.Queued;
      Guid userId = this.requestContext.GetUserId(true);
      ReleaseEnvironmentSnapshotDelta environmentSnapshotDelta = (ReleaseEnvironmentSnapshotDelta) null;
      if (!environmentUpdateData.Variables.IsNullOrEmpty<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>())
        environmentSnapshotDelta = new ReleaseEnvironmentSnapshotDelta(release.Id, releaseEnvironmentId, environmentUpdateData.Variables);
      this.TraceInformationMessage(1960033, "OSPV2: RetryDeploymentOnEnvironmentImplementation: Moving to QueueOnStartEnvironmentJob for next release step. ReleaseId: {0}, EnvrionmentName: {1}, DefinitionEnvironmentRank: {2}", (object) release.Id, (object) definitionEnvironmentData.Name, (object) definitionEnvironmentData.Rank);
      queueStartEnvironmentJob(release.Id, release.ReleaseDefinitionId, releaseEnvironment.DefinitionEnvironmentId, releaseEnvironmentId, userId, userId, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentReason.Manual, environmentUpdateData.Comment, true, environmentSnapshotDelta);
      return release;
    }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release Abandon(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      this.TraceEnterInformationMessage(1960038, nameof (Abandon));
      if (release.Environments.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (env => env.Status == ReleaseEnvironmentStatus.InProgress || env.Status == ReleaseEnvironmentStatus.Queued)))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.CannotAbandonRelease, (object) release.Name));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1 = this.dataAccessLayer.UpdateReleaseStatus(release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Abandoned, comment);
      if (this.requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.UseDirectReleaseStageScheduling"))
        this.DeleteStageSchedulingJobs((IEnumerable<Guid>) release1.Environments.Where<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (e => e.Status == ReleaseEnvironmentStatus.NotStarted && e.ScheduledOperationId != Guid.Empty)).Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, Guid>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, Guid>) (e => e.ScheduledOperationId)).ToList<Guid>());
      this.FireReleaseAbandonedEvent(release1);
      this.TraceLeaveInformationMessage(1960039, nameof (Abandon));
      return release1;
    }

    private void DeleteStageSchedulingJobs(IEnumerable<Guid> schedulingStageJobIds)
    {
      if (!schedulingStageJobIds.Any<Guid>())
        return;
      IEnumerable<Guid> guids = ReleaseOperationHelper.GetJobService(this.requestContext).QueryJobDefinitions(this.requestContext, schedulingStageJobIds).Where<TeamFoundationJobDefinition>((Func<TeamFoundationJobDefinition, bool>) (job => job.ExtensionName == "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtensions.ScheduleStageJobExtension")).Select<TeamFoundationJobDefinition, Guid>((Func<TeamFoundationJobDefinition, Guid>) (i => i.JobId));
      if (!guids.Any<Guid>())
        return;
      ReleaseOperationHelper.UpdateJobs(this.requestContext, (IEnumerable<TeamFoundationJobDefinition>) null, guids);
    }

    private void ExecuteReleaseStep(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release, ReleaseEnvironmentStep releaseEnvironmentStep)
    {
      this.TraceInformationMessage(1960051, "OSPV2: ExecuteReleaseStep: Enter. ReleaseId: {0}, ReleaseStepId: {1}", (object) release.Id, (object) releaseEnvironmentStep.Id);
      switch (releaseEnvironmentStep.StepType)
      {
        case EnvironmentStepType.PreDeploy:
        case EnvironmentStepType.PostDeploy:
          this.ExecuteApprovalStep(release, releaseEnvironmentStep, new Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, ReleaseEnvironmentStep, Guid, string, IEnumerable<ReleaseEnvironmentStep>>(this.AcceptStep), (Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, ReleaseEnvironmentStep>) ((release1, approvalSteptus) => this.FireApprovalPendingEvent(release1, approvalSteptus, release.Id)));
          break;
        case EnvironmentStepType.Deploy:
          this.StartDeployment(release, releaseEnvironmentStep);
          break;
        case EnvironmentStepType.PreGate:
        case EnvironmentStepType.PostGate:
          this.ExecuteGreenlightingStep(release, releaseEnvironmentStep);
          break;
      }
    }

    private void ExecuteGreenlightingStep(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release, ReleaseEnvironmentStep step) => new GreenlightingRunner(this.requestContext, this.projectId).Run(release, step);

    public void ExecuteApprovalStep(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      ReleaseEnvironmentStep releaseEnvironmentStep,
      Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, ReleaseEnvironmentStep, Guid, string, IEnumerable<ReleaseEnvironmentStep>> acceptStep,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, ReleaseEnvironmentStep> fireApprovalPendingEvent)
    {
      if (releaseEnvironmentStep.StepType == EnvironmentStepType.PostDeploy && !releaseEnvironmentStep.IsAutomated)
      {
        this.dataAccessLayer.SendReleaseApprovalPendingEvent(release.ReleaseDefinitionId, release.Id, releaseEnvironmentStep.ReleaseEnvironmentId, releaseEnvironmentStep.Id, releaseEnvironmentStep.ApproverId);
        fireApprovalPendingEvent(release, releaseEnvironmentStep);
      }
      if (releaseEnvironmentStep.Status == ReleaseEnvironmentStepStatus.Pending && releaseEnvironmentStep.IsAutomated && releaseEnvironmentStep.StepType != EnvironmentStepType.Deploy)
      {
        IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps1 = acceptStep(release, releaseEnvironmentStep, Guid.Empty, string.Empty);
      }
      if (releaseEnvironmentStep.StepType != EnvironmentStepType.PreDeploy || releaseEnvironmentStep.IsAutomated)
        return;
      ReleaseEnvironmentStep matchingPreviousEnvironmentStep = releaseEnvironmentStep.EvaluateApprovalPolicyAutoTriggeredAndPreviousEnvironmentApproved(release).FirstOrDefault<ReleaseEnvironmentStep>();
      if (matchingPreviousEnvironmentStep != null)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = release.Environments.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (env => env.Id == matchingPreviousEnvironmentStep.ReleaseEnvironmentId));
        if (releaseEnvironment == null)
          return;
        this.TraceInformationMessage(1960096, "OSPV2: ExecuteReleaseStepImplementation: Skipping approval due to already approval on Environment: {0}, ApprovedBy: {1}, AttempNo: {2}", (object) releaseEnvironment.Name, (object) matchingPreviousEnvironmentStep.ApproverId, (object) matchingPreviousEnvironmentStep.TrialNumber);
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.SkipApprovalForAutomaticAndPreviousSucceededEnvironment, (object) releaseEnvironment.Name, (object) matchingPreviousEnvironmentStep.TrialNumber);
        Guid userId = this.requestContext.GetUserId(true);
        IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps2 = acceptStep(release, releaseEnvironmentStep, userId, str);
      }
      else
      {
        this.dataAccessLayer.SendReleaseApprovalPendingEvent(release.ReleaseDefinitionId, release.Id, releaseEnvironmentStep.ReleaseEnvironmentId, releaseEnvironmentStep.Id, releaseEnvironmentStep.ApproverId);
        fireApprovalPendingEvent(release, releaseEnvironmentStep);
      }
    }

    private void StartDeployment(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release, ReleaseEnvironmentStep releaseEnvironmentStep)
    {
      this.TraceInformationMessage(1960054, "OSPV2: ExecuteReleaseStepImplementation: Executing deploy step. ReleaseId: {0}, ReleaseStepId: {1}, DefinitionEnvironmentId: {2}", (object) release.Id, (object) releaseEnvironmentStep.Id, (object) releaseEnvironmentStep.DefinitionEnvironmentId);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment = release.Environments.Single<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (x => x.Id == releaseEnvironmentStep.ReleaseEnvironmentId));
      if (!releaseEnvironmentStep.ApprovalTimeoutJobId.Equals(Guid.Empty) && environment.ShouldDeferDeployStepExecution() || environment.ShouldDeferDeployStepExecution())
      {
        if (releaseEnvironmentStep.ApprovalTimeoutJobId.Equals(Guid.Empty))
          releaseEnvironmentStep.ApprovalTimeoutJobId = Guid.NewGuid();
        this.QueueDeployStep(release, releaseEnvironmentStep, environment.ScheduledDeploymentTime.Value);
      }
      else
        this.ExecuteDeployStep(release, releaseEnvironmentStep);
    }

    public void QueueDeployStep(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      ReleaseEnvironmentStep releaseEnvironmentStep,
      DateTime scheduledDateTime)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.QueueDeployStepImplementation(release, releaseEnvironmentStep, scheduledDateTime, OrchestratorServiceProcessorV2.\u003C\u003EO.\u003C0\u003E__GetDeferredExecutionJobDefinitionForDeployStep ?? (OrchestratorServiceProcessorV2.\u003C\u003EO.\u003C0\u003E__GetDeferredExecutionJobDefinitionForDeployStep = new Func<Guid, ReleaseEnvironmentStep, DateTime, TeamFoundationJobDefinition>(ReleaseEnvironmentExtensions.GetDeferredExecutionJobDefinitionForDeployStep)), OrchestratorServiceProcessorV2.\u003C\u003EO.\u003C1\u003E__CreateJob ?? (OrchestratorServiceProcessorV2.\u003C\u003EO.\u003C1\u003E__CreateJob = new Action<IVssRequestContext, TeamFoundationJobDefinition>(ReleaseOperationHelper.CreateJob)), new Func<ReleaseEnvironmentStep, Guid, string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, IEnumerable<ReleaseEnvironmentStep>>(this.RejectStep));
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Needed for testing")]
    public void QueueDeployStepImplementation(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      ReleaseEnvironmentStep releaseEnvironmentStep,
      DateTime scheduledDateTime,
      Func<Guid, ReleaseEnvironmentStep, DateTime, TeamFoundationJobDefinition> getDeferredExecutionJob,
      Action<IVssRequestContext, TeamFoundationJobDefinition> createJobDefinition,
      Func<ReleaseEnvironmentStep, Guid, string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, IEnumerable<ReleaseEnvironmentStep>> rejectStep)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (releaseEnvironmentStep == null)
        throw new ArgumentNullException(nameof (releaseEnvironmentStep));
      if (getDeferredExecutionJob == null)
        throw new ArgumentNullException(nameof (getDeferredExecutionJob));
      if (createJobDefinition == null)
        throw new ArgumentNullException(nameof (createJobDefinition));
      if (rejectStep == null)
        throw new ArgumentNullException(nameof (rejectStep));
      TeamFoundationJobDefinition foundationJobDefinition = getDeferredExecutionJob(this.projectId, releaseEnvironmentStep, scheduledDateTime);
      this.TraceInformationMessage(1960151, "OSPV2: QueueDeployStep for ReleaseId : {0}, StepId : {1}, JobDetails : {2}", (object) release.Id, (object) releaseEnvironmentStep.Id, (object) foundationJobDefinition.ToString());
      try
      {
        createJobDefinition(this.requestContext, foundationJobDefinition);
      }
      catch (Exception ex)
      {
        this.TraceInformationMessage(1960093, "OSPV2: QueueDeployStep: Execution of QueueDeployStepImplementation failed, adding the exception to the exceptions log in the DB. ReleaseId: {0}, ReleaseStepId: {1}, Exception: {2}", (object) release.Id, (object) releaseEnvironmentStep.Id, (object) ex);
        releaseEnvironmentStep.Logs += TeamFoundationExceptionFormatter.FormatException(ex, false);
        this.dataAccessLayer.UpdateReleaseStep(releaseEnvironmentStep);
        IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps = rejectStep(releaseEnvironmentStep, Guid.Empty, string.Empty, release);
        throw;
      }
    }

    public void ExecuteDeployStep(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release, ReleaseEnvironmentStep releaseEnvironmentStep)
    {
      if (releaseEnvironmentStep == null)
        throw new ArgumentNullException(nameof (releaseEnvironmentStep));
      try
      {
        new DeployPhaseOrchestrator(this.requestContext, this.projectId).ProcessDeployPhases(release, releaseEnvironmentStep);
      }
      catch (Exception ex)
      {
        releaseEnvironmentStep.Logs += TeamFoundationExceptionFormatter.FormatException(ex, false);
        this.dataAccessLayer.UpdateReleaseStep(releaseEnvironmentStep);
        this.RejectStep(releaseEnvironmentStep, this.requestContext.GetUserId(), string.Empty, release);
        throw;
      }
    }

    public void HandleJobStarted(int releaseId, int releaseStepId, int releaseDeployPhaseId)
    {
      this.TraceInformationMessage(1973303, "OSPV2: HandleJobStartedImplementation: Enter. ReleaseStepId: {0}", (object) releaseStepId);
      try
      {
        this.dataAccessLayer.HandleEnvironmentDeployJobStarted(releaseId, releaseStepId, releaseDeployPhaseId);
      }
      catch (InvalidReleaseEnvironmentStepStatusException ex)
      {
        this.TraceInformationMessage(1973303, "OSPV2: HandleJobStartedImplementation: Failed with exception {0}, for releaseId {1}, ReleaseStepId: {2}", (object) ex.Message, (object) releaseId, (object) releaseStepId);
        throw new TaskOrchestrationPlanCanceledException(string.Empty);
      }
      this.RetainBuildIfRequired(releaseId, releaseStepId);
    }

    public void HandlePipelineAssigned(int releaseId, int releaseStepId)
    {
      try
      {
        this.TraceInformationMessage(1900029, "OSPV2: HandlePipelineAssigned: Enter. ReleaseId: {0}, ReleaseStepId: {1}", (object) releaseId, (object) releaseStepId);
        this.dataAccessLayer.HandlePipelineAssigned(releaseId, releaseStepId);
      }
      catch (DeploymentUpdateNotAllowedException ex)
      {
        this.TraceInformationMessage(1900029, "OSPV2: HandlePipelineAssigned: Failed with exception {0}, for releaseId {1}, ReleaseStepId: {2}", (object) ex.Message, (object) releaseId, (object) releaseStepId);
        throw new TaskOrchestrationPlanCanceledException(string.Empty);
      }
      catch (Exception ex)
      {
        this.TraceInformationMessage(1900018, "OSPV2: HandlePipelineAssigned: Execution of HandlePipelineAssignedImplementation failed, adding the exception to the exceptions log in the DB. ReleaseId: {0}, ReleaseStepId: {1}, Exception: {2}", (object) releaseId, (object) releaseStepId, (object) ex);
        throw;
      }
    }

    public void HandleQueuedForPipeline(int releaseId, int releaseStepId)
    {
      try
      {
        this.TraceInformationMessage(1900030, "OSPV2: HandleQueuedForPipeline: Enter. ReleaseId: {0}, ReleaseStepId: {1}", (object) releaseId, (object) releaseStepId);
        this.dataAccessLayer.HandleQueuedForPipeline(releaseId, releaseStepId);
      }
      catch (DeploymentUpdateNotAllowedException ex)
      {
        this.TraceInformationMessage(1900030, "OSPV2: HandleQueuedForPipeline: Failed with exception {0}, for releaseId {1}, ReleaseStepId: {2}", (object) ex.Message, (object) releaseId, (object) releaseStepId);
        throw new TaskOrchestrationPlanCanceledException(string.Empty);
      }
      catch (Exception ex)
      {
        this.TraceInformationMessage(1900031, "OSPV2: HandleQueuedForPipeline: Execution of HandlePipelineAssignedImplementation failed, adding the exception to the exceptions log in the DB. ReleaseId: {0}, ReleaseStepId: {1}, Exception: {2}", (object) releaseId, (object) releaseStepId, (object) ex);
        throw;
      }
    }

    public void AcceptDeployStep(
      int releaseId,
      int deployStepId,
      ReleaseEnvironmentStepStatus status)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = this.dataAccessLayer.GetRelease(releaseId);
      this.AcceptDeployStep(deployStepId, status, release);
    }

    public IEnumerable<ReleaseEnvironmentStep> AcceptDeployStep(
      int releaseEnvironmentStepId,
      ReleaseEnvironmentStepStatus releaseEnvironmentStepStatus,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      ReleaseEnvironmentStep updatedReleaseEnvironmentStep;
      if (this.SendDeploymentCancelledNotificationIfRequired(release, releaseEnvironmentStepId, out updatedReleaseEnvironmentStep))
        return (IEnumerable<ReleaseEnvironmentStep>) null;
      ReleaseEnvironmentStepStatus status = updatedReleaseEnvironmentStep.Status;
      try
      {
        updatedReleaseEnvironmentStep.Status = releaseEnvironmentStepStatus;
        return this.AcceptStep(release, updatedReleaseEnvironmentStep, Guid.Empty, string.Empty);
      }
      catch (DeployStepUpdateException ex1)
      {
        try
        {
          release = this.dataAccessLayer.GetRelease(release.Id);
          if (this.SendDeploymentCancelledNotificationIfRequired(release, releaseEnvironmentStepId, out updatedReleaseEnvironmentStep))
            return (IEnumerable<ReleaseEnvironmentStep>) null;
        }
        catch (ReleaseNotFoundException ex2)
        {
          return (IEnumerable<ReleaseEnvironmentStep>) null;
        }
        this.TraceInformationMessage(1960063, "OSPV2: AcceptStepImplementation from DT: Failed with exception {0} for release id : {1}, stepId {2}", (object) ex1.Message, (object) release.Id, (object) updatedReleaseEnvironmentStep.Id);
        return (IEnumerable<ReleaseEnvironmentStep>) null;
      }
      catch (Exception ex)
      {
        updatedReleaseEnvironmentStep.Status = status;
        updatedReleaseEnvironmentStep.Logs += TeamFoundationExceptionFormatter.FormatException(ex, false);
        this.dataAccessLayer.UpdateReleaseStep(updatedReleaseEnvironmentStep);
        throw;
      }
    }

    public IEnumerable<ReleaseEnvironmentStep> AcceptStep(
      IEnumerable<ReleaseEnvironmentStep> releaseSteps,
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> releases)
    {
      if (releaseSteps == null)
        throw new ArgumentNullException(nameof (releaseSteps));
      return releases != null ? this.AcceptStepImplementation((IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) releases.ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>(), (IList<ReleaseEnvironmentStep>) releaseSteps.ToList<ReleaseEnvironmentStep>(), new Func<IEnumerable<ReleaseEnvironmentStep>, IEnumerable<ReleaseEnvironmentStep>>(this.UpdatePendingReleaseStep), new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, ReleaseDefinitionEnvironmentsSnapshot, DefinitionEnvironmentData, ReleaseEnvironmentStep>(this.MoveWorkflowForward)) : throw new ArgumentNullException(nameof (releases));
    }

    private IEnumerable<ReleaseEnvironmentStep> AcceptStep(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      ReleaseEnvironmentStep releaseEnvironmentStep,
      Guid actualApproverId,
      string approverComment)
    {
      releaseEnvironmentStep.Status = ReleaseEnvironmentStepStatus.Done;
      releaseEnvironmentStep.ActualApproverId = actualApproverId;
      releaseEnvironmentStep.ApproverComment = approverComment;
      releaseEnvironmentStep.ReleaseId = release.Id;
      return this.AcceptStepImplementation((IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release[1]
      {
        release
      }, (IList<ReleaseEnvironmentStep>) new ReleaseEnvironmentStep[1]
      {
        releaseEnvironmentStep
      }, new Func<IEnumerable<ReleaseEnvironmentStep>, IEnumerable<ReleaseEnvironmentStep>>(this.UpdatePendingReleaseStep), new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, ReleaseDefinitionEnvironmentsSnapshot, DefinitionEnvironmentData, ReleaseEnvironmentStep>(this.MoveWorkflowForward));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private void QueueMoveToNextStepJob(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      DefinitionEnvironmentData environmentData,
      ReleaseEnvironmentStep completedStep)
    {
      if (environmentData.Steps.FirstOrDefault<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (s => s.Rank == completedStep.Rank + 1)) == null)
        return;
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new CreateNextStepData()
      {
        ProjectId = this.projectId,
        ReleaseId = release.Id,
        CompletedStepId = completedStep.Id
      });
      this.requestContext.Trace(1976426, TraceLevel.Info, "ReleaseManagementService", "Pipeline", "OSPV2: QueueMoveToNextStepJob: jobId: {0}, jobData: {1}", (object) this.requestContext.GetService<TeamFoundationJobService>().QueueOneTimeJob(this.requestContext, "CreateNextStep", "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.CreateNextStepJob", xml, true), (object) xml.OuterXml);
    }

    private void QueueMoveworkflowForwardJob(
      IEnumerable<ReleaseEnvironmentStep> updatedReleaseEnvironmentSteps)
    {
      this.TraceInformationMessage(1960066, "OSPV2: AcceptStepImplementation: Creating job for moving workflow forward. CurrentReleaseStepId(s): {0}", (object) OrchestratorServiceProcessorV2.JoinStepIds(updatedReleaseEnvironmentSteps));
      IEnumerable<XmlNode> xmlNodes = updatedReleaseEnvironmentSteps.Select<ReleaseEnvironmentStep, DeploymentApprovalJobData>((Func<ReleaseEnvironmentStep, DeploymentApprovalJobData>) (s => DeploymentApprovalJobData.GetDeploymentApprovalJobData(this.projectId, s.ReleaseId, s.Id))).Select<DeploymentApprovalJobData, XmlNode>((Func<DeploymentApprovalJobData, XmlNode>) (Jobdata => TeamFoundationSerializationUtility.SerializeToXml((object) Jobdata)));
      ITeamFoundationJobService jobService = ReleaseOperationHelper.GetJobService(this.requestContext.Elevate());
      List<TeamFoundationJobDefinition> foundationJobDefinitionList = new List<TeamFoundationJobDefinition>();
      foreach (XmlNode data in xmlNodes)
      {
        TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(Guid.NewGuid(), "OnDeploymentApproval", "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.DeploymentApprovalJob", data, TeamFoundationJobEnabledState.Enabled, false, JobPriorityClass.High);
        foundationJobDefinitionList.Add(foundationJobDefinition);
      }
      IEnumerable<TeamFoundationJobReference> jobReferences = foundationJobDefinitionList.Select<TeamFoundationJobDefinition, TeamFoundationJobReference>((Func<TeamFoundationJobDefinition, TeamFoundationJobReference>) (job => job.ToJobReference()));
      jobService.UpdateJobDefinitions(this.requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) foundationJobDefinitionList);
      jobService.QueueJobs(this.requestContext, jobReferences, JobPriorityLevel.Highest, (int) TimeSpan.Zero.TotalSeconds, false);
      this.TraceInformationMessage(1971707, "OSPV2: AcceptStepImplementation: Created job for moving workflow forward. CurrentReleaseStepId(s): {0}, JobId(s): {1}", (object) OrchestratorServiceProcessorV2.JoinStepIds(updatedReleaseEnvironmentSteps), (object) string.Join<Guid>(",", foundationJobDefinitionList.Select<TeamFoundationJobDefinition, Guid>((Func<TeamFoundationJobDefinition, Guid>) (Job => Job.JobId))));
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Needed for complexity")]
    public IEnumerable<ReleaseEnvironmentStep> AcceptStepImplementation(
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> releases,
      IList<ReleaseEnvironmentStep> releaseEnvironmentSteps,
      Func<IEnumerable<ReleaseEnvironmentStep>, IEnumerable<ReleaseEnvironmentStep>> updatePendingReleaseStep,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, ReleaseDefinitionEnvironmentsSnapshot, DefinitionEnvironmentData, ReleaseEnvironmentStep> moveWorkflowForward)
    {
      this.TraceInformationMessage(1960063, "OSPV2: AcceptStepImplementation: Enter. ReleaseId(s): {0}, ReleaseStepId(s): {1}", (object) OrchestratorServiceProcessorV2.JoinReleaseIds((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) releases), (object) OrchestratorServiceProcessorV2.JoinStepIds((IEnumerable<ReleaseEnvironmentStep>) releaseEnvironmentSteps));
      Dictionary<int, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> stepReleaseMap = OrchestratorServiceProcessorV2.GetStepReleaseMap((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) releases, (IEnumerable<ReleaseEnvironmentStep>) releaseEnvironmentSteps);
      this.ThrowIfApprovalUpdateIsNotAllowed((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) releases, (IEnumerable<ReleaseEnvironmentStep>) releaseEnvironmentSteps, stepReleaseMap);
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps1 = updatePendingReleaseStep((IEnumerable<ReleaseEnvironmentStep>) releaseEnvironmentSteps);
      IEnumerable<ReleaseEnvironmentStep> updatedSteps = this.GetUpdatedSteps((IEnumerable<ReleaseEnvironmentStep>) releaseEnvironmentSteps, releaseEnvironmentSteps1, 1960066);
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> activeReleases = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>();
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> releaseList = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>();
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) releases)
      {
        if (release.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Active)
          activeReleases.Add(release);
        else
          releaseList.Add(release);
      }
      this.TraceInformationMessage(1960066, "OSPV2: AcceptStepImplementation: Active release(s): {0}, InActiveRelease(s): {1}", (object) OrchestratorServiceProcessorV2.JoinReleaseIds((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) activeReleases), (object) OrchestratorServiceProcessorV2.JoinReleaseIds((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) releaseList));
      if (!activeReleases.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>())
        return releaseEnvironmentSteps1;
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps2 = updatedSteps.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => activeReleases.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, bool>) (r => r.Id == s.ReleaseId))));
      ReleaseEnvironmentStep updatedReleaseEnvironmentStep = releaseEnvironmentSteps2.First<ReleaseEnvironmentStep>();
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1 = stepReleaseMap[updatedReleaseEnvironmentStep.Id];
      if (releaseEnvironmentSteps2.Count<ReleaseEnvironmentStep>() == 1 && this.IsStepAutomated(release1, updatedReleaseEnvironmentStep))
      {
        ReleaseEnvironmentStep currentReleaseStep = releaseEnvironmentSteps1.First<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.Id == updatedReleaseEnvironmentStep.Id));
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment = release1.GetEnvironment(updatedReleaseEnvironmentStep.ReleaseEnvironmentId);
        DefinitionEnvironmentData definitionEnvironmentData = release1.GetDefinitionEnvironmentData(environment.Name);
        this.TraceInformationMessage(1960066, "OSPV2: AcceptStepImplementation: Moving workflow forward. ReleaseId: {0}, CurrentEnvironmentId: {1}, CurrentReleaseStepId: {2}", (object) release1.Id, (object) definitionEnvironmentData.Id, (object) currentReleaseStep.Id);
        if (currentReleaseStep.IsApprovalStep())
          this.MarkDeploymentOperationStatusAsApproved(environment, currentReleaseStep);
        moveWorkflowForward(release1, release1.DefinitionSnapshot, definitionEnvironmentData, currentReleaseStep);
      }
      else
      {
        this.QueueMoveworkflowForwardJob(releaseEnvironmentSteps2);
        this.SendReleaseUpdateEvent(releaseEnvironmentSteps2);
      }
      foreach (ReleaseEnvironmentStep approvalStep in releaseEnvironmentSteps2)
        this.FireApprovalCompletionEvent(stepReleaseMap[approvalStep.Id], approvalStep);
      return releaseEnvironmentSteps1.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => activeReleases.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, bool>) (r => r.Id == s.ReleaseId))));
    }

    public void TriggerMoveWorkflowForward(int releaseId, int approvalStepId) => this.TriggerMoveWorkflowForwardImplementation(releaseId, approvalStepId, new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, ReleaseDefinitionEnvironmentsSnapshot, DefinitionEnvironmentData, ReleaseEnvironmentStep>(this.MoveWorkflowForward));

    public void TriggerMoveWorkflowForwardImplementation(
      int releaseId,
      int approvalStepId,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, ReleaseDefinitionEnvironmentsSnapshot, DefinitionEnvironmentData, ReleaseEnvironmentStep> moveWorkflowForward)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1 = this.dataAccessLayer.GetRelease(releaseId);
      ReleaseEnvironmentStep step = release1.GetStep(approvalStepId);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment = release1.GetEnvironment(step.ReleaseEnvironmentId);
      bool areApprovalsParallel = release1.HasParallelApprovals(step.DefinitionEnvironmentId, step.Rank);
      if ((!this.ReleaseShouldBeActive(release1, approvalStepId) || !this.ApprovalShouldBeCompleted(step) || !this.EnvironmentShouldBeInProgress(environment, release1, step) ? 0 : (this.AreParallelApprovalsHandled(release1, step, environment, areApprovalsParallel) ? 1 : 0)) == 0)
        return;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release2 = this.CleanupApprovalsData(release1, environment, step, areApprovalsParallel);
      DefinitionEnvironmentData definitionEnvironmentData = release2.GetDefinitionEnvironmentData(environment.Name);
      if (step.IsApprovalStep())
        this.MarkDeploymentOperationStatusAsApproved(environment, step);
      this.TraceInformationMessage(1972117, "OSPV2: MoveWorkFlowForward: Moving workflow forward. ReleaseId: {0}, CurrentEnvironmentId: {1}, CurrentReleaseStepId: {2}", (object) release2.Id, (object) definitionEnvironmentData.Id, (object) step.Id);
      moveWorkflowForward(release2, release2.DefinitionSnapshot, definitionEnvironmentData, step);
      this.TraceInformationMessage(1972117, "OSPV2: MoveWorkFlowForward: Returning from MoveWorkFlowForward. ReleaseId: {0}, CurrentEnvironmentId: {1}, CurrentReleaseStepId: {2}", (object) release2.Id, (object) definitionEnvironmentData.Id, (object) step.Id);
    }

    private void SendReleaseUpdateEvent(IEnumerable<ReleaseEnvironmentStep> approvalSteps)
    {
      List<int> intList = new List<int>();
      foreach (ReleaseEnvironmentStep approvalStep in approvalSteps)
      {
        ReleaseEnvironmentStep step = approvalStep;
        if (!intList.Exists((Predicate<int>) (r => r == step.ReleaseId)))
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = this.dataAccessLayer.GetRelease(step.ReleaseId);
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment = release.GetEnvironment(step.ReleaseEnvironmentId);
          IEnumerable<ReleaseEnvironmentStep> releaseSteps = environment.GetApprovalSteps(step.StepType, step.TrialNumber).Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.Rank == step.Rank));
          if (!OrchestratorServiceProcessorV2.CanMoveWorkflowForward(release, step, releaseSteps))
          {
            int id = release.Id;
            this.dataAccessLayer.SendReleaseEnvironmentUpdatedEvent(step.ReleaseDefinitionId, id, step.ReleaseEnvironmentId);
            Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment latestDeployment = environment?.GetLatestDeployment();
            if (latestDeployment != null)
              this.requestContext.SendReleaseEnvironmentStatusUpdatedNotification(this.projectId, release.ReleaseDefinitionId, release.Id, step.ReleaseEnvironmentId, environment.Status.ToWebApi(), latestDeployment.Status.ToWebApi(), latestDeployment.OperationStatus.ToWebApi());
            intList.Add(release.Id);
          }
        }
      }
    }

    private IEnumerable<ReleaseEnvironmentStep> GetUpdatedSteps(
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps,
      IEnumerable<ReleaseEnvironmentStep> updatedReleaseStepWithHistorySteps,
      int tracePoint)
    {
      List<ReleaseEnvironmentStep> steps = new List<ReleaseEnvironmentStep>();
      List<ReleaseEnvironmentStep> updatedSteps = new List<ReleaseEnvironmentStep>();
      foreach (ReleaseEnvironmentStep releaseEnvironmentStep1 in releaseEnvironmentSteps)
      {
        ReleaseEnvironmentStep step = releaseEnvironmentStep1;
        ReleaseEnvironmentStep releaseEnvironmentStep2 = updatedReleaseStepWithHistorySteps.FirstOrDefault<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.Id == step.Id));
        if (releaseEnvironmentStep2 == null)
          steps.Add(step);
        else
          updatedSteps.Add(releaseEnvironmentStep2);
      }
      this.TraceInformationMessage(tracePoint, "OSPV2: Current updated step(s): {0}, Already updated step(s): {1}", (object) OrchestratorServiceProcessorV2.JoinStepIds((IEnumerable<ReleaseEnvironmentStep>) updatedSteps), (object) OrchestratorServiceProcessorV2.JoinStepIds((IEnumerable<ReleaseEnvironmentStep>) steps));
      if (updatedSteps.Any<ReleaseEnvironmentStep>())
        return (IEnumerable<ReleaseEnvironmentStep>) updatedSteps;
      ReleaseEnvironmentStep releaseEnvironmentStep = releaseEnvironmentSteps.First<ReleaseEnvironmentStep>();
      if (releaseEnvironmentStep.StepType == EnvironmentStepType.Deploy)
        throw new DeployStepUpdateException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.InvalidDeployStepUpdate, (object) releaseEnvironmentStep.Id, (object) releaseEnvironmentStep.ReleaseId, (object) releaseEnvironmentStep.Status));
      throw new ApprovalUpdateException(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.InvalidApprovalUpdate);
    }

    private void ThrowIfApprovalUpdateIsNotAllowed(
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> releases,
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps,
      Dictionary<int, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> stepReleaseMap)
    {
      IEnumerable<ReleaseEnvironmentStep> source = releaseEnvironmentSteps.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => !this.CheckApproverAllowed(stepReleaseMap[step.Id], step)));
      if (source.Any<ReleaseEnvironmentStep>())
      {
        IEnumerable<int> environmentIds = source.Select<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (step => step.ReleaseEnvironmentId));
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.NotValidReleaseApprover, (object) string.Join(",", releases.SelectMany<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>>) (e => (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>) e.Environments)).Where<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (e => environmentIds.Contains<int>(e.Id))).Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, string>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, string>) (e => e.Name)).Distinct<string>())));
      }
    }

    private bool IsStepAutomated(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release, ReleaseEnvironmentStep releaseEnvironmentStep)
    {
      bool flag = release.DefinitionSnapshot.Environments.First<DefinitionEnvironmentData>((Func<DefinitionEnvironmentData, bool>) (s => s.Id == releaseEnvironmentStep.DefinitionEnvironmentId)).Steps.Where<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (s => s.Rank == releaseEnvironmentStep.Rank)).All<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (s => s.IsAutomated));
      if (!releaseEnvironmentStep.IsAutomated)
        return false;
      return flag || OrchestratorServiceProcessorV2.IsApprovalPolicyFeatureRequirementMet(this.requestContext, release, releaseEnvironmentStep.ReleaseEnvironmentId, releaseEnvironmentStep.TrialNumber);
    }

    private void MarkDeploymentOperationStatusAsApproved(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      ReleaseEnvironmentStep currentReleaseStep)
    {
      if (!currentReleaseStep.IsApprovalStep() || currentReleaseStep.StepType == EnvironmentStepType.PreDeploy && releaseEnvironment.HasDeferredDeployment())
        return;
      this.dataAccessLayer.UpdateDeploymentOperationStatus(currentReleaseStep.ReleaseId, currentReleaseStep.ReleaseEnvironmentId, currentReleaseStep.TrialNumber, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Approved);
      this.requestContext.SendReleaseEnvironmentStatusUpdatedNotification(releaseEnvironment.ProjectId, releaseEnvironment.ReleaseId, releaseEnvironment.ReleaseId, releaseEnvironment.Id, releaseEnvironment.Status.ToWebApi(), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.InProgress.ToWebApi(), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Approved.ToWebApi());
    }

    private Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release MarkStepsAsSkippedIfRequired(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      ReleaseEnvironmentStep currentReleaseStep,
      bool areApprovalsParallel,
      int tracePoint)
    {
      if (!areApprovalsParallel)
        return release;
      try
      {
        ApprovalOptions releaseEnvironment = OrchestratorServiceProcessorV2.GetApprovalOptionsFromReleaseEnvironment(release, currentReleaseStep.ReleaseEnvironmentId, currentReleaseStep.StepType);
        int totalNumberOfApprovers = release.NumberOfApprovalsWithSameRank(currentReleaseStep.DefinitionEnvironmentId, currentReleaseStep.Rank);
        if (releaseEnvironment != null)
        {
          if (!releaseEnvironment.AreAllApprovalsRequired(totalNumberOfApprovers))
          {
            string comment = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ApprovalStatusSkippedComment, (object) releaseEnvironment.ReqApproverCount);
            return this.dataAccessLayer.UpdateApprovalStepsStatus(release.Id, currentReleaseStep.ReleaseEnvironmentId, currentReleaseStep.StepType, ReleaseEnvironmentStepStatus.Pending, ReleaseEnvironmentStepStatus.Skipped, comment);
          }
        }
      }
      catch (ApprovalUpdateException ex)
      {
        this.TraceInformationMessage(tracePoint, "OSPV2: MarkStepsAsSkipped: {0}", (object) "Already approvals are marked as skipped.");
      }
      return release;
    }

    private bool IsMoveWorkflowForwardAllowed(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      ReleaseEnvironmentStep currentReleaseStep,
      bool areApprovalsParallel)
    {
      bool flag;
      if (areApprovalsParallel)
      {
        IEnumerable<ReleaseEnvironmentStep> releaseSteps = this.dataAccessLayer.GetReleaseSteps(release.Id, currentReleaseStep.ReleaseEnvironmentId, currentReleaseStep.Rank, currentReleaseStep.TrialNumber);
        flag = OrchestratorServiceProcessorV2.CanMoveWorkflowForward(release, currentReleaseStep, releaseSteps);
      }
      else
        flag = true;
      return flag;
    }

    public IEnumerable<ReleaseEnvironmentStep> ReassignApprovalRequest(
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps)
    {
      return this.ReassignApprovalRequestImplementation(releaseEnvironmentSteps, new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, ReleaseEnvironmentStep, int>(this.FireApprovalPendingEvent));
    }

    public IEnumerable<ReleaseEnvironmentStep> ReassignApprovalRequestImplementation(
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, ReleaseEnvironmentStep, int> fireApprovalPendingEvent)
    {
      if (releaseEnvironmentSteps == null)
        throw new ArgumentNullException(nameof (releaseEnvironmentSteps));
      if (fireApprovalPendingEvent == null)
        throw new ArgumentNullException(nameof (fireApprovalPendingEvent));
      this.TraceInformationMessage(1960016, "OSPV2: ReassignApprovalRequestImplementation: Enter. ReleaseId(s): {0}, ReleaseEnvironmentStepId(s): {1}", (object) OrchestratorServiceProcessorV2.JoinStepReleaseIds(releaseEnvironmentSteps), (object) OrchestratorServiceProcessorV2.JoinStepIds(releaseEnvironmentSteps));
      IEnumerable<ReleaseEnvironmentStep> updatedReleaseStepWithHistorySteps = this.dataAccessLayer.UpdateReleaseEnvironmentSteps(releaseEnvironmentSteps);
      foreach (ReleaseEnvironmentStep updatedStep in this.GetUpdatedSteps(releaseEnvironmentSteps, updatedReleaseStepWithHistorySteps, 1960019))
        fireApprovalPendingEvent((Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release) null, updatedStep, updatedStep.ReleaseId);
      return updatedReleaseStepWithHistorySteps;
    }

    public IEnumerable<ReleaseEnvironmentStep> RejectStep(
      ReleaseEnvironmentStep releaseEnvironmentStep,
      Guid rejectedBy,
      string comment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (releaseEnvironmentStep == null)
        throw new ArgumentNullException(nameof (releaseEnvironmentStep));
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      releaseEnvironmentStep.ApproverComment = comment;
      releaseEnvironmentStep.ActualApproverId = rejectedBy;
      releaseEnvironmentStep.Status = ReleaseEnvironmentStepStatus.Rejected;
      return this.RejectStep((IEnumerable<ReleaseEnvironmentStep>) new List<ReleaseEnvironmentStep>()
      {
        releaseEnvironmentStep
      }, (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>()
      {
        release
      });
    }

    public IEnumerable<ReleaseEnvironmentStep> RejectStep(
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps,
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> releases)
    {
      if (releaseEnvironmentSteps == null)
        throw new ArgumentNullException(nameof (releaseEnvironmentSteps));
      if (releases == null)
        throw new ArgumentNullException(nameof (releases));
      IEnumerable<ReleaseEnvironmentStep> source = releaseEnvironmentSteps.Any<ReleaseEnvironmentStep>() && !releaseEnvironmentSteps.Any<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s == null)) ? this.RejectStepImplementation(releases, releaseEnvironmentSteps, new Func<IEnumerable<ReleaseEnvironmentStep>, IEnumerable<ReleaseEnvironmentStep>>(this.UpdatePendingReleaseStep), new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int, EnvironmentStatus, string>(this.FireEnvironmentCompletionEvent)) : throw new InvalidDataException();
      ReleaseOperationHelper.DeleteJobDefinition(this.requestContext, source.Select<ReleaseEnvironmentStep, Guid>((Func<ReleaseEnvironmentStep, Guid>) (s => s.ApprovalTimeoutJobId)).Distinct<Guid>(), releases.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int>) (r => r.Id)).Distinct<int>());
      return source;
    }

    public void RejectGreenlightingStep(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int releaseEnvironmentId,
      int releaseStepId)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = release != null ? release.GetEnvironment(releaseEnvironmentId) : throw new ArgumentNullException(nameof (release));
      if (releaseEnvironment.Status == ReleaseEnvironmentStatus.Canceled)
        return;
      ReleaseEnvironmentStep releaseEnvironmentStep = releaseEnvironment.GetStepsForTests.Single<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.Id == releaseStepId));
      int num;
      if (releaseEnvironmentStep.StepType != EnvironmentStepType.PreGate)
      {
        ApprovalOptions postApprovalOptions = releaseEnvironment.PostApprovalOptions;
        num = postApprovalOptions != null ? (int) postApprovalOptions.ExecutionOrder : 1;
      }
      else
      {
        ApprovalOptions preApprovalOptions = releaseEnvironment.PreApprovalOptions;
        num = preApprovalOptions != null ? (int) preApprovalOptions.ExecutionOrder : 1;
      }
      if (num == 4)
      {
        releaseEnvironmentStep.Status = ReleaseEnvironmentStepStatus.Rejected;
        this.UpdatePendingReleaseStep((IEnumerable<ReleaseEnvironmentStep>) new List<ReleaseEnvironmentStep>()
        {
          releaseEnvironmentStep
        });
        this.MoveToNextReleaseStep(release, release.GetDefinitionEnvironmentData(releaseEnvironmentStep.DefinitionEnvironmentId), releaseEnvironmentStep.Rank, releaseEnvironmentStep.TrialNumber);
      }
      else
      {
        Guid userId = this.requestContext.GetUserId(true);
        this.RejectStep(releaseEnvironmentStep, userId, string.Empty, release);
      }
    }

    public void RejectDeployStep(int releaseId, int deployStepId)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = this.dataAccessLayer.GetRelease(releaseId);
      this.RejectStep(releaseId, deployStepId, release);
    }

    public IEnumerable<ReleaseEnvironmentStep> RejectStep(
      int releaseId,
      int releaseEnvironmentStepId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      ReleaseEnvironmentStep updatedReleaseEnvironmentStep;
      if (this.SendDeploymentCancelledNotificationIfRequired(release, releaseEnvironmentStepId, out updatedReleaseEnvironmentStep))
        return (IEnumerable<ReleaseEnvironmentStep>) null;
      try
      {
        Guid userId = this.requestContext.GetUserId(true);
        return this.RejectStep(updatedReleaseEnvironmentStep, userId, string.Empty, release);
      }
      catch (DeployStepUpdateException ex1)
      {
        try
        {
          release = this.dataAccessLayer.GetRelease(release.Id);
          if (this.SendDeploymentCancelledNotificationIfRequired(release, releaseEnvironmentStepId, out updatedReleaseEnvironmentStep))
            return (IEnumerable<ReleaseEnvironmentStep>) null;
        }
        catch (ReleaseNotFoundException ex2)
        {
          return (IEnumerable<ReleaseEnvironmentStep>) null;
        }
        this.TraceInformationMessage(1960068, "OSPV2: RejectStepImplementation from DT: Failed with exception {0} for release id : {1}, stepId {2}", (object) ex1.Message, (object) release.Id, (object) updatedReleaseEnvironmentStep.Id);
        return (IEnumerable<ReleaseEnvironmentStep>) null;
      }
      catch (Exception ex)
      {
        updatedReleaseEnvironmentStep.Logs += TeamFoundationExceptionFormatter.FormatException(ex, false);
        this.dataAccessLayer.UpdateReleaseStep(updatedReleaseEnvironmentStep);
        throw;
      }
    }

    private bool SendDeploymentCancelledNotificationIfRequired(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int releaseEnvironmentStepId,
      out ReleaseEnvironmentStep updatedReleaseEnvironmentStep)
    {
      ReleaseEnvironmentStep releaseEnvironmentStep = release.GetStep(releaseEnvironmentStepId);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment = release.GetEnvironment(releaseEnvironmentStep.ReleaseEnvironmentId).DeploymentAttempts.SingleOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment, bool>) (d => d.Attempt == releaseEnvironmentStep.TrialNumber));
      updatedReleaseEnvironmentStep = releaseEnvironmentStep;
      if (deployment == null || deployment.Status != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.Failed && deployment.OperationStatus != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Canceled)
        return false;
      this.TraceInformationMessage(1960090, "OSPV2: hasDeploymentCancelled: Received Accept/RejectStep signal from DT for canceled environment. Environment Id {0}, ReleaseId : {1}", (object) deployment.ReleaseEnvironmentId, (object) release.Id);
      this.SendReleaseEnvironmentUpdatedEvent(release.ReleaseDefinitionId, release.Id, releaseEnvironmentStep.ReleaseEnvironmentId);
      return true;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Will refactor")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Needed for complexity")]
    public IEnumerable<ReleaseEnvironmentStep> RejectStepImplementation(
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> releases,
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps,
      Func<IEnumerable<ReleaseEnvironmentStep>, IEnumerable<ReleaseEnvironmentStep>> updatePendingReleaseStep,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int, EnvironmentStatus, string> fireEnvironmentCompletionEvent)
    {
      if (releases == null)
        throw new ArgumentNullException(nameof (releases));
      if (releaseEnvironmentSteps == null)
        throw new ArgumentNullException(nameof (releaseEnvironmentSteps));
      if (updatePendingReleaseStep == null)
        throw new ArgumentNullException(nameof (updatePendingReleaseStep));
      if (fireEnvironmentCompletionEvent == null)
        throw new ArgumentNullException(nameof (fireEnvironmentCompletionEvent));
      this.TraceInformationMessage(1960063, "OSPV2: AcceptStepImplementation: Enter. ReleaseId(s): {0}, ReleaseStepId(s): {1}", (object) OrchestratorServiceProcessorV2.JoinStepReleaseIds(releaseEnvironmentSteps), (object) OrchestratorServiceProcessorV2.JoinStepIds(releaseEnvironmentSteps));
      Dictionary<int, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> stepReleaseMap = OrchestratorServiceProcessorV2.GetStepReleaseMap((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) releases, releaseEnvironmentSteps);
      this.ThrowIfApprovalUpdateIsNotAllowed((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) releases, releaseEnvironmentSteps, stepReleaseMap);
      IEnumerable<ReleaseEnvironmentStep> updatedReleaseStepWithHistorySteps = updatePendingReleaseStep(releaseEnvironmentSteps);
      IEnumerable<ReleaseEnvironmentStep> updatedSteps = this.GetUpdatedSteps(releaseEnvironmentSteps, updatedReleaseStepWithHistorySteps, 1960071);
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps1 = updatedSteps.GroupBy<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (s => s.ReleaseEnvironmentId)).Select<IGrouping<int, ReleaseEnvironmentStep>, ReleaseEnvironmentStep>((Func<IGrouping<int, ReleaseEnvironmentStep>, ReleaseEnvironmentStep>) (g => g.First<ReleaseEnvironmentStep>()));
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> source = this.dataAccessLayer.RejectReleaseEnvironments(releaseEnvironmentSteps1);
      if (source == null || !source.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>())
      {
        this.TraceInformationMessage(1960071, "OSPV2: RejectStepImplementation: Already releaseenvironment rejected for release. CurrentReleaseEnvironmentId(s): {0}", (object) string.Join<int>(",", releaseEnvironmentSteps1.Select<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (s => s.ReleaseEnvironmentId))));
        return updatedReleaseStepWithHistorySteps;
      }
      foreach (int key in stepReleaseMap.Keys.ToList<int>())
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release oldRelease = stepReleaseMap[key];
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release newRelease = source.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, bool>) (release => release.Id == oldRelease.Id));
        this.UpdateVariables(oldRelease, newRelease);
        OrchestratorServiceProcessorV2.UpdateDefinitionEnvironmentData(oldRelease, newRelease);
        stepReleaseMap[key] = newRelease ?? oldRelease;
      }
      foreach (ReleaseEnvironmentStep approvalStep in updatedSteps)
        this.FireApprovalCompletionEvent(stepReleaseMap[approvalStep.Id], approvalStep);
      foreach (ReleaseEnvironmentStep releaseEnvironmentStep in releaseEnvironmentSteps1)
        fireEnvironmentCompletionEvent(stepReleaseMap[releaseEnvironmentStep.Id], releaseEnvironmentStep.ReleaseEnvironmentId, EnvironmentStatus.Rejected, releaseEnvironmentStep.ApproverComment);
      return updatedReleaseStepWithHistorySteps;
    }

    private IEnumerable<ReleaseEnvironmentStep> UpdatePendingReleaseStep(
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps)
    {
      if (releaseEnvironmentSteps == null)
        throw new ArgumentNullException(nameof (releaseEnvironmentSteps));
      this.TraceInformationMessage(1960072, "OSPV2: UpdatePendingReleaseStep: Updating pending release step. ReleaseStepId(s): {0}", (object) OrchestratorServiceProcessorV2.JoinStepIds(releaseEnvironmentSteps));
      return this.dataAccessLayer.UpdateReleaseEnvironmentSteps(releaseEnvironmentSteps);
    }

    private void MoveWorkflowForward(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      ReleaseDefinitionEnvironmentsSnapshot releaseDefinition,
      DefinitionEnvironmentData currentStage,
      ReleaseEnvironmentStep currentReleaseStep)
    {
      this.MoveWorkflowForwardImplementation(release, currentStage, currentReleaseStep, new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, DefinitionEnvironmentData, int, int>(this.MoveToNextReleaseStep), new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, DefinitionEnvironmentData, ReleaseEnvironmentStep>(this.QueueMoveToNextStepJob), new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int, EnvironmentStatus, string>(this.FireEnvironmentCompletionEvent), new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int, int>(this.EvaluateConditionsAndMoveToNextEnvironment));
    }

    public void MoveWorkflowForwardImplementation(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      DefinitionEnvironmentData definitionEnvironmentData,
      ReleaseEnvironmentStep releaseEnvironmentStep,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, DefinitionEnvironmentData, int, int> moveToNextReleaseStep,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, DefinitionEnvironmentData, ReleaseEnvironmentStep> queueMoveToNextStepJob,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int, EnvironmentStatus, string> fireEnvironmentCompletionEvent,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, int, int> evaluateConditionsAndMoveToNextEnvironment)
    {
      this.TraceInformationMessage(1960074, "OSPV2: MoveWorkflowForwardImplementation: Enter. ReleaseId: {0}, ReleaseStepId: {1}", (object) release.Id, (object) releaseEnvironmentStep.Id);
      if (definitionEnvironmentData.Steps.Max<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, int>) (s => s.Rank)) != releaseEnvironmentStep.Rank)
      {
        if (!releaseEnvironmentStep.IsApprovalStep())
          queueMoveToNextStepJob(release, definitionEnvironmentData, releaseEnvironmentStep);
        else
          moveToNextReleaseStep(release, definitionEnvironmentData, releaseEnvironmentStep.Rank, releaseEnvironmentStep.TrialNumber);
      }
      else
      {
        bool areApprovalsParallel = release.HasParallelApprovals(releaseEnvironmentStep.DefinitionEnvironmentId, releaseEnvironmentStep.Rank);
        try
        {
          int num = release.Environments.First<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (env => env.DefinitionEnvironmentId == definitionEnvironmentData.Id)).ReleaseDeployPhases.Where<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (s => s.Attempt == releaseEnvironmentStep.TrialNumber)).Any<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (s => s.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseStatus.PartiallySucceeded)) ? 1 : 0;
          ReleaseEnvironmentStatus environmentStatus = num != 0 ? ReleaseEnvironmentStatus.PartiallySucceeded : ReleaseEnvironmentStatus.Succeeded;
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus deploymentStatus = num != 0 ? Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.PartiallySucceeded : Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.Succeeded;
          release = this.dataAccessLayer.UpdateEnvironmentAndDeploymentStatus(release, releaseEnvironmentStep.ReleaseEnvironmentId, releaseEnvironmentStep.TrialNumber, environmentStatus, deploymentStatus, (ReleaseEnvironmentStatusChangeDetails) null, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Undefined);
          fireEnvironmentCompletionEvent(release, releaseEnvironmentStep.ReleaseEnvironmentId, environmentStatus.ToWebApi(), string.Empty);
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1 = this.MarkStepsAsSkippedIfRequired(release, releaseEnvironmentStep, areApprovalsParallel, 1960077);
          evaluateConditionsAndMoveToNextEnvironment(release1, release.Id, releaseEnvironmentStep.ReleaseEnvironmentId);
        }
        catch (InvalidReleaseEnvironmentStatusUpdateException ex)
        {
          if (areApprovalsParallel)
            this.TraceInformationMessage(1960077, "OSPV2: MoveWorkflowForwardImplementation: {0}", (object) "Duplicate environment status update due to parallel approvals.");
          else
            throw;
        }
      }
    }

    public virtual void SendReleaseEnvironmentUpdatedEvent(
      int definitionId,
      int releaseId,
      int releaseEnvironmentId)
    {
      this.requestContext.SendReleaseEnvironmentUpdatedEvent(this.projectId, definitionId, releaseId, releaseEnvironmentId);
    }

    private void CreateNextReleaseStep(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      DefinitionEnvironmentData definitionEnvironmentData,
      IEnumerable<DefinitionEnvironmentStepData> definitionEnvironmentStepsData,
      int environmentRank,
      int trialNumber)
    {
      if (definitionEnvironmentStepsData == null || definitionEnvironmentStepsData.Count<DefinitionEnvironmentStepData>() == 0)
        throw new ArgumentNullException(nameof (definitionEnvironmentStepsData));
      this.TraceInformationMessage(1960048, "OSPV2: CreateNextReleaseStep: Enter. ReleaseId: {0}, DefinitionEnvironmentDataId: {1}", (object) release.Id, (object) definitionEnvironmentData.Id);
      try
      {
        foreach (ReleaseEnvironmentStep releaseEnvironmentStep in this.CreateReleaseEnvironmentSteps(release, definitionEnvironmentStepsData, environmentRank, trialNumber))
          this.ExecuteReleaseStep(release, releaseEnvironmentStep);
      }
      catch (DeploymentUpdateNotAllowedException ex)
      {
        this.TraceInformationMessage(1960047, "OSPV2: CreateReleaseStep: {0}", (object) ex.Message);
        throw;
      }
    }

    private IEnumerable<ReleaseEnvironmentStep> CreateReleaseEnvironmentSteps(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      IEnumerable<DefinitionEnvironmentStepData> definitionEnvironmentStepsData,
      int environmentRank,
      int trialNumber)
    {
      List<ReleaseEnvironmentStep> source1 = new List<ReleaseEnvironmentStep>();
      IEnumerable<ReleaseEnvironmentStep> source2 = (IEnumerable<ReleaseEnvironmentStep>) new List<ReleaseEnvironmentStep>();
      foreach (DefinitionEnvironmentStepData stepData in definitionEnvironmentStepsData)
      {
        DefinitionEnvironmentStep definitionEnvironmentStep = stepData.ToDefinitionEnvironmentStep();
        ReleaseEnvironmentStep releaseEnvironmentStep = ReleaseEnvironmentStepBuilder.Build(release, definitionEnvironmentStep, environmentRank, trialNumber);
        source1.Add(releaseEnvironmentStep);
      }
      ReleaseEnvironmentStep releaseEnvironmentStep1 = source1.First<ReleaseEnvironmentStep>();
      this.TraceInformationMessage(1960046, "OSPV2: CreateReleaseStep: Before adding release steps. ReleaseId: {0}, DefinitionEnvironmentId: {1}, DefinitionEnvironmentStepType: {2}, DefinitionEnvironmentStepRank: {3}", (object) release.Id, (object) releaseEnvironmentStep1.DefinitionEnvironmentId, (object) releaseEnvironmentStep1.StepType, (object) releaseEnvironmentStep1.Rank);
      bool handleParallelApprovals = false;
      try
      {
        if (!releaseEnvironmentStep1.IsApprovalStep())
          handleParallelApprovals = release.HasParallelApprovals(releaseEnvironmentStep1.DefinitionEnvironmentId, releaseEnvironmentStep1.Rank - 1);
        source2 = this.dataAccessLayer.AddReleaseSteps(release, (IEnumerable<ReleaseEnvironmentStep>) source1, handleParallelApprovals);
        if (source2 != null)
        {
          if (source2.Any<ReleaseEnvironmentStep>())
          {
            ReleaseEnvironmentStep firstEnvironmentStep = source2.First<ReleaseEnvironmentStep>();
            if (firstEnvironmentStep.IsApprovalStep())
            {
              if (!firstEnvironmentStep.ApprovalTimeoutJobId.Equals(Guid.Empty))
              {
                if (!OrchestratorServiceProcessorV2.HasMatchingSteps(release, firstEnvironmentStep.DefinitionEnvironmentId, firstEnvironmentStep.StepType, firstEnvironmentStep.Rank - 1))
                  this.CreateApprovalTimeoutJob(release, firstEnvironmentStep);
              }
            }
          }
        }
      }
      catch (DuplicateStepsInsertionException ex)
      {
        if (handleParallelApprovals)
          this.TraceInformationMessage(1960047, "OSPV2: CreateReleaseStep: {0}", (object) ex.Message);
        else
          throw;
      }
      return source2;
    }

    private void CreateApprovalTimeoutJob(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      ReleaseEnvironmentStep firstEnvironmentStep)
    {
      ApprovalOptions releaseEnvironment = OrchestratorServiceProcessorV2.GetApprovalOptionsFromReleaseEnvironment(release, firstEnvironmentStep.ReleaseEnvironmentId, firstEnvironmentStep.StepType);
      int num = releaseEnvironment == null ? ApprovalOptions.ApprovalDefaultTimeoutInMinutes : releaseEnvironment.GetApprovalTimeout();
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) DeploymentApprovalTimeoutJobData.GetDeploymentApprovalTimeoutJobData(this.projectId, release.Id, firstEnvironmentStep.Id));
      TeamFoundationJobSchedule foundationJobSchedule = new TeamFoundationJobSchedule()
      {
        PriorityLevel = JobPriorityLevel.Normal,
        ScheduledTime = DateTime.UtcNow.AddMinutes(TimeSpan.FromMinutes((double) num).TotalMinutes),
        Interval = 0
      };
      ReleaseOperationHelper.CreateJob(this.requestContext, new TeamFoundationJobDefinition(firstEnvironmentStep.ApprovalTimeoutJobId, "OnDeploymentApprovalTimeout", "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.DeploymentApprovalTimeoutHandlerJob", xml)
      {
        Schedule = {
          foundationJobSchedule
        }
      });
    }

    private void EvaluateConditionsAndStartRelease(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release) => new ReleaseConditionEvaluator(this.requestContext, this.projectId).StartRelease(release);

    private void EvaluateConditionsAndMoveToNextEnvironment(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int releaseId,
      int completedEnvironmentId)
    {
      new ReleaseConditionEvaluator(this.requestContext, this.projectId).EvaluateConditionsOnEnvironmentCompletion(release, completedEnvironmentId);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private void TraceInformationMessage(int tracePoint, string format, params object[] args) => VssRequestContextExtensions.Trace(this.requestContext, tracePoint, TraceLevel.Info, "ReleaseManagementService", "Pipeline", format, args);

    private void TraceEnterInformationMessage(int tracePoint, string methodName) => this.requestContext.TraceEnter(tracePoint, "ReleaseManagementService", "Pipeline", methodName);

    private void TraceLeaveInformationMessage(int tracePoint, string methodName) => this.requestContext.TraceLeave(tracePoint, "ReleaseManagementService", "Pipeline", methodName);

    private void FireEnvironmentCompletionEvent(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int currentEnvironmentId,
      EnvironmentStatus environmentStatus,
      string comment)
    {
      EventsHelper.FireReleaseEnvironmentCompletionEvent(this.requestContext, this.projectId, release, currentEnvironmentId, environmentStatus, comment);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = release.Environments.SingleOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (e => e.Id == currentEnvironmentId));
      if (releaseEnvironment == null)
        return;
      this.dataAccessLayer.RemoveEnvironmentFromQueue(release.Id, releaseEnvironment.Id);
      if (!this.ShouldQueueOnStartEnvironmentJob())
        return;
      this.QueueOnStartEnvironmentJob(release.Id, release.ReleaseDefinitionId, releaseEnvironment.DefinitionEnvironmentId, 0, Guid.Empty, Guid.Empty, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentReason.None, true);
    }

    private bool ShouldQueueOnStartEnvironmentJob()
    {
      bool flag = false;
      this.requestContext.Items.TryGetValue<bool>("DontQueueStartEnvironmentJob", out flag);
      return !flag;
    }

    private void FireReleaseAbandonedEvent(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release) => EventsHelper.FireReleaseAbandonedEvent(this.requestContext, this.projectId, release);

    private void FireApprovalPendingEvent(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      ReleaseEnvironmentStep approvalStep,
      int releaseId)
    {
      EventsHelper.FireApprovalPendingEvent(this.requestContext, this.projectId, release, approvalStep, releaseId);
    }

    private void FireApprovalCompletionEvent(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release, ReleaseEnvironmentStep approvalStep) => EventsHelper.FireApprovalCompletedEvent(this.requestContext, this.projectId, release, approvalStep);

    private void FireEnvironmentExecutionStartEvent(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment) => EventsHelper.FireEnvironmentExecutionStartEvent(this.requestContext, this.projectId, release, environment);

    private bool ReleaseShouldBeActive(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release, int approvalStepId)
    {
      if (release.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Active)
        return true;
      this.TraceInformationMessage(1972117, "OSPV2: MoveWorkFlowForward: Release is not in active state. ReleaseId: {0}, ReleaseStatus {1}, ApprovalStepId {2}", (object) release.Id, (object) release.Status, (object) approvalStepId);
      return false;
    }

    private bool ApprovalShouldBeCompleted(ReleaseEnvironmentStep step)
    {
      if (step.Status == ReleaseEnvironmentStepStatus.Done)
        return true;
      this.TraceInformationMessage(1972117, "OSPV2: MoveWorkFlowForward: Approval step is not in Done state. ReleaseId: {0}, CurrentApprovalStepId: {1}, CurrentReleaseStepStatus: {2}", (object) step.ReleaseId, (object) step.Id, (object) step.Status);
      return false;
    }

    private bool EnvironmentShouldBeInProgress(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      ReleaseEnvironmentStep currentApprovalStep)
    {
      if (releaseEnvironment.Status == ReleaseEnvironmentStatus.InProgress)
        return true;
      this.TraceInformationMessage(1972117, "OSPV2: MoveWorkFlowForward: ReleaseEnvironment is not in inprogress state. ReleaseId: {0}, CurrentEnvironmentId: {1} CurrentEnvironmentStatus {2}, CurrentApprovalStepId: {3}", (object) release.Id, (object) releaseEnvironment.Id, (object) releaseEnvironment.Status, (object) currentApprovalStep.Id);
      return false;
    }

    private bool AreParallelApprovalsHandled(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      ReleaseEnvironmentStep currentApprovalStep,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      bool areApprovalsParallel)
    {
      bool flag = this.IsMoveWorkflowForwardAllowed(release, currentApprovalStep, areApprovalsParallel);
      if (!flag)
        this.TraceInformationMessage(1972117, "OSPV2: MoveWorkFlowForward: Required number approvals criteria not met. ReleaseId: {0}, CurrentEnvironmentId: {1}, CurrentReleaseStepId: {2}", (object) release.Id, (object) releaseEnvironment.Id, (object) currentApprovalStep.Id);
      return flag;
    }

    private Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release CleanupApprovalsData(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      ReleaseEnvironmentStep currentApprovalStep,
      bool areApprovalsParallel)
    {
      if (!release.GetHigherRankApprovalSteps(releaseEnvironment.Name, currentApprovalStep.StepType, currentApprovalStep.Rank).Any<DefinitionEnvironmentStep>())
        ReleaseOperationHelper.DeleteJobDefinition(this.requestContext, currentApprovalStep.ApprovalTimeoutJobId, release.Id);
      if (currentApprovalStep.StepType == EnvironmentStepType.PreDeploy)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1 = this.MarkStepsAsSkippedIfRequired(release, currentApprovalStep, areApprovalsParallel, 1972117);
        if (release1 != null & areApprovalsParallel)
        {
          this.SendReleaseEnvironmentUpdatedEvent(release1.ReleaseDefinitionId, release1.Id, releaseEnvironment.Id);
          return release1;
        }
      }
      return release;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The exception is thrown in the subsequent code.")]
    private void ProcessReleaseEnvironmentChecks(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment,
      int attempt)
    {
      if (!this.requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.ValidateStageForResourcesWithChecks"))
        return;
      string str = string.Empty;
      Exception exception = (Exception) null;
      bool flag = this.requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.InjectCheckAsGate.ServiceEndpoint") || this.requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.InjectCheckAsGate.AgentQueue");
      try
      {
        CheckConfigurationEvaluator configurationEvaluator = new CheckConfigurationEvaluator(this.requestContext, this.projectId, release, environment);
        IList<CheckConfiguration> configurationsOnResources = configurationEvaluator.GetCheckConfigurationsOnResources();
        if (flag)
        {
          (IList<CheckConfiguration> checkConfigs, IList<CheckConfiguration> checkConfigurationList) = configurationEvaluator.SplitBySupportedType(configurationsOnResources);
          if (!checkConfigurationList.IsNullOrEmpty<CheckConfiguration>())
          {
            str = configurationEvaluator.GetCheckValidationExceptionMessage(checkConfigurationList.First<CheckConfiguration>());
          }
          else
          {
            Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1 = release.DeepClone();
            if (configurationEvaluator.UpdateGatesWithInjectedChecks(checkConfigs))
            {
              ReleasesService service = this.requestContext.GetService<ReleasesService>();
              Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release contract = release.ToContract(this.requestContext, this.projectId, true, ApprovalFilters.All);
              contract.Comment = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ChecksInjectedAsGatesComment);
              IVssRequestContext requestContext = this.requestContext;
              Guid projectId = this.projectId;
              int id = release.Id;
              Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease = contract;
              Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release existingServerRelease = release1;
              service.UpdateRelease(requestContext, projectId, id, webApiRelease, existingServerRelease);
            }
          }
        }
        else if (!configurationsOnResources.IsNullOrEmpty<CheckConfiguration>())
          str = configurationEvaluator.GetCheckValidationExceptionMessage(configurationsOnResources.First<CheckConfiguration>());
        if (!str.IsNullOrEmpty<char>())
          exception = (Exception) new PipelineValidationException(str);
      }
      catch (Exception ex)
      {
        this.requestContext.Trace(1960098, TraceLevel.Error, "ReleaseManagementService", "Pipeline", "OSPV2: ValidateEnvironmentForChecks: Exception received while validating environment for checks: (ReleaseId: {0}, Name: {1}, EnvironmentId: {2}, Attempt: {3}) Message: {4} Stack trace: {5}", (object) release.Id, (object) release.Name, (object) environment.Id, (object) attempt, (object) ex.Message, (object) ex.StackTrace);
        str = flag ? ex.Message : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ChecksValidationGenericExceptionMessage);
        exception = (Exception) new PipelineValidationException(str);
      }
      if (!str.IsNullOrEmpty<char>())
      {
        ReleaseEnvironmentStatus environmentStatus = ReleaseEnvironmentStatus.Rejected;
        release = this.dataAccessLayer.UpdateEnvironmentAndDeploymentStatus(release, environment.Id, attempt, environmentStatus, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.NotDeployed, (ReleaseEnvironmentStatusChangeDetails) null, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.PhaseFailed);
        this.FireEnvironmentCompletionEvent(release, environment.Id, environmentStatus.ToWebApi(), str);
        throw exception;
      }
    }

    internal delegate void QueueReleaseValidator(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease,
      int releaseEnvironmentId,
      out Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment targetEnvironment);
  }
}
