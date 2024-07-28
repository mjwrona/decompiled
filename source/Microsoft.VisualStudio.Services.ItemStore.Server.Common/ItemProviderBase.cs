// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Server.Common.ItemProviderBase
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8190F04D-5888-4DB5-A838-8C98A67C6E45
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Server.Common.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ItemStore.Server.Common
{
  public abstract class ItemProviderBase : IItemProvider, IDisposable
  {
    public abstract bool IsReadOnly { get; }

    public abstract bool RequiresVssRequestContext { get; }

    public abstract Task<bool> CompareSwapItemAsync(
      VssRequestPump.Processor processor,
      ShardableLocator path,
      StoredItem item);

    public abstract IConcurrentIterator<KeyValuePair<ShardableLocator, bool>> CompareSwapItemsConcurrentIterator<T>(
      VssRequestPump.Processor processor,
      IReadOnlyDictionary<ShardableLocator, T> items,
      bool atomicDirectory = false)
      where T : StoredItem;

    public abstract Task<T> GetItemAsync<T>(
      VssRequestPump.Processor processor,
      ShardableLocator path,
      LatencyPreference latencyPreference = LatencyPreference.PreferHighThroughput)
      where T : StoredItem;

    public abstract IConcurrentIterator<ShardableLocator> GetPathsConcurrentIterator(
      VssRequestPump.Processor processor,
      ShardableLocator prefix,
      PathOptions options);

    public abstract IConcurrentIterator<KeyValuePair<ShardableLocator, T>> GetItemsConcurrentIterator<T>(
      VssRequestPump.Processor processor,
      ShardableLocator prefix,
      PathOptions options)
      where T : StoredItem;

    public abstract IConcurrentIterator<KeyValuePair<ShardableLocator, T>> GetResumableItemsConcurrentIterator<T>(
      VssRequestPump.Processor processor,
      ShardableLocator prefix,
      ShardableLocator resumePath,
      PathOptions options,
      IteratorPartition partition,
      FilterOptions filterOptions,
      ShardableLocatorRange locatorRange)
      where T : StoredItem;

    public abstract IConcurrentIterator<IEnumerable<KeyValuePair<ShardableLocator, T>>> GetItemPagesConcurrentIterator<T>(
      VssRequestPump.Processor processor,
      ShardableLocator prefix,
      PathOptions options)
      where T : StoredItem;

    public abstract Task<bool> RemoveItemAsync(
      VssRequestPump.Processor processor,
      ShardableLocator path,
      string etag = null);

    public virtual IConcurrentIterator<KeyValuePair<ShardableLocator, T>> GetItemsConcurrentIterator<T>(
      VssRequestPump.Processor processor,
      IEnumerable<ShardableLocator> paths)
      where T : StoredItem
    {
      int num;
      return (IConcurrentIterator<KeyValuePair<ShardableLocator, T>>) new ConcurrentIterator<ShardableLocator, KeyValuePair<ShardableLocator, T>>(paths, this.DefaultBoundedCapacity, processor.CancellationToken, (Func<ShardableLocator, TryAddValueAsyncFunc<KeyValuePair<ShardableLocator, T>>, CancellationToken, Task>) (async (path, valueAdderAsync, cancellationToken) => num = await valueAdderAsync(new KeyValuePair<ShardableLocator, T>(path, await this.GetItemAsync<T>(processor, path, LatencyPreference.PreferHighThroughput).ConfigureAwait(false))).ConfigureAwait(false) ? 1 : 0));
    }

    public virtual IConcurrentIterator<KeyValuePair<ShardableLocator, bool>> RemoveItemsConcurrentIterator(
      VssRequestPump.Processor processor,
      IReadOnlyDictionary<ShardableLocator, string> locatorsAndETags)
    {
      return (IConcurrentIterator<KeyValuePair<ShardableLocator, bool>>) new ConcurrentIterator<KeyValuePair<ShardableLocator, string>, KeyValuePair<ShardableLocator, bool>>((IEnumerable<KeyValuePair<ShardableLocator, string>>) locatorsAndETags, this.DefaultBoundedCapacity, processor.CancellationToken, (Func<KeyValuePair<ShardableLocator, string>, TryAddValueAsyncFunc<KeyValuePair<ShardableLocator, bool>>, CancellationToken, Task>) (async (locatorsAndETag, valueAdderAsync, cancellationToken) =>
      {
        ConfiguredTaskAwaitable<bool> configuredTaskAwaitable = this.RemoveItemAsync(processor, locatorsAndETag.Key, locatorsAndETag.Value).ConfigureAwait(false);
        configuredTaskAwaitable = valueAdderAsync(new KeyValuePair<ShardableLocator, bool>(locatorsAndETag.Key, await configuredTaskAwaitable)).ConfigureAwait(false);
        int num = await configuredTaskAwaitable ? 1 : 0;
      }));
    }

    protected abstract int? DefaultBoundedCapacity { get; }

    public void Dispose() => this.Dispose(true);

    protected virtual void Dispose(bool disposing)
    {
    }
  }
}
