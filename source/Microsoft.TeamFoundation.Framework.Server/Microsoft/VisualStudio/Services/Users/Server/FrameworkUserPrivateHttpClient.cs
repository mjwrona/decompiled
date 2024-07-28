// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.FrameworkUserPrivateHttpClient
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Users.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  [ResourceArea("970AA69F-E316-4D78-B7B0-B7137E47A22C")]
  [ClientCancellationTimeout(4)]
  [ClientCircuitBreakerSettings(2, 50, MaxConcurrentRequests = 40)]
  public class FrameworkUserPrivateHttpClient : UserPrivateHttpClientBase
  {
    private static readonly VssHttpRetryOptions s_aggressiveFailureRetryOptions = new VssHttpRetryOptions()
    {
      MinBackoff = TimeSpan.FromSeconds(1.0),
      MaxBackoff = TimeSpan.FromSeconds(3.0),
      MaxRetries = 1
    }.MakeReadonly();
    private static readonly VssHttpRetryOptions s_reliableRetryOptions = new VssHttpRetryOptions()
    {
      MinBackoff = TimeSpan.FromSeconds(4.0),
      MaxBackoff = TimeSpan.FromSeconds(5.0),
      MaxRetries = 3
    }.MakeReadonly();
    private static readonly TimeSpan s_writeTimeout = TimeSpan.FromSeconds(10.0);

    public FrameworkUserPrivateHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public FrameworkUserPrivateHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public virtual Task DeletePrivateAttributeAsync(
      SubjectDescriptor descriptor,
      string attributeName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.DeletePrivateAttributeAsync((string) descriptor, attributeName, userState, cancellationToken);
    }

    public virtual Task DeletePrivateAttributeAsync(
      Guid userId,
      string attributeName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.DeletePrivateAttributeAsync(userId.ToString("D"), attributeName, userState, cancellationToken);
    }

    public virtual Task<UserAttribute> GetPrivateAttributeAsync(
      SubjectDescriptor descriptor,
      string attributeName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetPrivateAttributeAsync((string) descriptor, attributeName, userState, cancellationToken);
    }

    public virtual Task<UserAttribute> GetPrivateAttributeAsync(
      Guid userId,
      string attributeName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetPrivateAttributeAsync(userId.ToString("D"), attributeName, userState, cancellationToken);
    }

    public virtual Task<List<UserAttribute>> QueryPrivateAttributesAsync(
      SubjectDescriptor descriptor,
      string queryPattern = null,
      DateTimeOffset? modifiedAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.QueryPrivateAttributesAsync((string) descriptor, queryPattern, modifiedAfter, userState, cancellationToken);
    }

    public virtual Task<List<UserAttribute>> QueryPrivateAttributesAsync(
      Guid userId,
      string queryPattern = null,
      DateTimeOffset? modifiedAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.QueryPrivateAttributesAsync(userId.ToString("D"), queryPattern, modifiedAfter, userState, cancellationToken);
    }

    public virtual Task<List<UserAttribute>> SetPrivateAttributesAsync(
      SubjectDescriptor descriptor,
      IEnumerable<SetUserAttributeParameters> attributeParametersList,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SetPrivateAttributesAsync((string) descriptor, attributeParametersList, userState, cancellationToken);
    }

    public virtual Task<List<UserAttribute>> SetPrivateAttributesAsync(
      Guid userId,
      IEnumerable<SetUserAttributeParameters> attributeParametersList,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SetPrivateAttributesAsync(userId.ToString("D"), attributeParametersList, userState, cancellationToken);
    }

    protected override async Task<HttpRequestMessage> CreateRequestMessageAsync(
      HttpMethod method,
      IEnumerable<KeyValuePair<string, string>> additionalHeaders,
      Guid locationId,
      object routeValues,
      ApiResourceVersion version,
      HttpContent content,
      IEnumerable<KeyValuePair<string, string>> queryParameters,
      object userState,
      CancellationToken cancellationToken,
      string mediaType)
    {
      HttpRequestMessage requestMessageAsync = await base.CreateRequestMessageAsync(method, additionalHeaders, locationId, routeValues, version, content, queryParameters, userState, cancellationToken, mediaType).ConfigureAwait(false);
      if (method == HttpMethod.Get)
      {
        requestMessageAsync.Properties["VssHttpRetryOptions"] = (object) FrameworkUserPrivateHttpClient.s_aggressiveFailureRetryOptions;
      }
      else
      {
        requestMessageAsync.Properties["VssHttpRetryOptions"] = (object) FrameworkUserPrivateHttpClient.s_reliableRetryOptions;
        requestMessageAsync.Properties["VssRequestTimeout"] = (object) FrameworkUserPrivateHttpClient.s_writeTimeout;
      }
      return requestMessageAsync;
    }
  }
}
