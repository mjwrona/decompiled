// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.HostingResources
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class HostingResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (HostingResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => HostingResources.s_resMgr;

    private static string Get(string resourceName) => HostingResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? HostingResources.Get(resourceName) : HostingResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) HostingResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? HostingResources.GetInt(resourceName) : (int) HostingResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) HostingResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? HostingResources.GetBool(resourceName) : (bool) HostingResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => HostingResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = HostingResources.Get(resourceName, culture);
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

    public static string AccountPerSubscriptionLimitMet(object arg0, object arg1) => HostingResources.Format(nameof (AccountPerSubscriptionLimitMet), arg0, arg1);

    public static string AccountPerSubscriptionLimitMet(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (AccountPerSubscriptionLimitMet), culture, arg0, arg1);
    }

    public static string AzureManageDevServicesLink(object arg0) => HostingResources.Format(nameof (AzureManageDevServicesLink), arg0);

    public static string AzureManageDevServicesLink(object arg0, CultureInfo culture) => HostingResources.Format(nameof (AzureManageDevServicesLink), culture, arg0);

    public static string AzureSubscriptionSignupLink(object arg0) => HostingResources.Format(nameof (AzureSubscriptionSignupLink), arg0);

    public static string AzureSubscriptionSignupLink(object arg0, CultureInfo culture) => HostingResources.Format(nameof (AzureSubscriptionSignupLink), culture, arg0);

    public static string AzureSubscriptionsPageLink(object arg0) => HostingResources.Format(nameof (AzureSubscriptionsPageLink), arg0);

    public static string AzureSubscriptionsPageLink(object arg0, CultureInfo culture) => HostingResources.Format(nameof (AzureSubscriptionsPageLink), culture, arg0);

    public static string MissingServiceIdentity() => HostingResources.Get(nameof (MissingServiceIdentity));

    public static string MissingServiceIdentity(CultureInfo culture) => HostingResources.Get(nameof (MissingServiceIdentity), culture);

    public static string InvalidEntityState(object arg0) => HostingResources.Format(nameof (InvalidEntityState), arg0);

    public static string InvalidEntityState(object arg0, CultureInfo culture) => HostingResources.Format(nameof (InvalidEntityState), culture, arg0);

    public static string InvalidOperationType(object arg0) => HostingResources.Format(nameof (InvalidOperationType), arg0);

    public static string InvalidOperationType(object arg0, CultureInfo culture) => HostingResources.Format(nameof (InvalidOperationType), culture, arg0);

    public static string AccountLinkingUnlinkingFailed(object arg0) => HostingResources.Format(nameof (AccountLinkingUnlinkingFailed), arg0);

    public static string AccountLinkingUnlinkingFailed(object arg0, CultureInfo culture) => HostingResources.Format(nameof (AccountLinkingUnlinkingFailed), culture, arg0);

    public static string InvalidResourceType(object arg0) => HostingResources.Format(nameof (InvalidResourceType), arg0);

    public static string InvalidResourceType(object arg0, CultureInfo culture) => HostingResources.Format(nameof (InvalidResourceType), culture, arg0);

    public static string MissingRequiredIntrinsicSetting(object arg0, object arg1) => HostingResources.Format(nameof (MissingRequiredIntrinsicSetting), arg0, arg1);

    public static string MissingRequiredIntrinsicSetting(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (MissingRequiredIntrinsicSetting), culture, arg0, arg1);
    }

    public static string MissingRequiredUserInfo(object arg0) => HostingResources.Format(nameof (MissingRequiredUserInfo), arg0);

    public static string MissingRequiredUserInfo(object arg0, CultureInfo culture) => HostingResources.Format(nameof (MissingRequiredUserInfo), culture, arg0);

    public static string NonActiveSubscriptionStatus(object arg0, object arg1) => HostingResources.Format(nameof (NonActiveSubscriptionStatus), arg0, arg1);

    public static string NonActiveSubscriptionStatus(object arg0, object arg1, CultureInfo culture) => HostingResources.Format(nameof (NonActiveSubscriptionStatus), culture, arg0, arg1);

    public static string OneOfTwoRequiredArgumentsIsMissing(object arg0, object arg1) => HostingResources.Format(nameof (OneOfTwoRequiredArgumentsIsMissing), arg0, arg1);

    public static string OneOfTwoRequiredArgumentsIsMissing(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (OneOfTwoRequiredArgumentsIsMissing), culture, arg0, arg1);
    }

    public static string QueryParameterMissing(object arg0) => HostingResources.Format(nameof (QueryParameterMissing), arg0);

    public static string QueryParameterMissing(object arg0, CultureInfo culture) => HostingResources.Format(nameof (QueryParameterMissing), culture, arg0);

    public static string SubscriptionNotFound(object arg0) => HostingResources.Format(nameof (SubscriptionNotFound), arg0);

    public static string SubscriptionNotFound(object arg0, CultureInfo culture) => HostingResources.Format(nameof (SubscriptionNotFound), culture, arg0);

    public static string AccountDoesNotExist(object arg0) => HostingResources.Format(nameof (AccountDoesNotExist), arg0);

    public static string AccountDoesNotExist(object arg0, CultureInfo culture) => HostingResources.Format(nameof (AccountDoesNotExist), culture, arg0);

    public static string UserNotAccountAdministrator(object arg0, object arg1) => HostingResources.Format(nameof (UserNotAccountAdministrator), arg0, arg1);

    public static string UserNotAccountAdministrator(object arg0, object arg1, CultureInfo culture) => HostingResources.Format(nameof (UserNotAccountAdministrator), culture, arg0, arg1);

    public static string AccountAlreadyLinked(object arg0) => HostingResources.Format(nameof (AccountAlreadyLinked), arg0);

    public static string AccountAlreadyLinked(object arg0, CultureInfo culture) => HostingResources.Format(nameof (AccountAlreadyLinked), culture, arg0);

    public static string RdfeClientCertNotMatch(object arg0, object arg1) => HostingResources.Format(nameof (RdfeClientCertNotMatch), arg0, arg1);

    public static string RdfeClientCertNotMatch(object arg0, object arg1, CultureInfo culture) => HostingResources.Format(nameof (RdfeClientCertNotMatch), culture, arg0, arg1);

    public static string ResourceIsUnusable(object arg0, object arg1) => HostingResources.Format(nameof (ResourceIsUnusable), arg0, arg1);

    public static string ResourceIsUnusable(object arg0, object arg1, CultureInfo culture) => HostingResources.Format(nameof (ResourceIsUnusable), culture, arg0, arg1);

    public static string InvalidCommitmentResourceQuantity(object arg0) => HostingResources.Format(nameof (InvalidCommitmentResourceQuantity), arg0);

    public static string InvalidCommitmentResourceQuantity(object arg0, CultureInfo culture) => HostingResources.Format(nameof (InvalidCommitmentResourceQuantity), culture, arg0);

    public static string InvalidUsageQuantity(object arg0) => HostingResources.Format(nameof (InvalidUsageQuantity), arg0);

    public static string InvalidUsageQuantity(object arg0, CultureInfo culture) => HostingResources.Format(nameof (InvalidUsageQuantity), culture, arg0);

    public static string InvalidAccountQuantities(object arg0) => HostingResources.Format(nameof (InvalidAccountQuantities), arg0);

    public static string InvalidAccountQuantities(object arg0, CultureInfo culture) => HostingResources.Format(nameof (InvalidAccountQuantities), culture, arg0);

    public static string UserContextNullReferenceExceptionMessage() => HostingResources.Get(nameof (UserContextNullReferenceExceptionMessage));

    public static string UserContextNullReferenceExceptionMessage(CultureInfo culture) => HostingResources.Get(nameof (UserContextNullReferenceExceptionMessage), culture);

    public static string AzureExtensionLink(object arg0) => HostingResources.Format(nameof (AzureExtensionLink), arg0);

    public static string AzureExtensionLink(object arg0, CultureInfo culture) => HostingResources.Format(nameof (AzureExtensionLink), culture, arg0);

    public static string AzureScaleTabDeepLink(object arg0) => HostingResources.Format(nameof (AzureScaleTabDeepLink), arg0);

    public static string AzureScaleTabDeepLink(object arg0, CultureInfo culture) => HostingResources.Format(nameof (AzureScaleTabDeepLink), culture, arg0);

    public static string SubscriptionNotLinkedToAccount(object arg0, object arg1, object arg2) => HostingResources.Format(nameof (SubscriptionNotLinkedToAccount), arg0, arg1, arg2);

    public static string SubscriptionNotLinkedToAccount(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (SubscriptionNotLinkedToAccount), culture, arg0, arg1, arg2);
    }

    public static string MissingRequiredProperty(object arg0) => HostingResources.Format(nameof (MissingRequiredProperty), arg0);

    public static string MissingRequiredProperty(object arg0, CultureInfo culture) => HostingResources.Format(nameof (MissingRequiredProperty), culture, arg0);

    public static string VSOExtensionOrBundlePurchaseEmailSubject() => HostingResources.Get(nameof (VSOExtensionOrBundlePurchaseEmailSubject));

    public static string VSOExtensionOrBundlePurchaseEmailSubject(CultureInfo culture) => HostingResources.Get(nameof (VSOExtensionOrBundlePurchaseEmailSubject), culture);

    public static string PurchaseRequestEmailSubject() => HostingResources.Get(nameof (PurchaseRequestEmailSubject));

    public static string PurchaseRequestEmailSubject(CultureInfo culture) => HostingResources.Get(nameof (PurchaseRequestEmailSubject), culture);

    public static string UserIsNotSubscriptionAdmin() => HostingResources.Get(nameof (UserIsNotSubscriptionAdmin));

    public static string UserIsNotSubscriptionAdmin(CultureInfo culture) => HostingResources.Get(nameof (UserIsNotSubscriptionAdmin), culture);

    public static string VSOExtensionEmailTemplate() => HostingResources.Get(nameof (VSOExtensionEmailTemplate));

    public static string VSOExtensionEmailTemplate(CultureInfo culture) => HostingResources.Get(nameof (VSOExtensionEmailTemplate), culture);

    public static string VSOExtensionAnnualEmailTemplate() => HostingResources.Get(nameof (VSOExtensionAnnualEmailTemplate));

    public static string VSOExtensionAnnualEmailTemplate(CultureInfo culture) => HostingResources.Get(nameof (VSOExtensionAnnualEmailTemplate), culture);

    public static string VSOSubscriptionMonthlyEmailTemplate() => HostingResources.Get(nameof (VSOSubscriptionMonthlyEmailTemplate));

    public static string VSOSubscriptionMonthlyEmailTemplate(CultureInfo culture) => HostingResources.Get(nameof (VSOSubscriptionMonthlyEmailTemplate), culture);

    public static string HockeyAppEmailTemplate() => HostingResources.Get(nameof (HockeyAppEmailTemplate));

    public static string HockeyAppEmailTemplate(CultureInfo culture) => HostingResources.Get(nameof (HockeyAppEmailTemplate), culture);

    public static string XamarinUniversityEmailTemplate() => HostingResources.Get(nameof (XamarinUniversityEmailTemplate));

    public static string XamarinUniversityEmailTemplate(CultureInfo culture) => HostingResources.Get(nameof (XamarinUniversityEmailTemplate), culture);

    public static string OfferMeterNotFound(object arg0) => HostingResources.Format(nameof (OfferMeterNotFound), arg0);

    public static string OfferMeterNotFound(object arg0, CultureInfo culture) => HostingResources.Format(nameof (OfferMeterNotFound), culture, arg0);

    public static string VSOExtensionTrialStartEmailTemplate() => HostingResources.Get(nameof (VSOExtensionTrialStartEmailTemplate));

    public static string VSOExtensionTrialStartEmailTemplate(CultureInfo culture) => HostingResources.Get(nameof (VSOExtensionTrialStartEmailTemplate), culture);

    public static string VSOExtensionTrialStartEmailSubject(object arg0) => HostingResources.Format(nameof (VSOExtensionTrialStartEmailSubject), arg0);

    public static string VSOExtensionTrialStartEmailSubject(object arg0, CultureInfo culture) => HostingResources.Format(nameof (VSOExtensionTrialStartEmailSubject), culture, arg0);

    public static string VSOExtensionTrialExpiryInSevenDaysEmailTemplate() => HostingResources.Get(nameof (VSOExtensionTrialExpiryInSevenDaysEmailTemplate));

    public static string VSOExtensionTrialExpiryInSevenDaysEmailTemplate(CultureInfo culture) => HostingResources.Get(nameof (VSOExtensionTrialExpiryInSevenDaysEmailTemplate), culture);

    public static string VSOExtensionTrialExpiryInSevenDaysWithIncludedQuantityEmailTemplate() => HostingResources.Get(nameof (VSOExtensionTrialExpiryInSevenDaysWithIncludedQuantityEmailTemplate));

    public static string VSOExtensionTrialExpiryInSevenDaysWithIncludedQuantityEmailTemplate(
      CultureInfo culture)
    {
      return HostingResources.Get(nameof (VSOExtensionTrialExpiryInSevenDaysWithIncludedQuantityEmailTemplate), culture);
    }

    public static string VSOExtensionTrialExpiryInSevenDaysEmailSubject(object arg0) => HostingResources.Format(nameof (VSOExtensionTrialExpiryInSevenDaysEmailSubject), arg0);

    public static string VSOExtensionTrialExpiryInSevenDaysEmailSubject(
      object arg0,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (VSOExtensionTrialExpiryInSevenDaysEmailSubject), culture, arg0);
    }

    public static string VSOExtensionTrialExpiredEmailTemplate() => HostingResources.Get(nameof (VSOExtensionTrialExpiredEmailTemplate));

    public static string VSOExtensionTrialExpiredEmailTemplate(CultureInfo culture) => HostingResources.Get(nameof (VSOExtensionTrialExpiredEmailTemplate), culture);

    public static string VSOExtensionTrialExpiredWithIncludedQuantityEmailTemplate() => HostingResources.Get(nameof (VSOExtensionTrialExpiredWithIncludedQuantityEmailTemplate));

    public static string VSOExtensionTrialExpiredWithIncludedQuantityEmailTemplate(
      CultureInfo culture)
    {
      return HostingResources.Get(nameof (VSOExtensionTrialExpiredWithIncludedQuantityEmailTemplate), culture);
    }

    public static string VSOExtensionTrialExpiredEmailSubject(object arg0) => HostingResources.Format(nameof (VSOExtensionTrialExpiredEmailSubject), arg0);

    public static string VSOExtensionTrialExpiredEmailSubject(object arg0, CultureInfo culture) => HostingResources.Format(nameof (VSOExtensionTrialExpiredEmailSubject), culture, arg0);

    public static string VSOBundlePurchaseRenewalEmailTemplate() => HostingResources.Get(nameof (VSOBundlePurchaseRenewalEmailTemplate));

    public static string VSOBundlePurchaseRenewalEmailTemplate(CultureInfo culture) => HostingResources.Get(nameof (VSOBundlePurchaseRenewalEmailTemplate), culture);

    public static string VSOBundlePurchaseRenewalEmailSubject() => HostingResources.Get(nameof (VSOBundlePurchaseRenewalEmailSubject));

    public static string VSOBundlePurchaseRenewalEmailSubject(CultureInfo culture) => HostingResources.Get(nameof (VSOBundlePurchaseRenewalEmailSubject), culture);

    public static string ErrorAddingPlans(object arg0) => HostingResources.Format(nameof (ErrorAddingPlans), arg0);

    public static string ErrorAddingPlans(object arg0, CultureInfo culture) => HostingResources.Format(nameof (ErrorAddingPlans), culture, arg0);

    public static string GalleryNameNotConstructed() => HostingResources.Get(nameof (GalleryNameNotConstructed));

    public static string GalleryNameNotConstructed(CultureInfo culture) => HostingResources.Get(nameof (GalleryNameNotConstructed), culture);

    public static string MissingGalleryItemController(object arg0) => HostingResources.Format(nameof (MissingGalleryItemController), arg0);

    public static string MissingGalleryItemController(object arg0, CultureInfo culture) => HostingResources.Format(nameof (MissingGalleryItemController), culture, arg0);

    public static string Success() => HostingResources.Get(nameof (Success));

    public static string Success(CultureInfo culture) => HostingResources.Get(nameof (Success), culture);

    public static string NoAccountRelationshipExists() => HostingResources.Get(nameof (NoAccountRelationshipExists));

    public static string NoAccountRelationshipExists(CultureInfo culture) => HostingResources.Get(nameof (NoAccountRelationshipExists), culture);

    public static string ResourceNotPayAsYouGo() => HostingResources.Get(nameof (ResourceNotPayAsYouGo));

    public static string ResourceNotPayAsYouGo(CultureInfo culture) => HostingResources.Get(nameof (ResourceNotPayAsYouGo), culture);

    public static string MarketplaceOfferControllerError(object arg0, object arg1) => HostingResources.Format(nameof (MarketplaceOfferControllerError), arg0, arg1);

    public static string MarketplaceOfferControllerError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (MarketplaceOfferControllerError), culture, arg0, arg1);
    }

    public static string AccountHasThirdPartyExtensionsException(object arg0) => HostingResources.Format(nameof (AccountHasThirdPartyExtensionsException), arg0);

    public static string AccountHasThirdPartyExtensionsException(object arg0, CultureInfo culture) => HostingResources.Format(nameof (AccountHasThirdPartyExtensionsException), culture, arg0);

    public static string PricingDataNotAvailable() => HostingResources.Get(nameof (PricingDataNotAvailable));

    public static string PricingDataNotAvailable(CultureInfo culture) => HostingResources.Get(nameof (PricingDataNotAvailable), culture);

    public static string UnknownArmResponseError() => HostingResources.Get(nameof (UnknownArmResponseError));

    public static string UnknownArmResponseError(CultureInfo culture) => HostingResources.Get(nameof (UnknownArmResponseError), culture);

    public static string AccountNameReserved() => HostingResources.Get(nameof (AccountNameReserved));

    public static string AccountNameReserved(CultureInfo culture) => HostingResources.Get(nameof (AccountNameReserved), culture);

    public static string RegionNotSupported(object arg0) => HostingResources.Format(nameof (RegionNotSupported), arg0);

    public static string RegionNotSupported(object arg0, CultureInfo culture) => HostingResources.Format(nameof (RegionNotSupported), culture, arg0);

    public static string UserIsNotAMemberOfTenant(object arg0, object arg1) => HostingResources.Format(nameof (UserIsNotAMemberOfTenant), arg0, arg1);

    public static string UserIsNotAMemberOfTenant(object arg0, object arg1, CultureInfo culture) => HostingResources.Format(nameof (UserIsNotAMemberOfTenant), culture, arg0, arg1);

    public static string VSOPurchaseRequestEmailForUnlinkedAccounts() => HostingResources.Get(nameof (VSOPurchaseRequestEmailForUnlinkedAccounts));

    public static string VSOPurchaseRequestEmailForUnlinkedAccounts(CultureInfo culture) => HostingResources.Get(nameof (VSOPurchaseRequestEmailForUnlinkedAccounts), culture);

    public static string VSOPurchaseRequestEmailForLinkedAccounts() => HostingResources.Get(nameof (VSOPurchaseRequestEmailForLinkedAccounts));

    public static string VSOPurchaseRequestEmailForLinkedAccounts(CultureInfo culture) => HostingResources.Get(nameof (VSOPurchaseRequestEmailForLinkedAccounts), culture);

    public static string RequestPurchaseOnUnlinkedAccountExceptionMessage() => HostingResources.Get(nameof (RequestPurchaseOnUnlinkedAccountExceptionMessage));

    public static string RequestPurchaseOnUnlinkedAccountExceptionMessage(CultureInfo culture) => HostingResources.Get(nameof (RequestPurchaseOnUnlinkedAccountExceptionMessage), culture);

    public static string AzureUnauthorizedAccessException() => HostingResources.Get(nameof (AzureUnauthorizedAccessException));

    public static string AzureUnauthorizedAccessException(CultureInfo culture) => HostingResources.Get(nameof (AzureUnauthorizedAccessException), culture);

    public static string CancellingThirdPartyExtensionDueToChangeSubscription() => HostingResources.Get(nameof (CancellingThirdPartyExtensionDueToChangeSubscription));

    public static string CancellingThirdPartyExtensionDueToChangeSubscription(CultureInfo culture) => HostingResources.Get(nameof (CancellingThirdPartyExtensionDueToChangeSubscription), culture);

    public static string OfferSubscriptionNotFound() => HostingResources.Get(nameof (OfferSubscriptionNotFound));

    public static string OfferSubscriptionNotFound(CultureInfo culture) => HostingResources.Get(nameof (OfferSubscriptionNotFound), culture);

    public static string UnexpectedServiceException(object arg0, object arg1) => HostingResources.Format(nameof (UnexpectedServiceException), arg0, arg1);

    public static string UnexpectedServiceException(object arg0, object arg1, CultureInfo culture) => HostingResources.Format(nameof (UnexpectedServiceException), culture, arg0, arg1);

    public static string FailedToGetAccountDetailsFor0(object arg0) => HostingResources.Format(nameof (FailedToGetAccountDetailsFor0), arg0);

    public static string FailedToGetAccountDetailsFor0(object arg0, CultureInfo culture) => HostingResources.Format(nameof (FailedToGetAccountDetailsFor0), culture, arg0);

    public static string InvalidResourceIdentifier0(object arg0) => HostingResources.Format(nameof (InvalidResourceIdentifier0), arg0);

    public static string InvalidResourceIdentifier0(object arg0, CultureInfo culture) => HostingResources.Format(nameof (InvalidResourceIdentifier0), culture, arg0);

    public static string MoveNotSupported() => HostingResources.Get(nameof (MoveNotSupported));

    public static string MoveNotSupported(CultureInfo culture) => HostingResources.Get(nameof (MoveNotSupported), culture);

    public static string OnlyOneAccountCanBeMovedAtATime() => HostingResources.Get(nameof (OnlyOneAccountCanBeMovedAtATime));

    public static string OnlyOneAccountCanBeMovedAtATime(CultureInfo culture) => HostingResources.Get(nameof (OnlyOneAccountCanBeMovedAtATime), culture);

    public static string OperationName0InvalidOrNotSupported(object arg0) => HostingResources.Format(nameof (OperationName0InvalidOrNotSupported), arg0);

    public static string OperationName0InvalidOrNotSupported(object arg0, CultureInfo culture) => HostingResources.Format(nameof (OperationName0InvalidOrNotSupported), culture, arg0);

    public static string ResourcesToMoveDoesNotContainAnyAccount() => HostingResources.Get(nameof (ResourcesToMoveDoesNotContainAnyAccount));

    public static string ResourcesToMoveDoesNotContainAnyAccount(CultureInfo culture) => HostingResources.Get(nameof (ResourcesToMoveDoesNotContainAnyAccount), culture);

    public static string SubscriptionId0OrResourceGroup1DoesNotMatch(object arg0, object arg1) => HostingResources.Format(nameof (SubscriptionId0OrResourceGroup1DoesNotMatch), arg0, arg1);

    public static string SubscriptionId0OrResourceGroup1DoesNotMatch(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (SubscriptionId0OrResourceGroup1DoesNotMatch), culture, arg0, arg1);
    }

    public static string AtleastOneResourceNeedToBeSpecified() => HostingResources.Get(nameof (AtleastOneResourceNeedToBeSpecified));

    public static string AtleastOneResourceNeedToBeSpecified(CultureInfo culture) => HostingResources.Get(nameof (AtleastOneResourceNeedToBeSpecified), culture);

    public static string InvalidMoveRequestErrorDetails0(object arg0) => HostingResources.Format(nameof (InvalidMoveRequestErrorDetails0), arg0);

    public static string InvalidMoveRequestErrorDetails0(object arg0, CultureInfo culture) => HostingResources.Format(nameof (InvalidMoveRequestErrorDetails0), culture, arg0);

    public static string ResourceGroupNameCannotBeChangedWhileMovingResourcesSourceRG0TargetRG1(
      object arg0,
      object arg1)
    {
      return HostingResources.Format(nameof (ResourceGroupNameCannotBeChangedWhileMovingResourcesSourceRG0TargetRG1), arg0, arg1);
    }

    public static string ResourceGroupNameCannotBeChangedWhileMovingResourcesSourceRG0TargetRG1(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (ResourceGroupNameCannotBeChangedWhileMovingResourcesSourceRG0TargetRG1), culture, arg0, arg1);
    }

    public static string TargetResourceGroupId0IsInvalid(object arg0) => HostingResources.Format(nameof (TargetResourceGroupId0IsInvalid), arg0);

    public static string TargetResourceGroupId0IsInvalid(object arg0, CultureInfo culture) => HostingResources.Format(nameof (TargetResourceGroupId0IsInvalid), culture, arg0);

    public static string Account0NotFoundInResourceGroup1InSubscription2(
      object arg0,
      object arg1,
      object arg2)
    {
      return HostingResources.Format(nameof (Account0NotFoundInResourceGroup1InSubscription2), arg0, arg1, arg2);
    }

    public static string Account0NotFoundInResourceGroup1InSubscription2(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (Account0NotFoundInResourceGroup1InSubscription2), culture, arg0, arg1, arg2);
    }

    public static string RiskEvaluationRejectException() => HostingResources.Get(nameof (RiskEvaluationRejectException));

    public static string RiskEvaluationRejectException(CultureInfo culture) => HostingResources.Get(nameof (RiskEvaluationRejectException), culture);
  }
}
