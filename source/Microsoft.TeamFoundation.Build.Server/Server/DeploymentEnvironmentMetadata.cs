// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DeploymentEnvironmentMetadata
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Public)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public class DeploymentEnvironmentMetadata : IValidatable
  {
    public DeploymentEnvironmentMetadata()
    {
    }

    public DeploymentEnvironmentMetadata(
      string name,
      string teamProject,
      string connectedServiceName,
      DeploymentEnvironmentKind kind,
      string friendlyName,
      string description)
    {
      this.Name = name;
      this.TeamProject = teamProject;
      this.ConnectedServiceName = connectedServiceName;
      this.Kind = kind;
      this.FriendlyName = friendlyName;
      this.Description = description;
    }

    protected DeploymentEnvironmentMetadata(DeploymentEnvironmentMetadata copy)
    {
      this.Name = copy.Name;
      this.TeamProject = copy.TeamProject;
      this.ConnectedServiceName = copy.ConnectedServiceName;
      this.FriendlyName = copy.FriendlyName;
      this.Description = copy.Description;
      this.Kind = copy.Kind;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string Name { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string TeamProject { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string ConnectedServiceName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string FriendlyName { get; set; }

    [ClientProperty(ClientVisibility.Public)]
    public string Description { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public DeploymentEnvironmentKind Kind { get; set; }

    void IValidatable.Validate(
      IVssRequestContext requestContext,
      ValidationContext validationContext)
    {
      this.Validate(requestContext, validationContext);
    }

    internal void Validate(IVssRequestContext requestContext, ValidationContext validationContext)
    {
      ArgumentValidation.Check("Name", this.Name, false, (string) null);
      ArgumentValidation.Check("TeamProject", this.TeamProject, false, (string) null);
    }
  }
}
