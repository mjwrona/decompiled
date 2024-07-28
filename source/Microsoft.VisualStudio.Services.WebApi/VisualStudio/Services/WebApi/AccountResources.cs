// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.AccountResources
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.WebApi
{
  internal static class AccountResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (AccountResources), typeof (AccountResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => AccountResources.s_resMgr;

    private static string Get(string resourceName) => AccountResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? AccountResources.Get(resourceName) : AccountResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) AccountResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? AccountResources.GetInt(resourceName) : (int) AccountResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) AccountResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? AccountResources.GetBool(resourceName) : (bool) AccountResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => AccountResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = AccountResources.Get(resourceName, culture);
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

    public static string AccountExists(object arg0) => AccountResources.Format(nameof (AccountExists), arg0);

    public static string AccountExists(object arg0, CultureInfo culture) => AccountResources.Format(nameof (AccountExists), culture, arg0);

    public static string AccountMarkedForDeletionError(object arg0) => AccountResources.Format(nameof (AccountMarkedForDeletionError), arg0);

    public static string AccountMarkedForDeletionError(object arg0, CultureInfo culture) => AccountResources.Format(nameof (AccountMarkedForDeletionError), culture, arg0);

    public static string AccountNotFound() => AccountResources.Get(nameof (AccountNotFound));

    public static string AccountNotFound(CultureInfo culture) => AccountResources.Get(nameof (AccountNotFound), culture);

    public static string AccountNotFoundByIdError(object arg0) => AccountResources.Format(nameof (AccountNotFoundByIdError), arg0);

    public static string AccountNotFoundByIdError(object arg0, CultureInfo culture) => AccountResources.Format(nameof (AccountNotFoundByIdError), culture, arg0);

    public static string AccountNotMarkedForDeletion() => AccountResources.Get(nameof (AccountNotMarkedForDeletion));

    public static string AccountNotMarkedForDeletion(CultureInfo culture) => AccountResources.Get(nameof (AccountNotMarkedForDeletion), culture);

    public static string MaxNumberOfAccountsExceptions() => AccountResources.Get(nameof (MaxNumberOfAccountsExceptions));

    public static string MaxNumberOfAccountsExceptions(CultureInfo culture) => AccountResources.Get(nameof (MaxNumberOfAccountsExceptions), culture);

    public static string MaxNumberOfAccountsPerUserException() => AccountResources.Get(nameof (MaxNumberOfAccountsPerUserException));

    public static string MaxNumberOfAccountsPerUserException(CultureInfo culture) => AccountResources.Get(nameof (MaxNumberOfAccountsPerUserException), culture);

    public static string AccountNotMarkedForDeletionError(object arg0) => AccountResources.Format(nameof (AccountNotMarkedForDeletionError), arg0);

    public static string AccountNotMarkedForDeletionError(object arg0, CultureInfo culture) => AccountResources.Format(nameof (AccountNotMarkedForDeletionError), culture, arg0);

    public static string AccountHostmappingNotFoundById(object arg0) => AccountResources.Format(nameof (AccountHostmappingNotFoundById), arg0);

    public static string AccountHostmappingNotFoundById(object arg0, CultureInfo culture) => AccountResources.Format(nameof (AccountHostmappingNotFoundById), culture, arg0);

    public static string AccountServiceLockDownModeException() => AccountResources.Get(nameof (AccountServiceLockDownModeException));

    public static string AccountServiceLockDownModeException(CultureInfo culture) => AccountResources.Get(nameof (AccountServiceLockDownModeException), culture);

    public static string AccountUserNotFoundException(object arg0, object arg1) => AccountResources.Format(nameof (AccountUserNotFoundException), arg0, arg1);

    public static string AccountUserNotFoundException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AccountResources.Format(nameof (AccountUserNotFoundException), culture, arg0, arg1);
    }

    public static string RegionExists(object arg0) => AccountResources.Format(nameof (RegionExists), arg0);

    public static string RegionExists(object arg0, CultureInfo culture) => AccountResources.Format(nameof (RegionExists), culture, arg0);

    public static string AccountNameReserved(object arg0) => AccountResources.Format(nameof (AccountNameReserved), arg0);

    public static string AccountNameReserved(object arg0, CultureInfo culture) => AccountResources.Format(nameof (AccountNameReserved), culture, arg0);

    public static string AccountServiceUnavailableException() => AccountResources.Get(nameof (AccountServiceUnavailableException));

    public static string AccountServiceUnavailableException(CultureInfo culture) => AccountResources.Get(nameof (AccountServiceUnavailableException), culture);

    public static string AccountNameTemporarilyUnavailable() => AccountResources.Get(nameof (AccountNameTemporarilyUnavailable));

    public static string AccountNameTemporarilyUnavailable(CultureInfo culture) => AccountResources.Get(nameof (AccountNameTemporarilyUnavailable), culture);

    public static string AccountMustBeUnlinkedBeforeDeletion() => AccountResources.Get(nameof (AccountMustBeUnlinkedBeforeDeletion));

    public static string AccountMustBeUnlinkedBeforeDeletion(CultureInfo culture) => AccountResources.Get(nameof (AccountMustBeUnlinkedBeforeDeletion), culture);
  }
}
