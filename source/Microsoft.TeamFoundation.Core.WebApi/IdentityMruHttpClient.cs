// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.IdentityMruHttpClient
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [ResourceArea("79134C72-4A58-4B42-976C-04E7115F32BF")]
  public class IdentityMruHttpClient : VssHttpClientBase
  {
    public IdentityMruHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public IdentityMruHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public IdentityMruHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public IdentityMruHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public IdentityMruHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task CreateIdentityMruAsync(
      IdentityData mruData,
      string mruName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5ead0b70-2572-4697-97e9-f341069a783a");
      object obj1 = (object) new{ mruName = mruName };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityData>(mruData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("2.1-preview.2");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return (Task) this.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task DeleteIdentityMruAsync(
      IdentityData mruData,
      string mruName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return (Task) this.SendAsync(new HttpMethod("DELETE"), new Guid("5ead0b70-2572-4697-97e9-f341069a783a"), (object) new
      {
        mruName = mruName
      }, new ApiResourceVersion("2.1-preview.2"), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<IdentityRef>> GetIdentityMruAsync(
      string mruName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<IdentityRef>>(new HttpMethod("GET"), new Guid("5ead0b70-2572-4697-97e9-f341069a783a"), (object) new
      {
        mruName = mruName
      }, new ApiResourceVersion("2.1-preview.2"), userState: userState, cancellationToken: cancellationToken);
    }

    public Task UpdateIdentityMruAsync(
      IdentityData mruData,
      string mruName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("5ead0b70-2572-4697-97e9-f341069a783a");
      object obj1 = (object) new{ mruName = mruName };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityData>(mruData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("2.1-preview.2");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return (Task) this.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
