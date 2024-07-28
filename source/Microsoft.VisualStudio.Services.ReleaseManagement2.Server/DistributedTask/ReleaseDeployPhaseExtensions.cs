// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.ReleaseDeployPhaseExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public static class ReleaseDeployPhaseExtensions
  {
    public static Dictionary<string, ConfigurationVariableValue> GetMergedOutputVariables(
      this IList<ReleaseDeployPhase> deployPhases,
      IDistributedTaskOrchestrator distributedTaskOrchestrator,
      int currentAttempt)
    {
      Dictionary<string, ConfigurationVariableValue> mergedOutputVariables = new Dictionary<string, ConfigurationVariableValue>();
      if (deployPhases == null || !deployPhases.Any<ReleaseDeployPhase>())
        return mergedOutputVariables;
      IEnumerable<ReleaseDeployPhase> source = deployPhases.Where<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (p => ReleaseDeployPhaseExtensions.CanShareOutputVariables(p, currentAttempt)));
      if (!source.Any<ReleaseDeployPhase>() || distributedTaskOrchestrator == null)
        return mergedOutputVariables;
      IList<OutputVariableScope> outputVariables = distributedTaskOrchestrator.GetOutputVariables((IList<Guid>) source.Select<ReleaseDeployPhase, Guid>((Func<ReleaseDeployPhase, Guid>) (p => p.RunPlanId.Value)).ToList<Guid>());
      if (outputVariables == null)
        return mergedOutputVariables;
      foreach (Guid? nullable1 in source.OrderBy<ReleaseDeployPhase, int>((Func<ReleaseDeployPhase, int>) (p => p.Rank)).Select<ReleaseDeployPhase, Guid?>((Func<ReleaseDeployPhase, Guid?>) (p => p.RunPlanId)))
      {
        Guid? planId = nullable1;
        OutputVariableScope outputVariableScope = outputVariables.SingleOrDefault<OutputVariableScope>((Func<OutputVariableScope, bool>) (x =>
        {
          Guid id = x.Id;
          Guid? nullable2 = planId;
          return nullable2.HasValue && id == nullable2.GetValueOrDefault();
        }));
        if (outputVariableScope != null)
        {
          foreach (KeyValuePair<string, VariableValue> keyValuePair in (IEnumerable<KeyValuePair<string, VariableValue>>) outputVariableScope.Flatten(false))
            mergedOutputVariables[keyValuePair.Key] = ReleaseDeployPhaseExtensions.ToConfigurationVariableValue(keyValuePair.Value);
        }
      }
      return mergedOutputVariables;
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
      if (phase.Attempt != attempt)
        return false;
      return phase.Status == DeployPhaseStatus.Succeeded || phase.Status == DeployPhaseStatus.PartiallySucceeded;
    }
  }
}
