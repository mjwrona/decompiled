// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Client.AccountLicensingHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Licensing.Client
{
  [ResourceArea("B002715C-F261-41EB-ACA3-19292F32B62B")]
  public class AccountLicensingHttpClient : VssHttpClientBase, IAccountLicensingHttpClient
  {
    private static readonly Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>()
    {
      {
        "InvalidRightNameException",
        typeof (InvalidRightNameException)
      },
      {
        "InvalidClientVersionException",
        typeof (InvalidClientVersionException)
      },
      {
        "InvalidClientRightsQueryContextException",
        typeof (InvalidClientRightsQueryContextException)
      },
      {
        "InvalidLicensingOperation",
        typeof (InvalidLicensingOperation)
      }
    };
    protected static readonly Version previewApiVersion = new Version(1, 0);
    protected static readonly Version SearchMemberspreviewApiVersion = new Version(7, 1);

    public AccountLicensingHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public AccountLicensingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public AccountLicensingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public AccountLicensingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public AccountLicensingHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task<IDictionary<string, bool>> ComputeExtensionRightsAsync(
      IEnumerable<string> extensionIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountLicensingHttpClient licensingHttpClient1 = this;
      IDictionary<string, bool> extensionRightsAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "ComputeExtensionRights"))
      {
        ObjectContent<IEnumerable<string>> objectContent = new ObjectContent<IEnumerable<string>>(extensionIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        AccountLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod post = HttpMethod.Post;
        Guid rightsLocationId = AccountLicensingResourceIds.ExtensionRightsLocationId;
        ApiResourceVersion version = new ApiResourceVersion(AccountLicensingHttpClient.previewApiVersion, 1);
        ObjectContent<IEnumerable<string>> content = objectContent;
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        extensionRightsAsync = await licensingHttpClient2.SendAsync<IDictionary<string, bool>>(post, rightsLocationId, version: version, content: (HttpContent) content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return extensionRightsAsync;
    }

    public virtual async Task<ExtensionRightsResult> GetExtensionRightsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountLicensingHttpClient licensingHttpClient1 = this;
      ExtensionRightsResult extensionRightsAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "GetExtensionRights"))
      {
        AccountLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid rightsLocationId = AccountLicensingResourceIds.ExtensionRightsLocationId;
        ApiResourceVersion version = new ApiResourceVersion(AccountLicensingHttpClient.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        extensionRightsAsync = await licensingHttpClient2.SendAsync<ExtensionRightsResult>(get, rightsLocationId, version: version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return extensionRightsAsync;
    }

    public virtual async Task<IEnumerable<AccountLicenseUsage>> GetAccountLicensesUsageAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountLicensingHttpClient licensingHttpClient1 = this;
      IEnumerable<AccountLicenseUsage> licensesUsageAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "GetAccountLicensesUsage"))
      {
        AccountLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid usageLocationId = AccountLicensingResourceIds.UsageLocationId;
        ApiResourceVersion version = new ApiResourceVersion(AccountLicensingHttpClient.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        licensesUsageAsync = await licensingHttpClient2.SendAsync<IEnumerable<AccountLicenseUsage>>(get, usageLocationId, version: version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false) ?? Enumerable.Empty<AccountLicenseUsage>();
      }
      return licensesUsageAsync;
    }

    public virtual async Task<IEnumerable<AccountEntitlement>> GetAccountEntitlementsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountLicensingHttpClient licensingHttpClient1 = this;
      IEnumerable<AccountEntitlement> entitlementsAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "GetAccountEntitlements"))
      {
        AccountLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid entitlementsLocationId = AccountLicensingResourceIds.EntitlementsLocationId;
        ApiResourceVersion version = new ApiResourceVersion(AccountLicensingHttpClient.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        entitlementsAsync = await licensingHttpClient2.SendAsync<IEnumerable<AccountEntitlement>>(get, entitlementsLocationId, version: version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return entitlementsAsync;
    }

    public virtual async Task<IEnumerable<AccountEntitlement>> GetAccountEntitlementsAsync(
      int top,
      int skip = 0,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountLicensingHttpClient licensingHttpClient1 = this;
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add<int>(nameof (top), top);
      collection.Add<int>(nameof (skip), skip);
      List<KeyValuePair<string, string>> keyValuePairList = collection;
      IEnumerable<AccountEntitlement> entitlementsAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "GetAccountEntitlements"))
      {
        AccountLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid entitlementsLocationId = AccountLicensingResourceIds.EntitlementsLocationId;
        ApiResourceVersion version = new ApiResourceVersion(AccountLicensingHttpClient.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        entitlementsAsync = await licensingHttpClient2.SendAsync<IEnumerable<AccountEntitlement>>(get, entitlementsLocationId, version: version, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return entitlementsAsync;
    }

    public virtual async Task<PagedAccountEntitlements> SearchAccountEntitlementsAsync(
      string continuation = null,
      string filter = null,
      string orderBy = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountLicensingHttpClient licensingHttpClient1 = this;
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (continuation != null)
        collection.Add(nameof (continuation), continuation);
      if (filter != null)
        collection.Add("$filter", filter);
      if (orderBy != null)
        collection.Add("$orderBy", orderBy);
      PagedAccountEntitlements accountEntitlements;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "SearchAccountEntitlements"))
      {
        AccountLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid searchLocationId = AccountLicensingResourceIds.EntitlementsSearchLocationId;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(AccountLicensingHttpClient.previewApiVersion, 1);
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        var routeValues = new{ action = "Search" };
        ApiResourceVersion version = apiResourceVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        accountEntitlements = await licensingHttpClient2.SendAsync<PagedAccountEntitlements>(get, searchLocationId, (object) routeValues, version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return accountEntitlements;
    }

    public virtual async Task<PagedAccountEntitlements> SearchMemberAccountEntitlementsAsync(
      string continuation = null,
      string filter = null,
      string orderBy = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountLicensingHttpClient licensingHttpClient1 = this;
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (continuation != null)
        collection.Add(nameof (continuation), continuation);
      if (filter != null)
        collection.Add("$filter", filter);
      if (orderBy != null)
        collection.Add("$orderBy", orderBy);
      PagedAccountEntitlements accountEntitlements;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "SearchAccountEntitlements"))
      {
        AccountLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid searchLocationId = AccountLicensingResourceIds.EntitlementsSearchLocationId;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(AccountLicensingHttpClient.SearchMemberspreviewApiVersion, 2);
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        var routeValues = new{ action = "Search" };
        ApiResourceVersion version = apiResourceVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        accountEntitlements = await licensingHttpClient2.SendAsync<PagedAccountEntitlements>(get, searchLocationId, (object) routeValues, version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return accountEntitlements;
    }

    public virtual async Task<IEnumerable<AccountEntitlement>> GetAccountEntitlementsAsync(
      IList<Guid> userIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountLicensingHttpClient licensingHttpClient1 = this;
      IEnumerable<AccountEntitlement> entitlementsAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "GetAccountEntitlements"))
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) userIds, nameof (userIds));
        ObjectContent<IList<Guid>> objectContent = new ObjectContent<IList<Guid>>(userIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        AccountLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod post = HttpMethod.Post;
        Guid entitlementsBatchLocationId = AccountLicensingResourceIds.UserEntitlementsBatchLocationId;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(AccountLicensingHttpClient.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        var routeValues = new
        {
          action = "GetUsersEntitlements"
        };
        ApiResourceVersion version = apiResourceVersion;
        ObjectContent<IList<Guid>> content = objectContent;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        entitlementsAsync = await licensingHttpClient2.SendAsync<IEnumerable<AccountEntitlement>>(post, entitlementsBatchLocationId, (object) routeValues, version, (HttpContent) content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return entitlementsAsync;
    }

    public virtual async Task<IList<AccountEntitlement>> ObtainAvailableAccountEntitlementsAsync(
      IList<Guid> userIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountLicensingHttpClient licensingHttpClient1 = this;
      IList<AccountEntitlement> entitlementsAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "GetAccountEntitlements"))
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) userIds, nameof (userIds));
        ObjectContent<IList<Guid>> objectContent = new ObjectContent<IList<Guid>>(userIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        AccountLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod post = HttpMethod.Post;
        Guid entitlementsBatchLocationId = AccountLicensingResourceIds.UserEntitlementsBatchLocationId;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(AccountLicensingHttpClient.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        var routeValues = new
        {
          action = "GetAvailableUsersEntitlements"
        };
        ApiResourceVersion version = apiResourceVersion;
        ObjectContent<IList<Guid>> content = objectContent;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        entitlementsAsync = await licensingHttpClient2.SendAsync<IList<AccountEntitlement>>(post, entitlementsBatchLocationId, (object) routeValues, version, (HttpContent) content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return entitlementsAsync;
    }

    public virtual async Task<AccountEntitlement> GetAccountEntitlementAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountLicensingHttpClient licensingHttpClient1 = this;
      AccountEntitlement entitlementAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "GetAccountEntitlement"))
      {
        AccountLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid entitlementsLocationId = AccountLicensingResourceIds.CurrentUserEntitlementsLocationId;
        ApiResourceVersion version = new ApiResourceVersion(AccountLicensingHttpClient.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        entitlementAsync = await licensingHttpClient2.SendAsync<AccountEntitlement>(get, entitlementsLocationId, version: version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return entitlementAsync;
    }

    public virtual Task<AccountEntitlement> GetAccountEntitlementAsync(
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetAccountEntitlementAsync(userId, (List<KeyValuePair<string, string>>) null, userState, cancellationToken);
    }

    public virtual Task<AccountEntitlement> GetAccountEntitlementAsync(
      Guid userId,
      bool determineRights,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add<bool>(nameof (determineRights), determineRights);
      List<KeyValuePair<string, string>> queryParams = collection;
      return this.GetAccountEntitlementAsync(userId, queryParams, userState, cancellationToken);
    }

    public virtual Task<AccountEntitlement> GetAccountEntitlementAsync(
      Guid userId,
      bool determineRights,
      bool createIfNotExists,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add<bool>(nameof (determineRights), determineRights);
      collection.Add<bool>(nameof (createIfNotExists), createIfNotExists);
      List<KeyValuePair<string, string>> queryParams = collection;
      return this.GetAccountEntitlementAsync(userId, queryParams, userState, cancellationToken);
    }

    private async Task<AccountEntitlement> GetAccountEntitlementAsync(
      Guid userId,
      List<KeyValuePair<string, string>> queryParams,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountLicensingHttpClient licensingHttpClient1 = this;
      AccountEntitlement entitlementAsync;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "GetAccountEntitlement"))
      {
        AccountLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid entitlementsLocationId = AccountLicensingResourceIds.UserEntitlementsLocationId;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(AccountLicensingHttpClient.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        var routeValues = new{ userId = userId };
        ApiResourceVersion version = apiResourceVersion;
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = queryParams;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        entitlementAsync = await licensingHttpClient2.SendAsync<AccountEntitlement>(get, entitlementsLocationId, (object) routeValues, version, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return entitlementAsync;
    }

    public virtual async Task<AccountEntitlement> AssignEntitlementAsync(
      Guid userId,
      License license,
      bool dontNotifyUser = false,
      LicensingOrigin origin = LicensingOrigin.None,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountLicensingHttpClient licensingHttpClient1 = this;
      AccountEntitlement accountEntitlement;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "AssignEntitlement"))
      {
        ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
        ArgumentUtility.CheckForNull<License>(license, nameof (license));
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add<bool>(nameof (dontNotifyUser), dontNotifyUser);
        collection.Add<LicensingOrigin>(nameof (origin), origin);
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        AccountLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod put = HttpMethod.Put;
        Guid entitlementsLocationId = AccountLicensingResourceIds.UserEntitlementsLocationId;
        var routeValues = new{ userId = userId };
        HttpContent contentFor = (HttpContent) licensingHttpClient1.CreateContentFor<AccountEntitlementUpdateModel>(new AccountEntitlementUpdateModel()
        {
          License = license
        });
        ApiResourceVersion version = new ApiResourceVersion(AccountLicensingHttpClient.previewApiVersion, 1);
        HttpContent content = contentFor;
        CancellationToken cancellationToken1 = cancellationToken;
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        accountEntitlement = await licensingHttpClient2.SendAsync<AccountEntitlement>(put, entitlementsLocationId, (object) routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState1, cancellationToken2).ConfigureAwait(false);
      }
      return accountEntitlement;
    }

    public virtual async Task<AccountEntitlement> AssignAvailableEntitlementAsync(
      Guid userId,
      bool dontNotifyUser = false,
      LicensingOrigin origin = LicensingOrigin.None,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountLicensingHttpClient licensingHttpClient1 = this;
      AccountEntitlement accountEntitlement;
      using (new VssHttpClientBase.OperationScope("AccountLicensing", "AssignEntitlement"))
      {
        ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add<bool>(nameof (dontNotifyUser), dontNotifyUser);
        collection.Add<LicensingOrigin>(nameof (origin), origin);
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        AccountLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod put = HttpMethod.Put;
        Guid entitlementsLocationId = AccountLicensingResourceIds.UserEntitlementsLocationId;
        var routeValues = new{ userId = userId };
        HttpContent contentFor = (HttpContent) licensingHttpClient1.CreateContentFor<AccountEntitlementUpdateModel>(new AccountEntitlementUpdateModel()
        {
          License = License.Auto
        });
        ApiResourceVersion version = new ApiResourceVersion(AccountLicensingHttpClient.previewApiVersion, 1);
        HttpContent content = contentFor;
        CancellationToken cancellationToken1 = cancellationToken;
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        accountEntitlement = await licensingHttpClient2.SendAsync<AccountEntitlement>(put, entitlementsLocationId, (object) routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState1, cancellationToken2).ConfigureAwait(false);
      }
      return accountEntitlement;
    }

    public virtual async Task DeleteEntitlementAsync(
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountLicensingHttpClient licensingHttpClient1 = this;
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      AccountLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
      HttpMethod delete = HttpMethod.Delete;
      Guid entitlementsLocationId = AccountLicensingResourceIds.UserEntitlementsLocationId;
      var routeValues = new{ userId = userId };
      ApiResourceVersion version = new ApiResourceVersion(AccountLicensingHttpClient.previewApiVersion, 1);
      CancellationToken cancellationToken1 = cancellationToken;
      object obj = userState;
      List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      HttpResponseMessage httpResponseMessage = await licensingHttpClient2.SendAsync(delete, entitlementsLocationId, (object) routeValues, version, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
    }

    public virtual async Task TransferIdentityRightsAsync(
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap,
      bool? validateOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountLicensingHttpClient licensingHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8671b016-fa74-4c88-b693-83bbb88c2264");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<KeyValuePair<Guid, Guid>>>(userIdTransferMap, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (validateOnly.HasValue)
        collection.Add(nameof (validateOnly), validateOnly.Value.ToString());
      AccountLicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(AccountLicensingHttpClient.previewApiVersion, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await licensingHttpClient2.SendAsync(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    protected override async Task<HttpRequestMessage> CreateRequestMessageAsync(
      HttpMethod method,
      IEnumerable<KeyValuePair<string, string>> additionalHeaders,
      Guid locationId,
      object routeValues,
      ApiResourceVersion version,
      HttpContent content,
      IEnumerable<KeyValuePair<string, string>> queryParameters,
      object userState,
      CancellationToken cancellationToken,
      string mediaType)
    {
      HttpRequestMessage requestMessageAsync = await base.CreateRequestMessageAsync(method, additionalHeaders, locationId, routeValues, version, content, queryParameters, userState, cancellationToken, mediaType).ConfigureAwait(false);
      requestMessageAsync.Properties["VssHttpRetryOptions"] = (object) new VssHttpRetryOptions()
      {
        MinBackoff = TimeSpan.FromMilliseconds(300.0),
        MaxBackoff = TimeSpan.FromSeconds(3.0),
        MaxRetries = 1
      };
      return requestMessageAsync;
    }

    protected ObjectContent<T> CreateContentFor<T>(T value) => new ObjectContent<T>(value, this.Formatter);

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) AccountLicensingHttpClient.s_translatedExceptions;
  }
}
