// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseEnvironmentExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "By Design")]
  public static class ReleaseEnvironmentExtensions
  {
    public static int GetReleaseEnvironmentStepApprovalInfo(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      ReleaseEnvironmentStepStatus stepStatus,
      EnvironmentStepType stepType)
    {
      if (releaseEnvironment == null)
        throw new ArgumentNullException(nameof (releaseEnvironment));
      return releaseEnvironment.GetStepStatusCount(stepStatus, stepType);
    }

    public static bool GetReleaseEnvironmentIsAuto(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      EnvironmentStepType stepType)
    {
      if (releaseEnvironment == null)
        throw new ArgumentNullException(nameof (releaseEnvironment));
      ReleaseEnvironmentStep releaseEnvironmentStep = releaseEnvironment.GetStepsForTests.FirstOrDefault<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.StepType == stepType));
      return releaseEnvironmentStep != null && releaseEnvironmentStep.IsAutomated;
    }

    public static IList<DeployPhaseSnapshot> GetDesignerDeployPhaseSnapshots(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes deployPhaseType)
    {
      return (IList<DeployPhaseSnapshot>) releaseEnvironment.GetDesignerDeployPhaseSnapshots().Where<DeployPhaseSnapshot>((Func<DeployPhaseSnapshot, bool>) (x => x.PhaseType == deployPhaseType)).ToList<DeployPhaseSnapshot>();
    }

    public static IList<DeployPhaseSnapshot> GetDesignerDeployPhaseSnapshots(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment)
    {
      if (releaseEnvironment == null)
        throw new ArgumentNullException(nameof (releaseEnvironment));
      return releaseEnvironment.IsYamlEnvironment() ? (IList<DeployPhaseSnapshot>) new List<DeployPhaseSnapshot>() : ((DesignerDeploymentSnapshot) releaseEnvironment.DeploymentSnapshot).DeployPhaseSnapshots;
    }

    public static IList<WorkflowTask> GetEnabledReleaseEnvironmentTasks(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment)
    {
      if (releaseEnvironment == null)
        throw new ArgumentNullException(nameof (releaseEnvironment));
      return (IList<WorkflowTask>) releaseEnvironment.GetDesignerDeployPhaseSnapshots().Aggregate<DeployPhaseSnapshot, List<WorkflowTask>>(new List<WorkflowTask>(), (Func<List<WorkflowTask>, DeployPhaseSnapshot, List<WorkflowTask>>) ((current, deployPhase) => current.Concat<WorkflowTask>(deployPhase.Workflow.Where<WorkflowTask>((Func<WorkflowTask, bool>) (x => x.Enabled))).ToList<WorkflowTask>()));
    }

    public static IEnumerable<WorkflowTask> GetReleaseEnvironmentMetaTasks(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      if (releaseEnvironment == null)
        throw new ArgumentNullException(nameof (releaseEnvironment));
      DefinitionEnvironment definitionEnvironment = releaseDefinition.Environments.FirstOrDefault<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (x => x.Id == releaseEnvironment.DefinitionEnvironmentId));
      List<WorkflowTask> seed = new List<WorkflowTask>();
      return definitionEnvironment == null ? (IEnumerable<WorkflowTask>) seed : (IEnumerable<WorkflowTask>) definitionEnvironment.DeployPhases.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase, List<WorkflowTask>>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase, List<WorkflowTask>>) (deployPhase => deployPhase.Workflow.ToWorkflowTaskList())).Aggregate<List<WorkflowTask>, List<WorkflowTask>>(seed, (Func<List<WorkflowTask>, List<WorkflowTask>, List<WorkflowTask>>) ((current, tasks) => current.Concat<WorkflowTask>(tasks.Where<WorkflowTask>((Func<WorkflowTask, bool>) (task => task.IsMetaTask() && task.Enabled))).ToList<WorkflowTask>()));
    }

    public static void FillDeploySteps(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverEnvironment,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      if (serverEnvironment == null)
        throw new ArgumentNullException(nameof (serverEnvironment));
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deploymentAttempt in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>) serverEnvironment.DeploymentAttempts)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment = deploymentAttempt;
        ReleaseEnvironmentStep releaseEnvironmentStep = serverEnvironment.GetDeploySteps().FirstOrDefault<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.TrialNumber == deployment.Attempt));
        int id = releaseEnvironmentStep == null ? 0 : releaseEnvironmentStep.Id;
        DeploymentAttempt webApiDeployment = new DeploymentAttempt()
        {
          Id = id,
          DeploymentId = deployment.Id,
          Attempt = deployment.Attempt,
          HasStarted = deployment.Status > Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.NotDeployed,
          Reason = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentReason) deployment.Reason,
          Status = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus) deployment.Status,
          OperationStatus = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus) deployment.OperationStatus,
          RequestedBy = new IdentityRef()
          {
            Id = deployment.RequestedBy.ToString()
          },
          RequestedFor = new IdentityRef()
          {
            Id = deployment.RequestedFor.ToString()
          },
          QueuedOn = deployment.QueuedOn,
          LastModifiedBy = new IdentityRef()
          {
            Id = deployment.LastModifiedBy.ToString()
          },
          LastModifiedOn = deployment.LastModifiedOn,
          ErrorLog = releaseEnvironmentStep?.Logs
        };
        if (!string.IsNullOrWhiteSpace(releaseEnvironmentStep?.Logs))
          webApiDeployment.Issues.Add(new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Issue()
          {
            IssueType = "Error",
            Message = releaseEnvironmentStep?.Logs
          });
        webApiDeployment.Issues.AddRange(deployment.DeploymentIssues.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Issue, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Issue>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Issue, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Issue>) (e => new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Issue()
        {
          IssueType = e.IssueType.ToString(),
          Message = e.Message
        })));
        environment.DeploySteps.Add(webApiDeployment);
        if (!requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.ListReleases.ExcludePhasesInPayload"))
          ReleaseEnvironmentExtensions.FillDeployPhases(requestContext, projectId, serverEnvironment, webApiDeployment);
      }
    }

    public static IEnumerable<ReleaseEnvironmentStep> GetDeploySteps(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverEnvironment)
    {
      if (serverEnvironment == null)
        throw new ArgumentNullException(nameof (serverEnvironment));
      return serverEnvironment.GetStepsForTests.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.StepType == EnvironmentStepType.Deploy));
    }

    public static bool ShouldDeferDeployStepExecution(this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment)
    {
      if (!environment.HasDeferredDeployment())
        return false;
      DateTime? scheduledDeploymentTime = environment.ScheduledDeploymentTime;
      DateTime utcNow = DateTime.UtcNow;
      return scheduledDeploymentTime.HasValue && scheduledDeploymentTime.GetValueOrDefault() > utcNow;
    }

    public static bool HasDeferredDeployment(this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      return environment.ScheduledDeploymentTime.HasValue;
    }

    public static TeamFoundationJobDefinition GetDeferredExecutionJobDefinitionForDeployStep(
      Guid projectIdentifier,
      ReleaseEnvironmentStep deployStep,
      DateTime scheduledDeploymentTime)
    {
      if (deployStep == null)
        throw new ArgumentNullException(nameof (deployStep));
      DeferredDeploymentExecutionJobData objectToSerialize = new DeferredDeploymentExecutionJobData()
      {
        ProjectId = projectIdentifier,
        DeployStepId = deployStep.Id,
        ReleaseEnvironmentId = deployStep.ReleaseEnvironmentId,
        ReleaseId = deployStep.ReleaseId
      };
      string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Deferred Deployment Job for ProjectId : {0} Release Id: {1}, Deploy Step Id: {2}", (object) projectIdentifier, (object) objectToSerialize.ReleaseId, (object) objectToSerialize.DeployStepId);
      return new TeamFoundationJobDefinition(deployStep.GetDeferredExecutionJobId(), name, "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.DeferredDeploymentExecutionJob", TeamFoundationSerializationUtility.SerializeToXml((object) objectToSerialize), TeamFoundationJobEnabledState.Enabled, true, JobPriorityClass.AboveNormal)
      {
        Schedule = {
          ReleaseEnvironmentExtensions.GetTeamFoundationJobSchedule(scheduledDeploymentTime)
        }
      };
    }

    public static IEnumerable<int> GetAllQueueIds(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment releaseEnvironment,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> releaseVariables)
    {
      if (releaseEnvironment == null || releaseEnvironment.DeployPhasesSnapshot == null)
        return Enumerable.Empty<int>();
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> mergedGroupVariables = VariableGroupsMerger.GetMergedGroupVariables(releaseEnvironment.VariableGroups);
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variables = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>[4]
      {
        (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) releaseEnvironment.ProcessParameters.GetProcessParametersAsWebContractVariables(),
        releaseEnvironment.Variables,
        mergedGroupVariables,
        releaseVariables
      });
      return releaseEnvironment.DeployPhasesSnapshot.Where<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, bool>) (dp => dp.PhaseType != Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.MachineGroupBasedDeployment)).Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, BaseDeploymentInput>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, BaseDeploymentInput>) (dp => dp.GetDeploymentInput(variables))).Where<BaseDeploymentInput>((Func<BaseDeploymentInput, bool>) (di => di != null)).Where<BaseDeploymentInput>((Func<BaseDeploymentInput, bool>) (di => di is DeploymentInput)).Select<BaseDeploymentInput, int>((Func<BaseDeploymentInput, int>) (di => (di as DeploymentInput).QueueId)).Where<int>((Func<int, bool>) (qid => qid > 0));
    }

    public static IEnumerable<int> GetAllQueueIds(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables)
    {
      if (releaseEnvironment == null)
        return Enumerable.Empty<int>();
      IList<DeployPhaseSnapshot> deployPhaseSnapshots = releaseEnvironment.GetDesignerDeployPhaseSnapshots();
      return deployPhaseSnapshots == null ? Enumerable.Empty<int>() : deployPhaseSnapshots.Where<DeployPhaseSnapshot>((Func<DeployPhaseSnapshot, bool>) (dp => dp.PhaseType != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.MachineGroupBasedDeployment)).Select<DeployPhaseSnapshot, BaseDeploymentInput>((Func<DeployPhaseSnapshot, BaseDeploymentInput>) (dp => dp.GetDeploymentInput(variables))).Where<BaseDeploymentInput>((Func<BaseDeploymentInput, bool>) (di => di != null)).Where<BaseDeploymentInput>((Func<BaseDeploymentInput, bool>) (di => di is DeploymentInput)).Select<BaseDeploymentInput, int>((Func<BaseDeploymentInput, int>) (di => (di as DeploymentInput).QueueId)).Where<int>((Func<int, bool>) (qid => qid > 0));
    }

    public static IEnumerable<int> GetAllMachineGroupIds(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment releaseEnvironment,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> releaseVariables)
    {
      if (releaseEnvironment == null || releaseEnvironment.DeployPhasesSnapshot == null)
        return Enumerable.Empty<int>();
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> mergedGroupVariables = VariableGroupsMerger.GetMergedGroupVariables(releaseEnvironment.VariableGroups);
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variables = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>[4]
      {
        (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) releaseEnvironment.ProcessParameters.GetProcessParametersAsWebContractVariables(),
        releaseEnvironment.Variables,
        mergedGroupVariables,
        releaseVariables
      });
      return releaseEnvironment.DeployPhasesSnapshot.Where<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, bool>) (dp => dp.PhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.MachineGroupBasedDeployment)).Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, BaseDeploymentInput>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, BaseDeploymentInput>) (dp => dp.GetDeploymentInput(variables))).Where<BaseDeploymentInput>((Func<BaseDeploymentInput, bool>) (di => di != null)).Where<BaseDeploymentInput>((Func<BaseDeploymentInput, bool>) (di => di is MachineGroupDeploymentInput)).Select<BaseDeploymentInput, int>((Func<BaseDeploymentInput, int>) (di => (di as MachineGroupDeploymentInput).QueueId)).Where<int>((Func<int, bool>) (qid => qid > 0));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentUpdateMetadata FromContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironmentUpdateMetadata releaseEnvironmentUpdateMetadata)
    {
      if (releaseEnvironmentUpdateMetadata == null)
        throw new ArgumentNullException(nameof (releaseEnvironmentUpdateMetadata));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentUpdateMetadata serveReleaseEnvironmentUpdateMetadata = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentUpdateMetadata()
      {
        Status = (ReleaseEnvironmentStatus) releaseEnvironmentUpdateMetadata.Status,
        Comment = releaseEnvironmentUpdateMetadata.Comment,
        ScheduledDeploymentTime = releaseEnvironmentUpdateMetadata.ScheduledDeploymentTime
      };
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variables = releaseEnvironmentUpdateMetadata.Variables;
      if (variables != null)
        variables.ForEach<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>((Action<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) (v => serveReleaseEnvironmentUpdateMetadata.Variables.Add(v.Key, v.Value.ToServerConfigurationVariableValue())));
      return serveReleaseEnvironmentUpdateMetadata;
    }

    public static void HandleCancelingStateBackCompatibility(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> releaseEnvironments)
    {
      if (releaseEnvironments == null)
        return;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment releaseEnvironment in releaseEnvironments)
        releaseEnvironment.HandleCancelingStateBackCompatibility();
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment HandleCancelingStateBackCompatibility(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment)
    {
      if (environment == null || environment.DeploySteps == null)
        return environment;
      DeploymentAttempt deploymentAttempt = (DeploymentAttempt) null;
      int num = -1;
      foreach (DeploymentAttempt deployStep in environment.DeploySteps)
      {
        if (deployStep.OperationStatus == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Cancelling)
        {
          deployStep.OperationStatus = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Canceled;
          if (deployStep.ReleaseDeployPhases != null)
          {
            foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase releaseDeployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase>) deployStep.ReleaseDeployPhases)
            {
              if (releaseDeployPhase.Status == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseStatus.Cancelling)
                releaseDeployPhase.Status = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseStatus.Canceled;
            }
          }
        }
        if (deployStep.Attempt > num)
        {
          deploymentAttempt = deployStep;
          num = deployStep.Attempt;
        }
      }
      if (deploymentAttempt != null && deploymentAttempt.OperationStatus == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Canceled && environment.Status == EnvironmentStatus.InProgress)
        environment.Status = EnvironmentStatus.Canceled;
      return environment;
    }

    public static void HandleGateCanceledStateBackCompatibility(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> releaseEnvironments)
    {
      if (releaseEnvironments == null)
        return;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment releaseEnvironment in releaseEnvironments)
        releaseEnvironment.HandleGateCanceledStateBackCompatibility();
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment HandleGateCanceledStateBackCompatibility(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment)
    {
      if (environment == null || environment.DeploySteps == null)
        return environment;
      foreach (DeploymentAttempt deployStep in environment.DeploySteps)
      {
        if (deployStep.PostDeploymentGates != null && deployStep.PostDeploymentGates.Status == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.GateStatus.Canceled)
          deployStep.PostDeploymentGates.Status = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.GateStatus.Failed;
        if (deployStep.PreDeploymentGates != null && deployStep.PreDeploymentGates.Status == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.GateStatus.Canceled)
          deployStep.PreDeploymentGates.Status = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.GateStatus.Failed;
      }
      return environment;
    }

    public static void FillDeployPhases(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverEnvironment,
      DeploymentAttempt webApiDeployment)
    {
      if (serverEnvironment == null)
        throw new ArgumentNullException(nameof (serverEnvironment));
      if (webApiDeployment == null)
        throw new ArgumentNullException(nameof (serverEnvironment));
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase releaseDeployPhase1 in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase>) serverEnvironment.ReleaseDeployPhases.FilterForAttempt(webApiDeployment.Attempt).OrderBy<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase, int>) (x => x.Rank)))
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase releaseDeployPhase = releaseDeployPhase1;
        IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention> manualInterventions = serverEnvironment.ManualInterventions.Where<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention, bool>) (mi => mi.ReleaseDeployPhaseId == releaseDeployPhase.Id));
        string phaseName = (string) null;
        if (serverEnvironment.DeploymentSnapshot is DesignerDeploymentSnapshot deploymentSnapshot)
          phaseName = deploymentSnapshot.GetPhaseName(releaseDeployPhase.Rank, releaseDeployPhase.PhaseType);
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment serverDeployment = serverEnvironment.DeploymentAttempts.SingleOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment, bool>) (d => d.Attempt == webApiDeployment.Attempt));
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase releaseDeployPhase2 = releaseDeployPhase.ToWebApiReleaseDeployPhase(manualInterventions, requestContext, projectId, phaseName, serverDeployment);
        webApiDeployment.ReleaseDeployPhases.Add(releaseDeployPhase2);
      }
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment RemoveNonLatestDeploymentAttemptData(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment)
    {
      if (releaseEnvironment == null)
        return (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment) null;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment1 = releaseEnvironment.DeepClone();
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment latestDeployment = releaseEnvironment.GetLatestDeployment();
      if (latestDeployment != null)
      {
        int deploymentAttempt = latestDeployment.Attempt;
        IEnumerable<ReleaseEnvironmentStep> values1 = releaseEnvironment.GetStepsForTests.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.TrialNumber == deploymentAttempt));
        List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase> list = releaseEnvironment.ReleaseDeployPhases.Where<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase, bool>) (phase => phase.Attempt == deploymentAttempt)).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase>();
        HashSet<int> validPhaseIds = list.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase, int>) (e => e.Id)).ToHashSet<int>();
        IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention> values2 = releaseEnvironment.ManualInterventions.Where<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention, bool>) (mi => validPhaseIds.Contains(mi.ReleaseDeployPhaseId)));
        releaseEnvironment1.DeploymentAttempts.Clear();
        releaseEnvironment1.DeploymentAttempts.Add(latestDeployment);
        releaseEnvironment1.GetStepsForTests.Clear();
        releaseEnvironment1.GetStepsForTests.AddRange<ReleaseEnvironmentStep, IList<ReleaseEnvironmentStep>>(values1);
        releaseEnvironment1.ReleaseDeployPhases.Clear();
        releaseEnvironment1.ReleaseDeployPhases.AddRange<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase, IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase>>((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase>) list);
        releaseEnvironment1.ManualInterventions.Clear();
        releaseEnvironment1.ManualInterventions.AddRange<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention, IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention>>(values2);
      }
      return releaseEnvironment1;
    }

    private static TeamFoundationJobSchedule GetTeamFoundationJobSchedule(DateTime dateTimeUtc) => new TeamFoundationJobSchedule()
    {
      Interval = 0,
      PriorityLevel = JobPriorityLevel.Highest,
      ScheduledTime = dateTimeUtc,
      TimeZoneId = TimeZoneInfo.Utc.Id
    };
  }
}
