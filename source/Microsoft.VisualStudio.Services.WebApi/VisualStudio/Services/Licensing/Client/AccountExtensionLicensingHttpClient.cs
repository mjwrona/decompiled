// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Client.AccountExtensionLicensingHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Licensing.Client
{
  [ResourceArea("B002715C-F261-41EB-ACA3-19292F32B62B")]
  public class AccountExtensionLicensingHttpClient : 
    VssHttpClientBase,
    IAccountExtensionLicensingHttpClient
  {
    protected static readonly Version currentApiVersion = new Version(3, 1);

    public AccountExtensionLicensingHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public AccountExtensionLicensingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public AccountExtensionLicensingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public AccountExtensionLicensingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public AccountExtensionLicensingHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task TransferExtensionsForIdentitiesAsync(
      IList<IdentityMapping> identityMapping,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountExtensionLicensingHttpClient licensingHttpClient1 = this;
      ArgumentUtility.CheckForNull<IList<IdentityMapping>>(identityMapping, nameof (identityMapping));
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "TransferExtensionsForIdentities"))
      {
        ObjectContent<IList<IdentityMapping>> objectContent = new ObjectContent<IList<IdentityMapping>>(identityMapping, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        AccountExtensionLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod post = HttpMethod.Post;
        Guid extensionsLocationId = AccountLicensingResourceIds.TransferIdentitiesExtensionsLocationId;
        ApiResourceVersion version = new ApiResourceVersion(AccountExtensionLicensingHttpClient.currentApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        ObjectContent<IList<IdentityMapping>> content = objectContent;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        HttpResponseMessage httpResponseMessage = await licensingHttpClient2.SendAsync(post, extensionsLocationId, version: version, content: (HttpContent) content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
    }

    public virtual async Task<ICollection<ExtensionOperationResult>> AssignExtensionToUsersAsync(
      string extensionId,
      IList<Guid> userIds,
      bool isAutoAssignment = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountExtensionLicensingHttpClient licensingHttpClient1 = this;
      ArgumentUtility.CheckStringForNullOrEmpty(extensionId, nameof (extensionId));
      ArgumentUtility.CheckForNull<IList<Guid>>(userIds, nameof (userIds));
      ICollection<ExtensionOperationResult> usersAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "AssignExtensionToUsers"))
      {
        ObjectContent<ExtensionAssignment> objectContent = new ObjectContent<ExtensionAssignment>(new ExtensionAssignment()
        {
          ExtensionGalleryId = extensionId,
          UserIds = userIds,
          IsAutoAssignment = isAutoAssignment
        }, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        AccountExtensionLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod put = HttpMethod.Put;
        Guid entitlementsLocationId = AccountLicensingResourceIds.UserExtensionEntitlementsLocationId;
        ApiResourceVersion version = new ApiResourceVersion(AccountExtensionLicensingHttpClient.currentApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        ObjectContent<ExtensionAssignment> content = objectContent;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        usersAsync = await licensingHttpClient2.SendAsync<ICollection<ExtensionOperationResult>>(put, entitlementsLocationId, version: version, content: (HttpContent) content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return usersAsync;
    }

    public virtual async Task<IDictionary<string, LicensingSource>> GetExtensionsAssignedToUserAsync(
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountExtensionLicensingHttpClient licensingHttpClient1 = this;
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      IDictionary<string, LicensingSource> assignedToUserAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "GetExtensionsAssignedToUser"))
      {
        AccountExtensionLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid entitlementsLocationId = AccountLicensingResourceIds.UserExtensionEntitlementsLocationId;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(AccountExtensionLicensingHttpClient.currentApiVersion, 1);
        var routeValues = new{ userId = userId };
        ApiResourceVersion version = apiResourceVersion;
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        assignedToUserAsync = await licensingHttpClient2.SendAsync<IDictionary<string, LicensingSource>>(get, entitlementsLocationId, (object) routeValues, version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return assignedToUserAsync;
    }

    public virtual async Task<IDictionary<Guid, IList<ExtensionSource>>> BulkGetExtensionsAssignedToUsersAsync(
      IList<Guid> userIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountExtensionLicensingHttpClient licensingHttpClient1 = this;
      ArgumentUtility.CheckForNull<IList<Guid>>(userIds, nameof (userIds));
      IDictionary<Guid, IList<ExtensionSource>> assignedToUsersAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "GetExtensionsAssignedToUsers"))
      {
        ObjectContent<IList<Guid>> objectContent = new ObjectContent<IList<Guid>>(userIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        AccountExtensionLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod put = HttpMethod.Put;
        Guid entitlementsLocationId = AccountLicensingResourceIds.UsersBatchExtensionEntitlementsLocationId;
        ApiResourceVersion version = new ApiResourceVersion(AccountExtensionLicensingHttpClient.currentApiVersion, 2);
        CancellationToken cancellationToken1 = cancellationToken;
        ObjectContent<IList<Guid>> content = objectContent;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        assignedToUsersAsync = await licensingHttpClient2.SendAsync<IDictionary<Guid, IList<ExtensionSource>>>(put, entitlementsLocationId, version: version, content: (HttpContent) content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return assignedToUsersAsync;
    }

    public virtual async Task<IDictionary<Guid, IList<string>>> GetExtensionsAssignedToUsersBatchAsync(
      IList<Guid> userIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountExtensionLicensingHttpClient licensingHttpClient1 = this;
      ArgumentUtility.CheckForNull<IList<Guid>>(userIds, nameof (userIds));
      IDictionary<Guid, IList<string>> toUsersBatchAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "GetExtensionsAssignedToUsers"))
      {
        ObjectContent<IList<Guid>> objectContent = new ObjectContent<IList<Guid>>(userIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        AccountExtensionLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod put = HttpMethod.Put;
        Guid entitlementsLocationId = AccountLicensingResourceIds.UsersBatchExtensionEntitlementsLocationId;
        ApiResourceVersion version = new ApiResourceVersion(AccountExtensionLicensingHttpClient.currentApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        ObjectContent<IList<Guid>> content = objectContent;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        toUsersBatchAsync = await licensingHttpClient2.SendAsync<IDictionary<Guid, IList<string>>>(put, entitlementsLocationId, version: version, content: (HttpContent) content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return toUsersBatchAsync;
    }

    public virtual async Task<IDictionary<Guid, ExtensionAssignmentDetails>> GetExtensionStatusForUsersAsync(
      string extensionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountExtensionLicensingHttpClient licensingHttpClient1 = this;
      ArgumentUtility.CheckStringForNullOrEmpty(extensionId, nameof (extensionId));
      IDictionary<Guid, ExtensionAssignmentDetails> statusForUsersAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "GetExtensionStatusForUsers"))
      {
        AccountExtensionLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid entitlementsLocationId = AccountLicensingResourceIds.ExtensionEntitlementsLocationId;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(AccountExtensionLicensingHttpClient.currentApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        var routeValues = new{ extensionId = extensionId };
        ApiResourceVersion version = apiResourceVersion;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        statusForUsersAsync = await licensingHttpClient2.SendAsync<IDictionary<Guid, ExtensionAssignmentDetails>>(get, entitlementsLocationId, (object) routeValues, version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return statusForUsersAsync;
    }

    public virtual async Task<IEnumerable<AccountLicenseExtensionUsage>> GetExtensionLicenseUsageAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountExtensionLicensingHttpClient licensingHttpClient1 = this;
      IEnumerable<AccountLicenseExtensionUsage> licenseUsageAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "GetExtensionsAssignedToUser"))
      {
        AccountExtensionLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid accountLocationId = AccountLicensingResourceIds.ExtensionsAssignedToAccountLocationId;
        ApiResourceVersion version = new ApiResourceVersion(AccountExtensionLicensingHttpClient.currentApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        licenseUsageAsync = await licensingHttpClient2.SendAsync<IEnumerable<AccountLicenseExtensionUsage>>(get, accountLocationId, version: version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return licenseUsageAsync;
    }

    public virtual async Task<ICollection<ExtensionOperationResult>> UnassignExtensionFromUsersAsync(
      string extensionId,
      IList<Guid> userIds,
      LicensingSource source,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountExtensionLicensingHttpClient licensingHttpClient1 = this;
      ArgumentUtility.CheckStringForNullOrEmpty(extensionId, nameof (extensionId));
      ArgumentUtility.CheckForNull<IList<Guid>>(userIds, nameof (userIds));
      ArgumentUtility.CheckForDefinedEnum<LicensingSource>(source, nameof (source));
      ICollection<ExtensionOperationResult> extensionOperationResults;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "UnassignExtensionFromUsers"))
      {
        ObjectContent<ExtensionAssignment> objectContent = new ObjectContent<ExtensionAssignment>(new ExtensionAssignment()
        {
          ExtensionGalleryId = extensionId,
          UserIds = userIds,
          LicensingSource = source
        }, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        AccountExtensionLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod delete = HttpMethod.Delete;
        Guid entitlementsLocationId = AccountLicensingResourceIds.UserExtensionEntitlementsLocationId;
        ApiResourceVersion version = new ApiResourceVersion(AccountExtensionLicensingHttpClient.currentApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        ObjectContent<ExtensionAssignment> content = objectContent;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        extensionOperationResults = await licensingHttpClient2.SendAsync<ICollection<ExtensionOperationResult>>(delete, entitlementsLocationId, version: version, content: (HttpContent) content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return extensionOperationResults;
    }

    public virtual async Task<ICollection<ExtensionOperationResult>> AssignExtensionToAllEligibleUsersAsync(
      string extensionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountExtensionLicensingHttpClient licensingHttpClient1 = this;
      ArgumentUtility.CheckStringForNullOrEmpty(extensionId, nameof (extensionId));
      ICollection<ExtensionOperationResult> eligibleUsersAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "AssignExtensionToAllEligibleUsers"))
      {
        AccountExtensionLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod put = HttpMethod.Put;
        Guid entitlementsLocationId = AccountLicensingResourceIds.ExtensionEntitlementsLocationId;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(AccountExtensionLicensingHttpClient.currentApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        var routeValues = new{ extensionId = extensionId };
        ApiResourceVersion version = apiResourceVersion;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        eligibleUsersAsync = await licensingHttpClient2.SendAsync<ICollection<ExtensionOperationResult>>(put, entitlementsLocationId, (object) routeValues, version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return eligibleUsersAsync;
    }

    public virtual async Task<IList<Guid>> GetEligibleUsersForExtensionAsync(
      string extensionId,
      ExtensionFilterOptions options = ExtensionFilterOptions.None,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountExtensionLicensingHttpClient licensingHttpClient1 = this;
      ArgumentUtility.CheckStringForNullOrEmpty(extensionId, nameof (extensionId));
      IList<Guid> forExtensionAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "GetEligibleUsersForExtension"))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add<ExtensionFilterOptions>(nameof (options), options);
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        AccountExtensionLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid entitlementsLocationId = AccountLicensingResourceIds.ExtensionEntitlementsLocationId;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(AccountExtensionLicensingHttpClient.currentApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
        var routeValues = new{ extensionId = extensionId };
        ApiResourceVersion version = apiResourceVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        forExtensionAsync = await licensingHttpClient2.SendAsync<IList<Guid>>(get, entitlementsLocationId, (object) routeValues, version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return forExtensionAsync;
    }
  }
}
