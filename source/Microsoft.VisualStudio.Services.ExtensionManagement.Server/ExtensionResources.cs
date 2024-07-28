// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.ExtensionResources
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ExtensionManagement
{
  internal static class ExtensionResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (ExtensionResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ExtensionResources.s_resMgr;

    private static string Get(string resourceName) => ExtensionResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ExtensionResources.Get(resourceName) : ExtensionResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ExtensionResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ExtensionResources.GetInt(resourceName) : (int) ExtensionResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ExtensionResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ExtensionResources.GetBool(resourceName) : (bool) ExtensionResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ExtensionResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ExtensionResources.Get(resourceName, culture);
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

    public static string BuyRedirectText() => ExtensionResources.Get(nameof (BuyRedirectText));

    public static string BuyRedirectText(CultureInfo culture) => ExtensionResources.Get(nameof (BuyRedirectText), culture);

    public static string CannotAuthorizeTrustedExtension() => ExtensionResources.Get(nameof (CannotAuthorizeTrustedExtension));

    public static string CannotAuthorizeTrustedExtension(CultureInfo culture) => ExtensionResources.Get(nameof (CannotAuthorizeTrustedExtension), culture);

    public static string CannotGetPoliciesForOtherUserThanMe() => ExtensionResources.Get(nameof (CannotGetPoliciesForOtherUserThanMe));

    public static string CannotGetPoliciesForOtherUserThanMe(CultureInfo culture) => ExtensionResources.Get(nameof (CannotGetPoliciesForOtherUserThanMe), culture);

    public static string CannotUpdateBuiltinExtension(object arg0) => ExtensionResources.Format(nameof (CannotUpdateBuiltinExtension), arg0);

    public static string CannotUpdateBuiltinExtension(object arg0, CultureInfo culture) => ExtensionResources.Format(nameof (CannotUpdateBuiltinExtension), culture, arg0);

    public static string CannotUpdateExtensionNeedingReauthorization(object arg0) => ExtensionResources.Format(nameof (CannotUpdateExtensionNeedingReauthorization), arg0);

    public static string CannotUpdateExtensionNeedingReauthorization(
      object arg0,
      CultureInfo culture)
    {
      return ExtensionResources.Format(nameof (CannotUpdateExtensionNeedingReauthorization), culture, arg0);
    }

    public static string CollectionNameExceedsLimit(object arg0, object arg1) => ExtensionResources.Format(nameof (CollectionNameExceedsLimit), arg0, arg1);

    public static string CollectionNameExceedsLimit(object arg0, object arg1, CultureInfo culture) => ExtensionResources.Format(nameof (CollectionNameExceedsLimit), culture, arg0, arg1);

    public static string DemandExtensionFailedFormat(object arg0) => ExtensionResources.Format(nameof (DemandExtensionFailedFormat), arg0);

    public static string DemandExtensionFailedFormat(object arg0, CultureInfo culture) => ExtensionResources.Format(nameof (DemandExtensionFailedFormat), culture, arg0);

    public static string ExtensionAlreadyInstalled() => ExtensionResources.Get(nameof (ExtensionAlreadyInstalled));

    public static string ExtensionAlreadyInstalled(CultureInfo culture) => ExtensionResources.Get(nameof (ExtensionAlreadyInstalled), culture);

    public static string OfferSubscriptionNotAvailable() => ExtensionResources.Get(nameof (OfferSubscriptionNotAvailable));

    public static string OfferSubscriptionNotAvailable(CultureInfo culture) => ExtensionResources.Get(nameof (OfferSubscriptionNotAvailable), culture);

    public static string TrialNotPossible() => ExtensionResources.Get(nameof (TrialNotPossible));

    public static string TrialNotPossible(CultureInfo culture) => ExtensionResources.Get(nameof (TrialNotPossible), culture);

    public static string TrialDisallowedExtensionAlreadyPurchased(object arg0) => ExtensionResources.Format(nameof (TrialDisallowedExtensionAlreadyPurchased), arg0);

    public static string TrialDisallowedExtensionAlreadyPurchased(object arg0, CultureInfo culture) => ExtensionResources.Format(nameof (TrialDisallowedExtensionAlreadyPurchased), culture, arg0);

    public static string ExtensionAlreadyRequested() => ExtensionResources.Get(nameof (ExtensionAlreadyRequested));

    public static string ExtensionAlreadyRequested(CultureInfo culture) => ExtensionResources.Get(nameof (ExtensionAlreadyRequested), culture);

    public static string UserDoesNotHavePermissionToInstall() => ExtensionResources.Get(nameof (UserDoesNotHavePermissionToInstall));

    public static string UserDoesNotHavePermissionToInstall(CultureInfo culture) => ExtensionResources.Get(nameof (UserDoesNotHavePermissionToInstall), culture);

    public static string UserCanNotRequest() => ExtensionResources.Get(nameof (UserCanNotRequest));

    public static string UserCanNotRequest(CultureInfo culture) => ExtensionResources.Get(nameof (UserCanNotRequest), culture);

    public static string ExtensionAlreadyUnderTrial(object arg0) => ExtensionResources.Format(nameof (ExtensionAlreadyUnderTrial), arg0);

    public static string ExtensionAlreadyUnderTrial(object arg0, CultureInfo culture) => ExtensionResources.Format(nameof (ExtensionAlreadyUnderTrial), culture, arg0);

    public static string ExtensionAlreadyUnderTrialIndefinite() => ExtensionResources.Get(nameof (ExtensionAlreadyUnderTrialIndefinite));

    public static string ExtensionAlreadyUnderTrialIndefinite(CultureInfo culture) => ExtensionResources.Get(nameof (ExtensionAlreadyUnderTrialIndefinite), culture);

    public static string ExtensionDoesNotExist() => ExtensionResources.Get(nameof (ExtensionDoesNotExist));

    public static string ExtensionDoesNotExist(CultureInfo culture) => ExtensionResources.Get(nameof (ExtensionDoesNotExist), culture);

    public static string ExtensionDoesNotExistAtVersion() => ExtensionResources.Get(nameof (ExtensionDoesNotExistAtVersion));

    public static string ExtensionDoesNotExistAtVersion(CultureInfo culture) => ExtensionResources.Get(nameof (ExtensionDoesNotExistAtVersion), culture);

    public static string ExtensionEventCallbackDeniedMessageFormat(object arg0) => ExtensionResources.Format(nameof (ExtensionEventCallbackDeniedMessageFormat), arg0);

    public static string ExtensionEventCallbackDeniedMessageFormat(object arg0, CultureInfo culture) => ExtensionResources.Format(nameof (ExtensionEventCallbackDeniedMessageFormat), culture, arg0);

    public static string ExtensionEventCallbackDeniedNoReason() => ExtensionResources.Get(nameof (ExtensionEventCallbackDeniedNoReason));

    public static string ExtensionEventCallbackDeniedNoReason(CultureInfo culture) => ExtensionResources.Get(nameof (ExtensionEventCallbackDeniedNoReason), culture);

    public static string ExtensionEventCallbackDeniedNoResult() => ExtensionResources.Get(nameof (ExtensionEventCallbackDeniedNoResult));

    public static string ExtensionEventCallbackDeniedNoResult(CultureInfo culture) => ExtensionResources.Get(nameof (ExtensionEventCallbackDeniedNoResult), culture);

    public static string ExtensionNotAvailableInAccountRegion(object arg0) => ExtensionResources.Format(nameof (ExtensionNotAvailableInAccountRegion), arg0);

    public static string ExtensionNotAvailableInAccountRegion(object arg0, CultureInfo culture) => ExtensionResources.Format(nameof (ExtensionNotAvailableInAccountRegion), culture, arg0);

    public static string ExtensionTrialExpired(object arg0, object arg1, object arg2) => ExtensionResources.Format(nameof (ExtensionTrialExpired), arg0, arg1, arg2);

    public static string ExtensionTrialExpired(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ExtensionResources.Format(nameof (ExtensionTrialExpired), culture, arg0, arg1, arg2);
    }

    public static string OnPremisesNotSupported() => ExtensionResources.Get(nameof (OnPremisesNotSupported));

    public static string OnPremisesNotSupported(CultureInfo culture) => ExtensionResources.Get(nameof (OnPremisesNotSupported), culture);

    public static string NoPublicOfferPlans() => ExtensionResources.Get(nameof (NoPublicOfferPlans));

    public static string NoPublicOfferPlans(CultureInfo culture) => ExtensionResources.Get(nameof (NoPublicOfferPlans), culture);

    public static string TrialDisalloweBillingNotStarted(object arg0) => ExtensionResources.Format(nameof (TrialDisalloweBillingNotStarted), arg0);

    public static string TrialDisalloweBillingNotStarted(object arg0, CultureInfo culture) => ExtensionResources.Format(nameof (TrialDisalloweBillingNotStarted), culture, arg0);

    public static string BuyNotPossible() => ExtensionResources.Get(nameof (BuyNotPossible));

    public static string BuyNotPossible(CultureInfo culture) => ExtensionResources.Get(nameof (BuyNotPossible), culture);

    public static string InstallOperation() => ExtensionResources.Get(nameof (InstallOperation));

    public static string InstallOperation(CultureInfo culture) => ExtensionResources.Get(nameof (InstallOperation), culture);

    public static string OperationNotAllowedForResourceTypeExtension(object arg0) => ExtensionResources.Format(nameof (OperationNotAllowedForResourceTypeExtension), arg0);

    public static string OperationNotAllowedForResourceTypeExtension(
      object arg0,
      CultureInfo culture)
    {
      return ExtensionResources.Format(nameof (OperationNotAllowedForResourceTypeExtension), culture, arg0);
    }

    public static string RequestOperation() => ExtensionResources.Get(nameof (RequestOperation));

    public static string RequestOperation(CultureInfo culture) => ExtensionResources.Get(nameof (RequestOperation), culture);

    public static string TrialOperation() => ExtensionResources.Get(nameof (TrialOperation));

    public static string TrialOperation(CultureInfo culture) => ExtensionResources.Get(nameof (TrialOperation), culture);

    public static string BuyResourceWorkflowNonAdminFirstTimePurchaseText() => ExtensionResources.Get(nameof (BuyResourceWorkflowNonAdminFirstTimePurchaseText));

    public static string BuyResourceWorkflowNonAdminFirstTimePurchaseText(CultureInfo culture) => ExtensionResources.Get(nameof (BuyResourceWorkflowNonAdminFirstTimePurchaseText), culture);

    public static string PurchaseRequestOperation() => ExtensionResources.Get(nameof (PurchaseRequestOperation));

    public static string PurchaseRequestOperation(CultureInfo culture) => ExtensionResources.Get(nameof (PurchaseRequestOperation), culture);
  }
}
