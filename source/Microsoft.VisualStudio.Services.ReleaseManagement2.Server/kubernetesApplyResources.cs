// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.kubernetesApplyResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class kubernetesApplyResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (kubernetesApplyResources), IntrospectionExtensions.GetTypeInfo(typeof (kubernetesApplyResources)).Assembly);

    public static ResourceManager Manager => kubernetesApplyResources.s_resMgr;

    private static string Get(string resourceName) => kubernetesApplyResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? kubernetesApplyResources.Get(resourceName) : kubernetesApplyResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) kubernetesApplyResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? kubernetesApplyResources.GetInt(resourceName) : (int) kubernetesApplyResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) kubernetesApplyResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? kubernetesApplyResources.GetBool(resourceName) : (bool) kubernetesApplyResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => kubernetesApplyResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = kubernetesApplyResources.Get(resourceName, culture);
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

    public static string Description() => kubernetesApplyResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => kubernetesApplyResources.Get(nameof (Description), culture);

    public static string Name() => kubernetesApplyResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => kubernetesApplyResources.Get(nameof (Name), culture);
  }
}
