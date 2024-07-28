// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewComponent
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  public class CodeReviewComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[6]
    {
      (IComponentCreator) new ComponentCreator<CodeReviewComponent>(17),
      (IComponentCreator) new ComponentCreator<CodeReviewComponent>(18),
      (IComponentCreator) new ComponentCreator<CodeReviewComponent>((int) sbyte.MaxValue),
      (IComponentCreator) new ComponentCreator<CodeReviewComponent>(133),
      (IComponentCreator) new ComponentCreator<CodeReviewComponent>(1560),
      (IComponentCreator) new ComponentCreator<CodeReviewComponent>(1590)
    }, "CodeReview", CodeReviewServiceBase.DataspaceCategory);
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private static readonly SqlMetaData[] typ_ReviewerInput = new SqlMetaData[3]
    {
      new SqlMetaData("ReviewerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ReviewerType", SqlDbType.SmallInt),
      new SqlMetaData("ReviewerState", SqlDbType.SmallInt)
    };
    private static readonly SqlMetaData[] typ_ReviewerInput3 = new SqlMetaData[4]
    {
      new SqlMetaData("ReviewerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ReviewerType", SqlDbType.SmallInt),
      new SqlMetaData("ReviewerState", SqlDbType.SmallInt),
      new SqlMetaData("IterationId", SqlDbType.Int)
    };

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      dataspaceCategory = serviceVersion > 17 ? CodeReviewServiceBase.DataspaceCategory : "CodeReview";
      base.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
    }

    static CodeReviewComponent()
    {
      CodeReviewComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550001, new SqlExceptionFactory(typeof (CodeReviewNotFoundException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550003, new SqlExceptionFactory(typeof (CodeReviewNotActiveException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550011, new SqlExceptionFactory(typeof (CodeReviewUnexpectedDiffFileIdException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550004, new SqlExceptionFactory(typeof (CodeReviewInvalidStatusChangeException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550101, new SqlExceptionFactory(typeof (CodeReviewIterationNotFoundException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550002, new SqlExceptionFactory(typeof (CodeReviewArgumentNullException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550201, new SqlExceptionFactory(typeof (CodeReviewReviewerSaveFailedUponNullException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1559901, new SqlExceptionFactory(typeof (CodeReviewOperationFailedException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550301, new SqlExceptionFactory(typeof (CodeReviewChangesAlreadyExistException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550302, new SqlExceptionFactory(typeof (CodeReviewChangesWithContentHashNotFoundException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550103, new SqlExceptionFactory(typeof (CodeReviewIterationAlreadyExistsException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550104, new SqlExceptionFactory(typeof (CodeReviewIterationMismatchedIdsException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550105, new SqlExceptionFactory(typeof (CodeReviewIterationCannotBeUnpublishedException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550107, new SqlExceptionFactory(typeof (CodeReviewIterationCannotBePublishedException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550106, new SqlExceptionFactory(typeof (CodeReviewUnpublishedIterationAlreadyExistsException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550202, new SqlExceptionFactory(typeof (ReviewerCannotBeAssociatedWithUnpublishedIterationException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550203, new SqlExceptionFactory(typeof (ReviewerCannotVoteWithoutPublishedIterationException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550501, new SqlExceptionFactory(typeof (CodeReviewStatusNotFoundException)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550502, new SqlExceptionFactory(typeof (CodeReviewTooManyStatusRecords)));
      CodeReviewComponent.s_sqlExceptionFactories.Add(1550005, new SqlExceptionFactory(typeof (CodeReviewArgumentEmptyException)));
    }

    public CodeReviewComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    protected SqlCommand PrepareStoredProcedure(string storedProcedure, Guid projectId)
    {
      SqlCommand sqlCommand = this.PrepareStoredProcedure(string.Format("CodeReview.{0}", (object) storedProcedure));
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      return sqlCommand;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) CodeReviewComponent.s_sqlExceptionFactories;

    public virtual Review SaveReview(Guid projectId, Review review)
    {
      this.PrepareStoredProcedure("prc_SaveReview", projectId);
      if (review.Id > 0)
        this.BindInt("@reviewId", review.Id);
      if (review.Title != null)
        this.BindString("@title", review.Title, 400, false, SqlDbType.NVarChar);
      if (review.Description != null)
        this.BindString("@description", review.Description, 4000, false, SqlDbType.NVarChar);
      if (review.Status.HasValue)
        this.BindByte("@reviewStatus", (byte) review.Status.Value);
      if (review.Author != null)
        this.BindGuid("@author", Guid.Parse(review.Author.Id));
      ReviewStatus? status = review.Status;
      ReviewStatus reviewStatus1 = ReviewStatus.Completed;
      if (!(status.GetValueOrDefault() == reviewStatus1 & status.HasValue))
      {
        status = review.Status;
        ReviewStatus reviewStatus2 = ReviewStatus.Abandoned;
        if (!(status.GetValueOrDefault() == reviewStatus2 & status.HasValue))
          goto label_13;
      }
      this.BindDateTime("@completedDate", DateTime.UtcNow);
label_13:
      if (review.SourceArtifactId != null)
        this.BindString("@sourceArtifactId", review.SourceArtifactId, 400, true, SqlDbType.NVarChar);
      if (!review.IsDeleted)
        this.BindBoolean("@isDeleted", review.IsDeleted);
      this.BindBoolean("@customStorage", review.CustomStorage);
      this.BindNullableInt("@diffFileId", review.DiffFileId);
      this.BindNullableInt("@expectedDiffFileId", review.ExpectedDiffFileId);
      this.BindIterations(review.Iterations);
      this.BindReviewers((IEnumerable<Reviewer>) review.Reviewers);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        DateTime? nullable = new DateTime?(DateTime.MinValue);
        rc.AddBinder<DateTime>((ObjectBinder<DateTime>) new PriorReviewUpdatedTimestampBinder());
        DateTime? lastUpdateTimestamp = this.GetPriorReviewLastUpdateTimestamp(rc, false);
        rc.NextResult();
        rc.AddBinder<Review>((ObjectBinder<Review>) new ReviewBinder(this));
        Review review1 = this.PopulateReviewersIterationsAttachmentsStatuses(rc.GetCurrent<Review>().Items.FirstOrDefault<Review>(), rc, ReviewScope.All);
        review1.PriorReviewUpdatedTimestamp = lastUpdateTimestamp;
        return review1;
      }
    }

    public virtual IEnumerable<Review> GetReviews(
      Guid projectId,
      IEnumerable<int> reviewIds,
      int top,
      int skip,
      bool includeDeleted,
      DateTime? modifiedSince,
      int? maxChangesCount,
      ReviewScope reviewScope = ReviewScope.All)
    {
      this.PrepareStoredProcedure("prc_QueryReviews", projectId);
      if (reviewIds != null && reviewIds.Count<int>() > 0)
        this.BindUniqueInt32Table("@reviewIds", reviewIds);
      this.BindInt("@top", top);
      this.BindInt("@skip", skip);
      this.BindBoolean("@includeDeleted", includeDeleted);
      if (modifiedSince.HasValue)
        this.BindDateTime2("@modifiedSince", modifiedSince.Value.ToUniversalTime());
      if (maxChangesCount.HasValue)
        this.BindInt("@maxChangesCount", maxChangesCount.Value);
      this.BindByte("@reviewScope", (byte) reviewScope);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<Review>((ObjectBinder<Review>) new ReviewBinder(this));
        List<Review> reviews = rc.GetCurrent<Review>().Items;
        if (reviews.Count == 1 && reviewIds != null && reviewIds.Count<int>() == 1)
          reviews[0] = this.PopulateReviewersIterationsAttachmentsStatuses(reviews[0], rc, reviewScope, maxChangesCount);
        else
          reviews = this.PopulateReviewsWithReviewers(reviews, rc);
        return (IEnumerable<Review>) reviews;
      }
    }

    public virtual void DestroyReview(Guid projectId, int reviewId)
    {
      this.PrepareStoredProcedure("prc_DestroyReview", projectId);
      this.BindInt("@reviewId", reviewId);
      this.ExecuteNonQuery();
    }

    public virtual void DeleteReview(Guid projectId, int reviewId)
    {
      this.PrepareStoredProcedure("prc_DeleteReview", projectId);
      this.BindInt("@reviewId", reviewId);
      this.ExecuteNonQuery();
    }

    public virtual IEnumerable<Review> QueryReviewsBySourceArtifactIds(
      Guid projectId,
      IEnumerable<string> sourceArtifactIds,
      bool includeDeleted,
      int? maxChangesCount)
    {
      this.PrepareStoredProcedure("prc_QueryReviewsBySourceArtifactIds", projectId);
      this.BindStringTable("@sourceArtifactIds", sourceArtifactIds, maxLength: 400);
      this.BindBoolean("@includeDeleted", includeDeleted);
      if (maxChangesCount.HasValue)
        this.BindInt("@maxChangesCount", maxChangesCount.Value);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<Review>((ObjectBinder<Review>) new ReviewBinder(this));
        List<Review> reviews = rc.GetCurrent<Review>().Items;
        if (reviews.Count == 1)
          reviews[0] = this.PopulateReviewersIterationsAttachmentsStatuses(reviews[0], rc, ReviewScope.All, maxChangesCount);
        else
          reviews = this.PopulateReviewsWithReviewers(reviews, rc);
        return (IEnumerable<Review>) reviews;
      }
    }

    public virtual IEnumerable<Review> QueryReviewsByFilters(
      Guid? projectId,
      ReviewSearchCriteria searchCriteria,
      int top,
      Guid? creatorId,
      Guid? reviewerId)
    {
      this.PrepareStoredProcedure("CodeReview.prc_QueryReviewsByFilters");
      if (projectId.HasValue)
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId.Value));
      if (searchCriteria.SourceArtifactPrefix != null)
        this.BindString("@sourceArtifactPrefix", searchCriteria.SourceArtifactPrefix, 400, true, SqlDbType.NVarChar);
      ReviewStatus? status = searchCriteria.Status;
      if (status.HasValue)
      {
        status = searchCriteria.Status;
        this.BindByte("@statusFilter", (byte) status.Value);
      }
      if (creatorId.HasValue)
        this.BindGuid("@creatorId", creatorId.Value);
      if (reviewerId.HasValue)
        this.BindGuid("@assignedToId", reviewerId.Value);
      DateTime? nullable = searchCriteria.MinCreatedDate;
      DateTime dateTime;
      if (nullable.HasValue)
      {
        nullable = searchCriteria.MinCreatedDate;
        dateTime = nullable.Value;
        this.BindDateTime2("@minCreatedDate", dateTime.ToUniversalTime());
      }
      nullable = searchCriteria.MaxCreatedDate;
      if (nullable.HasValue)
      {
        nullable = searchCriteria.MaxCreatedDate;
        dateTime = nullable.Value;
        this.BindDateTime2("@maxCreatedDate", dateTime.ToUniversalTime());
      }
      nullable = searchCriteria.MinUpdatedDate;
      if (nullable.HasValue)
      {
        nullable = searchCriteria.MinUpdatedDate;
        dateTime = nullable.Value;
        this.BindDateTime2("@minUpdatedDate", dateTime.ToUniversalTime());
      }
      nullable = searchCriteria.MaxUpdatedDate;
      if (nullable.HasValue)
      {
        nullable = searchCriteria.MaxUpdatedDate;
        dateTime = nullable.Value;
        this.BindDateTime2("@maxUpdatedDate", dateTime.ToUniversalTime());
      }
      bool? orderAscending = searchCriteria.OrderAscending;
      if (orderAscending.HasValue)
      {
        orderAscending = searchCriteria.OrderAscending;
        this.BindBoolean("@orderAscending", (orderAscending.Value ? 1 : 0) != 0);
      }
      this.BindInt("@top", top);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<Review>((ObjectBinder<Review>) new ReviewBinder(this));
        return (IEnumerable<Review>) this.PopulateReviewsWithReviewers(rc.GetCurrent<Review>().Items, rc);
      }
    }

    public virtual UpdateTimestamps MarkReviewAsModified(Guid projectId, int reviewId)
    {
      this.PrepareStoredProcedure("prc_MarkReviewAsModified", projectId);
      this.BindInt("@reviewId", reviewId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<UpdateTimestamps>((ObjectBinder<UpdateTimestamps>) new ReviewUpdatedTimestampBinder());
        return resultCollection.GetCurrent<UpdateTimestamps>().Items.First<UpdateTimestamps>();
      }
    }

    public virtual IEnumerable<Reviewer> GetReviewers(Guid projectId, int reviewId)
    {
      this.PrepareStoredProcedure("prc_QueryReviewers", projectId);
      this.BindInt("@reviewId", reviewId);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return (IEnumerable<Reviewer>) this.GetReviewers(rc);
    }

    public virtual void RemoveReviewers(
      Guid projectId,
      int reviewId,
      IEnumerable<Guid> identityIds,
      IEnumerable<Reviewer> groupReviewers,
      out UpdateTimestamps reviewUpdatedTimestamps)
    {
      reviewUpdatedTimestamps = new UpdateTimestamps();
      this.PrepareStoredProcedure("prc_DeleteReviewers", projectId);
      this.BindInt("@reviewId", reviewId);
      if (identityIds != null)
        this.BindGuidTable("@reviewers", identityIds);
      this.BindReviewers(groupReviewers, "@groupReviewers");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<UpdateTimestamps>((ObjectBinder<UpdateTimestamps>) new ReviewUpdatedTimestampBinder());
        reviewUpdatedTimestamps = resultCollection.GetCurrent<UpdateTimestamps>().Items.First<UpdateTimestamps>();
      }
    }

    public virtual IEnumerable<Reviewer> SaveReviewers(
      Guid projectId,
      int reviewId,
      IEnumerable<Reviewer> reviewers,
      Guid requesterId,
      IEnumerable<Tuple<Guid, Guid>> votedForTable,
      out DateTime? priorReviewUpdatedTimestamp)
    {
      priorReviewUpdatedTimestamp = new DateTime?();
      if (reviewers == null || !reviewers.Any<Reviewer>())
        return (IEnumerable<Reviewer>) Array.Empty<Reviewer>();
      this.PrepareStoredProcedure("prc_SaveReviewers", projectId);
      this.BindInt("@reviewId", reviewId);
      this.BindReviewers(reviewers);
      if (votedForTable.Any<Tuple<Guid, Guid>>())
        this.BindGuidGuidTable("@votedFor", votedForTable);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<Reviewer>((ObjectBinder<Reviewer>) new ReviewerBinder());
        rc.AddBinder<DelegatedVote>((ObjectBinder<DelegatedVote>) new DelegatedVoteBinder());
        rc.AddBinder<DateTime>((ObjectBinder<DateTime>) new PriorReviewUpdatedTimestampBinder());
        List<Reviewer> items1 = rc.GetCurrent<Reviewer>().Items;
        if (rc.TryNextResult())
        {
          List<DelegatedVote> items2 = rc.GetCurrent<DelegatedVote>().Items;
          this.FillDelegatedReviewers(items1, items2);
        }
        priorReviewUpdatedTimestamp = this.GetPriorReviewLastUpdateTimestamp(rc);
        return (IEnumerable<Reviewer>) items1;
      }
    }

    public virtual IEnumerable<Reviewer> ResetAllReviewerStatuses(
      Guid projectId,
      int reviewId,
      out DateTime? priorReviewUpdatedTimestamp)
    {
      priorReviewUpdatedTimestamp = new DateTime?();
      this.PrepareStoredProcedure("prc_ResetAllReviewerStatuses", projectId);
      this.BindInt("@reviewId", reviewId);
      ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      try
      {
        rc.AddBinder<Reviewer>((ObjectBinder<Reviewer>) new ReviewerBinder());
        rc.AddBinder<DelegatedVote>((ObjectBinder<DelegatedVote>) new DelegatedVoteBinder());
        rc.AddBinder<DateTime>((ObjectBinder<DateTime>) new PriorReviewUpdatedTimestampBinder());
        List<Reviewer> items1 = rc.GetCurrent<Reviewer>().Items;
        if (rc.TryNextResult())
        {
          List<DelegatedVote> items2 = rc.GetCurrent<DelegatedVote>().Items;
          this.FillDelegatedReviewers(items1, items2);
        }
        priorReviewUpdatedTimestamp = this.GetPriorReviewLastUpdateTimestamp(rc);
        return (IEnumerable<Reviewer>) items1;
      }
      finally
      {
        rc.Dispose();
      }
    }

    protected virtual void BindReviewers(IEnumerable<Reviewer> reviewers, string paramName = "@reviewers") => this.BindTable(paramName, "CodeReview.typ_ReviewerInput3", this.ConvertReviewersToRows((IEnumerable<Reviewer>) ((object) reviewers ?? (object) Array.Empty<Reviewer>())));

    protected List<Reviewer> GetReviewers(ResultCollection rc)
    {
      rc.AddBinder<Reviewer>((ObjectBinder<Reviewer>) new ReviewerBinder());
      rc.AddBinder<DelegatedVote>((ObjectBinder<DelegatedVote>) new DelegatedVoteBinder());
      List<Reviewer> items1 = rc.GetCurrent<Reviewer>().Items;
      rc.NextResult();
      List<DelegatedVote> items2 = rc.GetCurrent<DelegatedVote>().Items;
      this.FillDelegatedReviewers(items1, items2);
      return items1;
    }

    protected DateTime? GetPriorReviewLastUpdateTimestamp(ResultCollection rc, bool shouldSkip = true)
    {
      if (!shouldSkip)
        return new DateTime?(rc.GetCurrent<DateTime>().Items.FirstOrDefault<DateTime>());
      return rc.TryNextResult() ? new DateTime?(rc.GetCurrent<DateTime>().Items.FirstOrDefault<DateTime>()) : new DateTime?();
    }

    private IEnumerable<SqlDataRecord> ConvertReviewersToRows(IEnumerable<Reviewer> reviewers)
    {
      foreach (Reviewer reviewer in reviewers)
      {
        SqlDataRecord row = new SqlDataRecord(CodeReviewComponent.typ_ReviewerInput3);
        row.SetGuid(0, Guid.Parse(reviewer.Identity.Id));
        if (reviewer.Kind.HasValue)
          row.SetInt16(1, (short) reviewer.Kind.Value);
        if (reviewer.ReviewerStateId.HasValue)
          row.SetInt16(2, reviewer.ReviewerStateId.Value);
        if (reviewer.IterationId.HasValue)
          row.SetInt32(3, reviewer.IterationId.Value);
        yield return row;
      }
    }

    private void FillDelegatedReviewers(
      List<Reviewer> reviewerList,
      List<DelegatedVote> delegatedVotes)
    {
      Dictionary<Guid, Reviewer> dictionary = new Dictionary<Guid, Reviewer>();
      foreach (Guid guid in delegatedVotes.Select<DelegatedVote, Guid>((System.Func<DelegatedVote, Guid>) (delegatedVote => delegatedVote.VotedForId)))
      {
        Guid groupId = guid;
        if (!dictionary.ContainsKey(groupId))
          dictionary.Add(groupId, reviewerList.Where<Reviewer>((System.Func<Reviewer, bool>) (reviewer => reviewer.Identity.Id.Equals(groupId.ToString(), StringComparison.OrdinalIgnoreCase))).FirstOrDefault<Reviewer>());
      }
      foreach (Reviewer reviewer1 in reviewerList)
      {
        Reviewer reviewer = reviewer1;
        if (reviewer.VotedFor == null)
        {
          reviewer.VotedFor = (IList<Guid>) new List<Guid>();
          reviewer.VotedForGroups = (IList<Reviewer>) new List<Reviewer>();
        }
        foreach (DelegatedVote delegatedVote in delegatedVotes.Where<DelegatedVote>((System.Func<DelegatedVote, bool>) (delegatedVote => reviewer.Identity.Id.Equals(delegatedVote.ReviewerId.ToString(), StringComparison.OrdinalIgnoreCase))))
        {
          if (delegatedVote.ReviewId < 0 || delegatedVote.ReviewId > 0 && reviewer.ReviewId == delegatedVote.ReviewId)
          {
            reviewer.VotedFor.Add(delegatedVote.VotedForId);
            reviewer.VotedForGroups.Add(dictionary[delegatedVote.VotedForId]);
          }
        }
      }
    }

    public virtual IList<Iteration> GetIterations(
      Guid projectId,
      int reviewId,
      int? iterationId = null,
      bool includeUnpublished = false)
    {
      this.PrepareStoredProcedure("prc_QueryIterations", projectId);
      this.BindInt("@reviewId", reviewId);
      this.BindBoolean("@includeUnpublished", includeUnpublished);
      if (iterationId.HasValue)
        this.BindInt("@iterationId", iterationId.Value);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return (IList<Iteration>) this.GetIterations(rc);
    }

    public virtual Iteration SaveIteration(
      Guid projectId,
      Iteration iteration,
      bool updateIteration,
      out DateTime? priorReviewUpdatedTimestamp)
    {
      priorReviewUpdatedTimestamp = new DateTime?(DateTime.MinValue);
      IEnumerable<ChangeEntry> changeList = (IEnumerable<ChangeEntry>) iteration.ChangeList;
      bool flag = changeList != null && changeList.Any<ChangeEntry>();
      this.PrepareStoredProcedure("prc_SaveIteration", projectId);
      this.BindInt("@reviewId", iteration.ReviewId);
      this.BindInt("@iterationId", iteration.Id.Value);
      if (iteration.Description != null)
        this.BindString("@description", iteration.Description, 4000, false, SqlDbType.NVarChar);
      if (iteration.Author != null)
        this.BindGuid("@author", Guid.Parse(iteration.Author.Id));
      this.BindBoolean("@updateIteration", updateIteration);
      this.BindBoolean("@isUnpublished", iteration.IsUnpublished);
      if (flag)
        changeList.Bind((TeamFoundationSqlResourceComponent) this, "@changeEntries");
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<Iteration>((ObjectBinder<Iteration>) new IterationBinder());
        Iteration iteration1 = rc.GetCurrent<Iteration>().Items.FirstOrDefault<Iteration>();
        if (this.Version >= (int) sbyte.MaxValue)
        {
          if (flag)
          {
            rc.AddBinder<ChangeEntry>((ObjectBinder<ChangeEntry>) new ChangeEntryBinder());
            rc.NextResult();
            IList<ChangeEntry> items = (IList<ChangeEntry>) rc.GetCurrent<ChangeEntry>().Items;
            iteration1.ChangeList = items;
          }
          if (iteration.Id.Value != 1 | updateIteration)
          {
            rc.AddBinder<DateTime>((ObjectBinder<DateTime>) new PriorReviewUpdatedTimestampBinder());
            priorReviewUpdatedTimestamp = this.GetPriorReviewLastUpdateTimestamp(rc);
          }
        }
        else if (flag)
        {
          rc.AddBinder<ChangeEntry>((ObjectBinder<ChangeEntry>) new ChangeEntryBinder());
          rc.NextResult();
          IList<ChangeEntry> items = (IList<ChangeEntry>) rc.GetCurrent<ChangeEntry>().Items;
          rc.AddBinder<UpdateTimestamps>((ObjectBinder<UpdateTimestamps>) new ReviewUpdatedTimestampBinder());
          rc.NextResult();
          UpdateTimestamps updateTimestamps = rc.GetCurrent<UpdateTimestamps>().Items.First<UpdateTimestamps>();
          iteration1.ChangeList = items;
          iteration1.UpdatedDate = new DateTime?(updateTimestamps.Current);
          priorReviewUpdatedTimestamp = new DateTime?(updateTimestamps.Prior);
        }
        else
        {
          rc.AddBinder<DateTime>((ObjectBinder<DateTime>) new PriorReviewUpdatedTimestampBinder());
          priorReviewUpdatedTimestamp = this.GetPriorReviewLastUpdateTimestamp(rc);
        }
        return iteration1;
      }
    }

    public virtual void DeleteIterations(Guid projectId, int reviewId, int? iterationId = null)
    {
      this.PrepareStoredProcedure("prc_DeleteIterations", projectId);
      this.BindInt("@reviewId", reviewId);
      if (iterationId.HasValue)
        this.BindInt("@iterationId", iterationId.Value);
      this.ExecuteNonQuery();
    }

    public virtual UpdateTimestamps MarkIterationAsModified(
      Guid projectId,
      int reviewId,
      int iterationId)
    {
      this.PrepareStoredProcedure("prc_MarkIterationAsModified", projectId);
      this.BindInt("@reviewId", reviewId);
      this.BindInt("@iterationId", iterationId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<UpdateTimestamps>((ObjectBinder<UpdateTimestamps>) new ReviewUpdatedTimestampBinder());
        return resultCollection.GetCurrent<UpdateTimestamps>().Items.First<UpdateTimestamps>();
      }
    }

    protected List<Iteration> GetIterations(ResultCollection rc)
    {
      rc.AddBinder<Iteration>((ObjectBinder<Iteration>) new IterationBinder());
      return rc.GetCurrent<Iteration>().Items;
    }

    public virtual VirtualResultCollection<ChangeEntry> GetChangeList(
      Guid projectId,
      int reviewId,
      IEnumerable<int> iterationIds,
      int? maxChangesCount = null,
      int? skip = null,
      int? baseIteration = null)
    {
      this.PrepareStoredProcedure("prc_QueryChangeEntries", projectId);
      this.BindInt("@reviewId", reviewId);
      if (iterationIds != null)
        this.BindUniqueInt32Table("@iterationIds", iterationIds);
      if (maxChangesCount.HasValue)
        this.BindInt("@maxChangesCount", maxChangesCount.Value);
      if (skip.HasValue)
        this.BindInt("@skip", skip.Value);
      if (baseIteration.HasValue)
        this.BindInt("@baseIterationId", baseIteration.Value);
      return this.GetChanges(new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext));
    }

    public virtual ContentAccessMetadata ReadChangeEntryAccessMetadata(
      Guid projectId,
      int reviewId,
      int iterationId,
      int changeId,
      ChangeEntryFileType fileType)
    {
      this.PrepareStoredProcedure("prc_ReadChangeEntryFileData", projectId);
      this.BindInt("@reviewId", reviewId);
      this.BindInt("@iterationId", iterationId);
      this.BindInt("@changeId", changeId);
      this.BindShort("@fileType", (short) fileType);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ContentAccessMetadata>((ObjectBinder<ContentAccessMetadata>) new ChangeEntryFileDataBinder());
        return resultCollection.GetCurrent<ContentAccessMetadata>().Items.FirstOrDefault<ContentAccessMetadata>();
      }
    }

    private VirtualResultCollection<ChangeEntry> GetChanges(ResultCollection rc)
    {
      rc.AddBinder<ChangeEntry>((ObjectBinder<ChangeEntry>) new ChangeEntryBinder());
      return new VirtualResultCollection<ChangeEntry>(rc);
    }

    public virtual List<ReviewFileContentInfo> SaveContentMetadata(
      Guid projectId,
      int reviewId,
      List<ContentAccessMetadata> fileInfoToSave,
      ReviewFileType fileType)
    {
      this.PrepareStoredProcedure("prc_SaveContentMetadata", projectId);
      this.BindInt("@reviewId", reviewId);
      fileInfoToSave.Bind(this, "@contentMetadataToSave");
      this.BindShort("@fileType", (short) fileType);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReviewFileContentInfo>((ObjectBinder<ReviewFileContentInfo>) new ReviewFileContentInfoBinder());
        return resultCollection.GetCurrent<ReviewFileContentInfo>().Items;
      }
    }

    public virtual List<ReviewFileContentInfo> ReadContentMetadata(
      Guid projectId,
      int reviewId,
      IEnumerable<byte[]> contentHashes)
    {
      this.PrepareStoredProcedure("prc_QueryContentMetadata", projectId);
      this.BindInt("@reviewId", reviewId);
      contentHashes.Bind(this, nameof (contentHashes));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReviewFileContentInfo>((ObjectBinder<ReviewFileContentInfo>) new ReviewFileContentInfoBinder());
        return resultCollection.GetCurrent<ReviewFileContentInfo>().Items;
      }
    }

    public virtual IList<Attachment> GetAttachments(
      Guid projectId,
      int reviewId,
      int? attachmentId = null,
      DateTime? modifiedSince = null)
    {
      this.PrepareStoredProcedure("prc_QueryAttachments", projectId);
      this.BindInt("@reviewId", reviewId);
      if (attachmentId.HasValue)
        this.BindInt("@attachmentId", attachmentId.Value);
      if (!attachmentId.HasValue && modifiedSince.HasValue)
        this.BindDateTime2("@modifiedSince", modifiedSince.Value.ToUniversalTime());
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return (IList<Attachment>) this.GetAttachments(rc);
    }

    public virtual Attachment SaveAttachment(
      Guid projectId,
      int reviewId,
      Attachment attachment,
      Guid contentId,
      out DateTime? priorReviewUpdatedTimestamp)
    {
      priorReviewUpdatedTimestamp = new DateTime?(DateTime.MinValue);
      this.PrepareStoredProcedure("prc_SaveAttachment", projectId);
      this.BindInt("@reviewId", reviewId);
      this.BindGuid("@contentId", contentId);
      this.BindString("@displayName", attachment.DisplayName, 400, false, SqlDbType.NVarChar);
      if (attachment.Description != null)
        this.BindString("@description", attachment.Description, 4000, false, SqlDbType.NVarChar);
      this.BindGuid("@author", Guid.Parse(attachment.Author.Id));
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<Attachment>((ObjectBinder<Attachment>) new AttachmentBinder());
        Attachment attachment1 = rc.GetCurrent<Attachment>().Items.FirstOrDefault<Attachment>();
        if (rc.TryNextResult())
        {
          rc.AddBinder<DateTime>((ObjectBinder<DateTime>) new PriorReviewUpdatedTimestampBinder());
          priorReviewUpdatedTimestamp = this.GetPriorReviewLastUpdateTimestamp(rc);
        }
        return attachment1;
      }
    }

    public virtual void DeleteAttachments(
      Guid projectId,
      int reviewId,
      out UpdateTimestamps reviewUpdatedTimestamps,
      int? attachmentId = null)
    {
      reviewUpdatedTimestamps = (UpdateTimestamps) null;
      this.PrepareStoredProcedure("prc_DeleteAttachments", projectId);
      this.BindInt("@reviewId", reviewId);
      if (attachmentId.HasValue)
        this.BindInt("@attachmentId", attachmentId.Value);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<UpdateTimestamps>((ObjectBinder<UpdateTimestamps>) new ReviewUpdatedTimestampBinder());
        reviewUpdatedTimestamps = resultCollection.GetCurrent<UpdateTimestamps>().Items.First<UpdateTimestamps>();
      }
    }

    protected List<Attachment> GetAttachments(ResultCollection rc)
    {
      rc.AddBinder<Attachment>((ObjectBinder<Attachment>) new AttachmentBinder());
      return rc.GetCurrent<Attachment>().Items;
    }

    public virtual IList<Status> GetStatuses(
      Guid projectId,
      IEnumerable<int> reviewIds,
      int? iterationId = null,
      int? statusId = null)
    {
      this.PrepareStoredProcedure("prc_QueryStatuses", projectId);
      this.BindUniqueInt32Table("@reviewIds", reviewIds);
      if (iterationId.HasValue)
        this.BindInt("@iterationId", iterationId.Value);
      if (statusId.HasValue)
        this.BindInt("@statusId", statusId.Value);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return (IList<Status>) this.GetStatuses(rc);
    }

    public virtual IEnumerable<StatusContext> GetLatestStatusContextsBySourceArtifactPrefix(
      Guid projectId,
      string sourceArtifactPrefix,
      int topStatuses,
      int topReviews)
    {
      if (this.Version < 133)
      {
        this.PrepareStoredProcedure("prc_QueryLatestStatusesBySourceArtifactId", projectId);
        this.BindString("@sourceArtifactIdPrefix", sourceArtifactPrefix, 400, false, SqlDbType.NVarChar);
        this.BindBoolean("@uniqueByAuthor", false);
        this.BindInt("@top", topStatuses);
      }
      else
      {
        this.PrepareStoredProcedure("prc_QueryLatestStatusContextsBySourceArtifactId", projectId);
        this.BindString("@sourceArtifactIdPrefix", sourceArtifactPrefix, 400, false, SqlDbType.NVarChar);
        this.BindInt("@topStatuses", topStatuses);
        this.BindInt("@topReviews", topReviews);
      }
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return (IEnumerable<StatusContext>) this.GetStatusContexts(rc);
    }

    public virtual IList<Status> GetLatestStatusesByReviewId(Guid projectId, int reviewId, int top = 500)
    {
      if (this.Version < 133)
      {
        this.PrepareStoredProcedure("prc_QueryLatestStatusesBySourceArtifactId", projectId);
        this.BindString("@sourceArtifactIdPrefix", string.Empty, 400, false, SqlDbType.NVarChar);
        this.BindInt("@reviewId", reviewId);
        this.BindBoolean("@uniqueByAuthor", false);
        this.BindInt("@top", top);
      }
      else
      {
        this.PrepareStoredProcedure("prc_QueryLatestStatusesByReviewId", projectId);
        this.BindInt("@reviewId", reviewId);
        this.BindInt("@top", top);
      }
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return (IList<Status>) this.GetStatuses(rc);
    }

    public virtual Status SaveStatus(
      Guid projectId,
      int reviewId,
      int? iterationId,
      Status status,
      bool updateStatus,
      int maxStatusCount,
      out DateTime? priorReviewUpdatedTimestamp)
    {
      priorReviewUpdatedTimestamp = new DateTime?(DateTime.MinValue);
      this.PrepareStoredProcedure("prc_SaveStatus", projectId);
      this.BindInt("@reviewId", reviewId);
      if (status.Author != null)
        this.BindGuid("@author", Guid.Parse(status.Author.Id));
      if (iterationId.HasValue)
        this.BindInt("@iterationId", iterationId.Value);
      this.BindInt("@statusId", status.Id);
      this.BindString("@name", status.Context?.Name, 128, false, SqlDbType.NVarChar);
      if (status.Context?.Genre != null)
        this.BindString("@genre", status.Context.Genre, 128, false, SqlDbType.NVarChar);
      this.BindByte("@state", (byte) status.State);
      if (status.Description != null)
        this.BindString("@description", status.Description, 2048, false, SqlDbType.NVarChar);
      if (status.TargetUrl != null)
        this.BindString("@targetUrl", status.TargetUrl, 2048, false, SqlDbType.NVarChar);
      this.BindBoolean("@updateStatus", updateStatus);
      if (this.Version >= 17)
        this.BindNullableLong("@propertyId", status.PropertyId);
      if (this.Version >= 1560)
        this.BindInt("@maxStatusCount", maxStatusCount);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<Status>((ObjectBinder<Status>) new StatusBinder());
        Status status1 = rc.GetCurrent<Status>().Items.FirstOrDefault<Status>();
        rc.AddBinder<DateTime>((ObjectBinder<DateTime>) new PriorReviewUpdatedTimestampBinder());
        priorReviewUpdatedTimestamp = this.GetPriorReviewLastUpdateTimestamp(rc);
        return status1;
      }
    }

    public virtual List<Status> DeleteStatuses(
      Guid projectId,
      int reviewId,
      int? iterationId,
      IEnumerable<int> statusIdsToDelete,
      out DateTime priorReviewUpdatedTimestamp,
      out DateTime lastUpdatedTimestamp)
    {
      if (this.Version < 17)
        throw new NotSupportedException(CodeReviewResources.StatusesDeleteNotSupported());
      this.PrepareStoredProcedure("prc_DeleteStatuses", projectId);
      this.BindInt("@reviewId", reviewId);
      if (iterationId.HasValue)
        this.BindInt("@iterationId", iterationId.Value);
      this.BindUniqueInt32Table("@statusIds", statusIdsToDelete);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        List<Status> statuses = this.GetStatuses(rc);
        rc.NextResult();
        rc.AddBinder<UpdateTimestamps>((ObjectBinder<UpdateTimestamps>) new ReviewUpdatedTimestampBinder());
        UpdateTimestamps updateTimestamps = rc.GetCurrent<UpdateTimestamps>().Items.First<UpdateTimestamps>();
        lastUpdatedTimestamp = updateTimestamps.Current;
        priorReviewUpdatedTimestamp = updateTimestamps.Prior;
        return statuses;
      }
    }

    protected virtual List<Status> GetStatuses(ResultCollection rc)
    {
      rc.AddBinder<Status>((ObjectBinder<Status>) new StatusBinder());
      return rc.GetCurrent<Status>().Items;
    }

    protected virtual List<StatusContext> GetStatusContexts(ResultCollection rc)
    {
      rc.AddBinder<StatusContext>((ObjectBinder<StatusContext>) new StatusContextBinder());
      return rc.GetCurrent<StatusContext>().Items;
    }

    public virtual IEnumerable<ArtifactVisit> QueryVisits(
      IdentityRef identity,
      IEnumerable<string> artifactIds,
      bool includeViewedState)
    {
      this.PrepareStoredProcedure("CodeReview.prc_QueryVisits");
      this.BindString("@identityId", identity.Id, 128, false, SqlDbType.NVarChar);
      this.BindStringTable("@artifactIds", artifactIds, maxLength: 400);
      if (this.Version >= 1590)
        this.BindBoolean("@includeViewedState", includeViewedState);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ArtifactVisit>((ObjectBinder<ArtifactVisit>) new ArtifactVisitBinder());
        List<ArtifactVisit> items = resultCollection.GetCurrent<ArtifactVisit>().Items;
        items?.ForEach((Action<ArtifactVisit>) (v => v.User = identity));
        return (IEnumerable<ArtifactVisit>) items;
      }
    }

    public virtual ArtifactVisit UpdateLastVisit(IdentityRef identity, string artifactId)
    {
      this.PrepareStoredProcedure("CodeReview.prc_SaveLastVisit");
      this.BindString("@identityId", identity.Id, 128, false, SqlDbType.NVarChar);
      this.BindString("@artifactId", artifactId, 400, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ArtifactVisit>((ObjectBinder<ArtifactVisit>) new ArtifactVisitBinder());
        ArtifactVisit artifactVisit = resultCollection.GetCurrent<ArtifactVisit>().Items.First<ArtifactVisit>();
        if (artifactVisit != null)
          artifactVisit.User = identity;
        return artifactVisit;
      }
    }

    public virtual ArtifactVisit UpdateViewedState(
      IdentityRef identity,
      string artifactId,
      string viewedState)
    {
      if (this.Version < 1590)
        throw new NotSupportedException(CodeReviewResources.ViewedStatusNotSupported());
      this.PrepareStoredProcedure("CodeReview.prc_SaveViewedState");
      this.BindString("@identityId", identity.Id, 128, false, SqlDbType.NVarChar);
      this.BindString("@artifactId", artifactId, 400, false, SqlDbType.NVarChar);
      this.BindString("@viewedState", viewedState, -1, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ArtifactVisit>((ObjectBinder<ArtifactVisit>) new ArtifactVisitBinder());
        ArtifactVisit artifactVisit = resultCollection.GetCurrent<ArtifactVisit>().Items.First<ArtifactVisit>();
        if (artifactVisit != null)
          artifactVisit.User = identity;
        return artifactVisit;
      }
    }

    protected Review PopulateReviewersIterationsAttachmentsStatuses(
      Review review,
      ResultCollection rc,
      ReviewScope reviewScope,
      int? maxChangeCount = null)
    {
      if (rc.TryNextResult())
        review.Iterations = (IList<Iteration>) this.GetIterations(rc);
      if (reviewScope == ReviewScope.ReviewLevelOnly || reviewScope == ReviewScope.ReviewPlusIterations)
        return review;
      if (maxChangeCount.HasValue)
        this.PopulateChanges(review, rc, maxChangeCount);
      if (reviewScope == ReviewScope.ReviewPlusIterationsPlusChanges)
        return review;
      if (rc.TryNextResult())
        review.Reviewers = (IList<Reviewer>) this.GetReviewers(rc);
      if (rc.TryNextResult())
        review.Attachments = (IList<Attachment>) this.GetAttachments(rc);
      if (rc.TryNextResult())
        this.PopulateStatuses(review, this.GetStatuses(rc));
      return review;
    }

    private void PopulateChanges(Review review, ResultCollection rc, int? maxChangeCount = null)
    {
      if (review.Iterations == null || !rc.TryNextResult() || !maxChangeCount.HasValue)
        return;
      VirtualResultCollection<ChangeEntry> changes = this.GetChanges(rc);
      Dictionary<int, int> dictionary = new Dictionary<int, int>();
      for (int index = 0; index < review.Iterations.Count; ++index)
      {
        dictionary.Add(review.Iterations[index].Id.Value, index);
        review.Iterations[index].ChangeList = (IList<ChangeEntry>) new List<ChangeEntry>();
      }
      foreach (ChangeEntry changeEntry in changes.GetCurrent<ChangeEntry>())
      {
        int index;
        if (dictionary.TryGetValue(changeEntry.IterationId.Value, out index))
          review.Iterations[index].ChangeList.Add(changeEntry);
      }
    }

    private void PopulateStatuses(Review review, List<Status> statuses)
    {
      Dictionary<int, List<Status>> statusMapping = new Dictionary<int, List<Status>>();
      List<Status> statusList1 = new List<Status>();
      foreach (Status statuse in statuses)
      {
        int? iterationId = statuse.IterationId;
        if (iterationId.HasValue)
        {
          Dictionary<int, List<Status>> dictionary1 = statusMapping;
          iterationId = statuse.IterationId;
          int key1 = iterationId.Value;
          if (!dictionary1.ContainsKey(key1))
          {
            Dictionary<int, List<Status>> dictionary2 = statusMapping;
            iterationId = statuse.IterationId;
            int key2 = iterationId.Value;
            List<Status> statusList2 = new List<Status>();
            dictionary2.Add(key2, statusList2);
          }
          Dictionary<int, List<Status>> dictionary3 = statusMapping;
          iterationId = statuse.IterationId;
          int key3 = iterationId.Value;
          dictionary3[key3].Add(statuse);
        }
        else
          statusList1.Add(statuse);
      }
      foreach (Iteration iteration in review.Iterations.Where<Iteration>((System.Func<Iteration, bool>) (iter => statusMapping.ContainsKey(iter.Id.Value))))
        iteration.Statuses = (IList<Status>) statusMapping[iteration.Id.Value];
      review.Statuses = (IList<Status>) statusList1;
    }

    private void BindIterations(IList<Iteration> iterations)
    {
      if (iterations == null || iterations.Count == 0)
        return;
      Iteration iteration = iterations.First<Iteration>();
      if (iteration.Description != null)
        this.BindString("@iterationDescription", iteration.Description, 4000, false, SqlDbType.NVarChar);
      if (iteration.Author != null)
        this.BindGuid("@author", Guid.Parse(iteration.Author.Id));
      this.BindBoolean("@iterationIsUnpublished", iteration.IsUnpublished);
      IList<ChangeEntry> changeList = iteration.ChangeList;
      if (changeList == null)
        return;
      changeList.Bind((TeamFoundationSqlResourceComponent) this, "@changeEntries");
    }

    private List<Review> PopulateReviewsWithReviewers(List<Review> reviews, ResultCollection rc)
    {
      if (rc.TryNextResult())
      {
        List<Reviewer> reviewers = this.GetReviewers(rc);
        Dictionary<int, List<Reviewer>> reviewerMapping = new Dictionary<int, List<Reviewer>>();
        foreach (Reviewer reviewer in reviewers)
        {
          if (!reviewerMapping.ContainsKey(reviewer.ReviewId))
            reviewerMapping.Add(reviewer.ReviewId, new List<Reviewer>());
          reviewerMapping[reviewer.ReviewId].Add(reviewer);
        }
        foreach (Review review in reviews.Where<Review>((System.Func<Review, bool>) (review => reviewerMapping.ContainsKey(review.Id))))
          review.Reviewers = (IList<Reviewer>) reviewerMapping[review.Id];
      }
      return reviews;
    }
  }
}
