// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.IAccountProvider
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public interface IAccountProvider
  {
    Guid AccountProviderId { get; }

    IAccountStore AccountStore { get; }

    void ClearTokensForAccount(Account account);

    void Initialize(IAccountStore store);

    Task<Account> RefreshAuthenticationStateAsync(
      AccountKey accountKey,
      CancellationToken cancellationToken);

    Task<Account> AuthenticateAccountWithUIAsync(
      AccountKey accountKey,
      IntPtr parentWindowHandle,
      CancellationToken cancellationToken);

    Task<Account> CreateAccountWithUIAsync(
      IntPtr parentWindowHandle,
      CancellationToken cancellationToken);

    Task<Account> RefreshDisplayInfoAsync(
      AccountKey accountKey,
      CancellationToken cancellationToken);
  }
}
