// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachinePoolExtendedProperties
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public static class MachinePoolExtendedProperties
  {
    public const string IterationsPerRunMachineSession = "MS.VSS.MachineManagement.IterationsPerRunMachineSession";
    public const string KeepReplicatedImages = "MS.VSS.MachineManagement.KeepReplicatedImages";
    public const string MaxConcurrentPoolOperations = "MS.VSS.MachineManagement.MaxConcurrentPoolOperations";
    public const string StaggerInSeconds = "MS.VSS.MachineManagement.StaggerInSeconds";
    public const string OSDiskCaching = "OSDiskCaching";
    public const string OSDiskSizeGB = "OSDiskSizeGB";
    public const string FactoryDiskSizeGB = "FactoryDiskSizeGB";
    public const string RolePrefix = "MS.VSS.MachineManagement.RolePrefix";
    public const string RoleSize = "MS.VSS.MachineManagement.RoleSize";
    public const string StorageLocation = "MS.VSS.MachineManagement.StorageLocation";
    public const string StoragePoolName = "MS.VSS.MachineManagement.StoragePoolName";
    public const string StoragePoolSize = "MS.VSS.MachineManagement.StoragePoolSize";
    public const string StoragePoolStartIndex = "MS.VSS.MachineManagement.StoragePoolStartIndex";
    public const string MaxPoolResourceVersions = "MS.VSS.MachineManagement.MaxPoolResourceVersions";
    public const string DefaultResourceVersions = "MS.VSS.MachineManagement.DefaultResourceVersions";
    public const int IterationsPerRunMachineSessionDefault = 10;
    public const string StorageLocationDefault = "East US";
    public const int StoragePoolSizeDefault = 10;
    public const int StoragePoolStartIndexDefault = 1;
    public const string RoleSizeDefault = "Standard_DS2_v2";
    public const int MaxPoolResourceVersionsDefault = 3;
    public const string DefaultResourceVersionsDefault = "default";
    public const int OSDiskSizeGBDefault = 128;
    public const string OSDiskCachingDefault = "ReadWrite";

    public static int GetIterationsPerRunMachineSession(this MachinePool pool, int defaultValue = 10) => pool.Properties.GetValue<int>("MS.VSS.MachineManagement.IterationsPerRunMachineSession", defaultValue);

    public static void SetIterationsPerRunMachineSession(this MachinePool pool, int value) => pool.Properties["MS.VSS.MachineManagement.IterationsPerRunMachineSession"] = (object) value;

    public static int GetMaxConcurrentPoolOperations(this MachinePool pool, int defaultValue) => pool.Properties.GetValue<int>("MS.VSS.MachineManagement.MaxConcurrentPoolOperations", defaultValue);

    public static void SetMaxConcurrentPoolOperations(this MachinePool pool, int value) => pool.Properties["MS.VSS.MachineManagement.MaxConcurrentPoolOperations"] = (object) value;

    public static int GetStaggerInSeconds(this MachinePool pool, int defaultValue) => pool.Properties.GetValue<int>("MS.VSS.MachineManagement.StaggerInSeconds", defaultValue);

    public static void SetStaggerInSeconds(this MachinePool pool, int value) => pool.Properties["MS.VSS.MachineManagement.StaggerInSeconds"] = (object) value;

    public static string GetStorageLocation(this MachinePool instance, string defaultValue) => instance.Properties.GetValue<string>("MS.VSS.MachineManagement.StorageLocation", defaultValue);

    public static string GetStorageLocation(this MachinePool instance) => instance.Properties.GetValue<string>("MS.VSS.MachineManagement.StorageLocation", "East US");

    public static void SetStorageLocation(this MachinePool instance, string value) => instance.Properties["MS.VSS.MachineManagement.StorageLocation"] = (object) value;

    public static void SetStorageLocation(PropertiesCollection properties, string value) => properties["MS.VSS.MachineManagement.StorageLocation"] = (object) value;

    public static int GetStoragePoolSize(this MachinePool instance, int defaultValue) => instance.Properties.GetValue<int>("MS.VSS.MachineManagement.StoragePoolSize", defaultValue);

    public static int GetStoragePoolSize(this MachinePool instance) => instance.Properties.GetValue<int>("MS.VSS.MachineManagement.StoragePoolSize", 10);

    public static void SetStoragePoolSize(this MachinePool instance, int value) => instance.Properties["MS.VSS.MachineManagement.StoragePoolSize"] = (object) value;

    public static void SetStoragePoolSize(PropertiesCollection properties, int value) => properties["MS.VSS.MachineManagement.StoragePoolSize"] = (object) value;

    public static string GetStoragePoolName(this MachinePool instance, string defaultValue) => instance.Properties.GetValue<string>("MS.VSS.MachineManagement.StoragePoolName", defaultValue);

    public static string GetStoragePoolName(this MachinePool instance) => (string) instance.Properties["MS.VSS.MachineManagement.StoragePoolName"];

    public static void SetStoragePoolName(this MachinePool instance, string value) => instance.Properties["MS.VSS.MachineManagement.StoragePoolName"] = (object) value;

    public static void SetStoragePoolName(PropertiesCollection properties, string value) => properties["MS.VSS.MachineManagement.StoragePoolName"] = (object) value;

    public static int GetStoragePoolStartIndex(this MachinePool instance, int defaultValue) => instance.Properties.GetValue<int>("MS.VSS.MachineManagement.StoragePoolStartIndex", defaultValue);

    public static int GetStoragePoolStartIndex(this MachinePool instance) => instance.Properties.GetValue<int>("MS.VSS.MachineManagement.StoragePoolStartIndex", 1);

    public static void SetStoragePoolStartIndex(this MachinePool instance, int value) => instance.Properties["MS.VSS.MachineManagement.StoragePoolStartIndex"] = (object) value;

    public static void SetStoragePoolStartIndex(PropertiesCollection properties, int value) => properties["MS.VSS.MachineManagement.StoragePoolStartIndex"] = (object) value;

    public static string GetInstanceDeploymentName(this MachinePool pool, MachineInstance instance) => pool.GetRolePrefix() + instance.InstanceName;

    public static string GetRolePrefix(this MachinePool instance, string defaultValue) => instance.Properties.GetValue<string>("MS.VSS.MachineManagement.RolePrefix", defaultValue);

    public static string GetRolePrefix(this MachinePool instance)
    {
      string defaultValue = (instance.PoolType != null ? 0 : (instance.Region != null ? 1 : 0)) != 0 ? instance.Region.Replace(" ", "") + "-" : "";
      return instance.Properties.GetValue<string>("MS.VSS.MachineManagement.RolePrefix", defaultValue);
    }

    public static string GetRoleSize(this MachinePool instance, string defaultValue) => instance.Properties.GetValue<string>("MS.VSS.MachineManagement.RoleSize", defaultValue);

    public static string GetRoleSize(this MachinePool instance) => instance.Properties.GetValue<string>("MS.VSS.MachineManagement.RoleSize", "Standard_DS2_v2");

    public static void SetRoleSize(this MachinePool instance, string value) => instance.Properties["MS.VSS.MachineManagement.RoleSize"] = (object) value;

    public static void SetRoleSize(PropertiesCollection properties, string value) => properties["MS.VSS.MachineManagement.RoleSize"] = (object) value;

    public static int GetMaxPoolResourceVersions(this MachinePool instance, int defaultValue) => instance.Properties.GetValue<int>("MS.VSS.MachineManagement.MaxPoolResourceVersions", defaultValue);

    public static int GetMaxPoolResourceVersions(this MachinePool instance) => instance.Properties.GetValue<int>("MS.VSS.MachineManagement.MaxPoolResourceVersions", 3);

    public static void SetMaxPoolResourceVersions(this MachinePool instance, int value) => instance.Properties["MS.VSS.MachineManagement.MaxPoolResourceVersions"] = (object) value;

    public static void SetMaxPoolResourceVersions(PropertiesCollection properties, int value) => properties["MS.VSS.MachineManagement.MaxPoolResourceVersions"] = (object) value;

    public static string GetDefaultPoolResourceVersions(
      this MachinePool instance,
      string defaultValue)
    {
      return instance.Properties.GetValue<string>("MS.VSS.MachineManagement.DefaultResourceVersions", defaultValue);
    }

    public static string GetDefaultPoolResourceVersions(this MachinePool instance) => instance.Properties.GetValue<string>("MS.VSS.MachineManagement.DefaultResourceVersions", "default");

    public static void SetDefaultPoolResourceVersions(this MachinePool instance, string value) => instance.Properties["MS.VSS.MachineManagement.DefaultResourceVersions"] = (object) value;

    public static void SetDefaultPoolResourceVersions(PropertiesCollection properties, string value) => properties["MS.VSS.MachineManagement.DefaultResourceVersions"] = (object) value;

    public static int GetOSDiskSize(this MachinePool instance, int defaultValue) => instance.Properties.GetValue<int>("OSDiskSizeGB", defaultValue);

    public static int GetOSDiskSize(this MachinePool instance) => instance.Properties.GetValue<int>("OSDiskSizeGB", 128);

    public static void SetOSDiskSize(this MachinePool instance, int value) => instance.Properties["OSDiskSizeGB"] = (object) value;

    public static void SetOSDiskSize(PropertiesCollection properties, int value) => properties["OSDiskSizeGB"] = (object) value;

    public static int GetFactoryDiskSize(this MachinePool instance, int defaultValue) => instance.Properties.GetValue<int>("FactoryDiskSizeGB", defaultValue);

    public static int GetFactoryDiskSize(this MachinePool instance) => instance.Properties.GetValue<int>("FactoryDiskSizeGB", 128);

    public static void SetFactoryDiskSize(this MachinePool instance, int value) => instance.Properties["FactoryDiskSizeGB"] = (object) value;

    public static void SetFactoryDiskSize(PropertiesCollection properties, int value) => properties["FactoryDiskSizeGB"] = (object) value;

    public static string GetOSDiskCaching(this MachinePool instance, string defaultValue) => instance.Properties.GetValue<string>("OSDiskCaching", defaultValue);

    public static string GetOSDiskCaching(this MachinePool instance) => instance.Properties.GetValue<string>("OSDiskCaching", "ReadWrite");

    public static void SetOSDiskCaching(this MachinePool instance, string value) => instance.Properties["OSDiskCaching"] = (object) value;

    public static void SetOSDiskCaching(PropertiesCollection properties, string value) => properties["OSDiskCaching"] = (object) value;
  }
}
