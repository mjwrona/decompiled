// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders.PlatformTfsGitService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Tfs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3EB20FA-6669-4C21-BA19-EC9C2EBF5243
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Tfs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders
{
  internal class PlatformTfsGitService : IPipelineTfsGitService, IVssFrameworkService
  {
    private const int c_tfsGitSourceProviderTracePoint = 1013403;
    private static readonly RegistryQuery s_sourceProviderGetFileContentMaxLength = (RegistryQuery) (RegistryKeys.RegistrySettingsPath + "SourceProviderGetFileContentMaxLength");

    public string GetFileContent(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string refName,
      string version,
      string filePath)
    {
      using (new MethodScope(requestContext, nameof (PlatformTfsGitService), nameof (GetFileContent)))
      {
        using (ITfsGitRepository repositoryByName = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryByName(requestContext, projectId.ToString(), repositoryId, useCache: true))
        {
          TfsGitObject tfsGitObject = TfsGitDiffHelper.WalkPath(repositoryByName.LookupObject<TfsGitCommit>(new Sha1Id(version)).GetTree(), new NormalizedGitPath(filePath));
          if (tfsGitObject == null || tfsGitObject.ObjectType != GitObjectType.Blob)
            throw new FileNotFoundException(TfsResources.FileNotFoundInRepo((object) filePath, (object) repositoryByName.GetRepositoryWebUri(), (object) refName, (object) version));
          int maxSize = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in PlatformTfsGitService.s_sourceProviderGetFileContentMaxLength, false, 524288);
          using (Stream content = (tfsGitObject as TfsGitBlob).GetContent())
          {
            using (StreamReader streamReader = new StreamReader((Stream) new LimitReadStream(content, (long) maxSize)))
              return streamReader.ReadToEnd();
          }
        }
      }
    }

    public string GetRefHead(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string refName)
    {
      using (new MethodScope(requestContext, nameof (PlatformTfsGitService), nameof (GetRefHead)))
      {
        ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
        ITfsGitRepository repositoryByName;
        try
        {
          repositoryByName = service.FindRepositoryByName(requestContext, projectId.ToString(), repositoryId);
        }
        catch (GitRepositoryNotFoundException ex)
        {
          requestContext.TraceException(1013403, TraceLevel.Error, "DistributedTask", nameof (PlatformTfsGitService), (Exception) ex);
          return (string) null;
        }
        using (repositoryByName)
        {
          TfsGitRef tagRef = repositoryByName.Refs.MatchingName(refName);
          if (tagRef == null)
            return (string) null;
          string refHead = tagRef.ObjectId.ToString();
          try
          {
            TfsGitObject peeledObject;
            if (new AnnotatedTagPeeler(repositoryByName).TryPeelTagRef(tagRef, out peeledObject))
            {
              if (peeledObject != null)
                refHead = peeledObject.ObjectId.ToString();
            }
          }
          catch (GitObjectDoesNotExistException ex)
          {
            requestContext.TraceException(nameof (PlatformTfsGitService), (Exception) ex);
          }
          return refHead;
        }
      }
    }

    public CommitInfo GetCommit(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string version)
    {
      using (new MethodScope(requestContext, nameof (PlatformTfsGitService), nameof (GetCommit)))
      {
        ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
        ITfsGitRepository repositoryByName;
        try
        {
          repositoryByName = service.FindRepositoryByName(requestContext, projectId.ToString(), repositoryId, useCache: true);
        }
        catch (GitRepositoryNotFoundException ex)
        {
          requestContext.TraceException(1013403, TraceLevel.Error, "DistributedTask", nameof (PlatformTfsGitService), (Exception) ex);
          return (CommitInfo) null;
        }
        using (repositoryByName)
        {
          TfsGitCommit tfsGitCommit;
          try
          {
            tfsGitCommit = repositoryByName.LookupObject<TfsGitCommit>(new Sha1Id(version));
          }
          catch (GitObjectDoesNotExistException ex)
          {
            requestContext.TraceException(nameof (PlatformTfsGitService), (Exception) ex);
            return (CommitInfo) null;
          }
          return new CommitInfo()
          {
            CommitId = version,
            Info = new VersionInfo()
            {
              Author = tfsGitCommit.GetAuthor()?.Name,
              Message = tfsGitCommit.GetComment()
            }
          };
        }
      }
    }

    public string GetRepositoryUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId)
    {
      using (new MethodScope(requestContext, nameof (PlatformTfsGitService), nameof (GetRepositoryUrl)))
      {
        ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
        ITfsGitRepository repositoryByName;
        try
        {
          repositoryByName = service.FindRepositoryByName(requestContext, projectId.ToString(), repositoryId);
        }
        catch (GitRepositoryNotFoundException ex)
        {
          requestContext.TraceException(1013403, TraceLevel.Error, "DistributedTask", nameof (PlatformTfsGitService), (Exception) ex);
          return (string) null;
        }
        using (repositoryByName)
          return repositoryByName.GetRepositoryFullUri();
      }
    }

    public TfsGitRepository GetRepositoryInfo(
      IVssRequestContext requestContext,
      string project,
      string repository,
      ServiceEndpoint endpoint = null)
    {
      using (new MethodScope(requestContext, nameof (PlatformTfsGitService), nameof (GetRepositoryInfo)))
      {
        if (endpoint == null)
        {
          ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
          ITfsGitRepository repositoryByName;
          try
          {
            repositoryByName = service.FindRepositoryByName(requestContext, project, repository);
          }
          catch (GitRepositoryNotFoundException ex)
          {
            requestContext.TraceException(1013403, TraceLevel.Error, "DistributedTask", nameof (PlatformTfsGitService), (Exception) ex);
            return (TfsGitRepository) null;
          }
          using (repositoryByName)
            return new TfsGitRepository(repositoryByName.Key.RepoId, repositoryByName.Name, repositoryByName.Key.ProjectId, project, repositoryByName.GetRepositoryFullUri(), repositoryByName.Refs?.GetDefault()?.Name);
        }
        else
        {
          string key = "apitoken";
          if (!(endpoint.Url == (Uri) null) && endpoint.Authorization.Parameters.ContainsKey(key))
            return this.ConvertGitRepository(new VssConnection(endpoint.Url, (VssCredentials) (FederatedCredential) new VssBasicCredential(string.Empty, endpoint.Authorization.Parameters[key])).GetClient<GitHttpClient>().GetRepositoryAsync(project, repository, (object) null, new CancellationToken()).SyncResult<GitRepository>());
          requestContext.Trace(1013403, TraceLevel.Error, "DistributedTask", nameof (PlatformTfsGitService), "No git repository found with id {0} and endpoint {1}", (object) repository, (object) endpoint.Name);
          return (TfsGitRepository) null;
        }
      }
    }

    public IList<TfsGitRepository> GetRepositoriesInfo(
      IVssRequestContext requestContext,
      string project,
      IEnumerable<Guid> repositoryIds = null,
      ServiceEndpoint endpoint = null)
    {
      using (new MethodScope(requestContext, nameof (PlatformTfsGitService), nameof (GetRepositoriesInfo)))
      {
        IList<TfsGitRepository> list;
        if (endpoint == null)
        {
          list = (IList<TfsGitRepository>) requestContext.GetService<ITeamFoundationGitRepositoryService>().QueryRepositories(requestContext, new Guid(project), repositoryIds).Select<TfsGitRepositoryInfo, TfsGitRepository>((Func<TfsGitRepositoryInfo, TfsGitRepository>) (r => new TfsGitRepository(r.Key.RepoId, r.Name, r.Key.ProjectId, project, r.GetRepositoryWebUri(requestContext), r.DefaultBranch))).ToList<TfsGitRepository>();
        }
        else
        {
          string key = "apitoken";
          if (endpoint.Url == (Uri) null || !endpoint.Authorization.Parameters.ContainsKey(key))
          {
            requestContext.Trace(1013403, TraceLevel.Error, "DistributedTask", nameof (PlatformTfsGitService), "No git repository found with endpoint {0}", (object) endpoint.Name);
            return (IList<TfsGitRepository>) null;
          }
          GitHttpClient client = new VssConnection(endpoint.Url, (VssCredentials) (FederatedCredential) new VssBasicCredential(string.Empty, endpoint.Authorization.Parameters[key])).GetClient<GitHttpClient>();
          HashSet<Guid> repositoryIdHash = repositoryIds == null ? (HashSet<Guid>) null : new HashSet<Guid>(repositoryIds);
          string project1 = project;
          bool? includeLinks = new bool?();
          bool? includeAllUrls = new bool?();
          bool? includeHidden = new bool?();
          CancellationToken cancellationToken = new CancellationToken();
          list = (IList<TfsGitRepository>) client.GetRepositoriesAsync(project1, includeLinks, includeAllUrls, includeHidden, (object) null, cancellationToken).SyncResult<List<GitRepository>>().Where<GitRepository>((Func<GitRepository, bool>) (r => repositoryIdHash == null || repositoryIdHash.Contains(r.Id))).Select<GitRepository, TfsGitRepository>(new Func<GitRepository, TfsGitRepository>(this.ConvertGitRepository)).ToList<TfsGitRepository>();
        }
        return list;
      }
    }

    private TfsGitRepository ConvertGitRepository(GitRepository gitRepository) => new TfsGitRepository(gitRepository.Id, gitRepository.Name, gitRepository.ProjectReference.Id, gitRepository.ProjectReference.Name, gitRepository.Url);

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
      using (new MethodScope(requestContext, nameof (PlatformTfsGitService), nameof (HasReadAccess)))
      {
        ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
        try
        {
          using (service.FindRepositoryByName(requestContext, project, repositoryId))
            ;
        }
        catch (ProjectDoesNotExistWithNameException ex)
        {
          requestContext.TraceException(1013403, TraceLevel.Verbose, "DistributedTask", nameof (PlatformTfsGitService), (Exception) ex);
          return false;
        }
        catch (GitRepositoryNotFoundException ex)
        {
          requestContext.TraceException(1013403, TraceLevel.Verbose, "DistributedTask", nameof (PlatformTfsGitService), (Exception) ex);
          return false;
        }
        catch (GitNeedsPermissionException ex)
        {
          requestContext.TraceException(1013403, TraceLevel.Verbose, "DistributedTask", nameof (PlatformTfsGitService), (Exception) ex);
          return false;
        }
      }
      return true;
    }
  }
}
