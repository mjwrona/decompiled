// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitRecycleBinRepositoriesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "RecycleBinRepositories")]
  [ClientGroupByResource("Repositories")]
  public class GitRecycleBinRepositoriesController : GitApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<GitDeletedRepository>), null, null)]
    public HttpResponseMessage GetRecycleBinRepositories()
    {
      IList<TfsGitDeletedRepositoryInfo> source = this.TfsRequestContext.GetService<ITeamFoundationGitRepositoryService>().QueryDeletedRepositories(this.TfsRequestContext, new Guid?(this.ProjectId), true);
      Dictionary<Guid, TeamFoundationIdentity> guidToIdentity = new Dictionary<Guid, TeamFoundationIdentity>();
      IdentityLookupHelper.LoadIdentities(this.TfsRequestContext, source.Select<TfsGitDeletedRepositoryInfo, Guid>((Func<TfsGitDeletedRepositoryInfo, Guid>) (r => r.DeletedBy)), (IDictionary<Guid, TeamFoundationIdentity>) guidToIdentity);
      IList<GitDeletedRepository> collection = (IList<GitDeletedRepository>) new List<GitDeletedRepository>();
      foreach (TfsGitDeletedRepositoryInfo repo in (IEnumerable<TfsGitDeletedRepositoryInfo>) source)
        collection.Add(repo.ToWebApiItem(this.TfsRequestContext, (IDictionary<Guid, TeamFoundationIdentity>) guidToIdentity, this.ProjectInfo));
      return this.GenerateResponse<GitDeletedRepository>((IEnumerable<GitDeletedRepository>) collection);
    }

    [HttpPatch]
    [ClientResponseType(typeof (GitRepository), null, null)]
    public HttpResponseMessage RestoreRepositoryFromRecycleBin(
      Guid repositoryId,
      GitRecycleBinRepositoryDetails repositoryDetails)
    {
      ArgumentUtility.CheckForNull<GitRecycleBinRepositoryDetails>(repositoryDetails, nameof (repositoryDetails));
      if (!repositoryDetails.Deleted.HasValue)
        return this.Request.CreateResponse(HttpStatusCode.Accepted);
      if (repositoryDetails.Deleted.Value)
        throw new NotSupportedException(Resources.Get("ErrorDeletionNotSupportedOnPatch"));
      ITeamFoundationGitRepositoryService service = this.TfsRequestContext.GetService<ITeamFoundationGitRepositoryService>();
      TfsGitDeletedRepositoryInfo softDeletedRepoInfo = this.GetSoftDeletedRepoInfo(service, repositoryId);
      using (ITfsGitRepository repo = service.UndeleteRepository(this.TfsRequestContext, softDeletedRepoInfo.Key))
        return this.Request.CreateResponse<GitRepository>(HttpStatusCode.Created, repo.ToWebApiItem(this.TfsRequestContext, this.ProjectInfo, true));
    }

    [HttpDelete]
    public void DeleteRepositoryFromRecycleBin(Guid repositoryId)
    {
      ITeamFoundationGitRepositoryService service = this.TfsRequestContext.GetService<ITeamFoundationGitRepositoryService>();
      TfsGitDeletedRepositoryInfo softDeletedRepoInfo = this.GetSoftDeletedRepoInfo(service, repositoryId);
      service.DestroyRepositories(this.TfsRequestContext, (RepoScope) softDeletedRepoInfo.Key);
    }

    private TfsGitDeletedRepositoryInfo GetSoftDeletedRepoInfo(
      ITeamFoundationGitRepositoryService repoService,
      Guid repositoryId)
    {
      if (repositoryId == Guid.Empty)
        throw new ArgumentException("ErrorRespositoryIdInvalid").Expected("git");
      return repoService.QueryDeletedRepositories(this.TfsRequestContext, new Guid?(this.ProjectId), true).FirstOrDefault<TfsGitDeletedRepositoryInfo>((Func<TfsGitDeletedRepositoryInfo, bool>) (r => r.Key.RepoId == repositoryId)) ?? throw new GitRepositoryNotFoundException(repositoryId);
    }
  }
}
