// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseTriggerUtility
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Migration;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ReleaseTriggerUtility
  {
    public const int TriggerContentMaxLength = 4000;

    public static IEnumerable<ReleaseTriggerBase> GetConsolidatedReleaseDefinitionTriggers(
      this IEnumerable<ReleaseTriggerBase> releaseTriggers)
    {
      if (releaseTriggers == null)
        throw new ArgumentNullException(nameof (releaseTriggers));
      List<ReleaseTriggerBase> definitionTriggers = new List<ReleaseTriggerBase>();
      foreach (IGrouping<\u003C\u003Ef__AnonymousType0<ReleaseTriggerType>, ReleaseTriggerBase> source in releaseTriggers.GroupBy(t => new
      {
        TriggerType = t.TriggerType
      }))
      {
        List<ReleaseTriggerBase> list = source.ToList<ReleaseTriggerBase>();
        switch (source.Key.TriggerType)
        {
          case ReleaseTriggerType.ArtifactSource:
            definitionTriggers.AddRange((IEnumerable<ReleaseTriggerBase>) list.Select<ReleaseTriggerBase, ArtifactSourceTrigger>((Func<ReleaseTriggerBase, ArtifactSourceTrigger>) (t => t.ToArtifactSourceTrigger())).GetConsolidatedArtifactSourceTriggers());
            continue;
          case ReleaseTriggerType.SourceRepo:
            definitionTriggers.AddRange((IEnumerable<ReleaseTriggerBase>) list.Select<ReleaseTriggerBase, SourceRepoTrigger>((Func<ReleaseTriggerBase, SourceRepoTrigger>) (t => t.ToSourceRepoTrigger())).GetConsolidatedSourceRepoTriggers());
            continue;
          case ReleaseTriggerType.ContainerImage:
            definitionTriggers.AddRange((IEnumerable<ReleaseTriggerBase>) list.Select<ReleaseTriggerBase, ContainerImageTrigger>((Func<ReleaseTriggerBase, ContainerImageTrigger>) (t => t.ToContainerImageTrigger())).GetConsolidatedContainerImageTriggers());
            continue;
          case ReleaseTriggerType.Package:
            definitionTriggers.AddRange((IEnumerable<ReleaseTriggerBase>) list.Select<ReleaseTriggerBase, PackageTrigger>((Func<ReleaseTriggerBase, PackageTrigger>) (t => t.ToPackageTrigger())).GetConsolidatedPackageTriggers());
            continue;
          case ReleaseTriggerType.PullRequest:
            definitionTriggers.AddRange((IEnumerable<ReleaseTriggerBase>) list.Select<ReleaseTriggerBase, PullRequestTrigger>((Func<ReleaseTriggerBase, PullRequestTrigger>) (t => t.ToPullRequestTrigger())).GetConsolidatedPullRequestTriggers());
            continue;
          default:
            definitionTriggers.AddRange((IEnumerable<ReleaseTriggerBase>) list);
            continue;
        }
      }
      return (IEnumerable<ReleaseTriggerBase>) definitionTriggers;
    }

    public static IEnumerable<ArtifactSourceTrigger> GetConsolidatedArtifactSourceTriggers(
      this IEnumerable<ArtifactSourceTrigger> artifactSourceTriggers)
    {
      if (artifactSourceTriggers == null)
        throw new ArgumentNullException(nameof (artifactSourceTriggers));
      List<ArtifactSourceTrigger> artifactSourceTriggers1 = new List<ArtifactSourceTrigger>();
      foreach (IGrouping<\u003C\u003Ef__AnonymousType1<string>, ArtifactSourceTrigger> source in artifactSourceTriggers.GroupBy(t => new
      {
        Alias = t.Alias
      }))
      {
        string alias = source.Key.Alias;
        List<ArtifactSourceTrigger> list = source.ToList<ArtifactSourceTrigger>();
        ArtifactSourceTrigger artifactSourceTrigger1 = list.First<ArtifactSourceTrigger>();
        List<ArtifactFilter> artifactFilterList = new List<ArtifactFilter>();
        foreach (ArtifactSourceTrigger artifactSourceTrigger2 in list.Where<ArtifactSourceTrigger>((Func<ArtifactSourceTrigger, bool>) (t => !t.TriggerConditions.IsNullOrEmpty<ArtifactFilter>())))
        {
          foreach (ArtifactFilter triggerCondition in (IEnumerable<ArtifactFilter>) artifactSourceTrigger2.TriggerConditions)
            artifactFilterList.Add(triggerCondition);
        }
        artifactSourceTrigger1.TriggerConditions = (IList<ArtifactFilter>) artifactFilterList;
        artifactSourceTriggers1.Add(artifactSourceTrigger1);
      }
      return (IEnumerable<ArtifactSourceTrigger>) artifactSourceTriggers1;
    }

    public static IEnumerable<ContainerImageTrigger> GetConsolidatedContainerImageTriggers(
      this IEnumerable<ContainerImageTrigger> containerImageTriggerTriggers)
    {
      if (containerImageTriggerTriggers == null)
        throw new ArgumentNullException(nameof (containerImageTriggerTriggers));
      List<ContainerImageTrigger> containerImageTriggers = new List<ContainerImageTrigger>();
      foreach (IGrouping<\u003C\u003Ef__AnonymousType1<string>, ContainerImageTrigger> source in containerImageTriggerTriggers.GroupBy(t => new
      {
        Alias = t.Alias
      }))
      {
        string alias = source.Key.Alias;
        List<ContainerImageTrigger> list = source.ToList<ContainerImageTrigger>();
        ContainerImageTrigger containerImageTrigger1 = list.First<ContainerImageTrigger>();
        List<TagFilter> tagFilterList = new List<TagFilter>();
        foreach (ContainerImageTrigger containerImageTrigger2 in list.Where<ContainerImageTrigger>((Func<ContainerImageTrigger, bool>) (t => !t.TagFilters.IsNullOrEmpty<TagFilter>())))
        {
          foreach (TagFilter tagFilter in (IEnumerable<TagFilter>) containerImageTrigger2.TagFilters)
            tagFilterList.Add(tagFilter);
        }
        containerImageTrigger1.TagFilters = tagFilterList.Count <= 1 ? (IList<TagFilter>) tagFilterList : throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ContainerImageTriggerTagsLimitExceeded, (object) alias, (object) tagFilterList.Count));
        containerImageTriggers.Add(containerImageTrigger1);
      }
      return (IEnumerable<ContainerImageTrigger>) containerImageTriggers;
    }

    public static IEnumerable<PackageTrigger> GetConsolidatedPackageTriggers(
      this IEnumerable<PackageTrigger> packageTriggers)
    {
      if (packageTriggers == null)
        throw new ArgumentNullException(nameof (packageTriggers));
      List<PackageTrigger> consolidatedPackageTriggers = new List<PackageTrigger>();
      foreach (IGrouping<\u003C\u003Ef__AnonymousType1<string>, PackageTrigger> source in packageTriggers.GroupBy(t => new
      {
        Alias = t.Alias
      }))
      {
        string alias = source.Key.Alias;
        List<PackageTrigger> list = source.ToList<PackageTrigger>();
        consolidatedPackageTriggers.Add(list.First<PackageTrigger>());
      }
      return (IEnumerable<PackageTrigger>) consolidatedPackageTriggers;
    }

    public static IEnumerable<PullRequestTrigger> GetConsolidatedPullRequestTriggers(
      this IEnumerable<PullRequestTrigger> pullRequestTriggers)
    {
      if (pullRequestTriggers == null)
        throw new ArgumentNullException(nameof (pullRequestTriggers));
      List<PullRequestTrigger> pullRequestTriggers1 = new List<PullRequestTrigger>();
      foreach (IGrouping<\u003C\u003Ef__AnonymousType2<string>, PullRequestTrigger> source in pullRequestTriggers.GroupBy(t => new
      {
        ArtifactAlias = t.ArtifactAlias
      }))
      {
        string artifactAlias = source.Key.ArtifactAlias;
        List<PullRequestTrigger> list = source.ToList<PullRequestTrigger>();
        PullRequestTrigger pullRequestTrigger1 = list.First<PullRequestTrigger>();
        List<PullRequestFilter> pullRequestFilterList = new List<PullRequestFilter>();
        foreach (PullRequestTrigger pullRequestTrigger2 in list.Where<PullRequestTrigger>((Func<PullRequestTrigger, bool>) (t => !t.TriggerConditions.IsNullOrEmpty<PullRequestFilter>())))
        {
          foreach (PullRequestFilter triggerCondition in (IEnumerable<PullRequestFilter>) pullRequestTrigger2.TriggerConditions)
            pullRequestFilterList.Add(triggerCondition);
        }
        pullRequestTrigger1.TriggerConditions = (IList<PullRequestFilter>) pullRequestFilterList;
        pullRequestTriggers1.Add(pullRequestTrigger1);
      }
      return (IEnumerable<PullRequestTrigger>) pullRequestTriggers1;
    }

    public static IEnumerable<SourceRepoTrigger> GetConsolidatedSourceRepoTriggers(
      this IEnumerable<SourceRepoTrigger> sourceRepoTriggers)
    {
      if (sourceRepoTriggers == null)
        throw new ArgumentNullException(nameof (sourceRepoTriggers));
      List<SourceRepoTrigger> sourceRepoTriggers1 = new List<SourceRepoTrigger>();
      foreach (IGrouping<\u003C\u003Ef__AnonymousType1<string>, SourceRepoTrigger> source in sourceRepoTriggers.GroupBy(t => new
      {
        Alias = t.Alias
      }))
      {
        string alias = source.Key.Alias;
        List<SourceRepoTrigger> list = source.ToList<SourceRepoTrigger>();
        SourceRepoTrigger sourceRepoTrigger1 = list.First<SourceRepoTrigger>();
        List<string> collection = new List<string>();
        foreach (SourceRepoTrigger sourceRepoTrigger2 in list.Where<SourceRepoTrigger>((Func<SourceRepoTrigger, bool>) (t => !t.BranchFilters.IsNullOrEmpty<string>())))
        {
          foreach (string branchFilter in sourceRepoTrigger2.BranchFilters)
            collection.Add(branchFilter);
        }
        sourceRepoTrigger1.BranchFilters.RemoveAll((Predicate<string>) (b => true));
        sourceRepoTrigger1.BranchFilters.AddRange((IEnumerable<string>) collection);
        sourceRepoTriggers1.Add(sourceRepoTrigger1);
      }
      return (IEnumerable<SourceRepoTrigger>) sourceRepoTriggers1;
    }

    public static IList<ReleaseTriggerData> ConsolidateReleaseTriggers(
      this IEnumerable<ReleaseTriggerData> releaseTriggersData)
    {
      if (releaseTriggersData == null)
        throw new ArgumentNullException(nameof (releaseTriggersData));
      List<ReleaseTriggerData> releaseTriggerDataList = new List<ReleaseTriggerData>();
      foreach (IGrouping<\u003C\u003Ef__AnonymousType3<int, ReleaseTriggerType, string>, ReleaseTriggerData> source in releaseTriggersData.GroupBy(t => new
      {
        ReleaseDefinitionId = t.ReleaseDefinitionId,
        TriggerType = t.TriggerType,
        Alias = t.Alias
      }))
      {
        List<ReleaseTriggerData> list = source.ToList<ReleaseTriggerData>();
        if (list.Any<ReleaseTriggerData>())
        {
          ReleaseTriggerData releaseTriggerData = (ReleaseTriggerData) null;
          switch (source.Key.TriggerType)
          {
            case ReleaseTriggerType.ArtifactSource:
              releaseTriggerData = list.GetConsolidatedArtifactSourceTriggerData();
              break;
            case ReleaseTriggerType.SourceRepo:
              releaseTriggerData = list.GetConsolidatedSourceRepoTriggerData();
              break;
            case ReleaseTriggerType.ContainerImage:
              releaseTriggerData = list.GetConsolidatedContainerImageTriggerData();
              break;
            case ReleaseTriggerType.Package:
              releaseTriggerData = list.GetConsolidatedPackageTriggerData();
              break;
            case ReleaseTriggerType.PullRequest:
              releaseTriggerData = list.GetConsolidatedPullRequestTriggerData();
              break;
            default:
              releaseTriggerDataList.AddRange((IEnumerable<ReleaseTriggerData>) list);
              break;
          }
          if (releaseTriggerData != null)
          {
            if (releaseTriggerData.TriggerContent != null && releaseTriggerData.TriggerContent.Length > 4000)
              throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TriggerContentExceededLimit, (object) releaseTriggerData.Alias));
            releaseTriggerDataList.Add(releaseTriggerData);
          }
        }
      }
      return (IList<ReleaseTriggerData>) releaseTriggerDataList;
    }

    public static ReleaseTriggerData GetConsolidatedArtifactSourceTriggerData(
      this IEnumerable<ReleaseTriggerData> triggers)
    {
      ReleaseTriggerData sourceTriggerData = triggers != null ? triggers.FirstOrDefault<ReleaseTriggerData>() : throw new ArgumentNullException(nameof (triggers));
      if (sourceTriggerData == null)
        return (ReleaseTriggerData) null;
      List<ArtifactFilter> artifactFilterList = new List<ArtifactFilter>();
      foreach (ReleaseTriggerData releaseTriggerData in triggers.Where<ReleaseTriggerData>((Func<ReleaseTriggerData, bool>) (t => t.TriggerType == ReleaseTriggerType.ArtifactSource && !string.IsNullOrEmpty(t.TriggerContent))))
      {
        foreach (ArtifactFilter artifactFilter in (IEnumerable<ArtifactFilter>) JsonUtilities.Deserialize<IList<ArtifactFilter>>(releaseTriggerData.TriggerContent))
          artifactFilterList.Add(artifactFilter);
      }
      sourceTriggerData.TriggerContent = JsonConvert.SerializeObject((object) artifactFilterList);
      return sourceTriggerData;
    }

    public static ReleaseTriggerData GetConsolidatedContainerImageTriggerData(
      this IEnumerable<ReleaseTriggerData> triggers)
    {
      ReleaseTriggerData releaseTriggerData = triggers != null ? triggers.FirstOrDefault<ReleaseTriggerData>() : throw new ArgumentNullException(nameof (triggers));
      return releaseTriggerData == null || releaseTriggerData.TriggerType != ReleaseTriggerType.ContainerImage ? (ReleaseTriggerData) null : releaseTriggerData;
    }

    public static ReleaseTriggerData GetConsolidatedPackageTriggerData(
      this IEnumerable<ReleaseTriggerData> triggers)
    {
      ReleaseTriggerData releaseTriggerData = triggers != null ? triggers.FirstOrDefault<ReleaseTriggerData>() : throw new ArgumentNullException(nameof (triggers));
      return releaseTriggerData == null || releaseTriggerData.TriggerType != ReleaseTriggerType.Package ? (ReleaseTriggerData) null : releaseTriggerData;
    }

    public static ReleaseTriggerData GetConsolidatedPullRequestTriggerData(
      this IEnumerable<ReleaseTriggerData> triggers)
    {
      ReleaseTriggerData requestTriggerData = triggers != null ? triggers.FirstOrDefault<ReleaseTriggerData>() : throw new ArgumentNullException(nameof (triggers));
      if (requestTriggerData == null)
        return (ReleaseTriggerData) null;
      List<PullRequestFilter> pullRequestFilterList = new List<PullRequestFilter>();
      PullRequestConfiguration requestConfiguration = (PullRequestConfiguration) null;
      string str = (string) null;
      foreach (ReleaseTriggerData releaseTriggerData in triggers.Where<ReleaseTriggerData>((Func<ReleaseTriggerData, bool>) (t => t.TriggerType == ReleaseTriggerType.PullRequest && !string.IsNullOrEmpty(t.TriggerContent))))
      {
        PullRequestTrigger pullRequestTrigger = JsonUtilities.Deserialize<PullRequestTrigger>(releaseTriggerData.TriggerContent);
        IList<PullRequestFilter> triggerConditions = pullRequestTrigger.TriggerConditions;
        str = pullRequestTrigger.StatusPolicyName;
        foreach (PullRequestFilter pullRequestFilter in (IEnumerable<PullRequestFilter>) triggerConditions)
          pullRequestFilterList.Add(pullRequestFilter);
        if (requestConfiguration == null)
          requestConfiguration = pullRequestTrigger.PullRequestConfiguration;
      }
      PullRequestTrigger pullRequestTrigger1 = new PullRequestTrigger();
      pullRequestTrigger1.ReleaseDefinitionId = requestTriggerData.ReleaseDefinitionId;
      pullRequestTrigger1.ArtifactAlias = requestTriggerData.Alias;
      pullRequestTrigger1.TriggerConditions = (IList<PullRequestFilter>) pullRequestFilterList;
      pullRequestTrigger1.PullRequestConfiguration = requestConfiguration;
      pullRequestTrigger1.StatusPolicyName = str;
      PullRequestTrigger pullRequestTrigger2 = pullRequestTrigger1;
      requestTriggerData.TriggerContent = JsonConvert.SerializeObject((object) pullRequestTrigger2);
      return requestTriggerData;
    }

    public static ReleaseTriggerData GetConsolidatedSourceRepoTriggerData(
      this IEnumerable<ReleaseTriggerData> triggers)
    {
      ReleaseTriggerData sourceRepoTriggerData = triggers != null ? triggers.FirstOrDefault<ReleaseTriggerData>() : throw new ArgumentNullException(nameof (triggers));
      if (sourceRepoTriggerData == null)
        return (ReleaseTriggerData) null;
      List<string> collection = new List<string>();
      foreach (ReleaseTriggerData releaseTriggerData in triggers.Where<ReleaseTriggerData>((Func<ReleaseTriggerData, bool>) (t => t.TriggerType == ReleaseTriggerType.SourceRepo && !string.IsNullOrEmpty(t.TriggerContent))))
      {
        foreach (string branchFilter in JsonUtilities.Deserialize<SourceRepoTrigger>(releaseTriggerData.TriggerContent).BranchFilters)
          collection.Add(branchFilter);
      }
      SourceRepoTrigger sourceRepoTrigger1 = new SourceRepoTrigger();
      sourceRepoTrigger1.ReleaseDefinitionId = sourceRepoTriggerData.ReleaseDefinitionId;
      sourceRepoTrigger1.Alias = sourceRepoTriggerData.Alias;
      SourceRepoTrigger sourceRepoTrigger2 = sourceRepoTrigger1;
      sourceRepoTrigger2.BranchFilters.AddRange((IEnumerable<string>) collection);
      sourceRepoTriggerData.TriggerContent = JsonConvert.SerializeObject((object) sourceRepoTrigger2);
      return sourceRepoTriggerData;
    }
  }
}
