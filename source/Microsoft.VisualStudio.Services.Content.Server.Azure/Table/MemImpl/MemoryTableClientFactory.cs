// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl.MemoryTableClientFactory
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl
{
  public class MemoryTableClientFactory : ITableClientFactory, IDisposable
  {
    protected readonly List<MemoryTableStorage> storages;
    protected readonly string tableName;

    public MemoryTableClientFactory(MemoryTableStorage storage, string tableName)
    {
      this.storages = new List<MemoryTableStorage>()
      {
        storage
      };
      this.tableName = tableName;
    }

    public MemoryTableClientFactory(List<MemoryTableStorage> storages, string tableName)
    {
      this.storages = storages;
      this.tableName = tableName;
    }

    public bool IsReadOnly => false;

    public bool RequiresVssRequestContext => false;

    public virtual IEnumerable<ITable> GetAllTables() => (IEnumerable<ITable>) this.storages.Select<MemoryTableStorage, MemoryTableAdapter>((Func<MemoryTableStorage, MemoryTableAdapter>) (s => new MemoryTableAdapter(this.tableName, s)));

    public virtual ITable GetTable(string shardHint) => (ITable) new MemoryTableAdapter(this.tableName, this.storages[Math.Abs((shardHint ?? "").GetHashCode()) % this.storages.Count]);

    protected virtual void Dispose(bool disposing)
    {
    }

    public void Dispose() => this.Dispose(true);
  }
}
