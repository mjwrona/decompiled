// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ConditionsUtility
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ConditionsUtility
  {
    private const string ExcludeBranchOperator = "-";

    public static void PopulateEnvironmentConditions(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactSourceTrigger> triggers)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (triggers == null)
        throw new ArgumentNullException(nameof (triggers));
      int targetEnvironmentId = triggers.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactSourceTrigger>() ? triggers.First<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactSourceTrigger>().TargetEnvironmentId : 0;
      release.PopulateReleaseEnvironmentConditions(targetEnvironmentId);
    }

    public static void PopulateEnvironmentConditions(this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definition)
    {
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      if (definition.Environments.IsNullOrEmpty<DefinitionEnvironment>())
        return;
      DefinitionEnvironment definitionEnvironment1 = definition.Environments.Single<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (env => env.Rank == 1));
      if (definitionEnvironment1.Conditions == null)
        definitionEnvironment1.Conditions = (IList<Condition>) new List<Condition>();
      definitionEnvironment1.Conditions.Add((Condition) ConditionsUtility.NewReleaseStartedCondition());
      DefinitionEnvironment targetedEnvironment = definition.GetTargetedEnvironment();
      DefinitionEnvironment previousEnvironment = definitionEnvironment1;
      DefinitionEnvironment definitionEnvironment2;
      while ((definitionEnvironment2 = definition.Environments.FirstOrDefault<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (env => env.Rank == previousEnvironment.Rank + 1))) != null)
      {
        if (definitionEnvironment2.Conditions == null)
          definitionEnvironment2.Conditions = (IList<Condition>) new List<Condition>();
        if (targetedEnvironment != null && definitionEnvironment2.Rank <= targetedEnvironment.Rank)
          definitionEnvironment2.Conditions.Add((Condition) ConditionsUtility.NewEnvironmentStateCondition(previousEnvironment.Name, EnvironmentStatus.Succeeded));
        previousEnvironment = definitionEnvironment2;
      }
    }

    public static bool EvaluateConditions(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      bool isReleaseStart,
      int completedEnvironmentId,
      out Guid deploymentRequestedFor)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      return environment.EvaluateConditionsImplementation(requestContext, release, isReleaseStart, completedEnvironmentId, out deploymentRequestedFor);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By Design")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    internal static bool EvaluateConditionsImplementation(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      bool isReleaseStart,
      int completedEnvironmentId,
      out Guid deploymentRequestedFor)
    {
      bool flag = true;
      deploymentRequestedFor = Guid.Empty;
      List<ReleaseCondition> artifactConditions = new List<ReleaseCondition>();
      if (environment.Conditions.IsNullOrEmpty<ReleaseCondition>())
        return false;
      deploymentRequestedFor = release.CreatedBy;
      foreach (ReleaseCondition condition in (IEnumerable<ReleaseCondition>) environment.Conditions)
      {
        if (condition.ConditionType == ConditionType.Event && string.Equals(condition.Name, "ReleaseStarted", StringComparison.OrdinalIgnoreCase))
        {
          flag &= isReleaseStart;
          condition.Result = new bool?(flag);
        }
        else if (condition.ConditionType == ConditionType.EnvironmentState)
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environmentByName = release.GetEnvironmentByName(condition.Name);
          EnvironmentStatus environmentStatus;
          if (ConditionsUtility.TryParseEnvironmentStatus(condition.Value, out environmentStatus))
          {
            if (completedEnvironmentId != 0 && requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.ValidateReleaseEnvironmentConditions"))
            {
              Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment completedEnvironment = release.GetEnvironment(completedEnvironmentId);
              if (completedEnvironment != null && !environment.Conditions.Where<ReleaseCondition>((Func<ReleaseCondition, bool>) (x => x.ConditionType == ConditionType.EnvironmentState && x.Name == completedEnvironment.Name)).Any<ReleaseCondition>())
              {
                flag = false;
                condition.Result = new bool?(false);
                continue;
              }
            }
            flag = flag && environmentByName != null && (environmentByName.Status.ToWebApi() & environmentStatus) != 0;
            condition.Result = new bool?(flag);
            if (flag && ConditionsUtility.CheckCompletedEventEnvironmentId(requestContext, completedEnvironmentId, environmentByName))
            {
              Guid deploymentRequestedFor1 = environmentByName.GetLatestDeploymentRequestedFor();
              if (deploymentRequestedFor1 != Guid.Empty)
                deploymentRequestedFor = deploymentRequestedFor1;
            }
          }
        }
        else if (condition.ConditionType == ConditionType.Artifact)
        {
          bool? nullable = ConditionsUtility.IsValidArtifactCondition((Condition) condition, release);
          if (nullable.HasValue)
          {
            if (!nullable.Value)
            {
              flag = false;
              condition.Result = new bool?(false);
              requestContext.Trace(1980006, TraceLevel.Verbose, "ReleaseManagementService", "Pipeline", "ConditionUtilitiy: Conditions not satisfied because artifact {0} not found", (object) condition.Name);
            }
            else
              artifactConditions.Add(condition);
          }
        }
        else
          flag = false;
      }
      bool conditionsImplementation = flag && ConditionsUtility.EvaluateArtifactsConditions(requestContext, (IList<ReleaseCondition>) artifactConditions, release);
      if (!conditionsImplementation)
        deploymentRequestedFor = Guid.Empty;
      return conditionsImplementation;
    }

    private static bool CheckCompletedEventEnvironmentId(
      IVssRequestContext requestContext,
      int completedEnvironmentId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environmentToCheck)
    {
      if (requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.SkipCompletedEventEnvironmentIdCheck"))
        return true;
      return completedEnvironmentId != 0 && environmentToCheck.Id == completedEnvironmentId;
    }

    public static bool? IsValidArtifactCondition(Condition condition, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (condition == null)
        throw new ArgumentNullException(nameof (condition));
      if (condition.ConditionType != ConditionType.Artifact || string.IsNullOrEmpty(condition.Value))
        return new bool?();
      if (!Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter.TryParseArtifactFilter(condition.Value, out Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter _))
        return new bool?();
      return (PipelineArtifactSource) release.GetArtifactByAlias(condition.Name) != null ? new bool?(true) : new bool?(false);
    }

    public static bool EvaluateArtifactsConditions(
      IVssRequestContext requestContext,
      IList<ReleaseCondition> artifactConditions,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (artifactConditions == null)
        throw new ArgumentNullException(nameof (artifactConditions));
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (artifactConditions.Count == 0)
        return true;
      bool artifactsConditions = false;
      foreach (KeyValuePair<string, IList<ReleaseCondition>> keyValuePair in ConditionsUtility.GroupArtifactConditionsByArtifact(artifactConditions))
      {
        PipelineArtifactSource artifactByAlias = (PipelineArtifactSource) release.GetArtifactByAlias(keyValuePair.Key);
        artifactsConditions = ConditionsUtility.EvaluateArtifactConditions(requestContext, keyValuePair.Value, artifactByAlias);
        if (!artifactsConditions)
          break;
      }
      return artifactsConditions;
    }

    private static bool EvaluateArtifactConditions(
      IVssRequestContext requestContext,
      IList<ReleaseCondition> conditions,
      PipelineArtifactSource artifact)
    {
      IList<ReleaseCondition> includeConditions;
      IList<ReleaseCondition> excludeConditions;
      ConditionsUtility.SplitArtifactConditions(conditions, out includeConditions, out excludeConditions);
      bool artifactConditions = false;
      if (includeConditions.Count == 0)
      {
        ReleaseCondition releaseCondition = new ReleaseCondition(artifact.Alias, ConditionType.Artifact, "{\"sourceBranch\":\"*\",\"tags\":[]}", new bool?());
        includeConditions.Add(releaseCondition);
      }
      foreach (ReleaseCondition releaseCondition in (IEnumerable<ReleaseCondition>) includeConditions)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter expectedArtifactFilter;
        if (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter.TryParseArtifactFilter(releaseCondition.Value, out expectedArtifactFilter) && ArtifactFilterUtility.EvaluateFilters(requestContext, expectedArtifactFilter, artifact))
          artifactConditions = ConditionsUtility.ValidateConditionIsNotExcluded(requestContext, expectedArtifactFilter, excludeConditions, artifact);
        releaseCondition.Result = new bool?(artifactConditions);
        if (artifactConditions)
          break;
      }
      return artifactConditions;
    }

    private static bool ValidateConditionIsNotExcluded(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter includedArtifactFilter,
      IList<ReleaseCondition> excludeConditions,
      PipelineArtifactSource artifact)
    {
      bool flag = true;
      if (excludeConditions.Count == 0)
        return true;
      foreach (ReleaseCondition excludeCondition in (IEnumerable<ReleaseCondition>) excludeConditions)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter expectedArtifactFilter;
        if (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter.TryParseArtifactFilter(excludeCondition.Value, out expectedArtifactFilter))
        {
          expectedArtifactFilter.SourceBranch = expectedArtifactFilter.SourceBranch.Substring(1);
          if (ArtifactFilterUtility.IsBranchMatched(requestContext, expectedArtifactFilter, artifact.SourceBranch) && !ArtifactFilterUtility.EvaluateIncludeExcludeConflict(includedArtifactFilter, expectedArtifactFilter))
          {
            excludeCondition.Result = new bool?(false);
            flag = false;
            break;
          }
        }
      }
      return flag;
    }

    private static Dictionary<string, IList<ReleaseCondition>> GroupArtifactConditionsByArtifact(
      IList<ReleaseCondition> artifactConditions)
    {
      Dictionary<string, IList<ReleaseCondition>> dictionary = new Dictionary<string, IList<ReleaseCondition>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (ReleaseCondition artifactCondition in (IEnumerable<ReleaseCondition>) artifactConditions)
      {
        if (!string.IsNullOrEmpty(artifactCondition.Value))
        {
          if (!dictionary.ContainsKey(artifactCondition.Name))
            dictionary.Add(artifactCondition.Name, (IList<ReleaseCondition>) new List<ReleaseCondition>());
          dictionary[artifactCondition.Name].Add(artifactCondition);
        }
      }
      return dictionary;
    }

    private static void SplitArtifactConditions(
      IList<ReleaseCondition> artifactConditions,
      out IList<ReleaseCondition> includeConditions,
      out IList<ReleaseCondition> excludeConditions)
    {
      includeConditions = (IList<ReleaseCondition>) new List<ReleaseCondition>();
      excludeConditions = (IList<ReleaseCondition>) new List<ReleaseCondition>();
      foreach (ReleaseCondition artifactCondition in (IEnumerable<ReleaseCondition>) artifactConditions)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter expectedArtifactFilter;
        if (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter.TryParseArtifactFilter(artifactCondition.Value, out expectedArtifactFilter) && expectedArtifactFilter.SourceBranch.StartsWith("-", StringComparison.Ordinal))
          excludeConditions.Add(artifactCondition);
        else
          includeConditions.Add(artifactCondition);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Handling conditions null check in the code")]
    public static IList<Condition> ConvertToWebApiConditions(IList<Condition> conditions)
    {
      if (!conditions.IsNullOrEmpty<Condition>())
      {
        foreach (Condition condition in (IEnumerable<Condition>) conditions)
          ConditionsUtility.ConvertToWebApiCondition(condition);
      }
      return conditions;
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Handling conditions null check in the code")]
    public static IList<Condition> RemovePropertiesThatHasDefaultValues(
      IVssRequestContext requestContext,
      IList<Condition> conditions)
    {
      if (!conditions.IsNullOrEmpty<Condition>())
      {
        foreach (Condition condition in (IEnumerable<Condition>) conditions)
          ConditionsUtility.RemovePropertyThatHasDefaultValue(requestContext, condition);
      }
      return conditions;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "Always return environment status in int format")]
    public static void ConvertToWebApiCondition(Condition condition)
    {
      if (condition == null)
        throw new ArgumentNullException(nameof (condition));
      EnvironmentStatus environmentStatus;
      if (!ConditionsUtility.TryParseEnvironmentStatus(condition.Value, out environmentStatus))
        return;
      condition.Value = ((int) environmentStatus).ToString();
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions must be caught inorder to prevent them from being thrown")]
    private static void RemovePropertyThatHasDefaultValue(
      IVssRequestContext requestContext,
      Condition condition)
    {
      if (condition == null)
        throw new ArgumentNullException(nameof (condition));
      try
      {
        if (condition.ConditionType != ConditionType.Artifact || condition.Value.IsNullOrEmpty<char>())
          return;
        string str = JsonConvert.SerializeObject((object) JsonUtilities.Deserialize<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter>(condition.Value, true), new VssJsonMediaTypeFormatter().SerializerSettings);
        condition.Value = str;
      }
      catch (Exception ex)
      {
        requestContext.Trace(1980013, TraceLevel.Info, "ReleaseManagementService", "Pipeline", "ConditionUtilitiy: Exception occured while removing properties with default values in artifact conditions. Error: {0}", (object) ex);
      }
    }

    private static bool TryParseEnvironmentStatus(
      string conditionValue,
      out EnvironmentStatus environmentStatus)
    {
      if (conditionValue == null)
      {
        environmentStatus = EnvironmentStatus.Undefined;
        return false;
      }
      if (!conditionValue.Trim().Equals("3", StringComparison.OrdinalIgnoreCase))
        return Enum.TryParse<EnvironmentStatus>(conditionValue, true, out environmentStatus);
      environmentStatus = EnvironmentStatus.Succeeded;
      return true;
    }

    private static void PopulateReleaseEnvironmentConditions(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int targetEnvironmentId)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment1 = release.Environments.Single<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (env => env.Rank == 1));
      releaseEnvironment1.Conditions.Add(ConditionsUtility.NewReleaseStartedCondition());
      if (targetEnvironmentId <= 0)
        return;
      int rank = release.Environments.Single<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (env => env.DefinitionEnvironmentId == targetEnvironmentId)).Rank;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment previousEnvironment = releaseEnvironment1;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment2;
      while ((releaseEnvironment2 = release.Environments.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (env => env.Rank == previousEnvironment.Rank + 1))) != null && releaseEnvironment2.Rank <= rank)
      {
        releaseEnvironment2.Conditions.Add(ConditionsUtility.NewEnvironmentStateCondition(previousEnvironment.Name, EnvironmentStatus.Succeeded));
        previousEnvironment = releaseEnvironment2;
      }
    }

    private static ReleaseCondition NewReleaseStartedCondition() => new ReleaseCondition("ReleaseStarted", ConditionType.Event, (string) null, new bool?());

    private static ReleaseCondition NewEnvironmentStateCondition(
      string environmentName,
      EnvironmentStatus expectedStatus)
    {
      return new ReleaseCondition(environmentName, ConditionType.EnvironmentState, expectedStatus.ToString(), new bool?());
    }
  }
}
