// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ClientEncryptionKey
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal class ClientEncryptionKey : Resource
  {
    private KeyWrapMetadata keyWrapMetadata;

    [JsonProperty(PropertyName = "wrappedDataEncryptionKey")]
    internal string WrappedDataEncryptionKey
    {
      get => this.GetValue<string>("wrappedDataEncryptionKey");
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
