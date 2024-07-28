// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Generated.AnyProtocolPackagingInternalHttpClient
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 208E7E0C-C249-4CB0-B738-E2A4534A31E8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Generated
{
  [ResourceArea("E54D3ADC-D485-4536-B63D-1BD2BFC179BF")]
  public class AnyProtocolPackagingInternalHttpClient : VssHttpClientBase
  {
    public AnyProtocolPackagingInternalHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public AnyProtocolPackagingInternalHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public AnyProtocolPackagingInternalHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public AnyProtocolPackagingInternalHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public AnyProtocolPackagingInternalHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task IngestFromUpstreamAsync(
      ManualUpstreamIngestionParameters triggerParameters,
      string protocol,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AnyProtocolPackagingInternalHttpClient internalHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ad9e8ad1-93a9-4d37-9749-611ada29130d");
      object obj1 = (object) new
      {
        protocol = protocol,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ManualUpstreamIngestionParameters>(triggerParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      AnyProtocolPackagingInternalHttpClient internalHttpClient2 = internalHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await internalHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task IngestFromUpstreamAsync(
      ManualUpstreamIngestionParameters triggerParameters,
      string project,
      string protocol,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AnyProtocolPackagingInternalHttpClient internalHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ad9e8ad1-93a9-4d37-9749-611ada29130d");
      object obj1 = (object) new
      {
        project = project,
        protocol = protocol,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ManualUpstreamIngestionParameters>(triggerParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      AnyProtocolPackagingInternalHttpClient internalHttpClient2 = internalHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await internalHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task IngestFromUpstreamAsync(
      ManualUpstreamIngestionParameters triggerParameters,
      Guid project,
      string protocol,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AnyProtocolPackagingInternalHttpClient internalHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ad9e8ad1-93a9-4d37-9749-611ada29130d");
      object obj1 = (object) new
      {
        project = project,
        protocol = protocol,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ManualUpstreamIngestionParameters>(triggerParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      AnyProtocolPackagingInternalHttpClient internalHttpClient2 = internalHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await internalHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task NotifyOfPackagesIngestedInUpstreamsAsync(
      NotifyOfPackagesIngestedInUpstreamsParameters triggerParameters,
      string protocol,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AnyProtocolPackagingInternalHttpClient internalHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e98d3680-261f-4ed0-a0e1-74fc4372d8f5");
      object obj1 = (object) new{ protocol = protocol };
      HttpContent httpContent = (HttpContent) new ObjectContent<NotifyOfPackagesIngestedInUpstreamsParameters>(triggerParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      AnyProtocolPackagingInternalHttpClient internalHttpClient2 = internalHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await internalHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
