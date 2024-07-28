// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseDefinitionEnvironmentExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ReleaseDefinitionEnvironmentExtensions
  {
    public static ReleaseDefinitionApprovals ToWebApiDefinitionEnvironmentApprovals(
      this DefinitionEnvironment serverEnvironment,
      EnvironmentStepType stepType)
    {
      IEnumerable<DefinitionEnvironmentStep> source = serverEnvironment != null ? serverEnvironment.GetSteps(stepType) : throw new ArgumentNullException(nameof (serverEnvironment));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions approvalOptions;
      List<ReleaseDefinitionApprovalStep> list;
      if (stepType == EnvironmentStepType.PreDeploy)
      {
        int deploymentGatesStepRank = serverEnvironment.PreDeploymentGates.GetDeploymentGatesStepRank(serverEnvironment.GetSteps(EnvironmentStepType.PreDeploy), serverEnvironment.PreApprovalOptions, 0);
        approvalOptions = serverEnvironment.PreApprovalOptions;
        int rankOffset = (approvalOptions != null ? approvalOptions.ExecutionOrder : ApprovalExecutionOrder.BeforeGates) == ApprovalExecutionOrder.BeforeGates ? 0 : deploymentGatesStepRank;
        list = source.Select<DefinitionEnvironmentStep, ReleaseDefinitionApprovalStep>((Func<DefinitionEnvironmentStep, ReleaseDefinitionApprovalStep>) (s => ReleaseDefinitionEnvironmentExtensions.ConvertToWebApiStep(s, rankOffset))).ToList<ReleaseDefinitionApprovalStep>();
      }
      else
      {
        int rank = serverEnvironment.GetSteps(EnvironmentStepType.Deploy).Single<DefinitionEnvironmentStep>().Rank;
        int deploymentGatesStepRank = serverEnvironment.PostDeploymentGates.GetDeploymentGatesStepRank(serverEnvironment.GetSteps(EnvironmentStepType.PostDeploy), serverEnvironment.PostApprovalOptions, rank);
        approvalOptions = serverEnvironment.PostApprovalOptions;
        int rankOffset = (approvalOptions != null ? approvalOptions.ExecutionOrder : ApprovalExecutionOrder.BeforeGates) == ApprovalExecutionOrder.BeforeGates ? rank : deploymentGatesStepRank;
        list = source.Select<DefinitionEnvironmentStep, ReleaseDefinitionApprovalStep>((Func<DefinitionEnvironmentStep, ReleaseDefinitionApprovalStep>) (s => ReleaseDefinitionEnvironmentExtensions.ConvertToWebApiStep(s, rankOffset))).ToList<ReleaseDefinitionApprovalStep>();
      }
      return new ReleaseDefinitionApprovals()
      {
        Approvals = list,
        ApprovalOptions = approvalOptions.ToWebApiApprovalOptions()
      };
    }

    public static IList<DefinitionEnvironmentStep> FromWebApiDefinitionEnvironmentApprovals(
      this ReleaseDefinitionEnvironment releaseDefinitionEnvironment,
      EnvironmentStepType stepType,
      int rankOffset)
    {
      if (releaseDefinitionEnvironment == null)
        throw new ArgumentNullException(nameof (releaseDefinitionEnvironment));
      return (IList<DefinitionEnvironmentStep>) ReleaseDefinitionEnvironmentExtensions.GetDefinitionEnvironmentApprovals(releaseDefinitionEnvironment, stepType).Select<ReleaseDefinitionApprovalStep, DefinitionEnvironmentStep>((Func<ReleaseDefinitionApprovalStep, DefinitionEnvironmentStep>) (s => ReleaseDefinitionEnvironmentExtensions.ConvertToServerStep(s, stepType, rankOffset))).ToList<DefinitionEnvironmentStep>();
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions GetDefinitionEnvironmentApprovalOptions(
      ReleaseDefinitionEnvironment releaseDefinitionEnvironment,
      EnvironmentStepType stepType)
    {
      if (releaseDefinitionEnvironment == null)
        throw new ArgumentNullException(nameof (releaseDefinitionEnvironment));
      switch (stepType)
      {
        case EnvironmentStepType.PreDeploy:
          return releaseDefinitionEnvironment.PreDeployApprovals == null ? (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions) null : releaseDefinitionEnvironment.PreDeployApprovals.ApprovalOptions;
        case EnvironmentStepType.PostDeploy:
          return releaseDefinitionEnvironment.PostDeployApprovals == null ? (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions) null : releaseDefinitionEnvironment.PostDeployApprovals.ApprovalOptions;
        default:
          return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions) null;
      }
    }

    public static IList<ReleaseDefinitionApprovalStep> GetDefinitionEnvironmentApprovals(
      ReleaseDefinitionEnvironment releaseDefinitionEnvironment,
      EnvironmentStepType stepType)
    {
      if (releaseDefinitionEnvironment == null)
        throw new ArgumentNullException(nameof (releaseDefinitionEnvironment));
      List<ReleaseDefinitionApprovalStep> environmentApprovals = new List<ReleaseDefinitionApprovalStep>();
      switch (stepType)
      {
        case EnvironmentStepType.PreDeploy:
          environmentApprovals = releaseDefinitionEnvironment.PreDeployApprovals != null ? releaseDefinitionEnvironment.PreDeployApprovals.Approvals : environmentApprovals;
          break;
        case EnvironmentStepType.PostDeploy:
          environmentApprovals = releaseDefinitionEnvironment.PostDeployApprovals != null ? releaseDefinitionEnvironment.PostDeployApprovals.Approvals : environmentApprovals;
          break;
      }
      return (IList<ReleaseDefinitionApprovalStep>) environmentApprovals;
    }

    public static IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions> GetPreAndPostApprovalOptionsFromDefinitionEnvironment(
      this DefinitionEnvironment definitionEnvironment)
    {
      if (definitionEnvironment == null)
        throw new ArgumentNullException(nameof (definitionEnvironment));
      Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions> definitionEnvironment1 = new Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions>();
      if (definitionEnvironment.PreApprovalOptions != null)
        definitionEnvironment1.Add("pre", definitionEnvironment.PreApprovalOptions);
      if (definitionEnvironment.PostApprovalOptions != null)
        definitionEnvironment1.Add("post", definitionEnvironment.PostApprovalOptions);
      return (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions>) definitionEnvironment1;
    }

    public static void PopulateDefinitionPreAndPostApprovalOptions(
      this DefinitionEnvironment definitionEnvironment,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions> approvalOptions)
    {
      if (definitionEnvironment == null)
        throw new ArgumentNullException(nameof (definitionEnvironment));
      if (approvalOptions == null)
        return;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions approvalOptions1;
      if (approvalOptions.TryGetValue("pre", out approvalOptions1))
        definitionEnvironment.PreApprovalOptions = approvalOptions1;
      if (!approvalOptions.TryGetValue("post", out approvalOptions1))
        return;
      definitionEnvironment.PostApprovalOptions = approvalOptions1;
    }

    public static IDictionary<string, ReleaseDefinitionGatesStep> GetDefinitionGates(
      this DefinitionEnvironment definitionEnvironment)
    {
      if (definitionEnvironment == null)
        throw new ArgumentNullException(nameof (definitionEnvironment));
      Dictionary<string, ReleaseDefinitionGatesStep> definitionGates = new Dictionary<string, ReleaseDefinitionGatesStep>();
      if (definitionEnvironment.PreDeploymentGates != null)
        definitionGates.Add("pre", definitionEnvironment.PreDeploymentGates);
      if (definitionEnvironment.PostDeploymentGates != null)
        definitionGates.Add("post", definitionEnvironment.PostDeploymentGates);
      return (IDictionary<string, ReleaseDefinitionGatesStep>) definitionGates;
    }

    public static void PopulateDefinitionGates(
      this DefinitionEnvironment definitionEnvironment,
      IDictionary<string, ReleaseDefinitionGatesStep> definitionEnvironmentGates)
    {
      if (definitionEnvironment == null)
        throw new ArgumentNullException(nameof (definitionEnvironment));
      if (definitionEnvironmentGates == null)
        return;
      ReleaseDefinitionGatesStep definitionGatesStep;
      if (definitionEnvironmentGates.TryGetValue("pre", out definitionGatesStep))
        definitionEnvironment.PreDeploymentGates = definitionGatesStep;
      if (!definitionEnvironmentGates.TryGetValue("post", out definitionGatesStep))
        return;
      definitionEnvironment.PostDeploymentGates = definitionGatesStep;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentRetentionPolicy ConvertToServerEnvironmentRetentionPolicy(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.EnvironmentRetentionPolicy retentionPolicy)
    {
      if (retentionPolicy == null)
        throw new ArgumentNullException(nameof (retentionPolicy));
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentRetentionPolicy()
      {
        DaysToKeep = retentionPolicy.DaysToKeep,
        ReleasesToKeep = retentionPolicy.ReleasesToKeep,
        RetainBuild = retentionPolicy.RetainBuild
      };
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.EnvironmentRetentionPolicy ConvertToWebApiEnvironmentRetentionPolicy(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentRetentionPolicy retentionPolicy)
    {
      if (retentionPolicy == null)
        throw new ArgumentNullException(nameof (retentionPolicy));
      return new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.EnvironmentRetentionPolicy()
      {
        DaysToKeep = retentionPolicy.DaysToKeep,
        ReleasesToKeep = retentionPolicy.ReleasesToKeep,
        RetainBuild = retentionPolicy.RetainBuild
      };
    }

    public static IEnumerable<int> GetAllQueueIds(
      this ReleaseDefinitionEnvironment releaseDefinitionEnvironment,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> releaseDefinitionVariables,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> allVariableGroupsInRD)
    {
      if (releaseDefinitionEnvironment == null || releaseDefinitionEnvironment.DeployPhases == null)
        return Enumerable.Empty<int>();
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variableGroupVariables = ReleaseDefinitionVariableGroupUtility.GetVariableGroupVariables(releaseDefinitionEnvironment.VariableGroups, allVariableGroupsInRD);
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variables = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>[4]
      {
        (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) releaseDefinitionEnvironment.ProcessParameters.GetProcessParametersAsWebContractVariables(),
        releaseDefinitionEnvironment.Variables,
        variableGroupVariables,
        releaseDefinitionVariables
      });
      return releaseDefinitionEnvironment.DeployPhases.Where<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, bool>) (dp => dp.PhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.AgentBasedDeployment)).Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, BaseDeploymentInput>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, BaseDeploymentInput>) (dp => dp.GetDeploymentInput(variables))).Where<BaseDeploymentInput>((Func<BaseDeploymentInput, bool>) (di => di != null)).Where<BaseDeploymentInput>((Func<BaseDeploymentInput, bool>) (di => di is DeploymentInput)).Select<BaseDeploymentInput, int>((Func<BaseDeploymentInput, int>) (di => (di as DeploymentInput).QueueId)).Where<int>((Func<int, bool>) (qid => qid > 0));
    }

    public static IEnumerable<int> GetAllMachineGroupIds(
      this ReleaseDefinitionEnvironment releaseDefinitionEnvironment,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> releaseDefinitionVariables,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> allVariableGroupsInRD)
    {
      if (releaseDefinitionEnvironment == null || releaseDefinitionEnvironment.DeployPhases == null)
        return Enumerable.Empty<int>();
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variableGroupVariables = ReleaseDefinitionVariableGroupUtility.GetVariableGroupVariables(releaseDefinitionEnvironment.VariableGroups, allVariableGroupsInRD);
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variables = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>[4]
      {
        (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) releaseDefinitionEnvironment.ProcessParameters.GetProcessParametersAsWebContractVariables(),
        releaseDefinitionEnvironment.Variables,
        variableGroupVariables,
        releaseDefinitionVariables
      });
      return releaseDefinitionEnvironment.DeployPhases.Where<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, bool>) (dp => dp.PhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.MachineGroupBasedDeployment)).Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, BaseDeploymentInput>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, BaseDeploymentInput>) (dp => dp.GetDeploymentInput(variables))).Where<BaseDeploymentInput>((Func<BaseDeploymentInput, bool>) (di => di != null)).Where<BaseDeploymentInput>((Func<BaseDeploymentInput, bool>) (di => di is MachineGroupDeploymentInput)).Select<BaseDeploymentInput, int>((Func<BaseDeploymentInput, int>) (di => (di as MachineGroupDeploymentInput).QueueId)).Where<int>((Func<int, bool>) (qid => qid > 0));
    }

    public static bool HasMetaTask(
      this ReleaseDefinitionEnvironment releaseDefinitionEnvironment)
    {
      if (releaseDefinitionEnvironment == null)
        throw new ArgumentNullException(nameof (releaseDefinitionEnvironment));
      return releaseDefinitionEnvironment.DeployPhases.Where<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, bool>) (phase => phase != null)).SelectMany<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, WorkflowTask>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, IEnumerable<WorkflowTask>>) (phase => (IEnumerable<WorkflowTask>) phase.WorkflowTasks)).Any<WorkflowTask>((Func<WorkflowTask, bool>) (task => task.IsMetaTask()));
    }

    public static void ValidateWorkflowTasks(
      this ReleaseDefinitionEnvironment definitionEnvironment,
      IVssRequestContext context,
      Guid projectId)
    {
      List<TaskDefinition> allTaskDefinitions = TaskDefinitionsHelper.GetAllTaskDefinitions(context, projectId, definitionEnvironment.HasMetaTask());
      definitionEnvironment.DeployPhases.ValidateTasks(definitionEnvironment.Name, (IList<TaskDefinition>) allTaskDefinitions);
    }

    private static ReleaseDefinitionApprovalStep ConvertToWebApiStep(
      DefinitionEnvironmentStep serverStep,
      int rankOffset)
    {
      ReleaseDefinitionApprovalStep webApiStep = new ReleaseDefinitionApprovalStep();
      webApiStep.Id = serverStep.Id;
      webApiStep.Rank = serverStep.Rank - rankOffset;
      webApiStep.IsAutomated = serverStep.IsAutomated;
      webApiStep.IsNotificationOn = serverStep.IsNotificationOn;
      webApiStep.Approver = new IdentityRef()
      {
        Id = serverStep.ApproverId.ToString()
      };
      return webApiStep;
    }

    private static DefinitionEnvironmentStep ConvertToServerStep(
      ReleaseDefinitionApprovalStep approvalStep,
      EnvironmentStepType stepType,
      int rankOffset)
    {
      return new DefinitionEnvironmentStep()
      {
        Id = approvalStep.Id,
        Rank = rankOffset + approvalStep.Rank,
        IsAutomated = approvalStep.IsAutomated,
        IsNotificationOn = approvalStep.IsNotificationOn,
        ApproverId = approvalStep.IsAutomated ? Guid.Empty : new Guid(approvalStep.Approver.Id),
        StepType = stepType
      };
    }
  }
}
