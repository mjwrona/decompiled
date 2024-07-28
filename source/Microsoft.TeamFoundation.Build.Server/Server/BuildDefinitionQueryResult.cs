// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDefinitionQueryResult
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [CallOnDeserialization("AfterDeserialize")]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  [ClassVisibility(ClientVisibility.Internal)]
  public sealed class BuildDefinitionQueryResult
  {
    private List<BuildAgent> m_agents = new List<BuildAgent>();
    private List<BuildController> m_controllers = new List<BuildController>();
    private List<BuildServiceHost> m_serviceHosts = new List<BuildServiceHost>();
    private List<BuildDefinition> m_buildDefinitions = new List<BuildDefinition>();

    [XmlElement(Order = 1, Type = typeof (List<BuildDefinition>))]
    [ClientProperty(ClientVisibility.Private)]
    public List<BuildDefinition> Definitions => this.m_buildDefinitions;

    [XmlElement(Order = 2, Type = typeof (List<BuildAgent>))]
    [ClientProperty(ClientVisibility.Private)]
    public List<BuildAgent> Agents => this.m_agents;

    [XmlElement(Order = 3, Type = typeof (List<BuildController>))]
    [ClientProperty(ClientVisibility.Private)]
    public List<BuildController> Controllers => this.m_controllers;

    [XmlElement(Order = 4, Type = typeof (List<BuildServiceHost>))]
    [ClientProperty(ClientVisibility.Private)]
    public List<BuildServiceHost> ServiceHosts => this.m_serviceHosts;
  }
}
