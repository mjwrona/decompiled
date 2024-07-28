// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestStatusesBaseController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer.ObjectModel;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public abstract class GitPullRequestStatusesBaseController : GitApiController
  {
    protected HttpResponseMessage CreatePullRequestStatusInternal(
      string repositoryId,
      int pullRequestId,
      GitPullRequestStatus status,
      int? iterationId = null,
      string projectId = null)
    {
      if (status == null)
        return this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, Resources.Get("PullRequestStatusMalformed"));
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        TfsGitPullRequest pullRequestDetails = service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequestDetails == null)
          throw new GitPullRequestNotFoundException();
        if (this.TfsRequestContext.IsFeatureEnabled("SourceControl.EnableReturningPartiallySucceededGitStatusState") && status.State == GitStatusState.PartiallySucceeded)
          throw new InvalidArgumentValueException(Resources.Get("InvalidGitStatusStateValue"), "State");
        status.Id = 0;
        GitPullRequestStatus status1 = service.SaveStatus(this.TfsRequestContext, tfsGitRepository, pullRequestDetails, status, iterationId);
        if (status1 != null)
          status1.AddReferenceLinks(this.TfsRequestContext, pullRequestDetails, iterationId);
        return this.Request.CreateResponse<GitPullRequestStatus>(HttpStatusCode.OK, status1);
      }
    }

    protected void UpdatePullRequestStatusesInternal(
      string repositoryId,
      int pullRequestId,
      PatchDocument<IDictionary<string, object>> patchDocument,
      int? iterationId = null,
      string projectId = null)
    {
      ArgumentUtility.CheckForNull<PatchDocument<IDictionary<string, object>>>(patchDocument, nameof (patchDocument));
      IEnumerable<IPatchOperation<IDictionary<string, object>>> operations = patchDocument.Operations;
      IPatchOperation<IDictionary<string, object>> patchOperation = operations != null ? operations.FirstOrDefault<IPatchOperation<IDictionary<string, object>>>((Func<IPatchOperation<IDictionary<string, object>>, bool>) (op => op.Operation != Operation.Remove)) : (IPatchOperation<IDictionary<string, object>>) null;
      if (patchOperation != null)
        throw new PropertiesCollectionPatchException(FrameworkResources.JsonPatchOperationNotSupported((object) patchOperation.Operation.ToString()));
      PropertiesCollection propertiesCollection = PropertiesCollectionPatchHelper.ReadPatchDocument((IPatchDocument<IDictionary<string, object>>) patchDocument);
      List<int> source = new List<int>();
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) propertiesCollection)
      {
        int result;
        if (!int.TryParse(keyValuePair.Key, out result))
          throw new PropertiesCollectionPatchException(Resources.Get("PullRequestStatusesPatchRemoveArbitraryPath"));
        source.Add(result);
      }
      List<int> list = source.Distinct<int>().ToList<int>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        service.DeleteStatuses(this.TfsRequestContext, tfsGitRepository, service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId) ?? throw new GitPullRequestNotFoundException(), (IEnumerable<int>) list, iterationId);
      }
    }

    protected void DeletePullRequestStatusInternal(
      string repositoryId,
      int pullRequestId,
      int statusId,
      int? iterationId = null,
      string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        service.DeleteStatuses(this.TfsRequestContext, tfsGitRepository, service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId) ?? throw new GitPullRequestNotFoundException(), (IEnumerable<int>) new int[1]
        {
          statusId
        }, iterationId);
      }
    }

    protected void AddStatusReferenceLinks(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      IEnumerable<GitPullRequestStatus> statuses,
      int? iterationId = null)
    {
      if (statuses == null || !statuses.Any<GitPullRequestStatus>())
        return;
      foreach (GitPullRequestStatus statuse in statuses)
        statuse.AddReferenceLinks(requestContext, pullRequest, iterationId);
    }

    protected IEnumerable<GitPullRequestStatus> GetPullRequestIterationStatusesInternal(
      string repositoryId,
      int pullRequestId,
      int iterationId,
      string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        TfsGitPullRequest pullRequestDetails = service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequestDetails == null)
          throw new GitPullRequestNotFoundException();
        IEnumerable<GitPullRequestStatus> list = (IEnumerable<GitPullRequestStatus>) service.GetStatuses(this.TfsRequestContext, tfsGitRepository, pullRequestDetails, new int?(iterationId), true).ToList<GitPullRequestStatus>();
        this.AddStatusReferenceLinks(this.TfsRequestContext, pullRequestDetails, list, new int?(iterationId));
        return list;
      }
    }

    protected GitPullRequestStatus GetPullRequestIterationStatusInternal(
      string repositoryId,
      int pullRequestId,
      int iterationId,
      int statusId,
      string projectId)
    {
      ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        TfsGitPullRequest pullRequestDetails = service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequestDetails == null)
          throw new GitPullRequestNotFoundException();
        GitPullRequestStatus status = service.GetStatus(this.TfsRequestContext, tfsGitRepository, pullRequestDetails, statusId, new int?(iterationId));
        if (status != null)
          status.AddReferenceLinks(this.TfsRequestContext, pullRequestDetails, new int?(iterationId));
        return status;
      }
    }

    protected IEnumerable<GitPullRequestStatus> GetPullRequestStatusesInternal(
      string repositoryId,
      int pullRequestId,
      string projectId)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        TfsGitPullRequest pullRequestDetails = service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequestDetails == null)
          throw new GitPullRequestNotFoundException();
        IEnumerable<GitPullRequestStatus> statuses = service.GetStatuses(this.TfsRequestContext, tfsGitRepository, pullRequestDetails, includeProperties: true);
        this.AddStatusReferenceLinks(this.TfsRequestContext, pullRequestDetails, statuses);
        return statuses;
      }
    }

    protected GitPullRequestStatus GetPullRequestStatusInternal(
      string repositoryId,
      int pullRequestId,
      int statusId,
      string projectId)
    {
      ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        TfsGitPullRequest pullRequestDetails = service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequestDetails == null)
          throw new GitPullRequestNotFoundException();
        GitPullRequestStatus status = service.GetStatus(this.TfsRequestContext, tfsGitRepository, pullRequestDetails, statusId);
        if (status != null)
          status.AddReferenceLinks(this.TfsRequestContext, pullRequestDetails);
        return status;
      }
    }
  }
}
