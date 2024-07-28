// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.KeyVerifyResult
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.KeyVault.Models
{
  public class KeyVerifyResult
  {
    public KeyVerifyResult()
    {
    }

    public KeyVerifyResult(bool? value = null) => this.Value = value;

    [JsonProperty(PropertyName = "value")]
    public bool? Value { get; private set; }
  }
}
