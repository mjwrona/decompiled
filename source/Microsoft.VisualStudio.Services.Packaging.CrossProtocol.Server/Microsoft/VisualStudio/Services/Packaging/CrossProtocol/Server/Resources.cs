// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9BF15B3F-0578-4452-9C4B-B5237E218DF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources.Get(resourceName, culture);
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

    public static string Err_SecurityNamespaceNotAvailable() => Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources.Get(nameof (Err_SecurityNamespaceNotAvailable));

    public static string Err_SecurityNamespaceNotAvailable(CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources.Get(nameof (Err_SecurityNamespaceNotAvailable), culture);

    public static string Err_UnknownItemStoreExperience(object arg0) => Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources.Format(nameof (Err_UnknownItemStoreExperience), arg0);

    public static string Err_UnknownItemStoreExperience(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Resources.Format(nameof (Err_UnknownItemStoreExperience), culture, arg0);
  }
}
