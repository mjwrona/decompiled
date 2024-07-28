// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.WebApiResources
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  internal static class WebApiResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (WebApiResources), typeof (WebApiResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => WebApiResources.s_resMgr;

    private static string Get(string resourceName) => WebApiResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? WebApiResources.Get(resourceName) : WebApiResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) WebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? WebApiResources.GetInt(resourceName) : (int) WebApiResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) WebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? WebApiResources.GetBool(resourceName) : (bool) WebApiResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => WebApiResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = WebApiResources.Get(resourceName, culture);
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

    public static string CSS_EMPTY_ARGUMENT(object arg0) => WebApiResources.Format(nameof (CSS_EMPTY_ARGUMENT), arg0);

    public static string CSS_EMPTY_ARGUMENT(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (CSS_EMPTY_ARGUMENT), culture, arg0);

    public static string CSS_INVALID_NAME(object arg0) => WebApiResources.Format(nameof (CSS_INVALID_NAME), arg0);

    public static string CSS_INVALID_NAME(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (CSS_INVALID_NAME), culture, arg0);

    public static string CSS_INVALID_PROJECT_PROPERTY_NAME() => WebApiResources.Get(nameof (CSS_INVALID_PROJECT_PROPERTY_NAME));

    public static string CSS_INVALID_PROJECT_PROPERTY_NAME(CultureInfo culture) => WebApiResources.Get(nameof (CSS_INVALID_PROJECT_PROPERTY_NAME), culture);

    public static string CSS_INVALID_URI(object arg0, object arg1) => WebApiResources.Format(nameof (CSS_INVALID_URI), arg0, arg1);

    public static string CSS_INVALID_URI(object arg0, object arg1, CultureInfo culture) => WebApiResources.Format(nameof (CSS_INVALID_URI), culture, arg0, arg1);

    public static string ProjectUriIdError(object arg0) => WebApiResources.Format(nameof (ProjectUriIdError), arg0);

    public static string ProjectUriIdError(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (ProjectUriIdError), culture, arg0);

    public static string CSS_PROJECT_WORK_PENDING(object arg0) => WebApiResources.Format(nameof (CSS_PROJECT_WORK_PENDING), arg0);

    public static string CSS_PROJECT_WORK_PENDING(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (CSS_PROJECT_WORK_PENDING), culture, arg0);

    public static string ProjectDoesNotExist(object arg0) => WebApiResources.Format(nameof (ProjectDoesNotExist), arg0);

    public static string ProjectDoesNotExist(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (ProjectDoesNotExist), culture, arg0);

    public static string CSS_PROJECT_DOES_NOT_EXIST_NAME(object arg0) => WebApiResources.Format(nameof (CSS_PROJECT_DOES_NOT_EXIST_NAME), arg0);

    public static string CSS_PROJECT_DOES_NOT_EXIST_NAME(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (CSS_PROJECT_DOES_NOT_EXIST_NAME), culture, arg0);

    public static string CSS_PROJECT_DOES_NOT_EXIST_URI(object arg0) => WebApiResources.Format(nameof (CSS_PROJECT_DOES_NOT_EXIST_URI), arg0);

    public static string CSS_PROJECT_DOES_NOT_EXIST_URI(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (CSS_PROJECT_DOES_NOT_EXIST_URI), culture, arg0);

    public static string CSS_PROJECT_ALREADY_EXISTS(object arg0) => WebApiResources.Format(nameof (CSS_PROJECT_ALREADY_EXISTS), arg0);

    public static string CSS_PROJECT_ALREADY_EXISTS(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (CSS_PROJECT_ALREADY_EXISTS), culture, arg0);

    public static string InvalidNameNotRecognized(object arg0) => WebApiResources.Format(nameof (InvalidNameNotRecognized), arg0);

    public static string InvalidNameNotRecognized(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (InvalidNameNotRecognized), culture, arg0);

    public static string ProjectPropertyValueTypeUnsupported(object arg0) => WebApiResources.Format(nameof (ProjectPropertyValueTypeUnsupported), arg0);

    public static string ProjectPropertyValueTypeUnsupported(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (ProjectPropertyValueTypeUnsupported), culture, arg0);

    public static string ProjectPropertyValueTooLong(object arg0) => WebApiResources.Format(nameof (ProjectPropertyValueTooLong), arg0);

    public static string ProjectPropertyValueTooLong(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (ProjectPropertyValueTooLong), culture, arg0);

    public static string InvalidHostedHardDeleteProjectError() => WebApiResources.Get(nameof (InvalidHostedHardDeleteProjectError));

    public static string InvalidHostedHardDeleteProjectError(CultureInfo culture) => WebApiResources.Get(nameof (InvalidHostedHardDeleteProjectError), culture);

    public static string InvalidLegacyDeleteProjectError() => WebApiResources.Get(nameof (InvalidLegacyDeleteProjectError));

    public static string InvalidLegacyDeleteProjectError(CultureInfo culture) => WebApiResources.Get(nameof (InvalidLegacyDeleteProjectError), culture);
  }
}
