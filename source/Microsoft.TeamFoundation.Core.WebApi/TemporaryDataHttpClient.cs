// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.TemporaryDataHttpClient
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [ResourceArea("79134C72-4A58-4B42-976C-04E7115F32BF")]
  public class TemporaryDataHttpClient : VssHttpClientBase
  {
    public TemporaryDataHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TemporaryDataHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TemporaryDataHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TemporaryDataHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TemporaryDataHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<TemporaryDataCreatedDTO> CreateTemporaryDataAsync(
      TemporaryDataDTO requestBody,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b4b570ef-1775-4093-9218-afb7e4c8aef6");
      HttpContent httpContent = (HttpContent) new ObjectContent<TemporaryDataDTO>(requestBody, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TemporaryDataCreatedDTO>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<TemporaryDataCreatedDTO> CreateTemporaryDataAsync(
      TemporaryDataDTO requestBody,
      Guid id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("b4b570ef-1775-4093-9218-afb7e4c8aef6");
      object obj1 = (object) new{ id = id };
      HttpContent httpContent = (HttpContent) new ObjectContent<TemporaryDataDTO>(requestBody, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TemporaryDataCreatedDTO>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<TemporaryDataCreatedDTO> GetTemporaryDataAsync(
      Guid id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TemporaryDataCreatedDTO>(new HttpMethod("GET"), new Guid("b4b570ef-1775-4093-9218-afb7e4c8aef6"), (object) new
      {
        id = id
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
