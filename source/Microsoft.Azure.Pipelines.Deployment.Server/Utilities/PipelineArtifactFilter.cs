// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Utilities.PipelineArtifactFilter
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.Utilities
{
  public class PipelineArtifactFilter
  {
    private static readonly string BranchPrefix = "refs/heads/";
    private static readonly char PathSeparator = '/';
    private static readonly string InclusionOperator = "+";
    private static readonly string ExclusionOperator = "-";
    private IList<string> m_branchSpecs;
    private IList<string> m_stages;
    private IList<string> m_tags;

    public PipelineArtifactFilter(
      IList<string> branchSpecs = null,
      IList<string> stages = null,
      IList<string> tags = null)
    {
      this.m_branchSpecs = branchSpecs ?? (IList<string>) new List<string>();
      this.m_stages = stages ?? (IList<string>) new List<string>();
      this.m_tags = tags ?? (IList<string>) new List<string>();
    }

    public bool IsFilterMatches(
      string branch = null,
      string eventType = null,
      IList<Stage> stages = null,
      IList<string> tags = null)
    {
      return this.IsBranchIncluded(branch) && this.IsStagesIncluded(eventType, stages) && this.IsTagsIncluded(tags);
    }

    public bool IsBranchIncluded(string branch)
    {
      if (!this.m_branchSpecs.Any<string>())
        return true;
      if (string.IsNullOrEmpty(branch))
        return false;
      bool flag1 = false;
      bool flag2 = false;
      if (!string.IsNullOrEmpty(branch))
      {
        string refName1 = this.BranchToRefName(branch);
        foreach (string branchSpec in (IEnumerable<string>) this.m_branchSpecs)
        {
          string branch1 = branchSpec;
          bool flag3 = PipelineArtifactFilter.IsExcludeFilter(branchSpec);
          flag1 |= !flag3;
          if (flag3 || branchSpec.StartsWith(PipelineArtifactFilter.InclusionOperator, StringComparison.Ordinal))
            branch1 = branchSpec.Substring(1);
          string refName2 = this.BranchToRefName(branch1);
          if (!Wildcard.IsWildcard(refName2) ? refName1.Equals(refName2, StringComparison.Ordinal) : Wildcard.Match(refName1, refName2))
          {
            if (flag3)
              return false;
            flag2 = true;
          }
        }
      }
      return flag2 || !flag1;
    }

    public bool IsStagesIncluded(string eventType, IList<Stage> stages)
    {
      if (!this.m_stages.Any<string>())
        return !"ms.vss-pipelines.stage-completed-event".Equals(eventType, StringComparison.OrdinalIgnoreCase);
      if (stages == null || !stages.Any<Stage>())
        return false;
      List<string> completedStages = stages.Where<Stage>((Func<Stage, bool>) (stage =>
      {
        StageResult? result = stage.Result;
        StageResult stageResult = StageResult.Succeeded;
        return result.GetValueOrDefault() == stageResult & result.HasValue;
      })).Select<Stage, string>((Func<Stage, string>) (stage => stage.Name)).ToList<string>();
      return this.m_stages.All<string>((Func<string, bool>) (s => completedStages.Contains<string>(s, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)));
    }

    public bool IsTagsIncluded(IList<string> tags)
    {
      if (!this.m_tags.Any<string>())
        return true;
      return tags != null && tags.Any<string>() && this.m_tags.All<string>((Func<string, bool>) (t => tags.Contains<string>(t, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)));
    }

    public bool IsImageTagIncluded(string imageTag)
    {
      if (!this.m_tags.Any<string>())
        return true;
      if (string.IsNullOrEmpty(imageTag))
        return false;
      bool flag1 = false;
      bool flag2 = false;
      if (!string.IsNullOrEmpty(imageTag))
      {
        foreach (string tag in (IEnumerable<string>) this.m_tags)
        {
          string str = tag;
          bool flag3 = PipelineArtifactFilter.IsExcludeFilter(tag);
          flag1 |= !flag3;
          if (flag3 || tag.StartsWith(PipelineArtifactFilter.InclusionOperator, StringComparison.Ordinal))
            str = tag.Substring(1);
          if (!Wildcard.IsWildcard(str) ? imageTag.Equals(str, StringComparison.Ordinal) : Wildcard.Match(imageTag, str))
          {
            if (flag3)
              return false;
            flag2 = true;
          }
        }
      }
      return flag2 || !flag1;
    }

    public static bool IsExcludeFilter(string filter) => filter.StartsWith(PipelineArtifactFilter.ExclusionOperator, StringComparison.Ordinal);

    private string BranchToRefName(string branch)
    {
      if (branch.StartsWith("refs/", StringComparison.Ordinal))
        return branch;
      return PipelineArtifactFilter.BranchPrefix + branch.TrimStart(PipelineArtifactFilter.PathSeparator);
    }
  }
}
