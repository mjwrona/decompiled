// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.SharedResourceRequest
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class SharedResourceRequest
  {
    internal int ResourceId { get; set; }

    [ClientType(typeof (Uri))]
    [XmlAttribute(DataType = "anyURI")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string RequestedBy { get; set; }

    [ClientType(typeof (Uri))]
    [XmlAttribute(DataType = "anyURI")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string RequestedByBuild { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public DateTime RequestedOn { get; set; }
  }
}
