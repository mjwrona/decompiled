// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.CommonStructureProperty
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.Azure.Boards.CssNodes
{
  [XmlType("Property")]
  [ClientType("Property")]
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  [ClassNotSealed]
  [Serializable]
  public class CommonStructureProperty
  {
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string Name { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string Value { get; set; }

    public CommonStructureProperty()
    {
    }

    public CommonStructureProperty(string name, string propValue)
    {
      this.Name = name;
      this.Value = propValue;
    }
  }
}
