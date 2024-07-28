// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildQueueQueryResult2008
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal)]
  [XmlType("BuildQueueQueryResult")]
  public class BuildQueueQueryResult2008 : QueryResult2010
  {
    private List<QueuedBuild2008> m_builds = new List<QueuedBuild2008>();
    private List<BuildAgent2008> m_agents = new List<BuildAgent2008>();
    private List<BuildDefinition2010> m_definitions = new List<BuildDefinition2010>();

    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Private)]
    public List<QueuedBuild2008> Builds => this.m_builds;

    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Private)]
    public List<BuildDefinition2010> Definitions => this.m_definitions;

    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Private)]
    public List<BuildAgent2008> Agents => this.m_agents;
  }
}
