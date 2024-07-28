// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.IssuerParameters
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.KeyVault.Models
{
  public class IssuerParameters
  {
    public IssuerParameters()
    {
    }

    public IssuerParameters(string name = null, string certificateType = null, bool? certificateTransparency = null)
    {
      this.Name = name;
      this.CertificateType = certificateType;
      this.CertificateTransparency = certificateTransparency;
    }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "cty")]
    public string CertificateType { get; set; }

    [JsonProperty(PropertyName = "cert_transparency")]
    public bool? CertificateTransparency { get; set; }
  }
}
