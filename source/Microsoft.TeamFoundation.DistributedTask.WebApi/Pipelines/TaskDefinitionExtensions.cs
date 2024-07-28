// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.TaskDefinitionExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TaskDefinitionExtensions
  {
    private static readonly Dictionary<string, string> NodeVersionToAgentVersion = new Dictionary<string, string>()
    {
      ["Node10"] = AgentFeatureDemands.Node10TaskDemand().Value,
      ["Node14"] = AgentFeatureDemands.Node14TaskDemand().Value
    };

    public static string ComputeDisplayName(
      this TaskDefinition taskDefinition,
      IDictionary<string, string> inputs)
    {
      if (!string.IsNullOrEmpty(taskDefinition.InstanceNameFormat))
        return VariableUtility.ExpandVariables(taskDefinition.InstanceNameFormat, inputs);
      return !string.IsNullOrEmpty(taskDefinition.FriendlyName) ? taskDefinition.FriendlyName : taskDefinition.Name;
    }

    public static string GetMinimumAgentVersion(
      this TaskDefinition taskDefinition,
      string currentMinimum,
      out bool newMinimum)
    {
      newMinimum = false;
      string semanticVersion2;
      if (DemandMinimumVersion.CompareVersion(taskDefinition.MinimumAgentVersion, currentMinimum) > 0)
      {
        semanticVersion2 = taskDefinition.MinimumAgentVersion;
        newMinimum = true;
      }
      else
        semanticVersion2 = currentMinimum;
      string minimumVersionForNode = taskDefinition.GetMinimumVersionForNode();
      if (minimumVersionForNode != null && DemandMinimumVersion.CompareVersion(minimumVersionForNode, semanticVersion2) > 0)
      {
        semanticVersion2 = minimumVersionForNode;
        newMinimum = true;
      }
      return semanticVersion2;
    }

    private static string GetMinimumVersionForNode(this TaskDefinition taskDefinition)
    {
      string semanticVersion2 = (string) null;
      foreach (KeyValuePair<string, string> keyValuePair in TaskDefinitionExtensions.NodeVersionToAgentVersion)
      {
        if ((semanticVersion2 == null || DemandMinimumVersion.CompareVersion(keyValuePair.Value, semanticVersion2) > 0) && (taskDefinition.PreJobExecution.Keys.Contains<string>(keyValuePair.Key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) || taskDefinition.Execution.Keys.Contains<string>(keyValuePair.Key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) || taskDefinition.PostJobExecution.Keys.Contains<string>(keyValuePair.Key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)))
          semanticVersion2 = keyValuePair.Value;
      }
      return semanticVersion2;
    }
  }
}
