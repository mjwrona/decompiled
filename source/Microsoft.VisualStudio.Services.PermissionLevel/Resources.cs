// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.Resources
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.PermissionLevel
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.PermissionLevel.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.PermissionLevel.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.PermissionLevel.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.PermissionLevel.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.PermissionLevel.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.PermissionLevel.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.PermissionLevel.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.PermissionLevel.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.PermissionLevel.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.PermissionLevel.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.PermissionLevel.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.PermissionLevel.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.PermissionLevel.Resources.Get(resourceName, culture);
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

    public static string PermissionLevelDefinitionAlreadyExists(object arg0, object arg1) => Microsoft.VisualStudio.Services.PermissionLevel.Resources.Format(nameof (PermissionLevelDefinitionAlreadyExists), arg0, arg1);

    public static string PermissionLevelDefinitionAlreadyExists(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.PermissionLevel.Resources.Format(nameof (PermissionLevelDefinitionAlreadyExists), culture, arg0, arg1);
    }

    public static string PermissionLevelDefinitionNotFound(object arg0, object arg1) => Microsoft.VisualStudio.Services.PermissionLevel.Resources.Format(nameof (PermissionLevelDefinitionNotFound), arg0, arg1);

    public static string PermissionLevelDefinitionNotFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.PermissionLevel.Resources.Format(nameof (PermissionLevelDefinitionNotFound), culture, arg0, arg1);
    }

    public static string PermissionLevelAssignmentAlreadyExists(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.PermissionLevel.Resources.Format(nameof (PermissionLevelAssignmentAlreadyExists), arg0, arg1, arg2);
    }

    public static string PermissionLevelAssignmentAlreadyExists(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.PermissionLevel.Resources.Format(nameof (PermissionLevelAssignmentAlreadyExists), culture, arg0, arg1, arg2);
    }

    public static string PermissionLevelAssignmentNotFound(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.PermissionLevel.Resources.Format(nameof (PermissionLevelAssignmentNotFound), arg0, arg1, arg2);

    public static string PermissionLevelAssignmentNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.PermissionLevel.Resources.Format(nameof (PermissionLevelAssignmentNotFound), culture, arg0, arg1, arg2);
    }

    public static string PermissionLevelAssignmentUpdateScopeMismatch(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return Microsoft.VisualStudio.Services.PermissionLevel.Resources.Format(nameof (PermissionLevelAssignmentUpdateScopeMismatch), arg0, arg1, arg2, arg3);
    }

    public static string PermissionLevelAssignmentUpdateScopeMismatch(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.PermissionLevel.Resources.Format(nameof (PermissionLevelAssignmentUpdateScopeMismatch), culture, arg0, arg1, arg2, arg3);
    }
  }
}
