// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.Client.ProfileHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Profile.Client
{
  [ResourceArea("8CCFEF3D-2B87-4E99-8CCB-66E343D2DAA8")]
  public abstract class ProfileHttpClientBase : VssHttpClientBase
  {
    private static readonly Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>()
    {
      {
        "BadProfileRequestException",
        typeof (BadProfileRequestException)
      },
      {
        "BadPublicAliasException",
        typeof (BadPublicAliasException)
      },
      {
        "BadDisplayNameException",
        typeof (BadDisplayNameException)
      },
      {
        "BadCountryNameException",
        typeof (BadCountryNameException)
      },
      {
        "BadEmailAddressException",
        typeof (BadEmailAddressException)
      },
      {
        "BadAvatarValueException",
        typeof (BadAvatarValueException)
      },
      {
        "BadAttributeValueException",
        typeof (BadAttributeValueException)
      },
      {
        "BadServiceSettingNameException",
        typeof (BadServiceSettingNameException)
      },
      {
        "ProfileServiceSecurityException",
        typeof (ProfileServiceSecurityException)
      },
      {
        "ProfileNotAuthorizedException",
        typeof (ProfileNotAuthorizedException)
      },
      {
        "ProfileResourceNotFoundException",
        typeof (ProfileResourceNotFoundException)
      },
      {
        "ProfileAttributeNotFoundException",
        typeof (ProfileAttributeNotFoundException)
      },
      {
        "ProfileDoesNotExistException",
        typeof (ProfileDoesNotExistException)
      },
      {
        "ServiceSettingNotFoundException",
        typeof (ServiceSettingNotFoundException)
      },
      {
        "NewerVersionOfResourceExistsException",
        typeof (NewerVersionOfResourceExistsException)
      },
      {
        "NewerVersionOfProfileExists",
        typeof (NewerVersionOfProfileExists)
      },
      {
        "ProfileAlreadyExistsException",
        typeof (ProfileAlreadyExistsException)
      },
      {
        "PublicAliasAlreadyExistException",
        typeof (PublicAliasAlreadyExistException)
      },
      {
        "AttributeValueTooBigException",
        typeof (AttributeValueTooBigException)
      },
      {
        "AvatarTooBigException",
        typeof (AvatarTooBigException)
      },
      {
        "ServiceOperationNotAvailableException",
        typeof (ServiceOperationNotAvailableException)
      }
    };
    protected static readonly Version previewApiVersion = new Version(5, 0);

    protected ProfileHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ProfileHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ProfileHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ProfileHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ProfileHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    protected virtual async Task<ProfileAttribute> GetAttributeAsync(
      AttributeDescriptor descriptor,
      string id = "me",
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ProfileHttpClientBase profileHttpClientBase = this;
      ProfileHttpClientBase.ValidateId(id);
      ProfileHttpClientBase.ValidateAttributeDescriptor(descriptor);
      try
      {
        return await profileHttpClientBase.SendAsync<ProfileAttribute>(HttpMethod.Get, ProfileResourceIds.AttributeLocationid, (object) new
        {
          parentresource = "Profiles",
          id = id,
          descriptor = descriptor.ToString()
        }, new ApiResourceVersion(ProfileHttpClientBase.previewApiVersion, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      catch (ProfileAttributeNotFoundException ex)
      {
        return (ProfileAttribute) null;
      }
    }

    protected virtual async Task<int> SetAttributeAsync(
      ProfileAttribute attribute,
      string id = "me",
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ProfileHttpClientBase profileHttpClientBase1 = this;
      ProfileHttpClientBase.ValidateId(id);
      ProfileHttpClientBase.ValidateAttribute<string>((ProfileAttributeBase<string>) attribute);
      var data = new{ value = attribute.Value };
      ObjectContent objectContent = new ObjectContent(data.GetType(), (object) data, profileHttpClientBase1.Formatter);
      ProfileHttpClientBase profileHttpClientBase2 = profileHttpClientBase1;
      HttpMethod put = HttpMethod.Put;
      Guid attributeLocationid = ProfileResourceIds.AttributeLocationid;
      var routeValues = new
      {
        parentresource = "Profiles",
        id = id,
        descriptor = attribute.Descriptor.ToString()
      };
      HttpContent httpContent = (HttpContent) objectContent;
      ApiResourceVersion version = new ApiResourceVersion(ProfileHttpClientBase.previewApiVersion, 1);
      HttpContent content = httpContent;
      object userState1 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpRequestMessage message = await profileHttpClientBase2.CreateRequestMessageAsync(put, attributeLocationid, (object) routeValues, version, content, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      ProfileHttpClientBase.SetIfUnmodifiedSinceHeaders((ITimeStamped) attribute, message);
      ProfileHttpClientBase.SetIfMatchHeaders((IVersioned) attribute, message);
      return ProfileHttpClientBase.ExtractRevisionFromEtagInResponseHeader(await profileHttpClientBase1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false));
    }

    protected virtual async Task<int> DeleteAttributeAsync(
      ProfileAttribute attribute,
      string id = "me",
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ProfileHttpClientBase profileHttpClientBase = this;
      ProfileHttpClientBase.ValidateId(id);
      ProfileHttpClientBase.ValidateAttribute<string>((ProfileAttributeBase<string>) attribute);
      HttpRequestMessage message = await profileHttpClientBase.CreateRequestMessageAsync(HttpMethod.Delete, ProfileResourceIds.AttributeLocationid, (object) new
      {
        parentresource = "Profiles",
        id = id,
        descriptor = attribute.Descriptor.ToString()
      }, new ApiResourceVersion(ProfileHttpClientBase.previewApiVersion, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      ProfileHttpClientBase.SetIfUnmodifiedSinceHeaders((ITimeStamped) attribute, message);
      ProfileHttpClientBase.SetIfMatchHeaders((IVersioned) attribute, message);
      return ProfileHttpClientBase.ExtractRevisionFromEtagInResponseHeader(await profileHttpClientBase.SendAsync(message, userState, cancellationToken).ConfigureAwait(false));
    }

    protected virtual async Task<Microsoft.VisualStudio.Services.Profile.Profile> GetProfileAsync(
      ProfileQueryContext profileQueryContext,
      string id = "me",
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ProfileHttpClientBase profileHttpClientBase1 = this;
      ProfileHttpClientBase.ValidateId(id);
      ArgumentUtility.CheckForNull<ProfileQueryContext>(profileQueryContext, "attributesQueryContext");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add("details", "true");
      collection.Add("coreattributes", ProfileHttpClientBase.ConvertCoreAttributesFlagsToCommaDelimitedString(profileQueryContext.CoreAttributes));
      ProfileHttpClientBase profileHttpClientBase2 = profileHttpClientBase1;
      HttpMethod get = HttpMethod.Get;
      Guid profileLocationid = ProfileResourceIds.ProfileLocationid;
      var routeValues = new{ id = id };
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      ApiResourceVersion version = new ApiResourceVersion(ProfileHttpClientBase.previewApiVersion, 3);
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      Microsoft.VisualStudio.Services.Profile.Profile profileAsync = await profileHttpClientBase2.SendAsync<Microsoft.VisualStudio.Services.Profile.Profile>(get, profileLocationid, (object) routeValues, version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      foreach (KeyValuePair<string, CoreProfileAttribute> coreAttribute in (IEnumerable<KeyValuePair<string, CoreProfileAttribute>>) profileAsync.CoreAttributes)
        coreAttribute.Value.Value = ProfileUtility.GetCorrectlyTypedCoreAttribute(coreAttribute.Value.Descriptor.AttributeName, coreAttribute.Value.Value);
      return profileAsync;
    }

    protected async Task<int> UpdateProfileAsync(
      Microsoft.VisualStudio.Services.Profile.Profile profile,
      string id = "me",
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ProfileHttpClientBase profileHttpClientBase = this;
      ProfileHttpClientBase.ValidateId(id);
      ProfileHttpClientBase.ValidateProfile(profile);
      ObjectContent<Microsoft.VisualStudio.Services.Profile.Profile> content = new ObjectContent<Microsoft.VisualStudio.Services.Profile.Profile>(profile, profileHttpClientBase.Formatter);
      return ProfileHttpClientBase.ExtractRevisionFromEtagInResponseHeader(await profileHttpClientBase.SendAsync(new HttpMethod("PATCH"), ProfileResourceIds.ProfileLocationid, (object) new
      {
        id = id
      }, new ApiResourceVersion(ProfileHttpClientBase.previewApiVersion, 1), (HttpContent) content, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false));
    }

    protected virtual Task<Avatar> GetAvatarAsync(
      AvatarSize size = AvatarSize.Medium,
      string id = "me",
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ProfileHttpClientBase.ValidateId(id);
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>(nameof (size), size.ToString())
      };
      HttpMethod get = HttpMethod.Get;
      Guid avatarLocationid = ProfileResourceIds.AvatarLocationid;
      var routeValues = new
      {
        parentresource = "Profiles",
        id = id
      };
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      ApiResourceVersion version = new ApiResourceVersion(ProfileHttpClientBase.previewApiVersion, 1);
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      return this.SendAsync<Avatar>(get, avatarLocationid, (object) routeValues, version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1);
    }

    protected virtual async Task<Avatar> GetAvatarAsync(
      Avatar avatar,
      string id = "me",
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ProfileHttpClientBase profileHttpClientBase1 = this;
      ProfileHttpClientBase.ValidateId(id);
      ArgumentUtility.CheckForNull<Avatar>(avatar, nameof (avatar));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("size", avatar.Size.ToString())
      };
      ProfileHttpClientBase profileHttpClientBase2 = profileHttpClientBase1;
      HttpMethod get = HttpMethod.Get;
      Guid avatarLocationid = ProfileResourceIds.AvatarLocationid;
      var routeValues = new
      {
        parentresource = "Profiles",
        id = id
      };
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      ApiResourceVersion version = new ApiResourceVersion(ProfileHttpClientBase.previewApiVersion, 1);
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpRequestMessage message = await profileHttpClientBase2.CreateRequestMessageAsync(get, avatarLocationid, (object) routeValues, version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      ProfileHttpClientBase.SetIfModifiedSinceHeaders((ITimeStamped) avatar, message);
      try
      {
        return await profileHttpClientBase1.SendAsync<Avatar>(message, userState, cancellationToken).ConfigureAwait(false);
      }
      catch (VssServiceResponseException ex)
      {
        if (ex.HttpStatusCode == HttpStatusCode.NotModified)
          return avatar;
        throw;
      }
    }

    protected virtual async Task<int> SetAvatarAsync(
      Avatar avatar,
      string id = "me",
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ProfileHttpClientBase profileHttpClientBase1 = this;
      ProfileHttpClientBase.ValidateId(id);
      ArgumentUtility.CheckForNull<Avatar>(avatar, nameof (avatar));
      var data = new{ value = avatar.Value };
      ObjectContent objectContent = new ObjectContent(data.GetType(), (object) data, profileHttpClientBase1.Formatter);
      ProfileHttpClientBase profileHttpClientBase2 = profileHttpClientBase1;
      HttpMethod put = HttpMethod.Put;
      Guid avatarLocationid = ProfileResourceIds.AvatarLocationid;
      var routeValues = new
      {
        parentresource = "Profiles",
        id = id
      };
      HttpContent httpContent = (HttpContent) objectContent;
      ApiResourceVersion version = new ApiResourceVersion(ProfileHttpClientBase.previewApiVersion, 1);
      HttpContent content = httpContent;
      object userState1 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpRequestMessage message = await profileHttpClientBase2.CreateRequestMessageAsync(put, avatarLocationid, (object) routeValues, version, content, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      ProfileHttpClientBase.SetIfUnmodifiedSinceHeaders((ITimeStamped) avatar, message);
      return ProfileHttpClientBase.ExtractRevisionFromEtagInResponseHeader(await profileHttpClientBase1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false));
    }

    protected virtual async Task<int> ResetAvatarAsync(
      string id = "me",
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ProfileHttpClientBase profileHttpClientBase = this;
      ProfileHttpClientBase.ValidateId(id);
      HttpRequestMessage message = await profileHttpClientBase.CreateRequestMessageAsync(HttpMethod.Delete, ProfileResourceIds.AvatarLocationid, (object) new
      {
        parentresource = "Profiles",
        id = id
      }, new ApiResourceVersion(ProfileHttpClientBase.previewApiVersion, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return ProfileHttpClientBase.ExtractRevisionFromEtagInResponseHeader(await profileHttpClientBase.SendAsync(message, userState, cancellationToken).ConfigureAwait(false));
    }

    internal virtual async Task<IList<ProfileAttributeBase<object>>> GetAttributesInternalAsync(
      AttributesQueryContext attributesQueryContext,
      string id = "me",
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ProfileHttpClientBase profileHttpClientBase1 = this;
      ProfileHttpClientBase.ValidateId(id);
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      switch (attributesQueryContext.Scope)
      {
        case AttributesScope.Core:
          collection.Add("partition", "Core");
          break;
        case AttributesScope.Application:
          collection.Add("partition", attributesQueryContext.ContainerName);
          break;
        case AttributesScope.Core | AttributesScope.Application:
          collection.Add("partition", attributesQueryContext.ContainerName);
          collection.Add("withcoreattributes", "true");
          if (attributesQueryContext.CoreAttributes.HasValue)
          {
            collection.Add("coreattributes", ProfileHttpClientBase.ConvertCoreAttributesFlagsToCommaDelimitedString(attributesQueryContext.CoreAttributes.Value));
            break;
          }
          break;
      }
      if (attributesQueryContext.ModifiedSince.HasValue)
        collection.Add("modifiedsince", attributesQueryContext.ModifiedSince.Value.UtcDateTime.ToString((IFormatProvider) CultureInfo.InvariantCulture) + " GMT");
      if (attributesQueryContext.ModifiedAfterRevision.HasValue)
        collection.Add("modifiedafterrevision", attributesQueryContext.ModifiedAfterRevision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      ProfileHttpClientBase profileHttpClientBase2 = profileHttpClientBase1;
      HttpMethod get = HttpMethod.Get;
      Guid attributeLocationid = ProfileResourceIds.AttributeLocationid;
      var routeValues = new
      {
        parentresource = "Profiles",
        id = id
      };
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      ApiResourceVersion version = new ApiResourceVersion(ProfileHttpClientBase.previewApiVersion, 2);
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      return await profileHttpClientBase2.SendAsync<IList<ProfileAttributeBase<object>>>(get, attributeLocationid, (object) routeValues, version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
    }

    protected virtual async Task<Tuple<IList<ProfileAttribute>, IList<CoreProfileAttribute>>> GetAttributesAsync(
      AttributesQueryContext attributesQueryContext,
      string id = "me",
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IList<ProfileAttributeBase<object>> attributes = await this.GetAttributesInternalAsync(attributesQueryContext, id, userState, cancellationToken).ConfigureAwait(false);
      List<ProfileAttribute> profileAttributeList1 = new List<ProfileAttribute>();
      List<CoreProfileAttribute> profileAttributeList2 = new List<CoreProfileAttribute>();
      Tuple<IList<ProfileAttribute>, IList<CoreProfileAttribute>> attributesAsync = new Tuple<IList<ProfileAttribute>, IList<CoreProfileAttribute>>((IList<ProfileAttribute>) profileAttributeList1, (IList<CoreProfileAttribute>) profileAttributeList2);
      if (attributes != null)
      {
        if (attributes.Count != 0)
        {
          try
          {
            ProfileUtility.ValidateAttributes<object>((IEnumerable<ProfileAttributeBase<object>>) attributes);
          }
          catch (ArgumentException ex)
          {
            throw new ProfileException("The list of received attributes failed validation.", (Exception) ex);
          }
          foreach (ProfileAttributeBase<object> attribute in (IEnumerable<ProfileAttributeBase<object>>) attributes)
          {
            if (VssStringComparer.AttributesDescriptor.Compare(attribute.Descriptor.ContainerName, "Core") == 0)
              profileAttributeList2.Add(ProfileUtility.ExtractCoreAttribute<object>(attribute));
            else
              profileAttributeList1.Add(ProfileUtility.ExtractApplicationAttribute(attribute));
          }
          return attributesAsync;
        }
      }
      return attributesAsync;
    }

    protected async Task<int> SetAttributesAsync(
      IList<ProfileAttribute> applicationAttributes,
      IList<CoreProfileAttribute> coreAttributes,
      string id = "me",
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ProfileHttpClientBase profileHttpClientBase1 = this;
      ProfileHttpClientBase.ValidateId(id);
      if ((applicationAttributes == null || applicationAttributes.Count == 0) && (coreAttributes == null || coreAttributes.Count == 0))
        throw new ArgumentException(string.Format("Either param '{0}' or '{1}' should be non-empty", (object) nameof (applicationAttributes), (object) nameof (coreAttributes)));
      List<object> objectList = new List<object>();
      if (applicationAttributes != null)
      {
        foreach (ProfileAttribute applicationAttribute in (IEnumerable<ProfileAttribute>) applicationAttributes)
        {
          ProfileHttpClientBase.ValidateAttribute<string>((ProfileAttributeBase<string>) applicationAttribute);
          objectList.Add((object) applicationAttribute);
        }
      }
      if (coreAttributes != null)
      {
        foreach (CoreProfileAttribute coreAttribute in (IEnumerable<CoreProfileAttribute>) coreAttributes)
        {
          ProfileHttpClientBase.ValidateAttribute<object>((ProfileAttributeBase<object>) coreAttribute);
          objectList.Add((object) coreAttribute);
        }
      }
      ObjectContent objectContent = new ObjectContent(objectList.GetType(), (object) objectList, profileHttpClientBase1.Formatter);
      ProfileHttpClientBase profileHttpClientBase2 = profileHttpClientBase1;
      HttpMethod method = new HttpMethod("PATCH");
      Guid attributeLocationid = ProfileResourceIds.AttributeLocationid;
      var routeValues = new
      {
        parentresource = "Profiles",
        id = id
      };
      HttpContent httpContent = (HttpContent) objectContent;
      ApiResourceVersion version = new ApiResourceVersion(ProfileHttpClientBase.previewApiVersion, 1);
      HttpContent content = httpContent;
      object userState1 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpRequestMessage message = await profileHttpClientBase2.CreateRequestMessageAsync(method, attributeLocationid, (object) routeValues, version, content, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      return ProfileHttpClientBase.ExtractRevisionFromEtagInResponseHeader(await profileHttpClientBase1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false));
    }

    protected virtual Task<string> GetProfileLocationsAsync(
      ProfilePageType profilePageType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>(nameof (profilePageType), profilePageType.ToString())
      };
      HttpMethod get = HttpMethod.Get;
      Guid locationsLocationid = ProfileResourceIds.LocationsLocationid;
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      ApiResourceVersion version = new ApiResourceVersion(ProfileHttpClientBase.previewApiVersion, 1);
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      return this.SendAsync<string>(get, locationsLocationid, version: version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1);
    }

    protected virtual async Task<string> GetServiceSettingAsync(
      string settingName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ProfileHttpClientBase profileHttpClientBase = this;
      ArgumentUtility.CheckStringLength(settingName, nameof (settingName), 100);
      return await (await profileHttpClientBase.SendAsync(HttpMethod.Get, ProfileResourceIds.SettingsLocationid, (object) new
      {
        settingName = settingName
      }, new ApiResourceVersion(ProfileHttpClientBase.previewApiVersion, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    protected virtual Task<Microsoft.VisualStudio.Services.Profile.Profile> CreateProfileAsync(
      CreateProfileContext createProfileContext,
      bool? autoCreate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (!autoCreate.HasValue || !autoCreate.Value)
        ProfileHttpClientBase.ValidateCreateProfileContext(createProfileContext);
      HttpMethod post = HttpMethod.Post;
      Guid profileLocationid = ProfileResourceIds.ProfileLocationid;
      HttpContent httpContent = (HttpContent) new ObjectContent<CreateProfileContext>(createProfileContext, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (autoCreate.HasValue)
        collection.Add(nameof (autoCreate), autoCreate.Value.ToString());
      HttpMethod method = post;
      Guid locationId = profileLocationid;
      ApiResourceVersion version = new ApiResourceVersion("3.0-preview.3");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.VisualStudio.Services.Profile.Profile>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    protected virtual Task<HttpResponseMessage> VerifyAndUpdatePreferredEmailAsync(
      VerifyPreferredEmailContext verifyPreferredEmailContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ProfileHttpClientBase.ValidateVerifyPreferredEmailContext(verifyPreferredEmailContext);
      ObjectContent<VerifyPreferredEmailContext> content = new ObjectContent<VerifyPreferredEmailContext>(verifyPreferredEmailContext, this.Formatter);
      return this.SendAsync<HttpResponseMessage>(HttpMethod.Post, ProfileResourceIds.PreferredEmailConfirmationLocationid, version: new ApiResourceVersion(ProfileHttpClientBase.previewApiVersion, 1), content: (HttpContent) content, userState: userState, cancellationToken: cancellationToken);
    }

    protected virtual Task<ProfileRegions> GetRegionsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ProfileRegions>(HttpMethod.Get, ProfileResourceIds.RegionsLocationId, version: new ApiResourceVersion("3.0-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    private static void ValidateId(string id)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(id, nameof (id));
      ArgumentUtility.CheckStringForInvalidCharacters(id, nameof (id));
    }

    private static void ValidateAttributeDescriptor(AttributeDescriptor descriptor) => ArgumentUtility.CheckForNull<AttributeDescriptor>(descriptor, nameof (descriptor));

    private static void ValidateAttribute<T>(ProfileAttributeBase<T> attribute)
    {
      ArgumentUtility.CheckForNull<ProfileAttributeBase<T>>(attribute, nameof (attribute));
      ProfileHttpClientBase.ValidateAttributeDescriptor(attribute.Descriptor);
    }

    private static void ValidateProfile(Microsoft.VisualStudio.Services.Profile.Profile profile) => ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Profile.Profile>(profile, nameof (profile));

    private static void ValidateCreateProfileContext(CreateProfileContext createProfileContext)
    {
      ArgumentUtility.CheckForNull<CreateProfileContext>(createProfileContext, nameof (createProfileContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(createProfileContext.DisplayName, "DisplayName");
      ArgumentUtility.CheckStringForInvalidCharacters(createProfileContext.DisplayName, "DisplayName", false);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(createProfileContext.CountryName, "CountryName");
      ArgumentUtility.CheckStringForInvalidCharacters(createProfileContext.CountryName, "CountryName", false);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(createProfileContext.EmailAddress, "EmailAddress");
      ArgumentUtility.CheckStringForInvalidCharacters(createProfileContext.EmailAddress, "EmailAddress", false);
    }

    private static void ValidateVerifyPreferredEmailContext(
      VerifyPreferredEmailContext verifyPreferredEmailContext)
    {
      ArgumentUtility.CheckForNull<VerifyPreferredEmailContext>(verifyPreferredEmailContext, nameof (verifyPreferredEmailContext));
      ArgumentUtility.CheckForEmptyGuid(verifyPreferredEmailContext.Id, "Id");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(verifyPreferredEmailContext.HashCode, "HashCode");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(verifyPreferredEmailContext.HashCode, "HashCode");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(verifyPreferredEmailContext.EmailAddress, "EmailAddress");
      ArgumentUtility.CheckStringForInvalidCharacters(verifyPreferredEmailContext.EmailAddress, "EmailAddress", false);
    }

    private static void SetIfModifiedSinceHeaders(
      ITimeStamped timeStampedResource,
      HttpRequestMessage message)
    {
      if (timeStampedResource == null || !(timeStampedResource.TimeStamp != new DateTimeOffset()))
        return;
      message.Headers.IfModifiedSince = new DateTimeOffset?(timeStampedResource.TimeStamp);
    }

    private static void SetIfNoneMatchHeaders(string etagContent, HttpRequestMessage message)
    {
      if (string.IsNullOrEmpty(etagContent))
        return;
      EntityTagHeaderValue entityTagHeaderValue = EntityTagHeaderValue.Parse("\"" + etagContent + "\"");
      message.Headers.IfNoneMatch.Add(entityTagHeaderValue);
    }

    private static void SetIfMatchHeaders(IVersioned versionedResource, HttpRequestMessage message)
    {
      if (versionedResource == null || versionedResource.Revision == 0)
        return;
      EntityTagHeaderValue entityTagHeaderValue = EntityTagHeaderValue.Parse("\"" + versionedResource.Revision.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "\"");
      message.Headers.IfMatch.Add(entityTagHeaderValue);
    }

    private static void SetIfUnmodifiedSinceHeaders(
      ITimeStamped timeStampedResource,
      HttpRequestMessage message)
    {
      if (timeStampedResource == null || !(timeStampedResource.TimeStamp != new DateTimeOffset()))
        return;
      message.Headers.IfUnmodifiedSince = new DateTimeOffset?(timeStampedResource.TimeStamp);
    }

    private static int ExtractRevisionFromEtagInResponseHeader(HttpResponseMessage response)
    {
      try
      {
        string tag = response?.Headers?.ETag?.Tag;
        if (tag == null)
          return -1;
        return Convert.ToInt32(tag.Trim('"'));
      }
      catch (Exception ex)
      {
        return -1;
      }
    }

    private static string ConvertCoreAttributesFlagsToCommaDelimitedString(
      CoreProfileAttributes coreAttributes)
    {
      if (coreAttributes.HasFlag((System.Enum) CoreProfileAttributes.All))
        System.Enum.GetName(typeof (CoreProfileAttributes), (object) CoreProfileAttributes.All);
      List<string> values = new List<string>();
      foreach (CoreProfileAttributes flag in System.Enum.GetValues(typeof (CoreProfileAttributes)))
      {
        if (coreAttributes.HasFlag((System.Enum) flag))
          values.Add(System.Enum.GetName(typeof (CoreProfileAttributes), (object) flag));
      }
      return string.Join(",", (IEnumerable<string>) values);
    }

    internal int NotificationDelay { get; set; }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) ProfileHttpClientBase.s_translatedExceptions;
  }
}
