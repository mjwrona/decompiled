// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.JsonBlobIdentifier
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Content.Common;
using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  [JsonObject(MemberSerialization.OptIn)]
  public class JsonBlobIdentifier
  {
    [JsonProperty(PropertyName = "identifierValue")]
    [JsonConverter(typeof (ByteArrayAsBase64JsonConvertor))]
    private byte[] IdentifierValue { get; set; }

    public BlobIdentifier BlobIdentifier
    {
      get => this.IdentifierValue == null ? (BlobIdentifier) null : new BlobIdentifier(this.IdentifierValue);
      set => this.IdentifierValue = value?.Bytes;
    }
  }
}
