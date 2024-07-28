// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SecurityRoles.SecurityRolesHttpClient
// Assembly: Microsoft.VisualStudio.Services.SecurityRoles.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74D9BC5A-4C7E-4BC3-9331-A0A75718A098
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.SecurityRoles.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.SecurityRoles.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.SecurityRoles
{
  public class SecurityRolesHttpClient : VssHttpClientBase
  {
    public SecurityRolesHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public SecurityRolesHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public SecurityRolesHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public SecurityRolesHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public SecurityRolesHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task ChangeInheritanceAsync(
      string scopeId,
      string resourceId,
      bool inheritPermissions,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SecurityRolesHttpClient securityRolesHttpClient = this;
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("9461c234-c84c-4ed2-b918-2f0f92ad0a35");
      object routeValues = (object) new
      {
        scopeId = scopeId,
        resourceId = resourceId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (inheritPermissions), inheritPermissions.ToString());
      using (await securityRolesHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<List<RoleAssignment>> GetRoleAssignmentsAsync(
      string scopeId,
      string resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<RoleAssignment>>(new HttpMethod("GET"), new Guid("9461c234-c84c-4ed2-b918-2f0f92ad0a35"), (object) new
      {
        scopeId = scopeId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RemoveRoleAssignmentAsync(
      string scopeId,
      string resourceId,
      Guid identityId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("9461c234-c84c-4ed2-b918-2f0f92ad0a35"), (object) new
      {
        scopeId = scopeId,
        resourceId = resourceId,
        identityId = identityId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task RemoveRoleAssignmentsAsync(
      IEnumerable<Guid> identityIds,
      string scopeId,
      string resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SecurityRolesHttpClient securityRolesHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9461c234-c84c-4ed2-b918-2f0f92ad0a35");
      object obj1 = (object) new
      {
        scopeId = scopeId,
        resourceId = resourceId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<Guid>>(identityIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      SecurityRolesHttpClient securityRolesHttpClient2 = securityRolesHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await securityRolesHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public Task<RoleAssignment> SetRoleAssignmentAsync(
      UserRoleAssignmentRef roleAssignment,
      string scopeId,
      string resourceId,
      Guid identityId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("9461c234-c84c-4ed2-b918-2f0f92ad0a35");
      object obj1 = (object) new
      {
        scopeId = scopeId,
        resourceId = resourceId,
        identityId = identityId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UserRoleAssignmentRef>(roleAssignment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<RoleAssignment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<List<RoleAssignment>> SetRoleAssignmentsAsync(
      IEnumerable<UserRoleAssignmentRef> roleAssignments,
      string scopeId,
      string resourceId,
      bool? limitToCallerIdentityDomain = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("9461c234-c84c-4ed2-b918-2f0f92ad0a35");
      object obj1 = (object) new
      {
        scopeId = scopeId,
        resourceId = resourceId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<UserRoleAssignmentRef>>(roleAssignments, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (limitToCallerIdentityDomain.HasValue)
        collection.Add(nameof (limitToCallerIdentityDomain), limitToCallerIdentityDomain.Value.ToString());
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
      return this.SendAsync<List<RoleAssignment>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<List<SecurityRole>> GetRoleDefinitionsAsync(
      string scopeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<SecurityRole>>(new HttpMethod("GET"), new Guid("f4cc9a86-453c-48d2-b44d-d3bd5c105f4f"), (object) new
      {
        scopeId = scopeId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
