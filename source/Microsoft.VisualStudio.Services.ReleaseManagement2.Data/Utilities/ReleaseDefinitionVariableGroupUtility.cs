// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseDefinitionVariableGroupUtility
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ReleaseDefinitionVariableGroupUtility
  {
    public static IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> GetVariableGroups(
      IVssRequestContext context,
      Guid projectId,
      IList<int> variableGroupIds)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> variableGroups = new List<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>();
      if (variableGroupIds != null && variableGroupIds.Any<int>())
        variableGroups = context.GetService<IVariableGroupService>().GetVariableGroups(context.Elevate(), projectId, variableGroupIds).ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>();
      return (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) variableGroups;
    }

    public static IList<int> GetAllVariableGroupIdsInRD(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      List<int> source = new List<int>();
      if (releaseDefinition.VariableGroups != null)
        source.AddRange((IEnumerable<int>) releaseDefinition.VariableGroups);
      if (releaseDefinition.Environments != null)
        source.AddRange(releaseDefinition.Environments.Where<ReleaseDefinitionEnvironment>((Func<ReleaseDefinitionEnvironment, bool>) (env => env?.VariableGroups != null)).SelectMany<ReleaseDefinitionEnvironment, int>((Func<ReleaseDefinitionEnvironment, IEnumerable<int>>) (env => (IEnumerable<int>) env.VariableGroups)));
      return (IList<int>) source.Distinct<int>().ToList<int>();
    }

    public static IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> GetAllVariableGroupsInRD(
      IVssRequestContext vssRequestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition)
    {
      IList<int> variableGroupIdsInRd = ReleaseDefinitionVariableGroupUtility.GetAllVariableGroupIdsInRD(releaseDefinition);
      return ReleaseDefinitionVariableGroupUtility.GetVariableGroups(vssRequestContext, projectId, variableGroupIdsInRd);
    }

    public static IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> GetVariableGroupVariables(
      IList<int> variableGroupIds,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> allVariableGroupsInRD)
    {
      Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variableGroupVariables = new Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>();
      IList<int> intList = variableGroupIds;
      if ((intList != null ? (intList.Count > 0 ? 1 : 0) : 0) != 0 && allVariableGroupsInRD != null)
        variableGroupVariables = VariableGroupsMerger.MergeVariableGroups((IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) allVariableGroupsInRD.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, bool>) (vg => variableGroupIds.Contains(vg.Id))).ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>()).ToDictionary<KeyValuePair<string, MergedConfigurationVariableValue>, string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (v => v.Key), (Func<KeyValuePair<string, MergedConfigurationVariableValue>, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) (v => v.Value.ToWebApiConfigurationVariableValue()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      return (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) variableGroupVariables;
    }
  }
}
