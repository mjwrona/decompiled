// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.GroupLicensingRule.LicenseRule
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Licensing;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.GroupLicensingRule
{
  [DataContract]
  public class LicenseRule : IEquatable<LicenseRule>
  {
    [DataMember]
    private License license { get; set; }

    [DataMember]
    public GroupLicensingRuleStatus Status { get; set; }

    public License License
    {
      get
      {
        License license = this.license;
        return (object) license != null ? license : License.None;
      }
      set => this.license = value;
    }

    [DataMember]
    public DateTimeOffset? LastExecuted { get; set; }

    [DataMember]
    public DateTimeOffset LastUpdated { get; set; }

    public LicenseRule()
    {
    }

    public LicenseRule(License license) => this.license = license;

    public override bool Equals(object obj) => this.Equals(obj as LicenseRule);

    public bool Equals(LicenseRule other) => other != (LicenseRule) null && this.License.Equals(other.license);

    public override int GetHashCode() => this.License.GetHashCode();

    public static bool Equals(LicenseRule left, LicenseRule right)
    {
      if ((object) left == null)
        return (object) right == null;
      return (object) right != null && left.Equals(right);
    }

    public static bool operator ==(LicenseRule left, LicenseRule right) => LicenseRule.Equals(left, right);

    public static bool operator !=(LicenseRule left, LicenseRule right) => !LicenseRule.Equals(left, right);
  }
}
