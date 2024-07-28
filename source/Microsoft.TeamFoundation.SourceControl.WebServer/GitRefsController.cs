// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitRefsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Git.Server.Utils;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "refs", ResourceVersion = 1)]
  public class GitRefsController : GitApiController
  {
    protected const string RefsPrefix = "refs/";
    protected const int c_defaultTopStatuses = 1000;
    protected const int c_defaultSkipStatuses = 0;
    protected const int c_defaultPaginationSize = 100;
    protected const int c_maxPaginationSize = 1000;

    [HttpGet]
    [ClientLocationId("2D874A60-A811-4F62-9C9F-963A6EA0A55B")]
    [ClientResponseType(typeof (IPagedList<GitRef>), null, null)]
    [ClientExample("GET__git_repositories__repositoryId__refs.json", "Refs", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__refs_heads.json", "Refs heads", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__refs_heads_statuses.json", "Refs heads statuses", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__refs_tags.json", "Refs tags", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__refs_heads_contains_replacer.json", "Refs heads that contain a word", null, null)]
    [PublicProjectRequestRestrictions]
    public virtual HttpResponseMessage GetRefs(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      [ClientQueryParameter] string filter = null,
      bool includeLinks = false,
      bool includeStatuses = false,
      bool includeMyBranches = false,
      bool latestStatusesOnly = false,
      bool peelTags = false,
      string filterContains = null,
      [ClientInclude(RestClientLanguages.CSharp | RestClientLanguages.Swagger2 | RestClientLanguages.Python | RestClientLanguages.TypeScriptWebPlatform | RestClientLanguages.Go), FromUri(Name = "$top")] int? top = null,
      [ClientInclude(RestClientLanguages.CSharp | RestClientLanguages.Swagger2 | RestClientLanguages.Python | RestClientLanguages.TypeScriptWebPlatform | RestClientLanguages.Go)] string continuationToken = null)
    {
      ISecuredObject securedObject;
      string nextToken;
      HttpResponseMessage response = this.GenerateResponse<GitRef>(this.GetRefsInternal(out securedObject, out nextToken, repositoryId, projectId, filter, includeLinks, includeStatuses, includeMyBranches, latestStatusesOnly, peelTags, filterContains, top, continuationToken, true), securedObject);
      if (nextToken != null)
        response.Headers.Add("x-ms-continuationtoken", nextToken);
      return response;
    }

    protected IEnumerable<GitRef> GetRefsInternal(
      out ISecuredObject securedObject,
      out string nextToken,
      string repositoryId,
      string projectId = null,
      string filter = null,
      bool includeLinks = false,
      bool includeStatuses = false,
      bool includeMyBranches = false,
      bool latestStatusesOnly = false,
      bool peelTags = false,
      string filterContains = null,
      int? top = null,
      string continuationToken = null,
      bool mapGitStatusState = false)
    {
      if (includeMyBranches)
      {
        if ((!string.IsNullOrEmpty(filter) ? 1 : (!string.IsNullOrEmpty(filterContains) ? 1 : 0)) != 0)
          throw new InvalidArgumentValueException(nameof (filter), Resources.Get("GitRefsFilterAndMyBranches"));
        if (top.HasValue)
          throw new ArgumentException(Resources.Get("GitRefsPaginateMyBranchesIsNotSupported"), nameof (top));
        if (continuationToken != null)
          throw new ArgumentException(Resources.Get("GitRefsPaginateMyBranchesIsNotSupported"), nameof (continuationToken));
      }
      int? nullable;
      if (continuationToken != null)
      {
        if (top.HasValue)
        {
          nullable = top;
          int num = 0;
          if (!(nullable.GetValueOrDefault() == num & nullable.HasValue))
            goto label_11;
        }
        top = new int?(100);
      }
label_11:
      if (top.HasValue)
      {
        nullable = top;
        int num1 = 0;
        if (nullable.GetValueOrDefault() <= num1 & nullable.HasValue)
        {
          top = new int?();
        }
        else
        {
          nullable = top;
          int num2 = 1000;
          if (nullable.GetValueOrDefault() > num2 & nullable.HasValue)
            top = new int?(1000);
          nullable = top;
          top = nullable.HasValue ? new int?(nullable.GetValueOrDefault() + 1) : new int?();
        }
      }
      IEnumerable<GitRef> source1 = Enumerable.Empty<GitRef>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId, true))
      {
        securedObject = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        IEnumerable<TfsGitRef> tfsGitRefs;
        if (includeMyBranches)
          tfsGitRefs = tfsGitRepository.Refs.GetMyBranches();
        else if (!string.IsNullOrEmpty(filterContains))
        {
          tfsGitRefs = (IEnumerable<TfsGitRef>) tfsGitRepository.Refs.MatchingNames((IEnumerable<string>) new string[1]
          {
            filterContains
          }, GitRefSearchType.Contains, filter ?? "", continuationToken, top);
        }
        else
        {
          string str = string.IsNullOrEmpty(filter) ? (string) null : "refs/" + filter.Trim();
          tfsGitRefs = (IEnumerable<TfsGitRef>) tfsGitRepository.Refs.MatchingNames((IEnumerable<string>) new string[1]
          {
            str
          }, GitRefSearchType.StartsWith, firstRefName: continuationToken, pageSize: top);
        }
        nextToken = (string) null;
        if (tfsGitRefs.Any<TfsGitRef>())
        {
          if (top.HasValue)
          {
            IList<TfsGitRef> source2 = this.AsList<TfsGitRef>(tfsGitRefs);
            if (source2.Count == top.Value)
            {
              TfsGitRef tfsGitRef = source2.Last<TfsGitRef>();
              nextToken = tfsGitRef.Name;
              tfsGitRefs = source2.Take<TfsGitRef>(source2.Count - 1);
            }
          }
          ILookup<Sha1Id, GitStatus> statusLookup = (ILookup<Sha1Id, GitStatus>) null;
          if (includeStatuses)
            statusLookup = this.TfsRequestContext.GetService<ITeamFoundationGitCommitStatusService>().GetStatuses(this.TfsRequestContext, tfsGitRepository, tfsGitRefs.Select<TfsGitRef, Sha1Id>((Func<TfsGitRef, Sha1Id>) (tfgitRef => tfgitRef.ObjectId)).Distinct<Sha1Id>(), 1000, 0, latestStatusesOnly);
          source1 = tfsGitRefs.ToWebApiItems(this.TfsRequestContext, tfsGitRepository.Key, includeLinks, statusLookup, peelTags ? new AnnotatedTagPeeler(tfsGitRepository) : (AnnotatedTagPeeler) null, securedObject);
          if (mapGitStatusState)
            source1 = source1.Select<GitRef, GitRef>((Func<GitRef, GitRef>) (gitRef =>
            {
              GitStatusStateMapper.MapGitEntity<GitRef>(gitRef, this.TfsRequestContext);
              return gitRef;
            }));
          if (peelTags)
            source1 = (IEnumerable<GitRef>) source1.ToList<GitRef>();
        }
        return source1;
      }
    }

    [HttpPost]
    [ClientLocationId("2D874A60-A811-4F62-9C9F-963A6EA0A55B")]
    [ClientResponseType(typeof (IEnumerable<GitRefUpdateResult>), null, null)]
    [ClientExample("POST__git_repositories__repositoryId__refs.json", "Create/Update/Delete a ref by repositoryId", null, null)]
    public HttpResponseMessage UpdateRefs(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      List<GitRefUpdate> refUpdates,
      string projectId = null)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) refUpdates, nameof (refUpdates), this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) refUpdates, nameof (refUpdates), this.TfsRequestContext.ServiceName);
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        if (tfsGitRepository.IsInMaintenance)
          throw new GitRepoInMaintenanceException(MaintenanceMessageUtils.GetSanitizedMaintenanceMessage(this.TfsRequestContext));
        List<TfsGitRefUpdateRequest> refUpdates1 = new List<TfsGitRefUpdateRequest>(refUpdates.Count);
        foreach (GitRefUpdate refUpdate in refUpdates)
        {
          Sha1Id sha1Id1 = GitCommitUtility.ParseSha1Id(refUpdate.OldObjectId);
          Sha1Id sha1Id2 = GitCommitUtility.ParseSha1Id(refUpdate.NewObjectId);
          refUpdates1.Add(new TfsGitRefUpdateRequest(refUpdate.Name, sha1Id1, sha1Id2));
        }
        TfsGitRefUpdateResultSet refUpdateResultSet = this.TfsRequestContext.GetService<ITeamFoundationGitRefService>().UpdateRefs(this.TfsRequestContext, tfsGitRepository.Key.RepoId, refUpdates1);
        if (refUpdateResultSet == null || refUpdateResultSet.Results == null)
          throw new TeamFoundationServerException(Resources.Get("ErrorUpdatingRefs"));
        Guid repoId = tfsGitRepository.Key.RepoId;
        return this.GenerateResponse<GitRefUpdateResult>(refUpdateResultSet.Results.Select<TfsGitRefUpdateResult, GitRefUpdateResult>((Func<TfsGitRefUpdateResult, GitRefUpdateResult>) (r => new GitRefUpdateResult()
        {
          RepositoryId = repoId,
          Name = r.Name,
          OldObjectId = r.OldObjectId.ToString(),
          NewObjectId = r.NewObjectId.ToString(),
          UpdateStatus = r.Status,
          RejectedBy = r.RejectedBy,
          CustomMessage = r.CustomMessage,
          IsLocked = r.IsLockedById.HasValue
        })));
      }
    }

    [HttpPatch]
    [ClientLocationId("2D874A60-A811-4F62-9C9F-963A6EA0A55B")]
    [ClientExample("PATCH__git_repositories__repositoryId__refs.json", "Lock/unlock branch", null, null)]
    public virtual GitRef UpdateRef(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientQueryParameter] string filter,
      GitRefUpdate newRefInfo,
      string projectId = null)
    {
      GitRef entity = this.UpdateRefInternal(repositoryId, filter, newRefInfo, projectId);
      GitStatusStateMapper.MapGitEntity<GitRef>(entity, this.TfsRequestContext);
      return entity;
    }

    protected GitRef UpdateRefInternal(
      string repositoryId,
      string filter,
      GitRefUpdate newRefInfo,
      string projectId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(filter, nameof (filter), "git");
      ArgumentUtility.CheckForNull<GitRefUpdate>(newRefInfo, nameof (newRefInfo), "git");
      ArgumentUtility.CheckForNull<bool>(newRefInfo.IsLocked, "IsLocked", "git");
      if (newRefInfo.NewObjectId != null || newRefInfo.OldObjectId != null || newRefInfo.Name != null)
        throw new ArgumentException(Resources.Get("InvalidRefPatchProperties")).Expected("git");
      string refName = "refs/" + filter.Trim();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        if (tfsGitRepository.IsInMaintenance)
          throw new GitRepoInMaintenanceException(MaintenanceMessageUtils.GetSanitizedMaintenanceMessage(this.TfsRequestContext));
        if (newRefInfo.IsLocked.Value)
          tfsGitRepository.Refs.Lock(refName);
        else
          tfsGitRepository.Refs.Unlock(refName);
        return tfsGitRepository.Refs.MatchingName(refName).ToWebApiItem(this.TfsRequestContext, tfsGitRepository.Key, false, (ILookup<Sha1Id, GitStatus>) null, (AnnotatedTagPeeler) null, (ISecuredObject) null);
      }
    }

    protected IList<T> AsList<T>(IEnumerable<T> items) => items is IList<T> objList ? objList : (IList<T>) items.ToList<T>();
  }
}
