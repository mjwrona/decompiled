// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.RequestedForDisplayInformation
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Public)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public class RequestedForDisplayInformation
  {
    public RequestedForDisplayInformation()
    {
    }

    internal RequestedForDisplayInformation(Guid tfid, string displayName)
    {
      this.TeamFoundationId = tfid;
      this.DisplayName = displayName;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public Guid TeamFoundationId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string DisplayName { get; set; }
  }
}
