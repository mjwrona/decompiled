// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.Controllers.Git.GitCommitsBatch2Controller
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer.Controllers.Git
{
  [ValidateModel]
  [ClientGroupByResource("Commits")]
  [ControllerApiVersion(7.2)]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "CommitsBatch", ResourceVersion = 2)]
  public class GitCommitsBatch2Controller : GitCommitsBatchController
  {
    [HttpPost]
    [ClientResponseType(typeof (IList<GitCommitRef>), null, null)]
    [ClientLocationId("6400DFB2-0BCB-462B-B992-5A57F8F1416C")]
    [ClientExample("POST__git_repositories__repositoryId__commitsBatch__ids.json", "Commits by a list of commit IDs", "Send a POST request with a list of IDs you want.", null)]
    [ClientExample("POST__git_repositories__repositoryId__commitsBatch2.json", "Commits between two versions", "To return the set of commits in the history of one version that are not in the history of another, send a POST request and send the items versions in the request body.", null)]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetCommitsBatch(
      GitQueryCommitsCriteria searchCriteria,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$top")] int? top = null,
      bool includeStatuses = false)
    {
      if (skip.HasValue || top.HasValue)
      {
        IVssRegistryService service = this.TfsRequestContext.GetService<IVssRegistryService>();
        this.ValidateInput(service, "$skip", skip, GitCommitsBatchController.s_maxSkipQuery, int.MaxValue);
        this.ValidateInput(service, "$top", top, GitCommitsBatchController.s_maxTopQuery, int.MaxValue);
      }
      return this.QueryCommits2(searchCriteria, repositoryId, projectId, skip, top, includeStatuses);
    }
  }
}
