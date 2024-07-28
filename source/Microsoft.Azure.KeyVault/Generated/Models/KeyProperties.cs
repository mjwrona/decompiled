// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.KeyProperties
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.KeyVault.Models
{
  public class KeyProperties
  {
    public KeyProperties()
    {
    }

    public KeyProperties(
      bool? exportable = null,
      string keyType = null,
      int? keySize = null,
      bool? reuseKey = null,
      string curve = null)
    {
      this.Exportable = exportable;
      this.KeyType = keyType;
      this.KeySize = keySize;
      this.ReuseKey = reuseKey;
      this.Curve = curve;
    }

    [JsonProperty(PropertyName = "exportable")]
    public bool? Exportable { get; set; }

    [JsonProperty(PropertyName = "kty")]
    public string KeyType { get; set; }

    [JsonProperty(PropertyName = "key_size")]
    public int? KeySize { get; set; }

    [JsonProperty(PropertyName = "reuse_key")]
    public bool? ReuseKey { get; set; }

    [JsonProperty(PropertyName = "crv")]
    public string Curve { get; set; }
  }
}
