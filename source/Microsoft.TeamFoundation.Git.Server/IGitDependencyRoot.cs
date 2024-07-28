// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IGitDependencyRoot
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Policy;
using Microsoft.TeamFoundation.Git.Server.Protocol;
using Microsoft.TeamFoundation.Git.Server.Storage;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal interface IGitDependencyRoot
  {
    IGitKnownFilesProvider CreateKnownFilesProvider(IVssRequestContext rc, OdbId odbId);

    Odb CreateOdb(IVssRequestContext rc, OdbId odbId);

    IGitPackIndexPointerProvider CreatePackIdxPointerProvider(IVssRequestContext rc, OdbId odbId);

    TfsGitRepository CreateRepo(
      IVssRequestContext rc,
      string name,
      RepoKey repoKey,
      bool createdByForking,
      long size,
      bool isDisabled = false,
      bool isInMaintenance = false);

    ITfsGitBlobProvider GetBlobProvider(IVssRequestContext rc);

    ReceivePackTempRepo CreateReceivePackTempRepo(
      IVssRequestContext rc,
      ITfsGitRepository baseRepo,
      GitReceivePackDeserializer packDeserializer,
      FileBufferedStreamBase packStream,
      IBufferStreamFactory bufferStreamFactory,
      IReadOnlyList<TfsGitRefUpdateRequest> refUpdateRequests);

    PushPolicyManager CreatePushPolicyManager(
      IVssRequestContext requestContext,
      IReadOnlyList<ITeamFoundationGitPushPolicy> pushPolicies);

    GitRepoSizeCalculator CreateSizeCalculator(IVssRequestContext rc);

    CodeMigrator CreateRepoMigrator(IVssRequestContext rc);
  }
}
