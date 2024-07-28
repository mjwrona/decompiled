// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ItemStore.TracingItemStore
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ItemStore
{
  public class TracingItemStore : IItemStore, IVssFrameworkService
  {
    private readonly IItemStore backingItemStore;
    private static readonly TraceData TraceData = Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.TracingItemStore.TraceData;

    public TracingItemStore(IItemStore backingItemStore) => this.backingItemStore = backingItemStore;

    public bool IsReadOnly => this.backingItemStore.IsReadOnly;

    public async Task<AssociationsStatus> AssociateItemsAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      AssociationsItem associations)
    {
      AssociationsStatus associationsStatus;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingItemStore.TraceData, 5726100, nameof (AssociateItemsAsync)))
        associationsStatus = await this.backingItemStore.AssociateItemsAsync(requestContext, domainId, associations).ConfigureAwait(true);
      return associationsStatus;
    }

    public async Task<bool> CompareSwapItemAsync(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator path,
      StoredItem item)
    {
      bool flag;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingItemStore.TraceData, 5726110, nameof (CompareSwapItemAsync)))
        flag = await this.backingItemStore.CompareSwapItemAsync(requestContext, containerName, path, item).ConfigureAwait(true);
      return flag;
    }

    public async Task<IDictionary<Locator, bool>> CompareSwapItemsAsync(
      IVssRequestContext requestContext,
      Locator containerName,
      IReadOnlyDictionary<Locator, StoredItem> items,
      bool atomicDirectory = false)
    {
      IDictionary<Locator, bool> dictionary;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingItemStore.TraceData, 5726120, nameof (CompareSwapItemsAsync)))
        dictionary = await this.backingItemStore.CompareSwapItemsAsync(requestContext, containerName, items, atomicDirectory).ConfigureAwait(true);
      return dictionary;
    }

    public async Task<bool> DeleteContainerAsync(
      IVssRequestContext requestContext,
      ContainerItem container)
    {
      bool flag;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingItemStore.TraceData, 5726130, nameof (DeleteContainerAsync)))
        flag = await this.backingItemStore.DeleteContainerAsync(requestContext, container).ConfigureAwait(true);
      return flag;
    }

    public async Task<bool> DeleteItemAsync(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator path,
      StoredItem expectedItem)
    {
      bool flag;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingItemStore.TraceData, 5726140, nameof (DeleteItemAsync)))
        flag = await this.backingItemStore.DeleteItemAsync(requestContext, containerName, path, expectedItem).ConfigureAwait(true);
      return flag;
    }

    public async Task<ContainerItem> GetContainerAsync(
      IVssRequestContext requestContext,
      Locator name)
    {
      ContainerItem containerAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingItemStore.TraceData, 5726150, nameof (GetContainerAsync)))
        containerAsync = await this.backingItemStore.GetContainerAsync(requestContext, name).ConfigureAwait(true);
      return containerAsync;
    }

    public async Task<IConcurrentIterator<KeyValuePair<Locator, ContainerItem>>> GetContainersConcurrentIteratorAsync(
      IVssRequestContext requestContext,
      Locator pathPrefix,
      PathOptions pathOptions)
    {
      IConcurrentIterator<KeyValuePair<Locator, ContainerItem>> concurrentIteratorAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingItemStore.TraceData, 5726160, nameof (GetContainersConcurrentIteratorAsync)))
        concurrentIteratorAsync = await this.backingItemStore.GetContainersConcurrentIteratorAsync(requestContext, pathPrefix, pathOptions).ConfigureAwait(true);
      return concurrentIteratorAsync;
    }

    public async Task<T> GetItemAsync<T>(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator path,
      LatencyPreference latencyPreference = LatencyPreference.PreferHighThroughput)
      where T : StoredItem
    {
      T itemAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingItemStore.TraceData, 5726170, nameof (GetItemAsync)))
        itemAsync = await this.backingItemStore.GetItemAsync<T>(requestContext, containerName, path, latencyPreference).ConfigureAwait(true);
      return itemAsync;
    }

    async Task<IConcurrentIterator<KeyValuePair<Locator, T>>> IItemStore.GetItemsConcurrentIteratorAsync<T>(
      IVssRequestContext requestContext,
      Locator container,
      IReadOnlyCollection<Locator> itemLocators)
    {
      IConcurrentIterator<KeyValuePair<Locator, T>> concurrentIteratorAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingItemStore.TraceData, 5726180, "GetItemsConcurrentIteratorAsync"))
        concurrentIteratorAsync = await this.backingItemStore.GetItemsConcurrentIteratorAsync<T>(requestContext, container, itemLocators).ConfigureAwait(true);
      return concurrentIteratorAsync;
    }

    public async Task<IConcurrentIterator<KeyValuePair<Locator, T>>> GetItemsConcurrentIteratorAsync<T>(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator pathPrefix,
      PathOptions pathOptions)
      where T : StoredItem
    {
      IConcurrentIterator<KeyValuePair<Locator, T>> concurrentIteratorAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingItemStore.TraceData, 5726180, nameof (GetItemsConcurrentIteratorAsync)))
        concurrentIteratorAsync = await this.backingItemStore.GetItemsConcurrentIteratorAsync<T>(requestContext, containerName, pathPrefix, pathOptions).ConfigureAwait(true);
      return concurrentIteratorAsync;
    }

    public async Task<IConcurrentIterator<IEnumerable<KeyValuePair<Locator, T>>>> GetItemPagesConcurrentIteratorAsync<T>(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator pathPrefix,
      PathOptions pathOptions)
      where T : StoredItem
    {
      IConcurrentIterator<IEnumerable<KeyValuePair<Locator, T>>> concurrentIteratorAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingItemStore.TraceData, 5726180, nameof (GetItemPagesConcurrentIteratorAsync)))
        concurrentIteratorAsync = await this.backingItemStore.GetItemPagesConcurrentIteratorAsync<T>(requestContext, containerName, pathPrefix, pathOptions).ConfigureAwait(true);
      return concurrentIteratorAsync;
    }

    public async Task<ContainerItem> GetOrAddContainerAsync(
      IVssRequestContext requestContext,
      ContainerItem container)
    {
      ContainerItem addContainerAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingItemStore.TraceData, 5726190, nameof (GetOrAddContainerAsync)))
        addContainerAsync = await this.backingItemStore.GetOrAddContainerAsync(requestContext, container).ConfigureAwait(true);
      return addContainerAsync;
    }

    public async Task<IDictionary<MoveOperation, bool>> MoveItemsAsync<T>(
      IVssRequestContext requestContext,
      Locator container,
      MoveItemOperations moveOperations,
      MoveItemOptions options)
      where T : StoredItem
    {
      IDictionary<MoveOperation, bool> dictionary;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingItemStore.TraceData, 5726200, nameof (MoveItemsAsync)))
        dictionary = await this.backingItemStore.MoveItemsAsync<T>(requestContext, container, moveOperations, options).ConfigureAwait(true);
      return dictionary;
    }

    public async Task<bool> TryAddBlobItemAsync<T>(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator path,
      T itemToAdd,
      Func<Task<Stream>> blobStreamFactory,
      IDomainId domainId,
      bool repairManifest = false)
      where T : BlobItem
    {
      bool flag;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingItemStore.TraceData, 5726210, nameof (TryAddBlobItemAsync)))
        flag = await this.backingItemStore.TryAddBlobItemAsync<T>(requestContext, containerName, path, itemToAdd, blobStreamFactory, WellKnownDomainIds.DefaultDomainId, repairManifest).ConfigureAwait(true);
      return flag;
    }

    public async Task UpdateContainerAsync(
      IVssRequestContext requestContext,
      Locator name,
      ContainerItem container)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingItemStore.TraceData, 5726220, nameof (UpdateContainerAsync)))
        await this.backingItemStore.UpdateContainerAsync(requestContext, name, container).ConfigureAwait(true);
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => throw new InvalidTracingWrapperUsageException();

    public void ServiceEnd(IVssRequestContext systemRequestContext) => throw new InvalidTracingWrapperUsageException();
  }
}
