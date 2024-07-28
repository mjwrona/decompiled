// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicenseComparer
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class LicenseComparer : IComparer<License>
  {
    public int Compare(License x, License y)
    {
      if (x == (License) null && y == (License) null)
        return 0;
      if (x == (License) null)
        return -1;
      if (y == (License) null)
        return 1;
      return (x.Source == LicensingSource.Account || x.Source == LicensingSource.Msdn) && (y.Source == LicensingSource.Account || y.Source == LicensingSource.Msdn) ? this.GetWeight(x).CompareTo(this.GetWeight(y)) : x.GetLicenseAsInt32().CompareTo(y.GetLicenseAsInt32());
    }

    public int GetWeight(License license)
    {
      if (license == License.None)
        return 0;
      if (license == (License) AccountLicense.Stakeholder)
        return 1;
      if (license == (License) AccountLicense.Express)
        return 2;
      if (license == (License) AccountLicense.Professional)
        return 3;
      if (license == (License) MsdnLicense.Eligible)
        return 4;
      if (license == (License) MsdnLicense.Professional)
        return 5;
      if (license == (License) AccountLicense.Advanced)
        return 6;
      if (license == (License) MsdnLicense.TestProfessional)
        return 7;
      if (license == (License) MsdnLicense.Platforms)
        return 8;
      if (license == (License) MsdnLicense.Premium)
        return 9;
      if (license == (License) MsdnLicense.Ultimate)
        return 10;
      if (license == (License) MsdnLicense.Enterprise)
        return 11;
      return license == (License) AccountLicense.EarlyAdopter ? 12 : 0;
    }

    public static LicenseComparer Instance { get; } = new LicenseComparer();
  }
}
