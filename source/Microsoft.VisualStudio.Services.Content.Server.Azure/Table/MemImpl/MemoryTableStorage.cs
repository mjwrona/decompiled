// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl.MemoryTableStorage
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl
{
  public class MemoryTableStorage
  {
    private readonly ConcurrentDictionary<string, MemoryTable> tables = new ConcurrentDictionary<string, MemoryTable>((IEqualityComparer<string>) StringComparer.Ordinal);
    public static readonly MemoryTableStorage Global = new MemoryTableStorage("GLOBAL");
    public readonly string AccountName;

    public IClock testClock { get; }

    public MemoryTableStorage(string accountName, IClock testClock = null)
    {
      this.AccountName = accountName;
      this.testClock = testClock ?? UtcClock.Instance;
    }

    public bool IsReadOnly => false;

    public bool RequiresVssRequestContext => false;

    public bool TryGetTable(string tableName, out MemoryTable table) => this.tables.TryGetValue(tableName, out table);

    public bool CreateTableIfNotExists(string tableName) => this.tables.GetOrAdd<string, MemoryTable>(tableName, (Func<string, MemoryTable>) (_ => new MemoryTable()), out MemoryTable _);

    public bool DeleteTableIfExists(string tableName) => this.tables.TryRemove(tableName, out MemoryTable _);

    public void Reset() => this.tables.Clear();

    public bool IsEmpty => this.tables.Count == 0 || this.tables.Values.All<MemoryTable>((Func<MemoryTable, bool>) (t => t.IsEmpty));

    public override int GetHashCode()
    {
      int hashCode = this.AccountName.GetHashCode();
      foreach (KeyValuePair<string, MemoryTable> table in this.tables)
      {
        hashCode = hashCode << 1 | hashCode >> 31 & 1;
        hashCode |= table.Key.GetHashCode();
        hashCode |= table.Value.GetHashCode();
      }
      return hashCode;
    }
  }
}
