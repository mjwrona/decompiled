// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestWorkItemsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GitPullRequestWorkItemsController : GitApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (IList<ResourceRef>), null, null)]
    [ClientLocationId("0A637FCC-5370-4CE8-B0E8-98091F5F9482")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetPullRequestWorkItemRefs(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientParameterType(typeof (int), false)] string pullRequestId,
      [ClientIgnore] string projectId = null)
    {
      int result;
      if (string.IsNullOrEmpty(pullRequestId) || !int.TryParse(pullRequestId, out result))
        throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestId"));
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        IEnumerable<int> associatedWorkItems = (this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, result) ?? throw new GitPullRequestNotFoundException()).GetAssociatedWorkItems(this.TfsRequestContext, tfsGitRepository);
        return this.Request.CreateResponse<IEnumerable<ResourceRef>>(HttpStatusCode.OK, GitPullRequestModelExtensions.ToWorkItemResourceRefs(this.TfsRequestContext, tfsGitRepository, associatedWorkItems));
      }
    }
  }
}
