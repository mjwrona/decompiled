// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.Resources
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.Feed.WebApi.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.Feed.WebApi.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.Feed.WebApi.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Feed.WebApi.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.Feed.WebApi.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.Feed.WebApi.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Feed.WebApi.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.Feed.WebApi.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(resourceName, culture);
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

    public static string Error_FeedAlreadyExistsMessage(object arg0) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedAlreadyExistsMessage), arg0);

    public static string Error_FeedAlreadyExistsMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedAlreadyExistsMessage), culture, arg0);

    public static string Error_FeedIdNotFoundMessage(object arg0) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedIdNotFoundMessage), arg0);

    public static string Error_FeedIdNotFoundMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedIdNotFoundMessage), culture, arg0);

    public static string Error_FeedLacksPermissions(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedLacksPermissions), arg0, arg1);

    public static string Error_FeedLacksPermissions(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedLacksPermissions), culture, arg0, arg1);

    public static string Error_FeedNotReleased(object arg0) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedNotReleased), arg0);

    public static string Error_FeedNotReleased(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedNotReleased), culture, arg0);

    public static string Error_FeedPermissionsInvalidRoleMessage() => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_FeedPermissionsInvalidRoleMessage));

    public static string Error_FeedPermissionsInvalidRoleMessage(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_FeedPermissionsInvalidRoleMessage), culture);

    public static string Error_FeedPermissionsRemovalFailedMessage() => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_FeedPermissionsRemovalFailedMessage));

    public static string Error_FeedPermissionsRemovalFailedMessage(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_FeedPermissionsRemovalFailedMessage), culture);

    public static string Error_InvalidNuGetPackageIdMessage(object arg0) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_InvalidNuGetPackageIdMessage), arg0);

    public static string Error_InvalidNuGetPackageIdMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_InvalidNuGetPackageIdMessage), culture, arg0);

    public static string Error_InvalidVersionMessage(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_InvalidVersionMessage), arg0, arg1);

    public static string Error_InvalidVersionMessage(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_InvalidVersionMessage), culture, arg0, arg1);

    public static string Error_PackageNotFoundMessage(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageNotFoundMessage), arg0, arg1, arg2);

    public static string Error_PackageNotFoundMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageNotFoundMessage), culture, arg0, arg1, arg2);
    }

    public static string Error_PackagesCollectionWithDifferentIdsMessage() => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_PackagesCollectionWithDifferentIdsMessage));

    public static string Error_PackagesCollectionWithDifferentIdsMessage(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_PackagesCollectionWithDifferentIdsMessage), culture);

    public static string Error_PackageVersionAlreadyExistsMessage(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageVersionAlreadyExistsMessage), arg0, arg1);

    public static string Error_PackageVersionAlreadyExistsMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageVersionAlreadyExistsMessage), culture, arg0, arg1);
    }

    public static string Error_PackageVersionFromTypeAndNameNotFoundMessage(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageVersionFromTypeAndNameNotFoundMessage), arg0, arg1, arg2, arg3);
    }

    public static string Error_PackageVersionFromTypeAndNameNotFoundMessage(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageVersionFromTypeAndNameNotFoundMessage), culture, arg0, arg1, arg2, arg3);
    }

    public static string Error_ProtocolNotSupportedMessage(object arg0) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_ProtocolNotSupportedMessage), arg0);

    public static string Error_ProtocolNotSupportedMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_ProtocolNotSupportedMessage), culture, arg0);

    public static string Error_InvalidPackageVersionPatchMessage() => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_InvalidPackageVersionPatchMessage));

    public static string Error_InvalidPackageVersionPatchMessage(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_InvalidPackageVersionPatchMessage), culture);

    public static string Error_PackageVersionFromIdNotFoundMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageVersionFromIdNotFoundMessage), arg0, arg1, arg2);
    }

    public static string Error_PackageVersionFromIdNotFoundMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageVersionFromIdNotFoundMessage), culture, arg0, arg1, arg2);
    }

    public static string Error_PackageNotFoundByIdInFeedIdMessage(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageNotFoundByIdInFeedIdMessage), arg0, arg1);

    public static string Error_PackageNotFoundByIdInFeedIdMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageNotFoundByIdInFeedIdMessage), culture, arg0, arg1);
    }

    public static string Error_PackageVersionPackageIdAndNormalizedVersionNotFound(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageVersionPackageIdAndNormalizedVersionNotFound), arg0, arg1, arg2);
    }

    public static string Error_PackageVersionPackageIdAndNormalizedVersionNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageVersionPackageIdAndNormalizedVersionNotFound), culture, arg0, arg1, arg2);
    }

    public static string Error_PackageVersionPackageIdAndVersionIdNotFoundMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageVersionPackageIdAndVersionIdNotFoundMessage), arg0, arg1, arg2);
    }

    public static string Error_PackageVersionPackageIdAndVersionIdNotFoundMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageVersionPackageIdAndVersionIdNotFoundMessage), culture, arg0, arg1, arg2);
    }

    public static string Error_NoLatestVersionWasFoundForPackage(object arg0) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_NoLatestVersionWasFoundForPackage), arg0);

    public static string Error_NoLatestVersionWasFoundForPackage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_NoLatestVersionWasFoundForPackage), culture, arg0);

    public static string Error_PackageLimitExceededMessage(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageLimitExceededMessage), arg0, arg1);

    public static string Error_PackageLimitExceededMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageLimitExceededMessage), culture, arg0, arg1);
    }

    public static string Error_InvalidUserInputMessage(object arg0) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_InvalidUserInputMessage), arg0);

    public static string Error_InvalidUserInputMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_InvalidUserInputMessage), culture, arg0);

    public static string Error_InvalidAliasMessage(object arg0) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_InvalidAliasMessage), arg0);

    public static string Error_InvalidAliasMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_InvalidAliasMessage), culture, arg0);

    public static string Error_UnallowedUserIdMessage(object arg0) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_UnallowedUserIdMessage), arg0);

    public static string Error_UnallowedUserIdMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_UnallowedUserIdMessage), culture, arg0);

    public static string Error_PackageVersionPackageIdAndNormalizedVersionDeletedMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageVersionPackageIdAndNormalizedVersionDeletedMessage), arg0, arg1, arg2);
    }

    public static string Error_PackageVersionPackageIdAndNormalizedVersionDeletedMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageVersionPackageIdAndNormalizedVersionDeletedMessage), culture, arg0, arg1, arg2);
    }

    public static string Error_PackageVersionPackageIdAndVersionIdDeletedMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageVersionPackageIdAndVersionIdDeletedMessage), arg0, arg1, arg2);
    }

    public static string Error_PackageVersionPackageIdAndVersionIdDeletedMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageVersionPackageIdAndVersionIdDeletedMessage), culture, arg0, arg1, arg2);
    }

    public static string Error_KnownDBStateIsNoLongerValid() => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_KnownDBStateIsNoLongerValid));

    public static string Error_KnownDBStateIsNoLongerValid(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_KnownDBStateIsNoLongerValid), culture);

    public static string Error_UnknownDatabaseErrorOcurred(object arg0) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_UnknownDatabaseErrorOcurred), arg0);

    public static string Error_UnknownDatabaseErrorOcurred(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_UnknownDatabaseErrorOcurred), culture, arg0);

    public static string Error_FeedViewAlreadyExistsMessage(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedViewAlreadyExistsMessage), arg0, arg1);

    public static string Error_FeedViewAlreadyExistsMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedViewAlreadyExistsMessage), culture, arg0, arg1);
    }

    public static string Error_FeedViewNotFoundMessage(object arg0) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedViewNotFoundMessage), arg0);

    public static string Error_FeedViewNotFoundMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedViewNotFoundMessage), culture, arg0);

    public static string Error_FeedViewIdNotFoundMessage(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedViewIdNotFoundMessage), arg0, arg1);

    public static string Error_FeedViewIdNotFoundMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedViewIdNotFoundMessage), culture, arg0, arg1);
    }

    public static string Error_FeedViewNotReleased(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedViewNotReleased), arg0, arg1);

    public static string Error_FeedViewNotReleased(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedViewNotReleased), culture, arg0, arg1);

    public static string Error_LabelNameNotFoundMessage(object arg0) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_LabelNameNotFoundMessage), arg0);

    public static string Error_LabelNameNotFoundMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_LabelNameNotFoundMessage), culture, arg0);

    public static string Error_FeedIsReadOnlyMessage(object arg0) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedIsReadOnlyMessage), arg0);

    public static string Error_FeedIsReadOnlyMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_FeedIsReadOnlyMessage), culture, arg0);

    public static string Error_CannotUseViewNotation() => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_CannotUseViewNotation));

    public static string Error_CannotUseViewNotation(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_CannotUseViewNotation), culture);

    public static string Error_InvalidPackageIdMessage(object arg0) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_InvalidPackageIdMessage), arg0);

    public static string Error_InvalidPackageIdMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_InvalidPackageIdMessage), culture, arg0);

    public static string Error_ViewPermissionsInvalidRoleMessage() => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_ViewPermissionsInvalidRoleMessage));

    public static string Error_ViewPermissionsInvalidRoleMessage(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_ViewPermissionsInvalidRoleMessage), culture);

    public static string Error_PackageVersionPackageIdAndVersionIdNotFoundInRecycleBinMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageVersionPackageIdAndVersionIdNotFoundInRecycleBinMessage), arg0, arg1, arg2);
    }

    public static string Error_PackageVersionPackageIdAndVersionIdNotFoundInRecycleBinMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Format(nameof (Error_PackageVersionPackageIdAndVersionIdNotFoundInRecycleBinMessage), culture, arg0, arg1, arg2);
    }

    public static string Error_GenericDatabaseFailure() => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_GenericDatabaseFailure));

    public static string Error_GenericDatabaseFailure(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_GenericDatabaseFailure), culture);

    public static string Error_NullSecurityNamespaceMessage() => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_NullSecurityNamespaceMessage));

    public static string Error_NullSecurityNamespaceMessage(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_NullSecurityNamespaceMessage), culture);

    public static string Error_NullIdentityMessage() => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_NullIdentityMessage));

    public static string Error_NullIdentityMessage(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.WebApi.Resources.Get(nameof (Error_NullIdentityMessage), culture);
  }
}
