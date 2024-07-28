// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.DeployPhaseRunner
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed.")]
  public class DeployPhaseRunner
  {
    private readonly IVssRequestContext requestContext;
    private readonly Guid projectId;
    private readonly IDataAccessLayer dataAccessLayer;
    private readonly Func<IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>, IDictionary<string, MergedConfigurationVariableValue>> variableGroupsMerger;
    private readonly Func<IVssRequestContext, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes, IDistributedTaskOrchestrator> getDistributedTaskOrchestratorFunc;
    private readonly Func<IVssRequestContext, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, DeployPhaseSnapshot, AutomationEngineInput, IDictionary<string, string>, PhaseConditionResult> shouldExecutePhase;

    public DeployPhaseRunner(IVssRequestContext requestContext, Guid projectId)
      : this(requestContext, projectId, (IDataAccessLayer) new DataAccessLayer(requestContext, projectId), (Func<IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>, IDictionary<string, MergedConfigurationVariableValue>>) (groups => VariableGroupsMerger.MergeVariableGroups((IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) new List<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) groups))), DeployPhaseRunner.\u003C\u003EO.\u003C0\u003E__CreateDistributedTaskOrchestrator ?? (DeployPhaseRunner.\u003C\u003EO.\u003C0\u003E__CreateDistributedTaskOrchestrator = new Func<IVssRequestContext, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes, IDistributedTaskOrchestrator>(DistributedTaskOrchestrator.CreateDistributedTaskOrchestrator)), new Func<IVssRequestContext, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, DeployPhaseSnapshot, AutomationEngineInput, IDictionary<string, string>, PhaseConditionResult>(new PhaseConditionEvaluator().ShouldExecutePhase))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    internal DeployPhaseRunner(
      IVssRequestContext requestContext,
      Guid projectId,
      IDataAccessLayer dataAccessLayer,
      Func<IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>, IDictionary<string, MergedConfigurationVariableValue>> variableGroupsMerger,
      Func<IVssRequestContext, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes, IDistributedTaskOrchestrator> getDistributedTaskOrchestratorFunc,
      Func<IVssRequestContext, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, DeployPhaseSnapshot, AutomationEngineInput, IDictionary<string, string>, PhaseConditionResult> shouldExecutePhase)
    {
      this.requestContext = requestContext;
      this.projectId = projectId;
      this.dataAccessLayer = dataAccessLayer;
      this.variableGroupsMerger = variableGroupsMerger;
      this.getDistributedTaskOrchestratorFunc = getDistributedTaskOrchestratorFunc;
      this.shouldExecutePhase = shouldExecutePhase;
    }

    public bool Cancel(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release, ReleaseEnvironmentStep deployStep)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (deployStep == null)
        throw new ArgumentNullException(nameof (deployStep));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment = release.GetEnvironment(deployStep.ReleaseEnvironmentId);
      if (environment == null)
        return false;
      if (environment.IsYamlEnvironment())
        return deployStep.RunPlanId.HasValue && new PipelineOrchestrator(this.requestContext, this.projectId).CancelDeployment(deployStep.RunPlanId.Value, TimeSpan.FromMinutes(1.0));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase currentPhase = environment.GetDeployPhases(deployStep.TrialNumber).OrderBy<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase, int>) (p => p.Id)).LastOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase>();
      if (currentPhase != null)
      {
        Guid? runPlanId = currentPhase.RunPlanId;
        if (runPlanId.HasValue)
        {
          runPlanId = currentPhase.RunPlanId;
          if (!(runPlanId.Value == Guid.Empty))
          {
            IDistributedTaskOrchestrator taskOrchestrator = this.getDistributedTaskOrchestratorFunc(this.requestContext, this.projectId, currentPhase.PhaseType);
            Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dictionary = this.GetMergedReleaseVariables(release, environment, (ReleaseEnvironmentSnapshotDelta) null, false).ToDictionary<KeyValuePair<string, MergedConfigurationVariableValue>, string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, MergedConfigurationVariableValue>, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) (p => new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
            {
              Value = p.Value.Value,
              IsSecret = p.Value.IsSecret
            }), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            DeployPhaseSnapshot deployPhaseSnapshot = environment.GetDesignerDeployPhaseSnapshots().Single<DeployPhaseSnapshot>((Func<DeployPhaseSnapshot, bool>) (p => p.Rank == currentPhase.Rank));
            TimeSpan timeSpan = TimeSpan.FromMinutes(1.0);
            Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables = dictionary;
            BaseDeploymentInput deploymentInput = deployPhaseSnapshot.GetDeploymentInput((IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) variables);
            if (deploymentInput != null && currentPhase.PhaseType != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.RunOnServer)
            {
              int timeoutInMinutes = deploymentInput.JobCancelTimeoutInMinutes;
              if (timeoutInMinutes > 0 && timeoutInMinutes <= 60)
                timeSpan = TimeSpan.FromMinutes((double) timeoutInMinutes);
            }
            runPlanId = currentPhase.RunPlanId;
            Guid planId = runPlanId.Value;
            TimeSpan jobCancelTimeout = timeSpan;
            return taskOrchestrator.CancelDeployment(planId, jobCancelTimeout);
          }
        }
      }
      return false;
    }

    public DeployPhaseRunStatus Run(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      ReleaseEnvironmentStep deployStep,
      DeployPhaseSnapshot snapshotToProcess)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (releaseEnvironment == null)
        throw new ArgumentNullException(nameof (releaseEnvironment));
      if (deployStep == null)
        throw new ArgumentNullException(nameof (deployStep));
      if (snapshotToProcess == null)
        throw new ArgumentNullException(nameof (snapshotToProcess));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase releaseDeployPhase = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase) null;
      bool shouldSkipPhaseForRedeployment = false;
      try
      {
        IDistributedTaskOrchestrator distributedTaskOrchestrator = this.getDistributedTaskOrchestratorFunc(this.requestContext, this.projectId, snapshotToProcess.PhaseType);
        releaseDeployPhase = this.dataAccessLayer.AddDeployPhase(deployStep.ReleaseId, deployStep.ReleaseEnvironmentId, deployStep.TrialNumber, snapshotToProcess.Rank, snapshotToProcess.PhaseType);
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deploymentByAttempt = releaseEnvironment.GetDeploymentByAttempt(deployStep.TrialNumber);
        ReleaseEnvironmentSnapshotDelta deploymentSnapshotDelta = this.dataAccessLayer.GetDeploymentSnapshotDelta(release.Id, deploymentByAttempt.Id);
        AutomationEngineInput automationEngineInput = this.GetAutomationEngineInput(release, releaseEnvironment, deployStep, deployStep.TrialNumber, snapshotToProcess, deploymentSnapshotDelta);
        IDictionary<string, string> systemVariables = distributedTaskOrchestrator.GetSystemVariables(automationEngineInput);
        automationEngineInput.ReleaseDeployPhaseId = releaseDeployPhase.Id;
        if (releaseDeployPhase.PhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.RunOnServer)
        {
          Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> gateOutputVariables = OutputVariablesUtility.GetGateOutputVariables(distributedTaskOrchestrator, releaseEnvironment, deployStep.TrialNumber);
          if (gateOutputVariables != null)
            Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.VariablesUtility.FillVariables((IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) gateOutputVariables, (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) automationEngineInput.Variables);
        }
        if (DeployPhaseRunner.IsRedeployWithDeploymentGroupPhase(deploymentByAttempt, snapshotToProcess.PhaseType))
          this.UpdateAutomationEngineInputWithDeploymentGroupPhaseData(releaseEnvironment, deploymentByAttempt, snapshotToProcess, deploymentSnapshotDelta, automationEngineInput, out shouldSkipPhaseForRedeployment);
        PhaseConditionResult phaseConditionResult = this.ShouldExecutePhase(releaseEnvironment, snapshotToProcess, automationEngineInput, systemVariables, shouldSkipPhaseForRedeployment);
        if (phaseConditionResult.Value)
        {
          releaseDeployPhase.RunPlanId = new Guid?(distributedTaskOrchestrator.StartDeployment(automationEngineInput));
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus operationStatus = this.requestContext.IsFeatureEnabled("WebAccess.ReleaseManagement.QueuedForPipeline") ? Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Undefined : Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.QueuedForPipeline;
          this.dataAccessLayer.UpdateDeployPhase(releaseDeployPhase, operationStatus, release.ReleaseDefinitionId);
          return new DeployPhaseRunStatus()
          {
            HasStarted = true,
            ReleaseDeployPhaseId = releaseDeployPhase.Id
          };
        }
        releaseDeployPhase.Status = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseStatus.Skipped;
        releaseDeployPhase.Logs = phaseConditionResult.Message;
        this.dataAccessLayer.UpdateDeployPhase(releaseDeployPhase, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Undefined, release.ReleaseDefinitionId);
        return new DeployPhaseRunStatus()
        {
          HasStarted = false,
          ReleaseDeployPhaseId = releaseDeployPhase.Id
        };
      }
      catch (DeploymentOperationStatusAlreadyUpdatedException ex)
      {
      }
      catch (DeploymentUpdateNotAllowedException ex)
      {
      }
      catch (Exception ex1)
      {
        if (releaseDeployPhase == null)
        {
          deployStep.Logs += TeamFoundationExceptionFormatter.FormatException(ex1, false);
          this.dataAccessLayer.UpdateReleaseStep(deployStep);
          throw;
        }
        else
        {
          releaseDeployPhase.Status = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseStatus.Failed;
          releaseDeployPhase.Logs += TeamFoundationExceptionFormatter.FormatException(ex1, false);
          try
          {
            this.dataAccessLayer.UpdateDeployPhase(releaseDeployPhase, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.PhaseFailed, release.ReleaseDefinitionId);
            throw;
          }
          catch (DeploymentUpdateNotAllowedException ex2)
          {
          }
        }
      }
      return new DeployPhaseRunStatus()
      {
        HasStarted = false,
        ReleaseDeployPhaseId = releaseDeployPhase != null ? releaseDeployPhase.Id : 0
      };
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "As per design")]
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "When reached this flow, objects will never be null")]
    public AutomationEngineInput GetAutomationEngineInput(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      ReleaseEnvironmentStep step,
      int trialNumber,
      DeployPhaseSnapshot snapshotToProcess,
      ReleaseEnvironmentSnapshotDelta deploymentDelta)
    {
      IDictionary<string, MergedConfigurationVariableValue> releaseVariables = this.GetMergedReleaseVariables(release, releaseEnvironment, deploymentDelta);
      Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dictionary1 = releaseVariables.ToDictionary<KeyValuePair<string, MergedConfigurationVariableValue>, string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, MergedConfigurationVariableValue>, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) (p => new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
      {
        Value = p.Value.Value,
        IsSecret = p.Value.IsSecret
      }), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
      dictionary2.Add("ReleaseName", release.Name);
      dictionary2.Add("ReleaseReason", Enum.GetName(typeof (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason), (object) release.Reason));
      dictionary2.Add("ReleaseDescription", release.Description);
      dictionary2.Add("ReleaseDefinitionName", release.ReleaseDefinitionName);
      int num = release.ReleaseDefinitionId;
      dictionary2.Add("ReleaseDefinitionId", num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      dictionary2.Add("ReleaseEnvironmentName", releaseEnvironment.Name);
      num = releaseEnvironment.DefinitionEnvironmentId;
      dictionary2.Add("ReleaseDefinitionEnvironmentId", num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      num = releaseEnvironment.GetLatestDeploymentId();
      dictionary2.Add("DeploymentId", num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      Dictionary<string, string> dictionaryData = dictionary2;
      if (snapshotToProcess != null && snapshotToProcess.PhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.DeploymentGates && snapshotToProcess.GetDeploymentInput() is GatesDeploymentInput deploymentInput)
      {
        Dictionary<string, string> dictionary3 = dictionaryData;
        num = deploymentInput.SamplingInterval;
        string str1 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        dictionary3["GreenlightingSamplingIntervalInMinutes"] = str1;
        Dictionary<string, string> dictionary4 = dictionaryData;
        num = deploymentInput.StabilizationTime;
        string str2 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        dictionary4["GreenlightingStabilizationTimeInMinutes"] = str2;
        Dictionary<string, string> dictionary5 = dictionaryData;
        num = deploymentInput.MinimumSuccessDuration;
        string str3 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        dictionary5["GreenlightingMinimumSuccessfulMinutes"] = str3;
        Dictionary<string, string> dictionary6 = dictionaryData;
        num = deploymentInput.TimeoutInMinutes;
        string str4 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        dictionary6["GreenlightingJobTimeoutInMinutes"] = str4;
      }
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deploymentByAttempt = releaseEnvironment.GetDeploymentByAttempt(trialNumber);
      Guid requestedFor = deploymentByAttempt.RequestedFor;
      dictionaryData.Add("DeploymentStartTime", deploymentByAttempt.QueuedOn.ToString("u", (IFormatProvider) CultureInfo.InvariantCulture));
      dictionaryData.Add("ReleaseEnvironmentTriggerReason", Enum.GetName(typeof (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentReason), (object) deploymentByAttempt.Reason));
      AutomationEngineInput automationEngineInput = new AutomationEngineInput()
      {
        ReleaseId = release.Id,
        ReleaseStepId = step.Id,
        EnvironmentId = releaseEnvironment.Id,
        DeployPhaseData = snapshotToProcess,
        RequestedForId = release.CreatedBy,
        AttemptNumber = trialNumber,
        ReleaseDeploymentRequestedForId = requestedFor,
        StepType = (int) step.StepType,
        DeploymentId = deploymentByAttempt.Id
      };
      Dictionary<string, MergedConfigurationVariableValue> dictionary7 = releaseVariables.Where<KeyValuePair<string, MergedConfigurationVariableValue>>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, bool>) (v => v.Value.IsExternalVariable)).ToDictionary<KeyValuePair<string, MergedConfigurationVariableValue>, string, MergedConfigurationVariableValue>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, MergedConfigurationVariableValue>, MergedConfigurationVariableValue>) (p => p.Value));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.VariablesUtility.FillVariables((IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) dictionary1, (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) automationEngineInput.Variables);
      foreach (IGrouping<int, KeyValuePair<string, MergedConfigurationVariableValue>> grouping in dictionary7.GroupBy<KeyValuePair<string, MergedConfigurationVariableValue>, int>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, int>) (v => v.Value.VariableGroupId)))
      {
        IGrouping<int, KeyValuePair<string, MergedConfigurationVariableValue>> groupIdToExternalVariablesMap = grouping;
        IList<TaskInstance> variableGroupTasks = (release.VariableGroups.SingleOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, bool>) (vg => vg.Id == groupIdToExternalVariablesMap.Key)) ?? releaseEnvironment.VariableGroups.SingleOrDefault<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, bool>) (vg => vg.Id == groupIdToExternalVariablesMap.Key))).GetVariableGroupTasks((IList<string>) groupIdToExternalVariablesMap.Select<KeyValuePair<string, MergedConfigurationVariableValue>, string>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (v => v.Key)).ToList<string>());
        automationEngineInput.ExternalVariableTasks.AddRange<TaskInstance, IList<TaskInstance>>((IEnumerable<TaskInstance>) variableGroupTasks);
      }
      automationEngineInput.DataMerger((IDictionary<string, string>) dictionaryData);
      automationEngineInput.MergeArtifacts(release.LinkedArtifacts);
      this.PopulatePreviousReleaseArtifactVersion(this.requestContext, automationEngineInput);
      if (this.requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.UsePipelineOrchestrator.Phase") && snapshotToProcess != null && snapshotToProcess.PhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.AgentBasedDeployment)
      {
        IOrchestrationProcess pipelineProcess = snapshotToProcess != null ? snapshotToProcess.GetPipelineProcess(this.requestContext, this.projectId, automationEngineInput) : (IOrchestrationProcess) null;
        automationEngineInput.Process = pipelineProcess;
      }
      return automationEngineInput;
    }

    private static bool IsMatchingDeploymentGroupPhaseDelta(
      DeploymentGroupPhaseDelta deploymentGroupPhaseDelta,
      DeployPhaseSnapshot snapshotToProcess)
    {
      MachineGroupDeploymentInput deploymentInput = (MachineGroupDeploymentInput) snapshotToProcess.GetDeploymentInput();
      if (deploymentGroupPhaseDelta.DeploymentGroupId != deploymentInput.QueueId)
        return false;
      int? deployPhaseRank = deploymentGroupPhaseDelta.DeployPhaseRank;
      int rank = snapshotToProcess.Rank;
      return deployPhaseRank.GetValueOrDefault() == rank & deployPhaseRank.HasValue;
    }

    private static Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> GetArtifactVariables(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      IVssRequestContext requestContext)
    {
      Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> artifactVariables = new Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>();
      bool flag = false;
      ArtifactTypeServiceBase service = requestContext.GetService<ArtifactTypeServiceBase>();
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue configurationVariableValue = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
      {
        Value = DeployPhaseRunner.GetTriggeringArtifactAlias(release)
      };
      artifactVariables["release.triggeringartifact.alias"] = configurationVariableValue;
      foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) release.LinkedArtifacts)
      {
        IDictionary<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue> configurationVariables = service.GetArtifactType(requestContext, linkedArtifact.ArtifactTypeId).GetConfigurationVariables(requestContext, linkedArtifact);
        foreach (KeyValuePair<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue> keyValuePair in (IEnumerable<KeyValuePair<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue>>) configurationVariables)
        {
          string artifactVariableKey = WellKnownReleaseVariables.GetReleaseArtifactVariableKey(linkedArtifact.Alias, keyValuePair.Key);
          artifactVariables[artifactVariableKey] = DeployPhaseRunner.ToConfigurationVariable(keyValuePair.Value);
        }
        if (linkedArtifact.IsPrimary && !flag)
        {
          artifactVariables["release.primaryArtifactSourceAlias"] = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
          {
            Value = linkedArtifact.Alias
          };
          foreach (KeyValuePair<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue> keyValuePair in (IEnumerable<KeyValuePair<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue>>) configurationVariables)
          {
            string artifactVariableKey = WellKnownReleaseVariables.GetReleasePrimaryArtifactVariableKey(keyValuePair.Key);
            artifactVariables[artifactVariableKey] = DeployPhaseRunner.ToConfigurationVariable(keyValuePair.Value);
          }
          flag = true;
        }
      }
      return artifactVariables;
    }

    private static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue ToConfigurationVariable(
      Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue artifactVariableValue)
    {
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
      {
        AllowOverride = artifactVariableValue.AllowOverride,
        IsSecret = artifactVariableValue.IsSecret,
        Value = artifactVariableValue.Value
      };
    }

    private static string GetTriggeringArtifactAlias(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      string triggeringArtifactAlias = string.Empty;
      foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) release.LinkedArtifacts)
      {
        if (linkedArtifact.SourceData.ContainsKey("IsTriggeringArtifact") && linkedArtifact.SourceData["IsTriggeringArtifact"].Value.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
        {
          triggeringArtifactAlias = linkedArtifact.Alias;
          break;
        }
      }
      return triggeringArtifactAlias;
    }

    private static Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> GetReleaseVariables(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment)
    {
      Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> releaseVariables = new Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>()
      {
        {
          "release.releaseName",
          new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
          {
            Value = serverRelease.Name,
            IsSecret = false
          }
        },
        {
          "release.reason",
          new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
          {
            Value = Enum.GetName(typeof (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason), (object) serverRelease.Reason),
            IsSecret = false
          }
        },
        {
          "release.definitionName",
          new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
          {
            Value = serverRelease.ReleaseDefinitionName,
            IsSecret = false
          }
        },
        {
          "release.definitionId",
          new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
          {
            Value = serverRelease.ReleaseDefinitionId.ToString((IFormatProvider) CultureInfo.InvariantCulture),
            IsSecret = false
          }
        },
        {
          "release.releaseDescription",
          new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
          {
            Value = serverRelease.Description,
            IsSecret = false
          }
        },
        {
          "release.environmentName",
          new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
          {
            Value = releaseEnvironment.Name,
            IsSecret = false
          }
        },
        {
          "release.definitionEnvironmentId",
          new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
          {
            Value = releaseEnvironment.DefinitionEnvironmentId.ToString((IFormatProvider) CultureInfo.InvariantCulture),
            IsSecret = false
          }
        },
        {
          "release.deploymentId",
          new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
          {
            Value = releaseEnvironment.GetLatestDeploymentId().ToString((IFormatProvider) CultureInfo.InvariantCulture),
            IsSecret = false
          }
        }
      };
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>) serverRelease.Environments)
      {
        string environmentStatusKey = WellKnownReleaseVariables.GetReleaseEnvironmentStatusKey(environment.Name);
        releaseVariables[environmentStatusKey] = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
        {
          Value = environment.Status.ToString(),
          IsSecret = false
        };
      }
      return releaseVariables;
    }

    private static List<int> GetDeploymentGroupTargetIdsForRedeployTrigger(
      DeployPhaseSnapshot snapshotToProcess,
      ReleaseEnvironmentSnapshotDelta deploymentDelta)
    {
      List<int> source = new List<int>();
      if (deploymentDelta != null)
      {
        foreach (DeploymentGroupPhaseDelta deploymentGroupPhaseDelta in deploymentDelta.DeploymentGroupPhaseDelta.Where<DeploymentGroupPhaseDelta>((Func<DeploymentGroupPhaseDelta, bool>) (dpd => DeployPhaseRunner.IsMatchingDeploymentGroupPhaseDelta(dpd, snapshotToProcess))))
          source.AddRange(deploymentGroupPhaseDelta.DeploymentTargetIds);
      }
      return source.Distinct<int>().ToList<int>();
    }

    private static bool IsRedeployDeploymentGroupPhaseWithOnlyFailedTargets(
      IReadOnlyDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue configurationVariableValue;
      return variables.TryGetValue("release.redeployment.deploymentGroupTargetFilter", out configurationVariableValue) && "FailedTargets".Equals(configurationVariableValue.Value, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsRedeployWithDeploymentGroupPhase(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes phaseType)
    {
      return deployment.IsRedeployAttempt() && phaseType == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.MachineGroupBasedDeployment;
    }

    private void UpdateAutomationEngineInputWithDeploymentGroupPhaseData(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment,
      DeployPhaseSnapshot snapshotToProcess,
      ReleaseEnvironmentSnapshotDelta deploymentDelta,
      AutomationEngineInput automationEngineInput,
      out bool shouldSkipPhaseForRedeployment)
    {
      IList<int> enumerable = (IList<int>) null;
      shouldSkipPhaseForRedeployment = false;
      bool flag = false;
      if (deployment.Reason == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentReason.RedeployTrigger)
      {
        flag = true;
        enumerable = (IList<int>) DeployPhaseRunner.GetDeploymentGroupTargetIdsForRedeployTrigger(snapshotToProcess, deploymentDelta);
      }
      else if (DeployPhaseRunner.IsRedeployDeploymentGroupPhaseWithOnlyFailedTargets((IReadOnlyDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) automationEngineInput.Variables))
      {
        flag = true;
        enumerable = this.GetDeploymentGroupUnsuccessfulTargetsWithPreviousAttempt(releaseEnvironment, deployment.Attempt, snapshotToProcess);
      }
      if (!flag)
        return;
      if (enumerable.IsNullOrEmpty<int>())
      {
        shouldSkipPhaseForRedeployment = true;
      }
      else
      {
        MachineGroupDeploymentInput deploymentInput = (MachineGroupDeploymentInput) snapshotToProcess.GetDeploymentInput();
        deploymentInput.HealthPercent = 0;
        deploymentInput.DeploymentHealthOption = "Custom";
        automationEngineInput.Data.Add("DeploymentTargetIds", JsonConvert.SerializeObject((object) enumerable));
        snapshotToProcess.DeploymentInput = JObject.FromObject((object) deploymentInput);
      }
    }

    private void PopulatePreviousReleaseArtifactVersion(
      IVssRequestContext context,
      AutomationEngineInput automationEngineInput)
    {
      foreach (ArtifactSource artifact in (IEnumerable<ArtifactSource>) automationEngineInput.Artifacts)
      {
        if (artifact.IsPrimary && ReleaseWorkItemsCommitsHelper.DoesArtifactTypeSupportsCommitsOrWorkItemsTraceability(context, artifact.ArtifactTypeId) && !artifact.SourceData.ContainsKey("ArtifactDetailsReference"))
        {
          string releaseArtifactVersion = artifact.GetPreviousReleaseArtifactVersion(context, this.projectId, automationEngineInput.ReleaseId);
          artifact.SourceData["PreviousArtifactVersion"] = new InputValue()
          {
            Value = releaseArtifactVersion
          };
        }
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "As per design")]
    private IDictionary<string, MergedConfigurationVariableValue> GetMergedReleaseVariables(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      ReleaseEnvironmentSnapshotDelta deploymentDelta,
      bool includeArtifactVariables = true)
    {
      IDictionary<string, MergedConfigurationVariableValue> dictionary1 = this.variableGroupsMerger(release.VariableGroups);
      IDictionary<string, MergedConfigurationVariableValue> dictionary2 = this.variableGroupsMerger(releaseEnvironment.VariableGroups);
      Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dataModelVariables = releaseEnvironment.ProcessParameters.GetProcessParametersAsDataModelVariables();
      Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> source1 = new Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>();
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> source2 = (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) new Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>();
      if (includeArtifactVariables)
        source1 = DeployPhaseRunner.GetArtifactVariables(release, this.requestContext);
      if (deploymentDelta != null && !deploymentDelta.Variables.IsNullOrEmpty<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>())
        source2 = deploymentDelta.Variables;
      Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> releaseVariables = DeployPhaseRunner.GetReleaseVariables(release, releaseEnvironment);
      return DictionaryMerger.MergeDictionaries<string, MergedConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, MergedConfigurationVariableValue>>) new IDictionary<string, MergedConfigurationVariableValue>[8]
      {
        (IDictionary<string, MergedConfigurationVariableValue>) source1.ToDictionary<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string, MergedConfigurationVariableValue>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, MergedConfigurationVariableValue>) (p => p.Value.ToMergedConfigurationVariableValue())),
        (IDictionary<string, MergedConfigurationVariableValue>) releaseVariables.ToDictionary<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string, MergedConfigurationVariableValue>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, MergedConfigurationVariableValue>) (p => p.Value.ToMergedConfigurationVariableValue())),
        (IDictionary<string, MergedConfigurationVariableValue>) dataModelVariables.ToDictionary<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string, MergedConfigurationVariableValue>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, MergedConfigurationVariableValue>) (p => p.Value.ToMergedConfigurationVariableValue())),
        (IDictionary<string, MergedConfigurationVariableValue>) source2.ToDictionary<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string, MergedConfigurationVariableValue>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, MergedConfigurationVariableValue>) (p => p.Value.ToMergedConfigurationVariableValue())),
        (IDictionary<string, MergedConfigurationVariableValue>) releaseEnvironment.Variables.ToDictionary<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string, MergedConfigurationVariableValue>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, MergedConfigurationVariableValue>) (p => p.Value.ToMergedConfigurationVariableValue())),
        dictionary2,
        (IDictionary<string, MergedConfigurationVariableValue>) release.Variables.ToDictionary<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string, MergedConfigurationVariableValue>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, MergedConfigurationVariableValue>) (p => p.Value.ToMergedConfigurationVariableValue())),
        dictionary1
      });
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed")]
    private IList<int> GetDeploymentGroupUnsuccessfulTargetsWithPreviousAttempt(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      int currentDeploymentAttempt,
      DeployPhaseSnapshot snapshotToProcess)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase releaseDeployPhase = releaseEnvironment.ReleaseDeployPhases.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase, bool>) (p => p.Attempt == currentDeploymentAttempt - 1 && p.Rank == snapshotToProcess.Rank));
      if (releaseDeployPhase != null)
      {
        MachineGroupDeploymentInput deploymentInput = (MachineGroupDeploymentInput) snapshotToProcess.GetDeploymentInput();
        if (deploymentInput != null)
        {
          Dictionary<string, string> unsuccessfulRecords = this.getDistributedTaskOrchestratorFunc(this.requestContext, this.projectId, releaseDeployPhase.PhaseType).GetTimelineRecords(releaseDeployPhase.RunPlanId.GetValueOrDefault()).ToList<TimelineRecord>().Where<TimelineRecord>((Func<TimelineRecord, bool>) (t =>
          {
            if (t.RecordType == "Job")
            {
              TaskResult? result = t.Result;
              TaskResult taskResult1 = TaskResult.Succeeded;
              if (!(result.GetValueOrDefault() == taskResult1 & result.HasValue))
              {
                result = t.Result;
                TaskResult taskResult2 = TaskResult.SucceededWithIssues;
                if (!(result.GetValueOrDefault() == taskResult2 & result.HasValue))
                  return !t.WorkerName.IsNullOrEmpty<char>();
              }
            }
            return false;
          })).ToDictionary<TimelineRecord, string, string>((Func<TimelineRecord, string>) (k => k.WorkerName), (Func<TimelineRecord, string>) (k => k.WorkerName));
          if (unsuccessfulRecords.Any<KeyValuePair<string, string>>())
          {
            IDistributedTaskPoolService poolService = this.requestContext.GetService<IDistributedTaskPoolService>();
            DeploymentGroupManager deploymentGroupManager = new DeploymentGroupManager();
            return (IList<int>) this.requestContext.RunSynchronously<IList<DeploymentMachine>>((Func<Task<IList<DeploymentMachine>>>) (() => deploymentGroupManager.GetDeploymentTargetsAsync(this.requestContext, poolService, this.projectId, deploymentInput.QueueId, (IList<string>) null))).Where<DeploymentMachine>((Func<DeploymentMachine, bool>) (t => unsuccessfulRecords.ContainsKey(t.Agent.Name))).Select<DeploymentMachine, int>((Func<DeploymentMachine, int>) (a => a.Id)).ToList<int>();
          }
        }
      }
      return (IList<int>) new List<int>();
    }

    private PhaseConditionResult ShouldExecutePhase(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      DeployPhaseSnapshot snapshotToProcess,
      AutomationEngineInput automationEngineInput,
      IDictionary<string, string> systemVariables,
      bool shouldSkipPhaseOnRetrigger)
    {
      if (!shouldSkipPhaseOnRetrigger)
        return this.shouldExecutePhase(this.requestContext, releaseEnvironment, snapshotToProcess, automationEngineInput, systemVariables);
      return new PhaseConditionResult()
      {
        Value = false,
        Message = string.Empty
      };
    }
  }
}
