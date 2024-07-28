// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDeployment
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Public)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public class BuildDeployment
  {
    private List<PropertyValue> m_properties = new List<PropertyValue>();

    public BuildDeployment()
    {
    }

    internal BuildDeployment(
      Guid projectId,
      BuildSummary deployment,
      BuildSummary source,
      string deploymentDefinitionName,
      string sourceGetVersion)
    {
      this.ProjectId = projectId;
      this.Deployment = deployment;
      this.Source = source;
      this.DeploymentDefinitionName = deploymentDefinitionName;
      this.SourceGetVersion = sourceGetVersion;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public BuildSummary Deployment { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public BuildSummary Source { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string DeploymentDefinitionName { get; set; }

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string WebsiteUrl { get; set; }

    [XmlIgnore]
    internal string SourceGetVersion { get; set; }

    [XmlIgnore]
    internal Guid ProjectId { get; set; }

    [ClientProperty(ClientVisibility.Internal, PropertyName = "InternalProperties")]
    public List<PropertyValue> Properties => this.m_properties;
  }
}
