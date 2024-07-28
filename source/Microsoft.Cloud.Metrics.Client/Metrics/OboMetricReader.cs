// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.OboMetricReader
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Query;
using Microsoft.Cloud.Metrics.Client.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.Metrics
{
  public sealed class OboMetricReader
  {
    private readonly ConnectionInfo connectionInfo;
    private readonly HttpClient httpClient;
    private readonly string clientId;

    public OboMetricReader(ConnectionInfo connectionInfo, string clientId = "OBO")
    {
      this.connectionInfo = connectionInfo != null ? connectionInfo : throw new ArgumentNullException(nameof (connectionInfo));
      this.clientId = clientId;
      this.httpClient = HttpClientHelper.CreateHttpClientWithAuthInfo(connectionInfo);
    }

    public async Task<IReadOnlyList<IFilteredTimeSeriesQueryResponse>> GetFilteredTimeSeriesAsync(
      DateTime startTimeUtc,
      int numMinutes,
      string resourceId,
      SamplingType[] samplingTypes,
      List<string> categories,
      IReadOnlyDictionary<string, string> additionalTracingInformation = null)
    {
      Uri url = new Uri(this.connectionInfo.Endpoint, string.Format("/api/getMetricsForOBO/v2/serializationVersion/{0}/startMinute/{1}/numMinutes/{2}", (object) (byte) 3, (object) startTimeUtc.ToString("yyyy-MM-ddTHH:mmZ"), (object) numMinutes));
      Guid traceId = Guid.NewGuid();
      Tuple<List<string>, SamplingType[], List<string>> tuple = Tuple.Create<List<string>, SamplingType[], List<string>>(new List<string>()
      {
        resourceId
      }, samplingTypes, categories);
      HttpMethod post = HttpMethod.Post;
      HttpClient httpClient = this.httpClient;
      string traceId1 = MetricReader.BuildTraceId(additionalTracingInformation, new Guid?(traceId));
      Tuple<List<string>, SamplingType[], List<string>> httpContent = tuple;
      string clientId = this.clientId;
      string handlingRpServerId;
      IReadOnlyList<IFilteredTimeSeriesQueryResponse> seriesQueryResponseList;
      using (HttpResponseMessage httpResponseMessage = (await HttpClientHelper.GetResponseWithCustomTraceId(url, post, httpClient, (string) null, (string) null, traceId1, (object) httpContent, clientId, numAttempts: 1).ConfigureAwait(false)).Item2)
      {
        IEnumerable<string> values;
        httpResponseMessage.Headers.TryGetValues("__HandlingRpServerId__", out values);
        handlingRpServerId = values != null ? values.FirstOrDefault<string>() : (string) null;
        using (Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
          seriesQueryResponseList = FilteredQueryResponseDeserializer.Deserialize(stream);
      }
      foreach (IFilteredTimeSeriesQueryResponse seriesQueryResponse in (IEnumerable<IFilteredTimeSeriesQueryResponse>) seriesQueryResponseList)
      {
        seriesQueryResponse.DiagnosticInfo.TraceId = traceId.ToString("B");
        seriesQueryResponse.DiagnosticInfo.HandlingServerId = handlingRpServerId;
      }
      IReadOnlyList<IFilteredTimeSeriesQueryResponse> filteredTimeSeriesAsync = seriesQueryResponseList;
      handlingRpServerId = (string) null;
      return filteredTimeSeriesAsync;
    }
  }
}
