// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IGitConflict
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public interface IGitConflict
  {
    Sha1Id MergeBaseCommitId { get; }

    Sha1Id MergeSourceCommitId { get; }

    Sha1Id MergeTargetCommitId { get; }

    GitConflictType ConflictType { get; }

    NormalizedGitPath ConflictPath { get; }

    NormalizedGitPath SourcePath { get; }

    NormalizedGitPath TargetPath { get; }

    Sha1Id BaseObjectId { get; }

    Sha1Id BaseObjectIdForTarget { get; }

    Sha1Id SourceObjectId { get; }

    Sha1Id TargetObjectId { get; }

    GitResolutionStatus ResolutionStatus { get; }

    GitResolutionError ResolutionError { get; }

    byte ResolutionAction { get; }

    byte ResolutionMergeType { get; }

    NormalizedGitPath ResolutionPath { get; }

    Sha1Id ResolutionObjectId { get; }

    Guid ResolvedBy { get; }

    DateTime ResolvedDate { get; }

    Guid ResolutionAuthor { get; }
  }
}
