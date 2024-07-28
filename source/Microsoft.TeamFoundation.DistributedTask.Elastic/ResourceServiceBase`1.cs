// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ResourceServiceBase`1
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Azure;
using Microsoft.TeamFoundation.DistributedTask.AzureSdk;
using Microsoft.TeamFoundation.DistributedTask.AzureSdk.Tracing;
using Microsoft.TeamFoundation.DistributedTask.Elastic.Tracing;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  internal abstract class ResourceServiceBase<T> where T : ICanExpire
  {
    private const string m_authorizationUrlTemplate = "https://login.microsoftonline.com/{0}/oauth2/token";
    private T m_client;
    private VssHttpRequestSettings m_httpRequestSettings;
    private Guid m_serviceEndpointId;
    private Guid m_serviceEndpointScope;
    private ElasticPoolTracingInterceptor m_interceptor;
    private object m_lock = new object();
    private static readonly VssHttpRetryOptions s_retryOptions = new VssHttpRetryOptions()
    {
      MaxRetries = 5
    };

    protected T Client(
      IVssRequestContext requestContext,
      Guid serviceEndpointId,
      Guid serviceEndpointScope,
      string traceLayer,
      string clientName)
    {
      ref T local1 = ref this.m_client;
      int num;
      if ((object) default (T) == null)
      {
        T obj = local1;
        ref T local2 = ref obj;
        if ((object) obj == null)
        {
          num = 0;
          goto label_4;
        }
        else
          local1 = ref local2;
      }
      num = local1.IsExpired ? 1 : 0;
label_4:
      if (num != 0)
        Microsoft.TeamFoundation.DistributedTask.Server.RequestContextExtensions.TraceInfo(requestContext, traceLayer, "Cached instance of {0} was marked as expired, creating a new instance", (object) clientName);
      if (requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolLockAzureClients"))
      {
        lock (this.m_lock)
          this.InitializeClient(requestContext, serviceEndpointId, serviceEndpointScope);
      }
      else
        this.InitializeClient(requestContext, serviceEndpointId, serviceEndpointScope);
      return this.m_client;
    }

    protected static Uri GetAzureResourceManagerUrl() => AzureHttpClientBase.DefaultManagementUrl;

    protected HttpMessageHandler GetMessageHandler(IVssRequestContext requestContext, Uri resource)
    {
      ServiceEndpoint serviceEndpoint = requestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(requestContext.Elevate(), this.m_serviceEndpointScope, this.m_serviceEndpointId);
      if (serviceEndpoint == null)
        throw new ServiceEndpointDoesNotExistException(this.m_serviceEndpointId, this.m_serviceEndpointScope);
      if (serviceEndpoint.IsDisabled && requestContext.IsFeatureEnabled("ServiceEndpoints.AllowDisablingServiceEndpoints"))
        throw new ServiceEndpointDoesNotExistException(ElasticResources.ServiceEndpointDisabled((object) this.m_serviceEndpointId));
      string str;
      if (!serviceEndpoint.Authorization.Parameters.TryGetValue("TenantId", out str) || string.IsNullOrEmpty(str))
        throw new InvalidServiceEndpointException(this.m_serviceEndpointId, this.m_serviceEndpointScope);
      VssOAuthClientCredential clientCredential = this.CreateClientCredential(requestContext, serviceEndpoint);
      VssOAuthTokenParameters tokenParameters = new VssOAuthTokenParameters()
      {
        Resource = resource.ToString()
      };
      return HttpClientFactory.CreatePipeline((HttpMessageHandler) new VssHttpMessageHandler(new VssCredentials((FederatedCredential) new VssOAuthCredential(new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}/oauth2/token", (object) str)), (VssOAuthGrant) VssOAuthGrant.ClientCredentials, clientCredential, tokenParameters)), this.HttpRequestSettings), (IEnumerable<DelegatingHandler>) new List<DelegatingHandler>()
      {
        (DelegatingHandler) new VssHttpRetryMessageHandler(ResourceServiceBase<T>.s_retryOptions)
      });
    }

    private void InitializeClient(
      IVssRequestContext requestContext,
      Guid serviceEndpointId,
      Guid serviceEndpointScope)
    {
      if ((object) this.m_client != null && !this.m_client.IsExpired && !(this.m_serviceEndpointId != serviceEndpointId) && !(this.m_serviceEndpointScope != serviceEndpointScope))
        return;
      this.m_serviceEndpointId = serviceEndpointId;
      this.m_serviceEndpointScope = serviceEndpointScope;
      this.ClientFactory(requestContext, out this.m_client);
    }

    private VssOAuthClientCredential CreateClientCredential(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint)
    {
      string clientId;
      if (!serviceEndpoint.Authorization.Parameters.TryGetValue("ServicePrincipalId", out clientId) || string.IsNullOrEmpty(clientId))
        throw new InvalidServiceEndpointException(this.m_serviceEndpointId, this.m_serviceEndpointScope);
      if (serviceEndpoint.Authorization.Scheme == "WorkloadIdentityFederation")
      {
        OidcFederationClaims federationClaims = OidcFederationClaims.CreateOidcFederationClaims(requestContext, serviceEndpoint);
        try
        {
          JsonWebToken bearerToken = JsonWebToken.Create(SessionTokenGenerator.GenerateOidcToken(requestContext.Elevate(), this.m_serviceEndpointScope, (IOidcFederationClaims) federationClaims, TimeSpan.FromHours(1.0)).Token);
          return (VssOAuthClientCredential) new VssOAuthWorkloadIdentityFederationCredential(clientId, new VssOAuthJwtBearerAssertion(bearerToken));
        }
        catch (Exception ex)
        {
          Microsoft.TeamFoundation.DistributedTask.Server.RequestContextExtensions.TraceException(requestContext, 10015274, nameof (ResourceServiceBase<T>), ex);
          throw;
        }
      }
      else
      {
        string clientSecret;
        if (serviceEndpoint.Authorization.Scheme == "ServicePrincipal" && serviceEndpoint.Authorization.Parameters.TryGetValue("ServicePrincipalKey", out clientSecret) && !string.IsNullOrEmpty(clientSecret))
          return (VssOAuthClientCredential) new VssOAuthPasswordClientCredential(clientId, clientSecret);
        throw new InvalidServiceEndpointException(this.m_serviceEndpointId, this.m_serviceEndpointScope);
      }
    }

    protected void EnableTracing(IVssRequestContext systemRequestContext)
    {
      try
      {
        if (!systemRequestContext.IsFeatureEnabled("DistributedTask.ElasticPoolTurnOnAzureClientTracing"))
          return;
        AzureClientTracing.IsEnabled = true;
        this.m_interceptor = new ElasticPoolTracingInterceptor();
        AzureClientTracing.RemoveTracingInterceptor((IAzureClientTracingInterceptor) this.m_interceptor);
        AzureClientTracing.AddTracingInterceptor((IAzureClientTracingInterceptor) this.m_interceptor);
      }
      catch (Exception ex)
      {
        Microsoft.TeamFoundation.DistributedTask.Server.RequestContextExtensions.TraceException(systemRequestContext, 10015274, nameof (ResourceServiceBase<T>), ex);
      }
    }

    protected void DisableTracing(IVssRequestContext systemRequestContext)
    {
      try
      {
        if (!AzureClientTracing.IsEnabled || this.m_interceptor == null)
          return;
        AzureClientTracing.RemoveTracingInterceptor((IAzureClientTracingInterceptor) this.m_interceptor);
        AzureClientTracing.IsEnabled = AzureClientTracing.AnyActiveInterceptors();
      }
      catch (Exception ex)
      {
        Microsoft.TeamFoundation.DistributedTask.Server.RequestContextExtensions.TraceException(systemRequestContext, 10015274, nameof (ResourceServiceBase<T>), ex);
      }
    }

    protected abstract void ClientFactory(IVssRequestContext requestContext, out T client);

    private VssHttpRequestSettings HttpRequestSettings => this.m_httpRequestSettings ?? (this.m_httpRequestSettings = new VssHttpRequestSettings());
  }
}
