// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.NewDomainUrlOrchestrationHttpClient
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [ResourceArea("{45D1D290-B9A3-43F1-805E-9A6F61BC07B6}")]
  public class NewDomainUrlOrchestrationHttpClient : VssHttpClientBase
  {
    public NewDomainUrlOrchestrationHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public NewDomainUrlOrchestrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public NewDomainUrlOrchestrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public NewDomainUrlOrchestrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public NewDomainUrlOrchestrationHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<ServicingOrchestrationRequestStatus> GetStatusAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ServicingOrchestrationRequestStatus>(new HttpMethod("GET"), new Guid("6ef82534-4c2b-44be-8276-3f23238807bd"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task QueueMigrationAsync(
      bool codexDomainUrls,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NewDomainUrlOrchestrationHttpClient orchestrationHttpClient = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("6ef82534-4c2b-44be-8276-3f23238807bd");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (codexDomainUrls), codexDomainUrls.ToString());
      using (await orchestrationHttpClient.SendAsync(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }
  }
}
