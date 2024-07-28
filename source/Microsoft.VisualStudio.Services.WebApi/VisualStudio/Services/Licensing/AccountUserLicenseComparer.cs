// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.AccountUserLicenseComparer
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class AccountUserLicenseComparer : IComparer<AccountUserLicense>
  {
    public int Compare(AccountUserLicense x, AccountUserLicense y)
    {
      Func<AccountUserLicense, License> func = (Func<AccountUserLicense, License>) (userLicense => userLicense.Source != LicensingSource.Account ? MsdnLicense.GetLicense((MsdnLicenseType) userLicense.License) : AccountLicense.GetLicense((AccountLicenseType) userLicense.License));
      return LicenseComparer.Instance.Compare(func(x), func(y));
    }

    public static AccountUserLicenseComparer Instance { get; } = new AccountUserLicenseComparer();
  }
}
