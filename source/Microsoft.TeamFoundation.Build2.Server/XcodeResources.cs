// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.XcodeResources
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class XcodeResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (XcodeResources), typeof (XcodeResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => XcodeResources.s_resMgr;

    private static string Get(string resourceName) => XcodeResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? XcodeResources.Get(resourceName) : XcodeResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) XcodeResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? XcodeResources.GetInt(resourceName) : (int) XcodeResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) XcodeResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? XcodeResources.GetBool(resourceName) : (bool) XcodeResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => XcodeResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = XcodeResources.Get(resourceName, culture);
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

    public static string Description() => XcodeResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => XcodeResources.Get(nameof (Description), culture);

    public static string Name() => XcodeResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => XcodeResources.Get(nameof (Name), culture);

    public static string WorkspacePathDescription() => XcodeResources.Get(nameof (WorkspacePathDescription));

    public static string WorkspacePathDescription(CultureInfo culture) => XcodeResources.Get(nameof (WorkspacePathDescription), culture);

    public static string WorkspacePathLabel() => XcodeResources.Get(nameof (WorkspacePathLabel));

    public static string WorkspacePathLabel(CultureInfo culture) => XcodeResources.Get(nameof (WorkspacePathLabel), culture);

    public static string SchemeDescription() => XcodeResources.Get(nameof (SchemeDescription));

    public static string SchemeDescription(CultureInfo culture) => XcodeResources.Get(nameof (SchemeDescription), culture);

    public static string SchemeLabel() => XcodeResources.Get(nameof (SchemeLabel));

    public static string SchemeLabel(CultureInfo culture) => XcodeResources.Get(nameof (SchemeLabel), culture);

    public static string XcodeDeveloperDirDescription() => XcodeResources.Get(nameof (XcodeDeveloperDirDescription));

    public static string XcodeDeveloperDirDescription(CultureInfo culture) => XcodeResources.Get(nameof (XcodeDeveloperDirDescription), culture);

    public static string XcodeDeveloperDirLabel() => XcodeResources.Get(nameof (XcodeDeveloperDirLabel));

    public static string XcodeDeveloperDirLabel(CultureInfo culture) => XcodeResources.Get(nameof (XcodeDeveloperDirLabel), culture);

    public static string XcodeVersionDescription() => XcodeResources.Get(nameof (XcodeVersionDescription));

    public static string XcodeVersionDescription(CultureInfo culture) => XcodeResources.Get(nameof (XcodeVersionDescription), culture);

    public static string XcodeVersionLabel() => XcodeResources.Get(nameof (XcodeVersionLabel));

    public static string XcodeVersionLabel(CultureInfo culture) => XcodeResources.Get(nameof (XcodeVersionLabel), culture);
  }
}
