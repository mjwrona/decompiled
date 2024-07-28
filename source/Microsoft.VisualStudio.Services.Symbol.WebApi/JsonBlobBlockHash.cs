// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.JsonBlobBlockHash
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  [JsonObject(MemberSerialization.OptIn)]
  public class JsonBlobBlockHash
  {
    [JsonProperty(PropertyName = "HashBytes")]
    [JsonConverter(typeof (ByteArrayAsBase64JsonConvertor))]
    private byte[] HashBytes { get; set; }

    public BlobBlockHash BlobBlockHash
    {
      get => this.HashBytes == null ? (BlobBlockHash) null : new BlobBlockHash(this.HashBytes);
      set => this.HashBytes = value?.HashBytes;
    }
  }
}
