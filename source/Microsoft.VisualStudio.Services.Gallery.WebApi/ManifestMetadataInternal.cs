// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.ManifestMetadataInternal
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ManifestMetadataInternal
  {
    private List<ExtensionProperty> m_properties = new List<ExtensionProperty>();
    private List<ExtensionBadge> m_badges = new List<ExtensionBadge>();
    private string m_flagsString;

    public ManifestIdentity Identity { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public string ReleaseNotes { get; set; }

    public string VsixId { get; set; }

    public string IgnoreWarnings { get; set; }

    public string Categories { get; set; }

    public string Tags { get; set; }

    [XmlElement("PublisherDetails")]
    public PublisherDetails Publisher { get; set; }

    [XmlElement("GalleryFlags")]
    public string FlagsString
    {
      get => this.m_flagsString;
      set => this.m_flagsString = value;
    }

    [XmlArrayItem("Property")]
    public List<ExtensionProperty> Properties => this.m_properties;

    [XmlArrayItem("Badge")]
    public List<ExtensionBadge> Badges => this.m_badges;
  }
}
