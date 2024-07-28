// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.Resources
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Devops.Tags.Server
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.Azure.Devops.Tags.Server.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.Azure.Devops.Tags.Server.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.Azure.Devops.Tags.Server.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.Azure.Devops.Tags.Server.Resources.Get(resourceName) : Microsoft.Azure.Devops.Tags.Server.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.Azure.Devops.Tags.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.Azure.Devops.Tags.Server.Resources.GetInt(resourceName) : (int) Microsoft.Azure.Devops.Tags.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.Azure.Devops.Tags.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.Azure.Devops.Tags.Server.Resources.GetBool(resourceName) : (bool) Microsoft.Azure.Devops.Tags.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.Azure.Devops.Tags.Server.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.Azure.Devops.Tags.Server.Resources.Get(resourceName, culture);
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

    public static string InvalidTagName_InvalidCharactersException() => Microsoft.Azure.Devops.Tags.Server.Resources.Get(nameof (InvalidTagName_InvalidCharactersException));

    public static string InvalidTagName_InvalidCharactersException(CultureInfo culture) => Microsoft.Azure.Devops.Tags.Server.Resources.Get(nameof (InvalidTagName_InvalidCharactersException), culture);

    public static string InvalidTagName_Empty() => Microsoft.Azure.Devops.Tags.Server.Resources.Get(nameof (InvalidTagName_Empty));

    public static string InvalidTagName_Empty(CultureInfo culture) => Microsoft.Azure.Devops.Tags.Server.Resources.Get(nameof (InvalidTagName_Empty), culture);

    public static string InvalidTagName_TooLong(object arg0) => Microsoft.Azure.Devops.Tags.Server.Resources.Format(nameof (InvalidTagName_TooLong), arg0);

    public static string InvalidTagName_TooLong(object arg0, CultureInfo culture) => Microsoft.Azure.Devops.Tags.Server.Resources.Format(nameof (InvalidTagName_TooLong), culture, arg0);

    public static string InvalidTagName_Separator(object arg0) => Microsoft.Azure.Devops.Tags.Server.Resources.Format(nameof (InvalidTagName_Separator), arg0);

    public static string InvalidTagName_Separator(object arg0, CultureInfo culture) => Microsoft.Azure.Devops.Tags.Server.Resources.Format(nameof (InvalidTagName_Separator), culture, arg0);

    public static string InvalidTagName_InvalidCharacters(object arg0) => Microsoft.Azure.Devops.Tags.Server.Resources.Format(nameof (InvalidTagName_InvalidCharacters), arg0);

    public static string InvalidTagName_InvalidCharacters(object arg0, CultureInfo culture) => Microsoft.Azure.Devops.Tags.Server.Resources.Format(nameof (InvalidTagName_InvalidCharacters), culture, arg0);

    public static string InvalidTagName_Duplicate(object arg0) => Microsoft.Azure.Devops.Tags.Server.Resources.Format(nameof (InvalidTagName_Duplicate), arg0);

    public static string InvalidTagName_Duplicate(object arg0, CultureInfo culture) => Microsoft.Azure.Devops.Tags.Server.Resources.Format(nameof (InvalidTagName_Duplicate), culture, arg0);

    public static string TagLimitExceeded(object arg0) => Microsoft.Azure.Devops.Tags.Server.Resources.Format(nameof (TagLimitExceeded), arg0);

    public static string TagLimitExceeded(object arg0, CultureInfo culture) => Microsoft.Azure.Devops.Tags.Server.Resources.Format(nameof (TagLimitExceeded), culture, arg0);

    public static string TagCreateAccessDenied() => Microsoft.Azure.Devops.Tags.Server.Resources.Get(nameof (TagCreateAccessDenied));

    public static string TagCreateAccessDenied(CultureInfo culture) => Microsoft.Azure.Devops.Tags.Server.Resources.Get(nameof (TagCreateAccessDenied), culture);

    public static string TagDeleteAccessDenied() => Microsoft.Azure.Devops.Tags.Server.Resources.Get(nameof (TagDeleteAccessDenied));

    public static string TagDeleteAccessDenied(CultureInfo culture) => Microsoft.Azure.Devops.Tags.Server.Resources.Get(nameof (TagDeleteAccessDenied), culture);

    public static string TagEnumerateAccessDenied() => Microsoft.Azure.Devops.Tags.Server.Resources.Get(nameof (TagEnumerateAccessDenied));

    public static string TagEnumerateAccessDenied(CultureInfo culture) => Microsoft.Azure.Devops.Tags.Server.Resources.Get(nameof (TagEnumerateAccessDenied), culture);

    public static string TagExceptionWriteAccessDenied() => Microsoft.Azure.Devops.Tags.Server.Resources.Get(nameof (TagExceptionWriteAccessDenied));

    public static string TagExceptionWriteAccessDenied(CultureInfo culture) => Microsoft.Azure.Devops.Tags.Server.Resources.Get(nameof (TagExceptionWriteAccessDenied), culture);

    public static string TagUpdateAccessDenied() => Microsoft.Azure.Devops.Tags.Server.Resources.Get(nameof (TagUpdateAccessDenied));

    public static string TagUpdateAccessDenied(CultureInfo culture) => Microsoft.Azure.Devops.Tags.Server.Resources.Get(nameof (TagUpdateAccessDenied), culture);

    public static string TagUpdateAddRemoveConflict() => Microsoft.Azure.Devops.Tags.Server.Resources.Get(nameof (TagUpdateAddRemoveConflict));

    public static string TagUpdateAddRemoveConflict(CultureInfo culture) => Microsoft.Azure.Devops.Tags.Server.Resources.Get(nameof (TagUpdateAddRemoveConflict), culture);
  }
}
