// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.PackageManifestInternal
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [XmlRoot(ElementName = "PackageManifest", Namespace = "http://schemas.microsoft.com/developer/vsx-schema/2011")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PackageManifestInternal
  {
    private List<ManifestFile> m_assets = new List<ManifestFile>();
    private List<InstallationTarget> m_targets = new List<InstallationTarget>();

    internal string AssetCDNRoot { get; set; }

    public ManifestMetadataInternal Metadata { get; set; }

    [XmlArrayItem("Asset")]
    public List<ManifestFile> Assets => this.m_assets;

    [XmlArrayItem("InstallationTarget")]
    public List<InstallationTarget> Installation => this.m_targets;
  }
}
