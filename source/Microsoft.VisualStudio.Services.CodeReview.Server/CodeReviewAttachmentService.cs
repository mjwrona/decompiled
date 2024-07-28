// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewAttachmentService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Server.Utils;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi.Events;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class CodeReviewAttachmentService : 
    CodeReviewServiceBase,
    ICodeReviewAttachmentService,
    IVssFrameworkService
  {
    public Attachment GetAttachment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int attachmentId)
    {
      Attachment attachment = (Attachment) null;
      this.ExecuteAndTrace(requestContext, nameof (GetAttachment), 1383011, 1383012, 1383012, (Action) (() =>
      {
        requestContext.Trace(1383014, TraceLevel.Verbose, this.Area, this.Layer, "Getting an attachment: review id: '{0}', attachment id: '{1}', project id: '{2}'", (object) reviewId, (object) attachmentId, (object) projectId);
        attachment = this.GetAttachmentsInternal(requestContext, projectId, reviewId, new int?(attachmentId)).FirstOrDefault<Attachment>();
        if (attachment == null)
          throw new CodeReviewAttachmentNotFoundException(attachmentId, reviewId);
      }));
      return attachment;
    }

    public virtual IEnumerable<Attachment> GetAttachments(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      DateTime? modifiedSince = null)
    {
      IEnumerable<Attachment> attachments = (IEnumerable<Attachment>) null;
      this.ExecuteAndTrace(requestContext, nameof (GetAttachments), 1383021, 1383022, 1383023, (Action) (() =>
      {
        requestContext.Trace(1383024, TraceLevel.Verbose, this.Area, this.Layer, "Getting attachments: review id: '{0}', project id: '{1}', modifiedSince: '{2}'", (object) reviewId, (object) projectId, (object) modifiedSince);
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        int reviewId1 = reviewId;
        DateTime? nullable = modifiedSince;
        int? attachmentId = new int?();
        DateTime? modifiedSince1 = nullable;
        attachments = this.GetAttachmentsInternal(requestContext1, projectId1, reviewId1, attachmentId, modifiedSince1);
      }));
      return attachments;
    }

    protected IEnumerable<Attachment> GetAttachmentsInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int? attachmentId = null,
      DateTime? modifiedSince = null)
    {
      IList<Attachment> attachments = (IList<Attachment>) null;
      this.ValidateReviewAttachmentIds(reviewId, attachmentId);
      DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
      Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
      ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, reviewRaw.SourceArtifactId);
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
        attachments = component.GetAttachments(projectId, reviewId, attachmentId, modifiedSince);
      this.FetchAttachmentProperties(requestContext, projectId, reviewId, attachments);
      return IdentityHelper.FillAuthorIdentities(requestContext, (IEnumerable<Attachment>) attachments);
    }

    public virtual Attachment SaveAttachment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      Attachment attachment)
    {
      Attachment savedAttachment = (Attachment) null;
      this.ExecuteAndTrace(requestContext, nameof (SaveAttachment), 1383001, 1383002, 1383003, (Action) (() =>
      {
        ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
        this.ValidateAttachment(attachment);
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        attachment = this.SanitizeAttachmentInput(requestContext, attachment);
        requestContext.Trace(1383004, TraceLevel.Verbose, this.Area, this.Layer, "Saving attachment: review id: '{0}', project id: '{1}', displayName: '{2}'", (object) reviewId, (object) projectId, (object) attachment.DisplayName);
        Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
        ReviewSecurityEvaluator.CheckWriteReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewRaw.Id, reviewRaw.SourceArtifactId);
        List<ReviewFileContentInfo> metadataInternal = this.GetContentMetadataInternal(requestContext, projectId, reviewId, (IEnumerable<byte[]>) new List<byte[]>()
        {
          ReviewFileContentExtensions.ToSha1HashBytes(attachment.ContentHash)
        });
        Guid contentId = metadataInternal.Count == 1 ? metadataInternal.First<ReviewFileContentInfo>().ContentId : throw new CodeReviewAttachmentCannotBeCreatedBeforeFileUploadException(reviewRaw.Id);
        Guid projectId1 = projectId;
        Review review1 = reviewRaw;
        List<Attachment> attachmentList1 = new List<Attachment>();
        attachmentList1.Add(attachment);
        DateTime? updatedDate = reviewRaw.UpdatedDate;
        DateTime? createdDate1 = attachment.CreatedDate;
        EventServiceHelper.PublishDecisionPoint(requestContext, (CodeReviewEventNotification) new AttachmentAddedNotification(projectId1, review1, (IEnumerable<Attachment>) attachmentList1, updatedDate, createdDate1));
        DateTime? priorReviewUpdatedTimestamp1;
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          savedAttachment = component.SaveAttachment(projectId, reviewId, attachment, contentId, out priorReviewUpdatedTimestamp1);
        savedAttachment = this.SaveAttachmentProperties(requestContext, projectId, reviewId, attachment, savedAttachment);
        savedAttachment = IdentityHelper.FillAuthorIdentities(requestContext, (IEnumerable<Attachment>) new Attachment[1]
        {
          savedAttachment
        }).First<Attachment>();
        Review review2 = reviewRaw.ShallowClone();
        review2.UpdatedDate = savedAttachment.CreatedDate;
        Guid projectId2 = projectId;
        Review review3 = review2;
        List<Attachment> attachmentList2 = new List<Attachment>();
        attachmentList2.Add(savedAttachment);
        DateTime? priorReviewUpdatedTimestamp2 = priorReviewUpdatedTimestamp1;
        DateTime? createdDate2 = savedAttachment.CreatedDate;
        EventServiceHelper.PublishNotification(requestContext, (CodeReviewEventNotification) new AttachmentAddedNotification(projectId2, review3, (IEnumerable<Attachment>) attachmentList2, priorReviewUpdatedTimestamp2, createdDate2), this.Area, this.Layer);
      }));
      return savedAttachment;
    }

    public virtual void DeleteAttachment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int attachmentId,
      bool deleteFile = false)
    {
      this.ExecuteAndTrace(requestContext, nameof (DeleteAttachment), 1383031, 1383032, 1383033, (Action) (() =>
      {
        requestContext.Trace(1383034, TraceLevel.Verbose, this.Area, this.Layer, "Deleting an attachment: review id: '{0}', attachment id: '{1}', project id: '{2}'", (object) reviewId, (object) attachmentId, (object) projectId);
        this.DeleteAttachmentsInternal(requestContext, projectId, reviewId, attachmentId, deleteFile);
      }));
    }

    protected virtual void DeleteAttachmentsInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int attachmentId,
      bool deleteFile = false)
    {
      this.ValidateReviewAttachmentIds(reviewId, new int?(attachmentId));
      DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
      Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
      ReviewSecurityEvaluator.CheckWriteReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewRaw.Id, reviewRaw.SourceArtifactId);
      Attachment attachment = this.GetAttachmentsInternal(requestContext, projectId, reviewId, new int?(attachmentId)).FirstOrDefault<Attachment>();
      if (attachment == null)
        throw new CodeReviewAttachmentNotFoundException(attachmentId, reviewId);
      if (!IdentityHelper.CompareRequesterIdentity(requestContext, attachment.Author.Id) && !this.IsUserProjectAdmin(requestContext, projectId))
        throw new ArgumentException(CodeReviewResources.AttachmentDeleteInvalidAuthor());
      Guid projectId1 = projectId;
      Review review1 = reviewRaw;
      List<Attachment> attachmentList1 = new List<Attachment>();
      attachmentList1.Add(attachment);
      DateTime? updatedDate = reviewRaw.UpdatedDate;
      DateTime? createdDate = attachment.CreatedDate;
      CodeReviewEventNotification crEvent1 = (CodeReviewEventNotification) new AttachmentDeletedNotification(projectId1, review1, (IEnumerable<Attachment>) attachmentList1, updatedDate, createdDate);
      EventServiceHelper.PublishDecisionPoint(requestContext, crEvent1);
      UpdateTimestamps reviewUpdatedTimestamps;
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
        component.DeleteAttachments(projectId, reviewId, out reviewUpdatedTimestamps, new int?(attachmentId));
      if (deleteFile)
      {
        int fileServiceFileId = this.GetContentMetadataInternal(requestContext, projectId, reviewId, (IEnumerable<byte[]>) new List<byte[]>()
        {
          ReviewFileContentExtensions.ToSha1HashBytes(attachment.ContentHash)
        }).Single<ReviewFileContentInfo>().FileServiceFileId;
        requestContext.GetService<ITeamFoundationFileService>().DeleteFile(requestContext, (long) fileServiceFileId);
      }
      Review review2 = reviewRaw.ShallowClone();
      review2.UpdatedDate = new DateTime?(reviewUpdatedTimestamps.Current);
      Guid projectId2 = projectId;
      Review review3 = review2;
      List<Attachment> attachmentList2 = new List<Attachment>();
      attachmentList2.Add(attachment);
      DateTime? priorReviewUpdatedTimestamp = new DateTime?(reviewUpdatedTimestamps.Prior);
      DateTime? latestReviewUpdatedTimestamp = new DateTime?(reviewUpdatedTimestamps.Current);
      CodeReviewEventNotification crEvent2 = (CodeReviewEventNotification) new AttachmentDeletedNotification(projectId2, review3, (IEnumerable<Attachment>) attachmentList2, priorReviewUpdatedTimestamp, latestReviewUpdatedTimestamp);
      EventServiceHelper.PublishNotification(requestContext, crEvent2, this.Area, this.Layer);
    }

    private void ValidateReviewAttachmentIds(int reviewId, int? attachmentId)
    {
      ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
      if (!attachmentId.HasValue)
        return;
      ArgumentUtility.CheckForOutOfRange(attachmentId.Value, "id", 1);
    }

    private void ValidateAttachment(Attachment attachment)
    {
      if (attachment.Id != 0)
        throw new ArgumentException(CodeReviewResources.CannotSpecifyAttachmentId(), "attachmentId");
      if (attachment.Author != null)
        throw new ArgumentException(CodeReviewResources.CannotSpecifyAuthor(), "author");
      if (attachment.CreatedDate.HasValue)
        throw new ArgumentException(CodeReviewResources.CannotSpecifyCreatedDate(), "createdDate");
      ArgumentUtility.CheckStringForNullOrEmpty(attachment.DisplayName, "displayName");
      ArgumentUtility.CheckStringForNullOrEmpty(attachment.ContentHash, "contentHash");
    }

    private Attachment SanitizeAttachmentInput(
      IVssRequestContext requestContext,
      Attachment attachment)
    {
      attachment.Author = IdentityHelper.GetRequesterIdentityRef(requestContext);
      return attachment;
    }

    private Attachment SaveAttachmentProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      Attachment attachmentToSave,
      Attachment savedAttachment)
    {
      string attachmentMoniker = ArtifactPropertyKinds.GetAttachmentMoniker(projectId, savedAttachment);
      ArtifactPropertyKinds.SaveProperties(requestContext, ArtifactPropertyKinds.MakeArtifactSpec(ServerConstants.AttachmentPropertyKind, attachmentMoniker), attachmentToSave.Properties);
      savedAttachment.Properties = attachmentToSave.Properties;
      savedAttachment.AddReferenceLinks(requestContext, projectId, reviewId, savedAttachment.Id);
      return savedAttachment;
    }

    private void FetchAttachmentProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      IList<Attachment> attachments)
    {
      ArtifactPropertyKinds.FetchAttachmentExtendedProperties(requestContext, projectId, attachments);
      if (attachments == null || !attachments.Any<Attachment>())
        return;
      foreach (Attachment attachment in (IEnumerable<Attachment>) attachments)
        attachment.AddReferenceLinks(requestContext, projectId, reviewId, attachment.Id);
    }
  }
}
