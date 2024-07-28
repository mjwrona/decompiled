// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.WebApi.DevSecOpsHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78BC9F0A-6262-4C96-81AF-14E523464B20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.DevSecOps.WebApi
{
  [ResourceArea("{7F7E9705-96B8-4DA4-AF41-9E272C98DB69}")]
  [ClientCancellationTimeout(90)]
  [ClientCircuitBreakerSettings(10, 80)]
  public abstract class DevSecOpsHttpClientBase : VssHttpClientBase
  {
    public DevSecOpsHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public DevSecOpsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public DevSecOpsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public DevSecOpsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public DevSecOpsHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<BatchScanResult> ScanBatchAsync(
      Stream uploadStream,
      ContainerMetadata containerMetadata = null,
      Guid? correlationId = null,
      BatchScanOptions batchScanOptions = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d01c16dd-6253-47fb-b38c-56427415261d");
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (containerMetadata != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (containerMetadata), (object) containerMetadata);
      if (correlationId.HasValue)
        keyValuePairList.Add(nameof (correlationId), correlationId.Value.ToString());
      if (batchScanOptions != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (batchScanOptions), (object) batchScanOptions);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BatchScanResult>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ScanConfiguration> GetCurrentConfigurationAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ScanConfiguration>(new HttpMethod("GET"), new Guid("dae80a3c-e76a-4e57-b4df-5c400143be13"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ScanResult> ScanStreamAsync(
      Stream uploadStream,
      string instanceId = null,
      string resourceName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c8fd2dc0-053c-40c1-8e07-fe333146b255");
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (instanceId != null)
        collection.Add(nameof (instanceId), instanceId);
      if (resourceName != null)
        collection.Add(nameof (resourceName), resourceName);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ScanResult>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
