// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Location.ConnectionData
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core.Location
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class ConnectionData
  {
    private ConnectionData()
    {
    }

    public ConnectionData(
      Guid instanceId,
      Guid catalogResourceId,
      TeamFoundationIdentity authenticatedUser,
      TeamFoundationIdentity authorizedUser,
      string webApplicationRelativeDirectory,
      LocationServiceData locationServiceData,
      Microsoft.TeamFoundation.Server.Core.Location.ServerCapabilities serverCapabilities,
      string serverVersion)
    {
      this.AuthenticatedUser = authenticatedUser;
      this.AuthorizedUser = authorizedUser;
      this.LocationServiceData = locationServiceData;
      this.InstanceId = instanceId;
      this.CatalogResourceId = catalogResourceId;
      this.WebApplicationRelativeDirectory = webApplicationRelativeDirectory;
      this.ServerCapabilities = (int) serverCapabilities;
      this.ServerVersion = serverVersion;
    }

    public TeamFoundationIdentity AuthenticatedUser { get; set; }

    public TeamFoundationIdentity AuthorizedUser { get; set; }

    public LocationServiceData LocationServiceData { get; set; }

    [XmlAttribute]
    public Guid InstanceId { get; set; }

    [XmlAttribute]
    public Guid CatalogResourceId { get; set; }

    [XmlAttribute]
    public string WebApplicationRelativeDirectory { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public int ServerCapabilities { get; set; }

    [XmlAttribute]
    public string ServerVersion { get; set; }
  }
}
