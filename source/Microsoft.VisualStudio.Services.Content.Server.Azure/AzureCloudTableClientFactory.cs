// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.AzureCloudTableClientFactory
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class AzureCloudTableClientFactory : ITableClientFactory, IDisposable
  {
    private readonly string defaultTable;
    private readonly object sync = new object();
    private readonly Lazy<OrderedUpdateableClientList> clients;
    private readonly List<string> connectionStrings;
    private readonly AzureCloudTableClientFactory.GenerateShardIndex shardIndexGenerator;
    private LocationMode? currentLocationMode;

    public AzureCloudTableClientFactory(
      string accountConnectionString,
      string defaultTableName,
      AzureCloudTableClientFactory.GenerateShardIndex shardIndexGenerator,
      LocationMode? locationMode,
      IRetryPolicy overrideRetryPolicy,
      bool enableTracing)
      : this((IEnumerable<string>) new List<string>()
      {
        accountConnectionString
      }, defaultTableName, shardIndexGenerator, locationMode, overrideRetryPolicy, enableTracing)
    {
    }

    public AzureCloudTableClientFactory(
      IEnumerable<string> accountConnectionStrings,
      string defaultTableName,
      AzureCloudTableClientFactory.GenerateShardIndex shardIndexGenerator,
      LocationMode? locationMode,
      IRetryPolicy overrideRetryPolicy,
      bool enableTracing)
    {
      AzureCloudTableClientFactory tableClientFactory = this;
      this.shardIndexGenerator = shardIndexGenerator;
      this.defaultTable = defaultTableName;
      this.connectionStrings = accountConnectionStrings.ToList<string>();
      this.currentLocationMode = locationMode;
      this.clients = new Lazy<OrderedUpdateableClientList>((Func<OrderedUpdateableClientList>) (() => new OrderedUpdateableClientList((IEnumerable<string>) tableClientFactory.connectionStrings, tableClientFactory.currentLocationMode, overrideRetryPolicy)));
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

    protected int ClientCount => this.connectionStrings.Count;

    protected IEnumerable<ITableClient> Clients => (IEnumerable<ITableClient>) this.clients.Value.Clients;

    public bool RequiresVssRequestContext { get; private set; }

    public void Dispose() => this.Dispose(true);

    public virtual ITable GetTable(string shardHint) => this.GetTableClient(shardHint).GetTableReference(this.defaultTable);

    public virtual IEnumerable<ITable> GetAllTables() => this.Clients.Select<ITableClient, ITable>((Func<ITableClient, ITable>) (client => client.GetTableReference(this.defaultTable)));

    protected virtual void Dispose(bool disposing)
    {
    }

    protected void UpdateConnectionString(string oldConnectionString, string newConnectionString)
    {
      lock (this.sync)
      {
        if (!this.connectionStrings.Contains(oldConnectionString))
          return;
        this.connectionStrings.Remove(oldConnectionString);
        this.connectionStrings.Add(newConnectionString);
        if (!this.clients.IsValueCreated)
          return;
        this.clients.Value.UpdateConnectionString(oldConnectionString, newConnectionString);
      }
    }

    protected void UpdateLocationMode(LocationMode? newLocationMode)
    {
      lock (this.sync)
      {
        if (this.clients.IsValueCreated)
          this.clients.Value.UpdateLocationMode(newLocationMode);
        else
          this.currentLocationMode = newLocationMode;
      }
    }

    private ITableClient GetTableClient(string shardHint) => this.clients.Value.Clients[Math.Abs(this.shardIndexGenerator(shardHint)) % this.ClientCount];

    public delegate int GenerateShardIndex(string shardKey);
  }
}
