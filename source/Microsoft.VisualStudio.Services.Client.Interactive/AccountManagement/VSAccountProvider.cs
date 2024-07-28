// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.VSAccountProvider
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Services.Client.AccountManagement.Logging;
using Microsoft.VisualStudio.Services.Client.Keychain;
using Microsoft.VisualStudio.Services.Client.Keychain.VSProvider;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.TokenStorage;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public class VSAccountProvider : IAccountProvider
  {
    private const string EmailAddressKeyName = "EmailAddress";
    public static readonly Guid AccountProviderIdentifier = VSAccountProviderConstants.AccountProviderIdentifier;
    public const string PersonalizationAccountPropertyName = "VisualStudioPersonalizationAccount";
    public const string IsMSAPropertyName = "IsMSA";
    public const string SigninExtraQueryParameters = "site_id=501454&display=popup&nux=1";
    internal const string AccountHomeTenantPropertyName = "HomeTenant";
    internal const string AccountIdentityProviderProperty = "IdentityProvider";
    internal const string AccountTenantInformationPropertyName = "Tenants";
    internal const string IdTokenPayloadPropertyName = "IdTokenPayload";
    internal const string ScopesPropertyName = "Scopes";
    private const string DisableMsaFedQueryParameter = "&msafed=0";
    private const string LiveDomainHintQueryParameter = "&domain_hint=live.com";
    private const string AzureRMAPIVersion = "2014-04-01";
    private const string GraphEndpointApiVersion = "2013-04-05";
    private const int SilentRetryCount = 3;
    private static readonly IReadOnlyList<Guid> EmptyGuidList = (IReadOnlyList<Guid>) new List<Guid>().AsReadOnly();
    private static readonly string ExtraQueryParametersRegistryOverride;
    private IAccountCache accountCache;
    private readonly SessionTokenStorage sessionTokenStore;
    private IAccountStore accountStore;
    private static readonly byte[] defaultProviderImage = AccountManagementUtilities.GetDefaultProviderImage();
    private static readonly byte[] msaAccountLogo = AccountManagementUtilities.GetMSAImage();
    private readonly VSAccountProvider.TryGetSelfAsync vsoSelf;
    private readonly VSAccountProvider.TryGetSelfAsync azureRMSelf;
    private readonly IVSAccountProviderShim vsAccountProviderShim;
    private readonly IAadProviderConfiguration aadConfiguration;
    private readonly IAccountCacheConfiguration cacheConfiguration;
    private string extraQueryParametersOverride;
    private Guid msaHomeTenant;

    public event EventHandler<AccountsProcessedEventArgs> AccountProcessingEnd;

    public IAadProviderConfiguration Configuration => this.vsAccountProviderShim != null ? this.vsAccountProviderShim.GetConfiguration() : this.aadConfiguration;

    public IAccountCacheConfiguration CacheConfiguration => this.cacheConfiguration;

    [Obsolete("Use the Configuration.ResourceEndpoint property on this class instead. Note the new property is not settable", false)]
    public Uri Endpoint
    {
      get => this.Configuration.ResourceEndpoint;
      set
      {
        if (!(this.Configuration is AadProviderConfiguration))
          return;
        ((AadProviderConfiguration) this.Configuration).ResourceEndpoint = value;
      }
    }

    internal VSAccountProvider.TryGetSelfAsync GetSelfIdentity => !this.Configuration.AzureRMIdentityEnabled ? this.vsoSelf : this.azureRMSelf;

    internal VSAccountProvider.GetWrappedItemsFromCache GetCacheWrappedItems { get; set; }

    internal VSAccountProvider.AcquireTokenSilently AcquireTokenNoPrompt { get; set; }

    internal VSAccountProvider.AcquireTokenSilentlyAsync AcquireTokenNoPromptAsync { get; set; }

    internal VSAccountProvider.AcquireTokenWithUI AcquireTokenWithPrompt { get; set; }

    internal static Func<CultureInfo> GetQueryParameterCultureInfoFunc { get; set; }

    public string ExtraQueryParametersOverride
    {
      get => this.vsAccountProviderShim != null ? this.vsAccountProviderShim.ExtraQueryParameters : this.extraQueryParametersOverride;
      set
      {
        if (this.vsAccountProviderShim != null)
          this.vsAccountProviderShim.ExtraQueryParameters = value;
        else
          this.extraQueryParametersOverride = value;
      }
    }

    public Guid MsaHomeTenantId
    {
      get
      {
        if (this.msaHomeTenant == Guid.Empty)
        {
          if (this.vsAccountProviderShim != null)
            this.msaHomeTenant = this.vsAccountProviderShim.GetMsaHomeTenantId();
          else
            Guid.TryParse(VssAadSettings.ApplicationTenant, out this.msaHomeTenant);
        }
        return this.msaHomeTenant;
      }
    }

    public Guid AccountProviderId => VSAccountProvider.AccountProviderIdentifier;

    public IAccountStore AccountStore
    {
      get => this.accountStore;
      internal set
      {
        ArgumentUtility.CheckForNull<IAccountStore>(value, nameof (value));
        this.accountStore = value;
      }
    }

    static VSAccountProvider()
    {
      VSAccountProvider.ExtraQueryParametersRegistryOverride = AccountManagementUtilities.GetExtraParametersRegistryOverride();
      VSAccountProvider.GetQueryParameterCultureInfoFunc = (Func<CultureInfo>) (() => CultureInfo.CurrentUICulture);
    }

    public VSAccountProvider(string instanceName, IVSAccountProviderShim vsAccountProviderShim = null)
      : this((VSAccountProvider.TryGetSelfAsync) null, (VSAccountProvider.TryGetSelfAsync) null, (VSAccountProvider.GetWrappedItemsFromCache) null, (VSAccountProvider.AcquireTokenSilently) null, (VSAccountProvider.AcquireTokenWithUI) null, (SessionTokenStorage) null)
    {
      this.CacheConfiguration.InstanceName = instanceName;
      this.vsAccountProviderShim = vsAccountProviderShim;
      if (this.vsAccountProviderShim == null)
        return;
      this.vsAccountProviderShim.RaiseAccountProcessingDoneEvent = new Action<bool, List<Exception>>(this.RaiseAccountProcessingEnd);
    }

    internal VSAccountProvider(
      VSAccountProvider.TryGetSelfAsync self,
      VSAccountProvider.TryGetSelfAsync azureRMSelf,
      VSAccountProvider.GetWrappedItemsFromCache cacheLists,
      VSAccountProvider.AcquireTokenSilently acquireTokenSilently,
      VSAccountProvider.AcquireTokenWithUI acquireTokenWithUI,
      SessionTokenStorage sessionTokenStorage)
    {
      this.GetCacheWrappedItems = cacheLists ?? new VSAccountProvider.GetWrappedItemsFromCache(this.GetWrappedCacheItems);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.AcquireTokenNoPrompt = acquireTokenSilently ?? VSAccountProvider.\u003C\u003EO.\u003C0\u003E__AcquireTokenSilentAsync ?? (VSAccountProvider.\u003C\u003EO.\u003C0\u003E__AcquireTokenSilentAsync = new VSAccountProvider.AcquireTokenSilently(VSAccountProvider.AcquireTokenSilentAsync));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.AcquireTokenNoPromptAsync = VSAccountProvider.\u003C\u003EO.\u003C1\u003E__AcquireTokenSilentAsync ?? (VSAccountProvider.\u003C\u003EO.\u003C1\u003E__AcquireTokenSilentAsync = new VSAccountProvider.AcquireTokenSilentlyAsync(VSAccountProvider.AcquireTokenSilentAsync));
      this.AcquireTokenWithPrompt = acquireTokenWithUI ?? new VSAccountProvider.AcquireTokenWithUI(this.AuthenticateAgainstTenantWithUI);
      this.sessionTokenStore = sessionTokenStorage ?? new SessionTokenStorage(AccountManager.Logger, AccountManager.DefaultInstance.GetCache<VssTokenStorage>());
      this.aadConfiguration = (IAadProviderConfiguration) new AadProviderConfiguration();
      this.cacheConfiguration = (IAccountCacheConfiguration) new AccountCacheConfiguration();
      this.vsoSelf = self ?? new VSAccountProvider.TryGetSelfAsync(this.GetIdentityAsync);
      this.azureRMSelf = azureRMSelf ?? new VSAccountProvider.TryGetSelfAsync(this.GetIdentityAzureRM);
    }

    public void SetAccountCache(IAccountCache cache)
    {
      ArgumentUtility.CheckForNull<IAccountCache>(cache, nameof (cache));
      this.accountCache = this.accountCache == null ? cache : throw new InvalidOperationException(ClientResources.VsAccountProviderSetCacheInvalidOperation());
    }

    public async Task<string> ProcessAuthenticationResult(AuthenticationResult authenticationResult)
    {
      if (this.vsAccountProviderShim != null)
        return await this.vsAccountProviderShim.ProcessAuthenticationResultAsync(authenticationResult);
      ArgumentUtility.CheckForNull<AuthenticationResult>(authenticationResult, nameof (authenticationResult));
      return await this.ProcessAuthenticationResult((IAccountCacheItem) new AccountCacheItem(authenticationResult));
    }

    internal async Task<string> ProcessAuthenticationResult(IAccountCacheItem authenticationResult)
    {
      ArgumentUtility.CheckForNull<IAccountCacheItem>(authenticationResult, nameof (authenticationResult));
      Account addedAccount = (Account) null;
      List<Exception> errors = new List<Exception>();
      try
      {
        if (this.AccountStore != null)
        {
          try
          {
            IAccountCacheItem newCacheItem = this.GetCacheWrappedItems(this.accountCache).FirstOrDefault<IAccountCacheItem>((Func<IAccountCacheItem, bool>) (x => x.UniqueId.EqualsOrdinalIgnoreCase(authenticationResult.UniqueId) && x.TenantId.EqualsOrdinalIgnoreCase(authenticationResult.TenantId)));
            if (newCacheItem != null)
              addedAccount = await this.AddPossibleNewAccount(errors, newCacheItem, new CancellationToken());
          }
          catch (Exception ex)
          {
            errors.Add(ex);
          }
        }
      }
      finally
      {
        this.RaiseAccountProcessingEnd(false, errors);
      }
      string uniqueId = addedAccount != null ? addedAccount.UniqueId : (string) null;
      addedAccount = (Account) null;
      errors = (List<Exception>) null;
      return uniqueId;
    }

    private List<IAccountCacheItem> GetWrappedCacheItems(IAccountCache cache)
    {
      ArgumentUtility.CheckForNull<IAccountCache>(cache, nameof (cache));
      return cache.GetItems().ToList<IAccountCacheItem>();
    }

    private async Task<Account> AddPossibleNewAccount(
      List<Exception> errors,
      IAccountCacheItem newCacheItem,
      CancellationToken cancellationToken)
    {
      try
      {
        return await VSAccountProvider.AddNewAccountToStoreAsync(this.Configuration, errors, this.AcquireTokenNoPrompt, this.accountCache, this.accountStore, this.GetSelfIdentity, newCacheItem.Username, newCacheItem.TenantId, newCacheItem.Environment, newCacheItem.UniqueId, cancellationToken, newCacheItem.IdToken);
      }
      catch (Exception ex)
      {
        if (ex is OperationCanceledException)
          throw;
        else
          errors.Add(ex);
      }
      return (Account) null;
    }

    private IList<KeyValuePair<string, string>> GetUniqueIdTenantIdForMissingOrExpiredTokens(
      IEnumerable<TenantInformation> tenantInformation)
    {
      ArgumentUtility.CheckForNull<IEnumerable<TenantInformation>>(tenantInformation, nameof (tenantInformation));
      List<IAccountCacheItem> currentItems = this.GetCacheWrappedItems(this.accountCache);
      return VSAccountProvider.GetUniqueIdTenantIdForMissingOrExpiredTokens(tenantInformation, currentItems);
    }

    private static IList<KeyValuePair<string, string>> GetUniqueIdTenantIdForMissingOrExpiredTokens(
      IEnumerable<TenantInformation> tenantInformation,
      List<IAccountCacheItem> currentItems)
    {
      ArgumentUtility.CheckForNull<IEnumerable<TenantInformation>>(tenantInformation, nameof (tenantInformation));
      ArgumentUtility.CheckForNull<List<IAccountCacheItem>>(currentItems, nameof (currentItems));
      List<KeyValuePair<string, string>> missingOrExpiredTokens = new List<KeyValuePair<string, string>>();
      foreach (TenantInformation tenantInformation1 in tenantInformation)
      {
        TenantInformation info = tenantInformation1;
        foreach (string uniqueId1 in info.UniqueIds)
        {
          string uniqueId = uniqueId1;
          if (!currentItems.Any<IAccountCacheItem>((Func<IAccountCacheItem, bool>) (item => item.ExpiresOn > DateTimeOffset.Now && item.UniqueId.EqualsOrdinalIgnoreCase(uniqueId) && item.TenantId.EqualsOrdinalIgnoreCase(info.TenantId))))
            missingOrExpiredTokens.Add(new KeyValuePair<string, string>(uniqueId, info.TenantId));
        }
      }
      return (IList<KeyValuePair<string, string>>) missingOrExpiredTokens;
    }

    private async Task<bool?> GetAccountNeedsReauthenticationAsync(
      Account account,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<Account>(account, nameof (account));
      List<TenantInformation> information = VSAccountProvider.GetTenantInformation(account);
      if (!information.Any<TenantInformation>())
        return new bool?(true);
      IList<KeyValuePair<string, string>> expiredUniqueIdTenantIds = this.GetUniqueIdTenantIdForMissingOrExpiredTokens((IEnumerable<TenantInformation>) information);
      if (expiredUniqueIdTenantIds.Count == 0)
        return new bool?(false);
      if (account.NeedsReauthentication && expiredUniqueIdTenantIds.Count == 1 || !account.NeedsReauthentication && expiredUniqueIdTenantIds.Count == information.Sum<TenantInformation>((Func<TenantInformation, int>) (info => info.UniqueIds.Length)))
      {
        foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) expiredUniqueIdTenantIds)
        {
          KeyValuePair<string, string> kvp = keyValuePair;
          cancellationToken.ThrowIfCancellationRequested();
          try
          {
            if (await this.AcquireTokenNoPromptAsync(this.Configuration, this.accountCache, kvp, this.Configuration.Scopes))
            {
              expiredUniqueIdTenantIds.Remove(kvp);
              break;
            }
          }
          catch (Exception ex)
          {
            Trace.WriteLine(string.Format("Could not refresh the token for user {0}, tenant {1} : {2}", (object) kvp.Key, (object) kvp.Value, (object) ex));
          }
          kvp = new KeyValuePair<string, string>();
        }
      }
      return expiredUniqueIdTenantIds.Count != information.Sum<TenantInformation>((Func<TenantInformation, int>) (info => info.UniqueIds.Length)) ? (expiredUniqueIdTenantIds.Count != 0 ? new bool?() : new bool?(false)) : new bool?(true);
    }

    private static async Task<IAccountCacheItem> AcquireTokenSilentAsync(
      IAadProviderConfiguration config,
      IAccountCache accountCache,
      string tenantId,
      string userIdentifier,
      string[] scopes)
    {
      ArgumentUtility.CheckForNull<IAadProviderConfiguration>(config, nameof (config));
      ArgumentUtility.CheckForNull<IAccountCache>(accountCache, nameof (accountCache));
      ArgumentUtility.CheckStringForNullOrEmpty(userIdentifier, nameof (userIdentifier));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) scopes, nameof (scopes));
      AccountCache cache = new AccountCache((IAccountCacheConfiguration) new AccountCacheConfiguration(), config, tenantId);
      for (int i = 1; i <= 3; ++i)
      {
        try
        {
          return await cache.AcquireTokenSilentAsync(scopes, userIdentifier, tenantId);
        }
        catch (Exception ex)
        {
          Trace.WriteLine(string.Format("Could not refresh the token for user {0} : Tenant:{1}, Scopes:{2}, Try:{3}, Exception:{4}", (object) userIdentifier, (object) tenantId, (object) scopes, (object) i, (object) ex));
          if (i == 3)
            throw;
        }
      }
      return (IAccountCacheItem) null;
    }

    private static async Task<bool> AcquireTokenSilentAsync(
      IAadProviderConfiguration config,
      IAccountCache accountCache,
      KeyValuePair<string, string> userIdTenantIdPair,
      string[] scopes)
    {
      ArgumentUtility.CheckForNull<IAadProviderConfiguration>(config, nameof (config));
      ArgumentUtility.CheckForNull<IAccountCache>(accountCache, nameof (accountCache));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) scopes, nameof (scopes));
      string userId = userIdTenantIdPair.Key;
      string tenantId = userIdTenantIdPair.Value;
      if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tenantId))
        return false;
      try
      {
        return !string.IsNullOrEmpty((await accountCache.AcquireTokenSilentAsync(scopes, userId, tenantId)).AccessToken);
      }
      catch (Exception ex)
      {
        Trace.WriteLine(string.Format("Could not refresh the token for user {0} : {1}", (object) userId, (object) ex));
      }
      return false;
    }

    internal static async Task<Account> AddNewAccountToStoreAsync(
      IAadProviderConfiguration config,
      List<Exception> errors,
      VSAccountProvider.AcquireTokenSilently acquireTokenSilently,
      IAccountCache accountCache,
      IAccountStore store,
      VSAccountProvider.TryGetSelfAsync getSelfAsync,
      string userName,
      string tenantId,
      string identityProvider,
      string uniqueId,
      CancellationToken cancellationToken,
      string idToken)
    {
      return await VSAccountProvider.AddOrUpdateAccountToStoreAsync(config, errors, acquireTokenSilently, accountCache, getSelfAsync, userName, tenantId, identityProvider, uniqueId, store, (Account) null, (IEnumerable<ScopeInfo>) null, cancellationToken, idToken);
    }

    private async Task<IdentitySelf> GetIdentityAsync(
      List<Exception> errors,
      string tenantId,
      string uniqueId,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<List<Exception>>(errors, nameof (errors));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      ArgumentUtility.CheckStringForNullOrEmpty(uniqueId, nameof (uniqueId));
      Uri resolvedURI = AccountManager.VsoEndpoint;
      try
      {
        TenantInfo tenant = await new IdentityHttpClient(resolvedURI, (VssCredentials) (FederatedCredential) new VssAadCredential()).GetTenant(tenantId, (object) null, cancellationToken);
      }
      catch (Exception ex)
      {
        errors.Add(ex);
        return (IdentitySelf) null;
      }
      int maxRetries = 3;
      int retryDelay = 1;
      IAccountCacheItem result = (IAccountCacheItem) null;
      Exception lastException = (Exception) null;
      for (int i = 1; i <= maxRetries; ++i)
      {
        try
        {
          result = await this.accountCache.AcquireTokenSilentAsync(VssAadSettings.DefaultScopes, uniqueId, tenantId);
          break;
        }
        catch (Exception ex)
        {
          lastException = ex;
          Thread.Sleep(TimeSpan.FromSeconds((double) (retryDelay * i)));
        }
      }
      if (result == null)
      {
        if (lastException != null)
          errors.Add(lastException);
      }
      else
      {
        AuthenticationResult innerResult = result.InnerResult;
        IdentitySelf identitySelfAsync = await new IdentityHttpClient(resolvedURI, (VssCredentials) (FederatedCredential) new VssAadCredential(new VssAadToken(innerResult.TokenType, innerResult.AccessToken))).GetIdentitySelfAsync((object) null, cancellationToken);
        if (identitySelfAsync != null)
          return identitySelfAsync;
        Trace.WriteLine("VsAccountProvider AddNewAccounts: identity is null from vso");
      }
      return (IdentitySelf) null;
    }

    private async Task<IdentitySelf> GetIdentityAzureRM(
      List<Exception> errors,
      string homeTenantId,
      string homeUniqueId,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<List<Exception>>(errors, nameof (errors));
      ArgumentUtility.CheckStringForNullOrEmpty(homeTenantId, nameof (homeTenantId));
      ArgumentUtility.CheckStringForNullOrEmpty(homeUniqueId, nameof (homeUniqueId));
      IdentitySelf self = (IdentitySelf) null;
      Exception lastException = (Exception) null;
      JObject serverResponse = (JObject) null;
      for (int i = 1; i <= 3; ++i)
      {
        try
        {
          IAccountCacheItem accountCacheItem = await this.accountCache.AcquireTokenSilentAsync(this.Configuration.AzureRMAudienceScopes, homeUniqueId, homeTenantId);
          HttpResponseMessage response = await new HttpClient()
          {
            DefaultRequestHeaders = {
              Authorization = new AuthenticationHeaderValue("Bearer", accountCacheItem.AccessToken)
            }
          }.GetAsync(AccountManagementUtilities.CreateAzureRMUri(this.Configuration.AzureResourceManagementEndpoint, "2014-04-01", new Uri("/tenants", UriKind.Relative)));
          string json = await response.Content.ReadAsStringAsync();
          response.EnsureSuccessStatusCode();
          serverResponse = JObject.Parse(json);
          if (serverResponse["value"] == null)
            response = (HttpResponseMessage) null;
          else
            break;
        }
        catch (Exception ex)
        {
          await Task.Delay(TimeSpan.FromSeconds((double) i));
        }
      }
      if (serverResponse == null || serverResponse["value"] == null)
      {
        if (lastException != null)
          errors.Add(lastException);
      }
      else
      {
        self = new IdentitySelf();
        self.Id = Guid.Parse(homeTenantId);
        HashSet<TenantInfo> tenantInfoSet = new HashSet<TenantInfo>();
        foreach (JToken jtoken in (IEnumerable<JToken>) serverResponse["value"])
        {
          try
          {
            if (jtoken[(object) "tenantId"] != null)
            {
              string input = jtoken[(object) "tenantId"].ToString();
              TenantInfo tenantInfo = new TenantInfo();
              tenantInfo.TenantId = Guid.Parse(input);
              Guid guid = Guid.Parse(homeTenantId);
              tenantInfo.HomeTenant = guid == tenantInfo.TenantId;
              tenantInfo.TenantName = (string) null;
              tenantInfoSet.Add(tenantInfo);
            }
          }
          catch (Exception ex)
          {
            errors.Add(ex);
          }
        }
        self.Tenants = (IEnumerable<TenantInfo>) tenantInfoSet;
      }
      IdentitySelf identityAzureRm = self;
      self = (IdentitySelf) null;
      lastException = (Exception) null;
      serverResponse = (JObject) null;
      return identityAzureRm;
    }

    internal void RaiseAccountProcessingEnd(bool skipped, List<Exception> errors)
    {
      if (this.AccountProcessingEnd == null)
        return;
      if (errors != null && errors.Count > 0)
      {
        foreach (Exception error in errors)
          Trace.WriteLine("VsAccountProvider AddNewAccounts: " + error.ToReadableStackTrace());
      }
      this.AccountProcessingEnd((object) this, new AccountsProcessedEventArgs(skipped, (IList<Exception>) errors));
    }

    internal static bool IsPossibleNewAccount(
      AuthenticationResult account,
      HashSet<string> accountUniqueIds)
    {
      ArgumentUtility.CheckForNull<AuthenticationResult>(account, nameof (account));
      ArgumentUtility.CheckForNull<HashSet<string>>(accountUniqueIds, nameof (accountUniqueIds));
      return VSAccountProvider.IsPossibleNewAccount((IAccountCacheItem) new AccountCacheItem(account), accountUniqueIds);
    }

    internal static bool IsPossibleNewAccount(
      IAccountCacheItem item,
      HashSet<string> accountUniqueIds)
    {
      ArgumentUtility.CheckForNull<IAccountCacheItem>(item, nameof (item));
      ArgumentUtility.CheckForNull<HashSet<string>>(accountUniqueIds, nameof (accountUniqueIds));
      return string.IsNullOrWhiteSpace(item.UniqueId) || !accountUniqueIds.Contains(item.UniqueId);
    }

    internal static HashSet<string> GetAccountUniqueIds(
      List<Exception> errors,
      IEnumerable<Account> accountsForProvider)
    {
      HashSet<string> accountUniqueIds = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (accountsForProvider != null)
      {
        foreach (Account account in accountsForProvider)
        {
          try
          {
            foreach (TenantInformation tenantInformation in VSAccountProvider.GetTenantInformation(account))
              accountUniqueIds.UnionWith((IEnumerable<string>) tenantInformation.UniqueIds);
          }
          catch (Exception ex)
          {
            errors?.Add(ex);
            Trace.WriteLine("VSAccountProvider GetUniqueIdTenantIdMap: " + ex.ToReadableStackTrace());
          }
        }
      }
      return accountUniqueIds;
    }

    internal static Account GetAccountfromUniqueIdTenantId(
      List<Exception> errors,
      IEnumerable<Account> accountsForProvider,
      string uniqueID,
      string tenantID)
    {
      if (accountsForProvider != null)
      {
        foreach (Account account in accountsForProvider)
        {
          try
          {
            foreach (TenantInformation tenantInformation in VSAccountProvider.GetTenantInformation(account))
            {
              if (tenantInformation.TenantId.EqualsOrdinalIgnoreCase(tenantID) && ((IEnumerable<string>) tenantInformation.UniqueIds).Contains<string>(uniqueID))
                return account;
            }
          }
          catch (Exception ex)
          {
            errors?.Add(ex);
            Trace.WriteLine("VSAccountProvider GetAccountfromUniqueIdTenantId: " + ex.ToReadableStackTrace());
          }
        }
      }
      return (Account) null;
    }

    internal static async Task<Account> AddOrUpdateAccountToStoreAsync(
      IAadProviderConfiguration config,
      List<Exception> errors,
      VSAccountProvider.AcquireTokenSilently acquireTokenSilently,
      IAccountCache accountCache,
      VSAccountProvider.TryGetSelfAsync getSelfAsync,
      string userName,
      string tenantId,
      string identityProvider,
      string uniqueId,
      IAccountStore store,
      Account account,
      IEnumerable<ScopeInfo> scopes,
      CancellationToken cancellationToken,
      string idToken)
    {
      ArgumentUtility.CheckForNull<IAadProviderConfiguration>(config, nameof (config));
      ArgumentUtility.CheckStringForNullOrEmpty(uniqueId, nameof (uniqueId));
      ArgumentUtility.CheckStringForNullOrEmpty(userName, nameof (userName));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      ArgumentUtility.CheckForNull<IAccountStore>(store, nameof (store));
      ArgumentUtility.CheckForNull<List<Exception>>(errors, nameof (errors));
      ArgumentUtility.CheckForNull<VSAccountProvider.TryGetSelfAsync>(getSelfAsync, nameof (getSelfAsync));
      if (string.IsNullOrEmpty(userName))
        throw new ArgumentException(ClientResources.VsAccountProviderUserNameCannotBeEmpty());
      bool isMSA = VSAccountProvider.IsMSAAccount(identityProvider);
      IdentitySelf self = await getSelfAsync(errors, tenantId, uniqueId, cancellationToken).ConfigureAwait(false);
      if (self == null)
        return (Account) null;
      if (account == null)
        account = store.GetAccountFromKey(new AccountKey(VSAccountProvider.FormatGuid(self.Id), VSAccountProvider.AccountProviderIdentifier));
      if (scopes == null && account != null)
        scopes = VSAccountProvider.GetScopesForAccountInternal(account);
      string displayName = self.DisplayName ?? string.Empty;
      string identityUniqueId = VSAccountProvider.FormatGuid(self.Id);
      List<TenantInfo> tenantInfos = new List<TenantInfo>();
      if (self.Tenants != null)
        tenantInfos = self.Tenants.ToList<TenantInfo>();
      Dictionary<string, string> properties = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (account != null)
      {
        foreach (KeyValuePair<string, string> property in (IEnumerable<KeyValuePair<string, string>>) account.Properties)
          properties[property.Key] = property.Value;
      }
      properties["IdentityProvider"] = identityProvider ?? string.Empty;
      properties["IsMSA"] = isMSA.ToString();
      List<TenantInformation> tenantInformationList = new List<TenantInformation>();
      try
      {
        if (acquireTokenSilently != null)
          tenantInformationList = await VSAccountProvider.GetTokensForTenants(config, accountCache, acquireTokenSilently, tenantInfos, scopes, uniqueId, tenantId, userName, isMSA);
      }
      catch (Exception ex)
      {
        errors.Add(ex);
        Trace.WriteLine(string.Format("There was a problem getting tokens for account '{0}' : '{1}'", (object) self.Id, (object) ex));
        return (Account) null;
      }
      string str = (string) null;
      string providerDisplayName = isMSA ? ClientResources.VsAccountProviderName() : ClientResources.VsAccountProviderWorkOrSchool();
      if (isMSA)
      {
        str = VssAadSettings.ApplicationTenant;
      }
      else
      {
        TenantInfo tenantInfo = tenantInfos.SingleOrDefault<TenantInfo>((Func<TenantInfo, bool>) (x => x.HomeTenant));
        if (tenantInfo != null)
        {
          str = VSAccountProvider.FormatGuid(tenantInfo.TenantId);
          providerDisplayName = !string.IsNullOrEmpty(tenantInfo.TenantName) ? tenantInfo.TenantName : providerDisplayName;
        }
      }
      if (!string.IsNullOrEmpty(str))
        properties["HomeTenant"] = str;
      properties["Tenants"] = JsonConvert.SerializeObject((object) tenantInformationList);
      if (!string.IsNullOrEmpty(idToken))
        properties["IdTokenPayload"] = new JwtSecurityToken(idToken).Payload.SerializeToJson();
      if (scopes != null)
        properties["Scopes"] = JsonConvert.SerializeObject((object) scopes);
      Account account1 = new Account(new AccountInitializationData()
      {
        Authenticator = tenantId,
        DisplayInfo = new AccountDisplayInfo(displayName, providerDisplayName, userName, (byte[]) null, isMSA ? VSAccountProvider.msaAccountLogo : VSAccountProvider.defaultProviderImage),
        ParentProviderId = VSAccountProvider.AccountProviderIdentifier,
        SupportedAccountProviders = VSAccountProvider.EmptyGuidList,
        UniqueId = identityUniqueId ?? VSAccountProvider.FormatGuid(Guid.NewGuid()),
        Properties = (IReadOnlyDictionary<string, string>) new ReadOnlyDictionary<string, string>((IDictionary<string, string>) properties),
        NeedsReauthentication = false
      });
      cancellationToken.ThrowIfCancellationRequested();
      return store.AddOrUpdateAccount(account1);
    }

    public IEnumerable<ScopeInfo> GetScopesForAccount(AccountKey accountKey)
    {
      ArgumentUtility.CheckForNull<AccountKey>(accountKey, nameof (accountKey));
      return this.vsAccountProviderShim != null ? this.vsAccountProviderShim.GetScopesForAccount(accountKey) : VSAccountProvider.GetScopesForAccountInternal(this.FindAccount(accountKey));
    }

    internal static IEnumerable<ScopeInfo> GetScopesForAccountInternal(Account account)
    {
      ArgumentUtility.CheckForNull<Account>(account, nameof (account));
      string str;
      if (account.Properties.TryGetValue("Scopes", out str))
      {
        if (!string.IsNullOrEmpty(str))
        {
          try
          {
            return (IEnumerable<ScopeInfo>) JsonConvert.DeserializeObject<List<ScopeInfo>>(str);
          }
          catch (Exception ex)
          {
            Trace.WriteLine(string.Format("Account '{0}' has a bad scope property, because '{1}'", (object) account.UniqueId, (object) ex));
          }
        }
      }
      return Enumerable.Empty<ScopeInfo>();
    }

    internal static async Task<List<TenantInformation>> GetTokensForTenants(
      IAadProviderConfiguration config,
      IAccountCache accountCache,
      VSAccountProvider.AcquireTokenSilently acquireTokenSilently,
      List<TenantInfo> tenantInfos,
      IEnumerable<ScopeInfo> scopes,
      string authenticatorUniqueId,
      string authenticator,
      string userName,
      bool isMSA)
    {
      ArgumentUtility.CheckForNull<IAadProviderConfiguration>(config, nameof (config));
      ArgumentUtility.CheckForNull<List<TenantInfo>>(tenantInfos, nameof (tenantInfos));
      ArgumentUtility.CheckForNull<VSAccountProvider.AcquireTokenSilently>(acquireTokenSilently, nameof (acquireTokenSilently));
      ArgumentUtility.CheckStringForNullOrEmpty(authenticatorUniqueId, nameof (authenticatorUniqueId));
      ArgumentUtility.CheckStringForNullOrEmpty(authenticator, nameof (authenticator));
      ArgumentUtility.CheckStringForNullOrEmpty(userName, nameof (userName));
      if (isMSA)
      {
        List<TenantInfo> tenantInfoList = new List<TenantInfo>((IEnumerable<TenantInfo>) tenantInfos);
        TenantInfo tenantInfo = new TenantInfo();
        Guid result;
        if (Guid.TryParse(VssAadSettings.ApplicationTenant, out result) && !object.Equals((object) Guid.Empty, (object) result))
        {
          tenantInfo.TenantId = result;
          tenantInfo.TenantName = ClientResources.VsAccountProviderName();
          tenantInfoList.Add(tenantInfo);
          tenantInfos = tenantInfoList;
        }
      }
      List<TenantInformation> tenantInformation = new List<TenantInformation>();
      string id;
      if (scopes != null)
      {
        foreach (ScopeInfo scope in scopes.Where<ScopeInfo>((Func<ScopeInfo, bool>) (x => x.TenantId == Guid.Empty && !string.IsNullOrEmpty(x.Domain) || !tenantInfos.Any<TenantInfo>((Func<TenantInfo, bool>) (info => info.TenantId == x.TenantId)))))
        {
          string formattedTenantId = (string) null;
          id = (string) null;
          if (scope.TenantId != Guid.Empty)
          {
            formattedTenantId = VSAccountProvider.FormatGuid(scope.TenantId);
            id = (await acquireTokenSilently(config, accountCache, VSAccountProvider.FormatGuid(scope.TenantId), userName, config.Scopes))?.UniqueId;
          }
          else
          {
            IAccountCacheItem domainResult = await acquireTokenSilently(config, accountCache, scope.Domain, userName, config.Scopes);
            formattedTenantId = domainResult.TenantId;
            Guid result;
            if (Guid.TryParse(domainResult.TenantId, out result))
              scope.UpdateTenantIdForDomain(result);
            if (!tenantInfos.Any<TenantInfo>((Func<TenantInfo, bool>) (info => string.Equals(VSAccountProvider.FormatGuid(info.TenantId), domainResult.TenantId, StringComparison.OrdinalIgnoreCase))))
              id = (await acquireTokenSilently(config, accountCache, formattedTenantId, userName, config.Scopes))?.UniqueId;
          }
          if (id != null && !tenantInformation.Any<TenantInformation>((Func<TenantInformation, bool>) (ti => string.Equals(ti.TenantId, formattedTenantId, StringComparison.OrdinalIgnoreCase))))
            tenantInformation.Add(new TenantInformation(new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
            {
              id
            }, formattedTenantId, "", false));
          id = (string) null;
        }
      }
      foreach (TenantInfo tenantInfo in tenantInfos)
      {
        id = VSAccountProvider.FormatGuid(tenantInfo.TenantId);
        IAccountCacheItem result = await acquireTokenSilently(config, accountCache, id, userName, config.Scopes);
        string uniqueId = result.UniqueId;
        if (uniqueId != null)
        {
          HashSet<string> uniqueIds = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          uniqueIds.Add(uniqueId);
          if (string.Equals(id, authenticator, StringComparison.OrdinalIgnoreCase) && !string.Equals(authenticatorUniqueId, uniqueId, StringComparison.OrdinalIgnoreCase))
            uniqueIds.Add(authenticatorUniqueId);
          if (string.IsNullOrEmpty(tenantInfo.TenantName) && config.AzureRMIdentityEnabled)
          {
            for (int i = 0; i < 3; ++i)
            {
              try
              {
                IAccountCacheItem accountCacheItem = await acquireTokenSilently(config, accountCache, result.TenantId, result.UniqueId, config.GraphScopes);
                HttpResponseMessage response = await new HttpClient()
                {
                  DefaultRequestHeaders = {
                    Authorization = new AuthenticationHeaderValue("Bearer", accountCacheItem.AccessToken)
                  }
                }.GetAsync(AccountManagementUtilities.CreateAzureRMUri(config.GraphEndpoint, "2013-04-05", new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/tenantDetails", (object) result.TenantId), UriKind.Relative)));
                string json = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                JObject jobject = JObject.Parse(json);
                if (jobject["value"] != null && jobject["value"][(object) 0] != null && jobject["value"][(object) 0][(object) "displayName"] != null)
                {
                  tenantInfo.TenantName = jobject["value"][(object) 0][(object) "displayName"].ToString();
                  tenantInformation.Add(new TenantInformation(uniqueIds, id, tenantInfo.TenantName, true));
                  break;
                }
                response = (HttpResponseMessage) null;
              }
              catch (Exception ex)
              {
                if (i == 2)
                  throw;
              }
            }
          }
          else
            tenantInformation.Add(new TenantInformation(uniqueIds, id, tenantInfo.TenantName, true));
          uniqueIds = (HashSet<string>) null;
        }
        id = (string) null;
        result = (IAccountCacheItem) null;
      }
      List<TenantInformation> tokensForTenants = tenantInformation;
      tenantInformation = (List<TenantInformation>) null;
      return tokensForTenants;
    }

    internal static bool IsMSAAccount(string identityProvider)
    {
      string applicationTenant = VssAadSettings.ApplicationTenant;
      if (string.IsNullOrWhiteSpace(identityProvider))
        return false;
      return identityProvider.IndexOf("live.com", StringComparison.OrdinalIgnoreCase) != -1 || identityProvider.IndexOf("live-int.com", StringComparison.OrdinalIgnoreCase) != -1 || identityProvider.IndexOf(applicationTenant, StringComparison.OrdinalIgnoreCase) != -1 || identityProvider.IndexOf("f8cdef31-a31e-4b4a-93e4-5f571e91255a", StringComparison.OrdinalIgnoreCase) != -1 || identityProvider.IndexOf("ea8a4392-515e-481f-879e-6571ff2a8a36", StringComparison.OrdinalIgnoreCase) != -1;
    }

    internal static string FormatGuid(Guid guidToFormat) => guidToFormat.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);

    public void ClearTokensForAccount(Account account)
    {
      ArgumentUtility.CheckForNull<Account>(account, nameof (account));
      if (this.vsAccountProviderShim != null)
      {
        this.vsAccountProviderShim.ClearTokensForAccount(account);
      }
      else
      {
        if (this.accountCache == null)
          return;
        IEnumerable<IAccountCacheItem> items = this.accountCache.GetItems();
        VSAccountProvider.ClearTokensInternal(new VSAccountProvider.DeleteTokenCacheItem(this.DeleteTokenCacheEntry), account, items);
      }
    }

    internal static void ClearTokensInternal(
      VSAccountProvider.DeleteTokenCacheItem deleteAction,
      Account account,
      IEnumerable<IAccountCacheItem> tokens)
    {
      ArgumentUtility.CheckForNull<VSAccountProvider.DeleteTokenCacheItem>(deleteAction, nameof (deleteAction));
      ArgumentUtility.CheckForNull<Account>(account, nameof (account));
      ArgumentUtility.CheckForNull<IEnumerable<IAccountCacheItem>>(tokens, nameof (tokens));
      HashSet<string> uniqueIds = VSAccountProvider.GetAccountUniqueIds((List<Exception>) null, (IEnumerable<Account>) new Account[1]
      {
        account
      });
      Stack<IAccountCacheItem> source = new Stack<IAccountCacheItem>(tokens.Where<IAccountCacheItem>((Func<IAccountCacheItem, bool>) (x => uniqueIds.Contains(x.UniqueId))));
      while (source.Any<IAccountCacheItem>())
      {
        IAccountCacheItem accountCacheItem = source.Pop();
        Task task = deleteAction(accountCacheItem);
      }
    }

    private Task DeleteTokenCacheEntry(IAccountCacheItem token)
    {
      ArgumentUtility.CheckForNull<IAccountCacheItem>(token, nameof (token));
      return this.accountCache.DeleteItemAsync(token);
    }

    public async Task<AuthenticationResult> AcquireTokenAsync(
      string[] scopes,
      string tenantId,
      string userIdentifier,
      IntPtr parentWindowHandle,
      AccountKey accountKeyForReAuthentication = null,
      bool prompt = false,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(userIdentifier, nameof (userIdentifier));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) scopes, nameof (scopes));
      if (this.vsAccountProviderShim != null)
        return await this.vsAccountProviderShim.AcquireTokenAsync(scopes, tenantId, userIdentifier, parentWindowHandle, accountKeyForReAuthentication, prompt, cancellationToken).ConfigureAwait(false);
      Account account = (Account) null;
      if (prompt)
      {
        ArgumentUtility.CheckForNull<AccountKey>(accountKeyForReAuthentication, nameof (accountKeyForReAuthentication));
        account = this.FindAccount(accountKeyForReAuthentication);
      }
      string extraQueryParameters = VSAccountProvider.GetExtraQueryParameters(this.ExtraQueryParametersOverride, VSAccountProvider.ExtraQueryParametersRegistryOverride, account);
      return await this.AcquireTokenInternalAsync(scopes, this.accountCache, tenantId, userIdentifier, (AccountKey) account, prompt, extraQueryParameters, cancellationToken).ConfigureAwait(false);
    }

    public Task<AuthenticationResult> AcquireTokenAsync(
      string[] scopes,
      IAccountCache cache,
      string userIdentifier,
      AccountKey accountForReauthentication = null,
      bool prompt = false,
      string extraQueryParameters = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<IAccountCache>(cache, nameof (cache));
      ArgumentUtility.CheckForNull<string>(userIdentifier, nameof (userIdentifier));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) scopes, nameof (scopes));
      return this.AcquireTokenInternalAsync(scopes, cache, (string) null, userIdentifier, accountForReauthentication, prompt, extraQueryParameters, cancellationToken);
    }

    private async Task<AuthenticationResult> AcquireTokenInternalAsync(
      string[] scopes,
      IAccountCache cache,
      string tenantId,
      string userIdentifier,
      AccountKey accountForReauthentication = null,
      bool prompt = false,
      string extraQueryParameters = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.vsAccountProviderShim != null)
        return await this.vsAccountProviderShim.AcquireTokenAsyncWithContextAsync(scopes, cache, userIdentifier, accountForReauthentication, prompt, extraQueryParameters, cancellationToken);
      if (prompt)
        ArgumentUtility.CheckForNull<AccountKey>(accountForReauthentication, nameof (accountForReauthentication));
      IAccountCacheItem result = (IAccountCacheItem) null;
      VSAccountProvider.AcquireTokenLogEvent logEvent = new VSAccountProvider.AcquireTokenLogEvent();
      try
      {
        try
        {
          result = await cache.AcquireTokenSilentAsync(scopes, userIdentifier, tenantId);
          return result.InnerResult;
        }
        catch (Exception ex)
        {
          logEvent.SetSilentError(ex);
          if (!prompt)
            throw;
        }
        logEvent.Prompted = true;
        AccountManager.Logger.LogEvent("Prompt", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "AuthType",
            (object) "Unknown"
          },
          {
            "Source",
            (object) "Wrapper"
          }
        });
        try
        {
          IntPtr zero = IntPtr.Zero;
          Account account = await this.AuthenticateAccountWithUIAsyncInternal(accountForReauthentication, zero, (IEnumerable<ScopeInfo>) null, extraQueryParameters, cancellationToken);
        }
        catch (Exception ex)
        {
          logEvent.SetPromptError(ex);
          throw;
        }
        try
        {
          IAccountCacheItem accountCacheItem = await cache.AcquireTokenSilentAsync(scopes, userIdentifier, tenantId);
          return result.InnerResult;
        }
        catch (Exception ex)
        {
          logEvent.SetSilentError(ex);
          throw;
        }
      }
      catch
      {
        this.TrySetNeedsReauthentication(userIdentifier, true);
        throw;
      }
      finally
      {
        if (result != null)
          logEvent.Succeeded = true;
        logEvent.Log();
      }
    }

    internal void TrySetNeedsReauthentication(string userIdentifier, bool flag)
    {
      ArgumentUtility.CheckForNull<string>(userIdentifier, nameof (userIdentifier));
      Account account = this.FindAccount(userIdentifier);
      if (account == null)
        return;
      this.SetAccountNeedsReauthentication(account, new bool?(flag));
    }

    public Task<string> GetSessionTokenFromAccountAsync(
      Account account,
      string scope,
      bool forceRefresh = false,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<Account>(account, nameof (account));
      ArgumentUtility.CheckForNull<string>(scope, nameof (scope));
      if (this.vsAccountProviderShim != null)
        return this.vsAccountProviderShim.GetSessionTokenFromAccountAsync(account, scope, forceRefresh, cancellationToken);
      SessionTokenDescriptor tokenParameter = new SessionTokenDescriptor(scope);
      return this.sessionTokenStore.GetSessionTokenFromAccountAsync(account, tokenParameter, forceRefresh, cancellationToken);
    }

    private async Task<IAccountCacheItem> AuthenticateAgainstTenantWithUI(
      IntPtr parentWindowHandle,
      string userIdentifier,
      string tenantId,
      string queryParameters)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(userIdentifier, nameof (userIdentifier));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      AccountCache accountCache = new AccountCache(this.CacheConfiguration, this.Configuration, tenantId, queryParameters);
      IAccountCacheItem accountCacheItem;
      try
      {
        SignInHelper.PrepareSignInState();
        SessionCounterManager.EnsureSessionCounterSet();
        accountCacheItem = await accountCache.AcquireTokenInteractiveAsync(this.Configuration.Scopes, Prompt.ForceLogin, userIdentifier, (string) null);
      }
      catch (MsalClientException ex)
      {
        if (string.Equals(ex.ErrorCode, "authentication_canceled", StringComparison.OrdinalIgnoreCase))
          throw new OperationCanceledException(ex.Message, (Exception) ex);
        throw;
      }
      return accountCacheItem;
    }

    public string GetExtraQueryParameters(string authority)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(authority, nameof (authority));
      return this.vsAccountProviderShim != null ? this.vsAccountProviderShim.GetExtraQueryParameters(authority) : VSAccountProvider.GetExtraQueryParameters(this.ExtraQueryParametersOverride, VSAccountProvider.ExtraQueryParametersRegistryOverride, (Account) null);
    }

    internal string GetExtraQueryParameters(Account account)
    {
      ArgumentUtility.CheckForNull<Account>(account, nameof (account));
      return VSAccountProvider.GetExtraQueryParameters(this.ExtraQueryParametersOverride, VSAccountProvider.ExtraQueryParametersRegistryOverride, account);
    }

    internal static string GetExtraQueryParameters(
      string extraQueryParametersOverride,
      string registryOverride,
      Account account)
    {
      string queryParameters = !string.IsNullOrWhiteSpace(registryOverride) ? registryOverride : (!string.IsNullOrWhiteSpace(extraQueryParametersOverride) ? extraQueryParametersOverride : "site_id=501454&display=popup&nux=1");
      if (account != null)
        queryParameters = !VSAccountProvider.IsMSAAccount(account.GetIdentityProviderProperty()) ? queryParameters + "&msafed=0" : queryParameters + "&domain_hint=live.com";
      return VSAccountProvider.TryAppendLocaleParameter(queryParameters);
    }

    private static string TryAppendLocaleParameter(string queryParameters)
    {
      CultureInfo cultureInfo = VSAccountProvider.GetQueryParameterCultureInfoFunc();
      if (cultureInfo == null)
        return queryParameters;
      if (string.IsNullOrWhiteSpace(queryParameters))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "lc={0}", (object) cultureInfo.LCID);
      HashSet<string> parameterNames = new HashSet<string>((IEnumerable<string>) HttpUtility.ParseQueryString(queryParameters).AllKeys, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (((IEnumerable<string>) new string[2]
      {
        "MKT",
        "LC"
      }).Any<string>((Func<string, bool>) (localeParameterName => parameterNames.Contains(localeParameterName))))
        return queryParameters;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "&lc={0}", (object) cultureInfo.LCID);
      return queryParameters + str;
    }

    internal async Task<IAccountCacheItem> ReauthenticateWithUIInternal(
      IntPtr parentWindowHandle,
      Account account,
      string extraQueryParameters)
    {
      ArgumentUtility.CheckForNull<Account>(account, nameof (account));
      this.ClearTokensForAccount(account);
      return await this.AcquireTokenWithPrompt(parentWindowHandle, account.DisplayInfo.UserName, "common", extraQueryParameters);
    }

    public async Task<Account> RefreshAuthenticationStateAsync(
      AccountKey account,
      CancellationToken cancellationToken)
    {
      if (account == null)
        throw new ArgumentNullException(nameof (account));
      if (this.vsAccountProviderShim != null)
        return await this.vsAccountProviderShim.RefreshAuthenticationStateAsync(account, cancellationToken);
      if (account.ProviderId != this.AccountProviderId)
        throw new ArgumentException(ClientResources.VsAccountProviderUnsupportedAccount(), nameof (account));
      Account accountInstance = this.FindAccount(account);
      bool? reauthenticationAsync = await this.GetAccountNeedsReauthenticationAsync(accountInstance, cancellationToken);
      return this.SetAccountNeedsReauthentication(accountInstance, reauthenticationAsync);
    }

    private Account SetAccountNeedsReauthentication(Account account, bool? needsReauthentication) => this.AccountStore != null && needsReauthentication.HasValue && account.NeedsReauthentication != needsReauthentication.Value ? this.AccountStore.SetNeedsReauthentication((AccountKey) account, needsReauthentication.Value) : account;

    public async Task<Account> CreateAccountWithUIAsync(
      IntPtr parentWindowHandle,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.vsAccountProviderShim != null)
        return await this.vsAccountProviderShim.CreateAccountWithUIAsync(parentWindowHandle, cancellationToken);
      string queryParameters = VSAccountProvider.GetExtraQueryParameters(this.ExtraQueryParametersOverride, VSAccountProvider.ExtraQueryParametersRegistryOverride, (Account) null);
      Trace.WriteLine(string.Format("Resolved VSTS Endpoint during account creation as :{0}", (object) AccountManager.VsoEndpoint));
      AccountManager.Logger.LogEvent("Prompt", (IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "AuthType",
          (object) "Create"
        },
        {
          "Source",
          (object) "AccountSetting-Picker"
        }
      });
      Account account = (Account) null;
      List<Exception> errors = new List<Exception>();
      string userIdentifierAsync = await this.accountCache.GetAnyUserIdentifierAsync();
      IAccountCacheItem result = await this.AcquireTokenWithPrompt(parentWindowHandle, userIdentifierAsync, "common", queryParameters);
      if (result != null)
      {
        try
        {
          account = await VSAccountProvider.AddNewAccountToStoreAsync(this.Configuration, errors, this.AcquireTokenNoPrompt, this.accountCache, this.accountStore, this.GetSelfIdentity, result.Username, result.TenantId, result.Environment, result.UniqueId, cancellationToken, result.IdToken);
        }
        catch (Exception ex)
        {
          errors.Add(ex);
        }
      }
      this.RaiseAccountProcessingEnd(false, errors);
      if (account == null)
      {
        AggregateException ex = new AggregateException("Errors creating account", (IEnumerable<Exception>) errors);
        if (result != null)
          this.AddDatatoException((Exception) ex, (object) "EmailAddress", (object) result.Username);
        throw ex;
      }
      return account;
    }

    internal Account FindAccount(string userIdentifier)
    {
      ArgumentUtility.CheckForNull<string>(userIdentifier, nameof (userIdentifier));
      return this.AccountStore == null ? (Account) null : this.AccountStore.GetAllAccounts().FirstOrDefault<Account>((Func<Account, bool>) (acc =>
      {
        if (acc.ProviderId != this.AccountProviderId)
          return false;
        return Guid.TryParse(userIdentifier, out Guid _) ? VSAccountProvider.GetTenantInformation(acc).Any<TenantInformation>((Func<TenantInformation, bool>) (ti => ((IEnumerable<string>) ti.UniqueIds).Any<string>((Func<string, bool>) (id => string.Equals(id, userIdentifier, StringComparison.OrdinalIgnoreCase))))) : string.Equals(acc.DisplayInfo.UserName, userIdentifier, StringComparison.OrdinalIgnoreCase);
      }));
    }

    public void Initialize(IAccountStore store)
    {
      ArgumentUtility.CheckForNull<IAccountStore>(store, nameof (store));
      this.accountStore = store;
      this.accountStore.KeychainAccountStoreChanging += new EventHandler<AccountStoreChangedEventArgs>(this.AccountStore_KeychainAccountStoreChanging);
    }

    private void AccountStore_KeychainAccountStoreChanging(
      object sender,
      AccountStoreChangedEventArgs e)
    {
      if (e == null || e.Removed == null)
        return;
      foreach (Account account in (IEnumerable<Account>) e.Removed)
        this.ClearTokensForAccount(account);
    }

    public Task<Account> AuthenticateAccountWithUIAsync(
      AccountKey accountKey,
      IntPtr parentWindowHandle,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<AccountKey>(accountKey, nameof (accountKey));
      if (this.vsAccountProviderShim != null)
        return this.vsAccountProviderShim.AuthenticateAccountWithUIAsync(accountKey, parentWindowHandle, cancellationToken);
      AccountManager.Logger.LogEvent("Prompt", (IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "AuthType",
          (object) "Reauth"
        },
        {
          "Source",
          (object) "AccountSetting-Picker"
        }
      });
      return this.AuthenticateAccountWithUIAsyncInternal(accountKey, parentWindowHandle, (IEnumerable<ScopeInfo>) null, (string) null, cancellationToken);
    }

    internal Account FindAccount(AccountKey accountKey, bool throwOnAccountNotFound = true)
    {
      ArgumentUtility.CheckForNull<AccountKey>(accountKey, nameof (accountKey));
      Account accountFromKey = this.AccountStore.GetAccountFromKey(accountKey);
      return !(accountFromKey == null & throwOnAccountNotFound) ? accountFromKey : throw new ArgumentException(ClientResources.VsAccountProviderAccountNotFoundFromKey());
    }

    internal async Task<Account> AuthenticateAccountWithUIAsyncInternal(
      AccountKey accountKey,
      IntPtr parentWindowHandle,
      IEnumerable<ScopeInfo> scopes,
      string extraQueryParameters,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<AccountKey>(accountKey, nameof (accountKey));
      Account accountForReauthentication = this.FindAccount(accountKey);
      if (accountForReauthentication == null)
        throw new ArgumentException(ClientResources.VsAccountProviderAccountNotFoundFromKey());
      Trace.WriteLine(string.Format("Resolved VSTS Endpoint during account reauthentication as :{0}", (object) AccountManager.VsoEndpoint));
      List<Exception> errors = new List<Exception>();
      extraQueryParameters = extraQueryParameters ?? this.GetExtraQueryParameters(accountForReauthentication);
      Account result = (Account) null;
      IAccountCacheItem resultWrapper = await this.ReauthenticateWithUIInternal(parentWindowHandle, accountForReauthentication, extraQueryParameters);
      if (resultWrapper != null)
      {
        try
        {
          result = await VSAccountProvider.AddOrUpdateAccountToStoreAsync(this.Configuration, errors, this.AcquireTokenNoPrompt, this.accountCache, this.GetSelfIdentity, resultWrapper.Username, resultWrapper.TenantId, resultWrapper.Environment, resultWrapper.UniqueId, this.AccountStore, accountForReauthentication, scopes, cancellationToken, resultWrapper.IdToken);
          if (result != null)
            await this.sessionTokenStore.RefreshSessionTokensForAccountAsync(result);
        }
        catch (Exception ex)
        {
          errors.Add(ex);
        }
      }
      if (result == null)
      {
        this.ClearScopes((AccountKey) accountForReauthentication);
        this.SetAccountNeedsReauthentication(accountForReauthentication, new bool?(true));
        this.RaiseAccountProcessingEnd(false, errors);
        AggregateException ex = new AggregateException("Errors creating account", (IEnumerable<Exception>) errors);
        if (resultWrapper != null)
          this.AddDatatoException((Exception) ex, (object) "EmailAddress", (object) resultWrapper.Username);
        throw ex;
      }
      this.SetAccountNeedsReauthentication(accountForReauthentication, new bool?(false));
      this.RaiseAccountProcessingEnd(false, errors);
      Account account = result;
      accountForReauthentication = (Account) null;
      errors = (List<Exception>) null;
      result = (Account) null;
      resultWrapper = (IAccountCacheItem) null;
      return account;
    }

    public Task<Account> AuthenticateAndApplyScopeWithUIAsync(
      AccountKey accountKey,
      IntPtr parentWindowHandle,
      List<ScopeInfo> scopes,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<AccountKey>(accountKey, nameof (accountKey));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) scopes, nameof (scopes));
      return this.vsAccountProviderShim != null ? this.vsAccountProviderShim.AuthenticateAndApplyScopeWithUIAsync(accountKey, parentWindowHandle, (IEnumerable<ScopeInfo>) scopes, cancellationToken) : this.AuthenticateAccountWithUIAsyncInternal(accountKey, parentWindowHandle, (IEnumerable<ScopeInfo>) scopes, (string) null, cancellationToken);
    }

    public Account ClearScopes(AccountKey accountKey)
    {
      ArgumentUtility.CheckForNull<AccountKey>(accountKey, nameof (accountKey));
      if (this.vsAccountProviderShim != null)
        return this.vsAccountProviderShim.ClearScopes(accountKey);
      List<TenantInformation> list = VSAccountProvider.GetTenantInformation(this.FindAccount(accountKey)).Where<TenantInformation>((Func<TenantInformation, bool>) (info => info.IsOwned)).ToList<TenantInformation>();
      return this.AccountStore.SetProperties(accountKey, (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        ["Scopes"] = (string) null,
        ["Tenants"] = JsonConvert.SerializeObject((object) list)
      });
    }

    public async Task<Account> RefreshDisplayInfoAsync(
      AccountKey accountKey,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<AccountKey>(accountKey, nameof (accountKey));
      if (this.vsAccountProviderShim != null)
        return await this.vsAccountProviderShim.RefreshDisplayInfoAsync(accountKey, cancellationToken);
      Account account = this.FindAccount(accountKey);
      bool flag = VSAccountProvider.IsMSAAccount(account.GetIdentityProviderProperty());
      AccountDisplayInfo info = new AccountDisplayInfo(account.DisplayInfo.AccountDisplayName, account.DisplayInfo.ProviderDisplayName, account.DisplayInfo.UserName, (byte[]) null, flag ? VSAccountProvider.msaAccountLogo : VSAccountProvider.defaultProviderImage);
      return this.AccountStore.SetDisplayInfo(accountKey, info);
    }

    private static List<TenantInformation> GetTenantInformation(Account account)
    {
      ArgumentUtility.CheckForNull<Account>(account, nameof (account));
      List<TenantInformation> tenantInformationList = (List<TenantInformation>) null;
      string str;
      if (account.Properties.TryGetValue("Tenants", out str))
      {
        if (!string.IsNullOrEmpty(str))
        {
          try
          {
            tenantInformationList = JsonConvert.DeserializeObject<List<TenantInformation>>(str);
          }
          catch (Exception ex)
          {
            Trace.WriteLine(string.Format("GetTenantInformation failed for account '{0}' because of '{1}'", (object) account.UniqueId, (object) ex));
          }
        }
      }
      return tenantInformationList ?? new List<TenantInformation>();
    }

    internal static IEnumerable<TenantInformation> GetTenantsInScopeInternal(Account account)
    {
      ArgumentUtility.CheckForNull<Account>(account, nameof (account));
      List<ScopeInfo> scopes = VSAccountProvider.GetScopesForAccountInternal(account).ToList<ScopeInfo>();
      List<TenantInformation> tenantInformation = VSAccountProvider.GetTenantInformation(account);
      Guid tenantId;
      return !scopes.Any<ScopeInfo>() ? (IEnumerable<TenantInformation>) tenantInformation : tenantInformation.Where<TenantInformation>((Func<TenantInformation, bool>) (information => Guid.TryParse(information.TenantId, out tenantId) && scopes.Any<ScopeInfo>((Func<ScopeInfo, bool>) (scope => scope.TenantId == tenantId))));
    }

    public IEnumerable<TenantInformation> GetTenantsInScope(AccountKey accountKey)
    {
      ArgumentUtility.CheckForNull<AccountKey>(accountKey, nameof (accountKey));
      return this.vsAccountProviderShim != null ? this.vsAccountProviderShim.GetTenantsInScope(accountKey) : VSAccountProvider.GetTenantsInScopeInternal(this.FindAccount(accountKey));
    }

    public TenantInformation GetHomeTenantInfo(AccountKey accountKey)
    {
      ArgumentUtility.CheckForNull<AccountKey>(accountKey, nameof (accountKey));
      if (this.vsAccountProviderShim != null)
        return this.vsAccountProviderShim.GetHomeTenantInfo(accountKey);
      Account account = this.FindAccount(accountKey);
      string homeTenantId = account.GetHomeTenantId();
      return VSAccountProvider.GetTenantInformation(account).SingleOrDefault<TenantInformation>((Func<TenantInformation, bool>) (ti => string.Equals(homeTenantId, ti.TenantId, StringComparison.OrdinalIgnoreCase)));
    }

    private void AddDatatoException(Exception ex, object key, object value)
    {
      ArgumentUtility.CheckForNull<Exception>(ex, "Exception");
      ArgumentUtility.CheckForNull<object>(key, "Key");
      ArgumentUtility.CheckForNull<object>(value, "Value");
      ex.Data.Add(key, value);
    }

    internal delegate Task DeleteTokenCacheItem(IAccountCacheItem item);

    internal delegate Task<IdentitySelf> TryGetSelfAsync(
      List<Exception> errors,
      string tenantId,
      string uniqueId,
      CancellationToken cancellationToken);

    internal delegate List<IAccountCacheItem> GetWrappedItemsFromCache(IAccountCache accountCache);

    internal delegate Task<IAccountCacheItem> AcquireTokenSilently(
      IAadProviderConfiguration config,
      IAccountCache accountCache,
      string tenantId,
      string userIdentifier,
      string[] scopes);

    internal delegate Task<bool> AcquireTokenSilentlyAsync(
      IAadProviderConfiguration config,
      IAccountCache accountCache,
      KeyValuePair<string, string> userIdTenantIdPair,
      string[] scopes);

    internal delegate Task<IAccountCacheItem> AcquireTokenWithUI(
      IntPtr parentWindowHandle,
      string userIdentifier,
      string tenantId,
      string extraQueryParameters);

    internal class AcquireTokenLogEvent
    {
      internal static ILogger Logger = AccountManager.Logger;

      public bool Prompted { get; set; }

      public bool Succeeded { get; set; }

      public string SilentError { get; private set; }

      public string PromptError { get; private set; }

      public void SetSilentError(Exception exception) => this.SilentError = VSAccountProvider.AcquireTokenLogEvent.GetError(exception);

      public void SetPromptError(Exception exception) => this.PromptError = VSAccountProvider.AcquireTokenLogEvent.GetError(exception);

      public void Log() => VSAccountProvider.AcquireTokenLogEvent.Logger.LogEvent("VSAccountProviderAcquireToken", (IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "Prompted",
          (object) this.Prompted
        },
        {
          "Succeeded",
          (object) this.Succeeded
        },
        {
          "SilentError",
          (object) this.SilentError
        },
        {
          "PromptError",
          (object) this.PromptError
        }
      });

      private static string GetError(Exception exception)
      {
        if (exception == null)
          return string.Empty;
        return exception is MsalClientException msalClientException ? msalClientException.ErrorCode : exception.GetType().FullName;
      }
    }
  }
}
