// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ClientEncryptionKey
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal class ClientEncryptionKey : Resource
  {
    private KeyWrapMetadata keyWrapMetadata;

    [JsonProperty(PropertyName = "encryptionAlgorithm")]
    internal string EncryptionAlgorithm
    {
      get => this.GetValue<string>("encryptionAlgorithm");
      set => this.SetValue("encryptionAlgorithm", (object) value);
    }

    [JsonProperty(PropertyName = "wrappedDataEncryptionKey")]
    internal byte[] WrappedDataEncryptionKey
    {
      get => this.GetValue<byte[]>("wrappedDataEncryptionKey");
      set => this.SetValue("wrappedDataEncryptionKey", (object) value);
    }

    [JsonProperty(PropertyName = "keyWrapMetadata")]
    internal KeyWrapMetadata KeyWrapMetadata
    {
      get
      {
        if (this.keyWrapMetadata == null)
          this.keyWrapMetadata = this.GetObject<KeyWrapMetadata>("keyWrapMetadata") ?? new KeyWrapMetadata();
        return this.keyWrapMetadata;
      }
      set
      {
        this.keyWrapMetadata = value;
        this.SetObject<KeyWrapMetadata>("keyWrapMetadata", value);
      }
    }

    internal override void OnSave()
    {
      if (this.keyWrapMetadata == null)
        return;
      this.keyWrapMetadata.OnSave();
      this.SetObject<KeyWrapMetadata>("keyWrapMetadata", this.keyWrapMetadata);
    }
  }
}
