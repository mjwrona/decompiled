// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DelegatedAuthorizationResources
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal static class DelegatedAuthorizationResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (DelegatedAuthorizationResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => DelegatedAuthorizationResources.s_resMgr;

    private static string Get(string resourceName) => DelegatedAuthorizationResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? DelegatedAuthorizationResources.Get(resourceName) : DelegatedAuthorizationResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) DelegatedAuthorizationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? DelegatedAuthorizationResources.GetInt(resourceName) : (int) DelegatedAuthorizationResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) DelegatedAuthorizationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? DelegatedAuthorizationResources.GetBool(resourceName) : (bool) DelegatedAuthorizationResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => DelegatedAuthorizationResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = DelegatedAuthorizationResources.Get(resourceName, culture);
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

    public static string DrawerEmptyGuid() => DelegatedAuthorizationResources.Get(nameof (DrawerEmptyGuid));

    public static string DrawerEmptyGuid(CultureInfo culture) => DelegatedAuthorizationResources.Get(nameof (DrawerEmptyGuid), culture);

    public static string InvalidUserIdPassed() => DelegatedAuthorizationResources.Get(nameof (InvalidUserIdPassed));

    public static string InvalidUserIdPassed(CultureInfo culture) => DelegatedAuthorizationResources.Get(nameof (InvalidUserIdPassed), culture);

    public static string MediumTrustRequiresServiceIdentity() => DelegatedAuthorizationResources.Get(nameof (MediumTrustRequiresServiceIdentity));

    public static string MediumTrustRequiresServiceIdentity(CultureInfo culture) => DelegatedAuthorizationResources.Get(nameof (MediumTrustRequiresServiceIdentity), culture);

    public static string MediumTrustRequiresServiceIdentityWithDescriptor(object arg0) => DelegatedAuthorizationResources.Format(nameof (MediumTrustRequiresServiceIdentityWithDescriptor), arg0);

    public static string MediumTrustRequiresServiceIdentityWithDescriptor(
      object arg0,
      CultureInfo culture)
    {
      return DelegatedAuthorizationResources.Format(nameof (MediumTrustRequiresServiceIdentityWithDescriptor), culture, arg0);
    }

    public static string NoPermissionToCreateRegistration(object arg0) => DelegatedAuthorizationResources.Format(nameof (NoPermissionToCreateRegistration), arg0);

    public static string NoPermissionToCreateRegistration(object arg0, CultureInfo culture) => DelegatedAuthorizationResources.Format(nameof (NoPermissionToCreateRegistration), culture, arg0);

    public static string NoPermissionToGetToken(object arg0, object arg1) => DelegatedAuthorizationResources.Format(nameof (NoPermissionToGetToken), arg0, arg1);

    public static string NoPermissionToGetToken(object arg0, object arg1, CultureInfo culture) => DelegatedAuthorizationResources.Format(nameof (NoPermissionToGetToken), culture, arg0, arg1);

    public static string NoPermissionToImpersonate(object arg0) => DelegatedAuthorizationResources.Format(nameof (NoPermissionToImpersonate), arg0);

    public static string NoPermissionToImpersonate(object arg0, CultureInfo culture) => DelegatedAuthorizationResources.Format(nameof (NoPermissionToImpersonate), culture, arg0);

    public static string NoPermissionToRemoveSshKey() => DelegatedAuthorizationResources.Get(nameof (NoPermissionToRemoveSshKey));

    public static string NoPermissionToRemoveSshKey(CultureInfo culture) => DelegatedAuthorizationResources.Get(nameof (NoPermissionToRemoveSshKey), culture);

    public static string NoPermissionToRevokeHost() => DelegatedAuthorizationResources.Get(nameof (NoPermissionToRevokeHost));

    public static string NoPermissionToRevokeHost(CultureInfo culture) => DelegatedAuthorizationResources.Get(nameof (NoPermissionToRevokeHost), culture);

    public static string NoPermissionToRevokeSessionToken(object arg0, object arg1) => DelegatedAuthorizationResources.Format(nameof (NoPermissionToRevokeSessionToken), arg0, arg1);

    public static string NoPermissionToRevokeSessionToken(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DelegatedAuthorizationResources.Format(nameof (NoPermissionToRevokeSessionToken), culture, arg0, arg1);
    }

    public static string NoPermissionToUpdateClientType() => DelegatedAuthorizationResources.Get(nameof (NoPermissionToUpdateClientType));

    public static string NoPermissionToUpdateClientType(CultureInfo culture) => DelegatedAuthorizationResources.Get(nameof (NoPermissionToUpdateClientType), culture);

    public static string NoValidSessionTokenFound(object arg0) => DelegatedAuthorizationResources.Format(nameof (NoValidSessionTokenFound), arg0);

    public static string NoValidSessionTokenFound(object arg0, CultureInfo culture) => DelegatedAuthorizationResources.Format(nameof (NoValidSessionTokenFound), culture, arg0);

    public static string RegistrationCreationFailure() => DelegatedAuthorizationResources.Get(nameof (RegistrationCreationFailure));

    public static string RegistrationCreationFailure(CultureInfo culture) => DelegatedAuthorizationResources.Get(nameof (RegistrationCreationFailure), culture);

    public static string RegistrationNotFound(object arg0) => DelegatedAuthorizationResources.Format(nameof (RegistrationNotFound), arg0);

    public static string RegistrationNotFound(object arg0, CultureInfo culture) => DelegatedAuthorizationResources.Format(nameof (RegistrationNotFound), culture, arg0);

    public static string RegistrationUpdateFailure() => DelegatedAuthorizationResources.Get(nameof (RegistrationUpdateFailure));

    public static string RegistrationUpdateFailure(CultureInfo culture) => DelegatedAuthorizationResources.Get(nameof (RegistrationUpdateFailure), culture);

    public static string GCMPATAdditionalInfo() => DelegatedAuthorizationResources.Get(nameof (GCMPATAdditionalInfo));

    public static string GCMPATAdditionalInfo(CultureInfo culture) => DelegatedAuthorizationResources.Get(nameof (GCMPATAdditionalInfo), culture);

    public static string DropPATAdditionalInfo() => DelegatedAuthorizationResources.Get(nameof (DropPATAdditionalInfo));

    public static string DropPATAdditionalInfo(CultureInfo culture) => DelegatedAuthorizationResources.Get(nameof (DropPATAdditionalInfo), culture);

    public static string VPackPATAdditionalInfo() => DelegatedAuthorizationResources.Get(nameof (VPackPATAdditionalInfo));

    public static string VPackPATAdditionalInfo(CultureInfo culture) => DelegatedAuthorizationResources.Get(nameof (VPackPATAdditionalInfo), culture);

    public static string NoPermissionToAccessData(object arg0) => DelegatedAuthorizationResources.Format(nameof (NoPermissionToAccessData), arg0);

    public static string NoPermissionToAccessData(object arg0, CultureInfo culture) => DelegatedAuthorizationResources.Format(nameof (NoPermissionToAccessData), culture, arg0);

    public static string RegistrationCreationFailureTenantId() => DelegatedAuthorizationResources.Get(nameof (RegistrationCreationFailureTenantId));

    public static string RegistrationCreationFailureTenantId(CultureInfo culture) => DelegatedAuthorizationResources.Get(nameof (RegistrationCreationFailureTenantId), culture);
  }
}
