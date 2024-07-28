// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(resourceName, culture);
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

    public static string Error_ArgumentRequired(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_ArgumentRequired), arg0);

    public static string Error_ArgumentRequired(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_ArgumentRequired), culture, arg0);

    public static string Error_CannotAddViewFeedViewDoesntExist(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CannotAddViewFeedViewDoesntExist), arg0);

    public static string Error_CannotAddViewFeedViewDoesntExist(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CannotAddViewFeedViewDoesntExist), culture, arg0);

    public static string Error_CannotCreateUnknownTypeCommitLogEntry() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_CannotCreateUnknownTypeCommitLogEntry));

    public static string Error_CannotCreateUnknownTypeCommitLogEntry(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_CannotCreateUnknownTypeCommitLogEntry), culture);

    public static string Error_CannotPerformOperationOnReadOnlyFeed(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CannotPerformOperationOnReadOnlyFeed), arg0);

    public static string Error_CannotPerformOperationOnReadOnlyFeed(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CannotPerformOperationOnReadOnlyFeed), culture, arg0);
    }

    public static string Error_CantPromoteToImplicitView() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_CantPromoteToImplicitView));

    public static string Error_CantPromoteToImplicitView(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_CantPromoteToImplicitView), culture);

    public static string Error_CiDataExpectingSpecificHostType(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CiDataExpectingSpecificHostType), arg0);

    public static string Error_CiDataExpectingSpecificHostType(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CiDataExpectingSpecificHostType), culture, arg0);

    public static string Error_CommitIdMustBeString() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_CommitIdMustBeString));

    public static string Error_CommitIdMustBeString(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_CommitIdMustBeString), culture);

    public static string Error_CommitLogTimestampMustBeUtc() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_CommitLogTimestampMustBeUtc));

    public static string Error_CommitLogTimestampMustBeUtc(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_CommitLogTimestampMustBeUtc), culture);

    public static string Error_DeletedPackageJobDisabled() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_DeletedPackageJobDisabled));

    public static string Error_DeletedPackageJobDisabled(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_DeletedPackageJobDisabled), culture);

    public static string Error_PackageTooSmall() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_PackageTooSmall));

    public static string Error_PackageTooSmall(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_PackageTooSmall), culture);

    public static string Error_FailedToAddReferenceToPackageContent(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_FailedToAddReferenceToPackageContent), arg0, arg1);

    public static string Error_FailedToAddReferenceToPackageContent(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_FailedToAddReferenceToPackageContent), culture, arg0, arg1);
    }

    public static string Error_FailedToAddReferencesToPackageContent(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_FailedToAddReferencesToPackageContent), arg0);

    public static string Error_FailedToAddReferencesToPackageContent(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_FailedToAddReferencesToPackageContent), culture, arg0);
    }

    public static string Error_FeedAlreadyContainsPackageDeleted(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_FeedAlreadyContainsPackageDeleted), arg0, arg1);

    public static string Error_FeedAlreadyContainsPackageDeleted(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_FeedAlreadyContainsPackageDeleted), culture, arg0, arg1);
    }

    public static string Error_InvalidVersion(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_InvalidVersion), arg0);

    public static string Error_InvalidVersion(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_InvalidVersion), culture, arg0);

    public static string Error_VersionIntegerTooLarge() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_VersionIntegerTooLarge));

    public static string Error_VersionIntegerTooLarge(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_VersionIntegerTooLarge), culture);

    public static string Error_InvalidMultipartRequest() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InvalidMultipartRequest));

    public static string Error_InvalidMultipartRequest(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InvalidMultipartRequest), culture);

    public static string Error_InvalidPackageCorruptZip() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InvalidPackageCorruptZip));

    public static string Error_InvalidPackageCorruptZip(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InvalidPackageCorruptZip), culture);

    public static string Error_InvalidZipArchive() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InvalidZipArchive));

    public static string Error_InvalidZipArchive(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InvalidZipArchive), culture);

    public static string Error_InvalidZipCompressionMethod() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InvalidZipCompressionMethod));

    public static string Error_InvalidZipCompressionMethod(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InvalidZipCompressionMethod), culture);

    public static string Error_MissingCommitLogEntry(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_MissingCommitLogEntry), arg0, arg1, arg2);

    public static string Error_MissingCommitLogEntry(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_MissingCommitLogEntry), culture, arg0, arg1, arg2);
    }

    public static string Error_NotSupportedViewsPath(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NotSupportedViewsPath), arg0);

    public static string Error_NotSupportedViewsPath(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NotSupportedViewsPath), culture, arg0);

    public static string Error_NoDirectDownloadForDropBackedPackage() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_NoDirectDownloadForDropBackedPackage));

    public static string Error_NoDirectDownloadForDropBackedPackage(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_NoDirectDownloadForDropBackedPackage), culture);

    public static string Error_PackageVersionExceedsMaximumLength(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageVersionExceedsMaximumLength), arg0);

    public static string Error_PackageVersionExceedsMaximumLength(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageVersionExceedsMaximumLength), culture, arg0);

    public static string Error_PackageTooManyVersions(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageTooManyVersions), arg0, arg1);

    public static string Error_PackageTooManyVersions(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageTooManyVersions), culture, arg0, arg1);
    }

    public static string Error_PackagingServiceReadOnly() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_PackagingServiceReadOnly));

    public static string Error_PackagingServiceReadOnly(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_PackagingServiceReadOnly), culture);

    public static string Error_UnexpectedCommitLogOperation() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_UnexpectedCommitLogOperation));

    public static string Error_UnexpectedCommitLogOperation(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_UnexpectedCommitLogOperation), culture);

    public static string Error_UnexpectedMediaType(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UnexpectedMediaType), arg0);

    public static string Error_UnexpectedMediaType(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UnexpectedMediaType), culture, arg0);

    public static string Error_ExpiredUrl(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_ExpiredUrl), arg0, arg1, arg2);

    public static string Error_ExpiredUrl(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_ExpiredUrl), culture, arg0, arg1, arg2);
    }

    public static string Error_ValueCannotBeNull(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_ValueCannotBeNull), arg0);

    public static string Error_ValueCannotBeNull(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_ValueCannotBeNull), culture, arg0);

    public static string Error_ViewsOperationValueMustBeAString() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ViewsOperationValueMustBeAString));

    public static string Error_ViewsOperationValueMustBeAString(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ViewsOperationValueMustBeAString), culture);

    public static string Error_ViewsPatchOperationNotSupported(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_ViewsPatchOperationNotSupported), arg0);

    public static string Error_ViewsPatchOperationNotSupported(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_ViewsPatchOperationNotSupported), culture, arg0);

    public static string Error_FileLimitExceeded(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_FileLimitExceeded), arg0);

    public static string Error_FileLimitExceeded(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_FileLimitExceeded), culture, arg0);

    public static string Error_UpstreamDisabled(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UpstreamDisabled), arg0);

    public static string Error_UpstreamDisabled(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UpstreamDisabled), culture, arg0);

    public static string Warning_UpstreamContentDeletionDisabled() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Warning_UpstreamContentDeletionDisabled));

    public static string Warning_UpstreamContentDeletionDisabled(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Warning_UpstreamContentDeletionDisabled), culture);

    public static string Error_JobWritesBlocked() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_JobWritesBlocked));

    public static string Error_JobWritesBlocked(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_JobWritesBlocked), culture);

    public static string Error_ReadOnlyRequired() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ReadOnlyRequired));

    public static string Error_ReadOnlyRequired(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ReadOnlyRequired), culture);

    public static string Error_NoMoreRetries() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_NoMoreRetries));

    public static string Error_NoMoreRetries(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_NoMoreRetries), culture);

    public static string Error_NoMatchingHandlerForType(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NoMatchingHandlerForType), arg0);

    public static string Error_NoMatchingHandlerForType(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NoMatchingHandlerForType), culture, arg0);

    public static string Error_HandlerForTypeWouldBeHidden(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_HandlerForTypeWouldBeHidden), arg0, arg1);

    public static string Error_HandlerForTypeWouldBeHidden(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_HandlerForTypeWouldBeHidden), culture, arg0, arg1);
    }

    public static string Error_InvalidTracingWrapperUsage() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InvalidTracingWrapperUsage));

    public static string Error_InvalidTracingWrapperUsage(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InvalidTracingWrapperUsage), culture);

    public static string Error_UnknownOperationType(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UnknownOperationType), arg0);

    public static string Error_UnknownOperationType(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UnknownOperationType), culture, arg0);

    public static string Error_ExpectedEndOfArrayViews() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ExpectedEndOfArrayViews));

    public static string Error_ExpectedEndOfArrayViews(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ExpectedEndOfArrayViews), culture);

    public static string Error_ExpectedLastTokenEndOfObject() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ExpectedLastTokenEndOfObject));

    public static string Error_ExpectedLastTokenEndOfObject(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ExpectedLastTokenEndOfObject), culture);

    public static string Error_ExpectedStartOfArray() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ExpectedStartOfArray));

    public static string Error_ExpectedStartOfArray(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ExpectedStartOfArray), culture);

    public static string Error_ExpectedStartOfArrayViews() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ExpectedStartOfArrayViews));

    public static string Error_ExpectedStartOfArrayViews(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ExpectedStartOfArrayViews), culture);

    public static string Error_ExpectedStartOfObject() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ExpectedStartOfObject));

    public static string Error_ExpectedStartOfObject(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ExpectedStartOfObject), culture);

    public static string Error_FailedToApplyMigrationStateChange() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_FailedToApplyMigrationStateChange));

    public static string Error_FailedToApplyMigrationStateChange(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_FailedToApplyMigrationStateChange), culture);

    public static string Error_FailedToMigrateDataForFeed(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_FailedToMigrateDataForFeed), arg0, arg1, arg2);

    public static string Error_FailedToMigrateDataForFeed(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_FailedToMigrateDataForFeed), culture, arg0, arg1, arg2);
    }

    public static string Error_FailedToCompleteChangeProcessing(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_FailedToCompleteChangeProcessing), arg0, arg1, arg2);
    }

    public static string Error_FailedToCompleteChangeProcessing(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_FailedToCompleteChangeProcessing), culture, arg0, arg1, arg2);
    }

    public static string Error_UnexpectedMigrationFailure(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UnexpectedMigrationFailure), arg0);

    public static string Error_UnexpectedMigrationFailure(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UnexpectedMigrationFailure), culture, arg0);

    public static string Error_UnexpectedMigrationState(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UnexpectedMigrationState), arg0);

    public static string Error_UnexpectedMigrationState(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UnexpectedMigrationState), culture, arg0);

    public static string UpgradeAggregationsNotSupportedOnHosted() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (UpgradeAggregationsNotSupportedOnHosted));

    public static string UpgradeAggregationsNotSupportedOnHosted(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (UpgradeAggregationsNotSupportedOnHosted), culture);

    public static string DataImportUnsupportedOnPrem() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (DataImportUnsupportedOnPrem));

    public static string DataImportUnsupportedOnPrem(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (DataImportUnsupportedOnPrem), culture);

    public static string Error_InputNotAPropertyExpression() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InputNotAPropertyExpression));

    public static string Error_InputNotAPropertyExpression(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InputNotAPropertyExpression), culture);

    public static string Error_InvalidCommitEntryType() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InvalidCommitEntryType));

    public static string Error_InvalidCommitEntryType(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InvalidCommitEntryType), culture);

    public static string Error_InvalidViewSuboperation() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InvalidViewSuboperation));

    public static string Error_InvalidViewSuboperation(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InvalidViewSuboperation), culture);

    public static string Error_MultipleHandlersMatching() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_MultipleHandlersMatching));

    public static string Error_MultipleHandlersMatching(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_MultipleHandlersMatching), culture);

    public static string Error_MustCallInOrder(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_MustCallInOrder), arg0, arg1);

    public static string Error_MustCallInOrder(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_MustCallInOrder), culture, arg0, arg1);

    public static string Error_NoContentIdInWrappedItem() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_NoContentIdInWrappedItem));

    public static string Error_NoContentIdInWrappedItem(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_NoContentIdInWrappedItem), culture);

    public static string Error_PropertiesExpectedInsideMetadata() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_PropertiesExpectedInsideMetadata));

    public static string Error_PropertiesExpectedInsideMetadata(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_PropertiesExpectedInsideMetadata), culture);

    public static string Error_PropertyNotFound(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PropertyNotFound), arg0);

    public static string Error_PropertyNotFound(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PropertyNotFound), culture, arg0);

    public static string Error_UnknownStorageType() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_UnknownStorageType));

    public static string Error_UnknownStorageType(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_UnknownStorageType), culture);

    public static string Error_ValueNotFoundForProperty(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_ValueNotFoundForProperty), arg0);

    public static string Error_ValueNotFoundForProperty(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_ValueNotFoundForProperty), culture, arg0);

    public static string Error_AggregationNotFoundInMigration(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_AggregationNotFoundInMigration), arg0, arg1);

    public static string Error_AggregationNotFoundInMigration(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_AggregationNotFoundInMigration), culture, arg0, arg1);
    }

    public static string Error_CannotDeleteDropItem(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CannotDeleteDropItem), arg0);

    public static string Error_CannotDeleteDropItem(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CannotDeleteDropItem), culture, arg0);

    public static string Error_CollectionIDMismatch(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CollectionIDMismatch), arg0, arg1);

    public static string Error_CollectionIDMismatch(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CollectionIDMismatch), culture, arg0, arg1);

    public static string Error_InstructionNameNotSupported(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_InstructionNameNotSupported), arg0);

    public static string Error_InstructionNameNotSupported(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_InstructionNameNotSupported), culture, arg0);

    public static string Error_InvalidBatchOperation(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_InvalidBatchOperation), arg0);

    public static string Error_InvalidBatchOperation(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_InvalidBatchOperation), culture, arg0);

    public static string Error_InvalidProtocolOperation(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_InvalidProtocolOperation), arg0);

    public static string Error_InvalidProtocolOperation(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_InvalidProtocolOperation), culture, arg0);

    public static string Error_LockNameTooLong(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_LockNameTooLong), arg0);

    public static string Error_LockNameTooLong(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_LockNameTooLong), culture, arg0);

    public static string Error_MigrationNotFound(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_MigrationNotFound), arg0);

    public static string Error_MigrationNotFound(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_MigrationNotFound), culture, arg0);

    public static string Error_NotAValidIntegerRange() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_NotAValidIntegerRange));

    public static string Error_NotAValidIntegerRange(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_NotAValidIntegerRange), culture);

    public static string Error_PropertyNotReadable(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PropertyNotReadable), arg0);

    public static string Error_PropertyNotReadable(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PropertyNotReadable), culture, arg0);

    public static string Error_RequiredJobIdNotFound(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_RequiredJobIdNotFound), arg0, arg1, arg2, arg3);
    }

    public static string Error_RequiredJobIdNotFound(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_RequiredJobIdNotFound), culture, arg0, arg1, arg2, arg3);
    }

    public static string Error_UnexpectedJobState(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UnexpectedJobState), arg0, arg1);

    public static string Error_UnexpectedJobState(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UnexpectedJobState), culture, arg0, arg1);

    public static string Error_UnrecognizedMigrationEnum(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UnrecognizedMigrationEnum), arg0);

    public static string Error_UnrecognizedMigrationEnum(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UnrecognizedMigrationEnum), culture, arg0);

    public static string Error_UpstreamSourceEmpty() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_UpstreamSourceEmpty));

    public static string Error_UpstreamSourceEmpty(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_UpstreamSourceEmpty), culture);

    public static string Error_ExpectedStartOfArrayOrObject() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ExpectedStartOfArrayOrObject));

    public static string Error_ExpectedStartOfArrayOrObject(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ExpectedStartOfArrayOrObject), culture);

    public static string TraceError_FailedToRefreshUpstreamMetadataForFeedAndPackage(
      object arg0,
      object arg1)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (TraceError_FailedToRefreshUpstreamMetadataForFeedAndPackage), arg0, arg1);
    }

    public static string TraceError_FailedToRefreshUpstreamMetadataForFeedAndPackage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (TraceError_FailedToRefreshUpstreamMetadataForFeedAndPackage), culture, arg0, arg1);
    }

    public static string Error_StorageIdMustBeString() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_StorageIdMustBeString));

    public static string Error_StorageIdMustBeString(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_StorageIdMustBeString), culture);

    public static string Error_UpstreamFailedToRefresh(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UpstreamFailedToRefresh), arg0, arg1);

    public static string Error_UpstreamFailedToRefresh(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UpstreamFailedToRefresh), culture, arg0, arg1);
    }

    public static string Error_CannotRestorePermanentlyDeletedPackage(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CannotRestorePermanentlyDeletedPackage), arg0);

    public static string Error_CannotRestorePermanentlyDeletedPackage(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CannotRestorePermanentlyDeletedPackage), culture, arg0);
    }

    public static string Error_DeleteBeforePermanentDeletion(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_DeleteBeforePermanentDeletion), arg0);

    public static string Error_DeleteBeforePermanentDeletion(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_DeleteBeforePermanentDeletion), culture, arg0);

    public static string Error_DeletionNotSupportedOnPatch() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_DeletionNotSupportedOnPatch));

    public static string Error_DeletionNotSupportedOnPatch(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_DeletionNotSupportedOnPatch), culture);

    public static string Error_PackageVersionNotFoundInRecycleBin(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageVersionNotFoundInRecycleBin), arg0);

    public static string Error_PackageVersionNotFoundInRecycleBin(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageVersionNotFoundInRecycleBin), culture, arg0);

    public static string Error_PackageVersionNotFound(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageVersionNotFound), arg0, arg1, arg2);

    public static string Error_PackageVersionNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageVersionNotFound), culture, arg0, arg1, arg2);
    }

    public static string Error_UpstreamSourceNotSupported() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_UpstreamSourceNotSupported));

    public static string Error_UpstreamSourceNotSupported(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_UpstreamSourceNotSupported), culture);

    public static string Error_DeleteBeforeRestoringToFeed(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_DeleteBeforeRestoringToFeed), arg0);

    public static string Error_DeleteBeforeRestoringToFeed(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_DeleteBeforeRestoringToFeed), culture, arg0);

    public static string Error_UnexpectedUpstreamPackage(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UnexpectedUpstreamPackage), arg0, arg1);

    public static string Error_UnexpectedUpstreamPackage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UnexpectedUpstreamPackage), culture, arg0, arg1);
    }

    public static string Error_CannotPublishExistsOnUpstream(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CannotPublishExistsOnUpstream), arg0, arg1);

    public static string Error_CannotPublishExistsOnUpstream(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CannotPublishExistsOnUpstream), culture, arg0, arg1);
    }

    public static string Error_BatchRequestTooLarge(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_BatchRequestTooLarge), arg0, arg1, arg2, arg3);
    }

    public static string Error_BatchRequestTooLarge(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_BatchRequestTooLarge), culture, arg0, arg1, arg2, arg3);
    }

    public static string Error_UpstreamSourceTypeNotSupported(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UpstreamSourceTypeNotSupported), arg0, arg1);

    public static string Error_UpstreamSourceTypeNotSupported(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UpstreamSourceTypeNotSupported), culture, arg0, arg1);
    }

    public static string Error_MissingDocumentProperties(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_MissingDocumentProperties), arg0);

    public static string Error_MissingDocumentProperties(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_MissingDocumentProperties), culture, arg0);

    public static string Error_TokenNotAProperty(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_TokenNotAProperty), arg0);

    public static string Error_TokenNotAProperty(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_TokenNotAProperty), culture, arg0);

    public static string Error_ValueNotFound() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ValueNotFound));

    public static string Error_ValueNotFound(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ValueNotFound), culture);

    public static string Error_SourceChainNotFoundInMap(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_SourceChainNotFoundInMap), arg0, arg1);

    public static string Error_SourceChainNotFoundInMap(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_SourceChainNotFoundInMap), culture, arg0, arg1);
    }

    public static string Error_MissingSourceChainMap() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_MissingSourceChainMap));

    public static string Error_MissingSourceChainMap(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_MissingSourceChainMap), culture);

    public static string Error_SourceChainIndexOutOfBounds(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_SourceChainIndexOutOfBounds), arg0, arg1);

    public static string Error_SourceChainIndexOutOfBounds(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_SourceChainIndexOutOfBounds), culture, arg0, arg1);
    }

    public static string Error_ServiceReadOnly() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ServiceReadOnly));

    public static string Error_ServiceReadOnly(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ServiceReadOnly), culture);

    public static string Error_FeedAlreadyContainsPackage(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_FeedAlreadyContainsPackage), arg0);

    public static string Error_FeedAlreadyContainsPackage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_FeedAlreadyContainsPackage), culture, arg0);

    public static string Error_FeedAlreadyContainsPackageFile(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_FeedAlreadyContainsPackageFile), arg0, arg1, arg2);
    }

    public static string Error_FeedAlreadyContainsPackageFile(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_FeedAlreadyContainsPackageFile), culture, arg0, arg1, arg2);
    }

    public static string Error_PackageVersionDeleted(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageVersionDeleted), arg0, arg1, arg2);

    public static string Error_PackageVersionDeleted(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageVersionDeleted), culture, arg0, arg1, arg2);
    }

    public static string Error_FeedIsReadOnly() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_FeedIsReadOnly));

    public static string Error_FeedIsReadOnly(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_FeedIsReadOnly), culture);

    public static string Error_V2DoesntCache() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_V2DoesntCache));

    public static string Error_V2DoesntCache(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_V2DoesntCache), culture);

    public static string Error_PackageNotFound(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageNotFound), arg0, arg1);

    public static string Error_PackageNotFound(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageNotFound), culture, arg0, arg1);

    public static string Error_PackageFileNotFound(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageFileNotFound), arg0, arg1, arg2);

    public static string Error_PackageFileNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageFileNotFound), culture, arg0, arg1, arg2);
    }

    public static string Error_PackageNotFoundInUpstream(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageNotFoundInUpstream), arg0, arg1);

    public static string Error_PackageNotFoundInUpstream(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageNotFoundInUpstream), culture, arg0, arg1);
    }

    public static string Error_PackageNotFoundInUpstreams(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageNotFoundInUpstreams), arg0, arg1, arg2);

    public static string Error_PackageNotFoundInUpstreams(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageNotFoundInUpstreams), culture, arg0, arg1, arg2);
    }

    public static string Error_PackageFileNotFoundInUpstream(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageFileNotFoundInUpstream), arg0, arg1, arg2, arg3);
    }

    public static string Error_PackageFileNotFoundInUpstream(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageFileNotFoundInUpstream), culture, arg0, arg1, arg2, arg3);
    }

    public static string Error_PackageFileNotFoundInUpstreams(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageFileNotFoundInUpstreams), arg0, arg1, arg2, arg3);
    }

    public static string Error_PackageFileNotFoundInUpstreams(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageFileNotFoundInUpstreams), culture, arg0, arg1, arg2, arg3);
    }

    public static string Error_InternalUpstreamSourceDeleted(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_InternalUpstreamSourceDeleted), arg0, arg1);

    public static string Error_InternalUpstreamSourceDeleted(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_InternalUpstreamSourceDeleted), culture, arg0, arg1);
    }

    public static string Error_InternalUpstreamSourceNotFound(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_InternalUpstreamSourceNotFound), arg0);

    public static string Error_InternalUpstreamSourceNotFound(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_InternalUpstreamSourceNotFound), culture, arg0);

    public static string Error_UpstreamProjectDoesNotExist(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UpstreamProjectDoesNotExist), arg0, arg1);

    public static string Error_UpstreamProjectDoesNotExist(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UpstreamProjectDoesNotExist), culture, arg0, arg1);
    }

    public static string Warning_PackagingClientWarningTFSActivityIdSuffix(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Warning_PackagingClientWarningTFSActivityIdSuffix), arg0);

    public static string Warning_PackagingClientWarningTFSActivityIdSuffix(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Warning_PackagingClientWarningTFSActivityIdSuffix), culture, arg0);
    }

    public static string Warning_PackagingClientWarningVSTSActivityIdSuffix(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Warning_PackagingClientWarningVSTSActivityIdSuffix), arg0);

    public static string Warning_PackagingClientWarningVSTSActivityIdSuffix(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Warning_PackagingClientWarningVSTSActivityIdSuffix), culture, arg0);
    }

    public static string EmptyVersionNotAllowed() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (EmptyVersionNotAllowed));

    public static string EmptyVersionNotAllowed(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (EmptyVersionNotAllowed), culture);

    public static string Job_UpstreamRefresh(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Job_UpstreamRefresh), arg0, arg1, arg2);

    public static string Job_UpstreamRefresh(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Job_UpstreamRefresh), culture, arg0, arg1, arg2);
    }

    public static string Error_PackageFileNotFoundTryIngesting(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageFileNotFoundTryIngesting), arg0, arg1, arg2, arg3);
    }

    public static string Error_PackageFileNotFoundTryIngesting(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageFileNotFoundTryIngesting), culture, arg0, arg1, arg2, arg3);
    }

    public static string Error_PackageVersionNotFoundInFeedWithId(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageVersionNotFoundInFeedWithId), arg0, arg1, arg2);
    }

    public static string Error_PackageVersionNotFoundInFeedWithId(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageVersionNotFoundInFeedWithId), culture, arg0, arg1, arg2);
    }

    public static string Error_PackageBlockedDueToSecurityIncident(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageBlockedDueToSecurityIncident), arg0);

    public static string Error_PackageBlockedDueToSecurityIncident(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageBlockedDueToSecurityIncident), culture, arg0);

    public static string Error_PackageBlockedDueToSecurityIncidentAllVersions(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageBlockedDueToSecurityIncidentAllVersions), arg0);

    public static string Error_PackageBlockedDueToSecurityIncidentAllVersions(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageBlockedDueToSecurityIncidentAllVersions), culture, arg0);
    }

    public static string Error_InternalUpstreamToBaseFeedNotPermitted() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InternalUpstreamToBaseFeedNotPermitted));

    public static string Error_InternalUpstreamToBaseFeedNotPermitted(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InternalUpstreamToBaseFeedNotPermitted), culture);

    public static string Error_ViewNotWidelyVisible(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_ViewNotWidelyVisible), arg0, arg1);

    public static string Error_ViewNotWidelyVisible(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_ViewNotWidelyVisible), culture, arg0, arg1);

    public static string Error_NoMatchingHandlerForTypeInMigration(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NoMatchingHandlerForTypeInMigration), arg0, arg1);

    public static string Error_NoMatchingHandlerForTypeInMigration(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NoMatchingHandlerForTypeInMigration), culture, arg0, arg1);
    }

    public static string Error_PackageInnerFileNotFound(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageInnerFileNotFound), arg0, arg1, arg2);

    public static string Error_PackageInnerFileNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageInnerFileNotFound), culture, arg0, arg1, arg2);
    }

    public static string Error_NoDataGeneratedFromRequest(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NoDataGeneratedFromRequest), arg0, arg1, arg2);

    public static string Error_NoDataGeneratedFromRequest(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NoDataGeneratedFromRequest), culture, arg0, arg1, arg2);
    }

    public static string Error_PackageScopeUpstreamStatusNotImplemented() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_PackageScopeUpstreamStatusNotImplemented));

    public static string Error_PackageScopeUpstreamStatusNotImplemented(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_PackageScopeUpstreamStatusNotImplemented), culture);

    public static string Error_UnknownProtocol(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UnknownProtocol), arg0);

    public static string Error_UnknownProtocol(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UnknownProtocol), culture, arg0);

    public static string Error_InvalidCommitOperationDataMessage() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InvalidCommitOperationDataMessage));

    public static string Error_InvalidCommitOperationDataMessage(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_InvalidCommitOperationDataMessage), culture);

    public static string Error_ShouldInflateButNoZlibHeader() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ShouldInflateButNoZlibHeader));

    public static string Error_ShouldInflateButNoZlibHeader(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ShouldInflateButNoZlibHeader), culture);

    public static string Error_AuthTokenWasNull(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_AuthTokenWasNull), arg0);

    public static string Error_AuthTokenWasNull(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_AuthTokenWasNull), culture, arg0);

    public static string Error_NoAuthFoundInServiceConnection() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_NoAuthFoundInServiceConnection));

    public static string Error_NoAuthFoundInServiceConnection(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_NoAuthFoundInServiceConnection), culture);

    public static string Error_NoAuthTokenFoundInServiceConnection(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NoAuthTokenFoundInServiceConnection), arg0);

    public static string Error_NoAuthTokenFoundInServiceConnection(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NoAuthTokenFoundInServiceConnection), culture, arg0);

    public static string Error_NoServiceConnectionFound() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_NoServiceConnectionFound));

    public static string Error_NoServiceConnectionFound(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_NoServiceConnectionFound), culture);

    public static string Error_ServiceConnectionDisabled() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ServiceConnectionDisabled));

    public static string Error_ServiceConnectionDisabled(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_ServiceConnectionDisabled), culture);

    public static string Error_UpstreamJobFeedRequestDifferentFromRefreshInformationFeed(
      object arg0,
      object arg1)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UpstreamJobFeedRequestDifferentFromRefreshInformationFeed), arg0, arg1);
    }

    public static string Error_UpstreamJobFeedRequestDifferentFromRefreshInformationFeed(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UpstreamJobFeedRequestDifferentFromRefreshInformationFeed), culture, arg0, arg1);
    }

    public static string Error_CannotMixFilesFromDifferentUpstreams(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CannotMixFilesFromDifferentUpstreams), arg0, arg1, arg2);
    }

    public static string Error_CannotMixFilesFromDifferentUpstreams(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CannotMixFilesFromDifferentUpstreams), culture, arg0, arg1, arg2);
    }

    public static string Error_IncorrectPackageIdentityUpstream(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_IncorrectPackageIdentityUpstream), arg0, arg1);

    public static string Error_IncorrectPackageIdentityUpstream(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_IncorrectPackageIdentityUpstream), culture, arg0, arg1);
    }

    public static string Error_IncorrectPackageIdentityDirectPush(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_IncorrectPackageIdentityDirectPush), arg0, arg1);

    public static string Error_IncorrectPackageIdentityDirectPush(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_IncorrectPackageIdentityDirectPush), culture, arg0, arg1);
    }

    public static string Error_BlockedUpstream(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_BlockedUpstream), arg0, arg1, arg2);

    public static string Error_BlockedUpstream(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_BlockedUpstream), culture, arg0, arg1, arg2);
    }

    public static string Error_DisplayNameTooLong(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_DisplayNameTooLong), arg0, arg1);

    public static string Error_DisplayNameTooLong(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_DisplayNameTooLong), culture, arg0, arg1);

    public static string Error_DisplayVersionTooLong(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_DisplayVersionTooLong), arg0, arg1);

    public static string Error_DisplayVersionTooLong(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_DisplayVersionTooLong), culture, arg0, arg1);

    public static string Error_NormalizedNameTooLong(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NormalizedNameTooLong), arg0, arg1);

    public static string Error_NormalizedNameTooLong(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NormalizedNameTooLong), culture, arg0, arg1);

    public static string Error_NormalizedVersionTooLong(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NormalizedVersionTooLong), arg0, arg1);

    public static string Error_NormalizedVersionTooLong(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NormalizedVersionTooLong), culture, arg0, arg1);
    }

    public static string Error_NoCurrentStateForNonAddOperation() => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_NoCurrentStateForNonAddOperation));

    public static string Error_NoCurrentStateForNonAddOperation(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Get(nameof (Error_NoCurrentStateForNonAddOperation), culture);

    public static string Error_AuthForPossibleUpstreamRefresh(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_AuthForPossibleUpstreamRefresh), arg0);

    public static string Error_AuthForPossibleUpstreamRefresh(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_AuthForPossibleUpstreamRefresh), culture, arg0);

    public static string Error_AuthForVersionIngestion(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_AuthForVersionIngestion), arg0, arg1);

    public static string Error_AuthForVersionIngestion(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_AuthForVersionIngestion), culture, arg0, arg1);
    }

    public static string Error_PrematureEndOfUploadStream(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PrematureEndOfUploadStream), arg0, arg1);

    public static string Error_PrematureEndOfUploadStream(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PrematureEndOfUploadStream), culture, arg0, arg1);
    }

    public static string Error_CommitDeserializerCannotDetermineIdentity(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CommitDeserializerCannotDetermineIdentity), arg0);

    public static string Error_CommitDeserializerCannotDetermineIdentity(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CommitDeserializerCannotDetermineIdentity), culture, arg0);
    }

    public static string Error_CommitDeserializerCannotDetermineOperation(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CommitDeserializerCannotDetermineOperation), arg0);

    public static string Error_CommitDeserializerCannotDetermineOperation(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CommitDeserializerCannotDetermineOperation), culture, arg0);
    }

    public static string Error_CommitDeserializerNestedBatch(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CommitDeserializerNestedBatch), arg0);

    public static string Error_CommitDeserializerNestedBatch(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CommitDeserializerNestedBatch), culture, arg0);

    public static string Error_CommitDeserializerUnknownOperation(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CommitDeserializerUnknownOperation), arg0, arg1);

    public static string Error_CommitDeserializerUnknownOperation(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CommitDeserializerUnknownOperation), culture, arg0, arg1);
    }

    public static string Error_CannotExtractFromNonLocalContent(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CannotExtractFromNonLocalContent), arg0, arg1, arg2, arg3);
    }

    public static string Error_CannotExtractFromNonLocalContent(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CannotExtractFromNonLocalContent), culture, arg0, arg1, arg2, arg3);
    }

    public static string Error_CannotExtractFromNonLocalContentInnerFile(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CannotExtractFromNonLocalContentInnerFile), arg0, arg1, arg2, arg3, arg4);
    }

    public static string Error_CannotExtractFromNonLocalContentInnerFile(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_CannotExtractFromNonLocalContentInnerFile), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string Error_NoContentProviderForId(object arg0) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NoContentProviderForId), arg0);

    public static string Error_NoContentProviderForId(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NoContentProviderForId), culture, arg0);

    public static string Error_NoDirectDownloadNotSingleFile(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NoDirectDownloadNotSingleFile), arg0, arg1, arg2, arg3);
    }

    public static string Error_NoDirectDownloadNotSingleFile(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NoDirectDownloadNotSingleFile), culture, arg0, arg1, arg2, arg3);
    }

    public static string Error_NoDirectDownloadNotSingleFileInnerFile(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NoDirectDownloadNotSingleFileInnerFile), arg0, arg1, arg2, arg3, arg4);
    }

    public static string Error_NoDirectDownloadNotSingleFileInnerFile(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_NoDirectDownloadNotSingleFileInnerFile), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string Error_PackageFileNotFoundTryIngestingInnerFile(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageFileNotFoundTryIngestingInnerFile), arg0, arg1, arg2, arg3, arg4);
    }

    public static string Error_PackageFileNotFoundTryIngestingInnerFile(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageFileNotFoundTryIngestingInnerFile), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string Error_PackageFileNotFoundInUpstreamNoFeed(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageFileNotFoundInUpstreamNoFeed), arg0, arg1, arg2, arg3);
    }

    public static string Error_PackageFileNotFoundInUpstreamNoFeed(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageFileNotFoundInUpstreamNoFeed), culture, arg0, arg1, arg2, arg3);
    }

    public static string Error_PackageNotFoundInUpstreamNoFeed(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageNotFoundInUpstreamNoFeed), arg0, arg1, arg2);
    }

    public static string Error_PackageNotFoundInUpstreamNoFeed(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_PackageNotFoundInUpstreamNoFeed), culture, arg0, arg1, arg2);
    }

    public static string Error_UpstreamFailure(object arg0, object arg1) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UpstreamFailure), arg0, arg1);

    public static string Error_UpstreamFailure(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Format(nameof (Error_UpstreamFailure), culture, arg0, arg1);
  }
}
