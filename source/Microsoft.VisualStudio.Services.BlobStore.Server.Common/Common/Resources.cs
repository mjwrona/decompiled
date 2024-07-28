// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Get(resourceName, culture);
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

    public static string DefaultDomainNullError() => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Get(nameof (DefaultDomainNullError));

    public static string DefaultDomainNullError(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Get(nameof (DefaultDomainNullError), culture);

    public static string DomainNotFound(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Format(nameof (DomainNotFound), arg0);

    public static string DomainNotFound(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Format(nameof (DomainNotFound), culture, arg0);

    public static string DomainRequestNullError() => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Get(nameof (DomainRequestNullError));

    public static string DomainRequestNullError(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Get(nameof (DomainRequestNullError), culture);

    public static string ElevatedRequestContextRequired() => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Get(nameof (ElevatedRequestContextRequired));

    public static string ElevatedRequestContextRequired(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Get(nameof (ElevatedRequestContextRequired), culture);

    public static string InvalidMarkBeforeDate() => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Get(nameof (InvalidMarkBeforeDate));

    public static string InvalidMarkBeforeDate(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Get(nameof (InvalidMarkBeforeDate), culture);

    public static string InvalidSweepingDate() => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Get(nameof (InvalidSweepingDate));

    public static string InvalidSweepingDate(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Get(nameof (InvalidSweepingDate), culture);

    public static string ValidDefaultDomainRequired(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Format(nameof (ValidDefaultDomainRequired), arg0);

    public static string ValidDefaultDomainRequired(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Common.Resources.Format(nameof (ValidDefaultDomainRequired), culture, arg0);
  }
}
