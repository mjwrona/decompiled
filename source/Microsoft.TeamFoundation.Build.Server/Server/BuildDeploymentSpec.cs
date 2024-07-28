// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDeploymentSpec
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Public)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildDeploymentSpec : IValidatable
  {
    private string m_fullPath;
    private string m_path;
    private string m_teamProjectName;

    public BuildDeploymentSpec()
    {
      this.DefinitionPath = string.Empty;
      this.MaxDeployments = int.MaxValue;
      this.QueryOrder = BuildQueryOrder.StartTimeAscending;
      this.DeploymentStatus = BuildStatus.All;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string DefinitionPath
    {
      get => this.m_fullPath;
      set => this.m_fullPath = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string EnvironmentName { get; set; }

    [XmlAttribute]
    [DefaultValue(2147483647)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public int MaxDeployments { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public DateTime MinFinishTime { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public DateTime MaxFinishTime { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public BuildStatus DeploymentStatus { get; set; }

    [XmlAttribute]
    [DefaultValue(BuildQueryOrder.StartTimeAscending)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public BuildQueryOrder QueryOrder { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string RequestedFor { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string TeamProject
    {
      get => this.m_teamProjectName;
      set => this.m_teamProjectName = value;
    }

    [XmlIgnore]
    internal TeamFoundationIdentity RequestedForIdentity { get; set; }

    [XmlIgnore]
    internal string Path => this.m_path;

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      if (!string.IsNullOrWhiteSpace(this.m_fullPath))
        BuildPath.SplitTeamProject(this.m_fullPath, out this.m_teamProjectName, out this.m_path);
      if (this.MinFinishTime < DBHelper.MinAllowedDateTime)
        this.MinFinishTime = DBHelper.MinAllowedDateTime;
      if (this.MaxFinishTime < DBHelper.MinAllowedDateTime || this.MaxFinishTime > DBHelper.MaxAllowedDateTime)
        this.MaxFinishTime = DBHelper.MaxAllowedDateTime;
      if (this.MaxDeployments < 0)
        this.MaxDeployments = int.MaxValue;
      this.RequestedForIdentity = Validation.ResolveIdentity(requestContext, this.RequestedFor);
    }
  }
}
