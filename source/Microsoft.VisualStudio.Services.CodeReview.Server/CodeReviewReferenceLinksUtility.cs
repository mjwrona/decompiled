// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewReferenceLinksUtility
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal static class CodeReviewReferenceLinksUtility
  {
    internal static void AddReferenceLinks(
      this Review review,
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId)
    {
      if (review.Iterations != null)
      {
        foreach (Iteration iteration in (IEnumerable<Iteration>) review.Iterations)
          iteration.AddReferenceLinks(requestContext, projectId, reviewId, iteration.Id.Value);
      }
      if (review.Attachments != null)
      {
        foreach (Attachment attachment in (IEnumerable<Attachment>) review.Attachments)
          attachment.AddReferenceLinks(requestContext, projectId, reviewId, attachment.Id);
      }
      review.Links = new ReferenceLinks();
      review.Links.AddLink("self", CodeReviewReferenceLinksUtility.GetReviewUrl(requestContext, projectId, reviewId).ToString());
    }

    internal static void AddReferenceLinks(
      this Iteration iteration,
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int iterationId)
    {
      if (iteration.ChangeList != null)
      {
        foreach (ChangeEntry change in (IEnumerable<ChangeEntry>) iteration.ChangeList)
          change.PopulateReferenceLinksAndNormalizeOutputs(requestContext, projectId, reviewId);
      }
      iteration.Links = new ReferenceLinks();
      iteration.Links.AddLink("self", CodeReviewReferenceLinksUtility.GetIterationUrl(requestContext, projectId, reviewId, iterationId).ToString());
      iteration.Links.AddLink("review", CodeReviewReferenceLinksUtility.GetReviewUrl(requestContext, projectId, reviewId).ToString());
    }

    internal static void AddReferenceLinks(
      this ChangeEntryFileInfo entryFile,
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      string contentHash)
    {
      entryFile.Links = new ReferenceLinks();
      string str = CodeReviewReferenceLinksUtility.GetChangesContentUrl(requestContext, projectId, reviewId).ToString();
      NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
      queryString[nameof (contentHash)] = contentHash;
      entryFile.Links.AddLink("content", str + "?" + queryString?.ToString());
    }

    internal static void AddReferenceLinks(
      this Attachment attachment,
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int attachmentId)
    {
      attachment.Links = new ReferenceLinks();
      attachment.Links.AddLink("self", CodeReviewReferenceLinksUtility.GetAttachmentUrl(requestContext, projectId, reviewId, attachmentId).ToString());
      attachment.Links.AddLink("review", CodeReviewReferenceLinksUtility.GetReviewUrl(requestContext, projectId, reviewId).ToString());
    }

    internal static void AddReferenceLinks(
      this Status status,
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int statusId,
      int? iterationId = null)
    {
      status.Links = new ReferenceLinks();
      status.Links.AddLink("self", CodeReviewReferenceLinksUtility.GetStatusUrl(requestContext, projectId, reviewId, statusId, iterationId).ToString());
      if (iterationId.HasValue)
        status.Links.AddLink("iteration", CodeReviewReferenceLinksUtility.GetIterationUrl(requestContext, projectId, reviewId, iterationId.Value).ToString());
      status.Links.AddLink("review", CodeReviewReferenceLinksUtility.GetReviewUrl(requestContext, projectId, reviewId).ToString());
    }

    internal static void AddReferenceLinks(
      this ReviewSettings settings,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      settings.Links = new ReferenceLinks();
      settings.Links.AddLink("self", CodeReviewReferenceLinksUtility.GetReviewSettingsUrl(requestContext, projectId).ToString());
    }

    internal static void AddReferenceLinks(
      this CommentThread thread,
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      if (thread.Comments != null)
      {
        foreach (DiscussionComment comment in thread.Comments)
          comment.AddReferenceLinks(requestContext, projectId, reviewId, threadId, comment.CommentId, service);
      }
      thread.Links = new ReferenceLinks();
      thread.Links.AddLink("self", CodeReviewReferenceLinksUtility.GetCommentThreadUrl(requestContext, service, projectId, reviewId, threadId).ToString());
      thread.Links.AddLink("review", CodeReviewReferenceLinksUtility.GetReviewUrl(requestContext, projectId, reviewId).ToString());
    }

    internal static void AddReferenceLinks(
      this DiscussionComment comment,
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      short commentId,
      ILocationService locationService = null)
    {
      if (locationService == null)
        locationService = requestContext.GetService<ILocationService>();
      comment.Links = new ReferenceLinks();
      comment.Links.AddLink("self", CodeReviewReferenceLinksUtility.GetCommentUrl(requestContext, locationService, projectId, reviewId, threadId, commentId).ToString());
      if (comment.ParentCommentId > (short) 0)
        comment.Links.AddLink("parent", CodeReviewReferenceLinksUtility.GetCommentUrl(requestContext, locationService, projectId, reviewId, threadId, comment.ParentCommentId).ToString());
      comment.Links.AddLink("review", CodeReviewReferenceLinksUtility.GetReviewUrl(requestContext, projectId, reviewId).ToString());
    }

    private static Uri GetReviewUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>();
      if (projectId != Guid.Empty)
        routeValues["project"] = (object) projectId;
      routeValues[nameof (reviewId)] = (object) reviewId;
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "CodeReview", CodeReviewConstants.ReviewsResourceId, (object) routeValues);
    }

    private static Uri GetIterationUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int iterationId)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>();
      if (projectId != Guid.Empty)
        routeValues["project"] = (object) projectId;
      routeValues[nameof (reviewId)] = (object) reviewId;
      routeValues[nameof (iterationId)] = (object) iterationId;
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "CodeReview", CodeReviewConstants.IterationsResourceId, (object) routeValues);
    }

    private static Uri GetChangesContentUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>();
      if (projectId != Guid.Empty)
        routeValues["project"] = (object) projectId;
      routeValues[nameof (reviewId)] = (object) reviewId;
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "CodeReview", CodeReviewConstants.ChangesContentResourceId, (object) routeValues);
    }

    private static Uri GetAttachmentUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int attachmentId)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>();
      if (projectId != Guid.Empty)
        routeValues["project"] = (object) projectId;
      routeValues[nameof (reviewId)] = (object) reviewId;
      routeValues[nameof (attachmentId)] = (object) attachmentId;
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "CodeReview", CodeReviewConstants.AttachmentsResourceId, (object) routeValues);
    }

    private static Uri GetStatusUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int statusId,
      int? iterationId = null)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>();
      if (projectId != Guid.Empty)
        routeValues["project"] = (object) projectId;
      routeValues[nameof (reviewId)] = (object) reviewId;
      routeValues[nameof (statusId)] = (object) statusId;
      ILocationService service = requestContext.GetService<ILocationService>();
      Uri resourceUri;
      if (iterationId.HasValue)
      {
        routeValues[nameof (iterationId)] = (object) iterationId.Value;
        resourceUri = service.GetResourceUri(requestContext, "CodeReview", CodeReviewConstants.IterationStatusesResourceId, (object) routeValues);
      }
      else
        resourceUri = service.GetResourceUri(requestContext, "CodeReview", CodeReviewConstants.ReviewStatusesResourceId, (object) routeValues);
      return resourceUri;
    }

    private static Uri GetReviewSettingsUrl(IVssRequestContext requestContext, Guid projectId)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>();
      if (projectId != Guid.Empty)
        routeValues["project"] = (object) projectId;
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "CodeReview", CodeReviewConstants.ReviewSettingsResourceId, (object) routeValues);
    }

    private static Uri GetCommentThreadUrl(
      IVssRequestContext requestContext,
      ILocationService locationService,
      Guid projectId,
      int reviewId,
      int threadId)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>();
      if (projectId != Guid.Empty)
        routeValues["project"] = (object) projectId;
      routeValues[nameof (reviewId)] = (object) reviewId;
      routeValues[nameof (threadId)] = (object) threadId;
      return locationService.GetResourceUri(requestContext, "CodeReview", CodeReviewConstants.ReviewCommentThreadsResourceId, (object) routeValues);
    }

    private static Uri GetCommentUrl(
      IVssRequestContext requestContext,
      ILocationService locationService,
      Guid projectId,
      int reviewId,
      int threadId,
      short commentId)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>();
      if (projectId != Guid.Empty)
        routeValues["project"] = (object) projectId;
      routeValues[nameof (reviewId)] = (object) reviewId;
      routeValues[nameof (threadId)] = (object) threadId;
      routeValues[nameof (commentId)] = (object) threadId;
      return locationService.GetResourceUri(requestContext, "CodeReview", CodeReviewConstants.ReviewCommentsResourceId, (object) routeValues);
    }
  }
}
