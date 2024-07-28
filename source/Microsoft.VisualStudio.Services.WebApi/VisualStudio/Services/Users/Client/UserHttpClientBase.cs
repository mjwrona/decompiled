// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Client.UserHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Users.Client
{
  [ResourceArea("970AA69F-E316-4D78-B7B0-B7137E47A22C")]
  public abstract class UserHttpClientBase : UserCompatHttpClientBase
  {
    public UserHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public UserHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public UserHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public UserHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public UserHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    protected virtual async Task DeleteAttributeAsync(
      string descriptor,
      string attributeName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("ac77b682-1ef8-4277-afde-30af9b546004"), (object) new
      {
        descriptor = descriptor,
        attributeName = attributeName
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    protected virtual Task<UserAttribute> GetAttributeAsync(
      string descriptor,
      string attributeName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UserAttribute>(new HttpMethod("GET"), new Guid("ac77b682-1ef8-4277-afde-30af9b546004"), (object) new
      {
        descriptor = descriptor,
        attributeName = attributeName
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    protected virtual Task<UserAttributes> QueryAttributesAsync(
      string descriptor,
      string continuationToken = null,
      string queryPattern = null,
      DateTimeOffset? modifiedAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ac77b682-1ef8-4277-afde-30af9b546004");
      object routeValues = (object) new
      {
        descriptor = descriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (queryPattern != null)
        keyValuePairList.Add(nameof (queryPattern), queryPattern);
      if (modifiedAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (modifiedAfter), modifiedAfter.Value);
      return this.SendAsync<UserAttributes>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    protected virtual Task<List<UserAttribute>> SetAttributesAsync(
      string descriptor,
      IEnumerable<SetUserAttributeParameters> attributeParametersList,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ac77b682-1ef8-4277-afde-30af9b546004");
      object obj1 = (object) new{ descriptor = descriptor };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<SetUserAttributeParameters>>(attributeParametersList, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<UserAttribute>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    protected virtual async Task DeleteAvatarAsync(
      string descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("1c34cdf0-dd20-4370-a316-56ba776d75ce"), (object) new
      {
        descriptor = descriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    protected virtual Task<Avatar> GetAvatarAsync(
      string descriptor,
      AvatarSize? size = null,
      string format = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1c34cdf0-dd20-4370-a316-56ba776d75ce");
      object routeValues = (object) new
      {
        descriptor = descriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (size.HasValue)
        keyValuePairList.Add(nameof (size), size.Value.ToString());
      if (format != null)
        keyValuePairList.Add(nameof (format), format);
      return this.SendAsync<Avatar>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    protected virtual async Task SetAvatarAsync(
      string descriptor,
      Avatar avatar,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      UserHttpClientBase userHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("1c34cdf0-dd20-4370-a316-56ba776d75ce");
      object obj1 = (object) new{ descriptor = descriptor };
      HttpContent httpContent = (HttpContent) new ObjectContent<Avatar>(avatar, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      UserHttpClientBase userHttpClientBase2 = userHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await userHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    protected virtual Task<Avatar> CreateAvatarPreviewAsync(
      string descriptor,
      Avatar avatar,
      AvatarSize? size = null,
      string displayName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("aad154d3-750f-47e6-9898-dc3a2e7a1708");
      object obj1 = (object) new{ descriptor = descriptor };
      HttpContent httpContent = (HttpContent) new ObjectContent<Avatar>(avatar, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (size.HasValue)
        collection.Add(nameof (size), size.Value.ToString());
      if (displayName != null)
        collection.Add(nameof (displayName), displayName);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Avatar>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    protected virtual Task<SubjectDescriptor> GetDescriptorAsync(
      string descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<SubjectDescriptor>(new HttpMethod("GET"), new Guid("e338ed36-f702-44d3-8d18-9cba811d013a"), (object) new
      {
        descriptor = descriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task DisableUserProfileSyncAsync(
      SubjectDescriptor descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("PATCH"), new Guid("89cfcb2e-4780-4ace-82d7-b822be7e5579"), (object) new
      {
        descriptor = descriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task EnableUserProfileSyncAsync(
      SubjectDescriptor descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("PATCH"), new Guid("17cc5f5e-9cda-49ba-bc2f-78738986fbfe"), (object) new
      {
        descriptor = descriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    protected virtual async Task ConfirmMailAsync(
      string descriptor,
      MailConfirmationParameters confirmationParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      UserHttpClientBase userHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("fc213dcd-3a4e-4951-a2e2-7e3fed15706d");
      object obj1 = (object) new{ descriptor = descriptor };
      HttpContent httpContent = (HttpContent) new ObjectContent<MailConfirmationParameters>(confirmationParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      UserHttpClientBase userHttpClientBase2 = userHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await userHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    protected virtual Task<List<AccessedHost>> GetMostRecentlyAccessedHostsAsync(
      string descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<AccessedHost>>(new HttpMethod("GET"), new Guid("a72c0174-9db6-428d-8674-3e57ef050f3d"), (object) new
      {
        descriptor = descriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    protected virtual async Task UpdateMostRecentlyAccessedHostsAsync(
      IEnumerable<AccessedHostsParameters> parametersList,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      UserHttpClientBase userHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6c416d43-571a-454d-8350-df3e879cb33d");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<AccessedHostsParameters>>(parametersList, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      UserHttpClientBase userHttpClientBase2 = userHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await userHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    protected virtual Task<Guid> GetStorageKeyAsync(
      string descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Guid>(new HttpMethod("GET"), new Guid("c1d0bf9e-3220-44d9-b048-222ae15fc3e4"), (object) new
      {
        descriptor = descriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    protected virtual Task<User> GetUserDefaultsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<User>(new HttpMethod("GET"), new Guid("a9e65880-7489-4453-aa72-0f7896f0b434"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    protected virtual Task<User> CreateUserAsync(
      CreateUserParameters userParameters,
      bool? createLocal = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("61117502-a055-422c-9122-b56e6643ed02");
      HttpContent httpContent = (HttpContent) new ObjectContent<CreateUserParameters>(userParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (createLocal.HasValue)
        collection.Add(nameof (createLocal), createLocal.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<User>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    protected virtual async Task DeleteUserAsync(
      SubjectDescriptor descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("61117502-a055-422c-9122-b56e6643ed02"), (object) new
      {
        descriptor = descriptor
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<User> GetUserAsync(
      string descriptor,
      bool? createIfNotExists = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("61117502-a055-422c-9122-b56e6643ed02");
      object routeValues = (object) new
      {
        descriptor = descriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (createIfNotExists.HasValue)
        keyValuePairList.Add("X-VSS-FaultInUser", createIfNotExists.Value.ToString());
      return this.SendAsync<User>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, locationId, routeValues, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    protected virtual Task<User> UpdateUserAsync(
      string descriptor,
      UpdateUserParameters userParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("61117502-a055-422c-9122-b56e6643ed02");
      object obj1 = (object) new{ descriptor = descriptor };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateUserParameters>(userParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<User>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
