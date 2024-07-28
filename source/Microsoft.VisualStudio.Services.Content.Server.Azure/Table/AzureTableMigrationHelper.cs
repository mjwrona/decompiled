// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Table.AzureTableMigrationHelper
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Hosting;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Cloud.HostMigration;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Table
{
  public class AzureTableMigrationHelper
  {
    public static void CreateMissingTables(
      IVssRequestContext requestContext,
      List<StorageMigration> srcTables,
      List<MigrateStorageInfo> storageInfos)
    {
      if (srcTables == null || storageInfos == null)
        throw new ArgumentNullException("Both srcTables and storageInfos must not be null.");
      if (srcTables.Count != storageInfos.Count)
        throw new ArgumentException("The arguments \"srcTables\" and \"storageInfos\" are not lined up. " + string.Format("They have {0} and {1} entries respectively.", (object) srcTables.Count, (object) storageInfos.Count));
      Dictionary<string, Tuple<List<StorageMigration>, List<MigrateStorageInfo>>> dictionary = new Dictionary<string, Tuple<List<StorageMigration>, List<MigrateStorageInfo>>>();
      for (int index = 0; index < srcTables.Count; ++index)
      {
        StorageMigration srcTable = srcTables[index];
        MigrateStorageInfo storageInfo = storageInfos[index];
        if (!string.Equals(storageInfo.Name, srcTable.VsoArea, StringComparison.OrdinalIgnoreCase))
          throw new ArgumentException("The arguments \"srcTables\" and \"storageInfos\" are not lined up. " + string.Format("The names at index {0} are {1} and {2} respectively.", (object) index, (object) srcTable.VsoArea, (object) storageInfo.Name));
        Tuple<List<StorageMigration>, List<MigrateStorageInfo>> tuple;
        if (!dictionary.TryGetValue(srcTable.Id, out tuple))
        {
          tuple = new Tuple<List<StorageMigration>, List<MigrateStorageInfo>>(new List<StorageMigration>(), new List<MigrateStorageInfo>());
          dictionary[srcTable.Id] = tuple;
        }
        tuple.Item1.Add(srcTable);
        tuple.Item2.Add(storageInfo);
      }
      foreach (KeyValuePair<string, Tuple<List<StorageMigration>, List<MigrateStorageInfo>>> keyValuePair in dictionary)
      {
        string key = keyValuePair.Key;
        List<StorageMigration> srcTables1 = keyValuePair.Value.Item1;
        List<MigrateStorageInfo> storageInfos1 = keyValuePair.Value.Item2;
        AzureTableMigrationHelper.CreateMissingTables(requestContext, srcTables1, storageInfos1, key);
      }
    }

    private static bool CreateMissingTables(
      IVssRequestContext requestContext,
      List<StorageMigration> srcTables,
      List<MigrateStorageInfo> storageInfos,
      string tableId)
    {
      List<Tuple<StorageMigration, AzureProvider>> tupleList = new List<Tuple<StorageMigration, AzureProvider>>();
      int count = storageInfos.Count;
      for (int index = 0; index < count; ++index)
      {
        MigrateStorageInfo storageInfo = storageInfos[index];
        StorageMigration srcTable = srcTables[index];
        AzureProvider azureProvider = HostMigrationUtil.GetAzureProvider(requestContext, storageInfo.Drawer, storageInfo.LookupKey);
        if (srcTable.IsSharded)
          tupleList.Add(new Tuple<StorageMigration, AzureProvider>(srcTable, azureProvider));
      }
      List<CloudTable> cloudTableList = new List<CloudTable>();
      foreach (Tuple<StorageMigration, AzureProvider> tuple in tupleList)
      {
        StorageMigration storageMigration = tuple.Item1;
        AzureProvider azureProvider = tuple.Item2;
        if (storageMigration.StorageType == StorageType.Table)
        {
          CloudTable cloudTableReference = azureProvider.GetCloudTableReference(requestContext, tableId, false);
          if (!cloudTableReference.Exists())
            cloudTableList.Add(cloudTableReference);
        }
      }
      if (cloudTableList.Count == 0)
        return true;
      if (cloudTableList.Count == count)
        return false;
      if (cloudTableList.Count <= 0 || cloudTableList.Count >= count)
        throw new InvalidOperationException(string.Format("The total number of tables ({0}) are not expected. It should fall between [0, {1}].", (object) cloudTableList.Count, (object) count));
      foreach (CloudTable cloudTable in cloudTableList)
        cloudTable.CreateIfNotExists();
      return true;
    }

    public static void ResourceMigrationJobCompleted(
      IVssRequestContext deploymentContext,
      TargetHostMigration migration,
      Guid jobId,
      ResourceMigrationState stat)
    {
      using (HostMigrationComponent component = deploymentContext.CreateComponent<HostMigrationComponent>())
        component.UpdateResourceMigrationJobStatus(migration.MigrationId, jobId, stat);
      if (!migration.StorageOnly || stat < ResourceMigrationState.Complete)
        return;
      List<ResourceMigrationJob> resourceMigrationJobList;
      using (HostMigrationComponent component = deploymentContext.CreateComponent<HostMigrationComponent>())
        resourceMigrationJobList = component.QueryResourceMigrationJobs(migration.MigrationId);
      bool flag = false;
      Guid jobId1;
      foreach (ResourceMigrationJob resourceMigrationJob in resourceMigrationJobList)
      {
        switch (resourceMigrationJob.Status)
        {
          case ResourceMigrationState.Queued:
          case ResourceMigrationState.Started:
          case ResourceMigrationState.Verifing:
            flag = true;
            goto case ResourceMigrationState.Complete;
          case ResourceMigrationState.Complete:
            if (!flag)
              continue;
            goto label_23;
          case ResourceMigrationState.Failed:
            IVssRequestContext deploymentContext1 = deploymentContext;
            TargetHostMigration migration1 = migration;
            jobId1 = resourceMigrationJob.JobId;
            string message1 = "Migration cancelled due to at least one resource job getting failed: " + jobId1.ToString();
            AzureTableMigrationHelper.UpdateMigration(deploymentContext1, migration1, TargetMigrationState.Failed, message1);
            flag = true;
            goto case ResourceMigrationState.Complete;
          case ResourceMigrationState.Canceled:
            IVssRequestContext deploymentContext2 = deploymentContext;
            TargetHostMigration migration2 = migration;
            jobId1 = resourceMigrationJob.JobId;
            string message2 = "Migration cancelled due to at least one resource job getting cancelled: " + jobId1.ToString();
            AzureTableMigrationHelper.UpdateMigration(deploymentContext2, migration2, TargetMigrationState.Failed, message2);
            flag = true;
            goto case ResourceMigrationState.Complete;
          default:
            IVssRequestContext deploymentContext3 = deploymentContext;
            TargetHostMigration migration3 = migration;
            string str1 = resourceMigrationJob.Status.ToString();
            jobId1 = resourceMigrationJob.JobId;
            string str2 = jobId1.ToString();
            string message3 = "Unrecognized job state (" + str1 + ") for job: " + str2;
            AzureTableMigrationHelper.UpdateMigration(deploymentContext3, migration3, TargetMigrationState.Failed, message3);
            flag = true;
            goto case ResourceMigrationState.Complete;
        }
      }
label_23:
      if (flag)
        return;
      AzureTableMigrationHelper.UpdateMigration(deploymentContext, migration, TargetMigrationState.Complete, "StorageOnly migration is completed (updated by Artifact table copy job).");
    }

    private static void UpdateMigration(
      IVssRequestContext deploymentContext,
      TargetHostMigration migration,
      TargetMigrationState state,
      string message)
    {
      using (HostMigrationComponent component = deploymentContext.CreateComponent<HostMigrationComponent>())
        component.UpdateTargetMigration(migration.MigrationId, state, message);
    }
  }
}
