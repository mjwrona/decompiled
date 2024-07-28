// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UserMapping.Client.UserMappingHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.UserMapping.Client
{
  [ResourceArea("C8C8FFD0-2ECF-484A-B7E8-A226955EE7C8")]
  public class UserMappingHttpClient : VssHttpClientBase
  {
    public UserMappingHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public UserMappingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public UserMappingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public UserMappingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public UserMappingHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Guid> GetSyncUserAccountMappingsStatusAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Guid>(new HttpMethod("GET"), new Guid("0ff3e6da-ba57-4f24-8572-f442e5b55ae9"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Guid> QueueSyncUserAccountMappingsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Guid>(new HttpMethod("PUT"), new Guid("0ff3e6da-ba57-4f24-8572-f442e5b55ae9"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task ActivateUserAccountMappingAsync(
      Guid userId,
      Guid accountId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      UserMappingHttpClient mappingHttpClient = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("0dbf02cc-5ec3-4250-a145-5beb580e0086");
      object routeValues = (object) new{ userId = userId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (accountId), accountId.ToString());
      using (await mappingHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task ActivateUserAccountMappingAsync(
      Guid userId,
      Guid accountId,
      UserType userType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      UserMappingHttpClient mappingHttpClient = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("0dbf02cc-5ec3-4250-a145-5beb580e0086");
      object routeValues = (object) new{ userId = userId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (accountId), accountId.ToString());
      keyValuePairList.Add(nameof (userType), userType.ToString());
      using (await mappingHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<List<Guid>> QueryAccountIdsAsync(
      string userId,
      UserType userType,
      bool? useEqualsCheckForUserTypeMatch = null,
      bool? includeDeletedAccounts = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0dbf02cc-5ec3-4250-a145-5beb580e0086");
      object routeValues = (object) new{ userId = userId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (userType), userType.ToString());
      bool flag;
      if (useEqualsCheckForUserTypeMatch.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = useEqualsCheckForUserTypeMatch.Value;
        string str = flag.ToString();
        collection.Add(nameof (useEqualsCheckForUserTypeMatch), str);
      }
      if (includeDeletedAccounts.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeletedAccounts.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeletedAccounts), str);
      }
      return this.SendAsync<List<Guid>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
