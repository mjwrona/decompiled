// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ReleaseTriggerConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ReleaseTriggerConverter
  {
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactSourceTrigger ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactSourceTrigger trigger)
    {
      if (trigger == null)
        throw new ArgumentNullException(nameof (trigger));
      return new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactSourceTrigger()
      {
        ArtifactAlias = trigger.Alias,
        TriggerConditions = trigger.TriggerConditions.ToWebApi()
      };
    }

    public static IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactFilter> ToWebApi(
      this IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter> serverTriggerConditions)
    {
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactFilter> webApi = new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactFilter>();
      if (serverTriggerConditions != null)
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter triggerCondition in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter>) serverTriggerConditions)
          webApi.Add(new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactFilter()
          {
            SourceBranch = triggerCondition.SourceBranch,
            Tags = triggerCondition.Tags,
            UseBuildDefinitionBranch = triggerCondition.UseBuildDefinitionBranch,
            CreateReleaseOnBuildTagging = triggerCondition.CreateReleaseOnBuildTagging
          });
      }
      return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactFilter>) webApi;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduledReleaseTrigger ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger trigger)
    {
      return trigger != null ? new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduledReleaseTrigger()
      {
        Schedule = trigger.Schedule.ToWebApi()
      } : throw new ArgumentNullException(nameof (trigger));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.SourceRepoTrigger ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.SourceRepoTrigger trigger)
    {
      if (trigger == null)
        throw new ArgumentNullException(nameof (trigger));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.SourceRepoTrigger webApi = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.SourceRepoTrigger();
      webApi.Alias = trigger.Alias;
      webApi.BranchFilters.AddRange((IEnumerable<string>) trigger.BranchFilters);
      return webApi;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactSourceTrigger FromWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactSourceTrigger trigger)
    {
      if (trigger == null)
        throw new ArgumentNullException(nameof (trigger));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactSourceTrigger artifactSourceTrigger = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactSourceTrigger();
      artifactSourceTrigger.Alias = trigger.ArtifactAlias;
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactFilter> triggerConditions = trigger.TriggerConditions;
      artifactSourceTrigger.TriggerConditions = triggerConditions != null ? triggerConditions.FromWebApi() : (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter>) null;
      return artifactSourceTrigger;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ContainerImageTrigger ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ContainerImageTrigger trigger)
    {
      if (trigger == null)
        throw new ArgumentNullException(nameof (trigger));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ContainerImageTrigger webApi = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ContainerImageTrigger();
      webApi.Alias = trigger.Alias;
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.TagFilter> tagFilters = trigger.TagFilters;
      webApi.TagFilters = tagFilters != null ? tagFilters.ToWebApi() : (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.TagFilter>) null;
      return webApi;
    }

    public static IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.TagFilter> ToWebApi(
      this IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.TagFilter> serverTagFilters)
    {
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.TagFilter> webApi = new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.TagFilter>();
      if (serverTagFilters != null)
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.TagFilter serverTagFilter in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.TagFilter>) serverTagFilters)
          webApi.Add(new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.TagFilter()
          {
            Pattern = serverTagFilter.Pattern
          });
      }
      return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.TagFilter>) webApi;
    }

    public static IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter> FromWebApi(
      this IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactFilter> webApiTriggerConditions)
    {
      if (webApiTriggerConditions == null)
        throw new ArgumentNullException(nameof (webApiTriggerConditions));
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter> artifactFilterList = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter>();
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactFilter triggerCondition in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactFilter>) webApiTriggerConditions)
      {
        if (triggerCondition.UseBuildDefinitionBranch && !triggerCondition.SourceBranch.IsNullOrEmpty<char>())
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.BranchFilterNotAllowedWhenBuildDefinitionBranchIsUsed));
        artifactFilterList.Add(new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter()
        {
          SourceBranch = triggerCondition.SourceBranch,
          Tags = triggerCondition.Tags,
          UseBuildDefinitionBranch = triggerCondition.UseBuildDefinitionBranch,
          CreateReleaseOnBuildTagging = triggerCondition.CreateReleaseOnBuildTagging
        });
      }
      return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter>) artifactFilterList;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger FromWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduledReleaseTrigger trigger)
    {
      return trigger != null ? new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger()
      {
        Schedule = trigger.Schedule.FromWebApi()
      } : throw new ArgumentNullException(nameof (trigger));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactSourceTrigger ToArtifactSourceTrigger(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseTriggerBase trigger)
    {
      return trigger != null ? trigger as Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactSourceTrigger : throw new ArgumentNullException(nameof (trigger));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactSourceTrigger ToArtifactSourceTrigger(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase trigger)
    {
      return trigger != null ? trigger as Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactSourceTrigger : throw new ArgumentNullException(nameof (trigger));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger ToScheduledReleaseTrigger(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseTriggerBase trigger)
    {
      return trigger != null ? trigger as Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger : throw new ArgumentNullException(nameof (trigger));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduledReleaseTrigger ToScheduledReleaseTrigger(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase trigger)
    {
      return trigger != null ? trigger as Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduledReleaseTrigger : throw new ArgumentNullException(nameof (trigger));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.SourceRepoTrigger ToSourceRepoTrigger(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseTriggerBase trigger)
    {
      return trigger != null ? trigger as Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.SourceRepoTrigger : throw new ArgumentNullException(nameof (trigger));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.SourceRepoTrigger ToSourceRepoTrigger(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase trigger)
    {
      return trigger != null ? trigger as Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.SourceRepoTrigger : throw new ArgumentNullException(nameof (trigger));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.SourceRepoTrigger FromWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.SourceRepoTrigger trigger)
    {
      if (trigger == null)
        throw new ArgumentNullException(nameof (trigger));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.SourceRepoTrigger sourceRepoTrigger = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.SourceRepoTrigger();
      sourceRepoTrigger.Alias = trigger.Alias;
      sourceRepoTrigger.BranchFilters.AddRange((IEnumerable<string>) trigger.BranchFilters);
      return sourceRepoTrigger;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ContainerImageTrigger ToContainerImageTrigger(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseTriggerBase trigger)
    {
      return trigger != null ? trigger as Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ContainerImageTrigger : throw new ArgumentNullException(nameof (trigger));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ContainerImageTrigger ToContainerImageTrigger(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase trigger)
    {
      return trigger != null ? trigger as Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ContainerImageTrigger : throw new ArgumentNullException(nameof (trigger));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ContainerImageTrigger FromWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ContainerImageTrigger trigger)
    {
      if (trigger == null)
        throw new ArgumentNullException(nameof (trigger));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ContainerImageTrigger containerImageTrigger = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ContainerImageTrigger();
      containerImageTrigger.Alias = trigger.Alias;
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.TagFilter> tagFilters = trigger.TagFilters;
      containerImageTrigger.TagFilters = tagFilters != null ? tagFilters.FromWebApi() : (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.TagFilter>) null;
      return containerImageTrigger;
    }

    public static IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.TagFilter> FromWebApi(
      this IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.TagFilter> webApiTagFilters)
    {
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.TagFilter> tagFilterList = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.TagFilter>();
      if (webApiTagFilters != null)
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.TagFilter webApiTagFilter in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.TagFilter>) webApiTagFilters)
          tagFilterList.Add(new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.TagFilter()
          {
            Pattern = webApiTagFilter.Pattern
          });
      }
      return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.TagFilter>) tagFilterList;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PackageTrigger ToPackageTrigger(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseTriggerBase trigger)
    {
      return trigger != null ? trigger as Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PackageTrigger : throw new ArgumentNullException(nameof (trigger));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PackageTrigger ToPackageTrigger(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase trigger)
    {
      return trigger != null ? trigger as Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PackageTrigger : throw new ArgumentNullException(nameof (trigger));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PackageTrigger FromWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PackageTrigger trigger)
    {
      return trigger != null ? new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PackageTrigger()
      {
        Alias = trigger.Alias
      } : throw new ArgumentNullException(nameof (trigger));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PackageTrigger ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PackageTrigger trigger)
    {
      return trigger != null ? new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PackageTrigger()
      {
        Alias = trigger.Alias
      } : throw new ArgumentNullException(nameof (trigger));
    }

    internal static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestTrigger ToPullRequestTrigger(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase trigger)
    {
      return trigger != null ? trigger as Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestTrigger : throw new ArgumentNullException(nameof (trigger));
    }

    internal static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestTrigger ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestTrigger trigger)
    {
      if (trigger == null)
        throw new ArgumentNullException(nameof (trigger));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestTrigger webApi = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestTrigger()
      {
        ArtifactAlias = trigger.ArtifactAlias,
        TriggerConditions = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestFilter>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestFilter>(),
        PullRequestConfiguration = trigger.PullRequestConfiguration.ToWebApi(),
        StatusPolicyName = trigger.StatusPolicyName
      };
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestFilter triggerCondition in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestFilter>) trigger.TriggerConditions)
        webApi.TriggerConditions.Add(triggerCondition.ToWebApi());
      return webApi;
    }

    internal static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestFilter ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestFilter filter)
    {
      if (filter == null)
        throw new ArgumentNullException(nameof (filter));
      return new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestFilter(filter.TargetBranch, filter.Tags);
    }

    internal static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestConfiguration ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestConfiguration configuration)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestConfiguration webApi = configuration != null ? new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestConfiguration()
      {
        UseArtifactReference = configuration.UseArtifactReference
      } : throw new ArgumentNullException(nameof (configuration));
      if (configuration.CodeRepositoryReference != null)
        webApi.CodeRepositoryReference = configuration.CodeRepositoryReference.ToWebApi();
      return webApi;
    }

    internal static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.CodeRepositoryReference ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.CodeRepositoryReference codeRepositoryReference)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.CodeRepositoryReference webApi = codeRepositoryReference != null ? new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.CodeRepositoryReference()
      {
        SystemType = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestSystemType) Enum.Parse(typeof (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestSystemType), codeRepositoryReference.SystemType.ToString()),
        RepositoryReference = (IDictionary<string, ReleaseManagementInputValue>) new Dictionary<string, ReleaseManagementInputValue>()
      } : throw new ArgumentNullException(nameof (codeRepositoryReference));
      if (codeRepositoryReference.RepositoryReference != null)
      {
        foreach (KeyValuePair<string, ReleaseManagementInputValue> keyValuePair in (IEnumerable<KeyValuePair<string, ReleaseManagementInputValue>>) codeRepositoryReference.RepositoryReference)
          webApi.RepositoryReference.Add(keyValuePair.Key, keyValuePair.Value);
      }
      return webApi;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestTrigger ToPullRequestTrigger(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseTriggerBase trigger)
    {
      return trigger != null ? trigger as Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestTrigger : throw new ArgumentNullException(nameof (trigger));
    }

    internal static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestTrigger FromWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestTrigger trigger)
    {
      if (trigger == null)
        throw new ArgumentNullException(nameof (trigger));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestTrigger pullRequestTrigger = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestTrigger()
      {
        ArtifactAlias = trigger.ArtifactAlias,
        TriggerConditions = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestFilter>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestFilter>(),
        StatusPolicyName = trigger.StatusPolicyName
      };
      if (trigger.TriggerConditions != null)
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestFilter triggerCondition in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestFilter>) trigger.TriggerConditions)
          pullRequestTrigger.TriggerConditions.Add(triggerCondition.FromWebApi());
      }
      if (trigger.PullRequestConfiguration != null)
        pullRequestTrigger.PullRequestConfiguration = trigger.PullRequestConfiguration.FromWebApi();
      return pullRequestTrigger;
    }

    internal static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestFilter FromWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestFilter filter)
    {
      if (filter == null)
        throw new ArgumentNullException(nameof (filter));
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestFilter(filter.TargetBranch, filter.Tags);
    }

    internal static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestConfiguration FromWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestConfiguration configuration)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestConfiguration requestConfiguration = configuration != null ? new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestConfiguration(configuration.UseArtifactReference, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.CodeRepositoryReference) null) : throw new ArgumentNullException(nameof (configuration));
      if (configuration.CodeRepositoryReference != null)
        requestConfiguration.CodeRepositoryReference = configuration.CodeRepositoryReference.FromWebApi();
      return requestConfiguration;
    }

    internal static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.CodeRepositoryReference FromWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.CodeRepositoryReference codeRepositoryReference)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.CodeRepositoryReference repositoryReference = codeRepositoryReference != null ? new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.CodeRepositoryReference()
      {
        SystemType = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.PullRequestSystemType) Enum.Parse(typeof (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.PullRequestSystemType), codeRepositoryReference.SystemType.ToString()),
        RepositoryReference = (IDictionary<string, ReleaseManagementInputValue>) new Dictionary<string, ReleaseManagementInputValue>()
      } : throw new ArgumentNullException(nameof (codeRepositoryReference));
      if (codeRepositoryReference.RepositoryReference != null)
      {
        foreach (KeyValuePair<string, ReleaseManagementInputValue> keyValuePair in (IEnumerable<KeyValuePair<string, ReleaseManagementInputValue>>) codeRepositoryReference.RepositoryReference)
          repositoryReference.RepositoryReference.Add(keyValuePair.Key, keyValuePair.Value);
      }
      return repositoryReference;
    }
  }
}
