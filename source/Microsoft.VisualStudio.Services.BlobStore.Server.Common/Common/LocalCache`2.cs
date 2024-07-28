// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.LocalCache`2
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class LocalCache<K, V>
  {
    private readonly object m_syncLock;
    private readonly IRemoteCache<K, V> m_remote;
    private readonly int m_maxBufferSize;
    private readonly Action<TraceLevel, string> m_tracer;
    private readonly Action<Task> m_postWriteThrough;
    private readonly bool m_diag;
    private int m_writes;
    private ConcurrentBag<Tuple<K, V>> m_bag;

    public int Capacity => this.m_maxBufferSize;

    public LocalCache(
      IRemoteCache<K, V> remote,
      int maxBufferSize,
      Action<Task> postWriteThrough = null,
      bool diag = false,
      Action<TraceLevel, string> tracer = null)
    {
      this.m_syncLock = new object();
      this.m_remote = remote;
      this.m_maxBufferSize = maxBufferSize;
      this.m_postWriteThrough = postWriteThrough;
      this.m_diag = diag;
      this.m_tracer = tracer ?? (Action<TraceLevel, string>) ((l, s) => { });
      this.m_writes = 0;
      this.m_bag = this.CreateNewBag();
    }

    public void Add(VssRequestPump.Processor processor, K key, V value)
    {
      if (this.m_bag.Count >= this.m_maxBufferSize)
      {
        lock (this.m_syncLock)
        {
          this.m_bag.Add(Tuple.Create<K, V>(key, value));
          if (this.m_bag.Count < this.m_maxBufferSize)
            return;
          this.AddInternalAsync(processor);
        }
      }
      else
        this.m_bag.Add(Tuple.Create<K, V>(key, value));
    }

    private Task AddInternalAsync(VssRequestPump.Processor processor)
    {
      ConcurrentBag<Tuple<K, V>> bag = this.m_bag;
      this.m_bag = this.CreateNewBag();
      Task task = Task.Run(this.m_diag ? (Action) (() =>
      {
        DateTime utcNow = DateTime.UtcNow;
        int count = bag.Count;
        this.AddToRemote(processor, bag);
        Interlocked.Add(ref this.m_writes, count);
        double num = Math.Ceiling((DateTime.UtcNow - utcNow).TotalSeconds * 100.0) / 100.0;
        this.m_tracer(TraceLevel.Info, string.Format("Added {0} items from local to remote in {1} seconds.", (object) count, (object) num));
      }) : (Action) (() => this.AddToRemote(processor, bag)));
      if (this.m_postWriteThrough != null)
        task = task.ContinueWith(this.m_postWriteThrough);
      return task;
    }

    private void AddToRemote(VssRequestPump.Processor processor, ConcurrentBag<Tuple<K, V>> bag)
    {
      Dictionary<K, V> kvps = new Dictionary<K, V>();
      foreach (Tuple<K, V> tuple in bag)
        kvps[tuple.Item1] = tuple.Item2;
      this.m_remote.Add(processor, (IDictionary<K, V>) kvps);
    }

    private ConcurrentBag<Tuple<K, V>> CreateNewBag() => new ConcurrentBag<Tuple<K, V>>();

    public string Stats
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("Wrote ");
        stringBuilder.Append(this.m_writes);
        stringBuilder.Append(" entries to remote cache");
        if (this.m_remote is ICountableRemoteCache<K, V> remote)
        {
          stringBuilder.Append(" whose Current size:");
          stringBuilder.Append(remote.Count);
        }
        stringBuilder.Append(".");
        return stringBuilder.ToString();
      }
    }
  }
}
