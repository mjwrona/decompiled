// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitLimitedRefCriteriaController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi.Internal;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(2.2)]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "limitedRefCriteria", ResourceVersion = 1)]
  public class GitLimitedRefCriteriaController : GitApiController
  {
    [HttpGet]
    [ClientLocationId("F1D5D07A-6B89-4384-BEF6-446461E31A39")]
    [ClientIgnore]
    public GitLimitedRefCriteria GetLimitedRefCriteria([ClientParameterType(typeof (Guid), true)] string repositoryId, [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository repositoryByFilters = GitServerUtils.FindRepositoryByFilters(this.TfsRequestContext, repositoryId, this.GetProjectFilter(projectId)))
        return this.ConvertToWebApiItem(new LimitedRefManager(this.TfsRequestContext, repositoryByFilters).GetLimitedRefCriteria(), repositoryByFilters.Key);
    }

    [HttpPut]
    [ClientLocationId("F1D5D07A-6B89-4384-BEF6-446461E31A39")]
    [ClientResponseType(typeof (GitLimitedRefCriteria), null, null)]
    [ClientIgnore]
    public HttpResponseMessage UpdateLimitedRefCriteria(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      GitLimitedRefCriteria criteria,
      [ClientIgnore] string projectId = null)
    {
      if (criteria == null)
        throw new InvalidArgumentValueException(nameof (criteria), "LimitedRefCriteriaIsNull");
      using (ITfsGitRepository repositoryByFilters = GitServerUtils.FindRepositoryByFilters(this.TfsRequestContext, repositoryId, this.GetProjectFilter(projectId)))
      {
        LimitedRefManager limitedRefManager = new LimitedRefManager(this.TfsRequestContext, repositoryByFilters);
        try
        {
          return this.Request.CreateResponse<GitLimitedRefCriteria>(HttpStatusCode.OK, this.ConvertToWebApiItem(limitedRefManager.UpdateLimitedRefCriteria(criteria.RefExactMatches, criteria.RefNamespaces), repositoryByFilters.Key));
        }
        catch (GitInvalidLimitedRefCriteriaException ex)
        {
          throw new InvalidArgumentValueException(nameof (criteria), (Exception) ex);
        }
      }
    }

    private GitLimitedRefCriteria ConvertToWebApiItem(
      TfsGitLimitedRefCriteria value,
      RepoKey repoKey)
    {
      string absoluteUri = this.TfsRequestContext.GetService<ILocationService>().GetResourceUri(this.TfsRequestContext, "git", GitWebApiConstants.LimitedRefCriteriaLocationId, (object) new
      {
        repositoryId = repoKey.RepoId
      }).AbsoluteUri;
      ReferenceLinks baseReferenceLinks = GitReferenceLinksUtility.GetBaseReferenceLinks(this.TfsRequestContext, absoluteUri, repoKey);
      return new GitLimitedRefCriteria()
      {
        RefExactMatches = value.RefExactMatches,
        RefNamespaces = value.RefNamespaces,
        Url = absoluteUri,
        Links = baseReferenceLinks
      };
    }
  }
}
