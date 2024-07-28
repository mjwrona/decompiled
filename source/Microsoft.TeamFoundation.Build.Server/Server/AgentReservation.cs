// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.AgentReservation
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Private)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class AgentReservation
  {
    private List<string> m_possibleAgents = new List<string>();

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int Id { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime ReservationTime { get; set; }

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ControllerUri { get; set; }

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string BuildUri { get; set; }

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string ReservedAgentUri { get; set; }

    [ClientType(typeof (Uri[]))]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public List<string> PossibleAgents => this.m_possibleAgents;

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[AgentReservation Id={0} ReservationTime={1} ControllerUri={2} BuildUri={3} ReservedAgentUri={4}]", (object) this.Id, (object) this.ReservationTime, (object) this.ControllerUri, (object) this.BuildUri, (object) this.ReservedAgentUri);
  }
}
