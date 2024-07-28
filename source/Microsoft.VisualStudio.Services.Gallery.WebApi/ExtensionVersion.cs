// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionVersion
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public class ExtensionVersion
  {
    internal Guid ExtensionId { get; set; }

    public string Version { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string TargetPlatform { get; set; }

    public ExtensionVersionFlags Flags { get; set; }

    public DateTime LastUpdated { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string VersionDescription { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ValidationResultMessage { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<ExtensionFile> Files { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<KeyValuePair<string, string>> Properties { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<ExtensionBadge> Badges { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string AssetUri { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string FallbackAssetUri { get; set; }

    internal string CdnDirectory { get; set; }

    internal bool IsCdnEnabled { get; set; }

    public ExtensionVersion ShallowCopy() => (ExtensionVersion) this.MemberwiseClone();

    public string GetCdnDirectory() => this.CdnDirectory;
  }
}
