// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.CommonStructureNodeInfo
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.Azure.Boards.CssNodes
{
  [XmlType("NodeInfo")]
  [ClientType("NodeInfo")]
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  [ClassNotSealed]
  [Serializable]
  public class CommonStructureNodeInfo
  {
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string Uri { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string Name { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string Path { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string StructureType { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public CommonStructureProperty[] Properties { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string ParentUri { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string ProjectUri { get; set; }

    [XmlElement(DataType = "date")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public DateTime? StartDate { get; set; }

    [XmlElement(DataType = "date")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public DateTime? FinishDate { get; set; }

    public CommonStructureNodeInfo(
      string uri,
      string name,
      string structureType,
      CommonStructureProperty[] properties,
      string parentUri,
      string projectUri,
      string path,
      DateTime? startDate,
      DateTime? finishDate)
    {
      this.Uri = uri;
      this.Name = name;
      this.StructureType = structureType;
      this.Properties = (CommonStructureProperty[]) properties.Clone();
      this.Path = path;
      this.ParentUri = parentUri;
      this.ProjectUri = projectUri;
      this.StartDate = startDate;
      this.FinishDate = finishDate;
    }

    public CommonStructureNodeInfo()
    {
    }
  }
}
