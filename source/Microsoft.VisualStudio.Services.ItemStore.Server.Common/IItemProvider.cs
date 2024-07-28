// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Server.Common.IItemProvider
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8190F04D-5888-4DB5-A838-8C98A67C6E45
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Server.Common.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ItemStore.Server.Common
{
  public interface IItemProvider : IDisposable
  {
    bool IsReadOnly { get; }

    bool RequiresVssRequestContext { get; }

    Task<bool> CompareSwapItemAsync(
      VssRequestPump.Processor processor,
      ShardableLocator path,
      StoredItem item);

    IConcurrentIterator<KeyValuePair<ShardableLocator, bool>> CompareSwapItemsConcurrentIterator<T>(
      VssRequestPump.Processor processor,
      IReadOnlyDictionary<ShardableLocator, T> items,
      bool atomicDirectory = false)
      where T : StoredItem;

    Task<T> GetItemAsync<T>(
      VssRequestPump.Processor processor,
      ShardableLocator path,
      LatencyPreference latencyPreference = LatencyPreference.PreferHighThroughput)
      where T : StoredItem;

    IConcurrentIterator<KeyValuePair<ShardableLocator, T>> GetItemsConcurrentIterator<T>(
      VssRequestPump.Processor processor,
      IEnumerable<ShardableLocator> paths)
      where T : StoredItem;

    IConcurrentIterator<KeyValuePair<ShardableLocator, T>> GetItemsConcurrentIterator<T>(
      VssRequestPump.Processor processor,
      ShardableLocator prefix,
      PathOptions options)
      where T : StoredItem;

    IConcurrentIterator<KeyValuePair<ShardableLocator, T>> GetResumableItemsConcurrentIterator<T>(
      VssRequestPump.Processor processor,
      ShardableLocator prefix,
      ShardableLocator resumePath,
      PathOptions options,
      IteratorPartition partition,
      FilterOptions filterOptions = null,
      ShardableLocatorRange locatorRange = null)
      where T : StoredItem;

    IConcurrentIterator<IEnumerable<KeyValuePair<ShardableLocator, T>>> GetItemPagesConcurrentIterator<T>(
      VssRequestPump.Processor processor,
      ShardableLocator prefix,
      PathOptions options)
      where T : StoredItem;

    Task<bool> RemoveItemAsync(
      VssRequestPump.Processor processor,
      ShardableLocator path,
      string etag = null);

    IConcurrentIterator<KeyValuePair<ShardableLocator, bool>> RemoveItemsConcurrentIterator(
      VssRequestPump.Processor processor,
      IReadOnlyDictionary<ShardableLocator, string> locatorsAndETags);

    IConcurrentIterator<ShardableLocator> GetPathsConcurrentIterator(
      VssRequestPump.Processor processor,
      ShardableLocator prefix,
      PathOptions options);
  }
}
