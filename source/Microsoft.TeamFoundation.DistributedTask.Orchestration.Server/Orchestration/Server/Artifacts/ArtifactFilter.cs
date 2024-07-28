// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ArtifactFilter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public class ArtifactFilter
  {
    public const string BranchPatternOperator = "*";
    public const string ExcludeOperator = "-";
    public static readonly string BranchPrefix = "refs/heads/";
    public static readonly string RefsPrefix = "refs";
    private string sourceBranch;

    public string SourceBranch
    {
      get => this.sourceBranch;
      set
      {
        if (value == null)
          this.sourceBranch = string.Empty;
        else
          this.sourceBranch = value;
      }
    }

    public IList<string> Tags { get; set; }

    public bool UseBuildDefinitionBranch { get; set; }

    public bool CreateReleaseOnBuildTagging { get; set; }

    public ArtifactFilter()
    {
      this.SourceBranch = string.Empty;
      this.Tags = (IList<string>) new List<string>();
      this.UseBuildDefinitionBranch = false;
    }

    public static string AddBranchPrefix(string branch) => !string.IsNullOrEmpty(branch) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) ArtifactFilter.BranchPrefix, (object) branch.Trim()) : branch;

    public static string AddBranchPrefixIfRequired(string branch) => !string.IsNullOrEmpty(branch) && !branch.StartsWith(ArtifactFilter.RefsPrefix, StringComparison.Ordinal) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) ArtifactFilter.BranchPrefix, (object) branch.Trim()) : branch;

    public static string RemoveBranchPrefix(string branch)
    {
      if (string.IsNullOrEmpty(branch))
        return string.Empty;
      if (branch.Trim().StartsWith(ArtifactFilter.BranchPrefix, StringComparison.Ordinal))
        branch = branch.Trim().Substring(ArtifactFilter.BranchPrefix.Length);
      return branch;
    }

    public static bool TryParseArtifactFilter(
      string conditionValue,
      out ArtifactFilter expectedArtifactFilter)
    {
      return JsonUtilities.TryDeserialize<ArtifactFilter>(conditionValue, out expectedArtifactFilter);
    }

    public bool IsMatchingBranchFilter(string branchFilter, bool performWildcardMatching)
    {
      if (branchFilter == null)
        throw new ArgumentNullException(nameof (branchFilter));
      if (string.IsNullOrEmpty(this.SourceBranch))
        return true;
      string str1 = ArtifactFilter.RemoveBranchPrefix(this.SourceBranch.Trim());
      branchFilter = ArtifactFilter.RemoveBranchPrefix(branchFilter);
      if (str1.Equals(branchFilter, StringComparison.Ordinal))
        return true;
      if (performWildcardMatching && str1.EndsWith("*", StringComparison.Ordinal))
      {
        if (str1.Equals("*", StringComparison.OrdinalIgnoreCase))
          return true;
        string str2 = str1.Substring(0, str1.Length - 1);
        if (branchFilter.StartsWith(str2, StringComparison.Ordinal))
          return true;
      }
      return false;
    }

    public bool IsMatchingTags(IEnumerable<string> tags)
    {
      if (this.Tags == null || !this.Tags.Any<string>())
        return true;
      return tags != null && tags.Any<string>() && this.Tags.All<string>((Func<string, bool>) (elem => tags.ToList<string>().Contains<string>(elem, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)));
    }

    public bool EndsWithBranchPattern() => this.SourceBranch != null && this.SourceBranch.EndsWith("*", StringComparison.Ordinal);

    public bool IsExcludeFilter() => this.SourceBranch != null && this.SourceBranch.StartsWith("-", StringComparison.Ordinal);
  }
}
