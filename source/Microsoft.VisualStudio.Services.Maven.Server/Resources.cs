// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Resources
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.Maven.Server.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.Maven.Server.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.Maven.Server.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.Maven.Server.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.Maven.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Maven.Server.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.Maven.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.Maven.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Maven.Server.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.Maven.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(resourceName, culture);
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

    public static string Error_ArgumentNullOrEmpty(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArgumentNullOrEmpty), arg0);

    public static string Error_ArgumentNullOrEmpty(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArgumentNullOrEmpty), culture, arg0);

    public static string Error_ArtifactFileChecksumMissing(object arg0, object arg1) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactFileChecksumMissing), arg0, arg1);

    public static string Error_ArtifactFileChecksumMissing(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactFileChecksumMissing), culture, arg0, arg1);
    }

    public static string Error_ArtifactFileExsists(object arg0, object arg1) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactFileExsists), arg0, arg1);

    public static string Error_ArtifactFileExsists(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactFileExsists), culture, arg0, arg1);

    public static string Error_ArtifactFileMissing(object arg0, object arg1) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactFileMissing), arg0, arg1);

    public static string Error_ArtifactFileMissing(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactFileMissing), culture, arg0, arg1);

    public static string Error_ArtifactFileNameChecksumExtensionMissing(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactFileNameChecksumExtensionMissing), arg0);

    public static string Error_ArtifactFileNameChecksumExtensionMissing(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactFileNameChecksumExtensionMissing), culture, arg0);
    }

    public static string Error_ArtifactFileNotFound(object arg0, object arg1) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactFileNotFound), arg0, arg1);

    public static string Error_ArtifactFileNotFound(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactFileNotFound), culture, arg0, arg1);

    public static string Error_ArtifactFullNameNotValid(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactFullNameNotValid), arg0);

    public static string Error_ArtifactFullNameNotValid(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactFullNameNotValid), culture, arg0);

    public static string Error_ArtifactHasNoVersions(object arg0, object arg1) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactHasNoVersions), arg0, arg1);

    public static string Error_ArtifactHasNoVersions(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactHasNoVersions), culture, arg0, arg1);

    public static string Error_ArtifactIdLevelAndGroupIdLevelMetadataFilePathMustHaveSameName(
      object arg0,
      object arg1)
    {
      return Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactIdLevelAndGroupIdLevelMetadataFilePathMustHaveSameName), arg0, arg1);
    }

    public static string Error_ArtifactIdLevelAndGroupIdLevelMetadataFilePathMustHaveSameName(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactIdLevelAndGroupIdLevelMetadataFilePathMustHaveSameName), culture, arg0, arg1);
    }

    public static string Error_ArtifactNotFound(object arg0, object arg1) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactNotFound), arg0, arg1);

    public static string Error_ArtifactNotFound(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactNotFound), culture, arg0, arg1);

    public static string Error_ArtifactVersionNotFound(object arg0, object arg1) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactVersionNotFound), arg0, arg1);

    public static string Error_ArtifactVersionNotFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_ArtifactVersionNotFound), culture, arg0, arg1);
    }

    public static string Error_BlobStorePublishFailed(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_BlobStorePublishFailed), arg0);

    public static string Error_BlobStorePublishFailed(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_BlobStorePublishFailed), culture, arg0);

    public static string Error_DeleteUnsupportedOnViews(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_DeleteUnsupportedOnViews), arg0);

    public static string Error_DeleteUnsupportedOnViews(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_DeleteUnsupportedOnViews), culture, arg0);

    public static string Error_FileNameDoesNotStartWithArtifactId(object arg0, object arg1) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_FileNameDoesNotStartWithArtifactId), arg0, arg1);

    public static string Error_FileNameDoesNotStartWithArtifactId(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_FileNameDoesNotStartWithArtifactId), culture, arg0, arg1);
    }

    public static string Error_FileNameMustSpecifyVersionAfterArtifactId(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_FileNameMustSpecifyVersionAfterArtifactId), arg0);

    public static string Error_FileNameMustSpecifyVersionAfterArtifactId(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_FileNameMustSpecifyVersionAfterArtifactId), culture, arg0);
    }

    public static string Error_FileNotFound(object arg0, object arg1) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_FileNotFound), arg0, arg1);

    public static string Error_FileNotFound(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_FileNotFound), culture, arg0, arg1);

    public static string Error_FilePathContainsInvalidCharacters(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_FilePathContainsInvalidCharacters), arg0);

    public static string Error_FilePathContainsInvalidCharacters(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_FilePathContainsInvalidCharacters), culture, arg0);

    public static string Error_FilePathHasTooFewComponents(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_FilePathHasTooFewComponents), arg0);

    public static string Error_FilePathHasTooFewComponents(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_FilePathHasTooFewComponents), culture, arg0);

    public static string Error_FilePathHasEmptyComponents(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_FilePathHasEmptyComponents), arg0);

    public static string Error_FilePathHasEmptyComponents(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_FilePathHasEmptyComponents), culture, arg0);

    public static string Error_GuidVersionsNotAllowed() => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_GuidVersionsNotAllowed));

    public static string Error_GuidVersionsNotAllowed(CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_GuidVersionsNotAllowed), culture);

    public static string Error_InvalidMavenHashAlgorithm() => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_InvalidMavenHashAlgorithm));

    public static string Error_InvalidMavenHashAlgorithm(CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_InvalidMavenHashAlgorithm), culture);

    public static string Error_MavenServiceReadonly() => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_MavenServiceReadonly));

    public static string Error_MavenServiceReadonly(CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_MavenServiceReadonly), culture);

    public static string Error_MetadataFilePathsMustEndWithMavenMetadata(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_MetadataFilePathsMustEndWithMavenMetadata), arg0);

    public static string Error_MetadataFilePathsMustEndWithMavenMetadata(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_MetadataFilePathsMustEndWithMavenMetadata), culture, arg0);
    }

    public static string Error_NoProtocolDefined() => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_NoProtocolDefined));

    public static string Error_NoProtocolDefined(CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_NoProtocolDefined), culture);

    public static string Error_OperationApplierNotFound(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_OperationApplierNotFound), arg0);

    public static string Error_OperationApplierNotFound(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_OperationApplierNotFound), culture, arg0);

    public static string Error_PackageAddOrUpdateMustHaveBody() => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_PackageAddOrUpdateMustHaveBody));

    public static string Error_PackageAddOrUpdateMustHaveBody(CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_PackageAddOrUpdateMustHaveBody), culture);

    public static string Error_PackageVersionAlreadyDeleted(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_PackageVersionAlreadyDeleted), arg0);

    public static string Error_PackageVersionAlreadyDeleted(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_PackageVersionAlreadyDeleted), culture, arg0);

    public static string Error_PomMissingRootElement(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_PomMissingRootElement), arg0);

    public static string Error_PomMissingRootElement(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_PomMissingRootElement), culture, arg0);

    public static string Error_PomFileSizeLimitExceeded(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_PomFileSizeLimitExceeded), arg0);

    public static string Error_PomFileSizeLimitExceeded(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_PomFileSizeLimitExceeded), culture, arg0);

    public static string Error_RequestHasTooManyFiles() => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_RequestHasTooManyFiles));

    public static string Error_RequestHasTooManyFiles(CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_RequestHasTooManyFiles), culture);

    public static string Error_SnapshotLiteralNotSupported(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_SnapshotLiteralNotSupported), arg0);

    public static string Error_SnapshotLiteralNotSupported(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_SnapshotLiteralNotSupported), culture, arg0);

    public static string Error_SnapshotMetadataFilePathMustSpecifySnapshotVersion(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_SnapshotMetadataFilePathMustSpecifySnapshotVersion), arg0);

    public static string Error_SnapshotMetadataFilePathMustSpecifySnapshotVersion(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_SnapshotMetadataFilePathMustSpecifySnapshotVersion), culture, arg0);
    }

    public static string Error_SnapshotSuffixNotAllowedInGroupIdOrArtifactId(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_SnapshotSuffixNotAllowedInGroupIdOrArtifactId), arg0);

    public static string Error_SnapshotSuffixNotAllowedInGroupIdOrArtifactId(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_SnapshotSuffixNotAllowedInGroupIdOrArtifactId), culture, arg0);
    }

    public static string Error_UnknownOperationType(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_UnknownOperationType), arg0);

    public static string Error_UnknownOperationType(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_UnknownOperationType), culture, arg0);

    public static string Error_VersionListIsReservedName() => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_VersionListIsReservedName));

    public static string Error_VersionListIsReservedName(CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_VersionListIsReservedName), culture);

    public static string Info_DepenencyOptional(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Info_DepenencyOptional), arg0);

    public static string Info_DepenencyOptional(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Info_DepenencyOptional), culture, arg0);

    public static string Info_MetadataLandingPage(object arg0, object arg1) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Info_MetadataLandingPage), arg0, arg1);

    public static string Info_MetadataLandingPage(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Info_MetadataLandingPage), culture, arg0, arg1);

    public static string Info_RepositoryLandingPage(object arg0, object arg1) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Info_RepositoryLandingPage), arg0, arg1);

    public static string Info_RepositoryLandingPage(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Info_RepositoryLandingPage), culture, arg0, arg1);

    public static string Error_InvalidBatchOperation(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_InvalidBatchOperation), arg0);

    public static string Error_InvalidBatchOperation(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_InvalidBatchOperation), culture, arg0);

    public static string Error_VersionTooLong() => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_VersionTooLong));

    public static string Error_VersionTooLong(CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_VersionTooLong), culture);

    public static string Error_MavenInvalidFilePath(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_MavenInvalidFilePath), arg0);

    public static string Error_MavenInvalidFilePath(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_MavenInvalidFilePath), culture, arg0);

    public static string Error_NonUniqueSnapshotVersion() => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_NonUniqueSnapshotVersion));

    public static string Error_NonUniqueSnapshotVersion(CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_NonUniqueSnapshotVersion), culture);

    public static string Error_MavenInvalidFilename(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_MavenInvalidFilename), arg0);

    public static string Error_MavenInvalidFilename(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_MavenInvalidFilename), culture, arg0);

    public static string Error_UpstreamDoesNotHaveFileRequested(object arg0, object arg1) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_UpstreamDoesNotHaveFileRequested), arg0, arg1);

    public static string Error_UpstreamDoesNotHaveFileRequested(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_UpstreamDoesNotHaveFileRequested), culture, arg0, arg1);
    }

    public static string Error_UpstreamFailure(object arg0, object arg1) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_UpstreamFailure), arg0, arg1);

    public static string Error_UpstreamFailure(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_UpstreamFailure), culture, arg0, arg1);

    public static string Error_UpstreamIngestion_CannotSkipIngestion() => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_UpstreamIngestion_CannotSkipIngestion));

    public static string Error_UpstreamIngestion_CannotSkipIngestion(CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_UpstreamIngestion_CannotSkipIngestion), culture);

    public static string Error_UpstreamReturnedNotFound(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_UpstreamReturnedNotFound), arg0);

    public static string Error_UpstreamReturnedNotFound(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_UpstreamReturnedNotFound), culture, arg0);

    public static string Error_UpstreamReturnedUnauthorized(object arg0) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_UpstreamReturnedUnauthorized), arg0);

    public static string Error_UpstreamReturnedUnauthorized(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_UpstreamReturnedUnauthorized), culture, arg0);

    public static string Error_InvalidCommitEntryType() => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_InvalidCommitEntryType));

    public static string Error_InvalidCommitEntryType(CultureInfo culture) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Get(nameof (Error_InvalidCommitEntryType), culture);

    public static string Error_XmlUnexpectedNode(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_XmlUnexpectedNode), arg0, arg1, arg2);

    public static string Error_XmlUnexpectedNode(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Maven.Server.Resources.Format(nameof (Error_XmlUnexpectedNode), culture, arg0, arg1, arg2);
    }
  }
}
