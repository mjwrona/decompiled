// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryService.Client.DirectoryHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.AadMemberAccessStatus;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Directories.DirectoryService.Client
{
  [ResourceArea("2B98ABE4-FAE0-4B7F-8562-7141C309B9EE")]
  public class DirectoryHttpClient : VssHttpClientBase
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();
    private static readonly ApiResourceVersion s_currentApiVersion = new ApiResourceVersion(1.0);

    public DirectoryHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public DirectoryHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public DirectoryHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public DirectoryHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public DirectoryHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<IReadOnlyList<DirectoryEntityResult>> AddMembersAsync(
      IReadOnlyList<IDirectoryEntityDescriptor> members,
      string profile = null,
      string license = null,
      IEnumerable<string> localGroups = null,
      IEnumerable<string> propertiesToReturn = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DirectoryHttpClient directoryHttpClient = this;
      return await directoryHttpClient.PostAsync<IReadOnlyList<IDirectoryEntityDescriptor>, IReadOnlyList<DirectoryEntityResult>>(members, DirectoryResourceIds.Members, version: DirectoryHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) directoryHttpClient.GetQueryParameters(profile, license, localGroups, propertiesToReturn), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<DirectoryEntityResult> AddMemberAsync(
      IDirectoryEntityDescriptor member,
      string profile = null,
      string license = null,
      IEnumerable<string> localGroups = null,
      IEnumerable<string> propertiesToReturn = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DirectoryHttpClient directoryHttpClient = this;
      return await directoryHttpClient.PostAsync<IDirectoryEntityDescriptor, DirectoryEntityResult>(member, DirectoryResourceIds.Members, version: DirectoryHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) directoryHttpClient.GetQueryParameters(profile, license, localGroups, propertiesToReturn), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<DirectoryEntityResult> AddMemberAsync(
      string member,
      string profile = null,
      string license = null,
      IEnumerable<string> localGroups = null,
      IEnumerable<string> propertiesToReturn = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DirectoryHttpClient directoryHttpClient = this;
      return await directoryHttpClient.PostAsync<string, DirectoryEntityResult>(member, DirectoryResourceIds.Members, version: DirectoryHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) directoryHttpClient.GetQueryParameters(profile, license, localGroups, propertiesToReturn), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<AadMemberStatus> GetAadMemberStatusAsync(
      IdentityDescriptor identityDescriptor,
      Guid oid,
      Guid tenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DirectoryHttpClient directoryHttpClient1 = this;
      ArgumentUtility.CheckForEmptyGuid(tenantId, nameof (tenantId));
      ArgumentUtility.CheckForEmptyGuid(oid, nameof (oid));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(identityDescriptor, nameof (identityDescriptor));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (tenantId), tenantId.ToString());
      collection.Add("identity", identityDescriptor.IdentityType + ";" + identityDescriptor.Identifier);
      object obj = (object) new{ objectId = oid.ToString() };
      AadMemberStatus memberStatusAsync;
      using (new VssHttpClientBase.OperationScope("Directory", "DirectoryMemberStatus"))
      {
        DirectoryHttpClient directoryHttpClient2 = directoryHttpClient1;
        Guid statusLocationId = DirectoryResourceIds.MemberStatusLocationId;
        ApiResourceVersion currentApiVersion = DirectoryHttpClient.s_currentApiVersion;
        object routeValues = obj;
        ApiResourceVersion version = currentApiVersion;
        List<KeyValuePair<string, string>> queryParameters = collection;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        memberStatusAsync = await directoryHttpClient2.GetAsync<AadMemberStatus>(statusLocationId, routeValues, version, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
      }
      return memberStatusAsync;
    }

    public async Task<AadMemberStatus> GetAadMemberStatusAsync(
      SubjectDescriptor subjectDescriptor,
      Guid oid,
      Guid tenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DirectoryHttpClient directoryHttpClient1 = this;
      ArgumentUtility.CheckForEmptyGuid(oid, nameof (oid));
      ArgumentUtility.CheckForEmptyGuid(tenantId, nameof (tenantId));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>(2);
      collection.Add(nameof (tenantId), tenantId.ToString());
      collection.Add("identity", subjectDescriptor.ToString());
      object obj = (object) new{ objectId = oid.ToString() };
      AadMemberStatus memberStatusAsync;
      using (new VssHttpClientBase.OperationScope("Directory", "DirectoryMemberStatus"))
      {
        DirectoryHttpClient directoryHttpClient2 = directoryHttpClient1;
        Guid statusLocationId = DirectoryResourceIds.MemberStatusLocationId;
        ApiResourceVersion currentApiVersion = DirectoryHttpClient.s_currentApiVersion;
        object routeValues = obj;
        ApiResourceVersion version = currentApiVersion;
        List<KeyValuePair<string, string>> queryParameters = collection;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        memberStatusAsync = await directoryHttpClient2.GetAsync<AadMemberStatus>(statusLocationId, routeValues, version, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
      }
      return memberStatusAsync;
    }

    private List<KeyValuePair<string, string>> GetQueryParameters(
      string profile,
      string license,
      IEnumerable<string> localGroups,
      IEnumerable<string> propertiesToReturn)
    {
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (profile != null)
        collection.Add(nameof (profile), profile);
      if (license != null)
        collection.Add(nameof (license), license);
      if (localGroups != null)
        collection.Add(nameof (localGroups), this.EnumerableToCsv(localGroups));
      if (propertiesToReturn != null)
        collection.Add(nameof (propertiesToReturn), this.EnumerableToCsv(propertiesToReturn));
      return collection;
    }

    private string EnumerableToCsv(IEnumerable<string> values) => values != null ? string.Join(",", values) : (string) null;
  }
}
