// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPushes3Controller
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(7.2)]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "pushes", ResourceVersion = 3)]
  public class GitPushes3Controller : GitPushes2Controller
  {
    [HttpPost]
    [ClientExample("POST__git_repositories__tempRepoId__pushes.json", "Initial commit (Create a new branch)", null, null)]
    [ClientExample("POST__git_repositories__tempRepoId__pushes2.json", "Add a text file", null, null)]
    [ClientExample("POST__git_repositories__tempRepoId__pushes3.json", "Add a binary file", null, null)]
    [ClientExample("POST__git_repositories__tempRepoId__pushes4.json", "Update a file", null, null)]
    [ClientExample("POST__git_repositories__tempRepoId__pushes5.json", "Delete a file", null, null)]
    [ClientExample("POST__git_repositories__tempRepoId__pushes6.json", "Rename a file", null, null)]
    [ClientExample("POST__git_repositories__tempRepoId__pushes7.json", "Move a file", null, null)]
    [ClientExample("POST__git_repositories__tempRepoId__pushes8.json", "Update a file in a new branch", null, null)]
    [ClientExample("POST__git_repositories__tempRepoId__pushes9.json", "Multiple changes", null, null)]
    [ClientResponseType(typeof (GitPush), null, null)]
    [ClientLocationId("EA98D07B-3C87-4971-8EDE-A613694FFB55")]
    [ClientRequestBodyType(typeof (GitPush), "push")]
    public override HttpResponseMessage CreatePush([ClientParameterType(typeof (Guid), true)] string repositoryId) => this.Request.CreateResponse<GitPush>(HttpStatusCode.Created, this.CreatePushInternal(repositoryId));

    [HttpGet]
    [ClientExample("GET__git_repositories__repositoryId__pushes__pushId_.json", "Just the push", null, null)]
    [ClientResponseType(typeof (GitPush), null, null)]
    [ClientLocationId("EA98D07B-3C87-4971-8EDE-A613694FFB55")]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetPush(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pushId,
      [ClientIgnore] string projectId = null,
      int includeCommits = 100,
      bool includeRefUpdates = false)
    {
      GitPush gitPush = (GitPush) null;
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
        gitPush = GitPushUtility.GetPush(this.TfsRequestContext, tfsGitRepository, this.Url, pushId, includeCommits, includeRefUpdates);
      return gitPush == null ? this.Request.CreateResponse<GitPush>(HttpStatusCode.NotFound, gitPush) : this.Request.CreateResponse<GitPush>(HttpStatusCode.OK, gitPush);
    }

    [HttpGet]
    [ClientExample("GET__git_repositories__repositoryId__pushes.json", "By repository ID", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__pushes_fromDate-_fromDate__toDate-_toDate_.json", "In a date range", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__pushes_pusherId-_pusherId_.json", "By who submitted the pushes", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__pushes__skip-_skip___top-_top_.json", "A page at a time", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__pushes_refName-_refName__includeRefUpdates-true.json", "For a particular branch, including ref updates", null, null)]
    [ClientLocationId("EA98D07B-3C87-4971-8EDE-A613694FFB55")]
    [PublicProjectRequestRestrictions]
    public override IList<GitPush> GetPushes(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$top")] int? top = null,
      [ModelBinder] GitPushSearchCriteria searchCriteria = null)
    {
      return this.GetPushesInternal(repositoryId, projectId, skip, top, searchCriteria);
    }
  }
}
