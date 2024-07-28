// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.ManifestMetadata
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public class ManifestMetadata
  {
    private List<ExtensionProperty> m_properties = new List<ExtensionProperty>();
    private List<ExtensionBadge> m_badges = new List<ExtensionBadge>();

    public ManifestMetadata()
    {
    }

    public ManifestMetadata(ManifestMetadataInternal metadataInternal)
    {
      this.Identity = metadataInternal.Identity;
      this.DisplayName = metadataInternal.DisplayName;
      this.Description = metadataInternal.Description;
      this.ReleaseNotes = metadataInternal.ReleaseNotes;
      this.Categories = metadataInternal.Categories;
      this.Tags = metadataInternal.Tags;
      this.Publisher = metadataInternal.Publisher;
      this.Flags = this.ParseFlags(metadataInternal.FlagsString);
      this.IgnoreWarnings = metadataInternal.IgnoreWarnings;
      this.VsixId = metadataInternal.VsixId;
      this.m_properties = metadataInternal.Properties;
      this.m_badges = metadataInternal.Badges;
    }

    public ManifestIdentity Identity { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public string ReleaseNotes { get; set; }

    public string Categories { get; set; }

    public string Tags { get; set; }

    public string IgnoreWarnings { get; set; }

    public string VsixId { get; set; }

    [XmlElement("PublisherDetails")]
    public PublisherDetails Publisher { get; set; }

    [XmlElement("GalleryFlags")]
    public PublishedExtensionFlags Flags { get; set; }

    [XmlArrayItem("Property")]
    public List<ExtensionProperty> Properties => this.m_properties;

    [XmlArrayItem("Badge")]
    public List<ExtensionBadge> Badges => this.m_badges;

    private PublishedExtensionFlags ParseFlags(string flagsString)
    {
      PublishedExtensionFlags flags = PublishedExtensionFlags.None;
      if (!string.IsNullOrEmpty(flagsString))
      {
        string str = flagsString;
        char[] chArray = new char[1]{ ' ' };
        foreach (string b in str.Split(chArray))
        {
          foreach (PublishedExtensionFlags publishedExtensionFlags in Enum.GetValues(typeof (PublishedExtensionFlags)))
          {
            if (string.Equals(publishedExtensionFlags.ToString(), b, StringComparison.OrdinalIgnoreCase))
              flags |= publishedExtensionFlags;
          }
        }
      }
      return flags;
    }
  }
}
