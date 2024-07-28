// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.DevSecOpsSdkResources
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  internal static class DevSecOpsSdkResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (DevSecOpsSdkResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => DevSecOpsSdkResources.s_resMgr;

    private static string Get(string resourceName) => DevSecOpsSdkResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? DevSecOpsSdkResources.Get(resourceName) : DevSecOpsSdkResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) DevSecOpsSdkResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? DevSecOpsSdkResources.GetInt(resourceName) : (int) DevSecOpsSdkResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) DevSecOpsSdkResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? DevSecOpsSdkResources.GetBool(resourceName) : (bool) DevSecOpsSdkResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => DevSecOpsSdkResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = DevSecOpsSdkResources.Get(resourceName, culture);
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

    public static string CredScanSuppressionsDeserializationError() => DevSecOpsSdkResources.Get(nameof (CredScanSuppressionsDeserializationError));

    public static string CredScanSuppressionsDeserializationError(CultureInfo culture) => DevSecOpsSdkResources.Get(nameof (CredScanSuppressionsDeserializationError), culture);

    public static string CredScanSuppressionsElementDeserializationError() => DevSecOpsSdkResources.Get(nameof (CredScanSuppressionsElementDeserializationError));

    public static string CredScanSuppressionsElementDeserializationError(CultureInfo culture) => DevSecOpsSdkResources.Get(nameof (CredScanSuppressionsElementDeserializationError), culture);

    public static string CredScanSuppressionsNoSuppressionDefinitions() => DevSecOpsSdkResources.Get(nameof (CredScanSuppressionsNoSuppressionDefinitions));

    public static string CredScanSuppressionsNoSuppressionDefinitions(CultureInfo culture) => DevSecOpsSdkResources.Get(nameof (CredScanSuppressionsNoSuppressionDefinitions), culture);

    public static string CredScanSuppressionsWithoutFileHashOrPlaceholder() => DevSecOpsSdkResources.Get(nameof (CredScanSuppressionsWithoutFileHashOrPlaceholder));

    public static string CredScanSuppressionsWithoutFileHashOrPlaceholder(CultureInfo culture) => DevSecOpsSdkResources.Get(nameof (CredScanSuppressionsWithoutFileHashOrPlaceholder), culture);

    public static string CSCAN0020_MatchDetails() => DevSecOpsSdkResources.Get(nameof (CSCAN0020_MatchDetails));

    public static string CSCAN0020_MatchDetails(CultureInfo culture) => DevSecOpsSdkResources.Get(nameof (CSCAN0020_MatchDetails), culture);

    public static string CSCAN0030_MatchDetails() => DevSecOpsSdkResources.Get(nameof (CSCAN0030_MatchDetails));

    public static string CSCAN0030_MatchDetails(CultureInfo culture) => DevSecOpsSdkResources.Get(nameof (CSCAN0030_MatchDetails), culture);

    public static string CSCAN0041_MatchDetails() => DevSecOpsSdkResources.Get(nameof (CSCAN0041_MatchDetails));

    public static string CSCAN0041_MatchDetails(CultureInfo culture) => DevSecOpsSdkResources.Get(nameof (CSCAN0041_MatchDetails), culture);

    public static string CSCAN0060_MatchDetails() => DevSecOpsSdkResources.Get(nameof (CSCAN0060_MatchDetails));

    public static string CSCAN0060_MatchDetails(CultureInfo culture) => DevSecOpsSdkResources.Get(nameof (CSCAN0060_MatchDetails), culture);

    public static string CSCAN0230_MatchDetails() => DevSecOpsSdkResources.Get(nameof (CSCAN0230_MatchDetails));

    public static string CSCAN0230_MatchDetails(CultureInfo culture) => DevSecOpsSdkResources.Get(nameof (CSCAN0230_MatchDetails), culture);

    public static string CSCAN0250_MatchDetails() => DevSecOpsSdkResources.Get(nameof (CSCAN0250_MatchDetails));

    public static string CSCAN0250_MatchDetails(CultureInfo culture) => DevSecOpsSdkResources.Get(nameof (CSCAN0250_MatchDetails), culture);

    public static string CSCAN0260_MatchDetails() => DevSecOpsSdkResources.Get(nameof (CSCAN0260_MatchDetails));

    public static string CSCAN0260_MatchDetails(CultureInfo culture) => DevSecOpsSdkResources.Get(nameof (CSCAN0260_MatchDetails), culture);

    public static string BlockReleaseDefinitionSaveWhenSecretsAreFound(object arg0) => DevSecOpsSdkResources.Format(nameof (BlockReleaseDefinitionSaveWhenSecretsAreFound), arg0);

    public static string BlockReleaseDefinitionSaveWhenSecretsAreFound(
      object arg0,
      CultureInfo culture)
    {
      return DevSecOpsSdkResources.Format(nameof (BlockReleaseDefinitionSaveWhenSecretsAreFound), culture, arg0);
    }

    public static string BlockBuildDefinitionSaveWhenSecretsAreFound(object arg0) => DevSecOpsSdkResources.Format(nameof (BlockBuildDefinitionSaveWhenSecretsAreFound), arg0);

    public static string BlockBuildDefinitionSaveWhenSecretsAreFound(
      object arg0,
      CultureInfo culture)
    {
      return DevSecOpsSdkResources.Format(nameof (BlockBuildDefinitionSaveWhenSecretsAreFound), culture, arg0);
    }

    public static string BlockWorkItemSaveWhenSecretsAreFound(object arg0) => DevSecOpsSdkResources.Format(nameof (BlockWorkItemSaveWhenSecretsAreFound), arg0);

    public static string BlockWorkItemSaveWhenSecretsAreFound(object arg0, CultureInfo culture) => DevSecOpsSdkResources.Format(nameof (BlockWorkItemSaveWhenSecretsAreFound), culture, arg0);

    public static string BlockBuildDefinitionSaveWhenHighConfidenceSecretsAreFound(object arg0) => DevSecOpsSdkResources.Format(nameof (BlockBuildDefinitionSaveWhenHighConfidenceSecretsAreFound), arg0);

    public static string BlockBuildDefinitionSaveWhenHighConfidenceSecretsAreFound(
      object arg0,
      CultureInfo culture)
    {
      return DevSecOpsSdkResources.Format(nameof (BlockBuildDefinitionSaveWhenHighConfidenceSecretsAreFound), culture, arg0);
    }

    public static string BlockReleaseDefinitionSaveWhenHighConfidenceSecretsAreFound(object arg0) => DevSecOpsSdkResources.Format(nameof (BlockReleaseDefinitionSaveWhenHighConfidenceSecretsAreFound), arg0);

    public static string BlockReleaseDefinitionSaveWhenHighConfidenceSecretsAreFound(
      object arg0,
      CultureInfo culture)
    {
      return DevSecOpsSdkResources.Format(nameof (BlockReleaseDefinitionSaveWhenHighConfidenceSecretsAreFound), culture, arg0);
    }
  }
}
