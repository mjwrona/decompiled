// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewResources
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal static class CodeReviewResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (CodeReviewResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => CodeReviewResources.s_resMgr;

    private static string Get(string resourceName) => CodeReviewResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? CodeReviewResources.Get(resourceName) : CodeReviewResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) CodeReviewResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? CodeReviewResources.GetInt(resourceName) : (int) CodeReviewResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) CodeReviewResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? CodeReviewResources.GetBool(resourceName) : (bool) CodeReviewResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => CodeReviewResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = CodeReviewResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string CannotSpecifyCreatedDate() => CodeReviewResources.Get(nameof (CannotSpecifyCreatedDate));

    public static string CannotSpecifyCreatedDate(CultureInfo culture) => CodeReviewResources.Get(nameof (CannotSpecifyCreatedDate), culture);

    public static string CannotSpecifyIterationId() => CodeReviewResources.Get(nameof (CannotSpecifyIterationId));

    public static string CannotSpecifyIterationId(CultureInfo culture) => CodeReviewResources.Get(nameof (CannotSpecifyIterationId), culture);

    public static string CannotSpecifyReviewId() => CodeReviewResources.Get(nameof (CannotSpecifyReviewId));

    public static string CannotSpecifyReviewId(CultureInfo culture) => CodeReviewResources.Get(nameof (CannotSpecifyReviewId), culture);

    public static string CannotSpecifyAttachmentId() => CodeReviewResources.Get(nameof (CannotSpecifyAttachmentId));

    public static string CannotSpecifyAttachmentId(CultureInfo culture) => CodeReviewResources.Get(nameof (CannotSpecifyAttachmentId), culture);

    public static string CannotSpecifyUpdatedDate() => CodeReviewResources.Get(nameof (CannotSpecifyUpdatedDate));

    public static string CannotSpecifyUpdatedDate(CultureInfo culture) => CodeReviewResources.Get(nameof (CannotSpecifyUpdatedDate), culture);

    public static string CodeReviewArgumentNullException(object arg0) => CodeReviewResources.Format(nameof (CodeReviewArgumentNullException), arg0);

    public static string CodeReviewArgumentNullException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (CodeReviewArgumentNullException), culture, arg0);

    public static string CodeReviewNotFoundException(object arg0) => CodeReviewResources.Format(nameof (CodeReviewNotFoundException), arg0);

    public static string CodeReviewNotFoundException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (CodeReviewNotFoundException), culture, arg0);

    public static string CodeReviewOperationFailedException() => CodeReviewResources.Get(nameof (CodeReviewOperationFailedException));

    public static string CodeReviewOperationFailedException(CultureInfo culture) => CodeReviewResources.Get(nameof (CodeReviewOperationFailedException), culture);

    public static string ReviewerIdArgumentValidGuid() => CodeReviewResources.Get(nameof (ReviewerIdArgumentValidGuid));

    public static string ReviewerIdArgumentValidGuid(CultureInfo culture) => CodeReviewResources.Get(nameof (ReviewerIdArgumentValidGuid), culture);

    public static string ReviewerSaveFailedUponNullException(object arg0) => CodeReviewResources.Format(nameof (ReviewerSaveFailedUponNullException), arg0);

    public static string ReviewerSaveFailedUponNullException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (ReviewerSaveFailedUponNullException), culture, arg0);

    public static string ReviewerIdArgumentNullException() => CodeReviewResources.Get(nameof (ReviewerIdArgumentNullException));

    public static string ReviewerIdArgumentNullException(CultureInfo culture) => CodeReviewResources.Get(nameof (ReviewerIdArgumentNullException), culture);

    public static string InvalidIterationId(object arg0) => CodeReviewResources.Format(nameof (InvalidIterationId), arg0);

    public static string InvalidIterationId(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (InvalidIterationId), culture, arg0);

    public static string IterationArgumentNullException(object arg0) => CodeReviewResources.Format(nameof (IterationArgumentNullException), arg0);

    public static string IterationArgumentNullException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (IterationArgumentNullException), culture, arg0);

    public static string IterationMalformed() => CodeReviewResources.Get(nameof (IterationMalformed));

    public static string IterationMalformed(CultureInfo culture) => CodeReviewResources.Get(nameof (IterationMalformed), culture);

    public static string IterationNotFoundException(object arg0) => CodeReviewResources.Format(nameof (IterationNotFoundException), arg0);

    public static string IterationNotFoundException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (IterationNotFoundException), culture, arg0);

    public static string MismatchedIterationIds(object arg0, object arg1) => CodeReviewResources.Format(nameof (MismatchedIterationIds), arg0, arg1);

    public static string MismatchedIterationIds(object arg0, object arg1, CultureInfo culture) => CodeReviewResources.Format(nameof (MismatchedIterationIds), culture, arg0, arg1);

    public static string MismatchedReviewIds(object arg0, object arg1) => CodeReviewResources.Format(nameof (MismatchedReviewIds), arg0, arg1);

    public static string MismatchedReviewIds(object arg0, object arg1, CultureInfo culture) => CodeReviewResources.Format(nameof (MismatchedReviewIds), culture, arg0, arg1);

    public static string MustSpecifyIterationId() => CodeReviewResources.Get(nameof (MustSpecifyIterationId));

    public static string MustSpecifyIterationId(CultureInfo culture) => CodeReviewResources.Get(nameof (MustSpecifyIterationId), culture);

    public static string CannotSpecifyAuthor() => CodeReviewResources.Get(nameof (CannotSpecifyAuthor));

    public static string CannotSpecifyAuthor(CultureInfo culture) => CodeReviewResources.Get(nameof (CannotSpecifyAuthor), culture);

    public static string CannotDeleteIteration() => CodeReviewResources.Get(nameof (CannotDeleteIteration));

    public static string CannotDeleteIteration(CultureInfo culture) => CodeReviewResources.Get(nameof (CannotDeleteIteration), culture);

    public static string CannotDeleteReview() => CodeReviewResources.Get(nameof (CannotDeleteReview));

    public static string CannotDeleteReview(CultureInfo culture) => CodeReviewResources.Get(nameof (CannotDeleteReview), culture);

    public static string CannotSpecifyCompletedDate() => CodeReviewResources.Get(nameof (CannotSpecifyCompletedDate));

    public static string CannotSpecifyCompletedDate(CultureInfo culture) => CodeReviewResources.Get(nameof (CannotSpecifyCompletedDate), culture);

    public static string SuppliedAuthorCouldNotBeParsed() => CodeReviewResources.Get(nameof (SuppliedAuthorCouldNotBeParsed));

    public static string SuppliedAuthorCouldNotBeParsed(CultureInfo culture) => CodeReviewResources.Get(nameof (SuppliedAuthorCouldNotBeParsed), culture);

    public static string ChangesAlreadyExistException(object arg0) => CodeReviewResources.Format(nameof (ChangesAlreadyExistException), arg0);

    public static string ChangesAlreadyExistException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (ChangesAlreadyExistException), culture, arg0);

    public static string InvalidReviewerStateId(object arg0, object arg1) => CodeReviewResources.Format(nameof (InvalidReviewerStateId), arg0, arg1);

    public static string InvalidReviewerStateId(object arg0, object arg1, CultureInfo culture) => CodeReviewResources.Format(nameof (InvalidReviewerStateId), culture, arg0, arg1);

    public static string DataspaceStateError(object arg0) => CodeReviewResources.Format(nameof (DataspaceStateError), arg0);

    public static string DataspaceStateError(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (DataspaceStateError), culture, arg0);

    public static string CannotVoteForAnotherReviewer(object arg0, object arg1, object arg2) => CodeReviewResources.Format(nameof (CannotVoteForAnotherReviewer), arg0, arg1, arg2);

    public static string CannotVoteForAnotherReviewer(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (CannotVoteForAnotherReviewer), culture, arg0, arg1, arg2);
    }

    public static string InvalidReviewers(object arg0) => CodeReviewResources.Format(nameof (InvalidReviewers), arg0);

    public static string InvalidReviewers(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (InvalidReviewers), culture, arg0);

    public static string FileNotFoundException(object arg0) => CodeReviewResources.Format(nameof (FileNotFoundException), arg0);

    public static string FileNotFoundException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (FileNotFoundException), culture, arg0);

    public static string InvalidPath(object arg0) => CodeReviewResources.Format(nameof (InvalidPath), arg0);

    public static string InvalidPath(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (InvalidPath), culture, arg0);

    public static string GzipUnsupportedWithoutLength() => CodeReviewResources.Get(nameof (GzipUnsupportedWithoutLength));

    public static string GzipUnsupportedWithoutLength(CultureInfo culture) => CodeReviewResources.Get(nameof (GzipUnsupportedWithoutLength), culture);

    public static string InvalidContentRange() => CodeReviewResources.Get(nameof (InvalidContentRange));

    public static string InvalidContentRange(CultureInfo culture) => CodeReviewResources.Get(nameof (InvalidContentRange), culture);

    public static string InvalidFileTargetType(object arg0, object arg1) => CodeReviewResources.Format(nameof (InvalidFileTargetType), arg0, arg1);

    public static string InvalidFileTargetType(object arg0, object arg1, CultureInfo culture) => CodeReviewResources.Format(nameof (InvalidFileTargetType), culture, arg0, arg1);

    public static string InvalidProvider(object arg0) => CodeReviewResources.Format(nameof (InvalidProvider), arg0);

    public static string InvalidProvider(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (InvalidProvider), culture, arg0);

    public static string ProviderAlreadyExists(object arg0) => CodeReviewResources.Format(nameof (ProviderAlreadyExists), arg0);

    public static string ProviderAlreadyExists(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (ProviderAlreadyExists), culture, arg0);

    public static string InvalidProject(object arg0) => CodeReviewResources.Format(nameof (InvalidProject), arg0);

    public static string InvalidProject(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (InvalidProject), culture, arg0);

    public static string IterationPropertiesCannotBeUpdated() => CodeReviewResources.Get(nameof (IterationPropertiesCannotBeUpdated));

    public static string IterationPropertiesCannotBeUpdated(CultureInfo culture) => CodeReviewResources.Get(nameof (IterationPropertiesCannotBeUpdated), culture);

    public static string ReviewPropertiesCannotBeUpdated() => CodeReviewResources.Get(nameof (ReviewPropertiesCannotBeUpdated));

    public static string ReviewPropertiesCannotBeUpdated(CultureInfo culture) => CodeReviewResources.Get(nameof (ReviewPropertiesCannotBeUpdated), culture);

    public static string ReviewWasMalformed() => CodeReviewResources.Get(nameof (ReviewWasMalformed));

    public static string ReviewWasMalformed(CultureInfo culture) => CodeReviewResources.Get(nameof (ReviewWasMalformed), culture);

    public static string IterationAlreadyExistsException(object arg0) => CodeReviewResources.Format(nameof (IterationAlreadyExistsException), arg0);

    public static string IterationAlreadyExistsException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (IterationAlreadyExistsException), culture, arg0);

    public static string IterationMismatchedIdsException(object arg0, object arg1) => CodeReviewResources.Format(nameof (IterationMismatchedIdsException), arg0, arg1);

    public static string IterationMismatchedIdsException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (IterationMismatchedIdsException), culture, arg0, arg1);
    }

    public static string ActionRejectedByPolicy() => CodeReviewResources.Get(nameof (ActionRejectedByPolicy));

    public static string ActionRejectedByPolicy(CultureInfo culture) => CodeReviewResources.Get(nameof (ActionRejectedByPolicy), culture);

    public static string IterationCanNotBeSaved(object arg0) => CodeReviewResources.Format(nameof (IterationCanNotBeSaved), arg0);

    public static string IterationCanNotBeSaved(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (IterationCanNotBeSaved), culture, arg0);

    public static string IterationsCannotBeUpdated() => CodeReviewResources.Get(nameof (IterationsCannotBeUpdated));

    public static string IterationsCannotBeUpdated(CultureInfo culture) => CodeReviewResources.Get(nameof (IterationsCannotBeUpdated), culture);

    public static string ReviewersCannotBeUpdated() => CodeReviewResources.Get(nameof (ReviewersCannotBeUpdated));

    public static string ReviewersCannotBeUpdated(CultureInfo culture) => CodeReviewResources.Get(nameof (ReviewersCannotBeUpdated), culture);

    public static string IterationCannotBeCreated() => CodeReviewResources.Get(nameof (IterationCannotBeCreated));

    public static string IterationCannotBeCreated(CultureInfo culture) => CodeReviewResources.Get(nameof (IterationCannotBeCreated), culture);

    public static string AttachmentsCannotBeCreated() => CodeReviewResources.Get(nameof (AttachmentsCannotBeCreated));

    public static string AttachmentsCannotBeCreated(CultureInfo culture) => CodeReviewResources.Get(nameof (AttachmentsCannotBeCreated), culture);

    public static string AttachmentsCannotBeUpdated() => CodeReviewResources.Get(nameof (AttachmentsCannotBeUpdated));

    public static string AttachmentsCannotBeUpdated(CultureInfo culture) => CodeReviewResources.Get(nameof (AttachmentsCannotBeUpdated), culture);

    public static string StatusesCannotBeCreated() => CodeReviewResources.Get(nameof (StatusesCannotBeCreated));

    public static string StatusesCannotBeCreated(CultureInfo culture) => CodeReviewResources.Get(nameof (StatusesCannotBeCreated), culture);

    public static string StatusesCannotBeUpdated() => CodeReviewResources.Get(nameof (StatusesCannotBeUpdated));

    public static string StatusesCannotBeUpdated(CultureInfo culture) => CodeReviewResources.Get(nameof (StatusesCannotBeUpdated), culture);

    public static string CannotCreateOrUpdateReviewWithIsDeletedTrue() => CodeReviewResources.Get(nameof (CannotCreateOrUpdateReviewWithIsDeletedTrue));

    public static string CannotCreateOrUpdateReviewWithIsDeletedTrue(CultureInfo culture) => CodeReviewResources.Get(nameof (CannotCreateOrUpdateReviewWithIsDeletedTrue), culture);

    public static string CodeReviewCannotBeUpdatedSinceItsNotActive(object arg0) => CodeReviewResources.Format(nameof (CodeReviewCannotBeUpdatedSinceItsNotActive), arg0);

    public static string CodeReviewCannotBeUpdatedSinceItsNotActive(
      object arg0,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (CodeReviewCannotBeUpdatedSinceItsNotActive), culture, arg0);
    }

    public static string CodeReviewCannotBeUpdateSinceDiffFileIdWasUnexpected(object arg0) => CodeReviewResources.Format(nameof (CodeReviewCannotBeUpdateSinceDiffFileIdWasUnexpected), arg0);

    public static string CodeReviewCannotBeUpdateSinceDiffFileIdWasUnexpected(
      object arg0,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (CodeReviewCannotBeUpdateSinceDiffFileIdWasUnexpected), culture, arg0);
    }

    public static string DuplicateChangeTrackingIdInChangeList(object arg0) => CodeReviewResources.Format(nameof (DuplicateChangeTrackingIdInChangeList), arg0);

    public static string DuplicateChangeTrackingIdInChangeList(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (DuplicateChangeTrackingIdInChangeList), culture, arg0);

    public static string ChangeIdCannotBeSet() => CodeReviewResources.Get(nameof (ChangeIdCannotBeSet));

    public static string ChangeIdCannotBeSet(CultureInfo culture) => CodeReviewResources.Get(nameof (ChangeIdCannotBeSet), culture);

    public static string MissingProjectId() => CodeReviewResources.Get(nameof (MissingProjectId));

    public static string MissingProjectId(CultureInfo culture) => CodeReviewResources.Get(nameof (MissingProjectId), culture);

    public static string IterationUpdateCannotIncludeChangeListException(object arg0) => CodeReviewResources.Format(nameof (IterationUpdateCannotIncludeChangeListException), arg0);

    public static string IterationUpdateCannotIncludeChangeListException(
      object arg0,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (IterationUpdateCannotIncludeChangeListException), culture, arg0);
    }

    public static string ChangesWithContentHashNotFoundException(object arg0) => CodeReviewResources.Format(nameof (ChangesWithContentHashNotFoundException), arg0);

    public static string ChangesWithContentHashNotFoundException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (ChangesWithContentHashNotFoundException), culture, arg0);

    public static string FileNotFoundForChangeIdException(object arg0, object arg1) => CodeReviewResources.Format(nameof (FileNotFoundForChangeIdException), arg0, arg1);

    public static string FileNotFoundForChangeIdException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (FileNotFoundForChangeIdException), culture, arg0, arg1);
    }

    public static string IterationMustBeOfUnpublishedTypeException(object arg0) => CodeReviewResources.Format(nameof (IterationMustBeOfUnpublishedTypeException), arg0);

    public static string IterationMustBeOfUnpublishedTypeException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (IterationMustBeOfUnpublishedTypeException), culture, arg0);

    public static string IterationCannotBeUnpublishedException(object arg0) => CodeReviewResources.Format(nameof (IterationCannotBeUnpublishedException), arg0);

    public static string IterationCannotBeUnpublishedException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (IterationCannotBeUnpublishedException), culture, arg0);

    public static string UnpublishedIterationAlreadyExistsException(object arg0) => CodeReviewResources.Format(nameof (UnpublishedIterationAlreadyExistsException), arg0);

    public static string UnpublishedIterationAlreadyExistsException(
      object arg0,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (UnpublishedIterationAlreadyExistsException), culture, arg0);
    }

    public static string IterationCannotBePublishedException(object arg0) => CodeReviewResources.Format(nameof (IterationCannotBePublishedException), arg0);

    public static string IterationCannotBePublishedException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (IterationCannotBePublishedException), culture, arg0);

    public static string BaseIterationRequiresIterations() => CodeReviewResources.Get(nameof (BaseIterationRequiresIterations));

    public static string BaseIterationRequiresIterations(CultureInfo culture) => CodeReviewResources.Get(nameof (BaseIterationRequiresIterations), culture);

    public static string ReviewCommentThreadWasMalformed() => CodeReviewResources.Get(nameof (ReviewCommentThreadWasMalformed));

    public static string ReviewCommentThreadWasMalformed(CultureInfo culture) => CodeReviewResources.Get(nameof (ReviewCommentThreadWasMalformed), culture);

    public static string CommentFilePathCannotBeNullWhenIsReviewLevelIsFalse() => CodeReviewResources.Get(nameof (CommentFilePathCannotBeNullWhenIsReviewLevelIsFalse));

    public static string CommentFilePathCannotBeNullWhenIsReviewLevelIsFalse(CultureInfo culture) => CodeReviewResources.Get(nameof (CommentFilePathCannotBeNullWhenIsReviewLevelIsFalse), culture);

    public static string CommentThreadContextCannotBeUpdated() => CodeReviewResources.Get(nameof (CommentThreadContextCannotBeUpdated));

    public static string CommentThreadContextCannotBeUpdated(CultureInfo culture) => CodeReviewResources.Get(nameof (CommentThreadContextCannotBeUpdated), culture);

    public static string CommentThreadPropertiesCannotBeUpdated() => CodeReviewResources.Get(nameof (CommentThreadPropertiesCannotBeUpdated));

    public static string CommentThreadPropertiesCannotBeUpdated(CultureInfo culture) => CodeReviewResources.Get(nameof (CommentThreadPropertiesCannotBeUpdated), culture);

    public static string CommentThreadNotFoundException(object arg0) => CodeReviewResources.Format(nameof (CommentThreadNotFoundException), arg0);

    public static string CommentThreadNotFoundException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (CommentThreadNotFoundException), culture, arg0);

    public static string CommentThreadCreateIdException(object arg0) => CodeReviewResources.Format(nameof (CommentThreadCreateIdException), arg0);

    public static string CommentThreadCreateIdException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (CommentThreadCreateIdException), culture, arg0);

    public static string CannotAddCommentWithoutContent() => CodeReviewResources.Get(nameof (CannotAddCommentWithoutContent));

    public static string CannotAddCommentWithoutContent(CultureInfo culture) => CodeReviewResources.Get(nameof (CannotAddCommentWithoutContent), culture);

    public static string InvalidCommentDiscussionId(object arg0) => CodeReviewResources.Format(nameof (InvalidCommentDiscussionId), arg0);

    public static string InvalidCommentDiscussionId(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (InvalidCommentDiscussionId), culture, arg0);

    public static string CommentAuthorCannotBeUpdated() => CodeReviewResources.Get(nameof (CommentAuthorCannotBeUpdated));

    public static string CommentAuthorCannotBeUpdated(CultureInfo culture) => CodeReviewResources.Get(nameof (CommentAuthorCannotBeUpdated), culture);

    public static string CommentWasMalformed(object arg0, object arg1) => CodeReviewResources.Format(nameof (CommentWasMalformed), arg0, arg1);

    public static string CommentWasMalformed(object arg0, object arg1, CultureInfo culture) => CodeReviewResources.Format(nameof (CommentWasMalformed), culture, arg0, arg1);

    public static string ReviewerStateOrKindMustBeSetForUpdate(object arg0) => CodeReviewResources.Format(nameof (ReviewerStateOrKindMustBeSetForUpdate), arg0);

    public static string ReviewerStateOrKindMustBeSetForUpdate(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (ReviewerStateOrKindMustBeSetForUpdate), culture, arg0);

    public static string DuplicateReviewersInReviewerList(object arg0) => CodeReviewResources.Format(nameof (DuplicateReviewersInReviewerList), arg0);

    public static string DuplicateReviewersInReviewerList(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (DuplicateReviewersInReviewerList), culture, arg0);

    public static string CodeReviewContentUploadNotAllowed(object arg0) => CodeReviewResources.Format(nameof (CodeReviewContentUploadNotAllowed), arg0);

    public static string CodeReviewContentUploadNotAllowed(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (CodeReviewContentUploadNotAllowed), culture, arg0);

    public static string ActionRejectedByPullRequest() => CodeReviewResources.Get(nameof (ActionRejectedByPullRequest));

    public static string ActionRejectedByPullRequest(CultureInfo culture) => CodeReviewResources.Get(nameof (ActionRejectedByPullRequest), culture);

    public static string MustAcceptZip() => CodeReviewResources.Get(nameof (MustAcceptZip));

    public static string MustAcceptZip(CultureInfo culture) => CodeReviewResources.Get(nameof (MustAcceptZip), culture);

    public static string ChangeEntriesWithBothSetAndUnSetChangeTrackingIdsException(object arg0) => CodeReviewResources.Format(nameof (ChangeEntriesWithBothSetAndUnSetChangeTrackingIdsException), arg0);

    public static string ChangeEntriesWithBothSetAndUnSetChangeTrackingIdsException(
      object arg0,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (ChangeEntriesWithBothSetAndUnSetChangeTrackingIdsException), culture, arg0);
    }

    public static string DuplicateContentHashInContentHashList(object arg0) => CodeReviewResources.Format(nameof (DuplicateContentHashInContentHashList), arg0);

    public static string DuplicateContentHashInContentHashList(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (DuplicateContentHashInContentHashList), culture, arg0);

    public static string FilesWithContentHashesNotFoundException() => CodeReviewResources.Get(nameof (FilesWithContentHashesNotFoundException));

    public static string FilesWithContentHashesNotFoundException(CultureInfo culture) => CodeReviewResources.Get(nameof (FilesWithContentHashesNotFoundException), culture);

    public static string EmptyDownloadContentCriteriaNotAllowed() => CodeReviewResources.Get(nameof (EmptyDownloadContentCriteriaNotAllowed));

    public static string EmptyDownloadContentCriteriaNotAllowed(CultureInfo culture) => CodeReviewResources.Get(nameof (EmptyDownloadContentCriteriaNotAllowed), culture);

    public static string ReviewerCannotBeAssociatedWithUnpublishedIterationException(object arg0) => CodeReviewResources.Format(nameof (ReviewerCannotBeAssociatedWithUnpublishedIterationException), arg0);

    public static string ReviewerCannotBeAssociatedWithUnpublishedIterationException(
      object arg0,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (ReviewerCannotBeAssociatedWithUnpublishedIterationException), culture, arg0);
    }

    public static string ReviewerCannotVoteWithoutPublishedIterationException(object arg0) => CodeReviewResources.Format(nameof (ReviewerCannotVoteWithoutPublishedIterationException), arg0);

    public static string ReviewerCannotVoteWithoutPublishedIterationException(
      object arg0,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (ReviewerCannotVoteWithoutPublishedIterationException), culture, arg0);
    }

    public static string AttachmentMalformed() => CodeReviewResources.Get(nameof (AttachmentMalformed));

    public static string AttachmentMalformed(CultureInfo culture) => CodeReviewResources.Get(nameof (AttachmentMalformed), culture);

    public static string AttachmentCannotBeCreatedBeforeFileUploadException(object arg0) => CodeReviewResources.Format(nameof (AttachmentCannotBeCreatedBeforeFileUploadException), arg0);

    public static string AttachmentCannotBeCreatedBeforeFileUploadException(
      object arg0,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (AttachmentCannotBeCreatedBeforeFileUploadException), culture, arg0);
    }

    public static string AttachmentNotFoundException(object arg0, object arg1) => CodeReviewResources.Format(nameof (AttachmentNotFoundException), arg0, arg1);

    public static string AttachmentNotFoundException(object arg0, object arg1, CultureInfo culture) => CodeReviewResources.Format(nameof (AttachmentNotFoundException), culture, arg0, arg1);

    public static string StatusNotFoundException(object arg0, object arg1, object arg2) => CodeReviewResources.Format(nameof (StatusNotFoundException), arg0, arg1, arg2);

    public static string StatusNotFoundException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (StatusNotFoundException), culture, arg0, arg1, arg2);
    }

    public static string TooManyStatusRecords(object arg0, object arg1, object arg2) => CodeReviewResources.Format(nameof (TooManyStatusRecords), arg0, arg1, arg2);

    public static string TooManyStatusRecords(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (TooManyStatusRecords), culture, arg0, arg1, arg2);
    }

    public static string StatusMalformed() => CodeReviewResources.Get(nameof (StatusMalformed));

    public static string StatusMalformed(CultureInfo culture) => CodeReviewResources.Get(nameof (StatusMalformed), culture);

    public static string StatusNameCannotBeUpdated() => CodeReviewResources.Get(nameof (StatusNameCannotBeUpdated));

    public static string StatusNameCannotBeUpdated(CultureInfo culture) => CodeReviewResources.Get(nameof (StatusNameCannotBeUpdated), culture);

    public static string StatusGenreCannotBeUpdated() => CodeReviewResources.Get(nameof (StatusGenreCannotBeUpdated));

    public static string StatusGenreCannotBeUpdated(CultureInfo culture) => CodeReviewResources.Get(nameof (StatusGenreCannotBeUpdated), culture);

    public static string InvalidPropertiesPatchOperationException() => CodeReviewResources.Get(nameof (InvalidPropertiesPatchOperationException));

    public static string InvalidPropertiesPatchOperationException(CultureInfo culture) => CodeReviewResources.Get(nameof (InvalidPropertiesPatchOperationException), culture);

    public static string PropertiesReplacePatchPathCannotBeEmpty() => CodeReviewResources.Get(nameof (PropertiesReplacePatchPathCannotBeEmpty));

    public static string PropertiesReplacePatchPathCannotBeEmpty(CultureInfo culture) => CodeReviewResources.Get(nameof (PropertiesReplacePatchPathCannotBeEmpty), culture);

    public static string PropertiesRemovePatchPathCannotBeEmpty() => CodeReviewResources.Get(nameof (PropertiesRemovePatchPathCannotBeEmpty));

    public static string PropertiesRemovePatchPathCannotBeEmpty(CultureInfo culture) => CodeReviewResources.Get(nameof (PropertiesRemovePatchPathCannotBeEmpty), culture);

    public static string ReviewSettingsMalformed() => CodeReviewResources.Get(nameof (ReviewSettingsMalformed));

    public static string ReviewSettingsMalformed(CultureInfo culture) => CodeReviewResources.Get(nameof (ReviewSettingsMalformed), culture);

    public static string ReviewContinuationTokenCouldNotBeParsed() => CodeReviewResources.Get(nameof (ReviewContinuationTokenCouldNotBeParsed));

    public static string ReviewContinuationTokenCouldNotBeParsed(CultureInfo culture) => CodeReviewResources.Get(nameof (ReviewContinuationTokenCouldNotBeParsed), culture);

    public static string IdentifierValidGuid() => CodeReviewResources.Get(nameof (IdentifierValidGuid));

    public static string IdentifierValidGuid(CultureInfo culture) => CodeReviewResources.Get(nameof (IdentifierValidGuid), culture);

    public static string DefaultNotificationToolCaption() => CodeReviewResources.Get(nameof (DefaultNotificationToolCaption));

    public static string DefaultNotificationToolCaption(CultureInfo culture) => CodeReviewResources.Get(nameof (DefaultNotificationToolCaption), culture);

    public static string CannotShareReviewExceededMaxReceivers(object arg0, object arg1) => CodeReviewResources.Format(nameof (CannotShareReviewExceededMaxReceivers), arg0, arg1);

    public static string CannotShareReviewExceededMaxReceivers(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (CannotShareReviewExceededMaxReceivers), culture, arg0, arg1);
    }

    public static string CannotShareReviewExceededMaxMessageLength(object arg0, object arg1) => CodeReviewResources.Format(nameof (CannotShareReviewExceededMaxMessageLength), arg0, arg1);

    public static string CannotShareReviewExceededMaxMessageLength(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (CannotShareReviewExceededMaxMessageLength), culture, arg0, arg1);
    }

    public static string CannotShareReviewExceededMaxSubjectLength(object arg0, object arg1) => CodeReviewResources.Format(nameof (CannotShareReviewExceededMaxSubjectLength), arg0, arg1);

    public static string CannotShareReviewExceededMaxSubjectLength(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (CannotShareReviewExceededMaxSubjectLength), culture, arg0, arg1);
    }

    public static string TooManyVisitsToQueryFor(object arg0, object arg1) => CodeReviewResources.Format(nameof (TooManyVisitsToQueryFor), arg0, arg1);

    public static string TooManyVisitsToQueryFor(object arg0, object arg1, CultureInfo culture) => CodeReviewResources.Format(nameof (TooManyVisitsToQueryFor), culture, arg0, arg1);

    public static string InvalidArtifactIdForVisitUpdate(object arg0) => CodeReviewResources.Format(nameof (InvalidArtifactIdForVisitUpdate), arg0);

    public static string InvalidArtifactIdForVisitUpdate(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (InvalidArtifactIdForVisitUpdate), culture, arg0);

    public static string CodeReviewInvalidStatusChangeException(object arg0) => CodeReviewResources.Format(nameof (CodeReviewInvalidStatusChangeException), arg0);

    public static string CodeReviewInvalidStatusChangeException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (CodeReviewInvalidStatusChangeException), culture, arg0);

    public static string AttachmentSizeTooLarge(object arg0) => CodeReviewResources.Format(nameof (AttachmentSizeTooLarge), arg0);

    public static string AttachmentSizeTooLarge(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (AttachmentSizeTooLarge), culture, arg0);

    public static string InvalidAttachmentType(object arg0) => CodeReviewResources.Format(nameof (InvalidAttachmentType), arg0);

    public static string InvalidAttachmentType(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (InvalidAttachmentType), culture, arg0);

    public static string MaxAttachmentCountException(object arg0) => CodeReviewResources.Format(nameof (MaxAttachmentCountException), arg0);

    public static string MaxAttachmentCountException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (MaxAttachmentCountException), culture, arg0);

    public static string MaxCommentCountException(object arg0) => CodeReviewResources.Format(nameof (MaxCommentCountException), arg0);

    public static string MaxCommentCountException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (MaxCommentCountException), culture, arg0);

    public static string AttachmentAlreadyExists(object arg0) => CodeReviewResources.Format(nameof (AttachmentAlreadyExists), arg0);

    public static string AttachmentAlreadyExists(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (AttachmentAlreadyExists), culture, arg0);

    public static string AttachmentDeleteInvalidAuthor() => CodeReviewResources.Get(nameof (AttachmentDeleteInvalidAuthor));

    public static string AttachmentDeleteInvalidAuthor(CultureInfo culture) => CodeReviewResources.Get(nameof (AttachmentDeleteInvalidAuthor), culture);

    public static string CommentDeleteInvalidAuthor() => CodeReviewResources.Get(nameof (CommentDeleteInvalidAuthor));

    public static string CommentDeleteInvalidAuthor(CultureInfo culture) => CodeReviewResources.Get(nameof (CommentDeleteInvalidAuthor), culture);

    public static string ReviewSettingsSaveInsufficientPermissions() => CodeReviewResources.Get(nameof (ReviewSettingsSaveInsufficientPermissions));

    public static string ReviewSettingsSaveInsufficientPermissions(CultureInfo culture) => CodeReviewResources.Get(nameof (ReviewSettingsSaveInsufficientPermissions), culture);

    public static string ReviewCriteriaMustBeSpecified() => CodeReviewResources.Get(nameof (ReviewCriteriaMustBeSpecified));

    public static string ReviewCriteriaMustBeSpecified(CultureInfo culture) => CodeReviewResources.Get(nameof (ReviewCriteriaMustBeSpecified), culture);

    public static string AuthorOrReviewerMustBeSpecifiedWhileSearchingInAccount() => CodeReviewResources.Get(nameof (AuthorOrReviewerMustBeSpecifiedWhileSearchingInAccount));

    public static string AuthorOrReviewerMustBeSpecifiedWhileSearchingInAccount(CultureInfo culture) => CodeReviewResources.Get(nameof (AuthorOrReviewerMustBeSpecifiedWhileSearchingInAccount), culture);

    public static string ReviewStatusMustBeSpecified() => CodeReviewResources.Get(nameof (ReviewStatusMustBeSpecified));

    public static string ReviewStatusMustBeSpecified(CultureInfo culture) => CodeReviewResources.Get(nameof (ReviewStatusMustBeSpecified), culture);

    public static string ReviewCriteriaCannotHaveBothCreateAndUpdateTimestamps() => CodeReviewResources.Get(nameof (ReviewCriteriaCannotHaveBothCreateAndUpdateTimestamps));

    public static string ReviewCriteriaCannotHaveBothCreateAndUpdateTimestamps(CultureInfo culture) => CodeReviewResources.Get(nameof (ReviewCriteriaCannotHaveBothCreateAndUpdateTimestamps), culture);

    public static string CodeReviewArgumentEmptyException(object arg0) => CodeReviewResources.Format(nameof (CodeReviewArgumentEmptyException), arg0);

    public static string CodeReviewArgumentEmptyException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (CodeReviewArgumentEmptyException), culture, arg0);

    public static string StatusIterationIdNotConsistent() => CodeReviewResources.Get(nameof (StatusIterationIdNotConsistent));

    public static string StatusIterationIdNotConsistent(CultureInfo culture) => CodeReviewResources.Get(nameof (StatusIterationIdNotConsistent), culture);

    public static string StatusesDeleteNotSupported() => CodeReviewResources.Get(nameof (StatusesDeleteNotSupported));

    public static string StatusesDeleteNotSupported(CultureInfo culture) => CodeReviewResources.Get(nameof (StatusesDeleteNotSupported), culture);

    public static string ExceededMaxAllowedFileSize(object arg0, object arg1) => CodeReviewResources.Format(nameof (ExceededMaxAllowedFileSize), arg0, arg1);

    public static string ExceededMaxAllowedFileSize(object arg0, object arg1, CultureInfo culture) => CodeReviewResources.Format(nameof (ExceededMaxAllowedFileSize), culture, arg0, arg1);

    public static string CannontFindContentHashAssociatedWithReview(object arg0, object arg1) => CodeReviewResources.Format(nameof (CannontFindContentHashAssociatedWithReview), arg0, arg1);

    public static string CannontFindContentHashAssociatedWithReview(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CodeReviewResources.Format(nameof (CannontFindContentHashAssociatedWithReview), culture, arg0, arg1);
    }

    public static string BatchContentInvalid() => CodeReviewResources.Get(nameof (BatchContentInvalid));

    public static string BatchContentInvalid(CultureInfo culture) => CodeReviewResources.Get(nameof (BatchContentInvalid), culture);

    public static string FileWithSameContentHashInPackageException(object arg0) => CodeReviewResources.Format(nameof (FileWithSameContentHashInPackageException), arg0);

    public static string FileWithSameContentHashInPackageException(object arg0, CultureInfo culture) => CodeReviewResources.Format(nameof (FileWithSameContentHashInPackageException), culture, arg0);

    public static string CodeReviewNoFilesFoundToUploadInPackageException() => CodeReviewResources.Get(nameof (CodeReviewNoFilesFoundToUploadInPackageException));

    public static string CodeReviewNoFilesFoundToUploadInPackageException(CultureInfo culture) => CodeReviewResources.Get(nameof (CodeReviewNoFilesFoundToUploadInPackageException), culture);

    public static string InvalidArtifactIdString() => CodeReviewResources.Get(nameof (InvalidArtifactIdString));

    public static string InvalidArtifactIdString(CultureInfo culture) => CodeReviewResources.Get(nameof (InvalidArtifactIdString), culture);

    public static string ViewedStatusNotSupported() => CodeReviewResources.Get(nameof (ViewedStatusNotSupported));

    public static string ViewedStatusNotSupported(CultureInfo culture) => CodeReviewResources.Get(nameof (ViewedStatusNotSupported), culture);

    public static string ViewedStatusCountLimitReached() => CodeReviewResources.Get(nameof (ViewedStatusCountLimitReached));

    public static string ViewedStatusCountLimitReached(CultureInfo culture) => CodeReviewResources.Get(nameof (ViewedStatusCountLimitReached), culture);
  }
}
