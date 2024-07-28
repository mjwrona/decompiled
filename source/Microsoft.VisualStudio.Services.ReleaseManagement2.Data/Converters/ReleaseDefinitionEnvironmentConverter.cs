// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ReleaseDefinitionEnvironmentConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ReleaseDefinitionEnvironmentConverter
  {
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This class has many dependencies on other classes.")]
    public static ReleaseDefinitionEnvironment ConvertToWebApiEnvironment(
      IVssRequestContext context,
      Guid projectId,
      DefinitionEnvironment serverEnvironment)
    {
      if (serverEnvironment == null)
        throw new ArgumentNullException(nameof (serverEnvironment));
      ReleaseDefinitionEnvironment webApiEnv = new ReleaseDefinitionEnvironment()
      {
        Id = serverEnvironment.Id,
        Name = serverEnvironment.Name,
        Rank = serverEnvironment.Rank,
        Owner = new IdentityRef()
        {
          Id = serverEnvironment.OwnerId.ToString()
        },
        Conditions = ConditionsUtility.ConvertToWebApiConditions(serverEnvironment.Conditions),
        ExecutionPolicy = serverEnvironment.ExecutionPolicy,
        EnvironmentOptions = EnvironmentOptionsConverter.ToWebApiEnvironmentOptions(serverEnvironment.EnvironmentOptions),
        ProcessParameters = serverEnvironment.ProcessParameters,
        PreDeploymentGates = serverEnvironment.PreDeploymentGates,
        PostDeploymentGates = serverEnvironment.PostDeploymentGates,
        BadgeUrl = WebAccessUrlBuilder.GetDeploymentBadgeRestUrl(context, projectId, serverEnvironment.ReleaseDefinitionId, serverEnvironment.Id),
        EnvironmentTriggers = EnvironmentTriggerConverter.ToWebApiEnvironmentTriggers(serverEnvironment.EnvironmentTriggers)
      };
      ReleaseDefinitionEnvironmentConverter.PopulateGateIds(serverEnvironment, webApiEnv);
      webApiEnv.CurrentReleaseReference = new ReleaseShallowReference()
      {
        Id = serverEnvironment.CurrentReleaseId,
        Name = (string) null,
        Url = WebAccessUrlBuilder.GetReleaseRestUrl(context, projectId, serverEnvironment.CurrentReleaseId)
      };
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>) serverEnvironment.Variables)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue configurationVariableValue = variable.Value.ToWebApiConfigurationVariableValue();
        webApiEnv.Variables[variable.Key] = configurationVariableValue;
      }
      if (serverEnvironment.VariableGroups.Any<int>())
        webApiEnv.VariableGroups.AddRange<int, IList<int>>((IEnumerable<int>) serverEnvironment.VariableGroups);
      if (serverEnvironment.GetStepsForTests.Any<DefinitionEnvironmentStep>())
      {
        webApiEnv.PreDeployApprovals = serverEnvironment.ToWebApiDefinitionEnvironmentApprovals(EnvironmentStepType.PreDeploy);
        DefinitionEnvironmentStep definitionEnvironmentStep = serverEnvironment.GetSteps(EnvironmentStepType.Deploy).Single<DefinitionEnvironmentStep>();
        ReleaseDefinitionEnvironment definitionEnvironment = webApiEnv;
        ReleaseDefinitionDeployStep definitionDeployStep = new ReleaseDefinitionDeployStep();
        definitionDeployStep.Id = definitionEnvironmentStep.Id;
        definitionEnvironment.DeployStep = definitionDeployStep;
        webApiEnv.PostDeployApprovals = serverEnvironment.ToWebApiDefinitionEnvironmentApprovals(EnvironmentStepType.PostDeploy);
      }
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase deployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase>) serverEnvironment.DeployPhases)
        webApiEnv.DeployPhases.Add(deployPhase.ToWebApiDeployPhase());
      webApiEnv.DeployPhases = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) webApiEnv.DeployPhases.OrderBy<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, int>) (dp => dp.Rank)).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>();
      if (serverEnvironment.Schedules.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>())
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule schedule in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseSchedule>) serverEnvironment.Schedules)
          webApiEnv.Schedules.Add(schedule.ToWebApi());
      }
      ReleaseDefinitionEnvironment definitionEnvironment1 = webApiEnv;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentRetentionPolicy retentionPolicy = serverEnvironment.RetentionPolicy;
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.EnvironmentRetentionPolicy environmentRetentionPolicy = retentionPolicy != null ? retentionPolicy.ConvertToWebApiEnvironmentRetentionPolicy() : (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.EnvironmentRetentionPolicy) null;
      definitionEnvironment1.RetentionPolicy = environmentRetentionPolicy;
      if (serverEnvironment.Properties.Any<PropertyValue>())
      {
        foreach (PropertyValue property in (IEnumerable<PropertyValue>) serverEnvironment.Properties)
          webApiEnv.Properties.TryAdd<string, object>(property.PropertyName, property.Value);
      }
      return webApiEnv;
    }

    private static void PopulateGateIds(
      DefinitionEnvironment serverEnvironment,
      ReleaseDefinitionEnvironment webApiEnv)
    {
      if (serverEnvironment.GetStepsForTests.Any<DefinitionEnvironmentStep>((Func<DefinitionEnvironmentStep, bool>) (s => s.StepType == EnvironmentStepType.PreGate)))
        webApiEnv.PreDeploymentGates.Id = serverEnvironment.GetStepsForTests.First<DefinitionEnvironmentStep>((Func<DefinitionEnvironmentStep, bool>) (s => s.StepType == EnvironmentStepType.PreGate)).Id;
      if (!serverEnvironment.GetStepsForTests.Any<DefinitionEnvironmentStep>((Func<DefinitionEnvironmentStep, bool>) (s => s.StepType == EnvironmentStepType.PostGate)))
        return;
      webApiEnv.PostDeploymentGates.Id = serverEnvironment.GetStepsForTests.First<DefinitionEnvironmentStep>((Func<DefinitionEnvironmentStep, bool>) (s => s.StepType == EnvironmentStepType.PostGate)).Id;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This class has many dependencies on other classes.")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Error message")]
    public static DefinitionEnvironment ConvertToServerEnvironment(
      IVssRequestContext context,
      ReleaseDefinitionEnvironment webApiEnvironment)
    {
      if (webApiEnvironment == null)
        throw new ArgumentNullException(nameof (webApiEnvironment));
      Guid result = Guid.Empty;
      if (webApiEnvironment.Owner != null && !Guid.TryParse(webApiEnvironment.Owner.Id, out result))
        context.Trace(1961079, TraceLevel.Warning, "ReleaseManagementService", "Service", "Invalid environment owner id: {0}", (object) webApiEnvironment.Owner.Id);
      bool flag = context.IsFeatureEnabled("AzureDevOps.ReleaseManagement.RemovePropertiesWithDefaultValuesInArtifactConditions");
      IList<Condition> conditionList = (IList<Condition>) new List<Condition>();
      if (webApiEnvironment.Conditions != null)
        conditionList = !flag ? webApiEnvironment.Conditions : ConditionsUtility.RemovePropertiesThatHasDefaultValues(context, webApiEnvironment.Conditions);
      DefinitionEnvironment definitionEnvironment = new DefinitionEnvironment()
      {
        Id = webApiEnvironment.Id,
        Name = webApiEnvironment.Name,
        Rank = webApiEnvironment.Rank,
        OwnerId = result,
        Conditions = conditionList,
        ExecutionPolicy = webApiEnvironment.ExecutionPolicy,
        EnvironmentOptions = EnvironmentOptionsConverter.ToServerEnvironmentOptions(webApiEnvironment.EnvironmentOptions),
        ProcessParameters = webApiEnvironment.ProcessParameters,
        EnvironmentTriggers = EnvironmentTriggerConverter.ToServerEnvironmentTriggers(webApiEnvironment.EnvironmentTriggers)
      };
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) webApiEnvironment.DeployPhases)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase serverDeployPhase = deployPhase.ToServerDeployPhase(webApiEnvironment);
        definitionEnvironment.DeployPhases.Add(serverDeployPhase);
        context.Trace(1961079, TraceLevel.Verbose, "ReleaseManagementService", "Service", "ServerEnvironment DeployPhase Input : {0}", (object) serverDeployPhase.DeploymentInput);
      }
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) webApiEnvironment.Variables)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue configurationVariableValue = variable.Value.ToServerConfigurationVariableValue();
        definitionEnvironment.Variables[variable.Key] = configurationVariableValue;
      }
      if (webApiEnvironment.VariableGroups.Any<int>())
        definitionEnvironment.VariableGroups.AddRange<int, IList<int>>((IEnumerable<int>) webApiEnvironment.VariableGroups);
      if (webApiEnvironment.Properties.Any<KeyValuePair<string, object>>())
      {
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) webApiEnvironment.Properties)
          definitionEnvironment.Properties.Add(new PropertyValue(property.Key, property.Value));
      }
      if (webApiEnvironment.Schedules.Any<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule>())
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule schedule in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule>) webApiEnvironment.Schedules)
          definitionEnvironment.Schedules.Add(schedule.FromWebApi());
      }
      definitionEnvironment.PreApprovalOptions = webApiEnvironment.PreDeployApprovals.ApprovalOptions.FromWebApiApprovalOptions();
      definitionEnvironment.PostApprovalOptions = webApiEnvironment.PostDeployApprovals.ApprovalOptions.FromWebApiApprovalOptions();
      definitionEnvironment.PreDeploymentGates = webApiEnvironment.PreDeploymentGates;
      definitionEnvironment.PostDeploymentGates = webApiEnvironment.PostDeploymentGates;
      definitionEnvironment.RetentionPolicy = webApiEnvironment.RetentionPolicy.ConvertToServerEnvironmentRetentionPolicy();
      IList<DefinitionEnvironmentStep> withRanksAdjusted1 = ReleaseDefinitionEnvironmentConverter.GetApprovalAndGateStepsWithRanksAdjusted(webApiEnvironment, EnvironmentStepType.PreDeploy, definitionEnvironment.PreDeploymentGates);
      int num = withRanksAdjusted1.Select<DefinitionEnvironmentStep, int>((Func<DefinitionEnvironmentStep, int>) (s => s.Rank)).Max();
      DefinitionEnvironmentStep serverStep = ReleaseDefinitionEnvironmentConverter.ConvertToServerStep(webApiEnvironment.DeployStep.Id, num + 1, EnvironmentStepType.Deploy);
      IList<DefinitionEnvironmentStep> withRanksAdjusted2 = ReleaseDefinitionEnvironmentConverter.GetApprovalAndGateStepsWithRanksAdjusted(webApiEnvironment, EnvironmentStepType.PostDeploy, definitionEnvironment.PostDeploymentGates);
      foreach (DefinitionEnvironmentStep definitionEnvironmentStep in (IEnumerable<DefinitionEnvironmentStep>) withRanksAdjusted2)
        definitionEnvironmentStep.Rank = serverStep.Rank + definitionEnvironmentStep.Rank;
      ReleaseDefinitionEnvironmentConverter.ResetGateIdsIfGatesDisabled(definitionEnvironment);
      return ReleaseDefinitionEnvironmentConverter.AddStepsToServerEnvironment((IEnumerable<DefinitionEnvironmentStep>) withRanksAdjusted1, serverStep, (IEnumerable<DefinitionEnvironmentStep>) withRanksAdjusted2, definitionEnvironment);
    }

    private static void ResetGateIdsIfGatesDisabled(DefinitionEnvironment serverEnvironment)
    {
      if (!serverEnvironment.PreDeploymentGates.AreGatesEnabled())
        serverEnvironment.PreDeploymentGates.Id = 0;
      if (serverEnvironment.PostDeploymentGates.AreGatesEnabled())
        return;
      serverEnvironment.PostDeploymentGates.Id = 0;
    }

    private static IList<DefinitionEnvironmentStep> GetApprovalAndGateStepsWithRanksAdjusted(
      ReleaseDefinitionEnvironment webApiEnvironment,
      EnvironmentStepType approvalType,
      ReleaseDefinitionGatesStep gates)
    {
      int rankOffset = 0;
      ReleaseDefinitionApprovals definitionApprovals = approvalType == EnvironmentStepType.PreDeploy ? webApiEnvironment.PreDeployApprovals : webApiEnvironment.PostDeployApprovals;
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions approvalOptions = definitionApprovals.ApprovalOptions;
      ApprovalExecutionOrder approvalExecutionOrder = approvalOptions != null ? approvalOptions.ExecutionOrder : ApprovalExecutionOrder.BeforeGates;
      EnvironmentStepType stepType = approvalType == EnvironmentStepType.PreDeploy ? EnvironmentStepType.PreGate : EnvironmentStepType.PostGate;
      List<DefinitionEnvironmentStep> withRanksAdjusted = new List<DefinitionEnvironmentStep>();
      if (gates.AreGatesEnabled())
      {
        int num = definitionApprovals.Approvals.Max<ReleaseDefinitionApprovalStep>((Func<ReleaseDefinitionApprovalStep, int>) (a => a.Rank));
        DefinitionEnvironmentStep serverStep = ReleaseDefinitionEnvironmentConverter.ConvertToServerStep(gates.GetReleaseDefinitionGateStepId(), approvalExecutionOrder == ApprovalExecutionOrder.BeforeGates ? num + 1 : 1, stepType);
        withRanksAdjusted.Add(serverStep);
        rankOffset = approvalExecutionOrder != ApprovalExecutionOrder.BeforeGates ? 1 : 0;
      }
      IList<DefinitionEnvironmentStep> collection = webApiEnvironment.FromWebApiDefinitionEnvironmentApprovals(approvalType, rankOffset);
      withRanksAdjusted.AddRange((IEnumerable<DefinitionEnvironmentStep>) collection);
      return (IList<DefinitionEnvironmentStep>) withRanksAdjusted;
    }

    private static DefinitionEnvironment AddStepsToServerEnvironment(
      IEnumerable<DefinitionEnvironmentStep> preDeployServerSteps,
      DefinitionEnvironmentStep deployServerStep,
      IEnumerable<DefinitionEnvironmentStep> postDeployServerSteps,
      DefinitionEnvironment serverEnv)
    {
      List<DefinitionEnvironmentStep> definitionEnvironmentStepList = new List<DefinitionEnvironmentStep>();
      definitionEnvironmentStepList.AddRange(preDeployServerSteps);
      definitionEnvironmentStepList.Add(deployServerStep);
      definitionEnvironmentStepList.AddRange(postDeployServerSteps);
      foreach (DefinitionEnvironmentStep definitionEnvironmentStep in definitionEnvironmentStepList)
        definitionEnvironmentStep.DefinitionEnvironmentId = serverEnv.Id;
      definitionEnvironmentStepList.ForEach((Action<DefinitionEnvironmentStep>) (s => serverEnv.GetStepsForTests.Add(s)));
      return serverEnv;
    }

    private static DefinitionEnvironmentStep ConvertToServerStep(
      int stepId,
      int rank,
      EnvironmentStepType stepType)
    {
      return new DefinitionEnvironmentStep()
      {
        Id = stepId,
        Rank = rank,
        IsAutomated = true,
        IsNotificationOn = false,
        ApproverId = Guid.Empty,
        StepType = stepType
      };
    }
  }
}
