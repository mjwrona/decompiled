// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.Client.ProfileHttpClient
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

namespace Microsoft.VisualStudio.Services.Profile.Client
{
  [ClientCircuitBreakerSettings(100, 80, MaxConcurrentRequests = 40)]
  public class ProfileHttpClient : ProfileHttpClientBase
  {
    public const int MaxAttributeValueLength = 1048576;

    public ProfileHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ProfileHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ProfileHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ProfileHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ProfileHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<int> SetAttributeAsync(
      ProfileAttribute newAttribute,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SetAttributeAsync(newAttribute, "me", userState, cancellationToken);
    }

    public virtual Task<ProfileAttribute> GetAttributeAsync(
      AttributeDescriptor descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetAttributeAsync(descriptor, "me", userState, cancellationToken);
    }

    public virtual Task<int> DeleteAttributeAsync(
      ProfileAttribute attributeToDelete,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.DeleteAttributeAsync(attributeToDelete, "me", userState, cancellationToken);
    }

    public virtual Task<Microsoft.VisualStudio.Services.Profile.Profile> GetProfileAsync(
      ProfileQueryContext profileQueryContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetProfileAsync(profileQueryContext, "me", userState, cancellationToken);
    }

    public virtual Task<int> UpdateProfileAsync(
      Microsoft.VisualStudio.Services.Profile.Profile profile,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.UpdateProfileAsync(profile, "me", userState, cancellationToken);
    }

    public virtual Task<Avatar> GetAvatarAsync(
      AvatarSize size = AvatarSize.Medium,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetAvatarAsync(size, "me", userState, cancellationToken);
    }

    public virtual Task<Avatar> GetAvatarAsync(
      Guid id,
      AvatarSize size = AvatarSize.Medium,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetAvatarAsync(size, id.ToString(), userState, cancellationToken);
    }

    public virtual Task<Avatar> GetAvatarAsync(
      Avatar currentCopy,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetAvatarAsync(currentCopy, "me", userState, cancellationToken);
    }

    public virtual Task<int> SetAvatarAsync(
      Avatar newAvatar,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SetAvatarAsync(newAvatar, "me", userState, cancellationToken);
    }

    public virtual Task<string> GetDisplayNameAsync(object userState = null) => this.GetDisplayNameImplAsync(userState);

    public virtual Task<string> GetDisplayNameAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetDisplayNameImplAsync(userState, cancellationToken);
    }

    private async Task<string> GetDisplayNameImplAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await (await this.SendAsync(HttpMethod.Get, ProfileResourceIds.DisplayNameLocationid, (object) new
      {
        parentresource = "Profiles",
        id = "me"
      }, new ApiResourceVersion(ProfileHttpClientBase.previewApiVersion, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    public virtual Task<Tuple<IList<ProfileAttribute>, IList<CoreProfileAttribute>>> GetAttributesAsync(
      AttributesQueryContext attributesQueryContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetAttributesAsync(attributesQueryContext, "me", userState, cancellationToken);
    }

    public virtual Task<int> SetAttributesAsync(
      IList<ProfileAttribute> applicationAttributes,
      IList<CoreProfileAttribute> coreAttributes,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SetAttributesAsync(applicationAttributes, coreAttributes, "me", userState, cancellationToken);
    }

    public new virtual Task<string> GetServiceSettingAsync(
      string settingName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return base.GetServiceSettingAsync(settingName, userState, cancellationToken);
    }

    public new virtual Task<string> GetProfileLocationsAsync(
      ProfilePageType profilePageType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return base.GetProfileLocationsAsync(profilePageType, userState, cancellationToken);
    }

    public new virtual Task<Microsoft.VisualStudio.Services.Profile.Profile> CreateProfileAsync(
      CreateProfileContext createProfileContext,
      bool? autoCreate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return base.CreateProfileAsync(createProfileContext, autoCreate, userState, cancellationToken);
    }
  }
}
