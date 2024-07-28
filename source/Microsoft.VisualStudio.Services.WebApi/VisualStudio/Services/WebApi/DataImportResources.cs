// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.DataImportResources
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.WebApi
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

    public static string SourceIsNotDetachedDatabase() => DataImportResources.Get(nameof (SourceIsNotDetachedDatabase));

    public static string SourceIsNotDetachedDatabase(CultureInfo culture) => DataImportResources.Get(nameof (SourceIsNotDetachedDatabase), culture);

    public static string ImportInvalidSourceExtendedPropertyValue(
      object arg0,
      object arg1,
      object arg2)
    {
      return DataImportResources.Format(nameof (ImportInvalidSourceExtendedPropertyValue), arg0, arg1, arg2);
    }

    public static string ImportInvalidSourceExtendedPropertyValue(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return DataImportResources.Format(nameof (ImportInvalidSourceExtendedPropertyValue), culture, arg0, arg1, arg2);
    }

    public static string MissingSourceExtendedProperty(object arg0) => DataImportResources.Format(nameof (MissingSourceExtendedProperty), arg0);

    public static string MissingSourceExtendedProperty(object arg0, CultureInfo culture) => DataImportResources.Format(nameof (MissingSourceExtendedProperty), culture, arg0);

    public static string UnsupportedCollectionMilestone(object arg0) => DataImportResources.Format(nameof (UnsupportedCollectionMilestone), arg0);

    public static string UnsupportedCollectionMilestone(object arg0, CultureInfo culture) => DataImportResources.Format(nameof (UnsupportedCollectionMilestone), culture, arg0);

    public static string UnsupportedCollectionMilestoneForService(
      object arg0,
      object arg1,
      object arg2)
    {
      return DataImportResources.Format(nameof (UnsupportedCollectionMilestoneForService), arg0, arg1, arg2);
    }

    public static string UnsupportedCollectionMilestoneForService(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return DataImportResources.Format(nameof (UnsupportedCollectionMilestoneForService), culture, arg0, arg1, arg2);
    }

    public static string SourceIsTFSConfigurationDatabase() => DataImportResources.Get(nameof (SourceIsTFSConfigurationDatabase));

    public static string SourceIsTFSConfigurationDatabase(CultureInfo culture) => DataImportResources.Get(nameof (SourceIsTFSConfigurationDatabase), culture);

    public static string UnableToExtractDacpacInformation() => DataImportResources.Get(nameof (UnableToExtractDacpacInformation));

    public static string UnableToExtractDacpacInformation(CultureInfo culture) => DataImportResources.Get(nameof (UnableToExtractDacpacInformation), culture);

    public static string SourceDatabaseIsMissingSnapshotTable() => DataImportResources.Get(nameof (SourceDatabaseIsMissingSnapshotTable));

    public static string SourceDatabaseIsMissingSnapshotTable(CultureInfo culture) => DataImportResources.Get(nameof (SourceDatabaseIsMissingSnapshotTable), culture);

    public static string TfsMigratorVersionIsNotSupported() => DataImportResources.Get(nameof (TfsMigratorVersionIsNotSupported));

    public static string TfsMigratorVersionIsNotSupported(CultureInfo culture) => DataImportResources.Get(nameof (TfsMigratorVersionIsNotSupported), culture);

    public static string TfsMigratorVersionIsNotSupportedForPepare() => DataImportResources.Get(nameof (TfsMigratorVersionIsNotSupportedForPepare));

    public static string TfsMigratorVersionIsNotSupportedForPepare(CultureInfo culture) => DataImportResources.Get(nameof (TfsMigratorVersionIsNotSupportedForPepare), culture);

    public static string TfsMigratorVersionIsNotSupportedForImport() => DataImportResources.Get(nameof (TfsMigratorVersionIsNotSupportedForImport));

    public static string TfsMigratorVersionIsNotSupportedForImport(CultureInfo culture) => DataImportResources.Get(nameof (TfsMigratorVersionIsNotSupportedForImport), culture);

    public static string SourceDatabaseContainsExtractedData() => DataImportResources.Get(nameof (SourceDatabaseContainsExtractedData));

    public static string SourceDatabaseContainsExtractedData(CultureInfo culture) => DataImportResources.Get(nameof (SourceDatabaseContainsExtractedData), culture);

    public static string SqlPackageVersionNotSupportedException() => DataImportResources.Get(nameof (SqlPackageVersionNotSupportedException));

    public static string SqlPackageVersionNotSupportedException(CultureInfo culture) => DataImportResources.Get(nameof (SqlPackageVersionNotSupportedException), culture);

    public static string MaxDacpacSizeExceededException(object arg0, object arg1) => DataImportResources.Format(nameof (MaxDacpacSizeExceededException), arg0, arg1);

    public static string MaxDacpacSizeExceededException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DataImportResources.Format(nameof (MaxDacpacSizeExceededException), culture, arg0, arg1);
    }
  }
}
