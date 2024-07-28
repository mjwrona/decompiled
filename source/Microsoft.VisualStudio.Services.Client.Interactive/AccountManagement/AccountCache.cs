// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.AccountCache
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.VisualStudio.Services.Client.Keychain.VSProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  internal class AccountCache : IAccountCache
  {
    private readonly Storage m_storage;
    private readonly IPublicClientApplication m_application;
    private readonly string m_lockfilePath;
    private readonly string m_authorityBase;
    private readonly bool m_validateAuthority;

    public AccountCache(
      IAccountCacheConfiguration cacheConfig,
      IAadProviderConfiguration aadConfig,
      string tenantId,
      string queryParameters = null)
    {
      this.m_lockfilePath = cacheConfig.LockfilePath;
      this.m_authorityBase = aadConfig.AadAuthorityBase;
      this.m_validateAuthority = aadConfig.ValidateAadAuthority;
      string authorityUri = aadConfig.AadAuthorityBase + tenantId;
      HttpClientFactoryWithUserAgent factoryWithUserAgent = new HttpClientFactoryWithUserAgent();
      PublicClientApplicationBuilder applicationBuilder = PublicClientApplicationBuilder.Create(aadConfig.ClientIdentifier).WithHttpClientFactory((IMsalHttpClientFactory) factoryWithUserAgent).WithAuthority(authorityUri, aadConfig.ValidateAadAuthority).WithRedirectUri(aadConfig.NativeClientRedirect.AbsoluteUri);
      if (!string.IsNullOrWhiteSpace(queryParameters))
        applicationBuilder.WithExtraQueryParameters(queryParameters);
      this.m_application = applicationBuilder.Build();
      StorageCreationProperties creationProperties = new StorageCreationPropertiesBuilder(cacheConfig.FileName, cacheConfig.DirectoryPath).WithLinuxKeyring(cacheConfig.LinuxKeyRingSchema, cacheConfig.LinuxKeyRingCollection, cacheConfig.LinuxKeyRingLabel, cacheConfig.LinuxKeyRingAttr1, cacheConfig.LinuxKeyRingAttr2).WithMacKeyChain(cacheConfig.MacKeyChainService, cacheConfig.MacKeyChainAccount).Build();
      this.m_storage = Storage.Create(creationProperties, (TraceSource) null);
      MsalCacheHelper.CreateAsync(creationProperties, (TraceSource) null).SyncResultConfigured<MsalCacheHelper>().RegisterCache(this.m_application.UserTokenCache);
    }

    public IEnumerable<IAccountCacheItem> GetItems()
    {
      using (new CrossPlatLock(this.m_lockfilePath, 100, 600))
      {
        using (MemoryStream memoryStream = new MemoryStream(this.m_storage.ReadData()))
        {
          using (StreamReader reader = new StreamReader((Stream) memoryStream, Encoding.UTF8))
            return JsonSerializer.Create().Deserialize<IEnumerable<IAccountCacheItem>>((JsonReader) new JsonTextReader((TextReader) reader)) ?? Enumerable.Empty<IAccountCacheItem>();
        }
      }
    }

    public Task<IEnumerable<IAccountCacheItem>> GetItemsAsync() => Task.FromResult<IEnumerable<IAccountCacheItem>>(this.GetItems());

    public async Task<IAccountCacheItem> AcquireTokenSilentAsync(
      string[] scopes,
      string userIdentifier,
      string tenantId = null)
    {
      IAccount accountAsync = await this.m_application.GetAccountAsync(userIdentifier);
      AcquireTokenSilentParameterBuilder parameterBuilder = this.m_application.AcquireTokenSilent((IEnumerable<string>) scopes, accountAsync);
      if (!string.IsNullOrWhiteSpace(tenantId))
      {
        string authorityUri = this.m_authorityBase + tenantId;
        parameterBuilder.WithAuthority(authorityUri, this.m_validateAuthority);
      }
      return (IAccountCacheItem) new AccountCacheItem(await parameterBuilder.ExecuteAsync());
    }

    public async Task DeleteItemAsync(IAccountCacheItem token) => await this.m_application.RemoveAsync(await this.m_application.GetAccountAsync(token.UniqueId));

    public async Task<IAccountCacheItem> AcquireTokenInteractiveAsync(
      string[] scopes,
      Prompt prompt = default (Prompt),
      string userIdentifier = null,
      string tenantId = null)
    {
      AcquireTokenInteractiveParameterBuilder query = this.m_application.AcquireTokenInteractive((IEnumerable<string>) scopes).WithExtraScopesToConsent((IEnumerable<string>) new string[1]
      {
        "scope"
      });
      if (prompt != new Prompt())
        query.WithPrompt(prompt);
      if (!string.IsNullOrWhiteSpace(userIdentifier))
        query.WithAccount(await this.m_application.GetAccountAsync(userIdentifier));
      if (!string.IsNullOrWhiteSpace(tenantId))
        query.WithTenantId(tenantId);
      IAccountCacheItem accountCacheItem = (IAccountCacheItem) new AccountCacheItem(await query.ExecuteAsync());
      query = (AcquireTokenInteractiveParameterBuilder) null;
      return accountCacheItem;
    }

    public async Task<string> GetAnyUserIdentifierAsync() => (await this.m_application.GetAccountsAsync()).FirstOrDefault<IAccount>()?.HomeAccountId.ObjectId;

    public IEnumerable<IAccountCacheItem> GetVsoEndpointToken(IAccountCacheItem tokenCacheItem) => this.GetItems().Where<IAccountCacheItem>((Func<IAccountCacheItem, bool>) (item =>
    {
      if (!string.Equals(item.UniqueId, tokenCacheItem.UniqueId, StringComparison.OrdinalIgnoreCase))
        return false;
      AuthenticationResult innerResult = item.InnerResult;
      bool? nullable;
      if (innerResult == null)
      {
        nullable = new bool?();
      }
      else
      {
        IEnumerable<string> scopes = innerResult.Scopes;
        nullable = scopes != null ? new bool?(scopes.AreSetsEqual<string>((IEnumerable<string>) VssAadSettings.DefaultScopes, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)) : new bool?();
      }
      return nullable.GetValueOrDefault();
    }));

    public Task<IEnumerable<IAccountCacheItem>> GetVsoEndpointTokenAsync(
      IAccountCacheItem tokenCacheItem)
    {
      return Task.FromResult<IEnumerable<IAccountCacheItem>>(this.GetVsoEndpointToken(tokenCacheItem));
    }
  }
}
