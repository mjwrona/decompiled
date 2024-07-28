// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources
// Assembly: Microsoft.VisualStudio.Services.ItemStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DF52255-B389-4C6F-82CF-18DDB4745F9C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.AzureStorage.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ItemStore.AzureStorage
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources.Get(resourceName, culture);
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

    public static string MaximumBatchSizeArgumentExceptionMessage(object arg0, object arg1) => Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources.Format(nameof (MaximumBatchSizeArgumentExceptionMessage), arg0, arg1);

    public static string MaximumBatchSizeArgumentExceptionMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources.Format(nameof (MaximumBatchSizeArgumentExceptionMessage), culture, arg0, arg1);
    }

    public static string PartitionOutOfRangeExceptionMessage() => Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources.Get(nameof (PartitionOutOfRangeExceptionMessage));

    public static string PartitionOutOfRangeExceptionMessage(CultureInfo culture) => Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Resources.Get(nameof (PartitionOutOfRangeExceptionMessage), culture);
  }
}
