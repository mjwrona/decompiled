// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.Client.SecurityBackingStoreHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Security.Client
{
  public class SecurityBackingStoreHttpClient : VssHttpClientBase
  {
    private static readonly ApiResourceVersion s_apiVersion1 = new ApiResourceVersion("1.0");
    private static readonly ApiResourceVersion s_apiVersion2 = new ApiResourceVersion("2.0");

    public SecurityBackingStoreHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public SecurityBackingStoreHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public SecurityBackingStoreHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public SecurityBackingStoreHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public SecurityBackingStoreHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<SecurityNamespaceDataCollection> QuerySecurityDataAsync(
      Guid securityNamespaceId,
      bool useVsidSubjects = true,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SecurityBackingStoreHttpClient backingStoreHttpClient1 = this;
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      SecurityNamespaceDataCollection namespaceDataCollection;
      using (new VssHttpClientBase.OperationScope("SBS", "QuerySecurityData"))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        if (!useVsidSubjects)
          collection.Add<bool>(nameof (useVsidSubjects), useVsidSubjects);
        SecurityBackingStoreHttpClient backingStoreHttpClient2 = backingStoreHttpClient1;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        Guid backingStoreNamespace = LocationResourceIds.SecurityBackingStoreNamespace;
        var routeValues = new
        {
          securityNamespaceId = securityNamespaceId
        };
        ApiResourceVersion version = new ApiResourceVersion(new Version(3, 0), 3);
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        namespaceDataCollection = await backingStoreHttpClient2.GetAsync<SecurityNamespaceDataCollection>(backingStoreNamespace, (object) routeValues, version, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
      }
      return namespaceDataCollection;
    }

    public async Task<SecurityNamespaceData> QuerySecurityDataAsync(
      Guid securityNamespaceId,
      Guid aclStoreId,
      long oldSequenceId = -1,
      bool useVsidSubjects = true,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SecurityBackingStoreHttpClient backingStoreHttpClient1 = this;
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForEmptyGuid(aclStoreId, nameof (aclStoreId));
      SecurityNamespaceData securityNamespaceData;
      using (new VssHttpClientBase.OperationScope("SBS", "QuerySecurityData"))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        if (oldSequenceId >= 0L)
          collection.Add(nameof (oldSequenceId), oldSequenceId.ToString());
        if (!useVsidSubjects)
          collection.Add<bool>(nameof (useVsidSubjects), useVsidSubjects);
        SecurityBackingStoreHttpClient backingStoreHttpClient2 = backingStoreHttpClient1;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        Guid backingStoreAclStore = LocationResourceIds.SecurityBackingStoreAclStore;
        var routeValues = new
        {
          securityNamespaceId = securityNamespaceId,
          aclStoreId = aclStoreId
        };
        ApiResourceVersion version = new ApiResourceVersion(new Version(3, 0), 2);
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        securityNamespaceData = await backingStoreHttpClient2.GetAsync<SecurityNamespaceData>(backingStoreAclStore, (object) routeValues, version, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
      }
      return securityNamespaceData;
    }

    public Task<long> SetAccessControlListsAsync(
      Guid securityNamespaceId,
      IEnumerable<AccessControlList> acls,
      IEnumerable<AccessControlEntry> aces,
      bool throwOnInvalidIdentity = true,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForNull<IEnumerable<AccessControlList>>(acls, nameof (acls));
      ArgumentUtility.CheckForNull<IEnumerable<AccessControlEntry>>(aces, nameof (aces));
      using (new VssHttpClientBase.OperationScope("SBS", "SetAccessControlLists"))
        return this.PostAsync<SecurityBackingStoreHttpClient.SetAccessControlListsRequest, long>(new SecurityBackingStoreHttpClient.SetAccessControlListsRequest(acls, aces, throwOnInvalidIdentity), LocationResourceIds.SecurityBackingStoreAcls, (object) new
        {
          securityNamespaceId = securityNamespaceId
        }, SecurityBackingStoreHttpClient.s_apiVersion1, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<long> RemoveAccessControlListsAsync(
      Guid securityNamespaceId,
      IEnumerable<string> tokens,
      bool recurse,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(tokens, nameof (tokens));
      using (new VssHttpClientBase.OperationScope("SBS", "RemoveAccessControlLists"))
        return this.PatchAsync<SecurityBackingStoreHttpClient.RemoveAccessControlListsRequest, long>(new SecurityBackingStoreHttpClient.RemoveAccessControlListsRequest(tokens, recurse), LocationResourceIds.SecurityBackingStoreAcls, (object) new
        {
          securityNamespaceId = securityNamespaceId
        }, SecurityBackingStoreHttpClient.s_apiVersion1, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<long> SetPermissionsAsync(
      Guid securityNamespaceId,
      string token,
      IEnumerable<AccessControlEntry> permissions,
      bool merge,
      bool throwOnInvalidIdentity = true,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      ArgumentUtility.CheckForNull<IEnumerable<AccessControlEntry>>(permissions, nameof (permissions));
      using (new VssHttpClientBase.OperationScope("SBS", "SetPermissions"))
        return this.PostAsync<SecurityBackingStoreHttpClient.SetPermissionsRequest, long>(new SecurityBackingStoreHttpClient.SetPermissionsRequest(token, permissions, merge, throwOnInvalidIdentity), LocationResourceIds.SecurityBackingStoreAces, (object) new
        {
          securityNamespaceId = securityNamespaceId
        }, SecurityBackingStoreHttpClient.s_apiVersion1, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<long> RemovePermissionsAsync(
      Guid securityNamespaceId,
      string token,
      IEnumerable<Guid> identityIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(identityIds, nameof (identityIds));
      using (new VssHttpClientBase.OperationScope("SBS", "RemovePermissions"))
        return this.PatchAsync<SecurityBackingStoreHttpClient.RemovePermissionsRequest, long>(new SecurityBackingStoreHttpClient.RemovePermissionsRequest(token, identityIds), LocationResourceIds.SecurityBackingStoreAces, (object) new
        {
          securityNamespaceId = securityNamespaceId
        }, SecurityBackingStoreHttpClient.s_apiVersion1, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<long> SetInheritFlagAsync(
      Guid securityNamespaceId,
      string token,
      bool inheritFlag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      using (new VssHttpClientBase.OperationScope("SBS", "SetInheritFlag"))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (token), token);
        collection.Add(nameof (inheritFlag), inheritFlag.ToString().ToLowerInvariant());
        HttpMethod method = new HttpMethod("PATCH");
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        Guid backingStoreInherit = LocationResourceIds.SecurityBackingStoreInherit;
        var routeValues = new
        {
          securityNamespaceId = securityNamespaceId
        };
        ApiResourceVersion apiVersion1 = SecurityBackingStoreHttpClient.s_apiVersion1;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        return this.SendAsync<long>(method, backingStoreInherit, (object) routeValues, apiVersion1, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1);
      }
    }

    public Task<long> RenameTokensAsync(
      Guid securityNamespaceId,
      IEnumerable<TokenRename> renames,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForNull<IEnumerable<TokenRename>>(renames, nameof (renames));
      using (new VssHttpClientBase.OperationScope("SBS", "RenameTokens"))
        return this.PatchAsync<SecurityBackingStoreHttpClient.RenameTokensRequest, long>(new SecurityBackingStoreHttpClient.RenameTokensRequest(renames), LocationResourceIds.SecurityBackingStoreTokens, (object) new
        {
          securityNamespaceId = securityNamespaceId
        }, SecurityBackingStoreHttpClient.s_apiVersion1, userState: userState, cancellationToken: cancellationToken);
    }

    [DataContract]
    private class SetAccessControlListsRequest
    {
      public SetAccessControlListsRequest(
        IEnumerable<AccessControlList> acls,
        IEnumerable<AccessControlEntry> aces,
        bool throwOnInvalidIdentity)
      {
        this.AccessControlLists = acls;
        this.AccessControlEntries = aces;
        this.ThrowOnInvalidIdentity = throwOnInvalidIdentity;
      }

      [DataMember]
      public IEnumerable<AccessControlList> AccessControlLists { get; private set; }

      [DataMember]
      public IEnumerable<AccessControlEntry> AccessControlEntries { get; private set; }

      [DataMember]
      public bool ThrowOnInvalidIdentity { get; private set; }
    }

    [DataContract]
    private class RemoveAccessControlListsRequest
    {
      public RemoveAccessControlListsRequest(IEnumerable<string> tokens, bool recurse)
      {
        this.Tokens = tokens;
        this.Recurse = recurse;
      }

      [DataMember]
      public IEnumerable<string> Tokens { get; private set; }

      [DataMember]
      public bool Recurse { get; private set; }
    }

    [DataContract]
    private class SetPermissionsRequest
    {
      public SetPermissionsRequest(
        string token,
        IEnumerable<AccessControlEntry> aces,
        bool merge,
        bool throwOnInvalidIdentity)
      {
        this.Token = token;
        this.AccessControlEntries = aces;
        this.Merge = merge;
        this.ThrowOnInvalidIdentity = throwOnInvalidIdentity;
      }

      [DataMember]
      public string Token { get; private set; }

      [DataMember]
      public IEnumerable<AccessControlEntry> AccessControlEntries { get; private set; }

      [DataMember]
      public bool Merge { get; private set; }

      [DataMember]
      public bool ThrowOnInvalidIdentity { get; private set; }
    }

    [DataContract]
    private class RemovePermissionsRequest
    {
      public RemovePermissionsRequest(string token, IEnumerable<Guid> identityIds)
      {
        this.Token = token;
        this.IdentityIds = identityIds;
      }

      [DataMember]
      public string Token { get; private set; }

      [DataMember]
      public IEnumerable<Guid> IdentityIds { get; private set; }
    }

    [DataContract]
    private class RenameTokensRequest
    {
      public RenameTokensRequest(IEnumerable<TokenRename> renames) => this.Renames = renames;

      [DataMember]
      public IEnumerable<TokenRename> Renames { get; private set; }
    }
  }
}
