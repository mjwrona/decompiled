// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.Controllers.Git.GitCommitsBaseController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer.Controllers.Git
{
  public abstract class GitCommitsBaseController : GitApiController
  {
    private const int c_defaultTopStatuses = 1000;
    private const int c_defaultSkipStatuses = 0;

    protected internal HttpResponseMessage QueryCommits2(
      GitQueryCommitsCriteria searchCriteria,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      int? skip = null,
      int? top = null,
      bool includeStatuses = false,
      bool latestStatusesOnly = false)
    {
      if (searchCriteria == null)
        searchCriteria = new GitQueryCommitsCriteria();
      if (searchCriteria.Ids != null && searchCriteria.Ids.Count > 0)
      {
        ArgumentUtility.EnsureIsNull((object) searchCriteria.Author, "author", this.TfsRequestContext.ServiceName);
        ArgumentUtility.EnsureIsNull((object) searchCriteria.Committer, "committer", this.TfsRequestContext.ServiceName);
        ArgumentUtility.EnsureIsNull((object) searchCriteria.CompareVersion, "compareVersion", this.TfsRequestContext.ServiceName);
        ArgumentUtility.EnsureIsNull((object) searchCriteria.FromCommitId, "fromCommitId", this.TfsRequestContext.ServiceName);
        ArgumentUtility.EnsureIsNull((object) searchCriteria.FromDate, "fromDate", this.TfsRequestContext.ServiceName);
        ArgumentUtility.EnsureIsNull((object) searchCriteria.ItemPath, "itemPath", this.TfsRequestContext.ServiceName);
        ArgumentUtility.EnsureIsNull((object) searchCriteria.ItemVersion, "itemVersion", this.TfsRequestContext.ServiceName);
        ArgumentUtility.EnsureIsNull((object) searchCriteria.ToCommitId, "toCommitId", this.TfsRequestContext.ServiceName);
        ArgumentUtility.EnsureIsNull((object) searchCriteria.ToDate, "toDate", this.TfsRequestContext.ServiceName);
        return this.Request.CreateResponse<List<GitCommitRef>>(HttpStatusCode.OK, this.ReadCommitsById(repositoryId, projectId, searchCriteria.Ids, searchCriteria.IncludeUserImageUrl, searchCriteria.IncludePushData));
      }
      IList<Tuple<Sha1Id, GitCommitRef>> source1 = (IList<Tuple<Sha1Id, GitCommitRef>>) new List<Tuple<Sha1Id, GitCommitRef>>();
      string str = (string) null;
      int? nullable = skip;
      skip = nullable ?? searchCriteria.Skip;
      nullable = top;
      top = nullable ?? searchCriteria.Top;
      skip = new int?(Math.Max(0, skip.GetValueOrDefault()));
      ref int? local = ref top;
      nullable = top;
      int num = Math.Max(0, nullable ?? 100);
      local = new int?(num);
      int top1 = top.Value == int.MaxValue ? top.Value : top.Value + 1;
      Sha1Id? itemCommitId;
      IList<TfsGitCommitHistoryEntry> source2 = this.QueryCommitsHistory(searchCriteria, repositoryId, out itemCommitId, projectId, skip.Value, top1);
      int count = source2.Count;
      nullable = top;
      int valueOrDefault = nullable.GetValueOrDefault();
      if (count > valueOrDefault & nullable.HasValue)
      {
        Dictionary<string, object> routeValues = new Dictionary<string, object>();
        routeValues.Add(nameof (repositoryId), (object) repositoryId);
        routeValues.Add("$skip", (object) skip);
        routeValues.Add("$top", (object) top);
        Guid commitsLocationId = GitWebApiConstants.CommitsLocationId;
        if (projectId != null)
        {
          commitsLocationId = GitWebApiConstants.ProjectCommitsLocationId;
          routeValues.Add(nameof (projectId), (object) projectId);
        }
        str = this.Url.Link(commitsLocationId, (object) routeValues);
      }
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ISecuredObject securedObject = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        string projectUri = tfsGitRepository.Key.GetProjectUri();
        ProjectInfo project = this.TfsRequestContext.GetService<IProjectService>().GetProject(this.TfsRequestContext, ProjectInfo.GetProjectId(projectUri));
        GitCommitTranslator commitTranslator = new GitCommitTranslator(this.TfsRequestContext, tfsGitRepository.Key, this.Url);
        foreach (TfsGitCommitHistoryEntry historyEntry in source2.Take<TfsGitCommitHistoryEntry>(top.Value))
        {
          GitCommitRef gitCommitShallow = commitTranslator.ToGitCommitShallow(historyEntry, true, searchCriteria.IncludeUserImageUrl, securedObject);
          if (historyEntry.Change != null)
          {
            GitChange gitChange1 = new GitChange();
            gitChange1.ChangeType = GitModelExtensions.ToVersionControlChangeType(historyEntry.Change.ChangeType);
            gitChange1.SourceServerItem = historyEntry.Change.ParentPath + historyEntry.Change.ChildItem;
            GitChange gitChange2 = gitChange1;
            gitChange2.SetSecuredObject(securedObject);
            gitCommitShallow.Changes = (IEnumerable<GitChange>) new GitChange[1]
            {
              gitChange2
            };
          }
          gitCommitShallow.RemoteUrl = WebLinksUtility.GetCommitRemoteUrl(this.TfsRequestContext, tfsGitRepository.Name, project.Name, gitCommitShallow.CommitId);
          gitCommitShallow.Links = searchCriteria.IncludeLinks ? gitCommitShallow.GetCommitReferenceLinks(this.TfsRequestContext, tfsGitRepository.Key, securedObject) : (ReferenceLinks) null;
          source1.Add(new Tuple<Sha1Id, GitCommitRef>(historyEntry.Commit.CommitId, gitCommitShallow));
        }
        if (includeStatuses && source1 != null && source1.Count > 0)
        {
          IEnumerable<Sha1Id> commits = source1.Select<Tuple<Sha1Id, GitCommitRef>, Sha1Id>((Func<Tuple<Sha1Id, GitCommitRef>, Sha1Id>) (entry => entry.Item1));
          ILookup<Sha1Id, GitStatus> commitStatusLookup = this.TfsRequestContext.GetService<ITeamFoundationGitCommitStatusService>().GetStatuses(this.TfsRequestContext, tfsGitRepository, commits, 1000, 0, latestStatusesOnly);
          if (commitStatusLookup != null && commitStatusLookup.Count > 0)
          {
            foreach (Tuple<Sha1Id, GitCommitRef> tuple in (IEnumerable<Tuple<Sha1Id, GitCommitRef>>) source1)
            {
              Sha1Id commit = tuple.Item1;
              IList<GitStatus> list = (IList<GitStatus>) commitStatusLookup.SelectMany<IGrouping<Sha1Id, GitStatus>, GitStatus>((Func<IGrouping<Sha1Id, GitStatus>, IEnumerable<GitStatus>>) (x => commitStatusLookup[commit])).Distinct<GitStatus>().ToList<GitStatus>();
              if (list != null && list.Count > 0)
              {
                list.ForEach<GitStatus>((Action<GitStatus>) (status => status.SetSecuredObject(securedObject)));
                tuple.Item2.Statuses = list;
              }
            }
          }
        }
        if (searchCriteria.IncludeWorkItems)
        {
          Dictionary<Sha1Id, List<int>> idsLinkedToCommits = CommitLinkHelper.GetWorkItemIdsLinkedToCommits(this.TfsRequestContext, tfsGitRepository.Key, (IList<Sha1Id>) source2.Take<TfsGitCommitHistoryEntry>(top.Value).Select<TfsGitCommitHistoryEntry, Sha1Id>((Func<TfsGitCommitHistoryEntry, Sha1Id>) (entry => entry.Commit.CommitId)).ToList<Sha1Id>());
          for (int index = 0; index < source1.Count; ++index)
            source1[index].Item2.WorkItems = (IList<ResourceRef>) GitPullRequestModelExtensions.ToWorkItemResourceRefs(this.TfsRequestContext, tfsGitRepository, (IEnumerable<int>) idsLinkedToCommits[source2[index].Commit.CommitId], securedObject).ToArray<ResourceRef>();
        }
        if (searchCriteria.IncludePushData)
          this.PopulatePushes(tfsGitRepository, (IReadOnlyDictionary<Sha1Id, GitCommitRef>) source1.ToDictionary<Tuple<Sha1Id, GitCommitRef>, Sha1Id, GitCommitRef>((Func<Tuple<Sha1Id, GitCommitRef>, Sha1Id>) (t => t.Item1), (Func<Tuple<Sha1Id, GitCommitRef>, GitCommitRef>) (t => t.Item2)), securedObject);
      }
      HttpResponseMessage response = this.Request.CreateResponse<List<GitCommitRef>>(HttpStatusCode.OK, source1.Select<Tuple<Sha1Id, GitCommitRef>, GitCommitRef>((Func<Tuple<Sha1Id, GitCommitRef>, GitCommitRef>) (t => t.Item2)).ToList<GitCommitRef>());
      if (itemCommitId.HasValue)
        response.Headers.Add("Link", string.Format("<{0}>;rel=\"startingCommitId\"", (object) itemCommitId.ToString()));
      if (!string.IsNullOrEmpty(str))
        response.Headers.Add("Link", string.Format("<{0}>;rel=\"next\"", (object) str));
      return response;
    }

    protected internal HttpResponseMessage QueryCommits(
      GitQueryCommitsCriteria searchCriteria,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      int? skip = null,
      int? top = null,
      bool includeStatuses = false,
      bool latestStatusesOnly = false)
    {
      if (searchCriteria == null)
        searchCriteria = new GitQueryCommitsCriteria();
      if (searchCriteria.Ids != null && searchCriteria.Ids.Count > 0)
      {
        ArgumentUtility.EnsureIsNull((object) searchCriteria.Author, "author", this.TfsRequestContext.ServiceName);
        ArgumentUtility.EnsureIsNull((object) searchCriteria.Committer, "committer", this.TfsRequestContext.ServiceName);
        ArgumentUtility.EnsureIsNull((object) searchCriteria.CompareVersion, "compareVersion", this.TfsRequestContext.ServiceName);
        ArgumentUtility.EnsureIsNull((object) searchCriteria.FromCommitId, "fromCommitId", this.TfsRequestContext.ServiceName);
        ArgumentUtility.EnsureIsNull((object) searchCriteria.FromDate, "fromDate", this.TfsRequestContext.ServiceName);
        ArgumentUtility.EnsureIsNull((object) searchCriteria.ItemPath, "itemPath", this.TfsRequestContext.ServiceName);
        ArgumentUtility.EnsureIsNull((object) searchCriteria.ItemVersion, "itemVersion", this.TfsRequestContext.ServiceName);
        ArgumentUtility.EnsureIsNull((object) searchCriteria.ToCommitId, "toCommitId", this.TfsRequestContext.ServiceName);
        ArgumentUtility.EnsureIsNull((object) searchCriteria.ToDate, "toDate", this.TfsRequestContext.ServiceName);
        List<GitCommitRef> entity = this.ReadCommitsById(repositoryId, projectId, searchCriteria.Ids, searchCriteria.IncludeUserImageUrl, searchCriteria.IncludePushData);
        GitStatusStateMapper.MapGitEntity<List<GitCommitRef>>(entity, this.TfsRequestContext);
        return this.Request.CreateResponse<List<GitCommitRef>>(HttpStatusCode.OK, entity);
      }
      IList<Tuple<Sha1Id, GitCommitRef>> source1 = (IList<Tuple<Sha1Id, GitCommitRef>>) new List<Tuple<Sha1Id, GitCommitRef>>();
      string str = (string) null;
      int? nullable = skip;
      skip = nullable ?? searchCriteria.Skip;
      nullable = top;
      top = nullable ?? searchCriteria.Top;
      skip = new int?(Math.Max(0, skip.GetValueOrDefault()));
      ref int? local = ref top;
      nullable = top;
      int num = Math.Max(0, nullable ?? 100);
      local = new int?(num);
      int top1 = top.Value == int.MaxValue ? top.Value : top.Value + 1;
      Sha1Id? itemCommitId;
      IList<TfsGitCommitHistoryEntry> source2 = this.QueryCommitsHistory(searchCriteria, repositoryId, out itemCommitId, projectId, skip.Value, top1);
      int count = source2.Count;
      nullable = top;
      int valueOrDefault = nullable.GetValueOrDefault();
      if (count > valueOrDefault & nullable.HasValue)
      {
        Dictionary<string, object> routeValues = new Dictionary<string, object>();
        routeValues.Add(nameof (repositoryId), (object) repositoryId);
        routeValues.Add("$skip", (object) skip);
        routeValues.Add("$top", (object) top);
        Guid commitsLocationId = GitWebApiConstants.CommitsLocationId;
        if (projectId != null)
        {
          commitsLocationId = GitWebApiConstants.ProjectCommitsLocationId;
          routeValues.Add(nameof (projectId), (object) projectId);
        }
        str = this.Url.Link(commitsLocationId, (object) routeValues);
      }
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ISecuredObject securedObject = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        string projectUri = tfsGitRepository.Key.GetProjectUri();
        ProjectInfo project = this.TfsRequestContext.GetService<IProjectService>().GetProject(this.TfsRequestContext, ProjectInfo.GetProjectId(projectUri));
        GitCommitTranslator commitTranslator = new GitCommitTranslator(this.TfsRequestContext, tfsGitRepository.Key, this.Url);
        foreach (TfsGitCommitHistoryEntry historyEntry in source2.Take<TfsGitCommitHistoryEntry>(top.Value))
        {
          GitCommitRef gitCommitShallow = commitTranslator.ToGitCommitShallow(historyEntry, true, searchCriteria.IncludeUserImageUrl, securedObject);
          if (historyEntry.Change != null)
          {
            GitChange gitChange1 = new GitChange();
            gitChange1.ChangeType = GitModelExtensions.ToVersionControlChangeType(historyEntry.Change.ChangeType);
            gitChange1.SourceServerItem = historyEntry.Change.ParentPath + historyEntry.Change.ChildItem;
            GitChange gitChange2 = gitChange1;
            gitChange2.SetSecuredObject(securedObject);
            gitCommitShallow.Changes = (IEnumerable<GitChange>) new GitChange[1]
            {
              gitChange2
            };
          }
          gitCommitShallow.RemoteUrl = WebLinksUtility.GetCommitRemoteUrl(this.TfsRequestContext, tfsGitRepository.Name, project.Name, gitCommitShallow.CommitId);
          gitCommitShallow.Links = searchCriteria.IncludeLinks ? gitCommitShallow.GetCommitReferenceLinks(this.TfsRequestContext, tfsGitRepository.Key, securedObject) : (ReferenceLinks) null;
          source1.Add(new Tuple<Sha1Id, GitCommitRef>(historyEntry.Commit.CommitId, gitCommitShallow));
        }
        if (includeStatuses && source1 != null && source1.Count > 0)
        {
          IEnumerable<Sha1Id> commits = source1.Select<Tuple<Sha1Id, GitCommitRef>, Sha1Id>((Func<Tuple<Sha1Id, GitCommitRef>, Sha1Id>) (entry => entry.Item1));
          ILookup<Sha1Id, GitStatus> commitStatusLookup = this.TfsRequestContext.GetService<ITeamFoundationGitCommitStatusService>().GetStatuses(this.TfsRequestContext, tfsGitRepository, commits, 1000, 0, latestStatusesOnly);
          if (commitStatusLookup != null && commitStatusLookup.Count > 0)
          {
            foreach (Tuple<Sha1Id, GitCommitRef> tuple in (IEnumerable<Tuple<Sha1Id, GitCommitRef>>) source1)
            {
              Sha1Id commit = tuple.Item1;
              IList<GitStatus> list = (IList<GitStatus>) commitStatusLookup.SelectMany<IGrouping<Sha1Id, GitStatus>, GitStatus>((Func<IGrouping<Sha1Id, GitStatus>, IEnumerable<GitStatus>>) (x => commitStatusLookup[commit])).Distinct<GitStatus>().ToList<GitStatus>();
              if (list != null && list.Count > 0)
              {
                GitStatusStateMapper.MapGitEntity<IList<GitStatus>>(list, this.TfsRequestContext);
                list.ForEach<GitStatus>((Action<GitStatus>) (status => status.SetSecuredObject(securedObject)));
                tuple.Item2.Statuses = list;
              }
            }
          }
        }
        if (searchCriteria.IncludeWorkItems)
        {
          Dictionary<Sha1Id, List<int>> idsLinkedToCommits = CommitLinkHelper.GetWorkItemIdsLinkedToCommits(this.TfsRequestContext, tfsGitRepository.Key, (IList<Sha1Id>) source2.Take<TfsGitCommitHistoryEntry>(top.Value).Select<TfsGitCommitHistoryEntry, Sha1Id>((Func<TfsGitCommitHistoryEntry, Sha1Id>) (entry => entry.Commit.CommitId)).ToList<Sha1Id>());
          for (int index = 0; index < source1.Count; ++index)
            source1[index].Item2.WorkItems = (IList<ResourceRef>) GitPullRequestModelExtensions.ToWorkItemResourceRefs(this.TfsRequestContext, tfsGitRepository, (IEnumerable<int>) idsLinkedToCommits[source2[index].Commit.CommitId], securedObject).ToArray<ResourceRef>();
        }
        if (searchCriteria.IncludePushData)
          this.PopulatePushes(tfsGitRepository, (IReadOnlyDictionary<Sha1Id, GitCommitRef>) source1.ToDictionary<Tuple<Sha1Id, GitCommitRef>, Sha1Id, GitCommitRef>((Func<Tuple<Sha1Id, GitCommitRef>, Sha1Id>) (t => t.Item1), (Func<Tuple<Sha1Id, GitCommitRef>, GitCommitRef>) (t => t.Item2)), securedObject);
      }
      HttpResponseMessage response = this.Request.CreateResponse<List<GitCommitRef>>(HttpStatusCode.OK, source1.Select<Tuple<Sha1Id, GitCommitRef>, GitCommitRef>((Func<Tuple<Sha1Id, GitCommitRef>, GitCommitRef>) (t => t.Item2)).ToList<GitCommitRef>());
      if (itemCommitId.HasValue)
        response.Headers.Add("Link", string.Format("<{0}>;rel=\"startingCommitId\"", (object) itemCommitId.ToString()));
      if (!string.IsNullOrEmpty(str))
        response.Headers.Add("Link", string.Format("<{0}>;rel=\"next\"", (object) str));
      return response;
    }

    private IList<TfsGitCommitHistoryEntry> QueryCommitsHistory(
      GitQueryCommitsCriteria searchCriteria,
      string repositoryId,
      out Sha1Id? itemCommitId,
      string projectId,
      int skip,
      int top)
    {
      if (searchCriteria == null)
        searchCriteria = new GitQueryCommitsCriteria();
      bool recursive = true;
      if (string.IsNullOrEmpty(searchCriteria.ItemPath) || string.Equals(searchCriteria.ItemPath, "/", StringComparison.Ordinal))
        searchCriteria.ItemPath = (string) null;
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        itemCommitId = this.TfsRequestContext.GetCommitIdFromVersionDescriptor(tfsGitRepository, searchCriteria.ItemVersion);
        Sha1Id? versionDescriptor = this.TfsRequestContext.GetCommitIdFromVersionDescriptor(tfsGitRepository, searchCriteria.CompareVersion);
        searchCriteria.ItemPath = GitItemUtility.GetItemPath(this.TfsRequestContext, tfsGitRepository, searchCriteria, this.Url, ref recursive);
        GitLogHistoryMode historyMode = this.GetHistoryMode(searchCriteria.HistoryMode);
        return (IList<TfsGitCommitHistoryEntry>) this.TfsRequestContext.GetService<ITeamFoundationGitCommitService>().QueryCommitHistory(this.TfsRequestContext, tfsGitRepository, itemCommitId, searchCriteria.ItemPath, recursive, searchCriteria.ExcludeDeletes, searchCriteria.Author, searchCriteria.Committer, this.TfsRequestContext.ParseFromDate(searchCriteria.FromDate), this.TfsRequestContext.ParseToDate(searchCriteria.ToDate), GitCommitUtility.ParseSearchObjectId(searchCriteria.FromCommitId), GitCommitUtility.ParseSearchObjectId(searchCriteria.ToCommitId), versionDescriptor, new int?(skip), new int?(top), historyMode: historyMode, showOldestCommitsFirst: searchCriteria.ShowOldestCommitsFirst).ToList<TfsGitCommitHistoryEntry>();
      }
    }

    private GitLogHistoryMode GetHistoryMode(GitHistoryMode historyMode)
    {
      switch (historyMode)
      {
        case GitHistoryMode.FirstParent:
          return GitLogHistoryMode.FirstParent;
        case GitHistoryMode.FullHistory:
          return GitLogHistoryMode.FullHistory;
        case GitHistoryMode.FullHistorySimplifyMerges:
          return GitLogHistoryMode.FullHistorySimplifyMerges;
        default:
          return GitLogHistoryMode.Simplified;
      }
    }

    private List<GitCommitRef> ReadCommitsById(
      string repositoryId,
      string projectId,
      List<string> commitIds,
      bool includeUserImageUrl,
      bool includePushData)
    {
      Dictionary<Sha1Id, GitCommitRef> shasToCommitRefs = new Dictionary<Sha1Id, GitCommitRef>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        IReadOnlyList<TfsGitCommitMetadata> commitsById = this.TfsRequestContext.GetService<ITeamFoundationGitCommitService>().GetCommitsById(this.TfsRequestContext, tfsGitRepository, commitIds.Select<string, Sha1Id>((Func<string, Sha1Id>) (id => GitCommitUtility.ParseSha1Id(id))));
        GitCommitTranslator commitTranslator = new GitCommitTranslator(this.TfsRequestContext, tfsGitRepository.Key, this.Url);
        foreach (TfsGitCommitMetadata metadata in (IEnumerable<TfsGitCommitMetadata>) commitsById)
        {
          GitCommitRef gitCommitShallow = commitTranslator.ToGitCommitShallow(metadata, includeUserImageUrl);
          gitCommitShallow.Url = this.Url.RestLink(this.TfsRequestContext, GitWebApiConstants.CommitsLocationId, (object) new
          {
            repositoryId = repositoryId,
            commitId = gitCommitShallow.CommitId
          });
          gitCommitShallow.SetSecuredObject(repositoryReadOnly);
          shasToCommitRefs.Add(metadata.CommitId, gitCommitShallow);
        }
        if (includePushData)
          this.PopulatePushes(tfsGitRepository, (IReadOnlyDictionary<Sha1Id, GitCommitRef>) shasToCommitRefs, repositoryReadOnly);
      }
      return shasToCommitRefs.Values.ToList<GitCommitRef>();
    }

    protected void PopulatePushes(
      ITfsGitRepository repository,
      IReadOnlyDictionary<Sha1Id, GitCommitRef> shasToCommitRefs,
      ISecuredObject securedObject)
    {
      IReadOnlyDictionary<Sha1Id, int> pushIdsByCommitIds = this.TfsRequestContext.GetService<ITeamFoundationGitCommitService>().GetPushIdsByCommitIds(this.TfsRequestContext, repository, shasToCommitRefs.Keys);
      Dictionary<GitCommitRef, int> commitRefsToPushIds = new Dictionary<GitCommitRef, int>();
      foreach (KeyValuePair<Sha1Id, GitCommitRef> shasToCommitRef in (IEnumerable<KeyValuePair<Sha1Id, GitCommitRef>>) shasToCommitRefs)
      {
        int num;
        if (!pushIdsByCommitIds.TryGetValue(shasToCommitRef.Key, out num))
          throw new GitCommitDoesNotExistException(shasToCommitRef.Key);
        commitRefsToPushIds.Add(shasToCommitRef.Value, num);
      }
      this.PopulatePushes(repository, (IReadOnlyDictionary<GitCommitRef, int>) commitRefsToPushIds, securedObject);
    }

    protected void PopulatePushes(
      ITfsGitRepository repository,
      IReadOnlyDictionary<GitCommitRef, int> commitRefsToPushIds,
      ISecuredObject securedObject)
    {
      IReadOnlyDictionary<int, TfsGitPushMetadata> dictionary1 = (IReadOnlyDictionary<int, TfsGitPushMetadata>) this.TfsRequestContext.GetService<ITeamFoundationGitCommitService>().GetPushDataForPushIds(this.TfsRequestContext, repository.Key, commitRefsToPushIds.Values.Distinct<int>().ToArray<int>()).ToDictionary<TfsGitPushMetadata, int>((Func<TfsGitPushMetadata, int>) (push => push.PushId));
      Guid[] array = dictionary1.Values.Select<TfsGitPushMetadata, Guid>((Func<TfsGitPushMetadata, Guid>) (push => push.PusherId)).ToArray<Guid>();
      TeamFoundationIdentity[] foundationIdentityArray = this.TfsRequestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, array);
      Dictionary<Guid, TeamFoundationIdentity> dictionary2 = new Dictionary<Guid, TeamFoundationIdentity>();
      for (int index = 0; index < array.Length; ++index)
        dictionary2[array[index]] = foundationIdentityArray[index] != null ? foundationIdentityArray[index] : throw new IdentityNotFoundException(array[index]);
      foreach (KeyValuePair<GitCommitRef, int> commitRefsToPushId in (IEnumerable<KeyValuePair<GitCommitRef, int>>) commitRefsToPushIds)
      {
        TfsGitPushMetadata tfsGitPushMetadata = dictionary1[commitRefsToPushId.Value];
        TeamFoundationIdentity identity = dictionary2[tfsGitPushMetadata.PusherId];
        commitRefsToPushId.Key.Push = new GitPushRef()
        {
          Date = tfsGitPushMetadata.PushTime,
          PushedBy = identity.ToIdentityRef(this.TfsRequestContext),
          PushId = commitRefsToPushId.Value
        };
        commitRefsToPushId.Key.Push.SetSecuredObject(securedObject);
        if (tfsGitPushMetadata.PusherId != Guid.Empty)
        {
          commitRefsToPushId.Key.Push.PushedBy.Url = Microsoft.TeamFoundation.Framework.Server.IdentityHelper.GetIdentityResourceUriString(this.TfsRequestContext, tfsGitPushMetadata.PusherId);
          commitRefsToPushId.Key.Push.PushedBy.ImageUrl = Microsoft.TeamFoundation.Framework.Server.IdentityHelper.GetImageResourceUrl(this.TfsRequestContext, tfsGitPushMetadata.PusherId);
        }
      }
    }
  }
}
