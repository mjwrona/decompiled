// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.TeamFoundationDiscussionService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.Events;
using Microsoft.VisualStudio.Services.CodeReview.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  public class TeamFoundationDiscussionService : 
    ITeamFoundationDiscussionService,
    IVssFrameworkService
  {
    private IDisposableReadOnlyList<IDiscussionArtifactPlugin> m_artifactPlugins;
    public static readonly Guid DiscussionArtifactKind = new Guid("D49CB716-2170-49DB-BA5A-4A660275E730");

    internal TeamFoundationDiscussionService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => this.m_artifactPlugins = systemRequestContext.GetExtensions<IDiscussionArtifactPlugin>();

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_artifactPlugins == null)
        return;
      this.m_artifactPlugins.Dispose();
      this.m_artifactPlugins = (IDisposableReadOnlyList<IDiscussionArtifactPlugin>) null;
    }

    public List<int> PublishDiscussions(
      IVssRequestContext requestContext,
      DiscussionThread[] discussions,
      DiscussionComment[] comments,
      CommentId[] deletedComments,
      out List<short> commentIds,
      out DateTime lastUpdatedDate)
    {
      requestContext.TraceEnter(600000, TraceArea.Discussion, TraceLayer.Service, nameof (PublishDiscussions));
      ArgumentValidator.CheckNull((object) discussions, nameof (discussions));
      ArgumentValidator.CheckNullCollection<DiscussionThread>((IEnumerable<DiscussionThread>) discussions, "discussions.Item", false);
      IdentityRef crIdentityRef = requestContext.GetUserIdentity().ToCRIdentityRef(requestContext);
      if (crIdentityRef == null)
        throw new IdentityNotFoundException(requestContext.UserContext);
      if (comments != null)
      {
        ArgumentValidator.CheckNullCollection<DiscussionComment>((IEnumerable<DiscussionComment>) comments, "comments.Item");
        foreach (DiscussionComment comment in comments)
        {
          ArgumentValidator.CheckNull(comment.Content, "comment.Content");
          if (comment.Author?.Id != null && !comment.Author.Id.Equals(crIdentityRef.Id))
            throw new ArgumentException(Resources.CommentAuthorMustBeRequester, "comment.Author");
        }
      }
      else
        comments = Array.Empty<DiscussionComment>();
      bool deletedCommentsOnly = false;
      Guid? requestingIdentity = new Guid?();
      if (deletedComments != null)
      {
        ArgumentValidator.CheckNullCollection<CommentId>((IEnumerable<CommentId>) deletedComments, "deletedComments.Item");
        if (!this.HasDeletePermission(requestContext))
          requestingIdentity = new Guid?(new Guid(crIdentityRef.Id));
        if (comments != null && comments.Length == 0)
          deletedCommentsOnly = true;
      }
      else
        deletedComments = Array.Empty<CommentId>();
      foreach (DiscussionComment comment in comments)
      {
        if (comment.Author?.Id == null)
          comment.Author = crIdentityRef;
      }
      this.PrepareDiscussions(requestContext, discussions, comments, deletedComments);
      this.ParseVersionUris(requestContext, discussions);
      SecurityManager.CheckPermission(requestContext, (IEnumerable<DiscussionThread>) discussions, DiscussionPermissions.Contribute, true);
      this.CheckMaxDiscussions(requestContext, discussions);
      IEnumerable<DiscussionThread> source1 = ((IEnumerable<DiscussionThread>) discussions).Where<DiscussionThread>((Func<DiscussionThread, bool>) (x => x.DiscussionId < 0));
      int num = (int) requestContext.GetService<ITeamFoundationCounterService>().ReserveCounterIds(requestContext, "DiscussionThreadPropertyId", (long) source1.Count<DiscussionThread>());
      Guid projectId;
      int reviewId;
      DiscussionExtensions.ExtractMetadata(((IEnumerable<DiscussionThread>) discussions).FirstOrDefault<DiscussionThread>().ArtifactUri, out projectId, out reviewId);
      List<ArtifactPropertyValue> source2 = new List<ArtifactPropertyValue>();
      foreach (DiscussionThread discussionThread in source1)
      {
        IDiscussionArtifactPlugin discussionArtifactPlugin = (IDiscussionArtifactPlugin) null;
        ArtifactId artifact = LinkingUtilities.DecodeUri(discussionThread.ArtifactUri);
        foreach (IDiscussionArtifactPlugin artifactPlugin in this.ArtifactPlugins)
        {
          if (artifactPlugin.CanResolveArtifactId(artifact))
          {
            discussionArtifactPlugin = artifactPlugin;
            break;
          }
        }
        discussionThread.PropertyId = num++;
        if (discussionThread.Properties != null)
        {
          List<PropertyValue> source3 = new List<PropertyValue>();
          foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) discussionThread.Properties)
          {
            object obj = property.Value;
            if (property.Key == "Microsoft.TeamFoundation.Discussion.ItemPath" && discussionArtifactPlugin != null)
              obj = (object) discussionArtifactPlugin.TranslateIncomingPath(requestContext, (string) property.Value);
            source3.Add(new PropertyValue(property.Key, obj));
          }
          if (source3.Any<PropertyValue>())
            source2.Add(new ArtifactPropertyValue(TeamFoundationDiscussionService.DiscussionArtifactKind, discussionThread.PropertyId, 0, (IEnumerable<PropertyValue>) source3));
        }
      }
      if (source2.Any<ArtifactPropertyValue>())
        requestContext.GetService<ITeamFoundationPropertyService>().SetProperties(requestContext, (IEnumerable<ArtifactPropertyValue>) source2);
      List<DiscussionComment> comments1;
      List<DiscussionThread> discussionThreadList;
      List<int> list;
      using (DiscussionSqlResourceComponent component = requestContext.CreateComponent<DiscussionSqlResourceComponent>("VersionControl"))
      {
        discussionThreadList = component.PublishDiscussions(discussions, comments, deletedComments, requestingIdentity, projectId, reviewId, out comments1);
        list = discussionThreadList.Select<DiscussionThread, int>((Func<DiscussionThread, int>) (d => d.DiscussionId)).ToList<int>();
        commentIds = comments1.Select<DiscussionComment, short>((Func<DiscussionComment, short>) (c => c.CommentId)).ToList<short>();
        lastUpdatedDate = discussionThreadList.First<DiscussionThread>().LastUpdatedDate;
      }
      this.PostPublishDiscussions(requestContext, discussionThreadList, comments1, comments1.ToArray(), (DiscussionComment[]) null, deletedComments, deletedCommentsOnly);
      requestContext.TraceLeave(600004, TraceArea.Discussion, TraceLayer.Service, nameof (PublishDiscussions));
      return list;
    }

    public List<DiscussionThread> PublishDiscussions(
      IVssRequestContext requestContext,
      DiscussionThread[] discussionsToSave,
      DiscussionComment[] commentsToSave,
      CommentId[] deletedComments)
    {
      requestContext.TraceEnter(600283, TraceArea.Discussion, TraceLayer.Service, nameof (PublishDiscussions));
      if (commentsToSave == null)
        commentsToSave = Array.Empty<DiscussionComment>();
      Guid? requestingIdentity;
      int reviewId;
      Guid projectId;
      bool deletedCommentsOnly = this.PreparePublishingDiscussions(requestContext, discussionsToSave, commentsToSave, deletedComments, out requestingIdentity, out reviewId, out projectId);
      this.CheckMaxDiscussions(requestContext, discussionsToSave);
      List<DiscussionComment> comments;
      List<DiscussionThread> savedDiscussions;
      using (DiscussionSqlResourceComponent component = requestContext.CreateComponent<DiscussionSqlResourceComponent>("VersionControl"))
        savedDiscussions = component.PublishDiscussions(discussionsToSave, commentsToSave, deletedComments, requestingIdentity, projectId, reviewId, out comments);
      DiscussionComment[] array;
      if (comments.Count == commentsToSave.Length)
      {
        array = comments.ToArray();
      }
      else
      {
        List<DiscussionComment> discussionCommentList = new List<DiscussionComment>();
        foreach (DiscussionComment comment in commentsToSave)
        {
          DiscussionComment commentInList = TeamFoundationDiscussionService.FindCommentInList(comment, comments);
          if (commentInList != null)
            discussionCommentList.Add(commentInList);
        }
        array = discussionCommentList.ToArray();
      }
      this.PostPublishDiscussions(requestContext, savedDiscussions, comments, array, (DiscussionComment[]) null, deletedComments, deletedCommentsOnly);
      requestContext.TraceLeave(600284, TraceArea.Discussion, TraceLayer.Service, nameof (PublishDiscussions));
      return savedDiscussions;
    }

    private void CheckMaxDiscussions(
      IVssRequestContext requestContext,
      DiscussionThread[] discussions)
    {
      if (discussions == null || ((IEnumerable<DiscussionThread>) discussions).Count<DiscussionThread>() == 0)
        return;
      int maxDiscussionThreadCount = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/CodeReview/MaxDiscussionsPerArtifact", true, 10000);
      IEnumerable<DiscussionThread> source = ((IEnumerable<DiscussionThread>) discussions).Where<DiscussionThread>((Func<DiscussionThread, bool>) (x => x.DiscussionId < 0));
      foreach (string str in source.Select<DiscussionThread, string>((Func<DiscussionThread, string>) (discussion => discussion.ArtifactUri)).Distinct<string>().ToArray<string>())
      {
        string artifactUri = str;
        int currentCount = this.CountDiscussionsByArtifactUri(requestContext, artifactUri);
        int addedCount = source.Where<DiscussionThread>((Func<DiscussionThread, bool>) (discussion => discussion.ArtifactUri == artifactUri)).Count<DiscussionThread>();
        if (currentCount + addedCount > maxDiscussionThreadCount)
          throw new MaxDiscussionThreadCountException(artifactUri, maxDiscussionThreadCount, currentCount, addedCount);
      }
    }

    public Dictionary<string, IEnumerable<DiscussionThread>> QueryDiscussionsByArtifactUris(
      IVssRequestContext requestContext,
      string[] artifactUris)
    {
      requestContext.TraceEnter(600005, TraceArea.Discussion, TraceLayer.Service, nameof (QueryDiscussionsByArtifactUris));
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (string versionUri1 in ((IEnumerable<string>) artifactUris).Distinct<string>((IEqualityComparer<string>) TFStringComparer.ArtiFactUrl))
      {
        string versionUri2 = this.ParseVersionUri(requestContext, versionUri1);
        dictionary1.Add(versionUri2, versionUri1);
      }
      List<DiscussionComment> comments;
      List<DiscussionThread> discussionThreadList;
      using (DiscussionSqlResourceComponent component = requestContext.CreateComponent<DiscussionSqlResourceComponent>("VersionControl"))
        discussionThreadList = component.QueryDiscussionsByVersions((IEnumerable<string>) dictionary1.Keys, out comments);
      this.PopulateThreadsWithComments(requestContext, (IEnumerable<DiscussionThread>) discussionThreadList, (IList<DiscussionComment>) comments);
      this.PostQueryDiscussions(requestContext, discussionThreadList, comments);
      Dictionary<string, IEnumerable<DiscussionThread>> dictionary2 = new Dictionary<string, IEnumerable<DiscussionThread>>();
      foreach (IGrouping<string, DiscussionThread> source in discussionThreadList.GroupBy<DiscussionThread, string>((Func<DiscussionThread, string>) (x => x.ArtifactUri), (IEqualityComparer<string>) TFStringComparer.ArtiFactUrl))
      {
        DiscussionThread discussionThread = source.FirstOrDefault<DiscussionThread>();
        if (discussionThread != null)
        {
          string key = dictionary1[discussionThread.VersionId];
          dictionary2[key] = (IEnumerable<DiscussionThread>) source;
        }
      }
      requestContext.TraceLeave(600009, TraceArea.Discussion, TraceLayer.Service, nameof (QueryDiscussionsByArtifactUris));
      return dictionary2;
    }

    private static DiscussionComment FindCommentInList(
      DiscussionComment comment,
      List<DiscussionComment> list)
    {
      foreach (DiscussionComment commentInList in list)
      {
        if (commentInList.GetAuthorId() == comment.GetAuthorId() && string.Compare(commentInList.Content, comment.Content) == 0)
          return commentInList;
      }
      return (DiscussionComment) null;
    }

    public List<DiscussionThread> QueryDiscussionsByArtifactUri(
      IVssRequestContext requestContext,
      string artifactUri,
      out List<DiscussionComment> comments,
      out IdentityRef[] authors)
    {
      List<DiscussionThread> discussionThreadList = this.QueryDiscussionsByArtifactUri(requestContext, artifactUri, out comments, new DateTime?(), true);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> mappedIdentities = IdentityHelper.GetMappedIdentities(requestContext, (IList<Guid>) comments.Select<DiscussionComment, Guid>((Func<DiscussionComment, Guid>) (c => c.GetAuthorId())).Distinct<Guid>().ToList<Guid>());
      authors = mappedIdentities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>) (x => x.ToCRIdentityRef(requestContext))).ToArray<IdentityRef>();
      return discussionThreadList;
    }

    public List<DiscussionThread> QueryDiscussionsByArtifactUri(
      IVssRequestContext requestContext,
      string artifactUri,
      out List<DiscussionComment> comments,
      DateTime? modifiedSince = null,
      bool includeExtendedProperties = true)
    {
      requestContext.TraceEnter(600010, TraceArea.Discussion, TraceLayer.Service, nameof (QueryDiscussionsByArtifactUri));
      ArgumentValidator.CheckNull(artifactUri, nameof (artifactUri));
      string versionUri = this.ParseVersionUri(requestContext, artifactUri);
      List<DiscussionThread> discussionThreadList;
      using (DiscussionSqlResourceComponent component = requestContext.CreateComponent<DiscussionSqlResourceComponent>("VersionControl"))
        discussionThreadList = component.QueryDiscussionsByVersion(versionUri, modifiedSince, out comments);
      this.PopulateThreadsWithComments(requestContext, (IEnumerable<DiscussionThread>) discussionThreadList, (IList<DiscussionComment>) comments);
      this.PostQueryDiscussions(requestContext, discussionThreadList, comments, includeExtendedProperties);
      requestContext.TraceLeave(600019, TraceArea.Discussion, TraceLayer.Service, nameof (QueryDiscussionsByArtifactUri));
      return discussionThreadList;
    }

    public int CountDiscussionsByArtifactUri(
      IVssRequestContext requestContext,
      string artifactUri,
      DateTime? modifiedSince = null)
    {
      requestContext.TraceEnter(600010, TraceArea.Discussion, TraceLayer.Service, nameof (CountDiscussionsByArtifactUri));
      ArgumentValidator.CheckNull(artifactUri, nameof (artifactUri));
      string versionUri = this.ParseVersionUri(requestContext, artifactUri);
      using (DiscussionSqlResourceComponent component = requestContext.CreateComponent<DiscussionSqlResourceComponent>("VersionControl"))
      {
        int num = component.CountDiscussionsByVersion(versionUri, modifiedSince);
        requestContext.TraceLeave(600019, TraceArea.Discussion, TraceLayer.Service, nameof (CountDiscussionsByArtifactUri));
        return num;
      }
    }

    public List<DiscussionThread> QueryDiscussionsByCodeReviewRequest(
      IVssRequestContext requestContext,
      int workItemId,
      out List<DiscussionComment> comments,
      out IdentityRef[] authors)
    {
      List<DiscussionThread> discussionThreadList = this.QueryDiscussionsByCodeReviewRequest(requestContext, workItemId, out comments);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> mappedIdentities = IdentityHelper.GetMappedIdentities(requestContext, (IList<Guid>) comments.Select<DiscussionComment, Guid>((Func<DiscussionComment, Guid>) (c => c.GetAuthorId())).Distinct<Guid>().ToList<Guid>());
      authors = mappedIdentities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>) (x => x.ToCRIdentityRef(requestContext))).ToArray<IdentityRef>();
      return discussionThreadList;
    }

    public List<DiscussionThread> QueryDiscussionsByCodeReviewRequest(
      IVssRequestContext requestContext,
      int workItemId,
      out List<DiscussionComment> comments)
    {
      requestContext.TraceEnter(600020, TraceArea.Discussion, TraceLayer.Service, nameof (QueryDiscussionsByCodeReviewRequest));
      ArgumentValidator.Check(workItemId > 0, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArgumentInvalidError, (object) nameof (workItemId)));
      List<DiscussionThread> discussions;
      using (DiscussionSqlResourceComponent component = requestContext.CreateComponent<DiscussionSqlResourceComponent>("VersionControl"))
        discussions = component.QueryDiscussionsByCodeReviewRequest(workItemId, out comments);
      this.PostQueryDiscussions(requestContext, discussions, comments);
      requestContext.TraceLeave(600029, TraceArea.Discussion, TraceLayer.Service, nameof (QueryDiscussionsByCodeReviewRequest));
      return discussions;
    }

    public DiscussionThread QueryDiscussionsById(
      IVssRequestContext requestContext,
      int discussionId,
      out List<DiscussionComment> comments,
      bool includeExtendedProperties = true)
    {
      requestContext.TraceEnter(600020, TraceArea.Discussion, TraceLayer.Service, nameof (QueryDiscussionsById));
      ArgumentValidator.Check(discussionId > 0, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArgumentInvalidError, (object) nameof (discussionId)));
      List<DiscussionThread> discussionThreadList;
      using (DiscussionSqlResourceComponent component = requestContext.CreateComponent<DiscussionSqlResourceComponent>("VersionControl"))
        discussionThreadList = component.QueryDiscussionsById(discussionId, out comments);
      this.PopulateThreadsWithComments(requestContext, (IEnumerable<DiscussionThread>) discussionThreadList, (IList<DiscussionComment>) comments);
      this.PostQueryDiscussions(requestContext, discussionThreadList, comments, includeExtendedProperties);
      requestContext.TraceLeave(600024, TraceArea.Discussion, TraceLayer.Service, nameof (QueryDiscussionsById));
      return discussionThreadList.FirstOrDefault<DiscussionThread>();
    }

    public List<DiscussionThread> GetDiscussionThreadsChangedSinceLastWatermark(
      IVssRequestContext requestContext,
      DateTime lastUpdatedDate,
      int discussionId,
      int batchSize)
    {
      List<DiscussionThread> sinceLastWatermark;
      using (DiscussionSqlResourceComponent component = requestContext.CreateComponent<DiscussionSqlResourceComponent>("VersionControl"))
        sinceLastWatermark = component.GetDiscussionThreadsChangedSinceLastWatermark(lastUpdatedDate, discussionId, batchSize);
      int val1 = 5000;
      for (int index = 0; index < sinceLastWatermark.Count; index += val1)
        this.PostQueryDiscussions(requestContext, sinceLastWatermark.GetRange(index, Math.Min(val1, sinceLastWatermark.Count - index)), new List<DiscussionComment>());
      return sinceLastWatermark;
    }

    public List<DiscussionComment> GetDiscussionCommentsChangedSinceLastWatermark(
      IVssRequestContext requestContext,
      DateTime lastUpdatedDate,
      int discussionId,
      int commentId,
      int batchSize)
    {
      using (DiscussionSqlResourceComponent component = requestContext.CreateComponent<DiscussionSqlResourceComponent>("VersionControl"))
        return component.GetDiscussionCommentsChangedSinceLastWatermark(lastUpdatedDate, discussionId, commentId, batchSize);
    }

    public DiscussionComment UpdateDiscussionComment(
      IVssRequestContext requestContext,
      DiscussionThread thread,
      DiscussionComment newComment)
    {
      requestContext.TraceEnter(600025, TraceArea.Discussion, TraceLayer.Service, nameof (UpdateDiscussionComment));
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (userIdentity == null)
        throw new IdentityNotFoundException(requestContext.UserContext);
      ArgumentValidator.ValidateComment(newComment);
      if (string.IsNullOrEmpty(thread.ArtifactUri))
        thread = this.QueryDiscussionsById(requestContext, thread.DiscussionId, out List<DiscussionComment> _, true);
      SecurityManager.CheckPermission(requestContext, (IEnumerable<DiscussionThread>) new DiscussionThread[1]
      {
        thread
      }, DiscussionPermissions.Contribute, true);
      Guid projectId;
      int reviewId;
      DiscussionExtensions.ExtractMetadata(thread.ArtifactUri, out projectId, out reviewId);
      DiscussionComment discussionComment = (DiscussionComment) null;
      List<DiscussionThread> updatedDiscussions;
      using (DiscussionSqlResourceComponent component = requestContext.CreateComponent<DiscussionSqlResourceComponent>("VersionControl"))
        discussionComment = component.UpdateDiscussionComment(newComment, projectId, reviewId, userIdentity.Id, out updatedDiscussions);
      if (updatedDiscussions.Count == 0)
      {
        updatedDiscussions = new List<DiscussionThread>()
        {
          thread
        };
        thread.PriorLastUpdatedDate = thread.LastUpdatedDate;
        thread.LastUpdatedDate = discussionComment.LastUpdatedDate;
      }
      List<DiscussionComment> comments = new List<DiscussionComment>()
      {
        discussionComment
      };
      this.PopulateThreadsWithComments(requestContext, (IEnumerable<DiscussionThread>) updatedDiscussions, (IList<DiscussionComment>) comments);
      this.PostQueryDiscussions(requestContext, updatedDiscussions, comments);
      IVssRequestContext requestContext1 = requestContext;
      DiscussionsNotification notificationEvent = new DiscussionsNotification(updatedDiscussions.ToArray(), comments.ToArray(), (DiscussionComment[]) null);
      notificationEvent.HasUpdatedCommentsOnly = true;
      string discussion = TraceArea.Discussion;
      string service = TraceLayer.Service;
      EventServiceHelper.PublishNotification(requestContext1, notificationEvent, discussion, service);
      requestContext.TraceLeave(600029, TraceArea.Discussion, TraceLayer.Service, nameof (UpdateDiscussionComment));
      return discussionComment;
    }

    public long QueryReplicationState(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(600271, TraceArea.Discussion, TraceLayer.Service, nameof (QueryReplicationState));
      long num;
      using (DiscussionSqlResourceComponent component = requestContext.CreateComponent<DiscussionSqlResourceComponent>("VersionControl"))
        num = component.QueryReplicationState();
      requestContext.TraceLeave(600272, TraceArea.Discussion, TraceLayer.Service, nameof (QueryReplicationState));
      return num;
    }

    public bool CleanupDiscussions(
      IVssRequestContext requestContext,
      List<int> destroyedWorkItems,
      long highWaterMark,
      long newHighWaterMark)
    {
      requestContext.TraceEnter(600273, TraceArea.Discussion, TraceLayer.Service, nameof (CleanupDiscussions));
      List<int> source;
      using (DiscussionSqlResourceComponent component = requestContext.CreateComponent<DiscussionSqlResourceComponent>("VersionControl"))
      {
        source = component.CleanupDiscussions(destroyedWorkItems);
        if (!StringComparer.Ordinal.Equals((object) highWaterMark, (object) newHighWaterMark))
        {
          requestContext.Trace(600274, TraceLevel.Info, TraceArea.Discussion, TraceLayer.Service, "Updating watermark '{0}' to '{1}'", (object) highWaterMark, (object) newHighWaterMark);
          component.UpdateReplicationState(newHighWaterMark);
        }
      }
      if (source.Count > 0)
        requestContext.GetService<ITeamFoundationPropertyService>().DeleteArtifacts(requestContext, source.Select<int, ArtifactSpec>((Func<int, ArtifactSpec>) (propId => new ArtifactSpec(TeamFoundationDiscussionService.DiscussionArtifactKind, propId, 0))));
      requestContext.TraceLeave(600275, TraceArea.Discussion, TraceLayer.Service, nameof (CleanupDiscussions));
      return true;
    }

    public List<IdentityRef> LikeComment(
      IVssRequestContext requestContext,
      DiscussionThread thread,
      short commentId,
      List<IdentityRef> authors = null)
    {
      requestContext.TraceEnter(600276, TraceArea.Discussion, TraceLayer.Service, nameof (LikeComment));
      ArgumentValidator.ValidateInput(thread, (int) commentId);
      ArgumentValidator.ValidateUsers(requestContext, authors);
      SecurityManager.CheckPermission(requestContext, (IEnumerable<DiscussionThread>) new DiscussionThread[1]
      {
        thread
      }, DiscussionPermissions.Contribute, true);
      Guid projectId;
      int reviewId;
      DiscussionExtensions.ExtractMetadata(thread.ArtifactUri, out projectId, out reviewId);
      if (authors == null || authors.Count == 0)
        authors = new List<IdentityRef>()
        {
          requestContext.GetUserIdentity().ToCRIdentityRef(requestContext)
        };
      List<SocialAuthor> socialAuthors = new List<SocialAuthor>(authors.Count);
      foreach (IdentityRef author in authors)
        socialAuthors.Add(new SocialAuthor(thread.DiscussionId, commentId, new Guid(author.Id)));
      List<SocialAuthor> likeAuthors = (List<SocialAuthor>) null;
      List<DiscussionThread> discussions;
      List<DiscussionComment> comments;
      using (DiscussionSqlResourceComponent component = requestContext.CreateComponent<DiscussionSqlResourceComponent>("VersionControl"))
        likeAuthors = component.SaveSocialAuthors(socialAuthors, projectId, reviewId, out discussions, out comments);
      Dictionary<Guid, IdentityRef> likedUsers;
      this.PopulateCommentsWithLikes(requestContext, likeAuthors, comments, out likedUsers);
      this.PostPublishDiscussions(requestContext, discussions, comments, (DiscussionComment[]) null, comments.ToArray(), (CommentId[]) null, likedCommentOnly: true);
      requestContext.TraceLeave(600277, TraceArea.Discussion, TraceLayer.Service, nameof (LikeComment));
      return likedUsers.Values.Where<IdentityRef>((Func<IdentityRef, bool>) (id => authors.Any<IdentityRef>((Func<IdentityRef, bool>) (author => author.Id == id.Id)))).ToList<IdentityRef>();
    }

    public List<IdentityRef> QueryLikes(
      IVssRequestContext requestContext,
      DiscussionThread thread,
      short commentId)
    {
      requestContext.TraceEnter(600278, TraceArea.Discussion, TraceLayer.Service, nameof (QueryLikes));
      ArgumentValidator.ValidateInput(thread, (int) commentId);
      SecurityManager.CheckPermission(requestContext, (IEnumerable<DiscussionThread>) new DiscussionThread[1]
      {
        thread
      }, DiscussionPermissions.Read, true);
      List<IdentityRef> identityRefList = (List<IdentityRef>) null;
      using (DiscussionSqlResourceComponent component = requestContext.CreateComponent<DiscussionSqlResourceComponent>("VersionControl"))
        identityRefList = component.QuerySocialAuthors(thread.DiscussionId, commentId).Select<SocialAuthor, IdentityRef>((Func<SocialAuthor, IdentityRef>) (x => x.Identity)).ToList<IdentityRef>();
      requestContext.TraceLeave(600279, TraceArea.Discussion, TraceLayer.Service, nameof (QueryLikes));
      return identityRefList;
    }

    public void WithdrawLikeComment(
      IVssRequestContext requestContext,
      DiscussionThread thread,
      short commentId,
      IdentityRef author)
    {
      requestContext.TraceEnter(600280, TraceArea.Discussion, TraceLayer.Service, nameof (WithdrawLikeComment));
      ArgumentValidator.ValidateInput(thread, (int) commentId);
      ArgumentUtility.CheckForNull<IdentityRef>(author, nameof (author));
      ArgumentValidator.ValidateUsers(requestContext, new List<IdentityRef>()
      {
        author
      });
      Guid projectId;
      int reviewId;
      DiscussionExtensions.ExtractMetadata(thread.ArtifactUri, out projectId, out reviewId);
      List<DiscussionThread> discussions;
      List<DiscussionComment> comments;
      List<SocialAuthor> allAuthors;
      using (DiscussionSqlResourceComponent component = requestContext.CreateComponent<DiscussionSqlResourceComponent>("VersionControl"))
        component.DeleteSocialAuthors(thread.DiscussionId, commentId, new Guid(author.Id), projectId, reviewId, out discussions, out comments, out allAuthors);
      this.PopulateCommentsWithLikes(requestContext, allAuthors, comments, out Dictionary<Guid, IdentityRef> _);
      this.PostPublishDiscussions(requestContext, discussions, comments, (DiscussionComment[]) null, comments.ToArray(), (CommentId[]) null, withdrawCommentOnly: true);
      requestContext.TraceLeave(600281, TraceArea.Discussion, TraceLayer.Service, nameof (WithdrawLikeComment));
    }

    private void PopulateThreadsWithComments(
      IVssRequestContext requestContext,
      IEnumerable<DiscussionThread> threads,
      IList<DiscussionComment> comments)
    {
      if (comments == null)
        return;
      Dictionary<int, List<DiscussionComment>> dictionary = new Dictionary<int, List<DiscussionComment>>();
      foreach (DiscussionComment comment in (IEnumerable<DiscussionComment>) comments)
      {
        List<DiscussionComment> discussionCommentList = (List<DiscussionComment>) null;
        if (!dictionary.TryGetValue(comment.DiscussionId, out discussionCommentList))
        {
          discussionCommentList = new List<DiscussionComment>();
          dictionary.Add(comment.DiscussionId, discussionCommentList);
        }
        discussionCommentList.Add(comment);
      }
      foreach (DiscussionThread thread in threads)
      {
        List<DiscussionComment> discussionCommentList = (List<DiscussionComment>) null;
        if (dictionary.TryGetValue(thread.DiscussionId, out discussionCommentList))
        {
          thread.Comments = discussionCommentList.ToArray();
          thread.CommentsCount = DiscussionThread.GetCommentCount(thread);
        }
      }
      IdentityHelper.PopulateAuthorDisplayNames(requestContext, (IEnumerable<DiscussionComment>) comments);
    }

    public IEnumerable<IDiscussionArtifactPlugin> ArtifactPlugins => (IEnumerable<IDiscussionArtifactPlugin>) this.m_artifactPlugins;

    private bool HasDeletePermission(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DiscussionSecurity.NamespaceId).HasPermission(requestContext, DiscussionSecurity.NamespaceRootToken, 8, false);

    private string ParseVersionUri(IVssRequestContext requestContext, string versionUri)
    {
      requestContext.TraceEnter(600030, TraceArea.Discussion, TraceLayer.Service, nameof (ParseVersionUri));
      IDiscussionArtifactPlugin pluginExtension = (IDiscussionArtifactPlugin) null;
      ArtifactId artifact = LinkingUtilities.DecodeUri(versionUri);
      foreach (IDiscussionArtifactPlugin artifactPlugin in this.ArtifactPlugins)
      {
        if (artifactPlugin.CanResolveArtifactId(artifact))
        {
          pluginExtension = artifactPlugin;
          break;
        }
      }
      int changesetId;
      string shelvesetName;
      string shelvesetOwner;
      ArtifactHelper.ParseVersionUri(requestContext, versionUri, pluginExtension, out changesetId, out shelvesetName, out shelvesetOwner);
      string versionUri1;
      if (changesetId > 0)
      {
        versionUri1 = ArtifactHelper.CreateVersionId(changesetId);
        requestContext.Trace(600031, TraceLevel.Info, TraceArea.Discussion, TraceLayer.Service, "Version id '{0}' created from changeset '{1}'", (object) versionUri1, (object) changesetId);
      }
      else if (!string.IsNullOrEmpty(shelvesetName))
      {
        Guid identity = IdentityHelper.NameToIdentity(requestContext, shelvesetOwner);
        versionUri1 = ArtifactHelper.CreateVersionId(shelvesetName, identity);
        requestContext.Trace(600032, TraceLevel.Info, TraceArea.Discussion, TraceLayer.Service, "Version id '{0}' created from shelveset '{1}\\{2}'", (object) versionUri1, (object) shelvesetName, (object) shelvesetOwner);
      }
      else
        versionUri1 = versionUri;
      requestContext.TraceLeave(600039, TraceArea.Discussion, TraceLayer.Service, nameof (ParseVersionUri));
      return versionUri1;
    }

    private void ParseVersionUris(IVssRequestContext requestContext, DiscussionThread[] discussions)
    {
      requestContext.TraceEnter(600040, TraceArea.Discussion, TraceLayer.Service, nameof (ParseVersionUris));
      string x1 = (string) null;
      int changesetId = 0;
      string shelvesetName = (string) null;
      string shelvesetOwner = (string) null;
      for (int index = 0; index < discussions.Length; ++index)
      {
        DiscussionThread discussion = discussions[index];
        ArgumentValidator.CheckNull(discussion.ArtifactUri, "VersionUri");
        IDiscussionArtifactPlugin pluginExtension = (IDiscussionArtifactPlugin) null;
        ArtifactId artifact = LinkingUtilities.DecodeUri(discussion.ArtifactUri);
        foreach (IDiscussionArtifactPlugin artifactPlugin in this.ArtifactPlugins)
        {
          if (artifactPlugin.CanResolveArtifactId(artifact))
          {
            pluginExtension = artifactPlugin;
            break;
          }
        }
        if (x1 == null || !TFStringComparer.ArtiFactUrl.Equals(x1, discussion.ArtifactUri))
        {
          ArtifactHelper.ParseVersionUri(requestContext, discussion.ArtifactUri, pluginExtension, out changesetId, out shelvesetName, out shelvesetOwner);
          x1 = discussion.ArtifactUri;
          requestContext.Trace(600041, TraceLevel.Info, TraceArea.Discussion, TraceLayer.Service, "Parsed version uri '{0}' to changeset id '{1}' or shelveset '{0}\\{1}'", (object) x1, (object) changesetId, (object) shelvesetName, (object) shelvesetOwner);
        }
        if (changesetId > 0)
        {
          ChangesetDiscussionThread subclass = discussion.ToSubclass<ChangesetDiscussionThread>();
          subclass.ChangesetId = changesetId;
          discussions[index] = (DiscussionThread) subclass;
        }
        else if (!string.IsNullOrEmpty(shelvesetName))
        {
          ShelvesetDiscussionThread subclass = discussion.ToSubclass<ShelvesetDiscussionThread>();
          subclass.ShelvesetName = shelvesetName;
          subclass.ShelvesetOwner = shelvesetOwner;
          discussions[index] = (DiscussionThread) subclass;
        }
      }
      IEnumerable<string> strings = ((IEnumerable<DiscussionThread>) discussions).Where<DiscussionThread>((Func<DiscussionThread, bool>) (x => x is ShelvesetDiscussionThread)).Cast<ShelvesetDiscussionThread>().Select<ShelvesetDiscussionThread, string>((Func<ShelvesetDiscussionThread, string>) (d => d.ShelvesetOwner));
      Dictionary<string, Guid> dictionary = (Dictionary<string, Guid>) null;
      if (strings.Count<string>() > 0)
        dictionary = IdentityHelper.NamesToIdentities(requestContext, strings);
      foreach (DiscussionThread discussion in discussions)
      {
        switch (discussion)
        {
          case ChangesetDiscussionThread _:
            ChangesetDiscussionThread discussionThread1 = discussion as ChangesetDiscussionThread;
            discussionThread1.VersionId = ArtifactHelper.CreateVersionId(discussionThread1.ChangesetId);
            break;
          case ShelvesetDiscussionThread _:
            ShelvesetDiscussionThread discussionThread2 = discussion as ShelvesetDiscussionThread;
            discussionThread2.VersionId = ArtifactHelper.CreateVersionId(discussionThread2.ShelvesetName, dictionary[discussionThread2.ShelvesetOwner]);
            break;
          default:
            ArtifactId artifact = LinkingUtilities.DecodeUri(discussion.ArtifactUri);
            foreach (IDiscussionArtifactPlugin artifactPlugin in this.ArtifactPlugins)
            {
              if (artifactPlugin.CanResolveArtifactId(artifact))
              {
                artifactPlugin.ResolveArtifactId(artifact);
                break;
              }
            }
            discussion.VersionId = discussion.ArtifactUri;
            break;
        }
        requestContext.Trace(600042, TraceLevel.Info, TraceArea.Discussion, TraceLayer.Service, "Created version id '{0}' for discussion", (object) discussion.VersionId);
      }
      requestContext.TraceLeave(600049, TraceArea.Discussion, TraceLayer.Service, nameof (ParseVersionUris));
    }

    private void ParseVersionIds(
      IVssRequestContext requestContext,
      List<DiscussionThread> discussions)
    {
      requestContext.TraceEnter(600050, TraceArea.Discussion, TraceLayer.Service, nameof (ParseVersionIds));
      string a = (string) null;
      int changesetId = 0;
      string shelvesetName = (string) null;
      string str1 = (string) null;
      Guid shelvesetOwner = Guid.Empty;
      for (int index = 0; index < discussions.Count; ++index)
      {
        DiscussionThread discussion = discussions[index];
        IDiscussionArtifactPlugin discussionArtifactPlugin = (IDiscussionArtifactPlugin) null;
        foreach (IDiscussionArtifactPlugin artifactPlugin in this.ArtifactPlugins)
        {
          if (artifactPlugin.CanResolveVersionId(discussion.VersionId))
          {
            discussionArtifactPlugin = artifactPlugin;
            break;
          }
        }
        if (a == null || !string.Equals(a, discussion.VersionId, StringComparison.OrdinalIgnoreCase))
        {
          ArtifactHelper.ParseVersionId(requestContext, discussion.VersionId, out changesetId, out shelvesetName, out shelvesetOwner);
          a = discussion.VersionId;
          str1 = (string) null;
          requestContext.Trace(600051, TraceLevel.Info, TraceArea.Discussion, TraceLayer.Service, "Parsed version id '{0}'", (object) a);
        }
        if (changesetId > 0)
        {
          ChangesetDiscussionThread subclass = discussion.ToSubclass<ChangesetDiscussionThread>();
          subclass.ChangesetId = changesetId;
          ChangesetDiscussionThread discussionThread = subclass;
          string str2;
          if (discussionArtifactPlugin == null)
            str2 = (string) null;
          else
            str2 = discussionArtifactPlugin.CreateUri((object) changesetId);
          discussionThread.ArtifactUri = str2;
          requestContext.Trace(600052, TraceLevel.Info, TraceArea.Discussion, TraceLayer.Service, "Created version uri '{0}' from changeset '{1}'", (object) discussion.VersionId, (object) changesetId);
          discussions[index] = (DiscussionThread) subclass;
        }
        else if (shelvesetOwner != Guid.Empty)
        {
          if (str1 == null)
          {
            str1 = IdentityHelper.IdentityToUniqueName(requestContext, shelvesetOwner);
            requestContext.Trace(600053, TraceLevel.Info, TraceArea.Discussion, TraceLayer.Service, "Retrieved shelveset owner unique name '{0}' from identity '{1}'", (object) str1, (object) shelvesetOwner);
          }
          ShelvesetDiscussionThread subclass = discussion.ToSubclass<ShelvesetDiscussionThread>();
          subclass.ShelvesetName = shelvesetName;
          subclass.ShelvesetOwner = str1;
          ShelvesetDiscussionThread discussionThread = subclass;
          string str3;
          if (discussionArtifactPlugin == null)
            str3 = (string) null;
          else
            str3 = discussionArtifactPlugin.CreateUri((object) shelvesetName, (object) str1);
          discussionThread.ArtifactUri = str3;
          requestContext.Trace(600054, TraceLevel.Info, TraceArea.Discussion, TraceLayer.Service, "Created version uri '{0}' from shelveset '{1}\\{2}'", (object) discussion.VersionId, (object) shelvesetName, (object) str1);
          discussions[index] = (DiscussionThread) subclass;
        }
        else
          discussion.ArtifactUri = discussion.VersionId;
      }
      requestContext.TraceLeave(600059, TraceArea.Discussion, TraceLayer.Service, nameof (ParseVersionIds));
    }

    private void PostQueryDiscussions(
      IVssRequestContext requestContext,
      List<DiscussionThread> discussions,
      List<DiscussionComment> comments,
      bool includeExtendedProperties = true)
    {
      requestContext.TraceEnter(600060, TraceArea.Discussion, TraceLayer.Service, nameof (PostQueryDiscussions));
      if (discussions.Count > 0)
      {
        this.ParseVersionIds(requestContext, discussions);
        if (includeExtendedProperties && this.CanQueryDiscussionArtifacts(requestContext, discussions.Count))
        {
          Dictionary<int, DiscussionThread> dictionary = discussions.ToDictionary<DiscussionThread, int>((Func<DiscussionThread, int>) (x => x.PropertyId));
          ArtifactSpec[] array = discussions.Select<DiscussionThread, ArtifactSpec>((Func<DiscussionThread, ArtifactSpec>) (x => new ArtifactSpec(TeamFoundationDiscussionService.DiscussionArtifactKind, x.PropertyId, 0))).ToArray<ArtifactSpec>();
          using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, (IEnumerable<ArtifactSpec>) array, (IEnumerable<string>) null))
          {
            foreach (ArtifactPropertyValue artifactPropertyValue in properties)
            {
              DiscussionThread discussionThread = dictionary[artifactPropertyValue.GetPropertyId()];
              IDiscussionArtifactPlugin discussionArtifactPlugin = (IDiscussionArtifactPlugin) null;
              ArtifactId artifact = LinkingUtilities.DecodeUri(discussionThread.ArtifactUri);
              foreach (IDiscussionArtifactPlugin artifactPlugin in this.ArtifactPlugins)
              {
                if (artifactPlugin.CanResolveArtifactId(artifact))
                {
                  discussionArtifactPlugin = artifactPlugin;
                  break;
                }
              }
              discussionThread.Properties = new PropertiesCollection();
              foreach (PropertyValue propertyValue in artifactPropertyValue.PropertyValues)
              {
                if (propertyValue.PropertyName == "Microsoft.TeamFoundation.Discussion.ItemPath" && discussionArtifactPlugin != null)
                  propertyValue.Value = (object) discussionArtifactPlugin.TranslateOutgoingPath(requestContext, (string) propertyValue.Value);
                discussionThread.Properties[propertyValue.PropertyName] = propertyValue.Value;
              }
            }
          }
        }
        if (comments != null && comments.Count > 0)
        {
          bool flag = this.HasDeletePermission(requestContext);
          IdentityRef crIdentityRef = requestContext.GetUserIdentity().ToCRIdentityRef(requestContext);
          foreach (DiscussionComment comment in comments)
            comment.CanDelete = flag || comment.Author == crIdentityRef;
        }
        this.RemoveRestrictedQueryResults(requestContext, discussions, comments);
      }
      requestContext.TraceLeave(600069, TraceArea.Discussion, TraceLayer.Service, nameof (PostQueryDiscussions));
    }

    private void RemoveRestrictedQueryResults(
      IVssRequestContext requestContext,
      List<DiscussionThread> discussions,
      List<DiscussionComment> comments)
    {
      requestContext.TraceEnter(600070, TraceArea.Discussion, TraceLayer.Service, nameof (RemoveRestrictedQueryResults));
      Dictionary<int, bool> dictionary = SecurityManager.CheckPermission(requestContext, (IEnumerable<DiscussionThread>) discussions, DiscussionPermissions.Read, false);
      int index1 = 0;
      for (int index2 = 0; index2 < discussions.Count; ++index2)
      {
        int discussionId = discussions[index2].DiscussionId;
        bool flag;
        if (dictionary.TryGetValue(discussionId, out flag) & flag)
        {
          while (index1 < comments.Count && comments[index1].DiscussionId == discussionId)
            ++index1;
          requestContext.Trace(600071, TraceLevel.Info, TraceArea.Discussion, TraceLayer.Service, "Comment index incremented to {0} for discussion '{1}'", (object) index1, (object) discussionId);
        }
        else
        {
          discussions.RemoveAt(index2);
          --index2;
          int count = 0;
          while (index1 + count < comments.Count && comments[index1 + count].DiscussionId == discussionId)
            ++count;
          if (count > 0)
            comments.RemoveRange(index1, count);
          requestContext.Trace(600072, TraceLevel.Info, TraceArea.Discussion, TraceLayer.Service, "Removed discussion '{0}' and its {1} comment(s)", (object) discussionId, (object) count);
        }
      }
      foreach (DiscussionComment comment in comments)
      {
        if (comment.IsDeleted)
          comment.Content = (string) null;
      }
      requestContext.TraceLeave(600079, TraceArea.Discussion, TraceLayer.Service, nameof (RemoveRestrictedQueryResults));
    }

    private void PrepareDiscussions(
      IVssRequestContext requestContext,
      DiscussionThread[] discussions,
      DiscussionComment[] comments,
      CommentId[] deletedComments)
    {
      requestContext.TraceEnter(600080, TraceArea.Discussion, TraceLayer.Service, nameof (PrepareDiscussions));
      int num = ((IEnumerable<DiscussionThread>) discussions).Min<DiscussionThread>((Func<DiscussionThread, int>) (d => d.DiscussionId));
      int index1 = 0;
      int index2 = 0;
      foreach (DiscussionThread discussion in discussions)
      {
        if (discussion.DiscussionId != 0)
        {
          while (index1 < comments.Length && comments[index1].DiscussionId == discussion.DiscussionId)
            ++index1;
          requestContext.Trace(600081, TraceLevel.Verbose, TraceArea.Discussion, TraceLayer.Service, "Comment index incremented to {0} for discussion '{1}'", (object) index1, (object) discussion.DiscussionId);
          while (index2 < deletedComments.Length && deletedComments[index2].DiscussionId == discussion.DiscussionId)
            ++index2;
          requestContext.Trace(600269, TraceLevel.Verbose, TraceArea.Discussion, TraceLayer.Service, "Deleted comment index incremented to {0} for discussion '{1}'", (object) index2, (object) discussion.DiscussionId);
        }
        else if (index1 < comments.Length && comments[index1].DiscussionId == 0)
        {
          discussion.DiscussionId = --num;
          comments[index1].DiscussionId = discussion.DiscussionId;
          ++index1;
          requestContext.Trace(600082, TraceLevel.Verbose, TraceArea.Discussion, TraceLayer.Service, "Assigned temp discussion id '{0}'", (object) num);
        }
        else
        {
          requestContext.Trace(600083, TraceLevel.Error, TraceArea.Discussion, TraceLayer.Service, "Comment '{0}' has no discussion");
          break;
        }
      }
      if (index1 < comments.Length || index2 < deletedComments.Length)
        throw new ArgumentException(Resources.DiscussionsAndCommentsMismatch);
      requestContext.TraceLeave(600089, TraceArea.Discussion, TraceLayer.Service, nameof (PrepareDiscussions));
    }

    private bool PreparePublishingDiscussions(
      IVssRequestContext requestContext,
      DiscussionThread[] discussions,
      DiscussionComment[] comments,
      CommentId[] deletedComments,
      out Guid? requestingIdentity,
      out int reviewId,
      out Guid projectId)
    {
      ArgumentValidator.CheckNull((object) discussions, nameof (discussions));
      ArgumentValidator.CheckNullCollection<DiscussionThread>((IEnumerable<DiscussionThread>) discussions, "discussions.Item", false);
      IdentityRef crIdentityRef = requestContext.GetUserIdentity().ToCRIdentityRef(requestContext);
      if (crIdentityRef == null)
        throw new IdentityNotFoundException(requestContext.UserContext);
      if (comments != null)
      {
        ArgumentValidator.CheckNullCollection<DiscussionComment>((IEnumerable<DiscussionComment>) comments, "comments.Item");
        foreach (DiscussionComment comment in comments)
        {
          ArgumentValidator.ValidateComment(comment);
          if (comment.Author?.Id != null && !comment.Author.Id.Equals(crIdentityRef.Id))
            throw new ArgumentException(Resources.CommentAuthorMustBeRequester, "comment.Author");
        }
      }
      else
        comments = Array.Empty<DiscussionComment>();
      bool flag = false;
      requestingIdentity = new Guid?();
      if (deletedComments != null)
      {
        ArgumentValidator.CheckNullCollection<CommentId>((IEnumerable<CommentId>) deletedComments, "deletedComments.Item");
        if (!this.HasDeletePermission(requestContext))
          requestingIdentity = new Guid?(new Guid(crIdentityRef.Id));
        if (comments != null && ((IEnumerable<DiscussionComment>) comments).Count<DiscussionComment>() == 0)
          flag = true;
      }
      else
        deletedComments = Array.Empty<CommentId>();
      foreach (DiscussionComment comment in comments)
      {
        if (comment.Author?.Id == null)
          comment.Author = crIdentityRef;
      }
      this.PrepareDiscussions(requestContext, discussions, comments, deletedComments);
      this.ParseVersionUris(requestContext, discussions);
      SecurityManager.CheckPermission(requestContext, (IEnumerable<DiscussionThread>) discussions, DiscussionPermissions.Contribute, true);
      IEnumerable<DiscussionThread> source1 = ((IEnumerable<DiscussionThread>) discussions).Where<DiscussionThread>((Func<DiscussionThread, bool>) (x => x.DiscussionId < 0));
      int num = (int) requestContext.GetService<ITeamFoundationCounterService>().ReserveCounterIds(requestContext, "DiscussionThreadPropertyId", (long) source1.Count<DiscussionThread>());
      DiscussionExtensions.ExtractMetadata(((IEnumerable<DiscussionThread>) discussions).FirstOrDefault<DiscussionThread>().ArtifactUri, out projectId, out reviewId);
      List<ArtifactPropertyValue> source2 = new List<ArtifactPropertyValue>();
      foreach (DiscussionThread discussionThread in source1)
      {
        IDiscussionArtifactPlugin discussionArtifactPlugin = (IDiscussionArtifactPlugin) null;
        ArtifactId artifact = LinkingUtilities.DecodeUri(discussionThread.ArtifactUri);
        foreach (IDiscussionArtifactPlugin artifactPlugin in this.ArtifactPlugins)
        {
          if (artifactPlugin.CanResolveArtifactId(artifact))
          {
            discussionArtifactPlugin = artifactPlugin;
            break;
          }
        }
        discussionThread.PropertyId = num++;
        if (discussionThread.Properties != null)
        {
          List<PropertyValue> source3 = new List<PropertyValue>();
          foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) discussionThread.Properties)
          {
            object obj = property.Value;
            if (property.Key == "Microsoft.TeamFoundation.Discussion.ItemPath" && discussionArtifactPlugin != null)
              obj = (object) discussionArtifactPlugin.TranslateIncomingPath(requestContext, (string) property.Value);
            source3.Add(new PropertyValue(property.Key, obj));
          }
          if (source3.Any<PropertyValue>())
            source2.Add(new ArtifactPropertyValue(TeamFoundationDiscussionService.DiscussionArtifactKind, discussionThread.PropertyId, 0, (IEnumerable<PropertyValue>) source3));
        }
      }
      if (source2.Any<ArtifactPropertyValue>())
        requestContext.GetService<ITeamFoundationPropertyService>().SetProperties(requestContext, (IEnumerable<ArtifactPropertyValue>) source2);
      return flag;
    }

    private void PostPublishDiscussions(
      IVssRequestContext requestContext,
      List<DiscussionThread> savedDiscussions,
      List<DiscussionComment> savedComments,
      DiscussionComment[] newOrUpdatedComments,
      DiscussionComment[] likedComments,
      CommentId[] deletedComments,
      bool deletedCommentsOnly = false,
      bool likedCommentOnly = false,
      bool withdrawCommentOnly = false)
    {
      if (savedDiscussions.Count == 0)
        return;
      this.PopulateThreadsWithComments(requestContext, (IEnumerable<DiscussionThread>) savedDiscussions, (IList<DiscussionComment>) savedComments);
      this.PostQueryDiscussions(requestContext, savedDiscussions, savedComments);
      DiscussionComment[] newOrUpdatedComments1 = newOrUpdatedComments;
      if (deletedCommentsOnly)
        newOrUpdatedComments1 = savedComments.Where<DiscussionComment>((Func<DiscussionComment, bool>) (comment => ((IEnumerable<CommentId>) deletedComments).Contains<CommentId>(comment.ToCommentId(), (IEqualityComparer<CommentId>) new CommentId.Comparer()))).ToArray<DiscussionComment>();
      IVssRequestContext requestContext1 = requestContext;
      DiscussionsNotification notificationEvent = new DiscussionsNotification(savedDiscussions.ToArray(), newOrUpdatedComments1, likedComments);
      notificationEvent.HasDeletedCommentsOnly = deletedCommentsOnly;
      notificationEvent.LikeCommentOnly = likedCommentOnly;
      notificationEvent.WithdrawLikeCommentOnly = withdrawCommentOnly;
      string discussion = TraceArea.Discussion;
      string service = TraceLayer.Service;
      EventServiceHelper.PublishNotification(requestContext1, notificationEvent, discussion, service);
    }

    private void PopulateCommentsWithLikes(
      IVssRequestContext requestContext,
      List<SocialAuthor> likeAuthors,
      List<DiscussionComment> updatedComments,
      out Dictionary<Guid, IdentityRef> likedUsers)
    {
      HashSet<Guid> source = new HashSet<Guid>();
      foreach (SocialAuthor likeAuthor in likeAuthors)
        source.Add(likeAuthor.GetAuthorId());
      likedUsers = IdentityHelper.PopulateDisplayNames(requestContext, (IList<Guid>) source.ToList<Guid>());
      foreach (DiscussionComment updatedComment in updatedComments)
      {
        foreach (SocialAuthor likeAuthor in likeAuthors)
        {
          if (updatedComment.DiscussionId == likeAuthor.DiscussionId && (int) updatedComment.CommentId == (int) likeAuthor.CommentId)
          {
            if (updatedComment.UsersLiked == null)
              updatedComment.UsersLiked = new List<IdentityRef>();
            updatedComment.UsersLiked.Add(likedUsers[new Guid(likeAuthor.Identity.Id)]);
          }
        }
      }
    }

    private bool CanQueryDiscussionArtifacts(IVssRequestContext requestContext, int discussionCount)
    {
      if (discussionCount <= 50)
        return true;
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/CodeReview/MaxDiscussionsForArtifactQuery", true, 10000);
      return discussionCount < num;
    }
  }
}
