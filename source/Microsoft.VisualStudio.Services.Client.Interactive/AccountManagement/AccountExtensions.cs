// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.AccountExtensions
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public static class AccountExtensions
  {
    private static void VerifyVSAccountProviderAccount(Account account)
    {
      ArgumentUtility.CheckForNull<Account>(account, nameof (account));
      if (account.ProviderId != VSAccountProvider.AccountProviderIdentifier)
        throw new ArgumentException(ClientResources.VsAccountProviderUnsupportedAccount());
    }

    public static string GetIdentityProviderProperty(this Account account)
    {
      AccountExtensions.VerifyVSAccountProviderAccount(account);
      string providerProperty = (string) null;
      account.Properties.TryGetValue("IdentityProvider", out providerProperty);
      return providerProperty;
    }

    public static Account SetPersonalizationAccount(this Account account, IAccountStore store) => store?.SetProperty((AccountKey) account, "VisualStudioPersonalizationAccount", "true");

    public static bool IsPersonalizationAccount(this Account account)
    {
      string str = (string) null;
      account.Properties.TryGetValue("VisualStudioPersonalizationAccount", out str);
      bool result = false;
      if (str != null)
        bool.TryParse(str, out result);
      return result;
    }

    public static string GetIdTokenPayload(this Account account)
    {
      AccountExtensions.VerifyVSAccountProviderAccount(account);
      string idTokenPayload = (string) null;
      account.Properties.TryGetValue("IdTokenPayload", out idTokenPayload);
      return idTokenPayload;
    }

    public static string GetHomeTenantId(this Account account)
    {
      AccountExtensions.VerifyVSAccountProviderAccount(account);
      string homeTenantId;
      account.Properties.TryGetValue("HomeTenant", out homeTenantId);
      return homeTenantId;
    }
  }
}
