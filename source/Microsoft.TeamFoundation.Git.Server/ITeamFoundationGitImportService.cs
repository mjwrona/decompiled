// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ITeamFoundationGitImportService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationGitImportService))]
  public interface ITeamFoundationGitImportService : IVssFrameworkService
  {
    GitImportRequest CreateImportRequest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      InternalGitImportRequestParameters parameters,
      Guid? projectId = null);

    GitImportRequest GetImportRequestById(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int importOperationId);

    IEnumerable<GitImportRequest> QueryImportRequests(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      bool includeAbandoned);

    GitImportRequest UpdateImportStatus(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int importOperationId,
      GitAsyncOperationStatus status,
      GitImportStatusDetail detailedStatus);
  }
}
