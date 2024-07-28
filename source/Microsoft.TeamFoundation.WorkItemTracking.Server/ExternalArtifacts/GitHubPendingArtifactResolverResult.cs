// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.GitHubPendingArtifactResolverResult
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts
{
  public class GitHubPendingArtifactResolverResult
  {
    public GitHubPendingArtifactResolverResult() => this.FailedRepositories = (ISet<Guid>) new HashSet<Guid>();

    public int ResolvedCommits { get; set; }

    public int ResolvedPullRequests { get; set; }

    public int ResolvedIssues { get; set; }

    public int UnresolvedCommits { get; set; }

    public int UnresolvedPullRequests { get; set; }

    public int UnresolvedIssues { get; set; }

    public int FailedCommits { get; set; }

    public int FailedPullRequests { get; set; }

    public int FailedIssues { get; set; }

    public ISet<Guid> FailedRepositories { get; set; }

    public string ResultMessage => string.Format("Resolved: {0} commits, {1} pull requests and {2} issues {3}\r\nNot resolved: {4} commits, {5} pull requests and {6} issues {7}\r\nFailed to resolve: {8} commits, {9} pull requests and {10} issues {11}\r\nFailed repositories: {12}", (object) this.ResolvedCommits, (object) this.ResolvedPullRequests, (object) this.ResolvedIssues, (object) Environment.NewLine, (object) this.UnresolvedCommits, (object) this.UnresolvedPullRequests, (object) this.UnresolvedIssues, (object) Environment.NewLine, (object) this.FailedCommits, (object) this.FailedPullRequests, (object) this.FailedIssues, (object) Environment.NewLine, (object) string.Join<Guid>(",", (IEnumerable<Guid>) this.FailedRepositories));
  }
}
