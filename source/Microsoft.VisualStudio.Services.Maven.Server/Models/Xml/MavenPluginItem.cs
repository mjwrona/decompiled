// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.Xml.MavenPluginItem
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models.Xml
{
  [DataContract]
  public class MavenPluginItem : 
    IEquatable<MavenPluginItem>,
    IComparable<MavenPluginItem>,
    IComparable
  {
    [DataMember(Name = "name")]
    [XmlElement("name")]
    public string Name { get; set; }

    [DataMember(Name = "prefix")]
    [XmlElement("prefix")]
    public string Prefix { get; set; }

    [DataMember(Name = "artifactId")]
    [XmlElement("artifactId")]
    public string ArtifactId { get; set; }

    public bool Equals(MavenPluginItem other)
    {
      ArgumentUtility.CheckForNull<MavenPluginItem>(other, nameof (other));
      return this.ToString().Equals(other.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    public int CompareTo(MavenPluginItem other)
    {
      ArgumentUtility.CheckForNull<MavenPluginItem>(other, nameof (other));
      return string.Compare(this.ToString(), other.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    public int CompareTo(object obj) => this.CompareTo(obj as MavenPluginItem);

    public override int GetHashCode() => this.ToString().GetHashCode();

    public override string ToString() => "[" + this.Prefix + "] " + this.ArtifactId + " (" + this.Name + ")";
  }
}
