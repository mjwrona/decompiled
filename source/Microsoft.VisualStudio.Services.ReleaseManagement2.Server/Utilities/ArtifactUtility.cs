// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ArtifactUtility
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Build;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class ArtifactUtility
  {
    private static readonly Lazy<Regex> VariableReferenceRegex = new Lazy<Regex>((Func<Regex>) (() => new Regex("\\$\\(([^)]+)\\)", RegexOptions.Compiled | RegexOptions.Singleline)), true);

    public static void ResolveSourceBranchVariablesForArtifactSourcesAndTriggerConditions(
      IVssRequestContext context,
      ReleaseDefinition releaseDefinition)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      bool flag1 = releaseDefinition != null ? ArtifactUtility.ArtifactSourcesContainsVariables(releaseDefinition.LinkedArtifacts) : throw new ArgumentNullException(nameof (releaseDefinition));
      bool flag2 = ArtifactUtility.TriggerConditionsContainsVariables((IList<ArtifactSourceTrigger>) releaseDefinition.ArtifactSourceTriggers);
      if (!(flag1 | flag2))
        return;
      IDictionary<string, string> variablesMap = ArtifactUtility.FetchAllVariables(context, releaseDefinition.ProjectId, releaseDefinition.Id);
      if (flag1)
        ArtifactUtility.ResolveVariablesForArtifactSources(releaseDefinition.LinkedArtifacts, variablesMap);
      if (!flag2)
        return;
      ArtifactUtility.ResolveVariablesForArtifactSourceTriggers((IList<ArtifactSourceTrigger>) releaseDefinition.ArtifactSourceTriggers, variablesMap);
    }

    public static void ResolveSourceBranchVariablesForArtifactSources(
      IVssRequestContext context,
      Guid currentProjectId,
      IList<ArtifactSource> artifactSources,
      int releaseDefinitionId)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (!ArtifactUtility.ArtifactSourcesContainsVariables(artifactSources))
        return;
      IDictionary<string, string> variablesMap = ArtifactUtility.FetchAllVariables(context, currentProjectId, releaseDefinitionId);
      ArtifactUtility.ResolveVariablesForArtifactSources(artifactSources, variablesMap);
    }

    public static void ResolveBuildDefinitionDefaultBranchForTriggerConditions(
      IVssRequestContext context,
      ReleaseDefinition releaseDefinition,
      Guid buildProjectId,
      int buildDefinitionId)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      if (releaseDefinition.LinkedArtifacts.IsNullOrEmpty<ArtifactSource>() || releaseDefinition.ArtifactSourceTriggers.IsNullOrEmpty<ArtifactSourceTrigger>())
        return;
      List<string> source = new List<string>();
      foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) releaseDefinition.LinkedArtifacts)
      {
        if (linkedArtifact.HasMatchingDefinitionId(buildDefinitionId) && linkedArtifact.TeamProjectId == buildProjectId)
          source.Add(linkedArtifact.Alias);
      }
      string branch = (string) null;
      foreach (ArtifactSourceTrigger artifactSourceTrigger1 in releaseDefinition.ArtifactSourceTriggers)
      {
        ArtifactSourceTrigger artifactSourceTrigger = artifactSourceTrigger1;
        if (!artifactSourceTrigger.TriggerConditions.IsNullOrEmpty<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter>() && source.Select<string, bool>((Func<string, bool>) (x => x == artifactSourceTrigger.Alias)).Any<bool>())
        {
          foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter triggerCondition in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter>) artifactSourceTrigger.TriggerConditions)
          {
            if (triggerCondition.UseBuildDefinitionBranch)
            {
              if (branch == null)
              {
                branch = BuildArtifact.GetDefaultBranchForBuildDefinition(context, buildProjectId, buildDefinitionId);
                branch = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter.RemoveBranchPrefix(branch);
              }
              triggerCondition.SourceBranch = triggerCondition.SourceBranch == "-" ? triggerCondition.SourceBranch + branch : branch;
            }
          }
        }
      }
    }

    public static IDictionary<string, string> GetArtifactSourceInputs(ArtifactSource artifactSource)
    {
      if (artifactSource == null)
        throw new ArgumentNullException(nameof (artifactSource));
      Dictionary<string, string> artifactSourceInputs = new Dictionary<string, string>();
      foreach (KeyValuePair<string, InputValue> keyValuePair in artifactSource.SourceData)
      {
        if (keyValuePair.Value != null)
          artifactSourceInputs.Add(keyValuePair.Key, keyValuePair.Value.Value);
      }
      return (IDictionary<string, string>) artifactSourceInputs;
    }

    public static bool IsTriggeringBuildArtifactSource(
      string artifactAlias,
      ReleaseDefinition releaseDefinition,
      Guid teamProjectId,
      int buildDefinitionId)
    {
      if (artifactAlias == null)
        throw new ArgumentNullException(nameof (artifactAlias));
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      List<ArtifactSource> list = releaseDefinition.LinkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (a => string.Compare(a.Alias, artifactAlias, StringComparison.OrdinalIgnoreCase) == 0)).ToList<ArtifactSource>();
      if (list.IsNullOrEmpty<ArtifactSource>())
        return false;
      foreach (ArtifactSource artifactSource in list)
      {
        if (artifactSource.IsBuildArtifact && artifactSource.TeamProjectId == teamProjectId && artifactSource.HasMatchingDefinitionId(buildDefinitionId))
          return true;
      }
      return false;
    }

    public static bool IsTriggeringGitOrGitHubArtifactSource(
      ReleaseDefinition definition,
      string artifactAlias,
      Guid projectId,
      string repositoryId)
    {
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      if (artifactAlias == null)
        throw new ArgumentNullException(nameof (artifactAlias));
      if (repositoryId == null)
        throw new ArgumentNullException(nameof (repositoryId));
      ArtifactSource artifactSource = definition.LinkedArtifacts.FirstOrDefault<ArtifactSource>((Func<ArtifactSource, bool>) (a => string.Compare(a.Alias, artifactAlias, StringComparison.OrdinalIgnoreCase) == 0));
      return artifactSource != null && artifactSource.HasMatchingDefinitionId(repositoryId) && (artifactSource.IsGitHubArtifact || artifactSource.TeamProjectId == projectId);
    }

    public static string GetArtifactRepositoryId(ArtifactSource artifactSource)
    {
      if (artifactSource != null && artifactSource.SourceData != null)
      {
        InputValue inputValue1;
        artifactSource.SourceData.TryGetValue("version", out inputValue1);
        if (inputValue1 != null)
        {
          IDictionary<string, object> dictionary = inputValue1?.Data ?? (IDictionary<string, object>) null;
          if (dictionary != null)
          {
            object obj;
            dictionary.TryGetValue("repositoryId", out obj);
            if (obj == null)
            {
              InputValue inputValue2;
              artifactSource.SourceData.TryGetValue("repository", out inputValue2);
              if (inputValue2 != null)
                obj = (object) inputValue2.Value.ToString();
            }
            return obj.ToString();
          }
        }
      }
      return (string) null;
    }

    private static IDictionary<string, string> FetchAllVariables(
      IVssRequestContext context,
      Guid currentProjectId,
      int releaseDefinitionId)
    {
      IVssRequestContext context1 = context.Elevate();
      ReleaseDefinition releaseDefinition = context1.GetService<ReleaseDefinitionsService>().GetReleaseDefinition(context1, currentProjectId, releaseDefinitionId);
      return DictionaryMerger.MergeDictionaries<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, string>>) new IDictionary<string, string>[2]
      {
        (IDictionary<string, string>) releaseDefinition.Variables.Where<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, bool>) (p => p.Value != null && !p.Value.IsSecret)).ToDictionary<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (p => p.Value.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase),
        ArtifactUtility.GetVariableGroupVariables(context1, currentProjectId, releaseDefinition.VariableGroups)
      });
    }

    private static void ResolveVariablesForArtifactSources(
      IList<ArtifactSource> artifactSources,
      IDictionary<string, string> variablesMap)
    {
      foreach (ArtifactSource artifactSource in (IEnumerable<ArtifactSource>) artifactSources)
      {
        string versionBranchFilter = ArtifactUtility.GetArtifactSourceInputs(artifactSource).GetDefaultVersionBranchFilter();
        if (!versionBranchFilter.IsNullOrEmpty<char>())
        {
          string str = ArtifactUtility.ResolveVariable(versionBranchFilter, variablesMap);
          artifactSource.SourceData["defaultVersionBranch"] = new InputValue()
          {
            DisplayValue = str,
            Value = str
          };
        }
      }
    }

    private static void ResolveVariablesForArtifactSourceTriggers(
      IList<ArtifactSourceTrigger> artifactSourceTriggers,
      IDictionary<string, string> variablesMap)
    {
      if (artifactSourceTriggers.IsNullOrEmpty<ArtifactSourceTrigger>())
        return;
      foreach (ArtifactSourceTrigger artifactSourceTrigger in (IEnumerable<ArtifactSourceTrigger>) artifactSourceTriggers)
      {
        if (!artifactSourceTrigger.TriggerConditions.IsNullOrEmpty<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter>())
        {
          foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter triggerCondition in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter>) artifactSourceTrigger.TriggerConditions)
            triggerCondition.SourceBranch = ArtifactUtility.ResolveVariable(triggerCondition.SourceBranch, variablesMap);
        }
      }
    }

    private static string ResolveVariable(
      string sourceBranchVariable,
      IDictionary<string, string> variablesMap)
    {
      if (sourceBranchVariable.IsNullOrEmpty<char>() || !ArtifactUtility.VariableReferenceRegex.Value.IsMatch(sourceBranchVariable))
        return sourceBranchVariable;
      string input = VariableUtility.ExpandVariables(sourceBranchVariable, variablesMap);
      if (ArtifactUtility.VariableReferenceRegex.Value.IsMatch(input))
        throw new ReleaseManagementObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.CannotResolveBranchFilterVariable, (object) input));
      return input;
    }

    private static bool TriggerConditionsContainsVariables(
      IList<ArtifactSourceTrigger> artifactSourceTriggers)
    {
      if (artifactSourceTriggers.IsNullOrEmpty<ArtifactSourceTrigger>())
        return false;
      bool flag = false;
      foreach (ArtifactSourceTrigger artifactSourceTrigger in (IEnumerable<ArtifactSourceTrigger>) artifactSourceTriggers)
      {
        if (!artifactSourceTrigger.TriggerConditions.IsNullOrEmpty<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter>())
        {
          foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter triggerCondition in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter>) artifactSourceTrigger.TriggerConditions)
          {
            if (!triggerCondition.SourceBranch.IsNullOrEmpty<char>() && ArtifactUtility.VariableReferenceRegex.Value.IsMatch(triggerCondition.SourceBranch))
            {
              flag = true;
              break;
            }
          }
        }
      }
      return flag;
    }

    private static bool ArtifactSourcesContainsVariables(IList<ArtifactSource> artifactSources)
    {
      if (artifactSources.IsNullOrEmpty<ArtifactSource>())
        return false;
      bool flag = false;
      foreach (ArtifactSource artifactSource in (IEnumerable<ArtifactSource>) artifactSources)
      {
        string versionBranchFilter = ArtifactUtility.GetArtifactSourceInputs(artifactSource).GetDefaultVersionBranchFilter();
        if (!versionBranchFilter.IsNullOrEmpty<char>() && ArtifactUtility.VariableReferenceRegex.Value.IsMatch(versionBranchFilter))
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    private static IDictionary<string, string> GetVariableGroupVariables(
      IVssRequestContext context,
      Guid project,
      IList<int> variableGroupIds)
    {
      List<VariableGroup> groups = new List<VariableGroup>();
      if (variableGroupIds != null && variableGroupIds.Count > 0)
      {
        IList<VariableGroup> variableGroups = context.GetService<IVariableGroupService>().GetVariableGroups(context.Elevate(), project, (IList<int>) variableGroupIds.ToList<int>());
        if (variableGroups != null && variableGroups.Any<VariableGroup>())
          groups.AddRange((IEnumerable<VariableGroup>) variableGroups);
      }
      return (IDictionary<string, string>) VariableGroupsMerger.MergeVariableGroups((IList<VariableGroup>) groups).Where<KeyValuePair<string, MergedConfigurationVariableValue>>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, bool>) (p => p.Value != null && !p.Value.IsSecret)).ToDictionary<KeyValuePair<string, MergedConfigurationVariableValue>, string, string>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (p => p.Value.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
