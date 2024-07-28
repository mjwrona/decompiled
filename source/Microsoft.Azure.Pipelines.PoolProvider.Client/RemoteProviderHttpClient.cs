// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.PoolProvider.Client.RemoteProviderHttpClient
// Assembly: Microsoft.Azure.Pipelines.PoolProvider.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4D55F5C7-EE6B-4E5B-8407-D17F3B35057D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.PoolProvider.Client.dll

using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.PoolProvider.Client
{
  public class RemoteProviderHttpClient : IDisposable
  {
    private HttpClient m_client;
    private VssJsonMediaTypeFormatter m_formatter;
    private Uri m_provisionAgentUrl;
    private Uri m_releaseAgentUrl;
    private Uri m_agentDefinitionsUrl;
    private Uri m_agentRequestStatusUrl;
    private Uri m_accountParallelismUrl;

    public RemoteProviderHttpClient(
      string provisionAgentUrl,
      string releaseAgentUrl,
      string getAgentDefinitionsUrl = null,
      string getAgentRequestStatusUrl = null,
      string getAccountParallelismUrl = null,
      HttpMessageHandler messageHandler = null,
      IEnumerable<DelegatingHandler> delegatingHandlers = null)
    {
      this.m_formatter = new VssJsonMediaTypeFormatter();
      this.m_provisionAgentUrl = new Uri(provisionAgentUrl);
      this.m_releaseAgentUrl = new Uri(releaseAgentUrl);
      this.m_agentDefinitionsUrl = getAgentDefinitionsUrl != null ? new Uri(getAgentDefinitionsUrl) : (Uri) null;
      this.m_agentRequestStatusUrl = getAgentRequestStatusUrl != null ? new Uri(getAgentRequestStatusUrl) : (Uri) null;
      this.m_accountParallelismUrl = getAccountParallelismUrl != null ? new Uri(getAccountParallelismUrl) : (Uri) null;
      if (messageHandler == null)
        messageHandler = (HttpMessageHandler) new VssHttpMessageHandler(new VssCredentials(), new VssHttpRequestSettings());
      HttpMessageHandler handler = messageHandler;
      if (delegatingHandlers != null)
        handler = HttpClientFactory.CreatePipeline(messageHandler, delegatingHandlers);
      this.m_client = new HttpClient(handler, true);
    }

    public async Task<AgentRequestResponse> ProvisionAgentAsync(
      AgentRequest request,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AgentRequestResponse agentRequestResponse;
      using (HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, this.m_provisionAgentUrl))
      {
        httpRequest.Content = (HttpContent) new ObjectContent<AgentRequest>(request, (MediaTypeFormatter) this.m_formatter);
        using (HttpResponseMessage response = await this.m_client.SendAsync(httpRequest, cancellationToken))
        {
          if (response.IsSuccessStatusCode)
          {
            agentRequestResponse = await response.Content.ReadAsAsync<AgentRequestResponse>((IEnumerable<MediaTypeFormatter>) new VssJsonMediaTypeFormatter[1]
            {
              this.m_formatter
            }, cancellationToken);
          }
          else
          {
            InvalidResponseCodeException responseCodeException = new InvalidResponseCodeException(response.StatusCode);
            responseCodeException.RetryAfter = response.StatusCode == (HttpStatusCode) 429 ? this.ParseRetryAfter(response) : throw responseCodeException;
          }
        }
      }
      return agentRequestResponse;
    }

    public async Task ReleaseAgentAsync(AgentIdentifier agent, CancellationToken cancellationToken = default (CancellationToken))
    {
      using (HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, this.m_releaseAgentUrl))
      {
        httpRequest.Content = (HttpContent) new ObjectContent<AgentIdentifier>(agent, (MediaTypeFormatter) this.m_formatter);
        using (HttpResponseMessage response = await this.m_client.SendAsync(httpRequest, cancellationToken))
        {
          if (!response.IsSuccessStatusCode)
          {
            InvalidResponseCodeException responseCodeException = new InvalidResponseCodeException(response.StatusCode);
            responseCodeException.RetryAfter = response.StatusCode == (HttpStatusCode) 429 ? this.ParseRetryAfter(response) : throw responseCodeException;
          }
        }
      }
    }

    public async Task<IList<AgentDefinition>> GetAgentDefinitionsAsync(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.m_agentDefinitionsUrl == (Uri) null)
        throw new EndpointNotSupportedException("GetAgentDefinitions");
      IList<AgentDefinition> definitionsAsync;
      using (HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, this.m_agentDefinitionsUrl))
      {
        using (HttpResponseMessage response = await this.m_client.SendAsync(httpRequest, cancellationToken))
        {
          if (!response.IsSuccessStatusCode)
            throw new InvalidResponseCodeException(response.StatusCode);
          definitionsAsync = (await response.Content.ReadAsAsync<VssJsonCollectionWrapper<IList<AgentDefinition>>>((IEnumerable<MediaTypeFormatter>) new VssJsonMediaTypeFormatter[1]
          {
            this.m_formatter
          }, cancellationToken)).Value;
        }
      }
      return definitionsAsync;
    }

    public async Task<AgentRequestStatus> GetAgentRequestStatusAsync(
      AgentIdentifier agent,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.m_agentRequestStatusUrl == (Uri) null)
        throw new EndpointNotSupportedException("GetAgentRequestStatus");
      AgentRequestStatus requestStatusAsync;
      using (HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, this.m_agentRequestStatusUrl))
      {
        httpRequest.Content = (HttpContent) new ObjectContent<AgentIdentifier>(agent, (MediaTypeFormatter) this.m_formatter);
        using (HttpResponseMessage response = await this.m_client.SendAsync(httpRequest, cancellationToken))
        {
          if (!response.IsSuccessStatusCode)
            throw new InvalidResponseCodeException(response.StatusCode);
          requestStatusAsync = await response.Content.ReadAsAsync<AgentRequestStatus>((IEnumerable<MediaTypeFormatter>) new VssJsonMediaTypeFormatter[1]
          {
            this.m_formatter
          }, cancellationToken);
        }
      }
      return requestStatusAsync;
    }

    public async Task<AccountParallelismResponse> GetAccountParallelismAsync(
      AccountParallelismRequest request,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.m_accountParallelismUrl == (Uri) null)
        throw new EndpointNotSupportedException("GetAccountParallelism");
      AccountParallelismResponse parallelismAsync;
      using (HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, this.m_accountParallelismUrl))
      {
        httpRequest.Content = (HttpContent) new ObjectContent<AccountParallelismRequest>(request, (MediaTypeFormatter) this.m_formatter);
        using (HttpResponseMessage response = await this.m_client.SendAsync(httpRequest, cancellationToken))
        {
          if (!response.IsSuccessStatusCode)
            throw new InvalidResponseCodeException(response.StatusCode);
          parallelismAsync = await response.Content.ReadAsAsync<AccountParallelismResponse>((IEnumerable<MediaTypeFormatter>) new VssJsonMediaTypeFormatter[1]
          {
            this.m_formatter
          }, cancellationToken);
        }
      }
      return parallelismAsync;
    }

    public void Dispose()
    {
      if (this.m_client == null)
        return;
      this.m_client.Dispose();
      this.m_client = (HttpClient) null;
    }

    private DateTime? ParseRetryAfter(HttpResponseMessage response)
    {
      RetryConditionHeaderValue retryAfter = response?.Headers?.RetryAfter;
      if (retryAfter == null)
        return new DateTime?();
      DateTimeOffset? date = retryAfter.Date;
      ref DateTimeOffset? local = ref date;
      return new DateTime?(local.HasValue ? local.GetValueOrDefault().DateTime : DateTime.UtcNow.Add(retryAfter.Delta.Value));
    }
  }
}
