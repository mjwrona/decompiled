// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Utils.CosmosContainerExtensions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Utils
{
  internal static class CosmosContainerExtensions
  {
    private static readonly ItemRequestOptions itemRequestOptionsWithResponseEnabled = new ItemRequestOptions()
    {
      EnableContentResponseOnWrite = new bool?(false)
    };
    public static readonly CosmosSerializerCore DefaultJsonSerializer = new CosmosSerializerCore();

    public static async Task<T> TryGetItemAsync<T>(
      this Container container,
      PartitionKey partitionKey,
      string itemId)
    {
      T itemAsync;
      using (ResponseMessage responseMessage = await container.ReadItemStreamAsync(itemId, partitionKey).ConfigureAwait(false))
      {
        responseMessage.EnsureSuccessStatusCode();
        itemAsync = CosmosContainerExtensions.DefaultJsonSerializer.FromStream<T>(responseMessage.Content);
      }
      return itemAsync;
    }

    public static async Task<ItemResponse<T>> TryCreateItemAsync<T>(
      this Container container,
      PartitionKey partitionKey,
      T item)
    {
      using (Stream itemStream = CosmosContainerExtensions.DefaultJsonSerializer.ToStream<T>(item))
      {
        using (ResponseMessage responseMessage = await container.CreateItemStreamAsync(itemStream, partitionKey, CosmosContainerExtensions.itemRequestOptionsWithResponseEnabled).ConfigureAwait(false))
        {
          if (responseMessage.StatusCode == HttpStatusCode.Conflict)
            return (ItemResponse<T>) null;
          responseMessage.EnsureSuccessStatusCode();
          return new ItemResponse<T>(responseMessage.StatusCode, responseMessage.Headers, item, responseMessage.Diagnostics, responseMessage.RequestMessage);
        }
      }
    }

    public static async Task<ItemResponse<T>> TryReplaceItemAsync<T>(
      this Container container,
      string itemId,
      T item,
      PartitionKey partitionKey,
      ItemRequestOptions itemRequestOptions)
    {
      ItemResponse<T> itemResponse;
      using (Stream itemStream = CosmosContainerExtensions.DefaultJsonSerializer.ToStream<T>(item))
      {
        itemRequestOptions.EnableContentResponseOnWrite = new bool?(false);
        using (ResponseMessage responseMessage = await container.ReplaceItemStreamAsync(itemStream, itemId, partitionKey, itemRequestOptions).ConfigureAwait(false))
        {
          responseMessage.EnsureSuccessStatusCode();
          itemResponse = new ItemResponse<T>(responseMessage.StatusCode, responseMessage.Headers, item, responseMessage.Diagnostics, responseMessage.RequestMessage);
        }
      }
      return itemResponse;
    }

    public static async Task<bool> TryDeleteItemAsync<T>(
      this Container container,
      PartitionKey partitionKey,
      string itemId,
      ItemRequestOptions cosmosItemRequestOptions = null)
    {
      if (cosmosItemRequestOptions == null)
        cosmosItemRequestOptions = new ItemRequestOptions();
      cosmosItemRequestOptions.EnableContentResponseOnWrite = new bool?(false);
      bool successStatusCode;
      using (ResponseMessage responseMessage = await container.DeleteItemStreamAsync(itemId, partitionKey, cosmosItemRequestOptions).ConfigureAwait(false))
        successStatusCode = responseMessage.IsSuccessStatusCode;
      return successStatusCode;
    }

    public static async Task<bool> ItemExistsAsync(
      this Container container,
      PartitionKey partitionKey,
      string itemId)
    {
      return (await container.ReadItemStreamAsync(itemId, partitionKey).ConfigureAwait(false)).IsSuccessStatusCode;
    }

    public static async Task<string> GetMonitoredDatabaseAndContainerRidAsync(
      this Container monitoredContainer,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      string containerRid = await ((ContainerInternal) monitoredContainer).GetCachedRIDAsync(false, (ITrace) NoOpTrace.Singleton, cancellationToken);
      string containerRidAsync = await ((DatabaseInternal) monitoredContainer.Database).GetRIDAsync(cancellationToken) + "_" + containerRid;
      containerRid = (string) null;
      return containerRidAsync;
    }

    public static string GetLeasePrefix(
      this Container monitoredContainer,
      string leasePrefix,
      string monitoredDatabaseAndContainerRid)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}_{2}", (object) (leasePrefix ?? string.Empty), (object) ((ContainerInternal) monitoredContainer).ClientContext.Client.Endpoint.Host, (object) monitoredDatabaseAndContainerRid);
    }
  }
}
