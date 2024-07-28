// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ConnectedServiceCreationData
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core
{
  [ClassVisibility(ClientVisibility.Public)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Framework")]
  public class ConnectedServiceCreationData
  {
    public ConnectedServiceCreationData()
    {
    }

    public ConnectedServiceCreationData(
      string name,
      string teamProject,
      ConnectedServiceKind kind,
      string friendlyName,
      string description,
      string serviceUri,
      string endpoint,
      string credentialsXml)
    {
      this.ServiceMetadata = new ConnectedServiceMetadata(name, teamProject, kind, friendlyName, description, serviceUri);
      this.Endpoint = endpoint;
      this.CredentialsXml = credentialsXml;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public ConnectedServiceMetadata ServiceMetadata { get; set; }

    [XmlAttribute(DataType = "anyURI")]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string Endpoint { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string CredentialsXml { get; set; }

    internal void Validate()
    {
      ArgumentUtility.CheckForNull<ConnectedServiceMetadata>(this.ServiceMetadata, "ServiceMetadata");
      ArgumentUtility.CheckStringForNullOrEmpty(this.ServiceMetadata.Name, "ServiceMetadata.Name");
      ArgumentUtility.CheckStringForNullOrEmpty(this.ServiceMetadata.TeamProject, "TeamProject");
      ArgumentUtility.CheckStringForNullOrEmpty(this.Endpoint, "Endpoint");
      ArgumentUtility.CheckStringForNullOrEmpty(this.CredentialsXml, "CredentialsXml");
    }
  }
}
