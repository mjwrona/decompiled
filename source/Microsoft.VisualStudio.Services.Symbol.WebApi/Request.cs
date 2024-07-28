// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.Request
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  public class Request : ResourceBase
  {
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Description { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? ExpirationDate { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }

    [JsonConverter(typeof (DomainIdJsonConverter))]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public IDomainId DomainId { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public RequestStatus Status { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public long Size { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool IsChunked { get; set; }
  }
}
