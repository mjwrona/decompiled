// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.GatewayAddressCache
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Cosmos.Tracing.TraceData;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Rntbd;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal class GatewayAddressCache : IAddressCache, IDisposable
  {
    private const string protocolFilterFormat = "{0} eq {1}";
    private const string AddressResolutionBatchSize = "AddressResolutionBatchSize";
    private const int DefaultBatchSize = 50;
    private readonly Uri serviceEndpoint;
    private readonly Uri addressEndpoint;
    private readonly AsyncCacheNonBlocking<PartitionKeyRangeIdentity, PartitionAddressInformation> serverPartitionAddressCache;
    private readonly ConcurrentDictionary<PartitionKeyRangeIdentity, DateTime> suboptimalServerPartitionTimestamps;
    private readonly ConcurrentDictionary<ServerKey, HashSet<PartitionKeyRangeIdentity>> serverPartitionAddressToPkRangeIdMap;
    private readonly IServiceConfigurationReader serviceConfigReader;
    private readonly long suboptimalPartitionForceRefreshIntervalInSeconds;
    private readonly Protocol protocol;
    private readonly string protocolFilter;
    private readonly ICosmosAuthorizationTokenProvider tokenProvider;
    private readonly bool enableTcpConnectionEndpointRediscovery;
    private readonly CosmosHttpClient httpClient;
    private Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation> masterPartitionAddressCache;
    private DateTime suboptimalMasterPartitionTimestamp;
    private bool disposedValue;

    public GatewayAddressCache(
      Uri serviceEndpoint,
      Protocol protocol,
      ICosmosAuthorizationTokenProvider tokenProvider,
      IServiceConfigurationReader serviceConfigReader,
      CosmosHttpClient httpClient,
      long suboptimalPartitionForceRefreshIntervalInSeconds = 600,
      bool enableTcpConnectionEndpointRediscovery = false)
    {
      this.addressEndpoint = new Uri(serviceEndpoint?.ToString() + "/addresses");
      this.protocol = protocol;
      this.tokenProvider = tokenProvider;
      this.serviceEndpoint = serviceEndpoint;
      this.serviceConfigReader = serviceConfigReader;
      this.serverPartitionAddressCache = new AsyncCacheNonBlocking<PartitionKeyRangeIdentity, PartitionAddressInformation>();
      this.suboptimalServerPartitionTimestamps = new ConcurrentDictionary<PartitionKeyRangeIdentity, DateTime>();
      this.serverPartitionAddressToPkRangeIdMap = new ConcurrentDictionary<ServerKey, HashSet<PartitionKeyRangeIdentity>>();
      this.suboptimalMasterPartitionTimestamp = DateTime.MaxValue;
      this.enableTcpConnectionEndpointRediscovery = enableTcpConnectionEndpointRediscovery;
      this.suboptimalPartitionForceRefreshIntervalInSeconds = suboptimalPartitionForceRefreshIntervalInSeconds;
      this.httpClient = httpClient;
      this.protocolFilter = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} eq {1}", (object) nameof (protocol), (object) GatewayAddressCache.ProtocolString(this.protocol));
    }

    public Uri ServiceEndpoint => this.serviceEndpoint;

    public async Task OpenAsync(
      string databaseName,
      ContainerProperties collection,
      IReadOnlyList<PartitionKeyRangeIdentity> partitionKeyRangeIdentities,
      CancellationToken cancellationToken)
    {
      List<Task<DocumentServiceResponse>> taskList = new List<Task<DocumentServiceResponse>>();
      int count1 = 50;
      int result;
      if (Assembly.GetEntryAssembly() != (Assembly) null && int.TryParse(ConfigurationManager.AppSettings["AddressResolutionBatchSize"], out result))
        count1 = result;
      using (DocumentServiceRequest fromName = DocumentServiceRequest.CreateFromName(Microsoft.Azure.Documents.OperationType.Read, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}", (object) "dbs", (object) Uri.EscapeUriString(databaseName), (object) "colls", (object) Uri.EscapeUriString(collection.Id)), ResourceType.Collection, AuthorizationTokenType.PrimaryMasterKey))
      {
        for (int count2 = 0; count2 < partitionKeyRangeIdentities.Count; count2 += count1)
          taskList.Add(this.GetServerAddressesViaGatewayAsync(fromName, collection.ResourceId, partitionKeyRangeIdentities.Skip<PartitionKeyRangeIdentity>(count2).Take<PartitionKeyRangeIdentity>(count1).Select<PartitionKeyRangeIdentity, string>((Func<PartitionKeyRangeIdentity, string>) (range => range.PartitionKeyRangeId)), false));
      }
      foreach (DocumentServiceResponse documentServiceResponse in await Task.WhenAll<DocumentServiceResponse>((IEnumerable<Task<DocumentServiceResponse>>) taskList))
      {
        using (documentServiceResponse)
        {
          FeedResource<Address> resource = documentServiceResponse.GetResource<FeedResource<Address>>();
          bool inNetworkRequest = this.IsInNetworkRequest(documentServiceResponse);
          Func<Address, bool> predicate = closure_2 ?? (closure_2 = (Func<Address, bool>) (addressInfo => GatewayAddressCache.ProtocolFromString(addressInfo.Protocol) == this.protocol));
          foreach (Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation> tuple in resource.Where<Address>(predicate).GroupBy<Address, string>((Func<Address, string>) (address => address.PartitionKeyRangeId), (IEqualityComparer<string>) StringComparer.Ordinal).Select<IGrouping<string, Address>, Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation>>((Func<IGrouping<string, Address>, Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation>>) (group => this.ToPartitionAddressAndRange(collection.ResourceId, (IList<Address>) group.ToList<Address>(), inNetworkRequest))))
            this.serverPartitionAddressCache.Set(new PartitionKeyRangeIdentity(collection.ResourceId, tuple.Item1.PartitionKeyRangeId), tuple.Item2);
        }
      }
    }

    public async Task<PartitionAddressInformation> TryGetAddressesAsync(
      DocumentServiceRequest request,
      PartitionKeyRangeIdentity partitionKeyRangeIdentity,
      ServiceIdentity serviceIdentity,
      bool forceRefreshPartitionAddresses,
      CancellationToken cancellationToken)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (partitionKeyRangeIdentity == null)
        throw new ArgumentNullException(nameof (partitionKeyRangeIdentity));
      DateTime utcNow;
      try
      {
        if (partitionKeyRangeIdentity.PartitionKeyRangeId == "M")
          return (await this.ResolveMasterAsync(request, forceRefreshPartitionAddresses)).Item2;
        DateTime comparisonValue;
        if (this.suboptimalServerPartitionTimestamps.TryGetValue(partitionKeyRangeIdentity, out comparisonValue))
        {
          utcNow = DateTime.UtcNow;
          if (utcNow.Subtract(comparisonValue) > TimeSpan.FromSeconds((double) this.suboptimalPartitionForceRefreshIntervalInSeconds) && this.suboptimalServerPartitionTimestamps.TryUpdate(partitionKeyRangeIdentity, DateTime.MaxValue, comparisonValue))
            forceRefreshPartitionAddresses = true;
        }
        PartitionAddressInformation staleAddressInfo = (PartitionAddressInformation) null;
        PartitionAddressInformation async;
        if (forceRefreshPartitionAddresses || request.ForceCollectionRoutingMapRefresh)
        {
          async = await this.serverPartitionAddressCache.GetAsync(partitionKeyRangeIdentity, (Func<PartitionAddressInformation, Task<PartitionAddressInformation>>) (currentCachedValue =>
          {
            staleAddressInfo = currentCachedValue;
            GatewayAddressCache.SetTransportAddressUrisToUnhealthy(currentCachedValue, request?.RequestContext?.FailedEndpoints);
            return this.GetAddressesForRangeIdAsync(request, partitionKeyRangeIdentity.CollectionRid, partitionKeyRangeIdentity.PartitionKeyRangeId, forceRefreshPartitionAddresses);
          }), (Func<PartitionAddressInformation, bool>) (currentCachedValue =>
          {
            int valueOrDefault = request?.RequestContext?.LastPartitionAddressInformationHashCode.GetValueOrDefault();
            return valueOrDefault == 0 || currentCachedValue.GetHashCode() == valueOrDefault;
          }));
          if (staleAddressInfo != null)
            GatewayAddressCache.LogPartitionCacheRefresh(request.RequestContext.ClientRequestStatistics, staleAddressInfo, async);
          this.suboptimalServerPartitionTimestamps.TryRemove(partitionKeyRangeIdentity, out DateTime _);
        }
        else
          async = await this.serverPartitionAddressCache.GetAsync(partitionKeyRangeIdentity, (Func<PartitionAddressInformation, Task<PartitionAddressInformation>>) (_ => this.GetAddressesForRangeIdAsync(request, partitionKeyRangeIdentity.CollectionRid, partitionKeyRangeIdentity.PartitionKeyRangeId, false)), (Func<PartitionAddressInformation, bool>) (_ => false));
        if (request?.RequestContext != null)
          request.RequestContext.LastPartitionAddressInformationHashCode = async.GetHashCode();
        int maxReplicaSetSize = this.serviceConfigReader.UserReplicationPolicy.MaxReplicaSetSize;
        if (async.AllAddresses.Count<AddressInformation>() < maxReplicaSetSize)
          this.suboptimalServerPartitionTimestamps.TryAdd(partitionKeyRangeIdentity, DateTime.UtcNow);
        return async;
      }
      catch (DocumentClientException ex)
      {
        HttpStatusCode? statusCode1 = ex.StatusCode;
        HttpStatusCode httpStatusCode1 = HttpStatusCode.NotFound;
        if (!(statusCode1.GetValueOrDefault() == httpStatusCode1 & statusCode1.HasValue))
        {
          HttpStatusCode? statusCode2 = ex.StatusCode;
          HttpStatusCode httpStatusCode2 = HttpStatusCode.Gone;
          if (!(statusCode2.GetValueOrDefault() == httpStatusCode2 & statusCode2.HasValue) || ex.GetSubStatus() != SubStatusCodes.PartitionKeyRangeGone)
            throw;
        }
        this.suboptimalServerPartitionTimestamps.TryRemove(partitionKeyRangeIdentity, out utcNow);
        return (PartitionAddressInformation) null;
      }
      catch (Exception ex)
      {
        if (forceRefreshPartitionAddresses)
          this.suboptimalServerPartitionTimestamps.TryRemove(partitionKeyRangeIdentity, out utcNow);
        throw;
      }
    }

    private static void SetTransportAddressUrisToUnhealthy(
      PartitionAddressInformation stalePartitionAddressInformation,
      Lazy<HashSet<TransportAddressUri>> failedEndpoints)
    {
      if (stalePartitionAddressInformation == null || failedEndpoints == null || !failedEndpoints.IsValueCreated)
        return;
      IReadOnlyList<TransportAddressUri> transportAddressUris = stalePartitionAddressInformation.Get(Protocol.Tcp)?.ReplicaTransportAddressUris;
      if (transportAddressUris == null)
        return;
      foreach (TransportAddressUri transportAddressUri in (IEnumerable<TransportAddressUri>) transportAddressUris)
      {
        if (failedEndpoints.Value.Contains(transportAddressUri))
          transportAddressUri.SetUnhealthy();
      }
    }

    private static void LogPartitionCacheRefresh(
      IClientSideRequestStatistics clientSideRequestStatistics,
      PartitionAddressInformation old,
      PartitionAddressInformation updated)
    {
      if (!(clientSideRequestStatistics is ClientSideRequestStatisticsTraceDatum statisticsTraceDatum))
        return;
      statisticsTraceDatum.RecordAddressCachRefreshContent(old, updated);
    }

    public void TryRemoveAddresses(ServerKey serverKey)
    {
      if (serverKey == null)
        throw new ArgumentNullException(nameof (serverKey));
      HashSet<PartitionKeyRangeIdentity> source;
      if (!this.serverPartitionAddressToPkRangeIdMap.TryRemove(serverKey, out source))
        return;
      PartitionKeyRangeIdentity[] array;
      lock (source)
        array = source.ToArray<PartitionKeyRangeIdentity>();
      foreach (PartitionKeyRangeIdentity key in array)
      {
        DefaultTrace.TraceInformation("Remove addresses for collectionRid :{0}, pkRangeId: {1}, serviceEndpoint: {2}", (object) key.CollectionRid, (object) key.PartitionKeyRangeId, (object) this.serviceEndpoint);
        this.serverPartitionAddressCache.TryRemove(key);
      }
    }

    public async Task<PartitionAddressInformation> UpdateAsync(
      PartitionKeyRangeIdentity partitionKeyRangeIdentity,
      CancellationToken cancellationToken)
    {
      if (partitionKeyRangeIdentity == null)
        throw new ArgumentNullException(nameof (partitionKeyRangeIdentity));
      cancellationToken.ThrowIfCancellationRequested();
      return await this.serverPartitionAddressCache.GetAsync(partitionKeyRangeIdentity, (Func<PartitionAddressInformation, Task<PartitionAddressInformation>>) (_ => this.GetAddressesForRangeIdAsync((DocumentServiceRequest) null, partitionKeyRangeIdentity.CollectionRid, partitionKeyRangeIdentity.PartitionKeyRangeId, true)), (Func<PartitionAddressInformation, bool>) (_ => true));
    }

    private async Task<Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation>> ResolveMasterAsync(
      DocumentServiceRequest request,
      bool forceRefresh)
    {
      Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation> tuple = this.masterPartitionAddressCache;
      int targetReplicaSetSize = this.serviceConfigReader.SystemReplicationPolicy.MaxReplicaSetSize;
      forceRefresh = forceRefresh || tuple != null && tuple.Item2.AllAddresses.Count<AddressInformation>() < targetReplicaSetSize && DateTime.UtcNow.Subtract(this.suboptimalMasterPartitionTimestamp) > TimeSpan.FromSeconds((double) this.suboptimalPartitionForceRefreshIntervalInSeconds);
      if (forceRefresh || request.ForceCollectionRoutingMapRefresh || this.masterPartitionAddressCache == null)
      {
        string path = PathsHelper.GeneratePath(ResourceType.Database, string.Empty, true);
        try
        {
          using (DocumentServiceResponse addressesViaGatewayAsync = await this.GetMasterAddressesViaGatewayAsync(request, ResourceType.Database, (string) null, path, forceRefresh, false))
          {
            FeedResource<Address> resource = addressesViaGatewayAsync.GetResource<FeedResource<Address>>();
            bool inNetworkRequest = this.IsInNetworkRequest(addressesViaGatewayAsync);
            tuple = this.ToPartitionAddressAndRange(string.Empty, (IList<Address>) resource.ToList<Address>(), inNetworkRequest);
            this.masterPartitionAddressCache = tuple;
            this.suboptimalMasterPartitionTimestamp = DateTime.MaxValue;
          }
        }
        catch (Exception ex)
        {
          this.suboptimalMasterPartitionTimestamp = DateTime.MaxValue;
          throw;
        }
      }
      if (tuple.Item2.AllAddresses.Count<AddressInformation>() < targetReplicaSetSize && this.suboptimalMasterPartitionTimestamp.Equals(DateTime.MaxValue))
        this.suboptimalMasterPartitionTimestamp = DateTime.UtcNow;
      return tuple;
    }

    private async Task<PartitionAddressInformation> GetAddressesForRangeIdAsync(
      DocumentServiceRequest request,
      string collectionRid,
      string partitionKeyRangeId,
      bool forceRefresh)
    {
      GatewayAddressCache gatewayAddressCache = this;
      PartitionAddressInformation addressesForRangeIdAsync;
      using (DocumentServiceResponse addressesViaGatewayAsync = await gatewayAddressCache.GetServerAddressesViaGatewayAsync(request, collectionRid, (IEnumerable<string>) new string[1]
      {
        partitionKeyRangeId
      }, (forceRefresh ? 1 : 0) != 0))
      {
        FeedResource<Address> resource = addressesViaGatewayAsync.GetResource<FeedResource<Address>>();
        bool inNetworkRequest = gatewayAddressCache.IsInNetworkRequest(addressesViaGatewayAsync);
        Func<Address, bool> predicate = (Func<Address, bool>) (addressInfo => GatewayAddressCache.ProtocolFromString(addressInfo.Protocol) == this.protocol);
        Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation> tuple = resource.Where<Address>(predicate).GroupBy<Address, string>((Func<Address, string>) (address => address.PartitionKeyRangeId), (IEqualityComparer<string>) StringComparer.Ordinal).Select<IGrouping<string, Address>, Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation>>((Func<IGrouping<string, Address>, Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation>>) (group => this.ToPartitionAddressAndRange(collectionRid, (IList<Address>) group.ToList<Address>(), inNetworkRequest))).SingleOrDefault<Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation>>((Func<Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation>, bool>) (addressInfo => StringComparer.Ordinal.Equals(addressInfo.Item1.PartitionKeyRangeId, partitionKeyRangeId)));
        if (tuple == null)
        {
          PartitionKeyRangeGoneException rangeGoneException = new PartitionKeyRangeGoneException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.PartitionKeyRangeNotFound, (object) partitionKeyRangeId, (object) collectionRid));
          rangeGoneException.ResourceAddress = collectionRid;
          throw rangeGoneException;
        }
        addressesForRangeIdAsync = tuple.Item2;
      }
      return addressesForRangeIdAsync;
    }

    private async Task<DocumentServiceResponse> GetMasterAddressesViaGatewayAsync(
      DocumentServiceRequest request,
      ResourceType resourceType,
      string resourceAddress,
      string entryUrl,
      bool forceRefresh,
      bool useMasterCollectionResolver)
    {
      INameValueCollection addressQuery = (INameValueCollection) new RequestNameValueCollection()
      {
        {
          "$resolveFor",
          HttpUtility.UrlEncode(entryUrl)
        }
      };
      INameValueCollection headers = (INameValueCollection) new RequestNameValueCollection();
      if (forceRefresh)
        headers.Set("x-ms-force-refresh", bool.TrueString);
      if (useMasterCollectionResolver)
        headers.Set("x-ms-use-master-collection-resolver", bool.TrueString);
      if (request.ForceCollectionRoutingMapRefresh)
        headers.Set("x-ms-collectionroutingmap-refresh", bool.TrueString);
      addressQuery.Add("$filter", this.protocolFilter);
      string resourcePath = PathsHelper.GetResourcePath(resourceType);
      headers.Set("x-ms-date", Rfc1123DateTimeCache.UtcNow());
      DocumentServiceResponse addressesViaGatewayAsync;
      using (ITrace trace = (ITrace) Microsoft.Azure.Cosmos.Tracing.Trace.GetRootTrace(nameof (GetMasterAddressesViaGatewayAsync), TraceComponent.Authorization, TraceLevel.Info))
      {
        headers.Set("authorization", await this.tokenProvider.GetUserAuthorizationTokenAsync(resourceAddress, resourcePath, "GET", headers, AuthorizationTokenType.PrimaryMasterKey, trace));
        Uri uri = UrlUtility.SetQuery(this.addressEndpoint, UrlUtility.CreateQuery(addressQuery));
        string identifier = GatewayAddressCache.LogAddressResolutionStart(request, uri);
        using (HttpResponseMessage httpResponseMessage = await this.httpClient.GetAsync(uri, headers, resourceType, HttpTimeoutPolicyControlPlaneRetriableHotPath.Instance, request.RequestContext?.ClientRequestStatistics, new CancellationToken()))
        {
          DocumentServiceResponse responseAsync = await ClientExtensions.ParseResponseAsync(httpResponseMessage);
          GatewayAddressCache.LogAddressResolutionEnd(request, identifier);
          addressesViaGatewayAsync = responseAsync;
        }
      }
      addressQuery = (INameValueCollection) null;
      headers = (INameValueCollection) null;
      return addressesViaGatewayAsync;
    }

    private async Task<DocumentServiceResponse> GetServerAddressesViaGatewayAsync(
      DocumentServiceRequest request,
      string collectionRid,
      IEnumerable<string> partitionKeyRangeIds,
      bool forceRefresh)
    {
      INameValueCollection addressQuery = (INameValueCollection) new RequestNameValueCollection()
      {
        {
          "$resolveFor",
          HttpUtility.UrlEncode(PathsHelper.GeneratePath(ResourceType.Document, collectionRid, true))
        }
      };
      INameValueCollection headers = (INameValueCollection) new RequestNameValueCollection();
      if (forceRefresh)
        headers.Set("x-ms-force-refresh", bool.TrueString);
      if (request != null && request.ForceCollectionRoutingMapRefresh)
        headers.Set("x-ms-collectionroutingmap-refresh", bool.TrueString);
      addressQuery.Add("$filter", this.protocolFilter);
      addressQuery.Add("$partitionKeyRangeIds", string.Join(",", partitionKeyRangeIds));
      string resourceTypeToSign = PathsHelper.GetResourcePath(ResourceType.Document);
      headers.Set("x-ms-date", Rfc1123DateTimeCache.UtcNow());
      string token = (string) null;
      DocumentServiceResponse addressesViaGatewayAsync;
      using (ITrace trace = (ITrace) Microsoft.Azure.Cosmos.Tracing.Trace.GetRootTrace("GetMasterAddressesViaGatewayAsync", TraceComponent.Authorization, TraceLevel.Info))
      {
        try
        {
          token = await this.tokenProvider.GetUserAuthorizationTokenAsync(collectionRid, resourceTypeToSign, "GET", headers, AuthorizationTokenType.PrimaryMasterKey, trace);
        }
        catch (UnauthorizedException ex)
        {
        }
        if (token == null && request != null && request.IsNameBased)
          token = await this.tokenProvider.GetUserAuthorizationTokenAsync(PathsHelper.GetCollectionPath(request.ResourceAddress), resourceTypeToSign, "GET", headers, AuthorizationTokenType.PrimaryMasterKey, trace);
        headers.Set("authorization", token);
        Uri uri = UrlUtility.SetQuery(this.addressEndpoint, UrlUtility.CreateQuery(addressQuery));
        string identifier = GatewayAddressCache.LogAddressResolutionStart(request, uri);
        using (HttpResponseMessage httpResponseMessage = await this.httpClient.GetAsync(uri, headers, ResourceType.Document, HttpTimeoutPolicyControlPlaneRetriableHotPath.Instance, request.RequestContext?.ClientRequestStatistics, new CancellationToken()))
        {
          DocumentServiceResponse responseAsync = await ClientExtensions.ParseResponseAsync(httpResponseMessage);
          GatewayAddressCache.LogAddressResolutionEnd(request, identifier);
          addressesViaGatewayAsync = responseAsync;
        }
      }
      addressQuery = (INameValueCollection) null;
      headers = (INameValueCollection) null;
      resourceTypeToSign = (string) null;
      token = (string) null;
      return addressesViaGatewayAsync;
    }

    internal Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation> ToPartitionAddressAndRange(
      string collectionRid,
      IList<Address> addresses,
      bool inNetworkRequest)
    {
      Address address = addresses.First<Address>();
      IReadOnlyList<AddressInformation> addressInformation1 = GatewayAddressCache.GetSortedAddressInformation(addresses);
      PartitionKeyRangeIdentity keyRangeIdentity = new PartitionKeyRangeIdentity(collectionRid, address.PartitionKeyRangeId);
      if (this.enableTcpConnectionEndpointRediscovery && keyRangeIdentity.PartitionKeyRangeId != "M")
      {
        foreach (AddressInformation addressInformation2 in (IEnumerable<AddressInformation>) addressInformation1)
        {
          DefaultTrace.TraceInformation("Added address to serverPartitionAddressToPkRangeIdMap, collectionRid :{0}, pkRangeId: {1}, address: {2}", (object) keyRangeIdentity.CollectionRid, (object) keyRangeIdentity.PartitionKeyRangeId, (object) addressInformation2.PhysicalUri);
          HashSet<PartitionKeyRangeIdentity> orAdd = this.serverPartitionAddressToPkRangeIdMap.GetOrAdd(new ServerKey(new Uri(addressInformation2.PhysicalUri)), (Func<ServerKey, HashSet<PartitionKeyRangeIdentity>>) (_ => new HashSet<PartitionKeyRangeIdentity>()));
          lock (orAdd)
            orAdd.Add(keyRangeIdentity);
        }
      }
      return Tuple.Create<PartitionKeyRangeIdentity, PartitionAddressInformation>(keyRangeIdentity, new PartitionAddressInformation(addressInformation1, inNetworkRequest));
    }

    private static IReadOnlyList<AddressInformation> GetSortedAddressInformation(
      IList<Address> addresses)
    {
      AddressInformation[] array = new AddressInformation[addresses.Count];
      for (int index1 = 0; index1 < addresses.Count; ++index1)
      {
        Address address = addresses[index1];
        AddressInformation[] addressInformationArray = array;
        int index2 = index1;
        bool isPrimary = address.IsPrimary;
        AddressInformation addressInformation = new AddressInformation(address.PhysicalUri, true, isPrimary, GatewayAddressCache.ProtocolFromString(address.Protocol));
        addressInformationArray[index2] = addressInformation;
      }
      Array.Sort<AddressInformation>(array);
      return (IReadOnlyList<AddressInformation>) array;
    }

    private bool IsInNetworkRequest(DocumentServiceResponse documentServiceResponse)
    {
      bool result = false;
      string str = documentServiceResponse.ResponseHeaders.Get("x-ms-local-region-request");
      if (!string.IsNullOrEmpty(str))
        bool.TryParse(str, out result);
      return result;
    }

    private static string LogAddressResolutionStart(
      DocumentServiceRequest request,
      Uri targetEndpoint)
    {
      string str = (string) null;
      if (request != null && request.RequestContext.ClientRequestStatistics != null)
        str = request.RequestContext.ClientRequestStatistics.RecordAddressResolutionStart(targetEndpoint);
      return str;
    }

    private static void LogAddressResolutionEnd(DocumentServiceRequest request, string identifier)
    {
      if (request == null || request.RequestContext.ClientRequestStatistics == null)
        return;
      request.RequestContext.ClientRequestStatistics.RecordAddressResolutionEnd(identifier);
    }

    private static Protocol ProtocolFromString(string protocol)
    {
      switch (protocol.ToLowerInvariant())
      {
        case "https":
          return Protocol.Https;
        case "rntbd":
          return Protocol.Tcp;
        default:
          throw new ArgumentOutOfRangeException(nameof (protocol));
      }
    }

    private static string ProtocolString(Protocol protocol)
    {
      switch (protocol)
      {
        case Protocol.Https:
          return "https";
        case Protocol.Tcp:
          return "rntbd";
        default:
          throw new ArgumentOutOfRangeException(nameof (protocol));
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (disposing)
        this.serverPartitionAddressCache?.Dispose();
      this.disposedValue = true;
    }

    public void Dispose() => this.Dispose(true);
  }
}
