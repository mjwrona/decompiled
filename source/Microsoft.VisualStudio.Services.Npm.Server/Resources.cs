// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Resources
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Npm.Server
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.Npm.Server.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.Npm.Server.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.Npm.Server.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.Npm.Server.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.Npm.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Npm.Server.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.Npm.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.Npm.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Npm.Server.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.Npm.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(resourceName, culture);
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

    public static string Error_FeedAlreadyContainsPackage(object arg0, object arg1) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_FeedAlreadyContainsPackage), arg0, arg1);

    public static string Error_FeedAlreadyContainsPackage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_FeedAlreadyContainsPackage), culture, arg0, arg1);
    }

    public static string Error_GetDownloadUriFailed(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_GetDownloadUriFailed), arg0);

    public static string Error_GetDownloadUriFailed(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_GetDownloadUriFailed), culture, arg0);

    public static string Error_InvalidPackageFileName() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_InvalidPackageFileName));

    public static string Error_InvalidPackageFileName(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_InvalidPackageFileName), culture);

    public static string Error_InvalidPackageJson() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_InvalidPackageJson));

    public static string Error_InvalidPackageJson(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_InvalidPackageJson), culture);

    public static string Error_InvalidPackageName(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InvalidPackageName), arg0);

    public static string Error_InvalidPackageName(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InvalidPackageName), culture, arg0);

    public static string Error_InvalidPackageTarball() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_InvalidPackageTarball));

    public static string Error_InvalidPackageTarball(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_InvalidPackageTarball), culture);

    public static string Error_InvalidPackageVersion(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InvalidPackageVersion), arg0);

    public static string Error_InvalidPackageVersion(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InvalidPackageVersion), culture, arg0);

    public static string Error_InvalidPackageVersionTooLong(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InvalidPackageVersionTooLong), arg0);

    public static string Error_InvalidPackageVersionTooLong(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InvalidPackageVersionTooLong), culture, arg0);

    public static string Error_InvalidLatestPackageVersion() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_InvalidLatestPackageVersion));

    public static string Error_InvalidLatestPackageVersion(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_InvalidLatestPackageVersion), culture);

    public static string Error_NoPackageJsonInTarball() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_NoPackageJsonInTarball));

    public static string Error_NoPackageJsonInTarball(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_NoPackageJsonInTarball), culture);

    public static string Error_NoVersionInPackageMetadata() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_NoVersionInPackageMetadata));

    public static string Error_NoVersionInPackageMetadata(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_NoVersionInPackageMetadata), culture);

    public static string Error_PackageAddOrUpdateMustHaveBody() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_PackageAddOrUpdateMustHaveBody));

    public static string Error_PackageAddOrUpdateMustHaveBody(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_PackageAddOrUpdateMustHaveBody), culture);

    public static string Error_PackageJsonAndRequestMustMatch(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageJsonAndRequestMustMatch), arg0);

    public static string Error_PackageJsonAndRequestMustMatch(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageJsonAndRequestMustMatch), culture, arg0);

    public static string Error_PackageNameMustBeAlphaNumericOrDashUnderscoreDot() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_PackageNameMustBeAlphaNumericOrDashUnderscoreDot));

    public static string Error_PackageNameMustBeAlphaNumericOrDashUnderscoreDot(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_PackageNameMustBeAlphaNumericOrDashUnderscoreDot), culture);

    public static string Error_PackageNameMustBeLowercase() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_PackageNameMustBeLowercase));

    public static string Error_PackageNameMustBeLowercase(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_PackageNameMustBeLowercase), culture);

    public static string Error_PackageNameMustNotBeNullOrEmpty() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_PackageNameMustNotBeNullOrEmpty));

    public static string Error_PackageNameMustNotBeNullOrEmpty(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_PackageNameMustNotBeNullOrEmpty), culture);

    public static string Error_PackageNameMustNotContainWhitespace() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_PackageNameMustNotContainWhitespace));

    public static string Error_PackageNameMustNotContainWhitespace(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_PackageNameMustNotContainWhitespace), culture);

    public static string Error_PackageNameMustNotStartWithSpecialCharacter() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_PackageNameMustNotStartWithSpecialCharacter));

    public static string Error_PackageNameMustNotStartWithSpecialCharacter(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_PackageNameMustNotStartWithSpecialCharacter), culture);

    public static string Error_PackageNameReserved() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_PackageNameReserved));

    public static string Error_PackageNameReserved(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_PackageNameReserved), culture);

    public static string Error_PackageNameTooLong(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageNameTooLong), arg0);

    public static string Error_PackageNameTooLong(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageNameTooLong), culture, arg0);

    public static string Error_PackageNotFound(object arg0, object arg1) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageNotFound), arg0, arg1);

    public static string Error_PackageNotFound(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageNotFound), culture, arg0, arg1);

    public static string Error_PackagePublishMustHaveOneAttachment() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_PackagePublishMustHaveOneAttachment));

    public static string Error_PackagePublishMustHaveOneAttachment(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_PackagePublishMustHaveOneAttachment), culture);

    public static string Error_PackageVersionNotFound(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageVersionNotFound), arg0, arg1, arg2);

    public static string Error_PackageVersionNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageVersionNotFound), culture, arg0, arg1, arg2);
    }

    public static string Error_RequestMustContainPackageJsonVersion() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_RequestMustContainPackageJsonVersion));

    public static string Error_RequestMustContainPackageJsonVersion(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_RequestMustContainPackageJsonVersion), culture);

    public static string Error_VersionsNotFoundInPackageMetadata() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_VersionsNotFoundInPackageMetadata));

    public static string Error_VersionsNotFoundInPackageMetadata(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_VersionsNotFoundInPackageMetadata), culture);

    public static string Error_VersionWasMissingTarball() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_VersionWasMissingTarball));

    public static string Error_VersionWasMissingTarball(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_VersionWasMissingTarball), culture);

    public static string Error_NpmServiceReadOnly() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_NpmServiceReadOnly));

    public static string Error_NpmServiceReadOnly(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_NpmServiceReadOnly), culture);

    public static string Error_DistTagIsSemanticVersion() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_DistTagIsSemanticVersion));

    public static string Error_DistTagIsSemanticVersion(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_DistTagIsSemanticVersion), culture);

    public static string Error_DistTagNotFound(object arg0, object arg1) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_DistTagNotFound), arg0, arg1);

    public static string Error_DistTagNotFound(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_DistTagNotFound), culture, arg0, arg1);

    public static string Error_MultipleVersionsUnpublished(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_MultipleVersionsUnpublished), arg0);

    public static string Error_MultipleVersionsUnpublished(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_MultipleVersionsUnpublished), culture, arg0);

    public static string Error_RevisionMismatch() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_RevisionMismatch));

    public static string Error_RevisionMismatch(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_RevisionMismatch), culture);

    public static string Error_UnknownOperationType(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UnknownOperationType), arg0);

    public static string Error_UnknownOperationType(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UnknownOperationType), culture, arg0);

    public static string Error_OperationApplierNotFound(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_OperationApplierNotFound), arg0);

    public static string Error_OperationApplierNotFound(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_OperationApplierNotFound), culture, arg0);

    public static string Error_PackageTooManyVersions(object arg0, object arg1) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageTooManyVersions), arg0, arg1);

    public static string Error_PackageTooManyVersions(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageTooManyVersions), culture, arg0, arg1);
    }

    public static string Error_PackageTooLarge(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageTooLarge), arg0);

    public static string Error_PackageTooLarge(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageTooLarge), culture, arg0);

    public static string Error_PackageJsonTooLarge(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageJsonTooLarge), arg0);

    public static string Error_PackageJsonTooLarge(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageJsonTooLarge), culture, arg0);

    public static string Error_PackageNameExistsOnUpstream(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageNameExistsOnUpstream), arg0);

    public static string Error_PackageNameExistsOnUpstream(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageNameExistsOnUpstream), culture, arg0);

    public static string Error_UpstreamUnavailable() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_UpstreamUnavailable));

    public static string Error_UpstreamUnavailable(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_UpstreamUnavailable), culture);

    public static string Error_CannotParsePackageJson(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_CannotParsePackageJson), arg0);

    public static string Error_CannotParsePackageJson(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_CannotParsePackageJson), culture, arg0);

    public static string Error_CannotCreateOrUpdateCachedPackage() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_CannotCreateOrUpdateCachedPackage));

    public static string Error_CannotCreateOrUpdateCachedPackage(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_CannotCreateOrUpdateCachedPackage), culture);

    public static string Error_CannotDeletePackageWithMultipleVersions() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_CannotDeletePackageWithMultipleVersions));

    public static string Error_CannotDeletePackageWithMultipleVersions(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_CannotDeletePackageWithMultipleVersions), culture);

    public static string Error_BadPackageJson() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_BadPackageJson));

    public static string Error_BadPackageJson(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_BadPackageJson), culture);

    public static string Error_ParameterCannotBeNull(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_ParameterCannotBeNull), arg0);

    public static string Error_ParameterCannotBeNull(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_ParameterCannotBeNull), culture, arg0);

    public static string Error_ReadmeNotFoundPackageVersion(object arg0, object arg1) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_ReadmeNotFoundPackageVersion), arg0, arg1);

    public static string Error_ReadmeNotFoundPackageVersion(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_ReadmeNotFoundPackageVersion), culture, arg0, arg1);
    }

    public static string Error_LatestDistTagCannotBeDeleted() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_LatestDistTagCannotBeDeleted));

    public static string Error_LatestDistTagCannotBeDeleted(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_LatestDistTagCannotBeDeleted), culture);

    public static string Error_InvalidPackageVersions() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_InvalidPackageVersions));

    public static string Error_InvalidPackageVersions(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_InvalidPackageVersions), culture);

    public static string Error_OneOrMorePackagesWereNotFound() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_OneOrMorePackagesWereNotFound));

    public static string Error_OneOrMorePackagesWereNotFound(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_OneOrMorePackagesWereNotFound), culture);

    public static string Error_InvalidBatchOperation(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InvalidBatchOperation), arg0);

    public static string Error_InvalidBatchOperation(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InvalidBatchOperation), culture, arg0);

    public static string Error_UpstreamSourceDoesNotSupportNpm() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_UpstreamSourceDoesNotSupportNpm));

    public static string Error_UpstreamSourceDoesNotSupportNpm(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_UpstreamSourceDoesNotSupportNpm), culture);

    public static string Error_InvalidCommitEntryType() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_InvalidCommitEntryType));

    public static string Error_InvalidCommitEntryType(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_InvalidCommitEntryType), culture);

    public static string Error_UpstreamsFailureException(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamsFailureException), arg0);

    public static string Error_UpstreamsFailureException(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamsFailureException), culture, arg0);

    public static string Error_NamesDoNotMatch(object arg0, object arg1) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_NamesDoNotMatch), arg0, arg1);

    public static string Error_NamesDoNotMatch(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_NamesDoNotMatch), culture, arg0, arg1);

    public static string Error_UpstreamSourceTypeNotSupported(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamSourceTypeNotSupported), arg0);

    public static string Error_UpstreamSourceTypeNotSupported(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamSourceTypeNotSupported), culture, arg0);

    public static string Error_UpstreamInvalidPackageVersion(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamInvalidPackageVersion), arg0);

    public static string Error_UpstreamInvalidPackageVersion(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamInvalidPackageVersion), culture, arg0);

    public static string Error_PackageVersionExistsOnUpstream(object arg0, object arg1) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageVersionExistsOnUpstream), arg0, arg1);

    public static string Error_PackageVersionExistsOnUpstream(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageVersionExistsOnUpstream), culture, arg0, arg1);
    }

    public static string Error_NoNameInPackageMetadata() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_NoNameInPackageMetadata));

    public static string Error_NoNameInPackageMetadata(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_NoNameInPackageMetadata), culture);

    public static string Error_OneOrMorePackagesWereNotFoundInRecycleBin() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_OneOrMorePackagesWereNotFoundInRecycleBin));

    public static string Error_OneOrMorePackagesWereNotFoundInRecycleBin(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_OneOrMorePackagesWereNotFoundInRecycleBin), culture);

    public static string Error_InvalidPackageRevision() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_InvalidPackageRevision));

    public static string Error_InvalidPackageRevision(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_InvalidPackageRevision), culture);

    public static string Error_GettingTarballUrlsForInternalUpstreamsNotSupported() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_GettingTarballUrlsForInternalUpstreamsNotSupported));

    public static string Error_GettingTarballUrlsForInternalUpstreamsNotSupported(
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_GettingTarballUrlsForInternalUpstreamsNotSupported), culture);
    }

    public static string Error_CantPromoteToImplicitView() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_CantPromoteToImplicitView));

    public static string Error_CantPromoteToImplicitView(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_CantPromoteToImplicitView), culture);

    public static string Error_PackageNamesDontMatch(object arg0, object arg1) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageNamesDontMatch), arg0, arg1);

    public static string Error_PackageNamesDontMatch(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageNamesDontMatch), culture, arg0, arg1);

    public static string Error_UpstreamUnavailableAt(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamUnavailableAt), arg0);

    public static string Error_UpstreamUnavailableAt(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamUnavailableAt), culture, arg0);

    public static string Error_UpstreamVssServiceExceptionMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamVssServiceExceptionMessage), arg0, arg1, arg2);
    }

    public static string Error_UpstreamVssServiceExceptionMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamVssServiceExceptionMessage), culture, arg0, arg1, arg2);
    }

    public static string Error_UpstreamWebExceptionMessage(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamWebExceptionMessage), arg0, arg1, arg2, arg3);
    }

    public static string Error_UpstreamWebExceptionMessage(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamWebExceptionMessage), culture, arg0, arg1, arg2, arg3);
    }

    public static string Error_UpstreamError(object arg0, object arg1) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamError), arg0, arg1);

    public static string Error_UpstreamError(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamError), culture, arg0, arg1);

    public static string Error_MultipleUpstreamsUnavailable(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_MultipleUpstreamsUnavailable), arg0);

    public static string Error_MultipleUpstreamsUnavailable(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_MultipleUpstreamsUnavailable), culture, arg0);

    public static string Error_FeedNotBeingUpgraded() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_FeedNotBeingUpgraded));

    public static string Error_FeedNotBeingUpgraded(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_FeedNotBeingUpgraded), culture);

    public static string Error_MustUpgradeFeed() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_MustUpgradeFeed));

    public static string Error_MustUpgradeFeed(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_MustUpgradeFeed), culture);

    public static string Error_FeedIsReadOnly() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_FeedIsReadOnly));

    public static string Error_FeedIsReadOnly(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_FeedIsReadOnly), culture);

    public static string Error_V2DoesntCache() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_V2DoesntCache));

    public static string Error_V2DoesntCache(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_V2DoesntCache), culture);

    public static string Error_UpstreamInternalFeedExceptionMessage(object arg0, object arg1) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamInternalFeedExceptionMessage), arg0, arg1);

    public static string Error_UpstreamInternalFeedExceptionMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamInternalFeedExceptionMessage), culture, arg0, arg1);
    }

    public static string Error_UpstreamIngestion_CannotSkipIngestion() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_UpstreamIngestion_CannotSkipIngestion));

    public static string Error_UpstreamIngestion_CannotSkipIngestion(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_UpstreamIngestion_CannotSkipIngestion), culture);

    public static string Error_InvalidPackageMetadata(object arg0, object arg1) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InvalidPackageMetadata), arg0, arg1);

    public static string Error_InvalidPackageMetadata(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InvalidPackageMetadata), culture, arg0, arg1);
    }

    public static string Error_InvalidUpstreamPackageMetadata(object arg0, object arg1) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InvalidUpstreamPackageMetadata), arg0, arg1);

    public static string Error_InvalidUpstreamPackageMetadata(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InvalidUpstreamPackageMetadata), culture, arg0, arg1);
    }

    public static string Error_InvalidUpstreamSourceDuplicatePackages(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InvalidUpstreamSourceDuplicatePackages), arg0, arg1, arg2);
    }

    public static string Error_InvalidUpstreamSourceDuplicatePackages(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InvalidUpstreamSourceDuplicatePackages), culture, arg0, arg1, arg2);
    }

    public static string Error_UpstreamFailureException(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamFailureException), arg0);

    public static string Error_UpstreamFailureException(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamFailureException), culture, arg0);

    public static string Error_UpstreamGenericError(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamGenericError), arg0);

    public static string Error_UpstreamGenericError(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamGenericError), culture, arg0);

    public static string Error_UpstreamProjectDoesNotExist(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamProjectDoesNotExist), arg0);

    public static string Error_UpstreamProjectDoesNotExist(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_UpstreamProjectDoesNotExist), culture, arg0);

    public static string Error_PackageVersionBlocked(object arg0, object arg1) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageVersionBlocked), arg0, arg1);

    public static string Error_PackageVersionBlocked(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageVersionBlocked), culture, arg0, arg1);

    public static string Error_UpstreamSourcesCannotBeQueriedViaViews() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_UpstreamSourcesCannotBeQueriedViaViews));

    public static string Error_UpstreamSourcesCannotBeQueriedViaViews(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_UpstreamSourcesCannotBeQueriedViaViews), culture);

    public static string Error_InternalUpstreamSourceDeleted(object arg0) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InternalUpstreamSourceDeleted), arg0);

    public static string Error_InternalUpstreamSourceDeleted(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InternalUpstreamSourceDeleted), culture, arg0);

    public static string Error_PackageVersionsDontMatch(object arg0, object arg1) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageVersionsDontMatch), arg0, arg1);

    public static string Error_PackageVersionsDontMatch(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_PackageVersionsDontMatch), culture, arg0, arg1);
    }

    public static string Error_AuditNotEnabled() => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_AuditNotEnabled));

    public static string Error_AuditNotEnabled(CultureInfo culture) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Get(nameof (Error_AuditNotEnabled), culture);

    public static string Error_InvalidPackageMetadataUpstream(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InvalidPackageMetadataUpstream), arg0, arg1, arg2);
    }

    public static string Error_InvalidPackageMetadataUpstream(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Npm.Server.Resources.Format(nameof (Error_InvalidPackageMetadataUpstream), culture, arg0, arg1, arg2);
    }
  }
}
