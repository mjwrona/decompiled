// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PermissionLevel.Client
{
  [ResourceArea("E97D4D3C-C339-4745-A987-BD6F6C496788")]
  public class PermissionLevelHttpClient : VssHttpClientBase
  {
    public PermissionLevelHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public PermissionLevelHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public PermissionLevelHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public PermissionLevelHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public PermissionLevelHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<PermissionLevelAssignment> AssignPermissionLevelAsync(
      string subjectDescriptor,
      Guid definitionId,
      string resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("005e0302-7988-4066-9ac0-1d93a42a9f0b");
      object routeValues = (object) new
      {
        subjectDescriptor = subjectDescriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (definitionId), definitionId.ToString());
      keyValuePairList.Add(nameof (resourceId), resourceId);
      return this.SendAsync<PermissionLevelAssignment>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedPermissionLevelAssignment> GetPermissionLevelAssignmentsByDefinitionIdAsync(
      string resourceId,
      Guid definitionId,
      int? pageSize = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("005e0302-7988-4066-9ac0-1d93a42a9f0b");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (resourceId), resourceId);
      keyValuePairList.Add(nameof (definitionId), definitionId.ToString());
      if (pageSize.HasValue)
        keyValuePairList.Add(nameof (pageSize), pageSize.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedPermissionLevelAssignment>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedPermissionLevelAssignment> GetPermissionLevelAssignmentsByScopeAsync(
      string resourceId,
      PermissionLevelDefinitionScope scope,
      int? pageSize = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("005e0302-7988-4066-9ac0-1d93a42a9f0b");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (resourceId), resourceId);
      keyValuePairList.Add(nameof (scope), scope.ToString());
      if (pageSize.HasValue)
        keyValuePairList.Add(nameof (pageSize), pageSize.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedPermissionLevelAssignment>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<PermissionLevelAssignment>> GetPermissionLevelAssignmentsByScopeAndSubjectAsync(
      string subjectDescriptor,
      string resourceId,
      PermissionLevelDefinitionScope scope,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("005e0302-7988-4066-9ac0-1d93a42a9f0b");
      object routeValues = (object) new
      {
        subjectDescriptor = subjectDescriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (resourceId), resourceId);
      keyValuePairList.Add(nameof (scope), scope.ToString());
      return this.SendAsync<List<PermissionLevelAssignment>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task RemovePermissionLevelAssignmentAsync(
      string subjectDescriptor,
      Guid definitionId,
      string resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PermissionLevelHttpClient permissionLevelHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("005e0302-7988-4066-9ac0-1d93a42a9f0b");
      object routeValues = (object) new
      {
        subjectDescriptor = subjectDescriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (definitionId), definitionId.ToString());
      keyValuePairList.Add(nameof (resourceId), resourceId);
      using (await permissionLevelHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<PermissionLevelAssignment> UpdatePermissionLevelAssignmentAsync(
      string subjectDescriptor,
      string resourceId,
      Guid definitionId,
      Guid newDefinitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("005e0302-7988-4066-9ac0-1d93a42a9f0b");
      object routeValues = (object) new
      {
        subjectDescriptor = subjectDescriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (resourceId), resourceId);
      keyValuePairList.Add(nameof (definitionId), definitionId.ToString());
      keyValuePairList.Add(nameof (newDefinitionId), newDefinitionId.ToString());
      return this.SendAsync<PermissionLevelAssignment>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Dictionary<Guid, PermissionLevelDefinition>> GetPermissionLevelDefinitionsByIdAsync(
      IEnumerable<Guid> definitionIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d9247ea2-4e01-47c1-8662-980818aae5d3");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (definitionIds != null && definitionIds.Any<Guid>())
        keyValuePairList.Add(nameof (definitionIds), string.Join<Guid>(",", definitionIds));
      return this.SendAsync<Dictionary<Guid, PermissionLevelDefinition>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<PermissionLevelDefinition>> GetPermissionLevelDefinitionsByScopeAsync(
      PermissionLevelDefinitionScope definitionScope,
      PermissionLevelDefinitionType definitionType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d9247ea2-4e01-47c1-8662-980818aae5d3");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (definitionScope), definitionScope.ToString());
      keyValuePairList.Add(nameof (definitionType), definitionType.ToString());
      return this.SendAsync<List<PermissionLevelDefinition>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
