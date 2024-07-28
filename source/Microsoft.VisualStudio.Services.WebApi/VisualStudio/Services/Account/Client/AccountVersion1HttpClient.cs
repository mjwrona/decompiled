// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Account.Client.AccountVersion1HttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Account.Client
{
  [ResourceArea("0D55247A-1C47-4462-9B1F-5E2125590EE6")]
  public class AccountVersion1HttpClient : VssHttpClientBase
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();
    protected ApiResourceVersion CurrentApiVersion;
    private const double c_apiVersion = 1.0;

    static AccountVersion1HttpClient()
    {
      AccountVersion1HttpClient.s_translatedExceptions.Add("AccountExistsException", typeof (AccountExistsException));
      AccountVersion1HttpClient.s_translatedExceptions.Add("AccountNotFoundException", typeof (AccountNotFoundException));
      AccountVersion1HttpClient.s_translatedExceptions.Add("MaxNumberAccountsPerUserException", typeof (MaxNumberAccountsPerUserException));
      AccountVersion1HttpClient.s_translatedExceptions.Add("MaxNumberAccountsException", typeof (MaxNumberAccountsException));
      AccountVersion1HttpClient.s_translatedExceptions.Add("AccountPropertyException", typeof (AccountPropertyException));
      AccountVersion1HttpClient.s_translatedExceptions.Add("IdentityNotFoundException", typeof (IdentityNotFoundException));
      AccountVersion1HttpClient.s_translatedExceptions.Add("AccountUserNotFoundException", typeof (AccountUserNotFoundException));
    }

    internal AccountVersion1HttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
      this.CurrentApiVersion = new ApiResourceVersion(1.0);
    }

    internal AccountVersion1HttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
      this.CurrentApiVersion = new ApiResourceVersion(1.0);
    }

    internal AccountVersion1HttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
      this.CurrentApiVersion = new ApiResourceVersion(1.0);
    }

    internal AccountVersion1HttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
      this.CurrentApiVersion = new ApiResourceVersion(1.0);
    }

    internal AccountVersion1HttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
      this.CurrentApiVersion = new ApiResourceVersion(1.0);
    }

    [Obsolete("Please use appropriate method on Microsoft.VisualStudio.Services.Organization.Client.OrganizationHttpClient instead. See https://dev.azure.com/mseng/AzureDevOps/_wiki/wikis/AzureDevOps.wiki/1931/SDK-M113?anchor=accountservice-obsolescence for more details.")]
    public async Task<Microsoft.VisualStudio.Services.Account.Account> CreateAccountAsync(
      string name,
      string organization,
      Guid creatorId,
      CultureInfo language = null,
      CultureInfo culture = null,
      TimeZoneInfo timeZone = null,
      IDictionary<string, object> properties = null,
      bool usePrecreated = false,
      List<KeyValuePair<Guid, Guid>> serviceDefinitions = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountVersion1HttpClient version1HttpClient1 = this;
      Microsoft.VisualStudio.Services.Account.Account accountAsync;
      using (new VssHttpClientBase.OperationScope("Account", "CreateAccount"))
      {
        AccountCreateInfoInternal createInfoInternal1 = new AccountCreateInfoInternal(name, organization, creatorId, serviceDefinitions, language, culture, timeZone, properties);
        AccountVersion1HttpClient version1HttpClient2 = version1HttpClient1;
        bool? nullable = new bool?(usePrecreated);
        Guid? creatorId1 = new Guid?();
        Guid? ownerId = new Guid?();
        Guid? memberId = new Guid?();
        Guid? accountId = new Guid?();
        bool? includeOwner = new bool?(false);
        bool? includeDisabledAccounts = new bool?(false);
        bool? includeDeletedUsers = new bool?(false);
        bool? usePrecreated1 = nullable;
        List<KeyValuePair<string, string>> keyValuePairList = version1HttpClient2.AppendQueryString(creatorId1, ownerId, memberId, accountId, includeOwner, includeDisabledAccounts, includeDeletedUsers, usePrecreated: usePrecreated1);
        AccountVersion1HttpClient version1HttpClient3 = version1HttpClient1;
        AccountCreateInfoInternal createInfoInternal2 = createInfoInternal1;
        Guid account = AccountResourceIds.Account;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
        ApiResourceVersion currentApiVersion = version1HttpClient1.CurrentApiVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        accountAsync = await version1HttpClient3.PostAsync<AccountCreateInfoInternal, Microsoft.VisualStudio.Services.Account.Account>(createInfoInternal2, account, version: currentApiVersion, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return accountAsync;
    }

    [Obsolete("Please use appropriate method on Microsoft.VisualStudio.Services.UserMapping.Client.UserMappingHttpClient instead. See https://dev.azure.com/mseng/AzureDevOps/_wiki/wikis/AzureDevOps.wiki/1930/SDK-M112?anchor=accountservice-obsolescence for more details.")]
    public async Task<List<Microsoft.VisualStudio.Services.Account.Account>> GetAccountsAsync(
      bool includeDisabledAccounts = false,
      IEnumerable<string> propertyNameFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountVersion1HttpClient version1HttpClient1 = this;
      List<Microsoft.VisualStudio.Services.Account.Account> accountsAsync;
      using (new VssHttpClientBase.OperationScope("Account", "GetAccounts"))
      {
        AccountVersion1HttpClient version1HttpClient2 = version1HttpClient1;
        bool? nullable = new bool?(includeDisabledAccounts);
        IEnumerable<string> strings = propertyNameFilter;
        Guid? creatorId = new Guid?();
        Guid? ownerId = new Guid?();
        Guid? memberId = new Guid?();
        Guid? accountId = new Guid?();
        bool? includeOwner = new bool?(false);
        bool? includeDisabledAccounts1 = nullable;
        bool? includeDeletedUsers = new bool?(false);
        IEnumerable<string> propertyNames = strings;
        bool? usePrecreated = new bool?(false);
        List<KeyValuePair<string, string>> queryParameters = version1HttpClient2.AppendQueryString(creatorId, ownerId, memberId, accountId, includeOwner, includeDisabledAccounts1, includeDeletedUsers, propertyNames, usePrecreated);
        accountsAsync = await version1HttpClient1.GetAsync<List<Microsoft.VisualStudio.Services.Account.Account>>(AccountResourceIds.Account, version: version1HttpClient1.CurrentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return accountsAsync;
    }

    [Obsolete("Please use appropriate method on Microsoft.VisualStudio.Services.Organization.Client.OrganizationHttpClient instead. See https://dev.azure.com/mseng/AzureDevOps/_wiki/wikis/AzureDevOps.wiki/1931/SDK-M113?anchor=accountservice-obsolescence for more details.")]
    internal async Task<List<Microsoft.VisualStudio.Services.Account.Account>> GetAccountsByCreatorAsync(
      Guid creatorId,
      bool includeDisabledAccounts = false,
      IEnumerable<string> propertyNameFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountVersion1HttpClient version1HttpClient1 = this;
      List<Microsoft.VisualStudio.Services.Account.Account> accountsByCreatorAsync;
      using (new VssHttpClientBase.OperationScope("Account", "GetAccountsByCreator"))
      {
        ArgumentUtility.CheckForEmptyGuid(creatorId, nameof (creatorId));
        AccountVersion1HttpClient version1HttpClient2 = version1HttpClient1;
        Guid? creatorId1 = new Guid?(creatorId);
        bool? nullable = new bool?(includeDisabledAccounts);
        IEnumerable<string> strings = propertyNameFilter;
        Guid? ownerId = new Guid?();
        Guid? memberId = new Guid?();
        Guid? accountId = new Guid?();
        bool? includeOwner = new bool?(false);
        bool? includeDisabledAccounts1 = nullable;
        bool? includeDeletedUsers = new bool?(false);
        IEnumerable<string> propertyNames = strings;
        bool? usePrecreated = new bool?(false);
        List<KeyValuePair<string, string>> queryParameters = version1HttpClient2.AppendQueryString(creatorId1, ownerId, memberId, accountId, includeOwner, includeDisabledAccounts1, includeDeletedUsers, propertyNames, usePrecreated);
        accountsByCreatorAsync = await version1HttpClient1.GetAsync<List<Microsoft.VisualStudio.Services.Account.Account>>(AccountResourceIds.Account, version: version1HttpClient1.CurrentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return accountsByCreatorAsync;
    }

    public async Task<List<Microsoft.VisualStudio.Services.Account.Account>> GetAccountsByOwnerAsync(
      Guid ownerId,
      bool includeDisabledAccounts = false,
      IEnumerable<string> propertyNameFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountVersion1HttpClient version1HttpClient1 = this;
      List<Microsoft.VisualStudio.Services.Account.Account> accountsByOwnerAsync;
      using (new VssHttpClientBase.OperationScope("Account", "GetAccountsByOwner"))
      {
        ArgumentUtility.CheckForEmptyGuid(ownerId, nameof (ownerId));
        AccountVersion1HttpClient version1HttpClient2 = version1HttpClient1;
        Guid? nullable1 = new Guid?(ownerId);
        bool? nullable2 = new bool?(includeDisabledAccounts);
        IEnumerable<string> strings = propertyNameFilter;
        Guid? creatorId = new Guid?();
        Guid? ownerId1 = nullable1;
        Guid? memberId = new Guid?();
        Guid? accountId = new Guid?();
        bool? includeOwner = new bool?(false);
        bool? includeDisabledAccounts1 = nullable2;
        bool? includeDeletedUsers = new bool?(false);
        IEnumerable<string> propertyNames = strings;
        bool? usePrecreated = new bool?(false);
        List<KeyValuePair<string, string>> queryParameters = version1HttpClient2.AppendQueryString(creatorId, ownerId1, memberId, accountId, includeOwner, includeDisabledAccounts1, includeDeletedUsers, propertyNames, usePrecreated);
        accountsByOwnerAsync = await version1HttpClient1.GetAsync<List<Microsoft.VisualStudio.Services.Account.Account>>(AccountResourceIds.Account, version: version1HttpClient1.CurrentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return accountsByOwnerAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal Task<List<Microsoft.VisualStudio.Services.Account.Account>> GetAccountsByMemberAsync(
      Guid memberId,
      bool includeOwner,
      bool includeDisabledAccounts,
      IEnumerable<string> propertyNameFilter,
      object userState)
    {
      return this.GetAccountsByMemberAsync(memberId, includeOwner, includeDisabledAccounts, propertyNameFilter, userState, new CancellationToken());
    }

    public async Task<List<Microsoft.VisualStudio.Services.Account.Account>> GetAccountsByMemberAsync(
      Guid memberId,
      bool includeOwner = true,
      bool includeDisabledAccounts = false,
      IEnumerable<string> propertyNameFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountVersion1HttpClient version1HttpClient1 = this;
      List<Microsoft.VisualStudio.Services.Account.Account> accountsByMemberAsync;
      using (new VssHttpClientBase.OperationScope("Account", "GetAccountsByMember"))
      {
        ArgumentUtility.CheckForEmptyGuid(memberId, nameof (memberId));
        AccountVersion1HttpClient version1HttpClient2 = version1HttpClient1;
        Guid? nullable1 = new Guid?(memberId);
        bool? nullable2 = new bool?(includeOwner);
        bool? nullable3 = new bool?(includeDisabledAccounts);
        IEnumerable<string> strings = propertyNameFilter;
        Guid? creatorId = new Guid?();
        Guid? ownerId = new Guid?();
        Guid? memberId1 = nullable1;
        Guid? accountId = new Guid?();
        bool? includeOwner1 = nullable2;
        bool? includeDisabledAccounts1 = nullable3;
        bool? includeDeletedUsers = new bool?(false);
        IEnumerable<string> propertyNames = strings;
        bool? usePrecreated = new bool?(false);
        List<KeyValuePair<string, string>> queryParameters = version1HttpClient2.AppendQueryString(creatorId, ownerId, memberId1, accountId, includeOwner1, includeDisabledAccounts1, includeDeletedUsers, propertyNames, usePrecreated);
        accountsByMemberAsync = await version1HttpClient1.GetAsync<List<Microsoft.VisualStudio.Services.Account.Account>>(AccountResourceIds.Account, version: version1HttpClient1.CurrentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return accountsByMemberAsync;
    }

    [Obsolete("Please use appropriate method on Microsoft.VisualStudio.Services.Organization.Client.OrganizationHttpClient instead. See https://dev.azure.com/mseng/AzureDevOps/_wiki/wikis/AzureDevOps.wiki/1931/SDK-M113?anchor=accountservice-obsolescence for more details.")]
    public async Task<Microsoft.VisualStudio.Services.Account.Account> GetAccountAsync(
      string accountId,
      IEnumerable<string> propertyNameFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountVersion1HttpClient version1HttpClient1 = this;
      Microsoft.VisualStudio.Services.Account.Account accountAsync;
      using (new VssHttpClientBase.OperationScope("Account", "GetAccount"))
      {
        AccountVersion1HttpClient version1HttpClient2 = version1HttpClient1;
        IEnumerable<string> strings = propertyNameFilter;
        Guid? creatorId = new Guid?();
        Guid? ownerId = new Guid?();
        Guid? memberId = new Guid?();
        Guid? accountId1 = new Guid?();
        bool? includeOwner = new bool?(false);
        bool? includeDisabledAccounts = new bool?(false);
        bool? includeDeletedUsers = new bool?(false);
        IEnumerable<string> propertyNames = strings;
        bool? usePrecreated = new bool?(false);
        List<KeyValuePair<string, string>> queryParameters = version1HttpClient2.AppendQueryString(creatorId, ownerId, memberId, accountId1, includeOwner, includeDisabledAccounts, includeDeletedUsers, propertyNames, usePrecreated);
        accountAsync = await version1HttpClient1.GetAsync<Microsoft.VisualStudio.Services.Account.Account>(AccountResourceIds.Account, (object) new
        {
          accountId = accountId
        }, version1HttpClient1.CurrentApiVersion, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState, cancellationToken).ConfigureAwait(false);
      }
      return accountAsync;
    }

    [Obsolete("Please use appropriate method on Microsoft.VisualStudio.Services.Organization.Client.OrganizationHttpClient instead. See https://dev.azure.com/mseng/AzureDevOps/_wiki/wikis/AzureDevOps.wiki/1931/SDK-M113?anchor=accountservice-obsolescence for more details.")]
    public Microsoft.VisualStudio.Services.Account.Account GetAccount(
      string accountId,
      IEnumerable<string> propertyNameFilter = null,
      object userState = null)
    {
      try
      {
        return this.GetAccountAsync(accountId, propertyNameFilter, userState).SyncResult<Microsoft.VisualStudio.Services.Account.Account>();
      }
      catch (AccountNotFoundException ex)
      {
        return (Microsoft.VisualStudio.Services.Account.Account) null;
      }
      catch (VssServiceResponseException ex)
      {
        if (ex.HttpStatusCode == HttpStatusCode.NotFound)
          return (Microsoft.VisualStudio.Services.Account.Account) null;
        throw;
      }
    }

    [Obsolete("Please use appropriate method on Microsoft.VisualStudio.Services.Organization.Client.OrganizationHttpClient instead. See https://dev.azure.com/mseng/AzureDevOps/_wiki/wikis/AzureDevOps.wiki/1931/SDK-M113?anchor=accountservice-obsolescence for more details.")]
    public async Task<IEnumerable<AccountRegion>> GetRegionsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountVersion1HttpClient version1HttpClient1 = this;
      IEnumerable<AccountRegion> regionsAsync;
      using (new VssHttpClientBase.OperationScope("Account", "GetRegions"))
      {
        AccountVersion1HttpClient version1HttpClient2 = version1HttpClient1;
        Guid regionLocationId = AccountResourceIds.AccountRegionLocationId;
        object obj = userState;
        ApiResourceVersion currentApiVersion = version1HttpClient1.CurrentApiVersion;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        regionsAsync = await version1HttpClient2.GetAsync<IEnumerable<AccountRegion>>(regionLocationId, version: currentApiVersion, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return regionsAsync;
    }

    [Obsolete("Please use appropriate method on Microsoft.VisualStudio.Services.Organization.Client.OrganizationHttpClient instead. See https://dev.azure.com/mseng/AzureDevOps/_wiki/wikis/AzureDevOps.wiki/1931/SDK-M113?anchor=accountservice-obsolescence for more details.")]
    public async Task<AccountNameAvailability> GetAccountNameAvailabilityAsync(
      string accountName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountVersion1HttpClient version1HttpClient = this;
      AccountNameAvailability availabilityAsync;
      using (new VssHttpClientBase.OperationScope("Account", "IsValidAccountName"))
      {
        ArgumentUtility.CheckStringForNullOrEmpty(accountName, nameof (accountName));
        availabilityAsync = await version1HttpClient.GetAsync<AccountNameAvailability>(AccountResourceIds.AccountNameAvailabilityid, (object) new
        {
          accountName = accountName
        }, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return availabilityAsync;
    }

    [Obsolete("Please use appropriate method on Microsoft.VisualStudio.Services.Organization.Client.OrganizationHttpClient instead. See https://dev.azure.com/mseng/AzureDevOps/_wiki/wikis/AzureDevOps.wiki/1931/SDK-M113?anchor=accountservice-obsolescence for more details.")]
    public async Task<IDictionary<string, string>> GetAccountSettingsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountVersion1HttpClient version1HttpClient = this;
      IDictionary<string, string> accountSettingsAsync;
      using (new VssHttpClientBase.OperationScope("Account", nameof (GetAccountSettingsAsync)))
        accountSettingsAsync = await version1HttpClient.GetAsync<IDictionary<string, string>>(AccountResourceIds.AccountSettingsid, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return accountSettingsAsync;
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) AccountVersion1HttpClient.s_translatedExceptions;

    protected List<KeyValuePair<string, string>> AppendQueryString(
      Guid? creatorId = null,
      Guid? ownerId = null,
      Guid? memberId = null,
      Guid? accountId = null,
      bool? includeOwner = false,
      bool? includeDisabledAccounts = false,
      bool? includeDeletedUsers = false,
      IEnumerable<string> propertyNames = null,
      bool? usePrecreated = false,
      string statusReason = null)
    {
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (creatorId.HasValue)
        collection.Add(nameof (creatorId), creatorId.ToString());
      if (ownerId.HasValue)
        collection.Add(nameof (ownerId), ownerId.ToString());
      if (memberId.HasValue)
        collection.Add(nameof (memberId), memberId.ToString());
      if (accountId.HasValue)
        collection.Add(nameof (accountId), accountId.ToString());
      if (statusReason != null)
        collection.Add(nameof (statusReason), statusReason);
      if (usePrecreated.GetValueOrDefault())
        collection.Add(nameof (usePrecreated), "true");
      if (includeOwner.HasValue)
        collection.Add(nameof (includeOwner), includeOwner.Value.ToString());
      if (includeDisabledAccounts.GetValueOrDefault())
        collection.Add(nameof (includeDisabledAccounts), "true");
      if (includeDeletedUsers.GetValueOrDefault())
        collection.Add(nameof (includeDeletedUsers), "true");
      if (propertyNames != null)
        collection.AddMultiple("properties", propertyNames);
      return collection;
    }
  }
}
