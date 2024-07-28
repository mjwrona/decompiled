// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryResources
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal static class GalleryResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (GalleryResources), typeof (GalleryResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => GalleryResources.s_resMgr;

    private static string Get(string resourceName) => GalleryResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? GalleryResources.Get(resourceName) : GalleryResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) GalleryResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? GalleryResources.GetInt(resourceName) : (int) GalleryResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) GalleryResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? GalleryResources.GetBool(resourceName) : (bool) GalleryResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => GalleryResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = GalleryResources.Get(resourceName, culture);
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

    public static string AnonymousAccessDeniedFormat(object arg0, object arg1) => GalleryResources.Format(nameof (AnonymousAccessDeniedFormat), arg0, arg1);

    public static string AnonymousAccessDeniedFormat(object arg0, object arg1, CultureInfo culture) => GalleryResources.Format(nameof (AnonymousAccessDeniedFormat), culture, arg0, arg1);

    public static string CannotChangeExtensionName(object arg0, object arg1) => GalleryResources.Format(nameof (CannotChangeExtensionName), arg0, arg1);

    public static string CannotChangeExtensionName(object arg0, object arg1, CultureInfo culture) => GalleryResources.Format(nameof (CannotChangeExtensionName), culture, arg0, arg1);

    public static string CannotChangePublisher(object arg0, object arg1) => GalleryResources.Format(nameof (CannotChangePublisher), arg0, arg1);

    public static string CannotChangePublisher(object arg0, object arg1, CultureInfo culture) => GalleryResources.Format(nameof (CannotChangePublisher), culture, arg0, arg1);

    public static string CannotCreateCategoryAnonymously() => GalleryResources.Get(nameof (CannotCreateCategoryAnonymously));

    public static string CannotCreateCategoryAnonymously(CultureInfo culture) => GalleryResources.Get(nameof (CannotCreateCategoryAnonymously), culture);

    public static string CannotManageIndexedTermsNonPublicExtensions() => GalleryResources.Get(nameof (CannotManageIndexedTermsNonPublicExtensions));

    public static string CannotManageIndexedTermsNonPublicExtensions(CultureInfo culture) => GalleryResources.Get(nameof (CannotManageIndexedTermsNonPublicExtensions), culture);

    public static string CannotReportReviewAnonymously() => GalleryResources.Get(nameof (CannotReportReviewAnonymously));

    public static string CannotReportReviewAnonymously(CultureInfo culture) => GalleryResources.Get(nameof (CannotReportReviewAnonymously), culture);

    public static string CannotSubmitReviewAnonymously() => GalleryResources.Get(nameof (CannotSubmitReviewAnonymously));

    public static string CannotSubmitReviewAnonymously(CultureInfo culture) => GalleryResources.Get(nameof (CannotSubmitReviewAnonymously), culture);

    public static string CannotUpdateReviewAnonymously() => GalleryResources.Get(nameof (CannotUpdateReviewAnonymously));

    public static string CannotUpdateReviewAnonymously(CultureInfo culture) => GalleryResources.Get(nameof (CannotUpdateReviewAnonymously), culture);

    public static string InvalidCustomerSupportRequestReason() => GalleryResources.Get(nameof (InvalidCustomerSupportRequestReason));

    public static string InvalidCustomerSupportRequestReason(CultureInfo culture) => GalleryResources.Get(nameof (InvalidCustomerSupportRequestReason), culture);

    public static string CannotCreateSupportRequestAnonymously() => GalleryResources.Get(nameof (CannotCreateSupportRequestAnonymously));

    public static string CannotCreateSupportRequestAnonymously(CultureInfo culture) => GalleryResources.Get(nameof (CannotCreateSupportRequestAnonymously), culture);

    public static string ItemDoesNotExistError(object arg0, object arg1) => GalleryResources.Format(nameof (ItemDoesNotExistError), arg0, arg1);

    public static string ItemDoesNotExistError(object arg0, object arg1, CultureInfo culture) => GalleryResources.Format(nameof (ItemDoesNotExistError), culture, arg0, arg1);

    public static string CSRUploadToBlobFailed() => GalleryResources.Get(nameof (CSRUploadToBlobFailed));

    public static string CSRUploadToBlobFailed(CultureInfo culture) => GalleryResources.Get(nameof (CSRUploadToBlobFailed), culture);

    public static string InvalidCustomerSupportRequestSource() => GalleryResources.Get(nameof (InvalidCustomerSupportRequestSource));

    public static string InvalidCustomerSupportRequestSource(CultureInfo culture) => GalleryResources.Get(nameof (InvalidCustomerSupportRequestSource), culture);

    public static string ExtensionTypeNotVsOrVsCodeOrVsForMac() => GalleryResources.Get(nameof (ExtensionTypeNotVsOrVsCodeOrVsForMac));

    public static string ExtensionTypeNotVsOrVsCodeOrVsForMac(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionTypeNotVsOrVsCodeOrVsForMac), culture);

    public static string ContributorRoleName() => GalleryResources.Get(nameof (ContributorRoleName));

    public static string ContributorRoleName(CultureInfo culture) => GalleryResources.Get(nameof (ContributorRoleName), culture);

    public static string CreatorRoleName() => GalleryResources.Get(nameof (CreatorRoleName));

    public static string CreatorRoleName(CultureInfo culture) => GalleryResources.Get(nameof (CreatorRoleName), culture);

    public static string ExtensionAlreadyExists() => GalleryResources.Get(nameof (ExtensionAlreadyExists));

    public static string ExtensionAlreadyExists(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionAlreadyExists), culture);

    public static string ExtensionDoesNotExist() => GalleryResources.Get(nameof (ExtensionDoesNotExist));

    public static string ExtensionDoesNotExist(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionDoesNotExist), culture);

    public static string ExtensionNotAvailableInAccountRegion(object arg0) => GalleryResources.Format(nameof (ExtensionNotAvailableInAccountRegion), arg0);

    public static string ExtensionNotAvailableInAccountRegion(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ExtensionNotAvailableInAccountRegion), culture, arg0);

    public static string InternalFlagsCantBeUsed() => GalleryResources.Get(nameof (InternalFlagsCantBeUsed));

    public static string InternalFlagsCantBeUsed(CultureInfo culture) => GalleryResources.Get(nameof (InternalFlagsCantBeUsed), culture);

    public static string InvalidAcqusitionTargetFormat() => GalleryResources.Get(nameof (InvalidAcqusitionTargetFormat));

    public static string InvalidAcqusitionTargetFormat(CultureInfo culture) => GalleryResources.Get(nameof (InvalidAcqusitionTargetFormat), culture);

    public static string InValidCategoryLanguageCode(object arg0) => GalleryResources.Format(nameof (InValidCategoryLanguageCode), arg0);

    public static string InValidCategoryLanguageCode(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InValidCategoryLanguageCode), culture, arg0);

    public static string CollectionNotFound() => GalleryResources.Get(nameof (CollectionNotFound));

    public static string CollectionNotFound(CultureInfo culture) => GalleryResources.Get(nameof (CollectionNotFound), culture);

    public static string InvalidExtensionIdFormat(object arg0) => GalleryResources.Format(nameof (InvalidExtensionIdFormat), arg0);

    public static string InvalidExtensionIdFormat(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidExtensionIdFormat), culture, arg0);

    public static string InvalidFullyQualifiedName() => GalleryResources.Get(nameof (InvalidFullyQualifiedName));

    public static string InvalidFullyQualifiedName(CultureInfo culture) => GalleryResources.Get(nameof (InvalidFullyQualifiedName), culture);

    public static string InvalidKeyName(object arg0) => GalleryResources.Format(nameof (InvalidKeyName), arg0);

    public static string InvalidKeyName(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidKeyName), culture, arg0);

    public static string InvalidProductIdFormat(object arg0) => GalleryResources.Format(nameof (InvalidProductIdFormat), arg0);

    public static string InvalidProductIdFormat(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidProductIdFormat), culture, arg0);

    public static string InvalidResourceException() => GalleryResources.Get(nameof (InvalidResourceException));

    public static string InvalidResourceException(CultureInfo culture) => GalleryResources.Get(nameof (InvalidResourceException), culture);

    public static string InvalidResourceIdException() => GalleryResources.Get(nameof (InvalidResourceIdException));

    public static string InvalidResourceIdException(CultureInfo culture) => GalleryResources.Get(nameof (InvalidResourceIdException), culture);

    public static string InvalidReviewPatch() => GalleryResources.Get(nameof (InvalidReviewPatch));

    public static string InvalidReviewPatch(CultureInfo culture) => GalleryResources.Get(nameof (InvalidReviewPatch), culture);

    public static string InvalidVersionFormat(object arg0) => GalleryResources.Format(nameof (InvalidVersionFormat), arg0);

    public static string InvalidVersionFormat(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidVersionFormat), culture, arg0);

    public static string OnPremisesNotSupported() => GalleryResources.Get(nameof (OnPremisesNotSupported));

    public static string OnPremisesNotSupported(CultureInfo culture) => GalleryResources.Get(nameof (OnPremisesNotSupported), culture);

    public static string OnPremisesUnsupportedInstallationTarget(
      object arg0,
      object arg1,
      object arg2)
    {
      return GalleryResources.Format(nameof (OnPremisesUnsupportedInstallationTarget), arg0, arg1, arg2);
    }

    public static string OnPremisesUnsupportedInstallationTarget(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (OnPremisesUnsupportedInstallationTarget), culture, arg0, arg1, arg2);
    }

    public static string OperationNotSupported(object arg0) => GalleryResources.Format(nameof (OperationNotSupported), arg0);

    public static string OperationNotSupported(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (OperationNotSupported), culture, arg0);

    public static string OwnerRoleName() => GalleryResources.Get(nameof (OwnerRoleName));

    public static string OwnerRoleName(CultureInfo culture) => GalleryResources.Get(nameof (OwnerRoleName), culture);

    public static string PathMustMatchExtension(object arg0, object arg1) => GalleryResources.Format(nameof (PathMustMatchExtension), arg0, arg1);

    public static string PathMustMatchExtension(object arg0, object arg1, CultureInfo culture) => GalleryResources.Format(nameof (PathMustMatchExtension), culture, arg0, arg1);

    public static string PathMustMatchPublisher(object arg0, object arg1) => GalleryResources.Format(nameof (PathMustMatchPublisher), arg0, arg1);

    public static string PathMustMatchPublisher(object arg0, object arg1, CultureInfo culture) => GalleryResources.Format(nameof (PathMustMatchPublisher), culture, arg0, arg1);

    public static string PublicExtensionCantBeMadePrivate() => GalleryResources.Get(nameof (PublicExtensionCantBeMadePrivate));

    public static string PublicExtensionCantBeMadePrivate(CultureInfo culture) => GalleryResources.Get(nameof (PublicExtensionCantBeMadePrivate), culture);

    public static string ExtensionAssetUpload(object arg0) => GalleryResources.Format(nameof (ExtensionAssetUpload), arg0);

    public static string ExtensionAssetUpload(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ExtensionAssetUpload), culture, arg0);

    public static string PublisherAlreadyExists() => GalleryResources.Get(nameof (PublisherAlreadyExists));

    public static string PublisherAlreadyExists(CultureInfo culture) => GalleryResources.Get(nameof (PublisherAlreadyExists), culture);

    public static string PublisherDoesNotExist() => GalleryResources.Get(nameof (PublisherDoesNotExist));

    public static string PublisherDoesNotExist(CultureInfo culture) => GalleryResources.Get(nameof (PublisherDoesNotExist), culture);

    public static string ReaderRoleName() => GalleryResources.Get(nameof (ReaderRoleName));

    public static string ReaderRoleName(CultureInfo culture) => GalleryResources.Get(nameof (ReaderRoleName), culture);

    public static string ReviewPatchOperationNotSupported(object arg0) => GalleryResources.Format(nameof (ReviewPatchOperationNotSupported), arg0);

    public static string ReviewPatchOperationNotSupported(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ReviewPatchOperationNotSupported), culture, arg0);

    public static string TagTypeNotSupportedInMgmtIndexedTerms(object arg0) => GalleryResources.Format(nameof (TagTypeNotSupportedInMgmtIndexedTerms), arg0);

    public static string TagTypeNotSupportedInMgmtIndexedTerms(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (TagTypeNotSupportedInMgmtIndexedTerms), culture, arg0);

    public static string UnsupportedCategory(object arg0, object arg1) => GalleryResources.Format(nameof (UnsupportedCategory), arg0, arg1);

    public static string UnsupportedCategory(object arg0, object arg1, CultureInfo culture) => GalleryResources.Format(nameof (UnsupportedCategory), culture, arg0, arg1);

    public static string UnsupportedCategoryLanguage(object arg0) => GalleryResources.Format(nameof (UnsupportedCategoryLanguage), arg0);

    public static string UnsupportedCategoryLanguage(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (UnsupportedCategoryLanguage), culture, arg0);

    public static string VerifiedPublisherNameChangeNotAllowed() => GalleryResources.Get(nameof (VerifiedPublisherNameChangeNotAllowed));

    public static string VerifiedPublisherNameChangeNotAllowed(CultureInfo culture) => GalleryResources.Get(nameof (VerifiedPublisherNameChangeNotAllowed), culture);

    public static string FallbackUserNamePrefix() => GalleryResources.Get(nameof (FallbackUserNamePrefix));

    public static string FallbackUserNamePrefix(CultureInfo culture) => GalleryResources.Get(nameof (FallbackUserNamePrefix), culture);

    public static string PaidSupportedInstallationTarget(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5)
    {
      return GalleryResources.Format(nameof (PaidSupportedInstallationTarget), arg0, arg1, arg2, arg3, arg4, arg5);
    }

    public static string PaidSupportedInstallationTarget(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (PaidSupportedInstallationTarget), culture, arg0, arg1, arg2, arg3, arg4, arg5);
    }

    public static string ExtensionAlreadyUnderTrial(object arg0) => GalleryResources.Format(nameof (ExtensionAlreadyUnderTrial), arg0);

    public static string ExtensionAlreadyUnderTrial(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ExtensionAlreadyUnderTrial), culture, arg0);

    public static string ExtensionTrialExpired(object arg0, object arg1, object arg2) => GalleryResources.Format(nameof (ExtensionTrialExpired), arg0, arg1, arg2);

    public static string ExtensionTrialExpired(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (ExtensionTrialExpired), culture, arg0, arg1, arg2);
    }

    public static string ExtensionContributorDescription() => GalleryResources.Get(nameof (ExtensionContributorDescription));

    public static string ExtensionContributorDescription(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionContributorDescription), culture);

    public static string ExtensionOwnerDescription() => GalleryResources.Get(nameof (ExtensionOwnerDescription));

    public static string ExtensionOwnerDescription(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionOwnerDescription), culture);

    public static string ExtensionReaderDescription() => GalleryResources.Get(nameof (ExtensionReaderDescription));

    public static string ExtensionReaderDescription(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionReaderDescription), culture);

    public static string PublisherContributorDescription() => GalleryResources.Get(nameof (PublisherContributorDescription));

    public static string PublisherContributorDescription(CultureInfo culture) => GalleryResources.Get(nameof (PublisherContributorDescription), culture);

    public static string PublisherCreatorDescription() => GalleryResources.Get(nameof (PublisherCreatorDescription));

    public static string PublisherCreatorDescription(CultureInfo culture) => GalleryResources.Get(nameof (PublisherCreatorDescription), culture);

    public static string PublisherOwnerDescription() => GalleryResources.Get(nameof (PublisherOwnerDescription));

    public static string PublisherOwnerDescription(CultureInfo culture) => GalleryResources.Get(nameof (PublisherOwnerDescription), culture);

    public static string PublisherReaderDescription() => GalleryResources.Get(nameof (PublisherReaderDescription));

    public static string PublisherReaderDescription(CultureInfo culture) => GalleryResources.Get(nameof (PublisherReaderDescription), culture);

    public static string PublicExtensionWithNonZeroInstallCantBeDeleted(object arg0) => GalleryResources.Format(nameof (PublicExtensionWithNonZeroInstallCantBeDeleted), arg0);

    public static string PublicExtensionWithNonZeroInstallCantBeDeleted(
      object arg0,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (PublicExtensionWithNonZeroInstallCantBeDeleted), culture, arg0);
    }

    public static string ExtensionAlreadyPurchased(object arg0) => GalleryResources.Format(nameof (ExtensionAlreadyPurchased), arg0);

    public static string ExtensionAlreadyPurchased(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ExtensionAlreadyPurchased), culture, arg0);

    public static string BuyRedirectText() => GalleryResources.Get(nameof (BuyRedirectText));

    public static string BuyRedirectText(CultureInfo culture) => GalleryResources.Get(nameof (BuyRedirectText), culture);

    public static string DisallowedStringInPublisherName() => GalleryResources.Get(nameof (DisallowedStringInPublisherName));

    public static string DisallowedStringInPublisherName(CultureInfo culture) => GalleryResources.Get(nameof (DisallowedStringInPublisherName), culture);

    public static string PublisherNotifyMailSubject(object arg0) => GalleryResources.Format(nameof (PublisherNotifyMailSubject), arg0);

    public static string PublisherNotifyMailSubject(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (PublisherNotifyMailSubject), culture, arg0);

    public static string ReviewResponseNotifyNote(object arg0, object arg1) => GalleryResources.Format(nameof (ReviewResponseNotifyNote), arg0, arg1);

    public static string ReviewResponseNotifyNote(object arg0, object arg1, CultureInfo culture) => GalleryResources.Format(nameof (ReviewResponseNotifyNote), culture, arg0, arg1);

    public static string PublisherNotifyMailOnText() => GalleryResources.Get(nameof (PublisherNotifyMailOnText));

    public static string PublisherNotifyMailOnText(CultureInfo culture) => GalleryResources.Get(nameof (PublisherNotifyMailOnText), culture);

    public static string AdminText() => GalleryResources.Get(nameof (AdminText));

    public static string AdminText(CultureInfo culture) => GalleryResources.Get(nameof (AdminText), culture);

    public static string AdminNotifyHeaderNote(object arg0) => GalleryResources.Format(nameof (AdminNotifyHeaderNote), arg0);

    public static string AdminNotifyHeaderNote(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (AdminNotifyHeaderNote), culture, arg0);

    public static string PublisherNotifyMailResponseText() => GalleryResources.Get(nameof (PublisherNotifyMailResponseText));

    public static string PublisherNotifyMailResponseText(CultureInfo culture) => GalleryResources.Get(nameof (PublisherNotifyMailResponseText), culture);

    public static string PublisherNotifyMailRespondedText(object arg0) => GalleryResources.Format(nameof (PublisherNotifyMailRespondedText), arg0);

    public static string PublisherNotifyMailRespondedText(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (PublisherNotifyMailRespondedText), culture, arg0);

    public static string ViewOnMarketplaceButtonText() => GalleryResources.Get(nameof (ViewOnMarketplaceButtonText));

    public static string ViewOnMarketplaceButtonText(CultureInfo culture) => GalleryResources.Get(nameof (ViewOnMarketplaceButtonText), culture);

    public static string PublisherNotifyMailPrivacyText() => GalleryResources.Get(nameof (PublisherNotifyMailPrivacyText));

    public static string PublisherNotifyMailPrivacyText(CultureInfo culture) => GalleryResources.Get(nameof (PublisherNotifyMailPrivacyText), culture);

    public static string ReviewNotifyMailSubject(object arg0) => GalleryResources.Format(nameof (ReviewNotifyMailSubject), arg0);

    public static string ReviewNotifyMailSubject(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ReviewNotifyMailSubject), culture, arg0);

    public static string ReviewNotifyMailSubjectUpdated(object arg0) => GalleryResources.Format(nameof (ReviewNotifyMailSubjectUpdated), arg0);

    public static string ReviewNotifyMailSubjectUpdated(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ReviewNotifyMailSubjectUpdated), culture, arg0);

    public static string ReviewNotifyHeaderNote(object arg0) => GalleryResources.Format(nameof (ReviewNotifyHeaderNote), arg0);

    public static string ReviewNotifyHeaderNote(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ReviewNotifyHeaderNote), culture, arg0);

    public static string ReviewNotifyHeaderNoteUpdated(object arg0) => GalleryResources.Format(nameof (ReviewNotifyHeaderNoteUpdated), arg0);

    public static string ReviewNotifyHeaderNoteUpdated(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ReviewNotifyHeaderNoteUpdated), culture, arg0);

    public static string ReviewNotifyIntroNote(object arg0) => GalleryResources.Format(nameof (ReviewNotifyIntroNote), arg0);

    public static string ReviewNotifyIntroNote(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ReviewNotifyIntroNote), culture, arg0);

    public static string ReviewNotifyIntroNoteUpdated(object arg0) => GalleryResources.Format(nameof (ReviewNotifyIntroNoteUpdated), arg0);

    public static string ReviewNotifyIntroNoteUpdated(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ReviewNotifyIntroNoteUpdated), culture, arg0);

    public static string ReviewNotifyRatingText() => GalleryResources.Get(nameof (ReviewNotifyRatingText));

    public static string ReviewNotifyRatingText(CultureInfo culture) => GalleryResources.Get(nameof (ReviewNotifyRatingText), culture);

    public static string CustomerContactMailSubject(object arg0, object arg1) => GalleryResources.Format(nameof (CustomerContactMailSubject), arg0, arg1);

    public static string CustomerContactMailSubject(object arg0, object arg1, CultureInfo culture) => GalleryResources.Format(nameof (CustomerContactMailSubject), culture, arg0, arg1);

    public static string CustomerContactMailHeader(object arg0) => GalleryResources.Format(nameof (CustomerContactMailHeader), arg0);

    public static string CustomerContactMailHeader(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (CustomerContactMailHeader), culture, arg0);

    public static string CustomerContactMailIntroNote(object arg0, object arg1, object arg2) => GalleryResources.Format(nameof (CustomerContactMailIntroNote), arg0, arg1, arg2);

    public static string CustomerContactMailIntroNote(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (CustomerContactMailIntroNote), culture, arg0, arg1, arg2);
    }

    public static string ToCustomerAddress(object arg0) => GalleryResources.Format(nameof (ToCustomerAddress), arg0);

    public static string ToCustomerAddress(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ToCustomerAddress), culture, arg0);

    public static string CustomerContactMailActionButtonText() => GalleryResources.Get(nameof (CustomerContactMailActionButtonText));

    public static string CustomerContactMailActionButtonText(CultureInfo culture) => GalleryResources.Get(nameof (CustomerContactMailActionButtonText), culture);

    public static string CustomerContactMailConsentText() => GalleryResources.Get(nameof (CustomerContactMailConsentText));

    public static string CustomerContactMailConsentText(CultureInfo culture) => GalleryResources.Get(nameof (CustomerContactMailConsentText), culture);

    public static string CustomerContactMailReasonCodeText() => GalleryResources.Get(nameof (CustomerContactMailReasonCodeText));

    public static string CustomerContactMailReasonCodeText(CultureInfo culture) => GalleryResources.Get(nameof (CustomerContactMailReasonCodeText), culture);

    public static string CustomerContactMailReasonText() => GalleryResources.Get(nameof (CustomerContactMailReasonText));

    public static string CustomerContactMailReasonText(CultureInfo culture) => GalleryResources.Get(nameof (CustomerContactMailReasonText), culture);

    public static string SupportTextForHostedResource(object arg0) => GalleryResources.Format(nameof (SupportTextForHostedResource), arg0);

    public static string SupportTextForHostedResource(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (SupportTextForHostedResource), culture, arg0);

    public static string UserIdMismtach() => GalleryResources.Get(nameof (UserIdMismtach));

    public static string UserIdMismtach(CultureInfo culture) => GalleryResources.Get(nameof (UserIdMismtach), culture);

    public static string ExtensionVersionCannotBeDeleted(object arg0, object arg1) => GalleryResources.Format(nameof (ExtensionVersionCannotBeDeleted), arg0, arg1);

    public static string ExtensionVersionCannotBeDeleted(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (ExtensionVersionCannotBeDeleted), culture, arg0, arg1);
    }

    public static string VersionDeletionNotSupported() => GalleryResources.Get(nameof (VersionDeletionNotSupported));

    public static string VersionDeletionNotSupported(CultureInfo culture) => GalleryResources.Get(nameof (VersionDeletionNotSupported), culture);

    public static string InvalidMaxInclusiveCharacter(object arg0) => GalleryResources.Format(nameof (InvalidMaxInclusiveCharacter), arg0);

    public static string InvalidMaxInclusiveCharacter(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidMaxInclusiveCharacter), culture, arg0);

    public static string InvalidMinInclusiveCharacter(object arg0) => GalleryResources.Format(nameof (InvalidMinInclusiveCharacter), arg0);

    public static string InvalidMinInclusiveCharacter(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidMinInclusiveCharacter), culture, arg0);

    public static string InvalidTargetVersion(object arg0) => GalleryResources.Format(nameof (InvalidTargetVersion), arg0);

    public static string InvalidTargetVersion(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidTargetVersion), culture, arg0);

    public static string InvalidVersionRange(object arg0) => GalleryResources.Format(nameof (InvalidVersionRange), arg0);

    public static string InvalidVersionRange(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidVersionRange), culture, arg0);

    public static string InvalidMinGreaterThanMaxVersion(object arg0) => GalleryResources.Format(nameof (InvalidMinGreaterThanMaxVersion), arg0);

    public static string InvalidMinGreaterThanMaxVersion(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidMinGreaterThanMaxVersion), culture, arg0);

    public static string InvalidTfsVersion(object arg0) => GalleryResources.Format(nameof (InvalidTfsVersion), arg0);

    public static string InvalidTfsVersion(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidTfsVersion), culture, arg0);

    public static string MultipleProductFamilyError() => GalleryResources.Get(nameof (MultipleProductFamilyError));

    public static string MultipleProductFamilyError(CultureInfo culture) => GalleryResources.Get(nameof (MultipleProductFamilyError), culture);

    public static string NoProductFamilyError() => GalleryResources.Get(nameof (NoProductFamilyError));

    public static string NoProductFamilyError(CultureInfo culture) => GalleryResources.Get(nameof (NoProductFamilyError), culture);

    public static string ExtensionDailyStatsAccessDenied() => GalleryResources.Get(nameof (ExtensionDailyStatsAccessDenied));

    public static string ExtensionDailyStatsAccessDenied(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionDailyStatsAccessDenied), culture);

    public static string ExtensionDailyStatsNotSupported() => GalleryResources.Get(nameof (ExtensionDailyStatsNotSupported));

    public static string ExtensionDailyStatsNotSupported(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionDailyStatsNotSupported), culture);

    public static string PackageNotFound() => GalleryResources.Get(nameof (PackageNotFound));

    public static string PackageNotFound(CultureInfo culture) => GalleryResources.Get(nameof (PackageNotFound), culture);

    public static string NoEventsForExtension(object arg0, object arg1, object arg2) => GalleryResources.Format(nameof (NoEventsForExtension), arg0, arg1, arg2);

    public static string NoEventsForExtension(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (NoEventsForExtension), culture, arg0, arg1, arg2);
    }

    public static string StatisticDateText() => GalleryResources.Get(nameof (StatisticDateText));

    public static string StatisticDateText(CultureInfo culture) => GalleryResources.Get(nameof (StatisticDateText), culture);

    public static string LastContactedDateText() => GalleryResources.Get(nameof (LastContactedDateText));

    public static string LastContactedDateText(CultureInfo culture) => GalleryResources.Get(nameof (LastContactedDateText), culture);

    public static string WebPageViewsText() => GalleryResources.Get(nameof (WebPageViewsText));

    public static string WebPageViewsText(CultureInfo culture) => GalleryResources.Get(nameof (WebPageViewsText), culture);

    public static string VSTSInstallCountText() => GalleryResources.Get(nameof (VSTSInstallCountText));

    public static string VSTSInstallCountText(CultureInfo culture) => GalleryResources.Get(nameof (VSTSInstallCountText), culture);

    public static string VSCodeInstallCountText() => GalleryResources.Get(nameof (VSCodeInstallCountText));

    public static string VSCodeInstallCountText(CultureInfo culture) => GalleryResources.Get(nameof (VSCodeInstallCountText), culture);

    public static string DownloadCountText() => GalleryResources.Get(nameof (DownloadCountText));

    public static string DownloadCountText(CultureInfo culture) => GalleryResources.Get(nameof (DownloadCountText), culture);

    public static string UninstallCountText() => GalleryResources.Get(nameof (UninstallCountText));

    public static string UninstallCountText(CultureInfo culture) => GalleryResources.Get(nameof (UninstallCountText), culture);

    public static string WebDownloadText() => GalleryResources.Get(nameof (WebDownloadText));

    public static string WebDownloadText(CultureInfo culture) => GalleryResources.Get(nameof (WebDownloadText), culture);

    public static string DownloadFromIDEText() => GalleryResources.Get(nameof (DownloadFromIDEText));

    public static string DownloadFromIDEText(CultureInfo culture) => GalleryResources.Get(nameof (DownloadFromIDEText), culture);

    public static string ConnectedInstallCountText() => GalleryResources.Get(nameof (ConnectedInstallCountText));

    public static string ConnectedInstallCountText(CultureInfo culture) => GalleryResources.Get(nameof (ConnectedInstallCountText), culture);

    public static string AverageRatingText() => GalleryResources.Get(nameof (AverageRatingText));

    public static string AverageRatingText(CultureInfo culture) => GalleryResources.Get(nameof (AverageRatingText), culture);

    public static string AccountIdText() => GalleryResources.Get(nameof (AccountIdText));

    public static string AccountIdText(CultureInfo culture) => GalleryResources.Get(nameof (AccountIdText), culture);

    public static string AccountNameText() => GalleryResources.Get(nameof (AccountNameText));

    public static string AccountNameText(CultureInfo culture) => GalleryResources.Get(nameof (AccountNameText), culture);

    public static string TypeText() => GalleryResources.Get(nameof (TypeText));

    public static string TypeText(CultureInfo culture) => GalleryResources.Get(nameof (TypeText), culture);

    public static string StateText() => GalleryResources.Get(nameof (StateText));

    public static string StateText(CultureInfo culture) => GalleryResources.Get(nameof (StateText), culture);

    public static string CurrentQuantityText() => GalleryResources.Get(nameof (CurrentQuantityText));

    public static string CurrentQuantityText(CultureInfo culture) => GalleryResources.Get(nameof (CurrentQuantityText), culture);

    public static string TotalQuantityText() => GalleryResources.Get(nameof (TotalQuantityText));

    public static string TotalQuantityText(CultureInfo culture) => GalleryResources.Get(nameof (TotalQuantityText), culture);

    public static string ChangedQuantityText() => GalleryResources.Get(nameof (ChangedQuantityText));

    public static string ChangedQuantityText(CultureInfo culture) => GalleryResources.Get(nameof (ChangedQuantityText), culture);

    public static string TrialEndDateText() => GalleryResources.Get(nameof (TrialEndDateText));

    public static string TrialEndDateText(CultureInfo culture) => GalleryResources.Get(nameof (TrialEndDateText), culture);

    public static string HostedEnvironmentText() => GalleryResources.Get(nameof (HostedEnvironmentText));

    public static string HostedEnvironmentText(CultureInfo culture) => GalleryResources.Get(nameof (HostedEnvironmentText), culture);

    public static string OnPremisesEnvironmentText() => GalleryResources.Get(nameof (OnPremisesEnvironmentText));

    public static string OnPremisesEnvironmentText(CultureInfo culture) => GalleryResources.Get(nameof (OnPremisesEnvironmentText), culture);

    public static string NewPurchaseEvent() => GalleryResources.Get(nameof (NewPurchaseEvent));

    public static string NewPurchaseEvent(CultureInfo culture) => GalleryResources.Get(nameof (NewPurchaseEvent), culture);

    public static string CancelPurchaseEvent() => GalleryResources.Get(nameof (CancelPurchaseEvent));

    public static string CancelPurchaseEvent(CultureInfo culture) => GalleryResources.Get(nameof (CancelPurchaseEvent), culture);

    public static string UpgradePurchaseEvent() => GalleryResources.Get(nameof (UpgradePurchaseEvent));

    public static string UpgradePurchaseEvent(CultureInfo culture) => GalleryResources.Get(nameof (UpgradePurchaseEvent), culture);

    public static string DowngradePurchaseEvent() => GalleryResources.Get(nameof (DowngradePurchaseEvent));

    public static string DowngradePurchaseEvent(CultureInfo culture) => GalleryResources.Get(nameof (DowngradePurchaseEvent), culture);

    public static string TrialStartEvent() => GalleryResources.Get(nameof (TrialStartEvent));

    public static string TrialStartEvent(CultureInfo culture) => GalleryResources.Get(nameof (TrialStartEvent), culture);

    public static string ReasonText() => GalleryResources.Get(nameof (ReasonText));

    public static string ReasonText(CultureInfo culture) => GalleryResources.Get(nameof (ReasonText), culture);

    public static string ReasonCodeText() => GalleryResources.Get(nameof (ReasonCodeText));

    public static string ReasonCodeText(CultureInfo culture) => GalleryResources.Get(nameof (ReasonCodeText), culture);

    public static string ExtensionDailyStatsTabText() => GalleryResources.Get(nameof (ExtensionDailyStatsTabText));

    public static string ExtensionDailyStatsTabText(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionDailyStatsTabText), culture);

    public static string UninstallEventsTabText() => GalleryResources.Get(nameof (UninstallEventsTabText));

    public static string UninstallEventsTabText(CultureInfo culture) => GalleryResources.Get(nameof (UninstallEventsTabText), culture);

    public static string SalesTransactionsEventsTabText() => GalleryResources.Get(nameof (SalesTransactionsEventsTabText));

    public static string SalesTransactionsEventsTabText(CultureInfo culture) => GalleryResources.Get(nameof (SalesTransactionsEventsTabText), culture);

    public static string AcquisitionsEventsTabText() => GalleryResources.Get(nameof (AcquisitionsEventsTabText));

    public static string AcquisitionsEventsTabText(CultureInfo culture) => GalleryResources.Get(nameof (AcquisitionsEventsTabText), culture);

    public static string InstallEventsTabText() => GalleryResources.Get(nameof (InstallEventsTabText));

    public static string InstallEventsTabText(CultureInfo culture) => GalleryResources.Get(nameof (InstallEventsTabText), culture);

    public static string RatingAndReviewEventsTabText() => GalleryResources.Get(nameof (RatingAndReviewEventsTabText));

    public static string RatingAndReviewEventsTabText(CultureInfo culture) => GalleryResources.Get(nameof (RatingAndReviewEventsTabText), culture);

    public static string QnAEventsTabText() => GalleryResources.Get(nameof (QnAEventsTabText));

    public static string QnAEventsTabText(CultureInfo culture) => GalleryResources.Get(nameof (QnAEventsTabText), culture);

    public static string ReviewDateText() => GalleryResources.Get(nameof (ReviewDateText));

    public static string ReviewDateText(CultureInfo culture) => GalleryResources.Get(nameof (ReviewDateText), culture);

    public static string ReviewerText() => GalleryResources.Get(nameof (ReviewerText));

    public static string ReviewerText(CultureInfo culture) => GalleryResources.Get(nameof (ReviewerText), culture);

    public static string RatingText() => GalleryResources.Get(nameof (RatingText));

    public static string RatingText(CultureInfo culture) => GalleryResources.Get(nameof (RatingText), culture);

    public static string ReviewText() => GalleryResources.Get(nameof (ReviewText));

    public static string ReviewText(CultureInfo culture) => GalleryResources.Get(nameof (ReviewText), culture);

    public static string ResponseDateText() => GalleryResources.Get(nameof (ResponseDateText));

    public static string ResponseDateText(CultureInfo culture) => GalleryResources.Get(nameof (ResponseDateText), culture);

    public static string ResponseText() => GalleryResources.Get(nameof (ResponseText));

    public static string ResponseText(CultureInfo culture) => GalleryResources.Get(nameof (ResponseText), culture);

    public static string NameText() => GalleryResources.Get(nameof (NameText));

    public static string NameText(CultureInfo culture) => GalleryResources.Get(nameof (NameText), culture);

    public static string QnAText() => GalleryResources.Get(nameof (QnAText));

    public static string QnAText(CultureInfo culture) => GalleryResources.Get(nameof (QnAText), culture);

    public static string YesText() => GalleryResources.Get(nameof (YesText));

    public static string YesText(CultureInfo culture) => GalleryResources.Get(nameof (YesText), culture);

    public static string NoText() => GalleryResources.Get(nameof (NoText));

    public static string NoText(CultureInfo culture) => GalleryResources.Get(nameof (NoText), culture);

    public static string RespondedText() => GalleryResources.Get(nameof (RespondedText));

    public static string RespondedText(CultureInfo culture) => GalleryResources.Get(nameof (RespondedText), culture);

    public static string MarketPlaceText() => GalleryResources.Get(nameof (MarketPlaceText));

    public static string MarketPlaceText(CultureInfo culture) => GalleryResources.Get(nameof (MarketPlaceText), culture);

    public static string TrialSupportedInstallationTarget(object arg0, object arg1, object arg2) => GalleryResources.Format(nameof (TrialSupportedInstallationTarget), arg0, arg1, arg2);

    public static string TrialSupportedInstallationTarget(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (TrialSupportedInstallationTarget), culture, arg0, arg1, arg2);
    }

    public static string ExtensionDailyStatsVersionMismatch(object arg0, object arg1) => GalleryResources.Format(nameof (ExtensionDailyStatsVersionMismatch), arg0, arg1);

    public static string ExtensionDailyStatsVersionMismatch(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (ExtensionDailyStatsVersionMismatch), culture, arg0, arg1);
    }

    public static string ExtensionDailyStatsInvalidOperation(object arg0) => GalleryResources.Format(nameof (ExtensionDailyStatsInvalidOperation), arg0);

    public static string ExtensionDailyStatsInvalidOperation(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ExtensionDailyStatsInvalidOperation), culture, arg0);

    public static string InvalidCategoryIdFormatText() => GalleryResources.Get(nameof (InvalidCategoryIdFormatText));

    public static string InvalidCategoryIdFormatText(CultureInfo culture) => GalleryResources.Get(nameof (InvalidCategoryIdFormatText), culture);

    public static string UnsupportedProductText() => GalleryResources.Get(nameof (UnsupportedProductText));

    public static string UnsupportedProductText(CultureInfo culture) => GalleryResources.Get(nameof (UnsupportedProductText), culture);

    public static string ReviewNotifyCompanyAddress() => GalleryResources.Get(nameof (ReviewNotifyCompanyAddress));

    public static string ReviewNotifyCompanyAddress(CultureInfo culture) => GalleryResources.Get(nameof (ReviewNotifyCompanyAddress), culture);

    public static string ReviewNotifyCompanyIconUrl() => GalleryResources.Get(nameof (ReviewNotifyCompanyIconUrl));

    public static string ReviewNotifyCompanyIconUrl(CultureInfo culture) => GalleryResources.Get(nameof (ReviewNotifyCompanyIconUrl), culture);

    public static string NotificationMailPrivacyUrl() => GalleryResources.Get(nameof (NotificationMailPrivacyUrl));

    public static string NotificationMailPrivacyUrl(CultureInfo culture) => GalleryResources.Get(nameof (NotificationMailPrivacyUrl), culture);

    public static string VisualStudioMarketplaceText() => GalleryResources.Get(nameof (VisualStudioMarketplaceText));

    public static string VisualStudioMarketplaceText(CultureInfo culture) => GalleryResources.Get(nameof (VisualStudioMarketplaceText), culture);

    public static string CannotSubmitQnAAnonymously() => GalleryResources.Get(nameof (CannotSubmitQnAAnonymously));

    public static string CannotSubmitQnAAnonymously(CultureInfo culture) => GalleryResources.Get(nameof (CannotSubmitQnAAnonymously), culture);

    public static string CannotUpdateQnAAnonymously(object arg0) => GalleryResources.Format(nameof (CannotUpdateQnAAnonymously), arg0);

    public static string CannotUpdateQnAAnonymously(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (CannotUpdateQnAAnonymously), culture, arg0);

    public static string QnACreateResponseAccessDenied() => GalleryResources.Get(nameof (QnACreateResponseAccessDenied));

    public static string QnACreateResponseAccessDenied(CultureInfo culture) => GalleryResources.Get(nameof (QnACreateResponseAccessDenied), culture);

    public static string QnAResponseDoesNotExistException(object arg0) => GalleryResources.Format(nameof (QnAResponseDoesNotExistException), arg0);

    public static string QnAResponseDoesNotExistException(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (QnAResponseDoesNotExistException), culture, arg0);

    public static string QnAQuestionDoesNotExistException(object arg0) => GalleryResources.Format(nameof (QnAQuestionDoesNotExistException), arg0);

    public static string QnAQuestionDoesNotExistException(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (QnAQuestionDoesNotExistException), culture, arg0);

    public static string QnAUpdateQuestionAccessDenied() => GalleryResources.Get(nameof (QnAUpdateQuestionAccessDenied));

    public static string QnAUpdateQuestionAccessDenied(CultureInfo culture) => GalleryResources.Get(nameof (QnAUpdateQuestionAccessDenied), culture);

    public static string QnAUpdateResponseAccessDenied() => GalleryResources.Get(nameof (QnAUpdateResponseAccessDenied));

    public static string QnAUpdateResponseAccessDenied(CultureInfo culture) => GalleryResources.Get(nameof (QnAUpdateResponseAccessDenied), culture);

    public static string QnAUnhandledExceptionMessage(object arg0, object arg1) => GalleryResources.Format(nameof (QnAUnhandledExceptionMessage), arg0, arg1);

    public static string QnAUnhandledExceptionMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (QnAUnhandledExceptionMessage), culture, arg0, arg1);
    }

    public static string ExtensionDailyStatsActionNotSupported() => GalleryResources.Get(nameof (ExtensionDailyStatsActionNotSupported));

    public static string ExtensionDailyStatsActionNotSupported(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionDailyStatsActionNotSupported), culture);

    public static string ExtensionDailyStatsUserMustBeLoggedIn() => GalleryResources.Get(nameof (ExtensionDailyStatsUserMustBeLoggedIn));

    public static string ExtensionDailyStatsUserMustBeLoggedIn(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionDailyStatsUserMustBeLoggedIn), culture);

    public static string NotificationMailConcludingNote(object arg0, object arg1) => GalleryResources.Format(nameof (NotificationMailConcludingNote), arg0, arg1);

    public static string NotificationMailConcludingNote(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (NotificationMailConcludingNote), culture, arg0, arg1);
    }

    public static string QnANewItemNotificationMailToPublisherHeaderNote(object arg0, object arg1) => GalleryResources.Format(nameof (QnANewItemNotificationMailToPublisherHeaderNote), arg0, arg1);

    public static string QnANewItemNotificationMailToPublisherHeaderNote(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (QnANewItemNotificationMailToPublisherHeaderNote), culture, arg0, arg1);
    }

    public static string QnANewItemNotificationMailToPublisherIntroNote(object arg0, object arg1) => GalleryResources.Format(nameof (QnANewItemNotificationMailToPublisherIntroNote), arg0, arg1);

    public static string QnANewItemNotificationMailToPublisherIntroNote(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (QnANewItemNotificationMailToPublisherIntroNote), culture, arg0, arg1);
    }

    public static string QnANewItemNotificationMailToPublisherSubject(object arg0, object arg1) => GalleryResources.Format(nameof (QnANewItemNotificationMailToPublisherSubject), arg0, arg1);

    public static string QnANewItemNotificationMailToPublisherSubject(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (QnANewItemNotificationMailToPublisherSubject), culture, arg0, arg1);
    }

    public static string QnANewResponseNotificationMailToUserSubject(object arg0) => GalleryResources.Format(nameof (QnANewResponseNotificationMailToUserSubject), arg0);

    public static string QnANewResponseNotificationMailToUserSubject(
      object arg0,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (QnANewResponseNotificationMailToUserSubject), culture, arg0);
    }

    public static string QnANewResponseNotificationMailHeader(object arg0) => GalleryResources.Format(nameof (QnANewResponseNotificationMailHeader), arg0);

    public static string QnANewResponseNotificationMailHeader(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (QnANewResponseNotificationMailHeader), culture, arg0);

    public static string QnANewResponseNotificationMailToUserIntroNote(object arg0) => GalleryResources.Format(nameof (QnANewResponseNotificationMailToUserIntroNote), arg0);

    public static string QnANewResponseNotificationMailToUserIntroNote(
      object arg0,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (QnANewResponseNotificationMailToUserIntroNote), culture, arg0);
    }

    public static string QnAUpdatedItemNotificationMailHeaderNote(object arg0, object arg1) => GalleryResources.Format(nameof (QnAUpdatedItemNotificationMailHeaderNote), arg0, arg1);

    public static string QnAUpdatedItemNotificationMailHeaderNote(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (QnAUpdatedItemNotificationMailHeaderNote), culture, arg0, arg1);
    }

    public static string QnAUpdatedItemNotificationMailToPublisherIntroNote(
      object arg0,
      object arg1)
    {
      return GalleryResources.Format(nameof (QnAUpdatedItemNotificationMailToPublisherIntroNote), arg0, arg1);
    }

    public static string QnAUpdatedItemNotificationMailToPublisherIntroNote(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (QnAUpdatedItemNotificationMailToPublisherIntroNote), culture, arg0, arg1);
    }

    public static string QnAUpdatedItemNotificationMailToPublisherSubject(object arg0, object arg1) => GalleryResources.Format(nameof (QnAUpdatedItemNotificationMailToPublisherSubject), arg0, arg1);

    public static string QnAUpdatedItemNotificationMailToPublisherSubject(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (QnAUpdatedItemNotificationMailToPublisherSubject), culture, arg0, arg1);
    }

    public static string QnAUpdatedResponseNotificationMailToUserIntroNote(object arg0) => GalleryResources.Format(nameof (QnAUpdatedResponseNotificationMailToUserIntroNote), arg0);

    public static string QnAUpdatedResponseNotificationMailToUserIntroNote(
      object arg0,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (QnAUpdatedResponseNotificationMailToUserIntroNote), culture, arg0);
    }

    public static string QnAUpdatedResponseNotificationMailToUserSubject(object arg0) => GalleryResources.Format(nameof (QnAUpdatedResponseNotificationMailToUserSubject), arg0);

    public static string QnAUpdatedResponseNotificationMailToUserSubject(
      object arg0,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (QnAUpdatedResponseNotificationMailToUserSubject), culture, arg0);
    }

    public static string ExtensionAssetDirectAccessNotSupportedText() => GalleryResources.Get(nameof (ExtensionAssetDirectAccessNotSupportedText));

    public static string ExtensionAssetDirectAccessNotSupportedText(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionAssetDirectAccessNotSupportedText), culture);

    public static string CannotDeleteQnAAnonymously() => GalleryResources.Get(nameof (CannotDeleteQnAAnonymously));

    public static string CannotDeleteQnAAnonymously(CultureInfo culture) => GalleryResources.Get(nameof (CannotDeleteQnAAnonymously), culture);

    public static string RespondOnMarketplaceButtonText() => GalleryResources.Get(nameof (RespondOnMarketplaceButtonText));

    public static string RespondOnMarketplaceButtonText(CultureInfo culture) => GalleryResources.Get(nameof (RespondOnMarketplaceButtonText), culture);

    public static string VsExtensionExceptionMessage() => GalleryResources.Get(nameof (VsExtensionExceptionMessage));

    public static string VsExtensionExceptionMessage(CultureInfo culture) => GalleryResources.Get(nameof (VsExtensionExceptionMessage), culture);

    public static string VerificationLogNotFound() => GalleryResources.Get(nameof (VerificationLogNotFound));

    public static string VerificationLogNotFound(CultureInfo culture) => GalleryResources.Get(nameof (VerificationLogNotFound), culture);

    public static string ContentVerificationLogNotFound() => GalleryResources.Get(nameof (ContentVerificationLogNotFound));

    public static string ContentVerificationLogNotFound(CultureInfo culture) => GalleryResources.Get(nameof (ContentVerificationLogNotFound), culture);

    public static string AzurePublisherDoesNotExist(object arg0, object arg1) => GalleryResources.Format(nameof (AzurePublisherDoesNotExist), arg0, arg1);

    public static string AzurePublisherDoesNotExist(object arg0, object arg1, CultureInfo culture) => GalleryResources.Format(nameof (AzurePublisherDoesNotExist), culture, arg0, arg1);

    public static string ConversionToPaidRestricted() => GalleryResources.Get(nameof (ConversionToPaidRestricted));

    public static string ConversionToPaidRestricted(CultureInfo culture) => GalleryResources.Get(nameof (ConversionToPaidRestricted), culture);

    public static string ExtensionAlreadyExistsPublish() => GalleryResources.Get(nameof (ExtensionAlreadyExistsPublish));

    public static string ExtensionAlreadyExistsPublish(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionAlreadyExistsPublish), culture);

    public static string InvalidCategoryName(object arg0) => GalleryResources.Format(nameof (InvalidCategoryName), arg0);

    public static string InvalidCategoryName(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidCategoryName), culture, arg0);

    public static string InvalidContentPackageManifest(object arg0, object arg1) => GalleryResources.Format(nameof (InvalidContentPackageManifest), arg0, arg1);

    public static string InvalidContentPackageManifest(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (InvalidContentPackageManifest), culture, arg0, arg1);
    }

    public static string InvalidExtensionDefinition(object arg0) => GalleryResources.Format(nameof (InvalidExtensionDefinition), arg0);

    public static string InvalidExtensionDefinition(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidExtensionDefinition), culture, arg0);

    public static string InvalidExtensionIdentifier(object arg0) => GalleryResources.Format(nameof (InvalidExtensionIdentifier), arg0);

    public static string InvalidExtensionIdentifier(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidExtensionIdentifier), culture, arg0);

    public static string InvalidExtensionQuery(object arg0) => GalleryResources.Format(nameof (InvalidExtensionQuery), arg0);

    public static string InvalidExtensionQuery(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidExtensionQuery), culture, arg0);

    public static string InvalidPackageData() => GalleryResources.Get(nameof (InvalidPackageData));

    public static string InvalidPackageData(CultureInfo culture) => GalleryResources.Get(nameof (InvalidPackageData), culture);

    public static string InvalidTag(object arg0) => GalleryResources.Format(nameof (InvalidTag), arg0);

    public static string InvalidTag(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidTag), culture, arg0);

    public static string InvalidVersionNumber(object arg0, object arg1) => GalleryResources.Format(nameof (InvalidVersionNumber), arg0, arg1);

    public static string InvalidVersionNumber(object arg0, object arg1, CultureInfo culture) => GalleryResources.Format(nameof (InvalidVersionNumber), culture, arg0, arg1);

    public static string MustSupplySingleQueryValue(object arg0) => GalleryResources.Format(nameof (MustSupplySingleQueryValue), arg0);

    public static string MustSupplySingleQueryValue(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (MustSupplySingleQueryValue), culture, arg0);

    public static string PaidExtensionCantBeMadeFree() => GalleryResources.Get(nameof (PaidExtensionCantBeMadeFree));

    public static string PaidExtensionCantBeMadeFree(CultureInfo culture) => GalleryResources.Get(nameof (PaidExtensionCantBeMadeFree), culture);

    public static string PaidExtensionLicenseAsset() => GalleryResources.Get(nameof (PaidExtensionLicenseAsset));

    public static string PaidExtensionLicenseAsset(CultureInfo culture) => GalleryResources.Get(nameof (PaidExtensionLicenseAsset), culture);

    public static string PaidExtensionPrivacyAsset() => GalleryResources.Get(nameof (PaidExtensionPrivacyAsset));

    public static string PaidExtensionPrivacyAsset(CultureInfo culture) => GalleryResources.Get(nameof (PaidExtensionPrivacyAsset), culture);

    public static string PaidExtensionShouldContainSupportLink() => GalleryResources.Get(nameof (PaidExtensionShouldContainSupportLink));

    public static string PaidExtensionShouldContainSupportLink(CultureInfo culture) => GalleryResources.Get(nameof (PaidExtensionShouldContainSupportLink), culture);

    public static string PublicExtensionCantBeMadePrivatePublish() => GalleryResources.Get(nameof (PublicExtensionCantBeMadePrivatePublish));

    public static string PublicExtensionCantBeMadePrivatePublish(CultureInfo culture) => GalleryResources.Get(nameof (PublicExtensionCantBeMadePrivatePublish), culture);

    public static string ByolTagOnlyApplicableForPaidExtension(object arg0, object arg1) => GalleryResources.Format(nameof (ByolTagOnlyApplicableForPaidExtension), arg0, arg1);

    public static string ByolTagOnlyApplicableForPaidExtension(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (ByolTagOnlyApplicableForPaidExtension), culture, arg0, arg1);
    }

    public static string MicrosoftDevLabsPreview() => GalleryResources.Get(nameof (MicrosoftDevLabsPreview));

    public static string MicrosoftDevLabsPreview(CultureInfo culture) => GalleryResources.Get(nameof (MicrosoftDevLabsPreview), culture);

    public static string ByolTagNoPricingContentAvailable(object arg0, object arg1) => GalleryResources.Format(nameof (ByolTagNoPricingContentAvailable), arg0, arg1);

    public static string ByolTagNoPricingContentAvailable(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (ByolTagNoPricingContentAvailable), culture, arg0, arg1);
    }

    public static string TrialDaysWithoutByol(object arg0, object arg1) => GalleryResources.Format(nameof (TrialDaysWithoutByol), arg0, arg1);

    public static string TrialDaysWithoutByol(object arg0, object arg1, CultureInfo culture) => GalleryResources.Format(nameof (TrialDaysWithoutByol), culture, arg0, arg1);

    public static string TrialDaysInvalidValue() => GalleryResources.Get(nameof (TrialDaysInvalidValue));

    public static string TrialDaysInvalidValue(CultureInfo culture) => GalleryResources.Get(nameof (TrialDaysInvalidValue), culture);

    public static string PublicExtensionCantBeShared() => GalleryResources.Get(nameof (PublicExtensionCantBeShared));

    public static string PublicExtensionCantBeShared(CultureInfo culture) => GalleryResources.Get(nameof (PublicExtensionCantBeShared), culture);

    public static string PublicExtensionDefaultIconAsset() => GalleryResources.Get(nameof (PublicExtensionDefaultIconAsset));

    public static string PublicExtensionDefaultIconAsset(CultureInfo culture) => GalleryResources.Get(nameof (PublicExtensionDefaultIconAsset), culture);

    public static string PublicExtensionDetailsAsset() => GalleryResources.Get(nameof (PublicExtensionDetailsAsset));

    public static string PublicExtensionDetailsAsset(CultureInfo culture) => GalleryResources.Get(nameof (PublicExtensionDetailsAsset), culture);

    public static string QueryAccountTokenMismatch() => GalleryResources.Get(nameof (QueryAccountTokenMismatch));

    public static string QueryAccountTokenMismatch(CultureInfo culture) => GalleryResources.Get(nameof (QueryAccountTokenMismatch), culture);

    public static string VersionNumberMustIncrease(object arg0, object arg1, object arg2) => GalleryResources.Format(nameof (VersionNumberMustIncrease), arg0, arg1, arg2);

    public static string VersionNumberMustIncrease(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (VersionNumberMustIncrease), culture, arg0, arg1, arg2);
    }

    public static string SvgAssetsNotSupported(object arg0) => GalleryResources.Format(nameof (SvgAssetsNotSupported), arg0);

    public static string SvgAssetsNotSupported(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (SvgAssetsNotSupported), culture, arg0);

    public static string SvgReferenceInBadge(object arg0) => GalleryResources.Format(nameof (SvgReferenceInBadge), arg0);

    public static string SvgReferenceInBadge(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (SvgReferenceInBadge), culture, arg0);

    public static string SvgReferenceInFile(object arg0, object arg1) => GalleryResources.Format(nameof (SvgReferenceInFile), arg0, arg1);

    public static string SvgReferenceInFile(object arg0, object arg1, CultureInfo culture) => GalleryResources.Format(nameof (SvgReferenceInFile), culture, arg0, arg1);

    public static string SvgReferenceInOverview(object arg0) => GalleryResources.Format(nameof (SvgReferenceInOverview), arg0);

    public static string SvgReferenceInOverview(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (SvgReferenceInOverview), culture, arg0);

    public static string SvgReferenceInMarkdown(object arg0) => GalleryResources.Format(nameof (SvgReferenceInMarkdown), arg0);

    public static string SvgReferenceInMarkdown(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (SvgReferenceInMarkdown), culture, arg0);

    public static string AssetNotFound() => GalleryResources.Get(nameof (AssetNotFound));

    public static string AssetNotFound(CultureInfo culture) => GalleryResources.Get(nameof (AssetNotFound), culture);

    public static string ErrorCategoriesRequired() => GalleryResources.Get(nameof (ErrorCategoriesRequired));

    public static string ErrorCategoriesRequired(CultureInfo culture) => GalleryResources.Get(nameof (ErrorCategoriesRequired), culture);

    public static string ErrorDescriptionMaxLength() => GalleryResources.Get(nameof (ErrorDescriptionMaxLength));

    public static string ErrorDescriptionMaxLength(CultureInfo culture) => GalleryResources.Get(nameof (ErrorDescriptionMaxLength), culture);

    public static string ErrorDescriptionRequired() => GalleryResources.Get(nameof (ErrorDescriptionRequired));

    public static string ErrorDescriptionRequired(CultureInfo culture) => GalleryResources.Get(nameof (ErrorDescriptionRequired), culture);

    public static string ErrorDisplayNameRequired() => GalleryResources.Get(nameof (ErrorDisplayNameRequired));

    public static string ErrorDisplayNameRequired(CultureInfo culture) => GalleryResources.Get(nameof (ErrorDisplayNameRequired), culture);

    public static string ErrorDisplayNameTooLong() => GalleryResources.Get(nameof (ErrorDisplayNameTooLong));

    public static string ErrorDisplayNameTooLong(CultureInfo culture) => GalleryResources.Get(nameof (ErrorDisplayNameTooLong), culture);

    public static string ErrorInvalidSourceCodeUrl() => GalleryResources.Get(nameof (ErrorInvalidSourceCodeUrl));

    public static string ErrorInvalidSourceCodeUrl(CultureInfo culture) => GalleryResources.Get(nameof (ErrorInvalidSourceCodeUrl), culture);

    public static string ErrorLanguageRequired() => GalleryResources.Get(nameof (ErrorLanguageRequired));

    public static string ErrorLanguageRequired(CultureInfo culture) => GalleryResources.Get(nameof (ErrorLanguageRequired), culture);

    public static string ErrorTagEmpty() => GalleryResources.Get(nameof (ErrorTagEmpty));

    public static string ErrorTagEmpty(CultureInfo culture) => GalleryResources.Get(nameof (ErrorTagEmpty), culture);

    public static string ErrorTagTooLong(object arg0) => GalleryResources.Format(nameof (ErrorTagTooLong), arg0);

    public static string ErrorTagTooLong(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ErrorTagTooLong), culture, arg0);

    public static string ErrorExtensionPacksCategoryNotSupported() => GalleryResources.Get(nameof (ErrorExtensionPacksCategoryNotSupported));

    public static string ErrorExtensionPacksCategoryNotSupported(CultureInfo culture) => GalleryResources.Get(nameof (ErrorExtensionPacksCategoryNotSupported), culture);

    public static string ErrorExtensionPacksCategoryRequired() => GalleryResources.Get(nameof (ErrorExtensionPacksCategoryRequired));

    public static string ErrorExtensionPacksCategoryRequired(CultureInfo culture) => GalleryResources.Get(nameof (ErrorExtensionPacksCategoryRequired), culture);

    public static string ErrorExtensionSdkCategoryRequired() => GalleryResources.Get(nameof (ErrorExtensionSdkCategoryRequired));

    public static string ErrorExtensionSdkCategoryRequired(CultureInfo culture) => GalleryResources.Get(nameof (ErrorExtensionSdkCategoryRequired), culture);

    public static string ErrorInstallationTargetsRequired() => GalleryResources.Get(nameof (ErrorInstallationTargetsRequired));

    public static string ErrorInstallationTargetsRequired(CultureInfo culture) => GalleryResources.Get(nameof (ErrorInstallationTargetsRequired), culture);

    public static string ErrorRequestedCategoriesNotExist() => GalleryResources.Get(nameof (ErrorRequestedCategoriesNotExist));

    public static string ErrorRequestedCategoriesNotExist(CultureInfo culture) => GalleryResources.Get(nameof (ErrorRequestedCategoriesNotExist), culture);

    public static string ErrorOnlyOneRootCategoryAllowed() => GalleryResources.Get(nameof (ErrorOnlyOneRootCategoryAllowed));

    public static string ErrorOnlyOneRootCategoryAllowed(CultureInfo culture) => GalleryResources.Get(nameof (ErrorOnlyOneRootCategoryAllowed), culture);

    public static string ErrorRootCategoryNotFound() => GalleryResources.Get(nameof (ErrorRootCategoryNotFound));

    public static string ErrorRootCategoryNotFound(CultureInfo culture) => GalleryResources.Get(nameof (ErrorRootCategoryNotFound), culture);

    public static string ErrorDuplicateVsixId() => GalleryResources.Get(nameof (ErrorDuplicateVsixId));

    public static string ErrorDuplicateVsixId(CultureInfo culture) => GalleryResources.Get(nameof (ErrorDuplicateVsixId), culture);

    public static string ErrorNonTruncatedVsixId() => GalleryResources.Get(nameof (ErrorNonTruncatedVsixId));

    public static string ErrorNonTruncatedVsixId(CultureInfo culture) => GalleryResources.Get(nameof (ErrorNonTruncatedVsixId), culture);

    public static string ValidationMessageForIncorrectVsixTypeShouldBeControl() => GalleryResources.Get(nameof (ValidationMessageForIncorrectVsixTypeShouldBeControl));

    public static string ValidationMessageForIncorrectVsixTypeShouldBeControl(CultureInfo culture) => GalleryResources.Get(nameof (ValidationMessageForIncorrectVsixTypeShouldBeControl), culture);

    public static string ValidationMessageForIncorrectVsixTypeShouldBeTemplate() => GalleryResources.Get(nameof (ValidationMessageForIncorrectVsixTypeShouldBeTemplate));

    public static string ValidationMessageForIncorrectVsixTypeShouldBeTemplate(CultureInfo culture) => GalleryResources.Get(nameof (ValidationMessageForIncorrectVsixTypeShouldBeTemplate), culture);

    public static string ValidationMessageForIncorrectVsixTypeShouldBeTool() => GalleryResources.Get(nameof (ValidationMessageForIncorrectVsixTypeShouldBeTool));

    public static string ValidationMessageForIncorrectVsixTypeShouldBeTool(CultureInfo culture) => GalleryResources.Get(nameof (ValidationMessageForIncorrectVsixTypeShouldBeTool), culture);

    public static string ValidationMessageForIncorrectMsiExePayload() => GalleryResources.Get(nameof (ValidationMessageForIncorrectMsiExePayload));

    public static string ValidationMessageForIncorrectMsiExePayload(CultureInfo culture) => GalleryResources.Get(nameof (ValidationMessageForIncorrectMsiExePayload), culture);

    public static string ErrorInvalidVSIXPackage() => GalleryResources.Get(nameof (ErrorInvalidVSIXPackage));

    public static string ErrorInvalidVSIXPackage(CultureInfo culture) => GalleryResources.Get(nameof (ErrorInvalidVSIXPackage), culture);

    public static string ErrorInvalidVSIXManifest() => GalleryResources.Get(nameof (ErrorInvalidVSIXManifest));

    public static string ErrorInvalidVSIXManifest(CultureInfo culture) => GalleryResources.Get(nameof (ErrorInvalidVSIXManifest), culture);

    public static string ErrorMissingVSIXManifest() => GalleryResources.Get(nameof (ErrorMissingVSIXManifest));

    public static string ErrorMissingVSIXManifest(CultureInfo culture) => GalleryResources.Get(nameof (ErrorMissingVSIXManifest), culture);

    public static string ErrorMissingTemplateFile() => GalleryResources.Get(nameof (ErrorMissingTemplateFile));

    public static string ErrorMissingTemplateFile(CultureInfo culture) => GalleryResources.Get(nameof (ErrorMissingTemplateFile), culture);

    public static string ErrorInvalidTemplateZipFile() => GalleryResources.Get(nameof (ErrorInvalidTemplateZipFile));

    public static string ErrorInvalidTemplateZipFile(CultureInfo culture) => GalleryResources.Get(nameof (ErrorInvalidTemplateZipFile), culture);

    public static string ErrorInvalidVSTemplateFile() => GalleryResources.Get(nameof (ErrorInvalidVSTemplateFile));

    public static string ErrorInvalidVSTemplateFile(CultureInfo culture) => GalleryResources.Get(nameof (ErrorInvalidVSTemplateFile), culture);

    public static string ErrorInvalidCertificate() => GalleryResources.Get(nameof (ErrorInvalidCertificate));

    public static string ErrorInvalidCertificate(CultureInfo culture) => GalleryResources.Get(nameof (ErrorInvalidCertificate), culture);

    public static string ErrorInvalidSignature() => GalleryResources.Get(nameof (ErrorInvalidSignature));

    public static string ErrorInvalidSignature(CultureInfo culture) => GalleryResources.Get(nameof (ErrorInvalidSignature), culture);

    public static string ErrorMissingIcon() => GalleryResources.Get(nameof (ErrorMissingIcon));

    public static string ErrorMissingIcon(CultureInfo culture) => GalleryResources.Get(nameof (ErrorMissingIcon), culture);

    public static string ErrorMissingPreviewImage() => GalleryResources.Get(nameof (ErrorMissingPreviewImage));

    public static string ErrorMissingPreviewImage(CultureInfo culture) => GalleryResources.Get(nameof (ErrorMissingPreviewImage), culture);

    public static string ErrorMissingEULA() => GalleryResources.Get(nameof (ErrorMissingEULA));

    public static string ErrorMissingEULA(CultureInfo culture) => GalleryResources.Get(nameof (ErrorMissingEULA), culture);

    public static string ErrorInvalidImageFile() => GalleryResources.Get(nameof (ErrorInvalidImageFile));

    public static string ErrorInvalidImageFile(CultureInfo culture) => GalleryResources.Get(nameof (ErrorInvalidImageFile), culture);

    public static string ErrorMissingVSTemplateFile() => GalleryResources.Get(nameof (ErrorMissingVSTemplateFile));

    public static string ErrorMissingVSTemplateFile(CultureInfo culture) => GalleryResources.Get(nameof (ErrorMissingVSTemplateFile), culture);

    public static string ErrorInvalidTypeTargetingExpress() => GalleryResources.Get(nameof (ErrorInvalidTypeTargetingExpress));

    public static string ErrorInvalidTypeTargetingExpress(CultureInfo culture) => GalleryResources.Get(nameof (ErrorInvalidTypeTargetingExpress), culture);

    public static string ErrorPathMayBeTooLong() => GalleryResources.Get(nameof (ErrorPathMayBeTooLong));

    public static string ErrorPathMayBeTooLong(CultureInfo culture) => GalleryResources.Get(nameof (ErrorPathMayBeTooLong), culture);

    public static string ErrorInvalidVSIXLanguagePackManifest() => GalleryResources.Get(nameof (ErrorInvalidVSIXLanguagePackManifest));

    public static string ErrorInvalidVSIXLanguagePackManifest(CultureInfo culture) => GalleryResources.Get(nameof (ErrorInvalidVSIXLanguagePackManifest), culture);

    public static string ErrorEULAExceedsMaximumSize(object arg0) => GalleryResources.Format(nameof (ErrorEULAExceedsMaximumSize), arg0);

    public static string ErrorEULAExceedsMaximumSize(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ErrorEULAExceedsMaximumSize), culture, arg0);

    public static string ErrorVsixFormatNotSupportedByVS2010() => GalleryResources.Get(nameof (ErrorVsixFormatNotSupportedByVS2010));

    public static string ErrorVsixFormatNotSupportedByVS2010(CultureInfo culture) => GalleryResources.Get(nameof (ErrorVsixFormatNotSupportedByVS2010), culture);

    public static string ErrorVSIXIsExperimental() => GalleryResources.Get(nameof (ErrorVSIXIsExperimental));

    public static string ErrorVSIXIsExperimental(CultureInfo culture) => GalleryResources.Get(nameof (ErrorVSIXIsExperimental), culture);

    public static string ErrorPotentialLongPathName() => GalleryResources.Get(nameof (ErrorPotentialLongPathName));

    public static string ErrorPotentialLongPathName(CultureInfo culture) => GalleryResources.Get(nameof (ErrorPotentialLongPathName), culture);

    public static string ValidationMessageForTemplateNotSupportedWithLinkType() => GalleryResources.Get(nameof (ValidationMessageForTemplateNotSupportedWithLinkType));

    public static string ValidationMessageForTemplateNotSupportedWithLinkType(CultureInfo culture) => GalleryResources.Get(nameof (ValidationMessageForTemplateNotSupportedWithLinkType), culture);

    public static string ToolCannotUseExpress() => GalleryResources.Get(nameof (ToolCannotUseExpress));

    public static string ToolCannotUseExpress(CultureInfo culture) => GalleryResources.Get(nameof (ToolCannotUseExpress), culture);

    public static string LinkMustBeValidUrl() => GalleryResources.Get(nameof (LinkMustBeValidUrl));

    public static string LinkMustBeValidUrl(CultureInfo culture) => GalleryResources.Get(nameof (LinkMustBeValidUrl), culture);

    public static string ErrorTooManyEditAttempts() => GalleryResources.Get(nameof (ErrorTooManyEditAttempts));

    public static string ErrorTooManyEditAttempts(CultureInfo culture) => GalleryResources.Get(nameof (ErrorTooManyEditAttempts), culture);

    public static string ExtensionAlreadyUpdated() => GalleryResources.Get(nameof (ExtensionAlreadyUpdated));

    public static string ExtensionAlreadyUpdated(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionAlreadyUpdated), culture);

    public static string UnsupportedFileTypeError() => GalleryResources.Get(nameof (UnsupportedFileTypeError));

    public static string UnsupportedFileTypeError(CultureInfo culture) => GalleryResources.Get(nameof (UnsupportedFileTypeError), culture);

    public static string ErrorDraftCannotBeUsed() => GalleryResources.Get(nameof (ErrorDraftCannotBeUsed));

    public static string ErrorDraftCannotBeUsed(CultureInfo culture) => GalleryResources.Get(nameof (ErrorDraftCannotBeUsed), culture);

    public static string ErrorVsixIdCannotBeChanged() => GalleryResources.Get(nameof (ErrorVsixIdCannotBeChanged));

    public static string ErrorVsixIdCannotBeChanged(CultureInfo culture) => GalleryResources.Get(nameof (ErrorVsixIdCannotBeChanged), culture);

    public static string ErrorMicrosoftExtensionNotSigned() => GalleryResources.Get(nameof (ErrorMicrosoftExtensionNotSigned));

    public static string ErrorMicrosoftExtensionNotSigned(CultureInfo culture) => GalleryResources.Get(nameof (ErrorMicrosoftExtensionNotSigned), culture);

    public static string ErrorNotMicrosoftEmployee() => GalleryResources.Get(nameof (ErrorNotMicrosoftEmployee));

    public static string ErrorNotMicrosoftEmployee(CultureInfo culture) => GalleryResources.Get(nameof (ErrorNotMicrosoftEmployee), culture);

    public static string AuthorNamePublisherDisplayNameMismatch(object arg0, object arg1) => GalleryResources.Format(nameof (AuthorNamePublisherDisplayNameMismatch), arg0, arg1);

    public static string AuthorNamePublisherDisplayNameMismatch(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (AuthorNamePublisherDisplayNameMismatch), culture, arg0, arg1);
    }

    public static string InvalidVsixIdTargetingExpress() => GalleryResources.Get(nameof (InvalidVsixIdTargetingExpress));

    public static string InvalidVsixIdTargetingExpress(CultureInfo culture) => GalleryResources.Get(nameof (InvalidVsixIdTargetingExpress), culture);

    public static string VirusScanFailedMessage() => GalleryResources.Get(nameof (VirusScanFailedMessage));

    public static string VirusScanFailedMessage(CultureInfo culture) => GalleryResources.Get(nameof (VirusScanFailedMessage), culture);

    public static string PublishFailureGeneric() => GalleryResources.Get(nameof (PublishFailureGeneric));

    public static string PublishFailureGeneric(CultureInfo culture) => GalleryResources.Get(nameof (PublishFailureGeneric), culture);

    public static string ProductFamilyChangeError() => GalleryResources.Get(nameof (ProductFamilyChangeError));

    public static string ProductFamilyChangeError(CultureInfo culture) => GalleryResources.Get(nameof (ProductFamilyChangeError), culture);

    public static string AssetCountLimitExceededMessage(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return GalleryResources.Format(nameof (AssetCountLimitExceededMessage), arg0, arg1, arg2, arg3);
    }

    public static string AssetCountLimitExceededMessage(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (AssetCountLimitExceededMessage), culture, arg0, arg1, arg2, arg3);
    }

    public static string LockedExtensionEditErrorMessage() => GalleryResources.Get(nameof (LockedExtensionEditErrorMessage));

    public static string LockedExtensionEditErrorMessage(CultureInfo culture) => GalleryResources.Get(nameof (LockedExtensionEditErrorMessage), culture);

    public static string MemberAdditionEmailContent(object arg0, object arg1) => GalleryResources.Format(nameof (MemberAdditionEmailContent), arg0, arg1);

    public static string MemberAdditionEmailContent(object arg0, object arg1, CultureInfo culture) => GalleryResources.Format(nameof (MemberAdditionEmailContent), culture, arg0, arg1);

    public static string MemberAdditionEmailSubject(object arg0) => GalleryResources.Format(nameof (MemberAdditionEmailSubject), arg0);

    public static string MemberAdditionEmailSubject(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (MemberAdditionEmailSubject), culture, arg0);

    public static string MemberDeletionEmailContent(object arg0, object arg1, object arg2) => GalleryResources.Format(nameof (MemberDeletionEmailContent), arg0, arg1, arg2);

    public static string MemberDeletionEmailContent(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (MemberDeletionEmailContent), culture, arg0, arg1, arg2);
    }

    public static string MemberDeletionEmailSubject(object arg0) => GalleryResources.Format(nameof (MemberDeletionEmailSubject), arg0);

    public static string MemberDeletionEmailSubject(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (MemberDeletionEmailSubject), culture, arg0);

    public static string ErrorIconSize() => GalleryResources.Get(nameof (ErrorIconSize));

    public static string ErrorIconSize(CultureInfo culture) => GalleryResources.Get(nameof (ErrorIconSize), culture);

    public static string PublisherLogoDeletionFailedMessage() => GalleryResources.Get(nameof (PublisherLogoDeletionFailedMessage));

    public static string PublisherLogoDeletionFailedMessage(CultureInfo culture) => GalleryResources.Get(nameof (PublisherLogoDeletionFailedMessage), culture);

    public static string PublisherLogoUploadFailedMessage() => GalleryResources.Get(nameof (PublisherLogoUploadFailedMessage));

    public static string PublisherLogoUploadFailedMessage(CultureInfo culture) => GalleryResources.Get(nameof (PublisherLogoUploadFailedMessage), culture);

    public static string PublisherMetadataLengthExceeded() => GalleryResources.Get(nameof (PublisherMetadataLengthExceeded));

    public static string PublisherMetadataLengthExceeded(CultureInfo culture) => GalleryResources.Get(nameof (PublisherMetadataLengthExceeded), culture);

    public static string PublisherMetadataSerializationError() => GalleryResources.Get(nameof (PublisherMetadataSerializationError));

    public static string PublisherMetadataSerializationError(CultureInfo culture) => GalleryResources.Get(nameof (PublisherMetadataSerializationError), culture);

    public static string PublisherLogoSizeExceededMaxLimit() => GalleryResources.Get(nameof (PublisherLogoSizeExceededMaxLimit));

    public static string PublisherLogoSizeExceededMaxLimit(CultureInfo culture) => GalleryResources.Get(nameof (PublisherLogoSizeExceededMaxLimit), culture);

    public static string InvalidAssetType(object arg0) => GalleryResources.Format(nameof (InvalidAssetType), arg0);

    public static string InvalidAssetType(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidAssetType), culture, arg0);

    public static string PublisherLogoSpecificationsException() => GalleryResources.Get(nameof (PublisherLogoSpecificationsException));

    public static string PublisherLogoSpecificationsException(CultureInfo culture) => GalleryResources.Get(nameof (PublisherLogoSpecificationsException), culture);

    public static string CVSMetadataStreamDescription() => GalleryResources.Get(nameof (CVSMetadataStreamDescription));

    public static string CVSMetadataStreamDescription(CultureInfo culture) => GalleryResources.Get(nameof (CVSMetadataStreamDescription), culture);

    public static string TextViolationWords() => GalleryResources.Get(nameof (TextViolationWords));

    public static string TextViolationWords(CultureInfo culture) => GalleryResources.Get(nameof (TextViolationWords), culture);

    public static string PublisherDescriptionExceededLengthMessage(object arg0) => GalleryResources.Format(nameof (PublisherDescriptionExceededLengthMessage), arg0);

    public static string PublisherDescriptionExceededLengthMessage(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (PublisherDescriptionExceededLengthMessage), culture, arg0);

    public static string PublisherMetadataDeserializationError() => GalleryResources.Get(nameof (PublisherMetadataDeserializationError));

    public static string PublisherMetadataDeserializationError(CultureInfo culture) => GalleryResources.Get(nameof (PublisherMetadataDeserializationError), culture);

    public static string PublisherProfileContainsBlockedHosts() => GalleryResources.Get(nameof (PublisherProfileContainsBlockedHosts));

    public static string PublisherProfileContainsBlockedHosts(CultureInfo culture) => GalleryResources.Get(nameof (PublisherProfileContainsBlockedHosts), culture);

    public static string ExtensionMetadataHasSuspiciousContent() => GalleryResources.Get(nameof (ExtensionMetadataHasSuspiciousContent));

    public static string ExtensionMetadataHasSuspiciousContent(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionMetadataHasSuspiciousContent), culture);

    public static string ExtensionContainsBlockedHosts() => GalleryResources.Get(nameof (ExtensionContainsBlockedHosts));

    public static string ExtensionContainsBlockedHosts(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionContainsBlockedHosts), culture);

    public static string ExtensionOverviewScanTimedOut() => GalleryResources.Get(nameof (ExtensionOverviewScanTimedOut));

    public static string ExtensionOverviewScanTimedOut(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionOverviewScanTimedOut), culture);

    public static string CannotHaveCertificationPendingAndRejectionTogether() => GalleryResources.Get(nameof (CannotHaveCertificationPendingAndRejectionTogether));

    public static string CannotHaveCertificationPendingAndRejectionTogether(CultureInfo culture) => GalleryResources.Get(nameof (CannotHaveCertificationPendingAndRejectionTogether), culture);

    public static string CertifiedPublisherVersionValidationFailureMessage(object arg0) => GalleryResources.Format(nameof (CertifiedPublisherVersionValidationFailureMessage), arg0);

    public static string CertifiedPublisherVersionValidationFailureMessage(
      object arg0,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (CertifiedPublisherVersionValidationFailureMessage), culture, arg0);
    }

    public static string EulaText() => GalleryResources.Get(nameof (EulaText));

    public static string EulaText(CultureInfo culture) => GalleryResources.Get(nameof (EulaText), culture);

    public static string PrivacyPolicyText() => GalleryResources.Get(nameof (PrivacyPolicyText));

    public static string PrivacyPolicyText(CultureInfo culture) => GalleryResources.Get(nameof (PrivacyPolicyText), culture);

    public static string SupportInfoText() => GalleryResources.Get(nameof (SupportInfoText));

    public static string SupportInfoText(CultureInfo culture) => GalleryResources.Get(nameof (SupportInfoText), culture);

    public static string PricingFree() => GalleryResources.Get(nameof (PricingFree));

    public static string PricingFree(CultureInfo culture) => GalleryResources.Get(nameof (PricingFree), culture);

    public static string PricingPaid() => GalleryResources.Get(nameof (PricingPaid));

    public static string PricingPaid(CultureInfo culture) => GalleryResources.Get(nameof (PricingPaid), culture);

    public static string PricingTrial() => GalleryResources.Get(nameof (PricingTrial));

    public static string PricingTrial(CultureInfo culture) => GalleryResources.Get(nameof (PricingTrial), culture);

    public static string InvalidShareType() => GalleryResources.Get(nameof (InvalidShareType));

    public static string InvalidShareType(CultureInfo culture) => GalleryResources.Get(nameof (InvalidShareType), culture);

    public static string OrganizationNotFound() => GalleryResources.Get(nameof (OrganizationNotFound));

    public static string OrganizationNotFound(CultureInfo culture) => GalleryResources.Get(nameof (OrganizationNotFound), culture);

    public static string FilterValueNull(object arg0) => GalleryResources.Format(nameof (FilterValueNull), arg0);

    public static string FilterValueNull(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (FilterValueNull), culture, arg0);

    public static string UnRecognizedFilterType(object arg0) => GalleryResources.Format(nameof (UnRecognizedFilterType), arg0);

    public static string UnRecognizedFilterType(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (UnRecognizedFilterType), culture, arg0);

    public static string InvalidProductToCache(object arg0) => GalleryResources.Format(nameof (InvalidProductToCache), arg0);

    public static string InvalidProductToCache(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidProductToCache), culture, arg0);

    public static string PublisherVerificationMandatoryDescriptionMessage() => GalleryResources.Get(nameof (PublisherVerificationMandatoryDescriptionMessage));

    public static string PublisherVerificationMandatoryDescriptionMessage(CultureInfo culture) => GalleryResources.Get(nameof (PublisherVerificationMandatoryDescriptionMessage), culture);

    public static string PublisherVerificationMandatoryLinksMessage() => GalleryResources.Get(nameof (PublisherVerificationMandatoryLinksMessage));

    public static string PublisherVerificationMandatoryLinksMessage(CultureInfo culture) => GalleryResources.Get(nameof (PublisherVerificationMandatoryLinksMessage), culture);

    public static string PublisherVerificationMandatorySupportLinkMessage() => GalleryResources.Get(nameof (PublisherVerificationMandatorySupportLinkMessage));

    public static string PublisherVerificationMandatorySupportLinkMessage(CultureInfo culture) => GalleryResources.Get(nameof (PublisherVerificationMandatorySupportLinkMessage), culture);

    public static string OneOrMoreCategoryExpected() => GalleryResources.Get(nameof (OneOrMoreCategoryExpected));

    public static string OneOrMoreCategoryExpected(CultureInfo culture) => GalleryResources.Get(nameof (OneOrMoreCategoryExpected), culture);

    public static string DeprecatedAzureDevopsCategory(object arg0) => GalleryResources.Format(nameof (DeprecatedAzureDevopsCategory), arg0);

    public static string DeprecatedAzureDevopsCategory(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (DeprecatedAzureDevopsCategory), culture, arg0);

    public static string ExtensionScopesChangedMessage() => GalleryResources.Get(nameof (ExtensionScopesChangedMessage));

    public static string ExtensionScopesChangedMessage(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionScopesChangedMessage), culture);

    public static string ExtensionPublishFromNonServicePrincipal(object arg0, object arg1) => GalleryResources.Format(nameof (ExtensionPublishFromNonServicePrincipal), arg0, arg1);

    public static string ExtensionPublishFromNonServicePrincipal(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (ExtensionPublishFromNonServicePrincipal), culture, arg0, arg1);
    }

    public static string CertifiedPublisherVersionValidationFailureSubMessage(object arg0) => GalleryResources.Format(nameof (CertifiedPublisherVersionValidationFailureSubMessage), arg0);

    public static string CertifiedPublisherVersionValidationFailureSubMessage(
      object arg0,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (CertifiedPublisherVersionValidationFailureSubMessage), culture, arg0);
    }

    public static string ExtensionDoesNotExistInCache(object arg0, object arg1, object arg2) => GalleryResources.Format(nameof (ExtensionDoesNotExistInCache), arg0, arg1, arg2);

    public static string ExtensionDoesNotExistInCache(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (ExtensionDoesNotExistInCache), culture, arg0, arg1, arg2);
    }

    public static string DeletedUser() => GalleryResources.Get(nameof (DeletedUser));

    public static string DeletedUser(CultureInfo culture) => GalleryResources.Get(nameof (DeletedUser), culture);

    public static string MaxLimitOnPublisherError() => GalleryResources.Get(nameof (MaxLimitOnPublisherError));

    public static string MaxLimitOnPublisherError(CultureInfo culture) => GalleryResources.Get(nameof (MaxLimitOnPublisherError), culture);

    public static string PublisherMetadataSpamCheckError() => GalleryResources.Get(nameof (PublisherMetadataSpamCheckError));

    public static string PublisherMetadataSpamCheckError(CultureInfo culture) => GalleryResources.Get(nameof (PublisherMetadataSpamCheckError), culture);

    public static string CSRCreateThresholdExceededException() => GalleryResources.Get(nameof (CSRCreateThresholdExceededException));

    public static string CSRCreateThresholdExceededException(CultureInfo culture) => GalleryResources.Get(nameof (CSRCreateThresholdExceededException), culture);

    public static string ExtensionLinkTypeDisabledException() => GalleryResources.Get(nameof (ExtensionLinkTypeDisabledException));

    public static string ExtensionLinkTypeDisabledException(CultureInfo culture) => GalleryResources.Get(nameof (ExtensionLinkTypeDisabledException), culture);

    public static string UrlNotAllowedinPublisherProfileError() => GalleryResources.Get(nameof (UrlNotAllowedinPublisherProfileError));

    public static string UrlNotAllowedinPublisherProfileError(CultureInfo culture) => GalleryResources.Get(nameof (UrlNotAllowedinPublisherProfileError), culture);

    public static string InvalidReCaptchaToken() => GalleryResources.Get(nameof (InvalidReCaptchaToken));

    public static string InvalidReCaptchaToken(CultureInfo culture) => GalleryResources.Get(nameof (InvalidReCaptchaToken), culture);

    public static string ReCaptchaPrivateKeyNotFound() => GalleryResources.Get(nameof (ReCaptchaPrivateKeyNotFound));

    public static string ReCaptchaPrivateKeyNotFound(CultureInfo culture) => GalleryResources.Get(nameof (ReCaptchaPrivateKeyNotFound), culture);

    public static string ReCaptchaStrongBoxItemNotFound() => GalleryResources.Get(nameof (ReCaptchaStrongBoxItemNotFound));

    public static string ReCaptchaStrongBoxItemNotFound(CultureInfo culture) => GalleryResources.Get(nameof (ReCaptchaStrongBoxItemNotFound), culture);

    public static string CSRAuthorIdentityMismatchException() => GalleryResources.Get(nameof (CSRAuthorIdentityMismatchException));

    public static string CSRAuthorIdentityMismatchException(CultureInfo culture) => GalleryResources.Get(nameof (CSRAuthorIdentityMismatchException), culture);

    public static string InvalidReporterVSIDException() => GalleryResources.Get(nameof (InvalidReporterVSIDException));

    public static string InvalidReporterVSIDException(CultureInfo culture) => GalleryResources.Get(nameof (InvalidReporterVSIDException), culture);

    public static string ErrorExtensionVersionAlreadyExists(object arg0) => GalleryResources.Format(nameof (ErrorExtensionVersionAlreadyExists), arg0);

    public static string ErrorExtensionVersionAlreadyExists(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ErrorExtensionVersionAlreadyExists), culture, arg0);

    public static string ErrorCreatePublisherNoRecaptchaToken() => GalleryResources.Get(nameof (ErrorCreatePublisherNoRecaptchaToken));

    public static string ErrorCreatePublisherNoRecaptchaToken(CultureInfo culture) => GalleryResources.Get(nameof (ErrorCreatePublisherNoRecaptchaToken), culture);

    public static string ErrorUpdatePublisherNoRecaptchaToken() => GalleryResources.Get(nameof (ErrorUpdatePublisherNoRecaptchaToken));

    public static string ErrorUpdatePublisherNoRecaptchaToken(CultureInfo culture) => GalleryResources.Get(nameof (ErrorUpdatePublisherNoRecaptchaToken), culture);

    public static string ErrorProductArchitectureForNonVsixVsExtension() => GalleryResources.Get(nameof (ErrorProductArchitectureForNonVsixVsExtension));

    public static string ErrorProductArchitectureForNonVsixVsExtension(CultureInfo culture) => GalleryResources.Get(nameof (ErrorProductArchitectureForNonVsixVsExtension), culture);

    public static string PlatformSpecificExtensionsNotSupportedForNonVSCodeExtensions() => GalleryResources.Get(nameof (PlatformSpecificExtensionsNotSupportedForNonVSCodeExtensions));

    public static string PlatformSpecificExtensionsNotSupportedForNonVSCodeExtensions(
      CultureInfo culture)
    {
      return GalleryResources.Get(nameof (PlatformSpecificExtensionsNotSupportedForNonVSCodeExtensions), culture);
    }

    public static string VSCodePlatformSpecificExtensionsDisabled() => GalleryResources.Get(nameof (VSCodePlatformSpecificExtensionsDisabled));

    public static string VSCodePlatformSpecificExtensionsDisabled(CultureInfo culture) => GalleryResources.Get(nameof (VSCodePlatformSpecificExtensionsDisabled), culture);

    public static string ErrorExtensionVersionWithTargetPlatformAlreadyExists(
      object arg0,
      object arg1,
      object arg2)
    {
      return GalleryResources.Format(nameof (ErrorExtensionVersionWithTargetPlatformAlreadyExists), arg0, arg1, arg2);
    }

    public static string ErrorExtensionVersionWithTargetPlatformAlreadyExists(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (ErrorExtensionVersionWithTargetPlatformAlreadyExists), culture, arg0, arg1, arg2);
    }

    public static string ErrorUnSupportedVSCodeTargetPlatformSupplied(object arg0) => GalleryResources.Format(nameof (ErrorUnSupportedVSCodeTargetPlatformSupplied), arg0);

    public static string ErrorUnSupportedVSCodeTargetPlatformSupplied(
      object arg0,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (ErrorUnSupportedVSCodeTargetPlatformSupplied), culture, arg0);
    }

    public static string ErrorChangingNonPlatformSpecificToPlatformSpecificVersion(
      object arg0,
      object arg1)
    {
      return GalleryResources.Format(nameof (ErrorChangingNonPlatformSpecificToPlatformSpecificVersion), arg0, arg1);
    }

    public static string ErrorChangingNonPlatformSpecificToPlatformSpecificVersion(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (ErrorChangingNonPlatformSpecificToPlatformSpecificVersion), culture, arg0, arg1);
    }

    public static string ErrorChangingPlatformSpecificToNonPlatformSpecificVersion(
      object arg0,
      object arg1)
    {
      return GalleryResources.Format(nameof (ErrorChangingPlatformSpecificToNonPlatformSpecificVersion), arg0, arg1);
    }

    public static string ErrorChangingPlatformSpecificToNonPlatformSpecificVersion(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (ErrorChangingPlatformSpecificToNonPlatformSpecificVersion), culture, arg0, arg1);
    }

    public static string ErrorExtensionVersionHasNoSupportForRequestedTargetPlatform(
      object arg0,
      object arg1,
      object arg2)
    {
      return GalleryResources.Format(nameof (ErrorExtensionVersionHasNoSupportForRequestedTargetPlatform), arg0, arg1, arg2);
    }

    public static string ErrorExtensionVersionHasNoSupportForRequestedTargetPlatform(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (ErrorExtensionVersionHasNoSupportForRequestedTargetPlatform), culture, arg0, arg1, arg2);
    }

    public static string InvalidVSCodeStatType() => GalleryResources.Get(nameof (InvalidVSCodeStatType));

    public static string InvalidVSCodeStatType(CultureInfo culture) => GalleryResources.Get(nameof (InvalidVSCodeStatType), culture);

    public static string InvalidItemName(object arg0) => GalleryResources.Format(nameof (InvalidItemName), arg0);

    public static string InvalidItemName(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidItemName), culture, arg0);

    public static string NotSupportedForNonVSCodeExtensions() => GalleryResources.Get(nameof (NotSupportedForNonVSCodeExtensions));

    public static string NotSupportedForNonVSCodeExtensions(CultureInfo culture) => GalleryResources.Get(nameof (NotSupportedForNonVSCodeExtensions), culture);

    public static string DomainURLIncorrect() => GalleryResources.Get(nameof (DomainURLIncorrect));

    public static string DomainURLIncorrect(CultureInfo culture) => GalleryResources.Get(nameof (DomainURLIncorrect), culture);

    public static string URLSizeTooBig() => GalleryResources.Get(nameof (URLSizeTooBig));

    public static string URLSizeTooBig(CultureInfo culture) => GalleryResources.Get(nameof (URLSizeTooBig), culture);

    public static string DomainAlreadyExistsForPublisherException() => GalleryResources.Get(nameof (DomainAlreadyExistsForPublisherException));

    public static string DomainAlreadyExistsForPublisherException(CultureInfo culture) => GalleryResources.Get(nameof (DomainAlreadyExistsForPublisherException), culture);

    public static string NoDomainExistsForPublisherException() => GalleryResources.Get(nameof (NoDomainExistsForPublisherException));

    public static string NoDomainExistsForPublisherException(CultureInfo culture) => GalleryResources.Get(nameof (NoDomainExistsForPublisherException), culture);

    public static string DomainCouldNotBeReached() => GalleryResources.Get(nameof (DomainCouldNotBeReached));

    public static string DomainCouldNotBeReached(CultureInfo culture) => GalleryResources.Get(nameof (DomainCouldNotBeReached), culture);

    public static string UrlHasPathParameters() => GalleryResources.Get(nameof (UrlHasPathParameters));

    public static string UrlHasPathParameters(CultureInfo culture) => GalleryResources.Get(nameof (UrlHasPathParameters), culture);

    public static string UrlSchemeIsNotHttps() => GalleryResources.Get(nameof (UrlSchemeIsNotHttps));

    public static string UrlSchemeIsNotHttps(CultureInfo culture) => GalleryResources.Get(nameof (UrlSchemeIsNotHttps), culture);

    public static string ExistingDomainAlreadyVerified(object arg0, object arg1) => GalleryResources.Format(nameof (ExistingDomainAlreadyVerified), arg0, arg1);

    public static string ExistingDomainAlreadyVerified(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (ExistingDomainAlreadyVerified), culture, arg0, arg1);
    }

    public static string NoDomainProvidedToAddOrUpdate() => GalleryResources.Get(nameof (NoDomainProvidedToAddOrUpdate));

    public static string NoDomainProvidedToAddOrUpdate(CultureInfo culture) => GalleryResources.Get(nameof (NoDomainProvidedToAddOrUpdate), culture);

    public static string UrlContainsSubdomain() => GalleryResources.Get(nameof (UrlContainsSubdomain));

    public static string UrlContainsSubdomain(CultureInfo culture) => GalleryResources.Get(nameof (UrlContainsSubdomain), culture);

    public static string ExtensionNameAlreadyExists(object arg0) => GalleryResources.Format(nameof (ExtensionNameAlreadyExists), arg0);

    public static string ExtensionNameAlreadyExists(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ExtensionNameAlreadyExists), culture, arg0);

    public static string ExtensionDisplayNameAlreadyExists(object arg0) => GalleryResources.Format(nameof (ExtensionDisplayNameAlreadyExists), arg0);

    public static string ExtensionDisplayNameAlreadyExists(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ExtensionDisplayNameAlreadyExists), culture, arg0);

    public static string PublisherDisplayNameAlreadyExists() => GalleryResources.Get(nameof (PublisherDisplayNameAlreadyExists));

    public static string PublisherDisplayNameAlreadyExists(CultureInfo culture) => GalleryResources.Get(nameof (PublisherDisplayNameAlreadyExists), culture);

    public static string SimilarPublisherDisplayNameExists(object arg0) => GalleryResources.Format(nameof (SimilarPublisherDisplayNameExists), arg0);

    public static string SimilarPublisherDisplayNameExists(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (SimilarPublisherDisplayNameExists), culture, arg0);

    public static string SimilarExtensionDisplayNameExists(object arg0) => GalleryResources.Format(nameof (SimilarExtensionDisplayNameExists), arg0);

    public static string SimilarExtensionDisplayNameExists(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (SimilarExtensionDisplayNameExists), culture, arg0);

    public static string VSCodeExtensionInvalidPricingTagException() => GalleryResources.Get(nameof (VSCodeExtensionInvalidPricingTagException));

    public static string VSCodeExtensionInvalidPricingTagException(CultureInfo culture) => GalleryResources.Get(nameof (VSCodeExtensionInvalidPricingTagException), culture);

    public static string ExtensionCreationLimitExceedException(object arg0) => GalleryResources.Format(nameof (ExtensionCreationLimitExceedException), arg0);

    public static string ExtensionCreationLimitExceedException(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ExtensionCreationLimitExceedException), culture, arg0);

    public static string MaintenanceMessage() => GalleryResources.Get(nameof (MaintenanceMessage));

    public static string MaintenanceMessage(CultureInfo culture) => GalleryResources.Get(nameof (MaintenanceMessage), culture);

    public static string VsixTypeVsExtensionVersionImmutabilityError(object arg0) => GalleryResources.Format(nameof (VsixTypeVsExtensionVersionImmutabilityError), arg0);

    public static string VsixTypeVsExtensionVersionImmutabilityError(
      object arg0,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (VsixTypeVsExtensionVersionImmutabilityError), culture, arg0);
    }

    public static string NonVsixTypeVsExtensionVersionImmutabilityError(object arg0) => GalleryResources.Format(nameof (NonVsixTypeVsExtensionVersionImmutabilityError), arg0);

    public static string NonVsixTypeVsExtensionVersionImmutabilityError(
      object arg0,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (NonVsixTypeVsExtensionVersionImmutabilityError), culture, arg0);
    }

    public static string EnablingVsixConsolidationWarningMessage() => GalleryResources.Get(nameof (EnablingVsixConsolidationWarningMessage));

    public static string EnablingVsixConsolidationWarningMessage(CultureInfo culture) => GalleryResources.Get(nameof (EnablingVsixConsolidationWarningMessage), culture);

    public static string VsixTypeConsolidatedVsExtensionVersionImmutablityError(object arg0) => GalleryResources.Format(nameof (VsixTypeConsolidatedVsExtensionVersionImmutablityError), arg0);

    public static string VsixTypeConsolidatedVsExtensionVersionImmutablityError(
      object arg0,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (VsixTypeConsolidatedVsExtensionVersionImmutablityError), culture, arg0);
    }

    public static string VsVersionNotAllowedForConsolidationException() => GalleryResources.Get(nameof (VsVersionNotAllowedForConsolidationException));

    public static string VsVersionNotAllowedForConsolidationException(CultureInfo culture) => GalleryResources.Get(nameof (VsVersionNotAllowedForConsolidationException), culture);

    public static string MustSupplyExtensionName(object arg0) => GalleryResources.Format(nameof (MustSupplyExtensionName), arg0);

    public static string MustSupplyExtensionName(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (MustSupplyExtensionName), culture, arg0);

    public static string InvalidExtensionNameSupplied(object arg0) => GalleryResources.Format(nameof (InvalidExtensionNameSupplied), arg0);

    public static string InvalidExtensionNameSupplied(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (InvalidExtensionNameSupplied), culture, arg0);

    public static string DuplicateEntryErrorMessage() => GalleryResources.Get(nameof (DuplicateEntryErrorMessage));

    public static string DuplicateEntryErrorMessage(CultureInfo culture) => GalleryResources.Get(nameof (DuplicateEntryErrorMessage), culture);

    public static string FutureModifyDateErrorMessage() => GalleryResources.Get(nameof (FutureModifyDateErrorMessage));

    public static string FutureModifyDateErrorMessage(CultureInfo culture) => GalleryResources.Get(nameof (FutureModifyDateErrorMessage), culture);

    public static string ManifestFileParsingErrorMessage() => GalleryResources.Get(nameof (ManifestFileParsingErrorMessage));

    public static string ManifestFileParsingErrorMessage(CultureInfo culture) => GalleryResources.Get(nameof (ManifestFileParsingErrorMessage), culture);

    public static string ZipSlipErrorMessage() => GalleryResources.Get(nameof (ZipSlipErrorMessage));

    public static string ZipSlipErrorMessage(CultureInfo culture) => GalleryResources.Get(nameof (ZipSlipErrorMessage), culture);

    public static string ZipBombErrorMessage() => GalleryResources.Get(nameof (ZipBombErrorMessage));

    public static string ZipBombErrorMessage(CultureInfo culture) => GalleryResources.Get(nameof (ZipBombErrorMessage), culture);

    public static string RepositorySigningFailedMessage() => GalleryResources.Get(nameof (RepositorySigningFailedMessage));

    public static string RepositorySigningFailedMessage(CultureInfo culture) => GalleryResources.Get(nameof (RepositorySigningFailedMessage), culture);

    public static string UnknownErrorInRepositorySigning() => GalleryResources.Get(nameof (UnknownErrorInRepositorySigning));

    public static string UnknownErrorInRepositorySigning(CultureInfo culture) => GalleryResources.Get(nameof (UnknownErrorInRepositorySigning), culture);

    public static string ExtensionTagsLimitExceedException(object arg0) => GalleryResources.Format(nameof (ExtensionTagsLimitExceedException), arg0);

    public static string ExtensionTagsLimitExceedException(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (ExtensionTagsLimitExceedException), culture, arg0);

    public static string InvalidPublisherName(object arg0, object arg1) => GalleryResources.Format(nameof (InvalidPublisherName), arg0, arg1);

    public static string InvalidPublisherName(object arg0, object arg1, CultureInfo culture) => GalleryResources.Format(nameof (InvalidPublisherName), culture, arg0, arg1);

    public static string ErrorDeprecatedVSCodeTargetPlatformSupplied(object arg0) => GalleryResources.Format(nameof (ErrorDeprecatedVSCodeTargetPlatformSupplied), arg0);

    public static string ErrorDeprecatedVSCodeTargetPlatformSupplied(
      object arg0,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (ErrorDeprecatedVSCodeTargetPlatformSupplied), culture, arg0);
    }

    public static string MultipartInvalidNumberOfPartsException(object arg0, object arg1) => GalleryResources.Format(nameof (MultipartInvalidNumberOfPartsException), arg0, arg1);

    public static string MultipartInvalidNumberOfPartsException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (MultipartInvalidNumberOfPartsException), culture, arg0, arg1);
    }

    public static string MultipartInvalidPartHeadersException() => GalleryResources.Get(nameof (MultipartInvalidPartHeadersException));

    public static string MultipartInvalidPartHeadersException(CultureInfo culture) => GalleryResources.Get(nameof (MultipartInvalidPartHeadersException), culture);

    public static string MultipartMaxSizeExceededException(object arg0) => GalleryResources.Format(nameof (MultipartMaxSizeExceededException), arg0);

    public static string MultipartMaxSizeExceededException(object arg0, CultureInfo culture) => GalleryResources.Format(nameof (MultipartMaxSizeExceededException), culture, arg0);

    public static string MultipartMissingPackageException() => GalleryResources.Get(nameof (MultipartMissingPackageException));

    public static string MultipartMissingPackageException(CultureInfo culture) => GalleryResources.Get(nameof (MultipartMissingPackageException), culture);

    public static string MultipartMissingSignatureArchiveException() => GalleryResources.Get(nameof (MultipartMissingSignatureArchiveException));

    public static string MultipartMissingSignatureArchiveException(CultureInfo culture) => GalleryResources.Get(nameof (MultipartMissingSignatureArchiveException), culture);

    public static string MultipartUnknownFileTypeException() => GalleryResources.Get(nameof (MultipartUnknownFileTypeException));

    public static string MultipartUnknownFileTypeException(CultureInfo culture) => GalleryResources.Get(nameof (MultipartUnknownFileTypeException), culture);

    public static string PublisherSignedExtensionsNotEnabledException() => GalleryResources.Get(nameof (PublisherSignedExtensionsNotEnabledException));

    public static string PublisherSignedExtensionsNotEnabledException(CultureInfo culture) => GalleryResources.Get(nameof (PublisherSignedExtensionsNotEnabledException), culture);

    public static string PublisherSignedInvalidExtensionTypeException(object arg0) => GalleryResources.Format(nameof (PublisherSignedInvalidExtensionTypeException), arg0);

    public static string PublisherSignedInvalidExtensionTypeException(
      object arg0,
      CultureInfo culture)
    {
      return GalleryResources.Format(nameof (PublisherSignedInvalidExtensionTypeException), culture, arg0);
    }

    public static string PublisherSignedInvalidMediaTypeException() => GalleryResources.Get(nameof (PublisherSignedInvalidMediaTypeException));

    public static string PublisherSignedInvalidMediaTypeException(CultureInfo culture) => GalleryResources.Get(nameof (PublisherSignedInvalidMediaTypeException), culture);

    public static string PublisherSignedNotSupportedOnPremException() => GalleryResources.Get(nameof (PublisherSignedNotSupportedOnPremException));

    public static string PublisherSignedNotSupportedOnPremException(CultureInfo culture) => GalleryResources.Get(nameof (PublisherSignedNotSupportedOnPremException), culture);

    public static string PublisherSignedOnlyFirstPartyExtensionsAllowedException() => GalleryResources.Get(nameof (PublisherSignedOnlyFirstPartyExtensionsAllowedException));

    public static string PublisherSignedOnlyFirstPartyExtensionsAllowedException(CultureInfo culture) => GalleryResources.Get(nameof (PublisherSignedOnlyFirstPartyExtensionsAllowedException), culture);
  }
}
