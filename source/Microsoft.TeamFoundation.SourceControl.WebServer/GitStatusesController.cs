// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitStatusesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class GitStatusesController : GitApiController
  {
    protected const int c_defaultTop = 1000;

    [HttpGet]
    [ClientLocationId("428DD4FB-FDA5-4722-AF02-9313B80305DA")]
    [ClientResponseType(typeof (IList<GitStatus>), null, null)]
    [ClientExample("GET__git_repositories__repositoryId__commits__commitId__statuses.json", null, null, null)]
    [PublicProjectRequestRestrictions]
    public virtual HttpResponseMessage GetStatuses(
      string commitId,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      int top = 1000,
      int skip = 0,
      bool latestOnly = false)
    {
      ISecuredObject securedObject;
      List<GitStatus> statusesInternal = this.GetStatusesInternal(commitId, repositoryId, projectId, top, skip, latestOnly, out securedObject);
      if (statusesInternal == null)
        return this.Request.CreateResponse<IList<GitStatus>>(HttpStatusCode.OK, (IList<GitStatus>) Array.Empty<GitStatus>());
      GitStatusStateMapper.MapGitEntity<List<GitStatus>>(statusesInternal, this.TfsRequestContext);
      return this.Request.CreateResponse<IList<GitStatus>>(HttpStatusCode.OK, (IList<GitStatus>) statusesInternal.Select<GitStatus, GitStatus>((Func<GitStatus, GitStatus>) (x =>
      {
        x.SetSecuredObject(securedObject);
        return x;
      })).ToList<GitStatus>());
    }

    protected List<GitStatus> GetStatusesInternal(
      string commitId,
      string repositoryId,
      string projectId,
      int top,
      int skip,
      bool latestOnly,
      out ISecuredObject securedObject)
    {
      securedObject = (ISecuredObject) null;
      Sha1Id sha1Id = GitCommitUtility.ParseSha1Id(commitId);
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ILookup<Sha1Id, GitStatus> toReturn = this.TfsRequestContext.GetService<ITeamFoundationGitCommitStatusService>().GetStatuses(this.TfsRequestContext, tfsGitRepository, (IEnumerable<Sha1Id>) new Sha1Id[1]
        {
          sha1Id
        }, top, skip, (latestOnly ? 1 : 0) != 0);
        if (toReturn == null)
          return (List<GitStatus>) null;
        securedObject = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        return toReturn.SelectMany<IGrouping<Sha1Id, GitStatus>, GitStatus>((Func<IGrouping<Sha1Id, GitStatus>, IEnumerable<GitStatus>>) (x => toReturn[x.Key])).ToList<GitStatus>();
      }
    }

    [HttpPost]
    [ClientLocationId("428DD4FB-FDA5-4722-AF02-9313B80305DA")]
    [ClientResponseType(typeof (GitStatus), null, null)]
    [ClientExample("POST__git_repositories__repositoryId__commits__commitId__statuses.json", null, null, null)]
    public virtual HttpResponseMessage CreateCommitStatus(
      string commitId,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      GitStatus gitCommitStatusToCreate,
      [ClientIgnore] string projectId = null)
    {
      if (this.TfsRequestContext.IsFeatureEnabled("SourceControl.EnableReturningPartiallySucceededGitStatusState") && gitCommitStatusToCreate != null && gitCommitStatusToCreate.State == GitStatusState.PartiallySucceeded)
        throw new InvalidArgumentValueException(Resources.Get("InvalidGitStatusStateValue"), "State");
      Sha1Id sha1Id = GitCommitUtility.ParseSha1Id(commitId);
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
        return this.Request.CreateResponse<GitStatus>(HttpStatusCode.Created, this.TfsRequestContext.GetService<ITeamFoundationGitCommitStatusService>().AddStatus(this.TfsRequestContext, tfsGitRepository, sha1Id, gitCommitStatusToCreate));
    }
  }
}
