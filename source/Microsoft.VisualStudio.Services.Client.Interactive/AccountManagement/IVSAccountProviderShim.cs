// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.IVSAccountProviderShim
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Services.Client.Keychain.VSProvider;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public interface IVSAccountProviderShim
  {
    string ExtraQueryParameters { get; set; }

    IAadProviderConfiguration GetConfiguration();

    Guid GetMsaHomeTenantId();

    Task<string> ProcessAuthenticationResultAsync(AuthenticationResult authenticationResult);

    void ClearTokensForAccount(Account account);

    Task<AuthenticationResult> AcquireTokenAsync(
      string[] scopes,
      string tenantId,
      string userIdentifier,
      IntPtr parentWindowHandle,
      AccountKey accountKeyForReAuthentication = null,
      bool prompt = false,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AuthenticationResult> AcquireTokenAsyncWithContextAsync(
      string[] scopes,
      IAccountCache accountCache,
      string userIdentifier,
      AccountKey accountForReauthentication = null,
      bool prompt = false,
      string extraQueryParameters = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<string> GetSessionTokenFromAccountAsync(
      Account account,
      string scope,
      bool forceRefresh = false,
      CancellationToken cancellationToken = default (CancellationToken));

    string GetExtraQueryParameters(string authority);

    Task<Account> RefreshAuthenticationStateAsync(
      AccountKey account,
      CancellationToken cancellationToken);

    Task<Account> CreateAccountWithUIAsync(
      IntPtr parentWindowHandle,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<Account> AuthenticateAccountWithUIAsync(
      AccountKey accountKey,
      IntPtr parentWindowHandle,
      CancellationToken cancellationToken);

    Task<Account> AuthenticateAndApplyScopeWithUIAsync(
      AccountKey accountKey,
      IntPtr parentWindowHandle,
      IEnumerable<ScopeInfo> scopes,
      CancellationToken cancellationToken);

    Account ClearScopes(AccountKey accountKey);

    IEnumerable<ScopeInfo> GetScopesForAccount(AccountKey accountKey);

    TenantInformation GetHomeTenantInfo(AccountKey accountKey);

    IEnumerable<TenantInformation> GetTenantsInScope(AccountKey accountKey);

    Task<Account> RefreshDisplayInfoAsync(
      AccountKey accountKey,
      CancellationToken cancellationToken);

    Action<bool, List<Exception>> RaiseAccountProcessingDoneEvent { get; set; }
  }
}
