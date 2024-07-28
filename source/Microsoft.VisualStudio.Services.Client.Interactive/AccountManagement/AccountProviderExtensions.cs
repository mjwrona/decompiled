// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.AccountProviderExtensions
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public static class AccountProviderExtensions
  {
    public static async Task RefreshAuthenticationStateAsync(
      this IAccountProvider provider,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (provider == null)
        throw new ArgumentNullException(nameof (provider));
      if (provider.AccountStore == null)
        ;
      else
      {
        foreach (Account account1 in provider.AccountStore.GetAllAccounts().Where<Account>((Func<Account, bool>) (account => account.ProviderId == provider.AccountProviderId)))
        {
          cancellationToken.ThrowIfCancellationRequested();
          Account account2 = await provider.RefreshAuthenticationStateAsync((AccountKey) account1, cancellationToken);
        }
      }
    }
  }
}
