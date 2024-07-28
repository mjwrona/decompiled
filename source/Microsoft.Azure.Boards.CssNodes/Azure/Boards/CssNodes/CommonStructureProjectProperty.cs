// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.CommonStructureProjectProperty
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Azure.Boards.CssNodes
{
  [XmlType("ProjectProperty")]
  [ClientType("ProjectProperty")]
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  [ClassNotSealed]
  [Serializable]
  public class CommonStructureProjectProperty
  {
    public CommonStructureProjectProperty()
    {
    }

    public CommonStructureProjectProperty(string name, string value)
    {
      this.Name = name;
      this.Value = value;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public string Name { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public string Value { get; set; }

    public static CommonStructureProjectProperty[] ConvertProjectProperties(
      IList<ProjectProperty> properties)
    {
      if (properties == null)
        return (CommonStructureProjectProperty[]) null;
      List<CommonStructureProjectProperty> structureProjectPropertyList = new List<CommonStructureProjectProperty>(properties.Count);
      foreach (ProjectProperty property in (IEnumerable<ProjectProperty>) properties)
        structureProjectPropertyList.Add(new CommonStructureProjectProperty(property.Name, (string) property.Value));
      return structureProjectPropertyList.ToArray();
    }

    public static List<ProjectProperty> ConvertToProjectProperties(
      CommonStructureProjectProperty[] properties)
    {
      if (properties == null)
        return (List<ProjectProperty>) null;
      List<ProjectProperty> projectProperties = new List<ProjectProperty>(properties.Length);
      foreach (CommonStructureProjectProperty property in properties)
      {
        if (property == null)
          projectProperties.Add((ProjectProperty) null);
        else
          projectProperties.Add(new ProjectProperty(property.Name, (object) property.Value));
      }
      return projectProperties;
    }
  }
}
