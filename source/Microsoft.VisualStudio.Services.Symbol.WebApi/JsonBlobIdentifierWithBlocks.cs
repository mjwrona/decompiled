// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.JsonBlobIdentifierWithBlocks
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Content.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  [JsonObject(MemberSerialization.OptIn)]
  public class JsonBlobIdentifierWithBlocks
  {
    [JsonProperty(PropertyName = "identifierValue")]
    [JsonConverter(typeof (ByteArrayAsBase64JsonConvertor))]
    private byte[] IdentifierValue { get; set; }

    [JsonProperty(PropertyName = "blockHashes")]
    private IList<JsonBlobBlockHash> JsonBlobBlockHashes { get; set; }

    public Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks BlobIdentifierWithBlocks
    {
      get => this.IdentifierValue == null ? (Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks) null : new Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks(new BlobIdentifier(this.IdentifierValue), this.JsonBlobBlockHashes == null ? (IEnumerable<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash>) new List<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash>() : this.JsonBlobBlockHashes.Select<JsonBlobBlockHash, Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash>((Func<JsonBlobBlockHash, Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash>) (x => x.BlobBlockHash)));
      set
      {
        if (value == (Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks) null)
          throw new ArgumentNullException(nameof (value));
        if (value.BlobId == (BlobIdentifier) null)
          throw new ArgumentNullException("BlobId");
        this.IdentifierValue = value.BlobId.Bytes;
        this.JsonBlobBlockHashes = value.BlockHashes == null ? (IList<JsonBlobBlockHash>) new List<JsonBlobBlockHash>() : (IList<JsonBlobBlockHash>) value.BlockHashes.Select<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash, JsonBlobBlockHash>((Func<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash, JsonBlobBlockHash>) (x => new JsonBlobBlockHash()
        {
          BlobBlockHash = x
        })).ToList<JsonBlobBlockHash>();
      }
    }
  }
}
