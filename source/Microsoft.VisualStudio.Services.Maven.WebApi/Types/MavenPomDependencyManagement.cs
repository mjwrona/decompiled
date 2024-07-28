// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.WebApi.Types.MavenPomDependencyManagement
// Assembly: Microsoft.VisualStudio.Services.Maven.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 62CDE373-A3CE-478E-B824-A307191D9BE2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.WebApi.Types
{
  [DataContract]
  public class MavenPomDependencyManagement
  {
    public MavenPomDependencyManagement() => this.Dependencies = new List<MavenPomDependency>();

    [DataMember(EmitDefaultValue = false, Name = "dependencies")]
    [XmlArray("dependencies")]
    [XmlArrayItem("dependency")]
    public List<MavenPomDependency> Dependencies { get; set; }

    public override string ToString() => string.Format("Count = {0}", (object) this.Dependencies.Count);
  }
}
