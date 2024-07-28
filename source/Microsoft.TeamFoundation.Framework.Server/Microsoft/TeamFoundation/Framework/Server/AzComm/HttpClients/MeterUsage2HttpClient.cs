// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AzComm.HttpClients.MeterUsage2HttpClient
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.AzComm.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server.AzComm.HttpClients
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
  }
}
