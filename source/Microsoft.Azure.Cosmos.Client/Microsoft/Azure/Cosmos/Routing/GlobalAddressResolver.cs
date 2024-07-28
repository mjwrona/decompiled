// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.GlobalAddressResolver
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Rntbd;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal sealed class GlobalAddressResolver : IAddressResolver, IDisposable
  {
    private const int MaxBackupReadRegions = 3;
    private readonly GlobalEndpointManager endpointManager;
    private readonly GlobalPartitionEndpointManager partitionKeyRangeLocationCache;
    private readonly Protocol protocol;
    private readonly ICosmosAuthorizationTokenProvider tokenProvider;
    private readonly CollectionCache collectionCache;
    private readonly PartitionKeyRangeCache routingMapProvider;
    private readonly int maxEndpoints;
    private readonly IServiceConfigurationReader serviceConfigReader;
    private readonly CosmosHttpClient httpClient;
    private readonly ConcurrentDictionary<Uri, GlobalAddressResolver.EndpointCache> addressCacheByEndpoint;
    private readonly bool enableTcpConnectionEndpointRediscovery;

    public GlobalAddressResolver(
      GlobalEndpointManager endpointManager,
      GlobalPartitionEndpointManager partitionKeyRangeLocationCache,
      Protocol protocol,
      ICosmosAuthorizationTokenProvider tokenProvider,
      CollectionCache collectionCache,
      PartitionKeyRangeCache routingMapProvider,
      IServiceConfigurationReader serviceConfigReader,
      ConnectionPolicy connectionPolicy,
      CosmosHttpClient httpClient)
    {
      this.endpointManager = endpointManager;
      this.partitionKeyRangeLocationCache = partitionKeyRangeLocationCache;
      this.protocol = protocol;
      this.tokenProvider = tokenProvider;
      this.collectionCache = collectionCache;
      this.routingMapProvider = routingMapProvider;
      this.serviceConfigReader = serviceConfigReader;
      this.httpClient = httpClient;
      int num = !connectionPolicy.EnableReadRequestsFallback.HasValue || connectionPolicy.EnableReadRequestsFallback.Value ? 3 : 0;
      this.enableTcpConnectionEndpointRediscovery = connectionPolicy.EnableTcpConnectionEndpointRediscovery;
      this.maxEndpoints = num + 2;
      this.addressCacheByEndpoint = new ConcurrentDictionary<Uri, GlobalAddressResolver.EndpointCache>();
      foreach (Uri writeEndpoint in endpointManager.WriteEndpoints)
        this.GetOrAddEndpoint(writeEndpoint);
      foreach (Uri readEndpoint in endpointManager.ReadEndpoints)
        this.GetOrAddEndpoint(readEndpoint);
    }

    public async Task OpenAsync(
      string databaseName,
      ContainerProperties collection,
      CancellationToken cancellationToken)
    {
      CollectionRoutingMap collectionRoutingMap = await this.routingMapProvider.TryLookupAsync(collection.ResourceId, (CollectionRoutingMap) null, (DocumentServiceRequest) null, (ITrace) NoOpTrace.Singleton);
      if (collectionRoutingMap == null)
        ;
      else
      {
        List<PartitionKeyRangeIdentity> list = collectionRoutingMap.OrderedPartitionKeyRanges.Select<PartitionKeyRange, PartitionKeyRangeIdentity>((Func<PartitionKeyRange, PartitionKeyRangeIdentity>) (range => new PartitionKeyRangeIdentity(collection.ResourceId, range.Id))).ToList<PartitionKeyRangeIdentity>();
        List<Task> taskList = new List<Task>();
        foreach (GlobalAddressResolver.EndpointCache endpointCache in (IEnumerable<GlobalAddressResolver.EndpointCache>) this.addressCacheByEndpoint.Values)
          taskList.Add(endpointCache.AddressCache.OpenAsync(databaseName, collection, (IReadOnlyList<PartitionKeyRangeIdentity>) list, cancellationToken));
        await Task.WhenAll((IEnumerable<Task>) taskList);
      }
    }

    public async Task<PartitionAddressInformation> ResolveAsync(
      DocumentServiceRequest request,
      bool forceRefresh,
      CancellationToken cancellationToken)
    {
      PartitionAddressInformation addressInformation = await this.GetAddressResolver(request).ResolveAsync(request, forceRefresh, cancellationToken);
      return !this.partitionKeyRangeLocationCache.TryAddPartitionLevelLocationOverride(request) ? addressInformation : await this.GetAddressResolver(request).ResolveAsync(request, forceRefresh, cancellationToken);
    }

    public async Task UpdateAsync(
      IReadOnlyList<AddressCacheToken> addressCacheTokens,
      CancellationToken cancellationToken)
    {
      List<Task> taskList = new List<Task>();
      foreach (AddressCacheToken addressCacheToken in (IEnumerable<AddressCacheToken>) addressCacheTokens)
      {
        GlobalAddressResolver.EndpointCache endpointCache;
        if (this.addressCacheByEndpoint.TryGetValue(addressCacheToken.ServiceEndpoint, out endpointCache))
          taskList.Add((Task) endpointCache.AddressCache.UpdateAsync(addressCacheToken.PartitionKeyRangeIdentity, cancellationToken));
      }
      await Task.WhenAll((IEnumerable<Task>) taskList);
    }

    public Task UpdateAsync(ServerKey serverKey, CancellationToken cancellationToken)
    {
      foreach (KeyValuePair<Uri, GlobalAddressResolver.EndpointCache> keyValuePair in this.addressCacheByEndpoint)
        keyValuePair.Value.AddressCache.TryRemoveAddresses(serverKey);
      return Task.CompletedTask;
    }

    private IAddressResolver GetAddressResolver(DocumentServiceRequest request) => (IAddressResolver) this.GetOrAddEndpoint(this.endpointManager.ResolveServiceEndpoint(request)).AddressResolver;

    public void Dispose()
    {
      foreach (GlobalAddressResolver.EndpointCache endpointCache in (IEnumerable<GlobalAddressResolver.EndpointCache>) this.addressCacheByEndpoint.Values)
        endpointCache.AddressCache.Dispose();
    }

    private GlobalAddressResolver.EndpointCache GetOrAddEndpoint(Uri endpoint)
    {
      GlobalAddressResolver.EndpointCache orAddEndpoint;
      if (this.addressCacheByEndpoint.TryGetValue(endpoint, out orAddEndpoint))
        return orAddEndpoint;
      GlobalAddressResolver.EndpointCache orAdd = this.addressCacheByEndpoint.GetOrAdd(endpoint, (Func<Uri, GlobalAddressResolver.EndpointCache>) (resolvedEndpoint =>
      {
        GatewayAddressCache gatewayAddressCache = new GatewayAddressCache(resolvedEndpoint, this.protocol, this.tokenProvider, this.serviceConfigReader, this.httpClient, enableTcpConnectionEndpointRediscovery: this.enableTcpConnectionEndpointRediscovery);
        AddressResolver addressResolver = new AddressResolver((IMasterServiceIdentityProvider) null, (IRequestSigner) new NullRequestSigner(), this.endpointManager.GetLocation(endpoint));
        addressResolver.InitializeCaches(this.collectionCache, (ICollectionRoutingMapCache) this.routingMapProvider, (IAddressCache) gatewayAddressCache);
        return new GlobalAddressResolver.EndpointCache()
        {
          AddressCache = gatewayAddressCache,
          AddressResolver = addressResolver
        };
      }));
      if (this.addressCacheByEndpoint.Count > this.maxEndpoints)
      {
        Queue<Uri> uriQueue = new Queue<Uri>(this.endpointManager.WriteEndpoints.Union<Uri>((IEnumerable<Uri>) this.endpointManager.ReadEndpoints).Reverse<Uri>());
        while (this.addressCacheByEndpoint.Count > this.maxEndpoints && uriQueue.Count > 0)
          this.addressCacheByEndpoint.TryRemove(uriQueue.Dequeue(), out GlobalAddressResolver.EndpointCache _);
      }
      return orAdd;
    }

    private sealed class EndpointCache
    {
      public GatewayAddressCache AddressCache { get; set; }

      public AddressResolver AddressResolver { get; set; }
    }
  }
}
