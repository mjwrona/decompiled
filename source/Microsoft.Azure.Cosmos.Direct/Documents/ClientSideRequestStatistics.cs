// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ClientSideRequestStatistics
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Rntbd;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal sealed class ClientSideRequestStatistics : IClientSideRequestStatistics
  {
    private static readonly SystemUsageMonitor systemUsageMonitor;
    private static readonly SystemUsageRecorder systemRecorder;
    private static readonly TimeSpan SystemUsageRecordInterval = TimeSpan.FromSeconds(10.0);
    private const string EnableCpuMonitorConfig = "CosmosDbEnableCpuMonitor";
    private const int MaxSupplementalRequestsForToString = 10;
    private static bool enableCpuMonitorFlag = true;
    private DateTime requestStartTime;
    private DateTime? requestEndTime;
    private object lockObject = new object();
    private object requestEndTimeLock = new object();
    private List<ClientSideRequestStatistics.StoreResponseStatistics> responseStatisticsList;
    private List<ClientSideRequestStatistics.StoreResponseStatistics> supplementalResponseStatisticsList;
    private Dictionary<string, ClientSideRequestStatistics.AddressResolutionStatistics> addressResolutionStatistics;
    private Lazy<List<ClientSideRequestStatistics.HttpResponseStatistics>> httpResponseStatisticsList;
    private SystemUsageHistory systemUsageHistory;

    static ClientSideRequestStatistics()
    {
      string appSetting = ConfigurationManager.AppSettings["CosmosDbEnableCpuMonitor"];
      if (!string.IsNullOrEmpty(appSetting) && !bool.TryParse(appSetting, out ClientSideRequestStatistics.enableCpuMonitorFlag))
        ClientSideRequestStatistics.enableCpuMonitorFlag = true;
      if (!ClientSideRequestStatistics.enableCpuMonitorFlag)
        return;
      ClientSideRequestStatistics.systemRecorder = new SystemUsageRecorder(nameof (ClientSideRequestStatistics), 6, ClientSideRequestStatistics.SystemUsageRecordInterval);
      ClientSideRequestStatistics.systemUsageMonitor = SystemUsageMonitor.CreateAndStart((IReadOnlyList<SystemUsageRecorder>) new List<SystemUsageRecorder>()
      {
        ClientSideRequestStatistics.systemRecorder
      });
    }

    public ClientSideRequestStatistics()
    {
      this.requestStartTime = DateTime.UtcNow;
      this.requestEndTime = new DateTime?();
      this.responseStatisticsList = new List<ClientSideRequestStatistics.StoreResponseStatistics>();
      this.supplementalResponseStatisticsList = new List<ClientSideRequestStatistics.StoreResponseStatistics>();
      this.addressResolutionStatistics = new Dictionary<string, ClientSideRequestStatistics.AddressResolutionStatistics>();
      this.ContactedReplicas = new List<TransportAddressUri>();
      this.FailedReplicas = new HashSet<TransportAddressUri>();
      this.RegionsContacted = new HashSet<(string, Uri)>();
      this.httpResponseStatisticsList = new Lazy<List<ClientSideRequestStatistics.HttpResponseStatistics>>();
    }

    public List<TransportAddressUri> ContactedReplicas { get; set; }

    public HashSet<TransportAddressUri> FailedReplicas { get; private set; }

    public HashSet<(string, Uri)> RegionsContacted { get; private set; }

    public TimeSpan? RequestLatency => !this.requestEndTime.HasValue ? new TimeSpan?() : new TimeSpan?(this.requestEndTime.Value - this.requestStartTime);

    public bool? IsCpuHigh => this.systemUsageHistory?.IsCpuHigh;

    public bool? IsCpuThreadStarvation => this.systemUsageHistory?.IsCpuThreadStarvation;

    internal static void DisableCpuMonitor()
    {
      if (!ClientSideRequestStatistics.enableCpuMonitorFlag)
        return;
      ClientSideRequestStatistics.enableCpuMonitorFlag = false;
      if (ClientSideRequestStatistics.systemRecorder == null)
        return;
      ClientSideRequestStatistics.systemUsageMonitor.Stop();
      ClientSideRequestStatistics.systemUsageMonitor.Dispose();
    }

    public void RecordRequest(DocumentServiceRequest request)
    {
    }

    public void RecordResponse(
      DocumentServiceRequest request,
      StoreResult storeResult,
      DateTime startTimeUtc,
      DateTime endTimeUtc)
    {
      this.UpdateRequestEndTime(endTimeUtc);
      ClientSideRequestStatistics.StoreResponseStatistics responseStatistics;
      responseStatistics.RequestStartTime = startTimeUtc;
      responseStatistics.RequestResponseTime = endTimeUtc;
      responseStatistics.StoreResult = storeResult;
      responseStatistics.RequestOperationType = request.OperationType;
      responseStatistics.RequestResourceType = request.ResourceType;
      Uri locationEndpointToRoute = request.RequestContext.LocationEndpointToRoute;
      string str = request.RequestContext.RegionName ?? string.Empty;
      lock (this.lockObject)
      {
        if (locationEndpointToRoute != (Uri) null)
          this.RegionsContacted.Add((str, locationEndpointToRoute));
        if (responseStatistics.RequestOperationType == OperationType.Head || responseStatistics.RequestOperationType == OperationType.HeadFeed)
          this.supplementalResponseStatisticsList.Add(responseStatistics);
        else
          this.responseStatisticsList.Add(responseStatistics);
      }
    }

    public void RecordException(
      DocumentServiceRequest request,
      Exception exception,
      DateTime startTime,
      DateTime endTimeUtc)
    {
      this.UpdateRequestEndTime(endTimeUtc);
    }

    public string RecordAddressResolutionStart(Uri targetEndpoint)
    {
      string key = Guid.NewGuid().ToString();
      ClientSideRequestStatistics.AddressResolutionStatistics resolutionStatistics = new ClientSideRequestStatistics.AddressResolutionStatistics()
      {
        StartTime = DateTime.UtcNow,
        EndTime = DateTime.MaxValue,
        TargetEndpoint = targetEndpoint == (Uri) null ? "<NULL>" : targetEndpoint.ToString()
      };
      lock (this.lockObject)
        this.addressResolutionStatistics.Add(key, resolutionStatistics);
      return key;
    }

    public void RecordAddressResolutionEnd(string identifier)
    {
      if (string.IsNullOrEmpty(identifier))
        return;
      DateTime utcNow = DateTime.UtcNow;
      this.UpdateRequestEndTime(DateTime.UtcNow);
      lock (this.lockObject)
      {
        if (!this.addressResolutionStatistics.ContainsKey(identifier))
          throw new ArgumentException("Identifier {0} does not exist. Please call start before calling end.", identifier);
        this.addressResolutionStatistics[identifier].EndTime = utcNow;
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
      lock (this.httpResponseStatisticsList)
        this.httpResponseStatisticsList.Value.Add(new ClientSideRequestStatistics.HttpResponseStatistics(requestStartTimeUtc, utcNow, request.RequestUri, request.Method, resourceType, response, (Exception) null));
    }

    public void RecordHttpException(
      HttpRequestMessage request,
      Exception exception,
      ResourceType resourceType,
      DateTime requestStartTimeUtc)
    {
      DateTime utcNow = DateTime.UtcNow;
      this.UpdateRequestEndTime(utcNow);
      lock (this.httpResponseStatisticsList)
        this.httpResponseStatisticsList.Value.Add(new ClientSideRequestStatistics.HttpResponseStatistics(requestStartTimeUtc, utcNow, request.RequestUri, request.Method, resourceType, (HttpResponseMessage) null, exception));
    }

    private void UpdateRequestEndTime(DateTime requestEndTimeUtc)
    {
      lock (this.requestEndTimeLock)
      {
        if (this.requestEndTime.HasValue)
        {
          DateTime dateTime = requestEndTimeUtc;
          DateTime? requestEndTime = this.requestEndTime;
          if ((requestEndTime.HasValue ? (dateTime > requestEndTime.GetValueOrDefault() ? 1 : 0) : 0) == 0)
            return;
        }
        this.UpdateSystemUsageHistory();
        this.requestEndTime = new DateTime?(requestEndTimeUtc);
      }
    }

    private void UpdateSystemUsageHistory()
    {
      if (!ClientSideRequestStatistics.enableCpuMonitorFlag || ClientSideRequestStatistics.systemRecorder == null || this.systemUsageHistory != null && !(this.systemUsageHistory.LastTimestamp + ClientSideRequestStatistics.SystemUsageRecordInterval < DateTime.UtcNow))
        return;
      try
      {
        this.systemUsageHistory = ClientSideRequestStatistics.systemRecorder.Data;
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceCritical("System usage monitor failed with an unexpected exception: {0}", (object) ex);
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      this.AppendToBuilder(stringBuilder);
      return stringBuilder.ToString();
    }

    public void AppendToBuilder(StringBuilder stringBuilder)
    {
      if (stringBuilder == null)
        throw new ArgumentNullException(nameof (stringBuilder));
      lock (this.lockObject)
      {
        stringBuilder.AppendLine();
        string str = !this.requestEndTime.HasValue ? "No response recorded; Current Time: " + DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) : this.requestEndTime.Value.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture);
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "RequestStartTime: {0}, RequestEndTime: {1},  Number of regions attempted:{2}", (object) this.requestStartTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) str, (object) (this.RegionsContacted.Count == 0 ? 1 : this.RegionsContacted.Count));
        stringBuilder.AppendLine();
        if (this.systemUsageHistory == null)
          this.UpdateSystemUsageHistory();
        if (this.systemUsageHistory != null && this.systemUsageHistory.Values.Count > 0)
        {
          this.systemUsageHistory.AppendJsonString(stringBuilder);
          stringBuilder.AppendLine();
        }
        else
          stringBuilder.AppendLine("System history not available.");
        foreach (ClientSideRequestStatistics.StoreResponseStatistics responseStatistics in this.responseStatisticsList)
        {
          responseStatistics.AppendToBuilder(stringBuilder);
          stringBuilder.AppendLine();
        }
        foreach (ClientSideRequestStatistics.AddressResolutionStatistics resolutionStatistics in this.addressResolutionStatistics.Values)
        {
          resolutionStatistics.AppendToBuilder(stringBuilder);
          stringBuilder.AppendLine();
        }
        lock (this.httpResponseStatisticsList)
        {
          if (this.httpResponseStatisticsList.IsValueCreated)
          {
            foreach (ClientSideRequestStatistics.HttpResponseStatistics responseStatistics in this.httpResponseStatisticsList.Value)
            {
              responseStatistics.AppendToBuilder(stringBuilder);
              stringBuilder.AppendLine();
            }
          }
        }
        int count = this.supplementalResponseStatisticsList.Count;
        int num = Math.Max(count - 10, 0);
        if (num != 0)
        {
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "  -- Displaying only the last {0} head/headfeed requests. Total head/headfeed requests: {1}", (object) 10, (object) count);
          stringBuilder.AppendLine();
        }
        for (int index = num; index < count; ++index)
        {
          this.supplementalResponseStatisticsList[index].AppendToBuilder(stringBuilder);
          stringBuilder.AppendLine();
        }
      }
    }

    public struct StoreResponseStatistics
    {
      public DateTime RequestStartTime;
      public DateTime RequestResponseTime;
      public StoreResult StoreResult;
      public ResourceType RequestResourceType;
      public OperationType RequestOperationType;

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        this.AppendToBuilder(stringBuilder);
        return stringBuilder.ToString();
      }

      public void AppendToBuilder(StringBuilder stringBuilder)
      {
        if (stringBuilder == null)
          throw new ArgumentNullException(nameof (stringBuilder));
        stringBuilder.Append("RequestStart: ");
        stringBuilder.Append(this.RequestStartTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
        stringBuilder.Append("; ResponseTime: ");
        stringBuilder.Append(this.RequestResponseTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
        stringBuilder.Append("; StoreResult: ");
        if (this.StoreResult != null)
          this.StoreResult.AppendToBuilder(stringBuilder);
        stringBuilder.AppendLine();
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " ResourceType: {0}, OperationType: {1}", (object) this.RequestResourceType, (object) this.RequestOperationType);
      }
    }

    private class AddressResolutionStatistics
    {
      public DateTime StartTime { get; set; }

      public DateTime EndTime { get; set; }

      public string TargetEndpoint { get; set; }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        this.AppendToBuilder(stringBuilder);
        return stringBuilder.ToString();
      }

      public void AppendToBuilder(StringBuilder stringBuilder)
      {
        StringBuilder stringBuilder1 = stringBuilder != null ? stringBuilder : throw new ArgumentNullException(nameof (stringBuilder));
        DateTime dateTime = this.StartTime;
        string str1 = "AddressResolution - StartTime: " + dateTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ", ";
        StringBuilder stringBuilder2 = stringBuilder1.Append(str1);
        dateTime = this.EndTime;
        string str2 = "EndTime: " + dateTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ", ";
        stringBuilder2.Append(str2).Append("TargetEndpoint: ").Append(this.TargetEndpoint);
      }
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

      public void AppendToBuilder(StringBuilder stringBuilder)
      {
        if (stringBuilder == null)
          throw new ArgumentNullException(nameof (stringBuilder));
        stringBuilder.Append("HttpResponseStatistics - ").Append("RequestStartTime: ").Append(this.RequestStartTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture)).Append(", DurationInMs: ").Append(this.Duration.TotalMilliseconds).Append(", RequestUri: ").Append((object) this.RequestUri).Append(", ResourceType: ").Append((object) this.ResourceType).Append(", HttpMethod: ").Append((object) this.HttpMethod);
        if (this.Exception != null)
          stringBuilder.Append(", ExceptionType: ").Append((object) this.Exception.GetType()).Append(", ExceptionMessage: ").Append(this.Exception.Message);
        if (this.HttpResponseMessage == null)
          return;
        stringBuilder.Append(", StatusCode: ").Append((object) this.HttpResponseMessage.StatusCode);
        if (this.HttpResponseMessage.IsSuccessStatusCode)
          return;
        stringBuilder.Append(", ReasonPhrase: ").Append(this.HttpResponseMessage.ReasonPhrase);
      }
    }
  }
}
