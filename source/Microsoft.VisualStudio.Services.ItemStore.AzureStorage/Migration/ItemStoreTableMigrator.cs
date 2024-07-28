// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Migration.ItemStoreTableMigrator
// Assembly: Microsoft.VisualStudio.Services.ItemStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DF52255-B389-4C6F-82CF-18DDB4745F9C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.AzureStorage.dll

using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;

namespace Microsoft.VisualStudio.Services.ItemStore.AzureStorage.Migration
{
  public class ItemStoreTableMigrator : AzureTableMigrator
  {
    public ItemStoreTableMigrator(string vsoAreaPrefix)
      : base(vsoAreaPrefix)
    {
    }

    public ItemStoreTableMigrator(
      string vsoAreaPrefix,
      bool performTableCopy,
      bool performSyncDelete,
      TableMigrationConcurrencySetting setting)
      : base(vsoAreaPrefix, performTableCopy, performSyncDelete, setting)
    {
    }

    protected override ConcurrencyStrategy ConcurrencyStrategy => ConcurrencyStrategy.PerStorageAccount;

    protected override bool ShouldProcessPartitionKey(string pk) => true;

    protected internal override string GetShardName(
      ConsistentHashShardManager<TablePhysicalNode> manager,
      string partitonKey)
    {
      return manager.FindNode(TableKeyUtility.GetIdFromPartitonKey(partitonKey)).VirtualNode.PhysicalNode.UniqueName;
    }
  }
}
