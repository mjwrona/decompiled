// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.FrameworkUserHttpClient
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
  public class FrameworkUserHttpClient : UserHttpClientBase
  {
    private static readonly VssHttpRetryOptions s_zeroRetryOptions = new VssHttpRetryOptions()
    {
      MaxRetries = 0
    }.MakeReadonly();
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
    private static readonly TimeSpan s_longWriteTimeout = TimeSpan.FromSeconds(180.0);

    public FrameworkUserHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public FrameworkUserHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public virtual Task DeleteAttributeAsync(
      SubjectDescriptor descriptor,
      string attributeName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.DeleteAttributeAsync((string) descriptor, attributeName, userState, cancellationToken);
    }

    public virtual Task DeleteAttributeAsync(
      Guid userId,
      string attributeName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.DeleteAttributeAsync(userId.ToString("D"), attributeName, userState, cancellationToken);
    }

    public virtual Task<UserAttribute> GetAttributeAsync(
      SubjectDescriptor descriptor,
      string attributeName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetAttributeAsync((string) descriptor, attributeName, userState, cancellationToken);
    }

    public virtual Task<UserAttribute> GetAttributeAsync(
      Guid userId,
      string attributeName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetAttributeAsync(userId.ToString("D"), attributeName, userState, cancellationToken);
    }

    public virtual Task<UserAttributes> QueryAttributesAsync(
      SubjectDescriptor descriptor,
      string continuationToken,
      string queryPattern = null,
      DateTimeOffset? modifiedAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.QueryAttributesAsync((string) descriptor, continuationToken, queryPattern, modifiedAfter, userState, cancellationToken);
    }

    public virtual Task<UserAttributes> QueryAttributesAsync(
      Guid userId,
      string continuationToken,
      string queryPattern = null,
      DateTimeOffset? modifiedAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.QueryAttributesAsync(userId.ToString("D"), continuationToken, queryPattern, modifiedAfter, userState, cancellationToken);
    }

    public virtual Task<List<UserAttribute>> SetAttributesAsync(
      SubjectDescriptor descriptor,
      IEnumerable<SetUserAttributeParameters> attributeParametersList,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SetAttributesAsync((string) descriptor, attributeParametersList, userState, cancellationToken);
    }

    public virtual Task<List<UserAttribute>> SetAttributesAsync(
      Guid userId,
      IEnumerable<SetUserAttributeParameters> attributeParametersList,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SetAttributesAsync(userId.ToString("D"), attributeParametersList, userState, cancellationToken);
    }

    public virtual Task DeleteAvatarAsync(
      SubjectDescriptor descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.DeleteAvatarAsync((string) descriptor, userState, cancellationToken);
    }

    public virtual Task DeleteAvatarAsync(
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.DeleteAvatarAsync(userId.ToString("D"), userState, cancellationToken);
    }

    public virtual Task<Avatar> GetAvatarAsync(
      SubjectDescriptor descriptor,
      AvatarSize? size = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetAvatarAsync((string) descriptor, size, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Avatar> GetAvatarAsync(
      Guid userId,
      AvatarSize? size = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetAvatarAsync(userId.ToString("D"), size, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task SetAvatarAsync(
      SubjectDescriptor descriptor,
      Avatar avatar,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SetAvatarAsync((string) descriptor, avatar, userState, cancellationToken);
    }

    public virtual Task SetAvatarAsync(
      Guid userId,
      Avatar avatar,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SetAvatarAsync(userId.ToString("D"), avatar, userState, cancellationToken);
    }

    public new Task<User> CreateUserAsync(
      CreateUserParameters userParameters,
      bool? createLocal = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return base.CreateUserAsync(userParameters, createLocal, userState, cancellationToken);
    }

    public new Task DeleteUserAsync(
      SubjectDescriptor descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return base.DeleteUserAsync(descriptor, userState, cancellationToken);
    }

    public virtual Task<User> GetUserAsync(
      SubjectDescriptor descriptor,
      bool? createIfNotExists = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetUserAsync((string) descriptor, createIfNotExists, userState, cancellationToken);
    }

    public virtual Task<User> GetUserAsync(
      Guid userId,
      bool? createIfNotExists = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetUserAsync(userId.ToString("D"), createIfNotExists, userState, cancellationToken);
    }

    public virtual Task<User> UpdateUserAsync(
      SubjectDescriptor descriptor,
      UpdateUserParameters userParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.UpdateUserAsync((string) descriptor, userParameters, userState, cancellationToken);
    }

    public virtual Task<User> UpdateUserAsync(
      Guid userId,
      UpdateUserParameters userParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.UpdateUserAsync(userId.ToString("D"), userParameters, userState, cancellationToken);
    }

    public virtual Task<SubjectDescriptor> GetDescriptorAsync(
      Guid storageKey,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetDescriptorAsync(storageKey.ToString("D"), userState, cancellationToken);
    }

    public virtual Task<Guid> GetStorageKeyAsync(
      SubjectDescriptor descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetStorageKeyAsync((string) descriptor, userState, cancellationToken);
    }

    public virtual Task ConfirmPreferredMailAsync(
      SubjectDescriptor descriptor,
      MailConfirmationParameters confirmationParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ConfirmMailAsync((string) descriptor, confirmationParameters, userState, cancellationToken);
    }

    public virtual Task ConfirmPreferredMailAsync(
      Guid userId,
      MailConfirmationParameters confirmationParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ConfirmMailAsync(userId.ToString("D"), confirmationParameters, userState, cancellationToken);
    }

    public new Task<User> GetUserDefaultsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return base.GetUserDefaultsAsync(userState, cancellationToken);
    }

    public virtual Task<Avatar> CreateAvatarPreviewAsync(
      SubjectDescriptor descriptor,
      Avatar avatar,
      AvatarSize? size = null,
      string displayName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.CreateAvatarPreviewAsync((string) descriptor, avatar, size, displayName, userState, cancellationToken);
    }

    public virtual Task<Avatar> CreateAvatarPreviewAsync(
      Guid userId,
      Avatar avatar,
      AvatarSize? size = null,
      string displayName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.CreateAvatarPreviewAsync(userId.ToString("D"), avatar, size, displayName, userState, cancellationToken);
    }

    public virtual Task<List<AccessedHost>> GetMostRecentlyAccessedHostsAsync(
      SubjectDescriptor descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetMostRecentlyAccessedHostsAsync((string) descriptor, userState, cancellationToken);
    }

    public new Task UpdateMostRecentlyAccessedHostsAsync(
      IEnumerable<AccessedHostsParameters> parametersList,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return base.UpdateMostRecentlyAccessedHostsAsync(parametersList, userState, cancellationToken);
    }

    public new Task EnableUserProfileSyncAsync(
      SubjectDescriptor descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return base.EnableUserProfileSyncAsync(descriptor, userState, cancellationToken);
    }

    public new Task DisableUserProfileSyncAsync(
      SubjectDescriptor descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return base.DisableUserProfileSyncAsync(descriptor, userState, cancellationToken);
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
        requestMessageAsync.Properties["VssHttpRetryOptions"] = !(locationId == UserResourceIds.Avatar) ? (object) FrameworkUserHttpClient.s_aggressiveFailureRetryOptions : (object) FrameworkUserHttpClient.s_zeroRetryOptions;
      }
      else
      {
        requestMessageAsync.Properties["VssHttpRetryOptions"] = (object) FrameworkUserHttpClient.s_reliableRetryOptions;
        requestMessageAsync.Properties["VssRequestTimeout"] = !(locationId == UserResourceIds.RecentlyAccessedHosts) ? (object) FrameworkUserHttpClient.s_writeTimeout : (object) FrameworkUserHttpClient.s_longWriteTimeout;
      }
      return requestMessageAsync;
    }
  }
}
