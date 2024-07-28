// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.MergeCompletedNotification
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class MergeCompletedNotification : PullRequestNotification
  {
    internal MergeCompletedNotification(
      string teamProjectUri,
      Guid repositoryId,
      string repositoryName,
      int pullRequestId,
      Sha1Id? targetBranchTipCommit,
      Sha1Id? sourceCommit,
      string sourceRefName,
      string targetRefName,
      Sha1Id? mergeCommit,
      PullRequestAsyncStatus result,
      Sha1Id conflictResolutionHash,
      IdentityDescriptor lastConflictResolver,
      bool unattendedCompletion,
      bool isConflictResolution,
      Guid completedMergeRequestActor)
      : base(teamProjectUri, repositoryId, repositoryName, pullRequestId)
    {
      this.TargetBranchTipCommit = targetBranchTipCommit;
      this.SourceCommit = sourceCommit;
      this.SourceRefName = sourceRefName;
      this.TargetRefName = targetRefName;
      this.MergeCommit = mergeCommit;
      this.ConflictResolutionHash = conflictResolutionHash;
      this.LastConflictResolver = lastConflictResolver;
      this.Result = result;
      this.UnattendedCompletion = unattendedCompletion;
      this.IsConflictResolution = isConflictResolution;
      this.CompletedMergeRequestActor = completedMergeRequestActor;
    }

    public Sha1Id? TargetBranchTipCommit { get; private set; }

    public Sha1Id? SourceCommit { get; private set; }

    public string SourceRefName { get; private set; }

    public string TargetRefName { get; private set; }

    public Sha1Id? MergeCommit { get; private set; }

    public PullRequestAsyncStatus Result { get; private set; }

    public Sha1Id ConflictResolutionHash { get; private set; }

    public IdentityDescriptor LastConflictResolver { get; private set; }

    public bool UnattendedCompletion { get; private set; }

    public bool IsConflictResolution { get; private set; }

    public Guid CompletedMergeRequestActor { get; private set; }
  }
}
