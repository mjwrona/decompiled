// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPushesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(1.0)]
  public class GitPushesController : GitApiController
  {
    protected const int c_defaultTop = 100;
    protected const string TraceLayer = "GitPushesController";

    [HttpGet]
    [ClientExample("GET__git_repositories__repositoryId__pushes.json", "By repository ID", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__pushes_fromDate-_fromDate__toDate-_toDate_.json", "In a date range", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__pushes_pusherId-_pusherId_.json", "By who submitted the pushes", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__pushes__skip-_skip___top-_top_.json", "A page at a time", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__pushes_refName-_refName__includeRefUpdates-true.json", "For a particular branch, including ref updates", null, null)]
    [ClientLocationId("EA98D07B-3C87-4971-8EDE-A613694FFB55")]
    [PublicProjectRequestRestrictions]
    public virtual IList<GitPush> GetPushes(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$top")] int? top = null,
      [ModelBinder] GitPushSearchCriteria searchCriteria = null)
    {
      IList<GitPush> pushesInternal = this.GetPushesInternal(repositoryId, projectId, skip, top, searchCriteria);
      GitStatusStateMapper.MapGitEntity<IList<GitPush>>(pushesInternal, this.TfsRequestContext);
      return pushesInternal;
    }

    protected IList<GitPush> GetPushesInternal(
      string repositoryId,
      string projectId,
      int? skip,
      int? top,
      GitPushSearchCriteria searchCriteria)
    {
      if (searchCriteria == null)
        searchCriteria = new GitPushSearchCriteria();
      skip = new int?(Math.Max(0, skip.GetValueOrDefault()));
      top = new int?(Math.Max(0, top ?? 100));
      IList<GitPush> pushesInternal = (IList<GitPush>) new List<GitPush>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        List<TfsGitPushMetadata> source = tfsGitRepository.QueryPushHistory(searchCriteria.IncludeRefUpdates, searchCriteria.FromDate, searchCriteria.ToDate, searchCriteria.PusherId, skip, top, searchCriteria.RefName);
        if (source.Count > 0)
        {
          GitRepository webApiItem = tfsGitRepository.ToWebApiItem(this.TfsRequestContext);
          Dictionary<Guid, IdentityRef> dictionary = new Dictionary<Guid, IdentityRef>();
          ITeamFoundationIdentityService service = this.TfsRequestContext.GetService<ITeamFoundationIdentityService>();
          Guid[] array = source.Select<TfsGitPushMetadata, Guid>((Func<TfsGitPushMetadata, Guid>) (pm => pm.PusherId)).Distinct<Guid>().ToArray<Guid>();
          HashSet<Guid> guidSet = new HashSet<Guid>();
          IVssRequestContext requestContext = this.TfsRequestContext.Elevate();
          Guid[] teamFoundationIds = array;
          foreach (TeamFoundationIdentity readIdentity in service.ReadIdentities(requestContext, teamFoundationIds))
          {
            if (readIdentity != null)
            {
              if (dictionary.ContainsKey(readIdentity.TeamFoundationId))
              {
                guidSet.Add(readIdentity.TeamFoundationId);
              }
              else
              {
                if (readIdentity.TeamFoundationId == Guid.Empty)
                  this.TfsRequestContext.Trace(1013664, TraceLevel.Info, this.TraceArea, nameof (GitPushesController), "Empty VSID on identity with unique name: {0}", (object) readIdentity.UniqueName);
                IdentityRef identityRef = readIdentity.ToIdentityRef(this.TfsRequestContext);
                dictionary.Add(readIdentity.TeamFoundationId, identityRef);
              }
            }
          }
          if (guidSet.Any<Guid>())
            this.TfsRequestContext.TraceAlways(1013663, TraceLevel.Error, this.TraceArea, nameof (GitPushesController), "Inconsistent PusherIds found. This is an indication of MasterIds leaked by SPS - [{0}]", (object) string.Join<Guid>(", ", (IEnumerable<Guid>) guidSet));
          ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
          foreach (TfsGitPushMetadata tfsGitPushMetadata in source)
          {
            GitPush gitPush1 = new GitPush();
            gitPush1.Commits = (IEnumerable<GitCommitRef>) null;
            gitPush1.Date = tfsGitPushMetadata.PushTime;
            gitPush1.PushId = tfsGitPushMetadata.PushId;
            gitPush1.Repository = webApiItem;
            GitPush gitPush2 = gitPush1;
            if (searchCriteria.IncludeRefUpdates && tfsGitPushMetadata.RefLog != null)
              gitPush2.RefUpdates = (IEnumerable<GitRefUpdate>) tfsGitPushMetadata.RefLog.Select<TfsGitRefLogEntry, GitRefUpdate>((Func<TfsGitRefLogEntry, GitRefUpdate>) (logEntry => logEntry.ToRefUpdate())).ToList<GitRefUpdate>();
            Guid pusherId = tfsGitPushMetadata.PusherId;
            IdentityRef identityRef = (IdentityRef) null;
            if (dictionary.TryGetValue(pusherId, out identityRef))
            {
              gitPush2.PushedBy = identityRef;
            }
            else
            {
              this.TfsRequestContext.Trace(1013804, TraceLevel.Info, this.TraceArea, nameof (GitPushesController), "Unknown pusher identity. Push Id: {0}, Pusher: {1}", (object) gitPush2.PushId, (object) pusherId);
              gitPush2.PushedBy = Microsoft.TeamFoundation.Framework.Server.IdentityHelper.CreateUnboundIdentityRef(this.TfsRequestContext, pusherId);
            }
            gitPush2.SetSecuredObject(repositoryReadOnly);
            gitPush2.Url = this.Url.RestLink(this.TfsRequestContext, GitWebApiConstants.PushesLocationId, (object) new
            {
              repositoryId = tfsGitRepository.Key.RepoId,
              PushId = gitPush2.PushId
            });
            gitPush2.Links = searchCriteria.IncludeLinks ? gitPush2.GetPushReferenceLinks(this.TfsRequestContext, this.Url, tfsGitRepository.Key, repositoryReadOnly) : (ReferenceLinks) null;
            pushesInternal.Add(gitPush2);
          }
        }
      }
      return pushesInternal;
    }

    [HttpGet]
    [ClientExample("GET__git_repositories__repositoryId__pushes__pushId_.json", "Just the push", null, null)]
    [ClientResponseType(typeof (GitPush), null, null)]
    [ClientLocationId("EA98D07B-3C87-4971-8EDE-A613694FFB55")]
    [PublicProjectRequestRestrictions]
    public virtual HttpResponseMessage GetPush(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pushId,
      [ClientIgnore] string projectId = null,
      int includeCommits = 100,
      bool includeRefUpdates = false)
    {
      GitPush entity = (GitPush) null;
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
        entity = GitPushUtility.GetPush(this.TfsRequestContext, tfsGitRepository, this.Url, pushId, includeCommits, includeRefUpdates);
      if (entity == null)
        return this.Request.CreateResponse<GitPush>(HttpStatusCode.NotFound, entity);
      GitStatusStateMapper.MapGitEntity<GitPush>(entity, this.TfsRequestContext);
      return this.Request.CreateResponse<GitPush>(HttpStatusCode.OK, entity);
    }
  }
}
