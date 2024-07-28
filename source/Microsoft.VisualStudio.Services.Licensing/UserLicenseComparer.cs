// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.UserLicenseComparer
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.VisualStudio.Services.Account;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class UserLicenseComparer : IComparer<UserLicense>
  {
    private UserLicenseComparer()
    {
    }

    public int Compare(UserLicense x, UserLicense y)
    {
      if (x == null && y == null)
        return 0;
      if (x == null)
        return -1;
      if (y == null)
        return 1;
      if (x.Status != y.Status)
      {
        if ((x.Status == AccountUserStatus.Active || x.Status == AccountUserStatus.Pending) && y.Status != AccountUserStatus.Active && y.Status != AccountUserStatus.Pending)
          return 1;
        if ((y.Status == AccountUserStatus.Active || y.Status == AccountUserStatus.Pending) && x.Status != AccountUserStatus.Active && x.Status != AccountUserStatus.Pending)
          return -1;
      }
      if (x.Source != LicensingSource.Account && x.Source != LicensingSource.Msdn || y.Source != LicensingSource.Account && y.Source != LicensingSource.Msdn)
        return x.License.CompareTo(y.License);
      License license1 = x.Source == LicensingSource.Account ? AccountLicense.GetLicense((AccountLicenseType) x.License) : MsdnLicense.GetLicense((MsdnLicenseType) x.License);
      License license2 = y.Source == LicensingSource.Account ? AccountLicense.GetLicense((AccountLicenseType) y.License) : MsdnLicense.GetLicense((MsdnLicenseType) y.License);
      return LicenseComparer.Instance.GetWeight(license1).CompareTo(LicenseComparer.Instance.GetWeight(license2));
    }

    internal static UserLicenseComparer Instance { get; } = new UserLicenseComparer();
  }
}
