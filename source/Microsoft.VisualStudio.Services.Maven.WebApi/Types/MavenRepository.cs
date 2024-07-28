// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.WebApi.Types.MavenRepository
// Assembly: Microsoft.VisualStudio.Services.Maven.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 62CDE373-A3CE-478E-B824-A307191D9BE2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.WebApi.dll

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.WebApi.Types
{
  [DataContract]
  public class MavenRepository
  {
    [DataMember(EmitDefaultValue = false, Name = "uniqueVersion")]
    [XmlElement("uniqueVersion")]
    public bool UniqueVersion { get; set; } = true;
  }
}
