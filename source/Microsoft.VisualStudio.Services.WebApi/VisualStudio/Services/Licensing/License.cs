// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.License
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [JsonConverter(typeof (LicenseJsonConverter))]
  [TypeConverter(typeof (LicenseTypeConverter))]
  [JsonObject]
  [DebuggerDisplay("{ToString(), nq}")]
  public abstract class License : IEquatable<License>
  {
    public static readonly License None = (License) new License.NoLicense();
    public static readonly License Auto = (License) new License.AutoLicense();
    private Type licenseEnumType;
    private int license;

    internal License(LicensingSource source, Type licenseEnumType, int license)
    {
      this.licenseEnumType = licenseEnumType;
      this.license = license;
      this.Source = source;
    }

    public LicensingSource Source { get; private set; }

    internal int GetLicenseAsInt32() => this.license;

    public override int GetHashCode() => this.Source.GetHashCode() ^ this.license.GetHashCode();

    public override bool Equals(object obj) => this.Equals(obj as License);

    public bool Equals(License obj) => obj != (License) null && this.Source == obj.Source && this.license == obj.license;

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.Source.ToString());
      stringBuilder.Append('-');
      stringBuilder.Append(Enum.GetName(this.licenseEnumType, (object) this.license));
      return stringBuilder.ToString();
    }

    public static License Parse(string text) => License.Parse(text, false);

    public static License Parse(string text, bool ignoreCase)
    {
      License license;
      if (!License.TryParse(text, ignoreCase, out license))
        throw new FormatException();
      return license;
    }

    public static bool TryParse(string text, out License license) => License.TryParse(text, false, out license);

    public static bool TryParse(string text, bool ignoreCase, out License license)
    {
      license = License.None;
      if (string.IsNullOrWhiteSpace(text))
        return false;
      string[] strArray = text.Split('-');
      LicensingSource result1;
      if (!Enum.TryParse<LicensingSource>(strArray[0], ignoreCase, out result1))
        return false;
      if (strArray.Length == 1 && result1 == LicensingSource.None)
        return true;
      if (strArray.Length == 1 && result1 == LicensingSource.Auto)
      {
        license = License.Auto;
        return true;
      }
      if (strArray.Length > 2)
        return false;
      switch (result1)
      {
        case LicensingSource.Account:
          AccountLicenseType result2;
          if (Enum.TryParse<AccountLicenseType>(strArray[1], ignoreCase, out result2) && result2 != AccountLicenseType.None)
          {
            license = AccountLicense.GetLicense(result2);
            return true;
          }
          break;
        case LicensingSource.Msdn:
          MsdnLicenseType result3;
          if (Enum.TryParse<MsdnLicenseType>(strArray[1], ignoreCase, out result3) && result3 != MsdnLicenseType.None)
          {
            license = MsdnLicense.GetLicense(result3);
            return true;
          }
          break;
        case LicensingSource.Auto:
          LicensingSource result4;
          if (Enum.TryParse<LicensingSource>(strArray[1], ignoreCase, out result4))
          {
            license = (License) License.AutoLicense.GetLicense(result4);
            return true;
          }
          break;
      }
      return false;
    }

    public static bool Equals(License left, License right)
    {
      if ((object) left == null)
        return (object) right == null;
      return (object) right != null && left.Equals(right);
    }

    public static bool IsNullOrNone(License license) => license == (License) null || license.Source == LicensingSource.None;

    internal static License GetLicense(LicensingSource source, int license)
    {
      switch (source)
      {
        case LicensingSource.None:
          return License.None;
        case LicensingSource.Account:
          return AccountLicense.GetLicense((AccountLicenseType) license);
        case LicensingSource.Msdn:
          return MsdnLicense.GetLicense((MsdnLicenseType) license);
        case LicensingSource.Profile:
          throw new NotSupportedException();
        case LicensingSource.Auto:
          return License.Auto;
        default:
          throw new InvalidEnumArgumentException(nameof (source), (int) source, typeof (LicensingSource));
      }
    }

    public static bool operator ==(License left, License right) => License.Equals(left, right);

    public static bool operator !=(License left, License right) => !License.Equals(left, right);

    public static bool operator >(License left, License right) => LicenseComparer.Instance.Compare(left, right) > 0;

    public static bool operator >=(License left, License right) => LicenseComparer.Instance.Compare(left, right) >= 0;

    public static bool operator <(License left, License right) => LicenseComparer.Instance.Compare(left, right) < 0;

    public static bool operator <=(License left, License right) => LicenseComparer.Instance.Compare(left, right) <= 0;

    private sealed class NoLicense : License
    {
      internal NoLicense()
        : base(LicensingSource.None, (Type) null, 0)
      {
      }

      public override string ToString() => "None";
    }

    internal sealed class AutoLicense : License
    {
      internal static readonly License Msdn = (License) License.AutoLicense.GetLicense(LicensingSource.Msdn);

      internal AutoLicense()
        : base(LicensingSource.Auto, (Type) null, 0)
      {
      }

      private AutoLicense(LicensingSource licenseSource)
        : base(LicensingSource.Auto, typeof (LicensingSource), (int) licenseSource)
      {
      }

      internal static License.AutoLicense GetLicense(LicensingSource source) => new License.AutoLicense(source);

      public override string ToString() => this.GetLicenseAsInt32() != 0 ? base.ToString() : "Auto";
    }
  }
}
