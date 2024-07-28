// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.GalleryWebApiResources
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  internal static class GalleryWebApiResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (GalleryWebApiResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => GalleryWebApiResources.s_resMgr;

    private static string Get(string resourceName) => GalleryWebApiResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? GalleryWebApiResources.Get(resourceName) : GalleryWebApiResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) GalleryWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? GalleryWebApiResources.GetInt(resourceName) : (int) GalleryWebApiResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) GalleryWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? GalleryWebApiResources.GetBool(resourceName) : (bool) GalleryWebApiResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => GalleryWebApiResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = GalleryWebApiResources.Get(resourceName, culture);
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

    public static string AccountSpecifiedIsInvalid() => GalleryWebApiResources.Get(nameof (AccountSpecifiedIsInvalid));

    public static string AccountSpecifiedIsInvalid(CultureInfo culture) => GalleryWebApiResources.Get(nameof (AccountSpecifiedIsInvalid), culture);

    public static string ExtensionAssetNotFound(object arg0, object arg1, object arg2) => GalleryWebApiResources.Format(nameof (ExtensionAssetNotFound), arg0, arg1, arg2);

    public static string ExtensionAssetNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return GalleryWebApiResources.Format(nameof (ExtensionAssetNotFound), culture, arg0, arg1, arg2);
    }

    public static string ExtensionVersionNotFound(object arg0, object arg1) => GalleryWebApiResources.Format(nameof (ExtensionVersionNotFound), arg0, arg1);

    public static string ExtensionVersionNotFound(object arg0, object arg1, CultureInfo culture) => GalleryWebApiResources.Format(nameof (ExtensionVersionNotFound), culture, arg0, arg1);

    public static string InvalidElementFormat() => GalleryWebApiResources.Get(nameof (InvalidElementFormat));

    public static string InvalidElementFormat(CultureInfo culture) => GalleryWebApiResources.Get(nameof (InvalidElementFormat), culture);

    public static string InvalidElementForToken(object arg0) => GalleryWebApiResources.Format(nameof (InvalidElementForToken), arg0);

    public static string InvalidElementForToken(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (InvalidElementForToken), culture, arg0);

    public static string InvalidPackageFormat(object arg0) => GalleryWebApiResources.Format(nameof (InvalidPackageFormat), arg0);

    public static string InvalidPackageFormat(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (InvalidPackageFormat), culture, arg0);

    public static string InvalidPrivateTokenFormat() => GalleryWebApiResources.Get(nameof (InvalidPrivateTokenFormat));

    public static string InvalidPrivateTokenFormat(CultureInfo culture) => GalleryWebApiResources.Get(nameof (InvalidPrivateTokenFormat), culture);

    public static string InvalidSignedToken(object arg0) => GalleryWebApiResources.Format(nameof (InvalidSignedToken), arg0);

    public static string InvalidSignedToken(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (InvalidSignedToken), culture, arg0);

    public static string MissingContributionJson() => GalleryWebApiResources.Get(nameof (MissingContributionJson));

    public static string MissingContributionJson(CultureInfo culture) => GalleryWebApiResources.Get(nameof (MissingContributionJson), culture);

    public static string MissingPackageManifest() => GalleryWebApiResources.Get(nameof (MissingPackageManifest));

    public static string MissingPackageManifest(CultureInfo culture) => GalleryWebApiResources.Get(nameof (MissingPackageManifest), culture);

    public static string InvalidPackageStream() => GalleryWebApiResources.Get(nameof (InvalidPackageStream));

    public static string InvalidPackageStream(CultureInfo culture) => GalleryWebApiResources.Get(nameof (InvalidPackageStream), culture);

    public static string PrivateTokenHasExpired() => GalleryWebApiResources.Get(nameof (PrivateTokenHasExpired));

    public static string PrivateTokenHasExpired(CultureInfo culture) => GalleryWebApiResources.Get(nameof (PrivateTokenHasExpired), culture);

    public static string ReviewProductVersionMismatch(object arg0, object arg1) => GalleryWebApiResources.Format(nameof (ReviewProductVersionMismatch), arg0, arg1);

    public static string ReviewProductVersionMismatch(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryWebApiResources.Format(nameof (ReviewProductVersionMismatch), culture, arg0, arg1);
    }

    public static string SignedTokensMustHaveClaims() => GalleryWebApiResources.Get(nameof (SignedTokensMustHaveClaims));

    public static string SignedTokensMustHaveClaims(CultureInfo culture) => GalleryWebApiResources.Get(nameof (SignedTokensMustHaveClaims), culture);

    public static string ThereMustBeElementsAndSignature() => GalleryWebApiResources.Get(nameof (ThereMustBeElementsAndSignature));

    public static string ThereMustBeElementsAndSignature(CultureInfo culture) => GalleryWebApiResources.Get(nameof (ThereMustBeElementsAndSignature), culture);

    public static string TheSignatureMustMatchTheElements() => GalleryWebApiResources.Get(nameof (TheSignatureMustMatchTheElements));

    public static string TheSignatureMustMatchTheElements(CultureInfo culture) => GalleryWebApiResources.Get(nameof (TheSignatureMustMatchTheElements), culture);

    public static string UseOfResistrictedPart(object arg0) => GalleryWebApiResources.Format(nameof (UseOfResistrictedPart), arg0);

    public static string UseOfResistrictedPart(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (UseOfResistrictedPart), culture, arg0);

    public static string MustSupplyQueryValues(object arg0) => GalleryWebApiResources.Format(nameof (MustSupplyQueryValues), arg0);

    public static string MustSupplyQueryValues(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (MustSupplyQueryValues), culture, arg0);

    public static string ExtensionDoesNotExist(object arg0) => GalleryWebApiResources.Format(nameof (ExtensionDoesNotExist), arg0);

    public static string ExtensionDoesNotExist(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (ExtensionDoesNotExist), culture, arg0);

    public static string CharacterLimitExceeded(object arg0, object arg1) => GalleryWebApiResources.Format(nameof (CharacterLimitExceeded), arg0, arg1);

    public static string CharacterLimitExceeded(object arg0, object arg1, CultureInfo culture) => GalleryWebApiResources.Format(nameof (CharacterLimitExceeded), culture, arg0, arg1);

    public static string PublisherAlreadyExists() => GalleryWebApiResources.Get(nameof (PublisherAlreadyExists));

    public static string PublisherAlreadyExists(CultureInfo culture) => GalleryWebApiResources.Get(nameof (PublisherAlreadyExists), culture);

    public static string PublisherDoesNotExist() => GalleryWebApiResources.Get(nameof (PublisherDoesNotExist));

    public static string PublisherDoesNotExist(CultureInfo culture) => GalleryWebApiResources.Get(nameof (PublisherDoesNotExist), culture);

    public static string InvalidExtensionVersion(object arg0) => GalleryWebApiResources.Format(nameof (InvalidExtensionVersion), arg0);

    public static string InvalidExtensionVersion(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (InvalidExtensionVersion), culture, arg0);

    public static string UnsupportedTokenFormat() => GalleryWebApiResources.Get(nameof (UnsupportedTokenFormat));

    public static string UnsupportedTokenFormat(CultureInfo culture) => GalleryWebApiResources.Get(nameof (UnsupportedTokenFormat), culture);

    public static string KeyLengthMustBeDivisibleByEight(object arg0) => GalleryWebApiResources.Format(nameof (KeyLengthMustBeDivisibleByEight), arg0);

    public static string KeyLengthMustBeDivisibleByEight(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (KeyLengthMustBeDivisibleByEight), culture, arg0);

    public static string InvalidTokenDate(object arg0) => GalleryWebApiResources.Format(nameof (InvalidTokenDate), arg0);

    public static string InvalidTokenDate(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (InvalidTokenDate), culture, arg0);

    public static string VerifiedPublisherRequired(object arg0, object arg1) => GalleryWebApiResources.Format(nameof (VerifiedPublisherRequired), arg0, arg1);

    public static string VerifiedPublisherRequired(object arg0, object arg1, CultureInfo culture) => GalleryWebApiResources.Format(nameof (VerifiedPublisherRequired), culture, arg0, arg1);

    public static string InvalidAssetType(object arg0) => GalleryWebApiResources.Format(nameof (InvalidAssetType), arg0);

    public static string InvalidAssetType(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (InvalidAssetType), culture, arg0);

    public static string InvalidPublisherName(object arg0) => GalleryWebApiResources.Format(nameof (InvalidPublisherName), arg0);

    public static string InvalidPublisherName(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (InvalidPublisherName), culture, arg0);

    public static string InvalidPublisherDisplayName(object arg0) => GalleryWebApiResources.Format(nameof (InvalidPublisherDisplayName), arg0);

    public static string InvalidPublisherDisplayName(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (InvalidPublisherDisplayName), culture, arg0);

    public static string InvalidExtensionName(object arg0) => GalleryWebApiResources.Format(nameof (InvalidExtensionName), arg0);

    public static string InvalidExtensionName(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (InvalidExtensionName), culture, arg0);

    public static string ReviewAlreadyExistsException() => GalleryWebApiResources.Get(nameof (ReviewAlreadyExistsException));

    public static string ReviewAlreadyExistsException(CultureInfo culture) => GalleryWebApiResources.Get(nameof (ReviewAlreadyExistsException), culture);

    public static string ReviewAlreadyReportedException() => GalleryWebApiResources.Get(nameof (ReviewAlreadyReportedException));

    public static string ReviewAlreadyReportedException(CultureInfo culture) => GalleryWebApiResources.Get(nameof (ReviewAlreadyReportedException), culture);

    public static string ReviewDoesNotExistException(object arg0) => GalleryWebApiResources.Format(nameof (ReviewDoesNotExistException), arg0);

    public static string ReviewDoesNotExistException(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (ReviewDoesNotExistException), culture, arg0);

    public static string AzurePublisherExistsException(object arg0, object arg1) => GalleryWebApiResources.Format(nameof (AzurePublisherExistsException), arg0, arg1);

    public static string AzurePublisherExistsException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryWebApiResources.Format(nameof (AzurePublisherExistsException), culture, arg0, arg1);
    }

    public static string ExtensionInfoDoesNotExist() => GalleryWebApiResources.Get(nameof (ExtensionInfoDoesNotExist));

    public static string ExtensionInfoDoesNotExist(CultureInfo culture) => GalleryWebApiResources.Get(nameof (ExtensionInfoDoesNotExist), culture);

    public static string NoAuthorizationToResource(object arg0) => GalleryWebApiResources.Format(nameof (NoAuthorizationToResource), arg0);

    public static string NoAuthorizationToResource(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (NoAuthorizationToResource), culture, arg0);

    public static string DeprecatedExtensionCantBeUndeprecated() => GalleryWebApiResources.Get(nameof (DeprecatedExtensionCantBeUndeprecated));

    public static string DeprecatedExtensionCantBeUndeprecated(CultureInfo culture) => GalleryWebApiResources.Get(nameof (DeprecatedExtensionCantBeUndeprecated), culture);

    public static string InvalidProductVersion(object arg0) => GalleryWebApiResources.Format(nameof (InvalidProductVersion), arg0);

    public static string InvalidProductVersion(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (InvalidProductVersion), culture, arg0);

    public static string ExtensionSizeExceeded(object arg0, object arg1) => GalleryWebApiResources.Format(nameof (ExtensionSizeExceeded), arg0, arg1);

    public static string ExtensionSizeExceeded(object arg0, object arg1, CultureInfo culture) => GalleryWebApiResources.Format(nameof (ExtensionSizeExceeded), culture, arg0, arg1);

    public static string QnAItemDoesNotExistException(object arg0) => GalleryWebApiResources.Format(nameof (QnAItemDoesNotExistException), arg0);

    public static string QnAItemDoesNotExistException(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (QnAItemDoesNotExistException), culture, arg0);

    public static string InvalidPackageManifest(object arg0) => GalleryWebApiResources.Format(nameof (InvalidPackageManifest), arg0);

    public static string InvalidPackageManifest(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (InvalidPackageManifest), culture, arg0);

    public static string QnAItemAlreadyReportedException() => GalleryWebApiResources.Get(nameof (QnAItemAlreadyReportedException));

    public static string QnAItemAlreadyReportedException(CultureInfo culture) => GalleryWebApiResources.Get(nameof (QnAItemAlreadyReportedException), culture);

    public static string ExtensionAlreadyExists() => GalleryWebApiResources.Get(nameof (ExtensionAlreadyExists));

    public static string ExtensionAlreadyExists(CultureInfo culture) => GalleryWebApiResources.Get(nameof (ExtensionAlreadyExists), culture);

    public static string InvalidCategoryName(object arg0) => GalleryWebApiResources.Format(nameof (InvalidCategoryName), arg0);

    public static string InvalidCategoryName(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (InvalidCategoryName), culture, arg0);

    public static string InvalidExtensionIdentifier(object arg0) => GalleryWebApiResources.Format(nameof (InvalidExtensionIdentifier), arg0);

    public static string InvalidExtensionIdentifier(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (InvalidExtensionIdentifier), culture, arg0);

    public static string InvalidTag(object arg0) => GalleryWebApiResources.Format(nameof (InvalidTag), arg0);

    public static string InvalidTag(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (InvalidTag), culture, arg0);

    public static string PublicExtensionCantBeShared() => GalleryWebApiResources.Get(nameof (PublicExtensionCantBeShared));

    public static string PublicExtensionCantBeShared(CultureInfo culture) => GalleryWebApiResources.Get(nameof (PublicExtensionCantBeShared), culture);

    public static string ExtensionNoVersionFound(object arg0) => GalleryWebApiResources.Format(nameof (ExtensionNoVersionFound), arg0);

    public static string ExtensionNoVersionFound(object arg0, CultureInfo culture) => GalleryWebApiResources.Format(nameof (ExtensionNoVersionFound), culture, arg0);

    public static string PublisherDomainDoesNotExist() => GalleryWebApiResources.Get(nameof (PublisherDomainDoesNotExist));

    public static string PublisherDomainDoesNotExist(CultureInfo culture) => GalleryWebApiResources.Get(nameof (PublisherDomainDoesNotExist), culture);

    public static string TokenNotFoundInDnsTxtRecords() => GalleryWebApiResources.Get(nameof (TokenNotFoundInDnsTxtRecords));

    public static string TokenNotFoundInDnsTxtRecords(CultureInfo culture) => GalleryWebApiResources.Get(nameof (TokenNotFoundInDnsTxtRecords), culture);

    public static string DnsTokenNotVerified() => GalleryWebApiResources.Get(nameof (DnsTokenNotVerified));

    public static string DnsTokenNotVerified(CultureInfo culture) => GalleryWebApiResources.Get(nameof (DnsTokenNotVerified), culture);
  }
}
