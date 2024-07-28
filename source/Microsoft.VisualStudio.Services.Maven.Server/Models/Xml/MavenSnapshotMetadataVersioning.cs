// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.Xml.MavenSnapshotMetadataVersioning
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models.Xml
{
  public class MavenSnapshotMetadataVersioning
  {
    public MavenSnapshotMetadataVersioning()
    {
      this.Latest = new MavenSnapshotMetadataLatest();
      this.Versions = new List<MavenSnapshotMetadataVersions>();
    }

    [XmlElement("snapshot")]
    public MavenSnapshotMetadataLatest Latest { get; set; }

    [XmlArray("snapshotVersions")]
    [XmlArrayItem("snapshotVersion")]
    public List<MavenSnapshotMetadataVersions> Versions { get; set; }

    [XmlElement("lastUpdated")]
    public string LastUpdated { get; set; }
  }
}
