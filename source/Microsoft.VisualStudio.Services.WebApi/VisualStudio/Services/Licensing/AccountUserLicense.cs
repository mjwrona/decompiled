// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.AccountUserLicense
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [DataContract]
  public class AccountUserLicense
  {
    public AccountUserLicense(LicensingSource source, int license)
    {
      this.Source = source;
      this.License = license;
    }

    [DataMember]
    public virtual int License { get; set; }

    [DataMember]
    public virtual LicensingSource Source { get; set; }

    public override string ToString() => Microsoft.VisualStudio.Services.Licensing.License.GetLicense(this.Source, this.License).ToString();
  }
}
