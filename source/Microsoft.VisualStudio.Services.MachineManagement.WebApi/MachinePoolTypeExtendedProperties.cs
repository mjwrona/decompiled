// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachinePoolTypeExtendedProperties
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public static class MachinePoolTypeExtendedProperties
  {
    public const string PoolManagementBackend = "MS.VSS.MachineManagement.PoolManagementBackend";
    public const string SubscriptionId = "MS.VSS.MachineManagement.SubscriptionId";
    public const string ApiCertificate = "MS.VSS.MachineManagement.ApiCertificate";
    public const string ApiUrl = "MS.VSS.MachineManagement.ApiUrl";
    public const string DiagnosticsStorageAccount = "MS.VSS.MachineManagement.DiagnosticsStorageAccount";
    public const string StorageAccount = "MS.VSS.MachineManagement.StorageAccount";
    public const string StorageLocation = "MS.VSS.MachineManagement.StorageLocation";
    public const string StoragePoolSize = "MS.VSS.MachineManagement.StoragePoolSize";
    public const string StoragePoolName = "MS.VSS.MachineManagement.StoragePoolName";
    public const string StoragePoolStartIndex = "MS.VSS.MachineManagement.StoragePoolStartIndex";
    public const string LifecycleLoopDelay = "MS.VSS.MachineManagement.LifecycleLoopDelay";
    public const string MachineStartTimeout = "MS.VSS.MachineManagement.MachineStartTimeout";
    public const string MaxPoolResourceVersions = "MS.VSS.MachineManagement.MaxPoolResourceVersions";
    public const string RolePrefix = "MS.VSS.MachineManagement.RolePrefix";
    public const string RoleSize = "MS.VSS.MachineManagement.RoleSize";
    public const string DeleteDiskTaskBatchSize = "MS.VSS.MachineManagement.DeleteDiskTask.BatchSize";
    public const string DeleteDiskTaskInstanceId = "MS.VSS.MachineManagement.DeleteDiskTask.InstanceId";
    public const string PoolManagementBackendDefault = "Workflow";
    public const string StorageLocationDefault = "East US";
    public const int StoragePoolSizeDefault = 10;
    public const int StoragePoolStartIndexDefault = 1;
    public const int LifecycleLoopDelayDefault = 60;
    public const int MachineStartTimeoutDefault = 15;
    public const string RolePrefixDefault = "";
    public const int DeleteDiskTaskBatchSizeDefault = 25;

    public static string GetPoolManagementBackend(
      this MachinePoolType instance,
      string defaultValue)
    {
      return instance.Properties.GetValue<string>("MS.VSS.MachineManagement.PoolManagementBackend", defaultValue);
    }

    public static string GetPoolManagementBackend(this MachinePoolType instance) => instance.Properties.GetValue<string>("MS.VSS.MachineManagement.PoolManagementBackend", "Workflow");

    public static void SetPoolManagementBackend(this MachinePoolType instance, string value) => instance.Properties["MS.VSS.MachineManagement.PoolManagementBackend"] = (object) value;

    public static void SetPoolManagementBackend(PropertiesCollection properties, string value) => properties["MS.VSS.MachineManagement.PoolManagementBackend"] = (object) value;

    public static string GetSubscriptionId(this MachinePoolType instance, string defaultValue) => instance.Properties.GetValue<string>("MS.VSS.MachineManagement.SubscriptionId", defaultValue);

    public static string GetSubscriptionId(this MachinePoolType instance) => (string) instance.Properties["MS.VSS.MachineManagement.SubscriptionId"];

    public static void SetSubscriptionId(this MachinePoolType instance, string value) => instance.Properties["MS.VSS.MachineManagement.SubscriptionId"] = (object) value;

    public static void SetSubscriptionId(PropertiesCollection properties, string value) => properties["MS.VSS.MachineManagement.SubscriptionId"] = (object) value;

    public static string GetApiCertificate(this MachinePoolType instance, string defaultValue) => instance.Properties.GetValue<string>("MS.VSS.MachineManagement.ApiCertificate", defaultValue);

    public static string GetApiCertificate(this MachinePoolType instance) => (string) instance.Properties["MS.VSS.MachineManagement.ApiCertificate"];

    public static void SetApiCertificate(this MachinePoolType instance, string value) => instance.Properties["MS.VSS.MachineManagement.ApiCertificate"] = (object) value;

    public static void SetApiCertificate(PropertiesCollection properties, string value) => properties["MS.VSS.MachineManagement.ApiCertificate"] = (object) value;

    public static string GetDiagnosticsStorageAccount(
      this MachinePoolType instance,
      string defaultValue)
    {
      return instance.Properties.GetValue<string>("MS.VSS.MachineManagement.DiagnosticsStorageAccount", defaultValue);
    }

    public static string GetDiagnosticsStorageAccount(this MachinePoolType instance) => (string) instance.Properties["MS.VSS.MachineManagement.DiagnosticsStorageAccount"];

    public static void SetDiagnosticsStorageAccount(this MachinePoolType instance, string value) => instance.Properties["MS.VSS.MachineManagement.DiagnosticsStorageAccount"] = (object) value;

    public static void SetDiagnosticsStorageAccount(PropertiesCollection properties, string value) => properties["MS.VSS.MachineManagement.DiagnosticsStorageAccount"] = (object) value;

    public static string GetStorageLocation(this MachinePoolType instance, string defaultValue) => instance.Properties.GetValue<string>("MS.VSS.MachineManagement.StorageLocation", defaultValue);

    public static string GetStorageLocation(this MachinePoolType instance) => instance.Properties.GetValue<string>("MS.VSS.MachineManagement.StorageLocation", "East US");

    public static void SetStorageLocation(this MachinePoolType instance, string value) => instance.Properties["MS.VSS.MachineManagement.StorageLocation"] = (object) value;

    public static void SetStorageLocation(PropertiesCollection properties, string value) => properties["MS.VSS.MachineManagement.StorageLocation"] = (object) value;

    public static int GetStoragePoolSize(this MachinePoolType instance, int defaultValue) => instance.Properties.GetValue<int>("MS.VSS.MachineManagement.StoragePoolSize", defaultValue);

    public static int GetStoragePoolSize(this MachinePoolType instance) => instance.Properties.GetValue<int>("MS.VSS.MachineManagement.StoragePoolSize", 10);

    public static void SetStoragePoolSize(this MachinePoolType instance, int value) => instance.Properties["MS.VSS.MachineManagement.StoragePoolSize"] = (object) value;

    public static void SetStoragePoolSize(PropertiesCollection properties, int value) => properties["MS.VSS.MachineManagement.StoragePoolSize"] = (object) value;

    public static string GetStoragePoolName(this MachinePoolType instance, string defaultValue) => instance.Properties.GetValue<string>("MS.VSS.MachineManagement.StoragePoolName", defaultValue);

    public static string GetStoragePoolName(this MachinePoolType instance) => (string) instance.Properties["MS.VSS.MachineManagement.StoragePoolName"];

    public static void SetStoragePoolName(this MachinePoolType instance, string value) => instance.Properties["MS.VSS.MachineManagement.StoragePoolName"] = (object) value;

    public static void SetStoragePoolName(PropertiesCollection properties, string value) => properties["MS.VSS.MachineManagement.StoragePoolName"] = (object) value;

    public static int GetStoragePoolStartIndex(this MachinePoolType instance, int defaultValue) => instance.Properties.GetValue<int>("MS.VSS.MachineManagement.StoragePoolStartIndex", defaultValue);

    public static int GetStoragePoolStartIndex(this MachinePoolType instance) => instance.Properties.GetValue<int>("MS.VSS.MachineManagement.StoragePoolStartIndex", 1);

    public static void SetStoragePoolStartIndex(this MachinePoolType instance, int value) => instance.Properties["MS.VSS.MachineManagement.StoragePoolStartIndex"] = (object) value;

    public static void SetStoragePoolStartIndex(PropertiesCollection properties, int value) => properties["MS.VSS.MachineManagement.StoragePoolStartIndex"] = (object) value;

    public static int GetLifecycleLoopDelay(this MachinePoolType instance, int defaultValue) => instance.Properties.GetValue<int>("MS.VSS.MachineManagement.LifecycleLoopDelay", defaultValue);

    public static int GetLifecycleLoopDelay(this MachinePoolType instance) => instance.Properties.GetValue<int>("MS.VSS.MachineManagement.LifecycleLoopDelay", 60);

    public static void SetLifecycleLoopDelay(this MachinePoolType instance, int value) => instance.Properties["MS.VSS.MachineManagement.LifecycleLoopDelay"] = (object) value;

    public static void SetLifecycleLoopDelay(PropertiesCollection properties, int value) => properties["MS.VSS.MachineManagement.LifecycleLoopDelay"] = (object) value;

    public static int GetMachineStartTimeout(this MachinePoolType instance, int defaultValue) => instance.Properties.GetValue<int>("MS.VSS.MachineManagement.MachineStartTimeout", defaultValue);

    public static int GetMachineStartTimeout(this MachinePoolType instance) => instance.Properties.GetValue<int>("MS.VSS.MachineManagement.MachineStartTimeout", 15);

    public static void SetMachineStartTimeout(this MachinePoolType instance, int value) => instance.Properties["MS.VSS.MachineManagement.MachineStartTimeout"] = (object) value;

    public static void SetMachineStartTimeout(PropertiesCollection properties, int value) => properties["MS.VSS.MachineManagement.MachineStartTimeout"] = (object) value;

    public static string GetRolePrefix(this MachinePoolType instance, string defaultValue) => instance.Properties.GetValue<string>("MS.VSS.MachineManagement.RolePrefix", defaultValue);

    public static string GetRolePrefix(this MachinePoolType instance) => instance.Properties.GetValue<string>("MS.VSS.MachineManagement.RolePrefix", "");

    public static void SetRolePrefix(this MachinePoolType instance, string value) => instance.Properties["MS.VSS.MachineManagement.RolePrefix"] = (object) value;

    public static void SetRolePrefix(PropertiesCollection properties, string value) => properties["MS.VSS.MachineManagement.RolePrefix"] = (object) value;

    public static int GetDeleteDiskTaskBatchSize(this MachinePoolType instance, int defaultValue) => instance.Properties.GetValue<int>("MS.VSS.MachineManagement.DeleteDiskTask.BatchSize", defaultValue);

    public static int GetDeleteDiskTaskBatchSize(this MachinePoolType instance) => instance.Properties.GetValue<int>("MS.VSS.MachineManagement.DeleteDiskTask.BatchSize", 25);

    public static void SetDeleteDiskTaskBatchSize(this MachinePoolType instance, int value) => instance.Properties["MS.VSS.MachineManagement.DeleteDiskTask.BatchSize"] = (object) value;

    public static void SetDeleteDiskTaskBatchSize(PropertiesCollection properties, int value) => properties["MS.VSS.MachineManagement.DeleteDiskTask.BatchSize"] = (object) value;

    public static string GetDeleteDiskTaskInstanceId(
      this MachinePoolType instance,
      string defaultValue)
    {
      return instance.Properties.GetValue<string>("MS.VSS.MachineManagement.DeleteDiskTask.InstanceId", defaultValue);
    }

    public static string GetDeleteDiskTaskInstanceId(this MachinePoolType instance) => (string) instance.Properties["MS.VSS.MachineManagement.DeleteDiskTask.InstanceId"];

    public static void SetDeleteDiskTaskInstanceId(this MachinePoolType instance, string value) => instance.Properties["MS.VSS.MachineManagement.DeleteDiskTask.InstanceId"] = (object) value;

    public static void SetDeleteDiskTaskInstanceId(PropertiesCollection properties, string value) => properties["MS.VSS.MachineManagement.DeleteDiskTask.InstanceId"] = (object) value;
  }
}
