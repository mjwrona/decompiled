// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.IdentityResources
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.WebApi
{
  internal static class IdentityResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (IdentityResources), typeof (IdentityResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => IdentityResources.s_resMgr;

    private static string Get(string resourceName) => IdentityResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? IdentityResources.Get(resourceName) : IdentityResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) IdentityResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? IdentityResources.GetInt(resourceName) : (int) IdentityResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) IdentityResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? IdentityResources.GetBool(resourceName) : (bool) IdentityResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => IdentityResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = IdentityResources.Get(resourceName, culture);
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

    public static string FieldReadOnly(object arg0) => IdentityResources.Format(nameof (FieldReadOnly), arg0);

    public static string FieldReadOnly(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (FieldReadOnly), culture, arg0);

    public static string GROUPCREATIONERROR(object arg0, object arg1) => IdentityResources.Format(nameof (GROUPCREATIONERROR), arg0, arg1);

    public static string GROUPCREATIONERROR(object arg0, object arg1, CultureInfo culture) => IdentityResources.Format(nameof (GROUPCREATIONERROR), culture, arg0, arg1);

    public static string ADDMEMBERCYCLICMEMBERSHIPERROR(object arg0, object arg1) => IdentityResources.Format(nameof (ADDMEMBERCYCLICMEMBERSHIPERROR), arg0, arg1);

    public static string ADDMEMBERCYCLICMEMBERSHIPERROR(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return IdentityResources.Format(nameof (ADDMEMBERCYCLICMEMBERSHIPERROR), culture, arg0, arg1);
    }

    public static string GROUPSCOPECREATIONERROR(object arg0) => IdentityResources.Format(nameof (GROUPSCOPECREATIONERROR), arg0);

    public static string GROUPSCOPECREATIONERROR(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (GROUPSCOPECREATIONERROR), culture, arg0);

    public static string ADDMEMBERIDENTITYALREADYMEMBERERROR(object arg0, object arg1) => IdentityResources.Format(nameof (ADDMEMBERIDENTITYALREADYMEMBERERROR), arg0, arg1);

    public static string ADDMEMBERIDENTITYALREADYMEMBERERROR(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return IdentityResources.Format(nameof (ADDMEMBERIDENTITYALREADYMEMBERERROR), culture, arg0, arg1);
    }

    public static string REMOVEGROUPMEMBERNOTMEMBERERROR(object arg0) => IdentityResources.Format(nameof (REMOVEGROUPMEMBERNOTMEMBERERROR), arg0);

    public static string REMOVEGROUPMEMBERNOTMEMBERERROR(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (REMOVEGROUPMEMBERNOTMEMBERERROR), culture, arg0);

    public static string REMOVEADMINGROUPERROR() => IdentityResources.Get(nameof (REMOVEADMINGROUPERROR));

    public static string REMOVEADMINGROUPERROR(CultureInfo culture) => IdentityResources.Get(nameof (REMOVEADMINGROUPERROR), culture);

    public static string REMOVEEVERYONEGROUPERROR() => IdentityResources.Get(nameof (REMOVEEVERYONEGROUPERROR));

    public static string REMOVEEVERYONEGROUPERROR(CultureInfo culture) => IdentityResources.Get(nameof (REMOVEEVERYONEGROUPERROR), culture);

    public static string REMOVESERVICEGROUPERROR() => IdentityResources.Get(nameof (REMOVESERVICEGROUPERROR));

    public static string REMOVESERVICEGROUPERROR(CultureInfo culture) => IdentityResources.Get(nameof (REMOVESERVICEGROUPERROR), culture);

    public static string REMOVESPECIALGROUPERROR() => IdentityResources.Get(nameof (REMOVESPECIALGROUPERROR));

    public static string REMOVESPECIALGROUPERROR(CultureInfo culture) => IdentityResources.Get(nameof (REMOVESPECIALGROUPERROR), culture);

    public static string FINDGROUPSIDDOESNOTEXISTERROR(object arg0) => IdentityResources.Format(nameof (FINDGROUPSIDDOESNOTEXISTERROR), arg0);

    public static string FINDGROUPSIDDOESNOTEXISTERROR(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (FINDGROUPSIDDOESNOTEXISTERROR), culture, arg0);

    public static string GROUPRENAMEERROR(object arg0) => IdentityResources.Format(nameof (GROUPRENAMEERROR), arg0);

    public static string GROUPRENAMEERROR(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (GROUPRENAMEERROR), culture, arg0);

    public static string GROUPSCOPEDOESNOTEXISTERROR(object arg0) => IdentityResources.Format(nameof (GROUPSCOPEDOESNOTEXISTERROR), arg0);

    public static string GROUPSCOPEDOESNOTEXISTERROR(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (GROUPSCOPEDOESNOTEXISTERROR), culture, arg0);

    public static string IdentityNotFoundMessage(object arg0) => IdentityResources.Format(nameof (IdentityNotFoundMessage), arg0);

    public static string IdentityNotFoundMessage(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (IdentityNotFoundMessage), culture, arg0);

    public static string IdentityNotFoundWithDescriptor(object arg0, object arg1) => IdentityResources.Format(nameof (IdentityNotFoundWithDescriptor), arg0, arg1);

    public static string IdentityNotFoundWithDescriptor(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return IdentityResources.Format(nameof (IdentityNotFoundWithDescriptor), culture, arg0, arg1);
    }

    public static string IdentityNotFoundSimpleMessage() => IdentityResources.Get(nameof (IdentityNotFoundSimpleMessage));

    public static string IdentityNotFoundSimpleMessage(CultureInfo culture) => IdentityResources.Get(nameof (IdentityNotFoundSimpleMessage), culture);

    public static string IdentityNotFoundWithTfid(object arg0) => IdentityResources.Format(nameof (IdentityNotFoundWithTfid), arg0);

    public static string IdentityNotFoundWithTfid(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (IdentityNotFoundWithTfid), culture, arg0);

    public static string IdentityNotFoundWithName(object arg0) => IdentityResources.Format(nameof (IdentityNotFoundWithName), arg0);

    public static string IdentityNotFoundWithName(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (IdentityNotFoundWithName), culture, arg0);

    public static string IdentityNotFoundInCollection(object arg0) => IdentityResources.Format(nameof (IdentityNotFoundInCollection), arg0);

    public static string IdentityNotFoundInCollection(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (IdentityNotFoundInCollection), culture, arg0);

    public static string IdentityAccountNameAlreadyInUseError(object arg0) => IdentityResources.Format(nameof (IdentityAccountNameAlreadyInUseError), arg0);

    public static string IdentityAccountNameAlreadyInUseError(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (IdentityAccountNameAlreadyInUseError), culture, arg0);

    public static string IdentityAccountNameCollisionRepairFailedError(object arg0) => IdentityResources.Format(nameof (IdentityAccountNameCollisionRepairFailedError), arg0);

    public static string IdentityAccountNameCollisionRepairFailedError(
      object arg0,
      CultureInfo culture)
    {
      return IdentityResources.Format(nameof (IdentityAccountNameCollisionRepairFailedError), culture, arg0);
    }

    public static string IdentityAccountNameCollisionRepairUnsafeError(object arg0) => IdentityResources.Format(nameof (IdentityAccountNameCollisionRepairUnsafeError), arg0);

    public static string IdentityAccountNameCollisionRepairUnsafeError(
      object arg0,
      CultureInfo culture)
    {
      return IdentityResources.Format(nameof (IdentityAccountNameCollisionRepairUnsafeError), culture, arg0);
    }

    public static string IdentityAliasAlreadyInUseError(object arg0) => IdentityResources.Format(nameof (IdentityAliasAlreadyInUseError), arg0);

    public static string IdentityAliasAlreadyInUseError(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (IdentityAliasAlreadyInUseError), culture, arg0);

    public static string InvalidNameNotRecognized(object arg0) => IdentityResources.Format(nameof (InvalidNameNotRecognized), arg0);

    public static string InvalidNameNotRecognized(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (InvalidNameNotRecognized), culture, arg0);

    public static string IdentityMapReadOnlyException() => IdentityResources.Get(nameof (IdentityMapReadOnlyException));

    public static string IdentityMapReadOnlyException(CultureInfo culture) => IdentityResources.Get(nameof (IdentityMapReadOnlyException), culture);

    public static string IdentityAccountNamesAlreadyInUseError(object arg0, object arg1) => IdentityResources.Format(nameof (IdentityAccountNamesAlreadyInUseError), arg0, arg1);

    public static string IdentityAccountNamesAlreadyInUseError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return IdentityResources.Format(nameof (IdentityAccountNamesAlreadyInUseError), culture, arg0, arg1);
    }

    public static string InvalidServiceIdentityName(object arg0) => IdentityResources.Format(nameof (InvalidServiceIdentityName), arg0);

    public static string InvalidServiceIdentityName(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (InvalidServiceIdentityName), culture, arg0);

    public static string AccountPreferencesAlreadyExist() => IdentityResources.Get(nameof (AccountPreferencesAlreadyExist));

    public static string AccountPreferencesAlreadyExist(CultureInfo culture) => IdentityResources.Get(nameof (AccountPreferencesAlreadyExist), culture);

    public static string ADDGROUPMEMBERILLEGALINTERNETIDENTITY(object arg0) => IdentityResources.Format(nameof (ADDGROUPMEMBERILLEGALINTERNETIDENTITY), arg0);

    public static string ADDGROUPMEMBERILLEGALINTERNETIDENTITY(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (ADDGROUPMEMBERILLEGALINTERNETIDENTITY), culture, arg0);

    public static string ADDGROUPMEMBERILLEGALWINDOWSIDENTITY(object arg0) => IdentityResources.Format(nameof (ADDGROUPMEMBERILLEGALWINDOWSIDENTITY), arg0);

    public static string ADDGROUPMEMBERILLEGALWINDOWSIDENTITY(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (ADDGROUPMEMBERILLEGALWINDOWSIDENTITY), culture, arg0);

    public static string ADDPROJECTGROUPTPROJECTMISMATCHERROR(object arg0, object arg1) => IdentityResources.Format(nameof (ADDPROJECTGROUPTPROJECTMISMATCHERROR), arg0, arg1);

    public static string ADDPROJECTGROUPTPROJECTMISMATCHERROR(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return IdentityResources.Format(nameof (ADDPROJECTGROUPTPROJECTMISMATCHERROR), culture, arg0, arg1);
    }

    public static string CANNOT_REMOVE_SERVICE_ACCOUNT() => IdentityResources.Get(nameof (CANNOT_REMOVE_SERVICE_ACCOUNT));

    public static string CANNOT_REMOVE_SERVICE_ACCOUNT(CultureInfo culture) => IdentityResources.Get(nameof (CANNOT_REMOVE_SERVICE_ACCOUNT), culture);

    public static string IDENTITYDOMAINDOESNOTEXISTERROR(object arg0) => IdentityResources.Format(nameof (IDENTITYDOMAINDOESNOTEXISTERROR), arg0);

    public static string IDENTITYDOMAINDOESNOTEXISTERROR(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (IDENTITYDOMAINDOESNOTEXISTERROR), culture, arg0);

    public static string IDENTITYDOMAINMISMATCHERROR(object arg0, object arg1) => IdentityResources.Format(nameof (IDENTITYDOMAINMISMATCHERROR), arg0, arg1);

    public static string IDENTITYDOMAINMISMATCHERROR(object arg0, object arg1, CultureInfo culture) => IdentityResources.Format(nameof (IDENTITYDOMAINMISMATCHERROR), culture, arg0, arg1);

    public static string IdentityProviderUnavailable(object arg0, object arg1) => IdentityResources.Format(nameof (IdentityProviderUnavailable), arg0, arg1);

    public static string IdentityProviderUnavailable(object arg0, object arg1, CultureInfo culture) => IdentityResources.Format(nameof (IdentityProviderUnavailable), culture, arg0, arg1);

    public static string IDENTITY_SYNC_ERROR(object arg0) => IdentityResources.Format(nameof (IDENTITY_SYNC_ERROR), arg0);

    public static string IDENTITY_SYNC_ERROR(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (IDENTITY_SYNC_ERROR), culture, arg0);

    public static string IllegalIdentityException(object arg0) => IdentityResources.Format(nameof (IllegalIdentityException), arg0);

    public static string IllegalIdentityException(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (IllegalIdentityException), culture, arg0);

    public static string MODIFYEVERYONEGROUPEXCEPTION() => IdentityResources.Get(nameof (MODIFYEVERYONEGROUPEXCEPTION));

    public static string MODIFYEVERYONEGROUPEXCEPTION(CultureInfo culture) => IdentityResources.Get(nameof (MODIFYEVERYONEGROUPEXCEPTION), culture);

    public static string NOT_APPLICATION_GROUP() => IdentityResources.Get(nameof (NOT_APPLICATION_GROUP));

    public static string NOT_APPLICATION_GROUP(CultureInfo culture) => IdentityResources.Get(nameof (NOT_APPLICATION_GROUP), culture);

    public static string NOT_A_SECURITY_GROUP(object arg0) => IdentityResources.Format(nameof (NOT_A_SECURITY_GROUP), arg0);

    public static string NOT_A_SECURITY_GROUP(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (NOT_A_SECURITY_GROUP), culture, arg0);

    public static string REMOVENONEXISTENTGROUPERROR(object arg0) => IdentityResources.Format(nameof (REMOVENONEXISTENTGROUPERROR), arg0);

    public static string REMOVENONEXISTENTGROUPERROR(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (REMOVENONEXISTENTGROUPERROR), culture, arg0);

    public static string RemoveSelfFromAdminGroupError(object arg0) => IdentityResources.Format(nameof (RemoveSelfFromAdminGroupError), arg0);

    public static string RemoveSelfFromAdminGroupError(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (RemoveSelfFromAdminGroupError), culture, arg0);

    public static string ADDPROJECTGROUPTOGLOBALGROUPERROR(object arg0, object arg1) => IdentityResources.Format(nameof (ADDPROJECTGROUPTOGLOBALGROUPERROR), arg0, arg1);

    public static string ADDPROJECTGROUPTOGLOBALGROUPERROR(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return IdentityResources.Format(nameof (ADDPROJECTGROUPTOGLOBALGROUPERROR), culture, arg0, arg1);
    }

    public static string DynamicIdentityTypeCreationNotSupported() => IdentityResources.Get(nameof (DynamicIdentityTypeCreationNotSupported));

    public static string DynamicIdentityTypeCreationNotSupported(CultureInfo culture) => IdentityResources.Get(nameof (DynamicIdentityTypeCreationNotSupported), culture);

    public static string TooManyResultsError() => IdentityResources.Get(nameof (TooManyResultsError));

    public static string TooManyResultsError(CultureInfo culture) => IdentityResources.Get(nameof (TooManyResultsError), culture);

    public static string IncompatibleScopeError(object arg0, object arg1) => IdentityResources.Format(nameof (IncompatibleScopeError), arg0, arg1);

    public static string IncompatibleScopeError(object arg0, object arg1, CultureInfo culture) => IdentityResources.Format(nameof (IncompatibleScopeError), culture, arg0, arg1);

    public static string InvalidIdentityIdTranslations() => IdentityResources.Get(nameof (InvalidIdentityIdTranslations));

    public static string InvalidIdentityIdTranslations(CultureInfo culture) => IdentityResources.Get(nameof (InvalidIdentityIdTranslations), culture);

    public static string MultipleIdentitiesFoundError(object arg0, object arg1) => IdentityResources.Format(nameof (MultipleIdentitiesFoundError), arg0, arg1);

    public static string MultipleIdentitiesFoundError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return IdentityResources.Format(nameof (MultipleIdentitiesFoundError), culture, arg0, arg1);
    }

    public static string IdentityIdTranslationsAreMigrated() => IdentityResources.Get(nameof (IdentityIdTranslationsAreMigrated));

    public static string IdentityIdTranslationsAreMigrated(CultureInfo culture) => IdentityResources.Get(nameof (IdentityIdTranslationsAreMigrated), culture);

    public static string InvalidGetDescriptorRequestWithLocalId(object arg0) => IdentityResources.Format(nameof (InvalidGetDescriptorRequestWithLocalId), arg0);

    public static string InvalidGetDescriptorRequestWithLocalId(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (InvalidGetDescriptorRequestWithLocalId), culture, arg0);

    public static string IdentityMaterializationFailedMessage(object arg0) => IdentityResources.Format(nameof (IdentityMaterializationFailedMessage), arg0);

    public static string IdentityMaterializationFailedMessage(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (IdentityMaterializationFailedMessage), culture, arg0);

    public static string IdentityDescriptorNotFoundWithMasterId(object arg0) => IdentityResources.Format(nameof (IdentityDescriptorNotFoundWithMasterId), arg0);

    public static string IdentityDescriptorNotFoundWithMasterId(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (IdentityDescriptorNotFoundWithMasterId), culture, arg0);

    public static string IdentityDescriptorNotFoundWithLocalId(object arg0) => IdentityResources.Format(nameof (IdentityDescriptorNotFoundWithLocalId), arg0);

    public static string IdentityDescriptorNotFoundWithLocalId(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (IdentityDescriptorNotFoundWithLocalId), culture, arg0);

    public static string TooManyRequestedItemsError() => IdentityResources.Get(nameof (TooManyRequestedItemsError));

    public static string TooManyRequestedItemsError(CultureInfo culture) => IdentityResources.Get(nameof (TooManyRequestedItemsError), culture);

    public static string TooManyRequestedItemsErrorWithCount(object arg0, object arg1) => IdentityResources.Format(nameof (TooManyRequestedItemsErrorWithCount), arg0, arg1);

    public static string TooManyRequestedItemsErrorWithCount(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return IdentityResources.Format(nameof (TooManyRequestedItemsErrorWithCount), culture, arg0, arg1);
    }

    public static string InvalidIdentityKeyMaps() => IdentityResources.Get(nameof (InvalidIdentityKeyMaps));

    public static string InvalidIdentityKeyMaps(CultureInfo culture) => IdentityResources.Get(nameof (InvalidIdentityKeyMaps), culture);

    public static string InvitationPendingMessage(object arg0, object arg1) => IdentityResources.Format(nameof (InvitationPendingMessage), arg0, arg1);

    public static string InvitationPendingMessage(object arg0, object arg1, CultureInfo culture) => IdentityResources.Format(nameof (InvitationPendingMessage), culture, arg0, arg1);

    public static string ShouldBePersonalAccountMessage() => IdentityResources.Get(nameof (ShouldBePersonalAccountMessage));

    public static string ShouldBePersonalAccountMessage(CultureInfo culture) => IdentityResources.Get(nameof (ShouldBePersonalAccountMessage), culture);

    public static string ShouldCreatePersonalAccountMessage() => IdentityResources.Get(nameof (ShouldCreatePersonalAccountMessage));

    public static string ShouldCreatePersonalAccountMessage(CultureInfo culture) => IdentityResources.Get(nameof (ShouldCreatePersonalAccountMessage), culture);

    public static string ShouldBeWorkAccountMessage() => IdentityResources.Get(nameof (ShouldBeWorkAccountMessage));

    public static string ShouldBeWorkAccountMessage(CultureInfo culture) => IdentityResources.Get(nameof (ShouldBeWorkAccountMessage), culture);

    public static string IdentityNotFoundInCurrentDirectory() => IdentityResources.Get(nameof (IdentityNotFoundInCurrentDirectory));

    public static string IdentityNotFoundInCurrentDirectory(CultureInfo culture) => IdentityResources.Get(nameof (IdentityNotFoundInCurrentDirectory), culture);

    public static string InvalidIdentityIdException(object arg0) => IdentityResources.Format(nameof (InvalidIdentityIdException), arg0);

    public static string InvalidIdentityIdException(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (InvalidIdentityIdException), culture, arg0);

    public static string InvalidIdentityDescriptorException(object arg0) => IdentityResources.Format(nameof (InvalidIdentityDescriptorException), arg0);

    public static string InvalidIdentityDescriptorException(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (InvalidIdentityDescriptorException), culture, arg0);

    public static string RestoreGroupScopeValidationError(object arg0) => IdentityResources.Format(nameof (RestoreGroupScopeValidationError), arg0);

    public static string RestoreGroupScopeValidationError(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (RestoreGroupScopeValidationError), culture, arg0);

    public static string AccountOwnerCannotBeRemovedFromGroup(object arg0) => IdentityResources.Format(nameof (AccountOwnerCannotBeRemovedFromGroup), arg0);

    public static string AccountOwnerCannotBeRemovedFromGroup(object arg0, CultureInfo culture) => IdentityResources.Format(nameof (AccountOwnerCannotBeRemovedFromGroup), culture, arg0);

    public static string ProjectCollectionAdministrators() => IdentityResources.Get(nameof (ProjectCollectionAdministrators));

    public static string ProjectCollectionAdministrators(CultureInfo culture) => IdentityResources.Get(nameof (ProjectCollectionAdministrators), culture);
  }
}
