// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Resources
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.Content.Common.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.Content.Common.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.Content.Common.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Content.Common.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.Content.Common.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.Content.Common.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Content.Common.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.Content.Common.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.Content.Common.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Content.Common.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.Content.Common.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.Content.Common.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.Content.Common.Resources.Get(resourceName, culture);
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

    public static string InvalidHexString(object arg0) => Microsoft.VisualStudio.Services.Content.Common.Resources.Format(nameof (InvalidHexString), arg0);

    public static string InvalidHexString(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Content.Common.Resources.Format(nameof (InvalidHexString), culture, arg0);

    public static string ArtifactBillingException() => Microsoft.VisualStudio.Services.Content.Common.Resources.Get(nameof (ArtifactBillingException));

    public static string ArtifactBillingException(CultureInfo culture) => Microsoft.VisualStudio.Services.Content.Common.Resources.Get(nameof (ArtifactBillingException), culture);

    public static string CancellationCaller(object arg0) => Microsoft.VisualStudio.Services.Content.Common.Resources.Format(nameof (CancellationCaller), arg0);

    public static string CancellationCaller(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Content.Common.Resources.Format(nameof (CancellationCaller), culture, arg0);

    public static string CancellationNotCaller(object arg0) => Microsoft.VisualStudio.Services.Content.Common.Resources.Format(nameof (CancellationNotCaller), arg0);

    public static string CancellationNotCaller(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Content.Common.Resources.Format(nameof (CancellationNotCaller), culture, arg0);

    public static string CancellationUnknown(object arg0) => Microsoft.VisualStudio.Services.Content.Common.Resources.Format(nameof (CancellationUnknown), arg0);

    public static string CancellationUnknown(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Content.Common.Resources.Format(nameof (CancellationUnknown), culture, arg0);
  }
}
