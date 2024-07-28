// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UserAccountMapping.Client.UserAccountMappingHttpClientBase
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

namespace Microsoft.VisualStudio.Services.UserAccountMapping.Client
{
  [ResourceArea("B2F5FAA8-CAAF-436F-B40C-FC45778E174D")]
  public abstract class UserAccountMappingHttpClientBase : VssHttpClientBase
  {
    public UserAccountMappingHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public UserAccountMappingHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public UserAccountMappingHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public UserAccountMappingHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public UserAccountMappingHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    protected virtual async Task ActivateUserAccountMappingAsync(
      string descriptor,
      Guid accountId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      UserAccountMappingHttpClientBase mappingHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("ce4b4a54-f365-4fcc-b623-4a3f8e7bc07c");
      object routeValues = (object) new
      {
        descriptor = descriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (accountId), accountId.ToString());
      using (await mappingHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    protected virtual async Task ActivateUserAccountMappingAsync(
      string descriptor,
      Guid accountId,
      UserRole userRole,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      UserAccountMappingHttpClientBase mappingHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("ce4b4a54-f365-4fcc-b623-4a3f8e7bc07c");
      object routeValues = (object) new
      {
        descriptor = descriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (accountId), accountId.ToString());
      keyValuePairList.Add(nameof (userRole), userRole.ToString());
      using (await mappingHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    protected virtual async Task DeactivateUserAccountMappingAsync(
      string descriptor,
      Guid accountId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      UserAccountMappingHttpClientBase mappingHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("ce4b4a54-f365-4fcc-b623-4a3f8e7bc07c");
      object routeValues = (object) new
      {
        descriptor = descriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (accountId), accountId.ToString());
      using (await mappingHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    protected virtual Task<bool> HasMappingsAsync(
      string descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<bool>(new HttpMethod("GET"), new Guid("ce4b4a54-f365-4fcc-b623-4a3f8e7bc07c"), (object) new
      {
        descriptor = descriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    protected virtual Task<List<Guid>> QueryAccountIdsAsync(
      string descriptor,
      UserRole userRole,
      bool? useEqualsCheckForUserRoleMatch = null,
      bool? includeDeletedAccounts = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ce4b4a54-f365-4fcc-b623-4a3f8e7bc07c");
      object routeValues = (object) new
      {
        descriptor = descriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (userRole), userRole.ToString());
      bool flag;
      if (useEqualsCheckForUserRoleMatch.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = useEqualsCheckForUserRoleMatch.Value;
        string str = flag.ToString();
        collection.Add(nameof (useEqualsCheckForUserRoleMatch), str);
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

    protected virtual async Task SetUserAccountLicenseInfoAsync(
      string descriptor,
      Guid accountId,
      VisualStudioLevel maxVsLevelFromAccountLicense,
      VisualStudioLevel maxVsLevelFromAccountExtensions,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      UserAccountMappingHttpClientBase mappingHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("ce4b4a54-f365-4fcc-b623-4a3f8e7bc07c");
      object routeValues = (object) new
      {
        descriptor = descriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (accountId), accountId.ToString());
      keyValuePairList.Add(nameof (maxVsLevelFromAccountLicense), maxVsLevelFromAccountLicense.ToString());
      keyValuePairList.Add(nameof (maxVsLevelFromAccountExtensions), maxVsLevelFromAccountExtensions.ToString());
      using (await mappingHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }
  }
}
