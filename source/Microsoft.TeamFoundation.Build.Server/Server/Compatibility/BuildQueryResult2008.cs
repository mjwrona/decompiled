// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildQueryResult2008
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal)]
  [XmlType("BuildQueryResult")]
  public sealed class BuildQueryResult2008
  {
    private List<Failure2010> m_failures = new List<Failure2010>();
    private List<BuildAgent2008> m_agents = new List<BuildAgent2008>();
    private List<BuildDefinition2010> m_definitions = new List<BuildDefinition2010>();
    private StreamingCollection<BuildDetail2010> m_builds = new StreamingCollection<BuildDetail2010>();

    [XmlElement(Order = 1, Type = typeof (List<BuildDefinition2010>))]
    [ClientProperty(ClientVisibility.Private)]
    public List<BuildDefinition2010> Definitions => this.m_definitions;

    [XmlElement(Order = 2, Type = typeof (StreamingCollection<BuildDetail2010>))]
    [ClientProperty(ClientVisibility.Private)]
    public StreamingCollection<BuildDetail2010> Builds => this.m_builds;

    [XmlElement(Order = 3, Type = typeof (List<BuildAgent2008>))]
    [ClientProperty(ClientVisibility.Private)]
    public List<BuildAgent2008> Agents => this.m_agents;

    [XmlElement(Order = 4, Type = typeof (List<Failure2010>))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Private, PropertyName = "InternalFailures")]
    public List<Failure2010> Failures => this.m_failures;
  }
}
