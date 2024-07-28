// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.AzureStorage.ItemShardingAzureCloudTableClientFactory
// Assembly: Microsoft.VisualStudio.Services.ItemStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DF52255-B389-4C6F-82CF-18DDB4745F9C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.AzureStorage.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ItemStore.AzureStorage
{
  public class ItemShardingAzureCloudTableClientFactory : ShardingAzureCloudTableClientFactory
  {
    public ItemShardingAzureCloudTableClientFactory(
      IEnumerable<StrongBoxConnectionString> accountConnectionStrings,
      Func<StrongBoxConnectionString, ITableClient> getTableClient,
      LocationMode? locationMode,
      string defaultTableName,
      string shardingStrategy,
      bool enableTracing)
      : base(accountConnectionStrings, getTableClient, locationMode, defaultTableName, shardingStrategy, enableTracing)
    {
    }

    protected override ulong GetKeyForShardHint(string shardHint) => TableKeyUtility.GetIdFromPartitonKey(shardHint);
  }
}
