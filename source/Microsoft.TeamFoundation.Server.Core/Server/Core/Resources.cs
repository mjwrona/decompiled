// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Resources
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.TeamFoundation.Server.Core.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.TeamFoundation.Server.Core.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.TeamFoundation.Server.Core.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.Server.Core.Resources.Get(resourceName) : Microsoft.TeamFoundation.Server.Core.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.TeamFoundation.Server.Core.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.Server.Core.Resources.GetInt(resourceName) : (int) Microsoft.TeamFoundation.Server.Core.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.TeamFoundation.Server.Core.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.Server.Core.Resources.GetBool(resourceName) : (bool) Microsoft.TeamFoundation.Server.Core.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.TeamFoundation.Server.Core.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.TeamFoundation.Server.Core.Resources.Get(resourceName, culture);
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

    public static string AssemblyDescription() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (AssemblyDescription));

    public static string AssemblyDescription(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (AssemblyDescription), culture);

    public static string InvalidStateFilter(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidStateFilter), arg0);

    public static string InvalidStateFilter(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidStateFilter), culture, arg0);

    public static string QueryTeamsInCollectionError() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (QueryTeamsInCollectionError));

    public static string QueryTeamsInCollectionError(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (QueryTeamsInCollectionError), culture);

    public static string InvalidTagName(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidTagName), arg0);

    public static string InvalidTagName(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidTagName), culture, arg0);

    public static string TagEnumerateAccessDenied() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TagEnumerateAccessDenied));

    public static string TagEnumerateAccessDenied(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TagEnumerateAccessDenied), culture);

    public static string TagCreateAccessDenied() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TagCreateAccessDenied));

    public static string TagCreateAccessDenied(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TagCreateAccessDenied), culture);

    public static string TagUpdateAccessDenied() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TagUpdateAccessDenied));

    public static string TagUpdateAccessDenied(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TagUpdateAccessDenied), culture);

    public static string TagDeleteAccessDenied() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TagDeleteAccessDenied));

    public static string TagDeleteAccessDenied(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TagDeleteAccessDenied), culture);

    public static string AddAADGroupError() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (AddAADGroupError));

    public static string AddAADGroupError(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (AddAADGroupError), culture);

    public static string ProjectLockdown() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (ProjectLockdown));

    public static string ProjectLockdown(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (ProjectLockdown), culture);

    public static string InvalidTagName_InvalidCharactersException() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (InvalidTagName_InvalidCharactersException));

    public static string InvalidTagName_InvalidCharactersException(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (InvalidTagName_InvalidCharactersException), culture);

    public static string InvalidProjectUpdate() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (InvalidProjectUpdate));

    public static string InvalidProjectUpdate(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (InvalidProjectUpdate), culture);

    public static string InvalidCollectionName() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (InvalidCollectionName));

    public static string InvalidCollectionName(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (InvalidCollectionName), culture);

    public static string InvalidSubscriptionProjectReference() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (InvalidSubscriptionProjectReference));

    public static string InvalidSubscriptionProjectReference(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (InvalidSubscriptionProjectReference), culture);

    public static string InvalidSubscriptionToken() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (InvalidSubscriptionToken));

    public static string InvalidSubscriptionToken(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (InvalidSubscriptionToken), culture);

    public static string TemporaryDataInvalid() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TemporaryDataInvalid));

    public static string TemporaryDataInvalid(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TemporaryDataInvalid), culture);

    public static string TemporaryDataNotFoundException() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TemporaryDataNotFoundException));

    public static string TemporaryDataNotFoundException(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TemporaryDataNotFoundException), culture);

    public static string TemporaryDataTooLargeException(object arg0, object arg1) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (TemporaryDataTooLargeException), arg0, arg1);

    public static string TemporaryDataTooLargeException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (TemporaryDataTooLargeException), culture, arg0, arg1);
    }

    public static string ManageProcessTemplatesPermissionDenied(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (ManageProcessTemplatesPermissionDenied), arg0);

    public static string ManageProcessTemplatesPermissionDenied(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (ManageProcessTemplatesPermissionDenied), culture, arg0);

    public static string ProcessTemplatesReadPermissionDenied() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (ProcessTemplatesReadPermissionDenied));

    public static string ProcessTemplatesReadPermissionDenied(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (ProcessTemplatesReadPermissionDenied), culture);

    public static string ProcessTemplateVersionInvalid(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (ProcessTemplateVersionInvalid), arg0, arg1, arg2, arg3);
    }

    public static string ProcessTemplateVersionInvalid(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (ProcessTemplateVersionInvalid), culture, arg0, arg1, arg2, arg3);
    }

    public static string CatalogProjectDescriptionChange() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (CatalogProjectDescriptionChange));

    public static string CatalogProjectDescriptionChange(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (CatalogProjectDescriptionChange), culture);

    public static string InvalidProcessTemplateTypeId(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidProcessTemplateTypeId), arg0);

    public static string InvalidProcessTemplateTypeId(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidProcessTemplateTypeId), culture, arg0);

    public static string InvalidProjectCreate() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (InvalidProjectCreate));

    public static string InvalidProjectCreate(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (InvalidProjectCreate), culture);

    public static string HardDeleteOnHosted() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (HardDeleteOnHosted));

    public static string HardDeleteOnHosted(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (HardDeleteOnHosted), culture);

    public static string InvalidSourceControlType(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidSourceControlType), arg0);

    public static string InvalidSourceControlType(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidSourceControlType), culture, arg0);

    public static string ServiceFriendlyNameAlreadyInUse() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (ServiceFriendlyNameAlreadyInUse));

    public static string ServiceFriendlyNameAlreadyInUse(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (ServiceFriendlyNameAlreadyInUse), culture);

    public static string ProcessTemplateInvalidScopeStatusUpdate() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (ProcessTemplateInvalidScopeStatusUpdate));

    public static string ProcessTemplateInvalidScopeStatusUpdate(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (ProcessTemplateInvalidScopeStatusUpdate), culture);

    public static string ProcessTemplateConnectedToProjects(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (ProcessTemplateConnectedToProjects), arg0);

    public static string ProcessTemplateConnectedToProjects(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (ProcessTemplateConnectedToProjects), culture, arg0);

    public static string DefaultProcessTemplate(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (DefaultProcessTemplate), arg0);

    public static string DefaultProcessTemplate(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (DefaultProcessTemplate), culture, arg0);

    public static string ProcessLimitInformedExceeded(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (ProcessLimitInformedExceeded), arg0);

    public static string ProcessLimitInformedExceeded(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (ProcessLimitInformedExceeded), culture, arg0);

    public static string ProjectDescriptionTooLong(object arg0, object arg1) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (ProjectDescriptionTooLong), arg0, arg1);

    public static string ProjectDescriptionTooLong(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (ProjectDescriptionTooLong), culture, arg0, arg1);

    public static string TagExceptionWriteAccessDenied() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TagExceptionWriteAccessDenied));

    public static string TagExceptionWriteAccessDenied(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TagExceptionWriteAccessDenied), culture);

    public static string ProcessNotFound(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (ProcessNotFound), arg0);

    public static string ProcessNotFound(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (ProcessNotFound), culture, arg0);

    public static string ProcessNameInformedNotUnique(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (ProcessNameInformedNotUnique), arg0);

    public static string ProcessNameInformedNotUnique(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (ProcessNameInformedNotUnique), culture, arg0);

    public static string RemoveAADGroupError() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (RemoveAADGroupError));

    public static string RemoveAADGroupError(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (RemoveAADGroupError), culture);

    public static string InvalidConnectedServiceId(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidConnectedServiceId), arg0);

    public static string InvalidConnectedServiceId(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidConnectedServiceId), culture, arg0);

    public static string InvalidServiceEndpointUrl(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidServiceEndpointUrl), arg0);

    public static string InvalidServiceEndpointUrl(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidServiceEndpointUrl), culture, arg0);

    public static string EmptyProcessTemplateDataStream() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (EmptyProcessTemplateDataStream));

    public static string EmptyProcessTemplateDataStream(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (EmptyProcessTemplateDataStream), culture);

    public static string InvalidToken(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidToken), arg0);

    public static string InvalidToken(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidToken), culture, arg0);

    public static string ProcessTemplateInvalid() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (ProcessTemplateInvalid));

    public static string ProcessTemplateInvalid(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (ProcessTemplateInvalid), culture);

    public static string CustomProcessTemplateNotSupported(object arg0, object arg1) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (CustomProcessTemplateNotSupported), arg0, arg1);

    public static string CustomProcessTemplateNotSupported(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (CustomProcessTemplateNotSupported), culture, arg0, arg1);
    }

    public static string TrialExpiredMessage() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TrialExpiredMessage));

    public static string TrialExpiredMessage(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TrialExpiredMessage), culture);

    public static string TrialExpiredCanExtendMessage(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (TrialExpiredCanExtendMessage), arg0);

    public static string TrialExpiredCanExtendMessage(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (TrialExpiredCanExtendMessage), culture, arg0);

    public static string InTrialMessage(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InTrialMessage), arg0);

    public static string InTrialMessage(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InTrialMessage), culture, arg0);

    public static string TrialExpiredBanner(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (TrialExpiredBanner), arg0);

    public static string TrialExpiredBanner(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (TrialExpiredBanner), culture, arg0);

    public static string TrialExpiredCanExtendBanner(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (TrialExpiredCanExtendBanner), arg0);

    public static string TrialExpiredCanExtendBanner(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (TrialExpiredCanExtendBanner), culture, arg0);

    public static string DuplicateTemporaryDataIdMessage(object arg0, object arg1) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (DuplicateTemporaryDataIdMessage), arg0, arg1);

    public static string DuplicateTemporaryDataIdMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (DuplicateTemporaryDataIdMessage), culture, arg0, arg1);
    }

    public static string UnknownTemporaryDataPropertyMessage(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (UnknownTemporaryDataPropertyMessage), arg0);

    public static string UnknownTemporaryDataPropertyMessage(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (UnknownTemporaryDataPropertyMessage), culture, arg0);

    public static string ManageUsage() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (ManageUsage));

    public static string ManageUsage(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (ManageUsage), culture);

    public static string MicrosoftPublishedTFSExtensionsFeatureDescription() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (MicrosoftPublishedTFSExtensionsFeatureDescription));

    public static string MicrosoftPublishedTFSExtensionsFeatureDescription(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (MicrosoftPublishedTFSExtensionsFeatureDescription), culture);

    public static string MicrosoftPublishedTFSExtensionsFeatureName() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (MicrosoftPublishedTFSExtensionsFeatureName));

    public static string MicrosoftPublishedTFSExtensionsFeatureName(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (MicrosoftPublishedTFSExtensionsFeatureName), culture);

    public static string PackageManagementFeatureDescription() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (PackageManagementFeatureDescription));

    public static string PackageManagementFeatureDescription(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (PackageManagementFeatureDescription), culture);

    public static string PackageManagementFeatureName() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (PackageManagementFeatureName));

    public static string PackageManagementFeatureName(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (PackageManagementFeatureName), culture);

    public static string TeamServiceSafeguardBlockCreateTeamAndAdminOperations_Error() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TeamServiceSafeguardBlockCreateTeamAndAdminOperations_Error));

    public static string TeamServiceSafeguardBlockCreateTeamAndAdminOperations_Error(
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TeamServiceSafeguardBlockCreateTeamAndAdminOperations_Error), culture);
    }

    public static string FeatureDisabledError() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (FeatureDisabledError));

    public static string FeatureDisabledError(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (FeatureDisabledError), culture);

    public static string PROXY_PERMISSION_MANAGE() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (PROXY_PERMISSION_MANAGE));

    public static string PROXY_PERMISSION_MANAGE(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (PROXY_PERMISSION_MANAGE), culture);

    public static string PROXY_PERMISSION_READ() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (PROXY_PERMISSION_READ));

    public static string PROXY_PERMISSION_READ(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (PROXY_PERMISSION_READ), culture);

    public static string PropertiesCannotBeUpdated(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (PropertiesCannotBeUpdated), arg0);

    public static string PropertiesCannotBeUpdated(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (PropertiesCannotBeUpdated), culture, arg0);

    public static string InvalidCreateProjectVisibilityLevel(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidCreateProjectVisibilityLevel), arg0);

    public static string InvalidCreateProjectVisibilityLevel(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidCreateProjectVisibilityLevel), culture, arg0);

    public static string LicenseNotFound(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (LicenseNotFound), arg0);

    public static string LicenseNotFound(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (LicenseNotFound), culture, arg0);

    public static string TeamLimitExceededException(object arg0, object arg1) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (TeamLimitExceededException), arg0, arg1);

    public static string TeamLimitExceededException(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (TeamLimitExceededException), culture, arg0, arg1);

    public static string InvalidProjectVisibility(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidProjectVisibility), arg0);

    public static string InvalidProjectVisibility(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (InvalidProjectVisibility), culture, arg0);

    public static string EmptyProjectUri() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (EmptyProjectUri));

    public static string EmptyProjectUri(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (EmptyProjectUri), culture);

    public static string TagUpdateAddRemoveConflict() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TagUpdateAddRemoveConflict));

    public static string TagUpdateAddRemoveConflict(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (TagUpdateAddRemoveConflict), culture);

    public static string UnableToFindSoftDeletedProject(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (UnableToFindSoftDeletedProject), arg0);

    public static string UnableToFindSoftDeletedProject(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (UnableToFindSoftDeletedProject), culture, arg0);

    public static string CannotDeleteDefaultTeamMessage(object arg0) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (CannotDeleteDefaultTeamMessage), arg0);

    public static string CannotDeleteDefaultTeamMessage(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (CannotDeleteDefaultTeamMessage), culture, arg0);

    public static string MissingProjectId() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (MissingProjectId));

    public static string MissingProjectId(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (MissingProjectId), culture);

    public static string MissingTeamData() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (MissingTeamData));

    public static string MissingTeamData(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (MissingTeamData), culture);

    public static string MissingTeamId() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (MissingTeamId));

    public static string MissingTeamId(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (MissingTeamId), culture);

    public static string OverProjectCountLimit(object arg0, object arg1) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (OverProjectCountLimit), arg0, arg1);

    public static string OverProjectCountLimit(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Format(nameof (OverProjectCountLimit), culture, arg0, arg1);

    public static string UnableToParseRegistryKeyForProjectLimit() => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (UnableToParseRegistryKeyForProjectLimit));

    public static string UnableToParseRegistryKeyForProjectLimit(CultureInfo culture) => Microsoft.TeamFoundation.Server.Core.Resources.Get(nameof (UnableToParseRegistryKeyForProjectLimit), culture);
  }
}
