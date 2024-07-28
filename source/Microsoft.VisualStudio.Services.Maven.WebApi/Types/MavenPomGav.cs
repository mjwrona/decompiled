// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.WebApi.Types.MavenPomGav
// Assembly: Microsoft.VisualStudio.Services.Maven.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 62CDE373-A3CE-478E-B824-A307191D9BE2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.WebApi.Types
{
  [DataContract]
  public class MavenPomGav
  {
    [IgnoreDataMember]
    [XmlIgnore]
    public virtual string GroupId
    {
      get => !string.IsNullOrWhiteSpace(this.SerializedGroupId) ? this.SerializedGroupId : (string) null;
      set => this.SerializedGroupId = value;
    }

    [DataMember(EmitDefaultValue = false, Name = "groupId")]
    [XmlElement("groupId")]
    public string SerializedGroupId { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "artifactId")]
    [XmlElement("artifactId")]
    public string ArtifactId { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "version")]
    [XmlElement("version")]
    public string Version { get; set; }

    public override string ToString() => string.Join(":", new List<string>()
    {
      this.GroupId,
      this.ArtifactId,
      this.Version
    }.Where<string>((Func<string, bool>) (x => !string.IsNullOrWhiteSpace(x))));
  }
}
