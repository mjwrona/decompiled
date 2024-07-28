// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.WebApi.DevSecOpsWebApiResources
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78BC9F0A-6262-4C96-81AF-14E523464B20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.DevSecOps.WebApi
{
  internal static class DevSecOpsWebApiResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (DevSecOpsWebApiResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => DevSecOpsWebApiResources.s_resMgr;

    private static string Get(string resourceName) => DevSecOpsWebApiResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? DevSecOpsWebApiResources.Get(resourceName) : DevSecOpsWebApiResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) DevSecOpsWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? DevSecOpsWebApiResources.GetInt(resourceName) : (int) DevSecOpsWebApiResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) DevSecOpsWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? DevSecOpsWebApiResources.GetBool(resourceName) : (bool) DevSecOpsWebApiResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => DevSecOpsWebApiResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = DevSecOpsWebApiResources.Get(resourceName, culture);
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

    public static string AdvancedSecurityViolationText(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5)
    {
      return DevSecOpsWebApiResources.Format(nameof (AdvancedSecurityViolationText), arg0, arg1, arg2, arg3, arg4, arg5);
    }

    public static string AdvancedSecurityViolationText(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      CultureInfo culture)
    {
      return DevSecOpsWebApiResources.Format(nameof (AdvancedSecurityViolationText), culture, arg0, arg1, arg2, arg3, arg4, arg5);
    }

    public static string SecretScanExpressionEvaluationTimeoutException() => DevSecOpsWebApiResources.Get(nameof (SecretScanExpressionEvaluationTimeoutException));

    public static string SecretScanExpressionEvaluationTimeoutException(CultureInfo culture) => DevSecOpsWebApiResources.Get(nameof (SecretScanExpressionEvaluationTimeoutException), culture);

    public static string ViolationText(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5)
    {
      return DevSecOpsWebApiResources.Format(nameof (ViolationText), arg0, arg1, arg2, arg3, arg4, arg5);
    }

    public static string ViolationText(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      CultureInfo culture)
    {
      return DevSecOpsWebApiResources.Format(nameof (ViolationText), culture, arg0, arg1, arg2, arg3, arg4, arg5);
    }
  }
}
