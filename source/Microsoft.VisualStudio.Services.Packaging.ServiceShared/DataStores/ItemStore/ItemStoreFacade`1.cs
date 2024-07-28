// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataStores.ItemStore.ItemStoreFacade`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataStores.ItemStore
{
  public class ItemStoreFacade<TService> : IContentItemstore where TService : class, IItemStore
  {
    private readonly IVssRequestContext requestContext;
    private readonly ITracerService tracerService;
    private readonly TService itemStoreService;

    public ItemStoreFacade(IVssRequestContext requestContext, ITracerService tracerService)
    {
      this.requestContext = requestContext;
      this.tracerService = tracerService;
      this.itemStoreService = requestContext.GetService<TService>();
    }

    public async Task<bool> CompareSwapItemAsync(
      Locator containerName,
      Locator path,
      StoredItem item)
    {
      ItemStoreFacade<TService> sendInTheThisObject = this;
      bool flag;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (CompareSwapItemAsync)))
        flag = await sendInTheThisObject.itemStoreService.CompareSwapItemAsync(sendInTheThisObject.requestContext, containerName, path, item);
      return flag;
    }

    public async Task<IDictionary<Locator, bool>> CompareSwapItemsAsync(
      Locator containerName,
      IReadOnlyDictionary<Locator, StoredItem> items,
      bool atomicDirectory = false)
    {
      ItemStoreFacade<TService> sendInTheThisObject = this;
      IDictionary<Locator, bool> dictionary;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (CompareSwapItemsAsync)))
        dictionary = await sendInTheThisObject.itemStoreService.CompareSwapItemsAsync(sendInTheThisObject.requestContext, containerName, items, atomicDirectory);
      return dictionary;
    }

    public async Task<T> GetItemAsync<T>(
      Locator containerName,
      Locator path,
      LatencyPreference latencyPreference = LatencyPreference.PreferHighThroughput)
      where T : StoredItem
    {
      ItemStoreFacade<TService> sendInTheThisObject = this;
      T itemAsync;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetItemAsync)))
        itemAsync = await sendInTheThisObject.itemStoreService.GetItemAsync<T>(sendInTheThisObject.requestContext, containerName, path, latencyPreference);
      return itemAsync;
    }

    public async Task<IConcurrentIterator<KeyValuePair<Locator, T>>> GetItemsConcurrentIteratorAsync<T>(
      Locator containerName,
      Locator pathPrefix,
      PathOptions pathOptions)
      where T : StoredItem
    {
      ItemStoreFacade<TService> sendInTheThisObject = this;
      IConcurrentIterator<KeyValuePair<Locator, T>> concurrentIteratorAsync;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetItemsConcurrentIteratorAsync)))
        concurrentIteratorAsync = await sendInTheThisObject.itemStoreService.GetItemsConcurrentIteratorAsync<T>(sendInTheThisObject.requestContext, containerName, pathPrefix, pathOptions);
      return concurrentIteratorAsync;
    }

    public async Task<IConcurrentIterator<KeyValuePair<Locator, T>>> GetItemsConcurrentIteratorAsync<T>(
      Locator container,
      IReadOnlyCollection<Locator> itemLocators)
      where T : StoredItem
    {
      ItemStoreFacade<TService> sendInTheThisObject = this;
      IConcurrentIterator<KeyValuePair<Locator, T>> concurrentIteratorAsync;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetItemsConcurrentIteratorAsync)))
        concurrentIteratorAsync = await sendInTheThisObject.itemStoreService.GetItemsConcurrentIteratorAsync<T>(sendInTheThisObject.requestContext, container, itemLocators);
      return concurrentIteratorAsync;
    }

    public async Task<IConcurrentIterator<KeyValuePair<Locator, ContainerItem>>> GetContainersConcurrentIteratorAsync(
      Locator pathPrefix,
      PathOptions pathOptions)
    {
      ItemStoreFacade<TService> sendInTheThisObject = this;
      IConcurrentIterator<KeyValuePair<Locator, ContainerItem>> concurrentIteratorAsync;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetContainersConcurrentIteratorAsync)))
        concurrentIteratorAsync = await sendInTheThisObject.itemStoreService.GetContainersConcurrentIteratorAsync(sendInTheThisObject.requestContext, pathPrefix, pathOptions);
      return concurrentIteratorAsync;
    }

    public async Task<ContainerItem> GetOrAddContainerAsync(ContainerItem container)
    {
      ItemStoreFacade<TService> sendInTheThisObject = this;
      ContainerItem addContainerAsync;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetOrAddContainerAsync)))
        addContainerAsync = await sendInTheThisObject.itemStoreService.GetOrAddContainerAsync(sendInTheThisObject.requestContext, container);
      return addContainerAsync;
    }

    public async Task<bool> DeleteContainerAsync(ContainerItem container)
    {
      ItemStoreFacade<TService> sendInTheThisObject = this;
      bool flag;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (DeleteContainerAsync)))
        flag = await sendInTheThisObject.itemStoreService.DeleteContainerAsync(sendInTheThisObject.requestContext, container);
      return flag;
    }
  }
}
