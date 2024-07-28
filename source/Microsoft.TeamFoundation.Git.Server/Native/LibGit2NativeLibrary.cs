// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Native.LibGit2NativeLibrary
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Storage;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Native
{
  internal class LibGit2NativeLibrary : IDisposable
  {
    private readonly ITfsGitRepository m_serverRepository;
    private readonly ContentDB m_repoStorage;
    private readonly IVssRequestContext m_rc;
    private readonly TfsGitOdbBackend m_tfsGitBackend;
    private readonly Repository m_repository;
    private readonly List<IDisposable> m_toDispose = new List<IDisposable>();
    private DirectoryInfo m_tempFilePath;
    private bool m_disposed;
    private static readonly string s_Layer = typeof (LibGit2NativeLibrary).Name;

    public MergeWithConflictsResult RebaseCommitUsingIndex(
      Sha1Id commitId,
      Sha1Id ontoCommitId,
      MergeWithConflictsOptions options,
      ClientTraceData ctData)
    {
      if (this.m_disposed)
        throw new ObjectDisposedException(LibGit2NativeLibrary.s_Layer);
      MergeWithConflictsResult ourResult = new MergeWithConflictsResult()
      {
        MergeBaseCommitId = ontoCommitId,
        Status = PullRequestAsyncStatus.Failure,
        FailureType = PullRequestMergeFailureType.Unknown
      };
      long totalBytesRead = this.m_tfsGitBackend.TotalBytesRead;
      int numberOfBlobsRead = this.m_tfsGitBackend.NumberOfBlobsRead;
      int numberOfTreesRead = this.m_tfsGitBackend.NumberOfTreesRead;
      int numberOfCommitsRead = this.m_tfsGitBackend.NumberOfCommitsRead;
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        Commit commit1 = this.m_repository.Lookup<Commit>(commitId.ToObjectId());
        Commit cherryPickOnto = this.m_repository.Lookup<Commit>(ontoCommitId.ToObjectId());
        MergeTreeOptions mergeTreeOptions = new MergeTreeOptions();
        mergeTreeOptions.FailOnConflict = false;
        mergeTreeOptions.FindRenames = !options.DisableRenames;
        mergeTreeOptions.RenameThreshold = options.RenameThreshold;
        mergeTreeOptions.TargetLimit = options.TargetLimit;
        mergeTreeOptions.GIT_MERGE_LINEAR_EXACT_RENAME = true;
        MergeTreeOptions options1 = mergeTreeOptions;
        TransientIndex transientIndex = this.m_repository.ObjectDatabase.CherryPickCommitIntoIndex(commit1, cherryPickOnto, this.GetMainline(commit1), options1);
        this.m_toDispose.Add((IDisposable) transientIndex);
        if (transientIndex.IsFullyMerged)
        {
          LibGit2Sharp.Signature author = commit1.Author;
          if (string.IsNullOrEmpty(author.Email))
            author = new LibGit2Sharp.Signature(author.Name, author.Name, author.When);
          Commit commit2 = this.WriteCommit(new CommitDetails(commit1.Message, author, options.CommitDetails.Committer), transientIndex.WriteToTree(), (IEnumerable<Sha1Id>) new Sha1Id[1]
          {
            ontoCommitId
          });
          ourResult.MergeCommitId = new Sha1Id(commit2.Id.RawId);
          ourResult.Status = PullRequestAsyncStatus.Succeeded;
          ourResult.FailureType = PullRequestMergeFailureType.None;
          transientIndex.Dispose();
          this.m_toDispose.Remove((IDisposable) transientIndex);
        }
        else
        {
          ourResult.Status = PullRequestAsyncStatus.Conflicts;
          ourResult.FailureType = PullRequestMergeFailureType.None;
          ourResult.Index = (Index) transientIndex;
        }
        if (ctData != null)
        {
          ctData.Add("NativeRebaseBytesRead", (object) (this.m_tfsGitBackend.TotalBytesRead - totalBytesRead));
          ctData.Add("NativeRebaseNumberOfBlobs", (object) (this.m_tfsGitBackend.NumberOfBlobsRead - numberOfBlobsRead));
          ctData.Add("NativeRebaseNumberOfTrees", (object) (this.m_tfsGitBackend.NumberOfTreesRead - numberOfTreesRead));
          ctData.Add("NativeRebaseNumberOfCommits", (object) (this.m_tfsGitBackend.NumberOfCommitsRead - numberOfCommitsRead));
        }
      }
      catch (Exception ex) when (this.HandleExceptionOnCommitMerge(ex, ourResult))
      {
      }
      if (ctData != null)
      {
        ClientTraceData clientTraceData = ctData;
        Index index = ourResult.Index;
        int? nullable;
        if (index == null)
        {
          nullable = new int?();
        }
        else
        {
          ConflictCollection conflicts = index.Conflicts;
          nullable = conflicts != null ? new int?(conflicts.Count<Conflict>()) : new int?();
        }
        // ISSUE: variable of a boxed type
        __Boxed<int> valueOrDefault = (ValueType) nullable.GetValueOrDefault();
        clientTraceData.Add("NativeRebaseConflictsFound", (object) valueOrDefault);
        ctData.Add("PullRequestMergeStatus", (object) ourResult.Status);
        ctData.Add("NativeRebaseMillisecondsTaken", (object) stopwatch.ElapsedMilliseconds);
        if (ourResult.ObservedException != null)
        {
          ctData.Add("NativeRebaseExceptionType", (object) ourResult.ObservedException.GetType().Name);
          ctData.Add("NativeRebaseExceptionMessage", (object) ourResult.ObservedException.Message);
        }
      }
      return ourResult;
    }

    public MergeWithConflictsResult CreateSemiLinearMergeCommit(
      Sha1Id targetCommitId,
      Sha1Id sourceCommitId,
      MergeWithConflictsOptions options,
      ClientTraceData ctData)
    {
      MergeWithConflictsResult ourResult = new MergeWithConflictsResult()
      {
        MergeBaseCommitId = targetCommitId,
        Status = PullRequestAsyncStatus.Failure,
        FailureType = PullRequestMergeFailureType.Unknown
      };
      Commit commit1 = this.m_repository.Lookup<Commit>(sourceCommitId.ToObjectId());
      try
      {
        Commit commit2 = this.WriteCommit(options.CommitDetails, commit1.Tree, (IEnumerable<Sha1Id>) new Sha1Id[2]
        {
          targetCommitId,
          sourceCommitId
        });
        ourResult.MergeCommitId = new Sha1Id(commit2.Id.RawId);
        ourResult.Status = PullRequestAsyncStatus.Succeeded;
        ourResult.FailureType = PullRequestMergeFailureType.None;
      }
      catch (Exception ex) when (this.HandleExceptionOnCommitMerge(ex, ourResult))
      {
      }
      return ourResult;
    }

    static LibGit2NativeLibrary() => GlobalSettings.SetEnableCaching(false);

    public LibGit2NativeLibrary(IVssRequestContext rc, ITfsGitRepository repo)
      : this(rc, repo, GitServerUtils.GetContentDB(repo))
    {
    }

    public LibGit2NativeLibrary(
      IVssRequestContext rc,
      ITfsGitRepository repository,
      ContentDB repoStorage)
    {
      this.m_rc = rc;
      this.m_serverRepository = repository;
      this.m_repoStorage = repoStorage;
      if (rc.IsFeatureEnabled("Git.LibGit2.DisableStrictObjectCreation"))
        GlobalSettings.SetEnableStrictObjectCreation(false);
      this.m_tempFilePath = Directory.CreateDirectory(Path.Combine(GitServerUtils.GetCacheDirectory(rc, repository.Key.RepoId), Path.GetRandomFileName()));
      Repository.Init(this.m_tempFilePath.FullName, true);
      this.m_tfsGitBackend = new TfsGitOdbBackend(this.m_rc, this.m_tempFilePath, this.m_repoStorage);
      this.m_repository = new Repository(this.m_tempFilePath.FullName);
      this.m_repository.ObjectDatabase.AddBackend((OdbBackend) this.m_tfsGitBackend, int.MaxValue);
    }

    public MergeWithConflictsResult CreateMergeWithConflictTracking(
      Sha1Id mergeSourceCommitId,
      Sha1Id mergeTargetCommitId,
      MergeWithConflictsOptions options,
      IEnumerable<Sha1Id> parentIdsForMergeCommit,
      ClientTraceData ctData)
    {
      if (this.m_disposed)
        throw new ObjectDisposedException(LibGit2NativeLibrary.s_Layer);
      MergeWithConflictsResult ourResult = new MergeWithConflictsResult()
      {
        Status = PullRequestAsyncStatus.Failure,
        FailureType = PullRequestMergeFailureType.Unknown
      };
      long totalBytesRead = this.m_tfsGitBackend.TotalBytesRead;
      int numberOfBlobsRead = this.m_tfsGitBackend.NumberOfBlobsRead;
      int numberOfTreesRead = this.m_tfsGitBackend.NumberOfTreesRead;
      int numberOfCommitsRead = this.m_tfsGitBackend.NumberOfCommitsRead;
      try
      {
        Commit commit1 = this.m_repository.Lookup<Commit>(mergeSourceCommitId.ToObjectId());
        Commit commit2 = this.m_repository.Lookup<Commit>(mergeTargetCommitId.ToObjectId());
        Commit mergeBase = this.m_repository.ObjectDatabase.FindMergeBase(commit1, commit2);
        ourResult.MergeBaseCommitId = !((LibGit2Sharp.GitObject) mergeBase != (LibGit2Sharp.GitObject) null) ? Sha1Id.Empty : new Sha1Id(mergeBase.Id.RawId);
        Stopwatch timer = new Stopwatch();
        MergeTreeOptions mergeTreeOptions = new MergeTreeOptions();
        mergeTreeOptions.FailOnConflict = false;
        mergeTreeOptions.FindRenames = !options.DisableRenames;
        mergeTreeOptions.RenameThreshold = options.RenameThreshold;
        mergeTreeOptions.TargetLimit = options.TargetLimit;
        mergeTreeOptions.GIT_MERGE_LINEAR_EXACT_RENAME = true;
        MergeTreeOptions options1 = mergeTreeOptions;
        TracepointUtils.Tracepoint(this.m_rc, 1013027, GitServerUtils.TraceArea, LibGit2NativeLibrary.s_Layer, (Func<object>) (() => (object) new
        {
          MergeBaseCommitId = ourResult.MergeBaseCommitId
        }), caller: nameof (CreateMergeWithConflictTracking));
        timer.Start();
        TransientIndex mergedIndex = this.m_repository.ObjectDatabase.MergeCommitsIntoIndex(commit2, commit1, options1);
        MergeTreeStatus mergeTreeStatus;
        Tree mergedTree;
        if (mergedIndex.IsFullyMerged)
        {
          mergedTree = mergedIndex.WriteToTree();
          mergeTreeStatus = MergeTreeStatus.Succeeded;
        }
        else
        {
          mergedTree = (Tree) null;
          mergeTreeStatus = MergeTreeStatus.Conflicts;
        }
        TracepointUtils.Tracepoint(this.m_rc, 1013028, GitServerUtils.TraceArea, LibGit2NativeLibrary.s_Layer, (Func<object>) (() =>
        {
          int num = (int) mergeTreeStatus;
          ConflictCollection conflicts = mergedIndex.Conflicts;
          int? nullable = conflicts != null ? new int?(conflicts.Count<Conflict>()) : new int?();
          ObjectId id = mergedTree?.Id;
          long elapsedMilliseconds = timer.ElapsedMilliseconds;
          return (object) new
          {
            mergeTreeStatus = (MergeTreeStatus) num,
            conflictCount = nullable,
            treeId = id,
            ElapsedMilliseconds = elapsedMilliseconds
          };
        }), caller: nameof (CreateMergeWithConflictTracking));
        if (mergeTreeStatus == MergeTreeStatus.Succeeded)
        {
          ourResult.Status = PullRequestAsyncStatus.Succeeded;
          ourResult.FailureType = PullRequestMergeFailureType.None;
          Commit commit3 = this.WriteCommit(options.CommitDetails, mergedTree, parentIdsForMergeCommit);
          ourResult.MergeCommitId = new Sha1Id(commit3.Id.RawId);
          mergedIndex.Dispose();
        }
        else
        {
          ourResult.Status = PullRequestAsyncStatus.Conflicts;
          ourResult.FailureType = PullRequestMergeFailureType.None;
          ourResult.Index = (Index) mergedIndex;
          this.m_toDispose.Add((IDisposable) mergedIndex);
        }
        if (ctData != null)
        {
          ctData.Add("NativeMergeBytesRead", (object) (this.m_tfsGitBackend.TotalBytesRead - totalBytesRead));
          ctData.Add("NativeMergeNumberOfBlobs", (object) (this.m_tfsGitBackend.NumberOfBlobsRead - numberOfBlobsRead));
          ctData.Add("NativeMergeNumberOfTrees", (object) (this.m_tfsGitBackend.NumberOfTreesRead - numberOfTreesRead));
          ctData.Add("NativeMergeNumberOfCommits", (object) (this.m_tfsGitBackend.NumberOfCommitsRead - numberOfCommitsRead));
          ctData.Add("NativeMergeMillisecondsTaken", (object) timer.ElapsedMilliseconds);
        }
      }
      catch (Exception ex) when (this.HandleExceptionOnCommitMerge(ex, ourResult))
      {
      }
      if (ctData != null)
      {
        ctData.Add("PullRequestMergeBaseCommitId", (object) ourResult.MergeBaseCommitId);
        ClientTraceData clientTraceData = ctData;
        Index index = ourResult.Index;
        int? nullable;
        if (index == null)
        {
          nullable = new int?();
        }
        else
        {
          ConflictCollection conflicts = index.Conflicts;
          nullable = conflicts != null ? new int?(conflicts.Count<Conflict>()) : new int?();
        }
        // ISSUE: variable of a boxed type
        __Boxed<int> valueOrDefault = (ValueType) nullable.GetValueOrDefault();
        clientTraceData.Add("NativeMergeConflictsFound", (object) valueOrDefault);
        ctData.Add("PullRequestMergeStatus", (object) ourResult.Status);
        if (ourResult.ObservedException != null)
          ctData.Add("PullRequestMergeException", (object) ourResult.ObservedException.Message);
      }
      return ourResult;
    }

    public bool HandleExceptionOnCommitMerge(Exception ex, MergeWithConflictsResult ourResult)
    {
      switch (ex)
      {
        case GitObjectRejectedException rejectedException:
          this.m_rc.Trace(1013180, TraceLevel.Error, GitServerUtils.TraceArea, LibGit2NativeLibrary.s_Layer, rejectedException.ToString());
          ourResult.Status = PullRequestAsyncStatus.RejectedByPolicy;
          ourResult.FailureType = PullRequestMergeFailureType.ObjectTooLarge;
          ourResult.MergeCommitId = Sha1Id.Empty;
          ourResult.ObservedException = (Exception) rejectedException;
          return true;
        case GitPackDeserializerException deserializerException:
          this.m_rc.TraceException(1013366, TraceLevel.Error, GitServerUtils.TraceArea, LibGit2NativeLibrary.s_Layer, (Exception) deserializerException);
          ourResult.Status = PullRequestAsyncStatus.Failure;
          ourResult.FailureType = PullRequestMergeFailureType.Unknown;
          if (deserializerException.InnerException != null && deserializerException.InnerException is TreeCaseEnforcementException)
          {
            ourResult.Status = PullRequestAsyncStatus.RejectedByPolicy;
            ourResult.FailureType = PullRequestMergeFailureType.CaseSensitive;
          }
          ourResult.MergeCommitId = Sha1Id.Empty;
          ourResult.ObservedException = (Exception) deserializerException;
          return true;
        case LibGit2SharpException git2SharpException:
          this.m_rc.TraceException(1013366, TraceLevel.Error, GitServerUtils.TraceArea, LibGit2NativeLibrary.s_Layer, (Exception) git2SharpException);
          ourResult.Status = PullRequestAsyncStatus.Failure;
          ourResult.FailureType = git2SharpException.Message.Contains("TF401022") ? PullRequestMergeFailureType.ObjectTooLarge : PullRequestMergeFailureType.Unknown;
          ourResult.MergeCommitId = Sha1Id.Empty;
          ourResult.ObservedException = (Exception) git2SharpException;
          return true;
        default:
          this.m_rc.TraceException(1013366, TraceLevel.Error, GitServerUtils.TraceArea, LibGit2NativeLibrary.s_Layer, ex);
          ourResult.Status = PullRequestAsyncStatus.Failure;
          ourResult.FailureType = PullRequestMergeFailureType.Unknown;
          ourResult.MergeCommitId = Sha1Id.Empty;
          ourResult.ObservedException = ex;
          return true;
      }
    }

    public Commit WriteCommit(
      CommitDetails commitDetails,
      Tree mergedTree,
      IEnumerable<Sha1Id> parentCommitIds)
    {
      IEnumerable<Commit> parents = parentCommitIds.Select<Sha1Id, Commit>((Func<Sha1Id, Commit>) (parentId => this.m_repository.Lookup<Commit>(parentId.ToObjectId())));
      Commit commit = this.m_repository.ObjectDatabase.CreateCommit(commitDetails.Author, commitDetails.Committer, commitDetails.Message, mergedTree, parents, true);
      this.m_tfsGitBackend.Complete(this.m_serverRepository);
      return commit;
    }

    public void CreateNewCommitsWithTreeDefinition(
      LibGit2Sharp.Signature author,
      LibGit2Sharp.Signature committer,
      string commitMessage,
      Sha1Id[] commitIds,
      Action<Repository, TreeDefinition[]> updateTreesCallback)
    {
      if (this.m_disposed)
        throw new ObjectDisposedException(LibGit2NativeLibrary.s_Layer);
      List<Commit> list1 = ((IEnumerable<Sha1Id>) commitIds).Select<Sha1Id, Commit>((Func<Sha1Id, Commit>) (id => this.m_repository.Lookup<Commit>(id.ToObjectId()))).ToList<Commit>();
      List<Tree> list2 = list1.Select<Commit, Tree>((Func<Commit, Tree>) (commit => commit.Tree)).ToList<Tree>();
      TreeDefinition[] array = list2.Select<Tree, TreeDefinition>((Func<Tree, TreeDefinition>) (tree => TreeDefinition.From(tree))).ToArray<TreeDefinition>();
      updateTreesCallback(this.m_repository, array);
      List<Tree> list3 = ((IEnumerable<TreeDefinition>) array).Select<TreeDefinition, Tree>((Func<TreeDefinition, Tree>) (def => this.m_repository.ObjectDatabase.CreateTree(def))).ToList<Tree>();
      Sha1Id[] sha1IdArray = new Sha1Id[commitIds.Length];
      for (int index = 0; index < commitIds.Length; ++index)
      {
        if (list2[index].Id != list3[index].Id)
        {
          Commit commit = this.m_repository.ObjectDatabase.CreateCommit(author, committer, commitMessage, list3[index], (IEnumerable<Commit>) new Commit[1]
          {
            list1[index]
          }, false);
          commitIds[index] = new Sha1Id(commit.Id.RawId);
        }
      }
      this.m_tfsGitBackend.Complete(this.m_serverRepository);
    }

    public AsyncOperationResult TryCherryPick(
      AsyncOperationParameters parameters,
      ClientTraceData ctData,
      Action<Sha1Id, double> progressCallback,
      Action<Sha1Id> conflictCallback,
      out List<(Sha1Id SourceCommitId, Sha1Id TargetCommitId)> sourceToTargetCommitIds)
    {
      if (this.m_disposed)
        throw new ObjectDisposedException(LibGit2NativeLibrary.s_Layer);
      sourceToTargetCommitIds = new List<(Sha1Id, Sha1Id)>();
      Commit commit1 = this.m_repository.Lookup<Commit>(new ObjectId(parameters.OntoRefHead.ToByteArray()));
      AsyncOperationResult asyncOperationResult = new AsyncOperationResult()
      {
        Success = true
      };
      this.m_rc.Trace(1020010, TraceLevel.Verbose, GitServerUtils.TraceArea, LibGit2NativeLibrary.s_Layer, "Using LibGit2Sharp to perform a cherry-pick onto {0}\n", (object) commit1?.Id);
      Stopwatch stopwatch = new Stopwatch();
      try
      {
        stopwatch.Start();
        double num1 = 1.0 / (double) parameters.CommitIds.Count;
        int num2 = 0;
        Commit cherryPickOnto = commit1;
        foreach (Sha1Id commitId in (IEnumerable<Sha1Id>) parameters.CommitIds)
        {
          Commit commit2 = this.m_repository.Lookup<Commit>(new ObjectId(commitId.ToByteArray()));
          if ((LibGit2Sharp.GitObject) commit2 == (LibGit2Sharp.GitObject) null)
            throw new GitCommitDoesNotExistException(commitId);
          MergeTreeResult mergeTreeResult = this.m_repository.ObjectDatabase.CherryPickCommit(commit2, cherryPickOnto, this.GetMainline(commit2), new MergeTreeOptions()
          {
            GIT_MERGE_LINEAR_EXACT_RENAME = true
          });
          if (mergeTreeResult.Status == MergeTreeStatus.Conflicts)
          {
            asyncOperationResult.Conflicts = mergeTreeResult.Conflicts;
            asyncOperationResult.Success = false;
            conflictCallback(commitId);
            break;
          }
          LibGit2Sharp.Signature author = commit2.Author;
          if (string.IsNullOrEmpty(author.Email))
            author = new LibGit2Sharp.Signature(author.Name, author.Name, author.When);
          Commit commit3 = this.m_repository.ObjectDatabase.CreateCommit(author, parameters.Committer, commit2.Message, mergeTreeResult.Tree, (IEnumerable<Commit>) new Commit[1]
          {
            cherryPickOnto
          }, true);
          Sha1Id sha1Id = new Sha1Id(commit3.Id.RawId);
          asyncOperationResult.Commits.Add(sha1Id);
          progressCallback(commitId, (double) (num2 + 1) * num1);
          cherryPickOnto = commit3;
          ++num2;
          sourceToTargetCommitIds.Add((commitId, sha1Id));
        }
        this.m_tfsGitBackend.Complete(this.m_serverRepository);
        stopwatch.Stop();
        this.m_rc.Trace(1020013, TraceLevel.Verbose, GitServerUtils.TraceArea, LibGit2NativeLibrary.s_Layer, "Native cherry-pick completed {0}. Elapsed ms: {1}", asyncOperationResult.Success ? (object) "successfully" : (object) "unsuccessfully", (object) stopwatch.ElapsedMilliseconds);
      }
      catch (GitObjectTooLargeException ex)
      {
        this.m_rc.Trace(1020011, TraceLevel.Error, GitServerUtils.TraceArea, LibGit2NativeLibrary.s_Layer, ex.ToString());
        asyncOperationResult.Success = false;
        throw;
      }
      catch (Exception ex)
      {
        this.m_rc.TraceException(1020012, TraceLevel.Error, GitServerUtils.TraceArea, LibGit2NativeLibrary.s_Layer, ex);
        asyncOperationResult.Success = false;
        throw;
      }
      return asyncOperationResult;
    }

    public AsyncOperationResult TryRevert(
      AsyncOperationParameters parameters,
      ClientTraceData ctData,
      Action<Sha1Id, double> progressCallback,
      Action<Sha1Id> conflictCallback)
    {
      if (this.m_disposed)
        throw new ObjectDisposedException(LibGit2NativeLibrary.s_Layer);
      AsyncOperationResult asyncOperationResult = new AsyncOperationResult()
      {
        Success = true
      };
      if (parameters.CommitIds == null || !parameters.CommitIds.Any<Sha1Id>())
        return asyncOperationResult;
      this.m_rc.Trace(1020014, TraceLevel.Verbose, GitServerUtils.TraceArea, LibGit2NativeLibrary.s_Layer, "Using LibGit2Sharp to revert {0} {1} onto {2}\n", (object) parameters.CommitIds.Count, parameters.CommitIds.Count == 1 ? (object) "commit" : (object) "commits", (object) parameters.OntoRefHead.ToString());
      Commit commit1 = this.m_repository.Lookup<Commit>(new ObjectId(parameters.OntoRefHead.ToByteArray()));
      if ((LibGit2Sharp.GitObject) commit1 == (LibGit2Sharp.GitObject) null)
        throw new GitCommitDoesNotExistException(parameters.OntoRefHead);
      Stopwatch stopwatch = new Stopwatch();
      try
      {
        stopwatch.Start();
        double num1 = 1.0 / Math.Max(4.0 * (double) parameters.CommitIds.Count, 1.0);
        double num2 = 0.0;
        Commit revertOnto = commit1;
        foreach (Sha1Id commitId in (IEnumerable<Sha1Id>) parameters.CommitIds)
        {
          double num3 = 0.0;
          Commit commit2 = this.m_repository.Lookup<Commit>(new ObjectId(commitId.ToByteArray()));
          if ((LibGit2Sharp.GitObject) commit2 == (LibGit2Sharp.GitObject) null)
            throw new GitCommitDoesNotExistException(commitId);
          if (progressCallback != null)
            progressCallback(commitId, (++num3 + 4.0 * num2) * num1);
          MergeTreeResult mergeTreeResult = this.m_repository.ObjectDatabase.RevertCommit(commit2, revertOnto, this.GetMainline(commit2), new MergeTreeOptions()
          {
            GIT_MERGE_LINEAR_EXACT_RENAME = true
          });
          if (progressCallback != null)
            progressCallback(commitId, (++num3 + 4.0 * num2) * num1);
          if (mergeTreeResult.Status == MergeTreeStatus.Conflicts)
          {
            asyncOperationResult.Conflicts = mergeTreeResult.Conflicts;
            asyncOperationResult.Success = false;
            if (conflictCallback != null)
            {
              conflictCallback(commitId);
              break;
            }
            break;
          }
          Commit commit3 = this.m_repository.ObjectDatabase.CreateCommit(parameters.Committer, parameters.Committer, Resources.Format("RevertCommitMessageFormat", (object) commit2.Message.Trim()), mergeTreeResult.Tree, (IEnumerable<Commit>) new Commit[1]
          {
            revertOnto
          }, true);
          if (progressCallback != null)
            progressCallback(commitId, (++num3 + 4.0 * num2) * num1);
          asyncOperationResult.Commits.Add(new Sha1Id(commit3.Id.RawId));
          if (progressCallback != null)
          {
            double num4;
            progressCallback(commitId, ((num4 = num3 + 1.0) + 4.0 * num2) * num1);
          }
          revertOnto = commit3;
          ++num2;
        }
        this.m_tfsGitBackend.Complete(this.m_serverRepository);
        stopwatch.Stop();
        this.m_rc.Trace(1020015, TraceLevel.Verbose, GitServerUtils.TraceArea, LibGit2NativeLibrary.s_Layer, "Native revert completed {0}. Elapsed ms: {1}", asyncOperationResult.Success ? (object) "successfully" : (object) "unsuccessfully", (object) stopwatch.ElapsedMilliseconds);
      }
      catch (GitObjectTooLargeException ex)
      {
        this.m_rc.Trace(1020016, TraceLevel.Error, GitServerUtils.TraceArea, LibGit2NativeLibrary.s_Layer, ex.ToString());
        asyncOperationResult.Success = false;
        throw;
      }
      catch (Exception ex)
      {
        this.m_rc.TraceException(1020017, TraceLevel.Error, GitServerUtils.TraceArea, LibGit2NativeLibrary.s_Layer, ex.InnerException != null ? ex.InnerException : ex);
        asyncOperationResult.Success = false;
        throw;
      }
      return asyncOperationResult;
    }

    private int GetMainline(Commit commit) => commit.Parents.Count<Commit>() > 1 ? 1 : 0;

    public void Dispose()
    {
      if (this.m_disposed)
        return;
      if (this.m_repository != null)
        this.m_repository.Dispose();
      if (this.m_tfsGitBackend != null)
        this.m_tfsGitBackend.Dispose();
      if (this.m_toDispose != null)
      {
        foreach (IDisposable disposable in this.m_toDispose)
          disposable?.Dispose();
        this.m_toDispose.Clear();
      }
      if (this.m_tempFilePath != null)
      {
        try
        {
          this.m_tempFilePath.Delete(true);
        }
        catch (IOException ex)
        {
        }
        catch (UnauthorizedAccessException ex)
        {
        }
        this.m_tempFilePath = (DirectoryInfo) null;
      }
      this.m_disposed = true;
    }
  }
}
