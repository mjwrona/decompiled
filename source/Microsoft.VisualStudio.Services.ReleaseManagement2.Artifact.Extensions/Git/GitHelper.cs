// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Git.GitHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Git
{
  public class GitHelper
  {
    private readonly IGitData gitData;

    public GitHelper(IGitData gitData) => this.gitData = gitData;

    public IList<GitRepository> GetRepositories(IVssRequestContext context, Guid projectId)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      return this.gitData.GetRepositories(context, projectId);
    }

    public GitRepository GetRepository(
      IVssRequestContext context,
      Guid projectId,
      string repositoryId)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (projectId == Guid.Empty)
        throw new ArgumentException(Resources.ProjectIdNotPresent);
      if (repositoryId == null)
        throw new ArgumentNullException(nameof (repositoryId));
      return this.gitData.GetRepository(context, projectId, repositoryId);
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is required as we need to return error as well")]
    public IList<GitBranchStats> GetBranches(
      IVssRequestContext context,
      string repositoryId,
      out string errorMessage)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (repositoryId == null)
        throw new ArgumentNullException(nameof (repositoryId));
      errorMessage = (string) null;
      Guid result;
      if (Guid.TryParse(repositoryId, out result))
        return this.gitData.GetBranches(context, result);
      errorMessage = Resources.InvalidRepository;
      return (IList<GitBranchStats>) new List<GitBranchStats>();
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is required as we need to return error as well")]
    public GitCommit GetCommit(
      IVssRequestContext context,
      Guid projectId,
      string repositoryId,
      string commitId,
      out string errorMessage)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (repositoryId == null)
        throw new ArgumentNullException(nameof (repositoryId));
      if (commitId == null)
        throw new ArgumentNullException(nameof (commitId));
      errorMessage = (string) null;
      Guid result;
      if (Guid.TryParse(repositoryId, out result))
        return this.gitData.GetCommit(context, projectId, result, commitId);
      errorMessage = Resources.RepositoryIdNotValid;
      return new GitCommit();
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is required as we need to return error as well")]
    public IList<GitCommitRef> GetCommits(
      IVssRequestContext context,
      string repositoryId,
      string branch,
      int totalCommits,
      out string errorMessage)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (repositoryId == null)
        throw new ArgumentNullException(nameof (repositoryId));
      if (branch == null)
        throw new ArgumentNullException(nameof (branch));
      GitQueryCommitsCriteria criteria = new GitQueryCommitsCriteria();
      criteria.ItemPath = "/";
      criteria.ItemVersion = new GitVersionDescriptor()
      {
        Version = branch
      };
      errorMessage = (string) null;
      Guid result;
      if (Guid.TryParse(repositoryId, out result))
        return this.gitData.GetCommits(context, criteria, result, branch, totalCommits);
      errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.RepositoryIdNotValid, (object) repositoryId);
      return (IList<GitCommitRef>) new List<GitCommitRef>();
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Cant do much as this is what the api returns")]
    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is required as we need to return error as well")]
    public IList<List<GitItem>> GetItemsBatch(
      IVssRequestContext context,
      string repositoryId,
      string branch,
      string path,
      out string errorMessage)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (repositoryId == null)
        throw new ArgumentNullException(nameof (repositoryId));
      if (branch == null)
        throw new ArgumentNullException(nameof (branch));
      if (path == null)
        throw new ArgumentNullException(nameof (path));
      errorMessage = (string) null;
      Guid result;
      if (Guid.TryParse(repositoryId, out result))
        return this.gitData.GetItemsBatch(context, result, branch, path);
      errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.RepositoryIdNotValid, (object) repositoryId);
      return (IList<List<GitItem>>) new List<List<GitItem>>();
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Cant do much as this is what the api returns")]
    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is required as we need to return error as well")]
    public string GetItemContent(
      IVssRequestContext context,
      string repositoryId,
      string branch,
      string path,
      out string errorMessage)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (repositoryId == null)
        throw new ArgumentNullException(nameof (repositoryId));
      if (path == null)
        throw new ArgumentNullException(nameof (path));
      errorMessage = (string) null;
      Guid result;
      if (Guid.TryParse(repositoryId, out result))
        return this.gitData.GetItemContent(context, result, branch, path);
      errorMessage = Resources.InvalidRepository;
      return (string) null;
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is required as we need to return error as well")]
    public IList<GitItem> GetItems(
      IVssRequestContext context,
      string repositoryId,
      string branch,
      string commit,
      out string errorMessage)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (repositoryId == null)
        throw new ArgumentNullException(nameof (repositoryId));
      if (branch == null)
        throw new ArgumentNullException(nameof (branch));
      errorMessage = (string) null;
      Guid result;
      if (Guid.TryParse(repositoryId, out result))
        return this.gitData.GetItemsFirstLevel(context, result, branch, commit);
      errorMessage = Resources.InvalidRepository;
      return (IList<GitItem>) new List<GitItem>();
    }
  }
}
