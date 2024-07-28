// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ClientEncryptionIncludedPath
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal sealed class ClientEncryptionIncludedPath : JsonSerializable
  {
    [JsonProperty(PropertyName = "path")]
    public string Path
    {
      get => this.GetValue<string>("path");
      set => this.SetValue("path", (object) value);
    }

    [JsonProperty(PropertyName = "clientEncryptionKeyId")]
    public string ClientEncryptionKeyId
    {
      get => this.GetValue<string>("clientEncryptionKeyId");
      set => this.SetValue("clientEncryptionKeyId", (object) value);
    }

    [JsonProperty(PropertyName = "encryptionType")]
    public string EncryptionType
    {
      get => this.GetValue<string>("encryptionType");
      set => this.SetValue("encryptionType", (object) value);
    }

    [JsonProperty(PropertyName = "encryptionAlgorithm")]
    public string EncryptionAlgorithm
    {
      get => this.GetValue<string>("encryptionAlgorithm");
      set => this.SetValue("encryptionAlgorithm", (object) value);
    }
  }
}
