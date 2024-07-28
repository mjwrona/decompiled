// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.ManifestIdentity
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public class ManifestIdentity
  {
    [XmlAttribute("Publisher")]
    public string PublisherName { get; set; }

    [XmlAttribute("Id")]
    public string ExtensionName { get; set; }

    [XmlAttribute("Version")]
    public string Version { get; set; }

    [XmlAttribute("TargetPlatform")]
    public string TargetPlatform { get; set; }
  }
}
