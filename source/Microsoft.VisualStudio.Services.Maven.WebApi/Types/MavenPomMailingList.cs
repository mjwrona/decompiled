// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.WebApi.Types.MavenPomMailingList
// Assembly: Microsoft.VisualStudio.Services.Maven.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 62CDE373-A3CE-478E-B824-A307191D9BE2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.WebApi.Types
{
  [DataContract]
  public class MavenPomMailingList
  {
    [DataMember(EmitDefaultValue = false, Name = "name")]
    [XmlElement("name")]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "post")]
    [XmlElement("post")]
    public string Post { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "subscribe")]
    [XmlElement("subscribe")]
    public string Subscribe { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "unsubscribe")]
    [XmlElement("unsubscribe")]
    public string Unsubscribe { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "archive")]
    [XmlElement("archive")]
    public string Archive { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "otherArchives")]
    [XmlArray("otherArchives")]
    [XmlArrayItem("otherArchive")]
    public List<string> OtherArchives { get; set; }

    public override string ToString() => this.Name;
  }
}
