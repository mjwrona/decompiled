// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionFile
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public class ExtensionFile
  {
    internal Guid ReferenceId { get; set; }

    [XmlAttribute("Type")]
    public string AssetType { get; set; }

    [XmlAttribute("Lang")]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Language { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Source { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public string Version { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public string TargetPlatform { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public string ContentType { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public int FileId { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public string ShortDescription { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public bool IsDefault { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public bool IsPublic { get; set; }

    internal ExtensionFile ShallowCopy() => (ExtensionFile) this.MemberwiseClone();
  }
}
