// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.WebApi.Types.MavenPomPerson
// Assembly: Microsoft.VisualStudio.Services.Maven.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 62CDE373-A3CE-478E-B824-A307191D9BE2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.WebApi.Types
{
  [DataContract]
  public class MavenPomPerson
  {
    [DataMember(EmitDefaultValue = false, Name = "id")]
    [XmlElement("id")]
    public string Id { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "name")]
    [XmlElement("name")]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "email")]
    [XmlElement("email")]
    public string Email { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "url")]
    [XmlElement("url")]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "organization")]
    [XmlElement("organization")]
    public string Organization { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "organizationUrl")]
    [XmlElement("organizationUrl")]
    public string OrganizationUrl { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "timezone")]
    [XmlElement("timezone")]
    public string Timezone { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "roles")]
    [XmlArray("roles")]
    [XmlArrayItem("role")]
    public List<string> Roles { get; set; }

    public override string ToString() => this.Name;
  }
}
