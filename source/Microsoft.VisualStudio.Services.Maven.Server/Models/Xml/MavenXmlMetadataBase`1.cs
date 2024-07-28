// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.Xml.MavenXmlMetadataBase`1
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models.Xml
{
  public abstract class MavenXmlMetadataBase<T> : MavenXml
  {
    [DataMember(EmitDefaultValue = false, Name = "modelVersion")]
    [XmlElement("modelVersion")]
    public string ModelVersion { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "groupId")]
    [XmlElement("groupId")]
    public string GroupId { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "artifactId")]
    [XmlElement("artifactId")]
    public string ArtifactId { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "version")]
    [XmlElement("version")]
    public string Version { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "versioning")]
    [XmlElement("versioning")]
    public T Versioning { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "plugins")]
    [XmlElement("plugins")]
    public MavenPluginList Plugins { get; set; }

    public override string ToString()
    {
      IList<string> source = (IList<string>) new List<string>();
      if (!string.IsNullOrWhiteSpace(this.GroupId))
        source.Add(this.GroupId);
      if (!string.IsNullOrWhiteSpace(this.ArtifactId))
        source.Add(this.ArtifactId);
      if (!string.IsNullOrWhiteSpace(this.Version))
        source.Add(this.Version);
      return source.Any<string>() ? string.Join(":", source.ToArray<string>()) : base.ToString();
    }
  }
}
