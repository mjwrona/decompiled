// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.CommentService
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.TeamFoundation.Comments.Server.Events;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.MarkdownRenderer;
using Microsoft.TeamFoundation.Mention.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class CommentService : CommentServiceBase, ICommentService, IVssFrameworkService
  {
    public static readonly int DefaultPageSize = 200;
    public static readonly int MaxAllowedPageSize = 1000;
    private static readonly string ContinuationTokenFormat = "O";
    private const int DefaultChildCount = 5;
    private const char ProjectAttachmentReplacementCharacter = '\u0006';

    public CommentService()
      : base(VssDateTimeProvider.DefaultProvider)
    {
    }

    internal CommentService(IVssDateTimeProvider dateTimeProvider)
      : base(dateTimeProvider)
    {
    }

    internal CommentService(
      IVssDateTimeProvider dateTimeProvider,
      IDisposableReadOnlyList<ICommentProvider> providers)
      : base(dateTimeProvider, providers)
    {
    }

    public CommentsList GetComments(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      ISet<int> ids,
      ExpandOptions expandOptions = ExpandOptions.None,
      bool includeDeleted = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) ids, nameof (ids));
      ArgumentUtility.CheckBoundsInclusive(ids.Count, 1, CommentService.MaxAllowedPageSize, nameof (ids));
      requestContext.TraceEnter(140051, nameof (CommentService), "Service", nameof (GetComments));
      try
      {
        expandOptions = this.NormalizeExpandOptions(requestContext, artifactKind, expandOptions);
        this.CheckSupportsThreading(requestContext, artifactKind, expandOptions);
        ISecuredObject securedObject;
        this.CheckReadPermission(requestContext, projectId, artifactKind, artifactId, out securedObject);
        bool fetchText = !expandOptions.HasFlag((Enum) ExpandOptions.RenderedTextOnly);
        bool fetchRenderedText = expandOptions.HasFlag((Enum) ExpandOptions.RenderedText) || expandOptions.HasFlag((Enum) ExpandOptions.RenderedTextOnly);
        List<Comment> commentsByIds;
        using (CommentComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<CommentComponent>("Comments"))
          commentsByIds = replicaAwareComponent.GetCommentsByIds(artifactKind, artifactId, ids, fetchText, fetchRenderedText, includeDeleted, expandOptions.HasFlag((Enum) ExpandOptions.Children));
        this.ValidateResultCountMatch(requestContext, ids, (IList<Comment>) commentsByIds);
        this.HandleExpandOptions(requestContext, projectId, artifactKind, artifactId, (IEnumerable<Comment>) commentsByIds, expandOptions);
        return new CommentsList((ICollection<Comment>) commentsByIds, securedObject);
      }
      finally
      {
        requestContext.TraceLeave(140059, nameof (CommentService), "Service", nameof (GetComments));
      }
    }

    public CommentsList GetComments(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      int? top = null,
      string continuationToken = null,
      ExpandOptions expandOptions = ExpandOptions.None,
      bool includeDeleted = false,
      SortOrder order = SortOrder.Desc,
      int? parentId = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      requestContext.TraceEnter(140001, nameof (CommentService), "Service", nameof (GetComments));
      try
      {
        expandOptions = this.NormalizeExpandOptions(requestContext, artifactKind, expandOptions);
        this.CheckSupportsThreading(requestContext, artifactKind, expandOptions);
        ISecuredObject securedObject;
        this.CheckReadPermission(requestContext, projectId, artifactKind, artifactId, out securedObject);
        if (top.HasValue)
          ArgumentUtility.CheckBoundsInclusive(top.Value, 1, CommentService.MaxAllowedPageSize, nameof (top));
        int num = (top ?? CommentService.DefaultPageSize) + 1;
        DateTime result;
        DateTime? startFrom = DateTime.TryParseExact(continuationToken, CommentService.ContinuationTokenFormat, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result) ? new DateTime?(result.ToUniversalTime()) : new DateTime?();
        bool fetchText = !expandOptions.HasFlag((Enum) ExpandOptions.RenderedTextOnly);
        bool fetchRenderedText = expandOptions.HasFlag((Enum) ExpandOptions.RenderedText) || expandOptions.HasFlag((Enum) ExpandOptions.RenderedTextOnly);
        int expandChildCount = 6;
        bool flag = expandOptions.HasFlag((Enum) ExpandOptions.Children);
        CommentsList commentsList;
        using (CommentComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<CommentComponent>("Comments"))
          commentsList = this.SupportsThreading(requestContext, artifactKind) ? (!flag ? replicaAwareComponent.GetCommentsFromParent(artifactKind, artifactId, num, startFrom, fetchText, fetchRenderedText, includeDeleted, order, parentId) : replicaAwareComponent.GetCommentsWithChildren(artifactKind, artifactId, num, startFrom, fetchText, fetchRenderedText, includeDeleted, order, expandChildCount)) : replicaAwareComponent.GetComments(artifactKind, artifactId, num, startFrom, fetchText, fetchRenderedText, includeDeleted, order);
        this.UpdateContinuationToken(commentsList, num);
        this.HandleExpandOptions(requestContext, projectId, artifactKind, artifactId, (IEnumerable<Comment>) commentsList.Comments, expandOptions);
        if (securedObject != null)
          commentsList.SetSecuredObject(securedObject);
        return commentsList;
      }
      finally
      {
        requestContext.TraceLeave(140009, nameof (CommentService), "Service", nameof (GetComments));
      }
    }

    public Comment GetComment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      int commentId,
      ExpandOptions expandOptions = ExpandOptions.None,
      bool includeDeleted = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckForNonnegativeInt(commentId, nameof (commentId));
      requestContext.TraceEnter(140061, nameof (CommentService), "Service", nameof (GetComment));
      try
      {
        expandOptions = this.NormalizeExpandOptions(requestContext, artifactKind, expandOptions);
        this.CheckSupportsThreading(requestContext, artifactKind, expandOptions);
        ISecuredObject securedObject;
        this.CheckReadPermission(requestContext, projectId, artifactKind, artifactId, out securedObject);
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        Guid artifactKind1 = artifactKind;
        string artifactId1 = artifactId;
        HashSet<int> ids = new HashSet<int>();
        ids.Add(commentId);
        int num1 = (int) expandOptions;
        int num2 = includeDeleted ? 1 : 0;
        Comment comment = this.GetComments(requestContext1, projectId1, artifactKind1, artifactId1, (ISet<int>) ids, (ExpandOptions) num1, num2 != 0).Comments.FirstOrDefault<Comment>();
        if (comment == null)
          throw new CommentNotFoundException(commentId).Expected(requestContext.ServiceName);
        comment.SetSecuredObject(securedObject);
        return comment;
      }
      finally
      {
        requestContext.TraceLeave(140069, nameof (CommentService), "Service", nameof (GetComment));
      }
    }

    public IList<CommentVersion> GetCommentVersions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      int commentId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckForNonnegativeInt(commentId, nameof (commentId));
      requestContext.TraceEnter(140041, nameof (CommentService), "Service", nameof (GetCommentVersions));
      try
      {
        ISecuredObject securedObject;
        this.CheckReadPermission(requestContext, projectId, artifactKind, artifactId, out securedObject);
        List<CommentVersion> commentVersions;
        using (CommentComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<CommentComponent>("Comments"))
          commentVersions = replicaAwareComponent.GetCommentVersions(artifactKind, artifactId, commentId);
        if (!commentVersions.Any<CommentVersion>())
          throw new CommentNotFoundException(commentId).Expected(requestContext.ServiceName);
        if (securedObject != null)
          commentVersions.ForEach((Action<CommentVersion>) (cv => cv.SetSecuredObject(securedObject)));
        return (IList<CommentVersion>) commentVersions;
      }
      finally
      {
        requestContext.TraceLeave(140049, nameof (CommentService), "Service", nameof (GetCommentVersions));
      }
    }

    public CommentVersion GetCommentVersion(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      int commentId,
      int version)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckForNonnegativeInt(commentId, nameof (commentId));
      ArgumentUtility.CheckForNonnegativeInt(version, nameof (version));
      requestContext.TraceEnter(140071, nameof (CommentService), "Service", nameof (GetCommentVersion));
      try
      {
        ISecuredObject securedObject;
        this.CheckReadPermission(requestContext, projectId, artifactKind, artifactId, out securedObject);
        List<CommentVersion> commentsVersions;
        using (CommentComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<CommentComponent>("Comments"))
          commentsVersions = replicaAwareComponent.GetCommentsVersions(artifactKind, (IEnumerable<Microsoft.TeamFoundation.Comments.Server.GetCommentVersion>) new Microsoft.TeamFoundation.Comments.Server.GetCommentVersion[1]
          {
            new Microsoft.TeamFoundation.Comments.Server.GetCommentVersion(artifactId, commentId, version)
          });
        CommentVersion commentVersion = commentsVersions.Any<CommentVersion>() ? commentsVersions.First<CommentVersion>() : throw new CommentVersionNotFoundException(commentId, version).Expected(requestContext.ServiceName);
        commentVersion.SetSecuredObject(securedObject);
        return commentVersion;
      }
      finally
      {
        requestContext.TraceLeave(140072, nameof (CommentService), "Service", nameof (GetCommentVersion));
      }
    }

    public Comment AddComment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      Microsoft.TeamFoundation.Comments.Server.AddComment comment)
    {
      requestContext.TraceEnter(140091, nameof (CommentService), "Service", nameof (AddComment));
      try
      {
        return this.AddComments(requestContext, projectId, artifactKind, (IEnumerable<Microsoft.TeamFoundation.Comments.Server.AddComment>) new Microsoft.TeamFoundation.Comments.Server.AddComment[1]
        {
          comment
        }).FirstOrDefault<Comment>();
      }
      finally
      {
        requestContext.TraceLeave(140099, nameof (CommentService), "Service", nameof (AddComment));
      }
    }

    internal List<Comment> AddComments(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      IEnumerable<Microsoft.TeamFoundation.Comments.Server.AddComment> comments)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) comments, nameof (comments));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) comments, nameof (comments));
      ArgumentUtility.CheckBoundsInclusive(comments.Count<Microsoft.TeamFoundation.Comments.Server.AddComment>(), 1, CommentService.MaxAllowedPageSize, nameof (comments));
      requestContext.TraceEnter(140011, nameof (CommentService), "Service", nameof (AddComments));
      try
      {
        ICommentProvider provider = this.GetProvider(requestContext, artifactKind, new Guid?(projectId));
        CommentFormat commentFormat = provider.GetCommentFormat(requestContext, projectId);
        IEnumerable<string> artifactIds = comments.Select<Microsoft.TeamFoundation.Comments.Server.AddComment, string>((Func<Microsoft.TeamFoundation.Comments.Server.AddComment, string>) (cv => cv.ArtifactId)).Distinct<string>();
        IDictionary<string, ISecuredObject> securedObjects;
        this.CheckAddPermission(requestContext, projectId, artifactKind, artifactIds, out securedObjects);
        this.ProcessRenderedText(requestContext, projectId, artifactKind, provider, (IEnumerable<IRenderedText>) comments);
        this.SetChangedByAndDate(requestContext, (IEnumerable<IChangedBy>) comments);
        List<Comment> commentList;
        using (CommentComponent component = requestContext.CreateComponent<CommentComponent>())
          commentList = component.AddComments(artifactKind, comments, commentFormat);
        this.HandleAttachments(requestContext, projectId, artifactKind, (IEnumerable<IRenderedText>) commentList);
        this.SecureCommentsByArtifactId((IEnumerable<Comment>) commentList, securedObjects);
        CommentEvent.FireCommentEvents(requestContext, projectId, provider, (IEnumerable<Comment>) commentList, CommentEventType.Added);
        return commentList;
      }
      finally
      {
        requestContext.TraceLeave(140019, nameof (CommentService), "Service", nameof (AddComments));
      }
    }

    public Comment UpdateComment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      Microsoft.TeamFoundation.Comments.Server.UpdateComment comment)
    {
      requestContext.TraceEnter(140121, nameof (CommentService), "Service", nameof (UpdateComment));
      try
      {
        return this.UpdateComments(requestContext, projectId, artifactKind, (IEnumerable<Microsoft.TeamFoundation.Comments.Server.UpdateComment>) new Microsoft.TeamFoundation.Comments.Server.UpdateComment[1]
        {
          comment
        }).FirstOrDefault<Comment>();
      }
      finally
      {
        requestContext.TraceLeave(140129, nameof (CommentService), "Service", nameof (UpdateComment));
      }
    }

    internal List<Comment> UpdateComments(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      IEnumerable<Microsoft.TeamFoundation.Comments.Server.UpdateComment> comments)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) comments, nameof (comments));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) comments, nameof (comments));
      ArgumentUtility.CheckBoundsInclusive(comments.Count<Microsoft.TeamFoundation.Comments.Server.UpdateComment>(), 1, CommentService.MaxAllowedPageSize, nameof (comments));
      requestContext.TraceEnter(140021, nameof (CommentService), "Service", nameof (UpdateComments));
      try
      {
        ICommentProvider provider = this.GetProvider(requestContext, artifactKind, new Guid?(projectId));
        CommentFormat commentFormat = provider.GetCommentFormat(requestContext, projectId);
        IDictionary<int, ISecuredObject> securedObjects;
        this.CheckUpdatePermission(requestContext, projectId, artifactKind, comments, out securedObjects);
        this.ProcessRenderedText(requestContext, projectId, artifactKind, provider, (IEnumerable<IRenderedText>) comments);
        this.SetChangedByAndDate(requestContext, (IEnumerable<IChangedBy>) comments);
        List<Comment> commentList;
        using (CommentComponent component = requestContext.CreateComponent<CommentComponent>())
          commentList = component.UpdateComments(artifactKind, comments, commentFormat);
        this.HandleAttachments(requestContext, projectId, artifactKind, (IEnumerable<IRenderedText>) commentList);
        this.SecureCommentsByCommentId((IEnumerable<Comment>) commentList, securedObjects);
        CommentEvent.FireCommentEvents(requestContext, projectId, provider, (IEnumerable<Comment>) commentList, CommentEventType.Updated);
        return commentList;
      }
      finally
      {
        requestContext.TraceLeave(140029, nameof (CommentService), "Service", nameof (UpdateComments));
      }
    }

    public Comment DeleteComment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      int commentId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckForNonnegativeInt(commentId, nameof (commentId));
      requestContext.TraceEnter(140101, nameof (CommentService), "Service", nameof (DeleteComment));
      try
      {
        Microsoft.TeamFoundation.Comments.Server.DeleteComment deleteComment = new Microsoft.TeamFoundation.Comments.Server.DeleteComment(artifactId, commentId);
        return this.DeleteComments(requestContext, projectId, artifactKind, (IEnumerable<Microsoft.TeamFoundation.Comments.Server.DeleteComment>) new Microsoft.TeamFoundation.Comments.Server.DeleteComment[1]
        {
          deleteComment
        }).FirstOrDefault<Comment>();
      }
      finally
      {
        requestContext.TraceLeave(140109, nameof (CommentService), "Service", nameof (DeleteComment));
      }
    }

    public Comment DeleteComment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      Microsoft.TeamFoundation.Comments.Server.DeleteComment comment)
    {
      requestContext.TraceEnter(140101, nameof (CommentService), "Service", nameof (DeleteComment));
      try
      {
        return this.DeleteComments(requestContext, projectId, artifactKind, (IEnumerable<Microsoft.TeamFoundation.Comments.Server.DeleteComment>) new Microsoft.TeamFoundation.Comments.Server.DeleteComment[1]
        {
          comment
        }).FirstOrDefault<Comment>();
      }
      finally
      {
        requestContext.TraceLeave(140109, nameof (CommentService), "Service", nameof (DeleteComment));
      }
    }

    internal List<Comment> DeleteComments(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      IEnumerable<Microsoft.TeamFoundation.Comments.Server.DeleteComment> comments)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) comments, nameof (comments));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) comments, nameof (comments));
      ArgumentUtility.CheckBoundsInclusive(comments.Count<Microsoft.TeamFoundation.Comments.Server.DeleteComment>(), 1, CommentService.MaxAllowedPageSize, nameof (comments));
      requestContext.TraceEnter(140031, nameof (CommentService), "Service", nameof (DeleteComments));
      try
      {
        ICommentProvider provider = this.GetProvider(requestContext, artifactKind);
        IDictionary<int, ISecuredObject> securedObjects;
        this.CheckDeletePermission(requestContext, projectId, artifactKind, comments, out securedObjects);
        this.SetChangedByAndDate(requestContext, (IEnumerable<IChangedBy>) comments);
        List<Comment> commentResults;
        using (CommentComponent component = requestContext.CreateComponent<CommentComponent>())
          commentResults = component.DeleteComments(artifactKind, comments);
        this.SecureCommentsByCommentId((IEnumerable<Comment>) commentResults, securedObjects);
        CommentEvent.FireCommentEvents(requestContext, projectId, provider, (IEnumerable<Comment>) commentResults, CommentEventType.Deleted);
        return commentResults;
      }
      finally
      {
        requestContext.TraceLeave(140039, nameof (CommentService), "Service", nameof (DeleteComments));
      }
    }

    public void DestroyCommentsForArtifacts(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      ISet<string> artifactIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) artifactIds, nameof (artifactIds));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) artifactIds, nameof (artifactIds));
      requestContext.TraceEnter(140131, nameof (CommentService), "Service", nameof (DestroyCommentsForArtifacts));
      try
      {
        this.GetProvider(requestContext, artifactKind).CheckDestroyPermission(requestContext, projectId, artifactIds);
        using (CommentComponent component = requestContext.CreateComponent<CommentComponent>())
          component.DestroyComments(artifactKind, (IEnumerable<string>) artifactIds);
      }
      finally
      {
        requestContext.TraceLeave(140139, nameof (CommentService), "Service", nameof (DestroyCommentsForArtifacts));
      }
    }

    public Comment ProcessComment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      Microsoft.TeamFoundation.Comments.Server.ProcessComment comment)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Comments.Server.ProcessComment>(comment, nameof (comment));
      ICommentProvider provider = this.GetProvider(requestContext, artifactKind, new Guid?(projectId), new CommentFormat?(comment.Format));
      Microsoft.TeamFoundation.Comments.Server.ProcessComment[] comments = new Microsoft.TeamFoundation.Comments.Server.ProcessComment[1]
      {
        comment
      };
      this.ProcessRenderedText(requestContext, projectId, artifactKind, provider, (IEnumerable<IRenderedText>) comments);
      this.HandleAttachments(requestContext, projectId, artifactKind, (IEnumerable<IRenderedText>) new Microsoft.TeamFoundation.Comments.Server.ProcessComment[1]
      {
        comment
      });
      return new Comment(artifactKind, comment.ArtifactId, 0, comment.Text)
      {
        RenderedText = comment.RenderedText,
        Format = comment.Format
      };
    }

    private void ProcessRenderedText(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      ICommentProvider commentProvider,
      IEnumerable<IRenderedText> comments)
    {
      IMarkdownRendererService service = requestContext.GetService<IMarkdownRendererService>();
      CommentFormat commentFormat = commentProvider.GetCommentFormat(requestContext, projectId);
      foreach (IRenderedText comment in comments)
      {
        if (commentFormat == CommentFormat.Markdown)
        {
          IMentionSourceContext commentMentionContext = (IMentionSourceContext) CommentService.CreateCommentMentionContext(requestContext, projectId, commentProvider, comment, requestContext.GetUserId());
          RenderedHtml htmlWithMentions = service.ToHtmlWithMentions(requestContext, commentMentionContext, comment.Text);
          comment.RenderedText = SafeHtmlWrapper.MakeSafe(htmlWithMentions.Html);
        }
        if (commentFormat == CommentFormat.Html)
        {
          comment.RenderedText = string.Empty;
          comment.Text = SafeHtmlWrapper.MakeSafe(comment.Text);
        }
        this.HandleAttachmentsBeforeSave(requestContext, projectId, artifactKind, comment);
      }
    }

    private void HandleAttachmentsBeforeSave(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      IRenderedText comment)
    {
      string commentAttachmentsUri = this.GetCommentAttachmentsUri(requestContext, projectId, artifactKind, comment.ArtifactId);
      if (string.IsNullOrWhiteSpace(commentAttachmentsUri))
        return;
      comment.Text = comment.Text.ReplaceStringWithChar(commentAttachmentsUri, '\u0006');
      comment.RenderedText = comment.RenderedText.ReplaceStringWithChar(commentAttachmentsUri, '\u0006');
      foreach (Guid attachmentId in this.ParseAttachmentIds(comment.Text))
        comment.AddAttachment(attachmentId);
    }

    private IEnumerable<Guid> ParseAttachmentIds(string text)
    {
      int length = text.Length;
      for (int currentPosition = 0; currentPosition + 36 + 2 < length && (currentPosition = text.IndexOf('\u0006', currentPosition)) >= 0; ++currentPosition)
      {
        Guid result;
        if (Guid.TryParse(text.Substring(currentPosition + 2, 36), out result) && result != Guid.Empty)
        {
          currentPosition += 37;
          yield return result;
        }
      }
    }

    private void HandleAttachments(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      IEnumerable<IRenderedText> comments)
    {
      List<string> list = comments.Select<IRenderedText, string>((Func<IRenderedText, string>) (c => c.ArtifactId)).Distinct<string>().ToList<string>();
      Dictionary<string, string> urlMap = this.BuildArtifactToUrlMap(requestContext, projectId, artifactKind, list);
      foreach (IRenderedText comment in comments)
        this.HandleAttachments(urlMap, comment);
    }

    private void HandleAttachments(
      Dictionary<string, string> artifactToUrlMap,
      IRenderedText comment)
    {
      string newValue;
      if (!artifactToUrlMap.TryGetValue(comment.ArtifactId, out newValue) || string.IsNullOrWhiteSpace(newValue))
        return;
      comment.Text = comment.Text.ReplaceCharWithString('\u0006', newValue);
      comment.RenderedText = comment.RenderedText.ReplaceCharWithString('\u0006', newValue);
    }

    private Dictionary<string, string> BuildArtifactToUrlMap(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      List<string> artifactIds)
    {
      Dictionary<string, string> urlMap = new Dictionary<string, string>(artifactIds.Count);
      foreach (string artifactId in artifactIds)
      {
        string commentAttachmentsUri = this.GetCommentAttachmentsUri(requestContext, projectId, artifactKind, artifactId);
        urlMap.Add(artifactId, commentAttachmentsUri);
      }
      return urlMap;
    }

    private string GetCommentAttachmentsUri(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId)
    {
      return this.GetProvider(requestContext, artifactKind).GetAttachmentsUrl(requestContext, projectId, artifactId);
    }

    private void SetChangedByAndDate(
      IVssRequestContext requestContext,
      IEnumerable<IChangedBy> comments)
    {
      DateTime utcNow = this.dateTimeProvider.UtcNow;
      Guid userId = requestContext.GetUserId();
      foreach (IChangedBy comment in comments)
      {
        comment.ChangedDate = utcNow;
        comment.ChangedBy = userId;
      }
    }

    private void ValidateResultCountMatch(
      IVssRequestContext requestContext,
      ISet<int> expectedIds,
      IList<Comment> commentResults)
    {
      if (commentResults.Count < expectedIds.Count)
      {
        IEnumerable<int> second = commentResults.Select<Comment, int>((Func<Comment, int>) (c => c.CommentId));
        throw new CommentNotFoundException(expectedIds.Except<int>(second).First<int>()).Expected(requestContext.ServiceName);
      }
    }

    private void HandleExpandOptions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      IEnumerable<Comment> results,
      ExpandOptions expandOptions)
    {
      bool flag = expandOptions.HasFlag((Enum) ExpandOptions.Reactions);
      if (expandOptions.HasFlag((Enum) ExpandOptions.Children))
      {
        foreach (Comment result in results)
        {
          if (result.Children != null && result.Children.Comments.Count > 0)
          {
            results = results.Union<Comment>((IEnumerable<Comment>) result.Children.Comments);
            this.UpdateContinuationToken(result.Children, 6);
          }
        }
      }
      this.HandleAttachments(requestContext, projectId, artifactKind, (IEnumerable<IRenderedText>) results);
      HashSet<int> hashSet = results.ToHashSet<Comment, int>((Func<Comment, int>) (c => c.CommentId));
      if (!flag)
        return;
      this.GetReactionsByCommentIds(requestContext, projectId, artifactKind, artifactId, (ISet<int>) hashSet, results);
    }

    private void UpdateContinuationToken(CommentsList commentsList, int topParam)
    {
      List<Comment> commentList1;
      if (commentsList == null)
      {
        commentList1 = (List<Comment>) null;
      }
      else
      {
        IReadOnlyCollection<Comment> comments = commentsList.Comments;
        commentList1 = comments != null ? comments.ToList<Comment>() : (List<Comment>) null;
      }
      List<Comment> source = commentList1;
      if (source == null || source.Count != topParam)
        return;
      CommentsList commentsList1 = commentsList;
      DateTime dateTime = source.Last<Comment>().CreatedDate;
      dateTime = dateTime.ToUniversalTime();
      string str = dateTime.ToString(CommentService.ContinuationTokenFormat, (IFormatProvider) CultureInfo.InvariantCulture);
      commentsList1.ContinuationToken = str;
      List<Comment> commentList2;
      commentsList.Comments = (IReadOnlyCollection<Comment>) (commentList2 = new List<Comment>(source.Take<Comment>(source.Count - 1)));
    }

    private void GetReactionsByCommentIds(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      ISet<int> ids,
      IEnumerable<Comment> comments)
    {
      ICommentReactionService service = requestContext.GetService<ICommentReactionService>();
      ISet<int> intSet = ids;
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId1 = projectId;
      Guid artifactKind1 = artifactKind;
      string artifactId1 = artifactId;
      ISet<int> commentIds = intSet;
      IList<CommentReaction> commentReactions = service.GetCommentReactions(requestContext1, projectId1, artifactKind1, artifactId1, commentIds);
      this.MergeReactions(comments, (IEnumerable<CommentReaction>) commentReactions);
    }

    private void MergeReactions(
      IEnumerable<Comment> comments,
      IEnumerable<CommentReaction> reactions)
    {
      Dictionary<int, Comment> commentDictionary = comments.ToDictionary<Comment, int>((Func<Comment, int>) (comment => comment.CommentId));
      reactions.ForEach<CommentReaction>((Action<CommentReaction>) (reaction => commentDictionary[reaction.CommentId].AddReaction(reaction)));
    }

    private ExpandOptions NormalizeExpandOptions(
      IVssRequestContext requestContext,
      Guid artifactKind,
      ExpandOptions expandOptions)
    {
      if (expandOptions == ExpandOptions.All && !this.SupportsThreading(requestContext, artifactKind))
        expandOptions &= ~ExpandOptions.Children;
      return expandOptions;
    }

    private void CheckSupportsThreading(
      IVssRequestContext requestContext,
      Guid artifactKind,
      ExpandOptions expandOptions)
    {
      if (expandOptions.HasFlag((Enum) ExpandOptions.Children) && !this.SupportsThreading(requestContext, artifactKind))
        throw new CommentThreadingNotSupportedException(artifactKind).Expected(requestContext.ServiceName);
    }

    private bool SupportsThreading(IVssRequestContext requestContext, Guid artifactKind) => this.GetProvider(requestContext, artifactKind).SupportsThreading;

    private void CheckReadPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      out ISecuredObject securedObject)
    {
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId1 = projectId;
      Guid artifactKind1 = artifactKind;
      List<string> artifactIds = new List<string>();
      artifactIds.Add(artifactId);
      IDictionary<string, ISecuredObject> dictionary;
      ref IDictionary<string, ISecuredObject> local = ref dictionary;
      this.CheckReadPermission(requestContext1, projectId1, artifactKind1, (IEnumerable<string>) artifactIds, out local);
      dictionary.TryGetValue(artifactId, out securedObject);
    }

    private void CheckReadPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      IEnumerable<string> artifactIds,
      out IDictionary<string, ISecuredObject> securedObjects)
    {
      this.GetProvider(requestContext, artifactKind).CheckReadPermission(requestContext, projectId, (ISet<string>) artifactIds.ToHashSet<string>(), out securedObjects);
    }

    private void CheckAddPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      IEnumerable<string> artifactIds,
      out IDictionary<string, ISecuredObject> securedObjects)
    {
      this.GetProvider(requestContext, artifactKind).CheckAddPermission(requestContext, projectId, (ISet<string>) artifactIds.ToHashSet<string>(), out securedObjects);
    }

    private void CheckUpdatePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      IEnumerable<Microsoft.TeamFoundation.Comments.Server.UpdateComment> commentsToBeUpdated,
      out IDictionary<int, ISecuredObject> securedObjects)
    {
      IEnumerable<Comment> commentSet = this.GetCommentSet(requestContext, projectId, artifactKind, (IEnumerable<IComment>) commentsToBeUpdated);
      this.GetProvider(requestContext, artifactKind).CheckUpdatePermission(requestContext, projectId, commentSet, out securedObjects);
    }

    private void CheckDeletePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      IEnumerable<Microsoft.TeamFoundation.Comments.Server.DeleteComment> commentsToBeDeleted,
      out IDictionary<int, ISecuredObject> securedObjects)
    {
      IEnumerable<Comment> commentSet = this.GetCommentSet(requestContext, projectId, artifactKind, (IEnumerable<IComment>) commentsToBeDeleted);
      this.GetProvider(requestContext, artifactKind).CheckDeletePermission(requestContext, projectId, commentSet, out securedObjects);
    }

    private IEnumerable<Comment> GetCommentSet(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      IEnumerable<IComment> inputComments)
    {
      List<Comment> comments = new List<Comment>();
      IDictionary<string, List<int>> commentsPerArtifact = (IDictionary<string, List<int>>) new Dictionary<string, List<int>>();
      inputComments.ForEach<IComment>((Action<IComment>) (comment =>
      {
        List<int> intList;
        if (!commentsPerArtifact.TryGetValue(comment.ArtifactId, out intList))
        {
          intList = new List<int>();
          commentsPerArtifact[comment.ArtifactId] = intList;
        }
        intList.Add(comment.CommentId);
      }));
      commentsPerArtifact.Keys.ForEach<string>((Action<string>) (artifactId => comments.AddRange((IEnumerable<Comment>) this.GetComments(requestContext, projectId, artifactKind, artifactId, (ISet<int>) commentsPerArtifact[artifactId].ToHashSet<int>(), ExpandOptions.None, false).Comments)));
      return (IEnumerable<Comment>) comments;
    }

    private void SecureCommentsByCommentId(
      IEnumerable<Comment> commentResults,
      IDictionary<int, ISecuredObject> securedObjects)
    {
      if (securedObjects == null || commentResults == null)
        return;
      commentResults.ForEach<Comment>((Action<Comment>) (c =>
      {
        ISecuredObject securedObject;
        if (!securedObjects.TryGetValue(c.CommentId, out securedObject) || securedObject == null)
          return;
        c.SetSecuredObject(securedObject);
      }));
    }

    private void SecureCommentsByArtifactId(
      IEnumerable<Comment> commentResults,
      IDictionary<string, ISecuredObject> securedObjects)
    {
      if (securedObjects == null || commentResults == null)
        return;
      commentResults.ForEach<Comment>((Action<Comment>) (c =>
      {
        ISecuredObject securedObject;
        if (!securedObjects.TryGetValue(c.ArtifactId, out securedObject) || securedObject == null)
          return;
        c.SetSecuredObject(securedObject);
      }));
    }

    private static CommentMentionContext CreateCommentMentionContext(
      IVssRequestContext requestContext,
      Guid projectId,
      ICommentProvider commentProvider,
      IRenderedText comment,
      Guid mentioner)
    {
      ArtifactInfo artifactInfo = commentProvider.GetArtifactInfo(requestContext, projectId, comment.ArtifactId);
      CommentFormat commentFormat = commentProvider.GetCommentFormat(requestContext, projectId);
      return new CommentMentionContext(projectId, commentProvider.ArtifactKind, commentProvider.ArtifactFriendlyName, comment.ArtifactId, artifactInfo.ArtifactTitle, artifactInfo.ArtifactUri, new int?(), comment.Text, commentFormat == CommentFormat.Markdown ? MentionContentType.PlainText : MentionContentType.RichText, mentioner, !commentProvider.SupportsMentions);
    }
  }
}
