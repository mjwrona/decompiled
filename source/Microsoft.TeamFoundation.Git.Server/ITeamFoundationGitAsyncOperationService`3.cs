// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ITeamFoundationGitAsyncOperationService`3
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public interface ITeamFoundationGitAsyncOperationService<TOperation, TParam, TStatus> : 
    IVssFrameworkService
    where TOperation : GitAsyncOperation<TParam, TStatus>, new()
  {
    TOperation CreateAsyncOperation(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      GitAsyncOperationType operationType,
      TParam parameters);

    TOperation GetAsyncOperationById(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int operationId,
      bool throwIfNotFound = true);

    IEnumerable<TOperation> QueryAsyncOperationsByType(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      GitAsyncOperationType operationType);

    void UpdateAsyncOperationStatus(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int operationId,
      GitAsyncOperationStatus status,
      TStatus detailedStatus,
      GitAsyncOperationStatus? previousStatus = null);
  }
}
