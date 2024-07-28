// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.WebApi.Types.MavenPomMetadata
// Assembly: Microsoft.VisualStudio.Services.Maven.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 62CDE373-A3CE-478E-B824-A307191D9BE2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.WebApi.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.WebApi.Types
{
  [DataContract]
  [XmlRoot("project")]
  public class MavenPomMetadata : MavenPomGav, ICloneable
  {
    [IgnoreDataMember]
    [XmlIgnore]
    public override string GroupId
    {
      get
      {
        string groupId = base.GroupId;
        if (groupId != null)
          return groupId;
        return this.Parent?.GroupId;
      }
      set => base.GroupId = value;
    }

    [DataMember(EmitDefaultValue = false, Name = "modelVersion")]
    [XmlElement("modelVersion")]
    public string ModelVersion { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "description")]
    [XmlElement("description")]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "name")]
    [XmlElement("name")]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "packaging")]
    [XmlElement("packaging")]
    public string Packaging { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "url")]
    [XmlElement("url")]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "inceptionYear")]
    [XmlElement("inceptionYear")]
    public string InceptionYear { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "parent")]
    [XmlElement("parent")]
    public MavenPomParent Parent { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "dependencies")]
    [XmlArray("dependencies")]
    [XmlArrayItem("dependency")]
    public List<MavenPomDependency> Dependencies { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "build")]
    [XmlElement("build")]
    public MavenPomBuild Build { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "dependencyManagement")]
    [XmlElement("dependencyManagement")]
    public MavenPomDependencyManagement DependencyManagement { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "organization")]
    [XmlElement("organization")]
    public MavenPomOrganization Organization { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "scm")]
    [XmlElement("scm")]
    public MavenPomScm Scm { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "ciManagement")]
    [XmlElement("ciManagement")]
    public MavenPomCi CiManagement { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "issueManagement")]
    [XmlElement("issueManagement")]
    public MavenPomIssueManagement IssueManagement { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "licenses")]
    [XmlArray("licenses")]
    [XmlArrayItem("license")]
    public List<MavenPomLicense> Licenses { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "modules")]
    [XmlArray("modules")]
    [XmlArrayItem("module")]
    public List<string> Modules { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "contributors")]
    [XmlArray("contributors")]
    [XmlArrayItem("contributor")]
    public List<MavenPomPerson> Contributors { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "developers")]
    [XmlArray("developers")]
    [XmlArrayItem("developer")]
    public List<MavenPomPerson> Developers { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "mailingLists")]
    [XmlArray("mailingLists")]
    [XmlArrayItem("mailingList")]
    public List<MavenPomMailingList> MailingList { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "distributionManagement")]
    [XmlElement("distributionManagement")]
    public MavenDistributionManagement DistributionManagement { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "properties")]
    [XmlIgnore]
    public Dictionary<string, string> Properties { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "prerequisites")]
    [XmlIgnore]
    public Dictionary<string, string> Prerequisites { get; set; }

    public object Clone()
    {
      DataContractSerializer contractSerializer = new DataContractSerializer(this.GetType());
      using (MemoryStream memoryStream = new MemoryStream())
      {
        contractSerializer.WriteObject((Stream) memoryStream, (object) this);
        memoryStream.Position = 0L;
        return contractSerializer.ReadObject((Stream) memoryStream);
      }
    }
  }
}
