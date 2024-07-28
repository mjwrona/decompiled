// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Client.IdentityHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Identity.Client
{
  [ResourceArea("8A3D49B8-91F0-46EF-B33D-DDA338C25DB3")]
  [ClientCircuitBreakerSettings(100, 80, MaxConcurrentRequests = 110)]
  public class IdentityHttpClient : 
    VssHttpClientBase,
    IIdentityHttpClient,
    IVssHttpClient,
    IDisposable
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();
    private static readonly ApiResourceVersion s_currentApiVersion;
    private const int maxDescriptors = 5;
    private const int maxIds = 50;

    static IdentityHttpClient()
    {
      IdentityHttpClient.s_translatedExceptions.Add("IdentityDomainMismatchException", typeof (IdentityDomainMismatchException));
      IdentityHttpClient.s_translatedExceptions.Add("AddMemberCyclicMembershipException", typeof (AddMemberCyclicMembershipException));
      IdentityHttpClient.s_translatedExceptions.Add("IdentityPropertyRequiredException", typeof (IdentityPropertyRequiredException));
      IdentityHttpClient.s_translatedExceptions.Add("IdentityExpressionException", typeof (IdentityExpressionException));
      IdentityHttpClient.s_translatedExceptions.Add("InvalidDisplayNameException", typeof (InvalidDisplayNameException));
      IdentityHttpClient.s_translatedExceptions.Add("GroupNameNotRecognizedException", typeof (GroupNameNotRecognizedException));
      IdentityHttpClient.s_translatedExceptions.Add("IdentityMapReadOnlyException", typeof (IdentityMapReadOnlyException));
      IdentityHttpClient.s_translatedExceptions.Add("IdentityNotServiceIdentityException", typeof (IdentityNotServiceIdentityException));
      IdentityHttpClient.s_translatedExceptions.Add("InvalidServiceIdentityNameException", typeof (InvalidServiceIdentityNameException));
      IdentityHttpClient.s_translatedExceptions.Add("IllegalIdentityException", typeof (IllegalIdentityException));
      IdentityHttpClient.s_translatedExceptions.Add("MissingRequiredParameterException", typeof (MissingRequiredParameterException));
      IdentityHttpClient.s_translatedExceptions.Add("IncompatibleScopeException", typeof (IncompatibleScopeException));
      IdentityHttpClient.s_translatedExceptions.Add("GroupNameIsReservedBySystemException", typeof (GroupNameIsReservedBySystemException));
      IdentityHttpClient.s_translatedExceptions.Add("RemoveAccountOwnerFromAdminGroupException", typeof (RemoveAccountOwnerFromAdminGroupException));
      IdentityHttpClient.s_translatedExceptions.Add("RemoveSelfFromAdminGroupException", typeof (RemoveSelfFromAdminGroupException));
      IdentityHttpClient.s_translatedExceptions.Add("AddGroupMemberIllegalMemberException", typeof (AddGroupMemberIllegalMemberException));
      IdentityHttpClient.s_translatedExceptions.Add("AddGroupMemberIllegalWindowsIdentityException", typeof (AddGroupMemberIllegalWindowsIdentityException));
      IdentityHttpClient.s_translatedExceptions.Add("AddGroupMemberIllegalInternetIdentityException", typeof (AddGroupMemberIllegalInternetIdentityException));
      IdentityHttpClient.s_translatedExceptions.Add("RemoveSpecialGroupException", typeof (RemoveSpecialGroupException));
      IdentityHttpClient.s_translatedExceptions.Add("NotApplicationGroupException", typeof (NotApplicationGroupException));
      IdentityHttpClient.s_translatedExceptions.Add("ModifyEveryoneGroupException", typeof (ModifyEveryoneGroupException));
      IdentityHttpClient.s_translatedExceptions.Add("NotASecurityGroupException", typeof (NotASecurityGroupException));
      IdentityHttpClient.s_translatedExceptions.Add("RemoveMemberServiceAccountException", typeof (RemoveMemberServiceAccountException));
      IdentityHttpClient.s_translatedExceptions.Add("AccountPreferencesAlreadyExistException", typeof (AccountPreferencesAlreadyExistException));
      IdentityHttpClient.s_translatedExceptions.Add("RemoveGroupMemberNotMemberException", typeof (RemoveGroupMemberNotMemberException));
      IdentityHttpClient.s_translatedExceptions.Add("RemoveNonexistentGroupException", typeof (RemoveNonexistentGroupException));
      IdentityHttpClient.s_translatedExceptions.Add("FindGroupSidDoesNotExistException", typeof (FindGroupSidDoesNotExistException));
      IdentityHttpClient.s_translatedExceptions.Add("GroupScopeDoesNotExistException", typeof (GroupScopeDoesNotExistException));
      IdentityHttpClient.s_translatedExceptions.Add("IdentityNotFoundException", typeof (IdentityNotFoundException));
      IdentityHttpClient.s_translatedExceptions.Add("GroupCreationException", typeof (GroupCreationException));
      IdentityHttpClient.s_translatedExceptions.Add("GroupScopeCreationException", typeof (GroupScopeCreationException));
      IdentityHttpClient.s_translatedExceptions.Add("AddMemberIdentityAlreadyMemberException", typeof (AddMemberIdentityAlreadyMemberException));
      IdentityHttpClient.s_translatedExceptions.Add("GroupRenameException", typeof (GroupRenameException));
      IdentityHttpClient.s_translatedExceptions.Add("IdentityAlreadyExistsException", typeof (IdentityAlreadyExistsException));
      IdentityHttpClient.s_translatedExceptions.Add("IdentityAccountNameAlreadyInUseException", typeof (IdentityAccountNameAlreadyInUseException));
      IdentityHttpClient.s_translatedExceptions.Add("IdentityAliasAlreadyInUseException", typeof (IdentityAliasAlreadyInUseException));
      IdentityHttpClient.s_translatedExceptions.Add("AddProjectGroupProjectMismatchException", typeof (AddProjectGroupProjectMismatchException));
      IdentityHttpClient.s_translatedExceptions.Add("IdentitySyncException", typeof (IdentitySyncException));
      IdentityHttpClient.s_translatedExceptions.Add("IdentityProviderUnavailableException", typeof (IdentityProviderUnavailableException));
      IdentityHttpClient.s_currentApiVersion = new ApiResourceVersion(1.0);
    }

    public IdentityHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public IdentityHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public IdentityHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public IdentityHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public IdentityHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<IdentitiesCollection> ReadIdentitiesAsync(
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ReadIdentitiesAsyncInternal(new List<KeyValuePair<string, string>>(), queryMembership, propertyNameFilters, includeRestrictedVisibility, (RequestHeadersContext) null, userState, cancellationToken);
    }

    public virtual Task<IdentitiesCollection> ReadIdentitiesAsync(
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ReadIdentitiesAsync(descriptors, (RequestHeadersContext) null, queryMembership, propertyNameFilters, includeRestrictedVisibility, userState, cancellationToken);
    }

    public virtual Task<IdentitiesCollection> ReadIdentitiesAsync(
      IList<IdentityDescriptor> descriptors,
      RequestHeadersContext requestHeadersContext,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) descriptors, nameof (descriptors));
      if (descriptors.Count > 5)
        return this.ReadIdentitiesBatchAsyncInternal(descriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility, requestHeadersContext, userState, cancellationToken);
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      keyValuePairList.AddMultiple<IdentityDescriptor>(nameof (descriptors), (IEnumerable<IdentityDescriptor>) descriptors, IdentityHttpClient.\u003C\u003EO.\u003C0\u003E__SerializeDescriptor ?? (IdentityHttpClient.\u003C\u003EO.\u003C0\u003E__SerializeDescriptor = new Func<IdentityDescriptor, string>(IdentityHttpClient.SerializeDescriptor)));
      return this.ReadIdentitiesAsyncInternal(keyValuePairList, queryMembership, propertyNameFilters, includeRestrictedVisibility, requestHeadersContext, userState, cancellationToken);
    }

    public virtual Task<IdentitiesCollection> ReadIdentitiesAsync(
      IList<SocialDescriptor> socialDescriptors,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ReadIdentitiesAsync(socialDescriptors, (RequestHeadersContext) null, queryMembership, propertyNameFilters, includeRestrictedVisibility, userState, cancellationToken);
    }

    internal virtual Task<IdentitiesCollection> ReadIdentitiesAsync(
      IList<SocialDescriptor> socialDescriptors,
      RequestHeadersContext requestHeadersContext,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) socialDescriptors, nameof (socialDescriptors));
      if (socialDescriptors.Count > 5)
        return this.ReadIdentitiesBatchAsyncInternal(socialDescriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility, requestHeadersContext, userState, cancellationToken);
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.AddMultiple(nameof (socialDescriptors), (IEnumerable<string>) socialDescriptors.Select<SocialDescriptor, string>((Func<SocialDescriptor, string>) (descriptor => descriptor.ToString())).ToList<string>());
      return this.ReadIdentitiesAsyncInternal(keyValuePairList, queryMembership, propertyNameFilters, includeRestrictedVisibility, requestHeadersContext, userState, cancellationToken);
    }

    public virtual Task<IdentitiesCollection> ReadIdentitiesAsync(
      IList<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ReadIdentitiesAsync(subjectDescriptors, (RequestHeadersContext) null, queryMembership, propertyNameFilters, includeRestrictedVisibility, userState, cancellationToken);
    }

    internal virtual Task<IdentitiesCollection> ReadIdentitiesAsync(
      IList<SubjectDescriptor> subjectDescriptors,
      RequestHeadersContext requestHeadersContext,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) subjectDescriptors, nameof (subjectDescriptors));
      if (subjectDescriptors.Count > 5)
        return this.ReadIdentitiesBatchAsyncInternal(subjectDescriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility, requestHeadersContext, userState, cancellationToken);
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.AddMultiple(nameof (subjectDescriptors), (IEnumerable<string>) subjectDescriptors.Select<SubjectDescriptor, string>((Func<SubjectDescriptor, string>) (descriptor => descriptor.ToString())).ToList<string>());
      return this.ReadIdentitiesAsyncInternal(keyValuePairList, queryMembership, propertyNameFilters, includeRestrictedVisibility, requestHeadersContext, userState, cancellationToken);
    }

    public virtual Task<IdentitiesCollection> ReadIdentitiesAsync(
      IList<Guid> identityIds,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ReadIdentitiesAsync(identityIds, (RequestHeadersContext) null, queryMembership, propertyNameFilters, includeRestrictedVisibility, userState, cancellationToken);
    }

    internal virtual Task<IdentitiesCollection> ReadIdentitiesAsync(
      IList<Guid> identityIds,
      RequestHeadersContext requestHeadersContext,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) identityIds, nameof (identityIds));
      if (identityIds.Count > 50)
        return this.ReadIdentitiesBatchAsyncInternal(identityIds, queryMembership, propertyNameFilters, includeRestrictedVisibility, userState, requestHeadersContext, cancellationToken);
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.AddMultiple<Guid>(nameof (identityIds), (IEnumerable<Guid>) identityIds, (Func<Guid, string>) (id => id.ToString("N")));
      return this.ReadIdentitiesAsyncInternal(keyValuePairList, queryMembership, propertyNameFilters, includeRestrictedVisibility, requestHeadersContext, userState, cancellationToken);
    }

    public Task<IdentitiesCollection> ReadIdentitiesAsync(
      IdentitySearchFilter searchFilter,
      string filterValue,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ReadIdentitiesAsync(searchFilter, filterValue, ReadIdentitiesOptions.None, queryMembership, propertyNameFilters, userState, cancellationToken);
    }

    public virtual Task<IdentitiesCollection> ReadIdentitiesAsync(
      IdentitySearchFilter searchFilter,
      string filterValue,
      ReadIdentitiesOptions options,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckStringForNullOrEmpty(filterValue, nameof (filterValue));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (searchFilter), searchFilter.ToString());
      keyValuePairList.Add(nameof (filterValue), filterValue);
      keyValuePairList.Add(nameof (options), options.ToString());
      return this.ReadIdentitiesAsyncInternal(keyValuePairList, queryMembership, propertyNameFilters, false, (RequestHeadersContext) null, userState, cancellationToken);
    }

    public virtual Task<IdentitiesCollection> ReadIdentitiesAsync(
      Guid scopeId,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (scopeId), scopeId.ToString("N"));
      return this.ReadIdentitiesAsyncInternal(keyValuePairList, queryMembership, propertyNameFilters, false, (RequestHeadersContext) null, userState, cancellationToken);
    }

    public Task<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentityAsync(
      string identityPuid,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckStringForNullOrEmpty(identityPuid, nameof (identityPuid));
      return this.ReadIdentityAsyncInternal(identityPuid, queryMembership, propertyNameFilters, userState, cancellationToken);
    }

    public Task<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentityAsync(
      Guid identityId,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForEmptyGuid(identityId, nameof (identityId));
      return this.ReadIdentityAsyncInternal(identityId.ToString("D"), queryMembership, propertyNameFilters, userState, cancellationToken);
    }

    public async Task<IEnumerable<IdentityUpdateData>> UpdateIdentitiesAsync(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      IEnumerable<IdentityUpdateData> identityUpdateDatas;
      using (new VssHttpClientBase.OperationScope("IMS", "UpdateIdentities"))
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) identities, nameof (identities));
        HttpContent content = (HttpContent) new ObjectContent<VssJsonCollectionWrapper<IdentitiesCollection>>(new VssJsonCollectionWrapper<IdentitiesCollection>((IEnumerable) new IdentitiesCollection(identities)), identityHttpClient.Formatter);
        identityUpdateDatas = await identityHttpClient.SendAsync<IEnumerable<IdentityUpdateData>>(HttpMethod.Put, IdentityResourceIds.Identity, version: IdentityHttpClient.s_currentApiVersion, content: content, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return identityUpdateDatas;
    }

    public async Task<IEnumerable<IdentityUpdateData>> UpdateIdentitiesAsync(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool allowMetaDataUpdate,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient1 = this;
      IEnumerable<IdentityUpdateData> identityUpdateDatas;
      using (new VssHttpClientBase.OperationScope("IMS", "UpdateIdentities"))
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) identities, nameof (identities));
        IdentitiesCollection source = new IdentitiesCollection(identities);
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        if (allowMetaDataUpdate)
          collection.Add(nameof (allowMetaDataUpdate), "true");
        HttpContent httpContent = (HttpContent) new ObjectContent<VssJsonCollectionWrapper<IdentitiesCollection>>(new VssJsonCollectionWrapper<IdentitiesCollection>((IEnumerable) source), identityHttpClient1.Formatter);
        IdentityHttpClient identityHttpClient2 = identityHttpClient1;
        HttpMethod put = HttpMethod.Put;
        Guid identity = IdentityResourceIds.Identity;
        ApiResourceVersion currentApiVersion = IdentityHttpClient.s_currentApiVersion;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        HttpContent content = httpContent;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        identityUpdateDatas = await identityHttpClient2.SendAsync<IEnumerable<IdentityUpdateData>>(put, identity, version: currentApiVersion, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return identityUpdateDatas;
    }

    public async Task<HttpResponseMessage> UpdateIdentityAsync(
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      HttpResponseMessage httpResponseMessage;
      using (new VssHttpClientBase.OperationScope("IMS", "UpdateIdentity"))
      {
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
        HttpContent content = (HttpContent) new ObjectContent<Microsoft.VisualStudio.Services.Identity.Identity>(identity, identityHttpClient.Formatter);
        httpResponseMessage = await identityHttpClient.SendAsync(HttpMethod.Put, IdentityResourceIds.Identity, (object) new
        {
          identityId = identity.Id
        }, IdentityHttpClient.s_currentApiVersion, content, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> SwapIdentityAsync(
      Guid id1,
      Guid id2,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      HttpResponseMessage httpResponseMessage;
      using (new VssHttpClientBase.OperationScope("IMS", "SwapIdentity"))
      {
        ArgumentUtility.CheckForEmptyGuid(id1, nameof (id1));
        ArgumentUtility.CheckForEmptyGuid(id2, nameof (id2));
        HttpContent content = (HttpContent) new ObjectContent(typeof (SwapIdentityInfo), (object) new SwapIdentityInfo(id1, id2), identityHttpClient.Formatter);
        httpResponseMessage = await identityHttpClient.SendAsync(HttpMethod.Post, IdentityResourceIds.SwapLocationId, version: IdentityHttpClient.s_currentApiVersion, content: content, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return httpResponseMessage;
    }

    public async Task<ChangedIdentities> GetIdentityChangesAsync(
      int identitySequenceId,
      int groupSequenceId,
      Guid scopeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      int organizationIdentitySequenceId = -1;
      return await this.GetIdentityChangesAsync(identitySequenceId, groupSequenceId, organizationIdentitySequenceId, scopeId, userState, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ChangedIdentities> GetIdentityChangesAsync(
      int identitySequenceId,
      int groupSequenceId,
      int organizationIdentitySequenceId,
      Guid scopeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.GetIdentityChangesAsync(identitySequenceId, groupSequenceId, organizationIdentitySequenceId, 0, scopeId, userState, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<ChangedIdentities> GetIdentityChangesAsync(
      int identitySequenceId,
      int groupSequenceId,
      int organizationIdentitySequenceId,
      int pageSize,
      Guid scopeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      ChangedIdentities identityChangesAsync;
      using (new VssHttpClientBase.OperationScope("IMS", "GetIdentityChanges"))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (identitySequenceId), identitySequenceId.ToString());
        keyValuePairList.Add(nameof (groupSequenceId), groupSequenceId.ToString());
        keyValuePairList.Add(nameof (organizationIdentitySequenceId), organizationIdentitySequenceId.ToString());
        keyValuePairList.Add(nameof (pageSize), pageSize.ToString());
        keyValuePairList.Add(nameof (scopeId), scopeId.ToString("N"));
        identityChangesAsync = await identityHttpClient.SendAsync<ChangedIdentities>(HttpMethod.Get, IdentityResourceIds.Identity, version: IdentityHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return identityChangesAsync;
    }

    public async Task<IList<Guid>> GetUserIdentityIdsByDomainIdAsync(
      Guid domainId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      IList<Guid> idsByDomainIdAsync;
      using (new VssHttpClientBase.OperationScope("IMS", nameof (GetUserIdentityIdsByDomainIdAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(domainId, nameof (domainId));
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (domainId), domainId.ToString("N"));
        idsByDomainIdAsync = await identityHttpClient.SendAsync<IList<Guid>>(HttpMethod.Get, IdentityResourceIds.Identity, version: IdentityHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return idsByDomainIdAsync;
    }

    public async Task<IdentitySelf> GetIdentitySelfAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      IdentitySelf identitySelfAsync;
      using (new VssHttpClientBase.OperationScope("IMS", "GetIdentitySelf"))
        identitySelfAsync = await identityHttpClient.SendAsync<IdentitySelf>(HttpMethod.Get, IdentityResourceIds.IdentitySelf, version: IdentityHttpClient.s_currentApiVersion, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return identitySelfAsync;
    }

    public async Task<TenantInfo> GetTenant(
      string tenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      TenantInfo tenant;
      using (new VssHttpClientBase.OperationScope("IMS", nameof (GetTenant)))
      {
        ApiResourceLocation location = new ApiResourceLocation()
        {
          Id = IdentityResourceIds.IdentityTenant,
          ResourceName = "tenant",
          RouteTemplate = "_apis/identities/tenant/{tenantId}",
          ResourceVersion = 1,
          MinVersion = new Version(1, 0),
          MaxVersion = new Version(2, 0),
          ReleasedVersion = new Version(0, 0)
        };
        using (HttpRequestMessage requestMessage = identityHttpClient.CreateRequestMessage(HttpMethod.Get, location, (object) new
        {
          tenantId = tenantId
        }, IdentityHttpClient.s_currentApiVersion))
        {
          using (HttpClient client = new HttpClient())
          {
            HttpResponseMessage httpResponseMessage = await client.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();
            tenant = await httpResponseMessage.Content.ReadAsAsync<TenantInfo>((IEnumerable<MediaTypeFormatter>) new MediaTypeFormatter[1]
            {
              identityHttpClient.Formatter
            }, cancellationToken).ConfigureAwait(false);
          }
        }
      }
      return tenant;
    }

    public async Task<Microsoft.VisualStudio.Services.Identity.Identity> CreateFrameworkIdentityAsync(
      FrameworkIdentityType identityType,
      string role,
      string identifier,
      string displayName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      Microsoft.VisualStudio.Services.Identity.Identity frameworkIdentityAsync;
      using (new VssHttpClientBase.OperationScope("IMS", "CreateServiceIdentity"))
      {
        if (identityType == FrameworkIdentityType.None)
          throw new ArgumentException(CommonResources.EmptyStringNotAllowed(), nameof (identityType));
        ArgumentUtility.CheckStringForNullOrEmpty(displayName, nameof (role));
        ArgumentUtility.CheckStringForNullOrEmpty(displayName, nameof (identifier));
        ArgumentUtility.CheckStringForNullOrEmpty(displayName, nameof (displayName));
        Type type = typeof (FrameworkIdentityInfo);
        FrameworkIdentityInfo frameworkIdentityInfo = new FrameworkIdentityInfo();
        frameworkIdentityInfo.IdentityType = identityType;
        frameworkIdentityInfo.Role = role;
        frameworkIdentityInfo.Identifier = identifier;
        frameworkIdentityInfo.DisplayName = displayName;
        MediaTypeFormatter formatter = identityHttpClient.Formatter;
        HttpContent content = (HttpContent) new ObjectContent(type, (object) frameworkIdentityInfo, formatter);
        frameworkIdentityAsync = await identityHttpClient.SendAsync<Microsoft.VisualStudio.Services.Identity.Identity>(HttpMethod.Put, IdentityResourceIds.FrameworkIdentity, version: IdentityHttpClient.s_currentApiVersion, content: content, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return frameworkIdentityAsync;
    }

    public virtual async Task<IdentitiesCollection> ListGroupsAsync(
      Guid[] scopeIds = null,
      bool recurse = false,
      bool deleted = false,
      IEnumerable<string> propertyNameFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      IdentitiesCollection identitiesCollection;
      using (new VssHttpClientBase.OperationScope("IMS", "ListGroups"))
      {
        List<KeyValuePair<string, string>> keyValuePairList = (List<KeyValuePair<string, string>>) null;
        if (scopeIds != null || recurse || propertyNameFilters != null)
        {
          keyValuePairList = new List<KeyValuePair<string, string>>();
          if (scopeIds != null)
            keyValuePairList.AddMultiple<Guid>(nameof (scopeIds), (IEnumerable<Guid>) scopeIds, (Func<Guid, string>) (val => val.ToString("N")));
          if (recurse)
            keyValuePairList.Add(nameof (recurse), "true");
          if (deleted)
            keyValuePairList.Add(nameof (deleted), "true");
          if (propertyNameFilters != null)
            keyValuePairList.AddMultiple("properties", propertyNameFilters);
        }
        identitiesCollection = await identityHttpClient.SendAsync<IdentitiesCollection>(HttpMethod.Get, IdentityResourceIds.Group, version: IdentityHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return identitiesCollection;
    }

    public Task<HttpResponseMessage> DeleteGroupAsync(
      IdentityDescriptor descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.DeleteGroupAsyncInternal(IdentityHttpClient.SerializeDescriptor(descriptor), userState, cancellationToken);
    }

    public Task<HttpResponseMessage> DeleteGroupAsync(
      Guid groupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.DeleteGroupAsyncInternal(groupId.ToString(), userState, cancellationToken);
    }

    public async Task<IdentitiesCollection> CreateGroupsAsync(
      Guid scopeId,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> groups,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      IdentitiesCollection groupsAsync;
      using (new VssHttpClientBase.OperationScope("IMS", "CreateGroup"))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId));
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) groups, nameof (groups));
        HttpContent content = (HttpContent) new ObjectContent<CreateGroupsInfo>(new CreateGroupsInfo(scopeId, groups), identityHttpClient.Formatter);
        groupsAsync = await identityHttpClient.SendAsync<IdentitiesCollection>(HttpMethod.Post, IdentityResourceIds.Group, version: IdentityHttpClient.s_currentApiVersion, content: content, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return groupsAsync;
    }

    public async Task<IdentityScope> GetScopeAsync(
      string scopeName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      IdentityScope scopeAsync;
      using (new VssHttpClientBase.OperationScope("IMS", "GetScope"))
      {
        ArgumentUtility.CheckStringForNullOrEmpty(scopeName, nameof (scopeName));
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (scopeName), scopeName);
        scopeAsync = await identityHttpClient.SendAsync<IdentityScope>(HttpMethod.Get, IdentityResourceIds.Scope, version: IdentityHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return scopeAsync;
    }

    public virtual async Task<IdentityScope> GetScopeAsync(
      Guid scopeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      IdentityScope scopeAsync;
      using (new VssHttpClientBase.OperationScope("IMS", "GetScopeById"))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId));
        scopeAsync = await identityHttpClient.SendAsync<IdentityScope>(HttpMethod.Get, IdentityResourceIds.Scope, (object) new
        {
          scopeId = scopeId
        }, IdentityHttpClient.s_currentApiVersion, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return scopeAsync;
    }

    public async Task<IdentityScope> CreateScopeAsync(
      Guid scopeId,
      Guid parentScopeId,
      GroupScopeType scopeType,
      string scopeName,
      string adminGroupName,
      string adminGroupDescription,
      Guid creatorId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      IdentityScope scopeAsync;
      using (new VssHttpClientBase.OperationScope("IMS", "CreateScope"))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId));
        HttpContent content = (HttpContent) new ObjectContent<CreateScopeInfo>(new CreateScopeInfo(parentScopeId, scopeType, scopeName, adminGroupName, adminGroupDescription, creatorId), identityHttpClient.Formatter);
        scopeAsync = await identityHttpClient.SendAsync<IdentityScope>(HttpMethod.Put, IdentityResourceIds.Scope, (object) new
        {
          scopeId = scopeId
        }, IdentityHttpClient.s_currentApiVersion, content, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return scopeAsync;
    }

    public async Task<HttpResponseMessage> RenameScopeAsync(
      Guid scopeId,
      string newName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      HttpResponseMessage httpResponseMessage;
      using (new VssHttpClientBase.OperationScope("IMS", "RenameScope"))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId));
        ArgumentUtility.CheckStringForNullOrEmpty(newName, nameof (newName));
        HttpContent content = (HttpContent) new ObjectContent<IdentityScope>(new IdentityScope(scopeId, newName), identityHttpClient.Formatter);
        httpResponseMessage = await identityHttpClient.SendAsync(new HttpMethod("PATCH"), IdentityResourceIds.Scope, (object) new
        {
          scopeId = scopeId
        }, IdentityHttpClient.s_currentApiVersion, content, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> DeleteScopeAsync(
      Guid scopeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      HttpResponseMessage httpResponseMessage;
      using (new VssHttpClientBase.OperationScope("IMS", "DeleteScope"))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId));
        httpResponseMessage = await identityHttpClient.SendAsync(HttpMethod.Delete, IdentityResourceIds.Scope, (object) new
        {
          scopeId = scopeId
        }, IdentityHttpClient.s_currentApiVersion, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> RestoreGroupScopeAsync(
      Guid scopeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId));
      return await this.UpdateScopeAsync(scopeId, "IsActive", (object) true, userState, cancellationToken).ConfigureAwait(false);
    }

    private async Task<HttpResponseMessage> UpdateScopeAsync(
      Guid scopeId,
      string property,
      object value,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      HttpResponseMessage httpResponseMessage;
      using (new VssHttpClientBase.OperationScope("IMS", "UpdateScope"))
      {
        JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
        jsonPatchDocument.Add(new JsonPatchOperation()
        {
          Operation = Operation.Replace,
          Path = "/" + property,
          Value = value
        });
        HttpContent content = (HttpContent) new ObjectContent<JsonPatchDocument>(jsonPatchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
        httpResponseMessage = await identityHttpClient.SendAsync<HttpResponseMessage>(new HttpMethod("PATCH"), IdentityResourceIds.Scope, (object) new
        {
          scopeId = scopeId
        }, new ApiResourceVersion(5.0, 2), content, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return httpResponseMessage;
    }

    public Task<bool> AddMemberToGroupAsync(
      IdentityDescriptor containerId,
      Guid memberId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.AddMemberToGroupAsyncInternal((object) new
      {
        containerId = IdentityHttpClient.SerializeDescriptor(containerId),
        memberId = memberId
      }, (IEnumerable<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>(), userState, cancellationToken);
    }

    public Task<bool> AddMemberToGroupAsync(
      IdentityDescriptor containerId,
      IdentityDescriptor memberId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (memberId), IdentityHttpClient.SerializeDescriptor(memberId));
      return this.AddMemberToGroupAsyncInternal((object) new
      {
        containerId = IdentityHttpClient.SerializeDescriptor(containerId)
      }, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken);
    }

    public async Task<bool> RemoveMemberFromGroupAsync(
      IdentityDescriptor containerId,
      IdentityDescriptor memberId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      bool flag;
      using (new VssHttpClientBase.OperationScope("IMS", "RemoveMemberFromGroup"))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (memberId), IdentityHttpClient.SerializeDescriptor(memberId));
        flag = await identityHttpClient.SendAsync<bool>(HttpMethod.Delete, IdentityResourceIds.Member, (object) new
        {
          containerId = IdentityHttpClient.SerializeDescriptor(containerId)
        }, IdentityHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return flag;
    }

    public async Task<bool> ForceRemoveMemberFromGroupAsync(
      IdentityDescriptor containerId,
      IdentityDescriptor memberId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      bool flag;
      using (new VssHttpClientBase.OperationScope("IMS", "ForceRemoveMemberFromGroup"))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (memberId), IdentityHttpClient.SerializeDescriptor(memberId));
        keyValuePairList.Add(nameof (containerId), IdentityHttpClient.SerializeDescriptor(containerId));
        keyValuePairList.Add("forceRemove", true.ToString());
        flag = await identityHttpClient.SendAsync<bool>(HttpMethod.Delete, IdentityResourceIds.Member, (object) new
        {
          containerId = IdentityHttpClient.SerializeDescriptor(containerId),
          forceRemove = true.ToString()
        }, IdentityHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return flag;
    }

    public async Task<bool> IsMember(
      IdentityDescriptor containerId,
      IdentityDescriptor memberId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      bool flag;
      using (new VssHttpClientBase.OperationScope("IMS", nameof (IsMember)))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>(1);
        keyValuePairList.Add("queryMembership", "Expanded");
        flag = await identityHttpClient.SendAsync<IdentityDescriptor>(HttpMethod.Get, IdentityResourceIds.MemberOf, (object) new
        {
          memberId = IdentityHttpClient.SerializeDescriptor(memberId),
          containerId = IdentityHttpClient.SerializeDescriptor(containerId)
        }, IdentityHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false) != (IdentityDescriptor) null;
      }
      return flag;
    }

    public async Task<IdentitySnapshot> GetIdentitySnapshotAsync(
      Guid scopeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient1 = this;
      IdentitySnapshot identitySnapshotAsync;
      using (new VssHttpClientBase.OperationScope("IMS", "GetIdentitySnapshot"))
      {
        IdentityHttpClient identityHttpClient2 = identityHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid identitySnapshot = IdentityResourceIds.IdentitySnapshot;
        ApiResourceVersion currentApiVersion = IdentityHttpClient.s_currentApiVersion;
        var routeValues = new{ scopeId = scopeId };
        ApiResourceVersion version = currentApiVersion;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        identitySnapshotAsync = await identityHttpClient2.SendAsync<IdentitySnapshot>(get, identitySnapshot, (object) routeValues, version, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return identitySnapshotAsync;
    }

    public async Task<AccessTokenResult> GetSignoutToken(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient1 = this;
      AccessTokenResult signoutToken1;
      using (new VssHttpClientBase.OperationScope("IMS", nameof (GetSignoutToken)))
      {
        IdentityHttpClient identityHttpClient2 = identityHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid signoutToken2 = IdentityResourceIds.SignoutToken;
        ApiResourceVersion currentApiVersion = IdentityHttpClient.s_currentApiVersion;
        object routeValues = new object();
        ApiResourceVersion version = currentApiVersion;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        signoutToken1 = await identityHttpClient2.SendAsync<AccessTokenResult>(get, signoutToken2, routeValues, version, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return signoutToken1;
    }

    public async Task<AccessTokenResult> GetSignedInToken(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient1 = this;
      AccessTokenResult signedInToken1;
      using (new VssHttpClientBase.OperationScope("IMS", nameof (GetSignedInToken)))
      {
        IdentityHttpClient identityHttpClient2 = identityHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid signedInToken2 = IdentityResourceIds.SignedInToken;
        ApiResourceVersion currentApiVersion = IdentityHttpClient.s_currentApiVersion;
        object routeValues = new object();
        ApiResourceVersion version = currentApiVersion;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        signedInToken1 = await identityHttpClient2.SendAsync<AccessTokenResult>(get, signedInToken2, routeValues, version, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return signedInToken1;
    }

    public async Task<int> GetMaxSequenceIdAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      int maxSequenceIdAsync;
      using (new VssHttpClientBase.OperationScope("IMS", "GetMaxSequenceId"))
        maxSequenceIdAsync = await identityHttpClient.SendAsync<int>(HttpMethod.Get, IdentityResourceIds.IdentityMaxSequenceId, version: IdentityHttpClient.s_currentApiVersion, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return maxSequenceIdAsync;
    }

    public async Task<Microsoft.VisualStudio.Services.Identity.Identity> CreateOrBindIdentity(
      Microsoft.VisualStudio.Services.Identity.Identity sourceIdentity,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient1 = this;
      Microsoft.VisualStudio.Services.Identity.Identity orBindIdentity;
      using (new VssHttpClientBase.OperationScope("IMS", "CreateOrBindWithClaims"))
      {
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(sourceIdentity, nameof (sourceIdentity));
        ArgumentUtility.CheckForNull<IdentityDescriptor>(sourceIdentity.Descriptor, "Descriptor");
        HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.VisualStudio.Services.Identity.Identity>(sourceIdentity, identityHttpClient1.Formatter);
        IdentityHttpClient identityHttpClient2 = identityHttpClient1;
        HttpMethod put = HttpMethod.Put;
        Guid claims = IdentityResourceIds.Claims;
        ApiResourceVersion currentApiVersion = IdentityHttpClient.s_currentApiVersion;
        object obj = userState;
        HttpContent content = httpContent;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        orBindIdentity = await identityHttpClient2.SendAsync<Microsoft.VisualStudio.Services.Identity.Identity>(put, claims, version: currentApiVersion, content: content, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return orBindIdentity;
    }

    public async Task<IdentityDescriptor> GetDescriptorByIdAsync(
      Guid id,
      bool? isMasterId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (isMasterId.HasValue)
        keyValuePairList.Add(nameof (isMasterId), isMasterId.Value.ToString());
      return await identityHttpClient.SendAsync<IdentityDescriptor>(HttpMethod.Get, IdentityResourceIds.DescriptorsResourceLocationId, routeValues, new ApiResourceVersion("3.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task TransferIdentityRightsBatchAsync(
      IdentityRightsTransferData identityRightsTransferData,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      HttpContent content = (HttpContent) new ObjectContent<IdentityRightsTransferData>(identityRightsTransferData, identityHttpClient.Formatter);
      using (await identityHttpClient.SendAsync(HttpMethod.Post, IdentityResourceIds.RightsBatch, version: IdentityHttpClient.s_currentApiVersion, content: content, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task TransferIdentityRightsSingleAsync(
      Guid sourceId,
      Guid targetMasterId,
      Guid sourceMasterId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("IMS", "TransferIdentityRightsSingle"))
      {
        ArgumentUtility.CheckForEmptyGuid(sourceId, nameof (sourceId));
        ArgumentUtility.CheckForEmptyGuid(targetMasterId, nameof (targetMasterId));
        ArgumentUtility.CheckForEmptyGuid(sourceMasterId, nameof (sourceMasterId));
        var data1 = new{ fromIdentity = sourceId };
        var data2 = new
        {
          targetMasterId = targetMasterId,
          sourceMasterId = sourceMasterId
        };
        IdentityHttpClient identityHttpClient2 = identityHttpClient1;
        var data3 = data2;
        Guid rights = IdentityResourceIds.Rights;
        ApiResourceVersion currentApiVersion = IdentityHttpClient.s_currentApiVersion;
        var routeValues = data1;
        ApiResourceVersion version = currentApiVersion;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        using (await identityHttpClient2.PostAsync(data3, rights, (object) routeValues, version, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false))
          ;
      }
    }

    public async Task<PagedIdentities> ListUsersAsync(
      string scopeDescriptor = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopeDescriptor != null)
        keyValuePairList.Add(nameof (scopeDescriptor), scopeDescriptor);
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      PagedIdentities pagedIdentities1;
      using (HttpRequestMessage requestMessage = await identityHttpClient.CreateRequestMessageAsync(method, IdentityResourceIds.Users, version: new ApiResourceVersion("5.3-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        PagedIdentities returnObject = new PagedIdentities();
        using (HttpResponseMessage response = await identityHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ContinuationToken = identityHttpClient.GetHeaderValue(response, "X-MS-ContinuationToken");
          PagedIdentities pagedIdentities = returnObject;
          pagedIdentities.Identities = (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) await identityHttpClient.ReadContentAsAsync<List<Microsoft.VisualStudio.Services.Identity.Identity>>(response, cancellationToken).ConfigureAwait(false);
          pagedIdentities = (PagedIdentities) null;
        }
        pagedIdentities1 = returnObject;
      }
      return pagedIdentities1;
    }

    private async Task<IdentitiesCollection> ReadIdentitiesAsyncInternal(
      List<KeyValuePair<string, string>> searchQuery,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility,
      RequestHeadersContext requestHeadersContext,
      object userState,
      CancellationToken cancellationToken)
    {
      IdentityHttpClient identityHttpClient = this;
      IdentitiesCollection identitiesCollection;
      using (new VssHttpClientBase.OperationScope("IMS", "ReadIdentities"))
      {
        identityHttpClient.AppendQueryString(searchQuery, queryMembership, propertyNameFilters, includeRestrictedVisibility);
        KeyValuePair<string, string>[] additionalHeaders = RequestHeadersContext.HeadersUtils.PopulateRequestHeaders(requestHeadersContext);
        identitiesCollection = await identityHttpClient.SendAsync<IdentitiesCollection>(HttpMethod.Get, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, IdentityResourceIds.Identity, version: IdentityHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) searchQuery, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return identitiesCollection;
    }

    private async Task<IdentitiesCollection> ReadIdentitiesBatchAsyncInternal(
      IList<SocialDescriptor> socialDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility,
      RequestHeadersContext requestHeadersContext,
      object userState,
      CancellationToken cancellationToken)
    {
      IdentityHttpClient identityHttpClient1 = this;
      IdentitiesCollection identitiesCollection;
      using (new VssHttpClientBase.OperationScope("IMS", "ReadIdentitiesBatch"))
      {
        HttpContent httpContent = (HttpContent) new ObjectContent<IdentityBatchInfo>(new IdentityBatchInfo(socialDescriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility), identityHttpClient1.Formatter);
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (queryMembership), queryMembership.ToString());
        collection.Add("flavor", "socialDescriptor");
        IList<SocialDescriptor> socialDescriptorList = socialDescriptors;
        collection.Add("count", (socialDescriptorList != null ? socialDescriptorList.Count : 0).ToString());
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        KeyValuePair<string, string>[] keyValuePairArray = RequestHeadersContext.HeadersUtils.PopulateRequestHeaders(requestHeadersContext);
        IdentityHttpClient identityHttpClient2 = identityHttpClient1;
        HttpMethod post = HttpMethod.Post;
        KeyValuePair<string, string>[] additionalHeaders = keyValuePairArray;
        Guid identityBatch = IdentityResourceIds.IdentityBatch;
        ApiResourceVersion currentApiVersion = IdentityHttpClient.s_currentApiVersion;
        HttpContent content = httpContent;
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        identitiesCollection = await identityHttpClient2.SendAsync<IdentitiesCollection>(post, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, identityBatch, version: currentApiVersion, content: content, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return identitiesCollection;
    }

    private async Task<IdentitiesCollection> ReadIdentitiesBatchAsyncInternal(
      IList<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility,
      RequestHeadersContext requestHeadersContext,
      object userState,
      CancellationToken cancellationToken)
    {
      IdentityHttpClient identityHttpClient1 = this;
      IdentitiesCollection identitiesCollection;
      using (new VssHttpClientBase.OperationScope("IMS", "ReadIdentitiesBatch"))
      {
        HttpContent httpContent = (HttpContent) new ObjectContent<IdentityBatchInfo>(new IdentityBatchInfo(subjectDescriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility), identityHttpClient1.Formatter);
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (queryMembership), queryMembership.ToString());
        collection.Add("flavor", "subjectDescriptor");
        IList<SubjectDescriptor> subjectDescriptorList = subjectDescriptors;
        collection.Add("count", (subjectDescriptorList != null ? subjectDescriptorList.Count : 0).ToString());
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        KeyValuePair<string, string>[] keyValuePairArray = RequestHeadersContext.HeadersUtils.PopulateRequestHeaders(requestHeadersContext);
        IdentityHttpClient identityHttpClient2 = identityHttpClient1;
        HttpMethod post = HttpMethod.Post;
        KeyValuePair<string, string>[] additionalHeaders = keyValuePairArray;
        Guid identityBatch = IdentityResourceIds.IdentityBatch;
        ApiResourceVersion currentApiVersion = IdentityHttpClient.s_currentApiVersion;
        HttpContent content = httpContent;
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        identitiesCollection = await identityHttpClient2.SendAsync<IdentitiesCollection>(post, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, identityBatch, version: currentApiVersion, content: content, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return identitiesCollection;
    }

    private async Task<IdentitiesCollection> ReadIdentitiesBatchAsyncInternal(
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility,
      RequestHeadersContext requestHeadersContext,
      object userState,
      CancellationToken cancellationToken)
    {
      IdentityHttpClient identityHttpClient1 = this;
      IdentitiesCollection identitiesCollection;
      using (new VssHttpClientBase.OperationScope("IMS", "ReadIdentitiesBatch"))
      {
        HttpContent httpContent = (HttpContent) new ObjectContent<IdentityBatchInfo>(new IdentityBatchInfo(descriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility), identityHttpClient1.Formatter);
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (queryMembership), queryMembership.ToString());
        collection.Add("flavor", "descriptor");
        IList<IdentityDescriptor> identityDescriptorList = descriptors;
        collection.Add("count", (identityDescriptorList != null ? identityDescriptorList.Count : 0).ToString());
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        KeyValuePair<string, string>[] keyValuePairArray = RequestHeadersContext.HeadersUtils.PopulateRequestHeaders(requestHeadersContext);
        IdentityHttpClient identityHttpClient2 = identityHttpClient1;
        HttpMethod post = HttpMethod.Post;
        KeyValuePair<string, string>[] additionalHeaders = keyValuePairArray;
        Guid identityBatch = IdentityResourceIds.IdentityBatch;
        ApiResourceVersion currentApiVersion = IdentityHttpClient.s_currentApiVersion;
        HttpContent content = httpContent;
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        identitiesCollection = await identityHttpClient2.SendAsync<IdentitiesCollection>(post, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, identityBatch, version: currentApiVersion, content: content, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return identitiesCollection;
    }

    private async Task<IdentitiesCollection> ReadIdentitiesBatchAsyncInternal(
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility,
      object userState,
      RequestHeadersContext requestHeadersContext,
      CancellationToken cancellationToken)
    {
      IdentityHttpClient identityHttpClient1 = this;
      IdentitiesCollection identitiesCollection;
      using (new VssHttpClientBase.OperationScope("IMS", "ReadIdentitiesBatch"))
      {
        HttpContent httpContent = (HttpContent) new ObjectContent<IdentityBatchInfo>(new IdentityBatchInfo(identityIds, queryMembership, propertyNameFilters, includeRestrictedVisibility), identityHttpClient1.Formatter);
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (queryMembership), queryMembership.ToString());
        collection.Add("flavor", "id");
        IList<Guid> guidList = identityIds;
        collection.Add("count", (guidList != null ? guidList.Count : 0).ToString());
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        KeyValuePair<string, string>[] keyValuePairArray = RequestHeadersContext.HeadersUtils.PopulateRequestHeaders(requestHeadersContext);
        IdentityHttpClient identityHttpClient2 = identityHttpClient1;
        HttpMethod post = HttpMethod.Post;
        KeyValuePair<string, string>[] additionalHeaders = keyValuePairArray;
        Guid identityBatch = IdentityResourceIds.IdentityBatch;
        ApiResourceVersion currentApiVersion = IdentityHttpClient.s_currentApiVersion;
        HttpContent content = httpContent;
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        identitiesCollection = await identityHttpClient2.SendAsync<IdentitiesCollection>(post, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, identityBatch, version: currentApiVersion, content: content, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return identitiesCollection;
    }

    private async Task<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentityAsyncInternal(
      string identityId,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      object userState,
      CancellationToken cancellationToken)
    {
      IdentityHttpClient identityHttpClient = this;
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      using (new VssHttpClientBase.OperationScope("IMS", "ReadIdentity"))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        identityHttpClient.AppendQueryString(keyValuePairList, queryMembership, propertyNameFilters, false);
        identity = await identityHttpClient.SendAsync<Microsoft.VisualStudio.Services.Identity.Identity>(HttpMethod.Get, IdentityResourceIds.Identity, (object) new
        {
          identityId = identityId
        }, IdentityHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return identity;
    }

    private async Task<HttpResponseMessage> DeleteGroupAsyncInternal(
      string groupId,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      HttpResponseMessage httpResponseMessage;
      using (new VssHttpClientBase.OperationScope("IMS", "DeleteGroup"))
        httpResponseMessage = await identityHttpClient.SendAsync(HttpMethod.Delete, IdentityResourceIds.Group, (object) new
        {
          groupId = groupId
        }, IdentityHttpClient.s_currentApiVersion, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return httpResponseMessage;
    }

    public async Task<bool> AddMemberToGroupAsyncInternal(
      object routeParams,
      IEnumerable<KeyValuePair<string, string>> query,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityHttpClient identityHttpClient = this;
      bool groupAsyncInternal;
      using (new VssHttpClientBase.OperationScope("IMS", "AddMemberToGroup"))
        groupAsyncInternal = await identityHttpClient.SendAsync<bool>(HttpMethod.Put, IdentityResourceIds.Member, routeParams, IdentityHttpClient.s_currentApiVersion, queryParameters: query, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return groupAsyncInternal;
    }

    private void AppendQueryString(
      List<KeyValuePair<string, string>> queryParams,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility)
    {
      queryParams.Add(nameof (queryMembership), queryMembership.ToString());
      queryParams.AddMultiple("properties", propertyNameFilters);
      if (!includeRestrictedVisibility)
        return;
      queryParams.Add(nameof (includeRestrictedVisibility), "true");
    }

    private static string SerializeDescriptor(IdentityDescriptor descriptor)
    {
      if (descriptor == (IdentityDescriptor) null)
        return string.Empty;
      return string.Join(";", new string[2]
      {
        descriptor.IdentityType,
        descriptor.Identifier
      });
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) IdentityHttpClient.s_translatedExceptions;

    private static class IdentityBatchTelemetryConstants
    {
      public const string QueryMembershipHint = "queryMembership";
      public const string FlavorHint = "flavor";
      public const string CountHint = "count";
      public const string ByIdFlavor = "id";
      public const string ByDescriptorFlavor = "descriptor";
      public const string BySubjectDescriptorFlavor = "subjectDescriptor";
      public const string BySocialDescriptorFlavor = "socialDescriptor";
    }
  }
}
