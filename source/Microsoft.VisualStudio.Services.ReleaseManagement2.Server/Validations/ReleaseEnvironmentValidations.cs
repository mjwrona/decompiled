// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations.ReleaseEnvironmentValidations
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations
{
  public static class ReleaseEnvironmentValidations
  {
    public static void Validate(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment webApiEnvironment,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release parentRelease,
      IVssRequestContext context)
    {
      if (webApiEnvironment == null)
        throw new ArgumentNullException(nameof (webApiEnvironment));
      if (parentRelease == null)
        throw new ArgumentNullException(nameof (parentRelease));
      webApiEnvironment.PreDeploymentGatesSnapshot.ValidateGateStep(context, Resources.PreDeploymentGatesText, webApiEnvironment.Name);
      webApiEnvironment.PostDeploymentGatesSnapshot.ValidateGateStep(context, Resources.PostDeploymentGatesText, webApiEnvironment.Name);
      webApiEnvironment.EnvironmentOptions.Validate();
      ReleaseEnvironmentValidations.ValidateDeployPhasesSnapshot(webApiEnvironment, parentRelease, context);
      ReleaseEnvironmentValidations.ValidateApprovalOptions(webApiEnvironment.PreApprovalsSnapshot, ApprovalType.PreDeploy, webApiEnvironment.Name);
      ReleaseEnvironmentValidations.ValidateApprovalOptions(webApiEnvironment.PostApprovalsSnapshot, ApprovalType.PostDeploy, webApiEnvironment.Name);
    }

    public static void ValidateDeployPhaseSnapshotsAreNotModified(
      string webApiEnvironmentName,
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase> webApiDeployPhases,
      IList<DeployPhaseSnapshot> serverDeployPhases,
      bool checkOnlyWorkflow)
    {
      if (webApiEnvironmentName == null)
        throw new ArgumentNullException(nameof (webApiEnvironmentName));
      if (webApiDeployPhases == null)
        throw new ArgumentNullException(nameof (webApiDeployPhases));
      if (serverDeployPhases == null)
        throw new ArgumentNullException(nameof (serverDeployPhases));
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase webApiDeployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) webApiDeployPhases)
        ReleaseEnvironmentValidations.ValidateDeployPhaseSnapshotIsNotModified(webApiDeployPhase, serverDeployPhases, webApiEnvironmentName, checkOnlyWorkflow);
    }

    public static void WorkflowTasksShouldNotBeAddedOrRemoved(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase webApiDeployPhaseSnapshot,
      DeployPhaseSnapshot serverDeployPhaseSnapshot,
      string webApiEnvironmentName)
    {
      if (webApiDeployPhaseSnapshot == null)
        throw new ArgumentNullException(nameof (webApiDeployPhaseSnapshot));
      if (serverDeployPhaseSnapshot == null)
        throw new ArgumentNullException(nameof (serverDeployPhaseSnapshot));
      if (!ReleaseEnvironmentValidations.AreWorkflowsSame(webApiDeployPhaseSnapshot.WorkflowTasks, serverDeployPhaseSnapshot.Workflow.ToList<WorkflowTask>(), true))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDeployPhasesSnapshotTasksCannotBeAddedOrDeletedOrRenamed, (object) webApiDeployPhaseSnapshot.Name, (object) webApiEnvironmentName));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Will refactor later")]
    public static void EnvironmentShouldNotBeModified(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment webApiEnvironment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverEnvironment,
      DefinitionEnvironmentData serverEnvironmentSnapshotData)
    {
      if (webApiEnvironment == null)
        throw new ArgumentNullException(nameof (webApiEnvironment));
      if (serverEnvironment == null)
        throw new ArgumentNullException(nameof (serverEnvironment));
      List<string> stringList = new List<string>();
      if (ReleaseEnvironmentValidations.AreVariablesModified(webApiEnvironment.Variables, serverEnvironment.Variables))
        stringList.Add(ReleaseEnvironmentValidations.GetPropertyName<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>((Expression<Func<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>>) (() => webApiEnvironment.Variables)));
      if (ReleaseEnvironmentValidations.AreVariableGroupsModified(webApiEnvironment.VariableGroups, serverEnvironment.VariableGroups))
        stringList.Add(ReleaseEnvironmentValidations.GetPropertyName<IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup>>((Expression<Func<IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup>>>) (() => webApiEnvironment.VariableGroups)));
      if (ReleaseEnvironmentValidations.AreEnvironmentOptionsModified(webApiEnvironment.EnvironmentOptions, EnvironmentOptionsConverter.ToWebApiEnvironmentOptions(serverEnvironment.EnvironmentOptions)))
        stringList.Add(ReleaseEnvironmentValidations.GetPropertyName<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions>((Expression<Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions>>) (() => webApiEnvironment.EnvironmentOptions)));
      ReleaseEnvironmentValidations.AreSnapshotApprovalsModified(webApiEnvironment, serverEnvironmentSnapshotData, stringList);
      IEnumerable<DefinitionEnvironmentStepData> serverApprovals1 = serverEnvironmentSnapshotData.Steps.Where<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (s => s.StepType == EnvironmentStepType.PreDeploy));
      if (ReleaseEnvironmentValidations.AreApprovalOptionsModified(webApiEnvironment.PreApprovalsSnapshot, serverEnvironment.PreApprovalOptions, serverApprovals1))
        stringList.Add(ReleaseEnvironmentValidations.GetPropertyName<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions>((Expression<Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions>>) (() => serverEnvironment.PreApprovalOptions)));
      IEnumerable<DefinitionEnvironmentStepData> serverApprovals2 = serverEnvironmentSnapshotData.Steps.Where<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (s => s.StepType == EnvironmentStepType.PostDeploy));
      if (ReleaseEnvironmentValidations.AreApprovalOptionsModified(webApiEnvironment.PostApprovalsSnapshot, serverEnvironment.PostApprovalOptions, serverApprovals2))
        stringList.Add(ReleaseEnvironmentValidations.GetPropertyName<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions>((Expression<Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions>>) (() => serverEnvironment.PostApprovalOptions)));
      if (!webApiEnvironment.PreDeploymentGatesSnapshot.AreReleaseDefinitionGatesEqual(serverEnvironment.PreDeploymentGates))
        stringList.Add(ReleaseEnvironmentValidations.GetPropertyName<ReleaseDefinitionGatesStep>((Expression<Func<ReleaseDefinitionGatesStep>>) (() => serverEnvironment.PreDeploymentGates)));
      if (!webApiEnvironment.PostDeploymentGatesSnapshot.AreReleaseDefinitionGatesEqual(serverEnvironment.PostDeploymentGates))
        stringList.Add(ReleaseEnvironmentValidations.GetPropertyName<ReleaseDefinitionGatesStep>((Expression<Func<ReleaseDefinitionGatesStep>>) (() => serverEnvironment.PostDeploymentGates)));
      if (!stringList.IsNullOrEmpty<string>())
      {
        stringList.Sort();
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.EnvironmentCannotBeModified, (object) webApiEnvironment.Name, (object) string.Join(", ", (IEnumerable<string>) stringList)));
      }
      ReleaseEnvironmentValidations.ValidateProcessParameters(webApiEnvironment.ProcessParameters, serverEnvironment.ProcessParameters, webApiEnvironment.Name);
      ReleaseEnvironmentValidations.ValidateDeployPhaseSnapshotsAreNotModified(webApiEnvironment.Name, webApiEnvironment.DeployPhasesSnapshot, serverEnvironment.GetDesignerDeployPhaseSnapshots(), false);
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This method is supposed to return true/false only")]
    public static bool AreEnvironmentsSame(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment webApiEnvironment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverEnvironment,
      DefinitionEnvironmentData serverEnvironmentSnapshotData)
    {
      if (webApiEnvironment == null)
        throw new ArgumentNullException(nameof (webApiEnvironment));
      if (serverEnvironment == null)
        throw new ArgumentNullException(nameof (serverEnvironment));
      try
      {
        ReleaseEnvironmentValidations.EnvironmentShouldNotBeModified(webApiEnvironment, serverEnvironment, serverEnvironmentSnapshotData);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static bool AreApprovalOptionsModified(
      ReleaseDefinitionApprovals approvals,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions serverApprovalOptions,
      IEnumerable<DefinitionEnvironmentStepData> serverApprovals)
    {
      if (approvals == null)
        return serverApprovalOptions != null;
      bool isNewApprovalAutomated = approvals.Approvals.Any<ReleaseDefinitionApprovalStep>() && approvals.Approvals.First<ReleaseDefinitionApprovalStep>().IsAutomated;
      bool isExistingApprovalAutomated = serverApprovals.Any<DefinitionEnvironmentStepData>() && serverApprovals.First<DefinitionEnvironmentStepData>().IsAutomated;
      return ReleaseDefinitionApproverValidations.CompareApprovalOptions(approvals.ApprovalOptions, serverApprovalOptions.ToWebApiApprovalOptions(), isNewApprovalAutomated, isExistingApprovalAutomated);
    }

    public static void ValidateDeployPhaseSnapshotImmutablePropertyIsNotModified(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase webApiDeployPhaseSnapshot,
      IEnumerable<DeployPhaseSnapshot> serverDeployPhaseSnapshots,
      string webApiEnvironmentName)
    {
      if (webApiDeployPhaseSnapshot == null)
        throw new ArgumentNullException(nameof (webApiDeployPhaseSnapshot));
      if (serverDeployPhaseSnapshots == null)
        throw new ArgumentNullException(nameof (serverDeployPhaseSnapshots));
      List<DeployPhaseSnapshot> list = serverDeployPhaseSnapshots.Where<DeployPhaseSnapshot>((Func<DeployPhaseSnapshot, bool>) (sdps => sdps.Rank == webApiDeployPhaseSnapshot.Rank)).ToList<DeployPhaseSnapshot>();
      if (list.Count != 1)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDeployPhasesSnapshotNameAndRankCannotBeChanged, (object) webApiDeployPhaseSnapshot.Name, (object) webApiEnvironmentName));
      if (!ReleaseEnvironmentValidations.IsSamePhaseType(webApiDeployPhaseSnapshot.PhaseType, list.Single<DeployPhaseSnapshot>().PhaseType))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDeployPhasesSnapshotPhaseTypeCannotBeModified, (object) webApiDeployPhaseSnapshot.Name, (object) webApiEnvironmentName));
      ReleaseEnvironmentValidations.ValidateDeployPhaseNameIsNotModified(webApiDeployPhaseSnapshot.Name, list.Single<DeployPhaseSnapshot>().Name, webApiEnvironmentName);
    }

    private static void ValidateProcessParameters(
      ProcessParameters webApiProcessParameters,
      ProcessParameters serverProcessParameters,
      string webApiEnvironmentName)
    {
      ProcessParameters processParameters = new ProcessParameters();
      if ((webApiProcessParameters != null || serverProcessParameters != null) && (webApiProcessParameters != null || serverProcessParameters == null || !serverProcessParameters.Equals((object) processParameters)) && (serverProcessParameters != null || webApiProcessParameters == null || !webApiProcessParameters.Equals((object) processParameters)) && (webApiProcessParameters == null || serverProcessParameters == null || !webApiProcessParameters.Equals((object) serverProcessParameters)))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProcessParametersCannotBeModified, (object) webApiEnvironmentName));
    }

    private static void ValidateDeployPhaseSnapshotIsNotModified(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase webApiDeployPhaseSnapshot,
      IList<DeployPhaseSnapshot> serverDeployPhaseSnapshots,
      string webApiEnvironmentName,
      bool checkOnlyWorkflow)
    {
      DeployPhaseSnapshot serverDeployPhase = serverDeployPhaseSnapshots.Single<DeployPhaseSnapshot>((Func<DeployPhaseSnapshot, bool>) (sdps => sdps.Rank == webApiDeployPhaseSnapshot.Rank));
      if (!checkOnlyWorkflow)
      {
        ReleaseEnvironmentValidations.ValidateDeployPhaseNameIsNotModified(webApiDeployPhaseSnapshot.Name, serverDeployPhase.Name, webApiEnvironmentName);
        webApiDeployPhaseSnapshot.ValidateDeploymentInputIsNotModified(serverDeployPhase);
      }
      if (!ReleaseEnvironmentValidations.AreWorkflowsSame(webApiDeployPhaseSnapshot.WorkflowTasks, serverDeployPhase.Workflow.ToList<WorkflowTask>(), false))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDeployPhasesSnapshotTasksCannotBeModified, (object) webApiDeployPhaseSnapshot.Name, (object) webApiEnvironmentName, (object) "WorkflowTasks"));
    }

    private static void ValidateDeployPhasesSnapshot(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release parentRelease,
      IVssRequestContext context)
    {
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase> deployPhasesSnapshot = environment.DeployPhasesSnapshot;
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> mergedGroupVariables1 = VariableGroupsMerger.GetMergedGroupVariables(parentRelease.VariableGroups);
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> mergedGroupVariables2 = VariableGroupsMerger.GetMergedGroupVariables(environment.VariableGroups);
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variables = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>[5]
      {
        (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) environment.ProcessParameters.GetProcessParametersAsWebContractVariables(),
        environment.Variables,
        mergedGroupVariables2,
        parentRelease.Variables,
        mergedGroupVariables1
      });
      deployPhasesSnapshot.ValidatePhasesRefName();
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) deployPhasesSnapshot)
      {
        deployPhase.ValidateWorkflow(environment.Name, variables, context);
        deployPhase.ValidateDeploymentInput(variables, parentRelease.Artifacts, context);
        deployPhase.ValidatePhaseCondition(context, environment.Name);
      }
    }

    private static void ValidateDeployPhaseNameIsNotModified(
      string webApiDeployPhaseName,
      string serverDeployPhaseName,
      string webApiEnvironmentName)
    {
      if (!string.Equals(webApiDeployPhaseName, serverDeployPhaseName))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDeployPhasesSnapshotNameCannotBeModified, (object) webApiDeployPhaseName, (object) webApiEnvironmentName, (object) "Name"));
      if (string.IsNullOrWhiteSpace(webApiDeployPhaseName))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeployPhaseNameCannotBeEmpty, (object) webApiEnvironmentName, (object) "Name"));
    }

    private static void ValidateApprovalOptions(
      ReleaseDefinitionApprovals approvalsSnapshot,
      ApprovalType approvalType,
      string environmentName)
    {
      if (approvalsSnapshot == null || approvalsSnapshot.ApprovalOptions == null)
        return;
      List<ReleaseDefinitionApprovalStep> approvals = approvalsSnapshot.Approvals;
      if (approvals == null || !approvals.Any<ReleaseDefinitionApprovalStep>())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ApprovalOptionsShouldBeNull, approvalType == ApprovalType.PreDeploy ? (object) Resources.PreApprovals : (object) Resources.PostApprovals, (object) environmentName));
      ReleaseDefinitionApproverValidations.ValidateApprovalOptions(approvalsSnapshot.ApprovalOptions, approvals.Count<ReleaseDefinitionApprovalStep>(), approvalType, environmentName);
    }

    private static void AreSnapshotApprovalsModified(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment webApiEnvironment,
      DefinitionEnvironmentData serverEnvironmentSnapshotData,
      List<string> modifiedProperties)
    {
      IEnumerable<DefinitionEnvironmentStepData> environmentSteps1 = serverEnvironmentSnapshotData.GetDefinitionEnvironmentSteps(EnvironmentStepType.PreDeploy);
      int deploymentGatesStepRank1 = webApiEnvironment.PreDeploymentGatesSnapshot.GetDeploymentGatesStepRank(webApiEnvironment.PreApprovalsSnapshot, 0);
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions approvalOptions1 = webApiEnvironment.PreApprovalsSnapshot.ApprovalOptions;
      int deployStepRank1 = (approvalOptions1 != null ? (int) approvalOptions1.ExecutionOrder : 1) == 1 ? 0 : deploymentGatesStepRank1;
      if (ReleaseEnvironmentValidations.AreApprovalsModified(webApiEnvironment.PreApprovalsSnapshot, environmentSteps1, deployStepRank1))
      {
        string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}", (object) Resources.PreApprovals);
        modifiedProperties.Add(str);
      }
      DefinitionEnvironmentStepData environmentStepData = serverEnvironmentSnapshotData.GetDefinitionEnvironmentSteps(EnvironmentStepType.Deploy).SingleOrDefault<DefinitionEnvironmentStepData>();
      IEnumerable<DefinitionEnvironmentStepData> environmentSteps2 = serverEnvironmentSnapshotData.GetDefinitionEnvironmentSteps(EnvironmentStepType.PostDeploy);
      int offset = 0;
      if (environmentStepData != null)
        offset = environmentStepData.Rank;
      int deploymentGatesStepRank2 = webApiEnvironment.PostDeploymentGatesSnapshot.GetDeploymentGatesStepRank(webApiEnvironment.PostApprovalsSnapshot, offset);
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions approvalOptions2 = webApiEnvironment.PostApprovalsSnapshot.ApprovalOptions;
      int deployStepRank2 = (approvalOptions2 != null ? (int) approvalOptions2.ExecutionOrder : 1) == 1 ? offset : deploymentGatesStepRank2;
      if (!ReleaseEnvironmentValidations.AreApprovalsModified(webApiEnvironment.PostApprovalsSnapshot, environmentSteps2, deployStepRank2))
        return;
      string str1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}", (object) Resources.PostApprovals);
      modifiedProperties.Add(str1);
    }

    private static bool AreApprovalsModified(
      ReleaseDefinitionApprovals releaseDefinitionApprovals,
      IEnumerable<DefinitionEnvironmentStepData> snapshotSteps,
      int deployStepRank)
    {
      if (releaseDefinitionApprovals == null || releaseDefinitionApprovals.Approvals == null || releaseDefinitionApprovals.Approvals.Count<ReleaseDefinitionApprovalStep>() != snapshotSteps.Count<DefinitionEnvironmentStepData>())
        return true;
      foreach (ReleaseDefinitionApprovalStep approval in releaseDefinitionApprovals.Approvals)
      {
        if (!ReleaseEnvironmentValidations.IsApprovalPresent(approval, snapshotSteps, deployStepRank))
          return true;
      }
      return false;
    }

    private static bool IsApprovalPresent(
      ReleaseDefinitionApprovalStep approval,
      IEnumerable<DefinitionEnvironmentStepData> approvals,
      int deployStepRank)
    {
      if (approvals == null)
        return false;
      if (approval.IsAutomated)
      {
        DefinitionEnvironmentStepData environmentStepData = approvals.FirstOrDefault<DefinitionEnvironmentStepData>();
        return environmentStepData != null && environmentStepData.IsAutomated;
      }
      if (approval.Approver == null)
        return false;
      DefinitionEnvironmentStepData environmentStepData1 = approvals.FirstOrDefault<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (s => string.Equals(s.ApproverId.ToString(), approval.Approver.Id, StringComparison.OrdinalIgnoreCase)));
      if (environmentStepData1 == null)
        return false;
      int num = environmentStepData1.Rank - deployStepRank;
      return approval.IsAutomated == environmentStepData1.IsAutomated && approval.IsNotificationOn == environmentStepData1.IsNotificationOn && approval.Rank == num;
    }

    private static bool AreWorkflowsSame(
      IList<WorkflowTask> webApiWorkflowTasks,
      List<WorkflowTask> serverWorkflow,
      bool ignoreTaskInputsTaskNameAndControlOptions)
    {
      RunOnServerDeployPhase serverDeployPhase1 = new RunOnServerDeployPhase();
      serverDeployPhase1.WorkflowTasks = (IList<WorkflowTask>) webApiWorkflowTasks.DeepClone();
      RunOnServerDeployPhase webApiDeployPhase = serverDeployPhase1;
      RunOnServerDeployPhase serverDeployPhase2 = new RunOnServerDeployPhase();
      serverDeployPhase2.WorkflowTasks = (IList<WorkflowTask>) serverWorkflow;
      RunOnServerDeployPhase other = serverDeployPhase2;
      if (ignoreTaskInputsTaskNameAndControlOptions)
        return webApiDeployPhase.AreWorkflowTasksEqual((Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase) other, true);
      return webApiDeployPhase.AreWorkflowTasksEqual((Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase) other, true) && other.WorkflowTasks.All<WorkflowTask>((Func<WorkflowTask, bool>) (t1 => webApiDeployPhase.WorkflowTasks.Any<WorkflowTask>((Func<WorkflowTask, bool>) (t2 => WorkflowTask.EqualsAndOldTaskInputsSubsetNewTaskInputs(t1, t2)))));
    }

    private static bool AreEnvironmentOptionsModified(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions webApiEnvironmentOptions,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions serverEnvironmentOptions)
    {
      return webApiEnvironmentOptions == serverEnvironmentOptions || webApiEnvironmentOptions == null || serverEnvironmentOptions == null || !webApiEnvironmentOptions.AreEmailNotificationPropertiesSame(serverEnvironmentOptions);
    }

    private static bool AreVariablesModified(
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> webApiVariables,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> serverVariables)
    {
      if (webApiVariables.Count != serverVariables.Count)
        return true;
      foreach (string key in (IEnumerable<string>) webApiVariables.Keys)
      {
        if (!serverVariables.ContainsKey(key) || webApiVariables[key].IsSecret != serverVariables[key].IsSecret || !string.Equals(webApiVariables[key].Value, serverVariables[key].Value, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    private static bool AreVariableGroupsModified(
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup> webApiVariableGroups,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> serverVariableGroups)
    {
      if (webApiVariableGroups == null || webApiVariableGroups.Count != serverVariableGroups.Count)
        return true;
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup serverVariableGroup1 in (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) serverVariableGroups)
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup serverVariableGroup = serverVariableGroup1;
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup variableGroup = webApiVariableGroups.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup, bool>) (vg => vg.Id == serverVariableGroup.Id));
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup apiVariableGroup = VariableGroupConverter.ToWebApiVariableGroup(serverVariableGroup);
        if (variableGroup == null || !variableGroup.Equals((object) apiVariableGroup))
          return true;
      }
      return false;
    }

    private static string GetPropertyName<T>(Expression<Func<T>> propertyExpression) => !(propertyExpression.Body is MemberExpression) ? string.Empty : ((MemberExpression) propertyExpression.Body).Member.Name;

    private static bool IsSamePhaseType(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes webapiDeployPhaseType,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes serverDeployPhaseType)
    {
      switch (webapiDeployPhaseType)
      {
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.Undefined:
          return serverDeployPhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.Undefined;
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.AgentBasedDeployment:
          return serverDeployPhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.AgentBasedDeployment;
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.RunOnServer:
          return serverDeployPhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.RunOnServer;
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.MachineGroupBasedDeployment:
          return serverDeployPhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.MachineGroupBasedDeployment;
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.DeploymentGates:
          return serverDeployPhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.DeploymentGates;
        default:
          return false;
      }
    }
  }
}
