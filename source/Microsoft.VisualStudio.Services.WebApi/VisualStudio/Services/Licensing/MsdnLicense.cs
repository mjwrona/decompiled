// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.MsdnLicense
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public sealed class MsdnLicense : Microsoft.VisualStudio.Services.Licensing.License, IComparable<MsdnLicense>
  {
    public static readonly MsdnLicense Eligible = new MsdnLicense(MsdnLicenseType.Eligible);
    public static readonly MsdnLicense Professional = new MsdnLicense(MsdnLicenseType.Professional);
    public static readonly MsdnLicense Platforms = new MsdnLicense(MsdnLicenseType.Platforms);
    public static readonly MsdnLicense TestProfessional = new MsdnLicense(MsdnLicenseType.TestProfessional);
    public static readonly MsdnLicense Premium = new MsdnLicense(MsdnLicenseType.Premium);
    public static readonly MsdnLicense Ultimate = new MsdnLicense(MsdnLicenseType.Ultimate);
    public static readonly MsdnLicense Enterprise = new MsdnLicense(MsdnLicenseType.Enterprise);

    private MsdnLicense(MsdnLicenseType license)
      : base(LicensingSource.Msdn, typeof (MsdnLicenseType), (int) license)
    {
    }

    public MsdnLicenseType License => (MsdnLicenseType) this.GetLicenseAsInt32();

    public int CompareTo(MsdnLicense other) => MsdnLicense.Compare(this, other);

    public static int Compare(MsdnLicense left, MsdnLicense right) => left == null ? (right == null ? 0 : -1) : (right == null ? 1 : LicenseComparer.Instance.Compare((Microsoft.VisualStudio.Services.Licensing.License) left, (Microsoft.VisualStudio.Services.Licensing.License) right));

    public static bool operator >(MsdnLicense left, MsdnLicense right) => MsdnLicense.Compare(left, right) > 0;

    public static bool operator <(MsdnLicense left, MsdnLicense right) => MsdnLicense.Compare(left, right) < 0;

    public static Microsoft.VisualStudio.Services.Licensing.License GetLicense(
      MsdnLicenseType license)
    {
      switch (license)
      {
        case MsdnLicenseType.None:
          return Microsoft.VisualStudio.Services.Licensing.License.None;
        case MsdnLicenseType.Eligible:
          return (Microsoft.VisualStudio.Services.Licensing.License) MsdnLicense.Eligible;
        case MsdnLicenseType.Professional:
          return (Microsoft.VisualStudio.Services.Licensing.License) MsdnLicense.Professional;
        case MsdnLicenseType.Platforms:
          return (Microsoft.VisualStudio.Services.Licensing.License) MsdnLicense.Platforms;
        case MsdnLicenseType.TestProfessional:
          return (Microsoft.VisualStudio.Services.Licensing.License) MsdnLicense.TestProfessional;
        case MsdnLicenseType.Premium:
          return (Microsoft.VisualStudio.Services.Licensing.License) MsdnLicense.Premium;
        case MsdnLicenseType.Ultimate:
          return (Microsoft.VisualStudio.Services.Licensing.License) MsdnLicense.Ultimate;
        case MsdnLicenseType.Enterprise:
          return (Microsoft.VisualStudio.Services.Licensing.License) MsdnLicense.Enterprise;
        default:
          throw new InvalidEnumArgumentException(nameof (license), (int) license, typeof (MsdnLicenseType));
      }
    }
  }
}
