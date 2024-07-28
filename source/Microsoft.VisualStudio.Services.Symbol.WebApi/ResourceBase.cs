// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.ResourceBase
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  public class ResourceBase : JsonSerializeableObject
  {
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Guid? CreatedBy { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? CreatedDate { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Id { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string StorageETag { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Uri Url { get; set; }
  }
}
