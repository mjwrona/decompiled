// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Client.UserHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Users.Client
{
  [ResourceArea("970AA69F-E316-4D78-B7B0-B7137E47A22C")]
  [ClientCircuitBreakerSettings(100, 80, MaxConcurrentRequests = 40)]
  public class UserHttpClient : UserHttpClientBase
  {
    public UserHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public UserHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public UserHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public UserHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public UserHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task DeleteSelfAttributeAsync(
      string attributeName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.DeleteAttributeAsync("me", attributeName, userState, cancellationToken);
    }

    public Task<UserAttribute> GetSelfAttributeAsync(
      string attributeName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckStringForNullOrEmpty(attributeName, nameof (attributeName));
      return this.GetAttributeAsync("me", attributeName, userState, cancellationToken);
    }

    public Task<UserAttributes> QuerySelfAttributesAsync(
      string continuationToken,
      string queryPattern = null,
      DateTimeOffset? modifiedAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.QueryAttributesAsync("me", continuationToken, queryPattern, modifiedAfter, userState, cancellationToken);
    }

    public Task<List<UserAttribute>> SetSelfAttributesAsync(
      IEnumerable<SetUserAttributeParameters> attributeParametersList,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SetAttributesAsync("me", attributeParametersList, userState, cancellationToken);
    }

    public Task<User> GetSelfAsync(
      object userState = null,
      bool? createIfNotExists = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetUserAsync("me", createIfNotExists, userState, cancellationToken);
    }

    public virtual Task<User> UpdateSelfAsync(
      UpdateUserParameters userParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.UpdateUserAsync("me", userParameters, userState, cancellationToken);
    }

    public Task<Avatar> GetSelfAvatarAsync(
      AvatarSize? size = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetAvatarAsync("me", size, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
