// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.Types.V2.NugetV2PackageFeedResponse
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi.Types.V2
{
  [XmlRoot("entry", Namespace = "http://www.w3.org/2005/Atom")]
  public class NugetV2PackageFeedResponse
  {
    [DataMember]
    [XmlElement("id")]
    public string Id { get; set; }

    [XmlElement("title")]
    public string Title { get; set; }

    [XmlElement("updated")]
    public string Updated { get; set; }

    [XmlElement("content")]
    public Content Content { get; set; }

    [XmlElement(ElementName = "properties", Namespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata")]
    public Properties Properties { get; set; }
  }
}
