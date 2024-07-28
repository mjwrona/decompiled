// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.Client.SecurityHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Security.Client
{
  [ClientCancellationTimeout(60)]
  [ClientCircuitBreakerSettings(10, 50)]
  public class SecurityHttpClient : VssHttpClientBase
  {
    private static readonly ApiResourceVersion s_pluralHasPermissionVersion = new ApiResourceVersion(new Version(2, 2), 2);
    private static readonly ApiResourceVersion s_batchHasPermissionVersion = new ApiResourceVersion(new Version(3, 0), 1);

    public SecurityHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public SecurityHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public SecurityHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public SecurityHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public SecurityHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<bool> HasPermissionAsync(
      Guid securityNamespaceId,
      string token,
      int requestedPermissions,
      bool alwaysAllowAdministrators,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      Uri uri = new Uri(PathUtility.Combine(this.BaseAddress.GetLeftPart(UriPartial.Path), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/_apis/permissions/{0}/{1}", (object) securityNamespaceId, (object) requestedPermissions)));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (token), token);
      keyValuePairList.Add(nameof (alwaysAllowAdministrators), alwaysAllowAdministrators.ToString());
      return this.SendAsync<bool>(new HttpRequestMessage(HttpMethod.Get, uri.AppendQuery((IEnumerable<KeyValuePair<string, string>>) keyValuePairList).AbsoluteUri), userState, cancellationToken);
    }

    public async Task<List<bool>> HasPermissionsAsync(
      Guid securityNamespaceId,
      IEnumerable<string> tokens,
      int requestedPermissions,
      bool alwaysAllowAdministrators,
      char wireDelimiter = ',',
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SecurityHttpClient securityHttpClient1 = this;
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) tokens, nameof (tokens));
      ApiResourceVersion apiResourceVersion = await securityHttpClient1.NegotiateRequestVersionAsync(LocationResourceIds.SecurityPermissions, SecurityHttpClient.s_pluralHasPermissionVersion, userState, cancellationToken).ConfigureAwait(false);
      if (apiResourceVersion == null || apiResourceVersion.ResourceVersion < SecurityHttpClient.s_pluralHasPermissionVersion.ResourceVersion)
      {
        List<bool> toReturn = new List<bool>();
        foreach (string token in tokens)
          toReturn.Add(await securityHttpClient1.HasPermissionAsync(securityNamespaceId, token, requestedPermissions, alwaysAllowAdministrators, userState, cancellationToken).ConfigureAwait(false));
        return toReturn;
      }
      if (await securityHttpClient1.NegotiateRequestVersionAsync(LocationResourceIds.SecurityPermissionEvaluationBatch, SecurityHttpClient.s_batchHasPermissionVersion, userState, cancellationToken).ConfigureAwait(false) != null)
      {
        HttpContent httpContent = (HttpContent) new ObjectContent<PermissionEvaluationBatch>(new PermissionEvaluationBatch()
        {
          AlwaysAllowAdministrators = alwaysAllowAdministrators,
          Evaluations = tokens.Select<string, PermissionEvaluation>((Func<string, PermissionEvaluation>) (token => new PermissionEvaluation()
          {
            SecurityNamespaceId = securityNamespaceId,
            Token = token,
            Permissions = requestedPermissions
          })).ToArray<PermissionEvaluation>()
        }, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        SecurityHttpClient securityHttpClient2 = securityHttpClient1;
        HttpMethod method = new HttpMethod("POST");
        Guid permissionEvaluationBatch = LocationResourceIds.SecurityPermissionEvaluationBatch;
        ApiResourceVersion permissionVersion = SecurityHttpClient.s_batchHasPermissionVersion;
        object obj = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpContent content = httpContent;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        return await securityHttpClient2.SendAsync<PermissionEvaluationBatch>(method, permissionEvaluationBatch, version: permissionVersion, content: content, userState: userState1, cancellationToken: cancellationToken2).ContinueWith<List<bool>>((Func<Task<PermissionEvaluationBatch>, List<bool>>) (evalBatch => ((IEnumerable<PermissionEvaluation>) evalBatch.Result.Evaluations).Select<PermissionEvaluation, bool>((Func<PermissionEvaluation, bool>) (e => e.Value)).ToList<bool>())).ConfigureAwait(false);
      }
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (tokens), string.Join(wireDelimiter.ToString(), tokens));
      collection.Add(nameof (alwaysAllowAdministrators), alwaysAllowAdministrators.ToString());
      collection.Add("delimiter", wireDelimiter.ToString());
      SecurityHttpClient securityHttpClient3 = securityHttpClient1;
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      Guid securityPermissions = LocationResourceIds.SecurityPermissions;
      var routeValues = new
      {
        securityNamespaceId = securityNamespaceId,
        permissions = requestedPermissions
      };
      ApiResourceVersion permissionVersion1 = SecurityHttpClient.s_pluralHasPermissionVersion;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState2 = userState;
      CancellationToken cancellationToken3 = cancellationToken;
      return await securityHttpClient3.GetAsync<List<bool>>(securityPermissions, (object) routeValues, permissionVersion1, queryParameters, userState2, cancellationToken3).ConfigureAwait(false);
    }

    public Task<AccessControlEntry> RemovePermissionAsync(
      Guid securityNamespaceId,
      string token,
      IdentityDescriptor descriptor,
      int permissions,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      Uri uri = new Uri(PathUtility.Combine(this.BaseAddress.GetLeftPart(UriPartial.Path), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/_apis/permissions/{0}/{1}", (object) securityNamespaceId, (object) permissions)));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (token), token);
      keyValuePairList.Add(nameof (descriptor), descriptor.ToString());
      return this.SendAsync<AccessControlEntry>(new HttpRequestMessage(HttpMethod.Delete, uri.AppendQuery((IEnumerable<KeyValuePair<string, string>>) keyValuePairList).AbsoluteUri), userState, cancellationToken);
    }

    public Task<IEnumerable<AccessControlEntry>> SetAccessControlEntriesAsync(
      Guid securityNamespaceId,
      string token,
      IEnumerable<AccessControlEntry> accessControlEntries,
      bool merge,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      ArgumentUtility.CheckForNull<IEnumerable<AccessControlEntry>>(accessControlEntries, nameof (accessControlEntries));
      Uri uri = new Uri(PathUtility.Combine(this.BaseAddress.GetLeftPart(UriPartial.Path), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/_apis/accesscontrolentries/{0}", (object) securityNamespaceId)));
      SetAccessControlEntriesInfo controlEntriesInfo = new SetAccessControlEntriesInfo(token, accessControlEntries, merge);
      return this.SendAsync<IEnumerable<AccessControlEntry>>(new HttpRequestMessage(HttpMethod.Post, uri.AbsoluteUri)
      {
        Content = (HttpContent) new ObjectContent<SetAccessControlEntriesInfo>(controlEntriesInfo, this.Formatter)
      }, userState, cancellationToken);
    }

    public Task<bool> RemoveAccessControlEntriesAsync(
      Guid securityNamespaceId,
      string token,
      IEnumerable<IdentityDescriptor> descriptors,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      Uri uri = new Uri(PathUtility.Combine(this.BaseAddress.GetLeftPart(UriPartial.Path), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/_apis/accesscontrolentries/{0}", (object) securityNamespaceId)));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (token), token);
      if (descriptors != null)
        keyValuePairList.AddMultiple<IdentityDescriptor>(nameof (descriptors), descriptors, (Func<IdentityDescriptor, string>) (descriptor => descriptor.ToString()));
      return this.SendAsync<bool>(new HttpRequestMessage(HttpMethod.Delete, uri.AppendQuery((IEnumerable<KeyValuePair<string, string>>) keyValuePairList).AbsoluteUri), userState, cancellationToken);
    }

    public Task<IEnumerable<AccessControlList>> QueryAccessControlListsAsync(
      Guid securityNamespaceId,
      string token,
      IEnumerable<IdentityDescriptor> descriptors,
      bool includeExtendedInfo,
      bool recurse,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Uri uri = new Uri(PathUtility.Combine(this.BaseAddress.GetLeftPart(UriPartial.Path), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/_apis/accesscontrollists/{0}", (object) securityNamespaceId)));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (token != null)
        keyValuePairList.Add(nameof (token), token);
      keyValuePairList.Add(nameof (includeExtendedInfo), includeExtendedInfo.ToString());
      keyValuePairList.Add(nameof (recurse), recurse.ToString());
      if (descriptors != null)
        keyValuePairList.AddMultiple<IdentityDescriptor>(nameof (descriptors), descriptors, (Func<IdentityDescriptor, string>) (descriptor => descriptor.ToString()));
      return this.SendAsync<IEnumerable<AccessControlList>>(new HttpRequestMessage(HttpMethod.Get, uri.AppendQuery((IEnumerable<KeyValuePair<string, string>>) keyValuePairList).AbsoluteUri), userState, cancellationToken);
    }

    public Task<HttpResponseMessage> SetAccessControlListsAsync(
      Guid securityNamespaceId,
      IEnumerable<AccessControlList> accessControlLists,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<IEnumerable<AccessControlList>>(accessControlLists, nameof (accessControlLists));
      return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, new Uri(PathUtility.Combine(this.BaseAddress.GetLeftPart(UriPartial.Path), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/_apis/accesscontrollists/{0}", (object) securityNamespaceId))).AbsoluteUri)
      {
        Content = (HttpContent) new ObjectContent<VssJsonCollectionWrapper<AccessControlListsCollection>>(new VssJsonCollectionWrapper<AccessControlListsCollection>((IEnumerable) new AccessControlListsCollection(accessControlLists)), this.Formatter)
      }, userState, cancellationToken);
    }

    public Task<bool> RemoveAccessControlListsAsync(
      Guid securityNamespaceId,
      IEnumerable<string> tokens,
      bool recurse,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Uri uri = new Uri(PathUtility.Combine(this.BaseAddress.GetLeftPart(UriPartial.Path), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/_apis/accesscontrollists/{0}", (object) securityNamespaceId)));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (tokens != null)
        keyValuePairList.AddMultiple(nameof (tokens), tokens);
      keyValuePairList.Add(nameof (recurse), recurse.ToString());
      return this.SendAsync<bool>(new HttpRequestMessage(HttpMethod.Delete, uri.AppendQuery((IEnumerable<KeyValuePair<string, string>>) keyValuePairList).AbsoluteUri), userState, cancellationToken);
    }

    public Task<IEnumerable<SecurityNamespaceDescription>> QuerySecurityNamespacesAsync(
      Guid securityNamespaceId,
      bool localOnly = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Uri uri = new Uri(PathUtility.Combine(this.BaseAddress.GetLeftPart(UriPartial.Path), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/_apis/securitynamespaces/{0}", (object) securityNamespaceId)));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("localonly", localOnly.ToString());
      return this.SendAsync<IEnumerable<SecurityNamespaceDescription>>(new HttpRequestMessage(HttpMethod.Get, uri.AppendQuery((IEnumerable<KeyValuePair<string, string>>) keyValuePairList).AbsoluteUri), userState, cancellationToken);
    }

    public Task<HttpResponseMessage> SetInheritFlagAsync(
      Guid securityNamespaceId,
      string token,
      bool inherit,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      Uri uri = new Uri(PathUtility.Combine(this.BaseAddress.GetLeftPart(UriPartial.Path), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/_apis/securitynamespaces/{0}", (object) securityNamespaceId)));
      SetInheritFlagInfo setInheritFlagInfo = new SetInheritFlagInfo(token, inherit);
      return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, uri.AbsoluteUri)
      {
        Content = (HttpContent) new ObjectContent<SetInheritFlagInfo>(setInheritFlagInfo, this.Formatter)
      }, userState, cancellationToken);
    }
  }
}
