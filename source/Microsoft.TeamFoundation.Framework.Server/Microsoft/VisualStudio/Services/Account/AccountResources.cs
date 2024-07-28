// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Account.AccountResources
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Account
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

    public static string AccountIsBeingDeleted() => AccountResources.Get(nameof (AccountIsBeingDeleted));

    public static string AccountIsBeingDeleted(CultureInfo culture) => AccountResources.Get(nameof (AccountIsBeingDeleted), culture);

    public static string AccountNameForbiddenBeginOrEndDash() => AccountResources.Get(nameof (AccountNameForbiddenBeginOrEndDash));

    public static string AccountNameForbiddenBeginOrEndDash(CultureInfo culture) => AccountResources.Get(nameof (AccountNameForbiddenBeginOrEndDash), culture);

    public static string AccountNameForbiddenCharacters() => AccountResources.Get(nameof (AccountNameForbiddenCharacters));

    public static string AccountNameForbiddenCharacters(CultureInfo culture) => AccountResources.Get(nameof (AccountNameForbiddenCharacters), culture);

    public static string AccountNameForbiddenGuid(object arg0) => AccountResources.Format(nameof (AccountNameForbiddenGuid), arg0);

    public static string AccountNameForbiddenGuid(object arg0, CultureInfo culture) => AccountResources.Format(nameof (AccountNameForbiddenGuid), culture, arg0);

    public static string AccountNameReserved() => AccountResources.Get(nameof (AccountNameReserved));

    public static string AccountNameReserved(CultureInfo culture) => AccountResources.Get(nameof (AccountNameReserved), culture);

    public static string AccountNameTooLong(object arg0) => AccountResources.Format(nameof (AccountNameTooLong), arg0);

    public static string AccountNameTooLong(object arg0, CultureInfo culture) => AccountResources.Format(nameof (AccountNameTooLong), culture, arg0);

    public static string InstanceAllocationException() => AccountResources.Get(nameof (InstanceAllocationException));

    public static string InstanceAllocationException(CultureInfo culture) => AccountResources.Get(nameof (InstanceAllocationException), culture);

    public static string InvalidPathTwoDotError(object arg0) => AccountResources.Format(nameof (InvalidPathTwoDotError), arg0);

    public static string InvalidPathTwoDotError(object arg0, CultureInfo culture) => AccountResources.Format(nameof (InvalidPathTwoDotError), culture, arg0);

    public static string RegionNameTooLong() => AccountResources.Get(nameof (RegionNameTooLong));

    public static string RegionNameTooLong(CultureInfo culture) => AccountResources.Get(nameof (RegionNameTooLong), culture);
  }
}
