// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.DiscussionSqlResourceComponent
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  internal class DiscussionSqlResourceComponent : TeamFoundationSqlResourceComponent
  {
    protected const string CodeReviewDataSpaceCategory = "Git";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[6]
    {
      (IComponentCreator) new ComponentCreator<DiscussionSqlResourceComponent>(13),
      (IComponentCreator) new ComponentCreator<DiscussionSqlResourceComponent>(14),
      (IComponentCreator) new ComponentCreator<DiscussionSqlResourceComponent>(15),
      (IComponentCreator) new ComponentCreator<DiscussionSqlResourceComponent>(16),
      (IComponentCreator) new ComponentCreator<DiscussionSqlResourceComponent>(17),
      (IComponentCreator) new ComponentCreator<DiscussionSqlResourceComponent>(18)
    }, "Discussion");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1050004,
        new SqlExceptionFactory(typeof (DiscussionThreadNotFoundException))
      },
      {
        1050005,
        new SqlExceptionFactory(typeof (CommentNotFoundException))
      },
      {
        1050006,
        new SqlExceptionFactory(typeof (CommentCannotBeUpdatedException))
      }
    };

    public DiscussionSqlResourceComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    public override void Dispose()
    {
      base.Dispose();
      GC.SuppressFinalize((object) this);
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) DiscussionSqlResourceComponent.s_sqlExceptionFactories;

    protected override string TraceArea => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.TraceArea.Discussion;

    internal List<DiscussionThread> PublishDiscussions(
      DiscussionThread[] discussionsToSave,
      DiscussionComment[] commentsToSave,
      CommentId[] deletedComments,
      Guid? requestingIdentity,
      Guid projectId,
      int reviewId,
      out List<DiscussionComment> comments)
    {
      this.TraceEnter(600282, nameof (PublishDiscussions));
      this.PrepareStoredProcedure("prc_PublishDiscussions");
      this.BindReview(projectId, reviewId);
      this.BindDiscussionTable(discussionsToSave);
      this.BindCommentTable(commentsToSave);
      this.BindCommentIdTable(deletedComments);
      if (requestingIdentity.HasValue)
        this.BindGuid("@requestingIdentity", requestingIdentity.Value);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<Tuple<int, DateTime>>((ObjectBinder<Tuple<int, DateTime>>) new DiscussionPriorTimestampBinder());
        rc.AddBinder<DiscussionThread>(this.GetThreadBinder());
        rc.AddBinder<DiscussionComment>(this.GetCommentBinder());
        rc.AddBinder<DateTime>((ObjectBinder<DateTime>) new DiscussionSqlResourceComponent.LastUpdatedDateBinder());
        List<Tuple<int, DateTime>> items = rc.GetCurrent<Tuple<int, DateTime>>().Items;
        rc.NextResult();
        List<DiscussionThread> savedDiscussions = this.ReadQueryResults(rc, out comments);
        this.UpdatePriorUpdateTimestamp(savedDiscussions, items);
        return savedDiscussions;
      }
    }

    internal List<DiscussionThread> QueryDiscussionsByVersions(
      IEnumerable<string> versionIds,
      out List<DiscussionComment> comments)
    {
      this.TraceEnter(600140, nameof (QueryDiscussionsByVersions));
      this.PrepareStoredProcedure("prc_QueryDiscussionsByVersions", 3600);
      this.BindStringTable("@versionIds", versionIds, maxLength: 2083);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        comments = new List<DiscussionComment>();
        rc.AddBinder<DiscussionThread>(this.GetThreadBinder());
        rc.AddBinder<DiscussionComment>(this.GetCommentNoContentBinder());
        List<DiscussionThread> discussionThreadList = this.ReadQueryResults(rc, out comments);
        this.TraceLeave(600149, "QueryDiscussionsByVersion");
        return discussionThreadList;
      }
    }

    internal List<DiscussionThread> QueryDiscussionsByVersion(
      string versionId,
      DateTime? modifiedSince,
      out List<DiscussionComment> comments)
    {
      this.TraceEnter(600140, nameof (QueryDiscussionsByVersion));
      this.PrepareStoredProcedure("prc_QueryDiscussionsByVersion", 3600);
      this.BindString("@versionId", versionId, 2083, false, SqlDbType.NVarChar);
      if (modifiedSince.HasValue)
        this.BindDateTime2("@modifiedSince", modifiedSince.Value.ToUniversalTime());
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<DiscussionThread>(this.GetThreadBinder());
        rc.AddBinder<DiscussionComment>(this.GetCommentBinder());
        rc.AddBinder<SocialAuthor>((ObjectBinder<SocialAuthor>) new SocialAuthorBinder());
        List<DiscussionThread> discussionThreadList = this.ReadQueryResults(rc, out comments);
        this.PopulateUserLikes(rc, comments);
        this.TraceLeave(600149, nameof (QueryDiscussionsByVersion));
        return discussionThreadList;
      }
    }

    internal int CountDiscussionsByVersion(string versionId, DateTime? modifiedSince)
    {
      this.TraceEnter(600140, nameof (CountDiscussionsByVersion));
      if (this.Version < 18)
      {
        this.TraceAlways(1380000, TraceLevel.Warning, string.Format("DB version {0} is lower than 18 so CountDiscussionsByVersion is not supported. Falling back to QueryDiscussionsByVersion implementation", (object) this.Version));
        return this.QueryDiscussionsByVersion(versionId, modifiedSince, out List<DiscussionComment> _).Count;
      }
      this.PrepareStoredProcedure("prc_QueryGitPullRequestsV2");
      this.PrepareStoredProcedure("prc_CountDiscussionsByVersion", 3600);
      this.BindString("@versionId", versionId, 2083, false, SqlDbType.NVarChar);
      if (modifiedSince.HasValue)
        this.BindDateTime2("@modifiedSince", modifiedSince.Value.ToUniversalTime());
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new DiscussionSqlResourceComponent.DiscussionsCountBinder());
        ObjectBinder<int> current = resultCollection.GetCurrent<int>();
        if (!current.MoveNext())
        {
          this.Trace(600161, TraceLevel.Error, "Expected results from prc_CountDiscussionsByVersion");
          throw new UnexpectedDatabaseResultException("prc_CountDiscussionsByVersion");
        }
        this.TraceLeave(600149, nameof (CountDiscussionsByVersion));
        return current.Current;
      }
    }

    internal List<DiscussionThread> QueryDiscussionsByCodeReviewRequest(
      int workItemId,
      out List<DiscussionComment> comments)
    {
      this.TraceEnter(600150, nameof (QueryDiscussionsByCodeReviewRequest));
      this.PrepareStoredProcedure("prc_QueryDiscussionsByCodeReviewRequest");
      this.BindInt("@workItemId", workItemId);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<DiscussionThread>(this.GetThreadBinder());
        rc.AddBinder<DiscussionComment>(this.GetCommentBinder());
        List<DiscussionThread> discussionThreadList = this.ReadQueryResults(rc, out comments);
        this.TraceLeave(600154, nameof (QueryDiscussionsByCodeReviewRequest));
        return discussionThreadList;
      }
    }

    internal List<DiscussionThread> QueryDiscussionsById(
      int discussionId,
      out List<DiscussionComment> comments)
    {
      this.TraceEnter(600155, nameof (QueryDiscussionsById));
      this.PrepareStoredProcedure("prc_QueryDiscussionsById");
      this.BindInt("@discussionId", discussionId);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<DiscussionThread>(this.GetThreadBinder());
        rc.AddBinder<DiscussionComment>(this.GetCommentBinder());
        rc.AddBinder<SocialAuthor>((ObjectBinder<SocialAuthor>) new SocialAuthorBinder());
        List<DiscussionThread> discussionThreadList = this.ReadQueryResults(rc, out comments);
        this.PopulateUserLikes(rc, comments);
        this.TraceLeave(600157, nameof (QueryDiscussionsById));
        return discussionThreadList;
      }
    }

    internal DiscussionComment UpdateDiscussionComment(
      DiscussionComment newComment,
      Guid projectId,
      int reviewId,
      Guid requesterId,
      out List<DiscussionThread> updatedDiscussions)
    {
      this.TraceEnter(600158, nameof (UpdateDiscussionComment));
      this.PrepareStoredProcedure("prc_UpdateDiscussionComment");
      this.BindReview(projectId, reviewId);
      this.BindRequesterId(requesterId);
      this.BindInt("@discussionId", newComment.DiscussionId);
      this.BindInt("@commentId", (int) newComment.CommentId);
      this.BindString("@content", newComment.Content, -1, false, SqlDbType.NVarChar);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<DiscussionThread>(this.GetThreadBinder());
        rc.AddBinder<DiscussionComment>(this.GetCommentBinder());
        rc.AddBinder<DateTime>((ObjectBinder<DateTime>) new DiscussionSqlResourceComponent.PriorLastUpdatedDateBinder());
        List<DiscussionComment> comments;
        updatedDiscussions = this.ReadQueryResults(rc, out comments);
        DiscussionComment discussionComment = comments.FirstOrDefault<DiscussionComment>();
        rc.NextResult();
        foreach (DiscussionThread discussionThread in updatedDiscussions)
          discussionThread.PriorLastUpdatedDate = rc.GetCurrent<DateTime>().Items[0];
        this.TraceLeave(600159, nameof (UpdateDiscussionComment));
        return discussionComment;
      }
    }

    internal long QueryReplicationState()
    {
      this.TraceEnter(600160, nameof (QueryReplicationState));
      this.PrepareStoredProcedure("prc_QueryDiscussionDbReplicationState");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<long>((ObjectBinder<long>) new DiscussionSqlResourceComponent.DestroyedWorkItemWaterMarkBinder());
        ObjectBinder<long> current = resultCollection.GetCurrent<long>();
        if (!current.MoveNext())
        {
          this.Trace(600161, TraceLevel.Error, "Expected results from prc_QueryDiscussionDbReplicationState");
          throw new UnexpectedDatabaseResultException("prc_QueryDiscussionDbReplicationState");
        }
        this.TraceLeave(600169, nameof (QueryReplicationState));
        return current.Current;
      }
    }

    internal void UpdateReplicationState(long destroyedWorkItemWaterMark)
    {
      this.TraceEnter(600170, nameof (UpdateReplicationState));
      this.PrepareStoredProcedure("prc_UpdateDiscussionDbReplicationState");
      this.BindLong("@destroyedWorkItemWaterMark", destroyedWorkItemWaterMark);
      this.ExecuteNonQuery();
      this.TraceLeave(600179, nameof (UpdateReplicationState));
    }

    internal List<int> CleanupDiscussions(List<int> destroyedWorkItems)
    {
      if (destroyedWorkItems.Count == 0)
        return new List<int>(0);
      this.PrepareStoredProcedure("prc_CleanupDiscussions");
      this.BindInt32Table("@destroyedWorkItemsTable", (IEnumerable<int>) destroyedWorkItems);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => new SqlColumnBinder("DiscussionId").GetInt32(reader))));
        if (!resultCollection.TryNextResult())
          return new List<int>(0);
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => new SqlColumnBinder("PropertyId").GetInt32(reader))));
        return resultCollection.GetCurrent<int>().Items;
      }
    }

    internal List<SocialAuthor> SaveSocialAuthors(
      List<SocialAuthor> socialAuthors,
      Guid projectId,
      int reviewId,
      out List<DiscussionThread> discussions,
      out List<DiscussionComment> comments)
    {
      this.TraceEnter(600277, nameof (SaveSocialAuthors));
      if (socialAuthors == null || socialAuthors.Count == 0)
      {
        discussions = new List<DiscussionThread>(0);
        comments = new List<DiscussionComment>(0);
        return new List<SocialAuthor>(0);
      }
      this.PrepareStoredProcedure("prc_SaveSocial");
      this.BindReview(projectId, reviewId);
      this.BindInt("@discussionId", socialAuthors[0].DiscussionId);
      this.BindInt("@commentId", (int) socialAuthors[0].CommentId);
      this.BindGuidTable("@authors", (IEnumerable<Guid>) socialAuthors.Select<SocialAuthor, Guid>((System.Func<SocialAuthor, Guid>) (author => author.GetAuthorId())).ToList<Guid>());
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<SocialAuthor>((ObjectBinder<SocialAuthor>) new SocialAuthorBinder());
        rc.AddBinder<DiscussionThread>(this.GetThreadBinder());
        rc.AddBinder<DiscussionComment>(this.GetCommentBinder());
        rc.AddBinder<DateTime>((ObjectBinder<DateTime>) new DiscussionSqlResourceComponent.PriorLastUpdatedDateBinder());
        List<SocialAuthor> items = rc.GetCurrent<SocialAuthor>().Items;
        rc.NextResult();
        discussions = this.ReadQueryResults(rc, out comments);
        rc.NextResult();
        foreach (DiscussionThread discussionThread in discussions)
          discussionThread.PriorLastUpdatedDate = rc.GetCurrent<DateTime>().Items[0];
        return items;
      }
    }

    internal List<SocialAuthor> QuerySocialAuthors(int discussionId, short commentId)
    {
      this.TraceEnter(600278, nameof (QuerySocialAuthors));
      this.PrepareStoredProcedure("prc_QuerySocial");
      this.BindInt("@discussionId", discussionId);
      this.BindInt("@commentId", (int) commentId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SocialAuthor>((ObjectBinder<SocialAuthor>) new SocialAuthorBinder());
        return resultCollection.GetCurrent<SocialAuthor>().Items;
      }
    }

    internal void DeleteSocialAuthors(
      int discussionId,
      short commentId,
      Guid author,
      Guid projectId,
      int reviewId,
      out List<DiscussionThread> discussions,
      out List<DiscussionComment> comments,
      out List<SocialAuthor> allAuthors)
    {
      this.TraceEnter(600279, nameof (DeleteSocialAuthors));
      this.PrepareStoredProcedure("prc_DeleteSocial");
      this.BindInt("@discussionId", discussionId);
      this.BindInt("@commentId", (int) commentId);
      this.BindGuid("@author", author);
      this.BindReview(projectId, reviewId);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<DiscussionThread>(this.GetThreadBinder());
        rc.AddBinder<DiscussionComment>(this.GetCommentBinder());
        rc.AddBinder<SocialAuthor>((ObjectBinder<SocialAuthor>) new SocialAuthorBinder());
        rc.AddBinder<DateTime>((ObjectBinder<DateTime>) new DiscussionSqlResourceComponent.PriorLastUpdatedDateBinder());
        discussions = this.ReadQueryResults(rc, out comments);
        rc.NextResult();
        allAuthors = rc.GetCurrent<SocialAuthor>().Items;
        rc.NextResult();
        foreach (DiscussionThread discussionThread in discussions)
          discussionThread.PriorLastUpdatedDate = rc.GetCurrent<DateTime>().Items[0];
      }
    }

    internal List<DiscussionThread> GetDiscussionThreadsChangedSinceLastWatermark(
      DateTime lastUpdatedDate,
      int discussionId,
      int batchSize)
    {
      if (this.Version < 17)
        return new List<DiscussionThread>();
      this.PrepareStoredProcedure("prc_GetDiscussionThreadsChangedSinceLastWatermark");
      this.BindDateTime2("@lastUpdatedDate", lastUpdatedDate);
      this.BindInt("@discussionId", discussionId);
      this.BindInt("@batchSize", batchSize);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DiscussionThread>(this.GetThreadBinder());
        return resultCollection.GetCurrent<DiscussionThread>().Items;
      }
    }

    internal List<DiscussionComment> GetDiscussionCommentsChangedSinceLastWatermark(
      DateTime lastUpdatedDate,
      int discussionId,
      int commentId,
      int batchSize)
    {
      if (this.Version < 17)
        return new List<DiscussionComment>();
      this.PrepareStoredProcedure("prc_GetDiscussionCommentsChangedSinceLastWatermark");
      this.BindDateTime2("@lastUpdatedDate", lastUpdatedDate);
      this.BindInt("@discussionId", discussionId);
      this.BindInt("@commentId", commentId);
      this.BindInt("@batchSize", batchSize);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<DiscussionComment>(this.GetCommentBinder());
        rc.AddBinder<SocialAuthor>((ObjectBinder<SocialAuthor>) new SocialAuthorBinder());
        List<DiscussionComment> items = rc.GetCurrent<DiscussionComment>().Items;
        this.PopulateUserLikes(rc, items);
        return items;
      }
    }

    protected List<DiscussionThread> ReadQueryResults(
      ResultCollection rc,
      out List<DiscussionComment> comments)
    {
      this.TraceEnter(600190, nameof (ReadQueryResults));
      List<DiscussionThread> items = rc.GetCurrent<DiscussionThread>().Items;
      rc.NextResult();
      comments = rc.GetCurrent<DiscussionComment>().Items;
      this.TraceLeave(600199, nameof (ReadQueryResults));
      return items;
    }

    protected void PopulateUserLikes(ResultCollection rc, List<DiscussionComment> comments)
    {
      if (!rc.TryNextResult() || comments == null || comments.Count <= 0)
        return;
      List<SocialAuthor> items = rc.GetCurrent<SocialAuthor>().Items;
      foreach (DiscussionComment comment in comments)
      {
        comment.UsersLiked = new List<IdentityRef>();
        foreach (SocialAuthor socialAuthor in items)
        {
          if (socialAuthor.DiscussionId == comment.DiscussionId && (int) socialAuthor.CommentId == (int) comment.CommentId)
            comment.UsersLiked.Add(socialAuthor.Identity);
        }
      }
    }

    protected void UpdatePriorUpdateTimestamp(
      List<DiscussionThread> savedDiscussions,
      List<Tuple<int, DateTime>> discussionTimestamps)
    {
      Dictionary<int, DateTime> timestampMapping = new Dictionary<int, DateTime>();
      foreach (Tuple<int, DateTime> tuple in discussionTimestamps.Where<Tuple<int, DateTime>>((System.Func<Tuple<int, DateTime>, bool>) (d => !timestampMapping.ContainsKey(d.Item1))))
        timestampMapping.Add(tuple.Item1, tuple.Item2);
      foreach (DiscussionThread savedDiscussion in savedDiscussions)
        savedDiscussion.PriorLastUpdatedDate = timestampMapping.ContainsKey(savedDiscussion.DiscussionId) ? timestampMapping[savedDiscussion.DiscussionId] : savedDiscussion.LastUpdatedDate;
    }

    protected ObjectBinder<DiscussionComment> GetCommentBinder() => this.Version >= 15 ? (ObjectBinder<DiscussionComment>) new DiscussionSqlResourceComponent.CommentBinder3() : (ObjectBinder<DiscussionComment>) new DiscussionSqlResourceComponent.CommentBinder2();

    protected ObjectBinder<DiscussionComment> GetCommentNoContentBinder() => this.Version >= 15 ? (ObjectBinder<DiscussionComment>) new DiscussionSqlResourceComponent.CommentBinderNoContent2() : (ObjectBinder<DiscussionComment>) new DiscussionSqlResourceComponent.CommentBinderNoContent();

    protected ObjectBinder<DiscussionThread> GetThreadBinder() => this.Version >= 14 ? (ObjectBinder<DiscussionThread>) new DiscussionSqlResourceComponent.DiscussionThreadBinder3() : (ObjectBinder<DiscussionThread>) new DiscussionSqlResourceComponent.DiscussionThreadBinder2();

    protected void BindReview(Guid projectId, int reviewId)
    {
      if (reviewId <= 0)
        return;
      string dataspaceCategory = this.Version <= 15 ? "CodeReview" : "Git";
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, dataspaceCategory));
      this.BindInt("@reviewId", reviewId);
    }

    protected void BindRequesterId(Guid requesterId) => this.BindGuid("@requesterId", requesterId);

    protected override SqlCommand PrepareStoredProcedure(string storedProcedure) => base.PrepareStoredProcedure(string.Format("CodeReview.{0}", (object) storedProcedure));

    protected override SqlCommand PrepareStoredProcedure(string storedProcedure, int timeOut) => base.PrepareStoredProcedure(string.Format("CodeReview.{0}", (object) storedProcedure), timeOut);

    protected void BindDiscussionTable(DiscussionThread[] discussions) => discussions.Bind(this, "@discussionsTable");

    protected void BindCommentTable(DiscussionComment[] comments) => comments.Bind(this, "@commentsTable");

    protected void BindCommentIdTable(CommentId[] commentIds) => ((IEnumerable<CommentId>) commentIds).Bind(this, "@deleteCommentIdsTable");

    private sealed class DestroyedWorkItemWaterMarkBinder : ObjectBinder<long>
    {
      private SqlColumnBinder DestroyedWorkItemWaterMark = new SqlColumnBinder(nameof (DestroyedWorkItemWaterMark));

      protected override long Bind() => this.DestroyedWorkItemWaterMark.GetInt64((IDataReader) this.Reader);
    }

    protected sealed class LastUpdatedDateBinder : ObjectBinder<DateTime>
    {
      private SqlColumnBinder LastUpdatedDate = new SqlColumnBinder(nameof (LastUpdatedDate));

      protected override DateTime Bind() => this.LastUpdatedDate.GetDateTime((IDataReader) this.Reader);
    }

    protected sealed class PriorLastUpdatedDateBinder : ObjectBinder<DateTime>
    {
      private SqlColumnBinder priorLastUpdatedDate = new SqlColumnBinder("PriorLastUpdatedDate");

      protected override DateTime Bind() => this.priorLastUpdatedDate.GetDateTime((IDataReader) this.Reader);
    }

    protected class CommentBinder2 : ObjectBinder<DiscussionComment>
    {
      private SqlColumnBinder DiscussionId = new SqlColumnBinder(nameof (DiscussionId));
      private SqlColumnBinder CommentId = new SqlColumnBinder(nameof (CommentId));
      private SqlColumnBinder ParentCommentId = new SqlColumnBinder(nameof (ParentCommentId));
      private SqlColumnBinder Author = new SqlColumnBinder(nameof (Author));
      private SqlColumnBinder CommentType = new SqlColumnBinder(nameof (CommentType));
      private SqlColumnBinder Content = new SqlColumnBinder(nameof (Content));
      private SqlColumnBinder PublishedDate = new SqlColumnBinder(nameof (PublishedDate));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));
      private SqlColumnBinder LastUpdatedDate = new SqlColumnBinder(nameof (LastUpdatedDate));

      protected override DiscussionComment Bind()
      {
        DiscussionComment discussionComment = new DiscussionComment()
        {
          DiscussionId = this.DiscussionId.GetInt32((IDataReader) this.Reader),
          CommentId = this.CommentId.GetInt16((IDataReader) this.Reader),
          ParentCommentId = this.ParentCommentId.GetInt16((IDataReader) this.Reader),
          Author = new IdentityRef()
          {
            Id = this.Author.GetGuid((IDataReader) this.Reader).ToString()
          },
          CommentType = (CommentType) this.CommentType.GetByte((IDataReader) this.Reader),
          Content = this.Content.GetString((IDataReader) this.Reader, false),
          PublishedDate = this.PublishedDate.GetDateTime((IDataReader) this.Reader),
          IsDeleted = this.IsDeleted.GetBoolean((IDataReader) this.Reader),
          LastUpdatedDate = this.LastUpdatedDate.GetDateTime((IDataReader) this.Reader)
        };
        discussionComment.LastContentUpdatedDate = discussionComment.LastUpdatedDate;
        return discussionComment;
      }
    }

    protected class CommentBinder3 : ObjectBinder<DiscussionComment>
    {
      private SqlColumnBinder DiscussionId = new SqlColumnBinder(nameof (DiscussionId));
      private SqlColumnBinder CommentId = new SqlColumnBinder(nameof (CommentId));
      private SqlColumnBinder ParentCommentId = new SqlColumnBinder(nameof (ParentCommentId));
      private SqlColumnBinder Author = new SqlColumnBinder(nameof (Author));
      private SqlColumnBinder CommentType = new SqlColumnBinder(nameof (CommentType));
      private SqlColumnBinder Content = new SqlColumnBinder(nameof (Content));
      private SqlColumnBinder PublishedDate = new SqlColumnBinder(nameof (PublishedDate));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));
      private SqlColumnBinder LastUpdatedDate = new SqlColumnBinder(nameof (LastUpdatedDate));
      private SqlColumnBinder LastContentUpdatedDate = new SqlColumnBinder(nameof (LastContentUpdatedDate));

      protected override DiscussionComment Bind()
      {
        DiscussionComment discussionComment = new DiscussionComment()
        {
          DiscussionId = this.DiscussionId.GetInt32((IDataReader) this.Reader),
          CommentId = this.CommentId.GetInt16((IDataReader) this.Reader),
          ParentCommentId = this.ParentCommentId.GetInt16((IDataReader) this.Reader),
          Author = new IdentityRef()
          {
            Id = this.Author.GetGuid((IDataReader) this.Reader).ToString()
          },
          CommentType = (CommentType) this.CommentType.GetByte((IDataReader) this.Reader),
          Content = this.Content.GetString((IDataReader) this.Reader, false),
          PublishedDate = this.PublishedDate.GetDateTime((IDataReader) this.Reader),
          IsDeleted = this.IsDeleted.GetBoolean((IDataReader) this.Reader),
          LastUpdatedDate = this.LastUpdatedDate.GetDateTime((IDataReader) this.Reader),
          LastContentUpdatedDate = this.LastContentUpdatedDate.GetDateTime((IDataReader) this.Reader)
        };
        DateTime contentUpdatedDate = discussionComment.LastContentUpdatedDate;
        return discussionComment;
      }
    }

    protected class CommentBinderNoContent : ObjectBinder<DiscussionComment>
    {
      private SqlColumnBinder DiscussionId = new SqlColumnBinder(nameof (DiscussionId));
      private SqlColumnBinder CommentId = new SqlColumnBinder(nameof (CommentId));
      private SqlColumnBinder ParentCommentId = new SqlColumnBinder(nameof (ParentCommentId));
      private SqlColumnBinder Author = new SqlColumnBinder(nameof (Author));
      private SqlColumnBinder CommentType = new SqlColumnBinder(nameof (CommentType));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));
      private SqlColumnBinder LastUpdatedDate = new SqlColumnBinder(nameof (LastUpdatedDate));

      protected override DiscussionComment Bind()
      {
        DiscussionComment discussionComment = new DiscussionComment()
        {
          DiscussionId = this.DiscussionId.GetInt32((IDataReader) this.Reader),
          CommentId = this.CommentId.GetInt16((IDataReader) this.Reader),
          ParentCommentId = this.ParentCommentId.GetInt16((IDataReader) this.Reader),
          Author = new IdentityRef()
          {
            Id = this.Author.GetGuid((IDataReader) this.Reader).ToString()
          },
          CommentType = (CommentType) this.CommentType.GetByte((IDataReader) this.Reader),
          IsDeleted = this.IsDeleted.GetBoolean((IDataReader) this.Reader),
          LastUpdatedDate = this.LastUpdatedDate.GetDateTime((IDataReader) this.Reader)
        };
        discussionComment.LastContentUpdatedDate = discussionComment.LastUpdatedDate;
        return discussionComment;
      }
    }

    protected class CommentBinderNoContent2 : ObjectBinder<DiscussionComment>
    {
      private SqlColumnBinder DiscussionId = new SqlColumnBinder(nameof (DiscussionId));
      private SqlColumnBinder CommentId = new SqlColumnBinder(nameof (CommentId));
      private SqlColumnBinder ParentCommentId = new SqlColumnBinder(nameof (ParentCommentId));
      private SqlColumnBinder Author = new SqlColumnBinder(nameof (Author));
      private SqlColumnBinder CommentType = new SqlColumnBinder(nameof (CommentType));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));
      private SqlColumnBinder LastUpdatedDate = new SqlColumnBinder(nameof (LastUpdatedDate));
      private SqlColumnBinder LastContentUpdatedDate = new SqlColumnBinder(nameof (LastContentUpdatedDate));

      protected override DiscussionComment Bind()
      {
        DiscussionComment discussionComment = new DiscussionComment()
        {
          DiscussionId = this.DiscussionId.GetInt32((IDataReader) this.Reader),
          CommentId = this.CommentId.GetInt16((IDataReader) this.Reader),
          ParentCommentId = this.ParentCommentId.GetInt16((IDataReader) this.Reader),
          Author = new IdentityRef()
          {
            Id = this.Author.GetGuid((IDataReader) this.Reader).ToString()
          },
          CommentType = (CommentType) this.CommentType.GetByte((IDataReader) this.Reader),
          IsDeleted = this.IsDeleted.GetBoolean((IDataReader) this.Reader),
          LastUpdatedDate = this.LastUpdatedDate.GetDateTime((IDataReader) this.Reader),
          LastContentUpdatedDate = this.LastContentUpdatedDate.GetDateTime((IDataReader) this.Reader)
        };
        DateTime contentUpdatedDate = discussionComment.LastContentUpdatedDate;
        return discussionComment;
      }
    }

    protected sealed class DiscussionThreadBinder2 : ObjectBinder<DiscussionThread>
    {
      private SqlColumnBinder DiscussionId = new SqlColumnBinder(nameof (DiscussionId));
      private SqlColumnBinder Status = new SqlColumnBinder(nameof (Status));
      private SqlColumnBinder Severity = new SqlColumnBinder(nameof (Severity));
      private SqlColumnBinder WorkItemId = new SqlColumnBinder(nameof (WorkItemId));
      private SqlColumnBinder VersionId = new SqlColumnBinder(nameof (VersionId));
      private SqlColumnBinder PublishedDate = new SqlColumnBinder(nameof (PublishedDate));
      private SqlColumnBinder LastUpdatedDate = new SqlColumnBinder(nameof (LastUpdatedDate));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder PropertyId = new SqlColumnBinder(nameof (PropertyId));
      private SqlColumnBinder CommentsCount = new SqlColumnBinder(nameof (CommentsCount));

      protected override DiscussionThread Bind()
      {
        ArtifactDiscussionThread discussionThread = new ArtifactDiscussionThread();
        discussionThread.DiscussionId = this.DiscussionId.GetInt32((IDataReader) this.Reader);
        discussionThread.Status = (DiscussionStatus) this.Status.GetByte((IDataReader) this.Reader);
        discussionThread.Severity = (DiscussionSeverity) this.Severity.GetByte((IDataReader) this.Reader);
        discussionThread.WorkItemId = this.WorkItemId.GetInt32((IDataReader) this.Reader);
        discussionThread.VersionId = this.VersionId.GetString((IDataReader) this.Reader, false);
        discussionThread.PublishedDate = this.PublishedDate.GetDateTime((IDataReader) this.Reader);
        discussionThread.LastUpdatedDate = this.LastUpdatedDate.GetDateTime((IDataReader) this.Reader);
        discussionThread.Revision = this.Revision.GetInt32((IDataReader) this.Reader);
        discussionThread.PropertyId = this.PropertyId.GetInt32((IDataReader) this.Reader);
        discussionThread.CommentsCount = this.CommentsCount.GetInt32((IDataReader) this.Reader, 0, 0);
        return (DiscussionThread) discussionThread;
      }
    }

    protected sealed class DiscussionThreadBinder3 : ObjectBinder<DiscussionThread>
    {
      private SqlColumnBinder DiscussionId = new SqlColumnBinder(nameof (DiscussionId));
      private SqlColumnBinder Status = new SqlColumnBinder(nameof (Status));
      private SqlColumnBinder Severity = new SqlColumnBinder(nameof (Severity));
      private SqlColumnBinder WorkItemId = new SqlColumnBinder(nameof (WorkItemId));
      private SqlColumnBinder VersionId = new SqlColumnBinder(nameof (VersionId));
      private SqlColumnBinder PublishedDate = new SqlColumnBinder(nameof (PublishedDate));
      private SqlColumnBinder LastUpdatedDate = new SqlColumnBinder(nameof (LastUpdatedDate));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder PropertyId = new SqlColumnBinder(nameof (PropertyId));

      protected override DiscussionThread Bind()
      {
        ArtifactDiscussionThread discussionThread = new ArtifactDiscussionThread();
        discussionThread.DiscussionId = this.DiscussionId.GetInt32((IDataReader) this.Reader);
        discussionThread.Status = (DiscussionStatus) this.Status.GetByte((IDataReader) this.Reader);
        discussionThread.Severity = (DiscussionSeverity) this.Severity.GetByte((IDataReader) this.Reader);
        discussionThread.WorkItemId = this.WorkItemId.GetInt32((IDataReader) this.Reader);
        discussionThread.VersionId = this.VersionId.GetString((IDataReader) this.Reader, false);
        discussionThread.PublishedDate = this.PublishedDate.GetDateTime((IDataReader) this.Reader);
        discussionThread.LastUpdatedDate = this.LastUpdatedDate.GetDateTime((IDataReader) this.Reader);
        discussionThread.Revision = this.Revision.GetInt32((IDataReader) this.Reader);
        discussionThread.PropertyId = this.PropertyId.GetInt32((IDataReader) this.Reader);
        return (DiscussionThread) discussionThread;
      }
    }

    private sealed class DiscussionsCountBinder : ObjectBinder<int>
    {
      private SqlColumnBinder DiscussionsCount = new SqlColumnBinder(nameof (DiscussionsCount));

      protected override int Bind() => this.DiscussionsCount.GetInt32((IDataReader) this.Reader);
    }
  }
}
