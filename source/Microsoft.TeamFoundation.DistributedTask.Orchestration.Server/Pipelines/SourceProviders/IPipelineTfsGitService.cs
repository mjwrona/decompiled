// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders.IPipelineTfsGitService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders
{
  [DefaultServiceImplementation(typeof (FrameworkPipelineTfsGitService))]
  public interface IPipelineTfsGitService : IVssFrameworkService
  {
    string GetRefHead(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string refName);

    CommitInfo GetCommit(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string version);

    string GetFileContent(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string refName,
      string version,
      string filePath);

    string GetRepositoryUrl(IVssRequestContext requestContext, Guid projectId, string repositoryId);

    TfsGitRepository GetRepositoryInfo(
      IVssRequestContext requestContext,
      string project,
      string repository,
      ServiceEndpoint endpoint = null);

    IList<TfsGitRepository> GetRepositoriesInfo(
      IVssRequestContext requestContext,
      string project,
      IEnumerable<Guid> repositoryIds = null,
      ServiceEndpoint endpoint = null);

    bool HasReadAccess(IVssRequestContext requestContext, string project, string repositoryId);
  }
}
