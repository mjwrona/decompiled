// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.CodeIndexingStatusDetails
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public class CodeIndexingStatusDetails : IndexingStatusDetails
  {
    public CodeIndexingStatusDetails()
    {
      this.BranchesCurrentlyBeingIndexed = (IDictionary<CodeIndexingStatusDetails.RepositoryInfo, CodeIndexingStatusDetails.BranchesCurrentlyIndexing>) new Dictionary<CodeIndexingStatusDetails.RepositoryInfo, CodeIndexingStatusDetails.BranchesCurrentlyIndexing>();
      this.DefaultBranches = (IDictionary<CodeIndexingStatusDetails.RepositoryInfo, CodeIndexingStatusDetails.DefaultBranch>) new Dictionary<CodeIndexingStatusDetails.RepositoryInfo, CodeIndexingStatusDetails.DefaultBranch>();
    }

    public IDictionary<CodeIndexingStatusDetails.RepositoryInfo, CodeIndexingStatusDetails.BranchesCurrentlyIndexing> BranchesCurrentlyBeingIndexed { get; set; }

    public IDictionary<CodeIndexingStatusDetails.RepositoryInfo, CodeIndexingStatusDetails.DefaultBranch> DefaultBranches { get; set; }

    public void AddToBranchesCurrentlyBeingIndexed(
      string projectName,
      string repositoryName,
      List<string> branchesToBeBulkIndexed)
    {
      CodeIndexingStatusDetails.RepositoryInfo key = new CodeIndexingStatusDetails.RepositoryInfo(projectName, repositoryName);
      if (this.BranchesCurrentlyBeingIndexed.ContainsKey(key))
      {
        branchesToBeBulkIndexed.AddRange((IEnumerable<string>) this.BranchesCurrentlyBeingIndexed[key].BranchNames);
        this.BranchesCurrentlyBeingIndexed.Remove(key);
      }
      CodeIndexingStatusDetails.BranchesCurrentlyIndexing currentlyIndexing = new CodeIndexingStatusDetails.BranchesCurrentlyIndexing(branchesToBeBulkIndexed);
      currentlyIndexing.BranchNames = currentlyIndexing.BranchNames.Distinct<string>().ToList<string>();
      this.BranchesCurrentlyBeingIndexed.Add(key, currentlyIndexing);
    }

    public void AddToDefaultBranches(
      string projectName,
      string repositoryName,
      string defaultBranchName)
    {
      CodeIndexingStatusDetails.RepositoryInfo key = new CodeIndexingStatusDetails.RepositoryInfo(projectName, repositoryName);
      CodeIndexingStatusDetails.DefaultBranch defaultBranch = new CodeIndexingStatusDetails.DefaultBranch(defaultBranchName);
      if (this.DefaultBranches.ContainsKey(key))
        this.DefaultBranches.Remove(key);
      this.DefaultBranches.Add(key, defaultBranch);
    }

    public bool IsBranchIndexing(
      IVssRequestContext requestContext,
      string projectName,
      string repoName,
      string branchName,
      string collectionId)
    {
      CodeIndexingStatusDetails.RepositoryInfo repositoryInfo = new CodeIndexingStatusDetails.RepositoryInfo(projectName, repoName);
      if (!string.IsNullOrWhiteSpace(projectName) && !string.IsNullOrWhiteSpace(repoName))
      {
        if (branchName == null)
          branchName = this.GetDefaultBranch(repositoryInfo);
        if (branchName != null)
          return this.GetBranchStatus(repositoryInfo, branchName) || this.IsBranchIndexingLargeRepo(requestContext, projectName, repoName, branchName, collectionId);
      }
      return false;
    }

    public bool IsBranchIndexingLargeRepo(
      IVssRequestContext requestContext,
      string projectName,
      string repoName,
      string branchName,
      string collectionId)
    {
      if (string.IsNullOrWhiteSpace(projectName) || string.IsNullOrWhiteSpace(repoName) || string.IsNullOrWhiteSpace(branchName))
        return false;
      string traceLayer = "Common";
      return CodeBranchInformationHelperForLargeRepos.IsBranchIndexing(new RegistryManagerV2(requestContext, traceLayer), CodeBranchInformationHelperForLargeRepos.GetRegistryKey(projectName, repoName, branchName), collectionId);
    }

    internal string GetDefaultBranch(
      CodeIndexingStatusDetails.RepositoryInfo repositoryInfo)
    {
      return this.DefaultBranches.ContainsKey(repositoryInfo) ? this.DefaultBranches[repositoryInfo].DefaultBranchName.ToString() : (string) null;
    }

    internal bool GetBranchStatus(
      CodeIndexingStatusDetails.RepositoryInfo repositoryInfo,
      string branchName)
    {
      return this.BranchesCurrentlyBeingIndexed.ContainsKey(repositoryInfo) && !string.IsNullOrWhiteSpace(branchName) && this.BranchesCurrentlyBeingIndexed[repositoryInfo].BranchNames.Contains(branchName);
    }

    public class RepositoryInfo : IEquatable<CodeIndexingStatusDetails.RepositoryInfo>
    {
      public string ProjectName { get; set; }

      public string RepositoryName { get; set; }

      public RepositoryInfo(string projectName, string repositoryName)
      {
        this.ProjectName = projectName;
        this.RepositoryName = repositoryName;
      }

      public override bool Equals(object obj) => this.Equals(obj as CodeIndexingStatusDetails.RepositoryInfo);

      public bool Equals(CodeIndexingStatusDetails.RepositoryInfo other) => other != null && this.ProjectName == other.ProjectName && this.RepositoryName == other.RepositoryName;

      public override int GetHashCode() => (-1756218597 * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.ProjectName)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.RepositoryName);
    }

    public class BranchesCurrentlyIndexing : 
      IEquatable<CodeIndexingStatusDetails.BranchesCurrentlyIndexing>
    {
      public List<string> BranchNames { get; set; }

      public BranchesCurrentlyIndexing(List<string> branchNames)
      {
        if (this.BranchNames == null)
          this.BranchNames = new List<string>();
        foreach (string branchName in branchNames)
          this.BranchNames.Add(CustomUtils.GetBranchNameWithoutPrefix("refs/heads/", branchName));
      }

      public override bool Equals(object obj) => this.Equals(obj as CodeIndexingStatusDetails.BranchesCurrentlyIndexing);

      public bool Equals(
        CodeIndexingStatusDetails.BranchesCurrentlyIndexing other)
      {
        return other != null && EqualityComparer<List<string>>.Default.Equals(this.BranchNames, other.BranchNames);
      }

      public override int GetHashCode() => EqualityComparer<List<string>>.Default.GetHashCode(this.BranchNames) - 677506969;
    }

    public class DefaultBranch : IEquatable<CodeIndexingStatusDetails.DefaultBranch>
    {
      public string DefaultBranchName { get; set; }

      public DefaultBranch(string defaultBranchName) => this.DefaultBranchName = CustomUtils.GetBranchNameWithoutPrefix("refs/heads/", defaultBranchName);

      public override bool Equals(object obj) => this.Equals(obj as CodeIndexingStatusDetails.DefaultBranch);

      public bool Equals(CodeIndexingStatusDetails.DefaultBranch other) => other != null && this.DefaultBranchName == other.DefaultBranchName;

      public override int GetHashCode() => EqualityComparer<string>.Default.GetHashCode(this.DefaultBranchName) - 1700112747;
    }
  }
}
