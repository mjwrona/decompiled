// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.TraceData.ClientSideRequestStatisticsTraceDatum
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Handler;
using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Rntbd;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Microsoft.Azure.Cosmos.Tracing.TraceData
{
  internal sealed class ClientSideRequestStatisticsTraceDatum : 
    TraceDatum,
    IClientSideRequestStatistics
  {
    private static readonly IReadOnlyDictionary<string, ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics> EmptyEndpointToAddressResolutionStatistics = (IReadOnlyDictionary<string, ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics>) new Dictionary<string, ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics>();
    private static readonly IReadOnlyList<ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics> EmptyStoreResponseStatistics = (IReadOnlyList<ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics>) new List<ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics>();
    private static readonly IReadOnlyList<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics> EmptyHttpResponseStatistics = (IReadOnlyList<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics>) new List<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics>();
    private readonly List<(PartitionAddressInformation existing, PartitionAddressInformation newInfo)> partitionAddressInformationRefreshes = new List<(PartitionAddressInformation, PartitionAddressInformation)>();
    internal static readonly string HttpRequestRegionNameProperty = "regionName";
    private readonly object requestEndTimeLock = new object();
    private readonly Dictionary<string, ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics> endpointToAddressResolutionStats;
    private readonly List<ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics> storeResponseStatistics;
    private readonly List<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics> httpResponseStatistics;
    private IReadOnlyDictionary<string, ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics> shallowCopyOfEndpointToAddressResolutionStatistics;
    private IReadOnlyList<ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics> shallowCopyOfStoreResponseStatistics;
    private IReadOnlyList<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics> shallowCopyOfHttpResponseStatistics;
    private SystemUsageHistory systemUsageHistory;
    public TraceSummary TraceSummary;

    public ClientSideRequestStatisticsTraceDatum(DateTime startTime, TraceSummary summary)
    {
      this.RequestStartTimeUtc = startTime;
      this.RequestEndTimeUtc = new DateTime?();
      this.endpointToAddressResolutionStats = new Dictionary<string, ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics>();
      this.ContactedReplicas = new List<TransportAddressUri>();
      this.storeResponseStatistics = new List<ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics>();
      this.FailedReplicas = new HashSet<TransportAddressUri>();
      this.RegionsContacted = new HashSet<(string, Uri)>();
      this.httpResponseStatistics = new List<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics>();
      this.TraceSummary = summary;
    }

    public DateTime RequestStartTimeUtc { get; }

    public DateTime? RequestEndTimeUtc { get; private set; }

    public IReadOnlyDictionary<string, ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics> EndpointToAddressResolutionStatistics
    {
      get
      {
        if (this.endpointToAddressResolutionStats.Count == 0)
          return ClientSideRequestStatisticsTraceDatum.EmptyEndpointToAddressResolutionStatistics;
        lock (this.endpointToAddressResolutionStats)
        {
          if (this.shallowCopyOfEndpointToAddressResolutionStatistics == null)
            this.shallowCopyOfEndpointToAddressResolutionStatistics = (IReadOnlyDictionary<string, ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics>) new Dictionary<string, ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics>((IDictionary<string, ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics>) this.endpointToAddressResolutionStats);
          return this.shallowCopyOfEndpointToAddressResolutionStatistics;
        }
      }
    }

    public List<TransportAddressUri> ContactedReplicas { get; set; }

    public HashSet<TransportAddressUri> FailedReplicas { get; }

    public HashSet<(string, Uri)> RegionsContacted { get; }

    public IReadOnlyList<ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics> StoreResponseStatisticsList
    {
      get
      {
        if (this.storeResponseStatistics.Count == 0)
          return ClientSideRequestStatisticsTraceDatum.EmptyStoreResponseStatistics;
        lock (this.storeResponseStatistics)
        {
          if (this.shallowCopyOfStoreResponseStatistics == null)
            this.shallowCopyOfStoreResponseStatistics = (IReadOnlyList<ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics>) new List<ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics>((IEnumerable<ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics>) this.storeResponseStatistics);
          return this.shallowCopyOfStoreResponseStatistics;
        }
      }
    }

    public IReadOnlyList<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics> HttpResponseStatisticsList
    {
      get
      {
        if (this.httpResponseStatistics.Count == 0)
          return ClientSideRequestStatisticsTraceDatum.EmptyHttpResponseStatistics;
        lock (this.httpResponseStatistics)
        {
          if (this.shallowCopyOfHttpResponseStatistics == null)
            this.shallowCopyOfHttpResponseStatistics = (IReadOnlyList<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics>) new List<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics>((IEnumerable<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics>) this.httpResponseStatistics);
          return this.shallowCopyOfHttpResponseStatistics;
        }
      }
    }

    public TimeSpan? RequestLatency => this.RequestEndTimeUtc.HasValue ? new TimeSpan?(this.RequestEndTimeUtc.Value - this.RequestStartTimeUtc) : new TimeSpan?();

    public bool? IsCpuHigh => this.systemUsageHistory?.IsCpuHigh;

    public bool? IsCpuThreadStarvation => this.systemUsageHistory?.IsCpuThreadStarvation;

    public void RecordRequest(DocumentServiceRequest request)
    {
    }

    public void RecordAddressCachRefreshContent(
      PartitionAddressInformation existingInfo,
      PartitionAddressInformation newInfo)
    {
      lock (this.partitionAddressInformationRefreshes)
        this.partitionAddressInformationRefreshes.Add((existingInfo, newInfo));
    }

    public void WriteAddressCachRefreshContent(IJsonWriter jsonWriter)
    {
      if (this.partitionAddressInformationRefreshes.Count == 0)
        return;
      lock (this.partitionAddressInformationRefreshes)
      {
        if (this.partitionAddressInformationRefreshes.Count == 0)
          return;
        jsonWriter.WriteFieldName("ForceAddressRefresh");
        jsonWriter.WriteArrayStart();
        foreach ((PartitionAddressInformation addressInformation1, PartitionAddressInformation addressInformation2) in this.partitionAddressInformationRefreshes)
        {
          if (ClientSideRequestStatisticsTraceDatum.IsSamePartitionAddressInformation(addressInformation1, addressInformation2))
          {
            jsonWriter.WriteObjectStart();
            jsonWriter.WriteFieldName("No change to cache");
            jsonWriter.WriteArrayStart();
            foreach (AddressInformation allAddress in (IEnumerable<AddressInformation>) addressInformation1.AllAddresses)
              jsonWriter.WriteStringValue(allAddress.PhysicalUri);
            jsonWriter.WriteArrayEnd();
            jsonWriter.WriteObjectEnd();
          }
          else
          {
            jsonWriter.WriteObjectStart();
            jsonWriter.WriteFieldName("Original");
            jsonWriter.WriteArrayStart();
            foreach (AddressInformation allAddress in (IEnumerable<AddressInformation>) addressInformation1.AllAddresses)
              jsonWriter.WriteStringValue(allAddress.PhysicalUri);
            jsonWriter.WriteArrayEnd();
            jsonWriter.WriteFieldName("New");
            jsonWriter.WriteArrayStart();
            foreach (AddressInformation allAddress in (IEnumerable<AddressInformation>) addressInformation2.AllAddresses)
              jsonWriter.WriteStringValue(allAddress.PhysicalUri);
            jsonWriter.WriteArrayEnd();
            jsonWriter.WriteObjectEnd();
          }
        }
        jsonWriter.WriteArrayEnd();
      }
    }

    private static bool IsSamePartitionAddressInformation(
      PartitionAddressInformation info1,
      PartitionAddressInformation info2)
    {
      if (info1 == null && info2 == null)
        return true;
      if (info1 != null && info2 == null || info1 == null && info2 != null || info1.AllAddresses.Count != info2.AllAddresses.Count)
        return false;
      for (int index = 0; index < info1.AllAddresses.Count; ++index)
      {
        AddressInformation allAddress1 = info1.AllAddresses[index];
        AddressInformation allAddress2 = info2.AllAddresses[index];
        if (allAddress1.Protocol != allAddress2.Protocol || allAddress1.IsPrimary != allAddress2.IsPrimary || allAddress1.IsPublic != allAddress2.IsPublic || allAddress1.PhysicalUri != allAddress2.PhysicalUri)
          return false;
      }
      return true;
    }

    public void RecordResponse(
      DocumentServiceRequest request,
      StoreResult storeResult,
      DateTime startTimeUtc,
      DateTime endTimeUtc)
    {
      this.UpdateRequestEndTime(endTimeUtc);
      Uri locationEndpointToRoute = request.RequestContext.LocationEndpointToRoute;
      string regionName = request.RequestContext.RegionName;
      ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics responseStatistics = new ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics(new DateTime?(startTimeUtc), endTimeUtc, storeResult, request.ResourceType, request.OperationType, request.Headers["x-ms-session-token"], locationEndpointToRoute);
      lock (this.storeResponseStatistics)
      {
        if (locationEndpointToRoute != (Uri) null)
          this.TraceSummary?.AddRegionContacted(regionName, locationEndpointToRoute);
        if (responseStatistics.StoreResult != null && !((HttpStatusCode) responseStatistics.StoreResult.StatusCode).IsSuccess() && (responseStatistics.StoreResult.StatusCode != StatusCodes.NotFound || responseStatistics.StoreResult.SubStatusCode != SubStatusCodes.Unknown) && (responseStatistics.StoreResult.StatusCode != StatusCodes.Conflict || responseStatistics.StoreResult.SubStatusCode != SubStatusCodes.Unknown) && (responseStatistics.StoreResult.StatusCode != StatusCodes.PreconditionFailed || responseStatistics.StoreResult.SubStatusCode != SubStatusCodes.Unknown) && this.TraceSummary != null)
          this.TraceSummary.IncrementFailedCount();
        this.shallowCopyOfStoreResponseStatistics = (IReadOnlyList<ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics>) null;
        this.storeResponseStatistics.Add(responseStatistics);
      }
    }

    public void RecordException(
      DocumentServiceRequest request,
      Exception exception,
      DateTime startTimeUtc,
      DateTime endTimeUtc)
    {
      this.UpdateRequestEndTime(endTimeUtc);
    }

    public string RecordAddressResolutionStart(Uri targetEndpoint)
    {
      string key = Guid.NewGuid().ToString();
      ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics resolutionStatistics = new ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics(DateTime.UtcNow, DateTime.MaxValue, targetEndpoint == (Uri) null ? "<NULL>" : targetEndpoint.ToString());
      lock (this.endpointToAddressResolutionStats)
      {
        this.shallowCopyOfEndpointToAddressResolutionStatistics = (IReadOnlyDictionary<string, ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics>) null;
        this.endpointToAddressResolutionStats.Add(key, resolutionStatistics);
      }
      return key;
    }

    public void RecordAddressResolutionEnd(string identifier)
    {
      if (string.IsNullOrEmpty(identifier))
        return;
      DateTime utcNow = DateTime.UtcNow;
      this.UpdateRequestEndTime(utcNow);
      lock (this.endpointToAddressResolutionStats)
      {
        ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics resolutionStatistics = this.endpointToAddressResolutionStats.ContainsKey(identifier) ? this.endpointToAddressResolutionStats[identifier] : throw new ArgumentException("Identifier {0} does not exist. Please call start before calling end.", identifier);
        this.shallowCopyOfEndpointToAddressResolutionStatistics = (IReadOnlyDictionary<string, ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics>) null;
        this.endpointToAddressResolutionStats[identifier] = new ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics(resolutionStatistics.StartTime, utcNow, resolutionStatistics.TargetEndpoint);
      }
    }

    public void RecordHttpResponse(
      HttpRequestMessage request,
      HttpResponseMessage response,
      ResourceType resourceType,
      DateTime requestStartTimeUtc)
    {
      DateTime utcNow = DateTime.UtcNow;
      this.UpdateRequestEndTime(utcNow);
      lock (this.httpResponseStatistics)
      {
        Uri requestUri = request.RequestUri;
        object obj;
        if (request.Properties != null && request.Properties.TryGetValue(ClientSideRequestStatisticsTraceDatum.HttpRequestRegionNameProperty, out obj))
          this.TraceSummary.AddRegionContacted(Convert.ToString(obj), requestUri);
        this.shallowCopyOfHttpResponseStatistics = (IReadOnlyList<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics>) null;
        this.httpResponseStatistics.Add(new ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics(requestStartTimeUtc, utcNow, request.RequestUri, request.Method, resourceType, response, (Exception) null));
      }
    }

    public void RecordHttpException(
      HttpRequestMessage request,
      Exception exception,
      ResourceType resourceType,
      DateTime requestStartTimeUtc)
    {
      DateTime utcNow = DateTime.UtcNow;
      this.UpdateRequestEndTime(utcNow);
      lock (this.httpResponseStatistics)
      {
        Uri requestUri = request.RequestUri;
        object obj;
        if (request.Properties != null && request.Properties.TryGetValue(ClientSideRequestStatisticsTraceDatum.HttpRequestRegionNameProperty, out obj))
          this.TraceSummary.AddRegionContacted(Convert.ToString(obj), requestUri);
        this.shallowCopyOfHttpResponseStatistics = (IReadOnlyList<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics>) null;
        this.httpResponseStatistics.Add(new ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics(requestStartTimeUtc, utcNow, request.RequestUri, request.Method, resourceType, (HttpResponseMessage) null, exception));
      }
    }

    private DateTime UpdateRequestEndTime(DateTime requestEndTimeUtc)
    {
      lock (this.requestEndTimeLock)
      {
        if (this.RequestEndTimeUtc.HasValue)
        {
          DateTime dateTime = requestEndTimeUtc;
          DateTime? requestEndTimeUtc1 = this.RequestEndTimeUtc;
          if ((requestEndTimeUtc1.HasValue ? (dateTime > requestEndTimeUtc1.GetValueOrDefault() ? 1 : 0) : 0) == 0)
            goto label_4;
        }
        this.RequestEndTimeUtc = new DateTime?(requestEndTimeUtc);
label_4:
        this.UpdateSystemUsage();
      }
      return requestEndTimeUtc;
    }

    public void UpdateSystemUsage()
    {
      if (this.systemUsageHistory != null && this.systemUsageHistory.Values.Count != 0 && !(this.systemUsageHistory.LastTimestamp + DiagnosticsHandlerHelper.DiagnosticsRefreshInterval < DateTime.UtcNow))
        return;
      this.systemUsageHistory = DiagnosticsHandlerHelper.Instance.GetDiagnosticsSystemHistory();
    }

    internal override void Accept(ITraceDatumVisitor traceDatumVisitor) => traceDatumVisitor.Visit(this);

    public void AppendToBuilder(StringBuilder stringBuilder) => throw new NotImplementedException();

    public readonly struct AddressResolutionStatistics
    {
      public AddressResolutionStatistics(
        DateTime startTime,
        DateTime endTime,
        string targetEndpoint)
      {
        this.StartTime = startTime;
        this.EndTime = new DateTime?(endTime);
        this.TargetEndpoint = targetEndpoint ?? throw new ArgumentNullException(nameof (startTime));
      }

      public DateTime StartTime { get; }

      public DateTime? EndTime { get; }

      public string TargetEndpoint { get; }
    }

    public sealed class StoreResponseStatistics
    {
      public StoreResponseStatistics(
        DateTime? requestStartTime,
        DateTime requestResponseTime,
        StoreResult storeResult,
        ResourceType resourceType,
        OperationType operationType,
        string requestSessionToken,
        Uri locationEndpoint)
      {
        this.RequestStartTime = requestStartTime;
        this.RequestResponseTime = requestResponseTime;
        this.StoreResult = storeResult;
        this.RequestResourceType = resourceType;
        this.RequestOperationType = operationType;
        this.RequestSessionToken = requestSessionToken;
        this.LocationEndpoint = locationEndpoint;
        this.IsSupplementalResponse = operationType == OperationType.Head || operationType == OperationType.HeadFeed;
      }

      public DateTime? RequestStartTime { get; }

      public DateTime RequestResponseTime { get; }

      public StoreResult StoreResult { get; }

      public ResourceType RequestResourceType { get; }

      public OperationType RequestOperationType { get; }

      public string RequestSessionToken { get; }

      public Uri LocationEndpoint { get; }

      public bool IsSupplementalResponse { get; }
    }

    public readonly struct HttpResponseStatistics
    {
      public HttpResponseStatistics(
        DateTime requestStartTime,
        DateTime requestEndTime,
        Uri requestUri,
        HttpMethod httpMethod,
        ResourceType resourceType,
        HttpResponseMessage responseMessage,
        Exception exception)
      {
        this.RequestStartTime = requestStartTime;
        this.Duration = requestEndTime - requestStartTime;
        this.HttpResponseMessage = responseMessage;
        this.Exception = exception;
        this.ResourceType = resourceType;
        this.HttpMethod = httpMethod;
        this.RequestUri = requestUri;
        if (responseMessage != null)
          this.ActivityId = new Microsoft.Azure.Cosmos.Headers(GatewayStoreClient.ExtractResponseHeaders(responseMessage)).ActivityId ?? System.Diagnostics.Trace.CorrelationManager.ActivityId.ToString();
        else
          this.ActivityId = System.Diagnostics.Trace.CorrelationManager.ActivityId.ToString();
      }

      public DateTime RequestStartTime { get; }

      public TimeSpan Duration { get; }

      public HttpResponseMessage HttpResponseMessage { get; }

      public Exception Exception { get; }

      public ResourceType ResourceType { get; }

      public HttpMethod HttpMethod { get; }

      public Uri RequestUri { get; }

      public string ActivityId { get; }
    }
  }
}
