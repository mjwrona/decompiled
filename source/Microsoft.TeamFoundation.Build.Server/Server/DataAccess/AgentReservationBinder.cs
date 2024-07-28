// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.AgentReservationBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class AgentReservationBinder : BuildObjectBinder<AgentReservation>
  {
    private SqlColumnBinder reservationId = new SqlColumnBinder("ReservationId");
    private SqlColumnBinder reservationTime = new SqlColumnBinder("ReservationTime");
    private SqlColumnBinder controllerId = new SqlColumnBinder("ControllerId");
    private SqlColumnBinder buildId = new SqlColumnBinder("BuildId");
    private SqlColumnBinder reservedAgentId = new SqlColumnBinder("ReservedAgentId");
    private SqlColumnBinder possibleAgents = new SqlColumnBinder("PossibleAgents");

    protected override AgentReservation Bind()
    {
      AgentReservation agentReservation = new AgentReservation();
      agentReservation.Id = this.reservationId.GetInt32((IDataReader) this.Reader);
      agentReservation.ReservationTime = this.reservationTime.GetDateTime((IDataReader) this.Reader);
      agentReservation.ControllerUri = this.controllerId.GetArtifactUriFromInt32(this.Reader, "Controller", false);
      agentReservation.BuildUri = this.buildId.GetArtifactUriFromInt32(this.Reader, "Build", false);
      agentReservation.ReservedAgentUri = this.reservedAgentId.GetArtifactUriFromInt32(this.Reader, "Agent", true);
      agentReservation.PossibleAgents.AddRange(this.possibleAgents.XmlToListOfString(this.Reader).Select<string, string>((System.Func<string, string>) (x => DBHelper.CreateArtifactUri("Agent", x))));
      return agentReservation;
    }
  }
}
