// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl.MemoryTable
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl
{
  public class MemoryTable
  {
    private readonly ConcurrentDictionary<string, MemoryPartition> partitions = new ConcurrentDictionary<string, MemoryPartition>((IEqualityComparer<string>) StringComparer.Ordinal);

    public T UsePartition<T>(string partitionKey, Func<MemoryPartition, T> usePartition)
    {
      MemoryPartition orAdd = this.partitions.GetOrAdd(partitionKey, (Func<string, MemoryPartition>) (_ => new MemoryPartition()));
      lock (orAdd)
        return usePartition(orAdd);
    }

    public bool DeletePartition(string partitionKey) => this.partitions.TryRemove(partitionKey, out MemoryPartition _);

    public IEnumerable<string> PartitionNames => (IEnumerable<string>) this.partitions.Keys.OrderBy<string, string>((Func<string, string>) (p => p), (IComparer<string>) StringComparer.Ordinal);

    public bool IsEmpty => this.partitions.Count == 0 || this.partitions.Values.All<MemoryPartition>((Func<MemoryPartition, bool>) (p => p.IsEmpty));

    public override int GetHashCode()
    {
      int hashCode = 0;
      foreach (KeyValuePair<string, MemoryPartition> partition in this.partitions)
      {
        hashCode = hashCode << 1 | hashCode >> 31 & 1;
        hashCode |= partition.Key.GetHashCode();
        hashCode |= partition.Value.GetHashCode();
      }
      return hashCode;
    }
  }
}
