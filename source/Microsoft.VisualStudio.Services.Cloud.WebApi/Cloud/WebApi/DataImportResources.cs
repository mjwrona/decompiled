// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.DataImportResources
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  public static class DataImportResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (DataImportResources), typeof (DataImportResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => DataImportResources.s_resMgr;

    private static string Get(string resourceName) => DataImportResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? DataImportResources.Get(resourceName) : DataImportResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) DataImportResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? DataImportResources.GetInt(resourceName) : (int) DataImportResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) DataImportResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? DataImportResources.GetBool(resourceName) : (bool) DataImportResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => DataImportResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = DataImportResources.Get(resourceName, culture);
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

    public static string DataImportAlreadyExistsException(object arg0) => DataImportResources.Format(nameof (DataImportAlreadyExistsException), arg0);

    public static string DataImportAlreadyExistsException(object arg0, CultureInfo culture) => DataImportResources.Format(nameof (DataImportAlreadyExistsException), culture, arg0);

    public static string DataImportDoesNotExistException(object arg0) => DataImportResources.Format(nameof (DataImportDoesNotExistException), arg0);

    public static string DataImportDoesNotExistException(object arg0, CultureInfo culture) => DataImportResources.Format(nameof (DataImportDoesNotExistException), culture, arg0);

    public static string DataImportInvalidAccountException(object arg0, object arg1) => DataImportResources.Format(nameof (DataImportInvalidAccountException), arg0, arg1);

    public static string DataImportInvalidAccountException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DataImportResources.Format(nameof (DataImportInvalidAccountException), culture, arg0, arg1);
    }

    public static string DataImportSourceAndTargetRegionMismatch(
      object arg0,
      object arg1,
      object arg2)
    {
      return DataImportResources.Format(nameof (DataImportSourceAndTargetRegionMismatch), arg0, arg1, arg2);
    }

    public static string DataImportSourceAndTargetRegionMismatch(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return DataImportResources.Format(nameof (DataImportSourceAndTargetRegionMismatch), culture, arg0, arg1, arg2);
    }

    public static string DataImportSQLVMRegionMismatch(object arg0, object arg1) => DataImportResources.Format(nameof (DataImportSQLVMRegionMismatch), arg0, arg1);

    public static string DataImportSQLVMRegionMismatch(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DataImportResources.Format(nameof (DataImportSQLVMRegionMismatch), culture, arg0, arg1);
    }

    public static string ValidationDataHasBeenModified() => DataImportResources.Get(nameof (ValidationDataHasBeenModified));

    public static string ValidationDataHasBeenModified(CultureInfo culture) => DataImportResources.Get(nameof (ValidationDataHasBeenModified), culture);

    public static string ValidationDataChecksumIsNotValid() => DataImportResources.Get(nameof (ValidationDataChecksumIsNotValid));

    public static string ValidationDataChecksumIsNotValid(CultureInfo culture) => DataImportResources.Get(nameof (ValidationDataChecksumIsNotValid), culture);

    public static string MissingDataImportRequestProperty(object arg0) => DataImportResources.Format(nameof (MissingDataImportRequestProperty), arg0);

    public static string MissingDataImportRequestProperty(object arg0, CultureInfo culture) => DataImportResources.Format(nameof (MissingDataImportRequestProperty), culture, arg0);
  }
}
