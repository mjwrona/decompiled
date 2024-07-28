// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingResources
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal static class LicensingResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (LicensingResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => LicensingResources.s_resMgr;

    private static string Get(string resourceName) => LicensingResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? LicensingResources.Get(resourceName) : LicensingResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) LicensingResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? LicensingResources.GetInt(resourceName) : (int) LicensingResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) LicensingResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? LicensingResources.GetBool(resourceName) : (bool) LicensingResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => LicensingResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = LicensingResources.Get(resourceName, culture);
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

    public static string CertificateNotFound(object arg0) => LicensingResources.Format(nameof (CertificateNotFound), arg0);

    public static string CertificateNotFound(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (CertificateNotFound), culture, arg0);

    public static string CertificateDrawerNotFound(object arg0) => LicensingResources.Format(nameof (CertificateDrawerNotFound), arg0);

    public static string CertificateDrawerNotFound(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (CertificateDrawerNotFound), culture, arg0);

    public static string InvalidClientRightsQueryContextProductVersion() => LicensingResources.Get(nameof (InvalidClientRightsQueryContextProductVersion));

    public static string InvalidClientRightsQueryContextProductVersion(CultureInfo culture) => LicensingResources.Get(nameof (InvalidClientRightsQueryContextProductVersion), culture);

    public static string InvalidClientRightsQueryContextProductFamily() => LicensingResources.Get(nameof (InvalidClientRightsQueryContextProductFamily));

    public static string InvalidClientRightsQueryContextProductFamily(CultureInfo culture) => LicensingResources.Get(nameof (InvalidClientRightsQueryContextProductFamily), culture);

    public static string InvalidClientRightsQueryContextProductEdition() => LicensingResources.Get(nameof (InvalidClientRightsQueryContextProductEdition));

    public static string InvalidClientRightsQueryContextProductEdition(CultureInfo culture) => LicensingResources.Get(nameof (InvalidClientRightsQueryContextProductEdition), culture);

    public static string InvalidClientRightsQueryContextReleaseType() => LicensingResources.Get(nameof (InvalidClientRightsQueryContextReleaseType));

    public static string InvalidClientRightsQueryContextReleaseType(CultureInfo culture) => LicensingResources.Get(nameof (InvalidClientRightsQueryContextReleaseType), culture);

    public static string InvalidClientRightsQueryContextCanary() => LicensingResources.Get(nameof (InvalidClientRightsQueryContextCanary));

    public static string InvalidClientRightsQueryContextCanary(CultureInfo culture) => LicensingResources.Get(nameof (InvalidClientRightsQueryContextCanary), culture);

    public static string InvalidClientRightsQueryContextMachineId() => LicensingResources.Get(nameof (InvalidClientRightsQueryContextMachineId));

    public static string InvalidClientRightsQueryContextMachineId(CultureInfo culture) => LicensingResources.Get(nameof (InvalidClientRightsQueryContextMachineId), culture);

    public static string InvalidVisualStudioOffersQueryContextProductFamily() => LicensingResources.Get(nameof (InvalidVisualStudioOffersQueryContextProductFamily));

    public static string InvalidVisualStudioOffersQueryContextProductFamily(CultureInfo culture) => LicensingResources.Get(nameof (InvalidVisualStudioOffersQueryContextProductFamily), culture);

    public static string InvalidVisualStudioOffersQueryContextProductEdition() => LicensingResources.Get(nameof (InvalidVisualStudioOffersQueryContextProductEdition));

    public static string InvalidVisualStudioOffersQueryContextProductEdition(CultureInfo culture) => LicensingResources.Get(nameof (InvalidVisualStudioOffersQueryContextProductEdition), culture);

    public static string InvalidAccountId() => LicensingResources.Get(nameof (InvalidAccountId));

    public static string InvalidAccountId(CultureInfo culture) => LicensingResources.Get(nameof (InvalidAccountId), culture);

    public static string InvalidUserId(object arg0) => LicensingResources.Format(nameof (InvalidUserId), arg0);

    public static string InvalidUserId(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (InvalidUserId), culture, arg0);

    public static string InvaliduserIdServiceIdentity(object arg0) => LicensingResources.Format(nameof (InvaliduserIdServiceIdentity), arg0);

    public static string InvaliduserIdServiceIdentity(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (InvaliduserIdServiceIdentity), culture, arg0);

    public static string LicenseNotAvailableException() => LicensingResources.Get(nameof (LicenseNotAvailableException));

    public static string LicenseNotAvailableException(CultureInfo culture) => LicensingResources.Get(nameof (LicenseNotAvailableException), culture);

    public static string InvalidOperationRequiresProcessing() => LicensingResources.Get(nameof (InvalidOperationRequiresProcessing));

    public static string InvalidOperationRequiresProcessing(CultureInfo culture) => LicensingResources.Get(nameof (InvalidOperationRequiresProcessing), culture);

    public static string RequestedLicenseNotAvailable(object arg0) => LicensingResources.Format(nameof (RequestedLicenseNotAvailable), arg0);

    public static string RequestedLicenseNotAvailable(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (RequestedLicenseNotAvailable), culture, arg0);

    public static string NoLicenseFound() => LicensingResources.Get(nameof (NoLicenseFound));

    public static string NoLicenseFound(CultureInfo culture) => LicensingResources.Get(nameof (NoLicenseFound), culture);

    public static string NoMSDNSubscription() => LicensingResources.Get(nameof (NoMSDNSubscription));

    public static string NoMSDNSubscription(CultureInfo culture) => LicensingResources.Get(nameof (NoMSDNSubscription), culture);

    public static string UserDisabled() => LicensingResources.Get(nameof (UserDisabled));

    public static string UserDisabled(CultureInfo culture) => LicensingResources.Get(nameof (UserDisabled), culture);

    public static string UserIsDeletedFromAccount() => LicensingResources.Get(nameof (UserIsDeletedFromAccount));

    public static string UserIsDeletedFromAccount(CultureInfo culture) => LicensingResources.Get(nameof (UserIsDeletedFromAccount), culture);

    public static string UserIsMissingFromAccount() => LicensingResources.Get(nameof (UserIsMissingFromAccount));

    public static string UserIsMissingFromAccount(CultureInfo culture) => LicensingResources.Get(nameof (UserIsMissingFromAccount), culture);

    public static string UserLicenseExpired() => LicensingResources.Get(nameof (UserLicenseExpired));

    public static string UserLicenseExpired(CultureInfo culture) => LicensingResources.Get(nameof (UserLicenseExpired), culture);

    public static string EarlyAdopterLicenseExpired() => LicensingResources.Get(nameof (EarlyAdopterLicenseExpired));

    public static string EarlyAdopterLicenseExpired(CultureInfo culture) => LicensingResources.Get(nameof (EarlyAdopterLicenseExpired), culture);

    public static string OwnerCannotBeAssignedStakeholderLicense() => LicensingResources.Get(nameof (OwnerCannotBeAssignedStakeholderLicense));

    public static string OwnerCannotBeAssignedStakeholderLicense(CultureInfo culture) => LicensingResources.Get(nameof (OwnerCannotBeAssignedStakeholderLicense), culture);

    public static string UserCannotBeAssignedNoneLicense() => LicensingResources.Get(nameof (UserCannotBeAssignedNoneLicense));

    public static string UserCannotBeAssignedNoneLicense(CultureInfo culture) => LicensingResources.Get(nameof (UserCannotBeAssignedNoneLicense), culture);

    public static string InvaliduserIdGroupIdentity(object arg0) => LicensingResources.Format(nameof (InvaliduserIdGroupIdentity), arg0);

    public static string InvaliduserIdGroupIdentity(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (InvaliduserIdGroupIdentity), culture, arg0);

    public static string LicensingOperationFailed() => LicensingResources.Get(nameof (LicensingOperationFailed));

    public static string LicensingOperationFailed(CultureInfo culture) => LicensingResources.Get(nameof (LicensingOperationFailed), culture);

    public static string ExtensionAccountAssignmentAlreadyExistsWarning(object arg0) => LicensingResources.Format(nameof (ExtensionAccountAssignmentAlreadyExistsWarning), arg0);

    public static string ExtensionAccountAssignmentAlreadyExistsWarning(
      object arg0,
      CultureInfo culture)
    {
      return LicensingResources.Format(nameof (ExtensionAccountAssignmentAlreadyExistsWarning), culture, arg0);
    }

    public static string ExtensionOperationInternalFailure(object arg0) => LicensingResources.Format(nameof (ExtensionOperationInternalFailure), arg0);

    public static string ExtensionOperationInternalFailure(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (ExtensionOperationInternalFailure), culture, arg0);

    public static string ExtensionLicenseNotAvailableException(object arg0, object arg1) => LicensingResources.Format(nameof (ExtensionLicenseNotAvailableException), arg0, arg1);

    public static string ExtensionLicenseNotAvailableException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return LicensingResources.Format(nameof (ExtensionLicenseNotAvailableException), culture, arg0, arg1);
    }

    public static string MinimumRequiredAccessLevelException(object arg0, object arg1) => LicensingResources.Format(nameof (MinimumRequiredAccessLevelException), arg0, arg1);

    public static string MinimumRequiredAccessLevelException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return LicensingResources.Format(nameof (MinimumRequiredAccessLevelException), culture, arg0, arg1);
    }

    public static string ExtensionAlreadyExistsThroughBoundleWarning(object arg0) => LicensingResources.Format(nameof (ExtensionAlreadyExistsThroughBoundleWarning), arg0);

    public static string ExtensionAlreadyExistsThroughBoundleWarning(
      object arg0,
      CultureInfo culture)
    {
      return LicensingResources.Format(nameof (ExtensionAlreadyExistsThroughBoundleWarning), culture, arg0);
    }

    public static string ExtensionLicenseRegistrationFailed() => LicensingResources.Get(nameof (ExtensionLicenseRegistrationFailed));

    public static string ExtensionLicenseRegistrationFailed(CultureInfo culture) => LicensingResources.Get(nameof (ExtensionLicenseRegistrationFailed), culture);

    public static string UserExtensionLicenseNotFound() => LicensingResources.Get(nameof (UserExtensionLicenseNotFound));

    public static string UserExtensionLicenseNotFound(CultureInfo culture) => LicensingResources.Get(nameof (UserExtensionLicenseNotFound), culture);

    public static string UserExtensionLicenseUpdateException() => LicensingResources.Get(nameof (UserExtensionLicenseUpdateException));

    public static string UserExtensionLicenseUpdateException(CultureInfo culture) => LicensingResources.Get(nameof (UserExtensionLicenseUpdateException), culture);

    public static string UserNotEligibleForExtension(object arg0) => LicensingResources.Format(nameof (UserNotEligibleForExtension), arg0);

    public static string UserNotEligibleForExtension(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (UserNotEligibleForExtension), culture, arg0);

    public static string ExtensionAccountAssignmentDoesNotExistWarning(object arg0) => LicensingResources.Format(nameof (ExtensionAccountAssignmentDoesNotExistWarning), arg0);

    public static string ExtensionAccountAssignmentDoesNotExistWarning(
      object arg0,
      CultureInfo culture)
    {
      return LicensingResources.Format(nameof (ExtensionAccountAssignmentDoesNotExistWarning), culture, arg0);
    }

    public static string ExtensionBundleUnassignmentWarning(object arg0) => LicensingResources.Format(nameof (ExtensionBundleUnassignmentWarning), arg0);

    public static string ExtensionBundleUnassignmentWarning(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (ExtensionBundleUnassignmentWarning), culture, arg0);

    public static string UserPendingValidation(object arg0) => LicensingResources.Format(nameof (UserPendingValidation), arg0);

    public static string UserPendingValidation(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (UserPendingValidation), culture, arg0);

    public static string ImplicitAssignmentExtensionUnassignment(object arg0) => LicensingResources.Format(nameof (ImplicitAssignmentExtensionUnassignment), arg0);

    public static string ImplicitAssignmentExtensionUnassignment(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (ImplicitAssignmentExtensionUnassignment), culture, arg0);

    public static string UserImplicitAssignment(object arg0) => LicensingResources.Format(nameof (UserImplicitAssignment), arg0);

    public static string UserImplicitAssignment(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (UserImplicitAssignment), culture, arg0);

    public static string EmailContentYoureInvited() => LicensingResources.Get(nameof (EmailContentYoureInvited));

    public static string EmailContentYoureInvited(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentYoureInvited), culture);

    public static string EmailContentJoinPersonAtAccount(object arg0, object arg1) => LicensingResources.Format(nameof (EmailContentJoinPersonAtAccount), arg0, arg1);

    public static string EmailContentJoinPersonAtAccount(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return LicensingResources.Format(nameof (EmailContentJoinPersonAtAccount), culture, arg0, arg1);
    }

    public static string EmailContentJoinPersonAtAccountTracked(object arg0, object arg1) => LicensingResources.Format(nameof (EmailContentJoinPersonAtAccountTracked), arg0, arg1);

    public static string EmailContentJoinPersonAtAccountTracked(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return LicensingResources.Format(nameof (EmailContentJoinPersonAtAccountTracked), culture, arg0, arg1);
    }

    public static string EmailContentTwoWaysToAccept() => LicensingResources.Get(nameof (EmailContentTwoWaysToAccept));

    public static string EmailContentTwoWaysToAccept(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentTwoWaysToAccept), culture);

    public static string EmailContentJoin() => LicensingResources.Get(nameof (EmailContentJoin));

    public static string EmailContentJoin(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentJoin), culture);

    public static string EmailContentOpenInVisualStudio() => LicensingResources.Get(nameof (EmailContentOpenInVisualStudio));

    public static string EmailContentOpenInVisualStudio(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentOpenInVisualStudio), culture);

    public static string EmailContentServicesYoullLoveLike(object arg0, object arg1) => LicensingResources.Format(nameof (EmailContentServicesYoullLoveLike), arg0, arg1);

    public static string EmailContentServicesYoullLoveLike(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return LicensingResources.Format(nameof (EmailContentServicesYoullLoveLike), culture, arg0, arg1);
    }

    public static string EmailContentUnlimitedFreeRepos() => LicensingResources.Get(nameof (EmailContentUnlimitedFreeRepos));

    public static string EmailContentUnlimitedFreeRepos(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentUnlimitedFreeRepos), culture);

    public static string EmailContentBugsWorkItemsFeedback() => LicensingResources.Get(nameof (EmailContentBugsWorkItemsFeedback));

    public static string EmailContentBugsWorkItemsFeedback(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentBugsWorkItemsFeedback), culture);

    public static string EmailContentDevelopInAnyLanguage() => LicensingResources.Get(nameof (EmailContentDevelopInAnyLanguage));

    public static string EmailContentDevelopInAnyLanguage(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentDevelopInAnyLanguage), culture);

    public static string EmailContentUseVisualStudioEclipse(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return LicensingResources.Format(nameof (EmailContentUseVisualStudioEclipse), arg0, arg1, arg2, arg3);
    }

    public static string EmailContentUseVisualStudioEclipse(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return LicensingResources.Format(nameof (EmailContentUseVisualStudioEclipse), culture, arg0, arg1, arg2, arg3);
    }

    public static string EmailContentContinuousIntegrationBuilds() => LicensingResources.Get(nameof (EmailContentContinuousIntegrationBuilds));

    public static string EmailContentContinuousIntegrationBuilds(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentContinuousIntegrationBuilds), culture);

    public static string EmailContentEnterpriseGradeServices() => LicensingResources.Get(nameof (EmailContentEnterpriseGradeServices));

    public static string EmailContentEnterpriseGradeServices(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentEnterpriseGradeServices), culture);

    public static string EmailContentHereAreYourDetails() => LicensingResources.Get(nameof (EmailContentHereAreYourDetails));

    public static string EmailContentHereAreYourDetails(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentHereAreYourDetails), culture);

    public static string EmailContentAccountUrl() => LicensingResources.Get(nameof (EmailContentAccountUrl));

    public static string EmailContentAccountUrl(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentAccountUrl), culture);

    public static string EmailContentSignInAddress() => LicensingResources.Get(nameof (EmailContentSignInAddress));

    public static string EmailContentSignInAddress(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentSignInAddress), culture);

    public static string EmailContentNotSureWhatToDo(object arg0, object arg1) => LicensingResources.Format(nameof (EmailContentNotSureWhatToDo), arg0, arg1);

    public static string EmailContentNotSureWhatToDo(object arg0, object arg1, CultureInfo culture) => LicensingResources.Format(nameof (EmailContentNotSureWhatToDo), culture, arg0, arg1);

    public static string EmailContentNeedHelp(object arg0, object arg1) => LicensingResources.Format(nameof (EmailContentNeedHelp), arg0, arg1);

    public static string EmailContentNeedHelp(object arg0, object arg1, CultureInfo culture) => LicensingResources.Format(nameof (EmailContentNeedHelp), culture, arg0, arg1);

    public static string EmailContentHappyCoding() => LicensingResources.Get(nameof (EmailContentHappyCoding));

    public static string EmailContentHappyCoding(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentHappyCoding), culture);

    public static string EmailContentVisualStudioTeam() => LicensingResources.Get(nameof (EmailContentVisualStudioTeam));

    public static string EmailContentVisualStudioTeam(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentVisualStudioTeam), culture);

    public static string EmailContentServicesYoullLoveLikeTracked(object arg0, object arg1) => LicensingResources.Format(nameof (EmailContentServicesYoullLoveLikeTracked), arg0, arg1);

    public static string EmailContentServicesYoullLoveLikeTracked(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return LicensingResources.Format(nameof (EmailContentServicesYoullLoveLikeTracked), culture, arg0, arg1);
    }

    public static string EmailContentUseVisualStudioEclipseTracked(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return LicensingResources.Format(nameof (EmailContentUseVisualStudioEclipseTracked), arg0, arg1, arg2, arg3);
    }

    public static string EmailContentUseVisualStudioEclipseTracked(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return LicensingResources.Format(nameof (EmailContentUseVisualStudioEclipseTracked), culture, arg0, arg1, arg2, arg3);
    }

    public static string EmailContentNeedHelpTracked(object arg0, object arg1) => LicensingResources.Format(nameof (EmailContentNeedHelpTracked), arg0, arg1);

    public static string EmailContentNeedHelpTracked(object arg0, object arg1, CultureInfo culture) => LicensingResources.Format(nameof (EmailContentNeedHelpTracked), culture, arg0, arg1);

    public static string EmailContentJoinAtTitle(object arg0) => LicensingResources.Format(nameof (EmailContentJoinAtTitle), arg0);

    public static string EmailContentJoinAtTitle(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (EmailContentJoinAtTitle), culture, arg0);

    public static string EmailContentJoinNow() => LicensingResources.Get(nameof (EmailContentJoinNow));

    public static string EmailContentJoinNow(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentJoinNow), culture);

    public static string EmailContentNewtoVSTS() => LicensingResources.Get(nameof (EmailContentNewtoVSTS));

    public static string EmailContentNewtoVSTS(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentNewtoVSTS), culture);

    public static string EmailContentStartOwnProject() => LicensingResources.Get(nameof (EmailContentStartOwnProject));

    public static string EmailContentStartOwnProject(CultureInfo culture) => LicensingResources.Get(nameof (EmailContentStartOwnProject), culture);

    public static string AdvancedLicenseMessage(object arg0, object arg1) => LicensingResources.Format(nameof (AdvancedLicenseMessage), arg0, arg1);

    public static string AdvancedLicenseMessage(object arg0, object arg1, CultureInfo culture) => LicensingResources.Format(nameof (AdvancedLicenseMessage), culture, arg0, arg1);

    public static string UserInvitedToAccountBodyTemplate() => LicensingResources.Get(nameof (UserInvitedToAccountBodyTemplate));

    public static string UserInvitedToAccountBodyTemplate(CultureInfo culture) => LicensingResources.Get(nameof (UserInvitedToAccountBodyTemplate), culture);

    public static string UserInvitedToAccountV1_Template() => LicensingResources.Get(nameof (UserInvitedToAccountV1_Template));

    public static string UserInvitedToAccountV1_Template(CultureInfo culture) => LicensingResources.Get(nameof (UserInvitedToAccountV1_Template), culture);

    public static string UserInvitedToAccountV2_Template() => LicensingResources.Get(nameof (UserInvitedToAccountV2_Template));

    public static string UserInvitedToAccountV2_Template(CultureInfo culture) => LicensingResources.Get(nameof (UserInvitedToAccountV2_Template), culture);

    public static string InvalidIdentityType(object arg0) => LicensingResources.Format(nameof (InvalidIdentityType), arg0);

    public static string InvalidIdentityType(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (InvalidIdentityType), culture, arg0);

    public static string InvalidRoamingUnassignment(object arg0) => LicensingResources.Format(nameof (InvalidRoamingUnassignment), arg0);

    public static string InvalidRoamingUnassignment(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (InvalidRoamingUnassignment), culture, arg0);

    public static string InvalidRoamingAssignment(object arg0) => LicensingResources.Format(nameof (InvalidRoamingAssignment), arg0);

    public static string InvalidRoamingAssignment(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (InvalidRoamingAssignment), culture, arg0);

    public static string DeleteUserLicenseByCollection() => LicensingResources.Get(nameof (DeleteUserLicenseByCollection));

    public static string DeleteUserLicenseByCollection(CultureInfo culture) => LicensingResources.Get(nameof (DeleteUserLicenseByCollection), culture);

    public static string UserExtensionLicenseCopyException() => LicensingResources.Get(nameof (UserExtensionLicenseCopyException));

    public static string UserExtensionLicenseCopyException(CultureInfo culture) => LicensingResources.Get(nameof (UserExtensionLicenseCopyException), culture);

    public static string SourceExtensionLicensesNotFoundException(object arg0) => LicensingResources.Format(nameof (SourceExtensionLicensesNotFoundException), arg0);

    public static string SourceExtensionLicensesNotFoundException(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (SourceExtensionLicensesNotFoundException), culture, arg0);

    public static string SourceLicensesNotFoundException(object arg0) => LicensingResources.Format(nameof (SourceLicensesNotFoundException), arg0);

    public static string SourceLicensesNotFoundException(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (SourceLicensesNotFoundException), culture, arg0);

    public static string FoundAdditionalTargetExtensionLicenses(object arg0) => LicensingResources.Format(nameof (FoundAdditionalTargetExtensionLicenses), arg0);

    public static string FoundAdditionalTargetExtensionLicenses(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (FoundAdditionalTargetExtensionLicenses), culture, arg0);

    public static string FoundAdditionalTargetLicenses(object arg0) => LicensingResources.Format(nameof (FoundAdditionalTargetLicenses), arg0);

    public static string FoundAdditionalTargetLicenses(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (FoundAdditionalTargetLicenses), culture, arg0);

    public static string VsReleaseNoPriorRelease(object arg0, object arg1) => LicensingResources.Format(nameof (VsReleaseNoPriorRelease), arg0, arg1);

    public static string VsReleaseNoPriorRelease(object arg0, object arg1, CultureInfo culture) => LicensingResources.Format(nameof (VsReleaseNoPriorRelease), culture, arg0, arg1);

    public static string VsReleaseOverlapDetected(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return LicensingResources.Format(nameof (VsReleaseOverlapDetected), arg0, arg1, arg2, arg3);
    }

    public static string VsReleaseOverlapDetected(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return LicensingResources.Format(nameof (VsReleaseOverlapDetected), culture, arg0, arg1, arg2, arg3);
    }

    public static string VsReleasePastExpirationDate() => LicensingResources.Get(nameof (VsReleasePastExpirationDate));

    public static string VsReleasePastExpirationDate(CultureInfo culture) => LicensingResources.Get(nameof (VsReleasePastExpirationDate), culture);

    public static string ClientReleaseNotFoundException(object arg0) => LicensingResources.Format(nameof (ClientReleaseNotFoundException), arg0);

    public static string ClientReleaseNotFoundException(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (ClientReleaseNotFoundException), culture, arg0);

    public static string UserNotFound(object arg0) => LicensingResources.Format(nameof (UserNotFound), arg0);

    public static string UserNotFound(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (UserNotFound), culture, arg0);

    public static string SourceAcesNotFoundException(object arg0) => LicensingResources.Format(nameof (SourceAcesNotFoundException), arg0);

    public static string SourceAcesNotFoundException(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (SourceAcesNotFoundException), culture, arg0);

    public static string InternalAccountShouldUseEarlyAdopter() => LicensingResources.Get(nameof (InternalAccountShouldUseEarlyAdopter));

    public static string InternalAccountShouldUseEarlyAdopter(CultureInfo culture) => LicensingResources.Get(nameof (InternalAccountShouldUseEarlyAdopter), culture);

    public static string SourcePreviousLicensesNotFoundException(object arg0) => LicensingResources.Format(nameof (SourcePreviousLicensesNotFoundException), arg0);

    public static string SourcePreviousLicensesNotFoundException(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (SourcePreviousLicensesNotFoundException), culture, arg0);

    public static string ExtensionNotFound(object arg0) => LicensingResources.Format(nameof (ExtensionNotFound), arg0);

    public static string ExtensionNotFound(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (ExtensionNotFound), culture, arg0);

    public static string FailedToTransferLicensesException() => LicensingResources.Get(nameof (FailedToTransferLicensesException));

    public static string FailedToTransferLicensesException(CultureInfo culture) => LicensingResources.Get(nameof (FailedToTransferLicensesException), culture);

    public static string ShardingBlocksAddLicensedUser() => LicensingResources.Get(nameof (ShardingBlocksAddLicensedUser));

    public static string ShardingBlocksAddLicensedUser(CultureInfo culture) => LicensingResources.Get(nameof (ShardingBlocksAddLicensedUser), culture);

    public static string FailedToGetOfferSubscriptionsException() => LicensingResources.Get(nameof (FailedToGetOfferSubscriptionsException));

    public static string FailedToGetOfferSubscriptionsException(CultureInfo culture) => LicensingResources.Get(nameof (FailedToGetOfferSubscriptionsException), culture);

    public static string AuditGroupRuleReason() => LicensingResources.Get(nameof (AuditGroupRuleReason));

    public static string AuditGroupRuleReason(CultureInfo culture) => LicensingResources.Get(nameof (AuditGroupRuleReason), culture);

    public static string InvalidFilterQuery(object arg0) => LicensingResources.Format(nameof (InvalidFilterQuery), arg0);

    public static string InvalidFilterQuery(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (InvalidFilterQuery), culture, arg0);

    public static string InvalidOrderByQuery(object arg0) => LicensingResources.Format(nameof (InvalidOrderByQuery), arg0);

    public static string InvalidOrderByQuery(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (InvalidOrderByQuery), culture, arg0);

    public static string InvalidOrderByProperty(object arg0) => LicensingResources.Format(nameof (InvalidOrderByProperty), arg0);

    public static string InvalidOrderByProperty(object arg0, CultureInfo culture) => LicensingResources.Format(nameof (InvalidOrderByProperty), culture, arg0);
  }
}
