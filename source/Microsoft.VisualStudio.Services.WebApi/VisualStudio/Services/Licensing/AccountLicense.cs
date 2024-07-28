// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.AccountLicense
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public sealed class AccountLicense : Microsoft.VisualStudio.Services.Licensing.License, IComparable<AccountLicense>
  {
    public static readonly AccountLicense EarlyAdopter = new AccountLicense(AccountLicenseType.EarlyAdopter);
    public static readonly AccountLicense Stakeholder = new AccountLicense(AccountLicenseType.Stakeholder);
    public static readonly AccountLicense Express = new AccountLicense(AccountLicenseType.Express);
    public static readonly AccountLicense Professional = new AccountLicense(AccountLicenseType.Professional);
    public static readonly AccountLicense Advanced = new AccountLicense(AccountLicenseType.Advanced);

    private AccountLicense(AccountLicenseType license)
      : base(LicensingSource.Account, typeof (AccountLicenseType), (int) license)
    {
    }

    public AccountLicenseType License => (AccountLicenseType) this.GetLicenseAsInt32();

    public int CompareTo(AccountLicense other) => AccountLicense.Compare(this, other);

    public static int Compare(AccountLicense left, AccountLicense right) => left == null ? (right == null ? 0 : -1) : (right == null ? 1 : LicenseComparer.Instance.Compare((Microsoft.VisualStudio.Services.Licensing.License) left, (Microsoft.VisualStudio.Services.Licensing.License) right));

    public static bool operator >(AccountLicense left, AccountLicense right) => AccountLicense.Compare(left, right) > 0;

    public static bool operator <(AccountLicense left, AccountLicense right) => AccountLicense.Compare(left, right) < 0;

    public static Microsoft.VisualStudio.Services.Licensing.License GetLicense(
      AccountLicenseType license)
    {
      switch (license)
      {
        case AccountLicenseType.None:
          return Microsoft.VisualStudio.Services.Licensing.License.None;
        case AccountLicenseType.EarlyAdopter:
          return (Microsoft.VisualStudio.Services.Licensing.License) AccountLicense.EarlyAdopter;
        case AccountLicenseType.Express:
          return (Microsoft.VisualStudio.Services.Licensing.License) AccountLicense.Express;
        case AccountLicenseType.Professional:
          return (Microsoft.VisualStudio.Services.Licensing.License) AccountLicense.Professional;
        case AccountLicenseType.Advanced:
          return (Microsoft.VisualStudio.Services.Licensing.License) AccountLicense.Advanced;
        case AccountLicenseType.Stakeholder:
          return (Microsoft.VisualStudio.Services.Licensing.License) AccountLicense.Stakeholder;
        default:
          throw new InvalidEnumArgumentException(nameof (license), (int) license, typeof (AccountLicenseType));
      }
    }
  }
}
