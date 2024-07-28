// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Resources
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.NuGet.Server
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.NuGet.Server.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.NuGet.Server.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.NuGet.Server.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.NuGet.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.NuGet.Server.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.NuGet.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.NuGet.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.NuGet.Server.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.NuGet.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(resourceName, culture);
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

    public static string DeprecatedServiceEntry_UseLocationService() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (DeprecatedServiceEntry_UseLocationService));

    public static string DeprecatedServiceEntry_UseLocationService(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (DeprecatedServiceEntry_UseLocationService), culture);

    public static string DeprecatedServiceEntry_WithReplacement(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (DeprecatedServiceEntry_WithReplacement), arg0);

    public static string DeprecatedServiceEntry_WithReplacement(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (DeprecatedServiceEntry_WithReplacement), culture, arg0);

    public static string Error_ArgumentRequired(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_ArgumentRequired), arg0);

    public static string Error_ArgumentRequired(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_ArgumentRequired), culture, arg0);

    public static string Error_BlockListDoesntMatchBlobId() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_BlockListDoesntMatchBlobId));

    public static string Error_BlockListDoesntMatchBlobId(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_BlockListDoesntMatchBlobId), culture);

    public static string Error_CannotPerformOperationOnReadOnlyFeed(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_CannotPerformOperationOnReadOnlyFeed), arg0);

    public static string Error_CannotPerformOperationOnReadOnlyFeed(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_CannotPerformOperationOnReadOnlyFeed), culture, arg0);
    }

    public static string Error_CouldNotFindNuspec() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_CouldNotFindNuspec));

    public static string Error_CouldNotFindNuspec(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_CouldNotFindNuspec), culture);

    public static string Error_DependencyIdNotSpecified() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_DependencyIdNotSpecified));

    public static string Error_DependencyIdNotSpecified(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_DependencyIdNotSpecified), culture);

    public static string Error_FailedToCreateDrop() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_FailedToCreateDrop));

    public static string Error_FailedToCreateDrop(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_FailedToCreateDrop), culture);

    public static string Error_FailedToCreateDropWithStatusCode(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_FailedToCreateDropWithStatusCode), arg0);

    public static string Error_FailedToCreateDropWithStatusCode(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_FailedToCreateDropWithStatusCode), culture, arg0);

    public static string Error_FailedToFinalizeDrop() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_FailedToFinalizeDrop));

    public static string Error_FailedToFinalizeDrop(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_FailedToFinalizeDrop), culture);

    public static string Error_FeedAlreadyContainsPackageDeleted(object arg0, object arg1) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_FeedAlreadyContainsPackageDeleted), arg0, arg1);

    public static string Error_FeedAlreadyContainsPackageDeleted(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_FeedAlreadyContainsPackageDeleted), culture, arg0, arg1);
    }

    public static string Error_GetDownloadUriFailed(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_GetDownloadUriFailed), arg0);

    public static string Error_GetDownloadUriFailed(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_GetDownloadUriFailed), culture, arg0);

    public static string Error_IncorrectBlobsFoundForPackage(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_IncorrectBlobsFoundForPackage), arg0);

    public static string Error_IncorrectBlobsFoundForPackage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_IncorrectBlobsFoundForPackage), culture, arg0);

    public static string Error_IngestionInvalidVersion() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_IngestionInvalidVersion));

    public static string Error_IngestionInvalidVersion(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_IngestionInvalidVersion), culture);

    public static string Error_InvalidBatchOperation(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_InvalidBatchOperation), arg0);

    public static string Error_InvalidBatchOperation(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_InvalidBatchOperation), culture, arg0);

    public static string Error_InvalidBlobId() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_InvalidBlobId));

    public static string Error_InvalidBlobId(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_InvalidBlobId), culture);

    public static string Error_InvalidPackageMetadataInVersion() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_InvalidPackageMetadataInVersion));

    public static string Error_InvalidPackageMetadataInVersion(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_InvalidPackageMetadataInVersion), culture);

    public static string Error_InvalidPackageWithArgument(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_InvalidPackageWithArgument), arg0);

    public static string Error_InvalidPackageWithArgument(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_InvalidPackageWithArgument), culture, arg0);

    public static string Error_InvalidStorageId(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_InvalidStorageId), arg0);

    public static string Error_InvalidStorageId(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_InvalidStorageId), culture, arg0);

    public static string Error_InvalidViewSubOperation() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_InvalidViewSubOperation));

    public static string Error_InvalidViewSubOperation(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_InvalidViewSubOperation), culture);

    public static string Error_MissingNuspecElement(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_MissingNuspecElement), arg0);

    public static string Error_MissingNuspecElement(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_MissingNuspecElement), culture, arg0);

    public static string Error_MustCallInOrder(object arg0, object arg1) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_MustCallInOrder), arg0, arg1);

    public static string Error_MustCallInOrder(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_MustCallInOrder), culture, arg0, arg1);

    public static string Error_NoBlobsForPackageFound() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_NoBlobsForPackageFound));

    public static string Error_NoBlobsForPackageFound(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_NoBlobsForPackageFound), culture);

    public static string Error_NotSupportedOnPrem() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_NotSupportedOnPrem));

    public static string Error_NotSupportedOnPrem(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_NotSupportedOnPrem), culture);

    public static string Error_UpstreamsEmpty() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_UpstreamsEmpty));

    public static string Error_UpstreamsEmpty(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_UpstreamsEmpty), culture);

    public static string Error_NuGetApiKeyRequired() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_NuGetApiKeyRequired));

    public static string Error_NuGetApiKeyRequired(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_NuGetApiKeyRequired), culture);

    public static string Error_NuGetServiceReadOnly() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_NuGetServiceReadOnly));

    public static string Error_NuGetServiceReadOnly(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_NuGetServiceReadOnly), culture);

    public static string Error_NuspecNotFound() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_NuspecNotFound));

    public static string Error_NuspecNotFound(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_NuspecNotFound), culture);

    public static string Error_NuspecTooLarge(object arg0, object arg1) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_NuspecTooLarge), arg0, arg1);

    public static string Error_NuspecTooLarge(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_NuspecTooLarge), culture, arg0, arg1);

    public static string Error_PackageDependencyVersionFormat(object arg0, object arg1) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageDependencyVersionFormat), arg0, arg1);

    public static string Error_PackageDependencyVersionFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageDependencyVersionFormat), culture, arg0, arg1);
    }

    public static string Error_PackageDependencyVersionFormatNoGroup(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageDependencyVersionFormatNoGroup), arg0);

    public static string Error_PackageDependencyVersionFormatNoGroup(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageDependencyVersionFormatNoGroup), culture, arg0);
    }

    public static string Error_PackageIdInvalid() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_PackageIdInvalid));

    public static string Error_PackageIdInvalid(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_PackageIdInvalid), culture);

    public static string Error_PackageIdRequired() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_PackageIdRequired));

    public static string Error_PackageIdRequired(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_PackageIdRequired), culture);

    public static string Error_PackageIdTooLong(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageIdTooLong), arg0);

    public static string Error_PackageIdTooLong(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageIdTooLong), culture, arg0);

    public static string Error_PackageNotFound(object arg0, object arg1) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageNotFound), arg0, arg1);

    public static string Error_PackageNotFound(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageNotFound), culture, arg0, arg1);

    public static string Error_PackageSubresourceDoesntExist(object arg0, object arg1) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageSubresourceDoesntExist), arg0, arg1);

    public static string Error_PackageSubresourceDoesntExist(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageSubresourceDoesntExist), culture, arg0, arg1);
    }

    public static string Error_PackageSubresourceDoesntExistDidYouMean(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageSubresourceDoesntExistDidYouMean), arg0, arg1, arg2);
    }

    public static string Error_PackageSubresourceDoesntExistDidYouMean(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageSubresourceDoesntExistDidYouMean), culture, arg0, arg1, arg2);
    }

    public static string Error_PackageTooLarge(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageTooLarge), arg0);

    public static string Error_PackageTooLarge(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageTooLarge), culture, arg0);

    public static string Error_PackageTooManyVersions(object arg0, object arg1) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageTooManyVersions), arg0, arg1);

    public static string Error_PackageTooManyVersions(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageTooManyVersions), culture, arg0, arg1);
    }

    public static string Error_PackageVersionNotFound(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageVersionNotFound), arg0, arg1, arg2);

    public static string Error_PackageVersionNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageVersionNotFound), culture, arg0, arg1, arg2);
    }

    public static string Error_PackageVersionRequired() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_PackageVersionRequired));

    public static string Error_PackageVersionRequired(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_PackageVersionRequired), culture);

    public static string Error_PackageVersionTooLong(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageVersionTooLong), arg0);

    public static string Error_PackageVersionTooLong(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_PackageVersionTooLong), culture, arg0);

    public static string Error_PushCantUseBothBlobAndBlobId() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_PushCantUseBothBlobAndBlobId));

    public static string Error_PushCantUseBothBlobAndBlobId(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_PushCantUseBothBlobAndBlobId), culture);

    public static string Error_PushCantUseBothBlobAndDrop() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_PushCantUseBothBlobAndDrop));

    public static string Error_PushCantUseBothBlobAndDrop(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_PushCantUseBothBlobAndDrop), culture);

    public static string Error_PushRequestMissingBlob() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_PushRequestMissingBlob));

    public static string Error_PushRequestMissingBlob(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_PushRequestMissingBlob), culture);

    public static string Error_PushValidBlobIdAndBlockHashesAreRequired() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_PushValidBlobIdAndBlockHashesAreRequired));

    public static string Error_PushValidBlobIdAndBlockHashesAreRequired(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_PushValidBlobIdAndBlockHashesAreRequired), culture);

    public static string Error_UnreadableNuspec() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_UnreadableNuspec));

    public static string Error_UnreadableNuspec(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_UnreadableNuspec), culture);

    public static string Error_UpstreamFailure(object arg0, object arg1) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_UpstreamFailure), arg0, arg1);

    public static string Error_UpstreamFailure(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_UpstreamFailure), culture, arg0, arg1);

    public static string Error_UpstreamInvalidServiceIndex() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_UpstreamInvalidServiceIndex));

    public static string Error_UpstreamInvalidServiceIndex(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_UpstreamInvalidServiceIndex), culture);

    public static string Error_UpstreamIngestion_CannotSkipIngestion() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_UpstreamIngestion_CannotSkipIngestion));

    public static string Error_UpstreamIngestion_CannotSkipIngestion(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_UpstreamIngestion_CannotSkipIngestion), culture);

    public static string Error_UpstreamReturnedNotFound(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_UpstreamReturnedNotFound), arg0);

    public static string Error_UpstreamReturnedNotFound(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_UpstreamReturnedNotFound), culture, arg0);

    public static string Error_V2PushRequiresPackagePart() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_V2PushRequiresPackagePart));

    public static string Error_V2PushRequiresPackagePart(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_V2PushRequiresPackagePart), culture);

    public static string Error_ValueCannotBeNull(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_ValueCannotBeNull), arg0);

    public static string Error_ValueCannotBeNull(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_ValueCannotBeNull), culture, arg0);

    public static string Warning_NuGetWarningTFSActivityIdSuffix(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Warning_NuGetWarningTFSActivityIdSuffix), arg0);

    public static string Warning_NuGetWarningTFSActivityIdSuffix(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Warning_NuGetWarningTFSActivityIdSuffix), culture, arg0);

    public static string Warning_NuGetWarningVSTSActivityIdSuffix(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Warning_NuGetWarningVSTSActivityIdSuffix), arg0);

    public static string Warning_NuGetWarningVSTSActivityIdSuffix(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Warning_NuGetWarningVSTSActivityIdSuffix), culture, arg0);

    public static string Error_NupkgIngestionNotSupportedForDirectDownload() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_NupkgIngestionNotSupportedForDirectDownload));

    public static string Error_NupkgIngestionNotSupportedForDirectDownload(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_NupkgIngestionNotSupportedForDirectDownload), culture);

    public static string Error_InvalidUpstreamUrl() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_InvalidUpstreamUrl));

    public static string Error_InvalidUpstreamUrl(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_InvalidUpstreamUrl), culture);

    public static string Error_InvalidMultipartRequest() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_InvalidMultipartRequest));

    public static string Error_InvalidMultipartRequest(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_InvalidMultipartRequest), culture);

    public static string Error_CantPromoteToImplicitView() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_CantPromoteToImplicitView));

    public static string Error_CantPromoteToImplicitView(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_CantPromoteToImplicitView), culture);

    public static string Error_IngestIncorrectPackageIdentity(object arg0, object arg1) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_IngestIncorrectPackageIdentity), arg0, arg1);

    public static string Error_IngestIncorrectPackageIdentity(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_IngestIncorrectPackageIdentity), culture, arg0, arg1);
    }

    public static string Error_NoStreamForBlobBackedPackage() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_NoStreamForBlobBackedPackage));

    public static string Error_NoStreamForBlobBackedPackage(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_NoStreamForBlobBackedPackage), culture);

    public static string EmptyVersionNotAllowed() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (EmptyVersionNotAllowed));

    public static string EmptyVersionNotAllowed(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (EmptyVersionNotAllowed), culture);

    public static string Error_ArgumentOutOfRange(object arg0) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_ArgumentOutOfRange), arg0);

    public static string Error_ArgumentOutOfRange(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Format(nameof (Error_ArgumentOutOfRange), culture, arg0);

    public static string Error_ODataFilterNotSupported() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_ODataFilterNotSupported));

    public static string Error_ODataFilterNotSupported(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_ODataFilterNotSupported), culture);

    public static string Error_ODataOrderByNotSupported() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_ODataOrderByNotSupported));

    public static string Error_ODataOrderByNotSupported(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_ODataOrderByNotSupported), culture);

    public static string Error_CanOnlyExtractIconOrLicense() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_CanOnlyExtractIconOrLicense));

    public static string Error_CanOnlyExtractIconOrLicense(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_CanOnlyExtractIconOrLicense), culture);

    public static string Error_UpstreamRegistrationInvalid() => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_UpstreamRegistrationInvalid));

    public static string Error_UpstreamRegistrationInvalid(CultureInfo culture) => Microsoft.VisualStudio.Services.NuGet.Server.Resources.Get(nameof (Error_UpstreamRegistrationInvalid), culture);
  }
}
