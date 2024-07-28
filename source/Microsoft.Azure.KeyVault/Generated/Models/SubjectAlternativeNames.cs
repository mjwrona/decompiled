// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.SubjectAlternativeNames
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class SubjectAlternativeNames
  {
    public SubjectAlternativeNames()
    {
    }

    public SubjectAlternativeNames(
      IList<string> emails = null,
      IList<string> dnsNames = null,
      IList<string> upns = null)
    {
      this.Emails = emails;
      this.DnsNames = dnsNames;
      this.Upns = upns;
    }

    [JsonProperty(PropertyName = "emails")]
    public IList<string> Emails { get; set; }

    [JsonProperty(PropertyName = "dns_names")]
    public IList<string> DnsNames { get; set; }

    [JsonProperty(PropertyName = "upns")]
    public IList<string> Upns { get; set; }
  }
}
