// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.ShardingAzureCloudTableClientFactory
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common.ShardManager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public abstract class ShardingAzureCloudTableClientFactory : ITableClientFactory, IDisposable
  {
    private const int VirtualNodesPerPhysicalNode = 128;
    public readonly string DefaultTable;
    private readonly IShardManager<TablePhysicalNode> shardManager;
    internal readonly IList<ITableClient> clients;
    private LocationMode? currentLocationMode;

    protected ShardingAzureCloudTableClientFactory(
      IEnumerable<StrongBoxConnectionString> accountConnectionStrings,
      Func<StrongBoxConnectionString, ITableClient> getTableClient,
      LocationMode? locationMode,
      string defaultTableName,
      string shardingStrategy,
      bool enableTracing)
    {
      this.DefaultTable = defaultTableName;
      this.currentLocationMode = locationMode;
      this.clients = (IList<ITableClient>) new List<ITableClient>();
      List<TablePhysicalNode> physicalNodes = new List<TablePhysicalNode>();
      foreach (StrongBoxConnectionString connectionString in accountConnectionStrings)
      {
        ITableClient client = getTableClient(connectionString);
        this.clients.Add(client);
        TablePhysicalNode tablePhysicalNode = new TablePhysicalNode((Func<ITable>) (() => client.GetTableReference(this.DefaultTable)), StorageAccountUtilities.GetAccountInfo(connectionString.ConnectionString).Name);
        physicalNodes.Add(tablePhysicalNode);
      }
      this.shardManager = this.CreateTableShardManager((IEnumerable<TablePhysicalNode>) physicalNodes, shardingStrategy);
      this.RequiresVssRequestContext = enableTracing;
    }

    public bool IsReadOnly
    {
      get
      {
        LocationMode? currentLocationMode = this.currentLocationMode;
        LocationMode locationMode = LocationMode.SecondaryOnly;
        return currentLocationMode.GetValueOrDefault() == locationMode & currentLocationMode.HasValue;
      }
    }

    public bool RequiresVssRequestContext { get; private set; }

    public void Dispose() => this.Dispose(true);

    public virtual ITable GetTable(string shardHint)
    {
      using (VirtualNodeContext<TablePhysicalNode> node = this.shardManager.FindNode(this.GetKeyForShardHint(shardHint)))
        return node.GetTable();
    }

    public virtual IEnumerable<ITable> GetAllTables() => this.clients.Select<ITableClient, ITable>((Func<ITableClient, ITable>) (client => client.GetTableReference(this.DefaultTable)));

    protected abstract ulong GetKeyForShardHint(string shardHint);

    protected virtual void Dispose(bool disposing)
    {
    }

    private IShardManager<TablePhysicalNode> CreateTableShardManager(
      IEnumerable<TablePhysicalNode> physicalNodes,
      string shardingStrategy)
    {
      if (ShardingConstants.UseLinearSharding(shardingStrategy))
        return (IShardManager<TablePhysicalNode>) new LinearShardManager<TablePhysicalNode>(physicalNodes);
      if (ShardingConstants.UseConsistentHashingSharding(shardingStrategy))
        return (IShardManager<TablePhysicalNode>) new ConsistentHashShardManager<TablePhysicalNode>(physicalNodes, 128);
      throw new ArgumentException("No appropriate sharding manager is available for manager type: " + shardingStrategy);
    }
  }
}
