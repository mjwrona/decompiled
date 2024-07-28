// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewShareService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Server.Utils;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi.Events;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class CodeReviewShareService : 
    CodeReviewServiceBase,
    ICodeReviewShareService,
    IVssFrameworkService
  {
    private const int c_defaultMaxReceiversCount = 100;
    private const int c_defaultMaxMessageLength = 1024;
    private const int c_defaultMaxSubjectLength = 256;

    public void ShareReview(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      NotificationContext userMessage)
    {
      this.ExecuteAndTrace(requestContext, 1383801, 1383802, 1383803, (Action) (() =>
      {
        this.TraceShareInfo(requestContext, projectId, reviewId, userMessage, 1383804, "Share review");
        this.ShareReviewInternal(requestContext, projectId, reviewId, userMessage);
      }), nameof (ShareReview));
    }

    protected virtual void ShareReviewInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      NotificationContext userMessage)
    {
      ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
      ArgumentUtility.CheckForNull<NotificationContext>(userMessage, nameof (userMessage));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) userMessage.Receivers, "Receivers");
      int maxReceivers = this.GetMaxReceivers(requestContext);
      if (userMessage.Receivers.Count > maxReceivers)
        throw new ArgumentException(CodeReviewResources.CannotShareReviewExceededMaxReceivers((object) userMessage.Receivers.Count, (object) maxReceivers), "Receivers");
      int maxMessageLength = this.GetMaxMessageLength(requestContext);
      if (userMessage.Message != null && userMessage.Message.Length > maxMessageLength)
        throw new ArgumentException(CodeReviewResources.CannotShareReviewExceededMaxMessageLength((object) userMessage.Message.Length, (object) maxMessageLength), "Message");
      int maxSubjectLength = this.GetMaxSubjectLength(requestContext);
      if (userMessage.Subject != null && userMessage.Subject.Length > maxSubjectLength)
        throw new ArgumentException(CodeReviewResources.CannotShareReviewExceededMaxSubjectLength((object) userMessage.Subject.Length, (object) maxSubjectLength), "Subject");
      DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
      Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
      ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, reviewRaw.SourceArtifactId);
      Dictionary<string, IdentityRef> vsoIdentities = IdentityHelper.GetVsoIdentities(requestContext, (IList<Guid>) new List<Guid>()
      {
        new Guid(reviewRaw.Author.Id)
      });
      reviewRaw.Author = IdentityHelper.GetMatchedIdentity(reviewRaw.Author, vsoIdentities);
      ShareReviewNotification notificationMessage = this.GetNotificationMessage(requestContext, projectId, reviewRaw, userMessage);
      EventServiceHelper.PublishNotification(requestContext, (CodeReviewEventNotification) notificationMessage, this.Area, this.Layer);
    }

    private ShareReviewNotification GetNotificationMessage(
      IVssRequestContext requestContext,
      Guid projectId,
      Review baseReview,
      NotificationContext userMessage)
    {
      ShareReviewNotification notificationMessage = new ShareReviewNotification(projectId, baseReview);
      IReviewNotificationContentProvider contentExtension = this.GetReviewNotificationContentExtension(baseReview.SourceArtifactId);
      if (contentExtension != null)
      {
        ArtifactId artifact = LinkingUtilities.DecodeUri(baseReview.SourceArtifactId);
        notificationMessage.ExternalToolContext = contentExtension.GetToolContext(requestContext, artifact);
      }
      else
        notificationMessage.ExternalToolContext = new ToolContext()
        {
          Caption = CodeReviewResources.DefaultNotificationToolCaption(),
          Id = baseReview.Id.ToString()
        };
      notificationMessage.ShareMessage = userMessage;
      return notificationMessage;
    }

    private void TraceShareInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      NotificationContext userMessage,
      int tracePoint,
      string description)
    {
      if (!requestContext.IsTracing(tracePoint, TraceLevel.Verbose, this.Area, this.Layer))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Subject: '" + userMessage.Subject + "', ");
      stringBuilder.Append("Body: '" + userMessage.Message + "', ");
      stringBuilder.Append("Receivers:");
      foreach (IdentityRef receiver in userMessage.Receivers)
        stringBuilder.Append("'" + receiver.Id + "', ");
      requestContext.Trace(tracePoint, TraceLevel.Verbose, this.Area, this.Layer, string.Format("{0}: review id: '{1}', project id: '{2}', User message: {3}", (object) description, (object) reviewId, (object) projectId, (object) stringBuilder.ToString()));
    }

    private int GetMaxReceivers(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/CodeReview/CommentTracking/ShareReceiversLimit", 100);

    private int GetMaxMessageLength(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/CodeReview/CommentTracking/ShareMessageLimit", 1024);

    private int GetMaxSubjectLength(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/CodeReview/CommentTracking/ShareSubjectLimit", 256);
  }
}
