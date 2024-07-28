// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.AccountDisplayInfo
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public class AccountDisplayInfo
  {
    public static readonly AccountDisplayInfoNoLogoComparer DisplayInfoNoLogoComparer = new AccountDisplayInfoNoLogoComparer();

    public AccountDisplayInfo(
      string accountDisplayName,
      string providerDisplayName,
      string userName,
      byte[] accountLogo,
      byte[] providerLogo)
    {
      ArgumentUtility.CheckForNull<string>(accountDisplayName, nameof (accountDisplayName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(providerDisplayName, nameof (providerDisplayName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(userName, nameof (userName));
      this.AccountDisplayName = accountDisplayName;
      this.ProviderDisplayName = providerDisplayName;
      this.UserName = userName;
      this.AccountLogo = accountLogo;
      this.ProviderLogo = providerLogo;
    }

    public string AccountDisplayName { get; private set; }

    public string ProviderDisplayName { get; private set; }

    public string UserName { get; private set; }

    public byte[] AccountLogo { get; private set; }

    public byte[] ProviderLogo { get; private set; }
  }
}
