// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.WellKnownBuildVariables
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [Obsolete("Use BuildVariables instead.")]
  public static class WellKnownBuildVariables
  {
    public const string System = "system";
    public const string CollectionId = "system.collectionId";
    public const string TeamProject = "system.teamProject";
    public const string TeamProjectId = "system.teamProjectId";
    public const string DefinitionId = "system.definitionId";
    public const string HostType = "system.hosttype";
    public const string IsFork = "system.pullRequest.isFork";
    public const string DefinitionName = "build.definitionName";
    public const string DefinitionVersion = "build.definitionVersion";
    public const string QueuedBy = "build.queuedBy";
    public const string QueuedById = "build.queuedById";
    public const string Reason = "build.reason";
    public const string RequestedFor = "build.requestedFor";
    public const string RequestedForId = "build.requestedForId";
    public const string RequestedForEmail = "build.requestedForEmail";
    public const string SourceBranch = "build.sourceBranch";
    public const string SourceBranchName = "build.sourceBranchName";
    public const string SourceVersion = "build.sourceVersion";
    public const string SourceVersionAuthor = "build.sourceVersionAuthor";
    public const string SourceVersionMessage = "build.sourceVersionMessage";
    public const string SourceTfvcShelveset = "build.sourceTfvcShelveset";
    public const string BuildId = "build.buildId";
    public const string BuildUri = "build.buildUri";
    public const string BuildNumber = "build.buildNumber";
    public const string ContainerId = "build.containerId";
    public const string SyncSources = "build.syncSources";
    public const string JobAuthorizeAs = "Job.AuthorizeAs";
    public const string JobAuthorizeAsId = "Job.AuthorizeAsId";
    public const string RepoUri = "build.repository.uri";
    public const string RepositoryId = "build.repository.id";
    public const string RepositoryName = "build.repository.name";
    public const string RepositoryUri = "build.repository.uri";
    public const string DefinitionFolderPath = "build.definitionFolderPath";
  }
}
