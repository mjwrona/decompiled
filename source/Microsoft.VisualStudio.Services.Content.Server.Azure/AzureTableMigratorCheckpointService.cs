// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.AzureTableMigratorCheckpointService
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Content.Server.Common.Migration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class AzureTableMigratorCheckpointService : 
    IAzureTableMigratorCheckpointService,
    IVssFrameworkService
  {
    public readonly string CommonPrefix = "all";

    public virtual DateTime? GetIncrementalCopyStartTime(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string vsoAreaPrefix,
      ITrackingKey key)
    {
      string incrementalityRegistryPath = this.GetIncrementalityRegistryPath(migrationEntry, vsoAreaPrefix, key);
      string s = deploymentContext.GetService<IVssRegistryService>().GetValue(deploymentContext, (RegistryQuery) incrementalityRegistryPath, false, (string) null);
      return s != null ? new DateTime?(DateTime.ParseExact(s, "O", (IFormatProvider) CultureInfo.InvariantCulture).ToUniversalTime()) : new DateTime?();
    }

    public virtual void SaveIncrementalCopyStartTime(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      DateTime startTime,
      string vsoAreaPrefix,
      ITrackingKey key)
    {
      string incrementalityRegistryPath = this.GetIncrementalityRegistryPath(migrationEntry, vsoAreaPrefix, key);
      deploymentContext.GetService<IVssRegistryService>().SetValue<string>(deploymentContext, incrementalityRegistryPath, startTime.ToString("O", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public virtual void DeleteIncrementalCopyStartTime(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string vsoAreaPrefix,
      IEnumerable<ITrackingKey> keys)
    {
      IEnumerable<string> registryPathPatterns = keys.Select<ITrackingKey, string>((Func<ITrackingKey, string>) (key => this.GetIncrementalityRegistryPath(migrationEntry, vsoAreaPrefix, key)));
      deploymentContext.GetService<IVssRegistryService>().DeleteEntries(deploymentContext, registryPathPatterns);
    }

    public virtual int GetTableCopyParallelism(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string vsoAreaPrefix,
      MigratingOperationSettings settings)
    {
      string registryPath = this.GetRegistryPath(migrationEntry, vsoAreaPrefix);
      int prefixMaxParallelism = deploymentContext.GetService<IVssRegistryService>().GetValue<int>(deploymentContext, (RegistryQuery) ("/" + registryPath + "/DataCopyParallelism"), false, settings.PerStorageAccountAndPrefix_MaxParallelism);
      if (prefixMaxParallelism <= 0 || prefixMaxParallelism > settings.PerStorageAccountAndPrefix_MaxParallelism * 2)
        prefixMaxParallelism = settings.PerStorageAccountAndPrefix_MaxParallelism;
      return prefixMaxParallelism;
    }

    public virtual void SetTableSyncDeleteParallelism(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string vsoAreaPrefix,
      MigratingOperationSettings settings)
    {
      string registryPath = this.GetRegistryPath(migrationEntry, vsoAreaPrefix);
      IVssRegistryService service = deploymentContext.GetService<IVssRegistryService>();
      int num1 = service.GetValue<int>(deploymentContext, (RegistryQuery) ("/" + registryPath + "/DataSyncDeleteOuterParallelism"), false, settings.SyncDelete_OuterParallelism);
      if (num1 > 0 && num1 <= settings.SyncDelete_OuterParallelism * 8)
        settings.SyncDelete_OuterParallelism = num1;
      int num2 = service.GetValue<int>(deploymentContext, (RegistryQuery) ("/" + registryPath + "/DataSyncDeleteInnerParallelism"), false, settings.SyncDelete_InnerParallelism);
      if (num2 <= 0 || num2 > settings.SyncDelete_InnerParallelism * 8)
        return;
      settings.SyncDelete_InnerParallelism = num2;
    }

    public virtual string GetPartitionKey(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string storageAccountName,
      string operation,
      string prefix,
      string vsoAreaPrefix)
    {
      string defaultValue = prefix;
      if (string.IsNullOrEmpty(prefix))
        prefix = this.CommonPrefix;
      string query = this.GetPartitionKeyRegistryPath(migrationEntry, storageAccountName, operation, vsoAreaPrefix) + "/" + prefix;
      return deploymentContext.GetService<IVssRegistryService>().GetValue(deploymentContext, (RegistryQuery) query, false, defaultValue);
    }

    public virtual void SetPartitionKey(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string storageAccountName,
      string operation,
      string prefix,
      string partitionKey,
      string vsoAreaPrefix)
    {
      if (string.IsNullOrEmpty(prefix))
        prefix = this.CommonPrefix;
      string path = this.GetPartitionKeyRegistryPath(migrationEntry, storageAccountName, operation, vsoAreaPrefix) + "/" + prefix;
      deploymentContext.GetService<IVssRegistryService>().SetValue<string>(deploymentContext, path, partitionKey);
    }

    public virtual void DeletePartitionKeys(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      IEnumerable<string> storageAccountNames,
      string operation,
      string vsoAreaPrefix)
    {
      IEnumerable<string> registryPathPatterns = storageAccountNames.Select<string, string>((Func<string, string>) (san => this.GetPartitionKeyRegistryPath(migrationEntry, san, operation, vsoAreaPrefix) + "/**"));
      deploymentContext.GetService<IVssRegistryService>().DeleteEntries(deploymentContext, registryPathPatterns);
    }

    public virtual string GetPrefixParallelismSwitch(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string vsoAreaPrefix)
    {
      string path = new HostMigrationConfigManager((IServiceHostAccess) new RequestContextBasedServiceHostAccess(deploymentContext), migrationEntry.HostProperties.Id).GetPath(new Guid?(migrationEntry.HostProperties.Id), FrameworkServerConstants.ParallelMigration_EnableBlobTablePrefixParallelism);
      return deploymentContext.GetService<IVssRegistryService>().GetValue(deploymentContext, (RegistryQuery) path, false, (string) null);
    }

    public virtual void SetPrefixParallelismSwitch(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string isPrefixParallelismEnabled,
      string vsoAreaPrefix)
    {
      string path = new HostMigrationConfigManager((IServiceHostAccess) new RequestContextBasedServiceHostAccess(deploymentContext), migrationEntry.HostProperties.Id).GetPath(new Guid?(migrationEntry.HostProperties.Id), FrameworkServerConstants.ParallelMigration_EnableBlobTablePrefixParallelism);
      deploymentContext.GetService<IVssRegistryService>().SetValue<string>(deploymentContext, path, isPrefixParallelismEnabled);
    }

    public virtual string GetTableCopyDeleteCheckpointSwitch(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string vsoAreaPrefix)
    {
      string path = new HostMigrationConfigManager((IServiceHostAccess) new RequestContextBasedServiceHostAccess(deploymentContext), migrationEntry.HostProperties.Id).GetPath(new Guid?(migrationEntry.HostProperties.Id), FrameworkServerConstants.ParallelMigration_EnableBlobTableMigrationCheckpoints);
      return deploymentContext.GetService<IVssRegistryService>().GetValue(deploymentContext, (RegistryQuery) path, false, (string) null);
    }

    public virtual void SetTableCopyDeleteCheckpointSwitch(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string isTableCopyDeleteEnabled,
      string vsoAreaPrefix)
    {
      string path = new HostMigrationConfigManager((IServiceHostAccess) new RequestContextBasedServiceHostAccess(deploymentContext), migrationEntry.HostProperties.Id).GetPath(new Guid?(migrationEntry.HostProperties.Id), FrameworkServerConstants.ParallelMigration_EnableBlobTableMigrationCheckpoints);
      deploymentContext.GetService<IVssRegistryService>().SetValue<string>(deploymentContext, path, isTableCopyDeleteEnabled);
    }

    public virtual void CheckToUseLogFile(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private string GetPartitionKeyRegistryPath(
      TargetHostMigration migrationEntry,
      string storageAccountName,
      string operation,
      string vsoAreaPrefix)
    {
      return "/" + this.GetRegistryPath(migrationEntry, vsoAreaPrefix, (ITrackingKey) new UncategoriedTrackingKey(storageAccountName)) + "/" + operation;
    }

    private string GetIncrementalityRegistryPath(
      TargetHostMigration migrationEntry,
      string vsoAreaPrefix,
      ITrackingKey key)
    {
      return "/" + this.GetRegistryPath(migrationEntry, vsoAreaPrefix, key) + "/IncrementalCopyStartTime";
    }

    private string GetRegistryPath(
      TargetHostMigration migrationEntry,
      string vsoAreaPrefix,
      ITrackingKey key = null)
    {
      string registryPath = key?.RegistryPath;
      string str = string.IsNullOrEmpty(registryPath) ? "" : "/" + registryPath;
      return "AzureTableMigrator/" + vsoAreaPrefix + "/" + migrationEntry.HostProperties.Id.ToString("N") + str;
    }
  }
}
