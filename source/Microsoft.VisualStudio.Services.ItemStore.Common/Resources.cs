// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.Resources
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.ItemStore.Common.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.ItemStore.Common.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.ItemStore.Common.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.ItemStore.Common.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.ItemStore.Common.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.ItemStore.Common.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.ItemStore.Common.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.ItemStore.Common.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.ItemStore.Common.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.ItemStore.Common.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.ItemStore.Common.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.ItemStore.Common.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.ItemStore.Common.Resources.Get(resourceName, culture);
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

    public static string ContainerNotFound(object arg0) => Microsoft.VisualStudio.Services.ItemStore.Common.Resources.Format(nameof (ContainerNotFound), arg0);

    public static string ContainerNotFound(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ItemStore.Common.Resources.Format(nameof (ContainerNotFound), culture, arg0);

    public static string DeletePendingContainer(object arg0) => Microsoft.VisualStudio.Services.ItemStore.Common.Resources.Format(nameof (DeletePendingContainer), arg0);

    public static string DeletePendingContainer(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ItemStore.Common.Resources.Format(nameof (DeletePendingContainer), culture, arg0);

    public static string ItemNotFound(object arg0, object arg1) => Microsoft.VisualStudio.Services.ItemStore.Common.Resources.Format(nameof (ItemNotFound), arg0, arg1);

    public static string ItemNotFound(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.ItemStore.Common.Resources.Format(nameof (ItemNotFound), culture, arg0, arg1);

    public static string ManifestItemException(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.ItemStore.Common.Resources.Format(nameof (ManifestItemException), arg0, arg1, arg2);

    public static string ManifestItemException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.ItemStore.Common.Resources.Format(nameof (ManifestItemException), culture, arg0, arg1, arg2);
    }

    public static string SealedContainer(object arg0) => Microsoft.VisualStudio.Services.ItemStore.Common.Resources.Format(nameof (SealedContainer), arg0);

    public static string SealedContainer(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ItemStore.Common.Resources.Format(nameof (SealedContainer), culture, arg0);

    public static string BlobItemInvalidDomain(object arg0) => Microsoft.VisualStudio.Services.ItemStore.Common.Resources.Format(nameof (BlobItemInvalidDomain), arg0);

    public static string BlobItemInvalidDomain(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ItemStore.Common.Resources.Format(nameof (BlobItemInvalidDomain), culture, arg0);
  }
}
