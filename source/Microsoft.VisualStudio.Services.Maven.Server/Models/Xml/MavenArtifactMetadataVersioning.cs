// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.Xml.MavenArtifactMetadataVersioning
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models.Xml
{
  [DataContract]
  public class MavenArtifactMetadataVersioning
  {
    public MavenArtifactMetadataVersioning() => this.Versions = new List<string>();

    [DataMember(EmitDefaultValue = false, Name = "latest")]
    [XmlElement("latest")]
    public string Latest { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "release")]
    [XmlElement("release")]
    public string Release { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "versions")]
    [XmlArray("versions")]
    [XmlArrayItem("version")]
    public List<string> Versions { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "lastUpdated")]
    [XmlElement("lastUpdated")]
    public string LastUpdated { get; set; }

    public void SetLastUpdated(DateTime dateTime) => this.LastUpdated = dateTime.ToString("yyyyMMddHHmmss");

    public override string ToString()
    {
      List<string> stringList = new List<string>();
      if (!string.IsNullOrWhiteSpace(this.Latest))
        stringList.Add("Latest:'" + this.Latest + "'");
      if (!string.IsNullOrWhiteSpace(this.Latest))
        stringList.Add("Release:'" + this.Release + "'");
      if (this.Versions != null)
        stringList.Add(string.Format("Versions:'{0}'", (object) this.Versions.Count));
      if (!string.IsNullOrWhiteSpace(this.LastUpdated))
        stringList.Add("LastUpdated:'" + this.LastUpdated + "'");
      return stringList.Any<string>() ? string.Join("; ", (IEnumerable<string>) stringList) : base.ToString();
    }
  }
}
