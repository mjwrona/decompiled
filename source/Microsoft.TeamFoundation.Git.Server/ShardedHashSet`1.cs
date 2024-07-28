// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ShardedHashSet`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class ShardedHashSet<T> : ISet<T>, ICollection<T>, IEnumerable<T>, IEnumerable
  {
    private readonly HashSet<T>[] m_shards;
    private const int c_defaultShards = 4;

    public int Count
    {
      get
      {
        int count = 0;
        foreach (HashSet<T> shard in this.m_shards)
          count += shard.Count;
        return count;
      }
    }

    public int ShardCount => this.m_shards.Length;

    public bool IsReadOnly => false;

    public ShardedHashSet()
      : this(4)
    {
    }

    public ShardedHashSet(int shardCount)
    {
      ArgumentUtility.CheckForOutOfRange(shardCount, nameof (shardCount), 2, 20);
      if (shardCount % 2 == 1)
        ++shardCount;
      this.m_shards = new HashSet<T>[shardCount];
      for (int index = 0; index < shardCount; ++index)
        this.m_shards[index] = new HashSet<T>();
    }

    public IEnumerator<T> GetEnumerator() => this.BuildEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.BuildEnumerable().GetEnumerator();

    public bool Add(T item) => this.GetBucket(item).Add(item);

    void ICollection<T>.Add(T item) => this.Add(item);

    public void Clear()
    {
      foreach (HashSet<T> shard in this.m_shards)
        shard.Clear();
    }

    public bool Contains(T item) => this.GetBucket(item).Contains(item);

    public bool Remove(T item) => this.GetBucket(item).Remove(item);

    public void UnionWith(IEnumerable<T> other) => other.ForEach<T>((Action<T>) (item => this.Add(item)));

    private HashSet<T> GetBucket(T item) => this.m_shards[(int) ((long) (uint) item.GetHashCode() % (long) this.m_shards.Length)];

    private IEnumerable<T> BuildEnumerable()
    {
      for (int i = 0; i < this.m_shards.Length; ++i)
      {
        foreach (T obj in this.m_shards[i])
          yield return obj;
      }
    }

    public void IntersectWith(IEnumerable<T> other) => throw new NotImplementedException();

    public void ExceptWith(IEnumerable<T> other) => throw new NotImplementedException();

    public void SymmetricExceptWith(IEnumerable<T> other) => throw new NotImplementedException();

    public bool IsSubsetOf(IEnumerable<T> other) => throw new NotImplementedException();

    public bool IsSupersetOf(IEnumerable<T> other) => throw new NotImplementedException();

    public bool IsProperSupersetOf(IEnumerable<T> other) => throw new NotImplementedException();

    public bool IsProperSubsetOf(IEnumerable<T> other) => throw new NotImplementedException();

    public bool Overlaps(IEnumerable<T> other) => throw new NotImplementedException();

    public bool SetEquals(IEnumerable<T> other) => throw new NotImplementedException();

    public void CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
  }
}
