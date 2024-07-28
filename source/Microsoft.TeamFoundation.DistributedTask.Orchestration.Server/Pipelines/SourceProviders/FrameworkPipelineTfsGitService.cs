// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders.FrameworkPipelineTfsGitService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders
{
  internal class FrameworkPipelineTfsGitService : IPipelineTfsGitService, IVssFrameworkService
  {
    public string GetFileContent(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string refName,
      string version,
      string filePath)
    {
      GitHttpClient client = requestContext.GetClient<GitHttpClient>();
      Guid project = projectId;
      string repositoryId1 = repositoryId;
      string path = filePath;
      GitVersionDescriptor versionDescriptor1 = new GitVersionDescriptor()
      {
        Version = version,
        VersionType = GitVersionType.Commit
      };
      VersionControlRecursionType? recursionLevel = new VersionControlRecursionType?();
      bool? includeContentMetadata = new bool?();
      bool? latestProcessedChange = new bool?();
      bool? download = new bool?();
      GitVersionDescriptor versionDescriptor2 = versionDescriptor1;
      bool? includeContent = new bool?();
      bool? resolveLfs = new bool?();
      bool? sanitize = new bool?();
      CancellationToken cancellationToken = new CancellationToken();
      using (Stream result = client.GetItemContentAsync(project, repositoryId1, path, (string) null, recursionLevel, includeContentMetadata, latestProcessedChange, download, versionDescriptor2, includeContent, resolveLfs, sanitize, (object) null, cancellationToken).Result)
      {
        using (StreamReader streamReader = new StreamReader((Stream) new LimitReadStream(result, 524288L)))
          return streamReader.ReadToEnd();
      }
    }

    public string GetRefHead(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string refName)
    {
      GitHttpClient client = requestContext.GetClient<GitHttpClient>();
      try
      {
        GitHttpClient gitHttpClient = client;
        Guid project = projectId;
        string repositoryId1 = repositoryId;
        string filter = refName;
        bool? nullable = new bool?(true);
        bool? includeLinks = new bool?();
        bool? includeStatuses = new bool?();
        bool? includeMyBranches = new bool?();
        bool? latestStatusesOnly = new bool?();
        bool? peelTags = nullable;
        int? top = new int?();
        CancellationToken cancellationToken = new CancellationToken();
        GitRef gitRef = gitHttpClient.GetRefsAsync(project, repositoryId1, filter, includeLinks, includeStatuses, includeMyBranches, latestStatusesOnly, peelTags, (string) null, top, (string) null, (object) null, cancellationToken).Result.FirstOrDefault<GitRef>();
        return string.IsNullOrEmpty(gitRef?.PeeledObjectId) ? gitRef?.ObjectId : gitRef?.PeeledObjectId;
      }
      catch (VssServiceResponseException ex)
      {
        requestContext.TraceException(0, "DistributedTask", "PipelineSourceProvider", (Exception) ex);
        return (string) null;
      }
    }

    public CommitInfo GetCommit(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string version)
    {
      GitHttpClient client = requestContext.GetClient<GitHttpClient>();
      try
      {
        GitCommit result = client.GetCommitAsync(projectId, repositoryId, version).Result;
        return new CommitInfo()
        {
          CommitId = version,
          Info = new VersionInfo()
          {
            Author = result.Author?.Name,
            Message = result.Comment
          }
        };
      }
      catch (VssServiceResponseException ex)
      {
        requestContext.TraceException(0, "DistributedTask", "PipelineSourceProvider", (Exception) ex);
        return (CommitInfo) null;
      }
    }

    public string GetRepositoryUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId)
    {
      return requestContext.GetClient<GitHttpClient>().GetRepositoryAsync(projectId, repositoryId, (object) null, new CancellationToken()).Result.Url;
    }

    public TfsGitRepository GetRepositoryInfo(
      IVssRequestContext requestContext,
      string project,
      string repository,
      ServiceEndpoint endpoint = null)
    {
      return this.Convert(this.GetGitClient(requestContext, endpoint).GetRepositoryAsync(project, repository, (object) null, new CancellationToken()).SyncResult<GitRepository>());
    }

    public IList<TfsGitRepository> GetRepositoriesInfo(
      IVssRequestContext requestContext,
      string project,
      IEnumerable<Guid> repositoryIds = null,
      ServiceEndpoint endpoint = null)
    {
      HashSet<Guid> repositoryIdHash = repositoryIds == null ? (HashSet<Guid>) null : new HashSet<Guid>(repositoryIds);
      return (IList<TfsGitRepository>) this.GetGitClient(requestContext, endpoint).GetRepositoriesAsync(project).SyncResult<List<GitRepository>>().Where<GitRepository>((Func<GitRepository, bool>) (r => repositoryIdHash == null || repositoryIdHash.Contains(r.Id))).Select<GitRepository, TfsGitRepository>(new Func<GitRepository, TfsGitRepository>(this.Convert)).ToList<TfsGitRepository>();
    }

    private GitHttpClient GetGitClient(IVssRequestContext requestContext, ServiceEndpoint endpoint)
    {
      if (endpoint == null)
        return requestContext.GetClient<GitHttpClient>();
      string key = "apitoken";
      if (endpoint.Url == (Uri) null || !endpoint.Authorization.Parameters.ContainsKey(key))
        throw new ResourceValidationException(TaskResources.EndpointMissingInfo((object) endpoint.Name));
      return new VssConnection(endpoint.Url, (VssCredentials) (FederatedCredential) new VssBasicCredential(string.Empty, endpoint.Authorization.Parameters[key])).GetClient<GitHttpClient>();
    }

    private TfsGitRepository Convert(GitRepository gitRepository) => new TfsGitRepository(gitRepository.Id, gitRepository.Name, gitRepository.ProjectReference.Id, gitRepository.ProjectReference.Name, gitRepository.Url, gitRepository.DefaultBranch);

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public bool HasReadAccess(
      IVssRequestContext requestContext,
      string project,
      string repositoryId)
    {
      return true;
    }
  }
}
