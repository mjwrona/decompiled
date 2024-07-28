// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.PoolProvider.Client.PoolProviderClient
// Assembly: Microsoft.Azure.Pipelines.PoolProvider.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4D55F5C7-EE6B-4E5B-8407-D17F3B35057D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.PoolProvider.Client.dll

using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.PoolProvider.Client
{
  public class PoolProviderClient : IPoolProviderClient, IDisposable
  {
    private HttpClient m_client;
    private Uri m_appendRequestMessageUrl;
    private Uri m_updateRequestUrl;

    public PoolProviderClient(
      string authToken,
      string appendRequestMessageUrl,
      string updateRequestUrl,
      IEnumerable<DelegatingHandler> delegatingHandlers = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(authToken, nameof (authToken));
      ArgumentUtility.CheckStringForNullOrEmpty(authToken, nameof (appendRequestMessageUrl));
      this.m_appendRequestMessageUrl = new Uri(appendRequestMessageUrl);
      this.m_updateRequestUrl = new Uri(updateRequestUrl);
      this.m_client = new HttpClient(HttpClientFactory.CreatePipeline((HttpMessageHandler) new VssHttpMessageHandler((VssCredentials) (FederatedCredential) new VssOAuthAccessTokenCredential(authToken), new VssHttpRequestSettings()), delegatingHandlers), true);
    }

    public async Task AddAgentCloudRequestMessageAsync(
      AgentRequestMessageVerbosity verbosity,
      string message,
      CancellationToken cancellationToken)
    {
      AgentRequestMessage agentRequestMessage = new AgentRequestMessage()
      {
        Verbosity = verbosity,
        Message = message
      };
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this.m_appendRequestMessageUrl))
      {
        request.Content = (HttpContent) new ObjectContent<AgentRequestMessage>(agentRequestMessage, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        using (HttpResponseMessage httpResponseMessage = await this.m_client.SendAsync(request, cancellationToken).ConfigureAwait(false))
        {
          if (!httpResponseMessage.IsSuccessStatusCode)
            throw new InvalidResponseCodeException(httpResponseMessage.StatusCode);
        }
      }
    }

    public async Task UpdateAgentCloudRequestAsync(
      AgentRequestProvisioningResult provisioningResult,
      CancellationToken cancellationToken)
    {
      using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), this.m_updateRequestUrl))
      {
        request.Content = (HttpContent) new ObjectContent<AgentRequestProvisioningResult>(provisioningResult, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        using (HttpResponseMessage httpResponseMessage = await this.m_client.SendAsync(request, cancellationToken).ConfigureAwait(false))
        {
          if (!httpResponseMessage.IsSuccessStatusCode)
            throw new InvalidResponseCodeException(httpResponseMessage.StatusCode);
        }
      }
    }

    public void Dispose()
    {
      if (this.m_client == null)
        return;
      this.m_client.Dispose();
      this.m_client = (HttpClient) null;
    }
  }
}
