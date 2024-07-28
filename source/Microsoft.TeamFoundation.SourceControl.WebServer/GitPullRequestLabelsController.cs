// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestLabelsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [FeatureEnabled("WebAccess.VersionControl.PullRequests.Labels")]
  public class GitPullRequestLabelsController : GitPullRequestsBaseController
  {
    [HttpGet]
    [ClientLocationId("F22387E3-984E-4C52-9C6D-FBB8F14C812D")]
    [ClientResponseType(typeof (IEnumerable<WebApiTagDefinition>), null, null)]
    public HttpResponseMessage GetPullRequestLabels(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        TfsGitPullRequest pullRequest = this.ValidatePullRequest(this.TfsRequestContext, pullRequestId, tfsGitRepository);
        return this.Request.CreateResponse<IEnumerable<WebApiTagDefinition>>(HttpStatusCode.OK, this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().GetPullRequestLabels(this.TfsRequestContext, tfsGitRepository, pullRequest).Select<TagDefinition, WebApiTagDefinition>((Func<TagDefinition, WebApiTagDefinition>) (label => this.AddResourceUrl(pullRequest, label.ToApiTagDefinition()))));
      }
    }

    [HttpGet]
    [ClientLocationId("F22387E3-984E-4C52-9C6D-FBB8F14C812D")]
    [ClientResponseType(typeof (WebApiTagDefinition), null, null)]
    public HttpResponseMessage GetPullRequestLabel(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      string labelIdOrName,
      string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        TfsGitPullRequest pullRequest = this.ValidatePullRequest(this.TfsRequestContext, pullRequestId, tfsGitRepository);
        TagDefinition pullRequestLabel = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().GetPullRequestLabel(this.TfsRequestContext, tfsGitRepository, pullRequest, labelIdOrName);
        return this.Request.CreateResponse<WebApiTagDefinition>(HttpStatusCode.OK, this.AddResourceUrl(pullRequest, pullRequestLabel.ToApiTagDefinition()));
      }
    }

    [HttpPost]
    [ClientLocationId("F22387E3-984E-4C52-9C6D-FBB8F14C812D")]
    [ClientResponseType(typeof (WebApiTagDefinition), null, null)]
    public HttpResponseMessage CreatePullRequestLabel(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      WebApiCreateTagRequestData label,
      string projectId = null)
    {
      ClientTraceData clientTraceData = new ClientTraceData();
      if (label == null || string.IsNullOrEmpty(label.Name))
        throw new InvalidArgumentValueException(Resources.Get("MissingOrEmptyLabelName"));
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        TfsGitPullRequest pullRequest1 = this.ValidatePullRequest(this.TfsRequestContext, pullRequestId, tfsGitRepository);
        TagDefinition pullRequest2 = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().AddLabelToPullRequest(this.TfsRequestContext, label.Name, tfsGitRepository, pullRequest1, clientTraceData);
        WebApiTagDefinition apiTagDefinition = this.AddResourceUrl(pullRequest1, pullRequest2.ToApiTagDefinition());
        try
        {
          this.TfsRequestContext.GetService<ClientTraceService>().Publish(this.TfsRequestContext, "VersionControl", "Labelling", clientTraceData);
        }
        catch
        {
        }
        return this.Request.CreateResponse<WebApiTagDefinition>(HttpStatusCode.OK, apiTagDefinition);
      }
    }

    [HttpDelete]
    [ClientLocationId("F22387E3-984E-4C52-9C6D-FBB8F14C812D")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeletePullRequestLabels(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      string labelIdOrName,
      string projectId = null)
    {
      ClientTraceData clientTraceData = new ClientTraceData();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        TfsGitPullRequest pullRequest = this.ValidatePullRequest(this.TfsRequestContext, pullRequestId, tfsGitRepository);
        this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().RemoveLabelFromPullRequest(this.TfsRequestContext, tfsGitRepository, pullRequest, labelIdOrName, clientTraceData);
        try
        {
          this.TfsRequestContext.GetService<ClientTraceService>().Publish(this.TfsRequestContext, "VersionControl", "Labelling", clientTraceData);
        }
        catch
        {
        }
        return this.Request.CreateResponse(HttpStatusCode.OK);
      }
    }

    private TfsGitPullRequest ValidatePullRequest(
      IVssRequestContext TfsRequestContext,
      int pullRequestId,
      ITfsGitRepository repository)
    {
      return TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().GetPullRequestDetails(TfsRequestContext, repository, pullRequestId) ?? throw new GitPullRequestNotFoundException();
    }

    private WebApiTagDefinition AddResourceUrl(
      TfsGitPullRequest pullRequest,
      WebApiTagDefinition apiLabel)
    {
      if (this.TfsRequestContext != null && apiLabel != null)
      {
        ILocationService service = this.TfsRequestContext.GetService<ILocationService>();
        apiLabel.Url = service.GetResourceUri(this.TfsRequestContext, "git", GitWebApiConstants.PullRequestLabelsId, (object) new
        {
          repositoryId = pullRequest.RepositoryId,
          pullRequestId = pullRequest.PullRequestId,
          labelIdOrName = apiLabel.Id
        }).AbsoluteUri;
      }
      return apiLabel;
    }
  }
}
