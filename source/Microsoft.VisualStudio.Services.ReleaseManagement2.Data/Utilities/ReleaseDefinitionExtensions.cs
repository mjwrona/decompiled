// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseDefinitionExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ReleaseDefinitionExtensions
  {
    public static ReleaseDefinitionEnvironmentsSnapshot ToReleaseDefinitionEnvironmentSnapshot(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definition)
    {
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      ReleaseDefinitionEnvironmentsSnapshot environmentSnapshot = new ReleaseDefinitionEnvironmentsSnapshot();
      foreach (DefinitionEnvironment environment in (IEnumerable<DefinitionEnvironment>) definition.Environments)
      {
        DefinitionEnvironmentData definitionEnvironmentData = new DefinitionEnvironmentData()
        {
          Id = environment.Id,
          Name = environment.Name,
          Rank = environment.Rank,
          EnvironmentTriggers = (IList<string>) ReleaseDefinitionExtensions.GetEnvironmentTriggers(environment)
        };
        foreach (DefinitionEnvironmentStep getStepsForTest in (IEnumerable<DefinitionEnvironmentStep>) environment.GetStepsForTests)
          definitionEnvironmentData.Steps.Add(getStepsForTest.ToDefinitionEnvironmentStepData());
        environmentSnapshot.Environments.Add(definitionEnvironmentData);
      }
      return environmentSnapshot;
    }

    public static IList<ReleaseCondition> ToReleaseConditions(this IList<Condition> conditions) => (IList<ReleaseCondition>) conditions.Select<Condition, ReleaseCondition>((Func<Condition, ReleaseCondition>) (condition => condition.ToReleaseCondition())).ToList<ReleaseCondition>();

    public static bool HasMetaTask(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      return releaseDefinition.Environments.Where<ReleaseDefinitionEnvironment>((Func<ReleaseDefinitionEnvironment, bool>) (definitionEnvironment => definitionEnvironment != null)).Any<ReleaseDefinitionEnvironment>((Func<ReleaseDefinitionEnvironment, bool>) (definitionEnvironment => definitionEnvironment.HasMetaTask()));
    }

    public static bool IsYamlDefinition(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      return releaseDefinition.PipelineProcess != null && releaseDefinition.PipelineProcess.Type == PipelineProcessTypes.Yaml;
    }

    public static IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> GetVariablesFromReleaseDefinition(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> first = releaseDefinition != null ? releaseDefinition.Variables : throw new ArgumentNullException(nameof (releaseDefinition));
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> variableGroups = ReleaseDefinitionVariableGroupUtility.GetVariableGroups(requestContext, projectId, releaseDefinition.VariableGroups);
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variableGroupVariables = ReleaseDefinitionVariableGroupUtility.GetVariableGroupVariables(releaseDefinition.VariableGroups, variableGroups);
      return (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) first.Concat<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>((IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) variableGroupVariables).GroupBy<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>, string>) (x => x.Key)).ToDictionary<IGrouping<string, KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>, string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>((Func<IGrouping<string, KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>, string>) (y => y.Key), (Func<IGrouping<string, KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) (y => y.First<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>().Value));
    }

    private static ReleaseCondition ToReleaseCondition(this Condition condition)
    {
      if (condition == null)
        throw new ArgumentNullException(nameof (condition));
      return new ReleaseCondition(condition.Name, condition.ConditionType, condition.Value, new bool?());
    }

    private static List<string> GetEnvironmentTriggers(DefinitionEnvironment definitionEnvironment)
    {
      List<string> environmentTriggers = (List<string>) null;
      if (definitionEnvironment.EnvironmentTriggers.Count > 0)
      {
        environmentTriggers = new List<string>();
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentTrigger environmentTrigger in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentTrigger>) definitionEnvironment.EnvironmentTriggers)
        {
          switch ((EnvironmentTriggerType) environmentTrigger.TriggerType)
          {
            case EnvironmentTriggerType.DeploymentGroupRedeploy:
              environmentTriggers.Add("RedeployTrigger");
              continue;
            case EnvironmentTriggerType.RollbackRedeploy:
              environmentTriggers.Add("RollbackTrigger");
              continue;
            default:
              continue;
          }
        }
      }
      return environmentTriggers;
    }
  }
}
