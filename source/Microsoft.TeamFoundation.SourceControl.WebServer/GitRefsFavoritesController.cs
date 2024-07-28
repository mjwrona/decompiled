// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitRefsFavoritesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "refsFavorites")]
  public class GitRefsFavoritesController : GitApiController
  {
    [HttpGet]
    [ClientLocationId("876F70AF-5792-485A-A1C7-D0A7B2F42BBB")]
    [ClientResponseType(typeof (IList<GitRefFavorite>), null, null)]
    public HttpResponseMessage GetRefFavorites([ClientQueryParameter] string repositoryId = null, [ClientQueryParameter] string identityId = null)
    {
      if (repositoryId == null)
        throw new InvalidArgumentValueException(nameof (repositoryId), Resources.Get("GitRefFavoritesRepositoryIsNull"));
      Guid userId;
      if (string.IsNullOrEmpty(identityId))
      {
        userId = this.TfsRequestContext.GetUserId();
      }
      else
      {
        try
        {
          userId = Guid.Parse(identityId);
        }
        catch (FormatException ex)
        {
          throw new InvalidArgumentValueException(nameof (identityId), (Exception) ex);
        }
      }
      this.TfsRequestContext.GetService<ITeamFoundationGitRepositoryService>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId))
      {
        if (tfsGitRepository.Key.ProjectId != this.ProjectInfo.Id)
          throw new InvalidArgumentValueException(nameof (repositoryId), Resources.Get("GitRefFavoriteProjectMismatch"));
        return this.GetResponse(new RefFavoritesManager(this.TfsRequestContext).GetFavorites(userId, tfsGitRepository));
      }
    }

    [HttpGet]
    [ClientLocationId("876F70AF-5792-485A-A1C7-D0A7B2F42BBB")]
    [ClientResponseType(typeof (GitRefFavorite), null, null)]
    public HttpResponseMessage GetRefFavorite(int favoriteId)
    {
      TfsGitRefFavorite tfsGitRefFavorite = new RefFavoritesManager(this.TfsRequestContext).GetFavorite(this.ProjectId, favoriteId);
      if (tfsGitRefFavorite != null && tfsGitRefFavorite.RepoKey.ProjectId != this.ProjectInfo.Id)
        tfsGitRefFavorite = (TfsGitRefFavorite) null;
      return tfsGitRefFavorite != null ? this.GetResponse(this.ConvertToWebApiItem(tfsGitRefFavorite)) : throw new GitRefFavoriteNotFoundException(favoriteId);
    }

    [HttpDelete]
    [ClientLocationId("876F70AF-5792-485A-A1C7-D0A7B2F42BBB")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteRefFavorite(int favoriteId)
    {
      RefFavoritesManager favoritesManager = new RefFavoritesManager(this.TfsRequestContext);
      TfsGitRefFavorite favorite = favoritesManager.GetFavorite(this.ProjectId, favoriteId);
      if (favorite == null || favorite.RepoKey.ProjectId != this.ProjectInfo.Id)
        throw new GitRefFavoriteNotFoundException(favoriteId);
      favoritesManager.DeleteFavorite(this.ProjectId, favoriteId);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpPost]
    [ClientLocationId("876F70AF-5792-485A-A1C7-D0A7B2F42BBB")]
    [ClientResponseType(typeof (GitRefFavorite), null, null)]
    public HttpResponseMessage CreateFavorite(GitRefFavorite favorite)
    {
      if (favorite == null)
        throw new InvalidArgumentValueException(nameof (favorite), Resources.Get("GitRefFavoriteIsNull"));
      if (favorite.Type == GitRefFavorite.RefFavoriteType.Invalid)
        throw new InvalidArgumentValueException("Type", Resources.Format("GitRefFavoriteInvalidType", (object) this.GetValidTypeNames()));
      if (favorite.Name == null || favorite.Name.Length > GitConstants.MaxGitRefNameLength || !favorite.Name.StartsWith("refs/", StringComparison.Ordinal))
        throw new InvalidArgumentValueException("Name", Resources.Format("GitInvalidRefName", (object) favorite.Name, (object) "refs/", (object) GitConstants.MaxGitRefNameLength));
      if (favorite.IdentityId == Guid.Empty)
        favorite.IdentityId = this.TfsRequestContext.GetUserId();
      RefFavoritesManager favoritesManager = new RefFavoritesManager(this.TfsRequestContext);
      using (ITfsGitRepository repositoryById = this.TfsRequestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(this.TfsRequestContext, favorite.RepositoryId))
      {
        if (repositoryById.Key.ProjectId != this.ProjectInfo.Id)
          throw new InvalidArgumentValueException(nameof (favorite), Resources.Get("GitRefFavoriteProjectMismatch"));
        return this.GetResponse(this.ConvertToWebApiItem(favoritesManager.CreateFavorite(favorite.IdentityId, repositoryById, favorite.Name, favorite.Type == GitRefFavorite.RefFavoriteType.Folder)));
      }
    }

    private string GetValidTypeNames() => string.Join(", ", ((IEnumerable<string>) System.Enum.GetNames(typeof (GitRefFavorite.RefFavoriteType))).Where<string>((Func<string, bool>) (x => !x.Equals("Invalid"))));

    private HttpResponseMessage GetResponse(List<TfsGitRefFavorite> list) => this.Request.CreateResponse<List<GitRefFavorite>>(HttpStatusCode.OK, list.Select<TfsGitRefFavorite, GitRefFavorite>((Func<TfsGitRefFavorite, GitRefFavorite>) (x => this.ConvertToWebApiItem(x))).ToList<GitRefFavorite>());

    private HttpResponseMessage GetResponse(GitRefFavorite value) => this.Request.CreateResponse<GitRefFavorite>(HttpStatusCode.OK, value);

    private GitRefFavorite ConvertToWebApiItem(TfsGitRefFavorite value) => value.ToWebApiItem(this.TfsRequestContext, (ISecuredObject) null, true);
  }
}
