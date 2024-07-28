// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.AzComm.WebApi.HttpClients.MeterUsage2HttpClient
// Assembly: Microsoft.VisualStudio.Services.AzComm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1B69FBB-72A0-4C7F-B8FC-E0B0311A8184
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.AzComm.WebApi.dll

using Microsoft.VisualStudio.Services.AzComm.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.AzComm.WebApi.HttpClients
{
  [ResourceArea("ED1325FD-71E8-4623-89F3-485951654312")]
  [ClientCircuitBreakerSettings(30, 80, MaxConcurrentRequests = 50)]
  [ClientCancellationTimeout(60)]
  public class MeterUsage2HttpClient : VssHttpClientBase
  {
    public MeterUsage2HttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public MeterUsage2HttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public MeterUsage2HttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public MeterUsage2HttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public MeterUsage2HttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<MeterUsage2GetResponse> GetMeterUsageAsync(
      Guid organizationId,
      Guid meterId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<MeterUsage2GetResponse>(new HttpMethod("GET"), new Guid("1f553305-7e51-4278-837f-0c208c4a98fc"), (object) new
      {
        organizationId = organizationId,
        meterId = meterId
      }, new ApiResourceVersion(6.0, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task ReportUsageAsync(
      MeterUsageReportRequest payload,
      Guid organizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MeterUsage2HttpClient usage2HttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1f553305-7e51-4278-837f-0c208c4a98fc");
      object obj1 = (object) new
      {
        organizationId = organizationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<MeterUsageReportRequest>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MeterUsage2HttpClient usage2HttpClient2 = usage2HttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(6.0, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await usage2HttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
