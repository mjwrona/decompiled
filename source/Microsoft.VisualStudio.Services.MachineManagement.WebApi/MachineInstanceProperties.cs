// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineInstanceProperties
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public static class MachineInstanceProperties
  {
    public const string AdminUsername = "AdminUsername";
    public const string FactoryDiskName = "FactoryDiskName";
    public const string FactoryDiskUrl = "FactoryDiskUrl";
    public const string PreviousOrchestrationHubName = "PreviousOrchestrationHubName";
    public const string PreviousOrchestrationId = "PreviousOrchestrationId";
    public const string PublicIPAddress = "PublicIPAddress";
    public const string OSDiskName = "OSDiskName";
    public const string ResourceNameSuffix = "ResourceNameSuffix";
    public const string RoleName = "RoleName";
    public const string ServiceName = "ServiceName";
    public const string StorageAccountName = "StorageAccountName";
    public const string StorageAccountLocation = "StorageAccountLocation";
    public const string StorageContainerUrl = "StorageContainerUrl";
    public const string SupportsDeallocation = "SupportsDeallocation";
    public const string SupportsReallocationCompletedNotifications = "SupportsReallocationCompletedNotifications";
    public const string SecondaryOrchestrationId = "ProvisionImageOrchestrationId";

    public static bool GetSupportsDeallocation(this MachineInstance instance, bool defaultValue) => instance.Properties.GetValue<bool>("SupportsDeallocation", defaultValue);

    public static bool GetSupportsReallocationCompletedNotifications(
      this MachineInstance instance,
      bool defaultValue)
    {
      return instance.Properties.GetValue<bool>("SupportsReallocationCompletedNotifications", defaultValue);
    }

    public static string GetFactoryDiskName(this MachineInstance instance, string defaultValue) => instance.Properties.GetValue<string>("FactoryDiskName", defaultValue);

    public static string GetFactoryDiskName(this MachineInstance instance) => (string) instance.Properties["FactoryDiskName"];

    public static void SetFactoryDiskName(this MachineInstance instance, string value) => instance.Properties["FactoryDiskName"] = (object) value;

    public static void SetFactoryDiskName(PropertiesCollection properties, string value) => properties["FactoryDiskName"] = (object) value;

    public static Uri GetFactoryDiskUrl(this MachineInstance instance) => instance.Properties.GetUri("FactoryDiskUrl");

    public static Uri GetFactoryDiskUrl(this MachineInstance instance, Uri defaultValue) => instance.Properties.GetUri("FactoryDiskUrl", defaultValue);

    public static void SetFactoryDiskUrl(this MachineInstance instance, Uri value) => instance.Properties.SetUri("FactoryDiskUrl", value);

    public static void SetFactoryDiskUrl(PropertiesCollection properties, Uri value) => properties.SetUri("FactoryDiskUrl", value);

    public static string GetOSDiskName(this MachineInstance instance, string defaultValue) => instance.Properties.GetValue<string>("OSDiskName", defaultValue);

    public static string GetOSDiskName(this MachineInstance instance) => (string) instance.Properties["OSDiskName"];

    public static void SetOSDiskName(this MachineInstance instance, string value) => instance.Properties["OSDiskName"] = (object) value;

    public static void SetOSDiskName(PropertiesCollection properties, string value) => properties["OSDiskName"] = (object) value;

    public static string GetRoleName(this MachineInstance instance, string defaultValue) => instance.Properties.GetValue<string>("RoleName", defaultValue);

    public static string GetRoleName(this MachineInstance instance) => (string) instance.Properties["RoleName"];

    public static void SetRoleName(this MachineInstance instance, string value) => instance.Properties["RoleName"] = (object) value;

    public static void SetRoleName(PropertiesCollection properties, string value) => properties["RoleName"] = (object) value;

    public static string GetServiceName(this MachineInstance instance, string defaultValue) => instance.Properties.GetValue<string>("ServiceName", defaultValue);

    public static string GetServiceName(this MachineInstance instance) => (string) instance.Properties["ServiceName"];

    public static void SetServiceName(this MachineInstance instance, string value) => instance.Properties["ServiceName"] = (object) value;

    public static void SetServiceName(PropertiesCollection properties, string value) => properties["ServiceName"] = (object) value;

    public static Uri GetStorageContainerUrl(this MachineInstance instance) => instance.Properties.GetUri("StorageContainerUrl");

    public static Uri GetStorageContainerUrl(this MachineInstance instance, Uri defaultValue) => instance.Properties.GetUri("StorageContainerUrl", defaultValue);

    public static void SetStorageContainerUrl(this MachineInstance instance, Uri value) => instance.Properties.SetUri("StorageContainerUrl", value);

    public static void SetStorageContainerUrl(PropertiesCollection properties, Uri value) => properties.SetUri("StorageContainerUrl", value);

    public static string GetStorageAccountName(this MachineInstance instance, string defaultValue) => instance.Properties.GetValue<string>("StorageAccountName", defaultValue);

    public static string GetStorageAccountName(this MachineInstance instance) => (string) instance.Properties["StorageAccountName"];

    public static void SetStorageAccountName(this MachineInstance instance, string value) => instance.Properties["StorageAccountName"] = (object) value;

    public static void SetStorageAccountName(PropertiesCollection properties, string value) => properties["StorageAccountName"] = (object) value;

    public static string GetStorageAccountLocation(
      this MachineInstance instance,
      string defaultValue)
    {
      return instance.Properties.GetValue<string>("StorageAccountLocation", defaultValue);
    }

    public static string GetStorageAccountLocation(this MachineInstance instance) => (string) instance.Properties["StorageAccountLocation"];

    public static void SetStorageAccountLocation(this MachineInstance instance, string value) => instance.Properties["StorageAccountLocation"] = (object) value;

    public static void SetStorageAccountLocation(PropertiesCollection properties, string value) => properties["StorageAccountLocation"] = (object) value;

    public static string GetSecondaryOrchestrationId(
      this MachineInstance instance,
      string defaultValue)
    {
      return instance.Properties.GetValue<string>("ProvisionImageOrchestrationId", defaultValue);
    }

    public static string GetSecondaryOrchestrationId(this MachineInstance instance) => (string) instance.Properties["ProvisionImageOrchestrationId"];

    public static void SetSecondaryOrchestrationId(this MachineInstance instance, string value) => instance.Properties["ProvisionImageOrchestrationId"] = (object) value;

    public static void SetSecondaryOrchestrationId(PropertiesCollection properties, string value) => properties["ProvisionImageOrchestrationId"] = (object) value;
  }
}
