// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Manifest
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [CLSCompliant(false)]
  [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
  public sealed class Manifest
  {
    private const string versionV1 = "1.0.0";
    private const string versionV2 = "1.1.0";
    private static readonly ISet<string> validVersions = Manifest.CreateValidVersionsSet();
    [JsonProperty(PropertyName = "manifestFormat", Required = Required.Always)]
    public readonly string ManifestFormat;
    [JsonProperty(PropertyName = "items", Required = Required.Always)]
    public readonly IList<ManifestItem> Items;
    [JsonProperty(PropertyName = "manifestReferences", Required = Required.Default)]
    public readonly IList<ManifestReference> ManifestReferences;

    private static string LatestVersion => "1.1.0";

    public Manifest(IList<ManifestItem> items)
      : this(Manifest.LatestVersion, items)
    {
    }

    [JsonConstructor]
    public Manifest(
      string manifestFormat,
      IList<ManifestItem> items,
      IList<ManifestReference> manifestReferences = null)
    {
      this.ManifestFormat = Manifest.validVersions.Contains(manifestFormat) ? manifestFormat : throw new ArgumentException("The manifest version is not valid.");
      this.Items = items;
      this.ManifestReferences = manifestReferences ?? (IList<ManifestReference>) new List<ManifestReference>();
    }

    private static ISet<string> CreateValidVersionsSet() => (ISet<string>) new HashSet<string>()
    {
      "1.0.0",
      "1.1.0"
    };
  }
}
