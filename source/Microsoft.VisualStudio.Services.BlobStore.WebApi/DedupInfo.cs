// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DedupInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [CLSCompliant(false)]
  [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
  public sealed class DedupInfo
  {
    [JsonProperty(PropertyName = "id", Required = Required.Always)]
    public readonly string Id;
    [JsonProperty(PropertyName = "size", Required = Required.Always)]
    public readonly ulong Size;

    [JsonConstructor]
    public DedupInfo(string id, ulong size)
    {
      this.Id = id;
      this.Size = size;
    }
  }
}
