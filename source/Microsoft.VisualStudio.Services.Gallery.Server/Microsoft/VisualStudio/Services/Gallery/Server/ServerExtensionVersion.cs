// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ServerExtensionVersion
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ServerExtensionVersion
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
    public List<ServerExtensionFile> Files { get; set; }

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

    public ServerExtensionVersion ShallowCopy() => (ServerExtensionVersion) this.MemberwiseClone();

    public string GetCdnDirectory() => this.CdnDirectory;
  }
}
