// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.DataImportOrchestrationHttpClient
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [ResourceArea("{6C0ED9B4-E87E-4F1D-976A-F3A05794EBE2}")]
  public class DataImportOrchestrationHttpClient : VssHttpClientBase
  {
    public DataImportOrchestrationHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public DataImportOrchestrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public DataImportOrchestrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public DataImportOrchestrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public DataImportOrchestrationHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task CancelDataImportJobAsync(
      Guid requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("85a4ee77-d3ba-4d87-9851-fe22a8c6c636"), (object) new
      {
        requestId = requestId
      }, new ApiResourceVersion(7.2, 4), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<DataImportRequestStatus> GetDataImportRequestStatusAsync(
      Guid requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<DataImportRequestStatus>(new HttpMethod("GET"), new Guid("85a4ee77-d3ba-4d87-9851-fe22a8c6c636"), (object) new
      {
        requestId = requestId
      }, new ApiResourceVersion(7.2, 4), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<bool> QueueRequestAsync(
      FrameworkDataImportRequest request,
      bool validateOnly,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("85a4ee77-d3ba-4d87-9851-fe22a8c6c636");
      HttpContent httpContent = (HttpContent) new ObjectContent<FrameworkDataImportRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (validateOnly), validateOnly.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<bool>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
