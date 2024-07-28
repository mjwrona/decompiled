// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.OutputVariablesUtility
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public static class OutputVariablesUtility
  {
    public static string GetJobRefName(AutomationEngineInput input)
    {
      string jobRefName = "Release";
      if (input != null)
      {
        if (input.StepType == 4)
          return "Predeploygate";
        if (input.StepType == 8)
          return "Postdeploygate";
        if (input.DeployPhaseData != null && !string.IsNullOrEmpty(input.DeployPhaseData.RefName))
          jobRefName = input.DeployPhaseData.RefName;
      }
      return jobRefName;
    }

    public static Dictionary<string, ConfigurationVariableValue> GetGateOutputVariables(
      IDistributedTaskOrchestrator distributedTaskOrchestrator,
      ReleaseEnvironment environment,
      int currentAttempt)
    {
      return OutputVariablesUtility.GetMergedOutputVariables(distributedTaskOrchestrator, environment, currentAttempt, false);
    }

    public static Dictionary<string, ConfigurationVariableValue> GetMergedOutputVariables(
      IDistributedTaskOrchestrator distributedTaskOrchestrator,
      ReleaseEnvironment environment,
      int currentAttempt,
      bool includePhaseOutputVariables)
    {
      if (distributedTaskOrchestrator == null)
        throw new ArgumentNullException(nameof (distributedTaskOrchestrator));
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      Dictionary<string, ConfigurationVariableValue> mergedOutputVariables = new Dictionary<string, ConfigurationVariableValue>();
      IList<Guid> matchingDeploymentGates = OutputVariablesUtility.GetRunPlanIdsForMatchingDeploymentGates(environment, currentAttempt);
      if (includePhaseOutputVariables)
      {
        IList<Guid> matchingDeployPhases = OutputVariablesUtility.GetRunPlanIdsForMatchingDeployPhases(environment, currentAttempt);
        matchingDeploymentGates.AddRange<Guid, IList<Guid>>((IEnumerable<Guid>) matchingDeployPhases);
      }
      IList<OutputVariableScope> outputVariables = distributedTaskOrchestrator.GetOutputVariables(matchingDeploymentGates);
      if (outputVariables == null)
        return mergedOutputVariables;
      foreach (Guid guid in (IEnumerable<Guid>) matchingDeploymentGates)
      {
        Guid planId = guid;
        OutputVariableScope outputVariableScope = outputVariables.SingleOrDefault<OutputVariableScope>((Func<OutputVariableScope, bool>) (x => x.Id == planId));
        if (outputVariableScope != null)
        {
          foreach (KeyValuePair<string, VariableValue> keyValuePair in (IEnumerable<KeyValuePair<string, VariableValue>>) outputVariableScope.Flatten(includeSecrets: false))
            mergedOutputVariables[keyValuePair.Key] = OutputVariablesUtility.ToConfigurationVariableValue(keyValuePair.Value);
        }
      }
      return mergedOutputVariables;
    }

    private static IList<Guid> GetRunPlanIdsForMatchingDeployPhases(
      ReleaseEnvironment environment,
      int currentAttempt)
    {
      List<Guid> matchingDeployPhases = new List<Guid>();
      IList<ReleaseDeployPhase> releaseDeployPhases = environment.ReleaseDeployPhases;
      if (releaseDeployPhases != null && releaseDeployPhases.Any<ReleaseDeployPhase>())
      {
        IEnumerable<ReleaseDeployPhase> source = releaseDeployPhases.Where<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (p => OutputVariablesUtility.CanShareOutputVariables(p, currentAttempt)));
        matchingDeployPhases.AddRange(source.OrderBy<ReleaseDeployPhase, int>((Func<ReleaseDeployPhase, int>) (p => p.Rank)).Select<ReleaseDeployPhase, Guid>((Func<ReleaseDeployPhase, Guid>) (p => p.RunPlanId.Value)));
      }
      return (IList<Guid>) matchingDeployPhases;
    }

    private static IList<Guid> GetRunPlanIdsForMatchingDeploymentGates(
      ReleaseEnvironment environment,
      int currentAttempt)
    {
      List<Guid> matchingDeploymentGates = new List<Guid>();
      Deployment deploymentByAttempt = environment.GetDeploymentByAttempt(currentAttempt);
      if (deploymentByAttempt != null)
      {
        IList<DeploymentGate> deploymentGates = deploymentByAttempt.DeploymentGates;
        if (deploymentGates != null && deploymentGates.Any<DeploymentGate>())
        {
          IEnumerable<DeploymentGate> source = deploymentGates.Where<DeploymentGate>((Func<DeploymentGate, bool>) (g => g.Status == GateStatus.Succeeded));
          matchingDeploymentGates.AddRange((IEnumerable<Guid>) source.Select<DeploymentGate, Guid>((Func<DeploymentGate, Guid>) (g => g.RunPlanId.Value)).ToList<Guid>());
        }
      }
      return (IList<Guid>) matchingDeploymentGates;
    }

    private static ConfigurationVariableValue ToConfigurationVariableValue(
      VariableValue variableValue)
    {
      return new ConfigurationVariableValue()
      {
        Value = variableValue.Value,
        IsSecret = variableValue.IsSecret
      };
    }

    private static bool CanShareOutputVariables(ReleaseDeployPhase phase, int attempt)
    {
      if (phase == null || phase.Attempt != attempt)
        return false;
      return phase.Status == DeployPhaseStatus.Succeeded || phase.Status == DeployPhaseStatus.PartiallySucceeded;
    }
  }
}
