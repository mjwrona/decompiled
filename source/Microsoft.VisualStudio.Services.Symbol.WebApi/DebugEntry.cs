// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.DebugEntry
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Symbol.Common;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  [JsonObject(MemberSerialization.OptIn)]
  public class DebugEntry : ResourceBase
  {
    [JsonProperty(PropertyName = "blobDetails", NullValueHandling = NullValueHandling.Ignore)]
    private JsonBlobIdentifierWithBlocks JsonBlobDetails { get; set; }

    public Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks BlobDetails
    {
      get => this.JsonBlobDetails?.BlobIdentifierWithBlocks;
      set
      {
        if (value == (Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks) null)
          this.JsonBlobDetails = (JsonBlobIdentifierWithBlocks) null;
        else
          this.JsonBlobDetails = new JsonBlobIdentifierWithBlocks()
          {
            BlobIdentifierWithBlocks = value
          };
      }
    }

    [JsonProperty(PropertyName = "blobIdentifier", NullValueHandling = NullValueHandling.Ignore)]
    private JsonBlobIdentifier JsonBlobIdentifier { get; set; }

    public BlobIdentifier BlobIdentifier
    {
      get => this.JsonBlobIdentifier?.BlobIdentifier;
      set
      {
        if (value == (BlobIdentifier) null)
          this.JsonBlobIdentifier = (JsonBlobIdentifier) null;
        else
          this.JsonBlobIdentifier = new JsonBlobIdentifier()
          {
            BlobIdentifier = value
          };
      }
    }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Uri BlobUri { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ClientKey { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DebugInformationLevel InformationLevel { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string RequestId { get; set; }

    [JsonConverter(typeof (DomainIdJsonConverter))]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public IDomainId DomainId { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DebugEntryStatus Status { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public long Size { get; set; }
  }
}
