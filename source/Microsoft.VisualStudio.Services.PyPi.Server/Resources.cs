// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Resources
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.PyPi.Server
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.PyPi.Server.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.PyPi.Server.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.PyPi.Server.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.PyPi.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.PyPi.Server.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.PyPi.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.PyPi.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.PyPi.Server.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.PyPi.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(resourceName, culture);
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

    public static string Error_InvalidContentDigest(object arg0, object arg1) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidContentDigest), arg0, arg1);

    public static string Error_InvalidContentDigest(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidContentDigest), culture, arg0, arg1);

    public static string Error_MissingContentValidationDigests() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_MissingContentValidationDigests));

    public static string Error_MissingContentValidationDigests(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_MissingContentValidationDigests), culture);

    public static string Error_InvalidSha256Digest() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_InvalidSha256Digest));

    public static string Error_InvalidSha256Digest(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_InvalidSha256Digest), culture);

    public static string Error_InvalidBlake2Digest() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_InvalidBlake2Digest));

    public static string Error_InvalidBlake2Digest(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_InvalidBlake2Digest), culture);

    public static string Error_InvalidPyPiPackageName() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_InvalidPyPiPackageName));

    public static string Error_InvalidPyPiPackageName(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_InvalidPyPiPackageName), culture);

    public static string Error_InvalidPyPiPackageVersion() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_InvalidPyPiPackageVersion));

    public static string Error_InvalidPyPiPackageVersion(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_InvalidPyPiPackageVersion), culture);

    public static string Error_InvalidEmailAddress(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidEmailAddress), arg0);

    public static string Error_InvalidEmailAddress(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidEmailAddress), culture, arg0);

    public static string Error_InvalidUrl(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidUrl), arg0);

    public static string Error_InvalidUrl(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidUrl), culture, arg0);

    public static string Error_InvalidProjectUrl(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidProjectUrl), arg0);

    public static string Error_InvalidProjectUrl(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidProjectUrl), culture, arg0);

    public static string Error_InvalidProjectUrlLabel(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidProjectUrlLabel), arg0);

    public static string Error_InvalidProjectUrlLabel(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidProjectUrlLabel), culture, arg0);

    public static string Error_InvalidRequiresExternal(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidRequiresExternal), arg0);

    public static string Error_InvalidRequiresExternal(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidRequiresExternal), culture, arg0);

    public static string Error_SortableVersionExceedsMaximumLength(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_SortableVersionExceedsMaximumLength), arg0);

    public static string Error_SortableVersionExceedsMaximumLength(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_SortableVersionExceedsMaximumLength), culture, arg0);

    public static string Error_VersionHasLeadingOrTrailingWhitespace() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_VersionHasLeadingOrTrailingWhitespace));

    public static string Error_VersionHasLeadingOrTrailingWhitespace(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_VersionHasLeadingOrTrailingWhitespace), culture);

    public static string Error_MissingMultipartFormData() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_MissingMultipartFormData));

    public static string Error_MissingMultipartFormData(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_MissingMultipartFormData), culture);

    public static string Error_MissingMultipartFileData() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_MissingMultipartFileData));

    public static string Error_MissingMultipartFileData(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_MissingMultipartFileData), culture);

    public static string Error_ContentMediaTypeNotApplicationOctetStream() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_ContentMediaTypeNotApplicationOctetStream));

    public static string Error_ContentMediaTypeNotApplicationOctetStream(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_ContentMediaTypeNotApplicationOctetStream), culture);

    public static string Error_InvalidFilename(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidFilename), arg0);

    public static string Error_InvalidFilename(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidFilename), culture, arg0);

    public static string Error_InvalidPlatformPartOfWheelFilename(object arg0, object arg1) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidPlatformPartOfWheelFilename), arg0, arg1);

    public static string Error_InvalidPlatformPartOfWheelFilename(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidPlatformPartOfWheelFilename), culture, arg0, arg1);
    }

    public static string Error_MissingContentFilename() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_MissingContentFilename));

    public static string Error_MissingContentFilename(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_MissingContentFilename), culture);

    public static string Error_MissingGpgSignatureFilename() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_MissingGpgSignatureFilename));

    public static string Error_MissingGpgSignatureFilename(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_MissingGpgSignatureFilename), culture);

    public static string Error_MissingIngestionMetadata(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_MissingIngestionMetadata), arg0);

    public static string Error_MissingIngestionMetadata(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_MissingIngestionMetadata), culture, arg0);

    public static string Error_InvalidSignatureTooLong(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidSignatureTooLong), arg0);

    public static string Error_InvalidSignatureTooLong(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidSignatureTooLong), culture, arg0);

    public static string Error_MissingMetadataFile(object arg0, object arg1) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_MissingMetadataFile), arg0, arg1);

    public static string Error_MissingMetadataFile(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_MissingMetadataFile), culture, arg0, arg1);

    public static string Error_MissingContentPart() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_MissingContentPart));

    public static string Error_MissingContentPart(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_MissingContentPart), culture);

    public static string Error_NonIngestableDistributionType(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_NonIngestableDistributionType), arg0);

    public static string Error_NonIngestableDistributionType(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_NonIngestableDistributionType), culture, arg0);

    public static string Error_NonIngestableExtension(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_NonIngestableExtension), arg0, arg1, arg2);

    public static string Error_NonIngestableExtension(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_NonIngestableExtension), culture, arg0, arg1, arg2);
    }

    public static string Error_OnlyOneSourceDistribution(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_OnlyOneSourceDistribution), arg0);

    public static string Error_OnlyOneSourceDistribution(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_OnlyOneSourceDistribution), culture, arg0);

    public static string Error_UnsupportedProtocolVersion(object arg0, object arg1) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UnsupportedProtocolVersion), arg0, arg1);

    public static string Error_UnsupportedProtocolVersion(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UnsupportedProtocolVersion), culture, arg0, arg1);
    }

    public static string Error_UnsupportedMetadataVersion(object arg0, object arg1) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UnsupportedMetadataVersion), arg0, arg1);

    public static string Error_UnsupportedMetadataVersion(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UnsupportedMetadataVersion), culture, arg0, arg1);
    }

    public static string Error_InvalidPythonVersionForSourceDist(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidPythonVersionForSourceDist), arg0);

    public static string Error_InvalidPythonVersionForSourceDist(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InvalidPythonVersionForSourceDist), culture, arg0);

    public static string Error_InvalidSummary() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_InvalidSummary));

    public static string Error_InvalidSummary(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_InvalidSummary), culture);

    public static string Error_InvalidDescriptionContentType() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_InvalidDescriptionContentType));

    public static string Error_InvalidDescriptionContentType(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_InvalidDescriptionContentType), culture);

    public static string Error_UnsupportedDescriptionContentType(object arg0, object arg1) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UnsupportedDescriptionContentType), arg0, arg1);

    public static string Error_UnsupportedDescriptionContentType(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UnsupportedDescriptionContentType), culture, arg0, arg1);
    }

    public static string Error_UnsupportedDescriptionContentTypeCharset(object arg0, object arg1) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UnsupportedDescriptionContentTypeCharset), arg0, arg1);

    public static string Error_UnsupportedDescriptionContentTypeCharset(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UnsupportedDescriptionContentTypeCharset), culture, arg0, arg1);
    }

    public static string Error_UnsupportedMarkdownVariant(object arg0, object arg1) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UnsupportedMarkdownVariant), arg0, arg1);

    public static string Error_UnsupportedMarkdownVariant(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UnsupportedMarkdownVariant), culture, arg0, arg1);
    }

    public static string Error_DescriptionExceedsMaximumLength(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_DescriptionExceedsMaximumLength), arg0);

    public static string Error_DescriptionExceedsMaximumLength(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_DescriptionExceedsMaximumLength), culture, arg0);

    public static string Error_RequirementParseError(object arg0, object arg1) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_RequirementParseError), arg0, arg1);

    public static string Error_RequirementParseError(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_RequirementParseError), culture, arg0, arg1);

    public static string Error_UnknownMarkerVariable(object arg0, object arg1) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UnknownMarkerVariable), arg0, arg1);

    public static string Error_UnknownMarkerVariable(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UnknownMarkerVariable), culture, arg0, arg1);

    public static string Error_PackageContainsInvalidRequirement(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_PackageContainsInvalidRequirement), arg0);

    public static string Error_PackageContainsInvalidRequirement(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_PackageContainsInvalidRequirement), culture, arg0);

    public static string Error_RequirementParseTookTooLong() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_RequirementParseTookTooLong));

    public static string Error_RequirementParseTookTooLong(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_RequirementParseTookTooLong), culture);

    public static string Error_EmailParseTookTooLong(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_EmailParseTookTooLong), arg0);

    public static string Error_EmailParseTookTooLong(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_EmailParseTookTooLong), culture, arg0);

    public static string Error_UrlsNotAllowedInRequirementSpecs(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UrlsNotAllowedInRequirementSpecs), arg0);

    public static string Error_UrlsNotAllowedInRequirementSpecs(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UrlsNotAllowedInRequirementSpecs), culture, arg0);

    public static string Error_ExtrasNotAllowedInDistributionSpecs(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_ExtrasNotAllowedInDistributionSpecs), arg0);

    public static string Error_ExtrasNotAllowedInDistributionSpecs(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_ExtrasNotAllowedInDistributionSpecs), culture, arg0);

    public static string Error_UpstreamFailure(object arg0, object arg1) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UpstreamFailure), arg0, arg1);

    public static string Error_UpstreamFailure(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UpstreamFailure), culture, arg0, arg1);

    public static string Error_UpstreamReturnedNotFound(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UpstreamReturnedNotFound), arg0);

    public static string Error_UpstreamReturnedNotFound(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UpstreamReturnedNotFound), culture, arg0);

    public static string Error_UpstreamDoesNotHaveFileRequested(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UpstreamDoesNotHaveFileRequested), arg0, arg1, arg2);
    }

    public static string Error_UpstreamDoesNotHaveFileRequested(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_UpstreamDoesNotHaveFileRequested), culture, arg0, arg1, arg2);
    }

    public static string Error_InternalUpstreamDoesNotHaveFileRequested(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InternalUpstreamDoesNotHaveFileRequested), arg0, arg1, arg2);
    }

    public static string Error_InternalUpstreamDoesNotHaveFileRequested(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_InternalUpstreamDoesNotHaveFileRequested), culture, arg0, arg1, arg2);
    }

    public static string Error_RequirementParseErrorUnexpectedEOF(object arg0) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_RequirementParseErrorUnexpectedEOF), arg0);

    public static string Error_RequirementParseErrorUnexpectedEOF(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Format(nameof (Error_RequirementParseErrorUnexpectedEOF), culture, arg0);

    public static string Error_UpstreamIngestion_CannotSkipIngestion() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_UpstreamIngestion_CannotSkipIngestion));

    public static string Error_UpstreamIngestion_CannotSkipIngestion(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_UpstreamIngestion_CannotSkipIngestion), culture);

    public static string Error_ApiEndpointNotSupported() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_ApiEndpointNotSupported));

    public static string Error_ApiEndpointNotSupported(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_ApiEndpointNotSupported), culture);

    public static string Error_InvalidSignatureNotArmored() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_InvalidSignatureNotArmored));

    public static string Error_InvalidSignatureNotArmored(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_InvalidSignatureNotArmored), culture);

    public static string Error_NotSupportedOnPrem() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_NotSupportedOnPrem));

    public static string Error_NotSupportedOnPrem(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_NotSupportedOnPrem), culture);

    public static string Error_PushRequestMissingBlob() => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_PushRequestMissingBlob));

    public static string Error_PushRequestMissingBlob(CultureInfo culture) => Microsoft.VisualStudio.Services.PyPi.Server.Resources.Get(nameof (Error_PushRequestMissingBlob), culture);
  }
}
