// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Git.IGitData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Git
{
  public interface IGitData
  {
    GitCommit GetCommit(
      IVssRequestContext context,
      Guid projectId,
      Guid repositoryId,
      string commitId);

    IList<GitCommitRef> GetCommits(
      IVssRequestContext context,
      GitQueryCommitsCriteria criteria,
      Guid repositoryId,
      string branch,
      int totalCommits);

    IList<GitBranchStats> GetBranches(IVssRequestContext context, Guid repositoryId);

    IList<GitRepository> GetRepositories(IVssRequestContext context, Guid projectId);

    GitRepository GetRepository(IVssRequestContext context, Guid projectId, string repositoryId);

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Cant do much as this is what the rest api returns")]
    IList<List<GitItem>> GetItemsBatch(
      IVssRequestContext context,
      Guid repositoryId,
      string branch,
      string path);

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Cant do much as this is what the rest api returns")]
    string GetItemContent(
      IVssRequestContext context,
      Guid repositoryId,
      string branch,
      string path);

    IList<GitItem> GetItemsFirstLevel(
      IVssRequestContext context,
      Guid repositoryId,
      string branch,
      string commit);
  }
}
