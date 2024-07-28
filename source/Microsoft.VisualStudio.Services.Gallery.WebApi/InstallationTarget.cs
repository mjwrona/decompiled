// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.InstallationTarget
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public class InstallationTarget
  {
    [XmlAttribute("Id")]
    public string Target { get; set; }

    [XmlAttribute("Version")]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string TargetVersion { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public Version MinVersion { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public Version MaxVersion { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public bool MinInclusive { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public bool MaxInclusive { get; set; }

    [XmlIgnore]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ProductArchitecture { get; set; }

    [XmlIgnore]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ExtensionVersion { get; set; }

    [XmlIgnore]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string TargetPlatform { get; set; }

    public bool IsApplicableForVersion(Version version, string target = null)
    {
      bool flag = false;
      if (version != (Version) null)
      {
        if (target != null && !target.Equals(this.Target, StringComparison.OrdinalIgnoreCase))
          return false;
        if (((this.MinInclusive ? (this.MinVersion.CompareTo(version) <= 0 ? 1 : 0) : (this.MinVersion.CompareTo(version) < 0 ? 1 : 0)) & (this.MaxInclusive ? (this.MaxVersion.CompareTo(version) >= 0 ? 1 : 0) : (this.MaxVersion.CompareTo(version) > 0 ? 1 : 0))) != 0)
          flag = true;
      }
      return flag;
    }

    public InstallationTarget ShallowCopy() => (InstallationTarget) this.MemberwiseClone();
  }
}
