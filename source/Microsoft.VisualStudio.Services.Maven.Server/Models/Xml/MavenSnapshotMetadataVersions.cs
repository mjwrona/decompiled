// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.Xml.MavenSnapshotMetadataVersions
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Models.Xml.Helpers;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models.Xml
{
  public class MavenSnapshotMetadataVersions
  {
    public MavenSnapshotMetadataVersions()
    {
    }

    public MavenSnapshotMetadataVersions(MavenSnapshotMetadataFile file)
      : this()
    {
      this.Classifier = file.Classifier;
      this.Extension = file.Extension;
      this.Value = file.Value;
      this.Updated = file.Timestamp.ToString("yyyyMMddHHmmss");
    }

    [XmlElement("classifier")]
    public string Classifier { get; set; }

    [XmlElement("extension")]
    public string Extension { get; set; }

    [XmlElement("value")]
    public string Value { get; set; }

    [XmlElement("updated")]
    public string Updated { get; set; }
  }
}
