// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ConnectedServiceMetadata
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core
{
  [ClassVisibility(ClientVisibility.Public)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Framework")]
  public class ConnectedServiceMetadata
  {
    public ConnectedServiceMetadata() => this.Kind = ConnectedServiceKind.AzureSubscription;

    public ConnectedServiceMetadata(
      string name,
      string teamProject,
      ConnectedServiceKind kind,
      string friendlyName,
      string description,
      string serviceUri)
    {
      this.Name = name;
      this.TeamProject = teamProject;
      this.Kind = kind;
      this.FriendlyName = friendlyName;
      this.Description = description;
      this.ServiceUri = serviceUri;
    }

    internal ConnectedServiceMetadata(ConnectedServiceMetadata copy)
    {
      this.Name = copy.Name;
      this.TeamProject = copy.TeamProject;
      this.FriendlyName = copy.FriendlyName;
      this.Description = copy.Description;
      this.Kind = copy.Kind;
      this.ServiceUri = copy.ServiceUri;
      this.AuthenticatedBy = copy.AuthenticatedBy;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string Name { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string TeamProject { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string FriendlyName { get; set; }

    [ClientProperty(ClientVisibility.Public)]
    public string Description { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public ConnectedServiceKind Kind { get; set; }

    [XmlAttribute(DataType = "anyURI")]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public)]
    public string ServiceUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public Guid AuthenticatedBy { get; set; }
  }
}
