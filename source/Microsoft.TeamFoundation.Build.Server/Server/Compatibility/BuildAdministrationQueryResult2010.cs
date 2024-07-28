// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildAdministrationQueryResult2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [XmlType("BuildAdministrationQueryResult")]
  public abstract class BuildAdministrationQueryResult2010 : QueryResult2010
  {
    private List<BuildAgent2010> m_agents = new List<BuildAgent2010>();
    private List<BuildController2010> m_controllers = new List<BuildController2010>();
    private List<BuildServiceHost2010> m_serviceHosts = new List<BuildServiceHost2010>();

    [ClientProperty(ClientVisibility.Private)]
    public List<BuildAgent2010> Agents => this.m_agents;

    [ClientProperty(ClientVisibility.Private)]
    public List<BuildController2010> Controllers => this.m_controllers;

    [ClientProperty(ClientVisibility.Private)]
    public List<BuildServiceHost2010> ServiceHosts => this.m_serviceHosts;
  }
}
