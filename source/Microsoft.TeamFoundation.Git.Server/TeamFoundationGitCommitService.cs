// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TeamFoundationGitCommitService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.IdentityImage;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class TeamFoundationGitCommitService : 
    ITeamFoundationGitCommitService,
    IVssFrameworkService
  {
    private const string c_layer = "CommitService";
    private const string c_defaultGravatar = "mm";

    public IEnumerable<TfsGitCommitHistoryEntry> QueryCommitHistory(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id? commitId,
      string path,
      bool recursive,
      bool excludeDeletes,
      string author,
      string committer,
      DateTime? fromDate,
      DateTime? toDate,
      Sha1Id? fromCommitId,
      Sha1Id? toCommitId,
      Sha1Id? compareCommitId,
      int? skip,
      int? maxItemCount,
      bool useTopoOrder,
      GitLogHistoryMode historyMode,
      bool showOldestCommitsFirst = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repository, nameof (repository));
      int? nullable = maxItemCount;
      int num = 0;
      if (nullable.GetValueOrDefault() == num & nullable.HasValue)
        return Enumerable.Empty<TfsGitCommitHistoryEntry>();
      if (!commitId.HasValue && !string.IsNullOrEmpty(path) && !path.Equals("/"))
      {
        requestContext.Trace(1013538, TraceLevel.Info, GitServerUtils.TraceArea, "CommitService", "Calling QueryCommitHistory with commitId == null and path non-trivial.");
        commitId = new Sha1Id?(repository.Refs.GetDefaultOrAny().ObjectId);
      }
      List<TfsGitCommitHistoryEntry> list;
      if (!commitId.HasValue)
      {
        bool flag = requestContext.IsFeatureEnabled("Git.IsolationBitmap.Read");
        if (((!fromCommitId.HasValue || !toCommitId.HasValue || (int) fromCommitId.Value.ToByteArray()[0] != (int) toCommitId.Value.ToByteArray()[0] ? 1 : (!flag ? 1 : 0)) | (showOldestCommitsFirst ? 1 : 0)) != 0)
        {
          list = this.QueryCommits(requestContext, repository, author, committer, fromDate, toDate, fromCommitId, toCommitId, skip, maxItemCount, showOldestCommitsFirst).Select<Sha1Id, TfsGitCommit>((Func<Sha1Id, TfsGitCommit>) (id => repository.LookupObject<TfsGitCommit>(id))).Select<TfsGitCommit, TfsGitCommitHistoryEntry>((Func<TfsGitCommit, TfsGitCommitHistoryEntry>) (commit => TfsGitCommitHistoryEntry.FromQueryCommitHistory(new TfsGitCommitMetadata(commit)))).ToList<TfsGitCommitHistoryEntry>();
        }
        else
        {
          Sha1Id fromId = fromCommitId.HasValue ? fromCommitId.Value : Sha1Id.Empty;
          Sha1Id toId = toCommitId.HasValue ? toCommitId.Value : Sha1Id.Maximum;
          IEnumerable<TfsGitCommit> tfsGitCommits = repository.FindObjectsBetween<TfsGitCommit>(fromId, toId);
          if (author != null)
            tfsGitCommits = tfsGitCommits.FilterByAuthor(author);
          if (committer != null)
            tfsGitCommits = tfsGitCommits.FilterByCommitter(committer);
          if (fromDate.HasValue || toDate.HasValue)
            tfsGitCommits = tfsGitCommits.FilterByDateRange(fromDate, toDate, commitId.HasValue ? 128 : int.MaxValue);
          if (skip.HasValue)
            tfsGitCommits = tfsGitCommits.Skip<TfsGitCommit>(skip.Value);
          if (maxItemCount.HasValue)
            tfsGitCommits = tfsGitCommits.Take<TfsGitCommit>(maxItemCount.Value);
          list = tfsGitCommits.Select<TfsGitCommit, TfsGitCommitHistoryEntry>((Func<TfsGitCommit, TfsGitCommitHistoryEntry>) (commit => TfsGitCommitHistoryEntry.FromQueryCommitHistory(new TfsGitCommitMetadata(commit)))).ToList<TfsGitCommitHistoryEntry>();
        }
      }
      else
      {
        GitLogRevisionRange range = !compareCommitId.HasValue ? new GitLogRevisionRange(commitId.Value) : new GitLogRevisionRange(compareCommitId.Value, commitId.Value);
        GitLogArguments logArguments = new GitLogArguments()
        {
          FromDate = fromDate,
          ToDate = toDate,
          Author = author,
          Committer = committer,
          Path = path,
          Order = useTopoOrder ? CommitOrder.TopoOrder : CommitOrder.DateOrder,
          HistoryMode = historyMode
        };
        IEnumerable<GitLogEntry> gitLogEntries = requestContext.GetService<IGitLogService>().GitLog(requestContext, repository, (IRevisionRange) range, logArguments);
        if (fromCommitId.HasValue || toCommitId.HasValue)
          gitLogEntries = gitLogEntries.FilterBySha1IdRange(fromCommitId, toCommitId);
        if (excludeDeletes)
          gitLogEntries = gitLogEntries.Where<GitLogEntry>((Func<GitLogEntry, bool>) (logEntry => logEntry.Change == null || (logEntry.Change.ChangeType & TfsGitChangeType.Delete) == (TfsGitChangeType) 0));
        if (skip.HasValue)
          gitLogEntries = gitLogEntries.Skip<GitLogEntry>(skip.Value);
        if (maxItemCount.HasValue)
          gitLogEntries = gitLogEntries.Take<GitLogEntry>(maxItemCount.Value);
        list = gitLogEntries.Select<GitLogEntry, TfsGitCommitHistoryEntry>((Func<GitLogEntry, TfsGitCommitHistoryEntry>) (logEntry =>
        {
          TfsGitCommitHistoryEntry commitHistoryEntry = TfsGitCommitHistoryEntry.FromQueryCommitHistory(new TfsGitCommitMetadata(logEntry.Commit));
          if (logEntry.Change != null)
            commitHistoryEntry.Change = new TfsGitCommitChangeWithId(logEntry.Commit.ObjectId, logEntry.Change);
          return commitHistoryEntry;
        })).ToList<TfsGitCommitHistoryEntry>();
      }
      this.PopulateChangeCounts(requestContext, repository, list);
      return (IEnumerable<TfsGitCommitHistoryEntry>) list;
    }

    private void PopulateChangeCounts(
      IVssRequestContext rc,
      ITfsGitRepository repository,
      List<TfsGitCommitHistoryEntry> commitHistoryEntries)
    {
      if (commitHistoryEntries.Count == 0)
        return;
      IDictionary<Sha1Id, GitCommitChangeSummary> dictionary;
      using (GitOdbComponent gitOdbComponent = rc.CreateGitOdbComponent(repository.Key))
        dictionary = gitOdbComponent.ReadCommitChangeCounts(commitHistoryEntries.Select<TfsGitCommitHistoryEntry, Sha1Id>((Func<TfsGitCommitHistoryEntry, Sha1Id>) (e => e.Commit.CommitId)));
      if (dictionary.Count < commitHistoryEntries.Count)
      {
        ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(repository.Key);
        using (rc.AllowAnonymousOrPublicUserWrites(repositoryReadOnly))
          rc.GetService<IInternalGitCommitService>().QueueCommitMetadataCatchupJob(rc, repository.Key.OdbId, false);
      }
      foreach (TfsGitCommitHistoryEntry commitHistoryEntry in commitHistoryEntries)
      {
        GitCommitChangeSummary commitChangeSummary;
        if (dictionary.TryGetValue(commitHistoryEntry.Commit.CommitId, out commitChangeSummary))
          commitHistoryEntry.ChangeCounts = commitChangeSummary.ChangeCounts;
      }
    }

    private List<Sha1Id> QueryCommits(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      string author,
      string committer,
      DateTime? fromDate,
      DateTime? toDate,
      Sha1Id? fromCommitId,
      Sha1Id? toCommitId,
      int? skip,
      int? maxItemCount,
      bool orderByCommitTimeAscending)
    {
      using (requestContext.TraceBlock(1013536, 1013537, GitServerUtils.TraceArea, "CommitService", nameof (QueryCommits)))
      {
        if (fromDate.HasValue)
          fromDate = new DateTime?(fromDate.Value.ToUniversalTime());
        if (toDate.HasValue)
          toDate = new DateTime?(toDate.Value.ToUniversalTime());
        if (!fromCommitId.HasValue)
          fromCommitId = new Sha1Id?(Sha1Id.Empty);
        if (!toCommitId.HasValue)
          toCommitId = new Sha1Id?(Sha1Id.Maximum);
        using (GitOdbComponent gitOdbComponent = requestContext.CreateGitOdbComponent(repository.Key))
          return gitOdbComponent.QueryCommits(repository.Key, author, committer, fromDate, toDate, fromCommitId.Value, toCommitId.Value, skip, maxItemCount, orderByCommitTimeAscending);
      }
    }

    public IReadOnlyList<TfsGitCommitMetadata> GetOrderedCommitsById(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IEnumerable<Sha1Id> commitIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repository, nameof (repository));
      ArgumentUtility.CheckForNull<IEnumerable<Sha1Id>>(commitIds, nameof (commitIds));
      if (!(commitIds is ICollection<Sha1Id> sha1Ids))
        sha1Ids = (ICollection<Sha1Id>) new List<Sha1Id>(commitIds);
      ICollection<Sha1Id> commitIds1 = sha1Ids;
      Dictionary<Sha1Id, TfsGitCommitMetadata> dictionary = this.GetCommitsById(requestContext, repository, (IEnumerable<Sha1Id>) commitIds1).ToDictionary<TfsGitCommitMetadata, Sha1Id>((Func<TfsGitCommitMetadata, Sha1Id>) (x => x.CommitId));
      List<TfsGitCommitMetadata> orderedCommitsById = new List<TfsGitCommitMetadata>(commitIds1.Count);
      foreach (Sha1Id key in (IEnumerable<Sha1Id>) commitIds1)
      {
        TfsGitCommitMetadata gitCommitMetadata;
        if (dictionary.TryGetValue(key, out gitCommitMetadata))
          orderedCommitsById.Add(gitCommitMetadata);
        else
          orderedCommitsById.Add((TfsGitCommitMetadata) null);
      }
      return (IReadOnlyList<TfsGitCommitMetadata>) orderedCommitsById;
    }

    public IReadOnlyList<TfsGitCommitMetadata> GetCommitsById(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IEnumerable<Sha1Id> commitIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repository, nameof (repository));
      ArgumentUtility.CheckForNull<IEnumerable<Sha1Id>>(commitIds, nameof (commitIds));
      if (!(commitIds is HashSet<Sha1Id> sha1IdSet))
        sha1IdSet = new HashSet<Sha1Id>(commitIds);
      HashSet<Sha1Id> commitIds1 = sha1IdSet;
      requestContext.Trace(1013515, TraceLevel.Verbose, GitServerUtils.TraceArea, "CommitService", "GetCommits repoId:{0} commits:{1}", (object) repository.Key.RepoId, (object) commitIds1.Count);
      SecurityHelper.Instance.CheckReadPermission(requestContext, (RepoScope) repository.Key, repository.Name);
      if (commitIds1.Count == 0)
        return (IReadOnlyList<TfsGitCommitMetadata>) Array.Empty<TfsGitCommitMetadata>();
      List<TfsGitCommitMetadata> commitsById = new List<TfsGitCommitMetadata>(commitIds1.Count);
      using (GitOdbComponent gitOdbComponent = requestContext.CreateGitOdbComponent(repository.Key))
      {
        using (ResultCollection resultCollection = gitOdbComponent.ReadCommitsById((IEnumerable<Sha1Id>) commitIds1))
        {
          foreach (TfsGitCommitMetadata gitCommitMetadata in resultCollection.GetCurrent<TfsGitCommitMetadata>())
          {
            commitsById.Add(gitCommitMetadata);
            commitIds1.Remove(gitCommitMetadata.CommitId);
          }
        }
      }
      bool flag = false;
      foreach (Sha1Id objectId in commitIds1)
      {
        if (repository.TryLookupObject(objectId) is TfsGitCommit commit)
        {
          flag = true;
          commitsById.Add(new TfsGitCommitMetadata(commit));
        }
      }
      if (flag)
      {
        ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(repository.Key);
        using (requestContext.AllowAnonymousOrPublicUserWrites(repositoryReadOnly))
          requestContext.GetService<IInternalGitCommitService>().QueueCommitMetadataCatchupJob(requestContext, repository.Key.OdbId, false);
      }
      return (IReadOnlyList<TfsGitCommitMetadata>) commitsById;
    }

    public IEnumerable<TfsGitCommitLineageDiff> DiffCommitLineages(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id baseCommitId,
      IEnumerable<TfsGitRef> refs)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repository, nameof (repository));
      requestContext.Trace(1013514, TraceLevel.Verbose, GitServerUtils.TraceArea, "CommitService", "DiffCommitLineages repoId:{0} baseCommitId:{1}", (object) repository.Key.RepoId, (object) baseCommitId);
      this.CheckReadPermission(requestContext, repository, new Sha1Id?(baseCommitId));
      return (IEnumerable<TfsGitCommitLineageDiff>) this.DiffCommitLineagesAgainstCommitsWithGraph(requestContext, repository, baseCommitId, refs);
    }

    public IEnumerable<TfsGitCommitLineageDiff> DiffCommitLineagesAgainstCommits(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id baseCommitId,
      IEnumerable<Sha1Id> commitIdsToDiff)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repository, nameof (repository));
      ArgumentUtility.CheckForNull<IEnumerable<Sha1Id>>(commitIdsToDiff, nameof (commitIdsToDiff));
      requestContext.Trace(1013178, TraceLevel.Verbose, GitServerUtils.TraceArea, "CommitService", "DiffCommitLineagesForCommits repoId:{0} baseCommitId:{1}", (object) repository.Key.RepoId, (object) baseCommitId);
      this.CheckReadPermission(requestContext, repository, new Sha1Id?(baseCommitId));
      return (IEnumerable<TfsGitCommitLineageDiff>) this.DiffCommitLineagesAgainstCommitsWithGraph(requestContext, repository, baseCommitId, commitIdsToDiff.Select<Sha1Id, TfsGitRef>((Func<Sha1Id, TfsGitRef>) (id => new TfsGitRef((string) null, id))));
    }

    public bool HasMultipleMergeBases(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id commitId1,
      Sha1Id commitId2)
    {
      return new AncestralGraphAlgorithm<int, Sha1Id>().HasMultipleMergeBases((IDirectedGraph<int, Sha1Id>) repository.GetCommitGraph((IEnumerable<Sha1Id>) new Sha1Id[2]
      {
        commitId1,
        commitId2
      }), commitId1, commitId2);
    }

    public TfsGitCommit GetMergeBase(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id commitId1,
      Sha1Id commitId2)
    {
      IEnumerable<TfsGitCommit> mergeBases = this.GetMergeBases(requestContext, repository, commitId1, commitId2);
      return mergeBases == null ? (TfsGitCommit) null : mergeBases.FirstOrDefault<TfsGitCommit>();
    }

    public IEnumerable<TfsGitCommit> GetMergeBases(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id commitId1,
      Sha1Id commitId2)
    {
      List<Sha1Id> mergeBases;
      return new AncestralGraphAlgorithm<int, Sha1Id>().TryGetMergeBases((IDirectedGraph<int, Sha1Id>) repository.GetCommitGraph((IEnumerable<Sha1Id>) new Sha1Id[2]
      {
        commitId1,
        commitId2
      }), commitId1, commitId2, out mergeBases) ? mergeBases.Select<Sha1Id, TfsGitCommit>((Func<Sha1Id, TfsGitCommit>) (mergebase => repository.TryLookupObject<TfsGitCommit>(mergebase))) : (IEnumerable<TfsGitCommit>) null;
    }

    internal IReadOnlyList<TfsGitCommitLineageDiff> DiffCommitLineagesAgainstCommitsWithGraph(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id baseCommitId,
      IEnumerable<TfsGitRef> refs)
    {
      Sha1Id[] second = new Sha1Id[1]{ baseCommitId };
      List<TfsGitRef> tfsGitRefList = new List<TfsGitRef>();
      List<Sha1Id> sha1IdList = new List<Sha1Id>();
      foreach (TfsGitRef tfsGitRef in refs)
      {
        TfsGitObject tag = repository.LookupObject(tfsGitRef.ObjectId);
        TfsGitObject peeledObject = (TfsGitObject) null;
        if (tag.ObjectType == GitObjectType.Tag && (tag as TfsGitTag).TryPeelToNonTag(out peeledObject))
        {
          if (peeledObject.ObjectType != GitObjectType.Commit)
            peeledObject = (TfsGitObject) null;
        }
        else if (tag.ObjectType == GitObjectType.Commit)
          peeledObject = tag;
        if (peeledObject != null)
        {
          tfsGitRefList.Add(tfsGitRef);
          sha1IdList.Add(peeledObject.ObjectId);
        }
      }
      List<AheadBehind<Sha1Id>> list = new AncestralGraphAlgorithm<int, Sha1Id>().GetAheadBehind((IDirectedGraph<int, Sha1Id>) repository.GetCommitGraph(sha1IdList.Concat<Sha1Id>((IEnumerable<Sha1Id>) second)), baseCommitId, (IEnumerable<Sha1Id>) sha1IdList).ToList<AheadBehind<Sha1Id>>();
      List<TfsGitCommitLineageDiff> commitLineageDiffList = new List<TfsGitCommitLineageDiff>();
      for (int index = 0; index < tfsGitRefList.Count; ++index)
      {
        AheadBehind<Sha1Id> aheadBehind = list[index];
        Sha1Id objectId = aheadBehind.Item;
        TfsGitCommitMetadata metadata = new TfsGitCommitMetadata(repository.LookupObject<TfsGitCommit>(objectId));
        commitLineageDiffList.Add(new TfsGitCommitLineageDiff(metadata, aheadBehind.NumBehind, aheadBehind.NumAhead, tfsGitRefList[index].Name));
      }
      return (IReadOnlyList<TfsGitCommitLineageDiff>) commitLineageDiffList;
    }

    public CommitMetadataAndChanges GetCommitManifest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id commitId,
      bool isEntriesCountLimited)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repository, nameof (repository));
      requestContext.Trace(1013516, TraceLevel.Verbose, GitServerUtils.TraceArea, "CommitService", "GetCommitManifest repoId:{0} commitId:{1}", (object) repository.Key.RepoId, (object) commitId);
      this.CheckReadPermission(requestContext, repository, new Sha1Id?(commitId));
      Dictionary<Sha1Id, int> pushIdsByCommitIds;
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        pushIdsByCommitIds = gitCoreComponent.GetPushIdsByCommitIds(repository.Key, (IEnumerable<Sha1Id>) new Sha1Id[1]
        {
          commitId
        });
      CommitMetadataKey key;
      if (pushIdsByCommitIds.Count == 1)
      {
        key = new CommitMetadataKey(commitId, new int?(pushIdsByCommitIds.First<KeyValuePair<Sha1Id, int>>().Value), Guid.Empty);
      }
      else
      {
        if (repository.TryLookupObject<TfsGitCommit>(commitId) == null)
          return (CommitMetadataAndChanges) null;
        requestContext.Trace(1013717, TraceLevel.Warning, GitServerUtils.TraceArea, "CommitService", "Commit {0} in repo {1} doesn't exist in SQL but exists in the ODB", (object) commitId, (object) repository.Key);
        key = new CommitMetadataKey(commitId, new int?(), Guid.Empty);
      }
      return CommitMetadataAndChanges.ComputeFromKey(repository, key, isEntriesCountLimited);
    }

    public IEnumerable<TfsGitCommitHistoryEntry> QueryCommitItems(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id commitId,
      string parentPath,
      QueryCommitItemsRecursionLevel recursionLevel)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repository, nameof (repository));
      ArgumentUtility.CheckForNull<string>(parentPath, nameof (parentPath));
      requestContext.Trace(1013518, TraceLevel.Verbose, GitServerUtils.TraceArea, "CommitService", "QueryCommitItems repoId:{0} commitId:{1} parentPath:{2}", (object) repository.Key.RepoId, (object) commitId, (object) parentPath);
      this.CheckReadPermission(requestContext, repository, new Sha1Id?(commitId));
      if (!parentPath.StartsWith("/", StringComparison.Ordinal))
        parentPath = "/" + parentPath;
      if (!parentPath.EndsWith("/", StringComparison.Ordinal))
        parentPath += "/";
      return (IEnumerable<TfsGitCommitHistoryEntry>) LatestChangeAlgorithms.QueryCommitItemsFromGraphService(requestContext, repository, commitId, parentPath, recursionLevel).ToList<TfsGitCommitHistoryEntry>();
    }

    public IReadOnlyList<string> QueryCommitAuthorNames(
      IVssRequestContext requestContext,
      ITfsGitRepository repository)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repository, nameof (repository));
      requestContext.Trace(1013520, TraceLevel.Verbose, GitServerUtils.TraceArea, "CommitService", "QueryCommitAuthors repoId:{0}", (object) repository.Key.RepoId);
      this.CheckReadPermission(requestContext, repository, new Sha1Id?());
      using (GitOdbComponent gitOdbComponent = requestContext.CreateGitOdbComponent(repository.Key))
        return (IReadOnlyList<string>) gitOdbComponent.QueryCommitAuthors();
    }

    public IReadOnlyList<TfsGitPushMetadata> GetPushDataForPushIds(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int[] pushIds,
      bool includeRefUpdates = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<RepoKey>(repoKey, nameof (repoKey));
      ArgumentUtility.CheckForNull<int[]>(pushIds, nameof (pushIds));
      requestContext.Trace(1013526, TraceLevel.Verbose, GitServerUtils.TraceArea, "CommitService", "GetPushData pushIdsCount:{0}", (object) pushIds.Length);
      List<TfsGitRefLogEntry> refLogEntries = (List<TfsGitRefLogEntry>) null;
      List<TfsGitPushMetadata> items;
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
      {
        using (ResultCollection pushDataForPushIds = gitCoreComponent.GetPushDataForPushIds(repoKey, pushIds, includeRefUpdates))
        {
          items = pushDataForPushIds.GetCurrent<TfsGitPushMetadata>().Items;
          if (includeRefUpdates)
          {
            pushDataForPushIds.NextResult();
            refLogEntries = pushDataForPushIds.GetCurrent<TfsGitRefLogEntry>().Items;
          }
        }
      }
      if (includeRefUpdates && refLogEntries != null && refLogEntries.Count != 0)
        TfsGitPushMetadata.AssignRefLogsToMetadata(items, (IEnumerable<TfsGitRefLogEntry>) refLogEntries);
      return (IReadOnlyList<TfsGitPushMetadata>) items;
    }

    public IReadOnlyDictionary<Sha1Id, int> GetPushIdsByCommitIds(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IEnumerable<Sha1Id> commitIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repository, nameof (repository));
      ArgumentUtility.CheckForNull<IEnumerable<Sha1Id>>(commitIds, nameof (commitIds));
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        return (IReadOnlyDictionary<Sha1Id, int>) gitCoreComponent.GetPushIdsByCommitIds(repository.Key, commitIds);
    }

    public IReadOnlyList<TfsGitCommitMetadata> GetPushCommitsByPushId(
      IVssRequestContext requestContext,
      ITfsGitRepository repo,
      int pushId,
      int? skip,
      int? take)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repo, nameof (repo));
      ArgumentUtility.CheckForOutOfRange(pushId, nameof (pushId), 1);
      requestContext.Trace(1013167, TraceLevel.Verbose, GitServerUtils.TraceArea, "CommitService", "GetPushCommits pushId:{0}", (object) pushId);
      int? nullable = take;
      int num = 0;
      if (nullable.GetValueOrDefault() == num & nullable.HasValue)
        return (IReadOnlyList<TfsGitCommitMetadata>) Array.Empty<TfsGitCommitMetadata>();
      List<Sha1Id> pushCommitsByPushId;
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        pushCommitsByPushId = gitCoreComponent.GetPushCommitsByPushId(repo.Key, pushId, skip, take);
      return this.GetCommitsById(requestContext, repo, (IEnumerable<Sha1Id>) pushCommitsByPushId);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Artifact[] GetArtifacts(IVssRequestContext requestContext, string[] artifactUris)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string[]>(artifactUris, nameof (artifactUris));
      requestContext.Trace(1013530, TraceLevel.Verbose, GitServerUtils.TraceArea, "CommitService", "GetArtifacts artifactUrisCount:{0}", (object) artifactUris.Length);
      Artifact[] artifacts = new Artifact[artifactUris.Length];
      for (int index = 0; index < artifactUris.Length; ++index)
      {
        try
        {
          artifacts[index] = GitCommitArtifactId.DecodeArtifactUri(artifactUris[index]);
        }
        catch (ArgumentException ex)
        {
          requestContext.TraceException(1013004, GitServerUtils.TraceArea, "CommitService", (Exception) ex);
        }
      }
      return artifacts;
    }

    public GitUserDate CreateUserDate(
      IVssRequestContext rc,
      string name,
      string email,
      DateTime date,
      bool includeImageUrl = true)
    {
      string str = (string) null;
      if (includeImageUrl)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identityForEmail = GitServerUtils.GetIdentityForEmail(rc.Elevate(), email);
        if (identityForEmail != null)
        {
          IdentityRef identityRef = identityForEmail.ToIdentityRef(rc);
          object obj;
          if (identityRef.Links != null && identityRef.Links.Links.TryGetValue("avatar", out obj) && obj is ReferenceLink)
            str = ((ReferenceLink) obj).Href;
          if (string.IsNullOrEmpty(str))
            str = identityRef.ImageUrl;
        }
        else if (GitServerUtils.UseGravatarForExternalIdentities(rc))
          str = rc.GetService<IdentityImageService>().GetGravatar(rc, email, Uri.UriSchemeHttps, false, "mm");
      }
      return new GitUserDate()
      {
        Name = name,
        Email = email,
        Date = date,
        ImageUrl = str
      };
    }

    private void CheckReadPermission(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id? commitId)
    {
      SecurityHelper.Instance.CheckReadPermission(requestContext, (RepoScope) repository.Key, repository.Name);
      if (commitId.HasValue && !(repository.TryLookupObject(commitId.Value) is TfsGitCommit))
        throw new GitCommitDoesNotExistException(commitId.Value);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
