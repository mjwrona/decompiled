// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Server.Common.IItemStore
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8190F04D-5888-4DB5-A838-8C98A67C6E45
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ItemStore.Server.Common
{
  public interface IItemStore : IVssFrameworkService
  {
    bool IsReadOnly { get; }

    Task<AssociationsStatus> AssociateItemsAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      AssociationsItem associations);

    Task<bool> CompareSwapItemAsync(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator path,
      StoredItem item);

    Task<IDictionary<Locator, bool>> CompareSwapItemsAsync(
      IVssRequestContext requestContext,
      Locator containerName,
      IReadOnlyDictionary<Locator, StoredItem> items,
      bool atomicDirectory = false);

    Task<bool> DeleteContainerAsync(IVssRequestContext requestContext, ContainerItem container);

    Task<bool> DeleteItemAsync(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator path,
      StoredItem expectedItem);

    Task<ContainerItem> GetContainerAsync(IVssRequestContext requestContext, Locator name);

    Task<IConcurrentIterator<KeyValuePair<Locator, ContainerItem>>> GetContainersConcurrentIteratorAsync(
      IVssRequestContext requestContext,
      Locator pathPrefix,
      PathOptions pathOptions);

    Task<T> GetItemAsync<T>(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator path,
      LatencyPreference latencyPreference = LatencyPreference.PreferHighThroughput)
      where T : StoredItem;

    Task<IConcurrentIterator<KeyValuePair<Locator, T>>> GetItemsConcurrentIteratorAsync<T>(
      IVssRequestContext requestContext,
      Locator container,
      IReadOnlyCollection<Locator> itemLocators)
      where T : StoredItem;

    Task<IConcurrentIterator<KeyValuePair<Locator, T>>> GetItemsConcurrentIteratorAsync<T>(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator pathPrefix,
      PathOptions pathOptions)
      where T : StoredItem;

    Task<IConcurrentIterator<IEnumerable<KeyValuePair<Locator, T>>>> GetItemPagesConcurrentIteratorAsync<T>(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator pathPrefix,
      PathOptions pathOptions)
      where T : StoredItem;

    Task<ContainerItem> GetOrAddContainerAsync(
      IVssRequestContext requestContext,
      ContainerItem container);

    Task<IDictionary<MoveOperation, bool>> MoveItemsAsync<T>(
      IVssRequestContext requestContext,
      Locator container,
      MoveItemOperations moveOperations,
      MoveItemOptions options)
      where T : StoredItem;

    Task<bool> TryAddBlobItemAsync<T>(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator path,
      T itemToAdd,
      Func<Task<Stream>> blobStreamFactory,
      IDomainId domainId,
      bool repairManifest = false)
      where T : BlobItem;

    Task UpdateContainerAsync(
      IVssRequestContext requestContext,
      Locator name,
      ContainerItem container);
  }
}
