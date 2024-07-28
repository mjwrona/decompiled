// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Builders.ReleaseBuilderBase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Builders
{
  public class ReleaseBuilderBase
  {
    private readonly Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition, IVssRequestContext, string> releaseNameFiller;

    protected ReleaseBuilderBase()
      : this(ReleaseBuilderBase.\u003C\u003EO.\u003C0\u003E__FillReleaseNameWhileCreatingRelease ?? (ReleaseBuilderBase.\u003C\u003EO.\u003C0\u003E__FillReleaseNameWhileCreatingRelease = new Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition, IVssRequestContext, string>(ReleaseNameFormatter.FillReleaseNameWhileCreatingRelease)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    protected ReleaseBuilderBase(
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition, IVssRequestContext, string> releaseNameFillerParameter)
    {
      this.releaseNameFiller = releaseNameFillerParameter;
    }

    protected Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition, IVssRequestContext, string> ReleaseNameFiller => this.releaseNameFiller;

    protected static void PopulateArtifactData(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition definition,
      IList<ArtifactMetadata> releaseArtifactData,
      IVssRequestContext requestContext,
      string triggeringArtifactAlias)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) definition.LinkedArtifacts)
      {
        ArtifactSource artifactSource = linkedArtifact.Clone();
        artifactSource.SourceData["version"] = new InputValue();
        if (!artifactSource.SourceData.ContainsKey("TriggeringArtifactAlias") && !string.IsNullOrEmpty(triggeringArtifactAlias) && linkedArtifact.Alias.Equals(triggeringArtifactAlias, StringComparison.OrdinalIgnoreCase))
        {
          InputValue inputValue = new InputValue()
          {
            DisplayValue = bool.TrueString,
            Value = bool.TrueString
          };
          artifactSource.SourceData.Add("IsTriggeringArtifact", inputValue);
        }
        if (release.Status != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Draft)
        {
          if (artifactSource.SourceData.ContainsKey("defaultVersionType"))
            artifactSource.SourceData.Remove("defaultVersionType");
          if (artifactSource.SourceData.ContainsKey("defaultVersionBranch"))
            artifactSource.SourceData.Remove("defaultVersionBranch");
          if (artifactSource.SourceData.ContainsKey("defaultVersionTags"))
            artifactSource.SourceData.Remove("defaultVersionTags");
          if (artifactSource.SourceData.ContainsKey("defaultVersionSpecific"))
            artifactSource.SourceData.Remove("defaultVersionSpecific");
        }
        ArtifactMetadata artifactMetadata = ReleaseBuilderBase.GetMatchingArtifactMetadata(artifactSource.Alias, releaseArtifactData, requestContext);
        if (artifactMetadata != null)
        {
          BuildVersion instanceReference = artifactMetadata.InstanceReference;
          artifactSource.SourceData["version"] = new InputValue()
          {
            Value = instanceReference.Id,
            DisplayValue = instanceReference.Name,
            Data = (IDictionary<string, object>) new Dictionary<string, object>()
            {
              {
                "branch",
                (object) instanceReference.SourceBranch
              }
            }
          };
          if (artifactSource.IsMultiDefinitionType && !string.IsNullOrEmpty(instanceReference.Id) && string.IsNullOrEmpty(instanceReference.DefinitionId))
            throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MultiBuildDefinitionIdNotFound, (object) linkedArtifact.Alias));
          if (!string.IsNullOrEmpty(instanceReference.DefinitionId))
          {
            if (artifactSource.SourceData.ContainsKey(nameof (definition)))
              artifactSource.SourceData.Remove(nameof (definition));
            if (artifactSource.SourceData.ContainsKey("definitions"))
              artifactSource.SourceData.Remove("definitions");
            artifactSource.SourceData[nameof (definition)] = new InputValue()
            {
              Value = instanceReference.DefinitionId,
              DisplayValue = instanceReference.DefinitionName
            };
          }
          if (!string.IsNullOrEmpty(instanceReference.SourceVersion))
            artifactSource.SourceData["version"].Data.Add("sourceVersion", (object) artifactMetadata.InstanceReference.SourceVersion);
          if (!string.IsNullOrEmpty(instanceReference.SourceRepositoryId))
            artifactSource.SourceData["version"].Data.Add("repositoryId", (object) artifactMetadata.InstanceReference.SourceRepositoryId);
          if (!string.IsNullOrEmpty(instanceReference.SourceRepositoryType))
            artifactSource.SourceData["version"].Data.Add("repositoryType", (object) artifactMetadata.InstanceReference.SourceRepositoryType);
          if (artifactSource.IsMultiDefinitionType)
            ArtifactExtensions.PopulateArtifactWithSourceId(requestContext, artifactSource, false);
          if (instanceReference.SourceBranch != null)
          {
            artifactSource.SourceBranch = artifactMetadata.InstanceReference.SourceBranch;
            string branchName = ArtifactTypeBase.RefToBranchName(artifactMetadata.InstanceReference.SourceBranch);
            artifactSource.SourceData["branches"] = new InputValue()
            {
              Value = branchName,
              DisplayValue = branchName
            };
          }
          if (instanceReference.SourcePullRequestVersion != null)
          {
            SourcePullRequestVersion pullRequestVersion = instanceReference.SourcePullRequestVersion;
            if (!string.IsNullOrEmpty(pullRequestVersion.PullRequestId))
              artifactSource.SourceData[WellKnownPullRequestVariables.PullRequestId] = new InputValue()
              {
                Value = pullRequestVersion.PullRequestId
              };
            if (!string.IsNullOrEmpty(pullRequestVersion.SourceBranchCommitId))
              artifactSource.SourceData[WellKnownPullRequestVariables.PullRequestSourceBranchCommitId] = new InputValue()
              {
                Value = pullRequestVersion.SourceBranchCommitId
              };
            if (pullRequestVersion.PullRequestMergedAt.HasValue)
              artifactSource.SourceData[WellKnownPullRequestVariables.PullRequestMergedAt] = new InputValue()
              {
                Value = pullRequestVersion.PullRequestMergedAt.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture)
              };
            if (!string.IsNullOrEmpty(pullRequestVersion.TargetBranch))
              artifactSource.SourceData[WellKnownPullRequestVariables.PullRequestTargetBranch] = new InputValue()
              {
                Value = pullRequestVersion.TargetBranch
              };
            if (!string.IsNullOrEmpty(pullRequestVersion.SourceBranch))
              artifactSource.SourceData[WellKnownPullRequestVariables.PullRequestSourceBranch] = new InputValue()
              {
                Value = pullRequestVersion.SourceBranch
              };
            if (!string.IsNullOrEmpty(pullRequestVersion.IterationId))
              artifactSource.SourceData[WellKnownPullRequestVariables.PullRequestIterationId] = new InputValue()
              {
                Value = pullRequestVersion.IterationId
              };
          }
        }
        else if (release.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Active)
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseArtifactVersionIdCannotBeEmpty, (object) artifactSource.Alias));
        ReleaseBuilderBase.Trace(requestContext, 1973116, "ReleaseArtifactSource Alias: {0}, SourceData: {1}, SourceBranch: {2}", TraceLevel.Info, (object) artifactSource.Alias, (object) Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) artifactSource.SourceData), (object) artifactSource.SourceBranch);
        release.LinkedArtifacts.Add(artifactSource);
      }
      ReleaseBuilderBase.PopulatePullRequestDataInArtifact(definition, release);
    }

    protected static void Trace(
      IVssRequestContext requestContext,
      int tracePoint,
      string traceMessageFormat,
      TraceLevel traceLevel,
      params object[] messageParameters)
    {
      VssRequestContextExtensions.Trace(requestContext, tracePoint, traceLevel, "ReleaseManagementService", "Service", traceMessageFormat, messageParameters);
    }

    private static void PopulatePullRequestDataInArtifact(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (release.Reason != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.PullRequest)
        return;
      IDictionary<string, IDictionary<string, InputValue>> requestDataMapping = ReleaseBuilderBase.GetArtifactPullRequestDataMapping(releaseDefinition);
      foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) release.LinkedArtifacts)
      {
        if (requestDataMapping.ContainsKey(linkedArtifact.Alias))
        {
          foreach (KeyValuePair<string, InputValue> keyValuePair in (IEnumerable<KeyValuePair<string, InputValue>>) requestDataMapping[linkedArtifact.Alias])
          {
            if (keyValuePair.Key == "pullRequestProjectId")
              linkedArtifact.SourceData["project"] = keyValuePair.Value;
            else if (keyValuePair.Key == "pullRequestRepositoryId")
              linkedArtifact.SourceData["repository"] = keyValuePair.Value;
            else
              linkedArtifact.SourceData[keyValuePair.Key] = keyValuePair.Value;
          }
        }
      }
    }

    private static IDictionary<string, IDictionary<string, InputValue>> GetArtifactPullRequestDataMapping(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition)
    {
      IDictionary<string, IDictionary<string, InputValue>> requestDataMapping = (IDictionary<string, IDictionary<string, InputValue>>) new Dictionary<string, IDictionary<string, InputValue>>();
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseTriggerBase trigger in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseTriggerBase>) releaseDefinition.Triggers)
      {
        if (trigger.TriggerType.Equals((object) Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseTriggerType.PullRequest))
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestTrigger pullRequestTrigger = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestTrigger) trigger;
          requestDataMapping.Add(pullRequestTrigger.ArtifactAlias, pullRequestTrigger.GetPullRequestDictionaryObject(releaseDefinition));
        }
      }
      return requestDataMapping;
    }

    private static ArtifactMetadata GetMatchingArtifactMetadata(
      string alias,
      IList<ArtifactMetadata> artifactsMetadata,
      IVssRequestContext requestContext)
    {
      foreach (ArtifactMetadata artifactMetadata in (IEnumerable<ArtifactMetadata>) artifactsMetadata)
      {
        ReleaseBuilderBase.Trace(requestContext, 1973116, "Matching artifactMetadata.Alias: {0}, alias: {1}", TraceLevel.Verbose, (object) artifactMetadata.Alias, (object) alias);
        if (string.Equals(artifactMetadata.Alias, alias, StringComparison.OrdinalIgnoreCase))
          return artifactMetadata;
      }
      return (ArtifactMetadata) null;
    }
  }
}
