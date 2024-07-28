// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.AccountLicenseUsage
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class AccountLicenseUsage
  {
    public AccountLicenseUsage()
      : this(new AccountUserLicense(LicensingSource.Account, 0), 0, 0)
    {
    }

    public AccountLicenseUsage(AccountUserLicense license, int provisionedCount, int usedCount)
      : this(license, provisionedCount, usedCount, 0, provisionedCount)
    {
    }

    public AccountLicenseUsage(
      AccountUserLicense license,
      int provisionedCount,
      int usedCount,
      int disabledCount,
      int pendingProvisionedCount)
    {
      this.License = license;
      this.ProvisionedCount = provisionedCount;
      this.UsedCount = usedCount;
      this.DisabledCount = disabledCount;
      this.PendingProvisionedCount = pendingProvisionedCount;
    }

    public virtual AccountUserLicense License { get; set; }

    public int ProvisionedCount { get; set; }

    public int UsedCount { get; set; }

    public int DisabledCount { get; set; }

    public int PendingProvisionedCount { get; set; }
  }
}
