// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.ManifestItem
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [CLSCompliant(false)]
  [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
  public sealed class ManifestItem
  {
    [JsonProperty(PropertyName = "path", Required = Required.Always)]
    public readonly string Path;
    [JsonProperty(PropertyName = "blob", Required = Required.Default)]
    public readonly DedupInfo Blob;
    [JsonProperty(PropertyName = "type", Required = Required.Default)]
    public readonly ManifestItemType Type;

    public ManifestItem(string path, DedupInfo blob)
      : this(path, blob, ManifestItemType.File)
    {
    }

    [JsonConstructor]
    public ManifestItem(string path, DedupInfo blob, ManifestItemType type)
    {
      this.Path = path;
      this.Blob = blob;
      this.Type = type;
    }

    public bool ShouldSerializeType() => this.Type != 0;

    public bool ShouldSerializeBlob() => this.Type == ManifestItemType.File;
  }
}
