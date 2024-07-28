// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.WebApi.OfferMeterHttpClient
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.WebApi
{
  [ClientCircuitBreakerSettings(30, 80, MaxConcurrentRequests = 40)]
  [ClientCancellationTimeout(60)]
  public class OfferMeterHttpClient : OfferMeterHttpClientBase
  {
    private readonly MultiScaleUnitDistributor<OfferMeterHttpClient> scaleUnitDistributor;

    public OfferMeterHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
      this.scaleUnitDistributor = new MultiScaleUnitDistributor<OfferMeterHttpClient>(this.BaseAddress, credentials, CommerceServiceInstanceTypes.Commerce);
    }

    public OfferMeterHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
      this.scaleUnitDistributor = new MultiScaleUnitDistributor<OfferMeterHttpClient>(this.BaseAddress, credentials, CommerceServiceInstanceTypes.Commerce);
    }

    public OfferMeterHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
      this.scaleUnitDistributor = new MultiScaleUnitDistributor<OfferMeterHttpClient>(this.BaseAddress, credentials, CommerceServiceInstanceTypes.Commerce);
    }

    public OfferMeterHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
      this.scaleUnitDistributor = new MultiScaleUnitDistributor<OfferMeterHttpClient>(this.BaseAddress, credentials, CommerceServiceInstanceTypes.Commerce);
    }

    public OfferMeterHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
      this.scaleUnitDistributor = new MultiScaleUnitDistributor<OfferMeterHttpClient>(this.BaseAddress, pipeline, CommerceServiceInstanceTypes.Commerce);
    }

    public async Task CreateOfferMeterDefinitionDistributedAsync(
      OfferMeter offerConfig,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      await this.scaleUnitDistributor.ExecuteDistributedAsync((Func<OfferMeterHttpClient, object, CancellationToken, Task>) (async (client, innerUserState, innerCancellationToken) => await client.CreateOfferMeterDefinitionImpl(offerConfig, innerUserState, innerCancellationToken).ConfigureAwait(false)), userState, cancellationToken).ConfigureAwait(false);
    }

    public async Task<OfferMeter> GetMeterFromMeterNameAsync(string offerMeterName) => await this.\u003C\u003En__0(offerMeterName, "MeterName").ConfigureAwait(false);

    public async Task<OfferMeter> GetMeterFromGalleryIdAsync(string offerMeterName) => await this.\u003C\u003En__0(offerMeterName, "GalleryId").ConfigureAwait(false);

    private async Task CreateOfferMeterDefinitionImpl(
      OfferMeter offerConfig,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      // ISSUE: reference to a compiler-generated method
      await this.\u003C\u003En__1(offerConfig, userState, cancellationToken).ConfigureAwait(false);
    }
  }
}
