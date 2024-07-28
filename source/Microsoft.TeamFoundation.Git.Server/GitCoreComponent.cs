// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitCoreComponent
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Lfs;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  [SupportedSqlAccessIntent(SqlAccessIntent.ReadWrite | SqlAccessIntent.ReadOnly, null)]
  internal class GitCoreComponent : TeamFoundationSqlResourceComponent
  {
    private static readonly SqlMetaData[] typ_PullRequestCommits = new SqlMetaData[2]
    {
      new SqlMetaData("PullRequestId", SqlDbType.Int),
      new SqlMetaData("CommitId", SqlDbType.Binary, 20L)
    };
    private static readonly SqlMetaData[] typ_ReviewerUpdate = new SqlMetaData[2]
    {
      new SqlMetaData("Reviewer", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IsRequired", SqlDbType.Bit)
    };
    public const string PullRequestCounterName = "codeReviewId";
    private static readonly SqlMetaData[] typ_AdvSecBillableCommitsToAddTable = new SqlMetaData[6]
    {
      new SqlMetaData("ProjectId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("PushId", SqlDbType.Int),
      new SqlMetaData("CommitId", SqlDbType.Binary, 20L),
      new SqlMetaData("CommitNameAndEmail", SqlDbType.NVarChar, 400L),
      new SqlMetaData("CommitTime", SqlDbType.DateTime2)
    };
    private static readonly SqlMetaData[] typ_AdvSecCommitScanTable = new SqlMetaData[6]
    {
      new SqlMetaData("CommitId", SqlDbType.Binary, 20L),
      new SqlMetaData("ToolId", SqlDbType.Int),
      new SqlMetaData("VersionId", SqlDbType.Int),
      new SqlMetaData("ScanStatus", SqlDbType.Int),
      new SqlMetaData("AttemptCount", SqlDbType.Int),
      new SqlMetaData("ScanTime", SqlDbType.DateTime2)
    };
    private static readonly SqlMetaData[] typ_AdvSecCommitScanTable2 = new SqlMetaData[7]
    {
      new SqlMetaData("CommitId", SqlDbType.Binary, 20L),
      new SqlMetaData("CommitTime", SqlDbType.DateTime2),
      new SqlMetaData("ToolId", SqlDbType.Int),
      new SqlMetaData("VersionId", SqlDbType.Int),
      new SqlMetaData("ScanStatus", SqlDbType.Int),
      new SqlMetaData("AttemptCount", SqlDbType.Int),
      new SqlMetaData("ScanTime", SqlDbType.DateTime2)
    };
    private static readonly SqlMetaData[] typ_AdvSecEnablementTable = new SqlMetaData[3]
    {
      new SqlMetaData("ProjectId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("NewStatus", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_CommitAndTime = new SqlMetaData[2]
    {
      new SqlMetaData("CommitId", SqlDbType.Binary, 20L),
      new SqlMetaData("CommitTime", SqlDbType.DateTime2)
    };
    private readonly SqlMetaData[] typ_GitRefName = new SqlMetaData[1]
    {
      new SqlMetaData("Name", SqlDbType.NVarChar, 400L)
    };
    private readonly SqlMetaData[] typ_ObjectIdTable = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.Binary, 20L)
    };
    private readonly SqlMetaData[] typ_SourceToTargetObjectIdTable = new SqlMetaData[2]
    {
      new SqlMetaData("SourceObjectId", SqlDbType.Binary, 20L),
      new SqlMetaData("TargetObjectId", SqlDbType.Binary, 20L)
    };
    private static readonly SqlMetaData[] typ_RefUpdate = new SqlMetaData[4]
    {
      new SqlMetaData("Name", SqlDbType.NVarChar, 400L),
      new SqlMetaData("OldObjectId", SqlDbType.Binary, 20L),
      new SqlMetaData("NewObjectId", SqlDbType.Binary, 20L),
      new SqlMetaData("PeeledObjectId", SqlDbType.Binary, 20L)
    };
    private static readonly SqlMetaData[] typ_RefUpdate2 = new SqlMetaData[5]
    {
      new SqlMetaData("Name", SqlDbType.NVarChar, 400L),
      new SqlMetaData("OldObjectId", SqlDbType.Binary, 20L),
      new SqlMetaData("NewObjectId", SqlDbType.Binary, 20L),
      new SqlMetaData("PeeledObjectId", SqlDbType.Binary, 20L),
      new SqlMetaData("OnlyAllowDeleteIfNonExistent", SqlDbType.Bit)
    };
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[29]
    {
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(1520),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(1570),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(1580),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(1590),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(1600),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(1610),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(1620),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(1630),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(1650),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(1670),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(1830),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(1831),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(1840),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(2130),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(2160),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(2170),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(2180),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(2190),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(2200),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(2210),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(2211),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(2220),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(2240),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(2250),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(2251),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(2260),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(2271),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(2290),
      (IComponentCreator) new ComponentCreator<GitCoreComponent>(2310)
    }, "GitCore");
    public const int Tfs2018Version = 1220;

    private SqlParameter BindPullRequestCommits(
      string parameterName,
      IDictionary<int, IEnumerable<Sha1Id>> pullRequestCommits)
    {
      pullRequestCommits = pullRequestCommits ?? (IDictionary<int, IEnumerable<Sha1Id>>) new Dictionary<int, IEnumerable<Sha1Id>>();
      return this.BindTable(parameterName, "typ_PullRequestCommits", pullRequestCommits.SelectMany<KeyValuePair<int, IEnumerable<Sha1Id>>, SqlDataRecord>((System.Func<KeyValuePair<int, IEnumerable<Sha1Id>>, IEnumerable<SqlDataRecord>>) (prc =>
      {
        int key = prc.Key;
        List<SqlDataRecord> sqlDataRecordList = new List<SqlDataRecord>();
        if (prc.Value != null)
        {
          foreach (Sha1Id sha1Id in prc.Value)
          {
            SqlDataRecord sqlDataRecord = new SqlDataRecord(GitCoreComponent.typ_PullRequestCommits);
            sqlDataRecord.SetInt32(0, key);
            sqlDataRecord.SetSqlBinary(1, (SqlBinary) sha1Id.ToByteArray());
            sqlDataRecordList.Add(sqlDataRecord);
          }
        }
        return (IEnumerable<SqlDataRecord>) sqlDataRecordList;
      })));
    }

    private SqlParameter BindReviewerUpdateTable(
      string parameterName,
      IEnumerable<TfsGitPullRequest.ReviewerBase> updates)
    {
      updates = updates ?? Enumerable.Empty<TfsGitPullRequest.ReviewerBase>();
      System.Func<TfsGitPullRequest.ReviewerBase, SqlDataRecord> selector = (System.Func<TfsGitPullRequest.ReviewerBase, SqlDataRecord>) (update =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GitCoreComponent.typ_ReviewerUpdate);
        sqlDataRecord.SetGuid(0, update.Reviewer);
        sqlDataRecord.SetBoolean(1, update.IsRequired);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ReviewerUpdate", updates.Select<TfsGitPullRequest.ReviewerBase, SqlDataRecord>(selector));
    }

    private static List<TfsGitPullRequest.ReviewerWithVote> CalculateEffectiveReviewerVotes(
      IEnumerable<TfsGitPullRequest.ReviewerWithVote> reviewers,
      IEnumerable<TfsGitPullRequestDelegatedVote> delegatedVotes)
    {
      Dictionary<Guid, short> combinedVotes = reviewers.ToDictionary<TfsGitPullRequest.ReviewerWithVote, Guid, short>((System.Func<TfsGitPullRequest.ReviewerWithVote, Guid>) (x => x.Reviewer), (System.Func<TfsGitPullRequest.ReviewerWithVote, short>) (x => x.Vote));
      ILookup<Guid, Guid> votedForLists = delegatedVotes.ToLookup<TfsGitPullRequestDelegatedVote, Guid, Guid>((System.Func<TfsGitPullRequestDelegatedVote, Guid>) (x => x.ReviewerId), (System.Func<TfsGitPullRequestDelegatedVote, Guid>) (x => x.VotedForId));
      foreach (TfsGitPullRequestDelegatedVote delegatedVote in delegatedVotes)
      {
        short val2;
        short val1;
        if (combinedVotes.TryGetValue(delegatedVote.ReviewerId, out val2) && combinedVotes.TryGetValue(delegatedVote.VotedForId, out val1))
        {
          if (val1 == (short) 0)
            combinedVotes[delegatedVote.VotedForId] = val2;
          else if (val2 != (short) 0)
            combinedVotes[delegatedVote.VotedForId] = Math.Min(val1, val2);
        }
      }
      return reviewers.Select<TfsGitPullRequest.ReviewerWithVote, TfsGitPullRequest.ReviewerWithVote>((System.Func<TfsGitPullRequest.ReviewerWithVote, TfsGitPullRequest.ReviewerWithVote>) (x =>
      {
        Guid reviewer = x.Reviewer;
        int vote = (int) combinedVotes[x.Reviewer];
        IEnumerable<Guid> votedFor = votedForLists[x.Reviewer];
        bool hasDeclined = x.HasDeclined;
        int num1 = x.IsRequired ? 1 : 0;
        bool isFlagged = x.IsFlagged;
        int? pullRequestId = new int?();
        int num2 = isFlagged ? 1 : 0;
        int num3 = hasDeclined ? 1 : 0;
        return new TfsGitPullRequest.ReviewerWithVote(reviewer, (short) vote, ReviewerVoteStatus.None, votedFor, num1 != 0, pullRequestId, num2 != 0, num3 != 0);
      })).ToList<TfsGitPullRequest.ReviewerWithVote>();
    }

    public void CreateGitCommitToPullRequests(
      RepoKey repoKey,
      IDictionary<int, IEnumerable<Sha1Id>> pullRequestCommits)
    {
      this.PrepareStoredProcedure("prc_CreateGitCommitToPullRequests");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindPullRequestCommits("@pullRequestCommits", pullRequestCommits);
      this.ExecuteNonQuery();
    }

    public TfsGitPullRequest CreatePullRequest(
      RepoKey repoKey,
      Guid creatorId,
      string title,
      string description,
      string sourceBranchName,
      string targetBranchName,
      PullRequestAsyncStatus mergeStatus,
      Guid mergeId,
      Sha1Id? lastMergeSourceCommit,
      Sha1Id? lastMergeTargetCommit,
      Sha1Id? lastMergeCommit,
      bool upgraded,
      GitPullRequestMergeOptions mergeOptions,
      IEnumerable<TfsGitPullRequest.ReviewerBase> reviewers,
      int? pullRequestId = null,
      Guid? sourceRepositoryId = null,
      bool isDraft = false)
    {
      this.PrepareStoredProcedure("prc_CreateGitPullRequestAndCodeReview");
      this.BindDataspace((RepoScope) repoKey);
      if (sourceRepositoryId.HasValue)
        this.BindNullableGuid("@sourceRepositoryId", sourceRepositoryId);
      if (pullRequestId.HasValue)
        this.BindNullableInt("@pullRequestId", pullRequestId);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindGuid("@projectId", repoKey.ProjectId);
      this.BindGuid("@creatorId", creatorId);
      this.BindString("@title", title, 400, false, SqlDbType.NVarChar);
      this.BindString("@description", description, 4000, false, SqlDbType.NVarChar);
      this.BindString("@sourceName", sourceBranchName, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
      this.BindString("@targetName", targetBranchName, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
      this.BindShort("@mergeStatus", (short) mergeStatus);
      this.BindGuid("@mergeId", mergeId);
      this.BindBinary("@lastMergeSource", lastMergeSourceCommit.ToByteArray(), 20, SqlDbType.Binary);
      this.BindBinary("@lastMergeTarget", lastMergeTargetCommit.ToByteArray(), 20, SqlDbType.Binary);
      this.BindBinary("@lastMergeCommit", lastMergeCommit.ToByteArray(), 20, SqlDbType.Binary);
      this.BindBoolean("@isDraft", isDraft);
      if (mergeOptions != null)
      {
        string parameterValue = JsonConvert.SerializeObject((object) mergeOptions);
        if (parameterValue.Length > 4000)
          throw new ArgumentException("MergeOptions text is too large");
        this.BindString("@mergeOptions", parameterValue, 4000, false, SqlDbType.NVarChar);
      }
      this.BindReviewerUpdateTable("@reviewers", reviewers);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder pullRequestIdCol = new SqlColumnBinder("PullRequestId");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => pullRequestIdCol.GetInt32(reader))));
        SqlColumnBinder dateTimeCol = new SqlColumnBinder("DateTime");
        resultCollection.AddBinder<DateTime>((ObjectBinder<DateTime>) new SimpleObjectBinder<DateTime>((System.Func<IDataReader, DateTime>) (reader => dateTimeCol.GetDateTime(reader))));
        SqlColumnBinder codeReviewIdCol = new SqlColumnBinder("CodeReviewId");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => codeReviewIdCol.GetInt32(reader))));
        pullRequestId = new int?(resultCollection.GetCurrent<int>().First<int>());
        resultCollection.NextResult();
        DateTime dateTime = resultCollection.GetCurrent<DateTime>().First<DateTime>();
        resultCollection.NextResult();
        int num1 = resultCollection.GetCurrent<int>().First<int>();
        List<TfsGitPullRequest.ReviewerWithVote> reviewerWithVoteList = new List<TfsGitPullRequest.ReviewerWithVote>();
        foreach (TfsGitPullRequest.ReviewerBase reviewer in reviewers)
          reviewerWithVoteList.Add(new TfsGitPullRequest.ReviewerWithVote(reviewer));
        Guid repoId = repoKey.RepoId;
        int pullRequestId1 = pullRequestId.Value;
        Guid creator = creatorId;
        DateTime creationDate = dateTime;
        DateTime closedDate = new DateTime();
        string title1 = title;
        string description1 = description;
        string sourceBranchName1 = sourceBranchName;
        string targetBranchName1 = targetBranchName;
        int mergeStatus1 = (int) mergeStatus;
        Guid mergeId1 = mergeId;
        Sha1Id? lastMergeSourceCommit1 = lastMergeSourceCommit;
        Sha1Id? lastMergeTargetCommit1 = lastMergeTargetCommit;
        Sha1Id? lastMergeCommit1 = lastMergeCommit;
        Guid empty = Guid.Empty;
        List<TfsGitPullRequest.ReviewerWithVote> reviewers1 = reviewerWithVoteList;
        int codeReviewId = num1;
        int num2 = upgraded ? 1 : 0;
        GitPullRequestMergeOptions mergeOptions1 = mergeOptions;
        Guid? nullable = sourceRepositoryId;
        bool flag = isDraft;
        DateTime completionQueueTime = new DateTime();
        Guid autoCompleteAuthority = new Guid();
        Guid? sourceRepositoryId1 = nullable;
        int num3 = flag ? 1 : 0;
        DateTime updatedTime = new DateTime();
        return new TfsGitPullRequest(repoId, pullRequestId1, PullRequestStatus.Active, creator, creationDate, closedDate, title1, description1, sourceBranchName1, targetBranchName1, (PullRequestAsyncStatus) mergeStatus1, mergeId1, lastMergeSourceCommit1, lastMergeTargetCommit1, lastMergeCommit1, empty, (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) reviewers1, codeReviewId, num2 != 0, mergeOptions: mergeOptions1, completionQueueTime: completionQueueTime, autoCompleteAuthority: autoCompleteAuthority, sourceRepositoryId: sourceRepositoryId1, isDraft: num3 != 0, updatedTime: updatedTime);
      }
    }

    public TfsGitPullRequest CreatePullRequestWithIteration(
      RepoKey repoKey,
      Guid creatorId,
      string title,
      string description,
      string sourceBranchName,
      string targetBranchName,
      PullRequestAsyncStatus mergeStatus,
      Guid mergeId,
      Sha1Id? lastMergeSourceCommit,
      Sha1Id? lastMergeTargetCommit,
      Sha1Id? lastMergeCommit,
      bool upgraded,
      GitPullRequestMergeOptions mergeOptions,
      IEnumerable<TfsGitPullRequest.ReviewerBase> reviewers,
      Iteration iteration = null,
      int? pullRequestId = null,
      Guid? sourceRepositoryId = null,
      bool isDraft = false)
    {
      this.PrepareStoredProcedure("prc_CreateGitPullRequestWithFirstIteration");
      this.BindDataspace((RepoScope) repoKey);
      if (sourceRepositoryId.HasValue)
        this.BindNullableGuid("@sourceRepositoryId", sourceRepositoryId);
      if (pullRequestId.HasValue)
        this.BindNullableInt("@pullRequestId", pullRequestId);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindGuid("@projectId", repoKey.ProjectId);
      this.BindGuid("@creatorId", creatorId);
      this.BindString("@title", title, 400, false, SqlDbType.NVarChar);
      this.BindString("@description", description, 4000, false, SqlDbType.NVarChar);
      this.BindString("@sourceName", sourceBranchName, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
      this.BindString("@targetName", targetBranchName, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
      this.BindShort("@mergeStatus", (short) mergeStatus);
      this.BindGuid("@mergeId", mergeId);
      this.BindBinary("@lastMergeSource", lastMergeSourceCommit.ToByteArray(), 20, SqlDbType.Binary);
      this.BindBinary("@lastMergeTarget", lastMergeTargetCommit.ToByteArray(), 20, SqlDbType.Binary);
      this.BindBinary("@lastMergeCommit", lastMergeCommit.ToByteArray(), 20, SqlDbType.Binary);
      this.BindBoolean("@isDraft", isDraft);
      bool parameterValue1 = iteration != null;
      this.BindBoolean("@supportsIterations", parameterValue1);
      if (parameterValue1)
      {
        IEnumerable<ChangeEntry> changeList = (IEnumerable<ChangeEntry>) iteration.ChangeList;
        int num = changeList == null ? 0 : (changeList.Any<ChangeEntry>() ? 1 : 0);
        if (iteration.Description != null)
          this.BindString("@iterationDescription", iteration.Description, 4000, false, SqlDbType.NVarChar);
        if (num != 0)
          changeList.Bind((TeamFoundationSqlResourceComponent) this, "@changeEntries");
      }
      if (mergeOptions != null)
      {
        string parameterValue2 = JsonConvert.SerializeObject((object) mergeOptions);
        if (parameterValue2.Length > 4000)
          throw new ArgumentException("MergeOptions text is too large");
        this.BindString("@mergeOptions", parameterValue2, 4000, false, SqlDbType.NVarChar);
      }
      this.BindReviewerUpdateTable("@reviewers", reviewers);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder pullRequestIdCol = new SqlColumnBinder("PullRequestId");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => pullRequestIdCol.GetInt32(reader))));
        SqlColumnBinder dateTimeCol = new SqlColumnBinder("DateTime");
        resultCollection.AddBinder<DateTime>((ObjectBinder<DateTime>) new SimpleObjectBinder<DateTime>((System.Func<IDataReader, DateTime>) (reader => dateTimeCol.GetDateTime(reader))));
        SqlColumnBinder codeReviewIdCol = new SqlColumnBinder("CodeReviewId");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => codeReviewIdCol.GetInt32(reader))));
        pullRequestId = new int?(resultCollection.GetCurrent<int>().First<int>());
        resultCollection.NextResult();
        DateTime dateTime = resultCollection.GetCurrent<DateTime>().First<DateTime>();
        resultCollection.NextResult();
        int num1 = resultCollection.GetCurrent<int>().First<int>();
        List<TfsGitPullRequest.ReviewerWithVote> reviewerWithVoteList = new List<TfsGitPullRequest.ReviewerWithVote>();
        foreach (TfsGitPullRequest.ReviewerBase reviewer in reviewers)
          reviewerWithVoteList.Add(new TfsGitPullRequest.ReviewerWithVote(reviewer));
        Guid repoId = repoKey.RepoId;
        int pullRequestId1 = pullRequestId.Value;
        Guid creator = creatorId;
        DateTime creationDate = dateTime;
        DateTime closedDate = new DateTime();
        string title1 = title;
        string description1 = description;
        string sourceBranchName1 = sourceBranchName;
        string targetBranchName1 = targetBranchName;
        int mergeStatus1 = (int) mergeStatus;
        Guid mergeId1 = mergeId;
        Sha1Id? lastMergeSourceCommit1 = lastMergeSourceCommit;
        Sha1Id? lastMergeTargetCommit1 = lastMergeTargetCommit;
        Sha1Id? lastMergeCommit1 = lastMergeCommit;
        Guid empty = Guid.Empty;
        List<TfsGitPullRequest.ReviewerWithVote> reviewers1 = reviewerWithVoteList;
        int codeReviewId = num1;
        int num2 = upgraded ? 1 : 0;
        GitPullRequestMergeOptions mergeOptions1 = mergeOptions;
        Guid? nullable = sourceRepositoryId;
        bool flag = isDraft;
        DateTime completionQueueTime = new DateTime();
        Guid autoCompleteAuthority = new Guid();
        Guid? sourceRepositoryId1 = nullable;
        int num3 = flag ? 1 : 0;
        DateTime updatedTime = new DateTime();
        return new TfsGitPullRequest(repoId, pullRequestId1, PullRequestStatus.Active, creator, creationDate, closedDate, title1, description1, sourceBranchName1, targetBranchName1, (PullRequestAsyncStatus) mergeStatus1, mergeId1, lastMergeSourceCommit1, lastMergeTargetCommit1, lastMergeCommit1, empty, (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) reviewers1, codeReviewId, num2 != 0, mergeOptions: mergeOptions1, completionQueueTime: completionQueueTime, autoCompleteAuthority: autoCompleteAuthority, sourceRepositoryId: sourceRepositoryId1, isDraft: num3 != 0, updatedTime: updatedTime);
      }
    }

    public void CreateFirstPullRequestIteration(
      int pullRequestId,
      int dataspaceId,
      int codeReviewId,
      Iteration iteration)
    {
      IEnumerable<ChangeEntry> changeList = (IEnumerable<ChangeEntry>) iteration.ChangeList;
      int num = changeList == null ? 0 : (changeList.Any<ChangeEntry>() ? 1 : 0);
      this.PrepareStoredProcedure("prc_CreateGitPullRequestFirstIteration");
      this.BindInt("@dataspaceId", dataspaceId);
      this.BindInt("@pullRequestId", pullRequestId);
      this.BindInt("@codeReviewId", codeReviewId);
      if (iteration.Description != null)
        this.BindString("@description", iteration.Description, 4000, false, SqlDbType.NVarChar);
      if (iteration.Author != null)
        this.BindGuid("@author", Guid.Parse(iteration.Author.Id));
      if (num != 0)
        changeList.Bind((TeamFoundationSqlResourceComponent) this, "@changeEntries");
      this.ExecuteNonQuery();
    }

    private TfsGitPullRequest ExecutePullRequestReader()
    {
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        if (this.Version < 1270)
          rc.AddBinder<TfsGitPullRequest>((ObjectBinder<TfsGitPullRequest>) new GitCoreComponent.TfsGitPullRequestBinder());
        else
          rc.AddBinder<TfsGitPullRequest>((ObjectBinder<TfsGitPullRequest>) new GitCoreComponent.TfsGitPullRequestBinder2(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
        TfsGitPullRequest tfsGitPullRequest = rc.GetCurrent<TfsGitPullRequest>().FirstOrDefault<TfsGitPullRequest>();
        if (tfsGitPullRequest != null)
        {
          rc.NextResult();
          tfsGitPullRequest.Reviewers = this.ReadPullRequestReviewers(rc);
        }
        return tfsGitPullRequest;
      }
    }

    private IEnumerable<TfsGitPullRequest.ReviewerWithVote> ExecutePullRequestReviewersReader()
    {
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return this.ReadPullRequestReviewers(rc);
    }

    public virtual TfsGitPullRequest GetPullRequestDetails(Guid repositoryId, int pullRequestId)
    {
      this.PrepareStoredProcedure("prc_GetGitPullRequestDetails");
      if (repositoryId != Guid.Empty)
        this.BindGuid("@repositoryId", repositoryId);
      this.BindInt("@pullRequestId", pullRequestId);
      return this.ExecutePullRequestReader();
    }

    public IEnumerable<TfsGitPullRequest> GetPullRequestsChangedSinceLastWatermark(
      IVssRequestContext requestContext,
      DateTime updatedTime,
      int pullRequestId,
      int batchSize)
    {
      if (this.Version < 1620)
        return (IEnumerable<TfsGitPullRequest>) new List<TfsGitPullRequest>();
      this.PrepareStoredProcedure("prc_GetGitPullRequestsChangedSinceWatermark");
      this.BindDateTime2("@updatedTime", updatedTime);
      this.BindInt("@pullRequestId", pullRequestId);
      this.BindInt("@batchSize", batchSize);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TfsGitPullRequest>((ObjectBinder<TfsGitPullRequest>) new GitCoreComponent.TfsGitPullRequestBinder2(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
        return (IEnumerable<TfsGitPullRequest>) resultCollection.GetCurrent<TfsGitPullRequest>().Items;
      }
    }

    public virtual IEnumerable<TfsGitPullRequest> QueryGitPullRequests(
      string teamProjectUri,
      Guid? repositoryId,
      Guid? sourceRepositoryId,
      IEnumerable<string> sourceBranchFilters,
      IEnumerable<string> targetBranchFilters,
      bool? treatBranchFiltersAsUnion,
      PullRequestStatus? statusFilter,
      IEnumerable<Guid> creatorIdFilter,
      IEnumerable<Guid> assignedToIdFilter,
      DateTime? minClosedTime,
      Guid? mergeId,
      bool orderAscending,
      int top,
      int skip,
      bool completionAuthorityIsSet,
      DateTime? maxClosedTime = null,
      DateTime? minCreationTime = null,
      DateTime? maxCreationTime = null,
      DateTime? minUpdatedTime = null,
      DateTime? maxUpdatedTime = null,
      bool? draftFilter = null,
      bool isSecondVersionUsed = false)
    {
      if (isSecondVersionUsed && this.Version >= 2210)
        this.PrepareStoredProcedure("prc_QueryGitPullRequestsV2");
      else
        this.PrepareStoredProcedure("prc_QueryGitPullRequests");
      if (!string.IsNullOrWhiteSpace(teamProjectUri))
      {
        if (this.Version < 1270)
          this.BindString("@teamProjectUri", teamProjectUri, 256, false, SqlDbType.NVarChar);
        else
          this.BindInt("@dataspaceId", this.GetDataspaceId(ProjectInfo.GetProjectId(teamProjectUri)));
      }
      if (repositoryId.HasValue && repositoryId.Value != Guid.Empty)
        this.BindGuid("@repositoryId", repositoryId.Value);
      if (sourceRepositoryId.HasValue)
      {
        Guid? nullable = sourceRepositoryId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          this.BindGuid("@sourceRepositoryId", sourceRepositoryId.Value);
      }
      if (sourceBranchFilters != null)
        this.BindGitRefNameTable("@sourceFilterTable", sourceBranchFilters);
      if (targetBranchFilters != null)
        this.BindGitRefNameTable("@targetFilterTable", targetBranchFilters);
      if (treatBranchFiltersAsUnion.HasValue)
        this.BindBoolean("@treatBranchFiltersAsUnion", treatBranchFiltersAsUnion.Value);
      if (statusFilter.HasValue)
        this.BindByte("@statusFilter", (byte) statusFilter.Value);
      this.BindBoolean("@completionAuthorityIsSet", completionAuthorityIsSet);
      if (this.Version >= 1270)
      {
        if (creatorIdFilter != null)
          this.BindGuidTable("@creatorIdFilter", creatorIdFilter);
      }
      else if (creatorIdFilter != null && creatorIdFilter.Any<Guid>() && creatorIdFilter.First<Guid>() != Guid.Empty)
        this.BindGuid("@creatorIdFilter", creatorIdFilter.First<Guid>());
      if (this.Version >= 1270)
      {
        if (assignedToIdFilter != null)
          this.BindGuidTable("@assignedToIdFilter", assignedToIdFilter);
      }
      else if (assignedToIdFilter != null && assignedToIdFilter.Any<Guid>() && assignedToIdFilter.First<Guid>() != Guid.Empty)
        this.BindGuid("@assignedToIdFilter", assignedToIdFilter.First<Guid>());
      if (minClosedTime.HasValue)
      {
        DateTime? nullable = minClosedTime;
        DateTime minValue = DateTime.MinValue;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != minValue ? 1 : 0) : 0) : 1) != 0)
          this.BindDateTime("@minClosedTime", minClosedTime.Value);
      }
      if (mergeId.HasValue)
      {
        Guid? nullable = mergeId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          this.BindGuid("@mergeId", mergeId.Value);
      }
      this.BindBoolean("@orderAscending", orderAscending);
      if (top > 0)
        this.BindInt("@top", top);
      this.BindInt("@skip", skip);
      if (this.Version >= 1670)
      {
        if (minCreationTime.HasValue)
        {
          DateTime? nullable = minCreationTime;
          DateTime minValue = DateTime.MinValue;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != minValue ? 1 : 0) : 0) : 1) != 0)
            this.BindDateTime("@minCreationTime", minCreationTime.Value);
        }
        if (minUpdatedTime.HasValue)
        {
          DateTime? nullable = minUpdatedTime;
          DateTime minValue = DateTime.MinValue;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != minValue ? 1 : 0) : 0) : 1) != 0)
            this.BindDateTime("@minUpdatedTime", minUpdatedTime.Value);
        }
        if (draftFilter.HasValue)
          this.BindBoolean("@draftFilter", draftFilter.Value);
      }
      if (isSecondVersionUsed && this.Version >= 2210)
      {
        if (maxCreationTime.HasValue)
        {
          DateTime? nullable = maxCreationTime;
          DateTime maxValue = DateTime.MaxValue;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != maxValue ? 1 : 0) : 0) : 1) != 0)
            this.BindDateTime("@maxCreationTime", maxCreationTime.Value);
        }
        if (maxUpdatedTime.HasValue)
        {
          DateTime? nullable = maxUpdatedTime;
          DateTime maxValue = DateTime.MaxValue;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != maxValue ? 1 : 0) : 0) : 1) != 0)
            this.BindDateTime("@maxUpdatedTime", maxUpdatedTime.Value);
        }
        if (maxClosedTime.HasValue)
        {
          DateTime? nullable = maxClosedTime;
          DateTime maxValue = DateTime.MaxValue;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != maxValue ? 1 : 0) : 0) : 1) != 0)
            this.BindDateTime("@maxClosedTime", maxClosedTime.Value);
        }
      }
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        if (this.Version < 1270)
          resultCollection.AddBinder<TfsGitPullRequest>((ObjectBinder<TfsGitPullRequest>) new GitCoreComponent.TfsGitPullRequestBinder());
        else
          resultCollection.AddBinder<TfsGitPullRequest>((ObjectBinder<TfsGitPullRequest>) new GitCoreComponent.TfsGitPullRequestBinder2(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
        List<TfsGitPullRequest> items1 = resultCollection.GetCurrent<TfsGitPullRequest>().Items;
        resultCollection.NextResult();
        resultCollection.AddBinder<TfsGitPullRequest.ReviewerWithVote>((ObjectBinder<TfsGitPullRequest.ReviewerWithVote>) new GitCoreComponent.ReviewersWithVotesBinder2());
        resultCollection.AddBinder<TfsGitPullRequestDelegatedVote>((ObjectBinder<TfsGitPullRequestDelegatedVote>) new GitCoreComponent.DelegatedVoteBinder());
        List<TfsGitPullRequest.ReviewerWithVote> items2 = resultCollection.GetCurrent<TfsGitPullRequest.ReviewerWithVote>().Items;
        resultCollection.NextResult();
        List<TfsGitPullRequestDelegatedVote> items3 = resultCollection.GetCurrent<TfsGitPullRequestDelegatedVote>().Items;
        ILookup<int?, TfsGitPullRequest.ReviewerWithVote> lookup1 = items2.ToLookup<TfsGitPullRequest.ReviewerWithVote, int?>((System.Func<TfsGitPullRequest.ReviewerWithVote, int?>) (r => r.PullRequestId));
        ILookup<int?, TfsGitPullRequestDelegatedVote> lookup2 = items3.ToLookup<TfsGitPullRequestDelegatedVote, int?>((System.Func<TfsGitPullRequestDelegatedVote, int?>) (r => r.PullRequestId));
        foreach (TfsGitPullRequest tfsGitPullRequest in items1)
        {
          IEnumerable<TfsGitPullRequest.ReviewerWithVote> reviewers = lookup1[new int?(tfsGitPullRequest.PullRequestId)];
          IEnumerable<TfsGitPullRequestDelegatedVote> delegatedVotes = lookup2[new int?(tfsGitPullRequest.PullRequestId)];
          tfsGitPullRequest.Reviewers = (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) GitCoreComponent.CalculateEffectiveReviewerVotes(reviewers, delegatedVotes);
        }
        return (IEnumerable<TfsGitPullRequest>) items1;
      }
    }

    public IEnumerable<TfsGitPullRequest> QueryGitPullRequestsBulk(
      string teamProjectUri,
      Guid? repositoryId,
      PullRequestStatus? statusFilter,
      Guid? creatorIdFilter,
      IEnumerable<Guid> assignedToIdFilters,
      int top)
    {
      this.PrepareStoredProcedure("prc_QueryGitPullRequestsBulk");
      if (!string.IsNullOrWhiteSpace(teamProjectUri))
      {
        if (this.Version < 1270)
          this.BindString("@teamProjectUri", teamProjectUri, 256, false, SqlDbType.NVarChar);
        else
          this.BindInt("@dataspaceId", this.GetDataspaceId(ProjectInfo.GetProjectId(teamProjectUri)));
      }
      if (repositoryId.HasValue && repositoryId.Value != Guid.Empty)
        this.BindGuid("@repositoryId", repositoryId.Value);
      if (statusFilter.HasValue)
        this.BindByte("@statusFilter", (byte) statusFilter.Value);
      if (assignedToIdFilters != null)
        this.BindGuidTable("@assignedToIdFilterTable", assignedToIdFilters);
      if (creatorIdFilter.HasValue && creatorIdFilter.Value != Guid.Empty)
        this.BindGuid("@creatorIdFilter", creatorIdFilter.Value);
      if (top > 0)
        this.BindInt("@top", top);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        if (this.Version < 1270)
          resultCollection.AddBinder<TfsGitPullRequest>((ObjectBinder<TfsGitPullRequest>) new GitCoreComponent.TfsGitPullRequestBinder());
        else
          resultCollection.AddBinder<TfsGitPullRequest>((ObjectBinder<TfsGitPullRequest>) new GitCoreComponent.TfsGitPullRequestBinder2(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
        List<TfsGitPullRequest> items1 = resultCollection.GetCurrent<TfsGitPullRequest>().Items;
        resultCollection.NextResult();
        resultCollection.AddBinder<TfsGitPullRequest.ReviewerWithVote>((ObjectBinder<TfsGitPullRequest.ReviewerWithVote>) new GitCoreComponent.ReviewersWithVotesBinder2());
        resultCollection.AddBinder<TfsGitPullRequestDelegatedVote>((ObjectBinder<TfsGitPullRequestDelegatedVote>) new GitCoreComponent.DelegatedVoteBinder());
        List<TfsGitPullRequest.ReviewerWithVote> items2 = resultCollection.GetCurrent<TfsGitPullRequest.ReviewerWithVote>().Items;
        resultCollection.NextResult();
        List<TfsGitPullRequestDelegatedVote> items3 = resultCollection.GetCurrent<TfsGitPullRequestDelegatedVote>().Items;
        ILookup<int?, TfsGitPullRequest.ReviewerWithVote> lookup1 = items2.ToLookup<TfsGitPullRequest.ReviewerWithVote, int?>((System.Func<TfsGitPullRequest.ReviewerWithVote, int?>) (r => r.PullRequestId));
        ILookup<int?, TfsGitPullRequestDelegatedVote> lookup2 = items3.ToLookup<TfsGitPullRequestDelegatedVote, int?>((System.Func<TfsGitPullRequestDelegatedVote, int?>) (r => r.PullRequestId));
        foreach (TfsGitPullRequest tfsGitPullRequest in items1)
        {
          IEnumerable<TfsGitPullRequest.ReviewerWithVote> reviewers = lookup1[new int?(tfsGitPullRequest.PullRequestId)];
          IEnumerable<TfsGitPullRequestDelegatedVote> delegatedVotes = lookup2[new int?(tfsGitPullRequest.PullRequestId)];
          tfsGitPullRequest.Reviewers = (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) GitCoreComponent.CalculateEffectiveReviewerVotes(reviewers, delegatedVotes);
        }
        return (IEnumerable<TfsGitPullRequest>) items1;
      }
    }

    public ILookup<Sha1Id, TfsGitPullRequest> QueryGitPullRequestsByMergeCommits(
      RepoKey repoKey,
      IEnumerable<Sha1Id> commits)
    {
      this.PrepareStoredProcedure("prc_QueryGitPullRequestsByMergeCommits");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindObjectIdTable("@commits", commits);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      if (this.Version < 1270)
        resultCollection.AddBinder<TfsGitPullRequest>((ObjectBinder<TfsGitPullRequest>) new GitCoreComponent.TfsGitPullRequestBinder());
      else
        resultCollection.AddBinder<TfsGitPullRequest>((ObjectBinder<TfsGitPullRequest>) new GitCoreComponent.TfsGitPullRequestBinder2(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return resultCollection.GetCurrent<TfsGitPullRequest>().Items.ToLookup<TfsGitPullRequest, Sha1Id, TfsGitPullRequest>((System.Func<TfsGitPullRequest, Sha1Id>) (pr => pr.LastMergeCommit.Value), (System.Func<TfsGitPullRequest, TfsGitPullRequest>) (pr => pr));
    }

    public ILookup<Sha1Id, TfsGitPullRequest> QueryGitPullRequestsByCommits(
      RepoKey repoKey,
      IEnumerable<Sha1Id> commits)
    {
      this.PrepareStoredProcedure("prc_QueryGitCommitToPullRequest");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindObjectIdTable("@commits", commits);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<KeyValuePair<Sha1Id, int>>((ObjectBinder<KeyValuePair<Sha1Id, int>>) new GitCoreComponent.TfsGitCommitToPullRequestBinder());
      if (this.Version < 1270)
        resultCollection.AddBinder<TfsGitPullRequest>((ObjectBinder<TfsGitPullRequest>) new GitCoreComponent.TfsGitPullRequestBinder());
      else
        resultCollection.AddBinder<TfsGitPullRequest>((ObjectBinder<TfsGitPullRequest>) new GitCoreComponent.TfsGitPullRequestBinder2(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      List<KeyValuePair<Sha1Id, int>> items = resultCollection.GetCurrent<KeyValuePair<Sha1Id, int>>().Items;
      resultCollection.NextResult();
      IDictionary<int, TfsGitPullRequest> pullRequests = (IDictionary<int, TfsGitPullRequest>) resultCollection.GetCurrent<TfsGitPullRequest>().ToDictionary<TfsGitPullRequest, int, TfsGitPullRequest>((System.Func<TfsGitPullRequest, int>) (pr => pr.PullRequestId), (System.Func<TfsGitPullRequest, TfsGitPullRequest>) (pr => pr));
      System.Func<KeyValuePair<Sha1Id, int>, KeyValuePair<Sha1Id, TfsGitPullRequest>> selector = (System.Func<KeyValuePair<Sha1Id, int>, KeyValuePair<Sha1Id, TfsGitPullRequest>>) (kvp => new KeyValuePair<Sha1Id, TfsGitPullRequest>(kvp.Key, pullRequests[kvp.Value]));
      return items.Select<KeyValuePair<Sha1Id, int>, KeyValuePair<Sha1Id, TfsGitPullRequest>>(selector).ToLookup<KeyValuePair<Sha1Id, TfsGitPullRequest>, Sha1Id, TfsGitPullRequest>((System.Func<KeyValuePair<Sha1Id, TfsGitPullRequest>, Sha1Id>) (kvp => kvp.Key), (System.Func<KeyValuePair<Sha1Id, TfsGitPullRequest>, TfsGitPullRequest>) (kvp => kvp.Value));
    }

    public IList<TfsGitPullRequest> QueryGitPullRequestsToBackfill(
      RepoKey repoKey,
      int firstPullRequestId,
      int pullRequestsToFetch)
    {
      this.PrepareStoredProcedure("prc_QueryGitPullRequestsToBackfill");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindInt("@firstPullRequestId", firstPullRequestId);
      this.BindInt("@pullRequestsToFetch", pullRequestsToFetch);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        if (this.Version < 1270)
          resultCollection.AddBinder<TfsGitPullRequest>((ObjectBinder<TfsGitPullRequest>) new GitCoreComponent.TfsGitPullRequestBinder());
        else
          resultCollection.AddBinder<TfsGitPullRequest>((ObjectBinder<TfsGitPullRequest>) new GitCoreComponent.TfsGitPullRequestBinder2(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
        return (IList<TfsGitPullRequest>) resultCollection.GetCurrent<TfsGitPullRequest>().Items;
      }
    }

    private IEnumerable<TfsGitPullRequest.ReviewerWithVote> ReadPullRequestReviewers(
      ResultCollection rc)
    {
      rc.AddBinder<TfsGitPullRequest.ReviewerWithVote>((ObjectBinder<TfsGitPullRequest.ReviewerWithVote>) new GitCoreComponent.ReviewersWithVotesBinder2());
      rc.AddBinder<TfsGitPullRequestDelegatedVote>((ObjectBinder<TfsGitPullRequestDelegatedVote>) new GitCoreComponent.DelegatedVoteBinder());
      return (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) GitCoreComponent.CalculateEffectiveReviewerVotes((IEnumerable<TfsGitPullRequest.ReviewerWithVote>) rc.GetCurrent<TfsGitPullRequest.ReviewerWithVote>().Items, !rc.TryNextResult() ? Enumerable.Empty<TfsGitPullRequestDelegatedVote>() : (IEnumerable<TfsGitPullRequestDelegatedVote>) rc.GetCurrent<TfsGitPullRequestDelegatedVote>().Items);
    }

    public virtual TfsGitPullRequest UpdatePullRequest(
      RepoKey repoKey,
      int pullRequestId,
      PullRequestStatus status,
      string title,
      string description,
      PullRequestAsyncStatus? mergeStatus,
      Sha1Id? lastMergeSourceCommit,
      Sha1Id? lastMergeTargetCommit,
      Sha1Id? lastMergeCommit,
      Guid? completeWhenMergedAuthority,
      Sha1Id? lastMergeSourceCommitToVerify,
      int? codeReviewId,
      bool? upgraded,
      GitPullRequestCompletionOptions completionOptions,
      GitPullRequestMergeOptions mergeOptions,
      DateTime? completionQueueTime,
      PullRequestMergeFailureType? mergeFailureType,
      string mergeFailureMessage,
      IEnumerable<Sha1Id> commits,
      Guid? autoCompleteAuthority,
      bool? isDraft)
    {
      this.PrepareStoredProcedure("prc_UpdateGitPullRequestAndCodeReview");
      this.BindGuid("@projectId", repoKey.ProjectId);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindInt("@pullRequestId", pullRequestId);
      if (status != PullRequestStatus.NotSet)
        this.BindByte("@pullRequestStatus", (byte) status);
      if (title != null)
        this.BindString("@title", title, 400, false, SqlDbType.NVarChar);
      if (description != null)
        this.BindString("@description", description, 4000, false, SqlDbType.NVarChar);
      if (mergeStatus.HasValue)
        this.BindShort("@mergeStatus", (short) mergeStatus.Value);
      if (lastMergeSourceCommit.HasValue)
        this.BindBinary("@lastMergeSource", lastMergeSourceCommit.Value.ToByteArray(), 20, SqlDbType.Binary);
      if (lastMergeTargetCommit.HasValue)
        this.BindBinary("@lastMergeTarget", lastMergeTargetCommit.Value.ToByteArray(), 20, SqlDbType.Binary);
      if (lastMergeCommit.HasValue)
        this.BindBinary("@lastMergeCommit", lastMergeCommit.Value.ToByteArray(), 20, SqlDbType.Binary);
      if (completeWhenMergedAuthority.HasValue)
        this.BindGuid("@completeWhenMergedAuthority", completeWhenMergedAuthority.Value);
      if (lastMergeSourceCommitToVerify.HasValue)
        this.BindBinary("@lastMergeSourceToVerify", lastMergeSourceCommitToVerify.Value.ToByteArray(), 20, SqlDbType.Binary);
      if (upgraded.HasValue)
        this.BindBoolean("@upgraded", upgraded.Value);
      if (completionOptions != null)
      {
        string parameterValue = JsonConvert.SerializeObject((object) completionOptions);
        if (parameterValue.Length > 4000)
          throw new ArgumentException("CompletionOptions text is too large");
        this.BindString("@completionOptions", parameterValue, 4000, false, SqlDbType.NVarChar);
      }
      if (mergeOptions != null)
      {
        string parameterValue = JsonConvert.SerializeObject((object) mergeOptions);
        if (parameterValue.Length > 4000)
          throw new ArgumentException("MergeOptions text is too large");
        this.BindString("@mergeOptions", parameterValue, 4000, false, SqlDbType.NVarChar);
      }
      if (mergeFailureType.HasValue)
      {
        this.BindShort("@mergeFailureType", (short) mergeFailureType.Value);
        this.BindString("@mergeFailureMessage", mergeFailureMessage, 4000, true, SqlDbType.NVarChar);
      }
      if (isDraft.HasValue)
        this.BindBoolean("@isDraft", isDraft.Value);
      this.BindNullableDateTime2("@completionQueueTime", completionQueueTime);
      this.BindObjectIdTable("@commits", commits);
      if (autoCompleteAuthority.HasValue)
        this.BindGuid("@autoCompleteAuthority", autoCompleteAuthority.Value);
      return this.ExecutePullRequestReader();
    }

    public TfsGitPullRequest RetargetPullRequest(
      RepoKey repoKey,
      int pullRequestId,
      string sourceName,
      string targetName)
    {
      this.PrepareStoredProcedure("prc_RetargetGitPullRequest");
      this.BindGuid("@projectId", repoKey.ProjectId);
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindInt("@pullRequestId", pullRequestId);
      this.BindString("@sourceName", sourceName, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
      this.BindString("@targetName", targetName, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
      return this.ExecutePullRequestReader();
    }

    public IEnumerable<TfsGitPullRequest.ReviewerWithVote> UpdatePullRequestReviewers(
      RepoKey repoKey,
      int pullRequestId,
      IEnumerable<TfsGitPullRequest.ReviewerBase> reviewersToUpdate,
      IEnumerable<Guid> reviewerIdsToDelete,
      IEnumerable<Tuple<Guid, Guid>> updatedVotedForTable)
    {
      this.PrepareStoredProcedure("prc_UpdateGitPullRequestReviewers");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindInt("@pullRequestId", pullRequestId);
      this.BindReviewerUpdateTable("@reviewersToUpdate", reviewersToUpdate);
      this.BindGuidTable("@reviewerIdsToDelete", reviewerIdsToDelete);
      this.BindGuidGuidTable("@votedForTable", updatedVotedForTable);
      return this.ExecutePullRequestReviewersReader();
    }

    public virtual IEnumerable<TfsGitPullRequest.ReviewerWithVote> UpdatePullRequestReviewerVote(
      RepoKey repoKey,
      int pullRequestId,
      Guid reviewerId,
      short vote,
      IEnumerable<Guid> votedFor,
      bool? isFlagged = null,
      bool? hasDeclined = null)
    {
      this.PrepareStoredProcedure("prc_UpdateGitPullRequestReviewerVote");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindInt("@pullRequestId", pullRequestId);
      this.BindGuid("@reviewerId", reviewerId);
      this.BindShort("@reviewerVote", vote);
      this.BindGuidTable("@votedForIds", votedFor);
      if (this.Version >= 1570 && isFlagged.HasValue)
        this.BindBoolean("@isFlagged", isFlagged.Value);
      if (this.Version >= 1670 && hasDeclined.HasValue)
        this.BindBoolean("@hasDeclined", hasDeclined.Value);
      return this.ExecutePullRequestReviewersReader();
    }

    public IEnumerable<TfsGitPullRequest.ReviewerWithVote> ResetAllPullRequestReviewerVotes(
      RepoKey repoKey,
      int pullRequestId,
      IEnumerable<Guid> reviewerGuidsToReset)
    {
      if (this.Version >= 120)
      {
        this.PrepareStoredProcedure("prc_ResetAllPullRequestReviewerVotes");
        this.BindDataspace((RepoScope) repoKey);
        this.BindGuid("@repositoryId", repoKey.RepoId);
        this.BindInt("@pullRequestId", pullRequestId);
        return this.ExecutePullRequestReviewersReader();
      }
      Guid[] votedFor = Array.Empty<Guid>();
      IEnumerable<TfsGitPullRequest.ReviewerWithVote> reviewerWithVotes = Enumerable.Empty<TfsGitPullRequest.ReviewerWithVote>();
      foreach (Guid reviewerId in reviewerGuidsToReset)
        reviewerWithVotes = this.UpdatePullRequestReviewerVote(repoKey, pullRequestId, reviewerId, (short) 0, (IEnumerable<Guid>) votedFor);
      return reviewerWithVotes;
    }

    public IEnumerable<TfsGitPullRequest.ReviewerWithVote> ResetSomePullRequestReviewerVotes(
      RepoKey repoKey,
      int pullRequestId,
      IEnumerable<Guid> reviewerGuidsToReset)
    {
      this.PrepareStoredProcedure("prc_ResetSomePullRequestReviewerVotes");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindInt("@pullRequestId", pullRequestId);
      this.BindGuidTable("@reviewerIdsToReset", reviewerGuidsToReset);
      return this.ExecutePullRequestReviewersReader();
    }

    private SqlParameter BindAdvSecBillableCommitsToAddTable(
      string parameterName,
      IEnumerable<GitAdvSecBillableCommitsToAdd> rows)
    {
      rows = rows ?? Enumerable.Empty<GitAdvSecBillableCommitsToAdd>();
      System.Func<GitAdvSecBillableCommitsToAdd, SqlDataRecord> selector = (System.Func<GitAdvSecBillableCommitsToAdd, SqlDataRecord>) (billableCommits =>
      {
        SqlDataRecord addTable = new SqlDataRecord(GitCoreComponent.typ_AdvSecBillableCommitsToAddTable);
        addTable.SetGuid(0, billableCommits.ProjectId);
        addTable.SetGuid(1, billableCommits.RepositoryId);
        addTable.SetInt32(2, billableCommits.PushId);
        addTable.SetBytes(3, 0L, billableCommits.CommitId.ToByteArray(), 0, 20);
        addTable.SetString(4, billableCommits.CommitNameAndEmail);
        addTable.SetDateTime(5, billableCommits.CommitTime);
        return addTable;
      });
      return this.BindTable(parameterName, "typ_AdvSecBillableCommitsToAddTable", rows.Select<GitAdvSecBillableCommitsToAdd, SqlDataRecord>(selector));
    }

    private SqlParameter BindAdvSecCommitScanTable(
      string parameterName,
      IEnumerable<GitCommitScanData> rows)
    {
      rows = rows ?? Enumerable.Empty<GitCommitScanData>();
      System.Func<GitCommitScanData, SqlDataRecord> selector = (System.Func<GitCommitScanData, SqlDataRecord>) (commitScanData =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GitCoreComponent.typ_AdvSecCommitScanTable);
        sqlDataRecord.SetBytes(0, 0L, commitScanData.CommitId.ToByteArray(), 0, 20);
        sqlDataRecord.SetInt32(1, commitScanData.ToolId);
        sqlDataRecord.SetInt32(2, commitScanData.VersionId);
        sqlDataRecord.SetInt32(3, commitScanData.ScanStatus);
        sqlDataRecord.SetInt32(4, commitScanData.AttemptCount);
        sqlDataRecord.SetDateTime(5, commitScanData.ScanTime);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_AdvSecCommitScanTable", rows.Select<GitCommitScanData, SqlDataRecord>(selector));
    }

    private SqlParameter BindAdvSecCommitScanTable2(
      string parameterName,
      IEnumerable<GitCommitScanData> rows)
    {
      rows = rows ?? Enumerable.Empty<GitCommitScanData>();
      System.Func<GitCommitScanData, SqlDataRecord> selector = (System.Func<GitCommitScanData, SqlDataRecord>) (commitScanData =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GitCoreComponent.typ_AdvSecCommitScanTable2);
        sqlDataRecord.SetBytes(0, 0L, commitScanData.CommitId.ToByteArray(), 0, 20);
        sqlDataRecord.SetDateTime(1, commitScanData.CommitTime);
        sqlDataRecord.SetInt32(2, commitScanData.ToolId);
        sqlDataRecord.SetInt32(3, commitScanData.VersionId);
        sqlDataRecord.SetInt32(4, commitScanData.ScanStatus);
        sqlDataRecord.SetInt32(5, commitScanData.AttemptCount);
        sqlDataRecord.SetDateTime(6, commitScanData.ScanTime);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_AdvSecCommitScanTable2", rows.Select<GitCommitScanData, SqlDataRecord>(selector));
    }

    private SqlParameter BindAdvSecEnablementTable(
      string parameterName,
      IEnumerable<GitAdvSecEnablementUpdate> rows)
    {
      rows = rows ?? Enumerable.Empty<GitAdvSecEnablementUpdate>();
      System.Func<GitAdvSecEnablementUpdate, SqlDataRecord> selector = (System.Func<GitAdvSecEnablementUpdate, SqlDataRecord>) (update =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GitCoreComponent.typ_AdvSecEnablementTable);
        sqlDataRecord.SetGuid(0, update.ProjectId);
        sqlDataRecord.SetGuid(1, update.RepositoryId);
        sqlDataRecord.SetBoolean(2, update.NewStatus);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_AdvSecEnablementTable", rows.Select<GitAdvSecEnablementUpdate, SqlDataRecord>(selector));
    }

    private SqlParameter BindGitRefNameTable(string parameterName, IEnumerable<string> rows)
    {
      rows = rows ?? Enumerable.Empty<string>();
      System.Func<string, SqlDataRecord> selector = (System.Func<string, SqlDataRecord>) (name =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.typ_GitRefName);
        sqlDataRecord.SetString(0, name.Length <= GitConstants.MaxGitRefNameLength ? name : name.Substring(0, GitConstants.MaxGitRefNameLength));
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_GitRefName", rows.Where<string>((System.Func<string, bool>) (row => !string.IsNullOrEmpty(row))).Select<string, SqlDataRecord>(selector));
    }

    private SqlParameter BindObjectIdTable(string parameterName, IEnumerable<Sha1Id> rows)
    {
      rows = rows ?? Enumerable.Empty<Sha1Id>();
      System.Func<Sha1Id, SqlDataRecord> selector = (System.Func<Sha1Id, SqlDataRecord>) (hash =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.typ_ObjectIdTable);
        sqlDataRecord.SetBytes(0, 0L, hash.ToByteArray(), 0, 20);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ObjectIdTable", rows.Select<Sha1Id, SqlDataRecord>(selector));
    }

    private SqlParameter BindSourceToTargetObjectIdTable(
      string parameterName,
      List<(Sha1Id SourceCommitId, Sha1Id TargetCommitId)> oldToNewCommitIds)
    {
      // ISSUE: method pointer
      return this.BindTable(parameterName, "typ_SourceToTargetObjectIdTable", oldToNewCommitIds.Select<(Sha1Id, Sha1Id), SqlDataRecord>(new System.Func<(Sha1Id, Sha1Id), SqlDataRecord>((object) this, __methodptr(\u003CBindSourceToTargetObjectIdTable\u003Eg__rowBinder\u007C75_0))));
    }

    private SqlParameter BindRefUpdateTable(
      string parameterName,
      IEnumerable<TfsGitRefUpdateWithResolvedCommit> rows)
    {
      rows = rows ?? Enumerable.Empty<TfsGitRefUpdateWithResolvedCommit>();
      System.Func<TfsGitRefUpdateWithResolvedCommit, SqlDataRecord> selector = (System.Func<TfsGitRefUpdateWithResolvedCommit, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GitCoreComponent.typ_RefUpdate);
        sqlDataRecord.SetString(0, row.UpdateRequest.Name);
        sqlDataRecord.SetBytes(1, 0L, row.UpdateRequest.OldObjectId.ToByteArray(), 0, 20);
        sqlDataRecord.SetBytes(2, 0L, row.UpdateRequest.NewObjectId.ToByteArray(), 0, 20);
        if (row.ResolvedCommitId.HasValue)
          sqlDataRecord.SetBytes(3, 0L, row.ResolvedCommitId.ToByteArray(), 0, 20);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_RefUpdate", rows.Select<TfsGitRefUpdateWithResolvedCommit, SqlDataRecord>(selector));
    }

    private SqlParameter BindRefUpdateRequestTable(
      string parameterName,
      IEnumerable<TfsGitRefUpdateWithResolvedCommit> rows,
      IDictionary<string, Sha1Id> updatesToFailIfRefsExist)
    {
      rows = rows ?? Enumerable.Empty<TfsGitRefUpdateWithResolvedCommit>();
      System.Func<TfsGitRefUpdateWithResolvedCommit, SqlDataRecord> selector = (System.Func<TfsGitRefUpdateWithResolvedCommit, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GitCoreComponent.typ_RefUpdate2);
        sqlDataRecord.SetString(0, row.UpdateRequest.Name);
        sqlDataRecord.SetBytes(1, 0L, row.UpdateRequest.OldObjectId.ToByteArray(), 0, 20);
        sqlDataRecord.SetBytes(2, 0L, row.UpdateRequest.NewObjectId.ToByteArray(), 0, 20);
        if (row.ResolvedCommitId.HasValue)
          sqlDataRecord.SetBytes(3, 0L, row.ResolvedCommitId.ToByteArray(), 0, 20);
        bool flag = updatesToFailIfRefsExist.ContainsKey(row.UpdateRequest.Name);
        sqlDataRecord.SetBoolean(4, flag);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_RefUpdateRequest", rows.Select<TfsGitRefUpdateWithResolvedCommit, SqlDataRecord>(selector));
    }

    static GitCoreComponent() => GitCoreComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1200002,
        new SqlExceptionFactory(typeof (GenericDatabaseUpdateErrorException))
      },
      {
        1200022,
        new SqlExceptionFactory(typeof (GenericErrorException))
      },
      {
        1200016,
        new SqlExceptionFactory(typeof (GitPullRequestExistsException))
      },
      {
        1200015,
        new SqlExceptionFactory(typeof (GitPullRequestNotEditableException))
      },
      {
        1200014,
        new SqlExceptionFactory(typeof (GitPullRequestNotFoundException))
      },
      {
        1200017,
        new SqlExceptionFactory(typeof (GitRepositoryRequiredForBranchFilterException))
      },
      {
        1200018,
        new SqlExceptionFactory(typeof (GitRefLockDeniedException))
      },
      {
        1200011,
        new SqlExceptionFactory(typeof (GitRefNotFoundException))
      },
      {
        1200019,
        new SqlExceptionFactory(typeof (GitRefUnlockDeniedException))
      },
      {
        1200013,
        new SqlExceptionFactory(typeof (GitRepositoryMinimumPerProjectThresholdExceededException))
      },
      {
        1200010,
        new SqlExceptionFactory(typeof (GitRepositoryNameAlreadyExistsException))
      },
      {
        1200031,
        new SqlExceptionFactory(typeof (GitRepositoryNameStateConstrainException))
      },
      {
        1200009,
        new SqlExceptionFactory(typeof (GitRepositoryNotFoundException))
      },
      {
        1200012,
        new SqlExceptionFactory(typeof (GitRepositoryPerProjectThresholdExceededException))
      },
      {
        1200020,
        new SqlExceptionFactory(typeof (GitPullRequestCannotBeActivated))
      },
      {
        1200021,
        new SqlExceptionFactory(typeof (GitCommitDoesNotExistException))
      },
      {
        1200026,
        new SqlExceptionFactory(typeof (GitInvalidParentSpecified))
      },
      {
        1200027,
        new SqlExceptionFactory(typeof (GitDuplicateRefFavoriteException))
      }
    };

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) GitCoreComponent.s_sqlExceptionFactories;

    public GitCoreComponent()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    public void AddCherryPickRelationships(
      RepoKey repoKey,
      List<(Sha1Id SourceCommitId, Sha1Id TargetCommitId)> sourceToTargetCommitIds)
    {
      this.PrepareStoredProcedure("prc_AddCherryPickRelationships");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindSourceToTargetObjectIdTable("@sourceToTargetCommitIds", sourceToTargetCommitIds);
      this.ExecuteNonQuery();
    }

    public void AdvSecAddBillableCommits(
      IEnumerable<GitAdvSecBillableCommitsToAdd> billableCommitsToAdd)
    {
      this.EnforceMinimalVersion(2170);
      this.PrepareStoredProcedure("prc_AdvSecAddBillableCommits");
      this.BindAdvSecBillableCommitsToAddTable("@billableCommitsToAdd", billableCommitsToAdd);
      this.ExecuteNonQuery();
    }

    public void AdvSecBillableCommitsBackfill(Guid repositoryId)
    {
      this.EnforceMinimalVersion(2170);
      this.PrepareStoredProcedure("prc_AdvSecBackfillBillableCommits", 3600);
      this.BindGuid("@repositoryId", repositoryId);
      this.ExecuteNonQuery();
    }

    public void AdvSecDeleteEnablementData(
      bool allProjects,
      bool includeBillableCommitters,
      IEnumerable<Guid> projectIds)
    {
      this.EnforceMinimalVersion(2190);
      this.PrepareStoredProcedure("prc_AdvSecDeleteEnablementData");
      this.BindBoolean("@allProjects", allProjects);
      this.BindBoolean("@includeBillableCommitters", includeBillableCommitters);
      this.BindGuidTable("@projectIds", projectIds);
      this.ExecuteNonQuery();
    }

    public void AdvSecDeleteRepositoryEnablementData(
      Guid projectId,
      Guid repositoryId,
      bool includeBillableCommitters)
    {
      if (this.Version < 2251)
        return;
      this.PrepareStoredProcedure("prc_AdvSecDeleteRepositoryEnablementData");
      this.BindGuid("@projectId", projectId);
      this.BindGuid("@repositoryId", repositoryId);
      this.BindBoolean("@includeBillableCommitters", includeBillableCommitters);
      this.ExecuteNonQuery();
    }

    public List<GitBillableCommitter> AdvSecEstimateBillableCommitters(
      Guid projectId,
      Guid? repositoryId)
    {
      this.EnforceMinimalVersion(2170);
      this.PrepareStoredProcedure("prc_AdvSecQueryEstimateBillableCommitters");
      this.BindGuid("@projectId", projectId);
      if (repositoryId.HasValue)
        this.BindGuid("@repositoryId", repositoryId.Value);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitBillableCommitter>((ObjectBinder<GitBillableCommitter>) new GitCoreComponent.GitAdvSecBillableCommitterBinder());
        return resultCollection.GetCurrent<GitBillableCommitter>().Items;
      }
    }

    public List<GitBillablePusher> AdvSecEstimateBillablePushers(Guid? projectId)
    {
      this.EnforceMinimalVersion(2260);
      this.PrepareStoredProcedure("prc_AdvSecQueryEstimateBillablePushers");
      if (projectId.HasValue)
        this.BindGuid("@projectId", projectId.Value);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitBillablePusher>((ObjectBinder<GitBillablePusher>) new GitCoreComponent.GitAdvSecBillablePusherBinder());
        return resultCollection.GetCurrent<GitBillablePusher>().Items;
      }
    }

    public List<GitBillableCommitter> AdvSecQueryBillableCommitters(
      string teamProjectUri,
      DateTime? billingDate,
      int? skip,
      int? take)
    {
      this.EnforceMinimalVersion(2170);
      this.PrepareStoredProcedure("prc_AdvSecQueryBillableCommittersByProject");
      this.BindGuid("@projectId", ProjectInfo.GetProjectId(teamProjectUri));
      if (billingDate.HasValue)
        this.BindDateTime("@billingDate", billingDate.Value.ToUniversalTime(), true);
      if (skip.HasValue)
        this.BindInt("@skip", skip.Value);
      if (take.HasValue)
        this.BindInt("@take", take.Value);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitBillableCommitter>((ObjectBinder<GitBillableCommitter>) new GitCoreComponent.GitAdvSecBillableCommitterBinder());
        return resultCollection.GetCurrent<GitBillableCommitter>().Items;
      }
    }

    public List<GitBillableCommitterDetail> AdvSecQueryBillableCommittersDetailed(
      string teamProjectUri,
      DateTime? billingDate)
    {
      this.PrepareStoredProcedure("prc_AdvSecQueryBillableCommittersByProjectDetailed");
      this.BindGuid("@projectId", ProjectInfo.GetProjectId(teamProjectUri));
      if (billingDate.HasValue)
        this.BindDateTime("@billingDate", billingDate.Value.ToUniversalTime(), true);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        if (this.Version < 2290)
          resultCollection.AddBinder<GitBillableCommitterDetail>((ObjectBinder<GitBillableCommitterDetail>) new GitCoreComponent.GitAdvSecBillableCommitterDetailBinder());
        else
          resultCollection.AddBinder<GitBillableCommitterDetail>((ObjectBinder<GitBillableCommitterDetail>) new GitCoreComponent.GitAdvSecBillableCommitterDetailBinder2());
        return resultCollection.GetCurrent<GitBillableCommitterDetail>().Items;
      }
    }

    public List<GitAdvSecEnablementStatus> AdvSecQueryEnablementStatus(
      IEnumerable<Guid> projectIds,
      DateTime? billingDate,
      bool includeDeleted,
      int? skip,
      int? take)
    {
      if (this.Version >= 2260 && includeDeleted)
        this.PrepareStoredProcedure("prc_AdvSecQueryEnablementStatusIncludeDeleted");
      else
        this.PrepareStoredProcedure("prc_AdvSecQueryEnablementStatus");
      this.BindGuidTable("@projectIds", projectIds);
      if (billingDate.HasValue)
        this.BindDateTime("@billingDate", billingDate.Value.ToUniversalTime(), true);
      if (skip.HasValue)
        this.BindInt("@skip", skip.Value);
      if (take.HasValue)
        this.BindInt("@take", take.Value);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        if (this.Version < 2200)
          resultCollection.AddBinder<GitAdvSecEnablementStatus>((ObjectBinder<GitAdvSecEnablementStatus>) new GitCoreComponent.GitAdvSecEnablementStatusBinder());
        else
          resultCollection.AddBinder<GitAdvSecEnablementStatus>((ObjectBinder<GitAdvSecEnablementStatus>) new GitCoreComponent.GitAdvSecEnablementStatusBinder2());
        return resultCollection.GetCurrent<GitAdvSecEnablementStatus>().Items;
      }
    }

    public bool AdvSecQueryEnablementStatusForRepository(
      string teamProjectUri,
      Guid repositoryId,
      DateTime? billingDate)
    {
      if (this.Version < 2170)
        return false;
      this.PrepareStoredProcedure("prc_AdvSecQueryEnablementStatusForRepository");
      this.BindGuid("@projectId", ProjectInfo.GetProjectId(teamProjectUri));
      this.BindGuid("@repositoryId", repositoryId);
      if (billingDate.HasValue)
        this.BindDateTime("@billingDate", billingDate.Value.ToUniversalTime(), true);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder enabledColumn = new SqlColumnBinder("Enabled");
        resultCollection.AddBinder<bool>((ObjectBinder<bool>) new SimpleObjectBinder<bool>((System.Func<IDataReader, bool>) (reader => enabledColumn.GetBoolean(reader))));
        return resultCollection.GetCurrent<bool>().First<bool>();
      }
    }

    public List<TfsGitProcessedCommit> AdvSecQueryLastCommitForPluginWatermark(
      RepoKey repoKey,
      Guid jobId,
      int take)
    {
      if (this.Version < 2180)
      {
        List<TfsGitProcessedCommit> source = this.QueryUnprocessedCommits(repoKey, jobId, take);
        if (!source.Any<TfsGitProcessedCommit>())
          return source;
        return new List<TfsGitProcessedCommit>()
        {
          source.Last<TfsGitProcessedCommit>()
        };
      }
      this.PrepareStoredProcedure("prc_AdvSecQueryLastCommitForPluginWatermark");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@jobId", jobId);
      this.BindInt("@take", take);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TfsGitProcessedCommit>((ObjectBinder<TfsGitProcessedCommit>) new GitCoreComponent.ProcessedCommitBinder2());
        return resultCollection.GetCurrent<TfsGitProcessedCommit>().Items;
      }
    }

    public List<GitAdvSecProjectStats> AdvSecQueryProjectStatsForHost()
    {
      if (this.Version <= 2290)
        return new List<GitAdvSecProjectStats>();
      this.PrepareStoredProcedure("prc_AdvSecQueryProjectStatsForHost");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitAdvSecProjectStats>((ObjectBinder<GitAdvSecProjectStats>) new GitCoreComponent.GitAdvSecProjectStatsBinder());
        return resultCollection.GetCurrent<GitAdvSecProjectStats>().Items;
      }
    }

    public void AdvSecUpdateEnablementStatus(
      Guid changedBy,
      IEnumerable<GitAdvSecEnablementUpdate> updates)
    {
      this.EnforceMinimalVersion(2170);
      this.PrepareStoredProcedure("prc_AdvSecUpdateEnablementStatus");
      if (this.Version >= 2200)
        this.BindGuid("@changedBy", changedBy);
      this.BindAdvSecEnablementTable("@updates", updates);
      this.ExecuteNonQuery();
    }

    public bool AdvSecInitializePermissionsForOrganization()
    {
      this.EnforceMinimalVersion(2240);
      this.PrepareStoredProcedure("prc_AdvSecInitializePermissionsForOrg");
      this.ExecuteNonQuery();
      return this.Version >= 2271;
    }

    public bool AdvSecInitializePermissionsForProject(Guid project)
    {
      this.EnforceMinimalVersion(2180);
      this.PrepareStoredProcedure("prc_AdvSecInitializePermissions");
      this.BindDataspaceForProject(project);
      this.ExecuteNonQuery();
      return this.Version >= 2271;
    }

    public bool AdvSecInitializePermissionsForRepo(Guid project, Guid repository)
    {
      this.EnforceMinimalVersion(2210);
      this.PrepareStoredProcedure("prc_AdvSecInitializePermissionsForRepo");
      this.BindDataspaceForProject(project);
      this.BindString("@repoId", repository.ToString(), 36, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
      return this.Version >= 2271;
    }

    public bool AdvSecClearPermissionsForOrganization()
    {
      this.EnforceMinimalVersion(2240);
      this.PrepareStoredProcedure("prc_AdvSecClearPermissionsForOrg");
      this.ExecuteNonQuery();
      return this.Version >= 2271;
    }

    public bool AdvSecClearPermissionsForProject(Guid project)
    {
      this.EnforceMinimalVersion(2190);
      this.PrepareStoredProcedure("prc_AdvSecClearPermissions");
      this.BindDataspaceForProject(project);
      this.ExecuteNonQuery();
      return this.Version >= 2271;
    }

    public bool AdvSecClearPermissionsForRepo(Guid project, Guid repository)
    {
      this.EnforceMinimalVersion(2210);
      this.PrepareStoredProcedure("prc_AdvSecClearPermissionsForRepo");
      this.BindDataspaceForProject(project);
      this.BindString("@repoId", repository.ToString(), 36, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
      return this.Version >= 2271;
    }

    public int CopyRefFavorites(RepoKey sourceRepoKey, RepoKey targetRepoKey)
    {
      this.EnforceMinimalVersion(1610);
      this.PrepareStoredProcedure("prc_CopyGitRefFavorites");
      this.BindDataspace((RepoScope) sourceRepoKey);
      this.BindGuid("@sourceRepoId", sourceRepoKey.RepoId);
      this.BindGuid("@targetRepoId", targetRepoKey.RepoId);
      return (int) this.ExecuteScalar();
    }

    public LfsLock CreateLfsLock(RepoKey repoKey, Guid ownerId, string path)
    {
      path = FormattableString.Invariant(FormattableStringFactory.Create("/{0}/", (object) path.Trim('/')));
      this.PrepareStoredProcedure("prc_CreateGitLfsLock");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindGuid("@ownerId", ownerId);
      this.BindString("@path", path, 1024, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<LfsLock>((ObjectBinder<LfsLock>) new GitCoreComponent.LfsLockBinder());
        return resultCollection.GetCurrent<LfsLock>().Items.FirstOrDefault<LfsLock>();
      }
    }

    public int CreateRefFavorite(Guid identityId, RepoKey repoKey, string name, bool isFolder)
    {
      this.PrepareStoredProcedure("prc_CreateGitRefFavorite");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@identityId", identityId);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindString("@name", name, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
      this.BindBoolean("@isFolder", isFolder);
      return (int) this.ExecuteScalar();
    }

    public void CreateRepository(
      RepoKey toCreate,
      string repositoryName,
      int maxRepositoriesPerTeamProject,
      bool isHidden,
      bool createdByForking,
      Guid containerId)
    {
      if (containerId == toCreate.RepoId)
        throw new ArgumentException("containerId cannot be the same as RepoId");
      this.PrepareStoredProcedure("prc_CreateGitRepository");
      this.BindGuid("@repositoryId", toCreate.RepoId);
      this.BindDataspace((RepoScope) toCreate);
      this.BindString("@repositoryName", repositoryName, 400, false, SqlDbType.NVarChar);
      this.BindInt("@maxRepositoryCount", maxRepositoriesPerTeamProject);
      this.BindBoolean("@isHidden", isHidden);
      this.BindBoolean("@createdByForking", createdByForking);
      this.BindGuid("@containerId", containerId);
      this.ExecuteNonQuery();
    }

    public LfsLock DeleteLfsLock(RepoKey repoKey, int lockId, Guid deleterId, bool forceDelete)
    {
      this.PrepareStoredProcedure("prc_DeleteGitLfsLock");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindInt("@lockId", lockId);
      this.BindGuid("@deleterId", deleterId);
      this.BindBoolean("@forceDelete", forceDelete);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<LfsLock>((ObjectBinder<LfsLock>) new GitCoreComponent.LfsLockBinder());
        return resultCollection.GetCurrent<LfsLock>().Items.FirstOrDefault<LfsLock>();
      }
    }

    public void DeleteOldMetrics(int bucketsToRetain)
    {
      this.PrepareStoredProcedure("prc_DeleteOldGitMetrics");
      this.BindInt("@bucketsToRetain", bucketsToRetain);
      this.ExecuteNonQuery();
    }

    public void DeleteRefFavorite(Guid projectId, int favoriteId)
    {
      this.PrepareStoredProcedure("prc_DeleteGitRefFavorite");
      this.BindDataspaceForProject(projectId);
      this.BindInt("@favoriteId", favoriteId);
      this.ExecuteNonQuery();
    }

    public ICollection<TfsGitRepositoryInfo> DestroyRepositories(
      RepoScope toDestory,
      Guid deleterId)
    {
      this.PrepareStoredProcedure("prc_DestroyGitRepositories", 0);
      if (!this.TryBindDataspace(toDestory))
        return (ICollection<TfsGitRepositoryInfo>) Array.Empty<TfsGitRepositoryInfo>();
      this.BindNullableGuid("@repositoryId", toDestory.RepoId);
      this.BindGuid("@deleterId", deleterId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TfsGitRepositoryInfo>((ObjectBinder<TfsGitRepositoryInfo>) new GitCoreComponent.DeletedTfsGitRepositoryInfoBinder(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
        return (ICollection<TfsGitRepositoryInfo>) resultCollection.GetCurrent<TfsGitRepositoryInfo>().Items;
      }
    }

    public List<Sha1Id> GetCommitsMissingPushIds(RepoKey repoKey, IEnumerable<Sha1Id> commitIds)
    {
      this.PrepareStoredProcedure("prc_GetCommitsMissingPushIds");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindObjectIdTable("@commitIds", commitIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder commitIdBinder = new SqlColumnBinder("CommitId");
        resultCollection.AddBinder<Sha1Id>((ObjectBinder<Sha1Id>) new SimpleObjectBinder<Sha1Id>((System.Func<IDataReader, Sha1Id>) (reader => commitIdBinder.GetSha1Id(reader))));
        return resultCollection.GetCurrent<Sha1Id>().Items;
      }
    }

    public GitActivityMetrics GetGitProjectMetrics(
      Guid projectId,
      int startingTimeBucket,
      int endTimeBucket)
    {
      this.PrepareStoredProcedure("prc_GetGitProjectMetrics");
      this.BindDataspaceForProject(projectId);
      this.BindInt("@startingTimeBucket", startingTimeBucket);
      this.BindInt("@endTimeBucket", endTimeBucket);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return this.GetProjectRepositoryMetricsFromResultCollection(resultCollection, new RepoScope(projectId, Guid.Empty));
    }

    public GitActivityMetrics GetGitRepositoryMetrics(
      RepoKey repoKey,
      int startingTimeBucket,
      int endTimeBucket)
    {
      this.PrepareStoredProcedure("prc_GetGitRepositoryMetrics");
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindDataspace((RepoScope) repoKey);
      this.BindInt("@startingTimeBucket", startingTimeBucket);
      this.BindInt("@endTimeBucket", endTimeBucket);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return this.GetProjectRepositoryMetricsFromResultCollection(resultCollection, (RepoScope) repoKey);
    }

    public List<GitActivityMetrics> GetGitRepositoriesMetrics(
      Guid projectId,
      int startingTimeBucket,
      int endTimeBucket,
      int skip,
      int take)
    {
      this.PrepareStoredProcedure("prc_GetGitRepositoriesMetrics");
      this.BindDataspaceForProject(projectId);
      this.BindInt("@startingTimeBucket", startingTimeBucket);
      this.BindInt("@endTimeBucket", endTimeBucket);
      this.BindInt("@skip", skip);
      this.BindInt("@take", take);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder repositoryIdColumn = new SqlColumnBinder("RepositoryId");
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SimpleObjectBinder<Guid>((System.Func<IDataReader, Guid>) (reader => repositoryIdColumn.GetGuid(reader))));
        resultCollection.AddBinder<RepositoryMetricsTrendItem>((ObjectBinder<RepositoryMetricsTrendItem>) new GitCoreComponent.GitRepositoryMetricsTrendBinder());
        resultCollection.AddBinder<GitAuthorsCountInfo>((ObjectBinder<GitAuthorsCountInfo>) new GitCoreComponent.GitAuthorsCountBinder());
        Dictionary<Guid, List<MetricsTrendItem>> trendsByRepositoryId = new Dictionary<Guid, List<MetricsTrendItem>>();
        Dictionary<Guid, int> authorsCountByRepositoryId = new Dictionary<Guid, int>();
        List<Guid> items = resultCollection.GetCurrent<Guid>().Items;
        items.ForEach((Action<Guid>) (id =>
        {
          trendsByRepositoryId.Add(id, new List<MetricsTrendItem>());
          authorsCountByRepositoryId.Add(id, 0);
        }));
        resultCollection.NextResult();
        resultCollection.GetCurrent<RepositoryMetricsTrendItem>().Items.ForEach((Action<RepositoryMetricsTrendItem>) (metric => trendsByRepositoryId[metric.Id].Add((MetricsTrendItem) metric)));
        resultCollection.NextResult();
        resultCollection.GetCurrent<GitAuthorsCountInfo>().Items.ForEach((Action<GitAuthorsCountInfo>) (authorsCountInfo => authorsCountByRepositoryId[authorsCountInfo.Id] = authorsCountInfo.Count));
        List<GitActivityMetrics> repositoriesMetrics = new List<GitActivityMetrics>();
        foreach (Guid guid in items)
          repositoriesMetrics.Add(this.ToProjectMetrics(trendsByRepositoryId[guid], authorsCountByRepositoryId[guid], (RepoScope) new RepoKey(projectId, guid)));
        return repositoriesMetrics;
      }
    }

    public LfsLock GetLfsLockById(RepoKey repoKey, int lockId)
    {
      this.PrepareStoredProcedure("prc_GetGitLfsLockById");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindInt("@lockId", lockId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<LfsLock>((ObjectBinder<LfsLock>) new GitCoreComponent.LfsLockBinder());
        return resultCollection.GetCurrent<LfsLock>().Items.FirstOrDefault<LfsLock>();
      }
    }

    public IReadOnlyList<LfsLock> GetLfsLocks(RepoKey repoKey, int cursor, int limit, string path)
    {
      if (path != null)
        path = FormattableString.Invariant(FormattableStringFactory.Create("/{0}/", (object) path.Trim('/')));
      this.PrepareStoredProcedure("prc_GetGitLfsLocks");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindInt("@cursor", cursor);
      this.BindInt("@limit", limit);
      this.BindString("@path", path, 1024, true, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<LfsLock>((ObjectBinder<LfsLock>) new GitCoreComponent.LfsLockBinder());
        return (IReadOnlyList<LfsLock>) resultCollection.GetCurrent<LfsLock>().Items.AsReadOnly();
      }
    }

    public GitActiveRepoInfo GetMostActiveRepo(
      Guid projectId,
      GitMetrics evaluationMetric,
      int timePeriods)
    {
      this.PrepareStoredProcedure("prc_GetGitActiveRepo");
      this.BindDataspaceForProject(projectId);
      this.BindInt("@evaluationMetric", Convert.ToInt32((object) evaluationMetric));
      this.BindInt("@timePeriods", timePeriods);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitActiveRepoInfo>((ObjectBinder<GitActiveRepoInfo>) new GitCoreComponent.GitActiveRepoBinder());
        return resultCollection.GetCurrent<GitActiveRepoInfo>().Items.FirstOrDefault<GitActiveRepoInfo>();
      }
    }

    public List<TfsGitRefLogEntry> GetNextRefLogEntries(
      RepoKey repoKey,
      string refName,
      int? afterPushId,
      int? take)
    {
      this.PrepareStoredProcedure("prc_QueryGitRefLogAfterPushId");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindString("@refName", refName, GitConstants.MaxGitRefNameLength, false, SqlDbType.NChar);
      if (afterPushId.HasValue)
        this.BindInt("@afterPushId", afterPushId.Value);
      if (take.HasValue)
        this.BindInt("@take", take.Value);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TfsGitRefLogEntry>((ObjectBinder<TfsGitRefLogEntry>) new GitCoreComponent.TfsGitRefLogBinder2());
        return resultCollection.GetCurrent<TfsGitRefLogEntry>().Items;
      }
    }

    public (HashSet<Sha1Id> haves, HashSet<Sha1Id> wants) GetPrefetchHavesWants(
      RepoKey repoKey,
      DateTime timestamp)
    {
      this.PrepareStoredProcedure("prc_GetPrefetchHavesWants");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindDateTime2("@timestamp", timestamp);
      HashSet<Sha1Id> collection1 = new HashSet<Sha1Id>();
      HashSet<Sha1Id> collection2 = new HashSet<Sha1Id>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder haveBinder = new SqlColumnBinder("Have");
        SqlColumnBinder wantBinder = new SqlColumnBinder("Want");
        resultCollection.AddBinder<Sha1Id>((ObjectBinder<Sha1Id>) new SimpleObjectBinder<Sha1Id>((System.Func<IDataReader, Sha1Id>) (reader => haveBinder.GetSha1Id(reader))));
        resultCollection.AddBinder<Sha1Id>((ObjectBinder<Sha1Id>) new SimpleObjectBinder<Sha1Id>((System.Func<IDataReader, Sha1Id>) (reader => wantBinder.GetSha1Id(reader))));
        collection1.AddRange<Sha1Id, HashSet<Sha1Id>>((IEnumerable<Sha1Id>) resultCollection.GetCurrent<Sha1Id>());
        resultCollection.NextResult();
        collection2.AddRange<Sha1Id, HashSet<Sha1Id>>((IEnumerable<Sha1Id>) resultCollection.GetCurrent<Sha1Id>());
      }
      return (collection1, collection2);
    }

    public List<Sha1Id> GetPushCommitsByPushId(RepoKey repoKey, int pushId, int? skip, int? take)
    {
      this.PrepareStoredProcedure("prc_GetPushCommitsByPushId");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindInt("@pushId", pushId);
      if (skip.HasValue)
        this.BindInt("@skip", skip.Value);
      if (take.HasValue)
        this.BindInt("@take", take.Value);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder commitIdBinder = new SqlColumnBinder("CommitId");
        resultCollection.AddBinder<Sha1Id>((ObjectBinder<Sha1Id>) new SimpleObjectBinder<Sha1Id>((System.Func<IDataReader, Sha1Id>) (reader => commitIdBinder.GetSha1Id(reader))));
        return resultCollection.GetCurrent<Sha1Id>().Items;
      }
    }

    public ResultCollection GetPushDataForPushIds(
      RepoKey repoKey,
      int[] pushIds,
      bool includeRefUpdates)
    {
      this.PrepareStoredProcedure("prc_GetPushDataForPushIds");
      this.BindDataspace((RepoScope) repoKey);
      this.BindInt32Table("@pushIds", (IEnumerable<int>) pushIds);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindBoolean("@includeRefUpdates", includeRefUpdates);
      ResultCollection pushDataForPushIds = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      pushDataForPushIds.AddBinder<TfsGitPushMetadata>((ObjectBinder<TfsGitPushMetadata>) new GitCoreComponent.GitPushMetadataBinder());
      if (includeRefUpdates)
        pushDataForPushIds.AddBinder<TfsGitRefLogEntry>((ObjectBinder<TfsGitRefLogEntry>) new GitCoreComponent.TfsGitRefLogBinder2());
      return pushDataForPushIds;
    }

    public Dictionary<Sha1Id, int> GetPushIdsByCommitIds(
      RepoKey repoKey,
      IEnumerable<Sha1Id> commitIds)
    {
      this.PrepareStoredProcedure("prc_GetPushIdsByCommitIds");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindObjectIdTable("@commitIds", commitIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder commitIdBinder = new SqlColumnBinder("CommitId");
        SqlColumnBinder pushIdBinder = new SqlColumnBinder("PushId");
        resultCollection.AddBinder<KeyValuePair<Sha1Id, int>>((ObjectBinder<KeyValuePair<Sha1Id, int>>) new SimpleObjectBinder<KeyValuePair<Sha1Id, int>>((System.Func<IDataReader, KeyValuePair<Sha1Id, int>>) (reader => new KeyValuePair<Sha1Id, int>(commitIdBinder.GetSha1Id(reader), pushIdBinder.GetInt32(reader)))));
        return resultCollection.GetCurrent<KeyValuePair<Sha1Id, int>>().ToDictionary<KeyValuePair<Sha1Id, int>, Sha1Id, int>((System.Func<KeyValuePair<Sha1Id, int>, Sha1Id>) (kvp => kvp.Key), (System.Func<KeyValuePair<Sha1Id, int>, int>) (kvp => kvp.Value));
      }
    }

    public TfsGitRefFavorite GetRefFavorite(Guid projectId, int favoriteId)
    {
      this.PrepareStoredProcedure("prc_GetGitRefFavoriteById");
      this.BindDataspaceForProject(projectId);
      this.BindInt("@favoriteId", favoriteId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TfsGitRefFavorite>((ObjectBinder<TfsGitRefFavorite>) new GitCoreComponent.TfsGitRefFavoriteBinder2(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return resultCollection.GetCurrent<TfsGitRefFavorite>().Items.FirstOrDefault<TfsGitRefFavorite>();
    }

    public List<TfsGitRefFavorite> GetRefFavorites(Guid identityId, RepoKey repoKey)
    {
      this.PrepareStoredProcedure("prc_GetGitRefFavorites");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@identityId", identityId);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TfsGitRefFavorite>((ObjectBinder<TfsGitRefFavorite>) new GitCoreComponent.TfsGitRefFavoriteBinder2(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return resultCollection.GetCurrent<TfsGitRefFavorite>().Items;
    }

    public List<TfsGitRefFavorite> GetRefFavoritesForProject(Guid projectId, Guid identityId)
    {
      this.EnforceMinimalVersion(2290);
      this.PrepareStoredProcedure("prc_GetGitRefFavoritesForProject");
      this.BindDataspaceForProject(projectId);
      this.BindGuid("@identityId", identityId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TfsGitRefFavorite>((ObjectBinder<TfsGitRefFavorite>) new GitCoreComponent.TfsGitRefFavoriteBinder2(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      return resultCollection.GetCurrent<TfsGitRefFavorite>().Items;
    }

    public RepoStats GetRepositoryStats(RepoKey repoKey)
    {
      this.PrepareStoredProcedure("prc_GetRepositoryStats");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<RepoStats>((ObjectBinder<RepoStats>) new GitCoreComponent.TfsGitRepositoryStatsBinder(repoKey));
      return resultCollection.GetCurrent<RepoStats>().Items.Single<RepoStats>();
    }

    public void LockRef(RepoKey repoKey, string refName, Guid lockerId)
    {
      this.PrepareStoredProcedure("prc_LockGitRef");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindString("@refName", refName, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
      this.BindGuid("@lockerId", lockerId);
      this.ExecuteNonQuery();
    }

    public List<GitCommitScanData> QueryAdvSecMissingCommits(
      Guid objectDbId,
      Guid repositoryId,
      int toolId,
      int versionId,
      int? take)
    {
      if (this.Version < 2211)
        return new List<GitCommitScanData>(0);
      this.PrepareStoredProcedure("prc_AdvSecQueryMissingCommits");
      this.BindInt("@dataspaceId", this.GetDataspaceId(objectDbId, "GitOdb"));
      if (this.Version >= 2250)
        this.BindGuid("@repositoryId", repositoryId);
      this.BindInt("@toolId", toolId);
      this.BindInt("@versionId", versionId);
      if (take.HasValue)
        this.BindInt("@take", take.Value);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitCommitScanData>((ObjectBinder<GitCommitScanData>) new GitCoreComponent.GitCommitScanDataBinder());
        return resultCollection.GetCurrent<GitCommitScanData>().Items;
      }
    }

    public List<GitCommitScanStatusData> QueryAdvSecScannedCommitsStatus(
      Guid objectDbId,
      Guid repositoryId,
      int toolId,
      int versionId,
      List<Sha1Id> commits)
    {
      if (this.Version < 2250)
        return new List<GitCommitScanStatusData>();
      this.PrepareStoredProcedure("prc_AdvSecQueryScannedCommitsStatus");
      this.BindInt("@dataspaceId", this.GetDataspaceId(objectDbId, "GitOdb"));
      this.BindGuid("@repositoryId", repositoryId);
      this.BindInt("@toolId", toolId);
      this.BindInt("@versionId", versionId);
      this.BindObjectIdTable("@commitIds", (IEnumerable<Sha1Id>) commits);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitCommitScanStatusData>((ObjectBinder<GitCommitScanStatusData>) new GitCoreComponent.GitCommitScanStatusDataBinder());
        return resultCollection.GetCurrent<GitCommitScanStatusData>().Items;
      }
    }

    public List<GitCommitScanData> QueryAdvSecUnscannedCommits(
      Guid objectDbId,
      Guid repositoryId,
      int toolId,
      int versionId,
      int? take,
      int? maxAttemptCount,
      out int totalUnscannedCommits,
      out int totalCommits)
    {
      this.EnforceMinimalVersion(2160);
      List<GitCommitScanData> gitCommitScanDataList = new List<GitCommitScanData>();
      this.PrepareStoredProcedure("prc_AdvSecQueryUnscannedCommits");
      this.BindInt("@dataspaceId", this.GetDataspaceId(objectDbId, "GitOdb"));
      if (this.Version >= 2250)
        this.BindGuid("@repositoryId", repositoryId);
      this.BindInt("@toolId", toolId);
      this.BindInt("@versionId", versionId);
      if (take.HasValue)
        this.BindInt("@take", take.Value);
      if (maxAttemptCount.HasValue)
        this.BindInt("@maxAttemptCount", maxAttemptCount.Value);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitCommitScanData>((ObjectBinder<GitCommitScanData>) new GitCoreComponent.GitCommitScanDataBinder());
        SqlColumnBinder totalUnscannedCommitsCol = new SqlColumnBinder("TotalUnscannedCommits");
        resultCollection.AddBinder<int?>((ObjectBinder<int?>) new SimpleObjectBinder<int?>((System.Func<IDataReader, int?>) (reader => totalUnscannedCommitsCol.GetNullableInt32(reader))));
        SqlColumnBinder totalCommitsCol = new SqlColumnBinder("TotalCommits");
        resultCollection.AddBinder<int?>((ObjectBinder<int?>) new SimpleObjectBinder<int?>((System.Func<IDataReader, int?>) (reader => totalCommitsCol.GetNullableInt32(reader))));
        List<GitCommitScanData> items = resultCollection.GetCurrent<GitCommitScanData>().Items;
        gitCommitScanDataList.AddRange((IEnumerable<GitCommitScanData>) items);
        resultCollection.NextResult();
        totalUnscannedCommits = resultCollection.GetCurrent<int?>().Single<int?>().GetValueOrDefault();
        resultCollection.NextResult();
        totalCommits = resultCollection.GetCurrent<int?>().Single<int?>().GetValueOrDefault();
        return gitCommitScanDataList;
      }
    }

    public void UpdateAdvSecScannedCommits(
      IEnumerable<GitCommitScanData> updates,
      Guid objectDbId,
      Guid repositoryId)
    {
      this.EnforceMinimalVersion(2160);
      this.PrepareStoredProcedure("prc_AdvSecUpdateScannedCommits");
      this.BindInt("@dataspaceId", this.GetDataspaceId(objectDbId, "GitOdb"));
      if (this.Version >= 2250)
        this.BindGuid("@repositoryId", repositoryId);
      if (this.Version < 2211)
        this.BindAdvSecCommitScanTable("@commitScanData", updates);
      else
        this.BindAdvSecCommitScanTable2("@commitScanData", updates);
      this.ExecuteNonQuery();
    }

    public void MarkProcessedCommits(
      RepoKey repoKey,
      Guid jobId,
      IEnumerable<UnprocessedCommit> commits)
    {
      this.PrepareStoredProcedure("prc_MarkProcessedCommits");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindGuid("@jobId", jobId);
      UnprocessedCommit unprocessedCommit = commits.Last<UnprocessedCommit>();
      this.BindInt("@lastPushId", unprocessedCommit.PushId);
      this.BindBinary("@lastCommitId", unprocessedCommit.ObjectId.ToByteArray(), 20, SqlDbType.Binary);
      this.ExecuteNonQuery();
    }

    public List<Sha1Id> QueryCherryPickRelationships(RepoKey repoKey, Sha1Id commitId)
    {
      this.PrepareStoredProcedure("prc_QueryCherryPickRelationships");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindBinary("@commitId", commitId.ToByteArray(), 20, SqlDbType.Binary);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder commitIdBinder = new SqlColumnBinder("CommitId");
        resultCollection.AddBinder<Sha1Id>((ObjectBinder<Sha1Id>) new SimpleObjectBinder<Sha1Id>((System.Func<IDataReader, Sha1Id>) (reader => commitIdBinder.GetSha1Id(reader))));
        return resultCollection.GetCurrent<Sha1Id>().Items;
      }
    }

    public IList<TfsGitDeletedRepositoryInfo> QueryDeletedRepositories(
      Guid? projectId,
      bool isSoftDeletedOnly)
    {
      this.PrepareStoredProcedure("prc_QueryGitDeletedRepositories");
      if (projectId.HasValue)
        this.BindDataspaceForProject(projectId.Value);
      this.BindBoolean("@isSoftDeletedOnly", isSoftDeletedOnly);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TfsGitDeletedRepositoryInfo>((ObjectBinder<TfsGitDeletedRepositoryInfo>) new GitCoreComponent.TfsGitDeletedRepositoryInfoBinder2(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
        return (IList<TfsGitDeletedRepositoryInfo>) resultCollection.GetCurrent<TfsGitDeletedRepositoryInfo>().Items;
      }
    }

    public List<TfsGitRef> QueryGitRefs(
      RepoKey repoKey,
      Guid userId,
      bool includeDefault,
      bool includeImportantRefs,
      bool includeUserCreated,
      bool includeUserFavorites,
      int breadcrumbDays,
      string breadcrumbsPrefix)
    {
      this.PrepareStoredProcedure("prc_QueryGitRefs");
      this.BindDataspace((RepoScope) repoKey);
      this.BindBoolean("@includeDefault", includeDefault);
      this.BindBoolean("@includeImportantRefs", includeImportantRefs);
      this.BindBoolean("@includeUserCreated", includeUserCreated);
      this.BindBoolean("@includeUserFavorites", includeUserFavorites);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindGuid("@userId", userId);
      this.BindString("@breadcrumbsPrefix", breadcrumbsPrefix, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
      this.BindInt("@breadcrumbDays", breadcrumbDays);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        this.AddGitRefBinder(rc);
        return rc.GetCurrent<TfsGitRef>().Items;
      }
    }

    public List<TfsGitRefWithResolvedCommit> QueryGitRefsBySearchCriteria(
      RepoKey repoKey,
      IEnumerable<string> refNames,
      IEnumerable<Sha1Id> commitIds,
      GitRefSearchType searchType = GitRefSearchType.Exact,
      string refArea = "heads")
    {
      refArea = this.EnsureEndsWithSlash(refArea);
      this.PrepareStoredProcedure("prc_ReadGitRefs");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindGitRefNameTable("@refNamesIn", refNames);
      this.BindByte("@searchType", (byte) searchType);
      this.BindObjectIdTable("@commitIds", commitIds);
      if (this.Version < 1420)
      {
        this.BindInt("@breadcrumbDays", 0);
        this.BindString("@breadcrumbsPrefix", string.Empty, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
      }
      this.BindString("@refArea", refArea, refArea.Length, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@firstRefName", (string) null, GitConstants.MaxGitRefNameLength, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindInt("@pageSize", int.MaxValue);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TfsGitRefWithResolvedCommit>((ObjectBinder<TfsGitRefWithResolvedCommit>) new GitCoreComponent.GitRefWithResolvedCommitBinder3());
        return resultCollection.GetCurrent<TfsGitRefWithResolvedCommit>().Items;
      }
    }

    public ResultCollection QueryPushHistory(
      RepoKey repoKey,
      string refName,
      bool includeRefUpdates,
      DateTime? fromDate,
      DateTime? toDate,
      IEnumerable<Guid> pusherIds,
      bool excludePushers,
      int? skip,
      int? take)
    {
      if (pusherIds == null || !pusherIds.Any<Guid>())
        this.PrepareStoredProcedure("prc_QueryPushHistory");
      else if (excludePushers)
      {
        if (string.IsNullOrEmpty(refName))
          throw new ArgumentException("Querying for all refs is not supported when excluding pushers");
        this.PrepareStoredProcedure("prc_QueryPushHistoryExcludingPushers");
        this.BindGuidTable("@pusherIds", pusherIds);
      }
      else
      {
        if (pusherIds.Count<Guid>() != 1)
          throw new ArgumentException("Filtering by multi pushers is not supported when including pushers.");
        this.PrepareStoredProcedure("prc_QueryPushHistory");
        this.BindGuid("@pusherId", pusherIds.First<Guid>());
      }
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindBoolean("@includeRefUpdates", includeRefUpdates);
      if (fromDate.HasValue)
        this.BindDateTime2("@fromDate", fromDate.Value);
      if (toDate.HasValue)
        this.BindDateTime2("@toDate", toDate.Value);
      if (!string.IsNullOrEmpty(refName))
        this.BindString("@refName", refName, GitConstants.MaxGitRefNameLength, false, SqlDbType.NChar);
      if (skip.HasValue)
        this.BindInt("@skip", skip.Value);
      if (take.HasValue)
        this.BindInt("@take", take.Value);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TfsGitPushMetadata>((ObjectBinder<TfsGitPushMetadata>) new GitCoreComponent.GitPushMetadataBinder());
      if (includeRefUpdates)
        resultCollection.AddBinder<TfsGitRefLogEntry>((ObjectBinder<TfsGitRefLogEntry>) new GitCoreComponent.TfsGitRefLogBinder2());
      return resultCollection;
    }

    public List<TfsGitRepositoryInfo> QueryRepositories(bool excludeHiddenRepos = false)
    {
      this.PrepareStoredProcedure("prc_QueryGitRepositories");
      this.BindBoolean("@excludeHiddenRepos", excludeHiddenRepos);
      this.BindBoolean("@includeSoftDeleted", this.RequestContext.IsServicingContext);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        this.AddGitRepositoryInfoBinder(rc);
        return rc.GetCurrent<TfsGitRepositoryInfo>().Items;
      }
    }

    public List<RawRepoInfo> QueryRepositoriesRaw()
    {
      this.PrepareStoredProcedure("prc_QueryGitRepositories");
      this.BindBoolean("@excludeHiddenRepos", false);
      this.BindBoolean("@includeSoftDeleted", true);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        if (this.Version < 1650)
          resultCollection.AddBinder<RawRepoInfo>((ObjectBinder<RawRepoInfo>) new GitCoreComponent.RawRepoInfoBinder());
        else
          resultCollection.AddBinder<RawRepoInfo>((ObjectBinder<RawRepoInfo>) new GitCoreComponent.RawRepoInfoBinder2());
        return resultCollection.GetCurrent<RawRepoInfo>().Items;
      }
    }

    public IReadOnlyList<TfsGitRepositoryInfo> QueryRepositoriesAcrossProjects(
      IEnumerable<Guid> repoIds)
    {
      this.PrepareStoredProcedure("prc_QueryGitRepositoriesByIds");
      this.BindGuidTable("@repoIds", repoIds);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        this.AddGitRepositoryInfoBinder(rc);
        return (IReadOnlyList<TfsGitRepositoryInfo>) rc.GetCurrent<TfsGitRepositoryInfo>().Items;
      }
    }

    public IReadOnlyList<TfsGitRepositoryInfo> QueryRepositories(
      Guid projectId,
      IEnumerable<Guid> repoIds)
    {
      this.PrepareStoredProcedure("prc_QueryGitRepositoriesByIds");
      this.BindDataspaceForProject(projectId);
      this.BindGuidTable("@repoIds", repoIds);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        this.AddGitRepositoryInfoBinder(rc);
        return (IReadOnlyList<TfsGitRepositoryInfo>) rc.GetCurrent<TfsGitRepositoryInfo>().Items;
      }
    }

    public List<TfsGitRepositoryInfo> QueryRepositoriesUpdatedSinceLastWatermark(
      DateTime updatedTime,
      int batchSize,
      bool includeDisabled = false)
    {
      if (this.Version < 1650)
        return new List<TfsGitRepositoryInfo>();
      this.PrepareStoredProcedure("prc_GetGitRepositoriesUpdatedSinceWatermark");
      this.BindDateTime("@lastMetadataUpdate", updatedTime);
      this.BindInt("@batchSize", batchSize);
      if (this.Version >= 1830)
        this.BindBoolean("@includeDisabled", includeDisabled);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        this.AddGitRepositoryInfoBinder(rc);
        return rc.GetCurrent<TfsGitRepositoryInfo>().Items;
      }
    }

    public List<TfsGitProcessedCommit> QueryUnprocessedCommits(
      RepoKey repoKey,
      Guid jobId,
      int take)
    {
      this.PrepareStoredProcedure("prc_QueryUnprocessedCommits");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@jobId", jobId);
      this.BindInt("@take", take);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TfsGitProcessedCommit>((ObjectBinder<TfsGitProcessedCommit>) new GitCoreComponent.ProcessedCommitBinder2());
        return resultCollection.GetCurrent<TfsGitProcessedCommit>().Items;
      }
    }

    public TfsGitLimitedRefCriteria ReadLimitedRefCriteria(RepoKey repoKey)
    {
      this.PrepareStoredProcedure("prc_ReadGitLimitedRefCriteria");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder refTemplateColumn = new SqlColumnBinder("RefTemplate");
        resultCollection.AddBinder<string>((ObjectBinder<string>) new SimpleObjectBinder<string>((System.Func<IDataReader, string>) (reader => refTemplateColumn.GetString(reader, false))));
        return new TfsGitLimitedRefCriteria(resultCollection.GetCurrent<string>().Items.Where<string>((System.Func<string, bool>) (t => !t.EndsWith("%", StringComparison.Ordinal))).ToList<string>(), resultCollection.GetCurrent<string>().Items.Where<string>((System.Func<string, bool>) (t => t.EndsWith("%", StringComparison.Ordinal))).Select<string, string>((System.Func<string, string>) (n => n.Substring(0, n.Length - 1))).ToList<string>());
      }
    }

    public TfsGitRef ReadDefaultBranch(RepoKey repoKey)
    {
      this.PrepareStoredProcedure("prc_ReadGitDefaultBranch");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        this.AddGitRefBinder(rc);
        return rc.GetCurrent<TfsGitRef>().Items.FirstOrDefault<TfsGitRef>();
      }
    }

    public Guid ReadRefCreatorWithDefaultAce(RepoKey repoKey, string refName)
    {
      this.PrepareStoredProcedure("prc_ReadRefCreatorWithDefaultAce");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindString("@refName", refName, GitConstants.MaxGitRefNameLength, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      object obj = this.ExecuteScalar();
      switch (obj)
      {
        case null:
        case DBNull _:
          return Guid.Empty;
        default:
          return (Guid) obj;
      }
    }

    public string ReadAnyRefWithDefaultAceByCreator(RepoKey repoKey, Guid creatorId)
    {
      this.PrepareStoredProcedure("prc_ReadAnyRefWithDefaultAceByCreator");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindGuid("@creatorId", creatorId);
      object obj = this.ExecuteScalar();
      switch (obj)
      {
        case null:
        case DBNull _:
          return string.Empty;
        default:
          return (string) obj;
      }
    }

    public void SetAccessControlListForRefCreator(string token)
    {
      Guid? repoIdFromToken = GitPermissionsUtil.GetRepoIdFromToken(token);
      string refNameFromToken = GitPermissionsUtil.GetRefNameFromToken(token);
      Guid? projectIdFromToken = GitPermissionsUtil.GetProjectIdFromToken(token);
      this.PrepareStoredProcedure("prc_SetAccessControlListForRefCreator");
      this.BindNullableGuid("@repositoryId", repoIdFromToken);
      this.BindDataspaceForProject(projectIdFromToken.Value);
      this.BindString("@refName", refNameFromToken, GitConstants.MaxGitRefNameLength, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("@token", token, GitConstants.MaxGitRefNameLength, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@namespaceGuid", GitConstants.GitSecurityNamespaceId);
      this.ExecuteNonQuery();
    }

    public List<TfsGitRef> ReadRefs(
      RepoKey repoKey,
      IEnumerable<string> refNames,
      GitRefSearchType searchType,
      string refArea = "heads",
      string firstRefName = null,
      int? pageSize = null)
    {
      refArea = this.EnsureEndsWithSlash(refArea);
      this.PrepareStoredProcedure("prc_ReadGitRefs");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindGitRefNameTable("@refNamesIn", refNames);
      this.BindByte("@searchType", (byte) searchType);
      this.BindString("@refArea", refArea, GitConstants.MaxGitRefNameLength, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindObjectIdTable("@commitIds", (IEnumerable<Sha1Id>) Array.Empty<Sha1Id>());
      if (this.Version < 1420)
      {
        this.BindInt("@breadcrumbDays", 0);
        this.BindString("@breadcrumbsPrefix", string.Empty, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
      }
      this.BindString("@firstRefName", firstRefName, GitConstants.MaxGitRefNameLength, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindInt("@pageSize", pageSize ?? int.MaxValue);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        this.AddGitRefBinder(rc);
        return rc.GetCurrent<TfsGitRef>().Items;
      }
    }

    public ICollection<TfsGitRepositoryInfo> DeleteRepositories(RepoScope toDelete, Guid deleterId)
    {
      this.PrepareStoredProcedure("prc_DeleteGitRepositories");
      if (!this.TryBindDataspace(toDelete))
        return (ICollection<TfsGitRepositoryInfo>) Array.Empty<TfsGitRepositoryInfo>();
      this.BindNullableGuid("@repositoryId", toDelete.RepoId);
      this.BindGuid("@deleterId", deleterId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TfsGitRepositoryInfo>((ObjectBinder<TfsGitRepositoryInfo>) new GitCoreComponent.DeletedTfsGitRepositoryInfoBinder(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
        return (ICollection<TfsGitRepositoryInfo>) resultCollection.GetCurrent<TfsGitRepositoryInfo>().Items;
      }
    }

    public void UndeleteRepository(RepoKey toUndelete)
    {
      this.PrepareStoredProcedure("prc_UndeleteGitRepository");
      this.BindDataspace((RepoScope) toUndelete);
      this.BindGuid("@repositoryId", toUndelete.RepoId);
      this.ExecuteNonQuery();
    }

    public void DisableRepository(RepoScope toDisable)
    {
      this.PrepareStoredProcedure("prc_DisableGitRepository");
      this.BindDataspace(toDisable);
      this.BindGuid("@repositoryId", toDisable.RepoId);
      this.ExecuteNonQuery();
    }

    public void EnableRepository(RepoScope toEnable)
    {
      this.PrepareStoredProcedure("prc_EnableGitRepository");
      this.BindDataspace(toEnable);
      this.BindGuid("@repositoryId", toEnable.RepoId);
      this.ExecuteNonQuery();
    }

    public Guid? ReadPointer(RepoKey repoKey, RepoPointerType pointerType)
    {
      this.PrepareStoredProcedure("prc_ReadGitPointer");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindByte("@pointerType", (byte) pointerType);
      object obj = this.ExecuteScalar();
      switch (obj)
      {
        case null:
        case DBNull _:
          return new Guid?();
        default:
          return new Guid?((Guid) obj);
      }
    }

    public void UpdateRepository(
      RepoKey repoKey,
      string newRepositoryName,
      string newDefaultBranch)
    {
      this.PrepareStoredProcedure("prc_UpdateGitRepository");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindString("@newRepositoryName", newRepositoryName, GitConstants.MaxRepositoryNameLength, true, SqlDbType.NVarChar);
      this.BindString("@newDefaultBranch", newDefaultBranch, GitConstants.MaxGitRefNameLength, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public void UpdateRepoSize(RepoKey repoKey, long size)
    {
      this.PrepareStoredProcedure("prc_UpdateRepoSize");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindLong("@compressedSize", size);
      this.ExecuteNonQuery();
    }

    public RepoKey RepoKeyFromName(
      Guid projectId,
      string repositoryName,
      bool includeDisabled,
      out string canonicalName,
      out bool createdByForking,
      out long compressedSize,
      out bool disabled,
      out bool isInMaintenance)
    {
      this.PrepareStoredProcedure("prc_GitRepositoryIdFromName");
      this.BindDataspaceForProject(projectId);
      this.BindString("@repositoryName", repositoryName, 256, false, SqlDbType.NVarChar);
      if (this.Version >= 1831)
        this.BindBoolean("@includeDisabled", includeDisabled);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      sqlDataReader.Read();
      Guid guid1 = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("RepositoryId"));
      Guid guid2 = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("ContainerId"));
      canonicalName = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Name"));
      createdByForking = sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("CreatedByForking"));
      compressedSize = sqlDataReader.GetInt64(sqlDataReader.GetOrdinal("CompressedSize"));
      disabled = this.Version >= 1840 && sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("Disabled"));
      isInMaintenance = this.Version >= 2130 && sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("IsInMaintenance"));
      return new RepoKey(projectId, guid1, guid2);
    }

    public virtual string RepositoryNameFromId(
      Guid repositoryId,
      bool includeDisabled,
      out RepoKey repoKey,
      out bool createdByForking,
      out long compressedSize,
      out bool disabled,
      out bool isInMaintenance)
    {
      this.PrepareStoredProcedure("prc_GitRepositoryNameFromId");
      this.BindGuid("@repositoryId", repositoryId);
      this.BindBoolean("@includeSoftDeleted", this.RequestContext.IsServicingContext);
      if (this.Version >= 1830)
        this.BindBoolean("@includeDisabled", includeDisabled);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      sqlDataReader.Read();
      Guid guid = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("ContainerId"));
      createdByForking = sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("CreatedByForking"));
      compressedSize = sqlDataReader.GetInt64(sqlDataReader.GetOrdinal("CompressedSize"));
      disabled = this.Version >= 1830 && sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("Disabled"));
      isInMaintenance = this.Version >= 2130 && sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("IsInMaintenance"));
      Guid dataspaceIdentifier = this.GetDataspaceIdentifier(sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("DataspaceId")));
      repoKey = new RepoKey(dataspaceIdentifier, repositoryId, guid);
      return sqlDataReader.GetString(sqlDataReader.GetOrdinal("Name"));
    }

    public void SetDefaultBranch(RepoKey repoKey, string refName) => this.UpdateRepository(repoKey, (string) null, refName);

    public void UnlockRef(RepoKey repoKey, string refName, Guid unlockerId, bool skipIdCheck)
    {
      this.PrepareStoredProcedure("prc_UnlockGitRef");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindString("@refName", refName, GitConstants.MaxGitRefNameLength, false, SqlDbType.NVarChar);
      this.BindGuid("@unlockerId", unlockerId);
      this.BindBoolean("@skipIdCheck", skipIdCheck);
      this.ExecuteNonQuery();
    }

    public GetExistingBranchHintsResult GetExistingBranchHints(
      Guid projectId,
      string branchPattern,
      bool isDefault,
      int top)
    {
      this.PrepareStoredProcedure("prc_GetExistingBranchHints");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "Git"));
      this.BindString("@branchPattern", branchPattern, GitConstants.MaxGitRefNameLength, true, SqlDbType.NVarChar);
      this.BindBoolean("@isDefault", isDefault);
      this.BindInt("@top", top);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder repoCountColumn = new SqlColumnBinder("RepoCount");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => repoCountColumn.GetInt32(reader))));
        SqlColumnBinder branchCountColumn = new SqlColumnBinder("BranchCount");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => branchCountColumn.GetInt32(reader))));
        int num1 = resultCollection.GetCurrent<int>().First<int>();
        resultCollection.NextResult();
        int num2 = resultCollection.GetCurrent<int>().First<int>();
        resultCollection.NextResult();
        List<ExistingBranchHint> list;
        if (this.Version < 1600)
        {
          SqlColumnBinder repoNameColumn = new SqlColumnBinder("RepoName");
          resultCollection.AddBinder<string>((ObjectBinder<string>) new SimpleObjectBinder<string>((System.Func<IDataReader, string>) (reader => repoNameColumn.GetString(reader, false))));
          SqlColumnBinder branchNameColumn = new SqlColumnBinder("BranchName");
          resultCollection.AddBinder<string>((ObjectBinder<string>) new SimpleObjectBinder<string>((System.Func<IDataReader, string>) (reader => branchNameColumn.GetString(reader, false))));
          List<string> items1 = resultCollection.GetCurrent<string>().Items;
          resultCollection.NextResult();
          List<string> items2 = resultCollection.GetCurrent<string>().Items;
          list = items1.Zip<string, string, ExistingBranchHint>((IEnumerable<string>) items2, (Func<string, string, ExistingBranchHint>) ((r, b) => new ExistingBranchHint()
          {
            RepoName = r,
            BranchName = b
          })).ToList<ExistingBranchHint>();
        }
        else
        {
          resultCollection.AddBinder<Tuple<string, string>>((ObjectBinder<Tuple<string, string>>) new GitCoreComponent.TupleStringStringBinder("RepoName", "BranchName"));
          list = resultCollection.GetCurrent<Tuple<string, string>>().Items.Select<Tuple<string, string>, ExistingBranchHint>((System.Func<Tuple<string, string>, ExistingBranchHint>) (t => new ExistingBranchHint()
          {
            RepoName = t.Item1,
            BranchName = t.Item2
          })).ToList<ExistingBranchHint>();
        }
        return new GetExistingBranchHintsResult()
        {
          RepoCount = num1,
          BranchCount = num2,
          ExistingBranches = list
        };
      }
    }

    public List<(string Scope, Guid PolicyTypeId)> GetProjectPolicySummary(Guid projectId)
    {
      this.EnforceMinimalVersion(1590);
      this.PrepareStoredProcedure("prc_GetProjectPolicySummary");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<(string, Guid)>((ObjectBinder<(string, Guid)>) new GitCoreComponent.TupleStringGuidBinder("Scope", "PolicyType"));
        return resultCollection.GetCurrent<(string, Guid)>().Items;
      }
    }

    public void UpdateCommitIdForGitRefs(
      RepoKey repoKey,
      List<TfsGitRefUpdateWithResolvedCommit> gitRefs)
    {
      this.PrepareStoredProcedure("prc_UpdateCommitIdForGitRefs");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindRefUpdateTable("@refs", (IEnumerable<TfsGitRefUpdateWithResolvedCommit>) gitRefs);
      this.ExecuteNonQuery();
    }

    public void UpdateContainerIds(IEnumerable<Tuple<Guid, Guid>> repoContainerMaps)
    {
      this.PrepareStoredProcedure("prc_UpdateGitContainerIds");
      this.BindGuidGuidTable("@repoContainerMaps", repoContainerMaps);
      this.ExecuteNonQuery();
    }

    public void UpdateGitAuthorsLastUpdateTime(
      RepoKey repoKey,
      IEnumerable<string> authors,
      int timeBucket)
    {
      this.PrepareStoredProcedure("prc_UpdateGitAuthorsLastUpdateTime", 600);
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindStringTable("@authors", authors);
      this.BindInt("@timeBucket", timeBucket);
      this.ExecuteNonQuery();
    }

    public void UpdateGitMetrics(
      RepoKey repoKey,
      GitMetrics gitMetric,
      int incrementValue,
      int timeBucket)
    {
      this.PrepareStoredProcedure("prc_UpdateGitMetrics");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindByte("@metric", (byte) gitMetric);
      this.BindInt("@incrementValue", incrementValue);
      this.BindInt("@timeBucket", timeBucket);
      this.ExecuteNonQuery();
    }

    public void UpdateGitLimitedRefCriteria(
      RepoKey repoKey,
      IEnumerable<string> refExactMatch,
      IEnumerable<string> refNamespace)
    {
      this.PrepareStoredProcedure("prc_UpdateGitLimitedRefCriteria");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindStringTable("@refExactMatchIn", refExactMatch);
      this.BindStringTable("@refNamespaceIn", refNamespace);
      this.ExecuteNonQuery();
    }

    public Guid? UpdatePointer(
      RepoKey repoKey,
      RepoPointerType pointerType,
      Guid? oldPointerId,
      Guid? newPointerId)
    {
      this.PrepareStoredProcedure("prc_UpdateGitPointer");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindByte("@pointerType", (byte) pointerType);
      this.BindNullableGuid("@oldPointerId", oldPointerId);
      this.BindNullableGuid("@newPointerId", newPointerId);
      object obj = this.ExecuteScalar();
      switch (obj)
      {
        case null:
        case DBNull _:
          return new Guid?();
        default:
          return new Guid?((Guid) obj);
      }
    }

    public void SetGitRepoMaintenanceFlag(Guid odbId, bool state)
    {
      this.EnforceMinimalVersion(2130);
      this.PrepareStoredProcedure("prc_SetGitRepoMaintenanceFlag");
      this.BindGuid("@containerId", odbId);
      this.BindBoolean("@isInMaintenance", state);
      this.ExecuteNonQuery();
    }

    public List<TfsGitRefUpdateResult> WriteRefs(
      RepoKey repoKey,
      Guid pusherId,
      IEnumerable<TfsGitRefUpdateWithResolvedCommit> refUpdates,
      IEnumerable<Sha1Id> includedCommits,
      IEnumerable<Sha1Id> commitParents,
      string repoSecurityToken,
      bool enforceConsistentCase,
      IEnumerable<TfsGitRefUpdateRequest> updatesToFailIfRefsExist,
      out DateTime pushTime,
      out int? pushId,
      out List<Sha1Id> addCommitIds,
      DateTime? TEST_pushTime = null)
    {
      List<TfsGitRefUpdateResult> gitRefUpdateResultList = new List<TfsGitRefUpdateResult>();
      Dictionary<string, Sha1Id> dictionary = updatesToFailIfRefsExist.ToDictionary<TfsGitRefUpdateRequest, string, Sha1Id>((System.Func<TfsGitRefUpdateRequest, string>) (o => o.Name), (System.Func<TfsGitRefUpdateRequest, Sha1Id>) (o => o.NewObjectId));
      this.PrepareStoredProcedure("prc_WriteGitRefs", 3600);
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindGuid("@pusherId", pusherId);
      this.BindRefUpdateRequestTable("@refsIn", refUpdates, (IDictionary<string, Sha1Id>) dictionary);
      this.BindObjectIdTable("@commitsIn", includedCommits);
      this.BindObjectIdTable("@commitParentsIn", commitParents);
      this.BindGuid("@namespaceGuid", GitConstants.GitSecurityNamespaceId);
      this.BindString("@repoSecurityToken", repoSecurityToken, -1, true, SqlDbType.NVarChar);
      this.BindBoolean("@enforceConsistentCase", enforceConsistentCase);
      if (this.Version >= 1520)
        this.BindNullableDateTime2("@TEST_pushTime", TEST_pushTime);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TfsGitRefUpdateResult>((ObjectBinder<TfsGitRefUpdateResult>) new GitCoreComponent.TfsGitRefUpdateResultBinder4());
        SqlColumnBinder dateTimeCol = new SqlColumnBinder("DateTime");
        resultCollection.AddBinder<DateTime>((ObjectBinder<DateTime>) new SimpleObjectBinder<DateTime>((System.Func<IDataReader, DateTime>) (reader => dateTimeCol.GetDateTime(reader))));
        SqlColumnBinder pushIdCol = new SqlColumnBinder("PushId");
        resultCollection.AddBinder<int?>((ObjectBinder<int?>) new SimpleObjectBinder<int?>((System.Func<IDataReader, int?>) (reader => pushIdCol.GetNullableInt32(reader))));
        SqlColumnBinder commitIdCol = new SqlColumnBinder("CommitId");
        resultCollection.AddBinder<Sha1Id>((ObjectBinder<Sha1Id>) new SimpleObjectBinder<Sha1Id>((System.Func<IDataReader, Sha1Id>) (reader => commitIdCol.GetSha1Id(reader))));
        List<TfsGitRefUpdateResult> items = resultCollection.GetCurrent<TfsGitRefUpdateResult>().Items;
        gitRefUpdateResultList.AddRange((IEnumerable<TfsGitRefUpdateResult>) items);
        resultCollection.NextResult();
        pushTime = resultCollection.GetCurrent<DateTime>().Items.Single<DateTime>();
        resultCollection.NextResult();
        pushId = resultCollection.GetCurrent<int?>().Single<int?>();
        resultCollection.NextResult();
        addCommitIds = resultCollection.GetCurrent<Sha1Id>().ToList<Sha1Id>();
      }
      return gitRefUpdateResultList;
    }

    private bool TryBindDataspace(RepoScope scopeToTry)
    {
      int dataspaceId;
      if (!this.TryGetDataspaceId(scopeToTry.ProjectId, out dataspaceId))
        return false;
      this.BindInt("@dataspaceId", dataspaceId);
      return true;
    }

    private void BindDataspace(RepoScope scopeToBind) => this.BindDataspaceForProject(scopeToBind.ProjectId);

    private void BindDataspaceForProject(Guid projectId) => this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));

    private void EnforceMinimalVersion(int version)
    {
      if (this.Version < version)
        throw new ServiceVersionNotSupportedException(GitCoreComponent.ComponentFactory.ServiceName, this.Version, version);
    }

    private GitActivityMetrics GetProjectRepositoryMetricsFromResultCollection(
      ResultCollection resultCollection,
      RepoScope repoScope)
    {
      resultCollection.AddBinder<MetricsTrendItem>((ObjectBinder<MetricsTrendItem>) new GitCoreComponent.GitMetricsTrendBinder());
      SqlColumnBinder authorsCol = new SqlColumnBinder("Authors");
      resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => authorsCol.GetInt32(reader))));
      List<MetricsTrendItem> items = resultCollection.GetCurrent<MetricsTrendItem>().Items;
      resultCollection.NextResult();
      int authorsCount = resultCollection.GetCurrent<int>().Items[0];
      return this.ToProjectMetrics(items, authorsCount, repoScope);
    }

    private string EnsureEndsWithSlash(string name) => string.IsNullOrEmpty(name) || name[name.Length - 1] == '/' ? name : name + "/";

    private GitActivityMetrics ToProjectMetrics(
      List<MetricsTrendItem> metricsTrendList,
      int authorsCount,
      RepoScope repoScope)
    {
      List<CommitsTrendItem> commitsTrend = new List<CommitsTrendItem>();
      int commitsPushedCount = 0;
      int pullRequestsCompletedCount = 0;
      int pullRequestsCreatedCount = 0;
      foreach (MetricsTrendItem metricsTrend in metricsTrendList)
      {
        GitMetrics metricType = metricsTrend.MetricType;
        if (metricType.Equals((object) GitMetrics.CommitsPushed))
        {
          commitsPushedCount += metricsTrend.IntervalValue;
          CommitsTrendItem commitsTrendItem = new CommitsTrendItem(metricsTrend.TimeBucket, metricsTrend.IntervalValue);
          commitsTrend.Add(commitsTrendItem);
        }
        else
        {
          metricType = metricsTrend.MetricType;
          if (metricType.Equals((object) GitMetrics.PullRequestsCompleted))
          {
            pullRequestsCompletedCount += metricsTrend.IntervalValue;
          }
          else
          {
            metricType = metricsTrend.MetricType;
            if (metricType.Equals((object) GitMetrics.PullRequestsCreated))
              pullRequestsCreatedCount += metricsTrend.IntervalValue;
          }
        }
      }
      return new GitActivityMetrics(repoScope, commitsPushedCount, pullRequestsCreatedCount, pullRequestsCompletedCount, authorsCount, commitsTrend);
    }

    private void AddGitRefBinder(ResultCollection rc) => rc.AddBinder<TfsGitRef>((ObjectBinder<TfsGitRef>) new GitCoreComponent.GitRefBinder4());

    private void AddGitRepositoryInfoBinder(ResultCollection rc)
    {
      if (this.Version < 1830)
        rc.AddBinder<TfsGitRepositoryInfo>((ObjectBinder<TfsGitRepositoryInfo>) new GitCoreComponent.TfsGitRepositoryInfoBinder4(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
      else
        rc.AddBinder<TfsGitRepositoryInfo>((ObjectBinder<TfsGitRepositoryInfo>) new GitCoreComponent.TfsGitRepositoryInfoBinder5(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)));
    }

    private class DelegatedVoteBinder : ObjectBinder<TfsGitPullRequestDelegatedVote>
    {
      internal SqlColumnBinder reviewerId = new SqlColumnBinder("ReviewerId");
      internal SqlColumnBinder votedForId = new SqlColumnBinder("VotedForId");
      internal SqlColumnBinder pullRequestId = new SqlColumnBinder("PullRequestId");

      protected override TfsGitPullRequestDelegatedVote Bind()
      {
        int? pullRequestId = new int?();
        if (this.pullRequestId.ColumnExists((IDataReader) this.Reader))
          pullRequestId = new int?(this.pullRequestId.GetInt32((IDataReader) this.Reader));
        return new TfsGitPullRequestDelegatedVote(this.reviewerId.GetGuid((IDataReader) this.Reader, false), this.votedForId.GetGuid((IDataReader) this.Reader, false), pullRequestId);
      }
    }

    private class ReviewersWithVotesBinder2 : ObjectBinder<TfsGitPullRequest.ReviewerWithVote>
    {
      internal SqlColumnBinder reviewerId = new SqlColumnBinder("ReviewerId");
      internal SqlColumnBinder vote = new SqlColumnBinder("Vote");
      internal SqlColumnBinder status = new SqlColumnBinder("Status");
      internal SqlColumnBinder isRequired = new SqlColumnBinder("IsRequired");
      internal SqlColumnBinder pullRequestId = new SqlColumnBinder("PullRequestId");
      internal SqlColumnBinder isFlagged = new SqlColumnBinder("IsFlagged");
      internal SqlColumnBinder hasDeclined = new SqlColumnBinder("HasDeclined");

      protected override TfsGitPullRequest.ReviewerWithVote Bind()
      {
        int? pullRequestId = new int?();
        if (this.pullRequestId.ColumnExists((IDataReader) this.Reader))
          pullRequestId = new int?(this.pullRequestId.GetInt32((IDataReader) this.Reader));
        bool isFlagged = false;
        if (this.isFlagged.ColumnExists((IDataReader) this.Reader))
        {
          bool? nullableBoolean = this.isFlagged.GetNullableBoolean((IDataReader) this.Reader);
          bool flag = true;
          isFlagged = nullableBoolean.GetValueOrDefault() == flag & nullableBoolean.HasValue;
        }
        bool hasDeclined = false;
        if (this.hasDeclined.ColumnExists((IDataReader) this.Reader))
        {
          bool? nullableBoolean = this.hasDeclined.GetNullableBoolean((IDataReader) this.Reader);
          bool flag = true;
          hasDeclined = nullableBoolean.GetValueOrDefault() == flag & nullableBoolean.HasValue;
        }
        return new TfsGitPullRequest.ReviewerWithVote(this.reviewerId.GetGuid((IDataReader) this.Reader, false), this.vote.GetInt16((IDataReader) this.Reader), (ReviewerVoteStatus) this.status.GetByte((IDataReader) this.Reader, (byte) 0), isRequired: this.isRequired.GetBoolean((IDataReader) this.Reader, false), pullRequestId: pullRequestId, isFlagged: isFlagged, hasDeclined: hasDeclined);
      }
    }

    private class TfsGitPullRequestBinder : ObjectBinder<TfsGitPullRequest>
    {
      internal SqlColumnBinder pullRequestType = new SqlColumnBinder("PullRequestType");
      internal SqlColumnBinder repositoryId = new SqlColumnBinder("RepositoryId");
      internal SqlColumnBinder sourceRepositoryId = new SqlColumnBinder("SourceRepositoryId");
      internal SqlColumnBinder repositoryName = new SqlColumnBinder("Name");
      internal SqlColumnBinder repositoryCreatedByForking = new SqlColumnBinder("CreatedByForking");
      internal SqlColumnBinder projectUri = new SqlColumnBinder("TeamProjectUri");
      internal SqlColumnBinder pullRequestId = new SqlColumnBinder("PullRequestId");
      internal SqlColumnBinder status = new SqlColumnBinder("Status");
      internal SqlColumnBinder creatorId = new SqlColumnBinder("CreatorId");
      internal SqlColumnBinder creationTime = new SqlColumnBinder("CreationTime");
      internal SqlColumnBinder closedTime = new SqlColumnBinder("ClosedTime");
      internal SqlColumnBinder title = new SqlColumnBinder("Title");
      internal SqlColumnBinder description = new SqlColumnBinder("Description");
      internal SqlColumnBinder sourceName = new SqlColumnBinder("SourceName");
      internal SqlColumnBinder targetName = new SqlColumnBinder("TargetName");
      internal SqlColumnBinder mergeStatus = new SqlColumnBinder("MergeStatus");
      internal SqlColumnBinder mergeId = new SqlColumnBinder("MergeId");
      internal SqlColumnBinder lastMergeSource = new SqlColumnBinder("LastMergeSource");
      internal SqlColumnBinder lastMergeTarget = new SqlColumnBinder("LastMergeTarget");
      internal SqlColumnBinder lastMergeCommit = new SqlColumnBinder("LastMergeCommit");
      internal SqlColumnBinder completeWhenMergedAuthority = new SqlColumnBinder("CompleteWhenMergedAuthority");
      internal SqlColumnBinder reviewId = new SqlColumnBinder("ReviewId");
      internal SqlColumnBinder upgraded = new SqlColumnBinder("Upgraded");
      internal SqlColumnBinder completionOptions = new SqlColumnBinder("CompletionOptions");
      internal SqlColumnBinder mergeOptions = new SqlColumnBinder("MergeOptions");
      internal SqlColumnBinder mergeFailureMessage = new SqlColumnBinder("MergeFailureMessage");
      internal SqlColumnBinder failureType = new SqlColumnBinder("MergeFailureType");
      internal SqlColumnBinder completionQueueTime = new SqlColumnBinder("CompletionQueueTime");
      internal SqlColumnBinder autoCompleteAuthority = new SqlColumnBinder("AutoCompleteAuthority");
      internal SqlColumnBinder codeReviewType = new SqlColumnBinder("CodeReviewType");
      internal SqlColumnBinder codeReviewId = new SqlColumnBinder("CodeReviewId");

      protected override TfsGitPullRequest Bind() => new TfsGitPullRequest(this.repositoryId.GetGuid((IDataReader) this.Reader, false), this.pullRequestId.GetInt32((IDataReader) this.Reader), (PullRequestStatus) this.status.GetByte((IDataReader) this.Reader), this.creatorId.GetGuid((IDataReader) this.Reader, false), this.creationTime.GetDateTime((IDataReader) this.Reader), this.closedTime.GetDateTime((IDataReader) this.Reader, new DateTime()), this.title.GetString((IDataReader) this.Reader, true), this.description.GetString((IDataReader) this.Reader, true), this.sourceName.GetString((IDataReader) this.Reader, false), this.targetName.GetString((IDataReader) this.Reader, false), (PullRequestAsyncStatus) this.mergeStatus.GetByte((IDataReader) this.Reader), this.mergeId.GetGuid((IDataReader) this.Reader, false), this.lastMergeSource.GetNullableSha1Id((IDataReader) this.Reader), this.lastMergeTarget.GetNullableSha1Id((IDataReader) this.Reader), this.lastMergeCommit.GetNullableSha1Id((IDataReader) this.Reader), this.completeWhenMergedAuthority.GetGuid((IDataReader) this.Reader, true), (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) null, this.reviewId.ColumnExists((IDataReader) this.Reader) ? this.reviewId.GetInt32((IDataReader) this.Reader, 0, 0) : 0, this.upgraded.ColumnExists((IDataReader) this.Reader) && this.upgraded.GetBoolean((IDataReader) this.Reader, false), !this.completionOptions.ColumnExists((IDataReader) this.Reader) || this.completionOptions.IsNull((IDataReader) this.Reader) ? (GitPullRequestCompletionOptions) null : JsonConvert.DeserializeObject<GitPullRequestCompletionOptions>(this.completionOptions.GetString((IDataReader) this.Reader, (string) null)), !this.mergeOptions.ColumnExists((IDataReader) this.Reader) || this.mergeOptions.IsNull((IDataReader) this.Reader) ? (GitPullRequestMergeOptions) null : JsonConvert.DeserializeObject<GitPullRequestMergeOptions>(this.mergeOptions.GetString((IDataReader) this.Reader, (string) null)), this.failureType.ColumnExists((IDataReader) this.Reader) ? (PullRequestMergeFailureType) this.failureType.GetByte((IDataReader) this.Reader) : PullRequestMergeFailureType.None, this.mergeFailureMessage.ColumnExists((IDataReader) this.Reader) ? this.mergeFailureMessage.GetString((IDataReader) this.Reader, true) : (string) null, this.completionQueueTime.GetDateTime((IDataReader) this.Reader, new DateTime()), this.autoCompleteAuthority.GetGuid((IDataReader) this.Reader, true, Guid.Empty), this.repositoryName.GetString((IDataReader) this.Reader, (string) null), this.repositoryCreatedByForking.GetBoolean((IDataReader) this.Reader, false, false), this.projectUri.GetString((IDataReader) this.Reader, (string) null), new Guid?(this.sourceRepositoryId.ColumnExists((IDataReader) this.Reader) ? this.sourceRepositoryId.GetGuid((IDataReader) this.Reader, true, Guid.Empty) : Guid.Empty));
    }

    private class TfsGitPullRequestBinder2 : ObjectBinder<TfsGitPullRequest>
    {
      private SqlColumnBinder pullRequestType = new SqlColumnBinder("PullRequestType");
      private SqlColumnBinder repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder sourceRepositoryId = new SqlColumnBinder("SourceRepositoryId");
      private SqlColumnBinder repositoryName = new SqlColumnBinder("Name");
      private SqlColumnBinder repositoryCreatedByForking = new SqlColumnBinder("CreatedByForking");
      private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
      private SqlColumnBinder pullRequestId = new SqlColumnBinder("PullRequestId");
      private SqlColumnBinder status = new SqlColumnBinder("Status");
      private SqlColumnBinder creatorId = new SqlColumnBinder("CreatorId");
      private SqlColumnBinder creationTime = new SqlColumnBinder("CreationTime");
      private SqlColumnBinder closedTime = new SqlColumnBinder("ClosedTime");
      private SqlColumnBinder title = new SqlColumnBinder("Title");
      private SqlColumnBinder description = new SqlColumnBinder("Description");
      private SqlColumnBinder sourceName = new SqlColumnBinder("SourceName");
      private SqlColumnBinder targetName = new SqlColumnBinder("TargetName");
      private SqlColumnBinder mergeStatus = new SqlColumnBinder("MergeStatus");
      private SqlColumnBinder mergeId = new SqlColumnBinder("MergeId");
      private SqlColumnBinder lastMergeSource = new SqlColumnBinder("LastMergeSource");
      private SqlColumnBinder lastMergeTarget = new SqlColumnBinder("LastMergeTarget");
      private SqlColumnBinder lastMergeCommit = new SqlColumnBinder("LastMergeCommit");
      private SqlColumnBinder completeWhenMergedAuthority = new SqlColumnBinder("CompleteWhenMergedAuthority");
      private SqlColumnBinder reviewId = new SqlColumnBinder("ReviewId");
      private SqlColumnBinder upgraded = new SqlColumnBinder("Upgraded");
      private SqlColumnBinder completionOptions = new SqlColumnBinder("CompletionOptions");
      private SqlColumnBinder mergeOptions = new SqlColumnBinder("MergeOptions");
      private SqlColumnBinder mergeFailureMessage = new SqlColumnBinder("MergeFailureMessage");
      private SqlColumnBinder failureType = new SqlColumnBinder("MergeFailureType");
      private SqlColumnBinder isDraft = new SqlColumnBinder("IsDraft");
      private SqlColumnBinder completionQueueTime = new SqlColumnBinder("CompletionQueueTime");
      private SqlColumnBinder autoCompleteAuthority = new SqlColumnBinder("AutoCompleteAuthority");
      private SqlColumnBinder updatedTime = new SqlColumnBinder("UpdatedTime");
      private SqlColumnBinder codeReviewType = new SqlColumnBinder("CodeReviewType");
      private SqlColumnBinder codeReviewId = new SqlColumnBinder("CodeReviewId");
      private readonly System.Func<int, Guid> m_dataspaceIdToIdentifier;

      public TfsGitPullRequestBinder2(System.Func<int, Guid> dataspaceIdToIdentifier) => this.m_dataspaceIdToIdentifier = dataspaceIdToIdentifier;

      protected override TfsGitPullRequest Bind()
      {
        int int32 = this.dataspaceId.GetInt32((IDataReader) this.Reader, 0, 0);
        return new TfsGitPullRequest(this.repositoryId.GetGuid((IDataReader) this.Reader, false), this.pullRequestId.GetInt32((IDataReader) this.Reader), (PullRequestStatus) this.status.GetByte((IDataReader) this.Reader), this.creatorId.GetGuid((IDataReader) this.Reader, false), this.creationTime.GetDateTime((IDataReader) this.Reader), this.closedTime.GetDateTime((IDataReader) this.Reader, new DateTime()), this.title.GetString((IDataReader) this.Reader, true), this.description.GetString((IDataReader) this.Reader, true), this.sourceName.GetString((IDataReader) this.Reader, false), this.targetName.GetString((IDataReader) this.Reader, false), (PullRequestAsyncStatus) this.mergeStatus.GetByte((IDataReader) this.Reader), this.mergeId.GetGuid((IDataReader) this.Reader, false), this.lastMergeSource.GetNullableSha1Id((IDataReader) this.Reader), this.lastMergeTarget.GetNullableSha1Id((IDataReader) this.Reader), this.lastMergeCommit.GetNullableSha1Id((IDataReader) this.Reader), this.completeWhenMergedAuthority.GetGuid((IDataReader) this.Reader, true), (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) null, this.reviewId.ColumnExists((IDataReader) this.Reader) ? this.reviewId.GetInt32((IDataReader) this.Reader, 0, 0) : 0, this.upgraded.ColumnExists((IDataReader) this.Reader) && this.upgraded.GetBoolean((IDataReader) this.Reader, false), !this.completionOptions.ColumnExists((IDataReader) this.Reader) || this.completionOptions.IsNull((IDataReader) this.Reader) ? (GitPullRequestCompletionOptions) null : JsonConvert.DeserializeObject<GitPullRequestCompletionOptions>(this.completionOptions.GetString((IDataReader) this.Reader, (string) null)), !this.mergeOptions.ColumnExists((IDataReader) this.Reader) || this.mergeOptions.IsNull((IDataReader) this.Reader) ? (GitPullRequestMergeOptions) null : JsonConvert.DeserializeObject<GitPullRequestMergeOptions>(this.mergeOptions.GetString((IDataReader) this.Reader, (string) null)), this.failureType.ColumnExists((IDataReader) this.Reader) ? (PullRequestMergeFailureType) this.failureType.GetByte((IDataReader) this.Reader) : PullRequestMergeFailureType.None, this.mergeFailureMessage.ColumnExists((IDataReader) this.Reader) ? this.mergeFailureMessage.GetString((IDataReader) this.Reader, true) : (string) null, this.completionQueueTime.GetDateTime((IDataReader) this.Reader, new DateTime()), this.autoCompleteAuthority.GetGuid((IDataReader) this.Reader, true, Guid.Empty), this.repositoryName.GetString((IDataReader) this.Reader, (string) null), this.repositoryCreatedByForking.GetBoolean((IDataReader) this.Reader, false, false), int32 == 0 ? (string) null : ProjectInfo.GetProjectUri(this.m_dataspaceIdToIdentifier(int32)), new Guid?(this.sourceRepositoryId.ColumnExists((IDataReader) this.Reader) ? this.sourceRepositoryId.GetGuid((IDataReader) this.Reader, true, Guid.Empty) : Guid.Empty), this.isDraft.ColumnExists((IDataReader) this.Reader) && this.isDraft.GetBoolean((IDataReader) this.Reader, false), this.updatedTime.GetDateTime((IDataReader) this.Reader, new DateTime()));
      }
    }

    private class TfsGitCommitToPullRequestBinder : ObjectBinder<KeyValuePair<Sha1Id, int>>
    {
      internal SqlColumnBinder commitId = new SqlColumnBinder("CommitId");
      internal SqlColumnBinder pullRequestId = new SqlColumnBinder("PullRequestId");

      protected override KeyValuePair<Sha1Id, int> Bind() => new KeyValuePair<Sha1Id, int>(this.commitId.GetSha1Id((IDataReader) this.Reader), this.pullRequestId.GetInt32((IDataReader) this.Reader));
    }

    private class GitActiveRepoBinder : ObjectBinder<GitActiveRepoInfo>
    {
      private SqlColumnBinder m_metricValue = new SqlColumnBinder("MetricValue");
      private SqlColumnBinder m_repoId = new SqlColumnBinder("RepositoryId");

      protected override GitActiveRepoInfo Bind() => new GitActiveRepoInfo(this.m_repoId.GetGuid((IDataReader) this.Reader, true), this.m_metricValue.GetInt32((IDataReader) this.Reader));
    }

    private class GitAdvSecAddBillableCommits : ObjectBinder<GitAdvSecBillableCommit>
    {
      private SqlColumnBinder m_projectId = new SqlColumnBinder("ProjectId");
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_commitId = new SqlColumnBinder("CommitId");
      private SqlColumnBinder m_committerVSID = new SqlColumnBinder("CommitterVSID");
      private SqlColumnBinder m_commitTime = new SqlColumnBinder("CommitTime");
      private SqlColumnBinder m_pushedTime = new SqlColumnBinder("PushTime");

      protected override GitAdvSecBillableCommit Bind() => new GitAdvSecBillableCommit(this.m_projectId.GetGuid((IDataReader) this.Reader), this.m_repositoryId.GetGuid((IDataReader) this.Reader), this.m_commitId.GetSha1Id((IDataReader) this.Reader), this.m_committerVSID.GetNullableGuid((IDataReader) this.Reader), this.m_commitTime.GetDateTime((IDataReader) this.Reader), this.m_pushedTime.GetDateTime((IDataReader) this.Reader));
    }

    private class GitAdvSecBillableCommitterBinder : ObjectBinder<GitBillableCommitter>
    {
      private SqlColumnBinder m_vsid = new SqlColumnBinder("VSID");
      private SqlColumnBinder m_repoId = new SqlColumnBinder("RepoId");

      protected override GitBillableCommitter Bind() => new GitBillableCommitter(this.m_vsid.GetGuid((IDataReader) this.Reader), this.m_repoId.GetGuid((IDataReader) this.Reader));
    }

    private class GitAdvSecBillableCommitterDetailBinder : ObjectBinder<GitBillableCommitterDetail>
    {
      private SqlColumnBinder m_projectId = new SqlColumnBinder("ProjectId");
      private SqlColumnBinder m_projectName = new SqlColumnBinder("Projectname");
      private SqlColumnBinder m_repoId = new SqlColumnBinder("RepoId");
      private SqlColumnBinder m_repoName = new SqlColumnBinder("RepoName");
      private SqlColumnBinder m_commitId = new SqlColumnBinder("CommitId");
      private SqlColumnBinder m_vsid = new SqlColumnBinder("VSID");
      private SqlColumnBinder m_committerEmail = new SqlColumnBinder("CommitterEmail");
      private SqlColumnBinder m_commitTime = new SqlColumnBinder("CommitTime");
      private SqlColumnBinder m_pushId = new SqlColumnBinder("PushId");
      private SqlColumnBinder m_pushedTime = new SqlColumnBinder("PushedTime");

      protected override GitBillableCommitterDetail Bind() => new GitBillableCommitterDetail(this.m_projectId.GetGuid((IDataReader) this.Reader), this.m_projectName.GetString((IDataReader) this.Reader, false), this.m_repoId.GetGuid((IDataReader) this.Reader), this.m_repoName.GetString((IDataReader) this.Reader, false), this.m_commitId.GetSha1Id((IDataReader) this.Reader), this.m_vsid.GetGuid((IDataReader) this.Reader), this.m_committerEmail.GetString((IDataReader) this.Reader, true), this.m_commitTime.GetDateTime((IDataReader) this.Reader), this.m_pushId.GetInt32((IDataReader) this.Reader), this.m_pushedTime.GetDateTime((IDataReader) this.Reader));
    }

    private class GitAdvSecBillableCommitterDetailBinder2 : ObjectBinder<GitBillableCommitterDetail>
    {
      private SqlColumnBinder m_projectId = new SqlColumnBinder("ProjectId");
      private SqlColumnBinder m_projectName = new SqlColumnBinder("Projectname");
      private SqlColumnBinder m_repoId = new SqlColumnBinder("RepoId");
      private SqlColumnBinder m_repoName = new SqlColumnBinder("RepoName");
      private SqlColumnBinder m_commitId = new SqlColumnBinder("CommitId");
      private SqlColumnBinder m_vsid = new SqlColumnBinder("VSID");
      private SqlColumnBinder m_committerEmail = new SqlColumnBinder("CommitterEmail");
      private SqlColumnBinder m_commitTime = new SqlColumnBinder("CommitTime");
      private SqlColumnBinder m_pushId = new SqlColumnBinder("PushId");
      private SqlColumnBinder m_pushedTime = new SqlColumnBinder("PushedTime");
      private SqlColumnBinder m_pusherId = new SqlColumnBinder("PusherId");
      private SqlColumnBinder m_samAccountName = new SqlColumnBinder("SamAccountName");
      private SqlColumnBinder m_mailNickName = new SqlColumnBinder("MailNickName");
      private SqlColumnBinder m_displayName = new SqlColumnBinder("DisplayName");

      protected override GitBillableCommitterDetail Bind() => new GitBillableCommitterDetail(this.m_projectId.GetGuid((IDataReader) this.Reader), this.m_projectName.GetString((IDataReader) this.Reader, false), this.m_repoId.GetGuid((IDataReader) this.Reader), this.m_repoName.GetString((IDataReader) this.Reader, false), this.m_commitId.GetSha1Id((IDataReader) this.Reader), this.m_vsid.GetGuid((IDataReader) this.Reader), this.m_committerEmail.GetString((IDataReader) this.Reader, true), this.m_commitTime.GetDateTime((IDataReader) this.Reader), this.m_pushId.GetInt32((IDataReader) this.Reader), this.m_pushedTime.GetDateTime((IDataReader) this.Reader), this.m_pusherId.GetGuid((IDataReader) this.Reader), this.m_samAccountName.GetString((IDataReader) this.Reader, true), this.m_mailNickName.GetString((IDataReader) this.Reader, true), this.m_displayName.GetString((IDataReader) this.Reader, true));
    }

    private class GitAdvSecBillablePusherBinder : ObjectBinder<GitBillablePusher>
    {
      private SqlColumnBinder m_projectId = new SqlColumnBinder("ProjectId");
      private SqlColumnBinder m_repoId = new SqlColumnBinder("RepoId");
      private SqlColumnBinder m_vsid = new SqlColumnBinder("VSID");

      protected override GitBillablePusher Bind() => new GitBillablePusher(this.m_projectId.GetGuid((IDataReader) this.Reader), this.m_repoId.GetGuid((IDataReader) this.Reader), this.m_vsid.GetGuid((IDataReader) this.Reader));
    }

    private class GitAdvSecEnablementStatusBinder : ObjectBinder<GitAdvSecEnablementStatus>
    {
      private SqlColumnBinder m_projectId = new SqlColumnBinder("ProjectId");
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_enabled = new SqlColumnBinder("Enabled");
      private SqlColumnBinder m_enabledChangedOnDate = new SqlColumnBinder("EnabledChangedOnDate");

      protected override GitAdvSecEnablementStatus Bind() => new GitAdvSecEnablementStatus(this.m_projectId.GetGuid((IDataReader) this.Reader), this.m_repositoryId.GetGuid((IDataReader) this.Reader), this.m_enabled.GetNullableBoolean((IDataReader) this.Reader), this.m_enabledChangedOnDate.GetNullableDateTime((IDataReader) this.Reader));
    }

    private class GitAdvSecEnablementStatusBinder2 : ObjectBinder<GitAdvSecEnablementStatus>
    {
      private SqlColumnBinder m_projectId = new SqlColumnBinder("ProjectId");
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_enabled = new SqlColumnBinder("Enabled");
      private SqlColumnBinder m_changedOnDate = new SqlColumnBinder("ChangedOnDate");
      private SqlColumnBinder m_changedBy = new SqlColumnBinder("ChangedBy");

      protected override GitAdvSecEnablementStatus Bind() => new GitAdvSecEnablementStatus(this.m_projectId.GetGuid((IDataReader) this.Reader), this.m_repositoryId.GetGuid((IDataReader) this.Reader), this.m_enabled.GetNullableBoolean((IDataReader) this.Reader), this.m_changedOnDate.GetNullableDateTime((IDataReader) this.Reader), this.m_changedBy.IsNull((IDataReader) this.Reader) ? new Guid() : this.m_changedBy.GetGuid((IDataReader) this.Reader));
    }

    private class GitAdvSecProjectStatsBinder : ObjectBinder<GitAdvSecProjectStats>
    {
      private SqlColumnBinder m_projectId = new SqlColumnBinder("ProjectId");
      private SqlColumnBinder m_projectName = new SqlColumnBinder("ProjectName");
      private SqlColumnBinder m_totalRepositories = new SqlColumnBinder("TotalRepositories");
      private SqlColumnBinder m_enabledRepositories = new SqlColumnBinder("EnabledRepositories");
      private SqlColumnBinder m_validCommitters = new SqlColumnBinder("ValidCommitters");
      private SqlColumnBinder m_totalCommitters = new SqlColumnBinder("TotalCommitters");
      private SqlColumnBinder m_totalPushers = new SqlColumnBinder("TotalPushers");

      protected override GitAdvSecProjectStats Bind() => new GitAdvSecProjectStats(this.m_projectId.GetGuid((IDataReader) this.Reader), this.m_projectName.GetString((IDataReader) this.Reader, false), this.m_totalRepositories.GetInt32((IDataReader) this.Reader), this.m_enabledRepositories.GetInt32((IDataReader) this.Reader), this.m_validCommitters.GetInt32((IDataReader) this.Reader), this.m_totalCommitters.GetInt32((IDataReader) this.Reader), this.m_totalPushers.GetInt32((IDataReader) this.Reader));
    }

    private class GitAuthorsCountBinder : ObjectBinder<GitAuthorsCountInfo>
    {
      private SqlColumnBinder m_authorsCount = new SqlColumnBinder("Authors");
      private SqlColumnBinder m_repoId = new SqlColumnBinder("RepositoryId");

      protected override GitAuthorsCountInfo Bind() => new GitAuthorsCountInfo(this.m_repoId.GetGuid((IDataReader) this.Reader), this.m_authorsCount.GetInt32((IDataReader) this.Reader));
    }

    private class GitCommitScanDataBinder : ObjectBinder<GitCommitScanData>
    {
      private SqlColumnBinder m_commitId = new SqlColumnBinder("CommitId");
      private SqlColumnBinder m_commitTime = new SqlColumnBinder("CommitTime");
      private SqlColumnBinder m_toolId = new SqlColumnBinder("ToolId");
      private SqlColumnBinder m_versionId = new SqlColumnBinder("VersionId");
      private SqlColumnBinder m_scanStatus = new SqlColumnBinder("ScanStatus");
      private SqlColumnBinder m_attemptCount = new SqlColumnBinder("AttemptCount");
      private SqlColumnBinder m_scanTime = new SqlColumnBinder("ScanTime");

      protected override GitCommitScanData Bind() => new GitCommitScanData(this.m_commitId.GetSha1Id((IDataReader) this.Reader), this.m_commitTime.GetDateTime((IDataReader) this.Reader, DateTime.MinValue), this.m_toolId.GetInt32((IDataReader) this.Reader), this.m_versionId.GetInt32((IDataReader) this.Reader), this.m_scanStatus.GetInt32((IDataReader) this.Reader), this.m_attemptCount.GetInt32((IDataReader) this.Reader), this.m_scanTime.GetDateTime((IDataReader) this.Reader, DateTime.MinValue));
    }

    private class GitCommitScanStatusDataBinder : ObjectBinder<GitCommitScanStatusData>
    {
      private SqlColumnBinder m_commitId = new SqlColumnBinder("CommitId");
      private SqlColumnBinder m_scanStatus = new SqlColumnBinder("ScanStatus");

      protected override GitCommitScanStatusData Bind() => new GitCommitScanStatusData(this.m_commitId.GetSha1Id((IDataReader) this.Reader), this.m_scanStatus.GetInt32((IDataReader) this.Reader));
    }

    private class GitMetricsTrendBinder : ObjectBinder<MetricsTrendItem>
    {
      private SqlColumnBinder m_metric = new SqlColumnBinder("Metric");
      private SqlColumnBinder m_timeBucket = new SqlColumnBinder("TimeBucket");
      private SqlColumnBinder m_intervalValue = new SqlColumnBinder("IntervalValue");

      protected override MetricsTrendItem Bind() => new MetricsTrendItem((GitMetrics) this.m_metric.GetByte((IDataReader) this.Reader), this.m_timeBucket.GetInt32((IDataReader) this.Reader), this.m_intervalValue.GetInt32((IDataReader) this.Reader));
    }

    private class GitPushMetadataBinder : ObjectBinder<TfsGitPushMetadata>
    {
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_pushId = new SqlColumnBinder("PushId");
      private SqlColumnBinder m_pusherId = new SqlColumnBinder("PusherId");
      private SqlColumnBinder m_pushTime = new SqlColumnBinder("PushTime");

      protected override TfsGitPushMetadata Bind() => new TfsGitPushMetadata(this.m_repositoryId.GetGuid((IDataReader) this.Reader), this.m_pushId.GetInt32((IDataReader) this.Reader), this.m_pusherId.GetGuid((IDataReader) this.Reader), this.m_pushTime.GetDateTime((IDataReader) this.Reader));
    }

    private class GitRefBinder3 : ObjectBinder<TfsGitRef>
    {
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_objectId = new SqlColumnBinder("ObjectId");
      private SqlColumnBinder m_isDefaultBranch = new SqlColumnBinder("IsDefaultBranch");
      private SqlColumnBinder m_isLockedById = new SqlColumnBinder("IsLockedById");

      protected override TfsGitRef Bind() => new TfsGitRef(this.m_name.GetString((IDataReader) this.Reader, false), this.m_objectId.GetSha1Id((IDataReader) this.Reader), this.m_isDefaultBranch.GetBoolean((IDataReader) this.Reader), this.m_isLockedById.IsNull((IDataReader) this.Reader) ? new Guid?() : new Guid?(this.m_isLockedById.GetGuid((IDataReader) this.Reader)));
    }

    private class GitRefBinder4 : ObjectBinder<TfsGitRef>
    {
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_objectId = new SqlColumnBinder("ObjectId");
      private SqlColumnBinder m_isDefaultBranch = new SqlColumnBinder("IsDefaultBranch");
      private SqlColumnBinder m_isLockedById = new SqlColumnBinder("IsLockedById");
      private SqlColumnBinder m_creatorId = new SqlColumnBinder("CreatorId");

      protected override TfsGitRef Bind() => new TfsGitRef(this.m_name.GetString((IDataReader) this.Reader, false), this.m_objectId.GetSha1Id((IDataReader) this.Reader), this.m_isDefaultBranch.GetBoolean((IDataReader) this.Reader), this.m_isLockedById.IsNull((IDataReader) this.Reader) ? new Guid?() : new Guid?(this.m_isLockedById.GetGuid((IDataReader) this.Reader)), this.m_creatorId.IsNull((IDataReader) this.Reader) ? new Guid?() : new Guid?(this.m_creatorId.GetGuid((IDataReader) this.Reader)));
    }

    private class GitRefWithResolvedCommitBinder2 : ObjectBinder<TfsGitRefWithResolvedCommit>
    {
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_objectId = new SqlColumnBinder("ObjectId");
      private SqlColumnBinder m_isDefaultBranch = new SqlColumnBinder("IsDefaultBranch");
      private SqlColumnBinder m_isLockedById = new SqlColumnBinder("IsLockedById");
      private SqlColumnBinder m_commitId = new SqlColumnBinder("CommitId");

      protected override TfsGitRefWithResolvedCommit Bind() => new TfsGitRefWithResolvedCommit(this.m_name.GetString((IDataReader) this.Reader, false), this.m_objectId.GetSha1Id((IDataReader) this.Reader), this.m_isDefaultBranch.GetBoolean((IDataReader) this.Reader), this.m_isLockedById.IsNull((IDataReader) this.Reader) ? new Guid?() : new Guid?(this.m_isLockedById.GetGuid((IDataReader) this.Reader)), new Guid?(), this.m_commitId.GetNullableSha1Id((IDataReader) this.Reader));
    }

    private class GitRefWithResolvedCommitBinder3 : ObjectBinder<TfsGitRefWithResolvedCommit>
    {
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_objectId = new SqlColumnBinder("ObjectId");
      private SqlColumnBinder m_isDefaultBranch = new SqlColumnBinder("IsDefaultBranch");
      private SqlColumnBinder m_isLockedById = new SqlColumnBinder("IsLockedById");
      private SqlColumnBinder m_creatorId = new SqlColumnBinder("CreatorId");
      private SqlColumnBinder m_commitId = new SqlColumnBinder("CommitId");

      protected override TfsGitRefWithResolvedCommit Bind() => new TfsGitRefWithResolvedCommit(this.m_name.GetString((IDataReader) this.Reader, false), this.m_objectId.GetSha1Id((IDataReader) this.Reader), this.m_isDefaultBranch.GetBoolean((IDataReader) this.Reader), this.m_isLockedById.IsNull((IDataReader) this.Reader) ? new Guid?() : new Guid?(this.m_isLockedById.GetGuid((IDataReader) this.Reader)), this.m_creatorId.IsNull((IDataReader) this.Reader) ? new Guid?() : new Guid?(this.m_creatorId.GetGuid((IDataReader) this.Reader)), this.m_commitId.GetNullableSha1Id((IDataReader) this.Reader));
    }

    private class GitRepositoryMetricsTrendBinder : ObjectBinder<RepositoryMetricsTrendItem>
    {
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_metric = new SqlColumnBinder("Metric");
      private SqlColumnBinder m_timeBucket = new SqlColumnBinder("TimeBucket");
      private SqlColumnBinder m_intervalValue = new SqlColumnBinder("IntervalValue");

      protected override RepositoryMetricsTrendItem Bind() => new RepositoryMetricsTrendItem(this.m_repositoryId.GetGuid((IDataReader) this.Reader), (GitMetrics) this.m_metric.GetByte((IDataReader) this.Reader), this.m_timeBucket.GetInt32((IDataReader) this.Reader), this.m_intervalValue.GetInt32((IDataReader) this.Reader));
    }

    private class LfsLockBinder : ObjectBinder<LfsLock>
    {
      private SqlColumnBinder m_lockId = new SqlColumnBinder("LockId");
      private SqlColumnBinder m_path = new SqlColumnBinder("Path");
      private SqlColumnBinder m_ownerId = new SqlColumnBinder("OwnerId");
      private SqlColumnBinder m_createdDate = new SqlColumnBinder("CreatedDate");
      private SqlColumnBinder m_succeeded = new SqlColumnBinder("Succeeded");

      protected override LfsLock Bind()
      {
        int int32 = this.m_lockId.GetInt32((IDataReader) this.Reader);
        string str = this.m_path.GetString((IDataReader) this.Reader, false);
        Guid guid = this.m_ownerId.GetGuid((IDataReader) this.Reader, false);
        DateTime dateTime = this.m_createdDate.GetDateTime((IDataReader) this.Reader);
        bool boolean = this.m_succeeded.GetBoolean((IDataReader) this.Reader);
        string path = str.Trim('/');
        DateTime lockedAt = dateTime;
        Guid ownerId = guid;
        int num = boolean ? 1 : 0;
        return new LfsLock(int32, path, lockedAt, ownerId, num != 0);
      }
    }

    private class ProcessedCommitBinder2 : ObjectBinder<TfsGitProcessedCommit>
    {
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_commitId = new SqlColumnBinder("CommitId");
      private SqlColumnBinder m_pushId = new SqlColumnBinder("PushId");

      protected override TfsGitProcessedCommit Bind() => new TfsGitProcessedCommit(this.m_repositoryId.GetGuid((IDataReader) this.Reader), this.m_commitId.GetSha1Id((IDataReader) this.Reader), this.m_pushId.GetInt32((IDataReader) this.Reader));
    }

    private class TfsGitRefLogBinder2 : ObjectBinder<TfsGitRefLogEntry>
    {
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_pushId = new SqlColumnBinder("PushId");
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_oldObjectId = new SqlColumnBinder("OldObjectId");
      private SqlColumnBinder m_newObjectId = new SqlColumnBinder("NewObjectId");

      protected override TfsGitRefLogEntry Bind() => new TfsGitRefLogEntry(this.m_repositoryId.GetGuid((IDataReader) this.Reader, false), this.m_pushId.GetInt32((IDataReader) this.Reader), this.m_name.GetString((IDataReader) this.Reader, false), this.m_oldObjectId.GetSha1Id((IDataReader) this.Reader), this.m_newObjectId.GetSha1Id((IDataReader) this.Reader));
    }

    private class TfsGitRefUpdateResultBinder4 : ObjectBinder<TfsGitRefUpdateResult>
    {
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_oldObjectId = new SqlColumnBinder("OldObjectId");
      private SqlColumnBinder m_newObjectId = new SqlColumnBinder("NewObjectId");
      private SqlColumnBinder m_status = new SqlColumnBinder("Status");
      private SqlColumnBinder m_conflictingName = new SqlColumnBinder("ConflictingName");
      private SqlColumnBinder m_isLockedById = new SqlColumnBinder("IsLockedById");

      protected override TfsGitRefUpdateResult Bind() => new TfsGitRefUpdateResult(this.m_name.GetString((IDataReader) this.Reader, false), this.m_oldObjectId.GetSha1Id((IDataReader) this.Reader), this.m_newObjectId.GetSha1Id((IDataReader) this.Reader), (GitRefUpdateStatus) this.m_status.GetInt32((IDataReader) this.Reader), this.m_conflictingName.GetString((IDataReader) this.Reader, true), this.m_isLockedById.IsNull((IDataReader) this.Reader) ? new Guid?() : new Guid?(this.m_isLockedById.GetGuid((IDataReader) this.Reader)));
    }

    private class TfsGitRepositoryInfoBinder : ObjectBinder<TfsGitRepositoryInfo>
    {
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_teamProjectUri = new SqlColumnBinder("TeamProjectUri");
      private SqlColumnBinder m_containerId = new SqlColumnBinder("ContainerId");

      protected override TfsGitRepositoryInfo Bind()
      {
        Guid guid1 = this.m_repositoryId.GetGuid((IDataReader) this.Reader, false);
        Guid guid2 = this.m_containerId.GetGuid((IDataReader) this.Reader, false);
        return new TfsGitRepositoryInfo(this.m_name.GetString((IDataReader) this.Reader, false), new RepoKey(ProjectInfo.GetProjectId(this.m_teamProjectUri.GetString((IDataReader) this.Reader, false)), guid1, guid2));
      }
    }

    private class TfsGitRepositoryInfoBinder2 : ObjectBinder<TfsGitRepositoryInfo>
    {
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_teamProjectUri = new SqlColumnBinder("TeamProjectUri");
      private SqlColumnBinder m_defaultBranch = new SqlColumnBinder("DefaultBranch");
      private SqlColumnBinder m_containerId = new SqlColumnBinder("ContainerId");
      private SqlColumnBinder m_createdByForking = new SqlColumnBinder("CreatedByForking");

      protected override TfsGitRepositoryInfo Bind()
      {
        Guid guid1 = this.m_repositoryId.GetGuid((IDataReader) this.Reader, false);
        Guid guid2 = this.m_containerId.GetGuid((IDataReader) this.Reader, false);
        return new TfsGitRepositoryInfo(this.m_name.GetString((IDataReader) this.Reader, false), new RepoKey(ProjectInfo.GetProjectId(this.m_teamProjectUri.GetString((IDataReader) this.Reader, false)), guid1, guid2), this.m_defaultBranch.GetString((IDataReader) this.Reader, true), this.m_createdByForking.GetBoolean((IDataReader) this.Reader, false, false));
      }
    }

    private class TfsGitRepositoryInfoBinder3 : ObjectBinder<TfsGitRepositoryInfo>
    {
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
      private SqlColumnBinder m_defaultBranch = new SqlColumnBinder("DefaultBranch");
      private SqlColumnBinder m_containerId = new SqlColumnBinder("ContainerId");
      private SqlColumnBinder m_createdByForking = new SqlColumnBinder("CreatedByForking");
      private SqlColumnBinder m_compressedSize = new SqlColumnBinder("CompressedSize");
      private readonly System.Func<int, Guid> m_getDataspaceIdentifier;

      public TfsGitRepositoryInfoBinder3(System.Func<int, Guid> getDataspaceIdentifier) => this.m_getDataspaceIdentifier = getDataspaceIdentifier;

      protected override TfsGitRepositoryInfo Bind()
      {
        Guid guid1 = this.m_repositoryId.GetGuid((IDataReader) this.Reader, false);
        Guid guid2 = this.m_containerId.GetGuid((IDataReader) this.Reader, false);
        return new TfsGitRepositoryInfo(this.m_name.GetString((IDataReader) this.Reader, false), new RepoKey(this.m_getDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader)), guid1, guid2), this.m_defaultBranch.GetString((IDataReader) this.Reader, true), this.m_createdByForking.GetBoolean((IDataReader) this.Reader, false, false), this.m_compressedSize.GetInt64((IDataReader) this.Reader, -1L, -1L));
      }
    }

    private class TfsGitRepositoryInfoBinder4 : ObjectBinder<TfsGitRepositoryInfo>
    {
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
      private SqlColumnBinder m_defaultBranch = new SqlColumnBinder("DefaultBranch");
      private SqlColumnBinder m_containerId = new SqlColumnBinder("ContainerId");
      private SqlColumnBinder m_createdByForking = new SqlColumnBinder("CreatedByForking");
      private SqlColumnBinder m_compressedSize = new SqlColumnBinder("CompressedSize");
      private SqlColumnBinder m_creationDate = new SqlColumnBinder("CreationDate");
      private SqlColumnBinder m_lastMetadataUpdate = new SqlColumnBinder("LastMetadataUpdate");
      private readonly System.Func<int, Guid> m_getDataspaceIdentifier;

      public TfsGitRepositoryInfoBinder4(System.Func<int, Guid> getDataspaceIdentifier) => this.m_getDataspaceIdentifier = getDataspaceIdentifier;

      protected override TfsGitRepositoryInfo Bind()
      {
        Guid guid1 = this.m_repositoryId.GetGuid((IDataReader) this.Reader, false);
        Guid guid2 = this.m_containerId.GetGuid((IDataReader) this.Reader, false);
        return new TfsGitRepositoryInfo(this.m_name.GetString((IDataReader) this.Reader, false), new RepoKey(this.m_getDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader)), guid1, guid2), this.m_defaultBranch.GetString((IDataReader) this.Reader, true), this.m_createdByForking.GetBoolean((IDataReader) this.Reader, false, false), this.m_compressedSize.GetInt64((IDataReader) this.Reader, -1L, -1L), this.m_creationDate.GetDateTime((IDataReader) this.Reader), this.m_lastMetadataUpdate.GetDateTime((IDataReader) this.Reader));
      }
    }

    private class TfsGitRepositoryInfoBinder5 : ObjectBinder<TfsGitRepositoryInfo>
    {
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
      private SqlColumnBinder m_defaultBranch = new SqlColumnBinder("DefaultBranch");
      private SqlColumnBinder m_containerId = new SqlColumnBinder("ContainerId");
      private SqlColumnBinder m_createdByForking = new SqlColumnBinder("CreatedByForking");
      private SqlColumnBinder m_compressedSize = new SqlColumnBinder("CompressedSize");
      private SqlColumnBinder m_creationDate = new SqlColumnBinder("CreationDate");
      private SqlColumnBinder m_lastMetadataUpdate = new SqlColumnBinder("LastMetadataUpdate");
      private SqlColumnBinder m_Disabled = new SqlColumnBinder("Disabled");
      private readonly System.Func<int, Guid> m_getDataspaceIdentifier;

      public TfsGitRepositoryInfoBinder5(System.Func<int, Guid> getDataspaceIdentifier) => this.m_getDataspaceIdentifier = getDataspaceIdentifier;

      protected override TfsGitRepositoryInfo Bind()
      {
        Guid guid1 = this.m_repositoryId.GetGuid((IDataReader) this.Reader, false);
        Guid guid2 = this.m_containerId.GetGuid((IDataReader) this.Reader, false);
        return new TfsGitRepositoryInfo(this.m_name.GetString((IDataReader) this.Reader, false), new RepoKey(this.m_getDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader)), guid1, guid2), this.m_defaultBranch.GetString((IDataReader) this.Reader, true), this.m_createdByForking.GetBoolean((IDataReader) this.Reader, false, false), this.m_compressedSize.GetInt64((IDataReader) this.Reader, -1L, -1L), this.m_creationDate.GetDateTime((IDataReader) this.Reader), this.m_lastMetadataUpdate.GetDateTime((IDataReader) this.Reader), this.m_Disabled.ColumnExists((IDataReader) this.Reader) && this.m_Disabled.GetInt32((IDataReader) this.Reader, 0) != 0);
      }
    }

    private class RawRepoInfoBinder : ObjectBinder<RawRepoInfo>
    {
      private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_containerId = new SqlColumnBinder("ContainerId");
      private SqlColumnBinder m_createdByForking = new SqlColumnBinder("CreatedByForking");
      private SqlColumnBinder m_compressedSize = new SqlColumnBinder("CompressedSize");

      protected override RawRepoInfo Bind() => new RawRepoInfo(this.m_dataspaceId.GetInt32((IDataReader) this.Reader), this.m_repositoryId.GetGuid((IDataReader) this.Reader), this.m_name.GetString((IDataReader) this.Reader, false), this.m_containerId.GetGuid((IDataReader) this.Reader), this.m_createdByForking.GetBoolean((IDataReader) this.Reader), this.m_compressedSize.GetInt64((IDataReader) this.Reader, -1L));
    }

    private class RawRepoInfoBinder2 : ObjectBinder<RawRepoInfo>
    {
      private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_containerId = new SqlColumnBinder("ContainerId");
      private SqlColumnBinder m_createdByForking = new SqlColumnBinder("CreatedByForking");
      private SqlColumnBinder m_compressedSize = new SqlColumnBinder("CompressedSize");
      private SqlColumnBinder m_creationDate = new SqlColumnBinder("CreationDate");
      private SqlColumnBinder m_lastMetadataUpdate = new SqlColumnBinder("LastMetadataUpdate");

      protected override RawRepoInfo Bind() => new RawRepoInfo(this.m_dataspaceId.GetInt32((IDataReader) this.Reader), this.m_repositoryId.GetGuid((IDataReader) this.Reader), this.m_name.GetString((IDataReader) this.Reader, false), this.m_containerId.GetGuid((IDataReader) this.Reader), this.m_createdByForking.GetBoolean((IDataReader) this.Reader), this.m_compressedSize.GetInt64((IDataReader) this.Reader, -1L), this.m_creationDate.GetDateTime((IDataReader) this.Reader), this.m_lastMetadataUpdate.GetDateTime((IDataReader) this.Reader));
    }

    private class DeletedTfsGitRepositoryInfoBinder : ObjectBinder<TfsGitRepositoryInfo>
    {
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
      private SqlColumnBinder m_containerId = new SqlColumnBinder("ContainerId");
      private readonly System.Func<int, Guid> m_getDataspaceIdentifier;

      public DeletedTfsGitRepositoryInfoBinder(System.Func<int, Guid> getDataspaceIdentifier) => this.m_getDataspaceIdentifier = getDataspaceIdentifier;

      protected override TfsGitRepositoryInfo Bind()
      {
        Guid guid1 = this.m_repositoryId.GetGuid((IDataReader) this.Reader, false);
        Guid guid2 = this.m_containerId.GetGuid((IDataReader) this.Reader, false);
        return new TfsGitRepositoryInfo(this.m_name.GetString((IDataReader) this.Reader, false), new RepoKey(this.m_getDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader)), guid1, guid2));
      }
    }

    private class TfsGitDeletedRepositoryInfoBinder : ObjectBinder<TfsGitDeletedRepositoryInfo>
    {
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_teamProjectUri = new SqlColumnBinder("TeamProjectUri");
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_deletionUserId = new SqlColumnBinder("DeletionUser");
      private SqlColumnBinder m_creationDate = new SqlColumnBinder("CreationDate");
      private SqlColumnBinder m_deletionDate = new SqlColumnBinder("DeletionDate");

      protected override TfsGitDeletedRepositoryInfo Bind() => new TfsGitDeletedRepositoryInfo(this.m_name.GetString((IDataReader) this.Reader, false), new RepoKey(ProjectInfo.GetProjectId(this.m_teamProjectUri.GetString((IDataReader) this.Reader, false)), this.m_repositoryId.GetGuid((IDataReader) this.Reader, false)), this.m_deletionUserId.GetGuid((IDataReader) this.Reader, false), this.m_creationDate.GetDateTime((IDataReader) this.Reader), this.m_deletionDate.GetDateTime((IDataReader) this.Reader));
    }

    private class TfsGitDeletedRepositoryInfoBinder2 : ObjectBinder<TfsGitDeletedRepositoryInfo>
    {
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_deletionUserId = new SqlColumnBinder("DeletionUser");
      private SqlColumnBinder m_creationDate = new SqlColumnBinder("CreationDate");
      private SqlColumnBinder m_deletionDate = new SqlColumnBinder("DeletionDate");
      private readonly System.Func<int, Guid> m_getDataspaceIdentifier;

      public TfsGitDeletedRepositoryInfoBinder2(System.Func<int, Guid> getDataspaceIdentifier) => this.m_getDataspaceIdentifier = getDataspaceIdentifier;

      protected override TfsGitDeletedRepositoryInfo Bind() => new TfsGitDeletedRepositoryInfo(this.m_name.GetString((IDataReader) this.Reader, false), new RepoKey(this.m_getDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader)), this.m_repositoryId.GetGuid((IDataReader) this.Reader, false)), this.m_deletionUserId.GetGuid((IDataReader) this.Reader, false), this.m_creationDate.GetDateTime((IDataReader) this.Reader), this.m_deletionDate.GetDateTime((IDataReader) this.Reader));
    }

    private class TfsGitRefFavoriteBinder : ObjectBinder<TfsGitRefFavorite>
    {
      private SqlColumnBinder m_favoriteId = new SqlColumnBinder("FavoriteId");
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_projectUri = new SqlColumnBinder("TeamProjectUri");
      private SqlColumnBinder m_identityId = new SqlColumnBinder("IdentityId");
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_isFolder = new SqlColumnBinder("IsFolder");

      protected override TfsGitRefFavorite Bind()
      {
        RepoKey repoKey = new RepoKey(ProjectInfo.GetProjectId(this.m_projectUri.GetString((IDataReader) this.Reader, false)), this.m_repositoryId.GetGuid((IDataReader) this.Reader, false));
        return new TfsGitRefFavorite(this.m_favoriteId.GetInt32((IDataReader) this.Reader), this.m_identityId.GetGuid((IDataReader) this.Reader, false), repoKey, this.m_name.GetString((IDataReader) this.Reader, false), this.m_isFolder.GetBoolean((IDataReader) this.Reader, false));
      }
    }

    private class TfsGitRefFavoriteBinder2 : ObjectBinder<TfsGitRefFavorite>
    {
      private SqlColumnBinder m_favoriteId = new SqlColumnBinder("FavoriteId");
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
      private SqlColumnBinder m_identityId = new SqlColumnBinder("IdentityId");
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_isFolder = new SqlColumnBinder("IsFolder");
      private readonly System.Func<int, Guid> m_getDataspaceIdentifier;

      public TfsGitRefFavoriteBinder2(System.Func<int, Guid> getDataspaceIdentifier) => this.m_getDataspaceIdentifier = getDataspaceIdentifier;

      protected override TfsGitRefFavorite Bind()
      {
        RepoKey repoKey = new RepoKey(this.m_getDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader)), this.m_repositoryId.GetGuid((IDataReader) this.Reader, false));
        return new TfsGitRefFavorite(this.m_favoriteId.GetInt32((IDataReader) this.Reader), this.m_identityId.GetGuid((IDataReader) this.Reader, false), repoKey, this.m_name.GetString((IDataReader) this.Reader, false), this.m_isFolder.GetBoolean((IDataReader) this.Reader, false));
      }
    }

    private class TfsGitRepositoryStatsBinder : ObjectBinder<RepoStats>
    {
      private SqlColumnBinder m_commitsCount = new SqlColumnBinder("CommitsCount");
      private SqlColumnBinder m_branchesCount = new SqlColumnBinder("BranchesCount");
      private SqlColumnBinder m_activePullRequestsCount = new SqlColumnBinder("ActivePullRequestsCount");
      private readonly RepoKey m_repoKey;

      public TfsGitRepositoryStatsBinder(RepoKey repoKey) => this.m_repoKey = repoKey;

      protected override RepoStats Bind() => new RepoStats(this.m_repoKey, this.m_commitsCount.GetInt32((IDataReader) this.Reader), this.m_branchesCount.GetInt32((IDataReader) this.Reader), this.m_activePullRequestsCount.GetInt32((IDataReader) this.Reader));
    }

    private class TupleStringStringBinder : ObjectBinder<Tuple<string, string>>
    {
      private SqlColumnBinder m_item1Column;
      private SqlColumnBinder m_item2Column;

      public TupleStringStringBinder(string item1ColumnName, string item2ColumnName)
      {
        this.m_item1Column = new SqlColumnBinder(item1ColumnName);
        this.m_item2Column = new SqlColumnBinder(item2ColumnName);
      }

      protected override Tuple<string, string> Bind() => new Tuple<string, string>(this.m_item1Column.GetString((IDataReader) this.Reader, false), this.m_item2Column.GetString((IDataReader) this.Reader, false));
    }

    private class TupleStringGuidBinder : ObjectBinder<(string, Guid)>
    {
      private SqlColumnBinder m_stringColumn;
      private SqlColumnBinder m_guidColumn;

      public TupleStringGuidBinder(string stringColumnName, string guidColumnName)
      {
        this.m_stringColumn = new SqlColumnBinder(stringColumnName);
        this.m_guidColumn = new SqlColumnBinder(guidColumnName);
      }

      protected override (string, Guid) Bind() => (this.m_stringColumn.GetString((IDataReader) this.Reader, false), this.m_guidColumn.GetGuid((IDataReader) this.Reader, false));
    }
  }
}
