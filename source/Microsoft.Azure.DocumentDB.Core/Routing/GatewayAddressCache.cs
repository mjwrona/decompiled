// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.GatewayAddressCache
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Routing
{
  internal class GatewayAddressCache : IAddressCache, IDisposable
  {
    private const string protocolFilterFormat = "{0} eq {1}";
    private const string AddressResolutionBatchSize = "AddressResolutionBatchSize";
    private const int DefaultBatchSize = 50;
    private readonly Uri serviceEndpoint;
    private readonly Uri addressEndpoint;
    private readonly AsyncCache<PartitionKeyRangeIdentity, PartitionAddressInformation> serverPartitionAddressCache;
    private readonly ConcurrentDictionary<PartitionKeyRangeIdentity, DateTime> suboptimalServerPartitionTimestamps;
    private readonly IServiceConfigurationReader serviceConfigReader;
    private readonly long suboptimalPartitionForceRefreshIntervalInSeconds;
    private readonly Protocol protocol;
    private readonly string protocolFilter;
    private readonly IAuthorizationTokenProvider tokenProvider;
    private HttpClient httpClient;
    private Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation> masterPartitionAddressCache;
    private DateTime suboptimalMasterPartitionTimestamp;

    public GatewayAddressCache(
      Uri serviceEndpoint,
      Protocol protocol,
      IAuthorizationTokenProvider tokenProvider,
      UserAgentContainer userAgent,
      IServiceConfigurationReader serviceConfigReader,
      TimeSpan requestTimeout,
      long suboptimalPartitionForceRefreshIntervalInSeconds = 600,
      HttpMessageHandler messageHandler = null,
      ApiType apiType = ApiType.None)
    {
      this.addressEndpoint = new Uri(serviceEndpoint.ToString() + "/addresses");
      this.protocol = protocol;
      this.tokenProvider = tokenProvider;
      this.serviceEndpoint = serviceEndpoint;
      this.serviceConfigReader = serviceConfigReader;
      this.serverPartitionAddressCache = new AsyncCache<PartitionKeyRangeIdentity, PartitionAddressInformation>();
      this.suboptimalServerPartitionTimestamps = new ConcurrentDictionary<PartitionKeyRangeIdentity, DateTime>();
      this.suboptimalMasterPartitionTimestamp = DateTime.MaxValue;
      this.suboptimalPartitionForceRefreshIntervalInSeconds = suboptimalPartitionForceRefreshIntervalInSeconds;
      this.httpClient = messageHandler == null ? new HttpClient() : new HttpClient(messageHandler);
      this.httpClient.Timeout = requestTimeout;
      this.protocolFilter = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} eq {1}", (object) nameof (protocol), (object) GatewayAddressCache.ProtocolString(this.protocol));
      this.httpClient.DefaultRequestHeaders.Add("x-ms-version", HttpConstants.Versions.CurrentVersion);
      this.httpClient.AddUserAgentHeader(userAgent);
      this.httpClient.AddApiTypeHeader(apiType);
    }

    public Uri ServiceEndpoint => this.serviceEndpoint;

    [SuppressMessage("", "AsyncFixer02", Justification = "Multi task completed with await")]
    [SuppressMessage("", "AsyncFixer04", Justification = "Multi task completed outside of await")]
    public async Task OpenAsync(
      string databaseName,
      DocumentCollection collection,
      IReadOnlyList<PartitionKeyRangeIdentity> partitionKeyRangeIdentities,
      CancellationToken cancellationToken)
    {
      List<Task<FeedResource<Address>>> taskList = new List<Task<FeedResource<Address>>>();
      int count1 = 50;
      using (DocumentServiceRequest fromName = DocumentServiceRequest.CreateFromName(Microsoft.Azure.Documents.OperationType.Read, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}", (object) "dbs", (object) Uri.EscapeUriString(databaseName), (object) "colls", (object) Uri.EscapeUriString(collection.Id)), ResourceType.Collection, AuthorizationTokenType.PrimaryMasterKey))
      {
        for (int count2 = 0; count2 < partitionKeyRangeIdentities.Count; count2 += count1)
          taskList.Add(this.GetServerAddressesViaGatewayAsync(fromName, collection.ResourceId, partitionKeyRangeIdentities.Skip<PartitionKeyRangeIdentity>(count2).Take<PartitionKeyRangeIdentity>(count1).Select<PartitionKeyRangeIdentity, string>((Func<PartitionKeyRangeIdentity, string>) (range => range.PartitionKeyRangeId)), false));
      }
      foreach (IEnumerable<Address> source in await Task.WhenAll<FeedResource<Address>>((IEnumerable<Task<FeedResource<Address>>>) taskList))
      {
        foreach (Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation> tuple in source.Where<Address>((Func<Address, bool>) (addressInfo => GatewayAddressCache.ProtocolFromString(addressInfo.Protocol) == this.protocol)).GroupBy<Address, string>((Func<Address, string>) (address => address.PartitionKeyRangeId), (IEqualityComparer<string>) StringComparer.Ordinal).Select<IGrouping<string, Address>, Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation>>((Func<IGrouping<string, Address>, Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation>>) (group => this.ToPartitionAddressAndRange(collection.ResourceId, (IList<Address>) group.ToList<Address>()))))
          this.serverPartitionAddressCache.Set(new PartitionKeyRangeIdentity(collection.ResourceId, tuple.Item1.PartitionKeyRangeId), tuple.Item2);
      }
    }

    public async Task<PartitionAddressInformation> TryGetAddresses(
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
      try
      {
        if (partitionKeyRangeIdentity.PartitionKeyRangeId == "M")
          return (await this.ResolveMasterAsync(request, forceRefreshPartitionAddresses)).Item2;
        DateTime comparisonValue;
        if (this.suboptimalServerPartitionTimestamps.TryGetValue(partitionKeyRangeIdentity, out comparisonValue) && DateTime.UtcNow.Subtract(comparisonValue) > TimeSpan.FromSeconds((double) this.suboptimalPartitionForceRefreshIntervalInSeconds) && this.suboptimalServerPartitionTimestamps.TryUpdate(partitionKeyRangeIdentity, DateTime.MaxValue, comparisonValue))
          forceRefreshPartitionAddresses = true;
        PartitionAddressInformation async;
        if (forceRefreshPartitionAddresses || request.ForceCollectionRoutingMapRefresh)
        {
          async = await this.serverPartitionAddressCache.GetAsync(partitionKeyRangeIdentity, (PartitionAddressInformation) null, (Func<Task<PartitionAddressInformation>>) (() => this.GetAddressesForRangeId(request, partitionKeyRangeIdentity.CollectionRid, partitionKeyRangeIdentity.PartitionKeyRangeId, forceRefreshPartitionAddresses)), cancellationToken, true);
          this.suboptimalServerPartitionTimestamps.TryRemove(partitionKeyRangeIdentity, out DateTime _);
        }
        else
          async = await this.serverPartitionAddressCache.GetAsync(partitionKeyRangeIdentity, (PartitionAddressInformation) null, (Func<Task<PartitionAddressInformation>>) (() => this.GetAddressesForRangeId(request, partitionKeyRangeIdentity.CollectionRid, partitionKeyRangeIdentity.PartitionKeyRangeId, false)), cancellationToken);
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
        this.suboptimalServerPartitionTimestamps.TryRemove(partitionKeyRangeIdentity, out DateTime _);
        return (PartitionAddressInformation) null;
      }
      catch (Exception ex)
      {
        if (forceRefreshPartitionAddresses)
          this.suboptimalServerPartitionTimestamps.TryRemove(partitionKeyRangeIdentity, out DateTime _);
        throw;
      }
    }

    public async Task<PartitionAddressInformation> UpdateAsync(
      PartitionKeyRangeIdentity partitionKeyRangeIdentity,
      CancellationToken cancellationToken)
    {
      if (partitionKeyRangeIdentity == null)
        throw new ArgumentNullException(nameof (partitionKeyRangeIdentity));
      return await this.serverPartitionAddressCache.GetAsync(partitionKeyRangeIdentity, (PartitionAddressInformation) null, (Func<Task<PartitionAddressInformation>>) (() => this.GetAddressesForRangeId((DocumentServiceRequest) null, partitionKeyRangeIdentity.CollectionRid, partitionKeyRangeIdentity.PartitionKeyRangeId, true)), cancellationToken, true);
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
          FeedResource<Address> addressesViaGatewayAsync = await this.GetMasterAddressesViaGatewayAsync(request, ResourceType.Database, (string) null, path, forceRefresh, false);
          tuple = this.ToPartitionAddressAndRange(string.Empty, (IList<Address>) addressesViaGatewayAsync.ToList<Address>());
          this.masterPartitionAddressCache = tuple;
          this.suboptimalMasterPartitionTimestamp = DateTime.MaxValue;
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

    private async Task<PartitionAddressInformation> GetAddressesForRangeId(
      DocumentServiceRequest request,
      string collectionRid,
      string partitionKeyRangeId,
      bool forceRefresh)
    {
      Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation> tuple = (await this.GetServerAddressesViaGatewayAsync(request, collectionRid, (IEnumerable<string>) new string[1]
      {
        partitionKeyRangeId
      }, (forceRefresh ? 1 : 0) != 0)).Where<Address>((Func<Address, bool>) (addressInfo => GatewayAddressCache.ProtocolFromString(addressInfo.Protocol) == this.protocol)).GroupBy<Address, string>((Func<Address, string>) (address => address.PartitionKeyRangeId), (IEqualityComparer<string>) StringComparer.Ordinal).Select<IGrouping<string, Address>, Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation>>((Func<IGrouping<string, Address>, Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation>>) (group => this.ToPartitionAddressAndRange(collectionRid, (IList<Address>) group.ToList<Address>()))).SingleOrDefault<Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation>>((Func<Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation>, bool>) (addressInfo => StringComparer.Ordinal.Equals(addressInfo.Item1.PartitionKeyRangeId, partitionKeyRangeId)));
      if (tuple == null)
      {
        PartitionKeyRangeGoneException rangeGoneException = new PartitionKeyRangeGoneException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.PartitionKeyRangeNotFound, (object) partitionKeyRangeId, (object) collectionRid));
        rangeGoneException.ResourceAddress = collectionRid;
        throw rangeGoneException;
      }
      return tuple.Item2;
    }

    private async Task<FeedResource<Address>> GetMasterAddressesViaGatewayAsync(
      DocumentServiceRequest request,
      ResourceType resourceType,
      string resourceAddress,
      string entryUrl,
      bool forceRefresh,
      bool useMasterCollectionResolver)
    {
      INameValueCollection parsedQuery = (INameValueCollection) new DictionaryNameValueCollection(StringComparer.Ordinal);
      parsedQuery.Add("$resolveFor", HttpUtility.UrlEncode(entryUrl));
      INameValueCollection nameValueCollection = (INameValueCollection) new DictionaryNameValueCollection(StringComparer.Ordinal);
      if (forceRefresh)
        nameValueCollection.Set("x-ms-force-refresh", bool.TrueString);
      if (useMasterCollectionResolver)
        nameValueCollection.Set("x-ms-use-master-collection-resolver", bool.TrueString);
      if (request.ForceCollectionRoutingMapRefresh)
        nameValueCollection.Set("x-ms-collectionroutingmap-refresh", bool.TrueString);
      parsedQuery.Add("$filter", this.protocolFilter);
      string resourcePath = PathsHelper.GetResourcePath(resourceType);
      nameValueCollection.Set("x-ms-date", DateTime.UtcNow.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture));
      string authorizationToken = this.tokenProvider.GetUserAuthorizationToken(resourceAddress, resourcePath, "GET", nameValueCollection, AuthorizationTokenType.PrimaryMasterKey, out string _);
      nameValueCollection.Set("authorization", authorizationToken);
      Uri uri = UrlUtility.SetQuery(this.addressEndpoint, UrlUtility.CreateQuery(parsedQuery));
      string identifier = GatewayAddressCache.LogAddressResolutionStart(request, uri);
      FeedResource<Address> resource;
      using (HttpResponseMessage httpResponseMessage = await this.httpClient.GetAsync(uri, nameValueCollection))
      {
        using (DocumentServiceResponse responseAsync = await ClientExtensions.ParseResponseAsync(httpResponseMessage))
        {
          GatewayAddressCache.LogAddressResolutionEnd(request, identifier);
          resource = responseAsync.GetResource<FeedResource<Address>>();
        }
      }
      return resource;
    }

    private async Task<FeedResource<Address>> GetServerAddressesViaGatewayAsync(
      DocumentServiceRequest request,
      string collectionRid,
      IEnumerable<string> partitionKeyRangeIds,
      bool forceRefresh)
    {
      string path = PathsHelper.GeneratePath(ResourceType.Document, collectionRid, true);
      INameValueCollection parsedQuery = (INameValueCollection) new DictionaryNameValueCollection();
      parsedQuery.Add("$resolveFor", HttpUtility.UrlEncode(path));
      INameValueCollection nameValueCollection = (INameValueCollection) new DictionaryNameValueCollection();
      if (forceRefresh)
        nameValueCollection.Set("x-ms-force-refresh", bool.TrueString);
      if (request != null && request.ForceCollectionRoutingMapRefresh)
        nameValueCollection.Set("x-ms-collectionroutingmap-refresh", bool.TrueString);
      parsedQuery.Add("$filter", this.protocolFilter);
      parsedQuery.Add("$partitionKeyRangeIds", string.Join(",", partitionKeyRangeIds));
      string resourcePath = PathsHelper.GetResourcePath(ResourceType.Document);
      nameValueCollection.Set("x-ms-date", DateTime.UtcNow.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture));
      string str = (string) null;
      try
      {
        str = this.tokenProvider.GetUserAuthorizationToken(collectionRid, resourcePath, "GET", nameValueCollection, AuthorizationTokenType.PrimaryMasterKey, out string _);
      }
      catch (UnauthorizedException ex)
      {
      }
      if (str == null && request != null && request.IsNameBased)
        str = this.tokenProvider.GetUserAuthorizationToken(PathsHelper.GetCollectionPath(request.ResourceAddress), resourcePath, "GET", nameValueCollection, AuthorizationTokenType.PrimaryMasterKey, out string _);
      nameValueCollection.Set("authorization", str);
      Uri uri = UrlUtility.SetQuery(this.addressEndpoint, UrlUtility.CreateQuery(parsedQuery));
      string identifier = GatewayAddressCache.LogAddressResolutionStart(request, uri);
      FeedResource<Address> resource;
      using (HttpResponseMessage httpResponseMessage = await this.httpClient.GetAsync(uri, nameValueCollection))
      {
        using (DocumentServiceResponse responseAsync = await ClientExtensions.ParseResponseAsync(httpResponseMessage))
        {
          GatewayAddressCache.LogAddressResolutionEnd(request, identifier);
          resource = responseAsync.GetResource<FeedResource<Address>>();
        }
      }
      return resource;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.httpClient == null)
        return;
      try
      {
        this.httpClient.Dispose();
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceWarning("Exception {0} thrown during dispose of HttpClient, this could happen if there are inflight request during the dispose of client", (object) ex);
      }
      this.httpClient = (HttpClient) null;
    }

    internal Tuple<PartitionKeyRangeIdentity, PartitionAddressInformation> ToPartitionAddressAndRange(
      string collectionRid,
      IList<Address> addresses)
    {
      Address address = addresses.First<Address>();
      AddressInformation[] array = addresses.Select<Address, AddressInformation>((Func<Address, AddressInformation>) (addr => new AddressInformation()
      {
        IsPrimary = addr.IsPrimary,
        PhysicalUri = addr.PhysicalUri,
        Protocol = GatewayAddressCache.ProtocolFromString(addr.Protocol),
        IsPublic = true
      })).ToArray<AddressInformation>();
      PartitionKeyRangeIdentity keyRangeIdentity = new PartitionKeyRangeIdentity(collectionRid, address.PartitionKeyRangeId);
      return Tuple.Create<PartitionKeyRangeIdentity, PartitionAddressInformation>(keyRangeIdentity, new PartitionAddressInformation(array, keyRangeIdentity.PartitionKeyRangeId == "M" ? (PartitionKeyRangeIdentity) null : keyRangeIdentity, this.serviceEndpoint));
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
  }
}
