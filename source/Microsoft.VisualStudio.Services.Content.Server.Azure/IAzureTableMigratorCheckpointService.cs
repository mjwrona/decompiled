// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.IAzureTableMigratorCheckpointService
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  [DefaultServiceImplementation(typeof (AzureTableMigratorCheckpointService))]
  public interface IAzureTableMigratorCheckpointService : IVssFrameworkService
  {
    void CheckToUseLogFile(IVssRequestContext deploymentContext, TargetHostMigration migrationEntry);

    void SaveIncrementalCopyStartTime(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      DateTime startTime,
      string vsoAreaPrefix,
      ITrackingKey key);

    DateTime? GetIncrementalCopyStartTime(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string vsoAreaPrefix,
      ITrackingKey key);

    void DeleteIncrementalCopyStartTime(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string vsoAreaPrefix,
      IEnumerable<ITrackingKey> keys);

    int GetTableCopyParallelism(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string vsoAreaPrefix,
      MigratingOperationSettings settings);

    void SetTableSyncDeleteParallelism(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string vsoAreaPrefix,
      MigratingOperationSettings settings);

    string GetPartitionKey(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string storageAccountName,
      string operation,
      string prefix,
      string vsoAreaPrefix);

    void SetPartitionKey(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string storageAccountName,
      string operation,
      string prefix,
      string partitionKey,
      string vsoAreaPrefix);

    void DeletePartitionKeys(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      IEnumerable<string> storageAccountNames,
      string operation,
      string vsoAreaPrefix);

    string GetPrefixParallelismSwitch(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string vsoAreaPrefix);

    void SetPrefixParallelismSwitch(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string isPrefixParallelismEnabled,
      string vsoAreaPrefix);

    string GetTableCopyDeleteCheckpointSwitch(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string vsoAreaPrefix);

    void SetTableCopyDeleteCheckpointSwitch(
      IVssRequestContext deploymentContext,
      TargetHostMigration migrationEntry,
      string isTableCopyDeleteEnabled,
      string vsoAreaPrefix);
  }
}
