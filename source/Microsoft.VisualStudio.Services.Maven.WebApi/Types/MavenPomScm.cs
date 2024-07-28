// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.WebApi.Types.MavenPomScm
// Assembly: Microsoft.VisualStudio.Services.Maven.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 62CDE373-A3CE-478E-B824-A307191D9BE2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.WebApi.dll

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.WebApi.Types
{
  [DataContract]
  public class MavenPomScm
  {
    [DataMember(EmitDefaultValue = false, Name = "connection")]
    [XmlElement("connection")]
    public string Connection { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "developerConnection")]
    [XmlElement("developerConnection")]
    public string DeveloperConnection { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "tag")]
    [XmlElement("tag")]
    public string Tag { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "url")]
    [XmlElement("url")]
    public string Url { get; set; }
  }
}
