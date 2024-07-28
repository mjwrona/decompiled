// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.WebApi.Types.MavenPomCiNotifier
// Assembly: Microsoft.VisualStudio.Services.Maven.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 62CDE373-A3CE-478E-B824-A307191D9BE2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.WebApi.Types
{
  [DataContract]
  public class MavenPomCiNotifier
  {
    [DataMember(EmitDefaultValue = false, Name = "type")]
    [XmlElement("type")]
    public string Type { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "sendOnError")]
    [XmlElement("sendOnError")]
    public string SendOnError { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "sendOnFailure")]
    [XmlElement("sendOnFailure")]
    public string SendOnFailure { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "sendOnSuccess")]
    [XmlElement("sendOnSuccess")]
    public string SendOnSuccess { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "sendOnWarning")]
    [XmlElement("sendOnWarning")]
    public string SendOnWarning { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "configuration")]
    [XmlArray("configuration")]
    [XmlArrayItem("address")]
    public List<string> Configuration { get; set; }
  }
}
