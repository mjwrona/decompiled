// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Resources
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.Feed.Server.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.Feed.Server.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.Feed.Server.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.Feed.Server.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.Feed.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Feed.Server.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.Feed.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.Feed.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Feed.Server.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.Feed.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(resourceName, culture);
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

    public static string Error_CannotHaveBothNormalizedNameAndDirectUpstreamSourceId() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_CannotHaveBothNormalizedNameAndDirectUpstreamSourceId));

    public static string Error_CannotHaveBothNormalizedNameAndDirectUpstreamSourceId(
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_CannotHaveBothNormalizedNameAndDirectUpstreamSourceId), culture);
    }

    public static string Error_CannotHaveBothNormalizedNameAndNameQuery() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_CannotHaveBothNormalizedNameAndNameQuery));

    public static string Error_CannotHaveBothNormalizedNameAndNameQuery(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_CannotHaveBothNormalizedNameAndNameQuery), culture);

    public static string Error_DescriptionAndAllVersionsIncompatible() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_DescriptionAndAllVersionsIncompatible));

    public static string Error_DescriptionAndAllVersionsIncompatible(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_DescriptionAndAllVersionsIncompatible), culture);

    public static string Error_FailedToLoadPackagingProtocolExtension(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FailedToLoadPackagingProtocolExtension), arg0);

    public static string Error_FailedToLoadPackagingProtocolExtension(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FailedToLoadPackagingProtocolExtension), culture, arg0);
    }

    public static string Error_FeedDescriptionExcedesMaximumLength(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedDescriptionExcedesMaximumLength), arg0, arg1);

    public static string Error_FeedDescriptionExcedesMaximumLength(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedDescriptionExcedesMaximumLength), culture, arg0, arg1);
    }

    public static string Error_FeedIdMismatch(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedIdMismatch), arg0);

    public static string Error_FeedIdMismatch(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedIdMismatch), culture, arg0);

    public static string Error_FeedInvalidOrMissing() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_FeedInvalidOrMissing));

    public static string Error_FeedInvalidOrMissing(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_FeedInvalidOrMissing), culture);

    public static string Error_FeedMustNotHaveUpstreamV2Capability() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_FeedMustNotHaveUpstreamV2Capability));

    public static string Error_FeedMustNotHaveUpstreamV2Capability(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_FeedMustNotHaveUpstreamV2Capability), culture);

    public static string Error_FeedNameContainsWhitespace(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedNameContainsWhitespace), arg0);

    public static string Error_FeedNameContainsWhitespace(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedNameContainsWhitespace), culture, arg0);

    public static string Error_FeedNameExceedsMaximumLength(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedNameExceedsMaximumLength), arg0, arg1);

    public static string Error_FeedNameExceedsMaximumLength(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedNameExceedsMaximumLength), culture, arg0, arg1);
    }

    public static string Error_FeedNameIsGuid(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedNameIsGuid), arg0);

    public static string Error_FeedNameIsGuid(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedNameIsGuid), culture, arg0);

    public static string Error_FeedNameIsInvalid(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedNameIsInvalid), arg0, arg1);

    public static string Error_FeedNameIsInvalid(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedNameIsInvalid), culture, arg0, arg1);

    public static string Error_FeedNameIsReserved(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedNameIsReserved), arg0);

    public static string Error_FeedNameIsReserved(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedNameIsReserved), culture, arg0);

    public static string Error_FeedUpstreamConflictsEnabledWithoutUpstreamEnabled(
      object arg0,
      object arg1)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedUpstreamConflictsEnabledWithoutUpstreamEnabled), arg0, arg1);
    }

    public static string Error_FeedUpstreamConflictsEnabledWithoutUpstreamEnabled(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedUpstreamConflictsEnabledWithoutUpstreamEnabled), culture, arg0, arg1);
    }

    public static string Error_FeedViewNameContainsWhitespace(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedViewNameContainsWhitespace), arg0);

    public static string Error_FeedViewNameContainsWhitespace(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedViewNameContainsWhitespace), culture, arg0);

    public static string Error_FeedViewNameIsInvalid(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedViewNameIsInvalid), arg0, arg1);

    public static string Error_FeedViewNameIsInvalid(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedViewNameIsInvalid), culture, arg0, arg1);

    public static string Error_FeedViewNameIsInvalidReserved(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedViewNameIsInvalidReserved), arg0, arg1);

    public static string Error_FeedViewNameIsInvalidReserved(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedViewNameIsInvalidReserved), culture, arg0, arg1);
    }

    public static string Error_ImplicitViewCannotBeCreated() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ImplicitViewCannotBeCreated));

    public static string Error_ImplicitViewCannotBeCreated(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ImplicitViewCannotBeCreated), culture);

    public static string Error_ImplicitViewCannotBeDeleted() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ImplicitViewCannotBeDeleted));

    public static string Error_ImplicitViewCannotBeDeleted(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ImplicitViewCannotBeDeleted), culture);

    public static string Error_ImplicitViewCannotBeRenamed() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ImplicitViewCannotBeRenamed));

    public static string Error_ImplicitViewCannotBeRenamed(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ImplicitViewCannotBeRenamed), culture);

    public static string Error_IncompleteInternalUpstreamSourceFields() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_IncompleteInternalUpstreamSourceFields));

    public static string Error_IncompleteInternalUpstreamSourceFields(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_IncompleteInternalUpstreamSourceFields), culture);

    public static string Error_InvalidUpstream(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InvalidUpstream), arg0);

    public static string Error_InvalidUpstream(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InvalidUpstream), culture, arg0);

    public static string Error_CrossCollectionUPackUpstreamInvalid(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_CrossCollectionUPackUpstreamInvalid), arg0);

    public static string Error_CrossCollectionUPackUpstreamInvalid(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_CrossCollectionUPackUpstreamInvalid), culture, arg0);

    public static string Error_InternalUpstreamConfigurationShouldOnlyReceiveOneLocationSet() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_InternalUpstreamConfigurationShouldOnlyReceiveOneLocationSet));

    public static string Error_InternalUpstreamConfigurationShouldOnlyReceiveOneLocationSet(
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_InternalUpstreamConfigurationShouldOnlyReceiveOneLocationSet), culture);
    }

    public static string Error_InternalUpstreamContainsIncorrectFeed(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InternalUpstreamContainsIncorrectFeed), arg0);

    public static string Error_InternalUpstreamContainsIncorrectFeed(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InternalUpstreamContainsIncorrectFeed), culture, arg0);
    }

    public static string Error_InternalUpstreamContainsIncorrectScheme(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InternalUpstreamContainsIncorrectScheme), arg0);

    public static string Error_InternalUpstreamContainsIncorrectScheme(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InternalUpstreamContainsIncorrectScheme), culture, arg0);
    }

    public static string Error_InternalUpstreamLocationFormatIsIncorrect(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InternalUpstreamLocationFormatIsIncorrect), arg0);

    public static string Error_InternalUpstreamLocationFormatIsIncorrect(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InternalUpstreamLocationFormatIsIncorrect), culture, arg0);
    }

    public static string Error_InternalUpstreamsNotEnabled() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_InternalUpstreamsNotEnabled));

    public static string Error_InternalUpstreamsNotEnabled(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_InternalUpstreamsNotEnabled), culture);

    public static string Error_InvalidUpstreamProtocolValue(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InvalidUpstreamProtocolValue), arg0);

    public static string Error_InvalidUpstreamProtocolValue(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InvalidUpstreamProtocolValue), culture, arg0);

    public static string Error_InvalidUpstreamSourceLocation(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InvalidUpstreamSourceLocation), arg0, arg1);

    public static string Error_InvalidUpstreamSourceLocation(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InvalidUpstreamSourceLocation), culture, arg0, arg1);
    }

    public static string Error_InvalidUpstreamSourceType(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InvalidUpstreamSourceType), arg0);

    public static string Error_InvalidUpstreamSourceType(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InvalidUpstreamSourceType), culture, arg0);

    public static string Error_NameContainsIllegalCharacters() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_NameContainsIllegalCharacters));

    public static string Error_NameContainsIllegalCharacters(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_NameContainsIllegalCharacters), culture);

    public static string Error_NameInvalid() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_NameInvalid));

    public static string Error_NameInvalid(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_NameInvalid), culture);

    public static string Error_NameLengthInvalid(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_NameLengthInvalid), arg0);

    public static string Error_NameLengthInvalid(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_NameLengthInvalid), culture, arg0);

    public static string Error_NamePrefixInvalid() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_NamePrefixInvalid));

    public static string Error_NamePrefixInvalid(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_NamePrefixInvalid), culture);

    public static string Error_NameReserved() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_NameReserved));

    public static string Error_NameReserved(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_NameReserved), culture);

    public static string Error_NameSuffixInvalid() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_NameSuffixInvalid));

    public static string Error_NameSuffixInvalid(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_NameSuffixInvalid), culture);

    public static string Error_NormalizedMatchRequiresProtocol() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_NormalizedMatchRequiresProtocol));

    public static string Error_NormalizedMatchRequiresProtocol(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_NormalizedMatchRequiresProtocol), culture);

    public static string Error_NormalizedNameNotSupported() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_NormalizedNameNotSupported));

    public static string Error_NormalizedNameNotSupported(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_NormalizedNameNotSupported), culture);

    public static string Error_NullParameter(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_NullParameter), arg0);

    public static string Error_NullParameter(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_NullParameter), culture, arg0);

    public static string Error_PackageBadgeNotFound() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PackageBadgeNotFound));

    public static string Error_PackageBadgeNotFound(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PackageBadgeNotFound), culture);

    public static string Error_PackageRetentionCountUnsorted(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_PackageRetentionCountUnsorted), arg0);

    public static string Error_PackageRetentionCountUnsorted(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_PackageRetentionCountUnsorted), culture, arg0);

    public static string Error_PackageRetentionInvalidPolicy() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PackageRetentionInvalidPolicy));

    public static string Error_PackageRetentionInvalidPolicy(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PackageRetentionInvalidPolicy), culture);

    public static string Error_PackagingOperationNotFound(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_PackagingOperationNotFound), arg0);

    public static string Error_PackagingOperationNotFound(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_PackagingOperationNotFound), culture, arg0);

    public static string Error_RecycleBinUpdateOnlySupportsRestoreToFeed() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_RecycleBinUpdateOnlySupportsRestoreToFeed));

    public static string Error_RecycleBinUpdateOnlySupportsRestoreToFeed(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_RecycleBinUpdateOnlySupportsRestoreToFeed), culture);

    public static string Error_ReservedName() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ReservedName));

    public static string Error_ReservedName(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ReservedName), culture);

    public static string Error_TooManyUpstreamSourcesForProtocol(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_TooManyUpstreamSourcesForProtocol), arg0, arg1);

    public static string Error_TooManyUpstreamSourcesForProtocol(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_TooManyUpstreamSourcesForProtocol), culture, arg0, arg1);
    }

    public static string Error_UpstreamConfigurationFeedLocatorNotAllowedOnPrem() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_UpstreamConfigurationFeedLocatorNotAllowedOnPrem));

    public static string Error_UpstreamConfigurationFeedLocatorNotAllowedOnPrem(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_UpstreamConfigurationFeedLocatorNotAllowedOnPrem), culture);

    public static string Error_UpstreamSourceIdIsInvalid(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceIdIsInvalid), arg0);

    public static string Error_UpstreamSourceIdIsInvalid(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceIdIsInvalid), culture, arg0);

    public static string Error_UpstreamSourceLocationNotDistinct(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceLocationNotDistinct), arg0);

    public static string Error_UpstreamSourceLocationNotDistinct(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceLocationNotDistinct), culture, arg0);

    public static string Error_UpstreamSourceLocationNotParseable(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceLocationNotParseable), arg0);

    public static string Error_UpstreamSourceLocationNotParseable(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceLocationNotParseable), culture, arg0);

    public static string Error_UpstreamSourceNameSurroundingWhitespace(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceNameSurroundingWhitespace), arg0);

    public static string Error_UpstreamSourceNameSurroundingWhitespace(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceNameSurroundingWhitespace), culture, arg0);
    }

    public static string Error_UpstreamSourceNameIsInvalid(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceNameIsInvalid), arg0, arg1);

    public static string Error_UpstreamSourceNameIsInvalid(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceNameIsInvalid), culture, arg0, arg1);
    }

    public static string Error_UpstreamSourceNameLocationMismatch(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceNameLocationMismatch), arg0, arg1);

    public static string Error_UpstreamSourceNameLocationMismatch(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceNameLocationMismatch), culture, arg0, arg1);
    }

    public static string Error_UpstreamSourceNameNotDistinct(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceNameNotDistinct), arg0);

    public static string Error_UpstreamSourceNameNotDistinct(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceNameNotDistinct), culture, arg0);

    public static string Error_UpstreamSourceNameWrongLength(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceNameWrongLength), arg0, arg1);

    public static string Error_UpstreamSourceNameWrongLength(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceNameWrongLength), culture, arg0, arg1);
    }

    public static string Error_UpstreamSourceNotAllowed() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_UpstreamSourceNotAllowed));

    public static string Error_UpstreamSourceNotAllowed(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_UpstreamSourceNotAllowed), culture);

    public static string Error_UpstreamToSameFeedIsDisallowed() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_UpstreamToSameFeedIsDisallowed));

    public static string Error_UpstreamToSameFeedIsDisallowed(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_UpstreamToSameFeedIsDisallowed), culture);

    public static string PackageNotificationsChangeType() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsChangeType));

    public static string PackageNotificationsChangeType(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsChangeType), culture);

    public static string PackageNotificationsChangeTypeDelete() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsChangeTypeDelete));

    public static string PackageNotificationsChangeTypeDelete(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsChangeTypeDelete), culture);

    public static string PackageNotificationsChangeTypePromote() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsChangeTypePromote));

    public static string PackageNotificationsChangeTypePromote(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsChangeTypePromote), culture);

    public static string PackageNotificationsChangeTypePublish() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsChangeTypePublish));

    public static string PackageNotificationsChangeTypePublish(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsChangeTypePublish), culture);

    public static string PackageNotificationsChangeTypeRelist() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsChangeTypeRelist));

    public static string PackageNotificationsChangeTypeRelist(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsChangeTypeRelist), culture);

    public static string PackageNotificationsChangeTypeUnlist() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsChangeTypeUnlist));

    public static string PackageNotificationsChangeTypeUnlist(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsChangeTypeUnlist), culture);

    public static string PackageNotificationsFeed() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsFeed));

    public static string PackageNotificationsFeed(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsFeed), culture);

    public static string PackageNotificationsPackageName() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsPackageName));

    public static string PackageNotificationsPackageName(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsPackageName), culture);

    public static string PackageNotificationsPackageVersion() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsPackageVersion));

    public static string PackageNotificationsPackageVersion(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (PackageNotificationsPackageVersion), culture);

    public static string UpgradedFeedBanner() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (UpgradedFeedBanner));

    public static string UpgradedFeedBanner(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (UpgradedFeedBanner), culture);

    public static string UpgradedFeedBannerLink() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (UpgradedFeedBannerLink));

    public static string UpgradedFeedBannerLink(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (UpgradedFeedBannerLink), culture);

    public static string Error_InternalCrossCollectionUpstreamsNotEnabled() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_InternalCrossCollectionUpstreamsNotEnabled));

    public static string Error_InternalCrossCollectionUpstreamsNotEnabled(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_InternalCrossCollectionUpstreamsNotEnabled), culture);

    public static string Error_InternalUpstreamsNotEnabledForNonAad(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InternalUpstreamsNotEnabledForNonAad), arg0);

    public static string Error_InternalUpstreamsNotEnabledForNonAad(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InternalUpstreamsNotEnabledForNonAad), culture, arg0);
    }

    public static string ManuallyUpgradeFeedBanner() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (ManuallyUpgradeFeedBanner));

    public static string ManuallyUpgradeFeedBanner(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (ManuallyUpgradeFeedBanner), culture);

    public static string ManuallyUpgradeFeedBannerLink() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (ManuallyUpgradeFeedBannerLink));

    public static string ManuallyUpgradeFeedBannerLink(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (ManuallyUpgradeFeedBannerLink), culture);

    public static string Error_UpstreamSourceLocationInvalid(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceLocationInvalid), arg0);

    public static string Error_UpstreamSourceLocationInvalid(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceLocationInvalid), culture, arg0);

    public static string Error_UpstreamSourceLocationUnavailable(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceLocationUnavailable), arg0);

    public static string Error_UpstreamSourceLocationUnavailable(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceLocationUnavailable), culture, arg0);

    public static string Error_UpstreamSourceLocationNotHttp(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceLocationNotHttp), arg0);

    public static string Error_UpstreamSourceLocationNotHttp(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceLocationNotHttp), culture, arg0);

    public static string UpgradeNotSupported() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (UpgradeNotSupported));

    public static string UpgradeNotSupported(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (UpgradeNotSupported), culture);

    public static string FeedIsBeingUpgraded() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (FeedIsBeingUpgraded));

    public static string FeedIsBeingUpgraded(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (FeedIsBeingUpgraded), culture);

    public static string Error_BatchDataIsRequired() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_BatchDataIsRequired));

    public static string Error_BatchDataIsRequired(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_BatchDataIsRequired), culture);

    public static string Error_UnknownBatchOperation() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_UnknownBatchOperation));

    public static string Error_UnknownBatchOperation(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_UnknownBatchOperation), culture);

    public static string Error_InternalUpstreamsHostCouldNotBeResolved(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InternalUpstreamsHostCouldNotBeResolved), arg0);

    public static string Error_InternalUpstreamsHostCouldNotBeResolved(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InternalUpstreamsHostCouldNotBeResolved), culture, arg0);
    }

    public static string Error_PackageVersionProvenanceNotFound(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_PackageVersionProvenanceNotFound), arg0, arg1, arg2);
    }

    public static string Error_PackageVersionProvenanceNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_PackageVersionProvenanceNotFound), culture, arg0, arg1, arg2);
    }

    public static string Error_ProvenanceInvalidKey() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ProvenanceInvalidKey));

    public static string Error_ProvenanceInvalidKey(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ProvenanceInvalidKey), culture);

    public static string Error_ProvenanceTooManyKeys() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ProvenanceTooManyKeys));

    public static string Error_ProvenanceTooManyKeys(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ProvenanceTooManyKeys), culture);

    public static string Error_ProvenanceSourceInvalid() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ProvenanceSourceInvalid));

    public static string Error_ProvenanceSourceInvalid(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ProvenanceSourceInvalid), culture);

    public static string Error_ProvenanceValueTooLong() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ProvenanceValueTooLong));

    public static string Error_ProvenanceValueTooLong(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ProvenanceValueTooLong), culture);

    public static string Error_ProjectScopedFeedsNotEnabled() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ProjectScopedFeedsNotEnabled));

    public static string Error_ProjectScopedFeedsNotEnabled(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ProjectScopedFeedsNotEnabled), culture);

    public static string Error_ProjectScopedFeedsNotEnabledForThisResource() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ProjectScopedFeedsNotEnabledForThisResource));

    public static string Error_ProjectScopedFeedsNotEnabledForThisResource(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ProjectScopedFeedsNotEnabledForThisResource), culture);

    public static string Error_UpstreamSourceNameContainsInvalidWhitespace(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceNameContainsInvalidWhitespace), arg0);

    public static string Error_UpstreamSourceNameContainsInvalidWhitespace(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_UpstreamSourceNameContainsInvalidWhitespace), culture, arg0);
    }

    public static string Error_UpstreamFeedNotAllowedForProjectScopedFeed() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_UpstreamFeedNotAllowedForProjectScopedFeed));

    public static string Error_UpstreamFeedNotAllowedForProjectScopedFeed(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_UpstreamFeedNotAllowedForProjectScopedFeed), culture);

    public static string Error_UpstreamFeedNotAllowedForPublicFeed() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_UpstreamFeedNotAllowedForPublicFeed));

    public static string Error_UpstreamFeedNotAllowedForPublicFeed(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_UpstreamFeedNotAllowedForPublicFeed), culture);

    public static string Error_NonPrivateViewsNotSupportedForProjectScopedFeeds() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_NonPrivateViewsNotSupportedForProjectScopedFeeds));

    public static string Error_NonPrivateViewsNotSupportedForProjectScopedFeeds(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_NonPrivateViewsNotSupportedForProjectScopedFeeds), culture);

    public static string Error_MismatchedFeedProjectId(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_MismatchedFeedProjectId), arg0, arg1);

    public static string Error_MismatchedFeedProjectId(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_MismatchedFeedProjectId), culture, arg0, arg1);
    }

    public static string Error_RecycleBinPatchOnlySupportsRestoreFeed() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_RecycleBinPatchOnlySupportsRestoreFeed));

    public static string Error_RecycleBinPatchOnlySupportsRestoreFeed(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_RecycleBinPatchOnlySupportsRestoreFeed), culture);

    public static string Error_FeedIdNotFoundInRecycleBinMessage(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedIdNotFoundInRecycleBinMessage), arg0);

    public static string Error_FeedIdNotFoundInRecycleBinMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_FeedIdNotFoundInRecycleBinMessage), culture, arg0);

    public static string Error_InternalUpstreamsProjectCouldNotBeResolved(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InternalUpstreamsProjectCouldNotBeResolved), arg0, arg1);

    public static string Error_InternalUpstreamsProjectCouldNotBeResolved(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_InternalUpstreamsProjectCouldNotBeResolved), culture, arg0, arg1);
    }

    public static string Error_ProjectScopedUpstreamsNotEnabled() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ProjectScopedUpstreamsNotEnabled));

    public static string Error_ProjectScopedUpstreamsNotEnabled(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_ProjectScopedUpstreamsNotEnabled), culture);

    public static string Error_BuildCouldNotBeResolved(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_BuildCouldNotBeResolved), arg0, arg1);

    public static string Error_BuildCouldNotBeResolved(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (Error_BuildCouldNotBeResolved), culture, arg0, arg1);
    }

    public static string AuditAddedFormat(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (AuditAddedFormat), arg0);

    public static string AuditAddedFormat(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (AuditAddedFormat), culture, arg0);

    public static string AuditFromFormat(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (AuditFromFormat), arg0, arg1);

    public static string AuditFromFormat(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (AuditFromFormat), culture, arg0, arg1);

    public static string AuditModifedFormat(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (AuditModifedFormat), arg0);

    public static string AuditModifedFormat(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (AuditModifedFormat), culture, arg0);

    public static string AuditNoAdditionalDetails() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (AuditNoAdditionalDetails));

    public static string AuditNoAdditionalDetails(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (AuditNoAdditionalDetails), culture);

    public static string AuditRemovedFormat(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (AuditRemovedFormat), arg0);

    public static string AuditRemovedFormat(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (AuditRemovedFormat), culture, arg0);

    public static string AuditToFormat(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (AuditToFormat), arg0, arg1);

    public static string AuditToFormat(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (AuditToFormat), culture, arg0, arg1);

    public static string RetentionPolicy() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (RetentionPolicy));

    public static string RetentionPolicy(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (RetentionPolicy), culture);

    public static string FeedViewVisibility() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (FeedViewVisibility));

    public static string FeedViewVisibility(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (FeedViewVisibility), culture);

    public static string AuditModifiedFromToFormat(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (AuditModifiedFromToFormat), arg0, arg1, arg2);

    public static string AuditModifiedFromToFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (AuditModifiedFromToFormat), culture, arg0, arg1, arg2);
    }

    public static string FeedPermissions() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (FeedPermissions));

    public static string FeedPermissions(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (FeedPermissions), culture);

    public static string AuditFeedPermissionModified(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (AuditFeedPermissionModified), arg0, arg1);

    public static string AuditFeedPermissionModified(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (AuditFeedPermissionModified), culture, arg0, arg1);

    public static string UpstreamFailureMessage() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (UpstreamFailureMessage));

    public static string UpstreamFailureMessage(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (UpstreamFailureMessage), culture);

    public static string UpstreamFailureMessageLinkText() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (UpstreamFailureMessageLinkText));

    public static string UpstreamFailureMessageLinkText(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (UpstreamFailureMessageLinkText), culture);

    public static string ServiceEndpointNotSupportedText() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (ServiceEndpointNotSupportedText));

    public static string ServiceEndpointNotSupportedText(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (ServiceEndpointNotSupportedText), culture);

    public static string ServiceEndpointNotFoundText() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (ServiceEndpointNotFoundText));

    public static string ServiceEndpointNotFoundText(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (ServiceEndpointNotFoundText), culture);

    public static string InvalidServiceEndpointConfig(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (InvalidServiceEndpointConfig), arg0, arg1);

    public static string InvalidServiceEndpointConfig(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (InvalidServiceEndpointConfig), culture, arg0, arg1);
    }

    public static string ServiceEndpointUnset() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (ServiceEndpointUnset));

    public static string ServiceEndpointUnset(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (ServiceEndpointUnset), culture);

    public static string UpstreamServiceEndpointProjectMissing(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (UpstreamServiceEndpointProjectMissing), arg0);

    public static string UpstreamServiceEndpointProjectMissing(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (UpstreamServiceEndpointProjectMissing), culture, arg0);

    public static string UpstreamSourcesLimitReached(object arg0) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (UpstreamSourcesLimitReached), arg0);

    public static string UpstreamSourcesLimitReached(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Format(nameof (UpstreamSourcesLimitReached), culture, arg0);

    public static string Error_PassedProjectDoesNotMatchFeedProject() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PassedProjectDoesNotMatchFeedProject));

    public static string Error_PassedProjectDoesNotMatchFeedProject(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PassedProjectDoesNotMatchFeedProject), culture);

    public static string Error_PublicFeedsCannotHaveCustomPublicUpstreams() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PublicFeedsCannotHaveCustomPublicUpstreams));

    public static string Error_PublicFeedsCannotHaveCustomPublicUpstreams(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PublicFeedsCannotHaveCustomPublicUpstreams), culture);

    public static string Error_PublicFeedsCannotHavePrivateFeedAsUpstream() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PublicFeedsCannotHavePrivateFeedAsUpstream));

    public static string Error_PublicFeedsCannotHavePrivateFeedAsUpstream(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PublicFeedsCannotHavePrivateFeedAsUpstream), culture);

    public static string Error_PackageDetailsDataProviderErrorNamesAndGuid() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PackageDetailsDataProviderErrorNamesAndGuid));

    public static string Error_PackageDetailsDataProviderErrorNamesAndGuid(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PackageDetailsDataProviderErrorNamesAndGuid), culture);

    public static string Error_PackageDetailsDataProviderErrorNotFoundWithSamePackageCase() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PackageDetailsDataProviderErrorNotFoundWithSamePackageCase));

    public static string Error_PackageDetailsDataProviderErrorNotFoundWithSamePackageCase(
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PackageDetailsDataProviderErrorNotFoundWithSamePackageCase), culture);
    }

    public static string Error_PackageDetailsDataProviderErrorNotFoundWithSameVersionCase() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PackageDetailsDataProviderErrorNotFoundWithSameVersionCase));

    public static string Error_PackageDetailsDataProviderErrorNotFoundWithSameVersionCase(
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PackageDetailsDataProviderErrorNotFoundWithSameVersionCase), culture);
    }

    public static string Error_PackageDetailsDataProviderErrorUnknownProtocol() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PackageDetailsDataProviderErrorUnknownProtocol));

    public static string Error_PackageDetailsDataProviderErrorUnknownProtocol(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (Error_PackageDetailsDataProviderErrorUnknownProtocol), culture);

    public static string NpmAuditBanner() => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (NpmAuditBanner));

    public static string NpmAuditBanner(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Server.Resources.Get(nameof (NpmAuditBanner), culture);
  }
}
