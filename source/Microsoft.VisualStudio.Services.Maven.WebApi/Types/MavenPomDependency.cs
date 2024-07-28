// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.WebApi.Types.MavenPomDependency
// Assembly: Microsoft.VisualStudio.Services.Maven.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 62CDE373-A3CE-478E-B824-A307191D9BE2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.WebApi.Types
{
  [DataContract]
  public class MavenPomDependency : 
    MavenPomGav,
    IEquatable<MavenPomDependency>,
    IComparable<MavenPomDependency>,
    IComparable
  {
    private string optionalRawXmlValue = false.ToString();

    [DataMember(EmitDefaultValue = false, Name = "type")]
    [XmlElement("type")]
    public string Type { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "scope")]
    [XmlElement("scope")]
    public string Scope { get; set; }

    [IgnoreDataMember]
    [XmlElement("optional")]
    public string OptionalXmlValue
    {
      get => this.optionalRawXmlValue;
      set => this.optionalRawXmlValue = string.IsNullOrEmpty(value) || bool.TryParse(value, out bool _) ? value : throw new InvalidDataException("<optional>" + value + "</optional>");
    }

    [DataMember(Name = "optional")]
    [XmlIgnore]
    public bool Optional
    {
      get
      {
        bool result;
        return bool.TryParse(this.OptionalXmlValue, out result) & result;
      }
      set => this.OptionalXmlValue = value.ToString();
    }

    public int CompareTo(object obj) => this.CompareTo(obj as MavenPomDependency);

    public int CompareTo(MavenPomDependency other)
    {
      ArgumentUtility.CheckForNull<MavenPomDependency>(other, nameof (other));
      return string.Compare(this.ToString(), other.ToString(), StringComparison.Ordinal);
    }

    public bool Equals(MavenPomDependency other)
    {
      ArgumentUtility.CheckForNull<MavenPomDependency>(other, nameof (other));
      return this.ToString().Equals(other.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj) => obj != null && obj is MavenPomDependency mavenPomDependency && this.ToString().Equals(mavenPomDependency.ToString());

    public override int GetHashCode() => this.ToString().GetHashCode();
  }
}
