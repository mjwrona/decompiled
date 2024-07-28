// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Extensions.TableExtensionSettings
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;

namespace Microsoft.Azure.Cosmos.Table.Extensions
{
  internal static class TableExtensionSettings
  {
    static TableExtensionSettings() => TableExtensionSettings.LoadFrom(new AppSettingsWrapper());

    public static bool EnableAppSettingsBasedOptions { get; private set; }

    public static int? MaxItemCount { get; private set; }

    public static bool EnableScan { get; private set; }

    public static int MaxDegreeOfParallelism { get; private set; }

    public static int? ContinuationTokenLimitInKb { get; private set; }

    public static int TableQueryMaxBufferedItemCount { get; private set; }

    public static void LoadFrom(AppSettingsWrapper settingsWrapper)
    {
      TableExtensionSettings.EnableAppSettingsBasedOptions = TableExtensionSettings.ConvertTo<bool>(settingsWrapper.GetValue("EnableTableQueryOptions"));
      TableExtensionSettings.MaxItemCount = TableExtensionSettings.ConvertTo<int?>(settingsWrapper.GetValue("TableQueryMaxItemCount"), new int?(1000));
      TableExtensionSettings.EnableScan = TableExtensionSettings.ConvertTo<bool>(settingsWrapper.GetValue("TableQueryEnableScan"));
      TableExtensionSettings.MaxDegreeOfParallelism = TableExtensionSettings.ConvertTo<int>(settingsWrapper.GetValue("TableQueryMaxDegreeOfParallelism"), -1);
      TableExtensionSettings.ContinuationTokenLimitInKb = TableExtensionSettings.ConvertTo<int?>(settingsWrapper.GetValue("TableQueryContinuationTokenLimitInKb"));
      TableExtensionSettings.TableQueryMaxBufferedItemCount = TableExtensionSettings.ConvertTo<int>(settingsWrapper.GetValue("TableQueryMaxBufferedItemCount"), -1);
      if (!TableExtensionSettings.EnableAppSettingsBasedOptions && !string.IsNullOrEmpty(settingsWrapper.GetValue("TableQueryMaxItemCount")) && !string.IsNullOrEmpty(settingsWrapper.GetValue("TableQueryEnableScan")) && !string.IsNullOrEmpty(settingsWrapper.GetValue("TableQueryMaxDegreeOfParallelism")) && !string.IsNullOrEmpty(settingsWrapper.GetValue("TableQueryContinuationTokenLimitInKb")) && !string.IsNullOrEmpty(settingsWrapper.GetValue("TableQueryMaxBufferedItemCount")))
        throw new NotSupportedException("Options set in the App.config are not supported. Please use TableRequestOptions.");
    }

    private static T ConvertTo<T>(string value, T defaultValue = null)
    {
      if (string.IsNullOrEmpty(value))
        return defaultValue;
      Type nullableType = typeof (T);
      Type underlyingType = Nullable.GetUnderlyingType(nullableType);
      if ((object) underlyingType == null)
        underlyingType = nullableType;
      return TableExtensionSettings.ConvertTo<T>(underlyingType, value);
    }

    private static T ConvertTo<T>(Type underlyingType, string value) => underlyingType.IsEnum ? (T) Enum.Parse(underlyingType, value) : (T) Convert.ChangeType((object) value, underlyingType);
  }
}
