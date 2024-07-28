// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Account.AccountLicenseInfo
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Account
{
  [DataContract]
  public class AccountLicenseInfo
  {
    private AccountLicenseInfo()
    {
    }

    public AccountLicenseInfo(string licenseName, int provisioned, int consumed)
      : this()
    {
      this.LicenseName = licenseName;
      this.InUseCount = provisioned;
      this.ConsumedCount = consumed;
    }

    [DataMember]
    public string LicenseName { get; set; }

    [DataMember]
    public int InUseCount { get; set; }

    [DataMember]
    public int ConsumedCount { get; set; }

    public AccountLicenseInfo Clone() => new AccountLicenseInfo()
    {
      ConsumedCount = this.ConsumedCount,
      LicenseName = this.LicenseName,
      InUseCount = this.InUseCount
    };

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AccountLicenseInfo: LicenseName {0}, ProvisionedCount {1}, ConsumedCount {2}", (object) this.LicenseName, (object) this.InUseCount, (object) this.ConsumedCount);
  }
}
