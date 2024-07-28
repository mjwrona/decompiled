// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.Resources
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.TeamFoundation.SourceControl.WebServer.Resources).GetTypeInfo().Assembly);
    public const string AmbigousVersionType = "AmbigousVersionType";
    public const string AmbiguousVersion = "AmbiguousVersion";
    public const string AmbiguousVersionOption = "AmbiguousVersionOption";
    public const string BranchesNoCommonCommits = "BranchesNoCommonCommits";
    public const string ChangesetListInvalidEndDateFormat = "ChangesetListInvalidEndDateFormat";
    public const string ChangesetListInvalidStartDateFormat = "ChangesetListInvalidStartDateFormat";
    public const string CommitDescriptionFormat = "CommitDescriptionFormat";
    public const string DestroyedFileContentUnavailableException = "DestroyedFileContentUnavailableException";
    public const string ErrorBranchNotFound = "ErrorBranchNotFound";
    public const string ErrorCommitNotFound = "ErrorCommitNotFound";
    public const string ErrorInvalidFileType = "ErrorInvalidFileType";
    public const string ErrorInvalidGitVersionSpec = "ErrorInvalidGitVersionSpec";
    public const string ErrorItemBatchNotFound = "ErrorItemBatchNotFound";
    public const string ErrorItemNotFoundFormat = "ErrorItemNotFoundFormat";
    public const string ErrorNoBranchesFormat = "ErrorNoBranchesFormat";
    public const string ErrorPathEmpty = "ErrorPathEmpty";
    public const string ErrorPathsNotSpecified = "ErrorPathsNotSpecified";
    public const string ErrorRepositoryWithNameIsNotOfExpectedType = "ErrorRepositoryWithNameIsNotOfExpectedType";
    public const string ErrorSubModulesFileNotFoundFormat = "ErrorSubModulesFileNotFoundFormat";
    public const string ErrorTagNotFound = "ErrorTagNotFound";
    public const string InvalidArgumentValue = "InvalidArgumentValue";
    public const string InvalidChangesetVersion = "InvalidChangesetVersion";
    public const string InvalidChangeVersion = "InvalidChangeVersion";
    public const string InvalidLatestVersion = "InvalidLatestVersion";
    public const string InvalidMergeSourceError = "InvalidMergeSourceError";
    public const string InvalidShelvesetVersion = "InvalidShelvesetVersion";
    public const string InvalidTipVersion = "InvalidTipVersion";
    public const string InvalidUseRenameError = "InvalidUseRenameError";
    public const string InvalidVersionSpecForChangeModel = "InvalidVersionSpecForChangeModel";
    public const string ItemPath = "ItemPath";
    public const string Pending = "Pending";
    public const string SccItemDeleted = "SccItemDeleted";
    public const string SccItemLatest = "SccItemLatest";
    public const string UnsupportedOrderingOfItems = "UnsupportedOrderingOfItems";
    public const string AllChangesetChanges = "AllChangesetChanges";
    public const string LatestChangeset = "LatestChangeset";
    public const string ChangesetIdNotSpecified = "ChangesetIdNotSpecified";
    public const string InvalidOwnerId = "InvalidOwnerId";
    public const string InvalidQueryParam = "InvalidQueryParam";
    public const string ErrorPreviousChangeRecursive = "ErrorPreviousChangeRecursive";
    public const string CannotGetTreeAsText = "CannotGetTreeAsText";
    public const string UnresolvableSha = "UnresolvableSha";
    public const string UnsupportedMediaType = "UnsupportedMediaType";
    public const string ZipFileExtension = "ZipFileExtension";
    public const string ErrorInvalidFromDateFormat = "ErrorInvalidFromDateFormat";
    public const string ErrorInvalidToDateFormat = "ErrorInvalidToDateFormat";
    public const string ErrorItemPathWithRecursion = "ErrorItemPathWithRecursion";
    public const string ErrorItemAndScopePaths = "ErrorItemAndScopePaths";
    public const string ChangesetsAscendingOrderById = "ChangesetsAscendingOrderById";
    public const string ErrorChangesetBatchNotFound = "ErrorChangesetBatchNotFound";
    public const string LinkHeaderForNextPage = "LinkHeaderForNextPage";
    public const string ErrorCannotSpecifyScopeAndHierarchy = "ErrorCannotSpecifyScopeAndHierarchy";
    public const string ErrorTfvcBranchNotFound = "ErrorTfvcBranchNotFound";
    public const string InvalidPullRequestId = "InvalidPullRequestId";
    public const string InvalidPullRequestToUpdateInformation = "InvalidPullRequestToUpdateInformation";
    public const string InvalidPullRequestUpdate = "InvalidPullRequestUpdate";
    public const string InvalidPullRequestMergeStatusUpdate = "InvalidPullRequestMergeStatusUpdate";
    public const string InvalidIdentity = "InvalidIdentity";
    public const string NoGuidSupplied = "NoGuidSupplied";
    public const string NotFound = "NotFound";
    public const string InvalidConflictPatchProperties = "InvalidConflictPatchProperties";
    public const string MissingPullRequestRef = "MissingPullRequestRef";
    public const string MismatchRepositoryId = "MismatchRepositoryId";
    public const string LastMergeSourceCommitRequired = "LastMergeSourceCommitRequired";
    public const string PullRequestDescriptionTooLong = "PullRequestDescriptionTooLong";
    public const string InvalidParameters = "InvalidParameters";
    public const string MultiPathUpdateNotSupported = "MultiPathUpdateNotSupported";
    public const string RequestMaxSizeExceeded = "RequestMaxSizeExceeded";
    public const string InvalidJsonBody = "InvalidJsonBody";
    public const string InvalidReviewer = "InvalidReviewer";
    public const string InvalidReviewerId = "InvalidReviewerId";
    public const string OnlyPatchVoteValue = "OnlyPatchVoteValue";
    public const string MissingVote = "MissingVote";
    public const string MissingReviewerFlagOrDecline = "MissingReviewerFlagOrDecline";
    public const string TempWorkspaceComment = "TempWorkspaceComment";
    public const string InvalidPathActionsArgument = "InvalidPathActionsArgument";
    public const string APINotFound = "APINotFound";
    public const string ErrorUpdatingRefs = "ErrorUpdatingRefs";
    public const string OldAndNewObjectIdsCannotBeEmpty = "OldAndNewObjectIdsCannotBeEmpty";
    public const string RepositoryTypeNotSuppported = "RepositoryTypeNotSuppported";
    public const string GitRefUpdateModeNotSuppported = "GitRefUpdateModeNotSuppported";
    public const string GitRefUpdateStatusNotSuppported = "GitRefUpdateStatusNotSuppported";
    public const string RepositoryNotFoundOrNoReadPermission = "RepositoryNotFoundOrNoReadPermission";
    public const string UnsupportedRepoChangeType = "UnsupportedRepoChangeType";
    public const string TeamProjectRequired = "TeamProjectRequired";
    public const string CollectionRequired = "CollectionRequired";
    public const string RepositoryNameRequired = "RepositoryNameRequired";
    public const string MalformedGitRepositoryData = "MalformedGitRepositoryData";
    public const string TfvcMovePathActionNotSupported = "TfvcMovePathActionNotSupported";
    public const string PathActionInvalidEncodingValue = "PathActionInvalidEncodingValue";
    public const string PathActionInvalidBinaryEncoding = "PathActionInvalidBinaryEncoding";
    public const string PathActionInvalidUnchangedEncoding = "PathActionInvalidUnchangedEncoding";
    public const string CannotCreateWithVotes = "CannotCreateWithVotes";
    public const string CannotVoteForAnother = "CannotVoteForAnother";
    public const string InvalidVote = "InvalidVote";
    public const string MustAcceptZip = "MustAcceptZip";
    public const string ProjectCatalogNodeError = "ProjectCatalogNodeError";
    public const string InvalidPushCommitRefCount = "InvalidPushCommitRefCount";
    public const string MismatchedProjectId = "MismatchedProjectId";
    public const string RequestProjectScopeConflict = "RequestProjectScopeConflict";
    public const string RequestProjectMismatch = "RequestProjectMismatch";
    public const string InvalidPullRequestDescription = "InvalidPullRequestDescription";
    public const string InvalidPullRequestTitle = "InvalidPullRequestTitle";
    public const string InvalidPullRequestQueryType = "InvalidPullRequestQueryType";
    public const string InvalidPullRequestQueryTooManyItems = "InvalidPullRequestQueryTooManyItems";
    public const string InvalidPullRequestQueryTooManyQueries = "InvalidPullRequestQueryTooManyQueries";
    public const string AttachmentGzipUnsupportedWithoutLength = "AttachmentGzipUnsupportedWithoutLength";
    public const string LimitedRefCriteriaIsNull = "LimitedRefCriteriaIsNull";
    public const string CannotEditFolder = "CannotEditFolder";
    public const string ItemExistsException = "ItemExistsException";
    public const string NonEmptyFoldersNotSupported = "NonEmptyFoldersNotSupported";
    public const string TeamProjectDoesNotExist = "TeamProjectDoesNotExist";
    public const string GitRefFavoriteIsNull = "GitRefFavoriteIsNull";
    public const string GitRefFavoriteInvalidType = "GitRefFavoriteInvalidType";
    public const string GitInvalidRefName = "GitInvalidRefName";
    public const string GitRefFavoriteProjectMismatch = "GitRefFavoriteProjectMismatch";
    public const string GitRefFavoritesRepositoryIsNull = "GitRefFavoritesRepositoryIsNull";
    public const string GitRefsFilterAndMyBranches = "GitRefsFilterAndMyBranches";
    public const string PullRequestStatusMalformed = "PullRequestStatusMalformed";
    public const string ErrorChangesetsNotSpecified = "ErrorChangesetsNotSpecified";
    public const string MissingCherryPickRef = "MissingCherryPickRef";
    public const string ChangesetToBeCreatedNotSpecified = "ChangesetToBeCreatedNotSpecified";
    public const string IllegalChangeOnItemThatDoesNotExist = "IllegalChangeOnItemThatDoesNotExist";
    public const string IllegalSourceServerItem = "IllegalSourceServerItem";
    public const string UnsupportedChangeType = "UnsupportedChangeType";
    public const string MustSetVoteToZero = "MustSetVoteToZero";
    public const string ErrorBranchAndCommit = "ErrorBranchAndCommit";
    public const string CherryPickNeedsSingleSource = "CherryPickNeedsSingleSource";
    public const string MissingRevertRef = "MissingRevertRef";
    public const string RevertNeedsSingleSource = "RevertNeedsSingleSource";
    public const string InvalidImportRequestUpdate = "InvalidImportRequestUpdate";
    public const string ShelvesetToBeCreatedNotSpecified = "ShelvesetToBeCreatedNotSpecified";
    public const string CrossCollectionMergeBaseUnsupported = "CrossCollectionMergeBaseUnsupported";
    public const string ErrorGitTemplateNotFound = "ErrorGitTemplateNotFound";
    public const string UnsupportedTemplateChangeType = "UnsupportedTemplateChangeType";
    public const string InvalidGitChangeContentType = "InvalidGitChangeContentType";
    public const string GitNoParentCommit = "GitNoParentCommit";
    public const string TreeDiffBaseTargetNotSpecified = "TreeDiffBaseTargetNotSpecified";
    public const string UnresolvableToTree = "UnresolvableToTree";
    public const string MaxFilePathsLimitExceeded = "MaxFilePathsLimitExceeded";
    public const string ContinuationTokenForBlobScopeNotSupported = "ContinuationTokenForBlobScopeNotSupported";
    public const string InvalidContinuationToken = "InvalidContinuationToken";
    public const string InvalidContinuationTokenWithReason = "InvalidContinuationTokenWithReason";
    public const string ScopePathRequiredForPagingItems = "ScopePathRequiredForPagingItems";
    public const string ErrorRespositoryIdInvalid = "ErrorRespositoryIdInvalid";
    public const string InvalidPullRequestAutoCompleteSetById = "InvalidPullRequestAutoCompleteSetById";
    public const string InvalidRefPatchProperties = "InvalidRefPatchProperties";
    public const string CanOnlyResolveLfsToOctetStream = "CanOnlyResolveLfsToOctetStream";
    public const string BypassOptionNotAllowedWithAutoComplete = "BypassOptionNotAllowedWithAutoComplete";
    public const string GitImportTFVCImportSizeTooLarge = "GitImportTFVCImportSizeTooLarge";
    public const string GitImportTFVCImportHistorySizeTooLarge = "GitImportTFVCImportHistorySizeTooLarge";
    public const string GitImportTFVCFileTooLarge = "GitImportTFVCFileTooLarge";
    public const string GitImportTFVCFileTooLargeHistory = "GitImportTFVCFileTooLargeHistory";
    public const string GitImportTFVCRootNotAllowed = "GitImportTFVCRootNotAllowed";
    public const string UnexpectedFileIdForFileException = "UnexpectedFileIdForFileException";
    public const string CheckinFailedError = "CheckinFailedError";
    public const string PullRequestCompletionOptionsTooLong = "PullRequestCompletionOptionsTooLong";
    public const string MergeNotSupported = "MergeNotSupported";
    public const string PullRequestTargetEqualSource = "PullRequestTargetEqualSource";
    public const string MissingOrEmptyLabelName = "MissingOrEmptyLabelName";
    public const string PullRequestStatusesPatchRemoveArbitraryPath = "PullRequestStatusesPatchRemoveArbitraryPath";
    public const string ErrorDeletionNotSupportedOnPatch = "ErrorDeletionNotSupportedOnPatch";
    public const string BaseChangesetHigherThanTargetChangesetException = "BaseChangesetHigherThanTargetChangesetException";
    public const string BaseChangesetRequired = "BaseChangesetRequired";
    public const string TopOutOfRangeException = "TopOutOfRangeException";
    public const string ChangesetRangeTooLarge = "ChangesetRangeTooLarge";
    public const string InvalidRepositoryId = "InvalidRepositoryId";
    public const string InvalidPolicyType = "InvalidPolicyType";
    public const string GitRefsPaginateMyBranchesIsNotSupported = "GitRefsPaginateMyBranchesIsNotSupported";
    public const string DisplayNameForInvalidIdentity = "DisplayNameForInvalidIdentity";
    public const string VCErrorCommitNotFound = "VCErrorCommitNotFound";
    public const string ErrorPushNotFound = "ErrorPushNotFound";
    public const string CreateProjectFolderComment = "CreateProjectFolderComment";
    public const string FileDiffsInvalidFileParamsInput = "FileDiffsInvalidFileParamsInput";
    public const string FileDiffsCriteriaArgumentException = "FileDiffsCriteriaArgumentException";
    public const string FileDiffsMissingPath = "FileDiffsMissingPath";
    public const string InvalidObjectId = "InvalidObjectId";
    public const string UnsupportedNumberOfCommitsForMerge = "UnsupportedNumberOfCommitsForMerge";
    public const string InvalidSvgLargeFile = "InvalidSvgLargeFile";
    public const string InvalidSvgNullDocument = "InvalidSvgNullDocument";
    public const string InvalidSvgParseErros = "InvalidSvgParseErros";
    public const string SvgSanitizeError = "SvgSanitizeError";
    public const string SvgSanitizeLoadError = "SvgSanitizeLoadError";
    public const string ImportHistoryDurationTooLarge = "ImportHistoryDurationTooLarge";
    public const string WebEditDisabled = "WebEditDisabled";
    public const string WebEditEnabledSettingPath = "WebEditEnabledSettingPath";
    public const string ErrorDescriptorNull = "ErrorDescriptorNull";
    public const string InvalidGitStatusStateValue = "InvalidGitStatusStateValue";

    public static ResourceManager Manager => Microsoft.TeamFoundation.SourceControl.WebServer.Resources.s_resMgr;

    public static string Get(string resourceName) => Microsoft.TeamFoundation.SourceControl.WebServer.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    public static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.SourceControl.WebServer.Resources.Get(resourceName) : Microsoft.TeamFoundation.SourceControl.WebServer.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.TeamFoundation.SourceControl.WebServer.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.SourceControl.WebServer.Resources.GetInt(resourceName) : (int) Microsoft.TeamFoundation.SourceControl.WebServer.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.TeamFoundation.SourceControl.WebServer.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.SourceControl.WebServer.Resources.GetBool(resourceName) : (bool) Microsoft.TeamFoundation.SourceControl.WebServer.Resources.s_resMgr.GetObject(resourceName, culture);

    public static string Format(string resourceName, params object[] args) => Microsoft.TeamFoundation.SourceControl.WebServer.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    public static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.TeamFoundation.SourceControl.WebServer.Resources.Get(resourceName, culture);
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
  }
}
