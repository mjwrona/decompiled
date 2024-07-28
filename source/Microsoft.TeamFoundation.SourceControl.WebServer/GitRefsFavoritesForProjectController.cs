// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitRefsFavoritesForProjectController
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
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "refsFavoritesForProject")]
  public class GitRefsFavoritesForProjectController : GitApiController
  {
    [HttpGet]
    [ClientLocationId("4720896C-594C-4A6D-B94C-12EDDD80B34A")]
    [ClientResponseType(typeof (IList<GitRefFavorite>), null, null)]
    public HttpResponseMessage GetRefFavoritesForProject([ClientQueryParameter] string identityId = null)
    {
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
      return this.GetResponse(new RefFavoritesManager(this.TfsRequestContext).GetFavoritesForProject(this.ProjectInfo.Id, userId));
    }

    private HttpResponseMessage GetResponse(List<TfsGitRefFavorite> list) => this.Request.CreateResponse<List<GitRefFavorite>>(HttpStatusCode.OK, list.Select<TfsGitRefFavorite, GitRefFavorite>((Func<TfsGitRefFavorite, GitRefFavorite>) (x => this.ConvertToWebApiItem(x))).ToList<GitRefFavorite>());

    private GitRefFavorite ConvertToWebApiItem(TfsGitRefFavorite value) => value.ToWebApiItem(this.TfsRequestContext, (ISecuredObject) null, true);
  }
}
