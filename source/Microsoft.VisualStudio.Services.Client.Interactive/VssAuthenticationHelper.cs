// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssAuthenticationHelper
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Services.Client.AccountManagement;
using Microsoft.VisualStudio.Services.Client.Controls;
using Microsoft.VisualStudio.Services.Client.Controls.AccountPicker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Microsoft.VisualStudio.Services.Client
{
  public static class VssAuthenticationHelper
  {
    private static IAccountCache AccountCache => AccountManager.DefaultInstance.GetCache<IAccountCache>();

    private static string GetTenantIdFromAccountStore(string user)
    {
      string fromAccountStore = (string) null;
      List<Microsoft.VisualStudio.Services.Client.AccountManagement.Account> list = AccountManager.DefaultInstance.Store.GetAllAccounts().Where<Microsoft.VisualStudio.Services.Client.AccountManagement.Account>((Func<Microsoft.VisualStudio.Services.Client.AccountManagement.Account, bool>) (account => string.Equals(account.DisplayInfo.UserName, user, StringComparison.OrdinalIgnoreCase))).ToList<Microsoft.VisualStudio.Services.Client.AccountManagement.Account>();
      if (list.Count == 1)
        fromAccountStore = list.First<Microsoft.VisualStudio.Services.Client.AccountManagement.Account>().Authenticator;
      return fromAccountStore;
    }

    private static string ApplicationTenantId => VssAadSettings.ApplicationTenant;

    public static async Task<bool> CheckForValidTokenByTenantAsync(
      VssFederatedCredentialPrompt federatedPrompt,
      string tenant)
    {
      if (!string.IsNullOrEmpty(tenant) && string.Equals(tenant, Guid.Empty.ToString(), StringComparison.OrdinalIgnoreCase))
        tenant = VssAuthenticationHelper.ApplicationTenantId;
      string user;
      if (federatedPrompt.Parameters != null && federatedPrompt.Parameters.TryGetValue("user", out user) && !string.IsNullOrWhiteSpace(user))
        return await VssAuthenticationHelper.CheckForValidTokenByUserAndTenantAsync(federatedPrompt, user, tenant);
      try
      {
        VssAuthenticationHelper.CleanupAccessTokenInPromptParameters(federatedPrompt);
        VssAuthenticationHelper.CleanupUserInfoInPromptParameters(federatedPrompt);
        List<IAccountCacheItem> list = (await VssAuthenticationHelper.FilterForDistinctTenantUserAsync(VssAuthenticationHelper.AccountCache.GetItems().Where<IAccountCacheItem>((Func<IAccountCacheItem, bool>) (x => x.TenantId == tenant)).ToList<IAccountCacheItem>(), tenant)).ToList<IAccountCacheItem>();
        if (list.Any<IAccountCacheItem>())
        {
          IAccountCacheItem tokenCacheItem = VssAuthenticationHelper.ShowAccountPicker((IEnumerable<IAccountCacheItem>) list);
          if (tokenCacheItem != null)
          {
            int num = VssAuthenticationHelper.TrySetTokenForAuth(federatedPrompt, tokenCacheItem) ? 1 : 0;
            if (num == 0)
            {
              if (federatedPrompt.Parameters == null)
                federatedPrompt.Parameters = (IDictionary<string, string>) new Dictionary<string, string>();
              federatedPrompt.Parameters["user"] = tokenCacheItem.Username;
            }
            return num != 0;
          }
        }
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public static async Task<bool> CheckForValidTokenByUserAndTenantAsync(
      VssFederatedCredentialPrompt federatedPrompt,
      string user,
      string tenant)
    {
      try
      {
        VssAuthenticationHelper.CleanupAccessTokenInPromptParameters(federatedPrompt);
        if (string.IsNullOrEmpty(tenant))
          tenant = VssAuthenticationHelper.GetTenantIdFromAccountStore(user);
        if (tenant != null)
        {
          List<IAccountCacheItem> list = (await VssAuthenticationHelper.FilterForDistinctTenantUserAsync(VssAuthenticationHelper.AccountCache.GetItems().Where<IAccountCacheItem>((Func<IAccountCacheItem, bool>) (x => string.Equals(x.Username, user, StringComparison.OrdinalIgnoreCase) && string.Equals(x.TenantId, tenant, StringComparison.OrdinalIgnoreCase))).ToList<IAccountCacheItem>(), tenant)).ToList<IAccountCacheItem>();
          if (list.Count == 1)
            return VssAuthenticationHelper.TrySetTokenForAuth(federatedPrompt, list[0]);
        }
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    private static IAccountCacheItem ShowAccountPicker(IEnumerable<IAccountCacheItem> cacheItemList)
    {
      AccountPickerWindow state = new AccountPickerWindow(cacheItemList);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      bool? nullable = new DialogHost().InvokeDialogAsync(VssAuthenticationHelper.\u003C\u003EO.\u003C0\u003E__InvokeDialog ?? (VssAuthenticationHelper.\u003C\u003EO.\u003C0\u003E__InvokeDialog = new InvokeDialogFunc(VssAuthenticationHelper.InvokeDialog)), (object) state).SyncResultConfigured<bool?>();
      bool flag = true;
      if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
      {
        CachedAccount selectedAccount = state.SelectedAccount;
        if (selectedAccount != null)
          return selectedAccount.CachedToken;
      }
      return (IAccountCacheItem) null;
    }

    private static bool? InvokeDialog(IntPtr owner, object state)
    {
      if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
        throw new InvalidOperationException(ClientResources.STAThreadRequired());
      if (!(state is AccountPickerWindow accountPickerWindow))
        return new bool?(false);
      if (owner == IntPtr.Zero)
        owner = ClientNativeMethods.GetDefaultParentWindow();
      if (owner != IntPtr.Zero)
        new WindowInteropHelper((Window) accountPickerWindow).Owner = owner;
      return accountPickerWindow.ShowDialog();
    }

    private static bool TrySetTokenForAuth(
      VssFederatedCredentialPrompt federatedPrompt,
      IAccountCacheItem tokenCacheItem)
    {
      string str = VssAuthenticationHelper.VerifyAuthWithToken(tokenCacheItem);
      if (string.IsNullOrWhiteSpace(str) || federatedPrompt == null)
        return false;
      if (federatedPrompt.Parameters != null)
        federatedPrompt.Parameters["accessToken"] = str;
      else
        federatedPrompt.Parameters = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal)
        {
          {
            "accessToken",
            str
          }
        };
      return true;
    }

    private static string VerifyAuthWithToken(IAccountCacheItem tokenCacheItem)
    {
      Uri vsoEndpoint = AccountManager.VsoEndpoint;
      IAccountCacheItem accountCacheItem = (IAccountCacheItem) null;
      try
      {
        IEnumerable<IAccountCacheItem> vsoEndpointToken = VssAuthenticationHelper.AccountCache.GetVsoEndpointToken(tokenCacheItem);
        if (vsoEndpointToken != null)
        {
          if (vsoEndpointToken.Count<IAccountCacheItem>() == 1)
            accountCacheItem = vsoEndpointToken.First<IAccountCacheItem>();
        }
      }
      catch (Exception ex)
      {
      }
      if (accountCacheItem != null)
      {
        try
        {
          IdentitySelf identitySelf = new IdentityHttpClient(vsoEndpoint, (VssCredentials) (FederatedCredential) new VssAadCredential(new VssAadToken("Bearer", accountCacheItem.AccessToken))).GetIdentitySelfAsync((object) null, new CancellationToken()).SyncResultConfigured<IdentitySelf>();
          if (identitySelf != null && string.Equals(identitySelf.AccountName, tokenCacheItem.Username, StringComparison.OrdinalIgnoreCase))
            return accountCacheItem.AccessToken;
          Trace.WriteLine("VssAuthenticationHelper TryAuthenticationWithToken: identity is null from Service");
        }
        catch (Exception ex)
        {
        }
      }
      return (string) null;
    }

    private static void CleanupAccessTokenInPromptParameters(
      VssFederatedCredentialPrompt federatedPrompt)
    {
      if (federatedPrompt == null || federatedPrompt.Parameters == null)
        return;
      federatedPrompt.Parameters.Remove("accessToken");
    }

    private static void CleanupUserInfoInPromptParameters(
      VssFederatedCredentialPrompt federatedPrompt)
    {
      if (federatedPrompt == null || federatedPrompt.Parameters == null)
        return;
      federatedPrompt.Parameters.Remove("user");
    }

    private static async Task<IEnumerable<IAccountCacheItem>> FilterForDistinctTenantUserAsync(
      List<IAccountCacheItem> list,
      string tenantId)
    {
      List<IAccountCacheItem> possibleTokenCacheItemList = new List<IAccountCacheItem>();
      if (list == null || !list.Any<IAccountCacheItem>())
        return (IEnumerable<IAccountCacheItem>) possibleTokenCacheItemList;
      foreach (string username in list.Select<IAccountCacheItem, string>((Func<IAccountCacheItem, string>) (x => x.Username)).Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        IAccountCacheItem result = (IAccountCacheItem) null;
        try
        {
          result = await VssAuthenticationHelper.AccountCache.AcquireTokenSilentAsync(VssAadSettings.DefaultScopes, username, tenantId);
        }
        catch (MsalUiRequiredException ex)
        {
          IAccountCache accountCache = VssAuthenticationHelper.AccountCache;
          string[] defaultScopes = VssAadSettings.DefaultScopes;
          string str1 = username;
          string str2 = tenantId;
          Prompt prompt = new Prompt();
          string userIdentifier = str1;
          string tenantId1 = str2;
          result = await accountCache.AcquireTokenInteractiveAsync(defaultScopes, prompt, userIdentifier, tenantId1);
        }
        catch (Exception ex)
        {
          Trace.WriteLine(string.Format("Failed to acquire token for user {0} : {1}", (object) username, (object) ex.ToReadableStackTrace()));
        }
        if (result != null)
          possibleTokenCacheItemList.Add(result);
        result = (IAccountCacheItem) null;
      }
      return (IEnumerable<IAccountCacheItem>) possibleTokenCacheItemList;
    }
  }
}
