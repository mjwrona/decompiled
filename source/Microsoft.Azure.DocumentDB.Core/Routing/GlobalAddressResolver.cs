// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.GlobalAddressResolver
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Routing
{
  internal sealed class GlobalAddressResolver : 
    IAddressResolverExtension,
    IAddressResolver,
    IDisposable
  {
    private const int MaxBackupReadRegions = 3;
    private readonly GlobalEndpointManager endpointManager;
    private readonly Protocol protocol;
    private readonly IAuthorizationTokenProvider tokenProvider;
    private readonly UserAgentContainer userAgentContainer;
    private readonly CollectionCache collectionCache;
    private readonly PartitionKeyRangeCache routingMapProvider;
    private readonly int maxEndpoints;
    private readonly IServiceConfigurationReader serviceConfigReader;
    private readonly HttpMessageHandler messageHandler;
    private readonly ConcurrentDictionary<Uri, GlobalAddressResolver.EndpointCache> addressCacheByEndpoint;
    private readonly TimeSpan requestTimeout;
    private readonly ApiType apiType;

    public GlobalAddressResolver(
      GlobalEndpointManager endpointManager,
      Protocol protocol,
      IAuthorizationTokenProvider tokenProvider,
      CollectionCache collectionCache,
      PartitionKeyRangeCache routingMapProvider,
      UserAgentContainer userAgentContainer,
      IServiceConfigurationReader serviceConfigReader,
      HttpMessageHandler messageHandler,
      ConnectionPolicy connectionPolicy,
      ApiType apiType)
    {
      this.endpointManager = endpointManager;
      this.protocol = protocol;
      this.tokenProvider = tokenProvider;
      this.userAgentContainer = userAgentContainer;
      this.collectionCache = collectionCache;
      this.routingMapProvider = routingMapProvider;
      this.serviceConfigReader = serviceConfigReader;
      this.messageHandler = messageHandler;
      this.requestTimeout = connectionPolicy.RequestTimeout;
      this.apiType = apiType;
      this.maxEndpoints = (!connectionPolicy.EnableReadRequestsFallback.HasValue || connectionPolicy.EnableReadRequestsFallback.Value ? 3 : 0) + 2;
      this.addressCacheByEndpoint = new ConcurrentDictionary<Uri, GlobalAddressResolver.EndpointCache>();
      foreach (Uri writeEndpoint in endpointManager.WriteEndpoints)
        this.GetOrAddEndpoint(writeEndpoint);
      foreach (Uri readEndpoint in endpointManager.ReadEndpoints)
        this.GetOrAddEndpoint(readEndpoint);
    }

    public async Task OpenAsync(
      string databaseName,
      DocumentCollection collection,
      CancellationToken cancellationToken)
    {
      CollectionRoutingMap collectionRoutingMap = await this.routingMapProvider.TryLookupAsync(collection.ResourceId, (CollectionRoutingMap) null, (DocumentServiceRequest) null, cancellationToken);
      if (collectionRoutingMap == null)
        return;
      List<PartitionKeyRangeIdentity> list = collectionRoutingMap.OrderedPartitionKeyRanges.Select<PartitionKeyRange, PartitionKeyRangeIdentity>((Func<PartitionKeyRange, PartitionKeyRangeIdentity>) (range => new PartitionKeyRangeIdentity(collection.ResourceId, range.Id))).ToList<PartitionKeyRangeIdentity>();
      List<Task> taskList = new List<Task>();
      foreach (GlobalAddressResolver.EndpointCache endpointCache in (IEnumerable<GlobalAddressResolver.EndpointCache>) this.addressCacheByEndpoint.Values)
        taskList.Add(endpointCache.AddressCache.OpenAsync(databaseName, collection, (IReadOnlyList<PartitionKeyRangeIdentity>) list, cancellationToken));
      await Task.WhenAll((IEnumerable<Task>) taskList);
    }

    public Task<PartitionAddressInformation> ResolveAsync(
      DocumentServiceRequest request,
      bool forceRefresh,
      CancellationToken cancellationToken)
    {
      return this.GetAddressResolver(request).ResolveAsync(request, forceRefresh, cancellationToken);
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

    private IAddressResolver GetAddressResolver(DocumentServiceRequest request) => (IAddressResolver) this.GetOrAddEndpoint(this.endpointManager.ResolveServiceEndpoint(request)).AddressResolver;

    public void Dispose()
    {
      foreach (GlobalAddressResolver.EndpointCache endpointCache in (IEnumerable<GlobalAddressResolver.EndpointCache>) this.addressCacheByEndpoint.Values)
        endpointCache.AddressCache.Dispose();
    }

    private GlobalAddressResolver.EndpointCache GetOrAddEndpoint(Uri endpoint)
    {
      GlobalAddressResolver.EndpointCache orAdd = this.addressCacheByEndpoint.GetOrAdd(endpoint, (Func<Uri, GlobalAddressResolver.EndpointCache>) (resolvedEndpoint =>
      {
        GatewayAddressCache gatewayAddressCache = new GatewayAddressCache(resolvedEndpoint, this.protocol, this.tokenProvider, this.userAgentContainer, this.serviceConfigReader, this.requestTimeout, messageHandler: this.messageHandler, apiType: this.apiType);
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
